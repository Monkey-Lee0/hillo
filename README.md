# hillo

一个 **杀戮尖塔 2（Slay the Spire 2）** 卡牌 Mod，基于 [BaseLib](https://steamcommunity.com/) 与 Godot / C# 开发。为多个角色卡池新增了 37 张卡牌与若干配套能力（Power）、衍生牌。

Mod 的核心是一套**基于 Step 组合的卡牌框架**：每张卡由若干可复用的「效果单元（HilloStep）」拼装而成，避免为每张卡重复编写 `OnPlay` / `OnUpgrade` / 动态变量 / 悬停提示等样板。

---

## 安装与使用

1. 安装依赖 **[BaseLib](https://steamcommunity.com/) 3.3.1 或更高版本**（本 Mod 的前置，缺少或版本过低将无法加载）。
2. 将 `hillo.zip` 解压到游戏的 `mods` 文件夹下，解压后目录形如 `SlayTheSpire2/mods/hillo/`。
3. 启动游戏，在 Mod 菜单中启用 hillo 与 BaseLib 即可。

> **⚠️ 备注**：部分卡牌 / 能力的图片素材尚未到位，这些卡在游戏内会显示为缺失贴图（不影响其效果与正常游玩）。

---

## 卡牌一览

注：括号内为升级后效果。

### 故障机器人 · Defect

| 卡牌 | 费 | 类型 | 稀有度 | 效果 | 备注 |
|---|---|---|---|---|---|
| 闪电链 ChainLightning | 1(0) | 技能 | 先古 | 生成 3 个闪电充能球，对所有敌人触发所有闪电充能球的被动 1(2) 次。 | 升级自电击  Zap |
| 暗影侵入 UmbralIncursion | 2(1) | 能力 | 先古 | 每回合开始时，获得 1 个充能球栏位，生成 1 个黑暗充能球。 |  |
| 闪电时间 LightningTime | 3(2) | 能力 | 稀有 | 每当你生成一个闪电充能球时，再生成一个。 |  |
| 命令行 Terminal | 1(0) | 技能 · 消耗 | 稀有 | 选一张手牌本场耗能 -1；选抽牌堆一张牌本场耗能 +1。 |  |
| 二进制 Binary | 2(1) | 能力 | 罕见 | 每当你打出一张奇数耗能的牌时，抽一张牌。 |  |
| 冰雹 Hail | 1 | 攻击 | 罕见 | 生成 1 个冰霜充能球，对所有敌人造成本场战斗冰霜球数 2(3) 倍的伤害。 |  |
| 清空内存 CacheClear | 0 | 技能 | 普通 | 获得 3(6) 格挡，将弃牌堆洗入抽牌堆，抽 1 张牌。 |  |
| 故障 Glitch | 1 | 攻击 · 消耗 | 普通 | 造成 3 点伤害 4(5) 次，加入一张修复（+）。 |  |

### 亡灵契约师 · Necrobinder

| 卡牌 | 费 | 类型 | 稀有度 | 效果 |
|---|---|---|---|---|
| 支配时间 TemporalDominion | 2 | 能力 | 稀有 | 每回合结束时，奥斯提死亡，给予所有敌人奥斯提血量上限 ×4(5)层灾厄。 |
| 灵魂重塑 SoulRecall | 0 | 技能 | 稀有 | 将消耗堆的所有灵魂（升级后）加入抽牌堆。消耗。 |
| 诅咒回声 CursedEcho | 2 | 攻击 | 罕见 | 奥斯提造成 9(11) 点伤害 2 次；若目标有灾厄，改为对所有敌人造成伤害。 |
| 厄运当头 ImpendingDoom | 1(0) | 能力 | 罕见 | 敌人的灾厄改为在你的回合结束时触发。 |
| 招魂者 GraveCaller | 1 | 技能 · | 普通 | 召唤 3(5)，给予目标 7(9) 层灾厄。虚无 |
| 鼓励 Flourish | 0 | 技能 | 普通 | 当奥斯提满血时：召唤 3(4)，获得 3(4) 格挡，抽 1 张牌。 |

### 储君 · Regent

| 卡牌 | 费-星 | 类型 | 稀有度 | 效果 | 备注 |
|---|---|---|---|---|---|
| 万剑归宗 MyriadSwords | 2(1) | 能力 | 先古 | 铸造 15。回合结束时，无论何处，打出你的所有君王之剑。 |  |
| 景仰 Revere | 1(0) | 技能 | 先古 | 获得 6 颗星星。 | 升级自崇拜 Venerate |
| 星核聚爆 StarExplode | 3-X | 攻击 | 稀有 | 对所有敌人造成 X²（升级 (X+1)²）点伤害。 |  |
| 星能转化 StarConversion | 2-2 | 能力 | 稀有 | 每消耗 3 能量得 1 星；每消耗 3 星得 1 能量。（固有） |  |
| 星剑 StarSword | 1 | 能力 | 罕见 | 君王之剑耗能减少 1，额外消耗 1 星。（固有） |  |
| 出来！！ Out | 1 | 技能 | 罕见 | 抽 1 张牌；选 2 张手牌，各变为一张随机仆从牌（+）。 |  |
| 猛烈锻打 FerociousForge | 1 | 攻击 | 普通 | 造成 10(12) 伤害，铸造 9(11)，加入一张碎屑。 |  |
| 回收碎屑 RecycleDebris | 1(0) | 技能 | 普通 | 消耗手牌所有碎屑，每张得 1 星。 |  |

### 静默猎手 · Silent（奇巧 / 丢弃 / 变化）

| 卡牌 | 费 | 类型 | 稀有度 | 效果 | 备注 |
|---|---|---|---|---|---|
| 探索者 Explorer | 1 | 技能 | 先古 | 获得 14(17) 格挡；丢弃最多 2(3) 张牌，抽等量牌。 | 升级自生存者 Survivor |
| 奇巧形态 SlyForm | 3 | 能力 | 先古 | 每当你丢弃一张牌，获得 4(6) 格挡；所有牌丢弃后获得奇巧。 |  |
| 万化归刃 Transmute | 2 | 技能 | 稀有 | 选任意张手牌变为小刀（+）。 |  |
| 炼金术 Alchemy | 1 | 技能 | 罕见 | 消耗一张手牌，给予随机敌人 5(7) 层中毒。 |  |
| 疾风步 WindWalk | 1 | 能力 | 罕见 | 你每打出一张攻击牌，本回合获得 1 点敏捷。（固有） |  |
| 随机应变 Improvisation | 1 | 攻击 | 普通 | 造成 8(11) 伤害；敌人意图攻击则给虚弱，否则给易伤。 |  |
| 后撤步 BackStep | 1 | 技能 | 普通 | 丢弃 3 张牌，获得 5(8) 格挡；若丢光手牌则抽 3 张。 |  |
| 胸有成竹 Assurance | 2(1) | 能力 | 稀有 | 每打出一张非奇巧能力牌，弃牌堆加入其 3 费奇巧复制品。 |  |

### 无色 · Colorless

| 卡牌 | 费 | 类型 | 稀有度 | 效果 |
|---|---|---|---|---|
| 盛大返场 GrandEncore | 3(2) | 能力 | 先古 | 回合开始打出 1 张闪亮登场；回合结束打出消耗堆所有闪亮登场。 |
| 放手一搏 AllIn | 3 | 攻击 | 稀有 | 对所有敌人造成 18(21) 伤害 3 次；下回合失去 10 生命。 |
| 恶意 Malice | 3(2) | 能力 | 稀有 | 每打出 1 张牌，等概率给随机敌人 易伤 / 虚弱 / -1 力量。 |
| 完美计划 PerfectPlan | 0 | 技能 · 联机 | 稀有 | 所有玩家抽 1(2) 张牌、获得 1 能量、3(5) 活力、1 缓冲。 |
| 回溯打击 RetraceStrike | 1 | 攻击 | 罕见 | 造成 6(9) 伤害；将弃牌堆一张攻击/技能的消耗复制品加入手牌。 |
| 破碎铠甲 ShatteredArmor | 1 | 技能 | 罕见 | 获得 14(18) 格挡；本回合无法再获得格挡。 |
| 蓄势待发 Primed | 1 | 技能 | 罕见 | 给所有敌人 1(2) 虚弱；下回合获得能量、抽 1 张牌。 |

### 衍生牌 · Token

| 卡牌 | 费 | 类型 | 效果 |
|---|---|---|---|
| 修复 Repair | 1 | 技能 · | 获得 2 格挡 4(5) 次，加入一张故障（+）。消耗 |

---

## 架构：Step 卡牌框架

代码分为两层：

- **`Modules/`** — 通用框架（卡牌基类 + 效果 Step 库）。
- **`Scripts/`** — 具体的卡牌、能力、卡池、Patch。

### `HilloCardModel`（`Modules/Card/Model.cs`）

所有 Mod 卡牌继承它，构造时传入一组 `HilloStep`：

```csharp
public class ChainLightning : HilloCardModel
{
    public ChainLightning() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self,
        [
            new HilloChannelOrbStep<LightningOrb>("Channel", 3),
            new HilloPassiveAllStep<LightningOrb>("Passive", 1, upgradeDiff:1),
            new HilloEnergyUpgradeStep(-1)
        ]) {}
}
```

基类自动完成：`OnPlay`（依次执行各 Step）、`OnUpgrade`（转发给各 Step）、`CanonicalVars` / `CanonicalKeywords` / `CanonicalTags` / `ExtraHoverTips` 的聚合、以及标准卡图路径。构造参数还支持 `keywords:` / `tags:` / `extraHoverTips:` / `autoAdd:`。

### `HilloStep`（`Modules/Step/`）

每个 Step 是一个可复用的效果单元，覆写 `OnStep` / `OnUpgrade` / `GetDynamicVars` / `GetIHoverTips` 中需要的部分。主要 Step：

| 文件 | Step | 作用 |
|---|---|---|
| DamageStep | `HilloDamageAll/Single/RandomStep` | 造成伤害（全体/目标/随机），`DamageVar` |
| BlockStep | `HilloBlockSelfStep` | 获得格挡，`BlockVar` |
| PowerStep | `HilloPowerSelf/All/Single/RandomStep<T>` | 施加能力 T |
| EnergyStep | `HilloGainEnergyStep` / `HilloEnergyUpgradeStep` | 获得能量 / 升级改费 |
| StarStep | `HilloGainStarStep` / `HilloStarUpgradeStep` | 获得星星 / 升级改星费 |
| ForgeStep | `HilloForgeSelfStep` | 铸造 |
| HandStep | `HilloDraw/Discard/Exhaust/Create/Transform/ShuffleDiscard...Step` | 抽牌 / 弃牌 / 消耗 / 生成 / 变化 / 洗牌堆 |
| ChannelStep | `HilloChannelOrb<T>/ChannelRandomStep` | 生成充能球 |
| PassiveStep | `HilloPassiveAll/Single<T>` | 触发充能球被动 |
| OrbStep | `HilloOrbSlotStep` | 增减充能球栏位 |
| OstyStep | `HilloSummonStep` / `HilloOstyAttack*Step` / `HilloOstyDieStep` | 奥斯提召唤 / 攻击 / 死亡 |
| KeywordStep | `HilloAddKeywordUpgradeStep` | 升级时加关键字 |

### 控制流包装 Step

以下静态工厂把一组内层 Step 包装成一个 Step（转发变量/提示/升级，仅改变执行方式）：

- `HilloStep.When(谓词, ...steps)` — 满足条件才执行。
- `HilloStep.AllPlayers(...steps)` — 对所有玩家各执行一遍（联机）。
- `HilloStep.OtherPlayer(选择器, ...steps)` / `HilloStep.TargetPlayer(...steps)` — 对指定/目标玩家执行。

内部通过 `HilloStep.PlayerOverride`（重定向作用玩家）与 `HilloStep.HostCard`（让提示反映当前卡的升级态）实现。

### 卡牌专属逻辑

条件分支、跨牌堆操作、动态数值公式等无法用通用 Step 表达的效果，写成卡牌文件内的 `private class : HilloStep`，就近收敛（例：`Hail`、`Terminal`、`SoulRecall`、`CursedEcho`）。

### 衍生牌

由其它卡生成、不进奖励的牌，标注 `[Pool(typeof(TokenCardPool))]`（例：`Repair`）。缺少卡池归属会在显示时导致崩溃。

---

## 构建

```bash
dotnet build -c Debug
```

`hillo.csproj` 会在构建后把产物复制到 `SlayTheSpire2/mods/hillo/`。需在 `.csproj` 中将 `Sts2Dir` 指向本地游戏安装路径，并配置 BaseLib 引用。本地化文件位于 `hillo/localization/zhs/`。

> 图片资源（卡图 / 能力图标）已在 `.gitignore` 中忽略，不随仓库分发。
