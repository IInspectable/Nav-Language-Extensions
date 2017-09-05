Add-Type -Path "C:\ws\git\Nav.Language.Extensions\Nav.Language\bin\Debug\Pharmatechnik.Nav.Language.dll"

"**Nav Language diagnostic errors**"
""
"|   |  Id    | Category | Severity |  Message |"
"|---|--------|----------|----------|----------| "

$sources= @(
    [Pharmatechnik.Nav.Language.DiagnosticDescriptors+Semantic].GetFields([Reflection.BindingFlags]::Public -bor [Reflection.BindingFlags]::Static)
    [Pharmatechnik.Nav.Language.DiagnosticDescriptors+DeadCode].GetFields([Reflection.BindingFlags]::Public -bor [Reflection.BindingFlags]::Static)
)

$nbr=1
$sources | % {
    $diag=$_.GetValue($null)
    $diag
} | where Id -ne $null | Sort Id  |   % {
    
    $diag=$_

    "|$($nbr)|<a name=`"$($diag.Id)`">$($diag.Id)</a> | $($diag.Category)| $($diag.DefaultSeverity)| $($diag.MessageFormat)|"
    $nbr++
} 