# Expander 性能优化

> 状态：本轮追加 motion structural cleanup；不声明页面级 timing 收益。

---

## Motion Easing Cache

`Expander` 展开/折叠内容时会创建一次 `ExpandMotion` / `CollapseMotion`，原路径同时重复创建默认 `CubicEaseOut` / `CubicEaseIn`。Avalonia `CubicEaseOut` 与 `CubicEaseIn` 均只有 `Ease(double)` override，没有实例字段（`.referenceprojects/Avalonia/src/Avalonia.Base/Animation/Easings/CubicEaseOut.cs:7-13`，`.referenceprojects/Avalonia/src/Avalonia.Base/Animation/Easings/CubicEaseIn.cs:7-13`），因此默认 easing 可以复用。

本轮仅复用 easing；motion 类型、展开方向、duration、可见性切换和 `_animating` 防重入流程保持不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Expander default easing allocations / expand | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；展开不再新建默认 easing |
| Expander default easing allocations / collapse | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；折叠不再新建默认 easing |
| Expander motion objects / expand-or-collapse | 1 | 1 | `(1 - 1) / 1` | 0.00% | 行为保持；motion 仍按当前方向即时创建 |

说明：这是交互 motion 路径 structural-only 收益；没有新增 Gallery timing 对比，不声明页面加载速度提升。

---

## Verification

- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed.
