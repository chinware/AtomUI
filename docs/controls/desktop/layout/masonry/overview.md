# Masonry 桌面版架构设计

本文档定义 AtomUI 桌面版 Masonry 的最新架构设计、布局模型、API 契约、主题边界和验证策略。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，设计和契约变化记录见 [Masonry Changelog](changelog.md)。

## 1. 控件定位

Masonry 是面向桌面应用的瀑布流布局控件，用于将高度不一致的内容块按列组织，并通过 shortest-column 策略降低列高差。它适合图片墙、卡片流、示例集合、资源列表等内容高度不可预先统一的场景。

AtomUI 的 Masonry 公开类型为 `Masonry`，派生自 Avalonia `ItemsControl`，位于 `AtomUI.Desktop.Controls` 命名空间，对应 `atom:` 命名空间前缀。`Masonry` 同时承担数据绑定入口和子项布局元数据 attached property 容器（`atom:Masonry.Column`、`atom:Masonry.Span`）两种职责。

瀑布流布局计算由 internal 的 `MasonryPanel`（派生自 `Panel`）承担。`MasonryPanel` 是默认布局引擎，不暴露给开发者，仅作为 `Masonry` 的默认 `ItemsPanel` 由 ControlTheme 装配。开发者通常只通过 `Masonry` 使用瀑布流能力；如果显式替换继承自 `ItemsControl` 的 `ItemsPanel`，即表示替换 Masonry 的布局引擎，`MasonryPanel` 提供的 shortest-column、整行项和 `LayoutChanged` 布局结果语义不再适用。

`Masonry` 是布局控件。它支持 `ItemsControl` 的数据绑定入口，但不是数据源管理器、卡片控件、图片加载器、虚拟化列表、滚动容器或内容渲染器。它只负责对子元素进行测量、列分配和排列，不拥有子元素内容，也不改变子元素的业务语义。

`Masonry` 支持两种内容提供方式：

- 直接放置子元素：在 `atom:Masonry` 的内容中放置任意 `Control`，子元素本身即作为容器，不产生额外视觉包装层。
- 数据绑定：通过 `ItemsSource` 绑定数据集合，由 `ItemsControl` 固有机制生成容器。集合监听、容器生成、增删同步由基类承担。

滚动由外层 `ScrollViewer` 承担，图片加载由图片控件或外部加载库承担。

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
| Public API | 暴露列数、自适应宽度、间距、刷新策略、子项布局元数据和 `ItemsSource` 数据绑定入口。 | `Masonry`、`Masonry.Column` / `Masonry.Span` attached property |
| Effective State | 将公开属性归一为有效列数、有效列宽、有效行列间距和子项布局指令。 | `MasonryPanel.MeasureOverride` / `ArrangeOverride` 内部状态 |
| Layout Engine | 测量子元素高度，按 shortest-column 或显式列规则计算排列矩形。 | `MasonryPanel`（internal）布局计算逻辑 |
| Integration | 支持直接放置子元素，也支持通过 `ItemsSource` 绑定数据集合。两种方式由同一 `Masonry` 承载。 | `Masonry : ItemsControl` + internal `MasonryPanel : Panel` |
| Theme | 装配 `ItemsPresenter` 与默认 `MasonryPanel`，尊重继承的 `ItemsPanel` 契约，提供默认布局属性，不负责绘制子项外观。 | `MasonryTheme.axaml` |
| Token | 当前不定义 Masonry 专属 Token。 | SharedToken / 用户属性 |

状态流：

