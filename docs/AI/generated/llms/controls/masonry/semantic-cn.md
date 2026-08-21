# Masonry 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

### 2.1 `root`

| 字段 | 值 |
| --- | --- |
| Owner | `Masonry` |
| Part | `root` |
| Selector | Masonry 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `Masonry` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| ThemePropertyName | 不适用 |
| AtomUI 节点 | Masonry owner |
| 职责 | 瀑布流布局根区域，承载列数、间距、响应式、root chrome、ItemsControl 输入和 item Selector 作用域。 |
| 相关 API | `Items`、`ItemsSource`、`ItemTemplate`、`ItemContainerTheme`、`ItemsPanel`、`ColumnCount`、`ColumnInfo`、`MinColumnWidth`、`MaxColumnCount`、`ColumnGap`、`RowGap`、`Gutter`、`Background`、`BorderBrush`、`BorderThickness`、`CornerRadius`、`Padding`、`LayoutChanged` |
| 相关 Token | 无专属 Token；间距和列宽是实例布局状态。 |
| 稳定性 | stable since 6.0 |

### 2.2 `item`

| 字段 | 值 |
| --- | --- |
| Owner | `Masonry` |
| Part | `item` |
| Selector | `.semantic-item` |
| SelectorRoute | `> .semantic-item` |
| Style Type | `MasonryItemStyle` |
| ContractType | `Control` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| ThemePropertyName | 不适用 |
| AtomUI 节点 | 每个 item container：直接子元素或 generated `ContentPresenter`。 |
| 职责 | 表示一个被 Masonry 测量、分配列并排列的条目容器。 |
| 相关 API | `Items`、`ItemsSource`、`ItemTemplate`、`ItemContainerTheme`、`Masonry.Column`、`Masonry.Span` |
| 相关 Token | 无专属 Token；item 外观由 item 自身控件或模板负责。 |
| 稳定性 | stable since 6.0 |

`ContractType=Control` 是有意选择：Masonry 同时支持用户直接提供任意 `Control` 作为条目，以及 `ItemsSource` 场景下由
Avalonia 生成 `ContentPresenter`。因此 `MasonryItemStyle` 只能稳定依赖 `Control` 共有属性，例如 `Margin`、`Opacity`、
`Width`、`Height`、`MinHeight`、`MaxHeight`、`HorizontalAlignment`、`VerticalAlignment`、`RenderTransform`、
`RenderTransformOrigin` 和 `IsVisible`。需要设置 `Background`、`BorderBrush`、`CornerRadius` 或 Card 专属属性时，应继续定制
item 自身控件或 `ItemTemplate` 内部控件，而不是扩大 Masonry item 的 `ContractType`。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Masonry/Themes/MasonryTheme.axaml`

```xml
<PixelAlignedBorder Name="PART_RootBorder">
    <ItemsPresenter Name="PART_ItemsPresenter" />
</PixelAlignedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Masonry
  -> Masonry (control theme, MasonryTheme.axaml)
     -> PixelAlignedBorder#PART_RootBorder (template-stable)
        -> ItemsPresenter#PART_ItemsPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Masonry` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Masonry` | control theme | `MasonryTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `ClipToBounds`, `CornerRadius`, `ItemsPanel` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_RootBorder` | template node (PixelAlignedBorder) | `MasonryTheme.axaml` | Masonry | `Background`, `BorderBrush`, `BorderThickness`, `ClipToBounds`, `CornerRadius`, `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `MasonryTheme.axaml` | Masonry | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

源文档未声明稳定 Template Part。维护模板时应以源码和主题文件中的实际声明为准。

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

Masonry 本身没有 hover、pressed、disabled、loading 或 checked 等交互状态。它必须完整保留子元素自身的命中测试、焦点、键盘导航、拖拽、上下文菜单和动画行为。

有效状态由响应式属性、兼容属性、可用宽度和子项 attached property 共同决定。状态归一发生在 C# 布局层，AXAML 不承担列数、间距或子项位置计算。

布局状态包括：

- 有效列数。
- 有效列宽。
- 有效水平和垂直间距。
- 子项显式列。
- 子项是否整行。
- 最终有效列分配。
- 自动列分配策略，以及 `StableColumns` 下按 item container 实例维护的已提交列归属。

`Tab`、读屏顺序、logical children 顺序和 item container 顺序必须保持 `ItemsControl` 源集合顺序。两种列分配策略都只改变视觉位置，不重排 logical order，也不改变数据绑定容器生成顺序。

## Theme and Token Boundaries

Masonry 的默认主题装配 root chrome `PixelAlignedBorder`、`ItemsPresenter` 与 internal `MasonryPanel`，并把 Masonry 的布局属性与 root chrome 属性分别传递给对应层。Theme 不绘制子项外观，不通过 selector 计算列数和位置。Semantic Part marker 不写入主题静态节点；`.semantic-item` 由 Masonry 在 item container 准备阶段补齐。

