# LineEdit 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.LineEdit` 输入控件家族的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/development/control-development-guidelines.md)，内部实现原理见 [LineEdit 桌面版实现原理](implementation.md)，Token 专项设计见 [LineEdit Token 设计](token.md)，设计和契约变化记录见 [LineEdit Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit` |
| 控件状态 | Stable |

LineEdit 是 AtomUI 桌面数据录入体系中的文本输入控件家族，用于承载单行文本、密码输入、搜索输入和多行文本输入。它以 Avalonia `TextBox` 文本编辑能力为基础，接入 AtomUI 的尺寸、输入表面、native validation error 投射、清除按钮、密码 reveal、字数统计、Addon、CompactSpace、Form 和 Token 体系。

LineEdit 家族包含以下稳定入口：

| 类型 | 定位 |
| --- | --- |
| `TextBox` | AtomUI 基础文本框，提供尺寸、清除、密码 reveal、字数统计、Form 和 CompactSpace 能力。 |
| `LineEdit` | 标准单行输入框，在 `TextBox` 基础上加入输入表面、验证视觉投射和外部 AddOn。 |
| `SearchEdit` | 搜索输入框，在 `LineEdit` 基础上加入搜索按钮、搜索按钮样式、加载态和搜索事件；独立契约见 [SearchEdit 桌面版架构设计](../search-edit/overview.md)。 |
| `TextArea` | 多行输入框，独立继承 Avalonia `TextBox`，复用输入状态、尺寸、清除、字数统计、Form 和 TextArea 专属 resize 模型。 |

LineEdit 家族只负责文本输入与文本相关辅助操作，不负责结构化选择、日期时间选择、数值步进、异步候选数据管理、富文本编辑或表达式计算。

## 2. 设计语言

LineEdit 的设计语言来自可编辑文本表面和输入辅助内容的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 文本语义 | 用户输入或查看原始文本。 | `Text`、`PlaceholderText`、`IsReadOnly`、`PasswordChar`。 |
| 输入密度 | 控件在表单和工具栏中的尺寸等级。 | `SizeType=Large/Middle/Small/Custom`。 |
| 输入表面 | 边框和背景的视觉强度。 | `Outlined`、`Filled`、`Borderless`、`Underlined`。 |
| 反馈状态 | 输入的校验和业务状态。 | `Default`、`Error`、`Warning`。 |
| 辅助内容 | 外部附加、内部前后缀、清除、密码显示、字数统计和 Form feedback。 | AddOn、`InnerLeftContent`、`InnerRightContent`、`IsAllowClear`、`IsEnableRevealButton`、`IsShowCount`。 |
| 专用输入 | 搜索、多行、自动高度和可拖拽调整高度。 | `SearchEdit`、`TextArea`、`IsAutoSize`、`IsResizable`。 |

`Custom` 尺寸不是第四套预设 Token。它以 `Middle` 作为未显式设置时的视觉基线，并允许用户通过 `Height`、`FontSize`、`Padding` 等常规属性覆盖实际尺寸。

## 3. API 与契约模型

LineEdit 家族的公共 API 由 Avalonia 文本编辑 API 与 AtomUI 输入扩展 API 组成。Avalonia API 的文本编辑、选择、滚动、密码、换行、只读和数据校验语义必须保持兼容；AtomUI 扩展只负责外观、辅助内容和集成能力。

基础文本 API：

| API | 语义 |
| --- | --- |
| `Text` | 当前输入文本，支持双向绑定。 |
| `PlaceholderText` | 空文本状态下的占位提示。 |
| `IsReadOnly` | 文本可见但禁止编辑。 |
| `PasswordChar` / `RevealPassword` | 密码输入和 reveal 状态。 |
| `MaxLength` | 文本最大长度，字数统计显示依赖该值。 |
| `SelectionStart` / `SelectionEnd` / `CaretIndex` | 文本选择和光标位置。 |

AtomUI 输入扩展 API：

