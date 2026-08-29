# Card

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Card 是桌面端数据展示类控件，用于在独立的内容面板中组织信息。它提供标题、额外操作、封面媒体、内容区域、底部操作区、元信息、栅格内容、标签页内容、加载占位、悬停反馈和无边框样式。

Card 的职责是提供稳定的信息容器视觉和组合入口。它不负责集合筛选、排序、虚拟化、表单提交、复杂选择、弹层管理或远程数据加载；这些能力应由业务层、ListView、DataGrid、Form、Popup/Flyout 或专用控件承担。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Card` |
| 状态 | Stable |

## 何时使用

Card 的设计语言来自 参考设计体系的卡片容器：一个有明确边界的内容表面承载同一主题的信息，Header 表示信息组名称，Extra 表示辅助操作，Cover 表示主要媒体，Actions 表示底部轻量操作。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 信息容器 | 用边框、圆角、背景和阴影形成独立面板。 | `StyleVariant`、`BoxShadow`、`BorderThickness`、`CornerRadius`。 |
| 层级标题 | 用 Header/Extra 表示信息组标题和辅助入口。 | `Header`、`HeaderTemplate`、`Extra`、`ExtraTemplate`。 |
| 媒体展示 | 用封面区域承载图片或自定义媒体。 | `Cover`、`CoverTemplate`。 |
| 内容形态 | 支持普通内容、元信息、栅格内容和标签页内容。 | `Content`、`CardMetaContent`、`CardGridContent`、`CardTabsContent`。 |
| 操作区 | 底部均分操作按钮，适合轻量命令。 | `Actions`、`CardActionButton`。 |
| 密度规格 | 用尺寸控制 Header 和内容 padding。 | `SizeType`。 |
| 加载反馈 | 内容获取过程中显示 Skeleton 占位。 | `IsLoading`。 |

## 公共 API

Card 的公共 API 分为根容器属性、内容扩展入口和子控件家族。

根容器属性：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Extra` / `ExtraTemplate` | `object?` / `IDataTemplate?` | Header 右侧辅助内容及模板。 |
| `StyleVariant` | `CardStyleVariant` | 卡片外观，`Outlined` 使用边框，`Borderless` 使用无边框阴影。 |
| `SizeType` | `SizeType` | 尺寸规格，默认 `Middle`，影响 Header 和内容 padding。 |
| `IsLoading` | `bool` | 是否在内容区域显示 Skeleton 加载占位。 |
| `IsInnerMode` | `bool` | 内部卡片模式，用于嵌套信息结构。 |
| `IsHoverable` | `bool` | 是否启用指针悬停阴影和手型光标。 |
| `Cover` / `CoverTemplate` | `object?` / `IDataTemplate?` | 顶部封面内容及模板。 |
| `Actions` | `Controls` | 底部操作区集合。 |
| `BoxShadow` | `BoxShadows` | 卡片表面阴影，通常由主题和状态设置。 |
| `IsMotionEnabled` | `bool` | 是否启用 Card 相关过渡动效。 |

`CardStyleVariant` 的稳定取值：

| 值 | 语义 |
| --- | --- |
| `Outlined` | 默认有边框卡片。 |
| `Borderless` | 无边框卡片，使用阴影表达容器边界。 |

子控件家族：

| 类型 | 职责 |
| --- | --- |
| `CardMetaContent` | 承载头像、标题和描述内容的元信息布局。 |
| `CardGridContent` | 在 Card body 中生成 Grid 面板，承载 `CardGridItem`。 |
| `CardGridItem` | 栅格内容项，支持行列位置、span、hover 和尺寸同步。 |
| `CardTabsContent` | 在 Card 中承载 TabControl 内容和 tab bar extra。 |
| `CardActionButton` | 底部操作区推荐按钮类型，基于 `IconButton`。 |

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ActionPanel` | `CardActionPanel` | 底部 Actions 的实际布局和渲染承载。 |
| `PART_ItemsPresenter` | `ItemsPresenter` | `CardGridContent` 中 Grid items panel 的接入点。 |
| `PART_TabControl` | `TabControl` | `CardTabsContent` 内部 TabControl 接入点。 |

稳定伪类和内部状态：

| 状态 | 语义 |
| --- | --- |
| `:headerless` | Header、HeaderTemplate、Extra 和 ExtraTemplate 都为空时启用，用于隐藏 Header 区域。 |
| `ContentType` | 内部状态，区分 `Default`、`Meta`、`Grid` 和 `Tabs` 内容形态，驱动主题 padding、圆角和边框。 |

Card 没有专用 public routed event 或命令。

## 事件与命令

Card 没有专用 public routed event 或命令。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 带标签页

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Card/Views/CardAdvancedShowCase.axaml:54`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:Card Header="卡片标题" HorizontalAlignment="Stretch" SizeType="Large">
        <atom:Card.Extra>
            <atom:HyperLinkButton Content="更多" />
        </atom:Card.Extra>
        <atom:CardTabsContent>
            <atom:TabItem Header="标签页 1" Content="内容 1" />
            <atom:TabItem Header="标签页 2" Content="内容 2" />
        </atom:CardTabsContent>
    </atom:Card>

    <atom:Card HorizontalAlignment="Stretch">
        <atom:CardTabsContent>
            <atom:CardTabsContent.TabBarExtraContent>
                <atom:HyperLinkButton Content="更多" />
            </atom:CardTabsContent.TabBarExtraContent>
            <atom:TabItem Header="文章" Content="文章内容" />
            <atom:TabItem Header="应用" Content="应用内容" />
            <atom:TabItem Header="项目" Content="项目内容" />
        </atom:CardTabsContent>
    </atom:Card>
</StackPanel>
```

### 基础卡片

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Card/Views/CardBasicShowCase.axaml:12`

Gallery key：`ExamplesContent` / item `0`

```axaml
<WrapPanel ItemSpacing="20" LineSpacing="20">
    <atom:Card Header="大尺寸卡片" SizeType="Large" Width="300">
        <atom:Card.Extra>
            <atom:HyperLinkButton Content="更多" />
        </atom:Card.Extra>
        <StackPanel Orientation="Vertical" Spacing="3">
            <atom:TextBlock Text="卡片内容" />
            <atom:TextBlock Text="卡片内容" />
            <atom:TextBlock Text="卡片内容" />
        </StackPanel>
    </atom:Card>

    <atom:Card Header="默认尺寸卡片" SizeType="Middle" Width="300">
        <atom:Card.Extra>
            <atom:HyperLinkButton Content="更多" />
        </atom:Card.Extra>
        <StackPanel Orientation="Vertical" Spacing="3">
            <atom:TextBlock Text="卡片内容" />
            <atom:TextBlock Text="卡片内容" />
            <atom:TextBlock Text="卡片内容" />
        </StackPanel>
    </atom:Card>

    <atom:Card Header="小尺寸卡片" SizeType="Small" Width="300">
        <atom:Card.Extra>
            <atom:HyperLinkButton Content="更多" />
        </atom:Card.Extra>
        <StackPanel Orientation="Vertical" Spacing="3">
            <atom:TextBlock Text="卡片内容" />
            <atom:TextBlock Text="卡片内容" />
            <atom:TextBlock Text="卡片内容" />
        </StackPanel>
    </atom:Card>
</WrapPanel>
```

### 无边框

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Card/Views/CardBasicShowCase.axaml:52`

Gallery key：`ExamplesContent` / item `1`

```axaml
<Border Padding="20" Background="{Binding BorderlessFrameBg}">
    <atom:Card Header="卡片标题" Width="300" StyleVariant="Borderless" HorizontalAlignment="Left">
        <atom:Card.Extra>
            <atom:HyperLinkButton Content="更多" />
        </atom:Card.Extra>
        <StackPanel Orientation="Vertical" Spacing="3">
            <atom:TextBlock Text="卡片内容" />
            <atom:TextBlock Text="卡片内容" />
            <atom:TextBlock Text="卡片内容" />
        </StackPanel>
    </atom:Card>
</Border>
```

### 简单卡片

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Card/Views/CardBasicShowCase.axaml:69`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:Card Width="300" HorizontalAlignment="Left">
    <StackPanel Orientation="Vertical" Spacing="3">
        <atom:TextBlock Text="卡片内容" />
        <atom:TextBlock Text="卡片内容" />
        <atom:TextBlock Text="卡片内容" />
    </StackPanel>
</atom:Card>
```

## 状态模型

Card 的内容类型状态流：

```text
Content changed
  -> inspect direct content type
  -> ContentType = Meta / Grid / Tabs / Default
  -> sync border, corner radius and body padding
```

`ContentType` 只探测直接赋给 `Content` 的对象。`CardMetaContent`、`CardGridContent` 和 `CardTabsContent` 应直接作为 Card 内容使用；把它们包裹在其他容器中时，Card 按普通内容处理。

Actions 状态流：

```text
Card.Actions changed
  -> PART_ActionPanel.Actions
  -> UniformGrid columns = action count
  -> IsActionsPanelVisible
```

加载状态：

- `IsLoading=true` 时，根模板中的 Skeleton 负责展示加载占位，并保留内容区域的主题边界。
- `IsLoading=false` 时显示实际 `Content` 和 `ContentTemplate`。

悬停和动效：

- `IsHoverable=true` 时，Card 进入 pointer over 后使用 `CardShadows`，并隐藏边框以避免边框和阴影同时强化。
- `IsMotionEnabled=true` 时启用阴影和操作区文字颜色过渡。
- 初始加载时过渡会先禁用，再在 Loaded 后启用，避免首次渲染出现不必要动画。

## 主题与 Design Token

Card 的默认视觉由根 Card 主题和多个子控件主题组成。

| 主题文件 | 职责 |
| --- | --- |
| `CardTheme.axaml` | 根模板、Header/Extra、Cover、Content、Skeleton、ActionPanel 接入和 Card 状态样式。 |
| `CardActionPanelTheme.axaml` | 底部操作区背景、边框、圆角、最小高度和文字颜色过渡。 |
| `CardActionButtonTheme.axaml` | Card 操作按钮的拉伸布局、图标尺寸和图标颜色。 |
| `CardGridContentTheme.axaml` | 栅格内容 ItemsPresenter 和 Grid items panel。 |
| `CardGridItemTheme.axaml` | 栅格项内容承载、padding、阴影和 hover 状态。 |
| `CardTabsContentTheme.axaml` | 内部 TabControl、ContentPadding 和 header edge padding。 |
| `CardMetaContentTheme.axaml` | Avatar、标题和描述的元信息布局。 |

Token 关系：

```text
SharedToken
   ↓
CardToken
   ↓
CardTheme / CardActionPanelTheme / CardGridItemTheme / CardTabsContentTheme / CardMetaContentTheme
```

根容器边框、圆角、背景和部分文字色来自 SharedToken。Header 高度、字体、padding、body padding、操作区背景、tabs margin、extra 色、卡片阴影、操作图标尺寸和 grid item 阴影来自 CardToken。

Token 来源：

CardToken 是 Card 的控件级 Token scope，描述卡片 Header、Body、Actions、Tabs、Extra、阴影、Grid item 和 action icon 的组件语义值。

CardToken 不承载以下状态：

- `Content`、`Header`、`Extra`、`Cover`、`Actions` 等实际内容。
- `ContentType`、`IsLoading`、`IsHoverable`、`IsInnerMode`、`StyleVariant` 等实例行为状态。
- `HeaderBorderThickness`、`EffectiveBorderThickness`、`EffectiveCornerRadius`、`IsActionsPanelVisible` 等内部派生状态。
- `CardGridItem.Row`、`Column`、`RowSpan`、`ColumnSpan` 等布局位置。
- `TabControl` 的 selected item、current content 或 tab collection。

## AOT 与裁剪注意事项

Card 不依赖运行时反射发现模板结构。模板协作通过固定 part 名称、强类型控件和 Avalonia property binding 完成。

资源与生命周期边界：

- `CardToken` 通过 token generator 注册，Theme 通过 `CardTokenResource` 使用。
- `CardActionButton`、`CardGridItem`、`CardMetaContent` 等是 Visual 控件，Token resource 由主题和视觉树生命周期管理。
- `Card` 到动态 `Content` 的 Size/Motion 同步使用 `BindUtils.RelayBind`，因为目标不是稳定 template part；绑定 owner 是 `_contentStateBindings`。
- `CardGridContent.PrepareCardGridItem` 的扩展 disposable 由 `_cardGridItemDisposables` 管理，并在 container clear/recycle 时释放。
- 不在 layout hot path 中读取全局资源或创建反射路径。

AOT 边界：

- 不新增字符串 path binding、`ReflectionBinding`、assembly scan、`Activator.CreateInstance(Type)` 或动态成员访问。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护。
- Card 的动态内容同步只使用 `AvaloniaProperty` 强类型绑定。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Card/Card.cs`：根 Card 公共 API、Actions 集合、内容类型识别、Header/Content 边框和圆角派生状态、Size/Motion 内容同步。
- `src/AtomUI.Desktop.Controls/Card/CardActionPanel.cs`：内部操作区，承载 Actions，维护 UniformGrid children，并渲染分隔线。
- `src/AtomUI.Desktop.Controls/Card/CardActionButton.cs`：Card 操作区推荐按钮类型，提供主题 key 边界。
- `src/AtomUI.Desktop.Controls/Card/CardMetaContent.cs`：头像、标题和描述内容的元信息承载控件。
- `src/AtomUI.Desktop.Controls/Card/CardGridContent.cs`：Card grid 内容的 ItemsControl，负责 Grid panel 行列定义同步和容器准备/清理。
- `src/AtomUI.Desktop.Controls/Card/CardGridItem.cs`：Grid item 容器，负责 content、row/column/span 和 hover/size/motion 状态。
- `src/AtomUI.Desktop.Controls/Card/CardTabsContent.cs`：Card tabs 内容桥接控件，负责把 Items 集合复制到内部 TabControl。
- `src/AtomUI.Desktop.Controls/Card/CardPseudoClass.cs`：Card 稳定伪类常量。
- `src/AtomUI.Desktop.Controls/Card/CardToken.cs`：Card 控件 Token。
- `src/AtomUI.Desktop.Controls/Card/Themes/*.axaml`：根主题、操作区、操作按钮、Meta、Grid、Tabs 等主题资源。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/card/overview.md`
- 实现文档：`docs/controls/desktop/data-display/card/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/card/token.md`
- 变更记录：`docs/controls/desktop/data-display/card/changelog.md`
- 语义结构：`./semantic-cn.md`
