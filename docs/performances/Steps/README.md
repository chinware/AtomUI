# Steps 性能评估

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 2 #4
> 状态：本轮保留结构正确性修复；未声明稳定性能收益。

---

## 0. 结论

`Steps.ConfigureItemsPanel()` 在重建 Grid definitions 时，原代码连续清理两次 `RowDefinitions`，没有清理 `ColumnDefinitions`：

```csharp
_grid.RowDefinitions.Clear();
_grid.RowDefinitions.Clear();
```

本轮改为：

```csharp
_grid.RowDefinitions.Clear();
_grid.ColumnDefinitions.Clear();
```

这修复了横向/纵向切换或重复配置时 stale `ColumnDefinitions` 残留的结构问题。实测没有稳定页面级性能收益，因此按 structural-only 记录。

| 指标 | baseline | grid definitions fix | 变化 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 862.02 ms | 849.17 ms | +1.49% |
| Cold first navigation median | 851.95 ms | 846.98 ms | +0.58% |
| Cold first navigation P95 | 921.30 ms | 870.78 ms | +5.48% |
| Cold alloc mean | 67080.95 KB | 67085.26 KB | -0.01% |
| Repeated navigation mean | 333.91 ms | 340.19 ms | -1.88% |
| Repeated navigation median | 330.58 ms | 334.98 ms | -1.33% |
| Repeated navigation P95 | 360.92 ms | 368.61 ms | -2.13% |
| Repeated alloc mean | 63217.74 KB | 63219.08 KB | 0.00% |
| Runtime visuals | 2513 | 2513 | 0 |
| Runtime logical | 215 | 215 | 0 |

---

## 1. 评估过但未保留的候选

| 候选 | 结果 | 处理 |
| --- | --- | --- |
| `StepsItemIndicator` progress `Pen` cache | 首轮看似改善，但 rerun repeated timing 不稳定/退化 | 已回滚 |

---

## 2. 复现命令

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase steps --label baseline \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/steps-showcase-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase steps --label grid-definitions \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/steps-showcase-grid-definitions.md
```

---

## 3. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 `_ignoreXxx` 标志位 | 0 |
| 新增 disposable / event / timer | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 生产文件范围 | 1 个文件，1 行逻辑修正 |
