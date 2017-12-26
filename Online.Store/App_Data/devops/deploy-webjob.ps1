# https://docs.microsoft.com/en-us/azure/app-service/web-sites-create-web-jobs
#Resource details :
$commonResourcesName = "webapp-resource-group";
$databaseName = "planetscalestore";
$adminLogin = "your-sql-admin-login";
$adminPassword = "your-sql-admin-password";
$queueName = "orders"
$parentResourceGroup = "parent-resource-group";
$readAccessKey = (Get-AzureRmServiceBusKey -ResourceGroup  $parentResourceGroup `
     -Namespace $serviceBusNameSpace -Queue $queueName -Name "read").PrimaryKey

$webjobAppSettings = @{
  "ConnectionStrings" = @{
    "DefaultConnection" = "Server=tcp:$commonResourcesName.database.windows.net,1433;Initial Catalog=$databaseName;Persist Security Info=False;User ID=$adminLogin;Password=$adminPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  };
  "ServiceBus:Namespace" = "$parentResourceGroup";
  "ServiceBus:Queue" = "$queueName";
  "ServiceBus:ReadAccessKeyName" = "read";
  "ServiceBus:ReadAccessKey" = "$readAccessKey";
}

$webjobAppSettings = ConvertTo-Json $webjobAppSettings -Depth 2

$webjobAppLocation = "C:\workspace\chsakell\planet-scale-azure\Online.Store.WebJob";
# Update settings in WebJob project..
$webjobAppSettingsLocation = "$webjobAppLocation\appsettings.json";
Set-Content -Path $webjobAppSettingsLocation -Value $webjobAppSettings -Encoding Unicode

# Build WebJob project..
dotnet publish "$webjobAppLocation\Online.Store.WebJob.csproj" -c Release

# Zip publish folder..
$publishFolder = "$webjobAppLocation\bin\Release\netcoreapp2.0\publish"
$deploymentFolder = "$publishFolder\deployment"
if(!(Test-Path -Path $deploymentFolder )){
    New-Item -ItemType directory -Path $deploymentFolder
    Write-Host "Deployment folder created"
}
else
{
  Write-Host "Deployment folder already exists"
}
$zipFile = "$deploymentFolder\orders.zip"

 If(Test-path $zipFile) { Remove-item $zipFile }

# Zip the file
# Be carefull to zip only the files, not the folder
Write-Host "Zipping file.."
Compress-Archive -Path $publishFolder\* -DestinationPath $zipFile

# Start WebJob deployment 
# https://github.com/projectkudu/kudu/wiki/Deploying-a-WebJob-using-PowerShell-ARM-Cmdlets
$Apiversion = "2015-08-01"
$webjobName = "orders"

#Function to get Publishing credentials for the WebApp :
function Get-PublishingProfileCredentials($resourceGroupName, $webAppName) {
    $resourceType = "Microsoft.Web/sites/config"
    $resourceName = "$webAppName/publishingcredentials"
    $publishingCredentials = Invoke-AzureRmResourceAction -ResourceGroupName $resourceGroupName `
     -ResourceType $resourceType -ResourceName $resourceName -Action list -ApiVersion $Apiversion -Force
       return $publishingCredentials
}

#Pulling authorization access token :
function Get-KuduApiAuthorisationHeaderValue($resourceGroupName, $webAppName) {
    $publishingCredentials = Get-PublishingProfileCredentials $resourceGroupName $webAppName
    return ("Basic {0}" -f [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $publishingCredentials.Properties.PublishingUserName, $publishingCredentials.Properties.PublishingPassword))))
}

$accessToken = Get-KuduApiAuthorisationHeaderValue $resourceGroupName $webAppname

#Generating header to create and publish the Webjob :
$Header = @{
    'Content-Disposition'='attachment; attachment; filename=Orders.zip'
    'Authorization'=$accessToken
}

# Webjob uploading
# https://github.com/projectkudu/kudu/wiki/WebJobs-API
Write-Host "Uploading WebJob..."
$apiUrl = "https://$webAppName.scm.azurewebsites.net/api/continuouswebjobs/$webjobName" # WebJob Type: continous/triggered
$result = Invoke-RestMethod -Uri $apiUrl -Headers $Header -Method put -InFile "$zipFile" -ContentType 'application/zip' 
#NOTE: Update the above script with the parameters highlighted and run in order to push a new Webjob under the specified WebApp.
Write-Host "WebJob uploaded.."