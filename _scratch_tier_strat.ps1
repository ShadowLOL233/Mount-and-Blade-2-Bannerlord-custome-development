# Tier-computed armor stratification: RBM vs OSA by (Type, mat, tier)
# using DefaultItemValueModel.CalculateArmorTier formula (verified via ilspycmd 2026-09-20).

$rbm = @('RBMCombat_head_armors.xml','RBMCombat_body_armors.xml','RBMCombat_arm_armors.xml',
         'RBMCombat_leg_armors.xml','RBMCombat_shoulder_armors.xml') |
       ForEach-Object { "E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\$_" }

$osa = Get-ChildItem "E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_*.xml" -File |
       Select-Object -ExpandProperty FullName

function Get-ArmorTier {
    param([string]$Type, [int]$head, [int]$body, [int]$arm, [int]$leg)
    $raw = 1.2 * $head + 1.0 * $body + 1.0 * $leg + 1.0 * $arm
    switch ($Type) {
        'LegArmor'  { $raw *= 1.6 }
        'HandArmor' { $raw *= 1.7 }
        'HeadArmor' { $raw *= 1.2 }
        'Cape'      { $raw *= 1.8 }
    }
    $tierf = $raw * 0.1 - 0.4
    $rounded = [int]([Math]::Round($tierf))
    $clamped = [Math]::Max(0, [Math]::Min(6, $rounded))
    $tier = $clamped - 1  # 0-indexed Tier enum: Tier1=0 .. Tier6=5, -1 = below Tier1
    if ($tier -lt 0) { return 1 }  # treat sub-Tier1 as Tier1 minimum
    return $tier + 1  # 1-indexed for display
}

function ScanFiles($files, $srcTag) {
    foreach ($p in $files) {
        if (-not (Test-Path -LiteralPath $p)) { continue }
        try { $xml = [xml](Get-Content -LiteralPath $p -Raw) } catch { continue }
        if (-not $xml.Items) { continue }
        foreach ($it in $xml.Items.Item) {
            $arm = $it.ItemComponent.Armor
            if (-not $arm) { continue }
            $Type = $it.Type
            if (-not $Type) { continue }
            $h = if ($arm.head_armor) { [int]$arm.head_armor } else { 0 }
            $b = if ($arm.body_armor) { [int]$arm.body_armor } else { 0 }
            $a = if ($arm.arm_armor)  { [int]$arm.arm_armor  } else { 0 }
            $l = if ($arm.leg_armor)  { [int]$arm.leg_armor  } else { 0 }
            $tier = Get-ArmorTier -Type $Type -head $h -body $b -arm $a -leg $l
            [pscustomobject]@{
                source     = $srcTag
                id         = $it.id
                Type       = $Type
                mat        = if ($arm.material_type) { $arm.material_type } else { '(none)' }
                tier       = $tier
                head_armor = $h
                body_armor = $b
                arm_armor  = $a
                leg_armor  = $l
                weight     = if ($it.weight) { [double]$it.weight } else { 0.0 }
            }
        }
    }
}

$rows = @(ScanFiles $rbm 'RBM') + @(ScanFiles $osa 'OSA')

"=== Tier distribution by source, Type ==="
$rows | Group-Object source, Type, tier | ForEach-Object {
    [pscustomobject]@{
        source_Type_Tier = $_.Name
        n = $_.Count
    }
} | Sort-Object source_Type_Tier | Format-Table -AutoSize

"=== Task 24: OSA Cloth LegArmor sample ==="
$rows | Where-Object { $_.source -eq 'OSA' -and $_.Type -eq 'LegArmor' -and $_.mat -eq 'Cloth' } |
    Select-Object id, tier, leg_armor, weight |
    Format-Table -AutoSize
