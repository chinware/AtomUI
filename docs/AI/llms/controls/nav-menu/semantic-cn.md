# NavMenu 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

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
<ScrollViewer>
    <PixelAlignedBorder>
        <ItemsPresenter Name="PART_ItemsPresenter" />
    </PixelAlignedBorder>
</ScrollViewer>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
NavMenu
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
     -> ScrollViewer (template-stable)
        -> PixelAlignedBorder (template-stable)
           -> ItemsPresenter#PART_ItemsPresenter (template-stable)
     -> DockPanel (template-stable)
        -> PixelAlignedBorder#PART_HorizontalLine (template-stable)
        -> PixelAlignedBorder (template-stable)
           -> ItemsPresenter#PART_ItemsPresenter (template-stable)
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
| `NavMenuItem` | item container control theme | `NavMenuItemTheme.axaml` | NavMenu | `EffectivePopupMinWidth`, `Focusable`, `HasSubMenu`, `Header`, `HeaderTemplate`, `Icon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `NavMenuItemTheme.axaml` | NavMenuItem | `EffectivePopupMinWidth`, `HasSubMenu`, `Header`, `HeaderTemplate`, `Icon`, `IsDarkStyle` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Header` | template node (HorizontalNavMenuItemHeader) | `NavMenuItemTheme.axaml` | NavMenuItem | `HasSubMenu`, `Header`, `HeaderTemplate`, `Icon`, `IsDarkStyle`, `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Popup` | template node (Popup) | `NavMenuItemTheme.axaml` | NavMenuItem | `EffectivePopupMinWidth`, `IsMotionEnabled`, `ItemsPanel`, `ShouldUseOverlayPopup`, `atom` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PopupFrame` | template node (NavMenuPopupFrame) | `NavMenuItemTheme.axaml` | NavMenuItem | `EffectivePopupMinWidth`, `IsMotionEnabled`, `ItemsPanel`, `atom` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `NavMenuItemTheme.axaml` | NavMenuItem | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Header` | template node (VerticalNavMenuItemHeader) | `NavMenuItemTheme.axaml` | NavMenuItem | `HasSubMenu`, `Header`, `HeaderTemplate`, `Icon`, `IsDarkStyle`, `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `NavMenuItemTheme.axaml` | NavMenuItem | `Focusable`, `HasSubMenu`, `Header`, `HeaderTemplate`, `Icon`, `IsDarkStyle` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Header` | template node (InlineNavMenuItemHeader) | `NavMenuItemTheme.axaml` | NavMenuItem | `Focusable`, `HasSubMenu`, `Header`, `HeaderTemplate`, `Icon`, `IsDarkStyle` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ChildItemsLayoutTransform` | template node (LayoutAwareMotionActor) | `NavMenuItemTheme.axaml` | NavMenuItem | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ChildItemsFrame` | template node (Border) | `NavMenuItemTheme.axaml` | NavMenuItem | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ChildItemsPresenter` | template node (ItemsPresenter) | `NavMenuItemTheme.axaml` | NavMenuItem | `ItemsPanel` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `NavMenu` | control theme | `NavMenuTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BackgroundSizing`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `Padding` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `NavMenuTheme.axaml` | NavMenu | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `NavMenuTheme.axaml` | NavMenu | `Background`, `BackgroundSizing`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HorizontalLine` | template node (PixelAlignedBorder) | `NavMenuTheme.axaml` | NavMenu | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `VerticalNavMenuItemHeader` | control theme | `VerticalNavMenuItemHeaderTheme.axaml` | NavMenu | `Background`, `CornerRadius`, `Header`, `HeaderTemplate`, `Height`, `Icon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (Border) | `VerticalNavMenuItemHeaderTheme.axaml` | VerticalNavMenuItemHeader | `Background`, `CornerRadius`, `Header`, `HeaderTemplate`, `Height`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderLayout` | template node (Grid) | `VerticalNavMenuItemHeaderTheme.axaml` | VerticalNavMenuItemHeader | `Header`, `HeaderTemplate`, `Icon`, `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemIconPresenter` | template node (IconPresenter) | `VerticalNavMenuItemHeaderTheme.axaml` | VerticalNavMenuItemHeader | `Icon`, `IsEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ItemTextPresenter` | template node (ContentPresenter) | `VerticalNavMenuItemHeaderTheme.axaml` | VerticalNavMenuItemHeader | `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `CollapsedTitlePresenter` | template node (ContentPresenter) | `VerticalNavMenuItemHeaderTheme.axaml` | VerticalNavMenuItemHeader | `Header` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `MenuIndicatorIcon` | template node (RightOutlined) | `VerticalNavMenuItemHeaderTheme.axaml` | VerticalNavMenuItemHeader | `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ItemsPresenter` | `ItemsPresenter` | 承载顶层 `NavMenuItem` 容器。 |
| `PART_HorizontalLine` | `Rectangle` | Horizontal light style 下的底部分割线。 |
| `PART_Header` | NavMenuItemHeader | 菜单项 header，承载文字、图标、箭头和交互视觉。 |
| `PART_Popup` | `Popup` | `Vertical` / `Horizontal` 模式下的子菜单浮层。 |
| `PART_PopupFrame` | `Border` | Popup 背景、圆角、尺寸和内容边距。 |
| `PART_ChildItemsLayoutTransform` | `LayoutAwareMotionActor` | `Inline` 模式下的子菜单展开收起 motion 容器。 |
| `PART_ChildItemsFrame` | `Border` | `Inline` 模式下的子菜单背景块。 |
| `ChildItemsPresenter` | `ItemsPresenter` | `Inline` 模式下的子菜单内容承载。 |
| `PART_ActiveIndicator` | `Rectangle` | `Horizontal` 顶层 light style 下的活动指示条。 |

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

NavMenu 的交互行为由 mode 决定。

`Inline` 模式：

- 点击带子菜单的项目时切换 `IsSubMenuOpen`。
- 子菜单在当前视觉树中展开，使用 `LayoutAwareMotionActor` 承载展开收起 motion。
- 点击叶子节点时选中该节点，并更新所有祖先 `IsInSelectedPath`。
- `IsAccordionMode=true` 时，顶层子菜单互斥展开。
- `IsInlineCollapsed=true` 时，public `Mode` 仍保持 `Inline`，但内部有效模式切换为 vertical popup 语义：顶层只显示图标或无图标首字符，inline 子树不在主视觉树中展开，带子菜单的顶层项目通过 popup 打开。
- 进入折叠时缓存当前 inline 打开路径并关闭主视觉树中的 inline 子菜单；退出折叠时恢复缓存路径。折叠和展开不得清空 `SelectedItem` 或 selected path。
- 键盘 Up / Down 在当前可见层级内移动 active/focus 项。
- Enter 在带子菜单项上切换展开状态，在叶子节点上提交选择。
- Left / Right 可作为桌面增强支持折叠或展开当前 active 子菜单，并保持 keyboard active 在当前项；叶子项上为 no-op，不能改变 `SelectedItem`。

`Vertical` 与 `Horizontal` 模式：

- 带子菜单的项目通过 Popup 展开。
- hover 可以延迟打开子菜单；pointer 离开后延迟关闭。
- 点击叶子节点时选中节点；弹出层关闭由 pointer、窗口失焦、非客户端点击和同级打开状态共同控制。
- `Horizontal` 顶层菜单 popup 位于下方；非顶层 popup 按右侧边缘对齐。
- 键盘导航以当前打开的可见菜单层级为边界移动 active/focus 项，跳过禁用项、分割线和不可聚焦内容。
- 键盘 active 初次移动时优先以当前可见且已生成的 `SelectedItem` 容器作为方向键锚点，并立即移动到前一个或后一个可导航节点；如果没有选中项，或选中项隐藏在未打开的子菜单中，则从第一个可导航节点开始。
- `Horizontal` 顶层菜单使用 Left / Right 在顶层兄弟项之间移动，Down 或 Enter 打开当前 active 子菜单并进入子菜单第一项。
- `Vertical` 根层和所有 popup 子菜单使用 Up / Down 在同层兄弟项之间移动，Right 或 Enter 打开当前 active 子菜单并进入子菜单第一项，Left 或 Esc 返回父级并关闭当前 popup 分支。
- Enter 在叶子节点上提交选择并触发 `NavMenuNodeSelected` / `NavMenuItemClick`；方向键只改变 active/focus，不触发选择。
- Esc 只关闭当前键盘导航所在的 popup 分支并返回父级 active 项，不调用 `Close()`，因此不能清空 `SelectedItem`。

公共交互状态：

- `Disabled` 由节点 `IsEnabled` 和 command can-execute 共同决定，禁用项不应触发有效点击。
- 节点命令必须复用 `NavMenuItem` 的有效点击入口；pointer 与 keyboard 提交不能形成两条独立命令执行路径，也不能因选择事件再次执行命令。
- `PointerOver` 改变 header 前景和背景，但不能改变选中路径。
- `Pressed` 只作为点击过程状态，不应通过 ancestor selector 误作用到 header。
- `KeyboardActive` 表示键盘漫游中的当前项，只影响 focus 和 active 视觉，不改变选中路径。
- `Selected` 表示当前叶子节点被选中。
- `IsInSelectedPath` 表示某个祖先位于当前选中路径中。
- `Open` 表示当前项目子菜单打开。

## Theme and Token Boundaries

NavMenu Theme 按 mode、dark style、header state 和 item background model 分层。

```text
NavMenuTheme
  root template by Mode
  root background / padding / scroll behavior
  top-level ItemsPanel orientation

