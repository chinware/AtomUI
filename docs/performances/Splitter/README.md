# Splitter 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 2 #6
> 状态：本轮已完成控件级与 Gallery 长样本实测。

---

## 0. 结论

本轮优化有效，但收益主要来自创建/布局路径的轻量化，不改变 Splitter 运行时视觉结构：

- collapse `IconButton` 保留在 ControlTheme 中，符合 Theme Static Rule；隐藏或不可折叠时不再构造和保留 `PathIcon`。
- lazy 拖拽预览复用已有 `TranslateTransform`，避免每个 drag delta 重新分配 transform。
- `SplitterHandle` 转发拖拽事件时复用原始 `VectorEventArgs`，不再为每个 started/delta/completed 事件复制参数对象。
- `Splitter.Children` 动态 add/remove 同步到 `SplitterPanel.Children` 时不再通过 `OfType<Control>().ToList()` 创建 LINQ iterator + `List<T>`，改为一次 exact-size `Control[]` 拷贝。
- `SplitterPanel` 刷新内部 panel 缓存时复用 `_panels`，不再通过 `Children.ToList()` 创建临时 `List<T>` 和 backing array。
- resize 事件上报 sizes 时改为一次 exact-size `double[]`，去掉 `Select(...).ToArray()` 后外层再 `.ToArray()` 的双重数组分配。

Gallery 首轮 `warmup=5 / iterations=30` 出现 repeated mean 噪声回退，但同一轮 alloc 已下降。用临时 worktree 对修改前代码补跑 `warmup=15 / iterations=50` 后，当前代码在 cold、repeated、P95、alloc 上均为正向；最终结论采用长样本口径。

相关 Avalonia 成本依据：

- `IsVisible=False` 会短路 measure / arrange / render：`.referenceprojects/Avalonia/src/Avalonia.Base/Layout/Layoutable.cs:546`、`:671`；`.referenceprojects/Avalonia/src/Avalonia.Base/Rendering/ImmediateRenderer.cs:34`
- `RenderTransform` 属于 compositor 侧变换路径：`.referenceprojects/Avalonia/src/Avalonia.Base/Rendering/Composition/Server/ServerCompositionVisual/ServerCompositionVisual.DirtyInputs.cs:89-118`

---

## 1. 控件级基线

baseline 命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite splitter --count 60 \
  --markdown /tmp/splitter-control-baseline.md
```

current 复测命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite splitter --count 60 \
  --markdown /tmp/splitter-control-current.md
```

第二次 current 复测同命令，输出到 `/tmp/splitter-control-current-rerun.md`。控件级 headless timing 单次噪声较明显，所以这里同时列出两次 current 和平均改善；alloc 取两次一致值或均值。

| Scenario | baseline ms/item | current ms/item run1 | current ms/item run2 | avg timing 改善 | baseline KB/item | current KB/item | alloc 改善 | 判断 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| `Splitter.Basic.Vertical2` | 1.129 | 1.089 | 1.042 | 5.62% | 200.1 | 193.6 | 3.25% | 有效 |
| `Splitter.Basic.Horizontal2` | 1.154 | 1.060 | 0.999 | 10.79% | 201.9 | 195.3 | 3.27% | 有效 |
| `Splitter.Multi.Vertical4` | 4.203 | 4.443 | 3.630 | 3.96% | 565.3 | 545.9 | 3.43% | timing 抖动，alloc 有效 |
| `Splitter.Collapsible.Always3` | 6.182 | 4.295 | 5.815 | 18.23% | 922.0 | 922.0 | 0.00% | timing 有效，alloc 持平 |
| `Splitter.Collapsible.Hidden3` | 2.642 | 1.399 | 2.126 | 33.29% | 371.5 | 358.7 | 3.45% | 有效 |
| `Splitter.Lazy.Pair` | 2.733 | 1.412 | 2.412 | 30.04% | 408.1 | 395.0 | 3.21% | 有效 |
| `Splitter.GalleryShape` | 13.570 | 9.655 | 14.524 | 10.91% | 3130.7 | 3061.2 | 2.22% | timing 噪声，alloc 有效 |

控件级结论：分配下降是稳定信号；`Hidden3`、`Lazy.Pair` 和 collapse always 路径的 timing 两次复测均为正向。`Multi.Vertical4`、`GalleryShape` 的控件级 timing 有单次反向，不能单独作为收益证明；最终有效性以 Gallery 长样本为主。

### 1.1 动态 Children 同步结构收益

