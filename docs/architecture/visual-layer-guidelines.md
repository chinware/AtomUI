# AtomUI 视觉层规范

本文档定义 AtomUI 中跨普通视觉树绘制的 layer、overlay、adorner 的职责边界和使用规则。这里的“层”指会把内容放到普通控件布局流之外的宿主容器或窗口级覆盖区域，不包含普通 `Panel`、`Border.BoxShadow`、模板内部装饰容器等常规视觉结构。

## 总体原则

- 先判断覆盖内容的作用域，再选择层：控件局部、滚动/作用域局部、窗口级反馈、弹出浮层、窗口模板专用层分别使用不同机制。
- 不允许用子元素 `ZIndex` 解决跨父层遮挡问题。不同父层之间的遮挡必须在父层职责和层级顺序上解决。
- 不允许把局部装饰迁到窗口级层。Badge、拖拽预览、水印、Drawer、sticky mirror 等都有不同的锚定和裁剪语义。
- 新增跨视觉树覆盖能力前，必须先查本规范，复用现有层。只有现有层职责无法表达时，才新增 layer 类型。
- 新增 layer 或改变 layer 归属时，必须补充层级回归测试。修窗口级反馈层时，还必须验证 Badge 等局部装饰不受影响。

## 层级总览

| 层 / 宿主 | 所在模块 | 作用域 | 典型使用者 | 层级约定 |
|---|---|---|---|---|
| `VisualLayerManager` | Avalonia，AtomUI Window 模板承载 | TopLevel / Window | Popup overlay、原生 adorner、AtomUI 自定义层 | 根宿主，不直接作为业务层使用 |
| native `PopupRoot` / OS popup | Avalonia Popup | 独立 popup host / 原生窗口 | `Popup.ShouldUseOverlayLayer == false` 的弹出内容 | 不参与 `VisualLayerManager` 内部 ZIndex |
| Avalonia `PopupOverlayLayer` | Avalonia `VisualLayerManager` 内部层 | TopLevel / Window | Popup、ToolTip、ComboBox、Select、DatePicker、Dialog、MessageBox、ImagePreviewer | 由 Avalonia 管理，AtomUI 通过 resolver 获取 |
| Avalonia `AdornerLayer` | Avalonia 原生 adorner 层 | 控件局部装饰 | CountBadge、DotBadge、TreeView drag preview | 不用于窗口级反馈 |
| `ScopeAwareAdornerLayer` | `AtomUI.Controls.Primitives` | 控件、滚动区域或窗口中的作用域装饰 | Drawer、Watermark、Gallery sticky mirror | `int.MaxValue - 99` |
| `ScopeAwareOverlayLayerPanel` | `AtomUI.Controls.Primitives` | 模板内 host | ScrollViewer 内容区域 | 不是 layer，负责限定作用域 |
| `ScopeAwareOverlayLayer` | `AtomUI.Controls.Primitives` | 最近的 `ScopeAwareOverlayLayerPanel`、`VisualLayerManager` 或 TopLevel | FloatButton、滚动内容内的作用域浮动元素 | `int.MaxValue - 990` |
| `WindowFeedbackLayer` | `AtomUI.Desktop.Controls` | TopLevel / Window | Message、Notification | `int.MaxValue - 50`，高于 `ScopeAwareAdornerLayer` |
| `TourLayer` | `AtomUI.Desktop.Controls` | TopLevel / Window | Tour mask / spotlight | `int.MaxValue - 98` |
| `FullscreenPopoverLayer` | `AtomUI.Desktop.Controls.Window` | Window 模板内部 | macOS 全屏标题栏 popover | 模板固定层，不作为通用 overlay |

## 各层职责

### VisualLayerManager

`VisualLayerManager` 是 Avalonia 提供的窗口视觉层宿主。AtomUI Window 模板通过 `PART_VisualLayerManager` 承载窗口内容和多个 overlay/adorner 层。

使用规则：

- 业务控件不直接把自身当成 `VisualLayerManager` 的普通 child。
- 需要向 `VisualLayerManager` 注入自定义层时，必须通过明确的 layer 类型和固定职责实现，例如 `ScopeAwareAdornerLayer`、`ScopeAwareOverlayLayer`、`WindowFeedbackLayer`、`TourLayer`。
- 对 Avalonia 内部 layer 的访问必须集中在 `VisualLayerManagerReflectionExtensions` 等已有边界内，避免分散反射。

### PopupOverlayLayer

`PopupOverlayLayer` 是 Avalonia `VisualLayerManager` 的内部 popup overlay 层。AtomUI 通过 `GetPopupOverlayLayer()` 和 `OverlayLayerResolver` 获取它，用于需要窗口坐标系、popup 定位、modal mask 或无原生窗口 fallback 的浮层。

典型使用者：

- `Popup` 的 `ShouldUseOverlayLayer` 路径。
- `ToolTip`、`Flyout`、菜单、选择器、日期选择器等基于 popup 的控件。
- `Dialog`、`MessageBox`、`ImagePreviewer` 的 overlay host。
- `OverlayDialogMask` 作为 `OverlayPopupHost` 的兄弟节点插入 popup overlay layer，保证 mask 不被 dialog host 动画一起缩放。

