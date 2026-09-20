# Final (Type, slot, mat, tier) armor stratification with fallback.
# Uses DefaultItemValueModel.CalculateArmorTier for tier.
# Fallback ladder: (Type, mat, tier) -> (Type, mat) -> "skip if n<3 both".

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
    $tier = $clamped - 1
    if ($tier -lt 0) { return 1 }
    return $tier + 1
}

function ScanFiles($files, $srcTag) {
    foreach ($p in $files) {
        if (-not (Test-Path -LiteralPath $p)) { continue }
        try { $xml = [xml](Get-Content -LiteralPath $p -Raw) } catch { continue }
        if (-not $xml.Items) { continue }
        foreach ($it in $xml.Items.Item) {
            $arm = $it.ItemComponent.Armor
            if (-not $arm) { continue }
            $Type = $it.Type; if (-not $Type) { continue }
            $h = if ($arm.head_armor) { [int]$arm.head_armor } else { 0 }
            $b = if ($arm.body_armor) { [int]$arm.body_armor } else { 0 }
            $a = if ($arm.arm_armor)  { [int]$arm.arm_armor  } else { 0 }
            $l = if ($arm.leg_armor)  { [int]$arm.leg_armor  } else { 0 }
            $tier = Get-ArmorTier -Type $Type -head $h -body $b -arm $a -leg $l
            [pscustomobject]@{
                source=$srcTag; id=$it.id; Type=$Type
                mat = if ($arm.material_type) { $arm.material_type } else { '(none)' }
                tier=$tier; head_armor=$h; body_armor=$b; arm_armor=$a; leg_armor=$l
            }
        }
    }
}

$rows = @(ScanFiles $rbm 'RBM') + @(ScanFiles $osa 'OSA')

$queries = @(
    @{ Type='HeadArmor'; slot='head_armor' },
    @{ Type='HeadArmor'; slot='body_armor' },  # for helmet-extension (new field, RBM only)
    @{ Type='HeadArmor'; slot='arm_armor'  },  # for helmet-extension
    @{ Type='BodyArmor'; slot='body_armor' },
    @{ Type='BodyArmor'; slot='arm_armor'  },
    @{ Type='BodyArmor'; slot='leg_armor'  },
    @{ Type='Cape';      slot='body_armor' },
    @{ Type='Cape';      slot='arm_armor'  },
    @{ Type='HandArmor'; slot='arm_armor'  },
    @{ Type='LegArmor';  slot='leg_armor'  }
)

foreach ($q in $queries) {
    "=== $($q.Type) . $($q.slot) ==="
    # Only items with THIS slot > 0
    $subset = $rows | Where-Object { $_.Type -eq $q.Type -and $_.($q.slot) -gt 0 }

    $mats = ($subset | Select-Object -ExpandProperty mat -Unique) | Sort-Object
    foreach ($m in $mats) {
        $matSubset = $subset | Where-Object { $_.mat -eq $m }
        $tiers = ($matSubset | Select-Object -ExpandProperty tier -Unique) | Sort-Object
        foreach ($t in $tiers) {
            $rbmItems = @($matSubset | Where-Object { $_.source -eq 'RBM' -and $_.tier -eq $t })
            $osaItems = @($matSubset | Where-Object { $_.source -eq 'OSA' -and $_.tier -eq $t })
            $rbmAvg = if ($rbmItems.Count -gt 0) { [math]::Round( ($rbmItems | Measure-Object -Property $q.slot -Average).Average, 1 ) } else { $null }
            $osaAvg = if ($osaItems.Count -gt 0) { [math]::Round( ($osaItems | Measure-Object -Property $q.slot -Average).Average, 1 ) } else { $null }
            $buff = if ($rbmAvg -and $osaAvg -and $osaAvg -gt 0) { [math]::Round($rbmAvg / $osaAvg, 2) } else { $null }
            $flag = if ($rbmItems.Count -lt 3 -or $osaItems.Count -lt 3) { '(sparse)' } else { '' }
            "  $m T$t  RBM n=$($rbmItems.Count) avg=$rbmAvg  |  OSA n=$($osaItems.Count) avg=$osaAvg  =>  x$buff $flag"
        }
    }
    ""
}
