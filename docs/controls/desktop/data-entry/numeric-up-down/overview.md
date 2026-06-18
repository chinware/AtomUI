# NumericUpDown 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.NumericUpDown` 桌面版的最新架构设计、数值输入模型、API 模型、模板结构、主题边界和验证策略。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，NumericUpDown Token 的专项设计见 [NumericUpDown Token 设计](token.md)，设计和契约变化记录见 [NumericUpDown Changelog](changelog.md)。

## 1. 控件定位

NumericUpDown 是 AtomUI 桌面数据录入体系中的数值输入控件，用于在文本输入、步进按钮、键盘和鼠标滚轮之间建立统一的十进制数值编辑体验。它以 Avalonia `NumericUpDown` 为基础，保留原生数值、格式化、步进和范围约束语义，并接入 AtomUI 的输入外观、Addon、CompactSpace、Form 和 Token 体系。

NumericUpDown 的职责是编辑单个 `decimal?` 数值，或在 string mode 下以 `StringValue` 保存高精度原始输入文本并同步可解析的 `Value`。它不承担表达式计算、单位换算、多值范围输入、校验消息展示、数据源管理或异步选择职责。需要复杂校验提示时应由 Form 体系承载；需要选择型数字项时应使用 Select 或 ComboBox 类控件。

Gallery 中该控件以 `NumberUpDown` 页面展示，这是文档和示例命名层面的展示名称；实际控件类型为 `AtomUI.Desktop.Controls.NumericUpDown`。

## 2. 设计语言

NumericUpDown 的设计语言来自输入框与微调按钮的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 数值语义 | 输入值是可增减的数值，而不是普通文本。 | `Value`、`Minimum`、`Maximum`、`Increment`、`FormatString`。 |
| 输入密度 | 控件在表单、工具栏或紧凑布局中的尺寸等级。 | `Large`、`Middle`、`Small`。 |
| 输入表面 | 控件边框和背景的视觉强度。 | `Outlined`、`Filled`、`Borderless`。 |
| 反馈状态 | 输入的校验和业务状态。 | `Default`、`Error`、`Warning`。 |
| 辅助内容 | 输入框前后附加说明、单位、协议、图标或清除入口。 | `LeftAddOn`、`RightAddOn`、`InnerLeftContent`、`InnerRightContent`、`IsAllowClear`。 |

步进 Handle 是输入框语义的一部分，不是独立操作按钮组。它应在普通状态下提供数值增减入口，在禁用状态下不显示可操作 Handle，在 Filled 状态下与输入表面保持同源弱背景。

## 3. 架构分层

NumericUpDown 架构按职责分层，避免数值解析、输入视觉、Addon、按钮 Handle 和表单集成相互耦合。

| 层 | 责任 | 主要载体 |
| --- | --- | --- |
| Public API | 暴露数值、文本、格式化、步进、输入外观、Addon、清除按钮和集成入口。 | `NumericUpDown.cs` 以及继承的 Avalonia `NumericUpDown` API |
| Numeric State | 维护 `Value`、`Text`、`StringValue`、格式化和解析之间的同步。 | `NumericUpDown.cs`、`NumericUpDownTextConverter` |
| Input Shell | 承载输入框边框、背景、状态、Addon、CompactSpace 和 spinner Handle。 | `ButtonSpinner`、`ButtonSpinnerDecoratedBox` |
| Template Contract | 定义 `PART_Spinner`、`PART_TextBox`、`PART_ClearButton` 和内部右侧内容 presenter。 | `NumericUpDownTheme.axaml` |
| Theme Mapping | 将尺寸、variant、status、disabled、filled Handle、清除按钮等状态映射为视觉属性。 | `NumericUpDownTheme.axaml`、ButtonSpinner / TextBox 主题 |
| Component Token | 以独立 `NumericUpDown` token scope 复用 ButtonSpinner 的输入与 Handle Token。 | `NumericUpDownToken.cs` |
| Integration | 与 Form、CompactSpace、Motion、Gallery 和 Avalonia 原生数值事件协同。 | `IFormItemAware`、`ICompactSpaceAware`、`IMotionAwareControl` |

状态流：

```text
Public API
  Value / Text / StringValue / IsStringMode
  Minimum / Maximum / Increment / FormatString / NumberFormat
  SizeType / StyleVariant / Status / IsAllowClear / AddOn
        ↓
Numeric State
  parse / format / clamp / value changed
  effective clear button visibility
        ↓
Input Shell
  ButtonSpinner + TextBox + clear button + addon presenters
        ↓
Theme Variables
  AddOnDecoratedBox visual state
  ButtonSpinner handle state
  TextBox disabled text state
        ↓
Template Visual
  Normal / PointerOver / Pressed / Focus / Disabled / Error / Warning
```

