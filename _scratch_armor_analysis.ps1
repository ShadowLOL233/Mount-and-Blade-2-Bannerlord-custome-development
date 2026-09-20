# Analyze RBM + OSA armor XML: how often each ItemType fills each armor slot,
# and what average values look like (only rows with values > 0 counted).

$files = @()
$files += @{ src='RBM.head';     path='E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\RBMCombat_head_armors.xml' }
$files += @{ src='RBM.body';     path='E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\RBMCombat_body_armors.xml' }
$files += @{ src='RBM.arm';      path='E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\RBMCombat_arm_armors.xml' }
$files += @{ src='RBM.leg';      path='E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\RBMCombat_leg_armors.xml' }
$files += @{ src='RBM.shoulder'; path='E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\RBMCombat_shoulder_armors.xml' }
$files += @{ src='OSA.body';     path='E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_ar_reforms_body.xml' }
$files += @{ src='OSA.head';     path='E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_ar_reforms_head.xml' }
$files += @{ src='OSA.shoulder'; path='E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_ar_reforms_shoulder.xml' }

$rows = foreach ($f in $files) {
    if (-not (Test-Path -LiteralPath $f.path)) { Write-Warning "missing: $($f.path)"; continue }
    $xml = [xml](Get-Content -LiteralPath $f.path -Raw)
    foreach ($it in $xml.Items.Item) {
        $arm = $it.ItemComponent.Armor
        if (-not $arm) { continue }
        $Type = $it.Type
        if (-not $Type) { continue }
        [pscustomobject]@{
            source     = $f.src
            id         = $it.id
            Type       = $Type
            head_armor = if ($arm.head_armor) { [int]$arm.head_armor } else { 0 }
            body_armor = if ($arm.body_armor) { [int]$arm.body_armor } else { 0 }
            arm_armor  = if ($arm.arm_armor)  { [int]$arm.arm_armor  } else { 0 }
            leg_armor  = if ($arm.leg_armor)  { [int]$arm.leg_armor  } else { 0 }
            mat        = $arm.material_type
            modgroup   = $arm.modifier_group
        }
    }
}

"=== Row counts by source ==="
$rows | Group-Object source | Format-Table Count, Name -AutoSize

"=== Cross-slot coverage: for each (source, Type), how many items have >0 in each of the 4 slots ==="
$rows | Group-Object source, Type | ForEach-Object {
    $g = $_.Group
    [pscustomobject]@{
        source_Type = $_.Name
        n           = $_.Count
        head_gt0    = ($g | Where-Object { $_.head_armor -gt 0 }).Count
        body_gt0    = ($g | Where-Object { $_.body_armor -gt 0 }).Count
        arm_gt0     = ($g | Where-Object { $_.arm_armor  -gt 0 }).Count
        leg_gt0     = ($g | Where-Object { $_.leg_armor  -gt 0 }).Count
    }
} | Sort-Object source_Type | Format-Table -AutoSize

"=== Averages of >0 values, per (source, Type, slot) ==="
foreach ($slot in @('head_armor','body_armor','arm_armor','leg_armor')) {
    "--- slot: $slot ---"
    $rows | Where-Object { $_.$slot -gt 0 } | Group-Object source, Type | ForEach-Object {
        $g = $_.Group
        [pscustomobject]@{
            source_Type = $_.Name
            n           = $_.Count
            avg         = [math]::Round( ($g | Measure-Object -Property $slot -Average).Average, 2 )
            min         = ($g | Measure-Object -Property $slot -Minimum).Minimum
            max         = ($g | Measure-Object -Property $slot -Maximum).Maximum
        }
    } | Sort-Object source_Type | Format-Table -AutoSize
}
