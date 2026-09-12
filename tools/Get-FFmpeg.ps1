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

$repoRoot = Split-Path -Parent $PSScriptRoot
$targetDirectory = Join-Path $repoRoot '.ffmpeg'
$executable = Join-Path $targetDirectory 'ffmpeg.exe'

if ((Test-Path $executable) -and -not $Force) {
    Write-Host "ffmpeg already present at $executable (use -Force to re-download)."
    exit 0
}

$url = "https://github.com/GyanD/codexffmpeg/releases/download/$Version/ffmpeg-$Version-essentials_build.zip"
$archive = Join-Path ([System.IO.Path]::GetTempPath()) "ffmpeg-$Version-essentials_build.zip"

Write-Host "Downloading ffmpeg $Version..."
$previousProgress = $ProgressPreference
$ProgressPreference = 'SilentlyContinue'   # Progress rendering makes this ~10x slower on CI.
try {
    Invoke-WebRequest -Uri $url -OutFile $archive -UseBasicParsing -UserAgent 'Wget'
}
finally {
    $ProgressPreference = $previousProgress
}

$actualHash = (Get-FileHash -Path $archive -Algorithm SHA256).Hash
if ($actualHash -ne $Sha256.ToUpperInvariant()) {
    Remove-Item $archive -Force
    throw "SHA-256 mismatch for ffmpeg $Version.`n  expected: $($Sha256.ToUpperInvariant())`n  actual:   $actualHash"
}

Write-Host "Hash verified. Extracting ffmpeg.exe to $targetDirectory..."
New-Item -ItemType Directory -Path $targetDirectory -Force | Out-Null

# Only ffmpeg.exe is needed from the ~110 MB archive.
Add-Type -AssemblyName System.IO.Compression, System.IO.Compression.FileSystem
$zip = [System.IO.Compression.ZipFile]::OpenRead($archive)
try {
    $entry = $zip.Entries | Where-Object { $_.FullName -like '*/bin/ffmpeg.exe' } | Select-Object -First 1
    if (-not $entry) {
        throw "ffmpeg.exe was not found inside $archive."
    }
    [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $executable, $true)
}
finally {
    $zip.Dispose()
}
Remove-Item $archive -Force

if (-not (Test-Path $executable)) {
    throw "Extraction finished but $executable is missing."
}

Write-Host "ffmpeg $Version ready at $executable"
