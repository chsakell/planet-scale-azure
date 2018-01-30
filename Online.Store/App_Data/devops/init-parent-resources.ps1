<#
.SYNOPSIS
Create the Parent Region with its Resources [Service Bus, Redis Cache, Search Engine]
All Resources in the Resource Group are named the same "$primaryName-" + "$resourceGroupLocation",
e.g. planetscalestore-westeurope

.Author: Christos Sakellarios

.PARAMETER PrimaryName
Basic name to be used for resources
.PARAMETER ResourceGroupLocation
Azure Region for the parent resource group

#>
param (
    [Parameter(Mandatory = $true)] [string] $PrimaryName,
    [Parameter(Mandatory = $true)] [string] $ResourceGroupLocation
)

ECHO OFF
Clear-Host

# prefixes
$serviceBusPrefix = "servicebus";
$redisCachePrefix = "rediscache";
$searchServicePrefix = "search";

#####################################################################################################
# Create the parent Resource Group
# Get list of locations and select one.
# Get-AzureRmLocation | select Location 

$resourceGroupName = "$PrimaryName-" + "$ResourceGroupLocation";

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
# Create the Service Bus
# https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-manage-with-ps

$serviceBusNameSpace = "$PrimaryName-$ResourceGroupLocation-$serviceBusPrefix";

$serviceBusExists = Test-AzureName -ServiceBusNamespace $serviceBusNameSpace

