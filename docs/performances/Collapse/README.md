# Collapse 性能优化

`Collapse` 是 Data Display 中的折叠容器控件。本页记录 Phase 0 基线、本轮优化内容和最终结果。

## Phase 0 基线

测试环境：Debug，Avalonia headless，`2026-05-15`。

### 怎么看这些数字

- `ms/item`：创建、套模板、布局稳定的平均耗时。越低越好。
- `KB/item`：当前线程分配量平均值。越低越好。
- `Visual/root`：每个场景根控件展开后的视觉节点数。它不是绝对性能，但能解释成本来自哪里。
- `MotionActor`、`ExpandButton`、`AddOnPresenter`：Collapse 当前模板中的高关注固定槽位。

### 控件级基线

命令：

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite collapse --count 30 \
  --markdown /tmp/atomui-collapse-baseline.md
```

| 场景 | ms/item | KB/item | Visual/root | Collapse | CollapseItem | MotionActor | ExpandButton | AddOnPresenter | 说明 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| `Collapse.SingleClosed` | `2.291` | `246.0` | `17` | `1` | `1` | `1` | `1` | `1` | 单个关闭项仍创建内容动画宿主、图标按钮和空 addon presenter |
| `Collapse.SingleOpen` | `3.208` | `293.1` | `20` | `1` | `1` | `1` | `1` | `1` | 打开项比关闭项只多约 3 个 visual，说明关闭态已经承担了大部分成本 |
| `Collapse.Basic3Closed` | `6.559` | `666.3` | `43` | `1` | `3` | `3` | `3` | `3` | 三个关闭项线性复制固定槽位成本 |
| `Collapse.Basic3FirstOpen` | `5.458` | `713.6` | `46` | `1` | `3` | `3` | `3` | `3` | 首项打开后视觉树仅小幅增加 |
| `Collapse.NoArrow2` | `2.013` | `389.2` | `26` | `1` | `2` | `2` | `2` | `2` | `IsShowExpandIcon=False` 仍保留 expand button 槽位 |
| `Collapse.AddOn3` | `3.640` | `759.1` | `49` | `1` | `3` | `3` | `3` | `3` | 真实 addon 只在 3 项都存在时才合理；默认空槽仍是优化点 |
| `Collapse.GalleryShape` | `31.059` | `8031.3` | `521` | `16` | `33` | `33` | `33` | `33` | 复刻 Gallery 主体形态，未包含 ShowCaseItem 外壳 |

### Gallery 真实场景

命令：

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase collapse --warmup 2 --iterations 6 --timeout-ms 30000 \
  --markdown /tmp/atomui-collapse-gallery-baseline.md
```

源 XAML：`17` 个 `Collapse`、`34` 个 `CollapseItem`、`10` 个 `ShowCaseItem`、`3` 个 `AntDesignIconProvider`。

运行时稳定视觉树：`616` 个 visual、`141` 个 logical。运行时统计为 `16` 个 `Collapse`、`33` 个 `CollapseItem`，因为嵌套在关闭内容里的部分树不会进入当前可见 visual 统计。

| 场景 | Mean | Median | P95 | Alloc mean | Visuals | Logical | Collapse | CollapseItem | MotionActor | ExpandButton | AddOnPresenter | IconButton | TextBlock |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | `205.17ms` | `205.17ms` | `205.17ms` | `11445.31KB` | `616` | `141` | `16` | `33` | `33` | `33` | `33` | `33` | `60` |
| Repeated navigation | `119.31ms` | `112.27ms` | `149.27ms` | `10509.44KB` | `616` | `141` | `16` | `33` | `33` | `33` | `33` | `33` | `60` |

重复导航逐次样本：`159.99ms`、`117.09ms`、`116.46ms`、`106.57ms`、`108.08ms`、`107.64ms`。

## 读数结论

- `CollapseShowCase` 的 repeated mean `119.31ms` 对这个页面的视觉复杂度来说偏高，不是最糟糕的一类页面，但成本密度不合理。
- 最明确的问题是关闭态成本：`SingleClosed` 已经有 `17` 个 visual，并且默认创建 `MotionActor=1`、`ExpandButton=1`、`AddOnPresenter=1`。
- `SingleOpen` 只有 `20` 个 visual，说明“打开”并不是主要增量；主要浪费在所有关闭 item 已经提前创建了内容和交互槽位。
- Gallery 里实际只有 3 个 item 有 addon，但运行时有 `33` 个 `PART_AddOnContentPresenter`，这是直接违反“不使用的功能不承担成本”的点。
- `NoArrow2` 仍有 `2` 个 expand button，说明隐藏箭头目前没有免除按钮槽位成本。
- `ColorPickerCollapse` 复用 Collapse，且内容是 `ItemTemplate -> ItemsControl -> PaletteColorItem`，后续优化关闭内容按需创建时需要单独纳入验证，收益和风险都会比纯文本内容更高。

## 已执行优化

本轮遵循“不使用的功能不承担成本”，但不改变控件 API、视觉行为和交互语义。

