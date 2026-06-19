# NavMenu 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.NavMenu` 桌面版的最新架构设计、设计语言、交互模型、API 模型和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，NavMenu Token 的专项设计见 [NavMenu Token 设计](token.md)，设计和契约变化记录见 [NavMenu Changelog](changelog.md)。

## 1. 控件定位

NavMenu 是 AtomUI 桌面导航体系中的层级菜单导航控件，用于表达应用页面、模块、功能入口或命令集合之间的层级关系。它以树形节点为数据模型，以 `Inline`、`Vertical`、`Horizontal` 三种模式映射到不同导航场景。

NavMenu 的职责是管理导航节点容器生成、层级展开、选中路径、弹出式子菜单、主题视觉和菜单交互。它不负责路由切换、页面生命周期、权限过滤、数据懒加载、业务命令编排或页面内容渲染。业务导航行为应通过 `NavMenuNodeSelected`、`NavMenuItemClick` 或外部 ViewModel 处理。

NavMenu 支持两种节点提供方式：

- 直接在 `NavMenu.Items` 中放置 `NavMenuNode`。
- 通过 `ItemsSource` 绑定 `INavMenuNode` 集合。

两种方式都必须使用 `INavMenuNode` 作为节点契约。`NavMenu` 不支持把任意 `Control` 作为菜单项直接加入，因为控件内部需要稳定生成 `NavMenuItem` 容器以维护层级、选择、展开和主题状态。

## 2. 设计语言

NavMenu 表达的是“层级入口 + 当前路径”的导航语义。用户应能通过文字、图标、缩进、选中态、悬浮态和展开状态理解当前位置、可进入的下级路径以及不同层级之间的关系。

三种模式对应不同产品语义：

| 模式 | 语义 | 典型场景 |
| --- | --- | --- |
| `Inline` | 子菜单在当前导航面板内展开，强调完整层级结构。 | 侧边栏主导航、管理后台模块导航。 |
| `Vertical` | 顶层项目垂直排列，子菜单通过 Popup 展开。 | 紧凑菜单、上下文导航、浮层式侧栏。 |
| `Horizontal` | 顶层项目水平排列，子菜单通过 Popup 展开。 | 顶部导航栏、一级模块切换。 |

Light 与 Dark 样式不是简单反色。Dark 样式有独立的根背景、Popup 背景、子菜单背景、选中背景和文字透明度规则，应保持与 Ant Design Menu 的层级背景语义一致。

`IsItemBackgroundEnabled` 控制菜单项背景块模型。开启时，inline 子菜单背景块、选中背景和 hover 背景形成连续层级背景；关闭时，菜单项背景块保持透明，但 header 文本颜色、选中路径颜色和交互状态仍然保留。

## 3. 架构分层

NavMenu 架构按导航状态、容器生成、交互处理和主题视觉分层。

| 层 | 责任 | 主要载体 |
| --- | --- | --- |
| Public API | 暴露节点数据、模式、默认展开路径、选中节点、主题风格、背景策略、motion 和事件。 | `NavMenu`、`NavMenuNode`、`INavMenuNode` |
| Node Model | 表达 Header、Icon、ItemKey、Enabled、Children 和资源宿主。 | `NavMenuNode` |
| Container Layer | 将节点生成 `NavMenuItem`，转发 mode、theme、background、motion 和 popup 状态。 | `NavMenu`、`NavMenuItem`、`NavMenuItemContainerBinder` |
| Interaction | 按模式处理 pointer、click、hover、popup close、selection 和 accordion。 | `NavMenuInteractionHandlerBase`、`DefaultNavMenuInteractionHandler`、`InlineNavMenuInteractionHandler` |
| Selection State | 维护当前选中节点、选中容器和祖先路径状态。 | `NavMenuSelectionCoordinator`、`IsInSelectedPath` |
| Template Contract | 定义 root presenter、header、popup、inline child frame 和 active indicator。 | NavMenu 主题和 Header 主题 |
| Theme Mapping | 将 mode、dark、hover、selected、open、disabled、background-enabled 映射为视觉属性。 | `NavMenuTheme.axaml`、`NavMenuItemTheme.axaml`、Header Themes |
| Component Token | 将全局设计体系转换为 NavMenu 尺寸、颜色、间距、Popup 和 dark style 语义值。 | `NavMenuToken` |

