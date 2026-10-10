# OSW 盾牌数据总表 · 按材质索引

**生成日期**：2026-10-09
**数据源**：`ModuleData/OSABalance_shield_override.xml`（OSW 114 件盾牌 · 2026-09-25 v0.2 平衡后快照）
**生成工具**：`src/extract_shield_table.ps1` + `src/build_shield_md.ps1`

---

## 字段说明

| 字段 | 含义 |
|---|---|
| **StringId** | 游戏内部物品 ID（XML `id`）· XML 增补 override 的匹配键 |
| **Display Name** | 游戏内物品栏显示名（从 XML `name="{=...}English"` 剥去定位 token 后得到）|
| **Tier** | 武器等级（**计算值** · 不是 XML 字段）· 下方公式 |
| **HP** | 盾牌耐久（`hit_points`）· 直接决定战场抗击打能力 |
| **BA** | 盾牌护甲（`body_armor`）· 阻挡时的减伤 |
| **Wt** | 重量 kg（`weight`）· 影响 Tier 公式分母 + 持盾手挥速 |
| **ThrSpd** | 盾撞速度（`thrust_speed`）· 大多数 82，少数皮革盾 86/89 |
| **rawTier** | 公式原始输出（未取整 · 未 clamp）· 用于看 "勉强达到某 tier" 的 bug |

### Tier 计算公式（vanilla 反编译铁证 · `TaleWorlds.Core.DefaultItemValueModel.CalculateShieldTier`）

```
rawTier = (hit_points^1.22 + 3·body_armor + thrust_speed) / (6 + weight^1.11) × 0.04 − 2

Tier (显示) = ClampInt(Round(rawTier), 0, 6)
```

**关键观察**（已在 ModdingJournal §待办 P0 OSW Shield Tier Fix 中归档）：
- **weight 在分母 ^1.11 指数项**：重盾 (如 7.3kg) 会被显著压 tier
- Tier **不直接影响战场护盾强度**（hp/ba/weight 才直接影响）
- Tier 影响 UI 分类 / Retinues `AllowedTierDifference` 筛选 / 商人售价 / UX 感知

---

## 一、金属包边类（Metal Rim · 44 件 · 木芯 + 金属边条）

OSW 主力盾线 · 木质盾身用金属（铁/青铜/黄铜/钢）镶边或铆合 · 重量 2.5-4.7kg 范围。

### 1.1 Steel · 钢制（1 件）

<!-- MAT:Steel COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zc` | Steel Round Shield | T6 | 760 | 4 | 4 | 82 | 10.63 |

### 1.2 Brass · 黄铜（1 件）

<!-- MAT:Brass COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zzi3` | Brass Domed Shield | T6 | 712 | 4 | 4 | 82 | 9.69 |

### 1.3 Bronze · 青铜（20 件）

