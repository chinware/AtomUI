# Calendar 桌面版实现原理

本文档记录 Calendar 的当前源码 ownership、主题组合、状态流、生命周期、内部算法、资源边界和维护不变量。公共契约见 [Calendar 桌面版架构设计](overview.md)，行为规则见 [Calendar 行为设计](behavior-design.md)，范围条见 [Calendar 范围条设计](range-bar-design.md)，Token 见 [Calendar Token 设计](token.md)，变化记录见 [Calendar Changelog](changelog.md)。

## 1. 实现定位

Calendar 是一个 `TemplatedControl` 根控件，内部组合默认 Header、可选自定义 Header 和纯面板 `CalendarView`。`Calendar` 持有 public API 和事件提交逻辑；`CalendarView` 根据输入构建不可变 Cell model 并管理有限容器池；`CalendarViewCell` 只负责一个 Cell 的状态投影、模板内容和激活转发。具体属性注册、selector 和默认资源仍以源码为准，本文只描述稳定维护边界。

## 2. 源码文件结构

```text
src/AtomUI.Desktop.Controls/Calendar/
├── Calendar.cs
├── CalendarCellContext.cs
├── CalendarControlToken.cs
├── CalendarDateRange.cs
├── CalendarEnums.cs
├── CalendarEventArgs.cs
├── CalendarHeaderContext.cs
├── CalendarRangeBar.cs
├── CalendarToken.cs                 # DatePicker 旧 CalendarView 的遗留 Token，不属于新 Calendar
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
│   ├── en_US.cs
│   ├── zh_CN.cs
│   └── zh_TW.cs
└── Themes/
    ├── CalendarTheme.axaml(.cs)
    ├── CalendarHeaderTheme.axaml(.cs)
    ├── CalendarViewTheme.axaml(.cs)
    └── CalendarViewCellTheme.axaml(.cs)
```

`CalendarToken.cs` 属于 DatePicker 的旧 CalendarView 兼容边界；新 Calendar 的专属 Token 是 `CalendarControlToken.cs`。两者不得在实现或文档中混用。

## 3. 核心类职责

| 类型 | ownership 与稳定职责 |
| --- | --- |
| `Calendar` | Public Avalonia 属性、事件、用户选择提交、Mode 到内部 ViewMode 的映射、语言订阅和根伪类。 |
| `CalendarHeader` | 默认 Year/Month Select 与 Month/Year 模式切换；只报告意图，不拥有 `Value`/`Mode`。 |
| `CalendarHeaderOptions` | 年份/月选项生成、ValidRange 下的年份和月份收敛；无视觉依赖的纯逻辑。 |
| `CalendarRangeBar` | 连续日期范围条的公开描述对象；作为非 Visual `AvaloniaObject` 使用 scoped resource host。 |
| `CalendarRangeBarPanel` | 位于 Calendar body 的只读 overlay panel；根据月份日期网格和自身 bounds 统一排布所有范围条 segment。 |
| `CalendarView` | 根据 Value/Mode/Culture/Range/DisabledDate 构建 Cell model、维护容器池、roving focus、键盘导航和 Table Automation。 |
| `CalendarViewCellBuilder` | 生成日期、月份和周序号 model；集中日期/月边界与禁用算法。 |
| `CalendarViewCellModel` | 单个 Cell 的不可变值、类型、显示文本、状态和可聚焦性。 |
| `CalendarViewCell` | 将 model 绑定到 `CalendarCellContext`、伪类和模板，处理 Pointer/Automation 激活；不承载范围条布局。 |
| `CalendarViewAutomationPeer` | 暴露 `AutomationControlType.Table` 与单选 `ISelectionProvider`。 |
| `CalendarViewCellAutomationPeer` | 暴露 `AutomationControlType.ListItem` 与 `ISelectionItemProvider`；完整本地化名称、选中状态和 SelectionContainer 来自当前 owner/model。 |
| `CalendarControlToken` | 从 SharedToken 派生八个 Calendar 视觉 Token；不保存运行时状态。 |

