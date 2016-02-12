#!/bin/sh

curl --verbose -X PUT -H "Content-Type: application/json" -d '{"Key":"'$1'","Data":"'$2'"}' http://localhost:8888/value/$1
curl --verbose -X PUT -H "Content-Type: application/json" -d '{"Key":"'$1'_Other","Data":"'$2'. Some other data"}' http://localhost:8888/value/$1'_Other'

curl --verbose -X GET  -H "Content-Type: application/json" http://localhost:8888/value/$1
curl --verbose -X GET  -H "Content-Type: application/json" http://localhost:8888/value/$1'_Other'

curl --verbose -X GET  -H "Content-Type: application/json" http://localhost:8888/value/$1'_Missing'

curl --verbose -X DELETE  -H "Content-Type: application/json" http://localhost:8888/value/$1'_Other'
curl --verbose -X GET  -H "Content-Type: application/json" http://localhost:8888/value/$1'_Other'
curl --verbose -X GET  -H "Content-Type: application/json" http://localhost:8888/value/$1

