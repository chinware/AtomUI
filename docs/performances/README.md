# AtomUI 控件性能优化总览

本目录用于沉淀 AtomUI 控件性能优化的基线、方案、关键取舍和最终结果。

测试工具位于 [tools/performances](/Users/chinboy/Projects/dotnet/AtomUIV6/tools/performances/README.md)。

样式 selector 与代码状态机的性能边界见：[style-vs-code-guidelines.md](style-vs-code-guidelines.md)。

Avalonia 12 控件库研发的成本模型与高频踩雷点（含 `path:line` 引用）见：[avalonia12-control-library-pitfalls.md](avalonia12-control-library-pitfalls.md)。

按 Fan-In / Fan-Out 排序的 Desktop 控件优化任务路线图（含 ColorPicker、DataGrid 包）见：[desktop-controls-optimization-roadmap.md](desktop-controls-optimization-roadmap.md)。

## 目录规范

- 每个控件或共享性能体系使用一个独立目录，目录名使用控件名的大小写写法，例如 `AddOnDecoratedBox`、`LineEdit`。
- 控件目录内保留该控件自己的 README、长期复用的方案、基线、最终结果和复现命令；阶段中间记录应合并到 README 或最终结果文档。
- 跨控件共享的基础设施可以单独成目录，例如 `AddOnDecoratedBox`。
- Gallery 级页面实测放到最直接受影响的控件目录下，并在文档内说明是否包含 Gallery 上层成本。

## 入库规则

应进入版本控制：

- 总览、guide、控件目录 README。
- 控件级优化方案、关键设计取舍、优化前基线和最终结果的摘要。
- 对应 ShowCase 加载时间优化对比、Gallery 真实场景的汇总结论和复现命令，统一汇总到控件 README。

不进入版本控制：

- 单独的阶段计划、baseline、final、optimized 等中间 markdown 文件。
- 阶段中间跑分输出。
- 一次性评估草稿。
- 可由工具重新生成的逐次 sample 原始输出。
- `/tmp` worktree 中生成的 baseline 对照结果。

## 测试口径

| 口径 | 工具 | 用途 | 注意事项 |
| --- | --- | --- | --- |
| 控件级基准 | `tools/performances/AtomUI.Performance` | 单控件、小组合、批量实例化、专项行为验证 | 不代表 Gallery 页面打开体验 |
| Gallery 场景复现 | `tools/performances/AtomUI.GalleryPerformance` | 真实 Workspace route、真实 showcase XAML、真实视觉树稳定耗时 | 必须证明源 XAML 和运行时控件形态一致 |

控件存在对应 Gallery ShowCase 时，控件 README 必须单独列出 ShowCase 加载时间优化对比，至少包含 cold first navigation、repeated mean、repeated median、repeated P95，并标明 cold sample count、warmup/iterations。`Cold first navigation` 必须使用独立进程多样本，不能用单样本声明提升。

## 当前文档

