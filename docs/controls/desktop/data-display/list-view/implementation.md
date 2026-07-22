# ListView 桌面版实现原理

本文档描述 ListView 桌面版的内部源码结构、数据视图归一、选择状态、分页、分组、过滤、容器生命周期、虚拟化上下文和主题接入。公共设计与 API 契约见 [ListView 桌面版架构设计](overview.md)，完整选择设计见 [ListView 选择模型设计](selection-model-design.md)，Token 语义见 [ListView Token 设计](token.md)，变化记录见 [ListView Changelog](changelog.md)。

## 1. 实现定位

ListView 的实现目标是在 Avalonia `ItemsControl` 基础上增加 AtomUI 数据视图列表能力，并保持 public API、主题契约和容器生命周期稳定。实现文档覆盖 `ListView`、`ListViewItem`、选择 partial、分页 partial、虚拟化上下文、`ListCollectionView` 协作、ListView token 和 AXAML theme。

本文档不逐行复述属性注册、CLR wrapper 和小型 guard。维护具体行为时仍应直接阅读源码，尤其是 `ListView.Selecting.cs`、`ListCollectionView.cs` 和 `ListView.Pagination.cs`。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/ListView/ListView.cs`：public API、事件、ItemsSource 归一、collection view 订阅、分组、过滤、排序、容器生成、空状态、操作态和点击派发。
- `src/AtomUI.Desktop.Controls/ListView/ListView.Selecting.cs`：选择模型接入、source entry / view node 映射、SelectedIndex / SelectedIndexes / SelectedItem / SelectedItems / SelectedValue、键盘导航、文本搜索和容器 selected 状态同步。
- `src/AtomUI.Desktop.Controls/ListView/ListView.Pagination.cs`：PageIndex / PageSize、分页器接入、分页器属性同步和 page change 请求。
- `src/AtomUI.Desktop.Controls/ListView/ListView.Virtualizing.cs`：容器回收时的上下文保存、恢复和本地值清理。
- `src/AtomUI.Desktop.Controls/ListView/ListViewItem.cs`：条目容器、selected attached property、点击 routed event、pointer selection、组标题状态、selected indicator 和 motion 生命周期。
- `src/AtomUI.Desktop.Controls/ListView/ListViewSelectionModel.cs`：ListView canonical selection model，持有 selected EntryIds、anchor 和 active entry，并发布 source-index 与 item 投影。
- `src/AtomUI.Desktop.Controls/ListView/ListDefaultFilter.cs`：把 collection view `FilterDescriptions` 聚合成 `Func<object, bool>`。
- `src/AtomUI.Desktop.Controls/ListView/ListPaginationVisibility.cs`：分页器可见性枚举。
- `src/AtomUI.Desktop.Controls/ListView/ListPseudoClass.cs`：`:empty` 和 `:singleitem` 伪类常量。
- `src/AtomUI.Desktop.Controls/ListView/ListViewToken.cs`：ListView 专属 Token 定义。
- `src/AtomUI.Desktop.Controls/ListView/Themes/ListViewTheme.axaml`：root 模板、分页 presenter、Spin、ScrollViewer、ItemsPresenter、EmptyIndicator、默认 item / group template 和 root 样式。
- `src/AtomUI.Desktop.Controls/ListView/Themes/ListViewItemTheme.axaml`：条目模板、selected indicator、普通内容、组标题状态和条目状态样式。
- `src/AtomUI.Controls.Shared/Data/ListCollectionViews/`：`IListCollectionView`、source entry、view node、entry/index 映射、排序、过滤、分组和数据项契约。

## 3. 核心类职责

`ListView` 是列表根控件，负责把 Avalonia `ItemsSource` 归一为 `IListCollectionView`，将排序、过滤、分组和分页 public API 写入 collection view，并把 collection view 状态同步到空状态、分页器、伪类和选择模型。

`ListViewItem` 是条目容器，继承 `ContentControl` 并实现 `ISelectable` 和 `IListItemVirtualizingContextAware`。它负责内容承载、pointer selection、点击事件、组标题标记、selected indicator 可见性和初始化 / loaded 阶段动效启停。

`ListViewSelectionModel` 是选择状态的唯一 owner。它以 EntryId 保存选中集合、anchor 和 active entry，接收 source-index 或容器交互请求，并向 ListView 投影 `SelectedIndex(es)`、`SelectedItem(s)` 和 `SelectedValue`。

`ListCollectionView` 位于 `AtomUI.Controls.Shared`，负责 source entry 生命周期、当前视图枚举、排序、过滤、分组、分页、源集合变化、entry/index 映射和 page change 事件。ListView 不直接实现这些集合算法。

`AbstractPagination` 实例由用户注入到 `TopPagination` 或 `BottomPagination`。ListView 不创建默认分页器，只在分页器存在时同步 total、page size、current page、align、enabled、motion 和 hide-on-single-page。

## 4. 状态与数据流

ItemsSource 归一流：

```text
ItemsSource changed
      ↓
