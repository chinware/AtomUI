# Spin 性能优化

> 状态：本轮完成 `SpinIndicator` 动画生命周期收敛；包含一处正确性修复。

---

## 0. 结论

本轮没有把 `Spin` 模板里的 `MaskLayout` / `SpinIndicator` 搬到 C# 动态创建。它们属于 ControlTemplate 功能视觉，继续留在 axaml，通过 `IsVisible` 控制显示，这是当前性能守则允许且风险最低的方式。

实际优化点是 `SpinIndicator` 的动画对象生命周期：

- 隐藏的 `SpinIndicator` attach 到 visual tree 时不再提前构造 `Animation` / `KeyFrame` / `Setter`；
- 首次变为可见时才构造并启动动画；
- 隐藏时停止动画并释放 `CancellationTokenSource`；
- 已 materialized 的动画在 `MotionDuration` / `MotionEasingCurve` 变化时会重建并在运行态继续运行。

耗时指标在当前机器负载下有明显噪声，不能作为主收益；更客观的收益是隐藏态动画对象不再 materialize，非 spinning 场景分配下降约 `1.8%`，Gallery shape 分配下降约 `1.9%`。

---

## 1. 根因

`AbstractSpinIndicator.OnAttachedToVisualTree()` 原来无条件调用：

```csharp
BuildIndicatorAnimation();
```

这导致两类成本和一类正确性问题：

- 非 spinning 的 `Spin` 中，模板内 `SpinIndicator` 是静态存在但 `IsVisible=false`，仍会创建旋转动画对象；
- 独立 `SpinIndicator { IsVisible=false }` attach 时也会提前创建动画；
- 动画创建后，`MotionDuration` / `MotionEasingCurve` 变化不会刷新 `_animation`，运行中的动画可能继续使用旧参数。

`SpinTheme.axaml` 的 `MaskLayout` 是静态模板节点，本轮保持不变。Avalonia 的隐藏控件在 layout/render 路径上会短路（`.referenceprojects/Avalonia/src/Avalonia.Base/Layout/Layoutable.cs:546`, `:671`; `.referenceprojects/Avalonia/src/Avalonia.Base/Rendering/ImmediateRenderer.cs:34`），按性能守则不应为节省一个轻量隐藏模板节点引入 C# 创建/销毁状态机。

---

## 2. 改动

### 2.1 生产代码

`src/AtomUI.Controls/Spin/AbstractSpinIndicator.cs`

- `OnAttachedToVisualTree()` 删除无条件 `BuildIndicatorAnimation()`；
- `StartIndicatorAnimation()` 内部先 lazy build，再启动；
- `MotionDurationProperty` / `MotionEasingCurveProperty` 变化时，如果动画已经 materialized，则重建；
- 重建前若动画正在运行，重建后继续启动。

### 2.2 状态验证

`--verify-spin-states` 口径同步为静态模板语义：

- 非 spinning：`MaskLayout` / `SpinIndicator` 存在但隐藏；
- 隐藏 `SpinIndicator` 不构造动画；
- spinning：`SpinIndicator` 显示时 lazy build 并启动动画；
- 停止 spinning：动画 token 释放；
- 再次 spinning：动画重启；
- `MotionDuration` 变化：已 materialized 动画重建并保持运行态。

---

## 3. 控件级结果

命令：

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite spin --count 80 \
  --markdown /tmp/spin-after.md
```

After 复测跑了两轮，当前机器 `load averages: 9.32 8.45 7.88`，下表使用两轮 after 均值。

| Scenario | ms/item baseline | ms/item after avg | ms 变化 | KB/item baseline | KB/item after avg | KB 变化 | 结论 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| `SpinIndicator.BuiltIn.Middle` | 0.119 | 0.112 | +6.30% | 32.8 | 33.3 | -1.68% | timing 噪声内 |
| `SpinIndicator.CustomIcon.Middle` | 0.286 | 0.294 | -2.80% | 75.5 | 75.5 | -0.07% | 基本持平 |
| `Spin.Content.NotSpinning` | 0.417 | 0.404 | +3.12% | 78.8 | 77.4 | +1.78% | 主收益场景 |
| `Spin.Content.Spinning.Tip` | 0.604 | 0.596 | +1.32% | 99.3 | 99.3 | 0.00% | 基本持平 |
| `Spin.Content.Spinning.BlurMask` | 0.569 | 0.613 | -7.64% | 99.1 | 99.1 | 0.00% | timing 噪声/负载影响 |
| `Spin.GalleryShape` | 3.655 | 3.558 | +2.65% | 575.1 | 564.2 | +1.89% | 分配小幅下降 |

说明：

- Visual/logical 计数不变，符合本轮不改模板树的预期。
- spinning 场景本来就需要动画，分配不会下降。
- 真正的持续渲染热点仍是 `Render()` 每帧 `4 × DrawEllipse`，需要 Gallery 持续 spinning 场景专项验证，不能用本轮 headless materialization bench 证明。

---

## 4. 正确性验证

命令：

```bash
dotnet build -c Release -f net10.0 tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-spin-states
```

结果：

| 验证 | 结果 |
| --- | --- |
| Release build | passed, 0 warnings, 0 errors |
| `--verify-spin-states` | passed |
| hidden indicator 不构造动画 | passed |
| visible indicator lazy build / start | passed |
| hide 后 token 释放 | passed |
| show again 后动画重启 | passed |
| motion 参数变化后动画重建 | passed |

---

## 5. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 生产文件改动 | `AbstractSpinIndicator.cs` |
| 新增 `Ensure*/Clear*` 动态模板逻辑 | 0 |
| 新增 `_ignoreXxx` 标志位 | 0 |
| 新增 disposable / event / timer | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 正确性修复 | `MotionDuration` / `MotionEasingCurve` 变化后重建已 materialized 动画 |

本轮属于低风险生命周期优化：静态视觉树不变，可见态动画行为保留，隐藏态不再提前承担动画对象成本。
