param($VSIXPath)

if ($Env:APPVEYOR -ne 'True' -or $Env:APPVEYOR_PULL_REQUEST_NUMBER -ne $null)
{
	mkdir 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\PowerShell Tools for Visual Studio' -ErrorAction Continue
	Copy-Item (Join-Path $PSScriptRoot '.\Project\PowerShellTools.targets') 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\PowerShell Tools for Visual Studio' -ErrorAction Continue -Force
	Copy-Item (Join-Path $PSScriptRoot '.\..\PowerShellTools.Msbuild\bin\debug\PowerShellTools.Msbuild.dll') 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\PowerShell Tools for Visual Studio' -ErrorAction Continue -Force

    return
}

$ToolPath = (Join-Path $PSScriptRoot '..\packages\Microsoft.VSSDK.Vsixsigntool.15.0.26201\tools\vssdk\vsixsigntool.exe')

$Bin = Split-Path $VSIXPath -Parent
$PDBDir = Join-Path $Bin 'PDB'
mkdir $PDBDir
Copy-Item (Join-Path $Bin '*.pdb') $PDBDir

Set-Location $PSScriptRoot
Start-Process $ToolPath -ArgumentList "sign /f DigiCertJan2017.pfx /sha1 f2bef33f6dd732bbae4dd927a5bb010a93a49bd5  /p $Env:signing_code /v /t http://timestamp.digicert.com $VSIXPath" -NoNewWindow 