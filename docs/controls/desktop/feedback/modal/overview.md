# Modal 桌面版架构设计

本文档定义 `Modal` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [Modal 桌面版实现原理](implementation.md)，Modal Token 的专项设计见 [Modal Token 设计](token.md)，设计和契约变化记录见 [Modal Changelog](changelog.md)。

## 1. 控件定位

Modal 是 AtomUI 桌面控件体系中的模态对话框控件，用于阻断当前流程并承载确认、表单或复杂内容。

Modal 不负责轻量消息、通知卡片或普通浮出层。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Dialog`

## 2. 设计语言

Modal 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Modal 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Modal 是 AtomUI 桌面控件体系中的模态对话框控件，用于阻断当前流程并承载确认、表单或复杂内容。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `AbortButtonText`、`AddOnTemplate`、`ApplyButtonText`、`CancelButtonText`、`CheckedIcon`、`CloseButtonText`、`Content`、`ContentTemplate` 等 33 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、open/close、loading/async、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Modal Token + ControlTheme。 |

## 3. API 与契约模型

Modal 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `AbortButtonText`、`AddOnTemplate`、`ApplyButtonText`、`CancelButtonText`、`CheckedIcon`、`CloseButtonText`、`Content`、`ContentTemplate`、`DialogContent`、`DialogContentTemplate` 等 33 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `IsChecked` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsActivated`、`IsClosable`、`IsCloseButtonEnabled`、`IsConfirmLoading`、`IsDragMovable`、`IsEffectiveFooterVisible`、`IsFooterVisible`、`IsLoading`、`IsMaximizable`、`IsMaximizeButtonEnabled` 等 17 项 | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `HorizontalOffset`、`HorizontalStartupLocation`、`HostHeight`、`HostMaxHeight`、`HostMaxWidth`、`HostMinHeight`、`HostMinWidth`、`HostWidth`、`PlacementTarget`、`VerticalOffset` 等 11 项 | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 弹层与窗口 | `DialogHostType` | 控制 popup、flyout、dialog、window 或 overlay 宿主协作。 |
| 动效与异步 | `AnimationDuration` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `AddOn`、`DefaultStandardButton`、`EscapeStandardButton`、`Logo`、`Result`、`StandardButtons` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`Dialog`、`DialogActionResult`、`DialogBoxButtonSyncEventArgs`、`DialogButton`、`DialogButtonBox`、`DialogButtonClickedEventArgs`、`DialogCaptionButton`、`DialogFinishedEventArgs`、`DialogHost`、`DialogWindowContent`、`OverlayDialogHeader`、`OverlayDialogHost`、`OverlayDialogMask`、`OverlayDialogResizeEventArgs` 等 19 项。
- 枚举：`DialogButtonRole`、`DialogCode`、`DialogHorizontalAnchor`、`DialogHostType`、`DialogStandardButton`、`DialogVerticalAnchor`、`OverlayDialogState`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ButtonBox` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_CenterGroup` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_CloseButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_Header` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_LeftGroup` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_MaximizeButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_RightGroup` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_RootLayout` | `?` | 承载根视觉、边框、背景或尺寸基线。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 4. 行为与状态模型

Modal 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、open/close、loading/async、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 5. 视觉与主题模型

Modal 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `DialogButtonBoxTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `DialogCaptionButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `DialogHostTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `DialogTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `DialogThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `DialogWindowContentTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `OverlayDialogHeaderTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `OverlayDialogHostTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `OverlayDialogMaskTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `OverlayDialogResizerTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |

Modal 使用 `DialogToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、open/close、loading/async、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

## 6. 控件家族或集成关系

Modal 与同分类控件共享尺寸、状态、Token、Gallery 展示和验证规则。组合或派生控件应显式说明哪些 API 被继承、覆盖或不支持。

主要协作类型：

- `Dialog`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DialogActionResult`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DialogButton`：动作触发类型，负责点击、导航或局部操作状态。
- `DialogButtonBox`：模板协作类型，承载内容展示、宿主或视觉边界。
- `DialogCaptionButton`：动作触发类型，负责点击、导航或局部操作状态。
- `DialogHost`：模板协作类型，承载内容展示、宿主或视觉边界。
- `DialogHostTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `DialogToken`：组件 Token scope，负责从全局 token 派生控件语义变量。
- `DialogWindowContent`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `OverlayDialogHeader`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `OverlayDialogHost`：模板协作类型，承载内容展示、宿主或视觉边界。
- `OverlayDialogHostTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `OverlayDialogMask`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `OverlayDialogResizer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `OverlayDialogResizerVisibleConverter`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `en_US`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `zh_CN`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `zh_TW`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。

集成关系：

- 与 ThemeManager、SharedToken、ControlTheme 和 Gallery ShowCase 的示例/API/Token 表保持一致。
- 涉及 ItemsSource、Popup、Flyout、Window、Form 或 CompactSpace 的路径必须保持生命周期释放和数据状态同步。
- 源码目录中的共享基类和内部协作类型形成维护边界，不能只修改桌面包装类而忽略共享状态 owner。

## 7. 兼容性不变量

维护 Modal 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 选择与当前项模型

Modal 的当前项状态必须由单一 owner 推导。public 选择属性、集合项容器和伪类之间只能做单向同步，集合替换、清空和模板重套用时必须回放当前状态。

### 8.2 弹层与宿主模型

Modal 涉及弹层、窗口或 overlay 宿主时，打开状态、取消事件、定位和宿主释放必须保持一致。重复打开、关闭、窗口失活和 template reapply 都必须释放旧宿主引用。

### 8.3 动效模型

Modal 的动效只表达状态变化反馈，不应改变 public API 语义。初始加载、禁用态和卸载路径应能抑制或取消动效，避免保留旧控件实例。

### 8.4 视觉选项模型

Modal 的视觉选项通过 public API 归一为 theme variables、伪类或模板绑定。Token 保存组件语义值，不能保存实例运行时状态或业务色值。

## 9. 文档导航与验证策略

关联文档：

- [Modal 桌面版实现原理](implementation.md)
- [Modal Token 设计](token.md)
- [Modal Changelog](changelog.md)

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和继承语义。 |
| 状态模型 | 覆盖 selection/checked/active、open/close、loading/async、input/value、motion、visual option、disabled、hover、pressed、focus 以及控件特有状态。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和 Browser 主题。 |
| Token | 检查 TokenKind、AXAML token resource、Gallery Token 表和文档同步。 |
| Gallery | 走查对应 ShowCase 示例、API 表和 Token 表入口。 |
