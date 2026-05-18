# OptionButtonGroup 性能优化

`OptionButtonGroup` 是 `RadioButtonShowCase` 中按钮式单选的底层体系，也会被表单、筛选条件和分段选择类场景复用。本轮优化遵守边界：不改变 public API、视觉状态、动画行为和交互语义；所有按需创建的 visual 都有释放路径；优化后不能出现可重复性能回退。

本轮已完成 Phase 0-7。

## 主要结论

- 文本按钮不再为未使用的 `IconPresenter` 和 `WaveSpiritDecorator` 付费。
- 带 icon 的按钮仍按需创建 `IconPresenter`，保留原有 `IconPresenter#IconPresenter` selector 尺寸行为。
- wave decorator 改为首次 press/release 后才创建；禁用 wave、re-template、detach 都会释放。
- `AbstractOptionButtonGroup.Render()` 收敛重复 `ItemCount`、`BorderThickness`、`RenderOptions`、`Pen` 和容器查询成本。
- 修正 `OptionButtonToken.PaddingLG/PaddingSM` 计算错误，避免大小号 padding 使用错误高度。

## 优化前瓶颈

1. **默认文本按钮固定创建 icon slot**

   旧模板每个 `OptionButton` 都创建 `IconPresenter`，再通过 `Icon == null` 隐藏。真实 `RadioButtonShowCase` 运行时有 63 个 `OptionButton`，只有 21 个设置 icon，因此 42 个 `IconPresenter` 是纯隐藏成本。

2. **wave 默认预创建**

   `WaveSpiritDecorator` 只有用户交互触发水波时才需要，但旧模板每个按钮都固定创建。真实页面 63 个按钮会提前创建 63 个 wave 节点。

3. **Group render 热路径有重复计算**

   绘制分割线和选中边框时重复读取 `ItemCount`、`BorderThickness`、创建 `RenderOptions` 和 `Pen`，并且旧逻辑通过 `Items[i] as AbstractOptionButton` 更新位置，对 ItemsSource 容器场景不够稳。

## 本轮实现

- `OptionButtonTheme.axaml` 模板保留轻量 `PART_RootLayout` 和 `ContentLayout`，移除固定 `IconPresenter` 与固定 `WaveSpiritDecorator`。
- `AbstractOptionButton` 增加按需 visual 生命周期：
  - `Icon != null` 且模板存在时创建 `IconPresenter`。
  - `Icon == null`、re-template、detach 时移除 presenter，清理 `Icon`、`IconBrush` 和 templated parent。
  - wave 只在按下释放后且 `IsWaveSpiritEnabled=True` 时创建。
  - wave 禁用、re-template、detach 时移除 decorator，依赖 `WaveSpiritDecorator.OnDetachedFromVisualTree()` 取消动画 CTS。
- `IconPresenter.IconBrush` 使用 `[!]` 绑定到 `Foreground`，因为 source/target 生命周期由 `OptionButton` 同步管理，不引入额外 disposable binding。
- `AbstractOptionButtonGroup` 的位置计算改为 `ContainerFromIndex()`，避免 ItemsSource 场景把数据项当成容器。
- 增加 `--verify-optionbuttongroup-states`，覆盖 icon 创建/清理、wave 创建/清理、selection 和 position trait。
- `GalleryPerformance` 增加真实 `RadioButtonShowCase` 路由，保证按 Gallery 实际 XAML 和运行时视觉树测量。

## 最终数据

