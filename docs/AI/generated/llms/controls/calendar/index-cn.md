# Calendar

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

`Calendar` 是按日期组织业务展示内容的桌面日历控件，遵循本专题定义的月面板、年面板、Header、范围限制、禁用规则、周序号和单元格定制语义。它同时保留桌面端可用的焦点与方向键导航。`LunarCalendar` 继承这些稳定语义，并在同一公历状态模型上增加中国农历、二十四节气、传统节日、周末以及应用提供的节假日/调休投影。

Calendar 只负责“查看并选择一个日期或月份”的面板体验，不负责日期输入弹层、范围选择、多日期选择、时间编辑或复杂日程排布。日期输入由 DatePicker 等控件承担，范围和日程数据由业务层承担；Calendar 只提供轻量 `RangeBars` 标记能力，用于在日期网格中表达连续日期业务条。新 Calendar 的内部 `CalendarView` 与 DatePicker 的旧 CalendarView 子系统完全隔离。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Calendar`
- `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar` |
| 状态 | Stable |

## 何时使用

Calendar 的设计语言围绕日期面板的产品语义、可观察状态和主题契约组织，而不是围绕某个模板节点组织。

| 维度 | 含义 | Calendar 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | 以 Month/Year 两种面板展示日期或月份，并提交单一选中值。 |
| 内容承载 | 业务数据和模板如何进入控件。 | `Value`、`ValidRange`、`DisabledDate`、`CellTemplate`、`FullCellTemplate` 与 `HeaderTemplate`。 |
| 状态反馈 | API、内部状态与伪类如何形成反馈。 | `today`、`selected`、`outside`、`disabled`、`focused`、`fullscreen`、`mini`、`show-week`。 |
| 主题语义 | SharedToken、`CalendarToken`、`LunarCalendarToken`、ControlTheme 与模板如何表达视觉。 | Calendar 基础主题消费九个 Calendar 专属 Token；LunarCalendar 只增加农历内容所需的增量 Token。 |

设计上的首要不变量是：Cell 定制不能夺走日期值、选中、禁用、焦点和命中测试语义；这些语义由 Cell 容器保留，模板只改变内容呈现方式。

## 公共 API

Calendar 的公共契约由 Avalonia 属性、事件、模板、上下文类型、枚举、伪类和 ControlTheme 共同组成。

### 3.1 属性

| 属性 | 默认值 | 语义 |
| --- | --- | --- |
| `Value` | 构造时的 `DateTime.Today` | 当前选中日期和面板锚点；写入和提交均规范化到 `.Date`。 |
| `Mode` | `CalendarMode.Month` | `Month` 显示 6×7 日期网格；`Year` 显示 3×4 月份网格。 |
| `Fullscreen` | `true` | 完整布局或紧凑 Mini 布局；不改变日期算法和选择语义。 |
| `ShowWeek` | `false` | Month 日期面板是否显示周序号列。 |
| `ValidRange` | `null` | 首尾包含的日期范围；范围外日期禁用。 |
| `DisabledDate` | `null` | 业务禁用谓词；与 `ValidRange` 共同决定 Cell 是否可用。 |
| `CellTemplate` | `null` | 替换默认值下方的业务内容区域，但保留默认日期/月值和 Cell 状态。 |
| `FullCellTemplate` | `null` | 替换 Cell 的完整内部内容；优先于 `CellTemplate`。 |
| `HeaderTemplate` | `null` | 自定义 Header；为空时使用默认 Year/Month/Mode Header。 |
| `RangeBars` | empty | 声明连续日期范围条；仅在 Fullscreen Month 日期网格 overlay 中渲染。 |

`ValidRange` 使用 `CalendarDateRange(start, end)`，两端均包含，构造函数把时间规范化到日期并拒绝 `end < start`。`CalendarCellContext` 提供 `Value`、`Today`、`CellType`、`DisplayValue`、`IsToday`、`IsInView`、`IsSelected` 和 `IsDisabled`。`CalendarHeaderContext` 提供当前 `Value`/`Mode` 以及提交值和模式的命令。

`CalendarRangeBar` 提供 `StartDate`、`EndDate`、`Label`、`Background` 和 `Height`。日期端点按 `.Date` 投影，首尾包含；`Background` 支持普通 brush、binding、DynamicResource 和 TokenResource。范围条模型、分段算法和资源生命周期见 [Calendar 范围条设计](range-bar-design.md)。

`Value` 与 `Mode` 的默认 Avalonia binding mode 均为 `TwoWay`，用户通过 Cell 或 Header 提交的新状态可以写回绑定源。

### 3.2 事件

| 事件 | 语义 |
| --- | --- |
| `PanelChanged` | 用户选择跨自然月/自然年，或用户切换 Month/Year 模式时触发。 |
| `ValueChanged` | 用户提交后日期值实际变化时触发；程序直接设置 `Value` 不触发。 |
| `Selected` | 每次有效用户选择触发，即使选择的日期与当前 `Value` 相同。 |

用户选择的提交顺序固定为 `PanelChanged` → `ValueChanged` → `Selected`，不存在的事件从序列中省略。事件发生时新值已经写入 `Value`。程序直接设置 `Value` 或 `Mode` 只更新属性和渲染，不模拟用户事件。

`CalendarSelectSource` 用于区分 `Year`、`Month`、`Date` 和 `Customize` 来源。周序号 Cell 可选择；激活后以该行周首日作为日期值，并使用 `Date` 来源。

### 3.3 模板、伪类与主题契约

稳定的根模板协作入口为 `PART_HeaderPresenter`、`PART_BodyPresenter`、`PART_CalendarView`、`PART_RangeBarPanel`、`PART_DefaultHeader` 和 `PART_CustomHeader`；默认 Header 内部使用 `PART_YearSelect`、`PART_MonthSelect`、`PART_ModeSwitch`，View 内部使用 `PART_WeekHeader` 和 `PART_CellHost`，Cell 内部使用 `PART_Item`、`PART_CellInner`、`PART_ItemContent` 和 `PART_Value`。

根伪类包括 `:fullscreen`、`:mini`、`:month`、`:year`、`:show-week`；Cell 伪类包括 `:date`、`:month`、`:week`、`:today`、`:selected`、`:outside`、`:disabled`、`:focused`。四个内部 ControlTheme 的 key 与伪类是主题兼容契约，变更必须同步源码、Gallery 和文档。

### 3.4 LunarCalendar 扩展契约

`LunarCalendar` 是 `Calendar` 的公开派生控件，继续使用 `Value`、`Mode`、`Fullscreen`、`ShowWeek`、`ValidRange`、`DisabledDate`、三个模板入口、`RangeBars` 和三个选择事件。它增加 `ShowSolarTerms`、`ShowTraditionalFestivals`、`ShowHolidays`、`HighlightWeekends`、`HolidayProvider`、只读 `SelectedLunarDateInfo` 和 `RefreshHolidayData()`。

农历算法保证范围为 `1900-01-01` 至 `2100-12-31`。`Value` 在 LunarCalendar 上收敛到该范围；内部有效选择范围为支持范围与 `ValidRange` 的交集。法定节假日和调休不内置，由 `ILunarCalendarHolidayProvider` 以同步面板数据提供。完整模型、默认值、枚举、Provider 规则和显示优先级见 [LunarCalendar 农历能力设计](lunar-calendar-design.md)。

## 事件与命令

Calendar 的公共契约由 Avalonia 属性、事件、模板、上下文类型、枚举、伪类和 ControlTheme 共同组成。
`ValidRange` 使用 `CalendarDateRange(start, end)`，两端均包含，构造函数把时间规范化到日期并拒绝 `end < start`。`CalendarCellContext` 提供 `Value`、`Today`、`CellType`、`DisplayValue`、`IsToday`、`IsInView`、`IsSelected` 和 `IsDisabled`。`CalendarHeaderContext` 提供当前 `Value`/`Mode` 以及提交值和模式的命令。
### 3.2 事件
| 事件 | 语义 |
用户选择的提交顺序固定为 `PanelChanged` → `ValueChanged` → `Selected`，不存在的事件从序列中省略。事件发生时新值已经写入 `Value`。程序直接设置 `Value` 或 `Mode` 只更新属性和渲染，不模拟用户事件。
`LunarCalendar` 是 `Calendar` 的公开派生控件，继续使用 `Value`、`Mode`、`Fullscreen`、`ShowWeek`、`ValidRange`、`DisabledDate`、三个模板入口、`RangeBars` 和三个选择事件。它增加 `ShowSolarTerms`、`ShowTraditionalFestivals`、`ShowHolidays`、`HighlightWeekends`、`HolidayProvider`、只读 `SelectedLunarDateInfo` 和 `RefreshHolidayData()`。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### {gallery:CalendarShowCaseLangResource BasicTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml:44`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Calendar Value="{Binding SampleDate}"
```

### {gallery:CalendarShowCaseLangResource CardTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml:201`

