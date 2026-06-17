# AtomUI Changelog 规范

本文档定义 AtomUI Changelog 的维护规则。正式版本变更记录写入仓库根目录 `CHANGELOG.md`；规则、格式约束和发布前检查保留在本文档中，不写入 `CHANGELOG.md`。

Changelog 面向控件使用者、Gallery 使用者、集成方和发布维护者。它记录版本变化，不替代 commit log，也不记录内部开发流水账。

## 适用范围

只有以下场景需要维护正式 Changelog：

- 用户明确要求收集、生成或修改 Changelog。
- 准备 release、release PR、版本发布说明或版本公告。
- 正在编辑 `CHANGELOG.md`。
- 变更包含必须提前暴露的兼容性、迁移、AOT、包结构或发布产物信息。

普通功能、修复、文档、Gallery 示例或内部重构 PR 不默认修改 `CHANGELOG.md`。代码 review 时也不要仅因为缺少 Changelog 改动提出问题。

普通 PR 如果需要填写模板中的 Change Log，只描述本 PR 对用户或开发者的影响；如果没有用户可感知变化，填写 `N/A`、`No changelog required` 或 `无需更新日志`。正式 `CHANGELOG.md` 由 release owner 在发布流程中统一整理。

## 基本原则

- 只记录用户、集成方或发布维护者需要知道的变化。
- 条目描述用户或开发者可感知的结果，不描述私有实现细节。
- 内部重构默认不进入 Changelog，除非影响公开行为、兼容性、性能、AOT、包结构、迁移成本或发布产物。
- 每条记录应能追溯到 commit、PR、issue、测试或发布验证结果。
- 同一问题不要拆成多条实现细节；同一能力不要重复记录在多个分组下。
- 有 PR、issue、贡献者信息时，尽量在条目末尾补充引用。

## 版本规则

AtomUI 遵循 Semantic Versioning 2.0.0。

- `patch`：bug 修复、兼容性修复、性能修复、文档修正或发布修正。
- `minor`：新增控件、API、属性、主题能力、Gallery 示例或非破坏性能力增强。
- `major`：破坏性 API、包结构、默认行为、主题 Token、平台支持或迁移方式变化。

版本号以 `build/Version.props` 中的 `AtomUIVersion` 为准。正式版本日期使用 `YYYY-MM-DD`。

## CHANGELOG.md 文件结构

`CHANGELOG.md` 只记录版本变更。文件顶部保留标题和简短说明，之后按版本倒序记录发布内容。每个版本包含版本号、发布日期和本次发布的用户可见变化。

版本下优先按控件、模块或发布能力聚合条目；不要把每个版本机械拆成固定的 `Added`、`Changed`、`Fixed` 分类，因为控件库用户更关心哪个控件或模块发生了什么。

```md
# Changelog

All notable changes to AtomUI are documented in this file.

`AtomUI` follows Semantic Versioning 2.0.0.

## 6.0.5

`2026-06-17`

- DataGrid
  - Fix sorting crash in NativeAOT publish when clicking column headers. #123
  - Fix empty token table state in Gallery.
- Menu
  - Fix submenu foreground transition still running when motion is disabled.
- Theme
  - Improve dark mode token fallback for desktop controls.
```

如果某个版本条目很少，可以不分组：

```md
## 6.0.6

`2026-06-24`

- Fix Gallery NativeAOT publish missing generated DataGrid accessors.
```

## 分组规则

优先使用用户能识别的控件或模块名：

- 控件：`Button`、`Menu`、`DatePicker`、`DataGrid`、`ColorPicker`。
- 体系：`Theme`、`Localization`、`Motion`、`Icons`、`Native`。
- 包：`AtomUI.Core`、`AtomUI.Controls.Shared`、`AtomUI.Desktop.Controls`。
- 应用：`Gallery`。
- 发布能力：`NativeAOT`、`Build`、`Packaging`。

同一控件或模块有 2 条以上变化时，使用二级列表分组。只有一条变化时可以直接写单行条目。

## 条目写法

推荐格式：

```md
- DataGrid
  - Fix sorting crash in NativeAOT publish when clicking column headers. #123
  - Add empty state for token table when no design tokens are available.
```

写作规则：

- 英文优先，中文可接受；同一个版本内尽量保持一致。
- 使用现在时或祈使式结果描述：`Fix`、`Add`、`Improve`、`Remove`、`Deprecate`。
- 条目正文必须出现用户可识别的控件、模块或能力名。
- 控件名、模块名不需要反引号；属性名、API、配置项、包名使用反引号。
- 说明用户看到的变化，不写“重构某类”“调整某私有字段”。
- 一条记录对应一个用户可理解的问题或能力，不拆实现细节。
- 有 PR、issue 时放在条目末尾，例如 `#123`。
- 有外部贡献者时可追加 contributor，例如 `@username`。

## 必须进入 Changelog 的变化

- 新增、移除或废弃公开 API、控件、属性、事件、样式能力。
- 用户可见的控件行为、布局、动画、主题、国际化变化。
- 影响升级的默认值、包结构、资源注册、启动注册方式变化。
- bug 修复，尤其是崩溃、数据错误、视觉错误、交互错误、内存泄漏。
- NativeAOT、trimming、source generator、反射替代等兼容性变化。
- NuGet 包、目标框架、依赖版本、发布脚本、CI 发布流程变化。
- Gallery 中能帮助用户理解控件能力的重要示例变化。
- 文档中新增的迁移路径、兼容性说明或维护者必须知道的发布规则。

## 不进入 Changelog 的变化

- 纯格式化、命名整理、私有方法移动。
- 只影响测试代码且不改变用户行为的测试补充。
- 未改变发布产物的本地脚本整理。
- 单纯修正文档错别字。
- 开发过程中的临时修复、实验性提交。
- 对用户不可见、也不影响兼容性或维护风险的内部重构。

## Breaking Changes

破坏性变更必须放在版本最前面，并明确迁移方式。

```md
## 7.0.0

`2026-xx-xx`

- Breaking Changes
  - Theme: Rename `ControlDesignToken` to `AbstractControlDesignToken`. Custom token classes must update their base type.
  - Packaging: Move DataGrid APIs to `AtomUI.Desktop.Controls.DataGrid`. Applications must reference the package explicitly.
```

Breaking change 不要只写“调整 API”，必须写清楚受影响范围和迁移动作。

## AOT 与发布兼容性

NativeAOT、trimming、source generator、动态访问替代等变化必须在 Changelog 中可见，除非它们完全不影响用户、发布产物或迁移风险。

推荐写法：

```md
- NativeAOT
  - Fix DataGrid sorting in NativeAOT publish by using generated data member accessors.
- Generator
  - Add generated accessors for Gallery data models used by sorted DataGrid columns.
```

如果 AOT 修复属于某个控件的用户可见 bug，也可以放在控件分组下，但条目中必须明确 `NativeAOT`。

## Release 前检查

发布前维护 `CHANGELOG.md` 时必须检查：

- 版本号是否与 `build/Version.props` 一致。
- 是否覆盖 release 范围内用户可见的 `feat`、`fix`、`perf`、`gallery`、`build`、`release` 变化。
- 是否遗漏 AOT、trimming、source generator、包结构、目标框架相关兼容性变化。
- Breaking changes 是否包含迁移说明。
- 是否按控件或模块合并条目，而不是把 commit message 原样堆进去。
- 是否有可追溯的 PR、issue、commit 或验证来源。