<!-- MAT:Bronze COUNT:20 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zzd` | Bronze Round Shield | T6 | 760 | 4 | 4 | 82 | 10.63 |
| `AR_shield_infantry_zzd2` | Rimmed Bronze Round Shield | T6 | 760 | 4 | 4 | 82 | 10.63 |
| `AR_shield_infantry_c2` | Bronze Decorated Oval Shield | T6 | 700 | 6 | 4.7 | 82 | 8.57 |
| `AR_shield_infantry_h2` | Bronze Decorated Wooden Oval Shield | T6 | 700 | 6 | 4.7 | 82 | 8.57 |
| `AR_shield_infantry_e2` | Bronze Reinforced Round Shield | T6 | 665 | 6 | 3.5 | 82 | 9.5 |
| `AR_shield_infantry_zo2` | Bronze Decorated Domed Shield | T6 | 665 | 6 | 3.5 | 82 | 9.5 |
| `AR_shield_infantry_zzo2` | Bronze Decorated Round Shield | T6 | 665 | 6 | 3.5 | 82 | 9.5 |
| `AR_shield_infantry_a2` | Bronze Rimmed Oval Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |
| `AR_shield_infantry_b2` | Bronze Reinforced Oval Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |
| `AR_shield_infantry_f2` | Bronze Rimmed Wooden Oval Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |
| `AR_shield_infantry_g2` | Bronze Reinforced Wooden Oval Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |
| `AR_shield_infantry_u2` | Bronze Decorated Round Shield | T6 | 542 | 5 | 3.5 | 82 | 7.03 |
| `AR_shield_infantry_zn2` | Bronze Reinforced Domed Shield | T6 | 542 | 5 | 3.5 | 82 | 7.03 |
| `AR_shield_infantry_zzc2` | Bronze Decorated Buckler | T6 | 484 | 2 | 2.5 | 82 | 7.01 |
| `AR_shield_infantry_zze` | Bronze Buckler | T6 | 484 | 2 | 2.5 | 82 | 7.01 |
| `AR_shield_infantry_zze2` | Rimmed Bronze Buckler | T6 | 484 | 2 | 2.5 | 82 | 7.01 |
| `AR_shield_infantry_d2` | Bronze Simple Round Shield | T6 | 456 | 2 | 3.3 | 82 | 5.55 |
| `AR_shield_infantry_zm2` | Bronze Simple Domed Shield | T6 | 456 | 2 | 3.3 | 82 | 5.55 |
| `AR_shield_infantry_t2` | Bronze Reinforced Round Shield | T5 | 456 | 4 | 3.5 | 82 | 5.38 |
| `AR_shield_infantry_zzb2` | Bronze Reinforced Buckler | T5 | 408 | 2 | 2.5 | 82 | 5.39 |

### 1.4 Iron · 铁制（22 件）

<!-- MAT:Iron COUNT:22 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zzi2` | Iron Domed Shield | T6 | 712 | 4 | 4 | 82 | 9.69 |
| `AR_shield_infantry_c` | Iron Decorated Oval Shield | T6 | 700 | 6 | 4.7 | 82 | 8.57 |
| `AR_shield_infantry_h` | Iron Decorated Wooden Oval Shield | T6 | 700 | 6 | 4.7 | 82 | 8.57 |
| `AR_shield_infantry_zb` | Iron Decorated Coffin Shield | T6 | 700 | 6 | 4.7 | 82 | 8.57 |
| `AR_shield_infantry_e` | Iron Reinforced Round Shield | T6 | 665 | 6 | 3.5 | 82 | 9.5 |
| `AR_shield_infantry_zo` | Iron Decorated Domed Shield | T6 | 665 | 6 | 3.5 | 82 | 9.5 |
| `AR_shield_infantry_zzo` | Iron Decorated Round Shield | T6 | 665 | 6 | 3.5 | 82 | 9.5 |
| `AR_shield_infantry_a` | Iron Rimmed Oval Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |
| `AR_shield_infantry_b` | Iron Reinforced Oval Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |
| `AR_shield_infantry_f` | Iron Rimmed Wooden Oval Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |
| `AR_shield_infantry_g` | Iron Reinforced Wooden Oval Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |
| `AR_shield_infantry_x` | Iron Rimmed Wooden Square Shield | T6 | 570 | 5 | 4.7 | 82 | 6.29 |
| `AR_shield_infantry_y` | Iron Reinforced Wooden Square Shield | T6 | 570 | 5 | 4.7 | 82 | 6.29 |
| `AR_shield_infantry_za` | Iron Reinforced Coffin Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |
| `AR_shield_infantry_ze` | Iron Reinforced Rectangular Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |
| `AR_shield_infantry_u` | Iron Decorated Round Shield | T6 | 542 | 5 | 3.5 | 82 | 7.03 |
| `AR_shield_infantry_zn` | Iron Reinforced Domed Shield | T6 | 542 | 5 | 3.5 | 82 | 7.03 |
| `AR_shield_infantry_zzc` | Iron Decorated Buckler | T6 | 484 | 2 | 2.5 | 82 | 7.01 |
| `AR_shield_infantry_d` | Iron Simple Round Shield | T6 | 456 | 2 | 3.3 | 82 | 5.55 |
| `AR_shield_infantry_zm` | Iron Simple Domed Shield | T6 | 456 | 2 | 3.3 | 82 | 5.55 |
| `AR_shield_infantry_t` | Iron Reinforced Round Shield | T5 | 456 | 4 | 3.5 | 82 | 5.38 |
| `AR_shield_infantry_zzb` | Iron Reinforced Buckler | T5 | 408 | 2 | 2.5 | 82 | 5.39 |

---

## 二、带箍重木类（Strapped · 12 件 · 重木芯 + 金属束带 · weight=7.3kg）

**🔴 已知 tier 偏低问题根因**：全部 weight=7.3kg · 公式分母 (6+7.3^1.11)≈14.6 远高于普通盾 (6+4.7^1.11)≈11.4 · 导致同等 hp/ba 盾 tier 被压低 1-2 档。详见 ModdingJournal §P0 OSW Shield Tier Fix。

