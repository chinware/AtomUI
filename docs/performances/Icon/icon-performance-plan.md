# Icon 性能优化分析与方案

## 背景

`Icon` 是 AtomUI 的基础视觉原语。它不仅作为独立控件使用，也被 Button、IconButton、ToggleIconButton、Input clear/reveal、Select handle、Menu indicator、NavMenu indicator、Tabs close button、Message/Notification/Result status、TreeView switcher 等控件大量间接创建。

这类控件的性能目标应与 `AddOnDecoratedBox` 一致：

**不使用的功能不创建、不绑定、不监听、不参与状态计算。**

本方案先聚焦控件级与真实 Gallery 场景的可观测优化，不直接处理 Gallery 页面懒加载、路由、ShowCase 容器等上层问题。

## 当前架构

### Core Icon

当前核心实现：

- `src/AtomUI.Core/Controls/Icon/Icon.cs`
- `src/AtomUI.Core/Controls/Icon/DrawingInstruction.cs`
- `src/AtomUI.Core/Controls/Icon/IconProvider.cs`
- `src/AtomUI.Core/Controls/Icon/IconProviderCache.cs`

`Icon` 继承 Avalonia `PathIcon`，但没有使用 `PathIcon.Data` 模板绘制，而是重写 `Render()`，按 `DrawingInstructions` 自绘 geometry。

`Icon` 暴露了这些能力：

- `StrokeBrush` / `FillBrush`
- `SecondaryStrokeBrush` / `SecondaryFillBrush`
- `FallbackBrush`
- `StrokeWidth` / `StrokeLineCap` / `StrokeLineJoin`
- `IconTheme`
- `LoadingAnimation` / `LoadingAnimationDuration`
- `FillAnimationDuration`
- `IsMotionEnabled`

这些能力支持完整视觉体系，但多数普通静态图标只需要单色 fill。

### DrawingInstruction

`DrawingInstruction` 是图标绘制指令抽象。生成图标通常使用 `PathDrawingInstruction`，其 `Data` 是 `StreamGeometry.Parse(...)` 得到的 geometry。

当前绘制流程：

1. 首次绘制时通过 `BuildGeometry()` 缓存 geometry。
2. 每次 render 根据全局矩阵创建新的 `MatrixTransform`。
3. 临时写入 geometry 的 `Transform`。
4. 调用 `DrawingContext.DrawGeometry(...)`。
5. finally 恢复原始 transform。

这条路径功能完整，但存在热路径分配和共享 geometry 可变状态问题。

### AntDesign 图标包

当前实现：

- `src/AtomUI.Icons.AntDesign/AntDesignIcon.cs`
- `src/AtomUI.Icons.AntDesign/AntDesignIconProvider.cs`
- `src/AtomUI.Icons.AntDesign/GeneratedIcons/*.g.cs`
- `src/AtomUI.Icons.AntDesign.Generator/AntDesignGenerator.cs`

生成代码对每个图标生成一个 Control 类，并生成静态 `DrawingInstruction[] StaticInstructions`。这意味着 path 字符串解析在图标类型初始化时完成，后续实例共享 instruction 和 geometry 数据。

这是当前架构里比较好的部分：没有每个实例重复解析 SVG path。

### IconPark 图标包兼容性

外部包位置：

- `/Users/chinboy/Projects/dotnet/IconParkIconsPackage`

关键实现：

- `src/AtomUI.Icons.IconPark/IconParkIcon.cs`
- `src/AtomUI.Icons.IconPark/IconParkIconProvider.cs`
- `src/AtomUI.Icons.IconPark.Generator/IconParkIconsPackageGenerator.cs`
- `src/AtomUI.Icons.IconPark/GeneratedIcons/*.g.cs`

IconPark 的生成图标类继承 `IconParkIcon`，而 `IconParkIcon` 继承 `Icon`。和 AntDesign/Material 不同，IconPark 不为每个主题生成独立类，而是同一个 generated icon class 设置默认 `IconThemeType.Filled`，再通过 `IconParkIcon.FindIconBrush()` 根据当前 `IconTheme` 动态映射 brush：

