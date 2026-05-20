# Result 性能优化

## 背景

`Result` 是 Feedback 类控件，Gallery 的 `ResultShowCase` 一次性展示 8 个 Result：4 个普通状态、3 个错误码插画状态、1 个自定义图标状态。

优化前每个 `Result` 模板都会同时创建：

- `PART_StatusIconPresenter`
- `PART_ErrorCodeImage`

这导致普通状态也承担隐藏 `Svg` 成本，错误码状态也承担隐藏状态图标 presenter 成本。Gallery 实际只需要 3 个错误码 `Svg`，但运行时会创建 8 个 `Svg`。

## Phase 0：基线与观测

基线结论：

- `ResultShowCase` 视觉上只有 8 个 Result，但 `Svg/root` 为 `8`，其中 5 个属于未使用的隐藏错误图。
- 每个 Result 固定多创建一个不会使用的视觉节点，Gallery 下累积为 8 个多余 visual。
- 错误码 `Svg` 解析和 source 设置有明显波动，单次控件级耗时不稳定，最终判断以长样本真实 Gallery route 为主。

## Phase 1-3：低风险模板减重

实施内容：

- 从 `ResultTheme.axaml` 移除模板固定创建的 `PART_StatusIconPresenter` 和 `PART_ErrorCodeImage`。
- 保留 `PART_RootLayout` 作为稳定插入点，并把 `Frame`/`RootLayout` 命名统一为 `PART_Frame`/`PART_RootLayout`。
- 普通状态按需创建 `ContentPresenter#PART_StatusIconPresenter`。
- `ErrorCode403/404/500` 按需创建 `Svg#PART_ErrorCodeImage`。
- 状态切换时立即移除旧 visual，清空 `Content`/`Source`，并清理 `TemplatedParent`。
- 默认状态图标按当前状态缓存，避免同一状态重复创建默认 `PathIcon`。

生命周期边界：

- 本次没有新增事件订阅和外部 binding disposable。
- 所有代码创建 visual 都由 `Result` 自身持有，释放触发点为 re-template 或状态互斥切换。
- `Svg.Source` 在离开错误码状态时清空，避免旧 SVG 字符串继续挂在已分离 visual 上。

## Phase 4：正确性修复

顺带修复了一个正确性问题：

- `HeaderLineHeight` 原先监听 `FontSizeProperty`，但实际计算使用 `HeaderFontSize`。
- `SubHeaderLineHeight` 原先监听 `FontSizeProperty`，但实际计算使用 `SubHeaderFontSize`。
- 现在分别监听 `HeaderFontSizeProperty` 和 `SubHeaderFontSizeProperty`，运行时改字号会同步更新 line-height。

## 控件级结果

口径：`AtomUI.Performance --suite result --count 160`。控件级单次耗时受 SVG 解析和 GC 波动影响，主要看结构和分配；耗时只作为辅助参考。

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `Result.GalleryShape.ResultShowCase` time | `15.512ms/item` | `14.289ms/item` | 快 `7.88%` |
| `Result.GalleryShape.ResultShowCase` alloc | `5887.3KB/item` | `5770.5KB/item` | 少 `116.8KB/item` |
| `Result.GalleryShape.ResultShowCase` visuals | `190/root` | `182/root` | 少 `8/root` |
| `Result.GalleryShape.ResultShowCase` Svg | `8/root` | `3/root` | 少 `5/root` |

结构解释：

- 8 个 Result 每个少 1 个不会使用的 visual。
- 3 个错误码 Result 保留 `Svg`，但不再创建状态 icon presenter。
- 5 个普通/自定义 icon Result 保留状态 icon presenter，但不再创建隐藏 `Svg`。

## Gallery ShowCase 加载时间

口径：真实 Gallery route `ResultShowCase`，`warmup=5`，`iterations=30`，`cold-iterations=5`，cold 为独立进程多样本。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | `274.52ms` | `251.79ms` | 快 `8.28%` |
| Cold first navigation median | `267.42ms` | `253.69ms` | 快 `5.13%` |
| Cold first navigation P95 | `300.98ms` | `275.74ms` | 快 `8.39%` |
| Cold alloc | `9308.47KB` | `9184.80KB` | 少 `123.67KB` |
| Repeated mean | `74.01ms` | `67.19ms` | 快 `9.21%` |
| Repeated median | `71.06ms` | `53.32ms` | 快 `24.97%` |
| Repeated P95 | `108.85ms` | `105.49ms` | 快 `3.09%` |
| Repeated alloc | `7671.48KB` | `7553.19KB` | 少 `118.29KB` |
| Runtime visuals | `265` | `257` | 少 `8` |
| Runtime Svg | `8` | `3` | 少 `5` |

结论：符合优化预期。这个控件单页只有 8 个 Result，页面级百分比不会特别大；但“未使用功能不承担成本”的目标达成，真实 Gallery 长样本没有出现可重复性能下降。

## 验证

已补充 `--verify-result-states`，覆盖：

- 普通状态只创建状态 icon presenter，不创建错误码 `Svg`。
- 错误码状态只创建错误码 `Svg`，不创建状态 icon presenter。
- `Success -> ErrorCode403 -> Warning -> CustomIcon` 状态切换会 detach 旧 visual 并恢复目标 visual。
- `Svg.Source` 离开错误码状态后清空。
- 代码按需创建的状态图标仍能应用主题尺寸。
- `HeaderFontSize` 和 `SubHeaderFontSize` 变化会更新 line-height。

复现命令：

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-result-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite result --count 160
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-build -- --showcase result --warmup 5 --iterations 30 --cold-iterations 5
```