Gallery key：`ExamplesContent` / item `3`

```axaml
<Border Width="300"
        HorizontalAlignment="Left"
        BorderBrush="{atom:SharedTokenResource ColorBorderSecondary}"
        BorderThickness="{atom:SharedTokenResource BorderThickness}"
        CornerRadius="{atom:SharedTokenResource BorderRadiusLG}">
    <atom:Calendar Value="{Binding SampleDate}" Fullscreen="False" />
</Border>
```

### {gallery:CalendarShowCaseLangResource LunarCalendarTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml:217`

Gallery key：`ExamplesContent` / item `4`

```axaml
<atom:LunarCalendar Value="{Binding LunarCalendarSampleDate}"
```

### {gallery:CalendarShowCaseLangResource LunarCalendarCardTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml:234`

Gallery key：`ExamplesContent` / item `5`

```axaml
<Border MinWidth="300"
        HorizontalAlignment="Left"
        BorderBrush="{atom:SharedTokenResource ColorBorderSecondary}"
        BorderThickness="{atom:SharedTokenResource BorderThickness}"
        CornerRadius="{atom:SharedTokenResource BorderRadiusLG}">
    <atom:LunarCalendar Value="{Binding LunarCalendarSampleDate}"
                         Fullscreen="False"
                         ShowSolarTerms="True"
                         ShowTraditionalFestivals="True"
                         ShowHolidays="True"
                         HighlightWeekends="False" />
</Border>
```

