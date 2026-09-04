# NavMenu 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `NavMenu` | 导航控件根语义区域，承载 public API、状态归一和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载点击、键盘、打开关闭、跳转或提交入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `导航项区域` | 承载当前项、选中项、禁用项、层级项或分页项状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或内容区域` | 承载 flyout、dropdown、tab content、submenu 或候选内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达打开关闭、选中指示、切换和过渡反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/NavMenu/Themes/NavMenuTheme.axaml`

```xml
<PixelAlignedBorder>
    <Grid>
        <ContentPresenter Name="PART_HeaderPresenter" />
        <ScrollViewer>
            <ItemsPresenter Name="PART_ItemsPresenter" />
        </ScrollViewer>
        <ContentPresenter Name="PART_FooterPresenter" />
    </Grid>
</PixelAlignedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
NavMenu
  -> NavMenuDividerItem (item container control theme, NavMenuDividerItemTheme.axaml)
     -> PixelAlignedBorder (template-stable)
  -> NavMenuGroupItem (item container control theme, NavMenuGroupItemTheme.axaml)
     -> StackPanel (template-stable)
        -> ContentPresenter#PART_HeaderPresenter (template-stable)
        -> ItemsPresenter#PART_ItemsPresenter (template-stable)
  -> NavMenuItem (item container control theme, NavMenuItemTheme.axaml)
     -> Panel (template-stable)
        -> HorizontalNavMenuItemHeader#PART_Header (template-stable)
        -> Popup#PART_Popup (template-stable)
           -> NavMenuPopupFrame#PART_PopupFrame (template-stable)
              -> ScrollViewer (template-stable)
                 -> ItemsPresenter#PART_ItemsPresenter (template-stable)
     -> Panel (template-stable)
        -> VerticalNavMenuItemHeader#PART_Header (template-stable)
        -> Popup#PART_Popup (template-stable)
           -> NavMenuPopupFrame#PART_PopupFrame (template-stable)
              -> ScrollViewer (template-stable)
                 -> ItemsPresenter#PART_ItemsPresenter (template-stable)
     -> StackPanel (template-stable)
        -> InlineNavMenuItemHeader#PART_Header (template-stable)
        -> LayoutAwareMotionActor#PART_ChildItemsLayoutTransform (template-stable)
           -> Border#PART_ChildItemsFrame (template-stable)
              -> ItemsPresenter#ChildItemsPresenter (internal-observable)
  -> NavMenu (control theme, NavMenuTheme.axaml)
     -> PixelAlignedBorder (template-stable)
        -> Grid (template-stable)
           -> ContentPresenter#PART_HeaderPresenter (template-stable)
           -> ScrollViewer (template-stable)
              -> ItemsPresenter#PART_ItemsPresenter (template-stable)
           -> ContentPresenter#PART_FooterPresenter (template-stable)
     -> DockPanel (template-stable)
        -> PixelAlignedBorder#PART_HorizontalLine (template-stable)
        -> PixelAlignedBorder (template-stable)
           -> Grid (template-stable)
              -> ContentPresenter#PART_HeaderPresenter (template-stable)
              -> ItemsPresenter#PART_ItemsPresenter (template-stable)
              -> ContentPresenter#PART_FooterPresenter (template-stable)
  -> VerticalNavMenuItemHeader (control theme, VerticalNavMenuItemHeaderTheme.axaml)
     -> Border#Frame (template-stable)
        -> Grid#HeaderLayout (template-stable)
           -> IconPresenter#ItemIconPresenter (internal-observable)
           -> ContentPresenter#ItemTextPresenter (internal-observable)
           -> ContentPresenter#CollapsedTitlePresenter (internal-observable)
           -> RightOutlined#MenuIndicatorIcon (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `NavMenu` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `NavMenuDividerItem` | item container control theme | `NavMenuDividerItemTheme.axaml` | NavMenu | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `NavMenuGroupItem` | item container control theme | `NavMenuGroupItemTheme.axaml` | NavMenu | `EntryItemSpacing`, `Header`, `HeaderTemplate`, `ItemsPanel` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `StackPanel` | template node (StackPanel) | `NavMenuGroupItemTheme.axaml` | NavMenuGroupItem | `Header`, `HeaderTemplate`, `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderPresenter` | template node (ContentPresenter) | `NavMenuGroupItemTheme.axaml` | NavMenuGroupItem | `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `NavMenuGroupItemTheme.axaml` | NavMenuGroupItem | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `NavMenuItem` | item container control theme | `NavMenuItemTheme.axaml` | NavMenu | `CollapsedTooltipBetweenShowDelay`, `CollapsedTooltipPlacement`, `CollapsedTooltipShowDelay`, `EffectiveCollapsedTooltip`, `EffectivePopupMinWidth`, `EntryItemSpacing` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `NavMenuItemTheme.axaml` | NavMenuItem | `EffectivePopupMinWidth`, `HasSubMenu`, `Header`, `HeaderTemplate`, `Icon`, `IsDarkStyle` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Header` | template node (HorizontalNavMenuItemHeader) | `NavMenuItemTheme.axaml` | NavMenuItem | `HasSubMenu`, `Header`, `HeaderTemplate`, `Icon`, `IsDarkStyle`, `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Popup` | template node (Popup) | `NavMenuItemTheme.axaml` | NavMenuItem | `EffectivePopupMinWidth`, `IsMotionEnabled`, `ItemsPanel`, `ShouldUseOverlayPopup`, `atom` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PopupFrame` | template node (NavMenuPopupFrame) | `NavMenuItemTheme.axaml` | NavMenuItem | `EffectivePopupMinWidth`, `IsMotionEnabled`, `ItemsPanel`, `atom` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `NavMenuItemTheme.axaml` | NavMenuItem | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Header` | template node (VerticalNavMenuItemHeader) | `NavMenuItemTheme.axaml` | NavMenuItem | `CollapsedTooltipBetweenShowDelay`, `CollapsedTooltipPlacement`, `CollapsedTooltipShowDelay`, `EffectiveCollapsedTooltip`, `HasSubMenu`, `Header` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `NavMenuItemTheme.axaml` | NavMenuItem | `Focusable`, `HasSubMenu`, `Header`, `HeaderTemplate`, `Icon`, `IsDarkStyle` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Header` | template node (InlineNavMenuItemHeader) | `NavMenuItemTheme.axaml` | NavMenuItem | `Focusable`, `HasSubMenu`, `Header`, `HeaderTemplate`, `Icon`, `IsDarkStyle` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ChildItemsLayoutTransform` | template node (LayoutAwareMotionActor) | `NavMenuItemTheme.axaml` | NavMenuItem | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ChildItemsFrame` | template node (Border) | `NavMenuItemTheme.axaml` | NavMenuItem | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ChildItemsPresenter` | template node (ItemsPresenter) | `NavMenuItemTheme.axaml` | NavMenuItem | `ItemsPanel` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `NavMenu` | control theme | `NavMenuTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BackgroundSizing`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `Footer` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_HeaderPresenter` | template node (ContentPresenter) | `NavMenuTheme.axaml` | NavMenu | `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `NavMenuTheme.axaml` | NavMenu | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FooterPresenter` | template node (ContentPresenter) | `NavMenuTheme.axaml` | NavMenu | `Footer`, `FooterTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `NavMenuTheme.axaml` | NavMenu | `Background`, `BackgroundSizing`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `Footer` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HorizontalLine` | template node (PixelAlignedBorder) | `NavMenuTheme.axaml` | NavMenu | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `VerticalNavMenuItemHeader` | control theme | `VerticalNavMenuItemHeaderTheme.axaml` | NavMenu | `Background`, `CornerRadius`, `Header`, `HeaderTemplate`, `Height`, `Icon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (Border) | `VerticalNavMenuItemHeaderTheme.axaml` | VerticalNavMenuItemHeader | `Background`, `CornerRadius`, `Header`, `HeaderTemplate`, `Height`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderLayout` | template node (Grid) | `VerticalNavMenuItemHeaderTheme.axaml` | VerticalNavMenuItemHeader | `Header`, `HeaderTemplate`, `Icon`, `IsEnabled`, `NodeHeader` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemIconPresenter` | template node (IconPresenter) | `VerticalNavMenuItemHeaderTheme.axaml` | VerticalNavMenuItemHeader | `Icon`, `IsEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ItemTextPresenter` | template node (ContentPresenter) | `VerticalNavMenuItemHeaderTheme.axaml` | VerticalNavMenuItemHeader | `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `CollapsedTitlePresenter` | template node (ContentPresenter) | `VerticalNavMenuItemHeaderTheme.axaml` | VerticalNavMenuItemHeader | `NodeHeader` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `MenuIndicatorIcon` | template node (RightOutlined) | `VerticalNavMenuItemHeaderTheme.axaml` | VerticalNavMenuItemHeader | `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ItemsPresenter` | `ItemsPresenter` | 承载顶层 entry 容器。 |