状态流：

```text
Public API / Node Model
  Mode / IsDarkStyle / IsItemBackgroundEnabled
  SelectedItem / DefaultSelectedPath / DefaultOpenPaths
  NavMenuNode.Header / Icon / ItemKey / Children / IsEnabled
        ↓
Container Layer
  NavMenuItem.Mode / IsDarkStyle / IsItemBackgroundEnabled
  HasSubMenu / Level / IsTopLevel / IsSubMenuOpen
        ↓
Interaction + Selection
  Default handler or Inline handler
  Selected container / selected ancestor path
        ↓
Template + Theme
  Header theme
  Popup frame
  Inline child frame
  active indicator
```

## 4. API 设计

NavMenu 的公共 API 分为控件 API、节点 API 和事件 API。

控件 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Mode` | `NavMenuMode` | 菜单呈现模式，默认 `Inline`。 |
| `SelectedItem` | `INavMenuNode?` | 当前选中节点，双向绑定入口。 |
| `DefaultSelectedPath` | `TreeNodePath?` | 初始选中路径。`SelectedItem` 非空时优先级高于该属性。 |
| `DefaultOpenPaths` | `IList<TreeNodePath>?` | 初始展开路径集合。 |
| `IsAccordionMode` | `bool` | 顶层 inline/vertical 子菜单互斥展开策略。 |
| `IsDarkStyle` | `bool` | 使用 NavMenu dark style 视觉体系。 |
| `IsItemBackgroundEnabled` | `bool` | 控制 item / inline submenu 背景块模型，默认 `true`。 |
| `IsMotionEnabled` | `bool` | 控制 inline 展开收起和主题颜色过渡。 |
| `ShouldUseOverlayPopup` | `bool` | 控制弹出式子菜单是否使用 overlay popup。 |
| `Close()` | method | 关闭所有子菜单并清空 `SelectedItem`。 |

节点 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Header` | `object?` | 菜单项显示内容。 |
| `HeaderTemplate` | `IDataTemplate?` | 菜单项 header 模板。 |
| `ItemKey` | `EntityKey?` | 路径和业务标识。 |
| `Icon` | `PathIcon?` | 菜单项图标。 |
| `IsEnabled` | `bool` | 节点可用状态，默认 `true`。 |
| `Children` | `IList<INavMenuNode>` | 子节点集合。 |

事件 API：

| 事件 | 参数 | 语义 |
| --- | --- | --- |
| `NavMenuItemClick` | `NavMenuItemClickEventArgs` | 菜单项点击事件，事件参数暴露 `INavMenuItem` 作为只读交互上下文。 |
| `NavMenuNodeSelected` | `NavMenuNodeSelectedEventArgs` | 叶子节点选中事件，事件参数暴露 `INavMenuNode`。 |

`DefaultSelectedPath` 和 `DefaultOpenPaths` 是默认值入口，不是持续受控展开状态。控件加载后通过 bounded replay 在容器可用时应用路径，避免固定延时。运行期受控选择应使用 `SelectedItem`。

`SelectedItem` 是受控选择入口。程序连续设置多个节点时，过期的异步 replay 必须被忽略，最终只应用最新 revision 对应的节点。

`NavMenuItem` 是内部容器类型。它承载命令、热键、popup、selection 和 header 转发能力，但不作为外部直接实例化的公共控件契约使用。

## 5. 行为交互模型

NavMenu 的交互行为由 mode 决定。

`Inline` 模式：

- 点击带子菜单的项目时切换 `IsSubMenuOpen`。
- 子菜单在当前视觉树中展开，使用 `LayoutAwareMotionActor` 承载展开收起 motion。
- 点击叶子节点时选中该节点，并更新所有祖先 `IsInSelectedPath`。
- `IsAccordionMode=true` 时，顶层子菜单互斥展开。

`Vertical` 与 `Horizontal` 模式：

