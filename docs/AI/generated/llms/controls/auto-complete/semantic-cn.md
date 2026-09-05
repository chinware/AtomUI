# AutoComplete 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

AutoComplete 家族有三个 public owner：`AutoComplete`、`AutoCompleteSearchEdit` 与 `AutoCompleteTextArea`。三者
声明完全相同的 9 个语义键：`root`、`prefix`、`content`、`placeholder`、`input`、`clear`、`popup.root`、
`popup.list`、`popup.listItem`（与上游 AutoComplete 的语义 DOM 对齐，`popup.*` 对应上游 `popup`
分组的 `root` / `list` / `listItem`）。声明位于各 owner 同目录的 `*.SemanticParts.cs` partial 文件。

前 6 个宿主部件位于输入框模板内，且全部声明 `CrossNestedOwners=true`：`AutoCompleteSearchEdit` /
`AutoCompleteTextArea` 内嵌 `LineEdit` / `TextArea` 输入控件，宿主部件的 marker 由嵌套输入控件自身模板携带，
路由通过 `.semantic-scope-input` / `.semantic-scope-input-frame` scope 锚点从 owner 穿透到嵌套控件模板
（`/template/` 链不能直接跨越嵌套 owner，因此这些 Part 由生成 Style 借助 scope 路由命中）。弹层三部件
（`popup.root` / `popup.list` / `popup.listItem`）位于 owner 自有的 Popup 模板内，不涉及嵌套 owner。

各 Part 的 Selector、SelectorRoute、ContractType 以 `AutoComplete.SemanticParts.cs` 为准；下表为 owner
`AutoComplete` 的声明（SearchEdit / TextArea 声明同键，route 差异见 §1.2）：

### 1.1 部件表（`AutoComplete`）

| Part | SelectorClass | SelectorRoute | ContractType | Cardinality | Since |
| --- | --- | --- | --- | --- | --- |
| `root` | 不适用（owner 本身，无 marker） | 不适用 | `AutoComplete` | Single | 6.0 |
| `prefix` | `.semantic-prefix` | `/template/ .semantic-scope-input >> .semantic-scope-input-frame /template/ .semantic-scope-prefix > .semantic-prefix` | `ContentPresenter` | Single | 6.0 |
| `content` | `.semantic-content` | `/template/ .semantic-scope-input /template/ .semantic-content` | `Panel` | Single | 6.0 |
| `placeholder` | `.semantic-placeholder` | `/template/ .semantic-scope-input /template/ .semantic-content > .semantic-placeholder` | `TextBlock` | Single | 6.0 |
| `input` | `.semantic-input` | `/template/ .semantic-scope-input /template/ .semantic-input` | `TextPresenter` | Single | 6.0 |
| `clear` | `.semantic-clear` | `/template/ .semantic-scope-input >> .semantic-scope-input-frame /template/ .semantic-scope-suffix > .semantic-suffix > .semantic-clear` | `Button` | Single | 6.0 |
| `popup.root` | `.semantic-popup-root` | `/template/ .semantic-popup-root` | `Border` | Single | 6.0 |
| `popup.list` | `.semantic-popup-list` | `/template/ .semantic-popup-root > .semantic-popup-list` | `CandidateList` | Single | 6.0 |
| `popup.listItem` | `.semantic-popup-list-item` | `/template/ .semantic-popup-list >> .semantic-popup-list-item` | `CandidateListItem` | Multiple | 6.0 |

`prefix`、`content`、`placeholder`、`input`、`clear` 均声明 `CrossNestedOwners=true`；`popup.listItem` 声明
`RuntimeCreated=true`（候选条目运行时创建）。

### 1.2 嵌套输入控件 owner 的 route 差异

`AutoCompleteSearchEdit` 与 `AutoCompleteTextArea` 的声明与 §1.1 同键同 ContractType，仅两处 route 不同：

- `AutoCompleteSearchEdit.prefix`：输入框 frame 由 SearchEdit 主题承担，route 为
  `/template/ .semantic-scope-input >> .semantic-scope-input-frame /template/ .semantic-scope-prefix > .semantic-prefix`。
- `AutoCompleteTextArea.input`：文本域的可编辑区域 marker 为 `.semantic-textarea`，SelectorRoute 为
  `/template/ .semantic-scope-input /template/ .semantic-textarea`（SelectorClass 仍为 `semantic-input`，
  Part 身份与生成 Style 类型保持 `input` 命名）。

其余键（含 `clear`、`popup.*`）三个 owner 完全一致。

### 1.3 职责说明

