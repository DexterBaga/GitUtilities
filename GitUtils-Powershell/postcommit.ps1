
$scriptPath = (split-path -parent $MyInvocation.MyCommand.Definition)
$configPath = $scriptPath + "\resources\web.config"
$dllPath = $scriptPath + "\GitUtils.dll"
$realUserName = "realUserName"
$realPassword = "realPassword"

Add-Type -literalpath $dllPath

[GitUtils.WebConfigUtilities]::ResetCredentials($configPath,$realUserName,$realPassword)