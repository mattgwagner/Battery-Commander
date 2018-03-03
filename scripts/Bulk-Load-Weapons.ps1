$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

Import-Module "$Here\_Helpers" -DisableNameChecking

foreach($Weapon in (BC-Get -Uri "$URL/weapons"))
{
	Write-Output "Removing Weapon with ID $($Weapon.Id)"

	Invoke-RestMethod -Method Delete -Uri "$URL/weapons/$($Weapon.Id)" -Headers $Headers
}

# Get weapons from CSV file

# Create new weapon with info