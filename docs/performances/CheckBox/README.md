# CheckBox 性能优化方案

`CheckBox` 是基础输入控件，但使用面不只在 `CheckBoxShowCase`。它还出现在 `Form`、`DataGridCheckBoxColumn`、`TreeView` checkbox mode、`Transfer`、`MenuItem` toggle checkbox、Gallery workspace filter menu 等场景。单个控件成本如果偏高，会被 DataGrid 行、TreeView 节点、Transfer 列表和表单页面放大。

本轮已完成 Phase 0-7。优化边界是保持 public API、视觉状态、交互语义和动画行为不变；所有 lazy visual 都有对应释放路径，并通过专项状态验证覆盖。

## 优化前结构判断

### 已确认的高概率瓶颈

1. **默认 unchecked 也承担完整 indicator 成本**

   `CheckBoxTheme.axaml` 每个 `CheckBox` 都创建 `CheckBoxIndicator`，`CheckBoxIndicatorTheme.axaml` 又固定创建：

   - `WaveSpiritDecorator#PART_WaveSpirit`
   - `Border#Frame`
   - 内层 `Panel`
   - `antdicons:CheckBoldOutlined#CheckedMark`
   - `Rectangle#TristateMark`

   这意味着最常见的 unchecked 状态，也会创建 checked mark icon、tri-state mark 和 wave 节点。按“未使用功能不承担成本”的原则，这里是第一优先级。

2. **check mark 使用完整 AntDesign Icon Control**

   当前 checked mark 是 `CheckBoldOutlined`，虽然 Icon 体系已经做过 generated metadata 和 render 热路径优化，但对 CheckBox 这种固定小 glyph 来说，完整 Icon Control 仍然偏重。Avalonia Fluent/Simple CheckBox 模板使用的是 `Path`/几何数据，不会为固定 glyph 创建额外 Icon 控件体系。

3. **wave 默认预创建**

   `WaveSpiritDecorator` 只有在用户交互导致 checked 状态变化且 motion/wave 都启用时才需要播放。当前每个 CheckBox 默认都有一个 wave decorator，即使 disabled、contentless、DataGrid read-only、TreeView 大批量节点、从未点击的列表项也承担成本。

4. **多个 TemplateBinding / converter / `/template/` selector 叠加**

   `CheckBox` 到 `CheckBoxIndicator` 有 `IsMotionEnabled`、`IsWaveSpiritEnabled`、`IsEnabled`、`State` 等绑定，其中 `State` 还经过 `CheckBoxIndicatorStateConverter`。同时，外层 `CheckBox` 的 `:checked` / `:indeterminate` / `:disabled` / `:pointerover` selector 反复穿透到 `CheckBoxIndicator#Indicator` 设置 brush。

   这类写法样式表达清晰，但在高频控件上会带来绑定和 selector 匹配成本。是否迁移到代码状态机，需要由 Phase 0 数据证明，不应为了“看起来快”盲目牺牲主题可维护性。

5. **contentless CheckBox 仍创建 ContentPresenter 和 DockPanel**

   DataGrid cell、TreeView checkbox toggle、Transfer/Menu 等场景常见“只有勾选框，没有文本内容”。当前模板仍固定创建 `DockPanel` 和 `ContentPresenter`，只是通过 `IsVisible` 隐藏空内容。contentless 场景应该走轻量路径。

6. **CheckBoxGroup selection sync 有潜在冗余和正确性风险**

   `AbstractCheckBoxItemsControl.PrepareContainerForItemOverride()` 会在 item checked 时向 `SelectedItems` 添加 option；`UpdateCheckBoxCheckedStates()` 在 `SelectedItems != null` 时只把选中项设为 true，没有同步清理未选中容器。这里不一定是当前 Gallery 打开慢的主因，但属于必须在优化前补验证的区域。

### 当前 Gallery 形态

真实 `CheckBoxShowCase.axaml` 静态声明：

- 23 个直接 `CheckBox`
- 4 个 `CheckBoxGroup`
- 2 个 `Button`
- 6 个 `ShowCaseItem`

运行时 `BasicCheckBoxGroup` 会通过 `ItemsSource` 再生成 3 个 option，因此预期运行时约 26 个 `CheckBox`。这个页面本身数量不大，优化重点不应只看 `CheckBoxShowCase`；更关键的是 DataGrid / TreeView / Transfer / Menu 等高复用场景。

## 本轮实现

- `CheckBoxIndicatorTheme.axaml` 不再固定创建 `WaveSpiritDecorator`、`CheckBoldOutlined` 和 `Rectangle#TristateMark`，模板只保留 frame。
- `CheckBoxIndicator` 按状态创建轻量 visual：
  - unchecked：只保留 frame。
  - checked：创建 `Path#CheckedMark`，使用 AntDesign `CheckBoldOutlined` 的静态 geometry，不再创建 Icon 控件。
  - indeterminate：仅创建 `Rectangle#TristateMark`。
  - wave：仅在 loaded 后状态切到 checked 且 wave/motion/enabled 都满足时创建并播放；关闭 wave/motion/disabled 时释放。
