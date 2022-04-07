dotnet publish -c Release -o publish -r win-x64 --framework net6.0-windows

$localpublish = "publish"
$out = "$env:LOCALAPPDATA\Microsoft\PowerToys\PowerToys Run\Plugins\publish"

Copy-Item -Recurse $localpublish\* $out -Force

$currlocation = Get-Location
Set-Location $out

Stop-Process -n PowerToys

ILRepack.exe /out:RiderWorkspaces.dll `
FSharp.Core.dll `
PowerToys.Run.Plugin.RiderWorkspaces.dll 


Start-Process 'C:\Program Files\PowerToys\PowerToys.exe'

Set-Location $currlocation

