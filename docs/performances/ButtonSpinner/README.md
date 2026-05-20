# ButtonSpinner 性能优化

`ButtonSpinner` 是 Navigation/DataEntry 交界处的基础控件，同时也是 `NumericUpDown` 的内部承载结构。本轮目标是让隐藏 handle 和空 addon 不承担固定模板成本，并把 floatable handle 的全局输入订阅生命周期收敛到可验证状态。

## Phase 0 基线

测试环境：Debug，Avalonia headless，`2026-05-15`。

控件级独立 suite：`tools/performances/AtomUI.Performance --suite buttonspinner --count 60`。

| 场景 | ms/item | KB/item | Visual/root | ButtonSpinner | IconButton | Icon | 关键结论 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| `ButtonSpinner.Default` | `3.126` | `470.9` | `25` | `1` | `2` | `2` | 默认单实例成本偏高，固定创建 handle 与两个 icon button |
| `ButtonSpinner.HiddenHandle` | `3.186` | `474.4` | `25` | `1` | `2` | `2` | 关闭 handle 没有节省结构成本 |
| `ButtonSpinner.InnerIconAddOns` | `1.724` | `594.6` | `33` | `1` | `2` | `4` | 内部双 icon addon 成本明显增加 |
| `ButtonSpinner.GalleryShape.Batch24` | `38.159` | `12420.6` | `645` | `24` | `48` | `57` | 与 Gallery 真实 24 个控件形态一致，handle icon 是主要固定成本 |
| `NumericUpDown.Default` | `2.586` | `890.3` | `49` | `1` | `4` | `2` | 作为下游控件，默认成本比 ButtonSpinner 更高，后续应独立分析 |

真实 Gallery：`tools/performances/AtomUI.GalleryPerformance --showcase buttonspinner --iterations 30 --warmup 10`。

| 场景 | Mean | Median | P95 | Alloc mean | Visuals | Logical | ButtonSpinner | IconButton | Icon | AddOnDecoratedBox |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | `238.01ms` | `238.01ms` | `238.01ms` | `16028.38KB` | `725` | `64` | `24` | `48` | `57` | `24` |
| Repeated navigation | `60.16ms` | `56.75ms` | `75.38ms` | `14900.81KB` | `725` | `64` | `24` | `48` | `57` | `24` |

## 实施内容

- `ButtonSpinner` 不再在 `ControlTheme` 中固定声明 `ButtonSpinnerHandle`；改为 `IsButtonSpinnerVisible=true` 时按需创建并设置到 `ButtonSpinnerDecoratedBox.SpinnerContent`。
- `IsButtonSpinnerVisible=false` 时释放 handle，解除 `ButtonsCreated`、上下按钮 `Click` 订阅，并清理 handle 上的代码绑定。
- `ButtonSpinnerDecoratedBox` 的 `PART_SpinnerHandle` presenter 改为按需创建；没有 `SpinnerContent` 时不保留空 presenter。
- `PART_OverlayLayout` 开启 `ClipToBounds`，保证 floatable handle 的隐藏/显示动画不会绘制到外部 addon 区域。
- 外部 `LeftAddOn` / `RightAddOn` presenter 从 ButtonSpinner 专用模板中移除，复用 `AddOnDecoratedBox` 的动态 presenter 创建/释放路径。
- floatable handle 不再在 attached 后常驻全局 `IInputManager.Process` 订阅；pointer 进入当前控件时临时订阅全局坐标，离开、`IsShowHandle=false`、`IsHandleFloatable=false`、`IsEnabled=false` 或 detach 时统一释放，避免泄露并保持 TextBox/overlay/addon 组合下的打开和隐藏语义。
- floatable pointer move 热路径只在 hover 状态变化时更新 handle visual state，避免每个 move 都重复写属性。
- 补充 `--verify-buttonspinner-states`，覆盖 handle 显隐切换、旧 visual parent 清理、floatable 订阅开关、detach 清理、outer addon presenter 运行时创建/释放。

## Phase 记录

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 0 | 建立控件级和真实 Gallery 基线，补充 ButtonSpinner runtime/source 统计 | Done |
| Phase 1 | floatable 全局输入订阅生命周期收敛 | Done |
| Phase 2 | `ButtonSpinnerHandle` 按 `IsButtonSpinnerVisible` 创建/释放 | Done |
| Phase 3 | spinner handle presenter 按 `SpinnerContent` 创建/释放 | Done |
| Phase 4 | 外部 addon presenter 复用 AddOnDecoratedBox 动态创建路径 | Done |
| Phase 5 | 状态切换、visual parent、订阅释放验证 | Done |
| Phase 6 | 控件级与真实 Gallery 复测、文档沉淀 | Done |