| API | 所属类型 | 语义 |
| --- | --- | --- |
| `SizeType` | `TextBox`、`LineEdit`、`SearchEdit`、`TextArea` | 输入尺寸密度，类型为 `CustomizableSizeType`。 |
| `IsCustomFontSize` | `TextBox`、`LineEdit`、`SearchEdit` | 为 `true` 时不由 SizeType 样式覆盖 `FontSize`。 |
| `IsAllowClear` | `TextBox`、`LineEdit`、`SearchEdit`、`TextArea` | 是否允许展示清除按钮。 |
| `ClearIcon` | `TextBox`、`LineEdit`、`SearchEdit`、`TextArea` | 清除按钮图标。 |
| `IsEnableRevealButton` | `TextBox`、`LineEdit`、`SearchEdit` | 是否展示密码 reveal 按钮。 |
| `IsShowCount` | `TextBox`、`LineEdit`、`SearchEdit`、`TextArea` | 是否展示字数统计。 |
| `IsMotionEnabled` | 全家族 | 是否启用内部按钮动效。 |
| `StyleVariant` | `LineEdit`、`SearchEdit`、`TextArea` | 输入表面样式。 |
| `Status` | `LineEdit`、`SearchEdit`、`TextArea` | 手动输入反馈状态；native validation error 以 `DataValidationErrors` 为最高优先级。 |
| `LeftAddOn` / `LeftAddOnTemplate` | `LineEdit`、`SearchEdit` | 外部左侧附加内容和模板。 |
| `RightAddOn` / `RightAddOnTemplate` | `LineEdit` | 外部右侧附加内容和模板；SearchEdit 的右侧外部 add-on 位置由搜索按钮占用。 |
| `InnerLeftContentTemplate` / `InnerRightContentTemplate` | `LineEdit`、`TextArea` | 内部前后缀模板。 |

SearchEdit 专项 API：

| API | 语义 |
| --- | --- |
| `SearchButtonStyle` | 搜索按钮样式，支持 `Default` 和 `Primary`。 |
| `SearchButtonText` | 搜索按钮文字。 |
| `IsOperating` | 搜索按钮加载态；加载期间搜索按钮点击不重复触发事件。 |
| `SearchButtonClick` | 搜索按钮点击路由事件。 |

TextArea 专项 API：

| API | 语义 |
| --- | --- |
| `Lines` | 固定行数模式下的显示行数。 |
| `MinLines` / `MaxLines` | 自动高度和 resize 的行数边界。 |
| `IsAutoSize` | 是否根据文本内容自动调整高度。 |
| `IsResizable` | 是否展示右下角 resize handle。 |

稳定 template part：

| Template Part | 类型 | 所属主题 | 职责 |
| --- | --- | --- | --- |
| `PART_AddOnDecoratedBox` | `AddOnDecoratedBox` / 派生 decorated box | `LineEditTheme`、`SearchEditTheme`、`TextAreaTheme` | 输入壳体、Addon、状态视觉和 CompactSpace 状态承载。 |
| `PART_ScrollViewer` / `ScrollViewer` | `ScrollViewer` | 全家族 | 文本滚动区域。 |
| `PART_TextPresenter` | `InputTextPresenter` | 全家族 | 文本显示、光标、选择和密码 reveal。 |
| `Placeholder` | `TextBlock` | 全家族 | 空文本占位提示。 |
| `PART_ClearButton` | `InputClearIconButton` | 全家族 | 清除当前文本。 |
| `PART_RevealButton` | `RevealButton` | `TextBox`、`LineEdit`、`SearchEdit` | 切换密码 reveal。 |
| `FormFeedBack` / `PART_FormFeedBack` | `ContentPresenter` | `LineEdit`、`TextBox`、`TextArea` | Form feedback 内容承载。 |
| `InnerRightContentPresenter` / `PART_InnerRightContentPresenter` | `ContentPresenter` | `LineEdit`、`SearchEdit`、`TextArea` | 内部右侧内容承载。 |
| `TextCountIndicator` | `TextBlock` | 全家族 | 字数统计显示。 |
| `PART_ResizeHandle` | `ResizeHandle` | `TextAreaTheme` | TextArea 高度拖拽入口。 |

