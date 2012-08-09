$qseUrl= "http://ws2008r2efen64:6000/sites/intranet/QSE"
$qseTitle = "QSE"

$qseTemplate= Get-SPWebTemplate "Atkins.Intranet.Portal#2"
iisreset

Write-Host "Creating Sub-Site: "$qseTitle
$qseSubSite = New-SPWeb –url $qseUrl -name $qseTitle -template $qseTemplate -Language 1053
Write-Host "Sub-site "$qseTitle" successfully created!"
Write-Host "************************************************************"

iisreset

Write-Host "Activating QSE features:"
Write-Host "***********************Create lists*********"
Enable-SPFeature -Identity "5fda185d-59ca-4101-955b-c9f28dd3acd7" -URL $qseUrl

iisreset

Write-Host "QSE Features Activation Done!"