- `Outlined`：只绘制 stroke 与 secondary stroke。
- `Filled` / `Rounded` / `Sharp`：把 fill、secondary brush 映射到 stroke/fallback。
- `TwoTone`：stroke/fill 分别映射。
- `MultiColor`：stroke/fill/secondary stroke/secondary fill 全部保留。

同时，IconPark 依赖：

- `StrokeBrush` / `FillBrush` / `SecondaryStrokeBrush` / `SecondaryFillBrush` / `FallbackBrush`
- `StrokeWidth` / `StrokeLineCap` / `StrokeLineJoin`
- `IconTheme`
- `ProcessBrush()`：当前会把带透明度的 `ImmutableSolidColorBrush` 转成白底不透明色。
- `FindIconBrush()`：IconPark 在派生类里覆盖它实现主题映射。
- Phase 3 后 generator 会为每个 generated icon 写入 `GeneratedViewBox`、`GeneratedGeometryBounds`、`GeneratedZoomMatrix`。

结论：

- 不能把多色、stroke、theme switch、fallback brush 视为无用功能删除。
- 可以优化为按 instruction 需要懒初始化 brush/pen/transition。
- 可以把 `IconTheme` 状态变化成本收敛，但必须保留 IconPark 的运行时主题切换能力。

### Material 图标包兼容性

外部包位置：

- `/Users/chinboy/Projects/dotnet/MaterialIconsPackages`

关键实现：

- `src/AtomUI.Icons.Material/MaterialIcon.cs`
- `src/AtomUI.Icons.Material/MaterialIconProvider.cs`
- `src/AtomUI.Icons.Material.Generator/MaterialIconsPackageGenerator.cs`
- `src/AtomUI.Icons.Material/GeneratedIcons/*.g.cs`

Material 生成方式与 IconPark 不同：

- 每个主题是独立 generated class，例如 `ActionAlarmFilled`、`ActionAlarmTwoTone`。
- generated class 继承 `MaterialIcon`，`MaterialIcon` 继承 `Icon` 并提供 generated geometry metadata fallback。
- 构造函数设置固定 `IconTheme`。
- `Filled` / `Rounded` / `Sharp` 主要生成 `FillBrush = IconBrushType.Fill`。
- `Outlined` 生成 `FillBrush = IconBrushType.Stroke`。
- `TwoTone` 根据 path opacity 区分 `Fill` 与 `Stroke`。
- Phase 3 后 generator 会为每个 generated icon 写入 `GeneratedViewBox`、`GeneratedGeometryBounds`、`GeneratedZoomMatrix`。

Material Gallery 额外说明：

- `IconInfoRepository` 初次只物化 96 个图标，滚动加载更多。
- 但 generated repository 文件非常大，Material 约 10751 个 generated icon classes，IconPark 约 2658 个 generated icon classes。
- Gallery 的大规模列表性能不能只看单个 icon，还要看 repository、item、presenter 和 scroll incremental load。

结论：

- `IconProvider<TIconKind>` 的 enum -> type -> factory 缓存仍然必要。
- 不应缓存 `Icon` Control 实例；Material/IconPark Gallery 都通过 `Creator` 创建新 control。
- Material/IconPark 已同步生成静态 bounds/matrix 等不可变元数据，不能破坏现有 provider、theme 和 generated class 使用模型。

### Provider 与缓存

`IconProvider<TIconKind>` 是 XAML MarkupExtension。每次 XAML 使用会返回一个新的 `Icon` Control 实例。Control 实例不能跨 visual tree 共享，所以这里不能缓存 Icon Control 本身。

当前缓存策略：

- enum kind -> icon type
- icon type -> compiled constructor delegate
- enum kind -> creator

这能避免重复反射查找和表达式编译。后续优化应继续保持这个边界：缓存 factory 和不可变元数据，不缓存 Control 实例。

### Presenter 与模板

当前相关实现：

