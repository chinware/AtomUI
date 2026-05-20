# GroupBox 性能优化记录

日期：2026-05-17

## 结论

`GroupBox` 本身是轻量容器，但旧模板让所有实例都创建 `PART_HeaderIconPresenter`。真实 `GroupBoxShowCase` 有 9 个 `GroupBox`，只有 1 个设置 `HeaderIcon`，因此 8 个实例承担了未使用功能的成本。

本轮完成低风险优化：

- 无 `HeaderIcon` 时不创建 `PART_HeaderIconPresenter`。
- 设置、替换、清空 `HeaderIcon` 时按需创建、复用和释放 presenter，并清理旧 icon visual parent 与 presenter templated parent。
- 保留原有 XAML selector 样式，代码创建的 presenter 仍由 `GroupBoxTheme.axaml` 控制尺寸、margin、brush 和垂直对齐。
- `Render()` 不再每帧 `TranslatePoint` 计算 header 遮挡区域，改为 `ArrangeOverride()` 缓存。
- 补齐 `Background`、`BorderBrush`、`BorderThickness`、`CornerRadius` 的重绘触发。

未实施项：

- 未移除无标题时的 header layout。该改动会影响边框缺口和标题定位，容易改变 UI 行为，收益不足以承担风险。
- 未把 header text/layout 继续搬到代码中，避免增加实现复杂度。

## 测试口径

控件级：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -- --suite groupbox --count 60 --markdown /tmp/atomui-groupbox-control-after.md
```

状态与生命周期验证：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -- --verify-groupbox-states
```

真实 Gallery：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -- --showcase groupbox --warmup 6 --iterations 40 --cold-iterations 10 \
  --timeout-ms 45000 --label groupbox-after-final \
  --markdown /tmp/atomui-groupbox-gallery-after-final.md
