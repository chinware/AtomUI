# Segmented 桌面版实现原理

本文档描述 Segmented 桌面版的共享基类、桌面封装、容器准备、选择流、方向与 expanding 布局、键盘导航、选中滑块渲染和 Shape 主题协作。公共设计与 API 契约见 [Segmented 桌面版架构设计](overview.md)，Token 语义见 [Segmented Token 设计](token.md)，变化记录见 [Segmented Changelog](changelog.md)。

## 1. 实现定位

Segmented 的实现重点是把 Avalonia `SelectingItemsControl` 的单选状态、生成容器、item pointer 交互和根控件 render 层的选中滑块统一到一个稳定状态流中。

本文档只描述 Segmented 相关实现结构，不重新说明 `SelectingItemsControl`、Avalonia 模板系统、Token 系统或 `Panel` 测量排列的通用机制。

## 2. 源码文件结构

共享源码：

- `src/AtomUI.Controls/Segmented/SegmentedShape.cs`：Segmented 专用 `Default` / `Round` 形状枚举。
- `src/AtomUI.Controls/Segmented/AbstractSegmented.cs`：共享根控件，定义公共属性、内部滑块属性、选择生命周期、方向键选择、容器准备、Form 接口和 render 绘制。
- `src/AtomUI.Controls/Segmented/AbstractSegmentedItem.cs`：共享 item，定义 `IsSelected`、`Icon`、内部 `SizeType` / `Shape` / `IsMotionEnabled`、图标伪类和 pointer release 选择。
- `src/AtomUI.Controls/Segmented/SegmentedStackPanel.cs`：内部方向感知 items panel，执行横向/纵向排列、水平 expanding 和可导航容器协作。
- `src/AtomUI.Controls/Segmented/SegmentedPseudoClass.cs`：Segmented 专用伪类常量。

桌面源码：

- `src/AtomUI.Desktop.Controls/Segmented/Segmented.cs`：桌面公开根控件，注册 Token scope，创建 `SegmentedItem` 容器。
- `src/AtomUI.Desktop.Controls/Segmented/SegmentedItem.cs`：桌面公开 item，注册 Token scope。
- `src/AtomUI.Desktop.Controls/Segmented/SegmentedToken.cs`：Segmented 控件 Token。
- `src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedTheme.axaml`：根模板、轨道、滑块、方向/expanding、SizeType/Shape 和 motion 样式。
- `src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedItemTheme.axaml`：item 模板、状态样式、SizeType/Shape 和图标样式。

Gallery 和测试：

- `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented/`：Segmented 示例、源码片段和本地化文案。
- `tests/AtomUI.Desktop.Controls.Tests/Segmented/SegmentedSelectionInitializationTests.cs`：选择初始化、Form value 和 expanding 布局回归测试。
- `tests/AtomUI.Desktop.Controls.Tests/SizeType/CustomizableSizeTypeContractTests.cs`：`ICustomizableSizeTypeAware` 契约测试。
- `tests/AtomUIGallery.Tests/ShowCases/SegmentedShowCasePageTests.cs`：Gallery 页面结构和示例快照测试。

## 3. 核心类职责

`AbstractSegmented` 是状态协调器。它持有方向、形状、选择属性、内部选中滑块属性和 Form value 适配逻辑，负责方向键选择，并在选择变化和最终排列后同步选中滑块矩形。

`Segmented` 是桌面公开控件。它不重复共享选择逻辑，只注册 `SegmentedToken.ScopeProvider` 并为数据 item 创建 `SegmentedItem` 容器。

`AbstractSegmentedItem` 是 item 基类。它接入 `ISelectable`，接收 owner 的 SizeType、Shape 和 motion 状态，维护 `Icon` 到 `:has-icon` 的伪类同步，并在鼠标左键释放时请求 owner 更新选择。

`SegmentedItem` 是桌面公开 item。它不添加额外行为，只注册 Segment 专属 Token scope。

