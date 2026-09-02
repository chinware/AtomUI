# Transfer 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Transfer 家族有两个具体 public owner：`ListTransfer` 与 `TreeTransfer`。两者各自持有独立 ControlTheme 与模板，
分区 Part 集合完全一致；条目 Part 仅 `ListTransfer` 发布 `itemIcon` / `itemContent` 及其限定变体（树侧条目区域
由 `TreeViewItem` 家族契约覆盖）：`root`、`source.section`、`target.section`、`actions`、`header`、`title`、`body`、
`list`、`footer`，以及方向限定变体 `source.header` / `target.header` / `source.title` / `target.title` /
`source.body` / `target.body` / `source.list` / `target.list` / `source.footer` / `target.footer`（§1.1–1.9，
命名与上游 `source.section` 等语义键逐字对齐，`.` 为层级分隔符）。抽象基类 `AbstractTransfer` 没有自己的 ControlTemplate，不声明任何 Part；internal
`TransferItemDecorator`、`TransferSelectDropdown`、`TransferTreeViewItemHeader` 不注册独立 descriptor，也不能通过
模板复用获得其他 owner 的 owner-scoped Semantic Style。

条目级 Part（`item` / `itemIcon` / `itemContent` 及其 `source.*` / `target.*` 限定变体）由 Transfer owner
直接发布（§1.10 / §1.11），使语义清单与样式用法都保持组件级单列表——嵌套视图与条目容器不再注册为独立
Semantic owner：`TransferListView` 继承 `ListView` 的 `root` / `item` / `groupHeader` 家族契约，
`TransferTreeView` 继承 `TreeView` 契约，条目容器的指示 / 内容区域由本节 Part 覆盖；`TransferTreeViewItem` 的条目区域由
`TreeViewItem` 家族契约覆盖，`TransferRemoveItemButton` 的内部区域由 `IconButton` 家族契约与组合模型约束。Transfer
owner 不穿透这些嵌套 owner 的模板（§5）。

`source.section` / `target.section` / `actions` 是 owner 模板中的静态节点，`Single`；方向限定部件
`source.*` / `target.*` 与对应未限定部件共享终端 marker，由路由中的方向锚点（`.semantic-source` /
`.semantic-target`）或条目自锚点（`.semantic-source-item` / `.semantic-target-item`）区分实例；`header` / `title` / `body` / `list` /
`footer` 位于 internal `TransferItemDecorator` 的共享模板内，由源面板和目标面板两个实例同时实例化，`Multiple`
（数量恒为 2）。

### 1.1 `root`

| 字段 | 值 |
| --- | --- |
| Owner | `ListTransfer` / `TreeTransfer` |
| Part | `root` |
| Selector | owner 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `ListTransfer` / `TreeTransfer` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Transfer owner |
| 职责 | 承载数据、选择状态、过滤、布局模式和 owner-scoped Semantic Style 入口。 |
| 相关 API | `ItemsSource`、`TargetKeys`、`SelectedKeys`、`IsOneWay`、`IsStretchView`、`ListWidth`、`ListHeight`、`IsFilterEnabled`、`PageSize`、`Status`、`SizeType` |
| 相关 Token | `ListTransferToken` / `TreeTransferToken`、SharedToken |
| 稳定性 | stable since 6.0 |

`root` 是控件自身，不声明 `.semantic-root` marker。它适合定制 owner 级 `Foreground`、`Opacity` 与整体布局约束；
源/目标面板与中间操作区的结构由各 Part 负责，不通过 root Setter 改写。

### 1.2 `source.section`

| 字段 | 值 |
| --- | --- |
| Owner | `ListTransfer` / `TreeTransfer` |
| Part | `source.section` |
| Selector | `.semantic-source` |
| SelectorRoute | `/template/ .semantic-source` |
| Style Type | `ListTransferSourceSectionStyle` / `TreeTransferSourceSectionStyle` |
| ContractType | `TemplatedControl`（internal `TransferItemDecorator` 的最低 public 类型） |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | internal `TransferItemDecorator#SourceDecoratorView`（源面板 section） |
| 职责 | 源方向列表分区的外框，承载 header、过滤输入、列表宿主和 footer 的组织边界。 |
| 相关 API | `SourceTitle`、`SourceTitleTemplate`、`SourceViewFooter`、`SourceViewFooterTemplate`、`ListWidth`、`ListHeight` |
| 相关 Token | `ListTransferToken` / `TreeTransferToken` 的 `HeaderHeight`、`HeaderPadding`、SharedToken 边框圆角 |
| 稳定性 | stable since 6.0 |