- `src/AtomUI.Controls/Icon/IconPresenter.cs`
- `src/AtomUI.Controls/Icon/IconTemplatePresenter.cs`
- `src/AtomUI.Controls/Icon/Themes/IconTheme.axaml`
- `src/AtomUI.Controls/Icon/Themes/IconPresenterTheme.axaml`
- `src/AtomUI.Controls/Icon/Themes/PathIconTheme.axaml`

`IconPresenter` 负责把外部传入的 `PathIcon` 接入 logical/visual tree，并 relay bind 宽高、颜色和 motion 开关。

`IconTemplatePresenter` 负责从 `IconTemplate` 构建一个 `PathIcon`，用于按模板表达图标。

## 使用规模观察

本次静态扫描口径：

```bash
rg -o "AntDesignIconProvider" src controlgallery -g '*.axaml' -g '*.cs' | wc -l
rg -o "IconPresenter" src controlgallery -g '*.axaml' -g '*.cs' | wc -l
find src/AtomUI.Icons.AntDesign/GeneratedIcons -name '*.g.cs' | wc -l
```

当前观察结果：

| 项 | 数量 |
| --- | ---: |
| `src` 中 `AntDesignIconProvider` 使用 | 92 |
| Gallery 中 `AntDesignIconProvider` 使用 | 483 |
| `src` 中 `IconPresenter` 命中 | 600 |
| Gallery 中 `IconPresenter` 命中 | 2 |
| AntDesign 生成图标类 | 844 |

这些数量说明 `Icon` 是基础设施级热点。单个图标节省的节点、binding、transition 或 render 分配，都会在高频模板中被放大。

## 性能问题清单

### P0: Icon 自绘后仍有默认模板节点

当前 `Icon.Render()` 已直接绘制背景和 geometry：

- `src/AtomUI.Core/Controls/Icon/Icon.cs`

但 `IconTheme.axaml` 仍给 `Icon` 设置了一个包含 `Border` 的 `ControlTemplate`：

- `src/AtomUI.Controls/Icon/Themes/IconTheme.axaml`

问题：

- 每个 Icon 可能多一个模板视觉节点。
- `Icon` 已自绘背景，默认 `Border` 可能重复承担 Background/Width/Height。
- 高频控件里，这类节点成本会被批量放大。

建议：

- 建立 baseline 后评估空模板或移除模板节点。
- 验证 Background、HitTest、尺寸、Theme 兼容性。
- 如果外部依赖模板树，需要保留兼容路径，不能直接破坏。

### P0: 默认初始化创建完整 pen / transition 成本

当前 `OnInitialized()` 会为 5 类 brush 都创建 `Pen`，并在 motion 开启时创建 5 个 `SolidColorBrushTransition`。

问题：

- 大多数 AntDesign 图标是 fill path，不需要 stroke pen。
- 大多数静态图标不会发生 brush 动画。
- `Secondary*` 和 `Fallback` 对普通单色图标通常不会用到。
- 这使普通静态图标承担了多色、stroke、动画场景的成本。

同时存在一个疑似正确性问题：

- `strokeIndex` 当前被赋值为 `IconBrushType.Fallback`，不是 `IconBrushType.Stroke`。

建议：

- 修正 `strokeIndex`。
- `Pen` 按需创建：只有 instruction 需要 stroke 时才创建。
- `Transitions` 按实际参与动画的 brush 类型创建，或者在首次需要 brush 动画时创建。
- 默认静态图标不创建 secondary/fallback pen。

### P0: render 热路径每帧分配 MatrixTransform

当前 `DrawingInstruction.Draw()` 每次 render 都会：

- 创建新的 `MatrixTransform`。
- 写入 `_geometry.Transform`。
- 绘制后恢复 transform。

问题：

- render 热路径分配对象。
- 静态 `DrawingInstruction[]` 被所有同类型图标共享，写入 geometry transform 是共享可变状态。
- 当前 UI 线程顺序 render 通常不会出现并发问题，但架构上不够干净，也不利于未来缓存。

建议：

