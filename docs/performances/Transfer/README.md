# Transfer 性能优化

> 状态：本轮已完成 Gallery 实测。

---

## 0. 结论

本轮只收敛 `ListTransfer` / `TreeTransfer` 在 filter / `TargetKeys` 刷新时的目标 key 查询成本：

- 每次刷新先把非空 `TargetKeys` 转成 `HashSet<EntityKey>`，source / target 两侧复用同一份 membership lookup。
- `TargetKeys` 为空时，target panel 直接使用空集合，避免再扫描完整 `ItemsSource`。
- 修正 target-only filter 刷新时的变更检测顺序：先比较旧 `TargetViewSource`，再写入新值。
- 本次增量继续收敛 `TreeTransfer` 目标侧递归收集：去掉顶层 `ToList()` 源复制，并把每个节点递归返回一个临时结果 `List` 改为单个结果列表累积。
- 本次追加收敛 `TransferListView` / `TransferTreeView` 的 selected keys/items 同步：去掉 `Cast/Select/Where/ToList/ToHashSet` materialization 链，改为按已知 Count 预分配并显式拷贝。
- `AbstractTransfer` / `ListTransfer` / `TreeTransfer` 共享 key list/set 构造 helper，目标 key 集合、selection changed key 列表、transfer 后 `TargetKeys` 列表都按 Count 预分配。
- 本次继续补齐 `AbstractTransfer` / `ListTransfer` source/target filter 刷新：去掉剩余 `Where().Where().ToArray()` 链，改成一次显式扫描；`TreeTransfer` target 刷新去掉剩余 `Cast<ITreeItemNode>()` 枚举器。

没有移动任何主题视觉到 C# 动态创建，没有新增 `_ignoreXxx` 标志位、disposable、事件订阅或模板结构。

| 指标 | baseline | optimized avg | 改善 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 855.88 ms | 768.42 ms | 10.22% |
| Cold first navigation median | 828.53 ms | 772.19 ms | 6.80% |
| Cold first navigation P95 | 971.73 ms | 797.31 ms | 17.95% |
| Cold alloc mean | 42187.17 KB | 42177.28 KB | 0.02% |
| Repeated navigation mean | 242.72 ms | 222.12 ms | 8.49% |
| Repeated navigation median | 237.19 ms | 219.25 ms | 7.56% |
| Repeated navigation P95 | 276.83 ms | 244.25 ms | 11.77% |
| Repeated alloc mean | 37412.95 KB | 37399.13 KB | 0.04% |
| Runtime visuals | 1827 | 1827 | 0 |
| Runtime logical | 25 | 25 | 0 |

优化收益主要体现在页面导航时间；分配和视觉树规模基本不变，说明这是数据准备路径优化，不是结构裁剪。

