# Badge 性能优化

`Badge` 是高频 Data Display 控件，本轮目标是让隐藏态、无文本态和未使用的装饰层不提前承担成本，同时保证 `AdornerLayer`、运行时模式切换和 motion 生命周期不引入 visual parent 冲突或资源泄露。

## 问题基线

优化前主要瓶颈：

- `CountBadge Count=0` 且 `IsZeroVisible=false` 仍会创建 `CountBadgeAdorner`，隐藏 zero 场景承担了完整 adorner 成本。
- `DotBadge` 模板总是创建文本 `Label`，无 `Text` 或 target adorner 模式也会承担 Label visual。
- `DotBadge`、`CountBadge`、`RibbonBadge` 在 standalone 和 `DecoratedTarget` 模式之间切换时，adorner 的旧 visual parent 没有统一释放路径，存在后续冲突和泄露风险。
- Badge 颜色每次解析都会新建 brush；Ribbon corner darken brush 位于 render 热路径。

优化前控件级数据，Debug headless，`--suite badge --count 60`：

| 场景 | ms/item | KB/item | Visual/root | 关键形态 |
| --- | ---: | ---: | ---: | --- |
| `Badge.Count.Target` | `0.109` | `27.4` | `2` | target CountBadge |
| `Badge.Count.ZeroHiddenTarget` | `0.110` | `27.3` | `2` | target zero hidden |
| `Badge.Count.StandaloneZeroHidden` | `0.090` | `18.9` | `2` | `CountBadgeAdorner=1` |
| `Badge.Dot.StandaloneStatus` | `0.397` | `80.9` | - | `Label=1` |
| `Badge.Dot.StandaloneText` | `0.524` | `92.3` | - | `Label=1` |
| `Badge.GalleryShape` | `54.496` | `19818.4` | `394` | `Label=27` |

优化前真实 Gallery `BadgeShowCase`：

| 场景 | Mean | Median | P95 | Alloc mean | Visuals | Logical |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | `247.25ms` | `247.25ms` | `247.25ms` | `12551.88KB` | `497` | `173` |
| Repeated navigation | `46.13ms` | `45.85ms` | `60.21ms` | `10649.01KB` | `497` | `173` |

## 实施内容

- `CountBadge` 在 `Count=0 && !IsZeroVisible` 时不再创建 `_badgeAdorner`，隐藏 zero target/standalone 都只保留必要 target 或 Badge 根节点。
- 修复 `CountBadge` 运行时 `IsZeroVisible=false -> true` 不能恢复显示的正确性问题。
- `DotBadgeAdorner` 的文本 `Label` 从 XAML 模板移动到代码按需创建，仅 standalone 且 `Text` 非空时创建。
- `CountBadge`、`DotBadge`、`RibbonBadge` 增加 standalone adorner attach/detach 路径，切换到 `DecoratedTarget` 前先从 Badge 自身移除 adorner。
- `HideAdorner` 清理 `_adornerLayer`，并在从 `AdornerLayer` 移除后清空 adorned element，避免旧引用残留。
- `BadgeColorUtils` 增加有界 preset/custom color brush 缓存，使用 immutable brush 复用解析结果，避免动态颜色字符串无限增长。
- `RibbonBadgeAdorner.Render()` 不再逐帧创建 corner darken brush，改为在 `RibbonColor` 或 darken amount 变化时重建。
- `CountBadgeAdorner` 避免重复设置相同 `CountText`，无 shadow brush 时清掉旧 `BoxShadow`。
- 性能工具新增 `--suite badge`、`--verify-badge-states`，Gallery 工具新增 `--showcase badge` 和 Badge runtime/source 形态统计。

## Phase 记录

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 0 | 建立控件级和真实 Gallery 基线，补充 Badge visual 统计 | Done |
| Phase 1 | CountBadge zero hidden 按需创建 | Done |
| Phase 2 | DotBadge 文本 Label 按需创建 | Done |
| Phase 3 | standalone/target adorner 生命周期收敛 | Done |
| Phase 4 | 颜色 brush 缓存与 Ribbon render 热路径收敛 | Done |
| Phase 5 | Gallery `BadgeShowCase` 真实场景复测 | Done |
| Phase 6 | 状态切换、父级释放、detach 清理验证 | Done |
| Phase 7 | 文档、总览和复现命令沉淀 | Done |

