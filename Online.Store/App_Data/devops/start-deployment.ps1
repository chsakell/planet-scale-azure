# AppVeyor Environments & Deployments
# https://www.appveyor.com/docs/api/environments-deployments/

# AppVeyor settings
$token = 'appveyor-api-token' # API Token Page in AppVeyor
$accountName = 'app-veyor-account-name'
$projectSlug = 'app-veyor-project-name-from-url'
$headers = @{
  "Authorization" = "Bearer $token"
  "Content-type" = "application/json"
}
$deploymentEnvironment = "dotnetcoretoazure"
# Azure App Service settings
$webappName = "dotnetcoretoazure"
$resourceGroupName = "dotnetcoretoazure"

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

while($runningDeploymentStatus -eq "running"){
    Write-Host "Publishing artifacts to [$webappName] Status: $runningDeploymentStatus"
    # Sleep for 2 seconds..
    Start-Sleep -s 2
    $runningDeployment = Invoke-RestMethod -Uri "https://ci.appveyor.com/api/deployments/$deploymentId" `
        -Headers $headers -Method Get
    $runningDeploymentStatus = $runningDeployment.deployment.status;
}
if($runningDeploymentStatus -eq "success") {
    Write-Host "Artifacts deployed succsfully.."
    # Run additional scripts here..
    # Get App Service publish profile

    Write-Host "Retrieving publish profile information.."
    $publishProfile = Get-AzureRmWebAppSlotPublishingProfile -ResourceGroupName "$resourceGroupName" `
        -Name "$webappName" -Format "WebDeploy" -Slot "production"

    $xml = [xml]$publishProfile
    $msDeployNode = $xml | Select-XML -XPath "//*[@publishMethod='MSDeploy']"

    $userName = $msDeployNode.Node.userName
    $password = $msDeployNode.Node.userPWD

    $auth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $userName,$password)))
    $body = '{ "command": "npm install", "dir": "site/wwwroot" }'
    $apiUrl = "https://$webappName.scm.azurewebsites.net/api/command"

    Write-Output "Invoking 'npm install' on $webappName ..."

    $response = Invoke-RestMethod -Uri $apiUrl -Headers @{Authorization=("Basic {0}" -f $auth)} -Method Post `
                -ContentType "application/json" -Body $body

    Write-Output $response
    Write-Output $response.Output
    Write-Host "Deployment finished succesfully.."
}
else {
    Write-Host "Deployment failed with status: $runningDeploymentStatus "
}

$EndDate=(GET-DATE)

$timeEllapsed = NEW-TIMESPAN –Start $StartDate –End $EndDate
$totalMinutes = $timeEllapsed.Minutes
$totalSeconds = $timeEllapsed.Seconds

Write-Host "Total Deployment Time: $totalMinutes and $totalSeconds seconds"