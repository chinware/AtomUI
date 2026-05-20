# RadioButton 性能优化方案

`RadioButton` 是基础输入控件，也会被表单、筛选条件、配置项和 `RadioButtonGroup` 大量复用。本轮优化只处理 `RadioButton`/`RadioIndicator` 自身的隐形成本；`OptionButtonGroup` 的按钮式单选成本已在独立文档中记录。

优化边界：不改变 public API、视觉状态、交互语义和动画行为；所有 lazy visual 都必须有 re-template、detach 和状态关闭释放路径；优化后不能出现可重复性能回退。

## 优化前判断

已确认瓶颈：

1. `RadioIndicatorTheme.axaml` 默认为每个 radio 创建 `WaveSpiritDecorator#PART_WaveSpirit`。未点击、disabled、初始 checked、`IsWaveSpiritEnabled=false` 的控件都提前承担 wave visual 成本。
2. `RadioButtonGroup` 的 `CheckedItem` 同步只设置新选中项为 true，外部把 `CheckedItem` 切换到其他项或设为 null 时，旧 checked 状态可能残留，这是性能优化前必须优先修复的正确性问题。
3. `HandleRadioButtonCheckedChanged()` 原逻辑在 `SetCurrentValue()` 没有触发属性变更时可能留下 `_ignoreSyncChecked=true`，后续外部同步会被跳过。

真实 `RadioButtonShowCase.axaml` 当前静态形态：

- `RadioButton`: 17 个
- `RadioButtonGroup`: 3 个
- `AntDesignIconProvider`: 21 个
- `Button`: 1 个
- `ShowCaseItem`: 10 个

运行时会生成 21 个 `RadioButton`/`RadioIndicator`。

## 本轮实现

- `RadioIndicatorTheme.axaml` 不再固定创建 wave，模板只保留 `Panel#PART_RootLayout`。
- `RadioIndicator` 在 checked 状态由交互或运行时变化触发时，且 `IsWaveSpiritEnabled && IsMotionEnabled && IsEnabled && IsAttachedToVisualTree()` 成立，才创建并播放 `WaveSpiritDecorator`。
- 当 wave/motion/enable 状态关闭、re-template 或 detach 时，移除 lazy wave、清理 visual parent，并清空 templated parent，避免泄露。
- `RadioButtonGroup` 同步 `CheckedItem` 时会显式清理 stale checked item，并只在目标值变化时写 `IsChecked`，避免不必要 styled property 写入。
- `HandleRadioButtonCheckedChanged()` 使用 `try/finally` 复位 `_ignoreSyncChecked`，避免同步标记残留。
- 工具侧新增 `radiobutton` 控件 suite、`--verify-radiobutton-states` 状态验证，以及真实 `RadioButtonShowCase` 的 RadioButton/RadioIndicator/Wave 计数。

## 最终数据

控件级命令：

```bash
dotnet run --framework net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite radiobutton --count 300
```

代表场景如下。RadioButton 单项时间在 Debug/headless 下有波动，因此这里重点看结构、分配和稳定改善的场景。

| 场景 | 指标 | 优化前 | 优化后 | 变化 |
| --- | --- | ---: | ---: | ---: |
| `RadioButton.Default.Unchecked` | ms/item | 0.901 | 0.599 | -33.52% |
| `RadioButton.Default.Unchecked` | KB/item | 97.7 | 86.4 | -11.57% |
| `RadioButton.Default.Unchecked` | visuals/root | 8 | 7 | 少 1 |
| `RadioButton.Default.Unchecked` | Wave/root | 1 | 0 | 按需创建 |
| `RadioButton.Default.Checked` | ms/item | 0.919 | 0.778 | -15.34% |
| `RadioButton.Default.Checked` | KB/item | 96.1 | 86.0 | -10.51% |
| `RadioButtonGroup.ItemsSource4` | ms/item | 1.329 | 1.129 | -15.05% |
| `RadioButtonGroup.ItemsSource4` | KB/item | 449.0 | 407.9 | -9.15% |
| `RadioButtonGroup.ItemsSource4` | visuals/root | 36 | 32 | 少 4 |
| `RadioButton.GalleryShape.RoundOnly` | ms/item | 8.739 | 7.729 | -11.56% |
| `RadioButton.GalleryShape.RoundOnly` | KB/item | 2616.3 | 2394.1 | -8.49% |
| `RadioButton.GalleryShape.RoundOnly` | visuals/root | 207 | 186 | 少 21 |
| `RadioButton.GalleryShape.RoundOnly` | Wave/root | 22 | 1 | 少 21 |

