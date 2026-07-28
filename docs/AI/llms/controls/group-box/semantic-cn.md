# GroupBox 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `GroupBox` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/GroupBox/Themes/GroupBoxTheme.axaml`

```xml
<Border Name="PART_Frame">
    <DockPanel>
        <Panel Name="PART_HeaderContainer">
            <Decorator Name="PART_HeaderContent">
                <StackPanel>
                    <IconPresenter Name="PART_HeaderIconPresenter" />
                    <TextBlock Name="PART_HeaderPresenter" />
                </StackPanel>
            </Decorator>
        </Panel>
        <ContentPresenter Name="PART_ContentPresenter" />
    </DockPanel>
</Border>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
GroupBox
  -> GroupBox (control theme, GroupBoxTheme.axaml)
     -> Border#PART_Frame (template-stable)
        -> DockPanel (template-stable)
           -> Panel#PART_HeaderContainer (template-stable)
              -> Decorator#PART_HeaderContent (template-stable)
                 -> StackPanel (template-stable)
                    -> IconPresenter#PART_HeaderIconPresenter (template-stable)
                    -> TextBlock#PART_HeaderPresenter (template-stable)
           -> ContentPresenter#PART_ContentPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `GroupBox` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `GroupBox` | control theme | `GroupBoxTheme.axaml` | 用户代码 / 控件宿主 | `Content`, `ContentTemplate`, `CornerRadius`, `HeaderFontSize`, `HeaderFontStyle`, `HeaderFontWeight` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_Frame` | template node (Border) | `GroupBoxTheme.axaml` | GroupBox | `Content`, `ContentTemplate`, `CornerRadius`, `HeaderFontSize`, `HeaderFontStyle`, `HeaderFontWeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `GroupBoxTheme.axaml` | GroupBox | `Content`, `ContentTemplate`, `HeaderFontSize`, `HeaderFontStyle`, `HeaderFontWeight`, `HeaderIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderContainer` | template node (Panel) | `GroupBoxTheme.axaml` | GroupBox | `HeaderFontSize`, `HeaderFontStyle`, `HeaderFontWeight`, `HeaderIcon`, `HeaderTitle`, `HeaderTitleColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderContent` | template node (Decorator) | `GroupBoxTheme.axaml` | GroupBox | `HeaderFontSize`, `HeaderFontStyle`, `HeaderFontWeight`, `HeaderIcon`, `HeaderTitle`, `HeaderTitleColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `GroupBoxTheme.axaml` | GroupBox | `HeaderFontSize`, `HeaderFontStyle`, `HeaderFontWeight`, `HeaderIcon`, `HeaderTitle`, `HeaderTitleColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderIconPresenter` | template node (IconPresenter) | `GroupBoxTheme.axaml` | GroupBox | `HeaderIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderPresenter` | template node (TextBlock) | `GroupBoxTheme.axaml` | GroupBox | `HeaderFontSize`, `HeaderFontStyle`, `HeaderFontWeight`, `HeaderTitle`, `HeaderTitleColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `GroupBoxTheme.axaml` | GroupBox | `Content`, `ContentTemplate`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Frame` | `Border` | 承载整体布局根节点，并提供边框 bounds 参考。 |
| `PART_HeaderContainer` | `Panel` | Header 行容器。 |
| `PART_HeaderContent` | `Decorator` | Header 内容实际 bounds，用于标题缺口和对齐计算。 |
| `PART_HeaderIconPresenter` | `IconPresenter` | Header 图标展示。 |
| `PART_HeaderPresenter` | `TextBlock` | Header 标题展示。 |
| `PART_ContentPresenter` | `ContentPresenter` | 内容承载。 |

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

GroupBox 自身没有 hover、pressed、loading、selected、expanded 或 checked 状态。它不拦截输入事件，也不为 Header 提供默认点击行为。

GroupBox 的有效状态来自公共属性、模板测量结果和主题资源：

