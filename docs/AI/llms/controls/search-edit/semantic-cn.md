# SearchEdit 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `SearchEdit` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Input/Themes/SearchEditTheme.axaml`

```xml
<SearchEditDecoratedBox Name="{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}">
    <ScrollViewer Name="PART_ScrollViewer">
        <Panel>
            <TextBlock Name="Placeholder" />
            <InputTextPresenter Name="PART_TextPresenter" />
        </Panel>
    </ScrollViewer>
</SearchEditDecoratedBox>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
SearchEdit
  -> EmbeddedTextBox (control theme, EmbeddedTextBoxTheme.axaml)
  -> InputClearIconButton (control theme, InputClearIconButtonTheme.axaml)
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
  -> SearchEdit (control theme, SearchEditTheme.axaml)
     -> SearchEditDecoratedBox#{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart} (internal-observable)
        -> ScrollViewer#PART_ScrollViewer (template-stable)
           -> Panel (template-stable)
              -> TextBlock#Placeholder (template-stable)
              -> InputTextPresenter#PART_TextPresenter (template-stable)
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
| `SearchEdit` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `EmbeddedTextBox` | control theme | `EmbeddedTextBoxTheme.axaml` | SearchEdit | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `InputClearIconButton` | control theme | `InputClearIconButtonTheme.axaml` | SearchEdit | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ResizeHandle` | control theme | `ResizeHandleTheme.axaml` | SearchEdit | `Background` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (Border) | `ResizeHandleTheme.axaml` | ResizeHandle | `Background` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RevealButton` | control theme | `RevealButtonTheme.axaml` | SearchEdit | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SearchEditDecoratedBox` | control theme | `SearchEditDecoratedBoxTheme.axaml` | SearchEdit | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (SearchEditPanel) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.LeftAddOnPart}` | template node (PixelAlignedBorder) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `LeftAddOn`, `LeftAddOnBorderThickness`, `LeftAddOnCornerRadius`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LeftAddOnPresenter` | template node (AddOnContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `LeftAddOn`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.RightAddOnPart}` | template node (Button) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `IsEnabled`, `IsSearchButtonLoading`, `SearchButtonText`, `SearchButtonTheme`, `SizeType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart}` | template node (AddOnDecoratedBoxContentFrame) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentLayout` | template node (DockPanel) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart}` | template node (AddOnContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart}` | template node (AddOnContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `ContentRightAddOn`, `ContentRightAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `SearchEditDecoratedBoxTheme.axaml` | SearchEditDecoratedBox | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SearchEdit` | control theme | `SearchEditTheme.axaml` | 用户代码 / 控件宿主 | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `DataValidationErrors` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}` | template node (SearchEditDecoratedBox) | `SearchEditTheme.axaml` | SearchEdit | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `DataValidationErrors` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ScrollViewer` | template node (ScrollViewer) | `SearchEditTheme.axaml` | SearchEdit | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `HorizontalContentAlignment`, `IsPlaceholderTextVisible`, `LineHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `SearchEditTheme.axaml` | SearchEdit | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `HorizontalContentAlignment`, `IsPlaceholderTextVisible`, `LineHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Placeholder` | template node (TextBlock) | `SearchEditTheme.axaml` | SearchEdit | `HorizontalContentAlignment`, `IsPlaceholderTextVisible`, `LineHeight`, `PlaceholderForeground`, `PlaceholderText`, `TextAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TextPresenter` | template node (InputTextPresenter) | `SearchEditTheme.axaml` | SearchEdit | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `HorizontalContentAlignment`, `LineHeight`, `PasswordChar` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TextAreaDecoratedBox` | control theme | `TextAreaDecoratedBoxTheme.axaml` | SearchEdit | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
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
| `PART_AddOnDecoratedBox` | `SearchEditDecoratedBox` | `SearchEditTheme.axaml` | 搜索输入壳体、状态、Addon、CompactSpace 和搜索按钮协作入口。 |
| `PART_RightAddOn` | `Button` | `SearchEditDecoratedBoxTheme.axaml` | public Button 语义部件，承载图标、文字、loading、按钮样式和点击事件。 |
| `PART_ContentFrame` | `Border` | `SearchEditDecoratedBoxTheme.axaml` | 文本输入框视觉边框和背景。 |
| `PART_ScrollViewer` | `ScrollViewer` | `SearchEditTheme.axaml` | 文本滚动区域。 |
| `PART_TextPresenter` | `InputTextPresenter` | `SearchEditTheme.axaml` | 文本显示、光标、选择和密码 reveal。 |
| `Placeholder` | `TextBlock` | `SearchEditTheme.axaml` | 空文本占位提示。 |
| `PART_ClearButton` | `InputClearIconButton` | `SearchEditTheme.axaml` | 清除当前搜索文本。 |
| `PART_RevealButton` | `RevealButton` | `SearchEditTheme.axaml` | 密码 reveal 兼容入口。 |
| `InnerRightContentPresenter` | `ContentPresenter` | `SearchEditTheme.axaml` | 输入框内部右侧内容承载。 |

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

SearchEdit 的文本输入行为继承 LineEdit，搜索按钮行为独立建模：

```text
Button#PART_RightAddOn.Click
  ↓
SearchEditDecoratedBox.HandleSearchButtonClick
  ↓
SearchEdit.NotifySearchButtonClicked()
  ↓
if !IsOperating raise SearchButtonClick
```

`IsOperating=true` 只表示搜索按钮处于操作中状态。它不改变 `Text`、不自动禁用文本编辑、不管理异步任务，也不清空搜索结果。业务层负责在搜索开始和结束时设置该属性。

状态优先级：

