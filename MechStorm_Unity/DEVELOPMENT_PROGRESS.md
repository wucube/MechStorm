# MechStorm 开发进度

## 当前阶段

- 当前里程碑：P0 / Sprint 1
- 当前任务：Task 1.6 基础攻击结算
- 当前状态：未开始
- 最后更新：2026-06-17

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

- [ ] Task 1.6 基础攻击结算
  - 状态：未开始
  - 完成标准：相邻格可攻击，固定伤害，HP 归零死亡
  - 验证方式：战斗核心验证
  - 备注：

- [ ] Task 1.7 最简 Unity 表现层
  - 状态：未开始
  - 完成标准：Cube 占位模型，点击移动，血条 Slider
  - 验证方式：Unity Play Mode 手动验证
  - 备注：

## P1 / Sprint 2~3：核心机制深化

- [ ] Sprint 2：实体与战斗深化
  - 备注：后续可引入 `BattleController` / `TurnCoordinator` 管理单位列表、单位是否已行动、死亡/无法行动跳过、所有可行动单位完成后切换阵营；先不要把这些职责塞进 P0 的 `TurnStateMachine`
- [ ] Sprint 3：空间与地形深化

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

## 阻塞与风险

| 日期 | 问题 | 影响 | 处理方案 | 状态 |
|------|------|------|----------|------|

## 下一步

1. 开始 Task 1.6：基础攻击结算。
2. P0 仅支持相邻格攻击、固定伤害、耐久归零死亡。
3. 暂不引入命中率、RNG、防御公式、部位破坏、技能与 AP 消耗。
