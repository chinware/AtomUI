# DatePicker 桌面版架构设计

本文档定义 `DatePicker` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。共享输入分层见 [输入控件共享架构设计](../input-control-architecture-design.md)，通用控件研发约束见 [控件研发标准](../../../../engineering/development/control-development-guidelines.md)，内部实现原理见 [DatePicker 桌面版实现原理](implementation.md)，CalendarView 的系统性优化设计见 [CalendarView 系统性优化设计](calendar-view-system-optimization.md)，DatePicker Token 的专项设计见 [DatePicker Token 设计](token.md)，设计和契约变化记录见 [DatePicker Changelog](changelog.md)。

该控件的 Popup 钉住打开属于共享弹层契约，详见 [Popup 钉住打开设计](../../other/popup/popup-pinned-open-design.md)。本控件的语义 owner 为 `InfoPickerInput`，其 internal `IsPopupPinnedOpen` 只供测试和内部诊断使用；设置为 true 时保持 picker open state 并 relay 到 `PickerPopup`，设置为 false 时只解除关闭拦截。控件卸载、锚点失效、TopLevel 改变和模板重建仍按共享生命周期规则清理。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker` |
| 控件状态 | Stable |

DatePicker 是 AtomUI 桌面控件体系中的日期选择控件，用于单日期、周、月份、季度、年份以及日期范围的日历面板选择，并通过可选的包含式日期边界约束面板导航和用户选择。

DatePicker 不负责业务日程系统、时间选择或完整日期时间解析服务。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/DatePicker`

## 2. 设计语言

DatePicker 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | DatePicker 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | DatePicker 是 AtomUI 桌面控件体系中的日期选择控件，用于单日期、日期范围和日历面板选择。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `HeaderBackground`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | DatePicker Token + ControlTheme。 |

## 3. API 与契约模型

DatePicker 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `HeaderBackground` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `PickerMode`、`RangeEndSelectedDate`、`RangeStartSelectedDate`、`SelectedDateTime` | 维护提交值、范围端点和选择颗粒度；`SelectedDateTime`、`RangeStartSelectedDate`、`RangeEndSelectedDate` 默认 `TwoWay` 绑定并启用 Avalonia data validation。 |
| 日期边界 | `MinDate`、`MaxDate` | 以包含边界限制可选 picker unit 和面板导航范围；默认值均为 `null`，表示对应方向无边界。 |
| 弹层显示游标 | `PickerDisplayDate`；内部 `Calendar.DisplayDate`、`DisplayDateStart`、`DisplayDateEnd` | 维护弹出面板打开时显示到哪个日期区域，不代表已选值。 |
| 交互与状态 | `IsFloatingArrowPosition`、`IsHorizontalFlipped`、`IsNeedConfirm`、`IsShowNow`、`IsShowTime`、`IsTodayHighlighted` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `RangePickerIndicatorOffsetEnd`、`RangePickerIndicatorOffsetStart` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `ClockIdentifier`、`DefaultDateTime`、`Format` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

日期边界 public API：

| 属性 | 类型 | 默认值 | 契约 |
| --- | --- | --- | --- |
| `MinDate` | `DateTime?` | `null` | 最小可选日期，边界包含在有效范围内；忽略时间部分并按 `PickerMode` 归一化。 |
| `MaxDate` | `DateTime?` | `null` | 最大可选日期，边界包含在有效范围内；忽略时间部分并按 `PickerMode` 归一化。 |

