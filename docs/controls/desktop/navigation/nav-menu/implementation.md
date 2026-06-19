# NavMenu 桌面版实现原理

本文档描述 NavMenu 桌面版的内部容器生成、交互 handler、选择协调、默认路径 replay、popup 接入和主题状态维护。公共设计与 API 契约见 [NavMenu 桌面版架构设计](overview.md)，Token 语义见 [NavMenu Token 设计](token.md)，变化记录见 [NavMenu Changelog](changelog.md)。

## 1. 实现定位

NavMenu 的实现目标是在 `ItemsControl` 容器体系内维护树形导航状态，并按 mode 选择不同交互策略。实现文档聚焦 `NavMenu`、`NavMenuItem`、节点模型、handler、selection coordinator 和 theme part 的协作关系。

路由切换、权限过滤、业务命令编排和页面生命周期不属于 NavMenu 实现范围。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/NavMenu/NavMenu.cs`：公开控件、属性、ItemsControl 容器入口、默认路径 replay 和 mode 状态同步。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuItem.cs`：内部容器、header 转发、子菜单、popup、选中和打开状态。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuNode.cs`：`INavMenuNode` 节点契约、节点数据模型和资源宿主挂接。
- `src/AtomUI.Desktop.Controls/NavMenu/INavMenu.cs`、`INavMenuItem.cs`、`INavMenuElement.cs`：菜单和容器的内部/公共契约。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuSelectionCoordinator.cs`：选择状态和祖先路径状态同步。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuItemContainerBinder.cs`：节点数据与容器状态绑定。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuInteractionHandlerBase.cs`：交互 handler 基类。
- `src/AtomUI.Desktop.Controls/NavMenu/DefaultNavMenuInteractionHandler.cs`：Vertical / Horizontal popup 模式交互策略。
- `src/AtomUI.Desktop.Controls/NavMenu/InlineNavMenuInteractionHandler.cs`：Inline 展开收起交互策略。
- `src/AtomUI.Desktop.Controls/NavMenu/Header/`：三种 header 控件。
- `src/AtomUI.Desktop.Controls/NavMenu/Themes/`：root、item、header 和 popup 主题。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuToken.cs`：组件 Token。

## 3. 核心类职责

`NavMenu` 是树形导航根，负责 mode、theme、items source、默认路径、受控选择、事件和子容器状态下发。

`NavMenuItem` 是内部容器，负责承载单个节点的 header、icon、子节点、popup 或 inline child items，并维护 `IsSelected`、`IsInSelectedPath`、`IsSubMenuOpen`、`Level`、`IsTopLevel` 等状态。

`NavMenuSelectionCoordinator` 统一处理旧选中节点清理、新选中节点设置、祖先路径标记和事件派发，避免选择逻辑散落在 click handler、默认路径 replay 和 property changed 分支中。

interaction handler 按 mode 分工：Inline handler 处理视觉树内展开，Default handler 处理 popup 打开、延迟关闭、窗口失焦和同级互斥。

Header 控件只承担显示和局部视觉状态，不拥有选择或打开逻辑。

## 4. 状态与数据流

状态流：

```text
NavMenu public API / NavMenuNode
  Mode / IsDarkStyle / IsItemBackgroundEnabled
  SelectedItem / DefaultSelectedPath / DefaultOpenPaths
  Header / Icon / ItemKey / Children / IsEnabled
      ↓
ItemsControl container generation
      ↓
NavMenuItemContainerBinder
  sync node data and owner menu state
      ↓
NavMenuItem
  Level / IsTopLevel / HasSubMenu / IsSubMenuOpen
      ↓
Interaction handler + SelectionCoordinator
      ↓
