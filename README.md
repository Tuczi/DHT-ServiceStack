# How it works
Start with PARENT_URL environment variable set to parent url (if not set then this is main server).
Server will auto join to dht after local app set up (by sending REST request to parent).
Then server can operate.
Server will auto leave dht before shut down (by sending REST request to parent).

# REST Resources

## Value
Allowed methods: GET, PUT, DELETE


## dht/join
Allowed methods: POST
It is auto call by server.
Server split its hash range and data set.
Response will also contain child address.

## dht/leave
Allowed methods: POST
It is auto call by server.
Server merge hash range and data set.

# Main TODO 
- finish auto join/leave
- forward requests
- prepare AWS/OpenStack deploy

# TODO
- replace DHTServerCtx singleton with dependency injection (HashFunction and DHT classes)
- change RangeMin RangeMax type to BigInteger (some RestSharp problem)
- tests
