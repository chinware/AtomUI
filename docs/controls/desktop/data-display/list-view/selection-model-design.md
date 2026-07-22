# ListView 选择模型设计

本文档定义 `ListView` 的源条目标识、视图投影、选择状态、集合变化和跨数据源重建语义。控件总体设计见 [ListView 桌面版架构设计](overview.md)，源码职责与生命周期见 [ListView 桌面版实现原理](implementation.md)。

## 1. 设计定位

ListView 将数据源中的每一次出现建模为独立 source entry，并以 `EntryId` 连接源集合、排序、过滤、分组、分页、选择模型和条目容器。业务对象只承载数据；对象引用、`Equals`、`GetHashCode` 和显示内容均不作为源条目标识。

该模型覆盖：

- 同值不同实例、同一实例重复出现和重复值类型。
- 源集合增删、移动、替换和重置。
- 排序、过滤、分组和分页形成的视图投影。
- 单选、多选、范围选择、Toggle 和 AlwaysSelected。
- 容器创建、索引变化、虚拟化回收和重新准备。
- `ItemsSource` 整体替换后的可选恢复语义。

## 2. 设计原则

1. 数据源中的每一次出现拥有独立的 `EntryId`，源条目标识不由业务对象的相等语义推导。
2. `ListViewSelectionModel` 是选择状态的唯一 owner；公开选择属性和容器状态均为该状态的输入或投影。
3. 源索引、视图索引和页内索引是可重算位置，不作为稳定标识。
4. 排序、过滤、分组和分页只改变条目投影，不创建新的 source entry。
5. 增量集合变化以 `NotifyCollectionChangedEventArgs` 的索引和 action 为准，不通过 item equality 查找变化目标。
6. Item key 只用于 `Reset` 或 `ItemsSource` 替换后的逻辑实体匹配，不参与同一数据源生命周期内的条目选择。
7. 组标题是视图合成节点，不是源条目，不进入业务选择结果。
8. collection view、选择模型、公开投影和容器状态在同一变化事务中完成更新，不暴露中间状态。

## 3. Source Entry 模型与 Public API

### 3.1 术语

| 术语 | 定义 |
| --- | --- |
| Source item | 用户数据源中的业务对象或值。 |
| Source entry | Source item 在数据源中的一次出现，由 `EntryId` 和当前 item 组成。 |
| `EntryId` | 数据源单次生命周期内唯一且稳定的 internal 标识。 |
| Source index | Source entry 在原始数据源中的当前位置。 |
| View projection | 过滤、排序、分组和分页后供容器枚举的节点序列。 |
| View index | 节点在当前 view projection 中的位置，可以指向业务条目或组标题。 |
| Item key | 跨 Reset 或数据源替换匹配逻辑实体的稳定 `EntityKey`。 |

source entry 使用独立 internal 类型表达：

```csharp
internal sealed class ListCollectionEntry
{
    public long Id { get; }
    public object? Item { get; set; }
}
```

`Id` 在 entry 生命周期内不可变。`Item` 只在索引对应的 Replace 操作中更新。Source index 和 view index 由索引表维护，不复制为业务对象状态。

`ListItemData` 和 `GroupListItemData` 是可变展示实体，默认使用 class 的引用相等语义，不根据可变字段自动生成值相等。`IListItemData` 不定义 `IsSelected`，运行时选择状态不写回业务数据项。该默认语义只减少数据模型误用；ListView 的条目标识与选择映射仍必须覆盖任意 record、自定义 `Equals`、值类型和重复对象引用。

### 3.2 公开选择契约

