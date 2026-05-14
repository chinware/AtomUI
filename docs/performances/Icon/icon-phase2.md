# Icon Phase 2 结果

Phase 2 聚焦 render 热路径收敛，不改 public API，不缓存 Control 实例，不改变 `IconParkIconsPackage` 与 `MaterialIconsPackages` 依赖的 brush / stroke / theme 扩展点。

## 改动范围

- `DrawingInstruction.Draw()` 不再把 `MatrixTransform` 写入共享 `Geometry.Transform`。
- instruction transform 与 global geometry matrix 改为通过 `DrawingContext.PushTransform(...)` 进入绘制上下文。
- 当最终 transform 是 identity 时，不 push transform。
- 当 `Opacity == 1.0` 时，不 push opacity。
- `BuildPen(icon)` 暂时保持原语义：stroke pen 仍按 instruction 的 custom flags 构建，避免把 `Icon.StrokeWidth` / line cap / line join 错套到原本未声明 custom 的 path 上。

这次的核心边界仍然是“不使用的功能不承担成本”：没有 transform 的普通图标不承担 transform state 成本，没有透明度的普通 path 不承担 opacity state 成本。

## 正确性边界

- 旧路径是在共享 geometry 上临时设置 `Transform = instructionTransform * globalGeometryMatrix`，外层 `Icon.Render()` 已经 push 了 icon scale。
- Avalonia render data 的 `PushTransform` 会把新矩阵按 `Matrix * current` 合入当前 context；在 Avalonia 的 row-vector matrix 语义下，新路径等价于旧的 `point * instructionTransform * globalGeometryMatrix * scale`。
- 非 identity transform 仍进入 `PushTransform` 分支，非 `1.0` opacity 仍进入 `PushOpacity` 分支。
- AntDesign 生成图标中存在 transform path，例如 `ImportOutlined`、`MergeOutlined`、`PinterestOutlined` 等；`IconShowCase` 真实场景渲染 456 个图标，运行时形态保持一致。
- `Icon.TwoTone.Bulb` 覆盖 TwoTone brush 路径；本阶段未改 `FindIconBrush()`、`ProcessBrush()`、provider 或生成代码。

## 控件级结果

Phase 2 的控件级 micro 用 `count=120` 复测，主要用于观察创建加首次稳定渲染的整体趋势。这个口径包含 layout / template / render，不是纯 render-loop，因此时间仍有噪声；Gallery 真实场景结果优先级更高。

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite icon --count 120 \
  --markdown /tmp/icon-micro-phase2-final.md
```

| Scenario | Phase 2 ms/item | Phase 2 KB/item | Visual/root |
| --- | ---: | ---: | ---: |
| Icon.SearchOutlined.Direct | 0.114 | 31.9 | 2.0 |
| Icon.SearchOutlined.Presenter | 0.188 | 41.4 | 3.0 |
| Icon.SearchOutlined.Many10 | 1.796 | 330.9 | 21.0 |
| Icon.LoadingOutlined.Spin | 0.174 | 41.0 | 2.0 |
| Icon.TwoTone.Bulb | 0.139 | 31.8 | 2.0 |
| Icon.Provider.SearchOutlined | 0.136 | 33.2 | 2.0 |
| Icon.HiddenSlots.SelectDefault | 3.230 | 528.8 | 27.0 |
| Icon.HiddenSlots.MenuItemLeaf | 0.839 | 224.0 | 13.0 |

观察：

- 普通 direct / provider / TwoTone 图标保持 Phase 1 后的低成本状态。
- `Many10` 和 `SelectDefault` 的 allocation 继续维持低位，但 visual 数量没有变化；隐藏 slot 的结构性成本仍应放到后续模板阶段处理。
- Phase 2 不解决 `IconPresenter` / `PathIcon` / template 节点数量问题。

## Gallery IconShowCase 结果

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase icon --label icon-phase2-final \
  --iterations 10 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/icon-showcase-navigation-phase2-final.md
```

| Set | Baseline mean ms | Phase 1 mean ms | Phase 2 mean ms | vs Baseline | vs Phase 1 | Baseline alloc KB | Phase 2 alloc KB | Alloc vs Baseline |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | 756.28 | 715.61 | 641.22 | -15.2% | -10.4% | 66222.01 | 55388.48 | -16.4% |
| Repeated navigation | 193.31 | 160.81 | 151.15 | -21.8% | -6.0% | 57900.83 | 47020.96 | -18.8% |

运行时形态保持一致：

| Visuals | Logical | Icon | IconPresenter | PathIcon | IconGallery | IconInfoItem |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 3712 | 11 | 456 | 459 | 456 | 1 | 456 |

## LineEditShowCase 回归

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase lineedit --label icon-phase2-lineedit-final \
  --iterations 10 --warmup 10 --timeout-ms 30000 \
  --markdown /tmp/lineedit-showcase-navigation-icon-phase2-final.md
```

结果：

| Set | Phase 1 mean ms | Phase 2 mean ms | Phase 1 alloc KB | Phase 2 alloc KB | Visuals | Icon | IconPresenter | AddOnDecoratedBox |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Repeated navigation | 182.09 | 158.49 | 55070.89 | 54976.91 | 2358 | 80 | 34 | 93 |

判断：

- 没有观察到 LineEditShowCase 回归；本轮时间样本还更快，但主要应按“无回归”解读。
- allocation 基本持平，说明 Phase 2 的收益主要在 IconShowCase 这种大量图标页面更明显。

## 验证

```bash
dotnet build src/AtomUI.Core/AtomUI.Core.csproj -c Debug --no-restore
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --no-restore
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-build -- --verify-accessories --verify-effective-brushes --verify-addon-states
```

说明：

- `AtomUI.GalleryPerformance` 构建仍保留一个既有 DataGrid 未使用字段 warning：`DataGridColumn._clipboardContentBinding`。
- 本阶段没有新增 event subscription、binding 或缓存，不引入资源释放路径。

## 后续

- Phase 3：评估 AntDesign / Material / IconPark 的静态 bounds 或 matrix 元数据缓存。
- Phase 4：处理 Select/Menu/NavMenu/ToggleIconButton 等隐藏 icon slot 的按需创建。
- 只有在有专门 render-loop baseline 后，才继续尝试 pen cache；目前不为了小幅热路径收益冒 stroke 视觉兼容风险。
