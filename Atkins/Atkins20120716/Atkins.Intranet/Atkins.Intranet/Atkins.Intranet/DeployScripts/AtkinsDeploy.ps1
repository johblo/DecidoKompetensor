$intranetUrl = "http://ws2008r2efen64:6000/sites/intranet"
$intranetTitle = "Home"
$administratorAccount = "TRETTON37\administrator"

$rootTemplate = Get-SPWebTemplate "Atkins.Intranet.Portal#0"
iisreset

Write-Host "Creating Site: "$intranetTitle
$homeSiteCollection = New-SPSite -Url $intranetUrl -OwnerAlias $administratorAccount -Name $intranetTitle -Template $rootTemplate -Language 1053 -ErrorAction Stop
Write-Host "Site "$intranetTitle" successfully created!"
Write-Host "************************************************************"

Write-Host "Activating Rootweb features:"
Write-Host "***********************Permission level and Document ID Feature*********"
Enable-SPFeature -Identity "8886fec7-0bb3-4869-bc43-ac77ddfc3989" -URL $intranetUrl


Write-Host "Activating Rootweb features:"
Write-Host "***********************RootWeb Sample Data*********"
Enable-SPFeature -Identity "6297993d-59fc-43dd-9bc8-9459785a4acc" -URL $intranetUrl


iisreset

$hrUrl= "http://ws2008r2efen64:6000/sites/intranet/HR"
$hrTitle = "HR"

$hrTemplate= Get-SPWebTemplate "Atkins.Intranet.Portal#1"
iisreset

Write-Host "Creating Sub-Site: "$hrTitle
$hrSubSite = New-SPWeb –url $hrUrl -name $hrTitle -template $hrTemplate -Language 1053
Write-Host "Sub-site "$hrTitle" successfully created!"
Write-Host "************************************************************"

iisreset

Write-Host "Activating HR features:"
Write-Host "***********************Employee-Task Event Receiver*********"
Enable-SPFeature -Identity "08636772-0489-40fe-82bf-651ef3ec281e" -URL $hrUrl


Write-Host "***********************HR Sample Data*********"
Enable-SPFeature -Identity "ba6fd70d-555e-448f-9271-c87f3d354c66" -URL $hrUrl

Write-Host "***********************HR Print List Item *********"
Enable-SPFeature -Identity "e1aea629-4aa3-479d-b9fe-f3454f5227e6" -URL $hrUrl

Write-Host "***********************HR Add webparts to HR StartPage *********"
Enable-SPFeature -Identity "187a36fd-14df-4e0a-b23c-2ee1018fa1c5" -URL $hrUrl



iisreset

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

Write-Host "Activating QSE features:"
Write-Host "***********************QSE Sample Data*********"
Enable-SPFeature -Identity "da11ccec-692a-4402-a86e-6e4a53f56742" -URL $qseUrl


Write-Host "Activating QSE features:"
Write-Host "***********************QSE Add webparts to QSE startpage*********"
Enable-SPFeature -Identity "5dfbab14-178b-48b7-8a9d-99bbe5ad0bbb" -URL $qseUrl

iisreset
Write-Host "QSE Features Activation Done!"

Write-Host "Activating rootWeb Feature"
Write-Host "***********************RootWeb Subsite dependent webparts*********"
Enable-SPFeature -Identity "41e3bfe4-65dc-4b61-8510-684fe180ed2c" -URL $intranetUrl