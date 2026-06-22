# Card 桌面版实现原理

本文档描述 Card 桌面版的内容类型识别、Actions 同步、Grid 内容容器生命周期、Tabs 内容桥接、主题状态同步和维护边界。公共设计与 API 契约见 [Card 桌面版架构设计](overview.md)，Token 语义见 [Card Token 设计](token.md)，变化记录见 [Card Changelog](changelog.md)。

## 1. 实现定位

Card 的实现重点是把 Header/Extra/Cover/Content/Actions 组合成一个稳定面板，并根据直接内容类型选择不同 body 视觉。Card 自身不维护业务数据模型，也不管理 tab 选择、grid item 业务状态或 action 命令执行。

本文档只描述 Card 相关实现结构，不重新说明 `HeaderedContentControl`、`ItemsControl`、`TabControl`、`Skeleton`、Avalonia Grid 或 Token 系统的通用机制。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/Card/Card.cs`：根 Card 公共 API、Actions 集合、内容类型识别、Header/Content 边框和圆角派生状态、Size/Motion 内容同步。
- `src/AtomUI.Desktop.Controls/Card/CardActionPanel.cs`：内部操作区，承载 Actions，维护 UniformGrid children，并渲染分隔线。
- `src/AtomUI.Desktop.Controls/Card/CardActionButton.cs`：Card 操作区推荐按钮类型，提供主题 key 边界。
- `src/AtomUI.Desktop.Controls/Card/CardMetaContent.cs`：头像、标题和描述内容的元信息承载控件。
- `src/AtomUI.Desktop.Controls/Card/CardGridContent.cs`：Card grid 内容的 ItemsControl，负责 Grid panel 行列定义同步和容器准备/清理。
- `src/AtomUI.Desktop.Controls/Card/CardGridItem.cs`：Grid item 容器，负责 content、row/column/span 和 hover/size/motion 状态。
- `src/AtomUI.Desktop.Controls/Card/CardTabsContent.cs`：Card tabs 内容桥接控件，负责把 Items 集合复制到内部 TabControl。
- `src/AtomUI.Desktop.Controls/Card/CardPseudoClass.cs`：Card 稳定伪类常量。
- `src/AtomUI.Desktop.Controls/Card/CardToken.cs`：Card 组件 Token。
- `src/AtomUI.Desktop.Controls/Card/Themes/*.axaml`：根主题、操作区、操作按钮、Meta、Grid、Tabs 等主题资源。

## 3. 核心类职责

`Card` 是状态协调器。它继承 `HeaderedContentControl`，负责公共属性、Headerless 伪类、Actions 同步、ContentType 派生、CardToken scope 注册和初始动效屏蔽。

`CardActionPanel` 是内部 action layout 和 render owner。它持有内部 `Actions` 集合，模板接入后将集合写入 `UniformGrid`，并在 `Render` 中绘制操作区顶部边线和纵向分隔线。

`CardGridContent` 是 grid 内容的 ItemsControl。它生成或复用 `CardGridItem`，把 `ColumnDefinitions` / `RowDefinitions` 同步到 ItemsPresenter 的 Grid panel，并为 `PrepareCardGridItem` 提供 container 生命周期 disposable。

`CardGridItem` 是 grid item 容器。它把 `Row`、`Column`、`RowSpan`、`ColumnSpan` 的变化写入 Avalonia Grid attached properties。

`CardTabsContent` 是 Card 与 TabControl 的桥接层。它承载 XAML `Items`，也暴露 `TabItemsSource` 和 `TabItemTemplate`，模板中真正的选择和内容显示由 `TabControl` 负责。

`CardMetaContent` 和 `CardActionButton` 是主题语义类型。它们逻辑很薄，但类型本身是用户和主题可依赖的契约。

## 4. 状态与数据流

内容类型流：

```text
Card.Content changed
  -> Dispose old content state bindings
  -> inspect direct content
  -> ContentType = Meta / Grid / Tabs / Default
  -> bind SizeType/IsMotionEnabled to Grid or Tabs content when needed
  -> ConfigureContentCornerRadius()
  -> ConfigureHeaderBorderThickness()
```

`ContentType` 是内部 DirectProperty。它只由根 Card 根据直接 `Content` 类型维护，主题根据该状态选择 body padding、header border 和 corner radius。

Actions 流：

```text
Card.Actions.CollectionChanged
  Add/Remove/Replace/Move
  -> PART_ActionPanel.Actions
  -> IsActionsPanelVisible = Actions.Count > 0
```

`CardActionPanel.Actions.CollectionChanged` 再同步到 `UniformGrid.Children`，并设置 `UniformGrid.Columns = Actions.Count`。

Grid 内容流：

```text
CardGridContent.ColumnDefinitions / RowDefinitions
  -> SyncGridPanelProperties()
  -> ItemsPresenter.Panel as Grid
  -> grid.ColumnDefinitions / grid.RowDefinitions
```

Grid item 容器流：

```text
PrepareContainerForItemOverride
  -> CardGridItem.Content / ContentTemplate
  -> bind SizeType / IsEnabled / IsMotionEnabled
  -> PrepareCardGridItem(..., CompositeDisposable)

ClearContainerForItemOverride
  -> dispose prepared container state
```

Tabs 内容流：

```text
CardTabsContent.Items.CollectionChanged
  -> PART_TabControl.Items Add/Remove/Replace/Move
```

`TabItemsSource` 和 `TabItemTemplate` 通过 TemplateBinding 直接交给内部 TabControl。

## 5. 生命周期与模板接入

Card 构造阶段：

- 注册 `CardToken.ScopeProvider`。
- 订阅 `Actions.CollectionChanged`。`Actions` 是 Card 自身持有的生命周期集合，不需要在普通 detach 中解除。

Card 初始化和加载：

- `OnInitialized()` 调用 `DisableTransitions()`，避免初始样式应用触发过渡。
- `OnLoaded()` 通过 dispatcher 在加载完成后启用过渡。

Card 模板接入：

- `OnApplyTemplate()` 清空旧 `CardActionPanel.Actions`。
- 获取 `PART_ActionPanel`。
- 将当前 `Actions` 复制到新的 action panel。
- 重新配置 ContentType、Content corner radius 和 Header border。

内容状态绑定：

- `CardTabsContent` 和 `CardGridContent` 作为直接内容时，Card 使用 `BindUtils.RelayBind` 同步 `SizeType` 和 `IsMotionEnabled`。
- 该绑定无法放在 Card 根模板中表达，因为绑定目标是运行时 `Content` 对象而非稳定 template part。
- `_contentStateBindings` 是这组绑定的 owner，内容重新配置时必须先释放旧绑定。

GridContent 生命周期：

- `OnApplyTemplate()` 获取 `PART_ItemsPresenter`，调用 `ApplyTemplate()` 后同步 Grid 行列定义。
- `PrepareContainerForItemOverride()` 每次准备容器前先释放该容器旧状态，防止 recycle 时保留旧 binding 或订阅。
- `ClearContainerForItemOverride()` 释放对应 `CompositeDisposable`，再调用 base。

TabsContent 生命周期：

- 构造阶段订阅自有 `Items.CollectionChanged`。
- `OnApplyTemplate()` 清空旧内部 TabControl items，获取 `PART_TabControl`，再复制当前 Items。
- `Items` 是控件自有集合，订阅随控件实例生命周期存在。

## 6. 交互与事件处理

Card 自身不处理 keyboard、focus、pointer capture、drag/drop 或 popup 事件。

交互路径主要由主题和内部控件负责：

- `IsHoverable` 通过 selector 设置 cursor、pointer over 阴影和边框厚度。
- `CardActionPanel` pointer over 改变 Foreground。
- `CardActionButton` pointer over 改变 IconBrush。
- Tab 选择由内部 `TabControl` 处理。
- Loading 状态由 `Skeleton` 处理。

集合事件路径：

- `Card.Actions.CollectionChanged`
- `CardActionPanel.Actions.CollectionChanged`
- `CardTabsContent.Items.CollectionChanged`

这些路径只同步视觉集合，不执行业务命令。

## 7. 内部算法与关键流程

### 7.1 ContentType 配置

内容类型配置必须先释放旧内容状态绑定，再按当前直接内容设置内部状态：

```text
CardMetaContent -> Meta
CardTabsContent -> Tabs + bind state
CardGridContent -> Grid + bind state
others/null -> Default
```

`Default` 回退是必要状态，防止从 Grid/Tabs/Meta 切回普通内容后主题仍保留旧 padding、border 或 corner radius 分支。

### 7.2 Header 边框和内容圆角

Card 根据 `ContentType` 计算两个内部状态：

```text
ContentType=Grid
  HeaderBorderThickness = 0
  EffectiveCornerRadius = top corners only

otherwise
  HeaderBorderThickness = bottom border
  EffectiveCornerRadius = CornerRadius
```

Borderless 和 Outlined 的实际边框厚度由 `ConfigureContentBorderThickness()` 根据 `StyleVariant` 计算。

### 7.3 Headerless 伪类

Headerless 条件：

```text
Header == null
&& HeaderTemplate == null
&& Extra == null
&& ExtraTemplate == null
```

该伪类只表示 Header 区域是否有可展示入口，不受 Cover、Content、Actions 或 Loading 影响。

### 7.4 Grid item 准备和清理

`CardGridContent` 复用 Avalonia ItemsControl 容器机制：

- 非 Visual item 且 `CardGridItem.Content` 未显式设置时，将 item 作为 content。
- `ItemTemplate` 非空时同步到 `CardGridItem.ContentTemplate`。
- Size、Enabled 和 Motion 状态通过 Avalonia property binding 同步到容器。
- `PrepareCardGridItem` 允许派生类追加容器级绑定或订阅，追加内容必须放入传入的 `CompositeDisposable`。

容器 recycle 或清理时，`CompositeDisposable` 必须被释放，防止旧 item 状态泄漏到新 item。

### 7.5 Actions 渲染

`CardActionPanel` 使用 `UniformGrid` 均分 action。Render 阶段用当前 `Actions` 子项的 bounds 绘制：

- 顶部横线。
- 每个非最后 action 右侧纵向分隔线。

分隔线颜色和粗细来自 `BorderBrush` 和 `BorderThickness.Left`。

## 8. 资源、性能与 AOT 边界

Card 不依赖运行时反射发现模板结构。模板协作通过固定 part 名称、强类型控件和 Avalonia property binding 完成。

资源与生命周期边界：

- `CardToken` 通过 token generator 注册，Theme 通过 `CardTokenResource` 使用。
- `CardActionButton`、`CardGridItem`、`CardMetaContent` 等是 Visual 控件，Token resource 由主题和视觉树生命周期管理。
- `Card` 到动态 `Content` 的 Size/Motion 同步使用 `BindUtils.RelayBind`，因为目标不是稳定 template part；绑定 owner 是 `_contentStateBindings`。
- `CardGridContent.PrepareCardGridItem` 的扩展 disposable 由 `_cardGridItemDisposables` 管理，并在 container clear/recycle 时释放。
- 不在 layout hot path 中读取全局资源或创建反射路径。

AOT 边界：

- 不新增字符串 path binding、`ReflectionBinding`、assembly scan、`Activator.CreateInstance(Type)` 或动态成员访问。
- Gallery API/Token 表使用显式 ViewModel 数据。
- Card 的动态内容同步只使用 `AvaloniaProperty` 强类型绑定。

## 9. 维护不变量

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

## 10. 测试与验证

验证范围：

- `CardBehaviorTests`：CardGridContent 行列定义 wrapper、ContentType 回退、旧内容 SizeType 绑定释放、GridItem container disposable 释放。
- Gallery Card 示例：Basic、NoBorder、Simple、CustomizedContent、CardInColumn、GridCard、InnerCard、LoadingCard、WithTabs、MoreContentConfiguration。
- `CardShowCasePageTests`：Card ShowCase 页面结构、懒加载 API/Token 表、本地化和示例 snapshot。
- 修改运行时代码时运行 `tests/AtomUI.Desktop.Controls.Tests` 中 Card 相关测试，并按影响范围扩大到完整 Desktop 控件测试。
- 修改 Gallery 或 API/Token 表时运行 `tests/AtomUIGallery.Tests` 中 Card ShowCase 相关测试。
- 文档改动运行 `git diff --check`，并检查相对链接存在。
