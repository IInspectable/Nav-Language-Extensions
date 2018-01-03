@Echo off
call build.bat
REM call Run-Tests.bat
xcopy .\Nav.Language.BuildTasks\bin\Debug\*.* C:\ws\XTplus\z_MSBuildV15\build\nav /I /Y