# Cascader 性能优化

> 状态：本轮已完成结构性枚举收敛；未声明新的页面 timing 收益。

---

## 0. 本轮结论

本轮只收敛 `Cascader` / `CascaderView` / `CascaderViewItem` 的选择、过滤、展开和勾选联动枚举成本：

- filter 结果从 `Where().ToList()` 改为按 `_allPathInfos.Count` 预分配并显式筛选。
- path parse 的根 options copy 从 `Cast().ToList()` 改为显式列表构造，结果 path list 按 segment count 预分配。
- 展开路径根节点读取从 `pathNodes.First()` 改为 Count 保护后的 `pathNodes[0]`。
- default selected path 的 header parts 列表按 `TryParseSelectPath()` 返回的 `IList.Count` 预分配。
- 子节点存在性判断统一走 `HasChildren()`，对 `ICollection<T>` / `IReadOnlyCollection<T>` 使用 Count 快路径。
- 父级 checkbox 状态从 `Any()` + `All()` + `Any()` 三次枚举收敛为一次子节点扫描。
- 多选有效标签计算去掉 `All()`、祖先 `Any()`、children `Any()` 的 LINQ 调用。
- 勾选 / 取消勾选子树递归改为复用单个结果 `HashSet`，不再每层递归都创建临时结果集。
- 单选 `SelectedOptionPath` 的 header parts 列表按父链深度预分配。

没有移动主题视觉到 C# 动态创建，没有改 padding、模板槽位、popup 生命周期或 binding 优先级。

---

## 1. 收益

本轮收益是 structural-only，可由代码结构直接证明；没有新增 Gallery before/after timing，因此不把它写成页面耗时提升。

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| filter result LINQ chain / filter | 1 chain | 0 chains | `(1 - 0) / 1` | 100.00% | `Where().ToList()` 改为预分配列表 + 显式筛选 |
| path parse root options copy / parse | 1 chain | 0 chains | `(1 - 0) / 1` | 100.00% | `_options.Cast<ICascaderOption>().ToList()` 改为显式列表构造 |
| expand path root-node LINQ calls / expand | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | `pathNodes.First()` 改为 Count 保护后的 `pathNodes[0]` |
| default selected path header list growth reallocations / loaded default path | dynamic | exact path count | structural | 结构收益 | `options.Count` 已知时直接预分配 |
| child existence LINQ calls / Cascader tree operations | 10 calls | 0 calls | `(10 - 0) / 10` | 100.00% | `Children.Any()` 改为 Count 快路径 helper |
| parent checkbox child scans / ancestor | 3 passes | 1 pass | `(3 - 1) / 3` | 66.67% | `Any()` + `All()` + `Any()` 合并为一次扫描 |
| effective selected option LINQ calls / tag rebuild | 3 calls | 0 calls | `(3 - 0) / 3` | 100.00% | `All()`、祖先 `Any()`、children `Any()` 改为显式循环 |
| option path result list capacity / parse | dynamic growth | exact segment count | structural | 结构收益 | path segments 数已知时直接预分配 |
| filter full-path header list growth / leaf path build | dynamic growth | exact path depth | structural | 结构收益 | 叶子到根路径深度已知时预分配 |
| filter full-path node list growth / leaf path build | dynamic growth | exact path depth | structural | 结构收益 | `ExpandItems` 列表按同一深度预分配 |
| expand path node list growth / item expand | dynamic growth | exact path depth | structural | 结构收益 | 展开路径临时列表按父链深度预分配 |
| parent checked set growth / checkbox cascade | dynamic growth | exact parent depth | structural | 结构收益 | checked / unchecked 父级结果集按父链深度预分配 |
| checked subtree result `HashSet` allocations / 10 checkable nodes | 10 | 1 | `(10 - 1) / 10` | 90.00% | 递归复用同一结果集；实际收益随子树节点数变化 |
| unchecked subtree result `HashSet` allocations / 10 checkable nodes | 10 | 1 | `(10 - 1) / 10` | 90.00% | 递归复用同一结果集；实际收益随子树节点数变化 |
| count-equal checked sync fallback empty HashSet allocations / sync compare | 2 | 0 | `(2 - 0) / 2` | 100.00% | 不可达 defensive fallback 去掉；count-equal 分支已保证两边集合非空 |
| selected option path list growth / single selected option | dynamic growth | exact parent depth | structural | 结构收益 | 单选 path rebuild 按父链深度预分配 |
| Cascader option list growth / read-only option source copy | dynamic growth | exact read-only count | structural | 结构收益 | 只读 option source 复制时按 Count 预分配 |
| Cascader filter full-path cache list growth / first filter | dynamic growth | root option count lower-bound | structural | 结构收益 | 首次 filter path cache 按根 option 数预留初始容量 |

---

