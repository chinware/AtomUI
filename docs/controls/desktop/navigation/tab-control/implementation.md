# TabControl 桌面版实现原理

本文档描述 TabControl 桌面版的内部实现范围、源码职责、状态流、生命周期、资源边界和维护规则。公共设计与 API 契约见 [TabControl 桌面版架构设计](overview.md)，变化记录见 [TabControl Changelog](changelog.md)。涉及控件 Token 的实现应同时阅读 [TabControl Token 设计](token.md)。

## 1. 实现定位

本文档覆盖 TabControl 的控件实现、主题接入、状态同步和 Gallery 可见维护边界。具体属性注册、默认值、绘制细节和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构和不变量。

## 2. 源码文件结构

主要源码文件：

- `src/AtomUI.Desktop.Controls/TabControl`：14 个文件，代表文件 `BaseOverflowMenuItem.cs`、`BaseTabControl.cs`、`BaseTabScrollViewer.cs`、`CardTabControl.cs`、`ScrollContentPresenterReflectionExtensions.cs` 等。
- `src/AtomUI.Desktop.Controls/TabControl/TabStrip`：6 个文件，代表文件 `BaseTabStrip.cs`、`CardTabStrip.cs`、`TabStrip.cs`、`TabStripItem.cs`、`TabStripOverflowMenuItem.cs` 等。
- `src/AtomUI.Desktop.Controls/TabControl/Themes`：19 个文件，代表文件 `BaseOverflowMenuItemTheme.axaml`、`BaseTabControlTheme.axaml`、`BaseTabControlTheme.cs`、`BaseTabItemTheme.axaml`、`BaseTabItemTheme.cs` 等。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。
- Tab 拖动排序属于 TabControl 家族的集合与选择协作路径；实现应落在 `BaseTabControl`、Tab item 容器、滚动视口和内部拖动协作对象之间，不能把排序状态散落到 Gallery、theme 或业务数据对象中。
- 垂直页签图标对齐属于 TabControl 家族的 owner 级布局状态；`Left` / `Right` placement 下由 owner 统一判断同组是否存在图标，再把内部保留图标槽状态投射到 item container，不能通过 Gallery 手工补空图标或新增 public API。
- 默认 Line Tab 的 `Left` / `Right` placement 应保持紧凑的垂直节奏，减少无意义高度浪费；相邻间距和 item 自身垂直 padding 都应按 Line 紧凑模型处理。Card Tab 使用独立 `CardGutter` 和 Card padding 视觉节奏，本规则不得改变 Card 外观。

## 3. 核心类职责

- `BaseOverflowMenuItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `BaseTabControl`：控件核心或内部协作类型，维护 public surface、选择状态、内容页状态、关闭流程和拖动排序提交路径。
- `BaseTabControlTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `BaseTabItemTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `BaseTabScrollViewer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `BaseTabStrip`：控件核心或内部协作类型，维护页签条选择、滚动、overflow 和拖动排序交互。
- `BaseTabStripItemTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `BaseTabStripTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `CardTabControl`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `CardTabStrip`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TabControl`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TabControlOverflowMenuItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `TabControlScrollViewer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TabControlToken`：控件 Token scope，负责从全局 token 派生控件语义变量。
- `TabItem`：集合项、节点或容器类型，承载单项选择、关闭、拖动源和插入目标状态。
- `TabItemData`：数据、状态或行为协作类型，维护集合同步和事件路径。
- `TabScrollContentPresenter`：模板协作类型，承载内容展示、宿主或视觉边界。
- `TabStrip`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TabStripItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `TabStripOverflowMenuItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `TabStripScrollViewer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TabsContainerPanel`：布局面板，负责测量、排列、虚拟化或集合内容布局。

核心协作规则：

- 控件实例是 public API 和运行时状态 owner。
- Template part 是视觉协作对象，生命周期必须受 `OnApplyTemplate` 或模板加载流程管理。
- 数据对象、选项对象、任务对象或节点对象只保存业务数据，不应反向持有不可释放的视觉对象。
- 弹层、窗口、计时器、异步 loader 和全局管理器必须有明确关闭、解绑或释放路径。
- 拖动排序临时状态只属于控件实例当前交互会话；拖动 item、原 index、目标 index、临时 transform、临时绘制层级和 pointer capture 必须在提交、取消、capture lost、template reapply 或 detach 时统一释放。

## 4. 状态与数据流

