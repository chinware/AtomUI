# Button 性能优化方案

## 背景

`Button` 是 AtomUI 的高频基础控件。它不仅直接出现在业务页面和 Gallery 中，也被 `DropdownButton`、`SplitButton`、OptionButton、Form action、Input accessory、Menu/NavMenu 等体系间接引用。

本方案坚持和前几轮一致的原则：

**不使用的功能不创建、不绑定、不监听、不参与状态计算。**

本轮先聚焦控件级成本和真实 `ButtonShowCase` 场景，不处理 Gallery 页面懒加载、路由缓存、ShowCase 容器重构等上层问题。

## 当前结构

主要实现文件：

- `src/AtomUI.Desktop.Controls/Buttons/Button.cs`
- `src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonTheme.axaml`
- `src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonTheme.cs`
- `src/AtomUI.Controls/Buttons/Converters/ButtonIconVisibleConverter.cs`
- `src/AtomUI.Desktop.Controls/Buttons/DropdownButton.cs`
- `src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonTheme.axaml`
- `src/AtomUI.Desktop.Controls/Buttons/SplitButton.cs`
- `src/AtomUI.Desktop.Controls/Buttons/Themes/SplitButtonTheme.axaml`

普通 `Button` 当前模板默认包含：

- 外层 `Panel`
- `WaveSpiritDecorator#PART_WaveSpirit`
- `Border#ShadowsFrame`
- `Border#Frame`
- `DockPanel#PART_ContentLayout`
- `IconPresenter#PART_ButtonIcon`
- `ContentPresenter#PART_ContentPresenter`

当前已经完成过两轮默认 slot 优化：`LoadingOutlined` 不再默认创建，Phase 2 后固定 `PART_LoadingIconHost` 也已移除，只有 `IsLoading=true` 时才由代码创建并插入 `PART_ContentLayout`。但 icon presenter、wave decorator 和大量 selector 仍然是默认成本。

## 当前基线

### Gallery 场景

命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase button --warmup 3 --iterations 10 --label button-current --timeout-ms 15000
```

结果：

| 指标 | 值 |
| --- | ---: |
| `ButtonShowCase` source `atom:Button` | 89 |
| `ButtonShowCase` source `AntDesignIconProvider` | 49 |
| `ButtonShowCase` source `IsLoading=True` | 3 |
| repeated navigation mean | `134.60ms` |
| repeated navigation median | `136.20ms` |
| repeated navigation p95 | `155.73ms` |
| repeated allocation mean | `32177.02KB` |
| runtime visuals | 1128 |
| runtime logical | 218 |
| runtime Button | 89 |
| runtime Icon | 52 |
| runtime IconPresenter | 92 |
| runtime PathIcon | 52 |

解释：

- `Icon=52` 基本等于显式 icon 49 + loading icon 3，说明 `LoadingOutlined` 已经按需创建。
- `IconPresenter=92` 明显高于实际 icon 需求，说明大量无 icon Button 仍然创建 `PART_ButtonIcon`。
- 真实页面中只有 3 个 loading Button，但 89 个 Button 都创建了 loading host 和 loading 状态管线。

### 控件级微基准

命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite icon --count 60
```

关键结果：

