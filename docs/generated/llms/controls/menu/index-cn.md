# Menu

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Menu 是 AtomUI 桌面控件体系中的菜单控件家族，用于组织命令列表、上下文操作、MenuFlyout 和数据驱动菜单项。

Menu 不负责页面级导航树、树形选择或任意弹层宿主。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Menu`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Navigation/Menu` |
| 状态 | Stable |

## 何时使用

Menu 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Menu 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Menu 是 AtomUI 桌面控件体系中的菜单控件家族，用于组织命令列表、上下文操作、MenuFlyout 和数据驱动菜单项。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `Items`、`MenuItem`、`MenuItemData` 和 `MenuSeparatorData`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close、collection/filter、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Menu Token + ControlTheme。 |

## 公共 API

Menu 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Items`、`MenuItem`、`MenuItemData`、`MenuSeparatorData` | 定义菜单项集合、数据驱动菜单项和分割项入口。 |
| 选择与集合 | `DisplayPageSize`、`IsScrollEnabled` | 维护弹层显示页数上限、滚动开关、选择、展开和集合状态。 |
| 交互与状态 | `IsMotionEnabled`、`ShouldUseOverlayPopup` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `LineWidth`、`Orientation`、`OverlayHostShadow`、`PopupRootShadow`、`SizeType` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 动效与异步 | `CloseMotion`、`MotionDuration`、`OpenMotion` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |

稳定事件包括 `IsCheckStateChanged`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`ContextMenu`、`DefaultMenuInteractionHandler`、`FlyoutMenuItemClickedEventArgs`、`Menu`、`MenuFlyout`、`MenuFlyoutPresenter`、`MenuItem`、`MenuItemData`、`MenuSeparator`、`MenuSeparatorData`、`ToggleItemsLayoutVisibleConverter`。
- 枚举：无。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ItemsPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_Popup` | `?` | 承载弹层宿主、打开关闭或候选内容。 |
| `PART_ToggleCheckbox` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ToggleRadio` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

控件专属或内部伪类包括 `MenuItemPseudoClass.TopLevel`、`TopLevel=:toplevel`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

Menu 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `IsCheckStateChanged`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。
- 类型：`ContextMenu`、`DefaultMenuInteractionHandler`、`FlyoutMenuItemClickedEventArgs`、`Menu`、`MenuFlyout`、`MenuFlyoutPresenter`、`MenuItem`、`MenuItemData`、`MenuSeparator`、`MenuSeparatorData`、`ToggleItemsLayoutVisibleConverter`。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### {gallery:MenuShowCaseLangResource MenuItemItemsSourceTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuShowCase.axaml:155`

Gallery key：`ExamplesContent` / item `4`

```axaml
<atom:Menu Name="BasicItemsSourceMenu"
           ItemsSource="{Binding MenuItems}">
    <atom:Menu.ItemTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}"
                          x:DataType="atom:IMenuItemData">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:Menu.ItemTemplate>
</atom:Menu>
```

### {gallery:MenuShowCaseLangResource InlineNavMenuItemsSourceTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuShowCase.axaml:174`

Gallery key：`ExamplesContent` / item `5`

```axaml
<atom:NavMenu Mode="Inline" Name="InlineModeMenu"
              ItemsSource="{Binding InlineNavMenuNodes}">
    <atom:NavMenu.ItemTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}"
                          x:DataType="atom:INavMenuNode">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:NavMenu.ItemTemplate>
</atom:NavMenu>
```

### {gallery:MenuShowCaseLangResource NavMenuItemItemsSourceTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuShowCase.axaml:193`

Gallery key：`ExamplesContent` / item `6`

```axaml
<atom:NavMenu Name="ItemsSourceDemoNavMenu"
```

## 状态模型

Menu 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- open/close、collection/filter、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `IsScrollEnabled` 控制弹层内容是否创建 `ScrollViewer`。滚动开启时 `DisplayPageSize` 参与最大高度计算；滚动禁用时弹层直接显示全部菜单项，不使用 `DisplayPageSize` 限高。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

子菜单的 pointer 交互采用独立的 hover intent 模型：

- `SelectedItem` 表达菜单导航和当前项状态，`IsSubMenuOpen` 表达已提交的弹层状态；二者都不能作为延迟任务是否仍然有效的唯一依据。
- pointer 进入带子菜单的非顶层项时，只为当前目标建立延迟打开意图。pointer 在延迟完成前离开该项时，打开意图立即失效，子菜单不得在离开后继续弹出。
- 已打开子菜单的关闭延迟只用于允许 pointer 从父项移动到其弹层。pointer 重新进入父项、子菜单弹层或其后代项时，待执行的关闭意图必须失效。
- 同一目标不能同时持有互相矛盾的打开和关闭意图。不同兄弟项切换时可以同时存在“关闭旧项”和“打开新项”，但每类意图最多只有一个当前目标。
- keyboard、access key 和 pointer press 触发的显式打开不经过 hover 延迟，不得被旧 hover callback 覆盖或回滚。
- 菜单关闭、窗口失活、宿主解除连接或交互处理器 detach 时，所有未完成 hover intent 必须统一失效。