| API | 语义 |
| --- | --- |
| `Selection` | ListView 持有的只读选择模型入口。调用者可以执行选择命令，但不能替换模型或设置其数据源。 |
| `SelectedIndex` | 主选中 entry 的 source index；`-1` 表示无选择。设置该值会向选择模型提交一次 source-index 选择请求。 |
| `SelectedIndexes` | 全部选中 entry 的只读 source-index 投影，按 source index 升序排列。 |
| `SelectedItem` | `SelectedIndex` 对应 item 的只读投影。 |
| `SelectedItems` | `SelectedIndexes` 对应 item 的只读投影；同一对象重复出现并被分别选中时，结果允许包含重复引用或重复值。 |
| `SelectedValue` | 通过 `SelectedValueBinding` 从 `SelectedItem` 派生的只读结果，不按值反向查找条目。 |
| `ItemKeySelector` | 可选的 `EntityKey?` 提取器，用于 Reset 和数据源替换后的选择恢复。 |

`SelectedItem`、`SelectedItems` 和 `SelectedValue` 不接受反向赋值，因为 item 或 value 无法唯一表达数据源中的一次出现。外部控制选择使用 source index 或选择模型命令；稳定 item key 只承担跨 entry 生命周期的选择恢复。

配置 `ItemKeySelector` 时只使用 selector 的结果；未配置时读取业务项的 `IItemKey.ItemKey`。未提供 key 的业务项仍可在当前数据源生命周期内正常选择，但不能在 Reset 或数据源替换后恢复选择。

### 3.3 索引语义

公开选择索引始终是 source index，不随排序、过滤、分组和分页切换语义：

- 视图中的输入先解析为 `EntryId`，再投影为 source index。
- 外部设置 source index 时直接解析对应 source entry，不通过 item 反查。
- 过滤或分页隐藏已选 entry 时，选择状态保留；entry 再次进入当前投影后恢复选中视觉。
- 组标题没有 source index，所有选择入口均跳过组标题。

范围选择按照当前 view projection 的可选择条目顺序计算，跳过组标题和不可选择条目，再把结果转换为一组 `EntryId`。Select-all 作用于当前过滤结果中的全部可选择业务 entry，不包含组标题；分页只限制容器展示，不缩小该逻辑结果。

## 4. 架构与状态所有权

| 组件 | Owner 状态 | 输入 | 输出 |
| --- | --- | --- | --- |
| `ListCollectionView` | source entries、view projection、source/view 索引表 | 数据源变化、排序、过滤、分组、分页配置 | 可枚举 view nodes 和 entry/index 映射 |
| entry view bridge | ListCollectionView 的 internal entry 查询契约 | source index、view index 或 EntryId | source entry、view node 和双向索引结果 |
| `ListViewSelectionModel` | selected EntryIds、anchor EntryId、active EntryId | source-index 命令、容器交互、collection change | 选择集合和公开选择投影 |
| `ListView` | collection view 与选择模型的协作关系 | pointer、keyboard、text search、公共属性 | 选择请求、属性通知和容器状态 |
| `ListViewItem` | 当前准备周期的 entry context | item node、EntryId、owner selection projection | `IsSelected`、内容和交互事件 |
| 分组模型 | group key 与组标题节点 | 业务 entry projection | 不可选择的 group header node |

数据保持单向流动：

```text
ItemsSource
    -> source entries
    -> filtered / sorted business-entry projection
    -> grouped / paged view nodes
    -> ListViewItem entry context
    -> selection request by EntryId
    -> ListViewSelectionModel
    -> SelectedIndex(es) / SelectedItem(s) / container IsSelected
```

collection view 不持有选择状态，选择模型不执行排序、过滤、分组或分页，容器不缓存跨准备周期的选择结果。

ListView 只直接接入实现 internal entry view bridge 的 `IListCollectionView`。当调用者提供不具备该能力的自定义 `IListCollectionView` 时，ListView 使用其 `SourceCollection` 创建 AtomUI `ListCollectionView`，再建立 source entries。外部 view 的 item equality、当前枚举位置和 `IndexOf` 结果不参与选择映射。

## 5. View Projection 与 Template 集成

View projection 至少区分两类节点：

- Item node：持有一个 source entry，具有 `EntryId` 和 source index，可以参与选择。
- Group header node：持有 `GroupListItemData` 展示数据，没有 `EntryId` 和 source index，不参与选择。