`SegmentedStackPanel` 是布局和导航容器。它继承 `StackPanel` 以复用 `Orientation` 和 `INavigableContainer` 语义，但由自身算法控制可见 `AbstractSegmentedItem` 的横向/纵向测量、排列和 expanding。

## 4. 状态与数据流

容器准备流：

```text
Items / ItemsSource
  → CreateContainerForItemOverride()
  → SegmentedItem
  → PrepareContainerForItemOverride()
  → content/template sync
  → SizeType + Shape + IsMotionEnabled binding to owner
  → PrepareSegmentedItem()
```

键盘选择流：

```text
Left / Up    → previous direction
Right / Down → next direction
  → SelectingItemsControl.MoveSelection(wrap: true)
  → SegmentedStackPanel.INavigableContainer
  → skip disabled / hidden item
  → update selection and focus target container
```

选择流：

```text
pointer left button released on item
  → AbstractSegmentedItem.OnPointerReleased()
  → owner.UpdateSelectionFromPointerEvent()
  → SelectingItemsControl selection update
  → SelectionChanged
  → SetupSelectedThumbRect()
```

默认选择流：

```text
OnApplyTemplate()
  → scan current containers for IsSelected
  → if no container selected and SelectedIndex == -1 and SelectedItem == null and Items.Count > 0
  → set SelectedIndex = 0
  → SetupSelectedThumbRect()
```

Form value 流：

```text
IFormItemAware.SetFormValue(value) → SelectedItem = value
SelectedItem changed              → ValueChanged
IFormItemAware.GetFormValue()     → SelectedItem
IFormItemAware.ClearFormValue()   → SelectedItem = null
```

## 5. 生命周期与模板接入

静态初始化：

- `Orientation`、`IsExpanding` 和 `SizeType` 影响 measure/arrange。
- 选中滑块相关内部属性影响 render。
- `Orientation` 默认值覆盖为 `Horizontal`，`Shape` 默认值为 `Default`。
- `AutoScrollToSelectedItem` 对 Segmented 默认关闭。
- `SelectedItemProperty.Changed` 触发 Form value changed 通知。

构造阶段：

- `AbstractSegmented` 设置 `SelectionMode=Single`。
- `Segmented` 注册 `SegmentedToken.ScopeProvider`。
- `SegmentedItem` 注册 `SegmentedToken.ScopeProvider`。

初始化和加载：

- 根控件和 item 在 `OnInitialized()` 中临时禁用 transitions。
- `OnLoaded()` 通过 dispatcher 重新启用 transitions，避免初次布局时出现无意义动画。

模板接入：

- `OnApplyTemplate()` 保留已绑定或显式选择。
- 未提供选择时选择第一个 item。
- 模板应用后同步选中滑块矩形。

视觉树生命周期：

- `OnAttachedToVisualTree()` 订阅 `SelectionChanged`。
- `OnDetachedFromVisualTree()` 解除 `SelectionChanged`。
- `ArrangeOverride()` 在子项完成最终排列后校准选中滑块矩形。

## 6. 交互与事件处理

Segmented 的专用 pointer 交互在 item 层处理：

- `AbstractSegmentedItem.OnPointerReleased()` 只处理未被标记 handled 的事件。
- 当前 item 必须能找到 `AbstractSegmented` owner。
- 只有鼠标左键释放会立即请求 owner 更新选择。
- owner 的 `ShouldTriggerSelection()` 同样把鼠标左键释放识别为选择触发点。

键盘选择由根控件处理。四个方向键映射为前一项或后一项，并调用 `SelectingItemsControl.MoveSelection()`；`SegmentedStackPanel` 作为 `INavigableContainer` 提供当前方向下的容器顺序。选择首尾循环，Avalonia 的有效控件查找负责跳过 disabled、不可聚焦和 hidden 容器，成功后同步选择并把焦点移动到目标容器。