### 2.1 Strapped Iron · 带箍铁（5 件）

<!-- MAT:Strapped Iron COUNT:5 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_s` | Strapped Iron Reinforced Round Shield | T6 | 770 | 4 | 7.3 | 82 | 7.06 |
| `AR_shield_infantry_o` | Strapped Iron Decorated Round Shield | T5 | 627 | 6 | 7.3 | 82 | 5.12 |
| `AR_shield_infantry_n` | Strapped Iron Reinforced Round Shield | T4 | 528 | 3 | 7.3 | 82 | 3.8 |
| `AR_shield_infantry_r` | Strapped Iron Simple Round Shield | T4 | 528 | 3 | 7.3 | 82 | 3.8 |
| `AR_shield_infantry_m` | Strapped Iron Bound Round Shield | T2 | 418 | 2 | 7.3 | 82 | 2.42 |

### 2.2 Strapped Bronze · 带箍青铜（5 件）

<!-- MAT:Strapped Bronze COUNT:5 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_s2` | Strapped Bronze Reinforced Round Shield | T6 | 770 | 4 | 7.3 | 82 | 7.06 |
| `AR_shield_infantry_o2` | Strapped Bronze Decorated Round Shield | T5 | 627 | 6 | 7.3 | 82 | 5.12 |
| `AR_shield_infantry_n2` | Strapped Bronze Reinforced Round Shield | T4 | 528 | 3 | 7.3 | 82 | 3.8 |
| `AR_shield_infantry_r2` | Strapped Bronze Simple Round Shield | T4 | 528 | 3 | 7.3 | 82 | 3.8 |
| `AR_shield_infantry_m2` | Strapped Bronze Bound Round Shield | T2 | 418 | 2 | 7.3 | 82 | 2.42 |

### 2.3 Strapped Norse · 维京带箍（1 件）

<!-- MAT:Strapped Norse COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_q` | Strapped Norse Round Shield | T6 | 770 | 4 | 7.3 | 82 | 7.06 |

### 2.4 Strapped Northern · 北方带箍（1 件）

<!-- MAT:Strapped Northern COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_p` | Strapped Northern Round Shield | T4 | 528 | 3 | 7.3 | 82 | 3.8 |

---

## 三、高地变体类（Highland · 12 件 · Battanian 风格 · AD_ 前缀为显式冠金属版）

### 3.1 Highland Iron · 高地铁（4 件）

<!-- MAT:Highland Iron COUNT:4 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AD_shield_infantry_b` | Highland Iron Reinforced Oval Shield | T6 | 712 | 4 | 3.5 | 82 | 10.44 |
| `AD_shield_infantry_d` | Highland Iron Reinforced Rectangular Shield | T6 | 712 | 4 | 3.5 | 82 | 10.44 |
| `AD_shield_infantry_a` | Highland Iron Rimmed Oval Shield | T6 | 665 | 3 | 3.5 | 82 | 9.46 |
| `AD_shield_infantry_c` | Highland Iron Rimmed Rectangular Shield | T6 | 665 | 3 | 3.5 | 82 | 9.46 |

### 3.2 Highland Brass · 高地黄铜（4 件）

<!-- MAT:Highland Brass COUNT:4 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AD_shield_infantry_b2` | Highland Brass Reinforced Oval Shield | T6 | 712 | 4 | 3.5 | 82 | 10.44 |
| `AD_shield_infantry_d2` | Highland Brass Reinforced Rectangular Shield | T6 | 712 | 4 | 3.5 | 82 | 10.44 |
| `AD_shield_infantry_a2` | Highland Brass Rimmed Oval Shield | T6 | 665 | 3 | 3.5 | 82 | 9.46 |
| `AD_shield_infantry_c2` | Highland Brass Rimmed Rectangular Shield | T6 | 665 | 3 | 3.5 | 82 | 9.46 |

### 3.3 Highland · 通用高地（4 件 · 无显式金属标注）

