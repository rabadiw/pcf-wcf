$basePath = (Split-Path $Script:PSCommandPath)
$oauthData = (Get-Content $basePath\oauth-test.json | ConvertFrom-Json)
$auth_domain = $oauthData.AuthDomain
$client_id = $oauthData.AppID
$client_secret = $oauthData.AppSecret

$Body = @{
    grant_type     = "client_credentials"
    client_id      = $client_id
    client_secret  = $client_secret
    scope          = 'openid'
}

Invoke-RestMethod `
    -Method Post `
    -Body $Body `
    -Uri "${auth_domain}/oauth/token" | ConvertTo-Json 
