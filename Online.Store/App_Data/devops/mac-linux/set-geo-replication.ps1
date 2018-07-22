<#
.SYNOPSIS
# Stop or Start geo-repclication to ghost secondary databases
# https://docs.microsoft.com/en-us/powershell/module/azure/stop-azuresqldatabasecopy?view=azuresmps-4.0.0
# https://docs.microsoft.com/en-us/powershell/module/azure/start-azuresqldatabasecopy?view=azuresmps-4.0.0


.Author: Christos Sakellarios

.PARAMETER Database
The Database to start or stop being geo-replicated.

.PARAMETER PrimaryServerName
The primary's database SQL Server

.PARAMETER SecondaryServerName
The secondary's database SQL Server

.PARAMETER Action
Start or Stop geo-replication. Possible values "Stop", "Start"

#>
param (
    [Parameter(Mandatory = $true)] [string] $Database,
    [Parameter(Mandatory = $true)] [string] $PrimaryServerName,
    [Parameter(Mandatory = $true)] [string] $SecondaryServerName,
    [Parameter(Mandatory = $true)] [string] $Action
)

ECHO OFF
Clear-Host

if($Action -eq "Stop") 
{
    Write-Host "Stopping Geo-Replication from $PrimaryServerName to $SecondaryServerName..."

    Stop-AzureSqlDatabaseCopy -ServerName $PrimaryServerName `
                              -DatabaseName $Database `
                              -PartnerServer $SecondaryServerName

    Write-Host "Geo-Replication stopped successfully..."
}

if($Action -eq "Start") 
{
    Write-Host "Starting Geo-Replication from $PrimaryServerName to $SecondaryServerName..."
    
    Start-AzureSqlDatabaseCopy -ServerName $PrimaryServerName `
                              -DatabaseName $Database `
                              -PartnerServer $SecondaryServerName `
                              -ContinuousCopy

    Write-Host "Geo-Replication configured successfully..."
}
