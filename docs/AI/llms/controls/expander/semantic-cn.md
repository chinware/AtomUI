# Expander 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Expander` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Expander/Themes/ExpanderTheme.axaml`

```xml
<PixelAlignedBorder Name="PART_Frame">
    <DockPanel Name="PART_MainLayout">
        <LayoutTransformControl Name="PART_HeaderLayoutTransform">
            <PixelAlignedBorder Name="PART_HeaderDecorator">
                <Grid Name="PART_HeaderLayout">
                    <IconButton Name="PART_ExpandButton" />
                    <ContentPresenter Name="PART_HeaderPresenter" />
                    <ContentPresenter Name="PART_AddOnContentPresenter" />
                </Grid>
            </PixelAlignedBorder>
        </LayoutTransformControl>
        <LayoutAwareMotionActor Name="PART_ContentMotionActor">
            <PixelAlignedBorder>
                <ContentPresenter Name="PART_ContentPresenter" />
            </PixelAlignedBorder>
        </LayoutAwareMotionActor>
    </DockPanel>
</PixelAlignedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Expander
  -> Expander (control theme, ExpanderTheme.axaml)
     -> PixelAlignedBorder#PART_Frame (template-stable)
        -> DockPanel#PART_MainLayout (template-stable)
           -> LayoutTransformControl#PART_HeaderLayoutTransform (template-stable)
              -> PixelAlignedBorder#PART_HeaderDecorator (template-stable)
                 -> Grid#PART_HeaderLayout (template-stable)
                    -> IconButton#PART_ExpandButton (template-stable)
                    -> ContentPresenter#PART_HeaderPresenter (template-stable)
                    -> ContentPresenter#PART_AddOnContentPresenter (template-stable)
           -> LayoutAwareMotionActor#PART_ContentMotionActor (template-stable)
              -> PixelAlignedBorder (template-stable)
                 -> ContentPresenter#PART_ContentPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Expander` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Expander` | control theme | `ExpanderTheme.axaml` | 用户代码 / 控件宿主 | `AddOnContent`, `AddOnContentTemplate`, `Content`, `ContentBorderThickness`, `ContentPadding`, `ContentTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_Frame` | template node (PixelAlignedBorder) | `ExpanderTheme.axaml` | Expander | `AddOnContent`, `AddOnContentTemplate`, `Content`, `ContentBorderThickness`, `ContentPadding`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MainLayout` | template node (DockPanel) | `ExpanderTheme.axaml` | Expander | `AddOnContent`, `AddOnContentTemplate`, `Content`, `ContentBorderThickness`, `ContentPadding`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderLayoutTransform` | template node (LayoutTransformControl) | `ExpanderTheme.axaml` | Expander | `AddOnContent`, `AddOnContentTemplate`, `ExpandIcon`, `Header`, `HeaderPadding`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderDecorator` | template node (PixelAlignedBorder) | `ExpanderTheme.axaml` | Expander | `AddOnContent`, `AddOnContentTemplate`, `ExpandIcon`, `Header`, `HeaderPadding`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderLayout` | template node (Grid) | `ExpanderTheme.axaml` | Expander | `AddOnContent`, `AddOnContentTemplate`, `ExpandIcon`, `Header`, `HeaderTemplate`, `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ExpandButton` | template node (IconButton) | `ExpanderTheme.axaml` | Expander | `ExpandIcon`, `IsEnabled`, `IsMotionEnabled`, `IsShowExpandIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderPresenter` | template node (ContentPresenter) | `ExpanderTheme.axaml` | Expander | `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_AddOnContentPresenter` | template node (ContentPresenter) | `ExpanderTheme.axaml` | Expander | `AddOnContent`, `AddOnContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentMotionActor` | template node (LayoutAwareMotionActor) | `ExpanderTheme.axaml` | Expander | `Content`, `ContentBorderThickness`, `ContentPadding`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `ExpanderTheme.axaml` | Expander | `Content`, `ContentPadding`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Frame` | `PixelAlignedBorder` | 根边框、裁剪和整体布局承载。 |
