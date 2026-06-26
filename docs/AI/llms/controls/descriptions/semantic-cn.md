# Descriptions 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Descriptions` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Descriptions/Themes/DescriptionsTheme.axaml`

```xml
<StackPanel>
    <DockPanel Name="HeaderLayout">
        <ContentPresenter Name="ExtraPresenter" />
        <ContentPresenter Name="HeaderPresenter" />
    </DockPanel>
    <Border Name="ContentFrame">
        <Grid Name="PART_GridLayout" />
    </Border>
</StackPanel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Descriptions
  -> DescriptionBorderedItemContent (control theme, DescriptionBorderedItemContentTheme.axaml)
     -> ContentPresenter#ContentPresenter (internal-observable)
  -> DescriptionBorderedItemLabel (control theme, DescriptionBorderedItemLabelTheme.axaml)
     -> ContentPresenter#ContentPresenter (internal-observable)
  -> DescriptionDefaultItem (item container control theme, DescriptionDefaultItemTheme.axaml)
     -> DockPanel (template-stable)
        -> ContentPresenter#Label (internal-observable)
        -> TextBlock#Colon (template-stable)
        -> ContentPresenter#Content (internal-observable)
     -> DockPanel (template-stable)
        -> StackPanel (template-stable)
           -> ContentPresenter#Label (internal-observable)
           -> TextBlock#Colon (template-stable)
        -> ContentPresenter#Content (internal-observable)
     -> Border (template-stable)
        -> DockPanel (template-stable)
           -> ContentPresenter#Label (internal-observable)
           -> Rectangle#Separator (template-stable)
           -> ContentPresenter#Content (internal-observable)
  -> Descriptions (control theme, DescriptionsTheme.axaml)
     -> StackPanel (template-stable)
        -> DockPanel#HeaderLayout (template-stable)
           -> ContentPresenter#ExtraPresenter (internal-observable)
           -> ContentPresenter#HeaderPresenter (internal-observable)
        -> Border#ContentFrame (template-stable)
           -> Grid#PART_GridLayout (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Descriptions` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `DescriptionBorderedItemContent` | control theme | `DescriptionBorderedItemContentTheme.axaml` | Descriptions | `BorderBrush`, `Content`, `EffectiveBorderThickness`, `LineHeight`, `Padding` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `DescriptionBorderedItemContentTheme.axaml` | DescriptionBorderedItemContent | `BorderBrush`, `Content`, `EffectiveBorderThickness`, `LineHeight`, `Padding` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `DescriptionBorderedItemLabel` | control theme | `DescriptionBorderedItemLabelTheme.axaml` | Descriptions | `Background`, `BorderBrush`, `Content`, `EffectiveBorderThickness`, `LineHeight`, `Padding` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `DescriptionBorderedItemLabelTheme.axaml` | DescriptionBorderedItemLabel | `Background`, `BorderBrush`, `Content`, `EffectiveBorderThickness`, `LineHeight`, `Padding` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `DescriptionDefaultItem` | item container control theme | `DescriptionDefaultItemTheme.axaml` | Descriptions | `BorderBrush`, `Content`, `EffectiveBorderThickness`, `Header`, `IsColonVisible`, `LineHeight` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `DockPanel` | template node (DockPanel) | `DescriptionDefaultItemTheme.axaml` | DescriptionDefaultItem | `Content`, `Header`, `IsColonVisible`, `LineHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Label` | template node (ContentPresenter) | `DescriptionDefaultItemTheme.axaml` | DescriptionDefaultItem | `Header`, `LineHeight` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Colon` | template node (TextBlock) | `DescriptionDefaultItemTheme.axaml` | DescriptionDefaultItem | `IsColonVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Content` | template node (ContentPresenter) | `DescriptionDefaultItemTheme.axaml` | DescriptionDefaultItem | `Content`, `LineHeight` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `StackPanel` | template node (StackPanel) | `DescriptionDefaultItemTheme.axaml` | DescriptionDefaultItem | `Header`, `IsColonVisible`, `LineHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Separator` | template node (Rectangle) | `DescriptionDefaultItemTheme.axaml` | DescriptionDefaultItem | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Descriptions` | control theme | `DescriptionsTheme.axaml` | 用户代码 / 控件宿主 | `Extra`, `ExtraTemplate`, `Header`, `HeaderTemplate`, `IsHeaderLayoutVisible` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `StackPanel` | template node (StackPanel) | `DescriptionsTheme.axaml` | Descriptions | `Extra`, `ExtraTemplate`, `Header`, `HeaderTemplate`, `IsHeaderLayoutVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderLayout` | template node (DockPanel) | `DescriptionsTheme.axaml` | Descriptions | `Extra`, `ExtraTemplate`, `Header`, `HeaderTemplate`, `IsHeaderLayoutVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ExtraPresenter` | template node (ContentPresenter) | `DescriptionsTheme.axaml` | Descriptions | `Extra`, `ExtraTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `HeaderPresenter` | template node (ContentPresenter) | `DescriptionsTheme.axaml` | Descriptions | `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentFrame` | template node (Border) | `DescriptionsTheme.axaml` | Descriptions | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_GridLayout` | template node (Grid) | `DescriptionsTheme.axaml` | Descriptions | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `HeaderLayout` | `DockPanel` | Header/Extra 行容器，固定存在，通过 `IsHeaderLayoutVisible` 控制显示。 |
