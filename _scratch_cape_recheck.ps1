# Re-check ALL RBM armor XML files for any Cape/shoulder-slot item that grants arm_armor.
# Then sample OSA Cape+arm items to see their id/mesh shape.

$rbm = @(
    'RBMCombat_head_armors.xml',
    'RBMCombat_body_armors.xml',
    'RBMCombat_arm_armors.xml',
    'RBMCombat_leg_armors.xml',
    'RBMCombat_shoulder_armors.xml'
) | ForEach-Object { "E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\$_" }

$osa = @(
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_ar_reforms_body.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_ar_reforms_head.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_ar_reforms_shoulder.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_dz_assets_armor.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_dz_assets_head.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_tv_assets_armor.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_tv_assets_head.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_hmj_moretroops_armor.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_hmj_moretroops_head.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_bl_newequipment_items.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_ap_armorpack_items.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_extra_contribution_items.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_aom_bip_armor.xml',
    'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_aom_bip_head.xml'
)

function Extract($files, $srcTag) {
    foreach ($p in $files) {
        if (-not (Test-Path -LiteralPath $p)) { continue }
        $xml = [xml](Get-Content -LiteralPath $p -Raw)
        foreach ($it in $xml.Items.Item) {
            $arm = $it.ItemComponent.Armor
            if (-not $arm) { continue }
            [pscustomobject]@{
                source     = $srcTag
                file       = Split-Path $p -Leaf
                id         = $it.id
                mesh       = $it.mesh
                Type       = $it.Type
                head_armor = if ($arm.head_armor) { [int]$arm.head_armor } else { 0 }
                body_armor = if ($arm.body_armor) { [int]$arm.body_armor } else { 0 }
                arm_armor  = if ($arm.arm_armor)  { [int]$arm.arm_armor  } else { 0 }
                leg_armor  = if ($arm.leg_armor)  { [int]$arm.leg_armor  } else { 0 }
                mat        = $arm.material_type
                modgroup   = $arm.modifier_group
            }
        }
    }
}

$rbmRows = @(Extract $rbm 'RBM')
$osaRows = @(Extract $osa 'OSA')

"=== RBM Cape items with arm_armor > 0 ==="
$rbmCapeArm = $rbmRows | Where-Object { $_.Type -eq 'Cape' -and $_.arm_armor -gt 0 }
"count: $($rbmCapeArm.Count)"
$rbmCapeArm | Select-Object id, mesh, body_armor, arm_armor, mat, modgroup | Format-Table -AutoSize

"=== RBM Cape items (any) breakdown of arm_armor field ==="
$rbmCapeAll = $rbmRows | Where-Object { $_.Type -eq 'Cape' }
"total Cape: $($rbmCapeAll.Count)"
"with arm>0: $($rbmCapeAll | Where-Object arm_armor -gt 0 | Measure-Object).Count"
$rbmCapeAll | Group-Object mat | Format-Table Count, Name -AutoSize

"=== OSA Cape items with arm_armor > 0 - sample by mat + first 15 items ==="
$osaCapeArm = $osaRows | Where-Object { $_.Type -eq 'Cape' -and $_.arm_armor -gt 0 }
"count: $($osaCapeArm.Count)"
"---by material---"
$osaCapeArm | Group-Object mat | Format-Table Count, Name -AutoSize
"---by modgroup---"
$osaCapeArm | Group-Object modgroup | Format-Table Count, Name -AutoSize
"---first 20 items (id/mesh)---"
$osaCapeArm | Select-Object -First 20 id, mesh, body_armor, arm_armor, mat, modgroup | Format-Table -AutoSize

"=== RBM shoulder file: what ItemTypes are in it? ==="
$rbmShoulderFile = $rbmRows | Where-Object { $_.file -eq 'RBMCombat_shoulder_armors.xml' }
$rbmShoulderFile | Group-Object Type | Format-Table Count, Name -AutoSize
"---any of these with arm_armor > 0---"
"$($rbmShoulderFile | Where-Object arm_armor -gt 0 | Measure-Object).Count"

"=== ALL RBM items where arm_armor > 0 grouped by ItemType (which slots RBM uses arm_armor from) ==="
$rbmRows | Where-Object { $_.arm_armor -gt 0 } | Group-Object Type | Format-Table Count, Name -AutoSize
