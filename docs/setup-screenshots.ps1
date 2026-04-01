# Quick helper: Opens the screenshots folder in File Explorer
# so you can paste your screenshot files with the correct names.

$screenshotsDir = Join-Path $PSScriptRoot "screenshots"

if (-not (Test-Path $screenshotsDir)) {
    New-Item -ItemType Directory -Path $screenshotsDir -Force | Out-Null
}

Write-Host ""
Write-Host "=== VoxNote Screenshot Setup ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Opening folder: $screenshotsDir" -ForegroundColor Green
Write-Host ""
Write-Host "Please save your screenshots with these exact names:" -ForegroundColor Yellow
Write-Host ""
Write-Host "  01-login.png              - Login / Register page"
Write-Host "  02-record-idle.png        - Record page (idle, Start Recording button)"
Write-Host "  03-recording-active.png   - Recording in progress (timer + Stop)"
Write-Host "  04-mic-denied.png         - Microphone permission denied error"
Write-Host "  05-note-editor.png        - Note editor (title + textarea)"
Write-Host "  06-note-saved.png         - Note saved successfully"
Write-Host "  07-my-notes.png           - My Notes list (Edit/Delete)"
Write-Host "  08-admin-all-notes.png    - Admin Dashboard, All Notes tab"
Write-Host "  09-admin-users-modal.png  - Admin Dashboard, Users tab + modal"
Write-Host ""

Start-Process explorer.exe $screenshotsDir
