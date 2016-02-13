#!/bin/sh

# args: server_id
function server_url {
  echo "http://loalhost:"$((8000+$1))"/"
}

# args: server_id, value_id
function value_url {
  echo "$(server_url $1)value/"$2
}

# args: server_id, value_id, data
function put_value {
  curl --verbose -X PUT -H "Content-Type: application/json" -d '{"Key":"'$2'","Data":"'$3'"} $(value_url $1 $2)'
}

# args: server_id, value_id
function get_value {
  curl --verbose -H "Content-Type: application/json $(value_url $1 $2)"
}

# args: server_id, value_id
function delete_value {
  curl --verbose -X DELETE -H "Content-Type: application/json $(value_url $1 $2)"
}

rm -r test-bin
mkdir -p test-bin

count=1
for i in `seq 0 $count`; do
  cp -r server/bin/Debug test-bin/Debug$i
  #pipe[$i] = test-bin/pipe$i
  #mkfifo $pipe[$i]
  
  if [[ $i == 0 ]] ; then
    (cd test-bin/Debug$i; (sleep 5; echo "e") | PUBLIC_URL="$(server_url $i)" mono server.exe) &
  else
    (cd test-bin/Debug$i; (sleep 3; echo "e") | PUBLIC_URL="$(server_url $i)" PARENT_URL="$(server_url $((i-1)))" mono server.exe) &
  fi
done
wait;
