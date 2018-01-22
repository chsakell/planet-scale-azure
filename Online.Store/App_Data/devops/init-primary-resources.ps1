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
    [Parameter(Mandatory = $true)] [string] $ResourceGroupLocation
)

ECHO OFF
Clear-Host

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
$storageAccountName = "$PrimaryName";

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
$cdnProfileName = "$PrimaryName";

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

    $cdnEnpointName = "$PrimaryName";
    $endpointHost =  "$PrimaryName.blob.core.windows.net"

    $availability = Get-AzureRmCdnEndpointNameAvailability -EndpointName $cdnEnpointName

    if($availability.NameAvailable) {
        Write-Host "Creating endpoint..."

        New-AzureRmCdnEndpoint -ProfileName $cdnProfileName -ResourceGroupName $PrimaryName `
         -Location $ResourceGroupLocation -EndpointName $cdnEnpointName `
         -OriginName "$PrimaryName" -OriginHostName $endpointHost -OriginHostHeader $endpointHost
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

$documentDbDatabase = "$PrimaryName";

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
 

# Send a beep
[console]::beep(1000,500)