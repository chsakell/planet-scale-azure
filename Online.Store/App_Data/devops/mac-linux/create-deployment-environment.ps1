# AppVeyor Environments & Deployments
# https://www.appveyor.com/docs/api/environments-deployments/
<#
.SYNOPSIS
Starts a deployment in AppVeyor

.Author: Christos Sakellarios

.PARAMETER token
AppVeyor API Token

.PARAMETER accountName
AppVeyor account

.PARAMETER projectSlug
Last part from the project's url in AppVeyor

.PARAMETER webappName
App Service name <child-resource-group>

.PARAMETER resourceGroupName
App Service name <child-resource-group>

#>

param (
    [Parameter(Mandatory = $true)] [string] $token,
    [Parameter(Mandatory = $true)] [string] $projectSlug,
    [Parameter(Mandatory = $true)] [string] $webappName,
    [Parameter(Mandatory = $true)] [string] $resourceGroupName,
    [Parameter(Mandatory = $false)] [string] $slot
)

$headers = @{
  "Authorization" = "Bearer $token"
  "Content-type" = "application/json"
}

if(!$slot) {
    $slot = "production";
}

####################################################################
# Get App Service publish profile
# https://docs.microsoft.com/en-us/powershell/module/azurerm.websites/get-azurermwebappslotpublishingprofile?view=azurermps-5.1.1

$publishProfile = Get-AzureRmWebAppSlotPublishingProfile -ResourceGroupName "$resourceGroupName" `
    -Name "$webappName" -Format "WebDeploy" -Slot "$slot"

$xml = [xml]$publishProfile
$msDeployNode = $xml | Select-XML -XPath "//*[@publishMethod='MSDeploy']"

$publishUrl = $msDeployNode.Node.publishUrl
$userName = $msDeployNode.Node.userName
$password = $msDeployNode.Node.userPWD


# Get Environment
$deploymentEnvironmentName = "$webappName"

if($slot -and $slot -ne "production") {
    $deploymentEnvironmentName = "$webappName-$slot"
}

$environments = Invoke-RestMethod -Uri "https://ci.appveyor.com/api/environments/" `
    -Headers $headers -Method Get

$queryEnvironment = ($environments | Where-Object name -eq $deploymentEnvironmentName)

if($queryEnvironment) {
    $deploymentEnvironmentId = $queryEnvironment.deploymentEnvironmentId;

    Write-Host "Updating deployment environment..."
    # Update Environment
    $updatedEnvironment = @{
        deploymentEnvironmentId = "$deploymentEnvironmentId";
        name = "$deploymentEnvironmentName";
        environmentAccessKey = "WebDeploy";
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    settings = @{
        providerSettings = @(
            @{
                name = "server";
                value = @{
                    value = "https://$publishUrl/msdeploy.axd?site=$webappName";
                    isEncrypted = $false;
                }
             },
             @{
                name = "enable_ssl";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "website";
                value = @{
                   value = "$webappName";
                   isEncrypted = $false;
                }
             },
             @{
                name = "username";
                value = @{
                    value = "$userName";
                    isEncrypted = $false;
                }
             },
             @{
                name = "password";
                value = @{
                    value = "$password";
                    isEncrypted = $true;
                }
             },
             @{
                name = "artifact";
                value = @{
                    value = "WebSite";
                    isEncrypted = $false;
                }
             },
             @{
                name = "aspnet_core";
                value = @{
                    value = "true";
                    isEncrypted = $false;
                }
             },
             @{
                name = "ntlm";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "remove_files";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "app_offline";
                value = @{
                    value = "true";
                    isEncrypted = $false;
                }
             },
             @{
                name = "app_offline";
                value = @{
                    value = "true";
                    isEncrypted = $false;
                }
             },
             @{
                name = "do_not_use_checksum";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_retry_attempts";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_retry_interval";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "skip_dirs";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "skip_files";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "pre_sync";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "post_sync";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_wait_attempts";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_wait_interval";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "aspnet_core_force_restart";
                value = @{
                    value = "true";
                    isEncrypted = $false;
                }
             }
        );
        environmentVariables = @();
    };
    }

    $updatedEnvironment = ConvertTo-Json $updatedEnvironment -Depth 4

    $createdEnvironment = Invoke-RestMethod -Uri 'https://ci.appveyor.com/api/environments' `
        -Headers $headers -Method Put -Body $updatedEnvironment

    <# Or delete..
        Write-Host "Deleting current Deployment Environment..."
        # Delete an Environment
        Invoke-RestMethod -Uri "https://ci.appveyor.com/api/environments/$deploymentEnvironmentId" `
        -Headers $headers -Method Delete
    #>
 }
 else {
 Write-Host "Creating new Deployment Environment..."

    # Add an Environment 
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            $newEnvironment = @{
    name = "$deploymentEnvironmentName";
    provider = "WebDeploy";
    settings = @{
        providerSettings = @(
            @{
                name = "server";
                value = @{
                    value = "https://$publishUrl/msdeploy.axd?site=$webappName";
                    isEncrypted = $false;
                }
             },
             @{
                name = "enable_ssl";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "website";
                value = @{
                   value = "$webappName";
                   isEncrypted = $false;
                }
             },
             @{
                name = "username";
                value = @{
                    value = "$userName";
                    isEncrypted = $false;
                }
             },
             @{
                name = "password";
                value = @{
                    value = "$password";
                    isEncrypted = $true;
                }
             },
             @{
                name = "artifact";
                value = @{
                    value = "WebSite";
                    isEncrypted = $false;
                }
             },
             @{
                name = "aspnet_core";
                value = @{
                    value = "true";
                    isEncrypted = $false;
                }
             },
             @{
                name = "ntlm";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "remove_files";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "app_offline";
                value = @{
                    value = "true";
                    isEncrypted = $false;
                }
             },
             @{
                name = "app_offline";
                value = @{
                    value = "true";
                    isEncrypted = $false;
                }
             },
             @{
                name = "do_not_use_checksum";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_retry_attempts";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_retry_interval";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "skip_dirs";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "skip_files";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "pre_sync";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "post_sync";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_wait_attempts";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_wait_interval";
                value = @{
                    value = $null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "aspnet_core_force_restart";
                value = @{
                    value = "true";
                    isEncrypted = $false;
                }
             }
        );
        environmentVariables = @();
    };
    }

    $newEnvironment = ConvertTo-Json $newEnvironment -Depth 4

    $createdEnvironment = Invoke-RestMethod -Uri 'https://ci.appveyor.com/api/environments' `
        -Headers $headers -Method Post -Body $newEnvironment

}
