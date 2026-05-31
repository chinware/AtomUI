# FloatButton Performance Notes

## Scope

本轮补充 FloatButton 动态 children 同步路径：

- `FloatButtonGroup.Children -> FloatButtonItemsControl.Children`
- `FloatButtonItemsControl.Children -> StackPanel.Children`
- `FloatButtonGroupHost.Children -> FloatButtonGroup.Children`

## Root Cause

- FloatButton group add/remove 原先在多个 `NotifyCollectionChanged` 分支里用 `OfType<Control>()` 做类型过滤。
- `FloatButtonGroup` add 分支还会先遍历 `NewItems` 配置 embed mode，再让 `OfType<Control>()` 二次扫描 `NewItems` 插入子集合。
- remove 分支使用 `RemoveAll` 会进入 Avalonia `HashSet<T>` + 目标集合扫描路径；镜像集合已有 `OldStartingIndex` / `OldItems.Count`，`RemoveRange` 更直接。参考 `.referenceprojects/Avalonia/src/Avalonia.Base/Collections/AvaloniaList.cs:530-550`。

## Changes

- FloatButton add 路径改为一次 indexed collect 后复用同一 `List<Control>` 做 embed-mode 配置和 `InsertRange`。
- `FloatButtonGroup` / `FloatButtonItemsControl` / `FloatButtonGroupHost` remove 路径改为 `RemoveRange(OldStartingIndex, OldItems.Count)`。
- 保留 trigger/menu、badge、overlay host、motion actor 行为不变。

## Benefit

这轮是结构性收益：FloatButton 动态 children add/remove 少一次源集合类型过滤，remove 不再走 HashSet + 全集合扫描。不声明本轮页面加载耗时提升。

| Metric | Baseline | Optimized | Formula | Improvement | Conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| FloatButton collection sync `OfType<Control>` callsites | 6 | 0 | `(6 - 0) / 6` | 100.00% | 有效；group、items control、group host add/remove 均去掉 LINQ 类型过滤 |
| `FloatButtonGroup` add source scans before insert | 2 scans | 1 scan | `(2 - 1) / 2` | 50.00% | 有效；一次 collect 结果同时用于 embed-mode 配置和插入 |
| Mirrored children remove target scans | 3 scans | 0 scans | `(3 - 0) / 3` | 100.00% | 有效；group、items control、group host 均按 `OldStartingIndex` 删除 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Group Menu Motion Easing Cache

`FloatButtonGroup` 菜单展开/收起时仍需要按 placement 和当前尺寸创建 motion，但默认 `CubicEaseOut` / `CubicEaseIn` 不需要每次交互重复创建。Avalonia 两个 easing 类只有 `Ease(double)` override，没有实例字段（`.referenceprojects/Avalonia/src/Avalonia.Base/Animation/Easings/CubicEaseOut.cs:7-13`，`.referenceprojects/Avalonia/src/Avalonia.Base/Animation/Easings/CubicEaseIn.cs:7-13`）。

| Metric | Baseline | Optimized | Formula | Improvement | Conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| FloatButtonGroup default easing allocations / menu show | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；show motion 复用默认 easing |
| FloatButtonGroup default easing allocations / menu hide | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；hide motion 复用默认 easing |
| FloatButtonGroup motion objects / menu show-or-hide | 1 | 1 | `(1 - 1) / 1` | 0.00% | 行为保持；motion 仍按 placement 即时创建 |

说明：这是交互 motion 路径 structural-only 收益；没有新增 Gallery timing 对比，不声明页面加载速度提升。

## Verification

- `dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj` passed.
- `--verify-floatbutton-states` 当前仍受旧 overlay/badge/lazy menu 断言影响，不能作为本轮集合同步路径的通过证明。