## 4. 行为与状态模型

LineEdit 家族的交互优先级：

```text
Disabled
> ReadOnly
> Error / Warning
> Focus
> PointerOver
> Normal
```

`Disabled` 表示不可交互，文本使用 disabled 文本色，内部按钮不可作为操作入口。`IsReadOnly` 保持文本可见和可选中，但不允许编辑或清除。

清除按钮有效状态：

```text
TextBox / LineEdit / SearchEdit:
IsEffectiveShowClearButton =
  IsAllowClear
  && !IsReadOnly
  && !AcceptsReturn
  && !string.IsNullOrEmpty(Text)

TextArea:
IsEffectiveShowClearButton =
  IsAllowClear
  && !IsReadOnly
  && !string.IsNullOrEmpty(Text)
```

Form 集成以 `Text` 作为表单值。错误校验状态以 Avalonia `DataValidationErrors` 为真源，Form validator 产生的 error 应写入同一 native validation 通道；`IFormItemAware.NotifyValidateStatus` 只负责同步 `Warning`、`Success`、`Validating` 等 Form 扩展状态和 feedback 可见性。feedback 内容通过 `IFormItemFeedbackAware` 进入模板中的 feedback presenter。

CompactSpace 只影响相邻输入框之间的有效圆角和边框折叠，不改变文本编辑语义。

## 5. 视觉与主题模型

LineEdit 家族使用输入壳体和文本 presenter 分层：

| 主题 | 职责 |
| --- | --- |
| `TextBoxTheme.axaml` | 基础文本框模板、边框、padding、清除按钮、reveal、字数统计、基础 SizeType 字号和 TextPresenter margin。 |
| `LineEditTheme.axaml` | 单行输入壳体、外部 AddOn、variant/status/focus 视觉，并把 native validation error 投射到外层输入壳体。 |
| `SearchEditTheme.axaml` | 搜索输入壳体、搜索按钮状态传递和搜索按钮布局。 |
| `SearchEditDecoratedBoxTheme.axaml` | 搜索按钮与输入壳体的一体化边框和布局。 |
| `TextAreaTheme.axaml` | 多行输入壳体、字数统计、resize handle、固定行数和状态视觉。 |
| `TextAreaDecoratedBoxTheme.axaml` | TextArea 内部 padding、右侧附加内容和 resize 相关布局。 |
| `InputClearIconButtonTheme.axaml` / `RevealButtonTheme.axaml` | 内部 action 按钮视觉。 |

`TextBoxToken` 提供基础 TextBox 边框、padding、hover/focus 和 shadow Token。`LineEditToken` 提供单行输入字号 Token。`TextAreaToken` 提供多行输入字号、右侧附加 padding 和 resize handle Token。LineEdit / TextArea 输入壳体的背景、状态、focus shadow 和 disabled 语义主要来自 SharedToken 与 AddOnDecoratedBoxToken。

## 6. 控件家族或集成关系

LineEdit 是 Data Entry 文本输入家族的根入口，与 NumericUpDown、DatePicker、TimePicker、Select、TreeSelect、ComboBox 等控件共享 `SizeType`、`StyleVariant`、`Status`、Addon、Form 和 CompactSpace 语义。

集成关系：

