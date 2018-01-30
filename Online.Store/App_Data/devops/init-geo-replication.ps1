<#
.SYNOPSIS
Establish Active Geo-Replication between two regions. Database on the secondary region should not exist before the srcript runs
https://docs.microsoft.com/en-us/azure/sql-database/scripts/sql-database-setup-geodr-and-failover-database-powershell
https://docs.microsoft.com/en-us/powershell/module/azurerm.sql/new-azurermsqldatabasesecondary?view=azurermps-5.1.1

.Author: Christos Sakellarios

.PARAMETER Database
The Database name to be geo-replicated. Database should exist on the primary region server
.PARAMETER PrimaryResourceGroupName
The Resource Group name where the primary SQL Server exists
.PARAMETER PrimaryServerName (optional)
The Primary Server's name. If null the PrimaryResourceGroupName will be used
.PARAMETER SecondaryResourceGroupName
The Resource Group name where the secondary SQL Server exists
.PARAMETER SecondaryServerName (optional)
The Secondary Server's name. If null the SecondaryResourceGroupName will be used

#>
param (
    [Parameter(Mandatory = $true)] [string] $Database,
    [Parameter(Mandatory = $true)] [string] $PrimaryResourceGroupName,
    [Parameter(Mandatory = $false)] [string] $PrimaryServerName,
    [Parameter(Mandatory = $true)] [string] $SecondaryResourceGroupName,
    [Parameter(Mandatory = $false)] [string] $SecondaryServerName
)

ECHO OFF
Clear-Host

$sqlServerPrefix = "sqlserver";

if([string]::IsNullOrEmpty($PrimaryServerName)) { 
    $PrimaryServerName = "$PrimaryResourceGroupName-$sqlServerPrefix";
}

if([string]::IsNullOrEmpty($SecondaryServerName)) { 
    $SecondaryServerName = "$SecondaryResourceGroupName-$sqlServerPrefix";
}

Write-Host "Database: $Database"
Write-Host "PrimaryResourceGroupName: $PrimaryResourceGroupName"
Write-Host "PrimaryServerName: $PrimaryServerName"
Write-Host "SecondaryResourceGroupName: $SecondaryResourceGroupName"
Write-Host "SecondaryServerName: $SecondaryServerName"

Write-Host "Setting active Geo-Replication from $PrimaryServerName to $SecondaryServerName for database $Database"

$primaryDatabase = Get-AzureRmSqlDatabase -DatabaseName $Database `
    -ResourceGroupName $PrimaryResourceGroupName -ServerName $PrimaryServerName

$primaryDatabase | New-AzureRmSqlDatabaseSecondary -PartnerResourceGroupName "$SecondaryResourceGroupName" `
    -PartnerServerName "$SecondaryServerName" -AllowConnections "All"

Write-Host "Active Geo-Replication has been set succcessfully.."

# Send a beep
[console]::beep(1000,500)