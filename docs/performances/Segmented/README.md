# Segmented 性能优化记录

## 范围

- 控件：`Segmented`、`SegmentedItem`、`SegmentedStackPanel`。
- Gallery：真实 `SegmentedShowCase.axaml`，源码形态为 `ShowCaseItem=6`、`Segmented=11`、`SegmentedItem=41`、`AntDesignIconProvider=10`。
- 工具：`tools/performances/AtomUI.Performance --suite segmented`，`tools/performances/AtomUI.GalleryPerformance --showcase segmented`。

## 主要瓶颈与风险

1. `SegmentedItemTheme.axaml` 固定创建 `IconPresenter`。`SegmentedShowCase` 只有 10 个 item 有 icon，但 41 个 item 全部创建 presenter，其中 31 个是隐藏成本。
2. `SegmentedStackPanel` measure 阶段会处理隐藏 child，`IsExpanding=True` 也用 `Children.Count` 分配宽度。隐藏 item 既承担布局成本，也可能影响 expanding 宽度。
3. `AbstractSegmented.OnApplyTemplate()` 为默认选中全量扫描 items/container。页面中多个 segmented 时是可避免的初始化成本。
4. `SetupSelectedThumbRect()` 在位置和尺寸不变时仍写 `SelectedThumbPos/SelectedThumbSize`，可能触发不必要 render invalidation。
5. icon presenter 从 XAML 移到代码后必须保持现有 selector、尺寸、颜色和 disabled/selected 状态，同时保证清空 icon、替换 icon、re-template、detach 时释放 visual parent、templated parent 和旧 `PathIcon`。

## 实施内容

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 0 | 新增 `segmented` 控件级 suite、真实 `SegmentedShowCase` route 和状态验证入口 | Done |
| Phase 1 | `SegmentedItem` 的 `IconPresenter` 改为 `Icon != null` 时按需创建，无 icon item 不创建隐藏 presenter | Done |
| Phase 2 | 保守评估 content presenter lazy；收益只覆盖 2 个 icon-only item，未落地，避免增加复杂度 | Evaluated |
| Phase 3 | 默认选中改为 `SelectedIndex < 0` 时设置第 0 项；补充 container 清理标记，避免误清用户直接声明 item | Done |
| Phase 4 | thumb 位置/尺寸写入前用 `MathUtils` 去重，避免无变化时重复写 styled property | Done |
| Phase 5 | `SegmentedStackPanel` measure/arrange 跳过隐藏 item，expanding 按可见 item 数均分，并处理 0 可见项 | Done |
| Phase 6 | transition/dispatcher 成本只评估不改，避免改变动画行为 | Evaluated |
| Phase 7 | 控件级与真实 Gallery 复测、文档和清理 | Done |

## 控件级结果

口径：Debug / Avalonia Headless / `--suite segmented --count 160`。Before 为本轮修改前基线；After 为当前工作区优化后结果。极小场景的 `ms/item` 波动较大，因此结构和分配是更可靠的判断依据。

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `Segmented.TextOnly5` | `6.424 ms/item`, `459.4 KB/item`, `34 visuals`, `5 IconPresenter` | `2.545 ms/item`, `375.1 KB/item`, `29 visuals`, `0 IconPresenter` | 快 `60.38%`，内存少 `18.35%`，少 `5` visuals |
| `Segmented.DisabledMixed5` | `1.453 ms/item`, `448.0 KB/item`, `34 visuals`, `5 IconPresenter` | `0.974 ms/item`, `363.9 KB/item`, `29 visuals`, `0 IconPresenter` | 快 `32.97%`，内存少 `18.77%`，少 `5` visuals |
| `Segmented.GalleryShape.SegmentedShowCase` | `13.817 ms/item`, `4324.3 KB/item`, `309 visuals`, `41 IconPresenter` | `11.000 ms/item`, `3767.2 KB/item`, `278 visuals`, `10 IconPresenter` | 快 `20.39%`，内存少 `12.88%`，少 `31` visuals |
| `Segmented.Batch.FlexPanelControls` | `4.378 ms/item`, `1482.3 KB/item`, `113 visuals`, `16 IconPresenter` | `3.290 ms/item`, `1204.1 KB/item`, `97 visuals`, `0 IconPresenter` | 快 `24.85%`，内存少 `18.77%`，少 `16` visuals |

结构收益最核心：无 icon 的 `SegmentedItem` 不再创建 `IconPresenter`。严格复现 `SegmentedShowCase` 的控件级场景中，`IconPresenter` 从 `41` 降到 `10`，刚好移除 31 个未使用 presenter。

## Gallery 加载对比

口径：真实 `SegmentedShowCase`，Debug / Avalonia Headless / `1300x900` / `--cold-iterations 5 --warmup 3 --iterations 15 --timeout-ms 30000`。Before 与 After 使用相同采样策略。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `SegmentedShowCase` cold first navigation mean | `157.34 ms` | `119.42 ms` | 快 `37.92 ms`，`24.10%` |
| `SegmentedShowCase` cold first navigation median | `151.50 ms` | `119.69 ms` | 快 `31.81 ms`，`20.99%` |
| `SegmentedShowCase` cold first navigation P95 | `179.43 ms` | `126.08 ms` | 快 `53.35 ms`，`29.73%` |
| `SegmentedShowCase` cold alloc mean | `6408.23 KB` | `5840.63 KB` | 少 `567.60 KB`，`8.86%` |
| `SegmentedShowCase` repeated mean | `83.84 ms` | `68.32 ms` | 快 `15.52 ms`，`18.51%` |
| `SegmentedShowCase` repeated median | `82.25 ms` | `68.96 ms` | 快 `13.29 ms`，`16.16%` |
| `SegmentedShowCase` repeated P95 | `121.41 ms` | `81.37 ms` | 快 `40.04 ms`，`32.98%` |
| `SegmentedShowCase` repeated alloc mean | `5927.94 KB` | `5362.68 KB` | 少 `565.26 KB`，`9.54%` |
| Runtime visuals | `376` | `345` | 少 `31`，`8.24%` |
| Runtime IconPresenter | `41` | `10` | 少 `31`，`75.61%` |

用户可见效果：真实 `SegmentedShowCase` repeated open 从约 `84ms` 降到约 `68ms`，少约 `16ms`；冷启动均值少约 `38ms`。这符合“不使用的功能不承担成本”：没有 icon 的 item 不再承担 icon presenter 的 visual、style 和 layout 成本。

## 正确性与泄露验证

新增并通过：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  --framework net10.0 --no-build -- --verify-segmented-states
```

覆盖内容：

- 纯文本 item 不创建 `PART_IconPresenter`，仍保留 `ContentPresenter`。
- `Icon null -> icon -> null -> icon` 会按需创建/释放 presenter。
- 替换 icon 会复用 presenter，并释放旧 `PathIcon` visual parent。
- 清空 icon 会移除 presenter、清空 `TemplatedParent`，并释放当前 `PathIcon`。
- Large/Small icon presenter 保持主题尺寸，disabled/selected icon brush 与文本状态保持一致。
- 未指定选中项时默认选中第 0 项；显式 `IsSelected=True` 不被默认选中覆盖。
- hidden item 在 expanding 布局中不占宽度，全部 hidden 时不产生非法布局。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-segmented-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite segmented --count 160
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-build -- --showcase segmented --cold-iterations 5 --warmup 3 --iterations 15 --timeout-ms 30000
```

`GalleryPerformance` 构建仍有既有 warning：`DataGridColumn._clipboardContentBinding` 未使用，和本轮 Segmented 优化无关。
