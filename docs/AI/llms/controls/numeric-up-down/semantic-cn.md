# NumericUpDown 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `NumericUpDown` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/NumericUpDown/Themes/NumericUpDownTheme.axaml`

```xml
<ButtonSpinner Name="PART_Spinner">
    <EmbeddedTextBox Name="PART_TextBox" />
</ButtonSpinner>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
NumericUpDown
  -> ButtonSpinnerDecoratedBox (control theme, ButtonSpinnerDecoratedBoxTheme.axaml)
     -> DockPanel#RootLayout (template-stable)
        -> PixelAlignedBorder#{x:Static atom:AddOnDecoratedBoxThemeConstants.LeftAddOnPart} (template-stable)
           -> AddOnContentPresenter#PART_LeftAddOnPresenter (template-stable)
        -> PixelAlignedBorder#{x:Static atom:AddOnDecoratedBoxThemeConstants.RightAddOnPart} (template-stable)
           -> AddOnContentPresenter#PART_RightAddOnPresenter (template-stable)
        -> Panel (template-stable)
           -> AddOnDecoratedBoxContentFrame#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart} (template-stable)
              -> ButtonSpinnerContentPanel#ContentLayout (internal-observable)
                 -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart} (internal-observable)
                 -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart} (internal-observable)
                 -> ContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentPresenterPart} (internal-observable)
           -> ContentPresenter#PART_SpinnerHandle (template-stable)
  -> ButtonSpinnerHandle (control theme, ButtonSpinnerHandleTheme.axaml)
     -> UniformGrid (template-stable)
        -> IconButton#PART_IncreaseButton (template-stable)
        -> IconButton#PART_DecreaseButton (template-stable)
  -> EmbeddedTextBox (control theme, EmbeddedTextBoxTheme.axaml)
  -> InputClearIconButton (control theme, InputClearIconButtonTheme.axaml)
  -> ResizeHandle (control theme, ResizeHandleTheme.axaml)
     -> Border#Frame (template-stable)
  -> RevealButton (control theme, RevealButtonTheme.axaml)
  -> SearchButton (control theme, SearchButtonTheme.axaml)
  -> SearchEditDecoratedBox (control theme, SearchEditDecoratedBoxTheme.axaml)
     -> SearchEditPanel#RootLayout (internal-observable)
        -> PixelAlignedBorder#{x:Static atom:AddOnDecoratedBoxThemeConstants.LeftAddOnPart} (template-stable)
           -> AddOnContentPresenter#PART_LeftAddOnPresenter (template-stable)
        -> SearchButton#{x:Static atom:AddOnDecoratedBoxThemeConstants.RightAddOnPart} (internal-observable)
        -> AddOnDecoratedBoxContentFrame#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart} (template-stable)
           -> DockPanel#ContentLayout (template-stable)
              -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart} (internal-observable)
              -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart} (internal-observable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
  -> TextAreaDecoratedBox (control theme, TextAreaDecoratedBoxTheme.axaml)
     -> AddOnDecoratedBoxContentFrame#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart} (template-stable)
        -> Border#TextAreaContentFrame (template-stable)
           -> DockPanel#ContentLayout (template-stable)
              -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart} (internal-observable)
              -> Panel (template-stable)
                 -> ScrollViewer#PART_ScrollViewer (template-stable)
                    -> ContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentPresenterPart} (internal-observable)
                 -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart} (internal-observable)
  -> NumericUpDown (control theme, NumericUpDownTheme.axaml)
     -> ButtonSpinner#PART_Spinner (template-stable)
        -> EmbeddedTextBox#PART_TextBox (template-stable)
     -> ButtonSpinner#PART_Spinner (template-stable)
        -> EmbeddedTextBox#PART_TextBox (template-stable)
     -> ButtonSpinnerDecoratedBox#PART_DecoratedBox (template-stable)
        -> DockPanel (template-stable)
           -> PixelAlignedBorder (template-stable)
              -> IconButton#PART_DecreaseButton (template-stable)
           -> PixelAlignedBorder (template-stable)
              -> IconButton#PART_IncreaseButton (template-stable)
           -> DockPanel (template-stable)
              -> AddOnContentPresenter (internal-observable)
              -> AddOnContentPresenter (internal-observable)
              -> ContentPresenter (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `NumericUpDown` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ButtonSpinnerDecoratedBox` | control theme | `ButtonSpinnerDecoratedBoxTheme.axaml` | NumericUpDown | `Background`, `BorderBrush`, `ButtonSpinnerLocation`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (DockPanel) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `Background`, `BorderBrush`, `ButtonSpinnerLocation`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.LeftAddOnPart}` | template node (PixelAlignedBorder) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `LeftAddOn`, `LeftAddOnBorderThickness`, `LeftAddOnCornerRadius`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LeftAddOnPresenter` | template node (AddOnContentPresenter) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `LeftAddOn`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.RightAddOnPart}` | template node (PixelAlignedBorder) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `RightAddOn`, `RightAddOnBorderThickness`, `RightAddOnCornerRadius`, `RightAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RightAddOnPresenter` | template node (AddOnContentPresenter) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `RightAddOn`, `RightAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `Background`, `BorderBrush`, `ButtonSpinnerLocation`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart}` | template node (AddOnDecoratedBoxContentFrame) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `Background`, `BorderBrush`, `ButtonSpinnerLocation`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentLayout` | template node (ButtonSpinnerContentPanel) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `ButtonSpinnerLocation`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart}` | template node (AddOnContentPresenter) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart}` | template node (AddOnContentPresenter) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `ContentRightAddOn`, `ContentRightAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentPresenterPart}` | template node (ContentPresenter) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_SpinnerHandle` | template node (ContentPresenter) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `HandleOpacity`, `SpinnerContent` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ButtonSpinnerHandle` | control theme | `ButtonSpinnerHandleTheme.axaml` | NumericUpDown | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_IncreaseButton` | template node (IconButton) | `ButtonSpinnerHandleTheme.axaml` | ButtonSpinnerHandle | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_DecreaseButton` | template node (IconButton) | `ButtonSpinnerHandleTheme.axaml` | ButtonSpinnerHandle | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `EmbeddedTextBox` | control theme | `EmbeddedTextBoxTheme.axaml` | NumericUpDown | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `InputClearIconButton` | control theme | `InputClearIconButtonTheme.axaml` | NumericUpDown | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ResizeHandle` | control theme | `ResizeHandleTheme.axaml` | NumericUpDown | `Background` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (Border) | `ResizeHandleTheme.axaml` | ResizeHandle | `Background` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RevealButton` | control theme | `RevealButtonTheme.axaml` | NumericUpDown | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SearchButton` | control theme | `SearchButtonTheme.axaml` | NumericUpDown | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SearchEditDecoratedBox` | control theme | `SearchEditDecoratedBoxTheme.axaml` | NumericUpDown | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (SearchEditPanel) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.LeftAddOnPart}` | template node (PixelAlignedBorder) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `LeftAddOn`, `LeftAddOnBorderThickness`, `LeftAddOnCornerRadius`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LeftAddOnPresenter` | template node (AddOnContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `LeftAddOn`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.RightAddOnPart}` | template node (SearchButton) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `EffectiveStatus`, `IsEnabled`, `IsSearchButtonLoading`, `SearchButtonText`, `SizeType`, `StyleVariant` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart}` | template node (AddOnDecoratedBoxContentFrame) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentLayout` | template node (DockPanel) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart}` | template node (AddOnContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart}` | template node (AddOnContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `ContentRightAddOn`, `ContentRightAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TextAreaDecoratedBox` | control theme | `TextAreaDecoratedBoxTheme.axaml` | NumericUpDown | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart}` | template node (AddOnDecoratedBoxContentFrame) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TextAreaContentFrame` | template node (Border) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Spinner` | `ButtonSpinner` | 输入壳体、外部 AddOn、内部前后缀、步进入口和 CompactSpace 状态承载。 |
