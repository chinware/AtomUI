# AtomUI 控件文档规范

本文档定义 AtomUI 控件级研发文档的结构、内容边界、写作规则和审查标准，适用于 `AtomUI.Controls`、`AtomUI.Desktop.Controls`、DataGrid、ColorPicker 以及其他控件包。

控件文档用于描述控件的最新设计状态、公共契约、交互模型、状态模型、主题架构、Token 边界、内部实现原理和维护规则。它不替代用户文档、API reference、正式发布 `CHANGELOG.md` 或代码注释。

## 文档目录结构

每个控件必须使用独立目录承载文档：

```text
docs/controls/<platform>/<category>/<control>/
├── overview.md
├── implementation.md
├── token.md
└── changelog.md
```

要求：

- `overview.md` 必须存在，用于描述控件设计定位、公共契约、状态模型、视觉主题关系和维护入口。
- `implementation.md` 必须存在，用于描述特定控件的内部实现原理、源码职责边界、关键状态流和维护规则，以提升控件可维护性。
- `changelog.md` 必须存在，用于记录控件级设计、API、主题契约、Token 和实现结构变化。
- `token.md` 仅当控件存在专属 Token 或复杂主题变量关系时存在。
- 内容过多时可以继续拆分专题文档，但 `overview.md` 只保留设计与契约主线，`implementation.md` 只保留实现原理主线，并链接专题文档。
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
- `overview.md` 不展开具体代码实现，具体实现原理进入 `implementation.md`。
- `implementation.md` 不重复公共 API 清单、Token 全局规则或用户文档内容，只解释当前控件内部实现为什么这样组织、状态如何流动、维护时必须守住哪些边界。

历史变化统一进入控件级 `changelog.md`。

## `overview.md` 结构

`overview.md` 必须按以下结构书写：

```text
# <Control> <平台>架构设计

1. 控件定位
2. 设计语言
3. API 与契约模型
4. 行为与状态模型
5. 视觉与主题模型
6. 控件家族或集成关系
7. 兼容性不变量
8. 专项模型
9. 文档导航与验证策略
```

章节要求：

- 控件定位：定义控件职责边界，说明控件是什么、不是什么。
- 设计语言：描述控件表达的产品语义和视觉语义。
- API 与契约模型：说明公共属性、事件、方法、template part、伪类和主题入口的契约边界，不机械罗列成员。
- 行为与状态模型：说明 hover、pressed、disabled、loading、focus、keyboard、motion 等交互语义，以及 public API 如何归一为 effective state。
- 视觉与主题模型：说明模板、Theme、Token、SharedToken 的总体关系和不可破坏的视觉契约，不展开具体 AXAML 结构。
- 控件家族或集成关系：说明与派生控件、组合控件、Form、Compact、Browser theme 等关系。
- 兼容性不变量：列出优化和扩展时必须保持不变的 API、行为和渲染。
- 专项模型：记录控件特有模型，例如 Button 的 `Color / Variant`。
- 文档导航与验证策略：链接 `implementation.md`、`token.md`、`changelog.md`，并按 Public API、状态、AXAML、Token、文档分层列出验证要求。

无对应内容的章节不能删除，应写明“不适用”及原因。

## `implementation.md` 结构

`implementation.md` 描述特定控件的内部实现原理，用于帮助维护者理解源码结构、状态流转、生命周期和关键算法。它不替代代码注释，不记录临时实现过程，也不作为用户 API 文档。

`implementation.md` 必须按以下结构书写：

```text
# <Control> <平台>实现原理

1. 实现定位
2. 源码文件结构
3. 核心类职责
4. 状态与数据流
5. 生命周期与模板接入
6. 交互与事件处理
7. 内部算法与关键流程
8. 资源、性能与 AOT 边界
9. 维护不变量
10. 测试与验证
```

章节要求：

- 实现定位：说明本文档覆盖的内部实现范围，以及哪些细节仍应直接阅读源码。
- 源码文件结构：说明控件相关 `.cs`、`.axaml`、Token、helper、handler、内部 view 的职责边界。
- 核心类职责：描述主要类、内部接口、handler、presenter、decorator、数据节点等协作关系。
- 状态与数据流：描述 public API、internal state、effective state、ItemsSource、Children、selection、checked、expanded、filter 等状态如何流动。
- 生命周期与模板接入：描述构造、初始化、加载、卸载、template part 获取、事件订阅释放、container 准备和资源绑定规则。
- 交互与事件处理：描述 pointer、keyboard、focus、context menu、drag/drop、motion、popup 等内部事件路径。
- 内部算法与关键流程：描述维护者必须理解的遍历、布局、状态回放、同步、缓存、渲染等关键流程，不逐行复述代码。
- 资源、性能与 AOT 边界：描述动态资源、事件订阅、binding、缓存、异步任务、反射或生成器相关约束。
- 维护不变量：列出重构、优化和修 bug 时不能破坏的内部行为顺序、状态一致性、生命周期释放和主题契约。
- 测试与验证：列出对应测试、Gallery 走查点、AOT 或发布验证要求。

要求：

- 只写当前实现原理，不写历史过程；历史变化进入 `changelog.md`。
- 不把私有方法逐个改写成说明书；只记录维护者必须知道的稳定结构、关键路径和不变量。
- 不重复 `overview.md` 的设计语言、API 模型和公共契约；必要时只引用它们作为实现入口。
- 不重复 `token.md` 的 Token 分类和语义；实现中使用 Token 的路径可以链接 `token.md`。
- 当控件实现较简单时，也应保留该文档，并用简短章节说明实现没有额外内部模型，避免维护者误判文档缺失。

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
- 文档结构、实现文档边界和重要设计决策变化。

不记录：

- 临时讨论。
- 纯格式化。
- 无长期维护价值的实现细节。
- 正式 release changelog 条目。

## 链接规则

控件目录内文档必须互相链接：

- `overview.md` 链接 `implementation.md`、`token.md` 和 `changelog.md`。没有 `token.md` 时只链接 `implementation.md` 和 `changelog.md`。
- `implementation.md` 链接 `overview.md`、`changelog.md`，并在涉及控件专属 Token 时链接 `token.md`。
- `token.md` 链接 `overview.md`、`implementation.md`、`changelog.md` 和 [AtomUI 控件 Token 设计规范](control-token-guidelines.md)。
- `changelog.md` 不解释当前设计，只记录变化。

分类入口文档必须链接控件目录：

```md
- [Button 桌面版架构设计](button/overview.md)
- [Button 桌面版实现原理](button/implementation.md)
- [Button Token 设计](button/token.md)
- [Button Changelog](button/changelog.md)
```

## 审查标准

控件文档 review 时按以下标准检查：

- 是否描述最新状态，而不是历史过程。
- 是否明确公共 API、事件、方法和兼容边界。
- 是否有清晰的状态模型。
- 是否说明模板职责和不可破坏的 template part。
- 是否有独立 `implementation.md` 描述控件内部实现原理、源码职责边界、生命周期和维护不变量。
- `overview.md` 是否避免承载过重实现细节。
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
