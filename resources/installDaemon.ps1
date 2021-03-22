$binPath = (Get-Item .).FullName + "\microstack.Daemon.exe"
$serviceName = "Microstack Background Service"
sc.exe create $serviceName binPath=$binPath start=auto
sc.exe start $serviceName
Write-Host -NoNewLine 'Press any key to continue...';
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');