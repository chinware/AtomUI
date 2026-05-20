# Slider 性能优化记录

## 范围

- 控件：`Slider`、`SliderTrack`、`SliderThumb`。
- Gallery：真实 `SliderShowCase.axaml`，包含 `ShowCaseItem=4`、运行时 `Slider=11`，其中 6 个为 range slider，7 个通过 code-behind 绑定共享 `SliderMarks`。
- 工具：`tools/performances/AtomUI.Performance --suite slider`，`tools/performances/AtomUI.GalleryPerformance --showcase slider`。

## 主要瓶颈与风险

1. `SliderTheme.axaml` 固定创建 `PART_EndThumb`。普通单值 Slider 不使用 range 功能，也会承担第二个 thumb 的 visual、style、measure/arrange 和 tooltip 附加属性成本。
2. `SliderTrack` attached 后固定订阅 `IInputManager.Process`。只有 thumb 获焦后才需要全局 pointer down 判断，默认未聚焦状态不应承担全局输入订阅成本。
3. marks 文本测量和 `FormattedText` 缓存在 `SliderMark` 模型上。多个 slider 共享同一组 marks 时会把 per-track/per-flow/per-enabled 状态写回共享模型，既有正确性风险，也会增加重复文本对象成本。
4. render 热路径会为 mark border 和 thumb circle/outline 反复 new `Pen`。
5. `MarkLabelFontFamily` setter 错写到 `MarkLabelFontSizeProperty`，这是正确性问题，优先修复。
6. EndThumb 从 XAML 模板迁移到代码后，必须保持原有 theme selector、tooltip、motion、enabled、hover、re-template/detach 行为，并明确释放路径，不能引入资源泄露。

## 实施内容

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 0 | 新增 `slider` 控件级 suite、真实 `SliderShowCase` route 和 Slider 状态验证 | Done |
| Phase 1 | `PART_EndThumb` 改为 range 模式按需创建，普通 Slider 不创建第二个 thumb | Done |
| Phase 2 | EndThumb 创建与释放路径补齐：visual/logical child、tooltip attached value、`IsMotionEnabled` `[!]` binding、templated parent 全部清理 | Done |
| Phase 3 | 全局 input 订阅改为 thumb 获焦期间按需订阅，lost focus、detach、thumb replacement 时释放 | Done |
| Phase 4 | marks 文本缓存从 `SliderMark` 模型移出，改为弱引用 marks list 的 bounded cache，避免污染共享模型 | Done |
| Phase 5 | render 热路径复用 `RenderContextData` list 和 mark/thumb `Pen` | Done |
| Phase 6 | tooltip value/host width 写入前去重，tick 计算改用 `MathUtils` | Done |
| Phase 7 | 真实 Gallery 复测、状态验证、文档与中间结果清理 | Done |

## 关键实现取舍

- 普通 Slider 不再承担 EndThumb 成本，符合“不使用的功能不承担成本”。
- Range Slider 仍会在首次 materialize 时创建 EndThumb，这是使用 range 功能必须承担的成本。为了保持原有 SliderTheme selector 行为，生成的 EndThumb 会继承 Slider 的 `TemplatedParent`，释放时清空。
- marks cache 使用 `ConditionalWeakTable<List<SliderMark>, MarkLabelCache>`，cache 生命周期跟 marks list 绑定，每个 marks list 最多保留 8 个不同状态 entry，避免长期静态强引用导致泄露。
- 全局 input subscription 只在 thumb focus 期间存在，释放触发点包括 lost focus、detach、thumb replacement。
- 没有改 public API。

## 控件级结果

