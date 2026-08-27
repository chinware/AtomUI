# LineEdit 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

`LineEdit` 公开 `root`、`prefix`、`input`、`suffix`、`clear`、`count` 六个职责区域。契约只属于 public owner
`LineEdit`；`SearchEdit`、`TextArea`、`TextBox` 以及 internal `AddOnDecoratedBox` 本轮不注册独立 descriptor。
它们即使复用相邻模板结构，也不能通过继承关系自动获得 LineEdit 的 owner-scoped Semantic Style。

### 1.1 `root`

| 字段 | 值 |
| --- | --- |
| Owner | `LineEdit` |
| Part | `root` |
| Selector | LineEdit 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `LineEdit` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | LineEdit owner |
| 职责 | 承载文本值、输入状态、尺寸、variant、验证状态和 owner-scoped Semantic Style 入口。 |
| 相关 API | `Text`、`SizeType`、`StyleVariant`、`Status`、`IsEnabled`、`IsReadOnly`、`IsAllowClear`、`IsShowCount`、`Background`、`BorderBrush` |
| 相关 Token | SharedToken、`LineEditToken` |
| 稳定性 | stable since 6.0 |

`root` 是控件自身，不声明 `.semantic-root` marker。它适合定制 LineEdit 整体 `Background`、`BorderBrush`、
`BorderThickness`、`Opacity`、对齐和尺寸约束；其中 `Background` / `BorderBrush` 由 `AbstractTextInput` 以 LocalValue
中继到 `InputControlFrame` 生效——定制期间该属性槽的 hover / pressed / focus 变色冻结，focus 的 `BoxShadow` 光晕不受
影响，置空后恢复 frame 状态机。variant、effective status、CompactSpace 与 native validation 的状态归一仍由
`AbstractTextInput` 和 `InputControlFrame` 负责。

### 1.2 `prefix`

| 字段 | 值 |
| --- | --- |
| Owner | `LineEdit` |
| Part | `prefix` |
| Selector | `.semantic-prefix` |
| SelectorRoute | `/template/ .semantic-scope-input-frame /template/ .semantic-scope-prefix > .semantic-prefix` |
| Style Type | `LineEditPrefixStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | internal `AddOnContentPresenter`（最低 public 类型为 `ContentPresenter`） |
| 职责 | 承载 `InnerLeftContent` 与 `InnerLeftContentTemplate` 的最终呈现。 |
| 相关 API | `InnerLeftContent`、`InnerLeftContentTemplate` |
| 相关 Token | `SpacingXXS`、输入尺寸 padding |
| 稳定性 | stable since 6.0 |

`prefix` 是 LineEdit 模板中的稳定 presenter。internal `AddOnContentPresenter` 保留 template-only 场景的 child 创建与可见性
语义，同时通过 public 基类 `ContentPresenter` 约束 Setter。`InnerLeftContent=null` 且 template 也为 null 时 presenter 仍属于
静态模板结构；模板内容变化不改变 Part 身份。适合定制 `Opacity`、`Margin`、`Padding`、对齐和 presenter 级排版属性。
`InnerLeftContentTemplate` 创建的用户子树不属于 LineEdit Semantic Part。

### 1.3 `input`

| 字段 | 值 |
| --- | --- |
| Owner | `LineEdit` |
| Part | `input` |
| Selector | `.semantic-input` |
| SelectorRoute | `/template/ .semantic-input` |
| Style Type | `LineEditInputStyle` |
| ContractType | `TextPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `InputTextPresenter#PART_TextPresenter` |
| 职责 | 绘制当前文本、光标、选择范围和密码 reveal 结果。 |
| 相关 API | `Text`、`CaretIndex`、`SelectionStart`、`SelectionEnd`、`PasswordChar`、`RevealPassword` |
| 相关 Token | `FontSize`、`LineHeight`、选择色与 caret 资源 |
| 稳定性 | stable since 6.0 |

`input` 的最低 public `ContractType` 是 Avalonia `TextPresenter`，而不是 internal 实现细节。它适合定制 `Opacity`、
`FontSize`、`FontWeight`、`FontStyle`、`TextAlignment` 和局部 Margin。文本 viewport、选择布局、caret 与密码显示仍属于文本输入
内核，不通过 Semantic Style 改写其 ownership。

### 1.4 `suffix`