`source` 对应内部源面板装饰器实例。`BorderBrush`、`BorderThickness`、`CornerRadius` 经模板投影到分区外框
`Frame`；`Background` 同样投影到 `Frame`（默认为 null，即分区主体保持透明，header 保留自身背景 token）。面板宽度
由 owner `ListWidth` / `IsStretchView` 决定，`Width` 类 Setter 会与模板投影值按 Avalonia 原生优先级竞争。section
内部的 header / list / footer 区域由 §1.5–1.8 的共享 Part 表达，`source` 不展开其内部结构。

### 1.3 `target.section`

| 字段 | 值 |
| --- | --- |
| Owner | `ListTransfer` / `TreeTransfer` |
| Part | `target.section` |
| Selector | `.semantic-target` |
| SelectorRoute | `/template/ .semantic-target` |
| Style Type | `ListTransferTargetSectionStyle` / `TreeTransferTargetSectionStyle` |
| ContractType | `TemplatedControl`（internal `TransferItemDecorator` 的最低 public 类型） |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | internal `TransferItemDecorator#TargetDecoratorView`（目标面板 section） |
| 职责 | 目标方向列表分区的外框，承载 header、过滤输入、列表宿主和 footer 的组织边界。 |
| 相关 API | `TargetTitle`、`TargetTitleTemplate`、`TargetViewFooter`、`TargetViewFooterTemplate`、`IsOneWay`、`ListWidth`、`ListHeight` |
| 相关 Token | 同 `source` |
| 稳定性 | stable since 6.0 |

`target` 与 `source` 结构、route 深度和可定制属性完全一致，仅方向不同；`IsOneWay=True` 时目标面板仍存在，只是
条目选择与回移入口按 API 语义禁用或隐藏。方向差异化定制（如目标面板换背景）是 `source` / `target` 分区级 Part 的
核心用途。

### 1.4 `actions`

| 字段 | 值 |
| --- | --- |
| Owner | `ListTransfer` / `TreeTransfer` |
| Part | `actions` |
| Selector | `.semantic-actions` |
| SelectorRoute | `/template/ .semantic-actions` |
| Style Type | `ListTransferActionsStyle` / `TreeTransferActionsStyle` |
| ContractType | `StackPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 模板内 `StackPanel#ActionsLayout`（垂直布局容器） |
| 职责 | 组织"移至目标 / 移回源"两个操作按钮的中间操作区。 |
| 相关 API | `ToTargetTransferIcon`、`ToSourceTransferIcon`、`ToTargetButtonText`、`ToSourceButtonText`、`IsOneWay` |
| 相关 Token | `SpacingXXS`（按钮间距）、`SpacingXS`（与面板间距） |
| 稳定性 | stable since 6.0 |

`actions` 是操作区容器，适合定制 `Spacing`、`Margin`、`Opacity` 与对齐。`IsOneWay=True` 时"移回源"按钮仅切换
可见性，容器与 marker 不变。容器内的两个按钮是 public `Button` 实例，属于嵌套 Button 家族契约（§5），不通过
`actions` Setter 改写按钮内部视觉。

### 1.5 `header`

| 字段 | 值 |
| --- | --- |
| Owner | `ListTransfer` / `TreeTransfer` |
| Part | `header` |
| Selector | `.semantic-header` |
| SelectorRoute | `/template/ .semantic-scope-section /template/ .semantic-header` |
| Style Type | `ListTransferHeaderStyle` / `TreeTransferHeaderStyle` |
| ContractType | `PixelAlignedBorder` |
| Cardinality | `Multiple`（源、目标各一，恒为 2） |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TransferItemDecorator` 模板内 `PixelAlignedBorder#HeaderFrame` |
| 职责 | 面板头部分区，承载全选指示、选择计数与标题的组织边界。 |
| 相关 API | `IsShowSelectAllCheckbox`、`IsShowSelectDropdownMenu`、`SelectionsIcon`、`SourceTitle` / `TargetTitle` |
| 相关 Token | `HeaderHeight`、`HeaderPadding`、`ColorSplit`、`ColorBgContainer` |
| 稳定性 | stable since 6.0 |