<!-- MAT:Highland COUNT:4 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zza` | Highland Large Reinforced Shield | T6 | 712 | 4 | 3.5 | 82 | 10.44 |
| `AR_shield_infantry_zz` | Highland Large Rimmed Shield | T6 | 665 | 3 | 3.5 | 82 | 9.46 |
| `AR_shield_infantry_zzn` | Highland Reinforced Buckler | T6 | 570 | 5 | 4.7 | 82 | 6.29 |
| `AR_shield_infantry_zzm` | Highland Buckler | T5 | 480 | 4 | 4.7 | 82 | 4.78 |

---

## 四、木基础类（Wood / Timber / Wicker · 23 件 · 无金属加固）

### 4.1 Wood · 裸木（3 件 · 无材质前缀 · 为普通木盾默认分组）

<!-- MAT:Wood COUNT:3 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zr` | Large Round Shield | T6 | 770 | 7 | 7.3 | 82 | 7.08 |
| `AR_shield_infantry_zzf` | Wooden Tower Shield | T5 | 480 | 2 | 4.7 | 82 | 4.76 |
| `AR_shield_infantry_v` | Wooden Square Shield | T3 | 380 | 1 | 4.7 | 82 | 3.15 |

### 4.2 Timber · 实木板（12 件）

<!-- MAT:Timber COUNT:12 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_k2` | Decorated Timber Shield | T6 | 570 | 5 | 4.7 | 82 | 6.29 |
| `AR_shield_infantry_zw2` | Decorated Timber Oval Shield | T6 | 570 | 5 | 4.7 | 82 | 6.29 |
| `AR_shield_infantry_zzl` | Reinforced Timber Shield | T6 | 570 | 5 | 4.7 | 82 | 6.29 |
| `AR_shield_infantry_zt2` | Heavy Round Timber Shield | T5 | 627 | 4 | 7.3 | 82 | 5.11 |
| `AR_shield_infantry_j2` | Reinforced Timber Shield | T5 | 480 | 2 | 4.7 | 82 | 4.76 |
| `AR_shield_infantry_zv2` | Heavy Timber Oval Shield | T5 | 480 | 2 | 4.7 | 82 | 4.76 |
| `AR_shield_infantry_zzk` | Bound Timber Shield | T5 | 480 | 2 | 4.7 | 82 | 4.76 |
| `AR_shield_infantry_zzh` | Timber Round Shield | T4 | 361 | 1 | 3.3 | 82 | 3.75 |
| `AR_shield_infantry_l2` | Timber Shield | T3 | 380 | 1 | 4.7 | 82 | 3.15 |
| `AR_shield_infantry_zu2` | Timber Oval Shield | T3 | 380 | 1 | 4.7 | 82 | 3.15 |
| `AR_shield_infantry_zzj` | Timber Shield | T3 | 380 | 1 | 4.7 | 82 | 3.15 |
| `AR_shield_infantry_zs2` | Round Timber Shield | T2 | 418 | 2 | 7.3 | 82 | 2.42 |

### 4.3 Wicker · 柳条编制（8 件）

<!-- MAT:Wicker COUNT:8 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_k` | Decorated Wicker Shield | T6 | 570 | 5 | 4.7 | 82 | 6.29 |
| `AR_shield_infantry_zw` | Decorated Wicker Oval Shield | T6 | 570 | 5 | 4.7 | 82 | 6.29 |
| `AR_shield_infantry_zt` | Heavy Round Wicker Shield | T5 | 627 | 4 | 7.3 | 82 | 5.11 |
| `AR_shield_infantry_j` | Reinforced Wicker Shield | T5 | 480 | 2 | 4.7 | 82 | 4.76 |
| `AR_shield_infantry_zv` | Heavy Wicker Oval Shield | T5 | 480 | 2 | 4.7 | 82 | 4.76 |
| `AR_shield_infantry_l` | Wicker Shield | T3 | 380 | 1 | 4.7 | 82 | 3.15 |
| `AR_shield_infantry_zu` | Wicker Oval Shield | T3 | 380 | 1 | 4.7 | 82 | 3.15 |
| `AR_shield_infantry_zs` | Round Wicker Shield | T2 | 418 | 2 | 7.3 | 82 | 2.42 |

---

## 五、皮革与包覆类（Leather / Hide / Cloth / Wrapped · 10 件）

### 5.1 Leather · 皮革（5 件 · 唯一含 thrust_speed > 82 的盾组 · Bound 系列 86/89）

