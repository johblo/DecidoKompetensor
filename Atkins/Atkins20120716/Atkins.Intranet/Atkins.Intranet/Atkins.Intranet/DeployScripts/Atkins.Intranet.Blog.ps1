[int]$lcid = 1053
$culture = new-object "System.Globalization.CultureInfo" $lcid
[System.Threading.Thread]::CurrentThread.CurrentUICulture = $culture

$blogUrl = "http://ws2008r2efen64:6000/sites/intranet/Blog"
$blogTitle = "BLOG"
$administratorAccount = "TRETTON37\administrator"

$blogTemplate = Get-SPWebTemplate "BLOG#0"
iisreset

Write-Host "Creating Site: "$blogTitle
$blogSubSite = New-SPWeb -Url $blogUrl -name $blogTitle -template $blogTemplate -Language 1053

Write-Host "Site "$blogTitle" successfully created!"
Write-Host "************************************************************"


Write-Host "***********************Blog configuration feature*********"
Enable-SPFeature -Identity "4612ce28-dddf-4127-bb32-40d6e34de1c7" -URL $blogUrl
