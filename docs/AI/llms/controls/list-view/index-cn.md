# ListView

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

ListView 是桌面端数据展示类列表视图控件，用于展示一组数据项，并在同一控件内提供选择、分组、排序、过滤、分页、空状态和操作中状态。它以 Avalonia `ItemsControl` 为基础，使用 AtomUI 的 `ListCollectionView` 管理数据视图，并用 `ListViewItem` 作为条目容器。

ListView 的职责是把用户提供的 `ItemsSource` 归一到可排序、可过滤、可分组、可分页的列表视图，并把视图状态同步到容器、选择模型和分页器。它不负责树形层级、表格列模型、远程数据请求、跨控件业务命令编排或候选列表提交逻辑；这些场景应分别使用 TreeView、DataGrid、业务组合控件或 Select / AutoComplete 等候选列表控件。

ListView 支持两类数据入口：

- `ItemsSource` 绑定任意 `IEnumerable`，控件会使用 `IListCollectionView` 作为内部数据视图。
- 直接 `Items` 适合少量静态条目，直接条目需要符合 ListView 的数据项语义。

推荐数据项实现 `IListItemData`，以便默认模板、禁用状态、分组字段和默认内容展示保持一致。复杂业务对象应提供 `ItemTemplate`、`GroupItemTemplate`、`FilterValueSelector` 和 `SortDescriptions`。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/List` |
| 状态 | Stable |

## 何时使用

ListView 表达的是“数据视图 + 可操作列表项”的信息浏览语义。用户应能通过紧凑纵向结构、分组标题、hover 背景、selected 背景、可选选中标记、分页位置和操作中遮罩理解当前数据范围与交互目标。

设计语言由以下维度组成：

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 数据视图 | 原始集合经过过滤、排序、分组和分页后形成当前可见列表。 | `ListCollectionView`、`TotalItemCount`、`PageIndex`、`PageSize`。 |
| 列表结构 | 同级条目纵向排列，默认使用虚拟化面板。 | ScrollViewer、ItemsPresenter、VirtualizingStackPanel。 |
| 分组结构 | 组标题作为不可选择的列表项参与视图展示。 | `IsGroupEnabled`、`GroupItemTemplate`、组标题前景色。 |
| 选择状态 | 当前业务选中目标或选中集合。 | `SelectedIndex`、`SelectedItem`、`SelectedItems`、selected 背景、选中指示器。 |
| 过滤状态 | 外部条件筛选当前视图数据。 | `Filter`、`FilterValue`、`IsFiltering`、`FilterContextChanged`。 |
| 分页状态 | 当前视图仅展示指定页数据。 | `TopPagination`、`BottomPagination`、`PaginationVisibility`。 |
| 空状态 | 无数据或过滤后无结果。 | `EmptyIndicator` 内容区域。 |
| 操作中状态 | 数据处理或外部操作期间覆盖列表。 | `Spin`、`OperatingMsg`、`CustomOperatingIndicator`。 |

ListView 的视觉强度应保持数据展示控件的克制感。条目不是卡片，分组标题不是额外列表控件，分页器也不改变列表数据语义，只表达当前视图页边界。

## 公共 API

ListView 的公共契约由数据视图 API、选择 API、分页 API、视觉 API、事件 API、template part、伪类和主题入口组成。

数据视图 API：

| API | 语义 |
| --- | --- |
| `ItemsSource` / `Items` | 数据入口。`ItemsSource` 会归一为 `IListCollectionView`。 |
| `SortDescriptions` | 排序描述集合，交给 `IListCollectionView.SortDescriptions` 执行。 |
| `Filter` / `FilterValue` / `FilterValueSelector` | 过滤谓词、过滤条件和值选择器。 |
| `IsFiltering` | 当前是否存在有效过滤描述。 |
| `TotalItemCount` | 当前视图总数据量，用于空状态、伪类和分页同步。 |

分组 API：

| API | 语义 |
| --- | --- |
| `IsGroupEnabled` | 是否把当前视图按 group key 分组。 |
| `GroupPropertySelector` | 从数据项提取 group key；默认读取 `IGroupHeader.Group`。 |
| `GroupItemTemplate` | 组标题项模板，数据上下文为 collection view 插入的组标题数据项。 |

ListView public 分组模型是单层分组模型。`IsGroupEnabled=true` 时，ListView 会向 collection view 写入一个 `ListGroupDescription`，该描述由 `GroupPropertySelector` 提供 group key。`IListCollectionView.GroupDescriptions` 支持列表形式，但 ListView 当前 public API 不暴露多层分组配置；维护者不应把多层分组行为误写成 ListView public 契约。

选择 API：

| API | 语义 |
| --- | --- |
| `IsSelectable` | 是否允许用户通过 pointer 或 keyboard 更新选择。关闭时清空当前选择。 |
| `SelectionMode` | 选择模式，支持单选、多选、Toggle 和 AlwaysSelected 语义。 |
| `Selection` | 可替换的 `ISelectionModel`。 |
| `SelectedIndex` / `SelectedItem` / `SelectedItems` | 选择结果入口。 |
| `SelectedValue` / `SelectedValueBinding` | 按绑定值查找或派生选中值。 |
| `AutoScrollToSelectedItem` | 首次模板和视觉树就绪后滚动到 anchor 项。 |
| `IsTextSearchEnabled` | 是否启用文本增量搜索。 |
| `WrapSelection` | 键盘方向导航是否循环。 |

分页 API：

| API | 语义 |
| --- | --- |
| `PageIndex` | 当前页索引，内部与 `IListCollectionView.PageIndex` 同步。 |
| `PageSize` | 页大小，`0` 表示不分页。 |
| `TopPagination` / `BottomPagination` | 可注入的顶部和底部分页器。 |
| `PaginationVisibility` | 分页器显示位置：`None`、`Top`、`Bottom`、`Both`。 |
| `TopPaginationAlign` / `BottomPaginationAlign` | 对应分页器对齐方式。 |
| `IsHideOnSinglePage` | 透传给分页器的单页隐藏策略。 |

视觉与操作 API：

| API | 语义 |
| --- | --- |
| `SizeType` | 控制 root 圆角、空状态 padding、条目高度和 padding。 |
| `IsBorderless` | 是否隐藏 root 边框。 |
| `ItemHoverBg` / `ItemSelectedBg` | 条目 hover 和 selected 背景入口。 |
| `IsShowSelectedIndicator` / `SelectedIndicator` | 选中标记开关和图标模板。 |
| `IsMotionEnabled` | 是否启用条目和分页器动效。 |
| `EmptyIndicator` / `EmptyIndicatorTemplate` / `EmptyIndicatorPadding` / `IsShowEmptyIndicator` | 空状态展示入口。 |
| `IsOperating` / `OperatingMsg` / `CustomOperatingIndicator` / `CustomOperatingIndicatorTemplate` | 操作中遮罩和自定义指示器入口。 |
| `ItemClickMode` | 条目点击事件触发时机。 |

事件 API：

| 事件 | 语义 |
| --- | --- |
| `ItemClicked` | `ListViewItem.Clicked` 冒泡到 ListView 后触发。 |
| `ItemCountChanged` | `TotalItemCount` 变化后触发。 |
| `FilterContextChanged` | 过滤描述重新配置后触发。 |
| `SelectionChanged` | 选择模型变化后按 Avalonia routed event 形式触发。 |
| `IsSelectedChanged` | 容器 attached selected 状态变化的 routed event。 |

稳定模板命名和职责：

| 名称 | 所属控件 | 职责 |
| --- | --- | --- |
| `Frame` | `ListView` | root 背景、边框、圆角、padding 和 margin 边界。 |
| `TopPaginationPresenter` | `ListView` | 顶部分页器展示入口。 |
| `BottomPaginationPresenter` | `ListView` | 底部分页器展示入口。 |
| `PART_ScrollViewer` | `ListView` | 列表滚动容器。 |
| `ItemsPresenter` | `ListView` | 条目容器承载入口。 |
| `EmptyIndicator` | `ListView` | 空状态内容展示。 |
| `Frame` | `ListViewItem` | 条目背景、圆角和 padding 边界。 |
| `SelectedIndicator` | `ListViewItem` | 选中标记展示入口。 |
| `ContentPresenter` | `ListViewItem` | 普通内容或组标题内容展示入口。 |

伪类契约：

- `:empty` 表示 `TotalItemCount == 0`。
- `:singleitem` 表示 `TotalItemCount == 1`。
- `ListViewItem` 继承 Avalonia 选择、hover、pressed、focus 和 disabled 相关伪类语义，其中 `:selected` 由 `ListView.IsSelected` attached property 驱动。

## 事件与命令

ListView 的公共契约由数据视图 API、选择 API、分页 API、视觉 API、事件 API、template part、伪类和主题入口组成。
| `ItemClickMode` | 条目点击事件触发时机。 |
事件 API：
| 事件 | 语义 |
| `SelectionChanged` | 选择模型变化后按 Avalonia routed event 形式触发。 |
| `IsSelectedChanged` | 容器 attached selected 状态变化的 routed event。 |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 筛选

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/List/Views/ListAdvancedShowCase.axaml:12`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:ListView Name="FilteredList"
```

### 排序

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/List/Views/ListAdvancedShowCase.axaml:22`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:ListView Name="OrderedList"
```

### 简单列表框控件

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/List/Views/ListAdvancedShowCase.axaml:30`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:ListBox>
    <atom:ListBoxItem Content="赛车向人群喷出燃烧的燃料。" />
    <atom:ListBoxItem Content="日本公主将嫁给平民。" />
    <atom:ListBoxItem Content="澳大利亚人在内陆车祸后步行 100 公里。" />
    <atom:ListBoxItem Content="男子因婚礼女孩失踪案被起诉。" />
    <atom:ListBoxItem Content="洛杉矶抗击大规模山火。" />
