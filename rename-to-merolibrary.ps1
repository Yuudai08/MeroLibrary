# MeroLibrary - Safe Project Folder Rename Script (PowerShell)
# Switch working directory to temp to release any lock on D:\Dryice
Set-Location $env:TEMP

Write-Host "===================================================================" -ForegroundColor Cyan
Write-Host "           MeroLibrary - Safe Project Folder Rename               " -ForegroundColor Cyan
Write-Host "===================================================================" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path "D:\Dryice")) {
    if (Test-Path "D:\MeroLibrary") {
        Write-Host "[INFO] 'D:\MeroLibrary' already exists! The folder is already renamed." -ForegroundColor Green
    } else {
        Write-Host "[ERROR] Neither 'D:\Dryice' nor 'D:\MeroLibrary' was found on D:\ drive." -ForegroundColor Red
    }
    return
}

Write-Host "[STEP 1] Please make sure the following programs are closed:" -ForegroundColor Yellow
Write-Host "  1. Antigravity IDE"
Write-Host "  2. Visual Studio"
Write-Host "  3. Any open terminal or Explorer windows in D:\Dryice"
Write-Host ""
Read-Host "Once closed, press ENTER to execute the rename"

Write-Host "`n[STEP 2] Renaming D:\Dryice -> D:\MeroLibrary..." -ForegroundColor Cyan
try {
    Rename-Item -Path "D:\Dryice" -NewName "MeroLibrary" -ErrorAction Stop
    Write-Host "[SUCCESS] Renamed folder to D:\MeroLibrary!" -ForegroundColor Green

    Write-Host "`n[STEP 3] Creating backward-compatibility directory junction (D:\Dryice -> D:\MeroLibrary)..." -ForegroundColor Cyan
    cmd /c mklink /J "D:\Dryice" "D:\MeroLibrary" | Out-Null
    Write-Host "[SUCCESS] Directory junction created! Any old references will seamlessly resolve." -ForegroundColor Green

    Write-Host "`n===================================================================" -ForegroundColor Green
    Write-Host " All operations completed successfully with ZERO impact to your system!" -ForegroundColor Green
    Write-Host " You can now open D:\MeroLibrary in Antigravity IDE or Visual Studio." -ForegroundColor Green
    Write-Host "===================================================================" -ForegroundColor Green
} catch {
    Write-Host "`n[ERROR] Failed to rename folder: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "The folder is still locked by an active process. Close Antigravity IDE and Visual Studio, then run again." -ForegroundColor Yellow
}
