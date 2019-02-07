<#
	This is a custom script file (template) that can help you run all the scripts inside the devops folder
	Open a Powershell session and cd to this folder
	Every time you want to run a script make sure you change anything required 
	such as resource group location or login and password for SQL Databases, slots etc..

	Ideally, this code should be placed at a devops.ps1 file inside the devops folder 
	and not be tracked by source control
#>
ECHO OFF

# Add-AzureAccount 
$subscription = Login-AzureRmAccount;
$subscription.SubscriptionName
# select subscription
$subscriptionId = "YOUR-SUBSCRIPTION-ID";
Write-Host "Selecting subscription";
Select-AzureRmSubscription -SubscriptionID $subscriptionId;

# Select-AzureSubscription -Default -SubscriptionName "YOUR SUBSCRIPTION NAME"

ECHO OFF
# Clear all variables..
Remove-Variable * -ErrorAction SilentlyContinue

$primaryName = "primary-name-here"

#####################################################################################################
# Init Primary Region Resources
.\init-primary-resources.ps1 -PrimaryName "$primaryName" -ResourceGroupLocation "westeurope" `
                             -CreateIdentityDatabase $true -SqlServerLogin "YOUR-LOGIN" -SqlServerPassword "YOUR-PASSWORD"


#####################################################################################################
# Init Parent Region Resources
.\init-parent-resources.ps1 -PrimaryName "$primaryName" -ResourceGroupLocation "westeurope"


#####################################################################################################
# Init Child Resources
.\init-child-resources.ps1 -PrimaryName "$primaryName" -ResourceGroupLocation "westeurope" `
                                -SqlServerVersion "a" -SqlServerLogin "YOUR-LOGIN" -SqlServerPassword "YOUR-PASSWORD" `
                                -CreateDatabase $true -Database "$primaryName" -DocumentDBPolicies "West Europe" `
                                -UseIdentity $true -IdentitySqlServerLogin "YOUR-LOGIN" -IdentitySqlServerPassword "YOUR-PASSWORD"
                                #-AzureIDClientSecret "YOUR-AZURE-ID-CLIENT-SECRET" `
#####################################################################################################
# Init Child Slot Resources
.\add-child-slot.ps1 -PrimaryName "$primaryName" -ResourceGroupLocation "westeurope" `
                                -SqlServerVersion "a" -SqlServerLogin "YOUR-LOGIN" -SqlServerPassword "YOUR-PASSWORD" `
                                -Database "$primaryName" -DocumentDBPolicies "West Europe" `
                                -UseIdentity $true -IdentitySqlServerLogin "YOUR-LOGIN" -IdentitySqlServerPassword "YOUR-PASSWORD"
                                #-AzureIDClientSecret "YOUR-AZURE-ID-CLIENT-SECRET" `
#####################################################################################################
# Upload Web Job
.\deploy-webjob.ps1 -PrimaryDatabaseServer "$primaryName-westeurope-app-sql-a" `
                    -Database "$primaryName" `
                    -SqlServerLogin "YOUR-LOGIN" `
                    -SqlServerPassword "YOUR-PASSWORD" `
                    -WebappParentResourceGroup "$primaryName-westeurope"`
                    -WebappResourceGroup "$primaryName-westeurope-app" `
                    -WebjobAppLocation "PATH-TO\planet-scale-azure\Online.Store.WebJob" `
                    -DeploymentDestinationFolder "PATH-TO-SOME-FOLDER-ON-YOUR-PC" `
                    -DotNetCoreVersion "2.2" ` # in order to resolve the publish path
                    -slot "upgrade"



#####################################################################################################
# Init Geo-Replication
.\init-geo-replication.ps1 -Database "$primaryName" `
                            -PrimaryResourceGroupName "$primaryName-westeurope-app" `
                            -PrimaryServerName "$primaryName-westeurope-app-sql-b" `
                            -SecondaryResourceGroupName "$primaryName-westeurope-app" `
                            -SecondaryServerName "$primaryName-westeurope-app-sql-a"


#####################################################################################################
# Set Geo-Replication
.\set-geo-replication.ps1 `
        -Database "$primaryName" `
        -PrimaryServerName "$primaryName-westeurope-app-sql-b" `
        -SecondaryServerName "$primaryName-westeurope-app-sql-a" `
        -Action "Stop"

#####################################################################################################
# Create AppVeyor Environment

.\create-deployment-environment.ps1 -token "YOUR-APPVEYOR-TOKEN" `
                         -projectSlug "planet-scale-azure" `
                         -webappName "$primaryName-westeurope-app" `
                         -resourceGroupName "$primaryName-westeurope-app" `
                         -slot "upgrade"


#####################################################################################################
# Start deployment

.\start-deployment.ps1  -token "YOUR-APPVEYOR-TOKEN" -accountName "APPVEYOR-ACCOUNT-NAME" `
                        -projectSlug "YOUR-APPVEYOR-PROJECT-SLUG" `
                        -webappName "$primaryName-westeurope-app" `
                        -resourceGroupName "$primaryName-westeurope-app" `
                        -deploymentEnvironment "$primaryName-westeurope-app" `
                        -slot "upgrade"

#####################################################################################################
# Enable / Disable Traffic Manager Profile endpoints

.\set-traffic-manager-endpoint.ps1 -EndpointName "$primaryName-westeurope-app" `
                                   -TraficManagerProfile "$primaryName" `
                                   -ResourceGroupName "$primaryName" `
                                   -Enable $true

#####################################################################################################
# Swap deployment slots
Switch-AzureRmWebAppSlot -ResourceGroupName "$primaryName-westeurope-app" `
                         -Name "$primaryName-westeurope-app" `
                         -SourceSlotName "upgrade" `
                         -DestinationSlotName "production"  

<#

Remove-AzureRmResourceGroup -Name "$primaryName-westeurope"
Remove-AzureRmResourceGroup -Name "$primaryName-westcentralus"
Remove-AzureRmResourceGroup -Name "$primaryName-westeurope-app"
Remove-AzureRmResourceGroup -Name "$primaryName-westcentralus-app"

Remove-AzureRmResourceGroup -Name "$primaryName"
#>

#####################################################################################################