Segmented 不定义命令、弹层或异步数据加载路径。

## 7. 内部算法与关键流程

### 7.1 容器创建和数据项承载

`Segmented.CreateContainerForItemOverride()` 始终返回 `SegmentedItem`。当 item 不是 `Visual` 时，`PrepareContainerForItemOverride()` 把 item 设置为容器 `Content`，并在存在 `ItemTemplate` 时把 owner 的 `ItemTemplate` 绑定给容器 `ContentTemplate`。

容器准备阶段还会把 owner 的 `SizeType`、`Shape` 和 `IsMotionEnabled` 绑定给 item，确保生成 item 与根控件使用同一尺寸、形状和 motion 分支。`Orientation` 只控制 owner 与 items panel，不需要进入 item 状态。

### 7.2 选中滑块矩形

`SetupSelectedThumbRect()` 只在 `SelectedItem` 非空且能找到对应容器时更新滑块：

```text
container = ContainerFromItem(SelectedItem)
offset = container.TranslatePoint((0, 0), owner)
SelectedThumbPos = offset
SelectedThumbSize = container.Bounds.Size
```

滑块矩形是 render 输入，不是 visual tree 中的独立控件。选择变化时可以立即读取已排列容器；`ArrangeOverride()` 在布局完成后再次按最终 Bounds 校准。这样水平 expanding、垂直全宽、方向动态切换和父容器尺寸变化都使用同一坐标模型。

### 7.3 根控件 Render

`AbstractSegmented.Render()` 绘制两层矩形：

1. 使用 `Background` 和根 `CornerRadius` 绘制轨道背景。
2. 使用 `SelectedThumbBg`、`SelectedThumbCornerRadius` 和 `SelectedThumbBoxShadows` 绘制选中滑块。

item 本身的背景和文本状态仍由 `SegmentedItemTheme.axaml` 负责。

### 7.4 方向布局

`SegmentedStackPanel` 只累计可见 `AbstractSegmentedItem`。隐藏 item 不贡献期望尺寸，也不获得有效排列槽。

水平自然布局沿 X 轴累加 item 的 `DesiredSize.Width`，期望高度取可见 item 最大高度；排列时保持每个 item 的自然宽度。

垂直自然布局沿 Y 轴累加 item 的 `DesiredSize.Height`，期望宽度取可见 item 最大宽度；排列时每个 item 使用 Panel 的最终宽度和自身自然高度，从而保证纵向轨道内 item 与滑块全宽一致。

### 7.5 Expanding 布局

水平 expanding 先统计可见 `AbstractSegmentedItem` 数量。可见数量为 `0` 时退化为空尺寸；可用宽度有限时，每项的测量和排列宽度为：

可见数量大于 `0` 时，每个 item 的可用宽度为：

```text
availableSize.Width / visibleItemCount
```

可用宽度无限时，Panel 退化为水平自然测量，不能以无限值参与除法或返回错误的零宽结果；获得有限最终宽度后再执行等分排列。

垂直 `IsExpanding=true` 不执行高度等分。根主题只把控件水平对齐切换为 stretch，Panel 继续使用垂直自然高度，并把 item 排列为最终轨道宽度。

### 7.6 SizeType 与 Shape 分支

根主题按 `SizeType` 设置根圆角和选中滑块圆角。item 主题按 `SizeType` 设置 item 圆角、字体、最小高度、padding 和图标尺寸。

`Custom` 当前进入 Middle 分支，作为自定义尺寸的默认基线。实例显式设置的 `Padding`、`MinHeight`、`FontSize`、`CornerRadius` 等属性按 Avalonia setter 优先级覆盖主题默认值。

`Shape=Round` 是 SizeType 之后的最终圆角覆盖层。根主题把轨道和选中滑块圆角设置为足够大的固定值，item 主题对容器使用相同值，由 Avalonia 的圆角几何按实际 Bounds 形成胶囊。Shape 不改变 padding、MinHeight、字体、图标、颜色或 motion。