```text
Public API
  ColumnCount / MinColumnWidth / MaxColumnCount
  ColumnGap / RowGap
  Masonry.Column / Masonry.Span
  ItemsSource / ItemTemplate
        ↓
ItemsControl 容器生成
  直接放置子元素 → 子元素本身即容器
  ItemsSource 绑定   → 基类生成 ContentPresenter 容器
        ↓
MasonryPanel (internal)
  EffectiveColumnCount / EffectiveColumnWidth
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

Masonry 布局算法以 Avalonia 原生 `Panel` 测量/排列机制为基础。子元素尺寸变化通过 Avalonia layout lifecycle 自动触发重新测量，不需要额外监听机制。

## 4. API 设计

Masonry 的公共 API 应采用 Avalonia 原生属性模型。凡是需要 XAML 设置、Style 设置、绑定、动画或主题参与的状态，都必须定义为 `StyledProperty` 或 attached property，不使用普通 CLR 字段承载公共契约。

公开控件类型为 `Masonry`，派生自 Avalonia `ItemsControl`，位于 `AtomUI.Desktop.Controls` 命名空间，对应 `atom:` 命名空间前缀。布局引擎 `MasonryPanel` 派生自 `Panel`，标记 `internal`，仅作为 `Masonry` 的默认 `ItemsPanel` 由 ControlTheme 装配，不暴露给开发者。

核心布局属性定义在 `Masonry` 上：

```csharp
public class Masonry : ItemsControl
{
    public static readonly StyledProperty<int> ColumnCountProperty;
    public static readonly StyledProperty<ResponsiveInt?> ColumnInfoProperty;
    public static readonly StyledProperty<double> MinColumnWidthProperty;
    public static readonly StyledProperty<int> MaxColumnCountProperty;
    public static readonly StyledProperty<double> ColumnGapProperty;
    public static readonly StyledProperty<double> RowGapProperty;
    public static readonly StyledProperty<ResponsiveGutter?> GutterProperty;

    public int ColumnCount { get; set; }
    public ResponsiveInt? ColumnInfo { get; set; }
    public double MinColumnWidth { get; set; }
    public int MaxColumnCount { get; set; }
    public double ColumnGap { get; set; }
    public double RowGap { get; set; }
    public ResponsiveGutter? Gutter { get; set; }
}
```

`ColumnCount` 表达固定列数，默认值为 `0`。当 `ColumnCount > 0` 时，Masonry 使用固定列数；当 `ColumnCount <= 0` 时，Masonry 通过 `MinColumnWidth`、`MaxColumnCount` 和可用宽度计算有效列数。该模型同时覆盖固定列数和容器宽度自适应两种场景。

`ColumnInfo` 表达按媒体断点变化的列数，类型为共享响应式值 `ResponsiveInt?`。它对齐 Ant Design Masonry `columns` 语义，接受固定整数或 partial breakpoint map。`ColumnInfo` 只在当前断点命中显式配置时覆盖 `ColumnCount`；未命中时不改变既有固定列数或容器自适应列数规则。

`Gutter` 表达按媒体断点变化的水平/垂直间距，类型为共享响应式值 `ResponsiveGutter?`。它对齐 Ant Design Masonry `gutter` 语义，支持固定间距、水平/垂直 pair、responsive map 和水平/垂直 responsive pair。`Gutter` 只在当前断点命中显式配置时覆盖 `ColumnGap` 与 `RowGap`；未命中时回退到兼容属性。

布局属性默认值和归一规则：

| 属性 | 默认值 | 归一规则 |
| --- | --- | --- |
| `ColumnCount` | `0` | `> 0` 使用固定列数；`<= 0` 进入自适应列数计算。 |
| `ColumnInfo` | `null` | 当前断点命中显式配置时使用响应式列数；未命中时回退到 `ColumnCount` 或容器自适应列数。 |
| `MinColumnWidth` | `320d` | `NaN`、无穷值回退为 `1d`，最终有效值不小于 `1d`。 |
| `MaxColumnCount` | `4` | 自适应模式下有效值不小于 `1`。 |
| `ColumnGap` | `16d` | 负数、`NaN`、无穷值归一为 `0d`。 |
| `RowGap` | `16d` | 负数、`NaN`、无穷值归一为 `0d`。 |
| `Gutter` | `null` | 当前断点命中显式配置时使用响应式水平/垂直间距；未命中时回退到 `ColumnGap` / `RowGap`。 |

这些值是 `Masonry` 公共属性的当前契约。Theme 可以通过样式覆盖实例默认表现，但不能改变属性语义、单位或无效值归一规则。

响应式属性遵循 [AtomUI 响应式机制设计](../../../../modules/controls-shared/responsive-system.md)。Masonry 不复制断点定义、解析顺序或 partial map 继承规则；它只在 effective state 层提供列数和间距的控件级 fallback。

列数优先级：

1. `ColumnInfo` 在当前断点命中显式配置时，使用响应式列数。
2. `ColumnInfo` 未命中且 `ColumnCount > 0` 时，使用固定列数。
3. 前两者均未命中时，使用 `MinColumnWidth + MaxColumnCount + AvailableWidth` 的容器自适应列数。

间距优先级：

1. `Gutter` 在当前断点命中显式配置时，使用响应式水平/垂直间距。
2. `Gutter` 未命中时，使用 `ColumnGap` 与 `RowGap`。

`Gutter` 的水平和垂直维度独立解析。`Gutter="xs: 8, md: 16"` 只声明水平间距，垂直间距必须继续回退到 `RowGap`；如果需要同时声明水平和垂直响应式间距，应使用分号分隔：

```xml
<atom:Masonry Gutter="16" />
<atom:Masonry ColumnInfo="xs: 1, sm: 2, lg: 3, xxl: 4"
              Gutter="xs: 8, md: 16; xs: 8, md: 20" />
