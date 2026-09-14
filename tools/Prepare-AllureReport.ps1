#Requires -Version 5.1
<#
.SYNOPSIS
    Gets the test results ready to become a report: the previous report's history for the trend,
    the run's name and link, and what it ran against. Reads the run's details from GitHub Actions.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string]$ResultsDirectory,
    [Parameter(Mandatory)] [string]$ReportUrl
)

$ErrorActionPreference = 'Stop'

# The trend needs the previous report's history. A first run simply has none.
$history = Join-Path $ResultsDirectory 'history'
New-Item -ItemType Directory -Force $history | Out-Null

foreach ($file in 'history.json', 'history-trend.json', 'duration-trend.json', 'categories-trend.json', 'retry-trend.json') {
    try {
        Invoke-WebRequest "$($ReportUrl)history/$file" -OutFile (Join-Path $history $file)
    } catch {
        Write-Host "No previous $file"
    }
}

# The run's name, number and link, shown on each point of the trend.
@{
    name       = 'GitHub Actions'
    type       = 'github'
    buildName  = "$env:GITHUB_WORKFLOW #$env:GITHUB_RUN_NUMBER"
    buildOrder = [int]$env:GITHUB_RUN_NUMBER
    buildUrl   = "$env:GITHUB_SERVER_URL/$env:GITHUB_REPOSITORY/actions/runs/$env:GITHUB_RUN_ID"
    reportUrl  = $ReportUrl
} | ConvertTo-Json | Set-Content (Join-Path $ResultsDirectory 'executor.json')

# What the run ran against, shown on the report's Overview.
$exe = Join-Path (Split-Path -Parent $PSScriptRoot) '.keepass/KeePass.exe'
$keepass = if (Test-Path $exe) { (Get-Item $exe).VersionInfo.ProductVersion } else { 'not fetched' }

@(
    "Application=KeePass $keepass"
    "Runner=$env:ImageOS $env:ImageVersion"
    "Locale=$((Get-Culture).Name)"
    "Commit=$env:GITHUB_SHA"
) | Set-Content (Join-Path $ResultsDirectory 'environment.properties')
