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

# ==================== Vlandia L family 修订 (2026-09-24) ====================
# BALANCE_V2_LOG.md line ~3700: L family "Lamellar Over Heavy Mail Hauberk"
# anchor was wrong (plated_leather_coat 75), corrected to sturgian_fortified_armor 100
# "扎甲+mail 视为顶级防御" rule

$decisions['AR_vlandia_lamellar_a']  = @{ h=0; b=100; l=95;  a=100; wt=22 }  # Steel Lamellar (顶)
$decisions['AR_vlandia_lamellar_a2'] = @{ h=0; b=97;  l=92;  a=97;  wt=22 }  # Brass -3
$decisions['AR_vlandia_lamellar_b']  = @{ h=0; b=90;  l=85;  a=85;  wt=18 }  # Leather -10

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