```

环境：Debug / net10.0 / Avalonia Headless / 1300x900。

## 控件级结果

| 场景 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| `GroupBox.Basic` ms/item | 0.977ms | 0.639ms | 快 34.60% |
| `GroupBox.Basic` KB/item | 109.7KB | 96.8KB | 少 11.76% |
| `GroupBox.Basic` visual/root | 11 | 10 | 少 1 |
| `GroupBox.NoHeader` ms/item | 1.011ms | 0.598ms | 快 40.85% |
| `GroupBox.NoHeader` KB/item | 108.5KB | 95.6KB | 少 11.89% |
| `GroupBox.WithIcon` ms/item | 1.564ms | 1.102ms | 快 29.54% |
| `GroupBox.GalleryShape` ms/item | 11.179ms | 10.346ms | 快 7.45% |
| `GroupBox.GalleryShape` KB/item | 1270.5KB | 1160.7KB | 少 8.64% |
| `GroupBox.GalleryShape` visual/root | 104 | 96 | 少 8 |
| `GroupBox.GalleryShape` header icon presenter/root | 9 | 1 | 少 8 |

解释：控件级收益来自默认路径少创建 `IconPresenter` 和对应 icon presenter 样式/布局成本。设置 `HeaderIcon` 的场景仍创建 presenter，但替换/清空路径已有明确释放验证。

## GroupBoxShowCase 真实加载结果

`GroupBoxShowCase.axaml` 源形态：4 个 `ShowCaseItem`，9 个 `GroupBox`，其中 1 个 `GroupBox` 设置 `HeaderIcon`。

运行时结构：

| 指标 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| Visual nodes | 151 | 143 | 少 8 |
| Logical nodes | 35 | 35 | 持平 |
| IconPresenter | 9 | 1 | 少 8 |
| GroupBox | 9 | 9 | 持平 |
| GroupBox header icon presenter | 9 | 1 | 少 8 |
| Repeated alloc mean | 2309.46KB | 2197.57KB | 少 4.84% |

页面加载时间使用同一策略：`cold-iterations=10`、`warmup=6`、`iterations=40`。after 使用最终确认 run：`/tmp/atomui-groupbox-gallery-after-final.md`。

| 指标 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 79.59ms | 66.42ms | 快 16.55% |
| Cold first navigation median | 75.54ms | 65.27ms | 快 13.60% |
| Cold first navigation P95 | 96.84ms | 71.72ms | 快 25.94% |
| Repeated mean | 27.79ms | 26.16ms | 快 5.87% |
| Repeated median | 29.85ms | 25.01ms | 快 16.21% |
| Repeated P95 | 39.16ms | 35.43ms | 快 9.53% |

解释：

- 结构收益稳定：真实页面少 8 个 visual、少 8 个 `IconPresenter`，分配下降约 4.84%。
- 页面耗时收益小于控件级收益，因为 `GroupBoxShowCase` 自身很轻，固定 route、ShowCaseItem 和 layout 调度成本占比高。
- 最终确认 run 中冷启动、repeated mean、repeated median 与 repeated P95 均改善，没有保留可重复性能回退。

## 已实施 Phase

### Phase 0：基线与观测

已完成：

- 新增 `tools/performances/AtomUI.Performance/Suites/GroupBox/GroupBoxScenarios.cs`。
- `TreeStats`/Markdown 输出新增 `GroupBox/root` 与 `GroupBox header icon/root`。
- `AtomUI.GalleryPerformance` 支持 `--showcase groupbox`，并输出真实 route 的 `GroupBox` 与 header icon presenter 计数。

### Phase 1：HeaderIcon Presenter 按需创建

已完成：

- `GroupBoxTheme.axaml` 不再默认创建 `PART_HeaderIconPresenter`。
- 模板新增 `PART_HeaderLayout`，用于代码按需插入 header icon presenter。
- `GroupBox.HeaderIcon` 为 null 时不创建 presenter。
- `HeaderIcon` 设置后创建 presenter；替换 icon 时复用 presenter；清空 icon 时移除 presenter。
- 释放路径清理 `IconPresenter.Icon`、visual parent 和 templated parent。

### Phase 2：样式行为保留

已完成：

- 代码创建的 presenter 仍设置 `Name="PART_HeaderIconPresenter"` 与 `TemplatedParent=this`。
- 原 selector `^ /template/ atom|IconPresenter#PART_HeaderIconPresenter` 保留在 `GroupBoxTheme.axaml`。
- 状态验证覆盖 `VerticalAlignment`、token size、token margin 和 `IconBrush`，防止样式失效。

### Phase 3：Render 热路径收敛

已完成：

- `ArrangeOverride()` 缓存 `_headerOcclusionBounds`。
- `Render()` 使用缓存区域绘制 header 遮挡，不再每帧 `TranslatePoint`。
- `Background ?? Brushes.Transparent` 使用静态 brush，避免每次 render 创建 `SolidColorBrush`。
- 补齐边框相关属性的 `AffectsRender`。

### Phase 4：状态与泄露验证

已完成：

- 无 icon 初始状态不创建 presenter。
- icon 设置、替换、清空、再次设置路径全部验证。
- 替换和清空后旧 icon 不保留 visual parent。
- 移除 presenter 后不保留 visual parent 与 templated parent。
- `HeaderTitle`、`HeaderTitleColor`、`HeaderFontSize`、`HeaderFontStyle`、`HeaderFontWeight` 运行时同步验证。
- `HeaderTitlePosition` 的 Left/Center/Right 运行时同步验证。

### Phase 5：最终对比

本轮符合预期：

- 默认不用 `HeaderIcon` 的 `GroupBox` 不再承担 icon presenter 成本。
- 控件级 `GalleryShape` 少 8 个 visual，耗时快 7.45%，分配少 8.64%。
- 真实 `GroupBoxShowCase` 少 8 个 visual，冷启动和 repeated mean/median/P95 均改善。
