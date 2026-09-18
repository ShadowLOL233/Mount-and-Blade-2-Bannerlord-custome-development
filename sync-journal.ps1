<#
.SYNOPSIS
    Sync the live ModdingJournal.md from Bannerlord user data to this repo and push to GitHub.

.DESCRIPTION
    Source: E:\Bannerlord-UserData\ModdingJournal.md (live edit copy)
    Dest:   <this repo>\ModdingJournal.md
    Then: git add + commit + push

.PARAMETER Message
    Commit message. Defaults to "sync journal".

.EXAMPLE
    .\sync-journal.ps1
    .\sync-journal.ps1 -Message "add RBM food economy notes"
#>

[CmdletBinding()]
param(
    [string]$Message = "sync journal"
)

$ErrorActionPreference = "Stop"

$source = "E:\Bannerlord-UserData\ModdingJournal.md"
$repoRoot = Split-Path -Parent $PSCommandPath
$dest = Join-Path $repoRoot "ModdingJournal.md"

if (-not (Test-Path -LiteralPath $source)) {
    Write-Error "Source not found: $source"
    exit 1
}

Copy-Item -LiteralPath $source -Destination $dest -Force
Write-Host "Copied: $source -> $dest" -ForegroundColor Cyan

Push-Location $repoRoot
try {
    git add "ModdingJournal.md" | Out-Null
    $status = git status --porcelain "ModdingJournal.md"
    if (-not $status) {
        Write-Host "No changes to commit (journal already up-to-date)." -ForegroundColor Yellow
        return
    }
    Write-Host "Committing with message: $Message" -ForegroundColor Cyan
    git commit -m $Message
    Write-Host "Pushing to origin/main..." -ForegroundColor Cyan
    git push origin main
    Write-Host "Done." -ForegroundColor Green
} finally {
    Pop-Location
}
