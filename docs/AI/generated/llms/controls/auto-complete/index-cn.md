# AutoComplete

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

AutoComplete 是 AtomUI 桌面控件体系中的自动完成输入控件，用于把文本输入、候选弹层、过滤和提交事件组合在一起。

AutoComplete 不负责普通 Select、远程搜索协议或业务结果列表。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/AutoComplete`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete` |
| 状态 | Stable |

## 何时使用

AutoComplete 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | AutoComplete 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | AutoComplete 是 AtomUI 桌面控件体系中的自动完成输入控件，用于把文本输入、候选弹层、过滤和提交事件组合在一起。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `ClearIcon`、`ContentLeftAddOn`、`ContentLeftAddOnTemplate`、`ContentRightAddOn`、`ContentRightAddOnTemplate`、`DefaultValue`、`FilterValue`、`FilterValueSelector` 等 14 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、open/close、loading/async、collection/filter、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | AutoComplete Token + ControlTheme。 |

## 公共 API

AutoComplete 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ClearIcon`、`ContentLeftAddOn`、`ContentLeftAddOnTemplate`、`ContentRightAddOn`、`ContentRightAddOnTemplate`、`DefaultValue`、`FilterValue`、`FilterValueSelector`、`OptionTemplate`、`OptionsAsyncLoader` 等 14 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `CaretIndex`、`ClearSelectionOnLostFocus`、`DisplayCandidateCount`、`Filter`、`IsShowCount` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsAllowClear`、`IsAutoFocus`、`IsAutoSize`、`IsCompletionEnabled`、`IsDropDownOpen`、`IsLoading`、`IsMotionEnabled`、`IsSearching`、`IsSearchOnEnterEnabled`、`IsPopupMatchSelectWidth`、`IsReadOnly` 等 14 项 | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `MaxDropDownHeight`、`PlaceholderForeground`、`Placement`、`SearchButtonStyle`、`SizeType`、`StyleVariant` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 动效与异步 | `AsyncLoadDebounce`、`AsyncLoadTimeout` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `Lines`、`MaxLength`、`MinimumPrefixLength` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

稳定事件包括 `SelectionChanged`、`ValueChanged`；`AutoCompleteSearchEdit` 还通过内部 SearchEdit box 转发 `SearchRequested`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`AbstractAutoComplete`、`AutoComplete`、`AutoCompleteLineEditBox`、`AutoCompleteSearchEdit`、`AutoCompleteSearchEditBox`、`AutoCompleteTextArea`、`AutoCompleteTextAreaBox`、`CompactSpaceAwareAutoComplete`、`CompleteOptionsLoadResult`、`CompleteOptionsLoadedEventArgs`、`CompletePopulatedEventArgs`、`CompletePopulatingEventArgs`、`CompleteValueChangedEventArgs`。
- 枚举：`AutoCompletePlacementMode`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Popup` | `?` | 承载弹层宿主、打开关闭或候选内容。 |

控件专属或内部伪类包括 `AutoCompletePseudoClass.CandidatePopupOpen`、`CandidatePopupOpen=:candidateopen`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

AutoComplete 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `SelectionChanged`、`ValueChanged`；`AutoCompleteSearchEdit` 还通过内部 SearchEdit box 转发 `SearchRequested`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。
- 类型：`AbstractAutoComplete`、`AutoComplete`、`AutoCompleteLineEditBox`、`AutoCompleteSearchEdit`、`AutoCompleteSearchEditBox`、`AutoCompleteTextArea`、`AutoCompleteTextAreaBox`、`CompactSpaceAwareAutoComplete`、`CompleteOptionsLoadResult`、`CompleteOptionsLoadedEventArgs`、`CompletePopulatedEventArgs`、`CompletePopulatingEventArgs`、`CompleteValueChangedEventArgs`。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### {gallery:AutoCompleteShowCaseLangResource BasicUsageTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteShowCase.axaml:35`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:AutoComplete Name="BasicAutoComplete"
```

### {gallery:AutoCompleteShowCaseLangResource CustomizedTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteShowCase.axaml:48`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:AutoComplete Name="CustomizedAutoComplete"
```

### {gallery:AutoCompleteShowCaseLangResource TextAreaAutoCompletionTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteShowCase.axaml:165`

Gallery key：`ExamplesContent` / item `5`

```axaml
<atom:AutoCompleteTextArea Name="TextAreaAutoComplete"
```

### {gallery:AutoCompleteShowCaseLangResource NonCaseSensitiveTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteShowCase.axaml:180`

Gallery key：`ExamplesContent` / item `6`

```axaml
<atom:AutoComplete Name="FilterAutoComplete"
```

## 状态模型

AutoComplete 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、open/close、loading/async、collection/filter、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

AutoComplete 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AbstractAutoCompleteTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `AutoCompleteSearchEditTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `AutoCompleteTextAreaTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `AutoCompleteTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

AutoComplete 使用 `AutoCompleteToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、open/close、loading/async、collection/filter、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

AutoComplete Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `AutoCompleteToken`，scope id 为 `AutoComplete`，源码位于 `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/AutoComplete/AbstractAutoComplete.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoComplete.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteLineEdit.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteOption.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompletePseudoClass.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteSearchEdit.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteSearchEditBox.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteTextArea.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteTextAreaBox.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteThemeConstants.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteToken.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/CompactSpaceAwareAutoComplete.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/CompletePopulatedEventArgs.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/CompletePopulatingEventArgs.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/CompleteValueChangedEventArgs.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/DataLoad/CompleteOptionsLoadResult.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/DataLoad/CompleteOptionsLoadedEventArgs.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/DataLoad/ICompleteOptionsAsyncLoader.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/IAutoCompleteOption.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/Themes/AbstractAutoCompleteTheme.axaml`
- `src/AtomUI.Desktop.Controls/AutoComplete/Themes/AbstractAutoCompleteTheme.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/Themes/AutoCompleteSearchEditTheme.axaml`
- `src/AtomUI.Desktop.Controls/AutoComplete/Themes/AutoCompleteTextAreaTheme.axaml`
- `src/AtomUI.Desktop.Controls/AutoComplete/Themes/AutoCompleteTheme.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/auto-complete/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/auto-complete/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/auto-complete/token.md`
- 变更记录：`docs/controls/desktop/data-entry/auto-complete/changelog.md`
- 语义结构：`./semantic-cn.md`
