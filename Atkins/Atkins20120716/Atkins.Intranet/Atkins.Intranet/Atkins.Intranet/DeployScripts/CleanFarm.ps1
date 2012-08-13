
function RetractSolution([string]$Identity)
{
    Write-Host "Uninstalling $Identity"    
    
    $solution = Get-SPSolution | where { $_.Name -match $Identity }
    if($solution.ContainsWebApplicationResource)
    {                  
        Uninstall-SPSolution -identity $Identity  -allwebapplications -Confirm:$false       
    }
    else
    {       
        Uninstall-SPSolution -identity $Identity -Confirm:$false          
    }

	Write-Host "Waiting for the job to finish"
	Start-Sleep -Seconds 30
	   

    Write-Host "Removing solution:" $Identity
    Remove-SPSolution -Identity $Identity -Confirm:$false

	Start-Sleep -Seconds 30

    Write-Host -f Green $Identity" removed successfully!"
}

 Write-Host -f Green "Cleaning Atkins packages from the farm:"

iisreset

RetractSolution("Atkins.Intranet.HR.wsp")
iisreset

RetractSolution("Atkins.Intranet.QSE.wsp")
iisreset

RetractSolution("Atkins.Intranet.Portal.wsp")
iisreset

RetractSolution("Atkins.Intranet.Utilities.wsp")
iisreset


RetractSolution("Atkins.Intranet.SampleData.wsp")
iisreset

RetractSolution("Atkins.Intranet.Blog.wsp")
iisreset





