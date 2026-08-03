# Calendar 行为设计

本文档定义 Calendar 的日期/月面板组合、定制模板、禁用规则、键盘与 Automation 契约。控件定位、公共契约入口见 [Calendar 桌面版架构设计](overview.md)，源码 ownership 与维护边界见 [Calendar 桌面版实现原理](implementation.md)，范围条模型见 [Calendar 范围条设计](range-bar-design.md)，专属 Token 见 [Calendar Token 设计](token.md)。

## 1. 设计定位

Calendar 是按日期组织业务展示内容的桌面日历控件。公开 `Month` 模式显示 6×7 日期面板，公开 `Year` 模式显示 3×4 月份面板；默认 Header 提供年份、月份和模式选择，应用可以通过 Cell、FullCell 和 Header 模板扩展业务内容。

Calendar 的设计规则是：年面板使用 3 列 4 行，日期面板的周序号 Cell 先执行禁用判断，再以行首日期调用选择回调；连续日期业务标记使用内置范围条模型，不依赖应用在 `CellTemplate` 中手写横向连接算法。

该设计覆盖 Calendar 与内部 `CalendarView`、`CalendarHeader`、`CalendarViewCell` 的组合，不覆盖 DatePicker 的输入弹层、范围选择、时间选择或旧 WPF/Avalonia Calendar 体系。CalendarView 与 DatePicker 的 CalendarView 子系统保持独立。

## 2. 设计原则

1. `Calendar` 是 `Value`、`Mode`、公开事件和用户选择提交的唯一 owner。
2. `CalendarView` 是纯面板，只接收状态投影并报告用户意图，不维护第二份公开选中状态。
3. 公开模式与内部面板分离：`Month -> Date`，`Year -> Month`。
4. Cell 定制只替换内容，不绕过外层的禁用、焦点、命中测试、选择和 Automation 语义。
5. 事件先提交状态，再按 `PanelChanged -> ValueChanged -> Selected` 顺序通知调用方。
6. 日期计算使用日期部分；用户选择和属性入口的 `Value` 统一规范化到 `.Date`。
7. Fullscreen 与 Mini 共用同一模型和容器逻辑，只改变主题和密度。

## 3. 专项模型与 Public API

### 3.1 模式与选择来源

```csharp
public enum CalendarMode
{
    Month,
    Year
}

public enum CalendarSelectSource
{
    Year,
    Month,
    Date,
    Customize
}
```

`Calendar.Mode` 默认 `Month`。内部 `CalendarViewMode` 只包含 `Date` 和 `Month`，不进入 Public API。

| 用户入口 | 选择来源 |
| --- | --- |
| 默认 Header Year Select | `Year` |
| 默认 Header Month Select | `Month` |
| 日期 Cell 或周序号 Cell | `Date` |
| 月份 Cell | `Month` |
| HeaderTemplate 的 ChangeValueCommand | `Customize` |

### 3.2 Value 与范围

`Calendar.Value` 默认是实例创建时的 `DateTime.Today`，同时作为当前选中日期和面板锚点。属性设置、Header Context、Cell Model 和公开事件都只观察日期部分；带时间的外部值在属性入口规范化到 `.Date`。该规范化不触发用户事件，也不通过二次 `SetCurrentValue` 破坏已有绑定。

`CalendarDateRange` 是 ValidRange 的首尾包含值对象。构造函数把两端规范化到 `.Date`，`end < start` 抛出 `ArgumentOutOfRangeException`。`ValidRange` 只限制面板可选择范围，不改变外部传入的锚点值。

Calendar 的年月名称、星期标题、周规则和 Calendar 专用文案跟随 AtomUI 全局语言服务。`Value` 与 `Mode` 的默认绑定模式为 `TwoWay`。

### 3.3 Cell 与 Header 定制

`CellTemplate` 替换默认值下方的业务内容区域：默认日期/月值保持显示，模板内容进入值下方的 `itemContent` 区域。`FullCellTemplate` 替换完整内部内容；两者同时存在时 FullCell 优先。周标题和周序号不消费这两类模板。

`RangeBars` 是连续日期业务标记的内置模型。它在 Fullscreen Month 日期网格上方通过独立 overlay 绘制跨日期横条，并与 `CellTemplate`、`FullCellTemplate` 的 Cell 内部内容共存。

`CalendarCellContext` 提供 `Value`、`Today`、`CellType`、`DisplayValue`、`IsToday`、`IsInView`、`IsSelected` 和 `IsDisabled`。

`HeaderTemplate` 为 `null` 时使用默认 Header。非空时，模板的 DataContext 是 `CalendarHeaderContext`，通过 `ChangeValueCommand`（参数 `DateTime`）和 `ChangeModeCommand`（参数 `CalendarMode`）提交用户意图。自定义 Header 命令不自动套用 ValidRange 或 DisabledDate，由模板自身负责约束。

### 3.4 事件

一次有效用户选择先写入 `Value`，然后按以下顺序触发事件：

```text
跨面板边界 -> PanelChanged
日期实际变化 -> ValueChanged
每次有效选择 -> Selected
```

程序直接设置 `Value` 或 `Mode` 只更新属性和渲染，不触发三个用户事件。用户切换 Mode 始终触发一次 `PanelChanged`，不触发 `ValueChanged` 或 `Selected`。

## 4. 变体、平台与状态策略

### 4.1 面板与密度矩阵

| 公开 Mode | 内部面板 | Cell 数量 | ShowWeek | 选择单位 |
| --- | --- | --- | --- | --- |
| Month | Date | 42 日期，ShowWeek 时增加 6 周序号 | 生效 | 日期 |
| Year | Month | 12 月份 | 忽略 | 月份 |

| Fullscreen | Header 控件 | Cell 布局 | 内容区 |
| --- | --- | --- | --- |
| `true` | Middle/default | 日期/月值与业务内容纵向分区 | 可展示业务内容并独立滚动 |
| `false` | Small | 紧凑居中值布局 | 保持模板优先级，不改变定制语义 |

### 4.2 禁用组合

日期 Cell 的有效性是：

```text
outsideValidRange || DisabledDate(date)
```

ValidRange 首尾均可选，DisabledDate 只能增加禁用，不能放宽范围。DisabledDate 抛出的异常继续向调用方传播。

月份 Cell 遵循 MonthPanel 语义：分别用该月月首和月末执行合并后的禁用谓词，只有两端都禁用时才禁用整月。候选选择值仍保留锚点日，并在目标月份截断到月末；因此保留日命中 DisabledDate 不会错误禁用仍有可用日期的月份。

周序号 Cell 使用该行周首日执行合并后的禁用谓词。启用时选择周首日并以 `CalendarSelectSource.Date` 进入普通事件流程；禁用时不提交。周序号不进入日期/月 roving focus 序列，但支持 Pointer 和 Automation 激活。

### 4.3 语言与方向

Calendar 使用 AtomUI 语言服务。语言变化会使 Header 选项、月份名称、星期标题、周序号规则和模式标签失效并重建。语言资源包含 `Month`、`Year`、`YearSuffix` 和 `Week` 四个 Calendar 专用键；RTL 只影响布局方向和视觉对齐，不改变日期计算或事件顺序。

## 5. 架构、文件结构与职责

```text
Calendar
├── CalendarHeader 或 HeaderTemplate
└── BodyPresenter
    ├── CalendarView
    │   ├── WeekHeader
    │   └── CellHost
    │       └── CalendarViewCell × N
    └── CalendarRangeBarPanel
```

- `Calendar`：公开属性、用户事件、状态归一、Header/View 接线和模板生命周期。
- `CalendarHeader`：默认 Year Select、Month Select、模式 `OptionButtonGroup`；只报告 Year/Month/Mode 用户操作。
- `CalendarView`：按输入构建不可变 Cell Model，管理有界容器池、焦点和键盘；只通过 `CellSelected` 报告意图。
- `CalendarRangeBarPanel`：按月份网格和 body bounds 统一排布范围条 overlay；不参与命中测试。
- `CalendarViewCell`：保留外层交互和 Automation，应用 Cell Model，并承载 Cell/FullCell 内容。
- `CalendarViewCellBuilder`：无 Control 依赖的日期、月份和周序号纯算法。
- `CalendarHeaderOptions`：年份选项、月份选项和 ValidRange 边界收敛纯算法。

Calendar 不生成日期网格，不在 Pointer handler 中计算日期，也不复用 DatePicker CalendarView 的选择模型。

## 6. Template、组合与集成契约

### 6.1 稳定语义区域

| 语义区域 | AtomUI 区域 | 责任 |
| --- | --- | --- |
| `root` | Calendar / `PART_Root` | 背景、边框、密度和根状态 |
| `header` | `PART_HeaderPresenter` / CalendarHeader | 年月选项、模式切换或 HeaderTemplate |
| `body` | `PART_BodyPresenter` | CalendarView 与范围条 overlay 的叠放容器 |
| `view` | CalendarView / `PART_Body` | WeekHeader 与 CellHost 容器 |
| `content` | CalendarView / `PART_CellHost` | 日期/月/周序号网格 |
| `item` | CalendarViewCell / `PART_Item` | 命中测试、状态、焦点和 Automation |
| `rangeBar` | `PART_RangeBarPanel` / CalendarRangeBarPanel | Fullscreen Month 日期网格上方的连续范围条 overlay |
| `itemContent` | CalendarViewCell / `PART_ItemContent` | CellTemplate 业务内容 |

### 6.2 Template Part

Calendar 使用 `PART_HeaderPresenter`、`PART_BodyPresenter`、`PART_DefaultHeader`、`PART_CustomHeader`、`PART_CalendarView` 和 `PART_RangeBarPanel`；CalendarView 使用 `PART_Body`、`PART_WeekHeader` 和 `PART_CellHost`；CalendarViewCell 使用 `PART_Item`、`PART_CellInner`、`PART_Value` 和 `PART_ItemContent`。

