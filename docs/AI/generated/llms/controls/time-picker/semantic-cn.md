# TimePicker 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

`TimePicker` 公开 11 个 Semantic Part，`RangeTimePicker` 公开 12 个 Semantic Part（后者多出范围双输入框的
`secondaryInput`）。声明分别位于 `TimePicker.SemanticParts.cs` 与 `RangeTimePicker.SemanticParts.cs` partial 文件。

触发区部件的 marker 分布：

- 单值 `TimePicker` 的宿主模板是共享 `InfoPickerInputTheme.axaml`（`TimePickerTheme.axaml` 纯 `BasedOn` 继承，
  无自有模板）；`RangeTimePicker` 的宿主模板是自有 `RangeTimePickerTheme.axaml` 模板覆写（共享
  `RangeInfoPickerInputTheme.axaml` 保持无 semantic 标注）。两个宿主模板按同一模式标注
  `semantic-scope-input`（AddOnDecoratedBox 节点）、`semantic-input`（`PART_InfoInputBox`）、`semantic-suffix`
  （右侧内容 StackPanel）、`semantic-scope-handle`（PickerClearUpButton 节点）与 `semantic-popup-root`
  （`PART_Popup` 内的 `ArrowDecoratedBox`），Range 模板另标注 `semantic-secondary-input`
  （`PART_SecondaryInfoInputBox`）。
- `prefix` 借用共享 `AddOnDecoratedBoxTheme` 的 `.semantic-scope-prefix` scope 锚点路由到宿主模板
  `AddOnContentPresenter` 投影节点（与 Select、DatePicker 家族同构；投影节点以
  `CompiledBinding $parent[atom:InfoPickerInput].ContentLeftAddOn` 呈现公共 API 值）。
- `clear` 的物理按钮在共享 `PickerClearUpButtonTheme.axaml` 模板内（`PART_ClearButton`），声明
  `CrossNestedOwners=true`，生成器沿 PickerClearUpButton 主题链校验。
- 范围 `RangeTimePicker` 与单值 `TimePicker` 共用同一弹层 presenter（`TimePickerPresenter`），弹层 marker
  对两个 owner 同时成立。

弹层部件的 marker 分布（弹层内容全部由 owner `CreatePickerPresenter` 在首次打开时运行时创建）：

- `popup.container` / `popup.footer` 标注 presenter 主题模板节点：`popup.container` 为
  `TimePickerPresenterTheme.axaml` 的 `DockPanel #PART_MainLayout`，`popup.footer` 为同模板的
  `PixelAlignedBorder #PART_ButtonsFrame`。
- `popup.content` / `popup.column` 标注 `TimeViewTheme.axaml` 模板节点：`popup.content` 为时间列布局容器
  `Grid #PART_PickerContainer`，`popup.column` 为四个列宿主 Panel（`PART_HourHost` / `PART_MinuteHost` /
  `PART_SecondHost` / `PART_PeriodHost`）。
- `popup.item` 为运行时注入：`DateTimePickerPanel.CreateOrDestroyItems` 创建 `TimeViewCell` 时追加生成
  selector class 常量，覆盖滚动复用与循环搬移路径。
- `popup.*` 除 `popup.root` 外统一声明 `RuntimeCreated=true`（presenter 子树在运行时组装，生成器豁免宿主模板
  marker 校验，由控件行为测试兜底），其中 `popup.root` 为宿主模板静态节点、`RuntimeCreated=false`。
- **TimeView 子树类名避让**：`TimeView` 同时被 DatePicker 带时间弹层内嵌（`DatePickerPresenterTheme.axaml` 与
  `TimedRangeDatePickerPresenterTheme.axaml`）。因此落在 TimeView 子树内的三个 marker 类名使用 `time-` 中缀
  （`semantic-time-content` / `semantic-time-column` / `semantic-time-item`），避开 DatePicker 家族已声明的
  `semantic-popup-content` / `semantic-popup-body` / `semantic-cell` 等类名，防止 DatePicker 弹层上下文中的
  契约误命中（详见 §6 对照差异）。

### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `TimePicker` / `RangeTimePicker` |
| Part | `root` |
| Selector | owner 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `TimePicker` / `RangeTimePicker` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | TimePicker / RangeTimePicker owner |
| 职责 | owner 是时间值、约束、弹层状态、Form 值与验证状态的组织边界。 |
| 相关 API | 全部 TimePicker / RangeTimePicker public API |
| 相关 Token | TimePickerToken、SharedToken |
| 稳定性 | stable since 6.0 |

### `prefix`

| 字段 | 值 |
| --- | --- |
| Owner | `TimePicker` / `RangeTimePicker` |
| Part | `prefix` |
| Selector | `.semantic-prefix` |
| SelectorRoute | `/template/ .semantic-scope-input /template/ .semantic-scope-prefix > .semantic-prefix` |
| Style Type | `TimePickerPrefixStyle` / `RangeTimePickerPrefixStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 宿主模板中承载 `ContentLeftAddOn` 的 `AddOnContentPresenter` 投影节点 |
| 职责 | 输入区内容前缀区域，承载 `ContentLeftAddOn` 用户内容，在内容框内联展示。 |
| 相关 API | `ContentLeftAddOn`、`ContentLeftAddOnTemplate` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `input`

| 字段 | 值 |
| --- | --- |
| Owner | `TimePicker` / `RangeTimePicker` |
| Part | `input` |
| Selector | `.semantic-input` |
| SelectorRoute | `/template/ .semantic-input` |
| Style Type | `TimePickerInputStyle` / `RangeTimePickerInputStyle` |
| ContractType | `TextBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 宿主模板的 `InfoPickerTextBox #PART_InfoInputBox`（起始端输入框） |
| 职责 | 时间文本输入框，承载格式化显示值、占位符与只读/校验状态。 |
| 相关 API | `Text`、`PlaceholderText`、`IsReadOnly`、`PreferredInputWidth` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `secondaryInput`（仅 RangeTimePicker）

| 字段 | 值 |
| --- | --- |
| Owner | `RangeTimePicker` |
| Part | `secondaryInput` |
| Selector | `.semantic-secondary-input` |
| SelectorRoute | `/template/ .semantic-secondary-input` |
| Style Type | `RangeTimePickerSecondaryInputStyle` |
| ContractType | `TextBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `RangeTimePickerTheme.axaml` 的 `InfoPickerTextBox #PART_SecondaryInfoInputBox`（结束端输入框） |
| 职责 | 范围选择的结束端时间文本输入框，与 `input` 共用格式与宽度基线。 |
| 相关 API | `SecondaryText`、`SecondaryPlaceholderText` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `suffix`

| 字段 | 值 |
| --- | --- |
| Owner | `TimePicker` / `RangeTimePicker` |
| Part | `suffix` |
| Selector | `.semantic-suffix` |
| SelectorRoute | `/template/ .semantic-scope-input /template/ .semantic-scope-suffix > .semantic-suffix` |
| Style Type | `TimePickerSuffixStyle` / `RangeTimePickerSuffixStyle` |
| ContractType | `StackPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 宿主模板中投影给 `ContentRightAddOn` 的水平 StackPanel（含清除按钮与 `PART_ContentRightAddOnPresenter`） |
| 职责 | 输入区后缀区域，承载清除按钮、Form 反馈与用户后缀内容。 |
| 相关 API | `ContentRightAddOn`、`ContentRightAddOnTemplate` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `clear`

| 字段 | 值 |
| --- | --- |
| Owner | `TimePicker` / `RangeTimePicker` |
| Part | `clear` |
| Selector | `.semantic-clear` |
| SelectorRoute | `>> .semantic-scope-handle /template/ .semantic-clear` |
| Style Type | `TimePickerClearStyle` / `RangeTimePickerClearStyle` |
| ContractType | `IconButton` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossNestedOwners | `true` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 共享 `PickerClearUpButtonTheme.axaml` 模板内的 `InputClearIconButton #PART_ClearButton` |
| 职责 | 后缀区清除按钮，进入清除模式（hover / focus）时渲染。 |
| 相关 API | `ShowClearButtonPredicate`、`Clear` / `Reset` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `popup.root`

