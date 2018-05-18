#Import-VisualStudioVars 2017
'Hi'
$projectPath="C:\ws\XTplus\z_Himi\XTplusApplication\src\XTplus.Verkauf\XTplus.Verkauf.csproj"

$msbuildPath = Join-Path (split-path (gcm msbuild).Path) 'Microsoft.Build.dll'

Add-Type -Path $msbuildPath | Out-Null


tf checkout $projectPath | Out-Null
$projectCollection = (New-Object Microsoft.Build.Evaluation.ProjectCollection)
$pre=[Microsoft.Build.Construction.ProjectRootElement]::Open($projectPath, $projectCollection)

foreach($propertyGroup in $pre.PropertyGroups){

    foreach($property in $propertyGroup.Properties){

        if($property.Name -eq 'PreBuildEvent'){

            $valuesToAdd=@()
            $property.Value
            
            $pattern= ".*Framework\.Tools\.NavigationModeler\.exe.*"

             $values=@($property.Value | Split-String -separator '&& ^'  )
             foreach($value in $values){
                if($value -notmatch $pattern){
                    $valuesToAdd +=$value
                }
             }
             $property.Value = $valuesToAdd -join '&& ^'
             $property.Value
            
        }
    }
}

# Bisherige nav Items entfernen
foreach($itemGroup in $pre.ItemGroups) {

    $itemsToRemove=@()
    foreach($item in $itemGroup.Items){
    
        if($item.Include.endsWith('.nav')){
            $itemsToRemove += $item
        }        
    }
    
    foreach($item in $itemsToRemove) {
        #$itemGroup.RemoveChild($item)
    }
}

# Neue GenerateNavCode Items hinzufügen
$ig=$pre.AddItemGroup()

$projectdir = split-path $projectPath

ls $projectdir -filter '*.nav' -Recurse | % { 
    
    Push-Location $projectdir | Out-Null
    $navFile=Resolve-Path ($_.FullName) -Relative
    Pop-Location | Out-Null

    $ig.AddItem("GenerateNavCode", $navFile) | Out-Null
}

# Leere Itemgroups entfernen (=> Fleißarbeit)
$itemGroupsToRemove=@($pre.ItemGroups | where { $_.Items.Count -eq 0})
foreach( $itemGroup in $itemGroupsToRemove) {
    $pre.RemoveChild($itemGroup)
}

# Leere Property Groups entfernen
 $propertyGroupsToRemove=@($pre.PropertyGroups | where { $_.Properties.Count -eq 0})
 $propertyGroupsToRemove
 foreach( $propertyGroup in $propertyGroupsToRemove) {
     $pre.RemoveChild($propertyGroup)
  }

$pre.Save()