本次增量为 structural-only，不声明新的页面加载耗时提升：

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| `TreeTransfer` target 刷新顶层 source copy | 1 list / refresh | 0 list / refresh | `(1 - 0) / 1` | 100.00% | 去掉 `ItemsSource.Cast<ITreeItemNode>().ToList()` |
| `TreeTransfer` target 递归结果列表 | N lists / N visited nodes | 1 list / refresh | `(N - 1) / N` | N 越大越接近 100% | 从每个递归层返回临时 `List` 改为单列表累积 |
| `TreeTransfer` target filter 后数组 | 1 array / filtered refresh | 0 array / filtered refresh | `(1 - 0) / 1` | 100.00% | 过滤判断并入收集流程，目标列表直接作为 `TargetViewSource` |
| Transfer key list `Select().ToList()` materialization | 6 iterator chains / refresh | 0 iterator chains / refresh | `(6 - 0) / 6` | 100.00% | `sourceItemKeys` / `targetItemKeys` 显式构造并预分配 |
| Transfer target key `ToHashSet()` materialization | 3 LINQ materializations / refresh/sync | 0 LINQ materializations / refresh/sync | `(3 - 0) / 3` | 100.00% | `BuildTargetKeySet` / `BuildEntityKeySet` 保留 HashSet 但去掉 LINQ 链 |
| `TransferListView` selected-items sync LINQ chain | 1 `Cast+Where+ToList` / sync | 0 LINQ chains / sync | `(1 - 0) / 1` | 100.00% | 保留类型转换 fail-fast，结果列表按 `SelectedKeys.Count` 预分配 |
| `TransferTreeView` node snapshot list growth | dynamic growth / count+mask pass | exact capacity / count+mask pass | structural | 结构收益 | `Items.Count` 已知时直接预分配 |
| transfer 后 `TargetKeys` 列表 | LINQ `ToList()` / transfer | explicit exact list / transfer | structural | 结构收益 | `HashSet.Count` 已知时直接构造结果列表 |
| Transfer source/target filter LINQ operators / refresh | 12 operators | 0 operators | `(12 - 0) / 12` | 100.00% | `AbstractTransfer` / `ListTransfer` 的 4 条 source/target 路径都改为显式扫描 |
| Transfer source/target filter output copy / refresh | 4 `ToArray()` calls | 0 `ToArray()` calls | `(4 - 0) / 4` | 100.00% | 面板数据源直接使用预分配 `List<IItemKey>`，不再二次拷贝成数组 |
| `TreeTransfer` target root cast iterator / target refresh | 1 `Cast<ITreeItemNode>()` | 0 cast iterators | `(1 - 0) / 1` | 100.00% | 保留强制类型转换语义，但不再创建 LINQ cast enumerable |

> 说明：这是可由代码结构直接证明的分配次数下降；本次未新增 Gallery timing 对比，因此不把它写成页面耗时提升。

---

## 1. 资格门槛

Gallery source：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TransferShowCase.axaml`

| 项 | 数值 |
| --- | ---: |
| active `ListTransfer` | 7 |
| active `TreeTransfer` | 1 |
| largest list data source | 2000 |
| active transfer showcase sections | 7 |
| runtime visuals | 1827 |

真实会话操作频率：

- 页面导航：至少 1 次。
- Source / target filter 输入：Search / Tree Transfer 场景会触发。
- transfer to target / source、select all / deselect all、remove all：都会重新计算 source / target panel source。
- Pagination 场景的数据量为 2000；当前 XAML 未绑定默认 `TargetKeys`，旧实现仍会扫描完整 `ItemsSource` 才得到空 target panel，是本轮实测的主要放大器。

结论：实例数 > 5，操作 > 1/session，并已有 Gallery 数字，满足 SKILL Tier 1 §13。

---

## 2. 根因

`ListTransfer.ConfigurePanelItemsSourceForFilter` 原实现会在 target panel 刷新时扫描完整 `ItemsSource`。即使 `TargetKeys` 为空，Pagination 场景也会遍历约 2000 项后得到空 target panel；当 `TargetKeys` 非空时，source 和 target panel 又会分别调用 `TargetKeys.Contains(...)`，带来线性 membership check。

`TreeTransfer.CalculateTargetItemsSource` 也在递归遍历节点时重复使用 `TargetKeys.Contains(...)`。当前 Gallery tree 数据较小，但该路径与 list transfer 语义一致，适合用同一低复杂度修正。

同时，`ListTransfer` / `TreeTransfer` target 分支之前先写入 `TargetViewSource`，再比较 `TargetViewSource != targetPanelSource`，因此 target-only filter 刷新不会把 target source change 计入 `NotifySelectionChanged`。本轮顺手修正该顺序，避免 filter 场景选择状态不同步。

主导子系统：control data preparation triggered by property/filter changes。Avalonia 侧只负责属性变化触发；本轮没有依赖新的 Avalonia 行为假设。相关成本模型背景：

- `.referenceprojects/Avalonia/src/Avalonia.Base/AvaloniaObject.cs:761-806`：property changed 本身是通知分发，实际成本取决于订阅者和后续控制逻辑。
- `.referenceprojects/Avalonia/src/Avalonia.Base/PropertyStore/ValueStore.cs:286-294`：styled property 读取是 O(1) value store 查找，本轮热点不在属性读取，而在控制自身枚举。

可证伪假设：如果把 `TargetKeys` membership lookup 从每项线性查找改为每次刷新一个 `HashSet`，且空 target 时不扫描 `ItemsSource`，`TransferShowCase` repeated navigation 应有 >= 5% 改善；runtime visuals/logical 应保持不变。

---

## 3. 改动

### 3.1 `ListTransfer`

- `ConfigurePanelItemsSourceForFilter` 开始处为非空 `TargetKeys` 创建一次 `targetKeySet`。
- source panel 用 `targetKeySet.Contains(...)` 排除 target items。
- target panel 在 `targetKeySet == null` 时直接返回 `Array.Empty<IItemKey>()`。
- target panel 先计算 `targetPanelSourceChanged`，再写入 `TargetViewSource`。

### 3.2 `TreeTransfer`

- 同样复用 `targetKeySet`。
- `CalculateTargetItemsSource` 改为接收 `ISet<EntityKey>`，递归中不再反复访问 `TargetKeys.Contains(...)`。
- `targetKeySet == null` 时 target panel 直接为空。
- target panel 先计算 changed，再写入 `TargetViewSource`。
- 本次增量去掉 target 分支的顶层 `ToList()`，递归改为 `CollectTargetItemsSource(...)` 单列表累积，并把 target filter 判断并入收集流程，避免递归层级临时列表和最终 filter 数组。

---

## 4. 验证

### 4.1 构建

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug --framework net10.0 --no-restore
```