## 状态模型

状态流按单一 owner 收敛：

```text
Public API / Header / Cell input
  -> Calendar（Value、Mode、事件顺序）
  -> CalendarHeader + CalendarView（不可变投影）
  -> CalendarViewCell（状态、模板、命中测试）
  -> ControlTheme selector / Gallery 可观察行为
```

- `Calendar` 是唯一业务状态 owner；`CalendarView` 不保存第二份公开选中值。
- Month 模式按当前月生成 42 个日期 Cell；Year 模式按当前年生成 12 个月 Cell。
- `ShowWeek` 只作用于 Month 日期网格，增加每行一个周序号 Cell。周序号 Cell 可被点击并提交该行周首日，但不作为 `CalendarCellContext` 的日期/月模板项。
- 日期禁用由 `ValidRange` 与 `DisabledDate` 的并集决定；月份禁用按“月份首日和末日都在范围外”或业务规则判定，不能只检查当前日。
- 方向键只移动面板内的 roving focus；应跳过禁用 Cell 和周序号 Cell，无合法目标时保留当前焦点。Enter/Space 才提交选择。
- `Fullscreen`/Mini 只改变布局密度和 Header 控件尺寸，不改变值、事件顺序、禁用和模板优先级。
- `RangeBars` 只改变日期网格上方的 overlay 业务标记层，不改变 Cell 外间距、选择状态、禁用状态、鼠标指针、事件顺序或 Automation。
- AtomUI 语言服务改变会同步更新日期格式、周标题、月份名称、Header 的 Month/Year 文本与年份后缀。
- LunarCalendar 不建立第二份农历选中值；`SelectedLunarDateInfo`、Cell 农历内容和 Header 农历标签都从当前公历 Value/面板数据单向投影。
- Fullscreen/Card × Month/Year 四种组合共享 CalendarView 的网格拓扑、焦点、容器池和 Automation，农历 adapter 只改变专用 Cell、Header 文案和布局 metrics。