```text
HeaderTitle / HeaderIcon / HeaderTitlePosition
      + Header font properties
      + template measured bounds
        ↓
Header content bounds
        ↓
Frame border bounds + Header gap bounds
        ↓
Background / BorderBrush / BorderThickness / CornerRadius render state
```

`HeaderIcon`、Header 字体、标题位置和 Header 内容变化会影响缺口尺寸或位置。内容尺寸变化会通过模板根 `PART_Frame` 参与 GroupBox 的 measure pass，使自动高度随内容 `DesiredSize` 增长，同时仍尊重父容器可用空间和显式高度约束。`Background`、`BorderBrush`、`BorderThickness`、`CornerRadius` 改变会影响自绘边框和背景。

## Theme and Token Boundaries

GroupBox 的默认 Theme 位于 `src/AtomUI.Desktop.Controls/GroupBox/Themes/GroupBoxTheme.axaml`。Theme 提供 Header、Frame 和 Content 的可测量结构，并设置默认背景、边框、圆角、标题颜色、标题字号和间距。

标题缺口是 GroupBox 的视觉契约：Header 内容区域必须从上边框中形成缺口。缺口不能依赖用 `Background` 覆盖边框线，因为 `Background` 允许为透明或半透明。

Theme 职责：

- 设置 `Background`、`BorderBrush`、`BorderThickness`、`CornerRadius` 默认值。
- 设置 `HeaderTitleColor`、`HeaderFontSize`。
- 通过 GroupBox Token 设置 Header 容器外边距、Header 内容内边距、Header 图标间距和内容内边距。
- 根据 `HeaderTitlePosition` 设置 `PART_HeaderContent` 的水平对齐。

Theme 不负责动态构建 Header 或绘制边框。Header 缺口属于控件渲染模型。

Token 边界：

GroupBox Token 将全局 SharedToken 转换为 GroupBox 可消费的组件级结构值，主要覆盖 Header 和内容区域的间距。颜色、边框厚度、圆角和字体基础值直接来自 SharedToken，不在 GroupBox Token 中重复定义。

GroupBox Token 不表达实例状态，也不负责 Header 缺口的运行时 bounds。Header 缺口由模板测量结果和控件渲染模型共同决定。

## Customization Boundaries

维护 GroupBox 时必须保持以下不变量：

- `HeaderTitle`、`HeaderTitleColor`、`HeaderIcon`、`HeaderTitlePosition`、`HeaderFontSize`、`HeaderFontStyle`、`HeaderFontWeight` 的 API 名称、类型和默认语义不变。
- `GroupBoxTitlePosition.Left`、`Right`、`Center` 的名称和含义不变。
- `PART_Frame`、`PART_HeaderContainer`、`PART_HeaderContent`、`PART_HeaderIconPresenter`、`PART_HeaderPresenter`、`PART_ContentPresenter` 的 template part 名称不变。
- `Background="Transparent"` 时内容区保持透明，同时 Header 标题下方不应出现边框短线。
- 未设置显式高度时，GroupBox 的 `DesiredSize.Height` 必须包含 Header 通道、内容内边距和内容自身期望高度，避免内容多时被 Header 或边框区域挤压。
- Header 图标为 `null` 时图标节点不可见，不保留额外图标占位宽度。
- Header 内容位置改变只影响 Header 水平对齐，不改变内容区域布局语义。
- Token 名称和语义不擅自重命名或删除。

维护不变量：

内部重构必须保持以下不变量：

- Header 缺口计算基于 `PART_HeaderContent` 的实际 bounds。
- `PART_Frame` 保持边框绘制的布局参考。
- `PART_Frame` 必须参与 GroupBox 测量，自动高度不能退化为只测量裸 Content。
- 不用 Header 背景遮挡边框线来模拟缺口。
- 透明背景、半透明背景和普通背景走同一渲染模型。
- Header 图标隐藏时不保留额外图标占位。
- GroupBox 不新增点击、折叠或选择行为。
- Token 只表达布局和视觉默认值，不承载实例 bounds 或渲染缓存。
