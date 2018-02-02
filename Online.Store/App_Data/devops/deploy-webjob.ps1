<#
.SYNOPSIS
Deployes a WebJob to an App Service. Builds dynamically the app setting,
Builds, publishes and Zips the artifacts. Finally uploads the Zip to App Service.

.Author: Christos Sakellarios

.PARAMETER PrimaryDatabaseServer
The Database Server with read/write permissions

.PARAMETER Database
The database that the WebJob writes

.PARAMETER SqlServerLogin
SQL Logical Server's admin username.

.PARAMETER SqlServerPassword
SQL Logical Server's admin password.


.PARAMETER WebappParentResourceGroup
App Service's parent resource group to get the DocumentDB keys

.PARAMETER WebappResourceGroup
App Service Resource Group & Name

.PARAMETER WebjobAppLocation
Local path of the Online.Store.WebJob project
e.g. "C:\workspace\chsakell\planet-scale-azure\Online.Store.WebJob"

.PARAMETER slot
The slot to deploy the webjob. If omitted then production is used

#>
param (
    [Parameter(Mandatory = $true)] [string] $PrimaryDatabaseServer,
    [Parameter(Mandatory = $true)] [string] $Database,
    [Parameter(Mandatory = $true)] [string] $SqlServerLogin,
    [Parameter(Mandatory = $true)] [string] $SqlServerPassword,
    [Parameter(Mandatory = $true)] [string] $WebappParentResourceGroup,
    [Parameter(Mandatory = $true)] [string] $WebappResourceGroup,
    [Parameter(Mandatory = $true)] [string] $WebjobAppLocation,
    [Parameter(Mandatory = $true)] [string] $DeploymentDestinationFolder,
    [Parameter(Mandatory = $false)] [string] $slot
)

Clear-Host
# https://docs.microsoft.com/en-us/azure/app-service/web-sites-create-web-jobs

# prefixes
$appServicePlanPrefix = "plan";
$sqlServerPrefix = "sql";
$cosmosDbPrefix = "cosmosdb";
$storagePrefix = "storage";
$serviceBusPrefix = "servicebus";
$redisCachePrefix = "rediscache";
$searchServicePrefix = "search";

$serviceBusNameSpace = "$WebappParentResourceGroup-$serviceBusPrefix";
$webAppName = "$WebappResourceGroup";
$queueName = "orders"

$readAccessKey = (Get-AzureRmServiceBusKey -ResourceGroup  $WebappParentResourceGroup `
     -Namespace $serviceBusNameSpace -Queue $queueName -Name "read").PrimaryKey

$webjobAppSettings = @{
  "ConnectionStrings" = @{
    "DefaultConnection" = "Server=tcp:$PrimaryDatabaseServer.database.windows.net,1433;Initial Catalog=$Database;Persist Security Info=False;User ID=$SqlServerLogin;Password=$SqlServerPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  };
  "ServiceBus:Namespace" = "$serviceBusNameSpace";
  "ServiceBus:Queue" = "$queueName";
  "ServiceBus:ReadAccessKeyName" = "read";
  "ServiceBus:ReadAccessKey" = "$readAccessKey";
}

$webjobAppSettings = ConvertTo-Json $webjobAppSettings -Depth 2


# Update settings in WebJob project..
$webjobAppSettingsLocation = "$WebjobAppLocation\appsettings.json";
Set-Content -Path $webjobAppSettingsLocation -Value $webjobAppSettings -Encoding Unicode

# Build WebJob project..
dotnet publish "$WebjobAppLocation\Online.Store.WebJob.csproj" -c Release

# Zip publish folder..
$publishFolder = "$WebjobAppLocation\bin\Release\netcoreapp2.0\publish"
$currentDateTime =  (Get-Date -Format s).Replace(":","-");
$deploymentFolder = "$DeploymentDestinationFolder\$WebappResourceGroup-$currentDateTime";
Write-Host "Deployment folder: $deploymentFolder";

if(!(Test-Path -Path $deploymentFolder )){
    New-Item -ItemType directory -Path $deploymentFolder
    Write-Host "Deployment folder created"
}
else
{
  Write-Host "Deployment folder already exists"
}
$zipFile = "$deploymentFolder\orders.zip"

 If(Test-path $zipFile) { 
    Remove-item $zipFile
    
    # Sleep for 10 seconds..
    Start-Sleep -s 10   
 }

# Zip the file
# Be carefull to zip only the files, not the folder
Write-Host "Zipping file.."
Compress-Archive -Path $publishFolder\* -DestinationPath $zipFile
Start-Sleep -s 10

# Start WebJob deployment 
# https://github.com/projectkudu/kudu/wiki/Deploying-a-WebJob-using-PowerShell-ARM-Cmdlets
$Apiversion = "2015-08-01"
$webjobName = "orders"

#Function to get Publishing credentials for the WebApp :
function Get-PublishingProfileCredentials($resourceGroupName, $webAppName, $slot) {

    $publishProfile = Get-AzureRmWebAppSlotPublishingProfile -ResourceGroupName "$resourceGroupName" `
    -Name "$webappName" -Format "WebDeploy" -Slot "$slot"

    return $publishProfile;
}

#Pulling authorization access token :
#Pulling authorization access token :
function Get-KuduApiAuthorisationHeaderValue($resourceGroupName, $webAppName, $slot) {
    $publishProfile = Get-PublishingProfileCredentials $WebappResourceGroup $webAppName $slot
    $xml = [xml]$publishProfile
    $msDeployNode = $xml | Select-XML -XPath "//*[@publishMethod='MSDeploy']"

    $publishUrl = $msDeployNode.Node.publishUrl
    Write-Host $publishUrl
    $userName = $msDeployNode.Node.userName
    $password = $msDeployNode.Node.userPWD
    return ("Basic {0}" -f [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $userName, $password))))
}

$apiUrl = "https://$webAppName.scm.azurewebsites.net/api/continuouswebjobs/$webjobName" # WebJob Type: continous/triggered

if($slot -and $slot -ne "production") {
    $apiUrl = "https://$webAppName-$slot.scm.azurewebsites.net/api/continuouswebjobs/$webjobName" # WebJob Type: continous/triggered
}

if(!$slot) {
  $slot = "production";
}

$accessToken = Get-KuduApiAuthorisationHeaderValue $WebappResourceGroup $webAppname $slot

#Generating header to create and publish the Webjob :
$Header = @{
    'Content-Disposition'='attachment; attachment; filename=Orders.zip'
    'Authorization'=$accessToken
}

# Webjob uploading
# https://github.com/projectkudu/kudu/wiki/WebJobs-API
Write-Host "Uploading WebJob..."


Write-Host "API URL: $apiUrl" 

$result = Invoke-RestMethod -Uri $apiUrl -Headers $Header -Method put -InFile "$zipFile" -ContentType 'application/zip' 
#NOTE: Update the above script with the parameters highlighted and run in order to push a new Webjob under the specified WebApp.
Write-Host "WebJob uploaded.."

# Send a beep
[console]::beep(1000,500)