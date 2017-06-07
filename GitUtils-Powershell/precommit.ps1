
$scriptPath = (split-path -parent $MyInvocation.MyCommand.Definition)
$configPath = $scriptPath + "\resources\web.config"
$dllPath = $scriptPath + "\GitUtils.dll"
$randomValue = Get-Random

Add-Type -literalpath $dllPath

[GitUtils.WebConfigUtilities]::ResetCredentials($configPath,$randomValue,$randomValue)
git add $configPath