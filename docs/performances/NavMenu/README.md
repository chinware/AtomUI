# NavMenu Performance Notes

## Scope

本轮补充 NavMenu 手动遍历路径：

- accordion mode 关闭兄弟子菜单。
- submenu open 后刷新 direct child `NavMenuItem.TryUpdateCanExecute()`。

## Root Cause

- 旧路径在热交互分支里使用 `LogicalChildren.OfType<NavMenuItem>()`、`GetLogicalChildren().OfType<INavMenuItem>()` 和 `ItemsView.OfType<NavMenuItem>()`。
- 这些集合都有直接 Count/indexer 或控件自有 `LogicalChildren` 可用；手动遍历能避免 LINQ `OfType` iterator，同时保持原来的类型过滤语义。

## Benefit

这是交互路径结构性收益：NavMenu 打开/关闭子菜单时少创建 LINQ 类型过滤 iterator；不声明页面加载耗时提升。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| NavMenu accordion sibling close LINQ operators / state change | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；改为 `LogicalChildren.Count/indexer` |
| NavMenu submenu opened sibling close LINQ operators / open | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；改为 `LogicalChildren.Count/indexer` |
| NavMenuItem child execute refresh LINQ operators / submenu toggle | 2 | 0 | `(2 - 0) / 2` | 100.00% | 有效；改为 `ItemsView.Count/indexer` |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Additional Structural Pass

内部关闭/互斥子菜单路径原先通过 `INavMenuElement.SubItems` 属性取子项；第一步已把内部热路径改为直接遍历 `LogicalChildren.Count/indexer` 并做同样的 `is INavMenuItem` 类型过滤。本轮继续把公共接口实现本身从 `LogicalChildren.OfType<INavMenuItem>()` 改为显式类型过滤枚举，公共 API 形态不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Internal `SubItems` enumerable calls / submenu close-open paths | 7 | 0 | `(7 - 0) / 7` | 100.00% | 有效；内部路径不再触发 `OfType` iterator |
| NavMenu root public `SubItems` LINQ operators / enumeration | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；显式类型过滤替代 `OfType` |
| NavMenuItem public `SubItems` LINQ operators / enumeration | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；显式类型过滤替代 `OfType` |
| Default path root-node LINQ calls / default path apply | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；Count 保护后直接读 `pathNodes[0]` |
| Public `INavMenuElement.SubItems` compatibility | preserved | preserved | n/a | 0.00% | 公共接口保持不变 |
| Selected path item list growth / item select | dynamic growth | exact parent depth | structural | 结构收益 | logical parent 链深度已知时预分配 |
| Default path node list growth / default path apply | dynamic growth | exact node depth | structural | 结构收益 | nav node 父链深度已知时预分配 |

说明：覆盖 top-level close、pointer-enter sibling close、submenu-open sibling close、recursive close / clear 和 item close-submenus。`--verify-navmenu-states` 当前存在历史失败，本轮不声明页面 timing 或完整状态 suite 收益。

## Select Path Set Cleanup

`DefaultNavMenuInteractionHandler` / `InlineNavMenuInteractionHandler` 选中 leaf 时，旧路径用 `ToHashSet()` 构造新旧 selected path set，再用 `Except()` 得到要清理的旧路径。本轮改为复用 `NavMenu.BuildSelectPathSet()`，按路径列表 `Count` 预分配 HashSet，并在遍历旧 set 时直接 `Contains()` 判断是否需要清理。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| NavMenu selected path `ToHashSet()` LINQ calls / leaf select | 2 | 0 | `(2 - 0) / 2` | 100.00% | 结构收益；新旧路径 set 按列表容量手写构造 |
| NavMenu selected path `Except()` LINQ operators / leaf select | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；旧路径遍历时直接 `Contains` 判断 |
| InlineNavMenu selected path `ToHashSet()` LINQ calls / leaf select | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；new path set 按列表容量手写构造 |
| InlineNavMenu selected path `Except()` LINQ operators / leaf select | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；旧路径遍历时直接 `Contains` 判断 |
| DefaultNavMenu first leaf select empty old-path HashSet allocations / first select | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；无旧选中项时跳过旧路径集合 |
| InlineNavMenu first leaf select empty old-path HashSet allocations / first select | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；无旧选中项时跳过旧路径集合 |
| selected-path semantics | unchanged | unchanged | n/a | 0.00% | 行为保持 |

说明：这是 NavMenu 选中交互路径 structural-only 收益；没有新增页面导航 timing 对比，不声明页面加载速度提升。

## Verification

- `dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj` passed.
- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `--verify-navmenu-states` currently fails on both this change and clean HEAD `d35b21a1a` with the same default-path / lazy-subscription / binding-release expectations, so it is not introduced by this optimization.