TabControl 的状态流遵循下面路径：

```text
Public API / ItemsSource / Command / Event
  -> 控件实例状态
  -> internal state / effective state / pseudo-class
  -> template part property / AXAML selector
  -> renderer / popup / adorner / Gallery observable behavior
```

源码中的状态入口按以下语义维护：

- 内容与数据：`CloseIcon`、`ContentPadding`、`ContentTemplate`、`HeaderEndEdgePadding`、`HeaderEndExtraContent`、`HeaderEndExtraContentTemplate`、`HeaderStartEdgePadding`、`HeaderStartExtraContent`、`HeaderStartExtraContentTemplate`、`HorizontalContentAlignment` 等 15 项。
- 选择与集合：`IsSelected`、`IsTabReorderEnabled`、`TabActivationTrigger`、`SelectedIndex`、`SelectedItem`、`ItemsSource`。
- 交互与状态：`IsAutoHideCloseButton`、`IsClosable`、`IsMotionEnabled`、`IsShowAddTabButton`、`IsTabAutoHideCloseButton`、`IsTabClosable`。
- 视觉与布局：`SizeType`、`TabAlignmentCenter`、`TabStripPlacement`。
- 其他稳定入口：`AddTabButton`、`TabScrollViewer`。

维护要求：

- 外部设置的 Avalonia 属性必须在模板应用前后保持一致。
- 集合、选择、展开、过滤、分页、上传任务或异步 loader 必须能处理 reset、replace 和 clear。
- 伪类和 internal state 必须从单一 owner 推导，避免双向同步导致循环更新。
- overview.md 的 API 契约说明应与源码实际状态流一致。
- Pointer 选择状态流必须按 `TabActivationTrigger` 收敛。`PointerReleased` 是默认值，按下阶段只记录候选 Tab、pointer 和起点，释放时确认仍是同一个 Tab 且未进入拖动排序后再提交选择；`PointerPressed` 模式在按下阶段直接通过统一选择入口提交选择。
- 候选激活状态属于一次 pointer 会话，必须在 pointer released、capture lost、template reapply、detach、控件禁用或进入 reorder 时释放；不得保存在 item container、theme 或业务数据对象中。
- 拖动排序状态流必须按 `IsTabReorderEnabled` -> pointer threshold -> Chrome-like live reorder preview -> `TabReordering` -> 逻辑集合 move -> selection/content/overflow recompute -> `TabReordered` 收敛。
- 拖动过程中只能更新被拖 Tab 和兄弟 Tab 的临时 `RenderTransform`、候选目标 index 与自动滚动请求；被拖 Tab 必须被限制在当前 Tab 轨道主轴内移动，释放前不得实时移动 `ItemsSource`、`Items` 或 visual children，避免集合通知、选择状态和 container recycle 多次抖动。
- 排序提交后选中状态按逻辑 item 重新计算，`SelectedContent`、content presenter、选中指示条、close button 可见性和 overflow 菜单都从同一个集合顺序派生。
- overflow 菜单项必须从对应 `TabItem` 成对复制 `Header` 与 `HeaderTemplate`，使自定义标题模板在主标签和溢出菜单中保持同一呈现语义；不能只复制数据对象后依赖 `ToString()` 回退。
- 垂直图标槽状态必须从 `TabStripPlacement`、同组 item 的 `HasIcon` 和容器生成状态单向推导：`Top` / `Bottom` 保持紧凑布局，不默认保留图标槽；`Left` / `Right` 中只要同一 owner 下任一 Tab 有图标，全部 Tab item 都保留同宽图标槽，未配置图标的 item 渲染空槽而不是伪造图标。
- 图标槽保留状态是内部模板状态，不属于 public API、业务数据或 Token。它应随 item icon 变化、placement 变化、ItemsSource reset/replace/clear、container prepare/clear 和 template reapply 重新计算。
- `TabStripPlacement` 变化是纯布局变化，必须保留当前 `SelectedItem` / `SelectedIndex` 语义，不得为了更新方向重建 item containers；否则直接作为 `TabItem` 的容器会把旧 `IsSelected` 容器状态反向写回 owner selection。
- 默认 Line Tab 垂直 spacing 和垂直 item padding 只由默认 `TabControl` / `TabStrip` theme 消费，Card theme 继续使用 `CardGutter` 与 `VerticalItemPadding`；不要通过全局修改 Card token 或 Card theme 来修正 Line 布局。

