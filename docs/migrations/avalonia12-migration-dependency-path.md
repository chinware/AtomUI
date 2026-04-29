# Avalonia 12 迁移依赖路径

基于 `release/5.0` 分支的控件模块依赖分析（含 C# 代码引用 + Themes axaml 模板引用），制定分阶段迁移计划。共 74 个模块，无循环依赖，最大依赖深度 7 层。

## 关键基础设施（被依赖最多，优先迁移）

| 模块 | 被依赖次数 | 典型依赖方 |
|------|-----------|-----------|
| TextBlock | 25+ | Alert, Avatar, Badge, Empty, ListBox, NavMenu, Select... |
| Input | 20 | AutoComplete, Buttons, ComboBox, Menu, Select... |
| Buttons | 16 | Alert, Collapse, Dialog, Drawer, Expander, Form, Upload... |
| ScrollViewer | 14 | Carousel, Flyouts, ListBox, NavMenu, Primitives, TreeView... |
| Popup | 14 | Buttons, Cascader, Flyouts, Menu, Tooltip... |
| Window | 11 | Chrome, Dialog, Form, MessageBox, Popup... |
| Primitives | 10 | Buttons, Calendar, Flyouts, Input, Select, Tooltip... |
| HeaderedContentControl | 8 | Card, Collapse, Drawer, NavMenu, Steps... |

## 完整依赖关系图

### 叶子模块（18个，无跨模块依赖，可独立迁移）

AdornerLayer, Breadcrumb, CheckBox, HeaderedContentControl, MarqueeLabel, ProgressBar, RadioButton, Rate, Result, ScrollViewer, Segmented, Skeleton, Space, SplitView, Switch, Tag, TextBlock, Timeline

### 组合模块（56个）依赖关系

| 模块 | 依赖 |
|------|------|
| Alert | Buttons, MarqueeLabel |
| AutoComplete | Input, Popup, Window |
| Avatar | TextBlock |
| Badge | TextBlock |
| ButtonSpinner | Input |
| Buttons | Input, Popup, Primitives |
| Calendar | DatePicker, Primitives |
| Card | HeaderedContentControl, TabControl |
| Carousel | ScrollViewer |
| Cascader | ListBox, Popup, TreeView |
| Chrome | Window |
| Collapse | Buttons, HeaderedContentControl |
| ComboBox | Input, Popup, Window |
| DatePicker | Calendar, Input, Primitives, TimePicker |
| Descriptions | HeaderedContentControl, Window |
| Dialog | Buttons, Input, Skeleton, Window |
| Drawer | Buttons, HeaderedContentControl, Separator, TextBlock |
| Empty | TextBlock |
| Expander | Buttons |
| FloatButton | Input |
| Flyouts | Input, Menu, Popup, Primitives, ScrollViewer |
| Form | Buttons, Tooltip, Window |
| GroupBox | TextBlock |
| ImagePreviewer | Buttons, Dialog, Window |
| Input | Primitives, ScrollViewer, TextBlock |
| ListBox | Empty, ScrollViewer, TextBlock |
| ListView | Empty, ListBox, ScrollViewer, Spin, TextBlock |
| Mentions | Input, Popup, Window |
| Menu | CheckBox, Input, Popup, RadioButton |
| Message | AdornerLayer |
| MessageBox | Dialog, Window |
| NavMenu | HeaderedContentControl, Input, Menu, Popup, ScrollViewer, TextBlock |
| Notifications | AdornerLayer, Buttons, TextBlock |
| NumericUpDown | ButtonSpinner, Input |
| OptionButtonGroup | TextBlock |
| Pagination | ComboBox, Input, TextBlock |
| PopupConfirm | Buttons, TextBlock |
| Popup | AdornerLayer, Input, Primitives, Window |
| Primitives | Input, ListBox, Popup, ScrollViewer, TextBlock |
| QRCode | Buttons, Spin, TextBlock |
| Select | Input, ListView, Popup, Primitives, ScrollViewer, Tag, TextBlock, Window |
| Separator | TextBlock |
| Slider | Input, Tooltip |
| Spin | TextBlock |
| Splitter | Primitives |
| Statistic | HeaderedContentControl, Skeleton |
| Steps | HeaderedContentControl, Tooltip |
| TabControl | HeaderedContentControl, Input, Primitives, ScrollViewer |
| TimePicker | Input, TextBlock |
| Tooltip | Input, Popup, Primitives |
| Tour | Buttons, Dialog, Popup, Primitives, Steps |
| Transfer | Buttons, CheckBox, Input, ListView, RadioButton, TextBlock, TreeView |
| TreeSelect | Popup, TreeView |
| TreeView | AdornerLayer, CheckBox, Empty, Input, Popup, RadioButton, ScrollViewer, TextBlock |
| Upload | Buttons, ImagePreviewer, Input, ProgressBar, TextBlock, Tooltip |
| Window | ScrollViewer |

