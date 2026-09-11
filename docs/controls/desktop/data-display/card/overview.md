# Card 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.Card` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/development/control-development-guidelines.md)，内部实现原理见 [Card 桌面版实现原理](implementation.md)，Card Token 的专项设计见 [Card Token 设计](token.md)，设计和契约变化记录见 [Card Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Card` |
| 控件状态 | Stable |

Card 是桌面端数据展示类控件，用于在独立的内容面板中组织信息。它提供标题、额外操作、封面媒体、内容区域、底部操作区、元信息、栅格内容、标签页内容、加载占位、悬停反馈和无边框样式。

Card 的职责是提供稳定的信息容器视觉和组合入口。它不负责集合筛选、排序、虚拟化、表单提交、复杂选择、弹层管理或远程数据加载；这些能力应由业务层、ListView、DataGrid、Form、Popup/Flyout 或专用控件承担。

## 2. 设计语言

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

## 3. API 与契约模型

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

## 4. 行为与状态模型

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

## 5. 视觉与主题模型

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

## 6. 控件家族或集成关系

Card 属于 Data Display 分类，常与 Grid、Image、Avatar、TabControl、Skeleton、HyperLinkButton 和 IconButton 组合。

集成关系：

- Token 系统：根 Card 通过 `CardToken.ScopeProvider` 注册控件 Token 资源作用域。
- Skeleton：`IsLoading` 通过模板内 Skeleton 表达加载占位。
- TabControl：`CardTabsContent` 将 Card 的 `SizeType` 和 `IsMotionEnabled` 传递给内部 TabControl。
- ItemsControl：`CardGridContent` 通过 ItemsControl 生成 `CardGridItem` 容器。
- Gallery：Card ShowCase 覆盖基础尺寸、无边框、简单卡片、封面/Meta、栅格、内部卡片、Loading、Tabs 和更多配置。

Card 不实现 Form、CompactSpace、选择、弹层或媒体断点接口。

## 7. 兼容性不变量

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

## 8. 专项模型

### 8.1 内容类型模型

Card 使用内部 `ContentType` 将直接内容归一为四类：

```text
CardMetaContent -> Meta
CardGridContent -> Grid
CardTabsContent -> Tabs
others/null -> Default
```

主题根据 `ContentType` 控制 body padding、Header border 和圆角。Grid 内容取消 body padding，由 `CardGridItem` 自己提供 item padding 和边线阴影。

### 8.2 栅格内容模型

`CardGridContent` 暴露 `ColumnDefinitions` 和 `RowDefinitions`，并把它们同步到内部 ItemsPresenter 的 Grid panel。`CardGridItem` 使用 `Row`、`Column`、`RowSpan` 和 `ColumnSpan` 写入 Avalonia Grid attached properties。

### 8.3 操作区模型

`Actions` 集合中的控件被复制到内部 `CardActionPanel.Actions`。`CardActionPanel` 用 `UniformGrid` 均分每个 action，并在 render 阶段绘制顶部边线和 action 分隔线。

### 8.4 标签页内容模型

`CardTabsContent` 既支持 XAML `Items` 集合，也支持 `TabItemsSource` / `TabItemTemplate`。模板内 `TabControl` 负责实际 tab 选择和内容显示，Card 只负责把内容类型、SizeType 和 Motion 状态同步给它。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [Card 桌面版实现原理](implementation.md)
- [Card Token 设计](token.md)
- [Card Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Card` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + `implementation.md` + `token.md` + Gallery ShowCase | 生成 `controls/card/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/card/semantic-cn.md` |
| API 表 | overview.md 语义摘要 + 源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | token.md、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 层次 | 验证内容 |
| --- | --- |
| Public API | `Extra`、`StyleVariant`、`SizeType`、`IsLoading`、`IsInnerMode`、`IsHoverable`、`Cover`、`Actions`、`BoxShadow`、`IsMotionEnabled` 和子控件 API。 |
| 状态行为 | ContentType 切换、Headerless、Actions 增删改移、Grid 行列定义、GridItem row/column/span、Loading 和 hover。 |
| AXAML / Template | `PART_ActionPanel`、`PART_ItemsPresenter`、`PART_TabControl`、Header/Cover/Content/Action 区域、Skeleton 包裹关系。 |
| Token | Header、Body、Actions、Tabs、Extra、Shadow、Grid item 和 action icon Token。 |
| Gallery | Basic、NoBorder、Simple、CustomizedContent、CardInColumn、GridCard、InnerCard、LoadingCard、WithTabs、MoreContentConfiguration 示例。 |
| 文档 | 运行 `git diff --check`，检查相对链接存在。 |