```

`Gutter="16"` 属于固定间距配置，水平和垂直两个维度均使用 `16`。

子项布局元数据通过定义在 `Masonry` 上的 attached property 表达，`Masonry` 同时承担控件和 attached property 容器两种职责：

```csharp
public class Masonry : ItemsControl
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

attached property 的消费对象是 `MasonryPanel.Children` 中被测量和排列的 item container：

- 直接放置子元素时，子元素本身就是 item container，attached property 写在该子元素上。
- `ItemsSource` 数据绑定时，item container 是基类生成的 `ContentPresenter`。attached property 必须写到生成容器上，例如通过 `ItemContainerTheme` 绑定数据项字段；写在 `ItemTemplate` 根元素上不会被 `MasonryPanel` 当作布局元数据读取。

`Masonry.Column` 与 `Masonry.Span` 是布局影响属性。它们的值变化必须使父级 `MasonryPanel` 重新测量，不能只更新视觉状态。

XAML 用法：

```xml
<!-- 直接放置子元素：子元素本身即容器，无额外视觉层 -->
<atom:Masonry ColumnCount="3" ColumnGap="16">
    <Card atom:Masonry.Column="0"/>
    <Image atom:Masonry.Span="Full"/>
</atom:Masonry>

<!-- 数据绑定：ItemsSource + ItemContainerTheme + ItemTemplate -->
<atom:Masonry ItemsSource="{Binding Photos}" ColumnCount="3">
    <atom:Masonry.ItemContainerTheme>
        <ControlTheme TargetType="ContentPresenter">
            <Setter Property="atom:Masonry.Column" Value="{Binding Column}" />
            <Setter Property="atom:Masonry.Span" Value="{Binding Span}" />
        </ControlTheme>
    </atom:Masonry.ItemContainerTheme>

    <atom:Masonry.ItemTemplate>
        <DataTemplate>
            <Image Source="{Binding Url}"/>
        </DataTemplate>
    </atom:Masonry.ItemTemplate>
</atom:Masonry>
```

容器生成由 `ItemsControl` 基类承担：直接放置的 `Control` 子元素本身即为容器，不产生额外层级；`ItemsSource` 绑定的数据对象由基类自动生成 `ContentPresenter` 容器。`Masonry` 不 override 任何容器生成方法，集合监听、容器生成、增删同步、容器回收全部由基类提供。

`Masonry` 继承 `ItemsControl.ItemsPanel`。默认 ControlTheme 应以 internal `MasonryPanel` 作为默认 ItemsPanel，并把 `Masonry` 的布局属性传递给该面板。用户显式替换 `ItemsPanel` 时，替换后的面板接管布局；Masonry-specific 的 `Masonry.Column`、`Masonry.Span` 和 `LayoutChanged` 布局结果语义只对默认 `MasonryPanel` 布局引擎成立。

事件模型：

```csharp
public event EventHandler<MasonryLayoutChangedEventArgs>? LayoutChanged;
```

`LayoutChanged` 是布局结果通知，不是像素级滚动或动画事件。事件契约：