结果：构建通过；保留既有 warning `DataGridColumn._clipboardContentBinding is never used`，非本轮新增。

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-restore
```

结果：0 Warning(s), 0 Error(s)。

### 4.2 Gallery 基线与优化后对比

命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase transfer --label baseline \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/transfer-showcase-baseline-next.md
```

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase transfer --label target-keyset \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/transfer-showcase-target-keyset.md
```

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase transfer --label target-keyset-rerun \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/transfer-showcase-target-keyset-rerun.md
```

| Set | Mean | Median | P95 | Alloc mean | Visuals | Logical |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold baseline | 855.88 ms | 828.53 ms | 971.73 ms | 42187.17 KB | 1827 | 25 |
| Cold optimized run 1 | 777.62 ms | 781.68 ms | 817.35 ms | 42181.46 KB | 1827 | 25 |
| Cold optimized run 2 | 759.22 ms | 762.69 ms | 777.27 ms | 42173.10 KB | 1827 | 25 |
| Repeated baseline | 242.72 ms | 237.19 ms | 276.83 ms | 37412.95 KB | 1827 | 25 |
| Repeated optimized run 1 | 224.71 ms | 221.91 ms | 247.01 ms | 37394.42 KB | 1827 | 25 |
| Repeated optimized run 2 | 219.53 ms | 216.59 ms | 241.49 ms | 37403.85 KB | 1827 | 25 |

### 4.3 Regression matrix

自动状态验证：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Release --framework net10.0 --no-build -- \
  --verify-transfer-states
```

结果：`Transfer state verification passed.`

本次追加验证：

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-transfer-states
```

结果：0 warning / 0 error；`Transfer state verification passed.`

覆盖：

- `ListTransfer` source side 排除 target keys。
- `ListTransfer` target side 只包含 target keys。
- `TargetKeys = null` 后 source side 恢复完整 items，target side 为空。
- `TreeTransfer` target side 会按树遍历顺序 flatten 嵌套选中节点，`TargetKeys = null` 后 target side 为空。
- `TransferListView.BottomPagination = null` 后不会保留旧 visual parent，也不会被 items 变化复活。

---

## 5. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 `_ignoreXxx` / try-finally 标志位 | 0 |
| 新增 disposable 字段 | 0 |
| 新增订阅 / timer / reparented element | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| Theme Static Rule | 未触发 |
| 生产文件范围 | 5 个文件，均在 `Transfer` |

---