| 字段 | 值 |
| --- | --- |
| Owner | `TimePicker` / `RangeTimePicker` |
| Part | `popup.root` |
| Selector | `.semantic-popup-root` |
| SelectorRoute | `/template/ .semantic-popup-root` |
| Style Type | `TimePickerPopupRootStyle` / `RangeTimePickerPopupRootStyle` |
| ContractType | `ArrowDecoratedBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 单选：`InfoPickerInputTheme.axaml` 中 `PART_Popup` 的 `ArrowDecoratedBox`；范围：`RangeTimePickerTheme.axaml` 中的 `ArrowDecoratedBox` |
| 职责 | 弹层内容根视觉盒子，承载背景、边框、阴影与浮动箭头。 |
| 相关 API | `IsArrowVisible`（经 `IsArrowVisibleEffective`）、`ArrowPosition`、`IsMotionEnabled` |
| 相关 Token | PopupToken |
| 稳定性 | stable since 6.0 |

### `popup.container`

| 字段 | 值 |
| --- | --- |
| Owner | `TimePicker` / `RangeTimePicker` |
| Part | `popup.container` |
| Selector | `.semantic-popup-container` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-container` |
| Style Type | `TimePickerPopupContainerStyle` / `RangeTimePickerPopupContainerStyle` |
| ContractType | `DockPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TimePickerPresenterTheme.axaml` 模板的 `DockPanel #PART_MainLayout` |
| 职责 | 时间面板内容容器，组织时间区与底部按钮区的布局。 |
| 相关 API | 无（面板内容布局容器） |
| 相关 Token | TimePickerToken |
| 稳定性 | stable since 6.0 |

### `popup.content`

| 字段 | 值 |
| --- | --- |
| Owner | `TimePicker` / `RangeTimePicker` |
| Part | `popup.content` |
| Selector | `.semantic-time-content` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-time-content` |
| Style Type | `TimePickerPopupContentStyle` / `RangeTimePickerPopupContentStyle` |
| ContractType | `Grid` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TimeViewTheme.axaml` 的 `Grid #PART_PickerContainer`（时/分/秒/时段四列布局容器） |
| 职责 | 时间列布局容器，按 12/24 小时制组织全部时间列。 |
| 相关 API | 无（随 `popup.container` 呈现） |
| 相关 Token | TimePickerToken |
| 稳定性 | stable since 6.0 |

### `popup.column`

| 字段 | 值 |
| --- | --- |
| Owner | `TimePicker` / `RangeTimePicker` |
| Part | `popup.column` |
| Selector | `.semantic-time-column` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-time-column` |
| Style Type | `TimePickerPopupColumnStyle` / `RangeTimePickerPopupColumnStyle` |
| ContractType | `Panel` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TimeViewTheme.axaml` 的四个列宿主 Panel：`PART_HourHost` / `PART_MinuteHost` / `PART_SecondHost` / `PART_PeriodHost`（各列宽度 owner） |
| 职责 | 单个时间列宿主，承载滚动视口与列宽基线（时/分/秒列宽 = `ItemWidth`，时段列宽 = `PeriodHostWidth`）。 |
| 相关 API | 无（随 `popup.content` 呈现） |
| 相关 Token | TimePickerToken（`ItemWidth`、`PeriodHostWidth`） |
| 稳定性 | stable since 6.0 |

### `popup.item`

