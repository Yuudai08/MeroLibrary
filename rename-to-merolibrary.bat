@echo off
setlocal
cd /d "%TEMP%"
cls
echo ===================================================================
echo               MeroLibrary - Safe Project Folder Rename
echo ===================================================================
echo.
echo This script safely renames "D:\Dryice" to "D:\MeroLibrary"
echo and creates a directory junction so nothing breaks.
echo.

if not exist "D:\Dryice" (
    if exist "D:\MeroLibrary" (
        echo [INFO] "D:\MeroLibrary" already exists! The rename is already complete.
    ) else (
        echo [ERROR] Neither "D:\Dryice" nor "D:\MeroLibrary" was found on D:\ drive.
    )
    goto finish
)

echo [STEP 1] Please close:
echo   1. Antigravity IDE
echo   2. Visual Studio (if open)
echo   3. Any terminal or File Explorer windows open inside D:\Dryice
echo.
echo Once those programs are closed, press any key to perform the rename...
pause >nul

echo.
echo [STEP 2] Renaming "D:\Dryice" to "D:\MeroLibrary"...
powershell -NoProfile -ExecutionPolicy Bypass -Command "Rename-Item -Path 'D:\Dryice' -NewName 'MeroLibrary' -ErrorAction Stop"

if %ERRORLEVEL% equ 0 (
    echo [SUCCESS] Successfully renamed folder to "D:\MeroLibrary"!
    echo.
    echo [STEP 3] Creating backward-compatibility junction "D:\Dryice" -^> "D:\MeroLibrary"...
    mklink /J "D:\Dryice" "D:\MeroLibrary" >nul 2>&1
    if %ERRORLEVEL% equ 0 (
        echo [SUCCESS] Junction created! Both paths will seamlessly work.
    ) else (
        echo [NOTE] Junction creation skipped or already exists.
    )
    echo.
    echo ===================================================================
    echo  All changes are complete!
    echo  You can now open "D:\MeroLibrary" in Antigravity IDE or Visual Studio.
    echo ===================================================================
) else (
    echo.
    echo [ERROR] The folder could not be renamed because it is still in use.
    echo Please make sure Antigravity IDE and Visual Studio are fully closed,
    echo then right-click this script and run it again.
)

:finish
echo.
echo Press any key to exit.
pause >nul
