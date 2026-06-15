# MechStorm 开发进度

## 当前阶段

- 当前里程碑：P0 / Sprint 1
- 当前任务：Task 1.3 基础 BFS 移动范围
- 当前状态：未开始
- 最后更新：2026-06-15

## 状态约定

- `[ ]` 未开始
- `[~]` 进行中
- `[x]` 已完成
- `[!]` 阻塞
- `[?]` 待决策

## P0 / Sprint 1：最小可玩原型

- [ ] Task 1.0 建立 `MechStorm.Battle` / `MechStorm.Presentation` asmdef 边界
  - 状态：待核对
  - 完成标准：`MechStorm.Battle` 不引用 `UnityEngine`，`MechStorm.Presentation` 可引用 Battle
  - 验证方式：检查 asmdef 引用与编译结果
  - 备注：

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

- [ ] Task 1.3 基础 BFS 移动范围
  - 状态：未开始
  - 完成标准：给定移动力返回可到达格子集合
  - 验证方式：基础单元测试或临时验证脚本覆盖阻挡与边界
  - 备注：

- [ ] Task 1.4 实体壳结构：`PilotData` / `MechData` / `CombatUnit`
  - 状态：未开始
  - 完成标准：字段归属正确，AP 挂 Pilot，HP/攻击/移动挂 Mech
  - 验证方式：编译通过，基础构造与属性读取验证
  - 备注：

- [ ] Task 1.5 `TurnStateMachine`
  - 状态：未开始
  - 完成标准：玩家回合 → 敌方回合 → 循环
  - 验证方式：状态切换验证
  - 备注：

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
- [ ] Sprint 3：空间与地形深化

## P2 / Sprint 4~5：工程化与解耦

- [ ] Sprint 4：TEngine 接入 + 架构重构
- [ ] Sprint 5：数据与回溯

## P3 / Sprint 6+：扩展与商业化

- [ ] 背包词条、元件系统、天赋树、Utility AI、HybridCLR、YooAsset、状态透视仪、养成、录像

## 决策记录

| 日期 | 决策 | 原因 | 影响 |
|------|------|------|------|
| 2026-06-13 | 使用独立进度文件，不直接修改主计划 | 主计划保持蓝图稳定，进度文件记录执行状态 | 新会话需读取 `AGENT.md`、主计划、进度文件 |
| 2026-06-15 | Battle 核心坐标类型命名为 `Vector2Int`，不改为 `SVector2Int` | 保持简洁；若表现层需要 Unity 类型，则显式写 `UnityEngine.Vector2Int` | Battle 内默认使用 `MechStorm.Battle.Foundation.Vector2Int`，跨层代码需避免省略命名空间导致歧义 |

## 阻塞与风险

| 日期 | 问题 | 影响 | 处理方案 | 状态 |
|------|------|------|----------|------|

## 下一步

1. 开始 Task 1.3：实现基础 BFS 移动范围。
2. 设计最小 API：给定起点与移动力，返回可到达格子集合。
3. 继续保持 `MechStorm.Battle` 纯 C#，暂不引入地形 Cost、ZOC、A*。