| 场景 | ms/item | KB/item | Visual/root | Logical/root | Icon/root | IconPresenter/root | ButtonIconPresenter/root | Wave/root | ButtonLoadingHost/root |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Button.Default.Text` | `1.863` | `298.8` | `10.0` | `2.0` | `0.0` | `1.0` | `1.0` | `1.0` | `1.0` |
| `Button.Default.NoWave` | `2.347` | `298.9` | `10.0` | `2.0` | `0.0` | `1.0` | `1.0` | `1.0` | `1.0` |
| `Button.Text.Text` | `1.001` | `296.1` | `10.0` | `2.0` | `0.0` | `1.0` | `1.0` | `1.0` | `1.0` |
| `Button.Link.Text` | `0.808` | `297.9` | `10.0` | `2.0` | `0.0` | `1.0` | `1.0` | `1.0` | `1.0` |
| `Button.Primary.Loading` | `1.343` | `419.8` | `12.0` | `2.0` | `1.0` | `1.0` | `1.0` | `1.0` | `1.0` |

解释：

- 默认文本 Button 不再有 `Icon/root`，但仍有 `IconPresenter/root=1`。
- loading Button 比默认 Button 多约 `118.8KB/item` 和 2 个 visual/root，这部分是真实 loading 成本，应只由 loading Button 承担。
- `Button.Default.NoWave` 仍有 `Wave/root=1`，说明关闭 wave 不会避免模板创建 wave visual。
- `Button.Text.Text` / `Button.Link.Text` 仍有 `Wave/root=1`，说明不播放 wave 的类型仍承担 wave visual 成本。

## 性能问题清单

### P0: 伪类更新存在正确性风险

当前 `Button.UpdatePseudoClasses()` 设置：

- `:icononly`
- `:loading`
- `:default`
- `:dashed`
- `:primary`
- `:link`
- `:text`
- `:danger`

但 `OnPropertyChanged()` 只在 `Content` 或 `IsLoading` 变化时调用 `UpdatePseudoClasses()`。这意味着运行时改变 `Icon`、`ButtonType`、`IsDanger` 时，伪类可能不同步。

这是正确性问题，优先级高于纯性能优化。

建议：

- `IconProperty`、`ContentProperty`、`IsLoadingProperty`、`ButtonTypeProperty`、`IsDangerProperty` 变化时都更新伪类。
- 避免重复 setter 引发无意义 class 变化：内部先计算目标布尔值，只有变化时写入。
- 增加验证覆盖运行时切换 `Icon`、`ButtonType`、`IsDanger`、`IsLoading`。

### P1: 非 loading Button 仍承担 loading 插槽与状态管线成本

Phase 0 情况：

- `LoadingOutlined` 已经按需创建。
- 但每个 Button 模板仍默认创建 `Panel#PART_LoadingIconHost`。
- `ButtonIconVisibleConverter` 的 `MultiBinding` 每个 Button 都监听 `Icon`、`IsIconVisible`、`IsLoading`。
- `ButtonTheme.axaml` 里有大量 `Icon#PART_LoadingIcon` selector。
- `OnApplyTemplate()` 每个 Button 都调用 `UpdateLoadingIcon()`。

问题：

- 真实 `ButtonShowCase` 中只有 3 个 loading Button，但所有 89 个 Button 都承担 loading host 和相关样式/绑定成本。

Phase 2 已处理：

- 模板移除固定 `PART_LoadingIconHost`。
- 代码在 `IsLoading=true` 时创建 `LoadingOutlined` 并插入 `PART_ContentLayout` 左侧。
- `IsLoading=false` 或 re-template 时移除 loading icon，清理 `TemplatedParent` 和旧 visual parent。
- 避免为非 loading Button 创建任何 loading host、loading icon binding 或 loading visual。

注意：

- 需要保证 loading icon 在 icon 前还是替代 icon，保持现有 UI 行为。
- 需要保证 `LoadingOutlined` 动画取消和 visual parent 清理，不引入资源泄露。
- 如果保留 loading 相关样式 selector，只有实际创建的 `PART_LoadingIcon` 会承受匹配，不影响无 loading Button 的 visual 数量。

### P2: 无 icon Button 仍承担 IconPresenter 与 MultiBinding 成本

当前情况：

- `ButtonTheme.axaml` 默认创建 `IconPresenter#PART_ButtonIcon`。
- `IconPresenter.IsVisible` 使用三路 `MultiBinding`：
  - `Icon`
  - `IsIconVisible`
  - `IsLoading`
- 默认文本 Button 微基准显示 `IconPresenter/root=1`。

问题：

- 无 icon Button 占 Button 实例的大多数，但仍创建 presenter、binding expression、converter 路径和模板子树。
- `ButtonShowCase` 运行时 `IconPresenter=92`，但实际 icon 只有 52。

建议：

- 模板移除固定 `PART_ButtonIcon`。
- 代码在 `Icon != null && IsIconVisible && !IsLoading` 时创建 `IconPresenter` 并插入 `PART_ContentLayout` 左侧。
- `Icon` 变为 null、`IsLoading=true`、`IsIconVisible=false` 时移除 presenter。
- 用代码侧直接同步 `IconPresenter.Icon`，不再为默认路径创建 `MultiBinding`。
- presenter 移除时解除 `Icon` 引用并清理 `TemplatedParent`，避免旧 icon 保持 visual parent 或 owner 引用。

