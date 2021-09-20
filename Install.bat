@Echo Off
powershell.exe -noprofile -Command "& {. '%~dp0Nav.Language.Extension2019\bin\Debug\Pharmatechnik.Nav.Language.Extension.2019.vsix'}"
powershell.exe -noprofile -Command "& {. '%~dp0Nav.Language.Extension2022\bin\Debug\Pharmatechnik.Nav.Language.Extension.2022.vsix'}"