## 4. API 设计

NumericUpDown 的公共 API 由继承的数值编辑 API 与 AtomUI 输入扩展 API 组成。继承 API 的语义必须与 Avalonia `NumericUpDown` 保持一致，AtomUI 扩展只负责外观、辅助内容和集成能力。

### 4.1 数值编辑 API

| API | 类型 | 语义 |
| --- | --- | --- |
| `Value` | `decimal?` | 控件的数值值，支持双向绑定和数据校验。 |
| `Text` | `string?` | 当前显示文本，受 `TextConverter`、`FormatString` 和输入状态影响。 |
| `Minimum` / `Maximum` | `decimal` | 可接受数值范围，默认来自 Avalonia 基类。 |
| `Increment` | `decimal` | 单次步进增减值，默认 `1`。 |
| `ClipValueToMinMax` | `bool` | 文本转换为数值时是否裁剪到范围。 |
| `NumberFormat` | `NumberFormatInfo?` | 数值格式化和解析使用的区域格式。 |
| `FormatString` | `string` | 非编辑状态下的显示格式。 |
| `ParsingNumberStyle` | `NumberStyles` | 文本解析规则。 |
| `TextConverter` | `IValueConverter?` | 自定义 `Text` 与 `Value` 的双向转换器。 |
| `AllowSpin` | `bool` | 是否允许按钮、键盘和鼠标滚轮触发步进。 |
| `ShowButtonSpinner` | `bool` | 继承自 Avalonia 的 spinner 可见入口；AtomUI 桌面主题使用固定右侧浮动 Handle，不把该属性作为移除 Handle 的主题入口。 |
| `ButtonSpinnerLocation` | Avalonia `Location` | 继承自 Avalonia 的 spinner 位置入口；AtomUI 桌面主题的自定义 Handle 位置由 ButtonSpinner 模板保持右侧。 |

`ValueChanged` 和 `Spinned` 事件由 Avalonia 基类提供。NumericUpDown 不新增公开事件，也不新增公开方法。

### 4.2 String Mode API

| API | 类型 | 语义 |
| --- | --- | --- |
| `IsStringMode` | `bool` | 启用原始字符串值保存模式。 |
| `StringValue` | `string?` | string mode 下的原始输入文本。 |

`IsStringMode=false` 时，NumericUpDown 以 `Value` 为主状态。`IsStringMode=true` 时，`StringValue` 保存原始文本，控件尝试按当前文化和 `ParsingNumberStyle` 同步 `Value`；无法解析时 `Value` 置为 `null`，但原始文本不应被丢弃。

String mode 用于高精度小数、后端要求字符串传输或需要保留用户输入格式的场景。它不是独立的数据类型系统；步进、范围约束和 Form 集成仍以 `decimal? Value` 为可计算状态。

### 4.3 AtomUI 输入扩展 API

| API | 类型 | 语义 |
| --- | --- | --- |
| `SizeType` | `SizeType` | 输入框尺寸密度。 |
| `StyleVariant` | `InputControlStyleVariant` | 输入表面样式，支持 `Outlined`、`Filled`、`Borderless`。 |
| `Status` | `InputControlStatus` | 输入反馈状态，支持默认、错误和警告。 |
| `IsAllowClear` | `bool` | 是否展示清除按钮。 |
| `ClearIcon` | `PathIcon?` | 清除按钮图标，默认 `CloseCircleFilled`。 |
| `IsKeyboardEnabled` | `bool` | 是否允许上下方向键和 PageUp / PageDown 触发步进。 |
| `IsMotionEnabled` | `bool` | 是否启用输入壳体、Handle 和清除按钮相关动效。 |
| `LeftAddOn` / `RightAddOn` | `object?` | 外部前后附加内容。 |
| `LeftAddOnTemplate` / `RightAddOnTemplate` | `IDataTemplate?` | 外部附加内容模板。 |
| `InnerLeftContent` / `InnerRightContent` | 继承自 Avalonia `NumericUpDown` | 内部前后缀内容。 |
| `InnerLeftContentTemplate` / `InnerRightContentTemplate` | `IDataTemplate?` | 内部前后缀模板。 |