本轮追加优化点：`Splitter.Children` 运行时 add/remove 时，旧路径会对 `NotifyCollectionChangedEventArgs.NewItems/OldItems` 执行 `OfType<Control>().ToList()`，每次至少创建 1 个 LINQ iterator、1 个 `List<Control>` 和其 backing array。现在直接按 `items.Count` 拷贝到 `Control[]` 后传给 `SplitterPanel.Children.InsertRange/RemoveAll`，保持批量同步语义，但少一个 iterator 和一个 List 对象。

后续追加优化点：`SplitterPanel.RefreshPanelsAndHandles` 不再用 `Children.ToList()` 建临时列表；拖拽 resize 的 started/delta/completed 事件不再先 `Select(...).ToArray()` 再外层 `.ToArray()`，改为一次手写 exact-size `double[]` 填充。该收益是运行时结构性分配下降，不声明页面加载 timing 提升。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Splitter child add/remove temp objects / collection change | 3 objects | 1 array | `(3 - 1) / 3` | 66.67% | 结构性分配下降；只声明动态 children 变更路径收益 |
| SplitterPanel refresh temp objects / refresh | 2 objects | 0 objects | `(2 - 0) / 2` | 100.00% | 结构性分配下降；只声明内部 panel 缓存刷新路径收益 |
| Splitter resize event sizes temp objects / resize event | 3 objects | 1 array | `(3 - 1) / 3` | 66.67% | 结构性分配下降；只声明拖拽事件上报 sizes 路径收益 |

---

## 2. Gallery 真实场景

最终口径命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase splitter --label splitter-current-long \
  --cold-iterations 10 --iterations 50 --warmup 15 --timeout-ms 30000 \
  --markdown /tmp/splitter-gallery-current-long.md
```

baseline 取自临时 worktree `/tmp/AtomUIV6-splitter-baseline-codex`，代码为本轮修改前的 `066d915e9`。

| 指标 | baseline | current | 改善 | 结论 |
| --- | ---: | ---: | ---: | --- |
| Cold first navigation mean | 177.55 ms | 154.96 ms | 12.72% | 有效 |
| Cold first navigation median | 162.21 ms | 152.06 ms | 6.26% | 有效 |
| Cold first navigation P95 | 253.34 ms | 175.33 ms | 30.80% | 有效 |
| Cold alloc mean | 6380.36 KB | 6256.48 KB | 1.94% | 小幅有效 |
| Repeated navigation mean | 36.91 ms | 34.66 ms | 6.10% | 有效 |
| Repeated navigation median | 33.12 ms | 32.62 ms | 1.51% | 小幅有效 |
| Repeated navigation P95 | 51.79 ms | 40.43 ms | 21.93% | 有效 |
| Repeated alloc mean | 5770.31 KB | 5659.40 KB | 1.92% | 小幅有效 |
| Runtime visuals | 319 | 319 | 0 | 结构不变 |
| Runtime logical | 31 | 31 | 0 | 结构不变 |

补充：首轮同参数 `warmup=5 / iterations=30` 的 current repeated mean 为 `50.94 ms`，高于 baseline `49.15 ms`，但样本前段存在明显预热坡度；长样本复测未复现该退化。

---

## 3. 改动

| 路径 | 改动 |
| --- | --- |
| `src/AtomUI.Desktop.Controls/Splitter/SplitterHandle.cs` | 隐藏 collapse button 时清空 `Icon`；仅 visible 时创建 collapse icon；拖拽事件转发复用原始 `VectorEventArgs` |
| `src/AtomUI.Desktop.Controls/Splitter/SplitterPanel.cs` | lazy preview 复用同一个 `TranslateTransform` 并只更新 `X/Y`；内部 panel 缓存刷新去掉 `Children.ToList()`；resize event sizes 改为一次数组填充 |
| `tools/performances/AtomUI.Performance/Suites/Splitter/SplitterStateVerification.cs` | 状态验证按 Theme Static Rule 调整为“模板按钮保留，但隐藏态不持有 icon”；新增 transform reuse、动态 children 同步、resize sizes 上报验证 |

---

## 4. 验证

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0
```

结果：0 Warning(s), 0 Error(s)。

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-splitter-states
```

结果：`Splitter state verification passed.`

后续追加复测：

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-splitter-states
```

结果：构建 `0 Warning(s), 0 Error(s)`；`Splitter state verification passed.`

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug --framework net10.0
```

结果：构建通过；保留既有 warning `DataGridColumn._clipboardContentBinding is never used`，非本轮新增。

---

## 5. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 `_ignoreXxx` 标志位 | 0 |
| 新增 disposable / event / timer | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 生产文件范围 | 2 个文件 |
