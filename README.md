# How it works
Start with PARENT_URL environment variable set to parent url (if not set then this is main server).
Server will auto join to dht after local app set up (by sending REST request to parent and child).
Then server can operate.
Server will auto leave dht before shut down (by sending REST request to parent).

# REST Resources

## /value/{key}
Allowed methods: GET, PUT, DELETE


## /dht

### /dht/join/as-child
Allowed methods: POST
It is auto call by server.
Server split its hash range and data set.
Response will also contain child address.

### /dht/join/as-parent
Allowed methods: POST
It is auto call by server.
Server save its new parent

### /dht/leave/as-child
Allowed methods: POST
It is auto call by server.
Server merge hash range and data set.

### /dht/leave/as-parent
Allowed methods: POST
It is auto call by server.
Server save its new parent

# Main TODO
- forward requests test
- prepare AWS/OpenStack deploy

# TODO
- replace DHTServerCtx singleton with dependency injection (HashFunction and DHT classes)
- tests