| 字段 | 值 |
| --- | --- |
| Owner | `TimePicker` / `RangeTimePicker` |
| Part | `popup.item` |
| Selector | `.semantic-time-item` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-time-item` |
| Style Type | `TimePickerPopupItemStyle` / `RangeTimePickerPopupItemStyle` |
| ContractType | `ListBoxItem` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `DateTimePickerPanel` 视口内运行时创建的 `TimeViewCell`（`CreateOrDestroyItems` 创建路径，创建时注入 marker） |
| 职责 | 时间格子项，承载可选时间值与选中 / hover 状态视觉。 |
| 相关 API | 无（随 `popup.column` 呈现） |
| 相关 Token | TimePickerToken（`ItemHeight`） |
| 稳定性 | stable since 6.0 |

### `popup.footer`

| 字段 | 值 |
| --- | --- |
| Owner | `TimePicker` / `RangeTimePicker` |
| Part | `popup.footer` |
| Selector | `.semantic-popup-footer` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-footer` |
| Style Type | `TimePickerPopupFooterStyle` / `RangeTimePickerPopupFooterStyle` |
| ContractType | `PixelAlignedBorder` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TimePickerPresenterTheme.axaml` 的 `PixelAlignedBorder #PART_ButtonsFrame`（内含 `PART_NowButton` / `PART_ConfirmButton`） |
| 职责 | 面板底部操作区，承载此刻 / 确认按钮。 |
| 相关 API | `IsNeedConfirm`、`IsShowNow` |
| 相关 Token | TimePickerToken |
| 稳定性 | stable since 6.0 |

