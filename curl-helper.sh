#!/bin/sh

# args: server_id
function server_url {
  echo "http://localhost:"$((8000+$1))"/"
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

function todo {
  rm -r test-bin
  mkdir -p test-bin

  count=1
  for i in `seq 0 $count`; do
    cp -r server/bin/Debug test-bin/Debug$i
    pipe+=("pipe$i")
    
    mkfifo test-bin/${pipe[$i]}
    
    if [[ $i == 0 ]] ; then
      (cd test-bin/Debug$i; PUBLIC_URL="$(server_url $i)" mono server.exe 0< ../${pipe[$i]} ) &
    else
      (cd test-bin/Debug$i; PUBLIC_URL="$(server_url $i)" PARENT_URL="$(server_url $((i-1)))" mono server.exe 0< ../${pipe[$i]} ) &
    fi
    #printf "" > test-bin/${pipe[$i]}
  done

  echo "Sleep"
  sleep 5

  echo "Echo"
  for i in `seq 0 $count`; do
    echo "e" > test-bin/${pipe[$i]}
  done

  echo "Wait"
  wait;
}

i=$1
rm -r test-bin/Debug$i
mkdir -p test-bin
cp -r server/bin/Debug test-bin/Debug$i
if [[ $i == 0 ]] ; then
  (cd test-bin/Debug$i; PUBLIC_URL="$(server_url $i)" mono server.exe)
else
  (cd test-bin/Debug$i; PUBLIC_URL="$(server_url $i)" PARENT_URL="$(server_url $((i-1)))" mono server.exe)
fi
