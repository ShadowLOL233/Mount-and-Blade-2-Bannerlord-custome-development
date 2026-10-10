# Open Source Armoury RBM Balance Patch - manual override generator.
#
# Reads BALANCE_V2_LOG.md decisions (encoded here as a decisions hashtable) and
# generates OSABalance_manual_override.xml which overrides both:
#   1. auto-generated OSABalance_armor_override.xml (v1 factor-based)
#   2. vanilla RBM items (for Empire override + Vlandia L修订)
#   3. NavalDLC vanilla items (for Nord 65-piece batch override)
#
# XML load order (SubModule.xml):
#   OSABalance_armor_override.xml  (auto-generated, loaded first)
#   OSABalance_manual_override.xml (this, loaded after → overrides)
#
# Bannerlord XML load convention: same-id Item = whole-node replacement, so we
# must fetch the ORIGINAL item definition from its source XML and copy ALL
# attributes, only overriding the Armor sub-element's head/body/leg/arm values
# (and optionally weight).

param(
    [string]$OsaArmorRoot     = 'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData',
    [string]$OsaSaddleryRoot  = 'E:\SteamLibrary\steamapps\workshop\content\261550\3010990914\ModuleData',
    [string]$RbmArmorRoot     = 'E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData',
    [string]$NavalDlcRoot     = 'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\NavalDLC\ModuleData',
    [string]$SandboxCoreRoot  = 'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\SandBoxCore\ModuleData'
)

$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$outFile = Resolve-Path (Join-Path $here '..\ModuleData\OSABalance_manual_override.xml') -ErrorAction SilentlyContinue
if (-not $outFile) {
    $outFile = Join-Path (Resolve-Path (Join-Path $here '..\ModuleData')) 'OSABalance_manual_override.xml'
}

Write-Host "== OSABalance manual override generator ==" -ForegroundColor Cyan
Write-Host "output: $outFile"

# ---------- decisions hashtable ----------
# Format: id = @{ h = 0; b = 0; l = 0; a = 0; wt = $null (optional) }
# Missing fields default to 0. wt = $null means keep original weight.
# Ordering by section matches BALANCE_V2_LOG.md.

$decisions = [ordered]@{}

# ==================== Empire vanilla+RBM override (2026-09-24) ====================
# BALANCE_V2_LOG.md line ~91: "Vanilla+RBM Override 首次打破"
# Rationale: user judged lamellar_with_scale_skirt 118/122/45 vs imperial_scale_armor
# 135/122/67 are NOT balanced平替 — after swap, they become "高身甲 vs 高臂甲" pair
# with near-identical total defense (302 vs 307).

$decisions['lamellar_with_scale_skirt'] = @{ h=0; b=135; l=122; a=45 }   # was 118/122/45
$decisions['imperial_scale_armor']      = @{ h=0; b=118; l=122; a=67 }   # was 135/122/67

# Empire H family follow-up override (2026-09-24)
# Since vanilla lamellar_with_scale_skirt is now 135/122/45, the two OSA H
# family pieces that direct-match it need to update too.
$decisions['ao_imperial_cataphracts_lamellar']    = @{ h=0; b=135; l=122; a=45; wt=28 }
$decisions['ao_imperial_cataphracts_lamellar_b']  = @{ h=0; b=132; l=122; a=45; wt=28 }  # Brass -3

# ==================== Empire HeadArmor · Pilot batch A1 (2026-09-28) ====================
# Deploy 断层收尾第一批 · 15 件 · fixes user in-game observation of Kettle naming reversal
# (khuzait_ironlame_kettle "Imperial Closed Iron Kettle" T3 currently weaker than
# roundkettle_over_imperial_leather "Light Roundkettle over Leather" T4).
# Sourced from BALANCE_V2_LOG.md Empire HeadArmor section (line 184-1332).
# Remaining ~150 Empire HeadArmor items to be appended in follow-up batches.

# --- Provocator 4 (BALANCE_V2_LOG.md line 186-201 · v2a + v7 wt回改) ---
$decisions['AR_gladiator_helmet_e'] = @{ h=88; b=45; a=32; l=0; wt=4 }    # Gilded Provocator
$decisions['AR_gladiator_helmet_f'] = @{ h=90; b=40; a=35; l=0; wt=4 }    # Decorated Provocator
$decisions['AR_gladiator_helmet_g'] = @{ h=96; b=50; a=32; l=0; wt=5 }    # Gilded Provocator w/ Faceplate
$decisions['AR_gladiator_helmet_h'] = @{ h=98; b=45; a=35; l=0; wt=5 }    # Decorated Provocator w/ Faceplate

# --- Kettle 11 (BALANCE_V2_LOG.md line 1021-1066 · 头>身>臂铁律首用) ---
# Roundkettle 4
$decisions['ao_imperial_kettlehelm_with_leather']    = @{ h=74; b=20; a=12; l=0; wt=1.5 }
$decisions['ao_imperial_kettlehelm_with_mail']       = @{ h=92; b=28; a=22; l=0; wt=3.3 }
$decisions['ao_imperial_kettlehelm_over_mail_coif']  = @{ h=90; b=42; a=30; l=0; wt=3.7 }
$decisions['DZ_empire_helmet_a']                     = @{ h=92; b=28; a=22; l=0; wt=3.3 }   # Imperial Mailled Kettle Helmet
# Pointed/Spiked Kettle 7
$decisions['DZ_empire_helmet_e']                     = @{ h=60; b=18; a=12; l=0; wt=1.5 }   # Pointed Kettle Over Padding
$decisions['DZ_empire_helmet_f']                     = @{ h=68; b=20; a=15; l=0; wt=1.8 }   # Pointed Kettle Over Leather
$decisions['DZ_empire_helmet_g']                     = @{ h=90; b=28; a=22; l=0; wt=3.6 }   # Pointed Kettle Over Mail
$decisions['DZ_empire_helmet_h']                     = @{ h=90; b=28; a=22; l=0; wt=3.6 }   # Feathered Pointed Kettle Over Mail
$decisions['AR_closed_kettle_helmet_a']              = @{ h=97; b=32; a=25; l=0; wt=3.8 }   # Plumed Kettle Over Closed Mail
$decisions['khuzait_ironlame_kettle']                = @{ h=78; b=30; a=22; l=0; wt=3.8 }   # ⭐ Imperial Closed Iron Kettle (fixes 命名倒挂)
$decisions['khuzait_spiked_kettle']                  = @{ h=95; b=30; a=25; l=0; wt=3.9 }   # Imperial Closed Spiked Kettle

# ==================== Empire HeadArmor · Batch A2 (2026-09-28) ====================
# Cataphract v3 12 + Roman Helmet 12 (含头>身>臂追溯修正) = 24 件

# --- Cataphract v3 12 (BALANCE_V2_LOG.md line 736-747 · Goggled 反推阶梯) ---
$decisions['DZ_empire_helmet_b']                            = @{ h=123; b=62; a=36; l=0; wt=3.5 }   # Cataphract's Domed
$decisions['DZ_empire_helmet_c']                            = @{ h=123; b=62; a=36; l=0; wt=3.5 }   # Plumed Cataphract's Domed
$decisions['DZ_empire_helmet_i']                            = @{ h=110; b=56; a=32; l=0; wt=3.5 }   # Cataphract's Feathered Pointed Kettle
$decisions['DZ_empire_helmet_j']                            = @{ h=110; b=56; a=32; l=0; wt=3.5 }   # Cataphract's Plumed Pointed Kettle
$decisions['TV_empire_lord_helmet_j']                       = @{ h=128; b=65; a=38; l=0; wt=4.5 }   # Cataphract's Closed Fluted
$decisions['ao_imperial_cataphracts_closed_mail_helmet']    = @{ h=128; b=65; a=38; l=0; wt=4.5 }   # Cataphract's Closed Mail
$decisions['ao_imperial_cataphracts_flamboyant_helmet']     = @{ h=123; b=62; a=36; l=0; wt=3.5 }   # Ornate Cataphract
$decisions['ao_imperial_guarded_conical_nasal_helmet']      = @{ h=133; b=70; a=40; l=0; wt=3.7 }   # Guarded Cataphract's Conical Nasal
$decisions['ar_empire_cataphracts_helmet_a']                = @{ h=123; b=62; a=36; l=0; wt=3.5 }   # Plumed Cataphract
$decisions['varangian_guard_helmet_c']                      = @{ h=115; b=58; a=33; l=0; wt=3.5 }   # Stripped Cataphract Mail
$decisions['DZ_empire_helmet_d']                            = @{ h=123; b=62; a=36; l=0; wt=3.5 }   # Cataphract's Bent Conical
$decisions['ao_imperial_cataphracts_plumed_closed_mail_helmet'] = @{ h=133; b=70; a=40; l=0; wt=3.7 } # Plumed Spangen Over Closed Mail

# --- Roman Helmet 12 (BALANCE_V2_LOG.md line 832-844 + 863-866 追溯修正) ---
$decisions['AR_roman_helmet_a']         = @{ h=94;  b=12; a=0;  l=0; wt=3.4 }   # Open Helmet With Faceguard (vanilla helmet_with_faceguard 直匹配)
$decisions['AR_roman_helmet_a_fur_a']   = @{ h=80;  b=10; a=0;  l=0; wt=3.4 }   # Scout's Ridged Cap
$decisions['AR_roman_helmet_a_fur_b']   = @{ h=82;  b=10; a=0;  l=0; wt=3.4 }   # Scout's Banded Cap
$decisions['AR_roman_helmet_b_fur_a']   = @{ h=94;  b=12; a=0;  l=0; wt=3.7 }   # Scout's Helmet With Faceguard
$decisions['AR_roman_helmet_b_fur_b']   = @{ h=90;  b=12; a=0;  l=0; wt=3.7 }   # Scout's Tall Helmet
$decisions['AR_roman_helmet_b_fur_c']   = @{ h=104; b=24; a=0;  l=0; wt=3.7 }   # Scout's Plumed Helmet (plumed_helmet 直匹配)
$decisions['AR_roman_helmet_b_plumed']  = @{ h=104; b=24; a=0;  l=0; wt=3.7 }   # Plumed Helmet With Faceguard
$decisions['AR_roman_helmet_b_plumed_b']= @{ h=104; b=24; a=0;  l=0; wt=3.7 }   # Plumed Feathered Helmet With Faceguard
$decisions['AR_roman_helmet_d_plumed_a']= @{ h=104; b=24; a=0;  l=0; wt=3.9 }   # Decorated Plumed Helmet
$decisions['AR_roman_helmet_d_plumed_b']= @{ h=106; b=24; a=0;  l=0; wt=3.9 }   # Gilded Plumed Helmet
$decisions['AR_roman_helmet_d_strips']  = @{ h=84;  b=20; a=10; l=0; wt=3.5 }   # Plumed Tall Helmet Over Stripped Cloth (追溯修正)
$decisions['roman_helmet_z']            = @{ h=84;  b=20; a=10; l=0; wt=3.5 }   # Tall Helmet Over Stripped Cloth (追溯修正)

# --- Lord/Guarded Lord 12 (BALANCE_V2_LOG.md line 892-901 + 924-925 补漏) ---
$decisions['AR_Empire_Lord_Guarded_Face_Helmet'] = @{ h=140; b=70; a=40; l=0; wt=4.5 }  # Imperial Guarded Lord Helmet with Faceplate
$decisions['AR_empire_lord_helmet_d']            = @{ h=132; b=62; a=36; l=0; wt=3.9 }  # Gilded Imperial Guarded Lord Helmet
$decisions['AR_empire_lord_helmet_e']            = @{ h=125; b=60; a=36; l=0; wt=3.8 }  # Imperial Feathered Jeweled Helmet
$decisions['AR_empire_lord_helmet_g']            = @{ h=125; b=62; a=38; l=0; wt=3.5 }  # Imperial Silvered Ridge Helmet
$decisions['AR_empire_lord_helmet_h']            = @{ h=126; b=60; a=35; l=0; wt=2.5 }  # Imperial Jeweled Gilded Ridge Helmet
$decisions['AR_empire_lord_helmet_i']            = @{ h=122; b=60; a=35; l=0; wt=2.7 }  # Imperial Plumed Decorated Banded Metal Strips
$decisions['AR_empire_lord_helmet_j']            = @{ h=122; b=60; a=35; l=0; wt=2.5 }  # Imperial Jeweled Ridge Helmet
$decisions['TV_empire_lord_helmet_a']            = @{ h=125; b=62; a=38; l=0; wt=3.8 }  # Imperial Fluted Helmet With Metal Strips
$decisions['TV_empire_lord_helmet_d']            = @{ h=125; b=62; a=38; l=0; wt=3.5 }  # Imperial Decorated Silvered Ridge Helmet
$decisions['TV_empire_lord_helmet_e']            = @{ h=126; b=62; a=38; l=0; wt=3.5 }  # Imperial Decorated Gilded Ridge Helmet
$decisions['AR_empire_lord_helmet_a']            = @{ h=127; b=60; a=35; l=0; wt=3.5 }  # Imperial Lord Helmet With Faceguard (补漏)
$decisions['AR_empire_lord_helmet_b']            = @{ h=128; b=62; a=38; l=0; wt=3.7 }  # Imperial Lord Helmet With Faceplate (补漏)

# --- Nasalhelm 3 (BALANCE_V2_LOG.md line 959-961 + 980 追溯修正) ---
$decisions['ao_imperial_nasal_helmet']                    = @{ h=97;  b=28; a=22; l=0; wt=2.2 }  # Imperial Nasal Helmet (追溯修正)
$decisions['ao_imperial_guarded_nasal_helmet']            = @{ h=105; b=40; a=30; l=0; wt=2.6 }  # Imperial Guarded Nasal Helmet
$decisions['ao_imperial_guarded_decorated_nasal_helmet']  = @{ h=107; b=40; a=30; l=0; wt=2.6 }  # Imperial Guarded Decorated Nasal Helmet

# --- Elite 4 (BALANCE_V2_LOG.md line 1003-1006) ---
$decisions['ao_imperial_evocati_helmet']  = @{ h=122; b=55; a=37; l=0; wt=3.0 }   # Imperial Evocatus' Helmet
$decisions['varangian_guard_helmet_a']    = @{ h=138; b=70; a=42; l=0; wt=5.0 }   # Imperial Guard's Helmet With Closed Plated Mail
$decisions['echerian_elite_helm']         = @{ h=140; b=70; a=40; l=0; wt=4.5 }   # Imperial Closed Guarded Lord Helmet (echerian)
$decisions['imperial_lord_helm_mail']     = @{ h=140; b=70; a=40; l=0; wt=4.5 }   # Imperial Closed Guarded Lord Helmet (mail · 双同名)

# ==================== Empire HeadArmor · Batch A3 (2026-09-28) ====================
# Crowned/Palatine 5 + 特殊/独立 4 + Ridge/Intercisa/Lion 6 + Legatus 3 + Conical/Pointed 8 + TV/AR 混合 14 = 40 件

# --- Crowned/Palatine 5 (BALANCE_V2_LOG.md line 1088-1092) ---
$decisions['ao_imperial_palatine_guard_helmet']  = @{ h=128; b=60; a=38; l=0; wt=3.5 }   # Palatine Over Mail
$decisions['AO_empire_helmet_za']                = @{ h=125; b=42; a=25; l=0; wt=2.7 }   # Palatine Over Leather
$decisions['AO_empire_helmet_zb']                = @{ h=120; b=28; a=15; l=0; wt=2.3 }   # Palatine Over Cloth
$decisions['AR_Imperial_Crowned_Helmet_a']       = @{ h=120; b=45; a=25; l=0; wt=3.5 }   # Imperial Crowned Helmet
$decisions['AR_Imperial_Crowned_Helmet_b']       = @{ h=128; b=55; a=32; l=0; wt=3.9 }   # Imperial Crowned Helmet With Faceplate

# --- 特殊/独立顶档 4 (BALANCE_V2_LOG.md line 1125-1128) ---
$decisions['visored_helmet_x']                   = @{ h=95;  b=40; a=25; l=0; wt=4.0 }   # Visored Cap Helmet
$decisions['empire_battle_crown_west']           = @{ h=109; b=12; a=0;  l=0; wt=3.1 }   # Imperial Plumed Helmet (vanilla 本体直接回归)
$decisions['solar_helmet']                       = @{ h=128; b=50; a=30; l=0; wt=3.5 }   # Solar Helmet With Gilded Faceplate
$decisions['solar_helmet_b']                     = @{ h=127; b=50; a=30; l=0; wt=3.5 }   # Solar Helmet With Silvered Faceplate

# --- Ridge/Intercisa/Lion 6 (BALANCE_V2_LOG.md line 1161-1166) ---
$decisions['AR_intercisa_helmet_a']              = @{ h=90;  b=35; a=20; l=0; wt=3.5 }
$decisions['AR_intercisa_helmet_b']              = @{ h=82;  b=25; a=12; l=0; wt=3.2 }   # Open variant
$decisions['AR_intercisa_helmet_c']              = @{ h=90;  b=35; a=20; l=0; wt=3.5 }
$decisions['AR_intercisa_helmet_d']              = @{ h=90;  b=35; a=20; l=0; wt=3.5 }
$decisions['AR_intercisa_helmet_e']              = @{ h=92;  b=35; a=20; l=0; wt=3.5 }   # + Gilded
$decisions['AR_lion_head_c']                     = @{ h=115; b=40; a=25; l=0; wt=4.5 }   # Ridge + Lion Head

# --- Legatus 3 (BALANCE_V2_LOG.md line 1199-1201) ---
$decisions['AR_empire_legatus_helm_c']           = @{ h=110; b=40; a=22; l=0; wt=3.5 }
$decisions['AR_empire_legatus_helm_d']           = @{ h=110; b=40; a=22; l=0; wt=3.5 }
$decisions['AR_empire_legatus_helm_e']           = @{ h=110; b=40; a=22; l=0; wt=3.5 }

# --- Conical/Pointed Helmet 8 (BALANCE_V2_LOG.md line 1241-1253) ---
$decisions['ao_imperial_pointed_helmet_with_cloth_wrap']       = @{ h=80; b=15; a=8;  l=0; wt=1.5 }
$decisions['ao_imperial_pointed_helmet_with_leather']          = @{ h=85; b=25; a=15; l=0; wt=2.2 }
$decisions['ao_imperial_pointed_helmet_with_lamellar_strips']  = @{ h=90; b=32; a=18; l=0; wt=2.5 }
$decisions['ao_imperial_pointed_helmet_with_mail_coif']        = @{ h=93; b=42; a=28; l=0; wt=3.5 }
$decisions['ao_imperial_cone_helmet_with_leather']             = @{ h=90; b=25; a=15; l=0; wt=2.5 }
$decisions['ao_imperial_cone_helmet_with_lamellar_strips']     = @{ h=95; b=32; a=18; l=0; wt=2.8 }
$decisions['ao_imperial_noblemans_cone_helmet']                = @{ h=98; b=42; a=28; l=0; wt=3.5 }
$decisions['ao_imperial_plumed_cone_helmet_with_closed_mail']  = @{ h=98; b=45; a=30; l=0; wt=3.7 }

# --- TV/AR 混合尾单 14 (BALANCE_V2_LOG.md line 1288-1316 · Empire HeadArmor 收官批) ---
$decisions['AR_empire_helmet_g']         = @{ h=122; b=50; a=25; l=0; wt=3.8 }   # Gilded Metal Strips
$decisions['AR_empire_helmet_h']         = @{ h=127; b=55; a=30; l=0; wt=4.2 }   # Gilded Closed Metal Strips
$decisions['AR_empire_helmet_n']         = @{ h=108; b=40; a=20; l=0; wt=2.7 }   # Plumed Banded Metal Strips
$decisions['AR_empire_desert_helmet_e']  = @{ h=118; b=45; a=22; l=0; wt=3.5 }   # Southern Metal Strips
$decisions['AR_empire_helmet_l']         = @{ h=95;  b=32; a=18; l=0; wt=2.2 }   # Banded
$decisions['AR_empire_helmet_m']         = @{ h=98;  b=35; a=20; l=0; wt=2.5 }   # Banded + Leather Strips
$decisions['TV_empire_helmet_c']         = @{ h=105; b=42; a=28; l=0; wt=3.5 }   # Decorated Banded Over Mail Coif
$decisions['TV_empire_helmet_d']         = @{ h=108; b=45; a=28; l=0; wt=3.5 }   # Decorated Banded Faceguard Over Mail
$decisions['AR_empire_helmet_o3']        = @{ h=55;  b=30; a=18; l=0; wt=2.0 }   # Steel Scale Coif
$decisions['AR_empire_helmet_p']         = @{ h=25;  b=18; a=10; l=0; wt=0.6 }   # Padded Cloth Coif
$decisions['TV_empire_helmet_a']         = @{ h=108; b=42; a=22; l=0; wt=3.5 }   # Feathered Decorated Steel Cap Over Stripped Mail
$decisions['TV_empire_helmet_b']         = @{ h=95;  b=32; a=22; l=0; wt=3.5 }   # Spiked Roundkettle Over Mail
$decisions['TV_empire_helmet_e']         = @{ h=95;  b=45; a=28; l=0; wt=3.7 }   # Roundkettle Over Closed Mail Coif
$decisions['TV_empire_helmet_f']         = @{ h=98;  b=45; a=28; l=0; wt=3.7 }   # Spiked Kettle Over Closed Mail Coif

# ==================== Empire HeadArmor · Batch A4 (2026-09-28) ====================
# Spangenhelm 27 (v2b + v2c 字典 + v3 #28 + v7 wt + v8 Heavy 6件重审 · #24/#29 已在 Cataphract batch)
# + Crested 8 (v3/v4 + v7 arm 上调 for #1/#2/#3) + Secutor 6 (v7 arm 上调)
# + Sagittarius 8 + 杂项 3 = 52 件
# 帽/头饰 19 件符合铁律11 (cosmetic 低价值) 保持 🔵 log-only 不 deploy
# AR_empire_lord_helmet_c 不在 override XML (v1 script 跳过) 待用户确认

# --- Spangenhelm 27 (BALANCE_V2_LOG.md line 242-544 · 多轮 revision 最终态) ---
$decisions['ao_imperial_spangenhelm_with_leather']                   = @{ h=76;  b=12; a=15; l=0; wt=1.7 }
$decisions['ao_imperial_heavy_spangenhelm_with_leather']             = @{ h=80;  b=15; a=18; l=0; wt=3.7 }   # v8
$decisions['ao_imperial_nasal_spangenhelm_with_leather_strips']      = @{ h=79;  b=12; a=15; l=0; wt=1.8 }
$decisions['AR_empire_desert_helmet_c']                              = @{ h=65;  b=12; a=25; l=0; wt=3.2 }   # Southern Cap
$decisions['ao_imperial_spangenhelm_with_mail']                      = @{ h=95;  b=12; a=25; l=0; wt=3.3 }
$decisions['ao_imperial_heavy_spangenhelm_with_mail']                = @{ h=99;  b=15; a=28; l=0; wt=5.3 }   # v8
$decisions['ao_imperial_nasal_spangenhelmet_with_mail']              = @{ h=98;  b=12; a=25; l=0; wt=3.4 }
$decisions['ao_imperial_spangenhelm_over_padded_coif']               = @{ h=70;  b=15; a=25; l=0; wt=1.8 }
$decisions['TV_empire_helmet_g']                                     = @{ h=72;  b=15; a=25; l=0; wt=1.8 }   # v2c Gilded +2
$decisions['AR_empire_helmet_q']                                     = @{ h=65;  b=12; a=6;  l=0; wt=1.6 }
$decisions['AR_empire_helmet_r']                                     = @{ h=65;  b=12; a=6;  l=0; wt=1.6 }
$decisions['AR_empire_helmet_s']                                     = @{ h=65;  b=12; a=6;  l=0; wt=1.6 }
$decisions['ao_imperial_crowned_nasal_helmet']                       = @{ h=67;  b=12; a=6;  l=0; wt=1.7 }   # v2c Bronze -1
$decisions['AR_empire_desert_helmet_b']                              = @{ h=55;  b=12; a=6;  l=0; wt=3.2 }
$decisions['AR_empire_desert_helmet_d']                              = @{ h=61;  b=12; a=17; l=0; wt=1.9 }
$decisions['AR_empire_desert_helmet_a']                              = @{ h=90;  b=12; a=25; l=0; wt=3.9 }
$decisions['TV_empire_helmet_h']                                     = @{ h=97;  b=12; a=25; l=0; wt=3.3 }   # v2c Gilded +2
$decisions['TV_empire_lord_helmet_h']                                = @{ h=99;  b=13; a=26; l=0; wt=3.3 }   # v2c Gilded+Lord
$decisions['aserai_feathered_spangenhelm']                           = @{ h=95;  b=12; a=25; l=0; wt=3.3 }
$decisions['bronze_aserai_helm']                                     = @{ h=91;  b=12; a=25; l=0; wt=3.9 }   # v2c Iron +1
$decisions['AR_empire_lord_helmet_f']                                = @{ h=122; b=15; a=21; l=0; wt=3.8 }   # v2c Lord +2/+1/+1
$decisions['AR_empire_helmet_a']                                     = @{ h=128; b=36; a=20; l=0; wt=3.6 }   # v8 vanilla direct + Noble+Jeweled
$decisions['AR_empire_helmet_b']                                     = @{ h=129; b=36; a=20; l=0; wt=3.6 }   # v8 + Noble+Gilded+Jeweled
$decisions['ao_imperial_heavy_spangenhelm_over_mail_coif']           = @{ h=92;  b=25; a=43; l=0; wt=5.1 }   # v8
$decisions['ao_imperial_heavy_nasal_spangenhelm_over_mail_coif']     = @{ h=95;  b=25; a=43; l=0; wt=5.1 }   # v8
$decisions['ao_imperial_redcrest_bronze_spangenhelm_over_mail_coif'] = @{ h=87;  b=22; a=40; l=0; wt=3.1 }   # v2c Bronze -1
$decisions['ao_imperial_closed_mail_nasal_spangenhelmet']            = @{ h=103; b=17; a=28; l=0; wt=4.3 }   # v3 dict + v7 wt

# --- Crested 8 (BALANCE_V2_LOG.md line 403-410 v3 + v4 Ridge 提升 + v7 arm 上调 for 3 Faceguard) ---
$decisions['AR_roman_helmet_d_plumed_c'] = @{ h=96;  b=12; a=15; l=0; wt=3.5 }   # Gilded Crested + Faceplate (v7 arm)
$decisions['AR_empire_crested_helm_a']   = @{ h=94;  b=12; a=15; l=0; wt=3.5 }   # Crested Faceguard (v7 arm)
$decisions['TV_empire_crested_helm_a']   = @{ h=96;  b=12; a=15; l=0; wt=3.5 }   # Gilded Crested Faceguard (v7 arm)
$decisions['AR_empire_legatus_helm_a']   = @{ h=122; b=15; a=21; l=0; wt=3.8 }   # Decorated Crested Lord (v3 · Legatus family)
$decisions['AR_empire_legatus_helm_b']   = @{ h=124; b=15; a=21; l=0; wt=3.8 }   # Gilded Crested Lord (v3)
$decisions['TV_empire_lord_helmet_b']    = @{ h=109; b=15; a=2;  l=0; wt=3.1 }   # Silvered Ridge (v4 Ridge + v7 wt)
$decisions['TV_empire_lord_helmet_c']    = @{ h=110; b=15; a=2;  l=0; wt=3.1 }   # Gilded Ridge (v4 + v7)
$decisions['TV_empire_lord_helmet_f']    = @{ h=109; b=15; a=2;  l=0; wt=3.1 }   # Silvered Ridge Feather (v4 + v7)

# --- Secutor 6 (BALANCE_V2_LOG.md line 700-705 · v7 arm 上调) ---
$decisions['AR_gladiator_helmet_c']  = @{ h=94;  b=12; a=12; l=0; wt=3.5 }
$decisions['AR_gladiator_helmet_c2'] = @{ h=94;  b=12; a=12; l=0; wt=3.5 }
$decisions['AR_gladiator_helmet_a']  = @{ h=96;  b=12; a=12; l=0; wt=3.5 }
$decisions['AR_gladiator_helmet_a2'] = @{ h=96;  b=12; a=12; l=0; wt=3.5 }
$decisions['AR_gladiator_helmet_d']  = @{ h=99;  b=17; a=15; l=0; wt=4.5 }   # Visored Secutor
$decisions['AR_gladiator_helmet_b']  = @{ h=101; b=17; a=15; l=0; wt=4.5 }   # Gilded Visored Secutor

# --- Sagittarius 8 (BALANCE_V2_LOG.md line 608-615) ---
$decisions['AR_empire_archer_helmet_b']  = @{ h=62; b=12; a=6;  l=0; wt=1.2 }
$decisions['AR_empire_archer_helmet_b2'] = @{ h=64; b=12; a=6;  l=0; wt=1.2 }
$decisions['AR_empire_archer_helmet_a']  = @{ h=70; b=12; a=15; l=0; wt=1.7 }
$decisions['AR_empire_archer_helmet_a2'] = @{ h=72; b=12; a=15; l=0; wt=1.7 }
$decisions['AR_empire_archer_helmet_c']  = @{ h=82; b=15; a=25; l=0; wt=2.8 }
$decisions['AR_empire_archer_helmet_c2'] = @{ h=84; b=15; a=25; l=0; wt=2.8 }
$decisions['AR_empire_archer_helmet_d']  = @{ h=87; b=18; a=28; l=0; wt=3.0 }
$decisions['AR_empire_archer_helmet_d2'] = @{ h=89; b=18; a=28; l=0; wt=3.0 }

# --- 杂项 3 (BALANCE_V2_LOG.md line 560 + 追溯 line 581-582) ---
$decisions['AR_empire_helmet_o']    = @{ h=48; b=28; a=15; l=0; wt=1.8 }   # Imperial Brass Scale Coif (追溯)
$decisions['AR_empire_helmet_o2']   = @{ h=48; b=28; a=15; l=0; wt=1.8 }   # Imperial Alternating Scale Coif (追溯)
$decisions['AR_roman_helmet_cap']   = @{ h=40; b=0;  a=0;  l=0; wt=1.2 }   # Imperial Cap Helmet

# ==================== Vlandia L family 修订 (2026-09-24) ====================
# BALANCE_V2_LOG.md line ~3700: L family "Lamellar Over Heavy Mail Hauberk"
# anchor was wrong (plated_leather_coat 75), corrected to sturgian_fortified_armor 100
# "扎甲+mail 视为顶级防御" rule

$decisions['AR_vlandia_lamellar_a']  = @{ h=0; b=100; l=95;  a=100; wt=22 }  # Steel Lamellar (顶)
$decisions['AR_vlandia_lamellar_a2'] = @{ h=0; b=97;  l=92;  a=97;  wt=22 }  # Brass -3
$decisions['AR_vlandia_lamellar_b']  = @{ h=0; b=90;  l=85;  a=85;  wt=18 }  # Leather -10

# ==================== Vlandia HeadArmor · Batch B (2026-09-28) ====================
# Deploy 断层收尾第二批 · 128 件 · BALANCE_V2_LOG.md line 2850-3202
# 全 15 家族 · 全 🔵 log-only · v1 全 130 件 body/arm=0 · v2 补齐 aventail/faceguard 结构

