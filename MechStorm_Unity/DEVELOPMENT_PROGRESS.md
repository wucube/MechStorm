# MechStorm 开发进度

## 当前阶段

- 当前里程碑：P1 / Sprint 2
- 当前任务：Task 2.1 战斗流程控制器规划
- 当前状态：待开始
- 最后更新：2026-06-28

## 状态约定

- `[ ]` 未开始
- `[~]` 进行中
- `[x]` 已完成
- `[!]` 阻塞
- `[?]` 待决策

## P0 / Sprint 1：最小可玩原型

- [x] Task 1.0 建立 `MechStorm.Battle` / `MechStorm.Presentation` asmdef 边界
  - 状态：已完成
  - 完成标准：`MechStorm.Battle` 不引用 `UnityEngine`，`MechStorm.Presentation` 可引用 Battle
  - 验证方式：已检查 `MechStorm.Battle.asmdef` 的 `noEngineReferences: true` 与 `MechStorm.Presentation.asmdef` 引用 Battle；`dotnet msbuild MechStorm.Battle.Tests.csproj /t:Build /p:Configuration=Debug /verbosity:minimal` 通过
  - 备注：对应历史提交 `d612f9a Establish MechStorm battle assembly boundary`

- [x] Task 1.1 基础数据结构：`Vector2Int`、`MechPart`
  - 状态：已完成
  - 完成标准：纯 C# 结构体与枚举位于 `MechStorm.Battle`，不依赖 Unity
  - 验证方式：`dotnet build MechStorm.Battle.csproj --no-restore` 通过；确认 Battle 目录无 `UnityEngine` 引用
  - 备注：项目决策使用 `MechStorm.Battle.Foundation.Vector2Int`；表现层如需 Unity 类型，显式使用 `UnityEngine.Vector2Int`

- [x] Task 1.2 简单方格拓扑：`SquareGrid`
  - 状态：已完成
  - 完成标准：提供邻居查询与曼哈顿距离
  - 验证方式：`dotnet msbuild MechStorm.Battle.Tests.csproj /t:Build /p:Configuration=Debug /verbosity:minimal` 通过；EditMode 测试用例已覆盖边界、邻居数量、曼哈顿距离
  - 备注：已添加 `Vector2IntTests` 与 `SquareGridTests`；已修复非法尺寸校验、邻居集合容量、曼哈顿距离绝对值；后续可在 Unity Test Runner 实跑确认

- [x] Task 1.3 基础 BFS 移动范围
  - 状态：已完成
  - 完成标准：给定移动力返回可到达格子集合
  - 验证方式：`dotnet msbuild MechStorm.Battle.Tests.csproj /t:Build /p:Configuration=Debug /verbosity:minimal` 通过；Unity Test Runner EditMode 测试已通过
  - 备注：已补 `GetReachablePositions` 与测试，当前仅支持等代价四方向移动，不含障碍、地形 Cost、ZOC、A*；BFS 依赖 Queue 先进先出保证近距离格子先处理，首次访问即为最短步数

- [x] Task 1.4 实体壳结构：`PilotData` / `MechData` / `CombatUnit`
  - 状态：已完成
  - 完成标准：字段归属正确，AP 挂 Pilot，HP/攻击/移动挂 Mech
  - 验证方式：`dotnet msbuild MechStorm.Battle.Tests.csproj /t:Build /p:Configuration=Debug /verbosity:minimal` 通过；Unity Test Runner EditMode 测试已通过
  - 备注：已实现 `PilotData`、`MechData`、`CombatUnit`；`Position` 通过 `MoveTo` 显式变更，后续接入移动校验与 Command

