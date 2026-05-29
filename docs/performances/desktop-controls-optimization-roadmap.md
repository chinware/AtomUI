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
| 已完成基线 | 58 项 | `docs/performances/README.md` "当前文档" / "总列表" 的 Done 行 |

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
| AdornerLayer | Done (structural lifecycle) | 0 | 0 | `ScopeAwareAdornerLayer` attach/detach membership scan `1 -> 0`，detach target 引用释放；详见 [AdornerLayer](AdornerLayer/README.md) |
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
| PopupConfirm | Done (correctness + lifecycle) | 0 | 2 | 本地 Click handlers `2 -> 0`，隐藏 cancel direct-click side effects `2 -> 0`；静态 cancel/content slot 保留，不主张页面 timing 收益 |
| Primitives | Mixed | 16 | 4 | 多类汇总目录：AddOnDecoratedBox / CandidateList / InfoPickerInput 等已部分 Done；`PickerClearUpButton` 本地 Click handlers `1 -> 0`，隐藏 clear direct-click side effects `1 -> 0`；`AddOnDecoratedBox` ContentFrame pointer handlers `4 -> 0` / instance，Select/TreeSelect/Cascader right-addon template 绑定已修复；详见 [Primitives](Primitives/README.md)；目录整体仍未做一次性 audit |
| ProgressBar | Done | 1 | 0 | Done |
| QRCode | Done | 0 | 3 | Done |
| RadioButton | Done | 3 | 1 | Done |
| Rate | Done | 0 | 0 | Done |
| Result | Done | 8 | 0 | Done |
| ScrollViewer | Done (baseline) | 18 | 0 | 仅修了 selector/overlay 正确性，未证明页面级速度提升 |
| Segmented | Done | 0 | 1 | Done |
| Select | Done | 12 | 5 | Done — 大型聚合控件 |
| Separator | Done (structural-only) | 4 | 1 | `AbstractSeparator.Render` Pen 缓存（SKILL Cost Model 强制）；详见 [Separator](Separator/README.md)。Headless bench 解析力 < 0.1 KB |
| Skeleton | Done (structural lifecycle + correctness) | 3 | 0 | 重复 class handler、inactive animation、paragraph line rebuild/follow 生命周期已修复；详见 [Skeleton](Skeleton/README.md) |
| Slider | Done | 0 | 1 | Done |
| Space | Done | 11 | 0 | Done |
| Spin | Done (structural lifecycle + correctness) | 8 | 0 | hidden `SpinIndicator` animation 延迟到可见时创建；motion 参数变化后重建已 materialized 动画；详见 [Spin](Spin/README.md) |
| SplitView | Done (structural lifecycle) | 0 | 0 | 初始 pane transition 延迟到第一次 runtime open/close；详见 [SplitView](SplitView/README.md) |
| Splitter | Done | 0 | 1 | hidden collapse icon 不再构造/保留 PathIcon，lazy preview transform 复用，动态 children add/remove temp objects `3 -> 1`，panel refresh temp objects `2 -> 0`，resize event sizes temp objects `3 -> 1`；详见 [Splitter](Splitter/README.md) |
| Statistic | Done | 0 | 1 | generated content ownership、CountUp DataContext 清理、TimerStatistic attach-gated timer 生命周期；详见 [Statistic](Statistic/README.md) |
| Steps | Done (structural-only) | 1 | 1 | 修复 `ConfigureItemsPanel` 未清理 `ColumnDefinitions`；详见 [Steps](Steps/README.md) |
| Switch (ToggleSwitch) | Done (interaction structural) | 0 | 0 | `IsChecked` 切换不再触发 measure invalidation；详见 [Switch](Switch/README.md) |
| TabControl | Done (render structural) | 1 | 1 | 默认 hidden icon/close slot 按需化；`TabControlShowCase` repeated mean `170.75ms -> 157.35ms`；tab strip border repeated render `Pen` allocations `1/control -> 0 after first`；详见 [TabControl](TabControl/README.md) |
| Tag | Done | 10 | 1 | `AbstractTag.SetupDefaultCloseIcon` 门控到 `IsClosable=true`，非 closable Tag 减少 ~2 KB/instance × Gallery 76 实例 ≈ 140 KB；详见 [Tag](Tag/README.md) |
| TextBlock | Done (structural) | 33 | 0 | `HighlightableTextBlock` 段级 Run + `SelectableTextBlock` token binding/Cursor → Theme Setter；详见 [TextBlock](TextBlock/README.md)。微基准待哈纳斯恢复 |
| TimePicker | Done | 1 | 4 | `InfoPickerInput` closed presenter 延迟到首次打开；`TimePickerShowCase` repeated mean `148.79ms -> 138.48ms`；详见 [TimePicker](TimePicker/README.md) |
| Timeline | Done (structural + correctness) | 0 | 1 | `TimelineIndicator.Render` Pen 缓存；pending item 生命周期/default icon 修复；详见 [Timeline](Timeline/README.md) |
| Tooltip | Done (structural-only) | 5 | 1 | `ToolTipService.StartShowTimer` 复用 DispatcherTimer + method-group Tick handler；headless bench 不能触发 Show 流程，real-world hover 场景待 Gallery 实测；详见 [Tooltip](Tooltip/README.md) |
| TopLevel | n/a | 0 | 0 | 占位目录无主题/类 |
| Tour | Done (structural) | 0 | 4 | `TourStepsView` 同生命周期 binding 收敛；详见 [Tour](Tour/README.md)。popup 打开场景待专项 harness |
| Transfer | Done | 0 | 7 | `TargetKeys` lookup 复用 `HashSet`，空 target panel 直接短路；`TreeTransfer` target 递归结果列表 `N -> 1`；`TransferShowCase` repeated mean `242.72ms -> 222.12ms`；详见 [Transfer](Transfer/README.md) |
| TreeSelect | Done | 1 | 5 | 共享 `SelectHandle` 三套 indicator 收敛到单 presenter；详见 [TreeSelect](TreeSelect/README.md) |
| TreeView | Done | 4 | 5 | `NodeSwitcherButton` 5 个 icon presenter 收敛到 1 个；连接线 repeated render `Pen` allocations `1-2 -> 0 after first`，拖拽 indicator render frame `1 -> 0 after first`，checkbox parent aggregation child scans `2 -> 1`；详见 [TreeView](TreeView/README.md) |
| Upload | Baseline only | 0 | 4 | 已建立 baseline，3 个候选无收益/退化已回滚；详见 [Upload](Upload/README.md) |
| Window | Demoted to Tier 2 | 13 | 1 | Fan-in 13 是派生类计数，非实例化次数。实测 Gallery 一次会话 ≤ 1 常驻 + 5-10 dialog/drawer，未通过 SKILL Tier 1 §13 资格门槛。降级到 [T2.8](#tier-2--单功能-pending-控件) 与 macOS Metal jitter 协同时再处理。 |
| WindowTitleBar | Done (structural) | 3 | 1 | `CaptionButton` normal/checked 两套 `IconPresenter` 收敛为单 presenter；详见 [WindowTitleBar](WindowTitleBar/README.md) |

### 1.2 `AtomUI.Desktop.Controls.ColorPicker`

| 控件 | 状态 | 跨包依赖 |
| --- | --- | --- |
| ColorPicker | Done (structural lifecycle) | Primitives.ArrowDecoratedBox, Collapse.CollapseItem, ComboBox, ComboBoxItem, Input.LineEdit, NumericUpDown, Popup；closed `Window.Deactivated` 订阅 `23 -> 0`，详见 [ColorPicker](ColorPicker/README.md) |
| GradientColorPicker | Done (structural) | 共享 `AbstractColorPicker` lifecycle 收益；`GradientColorPickerView` 模板绑定收敛，详见 [ColorPicker](ColorPicker/README.md) |
| ColorSlider / ColorPickerPalette / ColorBlock | Done | `ColorSpectrum` brush 热路径复用、`HsvValue` 预览 / slider thumb brush 复用、`ColorPickerPaletteGroup` 选择事件实例级隔离、透明棋盘背景 brush token 缓存已完成 |

### 1.3 `AtomUI.Desktop.Controls.DataGrid`

| 控件 | 状态 | 跨包依赖 |
| --- | --- | --- |
| DataGrid | Partial | Primitives.ArrowDecoratedBox, Buttons.Button, TextBlock, Pagination, PopupConfirm, ScrollViewer, Spin, Tooltip.ToolTip, TreeView.TreeViewItemTheme；cell header-state binding / filter flyout lifecycle / filter indicator binding / filter materialization allocation cleanup / filter close selected-values allocation cleanup / filter selected-values capacity preallocation + empty selected-values shared list / filter item children lazy allocation / column/group/row/row group header pointer handler cleanup / column header click drag-over cleanup guard / column header resize/reorder drag state release cleanup / plain column header template-apply sort/list cleanup / column header clip cleanup / group header view item clip cleanup / core input handler cleanup / pagination re-template subscription cleanup / rows presenter scroll gesture + clip geometry cleanup / row bottom gridline clip cleanup / row hidden clip cleanup / row group header child clip + transform cleanup / details presenter clip cleanup / cell clip cleanup / GetAllRows struct enumerable cleanup / row group header slot struct enumeration cleanup / clipboard row content visible-column preallocation / clipboard content direct append formatting cleanup / auto-generated column order-list preallocation / star column width adjustment list lazy/preallocation / special column detach lifecycle / column reorder indicator pen cache / column reordering indicator clip cleanup / column drag-over null-target notification dedup / row reorder duplicate check LINQ cleanup / row reorder click no-op drop cleanup / row reorder drag state release cleanup / selected-items empty reset cache cleanup / selected-items index enumeration cleanup / selection-inclusive start-slot enumeration cleanup / selected-slots table copy preallocation / single-selection first-slot lookup cleanup / checkbox edit pointer bounds wait cleanup / details presenter measure registration / row expander details binding / column sort-filter description lookup cleanup / empty filter request object copy cleanup / column clipboard dead field cleanup / column group tree direct add/remove cleanup / DataConnection enumerable cleanup + read-only collection count fast path + read-only list indexed item lookup + read-only list index lookup + bare IEnumerable item-type probe disposal + read-only list item-type probe fast path + IList item-type probe fast path + collection-view `Any()` fast path + editable-attribute/read-only-attribute/display-attribute no-attribute lookup cleanup / CollectionView read-only source copy/reset fast path + read-only list source copy/local-array/filtered refresh fast path + IList source copy/local-array/filtered refresh fast path + collection-changed old-items indexed loop + edit currency duplicate index lookup cleanup + notifying source tracking enumerator lifecycle cleanup + sorted result preallocation / CollectionView paged enumerator direct range cleanup / CollectionView empty paged enumerator shared array / CollectionView group key matching correctness / CollectionView property changed args cache + reset collection changed args cache + Reset ICollection empty-check fast path / group data property changed args cache / PathGroupDescription owner-type cache / PathSortDescription comparer cache + key selector delegate cache / PrepareLocalArray no-filter copy fast path / FilterDescription property type cache + record value reuse / ValidationUtils contains/member-name comparison cleanup 已完成，分组行头模板 part lookup 正确性已修复，详见 [DataGrid](DataGrid/README.md) |
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
| 15 | Spin | 8 | 5 | Done |
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
| T0.5 | ~~**Spin**~~ | Desktop.Controls | 8 | 旋转动画 + token brush；ListView/Mentions/QRCode/TreeView 都嵌入 | **Done (structural lifecycle + correctness)** | hidden `SpinIndicator` animation 延迟到可见时创建；`MotionDuration` / `MotionEasingCurve` 变化后重建已 materialized 动画；非 spinning KB/item `78.8 -> 77.4`，GalleryShape KB/item `575.1 -> 564.2`；详见 [Spin](Spin/README.md)。持续 spinning 的 render 热点仍需专项 Gallery 场景 |
| T0.6 | ~~**Tooltip**~~ | Desktop.Controls | 5 | Form / Slider / Steps / Upload / DataGrid 都用 ToolTipService 全局订阅；hover 触发率高 | **Done (structural-only)** | `ToolTipService.StartShowTimer` 复用 DispatcherTimer；详见 [Tooltip](Tooltip/README.md)。Headless bench 无法触发 Show 流程 |
| T0.7 | ~~Skeleton~~ | Desktop.Controls | 3 | Card / Dialog / Statistic 切换 IsLoading；shimmer 动画始终活跃 | **Done (structural lifecycle + correctness)** | 修复重复 `ContentProperty` class handler、inactive animation 预创建、paragraph line rebuild/follow 生命周期；`Content.NotLoading` logical/root `902.5 -> 4.0`，KB/item `172.3 -> 129.9`；详见 [Skeleton](Skeleton/README.md)。持续 shimmer render 仍需 Gallery 级 active 场景专项 |
| T0.8 | ~~Separator~~ | Desktop.Controls | 4 | Drawer / Menu / Breadcrumb / NavMenu 内嵌；本身渲染廉价但 Children 计数大 | **Done (structural-only)** | `AbstractSeparator.Render` 加 Pen 缓存（SKILL Cost Model 强制）；详见 [Separator](Separator/README.md)。Headless bench < 解析力 |

### Tier 1 — 复合 Pending 控件（Fan-Out ≥ 4 且 Pending）

这些控件本身是"消费者"，优先级取决于 Tier 0 进度 — 可以在 Tier 0 做完后批量做。

| # | 控件 | Fan-Out | 主要依赖 | 现状 | SKILL 提示 |
| --- | --- | --- | --- | --- | --- |
| T1.1 | TreeView | 13 | CheckBox, Empty, Flyouts, Form, Menu, Message, NavMenu, Popup, RadioButton, Result, Spin | **Done** | Templates / visual tree：`NodeSwitcherButton` 单 presenter 化；`TreeViewShowCase` repeated mean `332.44ms -> 213.41ms`，visuals `2108 -> 1646`；连接线和拖拽 indicator render pen 复用；checkbox parent aggregation child scans `2 -> 1`；详见 [TreeView](TreeView/README.md) |
| T1.2 | Transfer | 13 | Buttons, CheckBox, Empty, Flyouts, Input, ListView, Menu, Pagination, Select, Tag, TreeView | **Done** | `TargetKeys` lookup 复用 `HashSet`，空 target panel 直接短路；`TreeTransfer` target 递归结果列表 `N -> 1`；`TransferShowCase` repeated mean `242.72ms -> 222.12ms`，visuals/logical 不变；详见 [Transfer](Transfer/README.md) |
| T1.3 | TreeSelect | 9 | Buttons, CheckBox, Form, Input, Popup, Primitives, Select, TreeView | **Done** | 共享 `SelectHandle` 单 presenter 化；`TreeSelectShowCase` repeated mean `124.72ms -> 108.95ms`，visuals `909 -> 858`；详见 [TreeSelect](TreeSelect/README.md) |
| T1.4 | TimePicker | 9 | Buttons, DatePicker, Empty, Form, ListBox, Primitives, Tag, TextBlock | **Done** | `InfoPickerInput` lazy presenter；`TimePickerShowCase` cold mean `464.82ms -> 393.11ms`，repeated mean `148.79ms -> 138.48ms`；详见 [TimePicker](TimePicker/README.md) |
| T1.5 | Upload | 8 | Buttons, Empty, Form, ImagePreviewer, Result, Select, Tag, TextBlock | Baseline only | 已建立 baseline，3 个候选无收益/退化已回滚；运行时代码未保留 |
| T1.6 | Tour | 7 | Buttons, Dialog, Empty, Popup, Primitives, Steps, Tag | Done (structural) | `TourStepsView` 同生命周期 `RelayBind` 改 direct binding；页面加载小幅改善，popup 打开待专项 harness |
| T1.7 | TabControl | 5 | Buttons, Card, Flyouts, Menu, ScrollViewer | **Done** | Templates / visual tree：默认 tab item hidden icon/close slot 按需化；`TabControlShowCase` visuals `1583 -> 1405`、repeated mean `170.75ms -> 157.35ms`；tab strip border render pen 复用；详见 [TabControl](TabControl/README.md) |

### Tier 2 — 单功能 Pending 控件

| # | 控件 | Fan-Out | 现状 | 备注 |
| --- | --- | --- | --- | --- |
| T2.1 | Calendar | 7 | **Done** | `Calendar.Default` ms/item `21.765 -> 16.405`，Year/Decade 初始模式约 `48%`；Gallery visuals `201 -> 189`；详见 [Calendar](Calendar/README.md) |
| T2.2 | Statistic | 2 | Done | generated content ownership、CountUp DataContext 清理、TimerStatistic attach-gated timer 生命周期；`StatisticShowCase` repeated alloc `7471.87KB -> 7228.74KB`、visuals `387 -> 384`。Skeleton active 优化尝试因 loading 分配回退 / Theme Static Rule 全部撤回 |
| T2.3 | Timeline | 3 | Done (structural + correctness) | `TimelineIndicator.Render` Pen 缓存；pending item 生命周期/default icon 修复，页面级 timing 未证明收益；详见 [Timeline](Timeline/README.md) |
| T2.4 | Steps | 2 | Done (structural-only) | 修复 Grid definitions 清理错误；页面级 timing 未证明稳定收益 |
| T2.5 | Breadcrumb | 2 | Done (correctness + baseline) | 父级 `Separator` 变化现在同步到 direct/generated inherited items；同参数复测下 cold / P95 / alloc 有收益，repeated median 基本持平 |
| T2.6 | Splitter | 3 | **Done** | hidden collapse icon 不再构造/保留 `PathIcon`，lazy preview transform 复用，动态 children add/remove temp objects `3 -> 1`；`SplitterShowCase` repeated mean `36.91ms -> 34.66ms`，alloc `5770.31KB -> 5659.40KB`；详见 [Splitter](Splitter/README.md) |
| T2.7 | Switch (ToggleSwitch) | 0 | Done (interaction structural) | `IsChecked` 切换 measure invalidations `1000 -> 0`，页面加载中性/噪声内；详见 [Switch](Switch/README.md) |
| T2.8 | WindowTitleBar | 3 | Done (structural) | `CaptionButton` 单 presenter 化；Windows 默认标题栏 visual/root `40 -> 27`，`IconPresenter/root 10 -> 5`；详见 [WindowTitleBar](WindowTitleBar/README.md) |
| T2.9 | AdornerLayer | 0 | Done (structural lifecycle) | `ScopeAwareAdornerLayer` attach/detach 不再扫描 children，detach 后清 adorned target 引用；详见 [AdornerLayer](AdornerLayer/README.md) |
| T2.10 | SplitView | 0 | Done (structural lifecycle) | 初始 pane transition 延迟到第一次 runtime open/close；详见 [SplitView](SplitView/README.md) |
| T2.11 | Watermark | 0 | Done (correctness + lifecycle) | `Glyph` 连续替换不再堆叠重复 Watermark，`Glyph = null` 可可靠清理；pending 4 次替换 visual/root `12 -> 5`、KB/item `244.1 -> 73.0`；详见 [Watermark](Watermark/README.md) |

### Tier 3 — ColorPicker 包

包级主控件和打开态子控件均已完成本阶段优化；依赖 Collapse / ComboBox / Input.LineEdit / NumericUpDown / Popup / Primitives.ArrowDecoratedBox 这些上层控件。

| # | 控件 | 现状 | 备注 |
| --- | --- | --- | --- |
| T3.1 | ColorPicker | Done (structural lifecycle) | closed `Window.Deactivated` 订阅 `23 -> 0`；页面 timing 噪声内，不作为主收益；详见 [ColorPicker](ColorPicker/README.md) |
| T3.2 | GradientColorPicker | Done (structural) | `GradientColorPickerView` 3 个 TemplatedParent binding 已收敛为 `TemplateBinding`；继承 T3.1 lifecycle 收益，详见 [ColorPicker](ColorPicker/README.md) |
| T3.3 | ColorSlider / Track / Spectrum / Palette / ColorBlock | Done | `ColorSpectrum` base/overlay brush refs / 1000 updates `1000/1000 -> 1/1`；preview/thumb `SolidColorBrush` allocations per `HsvValue` update `3 -> 0`，full `HsvColorUpdate` allocation `24386.6 -> 21434.6 bytes/update`；`ColorPickerPaletteGroup` global class handler `1/group -> 0/group`，并修复跨 group 选择误通知；透明背景同 token brush refs / 1000 builds `1000 -> 1`，allocation `10569.1 -> 0.2 bytes/update` |

### Tier 4 — DataGrid 包

DataGrid 是单文件最复杂的控件之一（~110 个公开类型）。依赖 Pagination / PopupConfirm / ScrollViewer / Spin / Tooltip / TreeView。

| # | 子模块 | 备注 |
| --- | --- | --- |
| T4.1 | DataGrid 核心（DataGrid / DataGridRow / DataGridCell / Presenter 系列） | Done：core input handler、pagination 生命周期、rows presenter scroll/clip/layout、row/cell/header clip、realized-row traversal、scrolling-element traversal、column coordinate/width/lifecycle traversal 均已收敛；本轮审计确认剩余 Children foreach 基于 AvaloniaList struct enumerator，不保留无收益改动，剩余无生产调用 yield helper 已审计关闭。 |
| T4.2 | 列模型（DataGridColumn / DataGridColumnHeader / FilterFlyout / SortIndicator） | Done：filter flyout lazy lifecycle、filter request/selected-values 分配、column header handler/drag lifecycle、resize/reorder 状态释放、column visible/displayed traversal、column group tree/layout traversal 均已收敛；剩余 resize/reorder 通过现有验证覆盖，未发现待改全局订阅。 |
| T4.3 | 行展开 / 详情（RowExpander / DetailsPresenter） | Done：special column detach lifecycle、RowExpander binding lifecycle、operation buttons class handler、DetailsPresenter measure registration/clip reuse 已完成；本轮审计确认 DetailsPresenter Children foreach 不是 heap iterator 热点。 |
| T4.4 | 行重排 / 选择（RowReorder / SelectionColumn） | Done：row/column reorder drag indicator 和 drag state lifecycle、row reorder no-op drop、selection delta list/cache、selected-slot traversal、single-selection first slot、checkbox edit bounds subscription 已完成；本轮审计确认 selection-inclusive 当前无生产调用点，内部 slot traversal 已是 struct enumerable。 |
| T4.5 | 分组 / 过滤（CollectionView / FilterDescription / GroupDescription） | Done：DataConnection、CollectionView、GroupDescription、Sort/FilterDescription、TypeHelper、ValidationUtils 相关 Count/indexer、cache、event args、LINQ/enumerator 和 correctness 项已完成；本轮审计确认剩余数据层路径无新的主题内待办。 |

T4.5 补充：CollectionView representative item lookup 在普通非分组 / 非分页 / 非 `AddNew` view 上已改走 indexed lookup，不再为首个非空代表项创建内部列表枚举器；分组、分页和新增事务保留原枚举器语义，详见 [DataGrid](DataGrid/README.md) 2.113。

T4.5 补充：CollectionView 0/1 项 sorted refresh 保留 sort description 初始化，但跳过 `OrderBy` / `ThenBy` 链和额外 sorted-list materialization；2 项及以上仍保持排序语义，详见 [DataGrid](DataGrid/README.md) 2.114。

T4.5 补充：CollectionView empty sorted insert 保留 sort description 初始化，但跳过 `MergedComparer` 构造和 comparer array；non-empty sorted insert 仍保持排序语义，详见 [DataGrid](DataGrid/README.md) 2.115。

T4.5 补充：PathSortDescription 非 string 默认 comparer 已按 `Type` 缓存，重复同类型 lookup 复用同一 comparer；string/culture 路径不共享，详见 [DataGrid](DataGrid/README.md) 2.116。

T4.5 补充：TypeHelper simple property path split 已增加 fast path，无 nested / indexer / parenthesis 的单段属性路径直接返回容量为 1 的 segment list；复杂路径语义不变，详见 [DataGrid](DataGrid/README.md) 2.117。

T4.5 补充：ValidationUtils `ContainsMemberName()` 已为 read-only member-name list 增加 Count/indexer fast path，命中 / 未命中不再创建 enumerator；empty list 只读 Count，详见 [DataGrid](DataGrid/README.md) 2.118。

T4.5 补充：CollectionView 处理 Remove / Replace 的 `NotifyCollectionChangedEventArgs.OldItems` 已改走 Count/indexer，不再为批量 old-items 创建 enumerator；remove 顺序和结果已验证，详见 [DataGrid](DataGrid/README.md) 2.119。

T4.5 补充：DataConnection 处理数据源 Remove 的 `NotifyCollectionChangedEventArgs.OldItems` 已改走 Count/indexer，不再为批量 removed rows 创建 enumerator；真实 DataGrid row slot 删除语义已验证，详见 [DataGrid](DataGrid/README.md) 2.120。

T4.5 补充：CollectionView multi-key grouping 的 `IList` key 集合新增 / 删除 subgroup 已改走 Count/indexer，不再为 key list 创建 enumerator；multi-key subgroup 结果和删除语义已验证，详见 [DataGrid](DataGrid/README.md) 2.121。

T4.5 补充：ValidationUtils `FindEqualValidationResult()` 已为 read-only validation result list 增加 Count/indexer fast path，命中 / 未命中不再创建 collection enumerator；旧 result identity 和 missing 语义已验证，详见 [DataGrid](DataGrid/README.md) 2.122。

T4.5 补充：ValidationUtils `AddExceptionIfNew()` 已为 read-only exception list 增加 Count/indexer fast path，重复 message / 新 message 追加都不再创建 collection enumerator；message 去重语义已验证，详见 [DataGrid](DataGrid/README.md) 2.123。

T4.5 补充：TypeHelper interface fallback 已改查当前 inherited interface，避免重复查询原 interface；DataGrid 接口 ItemsSource 的父接口 writable property 不再误判 read-only，详见 [DataGrid](DataGrid/README.md) 2.124。

T4.5 补充：DataGridFilterDescription 无 `PropertyPath` 时直接过滤 record 本身，不再解析 / 缓存 property type；默认字符串过滤和 custom filter 输入语义已验证，详见 [DataGrid](DataGrid/README.md) 2.125。

T4.5 补充：CollectionView multi-key grouping 的 `IReadOnlyList<object>` key 集合新增 / 删除 subgroup 已走 Count/indexer，两个 read-only key subgroup 和删除后 empty view 语义已验证，详见 [DataGrid](DataGrid/README.md) 2.126。

T4.5 补充：TypeHelper int indexer 带空白解析已从 `string.Trim()` 改为 span trim，避免临时 trimmed string；`"[ 1 ]"` / `"[2]"` int indexer、string indexer 原始文本和非 indexer 默认成员跳过语义已验证，详见 [DataGrid](DataGrid/README.md) 2.127。

T4.5 补充：TypeHelper default member lookup 的无 attribute 常见路径已先走 `IsDefined()`，避免创建空 `DefaultMemberAttribute` array；无默认成员、带默认成员、非 indexer path 和 null item 语义已验证，详见 [DataGrid](DataGrid/README.md) 2.128。

T4.2 补充：Column group collection changed / tree build / group header measure / arrange traversal 已改走 Count/indexer，Remove / Add 各减少 50% snapshot pass，并消除 group traversal enumerator callsites；nested group header 和 leaf order 已验证，详见 [DataGrid](DataGrid/README.md) 2.129。

T4.5 补充：DataGridDataConnection `IndexOf()` 已统一用 `Equals(dataItemTmp, dataItem)`，修复 null entry 错配任意非 null 目标和 null item 无法定位的问题；`IReadOnlyList<object>` 仍走 Count/indexer 且不枚举，详见 [DataGrid](DataGrid/README.md) 2.129。

T4.2 补充：filter flyout presenter 未生成容器时的 selected-values item fallback 已改为 `ItemsView[index]`，不再通过 `foreach (Items)` 线性扫描；未 realized / realized menu-tree presenter 语义已验证，详见 [DataGrid](DataGrid/README.md) 2.94。

T4.2 补充：filter flyout presenter reset / menu radio clear traversal 已复用 `ItemsView[index]` fallback，未生成容器的 nested checked leaf 也会被 reset 清空，详见 [DataGrid](DataGrid/README.md) 2.95。

---

## 4. 推荐执行顺序

```
Phase A (Tier 0) ─────────────────────────────────────────────
  T0.1 TextBlock      ┐
  T0.2 Popup          │  并行；任意一项独立 PR
  T0.5 Spin           ┘
  T0.4 Tag
  T0.6 Tooltip done → T0.7 Skeleton done

Phase B (Tier 0 Window 系) ────────────────────────────────────
  T0.3 Window — **降级**（实测未过 SKILL Tier 1 §13 资格门槛，详见 Tier 0 表）
  T2.8 WindowTitleBar done；T2.9 AdornerLayer structural done

Phase C (Tier 1 复合) ────────────────────────────────────────
  T1.1 TreeView       done → T1.3 TreeSelect / T1.2 Transfer done
  T2.1 Calendar       并行
  T1.4 TimePicker     done
  T1.5 Upload         baseline only；3 个候选已回滚
  T2.4 Steps          structural-only done → T1.6 Tour structural done
  T1.7 TabControl     独立

Phase D (Tier 2 单功能) ──────────────────────────────────────
  T2.2 Statistic done → T2.3 Timeline / T2.5 Breadcrumb / T2.6 Splitter done
  T2.7 Switch done → T2.8 WindowTitleBar structural done → T2.9 AdornerLayer structural done → T2.10 SplitView done → T2.11 Watermark done

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