| `PART_HeaderPresenter` | `ContentPresenter` | 承载根导航固定 Header；内容为空时折叠。 |
| `PART_FooterPresenter` | `ContentPresenter` | 承载根导航固定 Footer；内容为空或处于有效 inline collapsed 状态时折叠。 |
| `PART_HorizontalLine` | `PixelAlignedBorder` | Horizontal light style 下的底部分割线。 |
| `PART_Header` | `BaseNavMenuItemHeader` 的 mode 专用实现 | 菜单项 header，承载文字、图标、箭头和交互视觉。 |
| `PART_Popup` | `Popup` | `Vertical` / `Horizontal` 模式下的子菜单浮层。 |
| `PART_PopupFrame` | `NavMenuPopupFrame` | Popup 背景、圆角、尺寸约束和内容边距。 |
| `PART_ChildItemsLayoutTransform` | `LayoutAwareMotionActor` | `Inline` 模式下的子菜单展开收起 motion 容器。 |
| `PART_ChildItemsFrame` | `Border` | `Inline` 模式下的子菜单背景块。 |
| `ChildItemsPresenter` | `ItemsPresenter` | `Inline` 模式下的子菜单内容承载。 |
| `PART_ActiveIndicator` | `Rectangle` | `Horizontal` 顶层 light style 下的活动指示条。 |

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

