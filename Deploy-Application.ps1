Param(
    [Parameter(Mandatory=$true)]
    [string]$WebAppName,
    [Parameter(Mandatory=$true)]
    [string]$WebAppUri)

Function GetAuthToken
{
    param(
        [Parameter(Mandatory=$true)]
        $TenantId)

    Import-Module Azure
    $clientId = "1950a258-227b-4e31-a9cf-717495945fc2" # Set well known client ID for AzurePowershell Application 
    $redirectUri = "urn:ietf:wg:oauth:2.0:oob"
    $resourceAppIdURI = "https://graph.windows.net"
    $authority = "https://login.microsoftonline.com/$TenantId"
    $authContext = New-Object "Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext" -ArgumentList $authority
    $authResult = $authContext.AcquireToken($resourceAppIdURI, $clientId, $redirectUri, [Microsoft.IdentityModel.Clients.ActiveDirectory.PromptBehavior]::RefreshSession)
    return $authResult
}

$tenant = Login-AzureRmAccount

#Register the WebApplication in AAD
$password = [System.Guid]::NewGuid().ToString()
$webApplicaton = New-AzureRmADApplication -DisplayName $WebAppName -IdentifierUris $WebAppUri -AvailableToOtherTenants $true -ReplyUrls @("https://localhost:44377/signin-oidc", "https://localhost:44377/Account/ProcessSignUpCode")
$secret = New-AzureRmADAppCredential -ApplicationId $webApplicaton.ApplicationId -Password (ConvertTo-SecureString -String $password -AsPlainText -Force)


# Allow SignIn Users and Read Directory Dat
$requiredResourceAccess =@{requiredResourceAccess =
    @(@{
        resourceAppId = "00000002-0000-0000-c000-000000000000"
        resourceAccess = @(
        @{
            id = "311a71cc-e848-46a1-bdf8-97ff7156d8e6"
            type= "Scope"
        },
        @{
            id = "5778995a-e1bf-45b8-affa-663a9f3f4d04"
            type = "Scope"
        })
    })
}


$tenantId = $tenant.Context.Tenant.Id
$appId = $webApplicaton.ObjectId
$url = "https://graph.windows.net/$tenantId/applications/$($appId)?api-version=1.6"
$token = GetAuthToken -TenantId $tenantId

$headers = @{
'Content-Type' = 'application/json' 
'Authorization' = $token.CreateAuthorizationHeader()
}

$json = $requiredResourceAccess | ConvertTo-Json -Depth 4 -Compress

$result = Invoke-RestMethod -Uri $url -Method Patch -Headers $headers -Body $json -ContentType "application/json"

return @{
    ClientId = $webApplicaton.ApplicationId
    ClientSecret = $password
    }