# --- A. Circlets/Crowns 6 (line 2858-2863) ---
$decisions['green_hat']              = @{ h=8;  b=0; a=0; l=0; wt=0.2 }
$decisions['ao_gold_circlet_a']      = @{ h=12; b=0; a=0; l=0; wt=0.5 }
$decisions['ao_crown_a']             = @{ h=18; b=0; a=0; l=0; wt=0.5 }
$decisions['crown_x']                = @{ h=18; b=0; a=0; l=0; wt=0.5 }
$decisions['crown_z']                = @{ h=20; b=0; a=0; l=0; wt=0.7 }
$decisions['ao_angevin_crown']       = @{ h=22; b=0; a=0; l=0; wt=0.7 }

# --- B. Heavy Skullcap 7 (line 2877-2883) ---
$decisions['AR_vlandia_helmet_g']    = @{ h=50; b=0;  a=0;  l=0; wt=1.2 }
$decisions['AR_vlandia_helmet_h']    = @{ h=55; b=45; a=0;  l=0; wt=1.5 }
$decisions['AR_vlandia_helmet_f']    = @{ h=53; b=12; a=20; l=0; wt=1.5 }
$decisions['AR_vlandia_helmet_c']    = @{ h=56; b=12; a=8;  l=0; wt=1.5 }
$decisions['AR_vlandia_helmet_c2']   = @{ h=56; b=12; a=8;  l=0; wt=1.5 }
$decisions['AR_vlandia_helmet_d']    = @{ h=86; b=12; a=40; l=0; wt=3.5 }
$decisions['AR_vlandia_helmet_e']    = @{ h=88; b=15; a=40; l=0; wt=3.5 }

# --- C. Western Spangenhelm 12 (line 2899-2920) ---
$decisions['AR_vlandia_helmet_l']    = @{ h=55; b=0;  a=0;  l=0; wt=1.4 }
$decisions['AR_vlandia_helmet_k']    = @{ h=73; b=12; a=20; l=0; wt=1.5 }
$decisions['AR_vlandia_helmet_o']    = @{ h=75; b=0;  a=12; l=0; wt=1.4 }
$decisions['AR_vlandia_helmet_p']    = @{ h=75; b=12; a=20; l=0; wt=1.6 }
$decisions['AR_vlandia_helmet_q']    = @{ h=90; b=20; a=40; l=0; wt=3.6 }
$decisions['AR_vlandia_helmet_q2']   = @{ h=90; b=20; a=40; l=0; wt=3.6 }
$decisions['AR_vlandia_helmet_r']    = @{ h=95; b=25; a=40; l=0; wt=3.9 }
$decisions['AR_vlandia_helmet_r2']   = @{ h=95; b=25; a=40; l=0; wt=3.9 }
$decisions['AR_vlandia_helmet_m']    = @{ h=90; b=60; a=30; l=0; wt=3.8 }
$decisions['AR_vlandia_helmet_n']    = @{ h=92; b=25; a=40; l=0; wt=3.8 }
$decisions['ao_battanian_guarded_aristocrats_spangenhelmet']       = @{ h=80; b=50; a=12; l=0; wt=2.0 }
$decisions['ao_battanian_guarded_aristocrats_spangenhelmet_crest'] = @{ h=80; b=50; a=12; l=0; wt=2.0 }

