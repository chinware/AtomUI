# NavMenu 性能优化记录

## 范围

- 控件：`NavMenu`、`NavMenuItem`、`DefaultNavMenuInteractionHandler`。
- 工具：`tools/performances/AtomUI.Performance --suite navmenu`，`tools/performances/AtomUI.GalleryPerformance --showcase menu`。
- Gallery：真实 `MenuShowCase.axaml`，因为该页面同时承载 `Menu` 和大量 `NavMenu` 示例，本轮 NavMenu 的 Gallery 指标使用 `MenuShowCase` 的严格完整显示口径。

## 主要发现

- `PrepareContainerForItemOverride()` 中对 `INavMenuNode.Icon`、`IsEnabled`、`HeaderTemplate` 的 `BindUtils.RelayBind(...)` 返回值没有保存。容器 recycle/remove 后旧 node 仍可能继续推送到旧 container，这是资源泄露和错误状态风险，优先修复。
- `DefaultNavMenuInteractionHandler.AttachCore()` 在闭合状态就订阅 root pointer、`Window.Deactivated`、platform lost focus、`InputManager.Process`。绝大多数未打开的 vertical/horizontal NavMenu 不需要承担这些全局关闭成本。
- 默认路径遍历先取 container 再比较 key，遇到未命中的子项 container 尚未生成时会提前返回；这让 `DefaultOpenPaths` / `DefaultSelectedPath` 在真实 `MenuShowCase` 里存在延迟和失败风险。
- 曾评估把 popup/inline submenu 模板重内容移到代码中按需创建。控件级 visual 下降明显，但真实 Gallery repeated/cold 出现回退，且逻辑复杂度上升，所以已回撤，不作为本轮交付。

## 实施方案

| Phase | 状态 | 内容 |
| --- | --- | --- |
| Phase 0: Baseline 与观测 | Done | 新增 `NavMenu` 控件级 suite、真实 `MenuShowCase` 导航测量和 trace；发现旧 ready 条件会把 791 visuals 的未完整展开形态误判为完成。 |
| Phase 1: 容器 binding 生命周期修复 | Done | 保存 prepared node binding 的 `IDisposable`，在 `ClearContainerForItemOverride()` / recycle 前释放；同生命周期的 parent `ItemTemplate` 使用 `[!]`。 |
| Phase 2: 全局关闭订阅按需化 | Done | closed vertical/horizontal NavMenu 不再订阅全局 input/window/platform；首次 submenu open 时订阅，全部关闭或 detach 时释放。 |
| Phase 3: 默认路径遍历收敛 | Done | 先按 node/key 过滤目标，再取目标 container；避免未命中 sibling 的 container lookup 影响正确性。 |
| Phase 4: 固定等待移除 | Done | 默认路径子容器等待从固定 `Task.Delay(50ms)` 改为 dispatcher 驱动的 `current.UpdateLayout`，减少无意义等待。 |
| Phase 5: 模板减重评估 | Done | popup/inline 重模板懒创建方案因真实 Gallery 回退和复杂度增加已回撤；保留原模板行为、动画和 visual 结构。 |
| Phase 6: 状态/生命周期验证 | Done | 新增 `--verify-navmenu-states`，覆盖 icon 可见性、popup/inline 模板状态、默认路径应用、全局订阅按需释放、container binding 释放。 |
| Phase 7: Gallery 严格口径 | Done | `MenuShowCase` ready 条件必须等到完整展开形态：`visuals=868`、`logical=170`、`NavMenuHeader=42`、`MotionActor=30`。 |

## 控件级结果

命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug -f net10.0 --no-build -- --suite navmenu --count 80 \
  --markdown /tmp/navmenu-control-current.md
```

说明：本轮没有保留模板减重方案，所以 closed visual tree 基本不变；控件级主要收益是生命周期和默认路径等待收敛。单次 headless suite 的小 leaf timing 波动较明显，验收以真实 Gallery 多样本为主。

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `NavMenu.Inline.Submenu.DefaultOpenPath` | `10.241 ms/item`, `2619.3 KB/item` | `9.943 ms/item`, `2603.2 KB/item` | `2.91%` faster, `0.62%` less KB |
| `NavMenu.Horizontal.TopLevel.Closed` | `3.627 ms/item`, `1239.2 KB/item` | `2.988 ms/item`, `1226.6 KB/item` | `17.62%` faster, `1.02%` less KB |
| `NavMenu.WorkspaceNavigationShape` | `10.308 ms/item`, `3360.0 KB/item` | `9.903 ms/item`, `3346.5 KB/item` | `3.93%` faster, `0.40%` less KB |
| `NavMenu.MenuShowCaseShape` | `41.182 ms/item`, `12271.9 KB/item` | `40.981 ms/item`, `12211.5 KB/item` | `0.49%` faster, `0.49%` less KB |

## MenuShowCase 结果

严格可比口径：`cold-iterations=10`、`warmup=10`、`iterations=60`、`1300x900 headless`。ready 条件要求真实 `MenuShowCase` 完整展开到 `868 visuals / 170 logical / 42 NavMenuHeader / 30 MotionActor`。

旧工具的 `MenuShowCase` ready 条件只要求 `MenuItemCount > 0`，会在 `791 visuals / 36 NavMenuHeader / 24 MotionActor` 时提前结束。这个形态下默认展开路径还没完全显示，不能作为“showcase 完全显示”的耗时。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `MenuShowCase` cold mean | `416.22 ms` | `351.72 ms` | `15.50%` faster |
| `MenuShowCase` cold median | `383.44 ms` | `354.83 ms` | `7.46%` faster |
| `MenuShowCase` cold P95 | `583.69 ms` | `391.83 ms` | `32.87%` faster |
| `MenuShowCase` cold alloc | `19719.10 KB` | `19217.36 KB` | `2.54%` less KB |
| `MenuShowCase` repeated mean | `162.76 ms` | `99.13 ms` | `39.09%` faster |
| `MenuShowCase` repeated median | `158.12 ms` | `98.39 ms` | `37.78%` faster |
| `MenuShowCase` repeated P95 | `187.18 ms` | `106.22 ms` | `43.25%` faster |
| `MenuShowCase` repeated alloc | `18260.20 KB` | `17727.83 KB` | `2.92%` less KB |

## 取舍

- 保留了 XAML 模板结构，没有为了 visual 数下降牺牲模板可读性、动画行为或真实 Gallery 性能。
- `INavMenuNode` 到 `NavMenuItem` 的 binding 生命周期不一致，不能使用 `[!]` 直接替代；本轮显式保存并释放 binding，验证了容器移除后旧 node 更新不会再影响旧 container。
- `DefaultOpenPaths` / `DefaultSelectedPath` 的路径遍历修复同时是正确性修复。严格 Gallery 口径把这个完整展开状态纳入耗时，避免把未完全显示的页面算作完成。

## 验证

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 --no-build -- --verify-navmenu-states
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug -f net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug -f net10.0 --no-build -- --showcase menu --cold-iterations 10 --warmup 10 --iterations 60 --timeout-ms 30000
```

`GalleryPerformance` 构建仍有既有 warning：`DataGridColumn._clipboardContentBinding` 未使用，和本轮 NavMenu 改动无关。
