# Menu Performance Notes

## Scope

本轮补充 `MenuItem` tree-node children 枚举路径。

## Root Cause

`ITreeNode<IMenuItemData>.Children` 原先直接返回 `Items.OfType<IMenuItemData>()`。每次外部树遍历取 children 时都会构造 LINQ 类型过滤 operator。

新实现改为显式 `foreach` + `is IMenuItemData`，保留 `OfType` 的类型过滤语义，同时去掉 LINQ operator。

## Benefit

这是 Menu 树遍历路径的结构性收益，不声明页面加载 timing 提升。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| MenuItem child filter LINQ operators / children enumeration | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；显式类型过滤替代 `OfType` |
| Non-menu child filtering semantics | preserved | preserved | n/a | 0.00% | 仍跳过非 `IMenuItemData` 子项 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Verification

- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `--verify-menu-states` currently fails on both this change and clean HEAD `d35b21a1a` with the same template lazy-content / cleanup expectations, so it is not introduced by this optimization.
