<#
.SYNOPSIS
Create the Primary Region with its Resources [Storage Account, CDN Profile, Document DB Account & Database, Traffic Manager Profile]
All Resources in the Resource Group are named the same "$primaryName-" + "$resourceGroupLocation",
e.g. planetscalestore-westeurope

.Author: Christos Sakellarios

.PARAMETER PrimaryName
Basic name to be used for resources
.PARAMETER ResourceGroupLocation
Azure Region for the primary resource group

#>
param (
    [Parameter(Mandatory = $true)] [string] $PrimaryName,
    [Parameter(Mandatory = $true)] [string] $ResourceGroupLocation,
    [Parameter(Mandatory = $false)] [bool] $CreateIdentityDatabase,
    [Parameter(Mandatory = $false)] [string] $SqlServerLogin,
    [Parameter(Mandatory = $false)] [string] $SqlServerPassword
)

ECHO OFF
Clear-Host

# prefixes
$storagePrefix = "storage";
$cdnPrefix = "cdn";
$cosmosDbPrefix = "cosmosdb";
$sqlServerPrefix = "sqlserver";
$endpointPrefix = "endpoint";

#####################################################################################################
# Create the parent Resource Group
# Get list of locations and select one.
# Get-AzureRmLocation | select Location 

Get-AzureRmResourceGroup -Name $PrimaryName -ev notPresent -ea 0

if ($notPresent)
{
    # ResourceGroup doesn't exist
    Write-Host "Trying to create Resource Group: $PrimaryName "
    New-AzureRmResourceGroup -Name $PrimaryName -Location $ResourceGroupLocation
}
else
{
    # ResourceGroup exist
    Write-Host "Resource Group:  $PrimaryName  already exists.."
}
#####################################################################################################
# Create the Storage Account
# https://docs.microsoft.com/en-us/azure/storage/common/storage-powershell-guide-full
# https://docs.microsoft.com/en-us/azure/storage/common/storage-redundancy
$storageAccountName = "$PrimaryName" + "$storagePrefix";

Get-AzureRmStorageAccount -ResourceGroupName $PrimaryName `
                          -Name $storageAccountName -ev storageNotPresent -ea 0

if ($storageNotPresent)
{  
    Write-Host "Creating Storage Account $storageAccountName"
    $skuName = "Standard_GRS"

    # Create the storage account.
    $storageAccount = New-AzureRmStorageAccount -ResourceGroupName $PrimaryName `
      -Name $storageAccountName `
      -Location $ResourceGroupLocation `
      -SkuName $skuName

    Write-Host "Storage Account $storageAccountName successfully created.."
}
else
{
    Write-Host "Storage Account $storageAccountName already exists.."
}
#####################################################################################################
# Create the CDN Account
# https://docs.microsoft.com/en-us/azure/cdn/cdn-manage-powershell
$cdnProfileName = "$PrimaryName-$cdnPrefix";

Get-AzureRmCdnProfile -ProfileName $cdnProfileName -ResourceGroupName $PrimaryName -ev cdnNotPresent -ea 0
if ($cdnNotPresent)
{ 
    Write-Host "Creating CDN profile $cdnProfileName.."
    # Create a new profile
    New-AzureRmCdnProfile -ProfileName $cdnProfileName -ResourceGroupName $PrimaryName `
                          -Sku Standard_Verizon -Location $ResourceGroupLocation
    
    Write-Host "CDN profile $cdnProfileName succesfully created.."

    # Create a new endpoint
    # https://docs.microsoft.com/en-us/azure/cdn/cdn-manage-powershell#creating-cdn-profiles-and-endpoints

    $cdnEnpointName = "$PrimaryName-$endpointPrefix";
    $endpointHost =  "$storageAccountName.blob.core.windows.net"

    $availability = Get-AzureRmCdnEndpointNameAvailability -EndpointName $cdnEnpointName

    if($availability.NameAvailable) {
        Write-Host "Creating endpoint..."

        New-AzureRmCdnEndpoint -ProfileName $cdnProfileName -ResourceGroupName $PrimaryName `
         -Location $ResourceGroupLocation -EndpointName $cdnEnpointName `
         -OriginName "$storageAccountName" -OriginHostName $endpointHost -OriginHostHeader $endpointHost
    }
}
else
{
    Write-Host "CDN profile $cdnProfileName already exists.."
}
#####################################################################################################
# Create the Azure Cosmos DocumentDB Account
# https://docs.microsoft.com/en-us/azure/cosmos-db/scripts/create-database-account-powershell?toc=%2fpowershell%2fmodule%2ftoc.json
# https://docs.microsoft.com/en-us/azure/cosmos-db/manage-account-with-powershell

$documentDbDatabase = "$PrimaryName-$cosmosDbPrefix";

$query = Find-AzureRmResource -ResourceNameContains $documentDbDatabase -ResourceType "Microsoft.DocumentDb/databaseAccounts"

if (!$query)
{ 
    Write-Host "Creating DocumentDB account $documentDbDatabase.."
    # Create the account

    # Write and read locations and priorities for the database
    $locations = @(@{"locationName"= $ResourceGroupLocation; 
                     "failoverPriority"=0})

    # Consistency policy
    $consistencyPolicy = @{"defaultConsistencyLevel"="Session";}

    # DB properties
    # https://docs.microsoft.com/en-us/azure/cosmos-db/consistency-levels
    $DBProperties = @{"databaseAccountOfferType"="Standard"; 
                              "locations"=$locations; 
                              "consistencyPolicy"=$consistencyPolicy}

    # Create the database
    New-AzureRmResource -ResourceType "Microsoft.DocumentDb/databaseAccounts" `
                        -ApiVersion "2015-04-08" `
                        -ResourceGroupName $PrimaryName `
                        -Location $ResourceGroupLocation `
                        -Name $documentDbDatabase `
                        -PropertyObject $DBProperties
    
    Write-Host "DocumentDB account $documentDbDatabase succesfully created.."
}
else
{
    Write-Host "DocumentDB account $documentDbDatabase already exists.."
}
#####################################################################################################
# Create the Traffic Manager Profile

$tmpProfileName = "$PrimaryName";
$tmpDnsName = "$PrimaryName";

Get-AzureRmTrafficManagerProfile -Name $tmpProfileName -ResourceGroupName $PrimaryName -ev tmpNotPresent -ea 0
if($tmpNotPresent) {
    Write-Host "Creating Traffic Manager Profile $tmpProfileName.."

    New-AzureRmTrafficManagerProfile -Name $tmpProfileName `
    -ResourceGroupName $PrimaryName -TrafficRoutingMethod Performance `
    -RelativeDnsName $tmpDnsName -Ttl 30 -MonitorProtocol HTTP -MonitorPort 80 -MonitorPath "/"

    Write-Host "Traffic Manager Profile created successfully.."
}
else {
    Write-Host "Traffic Maanger $tmpProfileName already exists.."
}

#####################################################################################################
# Create the identitydb SQL Server for authentication

if($CreateIdentityDatabase -and $SqlServerLogin -and $SqlServerPassword)
{
    Write-Host "Checking for Identity Server and Database.."

    $serverName =  "$PrimaryName-$sqlServerPrefix";
    $resourceGroupName = $PrimaryName;
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
    # Create the identitydb SQL Server Database for authentication
    # Create the database
    # https://docs.microsoft.com/en-us/powershell/module/azurerm.sql/new-azurermsqldatabase?view=azurermps-5.1.1
    # https://docs.microsoft.com/en-us/azure/sql-database/sql-database-service-tiers
    # https://docs.microsoft.com/en-us/azure/sql-database/sql-database-what-is-a-dtu
    # https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.management.sql.models.database.requestedserviceobjectivename?view=azure-dotnet
    $Database = "identitydb";

    if($CreateIdentityDatabase) {
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
}
# Send a beep
[console]::beep(1000,500)