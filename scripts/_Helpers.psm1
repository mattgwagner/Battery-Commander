$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$URL = 'https://bc.red-leg-dev.com/api'
#$URL = 'http://localhost:50832/api'

$SolutionRoot = (Split-Path -parent $Here)

$JsonType = "application/json"

$ApiToken = ""

function BC-Get($Uri, $Body)
{
	return Invoke-RestMethod `
			-Method Get `
			-Body  (ConvertTo-Json $Body -Depth 10) `
			-Uri $Uri  `
			-Headers @{ Authorization = "Bearer $ApiToken"; Accept = $JsonType } `
			-ContentType $JsonType
}

function BC-Post($Uri, $Body)
{
	return Invoke-RestMethod `
			-Method Post `
			-Body  (ConvertTo-Json $Body -Depth 10) `
			-Uri $Uri  `
			-Headers @{ Authorization = "Bearer $ApiToken"; Accept = $JsonType } `
			-ContentType $JsonType
}

function BC-Put($Uri, $Body)
{
	return Invoke-RestMethod `
			-Method Put `
			-Body  (ConvertTo-Json $Body -Depth 10) `
			-Uri $Uri  `
			-Headers @{ Authorization = "Bearer $ApiToken"; Accept = $JsonType } `
			-ContentType $JsonType
}

function BC-Delete($Uri)
{
	return Invoke-RestMethod `
			-Method Delete `
			-Uri $Uri  `
			-Headers @{ Authorization = "Bearer $ApiToken"; Accept = $JsonType }
}

Export-ModuleMember -Function * -Variable 'URL'