`PrepareContainerForItemOverride` 将 item node 的 `EntryId` 写入容器准备上下文，并按该 EntryId 读取 `Selection.IsSelected`。`ContainerIndexChangedOverride` 重新读取 view node 与 entry 映射，不从容器 item 反查 EntryId。

容器回收必须清除 entry context、`IsSelected` 本地值和现有虚拟化状态。重新准备后的容器只投影新 entry 的状态。

该设计不改变 `ListViewTheme`、`ListViewItemTheme`、Template Part、伪类或 Token。`:selected` 仍由 `ListView.IsSelected` 驱动，其状态由容器当前 `EntryId` 投影。

## 6. 核心映射算法

### 6.1 索引表

collection view 维护：

```text
sourceEntries                 source index -> entry
entryIdToSourceIndex          EntryId -> source index
viewNodes                     view index -> item node / group header node
entryIdToViewIndex            EntryId -> 当前可见 view index
```

这些查询通过 `AtomUI.Controls.Shared` 中的 internal entry view bridge 暴露给 `AtomUI.Desktop.Controls`。bridge 至少提供 source index 到 entry、view index 到 node、EntryId 到 source index，以及 EntryId 到当前 view index 的查询；它不暴露 EntryId 生成器，也不允许 ListView 修改 projection 所有权。

容器到选择状态的映射为：

```text
container index
    -> view node
    -> EntryId
    -> Selection.IsSelected(EntryId)
```

选择到容器的映射为：

```text
EntryId
    -> current view index
    -> prepared container
```

标识映射路径不得调用 `IList.IndexOf(item)`、`IListCollectionView.IndexOf(item)` 或基于 `Equals` 的枚举查找。若 `IListCollectionView` 为一般集合查询保留 `IndexOf(object?)`，该方法不属于选择映射契约，ListView 内部不得依赖它解析 source entry。

### 6.2 投影失效

- Add、Remove、Move 和 Replace 更新 source entries 后，使受影响的 source-index map 与 view projection 失效。
- 排序、过滤、分组和分页配置变化只重建 view projection 与 view-index map。
- source-index map 和 view-index map 在一次 refresh transaction 内发布，容器和选择模型只能读取同一代映射。
- 过滤、分页或分组导致 entry 暂时没有 view index 时，`entryIdToViewIndex` 不包含该 entry，但选择状态继续存在。

## 7. 集合变化与选择生命周期

### 7.1 增量变化

| Action | Entry 变化 | 选择语义 |
| --- | --- | --- |
| Add | 在 `NewStartingIndex` 创建并插入新 EntryId。 | 已选 EntryIds 不变；source-index 投影随位置移动。 |
| Remove | 按 `OldStartingIndex` 删除准确 entries。 | 删除对应 EntryIds、anchor 和 active 状态；其他选择保持。 |
| Move | 移动原有 entries，不创建新 EntryId。 | 已选 entries 保持，只更新 source-index 投影。 |
| Replace | 新旧数量相同时按位置保留 EntryId 并更新 Item。 | 选中槽位保持；`SelectedItem(s)` 和 `SelectedValue` 重新投影。 |
| Replace | 新旧数量不同时在同一事务内执行 indexed Remove + Add。 | 被删除 EntryIds 取消选择，新增 entries 获得新 EntryId。 |

Replace 表达同一源槽位内容更新。需要表达旧业务条目移除和新业务条目加入时，数据源使用 Remove + Add。

Move、排序、过滤、分组和分页不触发业务选择集合变化事件。它们只在 source index、主选中索引或容器投影发生变化时发布对应属性通知。选中槽位发生 Replace 时，选择模型在一个批次内发布旧 item 和新 item 的选择结果变化。

### 7.2 Reset 与 ItemsSource 替换

Reset 和 `ItemsSource` 替换建立新的 entry 生命周期：

1. 在旧 entries 释放前记录已选 entry 的非空 item key。
2. 释放旧 view projection、索引表和 entries。
3. 为新数据源创建全新 EntryIds，并建立 key 到 entry 的映射。
4. 将旧选中 key 映射为新 EntryIds；没有稳定 key 的选择清除。
5. 一次性发布选择投影和容器状态。