## 主题与 Design Token

Calendar 使用源码中的 `CalendarToken` 以及 SharedToken。专属 Token 只表达九个组件视觉语义：`FullBg`、`FullPanelBg`、`ItemActiveBg`、`YearControlWidth`、`MonthControlWidth`、`YearMonthCellWidth`、`MiniContentHeight`、`FullCellMinHeight`、`RangeBarHeight`。运行时状态通过伪类和 selector 表达，不写入 Token。

LunarCalendar 使用独立 exact Control identity 和 `LunarCalendarToken`，只补充双行 Cell、农历次级文本、卡片内容高度、周末/节假日标记、Fullscreen Cell 高度和范围条避让所需语义。LunarCalendar root 不通过深层 selector 修改 CalendarHeader、CalendarView、ComboBox、OptionButtonGroup 或普通 CalendarViewCell 的模板内部。

| Theme 文件 | 稳定职责 |
| --- | --- |
| `CalendarTheme.axaml` | 根背景、Header/CustomHeader 选择、CalendarView 与范围条 overlay 接线；Fullscreen 拉伸，Mini 提供无外框的卡片内容布局，外部容器负责边框与宽度。 |
| `CalendarHeaderTheme.axaml` | Year Select、Month Select、Month/Year `OptionButtonGroup` 模式切换。Mini 时 Header 交互控件应使用 Small 尺寸。 |
| `CalendarViewTheme.axaml` | WeekHeader、CellHost，以及 Fullscreen/Mini 的布局差异。 |
| `CalendarViewCellTheme.axaml` | 默认日期值、Cell/FullCell 模板消费、状态 selector 和命中测试视觉。 |

`CellTemplate` 必须保留默认值显示，并与内置范围条 overlay 共存；`FullCellTemplate` 覆盖完整 Cell 内部内容且优先级最高，但不替换 Calendar body overlay。两者都不能删除禁用、选中、焦点和 outside 的容器状态。

普通 Calendar 在 `Fullscreen=false` 时使用 256 高内容区，包含 WeekHeader 与六行 CellHost；LunarCalendar 由自身 `MiniContentHeight` 按双行 Cell 尺寸和共享间距派生有效内容高度。body 的顶部分隔线和纵向 Padding 位于该内容区之外。Mini 的 selected/today/disabled 状态分别使用主色实心、主色单线描边和禁用背景；Fullscreen selected 保持 `ItemActiveBg` 与主色日期值，不复用 Mini 的实心主色规则。

Token 来源：

Calendar 的 Token 收敛为九个组件视觉语义。日期值、周标题、范围条间距、范围条圆角、Padding、Border、Typography 与 Motion 均从 SharedToken 派生；Fullscreen 单元最小高度通过 `FullCellMinHeight` 固化 Calendar 完整单元的测量规则，Year 模式月份单元宽度通过 `YearMonthCellWidth` 固化面板单元测量规则，范围条默认高度通过 `RangeBarHeight` 固化 Calendar overlay 的默认条高。

当前 Calendar Token 源：

- `CalendarToken`，源码位于 `src/AtomUI.Desktop.Controls/Calendar/CalendarToken.cs`。
- AXAML 通过生成的 `CalendarTokenResource` 访问这些值。

