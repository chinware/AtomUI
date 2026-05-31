# Cascader 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Data Entry / Cascader  
> 状态：本轮已完成结构性枚举收敛；未声明新的页面 timing 收益。

---

## 0. 本轮结论

本轮只收敛 `Cascader` / `CascaderView` / `CascaderViewItem` 的选择、过滤、展开和勾选联动枚举成本：

- filter 结果从 `Where().ToList()` 改为按 `_allPathInfos.Count` 预分配并显式筛选。
- path parse 的根 options copy 从 `Cast().ToList()` 改为显式列表构造，结果 path list 按 segment count 预分配。
- 子节点存在性判断统一走 `HasChildren()`，对 `ICollection<T>` / `IReadOnlyCollection<T>` 使用 Count 快路径。
- 父级 checkbox 状态从 `Any()` + `All()` + `Any()` 三次枚举收敛为一次子节点扫描。
- 多选有效标签计算去掉 `All()`、祖先 `Any()`、children `Any()` 的 LINQ 调用。

没有移动主题视觉到 C# 动态创建，没有改 padding、模板槽位、popup 生命周期或 binding 优先级。

---

## 1. 收益

本轮收益是 structural-only，可由代码结构直接证明；没有新增 Gallery before/after timing，因此不把它写成页面耗时提升。

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| filter result LINQ chain / filter | 1 chain | 0 chains | `(1 - 0) / 1` | 100.00% | `Where().ToList()` 改为预分配列表 + 显式筛选 |
| path parse root options copy / parse | 1 chain | 0 chains | `(1 - 0) / 1` | 100.00% | `_options.Cast<ICascaderOption>().ToList()` 改为显式列表构造 |
| child existence LINQ calls / Cascader tree operations | 10 calls | 0 calls | `(10 - 0) / 10` | 100.00% | `Children.Any()` 改为 Count 快路径 helper |
| parent checkbox child scans / ancestor | 3 passes | 1 pass | `(3 - 1) / 3` | 66.67% | `Any()` + `All()` + `Any()` 合并为一次扫描 |
| effective selected option LINQ calls / tag rebuild | 3 calls | 0 calls | `(3 - 0) / 3` | 100.00% | `All()`、祖先 `Any()`、children `Any()` 改为显式循环 |
| option path result list capacity / parse | dynamic growth | exact segment count | structural | 结构收益 | path segments 数已知时直接预分配 |

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

结果：仍失败在既有 lazy-slot 断言（closed Cascader / CascaderViewItem 不应创建部分子控件等）。这些断言覆盖的是历史 Cascader lazy materialization 目标，不作为本轮枚举收敛的通过项。

---

## 3. 改动文件

| 路径 | 改动 |
| --- | --- |
| `src/AtomUI.Desktop.Controls/Cascader/Cascader.cs` | 有效选中项计算去 LINQ 调用 |
| `src/AtomUI.Desktop.Controls/Cascader/CascaderOption.cs` | 新增 `HasChildren()` Count 快路径 helper |
| `src/AtomUI.Desktop.Controls/Cascader/CascaderView.Check.cs` | 父级 checkbox 子节点状态一次扫描 |
| `src/AtomUI.Desktop.Controls/Cascader/CascaderView.ExpandAndCollapse.cs` | 展开路径子节点判断走 helper |
| `src/AtomUI.Desktop.Controls/Cascader/CascaderView.Filter.cs` | filter 结果预分配并显式筛选 |
| `src/AtomUI.Desktop.Controls/Cascader/CascaderView.cs` | path parse 列表构造预分配 |
| `src/AtomUI.Desktop.Controls/Cascader/CascaderViewItem.cs` | leaf 状态子节点判断走 helper |