风险：

- 当前大量 icon 颜色、尺寸、margin 样式 selector 都依赖 `PART_ButtonIcon`。
- 按需创建的 presenter 必须仍设置 `Name="PART_ButtonIcon"` 和 `TemplatedParent=this`，确保现有 `/template/ atom|IconPresenter#PART_ButtonIcon` selector 继续生效。
- 插入顺序必须稳定：loading icon 优先于 normal icon，content 保持最后填充。

### P3: Text / Link 类型仍承担 WaveSpiritDecorator 成本

当前情况：

- `ButtonType=Default/Link/Primary/Text` 共用模板，都会创建 `PART_WaveSpirit`。
- `Button.cs` 播放 wave 的条件只包含 `Primary`、`Default`、`Dashed`。
- `Text` / `Link` 实际不会播放 wave，但仍创建 wave visual、应用 wave token/style、参与模板查找和属性绑定。

建议：

引入内部判断：

```csharp
private bool ShouldUseWaveSpirit =>
    IsWaveSpiritEnabled &&
    !IsLoading &&
    (ButtonType == ButtonType.Primary ||
     ButtonType == ButtonType.Default ||
     ButtonType == ButtonType.Dashed);
```

优化方式：

- 模板移除固定 `PART_WaveSpirit`。
- 代码在 `ShouldUseWaveSpirit` 为 true 时按需创建 `WaveSpiritDecorator`，并插入最底层视觉位置。
- `ShouldUseWaveSpirit` 变为 false 时取消正在播放的 wave，移除 decorator。
- `ButtonType`、`IsWaveSpiritEnabled`、`IsLoading`、`Shape`、`EffectiveCornerRadius` 改变时同步 decorator。

风险：

- `WaveSpiritDecorator` 当前在 `OnDetachedFromVisualTree()` 取消 `CancellationTokenSource`，按需移除应复用该释放路径。
- 如果在动画播放期间移除 decorator，必须保证 CTS cancel/dispose，不留下后台动画。
- `WaveSpiritDecorator` 是 internal 控件，按需创建不改变公共 API。

预期收益：

- `Text` / `Link` 默认少 1 个 visual/root。
- 全局关闭 wave 时所有 Button 少 1 个 visual/root。
- loading Button 不再承担不会播放的 wave。

### P4: ButtonTheme selector 面积过大

当前 `ButtonTheme.axaml` 约 250 个 `<Style Selector>`，其中 icon/loading brush 相关 selector 很多。状态组合主要通过 XAML selector 表达：

- type: Default / Dashed / Primary / Text / Link
- state: hover / pressed / disabled
- flags: danger / ghost / loading / icononly
- slot: icon / loading icon / frame / shadow

问题：

- 对高频控件来说，大量属性 selector、伪类 selector、`/template/` selector 会产生样式匹配、激活器和状态监听成本。
- icon/loading brush selector 对没有 icon/loading 的 Button 没有 UI 收益。

建议分两步，不一次性大改：

1. 结构减重后先复测。如果 ButtonShowCase 和微基准收益已满足预期，暂不动 selector。
2. 若 selector 仍是瓶颈，再引入代码侧 effective 状态属性：
   - `EffectiveIconBrush`
   - `EffectiveLoadingIconBrush`
   - `EffectiveBorderThickness`
   - 必要时 `EffectiveBackground` / `EffectiveForeground` / `EffectiveBorderBrush`

边界：

- 视觉 token、可主题化颜色仍保留在样式资源里。
- 状态组合和默认路径的热计算可以转到代码。
- 不为追求极限性能一次性把整个 Button 样式体系硬编码。

### P5: ButtonTheme 模板重复与 Dashed 覆盖需要清理

当前 `ButtonTheme.axaml` 中存在三处 `Template` setter：

- `ButtonType=Default/Link/Primary/Text` 条件模板。
- `ButtonType=Dashed` 条件模板。
- 后续无条件 Dashed-looking 模板。

问题：

- 可能存在模板覆盖或维护误导。
- 即使当前运行正确，也会影响后续按需 slot 改造的判断。

建议：

- 先写验证确认各 ButtonType 实际使用的 frame 类型：
  - Default / Primary / Text / Link 应使用 `Border#Frame`。
  - Dashed 应使用 `DashedBorder#Frame`。