`header` 的高度由 `HeaderHeight` token 经模板投影（`ControlHeightLG` 派生的固定基线），`Height` 类 Setter 按原生
优先级参与测量，但需与分区外框和 owner 布局协调（见 implementation.md 尺寸基线）。header 内部的全选
`CheckBox`、internal 下拉指示按钮与选择计数 presenter 属于 header 组合结构，不单独发布（§5）。`.semantic-scope-section`
是路由边界标记（源、目标两个装饰器实例同时携带），不进入公开 Part 表。五个分区 Part（`header` / `title` / `body` /
`list` / `footer`）的标记节点都位于 `TransferItemDecorator` 自身的 ControlTheme 模板内，由装饰器实例化产生，
描述符因此声明 `RuntimeCreated=true`（生成器据此跳过 owner 模板的静态 marker 计数，与 Collapse、Splitter 先例一致）。

### 1.6 `title`

| 字段 | 值 |
| --- | --- |
| Owner | `ListTransfer` / `TreeTransfer` |
| Part | `title` |
| Selector | `.semantic-title` |
| SelectorRoute | `/template/ .semantic-scope-section /template/ .semantic-title` |
| Style Type | `ListTransferTitleStyle` / `TreeTransferTitleStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Multiple`（源、目标各一，恒为 2） |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TransferItemDecorator` 模板内 `ContentPresenter#TitleContentPresenter` |
| 职责 | 承载 `SourceTitle` / `TargetTitle` 及其模板的最终呈现。 |
| 相关 API | `SourceTitle`、`SourceTitleTemplate`、`TargetTitle`、`TargetTitleTemplate` |
| 相关 Token | `HeaderPadding`（右对齐留白） |
| 稳定性 | stable since 6.0 |

`title` presenter 始终存在于静态模板结构，`Title=null` 时仍保留节点身份；适合定制 `Foreground`、`Opacity`、
`Margin` 与对齐。`SourceTitleTemplate` / `TargetTitleTemplate` 创建的用户子树不属于 Transfer Semantic Part。

### 1.7 `body`

| 字段 | 值 |
| --- | --- |
| Owner | `ListTransfer` / `TreeTransfer` |
| Part | `body` |
| Selector | `.semantic-body` |
| SelectorRoute | `/template/ .semantic-scope-section /template/ .semantic-body` |
| Style Type | `ListTransferBodyStyle` / `TreeTransferBodyStyle` |
| ContractType | `DockPanel` |
| Cardinality | `Multiple`（源、目标各一，恒为 2） |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TransferItemDecorator` 模板内 `DockPanel#BodyLayout`（分区主体包裹节点） |
| 职责 | 分区主体区域，承载过滤输入与列表宿主的组织边界。 |
| 相关 API | `IsFilterEnabled`、`FilterPlaceholderText`、`ListHeight` |
| 相关 Token | `MarginXS`（过滤输入外距）、SharedToken |
| 稳定性 | stable since 6.0 |

`body` 是 header 与 footer 之间的分区主体包裹节点，与上游 `body` 语义键对齐：过滤输入与视图宿主是它的直接子
节点。`Background`、`Padding`、`Margin` 类 Setter 直接作用于主体区域，与 header 的背景 token 相互独立；过滤输入
（LineEdit 家族契约）与列表宿主（`list` Part）各有边界，`body` Setter 不展开其内部结构。`IsFilterEnabled` 只切换
过滤输入可见性，不改变 `body` 节点身份。

### 1.8 `list`

| 字段 | 值 |
| --- | --- |
| Owner | `ListTransfer` / `TreeTransfer` |
| Part | `list` |
| Selector | `.semantic-list` |
| SelectorRoute | `/template/ .semantic-scope-section /template/ .semantic-list` |
| Style Type | `ListTransferListStyle` / `TreeTransferListStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Multiple`（源、目标各一，恒为 2） |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TransferItemDecorator` 模板内 `ContentPresenter#ContentPresenter`（视图宿主） |
| 职责 | 承载源/目标视图控件（`TransferListView` / `TransferTreeView`）的宿主分区。 |
| 相关 API | `ListHeight`、`SourceView` / `TargetView`、`PageSize`、`ItemTemplate` |
| 相关 Token | `ListHeight`（宿主高度基线）、`BorderRadiusLG`（底部圆角） |
| 稳定性 | stable since 6.0 |

