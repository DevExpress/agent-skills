@echo off
rem Builds ExportHarness.exe (Win32) against a project folder that contains the form/data-module units.
rem Usage: build-harness.cmd <project folder>
rem Run it from a copy of this folder outside the project. Set STUDIO/RS for other RAD Studio versions
rem (23.0 = RAD Studio 12 = RS29). DXVCL is set by the DevExpress VCL installer.
if "%~1"=="" (echo Usage: build-harness.cmd ^<project folder^> & exit /b 1)
set PROJECTDIR=%~f1
if "%STUDIO%"=="" set STUDIO=23.0
if "%RS%"=="" set RS=RS29
if "%DXVCL%"=="" set DXVCL=%ProgramFiles(x86)%\DevExpress\VCL
call "%ProgramFiles(x86)%\Embarcadero\Studio\%STUDIO%\bin\rsvars.bat"
cd /d "%~dp0"
set DX=%DXVCL%\Library\%RS%
if not exist dcu mkdir dcu
dcc32 -B -Q -NSSystem;Xml;Data;Datasnap;Web;Soap;Vcl;Vcl.Imaging;Winapi;System.Win;Data.Win;FireDAC -U"%DX%;%PROJECTDIR%" -I"%DX%" -R"%DX%;%PROJECTDIR%" -NU.\dcu -E. ExportHarness.dpr || exit /b 1
copy /Y "%BDS%\Redist\win32\WebView2Loader.dll" . >nul