- 只在最终 Arrange 结果对应的有效列分配或整行状态变化后触发。
- 同一布局周期内应合并为至多一次通知，触发时机必须避开 Avalonia layout pass 内部，防止用户 handler 修改属性引发布局重入。
- item 数量、顺序、有效列、`IsFullSpan` 任一变化时触发。
- 只发生坐标、宽度或高度像素变化，但有效列和整行状态不变时不触发。
- 从非空布局变为空布局时，应派发一次空 `Items` 通知；持续为空时不重复派发。
- `MasonryItemLayout.Element` 指向 `MasonryPanel` 实际排列的 item container：直接子元素场景为该子元素，`ItemsSource` 场景为生成的 `ContentPresenter`。
- 事件数据以 item container 引用、容器顺序索引、有效列和整行状态为主，不要求用户提供 key。数据项标识由 `ItemsControl` 容器和用户 ViewModel 维护。

## 5. 行为交互模型

Masonry 本身没有 hover、pressed、disabled、loading 或 checked 等交互状态。它必须完整保留子元素自身的命中测试、焦点、键盘导航、拖拽、上下文菜单和动画行为。

布局失效触发条件：

- 子元素集合变化（直接放置的 `Control` 或 `ItemsSource` 集合变更）。
- 子元素 `IsVisible` 变化。
- 子元素 DesiredSize 因图片加载、文本换行、异步内容或数据更新发生变化。
- `ColumnCount`、`ColumnInfo`、`MinColumnWidth`、`MaxColumnCount`、`ColumnGap`、`RowGap`、`Gutter` 变化。
- `Masonry.Column` 或 `Masonry.Span` attached property 变化。
- Panel 可用宽度变化。

动态内容行为：

- 测量发生在 Avalonia layout pass 中，子元素尺寸变化通过正常的 `InvalidateMeasure` 冒泡进入下一次布局，不需要额外监听机制或视觉包装层。
- 子元素尺寸变化由子元素自身发起 `InvalidateMeasure`，MasonryPanel 作为父级 `Panel` 自动响应重新测量。
- 图片加载、网络资源完成和内容异步替换不由 Masonry 发起。Masonry 只响应子元素尺寸变化。

键盘与焦点行为：

- `Tab`、读屏顺序、logical children 顺序和 item container 顺序必须保持 `ItemsControl` 源集合顺序。
- shortest-column 视觉排列不会重排 logical order，也不会改变数据绑定容器生成顺序。
- Masonry 不实现“视觉最近项”方向键导航。方向键、快捷键和焦点环行为由子项控件、外层页面或 Avalonia 默认焦点系统决定。
- 子项控件保留自身 `Focusable`、命中测试、上下文菜单、拖拽和交互状态；Masonry 不拦截或改写这些行为。

## 6. 状态模型

Masonry 的状态模型应在 C# 布局层完成归一，AXAML 不承担列数、间距或子项位置计算。

Masonry 的有效状态由响应式属性、兼容属性和布局约束共同决定。响应式属性只在当前媒体断点命中显式配置时覆盖兼容属性；未命中时继续使用兼容属性和容器自适应规则。

有效间距计算：

```text
fallbackColumnGap = normalize(ColumnGap)
fallbackRowGap    = normalize(RowGap)

if Gutter hits current breakpoint:
  EffectiveColumnGap = normalize(Gutter.Horizontal)
  EffectiveRowGap    = normalize(Gutter.Vertical)
else:
  EffectiveColumnGap = fallbackColumnGap
  EffectiveRowGap    = fallbackRowGap
```

`Gutter` 的水平和垂直维度独立解析。只声明水平响应式 gutter 时，垂直间距不得被隐式改为 `0`，必须回退到 `RowGap`。

有效列数计算：

```text
if ColumnInfo hits current breakpoint:
  EffectiveColumnCount = ColumnInfo
else if ColumnCount > 0:
  EffectiveColumnCount = ColumnCount
else:
  EffectiveColumnCount = floor((AvailableWidth + EffectiveColumnGap) / (MinColumnWidth + EffectiveColumnGap))
  EffectiveColumnCount = clamp(EffectiveColumnCount, 1, MaxColumnCount)
```

当可用宽度为无穷时，Masonry 使用 `MinColumnWidth * MaxColumnCount + EffectiveColumnGap * (MaxColumnCount - 1)` 作为内部测量宽度，其中 `MinColumnWidth`、`MaxColumnCount` 和 `EffectiveColumnGap` 先按 API 归一规则计算有效值。该规则保证控件在未被外层约束宽度时仍能得到确定布局。

有效列宽计算：

