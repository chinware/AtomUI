# Drawer 性能优化

> 状态：本轮追加 motion structural cleanup；不声明页面级 timing 收益。

---

## Motion Easing Cache

`DrawerContainer.BuildMotionByPlacement()` 每次打开/关闭都会按 placement 创建一个 motion，同时重复创建默认 `CubicEaseOut`。Avalonia `CubicEaseOut` 只有 `Ease(double)` override，没有实例字段（`.referenceprojects/Avalonia/src/Avalonia.Base/Animation/Easings/CubicEaseOut.cs:7-13`），因此默认 easing 可以作为类级共享实例复用。

本轮仅复用 easing；motion 类型、placement 方向、duration、mask/close 状态机均不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Drawer default easing allocations / open-or-close | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；每次打开/关闭不再新建默认 easing |
| Drawer motion objects / open-or-close | 1 | 1 | `(1 - 1) / 1` | 0.00% | 行为保持；motion 仍按方向和尺寸即时创建 |

说明：这是交互 motion 路径 structural-only 收益；没有新增 Gallery timing 对比，不声明页面加载速度提升。

---

## Verification

- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed.
