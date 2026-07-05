# TreeView 桌面版实现原理

本文档描述 TreeView 桌面版的内部源码结构、状态流转、容器生命周期、默认状态回放、过滤、勾选、异步加载和拖拽实现。公共设计与 API 契约见 [TreeView 桌面版架构设计](overview.md)，Token 语义见 [TreeView Token 设计](token.md)，变化记录见 [TreeView Changelog](changelog.md)。

## 1. 实现定位

TreeView 的实现目标是在 Avalonia `TreeView` 基类上增加 AtomUI 树形数据展示能力，并保持 API、行为和主题契约稳定。实现文档覆盖 `TreeView`、`TreeViewItem`、`TreeViewItemHeader`、`NodeSwitcherButton`、节点数据模型、interaction handler、过滤、拖拽、异步加载和主题接入。

TreeView 内部实现较多，按功能拆分 partial 文件。拆分边界服务于维护清晰度，不改变 public API 所在位置、属性定义顺序、事件语义和渲染结果。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/TreeView/TreeView.cs`：public API、事件、容器生成、生命周期、选择、空状态、Form 集成和公共方法。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.StateReplay.cs`：loaded 回放、ItemsSource 变化后的选择 / 勾选 / 展开状态恢复。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.PathTraversal.cs`：`TreeNodePath` 遍历、展开路径、容器确保和展开状态恢复。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.CheckedState.cs`：checkbox 严格 / 级联状态、`CheckedItems` 同步和半选父级计算。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.Filter.cs`：过滤、高亮、隐藏未命中、展开路径、filter context 备份和恢复。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.AsyncItemDataLoad.cs`：异步加载、加载合并、超时、取消和 `TreeItemLoaded` 派发。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.DragAndDrop.cs`：拖拽命中测试、drag preview、drop indicator、drop 操作和拖拽事件。
- `src/AtomUI.Desktop.Controls/TreeView/TreeViewItem.cs`：节点容器、展开收起、line 渲染、check/radio 状态、drag bounds 和数据节点承载。
- `src/AtomUI.Desktop.Controls/TreeView/TreeViewItemHeader.cs`：header 视觉状态、pointer 状态、switcher mode、filter highlight runs 和 template part 订阅。
- `src/AtomUI.Desktop.Controls/TreeView/NodeSwitcherButton.cs`：switcher 当前图标选择、默认图标、loading load request 和 rotation 动效。
- `src/AtomUI.Desktop.Controls/TreeView/TreeItemNode.cs`、`ITreeItemNode.cs`：轻量数据节点模型、节点契约和父子关系维护。绑定型节点模型属于同一节点数据职责边界，必须保持独立类型，不改造 `TreeItemNode`。
- `src/AtomUI.Desktop.Controls/TreeView/DefaultTreeViewInteractionHandler.cs`：pointer、context menu、radio group、checked changed 和 floating tree light dismiss。
- `src/AtomUI.Desktop.Controls/TreeView/TreeViewToken.cs`：TreeView Token 定义。
- `src/AtomUI.Desktop.Controls/TreeView/Themes/`：TreeView、TreeViewItem、TreeViewItemHeader、NodeSwitcherButton 主题和模板常量。

## 3. 核心类职责

`TreeView` 是树根控件，负责 public API、ItemsControl 容器生成、默认状态回放、选择、勾选集合、过滤入口、拖拽状态、异步加载入口、空状态和 Form 集成。

`TreeViewItem` 是节点容器，负责节点 icon、checked、loading、leaf、value、line render、展开收起动效和 drag bounds。它实现 `ITreeItemNode`，使直接 `TreeViewItem` 和数据节点场景能共享路径和层级逻辑。

`TreeViewItemHeader` 是节点 header 的视觉和交互边界，承载 switcher、checkbox / radio、icon、内容、filter highlighter、hover、pressed、selected、drag 和 disabled 状态。

`NodeSwitcherButton` 是展开、收起、loading 和 leaf 图标切换入口。它在未加载节点上触发 `NodeLoadRequestEvent`，由 TreeView 接管异步加载。

`DefaultTreeViewInteractionHandler` 处理 TreeView 级 pointer、右键上下文菜单、radio group、checked changed 和浮层关闭逻辑，使节点容器不直接持有全局输入订阅。

`TreeItemNode` 是轻量数据驱动节点模型，维护 `Children` 与 `ParentNode` 的一致性。它不继承 `AvaloniaObject`，不承载 `DynamicResource` 或 Avalonia binding target 语义。

`BindableTreeItemNode` 是绑定型数据节点模型，继承 `AvaloniaObject` 并实现 `ITreeItemNode`。它用于节点属性需要作为 Avalonia binding target 的场景，资源宿主生命周期由 `[GenerateScopedResourceHost]` 生成，owner TreeView / TreeViewItem 负责 attach/release。

## 4. 状态与数据流

容器生成流：

```text
Items / ItemsSource
      ↓
TreeView.CreateContainerForItemOverride
      ↓
TreeViewItem
      ↓
ContainerForItemPreparedOverride
  OwnerTreeView
  ApplyNodeData(ITreeItemNode)
  BindableTreeItemNode attach resource host + property sync
  ItemTemplate -> HeaderTemplate
  switcher icons
  filter / motion / hover / line / toggle / selectable state bindings
```

默认状态回放流：

```text
OnLoaded
  ConfigureDefaultSelectedPaths
  ConfigureDefaultCheckedPaths
  FilterTreeNode
  IsDefaultExpandAll ? ExpandAll(false) : ConfigureDefaultExpandedPaths
```

ItemsSource 变化恢复流：

```text
capture SelectedItem / SelectedItems / CheckedItems identity paths
clear current selection and checked collection
restore runtime selected path if possible
fallback to DefaultSelectedPaths
restore runtime checked paths if possible
fallback to DefaultCheckedPaths
restore expanded state or expand all
```

Form 值流：

```text
IFormItemAware.SetFormValue(value)
      ↓
SelectionMode.Multiple ? SelectedItems = value as IList : SelectedItem = value
      ↓
SelectedItem / SelectedItems class handler
      ↓
IFormItemAware.ValueChanged
```

TreeView 的 Form 适配层只投射 Avalonia 原生选择状态，不建立第二套选择值。单选模式写入和读取 `SelectedItem`，多选模式写入和读取 `SelectedItems`；写入时必须保留调用方传入的节点对象或 `IList` 实例，不能先转换为字符串。

勾选流：

```text
TreeViewItem.IsChecked changed
      ↓
DefaultTreeViewInteractionHandler.OnCheckedChanged
      ↓
Radio: RadioButtonGroupManager
CheckBox strict: sync current item only
CheckBox cascading: check / uncheck subtree + update parents
      ↓
CheckedItems + CheckedItemsChanged
```

过滤流：

```text
Filter / FilterValue / FilterStrategy
      ↓
expand realized tree without motion
      ↓
backup item filter context
      ↓
post-order match descendants
      ↓
highlight / hide / expand-path state
      ↓
FilterResultCount + empty indicator
```

异步加载流：

```text
NodeSwitcherButton.Toggle
  if HasTreeItemDataLoader && !AsyncLoaded
      raise NodeLoadRequestEvent
      ↓
TreeView.LoadNodeDataAsync
      ↓
ITreeItemNodeLoader.LoadAsync
      ↓
append result.Data to ITreeItemNode.Children
      ↓
