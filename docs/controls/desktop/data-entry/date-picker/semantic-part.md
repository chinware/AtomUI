# DatePicker Semantic Part 契约

本文档定义 DatePicker 家族公开的 Semantic Part、Selector、类型约束、数量语义和定制边界。DatePicker 家族包含两个
Semantic owner：单值 `DatePicker` 与范围 `RangeDatePicker`（语义对齐上游 Ant Design `DatePicker` / `RangePicker` 两个
owner 的分区式 `classNames` / `styles`）。控件整体设计见 [DatePicker 桌面版架构设计](overview.md)，真实模板、marker
映射与生命周期见 [DatePicker 桌面版实现原理](implementation.md)，系统级规则见
[AtomUI Semantic Part 系统设计](../../../../architecture/systems/theming/semantic-parts.md)。

## 1. Semantic Parts

`DatePicker` 公开 12 个 Semantic Part，`RangeDatePicker` 公开 13 个 Semantic Part（后者多出范围双输入框的
`secondaryInput`）。声明分别位于 `DatePicker.SemanticParts.cs` 与 `RangeDatePicker.SemanticParts.cs` partial 文件。

触发区部件的 marker 分布：

- 单值 `DatePicker` 的宿主模板是共享 `InfoPickerInputTheme.axaml`（`DatePickerTheme.axaml` 纯 `BasedOn` 继承，
  无自有模板）；`RangeDatePicker` 的宿主模板是自有 `RangeDatePickerTheme.axaml`。两个宿主模板按同一模式标注
  `semantic-scope-input`（AddOnDecoratedBox 节点）、`semantic-input`（`PART_InfoInputBox`）、`semantic-suffix`
  （右侧内容 StackPanel）、`semantic-scope-handle`（PickerClearUpButton 节点）与 `semantic-popup-root`
  （`PART_Popup` 内的 `ArrowDecoratedBox` / `DualMonthArrowDecoratedBox`）。
- `prefix` 借用共享 `AddOnDecoratedBoxTheme` 的 `.semantic-scope-prefix` scope 锚点路由到宿主模板新增的
  `AddOnContentPresenter` 投影节点（与 Select 家族同构；投影节点以
  `CompiledBinding $parent[atom:InfoPickerInput].ContentLeftAddOn` 呈现公共 API 值）。
- `clear` 的物理按钮在共享 `PickerClearUpButtonTheme.axaml` 模板内（`PART_ClearButton`），声明
  `CrossNestedOwners=true`，生成器沿 PickerClearUpButton 主题链校验；该共享主题同时服务于未来 TimePicker
  家族的同名 Part（inert marker，未声明契约的控件零影响）。
- `secondaryInput` 仅 `RangeDatePicker` 声明，标注自有模板的 `PART_SecondaryInfoInputBox`。

弹层部件的 marker 分布（弹层内容全部由 owner `CreatePickerPresenter` 在首次打开时运行时创建）：

- `popup.container` / `popup.footer` 标注 presenter 主题模板节点：`popup.container` 在模板根 `DockPanel #RootLayout`，
  `popup.footer` 在 `PixelAlignedBorder #ButtonsFrame`；三个带模板的 presenter 主题
  （`DatePickerPresenterTheme.axaml`、`DualMonthRangeDatePickerPresenterTheme.axaml`、
  `TimedRangeDatePickerPresenterTheme.axaml`）均标注。`RangeDatePickerPresenterTheme.axaml` 纯继承无模板。
- `popup.header` / `popup.body` / `popup.content` 标注 `CalendarItemTheme.axaml`（单月：
  `PART_HeaderFrame` / `PART_MonthViewLayout` / `PART_MonthView`）与 `DualMonthCalendarItemTheme.axaml`
  （双月：同名节点加 `PART_SecondaryMonthView`）。
- `popup.cell` 为运行时注入：`CalendarDayButton` 构造函数追加生成 selector class 常量（共享 CalendarView
  基础设施，两个 owner 的常量值一致），覆盖月网格重建与容器回收。
- `popup.*` 全部声明 `RuntimeCreated=true`（presenter 子树在运行时组装，生成器豁免宿主模板 marker 校验，
  由控件行为测试兜底），其中 `popup.root` 为宿主模板静态节点、`RuntimeCreated=false`。

### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `root` |
| Selector | owner 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `DatePicker` / `RangeDatePicker` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | DatePicker / RangeDatePicker owner |
| 职责 | owner 是日期值、格式化、弹层状态、Form 值与验证状态的组织边界；owner 级 `BorderBrush` 经控件中继为输入框边框颜色（root 级定制入口，未设置时恢复共享状态机）。 |
| 相关 API | 全部 DatePicker / RangeDatePicker public API |
| 相关 Token | DatePickerToken、SharedToken |
| 稳定性 | stable since 6.0 |

### `prefix`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `prefix` |
| Selector | `.semantic-prefix` |
| SelectorRoute | `/template/ .semantic-scope-input /template/ .semantic-scope-prefix > .semantic-prefix` |
| Style Type | `DatePickerPrefixStyle` / `RangeDatePickerPrefixStyle` |
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
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `input` |
| Selector | `.semantic-input` |
| SelectorRoute | `/template/ .semantic-input` |
| Style Type | `DatePickerInputStyle` / `RangeDatePickerInputStyle` |
| ContractType | `TextBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 宿主模板的 `InfoPickerTextBox #PART_InfoInputBox`（起始端输入框） |
| 职责 | 日期文本输入框，承载格式化显示值、占位符与只读/校验状态。 |
| 相关 API | `Text`、`PlaceholderText`、`Format`、`IsReadOnly`、`PreferredInputWidth` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `secondaryInput`（仅 RangeDatePicker）

| 字段 | 值 |
| --- | --- |
| Owner | `RangeDatePicker` |
| Part | `secondaryInput` |
| Selector | `.semantic-secondary-input` |
| SelectorRoute | `/template/ .semantic-secondary-input` |
| Style Type | `RangeDatePickerSecondaryInputStyle` |
| ContractType | `TextBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `RangeDatePickerTheme.axaml` 的 `InfoPickerTextBox #PART_SecondaryInfoInputBox`（结束端输入框） |
| 职责 | 范围选择的结束端日期文本输入框，与 `input` 共用格式与宽度基线。 |
| 相关 API | `SecondaryText`、`SecondaryPlaceholderText` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `suffix`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `suffix` |
| Selector | `.semantic-suffix` |
| SelectorRoute | `/template/ .semantic-scope-input /template/ .semantic-scope-suffix > .semantic-suffix` |
| Style Type | `DatePickerSuffixStyle` / `RangeDatePickerSuffixStyle` |
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
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `clear` |
| Selector | `.semantic-clear` |
| SelectorRoute | `>> .semantic-scope-handle /template/ .semantic-clear` |
| Style Type | `DatePickerClearStyle` / `RangeDatePickerClearStyle` |
| ContractType | `IconButton` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossNestedOwners | `true` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 共享 `PickerClearUpButtonTheme.axaml` 模板内的 `InputClearIconButton #PART_ClearButton` |
| 职责 | 后缀区清除按钮，进入清除模式（hover / focus）时渲染。 |
| 相关 API | `ShowClearButtonPredicate`（DatePicker）/ 范围清除行为（RangeDatePicker） |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `popup.root`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.root` |
| Selector | `.semantic-popup-root` |
| SelectorRoute | `/template/ .semantic-popup-root` |
| Style Type | `DatePickerPopupRootStyle` / `RangeDatePickerPopupRootStyle` |
| ContractType | `ArrowDecoratedBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 单选：`InfoPickerInputTheme.axaml` 中 `PART_Popup` 的 `ArrowDecoratedBox`；范围：`RangeDatePickerTheme.axaml` 中的 `DualMonthArrowDecoratedBox` |
| 职责 | 弹层内容根视觉盒子，承载背景、边框、阴影与浮动箭头；`BorderThickness` 定制为非零时盒子进入 `:bordered` 状态，内置主题自动隐藏浮动箭头（内置视觉不支持箭头与边框的融合呈现）。 |
| 相关 API | `IsArrowVisible`（经 `IsArrowVisibleEffective`）、`ArrowPosition`、`IsMotionEnabled` |
| 相关 Token | PopupToken |
| 稳定性 | stable since 6.0 |