`list` 是 Transfer owner 拥有的视图宿主，不是嵌套视图控件本身：宿主的 `Height` 由 `ListHeight` 模板投影，
`CornerRadius` 由 footer 存在性驱动（有 footer 时底部圆角归零）。条目、分组、分页等视图内部区域的定制属于嵌套
owner 契约（§5）。`Height` 类 Setter 与 owner `ListHeight` API 按原生优先级竞争，需按 implementation.md 尺寸基线
理解最终 Measure/Arrange 结果。

### 1.9 `footer`

| 字段 | 值 |
| --- | --- |
| Owner | `ListTransfer` / `TreeTransfer` |
| Part | `footer` |
| Selector | `.semantic-footer` |
| SelectorRoute | `/template/ .semantic-scope-section /template/ .semantic-footer` |
| Style Type | `ListTransferFooterStyle` / `TreeTransferFooterStyle` |
| ContractType | `PixelAlignedBorder` |
| Cardinality | `Multiple`（源、目标各一，恒为 2；未设置 footer 内容时隐藏） |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TransferItemDecorator` 模板内 `PixelAlignedBorder#FooterFrame` |
| 职责 | 面板底部分区，承载 `SourceViewFooter` / `TargetViewFooter` 及其模板的呈现边界。 |
| 相关 API | `SourceViewFooter`、`SourceViewFooterTemplate`、`TargetViewFooter`、`TargetViewFooterTemplate` |
| 相关 Token | `HeaderPadding`、`ColorSplit` |
| 稳定性 | stable since 6.0 |

`footer` 节点始终属于静态模板结构，可见性由对应方向 footer 内容驱动，未设置时隐藏但保留 marker 与对象身份；
设置 footer 内容时分区 body 的底部圆角归零，由 footer 闭合外框。footer presenter 内的用户内容子树不属于 Transfer
Semantic Part。

### 1.10 条目 Part（`item` / `itemIcon` / `itemContent`）

条目级 Part 由 Transfer owner 直接发布（不再以 `TransferListItem` / 嵌套视图为 owner），样式与语义清单保持
组件级单列表，与上游 Transfer 的 `item` / `itemIcon` / `itemContent` 语义键对齐：

| Part | Selector | SelectorRoute | Style Type | ContractType | Cardinality | AtomUI 节点 |
| --- | --- | --- | --- | --- | --- | --- |
| `item` | `.semantic-item` | `>> .semantic-item` | `ListTransferItemStyle` / `TreeTransferItemStyle` | `TransferListItem` / `TransferTreeViewItem` | `Multiple` | 视图容器（`TransferListItem` / `TransferTreeViewItem`） |
| `itemIcon` | `.semantic-item-icon` | `>> .semantic-item /template/ .semantic-item-icon` | `ListTransferItemIconStyle` | `CheckBox` | `Multiple` | `TransferListItem` 模板内 `CheckBox#SelectedIndicator` |
| `itemContent` | `.semantic-item-content` | `>> .semantic-item /template/ .semantic-item-content` | `ListTransferItemContentStyle` | `ContentPresenter` | `Multiple` | `TransferListItem` 模板内 `ContentPresenter#ContentPresenter` |

`itemIcon` / `itemContent` 仅 `ListTransfer` 发布（树侧条目模板无对应节点，指示 / 图标 / 标题区域由
`TreeViewItem` 家族的 `itemIndicator` / `itemIcon` / `itemTitle` 契约覆盖）。`>>` 是路由的后代步进
（从 owner 视觉子树任意深度命中），运行时以锚链匹配。容器由视图在创建 / prepare 路径建立 `.semantic-item`
与方向条目 marker（§1.11），prepare、restore、recycle 不切换任何 marker。`itemIcon` 是条目选择指示
CheckBox，`IsCheckable=false`（单向模式目标面板）时隐藏但保留节点身份，右缘的移除按钮与选择行为不属于
Part；`itemContent` 承载 `ItemTemplate` 内容，其创建的用户子树不属于 Semantic Part。