- 带子菜单的项目通过 Popup 展开。
- hover 可以延迟打开子菜单；pointer 离开后延迟关闭。
- 点击叶子节点时选中节点；弹出层关闭由 pointer、窗口失焦、非客户端点击和同级打开状态共同控制。
- `Horizontal` 顶层菜单 popup 位于下方；非顶层 popup 按右侧边缘对齐。

公共交互状态：

- `Disabled` 由节点 `IsEnabled` 和 command can-execute 共同决定，禁用项不应触发有效点击。
- `PointerOver` 改变 header 前景和背景，但不能改变选中路径。
- `Pressed` 只作为点击过程状态，不应通过 ancestor selector 误作用到 header。
- `Selected` 表示当前叶子节点被选中。
- `IsInSelectedPath` 表示某个祖先位于当前选中路径中。
- `Open` 表示当前项目子菜单打开。

NavMenu 不负责页面跳转。使用方应在 `NavMenuNodeSelected` 中执行路由或 ViewModel 状态切换。

## 6. 状态模型

NavMenu 的有效状态由节点树、容器层级、mode、选择状态和主题状态共同组成。

节点状态：

```text
INavMenuNode
  Header / HeaderTemplate / Icon / ItemKey / IsEnabled / Children
        ↓
NavMenuItem
  Header / HeaderTemplate / Icon / ItemKey / IsEnabled / HasSubMenu
```

层级状态：

- `Level` 从 logical parent 距离计算，用于 inline 缩进。
- `IsTopLevel` 由 parent 是否为 `NavMenu` 决定，用于 horizontal 顶层视觉和 hit test。
- `HasSubMenu` 由 `ItemCount > 0` 决定。

选择状态：

```text
Select leaf item
        ↓
NavMenuSelectionCoordinator
        ↓
old selected container IsSelected=false
old ancestor IsInSelectedPath=false
new selected container IsSelected=true
new ancestors IsInSelectedPath=true
        ↓
NavMenu.SelectedItem + NavMenuNodeSelected
```

展开状态：

- `DefaultOpenPaths` 和 `DefaultSelectedPath` 使用路径遍历打开中间节点。
- `Inline` 模式通过 `IsSubMenuOpen` 控制 child items actor 可见性和 motion。
- `Vertical` / `Horizontal` 模式通过 `Popup.IsOpen` 绑定 `IsSubMenuOpen && HasSubMenu`。

主题状态：

- `Mode` 同步到 root pseudo-class：`:inline-mode`、`:vertical-mode`、`:horizontal-mode`。
- `IsDarkStyle` 同步到 `:dark` / `:light`。
- `Icon` 同步到 item `:icon`。
- `IsTopLevel` 同步到 item `:toplevel`。

## 7. 模板与视觉架构

NavMenu root 模板按 mode 选择不同结构。

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ItemsPresenter` | `ItemsPresenter` | 承载顶层 `NavMenuItem` 容器。 |
| `PART_HorizontalLine` | `Rectangle` | Horizontal light style 下的底部分割线。 |

NavMenuItem 模板按 mode 选择不同结构。

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Header` | `HorizontalNavMenuItemHeader` / `VerticalNavMenuItemHeader` / `InlineNavMenuItemHeader` | 菜单项 header，承载文字、图标、箭头、hover、selected、disabled 等视觉状态。 |
| `PART_Popup` | `Popup` | `Vertical` / `Horizontal` 模式下的子菜单浮层。 |
| `PART_PopupFrame` | `Border` | Popup 背景、圆角、尺寸和内容边距。 |
| `PART_ChildItemsLayoutTransform` | `LayoutAwareMotionActor` | `Inline` 模式下的子菜单展开收起 motion 容器。 |
| `PART_ChildItemsFrame` | `Border` | `Inline` 模式下的子菜单背景块。 |
| `ChildItemsPresenter` | `ItemsPresenter` | `Inline` 模式下的子菜单内容承载。 |
| `PART_ActiveIndicator` | `Rectangle` | `Horizontal` 顶层 light style 下的活动指示条。 |

Header 视觉由三种 header 控件承载：

- `HorizontalNavMenuItemHeader`
- `VerticalNavMenuItemHeader`
- `InlineNavMenuItemHeader`

