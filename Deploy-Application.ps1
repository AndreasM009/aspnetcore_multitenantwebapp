Param(
    [Parameter(Mandatory=$true)]
    [string]$WebAppName,
    [Parameter(Mandatory=$true)]
    [string]$WebAppUri,
    [Parameter(Mandatory=$true)]
    [string]$WebApiName,
    [Parameter(Mandatory=$true)]
    [string]$WebApiUri)

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
$webApiPassword = [System.Guid]::NewGuid().ToString();
$webApplicaton = New-AzureRmADApplication -DisplayName $WebAppName -IdentifierUris $WebAppUri -AvailableToOtherTenants $true -ReplyUrls @("https://localhost:44377/signin-oidc", "https://localhost:44377/Account/ProcessSignUpCode")
$webApi = New-AzureRmADApplication -DisplayName $WebApiName -IdentifierUris $WebApiUri -AvailableToOtherTenants $false -ReplyUrls "https://localhost:44355/signin-oidc"

$webApplicationSecret = New-AzureRmADAppCredential -ApplicationId $webApplicaton.ApplicationId -Password (ConvertTo-SecureString -String $password -AsPlainText -Force)
$webAppliSecret = New-AzureRmADAppCredential -ApplicationId $webApi.ApplicationId -Password (ConvertTo-SecureString -String $webApiPassword -AsPlainText -Force)


# Allow SignIn Users and Read Directory Dat
$webAppRequiredResourceAccess =@{requiredResourceAccess = @(
    @{
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
    },
    @{
        resourceAppId = "49956254-82f3-49a1-b247-0b0a10e08f73"
        resourceAccess = @(
        @{
            id = "5dd66fad-4d2d-4e20-b1ea-251994040984"
            type = "Scope"
        })  
    })
}

$webApiRequiredPermissions = @{
        knownClientApplications = @($webApplicaton.ApplicationId)
        requiredResourceAccess = @(
        @{
            resourceAppId = "00000002-0000-0000-c000-000000000000"
            resourceAccess = @(
            @{
                id = "311a71cc-e848-46a1-bdf8-97ff7156d8e6"
                type = "Scope"
            },
            @{
                id = "5778995a-e1bf-45b8-affa-663a9f3f4d04"
                type = "Scope"
            },
            @{
                id = "cba73afc-7f69-4d86-8450-4978e04ecd1a"
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

$json = $webAppRequiredResourceAccess | ConvertTo-Json -Depth 4 -Compress

$result = Invoke-RestMethod -Uri $url -Method Patch -Headers $headers -Body $json -ContentType "application/json"

$appId = $webApi.ObjectId
$url = "https://graph.windows.net/$tenantId/applications/$($appId)?api-version=1.6"

$json = $webApiRequiredPermissions | ConvertTo-Json -Depth 4 -Compress
$result = Invoke-RestMethod -Uri $url -Method Patch -Headers $headers -Body $json -ContentType "application/json"

return @{
    WebAppClientId = $webApplicaton.ApplicationId
    WebAppClientSecret = $password
    WebApiClientId = $webApi.ApplicationId
    WebApiClienSecret = $webApiPassword
    WebApiUri = $WebAppUri
}
