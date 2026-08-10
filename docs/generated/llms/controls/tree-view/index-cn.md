# TreeView

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

TreeView 是桌面端数据展示类树形结构控件，用于展示具有父子层级的数据、文件目录、组织结构、权限结构、分类结构和可展开节点集合。它以 Avalonia `TreeView` 为基础，扩展 AtomUI 的节点数据模型、图标、连线、选择、勾选、过滤、高亮、异步加载、拖拽、空状态、动效、Form 集成和 Token 体系。

TreeView 的职责是管理树节点容器生成、层级展开、选择、勾选、过滤展示、节点异步加载和节点拖拽重排。它不负责业务路由、文件系统访问、权限计算、搜索数据源请求、远程分页、虚拟化列表、节点详情面板或业务命令编排。

TreeView 支持两种节点提供方式：

- 直接放置 `TreeViewItem`。
- 通过 `ItemsSource` 绑定实现 `ITreeItemNode` 的节点集合。

直接子元素适合静态树或少量手写节点；`ItemsSource` 适合数据驱动树、默认路径回放、异步加载和 Form / Select 体系协同。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/TreeView` |
| 状态 | Stable |

## 何时使用

TreeView 表达的是“层级结构 + 当前节点状态”的信息浏览语义。用户应能通过缩进、展开图标、节点图标、连线、hover 背景、选中背景、勾选状态和过滤高亮理解节点之间的层级关系和当前操作目标。

设计语言由以下维度组成：

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 层级结构 | 节点在树中的深度和父子关系。 | 缩进、switcher、连线。 |
| 展开状态 | 节点子级是否可见。 | expand / collapse 图标、展开动效。 |
| 选择状态 | 当前业务选择目标。 | selected 背景、`SelectedItem` / `SelectedItems`。 |
| 勾选状态 | 批量选择或节点状态标记。 | checkbox、radio、半选。 |
| 过滤状态 | 搜索或过滤命中的节点。 | 高亮文本、隐藏未命中节点、展开命中路径。 |
| 加载状态 | 子节点异步加载中。 | loading switcher icon。 |
| 拖拽状态 | 节点重排的当前目标。 | drag preview、drop indicator、drag-over 状态。 |

TreeView 的视觉强度应保持克制。节点 header 是可扫描的信息行，不应被绘制成卡片；连线和 switcher 是层级辅助，不应覆盖选中、禁用、过滤和拖拽状态的可读性。

## 公共 API

TreeView 的公共契约由 TreeView API、TreeViewItem API、节点数据 API、事件 API、template part 和伪类组成。

TreeView 核心 API：

| API | 语义 |
| --- | --- |
| `IsAutoExpandParent` | 子节点展开时是否自动展开父节点。 |
| `IsDraggable` | 是否启用节点拖拽重排。 |
| `IsShowIcon` | 是否显示节点图标。 |
| `IsShowLine` | 是否显示树形连线。 |
| `IsDefaultExpandAll` | 加载后是否默认展开全部节点。 |
| `NodeHoverMode` | 节点 hover 背景范围，支持 `Default`、`Block`、`WholeLine`。 |
| `SwitcherExpandIcon` / `SwitcherCollapseIcon` / `SwitcherRotationIcon` / `SwitcherLoadingIcon` / `SwitcherLeafIcon` | switcher 图标入口。 |
| `IsShowLeafIcon` | 是否显示叶子节点图标。 |
| `IsSwitcherRotation` | 是否使用旋转图标表达展开收起。 |
| `IsSelectable` | 是否允许节点选择。 |
| `IsSelectOnRightClick` | 右键节点时是否更新选择。 |
| `ToggleType` | 节点勾选模式，支持 none、checkbox、radio。 |
| `IsCheckStrictly` | checkbox 模式下是否关闭父子级级联。 |
| `DefaultSelectedPaths` / `DefaultCheckedPaths` / `DefaultExpandedPaths` | 初始选择、勾选和展开路径入口。 |
| `DataLoader` / `AsyncLoadTimeout` | 异步加载子节点入口和超时时间。 |
| `Filter` / `FilterValue` / `FilterValueSelector` / `FilterStrategy` | 过滤、高亮、展开路径和隐藏未命中节点入口。 |
| `FilterResultCount` | 当前过滤命中数量。 |
| `EmptyIndicator` / `EmptyIndicatorTemplate` / `IsShowEmptyIndicator` / `EmptyIndicatorPadding` | 空状态展示入口。 |
| `IsMotionEnabled` / `OpenMotion` / `CloseMotion` | 展开收起动效入口。 |

TreeViewItem 节点 API：

| API | 语义 |
| --- | --- |
| `Icon` | 节点图标。 |
| `IsChecked` | checkbox / radio 状态，支持半选。 |
| `IsLeaf` | 是否为叶子节点。 |
| `IsLoading` | 是否处于异步加载状态。 |
| `GroupName` | radio 分组名。 |
| `Value` | 节点值。 |
| `IsIndicatorEnabled` | 当前节点 checkbox / radio 是否可用。 |
| `ItemKey` | 路径匹配和节点身份标识。 |

`ITreeItemNode` 是数据驱动树的节点契约，提供 `Header`、`Icon`、`ItemKey`、`Children`、`IsEnabled`、`IsChecked`、`IsSelected`、`IsExpanded`、`IsIndicatorEnabled`、`GroupName`、`IsLeaf`、`Value` 和 parent node 更新能力。数据驱动场景下 `Children` 是节点结构的权威子集合；TreeViewItem 只投射节点状态和视觉交互，不作为结构数据源。

节点数据类型边界：

| 类型 | 定位 | 绑定能力 | 兼容边界 |
| --- | --- | --- | --- |
| `TreeItemNode` | 轻量数据节点，适合 C# 构造、异步加载结果和普通 ItemsSource。 | 作为 binding source 使用，不承载 Avalonia binding target 语义。 | 保持 POCO / record 形态，不改造成 `AvaloniaObject`。 |
| 用户自定义 `ITreeItemNode` | 业务自己的树节点模型。 | 可自行实现 `INotifyPropertyChanged` 或其他业务通知机制。 | TreeView 只依赖 `ITreeItemNode` 契约，不要求继承 AtomUI 节点基类。 |
| `BindableTreeItemNode` | 绑定型节点，适合在 XAML 中把节点属性作为 Avalonia binding target 或 `DynamicResource` target。 | 使用 Avalonia 属性系统，支持 `Header`、`Icon`、`IsChecked`、`IsSelected`、`IsExpanded` 等节点属性绑定。 | 独立于 `TreeItemNode`，不替代轻量节点；作为 owner-managed 非 Visual `AvaloniaObject` 必须遵守 scoped resource host 生命周期。 |

事件 API：

| 事件 | 语义 |
| --- | --- |
| `CheckedItemsChanged` | `CheckedItems` 增删变化。 |
| `TreeItemLoaded` | 异步节点加载完成。 |
| `ItemDragStarted` / `ItemDragCompleted` | 节点拖拽开始和结束。 |
| `ItemDragEnter` / `ItemDragLeave` / `ItemDragOver` | 拖拽目标变化。 |
| `ItemDropped` | 节点完成 drop 操作。 |
| `ItemExpanded` / `ItemCollapsed` | 节点展开和收起。 |
| `ItemClicked` | 节点 header 点击。 |
| `ItemContextMenuRequest` | 节点右键上下文菜单请求。 |

稳定 template part：

| Template Part | 所属控件 | 职责 |
| --- | --- | --- |
| `ItemsPresenter` | `TreeView` | 承载顶层节点容器。 |
| `EmptyIndicator` | `TreeView` | 空状态内容展示。 |
| `Header` | `TreeViewItem` | 节点 header 控件。 |
| `PART_ItemsPresenterMotionActor` | `TreeViewItem` | 子节点展开收起动效容器。 |
| `PART_NodeSwitcherButton` | `TreeViewItemHeader` | 展开、收起、加载和叶子图标入口。 |
| `PART_IconPresenter` | `TreeViewItemHeader` | 节点图标展示。 |
| `PART_HeaderContentFrame` | `TreeViewItemHeader` | header 内容背景、hover、selected、pressed 和 pointer 事件边界。 |

伪类契约：

- `TreeView :draggable` 表示启用拖拽。
- `TreeViewItem :toggle` 表示 checkbox 模式。
- `TreeViewItem :radio` 表示 radio 模式。
- `TreeViewItemHeader :toggle` / `:radio` 与节点 toggle 模式同步。

## 事件与命令

TreeView 的公共契约由 TreeView API、TreeViewItem API、节点数据 API、事件 API、template part 和伪类组成。
事件 API：
| 事件 | 语义 |
| `PART_HeaderContentFrame` | `TreeViewItemHeader` | header 内容背景、hover、selected、pressed 和 pointer 事件边界。 |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### {gallery:TreeViewShowCaseLangResource GenerateByTemplateTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/TreeView/Views/TreeViewShowCase.axaml:66`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:TreeView ToggleType="CheckBox"
               ItemsSource="{Binding BasicTreeNodes}"
               DefaultExpandedPaths="{Binding BasicTreeViewDefaultExpandedPaths}"
               DefaultSelectedPaths="{Binding BasicTreeViewDefaultSelectedPaths}"
               DefaultCheckedPaths="{Binding BasicTreeViewDefaultCheckedPaths}">
    <atom:TreeView.ItemTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}" DataType="atom:ITreeItemNode">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:TreeView.ItemTemplate>
