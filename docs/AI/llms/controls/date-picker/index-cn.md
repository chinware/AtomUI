# DatePicker

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

DatePicker 是 AtomUI 桌面控件体系中的日期选择控件，用于单日期、周、月份、季度、年份以及日期范围的日历面板选择，并通过可选的包含式日期边界约束面板导航和用户选择。

DatePicker 不负责业务日程系统、时间选择或完整日期时间解析服务。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/DatePicker`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker` |
| 状态 | Stable |

## 何时使用

DatePicker 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | DatePicker 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | DatePicker 是 AtomUI 桌面控件体系中的日期选择控件，用于单日期、日期范围和日历面板选择。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `HeaderBackground`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | DatePicker Token + ControlTheme。 |

## 公共 API

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

## 事件与命令

DatePicker 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。
- 类型：`Calendar`、`CalendarItem`、`ChoosingStatusEventArgs`、`DatePicker`、`DatePickerPresenter`、`DateSelectedEventArgs`、`DualMonthArrowDecoratedBox`、`DualMonthCalendarItem`、`DualMonthRangeCalendar`、`DualMonthRangeDatePickerPresenter`、`RangeCalendar`、`RangeCalendarItem`、`RangeDatePicker`、`RangeDatePickerPresenter` 等 19 项。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml:35`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:DatePicker PickerMode="Date"
                     PlaceholderText="选择日期" />
    <atom:DatePicker PickerMode="Week"
                     PlaceholderText="选择周" />
    <atom:DatePicker PickerMode="Month"
                     PlaceholderText="选择月份" />
    <atom:DatePicker PickerMode="Quarter"
                     PlaceholderText="选择季度" />
    <atom:DatePicker PickerMode="Year"
                     PlaceholderText="选择年份" />
</StackPanel>
```

### 切换不同的选择器

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml:57`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Horizontal" Spacing="12">
    <atom:Select Width="104"
                 Mode="Single"
                 OptionsSource="{Binding PickerTypeOptions}"
                 SelectedOption="{Binding SelectedPickerOption}" />
    <Grid>
        <atom:TimePicker Width="240"
                         PlaceholderText="{Binding SelectedPickerPlaceholderText}"
                         IsVisible="{Binding IsTimePickerVisible}" />
        <atom:DatePicker Width="240"
                         PlaceholderText="{Binding SelectedPickerPlaceholderText}"
                         IsVisible="{Binding IsDatePickerVisible}"
                         PickerMode="{Binding SelectedPickerMode}" />
    </Grid>
</StackPanel>
```

### SelectedDateTime 绑定

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml:83`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Vertical" Spacing="16">
    <StackPanel Orientation="Vertical" Spacing="12">
        <atom:DatePicker Width="240"
                         SelectedDateTime="{Binding BoundSelectedDateTime}"
                         PlaceholderText="选择日期" />
        <StackPanel Orientation="Horizontal" Spacing="8">
            <atom:TextBlock VerticalAlignment="Center"
                            Text="选中值：" />
            <atom:TextBlock VerticalAlignment="Center"
                            Text="{Binding BoundSelectedDateTimeText}" />
        </StackPanel>
        <StackPanel Orientation="Horizontal" Spacing="8">
            <atom:Button SizeType="Small"
                         Click="SetBoundSelectedDateTimeTomorrow"
                         Content="设置为明天" />
            <atom:Button SizeType="Small"
                         Click="ClearBoundSelectedDateTime"
                         Content="清空" />
        </StackPanel>
    </StackPanel>
    <StackPanel Orientation="Vertical" Spacing="12">
        <atom:RangeDatePicker PickerMode="Date"
                              IsShowTime="False"
                              RangeStartSelectedDate="{Binding BoundRangeStartSelectedDate}"
                              RangeEndSelectedDate="{Binding BoundRangeEndSelectedDate}"
                              PlaceholderText="开始日期"
                              SecondaryPlaceholderText="结束日期" />
        <StackPanel Orientation="Horizontal" Spacing="8">
            <atom:TextBlock VerticalAlignment="Center"
                            Text="选中范围：" />
            <atom:TextBlock VerticalAlignment="Center"
                            Text="{Binding BoundRangeSelectedDateText}" />
        </StackPanel>
        <StackPanel Orientation="Horizontal" Spacing="8">
            <atom:Button SizeType="Small"
                         Click="SetBoundSelectedDateRangeThisWeek"
                         Content="设置为本周" />
            <atom:Button SizeType="Small"
                         Click="ClearBoundSelectedDateRange"
                         Content="清空" />
        </StackPanel>
    </StackPanel>
</StackPanel>
```

