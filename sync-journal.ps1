<#
.SYNOPSIS
    Commit + push this repo's ModdingJournal.md and mirror it to the user-data live copy.

.DESCRIPTION
    Authoritative source: <this repo>\ModdingJournal.md (edit here)
    Mirror target:        E:\Bannerlord-UserData\ModdingJournal.md (read-only reference copy)

    Flow (reversed 2026-09-20; previous direction E:->repo would overwrite fresh commits
    with a stale live copy):
      1. git add + commit + push the repo's ModdingJournal.md (skip if unchanged)
      2. Copy repo -> E: so any tool looking at E: sees the same content

.PARAMETER Message
    Commit message. Defaults to "sync journal".

.PARAMETER SkipMirror
    Skip the copy to E:\Bannerlord-UserData (git-only sync).

.EXAMPLE
    .\sync-journal.ps1
    .\sync-journal.ps1 -Message "add RBM food economy notes"
    .\sync-journal.ps1 -SkipMirror
#>

[CmdletBinding()]
param(
    [string]$Message = "sync journal",
    [switch]$SkipMirror
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSCommandPath
$repoJournal = Join-Path $repoRoot "ModdingJournal.md"
$mirror = "E:\Bannerlord-UserData\ModdingJournal.md"

if (-not (Test-Path -LiteralPath $repoJournal)) {
    Write-Error "Repo journal not found: $repoJournal"
    exit 1
}

# --- git add + commit + push ---
Push-Location $repoRoot
try {
    git add "ModdingJournal.md" | Out-Null
    $status = git status --porcelain "ModdingJournal.md"
    if ($status) {
        Write-Host "Committing with message: $Message" -ForegroundColor Cyan
        git commit -m $Message
        Write-Host "Pushing to origin/main..." -ForegroundColor Cyan
        git push origin main
        Write-Host "Git sync done." -ForegroundColor Green
    } else {
        Write-Host "No repo changes to commit." -ForegroundColor Yellow
    }
} finally {
    Pop-Location
}

# --- Mirror repo -> E: ---
if ($SkipMirror) {
    Write-Host "Skipped mirror to E: (per -SkipMirror)." -ForegroundColor Yellow
    return
}

$mirrorDir = Split-Path -Parent $mirror
if (-not (Test-Path -LiteralPath $mirrorDir)) {
    Write-Warning "Mirror parent dir missing, skipping E: mirror: $mirrorDir"
    return
}

# Preserve the previous E: content once as a safety net, but only if the content
# actually differs (avoid churn).
if (Test-Path -LiteralPath $mirror) {
    $repoHash = (Get-FileHash -LiteralPath $repoJournal -Algorithm SHA1).Hash
    $mirrorHash = (Get-FileHash -LiteralPath $mirror -Algorithm SHA1).Hash
    if ($repoHash -eq $mirrorHash) {
        Write-Host "Mirror already in sync (hash match)." -ForegroundColor Yellow
        return
    }
    $stamp = Get-Date -Format 'yyyyMMdd'
    $bak = "$mirror.bak-pre-sync-$stamp"
    if (-not (Test-Path -LiteralPath $bak)) {
        Copy-Item -LiteralPath $mirror -Destination $bak -Force
        Write-Host "Backed up old mirror -> $bak" -ForegroundColor Cyan
    }
}

Copy-Item -LiteralPath $repoJournal -Destination $mirror -Force
Write-Host "Mirrored: $repoJournal -> $mirror" -ForegroundColor Green