</atom:TreeView>
```

### {gallery:TreeViewShowCaseLangResource AsyncLoadDataTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/TreeView/Views/TreeViewShowCase.axaml:355`

Gallery key：`ExamplesContent` / item `8`

```axaml
<atom:TreeView ItemsSource="{Binding AsyncLoadTreeNodes}"
               DataLoader="{Binding AsyncLoadTreeNodeLoader}">
    <atom:TreeView.ItemTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}" DataType="atom:ITreeItemNode">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:TreeView.ItemTemplate>
</atom:TreeView>
```

## 状态模型

TreeView 的状态模型由节点状态、选择状态、勾选状态、展开状态、过滤状态、异步加载状态、拖拽状态和空状态组成。

选择行为：

- `IsSelectable=false` 时不允许节点被选中，并清空 TreeView 当前选择。
- `IsSelectOnRightClick=false` 时，右键不更新选择。
- `SelectionMode` 继承 Avalonia `TreeView` 语义，单选使用 `SelectedItem`，多选使用 `SelectedItems`。
- Form 集成以单选 / 多选模式分别读取和写入 `SelectedItem` 或 `SelectedItems`，并保持原始节点对象 / 列表实例，不把节点值转换为字符串。

勾选行为：

- `ToggleType=None` 时不显示勾选入口。
- `ToggleType=CheckBox` 时显示 checkbox。
- `ToggleType=Radio` 时只在叶子节点显示 radio。
- `IsCheckStrictly=false` 时 checkbox 勾选会级联子树，并根据子级状态更新父级 true / false / null。
- `IsCheckStrictly=true` 时 checkbox 只同步当前节点，不级联父子级。
- `CheckedItems` 是当前勾选数据集合，变化会同步已实现容器状态并触发 `CheckedItemsChanged`。