## 最终结果

控件级 suite，Debug headless，`--suite buttonspinner --count 60`：

| 场景 | 优化前 ms/item | 优化后 ms/item | 变化 | 优化前 KB/item | 优化后 KB/item | 改善 | 优化前 Visual/root | 优化后 Visual/root |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `ButtonSpinner.Default` | `3.126` | `3.158` | `-1.02%` | `470.9` | `418.0` | `11.23%` | `25` | `23` |
| `ButtonSpinner.HiddenHandle` | `3.186` | `1.417` | `55.52%` | `474.4` | `212.5` | `55.21%` | `25` | `10` |
| `ButtonSpinner.RightIconAddOn` | `1.819` | `1.437` | `21.00%` | `534.8` | `485.1` | `9.29%` | `29` | `28` |
| `ButtonSpinner.InnerIconAddOns` | `1.724` | `1.382` | `19.84%` | `594.6` | `536.1` | `9.84%` | `33` | `31` |
| `ButtonSpinner.GalleryShape.Batch24` | `38.159` | `33.837` | `11.33%` | `12420.6` | `10993.7` | `11.49%` | `645` | `601` |
| `NumericUpDown.Default` | `2.586` | `2.387` | `7.70%` | `890.3` | `832.6` | `6.48%` | `49` | `47` |

真实 `ButtonSpinnerShowCase`，Debug headless，`1300x900`，warmup 10，measured 30：

| 指标 | 优化前 | 优化后 | 变化 |
| --- | ---: | ---: | ---: |
| Cold first navigation | `238.01ms` | `214.34ms` | `9.94%` |
| Cold alloc | `16028.38KB` | `14505.48KB` | `9.50%` |
| Repeated mean | `60.16ms` | `61.31ms` | `-1.91%` |
| Repeated median | `56.75ms` | `60.57ms` | `-6.73%` |
| Repeated P95 | `75.38ms` | `83.67ms` | `-11.00%` |
| Repeated alloc | `14900.81KB` | `13549.38KB` | `9.07%` |
| Runtime visuals | `725` | `681` | `6.07%` |
| Runtime logical | `64` | `64` | `0.00%` |

结论：本轮优化完成了结构性减重，尤其隐藏 handle 场景符合“不使用的功能不承担成本”；真实 Gallery 的 visual 和 allocation 明确下降，cold navigation 有收益。Repeated navigation timing 没有稳定改善，说明 `ButtonSpinnerShowCase` 主要剩余时间不只来自这 44 个 visual，后续如果继续压 timing，需要进一步拆 `IconButton`/`Icon` 组合、`ShowCaseItem`、Gallery route 稳定过程，而不是继续在 ButtonSpinner 外壳上做小改。

## 验证覆盖

- `IsButtonSpinnerVisible=false` 初始状态不创建 `ButtonSpinnerHandle`、`IconButton` 和 `PART_SpinnerHandle` presenter。
- `false -> true -> false -> true` 切换时 handle 可恢复，旧 handle 无 visual parent 残留，新旧 handle 不复用。
- `PART_SpinnerHandle` presenter 随 handle 按需创建/释放，释放后清理 visual parent 与 templated parent，重新显示时创建新 presenter。
- `PART_SpinnerHandle` presenter 的 `HorizontalAlignment` 跟随 `ButtonSpinnerLocation` 切换。
- floatable handle 只有在 attached、enabled、show handle、floatable 四个条件同时成立时才订阅全局 input manager。
- `IsButtonSpinnerVisible`、`IsButtonSpinnerFloatable`、`IsEnabled` 和 detach 都会释放活动期 pointer tracking subscription。
- 默认无外部 addon 时不创建 `PART_LeftAddOn` / `PART_RightAddOn`；运行时设置 addon 会创建，清空 addon 会释放且旧 presenter 无 visual parent。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
```

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-buttonspinner-states
```

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite buttonspinner --count 60 \
  --markdown /tmp/atomui-buttonspinner-control.md
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj
```

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase buttonspinner --iterations 30 --warmup 10 --timeout-ms 30000 \
  --label buttonspinner-optimized \
  --markdown /tmp/atomui-buttonspinner-gallery.md
```
