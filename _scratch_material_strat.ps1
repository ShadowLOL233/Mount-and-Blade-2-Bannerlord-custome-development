# Stratified analysis: for each (source, Type, slot), break down by material_type.
# Answers "how much variance does flat multiplier hide?"

$rbm = @('RBMCombat_head_armors.xml','RBMCombat_body_armors.xml','RBMCombat_arm_armors.xml',
         'RBMCombat_leg_armors.xml','RBMCombat_shoulder_armors.xml') |
       ForEach-Object { "E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\$_" }

$osa = Get-ChildItem "E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_*.xml" -File |
       Select-Object -ExpandProperty FullName

function ScanFiles($files, $srcTag) {
    foreach ($p in $files) {
        if (-not (Test-Path -LiteralPath $p)) { continue }
        try { $xml = [xml](Get-Content -LiteralPath $p -Raw) } catch { continue }
        if (-not $xml.Items) { continue }
        foreach ($it in $xml.Items.Item) {
            $arm = $it.ItemComponent.Armor
            if (-not $arm) { continue }
            [pscustomobject]@{
                source     = $srcTag
                Type       = $it.Type
                mat        = if ($arm.material_type) { $arm.material_type } else { '(none)' }
                head_armor = if ($arm.head_armor) { [int]$arm.head_armor } else { 0 }
                body_armor = if ($arm.body_armor) { [int]$arm.body_armor } else { 0 }
                arm_armor  = if ($arm.arm_armor)  { [int]$arm.arm_armor  } else { 0 }
                leg_armor  = if ($arm.leg_armor)  { [int]$arm.leg_armor  } else { 0 }
                weight     = if ($it.weight) { [double]$it.weight } else { 0.0 }
            }
        }
    }
}

$rows = @(ScanFiles $rbm 'RBM') + @(ScanFiles $osa 'OSA')

# For each (Type, primary-slot), compare RBM-avg vs OSA-avg per material.
$queries = @(
    @{ Type='HeadArmor'; slot='head_armor' },
    @{ Type='BodyArmor'; slot='body_armor' },
    @{ Type='BodyArmor'; slot='arm_armor' },
    @{ Type='BodyArmor'; slot='leg_armor' },
    @{ Type='Cape';      slot='body_armor' },
    @{ Type='Cape';      slot='arm_armor'  },
    @{ Type='HandArmor'; slot='arm_armor'  },
    @{ Type='LegArmor';  slot='leg_armor'  }
)

foreach ($q in $queries) {
    "=== $($q.Type) . $($q.slot) - by material ==="
    $subset = $rows | Where-Object { $_.Type -eq $q.Type -and $_.($q.slot) -gt 0 }
    $stats = @{}
    foreach ($src in @('RBM','OSA')) {
        foreach ($mat in ($subset | Where-Object source -eq $src | Select-Object -ExpandProperty mat -Unique)) {
            $items = @($subset | Where-Object { $_.source -eq $src -and $_.mat -eq $mat })
            $stats["$src|$mat"] = [pscustomobject]@{
                source = $src
                mat    = $mat
                n      = $items.Count
                avg    = [math]::Round( ($items | Measure-Object -Property $q.slot -Average).Average, 1 )
                min    = ($items | Measure-Object -Property $q.slot -Minimum).Minimum
                max    = ($items | Measure-Object -Property $q.slot -Maximum).Maximum
            }
        }
    }
    $stats.Values | Sort-Object mat, source | Format-Table -AutoSize

    # Buff factor per material
    "  buff × (RBM/OSA per mat):"
    $allMats = ($stats.Values | Select-Object -ExpandProperty mat -Unique) | Sort-Object
    foreach ($m in $allMats) {
        $r = $stats["RBM|$m"]
        $o = $stats["OSA|$m"]
        if ($r -and $o -and $o.avg -gt 0) {
            "    $m : OSA $($o.avg) (n=$($o.n)) -> RBM $($r.avg) (n=$($r.n))  = ×$([math]::Round($r.avg / $o.avg, 2))"
        } elseif ($r -and -not $o) {
            "    $m : RBM only ($($r.n) items, avg $($r.avg)) - OSA has 0 in this mat"
        } elseif ($o -and -not $r) {
            "    $m : OSA only ($($o.n) items, avg $($o.avg)) - RBM has 0 in this mat"
        }
    }
    ""
}
