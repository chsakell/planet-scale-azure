<#
.SYNOPSIS
Create the App Service's slot resources [SQL Server]

.Author: Christos Sakellarios

.PARAMETER PrimaryName
Basic name to be used for resources

.PARAMETER ResourceGroupLocation
Azure Region for the resource group. Must be the same with the parent one

.PARAMETER Version
Sub region Version. Usually you will need two for business continuity
e.g a or b

.PARAMETER SqlServerLogin
SQL Logical Server's admin username.

.PARAMETER SqlServerPassword
SQL Logical Server's admin password.

.PARAMETER CreateDatabase
Define to create or not the database. Set true only to the primary sub-region
All secondary databases will be created during geo-replication

.PARAMETER Database
The database name

.PARAMETER DocumentDBPolicies
The Connection Policies for connecting to Azure DocumentDB, comma separated
Start from first to second etc..
e.g. West Europe,Central US

#>
param (
    [Parameter(Mandatory = $true)] [string] $PrimaryName,
    [Parameter(Mandatory = $true)] [string] $ResourceGroupLocation,
    [Parameter(Mandatory = $true)] [string] $SqlServerVersion,
    [Parameter(Mandatory = $true)] [string] $SqlServerLogin,
    [Parameter(Mandatory = $true)] [string] $SqlServerPassword,
    [Parameter(Mandatory = $true)] [string] $Database,
    [Parameter(Mandatory = $true)] [string] $DocumentDBPolicies,
    [Parameter(Mandatory = $false)] [string] $AzureIDClientSecret,
    [Parameter(Mandatory = $false)] [bool] $UseIdentity,
    [Parameter(Mandatory = $false)] [string] $IdentitySqlServerLogin,
    [Parameter(Mandatory = $false)] [string] $IdentitySqlServerPassword
)

ECHO OFF
Clear-Host

# prefixes
$appServicePlanPrefix = "plan";
$sqlServerPrefix = "sql";
$cosmosDbPrefix = "cosmosdb";
$storagePrefix = "storage";
$serviceBusPrefix = "servicebus";
$redisCachePrefix = "rediscache";
$searchServicePrefix = "search";


Write-Host "PrimaryName: $PrimaryName"
Write-Host "ResourceGroupLocation $ResourceGroupLocation"
Write-Host "Version $Version"
Write-Host "SqlServerLogin $SqlServerLogin"
Write-Host "SqlServerPassword $SqlServerPassword"

$resourceGroupName = "$PrimaryName-$ResourceGroupLocation-app";


#####################################################################################################
# Create the App Service Slot
# https://docs.microsoft.com/en-us/powershell/module/azurerm.websites/get-azurermwebappslot?view=azurermps-5.1.1
# https://docs.microsoft.com/en-us/powershell/module/azurerm.websites/new-azurermwebappslot?view=azurermps-5.1.1

$appServicePlan = "$resourceGroupName-$appServicePlanPrefix";
$webappName = "$resourceGroupName";
$slotName = "upgrade";

$azureSlot = Get-AzureRmWebAppSlot -ResourceGroupName "$resourceGroupName" `
                      -Name "$webappName" -Slot "$slotName" `
                      -ErrorAction SilentlyContinue

if ($azureSlot)
{
    Write-Host "Azure Slot already exists.."
}
else
{
    # Upgrade slot doesn't exist
    Write-Host "Creating Azure Slot to $webappName .."

    New-AzureRmWebAppSlot -ResourceGroupName "$resourceGroupName" `
                          -Name "$webappName" -AppServicePlan "$appServicePlan" `
                          -Slot "$slotName"

    Write-Host "$slotName Azure Slot created.."
}

#####################################################################################################
# Create the Logical SQL Server and Database
# https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started-powershell

$serverName =  "$resourceGroupName-$sqlServerPrefix-$SqlServerVersion";
# Set an admin login and password for your database
# The login information for the server

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
    -Location $ResourceGroupLocation `
    -SqlAdministratorCredentials $(New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $SqlServerLogin, $(ConvertTo-SecureString -String $SqlServerPassword -AsPlainText -Force))

    Write-Host "SQL Server $serverName successfully created..."

    # Allow access to Azure Services
    # https://docs.microsoft.com/en-us/powershell/module/azure/new-azuresqldatabaseserverfirewallrule?view=azuresmps-4.0.0
    Write-Host "Allowing access to Azure Services..."

    New-AzureSqlDatabaseServerFirewallRule -ServerName $serverName -AllowAllAzureServices
}

#####################################################################################################
# Set App Service settings

$primaryResourceGroupName = "$PrimaryName";
$documentDbDatabase = "$PrimaryName-$cosmosDbPrefix"
$region = "$ResourceGroupLocation";
# Retrieve the primary account keys
$docDbPrimaryMasterKey = Invoke-AzureRmResourceAction -Action listKeys `
    -ResourceType "Microsoft.DocumentDb/databaseAccounts" `
    -ApiVersion "2015-04-08" `
    -ResourceGroupName $primaryResourceGroupName `
    -Name $documentDbDatabase | select -ExpandProperty  primaryMasterKey

# Get Storage Account Key
$storaceAccountName =  "$PrimaryName" + "$storagePrefix";
$storageAccountKey = (Get-AzureRmStorageAccountKey -ResourceGroupName $primaryResourceGroupName `
 -AccountName $storaceAccountName).Value[0] 