口径：Debug / Avalonia Headless / `--suite slider --count 1000`。控件级单 run timing 噪声明显，尤其单个 range 场景会受到场景顺序和首次 style/materialization 影响；因此这里同时列出结构和分配，真实页面结论以 Gallery 加载对比为准。

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `Slider.Default` | `0.631 ms/item`, `111.3 KB/item`, `4 visuals` | `0.550 ms/item`, `86.4 KB/item`, `3 visuals` | 快 `12.84%`，内存少 `22.37%`，少 `1` visual |
| `Slider.WithMarks` | `0.579 ms/item`, `204.9 KB/item`, `4 visuals` | `0.533 ms/item`, `178.0 KB/item`, `3 visuals` | 快 `7.94%`，内存少 `13.13%`，少 `1` visual |
| `Slider.Disabled` | `0.228 ms/item`, `109.5 KB/item`, `4 visuals` | `0.162 ms/item`, `85.1 KB/item`, `3 visuals` | 快 `28.95%`，内存少 `22.28%`，少 `1` visual |
| `Slider.GalleryShape.SliderShowCase` | `4.726 ms/item`, `1735.2 KB/item`, `50 visuals` | `4.281 ms/item`, `1358.3 KB/item`, `44 visuals` | 快 `9.42%`，内存少 `21.72%`，少 `6` visuals |

结构收益最稳定：单值 Slider 的 `PART_EndThumb` 从默认路径移除。严格复现 `SliderShowCase` 的控件级组合中，visual/root 从 `50` 降到 `44`，刚好对应 6 个非 range 或按需路径可移除的隐藏 EndThumb 成本。

## Gallery 加载对比

口径：真实 `SliderShowCase`，Debug / Avalonia Headless / `1300x900` / `--cold-iterations 10 --warmup 5 --iterations 30 --timeout-ms 30000`。Before 与 After 使用相同采样策略。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `SliderShowCase` cold first navigation mean | `144.47 ms` | `137.81 ms` | 快 `6.66 ms`，`4.61%` |
| `SliderShowCase` cold first navigation median | `138.44 ms` | `135.02 ms` | 快 `3.42 ms`，`2.47%` |
| `SliderShowCase` cold first navigation P95 | `165.71 ms` | `155.95 ms` | 快 `9.76 ms`，`5.89%` |
| `SliderShowCase` cold alloc mean | `3581.77 KB` | `3331.44 KB` | 少 `250.33 KB`，`6.99%` |
| `SliderShowCase` repeated mean | `46.31 ms` | `33.78 ms` | 快 `12.53 ms`，`27.06%` |
| `SliderShowCase` repeated median | `41.35 ms` | `36.74 ms` | 快 `4.61 ms`，`11.15%` |
| `SliderShowCase` repeated P95 | `73.91 ms` | `44.49 ms` | 快 `29.42 ms`，`39.81%` |
| `SliderShowCase` repeated alloc mean | `2911.91 KB` | `2718.96 KB` | 少 `192.95 KB`，`6.63%` |
| Runtime visuals | `111` | `105` | 少 `6`，`5.41%` |

用户可见效果：真实 `SliderShowCase` repeated open 从约 `46ms` 降到约 `34ms`，少约 `13ms`；P95 少约 `29ms`。本轮符合优化预期，主要收益来自普通 Slider 不再预创建 EndThumb、marks 文本缓存不污染共享模型、全局 input 订阅按需化。

## 正确性与泄露验证

新增并通过：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  --framework net10.0 --no-build -- --verify-slider-states
```

覆盖内容：

- 单值 Slider 不创建 `PART_EndThumb`。
- `IsRangeMode false -> true -> false -> true` 可以按需创建、释放、重新创建 EndThumb。
- 释放 EndThumb 后没有 visual parent，`TemplatedParent` 清空。
- 生成的 EndThumb 保留 Slider templated parent，确保 Slider template selector 仍能作用到它。
- 垂直 range EndThumb tooltip placement、show delay、formatted value、host width 正确。
- 共享 marks 不再被写入 per-track `FormattedText` 或 `LabelSize` cache。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-slider-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite slider --count 1000
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-build -- --showcase slider --cold-iterations 10 --warmup 5 --iterations 30 --timeout-ms 30000
```