三者共享 `BaseNavMenuItemHeaderTheme` 的文字、图标、hover、selected、disabled 和 dark state 规则，再按 mode 扩展布局。Header 背景与 NavMenuItem / inline submenu 背景块是不同职责，不应混为一个 selector 控制。

AXAML 层级优化必须保留以下边界：

- root `ScrollViewer` 负责 inline/vertical 菜单滚动，不应移动到 C# 动态创建。
- popup shell 保持在模板内，popup content 由 `ItemsPresenter` 承载。
- inline motion actor 负责动画隔离，不能与 child frame 合并。
- child frame 负责 inline 子菜单背景块，不能由 header margin 代替。
- horizontal active indicator 负责顶层选中线，不应并入 header 文本层。

## 8. Theme 架构

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
- Header 默认背景为 `Transparent`，hover/selected 背景由 header state 直接控制。
- `IsItemBackgroundEnabled=true` 时，inline child frame 使用 `SubMenuItemBg` / `DarkSubMenuItemBg`，并应用 `VerticalChildItemsMargin` 保留背景块与下一个根项之间的视觉间距。
- `IsItemBackgroundEnabled=false` 时，inline child frame 背景为 `Transparent`，不应用背景块专用外距；header 的文字色、hover、selected 和 selected path 仍然生效。
- Horizontal 顶层 light style 通过 `PART_ActiveIndicator` 表达选中；dark style 可以使用 selected background。

Theme 不承担节点路径解析、选择优先级、accordion 或 popup close 策略，这些属于 C# interaction 层职责。

## 9. 控件家族或集成关系

NavMenu 位于 Desktop Navigation 分类，与 Breadcrumb、Pagination、Steps、TabControl 等控件同属导航体系，但不共享状态模型。

NavMenu 与以下系统集成：

- `ItemsControl`：承载 `Items`、`ItemsSource`、`ItemTemplate` 和容器生成。
- AtomUI Token：通过 `NavMenuToken.ScopeProvider` 注册控件级资源。
- Popup/Overlay：`Vertical` 和 `Horizontal` 子菜单通过 `Popup` 展开，并由 `ShouldUseOverlayPopup` 控制 overlay 使用策略。
- MotionScene：`Inline` 子菜单展开收起使用 slide motion。
- Resource Host：`NavMenuNode` 可挂接 owner menu 的资源宿主，使节点 header 和模板动态资源跟随菜单资源域。
- Gallery：Navigation/Menu Showcase 展示 inline、vertical、horizontal、dark、items source、默认选中路径和默认展开路径。

NavMenu 不实现 Form、CompactSpace 或 Button 家族接口。

## 10. 兼容性不变量

维护 NavMenu 时必须保持以下不变量：

- `NavMenuMode.Vertical`、`Horizontal`、`Inline` 的名称、默认行为和模板模式不变。
- `Mode` 默认值保持 `Inline`。
- `SelectedItem` 优先级高于 `DefaultSelectedPath`。
- `DefaultOpenPaths` 和 `DefaultSelectedPath` 不依赖固定时间延迟；路径 replay 必须有有界重试。
- `NavMenuNode` 的 `Header`、`HeaderTemplate`、`ItemKey`、`Icon`、`IsEnabled`、`Children` 名称、类型和语义不变。
- `NavMenuItemClick` 和 `NavMenuNodeSelected` 的事件语义不变。
- `IsAccordionMode=true` 只控制同层展开互斥，不改变选中节点。
- `IsItemBackgroundEnabled=false` 不应关闭 header 前景色、hover、selected、selected path 或 disabled 视觉，只关闭 item/submenu 背景块。
- inline 子菜单背景块外距只在 `IsItemBackgroundEnabled=true` 时生效。
- popup frame 使用 `MenuPopupBg` / `DarkMenuPopupBg`，不回退为普通 shared elevated background。
- root background、popup background、header background 和 inline submenu background 必须保持职责分离。
- `PART_Header`、`PART_Popup`、`PART_PopupFrame`、`PART_ChildItemsLayoutTransform`、`PART_ChildItemsFrame`、`ChildItemsPresenter`、`PART_ItemsPresenter`、`PART_HorizontalLine`、`PART_ActiveIndicator` 名称不擅自修改。
- 点击子节点时，不应让父级 header 出现错误 hover 背景。
- NavMenu 优化不得关闭 motion 来规避点击、打开或关闭问题。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

