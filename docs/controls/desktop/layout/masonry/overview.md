# Masonry 桌面版架构设计

本文档定义 AtomUI 桌面版 Masonry 的最新架构设计、布局模型、API 契约、主题边界和验证策略。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，设计和契约变化记录见 [Masonry Changelog](changelog.md)。

## 1. 控件定位

Masonry 是面向桌面应用的瀑布流布局能力，用于将高度不一致的内容块按列组织，并通过 shortest-column 策略降低列高差。它适合图片墙、卡片流、示例集合、资源列表等内容高度不可预先统一的场景。

AtomUI 的 Masonry 以 Avalonia 原生布局容器为核心，公开布局类型命名为 `MasonryPanel`。`MasonryPanel` 是布局原语，不是数据源管理器、卡片控件、图片加载器、虚拟化列表、滚动容器或内容渲染器。它只负责对子元素进行测量、列分配和排列，不拥有子元素内容，也不改变子元素的业务语义。

数据绑定场景应通过 Avalonia `ItemsControl.ItemsPanel` 组合 Masonry，Masonry 自身不承担数据源管理、内容渲染或项模板职责。滚动由外层 `ScrollViewer` 承担，图片加载由图片控件或外部加载库承担。

## 2. 设计语言

Masonry 表达的是“密集但有秩序”的内容浏览体验。它利用可变高度卡片形成自然节奏，避免为了对齐网格而牺牲内容本身的信息密度。

设计语言要求：

- 视觉上按列平衡内容高度，降低单列过长造成的空白和浏览断层。
- 逻辑上保留子元素顺序，保证读屏、键盘导航、数据绑定容器生成顺序和源集合顺序一致。
- 不为子项强加卡片外观。子项可以是 Card、Image、ShowCaseItem 或任意 `Control`。
- 不插入额外视觉包装层，使 VisualTree 保持最小，避免瀑布流项数量增加时产生不必要的布局和渲染成本。
- 间距只表达布局节奏，不表达组件语义颜色、阴影或卡片样式。

## 3. 架构分层

Masonry 架构按布局职责分层，避免把数据、视觉和布局计算耦合到同一层。

| 层 | 责任 | 主要载体 |
| --- | --- | --- |
| Public API | 暴露列数、自适应宽度、间距、刷新策略和子项布局元数据。 | `MasonryPanel`、`Masonry` attached properties |
| Effective State | 将公开属性归一为有效列数、有效列宽、有效行列间距和子项布局指令。 | `MeasureOverride` / `ArrangeOverride` 内部状态 |
| Layout Engine | 测量子元素高度，按 shortest-column 或显式列规则计算排列矩形。 | Masonry 布局计算逻辑 |
| Integration | 支持直接放置子元素，也支持作为 `ItemsControl.ItemsPanel` 使用。 | Avalonia Panel / ItemsControl |
| Theme | 提供可选默认间距，不负责绘制子项外观。 | ControlTheme 或 style setter |
| Token | 当前不定义 Masonry 专属 Token。 | SharedToken / 用户属性 |

状态流：

```text
Public API
  ColumnCount / MinColumnWidth / MaxColumnCount
  ColumnGap / RowGap
  IsFreshLayoutEnabled
  Masonry.Column / Masonry.Span
        ↓
Effective State
  EffectiveColumnCount
  EffectiveColumnWidth
  EffectiveColumnGap / EffectiveRowGap
  EffectiveItemColumn / EffectiveItemSpan
        ↓
Layout Engine
  measure child with fixed column width
  place child into explicit column or shortest column
  calculate panel desired height
        ↓
Arrange
  arrange child at stable Rect
```

`ShowCaseMasonryPanel` 是 Gallery 内部辅助布局，已有 shortest-column、`MinItemWidth`、`MaxColumns`、行列间距和整行占用能力。它可以作为 Masonry 设计的本地参考，但不构成公共 API 契约。

## 4. API 设计

Masonry 的公共 API 应采用 Avalonia 原生属性模型。凡是需要 XAML 设置、Style 设置、绑定、动画或主题参与的状态，都必须定义为 `StyledProperty` 或 attached property，不使用普通 CLR 字段承载公共契约。

核心布局属性：

