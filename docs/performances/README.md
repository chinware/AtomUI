# AtomUI 控件性能优化总览

本目录用于沉淀 AtomUI 控件性能优化的基线、方案、阶段记录和最终结果。

测试工具位于 [tools/performances](/Users/chinboy/Projects/dotnet/AtomUIV6/tools/performances/README.md)。

样式 selector 与代码状态机的性能边界见：[style-vs-code-guidelines.md](style-vs-code-guidelines.md)。

## 目录规范

- 每个控件或共享性能体系使用一个独立目录，目录名使用控件名的大小写写法，例如 `AddOnDecoratedBox`、`LineEdit`。
- 控件目录内保留该控件自己的基线、方案、阶段记录、实测结果和复现命令。
- 跨控件共享的基础设施可以单独成目录，例如 `AddOnDecoratedBox`。
- Gallery 级页面实测放到最直接受影响的控件目录下，并在文档内说明是否包含 Gallery 上层成本。

## 入库规则

应进入版本控制：

- 总览、guide、控件目录 README。
- 控件级优化方案和关键设计取舍。
- 优化前基线和最终结果。
- Gallery 真实场景的汇总结论和复现命令。

不进入版本控制：

- 阶段中间跑分输出。
- 一次性评估草稿。
- 可由工具重新生成的逐次 sample 原始输出。
- `/tmp` worktree 中生成的 baseline 对照结果。

## 测试口径

| 口径 | 工具 | 用途 | 注意事项 |
| --- | --- | --- | --- |
| 控件级基准 | `tools/performances/AtomUI.Performance` | 单控件、小组合、批量实例化、专项行为验证 | 不代表 Gallery 页面打开体验 |
| Gallery 场景复现 | `tools/performances/AtomUI.GalleryPerformance` | 真实 Workspace route、真实 showcase XAML、真实视觉树稳定耗时 | 必须证明源 XAML 和运行时控件形态一致 |

## 当前文档

| 控件/体系 | 状态 | 目录 | 关键结果 |
| --- | --- | --- | --- |
| AddOnDecoratedBox | 本轮已完成 | [AddOnDecoratedBox](AddOnDecoratedBox/README.md) | 默认 LineEdit visual/root `26 -> 20`，KB/item `503.8 -> 398.4`，默认 icon scan `360 -> 0` |
| Icon | Phase 1 已完成 | [Icon](Icon/README.md) | `IconShowCase` repeated navigation `193.31ms -> 160.81ms`，提升 `16.8%`，分配下降 `17.3%` |
| LineEdit | 本轮已完成 Gallery 实测 | [LineEdit](LineEdit/README.md) | `LineEditShowCase` 重复导航均值 `229.07ms -> 183.98ms`，提升 `19.68%` |

## 总列表

| 分类 | 控件/体系 | 状态 | 备注 |
| --- | --- | --- | --- |
| Shared Primitive | AddOnDecoratedBox | Done | 输入类控件共享视觉与 addon 体系 |
| Shared Primitive | Icon | In Progress | Phase 1 已完成；render 热路径、静态元数据缓存、隐藏 icon slot 待继续 |
| General | Button | Pending | 待建立基线 |
| General | FloatButton | Pending | 待建立基线 |
| General | SplitButton | Pending | 待建立基线 |
| General | Separator | Pending | 待建立基线 |
| General | ButtonSpinner | Pending | 待建立基线 |
| Layout | FlexPanel | Pending | 待建立基线 |
| Layout | Grid | Pending | 待建立基线 |
| Layout | Space | Pending | 待建立基线 |
| Layout | Splitter | Pending | 待建立基线 |
| Navigation | Breadcrumb | Pending | 待建立基线 |
| Navigation | ComboBox | Pending | 待建立基线 |
| Navigation | DropdownButton | Pending | 待建立基线 |
| Navigation | Menu | Pending | 待建立基线 |
| Navigation | Pagination | Pending | 待建立基线 |
| Navigation | Steps | Pending | 待建立基线 |
| Navigation | TabControl | Pending | 待建立基线 |
| Data Entry | AutoComplete | Pending | 待建立基线 |
| Data Entry | Cascader | Pending | 受 AddOnDecoratedBox 优化影响 |
| Data Entry | CheckBox | Pending | 待建立基线 |
| Data Entry | ColorPicker | Pending | 待建立基线 |
| Data Entry | DatePicker | Pending | 受 AddOnDecoratedBox 优化影响 |
| Data Entry | TimePicker | Pending | 待建立基线 |
| Data Entry | Form | Pending | 待建立基线 |
| Data Entry | LineEdit | Done | 已完成控件级优化与 Gallery 实测 |
| Data Entry | Mentions | Pending | 待建立基线 |
| Data Entry | NumberUpDown | Pending | 待建立基线 |
| Data Entry | RadioButton | Pending | 待建立基线 |
| Data Entry | Rate | Pending | 待建立基线 |
| Data Entry | Select | Pending | 受 AddOnDecoratedBox 优化影响 |
| Data Entry | Slider | Pending | 待建立基线 |
| Data Entry | ToggleSwitch | Pending | 待建立基线 |
| Data Entry | TreeSelect | Pending | 受 AddOnDecoratedBox 优化影响 |
| Data Entry | Transfer | Pending | 待建立基线 |
| Data Entry | Upload | Pending | 待建立基线 |
| Data Display | Avatar | Pending | 待建立基线 |
| Data Display | Badge | Pending | 待建立基线 |
| Data Display | Calendar | Pending | 待建立基线 |
| Data Display | Card | Pending | 待建立基线 |
| Data Display | Carousel | Pending | 待建立基线 |
| Data Display | Collapse | Pending | 待建立基线 |
| Data Display | Descriptions | Pending | 待建立基线 |
| Data Display | DataGrid | Pending | 待建立基线 |
| Data Display | Expander | Pending | 待建立基线 |
| Data Display | Empty | Pending | 待建立基线 |
| Data Display | GroupBox | Pending | 待建立基线 |
| Data Display | ImagePreviewer | Pending | 待建立基线 |
| Data Display | InfoFlyout | Pending | 待建立基线 |
| Data Display | List | Pending | 待建立基线 |
| Data Display | QRCode | Pending | 待建立基线 |
| Data Display | Segmented | Pending | 待建立基线 |
| Data Display | Statistic | Pending | 待建立基线 |
| Data Display | Tag | Pending | 待建立基线 |
| Data Display | Timeline | Pending | 待建立基线 |
| Data Display | TreeView | Pending | 待建立基线 |
| Data Display | Tooltip | Pending | 待建立基线 |
| Data Display | Tour | Pending | 待建立基线 |
| Feedback | Alert | Pending | 待建立基线 |
| Feedback | Drawer | Pending | 待建立基线 |
| Feedback | Message | Pending | 待建立基线 |
| Feedback | Modal | Pending | 待建立基线 |
| Feedback | Notification | Pending | 待建立基线 |
| Feedback | PopupConfirm | Pending | 待建立基线 |
| Feedback | ProgressBar | Pending | 待建立基线 |
| Feedback | Result | Pending | 待建立基线 |
| Feedback | Skeleton | Pending | 待建立基线 |
| Feedback | Spin | Pending | 待建立基线 |
| Feedback | Watermark | Pending | 待建立基线 |

## 状态定义

| 状态 | 含义 |
| --- | --- |
| Pending | 尚未建立控件级性能基线 |
| Plan | 已形成分析方案，尚未建立控件级性能基线 |
| Baseline | 已有基线，尚未实施优化 |
| In Progress | 正在优化或验证 |
| Done | 已有基线、优化记录和最终数据 |