$parentResourceGroup = "$PrimaryName-$ResourceGroupLocation";
# Get Redis Cache Key
$cacheName = "$PrimaryName-$ResourceGroupLocation-$redisCachePrefix";
$cachePrimaryKey = (Get-AzureRmRedisCacheKey -Name $cacheName -ResourceGroupName $parentResourceGroup).PrimaryKey

# Get Service Bus Queue orders Write Rule Key
# https://docs.microsoft.com/en-us/powershell/module/azurerm.servicebus/Get-AzureRmServiceBusKey?view=azurermps-5.1.1
$queueName = "orders"
$serviceBusNameSpace = "$PrimaryName-$ResourceGroupLocation-$serviceBusPrefix";
$writeAccessKey = (Get-AzureRmServiceBusKey -ResourceGroup  $parentResourceGroup `
     -Namespace $serviceBusNameSpace -Queue $queueName -Name "write").PrimaryKey

# Get Search Service Primary Key
# https://docs.microsoft.com/en-us/azure/search/search-manage-powershell
$searchServiceInfo = "$PrimaryName-$ResourceGroupLocation-$searchServicePrefix";
# Get information about your new service and store it in $resource
$searchService = Get-AzureRmResource `
    -ResourceType "Microsoft.Search/searchServices" `
    -ResourceGroupName $parentResourceGroup `
    -ResourceName $searchServiceInfo `
    -ApiVersion "2015-08-19"

# Get the primary admin API key
$searchServicePrimaryKey = (Invoke-AzureRmResourceAction `
    -Action listAdminKeys `
    -ResourceId $searchService.ResourceId `
    -ApiVersion 2015-08-19).PrimaryKey

$settings = @{
    "WEBSITE_NODE_DEFAULT_VERSION" = "6.11.2";
    "DocumentDB:Key" = "$docDbPrimaryMasterKey";
    "DocumentDB:ConnectionPolicies" = "$DocumentDBPolicies";
    "SearchService:Name" = "$searchServiceInfo";
    "SearchService:ApiKey" = "$searchServicePrimaryKey";
    "Storage:AccountKey" = "$storageAccountKey";
    "RedisCache:Endpoint" = "$cacheName";
    "RedisCache:Key" = "$cachePrimaryKey";
    "Region"= "$region";
    "ServiceBus:Namespace" = "$serviceBusNameSpace";
    "ServiceBus:Queue" = "orders";
    "ServiceBus:WriteAccessKeyName" = "write";
    "ServiceBus:WriteAccessKey" = "$writeAccessKey";
}

$settings.Add("UseIdentity", $UseIdentity.ToString());

if($AzureIDClientSecret) {
  $settings.Add("AzureAd:ClientSecret", "$AzureIDClientSecret");
}

Write-Host "Setting App Settings for $webappName"
$printSettings = ConvertTo-Json $settings -Depth 2
$printSettings
Set-AzureRmWebAppSlot -ResourceGroupName "$resourceGroupName" `
                      -Name "$webappName" -AppServicePlan "$appServicePlan" `
                      -Slot "$slotName" -AppSettings $settings
Write-Host "App Settings updated successfully..."


#####################################################################################################
# Set App Service Connection string
# Create Hash variable for Connection Strings
$connString = @{}
# Add or Update a desired Connection String within the Hash collection
$connString["DefaultConnection"] = @{ Type = "SqlAzure"; Value = "Server=tcp:$serverName.database.windows.net,1433;Initial Catalog=$Database;Persist Security Info=False;User ID=$SqlServerLogin;Password=$SqlServerPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" }

if($UseIdentity -and $IdentitySqlServerLogin -and $IdentitySqlServerPassword) {
    $identityServer = "$PrimaryName-$sqlServerPrefix";
    $IdentityDatabase = "identitydb";
    $connString["IdentityConnection"] = @{ Type = "SqlAzure"; Value = "Server=tcp:$identityServer.database.windows.net,1433;Initial Catalog=$IdentityDatabase;Persist Security Info=False;User ID=$IdentitySqlServerLogin;Password=$IdentitySqlServerPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" }
}

Write-Host "Setting Connection String for $webappName"
# Save Connection String to Azure Web App
Set-AzureRmWebAppSlot -ResourceGroupName $resourceGroupName -Name $webappName -Slot "$slotName" -ConnectionStrings $connString
Write-Host "Connection String saved succeffully.."

#####################################################################################################
# Set Always-On property True - Required for continuous Web Jobs
$WebAppPropertiesObject = @{"siteConfig" = @{"AlwaysOn" = $true}}

$azureSlot = Get-AzureRmWebAppSlot -ResourceGroupName "$resourceGroupName" `
                      -Name "$webappName" -Slot "$slotName" `
                      -ErrorAction SilentlyContinue
$azureSlot | Set-AzureRmResource -PropertyObject $WebAppPropertiesObject -Force

# Send a beep
[console]::beep(1000,500)