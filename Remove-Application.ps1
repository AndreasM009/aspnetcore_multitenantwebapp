Param(
[Parameter(Mandatory=$true)]
[string]$ApplicationId)

Login-AzureRmAccount

$app = Get-AzureRmADApplication -ApplicationId $ApplicationId
Set-AzureRmADApplication -ApplicationId $ApplicationId -AvailableToOtherTenants $false
Remove-AzureRmADApplication -ObjectId $app.ObjectId -Force