# DataGrid 不可变 Query 与 Range Source 架构设计

> 状态：设计已确认，等待书面评审后制定实施计划。
>
> 关联需求：[AtomUI/AtomUI#455](https://github.com/AtomUI/AtomUI/issues/455)。

## 1. 结论

DataGrid 采用“**不可变 Query + range-based Source + DataGrid-owned request coordinator**”的唯一数据架构：

```text
column / filter / group / programmatic intent
  -> immutable DataGridQuery
  -> DataGridQueryController (revision + validation)
  -> DataGridRangeCoordinator (range + cancellation + cache)
  -> IDataGridSource.FetchAsync(...)
  -> validated immutable range result
  -> atomic presentation snapshot
  -> realized rows / headers / cells / pagination
```

本设计不增加“是否服务端排序”开关。DataGrid 不区分本地、HTTP、数据库或其他数据来源；差异只存在于
`IDataGridSource` 的实现中：

- `DataGridLocalSource<T>` 在本地建立排序、过滤和分组投影，再返回请求范围。
- 业务 Source 把同一份 `DataGridQuery` 翻译为服务端协议，只返回请求范围。
- DataGrid 只管理 query、请求代际、范围缓存、虚拟化投影和视觉状态，不执行来源特定逻辑。

这是一次明确授权的 L3 破坏性重构。完成态不保留 `ItemsSource` 与新 `Source` 双通路，不保留
`CollectionView.SortDescriptions` 与 `Query.Sorts` 双状态，也不使用兼容分支、反射排序或事件驱动的数据请求。

## 2. 当前事实与改造边界

### 2.1 当前事实

当前 DataGrid 的数据与排序链路具有以下事实：

1. `ItemsSource` 经 `DataGridDataConnection.CreateView` 变为 `IDataGridCollectionView`。
2. 表头点击在 `DataGridColumnHeader.ProcessSort` 内直接修改可变 `SortDescriptions`。
3. `DataGridCollectionView.SortDescriptionsChanged` 立即刷新 View；`SortList` 使用 LINQ 排序并重新物化
   `List<object>`。
4. 表头从 CollectionView 反查当前 sort description；每个已实现 `DataGridCell` 再通过订阅读取表头排序状态。
5. `CanUserSort` 会根据绑定 path 和运行时属性类型推断 `IComparable`，空数据和远端字段无法可靠表达能力。
6. 过滤、分页、分组、current item、编辑和移动也集中在 CollectionView，不能只替换表头事件而不处理状态所有权。
7. DataGrid 当前模板已经提供 `Spin`、空状态、上下分页、表头、rows presenter 和滚动条；就绪态无需新增视觉节点。
8. DataGrid 专用测试当前有 136 个 Fact/Theory，另有 Gallery、性能状态验证、AOT 与文档生成验证。

### 2.2 改造类型审计

| 项 | 结论 |
| --- | --- |
| 优化类型 | broad / architecture root fix |
| 当前排序 owner | `IDataGridCollectionView.SortDescriptions` |
| 当前分页 owner | `DataGridCollectionView` |
| 当前视觉投影 | Header pseudo-class + per-cell subscription |
| 根问题 | 数据执行、交互策略、可变查询状态和视觉投影耦合 |
| API 级别 | L3，用户已允许忽略兼容性 |
| 渲染边界 | Ready 状态像素、布局、主题 key、Template Part、伪类语义不变 |
| AOT 边界 | 正常路径无反射、无字符串成员发现、无表达式动态编译 |
| 生命周期边界 | Source replacement、Query replacement、detach、re-template、取消和迟到结果均有确定终止路径 |

### 2.3 本次包含

- 不可变查询模型：排序、过滤、分组。
- 范围请求和范围结果协议。
- Source schema、字段能力、稳定行 key 和 snapshot 一致性。
- 查询状态机、取消、迟到结果隔离、范围缓存、预取和错误回滚。
- 本地高性能 Source 与业务远端 Source 的统一接入。
- 连续虚拟滚动和分页的统一范围语义。
- 排序/过滤/分组视觉投影、选择、current item、编辑和行移动的 Source 边界。
- Gallery、全量测试、视觉、性能、生命周期和 NativeAOT 验证。

### 2.4 明确不包含

- DataGrid 之外控件的通用数据访问框架。
- ORM、HTTP client、重试策略或服务端 DSL 的内置实现。
- 运行时 assembly/type/property 扫描。
- 无限缓存、离线数据库、跨进程缓存或业务级乐观更新框架。
- 对旧 DataGrid 公共数据 API 的长期兼容层。
- 与数据架构无关的列宽、圆角、主题、拖拽视觉或模板层级重构。

## 3. 不可破坏的不变量

### 3.1 单一状态源

- `DataGrid.Query` 是排序、过滤和分组条件的唯一 public owner。
- `DataGrid.AppliedQuery` 只记录当前已提交 presentation snapshot 对应的 Query，不可由外部写入；presentation snapshot
  同时保存 applied PageWindow、GroupExpansion 和 committed visible range，作为失败回滚的完整基线。
- Header、SortIndicator、FilterIndicator、cell sort tint 和 Gallery 状态只投影 Query，不反向拥有状态。
- Source 只消费 request 并返回结果，不能回调修改 DataGrid Query。
- `DataGridRange` 是视口请求，不属于 Query；滚动或翻页不制造新的业务 Query。

### 3.2 一致性优先

- 同一个 presentation snapshot 只能组合相同 Query revision 和相同 Source snapshot id 的 range。
- 已取消、过时代际、旧 Source、旧 snapshot 或合同不合法的结果永远不能进入当前视图。
- Query 与已显示数据暂时不一致时，body 必须处于加载覆盖和不可提交状态。
- 用户驱动的 Query/PageWindow/GroupExpansion/visible-range 加载失败时回退到最后成功的 applied presentation；不能留下
  “新控件状态 + 旧数据”的可交互界面。
- Source invalidation 刷新失败时允许展示旧快照，但快照明确标记 stale，编辑、删除和移动保持禁用直至刷新成功。

### 3.3 Ready 视觉不变

- 不重命名或删除现有 DataGrid、Header、Cell、Row、Pagination 的 ControlTheme key、Template Part、Token 或伪类。
- Ready 状态的尺寸、列宽、圆角、滚动条、冻结列、hover、selected、sorted 和 empty 视觉保持像素一致。
- 已选行背景继续高于排序列 `BodySortBg`；未选行的排序列继续显示 sort tint。
- 不把 AXAML 功能视觉迁入 C# 动态创建。
- 异步加载复用现有 `Spin` 模板结构，只把绑定源改为 DataGrid 计算后的 effective loading state。

### 3.4 有界资源

- DataGrid 内存与 `TotalCount` 无关，只与已实现行、range block 数、列数以及用户实际产生的选择 key/interval 数有关。
- 请求并发、缓存 block、预取范围和重试次数都有上限。
- 每个订阅、CancellationTokenSource、缓存数组和异步任务都有 owner、失效条件和释放路径。

### 3.5 AOT 可证明

- 字段 identity 与显示 binding path 分离。
- Source schema 显式声明字段，DataGrid 不通过首行实例反射推断字段。
- 本地 Source 使用静态强类型 getter/comparer；不使用 `Expression.Compile()`、`PropertyInfo.GetValue` 或动态泛型构造。
- 服务端 Source 显式翻译 FieldId、OperatorId 和 scalar value；DataGrid 不猜测协议。

## 4. Public Query 模型

### 4.1 稳定 identity

`DataGridFieldId` 和 `DataGridOperatorId` 是 ordinal、大小写敏感、不可变的字符串值类型：

```csharp
public readonly struct DataGridFieldId : IEquatable<DataGridFieldId>
{
    public DataGridFieldId(string value);
    public string Value { get; }
}

public readonly struct DataGridOperatorId : IEquatable<DataGridOperatorId>
{
    public DataGridOperatorId(string value);
    public string Value { get; }
}
```

构造函数拒绝 `null`、空字符串、首尾空白和控制字符。FieldId 是数据协议 identity，不是 CLR property path，
因此允许后端字段 `created_at` 映射到客户端属性 `CreatedAt`。

### 4.2 排序

```csharp
public enum DataGridSortDirection
{
    Ascending,
    Descending
}

public readonly struct DataGridSort : IEquatable<DataGridSort>
{
    public DataGridSort(DataGridFieldId field, DataGridSortDirection direction);
    public DataGridFieldId Field { get; }
    public DataGridSortDirection Direction { get; }
}
```

`Sorts` 的数组顺序就是多列排序优先级。Query 构造时拒绝重复 FieldId；同一字段不能以两个方向同时存在。

### 4.3 过滤值与过滤条件

`DataGridScalar` 是不可变判别联合，只支持可稳定比较和序列化的值：null、bool、有符号/无符号整数、double、
decimal、string、Guid、DateOnly、TimeOnly 和 DateTimeOffset。它不接受任意可变 object、delegate 或表达式树。
枚举由 Source 明确映射为整数或字符串。

Scalar 保留整数的 signed/unsigned kind，拒绝 double NaN/Infinity，把 `-0d` 规范为 `0d`，把 DateTimeOffset 规范为 UTC
instant；string 的 Query equality/hash 使用 ordinal。文化相关比较属于 Source field comparer/operator 的明确配置，不能潜入
Query equality。这样同一 Query 在本地 cache、日志和远端序列化中具有确定 identity。

```csharp
public readonly struct DataGridFilter : IEquatable<DataGridFilter>
{
    public DataGridFilter(
        DataGridFieldId field,
        DataGridOperatorId @operator,
        ImmutableArray<DataGridScalar> values);

    public DataGridFieldId Field { get; }
    public DataGridOperatorId Operator { get; }
    public ImmutableArray<DataGridScalar> Values { get; }
}
```

多个 `Filters` 按 AND 组合；单个 filter 的 values 语义由 operator 决定。DataGrid 内置菜单过滤只生成标准
`in` operator，Source schema 可以声明其他 operator。任意复杂业务条件应由业务层先归一为明确 operator，不能把
本地 predicate 塞入 Query。

### 4.4 分组

```csharp
public readonly struct DataGridGroup : IEquatable<DataGridGroup>
{
    public DataGridGroup(DataGridFieldId field, DataGridSortDirection direction);
    public DataGridFieldId Field { get; }
    public DataGridSortDirection Direction { get; }
}
```

`Groups` 顺序表示层级。Source 不声明对应能力时，DataGrid 在 Query 提交前拒绝该 Query，而不是请求后静默忽略。

### 4.5 DataGridQuery

```csharp
public sealed class DataGridQuery : IEquatable<DataGridQuery>
{
    public static DataGridQuery Empty { get; }

    public ImmutableArray<DataGridSort> Sorts { get; }
    public ImmutableArray<DataGridFilter> Filters { get; }
    public ImmutableArray<DataGridGroup> Groups { get; }

    public DataGridQuery WithSorts(ImmutableArray<DataGridSort> sorts);
    public DataGridQuery WithFilters(ImmutableArray<DataGridFilter> filters);
    public DataGridQuery WithGroups(ImmutableArray<DataGridGroup> groups);
}
```

Query 使用只读构造和 `WithXxx`，不使用带 public `init` 的 record，避免 `with` 绕过校验。构造时把 default
`ImmutableArray` 归一为 Empty，验证所有 identity、重复项和 scalar，并预计算结构 hash。结构相同的 Query
视为 no-op，不递增 revision、不触发事件、不发请求。

Query 不包含页码、可见范围、加载状态、选中项或 UI 展开状态。这些状态不会改变数据集合的业务含义。

## 5. Source 契约

### 5.1 Schema

```csharp
public sealed class DataGridSourceSchema
{
    public Type ItemType { get; }
    public ImmutableArray<DataGridFieldSchema> Fields { get; }
    public int PreferredRangeSize { get; }
    public int MaximumRangeSize { get; }
}

public sealed class DataGridFieldSchema
{
    public DataGridFieldId Id { get; }
    public Type ValueType { get; }
    public DataGridSortDirections SortDirections { get; }
    public ImmutableArray<DataGridFilterOperatorSchema> FilterOperators { get; }
    public bool CanGroup { get; }
}

[Flags]
public enum DataGridScalarKinds
{
    None = 0,
    Null = 1 << 0,
    Boolean = 1 << 1,
    SignedInteger = 1 << 2,
    UnsignedInteger = 1 << 3,
    Double = 1 << 4,
    Decimal = 1 << 5,
    String = 1 << 6,
    Guid = 1 << 7,
    DateOnly = 1 << 8,
    TimeOnly = 1 << 9,
    DateTimeOffset = 1 << 10
}

public sealed class DataGridFilterOperatorSchema
{
    public DataGridOperatorId Id { get; }
    public int MinimumValueCount { get; }
    public int MaximumValueCount { get; }
    public DataGridScalarKinds AcceptedKinds { get; }
}
```

Schema 在 Source 生命周期内不可变。需要改变 schema 时替换 Source。`Type` 只用于显式类型 metadata 和列生成，
不用于运行时成员扫描。`MaximumRangeSize` 必须在 `[32, 4096]`，`PreferredRangeSize` 必须在
`[32, MaximumRangeSize]`；构造 Schema 时立即验证，避免 Source 通过极端 range 建议制造无界分配。
Schema 还必须拒绝 null ItemType、default/重复 FieldId、同一 field 下的重复 operator id，以及互相矛盾的 field/value kind；
display-only field 可以明确声明无 sort/filter/group capability。

Filter operator schema 明确校验 arity 和 scalar kind；`0 <= MinimumValueCount <= MaximumValueCount <= 1024`。
DataGrid 内置 `in` 要求至少一个值；用户在过滤菜单清空全部选项时删除该 filter，而不是生成语义含糊的 `in []`。
LocalSource 的 custom operator 还必须在 typed descriptor 中提供对应 evaluator；远端 Source 负责显式翻译其声明的 operator。

### 5.2 行 key 与行 entry

`DataGridRowKey` 是固定判别联合，直接支持 string、long、ulong 和 Guid，避免常见数值 key 的装箱。复合 key 由
Source 归一为稳定 string 或 Guid。默认值无效。`DataGridGroupKey` 是独立的非空 ordinal string identity，不能与 row
key 混用；它必须由完整 group path 稳定生成，不能使用页内序号或 object hash。

```csharp
public enum DataGridSourceEntryKind
{
    Data,
    GroupHeader
}

public readonly struct DataGridGroupKey : IEquatable<DataGridGroupKey>
{
    public DataGridGroupKey(string value);
    public string Value { get; }
}

public sealed class DataGridGroupEntry
{
    public DataGridGroupKey Key { get; }
    public DataGridFieldId Field { get; }
    public DataGridScalar Value { get; }
    public int Level { get; }
    public long LeafCount { get; }
}

public readonly struct DataGridSourceEntry
{
    public DataGridSourceEntryKind Kind { get; }
    public DataGridRowKey RowKey { get; }
    public object? Item { get; }
    public DataGridGroupEntry? Group { get; }
    public long DataIndex { get; }
}
```

Data entry 必须有有效且唯一的 RowKey；item 可以为 null。GroupHeader 的 RowKey 必须为无效默认值，Group 必须包含有效
GroupKey、field、value、level 和 leaf count，不参与选择或编辑。展开状态由请求中的 GroupExpansion 投影，不由 Source
结果维护第二份状态。Data entry 的 `DataIndex` 是 filter/sort 后、分页前的全局零基业务行索引；GroupHeader 为 -1。

Source 返回的范围以扁平 display entry 为单位，因此普通行和 group header 可以共享同一个虚拟化序列。
`TotalEntryCount` 表示当前活动窗口内的扁平 display entry 数并决定滚动 extent；`TotalDataCount` 表示 Query
命中但尚未分页的全局业务行数并决定分页总数。连续滚动没有 page window，此时活动窗口就是完整查询结果。

### 5.3 Range、snapshot 与 result

```csharp
public readonly struct DataGridRange
{
    public DataGridRange(long startIndex, int count);
    public long StartIndex { get; }
    public int Count { get; }
}

public readonly struct DataGridPageWindow
{
    public DataGridPageWindow(long dataStartIndex, int dataCount);
    public long DataStartIndex { get; }
    public int DataCount { get; }
}

public sealed class DataGridGroupExpansion : IEquatable<DataGridGroupExpansion>
{
    public static DataGridGroupExpansion AllExpanded { get; }
    public ImmutableArray<DataGridGroupKey> CollapsedGroups { get; }

    public DataGridGroupExpansion Collapse(DataGridGroupKey key);
    public DataGridGroupExpansion Expand(DataGridGroupKey key);
}

public readonly struct DataGridSnapshotId : IEquatable<DataGridSnapshotId>
{
    public DataGridSnapshotId(string value);
    public string Value { get; }
}

public readonly struct DataGridFetchRequest
{
    public DataGridQuery Query { get; }
    public DataGridPageWindow? PageWindow { get; }
    public DataGridGroupExpansion GroupExpansion { get; }
    public DataGridRange Range { get; }
    public DataGridSnapshotId? ExpectedSnapshot { get; }
    public long QueryRevision { get; }
    public long DataGeneration { get; }
}

public sealed class DataGridRangeResult
{
    public long StartIndex { get; }
    public ImmutableArray<DataGridSourceEntry> Entries { get; }
    public long TotalEntryCount { get; }
    public long TotalDataCount { get; }
    public DataGridSnapshotId Snapshot { get; }
}
```

DataGridRange 构造函数拒绝负 StartIndex 和非正 Count；协调器拒绝 Count 超过当前 Schema.MaximumRangeSize，且不调用
零长度 fetch。DataGridPageWindow 拒绝负 DataStartIndex 和非正 DataCount。DataGridSnapshotId 拒绝 null、空白和控制字符；
其值是 Source 定义的不透明 identity，DataGrid 只做 ordinal equality，不解析版本格式。

`PageWindow=null` 表示连续滚动；非 null 时，PageWindow 先在 Query 命中的业务行上切页。Source 再按
GroupExpansion 去掉 collapsed group 的后代并 flatten，Range 最后索引可见 display entries。Range 的 `StartIndex`
永远相对活动窗口，不同时承担全局 data index 和页内 display index 两种含义。DataGrid 使用 checked arithmetic 生成
PageWindow，Source 不自行读取 DataGrid 的 PageIndex。

GroupExpansion 使用排序、去重的 immutable array 形成确定 equality/hash/wire 顺序；`AllExpanded` 是空 collapsed set。
Groups 结构改变或 Source replacement 时重置为 AllExpanded；只改变 sort/filter 或 PageWindow 时保留 collapsed keys，Source
忽略当前结果不存在的 key。该状态仅随显式用户展开/收起操作增长，不随 TotalDataCount 自动物化。

每个结果必须是请求范围的精确切片：

```text
expectedCount = min(request.Range.Count, max(0, TotalEntryCount - request.Range.StartIndex))
result.StartIndex == request.Range.StartIndex
result.Entries.Length == expectedCount
0 <= TotalEntryCount
0 <= TotalDataCount
```

未分组的连续窗口必须满足 `TotalEntryCount == TotalDataCount`。分页窗口或分组窗口不能假设两个 total 的大小关系。
Data row key 在整个 snapshot 内稳定且唯一；group entry key 在当前活动窗口内唯一。DataGrid 检测当前缓存范围内的重复
key；Source 的合同测试负责覆盖同一活动窗口跨全部 range 的唯一性。相同 snapshot 的 `TotalDataCount` 必须恒定；相同
`(snapshot, PageWindow, GroupExpansion)` 的 `TotalEntryCount` 必须恒定。

每个 `(Source identity, QueryRevision, DataGeneration)` 的第一次请求使用 `ExpectedSnapshot=null`；后续请求携带该代际
第一次成功结果的 snapshot。Source 必须返回相同 snapshot，或抛出 `DataGridSnapshotExpiredException`。DataGrid 绝不
混合两个 snapshot。

### 5.4 IDataGridSource

```csharp
public interface IDataGridSource
{
    DataGridSourceSchema Schema { get; }

    ValueTask<DataGridRangeResult> FetchAsync(
        DataGridFetchRequest request,
        CancellationToken cancellationToken);

    event EventHandler? Invalidated;
}
```

`FetchAsync` 可以同步完成，也可以在任意线程完成；DataGrid 只在 UI 线程验证并提交结果。Source 不捕获 DataGrid，
不返回可变集合，不在返回后修改 entry。`Invalidated` 可以从任意线程触发；DataGrid 先捕获当前 Source identity，再调度到
UI 线程复核 identity，确认仍为当前 Source 后创建新的 data generation 并重载当前 Query。

Item 本身可以实现普通属性通知用于单元格显示，但任何会改变当前 sort/filter/group membership、row key、total 或 range
顺序的变更都必须触发 Invalidated 或通过 mutation capability 返回新 snapshot；不能只更新可见 cell 而让 Source 索引失真。

DataGrid 不 dispose 外部拥有的 Source。DataGrid 只订阅/退订 `Invalidated`。需要持有数据库连接或集合订阅的 Source
自行实现 `IDisposable`/`IAsyncDisposable`，由创建它的 ViewModel 或 service container 释放。

## 6. DataGrid Public API

### 6.1 核心属性

```csharp
public IDataGridSource? Source { get; set; }
public DataGridQuery Query { get; set; } = DataGridQuery.Empty;
public DataGridGroupExpansion GroupExpansion { get; set; } = DataGridGroupExpansion.AllExpanded;
public DataGridSelectionState Selection { get; set; } = DataGridSelectionState.Empty;
public DataGridRowKey? CurrentRowKey { get; set; }
public DataGridQuery AppliedQuery { get; }
public DataGridLoadState LoadState { get; }
public Exception? LoadError { get; }
public long TotalItemCount { get; }
public long TotalEntryCount { get; }
public bool IsDataStale { get; }
```

这些都是 runtime data state，使用 DirectProperty；`Source`、`Query`、`GroupExpansion`、`Selection` 和 CurrentRowKey 可绑定，
Query/GroupExpansion/Selection 默认 TwoWay，Applied/Load/Total/Stale 状态只读。Query/GroupExpansion/Selection 不允许 null。

```csharp
public enum DataGridLoadState
{
    Idle,
    Loading,
    Refreshing,
    Ready,
    Error
}
```

### 6.2 事件和操作

```csharp
public event EventHandler<DataGridQueryChangedEventArgs>? QueryChanged;

public void Reload();
public void SetSort(DataGridFieldId field, DataGridSortDirection? direction,
                    DataGridSortUpdateMode mode = DataGridSortUpdateMode.Replace);
public void ClearSorts();
public void CollapseGroup(DataGridGroupKey key);
public void ExpandGroup(DataGridGroupKey key);
```

`QueryChanged` 是观察事件，不可取消。它在 Query 完成内存提交、revision 递增和视觉状态更新之后触发，在首个新
Source 请求开始之前触发。参数包含 OldQuery、NewQuery、Revision 和 Reason。Reason 至少区分
`External`、`SortGesture`、`FilterGesture`、`GroupChange` 和 `LoadRollback`。

事件处理器若同步设置另一个 Query，内层设置创建更高 revision；外层事件返回后发现 revision 已过期，不再启动旧请求。
这个顺序不需要 `_ignoreQueryChanged` 或 Dispatcher 延时。

`Reload()` 不改变 Query、不触发 QueryChanged，只递增 DataGeneration 并刷新当前 Query。Collapse/Expand 只产生新的不可变
GroupExpansion、递增 DataGeneration 并刷新活动窗口，不触发 QueryChanged；对已经处于目标状态的 key 是零请求 no-op。
Query.Groups 为空时只接受 AllExpanded；用户触发 PageWindow/GroupExpansion 变化前同样必须先成功提交当前 edit。

### 6.3 列契约

```csharp
public DataGridFieldId? FieldId { get; set; }
public bool? CanUserSort { get; set; }
public DataGridSortDirections SupportedSortDirections { get; set; }
public DataGridColumnSortState SortState { get; }
```

`FieldId` 同时标识该列默认排序、过滤和分组字段；显示 Binding 只负责单元格内容。TemplateColumn 即使没有 Binding，
只要声明 FieldId 也可排序。

`CanUserSort=null` 表示继承 DataGrid；最终能力为：

```text
effectiveCanSort =
    (column.CanUserSort ?? grid.CanUserSortColumns)
    && column.FieldId is valid
    && source.Schema contains field
    && intersection(column directions, schema directions) is not empty
    && no blocking edit transaction
```

不再根据首行数据类型或 `IComparable` 推断。`SortState` 是只读 DirectProperty，包含 nullable Direction 和零基 Priority；
它由 Query 投影，Header 不再反查 CollectionView。

## 7. Query 交互策略

排序状态转换集中在无 UI 依赖的纯函数 `DataGridSortPolicy` 中。Header、tooltip、键盘、自动化测试和 public method
全部复用同一策略，禁止复制方向循环。

默认方向序列来自 column 与 schema 的交集，常规为：

```text
None -> Ascending -> Descending -> None
```

只允许单方向的列按 `None -> AllowedDirection -> None` 循环。

| 操作 | 规则 |
| --- | --- |
| 普通点击未排序列 | 清除其他 sort，把该列放在优先级 0，使用首个允许方向 |
| 普通点击唯一排序列 | 原位置循环方向；循环到 None 后 Query.Sorts 为空 |
| 普通点击多排序中的列 | 只保留该列并循环其方向 |
| Shift 点击未排序列 | 保留其他 sort，把该列追加到末尾 |
| Shift 点击已排序列 | 原优先级循环；到 None 时删除并压缩后续 priority |
| 强制 SetSort Replace | 产生只包含指定字段的 sort 数组 |
| 强制 SetSort AppendOrReplace | 原位置替换或末尾追加 |
| direction=null | 删除指定字段，保持其他字段顺序 |

排序前仍先提交当前编辑。编辑提交失败时 Query 不变化、事件不触发、请求不开始。

过滤菜单的 checked state 从 Query.Filters 派生。用户确认过滤时一次性构建新 Query；不再维护
`SelectedFilterValues` 与 filter descriptions 的双向状态。分组入口采用相同模式。

## 8. 请求协调器与状态机

### 8.1 两个单调代际

DataGrid 维护两个 `long` 计数器：

- `QueryRevision`：Query 结构变化时递增。
- `DataGeneration`：Source replacement、Reload、Invalidated、snapshot expiry、PageWindow 或 GroupExpansion 变化时递增。

二者只递增，不复用。每个 request 捕获 Source identity、Query reference、revision、generation、PageWindow、
GroupExpansion、range 和 expected snapshot。结果提交前逐项比较，任一不匹配即丢弃。

### 8.2 Query 更新顺序

```text
1. validate query structure
2. if Source != null, validate fields/operators against current Source.Schema
3. commit pending edit; failure => stop
4. structural-equality no-op check
5. cancel prior generation requests
6. SetAndRaise(Query), revision++
7. reset PageIndex to 0; if Groups structure changed, reset GroupExpansion to AllExpanded
8. project column/header/cell/filter/group visual state
9. raise QueryChanged
10. verify revision is still current
11. if Source != null, start page-0 visible-range load; otherwise remain Idle
```

步骤 9 的重入只会产生更高 revision；步骤 10 自然阻止外层旧请求。PageIndex/GroupExpansion 与 Query 的一次提交在 UI
线程形成一个原子 transition，观察者不会看到“新 Query + 旧页码/失效 group key”的中间状态。不存在 suppress flag。
Source 为 null 时允许预先绑定结构合法的 Query；候选 Source 设置时必须在 replacement 的提交点之前完成 schema validation。

### 8.3 Load 状态

```text
Source=null                           -> Idle
no applied snapshot + request        -> Loading
applied snapshot + new request       -> Refreshing
visible target range atomically ready -> Ready
non-cancellation failure             -> Error
```

Loading/Refreshing 通过内部 `EffectiveIsOperating = IsOperating || coordinator.IsLoading` 复用现有 Spin。body 在新
Query 未提交前保持旧 snapshot 但禁止 edit/delete/move；选择读取可保留，任何会写 Source 的操作被拒绝。

用户驱动的 Query、PageWindow、GroupExpansion 或 visible-range 请求失败且存在旧 snapshot 时：

1. 丢弃 pending snapshot；
2. 通过内部 `RollbackToAppliedPresentation` 状态转换恢复 snapshot 保存的 Query、PageWindow、GroupExpansion 和最后提交的
   visible range；只在 Query 实际回退时递增 QueryRevision，否则只递增 DataGeneration；
3. 恢复旧 Header/Cell/Group/Pagination/scroll 投影；
4. 设置 LoadState=Error、LoadError，并恢复旧 snapshot 的交互；
5. Query 实际回退时触发一次 Reason=LoadRollback 的 QueryChanged；仅 page/group/scroll 回退不伪造 QueryChanged；
6. 复核 revision/generation；若属性或事件处理器同步发起了更新请求，则只保留该更新产生的新状态和请求。

`RollbackToAppliedPresentation` 是具名的终态转换，不经过普通 load scheduling，也不依赖 suppress/ignore flag；回退本身
不重新请求，因为 applied snapshot 仍完整。滚动失败会回到最后成功显示的窗口，而不是在缺失 range 上显示伪空行。
初次加载失败时没有旧 snapshot，DataGrid 保持空状态并暴露错误。
Source invalidation 刷新失败时旧 snapshot 标记 `IsDataStale=true`，只读展示并等待 Reload；不谎称数据仍为最新。

Cancellation 和被替代请求不是 Error，不修改 LoadError，也不触发回退。

### 8.4 Snapshot expiry

同一 generation 的任意请求遇到 `DataGridSnapshotExpiredException` 时，协调器取消该 generation、清空 pending cache、
递增 DataGeneration，并以 expected snapshot=null 重启当前可见范围。一个用户操作最多自动重启一次；再次 expiry
进入 Error，防止无限重试。

## 9. Range 计算、缓存和虚拟化

### 9.1 逻辑索引

Source 和 DataGrid viewport 使用 `long` logical index；单个内存 buffer 和已实现 child collection 继续使用 `int` count。
`DataGridRow` 保存 long logical index。滚动 offset 到 logical index 的映射由 DataGrid 的行高 metrics owner 统一完成，
不把总数据量物化为 IList placeholder。

固定行高使用常数时间映射。可变行高复用并扩展现有稀疏行高表：默认 estimated height + 已测量行的 sparse delta；
offset 查询和前缀和由分块索引完成，内存只与已测量范围有关。

### 9.2 Block 策略

```text
blockSize = clamp(source.Schema.PreferredRangeSize, 32, source.Schema.MaximumRangeSize)
visibleTarget = visible range + one viewport before + one viewport after
request range = block-aligned missing intervals inside visibleTarget
```

初次布局尚无精确 viewport 时从 0 请求一个 block。每个 block 状态为 Missing、Queued、Loading 或 Ready；同一
revision/generation/block 最多存在一个请求。

新 generation 在 snapshot 尚未建立时只发一个 bootstrap block；取得 snapshot id 后，其余请求全部携带
ExpectedSnapshot，再把并发提升到默认上限 2。当前可见缺口优先，其次按滚动方向预取。快速滚动时取消完全离开保留窗口
的请求；Source 即使忽略 Cancellation，迟到结果仍被代际校验丢弃。

缓存使用有界 LRU，默认保留至少当前 visible target 和相邻若干 block。具体 block 数通过性能测试确定，不能根据
`TotalEntryCount` 扩张。被 presentation snapshot 引用的 block 在 snapshot 释放前不能回收。

### 9.3 原子提交

Query 或 generation 变化时创建 pending snapshot。覆盖当前可见区的所有目标 block 到齐且合同验证通过后，先根据结果 total
计算合法 PageWindow/scroll bounds；若需要 clamp，则丢弃该 pending presentation、递增 generation 并只请求最终窗口，不能先
提交一个瞬时空页。无需 clamp 时，UI 线程一次性：

1. 交换 applied snapshot；
2. 更新 AppliedQuery、snapshot id、TotalEntryCount 和 TotalItemCount；
3. 提交已验证的页码、GroupExpansion 和滚动位置；
4. 按 row key 恢复 current/selection；
5. 复用或回收已实现 row；
6. 更新 empty/loading/pagination 状态；
7. 请求一次必要布局。

不能先清空 rows、再逐 block 插入导致闪烁，也不能对每个 entry 发全局 Reset。相同 Query 下滚动到新范围时只更新进入或
离开 viewport 的 containers。

### 9.4 分页

PageIndex/PageSize 是 DataGrid viewport 状态，不进入 Query。DataGrid 将其转换为请求携带的 PageWindow：

```text
PageWindow.DataStartIndex = checked(PageIndex * PageSize)
PageWindow.DataCount = PageSize
```

Range 随后相对当前 PageWindow 的 display entries 从 0 开始。普通未分组结果的 display range 与页内 data range 相同；
分组 Source 负责按“filter -> sort -> page data rows -> group page -> apply collapsed group keys -> flatten visible entries”的
固定顺序返回当前页的 display entries。结果分别报告 Query 命中的全局 `TotalDataCount` 和当前页 collapse/flatten 后的
`TotalEntryCount`。

PageIndex 或 PageSize 变化会递增 DataGeneration、取消旧页请求并以新的 PageWindow 创建 pending snapshot；它不改变 Query，
不触发 QueryChanged。Query 改变时 PageIndex 在启动请求前同步归零，因此只产生 page 0 的一个 generation。若结果表明当前
PageIndex 越界，DataGrid clamp 后只为最终 PageWindow 再发一次请求。上下 Pagination 始终投影同一 DataGrid page state。

## 10. 本地高性能 Source

### 10.1 强类型 descriptor

```csharp
var source = DataGridLocalSource.Create(
    rows,
    DataGridLocalSourceDescriptor.For<PersonRow>(static row => row.Id)
        .Field("name", static row => row.Name, StringComparer.CurrentCulture)
        .Field("age", static row => row.Age));
```

API 接收普通 `Func<T, TKey>`，不是 expression tree；FieldId 由调用方显式提供。泛型 field descriptor 在内部以
`IComparer<TKey>` 比较，不把 TKey 装箱成 object。现有 generated data-member accessor 可以用于自动列和显示 metadata，
但 hot sort path 不退回运行时 path 解析。

### 10.2 投影算法

非分组查询不复制业务对象列表：

1. 为当前 source version 创建 `int[]` 或 `long[]` source index projection；
2. 过滤时原地紧缩 index projection；
3. 排序只排列 index；
4. range fetch 通过 index 读取原始 source item 并建立小型 entry buffer。

复合 comparer 按 Query.Sorts 顺序调用强类型 field comparer；所有字段相等时以原 source ordinal 作最终比较，因此
即使底层 `Array.Sort` 不稳定，结果仍具有确定的稳定顺序。

分组查询先复用 filter/sort index projection；PageWindow 非 null 时先切出页内 index slice，再只为该 slice 生成只读
group tree，应用 GroupExpansion 后扁平化。连续窗口才在完整 projection 上建立可按 entry range 定位的 group directory。
只有分组路径承担 group metadata；plain table 不创建 group 对象。

filter/sort 基础投影 cache key 为 `(sourceVersion, DataGridQuery)`；page/group/expansion 派生索引另以
`(baseProjection, PageWindow, GroupExpansion)` 为 key，并受同一有界 LRU 管理。Source 集合变化时 sourceVersion 递增并触发
Invalidated。可观察集合通过弱转发订阅，Source dispose 后不再接收通知。

### 10.3 线程边界

可变 UI 集合不能直接在后台枚举。LocalSource 在 owner 线程捕获一次引用快照和 sourceVersion，再在后台只处理该稳定
快照的 index。调用方提供线程安全 immutable list 时可跳过引用快照。快照建立、排序和过滤均支持 CancellationToken。

不在比较循环中执行 LINQ、反射、字符串 path 解析、文化对象创建或 delegate 组合分配。

## 11. 视觉和模板集成

### 11.1 排序视觉

Query 提交后 DataGrid 建立 FieldId -> `(direction, priority)` 的小型映射，只更新旧/新 Query 中受影响的列：

```text
Query.Sorts
  -> DataGridColumn.SortState
  -> Header.CurrentSortingState
  -> :sort-ascending / :sort-descending
  -> existing DataGridSortIndicator template
```

现有伪类名和 SortIndicator 模板不变。多排序 priority 进入 automation/tooltip 语义；本次不增加默认可见编号，避免无必要
视觉变化。

### 11.2 去除 per-cell sort subscription

`DataGridCell` 不再订阅 Header 的 `CurrentSortingState`。容器 prepare/recycle 时直接从 OwningColumn.SortState 初始化
`IsSorting`；Query sort delta 时 DataGrid 对受影响列的已实现 cells 做一次无分配更新。成本从“每个 cell 一个长期 binding”
变为“每次 Query 变化更新当前已实现 cells”。

Header drag mode binding 与本需求正交，不在该重构中顺手改动。

### 11.3 Loading 与 Ready

DataGridTheme 的视觉树保持不变。`Spin.IsSpinning` 从 `IsOperating` 改为内部
`EffectiveIsOperating = IsOperating || LoadState is Loading/Refreshing`。Ready、Idle 和普通用户设置 `IsOperating` 的
既有视觉不变。

加载中旧 rows 保持布局尺寸，Spin 阻止数据提交交互，因此不会发生列宽跳动或旧行误编辑。首次空加载继续使用同一 Frame、
Header 和 scroll geometry。Error 信息通过 `LoadError` 暴露；本次不新增未经设计的 error popup 或动态 overlay。

## 12. 选择、current、编辑和移动

### 12.1 选择和 current

远端 range 会回收对象实例，不能再让 object instance 或已实现 slot 拥有 selection/current。`DataGridSelectionState` 是不可变、
规范化的声明式状态，包含四部分：

- `ExplicitKeys`：用户逐行选择的稳定 DataGridRowKey。
- `AllMatchingQuery`：可选的全量选择标记，绑定 Source identity 和 Query 的 filter membership，不保存行数个 key。
- `IndexIntervals`：排序合并后的不相交全局 DataIndex 区间，绑定创建它们时的 Source identity、AppliedQuery 和 snapshot。
- `ExcludedKeys`：从 select-all/interval 中逐行取消的例外。

已加载 entry 的选择判定是
`(AllMatchingQuery matches || key in ExplicitKeys || DataIndex in scoped interval) && key not in ExcludedKeys`。新 entry 到达或
container recycle 时只做该判定并投影 selected/current 伪类；GroupHeader 永不进入 selection。

Extended 模式下 Header checkbox 和 Ctrl+A 表示“选中当前 AppliedQuery 命中的全部业务行（跨所有页）”，用一个覆盖
当前 filter membership 的 AllMatchingQuery 标记表达，不获取全部 key、不物化全部 item。Shift 范围选择生成规范化
interval；单行 toggle 只修改 key exception。Single 模式只允许最多一个 ExplicitKey，不允许 AllMatching/interval。
选择内存只随用户手势产生的
interval/key 数增长，与总行数无关。

状态失效规则固定如下：

- PageWindow、GroupExpansion 和纯 Groups 变化不改变业务行 identity，保留 selection。
- 纯 Sorts 变化保留 AllMatchingQuery，但清理普通 index interval；ExplicitKeys 保留。
- Filters 变化清理 AllMatchingQuery、全部 interval 和 ExcludedKeys，ExplicitKeys 保留但只在同 key 再次出现时显示。
- Source.Invalidated 清理普通 index interval；AllMatchingQuery 继续表示当前 filter membership 的全部匹配行，ExplicitKeys
  保留。
- Source replacement 清空 Selection 和 CurrentRowKey，防止不同 Source 的同值 key 串选。

这些基于 Query 的 selection 转换只在新 presentation 原子提交时执行；Refreshing 期间 selection 仍属于旧 applied snapshot，
加载失败回滚时不丢选择。Source replacement 则在新 owner 提交点清空，Source.Invalidated 按 stale 规则立即处理。

`SelectionChanged` 以 OldSelection/NewSelection 描述声明式 delta，禁止为了生成 item 列表而拉取全部数据。需要对未加载选择
执行 copy/export/bulk delete 的 Source 实现可选 `IDataGridBulkSelectionSource`，直接接收 selection expression、Query 和
snapshot；缺少该能力时相关全量命令明确 disabled，绝不只处理已加载行却报告全部成功。

Current row 以 CurrentRowKey + internal DataIndex hint 表达；hint 只用于滚动优化，不作为 identity。可选
`IDataGridKeyLookupSource` 用于把 cache 外的 explicit/current key 解析到当前 Query；未实现时 key 仍可保留，但 DataGrid 不
伪造 SelectedItem 或 index。Greenfield public contract 不再暴露可枚举全部远端对象的 `SelectedItems` 假象。

### 12.2 可选 mutation capability

读 Source 不被迫实现编辑或移动。能力由独立接口表达：

```text
IDataGridEditableSource   commit/cancel/add/delete by row key + snapshot
IDataGridMovableSource    move row key relative to before/after key + snapshot
IDataGridKeyLookupSource  resolve selected/current key when it is outside cache
```

Mutation request 必须携带 Query、snapshot 和 row key；Source 返回新的 snapshot 或触发 Invalidated。DataGrid 在 mutation
成功前不伪造完成事件。

Row reorder 不再把 view index 当作源集合 index。请求使用 source key 与目标邻接 key；Source 不支持移动或当前 Query
语义不允许移动时，handle 不进入可提交状态。LocalSource 在有 sort/filter/group/page 时默认拒绝 move，与当前行为一致。

用户触发的 Query、PageWindow、GroupExpansion 或 Source replacement 前必须提交当前 edit；提交失败则中止该用户操作。
Source.Invalidated 属于无法拒绝的外部事实，DataGrid 立即取消当前 edit/mutation session、将 snapshot 标为 stale，再开始刷新，
不能把已经失效的编辑提交回 Source。

## 13. 合同错误、异常和生命周期

### 13.1 结果合同验证

DataGrid 在提交前验证：

- Source、revision、generation、range、snapshot 是否匹配。
- start、entry count、total counts 是否满足精确切片公式。
- entry kind、key、item/group payload 是否合法。
- 当前活动窗口 cache 中 row/group key 是否分别重复。
- 相同 snapshot 的 TotalDataCount，以及相同 `(snapshot, PageWindow, GroupExpansion)` 的 TotalEntryCount 是否一致。

合同错误包装为 `DataGridSourceContractException`，进入 Error，永不部分提交，也不吞异常。

### 13.2 生命周期配对

| 获取 | Owner | 释放/失效 |
| --- | --- | --- |
| `Source.Invalidated +=` | DataGrid attachment | Source replace、detach；reattach 时重新订阅 |
| generation CTS | RangeCoordinator | Query/generation replace、detach、完成后 Dispose |
| 单请求 linked CTS | Range request | request 完成、取消或 cache eviction 后 Dispose |
| pending cache blocks | pending snapshot | commit、failure、query replace、detach |
| applied cache blocks | applied snapshot | snapshot replacement、Source replace、detach |
| pooled entry/index arrays | LocalSource/cache block | block/projection eviction 或 Source dispose 时归还 |
| template part refs | DataGrid | 下一次 OnApplyTemplate 开始和 detach |

Detach 先使 revision/generation owner 失效，再取消请求、退订 Source、释放 cache 和 part 引用。取消回调或迟到 continuation
看见失效 identity 后直接结束，不能重新 attach 旧状态。

### 13.3 Source replacement

Source replacement 是完整事务：

1. 在不修改当前状态的前提下，验证非 null 候选 Source.Schema、当前 Query、活动列和 PageWindow；不适用则抛出明确异常，
   旧 Source 与旧 presentation 保持原样，不静默删条件；
2. 提交当前 edit 并结束 mutation session；提交失败则终止 replacement，旧 Source 继续生效；
3. 一次性提交新 Source identity，递增 generation，使所有旧 continuation 从此失效；
4. 取消旧请求、退订旧 Source，并清空旧 snapshot、cache、row key resolution 和 totals；
5. 候选 Source 非 null 时订阅它；若外部 Source 的事件订阅违反合同并抛出异常，则进入可诊断 Error，且绝不重新使用
   已失效旧 owner；
6. 候选 Source 非 null 时以当前 Query/PageWindow/GroupExpansion 加载首个可见范围；null 时进入 Idle 并保持空 presentation。

所有可预见的结构/schema/edit 失败都发生在步骤 3 前，因此不会产生半替换状态。步骤 3 后只有新 owner 可以提交结果；
旧请求无论成功、失败还是忽略 Cancellation 都只能结束，不能复活旧 presentation。

## 14. AOT、性能与复杂度边界

### 14.1 AOT

- 所有 public Source/Query 类型位于 DataGrid package，不向 Core/Shared 引入反向依赖。
- LocalSource descriptor 使用静态泛型 getter/comparer。
- Auto-generation 继续复用 generated `IDataMemberAccessorDescriptor`；缺失 descriptor 时不在 NativeAOT 正常路径反射。
- Gallery 远端示例模型继续使用 generated accessors 和 compiled bindings。
- 不新增 linker root、suppress、assembly scan、runtime registration 或字符串 Binding。

### 14.2 性能门槛

| 指标 | 必须满足的门槛 |
| --- | --- |
| 远端初始加载请求行数 | 不超过首个 aligned visible target；不得请求全部数据 |
| Query 改变请求数 | 同一 revision 的相同 block 最多一次 |
| 迟到结果 | 0 次提交 |
| 同时请求数 | 默认不超过 2 |
| cache | 与 block/viewport 上限相关，与 TotalCount 无关 |
| per-cell sort binding | 1 -> 0，减少 100% |
| Query no-op | 0 个请求、0 次全行刷新 |
| Ready state visual nodes | 不增加 |
| local sort | 只排序 index projection；不重新物化完整 object list |
| UI thread | 不执行远端 I/O，不执行大集合 O(N log N) sort |

不能在没有基线和重复测量时声明具体时间百分比。实施阶段必须用相同样本策略给出 before/after mean、median、P95、
allocation 和结构计数。

### 14.3 复杂度约束

新抽象只允许对应稳定职责：Query value、schema、Source contract、query controller、range coordinator、range cache、
local source descriptor 和 presentation snapshot。禁止为单个触发路径添加 private boolean switch、ignore flag、forced refresh、
Dispatcher 延时或 source-type 分支。

DataGrid 主文件保留 public/protected contract 和生命周期入口；稳定职责使用 partial：

```text
DataGrid.cs                         public contract + lifecycle
DataGrid.Query.cs                   query commit and visual projection
DataGrid.RangeLoading.cs            viewport -> coordinator integration
Data/Query/*                        immutable values and validation
Data/Source/*                       public source/schema/result contracts
Data/Source/Local/*                 typed local source and projection
Data/Virtualization/*               cache, snapshot and logical metrics
```

文件按 ownership 拆分，不按 public/private 或任意行数拆分。

## 15. 旧契约的替换

最终完成态按下表替换，不保留并行兼容 API：

| 旧契约 | 新契约 |
| --- | --- |
| `DataGrid.ItemsSource` | `DataGrid.Source` |
| `DataGrid.CollectionView` | `IDataGridSource` + read-only applied snapshot state |
| `DataGridSortDescription*` | `DataGridQuery.Sorts` |
| `SortMemberPath` | `DataGridColumn.FieldId` |
| `CustomSortComparer` | LocalSource typed field comparer |
| `Sorting` + `Handled` | non-cancelable `QueryChanged` observation |
| `Sort(int, ListSortDirection?)` | `SetSort(FieldId, Direction?, Mode)` / immutable Query |
| `ClearSort()` | `ClearSorts()` / `Query.WithSorts(Empty)` |
| `SelectedFilterValues` + FilterDescriptions | `Query.Filters` |
| `FilterMemberPath` | `FieldId` + Source schema operator capability |
| CollectionView GroupDescriptions | `Query.Groups` |
| item/slot-backed `SelectedItems` | immutable `DataGridSelectionState` by key/query/interval |
| CollectionView current item | DataGrid row-key current model |
| view-index move | key-relative optional movable Source |

迁移不得通过删除测试来“解决”编译错误。旧测试只允许机械迁移 setup/API，原有行为与视觉断言必须保留或加强。

## 16. 实施分解与提交门禁

该改造按可独立证明的阶段实施，每阶段保持可构建，并在最终切换前不删除旧实现：

1. **基线与合同冻结**：跑全量测试、DataGrid Gallery 视觉基线和性能基线；补现有排序/分页/过滤/分组/选择行为表。
2. **纯模型**：实现 FieldId、Scalar、Query、schema、range/result 与纯策略测试，不接 UI。
3. **协调器**：用 deterministic fake Source 覆盖 revision、generation、取消、迟到结果、snapshot、错误和 cache。
4. **LocalSource**：实现 typed descriptor、index projection、稳定复合排序、过滤、分组和 observable invalidation。
5. **内部 presentation path**：让 DataGrid rows/viewport 读取 range snapshot；保留 ready-state layout/theme contract。
6. **Query UI**：接入 column/header/cell/filter/group，移除 per-cell sort subscription，保持现有伪类与主题。
7. **selection/mutation/paging**：迁移 row-key selection/current、编辑、key-relative move 和上下 Pagination。
8. **单路切换**：迁移 Gallery、性能场景和测试到 Source；删除 ItemsSource/CollectionView 数据执行路径及旧 API。
9. **文档与发布验证**：同步 DataGrid overview/implementation/topic design/changelog、Gallery API/ShowCase、LLMS 和 AOT。
10. **最终全量门禁**：所有测试、视觉矩阵、性能矩阵、analyzer、NativeAOT 和 diff hygiene 全部通过后才声明完成。

每一阶段先写失败测试再写实现。若阶段发现需要双状态、ignore flag、无界 cache、动态反射或改变 Ready 视觉，停止并回到
设计评审，不继续叠补丁。

## 17. 验证矩阵

### 17.1 Query 纯逻辑

- default ImmutableArray 归一、空 Query 单例、结构 equality/hash。
- 非法/重复 field、operator、sort、group 和可变 filter value 被拒绝。
- 单列方向循环、单方向列、多列 Shift、删除中间优先级、强制 Replace/Append。
- 等价 Query no-op。
- QueryChanged 时机、reason、revision 和同步重入。
- schema 不支持的 field/direction/operator/group 在请求前失败。

### 17.2 Source 合同

- 空数据、短尾页、range 超过尾部、long start、非法 count。
- start/count/total/snapshot 不一致均产生合同错误且不部分提交。
- 同 snapshot 的 TotalDataCount 恒定，同 snapshot/window/expansion 的 TotalEntryCount 恒定，row/group key 分域唯一，
  data/group payload 合法。
- PageWindow 与 Range 的索引空间不混用；collapsed group 的后代不出现在 display range。
- Source 同步完成、异步完成、后台线程完成、取消前后完成。
- Invalidated、Source replacement、detach/reattach 的订阅数量和释放。

### 17.3 并发与错误

- Q1 慢、Q2 快：Q1 迟到不能覆盖 Q2。
- Source A 慢、替换 Source B：A 永不提交。
- Source 忽略 CancellationToken：代际校验仍丢弃结果。
- 相邻/重叠 viewport 请求去重和合并。
- snapshot expiry 自动重启一次；连续 expiry 进入 Error。
- Query load failure 回退 Query/visual/applied snapshot，事件恰好一次 rollback。
- initial load failure 与 invalidation refresh failure 状态不同且无假成功。
- QueryChanged handler 再设置 Query 时只请求最终 revision。

### 17.4 LocalSource 正确性

- 0、1、大量项，null item，重复 sort value，稳定 tie-break。
- string culture、nullable、数字、DateTimeOffset、Guid、自定义 typed comparer。
- 多字段 asc/desc 组合、filter + sort + group + page 固定顺序。
- ObservableCollection Add/Remove/Replace/Move/Reset 和 sourceVersion。
- 取消不会发布半成品 projection。
- 普通查询不创建 group entry；range 只物化请求条目。
- NativeAOT 下 generated/explicit descriptor 正常，无反射 fallback。

### 17.5 DataGrid 行为

- 点击 indicator、右边缘点击、键盘、programmatic SetSort。
- grid 默认与 column override 的 effective sort capability。
- 空 Source/空 result 仍能显示可排序远端列。
- 三态、单方向、Shift 多列、tooltip 与 Query 策略一致。
- Query 变化自动回到第一页；上下分页始终一致。
- 连续滚动、快速跳转、短尾 range、TotalCount 缩小后的 clamp。
- explicit selection/current 通过 key 跨 sort/filter/range recycle 保持；Source replacement 清空。
- Header/Ctrl+A 不枚举数据即表达全 Query 选择；Shift interval、单行 exclusions、filter/invalidation 失效规则正确。
- 未实现 bulk-selection capability 时 copy/export/delete 不得对部分已加载行伪装成全量成功。
- edit commit failure 阻止 Query；成功后只发一个请求。
- reorder capability、取消、异常、source replacement 和 key-relative target。
- group header、row details、frozen columns、column resize/reorder 与 range recycle 共存。

### 17.6 视觉和输入

Ready 状态用基线截图和结构断言覆盖：

- Light/Dark、Large/Middle/Small/Custom。
- 普通/分组表头、空数据/有数据、Title/Footer、上下分页。
- 固定行高/自动行高、冻结列、水平/垂直滚动条、行详情。
- sorted 未选行、sorted 选中行、hover、focus、disabled。
- indicator 左/中/右命中，列 resize 边界不被排序手势吞掉。
- Loading/Refreshing 保持 Frame、Header、列宽、scrollbar 和旧 rows 几何，不发生闪白或跳宽。
- Query failure 后箭头、sort tint、rows 和 pagination 全部回到 applied snapshot。
- Browser/Desktop 至少各走查一个 local 和 fake-remote 场景。

Ready 状态非预期 pixel diff 为失败；不得用更新 golden 掩盖差异。异步新增状态只允许在设计明确的 Spin 区域出现差异。

### 17.7 性能

新增 DataGrid Regression.md 和可重复 benchmark：

- 100 万本地行的单列/三列 sort：mean、median、P95、allocation、UI thread stall。
- 100 万逻辑远端行：首次 visible load、连续滚动、快速跳转、Q1/Q2 竞争。
- 10/100/1000 已实现 cells 的订阅数：sort subscription 必须为 0。
- block request 数、最大并发、cache peak entries、eviction。
- DataGrid.Basic、Filter、RowGroups、GalleryShape 的 before/after cold 和 repeated 指标。

任何主要 Ready/Gallery 指标回退都必须定位、修复或回滚，不能用远端能力收益抵消普通 DataGrid 回退。

### 17.8 最终命令门禁

实施开始前先保存全量基线；完成后至少执行：

```bash
dotnet test AtomUI.slnx -c Release --no-restore /m:1 /nr:false --nologo -v:minimal
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Release --framework net10.0 --no-build -- --verify-datagrid-states
dotnet run --project tools/AtomUI.Docs.LLMsGenerator/AtomUI.Docs.LLMsGenerator.csproj \
  -- verify --config docs/AI/generated/llms.config.json
scripts/verification/verify-aot-trim-registration.sh --full
pwsh -NoLogo -NoProfile -File controlgallery/AtomUIGallery.Desktop/scripts/PublishToLocal.ps1 \
  -publishRootPath /tmp/atomui-datagrid-query-aot \
  -runtime osx-arm64 -buildType Release -publishAot true
git diff --check
```

`dotnet test AtomUI.slnx` 必须覆盖解决方案内全部测试项目且零失败；若解决方案未包含某个真实 test project，则逐项目补跑，
不能把 fixture 项目或未发现测试误报为全量通过。NativeAOT 产物还需启动到主窗口并打开 DataGrid fake-remote 示例做 smoke。

## 18. Gallery 与文档

Gallery 增加一个稳定 fake-remote ShowCase，不能依赖网络：

- 逻辑总数至少 100,000，但内存只生成请求 range。
- 显示当前 Query.Sorts、请求 range、revision、generation 和取消次数。
- 提供可控延迟、失败一次和乱序完成开关，用于人工验证 latest-wins 与 rollback。
- 展示空 Source 仍可排序、Shift 多排序、分页和快速滚动。
- 使用稳定 SourceKey，进入 LLMS 示例来源。

实现稳定后同步：

- `docs/controls/desktop/data-display/data-grid/overview.md`：Source/Query public contract 和状态模型摘要。
- `implementation.md`：query controller、range coordinator、snapshot/cache、生命周期和 AOT ownership。
- 新建 `query-source-design.md`：只描述最终稳定设计，不保留 Issue、候选方案或实施状态。
- `changelog.md`：记录 L3 API、数据 owner 和实现结构变化。
- Gallery API/ShowCase、性能 Regression.md 和 LLMS 输入。

当前书面 spec 不提前修改控件长期文档，因为源码尚未实现，避免把目标设计误写成当前事实。

## 19. 完成定义

只有同时满足以下条件才可声明改造完成：

1. DataGrid 运行时只有 Source/Query 一条数据与排序通路。
2. 不存在服务端排序 bool、old/new 双状态、ignore flag、反射排序或 event-handler fetch。
3. 所有迟到、取消、Source replacement、snapshot expiry 和错误回滚测试通过。
4. 现有 DataGrid 行为/视觉测试的断言未被削弱，新增 Source/Query 测试完整。
5. `dotnet test AtomUI.slnx` 全量零失败，DataGrid 性能状态验证通过。
6. Ready 状态视觉矩阵无非预期差异，加载状态只出现设计允许的 Spin 差异。
7. 本地排序不反射、不复制完整 object list；远端路径不请求全部数据。
8. cache、请求、订阅和 Source 生命周期全部有界并通过 detach/re-template 验证。
9. Release analyzer、完整 AOT/Trim 注册验证和真实 Gallery NativeAOT publish + startup smoke 通过。
10. DataGrid 文档、Gallery、LLMS 输入和性能报告与最终 public contract 一致。

该门禁不能证明软件在数学意义上“绝无 bug”，但它把逻辑正确性、视觉一致性、并发安全、资源释放和性能都转化为
可执行的设计不变量与验收证据；任何一项缺失都不以“基本完成”收尾。