```csharp
public class MasonryPanel : Panel
{
    public static readonly StyledProperty<int> ColumnCountProperty;
    public static readonly StyledProperty<double> MinColumnWidthProperty;
    public static readonly StyledProperty<int> MaxColumnCountProperty;
    public static readonly StyledProperty<double> ColumnGapProperty;
    public static readonly StyledProperty<double> RowGapProperty;
    public static readonly StyledProperty<bool> IsFreshLayoutEnabledProperty;

    public int ColumnCount { get; set; }
    public double MinColumnWidth { get; set; }
    public int MaxColumnCount { get; set; }
    public double ColumnGap { get; set; }
    public double RowGap { get; set; }
    public bool IsFreshLayoutEnabled { get; set; }
}
```

`ColumnCount` 表达固定列数，默认值为 `0`。当 `ColumnCount > 0` 时，Masonry 使用固定列数；当 `ColumnCount <= 0` 时，Masonry 通过 `MinColumnWidth`、`MaxColumnCount` 和可用宽度计算有效列数。该模型同时覆盖固定列数和容器宽度自适应两种场景。

`MinColumnWidth` 表达自适应布局中单列最小目标宽度，默认值应来自主题或 shared spacing 规则，但它本身是实例属性。`MaxColumnCount` 限制自适应列数上限，防止宽屏下列数失控。`ColumnGap` 与 `RowGap` 分别表达列间距和行间距，负数、`NaN` 和无穷值在 effective state 中归一为有效非负值。

子项元数据通过 attached property 表达：

```csharp
public static class Masonry
{
    public static readonly AttachedProperty<int?> ColumnProperty;
    public static readonly AttachedProperty<MasonryItemSpan> SpanProperty;
}

public enum MasonryItemSpan
{
    Auto,
    Full
}
```

`Masonry.Column` 用于指定子项所在列，列索引按 `0` 起始，超出范围时夹取到 `[0, EffectiveColumnCount - 1]`。未指定列时使用 shortest-column。`Masonry.Span=Full` 表示子项占据整行，排列位置从当前最高列之后开始，并将所有列高度推进到该子项底部。

事件模型：

```csharp
public event EventHandler<MasonryLayoutChangedEventArgs>? LayoutChanged;
```

`LayoutChanged` 只在子项有效列分配或有效排列信息变化时触发，不应替代 Avalonia 已有 layout lifecycle。事件数据应以子元素引用、子元素索引、有效列和排列矩形为主，不要求用户提供 key。作为 `ItemsControl.ItemsPanel` 使用时，数据项标识由 ItemsControl 容器和用户 ViewModel 维护。

## 5. 行为交互模型

Masonry 本身没有 hover、pressed、disabled、loading 或 checked 等交互状态。它必须完整保留子元素自身的命中测试、焦点、键盘导航、拖拽、上下文菜单和动画行为。

布局失效触发条件：

- 子元素集合变化。
- 子元素 `IsVisible` 变化。
- 子元素 DesiredSize 因图片加载、文本换行、异步内容或数据更新发生变化。
- `ColumnCount`、`MinColumnWidth`、`MaxColumnCount`、`ColumnGap`、`RowGap` 变化。
- `Masonry.Column` 或 `Masonry.Span` attached property 变化。
- Panel 可用宽度变化。
- 启用 `IsFreshLayoutEnabled` 时，已排列子元素尺寸变化。

动态内容行为：

- 默认测量发生在 Avalonia layout pass 中，子元素尺寸变化通过正常的 `InvalidateMeasure` 进入下一次布局。
- `IsFreshLayoutEnabled=true` 表示 Masonry 对已生成子元素尺寸变化更敏感，应在子元素实际尺寸变化时重新触发布局。实现必须遵守 Avalonia layout lifecycle，不能通过额外视觉 wrapper 监听尺寸。
- 图片加载、网络资源完成和内容异步替换不由 Masonry 发起。Masonry 只响应子元素尺寸变化。

## 6. 状态模型

Masonry 的状态模型应在 C# 布局层完成归一，AXAML 不承担列数、间距或子项位置计算。

有效列数计算：

```text
if ColumnCount > 0:
  EffectiveColumnCount = ColumnCount
else:
  EffectiveColumnCount = floor((AvailableWidth + ColumnGap) / (MinColumnWidth + ColumnGap))
  EffectiveColumnCount = clamp(EffectiveColumnCount, 1, MaxColumnCount)
```

