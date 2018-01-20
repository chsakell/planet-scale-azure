# AppVeyor Environments & Deployments
# https://www.appveyor.com/docs/api/environments-deployments/

param (
    [Parameter(Mandatory = $true)] [string] $token,
    [Parameter(Mandatory = $true)] [string] $accountName,
    [Parameter(Mandatory = $true)] [string] $projectSlug,
    [Parameter(Mandatory = $true)] [string] $webappName,
    [Parameter(Mandatory = $true)] [string] $resourceGroupName
)

$headers = @{
  "Authorization" = "Bearer $token"
  "Content-type" = "application/json"
}

####################################################################
# Get App Service publish profile
# https://docs.microsoft.com/en-us/powershell/module/azurerm.websites/get-azurermwebappslotpublishingprofile?view=azurermps-5.1.1

$publishProfile = Get-AzureRmWebAppSlotPublishingProfile -ResourceGroupName "$resourceGroupName" `
    -Name "$webappName" -Format "WebDeploy" -Slot "production"

$xml = [xml]$publishProfile
$msDeployNode = $xml | Select-XML -XPath "//*[@publishMethod='MSDeploy']"

$publishUrl = $msDeployNode.Node.publishUrl
$userName = $msDeployNode.Node.userName
$password = $msDeployNode.Node.userPWD


# Get Environment
$environments = Invoke-RestMethod -Uri "https://ci.appveyor.com/api/environments/" `
    -Headers $headers -Method Get

$queryEnvironment = ($environments | Where-Object name -eq $webappName)

if($queryEnvironment) {
    $deploymentEnvironmentId = $queryEnvironment.deploymentEnvironmentId;

    Write-Host "Updating deployment environment..."
    # Update Environment
    $updatedEnvironment = @{
        deploymentEnvironmentId = "$deploymentEnvironmentId";
        name = "$webappName";
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
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "remove_files";
                value = @{
                    value = null;
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
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_retry_attempts";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_retry_interval";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "skip_dirs";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "skip_files";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "pre_sync";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "post_sync";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_wait_attempts";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_wait_interval";
                value = @{
                    value = null;
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
    name = "$webappName";
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
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "remove_files";
                value = @{
                    value = null;
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
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_retry_attempts";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_retry_interval";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "skip_dirs";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "skip_files";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "pre_sync";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "post_sync";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_wait_attempts";
                value = @{
                    value = null;
                    isEncrypted = $false;
                }
             },
             @{
                name = "sync_wait_interval";
                value = @{
                    value = null;
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