- 删除或收敛重复模板，只保留明确分支。
- Dashed 高度 selector 应只作用于 Dashed 模板。

### P6: MeasureOverride 写 CornerRadius 的副作用需要评估

当前 `MeasureOverride()` 中：

- 所有 Button 都执行 `targetWidth = Math.Max(targetWidth, targetHeight)`。
- `Circle` / `Round` 会在 measure 阶段写 `CornerRadius`。

问题：

- measure 中写 styled property 可能触发额外属性变化、effective corner radius 更新和潜在布局 invalidation。
- 默认形状强制 `width >= height` 可能是视觉需求，也可能是不必要成本和行为限制。

建议：

- 第一轮不直接改，先加验证和基线。
- 如果确认有重复 measure 或属性变更成本，再改为：
  - `Circle` / `Round` 使用内部 `EffectiveCornerRadius` 或 arrange 尺寸计算。
  - 默认形状只在 icon-only 或最小尺寸需要时保证 `width >= height`。

这是行为敏感项，不能和 slot 按需创建混在同一阶段。

## 推荐实施阶段

### Phase 0: Baseline 与验证补齐

目标：

- 建立 Button 专项微基准与 Gallery 基线。
- 补齐正确性验证，避免后续优化只看速度不看行为。

任务：

- 在 `tools/performances/AtomUI.Performance` 新增 Button suite 或 Button 子目录场景：
  - Default text Button
  - Default icon Button
  - Primary text Button
  - Text Button
  - Link Button
  - Dashed Button
  - Loading Button
  - Icon + Loading 切换
  - Ghost + Danger
  - Circle / Round
  - DropdownButton
  - SplitButton
- 新增验证：
  - runtime 设置 `Icon` 后 `:icononly` 与 presenter 创建正确。
  - runtime 切换 `IsLoading` 后 loading icon 创建/移除正确。
  - runtime 切换 `ButtonType` 后 type pseudo class 正确。
  - runtime 切换 `IsDanger` 后 `:danger` 正确。
  - Default/Primary/Text/Link/Dashed 模板 frame 类型正确。
  - loading / icon / wave slot re-template 后无 visual parent 异常。
- 固化 `ButtonShowCase` Gallery 基线到 `docs/performances/Button`。

完成标准：

- 有当前版本 baseline 文档。
- 验证可以作为后续每个阶段的回归入口。

Phase 0 当前结果：

- 已新增 `tools/performances/AtomUI.Performance/Suites/Button`。
- 已新增 `--suite button` 控件级基准。
- 已新增 `--verify-button-states` 状态/模板验证入口。
- 已固化 [button-baseline.md](button-baseline.md)。
- 已固化 [button-showcase-baseline.md](button-showcase-baseline.md)。
- `--verify-button-states` 当前失败 4 项，均为 Phase 1 的伪类同步问题：
  - runtime 设置 `Icon` 后未设置 `:icononly`。
  - runtime 切换 `ButtonType=Primary` 后未设置 `:primary`。
  - runtime 切换 `ButtonType` 后未清理旧的 `:default`。
  - runtime 设置 `IsDanger=true` 后未设置 `:danger`。

### Phase 1: 正确性优先修复

目标：

- 修复伪类同步和模板重复/覆盖风险。
- 不做结构性大改，降低后续阶段误判。

任务：

- `IconProperty`、`ButtonTypeProperty`、`IsDangerProperty` 变化时更新伪类。
- 清理或确认重复 Dashed 模板。
- 检查 `WaveSpiritDecorator.OpacityMotionDurationProperty` 注册名疑似使用了 `nameof(SizeMotionDuration)` 的问题，必要时独立修复。

完成标准：

- Button 验证全部通过。
- `ButtonShowCase` 视觉结构符合预期。

Phase 1 当前结果：

- 已修复 runtime `Icon` / `ButtonType` / `IsDanger` 变化时伪类不同步问题。
- 已在 `Shape` 变化时同步 `WaveSpiritType`。
- 已清理 `ButtonTheme.axaml` 后半段重复的无条件 Dashed 模板。
- 已修复 `WaveSpiritDecorator.OpacityMotionDurationProperty` 注册名错误。
- `--verify-button-states` 已通过。
- `ButtonShowCase` smoke 结构保持 `Visuals=1128`、`Button=89`、`Icon=52`、`IconPresenter=92`。

### Phase 2: Loading slot 按需创建

目标：

- 非 loading Button 不再创建 loading host。
- loading 成本只由 `IsLoading=true` 的 Button 承担。

任务：

- 模板移除固定 `PART_LoadingIconHost`。
- 代码创建/插入/移除 `LoadingOutlined`。
- 确保 re-template、`IsLoading` 反复切换和旧实例移除过程无泄露。
- 保持 loading icon 的样式 selector 兼容。

预期：

- 默认 Button `Visual/root` 至少减少 1。
- `ButtonShowCase` 中 86 个非 loading Button 不再创建 loading host。

Phase 2 当前结果：

- `ButtonTheme.axaml` 已移除固定 `PART_LoadingIconHost`。
- `Button` 在 `IsLoading=true` 时创建 `LoadingOutlined#PART_LoadingIcon` 并插入 `PART_ContentLayout`。
- `Button` 在 `IsLoading=false`、re-template 前移除 loading icon，并清空 `TemplatedParent`。
- `--verify-button-states` 已覆盖非 loading 不创建、loading 创建、关闭移除、旧实例无 visual parent、再次开启创建新实例。
- 控件级 `Button.Default.Text`：`Visual/root 10 -> 9`，`ButtonLoadingHost/root 1 -> 0`，`ms/item 1.863 -> 1.537`。
- Gallery `ButtonShowCase`：`Visuals 1128 -> 1039`，repeated mean `134.60ms -> 107.82ms`，alloc mean `32177.02KB -> 31587.43KB`。
- `IconPresenter=92` 未变化，说明 Phase 3 仍有明确结构减重空间。

### Phase 3: IconPresenter 按需创建

目标：

- 无 icon Button 不再创建 `IconPresenter`。
- 移除默认路径的 `MultiBinding + ButtonIconVisibleConverter` 成本。

任务：

- 模板移除固定 `PART_ButtonIcon`。
- 代码按 `Icon != null && IsIconVisible && !IsLoading` 创建/移除 `IconPresenter`。
- 保持 `PART_ButtonIcon` 命名和 `TemplatedParent`，兼容现有样式。
- 删除或保留 converter 需看 `DropdownButton` 是否同步改造；若仍被 DropdownButton 使用，不删除。

预期：

- 默认 Button `IconPresenter/root: 1 -> 0`。
- `ButtonShowCase` 中 `IconPresenter` 接近实际 icon 需求数量。

Phase 3 当前结果：

- `ButtonTheme.axaml` 已移除固定 `PART_ButtonIcon` 和默认路径 `MultiBinding`。
- `Button` 在 `Icon != null && IsIconVisible && !IsLoading` 时创建 `IconPresenter#PART_ButtonIcon`。
- 移除 icon presenter 时会从 parent children 删除、清空 `Icon` 引用并清空 `TemplatedParent`。
- 按需创建的 presenter 保持 `Name=PART_ButtonIcon` 和 `TemplatedParent=this`，现有 `/template/ atom|IconPresenter#PART_ButtonIcon` selector 继续生效。
- 控件级 `Button.Default.Text`：`IconPresenter/root 1 -> 0`。
- Gallery `ButtonShowCase`：`IconPresenter 92 -> 51`。

### Phase 4: WaveSpiritDecorator 按需创建

目标：

- `Text` / `Link`、loading 状态、全局关闭 wave 时不创建 wave decorator。

任务：

- 引入 `ShouldUseWaveSpirit`。
- 模板移除固定 `PART_WaveSpirit`。
- 代码按需创建/移除 `WaveSpiritDecorator`。
- 动画中移除时正确 cancel/dispose。
- 保证 z-order：wave decorator 仍在底层，不遮挡 frame/content。

预期：

- `Text` / `Link` Button `Visual/root` 再减少 1。
- 全局关闭 wave 的页面收益更明显。

Phase 4 当前结果：

- `ButtonTheme.axaml` 已移除固定 `PART_WaveSpirit`。
- `Button` 仅在 `Default` / `Primary` / `Dashed`、非 loading、且 `IsWaveSpiritEnabled=true` 时创建 `WaveSpiritDecorator#PART_WaveSpirit`。
- `WaveSpiritDecorator` 按需创建后由代码同步 `CornerRadius` 和 `WaveType`，仍使用全局 `WaveSpiritDecoratorTheme` token。
- `Text` / `Link` / loading / no-wave Button 不创建 wave。
- `WaveSpiritDecorator` 动画取消路径已收敛，取消时会清理 CTS 和 `_isPlaying`。