NavMenuItemTheme
  item template by Mode
  popup frame
  inline child frame
  motion duration

Header Themes
  shared text/icon/background/selection
  horizontal active bar
  inline indentation and arrow rotation
```

Theme 映射规则：

- Root 背景使用 `ItemBg`，Dark root 背景使用 `DarkMenuBg`。
- Popup 背景使用 `MenuPopupBg`，Dark popup 使用 `DarkMenuPopupBg`。
- Header 默认背景为 `Transparent`，hover / selected 背景由 header state 直接控制。
- Keyboard active 背景使用 `ItemActiveBg`，其优先级低于 `Selected`，高于普通默认态；它可以叠加在 `IsInSelectedPath` 父节点上，使父节点保留 selected-path 文字色的同时显示临时 active 背景。dark style 下使用 dark 语义的 active 视觉，不复用 selected 背景表达临时漫游。
- Inline collapsed 根宽度使用 `InlineCollapsedWidth`，默认来自 `NavMenuToken.InlineCollapsedWidth=48`。折叠视觉只作用于 `Mode=Inline && IsInlineCollapsed=true`：一级 icon 使用 `CollapsedIconSize` 居中，标题和箭头收起，未配置 icon 的一级项显示标题首字符，叶子项可用 tooltip 展示完整标题。
- `IsItemBackgroundEnabled=true` 时，inline child frame 使用 `SubMenuItemBg` / `DarkSubMenuItemBg`，并应用背景块专用外距。
- `IsItemBackgroundEnabled=false` 时，inline child frame 背景为 `Transparent`，不应用背景块专用外距；header 的文字色、hover、selected 和 selected path 仍然生效。
- Horizontal 顶层 light style 通过 `PART_ActiveIndicator` 表达选中；dark style 可以使用 selected background。

Header 背景与 NavMenuItem / inline submenu 背景块是不同职责，不应混为一个 selector 控制。

Token 边界：

NavMenuToken 是 NavMenu 的组件级设计变量层。它把全局颜色、尺寸、间距、圆角、字体和 popup 体系转换为 NavMenu 可消费的语义值。

NavMenuToken 服务以下主题：

- `NavMenuTheme.axaml`
- `NavMenuItemTheme.axaml`
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
- 进入或退出 inline collapsed 不得调用 `Close()`，不得清空 `SelectedItem`，不得丢失 selected path。
- inline collapsed 期间打开的 popup 状态不得污染展开后恢复的 inline open path cache。
- 键盘 active/focus 状态不得进入公共 API，不得改变 `SelectedItem`、`DefaultSelectedPath` 或 `DefaultOpenPaths` 的语义。
- `NavMenuNode` / `INavMenuNode` 的 `Header`、`HeaderTemplate`、`ItemKey`、`Icon`、`IsEnabled`、`Command`、`CommandParameter`、`Children` 名称、类型和语义不变。
- `NavMenuNode` 只承载命令配置，不实现 `ICommandSource`，不直接订阅 `CanExecuteChanged`，也不保存当前 `NavMenuItem` 容器。
- `CommandParameter` 保持标准显式参数语义，不隐式回退到 `ItemKey`、`Header`、`SelectedItem` 或节点自身。
- `NavMenuItemClick` 和 `NavMenuNodeSelected` 的事件语义不变。
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

- mode 切换时重新挂接 handler，并清理旧模式打开状态。
- `IsInlineCollapsed` 切换不得改写 public `Mode`，不得调用 `Close()`，不得清空 `SelectedItem`。
- inline collapsed 进入时缓存 inline open path，退出时恢复 cache；折叠期间 popup 打开状态不得污染 cache。
- `InlineCollapsedWidth` 默认来自 `NavMenuToken.InlineCollapsedWidth`，本地属性值必须按 Avalonia 优先级覆盖 token 默认值。
- 折叠视觉不能通过改写 `Header`、删除 `HeaderTemplate` 或动态创建替代 header 实现。
- 点击 item 不得临时关闭 motion。
- 默认路径应用不使用固定 50ms sleep 作为稳定策略。
- selection coordinator 是选择状态的统一入口。
- keyboard active/focus 状态不能替代 selection coordinator。
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
