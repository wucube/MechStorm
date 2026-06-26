# MechStorm 战斗系统长期拓展规划

## 记录目的

本文记录从“战斗系统：GameObject / State / Attribute / Modifier / Trigger / Bullet”系列文章中提炼出的长期架构借鉴方向。

当前项目仍处于 P0 最小可玩原型阶段，本文不是近期必须全部实现的任务清单，而是后续扩展战斗系统时的技术路线约束。

本文属于前期规划文档，不是不可变更的架构承诺。后续应根据玩法验证结果、代码复杂度、测试成本、配置需求和团队工具链成熟度持续调整。

## 与轻量 GAS 变种的关系

项目主计划中已经明确采用“轻量级 GAS 变种”作为实体与状态管线方向，包括：

- `GameplayTags` 简版
- `GameplayAttribute`
- `GameplayModifier`
- `BattleResource.*`
- `State.Broken.*`
- `Locomotion.*`
- `Attack.Mode.*`
- `Timing.*`

因此，本文记录的文章思路不用于替代项目原有 GAS 变种规划，而是作为补充参考。

推荐关系如下：

| 职责 | 项目主干采用 | 文章思路提供的补充 |
|------|--------------|--------------------|
| 状态标识 | `GameplayTag` | 用层级标签表达状态、攻击方式、移动能力和时序 |
| 数值中心 | `GameplayAttribute` | 属性集中读取，避免各系统到处查询状态来源 |
| 数值修改 | `GameplayModifier` | Modifier 不拥有生命周期，只修改流程中的值 |
| 持续状态 | 轻量 `BattleState` / `StateComponent` | 借鉴“一切皆 State”，但只覆盖有生命周期的状态 |
| 主动行为 | `Command` / `Ability` / `ActionContext` | 不强行把所有主动技能都塞进 State |
| 条件触发 | `Trigger` / `EventBus` | Trigger 作为扩展点，不替代核心流程 |

当前建议是：

```text
主架构：轻量 GAS 变种
补充思想：State 生命周期统一、Modifier 流程化、Attribute 集中读取、Trigger 扩展点
不采用：完整 UE GAS、不采用完整“一切皆 State”框架
```

也就是说：

- 有生命周期的控制、破坏、过热、护盾、警戒等效果，可以逐步进入轻量 `State`。
- 主动行动仍优先用 `Command / Ability / ActionContext` 表达。
- 最终数值和功能开关通过 `GameplayAttribute` 读取。
- 分类、条件和状态标识通过 `GameplayTag` 表达。
- Modifier 作为属性或流程修改器，不直接承担生命周期。
- Trigger 用于反击、警戒、地形、回合开始等扩展点，但核心移动、攻击、回合推进仍保持显式流程。

## 核心决策

MechStorm 后续战斗系统应保持以下方向：

1. **纯逻辑优先**：战斗规则继续放在 `MechStorm.Battle`，不依赖 Unity。
2. **显式流程优先**：移动、攻击、回合推进等核心流程保持显式调用，不用事件系统替代主流程。
3. **逐步组件化**：当 `CombatUnit` 职责膨胀时，再逐步拆出属性、状态、行动等组件。
4. **状态统一建模**：眩晕、过热、护盾、死亡、行动结束、警戒等持续性效果，后续优先进入轻量 `State` 系统。
5. **属性集中读取**：HP、AP、移动力、攻击、防御、热量、护盾、功能禁用等最终结果，后续逐步收敛到统一属性中心。
6. **上下文承载流程**：伤害、命中、移动、技能释放等一次性流程，后续用 Context 承载输入、中间值和结果。
7. **Modifier / Trigger 后置**：修改器和触发器作为中后期扩展能力，不在 P0 阶段提前完整实现。
8. **数据驱动渐进引入**：先用简单配置满足玩法验证，技能数量和规则复杂度上来后，再引入更强的数据驱动或编辑器工具。

## 分阶段路线

### P0：最小可玩原型

目标是验证战棋机甲基础循环：

- 方格拓扑
- 移动范围
- 单位显示
- 点击移动
- 基础攻击
- 回合切换

此阶段应避免引入完整 `State / Modifier / Trigger / Bullet` 框架。

允许保留简单直接的实现，只要核心逻辑可测试、表现层与逻辑层边界清晰。

### P1：轻量战斗状态与行动流程

当基础闭环跑通后，可以引入：

- `BattleAction` / `Command`：统一移动、攻击、结束回合、使用技能
- `ActionResult`：承载逻辑结果，供表现层刷新
- 轻量 `StateComponent`：支持添加、移除、查询、回合开始/结束 Tick
- 基础状态：死亡、眩晕、行动结束、过热、护盾、警戒

重点是让战斗流程更可组合，但仍保持简单。

### P2：属性中心与流程上下文

当单位数据和规则开始变多后，逐步引入：

- `BattleAttributeSet`
- `DamageContext`
- `AttackContext`
- `MoveContext`
- 属性变化结果记录
- 战斗日志或调试输出