当可用宽度为无穷时，Masonry 使用 `MinColumnWidth * MaxColumnCount + ColumnGap * (MaxColumnCount - 1)` 作为内部测量宽度。该规则保证控件在未被外层约束宽度时仍能得到确定布局。

有效列宽计算：

```text
EffectiveColumnWidth =
  (AvailableWidth - ColumnGap * (EffectiveColumnCount - 1)) / EffectiveColumnCount
```

布局过程：

1. 按 `Children` 顺序处理子元素。
2. 不可见子元素保留默认排列矩形，不参与列高计算。
3. 普通子元素以 `EffectiveColumnWidth`、无限高度进行测量。
4. `Masonry.Span=Full` 子元素以完整可用宽度、无限高度进行测量。
5. 指定 `Masonry.Column` 的子元素放入有效列；未指定列的子元素放入当前最短列。
6. 子元素的 `Y` 等于目标列当前高度，非首项追加 `RowGap`。
7. 整行子元素的 `Y` 等于当前最高列高度，非首行追加 `RowGap`，排列后所有列高度同步到该子元素底部。
8. Panel desired height 等于所有列高度最大值。

Tie-break 规则必须稳定。多个列高度相同时，选择索引最小的列，保证相同输入得到相同布局结果。

## 7. 模板与视觉架构

`MasonryPanel` 是 `Panel`，默认不需要 `ControlTemplate`，也不定义 template part。它不应为每个子项插入 `ContentPresenter`、`Border` 或其他视觉包装层。作为 `ItemsControl.ItemsPanel` 使用时，由 ItemsControl 的容器生成机制负责容器层级，Masonry 不额外增加层级。

视觉树要求：

- Masonry 节点只承担布局职责。
- 子项视觉结构完全由子项控件负责。
- 不通过模板节点实现列、行、占位或测量辅助对象。
- 不通过透明 Border 扩展命中区域。
- 不通过不可见控件缓存测量结果。

如果主题提供默认样式，只能设置布局属性默认值或外层布局相关属性，不应绘制背景、边框、阴影或卡片状态。Masonry 的设计目标是在保证可维护性的前提下保持最少 VisualTree 层级。

## 8. Theme 架构

Masonry 当前不定义专属 `token.md`。Masonry 不需要创建组件级颜色、阴影或卡片外观 Token。

Theme 可以为 `MasonryPanel` 提供默认 `ColumnGap`、`RowGap`、`MinColumnWidth` 或 `MaxColumnCount`，这些默认值应来源于 SharedToken 或布局规范。Theme 不应把实例状态、动态列数或子项位置写入 Token。

允许的 Theme 职责：

- 设置默认行列间距。
- 设置默认最小列宽和最大列数。
- 支持暗色/亮色主题下相同的布局语义。

不允许的 Theme 职责：

- 为子项添加卡片背景、圆角、边框或阴影。
- 定义 Masonry 私有颜色表。
- 通过 selector 计算列数或位置。
- 依赖子项类型实现布局规则。

## 9. 控件家族或集成关系

Masonry 属于 Layout 分类，与 `FlexPanel`、`Row`、`Space` 等布局能力共同构成 AtomUI 的桌面布局基础。

集成关系：

- 直接子元素场景：用户在 `MasonryPanel.Children` 中放置任意控件，Masonry 负责布局。
- 数据绑定场景：用户将 `MasonryPanel` 放入 `ItemsControl.ItemsPanel`，由 ItemsControl 负责 item container、数据模板和集合变更。
- Gallery 场景：Gallery 当前使用 `ShowCaseMasonryPanel` 承载示例卡片，该类是 Gallery 内部实现，不是 AtomUI 控件包的公共契约。
- 图片场景：Masonry 可承载图片控件，但不负责图片解码、缓存、占位、错误图或网络加载。
- 滚动场景：Masonry 放在 `ScrollViewer` 内使用，不改变 ScrollViewer 的滚动语义。

Masonry 不提供虚拟化语义。瀑布流虚拟化涉及滚动偏移、容器回收、动态高度预测和键盘导航补偿，不能在 MasonryPanel 中隐式开启。

## 10. 兼容性不变量

优化或扩展 Masonry 时必须保持以下不变量：

