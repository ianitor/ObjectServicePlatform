# General

OspTool is a command line tool to configure Object Service Platform. Osp Tool is available for Windows, Linux and MacOS.


## Configuration

First, the Core Service, Identity Service and Job Service URI has to be defined. For many commands a default tenant id can be defined.

~~~
.\OspTool -c Config -csu "https://core.staging.salzburgdev.at/" -isu "https://connect.staging.salzburgdev.at/" -jsu "https://jobs.staging.salzburgdev.at/" -tid "paketservice"
.\OspTool -c Config -csu "https://core.production.salzburgdev.at/" -isu "https://connect.production.salzburgdev.at/" -jsu "https://jobs.production.salzburgdev.at/" -tid "paketservice"
.\OspTool -c Config -csu "https://localhost:5001/" -isu "https://localhost:5003/" -jsu "https://localhost:5009/" -tid "cd"
~~~

## Log-In
~~~
.\OspTool -c Login -i
~~~

## Import Construction Kit

Possible scopes/layer hierarchy: Application, Layer2, Layer3, Layer4

A layer can have dependencies to definitions of the same or superior layer 

~~~
.\OspTool -c ImportCk -s Application -f "D:\Development\PaketService\Backend\Persistence\PaketServiceConstructionKit.json" -w
.\OspTool -c ImportCk -s Application -f /Users/gerald/RiderProjects/PaketService/Backend/Persistence/PaketServiceConstructionKit.json -w


.\OspTool -c ImportCk -s Application -f /Users/gerald/RiderProjects/CdDataModelling/SgMetadata/CdScadaConstructionKit.json -w
.\OspTool -c ImportCk -s Application -f D:\Source\CdDataModelling\SgMetadata\CdScadaConstructionKit.json -w
.\OspTool -c ImportCk -s Layer2 -f  /Users/gerald/RiderProjects/CdDataModelling/SgMetadata/CdFnBConstructionKit.json -w
.\OspTool -c ImportCk -s Layer2 -f D:\Source\CdDataModelling\SgMetadata\CdFnBConstructionKit.json -w
~~~
## Import Runtime data
~~~
.\OspTool -c ImportRt -f "D:\rtmodel.zip" -w
.\OspTool -c ImportRt -f "/Users/gerald/RiderProjects/rtmodel.zip" -w
.\OspTool -c ImportRt -f "/Users/gerald/RiderProjects/PaketService/Deployment/data/all_accounting-rules.zip" -w
.\OspTool -c ImportRt -f "D:\Development\PaketService\Deployment\data\all_accounting-rules.zip" -w
.\OspTool -c ImportRt -f "/Users/gerald/RiderProjects/PaketService/Deployment/data/all_notification_templates.zip" -w
.\OspTool -c ImportRt -f "D:\Development\PaketService\Deployment\data\all_notification_templates.zip" -w
.\OspTool -c ImportRt -f "/Users/gerald/RiderProjects/PaketService/Deployment/data/servicehooks.zip" -w

~~~
## Tenant management
~~~
.\OspTool -c Delete -tid "paketservice"
.\OspTool -c Create -tid "paketservice" -db "paketservice"
~~~
## Client management
~~~
.\osptool -c AddClientCredentialsClient --clientid smsGateway --secret "{636B17DB-7155-4A73-8329-D00059D83CAC}"  --name "Paket-Service.eu SMS Gateway"
.\osptool -c AddClientCredentialsClient --clientid paketServiceSystem --secret "{FF31A4CE-8850-4FB0-B8DD-E0269B2C14E7}"  --name "Paket-Service.eu System"
.\osptool -c updateclient --clienturi https://core.staging.salzburgdev.at --clientid osp-coreServices --name "Open Object Platform Developer Tools" --redirectUri "https://core.staging.salzburgdev.at/signin-oidc"
.\osptool -c AddAuthorizationCodeClient --clienturi https://paketservice.staging.salzburgdev.at/ --clientid paketServiceClient --name "Paket-Service.eu Frontend"
.\osptool -c AddAuthorizationCodeClient --clienturi https://paketservice.production.salzburgdev.at/ --clientid paketServiceClient --name "Paket-Service.eu Frontend"
.\osptool -c AddAuthorizationCodeClient --clienturi https://localhost:5007/ --clientid paketServiceClient --name "Paket-Service.eu"
~~~
