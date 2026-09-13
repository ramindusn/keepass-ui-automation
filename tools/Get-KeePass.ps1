#Requires -Version 5.1
<#
.SYNOPSIS
    Downloads the pinned KeePass into .keepass/ (gitignored), verified against the SHA-256
    published at https://keepass.info/integrity.html. Delete .keepass/ to download it again.
#>

$ErrorActionPreference = 'Stop'

$version = '2.61.1'
$sha256 = '3952354db9b117e906f7cd4f9f5591065b95186472370da47f46f3e246fea864'

$targetDirectory = Join-Path (Split-Path -Parent $PSScriptRoot) '.keepass'
$executable = Join-Path $targetDirectory 'KeePass.exe'

if (-not (Test-Path $executable)) {
    & (Join-Path $PSScriptRoot 'Install-PinnedZip.ps1') `
        -Url "https://sourceforge.net/projects/keepass/files/KeePass%202.x/$version/KeePass-$version.zip/download" `
        -Sha256 $sha256 `
        -TargetDirectory $targetDirectory

    if (-not (Test-Path $executable)) {
        throw "The archive unpacked but $executable is missing, so its layout has changed."
    }
}

# Runs every time, including when CI restores .keepass/ from its cache.
Copy-Item -Path (Join-Path $PSScriptRoot 'KeePass.config.enforced.xml') -Destination $targetDirectory -Force

Write-Host "KeePass $version ready at $executable, with the enforced test configuration."