| 字段 | 值 |
| --- | --- |
| Owner | `LineEdit` |
| Part | `suffix` |
| Selector | `.semantic-suffix` |
| SelectorRoute | `/template/ .semantic-scope-input-frame /template/ .semantic-scope-suffix > .semantic-suffix` |
| Style Type | `LineEditSuffixStyle` |
| ContractType | `StackPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 内部后缀布局 `StackPanel` |
| 职责 | 组织 clear、reveal、Form feedback、内部右侧内容和 count 的横向布局。 |
| 相关 API | `InnerRightContent`、`InnerRightContentTemplate`、`IsAllowClear`、`IsEnableRevealButton`、`IsShowCount` |
| 相关 Token | `UniformlyPaddingXXS`、输入尺寸 padding |
| 稳定性 | stable since 6.0 |

`suffix` 是稳定的布局区域，不等于用户 `InnerRightContent` 本身。适合定制 `Spacing`、`Opacity`、`Margin`、对齐和布局方向；
clear 与 count 仍拥有各自更窄的 Part。reveal、Form feedback 和用户右侧内容没有独立 Semantic Part，其内部结构也不由
`suffix` 契约继续展开。

### 1.5 `clear`

| 字段 | 值 |
| --- | --- |
| Owner | `LineEdit` |
| Part | `clear` |
| Selector | `.semantic-clear` |
| SelectorRoute | `/template/ .semantic-scope-input-frame /template/ .semantic-scope-suffix > .semantic-suffix > .semantic-clear` |
| Style Type | `LineEditClearStyle` |
| ContractType | `Avalonia.Controls.Button` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `InputClearIconButton#PART_ClearButton` |
| 职责 | 提供清除当前文本的操作入口。 |
| 相关 API | `IsAllowClear`、`ClearIcon`、`IsReadOnly`、`Text` |
| 相关 Token | clear 按钮主题与 SharedToken |
| 稳定性 | stable since 6.0 |

`clear` 节点始终存在，`IsEffectiveShowClearButton` 只切换可见性。它适合定制 `Opacity`、`Margin`、`Padding`、`Cursor` 和
Button 级交互属性；清除命令仍必须进入 `NotifyClearButtonClicked()` / `Clear()` 的统一行为，不通过样式替换文本状态源。

### 1.6 `count`

| 字段 | 值 |
| --- | --- |
| Owner | `LineEdit` |
| Part | `count` |
| Selector | `.semantic-count` |
| SelectorRoute | `/template/ .semantic-scope-input-frame /template/ .semantic-scope-suffix > .semantic-suffix > .semantic-count` |
| Style Type | `LineEditCountStyle` |
| ContractType | `TextBlock` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `TextBlock#TextCountIndicator` |
| 职责 | 展示当前文本长度与 `MaxLength` 的计数文案。 |
| 相关 API | `IsShowCount`、`Text`、`MaxLength` |
| 相关 Token | `ColorTextPlaceholder`、字体与行高资源 |
| 稳定性 | stable since 6.0 |

