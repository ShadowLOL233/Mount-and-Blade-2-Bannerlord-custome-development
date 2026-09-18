# Mount & Blade II: Bannerlord — Custom Development Notes

个人骑马与砍杀 II: Bannerlord 模组诊断与定制开发日志。

## 内容

- **[`ModdingJournal.md`](./ModdingJournal.md)** — 完整开发日志。包含：
  - 环境与路径（游戏版本、模组安装位置、junction 化 Documents 目录、Steam beta 分支锁）
  - 已启用/禁用模组清单（20+ 项）
  - 已做的定制修改（Junction 化、PSR 调整、Garrison Drills DLL 补丁、IG 食物 getter 补丁、RBM 食物经济堆叠）
  - Bug 历史与修复（战后卡死、存档消失、进战即崩、UI 找不到设置、fief 食物赤字）
  - 模组间交互与已知风险
  - 备份与回滚
  - 待办 / 调查项目
  - 调试参考

## 环境快照

- Bannerlord Native v1.4.7（Steam beta 锁 `v1.4.7`）
- 未装 BLSE，原生启动器
- 主要 mods：Harmony / ButterLib / UIExtenderEx / MCM / RBM 4.5 / Retinues / PartySizeReunited / ImprovedGarrisons / GarrisonDrills / PlayerSettlement / RTSCamera / ChooseYourTroops 等
- 游戏语言：English

## 工具链

- **反编译 mod DLL**：`dnSpyEx`（`C:\Users\situj\Desktop\dnSpy-net-win64\dnSpy.exe`）
- **字节级 DLL patch**：PowerShell `[IO.File]::ReadAllBytes` / `WriteAllBytes`（前提是关闭 launcher 解除文件锁）
- **配置编辑**：`Configs\RBM\config.xml`、`Configs\ImprovedGarrisons\Saves\*.xml`、`Configs\ModSettings\PartySizeReunited\*.json`

## 备份约定

所有修改前的原文件都以 `.bak-<日期>` 或 `.orig-<描述>-<日期>` 后缀备份在原位置，便于回滚。备份清单见 `ModdingJournal.md` 的"备份与回滚"章节。

## 更新方式

**权威源**：`E:\Bannerlord-UserData\ModdingJournal.md`（游戏用户目录下的活文档，日常直接编辑这里）

**同步到 GitHub**：仓库根目录下的 [`sync-journal.ps1`](./sync-journal.ps1) 一键脚本：

```powershell
# 默认提交信息（"sync journal"）
.\sync-journal.ps1

# 自定义提交信息
.\sync-journal.ps1 -Message "add RBM food economy findings"
```

脚本会做三件事：
1. 从 `E:\Bannerlord-UserData\ModdingJournal.md` 拷贝到本仓库
2. 检查是否有实际改动（没改动就跳过 commit）
3. `git add` + `git commit` + `git push origin main`

无改动时不会造 empty commit，无副作用可以随时跑。