```text
EffectiveColumnWidth =
  (AvailableWidth - EffectiveColumnGap * (EffectiveColumnCount - 1)) / EffectiveColumnCount
```

布局过程：

1. 按 `Children` 顺序处理子元素。
2. 不可见子元素保留默认排列矩形，不参与列高计算。
3. 普通子元素以 `EffectiveColumnWidth`、无限高度进行测量。
4. `Masonry.Span=Full` 子元素以完整可用宽度、无限高度进行测量。
5. 指定 `Masonry.Column` 的子元素放入有效列；未指定列的子元素放入当前最短列。
6. 子元素的 `Y` 等于目标列当前高度，非首项追加 `EffectiveRowGap`。
7. 整行子元素的 `Y` 等于当前最高列高度，非首行追加 `EffectiveRowGap`，排列后所有列高度同步到该子元素底部。
8. Panel desired height 等于所有列高度最大值。

Tie-break 规则必须稳定。多个列高度相同时，选择索引最小的列，保证相同输入得到相同布局结果。

## 7. 模板与视觉架构

`Masonry` 派生自 `ItemsControl`，需要一个 `ControlTheme` 装配 `ItemsPresenter`。默认 `ItemsPanel` 为 internal `MasonryPanel`，它派生自 `Panel`，不定义 `ControlTemplate`，也不定义 template part。

`Masonry` 必须尊重继承自 `ItemsControl` 的 `ItemsPanel` 公共 API。默认主题负责提供 `MasonryPanel` 作为默认值；如果外部显式替换 `ItemsPanel`，替换后的面板接管 `ItemsPresenter` 子项排列。此时控件仍是 `ItemsControl`，但 Masonry-specific 的布局算法、attached property 消费和 `LayoutChanged` 语义不再由默认 `MasonryPanel` 保证。

`Masonry` 不主动为子项插入视觉包装层。两种内容提供方式产生不同的容器层级：

- 直接放置子元素：item 本身是 `Control`，`ItemsControl` 基类判定其自身即容器，不产生额外层级。
- `ItemsSource` 数据绑定：item 是数据对象，由 `ItemsControl` 固有机制生成 `ContentPresenter` 容器。这层 `ContentPresenter` 是 `ItemsControl` 渲染数据对象的固有产物，不属于 `Masonry` 或 `MasonryPanel` 主动插入的包装层。

默认视觉树形态：

```text
Masonry (ItemsControl, default ItemsPanel = MasonryPanel)
└─ ItemsPresenter
   └─ MasonryPanel (internal)        ← 布局引擎
      ├─ <子元素>                     ← 直接放置时无包装层
      ├─ ContentPresenter → <子元素>  ← ItemsSource 绑定时
      └─ ...
```

视觉树要求：

- `Masonry` 和 `MasonryPanel` 节点只承担布局与容器装配职责。
- 子项视觉结构完全由子项控件或 `ItemTemplate` 负责。
- 不通过模板节点实现列、行、占位或测量辅助对象。
- 不通过透明 Border 扩展命中区域。
- 不通过不可见控件缓存测量结果。
- 不为子项插入额外的 `ContentPresenter`、`Border` 或其他视觉包装层（数据绑定场景由 `ItemsControl` 固有机制产生的 `ContentPresenter` 不在此约束范围内）。

如果主题提供默认样式，只能设置布局属性默认值或外层布局相关属性，不应绘制背景、边框、阴影或卡片状态。Masonry 的设计目标是在保证可维护性的前提下保持最少 VisualTree 层级。

## 8. Theme 架构

Masonry 当前不定义专属 `token.md`。Masonry 不需要创建组件级颜色、阴影或卡片外观 Token。

`MasonryTheme.axaml` 装配 `ItemsPresenter`，默认使用 internal `MasonryPanel`，并通过 `RelativeSource` 或等价机制把 `Masonry` 上的布局属性传递给 `MasonryPanel`。Theme 不应把实例状态、动态列数或子项位置写入 Token。

允许的 Theme 职责：

