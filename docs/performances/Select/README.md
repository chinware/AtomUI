# Select 性能优化

> 状态：已完成主线优化；本页记录后续低风险结构性补充。

---

## 1. 追加结构优化：异步选项加载完成后的空判断

`SelectOptionsLoadResult.Data` 的类型是 `IReadOnlyList<ISelectOption>?`，异步加载完成后只需要判断是否有数据来决定是否打开下拉。本轮把 `loadResult?.Data?.Any() == true` 改为 `loadResult?.Data?.Count > 0`，保持空/非空语义不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Async options empty-check LINQ calls / load completion | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 结构收益；异步加载完成路径直接读 `IReadOnlyList.Count` |

说明：这是异步数据加载完成时的结构性收益；未新增 Gallery timing 对比，不声明页面加载速度提升。

## 2. 追加结构优化：单选默认值首项读取

`Select.DefaultValues` 是 `IList<object>?`。单选默认值配置路径只需要在 `DefaultValues.Count > 0` 后取首项，旧实现使用 `DefaultValues.First()`，会额外创建 LINQ enumerator；现在改为 `DefaultValues[0]`，默认选中语义不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Single-select default value first lookup LINQ calls / configure defaults | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 结构收益；直接读 `IList[0]` |
| Default value semantics | first item | first item | n/a | 0.00% | 正确性保持不变 |

说明：这是默认值初始化路径 structural-only 收益；未新增 Gallery timing 对比，不声明页面加载速度提升。

## 3. 追加结构优化：只读集合容量快路径

`BuildSelectedOptionsList()` 的输入类型是 `IEnumerable?`。旧逻辑只识别非泛型 `ICollection`，遇到只实现 `IReadOnlyCollection<ISelectOption>` 的选中集合会退回动态增长。本轮补上只读集合 Count 快路径，枚举顺序和类型转换不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Select selected-options list growth / read-only source copy | dynamic growth | exact read-only count | structural | 结构收益 | 只读选中集合复制时按 Count 预分配 |
| selected option copy semantics | unchanged | unchanged | n/a | 0.00% | 行为保持 |

说明：这是 Select 选择同步路径 structural-only 收益；没有新增页面 timing 对比，不声明页面加载速度提升。

## 4. 验证

- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `--verify-select-states` currently fails on both this change and clean HEAD `d35b21a1a` with the same closed lazy-content / dropdown event-count / TreeSelect expectations, so it is not introduced by this optimization.