- `root`：AutoComplete owner 本身，承载数据源、过滤、弹层与状态的组织边界，可定制 owner 级视觉属性。
- `prefix`：输入框框架的前缀区域（`ContentLeftAddOn` 宿主）。
- `content`：输入内容面板，承载文本呈现器与占位符。
- `placeholder`：输入为空时显示的占位符文本。
- `input`：可编辑输入区域的 `TextPresenter`。
- `clear`：后缀区域的清除按钮，`IsAllowClear` 启用且有输入时可见（`ClearIcon` 可定制）。
- `popup.root`：候选弹层根 `Border`，可定制弹层边框、背景与宽度。
- `popup.list`：弹层内候选列表容器（`CandidateList`）。
- `popup.listItem`：单个候选条目（`CandidateListItem`），每个选项运行时创建一个实例。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/AutoComplete/Themes/AutoCompleteTheme.axaml`

```xml
<Panel>
    <AutoCompleteLineEditBox Name="{x:Static atom:AutoCompleteThemeConstants.TextBoxPart}" />
    <Popup Name="{x:Static atom:AutoCompleteThemeConstants.PopupPart}">
        <Border Name="PopupFrame">
            <CandidateList Name="{x:Static atom:AutoCompleteThemeConstants.CandidateListPart}" />
        </Border>
    </Popup>
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
AutoComplete
  -> AutoCompleteSearchEdit (control theme, AutoCompleteSearchEditTheme.axaml)
     -> Panel (template-stable)
        -> AutoCompleteSearchEditBox#{x:Static atom:AutoCompleteThemeConstants.TextBoxPart} (internal-observable)
        -> Popup#{x:Static atom:AutoCompleteThemeConstants.PopupPart} (internal-observable)
           -> Border#PopupFrame (template-stable)
              -> CandidateList#{x:Static atom:AutoCompleteThemeConstants.CandidateListPart} (template-stable)
  -> AutoCompleteTextArea (control theme, AutoCompleteTextAreaTheme.axaml)
     -> Panel (template-stable)
        -> AutoCompleteTextAreaBox#{x:Static atom:AutoCompleteThemeConstants.TextBoxPart} (internal-observable)
        -> Popup#{x:Static atom:AutoCompleteThemeConstants.PopupPart} (internal-observable)
           -> Border#PopupFrame (template-stable)
              -> CandidateList#{x:Static atom:AutoCompleteThemeConstants.CandidateListPart} (template-stable)
  -> AutoComplete (control theme, AutoCompleteTheme.axaml)
     -> Panel (template-stable)
        -> AutoCompleteLineEditBox#{x:Static atom:AutoCompleteThemeConstants.TextBoxPart} (internal-observable)
        -> Popup#{x:Static atom:AutoCompleteThemeConstants.PopupPart} (internal-observable)
           -> Border#PopupFrame (template-stable)
              -> CandidateList#{x:Static atom:AutoCompleteThemeConstants.CandidateListPart} (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `AutoComplete` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `AutoCompleteSearchEdit` | control theme | `AutoCompleteSearchEditTheme.axaml` | 用户代码 / 控件宿主 | `CaretIndex`, `ClearIcon`, `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `AutoCompleteSearchEditTheme.axaml` | AutoCompleteSearchEdit | `CaretIndex`, `ClearIcon`, `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AutoCompleteThemeConstants.TextBoxPart}` | template node (AutoCompleteSearchEditBox) | `AutoCompleteSearchEditTheme.axaml` | AutoCompleteSearchEdit | `CaretIndex`, `ClearIcon`, `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AutoCompleteThemeConstants.PopupPart}` | template node (Popup) | `AutoCompleteSearchEditTheme.axaml` | AutoCompleteSearchEdit | `IsMotionEnabled`, `MaxPopupHeight`, `MinPopupWidth`, `OptionTemplate`, `PopupContentPadding`, `PopupPlacement` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PopupFrame` | template node (Border) | `AutoCompleteSearchEditTheme.axaml` | AutoCompleteSearchEdit | `IsMotionEnabled`, `MaxPopupHeight`, `MinPopupWidth`, `OptionTemplate`, `PopupContentPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AutoCompleteThemeConstants.CandidateListPart}` | template node (CandidateList) | `AutoCompleteSearchEditTheme.axaml` | AutoCompleteSearchEdit | `IsMotionEnabled`, `OptionTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `AutoCompleteTextArea` | control theme | `AutoCompleteTextAreaTheme.axaml` | 用户代码 / 控件宿主 | `CaretIndex`, `ClearIcon`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `AutoCompleteTextAreaTheme.axaml` | AutoCompleteTextArea | `CaretIndex`, `ClearIcon`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AutoCompleteThemeConstants.TextBoxPart}` | template node (AutoCompleteTextAreaBox) | `AutoCompleteTextAreaTheme.axaml` | AutoCompleteTextArea | `CaretIndex`, `ClearIcon`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AutoCompleteThemeConstants.PopupPart}` | template node (Popup) | `AutoCompleteTextAreaTheme.axaml` | AutoCompleteTextArea | `IsMotionEnabled`, `MaxPopupHeight`, `MinPopupWidth`, `OptionTemplate`, `PopupContentPadding`, `PopupPlacement` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PopupFrame` | template node (Border) | `AutoCompleteTextAreaTheme.axaml` | AutoCompleteTextArea | `IsMotionEnabled`, `MaxPopupHeight`, `MinPopupWidth`, `OptionTemplate`, `PopupContentPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AutoCompleteThemeConstants.CandidateListPart}` | template node (CandidateList) | `AutoCompleteTextAreaTheme.axaml` | AutoCompleteTextArea | `IsMotionEnabled`, `OptionTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `AutoComplete` | control theme | `AutoCompleteTheme.axaml` | 用户代码 / 控件宿主 | `CaretIndex`, `ClearIcon`, `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `AutoCompleteTheme.axaml` | AutoComplete | `CaretIndex`, `ClearIcon`, `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AutoCompleteThemeConstants.TextBoxPart}` | template node (AutoCompleteLineEditBox) | `AutoCompleteTheme.axaml` | AutoComplete | `CaretIndex`, `ClearIcon`, `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AutoCompleteThemeConstants.PopupPart}` | template node (Popup) | `AutoCompleteTheme.axaml` | AutoComplete | `IsMotionEnabled`, `MaxPopupHeight`, `MinPopupWidth`, `OptionTemplate`, `PopupContentPadding`, `PopupPlacement` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PopupFrame` | template node (Border) | `AutoCompleteTheme.axaml` | AutoComplete | `IsMotionEnabled`, `MaxPopupHeight`, `MinPopupWidth`, `OptionTemplate`, `PopupContentPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AutoCompleteThemeConstants.CandidateListPart}` | template node (CandidateList) | `AutoCompleteTheme.axaml` | AutoComplete | `IsMotionEnabled`, `OptionTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ClearIcon`、`ContentLeftAddOn`、`ContentLeftAddOnTemplate`、`ContentRightAddOn`、`ContentRightAddOnTemplate`、`DefaultValue`、`FilterValue`、`FilterValueSelector`、`OptionTemplate`、`OptionsAsyncLoader` 等 14 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `CaretIndex`、`ClearSelectionOnLostFocus`、`DisplayCandidateCount`、`Filter`、`IsShowCount` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsAllowClear`、`IsAutoFocus`、`IsAutoSize`、`IsCompletionEnabled`、`IsDropDownOpen`、`IsLoading`、`IsMotionEnabled`、`IsOperating`、`IsSearchOnEnterEnabled`、`IsPopupMatchSelectWidth`、`IsReadOnly` 等 14 项 | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `MaxDropDownHeight`、`PlaceholderForeground`、`Placement`、`SearchButtonStyle`、`SizeType`、`StyleVariant` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 动效与异步 | `AsyncLoadDebounce`、`AsyncLoadTimeout` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `Lines`、`MaxLength`、`MinimumPrefixLength` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、open/close、loading/async、collection/filter、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | AutoComplete Token + ControlTheme。 |

## State Flow

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

## Theme and Token Boundaries

AutoComplete 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

`AutoCompleteLineEditBox`、`AutoCompleteSearchEditBox` 和 `AutoCompleteTextAreaBox` 分别复用 `LineEdit`、`SearchEdit` 和 `TextArea` 的 `AbstractTextInput` 逻辑层与 `InputControlFrame` 输入表面。候选 popup 和过滤状态属于 AutoComplete 自身，不得重新声明输入边框、状态或 Form error owner。

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

Token 边界：

AutoComplete Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `AutoCompleteToken`，scope id 为 `AutoComplete`，源码位于 `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteToken.cs`。

## Customization Boundaries

维护 AutoComplete 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 AutoComplete 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
- Semantic Part marker、selector class、route、`ContractType` 与 cardinality（见 [AutoComplete Semantic Part 契约](semantic-part.md)）；宿主部件 marker 由嵌套输入控件模板携带，跨 owner 回收路径不泄漏 marker。