清除按钮只在 `IsAllowClear=true`、`IsReadOnly=false` 且 `Text` 非空时显示。清除动作将 `Value` 置为 `null`，并通过 Avalonia 文本和值同步机制更新显示状态。

## 5. 行为交互模型

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

`IsReadOnly` 保留文本可见但禁止编辑和步进。清除按钮在只读状态下隐藏。`AllowSpin=false` 禁止按钮、键盘和鼠标滚轮步进，但不等价于禁用控件；文本输入仍由 Avalonia 基类语义决定。

`IsKeyboardEnabled=false` 只拦截 `Up`、`Down`、`PageUp` 和 `PageDown` 的步进行为，不改变文本输入、焦点、鼠标或 spinner 可见性。该属性用于需要保留数字输入但不希望方向键改变数值的表单场景。

鼠标滚轮、按钮点击和键盘步进沿 Avalonia 基类的 `OnSpin` / `DoIncrement` / `DoDecrement` 语义更新 `Value`，并遵守 `Minimum`、`Maximum`、`Increment`、`IsReadOnly` 与 `AllowSpin`。

## 6. 状态模型

NumericUpDown 的状态模型由数值状态、文本状态和输入壳体状态组成。

数值同步：

```text
Text input
  ↓ parse by TextConverter or ParsingNumberStyle
Value
  ↓ format by TextConverter / FormatString / NumberFormat
Text display
```

String mode 同步：

```text
IsStringMode=true
  Text changed       → StringValue = raw text
  StringValue changed → Text = raw text, Value = parsed decimal? or null
  Value changed       → StringValue = raw decimal text
```

清除按钮有效状态：

```text
IsEffectiveShowClearButton =
  IsAllowClear
  && !IsReadOnly
  && !string.IsNullOrEmpty(Text)
```

输入壳体状态：

- `SizeType` 传递给 `ButtonSpinner`、`TextBox` 和内部 Handle。
- `StyleVariant` 传递给 `ButtonSpinner` 和 Handle，使 Filled / Borderless / Outlined 的背景、边框和 Handle 表面保持一致。
- `Status` 传递给 `ButtonSpinner`，由 AddOnDecoratedBox 体系映射错误和警告状态。
- `CompactSpaceItemPosition`、`CompactSpaceOrientation` 和 `IsUsedInCompactSpace` 只作为 CompactSpace 集成状态，不形成公开 API。

## 7. 模板与视觉架构

NumericUpDown 的模板以 `ButtonSpinner` 作为输入壳体，以 AtomUI `TextBox` 作为文本编辑器，以 `InputClearIconButton` 和 `ContentPresenter` 组成内部右侧内容区域。

稳定模板节点：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Spinner` | `ButtonSpinner` | 输入壳体、外部 AddOn、内部前后缀、浮动 Handle 和 CompactSpace 状态承载。 |
| `PART_TextBox` | `TextBox` | 文本输入、占位符、只读、数据校验和文本双向绑定。 |
| `PART_ClearButton` | `InputClearIconButton` | 清除 `Value` 的内部按钮。 |
| `PART_InnerRightContentPresenter` | `ContentPresenter` | 用户 `InnerRightContent` 的内部右侧内容承载。 |

默认视觉树形态：

```text
NumericUpDown
└─ ButtonSpinner#PART_Spinner
   ├─ ButtonSpinnerDecoratedBox
   │  ├─ outer add-ons
   │  ├─ content frame
   │  │  ├─ inner left content
   │  │  ├─ TextBox#PART_TextBox
   │  │  └─ inner right stack
   │  │     ├─ InputClearIconButton#PART_ClearButton
   │  │     └─ ContentPresenter#PART_InnerRightContentPresenter
   │  └─ floating spinner handle
   └─ ButtonSpinnerHandle