| `ExtraPresenter` | `ContentPresenter` | Extra 内容和模板承载。 |
| `HeaderPresenter` | `ContentPresenter` | Header 内容和模板承载。 |
| `ContentFrame` | `Border` | 内容区域边框、圆角和裁剪承载。 |
| `PART_GridLayout` | `Grid` | 生成的描述项视觉子控件布局容器。 |

## Pseudo Classes

Descriptions 没有 public routed event、命令或专用伪类。内部生成的 `DescriptionDefaultItem`、`DescriptionBorderedItemLabel` 和 `DescriptionBorderedItemContent` 是主题承载类型，不是公开用户 API。

## State Flow

Descriptions 的核心状态流：

```text
ItemsSource / Items
      ↓
DescriptionItem collection
      ↓
generated item controls
      ↓
responsive column count + item span
      ↓
PART_GridLayout row/column/column-span
```

列数解析：

- `ColumnInfo` 为空时，断点默认列数为 `ExtraSmall=1`、`Small=2`、`ExtraExtraExtraLarge=4`，其他断点为 `3`。
- `ColumnInfo` 不为空时，按 `ResponsiveInt.Resolve()` 使用移动端优先级解析列数。
- 水平边框模式下内部有效列数为展示列数的两倍，因为 label 和 content 分别占 grid cell。
- 非水平边框模式下内部有效列数等于展示列数。

布局语义：

- 普通水平模式使用一个 `DescriptionDefaultItem` 展示 label、冒号和 content。
- 水平边框模式为每个 item 生成一个 label cell 和一个 content cell。
- 纵向模式使用一个 `DescriptionDefaultItem`，label 在上、content 在下；当 `IsBordered=true` 时通过模板显示内部边框和分隔线。
- `Span` 限制在当前行剩余列范围内，最小为 `1`。
- 最后一个 item 或 `IsFilled=true` 的 item 会填满当前行剩余列。

Header/Extra 状态：

```text
Header != null || Extra != null
      ↓
IsHeaderLayoutVisible
      ↓
HeaderLayout.IsVisible
```

## Theme and Token Boundaries

Descriptions 的默认视觉由根主题、默认项主题和边框 cell 主题组成。

| 主题文件 | 职责 |
| --- | --- |
| `DescriptionsTheme.axaml` | 根模板、Header/Extra、ContentFrame、`PART_GridLayout`、边框模式内容框和非边框 RowSpacing。 |
| `DescriptionDefaultItemTheme.axaml` | 普通项 horizontal/vertical/vertical bordered 三种模板和冒号、label、content 视觉。 |
| `DescriptionBorderedItemLabelTheme.axaml` | 水平边框模式 label cell 视觉。 |
| `DescriptionBorderedItemContentTheme.axaml` | 水平边框模式 content cell 视觉。 |
| `DescriptionsThemes.axaml` | 汇总 Descriptions 相关主题资源。 |

