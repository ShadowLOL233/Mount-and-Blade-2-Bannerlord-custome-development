# PSPortFix · PlayerSettlement 港口崩溃修补

**版本**：v1.0.0
**日期**：2026-09-23
**作用**：修复 PlayerSettlement (workshop 3720376888) v7.5.0 的**默认模板缺 port location** bug，避免玩家点击 town 菜单的 "Enter Port" 时游戏崩溃。

## 问题诊断

### 症状
- PS 建的 town（如 Aetofolia）在 town menu 里可见 "Enter Port" 选项
- 点击后立即 TWCrashUploader 弹窗崩溃

### 根因
PS `Player_Settlement_Templates_default\` 里的 **69 个 town 变体全部未声明 `<Location id="port"/>`**：
- empire × 19 变体
- vlandia × 10 变体
- battania × 10 变体
- sturgia × 10 变体
- aserai × 10 变体
- khuzait × 10 变体

但 NavalDLC 的 `LocationComplexTemplate.town_complex` **全局给所有 town 注入 "Enter Port" 菜单**，fallback 场景 `empire_interior_tavern_a`（tavern 场景不含 wharf entity）。点击后游戏引擎尝试用 port 逻辑加载 tavern scene，native crash。

**对比**：PS `Player_Settlement_Templates_War_Sails\` 里 60 个变体全部有 `<Location id="port" scene_name="{culture}_shipyard"/>`——用户建 town 时未选 War Sails 模板则中招。

## 修复策略

**workshop XML 直接补丁**（当前 v1.0 方案）：
- 备份 PS 工作坊的 6 个 `_default.xml` 文件到 `Baselines/`
- 在每个 town 变体的 `<Locations>` 末尾注入：
  ```xml
  <Location id="port" scene_name="{culture}_shipyard" />
  ```
- 各文化使用对应 vanilla 场景（empire → `empire_shipyard` · vlandia → `vlandia_shipyard` · ...）

**优点**：立即生效、不需要 DLL 编译、可撤销
**风险**：Steam workshop 更新会覆盖 → 需要 `verify-port-fix.ps1` 定期检查 + 重新 apply

## 使用方法

### 首次应用（fix 当前 PS 工作坊）
```powershell
cd C:\Users\situj\git\Mount-and-Blade-2-Bannerlord-custome-development\PSPortFix\Tools
.\apply-port-fix.ps1
```

**执行流程**：
1. 备份 6 个 workshop XML 到 `PSPortFix\Baselines\`（含 sha256）
2. 逐文件注入 port location 到所有 town 变体
3. 打印修改统计

### 验证（每次玩游戏前 or 定期）
```powershell
.\verify-port-fix.ps1
```

若 workshop 更新覆盖，此脚本会检测到并提示重新 apply。

### 撤销（万一有问题）
```powershell
.\revert-port-fix.ps1
```

从 `Baselines/` 恢复原始文件。

## 已知限制

- **v1.0 是 workshop 直接编辑方案**：Steam 更新 PS 后需重新 apply
- **只补 port location，不补 shipyard building**：因为 shipyard building 影响 town building tree，避免破坏已有存档。若你希望在 default 变体也能建造船坞，未来 v2.0 可加此选项
- **场景选择**：所有 town 都用对应文化的 `{culture}_shipyard` 场景。此场景对海边 + 河边 town 都工作（vanilla Argoron 是河边帝国 town 也用 `empire_shipyard`）

## 未来 v2.0 计划

- 升级为 Harmony DLL mod：runtime 检测 PS town 缺 port → 动态注入 Location + 自动兼容 workshop 更新
- 或走 upstream：给 PlayerSettlementFixes 作者报 issue 让他修

## 文件结构

```
PSPortFix/
├── README.md                       (本文档)
├── ModuleData/                     (未来 v2.0 mod XML 预留)
├── Baselines/                      (备份原始 XML · apply 后生成)
│   └── {culture}_settlements_templates_default.xml.baseline
└── Tools/
    ├── apply-port-fix.ps1          (应用补丁)
    ├── revert-port-fix.ps1         (撤销)
    └── verify-port-fix.ps1         (校验)
```
