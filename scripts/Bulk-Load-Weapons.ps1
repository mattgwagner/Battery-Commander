$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

Import-Module "$Here\_Helpers" -DisableNameChecking

# Clear out existing -- this is not what we're looking for if we've got multiple units

foreach($Weapon in (BC-Get -Uri "$URL/weapons"))
{
	Write-Output "Removing Weapon with ID $($Weapon.Id)"

	BC-Delete -Uri "$URL/weapons?id=$($Weapon.Id)"
}

# Get weapons from CSV file

foreach($Weapon in (Import-Csv -Path $Here\WEAPONS_ROSTER.csv))
{
	# Create new weapon with info

	Write-Output "Create new MAL entry for $Weapon"

	#BC-Post -Uri "$URL/Weapons" -Body @{ UnitId = 3; AdminNumber = $Weapon.NUM; Serial = $Weapon.SERIAL; OpticSerial = $Weapon.CCO }
}

