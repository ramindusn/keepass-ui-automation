#Requires -Version 5.1
<#
.SYNOPSIS
    Downloads the pinned KeePass into .keepass/ (gitignored), verified against the SHA-256
    published at https://keepass.info/integrity.html.
#>
[CmdletBinding()]
param(
    [string]$Version = '2.61.1',
    [string]$Sha256 = '3952354db9b117e906f7cd4f9f5591065b95186472370da47f46f3e246fea864',
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$targetDirectory = Join-Path $repoRoot '.keepass'
$executable = Join-Path $targetDirectory 'KeePass.exe'

# Runs every time, including when CI restores .keepass/ from its cache.
function Copy-EnforcedConfig {
    Copy-Item -Path (Join-Path $PSScriptRoot 'KeePass.config.enforced.xml') -Destination $targetDirectory -Force
    Write-Host "Applied enforced test configuration."
}

if ((Test-Path $executable) -and -not $Force) {
    Write-Host "KeePass already present at $executable (use -Force to re-download)."
    Copy-EnforcedConfig
    exit 0
}

$url = "https://sourceforge.net/projects/keepass/files/KeePass%202.x/$Version/KeePass-$Version.zip/download"
$archive = Join-Path ([System.IO.Path]::GetTempPath()) "KeePass-$Version.zip"

Write-Host "Downloading KeePass $Version..."
$previousProgress = $ProgressPreference
$ProgressPreference = 'SilentlyContinue'   # Progress rendering makes this ~10x slower on CI.
try {
    # SourceForge serves an HTML page, not the file, to browser-like user agents.
    Invoke-WebRequest -Uri $url -OutFile $archive -UseBasicParsing -UserAgent 'Wget'
}
finally {
    $ProgressPreference = $previousProgress
}

$actualHash = (Get-FileHash -Path $archive -Algorithm SHA256).Hash
if ($actualHash -ne $Sha256.ToUpperInvariant()) {
    Remove-Item $archive -Force
    throw "SHA-256 mismatch for KeePass $Version.`n  expected: $($Sha256.ToUpperInvariant())`n  actual:   $actualHash"
}

Write-Host "Hash verified. Extracting to $targetDirectory..."
if (Test-Path $targetDirectory) {
    Remove-Item $targetDirectory -Recurse -Force
}

Expand-Archive -Path $archive -DestinationPath $targetDirectory -Force
Remove-Item $archive -Force

if (-not (Test-Path $executable)) {
    throw "Extraction finished but $executable is missing."
}

Copy-EnforcedConfig
Write-Host "KeePass $Version ready at $executable"