## 最终结果

控件级 suite，Debug headless，`--suite badge --count 60`：

| 场景 | 优化前 ms/item | 优化后 ms/item | 改善 | 优化前 KB/item | 优化后 KB/item | 改善 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| `Badge.Count.Target` | `0.109` | `0.091` | `16.51%` | `27.4` | `22.4` | `18.25%` |
| `Badge.Count.ZeroHiddenTarget` | `0.110` | `0.065` | `40.91%` | `27.3` | `12.0` | `56.04%` |
| `Badge.Count.StandaloneZeroHidden` | `0.090` | `0.031` | `65.56%` | `18.9` | `6.0` | `68.25%` |
| `Badge.Dot.StandaloneStatus` | `0.397` | `0.248` | `37.53%` | `80.9` | `57.1` | `29.42%` |
| `Badge.Dot.StandaloneText` | `0.524` | `0.464` | `11.45%` | `92.3` | `87.6` | `5.09%` |
| `Badge.GalleryShape` | `54.496` | `49.236` | `9.65%` | `19818.4` | `19481.0` | `1.70%` |

结构性变化：

| 场景 | 优化前 | 优化后 | 变化 |
| --- | ---: | ---: | ---: |
| `Badge.Count.StandaloneZeroHidden` visual/root | `2` | `1` | `-50.00%` |
| `Badge.Count.StandaloneZeroHidden` CountBadgeAdorner/root | `1` | `0` | `-100.00%` |
| `Badge.Dot.StandaloneStatus` Label/root | `1` | `0` | `-100.00%` |
| `Badge.GalleryShape` visual/root | `394` | `384` | `-2.54%` |
| `Badge.GalleryShape` Label/root | `27` | `22` | `-18.52%` |

真实 Gallery `BadgeShowCase`，Debug headless，`1300x900`，warmup 10，measured 30：

| 指标 | 优化前 | 优化后 | 变化 |
| --- | ---: | ---: | ---: |
| Cold first navigation | `247.25ms` | `247.65ms` | `-0.16%` |
| Cold alloc | `12551.88KB` | `12237.20KB` | `2.51%` |
| Repeated mean | `46.13ms` | `47.50ms` | `-2.97%` |
| Repeated median | `45.85ms` | `39.69ms` | `13.44%` |
| Repeated P95 | `60.21ms` | `70.97ms` | `-17.87%` |
| Repeated alloc | `10649.01KB` | `10378.31KB` | `2.54%` |
| Runtime visuals | `497` | `485` | `2.41%` |
| Runtime logical | `173` | `171` | `1.16%` |
| Runtime Label | `27` | `22` | `18.52%` |

结论：Badge 本身的隐藏态和无文本态成本已明显下降；真实 `BadgeShowCase` 里大部分 Badge 都是可见示例，且仍需要 adorner、motion actor 和 indicator，因此 Gallery 打开耗时收益主要体现在 visual/alloc 结构下降，导航 timing 均值不应解读为显著提升。后续如果继续压 BadgeShowCase，应拆 motion actor、ShowCaseItem 和 Button/Icon 组合成本。

## 验证覆盖

- hidden zero target/standalone `CountBadge` 不创建 `_badgeAdorner`。
- `Count=0` 时 `IsZeroVisible=false -> true -> false` 可正确创建、显示、隐藏，并清理 `_adornerLayer`。
- target `DotBadge` 无文本时不创建 `Label`。
- `BadgeIsVisible` 运行时隐藏/显示会清理 `_adornerLayer`，detach 后 `_adornerLayer` 为空。
- `DecoratedTarget` 从 old target 切换到 new target 时旧 target 无 visual parent。
- Count/Dot/Ribbon standalone 和 target 模式双向切换时，旧 adorner 不残留在 Badge 自身下，target 也会正确释放。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
```

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-badge-states
```

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite badge --count 60
```

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase badge --iterations 30 --warmup 10 --label badge-optimized-final
```