`CellTemplate` 不隐藏 `PART_Value`，也不替换内置范围条 overlay。`FullCellTemplate` 隐藏默认 inner 结构并显示完整模板，但不替换 `PART_Item` 或 Calendar body overlay。周序号 Cell 使用默认周序号内容，且不把空的 Week Context 传给业务模板。

WeekHeader 使用与 CellHost 相同的 Grid 列定义，ShowWeek 切换时周标题、周序号列和日期列保持对齐。Mini Header 的 ComboBox 和 `OptionButtonGroup` 使用 Small，Fullscreen 使用默认尺寸。

## 7. 核心算法、数据流与生命周期

### 7.1 Date 网格

输入为 `Value.Date`、Today、Culture 的 `FirstDayOfWeek`、ValidRange 和 DisabledDate。算法先取锚点月首，再按周首日回退得到网格起点，连续生成 42 个日期。每个 Cell 计算 `IsInView`、`IsToday`、`IsSelected`、`IsDisabled` 和 `IsFocusable`；日期显示始终为两位数。

### 7.2 Month 网格

按锚点年份生成 12 个月份。候选值保留锚点日并按目标月份最后一天截断；`IsSelected` 只比较年月。月份的禁用检查按月首/月末短路执行，ValidRange 部分相交的月份保持可选。

### 7.3 焦点与选择

进入面板时优先聚焦选中且可用的 Cell，否则聚焦第一个可用 Cell。Date 模式方向键步长为日和周，Month 模式步长为月和三个月。若候选禁用，沿相同步长继续查找当前已生成网格中的下一个可用 Cell，越出网格或 `DateTime` 可表示范围则停止。方向键只改变焦点；Enter、Space、Pointer 和 Automation 激活才进入 Calendar 的选择提交流程。

### 7.4 状态流与释放

Calendar 向 Header 和 View 单向投影状态。Template reapply 前解绑旧 part，View 在清空旧 WeekHeader/CellHost 后对所有池化 Cell 执行 Unbind，释放 owner、model、context、模板引用和焦点状态。Detach 和 owner 替换使用同一释放路径；新 part 接入后回放当前 Value、Mode、Range、Culture、Template 和 FlowDirection。

## 8. 资源、性能与 AOT 边界

- Date 模式最多 42 个日期容器，ShowWeek 增加 6 个周序号容器；Month 模式最多 12 个容器。
- 容器池有界，Mode、Value、语言和模板变化不无限创建容器。
- RangeBars overlay 只在日期网格、范围条输入、bounds 或主题 metrics 变化时计算，不在 pointer move 热路径中计算。
- Date/Week Cell 每个日期每次重建最多调用一次 DisabledDate；Month Cell 每月按月首/月末最多调用两次。
- Cell Model 不在 Measure/Arrange 热路径构建；Fullscreen/Mini 切换只更新主题状态。
- 事件订阅、模板 part、池化容器和语言服务均有对称释放路径。
- Avalonia 属性静态注册，模板关系使用 TemplateBinding、AXAML selector 或强类型 binding；不引入反射日期适配、运行时类型扫描或动态属性发现。
- Automation 使用 Avalonia 12 可实现的 `Table + ISelectionProvider` 与 `ListItem + ISelectionItemProvider`，不依赖不存在的跨平台 Grid Provider。

## 9. 兼容性与定制边界

稳定契约包括 Calendar 的现有公共属性、三个事件、Template Part、九个 CalendarControl Token、根/Cell 伪类和四个 Calendar 语言资源键。FullCellTemplate 不得移除外层交互和 Automation；CellTemplate 不得替换默认日期/月值或内置范围条 overlay。应用负责模板内部业务视觉和业务数据，AtomUI 负责外层选择、禁用、焦点、Automation、主题、范围条投影和生命周期。

旧 `SelectedDate`、`SelectedDates`、`SelectionMode`、`DisplayDate*`、`BlackoutDates`、Decade 面板、Previous/Next Header 和旧 CalendarButton 体系不属于本控件契约。DatePicker 继续使用独立的旧 CalendarToken 和 CalendarView 类型。

## 10. 验证要求

- 纯逻辑：42 日期 Cell 起点与顺序、12 月份候选值、闰年/月末截断、周序号规则、ValidRange 首尾包含、月份月首/月末禁用和周首日禁用。
- 控件行为：Value 日期归一、模式映射、四种选择来源、事件顺序、Header 边界选项、自定义 Header 命令、周序号选择、重复选择和程序设值不触发用户事件。
- 输入与 Automation：禁用 Cell 跳过、焦点伪类、Enter/Space、SelectionProvider、SelectionContainer 和 FullCellTemplate 外层 Peer 保留。
- Template/主题：CellTemplate 保留值、范围条 overlay 共存、FullCell 优先、WeekHeader 列对齐、Fullscreen/Mini 尺寸、本地化标签、Light/Dark 和运行时主题切换。
- 生命周期/性能：Template reapply、Detach、owner 替换、池化容器数量和池中 Cell 可回收性。
- Gallery/LLMS/AOT：Gallery 稳定示例、API/Token 表、范围条示例、LLMS 生成校验、Calendar 定向测试和 NativeAOT Gallery publish。
