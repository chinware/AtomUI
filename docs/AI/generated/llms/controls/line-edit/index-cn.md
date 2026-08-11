# LineEdit

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

LineEdit 是 AtomUI 桌面数据录入体系中的文本输入控件家族，用于承载单行文本、密码输入、搜索输入和多行文本输入。它以 Avalonia `TextBox` 文本编辑能力为基础，接入 AtomUI 的尺寸、输入表面、native validation error 投射、清除按钮、密码 reveal、字数统计、Addon、CompactSpace、Form 和 Token 体系。

LineEdit 家族包含以下稳定入口：

| 类型 | 定位 |
| --- | --- |
| `TextBox` | AtomUI 基础文本框，提供尺寸、清除、密码 reveal、字数统计、Form 和 CompactSpace 能力。 |
| `LineEdit` | 标准单行输入框，在 `TextBox` 基础上加入输入表面、验证视觉投射和外部 AddOn。 |
| `SearchEdit` | 搜索输入框，在 `LineEdit` 基础上加入搜索按钮、搜索按钮样式、加载态和搜索事件；独立契约见 [SearchEdit 桌面版架构设计](../search-edit/overview.md)。 |
| `TextArea` | 多行输入框，独立继承 Avalonia `TextBox`，复用输入状态、尺寸、清除、字数统计、Form 和 TextArea 专属 resize 模型。 |

LineEdit 家族只负责文本输入与文本相关辅助操作，不负责结构化选择、日期时间选择、数值步进、异步候选数据管理、富文本编辑或表达式计算。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit` |
| 状态 | Stable |

## 何时使用

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

## 公共 API

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
| `IsOperating` | 搜索进行中状态；显示按钮 loading 并阻止重复搜索请求。 |
| `IsSearchOnEnterEnabled` | 是否允许 Enter 键触发搜索请求，默认启用。 |
| `SearchRequested` | 按钮或 Enter 键触发的搜索请求路由事件。 |

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

## 事件与命令

| `SearchRequested` | 按钮或 Enter 键触发的搜索请求路由事件。 |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

- `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditBasicShowCase.axaml`
- `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditSearchShowCase.axaml`
- `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml`
- `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditStateShowCase.axaml`
- `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditTextAreaShowCase.axaml`

## 状态模型

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

## 主题与 Design Token

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

Token 来源：

LineEdit 输入家族使用三个控件级 Token scope：

| Token | Scope | 职责 |
| --- | --- | --- |
| `TextBoxToken` | `TextBox` | 基础文本框边框、圆角、尺寸 padding、hover/focus 边框和 focus shadow。 |
| `LineEditToken` | `LineEdit` | 单行输入框字号。 |
| `TextAreaToken` | `TextArea` | 多行输入框字号、右侧附加 padding 和 resize handle 视觉。 |

TextBox / LineEdit / TextArea Token 不承载文本值、placeholder、清除状态、密码 reveal、Form 状态、SearchEdit 运行状态、focus/hover/pressed 状态或 CompactSpace 运行时状态。这些状态分别由控件实例属性、共享输入主题、AddOnDecoratedBox、Form 和 C# 状态模型处理。

## AOT 与裁剪注意事项

LineEdit 家族不依赖运行时反射发现模板结构。跨模板协作通过稳定 part、`TemplateBinding`、`BindUtils.RelayBind`、接口和 owner 引用完成。

资源和生命周期边界：

- `_contentRightAddOnBindings` 必须在重新套用模板前 dispose。
- `_feedbackStatusSubscription` 必须在 `FormFeedback` 变化和 detach 时释放。
- clear button click 订阅必须在新模板接入前解绑旧按钮。
- 文本 viewport source 的 `Viewport`、`Padding` 和 presenter `Margin` 订阅必须由 TextBox/TextArea 持有，并在模板重套用时成组替换。
- TextArea resize 不创建全局订阅；拖拽状态保存在控件实例字段中。
- Token 只表达尺寸、字体、padding 和 resize 视觉语义，不承载文本值、清除状态、Form 状态或搜索运行状态。

AOT 边界：

- Token 类型通过 generator 显式注册。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护，不依赖运行时反射扫描。
- 文档中描述的 template part 名称应与 AXAML 和 C# 查找代码保持一致。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Input/TextBox.cs`：AtomUI 基础 TextBox，提供 `SizeType`、清除按钮、密码 reveal、字数统计、Form 和 CompactSpace。
- `src/AtomUI.Desktop.Controls/Input/LineEdit.cs`：标准单行输入框，提供 `StyleVariant`、`Status`、外部 AddOn、内部右侧内容绑定和 Form 扩展状态映射。
- `src/AtomUI.Desktop.Controls/Input/SearchEdit.cs`：搜索输入框，提供搜索按钮样式、搜索按钮文本、加载态和搜索点击事件。
- `src/AtomUI.Desktop.Controls/Input/TextArea.cs`：多行输入框，提供固定行数、自动高度、resize、清除、字数统计、Form 和 feedback。
- `src/AtomUI.Desktop.Controls/Input/InputTextPresenter.cs`：输入文本 presenter，处理 Avalonia 12 selection foreground 缓存刷新。
- `src/AtomUI.Desktop.Controls/Input/SearchEditDecoratedBox.cs`：SearchEdit 输入壳体与搜索按钮协作。
- `src/AtomUI.Desktop.Controls/Input/TextAreaDecoratedBox.cs`：TextArea 输入壳体、scroll viewer 和 resize 相关协作。
- `src/AtomUI.Desktop.Controls/Input/TextViewportMetrics.cs`：输入控件向同程序集消费方发布有效文本 viewport 宽度的内部度量契约。
- `src/AtomUI.Desktop.Controls/Input/ResizeHandle.cs`：TextArea resize 拖拽入口。
- `src/AtomUI.Desktop.Controls/Input/TextBoxToken.cs`：基础 TextBox 边框、padding、hover/focus 和 shadow Token。
- `src/AtomUI.Desktop.Controls/Input/LineEditToken.cs`：单行输入字号 Token。
- `src/AtomUI.Desktop.Controls/Input/TextAreaToken.cs`：TextArea 字号、右侧 padding 和 resize Token。
- `src/AtomUI.Desktop.Controls/Input/Themes/*.axaml`：TextBox、LineEdit、SearchEdit、TextArea 和内部按钮主题。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/line-edit/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/line-edit/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/line-edit/token.md`
- 变更记录：`docs/controls/desktop/data-entry/line-edit/changelog.md`
- 语义结构：`./semantic-cn.md`