默认视觉树：

```text
Masonry (ItemsControl, default ItemsPanel = MasonryPanel)
└─ PixelAlignedBorder#PART_RootBorder
   └─ ItemsPresenter
      └─ MasonryPanel (internal)
         ├─ <子元素>
         ├─ ContentPresenter → <子元素>
         └─ ...
```

视觉树要求：

- `Masonry` 和 `MasonryPanel` 节点只承担布局与容器装配职责；root chrome 由 `PixelAlignedBorder` 承载。
- 子项视觉结构完全由子项控件或 `ItemTemplate` 负责。
- 不通过模板节点实现列、行、占位或测量辅助对象。
- 不通过透明 Border 扩展命中区域。
- 不通过不可见控件缓存测量结果。
- 不为子项主动插入额外视觉包装层。

Masonry 当前不定义专属 Token，不需要创建 `token.md`。Masonry 的间距和列宽属于实例布局状态，不应迁移为控件 Token。root chrome 仅复用 `ItemsControl` 已有的 `Background`、`BorderBrush`、`BorderThickness`、`CornerRadius` 与 `Padding`，不引入独立 token 边界。

Token 边界：

- Masonry 当前没有专属 `token.md`；LLMS 生成按第 5 节视觉与主题模型、SharedToken、控件家族 Token 或主题资源说明 Token 边界。

## Customization Boundaries

优化或扩展 Masonry 时必须保持以下不变量：

- 不改变子元素 logical order、visual child order 和 ItemsControl 容器生成顺序。
- 除了发布契约要求的 `.semantic-item` marker，不修改子元素 `DataContext`、内容、样式类、主题或资源作用域。
- 不为子项主动插入额外视觉包装层。
- 不要求用户为子项提供 key。
- 不把 Masonry 变成 ScrollViewer、数据源管理器或卡片外观组件。
- 不隐式引入虚拟化、延迟生成或容器回收语义。
- 不通过 AXAML selector 承担列数和位置计算。
- 不改变 `ColumnInfo`、`ColumnCount` 与容器自适应列数之间的优先级。
- 不改变 `Gutter` 与 `ColumnGap` / `RowGap` 之间的优先级。
- 不在 `Gutter` 未声明垂直维度时把有效垂直间距隐式改为 `0`。
- `LayoutStrategy` 的默认值保持 `StableColumns`；经典 shortest-column 重排必须通过 `Reflow` 显式选择。
- `StableColumns` 在有效列数不变时按 item container 引用保持已提交列归属，不得退化为按索引或数据项值关联。
- 不把 `Masonry.Column`、`Masonry.Span` 的读取对象从 item container 隐式改为 `ItemTemplate` 内部元素。
- 不破坏继承自 `ItemsControl` 的 `ItemsPanel` 公共契约。
- `MasonryPanel` 保持 `internal`，仅作为 `Masonry` 的默认 `ItemsPanel` 装配。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- `MasonryPanel` 保持 internal。
- `MasonryPanel` 只读取直接 child 上的 `Masonry.Column` 和 `Masonry.Span`。
- 直接子元素模式不额外包装子项；`.semantic-item` marker 加在用户直接子 `Control` 上。
- `ItemsSource` 模式通过基类生成 `ContentPresenter`，Masonry 不重写容器生成；`.semantic-item` marker 加在 generated container 上。
- `MasonryItemStyle` 的 `ContractType` 保持 `Control`，不得收窄到 `ContentPresenter` 或任何具体 item 控件。
- root chrome 由默认主题中的 `PixelAlignedBorder` 承载；`Background`、`BorderBrush`、`BorderThickness`、`CornerRadius` 和 `Padding` 仍然属于 Masonry 的 inherited root 属性，不要改成 item 属性。
- 响应式断点变化只触发布局失效，不在断点回调中执行完整布局或派发事件。
- Measure→Arrange 同一有效宽度必须复用已测量的布局结果；不得在正常布局周期中无条件重复执行第二次 `O(items × columns)` 计算。
- 布局缓存不得跨越宽度、断点或下一次 Measure；Arrange 消费后必须释放缓存引用。
- `LayoutChanged` 不在 layout pass 内同步派发。
- `LayoutChanged` 只比较 item container 数量、顺序、有效列和整行状态，不因像素矩形、列宽或列内纵向位置变化派发。
- `StableColumns` 的持久状态只由 `MasonryPanel` 拥有，并按 item container 引用关联；不得退化为按索引保存。
- `StableColumns` 只在 Arrange 后提交分配；Measure 不得修改已提交快照。
- `StableColumns` 不等待异步内容加载完成；调用方通过尺寸约束控制首次分配依据。
- `Reflow` 每次布局计算都从当前高度状态执行 shortest-column 分配，不读取稳定列快照。
- 替换 `ItemsPanel` 等价于替换布局引擎，Masonry-specific 布局语义不再由默认面板保证。