### 弹出面板显示日期

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml:137`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:DatePicker PickerDisplayDate="2026-10-20"
```

## 状态模型

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
- DatePicker / RangeDatePicker 输入壳体必须把 `DataValidationErrors` 同步转发到外层 AddOn 和内部文本框；range indicator 等附属视觉读取 effective status，native error 优先于手动 warning/error 状态。
- `PickerMode=Week` 的月视图是带周序号列的 8 列 week panel，不是普通日期面板的 7 个日期按钮逐个选中；选中视觉和 hover 视觉都必须按整周连续行渲染，不能退回单个日期按钮的普通 pointerover 背景。
- 非 `Date` 颗粒度仍使用 `DateTime?` 保存提交值：`Week` 保存 ISO 周起始日，`Month` 保存当月 1 日，`Quarter` 保存季度首月 1 日，`Year` 保存当年 1 月 1 日。
- `IsShowTime` 只在 `PickerMode=Date` 时形成有效时间选择；其他颗粒度忽略时间面板和时间拼接。
- 范围选择的 committed 状态和 hover preview 状态必须分开：`:selected`、`:range-start`、`:range-end`、`:range-middle` 只来自真实端点；hover 只写入 `:range-preview-start`、`:range-preview-end`、`:range-preview-middle`，其中 preview start/end 在视觉上按临时端点显示，但不能污染真实提交状态。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

DatePicker 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `CalendarButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `CalendarDayButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `CalendarItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `CalendarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
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

Token 来源：

DatePicker Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `DatePickerToken`，scope id 为 `DatePicker`，源码位于 `src/AtomUI.Desktop.Controls/DatePicker/DatePickerToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。
- 日期边界模型使用静态注册的 Avalonia 属性和 internal 纯值计算，不引入反射、动态类型发现、运行时代码生成或 trimming 注解。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。
- effective range 计算和单值/端点校验保持 O(1)；面板刷新复用现有 `CalendarViewState`、panel model 和 generated button，不为每个 cell 创建日期约束对象或额外订阅。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/DatePicker`：DatePicker 控件家族根目录，代表文件 `DatePicker.cs`、`RangeDatePicker.cs`、`DatePickerPresenter.cs`、`DatePickerFormattingHelper.cs`、`DatePickerDateRangeConstraint.cs`、`DatePickerToken.cs`、`DualMonthRangeDatePickerPresenter.cs` 等。
- `src/AtomUI.Desktop.Controls/DatePicker/CalendarView`：CalendarView runtime。`State` 保存归一化状态和 action，`Models` 保存纯 panel model，`Rendering` 将 model 应用到 generated buttons，`Infrastructure` 封装 culture 和 pointer tracking。
- `src/AtomUI.Desktop.Controls/DatePicker/Localization`：`DatePickerLangResourceKind.cs` 定义稳定 Catalog，`en-US.xlf`、`zh-CN.xlf`、`zh-TW.xlf` 提供内置翻译。
- `src/AtomUI.Desktop.Controls/DatePicker/Themes`：19 个文件，代表文件 `CalendarButtonTheme.axaml`、`CalendarButtonTheme.cs`、`CalendarDayButtonTheme.axaml`、`CalendarItemTheme.axaml`、`CalendarItemTheme.cs` 等。
- `src/AtomUI.Desktop.Controls/Primitives/InfoPickerInput/InfoPickerTextBox.cs` 与 `Themes/InfoPickerTextBoxTheme.axaml`：DatePicker 输入框使用的 internal 子控件及其文本 presenter、padding 基础视觉。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。
- CalendarView 的深度重构必须以 [CalendarView 系统性优化设计](calendar-view-system-optimization.md) 中定义的单向状态模型、panel model、renderer 和生命周期规则为边界。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/date-picker/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/date-picker/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/date-picker/token.md`
- 变更记录：`docs/controls/desktop/data-entry/date-picker/changelog.md`
- 语义结构：`./semantic-cn.md`