LunarCalendar 使用独立 exact Control identity 和 `LunarCalendarToken`。它只补充农历双行内容、卡片内容高度、周末/节假日状态以及 Fullscreen RangeBars 避让所需语义，不复制 Calendar 的根背景、Header、普通 Cell 选中态或 RangeBars 默认条高。

## AOT 与裁剪注意事项

- 日期/月/周计算在 model 失效时完成，不在 Measure/Arrange 热路径重复执行。
- `DisabledDate` 对同一次 model 构建的每个候选值最多调用一次；异常不得被静默吞掉。
- Container pool 只复用无业务所有权的视觉容器；模板、Context、Focus 和 Automation 必须随 Bind/Unbind 完整更新。
- RangeBars 集合使用 owner-managed 非 Visual `AvaloniaObject` 范式；`CalendarRangeBar.Background` 的动态资源和 TokenResource 由 generated scoped resource host 承载，Calendar 负责 attach/release。
- `CalendarRangeBarPanel` 不遍历 Cell visual tree，不在 pointer move 热路径中计算，也不拥有业务数据生命周期。
- Token 通过 `CalendarTokenResource`、`LunarCalendarTokenResource` 和 SharedToken 进入 AXAML；运行时状态由伪类 selector 表达。
- 不使用运行时反射扫描 API、Token、日期类型或 Gallery 数据；属性静态注册、强类型上下文和生成资源保持 NativeAOT 兼容。
- LanguageManager、VisualTree、Template part 等外部订阅必须有成对释放路径，避免 detach 后保留 Calendar。
- 农历年表、二十四节气表和传统节日 resolver 是静态只读数据与纯函数；不依赖第三方农历运行库、系统时区、网络、反射或字符串 binding。面板数据有界且不在 Measure/Arrange/Render 热路径构建。

## 源码索引

```text
src/AtomUI.Desktop.Controls/Calendar/
├── Calendar.cs
├── CalendarCellContext.cs
├── CalendarDateRange.cs
├── CalendarEnums.cs
├── CalendarEventArgs.cs
├── CalendarHeaderContext.cs
├── CalendarRangeBar.cs
├── CalendarToken.cs
├── Internal/
│   ├── CalendarHeader.cs
│   ├── CalendarHeaderOptions.cs
│   ├── CalendarPseudoClass.cs
│   ├── CalendarRangeBarPanel.cs
│   ├── CalendarRelayCommand.cs
│   ├── CalendarView.cs
│   ├── CalendarViewAutomationPeer.cs
│   ├── CalendarViewCell.cs
│   ├── CalendarViewCellAutomationPeer.cs
│   ├── CalendarViewCellBuilder.cs
│   ├── CalendarViewCellModel.cs
│   └── CalendarViewMode.cs
├── Localization/
│   ├── CalendarControlLangResourceKind.cs
│   ├── en-US.xlf
│   ├── zh-CN.xlf
│   └── zh-TW.xlf
└── Themes/
    ├── CalendarTheme.axaml(.cs)
    ├── CalendarHeaderTheme.axaml(.cs)
    ├── CalendarViewTheme.axaml(.cs)
    └── CalendarViewCellTheme.axaml(.cs)
```

`CalendarToken.cs` 是当前 Calendar ControlTheme 实际消费的专属 Token 源。LunarCalendar 的公开类型、算法、数据表、internal adapter、专用 Cell、主题和本地化也统一归属本源码目录；稳定职责结构见 [LunarCalendar 农历能力设计](lunar-calendar-design.md)。DatePicker 的日期面板实现位于 DatePicker 模块，不与本目录 CalendarView 或 Token 混用。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/calendar/overview.md`
- 实现文档：`docs/controls/desktop/data-display/calendar/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/calendar/token.md`
- 变更记录：`docs/controls/desktop/data-display/calendar/changelog.md`
- 语义结构：`./semantic-cn.md`
