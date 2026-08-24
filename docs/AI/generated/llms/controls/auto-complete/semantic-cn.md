# AutoComplete 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `AutoComplete` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

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