```

视觉层级要求：

- `ButtonSpinner` 是输入壳体边界，不应被普通 `Border` 或 `Grid` 包装替代。
- `PART_TextBox` 的 `BorderThickness=0` 是为了避免内层 TextBox 与外层输入壳体重复绘制边框。
- `PART_ClearButton` 与 `PART_InnerRightContentPresenter` 共用内部右侧 stack，必须保留顺序：清除按钮在用户内部右侧内容之前。
- 浮动 Handle 由 `ButtonSpinnerDecoratedBox` 控制透明度和偏移，不应在 NumericUpDown 模板中动态创建或移除。
- 禁用态隐藏 Handle 应通过共享 `ButtonSpinnerDecoratedBox` 状态完成，而不是在 NumericUpDown 模板中写特例。

## 8. Theme 架构

NumericUpDown Theme 位于 `src/AtomUI.Desktop.Controls/NumericUpDown/Themes/NumericUpDownTheme.axaml`。该主题只负责装配输入壳体和文本编辑器，并设置 NumericUpDown 自身默认值。

Theme 职责：

- 装配 `ButtonSpinner#PART_Spinner`。
- 将 `AllowSpin`、`IsEnabled`、`SizeType`、`StyleVariant`、`Status`、Addon、内部内容和 CompactSpace 状态传递给 `ButtonSpinner`。
- 固定使用可见且可浮动的 ButtonSpinner Handle。
- 装配 `TextBox#PART_TextBox`，并把 `Text`、`PlaceholderText`、`PlaceholderForeground`、`IsReadOnly` 和数据校验错误传递给 TextBox。
- 设置 `IsMotionEnabled`、`SpinnerHandleWidth`、`HorizontalAlignment`、`VerticalAlignment` 和 `PlaceholderForeground` 默认值。

状态视觉由共享主题分层承担：

| 主题 | 职责 |
| --- | --- |
| `NumericUpDownTheme.axaml` | 装配控件结构和传递状态。 |
| `ButtonSpinnerTheme.axaml` | 装配 spinner 壳体和 Handle 内容。 |
| `ButtonSpinnerDecoratedBoxTheme.axaml` | 输入壳体、Addon、浮动 Handle 透明度和偏移。 |
| `ButtonSpinnerHandleTheme.axaml` | Handle 背景、边框、图标尺寸、hover / pressed / disabled 视觉。 |
| `TextBoxTheme.axaml` | 文本编辑器、placeholder、disabled 文本色和内部文本 presenter。 |
| `AddOnDecoratedBoxTheme.axaml` | Outlined / Filled / Borderless、focus、hover、error、warning、disabled 外观。 |

NumericUpDown 不应在自身主题中复制 ButtonSpinner、TextBox 或 AddOnDecoratedBox 的状态 selector。共享输入视觉问题应修在共享主题层。

## 9. 控件家族或集成关系

NumericUpDown 属于 Data Entry 控件，与 LineEdit、TextBox、TextArea、Select、ComboBox、DatePicker、TimePicker 等控件共享输入尺寸、状态、variant、Addon 和 Form 集成语义。

集成关系：

- Avalonia `NumericUpDown`：继承数值编辑、格式化、步进、事件和基础文本同步语义。
- `ButtonSpinner`：提供输入壳体、步进 Handle、Addon、CompactSpace 和输入状态视觉。
- AtomUI `TextBox`：提供文本输入、占位符、禁用文本色和清除按钮相关基础能力。
- `IFormItemAware`：允许 Form 读取、设置、清空 `Value`，并把校验状态映射到 `Status`。
- `ICompactSpaceAware`：允许 CompactSpace 统一边框折叠和圆角。
- `IMotionAwareControl`：统一动效开关。

NumericUpDown 不应直接依赖 Gallery 结构，也不应把 Gallery 示例中的单位、图标或布局写入控件默认主题。

## 10. 兼容性不变量

维护 NumericUpDown 时必须保持以下不变量：

- 不修改继承自 Avalonia `NumericUpDown` 的 `Value`、`Text`、`Minimum`、`Maximum`、`Increment`、`FormatString`、`NumberFormat`、`ParsingNumberStyle`、`TextConverter`、`AllowSpin`、`ShowButtonSpinner`、`ButtonSpinnerLocation`、`ValueChanged` 和 `Spinned` API 契约。
- 不在未授权情况下改变 AtomUI 桌面主题对 `ShowButtonSpinner` 和 `ButtonSpinnerLocation` 的当前解释：自定义浮动 Handle 保持右侧并由 ButtonSpinner 体系控制显示状态。
- 不擅自新增、删除、重命名或改变 AtomUI public API：`IsStringMode`、`StringValue`、`IsKeyboardEnabled`、`IsAllowClear`、`ClearIcon`、`SizeType`、`StyleVariant`、`Status`、Addon 和内部内容模板属性。
- `IsStringMode=true` 时必须保留原始 `StringValue`，不能因 `decimal` 无法表达高精度输入而丢失文本。
- `IsKeyboardEnabled=false` 只屏蔽步进快捷键，不屏蔽普通文本输入。
- 清除按钮只在允许清除、非只读且文本非空时显示。
- 禁用态下浮动 Handle 不显示；禁用文本必须使用 disabled 文本色。
- Filled 变体下 Handle 背景必须使用 `FilledHandleBg`，不能固定为默认容器背景。
- `PART_Spinner`、`PART_TextBox`、`PART_ClearButton`、`PART_InnerRightContentPresenter` 名称不变。
- `PART_TextBox` 不绘制独立边框，外层壳体负责边框和背景。
- CompactSpace 下的有效边框厚度、圆角和位置协同不变。
- Token 名称和语义不擅自重命名、删除或迁移为实例状态。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

