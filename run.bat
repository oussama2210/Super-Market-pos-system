@echo off
echo ========================================
echo   Supermarket POS System
echo ========================================
echo.
echo Building application...
dotnet build SupermarketPOS.sln --nologo --verbosity quiet
if %ERRORLEVEL% NEQ 0 (
    echo Build failed! Press any key to exit...
    pause > nul
    exit /b 1
)
echo Build successful!
echo.
echo Starting application...
echo.
start "" "SupermarketPOS.UI\bin\Debug\net8.0-windows\SupermarketPOS.UI.exe"
echo.
echo Application started!
echo Check your taskbar if you don't see the window.
echo.
timeout /t 2 > nul
