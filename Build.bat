@echo off

set config=%1
if "%config%" == "" (
   set config=Debug
)

"%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" -t:restore -m
"%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" Nav.Language.Extensions.sln -p:Configuration="%config%" -v:n -m

REM Pause