newItemsSource implements internal entry view bridge ?
  yes: use entry-capable IListCollectionView
  no : newItemsSource is IListCollectionView ?
         yes: wrap its SourceCollection with AtomUI ListCollectionView
         no : wrap newItemsSource with AtomUI ListCollectionView
      ↓
subscribe PropertyChanged / CollectionChanged / PageChanging / PageChanged
      ↓
SetValueNoCallback(ItemsSourceProperty, collectionView)
      ↓
ConfigureFilterDescription
ConfigureSortDescriptions
ConfigureGroupInfo
ReConfigurePagination
```

过滤、排序、分组流：

```text
Filter / FilterValue / FilterValueSelector
      ↓
ListFilterDescription
      ↓
IListCollectionView.FilterDescriptions
      ↓
ListDefaultFilter
      ↓
ListCollectionView.Refresh

SortDescriptions
      ↓
IListCollectionView.SortDescriptions

IsGroupEnabled / GroupPropertySelector
      ↓
ListGroupDescription
      ↓
IListCollectionView.GroupDescriptions
      ↓
ListCollectionViewGroupRoot
      ↓
GroupListItemData(Content = groupKey.ToString(), IsGroupItem = true)
```

分组视图流：

```text
Source item
      ↓
GroupPropertySelector(item)
      ↓
group key
      ↓
ListCollectionViewGroupRoot.AddToSubgroups
      ↓
new group?
  yes: create ListCollectionViewGroupInternal
       insert GroupListItemData before first leaf item
  no : append / insert leaf item into existing group
      ↓
RootGroup.GetLeafEnumerator
      ↓
ListView current view enumeration
```

容器生成流：

```text
Current view item
      ↓
CreateContainerForItemOverride -> ListViewItem
      ↓
PrepareContainerForItemOverride
  ContentTemplate <- ItemTemplate / GroupItemTemplate
  SizeType / Motion / SelectedIndicator / Item backgrounds
  IsShowSelectedIndicator / ItemClickMode
  Restore IListItemData default context
  Restore virtualizing context by VirtualIndex
      ↓
ContainerForItemPreparedOverride
  sync container IsSelected from Selection
```

选择状态流：

```text
Pointer / keyboard / text search / SelectedIndex / Selection command
      ↓
view node or source index
      ↓
EntryId
      ↓
ListViewSelectionModel
      ↓
SelectedIndex(es) / SelectedItem(s) / SelectedValue projections
      ↓
ListView.IsSelected attached property on container
      ↓
ListViewItem.IsSelectedIndicatorVisible
```

`SelectedIndex` 是 source-index 选择输入。`SelectedIndexes`、`SelectedItem`、`SelectedItems` 和 `SelectedValue` 是 canonical selection 的只读投影。Selection model 不从 item 或 selected value 反向查找 source entry，同一对象重复出现时可以生成多个独立选中结果。

分页流：

```text
PageSize
      ↓
IListCollectionView.PageSize
      ↓
Current view enumerates current page

Pagination.CurrentPageChanged
      ↓
IListCollectionView.MoveToPage(page - 1)
      ↓
PageChanged
      ↓
ListView.PageIndex
      ↓
TopPagination / BottomPagination CurrentPage
```

空状态流：

```text
CollectionChanged / TotalItemCount changed
ItemsSource changed
IsShowEmptyIndicator changed
      ↓
ConfigureEmptyIndicator
      ↓
