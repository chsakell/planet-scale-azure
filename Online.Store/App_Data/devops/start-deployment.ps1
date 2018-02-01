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

.PARAMETER deploymentEnvironment
App Service name <child-resource-group>

.PARAMETER build
build to deployed - optional. If omitted the lastest build will be used
#>

param (
    [Parameter(Mandatory = $true)] [string] $token,
    [Parameter(Mandatory = $true)] [string] $accountName,
    [Parameter(Mandatory = $true)] [string] $projectSlug,
    [Parameter(Mandatory = $true)] [string] $webappName,
    [Parameter(Mandatory = $true)] [string] $resourceGroupName,
    [Parameter(Mandatory = $true)] [string] $deploymentEnvironment,
    [Parameter(Mandatory = $false)] [string] $build,
    [Parameter(Mandatory = $false)] [string] $slot
)
Clear-Host
# Function to check if node process running (npm install..)
 Function NodeRunning {
[cmdletbinding()]
Param ([string]$webappName, [string]$auth) 
    Process {
        $processesAPI = "https://$webappName.scm.azurewebsites.net/api/processes/";

        if($slot) {
            $processesAPI = "https://$webappName-$slot.scm.azurewebsites.net/api/processes/";
        }

        Write-Host "Invoking 'get processes' on $webappName ..."

        $processes = Invoke-RestMethod -Uri $processesAPI `
        -Headers @{Authorization=("Basic {0}" -f $auth)} -Method Get `
        -ErrorAction SilentlyContinue

        $nodeProcesses = ($processes | Where-Object {$_.name -eq "node"});

        return $nodeProcesses;
    } 
}

# AppVeyor settings
$headers = @{
  "Authorization" = "Bearer $token"
  "Content-type" = "application/json"
}

####################################################################\
# Get project's last build

$latestBuild = Invoke-RestMethod -Uri "https://ci.appveyor.com/api/projects/$accountName/$projectSlug" `
    -Headers $headers -Method Get

####################################################################
# Start Deployment
$body = @{
    environmentName = "$deploymentEnvironment";
    accountName = "$accountName";
    projectSlug = "$projectSlug";
    buildVersion = $latestBuild.build.version;
}

if($build) {
    $body.buildVersion = "$build";
}

ConvertTo-Json $body

$StartDate=(GET-DATE)

$deployment = Invoke-RestMethod -Uri 'https://ci.appveyor.com/api/deployments' `
    -Headers $headers -Method Post -Body (ConvertTo-Json $body)


####################################################################
# Get Deployment - Post Build Events

$deploymentId = $deployment.deploymentId;
$runningDeployment = Invoke-RestMethod -Uri "https://ci.appveyor.com/api/deployments/$deploymentId" `
    -Headers $headers -Method Get
$runningDeploymentStatus = $runningDeployment.deployment.status;

while($runningDeploymentStatus -eq "running" -or $runningDeploymentStatus -eq "queued" ){
    Write-Host "Publishing artifacts to [$webappName] Slot: $slot Status: $runningDeploymentStatus"
    # Sleep for 2 seconds..
    Start-Sleep -s 2
    $runningDeployment = Invoke-RestMethod -Uri "https://ci.appveyor.com/api/deployments/$deploymentId" `
        -Headers $headers -Method Get
    $runningDeploymentStatus = $runningDeployment.deployment.status;
}
if($runningDeploymentStatus -eq "success") {
    if(!$slot) {
        $slot = "production";
    }

    Write-Host "Artifacts deployed succsfully.."
    # Run additional scripts here..
    # Get App Service publish profile

    Write-Host "Retrieving publish profile information.."
    $publishProfile = Get-AzureRmWebAppSlotPublishingProfile -ResourceGroupName "$resourceGroupName" `
        -Name "$webappName" -Format "WebDeploy" -Slot "$slot"

    $xml = [xml]$publishProfile
    $msDeployNode = $xml | Select-XML -XPath "//*[@publishMethod='MSDeploy']"

    $userName = $msDeployNode.Node.userName
    $password = $msDeployNode.Node.userPWD

    $auth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $userName,$password)))
    $body = '{ "command": "npm install", "dir": "site/wwwroot" }'
    $apiUrl = "https://$webappName.scm.azurewebsites.net/api/command"

    if($slot -ne "production") {
        $apiUrl = "https://$webappName-$slot.scm.azurewebsites.net/api/command"
    }

    Write-Output "Invoking 'npm install' on $webappName Slot: $slot ..."

    Try {
        $response = Invoke-RestMethod -Uri $apiUrl -Headers @{Authorization=("Basic {0}" -f $auth)} -Method Post `
                    -ContentType "application/json" -Body $body -TimeoutSec 1500 -ErrorAction Stop
    
        Write-Output $response
        Write-Output $response.Output
    }
    Catch {
        Write-Host "Waiting for npm install...";

        $nodeProcesses = NodeRunning -webappName $webappName -auth $auth;
        $nodeProcesses;
        while(!($nodeProcesses -eq $null)) {
            Write-Host "npm install still running..";
            # Sleep for 20 seconds..
            Start-Sleep -s 20
            $nodeProcesses = NodeRunning -webappName $webappName -auth $auth;
        }
    }


    Write-Host "Deployment finished succesfully.."
}
else {
    Write-Host "Deployment failed with status: $runningDeploymentStatus "
}

$EndDate=(GET-DATE)

$timeEllapsed = NEW-TIMESPAN –Start $StartDate –End $EndDate
$totalMinutes = $timeEllapsed.Minutes
$totalSeconds = $timeEllapsed.Seconds

Write-Host "Total Deployment Time: $totalMinutes minutes and $totalSeconds seconds"

# Send a beep
[console]::beep(1000,500)