控件级命令：

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite optionbuttongroup --count 300
```

| 场景 | 指标 | 优化前 | 优化后 | 改善 |
| --- | --- | ---: | ---: | ---: |
| `Text.Solid3` | ms/item | 2.851 | 1.985 | 30.37% |
| `Text.Solid3` | KB/item | 360.6 | 280.6 | 22.18% |
| `Text.Solid3` | visuals/root | 22 | 16 | 少 6 |
| `Text.Solid3` | IconPresenter/root | 3 | 0 | 少 3 |
| `Text.Solid3` | Wave/root | 3 | 0 | 少 3 |
| `Text.Outline3` | ms/item | 1.823 | 0.973 | 46.63% |
| `Text.Outline3` | KB/item | 344.9 | 264.1 | 23.43% |
| `Icon.Outline3` | ms/item | 1.153 | 0.985 | 14.57% |
| `Icon.Outline3` | KB/item | 443.4 | 391.5 | 11.70% |
| `GalleryShape.RadioButtonShowCase` | ms/item | 26.705 | 23.154 | 13.30% |
| `GalleryShape.RadioButtonShowCase` | KB/item | 8176.3 | 6649.1 | 18.68% |
| `GalleryShape.RadioButtonShowCase` | visuals/root | 493 | 388 | 少 105 |
| `GalleryShape.RadioButtonShowCase` | IconPresenter/root | 63 | 21 | 少 42 |
| `GalleryShape.RadioButtonShowCase` | Wave/root | 63 | 0 | 少 63 |

真实 Gallery 命令：

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase radiobutton --label after-long --warmup 3 --iterations 20 --cold-iterations 10 --timeout-ms 20000
```

| 场景 | 指标 | 优化前 | 优化后 | 改善 |
| --- | --- | ---: | ---: | ---: |
| `RadioButtonShowCase` cold | mean ms | 284.21 | 261.90 | 7.85% |
| `RadioButtonShowCase` cold | median ms | 287.53 | 263.72 | 8.28% |
| `RadioButtonShowCase` cold | P95 ms | 308.17 | 274.34 | 10.98% |
| `RadioButtonShowCase` cold | alloc KB | 15123.46 | 13495.81 | 10.76% |
| `RadioButtonShowCase` repeated | mean ms | 100.85 | 84.72 | 15.99% |
| `RadioButtonShowCase` repeated | median ms | 80.53 | 73.14 | 9.18% |
| `RadioButtonShowCase` repeated | P95 ms | 179.36 | 139.89 | 22.01% |
| `RadioButtonShowCase` repeated | alloc KB | 13001.33 | 11443.10 | 11.99% |
| `RadioButtonShowCase` runtime | visuals | 794 | 689 | 少 105 |
| `RadioButtonShowCase` runtime | IconPresenter | 63 | 21 | 少 42 |

说明：短样本曾出现 repeated mean 噪声性变差，因此最终采用相同口径的 10 次 cold 独立样本 + 20 次 repeated 样本。长样本下没有可重复性能回退。

## 验证

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --no-restore
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-optionbuttongroup-states
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --no-restore
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj --no-restore
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase radiobutton --warmup 3 --iterations 20 --cold-iterations 10 --timeout-ms 20000
git diff --check
```

## Phase 0-7 任务状态

| Phase | 状态 | 内容 |
| --- | --- | --- |
| Phase 0: 基线与观测 | Done | 新增 `optionbuttongroup` 控件 suite，补真实 `RadioButtonShowCase` Gallery 路由 |
| Phase 1: 默认模板减重 | Done | 移除默认 `IconPresenter` 和 `WaveSpiritDecorator` 固定节点 |
| Phase 2: Icon slot 按需创建 | Done | `Icon != null` 才创建 presenter，清空/re-template/detach 释放 |
| Phase 3: Wave slot 按需创建 | Done | 首次交互才创建 wave，禁用和 detach 释放 |
| Phase 4: Render 热路径收敛 | Done | 缓存渲染循环中的重复状态与绘制对象 |
| Phase 5: 状态和生命周期验证 | Done | 覆盖 icon/wave 创建、清理、selection 和 position trait |
| Phase 6: token 正确性修复 | Done | 修复大小号 padding 计算 |
| Phase 7: Gallery 实测和文档 | Done | 记录真实 `RadioButtonShowCase` 前后对比和复现命令 |