IsEffectiveEmptyVisible = IsShowEmptyIndicator && TotalItemCount == 0
      ↓
PART_ScrollViewer hidden / EmptyIndicator shown
```

## 5. 生命周期与模板接入

`ListView` 静态构造完成以下配置：

- 覆盖默认 `ItemsPanel` 为 `VirtualizingStackPanel`。
- 注册 `IsSelectedChangedEvent` class handler，同步容器 selected 状态回选择模型。
- 注册 `ListViewItem.ClickedEvent` class handler，收敛到 `ItemClicked`。
- 监听 `IsSelectableProperty`，关闭可选状态时清空选择。
- 监听 `TotalItemCountProperty`，触发 `ItemCountChanged`。
- 监听 `ItemsSourceProperty`，进入 collection view 归一流程。

`ListView` 实例构造注册 `ListViewToken` scope，监听 `ItemsView` source changed、当前 items collection changed 和 `Items.CollectionChanged`。集合变化会清理虚拟化恢复上下文、刷新空状态，并校验直接 `Items` 的数据项语义。

`HandleItemsSourcePropertyChanged` 是数据视图生命周期核心。它只直接接入具备 internal entry bridge 的 collection view；普通自定义 `IListCollectionView` 从 `SourceCollection` 重新包装。view 替换时解除旧 view 的 `PropertyChanged`、`CollectionChanged`、`PageChanging` 和 `PageChanged` 订阅，仅释放 ListView 自建 view。新 view 接入后设置 `IsEmptyDataSource`、`TotalItemCount`、过滤 callback、排序描述、过滤描述、分组描述和分页状态。

`OnInitialized` 设置默认 `GroupPropertySelector`，刷新空状态和过滤态，并确保 collection view entry projection 与 selection model 已接入。

分组相关生命周期集中在 collection view 配置阶段：

- `ConfigureGroupInfo` 根据 `IsGroupEnabled` 和 `GroupPropertySelector` 写入或清理 `GroupDescriptions`。
- `ReConfigureGroupInfo` 在 `GroupPropertySelector` 变化时清空旧分组描述，再按当前配置重建。
- `IsGroupEnabled` 变化后，ListView 使用 `_collectionView.DeferRefresh()` 合并分组重建，并刷新 view-node 映射。
- 分组只改变 view projection。selection model 始终以业务 source entries 为选择范围，组标题节点不进入该范围。

`OnApplyTemplate` 接入模板后初始化默认 `EmptyIndicator`，刷新伪类、空状态和分页器可见性。默认空状态是 `Empty` simple preset。

`PrepareContainerForItemOverride` 是 root 状态同步到 item 的核心入口。它先确保 selection model 已创建，再根据 item 类型选择 `ItemTemplate` 或 `GroupItemTemplate`，使用 Avalonia `[!]` binding 将同生命周期状态绑定到 `ListViewItem`，然后恢复默认数据上下文和虚拟化上下文。

`ClearContainerForItemOverride` 是容器回收清理入口。它临时关闭 motion 和容器 selected 同步，保存 `IsEnabled`、`IsGroupItem` 等虚拟化上下文，清理容器本地值，再调用基类释放容器。新增容器级状态时必须同步补齐 save、restore、clear 三条路径。

## 6. 交互与事件处理

pointer selection：

- 鼠标左键和右键按下时立即通过 owner ListView 更新 selection。
- touch 和 pen 的普通点击延迟到 release，避免按下阶段阻断滚动手势。
- pen 右键按下按鼠标右键处理。
- 组标题项在 `UpdateSelectionFromEventSource` 中被跳过。

click event：

- `ItemClickMode=Press` 时左键按下触发 `ListViewItem.Clicked`。
- `ItemClickMode=Release` 时左键释放触发 `ListViewItem.Clicked`。
- ListView 通过 class handler 接收后调用 `NotifyItemClicked`，再触发 public `ItemClicked`。

keyboard navigation：

- 方向键通过 `INavigableContainer` 查找下一个容器，并更新 selection 与 focus。
- `WrapSelection=true` 时同步 Avalonia focus wrap 行为。
- 多选模式下，平台 select-all 手势调用 `Selection.SelectAll()`。
- Space / Enter 从事件源更新选择。

text search：

- `IsTextSearchEnabled=true` 时，`OnTextInput` 累积输入文本。
- 搜索值优先使用 `TextSearch.TextBinding`，其次使用 `DisplayMemberBinding`。
- 命中第一个前缀匹配项后设置 `SelectedIndex`。
- 搜索词通过 1 秒 `DispatcherTimer` 清空。

pagination interaction：

- 分页器 `CurrentPageChanged` 只请求 collection view 移动页。
- collection view `PageChanged` 回写 `PageIndex`。
- 分页器替换时必须解除旧分页器事件和 relay binding。

## 7. 内部算法与关键流程

### 7.1 Collection view 桥接

ListView 接收 `ItemsSource` 后，只直接使用实现 internal entry view bridge 的 `IListCollectionView`，并且不拥有其生命周期。普通自定义 `IListCollectionView` 使用其 `SourceCollection` 创建 AtomUI `ListCollectionView`；其他 `IEnumerable` 直接包装，并在替换时释放自建 view。随后通过 `SetValueNoCallback` 把归一后的 view 写回 `ItemsSourceProperty`，避免重复进入 property changed handler。

此路径要求维护者严格区分：

- 用户设置的原始数据源。
- ListView 当前持有的 `_collectionView`。
- 控件是否拥有 `_collectionView`。
- collection view 持有的 source entries 与当前 view nodes。
- selection model 持有的 EntryIds 与公开 source-index 投影。

### 7.2 FilterDescriptions 聚合

ListView 使用 `ConfigureFilterDescription` 清空并重建 collection view 的 `FilterDescriptions`。当 `FilterValue` 和 `Filter` 同时存在时，创建 `ListFilterDescription`，并把 `FilterValueSelector` 或 `IListItemData.Content` 默认 selector 作为过滤值入口。

`ListDefaultFilter` 将多个 `FilterDescriptions` 做 AND 聚合。它通过 `WeakReference<IListCollectionView>` 持有 view，避免 filter callback 成为强引用链。

### 7.3 分组视图

分组开启时，`ConfigureGroupInfo` 向 collection view 写入 `ListGroupDescription`。默认 selector 从 `IGroupHeader.Group` 读取 group key，用户可通过 `GroupPropertySelector` 覆盖。

ListView public API 当前只创建一个 `ListGroupDescription`，因此 ListView 契约是单层分组。`IListCollectionView.GroupDescriptions` 本身是列表，属于 shared collection view 能力；除非 ListView 显式增加 public API，否则不应把多层分组写进 ListView 控件契约。

`ListCollectionViewGroupRoot.AddToSubgroups` 负责生成分组结构：

1. 使用当前层级的 `IListGroupDescription.GroupKeyFromItem` 取得 group key。
2. 如果 group key 命中已有 subgroup，则把数据项加入该 subgroup。
3. 如果 group key 是新 key，则创建 `ListCollectionViewGroupInternal`。
4. 新 subgroup 创建后，先插入一个 `GroupListItemData`，其 `Content` 为 `key.ToString()`，`IsGroupItem=true`。
5. 再把原始数据项加入同一 subgroup。

`RootGroup.GetLeafEnumerator` 会把组标题项和真实数据项一起展平成当前 view 枚举结果。ListView 因此不需要单独维护组标题集合；组标题与普通项走同一容器生成路径。

容器准备时，如果 item 实现 `IGroupListItemData` 且 `IsGroupItem=true`，ListView 使用 `GroupItemTemplate`；否则使用 `ItemTemplate`。组标题通过 `ListViewItem.IsGroupItem` 进入主题 selector，并在 pointer selection 入口被排除。

分组与分页组合时，collection view 使用两段式准备：

```text
PrepareTemporaryGroups
  build grouping for full filtered / sorted internal list
      ↓
