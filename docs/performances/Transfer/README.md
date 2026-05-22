# Transfer 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 1 #2
> 状态：本轮已完成 Gallery 实测。

---

## 0. 结论

本轮只收敛 `ListTransfer` / `TreeTransfer` 在 filter / `TargetKeys` 刷新时的目标 key 查询成本：

- 每次刷新先把非空 `TargetKeys` 转成 `HashSet<EntityKey>`，source / target 两侧复用同一份 membership lookup。
- `TargetKeys` 为空时，target panel 直接使用空集合，避免再扫描完整 `ItemsSource`。
- 修正 target-only filter 刷新时的变更检测顺序：先比较旧 `TargetViewSource`，再写入新值。

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
  -c Debug --framework net10.0 --no-build -- \
  --verify-transfer-states
```

结果：`Transfer state verification passed.`

覆盖：

- `ListTransfer` source side 排除 target keys。
- `ListTransfer` target side 只包含 target keys。
- `TargetKeys = null` 后 source side 恢复完整 items，target side 为空。
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
| 生产文件范围 | 2 个文件，均在 `Transfer` |

---

## 6. 后续

- `TransferItemDecorator` 中 `SelectionsIconTemplateProperty` 分支存在重复判断，应独立走 correctness-first 修复，不和本轮 perf 改动混在一起。
- lazy pagination 曾有结构收益但改变默认 `BottomPagination` 存在语义风险；需要更强状态验证后才能重新评估。