`RangeDatePicker` 通过 `AddOwner` 复用 `DatePicker.MinDateProperty` 和 `DatePicker.MaxDateProperty`，单值与范围选择使用同一日期边界契约。日期边界只限制可选值和面板导航，不公开内部 `Calendar.BlackoutDates`，也不承担动态业务禁用规则。

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`Calendar`、`CalendarItem`、`ChoosingStatusEventArgs`、`DatePicker`、`DatePickerPresenter`、`DateSelectedEventArgs`、`DualMonthArrowDecoratedBox`、`DualMonthCalendarItem`、`DualMonthRangeCalendar`、`DualMonthRangeDatePickerPresenter`、`RangeCalendar`、`RangeCalendarItem`、`RangeDatePicker`、`RangeDatePickerPresenter` 等 19 项。
- 枚举：`DatePickerMode`，取值为 `Date`、`Week`、`Month`、`Quarter`、`Year`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_CalendarItem` | `CalendarItem` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_CalendarView` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ClearUpButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ConfirmButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ContentRightAddOnPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_HeaderButton` | `HeadTextButton` | 承载用户触发入口、导航或关闭动作。 |
| `PART_HeaderFrame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_HeaderLayout` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_InfoInputBox` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_ItemFrame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_ItemRootLayout` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_MonthView` | `Grid` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_MonthViewLayout` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_NextButton` | `IconButton` | 承载用户触发入口、导航或关闭动作。 |
| `PART_NextMonthButton` | `IconButton` | 承载用户触发入口、导航或关闭动作。 |
| `PART_NowButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_Popup` | `?` | 承载弹层宿主、打开关闭或候选内容。 |
| `PART_PreviousButton` | `IconButton` | 承载用户触发入口、导航或关闭动作。 |
| `PART_PreviousMonthButton` | `IconButton` | 承载用户触发入口、导航或关闭动作。 |
| `PART_RangePickerArrow` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_RangePickerIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_SecondaryHeaderButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_SecondaryInfoInputBox` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_SecondaryMonthView` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| 其他 part | 7 项 | 参见源码和主题文件；维护时按同一生命周期规则检查。 |

控件专属或内部伪类包括 `Blackout=:blackout`、`BtnFocusedPC`、`CalendarDayButtonPseudoClass.Blackout`、`CalendarDayButtonPseudoClass.DayFocused`、`CalendarDayButtonPseudoClass.RangeEnd`、`CalendarDayButtonPseudoClass.RangeMiddle`、`CalendarDayButtonPseudoClass.RangePreviewEnd`、`CalendarDayButtonPseudoClass.RangePreviewMiddle`、`CalendarDayButtonPseudoClass.RangePreviewStart`、`CalendarDayButtonPseudoClass.RangeStart`、`CalendarDayButtonPseudoClass.Today`、`CalendarDayButtonPseudoClass.WeekHoverStart`、`CalendarDayButtonPseudoClass.WeekHoverMiddle`、`CalendarDayButtonPseudoClass.WeekHoverEnd`、`CalendarDayButtonPseudoClass.WeekSelectionStart`、`CalendarDayButtonPseudoClass.WeekSelectionMiddle`、`CalendarDayButtonPseudoClass.WeekSelectionEnd`、`CalendarDisabledPC`、`DayFocused=:dayfocused`、`RangeEnd=:range-end`、`RangeMiddle=:range-middle`、`RangePreviewEnd=:range-preview-end`、`RangePreviewMiddle=:range-preview-middle`、`RangePreviewStart=:range-preview-start` 等。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 4. 行为与状态模型

