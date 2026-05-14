# Icon Phase 4 结果

Phase 4 聚焦高频模板里的隐藏 icon slot。原则是：不使用的功能不承担 Control / template / binding / style 成本；按需创建的控件在状态切换时必须从 visual tree 移除，并解除事件订阅或 icon presenter 子节点引用。

## 改动范围

- `SelectHandle`
  - 模板只保留 `PART_IndicatorHost`。
  - open、loading、search、clear 按当前可见状态创建。
  - clear button 只在 allow-clear、非空、hover/pressed 时创建；移除时取消 click 订阅。
  - open/loading 通过 `IconPresenter.Icon = null` 解除外部 `PathIcon` 的 visual/logical parent。
- `MenuItem`
  - 叶子项不再创建 `RightOutlined`。
  - 有子项时在 `PART_MenuIndicatorIconHost` 中创建 submenu indicator。
- `ToggleIconButton`
  - 由 checked / unchecked 两个 `IconPresenter` 改为单 `PART_CurrentIconPresenter`。
  - 通过 theme 内部 converter 选择当前 icon，不新增 public 控件 API。
- `NavMenuItemHeader`
  - inline / vertical / horizontal header 改为 indicator host。
  - `BaseNavMenuItemHeader` 仅在 `HasSubMenu` 且不是 horizontal top-level 时创建 `RightOutlined`。
- `Button`
  - 默认模板不再常驻 `LoadingOutlined`。
  - `IsLoading=True` 时在 `PART_LoadingIconHost` 中创建 `PART_LoadingIcon`，退出 loading 后移除。

本阶段不缓存任何 `Icon`、`PathIcon`、`IconPresenter` 或 button 实例；按需控件只作为当前 owner 的临时 visual child。

## 控件级结果

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite icon --count 300 \
  --markdown /tmp/icon-micro-phase4-final-with-button.md
```

关键结构结果：

| Scenario | Phase 3 reference | Phase 4 current | 结构变化 |
| --- | ---: | ---: | --- |
| `Icon.HiddenSlots.SelectDefault` KB/item | 524.6 | 427.6 | `Icon/root 3 -> 1`，`IconPresenter/root 2 -> 1`，`Button/root 1 -> 0`，`Visual/root 27 -> 23` |
| `Icon.HiddenSlots.MenuItemLeaf` KB/item | 222.6 | 205.7 | `Icon/root 1 -> 0`，叶子项不再创建 submenu arrow |
| `Icon.HiddenSlots.ToggleIconButton` | inspection: 2 presenters | current: 1 presenter | checked/unchecked 不再同时 materialize |
| `Icon.HiddenSlots.NavMenuInlineLeaf` | inspection: hidden arrow | current: `Icon/root 0` | 叶子 header 不再创建 submenu arrow |
| `Icon.HiddenSlots.ButtonDefault` | inspection: hidden loading icon | current: `Icon/root 0` | 非 loading Button 不再创建 `LoadingOutlined` |

当前 micro 数据：

| Scenario | ms/item | KB/item | Visual/root | Icon/root | IconPresenter/root | Button/root |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| `Icon.HiddenSlots.SelectDefault` | 1.983 | 427.6 | 23.0 | 1.0 | 1.0 | 0.0 |
| `Icon.HiddenSlots.MenuItemLeaf` | 0.908 | 206.0 | 13.0 | 0.0 | 1.0 | 2.0 |
| `Icon.HiddenSlots.MenuItemSubmenu` | 0.968 | 256.2 | 15.0 | 1.0 | 1.0 | 2.0 |
| `Icon.HiddenSlots.ToggleIconButton` | 0.210 | 95.5 | 6.0 | 1.0 | 1.0 | 1.0 |
| `Icon.HiddenSlots.NavMenuInlineLeaf` | 0.383 | 125.3 | 8.0 | 0.0 | 1.0 | 0.0 |
| `Icon.HiddenSlots.NavMenuInlineSubmenu` | 0.560 | 166.9 | 10.0 | 1.0 | 1.0 | 0.0 |
| `Icon.HiddenSlots.ButtonDefault` | 0.904 | 297.4 | 10.0 | 0.0 | 1.0 | 1.0 |
| `Icon.HiddenSlots.ButtonLoading` | 1.113 | 421.1 | 12.0 | 1.0 | 1.0 | 1.0 |

说明：

- Phase 4 的主要收益是结构性减重和 allocation 下降，单次 timing 仍受 headless layout / JIT / GC 噪声影响。
- `SelectDefault` 是本阶段收益最大的 micro 场景，默认路径不再承担 loading/search/clear 的隐藏控件成本。
- `MenuItem` / `NavMenu` 的 leaf 场景验证了“没有子项就没有箭头控件”。

## Gallery 回归

LineEditShowCase 复现命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase lineedit --warmup 2 --iterations 10 \
  --label phase4-icon-slots-final \
  --markdown /tmp/gallery-lineedit-phase4-icon-slots-final.md
```

