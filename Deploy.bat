@Echo off
call build.bat
REM call Run-Tests.bat
xcopy .\Nav.Language.BuildTasks\bin\Debug\*.* C:\ws\XTplus\z_Nav3\build\Script\Nav /I /Y