DatePicker 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `PickerMode` 决定选择颗粒度和初始面板：`Date`、`Week` 使用月视图，`Month`、`Quarter` 使用年视图，`Year` 使用十年视图。目标颗粒度不能继续降级到更细面板。
- `SelectedDateTime`、`DefaultDateTime` 和 `PickerDisplayDate` 必须保持语义分离：`SelectedDateTime` 是已提交值，`DefaultDateTime` 是默认选中值/reset 值，`PickerDisplayDate` 只作为弹出面板打开时的显示锚点。
- `SelectedDateTime` 是单值 DatePicker 的受控 Form 值入口，默认 `BindingMode.TwoWay`，并通过 Avalonia `DataValidationErrors` 参与原生数据校验。
- `MinDate` 和 `MaxDate` 是包含式 picker unit 边界。两者先忽略时间部分，再按当前 `PickerMode` 归一化；`null` 表示对应方向不受限制。
- 当归一化后的 `MinDate` 晚于 `MaxDate` 时，有效范围收敛为 `MinDate` 所在的一个 picker unit，但控件不得修改或回写调用方设置的原始属性值。
- 外部受控值越界时，`SelectedDateTime`、`RangeStartSelectedDate` 和 `RangeEndSelectedDate` 保持不变，输入框继续显示外部值；Calendar 不标记越界值为选中，确认操作不可提交该值。用户选择有效日期后，才按既有 TwoWay 契约更新受控值。
- 设置 `PickerDisplayDate` 后不得写入 `SelectedDateTime`，不得改变输入框文本、Form value 或清除按钮状态；当已有已选值时，弹出面板仍优先围绕已选值展示。
- DatePicker / RangeDatePicker 必须把 `DataValidationErrors`、FormStatus 和显式 Status 投射到 shared `InputControlFrame`；range indicator 等附属视觉读取 `EffectiveStatus`，native error 优先于 Form/显式 warning/error 状态。Calendar 和 picker panel 只拥有日期选择、范围预览和面板交互状态。
- `PickerMode=Week` 的月视图是带周序号列的 8 列 week panel，不是普通日期面板的 7 个日期按钮逐个选中；选中视觉和 hover 视觉都必须按整周连续行渲染，不能退回单个日期按钮的普通 pointerover 背景。
- 非 `Date` 颗粒度仍使用 `DateTime?` 保存提交值：`Week` 保存 ISO 周起始日，`Month` 保存当月 1 日，`Quarter` 保存季度首月 1 日，`Year` 保存当年 1 月 1 日。
- `IsShowTime` 只在 `PickerMode=Date` 时形成有效时间选择；其他颗粒度忽略时间面板和时间拼接。
- 范围选择的 committed 状态和 hover preview 状态必须分开：`:selected`、`:range-start`、`:range-end`、`:range-middle` 只来自真实端点；hover 只写入 `:range-preview-start`、`:range-preview-end`、`:range-preview-middle`，其中 preview start/end 在视觉上按临时端点显示，但不能污染真实提交状态。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 5. 视觉与主题模型

DatePicker 的视觉模型由 `InputControlFrame` 输入表面、InfoPicker 输入子控件、控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。输入表面状态由 shared frame 统一表达，DatePicker 主题只扩展日期、范围和弹层内容。

| 主题文件 | 职责 |
| --- | --- |
| `CalendarButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `CalendarDayButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `CalendarItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `CalendarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `InfoPickerTextBoxTheme.axaml` | 通过 `StyleVariant=Borderless` 提供内部日期文本输入的无 chrome 布局。 |
| `InputControlFrameTheme.axaml` | 提供输入表面 variant、effective status、focus、disabled、error、warning、CompactSpace 和 motion。 |
| `DualMonthCalendarItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DualMonthRangeCalendarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `RangeCalendarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DatePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |
| `DatePickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DualMonthRangeDatePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |
| `RangeDatePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |
| `RangeDatePickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TimedRangeDatePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |

DatePicker 使用 `DatePickerToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- 日期边界外的 Calendar cell 保持可见并使用 disabled 状态视觉，不通过隐藏 cell 表达不可选择状态。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

## 6. 控件家族或集成关系

DatePicker 与同分类控件共享尺寸、状态、Token、Gallery 展示和验证规则。组合或派生控件应显式说明哪些 API 被继承、覆盖或不支持。

主要协作类型：

- `Calendar`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `CalendarButtonTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `CalendarItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `CalendarItemTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `CalendarTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `DatePicker`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DatePickerPresenter`：模板协作类型，承载内容展示、宿主或视觉边界。
- `DatePickerPresenterTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `InfoPickerTextBox`：internal `AbstractTextInput` 输入子控件，负责日期文本编辑、Form/native validation 接入和 Borderless 输入布局。
- `InputControlFrame`：internal 输入表面组合控件，负责 DatePicker/RangeDatePicker 的 variant、effective status、边框、背景和 CompactSpace 视觉。
- `DatePickerToken`：控件 Token scope，负责从全局 token 派生控件语义变量。
- `DualMonthArrowDecoratedBox`：模板协作类型，承载内容展示、宿主或视觉边界。
- `DualMonthCalendarItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `DualMonthRangeCalendar`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DualMonthRangeDatePickerPresenter`：模板协作类型，承载内容展示、宿主或视觉边界。
- `RangeCalendar`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `RangeCalendarItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `RangeDatePicker`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `RangeDatePickerPresenter`：模板协作类型，承载内容展示、宿主或视觉边界。
- `RangeDatePickerPresenterTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `TimedRangeDatePickerPresenter`：模板协作类型，承载内容展示、宿主或视觉边界。
- `DatePickerLangResourceKind`：稳定的本地化 Catalog enum；内置翻译由同目录三种语言 XLIFF 提供并在编译期生成。

集成关系：

- 与 ThemeManager、SharedToken、ControlTheme、控件文档和 Gallery ShowCase 示例保持一致。
- 涉及 ItemsSource、Popup、Flyout、Window、Form 或 CompactSpace 的路径必须保持生命周期释放和数据状态同步。
- 源码目录中的共享基类和内部协作类型形成维护边界，不能只修改桌面包装类而忽略共享状态 owner。
- `DatePicker` 与 `RangeDatePicker` 的日期边界必须通过 presenter 投影到内部 CalendarView；内部 `DisplayDateStart`、`DisplayDateEnd` 和 `BlackoutDates` 不是外层控件的替代 public API。

## 7. 兼容性不变量

维护 DatePicker 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 选择与当前项模型

DatePicker 的当前项状态必须由单一 owner 推导。public 选择属性、集合项容器和伪类之间只能做单向同步，集合替换、清空和模板重套用时必须回放当前状态。

### 8.2 视觉选项模型

DatePicker 的视觉选项通过 public API 归一为 theme variables、伪类或模板绑定。Token 保存组件语义值，不能保存实例运行时状态或业务色值。

### 8.3 PickerMode 颗粒度模型

`DatePicker.PickerMode` 和 `RangeDatePicker.PickerMode` 对齐 Ant Design 的 `picker` 模型，用一个核心控件表达日期、周、月份、季度和年份选择，不为每种颗粒度拆分独立控件。

`Format` 为空时，控件按颗粒度选择默认显示格式：日期 `yyyy-MM-dd`，周 `yyyy-ww周`，月份 `yyyy-MM`，季度 `yyyy-Qn`，年份 `yyyy`。自定义 `Format` 优先级高于默认格式。

CalendarView 必须把目标颗粒度作为状态模型的一部分处理。用户从更粗面板返回时，只能回到目标颗粒度面板；例如月份选择可以从年面板进入十年面板选择年份，但选完年份后回到月份面板，不继续进入日期面板。

### 8.4 日期边界约束模型

日期边界按当前 `PickerMode` 归一为可比较的 picker unit：

| `PickerMode` | 边界归一化单位 |
| --- | --- |
| `Date` | 当天 |
| `Week` | ISO 周起始日 |
| `Month` | 当月第一天 |
| `Quarter` | 当季度第一天 |
| `Year` | 当年第一天 |

有效边界同时约束日期 cell、周行、月份、季度、年份和面板导航。范围外单元保持可见但不可通过 pointer、keyboard 或提交动作选中；`Today` / `Now` 在目标 picker unit 越界时不可用。`RangeDatePicker` 的两个端点共享同一组边界，各端点按自身 active 状态独立校验，双面板显示锚点也必须落在有效范围内。

日期边界与受控值是两个独立状态域。边界变化可以使已有受控值失效，但不能隐式改写 ViewModel；Calendar 只投影当前有效的选中状态，输入和数据校验层继续拥有外部值的显示与校验责任。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [DatePicker 桌面版实现原理](implementation.md)
- [CalendarView 系统性优化设计](calendar-view-system-optimization.md)
- [DatePicker Token 设计](token.md)
- [DatePicker Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `DatePicker` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + `implementation.md` + `token.md` + Gallery ShowCase | 生成 `controls/date-picker/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/date-picker/semantic-cn.md` |
| API 表 | overview.md 语义摘要 + 源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | token.md、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和继承语义。 |
| 状态模型 | 覆盖 selection/checked/active、visual option、disabled、hover、pressed、focus 以及控件特有状态。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和 Browser 主题。 |
| Token | 检查 TokenKind、AXAML token resource、Token 类型、生成数据和 token.md和文档同步。 |
| Gallery | 走查对应 ShowCase 示例和源码片段入口。 |