结果：

| Set | Phase 3 reference | Phase 4 current |
| --- | ---: | ---: |
| Repeated mean | 166.11ms | 217.09ms |
| Alloc KB mean | 54810.29 | 52093.42 |
| Visuals | 2358 | 2357 |
| Icon | 80 | 51 |
| IconPresenter | 34 | 32 |

判断：

- LineEditShowCase 的 icon / presenter 数减少，allocation 下降约 `5.0%`。
- 本轮 10 次 timing 慢于 Phase 3 参考样本，但结构没有回退；LineEditShowCase timing 仍需多轮对照，不能把该样本解读为真实变慢。

IconShowCase 复现命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase icon --warmup 2 --iterations 10 \
  --label phase4-icon-slots-final \
  --markdown /tmp/gallery-icon-phase4-icon-slots-final.md
```

结果：

| Set | Phase 3 reference | Phase 4 current |
| --- | ---: | ---: |
| Repeated mean | 149.08ms | 161.33ms |
| Alloc KB mean | 46322.72 | 46325.11 |
| Visuals | 3712 | 3712 |
| Icon | 456 | 456 |
| IconPresenter | 459 | 459 |

判断：

- Phase 4 不改变 IconShowCase 的运行时结构。
- allocation 基本一致；timing 差异按样本噪声处理。

### 受影响控件 Gallery 补充

Phase 4 还扩展了 `tools/performances/AtomUI.GalleryPerformance`，支持直接复现 `ButtonShowCase`、`SelectShowCase`、`MenuShowCase`：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase button --warmup 2 --iterations 10 \
  --label phase4-icon-slots-final \
  --markdown /tmp/gallery-button-phase4-icon-slots-final.md

dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase select --warmup 2 --iterations 10 \
  --label phase4-icon-slots-final \
  --markdown /tmp/gallery-select-phase4-icon-slots-final.md

dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase menu --warmup 2 --iterations 10 \
  --label phase4-icon-slots-final \
  --markdown /tmp/gallery-menu-phase4-icon-slots-final.md
```

当前补充结果：

| Showcase | Source shape | Repeated mean | Alloc KB mean | Runtime structure |
| --- | --- | ---: | ---: | --- |
| `ButtonShowCase` | `Button=89`, `AntDesignIconProvider=49` | 114.55ms | 32178.40 | `Icon=52`, `IconPresenter=92`, `Button=89` |
| `SelectShowCase` | `Select=33`, `AntDesignIconProvider=4` | 174.66ms | 28062.18 | `Icon=46`, `IconPresenter=59`, `Select=33`, `AddOnDecoratedBox=33` |
| `MenuShowCase` | `Menu=5`, `MenuItem=82`, `AntDesignIconProvider=33` | 104.01ms | 16709.97 | `Icon=34`, `IconPresenter=36`, `Menu=5`, `MenuItem=10`, `NavMenuHeader=36` |

判断：

- `ButtonShowCase` 中 89 个 Button 没有带来 89 个默认 loading icon，运行时 `Icon=52` 与显式 icon 数量接近，符合 Button loading slot 按需创建预期。
- `SelectShowCase` 中 33 个 Select 的默认路径没有为 loading/search/clear 同时 materialize 隐藏控件；该结论与 `SelectDefault` micro 结构下降一致。
- `MenuShowCase` 的 XAML 声明了大量 `MenuItem`，但稳定 route 中只 materialize 当前可见层级；leaf submenu arrow 不再常驻，剩余 `NavMenuHeader` 来自 showcase 内部 NavMenu 示例。
- 这三组补充样本用于确认真实 XAML 场景的结构形态；当前 headless 运行的触发列为 `NavigateToCommand`，不作为左侧菜单真实点击耗时对比；由于缺少同一工具口径下的 Phase 3 前置样本，也不单独计算 timing 提升百分比。

## 验证

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-build -- --verify-accessories --verify-effective-brushes --verify-addon-states --verify-antdesign-metadata --verify-icon-hidden-slots
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug --no-restore
```

说明：

- `--verify-icon-hidden-slots` 覆盖 `MenuItem`、`ToggleIconButton`、`SelectHandle`、`NavMenuItemHeader`、`Button` 的按需创建和移除。
- 验证包含 clear click 事件、运行时状态切换、submenu indicator 创建/移除、移除后 visual parent 检查。
- `AtomUI.GalleryPerformance` 构建仍保留既有 warning：`DataGridColumn._clipboardContentBinding is never used`。

## 后续

- `IconButton` 自身没有常驻 hidden loading slot，本阶段未改；后续如发现具体使用场景有双 slot 再单独处理。
- Provider / generator 深化优化进入 Phase 5；只有 baseline 证明 provider 是瓶颈时再做 enum -> factory switch。