<!-- MAT:Leather COUNT:5 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zq` | Leather Bound Large Round Shield | T6 | 770 | 7 | 6.9 | 89 | 7.45 |
| `AR_shield_infantry_zp` | Leather Bound Heavy Round Shield | T6 | 750 | 7 | 4.7 | 86 | 9.49 |
| `AR_shield_infantry_z` | Leather Rimmed Coffin Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |
| `AR_shield_infantry_zd` | Leather Rimmed Rectangular Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |
| `AR_shield_infantry_w` | Leather Rimmed Wooden Square Shield | T5 | 480 | 2 | 4.7 | 82 | 4.76 |

### 5.2 Hide Wrapped · 兽皮包覆（1 件）

<!-- MAT:Hide Wrapped COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zzg` | Hide Wrapped Tower Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |

### 5.3 Cloth Wrapped · 布料包覆（1 件）

<!-- MAT:Cloth Wrapped COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zf` | Cloth Wrapped Rectangular Shield | T3 | 380 | 1 | 4.7 | 82 | 3.15 |

### 5.4 Wrapped · 通用包覆（1 件）

<!-- MAT:Wrapped COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zzi` | Wrapped Domed Shield | T6 | 542 | 3 | 4 | 82 | 6.47 |

### 5.5 Wrapped Timber · 包覆实木（1 件）

<!-- MAT:Wrapped Timber COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_i2` | Wrapped Timber Shield | T3 | 380 | 1 | 4.7 | 82 | 3.15 |

### 5.6 Wrapped Wicker · 包覆柳条（1 件）

<!-- MAT:Wrapped Wicker COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_i` | Wrapped Wicker Shield | T3 | 380 | 1 | 4.7 | 82 | 3.15 |

---

## 六、兽皮装饰类（Pelt / Skin · 6 件 · 显式动物皮覆面）

### 6.1 Lion Pelt · 狮皮（2 件）

<!-- MAT:Lion Pelt COUNT:2 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zzp3` | Reinforced Lion Pelt Oval Shield | T6 | 456 | 2 | 3.2 | 82 | 5.64 |
| `AR_shield_infantry_zl` | Lion Pelt Oval Shield | T5 | 480 | 2 | 4.7 | 82 | 4.76 |

### 6.2 Leopard Pelt · 豹皮（2 件）

<!-- MAT:Leopard Pelt COUNT:2 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zzp2` | Reinforced Leopard Pelt Oval Shield | T6 | 456 | 2 | 3.2 | 82 | 5.64 |
| `AR_shield_infantry_zk` | Leopard Pelt Oval Shield | T5 | 480 | 2 | 4.7 | 82 | 4.76 |

### 6.3 Zebra Skin · 斑马皮（2 件）

<!-- MAT:Zebra Skin COUNT:2 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zzp` | Reinforced Zebra Skin Oval Shield | T6 | 456 | 2 | 3.2 | 82 | 5.64 |
| `AR_shield_infantry_zj` | Zebra Skin Oval Shield | T5 | 480 | 2 | 4.7 | 82 | 4.76 |

---

## 七、Adarga 皮盾类（3 件 · 伊比利亚风格的双叶皮盾 · Aserai 文化）

**注**：Adarga 全系 weight=7.3kg 吃 tier · 外加低 hp (418) 导致 T2 (rawTier=2.43) · 若用户希望这组 UX 感知更"精英"可定向上调。

### 7.1 Lion Pelt Adarga · 狮皮 Adarga（1 件）

<!-- MAT:Lion Pelt Adarga COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zi` | Lion Pelt Adarga | T2 | 418 | 4 | 7.3 | 82 | 2.43 |

### 7.2 Leopard Pelt Adarga · 豹皮 Adarga（1 件）

<!-- MAT:Leopard Pelt Adarga COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zh` | Leopard Pelt Adarga | T2 | 418 | 4 | 7.3 | 82 | 2.43 |

### 7.3 Zebra Skin Adarga · 斑马皮 Adarga（1 件）

<!-- MAT:Zebra Skin Adarga COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zg` | Zebra Skin Adarga | T2 | 418 | 4 | 7.3 | 82 | 2.43 |

---

## 八、芦苇编织类（Woven Reed · 2 件 · Aserai 轻盾）

