# System requirements

* .NET Core 3.1 SDK
* Visual Studio 2019 (or newer) OR Jetbrains Rider. Other development environments may be supported but not actively used.
* A prepared Angular development environment (https://angular.io/guide/setup-local)
* REDIS server is available in PATH-Variable.
* MongoDB has to be installed on the local system

# Step-By-Step

To run OSP on a local system docker is **not** needed. Beside a correct configuration of MongoDB, also secrets has to defined to run OSP.

## MongoDB

MongoDB has to be configured to use authentication and replica sets has to be enabled to support transactions. 
[See here to configure MongoDB](configureMongodb.md).


## User secrets

For secrets, user secrets of .NET are used. Hint: For Rider, there is a Plug-In existing to define user secrets.

OSP needs credentials to access MongoDB. There are two accounts needed:
1) Admin account: osp-system-admin, with a password defined as user secret. For debugging systems, the password "OspAdmin1" is used. This account is used by OSP to create new databases.
2) Data source accounts for every database: osp-system-ds-user-{0}, there {0} is the name of the database in mongodb.

Fore data source accounts, the new user is created during creating a new data source. 

All services need the passwords provided by user secrets in debugging systems.

### Ianitor.Osp.Backend.CoreServices
~~~
{
  "OspSystem:AdminUserPassword": "OspAdmin1",
  "OspSystem:DatabaseUserPassword": "OspUser1"
}
~~~

### Ianitor.Osp.Backend.Identity

~~~
{
  "OspSystem:AdminUserPassword": "OspAdmin1",
  "OspSystem:DatabaseUserPassword": "OspUser1"
}
~~~

### Ianitor.Osp.Backend.JobServices

~~~
{
  "OspSystem:AdminUserPassword": "OspAdmin1",
  "OspSystem:DatabaseUserPassword": "OspUser1"
}
~~~

## Ianitor.Osp.Backend.Policy

~~~
{
  "OspSystem:AdminUserPassword": "OspAdmin1",
  "OspSystem:DatabaseUserPassword": "OspUser1"
}
~~~

## Redis
On Linux/Mac redis-server can be installed by APT or BREW. On Windows, use WSL2 and a PowerShell script as available [here.](../samples/redis-server.ps1). Important: By using command redis-server in any directory, redis-server have to be available.

## Build and start services

After installing the system requirements, run the script [buildAndstartservices.ps1](../buildAndstartservices.ps1) to build and start all services and the dashboard.
Please ensure, that in file buildAndstartservices.ps1 all projects are comment in.

After build, following services are available at following ports:
* https://localhost:5001: Core Services
* https://localhost:5003: Identity Services
* https://localhost:5005: Dashboard
* https://localhost:5009: Job Services

## Creating first user account

Login to https://localhost:5003 to create the first admin user to access the system. Continue with the Dashboard to create a new data source.

## Next steps

*) [Command line utility - OspTool](OspTool.md)
*) [Graph QL samples](GraphQLSamples.md)
*) [Npm](Npm.md)
