# Calendar 性能优化

> 状态：本轮已完成控件级与 Gallery 实测。

---

## 0. 结论

本轮优化 `CalendarItem` 的初始视图成本：

- Month 初始模式只创建 day title + 42 个 `CalendarDayButton`，不再提前创建 12 个 year/month `CalendarButton`。
- Year / Decade 初始模式只创建 12 个 `CalendarButton`，不再提前创建 42 个 day button 与 day title。
- 切换 `DisplayMode` 时按需补齐另一组 grid cell，已创建的 cell 会复用，不随模式来回切换重复创建。
- 这次没有把 axaml theme 里的固定视觉搬到 C#；Calendar 的 day/month/year cell 本来就是 C# 创建的 variable-N 数据项，属于性能 skill 的允许例外。

| 指标 | baseline | optimized | 改善 |
| --- | ---: | ---: | ---: |
| `Calendar.Default` ms/item | 21.765 | 16.405 | 24.6% |
| `Calendar.Default` KB/item | 3308.2 | 3105.7 | 6.1% |
| `Calendar.Default` visuals/root | 177 | 165 | 12 |
| `Calendar.YearMode` ms/item | 3.952 | 2.021 | 48.9% |
| `Calendar.YearMode` KB/item | 1976.9 | 1148.1 | 41.9% |
| `Calendar.YearMode` visuals/root | 109 | 60 | 49 |
| `Calendar.DecadeMode` ms/item | 4.490 | 2.328 | 48.2% |
| `Calendar.Batch4` ms/item | 29.522 | 21.241 | 28.0% |
| `Calendar.Batch4` KB/item | 10636.1 | 8627.6 | 18.9% |
| `Calendar.Batch4` visuals/root | 573 | 451 | 122 |

`CalendarShowCase` 只有 1 个 `<atom:Calendar />`，页面级 timing 不适合作为主要收益证明。本轮 Gallery 结构和分配下降明确：

| 指标 | baseline | optimized | 改善 |
| --- | ---: | ---: | ---: |
| Gallery cold mean | 114.18 ms | 111.64 ms | 2.2% |
| Gallery cold alloc mean | 4614.54 KB | 4335.19 KB | 6.1% |
| Gallery repeated mean | 39.44 ms | 39.66 ms | -0.6% |
| Gallery repeated alloc mean | 4058.68 KB | 3848.27 KB | 5.2% |
| Gallery visuals | 201 | 189 | 12 |
| Gallery `CalendarButton` trace | 12 | 0 | 12 |

`repeated mean` 的 0.22ms 差异低于这类单控件页面导航测量噪声；P95、alloc、visual tree 均下降。结论按控件级为主：Calendar 独立实例化和 Year/Decade 初始模式收益显著，Gallery 仅确认真实页面结构下降。

本次追加为 structural-only，不声明新的页面 timing 收益：

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Calendar blackout-date range predicate / `ContainsAny` | 1 LINQ `Any(predicate)` | 0 LINQ calls | `(1 - 0) / 1` | 100.00% | 改为显式循环，避免 predicate delegate / LINQ enumerator |
| DatePicker CalendarView blackout-date range predicate / `ContainsAny` | 1 LINQ `Any(predicate)` | 0 LINQ calls | `(1 - 0) / 1` | 100.00% | DatePicker 内部 CalendarView 同步收敛 |

追加验证：

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-calendar-states --verify-datepicker-states
```

结果：0 warning / 0 error；Calendar / DatePicker 状态验证通过。

---

## 1. 资格门槛

Gallery source：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CalendarShowCase.axaml`

| 项 | 数值 |
| --- | ---: |
| `<atom:Calendar>` | 1 |
| Runtime visuals baseline | 201 |
| Runtime `CalendarDayButton` baseline | 42 |
| Runtime `CalendarButton` baseline | 12 |

真实会话操作频率：

- 独立 Calendar 页面导航：至少 1 次。
- Calendar 是 DatePicker 的核心子控件；DatePicker 已 Done，但 Calendar 自身仍需要独立基线和模式切换验证。
- 用户切换 Month / Year / Decade 是 Calendar 的核心交互，初始模式与切换路径都需要保持低成本。

虽然 Gallery 只有 1 个 Calendar，但 Calendar 单实例 visual tree 很重，并且 DatePicker / RangeDatePicker 会复用其交互模型。因此本轮按 Tier 2 单功能重控件处理。

---

## 2. 根因

`CalendarItem.PopulateGrids()` 原先在模板应用时同时填充：

- `MonthView`：7 个 day title + 42 个 `CalendarDayButton`
- `YearView`：12 个 `CalendarButton`

默认 Month 模式不会立即显示 YearView，Year / Decade 模式也不会立即显示 MonthView。提前创建另一组 grid cell 会直接增加实例化、style/template、binding 和 layout tree 统计成本。

这组 cell 是 Calendar 数据项，不是 ControlTheme 中声明的固定视觉。因此本轮只把已有 C# cell 创建按当前 `DisplayMode` 延迟，并在切换时补齐。

---

## 3. 改动

涉及文件：

