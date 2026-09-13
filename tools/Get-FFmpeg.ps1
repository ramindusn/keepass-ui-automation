#Requires -Version 5.1
<#
.SYNOPSIS
    Downloads the pinned ffmpeg that encodes test videos into .ffmpeg/ (gitignored).
    The SHA-256 is gyan.dev's published hash for this build, which matches GitHub's release digest.
    Delete .ffmpeg/ to download it again.
#>

$ErrorActionPreference = 'Stop'

$version = '9.0.1'
$sha256 = 'fec81ae03971d9dd4be3ebe02e263bd2ec1d789483f931bdba5f5715e65da2e9'

$targetDirectory = Join-Path (Split-Path -Parent $PSScriptRoot) '.ffmpeg'
$executable = Join-Path $targetDirectory 'ffmpeg.exe'

if (-not (Test-Path $executable)) {
    # Only ffmpeg.exe is taken; nothing else in the ~110 MB archive is used.
    & (Join-Path $PSScriptRoot 'Install-PinnedZip.ps1') `
        -Url "https://github.com/GyanD/codexffmpeg/releases/download/$version/ffmpeg-$version-essentials_build.zip" `
        -Sha256 $sha256 `
        -TargetDirectory $targetDirectory `
        -EntryPattern '*/bin/ffmpeg.exe'
}

Write-Host "ffmpeg $version ready at $executable"
