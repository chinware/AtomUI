# NavMenu

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

NavMenu 是 AtomUI 桌面导航体系中的层级菜单导航控件，用于表达应用页面、模块、功能入口或命令集合之间的层级关系。它以树形节点为数据模型，以 `Inline`、`Vertical`、`Horizontal` 三种模式映射到不同导航场景。

NavMenu 的职责是管理导航节点容器生成、层级展开、选中路径、弹出式子菜单、主题视觉和菜单交互。它不负责路由切换、页面生命周期、权限过滤、数据懒加载、业务命令编排或页面内容渲染。业务导航行为可以通过节点 `Command`、`NavMenuNodeSelected`、`NavMenuItemClick` 或外部 ViewModel 处理。

NavMenu 支持两种 entry 提供方式：

- 直接在 `NavMenu.Items` 中放置 `INavMenuEntry`。
- 通过 `ItemsSource` 绑定 `INavMenuEntry` 集合。

`INavMenuEntry` 是节点、分组和分隔线的共同结构契约。`INavMenuNode` 表达可交互导航节点，`NavMenuGroup` 表达带标题的透明结构分组，`NavMenuDivider` 表达不可交互分隔线。首版支持的 entry 种类封闭为这三类；仅自行实现 `INavMenuEntry` 不能注册第四种容器类型，扩展交互节点应实现 `INavMenuNode`。NavMenu 不支持把任意 `Control`、`Panel` 或 `TextBlock` 作为 entry 直接加入；Header、Footer 和分组标题的任意内容通过对应 content/template API 承载。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Navigation/Menu`；结构化 entry 示例位于 Menu ShowCase 最后一项` |
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
| `Header` | `object?` | 根导航固定头部内容；Inline/Vertical 位于滚动菜单区上方，Horizontal 位于菜单项左侧。 |
| `HeaderTemplate` | `IDataTemplate?` | 根导航头部内容模板。 |
| `Footer` | `object?` | 根导航固定尾部内容；Inline/Vertical 位于滚动菜单区下方，Horizontal 位于菜单项右侧。 |
| `FooterTemplate` | `IDataTemplate?` | 根导航尾部内容模板。 |
| `ItemSpacing` | `double` | 默认 ItemsPanel 中相邻 entry 容器的额外间距；默认值来自当前 mode 的主题映射。 |
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
| `INavMenuNode.Entries` | `IEnumerable<INavMenuEntry>` | 容器读取子 entry 的协变契约。接口默认返回既有 `Children`，自定义节点无需新增实现。 |
| `NavMenuNode.Entries` | `IList<INavMenuEntry>` | 内置节点的唯一有序、可写子 entry 集合，也是 XAML content 属性；允许节点、任意层级分组和分隔线。 |
| `Children` | `IList<INavMenuNode>` | `Entries` 的实时语义节点兼容视图，不保存第二份集合；分组对该视图透明。 |

结构 entry API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `INavMenuEntry` | interface | NavMenu 有序 entry 的共同标记契约，不带选择、命令或路径语义；它不是自定义容器注册点。 |
| `NavMenuGroup.Header` | `object?` | 分组标题内容。 |
| `NavMenuGroup.HeaderTemplate` | `IDataTemplate?` | 分组标题模板。 |
| `NavMenuGroup.Entries` | `IList<INavMenuEntry>` | 分组内有序 entry 集合，允许继续嵌套分组。 |
| `NavMenuDivider` | class | 不可聚焦、不可选中、不进入路径或键盘漫游的结构分隔线。 |

事件 API：

| 事件 | 参数 | 语义 |
| --- | --- | --- |
| `NavMenuItemClick` | `NavMenuItemClickEventArgs` | 菜单项点击事件，事件参数暴露 `INavMenuItem` 作为只读交互上下文。 |
| `NavMenuNodeSelected` | `NavMenuNodeSelectedEventArgs` | 叶子节点选中事件，事件参数暴露 `INavMenuNode`。 |

`DefaultSelectedPath` 和 `DefaultOpenPaths` 是默认值入口，不是持续受控展开状态。运行期受控选择应使用 `SelectedItem`。

