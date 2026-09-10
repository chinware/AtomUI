# DropdownButton 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

DropdownButton 是唯一 Semantic owner，公开 5 个 Semantic Part，语义对齐上游 Dropdown 的 Semantic DOM
（`root` / `itemTitle` / `item` / `itemContent` / `itemIcon`）。上游 Dropdown 语义部件全部位于弹层侧——
`root` 是弹层根、`itemTitle` 是菜单分组标题、`item` / `itemIcon` / `itemContent` 是菜单项及菜单项内部槽位，
触发节点不是 Dropdown 的语义部件。AtomUI 保留 `root` 作为 owner 自身的隐式 Part（由生成器统一注册），因此
上游的弹层根 `root` 映射为 `popup.root`；`itemTitle` 对应上游的分组标题（`ant-menu-item-group-title`），
由 `MenuItemGroup` 的标题 ContentPresenter 承载。声明位于
`DropdownButton.SemanticParts.cs` partial 文件；`root` 为隐式 Part，不在该文件中显式声明。

五个弹层部件全部声明 `CrossVisualRoot=true` + `RuntimeCreated=true`，marker 在运行时注入：`popup.root` 由
`MenuFlyoutPresenter.OnApplyTemplate` 创建弹层根视觉面 `ArrowDecoratedBox` 时注入（边框 / 背景 / 圆角由
`ArrowDecoratedBox` 的 `PART_ContentDecorator` 渲染，`MenuFlyoutPresenter` 只是共享的菜单宿主容器）；`item` 由
`MenuFlyoutPresenter` 与 `MenuItem`
的容器创建路径（`CreateContainerForItemOverride` + `PrepareContainerForItemOverride`）注入，同时覆盖顶层菜单项
与嵌套子菜单项；`itemIcon` / `itemContent` 由 `MenuItem.OnApplyTemplate` 注入到 `ItemIconPresenter` /
`ItemTextPresenter` 模板节点（另声明 `CrossNestedOwners=true`）；`itemTitle` 由 `MenuItemGroup.OnApplyTemplate`
注入到分组标题 `GroupTitlePresenter` 模板节点（分组容器本身带 `semantic-item-title-group` 中间标记类，
另声明 `CrossNestedOwners=true`）。该形态沿用 FlyoutHost → FlyoutPresenter 的
跨视觉根弹层先例，避免把 marker 静态写进被 SplitButton、DataGrid、TabControl、Transfer 等复用的共享
MenuFlyout / MenuItem 控件。

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `DropdownButton` |
| Part | `root` |
| Selector | DropdownButton 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `DropdownButton` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | DropdownButton owner |
| 职责 | DropdownButton root 是动作内容、菜单数据、弹层与状态的组织边界。 |
| 相关 API | 全部 DropdownButton public API |
| 相关 Token | DropdownButtonToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `popup.root`

| 字段 | 值 |
| --- | --- |
| Owner | `DropdownButton` |
| Part | `popup.root` |
| Selector | `.semantic-popup-root` |
| SelectorRoute | `>> .semantic-popup-root` |
| Style Type | `DropdownButtonPopupRootStyle` |
| ContractType | `ArrowDecoratedBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `MenuFlyoutPresenter.OnApplyTemplate` 定位到的弹层根视觉面 `ArrowDecoratedBox`（模板应用时注入 marker，其逻辑祖先链经 Popup `PlacementTarget` 回到 DropdownButton） |
| 职责 | 下拉菜单弹层的根视觉面，承载菜单项集合与弹层根视觉（边框 / 背景 / 圆角由 `ArrowDecoratedBox` 渲染，对应上游的 `root`）。 |
| 相关 API | `DropdownFlyout`、`Items`、`ItemTemplate`、`ItemContainerTheme` |
| 相关 Token | MenuToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `itemTitle`

| 字段 | 值 |
| --- | --- |
| Owner | `DropdownButton` |
| Part | `itemTitle` |
| Selector | `.semantic-item-title` |
| SelectorRoute | `>> .semantic-item-title-group /template/ .semantic-item-title` |
| Style Type | `DropdownButtonItemTitleStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `MenuItemGroup` 模板 `GroupTitlePresenter`（`ContentPresenter`，`MenuItemGroup.OnApplyTemplate` 时注入 marker；分组容器本身带 `semantic-item-title-group` 中间标记类） |
| 职责 | 菜单分组标题节点（对应上游的 `itemTitle`，即 `ant-menu-item-group-title`）。 |
| 相关 API | `MenuItemGroup.Header`、`MenuItemGroup.HeaderTemplate` |
| 相关 Token | MenuToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `item`

