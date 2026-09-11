# LineEdit Token 设计

本文档定义 `TextBoxToken`、`LineEditToken` 与 `TextAreaToken` 在 LineEdit 输入家族中的专属语义、分类、使用范围和兼容边界。控件 Token 的通用分层、命名、计算、Theme Variables 边界和预设色规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/development/control-token-guidelines.md)。LineEdit 整体架构见 [LineEdit 桌面版架构设计](overview.md)，内部实现原理见 [LineEdit 桌面版实现原理](implementation.md)，设计和契约变化记录见 [LineEdit Changelog](changelog.md)。

## 1. 定位

LineEdit 输入家族使用三个控件级 Token scope：

| Token | Scope | 职责 |
| --- | --- | --- |
| `TextBoxToken` | `TextBox` | TextBox 专属内容 padding。 |
| `LineEditToken` | `LineEdit` | 单行输入框字号。 |
| `TextAreaToken` | `TextArea` | 多行输入框字号、右侧附加 padding 和 resize handle 视觉。 |

TextBox / LineEdit / TextArea Token 不承载文本值、placeholder、清除状态、密码 reveal、Form 状态、SearchEdit 运行状态、focus/hover/pressed 状态或 CompactSpace 运行时状态。输入表面边框、背景、圆角、shadow、error/warning、disabled 和 motion 由 `InputControlFrameTheme` 与 `SharedToken` 处理；运行时状态由 `AbstractTextInput`、Form 和 frame 状态模型处理。

## 2. Token 分类

### 2.1 TextBox 专属文本布局 Token

- `TextBox.ContentPaddingLG`
- `TextBox.ContentPadding`
- `TextBox.ContentPaddingSM`

这些 Token 只服务 `TextBoxTheme.axaml` 中 TextBox 自身内容 padding。TextPresenter 的基础 margin 属于模板布局常量；输入表面边框、背景、圆角、hover/focus shadow、error/warning 和 disabled 视觉不属于 `TextBoxToken`。

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
InputControlFrameTheme
   ↓
TextBoxTheme / LineEditTheme / SearchEditDecoratedBoxTheme / TextAreaDecoratedBoxTheme

TextBoxToken / LineEditToken / TextAreaToken
   ↓
各自文本尺寸、padding 和 resize 专属布局
```

输入表面的边框、圆角、背景、hover/focus shadow、error/warning 和 disabled 状态由 `InputControlFrameTheme` 读取 `SharedToken`。TextBox 的内容 padding 由 `TextBoxTokenResource` 进入 `TextBoxTheme.axaml`；LineEdit 的单行字号由 `LineEditTokenResource` 消费；TextArea 的字号、右侧 padding 和 resize 视觉由 `TextAreaTokenResource` 及相关布局主题消费。

TextArea 的多行字号由 `TextAreaTokenResource` 进入 `TextAreaTheme.axaml`。TextArea 右侧 padding 和 resize 视觉由 `TextAreaDecoratedBoxTheme.axaml`、`ResizeHandleTheme.axaml` 消费。

SearchEdit 不定义独立 Token。它复用 `AbstractTextInput` 的状态与 LineEdit 的文本尺寸，输入表面由 `InputControlFrame` 统一表达，搜索按钮使用 Button 体系和 SearchButton 主题表达。

## 4. 控件家族影响

TextBoxToken 影响以下控件或主题：

- `TextBoxTheme.axaml`
- 使用 AtomUI `TextBox` 作为内部输入框的组合控件

LineEditToken 影响以下控件或主题：

- `LineEditTheme.axaml`
- `SearchEditTheme.axaml`
- LineEdit / SearchEdit token.md 语义说明和 Gallery 示例

TextAreaToken 影响以下控件或主题：

- `TextAreaTheme.axaml`
- `TextAreaDecoratedBoxTheme.axaml`
- `ResizeHandleTheme.axaml`
- TextArea token.md 语义说明和 Gallery 示例

如果调整 TextBoxToken 的内容 padding，必须评估基础 TextBox 以及依赖 AtomUI TextBox 的组合控件。输入表面边框、圆角、hover/focus shadow 和状态视觉的变更应评估 `InputControlFrameTheme` 及全部 decorated box 派生控件。DatePicker / TimePicker 等内部 picker 输入框在自定义尺寸路径中可能显式绑定字体并设置 `IsCustomFontSize=true`，不应依赖隐藏宽度补偿适配字号变化。

如果调整 LineEditToken 字号，必须评估 LineEdit、SearchEdit 以及复用 LineEdit 输入壳体的组合控件。

## 5. 兼容性要求

TextBoxToken、LineEditToken 与 TextAreaToken 属于输入控件家族的主题契约。即使 Token 类型是 internal，生成的 TokenKind、AXAML resource 使用点和 token.md 语义说明已经形成稳定依赖。

Token 变更要求：

- 不把实例状态、交互状态、Form 状态、搜索运行状态或文本内容状态迁移为 Token。
- 不在 TextBoxToken、LineEditToken 或 TextAreaToken 中复制 InputControlFrame 的边框、背景、圆角、focus shadow、error/warning、disabled 或 motion 状态 Token。
- 不在 TextAreaToken 中表达自动高度、当前行数或 resize 当前拖拽高度。
- `SizeType=Custom` 不新增专属字号 Token；它默认复用 Middle 分支，由用户显式属性完成自定义。
- 任何 Token scope 调整都必须同步其主题资源、generator 注册、控件文档和相关组合控件文档。

## 6. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| 修改 TextBox padding Token | 验证 TextBox 的 Large/Middle/Small/Custom padding、clear/reveal 对齐，并评估依赖 AtomUI TextBox 的组合控件。 |
| 修改 LineEdit 字号 Token | 验证 LineEdit、SearchEdit 的 Large/Middle/Small/Custom 字号、line height、placeholder 和 clear/reveal 对齐。 |
| 修改 TextArea 字号 Token | 验证 Lines、MinLines、MaxLines、IsAutoSize 和 IsResizable 下的高度计算。 |
| 修改 TextArea padding Token | 验证 clear button、Form feedback、InnerRightContent、字数统计和文本区域不重叠。 |
| 修改 resize Token | 验证 resize handle 可见性、拖拽命中、线条颜色和尺寸。 |
| 删除或重命名 Token | 默认不允许；如获授权，需同步 AXAML 引用、生成文件、Token 类型、生成数据和 token.md和控件文档。 |
| 文档改动 | 运行 `git diff --check`，检查文档链接存在。 |
