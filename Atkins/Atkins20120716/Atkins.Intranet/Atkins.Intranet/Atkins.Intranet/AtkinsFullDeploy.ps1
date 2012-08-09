
Write-Host "*******Cleaning Atkins PAckages from the farm *****************"
iex ".\CleanFarm.ps1"



Write-Host "*******Deploying Atkins PAckages on the farm *****************"
iex ".\Deploy.ps1"



Write-Host "*******Setting up Atkins Intranet Site*****************"
iex ".\AtkinsDeploy.ps1"


Write-Host "*******Atkins Full Deploy is Done!*****************"
