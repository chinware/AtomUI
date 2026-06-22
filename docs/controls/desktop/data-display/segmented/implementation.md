# Segmented 桌面版实现原理

本文档描述 Segmented 桌面版的共享基类、桌面封装、容器准备、选择流、选中滑块渲染、expanding 布局和维护边界。公共设计与 API 契约见 [Segmented 桌面版架构设计](overview.md)，Token 语义见 [Segmented Token 设计](token.md)，变化记录见 [Segmented Changelog](changelog.md)。

## 1. 实现定位

Segmented 的实现重点是把 Avalonia `SelectingItemsControl` 的单选状态、生成容器、item pointer 交互和根控件 render 层的选中滑块统一到一个稳定状态流中。

本文档只描述 Segmented 相关实现结构，不重新说明 `SelectingItemsControl`、Avalonia 模板系统、Token 系统或 `Panel` 测量排列的通用机制。

## 2. 源码文件结构

共享源码：

- `src/AtomUI.Controls/Segmented/AbstractSegmented.cs`：共享根控件，定义公共属性、内部滑块属性、选择生命周期、容器准备、Form 接口和 render 绘制。
- `src/AtomUI.Controls/Segmented/AbstractSegmentedItem.cs`：共享 item，定义 `IsSelected`、`Icon`、内部 `SizeType` / `IsMotionEnabled`、图标伪类和 pointer release 选择。
- `src/AtomUI.Controls/Segmented/SegmentedStackPanel.cs`：内部 items panel，执行普通排列和 expanding 等分排列。
- `src/AtomUI.Controls/Segmented/SegmentedPseudoClass.cs`：Segmented 专用伪类常量。

桌面源码：

- `src/AtomUI.Desktop.Controls/Segmented/Segmented.cs`：桌面公开根控件，注册 Token scope，创建 `SegmentedItem` 容器。
- `src/AtomUI.Desktop.Controls/Segmented/SegmentedItem.cs`：桌面公开 item，注册 Token scope。
- `src/AtomUI.Desktop.Controls/Segmented/SegmentedToken.cs`：Segmented 组件 Token。
- `src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedTheme.axaml`：根模板、轨道、滑块、SizeType 和 motion 样式。
- `src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedItemTheme.axaml`：item 模板、状态样式、SizeType 和图标样式。
- `src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedThemes.axaml`：主题资源聚合入口。

Gallery 和测试：

- `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented/`：Segmented 示例、API 表、Token 表和本地化文案。
- `tests/AtomUI.Desktop.Controls.Tests/Segmented/SegmentedSelectionInitializationTests.cs`：选择初始化、Form value 和 expanding 布局回归测试。
- `tests/AtomUI.Desktop.Controls.Tests/SizeType/CustomizableSizeTypeContractTests.cs`：`ICustomizableSizeTypeAware` 契约测试。
- `tests/AtomUIGallery.Tests/ShowCases/SegmentedShowCasePageTests.cs`：Gallery 页面结构和示例快照测试。

## 3. 核心类职责

`AbstractSegmented` 是状态协调器。它持有选择属性、内部选中滑块属性和 Form value 适配逻辑，负责在模板应用、尺寸变化和选择变化后同步选中滑块矩形。

`Segmented` 是桌面公开控件。它不重复共享选择逻辑，只注册 `SegmentedToken.ScopeProvider` 并为数据 item 创建 `SegmentedItem` 容器。

`AbstractSegmentedItem` 是 item 基类。它接入 `ISelectable`，维护 `Icon` 到 `:has-icon` 的伪类同步，并在鼠标左键释放时请求 owner 更新选择。

`SegmentedItem` 是桌面公开 item。它不添加额外行为，只注册 Segment 专属 Token scope。

`SegmentedStackPanel` 是布局实现。它只认识可见的 `AbstractSegmentedItem`，普通模式使用自然宽度，expanding 模式按可见 item 数量等分宽度。

## 4. 状态与数据流

容器准备流：

