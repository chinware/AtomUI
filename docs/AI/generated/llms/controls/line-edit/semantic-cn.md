# LineEdit 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `LineEdit` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Input/Themes/LineEditTheme.axaml`

```xml
<AddOnDecoratedBox Name="{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}">
    <ScrollViewer Name="PART_ScrollViewer">
        <Panel>
            <TextBlock Name="Placeholder" />
            <InputTextPresenter Name="PART_TextPresenter" />
        </Panel>
    </ScrollViewer>
</AddOnDecoratedBox>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
LineEdit
  -> EmbeddedTextBox (control theme, EmbeddedTextBoxTheme.axaml)
  -> InputClearIconButton (control theme, InputClearIconButtonTheme.axaml)
  -> LineEdit (control theme, LineEditTheme.axaml)
     -> AddOnDecoratedBox#{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart} (template-stable)
        -> ScrollViewer#PART_ScrollViewer (template-stable)
           -> Panel (template-stable)
              -> TextBlock#Placeholder (template-stable)
              -> InputTextPresenter#PART_TextPresenter (template-stable)
  -> ResizeHandle (control theme, ResizeHandleTheme.axaml)
     -> Border#Frame (template-stable)
  -> RevealButton (control theme, RevealButtonTheme.axaml)
  -> SearchEditDecoratedBox (control theme, SearchEditDecoratedBoxTheme.axaml)
     -> SearchEditPanel#RootLayout (internal-observable)
        -> PixelAlignedBorder#{x:Static atom:AddOnDecoratedBoxThemeConstants.LeftAddOnPart} (template-stable)
           -> AddOnContentPresenter#PART_LeftAddOnPresenter (template-stable)
        -> Button#{x:Static atom:AddOnDecoratedBoxThemeConstants.RightAddOnPart} (template-stable)
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
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `LineEdit` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `EmbeddedTextBox` | control theme | `EmbeddedTextBoxTheme.axaml` | LineEdit | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `InputClearIconButton` | control theme | `InputClearIconButtonTheme.axaml` | LineEdit | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `LineEdit` | control theme | `LineEditTheme.axaml` | 用户代码 / 控件宿主 | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `ClipToBounds`, `CompactSpaceItemPosition`, `CompactSpaceOrientation` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}` | template node (AddOnDecoratedBox) | `LineEditTheme.axaml` | LineEdit | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `ClipToBounds`, `CompactSpaceItemPosition`, `CompactSpaceOrientation` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScrollViewer` | template node (ScrollViewer) | `LineEditTheme.axaml` | LineEdit | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `HorizontalContentAlignment`, `IsPlaceholderTextVisible`, `LineHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `LineEditTheme.axaml` | LineEdit | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `HorizontalContentAlignment`, `IsPlaceholderTextVisible`, `LineHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Placeholder` | template node (TextBlock) | `LineEditTheme.axaml` | LineEdit | `HorizontalContentAlignment`, `IsPlaceholderTextVisible`, `LineHeight`, `PlaceholderForeground`, `PlaceholderText`, `TextAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TextPresenter` | template node (InputTextPresenter) | `LineEditTheme.axaml` | LineEdit | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `HorizontalContentAlignment`, `LineHeight`, `PasswordChar` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ResizeHandle` | control theme | `ResizeHandleTheme.axaml` | LineEdit | `Background` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (Border) | `ResizeHandleTheme.axaml` | ResizeHandle | `Background` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RevealButton` | control theme | `RevealButtonTheme.axaml` | LineEdit | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SearchEditDecoratedBox` | control theme | `SearchEditDecoratedBoxTheme.axaml` | LineEdit | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (SearchEditPanel) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.LeftAddOnPart}` | template node (PixelAlignedBorder) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `LeftAddOn`, `LeftAddOnBorderThickness`, `LeftAddOnCornerRadius`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LeftAddOnPresenter` | template node (AddOnContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `LeftAddOn`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.RightAddOnPart}` | template node (Button) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `IsEnabled`, `IsSearchButtonLoading`, `SearchButtonText`, `SearchButtonTheme`, `SizeType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart}` | template node (AddOnDecoratedBoxContentFrame) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentLayout` | template node (DockPanel) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart}` | template node (AddOnContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart}` | template node (AddOnContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `ContentRightAddOn`, `ContentRightAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TextAreaDecoratedBox` | control theme | `TextAreaDecoratedBoxTheme.axaml` | LineEdit | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart}` | template node (AddOnDecoratedBoxContentFrame) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TextAreaContentFrame` | template node (Border) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentLayout` | template node (DockPanel) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart}` | template node (AddOnContentPresenter) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Content`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `ContentTemplate`, `HorizontalScrollBarVisibility`, `IsScrollChainingEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScrollViewer` | template node (ScrollViewer) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Content`, `ContentTemplate`, `HorizontalScrollBarVisibility`, `IsScrollChainingEnabled`, `ScrollerPadding`, `VerticalScrollBarVisibility` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentPresenterPart}` | template node (ContentPresenter) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart}` | template node (AddOnContentPresenter) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `ContentRightAddOn`, `ContentRightAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

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

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

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

