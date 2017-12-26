
# sign in
Write-Host "Logging in...";
# Login-AzureRmAccount;

# select subscription
$subscriptionId = "YOUR-SUBSCRIPTION-ID";
Write-Host "Selecting subscription";
#Select-AzureRmSubscription -SubscriptionID $subscriptionId;

# Check available locations per resource type
# Get-AzureRmResourceProvider


#####################################################################################################
# Create the sub Resource Group that contains the Web App and SQL Server/database
# Get list of locations and select one.
# Get-AzureRmLocation | select Location 

$version = "a" # switch between a and b
$primaryName = "planetscalestore";
$resourceGroupLocation = "westeurope";
$resourceGroupName = "$primaryName-$resourceGroupLocation-$version";

Get-AzureRmResourceGroup -Name $resourceGroupName -ev notPresent -ea 0

if ($notPresent)
{
    # ResourceGroup doesn't exist
    Write-Host "Trying to create Resource Group: $resourceGroupName "
    New-AzureRmResourceGroup -Name $resourceGroupName -Location $resourceGroupLocation
}
else
{
    # ResourceGroup exist
    Write-Host "Resource Group:  $resourceGroupName  already exists.."
}


#####################################################################################################
# Create an App Service plan in Free tier.
# https://docs.microsoft.com/en-us/powershell/module/azurerm.websites/new-azurermappserviceplan?view=azurermps-5.1.1
$webappName = "$resourceGroupName";

Get-AzureRmAppServicePlan -ResourceGroupName $resourceGroupName -Name $webappName  -ev planNotPresent -ea 0

if ($planNotPresent)
{
    # App Service Plan doesn't exist
    Write-Host "Trying to create App Service Plan $resourceGroupName "
    New-AzureRmAppServicePlan -Name $webappName -Location $resourceGroupLocation `
        -ResourceGroupName $resourceGroupName -Tier Standard -WorkerSize Small
}
else
{
    # App Service Plan exist
    Write-Host "App Service Plan:  $webappName  already exists.."
}
#####################################################################################################
# Create the App Service.
# https://docs.microsoft.com/en-us/azure/app-service/app-service-powershell-samples

$appServiceExists = Test-AzureName -Website $webappName

# Check if the namespace already exists or needs to be created
if ($appServiceExists) {
    # App Service Plan doesn't exist
    Write-Host "App Service $webappName already exists.."
}
else {
    # App Service Plan doesn't exist
    Write-Host "Trying to create App Service $webappName "

    New-AzureRmWebApp -Name $webappName -Location $resourceGroupLocation `
        -AppServicePlan $webappName -ResourceGroupName $resourceGroupName

    Write-Host "App Service $webappName successfully created at $resourceGroupName"
}


#####################################################################################################
# Create the Logical SQL Server and Database
# https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started-powershell

$serverName =  "$resourceGroupName";
# Set an admin login and password for your database
# The login information for the server
$adminLogin = "your-admin-login" 
$password = "your-admin-login-password"

# Create the logical server
# https://docs.microsoft.com/en-us/powershell/module/azurerm.sql/new-azurermsqlserver?view=azurermps-5.1.1
# https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started-powershell