NavMenu 的交互行为由 mode 决定。所有 mode 共用项激活事务契约：指针按下只建立待提交事务并尝试移动真实焦点，合法释放（按下与释放命中同一项）才按固定顺序提交选择并派发命令与事件，其余中断路径一律取消。键盘 Enter/Space 与指针合法释放复用激活入口；程序化 `SelectedItem` 是独立的选择入口，只同步选择状态，不执行节点命令或触发 `NavMenuItemClick`。完整状态机、提交顺序、同步重入和取消路径见 [NavMenu 项激活事务设计](item-activation-design.md)。

`Inline` 模式：

- 合法释放带子菜单的项目时切换 `IsSubMenuOpen`。
- 子菜单在当前视觉树中展开，使用 `LayoutAwareMotionActor` 承载展开收起 motion。
- 合法释放叶子节点时提交选择，并更新所有祖先 `IsInSelectedPath`。
- `IsAccordionMode=true` 时，顶层子菜单互斥展开。
- `IsInlineCollapsed=true` 时，public `Mode` 仍保持 `Inline`，但内部有效模式切换为 vertical popup 语义：顶层只显示图标或无图标首字符，inline 子树不在主视觉树中展开，带子菜单的顶层项目通过 popup 打开。
- 有效 inline collapsed 状态下，顶层叶子节点通过实际 header control 承载 Tooltip。节点显式 `Tooltip` 优先；未设置时回退到 `Header`；菜单级或节点级 Tooltip 被禁用、节点拥有子菜单或退出有效折叠状态时，不创建有效提示内容。
- 进入折叠时缓存当前 inline 打开路径并关闭主视觉树中的 inline 子菜单；退出折叠时恢复缓存路径。折叠和展开不得清空 `SelectedItem` 或 selected path。
- 键盘 Up / Down 在当前可见层级内移动 active/focus 项。
- Enter 在带子菜单项上切换展开状态，在叶子节点上提交选择。
- Left / Right 可作为桌面增强支持折叠或展开当前 active 子菜单，并保持 keyboard active 在当前项；叶子项上为 no-op，不能改变 `SelectedItem`。

`Vertical` 与 `Horizontal` 模式：

