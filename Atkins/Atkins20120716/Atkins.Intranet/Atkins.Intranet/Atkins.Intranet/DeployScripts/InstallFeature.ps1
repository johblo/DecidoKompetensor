Uninstall-SPFeature Atkins.Intranet.HR_Atkins.Intranet.HR.Taxonomy
Start-Sleep -Seconds 30
Disable-SPFeature -Url http://ws2008r2efen64:6000/sites/intranet/HR
Start-Sleep -Seconds 30
Install-SPFeature Atkins.Intranet.HR_Atkins.Intranet.HR.Taxonomy
Start-Sleep -Seconds 30
Enable-SPFeature Atkins.Intranet.HR_Atkins.Intranet.HR.Taxonomy -Url http://ws2008r2efen64:6000/sites/intranet/HR