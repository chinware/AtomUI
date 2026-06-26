# Masonry

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

Masonry 是桌面端瀑布流布局控件，用于将高度不一致的内容块按列组织，并通过 shortest-column 策略降低列高差。它适合图片墙、卡片流、示例集合、资源列表等内容高度不可预先统一的场景。

`Masonry` 派生自 Avalonia `ItemsControl`，同时承担数据绑定入口和子项布局元数据 attached property 容器（`atom:Masonry.Column`、`atom:Masonry.Span`）两种职责。

Masonry 是布局控件，不是数据源管理器、卡片控件、图片加载器、虚拟化列表、滚动容器或内容渲染器。它只负责对子元素进行测量、列分配和排列，不拥有子元素内容，也不改变子元素的业务语义。

Masonry 支持两种内容提供方式：

- 直接放置子元素：在 `atom:Masonry` 的内容中放置任意 `Control`，子元素本身即作为容器，不产生 Masonry 主动插入的视觉包装层。
- 数据绑定：通过 `ItemsSource` 绑定数据集合，由 `ItemsControl` 固有机制生成容器。集合监听、容器生成和增删同步由基类承担。

滚动由外层 `ScrollViewer` 承担，图片加载由图片控件或外部加载库承担。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Layout/Masonry` |
| 状态 | Stable |

## 何时使用

Masonry 表达的是“密集但有秩序”的内容浏览体验。它利用可变高度卡片形成自然节奏，避免为了对齐网格而牺牲内容本身的信息密度。

设计语言要求：

- 视觉上按列平衡内容高度，降低单列过长造成的空白和浏览断层。
- 逻辑上保留子元素顺序，保证读屏、键盘导航、数据绑定容器生成顺序和源集合顺序一致。
- 不为子项强加卡片外观。子项可以是 Card、Image、ShowCaseItem 或任意 `Control`。
- 不插入额外视觉包装层，使 VisualTree 保持最小。
- 间距只表达布局节奏，不表达组件语义颜色、阴影或卡片样式。

## 公共 API

Masonry 的公共 API 采用 Avalonia 原生属性模型。需要 XAML 设置、Style 设置、绑定、动画或主题参与的状态必须定义为 `StyledProperty` 或 attached property。

核心布局属性：

| API | 类型 | 默认值 | 语义 |
| --- | --- | --- | --- |
| `ColumnCount` | `int` | `0` | 固定列数；`<= 0` 时进入容器自适应列数。 |
| `ColumnInfo` | `ResponsiveInt?` | `null` | 按媒体断点变化的列数。 |
| `MinColumnWidth` | `double` | `320d` | 自适应列数下的最小列宽。 |
| `MaxColumnCount` | `int` | `4` | 自适应列数下的最大列数。 |
| `ColumnGap` | `double` | `16d` | 水平列间距。 |
| `RowGap` | `double` | `16d` | 垂直行间距。 |
| `Gutter` | `ResponsiveGutter?` | `null` | 按媒体断点变化的水平/垂直间距。 |

响应式属性遵循 [AtomUI 响应式机制设计](../../../../modules/controls-shared/responsive-system.md)。Masonry 不复制断点定义、解析顺序或 partial map 继承规则；它只在 effective state 层提供列数和间距 fallback。

列数优先级：

1. `ColumnInfo` 在当前断点命中显式配置时，使用响应式列数。
2. `ColumnInfo` 未命中且 `ColumnCount > 0` 时，使用固定列数。
3. 前两者均未命中时，使用 `MinColumnWidth + MaxColumnCount + AvailableWidth` 的容器自适应列数。

间距优先级：

1. `Gutter` 在当前断点命中显式配置时，使用响应式水平/垂直间距。
2. `Gutter` 未命中时，使用 `ColumnGap` 与 `RowGap`。

子项布局元数据：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Masonry.Column` | `int?` attached property | 指定子项所在列，列索引按 `0` 起始。 |
| `Masonry.Span` | `MasonryItemSpan` attached property | `Auto` 普通瀑布流项；`Full` 整行项。 |