Token 关系：

```text
SharedToken
   ↓
DescriptionsToken
   ↓
DescriptionsTheme / DescriptionDefaultItemTheme / bordered cell themes
```

根 `ContentFrame` 的边框、圆角、裁剪来自 SharedToken。label 背景、label/content/title/extra 颜色、Header margin、item padding 和冒号 margin 来自 DescriptionsToken。

Token 边界：

DescriptionsToken 是 Descriptions 的组件级 Token scope，描述描述列表的 label 背景、文本颜色、标题颜色、Header 间距、item padding、冒号间距、内容颜色和 Extra 颜色。

DescriptionsToken 不承载以下状态：

- `Items`、`ItemsSource`、`DescriptionItem.Content` 等数据状态。
- `ColumnInfo`、当前断点、有效列数、row/column/column-span 等布局状态。
- `IsBordered`、`Layout`、`IsShowColon`、`IsFilled`、`Span` 等实例行为状态。
- `Header`、`Extra` 的实际内容或模板。
- `IsLastRow`、`IsLastColumn`、`EffectiveBorderThickness` 等生成视觉派生状态。

## Customization Boundaries

维护 Descriptions 时必须保持以下不变量：

- `Items` 是布局主数据源，`ItemsSource` 只物化其中的 `DescriptionItem`。
- `DescriptionItem` 是非视觉 `AvaloniaObject` 描述对象，布局必须按集合位置和对象引用维护生成视觉，不能依赖值相等或重新构造对象后的隐式匹配。
- `DescriptionItem` 属性变化必须刷新对应生成视觉；影响布局的 `Span`、`IsFilled` 变化必须触发布局重算。
- `DescriptionItem` 被移除、集合替换、模板重建或控件 detach 时，item 属性订阅、binding 转接、item 到视觉控件映射必须成对释放。
- `DescriptionItem` 不允许直接使用 `DynamicResource` 或 token-resource binding，除非它拥有经过测试的 scoped `IResourceHost` / `IThemeVariantHost` 生命周期。
- `IsBordered` 或 `Layout` 改变时，必须按新视觉模式重建生成子控件。
- 水平边框模式必须为每个 item 生成 label/content 两个 cell。
- 普通水平和纵向非边框模式必须保持 `IsShowColon` 到冒号显示状态的绑定。
- `HeaderLayout` 是固定模板节点，Header/Extra 为空时隐藏而不是销毁。
- `PART_GridLayout`、`HeaderLayout`、`HeaderPresenter`、`ExtraPresenter`、`ContentFrame` 的名称和职责不能在未授权情况下改变。
- `SizeType` 默认值保持 `Large`。
- Token 名称和语义不擅自重命名或删除。
- 媒体断点订阅必须在 detach 时释放。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- `Items` 集合订阅必须在集合替换时成对解除和重新订阅。
- `DescriptionItem` 属性订阅必须在 item remove、reset、Items 替换、template reapply 和 detach 时释放。
- item 到 generated control 的映射必须随生成视觉重建清理，不能保留旧视觉控件。
- `MediaBreakPointChanged` 必须在 detach 时解除。
- `IsBordered` 和 `Layout` 变化必须重建生成视觉。
- 水平边框模式的 grid child 数量必须是 `Items.Count * 2`。
- 非水平边框模式的 grid child 数量必须是 `Items.Count`。
- 布局必须按集合位置定位 item，不能按值查找。
- `IsFilled` 和最后 item 必须填满当前行剩余列。
- `IsShowColon` 必须能在生成视觉存在期间重复更新。
- `HeaderLayout` 固定存在，通过 `IsHeaderLayoutVisible` 控制显示。
- 内部 marker 类型 `DescriptionBorderedItemLabel` 和 `DescriptionBorderedItemContent` 不能作为“空类”随意删除；它们承担主题选择器边界。
- `DescriptionItem` 不能升级成视觉控件，也不能永久持有 generated control、owner container 或 ShowCase。
- 非视觉 `DescriptionItem` 承载动态资源前必须补 scoped resource host 和资源生命周期测试。
