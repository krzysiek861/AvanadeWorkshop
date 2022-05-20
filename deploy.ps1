Param(
    [string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
    [string] $ResourceGroupName = 'AvanadeAzureWorkshop',
    [switch] $UploadArtifacts,
    [string] $StorageAccountName,
    [string] $StorageContainerName = $ResourceGroupName.ToLowerInvariant() + '-stageartifacts',
    [string] $TemplateFile = 'azuredeploy.json',
    [string] $TemplateParametersFile = 'azuredeploy.parameters.json',
    [string] $ArtifactStagingDirectory = '.',
    [switch] $ValidateOnly
)

$ObjectId = az ad signed-in-user show --query objectId --out tsv
$SubscriptionId = az account show --query id --out tsv

$StorageResourceGroupName = 'ARM_Deploy_Staging'
$TemplateFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $TemplateFile))
$TemplateParametersFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $TemplateParametersFile))

# Create a storage account name if none was provided
if ($StorageAccountName -eq '') {
    $StorageAccountName = 'stage' + $SubscriptionId.Replace('-', '').substring(0, 19)
}

$StorageAccount = az storage account show --name $StorageAccountName

# Create the storage account if it doesn't already exist
if ($StorageAccount -eq $null) {
    az group create --location $ResourceGroupLocation --name $StorageResourceGroupName
	az storage account create --name $StorageAccountName --resource-group $StorageResourceGroupName --location $ResourceGroupLocation --sku "Standard_LRS"
}

$BlobEndpoint = az storage account show --name $StorageAccountName --query primaryEndpoints.blob --out tsv
$ArtifactsLocation = $BlobEndpoint + $StorageContainerName

if ($UploadArtifacts) {
    # Convert relative paths to absolute paths if needed
    $ArtifactStagingDirectory = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $ArtifactStagingDirectory))

    # Parse the parameter file and update the values of artifacts location and artifacts location SAS token if they are present
    $JsonParameters = Get-Content $TemplateParametersFile -Raw | ConvertFrom-Json
    if (($JsonParameters | Get-Member -Type NoteProperty 'parameters') -ne $null) {
        $JsonParameters = $JsonParameters.parameters
    }
	
    # Copy files from the local storage staging location to the storage account container
	az role assignment create --role "Storage Blob Data Contributor" --assignee $ObjectId --scope "/subscriptions/$SubscriptionId/resourceGroups/$StorageResourceGroupName/providers/Microsoft.Storage/storageAccounts/$StorageAccountName"
	az storage container create --account-name $StorageAccountName --name $StorageContainerName --auth-mode login

	$ArtifactFilePaths = Get-ChildItem $ArtifactStagingDirectory -Recurse -File | ForEach-Object -Process {$_.FullName}
    foreach ($SourcePath in $ArtifactFilePaths) {
		az storage blob upload --account-name $StorageAccountName --container-name $StorageContainerName --name $SourcePath.Substring($ArtifactStagingDirectory.length + 1) --file $SourcePath --auth-mode login --overwrite true
	}
}

az group create -l $ResourceGroupLocation -n $ResourceGroupName

$SasToken = az storage container generate-sas --account-name $StorageAccountName --name $StorageContainerName --permissions acdlrw --expiry (Get-Date).AddDays(1).tostring("yyyy-MM-dd") --auth-mode login --as-user
$Parameters = '{ \"principalId\": { \"value\": \"' + $ObjectId + '\" }, \"_artifactsLocation\": { \"value\": \"' + $ArtifactsLocation + '\" }, \"_artifactsLocationSasToken\": { \"value\": \"' + $SasToken + '\" } }'

if ($ValidateOnly) {
    az deployment group validate --resource-group $ResourceGroupName --template-file $TemplateFile `
		--parameters $Parameters `
        --parameters $TemplateParametersFile `

}
else {
	$DeploymentName = ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm'));

    az deployment group create --name $DeploymentName --resource-group $ResourceGroupName --template-file $TemplateFile `
		--parameters $Parameters `
        --parameters $TemplateParametersFile `
}
