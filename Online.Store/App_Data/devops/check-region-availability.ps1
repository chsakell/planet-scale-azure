<#
.SYNOPSIS
Checks availability for Resource Types in a specific Region
e.g. .\check-region-availability.ps1 -Region "westeurope"

.Author: Christos Sakellarios

.PARAMETER Region
Region to check for availability
Value from : Get-AzureRmLocation | select Location

#>
param (
    [Parameter(Mandatory = $true)] [string] $Region
    )

ECHO OFF
Clear-Host
$providers = (Get-AzureRmLocation | Where-Object {$_.Location -eq $Region}).Providers
$cdnAvailable =  $providers.Contains("Microsoft.Cdn")
$documentDbAvailable =  $providers.Contains("Microsoft.DocumentDB")
$storageAvailable =  $providers.Contains("Microsoft.Storage")
$searchAvailable =  $providers.Contains("Microsoft.Search")
$cacheAvailable =  $providers.Contains("Microsoft.Cache")
$serviceBusAvailable =  $providers.Contains("Microsoft.ServiceBus")
$appServiceAvailable =  $providers.Contains("Microsoft.Web")
$sqlAvailable =  $providers.Contains("Microsoft.Sql")

Write-Host "CDN is available: $cdnAvailable"
Write-Host "Cosmos DB is available: $documentDbAvailable"
Write-Host "Storage is available: $storageAvailable"
Write-Host "Search Service is available: $searchAvailable"
Write-Host "Redis Cache is available: $cacheAvailable"
Write-Host "Service Bus is available: $serviceBusAvailable"
Write-Host "SQL Server is available: $sqlAvailable"

if($cdnAvailable -and $documentDbAvailable -and ` 
   $storageAvailable -and $searchAvailable -and `
   $cacheAvailable -and $serviceBusAvailable -and `
   $appServiceAvailable -and $sqlAvailable )
{
    Write-Host "Region $Region is Valid Region for Online.Store application"
}
else
{
    Write-Host "Not all resource types are available in region $Region. Maybe use a different one?"
}

<#
# Check available regions
Get-AzureRmLocation | select Location

# Check the available providers per region
$providers = (Get-AzureRmLocation | Where-Object {$_.Location -eq 'westeurope'}).Providers

# CDN
(Get-AzureRmResourceProvider -ListAvailable | Where-Object {$_.ProviderNamespace -eq 'Microsoft.Cdn'}).Locations

# DocumentDB
(Get-AzureRmResourceProvider -ListAvailable | Where-Object {$_.ProviderNamespace -eq 'Microsoft.DocumentDB'}).Locations

# Storage
(Get-AzureRmResourceProvider -ListAvailable | Where-Object {$_.ProviderNamespace -eq 'Microsoft.Storage'}).Locations

# Search Service
(Get-AzureRmResourceProvider -ListAvailable | Where-Object {$_.ProviderNamespace -eq 'Microsoft.Search'}).Locations

# Redis Cache
(Get-AzureRmResourceProvider -ListAvailable | Where-Object {$_.ProviderNamespace -eq 'Microsoft.Cache'}).Locations

# Service Bus
(Get-AzureRmResourceProvider -ListAvailable | Where-Object {$_.ProviderNamespace -eq 'Microsoft.ServiceBus'}).Locations

# App Service
(Get-AzureRmResourceProvider -ListAvailable | Where-Object {$_.ProviderNamespace -eq 'Microsoft.Web'}).Locations

# SQL Server - Database
(Get-AzureRmResourceProvider -ListAvailable | Where-Object {$_.ProviderNamespace -eq 'Microsoft.Sql'}).Locations
#>

<#
Function CheckAvailability {
[cmdletbinding()]
Param ([string]$region) 
    Process {
        Write-Host "Checking availability for region $region.."
        $providers = (Get-AzureRmLocation | Where-Object {$_.Location -eq 'westeurope'}).Providers
        Write-Host $providers
    } 
}

CheckAvailability -region westeurope
#>