| 控件/体系 | 状态 | 目录 | 关键结果 |
| --- | --- | --- | --- |
| AddOnDecoratedBox | 本轮已完成 | [AddOnDecoratedBox](AddOnDecoratedBox/README.md) | 默认 LineEdit visual/root `26 -> 20`，KB/item `503.8 -> 398.4`，默认 icon scan `360 -> 0` |
| Icon | 本轮已完成 | [Icon](Icon/README.md) | `IconShowCase` repeated `193.31ms -> 173.36ms`，alloc `57900.83KB -> 46331.65KB`；generated metadata 与 transform literal 已同步 AntDesign/Material/IconPark；`Select.Default` 默认隐藏 slot 从 `Icon/root 3 -> 1`、`Button/root 1 -> 0` |
| LineEdit | 本轮已完成 Gallery 实测 | [LineEdit](LineEdit/README.md) | `LineEditShowCase` 重复导航均值 `229.07ms -> 183.98ms`，提升 `19.68%` |
| Button | 本轮已完成 | [Button](Button/README.md) | loading/icon/wave 默认 slot 已按需创建；`ButtonShowCase` visuals `1128 -> 964`，IconPresenter `92 -> 51`，repeated mean `134.60ms -> 101.53ms` |
| ButtonSpinner | 本轮已完成 | [ButtonSpinner](ButtonSpinner/README.md) | `HiddenHandle` visuals `25 -> 10`，KB/item `474.4 -> 212.5`；`ButtonSpinnerShowCase` visuals `725 -> 681`，alloc `14900.81KB -> 13514.36KB` |
| Space | 本轮已完成 | [Space](Space/README.md) | `CompactSpaceItem 93 -> 75`，visuals `1864 -> 1846`，`SpaceShowCase` repeated mean `165.32ms -> 150.48ms` |
| AutoComplete | 本轮已完成 | [AutoComplete](AutoComplete/README.md) | 关闭态 `_candidateList 13 -> 0`；`AutoCompleteShowCase` repeated mean `74.97ms -> 55.63ms`，alloc `8143KB -> 7664.37KB` |
| Avatar | 本轮已完成 | [Avatar](Avatar/README.md) | `AvatarShowCase` repeated mean `42.33ms -> 32.54ms`，visuals `360 -> 258`，alloc `7252.55KB -> 5824.01KB` |
| Badge | 本轮已完成 | [Badge](Badge/README.md) | hidden zero `CountBadgeAdorner/root 1 -> 0`，`DotBadge` 无文本 `Label/root 1 -> 0`；`BadgeShowCase` visuals `497 -> 485` |
| Breadcrumb | 本轮已建立基线并修复 separator 传播 | [Breadcrumb](Breadcrumb/README.md) | 父级 `Separator` 变化现在同步到 direct/generated inherited items，local/data separator 继续胜出；`BreadcrumbShowCase` cold mean `92.98ms -> 87.53ms`，repeated P95 `36.72ms -> 34.00ms`，repeated median 基本持平 |
| Card | 本轮已完成 | [Card](Card/README.md) | `CardShowCase` repeated mean `78.11ms -> 50.66ms`，visuals `784 -> 582`；`CardActionPanel 18 -> 3`，`Skeleton 18 -> 0` |
| Carousel | 本轮已完成 | [Carousel](Carousel/README.md) | `CarouselShowCase` alloc `6113.86KB -> 5213.10KB`，visuals `369 -> 326`；nav `14 -> 4`，progress `28 -> 1`，transition `7 -> 0`，repeated mean 基本持平 |
| Cascader | 本轮已完成 | [Cascader](Cascader/README.md) | filter/multiple closed visual `41/25 -> 20`；`CascaderShowCase` visuals `1250 -> 1169`，repeated mean `141.81ms -> 133.27ms` |
| CheckBox | 本轮已完成 | [CheckBox](CheckBox/README.md) | 默认 unchecked `visuals 12 -> 8`，contentless unchecked `11 -> 6`，`CheckBoxShowCase` visuals `420 -> 318`，repeated alloc `6385.60KB -> 4629.80KB` |
| ColorPicker | 本轮已完成结构生命周期优化 | [ColorPicker](ColorPicker/README.md) | closed `Window.Deactivated` 订阅 `23 -> 0`；打开态自动关闭语义保留；`ColorPickerShowCase` visuals/logical `370/49` 不变，timing 噪声内 |
| Collapse | 本轮已完成 | [Collapse](Collapse/README.md) | `CollapseShowCase` repeated mean `119.31ms -> 100.18ms`，visuals `616 -> 553`；content motion `33 -> 1`，addon presenter `33 -> 3` |
| ComboBox | 本轮已完成 | [ComboBox](ComboBox/README.md) | `ComboBoxShowCase` repeated mean `109.35ms -> 91.07ms`，alloc `11384.83KB -> 9534.28KB`，visuals `562 -> 497`；默认 `Button/IconButton 23 -> 0` |
| DatePicker | 本轮已完成 | [DatePicker](DatePicker/README.md) | closed route `PickerHost 30 -> 0`，visuals `1570 -> 1540`，alloc `30959.16KB -> 29762.89KB`，长样本 repeated mean `174.77ms -> 154.21ms` |
| Descriptions | 本轮已完成 | [Descriptions](Descriptions/README.md) | `DescriptionsShowCase` repeated mean `75.84ms -> 68.64ms`，visuals `579 -> 571`；`GalleryShape.Batch8` `25.139ms/root -> 24.486ms/root`；修复 binding/collection/window 订阅生命周期风险 |
| Dialog / MessageBox | 本轮已完成结构与生命周期优化 | [Dialog](Dialog/README.md) | `MessageBox` closed `Dialog/root 1 -> 0`，`ModalShowCase` visuals `324 -> 317`，Dialog runtime `14 -> 7`；修复 RelayBind、mask binding、resizer 和 ButtonBox 父级生命周期风险，后续 MessageBox 细化见独立文档 |
| Expander | 本轮已完成 | [Expander](Expander/README.md) | closed content motion actor 按需创建；`ExpanderShowCase` cold mean `191.70ms -> 170.47ms`，repeated mean `82.18ms -> 67.30ms`，visuals `391 -> 360`，MotionActor `16 -> 1` |
| Empty | 本轮已完成低风险修复 | [Empty](Empty/README.md) | 修复 `IsDescriptionVisible=False`、图片来源运行时互斥和 `Svg.Source/Path` 残留；`EmptyShowCase` repeated mean `21.90ms -> 21.32ms`，alloc `2269.73KB -> 2253.90KB`，visuals 不变 |
| FloatButton | 本轮已完成 | [FloatButton](FloatButton/README.md) | closed trigger group visual/root `41 -> 14`；`FloatButtonShowCase` repeated mean `126.71ms -> 110.81ms`，visuals `949 -> 752`，alloc `14447.92KB -> 12182.94KB` |
| Form | 本轮已完成低风险结构与生命周期优化 | [Form](Form/README.md) | `Form.GalleryShape` `92.094ms/item -> 89.364ms/item`，visual/root `1544 -> 1508`；`FormShowCase` visuals `6435 -> 6300`，alloc 少 `3.54%`，页面 repeated 耗时基本持平 |
| GroupBox | 本轮已完成 | [GroupBox](GroupBox/README.md) | 默认无 `HeaderIcon` 不再创建 header icon presenter；`GroupBox.GalleryShape` visual/root `104 -> 96`，`GroupBoxShowCase` visuals `151 -> 143`，repeated mean `27.79ms -> 26.16ms` |
| ImagePreviewer | 本轮已完成 | [ImagePreviewer](ImagePreviewer/README.md) | 单图关闭态不再提前创建完整预览 source list；`ImagePreviewerShowCase` cold mean `180.28ms -> 147.48ms`，repeated mean `33.56ms -> 30.61ms` |
| Input | 本轮已完成结构优化 | [Input](Input/README.md) | `TextBox.Default` visual/root `21 -> 13`、KB/item `332.6 -> 236.7`；`TextArea.Default` visual/root `24 -> 21`；`LineEditShowCase` visuals `2307 -> 2284`，repeated mean 基本持平 |
| ListView | 本轮已完成 | [ListView](ListView/README.md) | `ListView.Grouped` `3.619ms/item -> 1.759ms/item`，`FilterActive` `3.421ms/item -> 1.841ms/item`；`ListShowCase` repeated mean `80.72ms -> 77.34ms` |
| MarqueeLabel / Alert | 本轮已完成 | [MarqueeLabel](MarqueeLabel/README.md) | Alert 默认路径不再创建隐藏 `MarqueeLabel`；`AlertShowCase` runtime `MarqueeLabel 25 -> 1`，visuals `585 -> 561`，repeated mean `65.42ms -> 54.67ms` |
| Mentions | 本轮已完成 | [Mentions](Mentions/README.md) | closed popup content 已按需创建；`MentionsShowCase` `_candidateList 15 -> 0`，repeated mean `110.05ms -> 106.95ms`，alloc `10115.79KB -> 9326.47KB` |
| Menu | 本轮已完成 | [Menu](Menu/README.md) | closed leaf `MenuItem` visual/root `13 -> 9`，KB/item `209.8 -> 122.1`；`MenuShowCase` Menu-only phase repeated mean `91.16ms -> 89.10ms`，严格 NavMenu shape 见 NavMenu 文档 |
| NavMenu | 本轮已完成 | [NavMenu](NavMenu/README.md) | 修复 prepared node binding 生命周期和默认路径遍历；`MenuShowCase` 严格完整形态 repeated mean `162.76ms -> 99.13ms`，cold mean `416.22ms -> 351.72ms` |
| Message | 本轮已完成 | [Message](Message/README.md) | `WindowMessageManager` 只在首次实际 show 时创建；timer/event/OnClose 生命周期已释放；`MessageShowCase` repeated mean `30.97ms -> 28.52ms`，cold mean `109.07ms -> 98.86ms` |
| MessageBox | 本轮已完成 | [MessageBox](MessageBox/README.md) | 同生命周期 binding 改为 `[!]`，loading skeleton 按需创建并验证释放；`ModalShowCase` repeated mean `34.53ms -> 32.11ms`，P95 `37.18ms -> 34.42ms` |
| Notification | 本轮已完成 | [Notification](Notification/README.md) | manager/card 生命周期补齐，manager/progress bar 按需创建；`NotificationShowCase` cold mean `176.82ms -> 155.20ms`，repeated mean `37.46ms -> 29.13ms` |
| NumericUpDown | 本轮已完成 | [NumericUpDown](NumericUpDown/README.md) | 默认 right accessory 按需创建；`NumberUpDownShowCase` visuals `1351 -> 1269`，Button/IconButton `90 -> 60`，cold mean `480.45ms -> 427.73ms` |
| OptionButtonGroup | 本轮已完成 | [OptionButtonGroup](OptionButtonGroup/README.md) | 文本按钮默认不创建 `IconPresenter/Wave`；`RadioButtonShowCase` visuals `794 -> 689`，repeated mean `100.85ms -> 84.72ms` |
| Pagination | 本轮已完成 | [Pagination](Pagination/README.md) | 页码项 icon/content slot 按需创建；`PaginationShowCase` visuals `1262 -> 1065`，IconPresenter `120 -> 51`，repeated mean `168.92ms -> 131.76ms` |
| PopupConfirm | 本轮已完成 | [PopupConfirm](PopupConfirm/README.md) | popup 打开态 `NoCancel` container `2.865ms/item -> 1.634ms/item`，Button `2 -> 1`；`PopupConfirmShowCase` repeated mean `45.21ms -> 40.42ms` |
| QRCode | 本轮已完成 | [QRCode](QRCode/README.md) | 默认 Active visual/root `19 -> 7`，`QRCodeShowCase` repeated mean `125.12ms -> 91.58ms`，visuals `436 -> 307` |
| RadioButton | 本轮已完成 | [RadioButton](RadioButton/README.md) | `RadioIndicator` wave 按需创建；`RadioButtonShowCase` visuals `689 -> 668`，Wave `22 -> 1`，repeated P95 `184.13ms -> 164.47ms` |
| Rate | 本轮已完成 | [Rate](Rate/README.md) | `Rate.GalleryShape` `15.846ms/item -> 10.354ms/item`；`RateShowCase` repeated mean `62.75ms -> 55.87ms`，visuals `376 -> 331` |
| Result | 本轮已完成 | [Result](Result/README.md) | 普通状态/错误码视觉按需创建；`ResultShowCase` repeated mean `74.01ms -> 67.19ms`，Svg `8 -> 3`，visuals `265 -> 257` |
| ScrollViewer | 本轮已完成基线与低风险正确性修复 | [ScrollViewer](ScrollViewer/README.md) | 修复 overlay host selector 与 motion owner；`FloatButtonShowCase` 页面级 timing 基本中性，后续优化需更强收益证明 |
| Skeleton | 本轮已完成结构生命周期与正确性修复 | [Skeleton](Skeleton/README.md) | 修复重复 class handler 导致的 logical children 异常；inactive animation 延迟创建；`Content.NotLoading` logical/root `902.5 -> 4.0`，KB/item `172.3 -> 129.9` |
| Slider | 本轮已完成 | [Slider](Slider/README.md) | 普通 Slider 不再预创建 `PART_EndThumb`；`SliderShowCase` repeated mean `46.31ms -> 33.78ms`，visuals `111 -> 105` |
| SplitView | 本轮已完成结构生命周期优化 | [SplitView](SplitView/README.md) | 初始 closed/open 不再 materialize pane transition；典型场景 KB/item 约下降 `~1KB`，runtime open/close transition 行为保留 |
| Spin | 本轮已完成结构生命周期与正确性修复 | [Spin](Spin/README.md) | hidden `SpinIndicator` 不再提前构造 animation；非 spinning KB/item `78.8 -> 77.4`，GalleryShape KB/item `575.1 -> 564.2`；motion 参数变化后动画重建 |
| Splitter | 本轮已完成 Gallery 实测 | [Splitter](Splitter/README.md) | hidden collapse icon 不再构造/保留 `PathIcon`，lazy preview transform 复用；`SplitterShowCase` repeated mean `36.91ms -> 34.66ms`，P95 `51.79ms -> 40.43ms`，alloc `5770.31KB -> 5659.40KB` |
| Statistic | 本轮已完成正确性与生命周期优化 | [Statistic](Statistic/README.md) | generated `Content` ownership、`StatisticCountUp.DataContext` 清理、`TimerStatistic` attach-gated timer 释放；`StatisticShowCase` repeated alloc `7471.87KB -> 7228.74KB`，visuals `387 -> 384` |
| Switch | 本轮已完成交互路径结构优化 | [Switch](Switch/README.md) | `IsChecked` 切换不再触发 measure invalidation；1000 次切换 measure invalidations `1000 -> 0`，arrange 保持 `1000`，页面加载指标中性/噪声内 |
| Timeline | 本轮已完成结构与正确性修复 | [Timeline](Timeline/README.md) | `TimelineIndicator.Render` Pen 缓存；修复 pending item 清空和默认/custom icon 显示；页面级 timing 未证明收益，pending spinner 正确显示带来 `+2` visuals |
| TextBlock | 本轮已完成 | [TextBlock](TextBlock/README.md) | `HighlightableTextBlock` 段级 Run 重写：Match.Medium `3.306ms→0.426ms (-87%)`、`355.8KB→44.0KB (-88%)`、`logical 56→4`；`SelectableTextBlock` token binding+Cursor 移到 Theme Setter |
| Popup | 本轮已完成 | [Popup](Popup/README.md) | `Popup` 4 条构造器 token binding 迁到 ControlTheme（fan-in 19 → ~76 条订阅消除 + Tier 1 §7 同优先级碰撞修复）；构造路径 `0.154ms→0.107ms (-31%)`、`23.4KB→21.9KB (-6%)`；`ShadowsAwareContainer` 影子 Border 延迟到 `HasBoxShadow` |
| Tag | 本轮已完成 | [Tag](Tag/README.md) | `AbstractTag.SetupDefaultCloseIcon` 门控到 `IsClosable=true`；非 closable Tag 各 scenario 减少 ~2 KB/instance；Gallery 76 实例 × ~70 非 closable ≈ 140 KB 加载分配下降；closable 路径行为不变 |
| Transfer | 本轮已完成 Gallery 实测 | [Transfer](Transfer/README.md) | `TargetKeys` membership lookup 改为每次刷新复用 `HashSet`，空 target panel 直接短路；`TransferShowCase` repeated mean `242.72ms -> 222.12ms`，cold mean `855.88ms -> 768.42ms`，visuals/logical 不变 |
| TreeView | 本轮已完成 Gallery 实测 | [TreeView](TreeView/README.md) | `NodeSwitcherButton` 模板 5 个图标 presenter 收敛到 1 个；`TreeViewShowCase` visuals `2108 -> 1646`、`IconPresenter 332 -> 112`、repeated mean `332.44ms -> 213.41ms`、alloc `49964.64KB -> 33732.33KB` |
| TreeSelect | 本轮已完成 Gallery 实测 | [TreeSelect](TreeSelect/README.md) | 共享 `SelectHandle` 三套 indicator 收敛到单 presenter；`TreeSelectShowCase` visuals `909 -> 858`、`IconPresenter 38 -> 21`、repeated mean `124.72ms -> 108.95ms`、alloc `19996.64KB -> 18118.79KB` |
| TimePicker | 本轮已完成 Gallery 实测 | [TimePicker](TimePicker/README.md) | `InfoPickerInput` closed presenter 延迟到首次打开；`TimePickerShowCase` cold mean `464.82ms -> 393.11ms`，repeated mean `148.79ms -> 138.48ms` |
| Upload | 本轮已建立 baseline，未保留 runtime 改动 | [Upload](Upload/README.md) | `UploadShowCase` baseline：cold mean `351.77ms`，repeated mean `87.48ms`；3 个候选因 timing 无收益/退化已回滚，仅保留 GalleryPerformance 映射 |
| Steps | 本轮已完成结构正确性修复 | [Steps](Steps/README.md) | `ConfigureItemsPanel` 清理 stale `ColumnDefinitions`；`StepsShowCase` repeated timing 未证明收益，按 structural-only 记录 |
| TabControl | 本轮已完成 Gallery 实测 | [TabControl](TabControl/README.md) | 默认无 icon/非 closable tab 不再创建隐藏 slot；`TabControlShowCase` visuals `1583 -> 1405`，repeated mean `170.75ms -> 157.35ms` |
| Tour | 本轮已完成结构绑定优化 | [Tour](Tour/README.md) | `TourStepsView` 7 条同生命周期 `RelayBind` 改为 template-priority direct binding；`TourShowCase` repeated mean `65.81ms -> 61.91ms`，visuals/logical 不变 |
| Tooltip | 本轮已完成（structural-only） | [Tooltip](Tooltip/README.md) | `ToolTipService.StartShowTimer` 复用 DispatcherTimer + method-group Tick handler，避免每次 hover-show 分配 timer + lambda 闭包；headless bench 不能触发 Show 流程，real-world hover-heavy 场景待 Gallery 实测 |
| Separator | 本轮已完成（structural-only） | [Separator](Separator/README.md) | `AbstractSeparator.Render` Pen 缓存（SKILL Cost Model 强制）；headless bench 解析力 < 0.1 KB/instance 看不出，real-world 60fps render loop 才能放大 |