## 11. 专项模型

### 11.1 Mode 模型

`Mode` 同时影响模板结构、交互 handler、popup 策略、ItemsPanel 方向和 header 主题。

| Mode | Handler | 子菜单承载 | 顶层排列 |
| --- | --- | --- | --- |
| `Inline` | `InlineNavMenuInteractionHandler` | 视觉树内 `LayoutAwareMotionActor` | Vertical StackPanel |
| `Vertical` | `DefaultNavMenuInteractionHandler` | Popup | Vertical StackPanel |
| `Horizontal` | `DefaultNavMenuInteractionHandler` | Popup | Horizontal StackPanel |

切换 mode 时必须关闭已打开子菜单并重新挂接 interaction handler，避免旧模式的 pointer、delay、popup 或 motion 状态泄漏到新模式。

### 11.2 Selection 与 Path 模型

`TreeNodePath` 通过 `ItemKey` 定位节点路径。路径 replay 先打开中间节点，再选中叶子节点。由于容器生成依赖 Avalonia layout 和 ItemsPresenter，replay 允许在 `DispatcherPriority.Loaded` 下有界重试。

`SelectedItem` 是持续选择状态，`DefaultSelectedPath` 是默认选择入口。两者同时存在时，`SelectedItem` 生效。

选中叶子节点时，NavMenu 必须同步：

- 叶子容器 `IsSelected=true`。
- 祖先容器 `IsInSelectedPath=true`。
- 旧叶子容器和旧祖先路径清理。
- `SelectedItem` 更新为对应 `INavMenuNode`。
- 派发 `NavMenuNodeSelected`。

### 11.3 Item Background 模型

`IsItemBackgroundEnabled` 控制背景块，不控制 header 文本状态。

开启时：

- inline child frame 使用子菜单背景色。
- 子菜单背景块与下一个根项之间保留 `VerticalChildItemsMargin`。
- selected / hover header 背景保持可见。

关闭时：

- inline child frame 背景透明。
- 不应用背景块专用外距。
- header 前景、selected、selected path 和 disabled 状态仍由 header theme 处理。

该模型要求 `NavMenuItem` 背景和 `NavMenuItemHeader` 背景分离。不能通过禁用 header selector 来实现无背景模式。

### 11.4 Popup 模型

`Vertical` 和 `Horizontal` 子菜单使用同一 popup shell。顶层 horizontal popup 放置在底部，非顶层和 vertical popup 使用右侧对齐。Popup 内容宽度、最大高度、背景、圆角和内边距由 NavMenuToken 和 PopupHostToken 共同决定。

Popup 打开时必须保证 items presenter 可应用模板，以便容器生成和选中 replay 正常完成。默认路径应用不能通过固定 sleep 或 timer 等待 popup 容器。

## 12. 验证策略

NavMenu 变更按以下层次验证：

| 层次 | 验证内容 |
| --- | --- |
| 文档 | `overview.md`、`token.md`、`changelog.md` 链接有效；Navigation 分类入口包含 NavMenu 链接。 |
| Public API | `NavMenu`、`NavMenuNode`、`INavMenuNode`、`INavMenu`、事件参数与文档一致。 |
| Mode 行为 | Inline、Vertical、Horizontal 的打开、关闭、选中、默认路径和 popup 逻辑稳定。 |
| Selection | `SelectedItem`、`DefaultSelectedPath`、`DefaultOpenPaths`、stale replay 和 clear selection 测试覆盖。 |
| AXAML | Template part 名称、header theme、popup frame、inline child frame、active indicator 和 item background selector 稳定。 |
| Layout | root item margin、inline child gap、popup item inset、background-enabled true/false gap 与 Ant Design 规则一致。 |
| Token | `NavMenuToken` 默认值、dark token、popup token 和 spacing token 与 `NavMenuTokenTests` 一致。 |
| Gallery | 运行 Navigation/Menu Showcase 相关测试，确认示例结构和 CaseNavigation 布局稳定。 |
