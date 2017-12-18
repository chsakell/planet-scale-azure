
ECHO OFF
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