`count` 节点始终存在，`IsShowCount=false` 只切换可见性。它适合定制 `Foreground`、`FontSize`、`FontWeight`、`Opacity`、
`Margin` 和对齐；计数格式和刷新时机由 `AbstractTextInput` 维护，不属于 Semantic Style。

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
     -> AddOnDecoratedBox#{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart} (internal-observable)
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
        -> AddOnDecoratedBoxContentFrame#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart} (internal-observable)
           -> DockPanel#ContentLayout (template-stable)
              -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart} (internal-observable)
              -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart} (internal-observable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
  -> TextAreaDecoratedBox (control theme, TextAreaDecoratedBoxTheme.axaml)
     -> AddOnDecoratedBoxContentFrame#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart} (internal-observable)
        -> Border#TextAreaContentFrame (template-stable)
           -> DockPanel#ContentLayout (template-stable)
              -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart} (internal-observable)
              -> Panel (template-stable)
                 -> ScrollViewer#PART_ScrollViewer (template-stable)
                    -> ContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentPresenterPart} (internal-observable)
                 -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart} (internal-observable)
  -> AddOnDecoratedBox (control theme, AddOnDecoratedBoxTheme.axaml)
     -> DockPanel#RootLayout (template-stable)
        -> PixelAlignedBorder#{x:Static atom:AddOnDecoratedBoxThemeConstants.LeftAddOnPart} (template-stable)
           -> AddOnContentPresenter#PART_LeftAddOnPresenter (template-stable)
        -> PixelAlignedBorder#{x:Static atom:AddOnDecoratedBoxThemeConstants.RightAddOnPart} (template-stable)
           -> AddOnContentPresenter#PART_RightAddOnPresenter (template-stable)
        -> AddOnDecoratedBoxContentFrame#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart} (internal-observable)
           -> AdaptiveSpacingDockPanel#ContentLayout (template-stable)
              -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart} (internal-observable)
              -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart} (internal-observable)
              -> ContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentPresenterPart} (internal-observable)
  -> InputControlFrame (control theme, InputControlFrameTheme.axaml)
     -> PixelAlignedBorder#InnerBoxDecorator (template-stable)
        -> ContentPresenter#PART_ContentPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `LineEdit` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `EmbeddedTextBox` | control theme | `EmbeddedTextBoxTheme.axaml` | LineEdit | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `InputClearIconButton` | control theme | `InputClearIconButtonTheme.axaml` | LineEdit | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `LineEdit` | control theme | `LineEditTheme.axaml` | 用户代码 / 控件宿主 | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `ClipToBounds`, `CompactSpaceItemPosition`, `CompactSpaceOrientation` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}` | template node (AddOnDecoratedBox) | `LineEditTheme.axaml` | LineEdit | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `ClipToBounds`, `CompactSpaceItemPosition`, `CompactSpaceOrientation` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
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
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart}` | template node (AddOnDecoratedBoxContentFrame) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentLayout` | template node (DockPanel) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart}` | template node (AddOnContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart}` | template node (AddOnContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `ContentRightAddOn`, `ContentRightAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TextAreaDecoratedBox` | control theme | `TextAreaDecoratedBoxTheme.axaml` | LineEdit | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart}` | template node (AddOnDecoratedBoxContentFrame) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TextAreaContentFrame` | template node (Border) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentLayout` | template node (DockPanel) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart}` | template node (AddOnContentPresenter) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Content`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `ContentTemplate`, `HorizontalScrollBarVisibility`, `IsScrollChainingEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScrollViewer` | template node (ScrollViewer) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Content`, `ContentTemplate`, `HorizontalScrollBarVisibility`, `IsScrollChainingEnabled`, `ScrollerPadding`, `VerticalScrollBarVisibility` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentPresenterPart}` | template node (ContentPresenter) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart}` | template node (AddOnContentPresenter) | `TextAreaDecoratedBoxTheme.axaml` | TextAreaDecoratedBox | `ContentRightAddOn`, `ContentRightAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `AddOnDecoratedBox` | control theme | `AddOnDecoratedBoxTheme.axaml` | LineEdit | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (DockPanel) | `AddOnDecoratedBoxTheme.axaml` | AddOnDecoratedBox | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.LeftAddOnPart}` | template node (PixelAlignedBorder) | `AddOnDecoratedBoxTheme.axaml` | AddOnDecoratedBox | `LeftAddOn`, `LeftAddOnBorderThickness`, `LeftAddOnCornerRadius`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LeftAddOnPresenter` | template node (AddOnContentPresenter) | `AddOnDecoratedBoxTheme.axaml` | AddOnDecoratedBox | `LeftAddOn`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.RightAddOnPart}` | template node (PixelAlignedBorder) | `AddOnDecoratedBoxTheme.axaml` | AddOnDecoratedBox | `RightAddOn`, `RightAddOnBorderThickness`, `RightAddOnCornerRadius`, `RightAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 所属主题 | 职责 |
| --- | --- | --- | --- |
| `PART_InputControlFrame` | `InputControlFrame` / `AddOnDecoratedBox` 派生类型 | 全家族输入主题 | 输入表面、边框、背景、状态视觉、CompactSpace 和动效承载；派生 decorated box 只增加 AddOn 或专用布局。 |
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

| `InputControlFrame` | internal 输入表面组合控件，统一 variant、effective status、边框、背景、圆角、阴影、交互伪类、CompactSpace 和动效；不作为公共控件入口。 |

继承与组合关系固定为：

```text
Avalonia.Controls.TextBox
        ↓
AbstractTextInput
   ├── TextBox
   ├── LineEdit
   └── TextArea

## State Flow

LineEdit 家族的交互优先级：

```text
Disabled
> Native Error
> Form Error
> Form Warning
> Explicit Warning
> Explicit Error
> Focus
> PointerOver
> Normal
```

输入状态由三个来源组成：

