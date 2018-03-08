$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$Unit = 3

Import-Module "$Here\_Helpers" -DisableNameChecking

# Clear out existing -- this is not what we're looking for if we've got multiple units

foreach($Weapon in (BC-Get -Uri "$URL/weapons?unit=$Unit"))
{
	Write-Output "Removing Weapon with ID $($Weapon.Id)"

	BC-Delete -Uri "$URL/weapons?id=$($Weapon.Id)"
}


$Soldiers = (BC-Get "$URL/soldiers?unit=$Unit")

# Get weapons from CSV file

foreach($Weapon in (Import-Csv -Path $Here\WEAPONS_ROSTER.csv))
{
	# Create new weapon with info

	$Soldier = @{}

	if($Weapon.NAME) 
	{
		Write-Host "Finding SM"

		$Soldier = $Soldiers | Where-Object -Property lastName -Like -Value "$($Weapon.NAME.Split(' ')[0])*" | Select-Object -First 1
	}

	Write-Output "Create new MAL entry for $Weapon to $Soldier"

	BC-Post -Uri "$URL/Weapons" -Body @{ UnitId = $Unit; AdminNumber = $Weapon.NUM; Serial = $Weapon.SERIAL; OpticSerial = $Weapon.CCO; AssignedId = $Soldier.id; }
}

