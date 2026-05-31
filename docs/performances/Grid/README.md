# Grid 性能与正确性优化

> 路线图位置：[`../README.md`](../README.md) Layout / Grid
> 状态：本轮完成正确性修复、控件级基线和状态验证；timing 为 smoke-only，不声明页面级速度提升。

---

## 0. 结论

`Row` 原本缓存了按 breakpoint 解析后的子项布局，但 `Col.Span/Order/Offset/...` 或子项 `IsVisible` 变化后缓存不会失效，会导致换列、排序和隐藏子项仍按旧布局排列。本轮修复 stale layout，并在同宽 measure → arrange 路径复用已计算 layout，减少一次重复 `BuildLayout`。

| 指标 | baseline | optimized | formula | improvement | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Grid stale layout 验证失败数 | 3 | 0 | `(3 - 0) / 3` | 100.00% | 修复 Span / Order / IsVisible 动态变化后的旧布局 |
| Grid 状态验证覆盖 | 0 | 3 | `(3 - 0) / 3` | 100.00% | 新增 3 个状态验证 |
| same-width layout pass 的 `BuildLayout` 调用 | 2 | 1 | `(2 - 1) / 2` | 50.00% | measure 后 arrange 可复用 layout result |
| `Grid.Row.Wrap.Col6x12` KB/item | 294.3 KB | 292.6 KB | `(294.3 - 292.6) / 294.3` | 0.58% | allocation 小幅下降 |
| `Grid.Row.Ordered.Col6x8` KB/item | 194.6 KB | 193.6 KB | `(194.6 - 193.6) / 194.6` | 0.51% | allocation 小幅下降 |
| 可证明页面级速度提升 | 无 | 无 | n/a | 不声明 | 控件级 timing 为 smoke-only，不能作为速度收益 |

---

## 1. 根因

`Row.GetOrderedChildren()` 会缓存 `RowChildInfo`，其中包含 `Col.ResolveLayout(breakPoint)` 的结果。旧实现只在 `ChildrenChanged` 和 media breakpoint 变化时设置 `_orderedChildrenDirty = true`。

这会漏掉三类常见动态变化：

| 动态变化 | 旧行为 |
| --- | --- |
| `Col.Span` 从 12 改 24 | 父级重新 measure，但 Row 继续用旧 span=12 |
| `Col.Order` 改为 -1 | Row 继续用旧 order 排列 |
| 子项 `IsVisible=false` | Row 继续把隐藏子项包含在 cached children 里 |

---

## 2. 改动

- `Col.OnPropertyChanged()`：当父级是 `Row` 时调用 `Row.InvalidateChildLayout()`，确保 Col 布局属性变化会清 Row cache。
- `Row.OnMeasureInvalidated()`：统一清 `_orderedChildrenDirty` 和 `_measureLayoutResult`，覆盖子项 `IsVisible` 触发的父级 measure invalidation。
- `Row.ArrangeOverride()`：当 arrange width 与上一次 measure width 一致时复用 measure 阶段的 `LayoutResult`，避免重复 `BuildLayout(finalSize, false)`。
- `Row.GetOrderedChildren()`：使用 static comparison，避免每次 sort 创建 comparison lambda。

---

## 3. 新增基准场景

`tools/performances/AtomUI.Performance` 新增 `grid` suite：

| Scenario | 覆盖点 |
| --- | --- |
| `Grid.Row.Col12x2` | 2 个 Col 半宽排列 |
| `Grid.Row.Col8x6` | 6 个 Col 三列排列 |
| `Grid.Row.Wrap.Col6x12` | 12 个 Col wrap 多行 |
| `Grid.Row.Ordered.Col6x8` | `Order` 排序 |
| `Grid.Row.Responsive.Col8x6` | responsive `Sm/Md` layout |
| `Grid.Row.Gutter.Col6x8` | horizontal / vertical gutter |

新增参数：