`NavMenuNode.Entries` 是内置节点子结构的唯一真源。`Children` 保持既有类型和节点语义：枚举时递归穿过同一语义层级内的 `NavMenuGroup`，但不进入子节点自身的后代；`Add` 把节点追加为直接 entry，`Insert`、替换和删除写回节点当前所在的实际 entry owner，`Clear` 清空当前节点的整个 `Entries`。`INavMenuNode.Entries` 使用 `IEnumerable<INavMenuEntry>`，利用 `IEnumerable<T>` 协变在接口默认实现中直接返回既有 `IEnumerable<INavMenuNode> Children`，不创建适配集合；`NavMenuNode` 通过显式接口实现把其可写 `Entries` 投影为该读取契约。因此既有纯节点自定义实现保持源码兼容，并继续由原 `Children` 集合通知驱动容器更新。

根 `NavMenu.Items`、根 `ItemsSource`、`NavMenuNode.Entries` 和 `NavMenuGroup.Entries` 使用同一 entry 类型边界：集合中的每一项必须是 `INavMenuNode`、`NavMenuGroup` 或 `NavMenuDivider`；`null`、普通业务对象、内部生成容器和仅实现 marker 的其他类型都不是合法数据项。直接 `Items` 变更与 `ItemsSource` 的初始装载、替换以及 Add、Replace、Reset 通知必须进入同一验证入口；发现非法项时立即抛出包含来源和索引信息的 `InvalidOperationException`，不能静默跳过、按普通内容呈现或等到容器绑定阶段再产生类型错误。

内置 `NavMenuNode` 和 `NavMenuGroup` 是有状态结构 entry，同一实例在整个 entry 树中只能拥有一个直接结构 owner，不能在同一集合重复，也不能同时挂到两个节点、分组或根 `NavMenu`。从原 owner Remove、Replace、Clear 或移除根 source 后，该实例可以重新挂载。结构 owner 使用弱引用，外部长期持有 node/group 不会反向保留已经不可达的根菜单。`NavMenuDivider` 不保存选择、展开、父级、资源或容器状态，因此同一 divider 实例允许在多个位置复用。

自定义 `INavMenuNode` 实例自身保持既有兼容契约，不强制登记内置 structural owner；但其 `Entries` / `Children` 中出现的内置 `NavMenuNode` 或 `NavMenuGroup` 仍必须参加完整引用唯一性检查。每个内置 `NavMenuNode` / `NavMenuGroup` entry owner 和根 `NavMenu` 都协调自己的结构 scope：遍历直接 entry，并递归穿过 custom node；遇到内置 node/group 后由该内置 entry 自己的协调器接管后代。custom node 下的内置后代继承最近的内置 entry owner，只有根级 custom node 的内置后代才由根菜单作为结构 owner。所有 scope 组合后覆盖完整 entry 图，因此离线构造的树也不能通过 custom wrapper 绕过唯一性，同时纯 built-in 深树不会形成祖先对后代集合的重复订阅。协调器只弱订阅当前 scope 内可通知的 custom entry source；custom node 增删内置后代时立即重新协调，在加入时拒绝指回当前 built-in owner 或任意 built-in 祖先的动态环，并在移除后释放 owner。不可通知的自定义 enumerable 按每次所属集合或可通知 custom 祖先变化时取得的当前快照校验。

`NavMenuNode.Entries` 和 `NavMenuGroup.Entries` 是控件拥有的可写集合，单项 Add、Insert、Replace 以及批量初始化先完成类型、循环、直接 owner 和批次重复校验，失败时不产生部分写入。成功写入先同步完整 structural ownership，再调用自定义节点的 parent callback 或分组语义父级投影，最后发送集合通知；因此可重入回调和观察者都不能抢占已经属于本次写入的 entry 或内置后代。批量初始化一次提交完整批次，并只发送单次 Add 通知。根 direct `Items` 与任意外部 `ItemsSource` 都通过 post-mutation 的 `ItemsView.CollectionChanged` 到达控件；根 `NavMenu` 对完整 entry 图做校验并确定性抛错，但不通过重入 Remove/Replace 回滚或篡改数据源。Move 和 Remove 仍执行正常的 owner、父级、容器和生命周期协调。

`IsInlineCollapsed` 是唯一折叠状态源。NavMenu 不增加语义相反的 `Expanded` / `IsExpanded` 属性；调用方需要正向展开状态时，通过双向 binding converter 映射 `IsInlineCollapsed`，避免两个公共状态互相写回。