</atom:ListBox>
```

### 简单列表框控件

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/List/Views/ListAdvancedShowCase.axaml:42`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:ListBox ItemsSource="{Binding BasicListBoxItems}"
```

## 状态模型

ListView 的状态模型由数据视图状态、选择状态、分页状态、过滤状态、分组状态、空状态、操作状态、尺寸状态和动效状态组成。

数据视图状态：

- `ItemsSource` 设置后归一为 `IListCollectionView`，ListView 监听其 `CollectionChanged`、`PropertyChanged`、`PageChanging` 和 `PageChanged`。
- `SortDescriptions`、`Filter` / `FilterValue` / `FilterValueSelector`、`IsGroupEnabled` / `GroupPropertySelector` 分别写入 collection view 的排序、过滤和分组描述。
- `TotalItemCount` 和 `IsEmptyDataSource` 来自 collection view，用于伪类、空状态和分页器同步。

选择行为：

- `IsSelectable=false` 时，ListView 不响应 pointer 或 keyboard 选择更新，并清空 `SelectedIndex`、`SelectedItem` 和 `SelectedItems`。
- 未分组时，选择源优先指向 collection view 的 `SourceCollection`，选择索引表达原始数据集合索引。
- 分组开启时，选择源指向 `IListCollectionView` 当前视图，组标题项不会被 pointer 选择路径选中。
- 分页开启时，容器索引和选择索引之间通过 `PageIndex * PageSize` 做全局索引转换。
- `SelectionMode.AlwaysSelected` 在存在数据且丢失选择时恢复到首项。
- `SelectedValueBinding` 存在时，`SelectedValue` 从选中项派生；外部设置 `SelectedValue` 时按绑定值查找选中项。

键盘和文本搜索行为：

- 方向键按 Avalonia navigation direction 移动选择，`WrapSelection` 控制边界循环。
- 多选模式下，平台 select-all 手势调用 `Selection.SelectAll()`。
- Space / Enter 会尝试从事件源更新选择。
- `IsTextSearchEnabled=true` 时，文本输入按 `TextSearch.TextBinding` 或 `DisplayMemberBinding` 做前缀搜索，搜索词由短定时器清空。

过滤行为：

- `Filter != null && FilterValue != null` 时，ListView 进入过滤态并向 collection view 写入 `ListFilterDescription`。
- `FilterValueSelector` 优先用于提取过滤值；未提供时默认读取 `IListItemData.Content`。
- 过滤改变后触发 `FilterContextChanged`，并刷新空状态。

分组行为：

- `IsGroupEnabled=true` 时，ListView 用 `GroupPropertySelector` 构造 `ListGroupDescription` 并写入 collection view。
- 默认 `GroupPropertySelector` 读取 `IGroupHeader.Group`，因此推荐分组数据项实现 `IListItemData` / `IGroupHeader` 并提供稳定、非空的 `Group` 值。
- collection view 会为每个 group key 插入一个 `GroupListItemData` 作为组标题项，其 `Content` 来自 `groupKey.ToString()`，`IsGroupItem=true`。
- 组标题项使用 `GroupItemTemplate`，普通数据项仍使用 `ItemTemplate`。
- 组标题项参与当前视图枚举和容器生成，但它表达的是视觉分隔，不是业务数据项；pointer selection 路径会跳过 `IsGroupItem=true` 的容器。
- 分组开启后，selection source 指向当前 `IListCollectionView`，因为当前视图包含组标题项、排序结果和过滤结果；未分组时 selection source 优先指向原始 `SourceCollection`。
- 与分页同时使用时，collection view 先按完整结果建立临时分组顺序，再按当前页重建对外可枚举的 group 结构。

分页行为：

- `PageSize=0` 表示不分页；`PageSize>0` 时 collection view 只枚举当前页。
- `TopPagination` 和 `BottomPagination` 接收当前 `Total`、`PageSize`、`CurrentPage`、对齐、启用状态、动效和单页隐藏策略。
- 分页器的 `CurrentPageChanged` 请求转换为 `IListCollectionView.MoveToPage(page - 1)`。
- `PaginationVisibility` 只控制分页器可见性，不改变 collection view 的分页数据。

空状态和操作态：

- `IsShowEmptyIndicator && TotalItemCount == 0` 时显示空状态。
- 空状态显示时隐藏滚动列表区域。
- `IsOperating=true` 时，root 模板内的 `Spin` 覆盖列表和空状态，用于表达外部操作中状态。

动效行为：

- ListViewItem 初始化阶段禁用 transitions，loaded 后启用，避免容器首次准备时出现非预期动画。
- 虚拟化容器保存、恢复和清理期间会临时关闭 motion，避免回收状态产生过渡。

## 主题与 Design Token

ListView 主题按 root、分页、操作态、滚动内容、空状态和 item 两层组织。

```text
ListViewTheme
  Frame
  TopPaginationPresenter
  BottomPaginationPresenter
  Spin
    PART_ScrollViewer
      ItemsPresenter
    EmptyIndicator