- [x] Task 1.5 `TurnStateMachine`
  - 状态：已完成
  - 完成标准：玩家回合 → 敌方回合 → 循环
  - 验证方式：`dotnet msbuild MechStorm.Battle.Tests.csproj /t:Build /p:Configuration=Debug /verbosity:minimal` 通过；Unity Test Runner EditMode 测试已通过
  - 备注：已由开发者实现初版，AI 做 Review 与小修；P0 暂不接入行动配额、事件系统、AI 决策与 TEngine Procedure；当前默认玩家先手仅服务单人 P0 原型，后续如需敌人先手可给构造函数增加初始阶段参数

- [x] Task 1.6 基础攻击结算
  - 状态：已完成
  - 完成标准：相邻格可攻击，固定伤害，HP 归零死亡
  - 验证方式：`dotnet msbuild MechStorm.Battle.Tests.csproj /t:Build /p:Configuration=Debug /verbosity:minimal` 通过；Unity Test Runner EditMode 测试已通过
  - 备注：已拆分 `PilotData` / `PilotRuntime`、`MechData` / `MechRuntime`；`MechRuntime` 已支持扣耐久、耐久归零与销毁状态；`AttackResolver` 已支持相邻格固定伤害与非相邻异常；相邻距离使用命名常量表达业务含义

- [x] Task 1.7 最简 Unity 表现层
  - 状态：已完成
  - 完成标准：完成 1.7.1~1.7.7 子任务后，能在 Unity 中看到格子、单位、点击移动与血条变化
  - 验证方式：Unity Play Mode 手动验证；纯 C# 辅助逻辑优先补 EditMode 测试
  - 备注：进入表现层前已补 `CombatUnitFactory`，用于由 `PilotData` / `MechData` / 初始位置创建逻辑战斗单位，并自动初始化独立的 `PilotRuntime` / `MechRuntime`；相关 EditMode 测试已通过；表现层保持在 `MechStorm.Presentation`，不要让 Battle 引用 Unity

- [x] Task 1.7.1 坐标转换器：`GridCoordinateConverter`
  - 状态：已完成
  - 完成标准：支持 `Battle.Foundation.Vector2Int` 与 `UnityEngine.Vector3` 双向转换，包含 `cellSize`、`origin`
  - 验证方式：`dotnet msbuild MechStorm.Presentation.csproj /t:Build /p:Configuration=Debug /verbosity:minimal` 通过；Unity Test Runner Presentation EditMode 测试已通过
  - 备注：这是 Presentation 适配层，不能放进 Battle；`origin` 使用 `UnityEngine.Vector3` 表示整个网格左下角世界坐标，`cellSize` 表示单格 Unity 世界尺寸，后续需抽到外部作为转换器与棋盘渲染器共享的公共参数；`GridToWorld` 返回格子中心点，`WorldToGrid` 返回世界点所在格子；注意避免与 `UnityEngine.Vector2Int` 命名冲突

- [x] Task 1.7.2 棋盘显示：`BattleBoardRenderer` / `CellView`
  - 状态：已完成
  - 完成标准：根据 `SquareGrid` 生成可见格子地面，P0 可使用薄 Cube 占位
  - 验证方式：Unity Play Mode 手动验证格子数量、位置、尺寸正确；已确认棋盘坐标与 `GridCoordinateConverter` 对齐
  - 备注：先不做复杂材质、地图资源、地形 Cost 与障碍；P0 可先用一个缩放后的 Plane 或薄 Cube 表示棋盘，`CellView` 可选；TODO：将 `cellSize` / `origin` 抽到外部公共配置或启动参数，供 `GridCoordinateConverter` 与 `BattleBoardRenderer` 共同使用

- [x] Task 1.7.3 单位显示：`CombatUnitVisual`
  - 状态：已完成
  - 完成标准：用 Cube 显示 `CombatUnit`，能根据 `CombatUnit.Position` 同步世界坐标
  - 验证方式：Unity Play Mode 手动验证单位出现在对应格子；已确认运行时测试通过
  - 备注：一个 `CombatUnitVisual` 只管理一个 `CombatUnit` 的表现，可持有单位引用和自身 `Transform`；Visual 只同步位置、颜色、选中态与后续血条挂点，不判断移动是否合法，也不写战斗规则