展开行为：

- switcher 触发展开收起。
- `IsDefaultExpandAll=true` 时加载后展开全部节点，并优先于 `DefaultExpandedPaths`。
- 默认路径通过 `TreeNodePath` 和 `ItemKey` / `Value` 匹配。
- 展开收起动效由 `IsMotionEnabled`、`OpenMotion`、`CloseMotion` 和 `MotionDuration` 控制。

过滤行为：

- `Filter`、`FilterValue` 和 `FilterValueSelector` 共同决定节点是否命中。
- `FilterStrategy` 控制高亮 match、整行高亮、加粗、展开命中路径和隐藏未命中节点。
- `FilterResultCount` 表示命中节点数量。
- 过滤模式下空状态依据 `FilterResultCount` 判断。

拖拽行为：

- `IsDraggable=true` 时，左键按下并超过拖拽阈值后进入拖拽。
- TreeView 创建拖拽预览和 drop indicator。
- drop 目标支持插入到根、插入到兄弟前后、插入到目标节点内部。
- 不允许把节点 drop 到自身或自身后代内。
- 拖拽命中以已实现的 `TreeViewItem` 容器计算，结构修改以数据源为权威。
- drop 操作通过 TreeView 内部数据控制器移动 root 集合或节点 `Children`，不直接修改生成容器的 `Items`。
- 节点移动是结构重排，不是业务删除；选中、勾选和展开状态按节点身份保留。

