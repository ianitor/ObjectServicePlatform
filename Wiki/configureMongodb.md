# Initial setup of mongodb

For development, a mongodb standalone has to be converted to a replica set with one node to support multi-document transactions

https://docs.mongodb.com/manual/tutorial/convert-standalone-to-replica-set/


## Enable replica set

add to mongodb.conf

~~~~
replication:
   replSetName: rs0
~~~~

Other possibility by command line option

~~~~
--replSet rs0
~~~~


## Log-In to mongo-cli
~~~~
 mongo mongodb://osp-system-admin:OspAdmin1@localhost:27017/?authSource=admin
~~~~

## Init cluster
~~~~
rs.initiate()
~~~~

## Add Admin user

use script [create-users.ps1](../Deployment/mongodb/create-users.ps1) to create the admin user first

Afterwards enable authentication
~~~~
security:
    authorization: enabled
~~~~