## 总列表

| 分类 | 控件/体系 | 状态 | 备注 |
| --- | --- | --- | --- |
| Shared Primitive | AddOnDecoratedBox | Done | 输入类控件共享视觉与 addon 体系 |
| Shared Primitive | Icon | Done | Phase 6 已完成；Material/IconPark 外部包已同步 generated metadata 与 transform literal；Provider cache 清理语义已补齐 |
| Shared Primitive | ScrollViewer | Done | [ScrollViewer](ScrollViewer/README.md)；补齐控件级 suite 和真实 `FloatButtonShowCase` 对比，落地 selector/motion 正确性修复，未证明稳定页面级性能提升 |
| General | Button | Done | loading/icon/wave 默认 slot 已按需创建；selector brush 状态保留在 XAML |
| General | FloatButton | Done | [FloatButton](FloatButton/README.md)；closed trigger menu、badge canvas、separator layer 已按需创建，Gallery repeated mean 提升约 `12.55%` |
| General | SplitButton | Pending | 已做 Button 联动 smoke；仍待独立基线 |
| General | Separator | Pending | 待建立基线 |
| General | ButtonSpinner | Done | [ButtonSpinner](ButtonSpinner/README.md)；handle/presenter/outer addon 已按需创建，floatable 全局 input 订阅生命周期已验证 |
| Layout | FlexPanel | Pending | 待建立基线 |
| Layout | Grid | Pending | 待建立基线 |
| Layout | Space | Done | 已完成 Phase 0-7；`CompactSpaceFiller` 不再承担 wrapper，repeated timing 小幅改善，cold 仍需后续针对子控件/Gallery 拆解 |
| Layout | SplitView | Done (structural lifecycle) | [SplitView](SplitView/README.md)；初始 pane transition 延迟到第一次 runtime open/close，典型场景 KB/item 约下降 `~1KB` |
| Layout | Splitter | Done | [Splitter](Splitter/README.md)；hidden collapse icon 不再构造/保留 `PathIcon`，lazy preview transform 复用，`SplitterShowCase` repeated mean 提升约 `6.10%`，P95 提升约 `21.93%` |
| Navigation | Breadcrumb | Done (correctness + baseline) | [Breadcrumb](Breadcrumb/README.md)；修复父级 separator 传播到 direct/generated inherited items；Gallery cold / P95 / alloc 有收益，repeated median 基本持平 |
| Navigation | ComboBox | Done | [ComboBox](ComboBox/README.md)；默认 host、popup content、handle `IconButton` 成本已按需/轻量化，Gallery repeated mean 提升约 `16.72%` |
| Navigation | DropdownButton | Pending | 已做 Button 联动 smoke；仍待独立基线 |
| Navigation | Menu | Done | [Menu](Menu/README.md)；闭合 leaf/toggle/submenu Popup 重内容已按需创建，ContextMenu window 订阅释放已补齐，Gallery repeated mean 小幅提升 |
| Navigation | NavMenu | Done | [NavMenu](NavMenu/README.md)；container binding 生命周期、全局关闭订阅按需化、默认路径完整显示和 Gallery 严格 ready 口径已完成 |
| Navigation | Pagination | Done | [Pagination](Pagination/README.md)；页码项和只读 `SimplePagination` 隐藏成本已按需化，`PaginationShowCase` repeated mean 提升约 `21.10%` |
| Navigation | Steps | Done (structural-only) | [Steps](Steps/README.md)；修复 `ConfigureItemsPanel` 未清理 `ColumnDefinitions` 的结构问题，未证明稳定页面级性能收益 |
| Navigation | TabControl | Done | [TabControl](TabControl/README.md)；默认 tab item hidden slot 按需化，`TabControlShowCase` repeated mean 提升约 `7.85%`，visuals `1583 -> 1405` |
| Data Entry | AutoComplete | Done | 关闭态 CandidateList/PopupFrame 已按需创建；Gallery repeated mean 提升约 `25.79%` |
| Data Entry | Cascader | Done | [Cascader](Cascader/README.md)；closed popup、filter input、multiple tags、filter list、checkbox/loading slot 已按需创建，Gallery repeated mean 小幅下降 |
| Data Entry | CheckBox | Done | [CheckBox](CheckBox/README.md)；unchecked/contentless 默认路径不再创建 wave、Icon mark、tristate mark；`CheckBoxShowCase` visuals `420 -> 318`，alloc `6385.60KB -> 4629.80KB` |
| Data Entry | ColorPicker | Done (structural lifecycle) | [ColorPicker](ColorPicker/README.md)；关闭态不再订阅 `Window.Deactivated`，Gallery 23 个 idle 订阅降为 0，页面 timing 噪声内 |
| Data Entry | DatePicker | Done | [DatePicker](DatePicker/README.md)；关闭态 popup content、默认 accessory host、Window.Deactivated 订阅已按需化 |
| Data Entry | TimePicker | Done | [TimePicker](TimePicker/README.md)；`InfoPickerInput` closed presenter 延迟到首次打开，真实 `TimePickerShowCase` repeated mean 提升约 `6.93%` |
| Data Entry | Form | Done | [Form](Form/README.md)；已完成低风险结构与生命周期优化，控件级小幅提升，Gallery 结构/分配下降，页面 repeated timing 基本持平 |
| Data Entry | Input | Done | [Input](Input/README.md)；TextBox/TextArea 默认 accessory、count、resize 成本按需化；真实 LineEditShowCase 结构/分配下降，repeated timing 基本持平 |
| Data Entry | LineEdit | Done | 已完成控件级优化与 Gallery 实测 |
| Data Entry | Mentions | Done | [Mentions](Mentions/README.md)；closed popup content 与 OptionsSource cache 已按需化，Gallery repeated mean 小幅改善，alloc 下降约 `7.80%` |
| Data Entry | NumberUpDown / NumericUpDown | Done | [NumericUpDown](NumericUpDown/README.md)；默认 clear/accessory 按需创建，`NumberUpDownShowCase` cold mean 提升约 `10.97%`，repeated mean 提升约 `3.97%` |
| Data Entry | OptionButtonGroup | Done | [OptionButtonGroup](OptionButtonGroup/README.md)；默认隐藏 icon/wave 成本已按需化，`RadioButtonShowCase` repeated mean 提升约 `15.99%` |
| Data Entry | RadioButton | Done | [RadioButton](RadioButton/README.md)；`RadioIndicator` wave 默认不预创建，`RadioButtonShowCase` visuals `689 -> 668`，Wave `22 -> 1` |
| Data Entry | Rate | Done | [Rate](Rate/README.md)；默认星形、焦点虚线框和全局 input 订阅成本已收敛，`RateShowCase` repeated mean 提升约 `10.96%` |
| Data Entry | Select | Done | [Select](Select/README.md)；`SelectShowCase` repeated mean `220.31ms -> 143.76ms`，alloc `28027.36KB -> 23904.61KB`；closed popup/list/accessory hidden cost 已按需化 |
| Data Entry | Slider | Done | [Slider](Slider/README.md)；普通 Slider 的 `PART_EndThumb` 按需创建，marks cache 与全局 input 订阅已收敛，Gallery repeated mean 提升约 `27.06%` |
| Data Entry | ToggleSwitch | Done (interaction structural) | [Switch](Switch/README.md)；`IsChecked` 切换 measure invalidations `1000 -> 0`，页面加载不作为主收益 |
| Data Entry | TreeSelect | Done | [TreeSelect](TreeSelect/README.md)；共享 `SelectHandle` 单 presenter 化，真实 `TreeSelectShowCase` repeated mean 提升约 `12.64%`，visuals `909 -> 858` |
| Data Entry | Transfer | Done | [Transfer](Transfer/README.md)；`TargetKeys` lookup 复用 `HashSet`，空 target panel 直接短路，`TransferShowCase` repeated mean 提升约 `8.49%`，cold mean 提升约 `10.22%` |
| Data Entry | Upload | Baseline only | [Upload](Upload/README.md)；已建立 Gallery baseline，3 个候选无收益/退化已回滚，仅保留测量入口 |
| Data Display | Avatar | Done | [Avatar](Avatar/README.md)；互斥内容 presenter 与折叠 flyout 已按需创建，Gallery repeated mean 提升约 `23.13%` |
| Data Display | Badge | Done | [Badge](Badge/README.md)；hidden zero 和无文本 DotBadge 已按需创建，Gallery visual/alloc 下降，导航 timing 收益有限 |
| Data Display | Calendar | Done | [Calendar](Calendar/README.md)；按当前 DisplayMode 延迟填充 MonthView/YearView cells，控件级 `Calendar.Default` ms/item 提升约 `24.63%`，Year/Decade 初始模式约 `48%`，Gallery visuals `201 -> 189` |
| Data Display | Card | Done | [Card](Card/README.md)；Header/Cover/Actions/Skeleton 已按需创建，Gallery repeated mean 提升约 `35.14%` |
| Data Display | Carousel | Done | [Carousel](Carousel/README.md)；nav/progress/PageTransition 已按需创建，真实 Gallery alloc `6113.86KB -> 5213.10KB`，cold `164.35ms -> 149.05ms`，repeated timing 基本持平 |
| Data Display | Collapse | Done | [Collapse](Collapse/README.md)；addon、no-arrow、closed content 已按需创建，Gallery repeated mean 提升约 `16.03%` |
| Data Display | Descriptions | Done | [Descriptions](Descriptions/README.md)；`DescriptionsShowCase` repeated mean `75.84ms -> 68.64ms`，visuals `579 -> 571`；binding/collection/window 订阅生命周期已补齐验证 |
| Data Display | DataGrid | Partial | [DataGrid](DataGrid/README.md)；filter flyout 内容延迟到首次打开，关闭态过滤菜单项 `7 -> 0`；filter indicator 可见性由 `MultiBinding` 收敛到 `TemplateBinding`；column/group/row header 本地 routed handlers 已收敛（column header hover `2/header -> 0/header`，press/release/move + forwarding `5/header -> 0/header`，group header forwarding `2/header -> 0/header`，row header press `1/header -> 0/header`，row group header press `1/header -> 0/header`）；column header clip repeated arrange allocation `1/header -> 0/header` after first；group column header view item clip repeated arrange allocation `1/header -> 0/header` after first；DataGrid core input handlers `4/grid -> 0/grid`；rows presenter scroll gesture handler `1/presenter -> 0/presenter`；rows presenter clip geometry repeated arrange allocation `1 -> 0` after first；row bottom grid-line clip repeated arrange allocation `1/row -> 0/row` after first；details presenter clip repeated arrange allocation `1/details -> 0/details` after first；cell clip repeated arrange allocation `1/cell -> 0/cell` after first；DataGridCell header-state binding converter 闭包已移除；special columns 在 `Columns.Clear()` 后释放 grid 事件链；列重排拖拽 indicator 复用 dashed `Pen`；DetailsPresenter measure registration 与 RowExpander details binding cleanup 已完成；分组行头内 row header owner/template 顺序空引用已修复 |
| Data Display | Expander | Done | [Expander](Expander/README.md)；closed content/addon/no-arrow slot 已按需创建，Gallery repeated mean 提升约 `18.11%` |
| Data Display | Empty | Done | [Empty](Empty/README.md)；低风险正确性修复与热路径收敛，Gallery repeated mean 小幅改善，visual tree 不变 |
| Data Display | GroupBox | Done | [GroupBox](GroupBox/README.md)；header icon presenter 已按需创建，`GroupBoxShowCase` visuals `151 -> 143`，repeated mean `27.79ms -> 26.16ms` |
| Data Display | ImagePreviewer | Done | [ImagePreviewer](ImagePreviewer/README.md)；单图关闭态只加载可见封面，完整弹窗 source list 延迟到首次打开；Gallery repeated mean 提升约 `8.79%` |
| Data Display | InfoFlyout | Pending | 待建立基线 |
| Data Display | List | Done | [ListView](ListView/README.md)；group/filter refresh 与分页枚举成本已收敛，真实 `ListShowCase` repeated mean 提升约 `4.19%` |
| Data Display | QRCode | Done | [QRCode](QRCode/README.md)；PNG encode/decode 已移除，icon/status hidden tree 按需创建，`QRCodeShowCase` repeated mean 提升约 `26.81%` |
| Data Display | Segmented | Done | [Segmented](Segmented/README.md)；无 icon item 的隐藏 `IconPresenter` 已按需化，真实 `SegmentedShowCase` repeated mean 提升约 `18.51%` |
| Data Display | Statistic | Done | [Statistic](Statistic/README.md)；generated content ownership、CountUp DataContext 清理、TimerStatistic timer 生命周期验证；`StatisticShowCase` repeated alloc 下降约 `3.25%`，visuals `387 -> 384` |
| Data Display | Tag | Pending | 待建立基线 |
| Data Display | TextBlock | Done (structural) | [TextBlock](TextBlock/README.md)；`HighlightableTextBlock` 段级 Run（N 字符 → ≤ 1 + 2K Run），`SelectableTextBlock` token binding + Cursor 移到 Theme Setter；微基准待 perf 哈纳斯恢复后补 |
| Data Display | Timeline | Done (structural + correctness) | [Timeline](Timeline/README.md)；`TimelineIndicator.Render` Pen 缓存，pending item 生命周期和默认/custom icon 显示已验证；页面级 timing 未证明收益 |
| Data Display | TreeView | Done | [TreeView](TreeView/README.md)；`NodeSwitcherButton` 单 presenter 化，真实 `TreeViewShowCase` repeated mean 提升约 `35.80%`，visuals `2108 -> 1646` |
| Data Display | Tooltip | Pending | 待建立基线 |
| Data Display | Tour | Done (structural) | [Tour](Tour/README.md)；`TourStepsView` 同生命周期 `RelayBind` 改为 template-priority direct binding，页面加载小幅改善，popup 打开场景待专项 harness |
| Feedback | Alert | Done | [MarqueeLabel](MarqueeLabel/README.md)；默认非 marquee Alert 不再承担隐藏 MarqueeLabel 成本，`AlertShowCase` repeated mean 提升约 `16.43%` |
| Feedback | Drawer | Done | [Drawer](Drawer/README.md)；关闭态 `OpenOn`/`SizeChanged` 已按需化，detach/open/close 生命周期已补齐验证；Gallery repeated mean `24.24ms -> 22.87ms` |
| Feedback | Message | Done | [Message](Message/README.md)；manager 按需创建，auto-close timer、MessageClosed、OnClose 生命周期已补齐，Gallery repeated mean 提升约 `7.91%` |
| Feedback | MessageBox | Done | [MessageBox](MessageBox/README.md)；同生命周期 binding 改为 `[!]`，loading skeleton 按需创建，状态/释放验证已补齐 |
| Feedback | Modal | Done | [Dialog](Dialog/README.md)；closed MessageBox 内部 Dialog 已按需创建，mask/resizer 生命周期已验证；Gallery 结构和分配下降，repeated timing 未证明提升 |
| Feedback | Notification | Done | [Notification](Notification/README.md)；manager/card 生命周期补齐，7 个 Gallery manager 与 progress bar 按需创建，Gallery repeated mean 提升约 `22.24%` |
| Feedback | PopupConfirm | Done | [PopupConfirm](PopupConfirm/README.md)；cancel/content slot 已按需创建，detach/reattach 与事件释放已验证，Gallery repeated mean 提升约 `10.60%` |
| Feedback | ProgressBar | Done | [ProgressBar](ProgressBar/README.md)；status icon 按需创建，真实 `ProgressBarShowCase` repeated mean `154.17ms -> 111.26ms`，runtime visuals `1389 -> 977` |
| Feedback | Result | Done | [Result](Result/README.md)；状态 icon presenter 与错误码 Svg 互斥按需创建，`ResultShowCase` repeated mean 提升约 `9.21%` |
| Feedback | Skeleton | Done (structural lifecycle + correctness) | [Skeleton](Skeleton/README.md)；重复 class handler、inactive animation、paragraph line rebuild/follow 生命周期已修复；页面 timing 不作为主收益 |
| Feedback | Spin | Done (structural lifecycle + correctness) | [Spin](Spin/README.md)；hidden `SpinIndicator` animation 延迟到可见时创建，motion 参数变化后重建已 materialized 动画；页面 timing 不作为主收益 |
| Feedback | Watermark | Pending | 待建立基线 |
| Tooling | AtomUI.Performance harness | Done (recovered) | Avalonia 12 迁移期间失同步的 62+ 错误已清干净（保留 stub `AddOnDecoratedBoxPerfProbe`、排除待重写的 `AccessoryLifecycleVerification.cs` / `EffectiveBrushVerification.cs`，3 处状态验证打 TODO）。`--suite textblock` / `--suite popup` / `--verify-textblock-states` 可用，T0.1 / T0.2 微基准已回填。 |

## 状态定义

| 状态 | 含义 |
| --- | --- |
| Pending | 尚未建立控件级性能基线 |
| Plan | 已形成分析方案，尚未建立控件级性能基线 |
| Baseline | 已有基线，尚未实施优化 |
| In Progress | 正在优化或验证 |
| Done | 已有基线、优化记录和最终数据 |