参与恢复的非空 key 在旧、新数据源内都必须唯一。重复 key 使逻辑实体匹配不确定，属于无效数据源并抛出 `InvalidOperationException`。空 key 不参与恢复，不使用 item equality、对象引用或相同 source index 作为回退。

### 7.3 AlwaysSelected

当 Remove、Reset 或数据源替换使选择集合为空，且 `SelectionMode.AlwaysSelected` 有效时，选择模型选中当前逻辑视图中的第一个可选择业务 entry。组标题、禁用条目和不在过滤结果中的条目不作为回退目标。逻辑视图为空时保持无选择。

## 8. 通知与一致性

一次数据或选择变化按以下顺序提交：

1. 更新 source entries。
2. 更新 source/view 索引表和 view projection。
3. 清理或迁移 selected EntryIds、anchor 和 active entry。
4. 计算 `SelectedIndex(es)`、`SelectedItem(s)` 和 `SelectedValue`。
5. 更新已准备容器的 `IsSelected`。
6. 发布属性通知和一次批量 `SelectionChanged`。

外部观察者不能看到 entry 已删除但仍被容器标记选中、source index 已变化但 SelectedItems 尚未更新，或多个选择属性分别持有不同状态的中间结果。

## 9. 性能与 AOT 边界

- EntryId 使用控件内部单调递增值生成，不依赖对象 hash code。
- 稳态容器准备和选中查询通过索引表完成，不执行线性 item `IndexOf`。
- 增量集合变化更新受影响区间；完整 Refresh 的映射重建为 O(n)，排序成本由 comparer 决定。
- source entries 与 view nodes 只持有当前数据源和当前投影所需引用，view 替换和控件释放时解除集合订阅并释放索引表。
- 设计使用普通强类型 C#、显式 delegate 和集合结构，不引入反射、运行期类型扫描、动态代码生成或 trimming root。
- Item key selector 由调用者显式提供，或通过静态接口 `IItemKey` 读取，不使用属性路径反射发现 key。

## 10. 定制边界

- 业务 item 可以是 record、重写 `Equals` 的 class、值类型或重复对象引用；这些选择不改变 ListView 的源条目标识与选择语义。
- 应用可以通过 item template、排序、过滤、分组和分页定制 view，但不能用 comparer 或 template 改写 EntryId。
- item key 必须表达跨数据源重建时的逻辑实体标识；重复 key、可变 key 或依赖显示文本的 key 不属于有效定制。
- 自定义 ControlTheme 必须继续绑定和展示 `ListView.IsSelected`，不得从数据项的 `IsSelected` 或相等比较自行推导选择状态。
- 业务数据模型不提供 ListView 运行时 `IsSelected` owner；初始选择通过 source index 或选择模型命令表达，item key 只用于 entry 生命周期重建。

## 11. 验证要求

| 分层 | 必须证明的设计不变量 |
| --- | --- |
| Entry 模型 | 同值不同实例、同一实例重复出现和重复值类型分别拥有不同 EntryId。 |
| 集合变化 | Add、Remove、Move、等量 Replace、非等量 Replace、Reset 和数据源替换遵守 EntryId 生命周期。 |
| 选择模式 | Single、Multiple、Toggle、Range 和 AlwaysSelected 只操作业务 entries。 |
| View 组合 | 排序、过滤、分组、分页及其组合不通过 item equality 重建选择。 |
| Public projection | SelectedIndexes 使用 source index；SelectedItems 允许重复 item；item/value 不作为反向选择入口。 |
| Key 恢复 | 唯一 key 恢复选择，空 key 清除选择，重复 key 明确失败。 |
| 容器生命周期 | prepare、index change、clear、recycle 和 reprepare 不泄漏旧 EntryId 或 selected 状态。 |
| 性能与 AOT | 稳态选择映射无 item 线性反查，NativeAOT 路径不引入反射或动态发现。 |