- Avalonia `TextBox`：提供文本编辑、选择、滚动、密码和基础输入事件。
- `AddOnDecoratedBox`：提供输入壳体、外部 AddOn、内部前后缀、variant/status/focus/CompactSpace 视觉。
- `SearchEditDecoratedBox`：在 AddOnDecoratedBox 基础上加入搜索按钮布局和点击回调。
- `TextAreaDecoratedBox`：在 AddOnDecoratedBox 基础上加入多行输入 padding、scroll viewer 接入和 resize 约束。
- `IFormItemAware` / `IFormItemFeedbackAware`：提供表单值、Form 扩展状态和 feedback 内容接入；error 由 `DataValidationErrors` 投射。
- `ICompactSpaceAware`：提供 CompactSpace 中圆角和边框折叠协同。

## 7. 兼容性不变量

维护 LineEdit 家族时必须保持以下不变量：

- 不改变 Avalonia `TextBox` 的 `Text`、选择、光标、密码、滚动和只读语义。
- 不擅自新增、删除、重命名或改变 public API、template part、Token 名称或主题 key。
- `SizeType=Custom` 以 `Middle` 作为未显式覆盖时的默认视觉基线。
- `IsCustomFontSize=true` 时不得由 SizeType 样式覆盖用户设置的 `FontSize`。
- 清除按钮只在有效状态为 true 时显示，且清除动作进入统一 `Clear()` 语义。
- `LineEdit` 的外部 AddOn 必须由 `AddOnDecoratedBox` 承载，不在控件 C# 中动态创建视觉结构。
- `SearchEdit.IsOperating=true` 时搜索按钮点击不重复触发 `SearchButtonClick`。
- `TextArea.Lines` 必须遵守 `MinLines` / `MaxLines`，resize 不得突破行数边界。
- Form feedback 订阅必须在 detach 时释放。
- TextPresenter 的 margin、placeholder、selection、caret 和 disabled 文本色属于输入模板契约，不应在业务控件中用 magic width 补偿。

## 8. 专项模型

### 8.1 Custom SizeType 模型

`CustomizableSizeType.Custom` 表示用户接管尺寸属性。主题层仍把 `Custom` 归入 `Middle` 的默认 line height、corner radius 和 font size 分支；用户显式设置 `Height`、`FontSize`、`Padding` 等属性时，由 Avalonia 属性优先级和 `IsCustomFontSize` 控制最终效果。

### 8.2 SearchEdit 模型

SearchEdit 将搜索按钮视为输入框的一部分。搜索按钮继承输入壳体的 `StyleVariant`、effective status、`SizeType` 和 `IsEnabled` 状态；native validation error 优先投射到按钮状态色，之后才回退到手动 `Status`。`SearchButtonStyle` 只控制按钮风格，不改变文本编辑行为。

### 8.3 TextArea 高度模型

TextArea 支持固定行数、自动高度和拖拽 resize。固定行数模式下，控件根据字体、line height 和文本 presenter/scroll viewer 的垂直间距计算高度。resize 模式记录拖拽开始时的高度，并把高度限制在 `MinLines` / `MaxLines` 对应的范围内。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [LineEdit 桌面版实现原理](implementation.md)
- [LineEdit Token 设计](token.md)
- [LineEdit Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `LineEdit` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + `implementation.md` + `token.md` + Gallery ShowCase | 生成 `controls/line-edit/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/line-edit/semantic-cn.md` |
| API 表 | overview.md 语义摘要 + 源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | token.md、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 层次 | 验证内容 |
| --- | --- |
| 文档 | `overview.md`、`implementation.md`、`token.md`、`changelog.md` 链接有效，Data Entry 分类入口包含 LineEdit。 |
| C# 状态 | 清除按钮、字数统计、Form value、Form feedback、CompactSpace、SearchEdit loading 和 TextArea resize。 |
| AXAML/Theme | template part、variant、status、focus、disabled、SizeType、Custom size、AddOn、TextArea resize handle。 |
| Token | `LineEditToken`、`TextAreaToken`、生成的 TokenKind、Token 类型、生成数据和主题引用一致。 |
| Gallery | 走查基础用法、尺寸、variant、AddOn、清除、密码、前后缀、状态、SearchEdit、TextArea、自动高度、字数统计和 resize 示例。 |