## 推荐迁移路径（8 个阶段）

### Phase 1 — 叶子模块（18个，可并行）

> AdornerLayer, Breadcrumb, CheckBox, HeaderedContentControl, MarqueeLabel, ProgressBar, RadioButton, Rate, Result, ScrollViewer, Segmented, Skeleton, Space, SplitView, Switch, Tag, TextBlock, Timeline

这些模块无跨模块依赖，可以任意顺序独立迁移。

### Phase 2 — 仅依赖 Phase 1 的简单组合（13个）

> Avatar, Badge, Carousel, Empty, GroupBox, Message, OptionButtonGroup, Separator, Spin, Statistic, Window, Notifications, Expander

这些模块仅依赖 Phase 1 的叶子模块（TextBlock、ScrollViewer、AdornerLayer、Skeleton、Buttons 等）。

注意：Expander 和 Notifications 依赖 Buttons，而 Buttons 依赖 Input → Primitives，属于 Phase 3。如果 Buttons 未就绪，Expander/Notifications 需推迟。

### Phase 3 — 核心基础设施

> Input → Primitives → Popup → Buttons

这四个是整个控件库的骨架，存在复杂的互相依赖：
- Input 依赖 Primitives、ScrollViewer、TextBlock
- Primitives 依赖 Input、ListBox、Popup、ScrollViewer、TextBlock
- Popup 依赖 AdornerLayer、Input、Primitives、Window
- Buttons 依赖 Input、Popup、Primitives

实际迁移时需要同步处理这一组。

### Phase 4 — 中间层

> Alert, ButtonSpinner, Chrome, Collapse, Drawer, FloatButton, ListBox, Menu, Slider, Tooltip, PopupConfirm, QRCode

依赖 Phase 1-3 模块，迁移风险适中。

### Phase 5 — 复合控件

> AutoComplete, ComboBox, Dialog, Flyouts, Form, ImagePreviewer, ListView, Mentions, NavMenu, NumericUpDown, Steps, TabControl, TimePicker, TreeView

依赖多个 Phase 3-4 模块，复杂度较高。

### Phase 6 — 高级控件

> Cascader, DatePicker, Calendar, Pagination, Select, Tour, Transfer, TreeSelect, Upload

依赖链较深，需要前置模块全部就绪。

### Phase 7 — 复杂组合

> Card, Descriptions, MessageBox

依赖 Phase 5-6 模块。

### Phase 8 — 最终

> (无特定模块，用于处理遗留问题和集成测试)

## 注意事项

- **Primitives 和 Popup 存在双向引用**：Primitives 依赖 Popup，Popup 依赖 Primitives。实际迁移时需要同步处理。
- **AtomUI.Controls 基类**：Desktop 控件通常继承 AtomUI.Controls 中的抽象基类，迁移时需同时扫描基类。
- **TextBlock 是隐性基础设施**：25+ 个模块通过 themes axaml 依赖 TextBlock，必须在 Phase 1 最先稳定。
- **Themes axaml 引入大量隐性依赖**：仅分析 C# 代码引用会遗漏模板中的控件引用（如 Alert 使用 IconButton）。
- **DatePicker ↔ Calendar 互相依赖**：需同步迁移。
- **Flyouts 迁移被阻塞**：依赖 Menu 和 Popup 模块，需等 Phase 3/4 完成后继续。
