# NavMenu 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.NavMenu` 桌面版的最新设计定位、公共契约、导航状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [NavMenu 桌面版实现原理](implementation.md)，NavMenu Token 的专项设计见 [NavMenu Token 设计](token.md)，设计和契约变化记录见 [NavMenu Changelog](changelog.md)。

## 1. 控件定位

NavMenu 是 AtomUI 桌面导航体系中的层级菜单导航控件，用于表达应用页面、模块、功能入口或命令集合之间的层级关系。它以树形节点为数据模型，以 `Inline`、`Vertical`、`Horizontal` 三种模式映射到不同导航场景。

NavMenu 的职责是管理导航节点容器生成、层级展开、选中路径、弹出式子菜单、主题视觉和菜单交互。它不负责路由切换、页面生命周期、权限过滤、数据懒加载、业务命令编排或页面内容渲染。业务导航行为应通过 `NavMenuNodeSelected`、`NavMenuItemClick` 或外部 ViewModel 处理。

NavMenu 支持两种节点提供方式：

- 直接在 `NavMenu.Items` 中放置 `NavMenuNode`。
- 通过 `ItemsSource` 绑定 `INavMenuNode` 集合。

两种方式都必须使用 `INavMenuNode` 作为节点契约。NavMenu 不支持把任意 `Control` 作为菜单项直接加入。

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

## 3. API 与契约模型

NavMenu 的公共 API 分为控件 API、节点 API 和事件 API。

