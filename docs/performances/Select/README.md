# Select 性能优化

> 状态：已完成主线优化；本页记录后续低风险结构性补充。

---

## 1. 追加结构优化：异步选项加载完成后的空判断

`SelectOptionsLoadResult.Data` 的类型是 `IReadOnlyList<ISelectOption>?`，异步加载完成后只需要判断是否有数据来决定是否打开下拉。本轮把 `loadResult?.Data?.Any() == true` 改为 `loadResult?.Data?.Count > 0`，保持空/非空语义不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Async options empty-check LINQ calls / load completion | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 结构收益；异步加载完成路径直接读 `IReadOnlyList.Count` |

说明：这是异步数据加载完成时的结构性收益；未新增 Gallery timing 对比，不声明页面加载速度提升。