- 装配 `ItemsPresenter` 与 `MasonryPanel`。
- 尊重 `ItemsControl.ItemsPanel` 公共契约，使显式替换 `ItemsPanel` 的行为可预测。
- 通过绑定把 `Masonry` 布局属性传递给 `MasonryPanel`，包括 `ColumnCount`、`ColumnInfo`、`MinColumnWidth`、`MaxColumnCount`、`ColumnGap`、`RowGap` 与 `Gutter`。
- 设置或覆盖默认行列间距。
- 设置或覆盖默认最小列宽和最大列数。
- 支持暗色/亮色主题下相同的布局语义。

不允许的 Theme 职责：

- 为子项添加卡片背景、圆角、边框或阴影。
- 定义 Masonry 私有颜色表。
- 通过 selector 计算列数或位置。
- 依赖子项类型实现布局规则。

## 9. 控件家族或集成关系

Masonry 属于 Layout 分类，与 `FlexPanel`、`Row`、`Space` 等布局能力共同构成 AtomUI 的桌面布局基础。

集成关系：

- 直接子元素场景：用户在 `atom:Masonry` 中放置任意 `Control`，子元素本身即容器，Masonry 负责布局。
- 数据绑定场景：用户为 `atom:Masonry` 设置 `ItemsSource`，由 `ItemsControl` 基类负责 item container、数据模板和集合变更监听，`Masonry` 不重复实现这部分能力。
- 图片场景：Masonry 可承载图片控件，但不负责图片解码、缓存、占位、错误图或网络加载。
- 滚动场景：Masonry 放在 `ScrollViewer` 内使用，不改变 ScrollViewer 的滚动语义。

Masonry 不提供虚拟化语义。瀑布流虚拟化涉及滚动偏移、容器回收、动态高度预测和键盘导航补偿，不能在 Masonry 中隐式开启。作为 `ItemsControl` 使用时，默认不回收容器，所有 item container 全量实现。

## 10. 兼容性不变量

优化或扩展 Masonry 时必须保持以下不变量：

- 不改变子元素 logical order、visual child order 和 ItemsControl 容器生成顺序。
- 不修改子元素 `DataContext`、内容、样式类、主题或资源作用域。
- 不为子项主动插入额外视觉包装层（数据绑定场景由 `ItemsControl` 固有机制产生的 `ContentPresenter` 不在此约束范围内）。
- 不要求用户为子项提供 key。
- 不把 Masonry 变成 ScrollViewer 或数据控件。
- 不隐式引入虚拟化、延迟生成或容器回收语义。
- 不把卡片外观、图片加载或业务状态写入 Masonry。
- 不通过 AXAML selector 承担列数和位置计算。
- 不改变 `ColumnInfo`、`ColumnCount` 与容器自适应列数之间的优先级。
- 不改变 `Gutter` 与 `ColumnGap` / `RowGap` 之间的优先级。
- 不在 `Gutter` 未声明垂直维度时把有效垂直间距隐式改为 `0`。
- 不改变 attached property 的含义、默认值和生效优先级。
- 不把 `Masonry.Column`、`Masonry.Span` 的读取对象从 item container 隐式改为 `ItemTemplate` 内部元素。
- 不破坏继承自 `ItemsControl` 的 `ItemsPanel` 公共契约；替换 `ItemsPanel` 必须被视为替换布局引擎。
- 不把 `LayoutChanged` 改成像素级布局事件或 layout pass 内同步事件。
- 不将实例状态迁移为控件 Token。
- `MasonryPanel` 保持 `internal`，仅作为 `Masonry` 的默认 `ItemsPanel` 装配，不暴露给开发者。
- `Masonry` 不 override `ItemsControl` 的 `NeedsContainer`、`CreateContainer`、`PrepareContainer` 等容器生成方法，两种内容提供方式的容器层级由基类决定。

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

Masonry 消费 AtomUI 共享响应式机制，不复制断点定义、解析顺序、XAML 字符串格式或 partial map 继承规则。共享机制见 [AtomUI 响应式机制设计](../../../../modules/controls-shared/responsive-system.md)。

响应式 API：

- `ColumnInfo` 对齐 Ant Design Masonry `columns`，类型为 `ResponsiveInt?`，支持固定整数和 partial breakpoint map。
- `Gutter` 对齐 Ant Design Masonry `gutter`，类型为 `ResponsiveGutter?`，支持固定间距、水平/垂直 pair、responsive map 和水平/垂直 responsive pair。

响应式优先级：