## 5. 生命周期与模板接入

生命周期规则：

- 构造阶段只注册必要状态，不依赖 template part。
- 模板应用时获取 part、建立事件订阅和绑定，并先释放旧 part 订阅。
- 控件卸载、弹层关闭、窗口关闭、集合替换或 container recycle 时释放事件订阅和资源宿主。
- DynamicResource、TokenResourceBinder 或 C# binding 必须有明确 owner 和释放点。
- Browser 和 Desktop 宿主下的主题加载顺序不得影响 public API 语义。
- `PointerReleased` 激活候选项不依赖 template part；模板重套用、detach 或 container recycle 时必须清理候选 Tab 和 pointer，避免旧容器在下一次释放事件中被错误激活。
- 拖动排序获得 pointer capture、应用临时 transform、订阅 pointer move/release 或启动边缘自动滚动时，必须在 pointer released、capture lost、cancel、collection reset、template reapply、detach 中走同一释放路径。
- 模板重套用后不得复用旧 `TabItem`、旧 scroll viewer、旧 transform、旧 z-index 或旧拖动会话状态；新的模板只从 public state 和当前集合重新生成可观察状态。
- 图标槽对齐状态必须由 owner 在模板接入和容器生命周期中统一同步。`TabItem` 只消费内部保留图标槽状态；容器回收或重新准备时必须清理旧 item 的图标槽状态，避免上一组带图标页签影响下一组无图标页签。

稳定 template part 接入点：

- `PART_AddTabButton`：承载用户触发入口、导航或关闭动作。
- `PART_AlignWrapper`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_CardTabStripScrollViewer`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_ItemCloseButton`：承载用户触发入口、导航或关闭动作。
- `PART_ItemsPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_ScrollEndEdgeIndicator`：展示指示器、进度、分页或状态反馈。
- `PART_ScrollMenuIndicator`：展示指示器、进度、分页或状态反馈。
- `PART_ScrollStartEdgeIndicator`：展示指示器、进度、分页或状态反馈。
- `PART_SelectedItemIndicator`：展示指示器、进度、分页或状态反馈。
- `PART_TabsContainer`：稳定模板协作入口，重命名前必须同步主题和实现。

## 6. 交互与事件处理

TabControl 的交互事件应从输入源收敛到控件级语义事件：

- Pointer、keyboard、focus 和 command 事件不应绕过 Avalonia 基础控件语义。
- 没有弹层职责的路径不应引入额外 popup 或全局输入捕获。
- 非集合控件不应通过隐藏集合状态模拟业务数据。
- 值提交或命令触发必须保持继承控件的事件顺序。
- Pointer 触发选择由 `TabActivationTrigger` 决定，`PointerReleased` 默认要求 press/release 命中同一个 Tab；实现应在 owner 控件统一判断，不在 `TabItem` 中直接修改 `SelectedIndex`。
- 键盘导航、focus directional navigation、access key 和程序化选择不受 `TabActivationTrigger` 影响。
- 拖动排序只响应可拖动 Tab item 的主按钮拖动；关闭按钮、添加按钮、`HeaderStartExtraContent`、`HeaderEndExtraContent`、overflow 菜单项和内容区域不得成为 reorder target。
- `TabStripPlacement=Top/Bottom` 时排序主轴为 X 轴，`TabStripPlacement=Left/Right` 时排序主轴为 Y 轴。被拖 Tab 只能沿主轴移动：Top/Bottom 的 Y 位移为 0，Left/Right 的 X 位移为 0；兄弟 Tab 让位和目标 index 也只能由主轴计算。
- 真实 Tab 视口靠近边缘时允许自动滚动以暴露更多排序目标；overflow 菜单只用于导航和选择，不承载拖动排序。

稳定事件路径包括 `AddTabRequest`、`CloseTab`、`Closed`、`Closing`、`TabReordering`、`TabReordered`。事件参数和触发时机属于兼容边界。

## 7. 内部算法与关键流程

维护者需要重点关注以下流程：

- API 默认值到 effective state 的归一。
- Template part 重新应用时的状态回放。
- 主题资源、Token 和 SharedToken 计算后的视觉更新。
- 内容、命令和视觉状态在模板节点之间的同步。
- 动效启停、初始加载阶段 transition 抑制和卸载取消。
- 激活触发：`PointerPressed` 模式直接在 press 阶段触发选择；`PointerReleased` 模式在 press 阶段记录候选项，release 阶段校验同一 pointer、同一 Tab、指针仍在 Tab bounds 内且未进入 reorder 后触发选择。
- 取消激活：press 后移动到其他 Tab、移出当前 Tab、capture lost、控件 detach、模板重套用、进入拖动排序或源 item 被删除时清理候选激活状态，不提交选择。
- 拖动开始：记录逻辑 item、原 index、pointer 起点和当前 `TabStripPlacement` 主轴；超过平台拖动阈值才进入排序态。
- 目标计算：只遍历真实可见的 Tab item 容器，使用被拖 Tab 的前进边缘判断是否跨过被覆盖兄弟 Tab 主轴中线；向后拖动使用 trailing edge，向前拖动使用 leading edge，header extra、add button、close button、overflow menu 和非 Tab 容器不进入候选集合。
- Chrome 式预览：拖动进入 active reorder 后，源 Tab 使用主轴 pointer 偏移量作为临时 transform 并提高绘制层级，非主轴位移保持为 0；兄弟 Tab 是否让位必须由同一个目标 index 阈值决定，不能按任意重叠距离提前移动。源 Tab 前进边缘跨过被覆盖兄弟 Tab 主轴中线后，目标区间内的兄弟 Tab 必须按相邻真实 layout slot 的主轴起点差值平移到前后相邻槽位，不能只按源 Tab 尺寸位移，因为 Line/Card 的 gutter 和可变宽度也属于 layout slot；位移使用短时过渡避免位置瞬移；目标 index 回退时，已让位兄弟 Tab 应沿同一 preview transform 动画归位，不能在拖动会话中直接清理原始 transform；半宽或半高阈值前兄弟 Tab 保持原位，拖动中不绘制插入线，不提交集合 move。
- 选中指示条：`PART_SelectedItemIndicator` 的位置以选中容器 layout bounds 为基础，并在 active reorder 期间叠加选中容器当前预览 transform 的主轴位移；选中源 Tab 和被让位的选中兄弟 Tab 都必须跟随视觉预览位置。当前被拖 Tab 同时也是选中 Tab 时，拖动会话内必须临时禁用 `SelectedIndicatorRenderTransform` transition；释放提交后要把 selection/bounds/scroll 触发的中间 indicator 刷新延迟到最终 layout 完成，再在同一清理路径恢复 transition，避免指示条先跳到旧布局再回到正确位置。
- 拖动源视觉：源 Tab 必须在拖动期间使用不透明背景，避免覆盖兄弟 Tab 时文字、图标或边框叠穿。Line 模式使用当前激活面背景，Card 模式使用卡片激活面背景；hover、pressed、selected 与 drag state 的颜色过渡必须继续走主题 transition。
- 集合提交：`ItemsSource` 可写且实现 `IList` 时移动 source list；未设置 `ItemsSource` 时移动 `Items`；只读、固定大小或不可写 source 不提交 reorder，并清理临时视觉状态。
- 事件顺序：释放时先触发可取消的 `TabReordering`；未取消且集合 move 成功后重新计算选择与内容，再触发 `TabReordered`。
- 异常边界：拖动期间集合 reset、item 被删除、控件禁用或模板失效时取消当前排序，不吞异常、不延迟强刷，也不把旧 index 当作可靠状态。
- 垂直图标槽计算：owner 只扫描当前有效 Tab item 容器或对应逻辑 item 的图标状态，得到同组 `HasAnyIconInVerticalPlacement` 语义后下发内部状态；主题结构应统一为稳定的 `IconSlot` + `ContentPresenter` + `CloseButton` 顺序。图标槽宽度沿用现有 `IconSize` / `IconSizeSM` 和 `ItemIconMargin` 语义，不新增 Token；无图标 item 的 `IconSlot` 保持占位但不显示内容。
- Placement 切换流程：owner 更新 pseudo-class、header padding、现有 container 的 `TabStripPlacement` 和内部布局状态即可；不得调用 `RefreshContainers()` 作为布局刷新手段。

实现文档不逐行解释私有方法。若某个私有算法成为稳定维护入口，应在本节补充算法不变量，而不是把代码复述为说明书。

