@echo off

set config=%1
if "%config%" == "" (
   set config=Debug
)

nuget restore
"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe" Nav.Language.Extensions.sln /p:Configuration="%config%" /maxcpucount:4 /v:n

REM Pause