## 4. 状态与数据流

```text
Calendar.Value/Mode/Range/Culture/RangeBars
  -> CalendarHeader 与 CalendarView 的属性投影
  -> CalendarViewCellBuilder 生成 IReadOnlyList<CalendarViewCellModel>
  -> CalendarViewCell.Bind(owner, model)
  -> CalendarRangeBarPanel 根据同一月份网格和 body bounds 排布范围条 segment
  -> 伪类、模板内容、Automation 与输入
  -> Calendar.CommitUserSelection / CommitModeChange
```

用户 Pointer、键盘或 Automation 激活 Cell 后，View 只发出 `CellSelected`，由 `Calendar.CommitUserSelection` 规范化目标日期、判断面板变化、写入 Value，并按 `PanelChanged -> ValueChanged -> Selected` 顺序通知。默认 Header 的年/月选择复用同一提交路径；模式切换只写 Mode 并触发 `PanelChanged`。

程序直接设置 `Value`/`Mode` 不应伪造用户事件。属性改变后，模板投影、伪类和 Cell model 必须更新；如果模板尚未应用，状态暂存于根控件并在 `OnApplyTemplate` 回放。

## 5. 组合结构模型

实际组合结构来自 `Themes/`：

```text
Calendar (CalendarTheme)
├── PART_HeaderPresenter
│   ├── PART_DefaultHeader (CalendarHeaderTheme)
│   │   ├── PART_YearSelect
│   │   ├── PART_MonthSelect
│   │   └── PART_ModeSwitch
│   └── PART_CustomHeader (ContentControl + HeaderTemplate)
└── PART_BodyPresenter
    ├── PART_CalendarView (CalendarViewTheme)
    │   ├── PART_WeekHeader
    │   └── PART_CellHost
    │       └── CalendarViewCell × 42/48/12
    │           ├── PART_Item
    │           └── PART_CellInner
    │               ├── PART_ItemContent
    │               └── PART_Value
    └── PART_RangeBarPanel (CalendarRangeBarPanel)
```

Date 模式默认生成 42 个日期 Cell；启用 `ShowWeek` 时另加 6 个周序号 Cell。Year 模式生成 12 个月份 Cell。当前主题通过 `CalendarView` 的 `Grid`/`Panel` 组合实现布局，容器池负责有限复用，不把业务数据 owner 交给模板。

## 6. 生命周期与模板接入

- 构造阶段只设置默认 Value，不读取模板 part。
- `Calendar.OnApplyTemplate` 先解除旧 View/Header 的事件订阅，再查找新 part、建立订阅、同步 ViewMode、Header 内容、语言 Culture 和根伪类。
- `CalendarView.OnApplyTemplate` 先解绑旧 WeekHeader/CellHost 和容器关系，再接入新模板并重建/实现 Cell。
- `CalendarViewCell.Bind` 每次复用都覆盖 owner、model、DisplayText、Context、Focusable 和伪类；Unbind 或回收时必须清空 owner/model、模板上下文和状态，避免旧 Cell 继续命中或保留旧订阅。
- Calendar attach 到 visual tree 时订阅 LanguageManager，detach 时解除订阅；语言变化会把全局 Culture 推给 Header/View，并通过 AtomUI 语言资源刷新 Calendar 文案。CalendarView detach 会释放容器，reattach 会从当前属性恢复网格。
- Calendar 对 `RangeBars` 中的每个 `CalendarRangeBar` 建立 resource host attachment 和属性变化订阅；条目移除、集合 reset、集合替换、控件 detach 或 owner 释放时必须成对 dispose，并使 `CalendarRangeBarPanel` 失效。
- 模板重应用、Mode 切换、ShowWeek 切换和容器数量变化不能残留旧容器、旧输入焦点或旧 CellTemplate。