- 带子菜单的项目通过 Popup 展开。
- hover 可以延迟打开子菜单；pointer 离开后延迟关闭。hover 打开流程独立于激活事务。
- 合法释放叶子节点时提交选择；弹出层关闭由 pointer、窗口失焦、非客户端点击和同级打开状态共同控制。这些关闭入口只结束临时 popup open state，不清空持续的 `SelectedItem` 或 selected path。
- `Horizontal` 顶层菜单 popup 位于下方；非顶层 popup 按右侧边缘对齐。
- 键盘导航以当前打开的可见菜单层级为边界移动 active/focus 项，跳过禁用项、分割线和不可聚焦内容。
- 键盘 active 初次移动时优先以当前可见且已生成的 `SelectedItem` 容器作为方向键锚点，并立即移动到前一个或后一个可导航节点；如果没有选中项，或选中项隐藏在未打开的子菜单中，则从第一个可导航节点开始。
- `Horizontal` 顶层菜单使用 Left / Right 在顶层兄弟项之间移动，Down 或 Enter 打开当前 active 子菜单并进入子菜单第一项。
- `Vertical` 根层和所有 popup 子菜单使用 Up / Down 在同层兄弟项之间移动，Right 或 Enter 打开当前 active 子菜单并进入子菜单第一项，Left 或 Esc 返回父级并关闭当前 popup 分支。
- Enter 在叶子节点上提交选择并触发 `NavMenuNodeSelected` / `NavMenuItemClick`；方向键只改变 active/focus，不触发选择。
- Esc 只关闭当前键盘导航所在的 popup 分支并返回父级 active 项，不调用 `Close()`，因此不能清空 `SelectedItem`。

公共交互状态：

- `Disabled` 由节点 `IsEnabled` 和 command can-execute 共同决定，禁用项不应触发有效点击。
- 节点命令必须复用 `NavMenuItem` 的有效激活入口；pointer 与 keyboard 提交不能形成两条独立命令执行路径，也不能因选择事件再次执行命令。
- `PointerOver` 改变 header 前景和背景，但不能改变选中路径。
- `PressCandidate` 表示激活事务的待提交项，建立后一直保留到提交或取消；只有指针仍命中该项时才贡献与 selected 相同的背景，文字颜色保持按下前状态，不影响选中状态。
- `KeyboardActive` 表示键盘漫游中的当前项，只影响键盘导航锚点和 active 视觉，不改变选中路径；pointer-hold 与 keyboard-active 由独立 owner 维护，可以同时存在。
- `Selected` 表示当前叶子节点已提交选中。
- `IsInSelectedPath` 表示某个祖先位于当前选中路径中。
- `Open` 表示当前项目子菜单打开。

结构 entry 不进入公共交互状态：分组和分隔线不产生 `Selected`、`KeyboardActive`、`Open`、`ItemKey` 或 `Command`。分组中的节点仍使用最近的节点祖先作为 `ParentNode`；分组本身不增加 `Level`，也不进入 `TreeNodePath`。

## Theme and Token Boundaries

NavMenu Theme 按 mode、dark style、header state 和 item background model 分层。

```text
NavMenuTheme
  root template by Mode
  fixed Header / scrollable entry region / fixed Footer
  root background / padding / scroll behavior
  top-level ItemsPanel orientation

NavMenuItemTheme
  item template by Mode
  popup frame
  inline child frame
  motion duration

NavMenuGroupItemTheme / NavMenuDividerItemTheme
  non-interactive structure containers
  group title / divider orientation

Header Themes
  shared text/icon/background/selection
  horizontal active bar
  inline indentation and arrow rotation
```

Theme 映射规则：