## Theme and Token Boundaries

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

Token 边界：

LineEdit 输入家族使用三个控件级 Token scope：

| Token | Scope | 职责 |
| --- | --- | --- |
| `TextBoxToken` | `TextBox` | 基础文本框边框、圆角、尺寸 padding、hover/focus 边框和 focus shadow。 |
| `LineEditToken` | `LineEdit` | 单行输入框字号。 |
| `TextAreaToken` | `TextArea` | 多行输入框字号、右侧附加 padding 和 resize handle 视觉。 |

TextBox / LineEdit / TextArea Token 不承载文本值、placeholder、清除状态、密码 reveal、Form 状态、SearchEdit 运行状态、focus/hover/pressed 状态或 CompactSpace 运行时状态。这些状态分别由控件实例属性、共享输入主题、AddOnDecoratedBox、Form 和 C# 状态模型处理。

## Customization Boundaries

维护 LineEdit 家族时必须保持以下不变量：

- 不改变 Avalonia `TextBox` 的 `Text`、选择、光标、密码、滚动和只读语义。
- 不擅自新增、删除、重命名或改变 public API、template part、Token 名称或主题 key。
- `SizeType=Custom` 以 `Middle` 作为未显式覆盖时的默认视觉基线。
- `IsCustomFontSize=true` 时不得由 SizeType 样式覆盖用户设置的 `FontSize`。
- 清除按钮只在有效状态为 true 时显示，且清除动作进入统一 `Clear()` 语义。
- `LineEdit` 的外部 AddOn 必须由 `AddOnDecoratedBox` 承载，不在控件 C# 中动态创建视觉结构。
- `SearchEdit.IsOperating=true` 时按钮和 Enter 键不重复触发 `SearchRequested`。
- `TextArea.Lines` 必须遵守 `MinLines` / `MaxLines`，resize 不得突破行数边界。
- Form feedback 订阅必须在 detach 时释放。
- TextPresenter 的 margin、placeholder、selection、caret 和 disabled 文本色属于输入模板契约，不应在业务控件中用 magic width 补偿。

维护不变量：

内部重构必须保持以下不变量：

- `TextChanged` 继续驱动字数统计和 Form value changed。
- 清除按钮可见性不在 AXAML 与 C# 中形成相互冲突的状态源。
- `IsCustomFontSize=true` 不能被 SizeType 字体样式覆盖。
- `LineEdit` 的 error 视觉必须优先响应 `DataValidationErrors`；`Status` 只作为无 native error 时的手动视觉请求，并继续支持 warning 扩展视觉。
- `SearchEdit.IsOperating=true` 必须阻止按钮和 Enter 键产生重复搜索请求。
- `TextArea` 的 fixed lines、auto-size 和 resize 不互相覆盖高度状态。
- 重新套用模板不能泄漏旧按钮 click、旧 binding 或旧 Form feedback 订阅。
- TextPresenter margin 是输入模板视觉契约；文本有效宽度由输入控件在模板所有权边界内统一计算并发布，不在业务控件或消费 behavior 中加入隐藏补偿。
- 输入控件不得向内部消费方暴露 `TextPresenter` / `ScrollViewer` 实例；模板结构变化只能影响输入控件自己的度量实现。