`ContractType` 不参与 selector 匹配，只约束生成 Style 的 `x:SetterTargetType` 与模板校验的类型兼容；internal
实现类型（`InfoPickerTextBox`、`TimePickerPresenter`、`TimeView`、`DateTimePickerPanel`）统一承诺到最低 public
基类或公开包装类型（`TextBox`、`ContentPresenter`、`DockPanel`、`Grid`、`Panel`、`ListBoxItem`、
`PixelAlignedBorder`——footer 物理节点是 `PixelAlignedBorder`，它继承 `DashedBorder` 而非 Avalonia `Border`，
与 DatePicker 家族的 `popup.footer` 契约保持一致）。

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
TimePicker
  -> InfoPickerTextBox (control theme, InfoPickerTextBoxTheme.axaml)
  -> PickerClearUpButton (control theme, PickerClearUpButtonTheme.axaml)
     -> Panel (template-stable)
        -> InputClearIconButton#PART_ClearButton (template-stable)
        -> StackPanel#IconLayout (template-stable)
           -> IconPresenter#PART_InfoIconPresenter (template-stable)
           -> ContentPresenter#FormFeedBack (internal-observable)
  -> TimePickerPresenter (presenter control theme, TimePickerPresenterTheme.axaml)
     -> Border (template-stable)
        -> DockPanel#PART_MainLayout (template-stable)
           -> PixelAlignedBorder#PART_ButtonsFrame (template-stable)
              -> Panel#PART_ButtonsLayout (template-stable)
                 -> Button#PART_NowButton (template-stable)
                 -> Button#PART_ConfirmButton (template-stable)
           -> TimeView#PART_TimeView (template-stable)
  -> TimePicker (control theme, TimePickerTheme.axaml)
  -> TimeView (control theme, TimeViewTheme.axaml)
     -> Border#PART_MainFrame (template-stable)
        -> Grid#PART_RootLayout (template-stable)
           -> TextBlock#PART_HeaderText (template-stable)
           -> Rectangle (template-stable)
           -> Grid#PART_PickerContainer (template-stable)
              -> Panel#PART_HourHost (template-stable)
                 -> ScrollViewer (template-stable)
                    -> DateTimePickerPanel#PART_HourSelector (template-stable)
              -> Rectangle#PART_FirstSpacer (template-stable)
              -> Panel#PART_MinuteHost (template-stable)
                 -> ScrollViewer (template-stable)
                    -> DateTimePickerPanel#PART_MinuteSelector (template-stable)
              -> Rectangle#PART_SecondSpacer (template-stable)
              -> Panel#PART_SecondHost (template-stable)
                 -> ScrollViewer (template-stable)
                    -> DateTimePickerPanel#PART_SecondSelector (template-stable)
              -> Rectangle#PART_ThirdSpacer (template-stable)
              -> Panel#PART_PeriodHost (template-stable)
                 -> ScrollViewer (template-stable)
                    -> DateTimePickerPanel#PART_PeriodSelector (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `TimePicker` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `InfoPickerTextBox` | control theme | `InfoPickerTextBoxTheme.axaml` | TimePicker | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PickerClearUpButton` | control theme | `PickerClearUpButtonTheme.axaml` | TimePicker | `FormFeedback`, `Icon`, `IsFormFeedbackVisible`, `IsInClearMode` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `PickerClearUpButtonTheme.axaml` | PickerClearUpButton | `FormFeedback`, `Icon`, `IsFormFeedbackVisible`, `IsInClearMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ClearButton` | template node (InputClearIconButton) | `PickerClearUpButtonTheme.axaml` | PickerClearUpButton | `IsInClearMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IconLayout` | template node (StackPanel) | `PickerClearUpButtonTheme.axaml` | PickerClearUpButton | `FormFeedback`, `Icon`, `IsFormFeedbackVisible`, `IsInClearMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_InfoIconPresenter` | template node (IconPresenter) | `PickerClearUpButtonTheme.axaml` | PickerClearUpButton | `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FormFeedBack` | template node (ContentPresenter) | `PickerClearUpButtonTheme.axaml` | PickerClearUpButton | `FormFeedback`, `IsFormFeedbackVisible` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TimePickerPresenter` | presenter control theme | `TimePickerPresenterTheme.axaml` | TimePicker | `BorderThickness`, `ClockIdentifier`, `IsButtonsPanelVisible`, `IsMotionEnabled`, `MinuteIncrement`, `SecondIncrement` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_MainLayout` | template node (DockPanel) | `TimePickerPresenterTheme.axaml` | TimePickerPresenter | `BorderThickness`, `ClockIdentifier`, `IsButtonsPanelVisible`, `IsMotionEnabled`, `MinuteIncrement`, `SecondIncrement` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ButtonsFrame` | template node (PixelAlignedBorder) | `TimePickerPresenterTheme.axaml` | TimePickerPresenter | `BorderThickness`, `IsButtonsPanelVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ButtonsLayout` | template node (Panel) | `TimePickerPresenterTheme.axaml` | TimePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_NowButton` | template node (Button) | `TimePickerPresenterTheme.axaml` | TimePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ConfirmButton` | template node (Button) | `TimePickerPresenterTheme.axaml` | TimePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TimeView` | template node (TimeView) | `TimePickerPresenterTheme.axaml` | TimePickerPresenter | `ClockIdentifier`, `IsMotionEnabled`, `MinuteIncrement`, `SecondIncrement`, `SelectedTime` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TimePicker` | control theme | `TimePickerTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `TimeView` | control theme | `TimeViewTheme.axaml` | TimePicker | `Background`, `IsMotionEnabled`, `IsShowHeader`, `Padding`, `SpacerWidth` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_MainFrame` | template node (Border) | `TimeViewTheme.axaml` | TimeView | `Background`, `IsMotionEnabled`, `IsShowHeader`, `Padding`, `SpacerWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RootLayout` | template node (Grid) | `TimeViewTheme.axaml` | TimeView | `IsMotionEnabled`, `IsShowHeader`, `SpacerWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderText` | template node (TextBlock) | `TimeViewTheme.axaml` | TimeView | `IsShowHeader` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PickerContainer` | template node (Grid) | `TimeViewTheme.axaml` | TimeView | `IsMotionEnabled`, `SpacerWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HourHost` | template node (Panel) | `TimeViewTheme.axaml` | TimeView | `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HourSelector` | template node (DateTimePickerPanel) | `TimeViewTheme.axaml` | TimeView | `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FirstSpacer` | template node (Rectangle) | `TimeViewTheme.axaml` | TimeView | `SpacerWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MinuteHost` | template node (Panel) | `TimeViewTheme.axaml` | TimeView | `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MinuteSelector` | template node (DateTimePickerPanel) | `TimeViewTheme.axaml` | TimeView | `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SecondSpacer` | template node (Rectangle) | `TimeViewTheme.axaml` | TimeView | `SpacerWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SecondHost` | template node (Panel) | `TimeViewTheme.axaml` | TimeView | `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SecondSelector` | template node (DateTimePickerPanel) | `TimeViewTheme.axaml` | TimeView | `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ThirdSpacer` | template node (Rectangle) | `TimeViewTheme.axaml` | TimeView | `SpacerWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PeriodHost` | template node (Panel) | `TimeViewTheme.axaml` | TimeView | `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PeriodSelector` | template node (DateTimePickerPanel) | `TimeViewTheme.axaml` | TimeView | `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `IsShowHeader`、`ItemFormat`、`ItemHeight` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `RangeEndSelectedTime`、`RangeStartSelectedTime`、`SelectedTime`、`SelectorRowCount` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsNeedConfirm`、`IsShowNow`、`ShouldLoop` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 其他稳定入口 | `ClockIdentifier`、`DefaultTime`、`MinuteIncrement`、`PanelType`、`PickerDisplayTime`、`RangeEndDefaultTime`、`RangeStartDefaultTime`、`SecondIncrement` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | TimePicker Token + ControlTheme。 |