## 主题与 Design Token

Menu 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `ContextMenuTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `MenuItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `MenuSeparatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `MenuTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TopLevelMenuItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `src/AtomUI.Desktop.Controls/Flyouts/Themes/MenuFlyoutPresenterTheme.axaml` | 定义 `MenuFlyout` 菜单项 presenter 的弹层内容模板和滚动承载结构。 |

Menu 使用 `MenuToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 open/close、collection/filter、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 弹层滚动开关通过内部 `MenuPopupScrollHost` 复用模板分支；禁用滚动时不能保留隐藏或禁用状态的 `ScrollViewer`。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Menu Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `MenuToken`，scope id 为 `Menu`，源码位于 `src/AtomUI.Desktop.Controls/Menu/MenuToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。
- hover intent 调度只使用显式类型、委托和 `IDisposable` 生命周期，不依赖反射或运行时类型扫描。
- 滚动模式使用共享 styled property、AXAML 模板分支和 `TemplateBinding`，不引入运行时反射、字符串 binding、动态注册或生成器变更。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 默认滚动路径允许保留一个轻量 `MenuPopupScrollHost` 节点，以换取四套弹层模板的统一分支；禁用滚动路径必须移除 `ScrollViewer` 子树，但会让全部菜单项直接参与测量和显示。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。
- 默认调度路径在 intent 失效时停止实际 timer；兼容的外部 delay runner 至少必须通过原子 callback clearing/release 使 callback 失效，避免过期任务修改状态并保留 owner graph。
- 交互处理器只保留当前 pending open/close 目标，不维护随 pointer 移动增长的历史队列。

## 源码索引

主要源码文件：

- `src/AtomUI.Controls.Shared/IScrollAware.cs`
- `src/AtomUI.Desktop.Controls/Flyouts/MenuFlyout.cs`
- `src/AtomUI.Desktop.Controls/Flyouts/MenuFlyoutPresenter.cs`
- `src/AtomUI.Desktop.Controls/Flyouts/Themes/MenuFlyoutPresenterTheme.axaml`
- `src/AtomUI.Desktop.Controls/Menu/ContextMenu.cs`
- `src/AtomUI.Desktop.Controls/Menu/ContextMenuReflectionExtensions.cs`
- `src/AtomUI.Desktop.Controls/Menu/Converters/ToggleItemsLayoutVisibleConverter.cs`
- `src/AtomUI.Desktop.Controls/Menu/DefaultMenuInteractionHandler.cs`
- `src/AtomUI.Desktop.Controls/Menu/Menu.cs`
- `src/AtomUI.Desktop.Controls/Menu/MenuItem.cs`
- `src/AtomUI.Desktop.Controls/Menu/MenuItemData.cs`
- `src/AtomUI.Desktop.Controls/Menu/MenuItemPseudoClass.cs`
- `src/AtomUI.Desktop.Controls/Menu/MenuPopupScrollHost.cs`
- `src/AtomUI.Desktop.Controls/Menu/MenuSeparator.cs`
- `src/AtomUI.Desktop.Controls/Menu/MenuSeparatorData.cs`
- `src/AtomUI.Desktop.Controls/Menu/MenuToken.cs`
- `src/AtomUI.Desktop.Controls/Menu/Themes/ContextMenuTheme.axaml`
- `src/AtomUI.Desktop.Controls/Menu/Themes/MenuItemTheme.axaml`
- `src/AtomUI.Desktop.Controls/Menu/Themes/MenuItemTheme.cs`
- `src/AtomUI.Desktop.Controls/Menu/Themes/MenuSeparatorTheme.axaml`
- `src/AtomUI.Desktop.Controls/Menu/Themes/MenuTheme.axaml`
- `src/AtomUI.Desktop.Controls/Menu/Themes/TopLevelMenuItemTheme.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/navigation/menu/overview.md`
- 实现文档：`docs/controls/desktop/navigation/menu/implementation.md`
- Token 文档：`docs/controls/desktop/navigation/menu/token.md`
- 变更记录：`docs/controls/desktop/navigation/menu/changelog.md`
- 语义结构：`./semantic-cn.md`