- `AbstractCheckBox` 按需创建 `ContentPresenter#ContentPresenter`，contentless 场景不再承担 presenter 成本。
- `CheckBox -> CheckBoxIndicator` 的状态同步从 `TemplateBinding + CheckBoxIndicatorStateConverter` 收敛到代码直写，删除内部 converter。
- `CheckBoxGroup` 同步修正：
  - 外部替换 `CheckedItems` 会同步清理 stale checked item。
  - `PrepareContainerForItemOverride()` 避免重复添加 selected item。
  - `SelectionChanged` 处理去掉弱 `Debug.Assert`，并避免对同一个 `CheckedItems` 列表重复写入。

## 最终数据

控件级命令：

```bash
dotnet run --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite checkbox --count 60
```

| 场景 | 指标 | 优化前 | 优化后 | 变化 |
| --- | --- | ---: | ---: | ---: |
| `CheckBox.Default.Unchecked` | ms/item | 1.730 | 0.762 | -55.95% |
| `CheckBox.Default.Unchecked` | KB/item | 148.5 | 83.1 | -44.04% |
| `CheckBox.Default.Unchecked` | visuals/root | 12 | 8 | -33.33% |
| `CheckBox.Default.Checked` | ms/item | 1.172 | 0.812 | -30.72% |
| `CheckBox.Default.Checked` | KB/item | 160.1 | 92.9 | -41.97% |
| `CheckBox.Contentless.Unchecked` | ms/item | 1.013 | 0.473 | -53.31% |
| `CheckBox.Contentless.Unchecked` | KB/item | 137.3 | 66.9 | -51.27% |
| `CheckBox.Batch50.Mixed` | ms/item | 25.831 | 15.956 | -38.23% |
| `CheckBox.Batch50.Mixed` | KB/item | 7670.9 | 4359.4 | -43.17% |
| `CheckBox.Batch50.Mixed` | visuals/root | 605 | 403 | -33.39% |

真实 Gallery 命令：

```bash
dotnet run --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase checkbox --warmup 3 --iterations 10
```

| 场景 | 指标 | 优化前 | 优化后 | 变化 |
| --- | --- | ---: | ---: | ---: |
| `CheckBoxShowCase` cold | mean ms | 198.95 | 193.65 | -2.66% |
| `CheckBoxShowCase` repeated | mean ms | 65.03 | 59.92 | -7.86% |
| `CheckBoxShowCase` repeated | alloc KB | 6385.60 | 4629.80 | -27.50% |
| `CheckBoxShowCase` | visuals | 420 | 318 | -24.29% |
| `CheckBoxShowCase` | Icon | 26 | 0 | -100% |
| `CheckBoxShowCase` | checked mark | 26 | 10 | 按需创建 |
| `CheckBoxShowCase` | tristate mark | 26 | 2 | 按需创建 |

补充真实页面 smoke：

- `MenuShowCase` 的 `ToggleType=CheckBox` 当前不走 `AtomUI.Desktop.Controls.CheckBox`，因此本轮没有直接收益。
- `TreeViewShowCase` 运行时有 `55` 个 CheckBox、`19` 个 `CheckBoxIndicator`，当前只创建 `4` 个 checked mark 和 `4` 个 tristate mark。
- `DataGridShowCase` 运行时有 `17` 个 CheckBox/Indicator，当前只创建 `7` 个 checked mark。
- `TransferShowCase` 运行时有 `41` 个 CheckBox、`40` 个 Indicator，默认 unchecked 项不创建 mark/wave。

