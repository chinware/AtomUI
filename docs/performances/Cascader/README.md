# Cascader 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Data Entry / Cascader  
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
