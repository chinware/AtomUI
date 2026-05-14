# Space / CompactSpace 性能优化方案

## 背景

`SpaceShowCase` 当前 cold first navigation 为 `769.44ms`，repeated navigation mean 为 `165.32ms`，runtime visual 数为 `1864`。这对一个布局类 ShowCase 来说偏高。

当前页面的真实成本不在 8 个普通 `Space`，而在 24 个 `CompactSpace`、93 个 `CompactSpaceItem` wrapper，以及这些 wrapper 内部承载的 Button、LineEdit、SearchEdit、Select、Picker、Cascader、TreeSelect、AddOnDecoratedBox 等复合控件。

本轮原则：

**不使用的功能不承担成本；不在 layout 热路径里做可提前完成的工作；所有按需创建和事件订阅必须可释放、可验证。**

## 当前基线

测试命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase space --warmup 3 --iterations 10 \
  --label space-showcase-baseline --timeout-ms 30000 \
  --markdown /tmp/atomui-space-showcase-baseline-output.md
```

核心数据：

| 指标 | 当前值 |
| --- | ---: |
| cold first navigation | `769.44ms` |
| repeated navigation mean | `165.32ms` |
| repeated navigation p95 | `196.46ms` |
| repeated allocation mean | `39791.91KB` |
| visuals | `1864` |
| logical | `136` |
| `Space` | `8` |
| `CompactSpace` | `24` |
| `CompactSpaceItem` | `93` |
| `Button` | `63` |
| `LineEdit total` | `20` |
| `Select` | `7` |
| `AddOnDecoratedBox` | `36` |

阶段目标：

| 阶段 | cold 目标 | repeated mean 目标 | 结构目标 |
| --- | ---: | ---: | --- |
| Phase 1-2 | `<650ms` | `<145ms` | 移除明显热路径浪费 |
| Phase 3-4 | `<560ms` | `<125ms` | 减少事件订阅、binding 和重建成本 |
| Final | `<500ms` | `<120ms` | 无正确性回归、无资源泄露 |

这些目标是 Debug headless 的阶段性目标，不承诺等同真实平台渲染耗时。

## 范围边界

本轮优先处理：

- `CompactSpace`
- `CompactSpaceItem`
- `CompactSpaceAddOn`
- `Space` 的低风险默认成本
- `tools/performances` 中 Space 专项基准与验证

本轮暂不优先处理：

- Gallery 分段加载、虚拟化、路由缓存。
- 重写 ShowCase XAML，减少展示内容。
- 删除 `CompactSpace` API。
- 大规模改动 LineEdit / Select / Picker 等子控件；这些子控件只作为回归观测对象。

## 性能问题与方案

### P0: Filler 校验在 Measure 热路径且对象不对

当前 `CompactSpace.MeasureCore()` 每次 measure 都扫描 `_contentLayout.Children`，但 `_contentLayout.Children` 放的是 `CompactSpaceItem`，不是原始 `CompactSpaceFiller`。这既浪费，又可能没有真正校验业务规则。

方案：

- 新增 `ValidateFillerUsage()`，校验 `Children` 原始列表。
- 在 `OnApplyTemplate()`、children add/remove、children move、`ConfigureSizeDefinitions()` 前调用。
- 从 `MeasureCore()` 移除 filler 校验，避免每次 measure 扫描。
- 校验规则保持当前语义：最多一个 filler，且只能作为尾部 filler。

完成标准：

- 错误位置的 filler 能稳定抛出 `InvalidSpaceFillerUsageException`。
- 正常 `SpaceShowCase` 不再在 measure 阶段执行 filler 扫描。

### P1: `PointerExited` sender 逻辑错误

当前 `CompactSpace` 订阅的是 target child 的 `PointerExited`，但 handler 判断 `sender is CompactSpaceItem`。这意味着退出时可能不会复位 z-index。

方案：

- 根据 sender child 查找对应 `CompactSpaceItem`。
- 对非 focus、非 always active 的 wrapper 恢复 `NORMAL_ZINDEX`。
- 补齐 pointer enter / exit 验证。

完成标准：

- pointer enter 后当前 item 为 active。
- pointer exit 后当前 item 在未 focus 时恢复 normal。
- always active child 不被错误复位。

### P2: Re-template / detach 生命周期不完整

当前 `OnApplyTemplate()` 只清理 class changed handlers，然后重新创建 wrapper；旧 wrapper 上的 child 事件订阅没有统一走完整 detach 路径。

方案：

- 抽出 `DetachCompactSpaceItems()`。
- re-template 前对旧 `_contentLayout.Children` 中的每个 `CompactSpaceItem` 执行 `NotifyRemoveCompactSpaceItem()`。
- 清空旧 grid children。
- detached from visual tree 时不清掉必要的 children，但要释放 visual-tree 相关监听，attached 后只恢复必要监听。
- 增加 re-template 验证，确认不会重复订阅。

完成标准：

- re-template 不出现 visual parent 异常。
- re-template 前后的 pointer/focus 行为一致。
- child remove 后不会继续响应 compact space 的事件处理。

### P3: `CompactSpaceItem.MeasureOverride()` 每次分配 `TranslateTransform`

当前 wrapper 在 `MeasureOverride()` 内为非首项创建新的 `TranslateTransform`。这属于 layout 热路径分配和属性写入副作用。

方案：

- 在 `CompactSpaceItem` 内缓存一个私有 `TranslateTransform`。
- 仅当 orientation、position index 或 border thickness 变化时更新 transform。
- 首项或单项清理 transform，避免无意义 property value。
- 避免在 measure 中 new object；第一阶段可仍在 measure 中调用同步方法，但同步方法必须只在值变化时写属性。

完成标准：

- repeated measure 不产生新的 transform 实例。
- horizontal / vertical overlap 位移保持现有视觉。
- 单项、first、first|last 不设置多余 transform。

### P4: `ConfigureSizeDefinitions()` 全量重建与 LINQ 分配

当前每次配置都会新建 Grid definitions，并用 `Where(...).ToList()` 计算 real children。ShowCase 有 24 个 `CompactSpace`，这部分会被反复放大。

方案：

- 用普通循环替代 LINQ 分配。
- 记录当前 orientation、children count、item size signature。
- signature 未变化时跳过 Grid definitions 重建。
- position/orientation 通知只在目标值变化时执行。
- children add/remove 先保守全量配置，后续再评估增量更新。

完成标准：

- 静态 `CompactSpace` 在重复 layout 中不反复重建 definitions。
- 仍支持 runtime 改 `ItemSize`、`Orientation`、children 增删。

### P5: Per-child 事件订阅按需化

当前每个 compact child 都可能订阅 focus、pointer、property changed、classes changed。对 93 个 wrapper 来说，这是明显隐形成本。

方案：

- 只在真实 compact item 数量大于 1 时启用 z-index tracking。
- 对 `IgnoreZIndexChange()` 的 child 不注册 pointer/focus 事件。
- class changed tracking 只为需要 focus-within 的 child 注册；能用 focus routed event 覆盖的场景优先走 container 级 handler。
- 评估将 pointer/focus 订阅改为 `_contentLayout` 或 `CompactSpace` 级别 routed handler，避免每 child 独立订阅。

完成标准：

- `CompactSpace` 单子项不注册 z-index 相关订阅。
- 多子项 hover/focus 的 z-index 行为不变。
- remove child、re-template 后订阅可完全释放。

### P6: Relay binding 改代码直写评估

当前每个 `CompactSpaceItem` 对 child 建立三路 relay binding：

- `CompactSpaceItemPosition`
- `CompactSpaceOrientation`
- `IsUsedInCompactSpace`

`SpaceShowCase` 93 个 wrapper 至少带来 279 条 relay binding。

方案：

- Phase 1-4 先不动 binding，避免行为面过大。
- 单独评估由 `CompactSpace` 直接写 wrapper 和 child 的 internal compact 状态。
- 如果实施，`CompactSpaceItem` 不再为 child 创建 binding expression。
- 保留 `ICompactSpaceAware.NotifyPositionChange()` / `NotifyOrientationChange()` 作为状态入口，避免 API 扩散。

完成标准：

- `LineEdit`、`SearchEdit`、`Select`、`DatePicker`、`TimePicker`、`Button`、`NumericUpDown` compact 圆角和边框仍正确。
- binding 数下降，并反映在 allocation 或 repeated timing 上。

### P7: 普通 `Space` 默认 token binding 按需化

普通 `Space` 构造时总是创建 `ItemSpacing` 和 `LineSpacing` token binding。当前这不是主瓶颈，但符合“不使用不承担成本”原则。

方案：

- 当 `ItemSpacing` / `LineSpacing` 有 local value 或 binding 时，不再为对应属性维护 token binding。
- 当 `SizeType=Custom` 时，默认不创建 token binding。
- `SizeType` 从 Custom 切回 Small/Middle/Large 时，如无 local value，再恢复 token binding。

完成标准：

- 显式 `ItemSpacing` / `LineSpacing` 的 `Space` 不承担默认 token binding 成本。
- 默认 `Space` 的 token 行为保持不变。

## 实施阶段

### Phase 0-7 实施结果

本轮已完成 Phase 0-7。关键结果：

| 指标 | Baseline | Final | 变化 |
| --- | ---: | ---: | ---: |
| cold first navigation | `769.44ms` | `795.77ms` | `-3.42%` |
| repeated navigation mean | `165.32ms` | `150.48ms` | `+8.98%` |
| repeated navigation p95 | `196.46ms` | `168.76ms` | `+14.10%` |
| repeated allocation mean | `39791.91KB` | `39301.50KB` | `+1.23%` |
| runtime visuals | `1864` | `1846` | `-18` |
| runtime `CompactSpaceItem` | `93` | `75` | `-18` |

结论：

- Space / CompactSpace 自身的无效 wrapper、relay binding、transform 分配、重复 definitions 和事件订阅成本已收敛。
- 控件级 suite 中 compact 场景能看到结构和 allocation 降低。
- 真实 `SpaceShowCase` repeated timing 有小幅改善，但 cold first navigation 未达到目标，且单样本波动明显。
- 剩余成本主要来自页面内大量 Button、LineEdit/SearchEdit、Select、Picker、Cascader、TreeSelect 和 AddOnDecoratedBox 模板 materialization，不应继续把 cold 指标压力全部归因于 `Space` 本体。

### Phase 0: Baseline 与验证补齐

目标：

- 建立 Space 控件级基准。
- 补齐 CompactSpace 正确性验证，避免后续只看 timing。

任务：

- 在 `tools/performances/AtomUI.Performance` 新增 `Suites/Space`。
- 新增 micro 场景：
  - `Space.Horizontal.Button3`
  - `Space.Wrap.Button15`
  - `Space.Split.Link3`
  - `CompactSpace.LineEdit3`
  - `CompactSpace.ButtonGroup`
  - `CompactSpace.MixedForm`
  - `CompactSpace.SingleChild`
  - `CompactSpace.WithFiller`
- 新增验证入口：
  - `--verify-space-states`
- 验证项：
  - filler 只能在尾部，且最多一个。
  - first / middle / last position 正确。
  - horizontal / vertical orientation 下圆角正确。
  - pointer enter / exit 后 z-index 正确。
  - focus within 后 z-index 正确。
  - child remove 后状态清理正确。
  - re-template 后不重复订阅、不残留旧 wrapper。

完成标准：

- Space suite 能输出控件级 baseline。
- `--verify-space-states` 可作为每阶段回归入口。
- 在 `README.md` / 本方案中固化关键汇总数据；原始跑分输出按需生成，不入库。

### Phase 1: 正确性优先修复

目标：

- 先修会影响行为和后续判断的正确性问题。

任务：

- 移出并修复 filler 校验。
- 修复 `PointerExited` sender 判断。
- 补齐 re-template 前完整 detach。
- 增加 remove child / re-template 验证。

完成标准：

- `--verify-space-states` 全部通过。
- `SpaceShowCase` 无 visual parent 异常。
- 无重复事件订阅迹象。

### Phase 2: Layout 热路径减负

目标：

- 移除 measure/arrange 热路径中的明显分配和副作用。

任务：

- 缓存 `CompactSpaceItem` overlap transform。
- transform 只在值变化时更新。
- 避免 `MeasureOverride()` 中重复 new `TranslateTransform`。
- 复测 micro 和 Gallery。

完成标准：

- `CompactSpace.LineEdit3`、`CompactSpace.ButtonGroup` allocation 下降。
- `SpaceShowCase` repeated mean 有可见下降。

### Phase 3: Grid definitions 与状态计算收敛

目标：

- 减少静态 compact group 在初始化和 layout 稳定过程中的重复重建。

任务：

- 移除 `ConfigureSizeDefinitions()` 中的 LINQ 分配。
- 增加 definitions signature，未变化时跳过重建。
- position/orientation 状态只在变化时通知。
- 保守保留 children 变更后的全量配置。

完成标准：

- `CompactSpace` 静态场景 repeated allocation 下降。
- runtime 改 `ItemSize`、`Orientation`、children add/remove 仍正确。

### Phase 4: Event subscription 按需化

目标：

- 减少每个 child 默认承担的事件订阅成本。

任务：

- 单子项或无 z-index 需求时不注册 pointer/focus。
- `IgnoreZIndexChange()` child 跳过对应订阅。
- 评估 container 级 routed handler 替代 per-child handler。
- 保证 detach/re-template 全量释放。

完成标准：

- `CompactSpace.SingleChild` 不承担 z-index 事件订阅。
- `CompactSpace.ButtonGroup` hover/focus 行为不变。
- 事件订阅数量下降，并反映到 allocation 或初始化 timing。

### Phase 5: Relay binding 代码直写评估

目标：

- 判断 279 条 compact relay binding 是否值得迁移。

任务：

- 建立可观测指标：binding count 如果无法直接量化，则以 allocation、tree shape、timing 判断。
- 原型化 `CompactSpace` 直接通知 wrapper + child。
- 保留 `ICompactSpaceAware` 现有接口，不做 public API 变更。
- 逐项验证输入控件 compact 圆角和边框。

完成标准：

- 有明确数据证明收益再合入。
- 如果收益不足，保留 XAML/binding 方案，并在文档记录不继续的理由。

### Phase 6: 普通 Space 低风险优化

目标：

- 补齐普通 `Space` 的默认成本按需化。

任务：

- `ItemSpacing` / `LineSpacing` 有 local value 或 external binding 时，不创建对应 token binding。
- `SizeType=Custom` 时不维护 token binding。
- split template 场景验证：`SplitTemplate` 变化后 split visual 正确创建/清理。

完成标准：

- 普通 `Space` 默认行为不变。
- 显式 spacing 场景 allocation 降低。

### Phase 7: Gallery 复测与决策

目标：

- 判断控件级优化对真实 `SpaceShowCase` 是否足够。

任务：

- 复测 `SpaceShowCase`：
  - cold first navigation
  - repeated mean / median / p95
  - allocation mean
  - visuals / logical
  - `Space` / `CompactSpace` / `CompactSpaceItem`
  - Button / LineEdit / Select / AddOnDecoratedBox
- 对比 baseline：
  - cold `769.44ms`
  - repeated mean `165.32ms`
  - alloc mean `39791.91KB`
  - visuals `1864`

完成标准：

- cold 低于 `500ms` 或明确说明剩余瓶颈。
- repeated mean 低于 `120ms` 或明确说明剩余瓶颈。
- 如果达不到目标，再讨论 Gallery 分段加载、ShowCase 内容拆分或 route cache。

## 风险矩阵

| 项目 | 风险 | 控制方式 |
| --- | --- | --- |
| filler 校验迁移 | 低 | 正确性更强；补异常验证 |
| pointer exit 修复 | 低 | 只修明显 sender 错误；补 z-index 验证 |
| re-template detach | 中 | 严格成对 add/remove handler；补 re-template 验证 |
| transform 缓存 | 中 | 每 item 独立实例；不跨控件复用 mutable transform |
| definitions 缓存 | 中 | signature 保守，变化时全量重建 |
| event subscription 按需化 | 中高 | 保留行为验证，分阶段改 |
| relay binding 代码直写 | 中高 | 单独阶段，有数据再合入 |
| ordinary Space token binding | 低中 | 保持默认无 local value 行为 |

## 资源泄露约束

所有阶段必须满足：

- event handler 必须具名订阅、具名解绑。
- re-template 前必须释放旧 wrapper 对 child 的订阅。
- child remove 后不能保留 child -> CompactSpace 的引用链。
- 不缓存 Control 实例给其他 parent 复用。
- mutable transform 只能归属单个 `CompactSpaceItem`。
- detached from visual tree 后不保留 visual-tree 相关临时监听。
- verification 失败时不继续做下一阶段性能优化。

## 不建议第一轮做的事

- 不建议直接拆 `SpaceShowCase` 内容来制造性能提升。
- 不建议删除 `CompactSpaceItem` wrapper；这会影响 Grid sizing、z-index、overlap 和 child 状态传播，风险太大。
- 不建议一次性把所有 compact 状态从 binding 改成代码。
- 不建议把 LineEdit / Select / Picker 的内部优化混入 Space 第一轮。
- 不建议为了 cold timing 引入 route cache 掩盖控件成本。

## 实施任务列表

- [x] Phase 0: 新增 Space suite micro baseline。
- [x] Phase 0: 新增 `--verify-space-states`。
- [x] Phase 0: 固化控件级 Space baseline。
- [x] Phase 1: 修复 filler 校验位置和对象错误。
- [x] Phase 1: 修复 `PointerExited` sender 逻辑。
- [x] Phase 1: 补齐 re-template / remove child 生命周期清理。
- [x] Phase 2: 缓存 `CompactSpaceItem` overlap transform。
- [x] Phase 2: 复测 micro 与 `SpaceShowCase`。
- [x] Phase 3: 收敛 `ConfigureSizeDefinitions()` 分配与重复重建。
- [x] Phase 4: event subscription 按需化。
- [x] Phase 5: 评估 relay binding 代码直写。
- [x] Phase 6: 普通 `Space` token binding 按需化。
- [x] Phase 7: 复测 `SpaceShowCase` 并更新最终结论。