- 不改变子元素 logical order、visual child order 和 ItemsControl 容器生成顺序。
- 不修改子元素 `DataContext`、内容、样式类、主题或资源作用域。
- 不为子项插入额外视觉包装层。
- 不要求用户为子项提供 key。
- 不把 Masonry 变成 ScrollViewer 或数据控件。
- 不隐式引入虚拟化、延迟生成或容器回收语义。
- 不把卡片外观、图片加载或业务状态写入 Masonry。
- 不通过 AXAML selector 承担列数和位置计算。
- 不改变 attached property 的含义、默认值和生效优先级。
- 不将实例状态迁移为控件 Token。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

## 11. 专项模型

### 11.1 AtomUI 布局算法

AtomUI Masonry 的布局算法必须由 Avalonia `MeasureOverride` 与 `ArrangeOverride` 驱动：

```text
MeasureOverride(availableSize)
  resolve width
  resolve effective columns and gaps
  for each visible child in order:
    resolve span and explicit column
    measure child with column width or full width
    compute rect using shortest-column state
  cache rects for arrange
  return Size(resolvedWidth, maxColumnHeight)

ArrangeOverride(finalSize)
  recompute layout if final width changed
  arrange each child using cached rect
  return finalSize
```

Measure 与 Arrange 的布局结果必须对同一输入保持一致。实现可以缓存矩形以减少重复计算，但缓存不能跨越影响布局的属性变更、子元素变更或可用宽度变更。

### 11.2 响应式模型

AtomUI 桌面版以容器可用宽度计算列数，符合 Avalonia 桌面布局模型。

桌面响应式模型由两层组成：

- 固定列数：`ColumnCount > 0` 时使用明确列数。
- 容器自适应：`ColumnCount <= 0` 时使用 `MinColumnWidth + MaxColumnCount + AvailableWidth` 计算列数。

当需要按全局媒体断点改变布局属性时，应通过 AtomUI 现有 media break 能力或外层样式设置 `ColumnCount`、`MinColumnWidth`、`MaxColumnCount`、`ColumnGap` 和 `RowGap`，Masonry 自身仍只消费最终属性值。

### 11.3 整行项模型

`Masonry.Span=Full` 是 AtomUI Masonry 的子项布局元数据，用于标题、说明块、分组分隔、宽幅图或需要打断瀑布流节奏的内容。整行项不改变前后子项的 logical order。

排列规则：

- 整行项从当前最高列之后开始。
- 非首行整行项前追加 `RowGap`。
- 整行项宽度为 Masonry 有效宽度。
- 整行项排列后，所有列高度同步到整行项底部。

整行项语义完全通过 `Masonry.Span` attached property 表达，不依赖具体子项类型。

## 12. 验证策略

Masonry 改动应按文档、C# 布局、AXAML/Theme、Public API 和 Gallery 分层验证。

文档验证：

```bash
git diff --check
```

C# 布局验证：

- 固定列数下按 shortest-column 放置子项。
- 自适应列数遵守 `MinColumnWidth`、`MaxColumnCount` 和可用宽度。
- `ColumnGap`、`RowGap` 影响坐标和 desired height。
- 多列等高时选择索引最小列。
- 不可见子元素不参与列高计算。
- `Masonry.Column` 指定列并正确夹取越界值。
- `Masonry.Span=Full` 从最高列之后开始并同步所有列高度。
- 无限可用宽度下仍能得到确定 desired size。
- 子元素尺寸变化、attached property 变化和布局属性变化会触发重新测量。

AXAML/Theme 验证：

- 默认 Theme 不增加子项视觉包装层。
- Theme 只设置布局默认值，不绘制子项外观。
- SharedToken 变更后默认间距能随主题更新。

Public API 验证：

- 所有公共布局属性均为 `StyledProperty`。
- 子项布局元数据均为 attached property。
- `LayoutChanged` 事件不要求业务 key，不改变 Avalonia layout lifecycle。
- API 默认值、单位和无效值归一规则稳定。

Gallery 验证：

- 作为 `ItemsControl.ItemsPanel` 使用时，容器顺序和键盘导航顺序保持源集合顺序。
- 图片、异步内容和高度变化场景下布局能重新稳定。
- 示例卡片、API 表和 Token 表等可变高度内容不出现重叠、裁剪或异常空白。
