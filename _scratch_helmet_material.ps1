$osa = Get-ChildItem "E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData\OSA_*.xml" -File |
       Select-Object -ExpandProperty FullName
$hits = foreach ($p in $osa) {
    try { $xml = [xml](Get-Content -LiteralPath $p -Raw) } catch { continue }
    if (-not $xml.Items) { continue }
    foreach ($it in $xml.Items.Item) {
        if ($it.Type -ne 'HeadArmor') { continue }
        $arm = $it.ItemComponent.Armor
        if (-not $arm) { continue }
        [pscustomobject]@{ id=$it.id; mat=$arm.material_type }
    }
}
"OSA HeadArmor by material_type:"
$hits | Group-Object mat | Sort-Object Count -Descending | Format-Table Count, Name -AutoSize