### 1.11 方向限定部件（`source.*` / `target.*`）

每个分区内部件与条目件都有方向限定变体，命名与上游语义键逐字对齐（`.` 为层级分隔符）。限定部件与对应未限定
部件**共享终端 marker**，由路由中的方向锚点区分实例，因此不新增任何模板 marker；`Since` 均为 `6.0`。

**owner 级分区限定部件**（`ListTransfer` / `TreeTransfer`，`Single`，`RuntimeCreated=true`）：

| Part | SelectorRoute | ContractType | AtomUI 节点 |
| --- | --- | --- | --- |
| `source.header` / `target.header` | `/template/ .semantic-source（或 target）/template/ .semantic-header` | `PixelAlignedBorder` | 装饰器模板内 `PixelAlignedBorder#HeaderFrame` |
| `source.title` / `target.title` | 同链至 `.semantic-title` | `ContentPresenter` | `ContentPresenter#TitleContentPresenter` |
| `source.body` / `target.body` | 同链至 `.semantic-body` | `DockPanel` | `DockPanel#BodyLayout` |
| `source.list` / `target.list` | 同链至 `.semantic-list` | `ContentPresenter` | `ContentPresenter#ContentPresenter`（视图宿主） |
| `source.footer` / `target.footer` | 同链至 `.semantic-footer` | `PixelAlignedBorder` | `PixelAlignedBorder#FooterFrame` |