```text
ColumnInfo hit -> ColumnCount -> MinColumnWidth + MaxColumnCount + AvailableWidth
Gutter hit     -> ColumnGap / RowGap
```

`ColumnInfo` 和 `Gutter` 只在当前断点命中显式配置时覆盖兼容属性。当前断点小于所有显式配置，或某个维度未声明时，Masonry 必须继续使用控件自身 fallback，而不能让共享 resolver 注入默认值。

断点变化由 `MasonryPanel` 通过 `MediaQueryHost.FindOwner` 获取最近的 `IMediaBreakAwareControl` 并订阅 `MediaBreakPointChanged`。事件回调只更新当前断点缓存并触发 `InvalidateMeasure`，不得在回调中执行完整布局、读取子项尺寸或派发 `LayoutChanged`。订阅必须在 detached 或 owner 替换时释放。

响应式 resolver 必须保持纯计算属性，不依赖 VisualTree、主题资源或控件实例默认值。控件级 fallback 始终保留在 Masonry effective state 层。

### 11.3 整行项模型

`Masonry.Span=Full` 是 AtomUI Masonry 的子项布局元数据，用于标题、说明块、分组分隔、宽幅图或需要打断瀑布流节奏的内容。整行项不改变前后子项的 logical order。

排列规则：

- 整行项从当前最高列之后开始。
- 非首行整行项前追加 `RowGap`。
- 整行项宽度为 Masonry 有效宽度。
- 整行项排列后，所有列高度同步到整行项底部。

整行项语义完全通过 `Masonry.Span` attached property 表达，不依赖具体子项类型。

### 11.4 子项容器元数据模型

Masonry 的布局元数据属于 item container，而不是数据对象或模板内部视觉元素。该规则保证直接子元素和 `ItemsSource` 两种模式都能通过同一套 `MasonryPanel.Children` 计算路径布局。

容器元数据模型：

```text
Direct children
  Masonry.Children item is Control
  attached properties live on that Control

ItemsSource
  data item → generated ContentPresenter
  attached properties live on generated ContentPresenter
  ItemTemplate root only defines item visuals
```

该模型的维护要求：

- `MasonryPanel` 只读取其直接 child 上的 `Masonry.Column` 和 `Masonry.Span`。
- 直接子元素模式不得额外包装子项以承载元数据。
- 数据绑定模式应通过 item container 层设置 attached property，例如 `ItemContainerTheme`。
- 如果需要让数据项字段驱动列或整行状态，应把数据绑定到 container 的 attached property，而不是绑定到模板根元素。
- attached property 变化必须触发布局重新测量，使列分配和整行状态在下一次 layout pass 中重新计算。

### 11.5 布局变化通知模型

`LayoutChanged` 的通知对象是有效布局分配，不是像素矩形。它用于让宿主页面观察每个 item container 的列归属和整行状态，例如埋点、辅助调试、外部状态同步或 Gallery 展示，不用于驱动动画帧或滚动位置。

通知模型：

```text
MasonryPanel Measure / Arrange
  calculate column assignment and full-span flags
  compare with last assignment snapshot
  if assignment changed:
    post event dispatch outside layout pass
```

比较维度只包含 item container 数量、顺序、有效列和 `IsFullSpan`。事件不暴露排列矩形，因为矩形变化可能由可用宽度、像素舍入、字体渲染或子项高度变化造成，作为公共事件会造成过高频率和错误依赖。

空集合也是有效布局状态。当上一次通知为非空而当前布局为空时，必须通知一次空集合；当布局持续为空时，不重复通知。

## 12. 验证策略

Masonry 改动应按文档、C# 布局、AXAML/Theme、Public API 和 Gallery 分层验证。

文档验证：

```bash
git diff --check
```

C# 布局验证（测试目标：`tests/AtomUI.Desktop.Controls.Tests/`）：

