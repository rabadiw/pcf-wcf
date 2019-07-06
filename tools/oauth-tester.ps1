$basePath = (Split-Path $Script:PSCommandPath)
$oauthData = (Get-Content $basePath\oauth-test.json | ConvertFrom-Json)
$oauth_token_url = $oauthData.OAuthTokenUrl
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
    -Uri ${oauth_token_url} | ConvertTo-Json

    