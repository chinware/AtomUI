# NavMenu

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

NavMenu 是 AtomUI 桌面导航体系中的层级菜单导航控件，用于表达应用页面、模块、功能入口或命令集合之间的层级关系。它以树形节点为数据模型，以 `Inline`、`Vertical`、`Horizontal` 三种模式映射到不同导航场景。

NavMenu 的职责是管理导航节点容器生成、层级展开、选中路径、弹出式子菜单、主题视觉和菜单交互。它不负责路由切换、页面生命周期、权限过滤、数据懒加载、业务命令编排或页面内容渲染。业务导航行为可以通过节点 `Command`、`NavMenuNodeSelected`、`NavMenuItemClick` 或外部 ViewModel 处理。

NavMenu 支持两种节点提供方式：

- 直接在 `NavMenu.Items` 中放置 `NavMenuNode`。
- 通过 `ItemsSource` 绑定 `INavMenuNode` 集合。

两种方式都必须使用 `INavMenuNode` 作为节点契约。NavMenu 不支持把任意 `Control` 作为菜单项直接加入。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `未独立 Gallery 页面；以 `docs/controls/desktop/navigation/nav-menu` 源文档为准` |
| 状态 | Stable |

## 何时使用

NavMenu 表达的是“层级入口 + 当前路径”的导航语义。用户应能通过文字、图标、缩进、选中态、悬浮态和展开状态理解当前位置、可进入的下级路径以及不同层级之间的关系。

三种模式对应不同产品语义：

| 模式 | 语义 | 典型场景 |
| --- | --- | --- |
| `Inline` | 子菜单在当前导航面板内展开，强调完整层级结构。 | 侧边栏主导航、管理后台模块导航。 |
| `Vertical` | 顶层项目垂直排列，子菜单通过 Popup 展开。 | 紧凑菜单、上下文导航、浮层式侧栏。 |
| `Horizontal` | 顶层项目水平排列，子菜单通过 Popup 展开。 | 顶部导航栏、一级模块切换。 |

Light 与 Dark 样式不是简单反色。Dark 样式有独立的根背景、Popup 背景、子菜单背景、选中背景和文字透明度规则，应保持与 参考 Menu 的层级背景语义一致。

`IsItemBackgroundEnabled` 控制菜单项背景块模型。开启时，inline 子菜单背景块、选中背景和 hover 背景形成连续层级背景；关闭时，菜单项背景块保持透明，但 header 文本颜色、选中路径颜色和交互状态仍然保留。

## 公共 API

NavMenu 的公共 API 分为控件 API、节点 API 和事件 API。