使用规则：

- 弹出层、菜单、下拉面板、modal dialog、图片预览窗口内浮层优先使用 popup overlay 路径。
- 需要解析宿主时使用 `OverlayLayerResolver`，不要在各控件里重复查找主窗口、TopLevel 或 SingleView。
- 不要把 Message/Notification 放入 popup overlay。它们是窗口级反馈，不是 anchored popup。

### Native PopupRoot / OS popup

当 `Popup.ShouldUseOverlayLayer == false` 且平台支持原生窗口时，Popup 可以走 native popup root / OS popup 路径。该路径不在 `VisualLayerManager` 的内部 layer 顺序中，定位、阴影和裁剪行为由 Popup 与平台窗口管理共同决定。

使用规则：

- 普通下拉、菜单、Tooltip 等可以按控件属性选择原生 popup 或 overlay popup。
- 浏览器、无原生窗口或显式 overlay 场景必须走 overlay layer。
- 不要用 native popup 承载 modal mask 或窗口级反馈。
- 修 popup 阴影、定位、裁剪时要同时确认 native popup root 和 overlay popup 两条路径是否受影响。

### Avalonia AdornerLayer

Avalonia 原生 `AdornerLayer` 用于紧贴控件或 TopLevel 的局部装饰。它适合与目标控件生命周期和布局强绑定的轻量覆盖，不适合作为 AtomUI 全局层级体系的最高层。

典型使用者：

- `AbstractCountBadge` / `AbstractDotBadge` 在 `DecoratedTarget` 模式下挂 Count/Dot Badge adorner。
- `TreeView` 拖拽预览。

使用规则：

- Count/Dot Badge 必须保持在原生 `AdornerLayer`，这是局部装饰语义。
- `RibbonBadge` 不使用窗口级 adorner，保持在目标内容的 inline visual tree 中。
- 不要通过抬高原生 `AdornerLayer` 来解决 Message、Notification 或其他窗口级反馈遮挡问题。这会破坏 Badge 等局部装饰的层级语义。

### ScopeAwareAdornerLayer

`ScopeAwareAdornerLayer` 是 AtomUI 的作用域感知 adorner 层。它会优先寻找最近的 `ScrollContentPresenter` 或 `VisualLayerManager`，必要时注入到内容宿主中，适合需要跟随目标作用域、滚动区域或局部窗口内容的覆盖元素。

典型使用者：

- `Drawer` / `DrawerContainer`。
- `Watermark`。
- Gallery `GalleryStickyTabsHost` 的 sticky mirror。

使用规则：

- 用于“覆盖某个目标区域或作用域”的内容，而不是窗口级全局反馈。
- 需要跟随 `ScrollContentPresenter`、被装饰元素或局部内容坐标时，应优先使用该层。
- 使用 `ScopeAwareAdornerLayer.SetAdornedElement` / `SetAdorner` 时，必须有对应的 detach / cleanup 路径。
- 它的层级高于 Avalonia 原生 adorner；因此原生 `AdornerLayer` 内的内容不能假设能盖住它。

### ScopeAwareOverlayLayer

`ScopeAwareOverlayLayer` 是作用域浮动层，通常配合 `ScopeAwareOverlayLayerPanel` 使用。ScrollViewer 模板在内容区域包裹 `ScopeAwareOverlayLayerPanel`，让滚动内容中的浮动元素可以覆盖滚动条区域或内容区域。

典型使用者：

- `FloatButton` / `FloatButtonGroup` / `FloatButtonHost`。
- ScrollViewer 内容区域内的局部 overlay。

使用规则：

- 用于“作用域内浮动控件”，不是 modal、全局反馈或目标装饰。
- 如果控件应限制在某个 `ScrollViewer` 或局部 host 范围内，优先选择该层。
- 不要用它承载 Drawer、Message、Notification、Dialog 或菜单下拉。

### WindowFeedbackLayer

`WindowFeedbackLayer` 是 AtomUI 桌面控件包的窗口级全局反馈层。它挂在 `VisualLayerManager` 中，层级高于 `ScopeAwareAdornerLayer`，用于保证全局反馈不会被 sticky mirror、Drawer 这类作用域层遮挡。

典型使用者：

- `WindowMessageManager`。
- `WindowNotificationManager`。

使用规则：

- Message、Notification 等窗口级反馈必须优先使用该层。
- 如果宿主没有 `VisualLayerManager`，可以 fallback 到 Avalonia 原生 `AdornerLayer`，但这是兼容路径，不是主路径。
- 不要把 Badge、水印、Drawer、Tour、Popup、Dialog、ImagePreviewer 等迁入该层。
- 变更该层时，必须验证全局反馈层级和 Badge 局部装饰行为。

### TourLayer

`TourLayer` 是 Tour 控件专用的窗口遮罩层，用于绘制 spotlight mask 和目标区域洞穿。它是窗口级引导遮罩，不是通用 modal layer。