| `PART_TextBox` | `TextBox` | 文本输入、占位符、只读、数据校验和文本双向绑定。 |
| `PART_ClearButton` | `InputClearIconButton` | 清除 `Value` 的内部按钮。 |
| `PART_InnerRightContentPresenter` | `ContentPresenter` | 用户 `InnerRightContent` 的内部右侧内容承载。 |
| `PART_DecreaseButton` | `IconButton` | `Mode=Spinner` 下触发减小步进。 |
| `PART_IncreaseButton` | `IconButton` | `Mode=Spinner` 下触发增加步进。 |

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

NumericUpDown 的交互优先级：

```text
Disabled
> ReadOnly
> Spin availability
> Pressed
> PointerOver
> Focus
> Normal
```

`Disabled` 表示控件不可交互，TextBox 文本使用禁用文字色，输入壳体使用禁用背景，浮动 Handle 不显示，清除按钮不应作为可操作入口出现。

`IsReadOnly` 保留文本可见但禁止编辑和步进。`AllowSpin=false` 禁止按钮、键盘和鼠标滚轮步进，但不等价于禁用控件。`IsKeyboardEnabled=false` 只拦截 `Up`、`Down`、`PageUp` 和 `PageDown` 的步进行为。

数值状态：

```text
Text input
  ↓ parse by TextConverter or ParsingNumberStyle
Value
  ↓ format by TextConverter / FormatString / NumberFormat
Text display
```