| 字段 | 值 |
| --- | --- |
| Owner | `DropdownButton` |
| Part | `item` |
| Selector | `.semantic-item` |
| SelectorRoute | `>> .semantic-item` |
| Style Type | `DropdownButtonItemStyle` |
| ContractType | `MenuItem` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `MenuFlyoutPresenter.CreateContainerForItemOverride` / `PrepareContainerForItemOverride` 与 `MenuItem.CreateContainerForItemOverride` / `PrepareContainerForItemOverride` 容器路径生成的 `MenuItem`（顶层与任意嵌套层级的子菜单项，回收复用时 marker 保持不变） |
| 职责 | 弹层中的单个菜单项容器，承载该项的状态、内容、图标与子菜单（对应上游的 `item`）。 |
| 相关 API | `Items`、`MenuItem.Header`、`MenuItem.Icon`、`MenuItem.Items`、`ItemTemplate` |
| 相关 Token | MenuToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `itemIcon`

| 字段 | 值 |
| --- | --- |
| Owner | `DropdownButton` |
| Part | `itemIcon` |
| Selector | `.semantic-item-icon` |
| SelectorRoute | `>> .semantic-item /template/ .semantic-item-icon` |
| Style Type | `DropdownButtonItemIconStyle` |
| ContractType | `IconPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `MenuItem` 模板 `ItemIconPresenter`（`IconPresenter`，`MenuItem.OnApplyTemplate` 时注入 marker） |
| 职责 | 菜单项模板内的图标节点（对应上游的 `itemIcon`）。 |
| 相关 API | `MenuItem.Icon` |
| 相关 Token | MenuToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `itemContent`

| 字段 | 值 |
| --- | --- |
| Owner | `DropdownButton` |
| Part | `itemContent` |
| Selector | `.semantic-item-content` |
| SelectorRoute | `>> .semantic-item /template/ .semantic-item-content` |
| Style Type | `DropdownButtonItemContentStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `MenuItem` 模板 `ItemTextPresenter`（`ContentPresenter`，`MenuItem.OnApplyTemplate` 时注入 marker） |
| 职责 | 菜单项模板内的文本内容节点（对应上游的 `itemContent`）。 |
| 相关 API | `MenuItem.Header`、`MenuItem.HeaderTemplate`、`ItemTemplate` |
| 相关 Token | MenuToken、SharedToken |
| 稳定性 | stable since 6.0 |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonBaseTheme.axaml`

```xml
<Panel>
    <WaveSpiritDecorator Name="PART_WaveSpirit" />
    <Border Name="ShadowsFrame" />
    <DashedBorder Name="Frame" />
    <Border>
        <DockPanel Name="PART_RootLayout">
            <LoadingOutlined Name="PART_LoadingIcon" />
            <IconPresenter Name="PART_ButtonIcon" />
            <ContentPresenter Name="PART_ContentPresenter" />
        </DockPanel>
    </Border>
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
DropdownButton
  -> DropdownButton (control theme, DropdownButtonBaseTheme.axaml)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> DashedBorder#Frame (template-stable)
        -> Border (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> LoadingOutlined#PART_LoadingIcon (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> DashedBorder#Frame (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> LoadingOutlined#PART_LoadingIcon (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> DashedBorder#Frame (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> LoadingOutlined#PART_LoadingIcon (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
  -> DropdownButton (control theme, DropdownButtonTheme.axaml)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> DashedBorder#Frame (template-stable)
        -> Border (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> IconPresenter#PART_DropdownIndicator (template-stable)
              -> LoadingOutlined#PART_LoadingIcon (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `DropdownButton` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `DropdownButton` | control theme | `DropdownButtonBaseTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `EffectiveBorderThickness` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `DropdownButtonBaseTheme.axaml` | DropdownButton | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `EffectiveBorderThickness` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_WaveSpirit` | template node (WaveSpiritDecorator) | `DropdownButtonBaseTheme.axaml` | DropdownButton | `EffectiveCornerRadius`, `IsMotionEnabled`, `IsWaveSpiritEnabled`, `WaveSpiritType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ShadowsFrame` | template node (Border) | `DropdownButtonBaseTheme.axaml` | DropdownButton | `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (DashedBorder) | `DropdownButtonBaseTheme.axaml` | DropdownButton | `Background`, `BackgroundSizing`, `BorderBrush`, `EffectiveBorderThickness`, `EffectiveCornerRadius`, `Height` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RootLayout` | template node (DockPanel) | `DropdownButtonBaseTheme.axaml` | DropdownButton | `Content`, `ContentTemplate`, `Foreground`, `HorizontalContentAlignment`, `Icon`, `IconHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LoadingIcon` | template node (LoadingOutlined) | `DropdownButtonBaseTheme.axaml` | DropdownButton | `Foreground`, `IconHeight`, `IconWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ButtonIcon` | template node (IconPresenter) | `DropdownButtonBaseTheme.axaml` | DropdownButton | `Foreground`, `Icon`, `IconHeight`, `IconWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `DropdownButtonBaseTheme.axaml` | DropdownButton | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DropdownButton` | control theme | `DropdownButtonTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `EffectiveBorderThickness` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `DropdownButtonTheme.axaml` | DropdownButton | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `EffectiveBorderThickness` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_WaveSpirit` | template node (WaveSpiritDecorator) | `DropdownButtonTheme.axaml` | DropdownButton | `EffectiveCornerRadius`, `IsMotionEnabled`, `IsWaveSpiritEnabled`, `WaveSpiritType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ShadowsFrame` | template node (Border) | `DropdownButtonTheme.axaml` | DropdownButton | `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (DashedBorder) | `DropdownButtonTheme.axaml` | DropdownButton | `Background`, `BackgroundSizing`, `BorderBrush`, `EffectiveBorderThickness`, `EffectiveCornerRadius`, `Height` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RootLayout` | template node (DockPanel) | `DropdownButtonTheme.axaml` | DropdownButton | `Content`, `ContentTemplate`, `Foreground`, `HorizontalContentAlignment`, `Icon`, `IconHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_DropdownIndicator` | template node (IconPresenter) | `DropdownButtonTheme.axaml` | DropdownButton | `Foreground`, `IsShowOpenIndicator`, `OpenIndicator` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LoadingIcon` | template node (LoadingOutlined) | `DropdownButtonTheme.axaml` | DropdownButton | `Foreground`, `IconHeight`, `IconWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ButtonIcon` | template node (IconPresenter) | `DropdownButtonTheme.axaml` | DropdownButton | `Icon`, `IconHeight`, `IconWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `DropdownButtonTheme.axaml` | DropdownButton | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content`、`Icon`、`Command` | 继承 Button 的展示内容、图标和动作命令入口。 |
| 交互与状态 | `IsArrowVisible`、`IsPointAtCenter`、`IsShowOpenIndicator`、`ShouldUseOverlayPopup` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `IconWidth`、`IconHeight`、`MarginToAnchor`、`Placement`、`PlacementAnchor`、`PlacementGravity` | 继承 Button 的用户图标和 loading 图标尺寸入口，并管理下拉定位、密度和模板视觉变量。 |
| 弹层与窗口 | `DropdownFlyout` | 控制 popup、flyout、dialog、window 或 overlay 宿主协作。 |
| 动效与异步 | `MouseEnterDelay`、`MouseLeaveDelay` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `OpenIndicator`、`TriggerType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | SharedToken / 关联控件 Token + ControlTheme。 |

## State Flow

DropdownButton 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- open/close、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

DropdownButton 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `DropdownButtonBaseTheme.axaml` | 定义 DropdownButton 与 Button 共享的模板、图标尺寸和基础状态视觉。 |
| `DropdownButtonTheme.axaml` | 定义 DropdownButton 的下拉指示器和具体视觉入口。 |

DropdownButton 当前没有专属 Token 文档；主题通过 SharedToken、关联控件 Token 或继承主题资源表达视觉语义。运行时状态不得写入 Token 模型。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Native 和 Browser 支持宿主必须使用同一套 DropdownButton 主题资产；平台差异不能通过 Browser 专用主题分叉复制视觉。
- 用户 icon 与 loading icon 的尺寸只能从 DropdownButton 自身的 `IconWidth`、`IconHeight` 投影；应用和 Gallery 不得通过 `/template/` 或 `PART_ButtonIcon`、`PART_LoadingIcon` selector 修改内部尺寸。
- `OpenIndicator` 尺寸由 DropdownButton 主题单独管理，不复用用户 icon 的宽高属性。

Token 边界：

- DropdownButton 当前没有专属 `token.md`；LLMS 生成按第 5 节视觉与主题模型、SharedToken、控件家族 Token 或主题资源说明 Token 边界。

## Customization Boundaries

维护 DropdownButton 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- `IconWidth`、`IconHeight` 本地值必须同时覆盖用户 icon 与 loading icon 的 Theme 默认值，且不得改变 `OpenIndicator` 尺寸。
- 非 loading 的 icon-only 用户图标继续使用普通 `IconSize*` 默认值；只有 icon-only loading 使用 DropdownButton 对应的 `OnlyIconSize*` 默认值。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 DropdownButton 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- `IconWidth`、`IconHeight` 的本地值优先级、两个图标 part 的一致投影以及 `OpenIndicator` 的独立尺寸职责。
- 外部样式不得使用 `/template/` 或 part selector 修改用户 icon、loading icon 的宽高。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
