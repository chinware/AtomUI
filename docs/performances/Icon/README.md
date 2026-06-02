# Icon 性能优化

> 状态：已完成页面级主优化；本轮追加 `PulseEasing` structural-only 收口。

既有主收益来自 Icon metadata / transform literal 优化：`IconShowCase` repeated `193.31ms -> 173.36ms`，alloc `57900.83KB -> 46331.65KB`。本轮不重新声明页面 timing 收益，只记录 `PulseEasing` 动画采样路径的结构性下降。

## PulseEasing Step Lookup

`PulseEasing` 原先在类型初始化时用 `Enumerable.Range().Select().ToArray()` 构造 9 个固定 step，并在每次 `Ease(progress)` 里调用 `Last(predicate)` 查找小于等于 progress 的最大 step。step 值固定且数量很小，可以用静态数组字面量和反向 `for` 保持同样查找语义，去掉 LINQ 链路。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| `PulseEasing` type-init LINQ operators | 3 operators | 0 operators | `(3 - 0) / 3` | 100.00% | 结构收益；固定 step 不再通过 Range/Select/ToArray 构建 |
| `PulseEasing.Ease()` LINQ lookup callsites / animation sample | 1 `Last(predicate)` | 0 `Last(predicate)` | `(1 - 0) / 1` | 100.00% | 结构收益；直接反向 for 查找 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Verification

```
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-restore
```
