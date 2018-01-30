<#
.SYNOPSIS
Create the sub-region resources [App Service, SQL Server, Database(optional)]
All Resources in the Resource Group are named the same "$primaryName-" + "$resourceGroupLocation",
e.g. planetscalestore-westeurope-a

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
    [Parameter(Mandatory = $true)] [string] $Version,
    [Parameter(Mandatory = $true)] [string] $SqlServerLogin,
    [Parameter(Mandatory = $true)] [string] $SqlServerPassword,
    [Parameter(Mandatory = $true)] [bool] $CreateDatabase,
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
$appServicePrefix = "web";
$trafficManagerPrefix = "traffic";
$sqlServerPrefix = "sqlserver";
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
Write-Host "CreateDatabase $CreateDatabase"
Write-Host "Database $Database"


#####################################################################################################
# Create the sub Resource Group that contains the Web App and SQL Server/database
# Get list of locations and select one.
# Get-AzureRmLocation | select Location 

$resourceGroupName = "$PrimaryName-$ResourceGroupLocation-$Version";

Get-AzureRmResourceGroup -Name $resourceGroupName -ev notPresent -ea 0

if ($notPresent)
{
    # ResourceGroup doesn't exist
    Write-Host "Trying to create Resource Group: $resourceGroupName "
    New-AzureRmResourceGroup -Name $resourceGroupName -Location $ResourceGroupLocation
}
else
{
    # ResourceGroup exist
    Write-Host "Resource Group:  $resourceGroupName  already exists.."
}


#####################################################################################################
# Create an App Service plan in Standard tier.
# https://docs.microsoft.com/en-us/powershell/module/azurerm.websites/new-azurermappserviceplan?view=azurermps-5.1.1
$appServicePlan = "$resourceGroupName-$appServicePlanPrefix";
$webappName = "$resourceGroupName-$appServicePrefix";

Get-AzureRmAppServicePlan -ResourceGroupName $resourceGroupName -Name $appServicePlan  -ev planNotPresent -ea 0

if ($planNotPresent)
{
    # App Service Plan doesn't exist
    Write-Host "Trying to create App Service Plan $appServicePlan "
    New-AzureRmAppServicePlan -Name $appServicePlan -Location $ResourceGroupLocation `
        -ResourceGroupName $resourceGroupName -Tier Standard -WorkerSize Small
}
else
{
    # App Service Plan exist
    Write-Host "App Service Plan:  $appServicePlan  already exists.."
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

    New-AzureRmWebApp -Name $webappName -Location $ResourceGroupLocation `
        -AppServicePlan $appServicePlan -ResourceGroupName $resourceGroupName

    Write-Host "App Service $webappName successfully created at $resourceGroupName"
}

#####################################################################################################
# Configure Auto Scaling.
# https://docs.microsoft.com/en-us/powershell/module/azurerm.insights/new-azurermautoscalerule?view=azurermps-5.1.1
# https://docs.microsoft.com/en-us/powershell/module/azurerm.insights/new-azurermautoscaleprofile?view=azurermps-5.1.1
# https://docs.microsoft.com/en-us/powershell/module/azurerm.insights/add-azurermautoscalesetting

Write-Host "Configuring auto scaling.."
$webAppResourceId = (Get-AzureRmResource -ResourceGroupName $resourceGroupName -ResourceName $webappName -ResourceType "Microsoft.web/sites").ResourceId

$autoScaleRule = New-AzureRmAutoscaleRule -MetricName "CpuTime" `
                         -MetricResourceId "$webAppResourceId" `
                         -Operator GreaterThan `
                         -MetricStatistic Average `
                         -Threshold 70 `
                         -TimeGrain 00:01:00 `
                         -TimeWindow 00:10:00 `
                         -ScaleActionCooldown 00:10:00 `
                         -ScaleActionDirection Increase `
                         -ScaleActionValue "2"

$autoScaleProfile = New-AzureRmAutoscaleProfile -DefaultCapacity "1" `
                                                -MaximumCapacity "10" `
                                                -MinimumCapacity "1" `
                                                -Rules $autoScaleRule `
                                                -Name "autoscale-when-cpu-high"


$webAppPlanResourceId = (Get-AzureRmResource -ResourceGroupName $resourceGroupName -ResourceName $appServicePlan -ResourceType "Microsoft.web/serverFarms").ResourceId

$autoScaleSetting = Get-AzureRmAutoscaleSetting -ResourceGroupName "$resourceGroupName" -Name "autoscale-when-cpu-high" -ErrorAction SilentlyContinue

if($autoScaleSetting) {
Remove-AzureRmAutoscaleSetting `
      -ResourceGroupName $resourceGroupName `
      -Name "autoscale-when-cpu-high" `
}
else {
Add-AzureRmAutoscaleSetting -Location "$ResourceGroupLocation" `
                            -Name "autoscale-when-cpu-high" `
                            -ResourceGroup "$resourceGroupName" `
                            -TargetResourceId "$webAppPlanResourceId" `
                            -AutoscaleProfiles $autoScaleProfile
}
Write-Host "Autoscale when CPU is high has been configured.."

#####################################################################################################
# Add the Traffic Manager Endpoint if not exists
# https://docs.microsoft.com/en-us/powershell/module/azurerm.trafficmanager/new-azurermtrafficmanagerendpoint?view=azurermps-5.1.1
$TrafficManagerEndpoint = Get-AzureRmTrafficManagerEndpoint -Name $webappName -ProfileName "$PrimaryName-$trafficManagerPrefix" `
                     -ResourceGroupName $PrimaryName -Type AzureEndpoints -ErrorAction SilentlyContinue

if(!$TrafficManagerEndpoint) {

    $webAppResourceId = (Get-AzureRmResource -ResourceGroupName $resourceGroupName -ResourceName $webappName -ResourceType "Microsoft.web/sites").ResourceId

    Write-Host "Adding new Traffic Manager Endpoint [$webappName]..."

    New-AzureRmTrafficManagerEndpoint -EndpointStatus Disabled -Name $webappName `
         -ProfileName "$PrimaryName-$trafficManagerPrefix" -ResourceGroupName $PrimaryName -Type AzureEndpoints `
         -TargetResourceId "$webAppResourceId"

    Write-Host "Endpoint [$webappName] added successfully.."
} else {
    Write-Host "Traffic Manager Endpoint [$webappName] already exists..."
}


#####################################################################################################
# Create the Logical SQL Server and Database
# https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started-powershell

$serverName =  "$resourceGroupName-$sqlServerPrefix";
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
# Create the database
# https://docs.microsoft.com/en-us/powershell/module/azurerm.sql/new-azurermsqldatabase?view=azurermps-5.1.1
# https://docs.microsoft.com/en-us/azure/sql-database/sql-database-service-tiers
# https://docs.microsoft.com/en-us/azure/sql-database/sql-database-what-is-a-dtu
# https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.management.sql.models.database.requestedserviceobjectivename?view=azure-dotnet

if($CreateDatabase) {
    $azureDatabase = Get-AzureRmSqlDatabase -ResourceGroupName $resourceGroupName `
     -ServerName $serverName -DatabaseName $Database -ErrorAction SilentlyContinue

    if ($azureDatabase) { 
        Write-Host "Azure SQL Database $Database already exists..."
    }
    else {
        Write-Host "Trying to create Azure SQL Database $Database at Server $serverName.."

        New-AzureRmSqlDatabase  -ResourceGroupName $resourceGroupName `
        -ServerName $serverName `
        -DatabaseName $Database `
        -RequestedServiceObjectiveName "Basic" `
        -MaxSizeBytes 524288000

        Write-Host "Azure SQL Database $Database successfully created..."

    }
}

#####################################################################################################
# Set App Service settings

$primaryResourceGroupName = "$PrimaryName";
$documentDbDatabase = "$PrimaryName-$cosmosDbPrefix"
$region = "$ResourceGroupLocation-$Version";
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
Set-AzureWebsite $webappName -AppSettings $settings
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
Set-AzureRmWebAppSlot -ResourceGroupName $resourceGroupName -Name $webappName -Slot production -ConnectionStrings $connString
Write-Host "Connection String saved succeffully.."

#####################################################################################################
# Set Always-On property True - Required for continuous Web Jobs
$WebAppPropertiesObject = @{"siteConfig" = @{"AlwaysOn" = $true}}
$WebAppResourceType = 'microsoft.web/sites'
$webAppResource = Get-AzureRmResource -ResourceType $WebAppResourceType -ResourceGroupName $resourceGroupName -ResourceName $webappName
$webAppResource | Set-AzureRmResource -PropertyObject $WebAppPropertiesObject -Force

# Send a beep
[console]::beep(1000,500)