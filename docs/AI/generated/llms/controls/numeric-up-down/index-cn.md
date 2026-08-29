# NumericUpDown

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

NumericUpDown 是 AtomUI 桌面数据录入体系中的数值输入控件，用于在文本输入、步进按钮、键盘和鼠标滚轮之间建立统一的十进制数值编辑体验。它以 Avalonia `NumericUpDown` 为基础，保留原生数值、格式化、步进和范围约束语义，并接入 AtomUI 的输入外观、Addon、CompactSpace、Form 和 Token 体系。

NumericUpDown 的职责是编辑单个 `decimal?` 数值，或在 string mode 下以 `StringValue` 保存高精度原始输入文本并同步可解析的 `Value`。它不承担表达式计算、单位换算、多值范围输入、校验消息展示、数据源管理或异步选择职责。

Gallery 中该控件以 `NumberUpDown` 页面展示；实际控件类型为 `AtomUI.Desktop.Controls.NumericUpDown`。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown` |
| 状态 | Stable |

## 何时使用

NumericUpDown 的设计语言来自输入框与微调按钮的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 数值语义 | 输入值是可增减的数值，而不是普通文本。 | `Value`、`Minimum`、`Maximum`、`Increment`、`FormatString`。 |
| 输入密度 | 控件在表单、工具栏或紧凑布局中的尺寸等级。 | `Large`、`Middle`、`Small`、`Custom`。 |
| 输入表面 | 控件边框和背景的视觉强度。 | `Outlined`、`Filled`、`Borderless`。 |
| 反馈状态 | 输入的校验和业务状态。 | `Default`、`Error`、`Warning`。 |
| 展示模式 | 控件以输入框或三段式拨轮呈现。 | `Mode=Input`、`Mode=Spinner`。 |
| 辅助内容 | 输入框前后附加说明、单位、协议、图标或清除入口。 | `LeftAddOn`、`RightAddOn`、`InnerLeftContent`、`InnerRightContent`、`IsAllowClear`。 |

步进能力是输入框语义的一部分，不是独立操作按钮组。`Input` 模式使用右侧浮动 Handle，在普通状态下提供数值增减入口；`Spinner` 模式使用左减号、中间输入、右加号的三段式结构。

## 公共 API

NumericUpDown 的公共 API 由继承的数值编辑 API 与 AtomUI 输入扩展 API 组成。继承 API 的语义必须与 Avalonia `NumericUpDown` 保持一致，AtomUI 扩展只负责外观、辅助内容和集成能力。

数值编辑 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Value` | `decimal?` | 控件的数值值，支持双向绑定和数据校验。 |
| `Text` | `string?` | 当前显示文本，受转换器、格式化和输入状态影响。 |
| `Minimum` / `Maximum` | `decimal` | 可接受数值范围。 |
| `Increment` | `decimal` | 单次步进增减值。 |
| `ClipValueToMinMax` | `bool` | 文本转换为数值时是否裁剪到范围。 |
| `NumberFormat` | `NumberFormatInfo?` | 数值格式化和解析使用的区域格式。 |
| `FormatString` | `string` | 非编辑状态下的显示格式。 |
| `ParsingNumberStyle` | `NumberStyles` | 文本解析规则。 |
| `TextConverter` | `IValueConverter?` | 自定义 `Text` 与 `Value` 的双向转换器。 |
| `AllowSpin` | `bool` | 是否允许按钮、键盘和鼠标滚轮触发步进。 |
| `ShowButtonSpinner` | `bool` | 是否显示步进按钮；`Mode=Input` 控制浮动 Handle，`Mode=Spinner` 控制左右 action 段。 |

String mode API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `IsStringMode` | `bool` | 启用原始字符串值保存模式。 |
| `StringValue` | `string?` | string mode 下的原始输入文本。 |