| 来源 | owner | 语义 |
| --- | --- | --- |
| `NativeValidationStatus` | `DataValidationErrors` | Avalonia 原生校验结果；`Error` 是 native error 的唯一视觉真源。 |
| `FormStatus` | Form 集成层 | Form 写入的 warning、success、validating 及其 feedback；Form error 通过 Form-owned native validation entry 写入 `DataValidationErrors`。 |
| `ExplicitStatus` | `AbstractTextInput.Status` | 用户显式设置的 error/warning 请求，不覆盖 native error，也不修改 Form 自己写入的状态。 |

有效状态由 `InputControlFrame` 计算：

```text
Native Error / Form Error
    > Form Warning
    > Explicit Warning
    > Explicit Error
    > Default
```

Form reset 只清理 Form 自己写入的状态；`Status=Error` 不写入 native error；`Warning` 伪类只在最终有效状态为 Warning 时出现。

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
| `AbstractTextInput` shared template contract | 统一文本输入属性、placeholder、clear/reveal/count、Form feedback、native validation、CompactSpace 和 viewport metrics 接入。 |
| `InputControlFrameTheme.axaml` | Outlined、Filled、Borderless、Underlined、hover、focus-within、pressed、disabled、error、warning、corner、shadow、CompactSpace 和 motion。 |
| `TextBoxTheme.axaml` | 基础文本编辑模板、文本 presenter、TextBox 专属 padding、clear/reveal、字数统计和 frame 组合。 |
| `LineEditTheme.axaml` | 单行文本布局、外部 AddOn、内部前后缀和 `InputControlFrame` 组合。 |
| `SearchEditTheme.axaml` | 搜索输入模板、搜索按钮状态传递和 `SearchEditDecoratedBox` 组合。 |
| `SearchEditDecoratedBoxTheme.axaml` | 搜索按钮与 frame 的一体化布局；不重复实现 frame 状态 selector。 |
| `TextAreaTheme.axaml` | 多行文本、字数统计、固定行数和 `InputControlFrame` 组合。 |
| `TextAreaDecoratedBoxTheme.axaml` | TextArea 内部 padding、右侧附加内容、scroll viewer 和 resize 相关布局。 |
| `InputClearIconButtonTheme.axaml` / `RevealButtonTheme.axaml` | 内部 action 按钮视觉。 |

共享输入表面值由 `SharedToken` 和 `InputControlFrameTheme` 消费。`TextBoxToken`、`LineEditToken`、`TextAreaToken` 只保留各自稳定的文本尺寸、padding 和多行 resize 专属语义，不再承载输入表面边框、背景、focus shadow、error/warning 或 disabled 状态。

Token 边界：

LineEdit 输入家族使用三个控件级 Token scope：

| Token | Scope | 职责 |
| --- | --- | --- |
| `TextBoxToken` | `TextBox` | TextBox 专属内容 padding。 |
| `LineEditToken` | `LineEdit` | 单行输入框字号。 |
| `TextAreaToken` | `TextArea` | 多行输入框字号、右侧附加 padding 和 resize handle 视觉。 |

TextBox / LineEdit / TextArea Token 不承载文本值、placeholder、清除状态、密码 reveal、Form 状态、SearchEdit 运行状态、focus/hover/pressed 状态或 CompactSpace 运行时状态。输入表面边框、背景、圆角、shadow、error/warning、disabled 和 motion 由 `InputControlFrameTheme` 与 `SharedToken` 处理；运行时状态由 `AbstractTextInput`、Form 和 frame 状态模型处理。

## Customization Boundaries

维护 LineEdit 家族时必须保持以下不变量：