PrepareGroupsForCurrentPage
  rebuild exposed _group from current page items
```

这保证分页后的枚举只包含当前页范围内需要展示的组标题和数据项，同时保留完整结果的分组顺序信息。

维护分组路径时必须注意：

- `GroupPropertySelector` 返回 `null` 时，当前分组算法不会自动创建默认 group；需要兜底分组时 selector 应返回明确 key。
- `GroupListItemData` 是 collection view 生成的展示项，不应写回用户 source collection。
- `TotalItemCount` 在分组态基于当前 view `Count`，可能包含组标题展示项；空状态和分页同步必须按当前实现语义处理。
- 分组节点没有 EntryId 和 source index；pointer、keyboard、range、select-all 和其他选择入口必须统一跳过组标题。
- 虚拟化上下文必须保存和清理 `IsGroupItem`，避免普通项被回收容器误显示成组标题，或组标题误使用普通项模板。

### 7.4 排序视图

`SortDescriptions` 直接同步到 collection view 的 `SortDescriptions`。`ListSortDescription.FromPath` 支持 property path 排序；NativeAOT 场景优先使用生成的数据成员访问器或显式 comparer，避免依赖运行期反射。

### 7.5 Entry 与索引映射

ListCollectionView 维护 source-index、EntryId 和 view-index 三个域：

```text
source index -> source entry
EntryId -> source index
view index -> item node / group header node
EntryId -> current view index
```

排序、过滤、分组和分页只重建 view node 顺序以及 `EntryId -> view index` 映射。公开选择索引始终由 `EntryId -> source index` 计算，不使用 `PageIndex * PageSize` 推测源位置，也不把 view item 传给 `IndexOf` 反查。

过滤或分页隐藏 entry 时，该 entry 没有当前 view index，但仍保留 source index 和选择状态。组标题只存在于 view node 序列，没有 source entry 映射。

### 7.6 SelectedValue

`SelectedValueBinding` 使用 `BindingEvaluator<object?>` 复用 evaluator。主选中 entry 变化或被 Replace 时，ListView 根据对应 item 计算 `SelectedValue`。`SelectedValue` 是只读投影，不按 value 遍历 items 反向选择条目。

`BeginInit` / `EndInit` 和 DataContext update 期间，ListView 使用选择事务延迟应用 source-index 输入与公开投影通知，避免 Items 和 selection 更新顺序暴露中间状态。

### 7.7 虚拟化上下文

`IListVirtualizingContextAware` 以容器 `VirtualIndex` 保存上下文。当前默认保存：

- `ListViewItem.IsEnabled`
- `ListViewItem.IsGroupItem`

恢复时通过 `ListVirtualizingContextAwareUtils.ExecuteWithinContextClosure` 标记上下文操作，避免恢复过程触发错误的本地状态传播。清理时移除 `Content`、`ContentTemplate`、`IsEnabled` 和 `IsGroupItem` 本地值。

## 8. 资源、性能与 AOT 边界

ListView 不通过反射访问模板内部结构。模板接入依赖稳定 part 名称、template binding、theme selector 和 Avalonia 属性绑定。

同生命周期的 root 到 item 状态同步使用 Avalonia `[!]` binding。容器由 ItemsControl 管理，binding 生命周期随容器生命周期结束。

默认 ItemsPanel 是 `VirtualizingStackPanel`。容器回收路径必须保持本地值对称清理，避免 selected、disabled、group item 和 content 状态泄漏到新数据项。

`ListCollectionView` 在排序、过滤、分页或分组启用时维护本地数组。维护过滤路径时应避免无活跃过滤条件时引入不必要的 collection view 本地数组刷新。

选择映射使用 source entries、`EntryId -> source index` 和 `EntryId -> view index` 索引表。容器准备、选中查询和选择到容器的映射不得执行基于 item equality 的线性 `IndexOf`。完整 Refresh 允许 O(n) 重建索引表；增量变化更新受影响区间并在同一 refresh transaction 中发布。

`ListSortDescription.FromPath` 存在运行期属性路径访问能力；NativeAOT 应优先使用 `GenerateDataMemberAccessors` 生成访问器，或使用 `ListSortDescription.FromComparer` 提供显式 comparer。

分页器 relay binding 和事件订阅必须在分页器替换时释放。collection view 事件订阅必须在 view 替换时释放，自建 view 必须释放其源集合弱转发器。

文本搜索 timer 不应成为长期后台工作；停止搜索时必须解除 tick 订阅并停止 timer。

## 9. 维护不变量

内部重构必须保持以下不变量：

- `ListView.cs` 保留 public API、事件、ItemsSource 归一、容器生命周期和 collection view 配置入口。
- `ListView.Selecting.cs` 保持选择模型接入、entry/index 映射、SelectedValue 和键盘 / 文本搜索职责，不把选择状态写入数据项或容器作为 owner。
- `ListViewSelectionModel` 是 selected EntryIds、anchor 和 active entry 的唯一 owner；公开属性不得维护平行选择状态。
- 标识映射路径不得使用 item `Equals`、对象引用、`IList.IndexOf` 或 `IListCollectionView.IndexOf` 解析 source entry。
- `SelectedItem`、`SelectedItems` 和 `SelectedValue` 只能从 canonical selection 投影，不作为反向选择输入。
- `ListView.Pagination.cs` 只处理分页器接入和 collection view page 状态同步，不执行数据请求。
- `ListViewItem.cs` 保持条目容器角色，不承载排序、过滤、分页或跨列表全局状态。
- collection view 替换时必须解绑旧 view 事件；只有具备 entry bridge 的 view 可以直接接入，且只有 ListView 自建 view 才能由 ListView dispose。
- `ConfigureFilterDescription`、`ConfigureSortDescriptions` 和 `ConfigureGroupInfo` 重建描述前必须清理旧描述。
- `IsGroupEnabled` 切换后只刷新 view projection，并保持组标题项没有 EntryId 且不进入业务选择结果。
- `GroupPropertySelector` 必须返回稳定 group key；需要兜底分组时由 selector 返回明确 key，不依赖 collection view 自动处理 `null`。
- `GroupListItemData` 是展示层合成项，不得写回用户源集合或被当作业务数据模型扩展点。
- 分组与分页组合时必须保持 `PrepareTemporaryGroups` 和 `PrepareGroupsForCurrentPage` 的顺序，不得只对当前页局部数据直接推断全局分组顺序。
- `PrepareContainerForItemOverride` 新增状态同步时，必须在 `ClearContainerForItemOverride` 或 virtualizing clear 路径中对称清理。
- EntryId 映射必须同时覆盖 selection change、container prepared、container index changed、auto scroll 和 selected indicator 同步。
- Add、Remove、Move 和 Replace 必须按 collection change index 更新 entries，不按 item equality 定位变化目标。
- Reset 和 ItemsSource 替换只通过唯一非空 item key 恢复选择，不按 item equality 或旧索引回退。
- 分页器替换时必须解除旧 `CurrentPageChanged` 和 relay binding。
- Token 变更必须同步 `ListViewTokenKind`、AXAML 引用和 Gallery token 表。

## 10. 测试与验证

ListView 相关验证入口：

- `tools/performances/AtomUI.Performance/Suites/ListView/ListViewStateVerification.cs`
- `tools/performances/AtomUI.Performance/Suites/ListView/ListViewScenarios.cs`
- `controlgallery/AtomUIGallery/ShowCases/DataDisplay/List/`
- `tests/AtomUI.Controls.Shared.Tests/` 中与 collection view、排序、过滤或数据成员访问器相关的测试。
- `tests/AtomUI.Desktop.Controls.Tests/` 中与 ListView、Selection 或 Pagination 相关的测试。

建议验证命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-restore -- --verify-listview-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-build -- --suite listview --count 60
dotnet test tests/AtomUI.Controls.Shared.Tests/AtomUI.Controls.Shared.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --no-restore
git diff --check
```

验证重点：

- ItemsSource 包装、替换、entry-capable view 直连、普通 `IListCollectionView` 重新归一和各自生命周期。
- 默认列表、空列表、操作态、分组、过滤、排序和分页显示。
- selected indicator 生命周期：选中、取消选中、切换选中项和虚拟化回收。
- selection 生命周期：重复 item、单选、多选、AlwaysSelected、`IsSelectable=false`、SelectedValue、Reset key 恢复、分页和分组。
- pagination 生命周期：分页器替换、可见性、对齐、motion、page size、current page 和 detach 后不再被 mutation。
- 虚拟化回收后 disabled、group item、content template 和 selected 状态不串扰。
- Gallery List ShowCase 的基础、进阶、API 和 Token 表显示稳定。