### `popup.container`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.container` |
| Selector | `.semantic-popup-container` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-container` |
| Style Type | `DatePickerPopupContainerStyle` / `RangeDatePickerPopupContainerStyle` |
| ContractType | `DockPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | presenter 主题模板根 `DockPanel #RootLayout`（三个 presenter 主题模板均标注） |
| 职责 | 日历面板内容容器，组织主体区与底部按钮区的布局。 |
| 相关 API | 无（面板内容布局容器） |
| 相关 Token | DatePickerToken |
| 稳定性 | stable since 6.0 |

### `popup.header`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.header` |
| Selector | `.semantic-popup-header` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-header` |
| Style Type | `DatePickerPopupHeaderStyle` / `RangeDatePickerPopupHeaderStyle` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `CalendarItemTheme.axaml` / `DualMonthCalendarItemTheme.axaml` 的 `PixelAlignedBorder #PART_HeaderFrame`（双月布局内含左右两月导航按钮组） |
| 职责 | 日历年月导航头部，承载年月标题与前进/后退/翻年按钮。 |
| 相关 API | 无（导航按钮交互由 CalendarView 内部承担） |
| 相关 Token | DatePickerToken |
| 稳定性 | stable since 6.0 |

### `popup.body`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.body` |
| Selector | `.semantic-popup-body` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-body` |
| Style Type | `DatePickerPopupBodyStyle` / `RangeDatePickerPopupBodyStyle` |
| ContractType | `UniformGrid` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `CalendarItemTheme.axaml` 的 `UniformGrid #PART_MonthViewLayout`；双月为 `DualMonthCalendarItemTheme.axaml` 的同名节点（Columns=2，包住两张月表） |
| 职责 | 日期面板表格容器，按月视图/年视图模式承载表格布局。 |
| 相关 API | 无 |
| 相关 Token | DatePickerToken |
| 稳定性 | stable since 6.0 |

### `popup.content`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.content` |
| Selector | `.semantic-popup-content` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-content` |
| Style Type | `DatePickerPopupContentStyle` / `RangeDatePickerPopupContentStyle` |
| ContractType | `Grid` |
| Cardinality | `DatePicker`: `Single`；`RangeDatePicker`: `Multiple`（双月两张表，带时间单月一张） |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 单月：`CalendarItemTheme.axaml` 的 `Grid #PART_MonthView`；双月：`DualMonthCalendarItemTheme.axaml` 的 `PART_MonthView` 与 `PART_SecondaryMonthView` |
| 职责 | 单个月份的 7×7 日期表格本体（含周序号列变体），承载日期格子与周头标题。 |
| 相关 API | 无（随 `popup.body` 呈现） |
| 相关 Token | DatePickerToken |
| 稳定性 | stable since 6.0 |

### `popup.cell`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.cell` |
| Selector | `.semantic-cell` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-cell` |
| Style Type | `DatePickerPopupCellStyle` / `RangeDatePickerPopupCellStyle` |
| ContractType | `Button` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 月网格运行时创建的 `CalendarDayButton`（`CalendarItem.PopulateMonthViewGrid` 与 `DualMonthCalendarItem.PopulateMonthViewsGrid` 创建路径，构造时注入 marker） |
| 职责 | 日期格子按钮，承载可选日期、选中/范围/今天/禁用等状态视觉（伪类见 overview）。 |
| 相关 API | 无（随 `popup.content` 呈现） |
| 相关 Token | DatePickerToken |
| 稳定性 | stable since 6.0 |

### `popup.footer`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.footer` |
| Selector | `.semantic-popup-footer` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-footer` |
| Style Type | `DatePickerPopupFooterStyle` / `RangeDatePickerPopupFooterStyle` |
| ContractType | `PixelAlignedBorder` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | presenter 主题模板的 `PixelAlignedBorder #ButtonsFrame`（内含 `PART_NowButton` / `PART_TodayButton` / `PART_ConfirmButton`） |
| 职责 | 面板底部操作区，承载此刻/今天/确认按钮。 |
| 相关 API | `IsNeedConfirm`、`IsShowNow` |
| 相关 Token | DatePickerToken |
| 稳定性 | stable since 6.0 |