- [x] Task 1.7.4 输入与射线检测：`BattleBoardInputter`
  - 状态：已完成
  - 完成标准：鼠标点击通过 `Physics.Raycast` 得到格子世界坐标，并转换为 Battle 网格坐标
  - 验证方式：Unity Play Mode 手动验证点击不同格子能得到正确坐标；已确认点击有日志输出
  - 备注：射线检测属于 Presentation；逻辑层只接收转换后的网格坐标；该控制器只负责输入采集与坐标转换，不选择单位、不执行移动；当前命名为 `BattleBoardInputter`

- [x] Task 1.7.5 移动规则控制器：`MovementResolver`
  - 状态：已完成
  - 完成标准：输入 `CombatUnit`、目标格与 `SquareGrid` 后，能判断目标格是否在移动范围内；合法时调用 `CombatUnit.MoveTo(target)`，非法时保持原位置
  - 验证方式：EditMode 测试覆盖可达移动、不可达不移动、越界不移动、原地移动、空单位与空网格；已确认测试用例全绿
  - 备注：这是移动规则入口，放在 Battle 逻辑层或纯 C# 可测试层；P0 复用现有 BFS 移动范围，不做路径动画、A*、障碍、地形 Cost 和单位占用表；命名采用 `MovementResolver`，与 `AttackResolver` 保持一致

- [x] Task 1.7.6 表现层编排器：`BattlePresentationController`
  - 状态：已完成
  - 完成标准：串联单位选择、格子点击、移动规则控制器与 `CombatUnitVisual` 刷新，形成“选单位 → 点格子 → 合法移动 → 表现同步”的闭环
  - 验证方式：Unity Play Mode 手动验证单位能移动到可达格子，不可达格子不移动；已确认验收通过
  - 备注：P0 先用单个玩家单位；该类只做流程编排，不直接实现移动规则，也不直接处理射线细节；移动插值、朝向变化和可移动范围高亮暂不纳入本任务

- [x] Task 1.7.7 最简血条：`UnitHealthBarView`
  - 状态：已完成
  - 完成标准：显示 `CurrentDurability / MaxDurability`，攻击或扣血后可刷新
  - 验证方式：Unity Play Mode 手动验证血条能正常创建并显示
  - 备注：当前采用程序化创建的 World Space Canvas，挂在单位 `HealthBarAnchor` 下；显示血条背景、填充条与 `current/max` 文本；暂不接 TEngine UIWindow、Prefab 动态加载和对象池

## 坐标系统约定

- Battle 逻辑层使用 `MechStorm.Battle.Foundation.Vector2Int` 表示平面格子坐标。
- `Vector2Int.X` 映射到 Unity 世界坐标 `x`。
- `Vector2Int.Y` 映射到 Unity 世界坐标 `z`。
- Unity 世界坐标 `y` 只表示表现高度，不进入当前 P0 战斗逻辑坐标。
- `origin` 表示整张棋盘左下角边界点，不是 `Grid(0,0)` 的中心。
- `GridCoordinateConverter.GridToWorld` 返回格子中心点。
- 当 `origin = (0,0,0)` 且 `cellSize = 1` 时，`Grid(0,0)` 的中心是 `(0.5,0,0.5)`，`Grid(1,1)` 的中心是 `(1.5,0,1.5)`。
- `WorldToGrid` 会把世界点转换为其所在格子；落在边界上的点按 `Mathf.FloorToInt` 规则归入右侧或上侧格子。
- `CombatUnit.Position` 暂时保持二维格子坐标；`CombatUnitVisual` 通过额外高度偏移处理 Cube 或后续模型的离地高度。

## P1 / Sprint 2~3：核心机制深化