- `src/AtomUI.Desktop.Controls/Calendar/CalendarItem.cs`
- `src/AtomUI.Desktop.Controls/Calendar/Calendar.cs`

`CalendarItem` 新增两个填充标记：

- `_monthViewPopulated`
- `_yearViewPopulated`

新增按需方法：

- `EnsureMonthViewPopulated()`
- `EnsureYearViewPopulated()`

调用关系：

- `OnApplyTemplate()` 根据 `Owner.DisplayMode` 只填充当前视图。
- `UpdateMonthMode()` 先确保 MonthView 已填充。
- `UpdateYearMode()` / `UpdateDecadeMode()` 先确保 YearView 已填充。
- `Calendar.OnHeaderClick()` 显示 YearView 前确保 YearView 已填充。
- `Calendar.OnMonthClick()` 显示 MonthView 前确保 MonthView 已填充。

同时把 MonthView 预分配容量从 56 修正为 49，匹配实际 7 个 title + 42 个 day button。该变更只影响 pooled list 容量，不改变可见结构。

未纳入的尝试：

- 曾尝试减少 `BaseCalendarButton` / `BaseCalendarDayButton` pseudo-class 更新，但控件级测量变慢，已撤回。

---

## 4. 验证

### 4.1 状态验证

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 -- \
  --verify-calendar-states
```

结果：`Calendar state verification passed.`

覆盖：

- 默认 Month 初始只创建 42 个 day button 和 0 个 `CalendarButton`。
- 初始 Year / Decade 只创建 12 个 `CalendarButton` 和 0 个 day button。
- Month / Year / Decade 循环切换后不重复创建 cell。
- `SelectedDate` / `SelectedDates` 和对应 day button selected 状态在模式切换后保持。

### 4.2 控件级基准

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 -- \
  --suite calendar --count 40 \
  --markdown /tmp/atomui-calendar-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 -- \
  --suite calendar --count 40 \
  --markdown /tmp/atomui-calendar-after-lazy-view.md
```

| Scenario | ms/item baseline | ms/item optimized | KB/item baseline | KB/item optimized | Visual baseline | Visual optimized | Button baseline | Button optimized |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Calendar.Default` | 21.765 | 16.405 | 3308.2 | 3105.7 | 177 | 165 | 59 | 47 |
| `Calendar.SingleDate.Selected` | 10.614 | 9.245 | 3272.0 | 3070.2 | 177 | 165 | 59 | 47 |
| `Calendar.SingleRange.Selected` | 8.136 | 7.730 | 3288.2 | 3086.4 | 177 | 165 | 59 | 47 |
| `Calendar.MultipleRange.Blackout` | 7.869 | 6.633 | 3278.5 | 3076.9 | 177 | 165 | 59 | 47 |
| `Calendar.YearMode` | 3.952 | 2.021 | 1976.9 | 1148.1 | 109 | 60 | 59 | 17 |
| `Calendar.DecadeMode` | 4.490 | 2.328 | 1977.7 | 1148.8 | 109 | 60 | 59 | 17 |
| `Calendar.RangeRestricted` | 8.636 | 6.942 | 3267.9 | 3067.3 | 177 | 165 | 59 | 47 |
| `Calendar.MotionDisabled` | 7.748 | 6.416 | 3222.4 | 3034.7 | 177 | 165 | 59 | 47 |
| `Calendar.Batch4` | 29.522 | 21.241 | 10636.1 | 8627.6 | 573 | 451 | 236 | 128 |

### 4.3 Gallery 导航基准

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 -- \
  --showcase calendar --label calendar-baseline \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/calendar-showcase-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 -- \
  --showcase calendar --label calendar-lazy-view \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/calendar-showcase-after-lazy-view.md
```

Trace:

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 -- \
  --showcase calendar --label calendar-lazy-view-trace \
  --trace-navigation --timeout-ms 30000 \
  --markdown /tmp/calendar-showcase-after-lazy-view-trace.md
```

| Set | Mean | Median | P95 | Alloc mean | Visuals | `CalendarDayButton` | `CalendarButton` |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold baseline | 114.18 ms | 111.64 ms | 126.33 ms | 4614.54 KB | 201 | 42 | 12 |
| Cold optimized | 111.64 ms | 112.42 ms | 114.99 ms | 4335.19 KB | 189 | 42 | 0 |
| Repeated baseline | 39.44 ms | 39.13 ms | 55.67 ms | 4058.68 KB | 201 | 42 | 12 |
| Repeated optimized | 39.66 ms | 42.53 ms | 53.08 ms | 3848.27 KB | 189 | 42 | 0 |

### 4.4 构建

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

## 5. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `_ignoreXxx` 标志位 | 0 |
| 新增 disposable 字段 | 0 |
| 新增事件订阅类型 | 0；仍是原有 cell 事件绑定 |
| axaml 固定视觉搬到 C# | 否 |
| 新增 lazy materialization | 2 个 grid cell 填充入口 |
| 状态验证 | `--verify-calendar-states` |

复杂度集中在 `CalendarItem` 的两个一次性填充标记。由于 cell 创建原本就在 C#，且切换路径只补齐另一组固定数量 cell，风险低于改写 ControlTheme 视觉结构。
