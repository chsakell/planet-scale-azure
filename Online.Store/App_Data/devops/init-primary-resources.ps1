
ECHO OFF

# Enable-AzureRmContextAutosave

# sign in
Write-Host "Logging in...";
# Login-AzureRmAccount;

# select subscription
$subscriptionId = "8433038c-5b0f-4501-ba56-4a4fea6a0ee3";
Write-Host "Selecting subscription";
# Select-AzureRmSubscription -SubscriptionID $subscriptionId;

#####################################################################################################
# Create the parent Resource Group
# Get list of locations and select one.
# Get-AzureRmLocation | select Location 

$primaryResourceGroupName = "planetscalestore";
$primaryResourceGroupLocation = "westeurope";

Get-AzureRmResourceGroup -Name $primaryResourceGroupName -ev notPresent -ea 0

if ($notPresent)
{
    # ResourceGroup doesn't exist
    Write-Host "Trying to create Resource Group: $primaryResourceGroupName "
    New-AzureRmResourceGroup -Name $primaryResourceGroupName -Location $primaryResourceGroupLocation
}
else
{
    # ResourceGroup exist
    Write-Host "Resource Group:  $primaryResourceGroupName  already exists.."
}
#####################################################################################################
# Create the Storage Account
# https://docs.microsoft.com/en-us/azure/storage/common/storage-powershell-guide-full
# https://docs.microsoft.com/en-us/azure/storage/common/storage-redundancy
$storageAccountName = "planetscalestore";

Get-AzureRmStorageAccount -ResourceGroupName $primaryResourceGroupName `
                          -Name $storageAccountName -ev storageNotPresent -ea 0

if ($storageNotPresent)
{  
    Write-Host "Creating Storage Account $storageAccountName"
    $skuName = "Standard_GRS"

    # Create the storage account.
    $storageAccount = New-AzureRmStorageAccount -ResourceGroupName $primaryResourceGroupName `
      -Name $storageAccountName `
      -Location $primaryResourceGroupLocation `
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
$cdnProfileName = "planetscalestore";

Get-AzureRmCdnProfile -ProfileName $cdnProfileName -ResourceGroupName $primaryResourceGroupName -ev cdnNotPresent -ea 0
if ($cdnNotPresent)
{ 
    Write-Host "Creating CDN profile $cdnProfileName.."
    # Create a new profile
    New-AzureRmCdnProfile -ProfileName $cdnProfileName -ResourceGroupName $primaryResourceGroupName `
                          -Sku Standard_Verizon -Location $primaryResourceGroupLocation
    
    Write-Host "CDN profile $cdnProfileName succesfully created.."
}
else
{
    Write-Host "CDN profile $cdnProfileName already exists.."
}
#####################################################################################################
# Create the Azure Cosmos DocumentDB Account
# https://docs.microsoft.com/en-us/azure/cosmos-db/scripts/create-database-account-powershell?toc=%2fpowershell%2fmodule%2ftoc.json
# https://docs.microsoft.com/en-us/azure/cosmos-db/manage-account-with-powershell

$documentDbDatabase = "planetscalestore";


$query = Find-AzureRmResource -ResourceNameContains $documentDbDatabase -ResourceType "Microsoft.DocumentDb/databaseAccounts"

if (!$query)
{ 
    Write-Host "Creating DocumentDB account $documentDbDatabase.."
    # Create the account

    # Write and read locations and priorities for the database
    $locations = @(@{"locationName"= $primaryResourceGroupLocation; 
                     "failoverPriority"=0})

    # Consistency policy
    $consistencyPolicy = @{"defaultConsistencyLevel"="BoundedStaleness";
                           "maxIntervalInSeconds"="10"; 
                           "maxStalenessPrefix"="200"}

    # DB properties
    $DBProperties = @{"databaseAccountOfferType"="Standard"; 
                              "locations"=$locations; 
                              "consistencyPolicy"=$consistencyPolicy}

    # Create the database
    New-AzureRmResource -ResourceType "Microsoft.DocumentDb/databaseAccounts" `
                        -ApiVersion "2015-04-08" `
                        -ResourceGroupName $primaryResourceGroupName `
                        -Location $primaryResourceGroupLocation `
                        -Name $documentDbDatabase `
                        -PropertyObject $DBProperties
    
    Write-Host "DocumentDB account $documentDbDatabase succesfully created.."
}
else
{
    Write-Host "DocumentDB account $documentDbDatabase already exists.."
}
#####################################################################################################
# Create the Traffic Manager Account

$tmpProfileName = "planetscalestore";
$tmpDnsName = "planetscalestore";

try {
    Get-AzureRmTrafficManagerProfile -Name $tmpProfileName -ResourceGroupName $primaryResourceGroupName -ErrorAction Stop
    Write-Host "Traffic Maanger $tmpProfileName already exists.."
 }
catch {
     $ErrorMessage = $_.Exception.Message;
     Write-Host $ErrorMessage;

     Write-Host "Creating Traffic Manager Profile $tmpProfileName.."

     New-AzureRmTrafficManagerProfile -Name $tmpProfileName -ResourceGroupName $primaryResourceGroupName -TrafficRoutingMethod Performance `
    -RelativeDnsName $tmpDnsName -Ttl 30 -MonitorProtocol HTTP -MonitorPort 80 -MonitorPath "/"
}