String mode 保留原始输入文本，并在能解析时同步 `Value`。Form 集成仍以 `Value` 作为表单值。

清除按钮有效状态：

```text
IsEffectiveShowClearButton =
  IsAllowClear
  && !IsReadOnly
  && !string.IsNullOrEmpty(Text)
```

`Mode` 只改变展示结构，不改变 `Value`、`Text`、`Minimum`、`Maximum`、`Increment`、`AllowSpin`、`ShowButtonSpinner`、键盘、滚轮、Form 或 string mode 的数值语义。

`SizeType=Custom` 进入自定义尺寸路径。用户未显式设置 `Height`、`FontSize`、`Padding` 等尺寸属性时，主题层应以 `Middle` 作为默认视觉基线；用户显式接管 `FontSize` 时，应通过 `IsCustomFontSize=true` 防止内部 `TextBox` 的 `SizeType` 字号样式覆盖用户设置。

## Theme and Token Boundaries

NumericUpDown 采用按需模板模型。`Mode=Input` 使用默认输入框模板，以 `ButtonSpinner` 作为输入壳体；`Mode=Spinner` 使用独立三段式拨轮模板，只在用户显式启用 spinner 模式时创建左右步进按钮和分隔视觉。

视觉层级要求：

- `Mode=Input` 的默认模板不得预埋 spinner 模式左右按钮或无职责 wrapper；`ShowButtonSpinner=false` 时必须隐藏浮动 Handle。
- `Mode=Spinner` 使用独立 `ControlTemplate`，不通过同一模板内两套视觉树加 `IsVisible` 切换实现；`ShowButtonSpinner=false` 时必须隐藏左右 action 段。
- `ButtonSpinner` 是默认输入壳体边界，不应被普通 `Border` 或 `Grid` 包装替代。
- `PART_TextBox` 的 `BorderThickness=0` 是为了避免内层 TextBox 与外层输入壳体重复绘制边框。
- `PART_ClearButton` 与 `PART_InnerRightContentPresenter` 共用内部右侧 stack，必须保留顺序：清除按钮在用户内部右侧内容之前。
- 浮动 Handle 由 `ButtonSpinnerDecoratedBox` 控制透明度和偏移，不应在 NumericUpDown 模板中动态创建或移除。

状态视觉由共享主题分层承担：

| 主题 | 职责 |
| --- | --- |
| `NumericUpDownTheme.axaml` | 装配控件结构和传递状态。 |
| `ButtonSpinnerTheme.axaml` | 装配 spinner 壳体和 Handle 内容。 |
| `ButtonSpinnerDecoratedBoxTheme.axaml` | 输入壳体、Addon、浮动 Handle 透明度和偏移。 |
| `ButtonSpinnerHandleTheme.axaml` | Handle 背景、边框、图标尺寸和交互视觉。 |
| `TextBoxTheme.axaml` | 文本编辑器、placeholder、disabled 文本色和内部文本 presenter。 |
| `AddOnDecoratedBoxTheme.axaml` | 输入 variant、focus、hover、error、warning、disabled 外观。 |

