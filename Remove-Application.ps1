Param(
[Parameter(Mandatory=$true)]
[string]$WebAppApplicationId,
[Parameter(Mandatory=$true)]
[string]$webApiApplicationId)

Login-AzureRmAccount

$webapp = Get-AzureRmADApplication -ApplicationId $WebAppApplicationId
$apiapp = Get-AzureRmADApplication -ApplicationId $webApiApplicationId
Set-AzureRmADApplication -ApplicationId $WebAppApplicationId -AvailableToOtherTenants $false
Remove-AzureRmADApplication -ObjectId $webapp.ObjectId -Force
Remove-AzureRmADApplication -ObjectId $apiapp.ObjectId -Force