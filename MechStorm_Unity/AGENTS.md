# MechStorm Droid Instructions

## 语言

- 请使用中文写提案、分析和回答。

## 必须先做

在回答任何 MechStorm 开发问题、设计方案、修改代码、提交或打标签前，必须先读取：

1. 当前开发进度：[`DEVELOPMENT_PROGRESS.md`](./DEVELOPMENT_PROGRESS.md)
2. TEngine 与项目工作流：[`CLAUDE.md`](./CLAUDE.md)
3. 项目架构与开发主计划：[`MechStorm (机兵风暴) - SRPG 战棋游戏架构与开发主计划.md`](../MechStorm%20%28%E6%9C%BA%E5%85%B5%E9%A3%8E%E6%9A%B4%29%20-%20SRPG%20%E6%88%98%E6%A3%8B%E6%B8%B8%E6%88%8F%E6%9E%B6%E6%9E%84%E4%B8%8E%E5%BC%80%E5%8F%91%E4%B8%BB%E8%AE%A1%E5%88%92.md)

其中 `DEVELOPMENT_PROGRESS.md` 是当前阶段、当前任务、验收状态、决策记录和下一步开发方向的唯一进度来源。

未读取这些文件前，不要开始设计、编码、提交或切换开发阶段。

## 项目规范来源

- TEngine、HybridCLR、YooAsset、UI、资源加载、事件系统、热更边界等规范，以 `CLAUDE.md` 和 `.claude/skills/tengine-dev/references/` 为准。
- 战斗系统长期架构、机制扩展、阶段性演进，以 `BATTLE_ARCHITECTURE_ROADMAP.md` 为准。
- 当前 Sprint 进度、任务验收、阶段决策，以 `DEVELOPMENT_PROGRESS.md` 为准。

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

## 规范分层原则

- `AGENTS.md` 只作为 Factory/Droid 的入口索引，不复制完整项目规范。
- 具体 TEngine 开发规范放在 `CLAUDE.md` 和 `.claude/skills/tengine-dev/references/`。
- 具体战斗架构规划放在 `BATTLE_ARCHITECTURE_ROADMAP.md`。
- 具体进度、验收和阶段决策放在 `DEVELOPMENT_PROGRESS.md`。
