#Requires -Version 5.1
<#
.SYNOPSIS
    Downloads the pinned ffmpeg that encodes test videos into .ffmpeg/ (gitignored).
    The SHA-256 is gyan.dev's published hash for this build, which matches GitHub's release digest.
#>
[CmdletBinding()]
param(
    [string]$Version = '9.0.1',
    [string]$Sha256 = 'fec81ae03971d9dd4be3ebe02e263bd2ec1d789483f931bdba5f5715e65da2e9',
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

$targetDirectory = Join-Path (Split-Path -Parent $PSScriptRoot) '.ffmpeg'
$executable = Join-Path $targetDirectory 'ffmpeg.exe'

if ($Force -or -not (Test-Path $executable)) {
    # Only ffmpeg.exe is taken; nothing else in the ~110 MB archive is used.
    & (Join-Path $PSScriptRoot 'Install-PinnedZip.ps1') `
        -Url "https://github.com/GyanD/codexffmpeg/releases/download/$Version/ffmpeg-$Version-essentials_build.zip" `
        -Sha256 $Sha256 `
        -TargetDirectory $targetDirectory `
        -EntryPattern '*/bin/ffmpeg.exe'
}

Write-Host "ffmpeg $Version ready at $executable"