真实 Gallery 命令：

```bash
dotnet run --framework net10.0 --no-build --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase radiobutton --label after-final-rerun --cold-iterations 10 --warmup 3 --iterations 20 --timeout-ms 20000
```

| 场景 | 指标 | 优化前 | 优化后 | 变化 |
| --- | --- | ---: | ---: | ---: |
| `RadioButtonShowCase` cold | mean ms | 300.49 | 292.19 | -2.76% |
| `RadioButtonShowCase` cold | median ms | 290.08 | 277.24 | -4.43% |
| `RadioButtonShowCase` cold | P95 ms | 393.93 | 340.11 | -13.66% |
| `RadioButtonShowCase` cold | alloc KB | 13500.98 | 13287.96 | -1.58% |
| `RadioButtonShowCase` repeated | mean ms | 100.24 | 99.79 | -0.45% |
| `RadioButtonShowCase` repeated | median ms | 88.53 | 87.05 | -1.67% |
| `RadioButtonShowCase` repeated | P95 ms | 184.13 | 164.47 | -10.67% |
| `RadioButtonShowCase` repeated | alloc KB | 11442.72 | 11212.92 | -2.01% |
| `RadioButtonShowCase` runtime | visuals | 689 | 668 | 少 21 |
| `RadioButtonShowCase` runtime | WaveSpiritDecorator | 22 | 1 | 少 21 |

结论：RadioButton 本体的隐形 visual/wave 成本已经移除，真实 Gallery 页面结构收益明确。页面级 repeated mean 只小幅改善，因为该 ShowCase 此前已经完成 `OptionButtonGroup` 优化，当前页面剩余成本主要来自 route、ShowCaseItem、Icon 和 Gallery 容器层。

## 验证

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --no-restore
dotnet run --framework net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-radiobutton-states
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --no-restore
dotnet run --framework net10.0 --no-build --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase radiobutton --label after-final-rerun --cold-iterations 10 --warmup 3 --iterations 20 --timeout-ms 20000
```

结果：

- `RadioButton state verification passed.`
- `AtomUI.Performance` build passed，0 warnings。
- `AtomUI.GalleryPerformance` build passed；存在一个既有 DataGrid 未使用字段 warning，不属于本次改动。

## Phase 任务

| Phase | 状态 | 内容 |
| --- | --- | --- |
| Phase 0: Baseline 与观测 | Done | 新增 `radiobutton` 控件 suite 和真实 Gallery route 计数 |
| Phase 1: 正确性优先修复 | Done | 修复 `RadioIndicator.IsCheckedProperty` owner、`CheckedItem` stale 状态和 `_ignoreSyncChecked` 残留 |
| Phase 2: Wave 默认成本按需化 | Done | `WaveSpiritDecorator` 从模板预创建改为首次需要播放时创建 |
| Phase 3: 生命周期与泄露验证 | Done | 覆盖 wave disable、motion disable、detach 后释放和 detach 后不重建 |
| Phase 4: Gallery 实测 | Done | 记录 `RadioButtonShowCase` before/after 加载时间和 runtime tree |
| Phase 5: 文档沉淀 | Done | 汇总方案、结果和复现命令 |