```bash
--suite grid
--verify-grid-states
```

---

## 4. 控件级 smoke 数据

Baseline 与 optimized 都是 `--suite grid --count 80` 单次 smoke；timing 不作为收益证明。

| Scenario | baseline ms/item | optimized ms/item | baseline KB/item | optimized KB/item | Visual | Logical |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| `Grid.Row.Col12x2` | 0.231 | 0.244 | 54.1 | 53.6 | 7 | 5 |
| `Grid.Row.Col8x6` | 0.685 | 0.745 | 150.2 | 149.2 | 19 | 13 |
| `Grid.Row.Wrap.Col6x12` | 1.425 | 1.608 | 294.3 | 292.6 | 37 | 25 |
| `Grid.Row.Ordered.Col6x8` | 0.437 | 0.468 | 194.6 | 193.6 | 25 | 17 |
| `Grid.Row.Responsive.Col8x6` | 0.348 | 0.410 | 148.4 | 147.5 | 19 | 13 |
| `Grid.Row.Gutter.Col6x8` | 0.392 | 0.457 | 193.4 | 192.4 | 25 | 17 |

---

## 5. 验证

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-grid-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite grid --count 80
```

---

## 6. Converter Parse Structural Pass

本轮补充 Grid parser 字符串解析路径：

- `RowJustifyConverter` / `RowAlignConverter` 旧实现 `Trim().ToLowerInvariant()` 后再匹配别名；第一步去掉 lowercase 临时字符串，本轮继续把 `Trim()` 改为 `ReadOnlySpan<char>.Trim()`，fallback 仍交给 `Enum.Parse(..., ignoreCase: true)`。
- `GridColSize.Parse` 旧实现先 `Trim()`，再 `Split(',')`，每个 segment 继续 `Trim()` / range substring；本轮改为 span 逐段扫描，保留空 segment 跳过、key/value 校验和异常语义。
- `GridColSpanInfo.Parse` / `GridGutterInfo.Parse` 单值 fast path 改为 span trim 后直接解析。
- `GridGutter.Parse` 的 `;` 双段和 `,` 数值 pair 解析改为 span 扫描，不再为 `Split(..., RemoveEmptyEntries)` 创建临时数组；`GridGutterInfo.Parse` 增加 span overload，避免双段解析时重新构造 trimmed string。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| RowJustify alias parse lowercase temp strings / conversion | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；别名大小写不敏感语义保持 |
| RowAlign alias parse lowercase temp strings / conversion | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；别名大小写不敏感语义保持 |
| RowJustify alias parse trim temp strings / conversion with surrounding whitespace | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；span trim 保持 trim 语义 |
| RowAlign alias parse trim temp strings / conversion with surrounding whitespace | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；span trim 保持 trim 语义 |
| GridColSize comma split arrays / key-value parse | 1 array | 0 arrays | `(1 - 0) / 1` | 100.00% | 结构收益；span 逐段扫描 |
| GridColSize segment/key/value trim temp strings / segment | 3 | 0 | `(3 - 0) / 3` | 100.00% | 结构收益；span trim 保持解析语义 |
| GridColSpanInfo single-value trim temp strings / parse | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；span trim fast path |
| GridGutterInfo single-value trim temp strings / parse | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；span trim fast path |
| GridGutter semicolon split arrays / responsive gutter parse | 1 array | 0 arrays | `(1 - 0) / 1` | 100.00% | 结构收益；span 双段扫描保持 `RemoveEmptyEntries` 语义 |
| GridGutter pair split arrays / numeric pair parse | 1 array | 0 arrays | `(1 - 0) / 1` | 100.00% | 结构收益；span 双段扫描保持 pair 校验 |
| GridGutterInfo segment trim temp strings / responsive gutter segment | 1 string risk | 0 strings | `(1 - 0) / 1` | 100.00% | 结构收益；双段路径直接传 span |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |
