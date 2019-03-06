@echo off

set config=%1
if "%config%" == "" (
   set config=Debug
)

"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Preview\\MSBuild\Current\Bin\MSBuild.exe" /t:restore
"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Preview\\MSBuild\Current\Bin\MSBuild.exe" Nav.Language.Extensions.sln /p:Configuration="%config%" /maxcpucount:4 /v:n

REM Pause