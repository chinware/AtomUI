# Timeline 性能评估

> 状态：本轮保留 Render 结构优化和 pending 正确性修复；未声明稳定页面级性能收益。

---

## 0. 结论

本轮最终保留两类改动：

- `TimelineIndicator.Render` 缓存 dot/tail `Pen`，避免每次 paint pass 都 `new Pen(...)`。这是 SKILL Cost Model 对自定义 `Render(DrawingContext)` 的强制性结构要求。
- 修复 pending item 状态：`Pending = null` 会移除旧 pending item；pending item 会真正使用默认 `LoadingOutlined` 或自定义 `PendingIcon`。

`TimelineShowCase` 因 pending 默认 loading icon 被正确挂载，运行时 visual 增加 `336 -> 338`，repeated alloc 增加约 `90.81 KB`。导航 timing 没有稳定收益，因此本轮按 structural + correctness 记录。

| 指标 | baseline | after | 变化 |
| --- | ---: | ---: | ---: |
| Gallery cold mean | 203.88 ms | 263.49 ms | -29.24% |
| Gallery cold median | 202.48 ms | 210.10 ms | -3.76% |
| Gallery cold P95 | 221.06 ms | 443.33 ms | outlier |
| Gallery cold alloc mean | 7596.05 KB | 7675.64 KB | -79.59 KB |
| Gallery repeated mean | 69.65 ms | 75.91 ms | -8.99% |
| Gallery repeated median | 58.32 ms | 75.09 ms | -28.75% |
| Gallery repeated P95 | 107.12 ms | 108.13 ms | -0.94% |
| Gallery repeated alloc mean | 6080.95 KB | 6171.76 KB | -90.81 KB |
| Runtime visuals | 336 | 338 | -2 |
| Runtime logical | 47 | 47 | 0 |

注意：cold mean 的 after 样本含一次 `499.87 ms` outlier；即便看 median，也没有页面级速度收益。

---

## 1. 根因

### 1.1 `TimelineIndicator.Render` 每次创建 Pen

旧实现每次 render 都创建 dot/tail 两个 `Pen`：

```csharp
var dotPen = new Pen(IndicatorColor ?? DefaultIndicatorColor, IndicatorDotBorderWidth);
...
var linePen = new Pen(IndicatorTailColor, IndicatorTailWidth);
```

`Pen` 是 Avalonia `StyledProperty` carrier。headless 导航基准通常只触发有限 paint pass，很难放大单个 `Pen` 分配；但真实持续重绘或 theme/token 变化场景中，每帧创建 `Pen` 是明确的结构成本。

### 1.2 pending item 状态不完整

`Timeline.CreatePendingItem()` 原本创建了 `PendingIcon ?? new LoadingOutlined()`，也设置了 loading spin，但没有把 icon 赋给 pending item：

```csharp
var pathIcon = PendingIcon ?? new LoadingOutlined();
if (pathIcon is Icon icon)
{
    icon.LoadingAnimation = IconAnimation.Spin;
}
var item = new TimelineItem
{
    Content   = Pending,
    IsPending = true
};
```

此外，`AbstractTimeline.SetupPendingItem()` 只在 `Pending != null` 时追加 pending item；当 `Pending` 清空时不会移除旧 pending item。

---

## 2. 改动

### 2.1 `TimelineIndicator` Pen cache

新增实例级缓存：

- dot pen：`IndicatorColor ?? DefaultIndicatorColor` + `IndicatorDotBorderWidth`
- tail pen：`IndicatorTailColor` + `IndicatorTailWidth`

属性变化时清空对应缓存：

- dot：`IndicatorColor` / `DefaultIndicatorColor` / `IndicatorDotBorderWidth`
- tail：`IndicatorTailColor` / `IndicatorTailWidth`

同时把 `DefaultIndicatorColorProperty` 加入 `AffectsRender<TimelineIndicator>`，因为 render fallback 会读取它。

### 2.2 pending 生命周期

`SetupPendingItem()` 先移除旧 pending item，再根据当前 `Pending` 决定是否创建新 pending item。这样覆盖三种状态：