此阶段目标是减少规则散落，方便后续 AI、UI、回放、调试读取同一套结果。

### P3：Modifier 与 Trigger

当装备、机甲部件、天赋、地形、状态开始影响战斗流程时，再引入：

- `AttributeModifier`
- `DamageModifier`
- `HitModifier`
- `MoveCostModifier`
- 回合触发器
- 进入格子触发器
- 受击触发器
- 警戒/反击触发器

核心原则：

- Modifier 只修改流程中的值，不拥有生命周期。
- State / 装备 / 地形可以提供 Modifier。
- Trigger 作为扩展点，不替代核心流程。

### P4：技能、子弹与复杂数据驱动

当项目需要复杂技能和投射物时，再考虑：

- 技能配置表
- 技能释放上下文
- 技能状态或技能脚本
- 独立生命周期的 Bullet
- 可拦截、延迟落点、追踪导弹、无人机等实体化效果
- 更强的数据驱动或可视化编辑器

普通瞬发攻击不应提前实体化为 Bullet。只有具备独立生命周期和交互需求的效果才需要 Bullet。

## 不应提前照搬的内容

P0 / P1 阶段不建议提前实现：

- 完整“一切皆 State”框架
- 完整 Modifier 表体系
- 复杂技能编辑器
- 行为树技能系统
- 大型互斥图
- 完整子弹框架
- 所有流程事件化
- 复杂配置 DSL

这些能力适合在玩法复杂度真实出现后再扩展。

## 当前开发约束

1. `MechStorm.Battle` 不引用 `UnityEngine`。
2. `MechStorm.Presentation` 只负责 Unity 表现、输入适配和 View 同步。
3. 新增战斗规则优先补 EditMode 测试。
4. P0 期间不为了架构完整性牺牲可玩闭环推进速度。
5. 每次引入新抽象前，应先确认已有代码出现重复、职责膨胀或测试困难。

## 后续必须补齐的设计约束

以下内容不一定在 P0 实现，但在进入 P1 / P2 前应逐步明确，否则轻量 GAS 变种会在后期变得难以维护。

### 术语边界

需要明确项目内这些概念的职责边界：

| 术语 | 建议含义 |
|------|----------|
| `GameplayTag` | 状态标识、分类、条件匹配，不直接承载生命周期 |
| `BattleState` | 有生命周期的状态对象，例如眩晕、过热、护盾、破坏、警戒 |
| `GameplayAttribute` | 可查询的最终数值或功能开关 |
| `GameplayModifier` | 对属性或流程值的修改器，不拥有生命周期 |
| `Trigger` | 条件满足后的扩展执行点 |
| `Command / Ability` | 主动行动入口，例如移动、攻击、技能、结束回合 |
| `Context` | 一次流程的上下文，例如攻击、伤害、移动、技能释放 |

尤其要避免混淆：

- `Tag` 不是 `State`，Tag 只是标识或条件。
- `Modifier` 不是 `Buff`，Modifier 只修改值。
- `State` 可以提供 Tag 或 Modifier，但 State 本身负责生命周期。
- 主动技能不强制等于 State，复杂技能可以由 Ability / Command / Context 驱动。

### Tag 注册与命名规范

`GameplayTag` 应尽早形成命名规则，避免随手字符串扩散。

建议采用层级命名：

```text
State.Broken.Leg
State.Broken.Arm
State.Control.Stun
State.Control.Root
Locomotion.Walk
Locomotion.Hover
Locomotion.Jump
Attack.Mode.Active
Attack.Mode.Reactive
Attack.Mode.Duel
Timing.Turn.Start
Timing.Action.Before
Timing.Hit.After
BattleResource.ActionPoint
BattleResource.PotentialPoint
```

后续应考虑集中注册或生成，避免运行时拼接未知 Tag。

### Attribute 重算策略

进入 `GameplayAttribute` 阶段前，需要明确属性值如何计算：

- 增量修改：添加 Modifier 时修改，移除时回滚。
- 脏标记重算：Modifier 变化时标脏，查询时重新计算。
- 分层缓存：局外静态层 + 局内动态层。

当前更推荐预留脏标记接口，P2 先做简单重算，P3 在 AI 大量读取属性时再做两层缓存。

还需要保留属性来源追踪能力，例如：

```text
MoveRange = 4
  Base: 3
  LegModule: +1
  State.Broken.Leg: Override 1
```

否则后期很难解释“为什么这个单位当前移动力是 1”。

### Modifier 计算顺序

Modifier 必须有明确计算顺序，否则数值不可预测。

建议先采用简单层级：

```text
Base
Add
Multiply
Override
Clamp
FinalModify
```

P0 / P1 可以不实现完整系统，但一旦进入 P2，就要明确：

- 同层 Modifier 的合并规则
- Override 之间的优先级
- Clamp 的上下限来源
- Modifier 来源记录
- Modifier 生效时机

### 调试工具需求