## 2. 职责与存在条件

- 触发区 `prefix` / `suffix` 的路由借用共享 `AddOnDecoratedBoxTheme` 的 scope 锚点；marker 本体在宿主模板内，
  不声明 `CrossNestedOwners`。`clear` 的锚点 `semantic-scope-handle` 位于宿主模板 PickerClearUpButton 节点，
  `>>` 首步覆盖该节点位于 `AddOnDecoratedBox.ContentRightAddOn` 属性值子树（无 `TemplatedParent` 传播）的场景，
  生成器沿 PickerClearUpButton 主题链在 `PickerClearUpButtonTheme.axaml` 校验 marker 数量与类型。
- `popup.root` 是宿主模板 `PART_Popup` 的静态子节点（Popup.Child），marker 为静态模板节点标注；
  `popup.container` / `popup.footer` / `popup.header` / `popup.body` / `popup.content` 的 marker 静态写在
  DatePicker 家族内部主题（presenter / CalendarItem 主题）模板节点上，`popup.cell` 由 `CalendarDayButton`
  构造函数注入；由于整个 presenter 子树在首次打开时运行时组装，以上部件统一声明 `RuntimeCreated=true`，
  生成器豁免宿主模板校验，由行为测试验证 marker 存在性与路由命中。
- **单值 / 范围存在条件**：
  - 单值 `DatePicker`：单月日历，`popup.content` 恒为 1 个；`secondaryInput` 不存在。
  - `RangeDatePicker`：默认双月布局（`popup.content` 为 2 个）；`IsShowTime=true` 且 `PickerMode=Date` 时
    切换为带时钟的单月布局（`popup.content` 为 1 个，`TimeView` 出现在 presenter 内容区但不属于契约）。
  - `popup.footer` 仅在 `IsButtonsPanelVisible=true` 时可见（`IsNeedConfirm` 提供 Confirm；
    `IsShowNow && PickerMode=Date` 提供 Now/Today 二选一）；不可见是可见性切换，marker 不增删。
  - `clear` 仅在清除模式（hover / focus 且存在可清除值）可见。
- `DisplayMode`（月视图 ↔ 年/十年视图）切换只改变 `PART_MonthViewLayout` / `PART_YearView` 的可见性；
  `popup.header` / `popup.body` / `popup.content` marker 恒存在。年/十年面板的 `CalendarButton`
  （`PART_YearView` 内运行时创建）不是 `popup.cell`，见 §5。
- `PickerMode`（Date/Week/Month/Quarter/Year）只改变 cell 内容与月网格列数（Week 为 8 列含周序号列），
  不改变 marker 身份与数量语义。

## 3. 数量语义

`root`、`prefix`、`input`、`suffix`、`clear`、`popup.root`、`popup.container`、`popup.header`、`popup.body`、
`popup.footer` 均为静态模板节点或每弹层唯一容器，`Single`；状态变化（清除模式、footer 可见性、DisplayMode、
`PickerMode`、disabled、验证状态）只切换可见性或有效视觉值，不增删 marker。
`popup.content` 在 `DatePicker` 上为 `Single`、在 `RangeDatePicker` 上为 `Multiple`（1~2 个月表）；
`popup.cell` 为 `Multiple`：随月网格 rebuild（月份切换、边界变化、模板重套用）销毁重建，构造注入保证
重建后 marker 不变。

## 4. Selector 用法

生成的 Semantic Style 类型命名为 `<Owner><PartPathPascalCase>Style`，如 `DatePickerPopupRootStyle`、
`RangeDatePickerSecondaryInputStyle`（命名空间 `AtomUI.Theme.Styling`，AXAML 命名空间 `https://atomui.net`）。
`root` 不生成 Style 类型，owner 级 Setter 写在外层普通 Style 上。

推荐写法（owner 嵌套 Style + 语义 class，对齐上游 object / function styles 示例）：

