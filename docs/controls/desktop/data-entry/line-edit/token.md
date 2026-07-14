# LineEdit Token 设计

本文档定义 `TextBoxToken`、`LineEditToken` 与 `TextAreaToken` 在 LineEdit 输入家族中的专属语义、分类、使用范围和兼容边界。控件 Token 的通用分层、命名、计算、Theme Variables 边界和预设色规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/control-token-guidelines.md)。LineEdit 整体架构见 [LineEdit 桌面版架构设计](overview.md)，内部实现原理见 [LineEdit 桌面版实现原理](implementation.md)，设计和契约变化记录见 [LineEdit Changelog](changelog.md)。

## 1. 定位

LineEdit 输入家族使用三个组件级 Token scope：

| Token | Scope | 职责 |
| --- | --- | --- |
| `TextBoxToken` | `TextBox` | 基础文本框边框、圆角、尺寸 padding、hover/focus 边框和 focus shadow。 |
| `LineEditToken` | `LineEdit` | 单行输入框字号。 |
| `TextAreaToken` | `TextArea` | 多行输入框字号、右侧附加 padding 和 resize handle 视觉。 |

TextBox / LineEdit / TextArea Token 不承载文本值、placeholder、清除状态、密码 reveal、Form 状态、SearchEdit 运行状态、focus/hover/pressed 状态或 CompactSpace 运行时状态。这些状态分别由控件实例属性、共享输入主题、AddOnDecoratedBox、Form 和 C# 状态模型处理。

## 2. Token 分类

### 2.1 TextBox 边框与 padding Token

- `TextBox.BorderRadius`
- `TextBox.BorderRadiusLG`
- `TextBox.BorderRadiusSM`
- `TextBox.BorderColor`
- `TextBox.BorderThickness`
- `TextBox.Padding`
- `TextBox.PaddingLG`
- `TextBox.PaddingSM`
- `TextBox.HoverBorderColor`
- `TextBox.ActiveBorderColor`
- `TextBox.ActiveShadow`

这些 Token 只服务 `TextBoxTheme.axaml` 中基础文本框边框本身的默认、hover、focus 视觉，以及 Large/Middle/Small/Custom 尺寸下的 TextBox 自身 padding。字号、行高、placeholder、disabled 文本色、选区、光标和 clear/reveal/action 间距继续使用 SharedToken；`TextBoxToken` 不承载 AddOn、InnerLeftContent/InnerRightContent、输入壳体或组合控件语义。

### 2.2 LineEdit 字号 Token

- `LineEdit.InputFontSize`
- `LineEdit.InputFontSizeLG`
- `LineEdit.InputFontSizeSM`

这些 Token 保持 LineEdit 单行输入框字号契约。当前默认计算语义与 TextBox 字号一致，供 `LineEdit` 及复用 LineEdit 输入壳体的组合控件独立覆盖。

### 2.3 TextArea 字号 Token

- `TextArea.FontSize`
- `TextArea.FontSizeLG`
- `TextArea.FontSizeSM`

这些 Token 服务 `TextAreaTheme.axaml` 中多行输入不同尺寸下的默认字体大小。TextArea 的行高使用 SharedToken 的 `FontHeight`、`FontHeightLG`、`FontHeightSM`。

### 2.4 TextArea 右侧附加 padding Token

- `TextArea.RightAddOnPadding`
- `TextArea.RightAddOnPaddingLG`
- `TextArea.RightAddOnPaddingSM`

这些 Token 用于 TextArea 内部右侧内容与边框之间的空隙。它们根据 SharedToken 的水平 padding 和线宽计算，避免右侧 clear / feedback / resize 相关内容与文本区域重叠。

### 2.5 TextArea resize Token

- `TextArea.ResizeIndicatorLineColor`
- `TextArea.ResizeHandleSize`

这些 Token 控制 TextArea 右下角 resize handle 的线条颜色和尺寸。它们只服务 `IsResizable=true` 的拖拽入口，不影响 TextArea 的自动高度算法或固定行数测量。

## 3. 控件专项模型中的 Token 使用

LineEdit 家族 Token 使用路径：

```text
SharedToken
   ↓
TextBoxToken / LineEditToken / TextAreaToken
   ↓
TextBoxTheme / TextAreaTheme / TextAreaDecoratedBoxTheme / ResizeHandleTheme
   ↓
TextPresenter + input action visuals
```