控件 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Mode` | `NavMenuMode` | 菜单呈现模式，默认 `Inline`。 |
| `SelectedItem` | `INavMenuNode?` | 当前选中节点，双向绑定入口。 |
| `DefaultSelectedPath` | `TreeNodePath?` | 初始选中路径；`SelectedItem` 非空时优先级更高。 |
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

`DefaultSelectedPath` 和 `DefaultOpenPaths` 是默认值入口，不是持续受控展开状态。运行期受控选择应使用 `SelectedItem`。

稳定 template part：

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

## 4. 行为与状态模型

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

## 5. 视觉与主题模型

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
- `IsItemBackgroundEnabled=true` 时，inline child frame 使用 `SubMenuItemBg` / `DarkSubMenuItemBg`，并应用背景块专用外距。
- `IsItemBackgroundEnabled=false` 时，inline child frame 背景为 `Transparent`，不应用背景块专用外距；header 的文字色、hover、selected 和 selected path 仍然生效。
- Horizontal 顶层 light style 通过 `PART_ActiveIndicator` 表达选中；dark style 可以使用 selected background。

Header 背景与 NavMenuItem / inline submenu 背景块是不同职责，不应混为一个 selector 控制。

## 6. 控件家族或集成关系

NavMenu 位于 Desktop Navigation 分类，与 Breadcrumb、Pagination、Steps、TabControl 等控件同属导航体系，但不共享状态模型。

集成关系：

- `ItemsControl`：承载 `Items`、`ItemsSource`、`ItemTemplate` 和容器生成。
- AtomUI Token：通过 `NavMenuToken.ScopeProvider` 注册控件级资源。
- Popup/Overlay：`Vertical` 和 `Horizontal` 子菜单通过 `Popup` 展开，并由 `ShouldUseOverlayPopup` 控制 overlay 使用策略。
- MotionScene：`Inline` 子菜单展开收起使用 slide motion。
- Resource Host：`NavMenuNode` 可挂接 owner menu 的资源宿主，使节点 header 和模板动态资源跟随菜单资源域。
- Gallery：展示 inline、vertical、horizontal、dark、items source、默认选中路径和默认展开路径。

NavMenu 不实现 Form、CompactSpace 或 Button 家族接口。

## 7. 兼容性不变量

维护 NavMenu 时必须保持以下不变量：

- `NavMenuMode.Vertical`、`Horizontal`、`Inline` 的名称、默认行为和模板模式不变。
- `Mode` 默认值保持 `Inline`。
- `SelectedItem` 优先级高于 `DefaultSelectedPath`。
- `DefaultOpenPaths` 和 `DefaultSelectedPath` 不依赖固定时间延迟。
- `NavMenuNode` 的 `Header`、`HeaderTemplate`、`ItemKey`、`Icon`、`IsEnabled`、`Children` 名称、类型和语义不变。
- `NavMenuItemClick` 和 `NavMenuNodeSelected` 的事件语义不变。
- `IsAccordionMode=true` 只控制同层展开互斥，不改变选中节点。
- `IsItemBackgroundEnabled=false` 不应关闭 header 前景色、hover、selected、selected path 或 disabled 视觉，只关闭 item / submenu 背景块。
- inline 子菜单背景块外距只在 `IsItemBackgroundEnabled=true` 时生效。
- popup frame 使用 `MenuPopupBg` / `DarkMenuPopupBg`，不回退为普通 shared elevated background。
- root background、popup background、header background 和 inline submenu background 必须保持职责分离。
- 点击子节点时，不应让父级 header 出现错误 hover 背景。
- NavMenu 优化不得关闭 motion 来规避点击、打开或关闭问题。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

## 8. 专项模型

### 8.1 Mode 模型

`Mode` 同时影响模板结构、交互 handler、popup 策略、ItemsPanel 方向和 header 主题。

| Mode | 子菜单承载 | 顶层排列 |
| --- | --- | --- |
| `Inline` | 视觉树内 `LayoutAwareMotionActor` | Vertical StackPanel |
| `Vertical` | Popup | Vertical StackPanel |
| `Horizontal` | Popup | Horizontal StackPanel |

### 8.2 Selection 与 Path 模型

`TreeNodePath` 通过 `ItemKey` 定位节点路径。路径 replay 先打开中间节点，再选中叶子节点。`SelectedItem` 是持续选择状态，`DefaultSelectedPath` 是默认选择入口。两者同时存在时，`SelectedItem` 生效。

### 8.3 Item Background 模型

`IsItemBackgroundEnabled` 控制背景块，不控制 header 文本状态。该模型要求 `NavMenuItem` 背景和 `NavMenuItemHeader` 背景分离，不能通过禁用 header selector 来实现无背景模式。

### 8.4 Popup 模型

`Vertical` 和 `Horizontal` 子菜单使用同一 popup shell。顶层 horizontal popup 放置在底部，非顶层和 vertical popup 使用右侧对齐。Popup 内容宽度、最大高度、背景、圆角和内边距由 NavMenuToken 和 PopupHostToken 共同决定。

## 9. 文档导航与验证策略

关联文档：

- [NavMenu 桌面版实现原理](implementation.md)
- [NavMenu Token 设计](token.md)
- [NavMenu Changelog](changelog.md)

验证策略：

| 层次 | 验证内容 |
| --- | --- |
| 文档 | `overview.md`、`implementation.md`、`token.md`、`changelog.md` 链接有效。 |
| Public API | `NavMenu`、`NavMenuNode`、`INavMenuNode`、`INavMenu`、事件参数与文档一致。 |
| Mode 行为 | Inline、Vertical、Horizontal 的打开、关闭、选中、默认路径和 popup 逻辑稳定。 |
| Selection | `SelectedItem`、`DefaultSelectedPath`、`DefaultOpenPaths`、stale replay 和 clear selection 测试覆盖。 |
| AXAML | Template part 名称、header theme、popup frame、inline child frame、active indicator 和 item background selector 稳定。 |
| Layout | root item margin、inline child gap、popup item inset、background-enabled true/false gap 与 Ant Design 规则一致。 |
| Token | `NavMenuToken` 默认值、dark token、popup token 和 spacing token 与测试一致。 |
| Gallery | 运行 Navigation/Menu Showcase 相关测试，确认示例结构和 CaseNavigation 布局稳定。 |
