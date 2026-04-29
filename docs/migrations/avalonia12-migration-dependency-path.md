# Avalonia 12 迁移依赖路径

基于 `release/5.0` 分支的控件模块依赖分析，制定分阶段迁移计划。共 74 个模块，无循环依赖，最大依赖深度 7 层。

## 关键基础设施（被依赖最多，优先迁移）

| 模块 | 被依赖次数 | 典型依赖方 |
|------|-----------|-----------|
| Input | 20 | AutoComplete, Buttons, ComboBox, Menu, Select... |
| Popup | 14 | Buttons, Cascader, Flyouts, Menu, Tooltip... |
| Window | 11 | Chrome, Dialog, Form, MessageBox, Popup... |
| Primitives | 10 | Buttons, Calendar, Flyouts, Input, Tooltip... |
| HeaderedContentControl | 8 | Card, Collapse, Drawer, NavMenu, Steps... |

## 完整依赖关系图

### 叶子模块（31个，无跨模块依赖，可独立迁移）

AdornerLayer, Alert, Avatar, Badge, Breadcrumb, Carousel, CheckBox, Empty, Expander, GroupBox, HeaderedContentControl, ListBox, MarqueeLabel, OptionButtonGroup, PopupConfirm, ProgressBar, QRCode, RadioButton, Rate, Result, ScrollViewer, Segmented, Separator, Skeleton, Space, Spin, SplitView, Switch, Tag, TextBlock, Timeline

### 组合模块（43个）依赖关系

| 模块 | 依赖 |
|------|------|
| AutoComplete | Input, Popup, Window |
| ButtonSpinner | Input |
| Buttons | Input, Popup, Primitives |
| Calendar | DatePicker, Primitives |
| Card | HeaderedContentControl, TabControl |
| Cascader | ListBox, Popup, TreeView |
| Chrome | Window |
| Collapse | HeaderedContentControl |
| ComboBox | Input, Popup, Window |
| DatePicker | Calendar, Input, Primitives, TimePicker |
| Descriptions | HeaderedContentControl, Window |
| Dialog | Buttons, Input, Window |
| Drawer | HeaderedContentControl |
| FloatButton | Input |
| Flyouts | Input, Menu, Popup, Primitives |
| Form | Window |
| ImagePreviewer | Window |
| Input | Primitives, ScrollViewer, TextBlock |
| ListView | Empty, ListBox |
| Mentions | Input, Popup, Window |
| Menu | CheckBox, Input, Popup, RadioButton |
| Message | AdornerLayer |
| MessageBox | Dialog, Window |
| NavMenu | HeaderedContentControl, Input, Menu, Popup |
| Notifications | AdornerLayer |
| NumericUpDown | ButtonSpinner |
| Pagination | ComboBox, TextBlock |
| Popup | AdornerLayer, Input, Primitives, Window |
| Primitives | Input, ListBox, Popup, ScrollViewer, TextBlock |
| Select | Input, ListView, Popup, ScrollViewer, Tag, Window |
| Slider | Input |
| Splitter | Primitives |
| Statistic | HeaderedContentControl |
| Steps | HeaderedContentControl |
| TabControl | HeaderedContentControl, Input, Primitives, ScrollViewer |
| TimePicker | Input, TextBlock |
| Tooltip | Input, Popup, Primitives |
| Tour | Popup, Primitives, Steps |
| Transfer | CheckBox, ListView, TreeView |
| TreeSelect | Popup, TreeView |
| TreeView | AdornerLayer, Input, Popup, RadioButton |
| Upload | ImagePreviewer, Input |
| Window | ScrollViewer |

## 推荐迁移路径（7 个阶段）

### Phase 1 — 叶子模块（31个，可并行）

> AdornerLayer, Alert, Avatar, Badge, Breadcrumb, Carousel, CheckBox, Empty, Expander, GroupBox, HeaderedContentControl, ListBox, MarqueeLabel, OptionButtonGroup, PopupConfirm, ProgressBar, QRCode, RadioButton, Rate, Result, ScrollViewer, Segmented, Separator, Skeleton, Space, Spin, SplitView, Switch, Tag, TextBlock, Timeline

这些模块无跨模块依赖，可以任意顺序独立迁移。

### Phase 2 — 核心基础设施

> Window → Primitives → Input → Popup

这四个是整个控件库的骨架，Input 被 20 个模块依赖，必须最先稳定。

### Phase 3 — 中间层

> Message, Notifications, ListView, Collapse, Drawer, Statistic, Steps, Chrome, Form, ImagePreviewer, Splitter

依赖较少，迁移风险低。

### Phase 4 — 复合控件

> Dialog, Menu, Buttons, Tooltip, Tour, TimePicker, Slider, FloatButton, ButtonSpinner

依赖 Phase 2 的核心基础设施。

### Phase 5 — 高级控件

> AutoComplete, ComboBox, Mentions, Select, TreeView, Cascader, TabControl, NavMenu, Flyouts

依赖多个 Phase 2-4 模块，复杂度较高。

### Phase 6 — 复杂组合

> DatePicker, Calendar, Pagination, NumericUpDown, Transfer, TreeSelect, Card, Descriptions, Upload

依赖链较深，需要前置模块全部就绪。

### Phase 7 — 最终

> MessageBox

依赖 Dialog + Window，是依赖链最深的模块。

## 注意事项

- **Primitives 和 Popup 存在双向引用**：Primitives 依赖 Popup，Popup 依赖 Primitives。实际迁移时需要同步处理。
- **AtomUI.Controls 基类**：Desktop 控件通常继承 AtomUI.Controls 中的抽象基类，迁移时需同时扫描基类。
- **Flyouts 迁移被阻塞**：依赖 Menu 和 Popup 模块，需等 Phase 2/4 完成后继续。
- **DatePicker ↔ Calendar 互相依赖**：需同步迁移。