基础 TextBox 的边框、圆角、padding、hover/focus 边框和 focus shadow 由 `TextBoxTokenResource` 进入 `TextBoxTheme.axaml`。TextBox 的字号、行高、placeholder、disabled 文本色、选区、光标和 clear/reveal/action 间距直接使用 SharedToken。LineEdit 自身的边框、背景、focus shadow、error/warning 和 disabled 状态不定义专属 Token，而是通过 SharedToken 和 AddOnDecoratedBoxToken 表达。

TextArea 的多行字号由 `TextAreaTokenResource` 进入 `TextAreaTheme.axaml`。TextArea 右侧 padding 和 resize 视觉由 `TextAreaDecoratedBoxTheme.axaml`、`ResizeHandleTheme.axaml` 消费。

SearchEdit 不定义独立 Token。它复用 LineEdit 的文本输入字号和 AddOnDecoratedBox 的输入壳体状态，搜索按钮使用 Button 体系和 SearchButton 主题表达。

## 4. 控件家族影响

TextBoxToken 影响以下控件或主题：

- `TextBoxTheme.axaml`
- 使用 AtomUI `TextBox` 作为内部输入框的组合控件

LineEditToken 影响以下控件或主题：

- `LineEditTheme.axaml`
- `SearchEditTheme.axaml`
- Gallery LineEdit / SearchEdit 示例和 Token 表

TextAreaToken 影响以下控件或主题：

- `TextAreaTheme.axaml`
- `TextAreaDecoratedBoxTheme.axaml`
- `ResizeHandleTheme.axaml`
- Gallery TextArea 示例和 Token 表

如果调整 TextBoxToken 边框、圆角、padding、hover/focus 边框或 focus shadow，必须评估基础 TextBox 以及依赖 AtomUI TextBox 的组合控件。DatePicker / TimePicker 等内部 picker 输入框在自定义尺寸路径中可能显式绑定字体并设置 `IsCustomFontSize=true`，不应依赖隐藏宽度补偿适配字号变化。

如果调整 LineEditToken 字号，必须评估 LineEdit、SearchEdit 以及复用 LineEdit 输入壳体的组合控件。

## 5. 兼容性要求

TextBoxToken、LineEditToken 与 TextAreaToken 属于输入控件家族的主题契约。即使 Token 类型是 internal，生成的 TokenKind、AXAML resource 使用点和 Gallery Token 表已经形成稳定依赖。

Token 变更要求：

- 不擅自重命名、删除或迁移既有 Token。
- 不把实例状态、交互状态、Form 状态、搜索运行状态或文本内容状态迁移为 Token。
- 不在 LineEditToken 中复制 AddOnDecoratedBox 的边框、背景、focus shadow、error/warning 或 disabled 状态 Token。
- 不在 TextAreaToken 中表达自动高度、当前行数或 resize 当前拖拽高度。
- `SizeType=Custom` 不新增专属字号 Token；它默认复用 Middle 分支，由用户显式属性完成自定义。
- 需要破坏性变更时，必须先说明影响范围并获得授权。

## 6. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| 修改 TextBox 边框或 padding Token | 验证 TextBox 的 Large/Middle/Small/Custom border、corner radius、padding、hover/focus 和 clear/reveal 对齐，并评估依赖 AtomUI TextBox 的组合控件。 |
| 修改 LineEdit 字号 Token | 验证 LineEdit、SearchEdit 的 Large/Middle/Small/Custom 字号、line height、placeholder 和 clear/reveal 对齐。 |
| 修改 TextArea 字号 Token | 验证 Lines、MinLines、MaxLines、IsAutoSize 和 IsResizable 下的高度计算。 |
| 修改 TextArea padding Token | 验证 clear button、Form feedback、InnerRightContent、字数统计和文本区域不重叠。 |
| 修改 resize Token | 验证 resize handle 可见性、拖拽命中、线条颜色和尺寸。 |
| 删除或重命名 Token | 默认不允许；如获授权，需同步 AXAML 引用、生成文件、Gallery Token 表和控件文档。 |
| 文档改动 | 运行 `git diff --check`，检查文档链接存在。 |