## 7. 交互与事件处理

### 7.1 Header

默认 Header 使用 Year Select、Month Select 和 Month/Year Segmented。ValidRange 限制年份选项；月份选项按年份和范围收敛；跨年时保留可用月份并把日期日收敛到目标月有效天数。Month/Year 显示文本和年份后缀必须来自当前语言资源，Mini 模式的 Header 控件使用 Small 尺寸。

### 7.2 Cell 与模板

`CellTemplate` 的内容上下文是 `CalendarCellContext`，默认 `PART_Value` 仍显示日期值；内置范围条 overlay 在 Calendar body 上层按日期网格绘制，并与 `CellTemplate` 共存。`FullCellTemplate` 直接替代完整 `PART_CellInner` 内容并优先于 `CellTemplate`，但不替换 Calendar body overlay。Week Cell 不产生日期/月上下文，周序号显示由 View 生成；周序号激活以该行首日提交选择。

模板只替换内容，不能绕过容器的 disabled hit-test、selected/today/outside/focused 状态、Automation 或事件提交路径。

### 7.3 键盘与 Automation

View 维护 roving focus：Date 模式左右移动一天、上下移动一周；Month 模式左右移动一个月、上下移动三个月，以匹配 3×4 月份网格。目标超出当前网格、落在禁用 Cell 或落在周序号 Cell 时继续寻找同方向的下一个可聚焦 Cell；没有合法目标则保持原焦点。Enter/Space 激活当前可用日期/月 Cell。

Automation 语义使用当前 Avalonia 可移植契约：View 是 `Table` 并实现单选 `ISelectionProvider`，Cell 是 `ListItem` 并实现 `ISelectionItemProvider`。Cell 返回 View 作为 `SelectionContainer`，日期/月名称使用当前 Culture 的完整格式；不宣称 GridItem provider 或不可移植的 Grid 结构接口。

## 8. 内部算法与关键流程

### 8.1 日期网格

以 `Value.Date` 所在自然月为锚点，根据当前 Culture 的 `FirstDayOfWeek` 计算网格首日，固定生成 42 个连续日期。靠近 `DateTime.MinValue/MaxValue` 时会平移网格起点，避免日期运算溢出并保持 42 个不同日期。每个日期 model 同时保存 `IsInView`、`IsToday`、`IsSelected`、`IsDisabled` 和 `IsFocusable`。`DisabledDate` 对候选日期使用日期值调用，不携带时间部分。

### 8.2 月份网格

以锚定年份生成 1 至 12 月。每月的选择值保留当前日并收敛到该月最后一天；月份是否超出 `ValidRange` 必须同时检查月首和月末，只有整月没有任何有效日期时才禁用。业务禁用策略若不能以整月判定，必须按最终组件契约明确其候选日期语义，不得错误地只检查一个代表日而禁用仍有可用日期的月份。

### 8.3 周序号

从日期网格每行首日按 Culture 的 `CalendarWeekRule` 和 `FirstDayOfWeek` 计算周号。周序号 Cell 的选择值是该行首日；它是可见的辅助 Cell，不进入日期/月 Cell 模板上下文。

### 8.4 失效与容器复用

Value、Today、ViewMode、ShowWeek、ValidRange、DisabledDate 或 Culture 改变时重建 model；CellTemplate/FullCellTemplate 改变时只需重新应用内容。容器池上限由当前模式和 ShowWeek 决定，切换模式、模板重应用和 detach 时都必须回收多余容器并解除 owner 关系。

### 8.5 范围条 overlay 排布

RangeBars 投影以 `PART_RangeBarPanel` 的本地坐标为坐标系。Panel 先用与 `CalendarViewCellBuilder` 等价的月份网格算法得到 42 个可见日期，再过滤缺少端点、端点反向或与可见网格不相交的条目，并按区间重叠分配 lane。每个有效范围按可见周行拆分成横向 segment；segment 的 x、width、y 和 height 由当前 bounds、ShowWeek 列偏移、周标题高度、lane 与条高共同决定。

