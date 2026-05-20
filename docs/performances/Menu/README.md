# Menu 性能优化记录

## 范围

- 控件：`Menu`、`MenuItem`、`ContextMenu`。
- 工具：`tools/performances/AtomUI.Performance --suite menu`，`tools/performances/AtomUI.GalleryPerformance --showcase menu`。
- Gallery：真实 `MenuShowCase.axaml`，源码形态为 `Menu=5`、`MenuItem=82`、`ShowCaseItem=15`。页面里同时包含大量 `NavMenu` 示例，所以 Gallery 页面耗时不是纯 `MenuItem` 成本。

## 主要瓶颈

- 默认闭合的普通 `MenuItem` 仍然创建 toggle `CheckBox`、`RadioButton`、`IconPresenter`、`InputGestureText` 等可选子控件。
- 有子菜单的 `MenuItem` 在闭合状态也提前创建 `PopupFrame -> ScrollViewer -> ItemsPresenter` 这棵重内容树。
- `ContextMenu` 打开时订阅 `Window.Deactivated`，关闭路径没有统一清理，存在重复打开后的订阅生命周期风险。
- `MenuShowCase` 的真实页面成本里混入 `NavMenu` 默认打开路径和 Workspace 导航状态，cold first navigation 需要看 visual shape 是否一致，不能只看单个冷启动数字。

## 实施方案

| Phase | 状态 | 内容 |
| --- | --- | --- |
| Phase 0: Baseline 与观测 | Done | 新增 `Menu` 控件级 suite 和真实 `MenuShowCase` 口径。 |
| Phase 1: 生命周期优先修复 | Done | `ContextMenu` 关闭、重复打开和已关闭 `Close()` 路径统一清理 `Window.Deactivated` 订阅。 |
| Phase 2: 闭合 leaf 减重 | Done | toggle、icon、input gesture 从模板常驻改为按需创建，使用 `[!]` 同生命周期绑定。 |
| Phase 3: 子菜单 Popup 重内容按需创建 | Done | 保留轻量 `Popup` shell，首次打开前才创建 `Border/ScrollViewer/ItemsPresenter`，关闭后复用，re-template 时释放。 |
| Phase 4: Popup 打开顺序与状态同步 | Done | 移除模板 `IsOpen` 双向绑定，由 `MenuItem` 先 materialize 内容再打开 `Popup`；`Popup.Closed` 回写 `IsSubMenuOpen=false` 保留原行为。 |
| Phase 5: 状态验证 | Done | 增加 `--verify-menu-states`，覆盖默认闭合轻量树、toggle/icon/gesture 生命周期、submenu 内容复用、ContextMenu 订阅释放。 |
| Phase 6: Gallery 实测 | Done | 使用真实 `MenuShowCase` 跑 repeated 60 次；cold 数据单独标注 shape 噪声，不用单次 cold 宣称收益。 |
| Phase 7: 清理 | Done | 未保留阶段中间 markdown；构建通过，新增代码未引入无效 using。 |

## 控件级结果

命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug -f net10.0 --no-build -- --suite menu --count 80
```

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `MenuItem.Leaf.NoIcon.NoGesture.Closed` | `1.177 ms/item`, `209.8 KB/item`, `13 visuals` | `0.693 ms/item`, `122.1 KB/item`, `9 visuals` | `41.1%` faster, `41.8%` less KB, `4` fewer visuals |
| `MenuItem.Leaf.Icon.Closed` | `1.441 ms/item`, `242.8 KB/item`, `15 visuals` | `1.018 ms/item`, `167.4 KB/item`, `12 visuals` | `29.4%` faster, `31.1%` less KB, `3` fewer visuals |
| `MenuItem.Leaf.InputGesture.Closed` | `1.268 ms/item`, `211.6 KB/item`, `13 visuals` | `0.866 ms/item`, `134.0 KB/item`, `10 visuals` | `31.7%` faster, `36.7%` less KB, `3` fewer visuals |
| `MenuItem.Toggle.CheckBox.Closed` | `1.755 ms/item`, `265.1 KB/item`, `18 visuals` | `1.171 ms/item`, `192.5 KB/item`, `15 visuals` | `33.3%` faster, `27.4%` less KB, `3` fewer visuals |
| `MenuItem.Toggle.Radio.Closed` | `1.837 ms/item`, `283.0 KB/item`, `19 visuals` | `1.608 ms/item`, `209.8 KB/item`, `16 visuals` | `12.5%` faster, `25.9%` less KB, `3` fewer visuals |
| `MenuItem.SubMenu.Closed` | `1.757 ms/item`, `286.6 KB/item`, `15 visuals` | `1.279 ms/item`, `199.9 KB/item`, `11 visuals` | `27.2%` faster, `30.3%` less KB, `4` fewer visuals |
| `Menu.TopLevel.Basic.Closed` | `4.269 ms/item`, `663.0 KB/item`, `22 visuals` | `3.203 ms/item`, `571.8 KB/item`, `22 visuals` | `25.0%` faster, `13.8%` less KB |
| `Menu.GalleryMenuOnlyShape` | `10.923 ms/item`, `2336.3 KB/item`, `63 visuals` | `9.181 ms/item`, `2135.4 KB/item`, `63 visuals` | `15.9%` faster, `8.6%` less KB |

## MenuShowCase 结果

可比 repeated 口径：`cold-iterations=5`、`warmup=10`、`iterations=60`、`1300x900 headless`，baseline 来自临时 clean worktree `94cafd5e4`。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `MenuShowCase` repeated mean | `91.16 ms` | `89.10 ms` | `2.26%` faster |
| `MenuShowCase` repeated median | `89.21 ms` | `87.94 ms` | `1.42%` faster |
| `MenuShowCase` repeated P95 | `101.65 ms` | `97.78 ms` | `3.81%` faster |
| `MenuShowCase` repeated alloc | `16565.64 KB` | `16303.66 KB` | `1.58%` less KB |

Cold first navigation 在 Menu-only 阶段不声明提升：当时工具会把 `visuals 791` 的未完整展开形态提前判定为 ready。NavMenu 本轮已把 `MenuShowCase` ready 条件固定到完整展开形态 `visuals 868 / NavMenuHeader 42 / MotionActor 30`，严格 Gallery 结果见 [NavMenu](../NavMenu/README.md)。

## 验证

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 --no-build -- --verify-menu-states
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug -f net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug -f net10.0 --no-build -- --showcase menu --cold-iterations 5 --warmup 10 --iterations 60 --timeout-ms 30000
```

`GalleryPerformance` 构建仍有既有 warning：`DataGridColumn._clipboardContentBinding` 未使用，和本轮 Menu 改动无关。
