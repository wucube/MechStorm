# MechStorm Droid Instructions

## 语言

- 请使用中文写提案、分析和回答。

## 必须先做

在回答任何 MechStorm 开发问题、设计方案、修改代码、提交或打标签前，必须先读取：

1. 当前开发进度：[`DEVELOPMENT_PROGRESS.md`](./DEVELOPMENT_PROGRESS.md)
2. TEngine 与项目工作流：[`CLAUDE.md`](./CLAUDE.md)
3. 项目架构与开发主计划：[`MechStorm (机兵风暴) - SRPG 战棋游戏架构与开发主计划.md`](../MechStorm%20%28%E6%9C%BA%E5%85%B5%E9%A3%8E%E6%9A%B4%29%20-%20SRPG%20%E6%88%98%E6%A3%8B%E6%B8%B8%E6%88%8F%E6%9E%B6%E6%9E%84%E4%B8%8E%E5%BC%80%E5%8F%91%E4%B8%BB%E8%AE%A1%E5%88%92.md)
4. 通用工程协作与编码原则：[`ENGINEERING_COLLABORATION_GUIDELINES.md`](./ENGINEERING_COLLABORATION_GUIDELINES.md)

其中 `DEVELOPMENT_PROGRESS.md` 是当前阶段、当前任务、验收状态、决策记录和下一步开发方向的唯一进度来源。

未读取这些文件前，不要开始设计、编码、提交或切换开发阶段。

## 项目规范来源

- TEngine、HybridCLR、YooAsset、UI、资源加载、事件系统、热更边界等规范，以 `CLAUDE.md` 和 `.claude/skills/tengine-dev/references/` 为准。
- 架构分析、编码取舍、精准修改、目标驱动执行、沟通和 Git 通用原则，以 `ENGINEERING_COLLABORATION_GUIDELINES.md` 为准。
- 战斗系统长期架构、机制扩展、阶段性演进，以 `BATTLE_ARCHITECTURE_ROADMAP.md` 为准。
- 当前 Sprint 进度、任务验收、阶段决策，以 `DEVELOPMENT_PROGRESS.md` 为准。
- 外部项目与文章的架构分析位于 `ReferenceAnalyses/` 的主题目录中，文件名直接表达分析主题；这些文档用于提供职责拆分、调用流程、优缺点和踩坑案例参考，不作为高于项目决策的规范来源。

## 逻辑任务强制参考流程

以下任务开始前，必须先查询与当前主题相关的引用分析文档和项目决策文档：

1. 编写或修改游戏代码逻辑。
2. 审查开发者编写的代码逻辑。
3. 规划、拆分或提示后续开发任务中的逻辑实现。

执行要求：

- 先根据任务主题在 `ReferenceAnalyses/` 中定位具体分析文档，只读取与当前主题有关的内容，避免无关资料扩大任务范围。
- 同时核对 `DEVELOPMENT_PROGRESS.md` 的当前任务与决策记录、`BATTLE_ARCHITECTURE_ROADMAP.md` 的长期约束，以及主计划中的对应机制边界。
- 引用分析用于借鉴职责拆分和已知风险；若引用分析与项目决策冲突，以项目决策文档为准，不得直接照搬参考项目实现。
- 设计、审查或实现结论应能说明其对应的项目任务要求、决策依据或引用分析启发，不能跳过该步骤直接给出代码。
- 同一会话内已经读取且主题未变化的文档可以复用；涉及新主题或文件已变更时必须重新查询。

### 引用分析主题路由

- 回合制战斗流程、单位注册表、回合队列：`ReferenceAnalyses/BattleArchitecture/TurnBasedCombatArchitecture.md`
- MechStorm 当前阶段的参考落地顺序：`ReferenceAnalyses/BattleArchitecture/MechStormArchitectureApplicationGuide.md`
- Gameplay Ability System、属性、状态、效果、Tag：`ReferenceAnalyses/AbilitySystems/GameplayAbilitySystemArchitecture.md`
- 复杂战斗系统职责拆分、逻辑与表现、回放边界：`ReferenceAnalyses/AbilitySystems/AbilityKitCombatArchitecture.md`
- Buff 生命周期、实例身份、叠层刷新、驱散、光环、来源和状态恢复：`ReferenceAnalyses/BuffSystems/ScalableBuffSystemArchitecture.md`
- C# 性能优化与 BenchmarkDotNet 工作流：`ReferenceAnalyses/Performance/BenchmarkDotNetAiOptimizationWorkflow.md`

## TEngine 规范路由

按任务类型读取对应精炼文档：

- 项目结构 / 启动流程：`.claude/skills/tengine-dev/references/architecture.md`
- 热更边界 / GameApp 入口：`.claude/skills/tengine-dev/references/hotfix-workflow.md`
- UI 生命周期 / UIWindow / UIWidget：`.claude/skills/tengine-dev/references/ui-lifecycle.md`
- UI 进阶模式：`.claude/skills/tengine-dev/references/ui-patterns.md`
- 资源加载 / 卸载：`.claude/skills/tengine-dev/references/resource-api.md`
- 事件系统：`.claude/skills/tengine-dev/references/event-system.md`
- 模块访问：`.claude/skills/tengine-dev/references/modules.md`
- Luban 配置：`.claude/skills/tengine-dev/references/luban-config.md`
- 命名规则 / 禁止模式：`.claude/skills/tengine-dev/references/naming-rules.md`

## C# 参数排版

- 方法、构造函数及调用参数默认尽量在同一行书写，不因参数数量机械换行。
- 只有整行明显过宽、影响阅读时才换行。
- 换行后按语义将多个相关参数放在同一行，不采用一个参数独占一行的排版。

## Skill 使用边界

- 默认只使用项目自身提供的 Skill，包括项目 `.factory/skills/`、`.claude/skills/` 中定义或由项目动态发现的 Skill。
- Factory/Droid 内置或其他工具层提供的通用 Skill，不得仅因为“可能有帮助”而主动调用。
- 工具层 Skill 仅在以下情况使用：
  1. 用户明确要求使用该 Skill 或其对应能力。
  2. 用户任务与该 Skill 的职责高度重合，并且项目自身没有等效流程或 Skill。
- 若项目文档、项目 Skill 或现有工作流已经覆盖任务需求，应优先遵循项目方案，不再叠加通用审查、简化或分析 Skill。
- 不得因主动调用工具层 Skill 而引入超出用户要求的子任务、额外 Agent 审查或扩大实现范围。

## 规范分层原则

- `AGENTS.md` 只作为 Factory/Droid 的入口索引，不复制完整项目规范。
- 具体 TEngine 开发规范放在 `CLAUDE.md` 和 `.claude/skills/tengine-dev/references/`。
- 具体战斗架构规划放在 `BATTLE_ARCHITECTURE_ROADMAP.md`。
- 具体进度、验收和阶段决策放在 `DEVELOPMENT_PROGRESS.md`。