## 验证

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0
dotnet run --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-checkbox-states
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0
dotnet run --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase checkbox --warmup 3 --iterations 10
```

结果：

- CheckBox state verification passed.
- `AtomUI.Performance` build passed.
- `AtomUI.GalleryPerformance` build passed.
- Gallery build 过程中没有 CheckBox 相关 warning；曾出现的 DataGrid 未使用字段 warning 不属于本次改动。

## 优化边界

- 不改变 public API，除非 Phase 0 数据证明必须引入新的内部开关。
- 不改变视觉：checked / unchecked / indeterminate / disabled / pointerover 的背景、边框、mark 颜色必须一致。
- 不改变动画：border/background 过渡、checked mark scale、wave 播放时机必须保持。
- 不引入资源泄露：任何 lazy visual、event handler、binding、transition 或 render transform 都必须有 re-template / detach / 状态关闭清理路径。
- 不留下无效 using。

## Phase 0: Baseline 与观测

目标：先把数据和正确性验证补齐，不直接优化。

任务：

- 新增 `tools/performances/AtomUI.Performance/Suites/CheckBox/`。
- 新增 `--suite checkbox`，至少覆盖：
  - `CheckBox.Default.Unchecked`
  - `CheckBox.Default.Checked`
  - `CheckBox.Default.Indeterminate`
  - `CheckBox.Disabled.Checked`
  - `CheckBox.Contentless.Unchecked`
  - `CheckBox.Contentless.Checked`
  - `CheckBox.NoWave`
  - `CheckBoxGroup.Static3`
  - `CheckBoxGroup.ItemsSource3`
  - `CheckBox.Batch50.Mixed`
- `TreeStats` 增加专项计数：
  - `CheckBox`
  - `CheckBoxGroup`
  - `CheckBoxItemsControl`
  - `CheckBoxIndicator`
  - `WaveSpiritDecorator`
  - `CheckBoldOutlined` / icon mark
  - `TristateMark`
  - content presenter fast-path相关节点
- 新增 `--verify-checkbox-states`，覆盖：
  - checked / unchecked / indeterminate 视觉节点存在性
  - disabled + checked / disabled + indeterminate brush
  - pointerover brush
  - wave 启用/禁用、disabled、re-template 后无旧 visual parent
  - motion 开关和 transition 启停
  - CheckBoxGroup checked items 初始化、切换、清空、再次设置
- GalleryPerformance 增加 `--showcase checkbox`，加载真实 `CheckBoxShowCase.axaml`，输出源 XAML 与运行时树形态。
- 额外加两个复用场景观测：
  - `DataGridShowCase` 中显式 CheckBox 与 DataGridCheckBoxColumn
  - `TreeViewShowCase` 中 `ToggleType=CheckBox`

完成标准：

- 有控件级 baseline。
- 有真实 `CheckBoxShowCase` baseline。
- 明确默认 unchecked、contentless、checked、group 场景各自的 visual/alloc/timing。
- 正确性验证先通过，避免后续优化时把已有行为误判为新问题。

## Phase 1: Indicator 可选 visual 按需创建

目标：让 unchecked 默认路径不创建 checked mark、tri-state mark 和 wave。

方案：

- `CheckBoxIndicatorTheme.axaml` 保留轻量 frame。
- `CheckBoxIndicator` 代码侧按状态创建：
  - `CheckedMark`：仅 `State == Checked` 或从 checked 动画退出期间需要。
  - `TristateMark`：仅 `State == Indeterminate`。
  - `WaveSpiritDecorator`：仅 wave/motion/enabled/loaded 且实际需要播放前创建。
- 对 lazy child 统一实现 `Detach...()`：
  - 从 parent panel 移除。
  - 清空 event handler / binding / animation 引用。
  - re-template 时旧 child 必须无 visual parent。
- 首次加载的 checked / indeterminate 状态要直接显示，不播放“从无到有”的错误动画；用户交互后才保留现有动画行为。

预期收益：

- unchecked 默认路径减少 `Icon`、`Rectangle`、`WaveSpiritDecorator` 和内层容器相关成本。
- `CheckBoxShowCase` 只有部分 checked/indeterminate，DataGrid/TreeView 大量 unchecked 时收益更明显。

风险：

- 最容易破坏 checked mark scale 动画和 disabled brush。
- 必须用状态验证覆盖初始 checked、runtime checked、runtime unchecked、indeterminate 往返、disabled 往返。

## Phase 2: 固定 check mark 从 Icon 降为轻量几何

目标：固定 glyph 不再走完整 Icon Control。

方案选项：

1. 低风险：用 `Path` + static geometry 替换 `CheckBoldOutlined`。
2. 更激进：`CheckBoxIndicator` 自绘 checked mark / indeterminate mark。

建议先选 1。Avalonia 自带 Fluent/Simple theme 都使用 Path/geometry 表达 CheckBox glyph，这个方向风险可控。

实现要求：

- checked mark 尺寸仍使用 `CheckBoxToken.CheckedMarkSize`。
- brush 仍来自 `CheckedMarkBrush`。
- scale transform 和 `RenderTransformOrigin=Center` 行为保持。
- 不缓存 visual 实例，只缓存不可变 geometry。

预期收益：

- 每个 checked CheckBox 少一个 Icon Control。
- `Icon/root` 下降，Icon render/status/brush 相关成本不再被 CheckBox 放大。

## Phase 3: contentless CheckBox 快路径

目标：没有内容的 CheckBox 不创建 `ContentPresenter` 和不必要布局节点。

方案：

- 引入内部 `CheckBoxContentHost` 或在 `CheckBox` 代码中管理 content presenter。
- `Content == null && ContentTemplate == null` 时只保留 indicator。
- 首次设置 Content / ContentTemplate 时创建 `ContentPresenter`，清空后释放。
- 保留 `TextMargin`、`Foreground`、`VerticalAlignment`、`ContentTemplate` 行为。

重点场景：

- `DataGridCheckBoxColumn`
- `TreeViewItemHeaderTheme.axaml` 的 checkbox toggle
- `Transfer*Theme.axaml`
- `MenuItemTheme.axaml` 的 checkbox toggle

风险：

- `Content` runtime 从 null 变非 null / 再变 null 必须验证。
- 不能破坏 access key、content template、alignment。

## Phase 4: wave 与 motion 按实际交互付费

目标：不是所有 CheckBox 都承担 wave/transition 成本。

方案：

- `WaveSpiritDecorator` 不再模板固定创建。
- 仅在以下条件满足时创建或保留：
  - `IsWaveSpiritEnabled`
  - `IsMotionEnabled`
  - `IsEnabled`
  - `IsHitTestVisible`
  - 已加载，且状态变化来自可交互路径或即将播放 wave
- 对 disabled/read-only/contentless DataGrid cell 不创建 wave。
- transition 创建和启用沿用现有原则：初始化禁用，Loaded 后使用 `Dispatcher.Post(this.EnableTransitions);`。

验证：

- wave enabled 的普通 CheckBox 点击后仍播放。
- `IsWaveSpiritEnabled=false` 不创建 wave。
- disabled 不创建或不播放 wave。
- re-template 后旧 wave 无 visual parent。

## Phase 5: selector 与状态计算收敛评估

目标：只在 Phase 0-4 后仍有明确 selector/binding 成本时再做。

候选：

- 移除 `CheckBoxIndicatorStateConverter`，由 `CheckBox` 在代码里同步 indicator state。
- 移除 `IsMotionEnabled` / `IsWaveSpiritEnabled` / `IsEnabled` 到 indicator 的固定 TemplateBinding，改为 part 存在后直写同步。
- 将高频状态下的 indicator brush 计算收敛到 `CheckBoxIndicator` effective properties。

边界：

- 主题表达仍要清晰。纯视觉 token 映射优先留在 XAML。
- 只有数据证明 `/template/` selector 和 binding 是剩余瓶颈时才迁移。
- 迁移后必须补 visual state verification，尤其是 pointerover + checked + disabled + indeterminate 的组合。

## Phase 6: CheckBoxGroup 同步与生命周期

目标：修正潜在冗余，降低 group 切换时的重复同步。

方案：

- 给 `CheckBoxGroup` / `CheckBoxItemsControl` 增加状态验证：
  - ItemsSource 初始 checked
  - CheckedItems 初始值
  - CheckedItems 清空
  - 外部替换 CheckedItems
  - item toggle 后 group event 和 form value event
- 检查 `PrepareContainerForItemOverride()` 中 `SelectedItems?.Add(checkBoxOption)` 是否可能重复添加。
- `UpdateCheckBoxCheckedStates()` 在 `SelectedItems != null` 时应考虑同步清理未选中项，避免旧容器 stale checked。
- re-template 时确认 `SelectionChanged -= HandleItemsSelectedChanged` 已覆盖旧 items control，无订阅泄露。

## Phase 7: 集成场景复测

必须复测：

- `--suite checkbox --count 60`
- `--verify-checkbox-states`
- `--showcase checkbox --warmup 3 --iterations 10`
- `DataGridShowCase` 相关 checkbox 场景
- `TreeViewShowCase` checkbox toggle 场景
- `MenuShowCase` checkbox toggle 场景
- `Transfer` 如已有 Gallery route，补真实场景；没有则至少控件级批量场景

验收指标：

- 默认 unchecked 和 contentless visual/root 明显下降。
- `CheckBoxShowCase` visual/alloc 下降；timing 至少不恶化。
- DataGrid/TreeView 批量场景是主要收益来源，必须单独说明。
- 无 visual parent 异常、无 event/binding 泄露、无动画行为丢失。

## 预期收益判断

按当前结构，最值得做的是 Phase 1、Phase 2、Phase 3：

- Phase 1 解决“unchecked 也创建 checked/indeterminate/wave”的核心浪费。
- Phase 2 解决固定小 glyph 使用完整 Icon Control 的隐形成本。
- Phase 3 解决 DataGrid/TreeView/Transfer/Menu 这类 contentless 高频场景。

单个 `CheckBoxShowCase` 只有约 26 个 CheckBox，打开耗时不一定会有 50% 级别变化；但 DataGrid、TreeView、Transfer 这类批量场景预计更明显。最终结论必须以 Phase 0 和 Phase 7 的数据为准。