```xml
<StackPanel.Styles>
    <Style Selector="atom|DatePicker.semantic-styles-demo">
        <atom:DatePickerPrefixStyle x:SetterTargetType="ContentPresenter">
            <Style Selector="^ atom|Icon">
                <Setter Property="StrokeBrush" Value="#1890FF" />
            </Style>
        </atom:DatePickerPrefixStyle>
        <atom:DatePickerPopupRootStyle x:SetterTargetType="ArrowDecoratedBox">
            <Setter Property="BorderBrush" Value="#1890FF" />
        </atom:DatePickerPopupRootStyle>
    </Style>
    <Style Selector="atom|RangeDatePicker.semantic-styles-demo">
        <atom:RangeDatePickerInputStyle x:SetterTargetType="TextBox">
            <Setter Property="FontStyle" Value="Italic" />
        </atom:RangeDatePickerInputStyle>
        <atom:RangeDatePickerPopupCellStyle x:SetterTargetType="Button">
            <Setter Property="Foreground" Value="#722ED1" />
        </atom:RangeDatePickerPopupCellStyle>
    </Style>
</StackPanel.Styles>
<atom:DatePicker Classes="semantic-styles-demo" ... />
<atom:RangeDatePicker Classes="semantic-styles-demo" ... />
```

`popup.*` 部件的生成 Style 在弹层打开后命中目标；Gallery Semantic Parts 页签以钉住常开弹层呈现全部弹层部件。
`prefix` 为 `Icon` 时，AtomUI 图标由 `StrokeBrush` / `FillBrush` 驱动而非 `Foreground`，需在
`DatePickerPrefixStyle` 内嵌套 `<Style Selector="^ atom|Icon">` 设置 `StrokeBrush`。
`popup.cell` 的样式作用于格子按钮本体；选中/范围/今天/禁用等状态视觉由 `CalendarDayButton` 伪类承担，
Semantic Style 遵循 Avalonia 原生属性优先级。

不得使用以下写法：

- `.semantic-root`、`PART_*`、Name selector、internal 类型或视觉祖先顺序作为应用主题契约。
- 把 `ArrowDecoratedBox.semantic-popup-root` 等 `ContractType` 写入 Part 身份 selector。
- 直接复制 `/template/ .semantic-scope-*` route；scope class 只用于生成 Style 的 owner-relative 路由。
- 穿过 `DayTitleTemplate`、`ContentLeftAddOnTemplate` 等用户模板继续匹配内部 Visual。

## 5. 定制边界

以下区域不属于 DatePicker 家族 Semantic Part：

- **时钟面板**：`TimeView #PART_TimeView`（带时间选择时出现在 presenter 内容区）属于 TimePicker 家族
  未来自身的 Semantic Part 契约，不经 DatePicker 发布。
- **年/十年面板**：`PART_YearView` 内运行时创建的 `CalendarButton`（月/季度/年/十年选择格子）是
  `DisplayMode` 切换的内部实现，上游同样无对应槽位，不发布。
- **周头标题**：月网格第 0 行由 `DayTitleTemplate` 构建的星期标题不是 `popup.cell`。
- **格子内部指示层**：`CalendarDayButton` 模板内的 range/week indicator Border 层由其自身模板拥有，
  `popup.cell` 只承诺格子按钮本体。
- **范围指示与换向箭头**：`PART_RangePickerIndicator`、`PART_RangePickerArrow` 是 Range 触发区的
  附属视觉，上游无对应槽位，不发布。
- **输入框内部结构**：占位符、文本 presenter 由 `InfoPickerTextBox`（`EmbeddedTextBox`）内部承载，
  不经 DatePicker 发布（上游 DatePicker 亦无 `placeholder` / `content` 槽）。
- **弹层宿主与定位**：`PART_Popup` 的定位、钉住打开、动画、Overlay 归共享 Popup 契约；`popup.root`
  只覆盖弹层内容根盒子的视觉。
- **输入表面**：variant、status、背景与状态描边由共享 `InputControlFrame` / `AddOnDecoratedBox`
  承担，归共享输入契约；owner 级 `BorderBrush` 经 LocalValue 中继可整体覆盖触发框边框颜色
  （与 NumericUpDown 家族的 root BorderBrush 中继同构），其余输入表面状态不经 owner 发布。