- 默认值保持 `ColumnCount=0`、`MinColumnWidth=320d`、`MaxColumnCount=4`、`ColumnGap=16d`、`RowGap=16d`。
- 默认响应式属性保持 `ColumnInfo=null`、`Gutter=null`。
- 无效值归一规则稳定：自适应最小列宽不小于 `1d`，最大列数不小于 `1`，无效 gap 归一为 `0d`。
- `ColumnInfo="xs: 1, md: 3"` 在 `xs/sm` 下解析为 `1`，在 `md` 及以上断点解析为 `3`。
- `ColumnInfo="xl: 4"` 在 `lg` 及以下断点未命中时回退到 `ColumnCount` 或容器自适应列数。
- `Gutter` 的水平和垂直维度独立解析，未声明维度回退到对应兼容属性。
- `Gutter` 当前断点未命中时回退到 `ColumnGap` 与 `RowGap`。
- 媒体断点变化只触发布局失效，并在下一次 layout pass 中重新计算有效列数和间距。
- 固定列数下按 shortest-column 放置子项。
- 自适应列数遵守 `MinColumnWidth`、`MaxColumnCount` 和可用宽度。
- `ColumnGap`、`RowGap` 影响坐标和 desired height。
- 多列等高时选择索引最小列。
- 不可见子元素不参与列高计算。
- `Masonry.Column` 指定列并正确夹取越界值。
- `Masonry.Span=Full` 从最高列之后开始并同步所有列高度。
- 无限可用宽度下仍能得到确定 desired size。
- 子元素尺寸变化、attached property 变化和布局属性变化会触发重新测量。
- 直接放置子元素时子元素本身即容器，无额外视觉层级。
- `ItemsSource` 绑定时由基类生成 `ContentPresenter` 容器，`ObservableCollection` 增删能正确同步到布局。
- `ItemsSource` 绑定时，`Masonry.Column` 与 `Masonry.Span` 在 generated item container 上生效；写在 `ItemTemplate` 内部视觉根元素上不应被误判为容器布局元数据。
- `LayoutChanged` 只在 item container 数量、顺序、有效列或整行状态变化时触发；纯像素矩形变化不触发。
- `LayoutChanged` 派发避开 layout pass 内部，同一布局周期不重复派发等价通知。
- 从非空布局切换为空布局时派发一次空 `Items` 通知，持续为空时不重复通知。
- `MasonryItemLayout.Element` 在直接子元素场景指向直接子元素，在 `ItemsSource` 场景指向生成的 `ContentPresenter`。
- 替换 `ItemsPanel` 时不应再承诺默认 `MasonryPanel` 的 shortest-column、整行项和 `LayoutChanged` 布局结果语义。

AXAML/Theme 验证：

- 默认 Theme 装配 `ItemsPresenter` 与 internal `MasonryPanel`，不增加子项视觉包装层。
- Theme 尊重 `ItemsControl.ItemsPanel` 公共契约；显式替换 `ItemsPanel` 的行为应可预测。
- Theme 通过绑定把 `Masonry` 布局属性传递给 `MasonryPanel`，包括 `ColumnInfo` 与 `Gutter`。
- Theme 只设置布局默认值，不绘制子项外观。
- 若 Theme 覆盖默认间距或列宽，这些覆盖只能改变实例默认表现，不能改变 API 语义和归一规则。

Public API 验证：

- 所有公共布局属性均为 `StyledProperty`，包括 `ColumnInfoProperty` 与 `GutterProperty`。
- 子项布局元数据 `Masonry.Column`、`Masonry.Span` 定义在 `Masonry` 类上，`Masonry` 同时承担控件与 attached property 容器职责。
- `MasonryPanel` 为 `internal`，不出现在公共 API 表面。
- `LayoutChanged` 事件不要求业务 key，不改变 Avalonia layout lifecycle，触发避开 layout pass 内部。
- API 默认值、单位和无效值归一规则稳定。
- 继承的 `ItemsSource`、`ItemTemplate`、`ItemContainerTheme`、`ItemsPanel` 等 `ItemsControl` API 语义不被 Masonry 重新定义。

Gallery 验证：

- 直接放置子元素和 `ItemsSource` 绑定两种用法下，容器顺序和键盘导航顺序均保持源集合顺序。
- 图片、异步内容和高度变化场景下布局能重新稳定。
- 示例卡片、API 表和 Token 表等可变高度内容不出现重叠、裁剪或异常空白。
- API 表默认值与公共属性保持一致：`0`、`320`、`4`、`16`、`16`、`null`、`Auto`。
- 涉及显式列或整行项的 Gallery 示例必须把 `Masonry.Column`、`Masonry.Span` 设置到 item container 层。