- [ ] Sprint 2：实体与战斗深化
  - 状态：待开始
  - 总目标：从单单位移动演示升级为最小战斗流程，支持多个单位、当前行动单位、移动 / 普通攻击、扣血、死亡、回合推进与表现同步
  - 完成标准：场上至少有 PlayerA 与 EnemyA；玩家可移动、攻击相邻敌人；敌人扣血后血条刷新；HP 为 0 后不再行动；回合可从 Player 切到 Enemy 再切回 Player
  - 备注：继续使用 `TempGameEntry` 作为测试入口，不立即接 TEngine 正式流程；先把 Battle 纯 C# 核心机制跑通

- [ ] Task 2.1 战斗流程控制器：`BattleController` / `BattleSession`
  - 状态：未开始
  - 完成标准：集中管理棋盘、单位列表、当前回合、当前行动方与当前行动单位；提供移动、攻击、结束行动等入口
  - 验证方式：EditMode 测试覆盖初始化、当前行动单位、行动结果与基础异常输入
  - 备注：不要让 `TempGameEntry` 继续直接持有全部战斗规则；`TempGameEntry` 只创建测试战斗并同步表现

- [ ] Task 2.2 多单位与阵营管理
  - 状态：未开始
  - 完成标准：支持 Player / Enemy 至少两个阵营；能查询存活单位、死亡单位、当前阵营单位；死亡单位不能再行动
  - 验证方式：EditMode 测试覆盖阵营查询、死亡跳过与所有单位死亡判断

- [ ] Task 2.3 行动流程与回合推进：`TurnCoordinator`
  - 状态：未开始
  - 完成标准：单位行动后能进入下一个可行动单位；阵营内所有可行动单位完成后切换阵营；新回合重置行动状态
  - 验证方式：EditMode 测试覆盖行动完成、死亡跳过、阵营切换与回合循环

- [ ] Task 2.4 普通攻击接入战斗流程
  - 状态：未开始
  - 完成标准：通过 `BattleController` 调用 `AttackResolver`；攻击后目标扣耐久，目标血条刷新；HP 归零后进入死亡状态
  - 验证方式：EditMode 测试覆盖相邻攻击、非相邻失败、扣血、死亡；Unity Play Mode 手动验证血条刷新

- [ ] Task 2.5 最小战斗输入
  - 状态：未开始
  - 完成标准：支持点击当前单位、点击格子移动、点击敌方单位普通攻击，以及最小结束回合入口
  - 验证方式：Unity Play Mode 手动验证“选单位 → 移动 / 攻击 → 回合推进”的最小闭环
  - 备注：先不做技能栏、部署拖拽、移动范围高亮和正式 TEngine `BattleMainUI`

- [ ] Task 2.6 战斗结果通知
  - 状态：未开始
  - 完成标准：移动、攻击、扣血、死亡、回合开始 / 结束等行为能以结果对象或本地事件形式返回给表现层
  - 验证方式：EditMode 测试覆盖结果内容；表现层根据结果刷新单位位置、血条和日志
  - 备注：P1 先使用纯 C# 结果对象或本地事件；不急着接 TEngine `GameEvent`

- [ ] Task 2.7 轻量战斗快照与调试导出
  - 状态：未开始
  - 完成标准：可从纯 Battle 逻辑层导出 `BattleSnapshot` 与关键 `BattleActionLog`；失败测试或手动调试时能输出 JSON 诊断数据，用于离线分析战斗状态
  - 验证方式：EditMode 测试构造一段移动 / 攻击 / 回合推进流程，导出的快照包含回合、当前行动方、当前单位、棋盘、单位列表、位置、HP、死亡状态、行动状态与关键行为记录
  - 备注：排在 Task 2.6 之后，因为它依赖 `BattleController`、多单位管理、回合推进和结果通知；Sprint 2 只做轻量调试导出，不做完整录像、悔棋、正式回放 UI 或权威回放系统

