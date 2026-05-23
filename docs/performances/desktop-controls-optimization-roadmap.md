# Desktop 控件性能优化任务路线图

本文档基于源码扫描结果（`src/AtomUI.Desktop.Controls`、`AtomUI.Desktop.Controls.ColorPicker`、`AtomUI.Desktop.Controls.DataGrid`），结合主题与代码两层依赖分析，给出按依赖顺序排定的性能优化任务表。

> 配套：
> - 现有控件级基线进度：[`README.md`](README.md)
> - Avalonia 12 成本模型：[`avalonia12-control-library-pitfalls.md`](avalonia12-control-library-pitfalls.md)
> - 优化纪律：`.agents/skills/atomui-control-performance/SKILL.md`

---

## 0. 数据来源与口径

| 维度 | 数据 | 取自 |
| --- | --- | --- |
| 控件清单 | 71 个 Desktop 控件目录 + ColorPicker + DataGrid | `find src/AtomUI.Desktop.Controls -maxdepth 1 -type d` |
| 主题依赖 | 137 条目录间边 | 在 `*.axaml` 中 grep `atom:Xxx`，去掉 token / lang / constants / converters 后映射到拥有该类型的目录 |
| 代码依赖 | 277 条目录间边（粗筛后） | 在 `*.cs` 中匹配类名 token 与全局类→目录映射的交集；含一定假阳性（常见单词 `Empty` / `Form` / `Message` 等会误判，已通过同时观察主题边校正主要结论） |
| 已完成基线 | 52 项 | `docs/performances/README.md` "当前文档" / "总列表" 的 Done 行 |

**结论权重**：主题依赖是优化排序的主依据；代码依赖只用于补充识别"程序化使用"的隐式耦合（例如 ContextMenu、Window、Message 等无主题模板嵌入但代码里直接 new 的依赖）。

---

## 1. 控件清单（按包分组）

### 1.1 `AtomUI.Desktop.Controls`（71 个目录 / 712 个公开类型）

每行附三段信息：
- **状态**：Done（README 已收录基线 + 优化结果）/ Partial（仅修了正确性或部分阶段）/ Pending（未建立基线）
- **TFan-In**：主题层被引用次数（被几个其他控件实例化）
- **TFan-Out**：主题层引用次数（实例化几个其他控件）