## 11. 专项模型

### 11.1 String Mode 模型

String mode 的核心目标是保存用户输入文本，同时尽可能同步可计算数值。

规则：

- `StringValue` 是原始输入状态，允许保存超出 `decimal` 精度表达能力的文本。
- `Value` 是可计算状态，仅在文本能解析为 `decimal` 时同步。
- 编辑焦点在 TextBox 内时，显示文本应尽量保持 raw text；失焦或显示格式刷新时才应用格式化策略。
- 用户设置自定义 `TextConverter` 时，关闭 string mode 后必须恢复用户转换器。
- 内部同步必须避免 `Text`、`Value` 和 `StringValue` 之间的递归更新。

### 11.2 浮动 Handle 模型

NumericUpDown 使用 ButtonSpinner 的浮动 Handle 模型：

- Handle normal 状态默认隐藏，pointer hover 输入壳体时显示。
- Handle 显示时通过 `HandleOpacity` 和 `HandleOffset` 动画进入，不改变控件整体布局尺寸。
- Handle 显示时内部右侧内容可产生位移，以避免内容与 Handle 重叠。
- 禁用态清除 hover 状态，并保持 `HandleOpacity=0`。
- Filled 状态的 Handle 背景由 `FilledHandleBg` 控制，disabled 状态由 shared disabled 背景控制。

该模型保证 NumericUpDown 在普通表单中保持输入框视觉密度，同时在可交互状态提供步进入口。

### 11.3 Form 集成模型

NumericUpDown 通过 `IFormItemAware` 暴露表单值能力：

```text
Form.SetValue(decimal?) → NumericUpDown.Value
NumericUpDown.ValueChanged → Form value changed
Form.ClearValue() → NumericUpDown.Value = null
Form.ValidateStatus → NumericUpDown.Status
```

Form 集成只以 `Value` 作为表单值，不直接使用 `StringValue`。需要提交原始字符串时，应由业务层绑定 `StringValue` 并明确处理。

## 12. 验证策略

NumericUpDown 改动应按文档、数值状态、AXAML/Theme、Token、Public API 和 Gallery 分层验证。

文档验证：

```bash
git diff --check
```

C# 状态验证：

- `IsStringMode` 开关时 `TextConverter` 保存与恢复正确。
- `StringValue`、`Text`、`Value` 的双向同步不丢失高精度原始文本。
- `IsKeyboardEnabled=false` 时拦截 `Up`、`Down`、`PageUp`、`PageDown` 步进行为。
- `IsAllowClear`、`IsReadOnly`、`Text` 改变时清除按钮有效可见状态正确。
- Form 设置、读取、清空和校验状态映射正确。
- CompactSpace 中边框厚度读取和位置变化正确。

AXAML/Theme 验证：

- `PART_Spinner`、`PART_TextBox`、`PART_ClearButton`、`PART_InnerRightContentPresenter` 名称稳定。
- Outlined、Filled、Borderless、Disabled、Error、Warning 状态外观稳定。
- Disabled 状态下 TextBox 文本色命中 disabled token。
- Disabled 状态下浮动 Handle 不显示。
- Filled 状态下 Handle 背景使用 `FilledHandleBg`。
- 清除按钮和 `InnerRightContent` 的顺序稳定。
- 不引入额外无职责 wrapper，不把共享 ButtonSpinner/TextBox 状态复制到 NumericUpDown 主题。

Token 验证：

- `NumericUpDownTokenKind` 与 Gallery Token 表保持一致。
- `NumericUpDownToken` 保持独立 scope，同时复用 ButtonSpinnerToken 的语义。
- Token 变更覆盖 ButtonSpinner 和 ComboBox 等共享输入 Handle 控件的影响面。

Gallery 验证：

- 运行 `NumberUpDownShowCasePageTests`，确认示例、API 表、Token 表和本地化结构稳定。
- 走查基础用法、string mode、键盘行为、鼠标滚轮、最小最大值、小数步进、尺寸、变体、禁用、前后缀、清除按钮和状态示例。