- [ ] Sprint 3：空间、攻击范围与技能雏形
  - 状态：未开始
  - 总目标：在普通移动 / 普通攻击稳定后，再引入攻击范围、武器范围、地形 / 占格规则与最小主动技能
  - 备注：技能可以比 Buff 早引入；Buff / Modifier / Trigger 不在 Sprint 3 初期展开

## P2 / Sprint 4~5：工程化与解耦

- [x] TEngine 框架接入
  - 状态：已完成
  - 完成标准：项目已接入 TEngine 框架及依赖库
  - 验证方式：历史提交记录确认
  - 备注：对应历史提交 `dbd147e 接入 TEngine 框架以及依赖库`；此项虽在原主计划 P2，但当前项目已前置完成

- [ ] Sprint 4：TEngine 接入 + 架构重构
  - 备注：PVP / 多阵营 / 敌方先手等规则应拆为 `InitiativeResolver` 生成初始行动顺序，`TurnStateMachine` 只负责按顺序推进；随机先手、速度先手、剧情指定先手都属于先手规则，不属于状态机本体
- [ ] Sprint 5：数据与回溯

## P3 / Sprint 6+：扩展与商业化

- [ ] 背包词条、元件系统、天赋树、Utility AI、HybridCLR、YooAsset、状态透视仪、养成、录像

## 决策记录