## 8. 资源、性能与 AOT 边界

Segmented 不依赖运行时反射或动态成员访问。主题协作通过固定模板节点、显式控件类型、Token resource 和 Avalonia property binding 完成。

资源与生命周期边界：

- `SegmentedToken` 通过 token generator 注册，Theme 通过 `SegmentedTokenResource` 使用。
- `SegmentedStackPanel.Orientation` 和 `IsExpanding` 在 AXAML 中绑定到最近的 `Segmented` ancestor。
- 容器的 `SizeType`、`Shape` 和 `IsMotionEnabled` 是生成容器与 owner 的固定关系，生命周期由容器准备和 Avalonia 绑定系统管理。
- `SelectionChanged` 订阅必须在 attach/detach 中成对管理。
- 选中滑块动画只在 `IsMotionEnabled=true` 时通过 transitions 启用。

方向和 Shape 不新增 Visual、缓存、timer、subscription 或运行时对象图。Panel 的 measure/arrange 仍为 O(n)，Shape 只增加一个 owner-to-container AvaloniaProperty 绑定；滑块继续使用值类型的 `Point` 和 `Size`，不拆分为方向专用 motion 对象。

AOT 边界：

- 不新增字符串反射、动态类型扫描、`Activator.CreateInstance(Type)` 或运行时属性名访问。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护。
- 主题内数据模板使用显式 `x:DataType`。

## 9. 维护不变量

内部重构必须保持以下不变量：

- `SelectionMode=Single` 只能在明确授权时改变。
- 模板应用时不能覆盖已绑定或已显式设置的选择。
- 默认选择第一个 item 的行为只能在没有任何选择输入时触发。
- `SelectionChanged` 订阅和解除必须配对。
- item pointer release 选择路径必须避免重复处理 handled 事件。
- `Orientation` 默认保持 `Horizontal`，`Shape` 默认保持 `Default`。
- 生成容器必须接收 owner 的 `SizeType`、`Shape` 和 `IsMotionEnabled`。
- 水平 `IsExpanding` 只能按可见 `AbstractSegmentedItem` 计数；垂直模式不能扩展父容器高度。
- 选中滑块矩形必须跟随当前选中容器最终的 `Bounds` 布局结果。
- 键盘导航必须首尾循环并跳过 disabled 和 hidden item。
- Round 必须覆盖所有 SizeType 圆角，但不能改变其他尺寸、颜色、状态或模板契约。
- 根 render 绘制和 item 主题状态不能互相替代；轨道/滑块在根，item 状态在 item。
- `Custom` 尺寸分支默认基线保持 Middle，除非获得 API/主题契约变更授权。

## 10. 测试与验证

验证范围：

- Segmented 选择测试：绑定选择保留、显式选择保留、默认选择、Form value、四方向键循环和 disabled/hidden 跳过。
- Segmented 布局测试：横向/纵向自然布局、水平 expanding、无限约束退化、垂直宽度适配、动态方向切换和滑块 Bounds。
- Segmented 主题测试：Orientation/Shape 属性默认值和传递、Round 对根/item/thumb 的最终圆角覆盖、各 SizeType 与 Custom 组合。
- `CustomizableSizeTypeContractTests`：`AbstractSegmented`、`Segmented`、`AbstractSegmentedItem`、`SegmentedItem` 支持 `CustomizableSizeType`。
- `SegmentedShowCasePageTests`：Gallery 页面结构、示例快照、动态选项追加状态和源码片段。
- 修改布局或选择行为时运行 `tests/AtomUI.Desktop.Controls.Tests` 中 Segmented 相关测试，并按影响范围扩大到完整 Desktop 控件测试。
- 修改 Gallery 示例时运行 `tests/AtomUIGallery.Tests` 中 Segmented 相关测试。
- 文档改动运行 `git diff --check`，并检查相对链接存在。