| `PART_MainLayout` | `DockPanel` | Header 与 Content 的 dock 布局。 |
| `PART_HeaderLayoutTransform` | `LayoutTransformControl` | 横向展开方向下旋转 Header。 |
| `PART_HeaderDecorator` | `PixelAlignedBorder` | Header 背景和 padding 承载，也是 Header 点击范围；不绘制 Header/Content 分隔线。 |
| `PART_HeaderLayout` | `Grid` | 展开图标、Header、AddOnContent 的列布局。 |
| `PART_ExpandButton` | `IconButton` | 展开图标显示和图标触发入口。 |
| `PART_HeaderPresenter` | `ContentPresenter` | Header 内容和模板承载。 |
| `PART_AddOnContentPresenter` | `ContentPresenter` | AddOnContent 内容和模板承载。 |
| `PART_ContentMotionActor` | `LayoutAwareMotionActor` | Content 分隔线和 Content 的共同展开/收起动效承载。 |
| `PART_ContentPresenter` | `ContentPresenter` | Content 内容、模板和 padding 承载。 |

## Pseudo Classes

稳定伪类和主题状态：

| 伪类 / selector 状态 | 语义 |
| --- | --- |
| `:expanded` | 继承展开状态，用于展开图标旋转。 |
| `:up` / `:down` / `:left` / `:right` | 展开方向状态。 |
| `:custom-header-padding` | `HeaderPadding` 非空，Header padding 和图标间距使用显式值。 |
| `:custom-content-padding` | `ContentPadding` 非空，Content padding 使用显式值。 |
| `[IsBorderless=True]` / `[IsGhostStyle=True]` | 视觉强度分支。 |
| `[TriggerType=Header]` / `[TriggerType=Icon]` | Cursor 和点击路径分支。 |

Expander 没有专用 routed event 或 command。展开状态通过继承的 `IsExpanded` 表达。

## State Flow

核心状态流：

```text
Header pointer / ExpandButton click
        ↓
TriggerType gate
        ↓
IsExpanded
        ↓
Content motion / stable visibility
        ↓
PART_ContentMotionActor.IsVisible
```

触发语义：

- `TriggerType=Header` 时，鼠标左键按下且命中 `PART_HeaderDecorator` 会切换 `IsExpanded`。
- `TriggerType=Icon` 时，Header 点击不切换状态，只能通过 `PART_ExpandButton.Click` 切换。
- 禁用状态下模板将 `IsEnabled` 传递给 `PART_ExpandButton`，主题同步禁用前景。

展开方向：

- `ExpandDirection=Down`：Header 停留顶部，Content 向下展开。
- `ExpandDirection=Up`：Header dock 到底部，Content 向上展开。
- `ExpandDirection=Left` / `Right`：Header 通过 `LayoutTransformControl` 旋转，整体对齐到对应侧。
- 展开图标在 `:expanded` 下按展开方向旋转，必须与内容运动方向一致。

动效状态：

- `IsMotionEnabled=false` 时直接同步 `PART_ContentMotionActor.IsVisible` 和透明度。
- `IsMotionEnabled=true` 时使用 `ExpandMotion` / `CollapseMotion` 运行布局感知动效。
- 展开状态在动画中再次变化时，当前 motion 会取消并以最新 `IsExpanded` 重新归一到最终可见状态。

自定义 padding：

```text
HeaderPadding != null
  → :custom-header-padding
  → PART_HeaderDecorator.Padding = HeaderPadding
  → EffectiveExpandButtonMargin 按 HeaderPadding.Left/Right 推导

ContentPadding != null
  → :custom-content-padding
  → PART_ContentPresenter.Padding = ContentPadding
```

自定义 HeaderPadding 分支必须保持展开图标与 Header 文本的水平间距和垂直居中关系，不应继续套用默认尺寸 token 的图标间距。

## Theme and Token Boundaries

Expander 的默认视觉由 `ExpanderTheme.axaml` 和 `ExpanderToken` 共同定义。

主题职责：

