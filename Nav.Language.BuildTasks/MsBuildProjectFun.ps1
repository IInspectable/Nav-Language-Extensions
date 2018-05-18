#Import-VisualStudioVars 2017

function Get-MSBuildProject {
    [cmdletbinding()]
    param(
        [Parameter(Position = 1)]
        $projectFile,

        [Parameter(ValueFromPipeline = $true)]
        $sourceObject
    )
    begin {
        $msbuildPath = Join-Path (split-path (Get-Command msbuild).Path) 'Microsoft.Build.dll'
        Add-Type -Path $msbuildPath | Out-Null
    }
    process {
        $project = $null
        if ($projectFile) 
        {
            $fullPath = Convert-Path $projectFile
            $projectCollection = (New-Object Microsoft.Build.Evaluation.ProjectCollection)
            $project = ([Microsoft.Build.Construction.ProjectRootElement]::Open([string]$fullPath, $projectCollection))
        } 
        elseif ($sourceObject -is [Microsoft.Build.Construction.ProjectRootElement]) 
        {
            $project = $sourceObject
        }
        else 
        {
            $project = $sourceObject.ContainingProject
        }

        return $project
    }
}

function Remove-EmptyItemGroups {
    [cmdletbinding()]
param(
    [Microsoft.Build.Construction.ProjectRootElement] $pre
)

    $itemGroupsToRemove = @($pre.ItemGroups | where { $_.Items.Count -eq 0})
    foreach ( $itemGroup in $itemGroupsToRemove) {
        $pre.RemoveChild($itemGroup)
    }

}

function Remove-EmptyPropertyGroups {
    [cmdletbinding()]
param(
    [Microsoft.Build.Construction.ProjectRootElement] $pre
)

   # Leere Property Groups entfernen
    $propertyGroupsToRemove = @($pre.PropertyGroups | where { $_.Properties.Count -eq 0})
    $propertyGroupsToRemove
    foreach ( $propertyGroup in $propertyGroupsToRemove) {
        $pre.RemoveChild($propertyGroup)
    }

}

$projectPath = "C:\ws\XTplus\z_Himi\XTplusApplication\src\XTplus.Verkauf\XTplus.Verkauf.csproj"


tf checkout $projectPath | Out-Null

$pre = Get-MSBuildProject $projectPath

foreach ($propertyGroup in $pre.PropertyGroups) {

    foreach ($property in $propertyGroup.Properties) {

        if ($property.Name -eq 'PreBuildEvent') {

            $valuesToAdd = @()
            $property.Value
            
            $pattern = ".*Framework\.Tools\.NavigationModeler\.exe.*"

            $values = @($property.Value | Split-String -separator '&& ^'  )
            foreach ($value in $values) {
                if ($value -notmatch $pattern) {
                    $valuesToAdd += $value
                }
            }
            $property.Value = $valuesToAdd -join '&& ^'
            $property.Value
            
        }
    }
}

# Bisherige nav Items entfernen
foreach ($itemGroup in $pre.ItemGroups) {

    $itemsToRemove = @()
    foreach ($item in $itemGroup.Items) {
    
        if ($item.Include.endsWith('.nav')) {
            $itemsToRemove += $item
        }        
    }
    
    foreach ($item in $itemsToRemove) {
        $itemGroup.RemoveChild($item)
    }
}

# Neue GenerateNavCode Items hinzufügen
$projectdir = split-path $projectPath
$ig = $pre.AddItemGroup()
ls $projectdir -filter '*.nav' -Recurse | % { 
    
    Push-Location $projectdir | Out-Null
    $navFile = Resolve-Path ($_.FullName) -Relative
    Pop-Location | Out-Null

    $ig.AddItem("GenerateNavCode", $navFile) | Out-Null
}

Remove-EmptyItemGroups $pre
Remove-EmptyPropertyGroups $pre


$pre.Save()
