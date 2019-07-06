# Publish the project
# Use Visual Studio to create a pubish profile first

param (
    [Parameter(Mandatory = $false)][switch]$build = $false,
    [Parameter(Mandatory = $false)][ValidateSet("Debug", "Release", "ReleaseSecure")][string]$configuration = "Release",
    [Parameter(Mandatory = $false)][switch]$deploy = $false,
    [Parameter(Mandatory = $false)][switch]$secure = $false,
    [Parameter(Mandatory = $false)][string]$ssoServiceName = ""
)

Write-Host "### Parameters used:"
foreach ($Parameter in $PSCmdlet.MyInvocation.MyCommand.Parameters.Keys) {
    Get-Variable -Name $Parameter -ErrorAction SilentlyContinue
}

### Build
###

if ($build) {
    $msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    if (!(Test-Path $msbuild)) {
        Write-Host "Could not find path ${msbuild}"
        exit 1
    }
    $csprojs = @(Get-Item *.csproj)
    if ($csprojs.Count -gt 1) {
        Write-Host "Too many *.csproj files"
        exit 1
    }
    & $msbuild $csprojs[0] `
        /t:"clean;build;publish" `
        /p:OutputPath="./bin/Release" `
        /p:Configuration=$configuration `
        /p:Platform=AnyCPU `
        /p:DeployOnBuild=true `
        /p:PublishDirectory="./bin/Release/Publish" `
        /p:PublishProfile=folderprofile
}

### Deploy
###

# Login to PCF will be required before running this script
if ($deploy) {

    if ($secure -and $ssoServiceName -eq "") {
        Write-Host "### Missing SSO Service Name"
        break
    }
    
    $appName = "wcf-ws"
    if ($secure) { $appName = "${appName}-secure" }

    cf push -f .\manifest.yaml `
        --var appName=${appName} 
    
    if ($secure) {
        $ssoparams = @{
            grant_types          = @("client_credentials")
            authorities          = @("openid", "uaa.resource")
            auto_approved_scopes = @("openid")
        } | ConvertTo-Json -Compress
        
        cf bind-service $appName ${ssoServiceName} -c $($ssoparams | ConvertTo-Json -Compress)
    }
}