| 控件 | 状态 | TFan-In | TFan-Out | 备注 |
| --- | --- | --- | --- | --- |
| AdornerLayer | Pending | 0 | 0 | 与 Drawer/Dialog/Tooltip 的 overlay 体系强相关，单独基线意义有限 |
| Alert | Done (MarqueeLabel) | 0 | 2 | Alert 自身随 MarqueeLabel 已优化 |
| AutoComplete | Done | 1 | 2 | Done |
| Avatar | Done | 0 | 1 | Done |
| Badge | Done | 1 | 0 | Done |
| Breadcrumb | Done (correctness + baseline) | 0 | 0 | separator 传播正确性修复 + 基线；Gallery cold / P95 / alloc 有收益，repeated median 基本持平 |
| ButtonSpinner | Done | 2 | 2 | Done |
| Buttons | Done | 34 | 0 | 基础控件，已完成 Button/IconButton/SplitButton 等子控件优化 |
| Calendar | Done | 1 | 2 | 按当前 DisplayMode 延迟填充 MonthView/YearView cells；详见 [Calendar](Calendar/README.md) |
| Card | Done | 1 | 2 | Done |
| Carousel | Done | 0 | 1 | Done |
| Cascader | Done | 0 | 8 | Done — 也是依赖最多的控件之一 |
| CheckBox | Done | 5 | 1 | Done |
| Collapse | Done | 1 | 1 | Done |
| ComboBox | Done | 3 | 5 | Done |
| DatePicker | Done | 3 | 7 | Done — TimePicker / Calendar / Primitives 链头 |
| Descriptions | Done | 0 | 0 | Done |
| Dialog | Done | 3 | 2 | Done — MessageBox / ImagePreviewer 复用 |
| Drawer | Done | 0 | 3 | Done |
| Empty | Done | 35 | 1 | Done — **复用面积最大的占位控件** |
| Expander | Done | 0 | 1 | Done |
| FloatButton | Done | 0 | 4 | Done |
| Flyouts | Done | 9 | 2 | Done（含 MenuFlyout/TreeViewFlyout/PopupConfirm 共用底） |
| Form | Done | 16 | 2 | Done — 表单控件统一壳，影响所有 Input 系 |
| GroupBox | Done | 0 | 1 | Done |
| HeaderedContentControl | Pending | 0 | 0 | 抽象壳，无独立基线必要 |
| ImagePreviewer | Done | 1 | 3 | Done |
| Input | Done | 13 | 4 | Done — LineEdit/TextArea/SearchEdit 父系 |
| ListBox | Done | 4 | 3 | Done（基线在 Cascader/Select 优化里覆盖） |
| ListView | Done | 2 | 4 | Done |
| MarqueeLabel | Done | 1 | 0 | Done — Alert 共用 |
| Mentions | Done | 0 | 3 | Done |
| Menu | Done | 7 | 5 | Done |
| Message | Done | 9 | 0 | Done |
| MessageBox | Done | 0 | 2 | Done |
| NavMenu | Done | 1 | 3 | Done |
| Notifications | Done | 1 | 2 | Done |
| NumericUpDown | Done | 0 | 2 | Done |
| OptionButtonGroup | Done | 0 | 1 | Done |
| Pagination | Done | 2 | 2 | Done |
| Popup | Done (structural) | **19** | **0** | 4 条构造器 token binding 迁 ControlTheme（消除 ~76 订阅 + 修复 Tier 1 §7 同优先级碰撞），`ShadowsAwareContainer` 影子 Border 延迟到 `HasBoxShadow`；详见 [Popup](Popup/README.md) |
| PopupConfirm | Done | 0 | 2 | Done |
| Primitives | Mixed | 16 | 4 | 多类汇总目录：AddOnDecoratedBox / CandidateList / InfoPickerInput 等已部分 Done；目录整体未做一次性 audit |
| ProgressBar | Done | 1 | 0 | Done |
| QRCode | Done | 0 | 3 | Done |
| RadioButton | Done | 3 | 1 | Done |
| Rate | Done | 0 | 0 | Done |
| Result | Done | 8 | 0 | Done |
| ScrollViewer | Done (baseline) | 18 | 0 | 仅修了 selector/overlay 正确性，未证明页面级速度提升 |
| Segmented | Done | 0 | 1 | Done |
| Select | Done | 12 | 5 | Done — 大型聚合控件 |
| Separator | Done (structural-only) | 4 | 1 | `AbstractSeparator.Render` Pen 缓存（SKILL Cost Model 强制）；详见 [Separator](Separator/README.md)。Headless bench 解析力 < 0.1 KB |
| Skeleton | Skipped (sub-noise) | 3 | 0 | 形状同 T0.5 Spin，`BuildActiveAnimation` 在 attach 时无条件构造，但 headless bench 不可见；推迟到 Gallery 级实测 |
| Slider | Done | 0 | 1 | Done |
| Space | Done | 11 | 0 | Done |
| Spin | Skipped (sub-noise) | 8 | 0 | `BuildIndicatorAnimation` lazy-build 改动 KB/item 落在变异范围内；Render 路径 (4 × DrawEllipse/frame) 才是真热点，需 Gallery 级实测 |
| SplitView | Pending | 0 | 0 | 简单容器 |
| Splitter | Done | 0 | 1 | hidden collapse icon 不再构造/保留 PathIcon，lazy preview transform 复用；详见 [Splitter](Splitter/README.md) |
| Statistic | Done | 0 | 1 | generated content ownership、CountUp DataContext 清理、TimerStatistic attach-gated timer 生命周期；详见 [Statistic](Statistic/README.md) |
| Steps | Done (structural-only) | 1 | 1 | 修复 `ConfigureItemsPanel` 未清理 `ColumnDefinitions`；详见 [Steps](Steps/README.md) |
| Switch (ToggleSwitch) | **Pending** | 0 | 0 | 单一控件，可独立基线 |
| TabControl | Done | 1 | 1 | 默认 hidden icon/close slot 按需化；`TabControlShowCase` repeated mean `170.75ms -> 157.35ms`；详见 [TabControl](TabControl/README.md) |
| Tag | Done | 10 | 1 | `AbstractTag.SetupDefaultCloseIcon` 门控到 `IsClosable=true`，非 closable Tag 减少 ~2 KB/instance × Gallery 76 实例 ≈ 140 KB；详见 [Tag](Tag/README.md) |
| TextBlock | Done (structural) | 33 | 0 | `HighlightableTextBlock` 段级 Run + `SelectableTextBlock` token binding/Cursor → Theme Setter；详见 [TextBlock](TextBlock/README.md)。微基准待哈纳斯恢复 |
| TimePicker | Done | 1 | 4 | `InfoPickerInput` closed presenter 延迟到首次打开；`TimePickerShowCase` repeated mean `148.79ms -> 138.48ms`；详见 [TimePicker](TimePicker/README.md) |
| Timeline | Done (structural + correctness) | 0 | 1 | `TimelineIndicator.Render` Pen 缓存；pending item 生命周期/default icon 修复；详见 [Timeline](Timeline/README.md) |
| Tooltip | Done (structural-only) | 5 | 1 | `ToolTipService.StartShowTimer` 复用 DispatcherTimer + method-group Tick handler；headless bench 不能触发 Show 流程，real-world hover 场景待 Gallery 实测；详见 [Tooltip](Tooltip/README.md) |
| TopLevel | n/a | 0 | 0 | 占位目录无主题/类 |
| Tour | Done (structural) | 0 | 4 | `TourStepsView` 同生命周期 binding 收敛；详见 [Tour](Tour/README.md)。popup 打开场景待专项 harness |
| Transfer | Done | 0 | 7 | `TargetKeys` lookup 复用 `HashSet`，空 target panel 直接短路；`TransferShowCase` repeated mean `242.72ms -> 222.12ms`；详见 [Transfer](Transfer/README.md) |
| TreeSelect | Done | 1 | 5 | 共享 `SelectHandle` 三套 indicator 收敛到单 presenter；详见 [TreeSelect](TreeSelect/README.md) |
| TreeView | Done | 4 | 5 | `NodeSwitcherButton` 5 个 icon presenter 收敛到 1 个；详见 [TreeView](TreeView/README.md) |
| Upload | Baseline only | 0 | 4 | 已建立 baseline，3 个候选无收益/退化已回滚；详见 [Upload](Upload/README.md) |
| Window | Demoted to Tier 2 | 13 | 1 | Fan-in 13 是派生类计数，非实例化次数。实测 Gallery 一次会话 ≤ 1 常驻 + 5-10 dialog/drawer，未通过 SKILL Tier 1 §13 资格门槛。降级到 [T2.8](#tier-2--单功能-pending-控件) 与 macOS Metal jitter 协同时再处理。 |
| WindowTitleBar | Pending | 3 | 1 | Dialog / ImagePreviewer / Window 复用 |

### 1.2 `AtomUI.Desktop.Controls.ColorPicker`

| 控件 | 状态 | 跨包依赖 |
| --- | --- | --- |
| ColorPicker | Done (structural lifecycle) | Primitives.ArrowDecoratedBox, Collapse.CollapseItem, ComboBox, ComboBoxItem, Input.LineEdit, NumericUpDown, Popup；closed `Window.Deactivated` 订阅 `23 -> 0`，详见 [ColorPicker](ColorPicker/README.md) |
| GradientColorPicker | Partial | 共享 `AbstractColorPicker` lifecycle 收益；打开态 Gradient 子控件仍待后续专项 |
| ColorSlider / ColorPickerPalette / ColorBlock | Pending | 内部子控件 |

### 1.3 `AtomUI.Desktop.Controls.DataGrid`

| 控件 | 状态 | 跨包依赖 |
| --- | --- | --- |
| DataGrid | Pending | Primitives.ArrowDecoratedBox, Buttons.Button, TextBlock, Pagination, PopupConfirm, ScrollViewer, Spin, Tooltip.ToolTip, TreeView.TreeViewItemTheme |
| DataGridRow / DataGridCell / DataGridColumnHeader 等 ~40 内部子控件 | Pending | 同上 |

DataGrid 是最重的复合控件，独立子表才是合理的优化粒度。

---

## 2. 依赖中枢分析

### 2.1 Top Fan-In（被复用最多 → 优化收益向上传递）

| 排名 | 控件 | 总 Fan-In（主题+代码） | 主题 Fan-In | 现状 |
| --- | --- | --- | --- | --- |
| 1 | Empty | 35 | 8 | Done |
| 2 | Buttons | 34 | 16 | Done |
| 3 | **TextBlock** | **33** | **20** | **Pending — 影响 33 个控件** |
| 4 | **Popup** | **19** | **15** | **Pending — 影响 19 个控件** |
| 5 | ScrollViewer | 18 | 11 | 仅基线 |
| 6 | Primitives | 16 | 7 | 部分 |
| 7 | Form | 16 | 1 | Done |
| 8 | **Window** | **13** | **2** | **Pending — Dialog/Drawer/Notifications 等都依赖** |
| 9 | Input | 13 | 5 | Done |
| 10 | Select | 12 | 2 | Done |
| 11 | Space | 11 | 0 | Done |
| 12 | **Tag** | **10** | **0** | **Pending — Dialog/Select/Transfer/Tour/Upload 复用** |
| 13 | Message | 9 | 0 | Done |
| 14 | Flyouts | 9 | 0 | Done |
| 15 | **Spin** | **8** | **5** | **Pending** |
| 16 | Result | 8 | 0 | Done |

### 2.2 Top Fan-Out（消耗依赖最多 → 内部聚合控件，需要先做底层）

| 控件 | Fan-Out | 现状 | 主要依赖 |
| --- | --- | --- | --- |
| Cascader | 16 | Done | CheckBox, Empty, Form, Input, ListBox, Message, Popup, Primitives, Select, TextBlock, TreeSelect, TreeView |
| Select | 15 | Done | Buttons, Empty, Form, Input, ListView, Message, Popup, Primitives, Result, ScrollViewer, Space, Spin, Tag, Window |
| TreeView | 13 | Done | CheckBox, Empty, Flyouts, Form, Menu, Message, NavMenu, Popup, RadioButton, Result, Spin |
| Transfer | 13 | Done | Buttons, CheckBox, Empty, Flyouts, Input, ListView, Menu, Pagination, Select, Tag, TreeView |
| Primitives | 13 | 部分 | Buttons, Empty, Flyouts, Form, Input, ListBox, Popup, ScrollViewer, Select, Space, TextBlock, Window |
| AutoComplete | 12 | Done | ComboBox, Empty, Form, Input, Menu, Message, Popup, Primitives, Result, Select, Space, Window |
| Upload | 10 | Baseline only | Buttons, Empty, Form, ImagePreviewer, Result, Select, Tag, TextBlock |
| Mentions | 10 | Done | AutoComplete, Empty, Form, Input, Message, Popup, Primitives, Result, Window |
| DatePicker | 10 | Done | Buttons, Calendar, Empty, Form, Popup, Primitives, Space, TimePicker |
| ComboBox | 10 | Done | ButtonSpinner, Buttons, Empty, Form, Input, Popup, Primitives, Window |
| TreeSelect | 9 | Done | Buttons, CheckBox, Form, Input, Popup, Primitives, Select, TreeView |
| TimePicker | 9 | Done | Buttons, DatePicker, Empty, Form, ListBox, Primitives, Tag, TextBlock |

### 2.3 主题循环 / 跨层互引

- **Primitives ↔ DatePicker**：Primitives 引用 `DualMonthArrowDecoratedBox`（DatePicker 自有），DatePicker 反过来引用 Primitives 的 `AddOnDecoratedBox / InfoPickerInput`。优化时 DatePicker 与 Primitives 必须放在同一个 PR 周期。
- **Primitives ↔ Input**：Primitives 引用 `TextBox`（Input 自有），Input 引用 Primitives 的 `AddOnDecoratedBox`。已是 Done 控件但需在后续 audit 时注意。
- **TreeSelect ↔ TreeView**：TreeSelect 同时引用 TreeView 自身类型与 TreeView 主题的 `TreeViewItemTheme`，意味着两者必须协调优化。
- **Cascader → TreeSelect**：Cascader 主题里引用了 `TreeSelectAddOnDecoratedBox`（实际通过派生），优化 TreeSelect 时要先回归 Cascader。

---

## 3. 优化任务表（按 Tier 排序）

### Tier 0 — 必须优先的高 Fan-In Pending 项

每一项都被 ≥ 8 个上层控件依赖，先做这些可以让后续控件优化的"免费收益"最大化。每行附 SKILL Tier 1 / 子系统提示。

| # | 控件 | 包 | Fan-In | 关键问题假设 | 优先级 | SKILL 提示 |
| --- | --- | --- | --- | --- | --- | --- |
| T0.1 | **TextBlock** | Desktop.Controls | 33 | `HighlightableTextBlock`、`HyperLinkTextBlock`、`SelectableTextBlock` 派生在不同场景下被反复挂在表单/列表/Tag/Tooltip 上；潜在热点：Highlight 范围 selector、HyperLink hover transition、SelectableText 的多余 SelectionAdorner 创建 | **Done (structural)** | 子系统：Property + Render；详见 [TextBlock](TextBlock/README.md)。微基准待哈纳斯恢复 |
| T0.2 | **Popup** | Desktop.Controls | 19 | window-host vs overlay-host 决策；motion 与 IsOpen 重入；嵌套 popup 边界（参见 SKILL [Re-entrancy](Re-entrancy)） | **Done (structural)** | 子系统：Popup + Binding；详见 [Popup](Popup/README.md)。微基准待哈纳斯恢复 |
| T0.3 | ~~**Window**~~ Demoted | Desktop.Controls | 13 | Fan-in 13 是派生类计数（DialogHost / ImagePreviewerDialog / Drawer host 等都 `: Window`），不是实例化次数。Gallery 实测 1 个常驻 `WorkspaceWindow`，dialog/drawer 操作 < 1/session 触发。 | **Skipped (Tier 1 §13)** | 转 [T2.8 WindowTitleBar](#tier-2--单功能-pending-控件) 同周期，并与 macOS Metal jitter 项（memory `project_metal_resize_jitter`）协同时回到此项 |
| T0.4 | ~~**Tag**~~ | Desktop.Controls | 10 | 每条 Select 的标签都创建 `IconButton` + `IconPresenter`；移除 / 输入框 hover 切换重 | **Done** | 子系统：Property + Render；详见 [Tag](Tag/README.md) |
| T0.5 | ~~**Spin**~~ | Desktop.Controls | 8 | 旋转动画 + token brush；ListView/Mentions/QRCode/TreeView 都嵌入 | **Skipped (sub-noise)** | Headless bench 下 `BuildIndicatorAnimation` lazy-build 改动 KB/item 落在变异范围内，未达 SKILL Tier 1 §9 量级。Render() 每帧 4 × DrawEllipse 才是真热点，需要 Gallery 级"持续 spinning"场景实测；推迟到 Tier 0 收尾后视情况回来 |
| T0.6 | ~~**Tooltip**~~ | Desktop.Controls | 5 | Form / Slider / Steps / Upload / DataGrid 都用 ToolTipService 全局订阅；hover 触发率高 | **Done (structural-only)** | `ToolTipService.StartShowTimer` 复用 DispatcherTimer；详见 [Tooltip](Tooltip/README.md)。Headless bench 无法触发 Show 流程 |
| T0.7 | ~~Skeleton~~ | Desktop.Controls | 3 | Card / Dialog / Statistic 切换 IsLoading；shimmer 动画始终活跃 | **Skipped (sub-noise)** | 与 T0.5 Spin 同样的 `BuildActiveAnimation` 在 attach 时无条件构造 Animation+3 KeyFrame+3 Setter，但 headless bench 解析力 < 0.1 KB/instance 看不出来。Render 路径 (`SkeletonElement` shimmer) 是真热点，需 Gallery 级"持续 IsActive=true"实测，推迟到 Tier 0 收尾后视情况回来 |
| T0.8 | ~~Separator~~ | Desktop.Controls | 4 | Drawer / Menu / Breadcrumb / NavMenu 内嵌；本身渲染廉价但 Children 计数大 | **Done (structural-only)** | `AbstractSeparator.Render` 加 Pen 缓存（SKILL Cost Model 强制）；详见 [Separator](Separator/README.md)。Headless bench < 解析力 |

### Tier 1 — 复合 Pending 控件（Fan-Out ≥ 4 且 Pending）

这些控件本身是"消费者"，优先级取决于 Tier 0 进度 — 可以在 Tier 0 做完后批量做。

| # | 控件 | Fan-Out | 主要依赖 | 现状 | SKILL 提示 |
| --- | --- | --- | --- | --- | --- |
| T1.1 | TreeView | 13 | CheckBox, Empty, Flyouts, Form, Menu, Message, NavMenu, Popup, RadioButton, Result, Spin | **Done** | Templates / visual tree：`NodeSwitcherButton` 单 presenter 化；`TreeViewShowCase` repeated mean `332.44ms -> 213.41ms`，visuals `2108 -> 1646`；详见 [TreeView](TreeView/README.md) |
| T1.2 | Transfer | 13 | Buttons, CheckBox, Empty, Flyouts, Input, ListView, Menu, Pagination, Select, Tag, TreeView | **Done** | `TargetKeys` lookup 复用 `HashSet`，空 target panel 直接短路；`TransferShowCase` repeated mean `242.72ms -> 222.12ms`，visuals/logical 不变；详见 [Transfer](Transfer/README.md) |
| T1.3 | TreeSelect | 9 | Buttons, CheckBox, Form, Input, Popup, Primitives, Select, TreeView | **Done** | 共享 `SelectHandle` 单 presenter 化；`TreeSelectShowCase` repeated mean `124.72ms -> 108.95ms`，visuals `909 -> 858`；详见 [TreeSelect](TreeSelect/README.md) |
| T1.4 | TimePicker | 9 | Buttons, DatePicker, Empty, Form, ListBox, Primitives, Tag, TextBlock | **Done** | `InfoPickerInput` lazy presenter；`TimePickerShowCase` cold mean `464.82ms -> 393.11ms`，repeated mean `148.79ms -> 138.48ms`；详见 [TimePicker](TimePicker/README.md) |
| T1.5 | Upload | 8 | Buttons, Empty, Form, ImagePreviewer, Result, Select, Tag, TextBlock | Baseline only | 已建立 baseline，3 个候选无收益/退化已回滚；运行时代码未保留 |
| T1.6 | Tour | 7 | Buttons, Dialog, Empty, Popup, Primitives, Steps, Tag | Done (structural) | `TourStepsView` 同生命周期 `RelayBind` 改 direct binding；页面加载小幅改善，popup 打开待专项 harness |
| T1.7 | TabControl | 5 | Buttons, Card, Flyouts, Menu, ScrollViewer | **Done** | Templates / visual tree：默认 tab item hidden icon/close slot 按需化；`TabControlShowCase` visuals `1583 -> 1405`、repeated mean `170.75ms -> 157.35ms`；详见 [TabControl](TabControl/README.md) |

### Tier 2 — 单功能 Pending 控件

| # | 控件 | Fan-Out | 现状 | 备注 |
| --- | --- | --- | --- | --- |
| T2.1 | Calendar | 7 | **Done** | `Calendar.Default` ms/item `21.765 -> 16.405`，Year/Decade 初始模式约 `48%`；Gallery visuals `201 -> 189`；详见 [Calendar](Calendar/README.md) |
| T2.2 | Statistic | 2 | Done | generated content ownership、CountUp DataContext 清理、TimerStatistic attach-gated timer 生命周期；`StatisticShowCase` repeated alloc `7471.87KB -> 7228.74KB`、visuals `387 -> 384`。Skeleton active 优化尝试因 loading 分配回退 / Theme Static Rule 全部撤回 |
| T2.3 | Timeline | 3 | Done (structural + correctness) | `TimelineIndicator.Render` Pen 缓存；pending item 生命周期/default icon 修复，页面级 timing 未证明收益；详见 [Timeline](Timeline/README.md) |
| T2.4 | Steps | 2 | Done (structural-only) | 修复 Grid definitions 清理错误；页面级 timing 未证明稳定收益 |
| T2.5 | Breadcrumb | 2 | Done (correctness + baseline) | 父级 `Separator` 变化现在同步到 direct/generated inherited items；同参数复测下 cold / P95 / alloc 有收益，repeated median 基本持平 |
| T2.6 | Splitter | 3 | **Done** | hidden collapse icon 不再构造/保留 `PathIcon`，lazy preview transform 复用；`SplitterShowCase` repeated mean `36.91ms -> 34.66ms`，alloc `5770.31KB -> 5659.40KB`；详见 [Splitter](Splitter/README.md) |
| T2.7 | Switch (ToggleSwitch) | 0 | Pending | RectTransition + WaveSpiritDecorator |
| T2.8 | WindowTitleBar | 3 | Pending | 与 Window (T0.3) 同周期 |
| T2.9 | AdornerLayer | 0 | Pending | 与 Drawer/Dialog/Tooltip overlay 协同 |
| T2.10 | SplitView | 0 | Pending | 简单容器 |

### Tier 3 — ColorPicker 包

整个包仍未优化，且依赖 Collapse / ComboBox / Input.LineEdit / NumericUpDown / Popup / Primitives.ArrowDecoratedBox 这些上层控件。

| # | 控件 | 现状 | 备注 |
| --- | --- | --- | --- |
| T3.1 | ColorPicker | Done (structural lifecycle) | closed `Window.Deactivated` 订阅 `23 -> 0`；页面 timing 噪声内，不作为主收益；详见 [ColorPicker](ColorPicker/README.md) |
| T3.2 | GradientColorPicker | Partial | 继承 T3.1 lifecycle 收益；打开态 Gradient 子控件仍待后续专项 |
| T3.3 | ColorSlider / Track / Spectrum / Palette | Pending | 子控件，跟随 T3.1 |

### Tier 4 — DataGrid 包

DataGrid 是单文件最复杂的控件之一（~110 个公开类型）。依赖 Pagination / PopupConfirm / ScrollViewer / Spin / Tooltip / TreeView。

| # | 子模块 | 备注 |
| --- | --- | --- |
| T4.1 | DataGrid 核心（DataGrid / DataGridRow / DataGridCell / Presenter 系列） | 前置：ScrollViewer 真正速度优化、Spin (T0.5)、Tooltip (T0.6)、TreeView (T1.1) |
| T4.2 | 列模型（DataGridColumn / DataGridColumnHeader / FilterFlyout / SortIndicator） | 与 T4.1 协同 |
| T4.3 | 行展开 / 详情（RowExpander / DetailsPresenter） | 与 T4.1 协同 |
| T4.4 | 行重排 / 选择（RowReorder / SelectionColumn） | 拖拽全局订阅 — Lifecycle Pairing 重点 |
| T4.5 | 分组 / 过滤（CollectionView / FilterDescription / GroupDescription） | 数据层成本，不在主题路径上但是真热点 |

---

## 4. 推荐执行顺序

```
Phase A (Tier 0) ─────────────────────────────────────────────
  T0.1 TextBlock      ┐
  T0.2 Popup          │  并行；任意一项独立 PR
  T0.5 Spin           ┘
  T0.4 Tag
  T0.6 Tooltip        ┐  与 T0.4 收尾后开始
  T0.7 Skeleton       ┘

Phase B (Tier 0 Window 系) ────────────────────────────────────
  T0.3 Window — **降级**（实测未过 SKILL Tier 1 §13 资格门槛，详见 Tier 0 表）
  T2.8 WindowTitleBar  +  T2.9 AdornerLayer  与 Window 同周期等 macOS Metal jitter 项触发

Phase C (Tier 1 复合) ────────────────────────────────────────
  T1.1 TreeView       done → T1.3 TreeSelect / T1.2 Transfer done
  T2.1 Calendar       并行
  T1.4 TimePicker     done
  T1.5 Upload         baseline only；3 个候选已回滚
  T2.4 Steps          structural-only done → T1.6 Tour structural done
  T1.7 TabControl     独立

Phase D (Tier 2 单功能) ──────────────────────────────────────
  T2.2 Statistic done → T2.3 Timeline / T2.5 Breadcrumb / T2.6 Splitter done
  T2.7 Switch / T2.10 SplitView

Phase E (Tier 3 ColorPicker) ─────────────────────────────────
  T3.1 lifecycle done → T3.2 / T3.3 打开态子控件专项

Phase F (Tier 4 DataGrid) ────────────────────────────────────
  T4.1 核心 → T4.2 列模型 → T4.3 行展开 → T4.4 行重排 → T4.5 数据层
  前置：T0.5 Spin / T0.6 Tooltip / T1.1 TreeView 全部 done
```

---

## 5. 每项任务进入 SKILL 流程的最低门槛

按 SKILL 已加固的纪律执行：

1. **Tier 1 §13 资格门槛**（[`../.agents/skills/atomui-control-performance/SKILL.md`](../../.agents/skills/atomui-control-performance/SKILL.md)）— 每项进入工作前必须有 Gallery 实例数 + 每会话频次。本表的 Fan-In 仅是上层耦合度，不能替代该资格判定。
2. **First-60-Minutes Investigation Playbook** — Step 1-5 走完后才动代码；输出 4 项产物。
3. **Theme Static Rule + `/template/` `TemplatedParent` 检查** — 任何新 `EnsureXxx`/`ClearXxx` 必须落在 3 类例外或回退。
4. **Re-entrancy & Ignore-Flag Guardrails** — 严禁 `_ignoreXxx` 的私有 bool；改写事件流。
5. **Lifecycle Pairing Checklist** — 写新代码前先写下 create/release 配对。
6. **Pattern Rollout Budget** — 同一 pattern 应用到 4 个控件后必须 audit。本表 Tier 0 多个高 Fan-In 项即对应此循环。

---

## 6. 数据复现命令

本表所用全部数据可由以下命令重现（路径相对仓库根）：

```bash
# 主题依赖（per-control）
cd src/AtomUI.Desktop.Controls
for dir in $(find . -maxdepth 1 -type d | sort); do
  ctrl=${dir#./}
  [ "$ctrl" = "." ] && continue
  case "$ctrl" in GeneratedFiles|Localization|Properties|Utils|Converters) continue ;; esac
  deps=$(find "$dir" -name '*.axaml' -print0 2>/dev/null \
    | xargs -0 grep -h -oE 'atom:[A-Z][A-Za-z0-9]+' 2>/dev/null \
    | sort -u | sed 's/atom://' \
    | grep -vE 'TokenResource$|LangResource$|LangResourceKind$|ThemeConstants$|Converter$' \
    | tr '\n' ',' | sed 's/,$//')
  echo "$ctrl|$deps"
done

# 类→目录映射
find src/AtomUI.Desktop.Controls -name '*.cs' -not -path '*/GeneratedFiles/*' -print0 \
  | xargs -0 grep -hE '^(public |internal )?(sealed |abstract )?(partial )?(class|enum|interface|record|struct) [A-Z][A-Za-z0-9]+' \
  | sed -E 's/.* (class|enum|interface|record|struct) ([A-Z][A-Za-z0-9]+).*/\2/'

# Fan-in / Fan-out 排序
awk -F'\t' '{print $2}' edges.txt | sort | uniq -c | sort -rn  # fan-in
awk -F'\t' '{print $1}' edges.txt | sort | uniq -c | sort -rn  # fan-out
```

---

## 7. 已知数据假阳性 / 边界

- 代码层 grep 把常见单词（`Empty`, `Form`, `Message`, `Result`, `Window`, `Tag` 等）当成类引用，部分边可能假阳性。修正方式：以主题层依赖为准，代码层只在主题层为空时作为补充信号。
- `Primitives` 目录内含 ~30 个共享类型（AddOnDecoratedBox, CandidateList, InfoPickerInput, ListBoxItemTheme 等），它们已在多个控件优化（AddOnDecoratedBox、Cascader、Select）里逐项触及，但 Primitives 目录本身没有独立基线。建议在 Phase A 末尾增加 Primitives audit。
- ColorPicker / DataGrid 包内部子控件之间的依赖未单独枚举（数量大 + 未做基线时不投入），现阶段把整个包视为单 Tier 处理即可。
- Calendar 既被 DatePicker 包含，又被 ColorPicker 间接使用（非主题），视为单独 Tier 2 项。

---

## 8. 后续维护

- 控件数量或目录结构发生变化时，重跑第 6 节脚本即可生成新的 Fan-In / Fan-Out 表，本文档随之更新。
- 每完成一项 Tier 0 / Tier 1 / Tier 2 任务，将其状态从 Pending → Done，并在 [`README.md`](README.md) "总列表" 同步更新。
- 跨包优化（Phase E / F）必须列入 SKILL Pattern Rollout Budget 的 4-控件审计周期。
