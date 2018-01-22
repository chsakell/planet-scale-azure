<#
.SYNOPSIS
Enable or disable Traffic Manager Endpoint

.Author: Christos Sakellarios

.PARAMETER EndpointName
App Service name

.PARAMETER TraficManagerProfile
Traffic Manager Profile Name <primary-resource-group>

.PARAMETER ResourceGroupName
Traffic Manager Profile resouroce group <primary-resource-group>

.PARAMETER Enable
Enable or Disable endpoint.

e.g. .\set-traffic-manager-endpoint.ps1 -EndpointName "<child-resource-group>" `
                                   -TraficManagerProfile "<primary-resource-group>" `
                                   -ResourceGroupName "<primary-resource-group>" `
                                   -Enable $true
#>

param (
    [Parameter(Mandatory = $true)] [string] $EndpointName,
    [Parameter(Mandatory = $true)] [string] $TraficManagerProfile,
    [Parameter(Mandatory = $true)] [string] $ResourceGroupName,
    [Parameter(Mandatory = $true)] [bool] $Enable
)

Clear-Host

# Enable disabled endpoints

if($Enable) {

Write-Host "Enabling $EndpointName...";

Enable-AzureRmTrafficManagerEndpoint -Name $EndpointName -Type AzureEndpoints `
                                     -ProfileName $TraficManagerProfile `
                                     -ResourceGroupName $ResourceGroupName
}
else {

Write-Host "Disabling $EndpointName...";

Disable-AzureRmTrafficManagerEndpoint -Name $EndpointName -Type AzureEndpoints `
                                     -ProfileName $TraficManagerProfile `
                                     -ResourceGroupName $ResourceGroupName

}

# Send a beep
[console]::beep(1000,500)