## State Flow

TimePicker 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `SelectedTime` 是单时间选择的唯一用户值 owner；外部绑定、Form set/get、清除和弹层提交都必须收敛到该属性。
- `PickerDisplayTime` 只定义弹出面板打开时的显示锚点；它不得写入 `SelectedTime`，也不得改变 `DefaultTime` 的 reset 语义。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

TimePicker 的视觉模型由 `InputControlFrame` 输入表面、InfoPicker 输入子控件、控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。输入表面状态由 shared frame 统一表达，TimePicker 主题只扩展时间面板、范围和弹层内容。

| 主题文件 | 职责 |
| --- | --- |
| `RangeTimePickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `InfoPickerTextBoxTheme.axaml` | 通过 `StyleVariant=Borderless` 提供内部时间文本输入的无 chrome 布局。 |
| `InputControlFrameTheme.axaml` | 提供输入表面 variant、effective status、focus、disabled、error、warning、CompactSpace 和 motion。 |
| `TimePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |
| `TimePickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TimeViewCellTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TimeViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

TimePicker 使用 `TimePickerToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

TimePicker Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TimePickerToken`，scope id 为 `TimePicker`，源码位于 `src/AtomUI.Desktop.Controls/TimePicker/TimePickerToken.cs`。

## Customization Boundaries

维护 TimePicker 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 TimePicker 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
- Semantic Part marker 的维护边界：共享 `InfoPickerInputTheme.axaml` 承载单选触发区静态 marker（`semantic-scope-input`、`semantic-prefix`、`semantic-input`、`semantic-suffix`、`semantic-scope-handle`、`semantic-popup-root`）；`RangeTimePickerTheme.axaml` 承载范围触发区同名 marker 与 `semantic-secondary-input`；共享 `PickerClearUpButtonTheme.axaml` 承载 `semantic-clear` marker（`clear` Part 声明 `CrossNestedOwners=true`，生成器沿 PickerClearUpButton 主题链校验）；`TimePickerPresenterTheme.axaml` 承载 `semantic-popup-container` / `semantic-popup-footer`；`TimeViewTheme.axaml` 承载 `semantic-time-content` / `semantic-time-column`（×4 列宿主）。运行时注入点：`DateTimePickerPanel.CreateOrDestroyItems` 创建 `TimeViewCell` 时追加 `popup.item` 的生成 selector class 常量。marker 随实例创建一次，滚动复用、循环搬移、`ClockIdentifier` 切换、弹层重开和容器回收路径不得增删；`TimeViewTheme` 的 `semantic-time-*` marker 对未声明该契约的 DatePicker 家族保持 inert，共享主题 marker 中的同名 Part 类（`semantic-popup-*`）在两个 picker 家族各自的弹层内互不嵌套。