- 改为 `DrawingContext.PushTransform(...)` 组合全局 matrix 和 instruction matrix。
- 绘制时不要修改 geometry 本体。
- 对 opacity 也保留 scoped push，不引入持久状态。

### P1: Generated icon bounds 每个实例首次 render 重算

`AntDesignIcon.CalculateGlobalGeometryMatrix()`、外部包专用 icon 基类在没有 generated metadata 时会用实例字段 `_geometryBounds` 缓存 bounds。首次计算会调用 `CalculateGeometryBounds()`。

问题：

- 同一个图标类型的 bounds 是固定的，但当前每个实例都要首次计算。
- `CalculateGeometryBounds()` 对 path geometry 执行 clone，并构建 `GeometryGroup`。
- 在批量创建相同图标时，这个成本重复。

建议：

- bounds 或 zoom matrix 按图标类型缓存。
- 更理想的方式是在 generator 阶段生成静态 bounds 或静态 zoom matrix；Phase 3 已对 AntDesign、Material、IconPark 采用该方式。
- 新增缓存必须只保存不可变值，不保存 Control 实例。

### P1: 隐藏 Icon 被默认创建

典型位置：

- `src/AtomUI.Desktop.Controls/Select/Themes/SelectHandleTheme.axaml`
- `src/AtomUI.Desktop.Controls/Menu/Themes/MenuItemTheme.axaml`
- `src/AtomUI.Desktop.Controls/NavMenu/Themes/*NavMenuItemHeaderTheme.axaml`
- `src/AtomUI.Desktop.Controls/Buttons/Themes/ToggleIconButtonTheme.axaml`

问题：

- `SelectHandle` 默认同时创建 open、loading、search、clear 多个图标或图标按钮，多数状态只显示一个。
- `MenuItem` 每个 item 默认创建 `RightOutlined`，再通过 `:empty` 隐藏。
- `ToggleIconButton` 默认同时创建 checked / unchecked 两套 IconPresenter。
- `IsVisible=false` 只能隐藏显示，不减少实例化、样式、绑定和模板成本。

建议：

- 高收益控件改为单 icon slot，根据状态设置当前 icon。
- loading/search/clear 这类状态图标按需 materialize。
- Menu/NavMenu 的 submenu indicator 只在存在子项时创建。
- 改造必须保持 public API 和视觉行为。

### P1: IconPresenter 绑定生命周期与重复配置

`IconPresenter` 在 `HandleIconChanged()` 中配置 icon，在 `OnAttachedToVisualTree()` 中也会再次配置当前 icon。

问题：

- 同一个 icon 可能经历重复 relay binding 创建与 dispose。
- 单次成本小，但 IconPresenter 使用频率高。

`IconTemplatePresenter` 目前清理子元素时直接 `LogicalChildren.Clear()` / `VisualChildren.Clear()`，没有集中管理绑定 disposable。

建议：

- `IconPresenter` 避免重复配置同一个 icon。
- `IconTemplatePresenter` 补齐 binding disposable 管理。
- 清理时解除 logical/visual parent，避免泄露。

### P2: Provider cache 清理语义可以更清晰

`IconProviderCache` 当前有 `TypeCache`、`CreatorCache`、`TypeToCreator`。`ClearCache(enumType)` 会清 enum 级缓存，但不会清 `TypeToCreator`。

判断：

- 对内置 AntDesign 图标包，这不是典型资源泄露，因为 type 数量有限且随 assembly 生命周期存在。
- 如果未来支持动态 icon package 或热加载插件，`TypeToCreator` 可能长期持有 type 和 delegate。

建议：

- 短期不作为 P0 优化。
- 后续如引入更多 icon package，需明确缓存生命周期和清理策略。
- 不引入以 brush、style、control instance 为 key 的无界缓存。

## Baseline 计划

### 控件级 micro benchmark

工具位置：

- `tools/performances/AtomUI.Performance`

建议新增场景：

