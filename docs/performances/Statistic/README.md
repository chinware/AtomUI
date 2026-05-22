# Statistic 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 2 #2
> 状态：本轮已完成正确性、生命周期与 Gallery 实测收尾。

---

## 0. 结论

本轮没有保留 Skeleton / Theme 结构层面的性能改动，最终只保留了对 `Statistic` 和 `TimerStatistic` 的低风险正确性、生命周期修复：

- `Statistic` 明确区分内部生成的 `Content` 与用户显式设置的 `Content`。
- `Value`、分隔符和精度变化后，内部生成的文本会继续同步；`Value = null` 会清空内部生成文本。
- 显式 `Content` 不会被后续 `Value` 更新覆盖。
- 替换 `StatisticCountUp` 内容时，会释放旧内容指向 owner 的 `DataContext`。
- `TimerStatistic` 只在进入 visual tree 后创建 `DispatcherTimer`，detach 时停止并释放。
- countdown 结束只触发一次 `CountdownFinished`，并把 `RemainingTime` clamp 到 `TimeSpan.Zero`。

`StatisticShowCase` 的主要收益体现在真实页面的分配和稳定视觉树形态：

| 指标 | baseline | optimized | 改善 |
| --- | ---: | ---: | ---: |
| Gallery cold mean | 248.01 ms | 235.08 ms | 5.2% |
| Gallery cold alloc mean | 8813.85 KB | 8638.98 KB | 174.87 KB |
| Gallery repeated mean | 61.78 ms | 62.69 ms | -1.5% |
| Gallery repeated median | 51.80 ms | 50.44 ms | 2.6% |
| Gallery repeated alloc mean | 7471.87 KB | 7228.74 KB | 243.13 KB |
| Gallery repeated visuals | 387 | 384 | 3 |

`repeated mean` 和 P95 在这轮样本里仍有噪声，因此本轮不宣称稳定页面速度提升；以 Gallery 分配下降、重复导航 visual tree 少 3 个、timer 生命周期释放和状态正确性作为主要结论。

---

## 1. 资格门槛

Gallery source：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/StatisticShowCase.axaml`

| 项 | 数值 |
| --- | ---: |
| `<atom:Statistic>` ready 条件 | >= 9 |
| `<atom:TimerStatistic>` ready 条件 | >= 6 |
| `<atom:StatisticCountUp>` ready 条件 | >= 2 |
| Runtime visuals baseline | 390 cold / 387 repeated |
| Runtime `SkeletonLine` baseline | 17 |

真实会话操作频率：

- 独立 `StatisticShowCase` 页面导航：至少 1 次。
- dashboard / summary / analytics 类页面会天然集中使用多个 `Statistic`。
- `TimerStatistic` 在 attach 后长期 tick，必须有明确的创建和释放路径；这属于生命周期风险，不只是启动分配问题。

`Statistic` 是 Tier 2 单功能控件，实例数不如 Select / TreeView 高，但 `TimerStatistic` 的常驻 timer 和 `StatisticCountUp` 的 owner binding 都值得补状态验证。

---

## 2. 根因

### 2.1 `Statistic.Content` ownership 不清晰

旧实现只在初始化时把 `EffectiveValue` 写入 `Content`。后续 `Value`、`Precision`、分隔符变化会更新 `EffectiveValue`，但不会同步到已生成的 `Content`。同时，控件无法区分 `Content` 是内部生成，还是用户显式提供的 `StatisticCountUp` / 自定义内容。

这个问题会带来两类后果：

- 默认文本路径可能显示 stale value。
- 如果简单在每次 `Value` 变化时写 `Content`，又会覆盖用户显式 `Content`。

### 2.2 `StatisticCountUp.DataContext` 缺少替换清理

`Content` 为 `StatisticCountUp` 时会把 owner 作为 `DataContext`。旧内容被替换后没有清空这个指针，容易留下旧 child 到 owner 的引用。

### 2.3 `TimerStatistic` timer 生命周期过早

旧实现会在 `Value` 或 `RefreshDuration` 变化时直接创建 `DispatcherTimer`，即使控件还没有进入 visual tree。detach 后虽然会释放 timer，但 pre-attach 创建会把未显示控件也放进计时器生命周期。

同时 countdown 结束路径依赖 `RemainingTime <= TimeSpan.Zero`，需要显式验证只触发一次并停在零。

---

## 3. 改动

涉及文件：

- `src/AtomUI.Desktop.Controls/Statistic/Statistic.cs`
- `src/AtomUI.Desktop.Controls/Statistic/TimerStatistic.cs`
- `tools/performances/AtomUI.Performance/Suites/Statistic/StatisticStateVerification.cs`

`Statistic` 新增 generated content ownership：

- `_generatedContent`
- `_isUsingGeneratedContent`
- `_isInitialized`

默认 `Content == null` 时调用 `SetGeneratedContent(EffectiveValue)`；后续 value/format 变化只在 `Content` 为空或仍是内部生成内容时同步。用户显式设置 `Content` 后，`_isUsingGeneratedContent` 会关闭，后续 value 变化不再覆盖。

`TimerStatistic` 新增 attach-gated timer 生命周期：

- `_isAttachedToVisualTree`
- `_isCountdown`
- `_hasCountdownFinished`
- `ReleaseTimer()`
- `HasTimerTarget()`
- `NotifyCountdownFinished()`

`BuildTimer()` 会先释放旧 timer，再捕获当前是 countdown 还是 countup。timer 只在 attach 后、且 `Value != default` 时创建；detach 时停止、解绑 Tick 并置空。

---

## 4. 未保留的尝试

本轮曾尝试过多种 Skeleton active 路径优化，包括：

- 在 Statistic theme 里加 `IsActive="{TemplateBinding IsLoading}"`。
- 用 style trigger 驱动 `Skeleton.IsActive`。
- 在 C# 里同步 `PART_Skeleton` 状态。
- 给 Skeleton 增加内部 effective active 状态。

这些候选要么造成 `Statistic.Loading` 分配回退，要么把 theme 固定视觉的状态逻辑转移到 C#，不符合性能 skill 的 Theme Static Rule。按 3-round 预算规则，Skeleton 相关性能改动全部撤回；只保留 correctness / lifecycle 修复和状态验证。

同时删除了旧的 `VerifyStatisticLazySkeleton` 验证。该验证要求 `IsLoading=false` 时动态移除 `SkeletonLine`，这本身违反 Theme Static Rule：Statistic theme 中固定声明的 Skeleton 视觉应保留在 axaml，由 `IsVisible` / 状态控制可见性，而不是运行时动态创建和释放。

---

## 5. 验证

### 5.1 状态验证

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 -- \
  --verify-statistic-states
```

