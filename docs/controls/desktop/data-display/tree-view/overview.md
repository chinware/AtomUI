# TreeView 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.TreeView` 桌面版的最新设计定位、公共契约、行为状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [TreeView 桌面版实现原理](implementation.md)，TreeView Token 的专项设计见 [TreeView Token 设计](token.md)，设计和契约变化记录见 [TreeView Changelog](changelog.md)。

## 1. 控件定位

TreeView 是桌面端数据展示类树形结构控件，用于展示具有父子层级的数据、文件目录、组织结构、权限结构、分类结构和可展开节点集合。它以 Avalonia `TreeView` 为基础，扩展 AtomUI 的节点数据模型、图标、连线、选择、勾选、过滤、高亮、异步加载、拖拽、空状态、动效、Form 集成和 Token 体系。

TreeView 的职责是管理树节点容器生成、层级展开、选择、勾选、过滤展示、节点异步加载和节点拖拽重排。它不负责业务路由、文件系统访问、权限计算、搜索数据源请求、远程分页、虚拟化列表、节点详情面板或业务命令编排。

TreeView 支持两种节点提供方式：

- 直接放置 `TreeViewItem`。
- 通过 `ItemsSource` 绑定实现 `ITreeItemNode` 的节点集合。

直接子元素适合静态树或少量手写节点；`ItemsSource` 适合数据驱动树、默认路径回放、异步加载和 Form / Select 体系协同。

## 2. 设计语言

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

## 3. API 与契约模型

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

`ITreeItemNode` 是数据驱动树的节点契约，提供 `Header`、`Icon`、`ItemKey`、`Children`、`IsEnabled`、`IsChecked`、`IsSelected`、`IsExpanded`、`IsIndicatorEnabled`、`GroupName`、`IsLeaf`、`Value` 和 parent node 更新能力。

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

## 4. 行为与状态模型

TreeView 的状态模型由节点状态、选择状态、勾选状态、展开状态、过滤状态、异步加载状态、拖拽状态和空状态组成。

选择行为：

- `IsSelectable=false` 时不允许节点被选中，并清空 TreeView 当前选择。
- `IsSelectOnRightClick=false` 时，右键不更新选择。
- `SelectionMode` 继承 Avalonia `TreeView` 语义，单选使用 `SelectedItem`，多选使用 `SelectedItems`。
- Form 集成以单选 / 多选模式分别读取 `SelectedItem` 或 `SelectedItems`。

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

异步加载行为：

- `DataLoader` 只在 `ItemsSource` 数据驱动场景下使用。
- 未加载节点点击 switcher 时触发加载。
- 加载中节点显示 loading switcher icon。
- 加载成功后把返回子节点写入目标节点 `Children`，并展开目标节点。

## 5. 视觉与主题模型

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

## 6. 控件家族或集成关系

TreeView 属于 Data Display 分类，与 List、DataGrid、Card、Descriptions、GroupBox 等控件共同服务结构化数据展示。

集成关系：

- Avalonia `TreeView`：继承 ItemsControl、Selection、容器生成和基础 keyboard / focus 语义。
- `TreeViewItem`：节点容器，承载 icon、checked、loading、leaf、value 和 event。
- `TreeItemNode` / `ITreeItemNode`：数据驱动节点模型。
- `NodeSwitcherButton`：节点展开、收起、加载和叶子图标入口。
- `FloatableTreeView`：TreeView 的浮层变体，用于 TreeViewFlyout 场景。
- Form：通过 `IFormItemAware` 读取和写入选择值。
- Motion：展开收起和背景状态过渡遵守 `IsMotionEnabled`。
- AsyncLoad：通过 `ITreeItemNodeLoader` 加载子节点。

TreeView 不实现 CompactSpace、Button 家族或 Popup 菜单导航模型。

## 7. 兼容性不变量