attached property 的消费对象是 `MasonryPanel.Children` 中被测量和排列的 item container。直接放置子元素时写在子元素本身；`ItemsSource` 场景应写在 generated container 上，例如通过 `ItemContainerTheme` 绑定。

事件模型：

```csharp
public event EventHandler<MasonryLayoutChangedEventArgs>? LayoutChanged;
```

`LayoutChanged` 是有效布局分配通知，不是像素级滚动或动画事件。它只表达 item container 顺序、有效列和整行状态的变化。

## 事件与命令

事件模型：
public event EventHandler<MasonryLayoutChangedEventArgs>? LayoutChanged;
`LayoutChanged` 是有效布局分配通知，不是像素级滚动或动画事件。它只表达 item container 顺序、有效列和整行状态的变化。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/Masonry/Views/MasonryShowCase.axaml:144`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Masonry ItemsSource="{Binding BasicItems}"
              ColumnCount="4"
              ColumnGap="16"
              RowGap="16"
              HorizontalAlignment="Stretch">
    <atom:Masonry.ItemTemplate>
        <DataTemplate x:DataType="vm:MasonryBasicItem">
            <Panel>
                <atom:Card SizeType="Small"
                           Height="{Binding Height}"
                           HorizontalAlignment="Stretch"
                           IsVisible="{Binding !IsSpecial}">
                    <atom:TextBlock Text="{Binding Index}" />
                </atom:Card>
                <atom:Card SizeType="Small"
                           ClipToBounds="True"
                           HorizontalAlignment="Stretch"
                           IsVisible="{Binding IsSpecial}">
                    <atom:Card.Cover>
                        <Panel MinHeight="210"
                               ClipToBounds="True">
                            <Image Name="SpecialCoverImage"
                                   asyncImageLoader:ImageLoader.Source="{Binding CoverSource}"
                                   Stretch="UniformToFill" />
                            <Border Padding="16,16"
                                    IsVisible="{Binding #SpecialCoverImage.Source, Converter={x:Static ObjectConverters.IsNull}}">
                                <atom:Skeleton IsLoading="True"
                                               IsActive="True"
                                               IsShowAvatar="False"
                                               IsShowTitle="True"
                                               ParagraphRows="3"
                                               IsRound="True" />
                            </Border>
                        </Panel>
                    </atom:Card.Cover>
                    <atom:CardMetaContent Header="{Binding Title}"
                                          Content="{Binding Description}" />
                </atom:Card>
            </Panel>
        </DataTemplate>
    </atom:Masonry.ItemTemplate>
</atom:Masonry>
```

### 响应式

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/Masonry/Views/MasonryShowCase.axaml:198`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:Masonry ItemsSource="{Binding ResponsiveItems}"
              ColumnInfo="xs: 1, sm: 2, md: 3, lg: 4"
              Gutter="xs: 8, sm: 12, md: 16; xs: 8, sm: 12, md: 16"
              HorizontalAlignment="Stretch">
    <atom:Masonry.ItemTemplate>
        <DataTemplate x:DataType="vm:MasonryBasicItem">
            <atom:Card SizeType="Small"
                       Height="{Binding Height}"
                       HorizontalAlignment="Stretch">
                <atom:TextBlock Text="{Binding Index}" />
            </atom:Card>
        </DataTemplate>
    </atom:Masonry.ItemTemplate>
</atom:Masonry>
```

### 图片

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/Masonry/Views/MasonryShowCase.axaml:224`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:Masonry ItemsSource="{Binding ImageItems}"
              ColumnCount="4"
              ColumnGap="16"
              RowGap="16"
              HorizontalAlignment="Stretch">
    <atom:Masonry.ItemTemplate>
        <DataTemplate x:DataType="vm:MasonryImageItem">
            <Panel ClipToBounds="True">
                <Image Name="MasonryImage"
                       asyncImageLoader:ImageLoader.Source="{Binding ImageSource}"
                       Stretch="Uniform"
                       HorizontalAlignment="Stretch" />
                <Border MinHeight="210"
                        Padding="16,16"
                        IsVisible="{Binding #MasonryImage.Source, Converter={x:Static ObjectConverters.IsNull}}">
                    <atom:Skeleton IsLoading="True"
                                   IsActive="True"
                                   IsShowAvatar="False"
                                   IsShowTitle="True"
                                   ParagraphRows="3"
                                   IsRound="True" />
                </Border>
            </Panel>
        </DataTemplate>
    </atom:Masonry.ItemTemplate>
</atom:Masonry>
```