## 8. 资源、性能与 AOT 边界

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。
- 拖动 move 帧内只更新轻量 transform、目标 index 和自动滚动请求；主轴约束和目标 index 计算必须是纯几何计算，不得在 pointer move 中反复移动集合、重建 item 容器或重新应用模板。
- 拖动预览 transform、绘制层级、计时器和订阅应按交互会话缓存并在会话结束释放；不得因一次拖动永久保留视觉对象或数据 item。
- 拖动排序不得引入运行时反射、动态类型扫描或 AOT 不友好的事件发现路径。
- 图标槽对齐不得为无图标 Tab 创建额外图标控件、动态占位对象或 C# 运行时模板分支；应复用静态 AXAML 槽位、现有资源绑定和内部布尔状态，避免增加模板实例化和 container recycle 成本。

## 9. 维护不变量

维护 TabControl 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- 拖动排序释放时必须修改逻辑集合顺序，拖动中允许用 `RenderTransform` 和临时 `ZIndex` 做实时视觉预览，但不能只调整 `Panel.Children`、`ZIndex` 或 transform 作为最终排序结果。
- 选中项必须跟随同一个逻辑 item，不能跟随旧 index；重排后内容页、指示条、overflow 菜单和关闭状态必须从新顺序统一推导。
- `TabActivationTrigger` 只能改变 pointer 激活提交时机，不能改变键盘选择、access key、关闭后选择、程序化选择或拖动排序后的选中项回放语义。
- `PointerReleased` 候选激活状态必须由控件 owner 持有并按 pointer 会话释放，不能让旧 `TabItem` 或旧 pointer 引用跨 template reapply / detach 存活。
- 所有拖动临时状态必须在提交、取消、capture lost、template reapply 和 detach 时释放，不能保留旧容器或旧 adorner。
- 垂直图标槽对齐不能改变 `Top` / `Bottom` 的紧凑布局；不能新增 public API、Token 或 Gallery-only workaround；`TabControl`、`TabStrip`、`CardTabControl` 和 `CardTabStrip` 的同组混合有图标/无图标布局必须使用同一套 owner 推导规则。
- 默认 Line Tab 的 `Left` / `Right` spacing / padding 调整不得影响 Card Tab、拖动排序阈值、选中指示条定位或 overflow 计算；选中指示条高度必须继续跟随 Line item 的真实 bounds。
- 切换 `TabStripPlacement` 后当前选中项必须继续跟随同一个逻辑 item，不能因 container 重新准备或旧 `IsSelected` 状态回流而改变。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。

## 10. 测试与验证

推荐验证：

- 纯文档改动运行 `git diff --check` 并检查相对链接。
- 控件 API 或行为变更运行对应 `tests/AtomUI.Desktop.Controls.Tests` 或专用包测试。
- Tab 激活触发变更需覆盖默认 `PointerReleased`、`PointerPressed`、press/release 同 Tab 激活、press 后移出不激活、press A release B 不激活、键盘选择不受影响，以及拖动排序释放不触发额外激活。
- Tab 拖动排序变更需覆盖 Top/Bottom 横向排序、Left/Right 纵向排序、选中 item 跟随、可写 `ItemsSource`、未设置 `ItemsSource`、只读 source 不提交、`TabReordering` 取消、overflow 边缘自动滚动、关闭/添加/extra 区域排除、template reapply 与 detach 释放。
- overflow 菜单呈现需覆盖自定义 `HeaderTemplate` 场景，验证生成菜单项的 `Header` / `HeaderTemplate` 与源 `TabItem` 一致。
- 垂直图标槽对齐变更需覆盖 `Left` / `Right` 下同组混合图标与无图标 Tab 的文本起点一致、全部无图标时不额外占位、`Top` / `Bottom` 保持紧凑、Line/Card 两类主题一致，以及 icon/placement/items 变化和 container recycle 后状态不串组。
- 默认 Line 垂直 spacing / padding 变更需覆盖 `TabControl` / `TabStrip` 在 `Left` / `Right` 下的相邻 container 主轴间距和 item 高度，并明确 Card theme 不被本规则修改。
- `TabStripPlacement` 行为变更需覆盖直接 `TabItem` 与数据 item 场景，确保切换 `Top` / `Right` / `Bottom` / `Left` 后 `SelectedItem` 不变。
- DataGrid 相关变更运行 `tests/AtomUI.Desktop.Controls.DataGrid.Tests`。
- Gallery 示例或源码片段变更运行 `tests/AtomUIGallery.Tests`。
- AOT、生成器或动态数据路径变更按 Gallery NativeAOT 发布流程验证。