默认条高来自 `CalendarControlToken.RangeBarHeight`；单条 `CalendarRangeBar.Height` 是实例级覆盖。条间距、圆角和 label padding 从 SharedToken 派生为内部 metrics。FlowDirection 只影响 overlay x 坐标镜像和视觉 inline 圆角，不改变日期顺序或事件语义。

## 9. 资源、性能与 AOT 边界

- 日期/月/周计算在 model 失效时完成，不在 Measure/Arrange 热路径重复执行。
- `DisabledDate` 对同一次 model 构建的每个候选值最多调用一次；异常不得被静默吞掉。
- Container pool 只复用无业务所有权的视觉容器；模板、Context、Focus 和 Automation 必须随 Bind/Unbind 完整更新。
- RangeBars 集合使用 owner-managed 非 Visual `AvaloniaObject` 范式；`CalendarRangeBar.Background` 的动态资源和 TokenResource 由 generated scoped resource host 承载，Calendar 负责 attach/release。
- `CalendarRangeBarPanel` 不遍历 Cell visual tree，不在 pointer move 热路径中计算，也不拥有业务数据生命周期。
- Token 通过 `CalendarControlTokenResource` 和 SharedToken 进入 AXAML；运行时状态由伪类 selector 表达。
- 不使用运行时反射扫描 API、Token、日期类型或 Gallery 数据；属性静态注册、强类型上下文和生成资源保持 NativeAOT 兼容。
- LanguageManager、VisualTree、Template part 等外部订阅必须有成对释放路径，避免 detach 后保留 Calendar。

## 10. 维护不变量

- Calendar 是唯一 public 状态 owner；View/Cell 不得引入第二份可写 Value。
- `Value` 永远是日期值；所有提交和上下文值均不携带时间部分。
- `FullCellTemplate` 优先于 `CellTemplate`，但两者都保留 Cell 状态和交互语义。
- `RangeBars` 只进入 Fullscreen Month 日期网格 overlay 层，不改变 Cell 外间距、Pointer、键盘、Automation 或选择事件顺序。
- Month 禁用使用月首/月末范围判断；方向键跳过禁用和周序号 Cell。
- 默认 Header、自定义 Header、Cell Pointer、键盘和 Automation 使用同一提交与事件顺序。
- 新 Calendar 与 DatePicker 旧 CalendarView 的类型、Token、Theme key 和生命周期互不越界。
- 改动 ControlTheme、伪类、Token、Template part 或 Automation 时，必须同步 Gallery、测试和本目录文档。

## 11. 测试与验证

至少覆盖：

- `Value` 默认值和时间规范化，程序设值不触发用户事件。
- Month/Year 面板、跨月/跨年选择和固定事件顺序。
- ValidRange 首尾包含、Month 两端禁用、DisabledDate 调用次数与异常传播。
- ShowWeek 的周号、周首日选择和 CellTemplate 不遮蔽周序号。
- CellTemplate/FullCellTemplate 优先级、默认日期值保留、HeaderTemplate 命令提交。
- RangeBars 的单日/跨日/跨周 overlay 分段、ShowWeek 列偏移、lane 分配、资源宿主释放、与 CellTemplate/FullCellTemplate 的共存语义。
- 方向键跳过禁用 Cell，Enter/Space 选择，Automation 控件类型和选择状态。
- 模板重应用、Mode/ShowWeek 切换、语言切换、容器回收和 detach 释放。
- Light/Dark、Fullscreen/Mini、Gallery API/Token/ShowCase 和 NativeAOT 生成边界。

纯文档改动运行 `git diff --check` 和相对链接检查；行为或主题改动运行对应 `tests/AtomUI.Desktop.Controls.Tests` 与 Gallery 验证。LLMS 生成文件只由仓库生成/verify 流程更新。