## 6. 后续

- `TransferItemDecorator` 中 `SelectionsIconTemplateProperty` 分支存在重复判断，应独立走 correctness-first 修复，不和本轮 perf 改动混在一起。
- lazy pagination 曾有结构收益但改变默认 `BottomPagination` 存在语义风险；需要更强状态验证后才能重新评估。

---

## 7. 追加结构优化：TransferListView 选中同步去数组快照

`TransferListViewSelectionModel.OnSelectionChanged()` 过去会把 `SelectionModelSelectionChangedEventArgs` 的 `DeselectedItems` / `SelectedItems` 都 `.ToArray()` 后再同步到 `WritableSelectedItems`。Avalonia 12 的 selection changed args 暴露的是 `IReadOnlyList`（`.referenceprojects/Avalonia/src/Avalonia.Controls/Selection/SelectionModelSelectionChangedEventArgs.cs:23-26`），本轮改为保存只读列表引用并按索引遍历。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| DeselectedItems array allocations / selection change | 1 array | 0 arrays | `(1 - 0) / 1` | 100.00% | 结构收益；取消选中同步不再创建快照数组 |
| SelectedItems array allocations / selection change | 1 array | 0 arrays | `(1 - 0) / 1` | 100.00% | 结构收益；选中同步不再创建快照数组 |

说明：这是 Transfer 列表选择变化路径的结构性收益；不声明页面导航 timing 提升。

---

## 8. 追加结构优化：SelectedItems collection changed indexed traversal

`TransferListViewSelectionModel.OnSelectedItemsCollectionChanged()` 在外部 `SelectedItems` 集合增删替换时需要把 item 反查到 selection index。`NotifyCollectionChangedEventArgs.NewItems/OldItems` 已经是 `IList`，本轮把 add/remove helper 从 `foreach` 改为 Count/indexer，避免 collection changed item 列表枚举器。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| SelectedItems add new-items enumerators / add or replace | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；新增 selected items 按 `NewItems.Count/indexer` 同步 |
| SelectedItems remove old-items enumerators / remove or replace | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；移除 selected items 按 `OldItems.Count/indexer` 同步 |

说明：这是外部 selected-items 同步路径的结构性收益；不声明页面导航 timing 提升。

---

## 9. 追加结构优化：只读集合容量快路径

Transfer 的 source panel、selection key、list view / tree view 快照 helper 原先只识别 `ICollection`。当 ItemsSource 或 ItemsView 形态只暴露 `IReadOnlyCollection<IItemKey>` / `IReadOnlyCollection<ITreeItemNode>` 时，临时 List / HashSet 会从默认容量动态增长。本轮补上只读集合 Count 快路径；过滤、树遍历顺序、key 提取和 selection 语义不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Transfer source panel list growth / read-only ItemsSource refresh | dynamic growth | exact read-only count | structural | 结构收益 | 只读 ItemsSource 刷新时按 Count 预分配 |
| Transfer item-key list growth / read-only panel source | dynamic growth | exact read-only count | structural | 结构收益 | selection key list 按只读 panel count 预分配 |
| Transfer list-view key HashSet growth / read-only ItemsView | dynamic growth | exact read-only count | structural | 结构收益 | list view key set 按 Count 预分配 |
| Transfer tree/list snapshot growth / read-only selection source | dynamic growth | exact read-only count | structural | 结构收益 | tree/list 快照按 Count 预分配 |
| Transfer filtering / selection semantics | unchanged | unchanged | n/a | 0.00% | 行为保持 |

说明：这是 Transfer 数据同步路径 structural-only 收益；没有新增页面 timing 对比，不声明页面加载速度提升。

---

## 10. 追加结构优化：空 selected keys 同步短路