### Phase 5: DropdownButton / SplitButton 联动评估

目标：

- 避免只优化 Button 本体，却让派生或组合控件继续承担同类成本。

任务：

- `DropdownButtonTheme.axaml` 也有固定 `PART_ButtonIcon` 和 `MultiBinding`，评估是否同步复用 Button 的 icon slot 管理。
- `DropdownButton` 当前 `OnApplyTemplate()` 会在 `RightExtraContent == null` 时创建一个空 `Border`，确认是否仍有必要。
- `SplitButton` 组合两个 `Button`，应天然受 Button 优化收益影响；需要复测。

完成标准：

- `DropdownButtonShowCase` / `SplitButtonShowCase` 无行为回归。
- 不引入 public API 变更。

Phase 5 当前结果：

- `DropdownButtonTheme.axaml` 已同步移除固定 `PART_ButtonIcon`、默认 `MultiBinding` 和固定 `PART_WaveSpirit`，复用 `Button` 的按需 slot 管理。
- `DropdownButton.OnApplyTemplate()` 中未使用的空 `Border` 分配已移除。
- `SplitButton` 由两个 `Button` 组合，本轮不新增独立模板改造；它会自然继承内部 Button 的 loading/icon/wave 按需创建。
- 控件级 `Button.Dropdown.Default`：`ButtonIconPresenter/root 1 -> 0`。

### Phase 6: Selector 热路径收敛评估

目标：

- 在结构减重后，再判断是否需要牺牲部分 XAML selector 优雅度来换性能。

任务：

- 基于 Phase 2-4 后的测量结果判断是否继续。
- 如果继续，优先收敛 icon/loading brush 状态：
  - default value 来自 token/style。
  - 状态组合由代码计算 effective brush。
  - presenter/loading icon 只绑定 effective brush。
- 保留主题资源，不把 token 硬编码到控件。

完成标准：

- 有明确数据证明 selector 收敛带来收益。
- 文档记录“哪些状态留在 Style，哪些状态进代码”的边界。

Phase 6 当前结论：

- 本轮不继续把 icon/loading brush selector 迁到代码。
- Phase 3-5 已把没有对应 slot 的默认路径成本移除，selector 只在真实创建的 icon/loading visual 上生效。
- 继续迁移 brush 状态需要引入 effective brush 状态机，风险主要是主题 token 表达能力下降、状态组合更难维护。
- 当前没有单独数据证明 brush selector 收敛能带来足够收益，因此先保留 XAML selector。

### Phase 7: Gallery 真实场景复测

目标：

- 证明优化对真实 `ButtonShowCase` 有收益，而不是只改善微基准。

任务：

- 复测：
  - `ButtonShowCase`
  - `DropdownButtonShowCase`
  - `SplitButtonShowCase`
  - 受 Button 间接影响的 `LineEditShowCase`、`SelectShowCase`、`MenuShowCase`
- 每轮至少保留：
  - repeated mean / median / p95
  - alloc mean
  - Visual / Logical
  - Button / Icon / IconPresenter / PathIcon

完成标准：

- `ButtonShowCase` 结构性指标下降。
- timing 有稳定改善或明确说明瓶颈转移。
- 无 visual parent 异常、无事件订阅泄露、无动画 CTS 泄露。

Phase 7 当前结果：

- 已固化 [button-phase3-7.md](button-phase3-7.md)。
- 已固化 [button-showcase-phase7.md](button-showcase-phase7.md)。
- 已新增 GalleryPerformance `dropdownbutton` / `splitbutton` 入口；smoke 结果只保留汇总，不单独入库原始阶段文档。
- `ButtonShowCase` repeated mean `134.60ms -> 101.53ms`，提升约 `24.57%`。
- `ButtonShowCase` alloc mean `32177.02KB -> 26653.55KB`，下降约 `17.17%`。
- `ButtonShowCase` runtime visuals `1128 -> 964`。
- `ButtonShowCase` `IconPresenter 92 -> 51`。
- `DropdownButtonShowCase` smoke：mean `54.91ms`，alloc mean `6940.50KB`，`Visuals=227`，`Button=17`，`IconPresenter=17`。
- `SplitButtonShowCase` smoke：mean `74.90ms`，alloc mean `9693.20KB`，`Visuals=318`，`Button=26`，`IconPresenter=13`。

