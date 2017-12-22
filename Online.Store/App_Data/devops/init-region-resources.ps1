# Check for resources 
# https://docs.microsoft.com/en-us/powershell/module/azurerm.resources/find-azurermresource?view=azurermps-5.1.1

ECHO OFF

# Enable-AzureRmContextAutosave

# sign in
Write-Host "Logging in...";
# Login-AzureRmAccount;

# select subscription
$subscriptionId = "YOUR-SUBSCRIPTION-ID";
Write-Host "Selecting subscription";
# Select-AzureRmSubscription -SubscriptionID $subscriptionId;

#####################################################################################################
# Create the parent Resource Group
# Get list of locations and select one.
# Get-AzureRmLocation | select Location 

$primaryName = "planetscalestore";
$resourceGroupLocation = "westeurope";
$resourceGroupName = "$primaryName-" + $resourceGroupLocation;

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
# Create the Service Bus
# https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-manage-with-ps

$serviceBusNameSpace = "$primaryName-" + $resourceGroupLocation;

$serviceBusExists = Test-AzureName -ServiceBusNamespace $serviceBusNameSpace

# Check if the namespace already exists or needs to be created
if ($serviceBusExists)
{
    Write-Host "The namespace $serviceBusNameSpace already exists in the $resourceGroupLocation region:"
    # Report what was found
    Get-AzureRMServiceBusNamespace -ResourceGroup $resourceGroupName -NamespaceName $serviceBusNameSpace
}
else
{
    Write-Host "The $serviceBusNameSpace namespace does not exist."
    Write-Host "Creating the $serviceBusNameSpace namespace in the $resourceGroupLocation region..."
    New-AzureRmServiceBusNamespace -ResourceGroup $resourceGroupName -NamespaceName $serviceBusNameSpace -Location $resourceGroupLocation
    $namespace = Get-AzureRMServiceBusNamespace -ResourceGroup $resourceGroupName -NamespaceName $serviceBusNameSpace
    Write-Host "The $serviceBusNameSpace namespace in Resource Group $resourceGroupName in the $resourceGroupLocation region has been successfully created."

}
#####################################################################################################
# Create the Redis Cache
# https://docs.microsoft.com/en-us/azure/redis-cache/cache-howto-manage-redis-cache-powershell
# $PSVersionTable # Check PowerShell Version
<#
Important

The first time you create a Redis cache in a subscription using the Azure portal, 
the portal registers the Microsoft.Cache namespace for that subscription. If you
attempt to create the first Redis cache in a subscription using PowerShell, you
must first register that namespace using the following command; otherwise cmdlets 
such as New-AzureRmRedisCache and Get-AzureRmRedisCache fail.

Register-AzureRmResourceProvider -ProviderNamespace "Microsoft.Cache"

#>

# Get-Help New-AzureRmRedisCache -detailed

$cacheName = "$primaryName-" + $resourceGroupLocation;

try {
    Get-AzureRmRedisCache -Name $cacheName -ResourceGroupName $resourceGroupName -ErrorAction Stop
    Write-Host "Redis Cache $cacheName already exists.."
 }
catch {
     $ErrorMessage = $_.Exception.Message;
     Write-Host $ErrorMessage;
     
     Write-Host "The $cacheName Redis Cache does not exist."
     Write-Host "Creating the $cacheName Redis Caache in the $resourceGroupLocation region..."
    
     New-AzureRmRedisCache -ResourceGroupName $resourceGroupName -Name $cacheName `
      -Location $resourceGroupLocation -Size 250MB

     Write-Host "$cacheName Redis Cache successfully created.." 
     Get-AzureRmRedisCache -Name $cacheName -ResourceGroupName $resourceGroupName
 }

 #####################################################################################################
# Create the Search Engine
# https://docs.microsoft.com/en-us/azure/search/search-manage-powershell

# Register the ARM provider idempotently. This must be done once per subscription
# Register-AzureRmResourceProvider -ProviderNamespace "Microsoft.Search"

$searchEngineName = "$primaryName-" + $resourceGroupLocation;  #"your-service-name-lowercase-with-dashes"
$sku = "free" # or "basic" or "standard" for paid services

# You can get a list of potential locations with
# (Get-AzureRmResourceProvider -ListAvailable | Where-Object {$_.ProviderNamespace -eq 'Microsoft.Search'}).Locations

$query = Find-AzureRmResource -ResourceNameContains $searchEngineName -ResourceType "Microsoft.Search/searchServices"

if (!$query) {
    
# Create a new search service
    Write-Host "Creating Search Engine $searchEngineName in Resource Group $resourceGroupLocation"
  
    New-AzureRmResourceGroupDeployment `
    -ResourceGroupName $resourceGroupName `
    -TemplateUri "https://gallery.azure.com/artifact/20151001/Microsoft.Search.1.0.9/DeploymentTemplates/searchServiceDefaultTemplate.json" `
    -NameFromTemplate $searchEngineName `
    -Sku $sku `
    -Location $resourceGroupLocation `
    -PartitionCount 1 `
    -ReplicaCount 1
}
else {
    Write-Host "Search Engine $searchEngineName already exists in Resource Group $resourceGroupName"

    Get-AzureRmResource `
    -ResourceType "Microsoft.Search/searchServices" `
    -ResourceGroupName $resourceGroupName `
    -ResourceName $searchEngineName `
    -ApiVersion 2015-08-19 `
}