节点命令遵守 Avalonia 命令语义：`NavMenuNode` / `INavMenuNode` 只保存 `Command` 和 `CommandParameter`，实际执行、`CanExecute` 评估和 `CanExecuteChanged` 生命周期由生成出的 `NavMenuItem` 容器负责。`INavMenuNode` 为两个成员提供 `null` 默认实现，使既有自定义节点实现无需声明命令也能继续工作。`CommandParameter=null` 表示显式空参数，控件不能自动替换为 `ItemKey`；需要使用业务 key 时，应显式把 `ItemKey` 绑定或赋值给 `CommandParameter`。容器按 UI 周期合并连续的 `CanExecuteChanged` 通知：持续的 `false` 仍进入 disabled，同一同步执行周期内的 `false -> true` 瞬时变化只投影最终状态，避免多个共享命令节点触发无业务意义的禁用颜色闪动。

`IsInlineCollapsed` 只对 `Mode=Inline` 生效。`Mode=Vertical` 或 `Mode=Horizontal` 时设置该属性不应改变当前模式的 popup、布局或键盘语义。`InlineCollapsedWidth` 参与布局测量，默认通过 theme setter 取得 `NavMenuToken.InlineCollapsedWidth`；collapsed 状态下由控件内部对 `Width` / `MinWidth` 做有效值 coercion，本地设置的属性值应按 Avalonia 属性优先级覆盖 token 默认值，展开后原始 `Width` 或绑定必须恢复。

键盘漫游状态属于内部交互状态，不进入公共 API。NavMenu 保持与 参考 Menu 一致的分层：`SelectedItem` 表示已提交的导航选择，`DefaultOpenPaths` / `IsSubMenuOpen` 表示展开状态，键盘当前项只表示临时 active/focus 目标。业务代码不应通过公开属性控制键盘 active 项，也不应把 active 项误认为已选择节点。

稳定 template part：

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

## 事件与命令

NavMenu 的公共 API 分为控件 API、节点 API 和事件 API。
| `Command` | `ICommand?` | 节点被有效触发时由当前 `NavMenuItem` 容器执行的业务命令。节点只承载命令配置，不负责订阅或执行。 |
| `INavMenuEntry` | interface | NavMenu 有序 entry 的共同标记契约，不带选择、命令或路径语义；它不是自定义容器注册点。 |
事件 API：
| 事件 | 参数 | 语义 |
| `NavMenuItemClick` | `NavMenuItemClickEventArgs` | 菜单项点击事件，事件参数暴露 `INavMenuItem` 作为只读交互上下文。 |
| `NavMenuNodeSelected` | `NavMenuNodeSelectedEventArgs` | 叶子节点选中事件，事件参数暴露 `INavMenuNode`。 |
节点命令遵守 Avalonia 命令语义：`NavMenuNode` / `INavMenuNode` 只保存 `Command` 和 `CommandParameter`，实际执行、`CanExecute` 评估和 `CanExecuteChanged` 生命周期由生成出的 `NavMenuItem` 容器负责。`INavMenuNode` 为两个成员提供 `null` 默认实现，使既有自定义节点实现无需声明命令也能继续工作。`CommandParameter=null` 表示显式空参数，控件不能自动替换为 `ItemKey`；需要使用业务 key 时，应显式把 `ItemKey` 绑定或赋值给 `CommandParameter`。容器按 UI 周期合并连续的 `CanExecuteChanged` 通知：持续的 `false` 仍进入 disabled，同一同步执行周期内的 `false -> true` 瞬时变化只投影最终状态，避免多个共享命令节点触发无业务意义的禁用颜色闪动。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

Gallery 目录 `controlgallery/AtomUIGallery/ShowCases/Navigation/Menu`；结构化 entry 示例位于 Menu ShowCase 最后一项` 当前不存在；请检查控件文档中的 Gallery 页面元数据。

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

结构 entry 不进入公共交互状态：分组和分隔线不产生 `Selected`、`KeyboardActive`、`Open`、`ItemKey` 或 `Command`。分组中的节点仍使用最近的节点祖先作为 `ParentNode`；分组本身不增加 `Level`，也不进入 `TreeNodePath`。

## 主题与 Design Token

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
- Keyboard active 背景使用 `ItemActiveBg`，其优先级低于 `Selected`，高于普通默认态；它可以叠加在 `IsInSelectedPath` 父节点上，使父节点保留 selected-path 文字色的同时显示临时 active 背景。dark style 下使用 dark 语义的 active 视觉，不复用 selected 背景表达临时漫游。
- Inline collapsed 根宽度使用 `InlineCollapsedWidth`，默认来自 `NavMenuToken.InlineCollapsedWidth=48`。折叠视觉只作用于 `Mode=Inline && IsInlineCollapsed=true`：一级 icon 使用 `CollapsedIconSize` 居中，标题和箭头收起，未配置 icon 的一级项显示标题首字符，叶子项可用 tooltip 展示完整标题。
- Inline/Vertical 的 Header 和 Footer 位于菜单滚动区之外；无 Header/Footer 时对应 presenter 折叠，不占用布局空间。Horizontal 中 Header 左停靠、Footer 右停靠，菜单项占用中间区域。进入 inline collapsed 后 Header 保持可见以承载展开入口，Footer 自动隐藏；Header 内容需要根据 `IsInlineCollapsed` 自适应折叠宽度。
- 根层 inline collapsed 分组标题隐藏，分组及其透明嵌套分组内的节点继续继承根折叠状态，按顶层节点使用 `CollapsedIconSize` 居中；popup 或非根语义层级中的分组标题和节点保持普通 vertical 视觉。Horizontal 根层把分组渲染为透明水平集合并隐藏标题，popup 中恢复垂直分组标题。
- Horizontal 根层分隔线为竖线；Inline、Vertical、popup 和 inline collapsed 根层分隔线为横线。
- 根默认 ItemsPanel 通过 `TemplateBinding` 消费公开 `ItemSpacing`；submenu、popup 和 group 默认 ItemsPanel 消费由根控件投影的内部 effective spacing。该路径不使用进入子控件模板的 selector，也不建立逐容器 binding。自定义 ItemsPanel 是否消费 spacing 由自定义面板负责。
- `IsItemBackgroundEnabled=true` 时，inline child frame 使用 `SubMenuItemBg` / `DarkSubMenuItemBg`，并应用背景块专用外距。
- `IsItemBackgroundEnabled=false` 时，inline child frame 背景为 `Transparent`，不应用背景块专用外距；header 的文字色、hover、selected 和 selected path 仍然生效。
- Horizontal 顶层 light style 通过 `PART_ActiveIndicator` 表达选中；dark style 可以使用 selected background。

Header 背景与 NavMenuItem / inline submenu 背景块是不同职责，不应混为一个 selector 控制。

Token 来源：

NavMenuToken 是 NavMenu 的组件级设计变量层。它把全局颜色、尺寸、间距、圆角、字体和 popup 体系转换为 NavMenu 可消费的语义值。

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

## AOT 与裁剪注意事项

NavMenu 不应通过反射访问 template part 或内部状态。Header、popup、inline child frame 和 active indicator 均通过稳定 template part 和 Avalonia 属性接入。

`NavMenuNode` 必须通过 `[GenerateScopedResourceHost]` 生成 `IResourceHost` / `IThemeVariantHost`、attachment count、host generation 和 `IDisposable` attach token。generation 用于使跨 host 切换后遗留的 stale token 失效，尤其不能让 `A -> B -> A` 中第一轮 A token 释放当前 A attachment。scoped resource host 只解决动态资源宿主及其事件订阅，不替代节点到容器 binding 的释放，也不替代 `ICommand.CanExecuteChanged` 的解绑。

`NavMenuGroup` 遵守相同的 generated scoped resource-host 生命周期；结构容器不能永久持有 group 或 owner menu。`NavMenuDivider` 不需要资源宿主和订阅。

命令能力的完整释放边界由三层共同组成：

1. generated scoped resource host 释放 `ResourcesChanged` / `ActualThemeVariantChanged` owner 订阅；
2. container `CompositeDisposable` 释放节点到 `NavMenuItem` 的 `Command`、`CommandParameter` 和其他属性 binding；
3. `NavMenuItem` 在 command replacement 与 logical-tree detach 时解除 `CanExecuteChanged`。

缺少任意一层都不能宣称节点命令生命周期完整。不得通过弱化动态资源、改为静态值、永久 owner 引用、全局 command cache 或延迟清理规避释放问题。

`CanExecuteChanged` 可能在一次同步命令执行中快速发出 `false -> true`，共享同一命令的多个叶子容器如果逐次立即更新 effective enabled，会同时启动 disabled 前景色过渡并产生闪动。`NavMenuItem` 将通知统一 marshal 到 UI Dispatcher，并以 `Input` 优先级合并同一 UI 周期内的重复通知；回调只重新读取一次当前 `Command.CanExecute(CommandParameter)`，因此持续 `false` 仍会在下一轮交互前生效，而瞬时变化不会暴露中间视觉状态。待处理 operation 由当前 container 持有，并在 command replacement、parameter replacement 和 logical-tree detach 时 abort，不能让已回收容器被 dispatcher callback 延迟持有或被旧 command 状态回写。

handler 持有事件订阅时必须在 mode 切换、detached 或模板替换时释放。延迟打开 / 关闭任务必须支持取消，避免旧 pointer 状态影响新 mode 或新 popup。任务必须记录对应目标容器，container clear 只取消引用该容器的 pending open / close，不能误取消其他项的当前 hover。

inline collapsed cache 不得持有 `NavMenuItem`、header、popup 或 template part 引用。状态失效边界包括 ItemsSource reset、container clear、detach、mode change 和 default path replay revision 变化。

生成容器进入 clear/recycle 时必须通过 `NavMenu.ForgetGeneratedContainer` 同时通知 selection coordinator 和当前 interaction handler。该入口清除 selection coordinator 的 realized container 引用、keyboard-active 引用、pointer press/release 目标，以及 Default handler 指向该容器的 pending open / close；之后才释放 binding、resource host 和 entry context，避免旧交互回调读取已清空的数据对象。

键盘导航状态持有的 active item 引用必须随 detach、mode 切换、container clear、popup close 和 item disabled 变化失效。popup 内容可能在关闭后继续保留生成容器，因此有效性不能只看局部 `IsVisible`；必须同时验证 effective visible/effective enabled 和完整语义父链的打开状态。失效后第一次 Enter 只从当前可见层级重新建立 active 项，不提交旧项。

默认路径 replay 必须有界，避免容器生成失败时形成无休止 dispatcher 队列。

纯节点菜单必须保持一个数据节点对应一个 `NavMenuItem`，不得为统一模型增加额外 wrapper。只有实际存在 `NavMenuGroup` 或 `NavMenuDivider` 时才生成结构容器。语义 `Level` 在 prepare 上下文中 O(1) 得到；直接容器和路径查找优先走无分组快路径；spacing 只在根控件计算公开值与后代默认值的关系，再通过内部继承属性和 ItemsPanel `TemplateBinding` 传播，不为每个节点增加 relay binding。

默认 TreeDataTemplate 对 `INavMenuNode.Entries` 使用编译绑定；自定义节点通过协变接口默认实现回退到 `Children`。内置集合直接满足 Avalonia ItemsSource 的 `IList` / `INotifyCollectionChanged` 契约，该默认路径不分配适配集合。实现不使用反射、运行期类型扫描、动态 Style、dispatcher 延时或持久化扁平导航缓存，保持 NativeAOT 可分析。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/NavMenu/NavMenu.cs`：公开控件、属性、ItemsControl 容器入口、默认路径 replay 和 mode 状态同步。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuItem.cs`：内部容器、header 转发、子菜单、popup、选中和打开状态。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuEntry.cs`：`INavMenuEntry` 最小标记契约和无状态 `NavMenuDivider` 数据模型。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuNode.cs`：`INavMenuNode` 契约、接口级协变 entry 读取入口、`NavMenuNode.Entries` 唯一可写集合和 `Children` 实时兼容视图；`NavMenuNode` 使用 `[GenerateScopedResourceHost]` 获得 scoped `IResourceHost` / `IThemeVariantHost` 生命周期。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuGroup.cs`：公开分组 entry 数据模型，管理透明 entry 子树和 scoped resource-host 生命周期。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuEntryCollection.cs`、`NavMenuEntryGraph.cs`、`NavMenuEntryOwnershipCoordinator.cs`：内置 entry owner 的事务前校验、循环检测、完整图唯一性协调、弱 structural owner 和嵌套 source 生命周期。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuGroupItem.cs`、`NavMenuDividerItem.cs`：内部不可交互结构容器。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuEntryContainerCoordinator.cs`：统一节点、分组和分隔线的容器类型分派、准备、清理和回收键。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuSemanticNavigator.cs`：按语义 owner 遍历已生成节点容器，跳过结构 entry。
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