结果：`Statistic state verification passed.`

覆盖：

- 默认生成 `Content` 后，`Value` 变化会同步文本。
- `Value = null` 会清空内部生成文本。
- 显式 `Content` 不会被 generated value 覆盖。
- 替换 `StatisticCountUp` 时旧 child 的 `DataContext` 会清空。
- `TimerStatistic` attach 前不创建 timer，attach 后创建，detach 后释放。
- countdown 结束事件只触发一次，`RemainingTime` 停在零。

### 5.2 控件级基准

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 -- \
  --suite statistic --count 40 \
  --markdown /tmp/atomui-statistic-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 -- \
  --suite statistic --count 40 \
  --markdown /tmp/atomui-statistic-after.md
```

控件级 suite 的普通 `Statistic` 场景 visual tree 保持不变；`TimerStatistic` ms/item 在 Debug headless 下波动明显，因此不作为本轮主要收益证明。最终看 Gallery 真实页面的分配、visual tree 和生命周期验证。

基线摘要：

| Scenario | ms/item | KB/item | Visual/root |
| --- | ---: | ---: | ---: |
| `Statistic.Basic` | 1.586 | 283.6 | 18 |
| `Statistic.Precision` | 1.809 | 289.5 | 18 |
| `Statistic.Loading` | 1.438 | 285.6 | 16 |
| `Statistic.PrefixSuffix` | 2.518 | 355.2 | 21 |
| `Statistic.CountUp` | 2.116 | 314.5 | 19 |
| `TimerStatistic.Default` | 1.629 | 308.7 | 18 |
| `TimerStatistic.MillisecondFormat` | 2.022 | 313.2 | 18 |
| `Statistic.GalleryShape` | 49.162 | 7556.7 | 337 |

### 5.3 Gallery 导航基准

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 -- \
  --showcase statistic --label statistic-baseline \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/statistic-showcase-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 -- \
  --showcase statistic --label statistic-optimized \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/statistic-showcase-after.md
```

| Set | Mean | Median | P95 | Alloc mean | Visuals | TextBlock | SkeletonLine |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold baseline | 248.01 ms | 244.71 ms | 272.39 ms | 8813.85 KB | 390 | 42 | 17 |
| Cold optimized | 235.08 ms | 234.00 ms | 253.07 ms | 8638.98 KB | 390 | 42 | 17 |
| Repeated baseline | 61.78 ms | 51.80 ms | 105.72 ms | 7471.87 KB | 387 | 39 | 17 |
| Repeated optimized | 62.69 ms | 50.44 ms | 109.85 ms | 7228.74 KB | 384 | 36 | 17 |

### 5.4 构建

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0
```

结果：两个构建均通过。

---

## 6. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `_ignoreXxx` 标志位 | 0 |
| 新增 axaml 固定视觉动态创建 | 0 |
| 新增 timer 字段 | 0；复用原 `_timer`，补 release helper |
| 新增事件订阅 | 0；原有 Tick 订阅现在有明确 release |
| 新增状态验证 | `--verify-statistic-states` 覆盖 generated content、CountUp DataContext、timer lifecycle、countdown finish |
| 保留 Skeleton 性能实验 | 否，全部撤回 |