异步加载行为：

- `DataLoader` 只在 `ItemsSource` 数据驱动场景下使用。
- 未加载节点点击 switcher 时触发加载。
- 加载中节点显示 loading switcher icon。
- 加载成功后把返回子节点写入目标节点 `Children`，并展开目标节点。

绑定型节点行为：

- `TreeItemNode` 的定位是轻量数据源节点，不承载 `DynamicResource`、Avalonia styled binding target 或资源宿主职责。
- 需要在 XAML 中直接绑定节点属性，或把节点 `Header`、`Icon`、状态属性设置为 `DynamicResource` 时，使用独立的 `BindableTreeItemNode`。
- `BindableTreeItemNode` 进入 TreeView 容器生命周期时，由 owner TreeView / TreeViewItem attach scoped resource host；离开容器、detach、re-template 或 container recycle 时释放 attach token。
- 绑定型节点属性变化应同步当前生成的 `TreeViewItem` 容器；容器交互导致的 checked、selected、expanded 等状态变化也应按契约回写节点状态。
- 自定义 `ITreeItemNode` 仍按普通数据模型处理；TreeView 不要求用户模型继承 `BindableTreeItemNode`。

## 主题与 Design Token

TreeView 主题按 root、item、header、switcher 四层组织。

```text
TreeViewTheme
  root scroll viewer
  items presenter
  empty indicator

TreeViewItemTheme
  header
  child items motion actor

TreeViewItemHeaderTheme
  switcher
  checkbox / radio
  icon
  header content frame
  filter highlighter

NodeSwitcherButtonTheme
  current icon presenter
  hover background
  rotation / loading transition
```

视觉规则：

- `NodeHoverMode=Default` 时 header 内容背景按内容宽度绘制。
- `NodeHoverMode=Block` 时 header 内容背景横向拉伸到剩余区域。
- `NodeHoverMode=WholeLine` 时背景由 TreeViewItem 行级绘制，覆盖整行宽度。
- disabled 节点应使用 disabled 文本色和弱化图标，不应保留可交互 hover 视觉。
- filter match 时显示 `FilterHighlighter`，未命中时显示普通 `HeaderPresenter`。
- `IsShowLine=true` 时，TreeViewItem 自绘树形连线。
- drag indicator 由 TreeView 自绘，不进入节点模板内部。

TreeViewToken 提供节点高度、hover / selected 背景、目录树选中颜色、节点间距、header padding、switcher / icon 间距、拖拽指示线宽和过滤高亮色。Token 详情见 [TreeView Token 设计](token.md)。

Token 来源：

TreeViewToken 是 TreeView 的组件级设计变量层。它把全局颜色、尺寸、间距、线宽和状态色转换为 TreeView 节点 header、switcher、icon、拖拽指示器和过滤高亮可消费的语义值。

TreeViewToken 服务以下主题和控件：

- `TreeViewTheme.axaml`
- `TreeViewItemTheme.axaml`
- `TreeViewItemHeaderTheme.axaml`
- `NodeSwitcherButtonTheme.axaml`
- `TreeView` drag indicator render state
- `TreeViewItem` line render state
- `TreeViewItemHeader` hover / selected / filter state

TreeViewToken 不承载 `SelectedItem`、`SelectedItems`、`CheckedItems`、`IsExpanded`、`IsChecked`、`IsFilterMode`、`FilterResultCount`、`IsDragging`、`DragIndicatorRenderInfo` 等实例状态。这些状态由 C# 状态模型、容器属性和主题 selector 处理。

## AOT 与裁剪注意事项

TreeView 不通过反射访问模板内部结构。模板接入依赖稳定 part 名称和 Avalonia 属性绑定。

`BindableTreeItemNode` 属于 owner-managed 非 Visual `AvaloniaObject`。它承载 `DynamicResource`、token-resource binding 或 XAML binding target 时，必须按 [Scoped Resource Host Source Generator 范式](../../../../modules/generator/scoped-resource-host-generator.md) 生成 scoped `IResourceHost` / `IThemeVariantHost` 生命周期样板代码。资源查找顺序必须是 owner TreeView / TreeViewItem 优先，再 fallback 到 `Application.Current`。

