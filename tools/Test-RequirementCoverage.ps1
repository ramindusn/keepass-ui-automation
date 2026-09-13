#Requires -Version 5.1
<#
.SYNOPSIS
    Checks the test results against the open requirement issues. A test links to the requirement it
    verifies with [Requirement("<wording>", <issue number>)]; the issue's title is the requirement's wording. Fails
    when a requirement has no test, or a test links to something that is not an open requirement issue.
    Adds each requirement to its tests' results, so the report groups tests by requirement.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string]$Repository,
    [Parameter(Mandatory)] [string]$ResultsDirectory
)

$ErrorActionPreference = 'Stop'

$issues = gh issue list --repo $Repository --label requirement --state open --limit 500 --json number,title | ConvertFrom-Json

if ($LASTEXITCODE -ne 0) {
    throw "Could not list the requirement issues of $Repository."
}

$wording = @{}

foreach ($issue in $issues) {
    $wording[[int]$issue.number] = $issue.title
}

$testCounts = @{}
$unknown = @()

foreach ($file in Get-ChildItem $ResultsDirectory -Filter '*-result.json') {
    $result = Get-Content $file.FullName -Raw | ConvertFrom-Json
    $changed = $false

    foreach ($link in $result.links) {
        if ($link.type -ne 'tms') {
            continue
        }

        # The address ends in the issue number, whether or not the link template has been applied.
        if ($link.url -notmatch '(\d+)$' -or -not $wording.ContainsKey([int]$Matches[1])) {
            $unknown += "Link '$($link.name)' ($($link.url)) in $($result.fullName)"
            continue
        }

        $number = [int]$Matches[1]

        $testCounts[$number] = 1 + [int]$testCounts[$number]
        $result.labels += [pscustomobject]@{ name = 'feature'; value = "REQ-$number $($wording[$number])" }
        $result | Add-Member -NotePropertyName description -NotePropertyValue "REQ-${number}: $($wording[$number])" -Force
        $changed = $true
    }

    if ($changed) {
        $result | ConvertTo-Json -Depth 50 | Set-Content $file.FullName -Encoding utf8
    }
}

$uncovered = @()

foreach ($number in ($wording.Keys | Sort-Object)) {
    $count = [int]$testCounts[$number]

    if ($count -eq 0) {
        $uncovered += "REQ-$number"
        Write-Host "REQ-$number  NO TEST FOUND  $($wording[$number])"
    } else {
        Write-Host "REQ-$number  $count test(s)  $($wording[$number])"
    }
}

foreach ($tag in $unknown) {
    Write-Host "$tag  NOT AN OPEN REQUIREMENT ISSUE"
}

if ($uncovered.Count -gt 0) {
    Write-Host "::error title=Requirement without a test::$($uncovered -join ', ')"
}

if ($unknown.Count -gt 0) {
    Write-Host "::error title=Test names an unknown requirement::$($unknown -join '; ')"
}

if ($uncovered.Count -gt 0 -or $unknown.Count -gt 0) {
    exit 1
}

Write-Host "All $($wording.Count) requirements have at least one test."