| 场景 | 目的 |
| --- | --- |
| `Icon.SearchOutlined.Direct` | 单个 AntDesign Icon 创建、模板、首次 render |
| `Icon.SearchOutlined.Presenter` | `IconPresenter + Icon` 的真实包装成本 |
| `Icon.SearchOutlined.Many` | 批量静态图标成本 |
| `Icon.LoadingOutlined.Spin` | 动画图标成本 |
| `Icon.TwoTone` | 双色图标 brush/selector 成本 |
| `Icon.HiddenSlots.SelectHandle` | SelectHandle 默认隐藏图标成本 |
| `Icon.MenuItem.Indicator` | MenuItem submenu indicator 默认创建成本 |

建议指标：

- 创建 N 个控件并 ApplyTemplate 的耗时。
- 首次 render 耗时。
- 稳定 render 耗时。
- visual descendants 数。
- logical descendants 数。
- allocations / KB per item。
- transition count。
- binding/subscription count，如果工具能观测。

### Gallery 真实场景

工具位置：

- `tools/performances/AtomUI.GalleryPerformance`

严格要求：

- 必须加载 Gallery 真实 route。
- 必须加载真实 ShowCase XAML。
- 必须验证运行时控件数量与源 XAML 一致。
- 不用 synthetic 页面替代真实 Gallery 场景。

建议优先场景：

| 场景 | 原因 |
| --- | --- |
| `General/IconShowCase` | 图标数量最多，适合看 Icon 自身批量成本 |
| `Navigation/DropdownButtonShowCase` | MenuItem / submenu indicator 密集 |
| `Navigation/ComboBoxShowCase` | SelectHandle / Input accessory / dropdown 组合 |
| `DataEntry/LineEditShowCase` | 已有历史数据，可观察 Icon 优化叠加收益 |
| `General/ButtonShowCase` | Button/IconButton/LoadingIcon 高频 |
| `DataDisplay/TreeViewShowCase` | switcher/loading/leaf icon 密集 |

## 优化目标

### 必须保持

- 保留现有 `Icon` / `PathIcon` / `IconProvider` public API。
- 保留 AntDesign 图标包的 XAML 使用方式。
- 保留 IconPark 图标包通过单个图标类动态切换 `IconTheme` 的能力。
- 保留 Material 图标包按主题生成独立图标类的能力。
- 保留单色、双色、多色、stroke、fill、loading animation 行为。
- 保留现有主题 token 对颜色、尺寸、stroke 的控制。
- 保留控件允许外部传入 `PathIcon` 的能力。

### 希望降低

- 静态单色图标的初始化成本。
- 每个图标默认 visual/logical 节点数量。
- 每个图标默认 transition / pen / binding 成本。
- 首次 render 中 geometry bounds 重算成本。
- 稳定 render 中临时对象分配。
- 高频控件模板中的隐藏图标数量。

### 不在本阶段处理

- 替换 AntDesign 图标资源。
- 大范围视觉重设计。
- Gallery 页面懒加载。
- Avalonia 框架源码 patch。
- 缓存或复用 Icon Control 实例。

## 资源泄露约束

后续所有实现必须遵守：

- 不缓存 `Icon`、`PathIcon`、`IconPresenter` 等 Control 实例。
- 不缓存含有 visual/logical parent 的对象。
- 新增缓存只允许保存不可变 geometry、bounds、matrix、factory、枚举映射等元数据。
- 所有事件订阅、binding disposable、animation style 必须在 detached/unloaded 或内容替换时释放。
- 按需创建的图标在替换或卸载时必须解除 logical parent 和 visual parent。
- 新增缓存必须有明确边界；不能按 brush 实例、style 实例、templated parent 实例做无界缓存。

## 分阶段方案

### Phase 0: Baseline 与观测

- [x] 在 `tools/performances/AtomUI.Performance` 增加 Icon micro benchmark。
- [x] 记录 direct Icon、IconPresenter、animated Icon、TwoTone Icon 的 baseline。
- [x] 记录 SelectHandle/MenuItem 等隐藏图标模板成本。
- [x] 在 `tools/performances/AtomUI.GalleryPerformance` 增加真实 `--showcase icon` route。
- [x] 记录 IconShowCase baseline。
- [x] 输出 `docs/performances/Icon/icon-baseline.md`。
- [ ] 记录 DropdownButtonShowCase、ComboBoxShowCase、LineEditShowCase 的 Icon 叠加 baseline。
- [x] 阅读 IconParkIconsPackage 与 MaterialIconsPackages，补齐兼容性约束。