Header theme / Popup frame / Inline child frame
```

`SelectedItem` 是持续选择状态。`DefaultSelectedPath` 和 `DefaultOpenPaths` 只在初始路径应用中参与 replay。程序连续设置多个选择时，过期 replay 必须被忽略，只应用最新 revision。

`IsItemBackgroundEnabled` 下发到 `NavMenuItem` 和 header theme，但它只控制 item / submenu 背景块，不关闭 header 文本状态。

## 5. 生命周期与模板接入

`NavMenu` 在 mode 变化时同步 root pseudo-class、ItemsPanel 方向、interaction handler 和已打开子菜单状态。切换 mode 必须关闭旧模式下的 popup 或 inline 子菜单，避免旧 handler 的 pointer、delay、popup 或 motion 状态泄漏。

`NavMenuItem.OnApplyTemplate` 获取 header、popup、popup frame、inline motion actor、child frame、items presenter 和 active indicator。模板替换时必须解除旧 part 事件订阅，并重新绑定 handler 需要的 part。

默认路径 replay 依赖容器生成和模板应用。实现必须使用有界 replay，不使用固定 sleep 或 timer 作为容器可用性的长期机制。

## 6. 交互与事件处理

Inline handler：

- 点击带子菜单项切换 `IsSubMenuOpen`。
- 点击叶子节点进入 selection coordinator。
- `IsAccordionMode=true` 时关闭同层其他打开项。
- Inline 展开收起保持 motion，不通过临时关闭 motion 规避问题。

Default handler：

- pointer enter 可延迟打开 popup。
- pointer leave 可延迟关闭 popup。
- 点击叶子节点进入 selection coordinator。
- popup close 由 pointer、窗口失焦、非客户端点击和同级打开状态共同控制。
- Horizontal 顶层 popup 放置在下方，非顶层和 vertical popup 使用侧向层级。

`NavMenuItemClick` 表达 item 点击，`NavMenuNodeSelected` 表达叶子节点选择。禁用项不得触发有效点击或选择。

## 7. 内部算法与关键流程

### 7.1 容器绑定

节点数据到容器的绑定必须包括 Header、HeaderTemplate、Icon、ItemKey、IsEnabled、Children、owner menu、mode、dark style、background mode 和 motion 状态。容器解绑时必须释放资源宿主关系和事件订阅。

### 7.2 选择流程

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

祖先路径只标记导航路径，不应通过 ancestor pointer state 让父级 header 进入 hover 背景。

### 7.3 默认路径 replay

`TreeNodePath` 通过 `ItemKey` 定位节点路径。路径 replay 先打开中间节点，再选中叶子节点。由于容器生成依赖 layout 和 ItemsPresenter，replay 可以在 loaded priority 下有界重试。

replay 必须具备 revision 控制：新的默认路径或 `SelectedItem` 设置产生新 revision，旧 revision 的异步结果必须丢弃。

### 7.4 背景块模型

`NavMenuItem` 背景和 `NavMenuItemHeader` 背景是两层不同职责：

- item / child frame 背景表达 inline 子菜单背景块和连续层级背景。
- header 背景表达 hover、selected、disabled 等交互视觉。

`IsItemBackgroundEnabled=false` 只关闭 item / child frame 背景块和背景块专用外距。header 前景、hover、selected、selected path 和 disabled 仍由 header theme 处理。

### 7.5 Popup 模型

Popup shell 位于 `NavMenuItem` 模板内，popup content 由 `ItemsPresenter` 承载。打开 popup 前后必须确保子容器可生成，默认路径 replay 不能依赖固定等待时间。

Popup 背景使用 `MenuPopupBg` / `DarkMenuPopupBg`，不能回退成普通 elevated background。

## 8. 资源、性能与 AOT 边界

NavMenu 不应通过反射访问 template part 或内部状态。Header、popup、inline child frame 和 active indicator 均通过稳定 template part 和 Avalonia 属性接入。

handler 持有事件订阅时必须在 mode 切换、detached 或模板替换时释放。延迟打开 / 关闭任务必须支持取消，避免旧 pointer 状态影响新 mode 或新 popup。

默认路径 replay 必须有界，避免容器生成失败时形成无休止 dispatcher 队列。

## 9. 维护不变量

内部重构必须保持以下不变量：

- mode 切换时重新挂接 handler，并清理旧模式打开状态。
- 点击 item 不得临时关闭 motion。
- 默认路径应用不使用固定 50ms sleep 作为稳定策略。
- selection coordinator 是选择状态的统一入口。
- header hover / selected 背景不通过父级 item hover 状态误触发。
- `IsItemBackgroundEnabled=false` 不关闭 header 颜色和交互状态。
- popup、root、inline child frame、header 四类背景职责保持分离。
- handler 取消逻辑不能泄漏事件订阅或延迟任务。

## 10. 测试与验证

验证范围：

- Inline、Vertical、Horizontal 打开、关闭、hover、click 和同级互斥。
- `SelectedItem`、`DefaultSelectedPath`、`DefaultOpenPaths`、stale replay 和 clear selection。
- 点击子节点时父级 header 不出现错误 hover 背景。
- `IsItemBackgroundEnabled=true/false` 下 inline 背景块、header 背景和间距分别正确。
- Dark root、popup、submenu、header、selected 和 hover 颜色与 Token 语义一致。
- Popup 打开、关闭、失焦、pointer leave 和 mode 切换后无旧状态残留。
- Motion 不因点击、打开或关闭流程被临时禁用。
- 文档改动运行 `git diff --check`。
