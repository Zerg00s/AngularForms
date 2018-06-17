 param (
    [string]$Path
 )

 Write-Host $Path -ForegroundColor Green
. .\DeploymentSteps\DEPLOYMENT_CONFIG.ps1
. .\DeploymentSteps\DEPLOY_LISTS_AND_FIELDS.ps1
. .\DeploymentSteps\ADD_TEST_DATA.ps1