- `Pending` 初次设置：创建 pending item。
- `PendingIcon` 变化：替换 pending item 并使用新 icon。
- `Pending = null`：移除 pending item。

`CreatePendingItem()` 现在把 `pathIcon` 赋给 `TimelineItem.IndicatorIcon`，默认 pending spinner 和自定义 `PendingIcon` 都能显示。

---

## 3. 未保留的尝试

| 候选 | 结果 | 处理 |
| --- | --- | --- |
| 缓存 `TimelineItemPanel` 固定模板子节点 | Gallery timing 没有稳定收益 | 已回滚 |
| `CalculateItemsPositionInfo` 中可见项计数只扫描一次 | Gallery timing 没有稳定收益 | 已回滚 |

---

## 4. 验证

### 4.1 状态验证

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 -- \
  --verify-timeline-states
```

结果：`Timeline state verification passed.`

覆盖：

- `Pending` 创建一个 pending item。
- `Pending = null` 移除 pending item，并恢复用户 item 数量。
- 重新设置 `Pending` 后只创建一个 pending item，并使用最新 content。
- 默认 pending item 使用 `LoadingOutlined`。
- 修改 `PendingIcon` 后仍只有一个 pending item，并使用自定义 icon。

### 4.2 控件级基准

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 -- \
  --suite timeline --count 50 \
  --markdown /tmp/atomui-timeline-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 -- \
  --suite timeline --count 50 \
  --markdown /tmp/atomui-timeline-after.md
```

| Scenario | ms/item baseline | ms/item after | KB/item baseline | KB/item after | Visual baseline | Visual after |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| `Timeline.Basic.Items3` | 3.663 | 3.545 | 450.9 | 453.8 | 32 | 32 |
| `Timeline.Color.Items5` | 5.938 | 5.113 | 653.1 | 653.3 | 46 | 46 |
| `Timeline.Pending.Items3` | 3.801 | 5.930 | 575.7 | 648.8 | 39 | 41 |
| `Timeline.Alternate.Items4` | 3.600 | 4.989 | 574.7 | 576.3 | 39 | 39 |
| `Timeline.Label.Items4` | 3.046 | 3.306 | 558.2 | 560.2 | 39 | 39 |
| `Timeline.Right.Items4` | 2.636 | 1.839 | 580.7 | 582.4 | 39 | 39 |
| `Timeline.Icon.Items4` | 3.043 | 2.406 | 644.8 | 645.1 | 41 | 41 |
| `Timeline.GalleryShape` | 14.795 | 13.164 | 4246.0 | 4319.7 | 275 | 277 |

`Pending` 和 `GalleryShape` 的 visual/alloc 增加来自默认 loading icon 现在正确显示。Pen cache 在该基准口径下不一定能被放大。

### 4.3 Gallery 导航基准

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 -- \
  --showcase timeline --label timeline-baseline \
  --cold-iterations 5 --iterations 20 --warmup 4 --timeout-ms 30000 \
  --markdown /tmp/timeline-showcase-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 -- \
  --showcase timeline --label timeline-after \
  --cold-iterations 5 --iterations 20 --warmup 4 --timeout-ms 30000 \
  --markdown /tmp/timeline-showcase-after.md
```

| Set | Mean | Median | P95 | Alloc mean | Visuals | PathIcon |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold baseline | 203.88 ms | 202.48 ms | 221.06 ms | 7596.05 KB | 336 | 1 |
| Cold after | 263.49 ms | 210.10 ms | 443.33 ms | 7675.64 KB | 338 | 3 |
| Repeated baseline | 69.65 ms | 58.32 ms | 107.12 ms | 6080.95 KB | 336 | 1 |
| Repeated after | 75.91 ms | 75.09 ms | 108.13 ms | 6171.76 KB | 338 | 3 |

---

## 5. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 `_ignoreXxx` 标志位 | 0 |
| 新增 disposable / event / timer | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 新增缓存字段 | 6（2 个 Pen + 4 个 cache key） |
| 生产文件范围 | 3 个文件 |