AtomUI 输入扩展 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Mode` | `NumericUpDownMode` | 控件展示模式，默认 `Input`，可设置为 `Spinner`。 |
| `SizeType` | `CustomizableSizeType` | 输入框尺寸密度；`Custom` 未显式覆盖时以 `Middle` 为视觉基线。 |
| `IsCustomFontSize` | `bool` | 为 `true` 时内部 `TextBox` 不由 `SizeType` 字号样式覆盖 `FontSize`。 |
| `StyleVariant` | `InputControlStyleVariant` | 输入表面样式。 |
| `Status` | `InputControlStatus` | 显式输入反馈状态；最终视觉由 `InputControlFrame.EffectiveStatus` 计算，native validation error 以 `DataValidationErrors` 为唯一真源。 |
| `IsAllowClear` | `bool` | 是否展示清除按钮。 |
| `ClearIcon` | `PathIcon?` | 清除按钮图标。 |
| `IsKeyboardEnabled` | `bool` | 是否允许方向键和 PageUp / PageDown 触发步进。 |
| `IsMotionEnabled` | `bool` | 是否启用输入壳体、Handle 和清除按钮相关动效。 |
| `LeftAddOn` / `RightAddOn` | `object?` | 外部前后附加内容。 |
| `LeftAddOnTemplate` / `RightAddOnTemplate` | `object?` | 外部附加内容模板；实际模板契约为 `IDataTemplate?`。 |
| `InnerLeftContent` / `InnerRightContent` | `object?` | 内部前后缀内容。 |
| `InnerLeftContentTemplate` / `InnerRightContentTemplate` | `object?` | 内部前后缀模板；实际模板契约为 `IDataTemplate?`。 |

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Spinner` | `ButtonSpinner` | `InputControlFrame` / AddOnDecoratedBox 组合、外部 AddOn、内部前后缀、步进入口和 CompactSpace 布局承载。 |
| `PART_TextBox` | `TextBox` | 文本输入、占位符、只读、数据校验和文本双向绑定。 |
| `PART_ClearButton` | `InputClearIconButton` | 清除 `Value` 的内部按钮。 |
| `PART_InnerRightContentPresenter` | `ContentPresenter` | 用户 `InnerRightContent` 的内部右侧内容承载。 |
| `PART_DecreaseButton` | `IconButton` | `Mode=Spinner` 下触发减小步进。 |
| `PART_IncreaseButton` | `IconButton` | `Mode=Spinner` 下触发增加步进。 |

## 事件与命令

NumericUpDown 的事件与命令以控件文档、源码 public surface 和 Avalonia 基类契约为准；生成器不从源码发明额外事件。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownShowCase.axaml:42`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:NumericUpDown Value="3"
```

### 拨轮

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownShowCase.axaml:55`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical"
            Spacing="{atom:SharedTokenResource UniformlyMargin}">
    <atom:NumericUpDown Mode="Spinner"
                        Minimum="1"
                        Maximum="10"
                        Value="3"
                        FormatString="0"
                        PlaceholderText="Outlined"
                        Width="150"
                        HorizontalAlignment="Left" />
    <atom:NumericUpDown Mode="Spinner"
                        Minimum="1"
                        Maximum="10"
                        Value="3"
                        FormatString="0"
                        PlaceholderText="Filled"
                        Width="150"
                        HorizontalAlignment="Left"
                        StyleVariant="Filled" />
</StackPanel>
```

### 隐藏步进按钮

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownShowCase.axaml:86`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Vertical"
            Spacing="{atom:SharedTokenResource UniformlyMargin}">
    <Grid ColumnDefinitions="*,*">
        <atom:NumericUpDown Grid.Column="0"
                            Minimum="1"
                            Maximum="10"
                            Value="3"
                            ShowButtonSpinner="False" />
    </Grid>
    <Grid ColumnDefinitions="*,*">
        <atom:NumericUpDown Grid.Column="0"
                            Mode="Spinner"
                            Minimum="1"
                            Maximum="10"
                            Value="3"
                            ShowButtonSpinner="False" />
    </Grid>
</StackPanel>
```

### 字符串模式（高精度）

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownShowCase.axaml:114`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:NumericUpDown IsStringMode="True"
                        StringValue="{Binding StringModeValue, Mode=TwoWay}"
                        Increment="0.0001"
                        Minimum="0"
                        Maximum="100"
                        PlaceholderText="输入重量" />
    <TextBlock>
        <Run Text="原始值：" />
        <Run Text="{Binding StringModeValue}" />
    </TextBlock>
</StackPanel>
```

## 状态模型

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

## 主题与 Design Token

NumericUpDown 采用按需模板模型。`Mode=Input` 使用默认输入框模板，以 `ButtonSpinner` 作为输入壳体；`Mode=Spinner` 使用独立三段式拨轮模板，只在用户显式启用 spinner 模式时创建左右步进按钮和分隔视觉。

视觉层级要求：

- `Mode=Input` 的默认模板不得预埋 spinner 模式左右按钮或无职责 wrapper；`ShowButtonSpinner=false` 时必须隐藏浮动 Handle。
- `Mode=Spinner` 使用独立 `ControlTemplate`，不通过同一模板内两套视觉树加 `IsVisible` 切换实现；`ShowButtonSpinner=false` 时必须隐藏左右 action 段。
- `ButtonSpinner` 是默认输入组合边界，复用 `InputControlFrame` 的输入表面和有效状态，不应被普通 `Border` 或 `Grid` 包装替代。
- `PART_TextBox` 的 `BorderThickness=0` 是为了避免内层 TextBox 与外层输入壳体重复绘制边框。
- `PART_ClearButton` 与 `PART_InnerRightContentPresenter` 共用内部右侧 stack，必须保留顺序：清除按钮在用户内部右侧内容之前。
- 浮动 Handle 由 `ButtonSpinnerDecoratedBox` 控制透明度和偏移，不应在 NumericUpDown 模板中动态创建或移除。

状态视觉由共享主题分层承担：

| 主题 | 职责 |
| --- | --- |
| `NumericUpDownTheme.axaml` | 装配控件结构和传递状态。 |
| `NumericUpDownSpinnerTheme.axaml` | 装配 NumericUpDown 专用 inline spinner 模板及其 action 按钮状态。 |
| `ButtonSpinnerTheme.axaml` | 装配 spinner 壳体和 Handle 内容。 |
| `ButtonSpinnerDecoratedBoxTheme.axaml` | 输入壳体、Addon、浮动 Handle 透明度和偏移。 |
| `ButtonSpinnerHandleTheme.axaml` | Handle 背景、边框、图标尺寸和交互视觉。 |
| `TextBoxTheme.axaml` | 文本编辑器、placeholder、disabled 文本色和内部文本 presenter。 |
| `InputControlFrameTheme.axaml` | 输入 variant、effective status、focus、hover、pressed、error、warning、disabled 和 motion 外观。 |

Token 来源：

NumericUpDownToken 是 NumericUpDown 的控件级 Token scope。它继承 `ButtonSpinnerToken`，以独立 `NumericUpDown` scope 提供步进 Handle、字体和尺寸语义；输入表面边框、背景、圆角、focus shadow、error/warning、disabled 和 motion 统一由 `InputControlFrameTheme` 与 `SharedToken` 提供。

该设计使 NumericUpDown 能复用 ButtonSpinner 输入壳体体系，同时保留控件级 Token scope。生成的 `NumericUpDownTokenKind` 表达 NumericUpDown scope 下可展示和可覆盖的 Token；默认主题中的输入壳体和 Handle 仍通过 `ButtonSpinnerTokenResource` 消费共享 ButtonSpinner 语义值。

NumericUpDownToken 不承载以下状态：

- `Value`、`Text`、`StringValue`、`Minimum`、`Maximum`、`Increment` 等实例数值状态。
- `IsStringMode`、`IsKeyboardEnabled`、`IsAllowClear` 等行为状态。
- `IsPointerOver`、`IsPressed`、`IsFocused`、`IsEnabled` 等交互状态。
- `EffectiveContentPadding`、`HandleOpacity`、`HandleOffset` 等模板运行时状态。

这些状态分别由 C# 状态模型、共享输入主题和 ButtonSpinnerDecoratedBox 内部属性处理。

## AOT 与裁剪注意事项

NumericUpDown 不通过反射访问 ButtonSpinner、TextBox 或 AddOnDecoratedBox 内部状态。跨控件协同通过公开属性、稳定 template part 和接口完成。

`Mode=Input` 是默认路径，必须避免 spinner 模式额外节点和额外按钮事件订阅。共享视觉问题应修在 `InputControlFrameTheme` 或 ButtonSpinner 布局主题层；NumericUpDown 专用的 inline 按钮视觉应修在 NumericUpDownSpinnerTheme 中，不能在 NumericUpDown 主题中复制跨模板 selector。

Token 通过动态资源进入主题。NumericUpDown 不把实例状态、当前值、文本、按钮 enabled 状态或模板切换状态写入 Token。

`IsCustomFontSize` 只通过局部绑定影响内部 `TextBox` 的字号样式选择，不应引入全局资源、反射访问或跨模板缓存。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/NumericUpDown/NumericUpDown.cs`：public API、数值同步、custom size、string mode、清除按钮、键盘处理、Form / CompactSpace / Motion 接口。
- `src/AtomUI.Desktop.Controls/NumericUpDown/NumericUpDownSpinner.cs`：internal Spinner 子控件，复用 ButtonSpinner 的数值步进语义并承载 NumericUpDown 专用 inline 模板。
- `src/AtomUI.Desktop.Controls/NumericUpDown/NumericUpDownToken.cs`：NumericUpDown Token scope。
- `src/AtomUI.Desktop.Controls/NumericUpDown/Themes/NumericUpDownTheme.axaml`：输入模式和 spinner 模式外层模板、状态传递。
- `src/AtomUI.Desktop.Controls/NumericUpDown/Themes/NumericUpDownSpinnerTheme.axaml`：NumericUpDownSpinner 的 inline 模板、分隔线、加减按钮和按钮状态视觉。
- `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinner.cs`：输入壳体和 spin 入口。
- `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinnerDecoratedBox.cs`：在 shared frame 上扩展浮动 Handle、Addon、CompactSpace 和 spinner 布局。
- `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinnerHandle.cs`：Handle 按钮和上下箭头。
- `src/AtomUI.Desktop.Controls/Input/TextBox.cs`：文本编辑器。
- `src/AtomUI.Desktop.Controls/Input/InputClearIconButton.cs`：清除按钮。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/numeric-up-down/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/numeric-up-down/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/numeric-up-down/token.md`
- 变更记录：`docs/controls/desktop/data-entry/numeric-up-down/changelog.md`
- 语义结构：`./semantic-cn.md`
