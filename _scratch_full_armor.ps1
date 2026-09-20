# Full-corpus armor scan for both RBM and OSA (all files).
# Emits per-ItemType averages of each slot (only >0 counted).

$rbm = @(
    'RBMCombat_head_armors.xml',
    'RBMCombat_body_armors.xml',
    'RBMCombat_arm_armors.xml',
    'RBMCombat_leg_armors.xml',
    'RBMCombat_shoulder_armors.xml'
) | ForEach-Object { "E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\$_" }

$osa = Get-ChildItem "E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_*.xml" -File |
       Where-Object { $_.Name -notlike 'OSA_*languages*' } |
       Select-Object -ExpandProperty FullName

function ScanFiles($files, $srcTag) {
    foreach ($p in $files) {
        if (-not (Test-Path -LiteralPath $p)) { continue }
        try { $xml = [xml](Get-Content -LiteralPath $p -Raw) } catch { Write-Warning "parse fail: $p"; continue }
        if (-not $xml.Items) { continue }
        foreach ($it in $xml.Items.Item) {
            $arm = $it.ItemComponent.Armor
            if (-not $arm) { continue }
            [pscustomobject]@{
                source     = $srcTag
                Type       = $it.Type
                head_armor = if ($arm.head_armor) { [int]$arm.head_armor } else { 0 }
                body_armor = if ($arm.body_armor) { [int]$arm.body_armor } else { 0 }
                arm_armor  = if ($arm.arm_armor)  { [int]$arm.arm_armor  } else { 0 }
                leg_armor  = if ($arm.leg_armor)  { [int]$arm.leg_armor  } else { 0 }
                mat        = $arm.material_type
            }
        }
    }
}

$rbmRows = @(ScanFiles $rbm 'RBM')
$osaRows = @(ScanFiles $osa 'OSA')

"=== Totals ==="
"RBM armor items: $($rbmRows.Count)"
"OSA armor items: $($osaRows.Count)"

"=== Per (source, Type) with each slot's fill-rate + avg (of >0 values) ==="
$rows = $rbmRows + $osaRows
foreach ($grp in ($rows | Group-Object source, Type | Sort-Object Name)) {
    $g = $grp.Group
    $n = $grp.Count
    $line = "{0,-20} n={1,4}" -f $grp.Name, $n
    foreach ($slot in @('head_armor','body_armor','arm_armor','leg_armor')) {
        $nz = @($g | Where-Object { $_.$slot -gt 0 })
        $rate = if ($n -gt 0) { [math]::Round($nz.Count / $n * 100, 0) } else { 0 }
        $avg  = if ($nz.Count -gt 0) { [math]::Round( ($nz | Measure-Object -Property $slot -Average).Average, 1 ) } else { 0 }
        $short = $slot.Substring(0,4)
        $line += "  {0}={1,3}% avg={2,5}" -f $short, $rate, $avg
    }
    $line
}