ListViewItemTheme
  Frame
    SelectedIndicator
    ContentPresenter
```

视觉规则：

- root 边框由 `BorderBrush`、`BorderThickness`、`CornerRadius` 和 `IsBorderless` 共同决定。
- `SizeType` 控制 root 圆角、空状态 padding、条目最小高度、条目 padding 和条目圆角。
- 默认条目背景透明，hover 使用 `ItemHoverBg`，selected 使用 `ItemSelectedBg`。
- 组标题使用 `GroupHeaderColor`，不应用普通条目的 hover / selected 状态背景。
- selected indicator 默认使用 默认 `CheckOutlined`，颜色和尺寸来自 SharedToken。
- 空状态默认使用 `Empty` 的 simple preset image。
- 分页器 margin 使用 ListViewToken 的 `PaginationMargin`。

ListViewToken 提供内容 padding、条目文字颜色、条目状态背景、条目 padding、条目 margin、分页器 margin、组标题色和选中指示器 margin。Token 详情见 [ListView Token 设计](token.md)。

Token 来源：

ListViewToken 是 ListView 的组件级设计变量层。它把全局颜色、尺寸、间距和状态色转换为 ListView root、ListViewItem、组标题、分页器间距和 selected indicator 可消费的语义值。

ListViewToken 服务以下主题和控件：

- `ListViewTheme.axaml`
- `ListViewItemTheme.axaml`
- `ListView`
- `ListViewItem`

ListViewToken 不承载 `ItemsSource`、`SelectedIndex`、`SelectedItem`、`SelectedItems`、`IsSelected`、`IsFiltering`、`FilterValue`、`PageIndex`、`PageSize`、`IsOperating`、`IsEffectiveEmptyVisible`、`VirtualIndex` 等实例状态。这些状态由 C# 状态模型、collection view、容器属性和 theme selector 处理。

## AOT 与裁剪注意事项

ListView 不通过反射访问模板内部结构。模板接入依赖稳定 part 名称、template binding、theme selector 和 Avalonia 属性绑定。

同生命周期的 root 到 item 状态同步使用 Avalonia `[!]` binding。容器由 ItemsControl 管理，binding 生命周期随容器生命周期结束。

默认 ItemsPanel 是 `VirtualizingStackPanel`。容器回收路径必须保持本地值对称清理，避免 selected、disabled、group item 和 content 状态泄漏到新数据项。

`ListCollectionView` 在排序、过滤、分页或分组启用时维护本地数组。维护过滤路径时应避免无活跃过滤条件时引入不必要的 collection view 本地数组刷新。

`ListSortDescription.FromPath` 存在运行期属性路径访问能力；NativeAOT 应优先使用 `GenerateDataMemberAccessors` 生成访问器，或使用 `ListSortDescription.FromComparer` 提供显式 comparer。

分页器 relay binding 和事件订阅必须在分页器替换时释放。collection view 事件订阅必须在 view 替换时释放，自建 view 必须释放其源集合弱转发器。

文本搜索 timer 不应成为长期后台工作；停止搜索时必须解除 tick 订阅并停止 timer。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/ListView/ListView.cs`：public API、事件、ItemsSource 归一、collection view 订阅、分组、过滤、排序、容器生成、空状态、操作态和点击派发。
- `src/AtomUI.Desktop.Controls/ListView/ListView.Selecting.cs`：`ISelectionModel` 接入、SelectedIndex / SelectedItem / SelectedItems / SelectedValue、键盘导航、文本搜索、分页索引转换和容器 selected 状态同步。
- `src/AtomUI.Desktop.Controls/ListView/ListView.Pagination.cs`：PageIndex / PageSize、分页器接入、分页器属性同步和 page change 请求。
- `src/AtomUI.Desktop.Controls/ListView/ListView.Virtualizing.cs`：容器回收时的上下文保存、恢复和本地值清理。
- `src/AtomUI.Desktop.Controls/ListView/ListViewItem.cs`：条目容器、selected attached property、点击 routed event、pointer selection、组标题状态、selected indicator 和 motion 生命周期。
- `src/AtomUI.Desktop.Controls/ListView/ListViewSelectionModel.cs`：ListView 默认 selection model。
- `src/AtomUI.Desktop.Controls/ListView/ListDefaultFilter.cs`：把 collection view `FilterDescriptions` 聚合成 `Func<object, bool>`。
- `src/AtomUI.Desktop.Controls/ListView/ListPaginationVisibility.cs`：分页器可见性枚举。
- `src/AtomUI.Desktop.Controls/ListView/ListPseudoClass.cs`：`:empty` 和 `:singleitem` 伪类常量。
- `src/AtomUI.Desktop.Controls/ListView/ListViewToken.cs`：ListView 专属 Token 定义。
- `src/AtomUI.Desktop.Controls/ListView/Themes/ListViewTheme.axaml`：root 模板、分页 presenter、Spin、ScrollViewer、ItemsPresenter、EmptyIndicator、默认 item / group template 和 root 样式。
- `src/AtomUI.Desktop.Controls/ListView/Themes/ListViewItemTheme.axaml`：条目模板、selected indicator、普通内容、组标题状态和条目状态样式。
- `src/AtomUI.Controls.Shared/Data/ListCollectionViews/`：`IListCollectionView`、`ListCollectionView`、排序、过滤、分组和数据项契约。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/list-view/overview.md`
- 实现文档：`docs/controls/desktop/data-display/list-view/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/list-view/token.md`
- 变更记录：`docs/controls/desktop/data-display/list-view/changelog.md`
- 语义结构：`./semantic-cn.md`