| 日期 | 决策 | 原因 | 影响 |
|------|------|------|------|
| 2026-06-13 | 使用独立进度文件，不直接修改主计划 | 主计划保持蓝图稳定，进度文件记录执行状态 | 新会话需读取 `AGENT.md`、主计划、进度文件 |
| 2026-06-15 | Battle 核心坐标类型命名为 `Vector2Int`，不改为 `SVector2Int` | 保持简洁；若表现层需要 Unity 类型，则显式写 `UnityEngine.Vector2Int` | Battle 内默认使用 `MechStorm.Battle.Foundation.Vector2Int`，跨层代码需避免省略命名空间导致歧义 |
| 2026-06-17 | AI 新建 Unity 文件或目录时不手写 `.meta` 文件 | `.meta` 应由 Unity Editor 自动生成，避免 GUID / 导入配置不一致 | AI 可创建源码文件，但 `.meta` 等用户打开 Unity 后自动生成，再由 AI 检查是否一并提交 |
| 2026-06-18 | 采用 `Data + Runtime` 命名区分静态数据与运行时数据 | `Data` 表示静态基础数据，`Runtime` 表示某场战斗中会变化的实例状态 | `PilotData` / `MechData` 不承载当前值，当前 AP / 当前耐久放入 `PilotRuntime` / `MechRuntime` |
| 2026-06-18 | 业务含义数字使用命名常量，坐标方向字面值集中管理 | `1` 在攻击距离中表示“相邻格”，属于业务规则；方向向量中的 `1/-1` 表示坐标单位，可集中在方向数组中 | `AttackResolver` 使用局部常量表达相邻距离；后续若引入武器/技能范围，再迁移到 `Data` 配置 |
| 2026-06-26 | 战斗系统长期架构以轻量 GAS 变种为主干，借鉴 `GameObject + State + Attribute + Modifier + Trigger` 思路作为补充，按阶段渐进引入 | 该思路适合支撑后续机甲状态、装备、地形、触发器与复杂技能扩展；当前 P0 不应提前完整照搬，且该规划属于前期方案，后续可随玩法验证调整 | 新增 `BATTLE_ARCHITECTURE_ROADMAP.md` 作为长期拓展规划；P0 继续优先完成可玩闭环，后续按复杂度逐步引入 GameplayTag、GameplayAttribute、轻量 State、流程上下文、Modifier 与 Trigger |
| 2026-06-26 | 战斗逻辑层优先采用整数化数值规则，浮点只用于表现层 | PVP 服务端权威结算、悔棋、续战、录像和回放都需要确定性；整数化概率、倍率、减免和取整策略更容易复现与校验 | 百分比、倍率、固伤率等用万分比或整数缩放表达；所有非整数结果必须声明取整时机和方式；定点数库暂不作为 P0 必需项，后续如出现实时 Lockstep 或连续物理逻辑再评估 |
| 2026-06-27 | AI 默认只提供设计思路、职责划分、接口建议与验证方式，不直接修改代码 | 当前阶段开发者希望先自行实现代码，AI 作为方案讨论与校验辅助，避免未确认方案被提前写入项目 | 除非开发者明确要求“帮我实现 / 帮我改 / 提交”，AI 不主动编辑代码；若涉及文档记录、进度同步或提交，也应先确认意图 |
| 2026-06-28 | 阶段完成后打版本标签，后续功能通过临时分支开发 | P0 / Sprint 1 已形成可运行小版本，需要稳定主线并方便回溯；后续直接在 `main` 开发风险较高 | 当前版本建议标记为 `v0.1.0-p0-sprint1`；后续从 `main` 新建 `feature/<阶段或功能名>` 分支开发，验证通过后再合并回 `main` |
| 2026-06-28 | P1 / Sprint 2 继续使用临时入口，不立即接 TEngine 正式流程 | Sprint 2 的重点是 Battle 纯 C# 核心机制，过早接 Procedure、BattleModule、正式 UI 和资源加载会增加干扰 | `TempGameEntry` 继续作为测试入口；TEngine 正式流程、BattleMainUI、Prefab 动态加载与对象池放到 P2 或机制稳定后处理 |
| 2026-06-28 | Buff / Modifier / Trigger 后置，技能雏形可在普通攻击稳定后引入 | Buff 会牵扯持续时间、叠层、触发时机、属性修正、驱散和免疫，过早引入会让基础战斗流程复杂化 | Sprint 2 不做 Buff / 技能；Sprint 3 可做最小主动技能；P2 再引入 Buff、Modifier、Trigger 与轻量 GAS 变种 |
| 2026-06-28 | 输入 UI 分阶段接入，先做最小战斗输入 | 技能栏、目标选择、部署拖拽和正式 UI 都依赖稳定的战斗流程和单位管理 | Sprint 2 只做点击单位、点击格子移动、点击敌方普通攻击与结束回合；Sprint 3 再做技能按钮和目标选择；部署拖拽单独作为 BattlePreparation / Deployment 阶段 |
| 2026-06-28 | 轻量战斗快照与调试导出排入 Sprint 2 后段 | 离线调试需要依赖 BattleController、多单位、回合推进和结果通知，太早做会缺少稳定数据源；但等到 P2 再做会降低后续逻辑调试效率 | Sprint 2 在 Task 2.6 之后追加 Task 2.7，只做 `BattleSnapshot`、关键 `BattleActionLog` 和 JSON 诊断导出；完整 Command Replay、Result Replay、悔棋和录像仍放到 Sprint 5 / P2 |

## Git 版本管理约定

- `main` 保持可运行、可回退的稳定版本。
- 每完成一个阶段性小版本，在 `main` 当前提交上创建 annotated tag。
- P0 / Sprint 1 建议标签：`v0.1.0-p0-sprint1`。
- 后续功能开发从 `main` 新建临时分支，例如：`feature/p1-sprint2-battle-controller`。
- 分支内完成实现、编译验证、必要运行验证后，再合并回 `main`。
- 若需要同步远程，先推送功能分支或合并后的 `main`，不要在未验证状态直接污染主线。

## 阻塞与风险

| 日期 | 问题 | 影响 | 处理方案 | 状态 |
|------|------|------|----------|------|

## 下一步

1. 从 `main` 新建 `feature/p1-sprint2-battle-controller` 分支。
2. 开始 Task 2.1：设计 `BattleController` / `BattleSession` 的最小职责和接口。
3. 优先补 EditMode 测试，验证多单位、当前行动单位、移动 / 攻击入口与回合推进的核心规则。
