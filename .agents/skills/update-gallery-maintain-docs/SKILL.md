---
name: update-gallery-maintain-docs
description: Use when AtomUI Gallery code changes require updates to maintenance documentation under docs/Maintain/AtomUIGallery, especially after ReactiveUI upgrades, architecture changes, API changes, routing updates, or Gallery ViewModel/View refactors.
---

# SKILL: 根据代码修改同步 Gallery 文档

## 名称
`update-gallery-maintain-docs`

## 触发条件
当 Gallery 项目完成一次重要的代码升级或重构（例如 ReactiveUI 版本升级、架构调整、API 变更）后，需要更新 `docs/Gallery` 下的维护文档时，使用本 SKILL。

## Gallery 项目源码

gallery 项目的源码主要在以下三个地方，非常重要，你务必要仔细分析这三个项目的代码修改，才能正确更新文档！！！！！

- Gallery 基础项目：`controlgallery/AtomUIGallery`，这个项目包含了大部分的 ViewModel 和 ShowCase 相关代码，是文档更新的重点。
- Gallery 主项目：`controlgallery/AtomUIGallery.Desktop`，这个项目 Gallery 的主入口，包含了 App.xaml.cs、ViewLocator、依赖注入容器配置等全局相关代码。

## Gallery 项目维护文档
Gallery 项目的维护文档主要位于 `docs/Maintain/AtomUIGallery` 目录下，每次生成维护文档前，你需要认真分析已有的维护文档内容，找出与当前代码库不一致的部分，并根据实际代码修改进行更新。

## 输入要求
- 本次代码修改的简要描述（例如：“将 ReactiveUI 从旧版本升级到 23.1.8，并重构了 WhenActivated 用法”）
- 修改涉及的文件列表（可由 Git diff 或用户提供）
- 目标文档路径：`docs/Maintain/AtomUIGallery`
- 如果对应的文档存在，除非大规模修改了文件的内容，尽量在原有文档上修订，而不是创建新文档。

## 执行步骤

### 第一步：分析代码修改
1. 请用户提供本次修改的 Git 提交哈希或变更文件列表。如果无法提供，请扫描 Gallery 相关项目项目，对比最近两次提交或最近一次重要提交。
2. 你需要特别注意整体架构的变更，例如新增了哪些核心类、修改了哪些核心类的职责、引入了哪些新的设计模式等，这些都是文档中需要重点更新的内容。
3. 我们底层是基于 ReactiveUI 和 Avalonia 的，所以你需要特别关注与这两个框架相关的变更，尤其是 ReactiveUI 的升级可能会涉及到 `WhenActivated`、`ViewModelActivator`、路由机制等方面的变更。
3. 识别变更类型：
    - 全局配置变更（`App.xaml.cs`、`ViewLocator`、依赖注入容器等）
    - ViewModel 基类或公共逻辑变更
    - 路由和导航服务的变更
    - 控件演示页面的 ViewModel/View 代码变更（特别是 `WhenActivated`、`ReactiveCommand` 等）
    - 包引用升级（`.csproj` 文件）
4. 提取每个变更的**旧写法**和**新写法**，并记录变更原因（例如 API 弃用、性能优化、规范统一）。

### 第二步：读取现有 Gallery 文档
1. 列出 `docs/Maintain/AtomUIGallery` 目录下的所有 `.md` 文件。
2. 对于每个文件，读取其内容并分析其结构（例如章节标题、代码示例、图表等），找出与本次代码修改相关的部分。
3. 如果某些文件不存在，请准备创建它们。

### 第三步：对比并确定需要更新的文档
对每个现有文档：
- 找出其中与本次代码修改**不一致**的描述（例如文档中写的是旧的 `WhenActivated` 签名，但代码已更新为新签名）。
- 找出**缺失**的内容（例如新引入的 `ViewModelActivator` 用法未在文档中说明）。
- 标记哪些文档完全过时需要重写，哪些只需局部修改。

### 第四步：生成或更新文档
遵循以下规则更新文档：

1. **明确标记变更**：对于修改的内容，使用 `> **【已更新】**` 或 `[UPDATE: ReactiveUI 23.1.8]` 等标记，便于未来追溯。
2. **保留有效旧内容**：如果旧文档中某些部分仍然正确（例如项目结构说明、第三方依赖列表），不要删除。
3. **添加迁移指南**：如果本次升级涉及破坏性变更，请在 `UPGRADE_LOG.md` 或单独的 `MIGRATION_GUIDE.md` 中记录从旧写法到新写法的转换步骤。
4. **更新代码示例**：所有代码块中的示例必须与当前代码库中的实际写法一致。
5. **更新架构图**：如果架构有变化（例如新增了服务类、改变了数据流），请用 Mermaid 格式更新 `ARCHITECTURE.md` 中的图表。

### 第五步：输出文档
将更新后的文档写入以下路径： `docs/Maintain/AtomUIGallery`
另外，生成一个**变更摘要文件**：`docs/Maintain/AtomUIGallery/CHANGELOG_DOCS.md`，内容为表格形式，列出每个文档文件的变更类型（新增/修改/删除/无变更）和简要说明。

### 第六步：向用户报告
完成后，输出以下信息：
- 本次更新的文档列表
- 每个文档的主要变更点摘要（3-5 条）
- 如果有任何无法自动决策的内容（例如矛盾信息、缺失信息），请明确指出并请求用户提供补充。

## 注意事项
- **不要凭空捏造**：所有文档内容必须基于实际代码修改，不能猜测或加入未实现的设计。
- **保持一致性**：文档中的术语、命名规范应与代码保持一致。
- **优先使用官方术语**：ReactiveUI 相关概念（如 `WhenActivated`、`ViewModelActivator`、`RoutableViewModel`）应使用官方名称。
- **如果用户提供了额外的参考文档**（如 `.referenceprojects/ReactiveUI` 中的官方指南），应优先参考它们来解释变更。

## 示例执行流程（供 Agent 参考）
假设本次升级是将 ReactiveUI 从 11.0.10 升级到 23.1.8，主要变化是将所有 `WhenActivated` 的用法从旧委托形式改为新的 `disposables` 参数形式，并且 ViewLocator 实现有调整。

Agent 应：
1. 扫描所有 `.cs` 文件，找出所有 `WhenActivated` 的旧写法（例如 `WhenActivated(d => { ... })`）已被改为新写法（`WhenActivated(disposables => { ... })`）。
2. 扫描 `docs/Maintain/AtomUIGallery` 路径下已经存在的文档的所有文档，根据扫描代码得到的变更点，找出哪些文档中提到了 `WhenActivated` 的旧写法，并标记需要更新。
5. 生成 `CHANGELOG_DOCS.md`，列出上述修改。
6. 向用户报告完成情况。

## 附：推荐的文档结构（供创建新文档时参考）

如果某个文档不存在，按以下模板创建：

- **ARCHITECTURE.md**：项目整体架构（Mermaid 图）、模块职责、数据流、外部依赖。
- **CODING_STANDARDS.md**：命名规范、ReactiveUI 使用规范（WhenActivated、ReactiveCommand、路由）、XAML 规范、异常处理。
- **ROUTING.md**：路由注册方式、导航服务使用示例、ViewLocator 配置。
- **UPGRADE_LOG.md**：按时间倒序记录每次升级（包版本、变更摘要、迁移说明）。
- **MAINTENANCE.md**：常见维护任务（添加新页面、修复 Bug）、调试技巧、验证清单。