- Root 背景使用 `ItemBg`，Dark root 背景使用 `DarkMenuBg`。
- Popup 背景使用 `MenuPopupBg`，Dark popup 使用 `DarkMenuPopupBg`。
- Header 默认背景为 `Transparent`，hover / selected 背景由 header state 直接控制。
- Keyboard active 通过 `IsKeyboardActive` / `ItemActiveBg` 表达；指针按住通过独立的 `IsPointerHold` 只使用与 selected 相同的 `ItemSelectedBg`（dark 使用 `DarkItemSelectedBg`），不覆盖既有文字颜色。两者优先级都低于真实 `Selected`，清除其中一个 owner 不得覆盖另一个 owner 的状态。keyboard active 可以叠加在 `IsInSelectedPath` 父节点上，使父节点保留 selected-path 文字色的同时显示临时 active 背景。header 背景使用 `MotionDurationSlow`（默认 300ms）与 `ItemBackgroundMotionEasing`（默认 CSS `ease` 等价曲线），使 hover 灰在按下后过渡到 selected 色，而 selection 只在合法释放时提交。
- Inline collapsed 根宽度使用 `InlineCollapsedWidth`，默认来自 `NavMenuToken.InlineCollapsedWidth=48`。折叠视觉只作用于 `Mode=Inline && IsInlineCollapsed=true`：一级 icon 使用 `CollapsedIconSize` 居中，标题和箭头收起，未配置 icon 的一级项从节点 `Header` 显示首字符；顶层叶子项使用独立 `Tooltip`，未设置时回退到 `Header`。
- Inline/Vertical 的 Header 和 Footer 位于菜单滚动区之外；无 Header/Footer 时对应 presenter 折叠，不占用布局空间。Horizontal 中 Header 左停靠、Footer 右停靠，菜单项占用中间区域。进入 inline collapsed 后 Header 保持可见以承载展开入口，Footer 自动隐藏；Header 内容需要根据 `IsInlineCollapsed` 自适应折叠宽度。
- 根层 inline collapsed 分组标题隐藏，分组及其透明嵌套分组内的节点继续继承根折叠状态，按顶层节点使用 `CollapsedIconSize` 居中；popup 或非根语义层级中的分组标题和节点保持普通 vertical 视觉。Horizontal 根层把分组渲染为透明水平集合并隐藏标题，popup 中恢复垂直分组标题。
- Horizontal 根层分隔线为竖线；Inline、Vertical、popup 和 inline collapsed 根层分隔线为横线。
- 根默认 ItemsPanel 通过 `TemplateBinding` 消费公开 `ItemSpacing`；submenu、popup 和 group 默认 ItemsPanel 消费由根控件投影的内部 effective spacing。该路径不使用进入子控件模板的 selector，也不建立逐容器 binding。自定义 ItemsPanel 是否消费 spacing 由自定义面板负责。
- `IsItemBackgroundEnabled=true` 时，inline child frame 使用 `SubMenuItemBg` / `DarkSubMenuItemBg`，并应用背景块专用外距。
- `IsItemBackgroundEnabled=false` 时，inline child frame 背景为 `Transparent`，不应用背景块专用外距；header 的文字色、hover、selected 和 selected path 仍然生效。
- Horizontal 顶层 light style 通过 `PART_ActiveIndicator` 表达选中；dark style 可以使用 selected background。

Header 背景与 NavMenuItem / inline submenu 背景块是不同职责，不应混为一个 selector 控制。

Token 边界：

NavMenuToken 是 NavMenu 的组件级设计变量层。它把全局颜色、尺寸、间距、圆角、字体、动效和 popup 体系转换为 NavMenu 可消费的语义值。

NavMenuToken 服务以下主题：

- `NavMenuTheme.axaml`
- `NavMenuItemTheme.axaml`
- `NavMenuGroupItemTheme.axaml`
- `NavMenuDividerItemTheme.axaml`
- `BaseNavMenuItemHeaderTheme.axaml`
- `HorizontalNavMenuItemHeaderTheme.axaml`
- `VerticalNavMenuItemHeaderTheme.axaml`
- `InlineNavMenuItemHeaderTheme.axaml`

NavMenuToken 不承载 `SelectedItem`、`IsSubMenuOpen`、`IsInSelectedPath`、`IsPointerOverSubMenu`、`Level`、`IsTopLevel` 等实例状态。这些状态由控件状态模型、容器层和主题 selector 处理。

## Customization Boundaries

维护 NavMenu 时必须保持以下不变量：

