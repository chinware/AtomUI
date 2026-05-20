# Card 性能优化

`Card` 是 Data Display 中的高频容器控件。本轮已完成 Phase 0-7：建立独立基线、复现真实 `CardShowCase`，并将 Header/Cover/Actions/Skeleton 等可选槽位改为按需承担成本。

## Phase 0 基线

测试环境：Debug，Avalonia headless，`2026-05-15`。

控件级独立 suite：`tools/performances/AtomUI.Performance --suite card --count 60`。

| 场景 | ms/item | KB/item | Visual/root | Card | CardActionPanel | Skeleton | 关键形态 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| `Card.ContentOnly` | `2.318` | `361.7` | `24` | `1` | `1` | `1` | 纯内容 Card 仍创建 header、cover、actions、skeleton 槽位 |
| `Card.HeaderExtra` | `4.075` | `560.8` | `32` | `1` | `1` | `1` | Header/Extra 引入 `HyperLinkButton` 和 icon presenter 成本 |
| `Card.CoverMeta` | `2.170` | `480.5` | `29` | `1` | `1` | `1` | Cover/Meta 场景仍承担空 actions panel |
| `Card.Actions3` | `3.381` | `806.3` | `50` | `1` | `1` | `1` | 3 个 action button 带来 Button/Icon/ButtonPresenter 成本 |
| `Card.LoadingFalse` | `3.151` | `817.8` | `51` | `1` | `1` | `1` | 非 loading 状态仍创建 Skeleton 包装层 |
| `Card.LoadingTrue` | `2.264` | `814.1` | `54` | `1` | `1` | `1` | loading 显示骨架分支，隐藏真实内容分支 |
| `Card.Grid7` | `2.739` | `835.4` | `53` | `1` | `1` | `1` | 7 个 `CardGridItem`，每个 item 有 BoxShadow 边线成本 |
| `Card.Tabs` | `3.031` | `1102.4` | `64` | `1` | `1` | `1` | TabControl 是必要成本，但不是默认 Card 固定成本 |
| `Card.GalleryShape.Batch18` | `39.180` | `13482.7` | `692` | `18` | `18` | `18` | 复刻 Gallery 18 个 Card 的组合形态 |

真实 Gallery：`tools/performances/AtomUI.GalleryPerformance --showcase card --iterations 30 --warmup 10`。

| 场景 | Mean | Median | P95 | Alloc mean | Visuals | Logical | Card | CardActionPanel | Skeleton | CardActionButton |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | `376.94ms` | `376.94ms` | `376.94ms` | `15018.47KB` | `784` | `98` | `18` | `18` | `18` | `9` |
| Repeated navigation | `78.11ms` | `79.32ms` | `90.93ms` | `13962.15KB` | `784` | `98` | `18` | `18` | `18` | `9` |

Gallery 源 XAML 形态：

| Card | CardActionButton | CardGridContent | CardGridItem | CardMetaContent | CardTabsContent | Avatar | Image | ShowCaseItem |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `18` | `9` | `1` | `7` | `4` | `2` | `3` | `2` | `10` |

## 优化结果

### 控件级结果

测试环境：Debug，Avalonia headless，`tools/performances/AtomUI.Performance --suite card --count 60`。

| 场景 | 基线 ms/item | 优化后 ms/item | 耗时变化 | 基线 KB/item | 优化后 KB/item | Visual/root | 关键结果 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| `Card.ContentOnly` | `2.318` | `0.670` | `-71.10%` | `361.7` | `94.0` | `24 -> 10` | 纯内容 Card 不再创建 Header/Cover/ActionPanel/Skeleton |
| `Card.HeaderExtra` | `4.075` | `3.140` | `-22.94%` | `560.8` | `330.6` | `32 -> 22` | Header/Extra 只在实际使用时创建 |
| `Card.CoverMeta` | `2.170` | `1.448` | `-33.27%` | `480.5` | `211.7` | `29 -> 17` | Cover 槽位按需创建，headerless 圆角语义保留 |
| `Card.Actions3` | `3.381` | `3.561` | `+5.32%` | `806.3` | `542.9` | `50 -> 39` | 只有 actions 场景创建 `CardActionPanel`；该场景耗时有小幅噪声回退，但分配和节点稳定下降 |
| `Card.GalleryShape.Batch18` | `39.180` | `30.477` | `-22.21%` | `13482.7` | `7185.4` | `692 -> 490` | 复刻 Gallery 18 个 Card 的组合形态 |