## 2. 验证

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-addon-states --verify-datepicker-states --verify-listbox-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-timepicker-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-transfer-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-treeview-states
```

结果：上述构建和状态验证通过。

补充执行：

```bash
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-cascader-states
```

结果：仍失败在既有 lazy-slot 断言（closed Cascader / CascaderViewItem 不应创建部分子控件等）。同一失败列表也出现在 clean HEAD `d35b21a1a`，不是本轮枚举收敛引入的问题，不作为本轮通过项。

---

## 3. 改动文件

| 路径 | 改动 |
| --- | --- |
| `src/AtomUI.Desktop.Controls/Cascader/Cascader.cs` | 有效选中项计算去 LINQ 调用；单选 `SelectedOptionPath` header parts 列表按父链深度预分配；只读 options source 复制按 Count 预分配 |
| `src/AtomUI.Desktop.Controls/Cascader/CascaderOption.cs` | 新增 `HasChildren()` Count 快路径 helper |
| `src/AtomUI.Desktop.Controls/Cascader/CascaderView.Check.cs` | 父级 checkbox 子节点状态一次扫描；勾选 / 取消勾选子树结果集改为单 accumulator |
| `src/AtomUI.Desktop.Controls/Cascader/CascaderView.ExpandAndCollapse.cs` | 展开路径根节点读取走 indexer，子节点判断走 helper |
| `src/AtomUI.Desktop.Controls/Cascader/CascaderView.Filter.cs` | filter 结果预分配并显式筛选；首次 full-path cache 按根 option 数预留容量 |
| `src/AtomUI.Desktop.Controls/Cascader/CascaderView.cs` | path parse 列表构造预分配；只读 options source 复制按 Count 预分配 |
| `src/AtomUI.Desktop.Controls/Cascader/CascaderViewItem.cs` | leaf 状态子节点判断走 helper |

---

## 4. 追加结构优化：path 构造去 Reverse 和 list wrapper

`CascaderView.GetFullPath()` 和 `Cascader.ConfigureSelectedOptionPath()` 都已经先计算父链深度。旧实现仍然从叶子到根 `List.Add()`，再 `Reverse()`，最后 `string.Join()`；本轮改为按已知深度创建数组，并从后往前填充。路径文本、节点顺序和 selected path 语义保持不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Filter full-path header container objects / leaf path build | 1 `List<string>` + 1 backing array | 1 `string[]` | `(2 - 1) / 2` | 50.00% | structural-only；去掉 list wrapper |
| Filter full-path node container objects / leaf path build | 1 `List<ICascaderOption>` + 1 backing array | 1 `ICascaderOption[]` | `(2 - 1) / 2` | 50.00% | structural-only；`ExpandItems` 仍按 index/count 使用 |
| Filter full-path reverse passes / leaf path build | 2 `Reverse()` | 0 | `(2 - 0) / 2` | 100.00% | structural-only；数组倒序填充直接得到根到叶顺序 |
| SelectedOptionPath header container objects / selected option rebuild | 1 `List<string>` + 1 backing array | 1 `string[]` | `(2 - 1) / 2` | 50.00% | structural-only；去掉 selected path list wrapper |
| SelectedOptionPath reverse passes / selected option rebuild | 1 `Reverse()` | 0 | `(1 - 0) / 1` | 100.00% | structural-only；数组倒序填充直接得到根到叶顺序 |
| Path text / expand node order | unchanged | unchanged | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

---

## 5. 追加结构优化：expand path 去 Reverse

`CascaderView.ExpandItemAsync()` 原先按 leaf-to-root 追加待展开路径，再 `Reverse()` 得到 root-to-leaf 顺序。本轮保留 exact depth 的 `List<object>`，先填充占位，再从后往前按 index 写入；根节点检查、wild data 检查和逐级展开顺序保持不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Cascader expand path reverse passes / expand | 1 | 0 | `(1 - 0) / 1` | 100.00% | structural-only；按父链深度倒序填充 |
| Cascader expand path list capacity / expand | exact path depth | exact path depth | n/a | 0.00% | 已有容量收益保持 |
| Expand path order / target node semantics | root-to-leaf after Reverse | root-to-leaf direct fill | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

---

## 6. 追加结构优化：effective tag 祖先判断和默认路径容器收敛

`ShowCheckedStrategy.ShowParent` 需要在构建多选有效标签时排除“已经有完整选中父级”的后代。旧实现对每个 selected option 都遍历一遍 `fullySelectedParents`，再从当前节点向上判断是否属于该 parent；本轮改为直接沿当前 option 的父链向上查 `HashSet`，语义仍然跳过自身，只判断祖先。

同时，`DefaultSelectOptionPath` 解析成功后的 header 文本原先创建 `List<string>` 再 `string.Join()`；`TryParseSelectPath()` 返回的是 `IList<ICascaderOption>`，Count 已知，因此改为 `string[]`，去掉 list wrapper。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Effective tag ancestor candidate scans / selected option | `P` parent candidates | 0 parent-candidate loop | `(P - 0) / P` | P>0 时 100.00% | structural-only；改为父链 HashSet lookup |
| Effective tag ancestor path walks / selected option | up to `P * depth` | up to `depth` | `(P*D - D) / (P*D)` | P>1 时随 P 增大 | 行为保持；自身不算祖先 |
| Empty selected effective tag list allocations / rebuild | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | structural-only；空多选直接复用当前空 selected list |
| Default path header container objects / loaded default path | 1 `List<string>` + 1 backing array | 1 `string[]` | `(2 - 1) / 2` | 50.00% | structural-only；去掉 list wrapper |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

---

## 7. 追加结构优化：checked strategy flag snapshot

`BuildEffectiveSelectedOptions()` 与 TreeSelect 的 effective tag 路径一致，需要判断 `ShowParent` / `ShowChild` 两个 flag。本轮把 `HasFlag()` 改为一次 `ShowCheckedStrategy` 快照 + bitwise check。ShowParent / ShowChild / All 的有效标签语义不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Cascader checked strategy `HasFlag` callsites / effective tag rebuild | 2 | 0 | `(2 - 0) / 2` | 100.00% | structural-only；flag 判断不再走 enum helper |
| Cascader `ShowCheckedStrategy` property reads / effective tag rebuild | 2 | 1 | `(2 - 1) / 2` | 50.00% | structural-only；一次 rebuild 内复用策略快照 |
| Effective tag semantics | unchanged | unchanged | n/a | 0.00% | ShowParent / ShowChild / All 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |
