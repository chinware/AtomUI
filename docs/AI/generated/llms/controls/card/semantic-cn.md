# Card 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

`Card` 与 `CardMetaContent` 是两个独立 public owner。同名 `title` 表示相近的产品职责，不表示两个 owner 共享模板节点、
状态或样式作用域。

### 1.1 `Card`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `Card` |
| Part | `root` |
| Selector | Card 本身 |
| ContractType | `Card` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Card owner |
| 职责 | Card root 是外观、尺寸、加载、悬停和内容组合的统一 owner。 |
| 相关 API | 全部 Card public API |
| 相关 Token | CardToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `header`

| 字段 | 值 |
| --- | --- |
| Owner | `Card` |
| Part | `header` |
| Selector | `.semantic-header` |
| ContractType | `DashedBorder` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Header frame |
| 职责 | 表示完整头部表面和标题、额外内容的共同布局边界。 |
| 相关 API | `Header`、`HeaderTemplate`、`Extra`、`ExtraTemplate`、`SizeType`、`IsInnerMode` |
| 相关 Token | Header、Extra、Border、Radius Token |
| 稳定性 | stable since 6.0 |

#### `title`

| 字段 | 值 |
| --- | --- |
| Owner | `Card` |
| Part | `title` |
| Selector | `.semantic-title` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Header title presenter |
| 职责 | 表示 Card 标题的展示与排版区域。 |
| 相关 API | `Header`、`HeaderTemplate` |
| 相关 Token | Header typography Token |
| 稳定性 | stable since 6.0 |

#### `extra`

| 字段 | 值 |
| --- | --- |
| Owner | `Card` |
| Part | `extra` |
| Selector | `.semantic-extra` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Header extra presenter |
| 职责 | 表示头部尾侧的辅助内容区域。 |
| 相关 API | `Extra`、`ExtraTemplate` |
| 相关 Token | `ExtraColor`、Header padding Token |
| 稳定性 | stable since 6.0 |

#### `cover`

| 字段 | 值 |
| --- | --- |
| Owner | `Card` |
| Part | `cover` |
| Selector | `.semantic-cover` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Cover frame |
| 职责 | 表示封面内容的裁剪、圆角和布局边界。 |
| 相关 API | `Cover`、`CoverTemplate` |
| 相关 Token | Shared radius Token |
| 稳定性 | stable since 6.0 |

#### `body`

| 字段 | 值 |
| --- | --- |
| Owner | `Card` |
| Part | `body` |
| Selector | `.semantic-body` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Body frame |
| 职责 | 表示普通、Meta、Grid、Tabs 和 Loading 内容共享的主体表面。 |
| 相关 API | `Content`、`ContentTemplate`、`IsLoading`、`SizeType` |
| 相关 Token | Body padding Token |
| 稳定性 | stable since 6.0 |

#### `actions`

| 字段 | 值 |
| --- | --- |
| Owner | `Card` |
| Part | `actions` |
| Selector | `.semantic-actions` |
| ContractType | `TemplatedControl` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Action panel owner |
| 职责 | 表示底部操作组的完整表面、均分布局和分隔线边界。 |
| 相关 API | `Actions`、`IsMotionEnabled` |
| 相关 Token | Actions、Border、Radius Token |
| 稳定性 | stable since 6.0 |

### 1.2 `CardMetaContent`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `CardMetaContent` |
| Part | `root` |
| Selector | CardMetaContent 本身 |
| ContractType | `CardMetaContent` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | CardMetaContent owner |
| 职责 | Meta root 是头像、标题和描述组合的统一 owner。 |
| 相关 API | `Avatar`、`Header`、`HeaderTemplate`、`Content`、`ContentTemplate` |
| 相关 Token | Shared typography、spacing Token |
| 稳定性 | stable since 6.0 |

#### `section`

| 字段 | 值 |
| --- | --- |
| Owner | `CardMetaContent` |
| Part | `section` |
| Selector | `.semantic-section` |
| ContractType | `Control` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Meta detail section |
| 职责 | 表示标题与描述共同占用的详情布局区域。 |
| 相关 API | `Header`、`HeaderTemplate`、`Content`、`ContentTemplate` |
| 相关 Token | Shared spacing Token |
| 稳定性 | stable since 6.0 |

#### `avatar`

