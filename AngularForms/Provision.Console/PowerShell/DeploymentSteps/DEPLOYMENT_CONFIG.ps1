$ErrorActionPreference = "Stop";

Clear-Host

if ($path -ne $null){
	Set-Location $path
	Write-Host $path
}

if($TARGET_SITE_URL -eq $null -or [String]::IsNullOrEmpty($TARGET_SITE_URL)){
    [void][Reflection.Assembly]::LoadWithPartialName('Microsoft.VisualBasic')
    $title = 'SITE URL'
    $msg   = 'Enter full site url:'
    $TARGET_SITE_URL = [Microsoft.VisualBasic.Interaction]::InputBox($msg, $title)

    $TARGET_SITE_URL = $TARGET_SITE_URL.Trim('/');

    $uri = [System.Uri]$TARGET_SITE_URL
    $SERVER_RELATIVE_WEB = $uri.AbsolutePath

    if ($TARGET_SITE_URL -eq $null -or $TARGET_SITE_URL -eq ""){
	    exit
	    Throw "no URL supplied. Exiting the script"
    }

    $credentials = Get-Credential

    if ($credentials -ne $null){
	    Connect-PnPOnline $TARGET_SITE_URL -Credentials $credentials
    }
    else{
	    Connect-PnPOnline $TARGET_SITE_URL -CurrentCredentials
    }
}

$web = Get-PnPWeb
$web.ServerRelativeUrl

Write-Host "Connected to the site:" $web.ServerRelativeUrl