| Phase | 内容 | 关键约束 |
| --- | --- | --- |
| P0 | 修正 `IsBorderless/IsGhostStyle` 运行时同步分支；修正 `ItemHeaderPadding/ItemContentPadding` 动态同步 | 正确性优先；item 本地 padding 仍保持更高优先级 |
| P1 | `PART_AddOnContentPresenter` 仅在 `AddOnContent/AddOnContentTemplate` 存在时创建，清空后释放 | 释放时清空 Content/Template、移出 visual tree、清理 templated parent |
| P1 | `IsShowExpandIcon=False` 时不创建 `PART_ExpandButton` 和默认 `RightOutlined` | 动态开关可创建、释放、再创建；释放 Click 事件 |
| P2 | 关闭态不创建 `PART_ContentMotionActor + PART_ContentPresenter`，首次展开时创建 | 展开/折叠后复用同一 actor，不重复创建；保留动画入口 |
| P3 | `SelectionChanged` 从全量 item 边框重算收敛到变更项重算 | borderless/ghost/border thickness 这类全局状态仍全量同步 |
| P4 | 补齐直接 `CollapseItem` 从 `Items` 移除时的 padding binding 释放 | 移除后不再跟随 `Collapse.ItemHeaderPadding/ItemContentPadding` 更新 |

## 优化后结果

测试环境：Debug，Avalonia headless，`2026-05-15`。

### 控件级对比

| 场景 | 基线 ms/item | 优化后 ms/item | 提升 | 基线 KB/item | 优化后 KB/item | Visual/root | MotionActor | ExpandButton | AddOnPresenter |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Collapse.SingleClosed` | `2.291` | `1.834` | `19.95%` | `246.0` | `212.3` | `17 -> 15` | `1 -> 0` | `1 -> 1` | `1 -> 0` |
| `Collapse.SingleOpen` | `3.208` | `3.064` | `4.49%` | `293.1` | `273.4` | `20 -> 19` | `1 -> 1` | `1 -> 1` | `1 -> 0` |
| `Collapse.Basic3Closed` | `6.559` | `4.686` | `28.56%` | `666.3` | `565.8` | `43 -> 37` | `3 -> 0` | `3 -> 3` | `3 -> 0` |
| `Collapse.Basic3FirstOpen` | `5.458` | `5.122` | `6.16%` | `713.6` | `627.1` | `46 -> 41` | `3 -> 1` | `3 -> 3` | `3 -> 0` |
| `Collapse.NoArrow2` | `2.013` | `2.104` | `-4.52%` | `389.2` | `292.5` | `26 -> 21` | `2 -> 0` | `2 -> 1` | `2 -> 0` |
| `Collapse.AddOn3` | `3.640` | `4.567` | `-25.47%` | `759.1` | `678.8` | `49 -> 46` | `3 -> 0` | `3 -> 3` | `3 -> 3` |
| `Collapse.GalleryShape` | `31.059` | `29.663` | `4.49%` | `8031.3` | `6926.0` | `521 -> 458` | `33 -> 1` | `33 -> 32` | `33 -> 3` |

说明：

- 结构收益是确定的：`GalleryShape` 少 `63` 个 visual，空 addon 和关闭内容 actor 基本消除。
- 部分小场景计时变差，但分配和 visual 下降。小样本 `ms/item` 对 Debug/headless 调度抖动敏感，判断优先看结构、分配和 Gallery 真实场景。
- `Basic3FirstOpen` 的内容成本被移动到首次展开路径，属于预期取舍；关闭态不再提前承担内容树成本。

### Gallery 真实场景对比

| 场景 | 基线 | 优化后 | 提升 |
| --- | ---: | ---: | ---: |
| Cold first navigation | `205.17ms` | `193.35ms` | `5.76%` |
| Repeated navigation mean | `119.31ms` | `100.18ms` | `16.03%` |
| Repeated navigation median | `112.27ms` | `96.16ms` | `14.35%` |
| Repeated navigation P95 | `149.27ms` | `117.20ms` | `21.48%` |
| Alloc mean | `10509.44KB` | `9347.90KB` | `11.05%` |
| Visuals | `616` | `553` | `10.23%` |
| Collapse content motion | `33` | `1` | `96.97%` |
| Collapse expand button | `33` | `32` | `3.03%` |
| Collapse addon presenter | `33` | `3` | `90.91%` |

优化后重复导航样本：`123.20ms`、`98.08ms`、`99.19ms`、`93.82ms`、`92.52ms`、`94.23ms`。

## 结论

- 本轮达成主要目标：默认关闭态不再创建内容 motion actor 和空 addon presenter。
- Gallery 真实打开耗时有可见收益，但不是 50% 级别；`CollapseShowCase` 里仍保留 `32` 个 expand button/icon，这是真实功能成本，不应为了数据移除。
- 后续若继续压榨，需要评估用更轻量的 expand icon 交互替代 `IconButton`，但这会触及视觉/交互细节，风险高于本轮优化。

## 原优化目标

优先级从高到低：

| 优先级 | 目标 | 预期收益 | 风险 |
| --- | --- | --- | --- |
| P0 | 修正 `IsBorderless/IsGhostStyle` 运行时同步分支和 padding 动态同步 | 正确性优先 | 低 |
| P1 | `AddOnContentPresenter` 按需创建，清空后释放 | Gallery 可减少约 30 个空 presenter | 低，中等生命周期验证 |
| P1 | `IsShowExpandIcon=False` 时不创建 expand button/icon 槽位 | No Arrow 场景直接减少 button/icon 成本 | 中，需验证 Header/Icon trigger 行为 |
| P2 | 关闭内容不创建 `LayoutAwareMotionActor + ContentPresenter`，首次展开时创建 | 最大结构收益，尤其 ColorPicker | 中高，必须验证动画、re-template、detach、重复开合无泄露 |
| P3 | 边框计算从全量扫描改为局部更新 | 大量 item 场景收益 | 中，需覆盖首尾项、accordion、动画中边框 |

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0
```

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite collapse --count 30
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0
```

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase collapse --warmup 2 --iterations 6 --timeout-ms 30000
```