$serverInstance = Get-AzureRmSqlServer -ServerName $serverName -ResourceGroupName $resourceGroupName -ErrorAction SilentlyContinue
if ($serverInstance) { 
    Write-Host "SQL Server $serverName already exists..."
}
else {
    Write-Host "Trying to create SQL Server $serverName.."

    New-AzureRmSqlServer -ResourceGroupName $resourceGroupName `
    -ServerName $serverName `
    -Location $resourceGroupLocation `
    -SqlAdministratorCredentials $(New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $adminLogin, $(ConvertTo-SecureString -String $password -AsPlainText -Force))

    Write-Host "SQL Server $serverName successfully created..."

    # Allow access to Azure Services
    # https://docs.microsoft.com/en-us/powershell/module/azure/new-azuresqldatabaseserverfirewallrule?view=azuresmps-4.0.0
    Write-Host "Allowing access to Azure Services..."

    New-AzureSqlDatabaseServerFirewallRule -ServerName $serverName -AllowAllAzureServices
}

#####################################################################################################
# Create the database
# https://docs.microsoft.com/en-us/powershell/module/azurerm.sql/new-azurermsqldatabase?view=azurermps-5.1.1
# https://docs.microsoft.com/en-us/azure/sql-database/sql-database-service-tiers
# https://docs.microsoft.com/en-us/azure/sql-database/sql-database-what-is-a-dtu
# https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.management.sql.models.database.requestedserviceobjectivename?view=azure-dotnet

$databaseName = "$primaryName";

$database = Get-AzureRmSqlDatabase -ResourceGroupName $resourceGroupName `
 -ServerName $serverName -DatabaseName $databaseName -ErrorAction SilentlyContinue

if ($database) { 
    Write-Host "Azure SQL Database $databaseName already exists..."
}
else {
    Write-Host "Trying to create Azure SQL Database $databaseName at Server $serverName.."

    New-AzureRmSqlDatabase  -ResourceGroupName $resourceGroupName `
    -ServerName $serverName `
    -DatabaseName $databaseName `
    -RequestedServiceObjectiveName "Basic" `
    -MaxSizeBytes 524288000

    Write-Host "Azure SQL Database $databaseName successfully created..."

}

#####################################################################################################
# Set App Service settings
$primaryResourceGroupName = "planetscalestore";
$documentDbDatabase = "planetscalestore"
$region = "$resourceGroupLocation-$version";
# Retrieve the primary account keys
$docDbPrimaryMasterKey = Invoke-AzureRmResourceAction -Action listKeys `
    -ResourceType "Microsoft.DocumentDb/databaseAccounts" `
    -ApiVersion "2015-04-08" `
    -ResourceGroupName $primaryResourceGroupName `
    -Name $documentDbDatabase | select -ExpandProperty  primaryMasterKey

# Get Storage Account Key
$storageAccountKey = (Get-AzureRmStorageAccountKey -ResourceGroupName $primaryResourceGroupName `
 -AccountName $primaryResourceGroupName).Value[0] 

$parentResourceGroup = "$primaryName-$resourceGroupLocation"
# Get Redis Cache Key
$cacheName = "$primaryName-$resourceGroupLocation"
$cachePrimaryKey = (Get-AzureRmRedisCacheKey -Name $cacheName -ResourceGroupName $parentResourceGroup).PrimaryKey

# Get Service Bus Queue orders Write Rule Key
# https://docs.microsoft.com/en-us/powershell/module/azurerm.servicebus/Get-AzureRmServiceBusKey?view=azurermps-5.1.1
$queueName = "orders"
$serviceBusNameSpace = "$primaryName-" + $resourceGroupLocation;
$writeAccessKey = (Get-AzureRmServiceBusKey -ResourceGroup  $parentResourceGroup `
     -Namespace $serviceBusNameSpace -Queue $queueName -Name "write").PrimaryKey

$settings = @{
    "WEBSITE_NODE_DEFAULT_VERSION" = "6.11.2";
    "DocumentDB:Key" = "$docDbPrimaryMasterKey";
    "SearchService:ApiKey" = "todo";
    "Storage:AccountKey" = "$storageAccountKey";
    "MediaServices:AccountKey" = "SjcR8Jl6tBXmWgrR8VG5hhl11vsZMoHU/zpWfyhS8AY=";
    "SQL:ElasticDbUsername" = "sqladmin";
    "SQL:ElasticDbPassword" = "%Kupimarko10";
    "RedisCache:Endpoint" = "$parentResourceGroup.redis.cache.windows.net:6380";
    "RedisCache:Key" = "$cachePrimaryKey";
    "AzureAd:Instance" = "https://login.microsoftonline.com/";
    "AzureAd:Domain" = "PlanetScaleStoreTenant.onmicrosoft.com";
    "AzureAd:TenantId" = "01888d05-7894-4567-a492-37e26936a362";
    "AzureAd:ClientId" = "c956f8f1-5495-411d-94c1-ed93c2d6d822";
    "AzureAd:CallbackPath" = "/signin-oidc";
    "AzureAd:ClientSecret" = "2+iF75CbOs6ZzkySGHaK+vcVNhIcIcGMAuGG+cFsvr8=";
    "Region"= "$region";
    "ServiceBus:Namespace" = "$parentResourceGroup";
    "ServiceBus:Queue" = "orders";
    "ServiceBus:WriteAccessKeyName" = "write";
    "ServiceBus:WriteAccessKey" = "$writeAccessKey";
}

Write-Host "Setting App Settings for $webappName"
Set-AzureWebsite $webappName -AppSettings $settings
Write-Host "App Settings updated successfully..."


#####################################################################################################
# Set App Service Connection string
# Create Hash variable for Connection Strings
$connString = @{}
# Add or Update a desired Connection String within the Hash collection
$connString["DefaultConnection"] = @{ Type = "SqlAzure"; Value = "Server=tcp:$serverName.database.windows.net,1433;Initial Catalog=$databaseName;Persist Security Info=False;User ID=$adminLogin;Password=$password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" }

Write-Host "Setting Connection String for $webappName"
# Save Connection String to Azure Web App
Set-AzureRmWebAppSlot -ResourceGroupName $resourceGroupName -Name $webappName -Slot production -ConnectionStrings $connString
Write-Host "Connection String saved succeffully.."

#####################################################################################################
# Set Always-On property True - Required for continuous Web Jobs
$WebAppPropertiesObject = @{"siteConfig" = @{"AlwaysOn" = $true}}
$WebAppResourceType = 'microsoft.web/sites'
$webAppResource = Get-AzureRmResource -ResourceType $WebAppResourceType -ResourceGroupName $resourceGroupName -ResourceName $webappName
$webAppResource | Set-AzureRmResource -PropertyObject $WebAppPropertiesObject -Force
