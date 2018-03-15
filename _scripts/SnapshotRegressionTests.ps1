
$dir = Join-Path $PSScriptRoot "..\Nav.Language.Tests\Regression\Tests"

ls $dir -recurse -Filter '*.expected.cs' | del -verbose

ls $dir -recurse -Filter '*.cs' | % {

    $file=$_
    $newName= [IO.Path]::ChangeExtension($file.FullName, 'expected.cs')

    Copy $file.FullName $newName -Pass | Select FullName

}