**条目限定部件**（`Multiple`，`RuntimeCreated=true`，由 Transfer owner 直接发布）：
`source.item` / `target.item`（`>> .semantic-source-item` / `>> .semantic-target-item`）、
`source.itemIcon` / `target.itemIcon`（`>> .semantic-source-item /template/ .semantic-item-icon` 等，仅
`ListTransfer`）、`source.itemContent` / `target.itemContent`（同形，仅 `ListTransfer`）。方向条目类由视图在
prepare 路径按自身 `ViewType` 一次性补挂（路 A：容器与视图绑定后不迁移、`ViewType` 不可变，marker 保持静态
稳定），从而与未限定条目部件共享终端 marker。Style Type 依 Path 生成：
`ListTransferSourceHeaderStyle` / `TreeTransferTargetListStyle` / `ListTransferSourceItemIconStyle` 等，
全部直接挂在 Transfer 作用域下使用（如 `<atom:ListTransferSourceItemIconStyle>` 置于
`<Style Selector="atom|ListTransfer.my-demo">` 内即可只命中源面板条目图标）。

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Transfer
  -> TransferItemDecorator (control theme, TransferItemDecoratorTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> DockPanel#RootLayout (template-stable)
           -> PixelAlignedBorder#HeaderFrame (template-stable)
              -> DockPanel#HeaderLayout (template-stable)
                 -> CheckBox#SelectAllCheckBox (template-stable)
                 -> TransferSelectDropdown#MenuIndicator (internal-observable)
                 -> ContentPresenter#SelectedInfo (internal-observable)
                 -> ContentPresenter#TitleContentPresenter (internal-observable)
           -> PixelAlignedBorder#FooterFrame (template-stable)
              -> ContentPresenter#FooterPresenter (internal-observable)
           -> DockPanel#BodyLayout (template-stable)
              -> LineEdit#FilterInput (template-stable)
              -> ContentPresenter#ContentPresenter (internal-observable)
  -> TransferListItem (item container control theme, TransferListItemTheme.axaml)
     -> Border#Frame (template-stable)
        -> DockPanel#ContentLayout (template-stable)
           -> CheckBox#SelectedIndicator (template-stable)
           -> TransferRemoveItemButton#RemoveButton (public)
           -> ContentPresenter#ContentPresenter (internal-observable)
  -> TransferListView (control theme, TransferListViewTheme.axaml)
  -> TransferSelectDropdown (control theme, TransferSelectDropdownTheme.axaml)
  -> TransferTreeViewItemHeader (control theme, TransferTreeViewItemHeaderTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> Grid#ItemsLayout (template-stable)
           -> NodeSwitcherButton#{x:Static atom:TreeViewItemHeaderThemeConstants.NodeSwitcherButtonPart} (template-stable)
           -> Decorator (template-stable)
              -> CheckBox#ToggleCheckbox (template-stable)
           -> Decorator (template-stable)
              -> RadioButton#ToggleRadio (template-stable)
           -> Decorator (template-stable)
              -> IconPresenter#{x:Static atom:TreeViewItemHeaderThemeConstants.IconPresenterPart} (internal-observable)
           -> Decorator (template-stable)
              -> Border#{x:Static atom:TreeViewItemHeaderThemeConstants.HeaderContentFramePart} (template-stable)
                 -> Panel (template-stable)
                    -> ContentPresenter#HeaderPresenter (internal-observable)
                    -> TextBlock#FilterHighlighter (template-stable)
  -> TransferTreeViewItem (item container control theme, TransferTreeViewItemTheme.axaml)
     -> StackPanel (template-stable)
        -> TransferTreeViewItemHeader#Header (internal-observable)
        -> LayoutAwareMotionActor#{x:Static atom:TreeViewItemThemeConstants.ItemsPresenterMotionActorPart} (internal-observable)
           -> ItemsPresenter#ItemsPresenter (internal-observable)
  -> TransferTreeView (control theme, TransferTreeViewTheme.axaml)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Transfer` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `TransferItemDecorator` | control theme | `TransferItemDecoratorTheme.axaml` | Transfer | `Background`, `BodyCornerRadius`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (PixelAlignedBorder) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `Background`, `BodyCornerRadius`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RootLayout` | template node (DockPanel) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `BodyCornerRadius`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius`, `FilterPlaceholderText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderFrame` | template node (PixelAlignedBorder) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `BorderThickness`, `CornerRadius`, `HeaderHeight`, `HeaderPadding`, `IsAllSelected`, `IsItemsSourceEmpty` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderLayout` | template node (DockPanel) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `IsAllSelected`, `IsItemsSourceEmpty`, `IsMotionEnabled`, `IsOneWay`, `IsPaginationEnabled`, `IsShowSelectDropdownMenu` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectAllCheckBox` | template node (CheckBox) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `IsAllSelected`, `IsItemsSourceEmpty` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `MenuIndicator` | template node (TransferSelectDropdown) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `IsAllSelected`, `IsItemsSourceEmpty`, `IsMotionEnabled`, `IsOneWay`, `IsPaginationEnabled`, `IsShowSelectDropdownMenu` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SelectedInfo` | template node (ContentPresenter) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `SelectedMessage` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TitleContentPresenter` | template node (ContentPresenter) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `Title`, `TitleTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `FooterFrame` | template node (PixelAlignedBorder) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `BorderThickness`, `CornerRadius`, `Footer`, `FooterTemplate`, `HeaderPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FooterPresenter` | template node (ContentPresenter) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `Footer`, `FooterTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `BodyLayout` | template node (DockPanel) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `BodyCornerRadius`, `Content`, `ContentTemplate`, `FilterPlaceholderText`, `IsFilterEnabled`, `ListHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FilterInput` | template node (LineEdit) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `FilterPlaceholderText`, `IsFilterEnabled`, `ViewType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `BodyCornerRadius`, `Content`, `ContentTemplate`, `ListHeight` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TransferListItem` | item container control theme | `TransferListItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `Content`, `ContentTemplate`, `CornerRadius`, `HorizontalContentAlignment`, `IsCheckable` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `TransferListItemTheme.axaml` | TransferListItem | `Background`, `Content`, `ContentTemplate`, `CornerRadius`, `HorizontalContentAlignment`, `IsCheckable` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentLayout` | template node (DockPanel) | `TransferListItemTheme.axaml` | TransferListItem | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `IsCheckable`, `IsSelected`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectedIndicator` | template node (CheckBox) | `TransferListItemTheme.axaml` | TransferListItem | `IsCheckable`, `IsSelected` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RemoveButton` | template node (TransferRemoveItemButton) | `TransferListItemTheme.axaml` | TransferListItem | `IsCheckable` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ContentPresenter` | template node (ContentPresenter) | `TransferListItemTheme.axaml` | TransferListItem | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TransferListView` | control theme | `TransferListViewTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `TransferSelectDropdown` | control theme | `TransferSelectDropdownTheme.axaml` | Transfer | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TransferTreeViewItemHeader` | control theme | `TransferTreeViewItemHeaderTheme.axaml` | Transfer | `Background`, `BorderThickness`, `Content`, `ContentFrameBackground`, `ContentTemplate`, `CornerRadius` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (PixelAlignedBorder) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `Background`, `BorderThickness`, `Content`, `ContentFrameBackground`, `ContentTemplate`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsLayout` | template node (Grid) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `Content`, `ContentFrameBackground`, `ContentTemplate`, `FilterHighlightRuns`, `GroupName`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:TreeViewItemHeaderThemeConstants.NodeSwitcherButtonPart}` | template node (NodeSwitcherButton) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `IsExpanded`, `IsLoading`, `IsMotionEnabled`, `SwitcherCollapseIcon`, `SwitcherExpandIcon`, `SwitcherLeafIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ToggleCheckbox` | template node (CheckBox) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `IsChecked` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ToggleRadio` | template node (RadioButton) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `GroupName`, `IsChecked` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:TreeViewItemHeaderThemeConstants.IconPresenterPart}` | template node (IconPresenter) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `Icon`, `IconEffectiveVisible`, `IsEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:TreeViewItemHeaderThemeConstants.HeaderContentFramePart}` | template node (Border) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `Content`, `ContentFrameBackground`, `ContentTemplate`, `FilterHighlightRuns`, `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `Content`, `ContentTemplate`, `FilterHighlightRuns` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderPresenter` | template node (ContentPresenter) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `FilterHighlighter` | template node (TextBlock) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `FilterHighlightRuns` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TransferTreeViewItem` | item container control theme | `TransferTreeViewItemTheme.axaml` | 用户代码 / 控件宿主 | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `StackPanel` | template node (StackPanel) | `TransferTreeViewItemTheme.axaml` | TransferTreeViewItem | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content`、`ContentTemplate`、`FilterPlaceholderText`、`FilterValueSelector`、`FooterTemplate`、`ItemTemplate`、`SelectionsIcon`、`SelectionsIconTemplate`、`SourceTitle`、`SourceTitleTemplate` 等 22 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `TargetKeys`、`SelectedKeys`、`Filter`、`IsAllSelected`、`IsFilterEnabled`、`PageSize` | 维护目标集合、当前面板选择、过滤、分页和集合状态。 |
| 交互与状态 | `IsMasked`、`IsMotionEnabled`、`IsOneWay`、`IsPaginationEnabled`、`IsShowSearch`、`IsShowSelectAll`、`IsShowSelectAllCheckbox`、`IsShowSelectDropdownMenu`、`IsStretchView`、`Status` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `ListHeight`、`ListWidth`、`SizeType` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Footer`、`TargetView`、`TargetViewFooter`、`ViewType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、collection/filter、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Transfer Token + ControlTheme。 |

## State Flow

Transfer 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、collection/filter、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `TargetKeys` 是目标集合的 public owner，源/目标面板数据由 `ItemsSource` 与 `TargetKeys` 推导；`SelectedKeys` 是当前选择的 public owner，内部源面板选择和目标面板选择按 key 是否存在于 `TargetKeys` 自动拆分。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

Transfer 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AbstractTransferTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ListTransferTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TransferItemDecoratorTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TransferListItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TransferListViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TransferSelectDropdownTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TransferTreeViewItemHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TransferTreeViewItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TransferTreeViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TreeTransferTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Transfer 使用 `TransferToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、collection/filter、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Transfer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TransferToken`，scope id 为 `Transfer`，源码位于 `src/AtomUI.Desktop.Controls/Transfer/TransferToken.cs`。

## Customization Boundaries

维护 Transfer 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Transfer 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- `TargetKeys` / `SelectedKeys` 不能与 `TransferListView.SelectedItems`、`TransferTreeView.CheckedItems` 或容器状态形成多个业务 owner。
- 对绑定集合的移动、移除和清空不能无条件替换集合实例；可写集合必须原地更新。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
- Semantic Part marker、selector class、route、`ContractType` 与 cardinality（见 [Transfer Semantic Part 契约](semantic-part.md)）；嵌套视图容器创建 / 回收路径稳定携带继承 `.semantic-item` marker；marker 不随状态增删，默认主题不消费 `.semantic-*` selector。