- 提供 Header、Content、MotionActor 和 Frame 的稳定模板结构。
- 根据 `SizeType` 选择 Header/Content padding 和字体大小。
- 根据 `ExpandDirection` 设置 Header dock、旋转和图标旋转。
- Content 靠近 Header 的一侧固定承担分隔线，分隔线不依赖 `IsExpanded` 或 motion 时序。
- 根据 `IsBorderless` / `IsGhostStyle` 同时移除根边框和 Content 分隔线，并保持既有背景规则。
- 根据 `TriggerType` 设置可点击区域 cursor。
- 根据自定义 padding 伪类覆盖 Header/Content padding 和展开图标间距。

Token 关系：

```text
SharedToken
   ↓
ExpanderToken
   ↓
ExpanderTheme
   ↓
Frame + Header + ExpandButton + Content Border + Content
```

SharedToken 提供全局边框、字体、动效时长、图标大小和基础颜色。ExpanderToken 提供 Header/Content padding、Header/Content 背景、圆角和展开图标默认外边距。

Token 边界：

ExpanderToken 是 Expander 的组件级 Token scope，描述 Header/Content 的默认 padding、背景、整体圆角和展开图标默认外边距。

ExpanderToken 不承载以下状态：

- `Header`、`Content`、`AddOnContent` 等实例内容。
- `IsExpanded`、`ExpandDirection`、`TriggerType`、`ExpandIconPosition` 等实例行为状态。
- `HeaderPadding` / `ContentPadding` 的显式用户覆盖值。
- `EffectiveBorderThickness`、`ContentBorderThickness`、`EffectiveExpandButtonMargin` 等运行时派生状态。
- motion 运行状态、cancellation 或临时 transform。

## Customization Boundaries

维护 Expander 时必须保持以下不变量：

- 公共 API 名称、类型、默认值和继承语义不能在未授权情况下改变。
- `Header`、`Content`、`IsExpanded`、`ExpandDirection` 继续遵守 Avalonia `Expander` 语义。
- `PART_Frame`、`PART_HeaderDecorator`、`PART_ExpandButton`、`PART_HeaderPresenter`、`PART_AddOnContentPresenter`、`PART_ContentMotionActor`、`PART_ContentPresenter` 的名称和外部协作语义保持稳定。
- `PART_HeaderDecorator` 只负责 Header 背景和 padding，不承担 Header/Content 分隔线。
- Header/Content 分隔线必须位于 `PART_ContentMotionActor` 内部，并随 Content 自然显示、裁剪和隐藏。
- `TriggerType=Icon` 时 Header 点击不能切换 `IsExpanded`。
- `TriggerType=Header` 时 Header 区域点击应切换 `IsExpanded`。
- 默认 `ExpandIcon` 为空时必须补齐 `RightOutlined`。
- `IsBorderless` 和 `IsGhostStyle` 必须让有效根边框和 Content 分隔线厚度都为 `0`。
- `IsMotionEnabled=false` 必须直接进入稳定显示/隐藏状态，不留下 motion 临时值。
- 动画取消、模板重套用和 detach 时不能保留旧 motion actor 的运动属性或未释放 cancellation。
- `SizeType=Custom` 默认沿用 Middle 尺寸分支，显式 padding 覆盖默认 token。
- Token 名称、伪类名称和 template selector 入口不能擅自删除或重命名。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- template reapply 时旧 `_expandButton.Click` 必须解除。
- detach 时必须取消 motion 并清理临时值。
- `TriggerType=Icon` 不能通过 Header 点击切换状态。
- 默认 `ExpandIcon` 为空时必须使用 `RightOutlined`，且不覆盖用户显式图标。
- `IsMotionEnabled=false` 不能留下 Height 或 transform 临时值。
- `CompleteContentMotion` 必须校验当前 cancellation 和当前 motion actor。
- Header/Content 分隔线只能由方向、边框厚度和视觉模式决定，不能依赖 `IsExpanded` 或 motion 时序。
- `ExpandDirection` 的 motion 方向、Header dock、Header transform 和图标旋转必须同步维护。
- 自定义 HeaderPadding 下的图标间距必须跟随 HeaderPadding 对应方向，不回退到默认 SizeType token。
- `:custom-header-padding` 和 `:custom-content-padding` 的伪类语义不能混用。
- Expander 不引入多面板或手风琴状态；这属于 Collapse 的职责。