# --- D. Nasal/Painted/Conical/Bent Conical 35 (line 2938-2992) ---
$decisions['tv_vlandia_helmet_a']    = @{ h=57; b=0;  a=0;  l=0; wt=2.1 }
$decisions['tv_vlandia_helmet_b']    = @{ h=57; b=0;  a=0;  l=0; wt=2.1 }
$decisions['tv_vlandia_helmet_c']    = @{ h=57; b=0;  a=0;  l=0; wt=2.1 }
$decisions['tv_vlandia_helmet_t']    = @{ h=57; b=0;  a=0;  l=0; wt=2.1 }
$decisions['tv_vlandia_helmet_k']    = @{ h=68; b=0;  a=0;  l=0; wt=2.1 }
$decisions['tv_vlandia_helmet_a2']   = @{ h=57; b=12; a=8;  l=0; wt=2.4 }
$decisions['tv_vlandia_helmet_b2']   = @{ h=57; b=12; a=8;  l=0; wt=2.4 }
$decisions['tv_vlandia_helmet_c2']   = @{ h=57; b=12; a=8;  l=0; wt=2.4 }
$decisions['tv_vlandia_helmet_i']    = @{ h=57; b=12; a=8;  l=0; wt=2.4 }
$decisions['AR_vlandia_helmet_x']    = @{ h=60; b=60; a=8;  l=0; wt=3.0 }
$decisions['TV_vlandia_helmet_s']    = @{ h=68; b=6;  a=10; l=0; wt=2.5 }
$decisions['ao_vlandian_nasal_helmet_with_mail_coif'] = @{ h=89; b=24; a=40; l=0; wt=2.7 }
$decisions['AR_vlandia_helmet_i']    = @{ h=90; b=60; a=30; l=0; wt=3.6 }
$decisions['tv_vlandia_helmet_a3']   = @{ h=86; b=12; a=20; l=0; wt=3.1 }
$decisions['tv_vlandia_helmet_b3']   = @{ h=86; b=12; a=20; l=0; wt=3.1 }
$decisions['tv_vlandia_helmet_c3']   = @{ h=86; b=12; a=20; l=0; wt=3.1 }
$decisions['tv_vlandia_helmet_i2']   = @{ h=86; b=12; a=20; l=0; wt=3.1 }
$decisions['tv_vlandia_helmet_r2']   = @{ h=86; b=12; a=20; l=0; wt=3.1 }
$decisions['TV_vlandia_helmet_n']    = @{ h=92; b=12; a=25; l=0; wt=3.0 }
$decisions['tv_vlandia_helmet_a4']   = @{ h=89; b=24; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_b4']   = @{ h=89; b=24; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_c4']   = @{ h=89; b=24; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_i3']   = @{ h=89; b=24; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_j']    = @{ h=91; b=24; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_t3']   = @{ h=86; b=12; a=20; l=0; wt=3.0 }
$decisions['tv_vlandia_helmet_a5']   = @{ h=89; b=24; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_b5']   = @{ h=89; b=24; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_c5']   = @{ h=89; b=24; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_i4']   = @{ h=89; b=24; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_j2']   = @{ h=91; b=24; a=40; l=0; wt=3.2 }
$decisions['TV_vlandia_helmet_p']    = @{ h=86; b=24; a=40; l=0; wt=3.0 }
$decisions['TV_vlandia_helmet_q']    = @{ h=89; b=24; a=40; l=0; wt=3.0 }
$decisions['TV_vlandia_helmet_p2']   = @{ h=86; b=24; a=40; l=0; wt=3.0 }
$decisions['TV_vlandia_helmet_q2']   = @{ h=89; b=24; a=40; l=0; wt=3.0 }
$decisions['tv_vlandia_helmet_t4']   = @{ h=89; b=24; a=40; l=0; wt=3.2 }

# --- E. Banded 11 (line 3006-3016) ---
$decisions['tv_vlandia_helmet_o']    = @{ h=56; b=12; a=7;  l=0; wt=2.4 }
$decisions['tv_vlandia_helmet_o2']   = @{ h=84; b=0;  a=20; l=0; wt=3.1 }
$decisions['tv_vlandia_helmet_o3']   = @{ h=86; b=12; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_o4']   = @{ h=86; b=12; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_r']    = @{ h=57; b=12; a=8;  l=0; wt=2.4 }
$decisions['tv_vlandia_helmet_r3']   = @{ h=89; b=24; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_r4']   = @{ h=89; b=24; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_helmet_u']    = @{ h=60; b=60; a=8;  l=0; wt=2.5 }
$decisions['tv_vlandia_helmet_u2']   = @{ h=86; b=60; a=25; l=0; wt=3.0 }
$decisions['tv_vlandia_helmet_u3']   = @{ h=89; b=70; a=40; l=0; wt=3.2 }
$decisions['AR_vlandia_helmet_w']    = @{ h=60; b=60; a=0;  l=0; wt=3.0 }

# --- F. Peaked 10 (line 3030-3039) ---
$decisions['tv_vlandia_helmet_k2']   = @{ h=68; b=0;  a=0;  l=0; wt=2.5 }
$decisions['tv_vlandia_helmet_k3']   = @{ h=92; b=0;  a=20; l=0; wt=3.0 }
$decisions['tv_vlandia_helmet_k4']   = @{ h=90; b=25; a=25; l=0; wt=3.0 }
$decisions['tv_vlandia_helmet_k5']   = @{ h=92; b=25; a=25; l=0; wt=3.0 }
$decisions['tv_vlandia_helmet_k6']   = @{ h=95; b=12; a=40; l=0; wt=3.2 }
$decisions['TV_vlandia_helmet_l']    = @{ h=68; b=0;  a=12; l=0; wt=2.5 }
$decisions['TV_vlandia_helmet_m']    = @{ h=72; b=0;  a=12; l=0; wt=2.5 }
$decisions['AR_vlandia_helmet_s']    = @{ h=95; b=12; a=40; l=0; wt=4.0 }
$decisions['tv_vlandia_lord_helmet_c'] = @{ h=92; b=12; a=25; l=0; wt=3.0 }
$decisions['tv_vlandia_lord_helmet_d'] = @{ h=92; b=12; a=25; l=0; wt=3.0 }

# --- G. Roundkettle/Pointed 3 (line 3053-3055) ---
$decisions['DZ_vlandia_helmet_a']    = @{ h=60; b=0;  a=0;  l=0; wt=1.4 }
$decisions['TV_vlandia_helmet_h']    = @{ h=77; b=0;  a=0;  l=0; wt=2.5 }
$decisions['TV_vlandia_helmet_g']    = @{ h=92; b=5;  a=33; l=0; wt=2.9 }

# --- H. Flat Topped 2 (line 3067-3068) ---
$decisions['vlandia_helmet_s_ii']    = @{ h=55; b=0;  a=0;  l=0; wt=1.5 }
$decisions['vlandia_helmet_s_iii']   = @{ h=86; b=12; a=20; l=0; wt=2.5 }

# --- I. Fluted 6 (line 3082-3087) ---
$decisions['tv_vlandia_helmet_d']    = @{ h=95;  b=12; a=25; l=0; wt=3.5 }
$decisions['tv_vlandia_royal_helmet_c']  = @{ h=100; b=12; a=25; l=0; wt=3.5 }
$decisions['tv_vlandia_lord_helmet_a']   = @{ h=105; b=75; a=40; l=0; wt=4.0 }
$decisions['tv_vlandia_lord_helmet_a2']  = @{ h=105; b=75; a=40; l=0; wt=4.0 }
$decisions['tv_vlandia_lord_helmet_b']   = @{ h=105; b=75; a=40; l=0; wt=4.0 }
$decisions['tv_vlandia_lord_helmet_b2']  = @{ h=105; b=75; a=40; l=0; wt=4.0 }

# --- J. Cervelliere/Segmented/Domed 11 (line 3101-3111) ---
$decisions['AR_sturgia_helmet_n']    = @{ h=84; b=0;  a=20; l=0; wt=3.3 }   # Cervelliere Over Open Mail (mesh 归 Vlandia)
$decisions['tv_vlandia_lord_helmet_e']   = @{ h=80; b=50; a=20; l=0; wt=3.5 }
$decisions['tv_vlandia_lord_helmet_h']   = @{ h=95; b=75; a=40; l=0; wt=3.6 }
$decisions['tv_vlandia_lord_helmet_h2']  = @{ h=95; b=75; a=40; l=0; wt=3.6 }
$decisions['tv_vlandia_lord_helmet_i']   = @{ h=90; b=25; a=25; l=0; wt=3.0 }
$decisions['tv_vlandia_lord_helmet_k']   = @{ h=95; b=75; a=40; l=0; wt=3.6 }
$decisions['tv_vlandia_lord_helmet_k2']  = @{ h=95; b=75; a=40; l=0; wt=3.6 }
$decisions['tv_vlandia_lord_helmet_f']   = @{ h=95; b=75; a=40; l=0; wt=3.6 }
$decisions['tv_vlandia_lord_helmet_f2']  = @{ h=95; b=75; a=40; l=0; wt=3.6 }
$decisions['tv_vlandia_lord_helmet_l']   = @{ h=95; b=75; a=40; l=0; wt=3.6 }
$decisions['tv_vlandia_lord_helmet_l2']  = @{ h=95; b=75; a=40; l=0; wt=3.6 }

# --- K. Visored 7 (line 3125-3131) ---
$decisions['RW_vlandia_helmet_a']    = @{ h=83;  b=75; a=12; l=0; wt=2.3 }
$decisions['RW_vlandia_helmet_b']    = @{ h=89;  b=75; a=25; l=0; wt=2.5 }
$decisions['RW_vlandia_lord_helmet_a']   = @{ h=109; b=75; a=40; l=0; wt=3.6 }
$decisions['AR_vlandia_helmet_z']    = @{ h=83;  b=75; a=12; l=0; wt=2.3 }
$decisions['AR_vlandia_helmet_z2']   = @{ h=109; b=75; a=40; l=0; wt=3.6 }
$decisions['full_helm_over_mail_coif_x'] = @{ h=105; b=75; a=40; l=0; wt=4.0 }
$decisions['full_helm_over_mail_coif_z'] = @{ h=108; b=80; a=40; l=0; wt=4.0 }

# --- L. Knight's Helmet 6 (line 3145-3150) ---
$decisions['AR_vlandian_knights_helmet_a'] = @{ h=90; b=70; a=0;  l=0; wt=2.5 }
$decisions['AR_vlandian_knights_helmet_b'] = @{ h=90; b=70; a=0;  l=0; wt=2.5 }
$decisions['AR_vlandian_knights_helmet_c'] = @{ h=90; b=90; a=0;  l=0; wt=2.5 }
$decisions['tv_vlandia_lord_helmet_j']     = @{ h=95; b=90; a=25; l=0; wt=3.0 }
$decisions['tv_vlandia_lord_helmet_j2']    = @{ h=95; b=90; a=25; l=0; wt=3.0 }
$decisions['AR_vlandia_lord_helmet_d']     = @{ h=95; b=70; a=25; l=0; wt=3.0 }

# --- M. Bandedhelm/Goggled 3 (line 3164-3166) ---
$decisions['AR_vlandia_helmet_y']    = @{ h=90; b=60; a=25; l=0; wt=2.5 }
$decisions['AR_vlandia_lord_helmet_c']   = @{ h=92; b=60; a=25; l=0; wt=2.5 }
$decisions['AR_vlandia_lord_helmet_a']   = @{ h=95; b=60; a=25; l=0; wt=3.0 }

# --- N. Cataphract's Helmet 3 (line 3180-3182) ---
$decisions['AR_vlandia_helmet_t']        = @{ h=107; b=60; a=25; l=0; wt=3.0 }
$decisions['AR_vlandia_royal_helmet_a']  = @{ h=111; b=70; a=40; l=0; wt=3.5 }
$decisions['AR_vlandia_royal_helmet_b']  = @{ h=111; b=80; a=40; l=0; wt=3.5 }

# --- O. Crowned Mask 6 (line 3194-3199) ---
$decisions['AR_vlandian_crown_mask_a']   = @{ h=85; b=60; a=12; l=0; wt=3.5 }
$decisions['AR_vlandian_crown_mask_b']   = @{ h=95; b=60; a=25; l=0; wt=3.5 }
$decisions['ao_mailed_angevin_crown']    = @{ h=35; b=12; a=40; l=0; wt=1.7 }
$decisions['TV_vlandia_royal_helmet_a']  = @{ h=30; b=12; a=40; l=0; wt=1.7 }
$decisions['TV_vlandia_royal_helmet_b']  = @{ h=95; b=24; a=40; l=0; wt=3.2 }
$decisions['tv_vlandia_lord_helmet_g']   = @{ h=90; b=20; a=0;  l=0; wt=3.0 }

# ==================== Battania HeadArmor · Batch C (2026-09-28) ====================
# Deploy 断层收尾第三批 · ~145 件 · BALANCE_V2_LOG.md line 4002-4278
# 全 15 家族 · 全 🔵 log-only · v1 全 155 件 body/arm=0 · v2 补齐
# 排除 ao_battanian_guarded_aristocrats_spangenhelmet + _crest（已在 Vlandia C.3 batch）

# --- A. Cloth Hat/Scarf/Hood 8 ---
$decisions['highland_headscarf']     = @{ h=6;  b=0; a=0; l=0; wt=0.3 }
$decisions['AR_phrygian_cap_b']      = @{ h=8;  b=0; a=0; l=0; wt=0.5 }
$decisions['AR_hat_e']               = @{ h=11; b=0; a=0; l=0; wt=0.2 }
$decisions['AR_hat_f']               = @{ h=11; b=0; a=0; l=0; wt=0.2 }
$decisions['AR_hat_g']               = @{ h=11; b=0; a=0; l=0; wt=0.2 }
$decisions['TV_battania_hood_a']     = @{ h=11; b=0; a=0; l=0; wt=0.5 }
$decisions['TV_battania_hood_a2']    = @{ h=11; b=0; a=0; l=0; wt=0.5 }
$decisions['TV_battania_hood_a3']    = @{ h=11; b=0; a=0; l=0; wt=0.5 }

# --- B. Wolf/Bear Head + Fur Coif 4 ---
$decisions['TV_wolf_pelt_c']         = @{ h=25; b=0; a=0; l=0; wt=1.3 }
$decisions['TV_wolf_pelt_e']         = @{ h=25; b=0; a=0; l=0; wt=1.3 }
$decisions['battania_fur_cap']       = @{ h=34; b=0; a=0; l=0; wt=1.3 }
$decisions['TV_battania_shoulders_a']= @{ h=34; b=0; a=0; l=0; wt=1.4 }

# --- C. Highland Simple 板甲基础 18 ---
$decisions['ao_battanian_segmented_helmet']    = @{ h=42; b=0;  a=0; l=0; wt=1.8 }
$decisions['AR_battania_helmet_w']             = @{ h=48; b=0;  a=0; l=0; wt=2.5 }
$decisions['AR_battania_helmet_w2']            = @{ h=48; b=0;  a=0; l=0; wt=2.5 }
$decisions['ao_battanian_nasal_spangenhelm']   = @{ h=45; b=0;  a=0; l=0; wt=1.8 }
$decisions['ao_battanian_simple_ridge_helmet'] = @{ h=45; b=0;  a=0; l=0; wt=1.8 }
$decisions['battania_simple_spangenhelm']      = @{ h=42; b=0;  a=0; l=0; wt=1.5 }
$decisions['battania_skull_cap']               = @{ h=42; b=0;  a=0; l=0; wt=1.5 }
$decisions['ao_battanian_nasal_helmet']        = @{ h=45; b=0;  a=0; l=0; wt=1.8 }
$decisions['AR_battania_helmet_l']             = @{ h=50; b=40; a=0; l=0; wt=2.3 }
$decisions['AR_battania_helmet_m']             = @{ h=50; b=40; a=0; l=0; wt=2.3 }
$decisions['AR_battania_helmet_p']             = @{ h=48; b=0;  a=0; l=0; wt=2.3 }
$decisions['AR_battania_helmet_q']             = @{ h=48; b=0;  a=0; l=0; wt=2.3 }
$decisions['hmj_simple_nasal_helm']            = @{ h=45; b=0;  a=0; l=0; wt=1.5 }
$decisions['simple_helmet']                    = @{ h=45; b=0;  a=0; l=0; wt=1.8 }
$decisions['simple_helmet_scarf']              = @{ h=45; b=0;  a=0; l=0; wt=1.8 }
$decisions['ao_battanian_ridge_helmet']        = @{ h=50; b=0;  a=0; l=0; wt=2.0 }
$decisions['hmj_simple_nasal_helm_hood']       = @{ h=50; b=0;  a=0; l=0; wt=1.5 }
$decisions['simple_helmet_headcloth']          = @{ h=48; b=0;  a=0; l=0; wt=1.8 }

# --- D. Highland Tall/Kettle 8 ---
$decisions['AR_battania_helmet_c']             = @{ h=65; b=0;  a=12; l=0; wt=3.0 }
$decisions['TV_battania_lord_helmet_d']        = @{ h=65; b=0;  a=12; l=0; wt=3.0 }
$decisions['AR_battania_helmet_d']             = @{ h=70; b=0;  a=12; l=0; wt=3.0 }
$decisions['hmj_imperial_cheek_pteurges_plume']= @{ h=70; b=0;  a=12; l=0; wt=3.0 }
$decisions['AR_battania_helmet_a']             = @{ h=55; b=0;  a=0;  l=0; wt=2.5 }
$decisions['AR_battania_helmet_a2']            = @{ h=60; b=12; a=8;  l=0; wt=2.5 }
$decisions['AR_battania_helmet_b']             = @{ h=65; b=0;  a=0;  l=0; wt=2.7 }
$decisions['AR_battania_helmet_b2']            = @{ h=75; b=0;  a=20; l=0; wt=2.8 }

# --- E. Highland Ridge Helmet 13 ---
$decisions['AR_battania_helmet_za']            = @{ h=55; b=0;  a=0;  l=0; wt=2.8 }
$decisions['AR_battania_helmet_zb']            = @{ h=60; b=0;  a=0;  l=0; wt=2.8 }
$decisions['ao_battanian_noblemans_ridge_helmet']        = @{ h=75; b=0;  a=20; l=0; wt=2.5 }
$decisions['AR_battania_helmet_r']             = @{ h=70; b=0;  a=0;  l=0; wt=2.8 }
$decisions['AR_battania_helmet_s']             = @{ h=70; b=0;  a=0;  l=0; wt=2.8 }
$decisions['ao_battanian_noblemans_plumed_ridge_helmet'] = @{ h=75; b=0;  a=20; l=0; wt=2.5 }
$decisions['AR_battania_helmet_t']             = @{ h=82; b=0;  a=25; l=0; wt=3.0 }
$decisions['AR_battania_helmet_u']             = @{ h=82; b=0;  a=25; l=0; wt=3.0 }
$decisions['AR_battania_helmet_zc']            = @{ h=80; b=0;  a=20; l=0; wt=3.0 }
$decisions['AR_battania_lord_helmet_l']        = @{ h=90; b=0;  a=20; l=0; wt=3.0 }
$decisions['AR_battania_lord_helmet_m']        = @{ h=90; b=0;  a=20; l=0; wt=3.0 }
$decisions['AR_battania_lord_helmet_n']        = @{ h=94; b=12; a=0;  l=0; wt=3.3 }
$decisions['AR_battania_lord_helmet_o']        = @{ h=94; b=12; a=0;  l=0; wt=3.3 }

# --- F. Highland Nasal Over Mail 系 14 (log 记 12 实际展开 14) ---
$decisions['ao_eorling_riveted_nasal_helmet']  = @{ h=75; b=0; a=20; l=0; wt=2.5 }
$decisions['celtic_mail_helm']                 = @{ h=82; b=0; a=20; l=0; wt=3.0 }
$decisions['TV_pict_helmet_a']                 = @{ h=75; b=0; a=12; l=0; wt=2.8 }
$decisions['TV_pict_helmet_b']                 = @{ h=80; b=0; a=15; l=0; wt=2.8 }
$decisions['TV_pict_helmet_c']                 = @{ h=88; b=0; a=20; l=0; wt=3.0 }
$decisions['TV_pict_helmet_d']                 = @{ h=75; b=0; a=0;  l=0; wt=2.8 }
$decisions['TV_pict_helmet_e']                 = @{ h=82; b=0; a=12; l=0; wt=2.8 }
$decisions['TV_pict_helmet_f']                 = @{ h=88; b=0; a=20; l=0; wt=2.8 }
$decisions['TV_pict_helmet_g']                 = @{ h=75; b=0; a=0;  l=0; wt=2.8 }
$decisions['TV_pict_helmet_h']                 = @{ h=82; b=0; a=12; l=0; wt=2.8 }
$decisions['TV_pict_helmet_i']                 = @{ h=88; b=0; a=20; l=0; wt=2.8 }
$decisions['AR_battania_helmet_zd']            = @{ h=75; b=0; a=20; l=0; wt=3.2 }
$decisions['AR_battania_helmet_ze']            = @{ h=85; b=0; a=20; l=0; wt=3.2 }
$decisions['AR_battania_helmet_zf']            = @{ h=95; b=0; a=25; l=0; wt=3.2 }

# --- G. Highland Spangenhelm 10 (含 TV_pict_helmet_k/l · TV_battania_helmet_g/h/i) ---
$decisions['AR_battania_helmet_g']             = @{ h=65; b=0; a=0;  l=0; wt=2.8 }
$decisions['AR_battania_helmet_h']             = @{ h=70; b=0; a=0;  l=0; wt=2.8 }
$decisions['AR_battania_helmet_i']             = @{ h=85; b=0; a=20; l=0; wt=3.2 }
$decisions['TV_pict_helmet_j']                 = @{ h=70; b=0; a=0;  l=0; wt=2.8 }
$decisions['TV_pict_helmet_j2']                = @{ h=65; b=0; a=0;  l=0; wt=2.8 }
$decisions['TV_pict_helmet_k']                 = @{ h=75; b=0; a=0;  l=0; wt=2.8 }
$decisions['TV_pict_helmet_l']                 = @{ h=88; b=0; a=20; l=0; wt=2.8 }
$decisions['TV_battania_helmet_g']             = @{ h=85; b=0; a=0;  l=0; wt=3.0 }
$decisions['TV_battania_helmet_h']             = @{ h=88; b=0; a=0;  l=0; wt=3.0 }
$decisions['TV_battania_helmet_i']             = @{ h=88; b=0; a=0;  l=0; wt=3.0 }

# --- H. Highland Banded 6 ---
$decisions['AR_battania_helmet_y']             = @{ h=65; b=0; a=0;  l=0; wt=2.8 }
$decisions['AR_battania_helmet_y2']            = @{ h=65; b=0; a=0;  l=0; wt=2.8 }
$decisions['AR_battania_helmet_z']             = @{ h=85; b=0; a=20; l=0; wt=3.0 }
$decisions['TV_pict_helmet_m']                 = @{ h=68; b=0; a=0;  l=0; wt=2.8 }
$decisions['TV_pict_helmet_n']                 = @{ h=75; b=0; a=0;  l=0; wt=2.8 }
$decisions['TV_pict_helmet_o']                 = @{ h=82; b=0; a=0;  l=0; wt=2.8 }

# --- I. Dryatic Bent Conical 6 ---
$decisions['AR_phrygian_helmet_c']             = @{ h=65; b=0; a=0; l=0; wt=2.8 }
$decisions['AR_phrygian_helmet_c2']            = @{ h=65; b=0; a=0; l=0; wt=2.8 }
$decisions['AR_phrygian_helmet_a']             = @{ h=73; b=0; a=0; l=0; wt=2.8 }
$decisions['AR_phrygian_helmet_a2']            = @{ h=73; b=0; a=0; l=0; wt=2.8 }
$decisions['AR_phrygian_helmet_b']             = @{ h=82; b=0; a=0; l=0; wt=2.8 }
$decisions['AR_phrygian_helmet_b2']            = @{ h=82; b=0; a=0; l=0; wt=2.8 }

# --- J. Faceguard/Guarded Nobleman's 16 ---
$decisions['AR_battania_helmet_j']             = @{ h=70; b=40; a=0;  l=0; wt=3.0 }
$decisions['AR_battania_helmet_k']             = @{ h=70; b=40; a=0;  l=0; wt=3.0 }
$decisions['AR_battania_helmet_n']             = @{ h=70; b=40; a=0;  l=0; wt=3.0 }
$decisions['AR_battania_helmet_o']             = @{ h=70; b=40; a=0;  l=0; wt=3.0 }
$decisions['AR_battania_helmet_v']             = @{ h=72; b=40; a=0;  l=0; wt=3.0 }
$decisions['AR_battania_helmet_v2']            = @{ h=72; b=40; a=0;  l=0; wt=3.0 }
$decisions['ao_battanian_guarded_aristocrats_helmet']   = @{ h=75; b=40; a=0;  l=0; wt=2.5 }
$decisions['ao_eorling_guarded_noblemans_helmet']       = @{ h=85; b=40; a=0;  l=0; wt=3.2 }
$decisions['ao_eorling_guarded_noblemans_helmet_b']     = @{ h=85; b=40; a=0;  l=0; wt=3.2 }
$decisions['ao_eorling_heavy_guarded_noblemans_helmet_a'] = @{ h=90; b=50; a=0;  l=0; wt=3.4 }
$decisions['ao_eorling_heavy_guarded_noblemans_helmet_b'] = @{ h=90; b=50; a=0;  l=0; wt=3.4 }
$decisions['ao_eorling_heavy_guarded_noblemans_helmet_c'] = @{ h=90; b=50; a=0;  l=0; wt=3.4 }
$decisions['ao_eorling_guardian_helmet']       = @{ h=95; b=50; a=0;  l=0; wt=3.4 }
$decisions['AR_goth_helmet_a']                 = @{ h=88; b=0;  a=20; l=0; wt=3.2 }
$decisions['AR_goth_helmet_b']                 = @{ h=92; b=40; a=20; l=0; wt=3.2 }
$decisions['AR_goth_helmet_c']                 = @{ h=92; b=40; a=20; l=0; wt=3.2 }

# --- K. Cheek Guards Cap 15 ---
$decisions['TV_battania_helmet_l']             = @{ h=65; b=0;  a=0; l=0; wt=2.9 }
$decisions['TV_battania_helmet_m']             = @{ h=65; b=0;  a=0; l=0; wt=2.9 }
$decisions['TV_battania_helmet_c']             = @{ h=79; b=0;  a=0; l=0; wt=3.1 }   # Steel Fringed Decorated
$decisions['TV_battania_helmet_d']             = @{ h=73; b=0;  a=0; l=0; wt=3.1 }   # Bronze Fringed Decorated
$decisions['TV_battania_helmet_e']             = @{ h=74; b=0;  a=0; l=0; wt=3.1 }
$decisions['TV_battania_helmet_f']             = @{ h=74; b=0;  a=0; l=0; wt=3.1 }
$decisions['TV_battania_helmet_s']             = @{ h=71; b=0;  a=0; l=0; wt=3.2 }
$decisions['TV_battania_helmet_s2']            = @{ h=71; b=0;  a=0; l=0; wt=3.2 }
$decisions['TV_battania_lord_helmet_c']        = @{ h=89; b=0;  a=0; l=0; wt=3.1 }
$decisions['TV_battania_helmet_a']             = @{ h=79; b=0;  a=0; l=0; wt=3.0 }
$decisions['TV_battania_helmet_b']             = @{ h=79; b=0;  a=0; l=0; wt=3.0 }
$decisions['TV_battania_helmet_p']             = @{ h=92; b=12; a=0; l=0; wt=3.2 }
$decisions['TV_battania_helmet_q']             = @{ h=92; b=12; a=0; l=0; wt=3.2 }
$decisions['TV_battania_helmet_u']             = @{ h=92; b=12; a=0; l=0; wt=3.2 }
$decisions['TV_battania_helmet_u2']            = @{ h=92; b=12; a=0; l=0; wt=3.2 }

# --- L. Wolfskin/Bearhelmet 3 ---
$decisions['wolfskin_helmet_guard']            = @{ h=88; b=0; a=0; l=0; wt=2.5 }
$decisions['wolfhelmet']                       = @{ h=90; b=0; a=0; l=0; wt=2.5 }
$decisions['bearhelmet']                       = @{ h=92; b=0; a=0; l=0; wt=2.8 }

# --- M. Highland Crown 6 ---
$decisions['battanian_crown']                  = @{ h=15;  b=0;  a=0;  l=0; wt=0.5 }
$decisions['TV_battania_lord_helmet_a']        = @{ h=66;  b=0;  a=0;  l=0; wt=2.3 }
$decisions['TV_battania_lord_helmet_b']        = @{ h=66;  b=0;  a=0;  l=0; wt=2.3 }
$decisions['ao_battanian_crowned_helmet']      = @{ h=102; b=12; a=25; l=0; wt=2.5 }
$decisions['AR_battania_lord_helmet_a']        = @{ h=121; b=16; a=15; l=0; wt=3.2 }
$decisions['AR_battania_lord_helmet_b']        = @{ h=121; b=16; a=15; l=0; wt=3.2 }

# --- N. Highland Lord/Warlord/Noble 15 ---
$decisions['AR_battania_lord_helmet_c']        = @{ h=95;  b=16; a=15; l=0; wt=3.1 }
$decisions['AR_battania_lord_helmet_d']        = @{ h=95;  b=16; a=15; l=0; wt=3.1 }
$decisions['AR_battania_lord_helmet_e']        = @{ h=95;  b=16; a=15; l=0; wt=3.1 }
$decisions['AR_battania_lord_helmet_k']        = @{ h=95;  b=16; a=15; l=0; wt=3.1 }
$decisions['AR_battania_lord_helmet_j']        = @{ h=102; b=12; a=25; l=0; wt=3.1 }
$decisions['AR_battania_lord_helmet_f']        = @{ h=95;  b=12; a=0;  l=0; wt=3.3 }
$decisions['AR_battania_lord_helmet_g']        = @{ h=95;  b=12; a=0;  l=0; wt=3.3 }
$decisions['AR_battania_lord_helmet_h']        = @{ h=95;  b=12; a=0;  l=0; wt=3.3 }
$decisions['AR_battania_lord_helmet_i']        = @{ h=95;  b=12; a=0;  l=0; wt=3.3 }
$decisions['AR_battania_lord_helmet_p']        = @{ h=102; b=12; a=25; l=0; wt=4.0 }
$decisions['AR_battania_lord_helmet_q']        = @{ h=102; b=12; a=25; l=0; wt=4.0 }
$decisions['TV_battania_helmet_j']             = @{ h=95;  b=0;  a=20; l=0; wt=3.6 }
$decisions['TV_battania_helmet_k']             = @{ h=95;  b=0;  a=20; l=0; wt=3.6 }
$decisions['TV_battania_helmet_n']             = @{ h=95;  b=0;  a=0;  l=0; wt=3.5 }
$decisions['TV_battania_helmet_o']             = @{ h=95;  b=0;  a=0;  l=0; wt=3.5 }

# --- O. Bone Lamellar Shaman 3 ---
$decisions['AR_bone_helmet_a']                 = @{ h=55; b=0; a=0; l=0; wt=1.8 }
$decisions['AR_bone_helmet_b']                 = @{ h=60; b=0; a=0; l=0; wt=2.0 }
$decisions['AR_bone_helmet_c']                 = @{ h=65; b=0; a=0; l=0; wt=2.2 }

# ==================== Nord 精英维京线 (2026-09-24, NavalDLC vanilla override, 选项 B) ====================
# BALANCE_V2_LOG.md: 65 items (HeadArmor 15 + BodyArmor 22 + Cape 15 + HandArmor 7 + LegArmor 6)
# Sturgia RBM 顶 + Nord 精英升级 5-10. head 155 / body 115 / leg 65 均越 Sturgia 跨文化顶.

# --- Nord HeadArmor 15 ---
$decisions['berserker_helmet_reinforced']    = @{ h=155; b=95; a=50; l=0; wt=3.5 }
$decisions['berserker_helmet']               = @{ h=125; b=45; a=35; l=0; wt=2.0 }
$decisions['nord_helmet_a']                  = @{ h=140; b=70; a=45; l=0; wt=3.3 }
$decisions['nord_helmet_b']                  = @{ h=138; b=68; a=40; l=0; wt=3.2 }
$decisions['nord_helmet_c']                  = @{ h=135; b=68; a=40; l=0; wt=3.2 }
$decisions['improved_assassin_hood_q5']      = @{ h=125; b=45; a=35; l=0; wt=0.6 }
$decisions['nord_helmet_d']                  = @{ h=110; b=30; a=25; l=0; wt=2.8 }
$decisions['nord_helmet_da']                 = @{ h=95;  b=20; a=15; l=0; wt=2.5 }
$decisions['nord_helmet_e']                  = @{ h=100; b=22; a=35; l=0; wt=2.3 }
$decisions['nord_helmet_ea']                 = @{ h=80;  b=12; a=25; l=0; wt=2.2 }
$decisions['nord_helmet_f']                  = @{ h=85;  b=22; a=30; l=0; wt=2.1 }
$decisions['nord_helmet_fa']                 = @{ h=70;  b=12; a=22; l=0; wt=2.0 }
$decisions['nordic_civilian_hat_fur_brim']   = @{ h=14;  b=0;  a=0;  l=0; wt=0.6 }
$decisions['nordic_civilian_hat']            = @{ h=7;   b=0;  a=0;  l=0; wt=0.5 }
$decisions['nord_king_crown']                = @{ h=20;  b=0;  a=0;  l=0; wt=0.3 }

# --- Nord BodyArmor 22 ---
$decisions['nord_king_armor']                = @{ h=0; b=115; l=80; a=50; wt=28 }
$decisions['nord_king_armor_d']              = @{ h=0; b=110; l=75; a=45; wt=25 }
$decisions['nord_king_armor_b']              = @{ h=0; b=105; l=70; a=45; wt=24 }
$decisions['nord_king_armor_c']              = @{ h=0; b=100; l=60; a=35; wt=21 }
$decisions['improved_assassin_armor_q5']     = @{ h=0; b=95;  l=55; a=40; wt=5 }
$decisions['nord_chainmail_armor']           = @{ h=0; b=90;  l=50; a=45; wt=22 }
$decisions['norse_chainmail_pleatedtrousers']= @{ h=0; b=85;  l=45; a=40; wt=16 }
$decisions['nordic_chainmail_c']             = @{ h=0; b=80;  l=45; a=35; wt=18 }
$decisions['nordic_chainmail_a']             = @{ h=0; b=77;  l=44; a=44; wt=18 }
$decisions['northman_raider_armor']          = @{ h=0; b=75;  l=45; a=35; wt=23 }
$decisions['pirate_tier_3_armor']            = @{ h=0; b=70;  l=35; a=30; wt=20 }
$decisions['heavy_gambeson_armor']           = @{ h=0; b=50;  l=32; a=28; wt=6 }
$decisions['heavy_gambeson_armor_plain']     = @{ h=0; b=32;  l=20; a=18; wt=5 }
$decisions['double_belted_leather_armor']    = @{ h=0; b=28;  l=18; a=15; wt=6 }
$decisions['naval_strappy_tunic']            = @{ h=0; b=22;  l=15; a=12; wt=5 }
$decisions['pirate_tier_2_body']             = @{ h=0; b=22;  l=15; a=12; wt=5 }
$decisions['pirate_a']                       = @{ h=0; b=14;  l=8;  a=6;  wt=3 }
$decisions['nord_casual_tunic']              = @{ h=0; b=12;  l=8;  a=6;  wt=2 }
$decisions['nordic_civilian_dress_plaid']    = @{ h=0; b=10;  l=6;  a=5;  wt=4 }
$decisions['nordic_civilian_dress']          = @{ h=0; b=10;  l=6;  a=5;  wt=4 }
$decisions['dress_norse_lady']               = @{ h=0; b=8;   l=6;  a=5;  wt=2 }
$decisions['nord_poor_civil']                = @{ h=0; b=6;   l=4;  a=3;  wt=3 }

# --- Nord Cape 15 (Cape 三部分律 + Khuzait arm 30 特批档) ---
$decisions['improved_assassin_shoulder_q5']  = @{ h=0; b=32; l=0; a=12; wt=0.85 }
$decisions['northman_raider_shoulder']       = @{ h=0; b=32; l=0; a=22; wt=4.0 }
$decisions['nord_shoulder_b']                = @{ h=0; b=30; l=0; a=30; wt=2.7 }  # Lamellar Pauldrons arm 30 特批
$decisions['nord_shoulder_a']                = @{ h=0; b=28; l=0; a=30; wt=2.5 }  # Reinforced Lamellar Pauldrons
$decisions['nord_king_shoulder_armor']       = @{ h=0; b=25; l=0; a=22; wt=1.0 }
$decisions['nord_king_shoulder_armor_b']     = @{ h=0; b=25; l=0; a=20; wt=2.5 }
$decisions['bandit_hybrid_armor_shoulder_b'] = @{ h=0; b=25; l=0; a=18; wt=4.0 }
$decisions['chainmail_shoulder_armor_b']     = @{ h=0; b=20; l=0; a=15; wt=3.0 }
$decisions['berserker_cloak']                = @{ h=0; b=25; l=0; a=0;  wt=3.8 }  # Cloak 命名 arm=0
$decisions['northman_raider_cape']           = @{ h=0; b=22; l=0; a=0;  wt=2.3 }  # Cape 命名 arm=0
$decisions['fur_cape_a']                     = @{ h=0; b=20; l=0; a=0;  wt=1.5 }  # Cape 命名 arm=0
$decisions['nord_fur_shoulder']              = @{ h=0; b=18; l=0; a=0;  wt=3.0 }  # 视觉判断 Cape · arm=0
$decisions['fur_cape_b']                     = @{ h=0; b=10; l=0; a=0;  wt=1.2 }
$decisions['cape_norse_lady']                = @{ h=0; b=8;  l=0; a=0;  wt=1.0 }
$decisions['nord_casual_cloak']              = @{ h=0; b=6;  l=0; a=0;  wt=0.5 }

# --- Nord HandArmor 7 ---
$decisions['mail_mitten_d']                  = @{ h=0; b=0; l=0; a=60; wt=1.7 }  # Nord 顶 · +4 vs Sturgia 56
$decisions['mail_mitten_g']                  = @{ h=0; b=0; l=0; a=56; wt=1.7 }  # = Sturgia 顶
$decisions['hybrid_armor_gloves_b']          = @{ h=0; b=0; l=0; a=50; wt=1.8 }
$decisions['northman_wrist_armor']           = @{ h=0; b=0; l=0; a=35; wt=1.2 }
$decisions['studded_arm_guards']             = @{ h=0; b=0; l=0; a=25; wt=1.1 }
$decisions['splinted_bracers']               = @{ h=0; b=0; l=0; a=20; wt=1.1 }
$decisions['nord_casual_bracers']            = @{ h=0; b=0; l=0; a=18; wt=0.6 }

# --- Nord LegArmor 6 ---
$decisions['hybrid_armor_boots_b']           = @{ h=0; b=0; l=65; a=0; wt=2.0 }  # Nord 顶 · +5 vs Sturgia 60
$decisions['improved_assassin_boot_q5']      = @{ h=0; b=0; l=60; a=0; wt=1.0 }  # = Sturgia 顶
$decisions['strapped_jackboots']             = @{ h=0; b=0; l=28; a=0; wt=0.7 }
$decisions['northman_raider_boot']           = @{ h=0; b=0; l=22; a=0; wt=0.4 }
$decisions['nord_casual_boots']              = @{ h=0; b=0; l=12; a=0; wt=0.5 }
$decisions['nord_poor_boot']                 = @{ h=0; b=0; l=8;  a=0; wt=0.5 }

# ==================== Aserai 波斯萨珊追加 (2026-09-24, 20 items) ====================

# --- Aserai BodyArmor 顶级线 5 ---
$decisions['AR_aserai_scale_armor_c']        = @{ h=0; b=138; l=122; a=67; wt=28 }  # 波斯顶 · Hauberk
$decisions['AR_aserai_scale_armor_b']        = @{ h=0; b=130; l=95;  a=60; wt=22 }  # 单 mail
$decisions['AR_aserai_armor_y']              = @{ h=0; b=130; l=110; a=60; wt=20 }  # Alternating + Heavy Mail
$decisions['tv_aserai_lamellar_e']           = @{ h=0; b=125; l=95;  a=55; wt=22 }  # Southern Heavy Steel Lamellar
$decisions['tv_aserai_lamellar_e2']          = @{ h=0; b=122; l=95;  a=55; wt=22 }  # Brass -3

# --- Aserai Darshi 略微增强 10 ---
$decisions['AR_aserai_armor_u']              = @{ h=0; b=70; l=45; a=35; wt=9 }
$decisions['AR_aserai_armor_u2']             = @{ h=0; b=70; l=45; a=35; wt=9 }
$decisions['tv_empire_armor_s']              = @{ h=0; b=70; l=45; a=35; wt=9 }
$decisions['tv_empire_armor_s2']             = @{ h=0; b=70; l=45; a=35; wt=9 }
$decisions['AR_aserai_armor_v']              = @{ h=0; b=62; l=25; a=30; wt=9 }
$decisions['AR_aserai_armor_v2']             = @{ h=0; b=62; l=25; a=30; wt=9 }
$decisions['AR_aserai_armor_t']              = @{ h=0; b=58; l=32; a=22; wt=6 }
$decisions['AR_aserai_armor_t2']             = @{ h=0; b=58; l=32; a=22; wt=6 }
$decisions['tv_empire_armor_r']              = @{ h=0; b=58; l=32; a=22; wt=6 }
$decisions['tv_empire_armor_r2']             = @{ h=0; b=58; l=32; a=22; wt=6 }

# --- Aserai HeadArmor Darshi Close Mail/Cavalry 升档 5 ---
$decisions['AR_aserai_lord_helmet_f']        = @{ h=130; b=30; a=25; l=0; wt=3.7 }  # Closed Noble Cavalry
$decisions['AR_aserai_lord_helmet_f2']       = @{ h=115; b=22; a=20; l=0; wt=3.5 }  # Open Noble Cavalry
$decisions['AR_aserai_helmet_h']             = @{ h=124; b=36; a=25; l=0; wt=3.5 }  # Tall Over Closed Mail (覆盖 K 家族 95)
$decisions['TV_aserai_helmet_l']             = @{ h=124; b=36; a=25; l=0; wt=3.2 }  # Spangenhelm W/ Closed Mail
$decisions['TV_aserai_helmet_i']             = @{ h=122; b=36; a=25; l=0; wt=3.2 }  # Decorated Spangenhelm W/ Closed Mail

# ==================== Khuzait 232 items (2026-09-24) ====================
# HeadArmor 129 (G 铁浮屠 20 + 剩 14 家族 109) + BodyArmor 42 (华夏中原 12 + 剩 30) +
# Cape 38 (arm 30 特批 6 + 其他 32) + HandArmor 4 + LegArmor 6 + HorseHarness 13

# --- Khuzait HeadArmor G 家族 铁浮屠 20 ---
$decisions['TV_khuzait_helmet_r']  = @{ h=148; b=82; a=45; l=0; wt=3.8 }
$decisions['TV_khuzait_helmet_q']  = @{ h=148; b=82; a=45; l=0; wt=3.8 }
$decisions['TV_khuzait_helmet_s']  = @{ h=148; b=82; a=45; l=0; wt=3.8 }
$decisions['TV_khuzait_helmet_b']  = @{ h=148; b=82; a=45; l=0; wt=4.8 }
$decisions['TV_khuzait_helmet_u']  = @{ h=130; b=60; a=40; l=0; wt=3.7 }
$decisions['TV_khuzait_helmet_p']  = @{ h=124; b=36; a=25; l=0; wt=3.6 }
$decisions['TV_khuzait_helmet_n']  = @{ h=115; b=25; a=28; l=0; wt=3.4 }
$decisions['TV_khuzait_helmet_o']  = @{ h=115; b=25; a=28; l=0; wt=3.4 }
$decisions['TV_khuzait_helmet_m']  = @{ h=115; b=25; a=28; l=0; wt=3.4 }
$decisions['TV_khuzait_helmet_l']  = @{ h=115; b=25; a=28; l=0; wt=3.4 }
$decisions['TV_khuzait_helmet_k']  = @{ h=120; b=25; a=40; l=0; wt=3.5 }
$decisions['TV_khuzait_helmet_j']  = @{ h=110; b=40; a=0;  l=0; wt=3.3 }
$decisions['TV_khuzait_helmet_h']  = @{ h=100; b=12; a=35; l=0; wt=3.4 }
$decisions['TV_khuzait_helmet_i']  = @{ h=85;  b=12; a=22; l=0; wt=3.2 }
$decisions['TV_khuzait_helmet_g']  = @{ h=70;  b=6;  a=12; l=0; wt=3.0 }
$decisions['TV_khuzait_helmet_c']  = @{ h=70;  b=0;  a=0;  l=0; wt=2.8 }
$decisions['TV_khuzait_helmet_f']  = @{ h=70;  b=0;  a=0;  l=0; wt=2.8 }
$decisions['TV_khuzait_helmet_e']  = @{ h=68;  b=0;  a=0;  l=0; wt=2.8 }
$decisions['TV_khuzait_helmet_d']  = @{ h=68;  b=0;  a=0;  l=0; wt=2.8 }
$decisions['TV_khuzait_helmet_c2'] = @{ h=55;  b=0;  a=0;  l=0; wt=2.6 }

# --- Khuzait HeadArmor A Noble/Lord 14 ---
$decisions['AR_khuzait_lord_helmet_a']  = @{ h=105; b=25; a=0;  l=0; wt=3.7 }
$decisions['AR_khuzait_lord_helmet_c']  = @{ h=102; b=14; a=25; l=0; wt=3.5 }
$decisions['AR_khuzait_lord_helmet_c2'] = @{ h=100; b=14; a=25; l=0; wt=3.5 }
$decisions['AR_khuzait_lord_helmet_d']  = @{ h=110; b=25; a=30; l=0; wt=3.8 }
$decisions['AR_khuzait_lord_helmet_d2'] = @{ h=108; b=25; a=30; l=0; wt=3.8 }
$decisions['AR_khuzait_lord_helmet_e']  = @{ h=108; b=22; a=15; l=0; wt=3.7 }
$decisions['AR_khuzait_lord_helmet_h']  = @{ h=65;  b=8;  a=8;  l=0; wt=2.5 }
$decisions['TV_khuzait_lord_helmet_a']  = @{ h=100; b=14; a=22; l=0; wt=3.7 }
$decisions['TV_khuzait_lord_helmet_b']  = @{ h=100; b=14; a=22; l=0; wt=3.7 }
$decisions['TV_khuzait_lord_helmet_i']  = @{ h=102; b=22; a=28; l=0; wt=3.5 }
$decisions['TV_khuzait_lord_helmet_j']  = @{ h=95;  b=12; a=22; l=0; wt=3.4 }
$decisions['TV_khuzait_lord_helmet_k']  = @{ h=102; b=25; a=22; l=0; wt=3.6 }
$decisions['TV_khuzait_lord_helmet_m']  = @{ h=105; b=25; a=22; l=0; wt=3.6 }
$decisions['TV_khuzait_lord_helmet_n']  = @{ h=130; b=30; a=25; l=0; wt=3.7 }

# --- Khuzait HeadArmor B Cataphract 5 ---
$decisions['AR_khuzait_lord_helmet_b']       = @{ h=135; b=50; a=40; l=0; wt=3.9 }
$decisions['ao_durkhan_cataphract_helmet_a'] = @{ h=130; b=36; a=38; l=0; wt=3.8 }
$decisions['ao_durkhan_cataphract_helmet_b'] = @{ h=130; b=36; a=40; l=0; wt=3.8 }
$decisions['AR_khuzait_helmet_o']            = @{ h=125; b=25; a=35; l=0; wt=3.9 }
$decisions['AR_goth_helmet_f']               = @{ h=95;  b=22; a=28; l=0; wt=3.2 }

# --- Khuzait HeadArmor C Vendel 2 ---
$decisions['AR_vaegir_helmet_d']         = @{ h=106; b=65; a=40; l=0; wt=4.0 }
$decisions['AR_khuzait_lord_helmet_g']   = @{ h=100; b=45; a=28; l=0; wt=3.9 }

# --- Khuzait HeadArmor D Battle Crown 3 ---
$decisions['TV_khuzait_lord_helmet_c']  = @{ h=90; b=12; a=0;  l=0; wt=2.7 }
$decisions['TV_khuzait_lord_helmet_h']  = @{ h=92; b=12; a=25; l=0; wt=2.9 }
$decisions['TV_khuzait_lord_helmet_h2'] = @{ h=90; b=12; a=22; l=0; wt=2.9 }

# --- Khuzait HeadArmor E Spiked 8 ---
$decisions['AR_khuzait_helmet_g']       = @{ h=80;  b=6;  a=20; l=0; wt=3.8 }
$decisions['AR_khuzait_lord_helmet_f']  = @{ h=110; b=85; a=30; l=0; wt=3.9 }
$decisions['DZ_khuzait_helmet_d']       = @{ h=100; b=60; a=25; l=0; wt=4.5 }
$decisions['DZ_khuzait_helmet_e']       = @{ h=105; b=70; a=25; l=0; wt=4.5 }
$decisions['TV_khuzait_lord_helmet_e']  = @{ h=119; b=90; a=40; l=0; wt=4.5 }
$decisions['TV_khuzait_lord_helmet_l']  = @{ h=119; b=90; a=40; l=0; wt=4.5 }
$decisions['AR_vaegir_helmet_a']        = @{ h=94;  b=12; a=35; l=0; wt=4.5 }
$decisions['AR_vaegir_helmet_c']        = @{ h=94;  b=14; a=35; l=0; wt=4.6 }

# --- Khuzait HeadArmor F Plumed Lamellar 18 ---
$decisions['AR_goth_helmet_e']                   = @{ h=70;  b=6;  a=15; l=0; wt=3.5 }
$decisions['AR_goth_helmet_g']                   = @{ h=100; b=22; a=28; l=0; wt=4.2 }
$decisions['AR_goth_helmet_j']                   = @{ h=70;  b=6;  a=18; l=0; wt=1.7 }
$decisions['AR_goth_helmet_k']                   = @{ h=80;  b=12; a=25; l=0; wt=1.7 }
$decisions['AR_goth_helmet_l']                   = @{ h=82;  b=12; a=25; l=0; wt=1.7 }
$decisions['AR_goth_helmet_d']                   = @{ h=60;  b=6;  a=12; l=0; wt=3.2 }
$decisions['AR_khuzait_helmet_c']                = @{ h=65;  b=6;  a=12; l=0; wt=3.3 }
$decisions['AR_khuzait_helmet_c_fur']            = @{ h=65;  b=6;  a=12; l=0; wt=3.3 }
$decisions['ao_durkhan_heavy_lamellar_helmet']   = @{ h=95;  b=12; a=25; l=0; wt=3.8 }
$decisions['ao_durkhan_heavy_fur_trimmed_helmet']= @{ h=95;  b=12; a=25; l=0; wt=3.8 }
$decisions['TV_khuzait_lord_helmet_f']           = @{ h=80;  b=12; a=25; l=0; wt=3.4 }
$decisions['TV_khuzait_lord_helmet_g']           = @{ h=105; b=22; a=28; l=0; wt=4.6 }
$decisions['TV_khuzait_helmet_v']                = @{ h=65;  b=0;  a=15; l=0; wt=1.7 }
$decisions['TV_khuzait_helmet_w']                = @{ h=70;  b=6;  a=15; l=0; wt=1.7 }
$decisions['TV_khuzait_helmet_z']                = @{ h=60;  b=6;  a=12; l=0; wt=3.2 }
$decisions['TV_khuzait_helmet_z2']               = @{ h=70;  b=6;  a=15; l=0; wt=3.5 }
$decisions['TV_khuzait_helmet_z3']               = @{ h=77;  b=0;  a=25; l=0; wt=3.6 }
$decisions['TV_khuzait_helmet_a']                = @{ h=75;  b=12; a=22; l=0; wt=3.3 }

# --- Khuzait HeadArmor H Nasalhelm 5 ---
$decisions['AR_khuzait_helmet_v']       = @{ h=80;  b=12; a=22; l=0; wt=1.7 }
$decisions['AR_khuzait_helmet_w']       = @{ h=90;  b=22; a=28; l=0; wt=2.8 }
$decisions['AR_khuzait_helmet_u']       = @{ h=65;  b=6;  a=15; l=0; wt=1.7 }
$decisions['AR_khuzait_lord_helmet_i']  = @{ h=100; b=22; a=28; l=0; wt=2.8 }
$decisions['TV_khuzait_helmet_t']       = @{ h=90;  b=22; a=25; l=0; wt=3.15 }

# --- Khuzait HeadArmor I Spangenhelm 13 ---
$decisions['AR_khuzait_helmet_k'] = @{ h=60; b=6;  a=12; l=0; wt=1.7 }
$decisions['AR_khuzait_helmet_l'] = @{ h=58; b=6;  a=12; l=0; wt=1.7 }
$decisions['AR_khuzait_helmet_m'] = @{ h=85; b=22; a=28; l=0; wt=3.9 }
$decisions['AR_khuzait_helmet_n'] = @{ h=55; b=6;  a=12; l=0; wt=1.7 }
$decisions['AR_khuzait_helmet_p'] = @{ h=50; b=6;  a=12; l=0; wt=1.4 }
$decisions['AR_khuzait_helmet_q'] = @{ h=65; b=12; a=22; l=0; wt=1.7 }
$decisions['AR_khuzait_helmet_r'] = @{ h=70; b=12; a=22; l=0; wt=1.7 }
$decisions['AR_khuzait_helmet_s'] = @{ h=95; b=22; a=28; l=0; wt=1.7 }
$decisions['AR_khuzait_helmet_t'] = @{ h=97; b=22; a=40; l=0; wt=1.7 }
$decisions['AR_empire_helmet_c']  = @{ h=88; b=22; a=40; l=0; wt=3.1 }
$decisions['AR_empire_helmet_d']  = @{ h=76; b=12; a=15; l=0; wt=1.7 }
$decisions['AR_empire_helmet_e']  = @{ h=95; b=12; a=25; l=0; wt=3.3 }
$decisions['AR_empire_helmet_f']  = @{ h=65; b=12; a=25; l=0; wt=3.2 }

# --- Khuzait HeadArmor J Banded 5 ---
$decisions['AR_goth_helmet_m']    = @{ h=70;  b=6;  a=15; l=0; wt=3.2 }
$decisions['AR_goth_helmet_n']    = @{ h=90;  b=12; a=25; l=0; wt=3.5 }
$decisions['AR_goth_helmet_o']    = @{ h=92;  b=14; a=28; l=0; wt=3.2 }
$decisions['DZ_khuzait_helmet_b'] = @{ h=75;  b=6;  a=12; l=0; wt=4.5 }
$decisions['DZ_khuzait_helmet_c'] = @{ h=100; b=22; a=25; l=0; wt=4.0 }

# --- Khuzait HeadArmor K Iron/Brass 11 ---
$decisions['AR_khuzait_helmet_h']                     = @{ h=34; b=0;  a=0;  l=0; wt=1.4 }
$decisions['AR_khuzait_helmet_h2']                    = @{ h=32; b=0;  a=0;  l=0; wt=1.4 }
$decisions['AR_khuzait_helmet_i']                     = @{ h=46; b=0;  a=0;  l=0; wt=1.4 }
$decisions['AR_khuzait_helmet_i2']                    = @{ h=44; b=0;  a=0;  l=0; wt=1.4 }
$decisions['AR_khuzait_helmet_j']                     = @{ h=60; b=12; a=0;  l=0; wt=3.9 }
$decisions['ao_durkhan_iron_helmet_with_leather']     = @{ h=46; b=0;  a=0;  l=0; wt=3.8 }
$decisions['ao_durkhan_brass_helmet_with_leather']    = @{ h=44; b=0;  a=0;  l=0; wt=3.8 }
$decisions['ao_durkhan_iron_horsehair_helmet']        = @{ h=50; b=6;  a=0;  l=0; wt=3.8 }
$decisions['ao_durkhan_brass_horsehair_helmet']       = @{ h=48; b=6;  a=0;  l=0; wt=3.8 }
$decisions['ao_durkhan_iron_lamellar_helmet']         = @{ h=65; b=6;  a=22; l=0; wt=3.8 }
$decisions['ao_durkhan_brass_lamellar_helmet']        = @{ h=63; b=6;  a=22; l=0; wt=3.8 }

# --- Khuzait HeadArmor L Ornate Cap 10 ---
$decisions['TV_khuzait_lord_helmet_d'] = @{ h=45; b=0;  a=0;  l=0; wt=1.4 }
$decisions['TV_khuzait_helmet_y']      = @{ h=45; b=0;  a=0;  l=0; wt=1.4 }
$decisions['TV_khuzait_helmet_y2']     = @{ h=55; b=0;  a=0;  l=0; wt=2.1 }
$decisions['TV_khuzait_helmet_y3']     = @{ h=58; b=0;  a=0;  l=0; wt=2.1 }
$decisions['TV_khuzait_helmet_y4']     = @{ h=68; b=0;  a=0;  l=0; wt=4.6 }
$decisions['TV_khuzait_helmet_za']     = @{ h=55; b=0;  a=0;  l=0; wt=2.1 }
$decisions['TV_khuzait_helmet_zb']     = @{ h=55; b=0;  a=0;  l=0; wt=2.1 }
$decisions['DZ_khuzait_helmet_a']      = @{ h=65; b=12; a=22; l=0; wt=3.3 }
$decisions['AR_khuzait_helmet_c3']     = @{ h=38; b=0;  a=0;  l=0; wt=2.2 }
$decisions['TV_khuzait_helmet_a2']     = @{ h=45; b=0;  a=0;  l=0; wt=3.3 }

# --- Khuzait HeadArmor M Steppe Leather 6 ---
$decisions['AR_khuzait_helmet_b']  = @{ h=22; b=0; a=0; l=0; wt=1.4 }
$decisions['AR_khuzait_helmet_b2'] = @{ h=20; b=0; a=0; l=0; wt=1.4 }
$decisions['AR_khuzait_helmet_d']  = @{ h=14; b=0; a=0; l=0; wt=1.1 }
$decisions['AR_khuzait_helmet_d2'] = @{ h=14; b=0; a=0; l=0; wt=1.1 }
$decisions['AR_khuzait_helmet_e']  = @{ h=18; b=0; a=0; l=0; wt=1.1 }
$decisions['AR_khuzait_helmet_f']  = @{ h=18; b=0; a=0; l=0; wt=1.1 }

# --- Khuzait HeadArmor N Fur Cap 4 ---
$decisions['AR_khuzait_helmet_a'] = @{ h=7;  b=0; a=0; l=0; wt=0.6 }
$decisions['AR_khuzait_hood_a']   = @{ h=12; b=0; a=0; l=0; wt=0.6 }
$decisions['AR_khuzait_hood_b']   = @{ h=14; b=0; a=0; l=0; wt=0.7 }
$decisions['ON_cone_hat_a']       = @{ h=6;  b=0; a=0; l=0; wt=1.2 }

# --- Khuzait HeadArmor O Special 4 ---
$decisions['AR_goth_helmet_h']                  = @{ h=34; b=0;  a=0; l=0; wt=1.4 }
$decisions['AR_goth_helmet_i']                  = @{ h=38; b=6;  a=0; l=0; wt=1.7 }
$decisions['ao_durkhan_guarded_iron_helmet']    = @{ h=50; b=12; a=0; l=0; wt=3.8 }
$decisions['ao_durkhan_fur_trimmed_helmet']     = @{ h=50; b=6;  a=0; l=0; wt=3.1 }

# --- Khuzait BodyArmor 华夏中原线 12 ---
$decisions['eastern_heavy_lamellar_armor']   = @{ h=0; b=138; l=122; a=67; wt=28 }
$decisions['AR_aserai_armor_z']              = @{ h=0; b=138; l=122; a=67; wt=28 }
$decisions['ao_durkhan_lamellar_with_mail']  = @{ h=0; b=130; l=110; a=60; wt=25 }
$decisions['ao_durkhan_heavy_lamellar']      = @{ h=0; b=118; l=85;  a=50; wt=14 }
$decisions['TV_khuzait_armor_k']             = @{ h=0; b=120; l=95;  a=50; wt=22 }
$decisions['TV_khuzait_armor_j']             = @{ h=0; b=120; l=95;  a=50; wt=22 }
$decisions['TV_khuzait_armor_o']             = @{ h=0; b=110; l=65;  a=45; wt=20 }
$decisions['TV_khuzait_armor_n']             = @{ h=0; b=110; l=65;  a=45; wt=20 }
$decisions['TV_khuzait_armor_m']             = @{ h=0; b=95;  l=80;  a=38; wt=14 }
$decisions['TV_khuzait_armor_l']             = @{ h=0; b=95;  l=80;  a=38; wt=14 }
$decisions['TV_khuzait_armor_q']             = @{ h=0; b=80;  l=50;  a=28; wt=13 }
$decisions['TV_khuzait_armor_p']             = @{ h=0; b=80;  l=50;  a=28; wt=13 }

# --- Khuzait BodyArmor 剩 30 ---
$decisions['AR_khuzait_armor_b']                       = @{ h=0; b=100; l=44; a=44; wt=22 }
$decisions['AR_khuzait_armor_a']                       = @{ h=0; b=60;  l=40; a=35; wt=16 }
$decisions['ao_durkhan_nobles_lamellar']               = @{ h=0; b=75;  l=50; a=25; wt=14 }
$decisions['ao_durkhan_lamellar_thighguard']           = @{ h=0; b=60;  l=40; a=25; wt=12 }
$decisions['TV_khuzait_armor_g']                       = @{ h=0; b=50;  l=25; a=15; wt=14 }
$decisions['TV_khuzait_armor_u']                       = @{ h=0; b=50;  l=25; a=25; wt=18 }
$decisions['TV_khuzait_armor_s']                       = @{ h=0; b=32;  l=32; a=27; wt=8 }
$decisions['TV_khuzait_armor_e']                       = @{ h=0; b=32;  l=38; a=25; wt=14 }
$decisions['TV_khuzait_armor_f']                       = @{ h=0; b=32;  l=28; a=22; wt=13 }
$decisions['TV_khuzait_armor_t']                       = @{ h=0; b=28;  l=22; a=17; wt=12 }
$decisions['TV_khuzait_armor_h']                       = @{ h=0; b=24;  l=12; a=12; wt=8 }
$decisions['TV_khuzait_armor_r']                       = @{ h=0; b=18;  l=19; a=12; wt=3 }
$decisions['TV_khuzait_armor_b']                       = @{ h=0; b=35;  l=22; a=12; wt=13 }
$decisions['ao_durkhan_lamellar']                      = @{ h=0; b=32;  l=20; a=15; wt=10 }
$decisions['TV_khuzait_armor_i']                       = @{ h=0; b=22;  l=15; a=10; wt=8 }
$decisions['TV_khuzait_armor_a']                       = @{ h=0; b=24;  l=18; a=10; wt=12 }
$decisions['TV_khuzait_armor_d']                       = @{ h=0; b=28;  l=15; a=8;  wt=12 }
$decisions['TV_khuzait_armor_d2']                      = @{ h=0; b=28;  l=15; a=8;  wt=12 }
$decisions['ao_durkhan_light_lamellar']                = @{ h=0; b=22;  l=12; a=8;  wt=6 }
$decisions['TV_khuzait_armor_c']                       = @{ h=0; b=20;  l=10; a=8;  wt=10 }
$decisions['ao_khuzait_leather_vest']                  = @{ h=0; b=15;  l=12; a=6;  wt=2 }
$decisions['ao_durkhan_leather_lamellar']              = @{ h=0; b=18;  l=10; a=8;  wt=5 }
$decisions['ao_durkhan_sleeveless_leather_lamellar']   = @{ h=0; b=16;  l=8;  a=4;  wt=5 }
$decisions['ao_durkhan_chestplate']                    = @{ h=0; b=14;  l=6;  a=3;  wt=4 }
$decisions['ao_durkhan_topless_leather_lamellar']      = @{ h=0; b=14;  l=6;  a=2;  wt=4 }
$decisions['ao_durkhan_hide_poncho']                   = @{ h=0; b=12;  l=8;  a=4;  wt=1 }
$decisions['DZ_khuzait_armor_b']                       = @{ h=0; b=15;  l=12; a=12; wt=2 }
$decisions['DZ_khuzait_armor_a']                       = @{ h=0; b=12;  l=12; a=6;  wt=1 }
$decisions['ao_durkhan_trousers']                      = @{ h=0; b=0;   l=3;  a=0;  wt=0.4 }
$decisions['ao_durkhan_tunic']                         = @{ h=0; b=4;   l=3;  a=2;  wt=0.5 }

# --- Khuzait Cape 38 ---
# G arm 30 特批 6
$decisions['TV_khuzait_shoulder_g']              = @{ h=0; b=35; l=0; a=30; wt=3.5 }
$decisions['DZ_sturgia_shoulder_a']              = @{ h=0; b=30; l=0; a=30; wt=4.1 }
$decisions['TV_khuzait_shoulder_f']              = @{ h=0; b=40; l=0; a=30; wt=5.0 }
$decisions['eastern_heavy_lamellar_shoulders']   = @{ h=0; b=40; l=0; a=30; wt=4.5 }
$decisions['AR_aserai_shoulder_w']               = @{ h=0; b=40; l=0; a=30; wt=4.5 }
$decisions['AR_aserai_shoulder_w2']              = @{ h=0; b=40; l=0; a=30; wt=4.5 }
# F Elite Heavy 25 4
$decisions['TV_khuzait_shoulder_p']              = @{ h=0; b=35; l=0; a=25; wt=3.8 }
$decisions['TV_khuzait_shoulder_o']              = @{ h=0; b=35; l=0; a=25; wt=3.8 }
$decisions['TV_khuzait_shoulder_n']              = @{ h=0; b=33; l=0; a=25; wt=3.8 }
$decisions['TV_khuzait_shoulder_m']              = @{ h=0; b=33; l=0; a=25; wt=3.8 }
# E Standard 20 2
$decisions['TV_khuzait_shoulder_i']              = @{ h=0; b=30; l=0; a=20; wt=3.8 }
$decisions['TV_khuzait_shoulder_h']              = @{ h=0; b=30; l=0; a=20; wt=3.8 }
# D 部分覆盖 15 1
$decisions['TV_khuzait_shoulder_r']              = @{ h=0; b=32; l=0; a=15; wt=5.0 }
# C Chainmail Cape 8
$decisions['AR_khuzait_cape_a']                  = @{ h=0; b=26; l=0; a=12; wt=3.9 }
$decisions['AR_khuzait_cape_b']                  = @{ h=0; b=26; l=0; a=12; wt=3.9 }
$decisions['AR_khuzait_cape_c']                  = @{ h=0; b=26; l=0; a=12; wt=3.9 }
$decisions['AR_khuzait_cape_f']                  = @{ h=0; b=22; l=0; a=8;  wt=3.9 }
$decisions['AR_khuzait_cape_g']                  = @{ h=0; b=22; l=0; a=8;  wt=3.9 }
$decisions['AR_khuzait_cape_h']                  = @{ h=0; b=22; l=0; a=8;  wt=3.9 }
$decisions['brass_lamellar_cape']                = @{ h=0; b=26; l=0; a=12; wt=5.0 }
$decisions['brass_lamellar_cape_z']              = @{ h=0; b=26; l=0; a=12; wt=5.0 }
# B Blackened Leather 4
$decisions['TV_khuzait_shoulder_s']              = @{ h=0; b=26; l=0; a=12; wt=3.8 }
$decisions['TV_khuzait_shoulder_t']              = @{ h=0; b=26; l=0; a=12; wt=3.8 }
$decisions['AO_aserai_shoulders_d']              = @{ h=0; b=28; l=0; a=12; wt=4.2 }
$decisions['AO_aserai_shoulders_d2']             = @{ h=0; b=28; l=0; a=12; wt=4.2 }
# Leather Shoulders 4
$decisions['TV_khuzait_shoulder_k']              = @{ h=0; b=22; l=0; a=10; wt=2.3 }
$decisions['TV_khuzait_shoulder_l']              = @{ h=0; b=22; l=0; a=10; wt=2.3 }
$decisions['TV_khuzait_shoulder_j']              = @{ h=0; b=18; l=0; a=12; wt=2.5 }
$decisions['DZ_khuzait_shoulder_a']              = @{ h=0; b=16; l=0; a=8;  wt=4.1 }
# Shoulder Straps 2
$decisions['TV_khuzait_shoulder_a']              = @{ h=0; b=20; l=0; a=0;  wt=3.5 }
$decisions['TV_khuzait_shoulder_b']              = @{ h=0; b=20; l=0; a=0;  wt=3.5 }
# 单肩甲 2
$decisions['TV_khuzait_shoulder_e']              = @{ h=0; b=0;  l=0; a=15; wt=2.8 }
$decisions['TV_khuzait_shoulder_q']              = @{ h=0; b=0;  l=0; a=10; wt=2.8 }
# Small Pauldrons 2
$decisions['TV_khuzait_shoulder_c']              = @{ h=0; b=12; l=0; a=8;  wt=1.5 }
$decisions['TV_khuzait_shoulder_d']              = @{ h=0; b=14; l=0; a=10; wt=1.5 }
# Cape/Cloak 民用 3
$decisions['AR_khuzait_cape_d']                  = @{ h=0; b=8;  l=0; a=0;  wt=1.0 }
$decisions['AR_khuzait_cape_e']                  = @{ h=0; b=8;  l=0; a=0;  wt=1.0 }
$decisions['ao_durkhan_tassled_necklace']        = @{ h=0; b=1;  l=0; a=0;  wt=0.2 }

# --- Khuzait HandArmor 4 ---
$decisions['AR_khuzait_gloves_b'] = @{ h=0; b=0; l=0; a=50; wt=1.8 }
$decisions['AR_khuzait_gloves_a'] = @{ h=0; b=0; l=0; a=44; wt=0.5 }
$decisions['TV_khuzait_gloves_a'] = @{ h=0; b=0; l=0; a=25; wt=0.6 }
$decisions['TV_khuzait_gloves_b'] = @{ h=0; b=0; l=0; a=25; wt=0.5 }

# --- Khuzait LegArmor 6 ---
$decisions['eastern_splint_boots']       = @{ h=0; b=0; l=40; a=0; wt=2.7 }
$decisions['AO_durkhan_boots_c']         = @{ h=0; b=0; l=40; a=0; wt=2.7 }
$decisions['hmj_eastern_leather_boots']  = @{ h=0; b=0; l=30; a=0; wt=0.9 }
$decisions['AO_durkhan_boots_b']         = @{ h=0; b=0; l=30; a=0; wt=0.9 }
$decisions['TV_khuzait_boots_b']         = @{ h=0; b=0; l=20; a=0; wt=0.8 }
$decisions['ao_durkhan_tassled_boots']   = @{ h=0; b=0; l=15; a=0; wt=0.7 }

# --- Khuzait HorseHarness 13 (华夏中原线升档) ---
$decisions['AR_horse_armor_z']   = @{ h=90; b=50; l=50; a=60; wt=30 }
$decisions['AR_horse_armor_z3']  = @{ h=90; b=50; l=50; a=60; wt=30 }
$decisions['AR_horse_armor_x']   = @{ h=90; b=50; l=50; a=60; wt=30 }
$decisions['AR_horse_armor_z2']  = @{ h=90; b=50; l=50; a=60; wt=30 }
$decisions['AR_horse_armor_za']  = @{ h=90; b=50; l=50; a=60; wt=30 }
$decisions['AR_horse_armor_y']   = @{ h=70; b=50; l=5;  a=60; wt=17 }
$decisions['AR_horse_armor_y3']  = @{ h=70; b=50; l=5;  a=60; wt=17 }
$decisions['AR_horse_armor_w']   = @{ h=70; b=50; l=5;  a=60; wt=17 }
$decisions['AR_horse_armor_v']   = @{ h=70; b=50; l=5;  a=60; wt=17 }
$decisions['AR_horse_armor_g']   = @{ h=70; b=50; l=5;  a=60; wt=17 }
$decisions['AR_horse_armor_y2']  = @{ h=70; b=50; l=5;  a=60; wt=17 }
$decisions['AR_horse_armor_zh']  = @{ h=5;  b=0;  l=0;  a=5;  wt=8.5 }
$decisions['AR_horse_armor_zf']  = @{ h=5;  b=0;  l=0;  a=5;  wt=8.5 }

# ==================== Empire HorseHarness 26 件 (2026-09-24 T1-T9 重审) ====================
# T1 民用 3
$decisions['AR_horse_armor_zg']  = @{ h=5;  b=5;  l=3;  a=5;  wt=6 }   # Light Harness
$decisions['AR_horse_armor_zc']  = @{ h=8;  b=12; l=5;  a=8;  wt=8 }   # Noble Harness (+Noble)
$decisions['AR_horse_armor_zb']  = @{ h=12; b=15; l=5;  a=12; wt=10 }  # Stripped Noble
# T2 Half Padded 3
$decisions['AR_horse_armor_n3']  = @{ h=45; b=22; l=3;  a=28; wt=13 }
$decisions['AR_horse_armor_n']   = @{ h=45; b=22; l=3;  a=28; wt=13 }
$decisions['AR_horse_armor_n2']  = @{ h=45; b=22; l=3;  a=28; wt=13 }
# T3 Half Leather 1
$decisions['AR_horse_armor_i']   = @{ h=55; b=30; l=3;  a=35; wt=13 }
# T4 Full Studded Leather 1
$decisions['AR_horse_armor_h']   = @{ h=65; b=35; l=35; a=40; wt=18 }
# T7 Half Lamellar 4 (Lamellar 归 T7 单层甲片 = Scale)
$decisions['DZ_horse_armor_e']   = @{ h=90; b=50; l=5;  a=60; wt=20 }  # +Heavy wt +3
$decisions['DZ_horse_armor_f']   = @{ h=90; b=50; l=5;  a=60; wt=17 }
$decisions['DZ_horse_armor_g']   = @{ h=90; b=50; l=5;  a=60; wt=20 }  # +Heavy wt +3
$decisions['DZ_horse_armor_h']   = @{ h=90; b=50; l=5;  a=60; wt=17 }
# T6 Half Mail 4
$decisions['AR_horse_armor_b']   = @{ h=90; b=40; l=5;  a=50; wt=17 }
$decisions['AR_horse_armor_b2']  = @{ h=90; b=40; l=5;  a=50; wt=17 }
$decisions['AR_horse_armor_a']   = @{ h=90; b=40; l=5;  a=50; wt=17 }
$decisions['AR_horse_armor_a2']  = @{ h=90; b=40; l=5;  a=50; wt=17 }
# T7 Half Plate/Scale 3
$decisions['AR_horse_armor_f']   = @{ h=90; b=50; l=5;  a=60; wt=17 }
$decisions['AR_horse_armor_zaa'] = @{ h=90; b=50; l=5;  a=60; wt=17 }
$decisions['AR_horse_armor_zac'] = @{ h=90; b=50; l=5;  a=60; wt=17 }
# T7 Full Lamellar 2 (归 T7 单层甲片)
$decisions['DZ_horse_armor_b']   = @{ h=90; b=50; l=50; a=60; wt=30 }
$decisions['DZ_horse_armor_d']   = @{ h=90; b=50; l=50; a=60; wt=30 }
# T7 Full Plate/Scale 3
$decisions['AR_horse_armor_e']   = @{ h=90; b=50; l=50; a=60; wt=30 }
$decisions['AR_horse_armor_zab'] = @{ h=90; b=50; l=50; a=60; wt=30 }
$decisions['AR_horse_armor_zad'] = @{ h=90; b=50; l=50; a=60; wt=30 }
# T7 Full Lamellar Heavy 2 (Heavy wt +2)
$decisions['DZ_horse_armor_a']   = @{ h=90; b=50; l=50; a=60; wt=32 }
$decisions['DZ_horse_armor_c']   = @{ h=90; b=50; l=50; a=60; wt=32 }

# ==================== Vlandia HorseHarness 34 件 (2026-09-24 T1-T9 重审) ====================
# T1 民用 3
$decisions['AR_horse_armor_zah'] = @{ h=10; b=8;  l=5;  a=10; wt=8 }
$decisions['AR_horse_armor_s']   = @{ h=12; b=12; l=5;  a=12; wt=10 }
$decisions['AR_horse_armor_zai'] = @{ h=12; b=12; l=5;  a=12; wt=10 }
# T2 Half Padded/Cloth 3
$decisions['AR_horse_armor_zal'] = @{ h=45; b=22; l=3;  a=28; wt=13 }
$decisions['AR_horse_armor_zat'] = @{ h=45; b=22; l=3;  a=28; wt=13 }
$decisions['TV_horse_armor_c3']  = @{ h=45; b=22; l=3;  a=28; wt=13 }
# T2 Padded/Cloth Full 3
$decisions['AR_horse_armor_zak'] = @{ h=45; b=22; l=22; a=28; wt=16 }
$decisions['AR_horse_armor_zas'] = @{ h=45; b=22; l=22; a=28; wt=16 }
$decisions['TV_horse_armor_b3']  = @{ h=45; b=22; l=22; a=28; wt=16 }
# T2 Heavy Padded/Cloth 6 (Heavy +2 head, +2 wt)
$decisions['AR_horse_armor_o']   = @{ h=47; b=22; l=3;  a=28; wt=15 }  # Half
$decisions['AR_horse_armor_zaj'] = @{ h=47; b=22; l=22; a=28; wt=18 }  # Full
$decisions['AR_horse_armor_zar'] = @{ h=47; b=22; l=22; a=28; wt=18 }  # Full
$decisions['AR_horse_armor_zau'] = @{ h=45; b=22; l=3;  a=28; wt=13 }  # Half (no Heavy)
$decisions['TV_horse_armor_a3']  = @{ h=47; b=22; l=22; a=28; wt=18 }  # Full
$decisions['TV_horse_armor_d3']  = @{ h=45; b=22; l=3;  a=28; wt=13 }  # Half
# T5 半复合 (Leather Scale · Padded Mail) 4
$decisions['AR_horse_armor_m']   = @{ h=75; b=40; l=5;  a=50; wt=16 }  # Half Leather Scale
$decisions['AR_horse_armor_p']   = @{ h=45; b=22; l=22; a=28; wt=16 }  # Padded Full → T2
$decisions['AR_horse_armor_p2']  = @{ h=45; b=22; l=22; a=28; wt=16 }  # Padded Full → T2
$decisions['AR_horse_armor_l']   = @{ h=75; b=40; l=40; a=50; wt=22 }  # Leather Scale Full → T5 Full
# T7 Half Scale/Lamellar 6
$decisions['TV_horse_armor_c']   = @{ h=90; b=50; l=5;  a=60; wt=17 }  # Half Steel Scale
$decisions['TV_horse_armor_c2']  = @{ h=90; b=50; l=5;  a=60; wt=17 }  # Half Lamellar
$decisions['TV_horse_armor_d2']  = @{ h=90; b=50; l=5;  a=60; wt=17 }  # Half Lamellar
$decisions['TV_horse_armor_d']   = @{ h=90; b=50; l=5;  a=60; wt=17 }  # Half Steel Scale
$decisions['AR_horse_armor_k']   = @{ h=90; b=50; l=5;  a=60; wt=17 }  # Half Scale
$decisions['AR_horse_armor_k2']  = @{ h=90; b=50; l=5;  a=60; wt=17 }  # Half Lamellar
# T5 Padded Mail 2 (半复合)
$decisions['AR_horse_armor_zan'] = @{ h=75; b=40; l=40; a=50; wt=22 }  # Padded Mail
$decisions['AR_horse_armor_zam'] = @{ h=77; b=40; l=40; a=50; wt=24 }  # Heavy Padded Mail
# T6 Chainmail Full 1
$decisions['AR_horse_armor_zao'] = @{ h=90; b=40; l=40; a=50; wt=26 }  # Reinforced Chainmail (vanilla direct)
# T7 Full Scale/Lamellar 2
$decisions['TV_horse_armor_b']   = @{ h=90; b=50; l=50; a=60; wt=30 }  # Steel Scale Full
$decisions['TV_horse_armor_b2']  = @{ h=90; b=50; l=50; a=60; wt=30 }  # Lamellar Full
# T7 Full Heavy Scale/Lamellar 2 (Heavy wt +2)
$decisions['TV_horse_armor_a']   = @{ h=90; b=50; l=50; a=60; wt=32 }  # Heavy Steel Scale
$decisions['TV_horse_armor_a2']  = @{ h=90; b=50; l=50; a=60; wt=32 }  # Heavy Lamellar
# ⚠ T8 复合双层 2 (用户特批越 vanilla · Scale/Lamellar + Mail)
$decisions['AR_horse_armor_j']   = @{ h=95; b=52; l=52; a=62; wt=28 }  # Scale And Mail
$decisions['AR_horse_armor_j2']  = @{ h=95; b=52; l=52; a=62; wt=28 }  # Lamellar And Mail

# ==================== Sturgia HorseHarness 6 件 (2026-09-24 T1-T9 重审 · 跨文化统一) ====================
# ⚠ Sturgia vanilla `northern_ring_barding` 45/35/5/45/15 大幅越权 · 用户批准跨文化统一
$decisions['AR_horse_armor_zi']   = @{ h=12; b=13; l=5;  a=12; wt=8 }   # T1 Heavy Noble Harness
$decisions['AR_horse_armor_zd']   = @{ h=90; b=40; l=40; a=50; wt=26 }  # T6 Plated Ring Barding (Chainmail Full)
$decisions['AR_horse_armor_zag']  = @{ h=90; b=40; l=40; a=50; wt=26 }  # T6 Chainmail Barding Full
$decisions['AR_horse_armor_zae']  = @{ h=90; b=50; l=50; a=60; wt=30 }  # T7 Iron Scale Barding Full
$decisions['AR_horse_armor_zae2'] = @{ h=90; b=50; l=50; a=60; wt=30 }  # T7 Steel Scale Barding Full
$decisions['AR_horse_armor_zaf']  = @{ h=90; b=40; l=40; a=50; wt=26 }  # T6 Ringed Mail Barding Full

# ==================== Aserai HorseHarness 31 件 (2026-09-24 T1-T9 重审) ====================
# T1 民用 1
$decisions['AR_horse_armor_ze']   = @{ h=8;  b=12; l=5;  a=8;  wt=8 }  # Heavy Harness (+Heavy)
# T2 Wicker Dromedary (Cloth/Padded 变体)
$decisions['tv_camel_armor_i']    = @{ h=45; b=22; l=3;  a=28; wt=13 }  # Half Wicker
$decisions['tv_camel_armor_j']    = @{ h=45; b=22; l=22; a=28; wt=16 }  # Wicker Full
# T3 Padded Leather Dromedary
$decisions['tv_camel_armor_h']    = @{ h=55; b=30; l=30; a=35; wt=16 }  # Padded Leather Full
# T2 Half Padded 3
$decisions['AR_horse_armor_q']    = @{ h=45; b=22; l=3;  a=28; wt=13 }
$decisions['AR_horse_armor_q2']   = @{ h=45; b=22; l=3;  a=28; wt=13 }
$decisions['AR_horse_armor_q3']   = @{ h=45; b=22; l=3;  a=28; wt=13 }
# T3 Half Padded Leather Dromedary
$decisions['tv_camel_armor_g']    = @{ h=55; b=30; l=3;  a=35; wt=13 }
# T7 Half Scale Dromedary 2
$decisions['tv_camel_armor_a']    = @{ h=90; b=50; l=5;  a=60; wt=17 }  # Brass Half Scale (装饰无加成)
$decisions['tv_camel_armor_b']    = @{ h=90; b=50; l=5;  a=60; wt=17 }  # Steel Half Scale
# T2 Padded Full 3
$decisions['AR_horse_armor_r']    = @{ h=45; b=22; l=22; a=28; wt=16 }
$decisions['AR_horse_armor_r2']   = @{ h=45; b=22; l=22; a=28; wt=16 }
$decisions['AR_horse_armor_r3']   = @{ h=45; b=22; l=22; a=28; wt=16 }
# T4 Studded Leather Dromedary 2
$decisions['tv_camel_armor_e']    = @{ h=65; b=35; l=5;  a=40; wt=14 }  # Half
$decisions['tv_camel_armor_f']    = @{ h=65; b=35; l=35; a=40; wt=18 }  # Full
# T6 Half Mail 4
$decisions['AR_horse_armor_u']    = @{ h=90; b=40; l=5;  a=50; wt=17 }
$decisions['AR_horse_armor_u2']   = @{ h=90; b=40; l=5;  a=50; wt=17 }
$decisions['AR_horse_armor_t']    = @{ h=90; b=40; l=5;  a=50; wt=17 }
$decisions['AR_horse_armor_t2']   = @{ h=90; b=40; l=5;  a=50; wt=17 }
# T7 Half Scale/Lamellar 3
$decisions['AR_horse_armor_d']    = @{ h=90; b=50; l=5;  a=60; wt=17 }
$decisions['AR_horse_armor_d2']   = @{ h=90; b=50; l=5;  a=60; wt=17 }
$decisions['AR_horse_armor_d3']   = @{ h=90; b=50; l=5;  a=60; wt=17 }
# T7 Half Plate 1
$decisions['AR_horse_armor_zap']  = @{ h=90; b=50; l=5;  a=60; wt=17 }
# ⚠ T8 Half Mail And Plate 1 (复合双层)
$decisions['AR_horse_armor_zaq']  = @{ h=95; b=52; l=5;  a=62; wt=17 }
# T7 Full Plate 1
$decisions['AR_horse_armor_zap2'] = @{ h=90; b=50; l=50; a=60; wt=30 }
# ⚠ T8 Full Mail And Plate 1 (复合双层 · 越 vanilla)
$decisions['AR_horse_armor_zaq2'] = @{ h=95; b=52; l=52; a=62; wt=28 }
# T7 Full Scale/Lamellar 3
$decisions['AR_horse_armor_c']    = @{ h=90; b=50; l=50; a=60; wt=30 }
$decisions['AR_horse_armor_c2']   = @{ h=90; b=50; l=50; a=60; wt=30 }
$decisions['AR_horse_armor_c3']   = @{ h=90; b=50; l=50; a=60; wt=30 }
# T7 Full Scale Dromedary 2
$decisions['tv_camel_armor_c']    = @{ h=90; b=50; l=50; a=60; wt=30 }
$decisions['tv_camel_armor_d']    = @{ h=90; b=50; l=50; a=60; wt=30 }

# ==================== FIX Batch E1/V1/B1 · 金属盔 head 主导修正 (2026-09-28 晚) ====================
# 用户诊断：v2 v2b baseline 直接照抄 vanilla+RBM 的 "roundkettle_over_leather 74 vs over_mail 92" 差距
# 导致 metal helmet + light aventail (cloth/leather/padding) 命名的物品 head penalty 15-33 点
# 违背设计直觉：金属盔 mesh 才是主要防御，aventail 类型主要决定 body/arm
# 修正原则（vanilla+RBM Override · 铁律 10 第 2 次打破 · 用户 2026-09-28 明示）：
#   - head 由 base_type 决定（金属盔全档一致）· aventail 仅少量 head 加成 (≤+4)
#   - body/arm 反映 aventail 覆盖度：cloth 8/5 · leather 15/12 · mail 25/22 · mail coif 42/30
# 覆盖已有 decision（$decisions hashtable 后写覆盖前写）

# ---------- Empire Fix E1 (15 件) ----------
# --- Roundkettle 家族 ---
$decisions['ao_imperial_kettlehelm_with_leather']   = @{ h=90; b=18; a=10; l=0; wt=1.8 }   # was 74/20/12
$decisions['ao_imperial_kettlehelm_over_mail_coif'] = @{ h=94; b=42; a=30; l=0; wt=3.7 }   # was 90 · coif +4 head
# --- Pointed Kettle 家族 ---
$decisions['DZ_empire_helmet_e']                    = @{ h=88; b=12; a=8;  l=0; wt=2.0 }   # was 60/18/12 (Pointed Kettle Over Padding)
$decisions['DZ_empire_helmet_f']                    = @{ h=88; b=18; a=12; l=0; wt=2.2 }   # was 68/20/15 (Pointed Kettle Over Leather)
# --- Pointed Helmet (Cone family) ---
$decisions['ao_imperial_pointed_helmet_with_cloth_wrap']  = @{ h=85; b=8;  a=5;  l=0; wt=1.8 }   # was 80
$decisions['ao_imperial_pointed_helmet_with_leather']     = @{ h=85; b=18; a=12; l=0; wt=2.2 }   # was head OK, body/arm 调轻
$decisions['ao_imperial_cone_helmet_with_leather']        = @{ h=90; b=18; a=12; l=0; wt=2.5 }   # was body/arm 25/15 · 调轻
# --- Spangenhelm 家族 ---
$decisions['ao_imperial_spangenhelm_with_leather']              = @{ h=95; b=12; a=12; l=0; wt=2.0 }   # was 76 · Spangenhelm 金属 95
$decisions['ao_imperial_heavy_spangenhelm_with_leather']        = @{ h=99; b=15; a=15; l=0; wt=3.9 }   # was 80 · Heavy 99
$decisions['ao_imperial_nasal_spangenhelm_with_leather_strips'] = @{ h=98; b=12; a=12; l=0; wt=2.0 }   # was 79 · Nasal 98
$decisions['ao_imperial_spangenhelm_over_padded_coif']          = @{ h=95; b=22; a=20; l=0; wt=2.5 }   # was 70
$decisions['TV_empire_helmet_g']                                = @{ h=97; b=22; a=20; l=0; wt=2.0 }   # was 72 · Gilded +2
$decisions['AR_empire_helmet_q']                                = @{ h=95; b=8;  a=5;  l=0; wt=1.8 }   # was 65 (Over Cloth)
$decisions['AR_empire_helmet_r']                                = @{ h=95; b=8;  a=5;  l=0; wt=1.8 }   # was 65 (Over Stripped Cloth)
$decisions['AR_empire_helmet_s']                                = @{ h=95; b=8;  a=5;  l=0; wt=1.8 }   # was 65 (Eastern Over Cloth)
$decisions['ao_imperial_crowned_nasal_helmet']                  = @{ h=86; b=8;  a=5;  l=0; wt=1.9 }   # was 67 · Nasal 86
$decisions['AR_empire_desert_helmet_b']                         = @{ h=86; b=8;  a=5;  l=0; wt=3.2 }   # was 55 (Southern Nasal Over Cloth)
$decisions['AR_empire_desert_helmet_d']                         = @{ h=86; b=15; a=12; l=0; wt=2.2 }   # was 61 (Southern Nasal Over Leather)

# ---------- Vlandia Fix V1 (12 件) ----------
# C 家族 Western Spangenhelm light-aventail
$decisions['AR_vlandia_helmet_l']    = @{ h=85; b=15; a=12; l=0; wt=2.0 }   # was 55 (Over Leather)
$decisions['AR_vlandia_helmet_o']    = @{ h=85; b=12; a=12; l=0; wt=2.0 }   # was 75 (Bronze Over Cloth Coif · Bronze -1 净=84 · 就近 85)
$decisions['AR_vlandia_helmet_p']    = @{ h=85; b=22; a=18; l=0; wt=2.2 }   # was 75 (Bronze Over Padded Coif)
# D.2 Nasal Padded Cloth/Leather 4 件
$decisions['tv_vlandia_helmet_a2']   = @{ h=86; b=8;  a=5;  l=0; wt=2.6 }   # was 57
$decisions['tv_vlandia_helmet_b2']   = @{ h=86; b=8;  a=5;  l=0; wt=2.6 }
$decisions['tv_vlandia_helmet_c2']   = @{ h=86; b=8;  a=5;  l=0; wt=2.6 }
$decisions['tv_vlandia_helmet_i']    = @{ h=86; b=8;  a=5;  l=0; wt=2.6 }
$decisions['TV_vlandia_helmet_s']    = @{ h=90; b=8;  a=8;  l=0; wt=3.0 }   # was 68 (Peaked Lancer Cloth Cap)
# F Peaked light-aventail
$decisions['tv_vlandia_helmet_k2']   = @{ h=90; b=15; a=12; l=0; wt=3.0 }   # was 68 (Over Leather)
$decisions['TV_vlandia_helmet_l']    = @{ h=90; b=8;  a=8;  l=0; wt=3.0 }   # was 68 (Peaked Lancer Over Cloth)
$decisions['TV_vlandia_helmet_m']    = @{ h=90; b=15; a=12; l=0; wt=3.0 }   # was 72 (Peaked Lancer Over Leather)
# E Banded light-aventail
$decisions['tv_vlandia_helmet_o']    = @{ h=84; b=8;  a=5;  l=0; wt=2.6 }   # was 56 (Banded Cerv Padded Cloth)
$decisions['tv_vlandia_helmet_r']    = @{ h=86; b=8;  a=5;  l=0; wt=2.6 }   # was 57 (Banded Nasal Padded Cloth)
$decisions['tv_vlandia_helmet_u']    = @{ h=86; b=60; a=8;  l=0; wt=2.8 }   # was 60 (Banded Faceguard Padded Cloth · Faceguard body 60 保留)
# H Flat Topped light
$decisions['vlandia_helmet_s_ii']    = @{ h=86; b=15; a=12; l=0; wt=2.5 }   # was 55 (Flat Topped Over Leather)

# ---------- Battania Fix B1 (~20 件) ----------
# Ridge 家族 (9): vanilla `ridged_northernhelm` 94 · 金属 base ~85-90
$decisions['AR_battania_helmet_za']  = @{ h=85; b=8;  a=5;  l=0; wt=2.8 }   # was 55 (Iron Ridge Over Cloth)
$decisions['AR_battania_helmet_zb']  = @{ h=85; b=15; a=12; l=0; wt=2.8 }   # was 60 (Iron Ridge W. Leather)
$decisions['AR_battania_helmet_r']   = @{ h=85; b=0;  a=0;  l=0; wt=2.8 }   # was 70 (Steel Ridged 无 aventail)
$decisions['AR_battania_helmet_s']   = @{ h=84; b=0;  a=0;  l=0; wt=2.8 }   # was 70 (Bronze Ridged · -1)
$decisions['ao_battanian_noblemans_ridge_helmet']        = @{ h=90; b=12; a=25; l=0; wt=3.0 }   # was 75 (Ridge W. Mail)
$decisions['ao_battanian_noblemans_plumed_ridge_helmet'] = @{ h=90; b=12; a=25; l=0; wt=3.0 }   # was 75
$decisions['AR_battania_helmet_t']   = @{ h=90; b=12; a=25; l=0; wt=3.0 }   # was 82 (Steel Ridged Over Mail)
$decisions['AR_battania_helmet_u']   = @{ h=89; b=12; a=25; l=0; wt=3.0 }   # was 82 (Bronze · -1)
$decisions['AR_battania_helmet_zc']  = @{ h=88; b=12; a=22; l=0; wt=3.0 }   # was 80 (Iron Ridge Over Stripped Mail)
# Nasal 家族 (8): vanilla `roughscale_helmet` 88 · Heavy Nasal 95
$decisions['TV_pict_helmet_a']       = @{ h=88; b=8;  a=5;  l=0; wt=2.8 }   # was 75 (Heavy Banded Nasal)
$decisions['TV_pict_helmet_b']       = @{ h=88; b=15; a=12; l=0; wt=2.8 }   # was 80 (Heavy Banded Nasal Over Hide)
$decisions['TV_pict_helmet_d']       = @{ h=88; b=0;  a=0;  l=0; wt=2.8 }   # was 75 (Domed Nasal Spangen 无 aventail)
$decisions['TV_pict_helmet_e']       = @{ h=88; b=8;  a=5;  l=0; wt=2.8 }   # was 82 (Domed + Padding)
$decisions['TV_pict_helmet_g']       = @{ h=88; b=0;  a=0;  l=0; wt=2.8 }   # was 75 (Tall Nasal Spangen 无 aventail)
$decisions['TV_pict_helmet_h']       = @{ h=88; b=8;  a=5;  l=0; wt=2.8 }   # was 82 (Tall Nasal + Cloth)
$decisions['AR_battania_helmet_zd']  = @{ h=95; b=8;  a=5;  l=0; wt=3.2 }   # was 75 (Heavy Nasalhelm Cloth)
$decisions['AR_battania_helmet_ze']  = @{ h=95; b=15; a=12; l=0; wt=3.2 }   # was 85 (Heavy Nasalhelm Leather)
# Banded 家族 (3)
$decisions['AR_battania_helmet_y']   = @{ h=85; b=15; a=12; l=0; wt=2.8 }   # was 65 (Banded Over Leather)
$decisions['AR_battania_helmet_y2']  = @{ h=85; b=8;  a=5;  l=0; wt=2.8 }   # was 65 (Banded Over Cloth)
$decisions['AR_battania_helmet_z']   = @{ h=90; b=12; a=25; l=0; wt=3.0 }   # was 85 (Banded Over Mail)
# Spangenhelm 家族 (3)
$decisions['AR_battania_helmet_h']   = @{ h=85; b=8;  a=5;  l=0; wt=2.8 }   # was 70 (Feathered Spangen Over Suede)
$decisions['TV_pict_helmet_j']       = @{ h=90; b=0;  a=0;  l=0; wt=2.8 }   # was 70 (Tall Silvered Nasal Spangen · Silvered +2)
$decisions['TV_pict_helmet_k']       = @{ h=90; b=0;  a=0;  l=0; wt=2.8 }   # was 75 (Tall Plumed Silvered Spangen)

# ==================== Batch D · Full Deploy · 2026-09-28 晚 (~890 件) ====================
# 用户 2026-09-28 明示：一次性 deploy 所有剩余 log-only 决议（跨 5 文化 · Cape/Body/Hand/Leg + Sturgia/Aserai HeadArmor）
# 数据来自 4 并行 agent 抽取 BALANCE_V2_LOG.md · 决议已归档 · 铁律遵循 · 无追溯冲突

# ========== Empire · Cape G-T (from agent A1) ==========
$decisions['AR_imperial_shoulders_a']         = @{ h=0; b=40; a=20; l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_e']         = @{ h=0; b=40; a=20; l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_e3']        = @{ h=0; b=40; a=20; l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_q']         = @{ h=0; b=40; a=20; l=0; wt=3.8 }
$decisions['AR_imperial_lamellar_shoulders']  = @{ h=0; b=42; a=25; l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_a2']        = @{ h=0; b=42; a=25; l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_e2']        = @{ h=0; b=42; a=25; l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_e4']        = @{ h=0; b=42; a=25; l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_q2']        = @{ h=0; b=42; a=25; l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_b']         = @{ h=0; b=35; a=12; l=0; wt=3.6 }
$decisions['AR_imperial_shoulders_f']         = @{ h=0; b=35; a=12; l=0; wt=3.6 }
$decisions['AR_imperial_shoulders_f2']        = @{ h=0; b=35; a=12; l=0; wt=3.6 }
$decisions['AR_imperial_shoulders_h']         = @{ h=0; b=35; a=12; l=0; wt=3.6 }
$decisions['AR_imperial_shoulders_r']         = @{ h=0; b=35; a=12; l=0; wt=3.6 }
$decisions['DZ_empire_shoulder_a']            = @{ h=0; b=35; a=12; l=0; wt=3.6 }
$decisions['DZ_empire_shoulder_b']            = @{ h=0; b=35; a=12; l=0; wt=3.6 }
$decisions['AR_imperial_shoulders_o']         = @{ h=0; b=26; a=8;  l=0; wt=3.9 }
$decisions['AR_imperial_shoulders_o2']        = @{ h=0; b=26; a=8;  l=0; wt=3.9 }
$decisions['TV_empire_shoulders_b']           = @{ h=0; b=26; a=8;  l=0; wt=3.9 }
$decisions['AR_imperial_shoulders_c']         = @{ h=0; b=32; a=0;  l=0; wt=3.5 }
$decisions['AR_imperial_shoulders_l']         = @{ h=0; b=32; a=0;  l=0; wt=3.5 }
$decisions['AR_imperial_shoulders_d']         = @{ h=0; b=38; a=20; l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_v']         = @{ h=0; b=32; a=0;  l=0; wt=3.6 }
$decisions['AR_imperial_shoulders_u']         = @{ h=0; b=32; a=0;  l=0; wt=3.6 }
$decisions['AR_imperial_shoulders_x']         = @{ h=0; b=33; a=0;  l=0; wt=3.7 }
$decisions['AR_imperial_shoulders_w']         = @{ h=0; b=33; a=0;  l=0; wt=3.7 }
$decisions['AR_imperial_shoulders_y']         = @{ h=0; b=40; a=15; l=0; wt=4.5 }
$decisions['AR_wolf_shoulder_a']              = @{ h=0; b=40; a=20; l=0; wt=3.8 }
$decisions['AR_wolf_shoulder_b']              = @{ h=0; b=40; a=20; l=0; wt=3.8 }
$decisions['AR_wolf_shoulder_c']              = @{ h=0; b=35; a=12; l=0; wt=3.6 }
$decisions['AR_wolf_shoulder_d']              = @{ h=0; b=35; a=12; l=0; wt=3.6 }
$decisions['AR_wolf_shoulder_e']              = @{ h=0; b=18; a=6;  l=0; wt=2.2 }
$decisions['AR_wolf_shoulder_e2']             = @{ h=0; b=18; a=6;  l=0; wt=2.2 }
$decisions['AR_wolf_shoulder_f']              = @{ h=0; b=35; a=10; l=0; wt=2.9 }
$decisions['AR_wolf_shoulder_f2']             = @{ h=0; b=35; a=10; l=0; wt=2.9 }
$decisions['AR_wolf_shoulder_g']              = @{ h=0; b=32; a=0;  l=0; wt=2.9 }
$decisions['AR_wolf_shoulder_g2']             = @{ h=0; b=32; a=0;  l=0; wt=2.9 }
$decisions['AR_imperial_shoulders_g']         = @{ h=0; b=17; a=12; l=0; wt=3.5 }
$decisions['BA_empire_shoulders_a']           = @{ h=0; b=24; a=12; l=0; wt=3.0 }
$decisions['BA_empire_shoulders_b']           = @{ h=0; b=26; a=12; l=0; wt=3.0 }
$decisions['BA_empire_shoulders_c']           = @{ h=0; b=20; a=10; l=0; wt=2.8 }
$decisions['BA_empire_shoulders_d']           = @{ h=0; b=22; a=10; l=0; wt=2.8 }
$decisions['AR_imperial_shoulders_i']         = @{ h=0; b=28; a=0;  l=0; wt=3.6 }
$decisions['AR_imperial_shoulders_j']         = @{ h=0; b=31; a=0;  l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_j2']        = @{ h=0; b=31; a=0;  l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_k']         = @{ h=0; b=33; a=0;  l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_k2']        = @{ h=0; b=33; a=0;  l=0; wt=3.8 }
$decisions['AR_imperial_shoulders_z']         = @{ h=0; b=33; a=0;  l=0; wt=3.6 }
$decisions['AR_imperial_shoulders_za']        = @{ h=0; b=33; a=0;  l=0; wt=3.6 }
$decisions['AR_imperial_shoulders_zb']        = @{ h=0; b=35; a=0;  l=0; wt=3.6 }
$decisions['AR_imperial_shoulders_m']         = @{ h=0; b=16; a=4;  l=0; wt=2.0 }
$decisions['AR_imperial_shoulders_s']         = @{ h=0; b=20; a=6;  l=0; wt=2.2 }
$decisions['AR_imperial_shoulders_t']         = @{ h=0; b=17; a=5;  l=0; wt=2.0 }
$decisions['AR_empire_shoulders_b']           = @{ h=0; b=22; a=6;  l=0; wt=1.8 }
$decisions['AR_imperial_leather_cape_a']      = @{ h=0; b=22; a=8;  l=0; wt=3.5 }
$decisions['AR_imperial_leather_cape_b']      = @{ h=0; b=22; a=8;  l=0; wt=3.5 }
$decisions['AR_imperial_leather_cape_b2']     = @{ h=0; b=22; a=8;  l=0; wt=3.5 }
$decisions['tv_imperial_leather_cape_a']      = @{ h=0; b=22; a=8;  l=0; wt=3.5 }
$decisions['tv_imperial_leather_cape_b']      = @{ h=0; b=22; a=8;  l=0; wt=3.5 }
$decisions['AR_imperial_shoulders_n']         = @{ h=0; b=35; a=10; l=0; wt=2.9 }
$decisions['AR_imperial_shoulders_p']         = @{ h=0; b=22; a=8;  l=0; wt=3.4 }
$decisions['AR_imperial_shoulders_p2']        = @{ h=0; b=18; a=6;  l=0; wt=3.2 }
$decisions['AR_gladiator_shoulder_a']         = @{ h=0; b=20; a=12; l=0; wt=2.5 }
$decisions['AR_gladiator_shoulder_b']         = @{ h=0; b=22; a=12; l=0; wt=2.8 }
$decisions['TV_empire_shoulders_a']           = @{ h=0; b=22; a=8;  l=0; wt=3.4 }
$decisions['AR_noble_collar_a']               = @{ h=0; b=24; a=10; l=0; wt=2.5 }
$decisions['lamellar_scarf']                  = @{ h=0; b=40; a=20; l=0; wt=3.8 }
$decisions['AR_imperial_lamellar_cape_a']     = @{ h=0; b=45; a=0;  l=0; wt=4.0 }
$decisions['AR_imperial_lamellar_cape_b']     = @{ h=0; b=45; a=0;  l=0; wt=4.0 }
$decisions['AR_imperial_lamellar_cape_c']     = @{ h=0; b=45; a=0;  l=0; wt=4.0 }
$decisions['AR_imperial_lamellar_cape_e']     = @{ h=0; b=45; a=0;  l=0; wt=4.0 }
$decisions['AR_imperial_lamellar_cape_f']     = @{ h=0; b=45; a=0;  l=0; wt=4.0 }
$decisions['AR_imperial_lamellar_cape_g']     = @{ h=0; b=45; a=0;  l=0; wt=4.0 }
$decisions['AR_imperial_lamellar_cape_a2']    = @{ h=0; b=47; a=0;  l=0; wt=4.0 }
$decisions['AR_imperial_lamellar_cape_b2']    = @{ h=0; b=47; a=0;  l=0; wt=4.0 }
$decisions['AR_noble_collar_b']               = @{ h=0; b=50; a=0;  l=0; wt=4.0 }
$decisions['AR_imperial_lamellar_cape_d']     = @{ h=0; b=28; a=8;  l=0; wt=4.0 }
$decisions['ao_focale']                       = @{ h=0; b=1;  a=0;  l=0; wt=0.1 }
$decisions['ao_imperial_noblemans_sash']      = @{ h=0; b=5;  a=0;  l=0; wt=0.5 }
$decisions['AR_leopard_pelt_a']               = @{ h=0; b=6;  a=0;  l=0; wt=1.5 }
$decisions['AR_lion_head_a']                  = @{ h=0; b=22; a=0;  l=0; wt=5.0 }
$decisions['AR_lion_head_b']                  = @{ h=0; b=28; a=0;  l=0; wt=5.0 }
$decisions['AR_lion_head_d']                  = @{ h=0; b=20; a=0;  l=0; wt=5.0 }
$decisions['TV_battania_cloak_m']             = @{ h=0; b=5;  a=0;  l=0; wt=0.5 }
$decisions['TV_battania_cloak_m2']            = @{ h=0; b=5;  a=0;  l=0; wt=0.5 }
$decisions['TV_battania_cloak_m3']            = @{ h=0; b=5;  a=0;  l=0; wt=0.5 }
$decisions['TV_battania_cloak_o']             = @{ h=0; b=2;  a=0;  l=0; wt=0.2 }
$decisions['TV_battania_cloak_o2']            = @{ h=0; b=8;  a=0;  l=0; wt=1.5 }
$decisions['TV_battania_cloak_o3']            = @{ h=0; b=8;  a=0;  l=0; wt=1.5 }

# ========== Empire · BodyArmor A-G (from agent A1 · H family already deployed) ==========
$decisions['AR_empire_armor_f']               = @{ h=0; b=3;  a=2;  l=3;  wt=0.4 }
$decisions['AR_empire_armor_g']               = @{ h=0; b=3;  a=2;  l=3;  wt=0.4 }
$decisions['AR_noble_dress_a']                = @{ h=0; b=7;  a=5;  l=5;  wt=0.5 }
$decisions['AR_noble_dress_a2']               = @{ h=0; b=7;  a=5;  l=5;  wt=0.5 }
$decisions['AR_vlandia_armor_g']              = @{ h=0; b=6;  a=5;  l=5;  wt=0.4 }
$decisions['plain_hemp_tunic']                = @{ h=0; b=5;  a=5;  l=5;  wt=0.4 }
$decisions['AR_empire_armor_p']               = @{ h=0; b=6;  a=4;  l=6;  wt=0.5 }
$decisions['AO_imperial_scouts_tunic']        = @{ h=0; b=7;  a=5;  l=5;  wt=0.4 }
$decisions['AR_empire_armor_k']               = @{ h=0; b=8;  a=7;  l=7;  wt=0.6 }
$decisions['AR_empire_armor_q']               = @{ h=0; b=7;  a=5;  l=5;  wt=0.4 }
$decisions['TV_empire_armor_k']               = @{ h=0; b=8;  a=2;  l=3;  wt=1.0 }
$decisions['AR_empire_armor_b']               = @{ h=0; b=8;  a=6;  l=8;  wt=1.5 }
$decisions['AR_empire_armor_m']               = @{ h=0; b=8;  a=6;  l=8;  wt=0.9 }
$decisions['AR_empire_armor_n']               = @{ h=0; b=8;  a=6;  l=8;  wt=0.9 }
$decisions['AR_empire_armor_t']               = @{ h=0; b=10; a=6;  l=8;  wt=0.9 }
$decisions['AR_merchants_coat_a']             = @{ h=0; b=8;  a=6;  l=8;  wt=1.0 }
$decisions['AR_empire_scout_armor_c']         = @{ h=0; b=14; a=12; l=14; wt=1.9 }
$decisions['AR_huntress_armor_a']             = @{ h=0; b=14; a=12; l=14; wt=1.9 }
$decisions['AR_empire_armor_e']               = @{ h=0; b=15; a=13; l=15; wt=2.1 }
$decisions['AR_empire_armor_d']               = @{ h=0; b=16; a=14; l=16; wt=2.1 }
$decisions['AR_empire_dress_armor_a']         = @{ h=0; b=18; a=12; l=18; wt=2.0 }
$decisions['AR_huntress_armor_c']             = @{ h=0; b=18; a=12; l=18; wt=2.0 }
$decisions['ao_imperial_light_leather']       = @{ h=0; b=18; a=15; l=23; wt=2.1 }
$decisions['AR_empire_armor_o']               = @{ h=0; b=20; a=13; l=22; wt=2.5 }
$decisions['AR_empire_scout_armor_a']         = @{ h=0; b=22; a=13; l=22; wt=3.0 }
$decisions['AR_empire_armor_a']               = @{ h=0; b=22; a=14; l=22; wt=3.1 }
$decisions['AR_aserai_leather_a']             = @{ h=0; b=28; a=15; l=22; wt=5.0 }
$decisions['AR_empire_brass_lamellar_b']      = @{ h=0; b=32; a=15; l=25; wt=6.0 }
$decisions['AR_empire_brass_lamellar_d']      = @{ h=0; b=34; a=15; l=25; wt=6.0 }
$decisions['AR_empire_armor_r']               = @{ h=0; b=32; a=25; l=22; wt=9.0 }
$decisions['AR_empire_armor_s']               = @{ h=0; b=32; a=25; l=22; wt=9.0 }
$decisions['ao_imperial_lamellar_vest']       = @{ h=0; b=37; a=15; l=25; wt=7.0 }
$decisions['ao_imperial_lamellar_vest_b']     = @{ h=0; b=35; a=15; l=25; wt=7.0 }
$decisions['TV_empire_armor_l']               = @{ h=0; b=37; a=12; l=25; wt=7.0 }
$decisions['TV_empire_armor_l2']              = @{ h=0; b=35; a=12; l=25; wt=7.0 }
$decisions['AR_empire_scout_armor_d']         = @{ h=0; b=33; a=10; l=15; wt=6.0 }
$decisions['AR_huntress_armor_b']             = @{ h=0; b=32; a=10; l=15; wt=6.0 }
$decisions['AR_empire_horseman_armor_d']      = @{ h=0; b=37; a=15; l=22; wt=8.0 }
$decisions['AR_empire_scout_armor_b']         = @{ h=0; b=37; a=15; l=25; wt=7.0 }
$decisions['AR_empire_horseman_armor_c']      = @{ h=0; b=47; a=33; l=22; wt=8.6 }
$decisions['TV_empire_armor_p']               = @{ h=0; b=45; a=30; l=22; wt=9.0 }
$decisions['AR_empire_armor_c']               = @{ h=0; b=46; a=30; l=27; wt=10.2 }
$decisions['AR_empire_armor_j']               = @{ h=0; b=55; a=18; l=44; wt=22.0 }
$decisions['ao_imperial_lamellar_vest_over_mail']   = @{ h=0; b=55; a=38; l=34; wt=12.0 }
$decisions['ao_imperial_lamellar_vest_over_mail_b'] = @{ h=0; b=53; a=38; l=34; wt=12.0 }
$decisions['tv_empire_armor_b']               = @{ h=0; b=60; a=25; l=30; wt=15.0 }
$decisions['TV_empire_armor_i']               = @{ h=0; b=45; a=15; l=22; wt=5.0 }
$decisions['AR_empire_horseman_armor']        = @{ h=0; b=55; a=38; l=27; wt=9.5 }
$decisions['AR_empire_horseman_armor_b']      = @{ h=0; b=57; a=38; l=27; wt=9.5 }
$decisions['TV_empire_armor_f']               = @{ h=0; b=47; a=33; l=22; wt=14.0 }
$decisions['TV_empire_armor_g']               = @{ h=0; b=47; a=33; l=22; wt=14.0 }
$decisions['TV_empire_armor_h']               = @{ h=0; b=49; a=33; l=22; wt=14.0 }
$decisions['AR_empire_scale_armor_a']         = @{ h=0; b=50; a=33; l=25; wt=15.0 }
$decisions['AR_empire_scale_armor_b']         = @{ h=0; b=52; a=33; l=25; wt=15.0 }
$decisions['tv_empire_armor_a']               = @{ h=0; b=55; a=38; l=30; wt=13.0 }
$decisions['AR_empire_brass_lamellar_a']      = @{ h=0; b=55; a=38; l=27; wt=12.0 }
$decisions['AR_empire_brass_lamellar_c']      = @{ h=0; b=57; a=38; l=27; wt=12.0 }
$decisions['TV_empire_armor_j']               = @{ h=0; b=55; a=38; l=27; wt=10.0 }
$decisions['AR_empire_horseman_armor_a']      = @{ h=0; b=62; a=25; l=40; wt=13.0 }
$decisions['AR_empire_horseman_armor_a2']     = @{ h=0; b=65; a=25; l=40; wt=13.0 }
$decisions['ao_imperial_southern_scale_armor_over_mail']   = @{ h=0; b=79; a=45; l=78; wt=25.0 }
$decisions['ao_imperial_southern_scale_armor_over_mail_b'] = @{ h=0; b=82; a=45; l=78; wt=25.0 }
$decisions['DZ_empire_armor_d']               = @{ h=0; b=80; a=30; l=45; wt=20.0 }
$decisions['DZ_empire_armor_d2']              = @{ h=0; b=78; a=30; l=45; wt=20.0 }
$decisions['AR_empire_armor_h']               = @{ h=0; b=79; a=30; l=45; wt=16.0 }
$decisions['AR_empire_scale_armor_f']         = @{ h=0; b=82; a=30; l=45; wt=16.0 }
$decisions['AR_empire_scale_armor_g']         = @{ h=0; b=82; a=30; l=45; wt=16.0 }
$decisions['AR_empire_scale_armor_h']         = @{ h=0; b=82; a=30; l=45; wt=16.0 }
$decisions['AR_empire_armor_i']               = @{ h=0; b=88; a=35; l=45; wt=20.0 }
$decisions['AR_empire_scale_armor_c']         = @{ h=0; b=85; a=35; l=55; wt=20.0 }
$decisions['AR_empire_scale_armor_d']         = @{ h=0; b=83; a=35; l=55; wt=20.0 }
$decisions['TV_empire_armor_d']               = @{ h=0; b=85; a=45; l=78; wt=24.0 }
$decisions['TV_empire_armor_m']               = @{ h=0; b=85; a=35; l=55; wt=22.0 }

# ========== Empire · HandArmor (from agent A1) ==========
$decisions['AR_empire_gloves_e']              = @{ h=0; b=0; a=8;  l=0; wt=0.5 }
$decisions['AR_empire_gloves_a']              = @{ h=0; b=0; a=20; l=0; wt=0.6 }
$decisions['ao_imperial_decorated_bracers']   = @{ h=0; b=0; a=32; l=0; wt=0.8 }
$decisions['ao_imperial_decorated_bracers_b'] = @{ h=0; b=0; a=37; l=0; wt=0.9 }
$decisions['AO_durkhan_gauntlets_a']          = @{ h=0; b=0; a=42; l=0; wt=1.0 }
$decisions['AO_durkhan_gauntlets_b']          = @{ h=0; b=0; a=45; l=0; wt=1.0 }
$decisions['AR_empire_gloves_b']              = @{ h=0; b=0; a=50; l=0; wt=1.5 }
$decisions['AR_empire_gloves_c']              = @{ h=0; b=0; a=53; l=0; wt=1.5 }
$decisions['AR_empire_gloves_d']              = @{ h=0; b=0; a=63; l=0; wt=1.8 }
$decisions['tv_empire_plate_b']               = @{ h=0; b=0; a=50; l=0; wt=1.6 }
$decisions['hmj_plated_mail_mittens']         = @{ h=0; b=0; a=50; l=0; wt=1.6 }
$decisions['TV_sturgia_gloves_a']             = @{ h=0; b=0; a=42; l=0; wt=1.4 }

# ========== Empire · LegArmor (from agent A1) ==========
$decisions['TV_moccasins_a']                  = @{ h=0; b=0; a=0; l=3;  wt=0.4 }
$decisions['DZ_empire_boots_b']               = @{ h=0; b=0; a=0; l=8;  wt=0.5 }
$decisions['DZ_empire_boots_a']               = @{ h=0; b=0; a=0; l=12; wt=0.7 }
$decisions['TV_empire_boots_c']               = @{ h=0; b=0; a=0; l=24; wt=1.5 }
$decisions['AR_empire_boots_a']               = @{ h=0; b=0; a=0; l=32; wt=2.5 }
$decisions['AR_empire_boots_b']               = @{ h=0; b=0; a=0; l=35; wt=2.5 }
$decisions['tv_empire_plate_c']               = @{ h=0; b=0; a=0; l=42; wt=2.7 }
$decisions['AR_empire_boots_c']               = @{ h=0; b=0; a=0; l=62; wt=3.5 }
$decisions['TV_empire_boots_a']               = @{ h=0; b=0; a=0; l=44; wt=2.7 }
$decisions['TV_empire_boots_b']               = @{ h=0; b=0; a=0; l=42; wt=2.7 }
$decisions['tv_empire_plate_a']               = @{ h=0; b=0; a=0; l=50; wt=2.5 }

# ========== Vlandia · Cape A/B (from agent A2) ==========
$decisions['AR_vlandia_shoulders_c']          = @{ h=0; b=30; a=12; l=0; wt=2.0 }
$decisions['AR_vlandia_shoulders_b']          = @{ h=0; b=35; a=20; l=0; wt=3.5 }
$decisions['AR_vlandia_shoulders_a']          = @{ h=0; b=40; a=20; l=0; wt=3.8 }
$decisions['AR_vlandia_shoulders_d']          = @{ h=0; b=33; a=12; l=0; wt=2.7 }
$decisions['TV_vlandia_shoulders_d']          = @{ h=0; b=35; a=12; l=0; wt=3.0 }
$decisions['TV_vlandia_shoulders_e']          = @{ h=0; b=35; a=12; l=0; wt=3.0 }
$decisions['TV_vlandia_shoulders_f']          = @{ h=0; b=38; a=12; l=0; wt=3.5 }
$decisions['AR_vlandia_shoulders_f']          = @{ h=0; b=68; a=20; l=0; wt=3.5 }
$decisions['AR_vlandia_shoulders_e']          = @{ h=0; b=15; a=6;  l=0; wt=2.5 }
$decisions['TV_vlandia_shoulders_m']          = @{ h=0; b=14; a=5;  l=0; wt=2.0 }
$decisions['leather_shoulder_a']              = @{ h=0; b=15; a=6;  l=0; wt=2.5 }
$decisions['leather_shoulder_b']              = @{ h=0; b=20; a=8;  l=0; wt=3.0 }
$decisions['TV_vlandia_shoulders_a']          = @{ h=0; b=20; a=10; l=0; wt=2.7 }
$decisions['mercenary_padding_cape']          = @{ h=0; b=22; a=10; l=0; wt=2.8 }
$decisions['TV_vlandia_shoulders_c']          = @{ h=0; b=24; a=10; l=0; wt=2.8 }
$decisions['TV_vlandia_shoulders_q']          = @{ h=0; b=24; a=10; l=0; wt=2.8 }
$decisions['DZ_vlandia_shoulders_a']          = @{ h=0; b=22; a=10; l=0; wt=2.8 }
$decisions['DZ_vlandia_shoulders_b']          = @{ h=0; b=22; a=10; l=0; wt=2.8 }
$decisions['TV_vlandia_shoulders_n']          = @{ h=0; b=25; a=10; l=0; wt=2.7 }
$decisions['TV_vlandia_shoulders_o']          = @{ h=0; b=27; a=10; l=0; wt=3.0 }
$decisions['TV_vlandia_shoulders_p']          = @{ h=0; b=27; a=10; l=0; wt=3.0 }
$decisions['TV_vlandia_shoulders_r']          = @{ h=0; b=33; a=12; l=0; wt=3.0 }
$decisions['TV_vlandia_shoulders_s']          = @{ h=0; b=40; a=20; l=0; wt=3.8 }
$decisions['TV_vlandia_shoulders_t']          = @{ h=0; b=42; a=20; l=0; wt=3.8 }
$decisions['AR_imperial_lamellar_cape_k']     = @{ h=0; b=28; a=8;  l=0; wt=3.5 }
$decisions['TV_vlandia_shoulders_h']          = @{ h=0; b=5;  a=0;  l=0; wt=0.5 }
$decisions['TV_vlandia_shoulders_i']          = @{ h=0; b=6;  a=0;  l=0; wt=1.5 }
$decisions['TV_vlandia_shoulders_j']          = @{ h=0; b=8;  a=0;  l=0; wt=1.5 }
$decisions['AR_fur_cape_e']                   = @{ h=0; b=10; a=0;  l=0; wt=2.0 }
$decisions['TV_vlandia_shoulders_k']          = @{ h=0; b=10; a=0;  l=0; wt=2.0 }
$decisions['TV_vlandia_shoulders_l']          = @{ h=0; b=10; a=0;  l=0; wt=2.0 }
$decisions['tv_battania_cloak_k']             = @{ h=0; b=12; a=0;  l=0; wt=2.0 }
$decisions['TV_vlandia_shoulders_g']          = @{ h=0; b=15; a=0;  l=0; wt=1.4 }
$decisions['TV_vlandia_shoulders_b']          = @{ h=0; b=22; a=0;  l=0; wt=2.8 }
$decisions['TV_vlandia_shoulders_b2']         = @{ h=0; b=22; a=0;  l=0; wt=2.8 }
$decisions['AR_imperial_lamellar_cape_i']     = @{ h=0; b=45; a=0;  l=0; wt=4.0 }
$decisions['AR_imperial_lamellar_cape_h']     = @{ h=0; b=48; a=0;  l=0; wt=4.0 }
$decisions['AR_imperial_lamellar_cape_j']     = @{ h=0; b=45; a=0;  l=0; wt=4.0 }

# ========== Vlandia · BodyArmor A-O (from agent A2, L family skipped) ==========
$decisions['AR_vlandia_armor_i']              = @{ h=0; b=6;  a=5;  l=5;  wt=0.4 }
$decisions['TV_vlandia_armor_u']              = @{ h=0; b=6;  a=5;  l=5;  wt=0.4 }
$decisions['AR_monk_robe_a']                  = @{ h=0; b=6;  a=5;  l=6;  wt=0.5 }
$decisions['AR_monk_robe_a2']                 = @{ h=0; b=6;  a=5;  l=6;  wt=0.5 }
$decisions['AR_monk_robe_b']                  = @{ h=0; b=6;  a=5;  l=6;  wt=0.5 }
$decisions['TV_monk_robe_a']                  = @{ h=0; b=6;  a=5;  l=6;  wt=0.5 }
$decisions['TV_monk_robe_c']                  = @{ h=0; b=6;  a=5;  l=6;  wt=0.5 }
$decisions['DZ_vlandia_armor_a']              = @{ h=0; b=8;  a=6;  l=8;  wt=1.5 }
$decisions['AR_vlandia_armor_a2']             = @{ h=0; b=19; a=19; l=19; wt=2.4 }
$decisions['AR_vlandia_armor_j']              = @{ h=0; b=18; a=18; l=18; wt=2.4 }
$decisions['AR_vlandia_armor_k']              = @{ h=0; b=18; a=10; l=18; wt=2.2 }
$decisions['AR_vlandia_armor_l']              = @{ h=0; b=18; a=18; l=12; wt=2.2 }
$decisions['AR_vlandia_armor_m']              = @{ h=0; b=18; a=10; l=12; wt=2.0 }
$decisions['AR_vlandia_armor_t']              = @{ h=0; b=18; a=14; l=18; wt=2.0 }
$decisions['mercenary_gambeson']              = @{ h=0; b=19; a=19; l=19; wt=2.5 }
$decisions['ao_vlandian_tabard_over_aketon']  = @{ h=0; b=20; a=16; l=19; wt=3.0 }
$decisions['ao_vlandian_tabard_over_aketon_b']= @{ h=0; b=20; a=16; l=19; wt=3.0 }
$decisions['TV_vlandia_armor_e']              = @{ h=0; b=20; a=16; l=19; wt=3.0 }
$decisions['AO_vlandia_armor_c']              = @{ h=0; b=22; a=16; l=19; wt=3.2 }
$decisions['AR_vlandia_armor_n']              = @{ h=0; b=22; a=12; l=18; wt=3.0 }
$decisions['AR_vlandia_armor_q2']             = @{ h=0; b=24; a=18; l=22; wt=6.0 }
$decisions['AR_vlandia_armor_r']              = @{ h=0; b=24; a=18; l=22; wt=6.0 }
$decisions['ao_crude_mail_with_tunic']        = @{ h=0; b=28; a=16; l=25; wt=6.0 }
$decisions['AR_vlandia_armor_d']              = @{ h=0; b=41; a=36; l=33; wt=7.5 }
$decisions['AR_vlandia_armor_d2']             = @{ h=0; b=41; a=36; l=33; wt=7.5 }
$decisions['AR_empire_armor_l']               = @{ h=0; b=43; a=36; l=33; wt=7.5 }
$decisions['TV_vlandia_armor_h2']             = @{ h=0; b=40; a=40; l=44; wt=8.0 }
$decisions['mercenary_mail_armor']            = @{ h=0; b=37; a=13; l=33; wt=8.0 }
$decisions['TV_vlandia_armor_l']              = @{ h=0; b=37; a=13; l=33; wt=8.0 }
$decisions['TV_vlandia_armor_j']              = @{ h=0; b=41; a=36; l=33; wt=8.6 }
$decisions['TV_vlandia_armor_h']              = @{ h=0; b=42; a=40; l=44; wt=9.0 }
$decisions['AR_monk_robe_c']                  = @{ h=0; b=40; a=36; l=33; wt=9.0 }
$decisions['AR_monk_robe_c2']                 = @{ h=0; b=40; a=36; l=33; wt=9.0 }
$decisions['AR_monk_robe_d']                  = @{ h=0; b=40; a=36; l=33; wt=9.0 }
$decisions['TV_monk_robe_b']                  = @{ h=0; b=40; a=36; l=33; wt=9.0 }
$decisions['TV_monk_robe_d']                  = @{ h=0; b=40; a=36; l=33; wt=9.0 }
$decisions['AP_leather_tabard_d']             = @{ h=0; b=43; a=36; l=33; wt=10.0 }
$decisions['leather_tabard_over_mail']        = @{ h=0; b=45; a=36; l=38; wt=10.0 }
$decisions['TV_vlandia_armor_b2']             = @{ h=0; b=45; a=36; l=38; wt=10.0 }
$decisions['TV_vlandia_armor_b4']             = @{ h=0; b=43; a=36; l=33; wt=10.0 }
$decisions['AR_vlandia_armor_f']              = @{ h=0; b=45; a=42; l=38; wt=9.0 }
$decisions['AP_leather_tabard_a']             = @{ h=0; b=45; a=42; l=38; wt=10.0 }
$decisions['AP_leather_tabard_c']             = @{ h=0; b=43; a=42; l=33; wt=10.0 }
$decisions['TV_vlandia_armor_b']              = @{ h=0; b=45; a=42; l=38; wt=10.0 }
$decisions['TV_vlandia_armor_b3']             = @{ h=0; b=43; a=42; l=33; wt=10.0 }
$decisions['AR_vlandia_armor_c']              = @{ h=0; b=45; a=42; l=38; wt=9.0 }
$decisions['TV_vlandia_armor_a']              = @{ h=0; b=45; a=42; l=38; wt=10.0 }
$decisions['TV_vlandia_armor_c']              = @{ h=0; b=45; a=42; l=38; wt=9.6 }
$decisions['TV_vlandia_armor_i']              = @{ h=0; b=45; a=42; l=38; wt=10.0 }
$decisions['TV_vlandia_armor_i2']             = @{ h=0; b=45; a=42; l=38; wt=10.0 }
$decisions['TV_vlandia_armor_o']              = @{ h=0; b=48; a=42; l=38; wt=9.0 }
$decisions['LE_vlandia_armor_a']              = @{ h=0; b=45; a=42; l=38; wt=9.0 }
$decisions['LE_vlandia_armor_b']              = @{ h=0; b=45; a=42; l=38; wt=9.0 }
$decisions['AR_vlandia_armor_p']              = @{ h=0; b=52; a=27; l=27; wt=10.0 }
$decisions['AR_vlandia_armor_p2']             = @{ h=0; b=50; a=27; l=27; wt=10.0 }
$decisions['AR_vlandia_armor_o']              = @{ h=0; b=45; a=36; l=33; wt=10.0 }
$decisions['TV_vlandia_armor_f']              = @{ h=0; b=50; a=42; l=38; wt=10.0 }
$decisions['AR_vlandia_armor_e']              = @{ h=0; b=50; a=42; l=38; wt=8.0 }
$decisions['AR_vlandia_leather_a']            = @{ h=0; b=52; a=42; l=38; wt=12.0 }
$decisions['TV_vlandia_armor_k']              = @{ h=0; b=52; a=42; l=38; wt=12.0 }
$decisions['TV_vlandia_armor_m']              = @{ h=0; b=52; a=42; l=38; wt=12.0 }
$decisions['TV_vlandia_armor_n']              = @{ h=0; b=52; a=42; l=38; wt=12.0 }
$decisions['AR_vlandia_armor_q']              = @{ h=0; b=50; a=42; l=38; wt=10.0 }
$decisions['AR_vlandia_armor_r2']             = @{ h=0; b=50; a=42; l=38; wt=10.0 }
$decisions['TV_empire_armor_e']               = @{ h=0; b=65; a=42; l=38; wt=9.0 }
$decisions['TV_empire_armor_e2']              = @{ h=0; b=63; a=42; l=38; wt=9.0 }
$decisions['TV_vlandia_armor_d']              = @{ h=0; b=65; a=42; l=38; wt=10.0 }
$decisions['TV_vlandia_armor_d2']             = @{ h=0; b=63; a=42; l=38; wt=10.0 }
$decisions['AR_empire_scale_armor_e2']        = @{ h=0; b=65; a=36; l=33; wt=10.0 }
$decisions['AR_vlandia_armor_h2']             = @{ h=0; b=68; a=36; l=33; wt=10.0 }
$decisions['AR_empire_scale_armor_e']         = @{ h=0; b=68; a=42; l=38; wt=12.0 }
$decisions['AR_empire_scale_armor_e3']        = @{ h=0; b=72; a=44; l=44; wt=12.0 }
$decisions['TV_vlandia_armor_p']              = @{ h=0; b=75; a=60; l=56; wt=14.0 }
$decisions['TV_vlandia_armor_q']              = @{ h=0; b=77; a=60; l=56; wt=14.0 }
$decisions['TV_vlandia_armor_s']              = @{ h=0; b=77; a=60; l=56; wt=14.0 }
$decisions['TV_vlandia_armor_s2']             = @{ h=0; b=75; a=60; l=56; wt=14.0 }
$decisions['AR_vlandia_scale_b']              = @{ h=0; b=75; a=60; l=56; wt=13.0 }
$decisions['AR_vlandia_armor_h']              = @{ h=0; b=82; a=49; l=44; wt=13.0 }
$decisions['AR_vlandia_armor_s']              = @{ h=0; b=78; a=60; l=56; wt=14.0 }
$decisions['AR_vlandia_armor_s2']             = @{ h=0; b=76; a=60; l=56; wt=14.0 }
$decisions['AR_vlandia_scale_a']              = @{ h=0; b=80; a=60; l=56; wt=15.0 }
$decisions['TV_empire_armor_n']               = @{ h=0; b=82; a=60; l=56; wt=16.0 }
$decisions['TV_empire_armor_o']               = @{ h=0; b=77; a=44; l=56; wt=16.0 }
$decisions['TV_vlandia_armor_t']              = @{ h=0; b=82; a=60; l=56; wt=15.0 }
$decisions['TV_vlandia_armor_t2']             = @{ h=0; b=80; a=60; l=56; wt=15.0 }
$decisions['AR_vlandia_armor_a']              = @{ h=0; b=90; a=80; l=60; wt=16.0 }
$decisions['TV_vlandia_armor_r']              = @{ h=0; b=90; a=80; l=60; wt=16.0 }
$decisions['TV_vlandia_armor_g']              = @{ h=0; b=95; a=85; l=70; wt=18.0 }
$decisions['AR_vlandia_armor_b']              = @{ h=0; b=100; a=100; l=95; wt=22.0 }
$decisions['AR_vlandia_armor_u']              = @{ h=0; b=100; a=80; l=70; wt=20.0 }

# ========== Vlandia · HandArmor 9 (manual extract line 3893-3901) ==========
$decisions['TV_vlandia_gloves_c']             = @{ h=0; b=0; a=20; l=0; wt=0.6 }
$decisions['TV_vlandia_gloves_d']             = @{ h=0; b=0; a=22; l=0; wt=0.5 }
$decisions['TV_vlandia_gloves_e']             = @{ h=0; b=0; a=22; l=0; wt=0.5 }
$decisions['TV_vlandia_gloves_f']             = @{ h=0; b=0; a=22; l=0; wt=0.5 }
$decisions['TV_vlandia_gloves_a']             = @{ h=0; b=0; a=30; l=0; wt=1.0 }
$decisions['TV_vlandia_gloves_g']             = @{ h=0; b=0; a=45; l=0; wt=1.4 }
$decisions['TV_vlandia_gloves_b']             = @{ h=0; b=0; a=48; l=0; wt=1.4 }
$decisions['TV_vlandia_gloves_i']             = @{ h=0; b=0; a=40; l=0; wt=1.0 }
$decisions['TV_vlandia_gloves_h']             = @{ h=0; b=0; a=50; l=0; wt=1.5 }

# ========== Vlandia · LegArmor 7 (manual extract line 3919-3925) ==========
$decisions['AR_vlandia_boots_a']              = @{ h=0; b=0; a=0; l=32; wt=0.9 }
$decisions['DZ_vlandia_boots_a']              = @{ h=0; b=0; a=0; l=43; wt=1.8 }
$decisions['TV_vlandia_boots_b']              = @{ h=0; b=0; a=0; l=40; wt=1.6 }
$decisions['AR_vlandia_boots_b']              = @{ h=0; b=0; a=0; l=38; wt=2.0 }
$decisions['AR_vlandia_boots_c']              = @{ h=0; b=0; a=0; l=38; wt=2.0 }
$decisions['TV_vlandia_boots_a']              = @{ h=0; b=0; a=0; l=43; wt=2.5 }
$decisions['TV_vlandia_boots_c']              = @{ h=0; b=0; a=0; l=43; wt=2.8 }

# ========== Battania · Cape A/B (from agent A3) ==========
$decisions['AR_battanian_shoulder_e']         = @{ h=0; b=14; a=6;  l=0; wt=1.0 }
$decisions['AR_battanian_shoulder_f']         = @{ h=0; b=16; a=6;  l=0; wt=1.3 }
$decisions['ao_light_fur_shoulder']           = @{ h=0; b=15; a=6;  l=0; wt=1.4 }
$decisions['AR_battanian_shoulder_g']         = @{ h=0; b=16; a=6;  l=0; wt=1.3 }
$decisions['AR_battanian_shoulder_h']         = @{ h=0; b=20; a=8;  l=0; wt=1.5 }
$decisions['AR_battanian_shoulder_b']         = @{ h=0; b=12; a=6;  l=0; wt=1.4 }
$decisions['AR_battanian_shoulder_c']         = @{ h=0; b=15; a=6;  l=0; wt=1.5 }
$decisions['AR_battanian_shoulder_d']         = @{ h=0; b=15; a=6;  l=0; wt=1.5 }
$decisions['HMJ_battanian_leather_scarf']     = @{ h=0; b=15; a=6;  l=0; wt=1.5 }
$decisions['AR_empire_shoulders_a']           = @{ h=0; b=12; a=6;  l=0; wt=1.2 }
$decisions['TV_wolf_pelt_d']                  = @{ h=0; b=22; a=8;  l=0; wt=2.0 }
$decisions['TV_wolf_pelt_g']                  = @{ h=0; b=22; a=8;  l=0; wt=2.0 }
$decisions['TV_wolf_pelt_f']                  = @{ h=0; b=26; a=10; l=0; wt=2.5 }
$decisions['TV_battania_shoulders_d']         = @{ h=0; b=24; a=10; l=0; wt=2.5 }
$decisions['ao_battanian_mail_shoulders']     = @{ h=0; b=27; a=10; l=0; wt=3.5 }
$decisions['hmj_battanian_shoulders_a']       = @{ h=0; b=28; a=10; l=0; wt=3.5 }
$decisions['battanian_mail_scarf']            = @{ h=0; b=28; a=10; l=0; wt=3.5 }
$decisions['hmj_battanian_shoulders_c']       = @{ h=0; b=30; a=10; l=0; wt=3.5 }
$decisions['hmj_battanian_shoulders_d']       = @{ h=0; b=30; a=10; l=0; wt=3.5 }
$decisions['hmj_battanian_shoulders_e']       = @{ h=0; b=32; a=10; l=0; wt=3.5 }
$decisions['tv_battania_cloak_h']             = @{ h=0; b=28; a=10; l=0; wt=3.5 }
$decisions['tv_battania_cloak_i']             = @{ h=0; b=28; a=10; l=0; wt=3.5 }
$decisions['tv_battania_cloak_j']             = @{ h=0; b=28; a=10; l=0; wt=3.5 }
$decisions['AR_battanian_shoulder_a']         = @{ h=0; b=30; a=12; l=0; wt=3.5 }
$decisions['tv_battania_cloak_c']             = @{ h=0; b=32; a=12; l=0; wt=3.5 }
$decisions['tv_battania_cloak_d']             = @{ h=0; b=32; a=12; l=0; wt=3.5 }
$decisions['tv_battania_cloak_e']             = @{ h=0; b=32; a=12; l=0; wt=3.5 }
$decisions['tv_battania_cloak_r']             = @{ h=0; b=32; a=12; l=0; wt=3.5 }
$decisions['tv_battania_cloak_r2']            = @{ h=0; b=32; a=12; l=0; wt=3.5 }
$decisions['tv_battania_cloak_r3']            = @{ h=0; b=32; a=12; l=0; wt=3.5 }
$decisions['tv_battania_cloak_s']             = @{ h=0; b=34; a=12; l=0; wt=3.5 }
$decisions['tv_battania_cloak_s2']            = @{ h=0; b=34; a=12; l=0; wt=3.5 }
$decisions['tv_battania_cloak_s3']            = @{ h=0; b=34; a=12; l=0; wt=3.5 }
$decisions['TV_battania_warlord_pauldrons_b'] = @{ h=0; b=42; a=20; l=0; wt=4.0 }
$decisions['TV_battania_warlord_pauldrons_c'] = @{ h=0; b=45; a=20; l=0; wt=4.0 }
$decisions['AR_wolf_shoulder_h']              = @{ h=0; b=45; a=20; l=0; wt=4.0 }
$decisions['AR_wolf_shoulder_h2']             = @{ h=0; b=45; a=20; l=0; wt=4.0 }
$decisions['AR_wolf_shoulder_i']              = @{ h=0; b=43; a=20; l=0; wt=4.0 }
$decisions['AR_wolf_shoulder_i2']             = @{ h=0; b=43; a=20; l=0; wt=4.0 }
$decisions['hmj_battanian_shoulders_b']       = @{ h=0; b=6;  a=2;  l=0; wt=0.3 }
$decisions['ao_battanian_medallion']          = @{ h=0; b=3;  a=0;  l=0; wt=0.2 }
$decisions['ao_caladogs_medallion']           = @{ h=0; b=5;  a=0;  l=0; wt=0.2 }
$decisions['ao_generic_short_cape']           = @{ h=0; b=6;  a=0;  l=0; wt=0.5 }
$decisions['TV_battania_cloak_l']             = @{ h=0; b=6;  a=0;  l=0; wt=0.5 }
$decisions['TV_battania_cloak_l2']            = @{ h=0; b=6;  a=0;  l=0; wt=0.5 }
$decisions['TV_battania_cloak_l3']            = @{ h=0; b=6;  a=0;  l=0; wt=0.5 }
$decisions['TV_battania_cloak_p']             = @{ h=0; b=8;  a=0;  l=0; wt=1.5 }
$decisions['TV_battania_cloak_p2']            = @{ h=0; b=8;  a=0;  l=0; wt=1.5 }
$decisions['TV_battania_cloak_p3']            = @{ h=0; b=8;  a=0;  l=0; wt=1.5 }
$decisions['TV_battania_cloak_q']             = @{ h=0; b=8;  a=0;  l=0; wt=1.5 }
$decisions['TV_battania_cloak_q2']            = @{ h=0; b=8;  a=0;  l=0; wt=1.5 }
$decisions['TV_battania_cloak_q3']            = @{ h=0; b=8;  a=0;  l=0; wt=1.5 }
$decisions['TV_wolf_pelt_a']                  = @{ h=0; b=22; a=0;  l=0; wt=2.0 }
$decisions['TV_wolf_pelt_b']                  = @{ h=0; b=22; a=0;  l=0; wt=2.0 }
$decisions['AR_fur_cape_a']                   = @{ h=0; b=20; a=0;  l=0; wt=2.5 }
$decisions['AR_fur_cape_b']                   = @{ h=0; b=20; a=0;  l=0; wt=2.5 }
$decisions['AR_fur_cape_c']                   = @{ h=0; b=20; a=0;  l=0; wt=2.5 }
$decisions['AR_fur_cape_d']                   = @{ h=0; b=20; a=0;  l=0; wt=2.5 }
$decisions['AR_wolf_head_cloak_a']            = @{ h=0; b=24; a=0;  l=0; wt=3.0 }
$decisions['AR_wolf_head_cloak_b']            = @{ h=0; b=24; a=0;  l=0; wt=3.0 }
$decisions['TV_wolf_pelt_h']                  = @{ h=0; b=24; a=0;  l=0; wt=3.0 }
$decisions['tv_battania_cloak_a']             = @{ h=0; b=14; a=0;  l=0; wt=2.0 }
$decisions['tv_battania_cloak_b']             = @{ h=0; b=14; a=0;  l=0; wt=2.0 }
$decisions['tv_battania_cloak_f']             = @{ h=0; b=14; a=0;  l=0; wt=2.0 }
$decisions['tv_battania_cloak_g']             = @{ h=0; b=16; a=0;  l=0; wt=2.0 }

# ========== Battania · BodyArmor A-L (from agent A3) ==========
$decisions['AR_battania_armor_c']             = @{ h=0; b=0;  a=0;  l=15; wt=0.5 }
$decisions['AR_battania_armor_d']             = @{ h=0; b=0;  a=0;  l=15; wt=0.5 }
$decisions['AR_battania_armor_e']             = @{ h=0; b=0;  a=0;  l=15; wt=0.5 }
$decisions['ao_battanian_cloth_tunic_with_kilt']  = @{ h=0; b=7;  a=5;  l=7;  wt=0.5 }
$decisions['ao_battanian_woolen_tunic_with_kilt'] = @{ h=0; b=7;  a=5;  l=7;  wt=0.5 }
$decisions['AR_battania_armor_k']             = @{ h=0; b=8;  a=6;  l=8;  wt=0.6 }
$decisions['AR_battania_armor_g']             = @{ h=0; b=13; a=5;  l=11; wt=1.5 }
$decisions['ao_battanian_leather_tunic_with_kilt'] = @{ h=0; b=14; a=9;  l=12; wt=1.8 }
$decisions['AR_battania_armor_h']             = @{ h=0; b=14; a=8;  l=12; wt=1.5 }
$decisions['ao_battanian_gambeson_with_kilt'] = @{ h=0; b=19; a=11; l=19; wt=2.0 }
$decisions['tv_battania_armor_e']             = @{ h=0; b=20; a=14; l=22; wt=1.5 }
$decisions['ao_battanian_scale_armor']        = @{ h=0; b=34; a=0;  l=20; wt=10.7 }
$decisions['tv_battania_armor_a']             = @{ h=0; b=34; a=0;  l=20; wt=10.7 }
$decisions['AR_battania_armor_i']             = @{ h=0; b=36; a=16; l=22; wt=8.0 }
$decisions['AR_battania_armor_j']             = @{ h=0; b=40; a=16; l=25; wt=8.5 }
$decisions['AR_battania_armor_l']             = @{ h=0; b=36; a=16; l=22; wt=8.0 }
$decisions['AR_battania_armor_f']             = @{ h=0; b=40; a=16; l=25; wt=9.5 }
$decisions['ao_battanian_mail_shirt_with_kilt']    = @{ h=0; b=36; a=16; l=22; wt=8.0 }
$decisions['ao_battanian_hauberk_with_kilt']  = @{ h=0; b=40; a=16; l=25; wt=9.5 }
$decisions['AR_battania_armor_p']             = @{ h=0; b=50; a=38; l=38; wt=10.0 }
$decisions['AR_battania_armor_p2']            = @{ h=0; b=50; a=38; l=38; wt=10.0 }
$decisions['AR_battania_armor_p3']            = @{ h=0; b=50; a=38; l=38; wt=10.0 }
$decisions['AR_battania_armor_o']             = @{ h=0; b=60; a=15; l=30; wt=10.0 }
$decisions['AR_battania_armor_o2']            = @{ h=0; b=60; a=15; l=30; wt=10.0 }
$decisions['tv_battania_armor_f']             = @{ h=0; b=50; a=38; l=38; wt=10.7 }
$decisions['AR_battania_armor_q']             = @{ h=0; b=45; a=38; l=27; wt=9.5 }
$decisions['AR_battania_armor_q2']            = @{ h=0; b=45; a=38; l=27; wt=9.5 }
$decisions['AR_battania_armor_q3']            = @{ h=0; b=45; a=38; l=27; wt=9.5 }
$decisions['AR_battania_armor_a']             = @{ h=0; b=50; a=38; l=38; wt=13.0 }
$decisions['AR_battania_armor_m']             = @{ h=0; b=55; a=33; l=27; wt=14.0 }
$decisions['AR_battania_armor_m2']            = @{ h=0; b=57; a=33; l=27; wt=14.0 }
$decisions['tv_battania_warlord_armor_b']     = @{ h=0; b=80; a=35; l=45; wt=22.0 }
$decisions['tv_battania_warlord_armor_c']     = @{ h=0; b=80; a=35; l=45; wt=22.0 }
$decisions['tv_battania_warlord_armor_a']     = @{ h=0; b=85; a=38; l=48; wt=20.0 }
$decisions['tv_battania_warlord_armor_b2']    = @{ h=0; b=92; a=40; l=48; wt=22.0 }
$decisions['tv_battania_warlord_armor_c2']    = @{ h=0; b=92; a=40; l=48; wt=22.0 }
$decisions['AR_battania_armor_b']             = @{ h=0; b=96; a=40; l=50; wt=22.0 }
$decisions['ao_battanian_decorated_scale_armor']   = @{ h=0; b=66; a=33; l=33; wt=16.0 }
$decisions['ao_battanian_decorated_scale_armor_b'] = @{ h=0; b=66; a=33; l=33; wt=16.0 }
$decisions['ao_battanian_decorated_scale_armor_c'] = @{ h=0; b=66; a=33; l=33; wt=16.0 }
$decisions['tv_battania_armor_b']             = @{ h=0; b=47; a=18; l=15; wt=12.0 }
$decisions['tv_battania_armor_c']             = @{ h=0; b=48; a=12; l=48; wt=10.8 }
$decisions['ao_eorling_plates_over_mail']     = @{ h=0; b=50; a=38; l=38; wt=14.0 }
$decisions['AR_battania_armor_n']             = @{ h=0; b=55; a=33; l=27; wt=16.0 }
$decisions['AR_battania_armor_n2']            = @{ h=0; b=57; a=33; l=27; wt=16.0 }
$decisions['AR_battania_armor_a2']            = @{ h=0; b=70; a=20; l=20; wt=14.0 }
$decisions['AR_battania_armor_b2']            = @{ h=0; b=79; a=25; l=35; wt=16.0 }

# ========== Battania · HandArmor 10 (manual extract line 4647-4653) ==========
$decisions['TV_battania_gloves_a']            = @{ h=0; b=0; a=30; l=0; wt=1.0 }
$decisions['TV_battania_gloves_d']            = @{ h=0; b=0; a=39; l=0; wt=0.6 }
$decisions['AR_pict_glove_a']                 = @{ h=0; b=0; a=30; l=0; wt=1.2 }
$decisions['AR_brass_scale_gloves']           = @{ h=0; b=0; a=35; l=0; wt=1.3 }
$decisions['AR_pict_glove_b']                 = @{ h=0; b=0; a=35; l=0; wt=1.3 }
$decisions['TV_battania_gloves_b']            = @{ h=0; b=0; a=30; l=0; wt=1.2 }
$decisions['TV_battania_gloves_c']            = @{ h=0; b=0; a=32; l=0; wt=1.2 }
$decisions['tv_battania_warlord_bracers']     = @{ h=0; b=0; a=50; l=0; wt=1.5 }
$decisions['TV_battania_warlord_bracers_b']   = @{ h=0; b=0; a=50; l=0; wt=1.5 }
$decisions['TV_battania_warlord_bracers_c']   = @{ h=0; b=0; a=50; l=0; wt=1.5 }

# ========== Battania · LegArmor 27 (manual extract line 4667-4673) ==========
$decisions['ao_leather_shoes']                = @{ h=0; b=0; a=0; l=6;  wt=0.4 }
$decisions['simple_shoes']                    = @{ h=0; b=0; a=0; l=6;  wt=0.4 }
$decisions['TV_battania_boots_w']             = @{ h=0; b=0; a=0; l=6;  wt=0.4 }
$decisions['TV_battania_boots_a']             = @{ h=0; b=0; a=0; l=12; wt=0.7 }
$decisions['TV_battania_boots_b']             = @{ h=0; b=0; a=0; l=12; wt=0.7 }
$decisions['TV_battania_boots_c']             = @{ h=0; b=0; a=0; l=12; wt=0.7 }
$decisions['TV_battania_boots_d']             = @{ h=0; b=0; a=0; l=12; wt=0.7 }
$decisions['TV_battania_boots_s']             = @{ h=0; b=0; a=0; l=12; wt=0.7 }
$decisions['TV_battania_boots_t']             = @{ h=0; b=0; a=0; l=12; wt=0.7 }
$decisions['TV_battania_boots_u']             = @{ h=0; b=0; a=0; l=12; wt=0.7 }
$decisions['TV_battania_boots_v']             = @{ h=0; b=0; a=0; l=12; wt=0.7 }
$decisions['TV_battania_boots_g']             = @{ h=0; b=0; a=0; l=25; wt=1.0 }
$decisions['TV_battania_boots_h']             = @{ h=0; b=0; a=0; l=25; wt=1.0 }
$decisions['TV_battania_boots_i']             = @{ h=0; b=0; a=0; l=25; wt=1.0 }
$decisions['TV_battania_boots_j']             = @{ h=0; b=0; a=0; l=25; wt=1.0 }
$decisions['DZ_battania_boots_a']             = @{ h=0; b=0; a=0; l=30; wt=1.5 }
$decisions['TV_battania_boots_k']             = @{ h=0; b=0; a=0; l=35; wt=1.5 }
$decisions['TV_battania_boots_l']             = @{ h=0; b=0; a=0; l=35; wt=1.5 }
$decisions['TV_battania_boots_m']             = @{ h=0; b=0; a=0; l=35; wt=1.5 }
$decisions['TV_battania_boots_n']             = @{ h=0; b=0; a=0; l=35; wt=1.5 }
$decisions['TV_battania_boots_o']             = @{ h=0; b=0; a=0; l=35; wt=1.5 }
$decisions['TV_battania_boots_p']             = @{ h=0; b=0; a=0; l=35; wt=1.5 }
$decisions['TV_battania_boots_q']             = @{ h=0; b=0; a=0; l=35; wt=1.5 }
$decisions['TV_battania_boots_r']             = @{ h=0; b=0; a=0; l=35; wt=1.5 }
$decisions['tv_battania_warlord_boots']       = @{ h=0; b=0; a=0; l=46; wt=2.6 }
$decisions['tv_battania_warlord_boots_b']     = @{ h=0; b=0; a=0; l=46; wt=2.6 }
$decisions['tv_battania_warlord_boots_c']     = @{ h=0; b=0; a=0; l=46; wt=2.6 }

# ========== Sturgia · HeadArmor A-N (from agent A4) ==========
$decisions['AR_sturgia_womens_hat_a']         = @{ h=6;  b=0;  a=0;  l=0; wt=0.3 }
$decisions['sloven_hat']                      = @{ h=10; b=0;  a=0;  l=0; wt=0.4 }
$decisions['sloven_hat_a']                    = @{ h=12; b=0;  a=0;  l=0; wt=0.5 }
$decisions['AR_sturgia_helmet_f']             = @{ h=50; b=0;  a=0;  l=0; wt=1.5 }
$decisions['AR_sturgia_helmet_g']             = @{ h=50; b=0;  a=0;  l=0; wt=1.5 }
$decisions['AR_sturgia_helmet_g2']            = @{ h=50; b=0;  a=0;  l=0; wt=1.5 }
$decisions['AR_sturgia_helmet_h']             = @{ h=61; b=3;  a=15; l=0; wt=1.5 }
$decisions['AR_sturgia_helmet_h2']            = @{ h=61; b=3;  a=15; l=0; wt=1.5 }
$decisions['AR_sturgia_helmet_i']             = @{ h=67; b=0;  a=20; l=0; wt=1.5 }
$decisions['AR_sturgia_helmet_i2']            = @{ h=67; b=0;  a=20; l=0; wt=1.5 }
$decisions['sloven_helmet']                   = @{ h=50; b=0;  a=0;  l=0; wt=1.5 }
$decisions['AR_sturgia_helmet_base_b']        = @{ h=50; b=0;  a=0;  l=0; wt=2.0 }
$decisions['AR_sturgia_helmet_base']          = @{ h=55; b=0;  a=0;  l=0; wt=2.0 }
$decisions['AR_vaegir_helmet_b']              = @{ h=50; b=12; a=0;  l=0; wt=1.8 }
$decisions['AR_vaegir_helmet_b2']             = @{ h=50; b=12; a=0;  l=0; wt=1.8 }
$decisions['sturgian_nasal_helmet_fur']       = @{ h=50; b=12; a=0;  l=0; wt=1.8 }
$decisions['TV_nord_helmet_t']                = @{ h=55; b=0;  a=0;  l=0; wt=2.0 }
$decisions['AR_sturgia_helmet_k']             = @{ h=63; b=12; a=25; l=0; wt=1.8 }
$decisions['AR_sturgia_helmet_a']             = @{ h=61; b=3;  a=15; l=0; wt=1.5 }
$decisions['AR_sturgia_helmet_d']             = @{ h=65; b=12; a=20; l=0; wt=1.8 }
$decisions['AR_sturgia_helmet_b']             = @{ h=75; b=12; a=17; l=0; wt=2.5 }
$decisions['hmj_forpolkka_sturgia_gopniki_helm'] = @{ h=65; b=12; a=25; l=0; wt=1.8 }
$decisions['TV_nord_helmet_t2']               = @{ h=75; b=12; a=17; l=0; wt=2.5 }
$decisions['AR_sturgia_helmet_l']             = @{ h=65; b=12; a=0;  l=0; wt=1.8 }
$decisions['AR_sturgia_helmet_l2']            = @{ h=75; b=12; a=17; l=0; wt=1.8 }
$decisions['AR_sturgia_helmet_m']             = @{ h=85; b=12; a=17; l=0; wt=3.5 }
$decisions['mailed_cavalry_helm']             = @{ h=86; b=40; a=40; l=0; wt=3.5 }
$decisions['AR_sturgia_helmet_c']             = @{ h=104;b=64; a=40; l=0; wt=3.8 }
$decisions['TV_nord_helmet_i']                = @{ h=63; b=12; a=25; l=0; wt=2.5 }
$decisions['ao_sturgian_spangenhelm_with_leather']         = @{ h=67; b=0;  a=20; l=0; wt=1.5 }
$decisions['ao_eorling_guarded_nasal_helmet_with_leather'] = @{ h=63; b=12; a=25; l=0; wt=2.0 }
$decisions['ao_sturgian_helmet_with_leather'] = @{ h=63; b=12; a=25; l=0; wt=2.0 }
$decisions['AR_sturgia_helmet_e']             = @{ h=75; b=12; a=17; l=0; wt=2.5 }
$decisions['AR_sturgia_helmet_j']             = @{ h=75; b=12; a=17; l=0; wt=2.5 }
$decisions['AR_sturgia_helmet_j2']            = @{ h=75; b=12; a=17; l=0; wt=2.5 }
$decisions['TV_nord_helmet_j']                = @{ h=80; b=12; a=17; l=0; wt=3.0 }
$decisions['ao_eorling_guarded_nasal_helmet_with_mail']    = @{ h=85; b=12; a=17; l=0; wt=3.0 }
$decisions['TV_nord_helmet_u']                = @{ h=60; b=12; a=17; l=0; wt=2.8 }
$decisions['TV_nord_helmet_u2']               = @{ h=75; b=12; a=17; l=0; wt=2.8 }
$decisions['TV_nord_helmet_u3']               = @{ h=95; b=50; a=20; l=0; wt=3.0 }
$decisions['TV_nord_helmet_v']                = @{ h=60; b=12; a=17; l=0; wt=2.8 }
$decisions['TV_nord_helmet_v2']               = @{ h=75; b=12; a=17; l=0; wt=2.8 }
$decisions['TV_nord_helmet_v3']               = @{ h=88; b=12; a=20; l=0; wt=2.8 }
$decisions['TV_nord_helmet_w']                = @{ h=65; b=12; a=20; l=0; wt=2.8 }
$decisions['TV_nord_helmet_w2']               = @{ h=75; b=12; a=20; l=0; wt=2.8 }
$decisions['TV_nord_helmet_w3']               = @{ h=89; b=12; a=20; l=0; wt=2.8 }
$decisions['AR_vaegir_helmet_e']              = @{ h=80; b=12; a=17; l=0; wt=3.5 }
$decisions['AR_vaegir_helmet_f']              = @{ h=80; b=12; a=17; l=0; wt=3.5 }
$decisions['AR_vaegir_helmet_g']              = @{ h=95; b=50; a=20; l=0; wt=3.8 }
$decisions['battanian_decorated_spangenhelm'] = @{ h=89; b=24; a=20; l=0; wt=3.0 }
$decisions['TV_nord_helmet_r']                = @{ h=95; b=50; a=20; l=0; wt=3.0 }
$decisions['TV_nord_helmet_a']                = @{ h=85; b=12; a=0;  l=0; wt=3.0 }
$decisions['TV_nord_helmet_b']                = @{ h=95; b=12; a=17; l=0; wt=3.0 }
$decisions['TV_nord_helmet_c']                = @{ h=105;b=24; a=20; l=0; wt=3.4 }
$decisions['TV_nord_helmet_d']                = @{ h=113;b=36; a=12; l=0; wt=3.8 }
$decisions['TV_nord_helmet_e']                = @{ h=86; b=40; a=40; l=0; wt=3.5 }
$decisions['TV_nord_helmet_f']                = @{ h=95; b=40; a=40; l=0; wt=3.5 }
$decisions['TV_nord_helmet_g']                = @{ h=104;b=64; a=40; l=0; wt=3.8 }
$decisions['TV_nord_helmet_h']                = @{ h=114;b=74; a=40; l=0; wt=4.2 }
$decisions['TV_nord_helmet_m']                = @{ h=89; b=24; a=20; l=0; wt=3.0 }
$decisions['TV_nord_helmet_n']                = @{ h=106;b=24; a=20; l=0; wt=3.2 }
$decisions['TV_nord_helmet_o']                = @{ h=115;b=36; a=20; l=0; wt=3.6 }
$decisions['TV_nord_helmet_p']                = @{ h=119;b=24; a=20; l=0; wt=4.0 }
$decisions['TV_sturgia_helmet_i']             = @{ h=83; b=12; a=0;  l=0; wt=2.8 }
$decisions['TV_sturgia_helmet_j']             = @{ h=85; b=12; a=17; l=0; wt=2.8 }
$decisions['TV_sturgia_helmet_j2']            = @{ h=87; b=12; a=17; l=0; wt=2.8 }
$decisions['TV_sturgia_helmet_k']             = @{ h=95; b=12; a=17; l=0; wt=2.8 }
$decisions['TV_sturgia_helmet_k2']            = @{ h=98; b=14; a=0;  l=0; wt=2.8 }
$decisions['TV_sturgia_helmet_c']             = @{ h=107;b=82; a=30; l=0; wt=3.0 }
$decisions['TV_sturgia_helmet_e']             = @{ h=107;b=82; a=30; l=0; wt=3.0 }
$decisions['TV_sturgia_helmet_g']             = @{ h=113;b=36; a=12; l=0; wt=3.2 }
$decisions['TV_sturgia_helmet_h']             = @{ h=113;b=36; a=20; l=0; wt=3.2 }
$decisions['TV_sturgia_helmet_f']             = @{ h=118;b=82; a=30; l=0; wt=3.5 }
$decisions['ao_sturgian_feathered_helmet']              = @{ h=83; b=12; a=0;  l=0; wt=2.0 }
$decisions['ao_sturgian_feathered_helmet_with_mail']    = @{ h=95; b=12; a=17; l=0; wt=2.5 }
$decisions['ao_sturgian_feathered_helmet_closed']       = @{ h=107;b=82; a=30; l=0; wt=2.5 }
$decisions['ao_sturgian_feathered_helmet_with_closed_mail'] = @{ h=118;b=82; a=30; l=0; wt=2.8 }
$decisions['ulfhednar_helm_b']                = @{ h=89; b=24; a=20; l=0; wt=3.5 }
$decisions['ulfhednar_helm']                  = @{ h=107;b=82; a=30; l=0; wt=3.5 }
$decisions['berserkr_helm_a']                 = @{ h=95; b=24; a=20; l=0; wt=3.5 }
$decisions['TV_sturgia_lord_helmet_a']        = @{ h=125;b=12; a=0;  l=0; wt=2.2 }
$decisions['TV_sturgia_lord_helmet_b']        = @{ h=113;b=36; a=12; l=0; wt=3.2 }
$decisions['TV_sturgia_lord_helmet_c']        = @{ h=115;b=36; a=20; l=0; wt=3.2 }
$decisions['TV_sturgia_lord_helmet_d']        = @{ h=116;b=24; a=20; l=0; wt=3.5 }
$decisions['TV_sturgia_lord_helmet_e']        = @{ h=116;b=24; a=20; l=0; wt=3.5 }
$decisions['TV_sturgia_lord_helmet_b2']       = @{ h=119;b=24; a=20; l=0; wt=3.5 }
$decisions['TV_sturgia_lord_helmet_c2']       = @{ h=120;b=12; a=20; l=0; wt=3.5 }
$decisions['TV_nord_helmet_k']                = @{ h=95; b=50; a=20; l=0; wt=3.5 }
$decisions['TV_nord_helmet_l']                = @{ h=107;b=82; a=30; l=0; wt=3.5 }
$decisions['TV_nord_helmet_s']                = @{ h=114;b=74; a=40; l=0; wt=3.5 }
$decisions['DZ_sturgia_helmet_b']             = @{ h=65; b=12; a=20; l=0; wt=2.8 }
$decisions['DZ_sturgia_helmet_c']             = @{ h=85; b=12; a=17; l=0; wt=3.2 }
$decisions['DZ_sturgia_helmet_a']             = @{ h=107;b=82; a=30; l=0; wt=3.5 }
$decisions['DZ_sturgia_helmet_d']             = @{ h=114;b=74; a=40; l=0; wt=3.5 }
$decisions['vaegir_helmet_open']              = @{ h=83; b=12; a=0;  l=0; wt=2.5 }
$decisions['vaegir_helmet_closed']            = @{ h=107;b=82; a=30; l=0; wt=2.5 }
$decisions['ao_sturgian_spiked_nasal_helmet'] = @{ h=85; b=12; a=17; l=0; wt=1.8 }
$decisions['tv_vlandia_helmet_f']             = @{ h=114;b=74; a=40; l=0; wt=3.5 }
$decisions['TV_sturgia_helmet_a']             = @{ h=127;b=84; a=45; l=0; wt=3.8 }
$decisions['TV_sturgia_helmet_b']             = @{ h=127;b=84; a=45; l=0; wt=3.8 }
$decisions['TV_sturgia_helmet_d']             = @{ h=125;b=12; a=0;  l=0; wt=3.5 }
$decisions['TV_sturgia_helmet_g2']            = @{ h=127;b=84; a=45; l=0; wt=3.8 }

# ========== Sturgia · Cape A/B (from agent A4) ==========
$decisions['TV_sturgia_shoulders_c']          = @{ h=0; b=18; a=8;  l=0; wt=1.6 }
$decisions['TV_sturgia_shoulders_f']          = @{ h=0; b=20; a=8;  l=0; wt=1.6 }
$decisions['TV_sturgia_shoulders_k']          = @{ h=0; b=18; a=8;  l=0; wt=1.6 }
$decisions['TV_sturgia_shoulders_b']          = @{ h=0; b=25; a=12; l=0; wt=2.5 }
$decisions['TV_sturgia_shoulders_e']          = @{ h=0; b=27; a=12; l=0; wt=2.5 }
$decisions['TV_sturgia_shoulders_j']          = @{ h=0; b=25; a=12; l=0; wt=2.5 }
$decisions['TV_sturgia_shoulders_g']          = @{ h=0; b=40; a=20; l=0; wt=2.5 }
$decisions['AR_sturgia_shoulders_a']          = @{ h=0; b=14; a=6;  l=0; wt=1.5 }
$decisions['AR_sturgia_shoulders_e']          = @{ h=0; b=12; a=6;  l=0; wt=1.4 }
$decisions['DZ_sturgia_shoulder_b']           = @{ h=0; b=25; a=12; l=0; wt=3.5 }
$decisions['DZ_sturgia_shoulder_c']           = @{ h=0; b=27; a=12; l=0; wt=3.5 }
$decisions['TV_sturgia_shoulders_h']          = @{ h=0; b=38; a=18; l=0; wt=3.5 }
$decisions['AR_sturgia_shoulders_e2']         = @{ h=0; b=30; a=12; l=0; wt=2.5 }
$decisions['AR_sturgia_shoulders_a2']         = @{ h=0; b=30; a=12; l=0; wt=2.5 }
$decisions['AR_sturgia_shoulders_a3']         = @{ h=0; b=32; a=12; l=0; wt=2.5 }
$decisions['AR_sturgia_shoulders_a4']         = @{ h=0; b=32; a=12; l=0; wt=3.0 }
$decisions['AR_sturgia_shoulders_a5']         = @{ h=0; b=34; a=12; l=0; wt=3.0 }
$decisions['TV_sturgia_shoulders_a']          = @{ h=0; b=32; a=15; l=0; wt=3.0 }
$decisions['TV_sturgia_shoulders_d']          = @{ h=0; b=34; a=15; l=0; wt=3.0 }
$decisions['TV_sturgia_shoulders_i']          = @{ h=0; b=32; a=15; l=0; wt=3.0 }
$decisions['AR_sturgia_shoulders_b']          = @{ h=0; b=25; a=12; l=0; wt=2.5 }
$decisions['AR_sturgia_shoulders_c']          = @{ h=0; b=32; a=12; l=0; wt=3.5 }
$decisions['AR_sturgia_shoulders_d']          = @{ h=0; b=34; a=12; l=0; wt=3.5 }
$decisions['lamellar_shoulder']               = @{ h=0; b=32; a=12; l=0; wt=3.0 }
$decisions['lamellar_shoulder_scarf']         = @{ h=0; b=32; a=12; l=0; wt=3.0 }
$decisions['tv_sturgia_cloak_a']              = @{ h=0; b=12; a=0;  l=0; wt=2.0 }
$decisions['pauldron_cape_z']                 = @{ h=0; b=20; a=0;  l=0; wt=3.0 }
$decisions['berserkr_fur_b']                  = @{ h=0; b=24; a=0;  l=0; wt=2.5 }
$decisions['berserkr_fur_c']                  = @{ h=0; b=24; a=0;  l=0; wt=2.5 }
$decisions['TV_battania_shoulders_c']         = @{ h=0; b=24; a=0;  l=0; wt=2.5 }
$decisions['berserkr_fur_a']                  = @{ h=0; b=28; a=0;  l=0; wt=3.0 }
$decisions['TV_sturgia_shoulders_l']          = @{ h=0; b=25; a=0;  l=0; wt=3.0 }
$decisions['TV_sturgia_shoulders_m']          = @{ h=0; b=27; a=0;  l=0; wt=3.0 }
$decisions['TV_sturgia_shoulders_n']          = @{ h=0; b=25; a=0;  l=0; wt=3.0 }

# ========== Sturgia · BodyArmor A-J (from agent A4) ==========
$decisions['kaftan']                          = @{ h=0; b=6;  a=5;  l=5;  wt=1.0 }
$decisions['ao_sturgian_leather_over_cloth']  = @{ h=0; b=20; a=14; l=21; wt=1.5 }
$decisions['AR_sturgia_armor_b']              = @{ h=0; b=25; a=22; l=25; wt=2.0 }
$decisions['AR_sturgia_armor_d']              = @{ h=0; b=25; a=22; l=25; wt=2.0 }
$decisions['AR_sturgia_armor_d2']             = @{ h=0; b=25; a=22; l=25; wt=2.0 }
$decisions['AR_sturgia_armor_e']              = @{ h=0; b=35; a=29; l=33; wt=8.3 }
$decisions['AR_sturgia_armor_e2']             = @{ h=0; b=35; a=29; l=33; wt=8.3 }
$decisions['ao_sturgian_leather_over_mail']   = @{ h=0; b=44; a=38; l=22; wt=8.3 }
$decisions['TV_sturgia_armor_a']              = @{ h=0; b=77; a=44; l=44; wt=12.0 }
$decisions['TV_sturgia_armor_a2']             = @{ h=0; b=77; a=44; l=44; wt=12.3 }
$decisions['ao_gen_gambeson_over_mail']       = @{ h=0; b=44; a=38; l=22; wt=9.0 }
$decisions['AR_sturgia_armor_a']              = @{ h=0; b=47; a=41; l=22; wt=9.0 }
$decisions['DZ_sturgia_armor_a']              = @{ h=0; b=40; a=15; l=15; wt=6.2 }
$decisions['DZ_sturgia_armor_a2']             = @{ h=0; b=40; a=15; l=15; wt=6.2 }
$decisions['DZ_sturgia_armor_a3']             = @{ h=0; b=40; a=15; l=15; wt=6.2 }
$decisions['DZ_sturgia_armor_b']              = @{ h=0; b=55; a=15; l=24; wt=12.0 }
$decisions['DZ_sturgia_armor_b2']             = @{ h=0; b=55; a=15; l=24; wt=12.0 }
$decisions['DZ_sturgia_armor_b3']             = @{ h=0; b=55; a=15; l=24; wt=12.0 }
$decisions['AR_sturgia_armor_c']              = @{ h=0; b=47; a=41; l=22; wt=9.0 }
$decisions['ao_sturgian_lamellar_vest']       = @{ h=0; b=73; a=25; l=40; wt=8.3 }
$decisions['ao_sturgian_lamellar_vest_b']     = @{ h=0; b=73; a=25; l=40; wt=8.3 }
$decisions['AO_eorling_armor_d']              = @{ h=0; b=55; a=15; l=24; wt=12.0 }
$decisions['AO_eorling_armor_d2']             = @{ h=0; b=55; a=15; l=24; wt=12.0 }
$decisions['ao_eorling_bronze_scale_over_mail'] = @{ h=0; b=77; a=44; l=44; wt=12.0 }
$decisions['ao_eorling_iron_scale_over_mail']   = @{ h=0; b=77; a=44; l=44; wt=12.0 }
$decisions['TV_sturgia_armor_f']              = @{ h=0; b=79; a=40; l=38; wt=21.5 }
$decisions['TV_sturgia_armor_f2']             = @{ h=0; b=79; a=40; l=38; wt=21.5 }
$decisions['TV_sturgia_armor_f3']             = @{ h=0; b=79; a=40; l=38; wt=21.5 }
$decisions['AR_sturgia_armor_f']              = @{ h=0; b=79; a=40; l=38; wt=21.5 }
$decisions['AR_sturgia_armor_f2']             = @{ h=0; b=79; a=40; l=38; wt=21.5 }
$decisions['ao_sturgia_armor_d']              = @{ h=0; b=90; a=45; l=50; wt=22.0 }
$decisions['ao_sturgia_armor_d2']             = @{ h=0; b=90; a=45; l=50; wt=22.0 }
$decisions['TV_sturgia_armor_e']              = @{ h=0; b=105;a=45; l=75; wt=24.0 }
$decisions['TV_sturgia_armor_e2']             = @{ h=0; b=105;a=45; l=75; wt=24.0 }
$decisions['TV_sturgia_armor_e3']             = @{ h=0; b=105;a=45; l=75; wt=24.0 }
$decisions['TV_sturgia_armor_b']              = @{ h=0; b=90; a=30; l=22; wt=20.0 }
$decisions['TV_sturgia_armor_c']              = @{ h=0; b=90; a=45; l=50; wt=22.0 }
$decisions['TV_sturgia_armor_d']              = @{ h=0; b=90; a=45; l=50; wt=22.0 }

# ========== Sturgia · Hand + Leg (from agent A4) ==========
$decisions['AR_sturgia_gloves_c']             = @{ h=0; b=0; a=41; l=0; wt=1.3 }
$decisions['TV_sturgia_gloves_b']             = @{ h=0; b=0; a=41; l=0; wt=1.3 }
$decisions['TV_sturgia_gloves_c']             = @{ h=0; b=0; a=41; l=0; wt=1.3 }
$decisions['TV_sturgia_gloves_d']             = @{ h=0; b=0; a=41; l=0; wt=1.3 }
$decisions['TV_sturgia_gloves_e']             = @{ h=0; b=0; a=41; l=0; wt=1.3 }
$decisions['AR_sturgia_gloves_a']             = @{ h=0; b=0; a=56; l=0; wt=1.5 }
$decisions['AR_sturgia_gloves_b']             = @{ h=0; b=0; a=56; l=0; wt=1.5 }
$decisions['TV_aserai_boots_h']               = @{ h=0; b=0; a=0; l=20; wt=0.9 }
$decisions['TV_aserai_boots_i']               = @{ h=0; b=0; a=0; l=23; wt=1.0 }
$decisions['TV_sturgia_boots_a']              = @{ h=0; b=0; a=0; l=60; wt=2.9 }
$decisions['TV_sturgia_boots_b']              = @{ h=0; b=0; a=0; l=60; wt=2.9 }

# ========== Aserai · HeadArmor A-K + Darshi追加 (from agent A4) ==========
$decisions['ao_mabili_feathered_headband']    = @{ h=6;  b=0;  a=0;  l=0; wt=0.5 }
$decisions['ao_mabili_leather_headband']      = @{ h=6;  b=0;  a=0;  l=0; wt=0.5 }
$decisions['AR_hat_c']                        = @{ h=9;  b=0;  a=0;  l=0; wt=0.5 }
$decisions['AR_hat_d']                        = @{ h=9;  b=0;  a=0;  l=0; wt=0.5 }
$decisions['AR_hat_h']                        = @{ h=10; b=0;  a=0;  l=0; wt=0.5 }
$decisions['AR_hat_i']                        = @{ h=14; b=0;  a=0;  l=0; wt=0.5 }
$decisions['AR_aserai_helmet_a']              = @{ h=44; b=0;  a=12; l=0; wt=1.5 }
$decisions['AR_aserai_helmet_s']              = @{ h=44; b=0;  a=12; l=0; wt=1.5 }
$decisions['AR_aserai_helmet_s2']             = @{ h=44; b=0;  a=12; l=0; wt=1.5 }
$decisions['aserai_nasalhelm_cloth']          = @{ h=44; b=0;  a=12; l=0; wt=1.5 }
$decisions['TV_aserai_helmet_v2']             = @{ h=44; b=0;  a=12; l=0; wt=1.5 }
$decisions['AR_aserai_helmet_b']              = @{ h=46; b=12; a=0;  l=0; wt=1.5 }
$decisions['hmj_southern_helmet']             = @{ h=46; b=12; a=0;  l=0; wt=1.5 }
$decisions['TV_aserai_helmet_b']              = @{ h=46; b=12; a=0;  l=0; wt=1.5 }
$decisions['TV_aserai_helmet_c']              = @{ h=46; b=12; a=0;  l=0; wt=1.5 }
$decisions['TV_aserai_guard_helmet_a']        = @{ h=46; b=12; a=0;  l=0; wt=1.8 }
$decisions['TV_aserai_guard_helmet_a2']       = @{ h=46; b=12; a=0;  l=0; wt=1.8 }
$decisions['TV_aserai_guard_helmet_b']        = @{ h=46; b=12; a=0;  l=0; wt=1.8 }
$decisions['TV_aserai_guard_helmet_d']        = @{ h=46; b=12; a=0;  l=0; wt=1.8 }
$decisions['ao_subuwari_a']                   = @{ h=50; b=0;  a=6;  l=0; wt=1.3 }
$decisions['ao_subuwari_a2']                  = @{ h=50; b=0;  a=6;  l=0; wt=1.3 }
$decisions['southern_cone_helmet']            = @{ h=50; b=0;  a=6;  l=0; wt=1.3 }
$decisions['saracen_helmet_c']                = @{ h=50; b=0;  a=6;  l=0; wt=1.3 }
$decisions['saracen_helmet_d_ii']             = @{ h=57; b=12; a=15; l=0; wt=1.2 }
$decisions['AR_aserai_archer_helmet_a']       = @{ h=50; b=0;  a=6;  l=0; wt=1.5 }
$decisions['AR_aserai_archer_helmet_e']       = @{ h=50; b=0;  a=6;  l=0; wt=1.5 }
$decisions['AR_aserai_archer_helmet_b']       = @{ h=50; b=0;  a=6;  l=0; wt=1.5 }
$decisions['AR_aserai_archer_helmet_f']       = @{ h=50; b=0;  a=6;  l=0; wt=1.5 }
$decisions['TV_aserai_guard_helmet_d2']       = @{ h=50; b=0;  a=12; l=0; wt=1.5 }
$decisions['TV_aserai_guard_helmet_d3']       = @{ h=50; b=0;  a=12; l=0; wt=1.5 }
$decisions['TV_aserai_guard_helmet_e']        = @{ h=80; b=12; a=25; l=0; wt=2.8 }
$decisions['TV_aserai_guard_helmet_e2']       = @{ h=80; b=12; a=25; l=0; wt=2.8 }
$decisions['TV_aserai_guard_helmet_e3']       = @{ h=80; b=12; a=25; l=0; wt=2.8 }
$decisions['TV_aserai_guard_helmet_c']        = @{ h=85; b=12; a=25; l=0; wt=2.9 }
$decisions['TV_aserai_guard_helmet_c2']       = @{ h=85; b=12; a=25; l=0; wt=2.9 }
$decisions['TV_aserai_guard_helmet_c3']       = @{ h=85; b=12; a=25; l=0; wt=2.9 }
$decisions['TV_aserai_guard_helmet_c4']       = @{ h=85; b=12; a=25; l=0; wt=2.9 }
$decisions['TV_aserai_guard_helmet_c5']       = @{ h=85; b=12; a=25; l=0; wt=2.9 }
$decisions['TV_aserai_guard_helmet_f']        = @{ h=88; b=24; a=40; l=0; wt=2.8 }
$decisions['TV_aserai_guard_helmet_f2']       = @{ h=88; b=24; a=40; l=0; wt=2.8 }
$decisions['TV_aserai_guard_helmet_f3']       = @{ h=88; b=24; a=40; l=0; wt=2.8 }
$decisions['AR_aserai_helmet_c']              = @{ h=57; b=12; a=15; l=0; wt=1.3 }
$decisions['AR_aserai_helmet_o']              = @{ h=57; b=12; a=15; l=0; wt=1.3 }
$decisions['AR_aserai_helmet_w']              = @{ h=57; b=12; a=15; l=0; wt=1.3 }
$decisions['AR_aserai_helmet_za']             = @{ h=57; b=12; a=15; l=0; wt=1.3 }
$decisions['TV_aserai_helmet_v']              = @{ h=57; b=12; a=15; l=0; wt=1.3 }
$decisions['TV_aserai_helmet_m']              = @{ h=57; b=12; a=15; l=0; wt=1.3 }
$decisions['TV_aserai_helmet_o']              = @{ h=57; b=12; a=15; l=0; wt=1.3 }
$decisions['AR_aserai_helmet_d']              = @{ h=62; b=16; a=10; l=0; wt=1.5 }
$decisions['TV_aserai_helmet_n']              = @{ h=62; b=16; a=10; l=0; wt=1.5 }
$decisions['saracen_helmet_c_ii']             = @{ h=62; b=16; a=10; l=0; wt=1.5 }
$decisions['southern_light_helmet_a']         = @{ h=57; b=12; a=15; l=0; wt=1.2 }
$decisions['AR_aserai_helmet_c2']             = @{ h=65; b=12; a=20; l=0; wt=1.5 }
$decisions['TV_aserai_helmet_w']              = @{ h=65; b=12; a=20; l=0; wt=1.5 }
$decisions['TV_aserai_helmet_p']              = @{ h=85; b=12; a=25; l=0; wt=2.9 }
$decisions['TV_aserai_helmet_r']              = @{ h=85; b=12; a=25; l=0; wt=2.9 }
$decisions['TV_aserai_helmet_t']              = @{ h=85; b=12; a=25; l=0; wt=2.9 }
$decisions['TV_aserai_helmet_x']              = @{ h=85; b=12; a=25; l=0; wt=2.9 }
$decisions['AR_aserai_helmet_e']              = @{ h=85; b=12; a=25; l=0; wt=2.9 }
$decisions['AR_aserai_helmet_f']              = @{ h=85; b=12; a=25; l=0; wt=2.9 }
$decisions['TV_aserai_helmet_q']              = @{ h=88; b=24; a=40; l=0; wt=3.0 }
$decisions['TV_aserai_helmet_s']              = @{ h=88; b=24; a=40; l=0; wt=3.0 }
$decisions['TV_aserai_helmet_u']              = @{ h=88; b=24; a=40; l=0; wt=3.0 }
$decisions['AR_aserai_helmet_zc']             = @{ h=90; b=24; a=40; l=0; wt=2.5 }
$decisions['southern_helmet_IIII']            = @{ h=57; b=12; a=15; l=0; wt=1.2 }
$decisions['southern_helmet_with_band']       = @{ h=62; b=16; a=10; l=0; wt=1.5 }
$decisions['AR_aserai_helmet_j']              = @{ h=62; b=16; a=10; l=0; wt=1.5 }
$decisions['AR_aserai_helmet_k']              = @{ h=75; b=4;  a=0;  l=0; wt=1.8 }
$decisions['AR_aserai_helmet_k2']             = @{ h=75; b=4;  a=0;  l=0; wt=1.8 }
$decisions['AR_aserai_helmet_l']              = @{ h=75; b=4;  a=0;  l=0; wt=1.8 }
$decisions['AR_aserai_helmet_l2']             = @{ h=75; b=4;  a=0;  l=0; wt=1.8 }
$decisions['AR_aserai_helmet_m']              = @{ h=77; b=4;  a=0;  l=0; wt=2.0 }
$decisions['AR_aserai_helmet_m2']             = @{ h=77; b=4;  a=0;  l=0; wt=2.0 }
$decisions['ao_subuwari_noblemans_helmet']    = @{ h=85; b=12; a=25; l=0; wt=2.5 }
$decisions['ao_subuwari_noblemans_helmet_b']  = @{ h=85; b=12; a=25; l=0; wt=2.5 }
$decisions['AR_aserai_helmet_n']              = @{ h=88; b=24; a=40; l=0; wt=3.5 }
$decisions['AR_aserai_helmet_n2']             = @{ h=88; b=24; a=40; l=0; wt=3.5 }
$decisions['hmj_aserai_helmet_a']             = @{ h=88; b=24; a=40; l=0; wt=3.0 }
$decisions['TV_aserai_helmet_f']              = @{ h=57; b=12; a=15; l=0; wt=2.0 }
$decisions['aserai_nasalhelm_mailcoif']       = @{ h=73; b=12; a=20; l=0; wt=2.8 }
$decisions['aserai_nasalhelm_mail_veil']      = @{ h=85; b=12; a=25; l=0; wt=3.0 }
$decisions['southern_mail_helm_band']         = @{ h=85; b=12; a=25; l=0; wt=3.0 }
$decisions['TV_aserai_helmet_a']              = @{ h=75; b=4;  a=0;  l=0; wt=2.0 }
$decisions['AR_aserai_helmet_p']              = @{ h=88; b=24; a=40; l=0; wt=3.0 }
$decisions['AR_aserai_helmet_q']              = @{ h=88; b=24; a=40; l=0; wt=3.0 }
$decisions['AR_aserai_helmet_y']              = @{ h=88; b=24; a=40; l=0; wt=3.0 }
$decisions['AR_aserai_helmet_x']              = @{ h=88; b=24; a=40; l=0; wt=3.0 }
$decisions['AR_aserai_helmet_z']              = @{ h=88; b=24; a=40; l=0; wt=3.0 }
$decisions['AR_aserai_helmet_zb']             = @{ h=88; b=24; a=40; l=0; wt=3.0 }
$decisions['AR_aserai_helmet_zd']             = @{ h=135;b=50; a=40; l=0; wt=4.0 }
$decisions['southern_heavy_tuareg']           = @{ h=75; b=4;  a=0;  l=0; wt=1.8 }
$decisions['TV_aserai_helmet_k']              = @{ h=75; b=4;  a=0;  l=0; wt=1.8 }
$decisions['saracen_helmet_d']                = @{ h=83; b=14; a=20; l=0; wt=2.0 }
$decisions['southern_heavy_helmet']           = @{ h=77; b=4;  a=0;  l=0; wt=2.0 }
$decisions['southern_heavy_helmet_with_feather'] = @{ h=85; b=12; a=25; l=0; wt=2.5 }
$decisions['AR_aserai_helmet_t']              = @{ h=75; b=4;  a=0;  l=0; wt=2.0 }
$decisions['AR_aserai_helmet_t2']             = @{ h=77; b=4;  a=0;  l=0; wt=2.0 }
$decisions['AR_aserai_helmet_u']              = @{ h=83; b=14; a=20; l=0; wt=2.2 }
$decisions['AR_aserai_helmet_u2']             = @{ h=85; b=14; a=20; l=0; wt=2.3 }
$decisions['AR_aserai_helmet_v']              = @{ h=88; b=64; a=40; l=0; wt=2.5 }
$decisions['AR_aserai_helmet_v2']             = @{ h=88; b=64; a=40; l=0; wt=2.5 }
$decisions['southern_lord_helmet_b']          = @{ h=90; b=22; a=0;  l=0; wt=2.5 }
$decisions['southern_lord_helmet_a']          = @{ h=90; b=70; a=30; l=0; wt=2.5 }
$decisions['AR_aserai_cataphract_helmet_a']   = @{ h=91; b=12; a=25; l=0; wt=3.5 }
$decisions['AR_aserai_cataphract_helmet_b']   = @{ h=91; b=12; a=25; l=0; wt=3.5 }
$decisions['AR_aserai_cataphract_helmet_c']   = @{ h=91; b=12; a=25; l=0; wt=3.5 }
$decisions['AR_aserai_cataphract_helmet_d']   = @{ h=91; b=12; a=25; l=0; wt=3.5 }
$decisions['tv_aserai_lord_helmet_h']         = @{ h=94; b=10; a=40; l=0; wt=3.5 }
$decisions['tv_aserai_lord_helmet_i']         = @{ h=94; b=10; a=40; l=0; wt=3.5 }
$decisions['tv_aserai_lord_helmet_e']         = @{ h=91; b=12; a=25; l=0; wt=3.5 }
$decisions['tv_aserai_lord_helmet_f']         = @{ h=91; b=12; a=25; l=0; wt=3.5 }
$decisions['AR_aserai_archer_helmet_c']       = @{ h=85; b=12; a=25; l=0; wt=2.8 }
$decisions['AR_aserai_archer_helmet_g']       = @{ h=85; b=12; a=25; l=0; wt=2.8 }
$decisions['AR_aserai_archer_helmet_d']       = @{ h=91; b=12; a=25; l=0; wt=3.5 }
$decisions['AR_aserai_archer_helmet_h']       = @{ h=91; b=12; a=25; l=0; wt=3.5 }
$decisions['AR_aserai_lord_helmet_a']         = @{ h=91; b=12; a=25; l=0; wt=3.5 }
$decisions['AR_aserai_lord_helmet_b']         = @{ h=91; b=12; a=25; l=0; wt=3.5 }
$decisions['TV_aserai_lord_helmet_b']         = @{ h=90; b=22; a=0;  l=0; wt=2.5 }
$decisions['TV_aserai_lord_helmet_c']         = @{ h=90; b=22; a=0;  l=0; wt=2.5 }
$decisions['TV_aserai_lord_helmet_d']         = @{ h=90; b=22; a=0;  l=0; wt=2.5 }
$decisions['DZ_aserai_helmet_f']              = @{ h=91; b=12; a=25; l=0; wt=3.0 }
$decisions['AR_aserai_lord_helmet_c']         = @{ h=94; b=10; a=40; l=0; wt=3.5 }
$decisions['AR_aserai_lord_helmet_d']         = @{ h=94; b=10; a=40; l=0; wt=3.5 }
$decisions['DZ_aserai_helmet_d']              = @{ h=97; b=12; a=25; l=0; wt=3.5 }
$decisions['southern_vendel_helmet']          = @{ h=97; b=12; a=25; l=0; wt=3.5 }
$decisions['AR_aserai_lord_helmet_g']         = @{ h=97; b=12; a=25; l=0; wt=3.5 }
$decisions['AR_aserai_lord_helmet_g2']        = @{ h=97; b=12; a=25; l=0; wt=3.5 }
$decisions['tv_aserai_lord_helmet_g']         = @{ h=97; b=12; a=25; l=0; wt=2.9 }
$decisions['AR_aserai_lord_helmet_e']         = @{ h=135;b=50; a=40; l=0; wt=4.0 }
$decisions['AR_aserai_lord_helmet_i']         = @{ h=135;b=50; a=40; l=0; wt=4.0 }
$decisions['AR_aserai_helmet_r']              = @{ h=135;b=50; a=40; l=0; wt=4.0 }
$decisions['AR_aserai_lord_helmet_h']         = @{ h=135;b=70; a=40; l=0; wt=4.0 }
$decisions['AR_aserai_lord_helmet_j']         = @{ h=97; b=12; a=25; l=0; wt=3.0 }
$decisions['AR_aserai_helmet_g']              = @{ h=95; b=12; a=40; l=0; wt=2.5 }
$decisions['AR_aserai_helmet_i']              = @{ h=73; b=12; a=20; l=0; wt=2.0 }
$decisions['AR_aserai_crown_a']               = @{ h=60; b=3;  a=0;  l=0; wt=2.5 }
$decisions['aserai_mubarizun_helmet']         = @{ h=97; b=12; a=25; l=0; wt=3.5 }
$decisions['TV_aserai_helmet_y']              = @{ h=95; b=12; a=25; l=0; wt=2.9 }
$decisions['TV_aserai_helmet_y2']             = @{ h=95; b=12; a=25; l=0; wt=2.9 }
$decisions['AR_aserai_lord_helmet_f']         = @{ h=130;b=30; a=25; l=0; wt=3.7 }
$decisions['AR_aserai_lord_helmet_f2']        = @{ h=115;b=22; a=20; l=0; wt=3.5 }
$decisions['AR_aserai_helmet_h']              = @{ h=124;b=36; a=25; l=0; wt=3.5 }
$decisions['TV_aserai_helmet_l']              = @{ h=124;b=36; a=25; l=0; wt=3.2 }
$decisions['TV_aserai_helmet_i']              = @{ h=122;b=36; a=25; l=0; wt=3.2 }

# ========== Aserai · Cape A/B (from agent A4) ==========
$decisions['AR_aserai_shoulder_g']            = @{ h=0; b=12; a=8;  l=0; wt=1.5 }
$decisions['AR_aserai_shoulder_v']            = @{ h=0; b=12; a=8;  l=0; wt=1.5 }
$decisions['AR_aserai_shoulder_v2']           = @{ h=0; b=12; a=8;  l=0; wt=1.5 }
$decisions['AR_aserai_shoulder_f']            = @{ h=0; b=13; a=6;  l=0; wt=1.8 }
$decisions['AR_aserai_shoulder_x']            = @{ h=0; b=13; a=6;  l=0; wt=1.5 }
$decisions['AR_aserai_shoulder_e']            = @{ h=0; b=15; a=8;  l=0; wt=2.5 }
$decisions['AR_aserai_shoulder_u']            = @{ h=0; b=15; a=8;  l=0; wt=2.5 }
$decisions['AR_aserai_shoulder_u2']           = @{ h=0; b=15; a=8;  l=0; wt=2.5 }
$decisions['AR_aserai_shoulder_h']            = @{ h=0; b=8;  a=4;  l=0; wt=0.5 }
$decisions['AR_aserai_shoulder_i']            = @{ h=0; b=12; a=6;  l=0; wt=1.0 }
$decisions['AR_aserai_shoulder_j']            = @{ h=0; b=12; a=6;  l=0; wt=1.0 }
$decisions['AR_aserai_shoulder_p']            = @{ h=0; b=12; a=6;  l=0; wt=1.0 }
$decisions['AR_aserai_scale_shoulder_d']      = @{ h=0; b=16; a=8;  l=0; wt=2.4 }
$decisions['AR_aserai_scale_cape_c']          = @{ h=0; b=20; a=10; l=0; wt=2.5 }
$decisions['AR_aserai_scale_cape_c2']         = @{ h=0; b=20; a=10; l=0; wt=2.5 }
$decisions['AR_aserai_scale_shoulder_e']      = @{ h=0; b=20; a=10; l=0; wt=2.5 }
$decisions['AR_aserai_scale_shoulder_c']      = @{ h=0; b=15; a=8;  l=0; wt=2.0 }
$decisions['AR_aserai_scale_shoulder_a']      = @{ h=0; b=24; a=8;  l=0; wt=2.5 }
$decisions['AR_aserai_scale_cape_a']          = @{ h=0; b=30; a=10; l=0; wt=3.5 }
$decisions['AR_aserai_scale_cape_a2']         = @{ h=0; b=30; a=10; l=0; wt=3.5 }
$decisions['AR_aserai_scale_cape_b']          = @{ h=0; b=36; a=10; l=0; wt=2.5 }
$decisions['AR_aserai_scale_cape_b2']         = @{ h=0; b=36; a=10; l=0; wt=2.5 }
$decisions['AR_aserai_scale_shoulder_b']      = @{ h=0; b=37; a=10; l=0; wt=2.0 }
$decisions['AO_aserai_shoulders_a']           = @{ h=0; b=39; a=10; l=0; wt=2.5 }
$decisions['AO_aserai_shoulders_a2']          = @{ h=0; b=39; a=10; l=0; wt=2.5 }
$decisions['AR_aserai_shoulder_a']            = @{ h=0; b=43; a=12; l=0; wt=3.0 }
$decisions['AR_aserai_shoulder_a2']           = @{ h=0; b=43; a=12; l=0; wt=3.0 }
$decisions['AR_aserai_shoulder_o']            = @{ h=0; b=43; a=12; l=0; wt=3.0 }
$decisions['AR_aserai_shoulder_o2']           = @{ h=0; b=43; a=12; l=0; wt=3.0 }
$decisions['AR_aserai_shoulder_t']            = @{ h=0; b=43; a=12; l=0; wt=3.0 }
$decisions['AR_aserai_shoulder_t2']           = @{ h=0; b=43; a=12; l=0; wt=3.0 }
$decisions['AR_aserai_shoulder_y']            = @{ h=0; b=30; a=10; l=0; wt=3.0 }
$decisions['AR_aserai_shoulder_y2']           = @{ h=0; b=30; a=10; l=0; wt=3.0 }
$decisions['AR_aserai_shoulder_y3']           = @{ h=0; b=30; a=10; l=0; wt=3.0 }
$decisions['tv_aserai_cloak_b']               = @{ h=0; b=32; a=10; l=0; wt=3.0 }
$decisions['tv_aserai_cloak_c']               = @{ h=0; b=32; a=10; l=0; wt=3.0 }
$decisions['AR_aserai_scarf_a']               = @{ h=0; b=6;  a=0;  l=0; wt=0.5 }
$decisions['AR_aserai_shoulder_k']            = @{ h=0; b=13; a=0;  l=0; wt=1.5 }
$decisions['AR_aserai_shoulder_q']            = @{ h=0; b=13; a=0;  l=0; wt=1.5 }
$decisions['tv_aserai_cloak_a']               = @{ h=0; b=12; a=0;  l=0; wt=2.0 }
$decisions['AR_aserai_shoulder_l']            = @{ h=0; b=28; a=0;  l=0; wt=3.0 }
$decisions['AR_aserai_shoulder_m']            = @{ h=0; b=28; a=0;  l=0; wt=3.0 }
$decisions['AR_aserai_shoulder_r']            = @{ h=0; b=28; a=0;  l=0; wt=3.0 }
$decisions['AR_aserai_shoulder_s']            = @{ h=0; b=28; a=0;  l=0; wt=3.0 }
$decisions['AR_aserai_lamellar_cape_a']       = @{ h=0; b=40; a=0;  l=0; wt=4.0 }
$decisions['AR_aserai_lamellar_cape_b']       = @{ h=0; b=40; a=0;  l=0; wt=4.0 }

# ========== Aserai · BodyArmor A-J + 波斯萨珊追加 (from agent A4) ==========
$decisions['AR_aserai_armor_n']               = @{ h=0; b=6;  a=5;  l=5;  wt=0.5 }
$decisions['AR_aserai_armor_zc']              = @{ h=0; b=7;  a=6;  l=7;  wt=1.0 }
$decisions['AR_aserai_armor_zd']              = @{ h=0; b=7;  a=6;  l=7;  wt=1.0 }
$decisions['AR_aserai_armor_zd2']             = @{ h=0; b=7;  a=6;  l=7;  wt=1.0 }
$decisions['AR_aserai_armor_o']               = @{ h=0; b=13; a=6;  l=13; wt=1.0 }
$decisions['ao_mabili_hide_skirt']            = @{ h=0; b=18; a=16; l=18; wt=1.3 }
$decisions['ao_mabili_leopard_skirt']         = @{ h=0; b=18; a=16; l=18; wt=1.3 }
$decisions['ao_mabili_zebra_skirt']           = @{ h=0; b=18; a=16; l=18; wt=1.3 }
$decisions['AR_aserai_armor_h']               = @{ h=0; b=18; a=16; l=18; wt=2.0 }
$decisions['AR_aserai_armor_s']               = @{ h=0; b=18; a=11; l=18; wt=2.0 }
$decisions['AR_aserai_armor_za']              = @{ h=0; b=18; a=11; l=18; wt=2.0 }
$decisions['AR_aserai_armor_p']               = @{ h=0; b=20; a=10; l=20; wt=2.0 }
$decisions['tv_aserai_lamellar_i']            = @{ h=0; b=21; a=13; l=20; wt=2.0 }
$decisions['tv_aserai_lamellar_j']            = @{ h=0; b=21; a=13; l=20; wt=2.0 }
$decisions['AR_aserai_armor_a']               = @{ h=0; b=25; a=15; l=24; wt=4.0 }
$decisions['AR_aserai_armor_k']               = @{ h=0; b=26; a=12; l=10; wt=3.0 }
$decisions['AR_aserai_armor_l']               = @{ h=0; b=26; a=12; l=10; wt=3.0 }
$decisions['AR_aserai_armor_q']               = @{ h=0; b=26; a=12; l=10; wt=3.0 }
$decisions['AR_aserai_armor_zb']              = @{ h=0; b=26; a=12; l=10; wt=3.0 }
$decisions['tv_empire_armor_q']               = @{ h=0; b=26; a=12; l=10; wt=4.0 }
$decisions['tv_empire_armor_t']               = @{ h=0; b=26; a=12; l=10; wt=4.0 }
$decisions['AR_aserai_armor_r']               = @{ h=0; b=38; a=33; l=31; wt=7.5 }
$decisions['tv_empire_armor_c']               = @{ h=0; b=38; a=33; l=31; wt=7.5 }
$decisions['AR_aserai_armor_e']               = @{ h=0; b=47; a=14; l=23; wt=12.0 }
$decisions['AR_aserai_armor_e2']              = @{ h=0; b=47; a=14; l=23; wt=12.0 }
$decisions['AR_aserai_armor_d']               = @{ h=0; b=50; a=15; l=25; wt=11.0 }
$decisions['AR_aserai_armor_d2']              = @{ h=0; b=50; a=15; l=25; wt=11.0 }
$decisions['AR_aserai_armor_g']               = @{ h=0; b=55; a=46; l=37; wt=15.0 }
$decisions['AR_aserai_armor_g2']              = @{ h=0; b=55; a=46; l=37; wt=15.0 }
$decisions['AR_aserai_armor_c']               = @{ h=0; b=47; a=14; l=23; wt=12.0 }
$decisions['AR_aserai_armor_b']               = @{ h=0; b=38; a=33; l=31; wt=7.5 }
$decisions['TV_aserai_robe_a']                = @{ h=0; b=49; a=49; l=49; wt=9.0 }
$decisions['AR_aserai_armor_m']               = @{ h=0; b=47; a=44; l=55; wt=11.0 }
$decisions['AR_aserai_armor_i']               = @{ h=0; b=49; a=49; l=44; wt=17.0 }
$decisions['AR_aserai_armor_j']               = @{ h=0; b=49; a=49; l=44; wt=17.0 }
$decisions['tv_aserai_lamellar_g']            = @{ h=0; b=50; a=15; l=25; wt=11.0 }
$decisions['tv_aserai_lamellar_g2']           = @{ h=0; b=50; a=15; l=25; wt=11.0 }
$decisions['tv_aserai_lamellar_c']            = @{ h=0; b=50; a=15; l=25; wt=8.0 }
$decisions['tv_aserai_lamellar_c2']           = @{ h=0; b=50; a=15; l=25; wt=8.0 }
$decisions['tv_aserai_lamellar_a2']           = @{ h=0; b=55; a=46; l=37; wt=8.0 }
$decisions['tv_aserai_lamellar_d2']           = @{ h=0; b=55; a=46; l=37; wt=8.0 }
$decisions['tv_aserai_lamellar_h']            = @{ h=0; b=60; a=55; l=44; wt=8.0 }
$decisions['tv_aserai_lamellar_h2']           = @{ h=0; b=60; a=55; l=44; wt=8.0 }
$decisions['tv_aserai_lamellar_b']            = @{ h=0; b=66; a=44; l=13; wt=14.0 }
$decisions['tv_aserai_lamellar_b2']           = @{ h=0; b=66; a=44; l=13; wt=14.0 }
$decisions['tv_aserai_lamellar_e']            = @{ h=0; b=125;a=55; l=95; wt=22.0 }
$decisions['tv_aserai_lamellar_e2']           = @{ h=0; b=122;a=55; l=95; wt=22.0 }
$decisions['tv_aserai_lamellar_f']            = @{ h=0; b=66; a=44; l=13; wt=18.0 }
$decisions['tv_aserai_lamellar_f2']           = @{ h=0; b=66; a=44; l=13; wt=18.0 }
$decisions['AR_aserai_armor_x']               = @{ h=0; b=60; a=55; l=44; wt=13.0 }
$decisions['AR_aserai_armor_w']               = @{ h=0; b=60; a=55; l=44; wt=13.0 }
$decisions['AR_aserai_armor_w2']              = @{ h=0; b=60; a=55; l=44; wt=13.0 }
$decisions['tv_aserai_lamellar_a']            = @{ h=0; b=66; a=16; l=38; wt=8.0 }
$decisions['tv_aserai_lamellar_d']            = @{ h=0; b=66; a=16; l=38; wt=8.0 }
$decisions['AR_aserai_armor_t']               = @{ h=0; b=58; a=22; l=32; wt=6.0 }
$decisions['AR_aserai_armor_t2']              = @{ h=0; b=58; a=22; l=32; wt=6.0 }
$decisions['tv_empire_armor_r']               = @{ h=0; b=58; a=22; l=32; wt=6.0 }
$decisions['tv_empire_armor_r2']              = @{ h=0; b=58; a=22; l=32; wt=6.0 }
$decisions['AR_aserai_lamellar_a']            = @{ h=0; b=85; a=45; l=43; wt=17.0 }
$decisions['AR_aserai_lamellar_b']            = @{ h=0; b=85; a=45; l=43; wt=17.0 }
$decisions['AR_aserai_armor_f']               = @{ h=0; b=90; a=35; l=65; wt=22.0 }
$decisions['AR_aserai_armor_f2']              = @{ h=0; b=90; a=35; l=65; wt=22.0 }
$decisions['AR_aserai_scale_armor_a']         = @{ h=0; b=93; a=38; l=33; wt=18.0 }
$decisions['mirror_armor']                    = @{ h=0; b=70; a=15; l=20; wt=8.0 }
$decisions['AR_aserai_scale_armor_b']         = @{ h=0; b=130;a=60; l=95; wt=22.0 }
$decisions['AR_aserai_scale_armor_c']         = @{ h=0; b=138;a=67; l=122; wt=28.0 }
$decisions['AR_aserai_armor_u']               = @{ h=0; b=70; a=35; l=45; wt=9.0 }
$decisions['AR_aserai_armor_u2']              = @{ h=0; b=70; a=35; l=45; wt=9.0 }
$decisions['AR_aserai_armor_y']               = @{ h=0; b=130;a=60; l=110; wt=20.0 }
$decisions['ao_imperial_brass_scale_armor']   = @{ h=0; b=95; a=45; l=68; wt=22.0 }
$decisions['ao_imperial_brass_scale_armor_b'] = @{ h=0; b=95; a=45; l=68; wt=22.0 }
$decisions['AR_aserai_armor_v']               = @{ h=0; b=62; a=30; l=25; wt=9.0 }
$decisions['AR_aserai_armor_v2']              = @{ h=0; b=62; a=30; l=25; wt=9.0 }
$decisions['AO_aserai_armor_a']               = @{ h=0; b=60; a=55; l=44; wt=12.0 }
$decisions['AO_aserai_armor_b']               = @{ h=0; b=60; a=55; l=44; wt=12.0 }
$decisions['AO_aserai_armor_c']               = @{ h=0; b=60; a=55; l=44; wt=12.0 }
$decisions['AO_aserai_armor_d']               = @{ h=0; b=60; a=55; l=44; wt=12.0 }
$decisions['tv_empire_armor_s']               = @{ h=0; b=70; a=35; l=45; wt=9.0 }
$decisions['tv_empire_armor_s2']              = @{ h=0; b=70; a=35; l=45; wt=9.0 }

# ========== Aserai · Hand + Leg (from agent A4) ==========
$decisions['TV_aserai_gloves_c']              = @{ h=0; b=0; a=20; l=0; wt=0.6 }
$decisions['TV_aserai_gloves_d']              = @{ h=0; b=0; a=20; l=0; wt=0.6 }
$decisions['TV_aserai_gloves_a']              = @{ h=0; b=0; a=29; l=0; wt=1.3 }
$decisions['TV_aserai_gloves_b']              = @{ h=0; b=0; a=29; l=0; wt=1.3 }
$decisions['TV_moccasins_b']                  = @{ h=0; b=0; a=0; l=16; wt=0.5 }
$decisions['TV_moccasins_c']                  = @{ h=0; b=0; a=0; l=16; wt=0.5 }
$decisions['TV_aserai_boots_e']               = @{ h=0; b=0; a=0; l=25; wt=1.5 }
$decisions['TV_aserai_boots_e2']              = @{ h=0; b=0; a=0; l=25; wt=1.5 }
$decisions['TV_aserai_boots_c']               = @{ h=0; b=0; a=0; l=35; wt=2.5 }
$decisions['TV_aserai_boots_d']               = @{ h=0; b=0; a=0; l=35; wt=2.5 }

Write-Host "Decisions loaded: $($decisions.Count) items" -ForegroundColor Green

# ---------- source XML index ----------
# Build id -> original XML node index across all sources

Write-Host "Indexing source XMLs..." -ForegroundColor Cyan
$idToNode = @{}

function Index-XmlFile {
    param([string]$Path)
    if (-not (Test-Path -LiteralPath $Path)) { return }
    try {
        [xml]$doc = Get-Content -LiteralPath $Path -Encoding UTF8
        if (-not $doc.Items) { return }
        foreach ($item in $doc.Items.Item) {
            $iid = $item.id
            if ($iid -and -not $idToNode.ContainsKey($iid)) {
                $idToNode[$iid] = @{ Node = $item; Source = $Path }
            }
        }
    } catch {}
}

# Index in priority order: RBM (overrides vanilla), OSA (workshop), NavalDLC, SandBoxCore
foreach ($root in @($RbmArmorRoot, $OsaArmorRoot, $OsaSaddleryRoot, $NavalDlcRoot, $SandboxCoreRoot)) {
    if (-not (Test-Path -LiteralPath $root)) { continue }
    Get-ChildItem -LiteralPath $root -Filter "*.xml" -File | ForEach-Object {
        Index-XmlFile -Path $_.FullName
    }
}

Write-Host "Indexed $($idToNode.Count) items across all sources" -ForegroundColor Green

# ---------- build output XML ----------
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine('<?xml version="1.0" encoding="utf-8"?>')
[void]$sb.AppendLine('<!-- OSABalance manual override - generated by src/manual_override.ps1. -->')
[void]$sb.AppendLine('<!-- Loaded AFTER OSABalance_armor_override.xml to apply hand-audited v2 values. -->')
[void]$sb.AppendLine('<!-- Sources: BALANCE_V2_LOG.md decisions across 6 cultures + NavalDLC Nord. -->')
[void]$sb.AppendLine('<Items>')

$found = 0
$missing = @()

foreach ($id in $decisions.Keys) {
    if (-not $idToNode.ContainsKey($id)) {
        $missing += $id
        continue
    }
    $entry = $idToNode[$id]
    $orig = $entry.Node
    $d = $decisions[$id]

    # Clone the original node so we can safely mutate
    $doc = New-Object System.Xml.XmlDocument
    $imported = $doc.ImportNode($orig, $true)

    # Locate Armor sub-element
    $armorNode = $null
    if ($imported.ItemComponent -and $imported.ItemComponent.Armor) {
        $armorNode = $imported.ItemComponent.Armor
    }
    if (-not $armorNode) {
        $missing += "$id (no Armor node)"
        continue
    }

    # Apply armor values
    if ($null -ne $d.h) { $armorNode.SetAttribute('head_armor', "$($d.h)") }
    if ($null -ne $d.b) { $armorNode.SetAttribute('body_armor', "$($d.b)") }
    if ($null -ne $d.l) { $armorNode.SetAttribute('leg_armor',  "$($d.l)") }
    if ($null -ne $d.a) { $armorNode.SetAttribute('arm_armor',  "$($d.a)") }

    # Optional weight override on the Item element itself
    if ($null -ne $d.wt) {
        $imported.SetAttribute('weight', "$($d.wt)")
    }

    # Serialize with 2-space indent
    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Indent = $true
    $settings.IndentChars = '  '
    $settings.OmitXmlDeclaration = $true

    $sw = New-Object System.IO.StringWriter
    $xw = [System.Xml.XmlWriter]::Create($sw, $settings)
    $imported.WriteTo($xw)
    $xw.Flush()
    [void]$sb.AppendLine("  " + $sw.ToString())
    $found++
}

[void]$sb.AppendLine('</Items>')

Set-Content -LiteralPath $outFile -Value $sb.ToString() -Encoding UTF8
Write-Host "Wrote $found items to $outFile" -ForegroundColor Green
if ($missing.Count -gt 0) {
    Write-Warning "Missing items ($($missing.Count)):"
    $missing | ForEach-Object { Write-Warning "  $_" }
}