<!-- MAT:Woven Reed COUNT:2 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zzr` | Reinforced Woven Reed Oval Shield | T6 | 456 | 2 | 3.2 | 82 | 5.64 |
| `AR_shield_infantry_zzq` | Woven Reed Oval Shield | T5 | 480 | 2 | 4.7 | 82 | 4.76 |

---

## 九、文化专属类（Cultural · 2 件 · 命名直接反映文化来源）

### 9.1 Nomadic Steppe · 游牧草原（1 件 · Khuzait）

<!-- MAT:Nomadic Steppe COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zx` | Nomadic Steppe Shield | T6 | 570 | 3 | 4.7 | 82 | 6.27 |

### 9.2 Eastern Rider · 东方骑兵（1 件 · 骑兵小盾 · 2.7kg）

<!-- MAT:Eastern Rider COUNT:1 -->
| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |
|---|---|---:|---:|---:|---:|---:|---:|
| `AR_shield_infantry_zy` | Eastern Rider Shield | T6 | 484 | 2 | 2.7 | 82 | 6.76 |

---

## Tier 分布汇总（全 114 件）

<!-- TIER_DIST -->
| Tier | 件数 | 占比 |
|---|---:|---:|
| T6 | 72 | 63.2% |
| T5 | 20 | 17.5% |
| T4 | 6 | 5.3% |
| T3 | 9 | 7.9% |
| T2 | 7 | 6.1% |

---

## 材质分布汇总（29 组材质）

| 材质 | 件数 | 顶 tier | 底 tier | 备注 |
|---|---:|---:|---:|---|
| Iron | 22 | T6 | T5 | OSW 主力金属盾线 |
| Bronze | 20 | T6 | T5 | Iron 系的 2 变体镜像 |
| Timber | 12 | T6 | T2 | 实木裸盾 · 全 tier 覆盖 |
| Wicker | 8 | T6 | T2 | 柳条裸盾 · 全 tier 覆盖 |
| Strapped Iron | 5 | T6 | T2 | 🔴 **weight=7.3 导致 m=T2 bug** |
| Strapped Bronze | 5 | T6 | T2 | 🔴 同上问题（镜像）|
| Leather | 5 | T6 | T5 | 唯一 thrust_speed >82 组 |
| Highland Iron | 4 | T6 | T6 | AD_ 前缀 · Battanian AD mod 新增 |
| Highland Brass | 4 | T6 | T6 | 同上 |
| Highland | 4 | T6 | T5 | 原生 Battanian 风 |
| Wood | 3 | T6 | T3 | 裸木默认组 |
| Lion Pelt | 2 | T6 | T5 | Aserai 兽皮覆面 |
| Leopard Pelt | 2 | T6 | T5 | 同上 |
| Zebra Skin | 2 | T6 | T5 | 同上 |
| Woven Reed | 2 | T6 | T5 | Aserai 轻盾 |
| Strapped Norse | 1 | T6 | — | 维京顶级 |
| Strapped Northern | 1 | T4 | — | 北方中档 |
| Lion Pelt Adarga | 1 | T2 | — | 皮盾 · tier 偏低 |
| Leopard Pelt Adarga | 1 | T2 | — | 同上 |
| Zebra Skin Adarga | 1 | T2 | — | 同上 |
| Hide Wrapped | 1 | T6 | — | 单例 Tower Shield |
| Cloth Wrapped | 1 | T3 | — | 单例 Rectangular |
| Wrapped | 1 | T6 | — | Domed Shield |
| Wrapped Timber | 1 | T3 | — | 单例 |
| Wrapped Wicker | 1 | T3 | — | 单例 |
| Steel | 1 | T6 | — | 独苗 Round Shield |
| Brass | 1 | T6 | — | 独苗 Domed Shield |
| Nomadic Steppe | 1 | T6 | — | Khuzait 文化盾 |
| Eastern Rider | 1 | T6 | — | 骑兵小盾 |

---

## 使用说明

- **要查某把具体盾**：按材质定位章节 · 用 `Ctrl+F` 搜 StringId
- **要对比同材质跨 tier**：直接看材质小节内表格（已按 Tier desc → HP desc 排序）
- **要查 tier bug**（rawTier 虚高/虚低造成的异常）：关注 rawTier 列 · 远离整数边界 (±0.3) 的是稳态 · 贴近边界的是潜在 bug 候选

**重跑数据**：如 OSW override XML 有变 · 跑：
```powershell
& src/extract_shield_table.ps1   # 刷新 shield_table_data.json
& src/build_shield_md.ps1        # 刷新 ASCII tables (debug 用)
# SHIELD_DATA_COMPLETE.md 需手工同步 Chinese 包装层
```