TreeView / TreeViewItem 对绑定型节点的 attach/release 是资源生命周期边界。不得把 generated `TreeViewItem`、header、container 或 Gallery ShowCase 永久挂回节点，也不得用清空 DataContext 或静态资源替代动态资源能力。

过滤、默认状态回放和勾选级联会触发 layout pass 以实现需要访问的子容器；这些路径必须保持有界，不能引入固定延时等待。

拖拽 preview 使用 `AdornerLayer`，完成或取消拖拽时必须从 adorner layer 移除。drag indicator Pen 和 tree line Pen 均按 brush / width 缓存，避免每帧重复创建。

拖拽性能边界：

- pointer move 期间只查询已实现容器和 header bounds，不访问整棵数据树。
- drop 阶段通过 `TreeNodeIndex` 读取节点上下文，避免按节点引用全树扫描。
- `TreeNodeIndex` 只在 root 数据源替换、节点集合增删移和异步加载结果进入时增量维护。
- 移动的主要成本限制在源 / 目标兄弟集合的 remove / insert；不能引入与整棵树节点数线性相关的 drop 热路径。
- 索引订阅必须随 root source 替换、节点移除、TreeView detach 和节点 collection 替换释放，避免保留旧节点树。

异步加载必须支持取消、超时和 detach 清理。加载结果回到 UI 线程后才能修改节点集合和容器状态。

Filter highlight runs 是 header 状态，不应写入 Token 或节点数据模型。Token 只提供默认颜色、尺寸和间距。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/TreeView/TreeView.cs`：public API、事件、容器生成、生命周期、选择、空状态、Form 集成和公共方法。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.StateReplay.cs`：loaded 回放、ItemsSource 变化后的选择 / 勾选 / 展开状态恢复。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.PathTraversal.cs`：`TreeNodePath` 遍历、展开路径、容器确保和展开状态恢复。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.CheckedState.cs`：checkbox 严格 / 级联状态、`CheckedItems` 同步和半选父级计算。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.Filter.cs`：过滤、高亮、隐藏未命中、展开路径、filter context 备份和恢复。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.AsyncItemDataLoad.cs`：异步加载、加载合并、超时、取消和 `TreeItemLoaded` 派发。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.DragAndDrop.cs`：拖拽命中测试、drag preview、drop indicator、drop 操作和拖拽事件。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.DataController.cs`：内部数据控制器、drop request / result、root / children 集合移动和 parent node 同步。
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.NodeIndex.cs`：节点索引、节点父级、所在集合、兄弟索引和 collection changed 增量维护。
- `src/AtomUI.Desktop.Controls/TreeView/TreeViewItem.cs`：节点容器、展开收起、line 渲染、check/radio 状态、drag bounds 和数据节点承载。
- `src/AtomUI.Desktop.Controls/TreeView/TreeViewItemHeader.cs`：header 视觉状态、pointer 状态、switcher mode、filter highlight runs 和 template part 订阅。
- `src/AtomUI.Desktop.Controls/TreeView/NodeSwitcherButton.cs`：switcher 当前图标选择、默认图标、loading load request 和 rotation 动效。
- `src/AtomUI.Desktop.Controls/TreeView/TreeItemNode.cs`、`ITreeItemNode.cs`：轻量数据节点模型、节点契约和父子关系维护。绑定型节点模型属于同一节点数据职责边界，必须保持独立类型，不改造 `TreeItemNode`。
- `src/AtomUI.Desktop.Controls/TreeView/DefaultTreeViewInteractionHandler.cs`：pointer、context menu、radio group、checked changed 和 floating tree light dismiss。
- `src/AtomUI.Desktop.Controls/TreeView/TreeViewToken.cs`：TreeView Token 定义。
- `src/AtomUI.Desktop.Controls/TreeView/Themes/`：TreeView、TreeViewItem、TreeViewItemHeader、NodeSwitcherButton 主题和模板常量。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/tree-view/overview.md`
- 实现文档：`docs/controls/desktop/data-display/tree-view/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/tree-view/token.md`
- 变更记录：`docs/controls/desktop/data-display/tree-view/changelog.md`
- 语义结构：`./semantic-cn.md`