TreeItemLoaded + expand target node
```

## 5. 生命周期与模板接入

`TreeView` 静态构造注册 drag/drop、TreeViewItem routed event class handler、filter class handler 和 Form value changed class handler。

`TreeView` 实例构造注册 TreeViewToken scope，并监听 root `Items.CollectionChanged`。`OnInitialized` 设置默认 filter、默认 filter value selector 和空状态。`OnAttachedToVisualTree` 挂接 interaction handler，`OnDetachedFromVisualTree` 解除 handler 并取消所有异步加载。`OnLoaded` 回放默认状态。

`TreeViewItem.OnApplyTemplate` 获取 `Header` 和 `PART_ItemsPresenterMotionActor`，配置叶子状态，并按当前 `IsExpanded` 同步子项实际可见状态。

`TreeViewItemHeader.OnApplyTemplate` 获取 `PART_HeaderContentFrame`、`PART_IconPresenter` 和 `PART_NodeSwitcherButton`。设置新 `PART_HeaderContentFrame` 前必须解除旧 frame 的 pointer 事件订阅，避免旧模板节点继续影响 hover 状态。

`NodeSwitcherButton.OnAttachedToVisualTree` 设置默认图标。`OnLoaded` 后再启用 transitions，避免初始化阶段产生非预期动画。

## 6. 交互与事件处理

TreeView pointer 流程：

- `OnPointerPressed` 在可选择节点上更新选择；开启拖拽时记录起始点并阻止 gesture recognition。
- `DefaultTreeViewInteractionHandler.PointerReleased` 在 header bounds 内触发 click、context menu 和可选的右键选中。
- `UpdateSelectionFromEvent` 根据 `IsSelectOnRightClick` 决定右键是否进入 Avalonia 选择流程。

展开收起：

- SwitcherButton 通过 `IsChecked` 双向绑定到 `TreeViewItem.IsExpanded`。
- `TreeViewItem.HandleExpandedChanged` 进入 `ExpandChildren` 或 `CollapseChildren`。
- 展开收起通过 `LayoutAwareMotionActor` 承载；全量展开、路径回放和内部状态同步可临时关闭 motion。

上下文菜单：

- 右键 header 时可以先确保当前项被选中。
- 打开新上下文菜单前关闭 TreeView 或祖先 TreeViewItem 上仍打开的旧 ContextMenu。

拖拽：

- `OnPointerMoved` 超过拖拽阈值后创建 drag preview。
- 拖动中持续更新 drag-over 节点和 drop indicator。
- `OnPointerReleased` 或 pointer capture lost 完成拖拽并清理 preview、drag-over 和 indicator 状态。

## 7. 内部算法与关键流程

### 7.1 Path 遍历

`TraverseTreeViewPath` 是默认状态回放和路径操作的统一入口。它按 `TreeNodePath.Segments` 从 root 到 leaf 查找节点，支持以下选项：

- 关闭 motion。
- 展开命中节点。
- 操作完成后恢复原展开状态。
- 执行 layout pass 以确保子容器生成。

路径段匹配优先使用 `TreeViewItem.ItemKey.Value`，未命中时使用 `TreeViewItem.Value?.ToString()`。

### 7.2 默认状态回放

默认状态回放按 selected、checked、filter、expanded 的顺序执行。这样选中和勾选可以在过滤和展开之前完成基础状态同步，最后由 `IsDefaultExpandAll` 或 `DefaultExpandedPaths` 决定可见展开结构。

ItemsSource 变化时，TreeView 先捕获运行期状态的节点身份路径，再清空当前 selection / checked 集合，随后按新数据源尝试恢复运行期状态，最后回退默认路径。

### 7.3 勾选级联

级联勾选需要临时展开未实现子树以确保子容器可访问，并在完成后恢复原展开状态。`CheckedItemsSyncScope` 用于防止内部批量更新 `CheckedItems` 时递归触发容器同步和重复事件。

父级状态按所有有效子节点计算：

- 全部有效子节点 checked：父级 true。
- 部分有效子节点 checked 或 indeterminate：父级 null。
- 无有效子节点 checked：父级 false。

### 7.4 过滤和高亮

过滤采用后序遍历。先计算子节点是否命中，再处理当前节点，从而支持“当前未命中但后代命中时保留路径”的场景。

首次进入过滤模式时，TreeViewItem 保存 filter context backup。清除过滤时递归恢复可见性、展开状态、高亮文本和 match 状态。

`TreeViewItemHeader.BuildFilterHighlightRuns` 根据 `FilterStrategy` 构造 `InlineCollection`，将命中片段设置为高亮前景和可选加粗。

### 7.5 异步加载

`AsyncExpandLoadCoordinator` 以节点引用作为 key 合并同一节点的并发加载请求，并应用 `AsyncLoadTimeout`。加载成功后，返回的子节点会更新 parent node，再加入目标节点 `Children`。

detached 时必须调用 `CancelAll`，避免已离开视觉树的 TreeView 继续处理加载结果。

### 7.6 拖拽和 drop

拖拽命中包含三类查询：

- self-first 查询用于确认拖拽发起节点。
- child-first 查询用于当前 drag-over 节点。
- offset-y 查询用于计算最终 drop 位置。

drop indicator 根据目标 header 上半区、中间区域和下半区决定插入到前、插入到内部或插入到后。执行 drop 前必须检查目标不是被拖拽节点自身或其后代。

### 7.7 绑定型节点同步

绑定型节点只解决节点属性作为 Avalonia binding target 的场景，不替代轻量 `TreeItemNode`。

同步规则：

- `ContainerForItemPreparedOverride` 识别 `BindableTreeItemNode` 后，先 attach scoped resource host，再执行普通 `ITreeItemNode` 数据转接，并建立节点到当前 `TreeViewItem` 的属性同步。
- 节点到容器同步覆盖 `Icon`、`IsEnabled`、`IsChecked`、`IsSelected`、`IsExpanded`、`IsIndicatorEnabled`、`GroupName`、`Value`、`IsLeaf` 和 `ItemKey` 等当前容器状态。`Header` 保持 Avalonia TreeView 的数据项语义，容器 `Header` 仍是节点对象本身；显示内容通过 `TreeDataTemplate` 绑定 `node.Header` 更新。
- 容器交互导致的 checked、selected、expanded 状态变化应回写绑定型节点，避免节点状态和当前容器状态分叉。
- 同步绑定必须进入容器生命周期 disposable。`ClearContainerForItemOverride`、detach、re-template 或 container recycle 时释放，不允许让旧容器继续订阅节点。容器清理还必须释放 `TreeDataTemplate` 建立的 children binding，避免回收容器继续持有旧节点。
- 同一个节点被多个容器短暂使用时，resource host attach 必须使用 attachment count 或等价 scoped token，释放到 0 后再解除 owner 订阅。

## 8. 资源、性能与 AOT 边界

TreeView 不通过反射访问模板内部结构。模板接入依赖稳定 part 名称和 Avalonia 属性绑定。

`BindableTreeItemNode` 属于 owner-managed 非 Visual `AvaloniaObject`。它承载 `DynamicResource`、token-resource binding 或 XAML binding target 时，必须按 [Scoped Resource Host Source Generator 范式](../../../../modules/generator/scoped-resource-host-generator.md) 生成 scoped `IResourceHost` / `IThemeVariantHost` 生命周期样板代码。资源查找顺序必须是 owner TreeView / TreeViewItem 优先，再 fallback 到 `Application.Current`。

TreeView / TreeViewItem 对绑定型节点的 attach/release 是资源生命周期边界。不得把 generated `TreeViewItem`、header、container 或 Gallery ShowCase 永久挂回节点，也不得用清空 DataContext 或静态资源替代动态资源能力。

过滤、默认状态回放和勾选级联会触发 layout pass 以实现需要访问的子容器；这些路径必须保持有界，不能引入固定延时等待。

拖拽 preview 使用 `AdornerLayer`，完成或取消拖拽时必须从 adorner layer 移除。drag indicator Pen 和 tree line Pen 均按 brush / width 缓存，避免每帧重复创建。

异步加载必须支持取消、超时和 detach 清理。加载结果回到 UI 线程后才能修改节点集合和容器状态。

Filter highlight runs 是 header 状态，不应写入 Token 或节点数据模型。Token 只提供默认颜色、尺寸和间距。

## 9. 维护不变量

内部重构必须保持以下不变量：

- public API、事件、Avalonia 属性字段和 CLR wrapper 不擅自变更。
- `TreeView.cs` 保留公共属性、事件、公共方法、生命周期和接口入口；复杂内部逻辑可继续按功能拆分 partial。
- `TraverseTreeViewPath` 是路径回放和路径操作的统一入口。
- 默认状态回放顺序保持 selected、checked、filter、expanded。
- ItemsSource 变化先尝试恢复运行期状态，再回退默认状态。
- `CheckedItemsSyncScope` 必须包裹内部批量 checked 集合更新。
- filter 进入时备份上下文，退出时恢复。
- `TreeViewItemHeader` 替换 `PART_HeaderContentFrame` 时必须解除旧 pointer 事件。
- `DefaultTreeViewInteractionHandler.Detach` 必须释放 pointer、input manager、root handler 和 radio group 关系。
- `NodeSwitcherButton.Toggle` 在节点加载中不重复触发展开。
- drag preview、drag-over、drop target 和 indicator 状态必须在拖拽完成或取消时清理。
- `TreeItemNode` 保持轻量数据节点定位，不承载 Avalonia 属性系统。
- `BindableTreeItemNode` 的 resource host attach、属性订阅和容器同步必须与容器生命周期成对释放。
- 绑定型节点不能永久保存当前 `TreeViewItem`、header、template part 或 visual container。

## 10. 测试与验证

验证范围：

- `TreeViewStateReplayTests`：默认 selected / checked / expanded 回放顺序、`IsDefaultExpandAll` 优先级、ItemsSource 变化状态恢复、strict / cascading checked。
- `TreeViewItemHeaderLifecycleTests`：template 替换时旧 `PART_HeaderContentFrame` pointer 事件释放。
- 选择：单选、多选、`IsSelectable=false`、右键选择开关和 Form value 读写。
- 勾选：checkbox 级联、strict、半选父级、radio group、`CheckedItemsChanged` added / removed。
- 过滤：匹配、高亮、加粗、隐藏未命中、展开路径、清除过滤和空状态。
- 异步加载：加载成功、超时、取消、重复请求合并、detach 取消。
- 拖拽：drag preview、drop indicator、根插入、子节点插入、自身后代保护和事件顺序。
- Theme：template part、hover mode、selected / disabled、line rendering、switcher icons、drag indicator 和 filter highlighter。
- 绑定型节点：节点属性变化同步当前容器、容器交互回写节点、DynamicResource 不 root 已移除节点、owner resource 优先于 Application resource。
- 文档改动运行 `git diff --check`。
