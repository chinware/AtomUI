# AtomUI 控件文档规范

本文档定义 AtomUI 控件级研发文档的结构、内容边界、写作规则和审查标准，适用于 `AtomUI.Controls`、`AtomUI.Desktop.Controls`、DataGrid、ColorPicker 以及后续新增控件包。

控件文档用于描述控件的最新设计状态、公共契约、交互模型、状态模型、主题架构、Token 边界和维护规则。它不替代用户文档、API reference、正式发布 `CHANGELOG.md` 或代码注释。

## 文档目录结构

每个控件必须使用独立目录承载文档：

```text
docs/controls/<platform>/<category>/<control>/
├── overview.md
├── token.md
└── changelog.md
```

要求：

- `overview.md` 必须存在，用于描述控件整体架构。
- `changelog.md` 必须存在，用于记录控件级设计、API、主题契约、Token 和实现结构变化。
- `token.md` 仅当控件存在专属 Token 或复杂主题变量关系时存在。
- 内容过多时可以继续拆分专题文档，但 `overview.md` 只保留架构主线，并链接专题文档。
- 分类入口文档必须链接控件目录中的主要文档。

## 写作原则

控件文档必须采用严谨、稳定、工程化的语言。

必须遵守：

- 描述最新设计状态，不写历史过程。
- 不使用“未来”“后续”“演进方向”等路线图表述。
- 不堆砌属性清单；属性必须服务于 API 模型、状态模型或兼容边界。
- 不重复全局规范；通用规则应链接全局规范文档。
- 不记录临时讨论、实现流水账或无长期维护价值的细节。
- API、主题契约、Token、template part、伪类等必须表述为稳定契约。

历史变化统一进入控件级 `changelog.md`。

## `overview.md` 结构

`overview.md` 必须按以下结构书写：

```text
# <Control> <平台>架构设计

1. 控件定位
2. 设计语言
3. 架构分层
4. API 设计
5. 行为交互模型
6. 状态模型
7. 模板与视觉架构
8. Theme 架构
9. 控件家族或集成关系
10. 兼容性不变量
11. 专项模型
12. 验证策略
```

章节要求：

- 控件定位：定义控件职责边界，说明控件是什么、不是什么。
- 设计语言：描述控件表达的产品语义和视觉语义。
- 架构分层：说明 Public API、Effective State、Template Contract、Theme、Token、Integration 的职责。
- API 设计：说明公共属性、事件、方法、兼容 API、正交 API 和优先级。
- 行为交互模型：说明 hover、pressed、disabled、loading、focus、keyboard、motion 等交互语义。
- 状态模型：说明 public API 如何归一为 effective state。
- 模板与视觉架构：说明 template part、伪类、视觉层职责和不可合并边界。
- Theme 架构：说明状态如何映射到视觉属性。
- 控件家族或集成关系：说明与派生控件、组合控件、Form、Compact、Browser theme 等关系。
- 兼容性不变量：列出优化和扩展时必须保持不变的 API、行为和渲染。
- 专项模型：记录控件特有模型，例如 Button 的 `Color / Variant`。
- 验证策略：按文档、C# 状态、AXAML、Token、Public API 分层列出验证要求。

无对应内容的章节不能删除，应写明“不适用”及原因。

## `token.md` 结构

`token.md` 只记录控件专属 Token 内容，不重复全局 Token 系统规则。通用 Token 分层、命名、计算、Theme Variables、预设色规则统一链接 [AtomUI 控件 Token 设计规范](control-token-guidelines.md)。

`token.md` 必须按以下结构书写：

```text
# <Control> Token 设计

1. 定位
2. Token 分类
3. 控件专项模型中的 Token 使用
4. 控件家族影响
5. 兼容性要求
6. 验证策略
```

要求：

- Token 分类必须按控件语义组织，而不是按代码顺序机械罗列。
- Token 文档必须说明哪些主题和控件家族引用这些 Token。
- 不允许把实例状态、交互状态或 `EffectiveXxx` 状态写成 Token。
- 不允许在控件 Token 中展开颜色、variant、状态的组合 Token。
- 没有专属 Token 的控件不需要创建 `token.md`，但 `overview.md` 中仍应说明其 Theme 是否直接使用 SharedToken。

## `changelog.md` 结构

每个控件必须维护控件级 changelog。

格式：

```md
# <Control> Changelog

本文档记录 <Control> 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## YYYY-MM-DD

- Docs
  - ...
- API
  - ...
- Theme
  - ...
- Token
  - ...
```

记录范围：

- API 模型变化。
- 主题契约变化。
- Token 分类、语义或边界变化。
- template part、伪类、状态模型变化。
- 控件家族协同规则变化。
- 文档结构和重要设计决策变化。

不记录：

- 临时讨论。
- 纯格式化。
- 无长期维护价值的实现细节。
- 正式 release changelog 条目。

## 链接规则

控件目录内文档必须互相链接：

- `overview.md` 链接 `token.md` 和 `changelog.md`。没有 `token.md` 时只链接 `changelog.md`。
- `token.md` 链接 `overview.md`、`changelog.md` 和 [AtomUI 控件 Token 设计规范](control-token-guidelines.md)。
- `changelog.md` 不解释当前设计，只记录变化。

分类入口文档必须链接控件目录：

```md
- [Button 桌面版架构设计](button/overview.md)
- [Button Token 设计](button/token.md)
- [Button Changelog](button/changelog.md)
```

## 审查标准

控件文档 review 时按以下标准检查：

- 是否描述最新状态，而不是历史过程。
- 是否明确公共 API、事件、方法和兼容边界。
- 是否有清晰的状态模型。
- 是否说明模板职责和不可破坏的 template part。
- 是否把通用规则放到全局规范，而不是控件文档重复。
- 是否维护控件级 `changelog.md`。
- 是否包含分层验证策略。
- 是否存在“未来”“后续”“演进方向”等不适合架构文档的表述。

## 验证要求

控件文档改动至少验证：

```bash
git diff --check
```

同时检查：

- 新增文档没有尾随空白。
- 所有相对链接存在。
- 分类入口文档已更新。
- 只改文档时，不应误改控件实现代码或主题文件。