```text
Disabled
> Operating button loading
> Error / Warning
> Focus
> PointerOver / Pressed
> Normal
```

`IsEnabled=false` 会传递给搜索按钮，使输入壳体和按钮一起进入 disabled 视觉。native validation error 通过 `DataValidationErrors` 优先影响输入框边框、文本前景和搜索按钮状态色；`Status=Warning` 继续表达 AtomUI warning 视觉，显式 `Status=Error` 只作为无 native error 时的手动错误视觉请求。`SearchButtonStyle` 只控制按钮强调度，不改变文本编辑、清除、Form 或搜索事件语义。

## Theme and Token Boundaries

SearchEdit 使用三层主题协作：

| 主题 | 职责 |
| --- | --- |
| `SearchEditTheme.axaml` | SearchEdit 根模板、文本 presenter、placeholder、clear/reveal/inner-right 内容、focus/status 视觉。 |
| `SearchEditDecoratedBoxTheme.axaml` | 输入框内容边框、搜索按钮、左右布局、搜索按钮 style 和 z-index 关系。 |
| `SearchButtonTheme.axaml` | 搜索按钮在不同输入表面中的背景、前景和状态色。 |

SearchEdit 拥有独立 `ControlTokenIdentity`，但不定义 Own Token。它继承 `LineEdit` 的行为并不意味着继承或借用
LineEdit identity；Control 级配置中的任意已注册 Global Token 都绑定到 SearchEdit 自己的 Effective Global
Token。合法但没有被当前主题直接或间接消费的 Global Token 可以没有视觉效果。

主题资源边界：

| Token 来源 | 用途 |
| --- | --- |
| `SearchEditTokenResource` | 读取 SearchEdit Effective Global Token，负责输入与搜索按钮组合语义，例如 focus shadow、主色和输入状态背景。 |
| `AddOnDecoratedBoxTokenResource` | 由 `SearchEditDecoratedBoxTheme` 的 BasedOn 主题显式读取输入壳体 Own/Effective Global Token。 |
| `ButtonTokenResource` | 由真实 Button 和 `SearchButtonTheme` 显式读取 Button Own/Effective Global Token，负责按钮基础视觉。 |
| `SharedTokenResource` | 读取真正的 Global Token，只用于不响应 SearchEdit Control 级覆盖的共享值。 |

`SearchButtonTheme` 的 `TargetType` 是 Button，但资产 owner 和组合语义属于 SearchEdit。它可以同时使用
`SearchEditTokenResource` 与 `ButtonTokenResource`；这是显式跨 Control 资源引用，不是 SearchEdit 借用 Button
或 LineEdit identity。

搜索按钮必须与输入框视觉上组成单一控件。Custom 高度下，搜索按钮的可视 `Frame` 高度必须跟随 `SearchEditDecoratedBox` 的实际布局高度，避免按钮边框和输入框边框错位。

Token 边界：

- SearchEdit 当前没有 Own Token，因此没有专属 `token.md`；LLMS 生成按第 5 节说明独立 SearchEdit identity、完整 Effective Global Token、Button Semantic Part 和显式跨 Control 资源边界。

## Customization Boundaries

维护 SearchEdit 时必须保持以下不变量：

- 不改变继承自 LineEdit 的 `Text`、选择、光标、清除、reveal、Form 和 CompactSpace 语义。
- 不删除或重命名 `SearchButtonStyle`、`SearchButtonText`、`IsOperating`、`SearchButtonClick`。
- `IsOperating=true` 必须阻止重复搜索点击，但不得自动管理异步任务或修改 `Text`。
- 搜索按钮的 `IsEnabled`、`SizeType` 和 loading 必须跟随 SearchEdit；按钮组合视觉必须响应输入壳体的 `Status` 和 `StyleVariant`。
- 右侧外部 add-on 位置属于搜索按钮；内部右侧内容必须继续由 `InnerRightContent` 承载。
- SearchEdit 的按钮边框和输入框边框必须在 Large、Middle、Small 和 Custom 高度下严格对齐。
- `SizeType=Custom` 必须以 Middle 作为未显式覆盖时的视觉基线。
- SearchEdit 保持独立 Control identity；当前不新增 Own Token，也不得借用 LineEdit 或 Button identity。
- `SearchButtonTheme` 必须继续以 public Button 为 TargetType，并显式区分 SearchEdit 组合语义与 Button 基础视觉。
- 重新套用模板时必须释放旧搜索按钮 click 订阅。

维护不变量：

内部重构必须保持以下不变量：

- 搜索按钮 click 只能通过 `SearchEdit.NotifySearchButtonClicked()` 抛出 `SearchButtonClick`。
- `IsOperating=true` 必须阻止重复搜索事件，并继续驱动按钮 loading。
- 搜索按钮和内容框的边框必须在同一布局高度下绘制。
- `SearchEditPanel` 的按钮左边框重叠算法不能破坏单线边框视觉。
- 搜索按钮必须接收 SearchEdit 的 `SizeType`、`IsEnabled` 和 loading；`StyleVariant` 与 effective status 的组合视觉由 SearchEdit owner theme 投射。
- 搜索按钮必须保持 public Button 类型；不得重新引入借用 LineEdit 或 Button identity 的 internal SearchButton。
- `SearchButtonTheme` 必须继续作为强类型 Semantic Part Theme，并允许实例级替换。
- 搜索按钮右侧外部 AddOn 位置不可被用户内容替代。
- `InnerRightContent`、clear、reveal 和文本 presenter 的绑定仍由 LineEdit 模板路径维护。
- AutoCompleteSearchEdit 复用 SearchEdit 视觉时不能绕过 SearchEdit 搜索按钮契约。