使用规则：

- 只用于 Tour。
- 其他控件需要 modal mask 时，应使用 popup overlay / dialog mask 体系，而不是复用 TourLayer。
- Tour 弹窗本体仍通过 popup overlay 路径定位，mask 和弹窗职责分离。

### FullscreenPopoverLayer

`FullscreenPopoverLayer` 是 Window 模板内部的固定层，用于 macOS 全屏场景下顶部标题栏 popover。它不是注入到 `VisualLayerManager` 的通用 layer。

使用规则：

- 只由 Window 模板和 Window 控件生命周期管理。
- 不得作为普通控件的 overlay 宿主。
- 不参与 Message、Notification、Popup、Badge、Drawer 等层级决策。

## 选择规则

| 场景 | 应使用 | 不应使用 |
|---|---|---|
| 全局 Message / Notification | `WindowFeedbackLayer` | `AdornerLayer`、`PopupOverlayLayer`、`ScopeAwareAdornerLayer` |
| 下拉、菜单、Tooltip、Flyout、选择器面板 | `PopupOverlayLayer` / `Popup.ShouldUseOverlayLayer` | `WindowFeedbackLayer` |
| Dialog / MessageBox / ImagePreviewer overlay host | `OverlayLayerResolver` 获取 `PopupOverlayLayer` | 手动遍历窗口或主视图 |
| Count/Dot Badge | Avalonia `AdornerLayer` | `WindowFeedbackLayer`、`ScopeAwareAdornerLayer` |
| RibbonBadge | inline visual tree | 任意窗口级 layer |
| Drawer / Watermark / sticky mirror | `ScopeAwareAdornerLayer` | `WindowFeedbackLayer`、`PopupOverlayLayer` |
| FloatButton / 作用域浮动按钮 | `ScopeAwareOverlayLayer` | `PopupOverlayLayer`、`WindowFeedbackLayer` |
| Tour mask | `TourLayer` | Dialog mask、`WindowFeedbackLayer` |
| TreeView 拖拽预览 | Avalonia `AdornerLayer` | `WindowFeedbackLayer` |
| macOS 全屏标题栏 popover | `FullscreenPopoverLayer` | 通用 layer API |

## 层级顺序规则

- `WindowFeedbackLayer` 必须高于 `ScopeAwareAdornerLayer`，否则 Message / Notification 会被 sticky mirror 或作用域装饰遮挡。
- `TourLayer` 必须高于 `ScopeAwareAdornerLayer`，以保证引导遮罩覆盖作用域装饰。
- `ScopeAwareAdornerLayer` 可以高于 Avalonia 原生 `AdornerLayer`，这意味着原生 adorner 不应用于全局优先级覆盖。
- `ScopeAwareOverlayLayer` 是作用域 overlay，层级不应被用来表达全局优先级。
- 新增 `int.MaxValue - N` 层级常量时，必须在本文档更新职责和相对顺序。不要裸写 `int.MaxValue` 或随机选择接近最大值的数字。

当前 AtomUI 自定义层级区间：

| 层 | ZIndex |
|---|---:|
| `WindowFeedbackLayer` | `int.MaxValue - 50` |
| `TourLayer` | `int.MaxValue - 98` |
| `ScopeAwareAdornerLayer` | `int.MaxValue - 99` |
| `ScopeAwareOverlayLayer` | `int.MaxValue - 990` |

## 生命周期与清理

- 所有动态加入 layer 的元素必须在 detach、close、dispose、template reapply 或 owner 变更时移除。
- 如果 layer child 设置了 adorned element、logical parent、事件订阅、timer、binding 或 resource host，释放路径必须和获取路径成对出现。
- 对 `WindowMessageManager`、`WindowNotificationManager` 这类 manager，宿主模板重套用时必须先从旧 host layer 移除，再重新安装。
- 对 `ScopeAwareAdornerLayer` / `ScopeAwareOverlayLayer` 注入式层，缓存 layer 时必须确认 visual parent 仍然有效。

## 测试要求

- 新增或改变 layer 归属时，应写层级回归测试，断言父层级而不是只断言 child `ZIndex`。
- 修窗口级反馈遮挡时，必须覆盖 Message 和 Notification。
- 涉及 Badge、Watermark、Drawer、sticky mirror、TreeView drag preview 时，应分别验证其既有层归属没有被误改。
- Gallery sticky mirror、Message / Notification 和 Badge 的组合是当前层级体系的关键回归场景。

## 禁止做法

- 禁止为了让某个控件“显示在最上面”直接把它放入 `WindowFeedbackLayer`。
- 禁止通过提高 MessageCard、NotificationCard、Badge adorner 等 child 的 `ZIndex` 解决跨父层遮挡。
- 禁止把 modal mask 放进 popup host 子树并跟随 dialog 动画缩放；mask 应作为 popup overlay layer 中的兄弟节点。
- 禁止在每个控件中复制 TopLevel / MainWindow / SingleView 查找逻辑；应使用现有 resolver。
- 禁止新增 layer 后不更新本文档和回归测试。
