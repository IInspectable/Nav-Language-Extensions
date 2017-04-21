@Echo Off

.\packages\NUnit.ConsoleRunner.3.6.1\tools\nunit3-console.exe ".\Nav.Language.Tests\bin\Debug\Nav.Language.Tests.dll" ".\Nav.Language.Extension.Tests\bin\Debug\Nav.Language.Extension.Tests.dll" --noresult --verbose

REM Pause