`Card.GalleryShape.Batch18` 中，`CardActionPanel/root` 从 `18 -> 3`，`Skeleton/root` 从 `18 -> 0`。

### Gallery 真实场景

真实 Gallery：`tools/performances/AtomUI.GalleryPerformance --showcase card --iterations 30 --warmup 10`。

| 场景 | 基线 | 优化后 | 变化 | Alloc mean | Visuals | CardActionPanel | Skeleton |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | `376.94ms` | `319.03ms` | `-15.36%` | `15018.47KB -> 11091.88KB` | `784 -> 582` | `18 -> 3` | `18 -> 0` |
| Repeated navigation mean | `78.11ms` | `50.66ms` | `-35.14%` | `13962.15KB -> 9306.75KB` | `784 -> 582` | `18 -> 3` | `18 -> 0` |
| Repeated navigation median | `79.32ms` | `49.85ms` | `-37.15%` | - | `784 -> 582` | `18 -> 3` | `18 -> 0` |
| Repeated navigation P95 | `90.93ms` | `60.97ms` | `-32.95%` | - | `784 -> 582` | `18 -> 3` | `18 -> 0` |

Gallery 源 XAML 形态未改变：`18` 个 `Card`，`9` 个 `CardActionButton`，`1` 个 `CardGridContent`，`7` 个 `CardGridItem`，`4` 个 `CardMetaContent`，`2` 个 `CardTabsContent`。

## 已实施内容

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 1 | 非 loading Card 使用轻量 `ContentPresenter`，`Skeleton` 仅在 `IsLoading=True` 时创建 | Done |
| Phase 2 | `CardActionPanel` 仅在 `Actions.Count > 0` 时创建，清空 actions 时释放 | Done |
| Phase 3 | `CoverFrame` 和 cover presenter 按 `Cover/CoverTemplate` 创建和释放 | Done |
| Phase 4 | Header/Extra presenter 树按实际 header 内容创建，`headerless` 语义保留 | Done |
| Phase 5 | BoxShadow transition 只在 `IsMotionEnabled=True` 且 `IsHoverable=True` 时承担成本 | Done |
| Phase 6 | `CardGridContent.ColumnDefinitions` / `RowDefinitions` 改为真实 styled property wrapper | Done |
| Phase 7 | `OnApplyTemplate` 期间合并 root layout 重排，避免重复 remove/add 子节点 | Done |

## 正确性与资源释放验证

- `tools/performances/AtomUI.Performance --verify-card-states` 已覆盖空槽懒创建、Header/Cover/ActionPanel/Skeleton 创建与释放、旧 visual parent 清理、templated parent 清理、actions clear/recreate、motion scope、Grid definitions 初始与运行时同步。
- 动态创建的 Header 仍通过 `/template/ Border#HeaderFrame` 命中 Card token padding/font/background 等样式。
- 所有懒创建路径都具备对应 release path；本轮没有新增长期订阅或未释放绑定。

## 结论

- `CardShowCase` 的 repeated mean `78.11ms` 不算最糟糕的一类 Gallery 页面，但对 18 个 Card 来说结构成本偏重。
- 本轮优化符合“不使用的功能不承担成本”：Gallery 中只有 3 个 Card 有 actions，运行时也只创建 3 个 `CardActionPanel`；非 loading Card 不再创建 `Skeleton`。
- 控件级纯内容场景收益最大，`ms/item -71.10%`，`KB/item -74.01%`，`Visual/root 24 -> 10`。
- Gallery 真实重复导航收益明显，mean `-35.14%`，median `-37.15%`，P95 `-32.95%`，alloc mean `-33.34%`，visuals `784 -> 582`。
- 后续若继续压 Card，需要单独看 `CardTabsContent`、`CardGridItem` 阴影和 action button/icon 组合成本；这些是实际使用功能，不属于本轮默认空槽成本。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
```

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-card-states
```

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite card --count 60 \
  --markdown /tmp/atomui-card-optimized.md
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj
```

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase card --iterations 30 --warmup 10 --timeout-ms 30000 \
  --label card-optimized \
  --markdown /tmp/atomui-card-showcase-optimized.md
```