# Check if the namespace already exists or needs to be created
if ($serviceBusExists)
{
    Write-Host "The namespace $serviceBusNameSpace already exists in the $ResourceGroupLocation region:"
    # Report what was found
    Get-AzureRMServiceBusNamespace -ResourceGroup $resourceGroupName -NamespaceName $serviceBusNameSpace
}
else
{
    Write-Host "The $serviceBusNameSpace namespace does not exist."
    Write-Host "Creating the $serviceBusNameSpace namespace in the $ResourceGroupLocation region..."
    New-AzureRmServiceBusNamespace -ResourceGroup $resourceGroupName -NamespaceName $serviceBusNameSpace -Location $ResourceGroupLocation
    $namespace = Get-AzureRMServiceBusNamespace -ResourceGroup $resourceGroupName -NamespaceName $serviceBusNameSpace
    Write-Host "The $serviceBusNameSpace namespace in Resource Group $resourceGroupName in the $ResourceGroupLocation region has been successfully created."

    # Create the Orders queue
    # Check if queue already exists
    $queueName = "orders"
    $ordersQueue = Get-AzureRmServiceBusQueue -ResourceGroup $resourceGroupName `
         -NamespaceName $serviceBusNameSpace -QueueName $queueName -ErrorAction SilentlyContinue

    if($ordersQueue)
    {
        Write-Host "The queue $queueName already exists in the $ResourceGroupLocation region:"
    }
    else
    {
        Write-Host "The $queueName queue does not exist."
        Write-Host "Creating the $queueName queue in the $ResourceGroupLocation region..."
        New-AzureRmServiceBusQueue -ResourceGroup $resourceGroupName -NamespaceName $serviceBusNameSpace -QueueName $queueName -EnablePartitioning $True
        $ordersQueue = Get-AzureRmServiceBusQueue -ResourceGroup $resourceGroupName -NamespaceName $serviceBusNameSpace -QueueName $queueName
        Write-Host "The $queueName queue in Resource Group $resourceGroupName in the $ResourceGroupLocation region has been successfully created."
    }

    # Create new Authorization Rules
    # https://docs.microsoft.com/en-us/powershell/module/azurerm.servicebus/New-AzureRmServiceBusAuthorizationRule?view=azurermps-5.1.1
    # https://docs.microsoft.com/en-us/powershell/module/azurerm.servicebus/Get-AzureRmServiceBusAuthorizationRule?view=azurermps-5.1.1
    $writeRule = "write"
    $readRule = "read"

    $authWriteRule = Get-AzureRmServiceBusAuthorizationRule -ResourceGroup $resourceGroupName -Namespace $serviceBusNameSpace `
         -Queue $queueName -Name $writeRule -ErrorAction SilentlyContinue
    if(!$authWriteRule) {
        "Write Rule not found. Creating rule in namespace $serviceBusNameSpace.."
        New-AzureRmServiceBusAuthorizationRule -ResourceGroup $resourceGroupName -Namespace $serviceBusNameSpace `
         -Queue $queueName -Name $writeRule -Rights @("Send")
    } 
    else {
        Write-Host "Write Rule already exists..."
    }

    $authReadRule = Get-AzureRmServiceBusAuthorizationRule -ResourceGroup $resourceGroupName -Namespace $serviceBusNameSpace `
         -Queue $queueName -Name $readRule -ErrorAction SilentlyContinue
    if(!$authReadRule) {
        "Read Rule not found. Creating rule in namespace $serviceBusNameSpace.."
        New-AzureRmServiceBusAuthorizationRule -ResourceGroup $resourceGroupName -Namespace $serviceBusNameSpace `
         -Queue $queueName -Name $readRule -Rights @("Listen")
    } 
    else {
        Write-Host "Read Rule already exists..."
    }

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

$cacheName = "$PrimaryName-$ResourceGroupLocation-$redisCachePrefix";

$redisCache = Get-AzureRmRedisCache -Name $cacheName -ResourceGroupName $resourceGroupName -ErrorAction SilentlyContinue

if($redisCache) {
    Write-Host "Redis Cache $cacheName already exists.."
}
else {
    Write-Host "The $cacheName Redis Cache does not exist."
    Write-Host "Creating the $cacheName Redis Cache in the $ResourceGroupLocation region..."
    
    New-AzureRmRedisCache -ResourceGroupName $resourceGroupName -Name $cacheName `
    -Location $ResourceGroupLocation -Size 250MB

    Write-Host "$cacheName Redis Cache successfully created.." 
    Get-AzureRmRedisCache -Name $cacheName -ResourceGroupName $resourceGroupName
}

 #####################################################################################################
# Create the Search Engine
# https://docs.microsoft.com/en-us/azure/search/search-manage-powershell

# Register the ARM provider idempotently. This must be done once per subscription
# Register-AzureRmResourceProvider -ProviderNamespace "Microsoft.Search"

$searchEngineName = "$PrimaryName-$ResourceGroupLocation-$searchServicePrefix";
$sku = "basic" # or "basic" or "standard" for paid services

# You can get a list of potential locations with
# (Get-AzureRmResourceProvider -ListAvailable | Where-Object {$_.ProviderNamespace -eq 'Microsoft.Search'}).Locations

$query = Find-AzureRmResource -ResourceNameContains $searchEngineName -ResourceType "Microsoft.Search/searchServices"

if (!$query) {
    
# Create a new search service
    Write-Host "Creating Search Engine $searchEngineName in Resource Group $ResourceGroupLocation"
  
    New-AzureRmResourceGroupDeployment `
    -ResourceGroupName $resourceGroupName `
    -TemplateUri "https://gallery.azure.com/artifact/20151001/Microsoft.Search.1.0.9/DeploymentTemplates/searchServiceDefaultTemplate.json" `
    -NameFromTemplate $searchEngineName `
    -Sku $sku `
    -Location $ResourceGroupLocation `
    -PartitionCount 1 `
    -ReplicaCount 1
}
else {
    Write-Host "Search Engine $searchEngineName already exists in Resource Group $resourceGroupName"

    Get-AzureRmResource `
    -ResourceType "Microsoft.Search/searchServices" `
    -ResourceGroupName $resourceGroupName `
    -ResourceName $searchEngineName `
    -ApiVersion "2015-08-19" `
}

# Send a beep
[console]::beep(1000,500)