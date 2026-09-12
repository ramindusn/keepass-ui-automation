#Requires -Version 5.1
<#
.SYNOPSIS
    Downloads a zip, checks it against the SHA-256 it is pinned to, and unpacks it.
    Anything that does not match the hash is deleted before it can be used.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Url,
    [Parameter(Mandatory)][string]$Sha256,
    [Parameter(Mandatory)][string]$TargetDirectory,
    # Given, only the entry matching this pattern is taken, and it lands in the target
    # under its own name. Omitted, the whole archive is unpacked into the target.
    [string]$EntryPattern
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'   # Progress rendering makes the download ~10x slower on CI.

$expectedHash = $Sha256.ToUpperInvariant()
$archive = Join-Path ([System.IO.Path]::GetTempPath()) ([System.IO.Path]::GetRandomFileName() + '.zip')

Write-Host "Downloading $Url"

# SourceForge serves HTML, not the file, to browser-like user agents.
Invoke-WebRequest -Uri $Url -OutFile $archive -UseBasicParsing -UserAgent 'Wget'

$actualHash = (Get-FileHash -Path $archive -Algorithm SHA256).Hash

if ($actualHash -ne $expectedHash) {
    Remove-Item $archive -Force
    throw "SHA-256 mismatch, so the download was deleted.`n  url:      $Url`n  expected: $expectedHash`n  actual:   $actualHash"
}

Write-Host "Hash verified. Unpacking into $TargetDirectory"

try {
    if ($EntryPattern) {
        New-Item -ItemType Directory -Path $TargetDirectory -Force | Out-Null

        Add-Type -AssemblyName System.IO.Compression, System.IO.Compression.FileSystem
        $zip = [System.IO.Compression.ZipFile]::OpenRead($archive)

        try {
            $entry = $zip.Entries | Where-Object { $_.FullName -like $EntryPattern } | Select-Object -First 1

            if (-not $entry) {
                throw "No entry matching '$EntryPattern' inside $Url."
            }

            [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, (Join-Path $TargetDirectory $entry.Name), $true)
        }
        finally {
            $zip.Dispose()
        }
    }
    else {
        if (Test-Path $TargetDirectory) {
            Remove-Item $TargetDirectory -Recurse -Force
        }

        Expand-Archive -Path $archive -DestinationPath $TargetDirectory -Force
    }
}
finally {
    Remove-Item $archive -Force
}