维护 TreeView 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 TreeView / TreeViewItem / ITreeItemNode public API。
- `DefaultSelectedPaths`、`DefaultCheckedPaths`、`DefaultExpandedPaths` 是默认状态入口，不是持续受控状态。
- `IsDefaultExpandAll=true` 优先于 `DefaultExpandedPaths`。
- `SelectedItem` / `SelectedItems` 的 Avalonia 选择语义不变。
- `CheckedItems` 与已实现容器的 `IsChecked` 必须双向同步，内部同步不得递归触发重复事件。
- `IsCheckStrictly=false` 时 checkbox 保持父子级级联和半选语义；`true` 时只同步当前节点。
- `ToggleType=Radio` 只在叶子节点显示 radio，并遵守 `GroupName` 分组。
- `TreeNodePath` 匹配优先使用 `ItemKey`，没有 `ItemKey` 时才使用 `Value` 字符串。
- `ItemsSource` 变化后应尽量按节点身份路径恢复运行期选择和勾选状态，再回放默认状态。
- filter 清除后必须恢复过滤前节点可见性、展开状态和高亮状态。
- 异步加载只在数据节点模型下写入 `ITreeItemNode.Children`，不修改普通手写 `TreeViewItem` 子树。
- 拖拽不得允许节点 drop 到自身或自身后代。
- Template part 名称和职责不擅自修改。
- Token 名称和语义不擅自重命名或迁移为实例状态。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

## 8. 专项模型

### 8.1 默认状态回放模型

TreeView 加载后按固定顺序回放默认选中、默认勾选、过滤和默认展开。`IsDefaultExpandAll=true` 时展开全部节点，并覆盖 `DefaultExpandedPaths` 的展开路径。

### 8.2 Path 模型

`TreeNodePath` 通过路径段定位节点。路径段匹配 `TreeViewItem.ItemKey` 或 `TreeViewItem.Value`，用于默认状态回放、ItemsSource 变化后的状态恢复和选中路径定位。

### 8.3 勾选模型

TreeView 同时支持 checkbox 和 radio。checkbox 可以按 `IsCheckStrictly` 选择级联或严格模式；radio 使用 `GroupName` 和 radio group manager 保持同组互斥。

### 8.4 过滤模型

过滤模型由 `FilterStrategy` 组合控制：高亮命中片段、整行高亮、加粗、展开路径、隐藏未命中。过滤不是数据源查询，只作用于已实现的 TreeViewItem 容器。

### 8.5 异步加载模型

异步加载通过 `ITreeItemNodeLoader` 与 `AsyncLoadTimeout` 控制。加载请求由 switcher 触发，同一节点加载请求应合并或等待已有请求完成。

### 8.6 拖拽模型

拖拽模型以 TreeViewItem header bounds 和树层级关系计算 drop 目标。drop 结果直接调整 Items / child Items 顺序，并通过 `ItemDropped` 暴露目标父节点和插入位置。

## 9. 文档导航与验证策略

关联文档：

- [TreeView 桌面版实现原理](implementation.md)
- [TreeView Token 设计](token.md)
- [TreeView Changelog](changelog.md)

验证策略：

| 层次 | 验证内容 |
| --- | --- |
| 文档 | `overview.md`、`implementation.md`、`token.md`、`changelog.md` 链接有效。 |
| Public API | TreeView、TreeViewItem、ITreeItemNode、事件参数和默认值与源码一致。 |
| 状态回放 | 默认选中、默认勾选、默认展开、默认展开全部和 ItemsSource 变化恢复。 |
| 勾选 | strict / cascading、半选、CheckedItems 同步、radio group。 |
| 过滤 | 高亮、加粗、展开路径、隐藏未命中、清除过滤和空状态。 |
| 异步加载 | loading 状态、成功写入子节点、超时 / 取消事件结果和 detach 取消。 |
| 拖拽 | preview、drop indicator、根 / 子级插入、自身后代保护和事件顺序。 |
| Theme / Token | template part、hover mode、selected / disabled、line、switcher、drag indicator 和 token 表。 |
