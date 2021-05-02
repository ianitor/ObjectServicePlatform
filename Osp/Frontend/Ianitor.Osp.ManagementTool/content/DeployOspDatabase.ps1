$basedir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$OspTool = Join-Path $basedir 'OspTool\OspTool.exe'

# Define data source and database
$ds = "(local)\<define instance here>"
$db = "<define database here>"

# Create database, import custom ck model and basic rt model
& $OspTool -c delete -ds $ds -db $db
& $OspTool -c create -ds $ds -db $db
& $OspTool -c importck -ds $ds -db $db -f "ConstructionKitModel.json"
& $OspTool -c importck -ds $ds -db $db -f "RuntimeModel.json"