控件 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Mode` | `NavMenuMode` | 菜单呈现模式，默认 `Inline`。 |
| `IsInlineCollapsed` | `bool` | `Mode=Inline` 时是否进入内联折叠状态，默认 `false`。折叠不改变 public `Mode`，只改变内部有效呈现和交互模式。 |
| `InlineCollapsedWidth` | `double` | 内联折叠状态下的菜单宽度。默认值来自 `NavMenuToken.InlineCollapsedWidth`，初始设计值为 `48`；开发者可通过本地值覆盖 token 默认宽度。 |
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
| `Command` | `ICommand?` | 节点被有效触发时由当前 `NavMenuItem` 容器执行的业务命令。节点只承载命令配置，不负责订阅或执行。 |
| `CommandParameter` | `object?` | 传递给 `Command` 的参数。默认 `null`，不隐式回退到 `ItemKey` 或节点自身。 |
| `Children` | `IList<INavMenuNode>` | 子节点集合。 |

事件 API：

| 事件 | 参数 | 语义 |
| --- | --- | --- |
| `NavMenuItemClick` | `NavMenuItemClickEventArgs` | 菜单项点击事件，事件参数暴露 `INavMenuItem` 作为只读交互上下文。 |
| `NavMenuNodeSelected` | `NavMenuNodeSelectedEventArgs` | 叶子节点选中事件，事件参数暴露 `INavMenuNode`。 |

`DefaultSelectedPath` 和 `DefaultOpenPaths` 是默认值入口，不是持续受控展开状态。运行期受控选择应使用 `SelectedItem`。

节点命令遵守 Avalonia 命令语义：`NavMenuNode` / `INavMenuNode` 只保存 `Command` 和 `CommandParameter`，实际执行、`CanExecute` 评估和 `CanExecuteChanged` 生命周期由生成出的 `NavMenuItem` 容器负责。`INavMenuNode` 为两个成员提供 `null` 默认实现，使既有自定义节点实现无需声明命令也能继续工作。`CommandParameter=null` 表示显式空参数，控件不能自动替换为 `ItemKey`；需要使用业务 key 时，应显式把 `ItemKey` 绑定或赋值给 `CommandParameter`。容器按 UI 周期合并连续的 `CanExecuteChanged` 通知：持续的 `false` 仍进入 disabled，同一同步执行周期内的 `false -> true` 瞬时变化只投影最终状态，避免多个共享命令节点触发无业务意义的禁用颜色闪动。

`IsInlineCollapsed` 只对 `Mode=Inline` 生效。`Mode=Vertical` 或 `Mode=Horizontal` 时设置该属性不应改变当前模式的 popup、布局或键盘语义。`InlineCollapsedWidth` 参与布局测量，默认通过 theme setter 取得 `NavMenuToken.InlineCollapsedWidth`；collapsed 状态下由控件内部对 `Width` / `MinWidth` 做有效值 coercion，本地设置的属性值应按 Avalonia 属性优先级覆盖 token 默认值，展开后原始 `Width` 或绑定必须恢复。

键盘漫游状态属于内部交互状态，不进入公共 API。NavMenu 保持与 参考 Menu 一致的分层：`SelectedItem` 表示已提交的导航选择，`DefaultOpenPaths` / `IsSubMenuOpen` 表示展开状态，键盘当前项只表示临时 active/focus 目标。业务代码不应通过公开属性控制键盘 active 项，也不应把 active 项误认为已选择节点。

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

## 事件与命令

NavMenu 的公共 API 分为控件 API、节点 API 和事件 API。
| `Command` | `ICommand?` | 节点被有效触发时由当前 `NavMenuItem` 容器执行的业务命令。节点只承载命令配置，不负责订阅或执行。 |
事件 API：
| 事件 | 参数 | 语义 |
| `NavMenuItemClick` | `NavMenuItemClickEventArgs` | 菜单项点击事件，事件参数暴露 `INavMenuItem` 作为只读交互上下文。 |
| `NavMenuNodeSelected` | `NavMenuNodeSelectedEventArgs` | 叶子节点选中事件，事件参数暴露 `INavMenuNode`。 |
节点命令遵守 Avalonia 命令语义：`NavMenuNode` / `INavMenuNode` 只保存 `Command` 和 `CommandParameter`，实际执行、`CanExecute` 评估和 `CanExecuteChanged` 生命周期由生成出的 `NavMenuItem` 容器负责。`INavMenuNode` 为两个成员提供 `null` 默认实现，使既有自定义节点实现无需声明命令也能继续工作。`CommandParameter=null` 表示显式空参数，控件不能自动替换为 `ItemKey`；需要使用业务 key 时，应显式把 `ItemKey` 绑定或赋值给 `CommandParameter`。容器按 UI 周期合并连续的 `CanExecuteChanged` 通知：持续的 `false` 仍进入 disabled，同一同步执行周期内的 `false -> true` 瞬时变化只投影最终状态，避免多个共享命令节点触发无业务意义的禁用颜色闪动。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

Gallery 目录 `未独立 Gallery 页面；以 `docs/controls/desktop/navigation/nav-menu` 源文档为准` 当前不存在；请检查控件文档中的 Gallery 页面元数据。

## 状态模型

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

## 主题与 Design Token

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

Token 来源：

NavMenuToken 是 NavMenu 的组件级设计变量层。它把全局颜色、尺寸、间距、圆角、字体和 popup 体系转换为 NavMenu 可消费的语义值。

NavMenuToken 服务以下主题：

- `NavMenuTheme.axaml`
- `NavMenuItemTheme.axaml`
- `BaseNavMenuItemHeaderTheme.axaml`
- `HorizontalNavMenuItemHeaderTheme.axaml`
- `VerticalNavMenuItemHeaderTheme.axaml`
- `InlineNavMenuItemHeaderTheme.axaml`

NavMenuToken 不承载 `SelectedItem`、`IsSubMenuOpen`、`IsInSelectedPath`、`IsPointerOverSubMenu`、`Level`、`IsTopLevel` 等实例状态。这些状态由控件状态模型、容器层和主题 selector 处理。

## AOT 与裁剪注意事项

NavMenu 不应通过反射访问 template part 或内部状态。Header、popup、inline child frame 和 active indicator 均通过稳定 template part 和 Avalonia 属性接入。

`NavMenuNode` 必须通过 `[GenerateScopedResourceHost]` 生成 `IResourceHost` / `IThemeVariantHost`、attachment count、host generation 和 `IDisposable` attach token。generation 用于使跨 host 切换后遗留的 stale token 失效，尤其不能让 `A -> B -> A` 中第一轮 A token 释放当前 A attachment。scoped resource host 只解决动态资源宿主及其事件订阅，不替代节点到容器 binding 的释放，也不替代 `ICommand.CanExecuteChanged` 的解绑。

命令能力的完整释放边界由三层共同组成：

1. generated scoped resource host 释放 `ResourcesChanged` / `ActualThemeVariantChanged` owner 订阅；
2. container `CompositeDisposable` 释放节点到 `NavMenuItem` 的 `Command`、`CommandParameter` 和其他属性 binding；
3. `NavMenuItem` 在 command replacement 与 logical-tree detach 时解除 `CanExecuteChanged`。

缺少任意一层都不能宣称节点命令生命周期完整。不得通过弱化动态资源、改为静态值、永久 owner 引用、全局 command cache 或延迟清理规避释放问题。

`CanExecuteChanged` 可能在一次同步命令执行中快速发出 `false -> true`，共享同一命令的多个叶子容器如果逐次立即更新 effective enabled，会同时启动 disabled 前景色过渡并产生闪动。`NavMenuItem` 将通知统一 marshal 到 UI Dispatcher，并以 `Input` 优先级合并同一 UI 周期内的重复通知；回调只重新读取一次当前 `Command.CanExecute(CommandParameter)`，因此持续 `false` 仍会在下一轮交互前生效，而瞬时变化不会暴露中间视觉状态。待处理 operation 由当前 container 持有，并在 command replacement、parameter replacement 和 logical-tree detach 时 abort，不能让已回收容器被 dispatcher callback 延迟持有或被旧 command 状态回写。

handler 持有事件订阅时必须在 mode 切换、detached 或模板替换时释放。延迟打开 / 关闭任务必须支持取消，避免旧 pointer 状态影响新 mode 或新 popup。

inline collapsed cache 不得持有 `NavMenuItem`、header、popup 或 template part 引用。状态失效边界包括 ItemsSource reset、container clear、detach、mode change 和 default path replay revision 变化。

键盘导航状态持有的 active item 引用必须随 detach、mode 切换、container clear、popup close 和 item disabled 变化失效。失效时应重新从当前可见层级解析 active 项，不保留悬空容器引用。

默认路径 replay 必须有界，避免容器生成失败时形成无休止 dispatcher 队列。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/NavMenu/NavMenu.cs`：公开控件、属性、ItemsControl 容器入口、默认路径 replay 和 mode 状态同步。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuItem.cs`：内部容器、header 转发、子菜单、popup、选中和打开状态。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuNode.cs`：`INavMenuNode` 节点契约和节点数据模型；`NavMenuNode` 使用 `[GenerateScopedResourceHost]` 获得 scoped `IResourceHost` / `IThemeVariantHost` 生命周期。
- `src/AtomUI.Desktop.Controls/NavMenu/INavMenu.cs`、`INavMenuItem.cs`、`INavMenuElement.cs`：菜单和容器的内部/公共契约。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuSelectionCoordinator.cs`：选择状态和祖先路径状态同步。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuItemContainerBinder.cs`：节点数据与容器状态绑定。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuInteractionHandlerBase.cs`：交互 handler 基类。
- `src/AtomUI.Desktop.Controls/NavMenu/DefaultNavMenuInteractionHandler.cs`：Vertical / Horizontal popup 模式交互策略。
- `src/AtomUI.Desktop.Controls/NavMenu/InlineNavMenuInteractionHandler.cs`：Inline 展开收起交互策略。
- `src/AtomUI.Desktop.Controls/NavMenu/Header/`：三种 header 控件。
- `src/AtomUI.Desktop.Controls/NavMenu/Themes/`：root、item、header 和 popup 主题。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuToken.cs`：控件 Token。

## 相关文档

- 源设计文档：`docs/controls/desktop/navigation/nav-menu/overview.md`
- 实现文档：`docs/controls/desktop/navigation/nav-menu/implementation.md`
- Token 文档：`docs/controls/desktop/navigation/nav-menu/token.md`
- 变更记录：`docs/controls/desktop/navigation/nav-menu/changelog.md`
- 语义结构：`./semantic-cn.md`