### Phase 1: Icon 低风险修复

- [x] 修正 `Icon.OnInitialized()` 中 `strokeIndex` 错误。
- [x] 将默认 pen 初始化改为按需创建或按实际 instruction 创建。
- [x] 将 transition 初始化限制到实际需要动画的 brush。
- [x] 避免 `IconPresenter` attach 阶段重复配置同一个 icon。
- [x] 为 `IconTemplatePresenter` 增加 disposable 清理与 parent 解除逻辑。
- [x] 验证无订阅泄露、无 visual parent 残留。

### Phase 2: Render 热路径收敛

- [x] 将 `DrawingInstruction.Draw()` 改为 `PushTransform`，避免每帧创建 `MatrixTransform`。
- [x] 绘制时不再写入共享 geometry 的 `Transform`。
- [x] 复测首次 render 与稳定 render allocations。
- [x] 验证 transform、opacity、TwoTone、多 path 图标视觉一致。

### Phase 3: AntDesign/Material/IconPark 静态元数据缓存

- [x] 评估按图标类型缓存 geometry bounds。
- [x] generator 直接生成静态 bounds 和 zoom matrix。
- [x] 确认新增缓存不会持有 Control 实例。
- [x] 复测批量同类图标创建和首次 render。
- [x] 同步 `MaterialIconsPackages` 与 `IconParkIconsPackage` 的 generator 和基类 fallback。
- [x] 重新生成外部包 icon class 并完成 solution build 验证。

### Phase 4: 高频模板按需创建

- [x] SelectHandle：open/loading/search/clear 按可见状态 materialize。
- [x] MenuItem：submenu indicator 只在有子项时创建。
- [x] NavMenu：submenu indicator 按实际数据创建；item icon presenter 保持现状。
- [x] ToggleIconButton：单 presenter 切换 icon，避免双 presenter 常驻。
- [x] Button / IconButton：Button loading icon 按需创建；IconButton 暂未发现常驻 hidden loading slot。
- [x] 每个模板改造增加专项验证；Gallery 已复测 LineEditShowCase / IconShowCase / ButtonShowCase / SelectShowCase / MenuShowCase。

### Phase 5: Provider / Generator 深化优化

- [x] 只有在 baseline 证明 Provider 是瓶颈时，再考虑生成 enum -> factory switch；当前 provider micro `0.065ms/item`，不是主导瓶颈，暂不实施。
- [x] 明确 `IconProviderCache.ClearCache()` 与 `TypeToCreator` 的清理语义。
- [x] 评估生成代码移除不必要 using 和运行时解析路径；generated transform 已改为生成期 `Matrix` literal。
- [x] 保持 XAML API 兼容。

### Phase 6: 最终验证与文档

- [x] 对比优化前后 micro benchmark。
- [x] 对比优化前后 Gallery 真实场景。
- [x] 补充 `docs/performances/Icon/icon-final.md`。
- [x] 更新 `docs/performances/README.md` 的关键结果。
- [x] 列出未完成风险和后续任务。

## 预期收益判断

Icon 优化的收益更可能体现在批量场景，而不是单个图标的肉眼响应：

- 对 `IconShowCase`、菜单、导航、TreeView、Select/ComboBox 这类图标密集页面，预计收益更明显。
- 对 `LineEditShowCase`，Icon 优化会叠加在 AddOnDecoratedBox/LineEdit 优化之后，单独百分比可能低于之前右侧 accessory 按需创建，但 visual count 和 allocations 应继续下降。
- 对普通业务页面，收益取决于图标密度和是否存在隐藏图标模板。

是否继续进入实现，应以 Phase 0 baseline 为准。