| 字段 | 值 |
| --- | --- |
| Owner | `CardMetaContent` |
| Part | `avatar` |
| Selector | `.semantic-avatar` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Avatar presenter |
| 职责 | 表示 Meta 头像内容的展示与布局区域。 |
| 相关 API | `Avatar` |
| 相关 Token | Shared spacing Token |
| 稳定性 | stable since 6.0 |

#### `title`

| 字段 | 值 |
| --- | --- |
| Owner | `CardMetaContent` |
| Part | `title` |
| Selector | `.semantic-title` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Meta title presenter |
| 职责 | 表示 Meta 标题的展示与排版区域。 |
| 相关 API | `Header`、`HeaderTemplate` |
| 相关 Token | Shared heading typography Token |
| 稳定性 | stable since 6.0 |

#### `description`

| 字段 | 值 |
| --- | --- |
| Owner | `CardMetaContent` |
| Part | `description` |
| Selector | `.semantic-description` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Meta description presenter |
| 职责 | 表示 Meta 描述内容的展示与排版区域。 |
| 相关 API | `Content`、`ContentTemplate` |
| 相关 Token | Shared description typography Token |
| 稳定性 | stable since 6.0 |

所有 root 都是隐式 Part，不添加 `.semantic-root`。`ContractType` 是 Setter 可以稳定依赖的最低 public 类型，并通过
`x:SetterTargetType` 提供 AXAML 编译期类型上下文；它不参与 `.semantic-*` 的身份匹配。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Card/Themes/CardTheme.axaml`

```xml
<PixelAlignedBorder Name="Frame">
    <DockPanel>
        <PixelAlignedBorder Name="HeaderFrame">
            <DockPanel>
                <ContentPresenter Name="HeaderExtra" />
                <ContentPresenter Name="TitlePresenter" />
            </DockPanel>
        </PixelAlignedBorder>
        <CardActionPanel Name="PART_ActionPanel" />
        <Border Name="CoverFrame">
            <ContentPresenter Name="CoverContentPresenter" />
        </Border>
        <Border Name="CardContent">
            <Skeleton />
        </Border>
    </DockPanel>
</PixelAlignedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Card
  -> CardActionButton (control theme, CardActionButtonTheme.axaml)
  -> CardActionPanel (control theme, CardActionPanelTheme.axaml)
     -> Border#Frame (template-stable)
        -> UniformGrid#PART_GridPanel (template-stable)
  -> CardGridContent (control theme, CardGridContentTheme.axaml)
     -> Border#Frame (template-stable)
        -> ItemsPresenter#PART_ItemsPresenter (template-stable)
  -> CardGridItem (item container control theme, CardGridItemTheme.axaml)
     -> Border#Frame (template-stable)
        -> ContentPresenter#ContentPresenter (internal-observable)
  -> CardMetaContent (control theme, CardMetaContentTheme.axaml)
     -> DockPanel (template-stable)
        -> ContentPresenter#AvatarContentPresenter (internal-observable)
        -> DockPanel (template-stable)
           -> ContentPresenter#TitleContentPresenter (internal-observable)
           -> ContentPresenter#DescriptionContentPresenter (internal-observable)
  -> CardTabsContent (control theme, CardTabsContentTheme.axaml)
     -> TabControl#PART_TabControl (template-stable)
  -> Card (control theme, CardTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> DockPanel (template-stable)
           -> PixelAlignedBorder#HeaderFrame (template-stable)
              -> DockPanel (template-stable)
                 -> ContentPresenter#HeaderExtra (internal-observable)
                 -> ContentPresenter#TitlePresenter (internal-observable)
           -> CardActionPanel#PART_ActionPanel (template-stable)
           -> Border#CoverFrame (template-stable)
              -> ContentPresenter#CoverContentPresenter (internal-observable)
           -> Border#CardContent (template-stable)
              -> Skeleton (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Card` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `CardActionButton` | control theme | `CardActionButtonTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `CardActionPanel` | control theme | `CardActionPanelTheme.axaml` | Card | `Background`, `CornerRadius` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (Border) | `CardActionPanelTheme.axaml` | CardActionPanel | `Background`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_GridPanel` | template node (UniformGrid) | `CardActionPanelTheme.axaml` | CardActionPanel | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CardGridContent` | control theme | `CardGridContentTheme.axaml` | 用户代码 / 控件宿主 | `Padding` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `CardGridContentTheme.axaml` | CardGridContent | `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `CardGridContentTheme.axaml` | CardGridContent | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CardGridItem` | item container control theme | `CardGridItemTheme.axaml` | 用户代码 / 控件宿主 | `BoxShadow`, `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `Padding`, `VerticalContentAlignment` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `CardGridItemTheme.axaml` | CardGridItem | `BoxShadow`, `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `Padding`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `CardGridItemTheme.axaml` | CardGridItem | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `CardMetaContent` | control theme | `CardMetaContentTheme.axaml` | 用户代码 / 控件宿主 | `Avatar`, `Content`, `ContentTemplate`, `Header`, `HeaderTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `DockPanel` | template node (DockPanel) | `CardMetaContentTheme.axaml` | CardMetaContent | `Avatar`, `Content`, `ContentTemplate`, `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `AvatarContentPresenter` | template node (ContentPresenter) | `CardMetaContentTheme.axaml` | CardMetaContent | `Avatar` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TitleContentPresenter` | template node (ContentPresenter) | `CardMetaContentTheme.axaml` | CardMetaContent | `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `DescriptionContentPresenter` | template node (ContentPresenter) | `CardMetaContentTheme.axaml` | CardMetaContent | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `CardTabsContent` | control theme | `CardTabsContentTheme.axaml` | 用户代码 / 控件宿主 | `IsMotionEnabled`, `SizeType`, `TabBarExtraContent`, `TabBarExtraContentTemplate`, `TabItemTemplate`, `TabItemsSource` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_TabControl` | template node (TabControl) | `CardTabsContentTheme.axaml` | CardTabsContent | `IsMotionEnabled`, `SizeType`, `TabBarExtraContent`, `TabBarExtraContentTemplate`, `TabItemTemplate`, `TabItemsSource` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Card` | control theme | `CardTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentTemplate`, `CornerRadius` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (PixelAlignedBorder) | `CardTheme.axaml` | Card | `Background`, `BorderBrush`, `BoxShadow`, `Content`, `ContentTemplate`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `CardTheme.axaml` | Card | `Content`, `ContentTemplate`, `CornerRadius`, `Cover`, `CoverTemplate`, `Extra` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderFrame` | template node (PixelAlignedBorder) | `CardTheme.axaml` | Card | `Extra`, `ExtraTemplate`, `Header`, `HeaderBorderThickness`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderExtra` | template node (ContentPresenter) | `CardTheme.axaml` | Card | `Extra`, `ExtraTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TitlePresenter` | template node (ContentPresenter) | `CardTheme.axaml` | Card | `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ActionPanel` | template node (CardActionPanel) | `CardTheme.axaml` | Card | `CornerRadius`, `IsActionsPanelVisible`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CoverFrame` | template node (Border) | `CardTheme.axaml` | Card | `CornerRadius`, `Cover`, `CoverTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CoverContentPresenter` | template node (ContentPresenter) | `CardTheme.axaml` | Card | `Cover`, `CoverTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `CardContent` | template node (Border) | `CardTheme.axaml` | Card | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `IsLoading`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ActionPanel` | `CardActionPanel` | 底部 Actions 的实际布局和渲染承载。 |
| `PART_ItemsPresenter` | `ItemsPresenter` | `CardGridContent` 中 Grid items panel 的接入点。 |
| `PART_TabControl` | `TabControl` | `CardTabsContent` 内部 TabControl 接入点。 |

## Pseudo Classes

稳定伪类和内部状态：

| 状态 | 语义 |
| --- | --- |
| `:headerless` | Header、HeaderTemplate、Extra 和 ExtraTemplate 都为空时启用，用于隐藏 Header 区域。 |
| `ContentType` | 内部状态，区分 `Default`、`Meta`、`Grid` 和 `Tabs` 内容形态，驱动主题 padding、圆角和边框。 |

Card 没有专用 public routed event 或命令。

## State Flow

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

## Theme and Token Boundaries

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

### 5.1 Semantic Part 支持摘要

Card 家族由两个独立 public owner 公开 Semantic Part：

| Owner | Parts | 职责摘要 |
| --- | --- | --- |
| `Card` | `root`、`header`、`title`、`extra`、`cover`、`body`、`actions` | 覆盖 Card 根表面、头部、标题、辅助内容、封面、主体和底部操作组。 |
| `CardMetaContent` | `root`、`section`、`avatar`、`title`、`description` | 覆盖 Meta 根区域、详情区、头像、标题和描述。 |

`CardGridContent`、`CardGridItem`、`CardTabsContent` 和 `CardActionButton` 是公开组合类型，但不由 Card 家族推导独立
Semantic Part descriptor。完整 Selector、ContractType、cardinality、状态矩阵和排除边界以
[Card Semantic Part 契约](semantic-part.md)为准。

Token 边界：

CardToken 是 Card 的控件级 Token scope，描述卡片 Header、Body、Actions、Tabs、Extra、阴影、Grid item 和 action icon 的组件语义值。

CardToken 不承载以下状态：

- `Content`、`Header`、`Extra`、`Cover`、`Actions` 等实际内容。
- `ContentType`、`IsLoading`、`IsHoverable`、`IsInnerMode`、`StyleVariant` 等实例行为状态。
- `HeaderBorderThickness`、`EffectiveBorderThickness`、`EffectiveCornerRadius`、`IsActionsPanelVisible` 等内部派生状态。
- `CardGridItem.Row`、`Column`、`RowSpan`、`ColumnSpan` 等布局位置。
- `TabControl` 的 selected item、current content 或 tab collection。

## Customization Boundaries

维护 Card 时必须保持以下不变量：

- `CardStyleVariant` 取值和默认 `Outlined` 语义不能擅自改变。
- `SizeType` 默认值保持 `Middle`。
- `Actions` 是 Card 底部操作区的公开集合入口，集合变更必须同步到 `PART_ActionPanel`。
- `:headerless` 必须只由 Header/Extra 四个入口共同决定。
- `ContentType` 必须随直接 `Content` 类型变化回到正确状态，不能在 Grid/Tabs/Meta 被替换后残留。
- `CardGridContent.ColumnDefinitions` 和 `RowDefinitions` 必须参与 Avalonia 属性系统，以支持 XAML、样式和绑定。
- `CardTabsContent` 和 `CardGridContent` 接收 Card 的 Size/Motion 状态时，旧内容被替换后必须释放同步绑定。
- `PrepareCardGridItem` 的 `CompositeDisposable` 必须由 container clear/recycle 生命周期释放。
- `PART_ActionPanel`、`PART_ItemsPresenter`、`PART_TabControl` 的名称和职责不能在未授权情况下改变。
- `CardActionButton` 和 `CardPseudoClass` 是主题契约入口，即使实现很薄也不能当作无用类型删除。
- Token 名称和语义不擅自重命名或删除。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- `ContentType` 必须覆盖 Meta、Grid、Tabs 和 Default 四个分支。
- 重新配置内容类型前必须释放旧内容状态绑定。
- `CardGridContent` 的行列定义 wrapper 必须读写 `ColumnDefinitionsProperty` 和 `RowDefinitionsProperty`。
- `PrepareCardGridItem` 传入的 `CompositeDisposable` 必须有清理路径。
- `CardActionPanel` 换模板时必须清空旧 panel children，避免 action visual 同时挂到多个父级。
- `CardTabsContent` 换模板时必须清空旧 TabControl items，再复制当前 Items。
- `Actions`、`CardTabsContent.Items` 的 Reset 行为当前为 `NotSupportedException`，不能在结构整理中静默改变。
- 初始 transition 禁用/加载后启用的顺序不能在未验证视觉影响时移除。
- 空实现或薄实现的主题语义类型不能随意删除。
- `Card` descriptor 必须只包含 `root/header/title/extra/cover/body/actions`，`CardMetaContent` descriptor 必须只包含
  `root/section/avatar/title/description`。
- Card 与 CardMetaContent 的同名 title marker 必须由 owner Selector 隔离，不能使用类型前缀编码 Part identity。
- headerless、空 Cover、空 Actions、Loading 和 ContentType 切换只能改变内容或可见性，不能改变静态 Part cardinality。
- 根 Frame 必须承载 Card 内部 DockPanel，使 Header 与 Actions 分隔线从外框内缘开始；不得恢复为同级覆盖绘制。
- Borderless 的零边框必须保留为 Theme 默认值，不能在 C# 派生状态中阻止 owner-scoped root style 覆盖 `BorderThickness`。
- Card actions marker 不能进入 internal CardActionPanel 的第二个模板边界；Grid、Tabs 和 action child 不得被 Card owner 穿透。
- 默认 Card Themes 不得消费 `.semantic-*`，避免为未使用该能力的 Card 增加动态 selector 成本。
- 布局型 Semantic Style 必须建立在一套完整的 `SizeType` 分支上；未覆盖属性继续使用该分支的 Token 基线，不得在示例中
  局部拼接不同尺寸档位的 Header `MinHeight`、字体与 Padding。
