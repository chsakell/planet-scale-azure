
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

# Query to see if the namespace currently exists
$namespace = Get-AzureRMServiceBusNamespace -ResourceGroup $resourceGroupName -NamespaceName $Namespace

# Check if the namespace already exists or needs to be created
if ($namespace)
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