GAS 类架构没有调试工具会很难排查问题。后期至少需要能查看：

- 单位当前 `GameplayTag` 列表
- 单位当前 `BattleState` 列表
- 属性当前值和来源
- 生效中的 Modifier
- 最近触发的 Trigger
- 最近一次 Attack / Damage / Move Context
- 回合内 Command 记录

这些可以先从日志开始，不必一开始做完整 Editor 面板。

### 确定性、悔棋与回放边界

如果未来要支持悔棋、续战、录像或战斗复现，需要尽早保证：

- 行动通过 `Command` 表达。
- 随机数通过 `BattleRNG` 管理。
- 每次行动的输入、结果、影响目标可记录。
- 关键流程不依赖 Unity 时间、帧率或场景对象。
- `Context` 不保存不可序列化对象。

这部分不要求 P0 完成，但 P0 以后新增系统时要避免破坏确定性。

### 回放技术选型

回放要区分两种目标：

```text
复现战斗结果：重新跑战斗逻辑，要求确定性。
再现战斗表现：按权威结果播放，不要求客户端重新算出同样结果。
```

因此后续回放建议分层设计：

| 类型 | 数据内容 | 适用场景 | 特点 |
|------|----------|----------|------|
| Command Replay | 初始快照 + Command 序列 + RNG 种子/游标 | 本地回放、开发调试、确定性验证 | 数据小，但要求逻辑、配置和 RNG 完全一致 |
| Result Replay | 初始快照 + 每步权威结果日志 | PVP 回放、分享回放、排行榜、防作弊 | 更稳，不依赖观看端重新结算，数据更大 |
| 混合日志 | Command + 权威 Result + 关键表现事件 | 长期推荐方案 | 既能追踪玩家意图，又能按权威结果稳定播放 |

PVP 和可分享回放更推荐使用“Command + 权威 Result 混合日志”：

```text
客户端提交 CommandIntent
-> 服务端校验并结算
-> 服务端记录 Command + CommandResult / BattleResult
-> 分享时其他客户端下载权威 BattleLog
-> 本地按 Result 播放表现
```

PVE 如果需要排行榜、防作弊、分享回放，也应尽量上传服务器认可的 BattleLog，而不是只分享本地录像文件。

本地调试和单机复盘可以继续使用 Command Replay，但它不能替代权威分享回放。

### 整数化战斗逻辑

PVP、悔棋、续战、录像和服务端权威结算都会要求战斗结果可复现、可校验。

因此，战斗逻辑层应优先使用整数表达规则：

- 坐标：整数格子坐标。
- HP / AP / 护盾 / 热量：整数。
- 概率：整数万分比，例如 `7500 = 75%`。
- 倍率：整数缩放值，例如 `15000 = 1.5 倍`。
- 减免：整数万分比，例如 `6600 = 66%`。
- 固伤率：整数万分比，例如 `1020 = 10.2%`。

浮点数只用于表现层动画、插值、相机、特效和 UI 过渡，不参与核心战斗结算。

使用整数逻辑的原因：

1. **结果确定**：不同平台、不同运行环境不容易出现浮点误差。
2. **便于服务端校验**：PVP 中客户端只提交指令，服务端用同一套整数规则校验并结算。
3. **便于回放复现**：Command + RNG + 整数公式更容易重放出完全一致的结果。
4. **便于配置解释**：策划配置 `40%`、`66%`、`10.2%` 时，可以精确映射为整数比例。
5. **便于取整约束**：所有出现非整数结果的地方都必须声明取整策略，避免隐藏规则。

示例：

```text
基础子弹数 = 18
子弹数加成 = 40% = 4000
最终子弹数 = Ceil(18 * (10000 + 4000) / 10000) = 26
```

```text
基础固伤率 = 30% = 3000
Boss 固伤减免 = 66% = 6600
最终固伤率 = 3000 * (10000 - 6600) / 10000 = 1020
1020 = 10.2%
最终伤害 = Floor(目标最大 HP * 1020 / 10000)
```

需要明确的规则：

- 每个公式的取整时机。
- 每个公式的取整方式：`Floor`、`Ceil`、`Round`、`Trunc`。
- 是否存在最低值，例如最低 1 点伤害、最低 1 点 AP 消耗。
- 中间比例是否保留缩放整数，通常推荐中间不提前取整，最终落到离散值时再取整。

完整定点数库暂时不是 P0 必需项。只有当后期出现实时 Lockstep、连续坐标、弹道物理、复杂曲线结果参与战斗逻辑时，才重新评估是否引入定点数库。

### 事件边界

事件系统只作为扩展点，不替代核心流程。

应保持显式流程：

```text
Validate -> Execute -> ApplyResult -> Notify
```

不推荐把移动、攻击、扣血、死亡这类主流程拆成一串无序事件监听。

推荐事件用途：

- UI 刷新通知
- 战斗日志
- 触发器扩展点
- 成就/任务统计
- 表现层播放提示

核心规则：

```text
主流程显式，扩展点事件化。
```
