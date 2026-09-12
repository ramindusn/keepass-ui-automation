#Requires -Version 5.1
<#
.SYNOPSIS
    Fails when a requirement in requirements.json has no test in the Allure results.
    Each test names its requirement as an Allure feature, so coverage is read from the results.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string]$RequirementsPath,
    [Parameter(Mandatory)] [string]$ResultsDirectory
)

$ErrorActionPreference = 'Stop'

$requirements = Get-Content $RequirementsPath -Raw | ConvertFrom-Json

$features = Get-ChildItem $ResultsDirectory -Filter '*-result.json' |
    ForEach-Object { (Get-Content $_.FullName -Raw | ConvertFrom-Json).labels } |
    Where-Object { $_.name -eq 'feature' } |
    ForEach-Object { $_.value }

$uncovered = @()

foreach ($requirement in $requirements) {
    $prefix = $requirement.id + ' '
    $tests = @($features | Where-Object { $_.StartsWith($prefix) }).Count

    if ($tests -eq 0) {
        $uncovered += $requirement.id
        Write-Host "$($requirement.id)  NO TEST FOUND"
    } else {
        Write-Host "$($requirement.id)  $tests test(s)"
    }
}

if ($uncovered.Count -gt 0) {
    Write-Host "::error title=Requirement without a test::$($uncovered -join ', ')"
    exit 1
}

Write-Host "All $($requirements.Count) requirements have at least one test."