## 资源泄露约束

所有按需创建都必须满足：

- 创建的 child 只能有一个 owner，不复用 Control 实例。
- 插入前确认没有 visual parent。
- 移除时从 parent children 中删除。
- 移除时清空 `TemplatedParent`。
- 移除 icon presenter 时清空 `Icon` 引用。
- event handler 必须具名订阅、具名解绑。
- `WaveSpiritDecorator` 动画中的 CTS 必须 cancel/dispose。
- re-template 前先 detach 旧 slot。
- detached from visual tree 时清理临时 slot。

## 风险矩阵

| 项目 | 风险 | 控制方式 |
| --- | --- | --- |
| 伪类修复 | 低 | 行为更正确；加 runtime 切换验证 |
| loading slot 按需创建 | 中 | 重点验证切换和 re-template，不复用 Control |
| icon presenter 按需创建 | 中 | 保持 `PART_ButtonIcon` 名称和 templated parent，复用现有 selector |
| wave 按需创建 | 中高 | 重点验证动画 cancel/dispose 和 z-order |
| selector 收敛到代码 | 中高 | 必须有数据再做；保持 token 可主题化 |
| MeasureOverride 调整 | 高 | 单独阶段处理，不与 slot 优化混改 |

## 预期收益

保守预期：

- 默认文本 Button 减少至少 2 个默认 visual：loading host、icon presenter。
- Text / Link Button 再减少 1 个 wave visual。
- 无 icon Button 移除 3 路 `MultiBinding` 和 converter 调用路径。
- `ButtonShowCase` 的 `IconPresenter` 数量应从 92 降到接近实际 icon/loading 需求。

不承诺：

- 不承诺 Gallery 打开时间立刻提升 50%。`ButtonShowCase` 只有 89 个 Button，且页面还包含 ShowCase 容器、WrapPanel、TextBlock、主题/style、路由等成本。
- 第一目标是结构性减重和默认路径成本归零；真实耗时改善由复测决定。

## 不建议第一轮做的事

- 不建议直接重写整套 Button 主题为代码绘制。
- 不建议删除 `IsLoading`、`Icon`、`IsGhost`、`IsDanger` 等 API。
- 不建议为了 Button 本体优化破坏 `DropdownButton`、`SplitButton` 的现有模板兼容。
- 不建议在没有数据证明前大规模迁移所有颜色状态到代码。
- 不建议缓存任何 `Icon`、`IconPresenter`、`LoadingOutlined`、`WaveSpiritDecorator` Control 实例。

## 实施任务列表

- [x] Phase 0: 新增 Button 控件级微基准场景。
- [x] Phase 0: 新增 Button runtime 行为验证。
- [x] Phase 0: 固化当前 `ButtonShowCase` baseline 文档。
- [x] Phase 1: 修复 `Icon` / `ButtonType` / `IsDanger` 变化时伪类不同步问题。
- [x] Phase 1: 验证并清理 `ButtonTheme.axaml` 重复 Dashed 模板。
- [x] Phase 1: 评估并修复 `WaveSpiritDecorator.OpacityMotionDurationProperty` 注册名问题。
- [x] Phase 2: 移除固定 loading host，实现 loading icon 按需插入与清理。
- [x] Phase 2: 增加 loading 反复切换、re-template 与旧实例 visual parent 清理验证。
- [x] Phase 3: 移除固定 `PART_ButtonIcon`，实现 icon presenter 按需创建。
- [x] Phase 3: 移除默认路径 `MultiBinding`，保留样式兼容。
- [x] Phase 4: 实现 `WaveSpiritDecorator` 按需创建和动画安全清理。
- [x] Phase 5: 评估 `DropdownButton` / `SplitButton` 联动收益和兼容性。
- [x] Phase 6: 基于数据决定是否做 selector 状态收敛。
- [x] Phase 7: 复测 Button / DropdownButton / SplitButton Gallery 场景。
- [x] Phase 7: 更新最终性能结果和结论。