## 状态模型

Masonry 本身没有 hover、pressed、disabled、loading 或 checked 等交互状态。它必须完整保留子元素自身的命中测试、焦点、键盘导航、拖拽、上下文菜单和动画行为。

有效状态由响应式属性、兼容属性、可用宽度和子项 attached property 共同决定。状态归一发生在 C# 布局层，AXAML 不承担列数、间距或子项位置计算。

布局状态包括：

- 有效列数。
- 有效列宽。
- 有效水平和垂直间距。
- 子项显式列。
- 子项是否整行。
- 最终有效列分配。

`Tab`、读屏顺序、logical children 顺序和 item container 顺序必须保持 `ItemsControl` 源集合顺序。shortest-column 视觉排列不会重排 logical order，也不会改变数据绑定容器生成顺序。

## 主题与 Design Token

Masonry 的默认主题只装配 `ItemsPresenter` 与 internal `MasonryPanel`，并把 Masonry 布局属性传递给布局面板。Theme 不绘制子项外观，不通过 selector 计算列数和位置。

默认视觉树：

```text
Masonry (ItemsControl, default ItemsPanel = MasonryPanel)
└─ ItemsPresenter
   └─ MasonryPanel (internal)
      ├─ <子元素>
      ├─ ContentPresenter → <子元素>
      └─ ...
```

视觉树要求：

- `Masonry` 和 `MasonryPanel` 节点只承担布局与容器装配职责。
- 子项视觉结构完全由子项控件或 `ItemTemplate` 负责。
- 不通过模板节点实现列、行、占位或测量辅助对象。
- 不通过透明 Border 扩展命中区域。
- 不通过不可见控件缓存测量结果。
- 不为子项主动插入额外视觉包装层。

Masonry 当前不定义专属 Token，不需要创建 `token.md`。Masonry 的间距和列宽属于实例布局状态，不应迁移为控件 Token。

Token 来源：

- Masonry 当前没有专属 `token.md`；LLMS 生成按第 5 节视觉与主题模型、SharedToken、控件家族 Token 或主题资源说明 Token 边界。

## AOT 与裁剪注意事项

Masonry 不使用反射读取 item template 内部元素，不创建不可见测量控件，不通过透明元素扩展命中区域。

响应式 resolver 必须保持纯计算属性，不依赖 VisualTree、主题资源或控件实例默认值。控件级 fallback 始终保留在 Masonry effective state 层。

布局变化通知比较维度只包含 item container 数量、顺序、有效列和 `IsFullSpan`。不把像素矩形作为事件比较维度，可以避免宽度变化、字体渲染或子项高度细微变化造成高频事件。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Masonry/Masonry.cs`：公开控件类型、布局属性、attached property、`LayoutChanged` 事件入口。
- `src/AtomUI.Desktop.Controls/Masonry/MasonryPanel.cs`：internal 布局引擎，执行测量、排列、响应式断点监听和布局结果比较。
- `src/AtomUI.Desktop.Controls/Masonry/MasonryItemSpan.cs`：子项 span 枚举。
- `src/AtomUI.Desktop.Controls/Masonry/MasonryLayoutChangedEventArgs.cs`：布局结果事件参数。
- `src/AtomUI.Desktop.Controls/Masonry/Themes/MasonryTheme.axaml`：默认 ControlTheme，装配 `ItemsPresenter` 和 `MasonryPanel`。
- `src/AtomUI.Desktop.Controls/Masonry/Themes/MasonryThemes.axaml`：主题资源聚合。

## 相关文档

- 源设计文档：`docs/controls/desktop/layout/masonry/overview.md`
- 实现文档：`docs/controls/desktop/layout/masonry/implementation.md`
- 变更记录：`docs/controls/desktop/layout/masonry/changelog.md`
- 语义结构：`./semantic-cn.md`