`TransferListView.BuildSelectedItemsList()` 和 `TransferTreeView.BuildCheckedItemsList()` 在 `SelectedKeys == null` 或 Count 为 0 时，旧实现仍然扫描完整 `ItemsSource`，最后得到空 selected/checked items。现在空 selected keys 直接返回空列表，避免无意义的数据源扫描；`ItemsSource == null` 仍返回 null，保持原语义。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| TransferListView empty selected-keys source scans / sync | `N` item scans | 0 item scans | `(N - 0) / N` | 100.00% | structural-only；空选中不再遍历列表数据源 |
| TransferTreeView empty selected-keys root scans / sync | `N` root item scans | 0 root item scans | `(N - 0) / N` | 100.00% | structural-only；空选中不再遍历树根数据源 |
| Empty selected result shape | empty mutable `List` | empty mutable `List` | n/a | 0.00% | 行为保持；不改 SelectedItems/CheckedItems 写入形态 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

---

## 11. 追加结构优化：Transfer 树根遍历去临时快照

`TransferTreeView.HandleItemCountChanged()` 和 `MaskNodes()` 原先都会先把 `Items` 复制成 `List<ITreeItemNode>`，再遍历这份快照递归统计 / mask。两条路径都不修改根集合，本轮改为直接遍历 `Items` 并保持原来的强制类型转换语义。

另外，`TransferListView.HandleItemsSourceChange()` 在 `SelectedKeys.Count == 0` 时不再构造完整 `ItemsSource` key set；`TreeTransfer` target 刷新在 `ItemsSource == null` 时直接复用空数组，不再创建空结果列表。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| TransferTreeView item-count root snapshot lists / refresh | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | structural-only；直接遍历根 Items |
| TransferTreeView mask root snapshot lists / mask refresh | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | structural-only；不再复制根节点快照 |
| TransferListView ItemsSource change scans with empty selected keys | `N` item-key scans | 0 scans | `(N - 0) / N` | 100.00% | structural-only；空 selected keys 不需要构造 all-items set |
| TreeTransfer null ItemsSource target result list allocations / refresh | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | structural-only；复用 `Array.Empty` |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

---

## 12. 追加结构优化：TransferListView selection model 同步收敛

`TransferListViewSelectionModel.SyncToSelectedItems()` 原先用 LINQ `SequenceEqual()` 比较 `WritableSelectedItems` 和 `base.SelectedItems`，随后用 `foreach` 回填。两边都有 Count/indexer，本轮改为 Count 先判等、索引逐项比较和索引回填。

`SetListSource()` 原先创建 `oldSelection` 数组并 `CopyTo()`，但后续只判断它是否为 null，不读取数组内容。本轮保留 `WritableSelectedItems` 的懒初始化边界，只去掉数组分配和复制。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| SelectedItems equality LINQ chains / sync-to-selected-items | 1 chain | 0 chains | `(1 - 0) / 1` | 100.00% | structural-only；Count + indexer 比较 |
| SelectedItems equality full scans on count mismatch | up to `N` comparisons | 0 comparisons | `(N - 0) / N` | 100.00% | structural-only；Count 不同直接返回 false |
| SelectedItems refill enumerators / sync-to-selected-items | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | structural-only；索引回填 |
| Source change old-selection snapshot arrays / source change | 1 array | 0 arrays | `(1 - 0) / 1` | 100.00% | structural-only；保留 lazy selected-items 初始化但不复制内容 |
| Source change old-selection CopyTo calls / source change | 1 copy | 0 copies | `(1 - 0) / 1` | 100.00% | structural-only；原数组内容未被读取 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

---

## 13. 追加结构优化：filter source/target flag 判断

`AbstractTransfer` / `ListTransfer` / `TreeTransfer` 在 filter 刷新时都会判断 `FilterChangeType.Source` 和 `FilterChangeType.Target`。本轮把 6 个 `HasFlag()` 改为 bitwise check，并在每次刷新内复用 source/target 两个布尔结果。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Transfer filter change `HasFlag` callsites / refresh | 6 | 0 | `(6 - 0) / 6` | 100.00% | structural-only；source/target 判断不再走 enum helper |
| Source/target refresh semantics | unchanged | unchanged | n/a | 0.00% | Source / Target / Both 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

验证：`--verify-transfer-states` 通过。