```text
Items / ItemsSource
  → CreateContainerForItemOverride()
  → SegmentedItem
  → PrepareContainerForItemOverride()
  → content/template sync
  → SizeType + IsMotionEnabled binding to owner
  → PrepareSegmentedItem()
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

- `IsExpanding` 和 `SizeType` 影响 measure。
- 选中滑块相关内部属性影响 render。
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
- `OnSizeChanged()` 重新同步选中滑块矩形。

## 6. 交互与事件处理

Segmented 的专用 pointer 交互在 item 层处理：

- `AbstractSegmentedItem.OnPointerReleased()` 只处理未被标记 handled 的事件。
- 当前 item 必须能找到 `AbstractSegmented` owner。
- 只有鼠标左键释放会立即请求 owner 更新选择。
- owner 的 `ShouldTriggerSelection()` 同样把鼠标左键释放识别为选择触发点。

键盘、焦点和 disabled 语义沿用 Avalonia `SelectingItemsControl`、`ISelectable`、`Focusable` 和标准伪类。Segmented 当前没有自定义键盘导航算法、命令、弹层或异步数据加载路径。

## 7. 内部算法与关键流程

### 7.1 容器创建和数据项承载

`Segmented.CreateContainerForItemOverride()` 始终返回 `SegmentedItem`。当 item 不是 `Visual` 时，`PrepareContainerForItemOverride()` 把 item 设置为容器 `Content`，并在存在 `ItemTemplate` 时把 owner 的 `ItemTemplate` 绑定给容器 `ContentTemplate`。

容器准备阶段还会把 owner 的 `SizeType` 和 `IsMotionEnabled` 绑定给 item，确保生成 item 与根控件使用同一尺寸和 motion 分支。

### 7.2 选中滑块矩形

`SetupSelectedThumbRect()` 只在 `SelectedItem` 非空且能找到对应容器时更新滑块：

```text
container = ContainerFromItem(SelectedItem)
offset = container.TranslatePoint((0, 0), owner)
SelectedThumbPos = offset
SelectedThumbSize = container.DesiredSize
```

滑块矩形是 render 输入，不是 visual tree 中的独立控件。修改容器测量、排列或 selection 时序时，必须同时验证 `SelectedThumbPos` 和 `SelectedThumbSize`。

### 7.3 根控件 Render

`AbstractSegmented.Render()` 绘制两层矩形：

1. 使用 `Background` 和根 `CornerRadius` 绘制轨道背景。
2. 使用 `SelectedThumbBg`、`SelectedThumbCornerRadius` 和 `SelectedThumbBoxShadows` 绘制选中滑块。

item 本身的背景和文本状态仍由 `SegmentedItemTheme.axaml` 负责。

### 7.4 普通布局

`SegmentedStackPanel.MeasureOverrideNoExpanding()` 使用无限宽度测量可见 `AbstractSegmentedItem`，总宽度为各 item `DesiredSize.Width` 之和，高度为可见 item 最大高度。

`ArrangeOverrideNoExpanding()` 从 `offsetX=0` 开始按 item `DesiredSize.Width` 顺序排列可见 item。

### 7.5 Expanding 布局

`SegmentedStackPanel.MeasureOverrideExpanding()` 先统计可见 `AbstractSegmentedItem` 数量。可见数量为 `0` 时返回可用宽度和 `0` 高度；可用宽度为无限时宽度返回 `0`。

可见数量大于 `0` 时，每个 item 的可用宽度为：

```text
availableSize.Width / visibleItemCount
```

`ArrangeOverrideExpanding()` 使用 `finalSize.Width / visibleItemCount` 作为每个可见 item 的排列宽度。隐藏 item 不测量为有效宽度，也不参与排列。

### 7.6 SizeType 分支

根主题按 `SizeType` 设置根圆角和选中滑块圆角。item 主题按 `SizeType` 设置 item 圆角、字体、最小高度、padding 和图标尺寸。

`Custom` 当前进入 Middle 分支，作为自定义尺寸的默认基线。实例显式设置的 `Padding`、`MinHeight`、`FontSize`、`CornerRadius` 等属性按 Avalonia setter 优先级覆盖主题默认值。

## 8. 资源、性能与 AOT 边界

Segmented 不依赖运行时反射或动态成员访问。主题协作通过固定模板节点、显式控件类型、Token resource 和 Avalonia property binding 完成。

资源与生命周期边界：

- `SegmentedToken` 通过 token generator 注册，Theme 通过 `SegmentedTokenResource` 使用。
- `SegmentedStackPanel.IsExpanding` 在 AXAML 中绑定到最近的 `Segmented` ancestor。
- 容器的 `SizeType` 和 `IsMotionEnabled` 是生成容器与 owner 的固定关系，生命周期由容器准备和 Avalonia 绑定系统管理。
- `SelectionChanged` 订阅必须在 attach/detach 中成对管理。
- 选中滑块动画只在 `IsMotionEnabled=true` 时通过 transitions 启用。

AOT 边界：

- 不新增字符串反射、动态类型扫描、`Activator.CreateInstance(Type)` 或运行时属性名访问。
- Gallery API/Token 表使用显式 ViewModel 数据。
- 主题内数据模板使用显式 `x:DataType`。

## 9. 维护不变量

内部重构必须保持以下不变量：

- `SelectionMode=Single` 只能在明确授权时改变。
- 模板应用时不能覆盖已绑定或已显式设置的选择。
- 默认选择第一个 item 的行为只能在没有任何选择输入时触发。
- `SelectionChanged` 订阅和解除必须配对。
- item pointer release 选择路径必须避免重复处理 handled 事件。
- 生成容器必须接收 owner 的 `SizeType` 和 `IsMotionEnabled`。
- `IsExpanding` 只能按可见 `AbstractSegmentedItem` 计数。
- 选中滑块矩形必须跟随当前选中容器的实际布局结果。
- 根 render 绘制和 item 主题状态不能互相替代；轨道/滑块在根，item 状态在 item。
- `Custom` 尺寸分支默认基线保持 Middle，除非获得 API/主题契约变更授权。

## 10. 测试与验证

验证范围：

- `SegmentedSelectionInitializationTests`：绑定选择保留、显式选择保留、默认选择、Form value、expanding 可见 item 等分。
- `CustomizableSizeTypeContractTests`：`AbstractSegmented`、`Segmented`、`AbstractSegmentedItem`、`SegmentedItem` 支持 `CustomizableSizeType`。
- `SegmentedShowCasePageTests`：Gallery 页面结构、API 表、Token 表和示例快照。
- 修改布局或选择行为时运行 `tests/AtomUI.Desktop.Controls.Tests` 中 Segmented 相关测试，并按影响范围扩大到完整 Desktop 控件测试。
- 修改 Gallery 示例时运行 `tests/AtomUIGallery.Tests` 中 Segmented 相关测试。
- 文档改动运行 `git diff --check`，并检查相对链接存在。
