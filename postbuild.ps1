# dotnet publish -c Release -o publish -r win-x64 --framework net48
dotnet publish -c Release -o publish -r win-x64 --framework net6.0-windows

$localpublish = "publish"
$out = "C:\Users\kast\AppData\Local\Microsoft\PowerToys\PowerToys Run\Plugins\publish"

Copy-Item -Recurse $localpublish\* $out -Force

$currlocation = Get-Location
Set-Location $out

Stop-Process -n PowerToys

# C:\Users\kast\.nuget\packages\ilmerge\3.0.41\tools\net452\ILMerge.exe /out:RiderWorkspaces.dll `
# FSharp.Core.dll `
# PowerToys.Run.Plugin.RiderWorkspaces.dll 

ILRepack.exe /out:RiderWorkspaces.dll `
FSharp.Core.dll `
PowerToys.Run.Plugin.RiderWorkspaces.dll 


Start-Process 'C:\Program Files\PowerToys\PowerToys.exe'

Set-Location $currlocation