- `PART_*` 名称、internal 类型、`.semantic-scope-*` 路由标记与模板层级。

默认主题不消费 `.semantic-*` selector；静态 marker 只提供应用样式命中点，不改变默认属性优先级或增加状态
订阅。共享 `InfoPickerInputTheme` / `PickerClearUpButtonTheme` 中的 marker 对尚未声明契约的 TimePicker
家族是 inert class。

## 6. 兼容性与验证

删除或重命名 Part、修改 selector class / route、收窄 `ContractType`（含把公共基类承诺收窄为具体实现
类型）、改变 cardinality，或让任一内置模板变体（单值共享模板、Range 自有模板、三个 presenter 模板、
单月/双月 CalendarItem 模板）缺少 marker，均属于公共主题契约变更。

与上游 antd DatePicker 的对照差异（有意保持）：

- 上游 `DatePickerSemanticType` 未声明 `clear` 槽（清除样式归入 `suffix` 描述）；AtomUI Select 家族将
  `clear` 作为独立 Part 发布（物理节点在共享 `PickerClearUpButtonTheme` 内），DatePicker 家族保持一致。
- 上游 popup 组的日期格子槽名为 `item`；AtomUI 采用 `popup.cell`（批次任务预划命名，格子语义比列表项
  语义更贴合日历网格，与 Select 家族 `popup.listItem` 的本地化命名策略一致）。
- 上游未区分范围双输入框（`input` 槽作用于激活输入框）；AtomUI 依据双 `InfoPickerTextBox` 结构事实发布
  `input` + `secondaryInput` 两个 Part。
- 上游 `popup.root` 是定位包装层、`popup.container` 是可见面板盒；AtomUI 的定位层是 `Popup` 本体（不可作为
  样式目标），故 `popup.root` 落在内容根盒子（ArrowDecoratedBox）上、`popup.container` 落在面板内容容器
  （presenter 模板根）上，两槽各有所指、不共用节点。
- 上游 popup 组的 `body`（面板表格容器）与 `content`（表格本体）分别对应 AtomUI 的 `PART_MonthViewLayout`
  与 `PART_MonthView`/`PART_SecondaryMonthView`。

验证至少覆盖：

- owner descriptor 只包含 §1 声明的 Part（DatePicker 12 个、RangeDatePicker 13 个），字段值与本文一致。
- `InfoPickerInputTheme.axaml` 携带单选触发区与 `popup.root` marker；`RangeDatePickerTheme.axaml` 携带范围
  触区、`secondaryInput` 与 `popup.root` marker；`PickerClearUpButtonTheme.axaml` 携带 `clear` marker。
- 三个 presenter 主题模板携带 `popup.container` / `popup.footer` marker；`CalendarItemTheme.axaml` 与
  `DualMonthCalendarItemTheme.axaml` 携带 `popup.header` / `popup.body` / `popup.content` marker（双月含
  secondary 月表）。
- `popup.cell` marker 在月网格创建路径注入（单月、双月、周模式 8 列），月网格 rebuild 与容器回收后
  marker 保持。
- 生成的 `DatePicker*Style` / `RangeDatePicker*Style` 可编译并命中目标节点（见
  `tests/AtomUI.Desktop.Controls.Tests/DatePicker/DatePickerSemanticPartTests.cs`）。
- 弹层首次打开、关闭、重开与模板重套用后弹层部件 marker 保持；`IsShowTime` 切换（单月+时钟 ↔ 双月）
  不丢失 marker。
- 状态变化（清除模式、footer 可见性、DisplayMode、`PickerMode`、disabled）不改变 marker 身份与数量语义。
- 默认主题不消费 `.semantic-*`，未声明用户 Semantic Style 时不增加 selector activator；共享主题 marker
  对 TimePicker / RangeTimePicker 既有视觉零影响。
- Gallery Semantic Parts Tab 延迟创建 Preview，弹层钉住常开并以范围预选呈现双月，13 个 Part 均可解析高亮。
- descriptor、生成 Style 与 NativeAOT 路径使用编译期生成数据，不依赖运行时反射或 VisualTree 扫描。