- 不改变 Avalonia `TextBox` 的 `Text`、选择、光标、密码、滚动和只读语义。
- 不擅自新增、删除、重命名或改变 public API、template part、Token 名称或主题 key。
- `SizeType=Custom` 以 `Middle` 作为未显式覆盖时的默认视觉基线。
- `IsCustomFontSize=true` 时不得由 SizeType 样式覆盖用户设置的 `FontSize`。
- 清除按钮只在有效状态为 true 时显示，且清除动作进入统一 `Clear()` 语义。
- `InputControlFrame` 是所有输入表面状态 selector、边框、背景、focus shadow、error/warning、disabled 和 motion 的唯一 owner。
- `AddOnDecoratedBox` 及其派生类型只承载 AddOn、内部内容和专用布局，不复制 frame 的状态计算或视觉 selector。
- `StyleVariant`、`Status`、`SizeType`、Form 状态和 native validation 必须先由 `AbstractTextInput` 归一，再通过稳定绑定传给 frame。
- 输入主题不在 owner 层携带 `Background` / `BorderBrush` 默认值，frame 主题是 rest 态取值的唯一来源；`TextBox` 模板不以 `TemplateBinding` 绑定这两个属性。owner 值是“用户是否定制 root 表面”的判定输入，恢复任何 owner 层默认都会使中继判定失效。
- root `Background` / `BorderBrush` 中继语义保持不变：owner 有值 → frame 以 LocalValue 接管（定制期间该属性槽的交互态变色冻结），owner 置空 → frame `ClearValue` 恢复状态机；focus 反馈依赖 `BoxShadow` 独立属性槽，不随边框定制失效。
- `SearchEdit.IsOperating=true` 时按钮和 Enter 键不重复触发 `SearchRequested`。
- `TextArea.Lines` 必须遵守 `MinLines` / `MaxLines`，resize 不得突破行数边界。
- Form feedback 订阅必须在 detach 时释放。
- TextPresenter 的 margin、placeholder、selection、caret 和 disabled 文本色属于输入模板契约，不应在业务控件中用 magic width 补偿。
- LineEdit 的 `root`、`prefix`、`input`、`suffix`、`clear`、`count` Part 名称、route、最低 public `ContractType` 与 `Single` 数量语义必须保持稳定。
- `SearchEdit`、`TextArea`、`TextBox` 不因继承或模板复用自动获得 LineEdit descriptor；扩展其契约必须单独评审 public owner 边界。

维护不变量：

内部重构必须保持以下不变量：

- `TextChanged` 继续驱动字数统计和 Form value changed。
- 清除按钮可见性不在 AXAML 与 C# 中形成相互冲突的状态源。
- TextBox 外框宽度由父布局和 `Width` / `MinWidth` / `MaxWidth` 契约决定，不能随 placeholder 或当前文本的测量宽度伸缩。
- `IsCustomFontSize=true` 不能被 SizeType 字体样式覆盖。
- `InputControlFrame.EffectiveStatus` 必须遵守 Native Error / Form Error > Form Warning > Explicit Warning > Explicit Error > Default；native error 由 `DataValidationErrors` 唯一提供，Form reset 不得清理外部 native error。
- 所有 `*AddOnDecoratedBox` 只能扩展 frame 布局，不能重新定义 variant/status/error/warning/disabled/motion selector。
- `SearchEdit.IsOperating=true` 必须阻止按钮和 Enter 键产生重复搜索请求。
- `TextArea` 的 fixed lines、auto-size 和 resize 不互相覆盖高度状态。
- 重新套用模板不能泄漏旧按钮 click、旧模板 binding、旧 preedit 或旧 viewport source；logical reattach 后当前模板交互与状态绑定必须保持有效，Form feedback 必须重新订阅。
- TextPresenter margin 是输入模板视觉契约；文本有效宽度由输入控件在模板所有权边界内统一计算并发布，不在业务控件或消费 behavior 中加入隐藏补偿。
- 输入控件不得向内部消费方暴露 `TextPresenter` / `ScrollViewer` 实例；模板结构变化只能影响输入控件自己的度量实现。
- `.semantic-scope-input-frame`、`.semantic-scope-prefix`、`.semantic-scope-suffix` 只能服务 LineEdit 的显式 route，不得提升为公开 Part 或默认主题 selector。
- clear、count、prefix、suffix 的可见性和内容变化不得增删 marker；五个 selector Part 在内置 LineEdit 模板中均保持 `Single`。
- 四个输入主题（LineEdit / TextBox / SearchEdit / TextArea）不得在 owner 层恢复 `Background` / `BorderBrush` 默认值，`TextBox` 模板不得重新绑定这两个属性的 `TemplateBinding`；owner 属性值是中继判定“是否定制”的唯一输入。`EmbeddedTextBox` 这类需要压制 chrome 的内嵌派生在自身 ControlTheme 中显式设置 owner 值，经中继成为 frame LocalValue，不依赖模板绑定。
- root `Background` / `BorderBrush` 中继不得改变共享状态机：未定制路径零 LocalValue 写入，rest / hover / pressed / focus / disabled 视觉与 `LineEditBorderRenderTests` 基线保持一致；定制路径冻结该属性槽的交互态变色，`ClearValue` 后必须完整恢复状态机。