- `NavMenuMode.Vertical`、`Horizontal`、`Inline` 的名称、默认行为和模板模式不变。
- `Mode` 默认值保持 `Inline`。
- `IsInlineCollapsed` 不引入新的 `NavMenuMode`，也不直接改写 `Mode`；折叠只通过内部 effective mode、theme state 和 popup 交互表达。
- `InlineCollapsedWidth` 默认由 `NavMenuToken.InlineCollapsedWidth` 提供，开发者本地设置必须能覆盖 token 默认值。
- `SelectedItem` 优先级高于 `DefaultSelectedPath`。
- `DefaultOpenPaths` 和 `DefaultSelectedPath` 不依赖固定时间延迟。
- pointer 外点、窗口停用、平台失焦、非客户端点击和 `Mode` 切换只关闭 popup/submenu，不得通过 `Close()` 隐式清空 `SelectedItem`；显式调用 public `Close()` 仍保持“关闭全部子菜单并清空选择”的既有合同。
- 进入或退出 inline collapsed 不得调用 `Close()`，不得清空 `SelectedItem`，不得丢失 selected path。
- inline collapsed 期间打开的 popup 状态不得污染展开后恢复的 inline open path cache。
- 键盘 active/focus 状态不得进入公共 API，不得改变 `SelectedItem`、`DefaultSelectedPath` 或 `DefaultOpenPaths` 的语义。
- `NavMenuNode` / `INavMenuNode` 的 `Header`、`HeaderTemplate`、`Tooltip`、`IsTooltipEnabled`、`ItemKey`、`Icon`、`IsEnabled`、`Command`、`CommandParameter`、`Children` 名称、类型和语义不变。
- `Tooltip=null` 必须回退到节点 `Header`；折叠提示只作用于有效 inline collapsed 状态下的顶层叶子节点，不能扩展到带子菜单节点、普通 Vertical/Horizontal 或展开后的 Inline 状态。
- `NavMenuNode.Entries` 是子 entry 唯一真源；`Children` 只能作为同一集合的实时节点兼容视图，不能引入第二份节点集合或双向同步状态。
- direct `Items`、`ItemsSource`、节点 `Entries` 和分组 `Entries` 对非法 entry 的拒绝语义一致；不能因 source 是否只读或集合通知类型不同而绕过验证。
- 同一内置 `NavMenuNode` / `NavMenuGroup` 实例在 entry 树中只能有一个直接结构 owner；释放 owner 后才允许重挂载。无状态 `NavMenuDivider` 可以复用。
- custom `INavMenuNode` 本身保持兼容，但它暴露的内置 node/group 仍必须参加完整 entry 图唯一性校验；嵌套可通知 source 使用弱订阅。
- `NavMenuGroup` 和 `NavMenuDivider` 在任意数据层级都保持结构语义，不进入选择、命令、路径、层级缩进或键盘状态。
- 根分组中的节点仍为顶层节点；嵌套分组不能改变节点的 `ParentNode`、`Level` 或 `IsTopLevel`。
- `Header` / `Footer` 固定区域不能进入菜单 ItemsPanel 或随菜单项滚动；空 content 不得改变既有无 slot 布局。
- `ItemSpacing` 只控制根默认 ItemsPanel，并在具有有效设置时覆盖后代默认 ItemsPanel 的额外容器间距；未设置的 Horizontal 根层保持 `0`，其 popup、submenu 和 group 仍使用 `VerticalItemsPanelSpacing`。该属性不重定义 `ItemContentMargin`、`VerticalChildItemsMargin` 或自定义 ItemsPanel 的布局语义。
- `NavMenuNode` 只承载命令配置，不实现 `ICommandSource`，不直接订阅 `CanExecuteChanged`，也不保存当前 `NavMenuItem` 容器。
- `CommandParameter` 保持标准显式参数语义，不隐式回退到 `ItemKey`、`Header`、`SelectedItem` 或节点自身。
- `NavMenuItemClick` 和 `NavMenuNodeSelected` 的事件语义不变。
- `SelectedItem` 只表示已提交选择；指针按下不改变选择、不执行命令、不触发 `NavMenuItemClick` 或 `NavMenuNodeSelected`。
- 指针激活只有"合法释放提交"一种行为：按下与释放命中同一项才提交；释放到其他节点、菜单外以及激活事务的全部取消路径均不产生选择、命令或事件。
- 叶子提交顺序固定为：选中路径与 `IsSelected` 更新、`SelectedItem` 更新、`NavMenuNodeSelected`、节点 `Command`、`NavMenuItemClick`。`NavMenuNodeSelected` 开始派发前，`SelectedItem` 必须仍指向该事件节点；若同步观察者或事件处理器改写选择，原节点提交被视为 superseded，并停止尚未发生的事件或动作。父节点提交不修改 `SelectedItem`、不触发 `NavMenuNodeSelected`。
- 键盘 Enter/Space 与指针合法释放必须共用同一提交入口和事件顺序。
- pointer-hold 的背景必须与 selected 完全一致、文字颜色保持不变，且不得提前写入 selected 状态。按下先交接 pointer-hold 再捕获带 `Cursor=Hand` 的 header；合法释放时保留 pointer-hold 直到 selection 提交完成，再清除临时状态，过程不得闪回 hover、透明或默认背景。
- 方向键移动 active 项不得触发 `NavMenuItemClick` 或 `NavMenuNodeSelected`。
- Esc 关闭 popup 分支不得调用 `Close()`，不得清空已选中节点。
- `IsAccordionMode=true` 只控制同层展开互斥，不改变选中节点。
- `IsItemBackgroundEnabled=false` 不应关闭 header 前景色、hover、selected、selected path 或 disabled 视觉，只关闭 item / submenu 背景块。
- inline 子菜单背景块外距只在 `IsItemBackgroundEnabled=true` 时生效。
- popup frame 使用 `MenuPopupBg` / `DarkMenuPopupBg`，不回退为普通 shared elevated background。
- root background、popup background、header background 和 inline submenu background 必须保持职责分离。
- 点击子节点时，不应让父级 header 出现错误 hover 背景。
- NavMenu 优化不得关闭 motion 来规避点击、打开或关闭问题。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- mode 切换时重新挂接 handler，并通过保留选择的关闭路径清理旧模式打开状态；当前 `SelectedItem` 和 selected path 必须在当前容器上继续投影。
- pointer 外点、窗口停用、平台失焦和非客户端点击关闭 popup 时不得调用 `NavMenu.Close()`，不得清空 `SelectedItem`；public `Close()` 的显式清空合同保持不变。
- `IsInlineCollapsed` 切换不得改写 public `Mode`，不得调用 `Close()`，不得清空 `SelectedItem`。
- inline collapsed 进入时缓存 inline open path，退出时恢复 cache；折叠期间 popup 打开状态不得污染 cache。
- `InlineCollapsedWidth` 默认来自 `NavMenuToken.InlineCollapsedWidth`，本地属性值必须按 Avalonia 优先级覆盖 token 默认值。
- 折叠视觉不能通过改写 `Header`、删除 `HeaderTemplate` 或动态创建替代 header 实现。
- 节点 `Tooltip` 与 `Header` 保持独立；未设置 `Tooltip` 时才回退到 `Header`，不能引入第二个标题属性代替 Tooltip 语义。
- `ToolTip.Tip` 只能附加到实际 header control，不能扩大到非 Visual `NavMenuNode`；有效内容只由 `NavMenuItem` 计算。
- container rebind、clear 和 recycle 必须同时释放节点 Tooltip binding 与菜单 Tooltip policy binding，并使旧 header 的有效 `ToolTip.Tip` 归零。
- 点击 item 不得临时关闭 motion。
- 默认路径应用不使用固定 50ms sleep 作为稳定策略。
- selection coordinator 是选择状态的统一入口。
- keyboard active/focus 状态不能替代 selection coordinator。
- 激活事务由 interaction handler 基类唯一持有；按下只建立事务、置仅覆盖背景的 pointer-hold selected-background 视觉并按 mode 尝试移动真实焦点，不覆盖 keyboard-active owner、不写 selected 状态、不进入 selection coordinator、不执行命令、不触发路由事件、不切换 inline 展开状态。
- 指针提交以“释放点命中待提交项视觉子树”为唯一合法性判据，不使用捕获期间的 `IsPointerOver`；取消路径（拖离释放、当前指针捕获丢失、节点移除或禁用、detach、非主按钮释放、新按下替代）零副作用，其他指针的 capture-lost 不得误取消。
- 调用命令与触发 `NavMenuItemClick` 前必须先释放指针捕获并清理事务字段，防止用户回调重入时残留旧事务。
- 叶子提交顺序固定：选中路径与 `IsSelected` 更新、`SelectedItem` 更新、`NavMenuNodeSelected`、节点 `Command`、`NavMenuItemClick`；同步重入替换选择时停止原提交尚未发生的事件与动作。父节点提交不修改 `SelectedItem`。
- pointer-hold 与 keyboard-active 独立持有并分别投影到 header 的 `IsPointerHold` 与 `IsKeyboardActive`；前者只使用 selected 背景 Token 且不覆盖文字颜色，后者使用 active Token。pointer-hold 不写入 keyboard active、`IsSelected` / `IsInSelectedPath`，也不直接依赖捕获期间的原生 `:pointerover`。
- 方向键移动不得触发点击或选中事件。
- keyboard active 初始解析可以复用可见 `SelectedItem` 容器作为方向键移动锚点，但不得吃掉第一次方向键、触发选择事件或自动打开隐藏分支。
- Esc 关闭 popup 分支不得调用 `NavMenu.Close()`，不得清空 `SelectedItem`。
- 键盘打开 popup 或 inline 子项不得依赖固定 timer 生成容器。
- header hover / selected 背景不通过父级 item hover 状态误触发。
- `IsItemBackgroundEnabled=false` 不关闭 header 颜色和交互状态。
- popup、root、inline child frame、header 四类背景职责保持分离。
- handler 取消逻辑不能泄漏事件订阅或延迟任务。
- `NavMenuNode` 不实现 `ICommandSource`，不直接执行命令或订阅 `CanExecuteChanged`。
- scoped resource-host attachment、node relay binding 和 command subscription 必须各自具有确定释放点。
- `CanExecuteChanged` 的合并 operation 必须由当前 container 持有，并在 command / parameter 替换和 logical-tree detach 时取消。
- container rebind、clear、recycle、Items reset 和 re-template 后，旧节点、旧命令和旧 owner 不得继续持有当前容器。
- `CommandParameter` 不隐式使用 `ItemKey`，避免显式 `null` 和容器同步语义分叉。
- `Entries` 是唯一结构集合，`Children` 不得拥有第二份节点存储。
- direct `Items`、`ItemsSource`、节点 `Entries` 和分组 `Entries` 必须共用 `INavMenuEntry` 校验语义；初始装载、source replacement、Add、Replace 和 Reset 不得存在绕过路径。
- 内置 `NavMenuNode` / `NavMenuGroup` 必须以弱 structural owner 保证同一实例只有一个直接挂载位置；`ParentNode` 和 `SemanticParentNode` 不能替代该结构所有权。无状态 `NavMenuDivider` 不进入 owner 跟踪。
- custom node 自身不登记 owner，但其内置后代必须由最近 built-in/root scope 的完整图协调器检测；嵌套 collection subscription 必须是弱订阅并在 source 离图时释放。
- custom observable source 的 post-mutation 同步必须重新执行 owner-cycle 校验，不能把当前 built-in owner 或任意 built-in 祖先登记为自己的后代。
- built-in child collection 由 child 自身协调器负责，祖先不得递归订阅；纯 built-in 深树的订阅数量不得随祖先/后代组合增长为 O(N²)。
- 非法 entry 必须在容器生成和资源 attach 前确定性失败；不能静默忽略、降级为普通 content 或依赖后续 cast 暴露错误。
- 分组和分隔线不得实现或模拟 `INavMenuNode`、`ISelectable`、`ICommandSource` 或 keyboard active 状态。
- 节点的 `ParentNode`、容器的 `Level` / `IsTopLevel`、选择祖先和键盘父级只能来自 semantic owner，不得从逻辑树距离推导。
- 节点、分组和分隔线使用不同 recycle key，clear 必须移除各自 owner、binding 和状态。
- 纯节点菜单不增加结构容器、递归扁平缓存或每项 spacing binding。
- root Header/Footer 保持固定，空 slot 不占布局；结构标题和分隔线的 mode/collapsed 变体由各自内部 ControlTheme 维护。