Token 边界：

NumericUpDownToken 是 NumericUpDown 的组件级 Token scope。它继承 `ButtonSpinnerToken`，以独立 `NumericUpDown` scope 提供数值输入控件可消费的输入壳体、步进 Handle、字体和尺寸语义。

该设计使 NumericUpDown 能复用 ButtonSpinner 输入壳体体系，同时保留控件级 Token scope。生成的 `NumericUpDownTokenKind` 表达 NumericUpDown scope 下可展示和可覆盖的 Token；默认主题中的输入壳体和 Handle 仍通过 `ButtonSpinnerTokenResource` 消费共享 ButtonSpinner 语义值。

NumericUpDownToken 不承载以下状态：

- `Value`、`Text`、`StringValue`、`Minimum`、`Maximum`、`Increment` 等实例数值状态。
- `IsStringMode`、`IsKeyboardEnabled`、`IsAllowClear` 等行为状态。
- `IsPointerOver`、`IsPressed`、`IsFocused`、`IsEnabled` 等交互状态。
- `EffectiveContentPadding`、`HandleOpacity`、`HandleOffset` 等模板运行时状态。

这些状态分别由 C# 状态模型、共享输入主题和 ButtonSpinnerDecoratedBox 内部属性处理。

## Customization Boundaries

维护 NumericUpDown 时必须保持以下不变量：

- 不修改继承自 Avalonia `NumericUpDown` 的数值、格式化、步进和事件契约。
- `ShowButtonSpinner` 必须同时作用于 `Mode=Input` 的浮动 Handle 和 `Mode=Spinner` 的左右 action 段。
- 不擅自新增、删除、重命名或改变 AtomUI public API。
- `Mode=Input` 默认行为和渲染效果不变；spinner 模式不能让默认用户承担额外视觉树或额外交互订阅成本。
- `Mode=Spinner` 只改变展示结构，不改变数值解析、格式化、步进、Form、CompactSpace 或 string mode 语义。
- `SizeType=Custom` 必须以 `Middle` 作为未显式覆盖时的默认视觉基线。
- `IsCustomFontSize=true` 时不得由内部 `TextBox` 的 `SizeType` 字体样式覆盖用户设置的 `FontSize`。
- `IsStringMode=true` 时必须保留原始 `StringValue`。
- `IsKeyboardEnabled=false` 只屏蔽步进快捷键，不屏蔽普通文本输入。
- 清除按钮只在允许清除、非只读且文本非空时显示。
- 禁用态下浮动 Handle 不显示；禁用文本必须使用 disabled 文本色。
- Filled 变体下 Handle 背景必须使用 `FilledHandleBg`。
- Template part 名称不变。
- CompactSpace 下的有效边框厚度、圆角和位置协同不变。
- Token 名称和语义不擅自重命名、删除或迁移为实例状态。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- `Value`、`Text`、`StringValue` 同步不递归、不丢失 raw text。
- string mode 内部同步标志必须通过成对 helper 设置和恢复，不能在多个调用点手写进入/退出逻辑。
- `SizeType=Custom` 以 `Middle` 作为未显式覆盖时的默认视觉基线。
- `IsCustomFontSize=true` 不能被内部 `TextBox` 的 `SizeType` 字体样式覆盖。
- `IsKeyboardEnabled=false` 只影响步进快捷键。
- `Mode=Input` 不承担 spinner 模式成本。
- `Mode=Spinner` 不复制数值增减算法。
- `ShowButtonSpinner=false` 同时隐藏默认输入模式浮动 Handle 和 spinner 模式左右 action 段。
- NumericUpDown 本体不直接订阅 spinner 模式左右按钮事件。
- 清除按钮和 `InnerRightContent` 顺序不变。
- 禁用态隐藏浮动 Handle，并命中 disabled 文本色。
- Filled Handle 背景来自 `FilledHandleBg`。
- CompactSpace 状态传递给输入壳体，不在 NumericUpDown 中另写边框折叠规则。
