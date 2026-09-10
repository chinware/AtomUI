# Drawer 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Drawer 的语义 owner 是 `AtomUI.Desktop.Controls.Drawer` 本身（无模板的零尺寸标记控件）。全部非 root 部件位于运行时创建的 `DrawerContainer`（注入 `ScopeAwareAdornerLayer`，在 owner 可视子树之外、同一 TopLevel 之内），因此统一声明 `CrossVisualRoot=true` + `RuntimeCreated=true`；marker 静态声明在两个内部容器主题上（`DrawerContainerTheme.axaml`、`DrawerInfoContainerTheme.axaml`），运行时无需代码注入。

与上游 Drawer 的语义 DOM（`_semantic.tsx`，10 个槽位）的映射：

| 上游 Drawer 槽位 | AtomUI 部件 | 说明 |
| --- | --- | --- |
| `root` | `root`（隐式，owner 契约） | 上游 root 是 fixed 定位容器；AtomUI 遵循系统契约 root=owner 控件，上游 root 对应内部 `DrawerContainer` 基础设施，不作为部件暴露。内联语义预览中 owner 拉伸铺满舞台，root 高亮即整个内联容器，视觉语义对齐上游。 |
| `mask` | `mask` | 一一对应；`IsShowMask=false` 时不呈现（Optional）。 |
| `section` | `section` | 上游 v6 由 `content` 改名而来；AtomUI 由 `DrawerInfoContainer` 模板中的 `Frame` Border 承载（背景/阴影层）。 |
| `header` | `header` | 一一对应（`InfoHeader` Grid）。 |
| `title` | `title` | 一一对应（`HeaderText`）。 |
| `extra` | `extra` | 一一对应（`ExtraContentPresenter`）。 |
| `body` | `body` | 一一对应（`InfoContainer` presenter，`ContentPadding` 落点）。 |
| `footer` | `footer` | 一一对应（`InfoFooter` presenter，仅设置 Footer 时可见）。 |
| `close` | `close` | 一一对应（`PART_CloseButton` IconButton）。 |
| `dragger` | 不暴露 | 上游 v6 的 resizable 拖拽手柄；AtomUI Drawer 暂无 resizable 能力，待能力落地后按上游补齐。 |
| `wrapper` | 不暴露 | 上游动效包装容器，其语义预览清单同样不包含它；对应 AtomUI 的 `PART_InfoContainerMotionActor` 动效基础设施。 |

部件明细：

#### `mask`

| 字段 | 值 |
| --- | --- |
| Owner / Part | `Drawer` / `mask` |
| Selector | `.semantic-mask` |
| SelectorRoute | `>> .semantic-mask` |
| Style Type | `AtomUI.Theme.Styling.DrawerMaskStyle` |
| ContractType | `Avalonia.Controls.Border` |
| Cardinality | Optional（`IsShowMask=false` 时不呈现） |
| CrossVisualRoot / RuntimeCreated | true / true |
| AtomUI 节点 | `DrawerContainerTheme.axaml` 的 `PART_Mask`（静态 marker） |
| 职责 | 遮罩层：绝对定位、层级、`ColorBgMask` 背景、指针事件命中 |
| 相关 API | `IsShowMask`、`IsCloseOnMaskClick` |
| 相关 Token | `ColorBgMask` |

#### `section`

| 字段 | 值 |
| --- | --- |
| Owner / Part | `Drawer` / `section` |
| Selector | `.semantic-section` |
| SelectorRoute | `>> .semantic-section` |
| Style Type | `AtomUI.Theme.Styling.DrawerSectionStyle` |
| ContractType | `Avalonia.Controls.Border` |
| Cardinality | Single |
| CrossVisualRoot / RuntimeCreated | true / true |
| AtomUI 节点 | `DrawerInfoContainerTheme.axaml` 的 `Frame`（静态 marker） |
| 职责 | 抽屉面板容器：flex 布局、宽高、背景、按 Placement 的边缘阴影 |
| 相关 API | `DialogSize`、`Placement`、`SizeType` |
| 相关 Token | `ColorBgElevated`、`BoxShadowDrawer{Left,Right,Up,Down}` |

#### `header`

| 字段 | 值 |
| --- | --- |
| Owner / Part | `Drawer` / `header` |
| Selector | `.semantic-header` |
| SelectorRoute | `>> .semantic-header` |
| Style Type | `AtomUI.Theme.Styling.DrawerHeaderStyle` |
| ContractType | `Avalonia.Controls.Grid` |
| Cardinality | Single |
| CrossVisualRoot / RuntimeCreated | true / true |
| AtomUI 节点 | `DrawerInfoContainerTheme.axaml` 的 `InfoHeader`（静态 marker） |
| 职责 | 头部区域：标题/关闭按钮/额外操作的排布、内边距 |
| 相关 API | `Title`、`Extra`、`IsShowCloseButton` |
| 相关 Token | `HeaderMargin` |

#### `title`

| 字段 | 值 |
| --- | --- |
| Owner / Part | `Drawer` / `title` |
| Selector | `.semantic-title` |
| SelectorRoute | `>> .semantic-title` |
| Style Type | `AtomUI.Theme.Styling.DrawerTitleStyle` |
| ContractType | `AtomUI.Desktop.Controls.TextBlock` |
| Cardinality | Single |
| CrossVisualRoot / RuntimeCreated | true / true |
| AtomUI 节点 | `DrawerInfoContainerTheme.axaml` 的 `HeaderText`（静态 marker） |
| 职责 | 标题文字排版 |
| 相关 API | `Title` |
| 相关 Token | `FontSizeLG`、`FontHeightLG`、`FontWeightStrong` |

#### `extra`

| 字段 | 值 |
| --- | --- |
| Owner / Part | `Drawer` / `extra` |
| Selector | `.semantic-extra` |
| SelectorRoute | `>> .semantic-extra` |
| Style Type | `AtomUI.Theme.Styling.DrawerExtraStyle` |
| ContractType | `Avalonia.Controls.Presenters.ContentPresenter` |
| Cardinality | Single |
| CrossVisualRoot / RuntimeCreated | true / true |
| AtomUI 节点 | `DrawerInfoContainerTheme.axaml` 的 `ExtraContentPresenter`（静态 marker） |
| 职责 | 头部尾缘的额外操作内容呈现 |
| 相关 API | `Extra`、`ExtraTemplate` |

#### `body`

| 字段 | 值 |
| --- | --- |
| Owner / Part | `Drawer` / `body` |
| Selector | `.semantic-body` |
| SelectorRoute | `>> .semantic-body` |
| Style Type | `AtomUI.Theme.Styling.DrawerBodyStyle` |
| ContractType | `Avalonia.Controls.Presenters.ContentPresenter` |
| Cardinality | Single |
| CrossVisualRoot / RuntimeCreated | true / true |
| AtomUI 节点 | `DrawerInfoContainerTheme.axaml` 的 `InfoContainer`（静态 marker） |
| 职责 | 主内容区：flex 占比、内边距、滚动 |
| 相关 API | `Content`、`ContentTemplate`、`ContentPadding` |
| 相关 Token | `ContentPadding` |

#### `footer`

| 字段 | 值 |
| --- | --- |
| Owner / Part | `Drawer` / `footer` |
| Selector | `.semantic-footer` |
| SelectorRoute | `>> .semantic-footer` |
| Style Type | `AtomUI.Theme.Styling.DrawerFooterStyle` |
| ContractType | `Avalonia.Controls.Presenters.ContentPresenter` |
| Cardinality | Single |
| CrossVisualRoot / RuntimeCreated | true / true |
| AtomUI 节点 | `DrawerInfoContainerTheme.axaml` 的 `InfoFooter`（静态 marker） |
| 职责 | 底部操作区：上分隔线、内边距；仅设置 Footer 时可见 |
| 相关 API | `Footer`、`FooterTemplate` |
| 相关 Token | `FooterPadding` |

#### `close`

| 字段 | 值 |
| --- | --- |
| Owner / Part | `Drawer` / `close` |
| Selector | `.semantic-close` |
| SelectorRoute | `>> .semantic-close` |
| Style Type | `AtomUI.Theme.Styling.DrawerCloseStyle` |
| ContractType | `AtomUI.Controls.IconButton` |
| Cardinality | Single |
| CrossVisualRoot / RuntimeCreated | true / true |
| AtomUI 节点 | `DrawerInfoContainerTheme.axaml` 的 `PART_CloseButton`（静态 marker） |
| 职责 | 关闭按钮：图标、hover/pressed 反馈；钉住预览时忽略其关闭请求 |
| 相关 API | `IsShowCloseButton`、`IsPinnedOpen` |
| 相关 Token | `CloseIconPadding`、`CloseIconMargin` |

隐式 `root`：由生成器无条件加入，`StyleType=null`，定制走 owner 级普通 Style（见系统文档）。

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Drawer
  -> DrawerContainer (internal container control theme, DrawerContainerTheme.axaml)
     -> Border#PART_RootClip (template-stable)
        -> Panel#RootLayout (template-stable)
           -> Border#PART_Mask (template-stable)
           -> MotionActor#PART_InfoContainerMotionActor (template-stable)
              -> DrawerInfoContainer#PART_InfoContainer (template-stable)
  -> DrawerInfoContainer (internal container control theme, DrawerInfoContainerTheme.axaml)
     -> Panel#RootLayout (template-stable)
        -> Border#Frame (template-stable)
        -> Grid#InfoLayout (template-stable)
           -> Grid#InfoHeader (template-stable)
              -> IconButton#PART_CloseButton (template-stable)
              -> TextBlock#HeaderText (template-stable)
              -> ContentPresenter#ExtraContentPresenter (internal-observable)
           -> Separator (template-stable)
           -> ContentPresenter#InfoContainer (internal-observable)
           -> Separator (template-stable)
           -> ContentPresenter#InfoFooter (internal-observable)
  -> Drawer (control theme, DrawerTheme.axaml)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Drawer` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `DrawerContainer` | internal container control theme | `DrawerContainerTheme.axaml` | Drawer | `Background`, `Content`, `ContentPadding`, `ContentTemplate`, `CornerRadius`, `DialogSize` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_RootClip` | template node (Border) | `DrawerContainerTheme.axaml` | DrawerContainer | `Background`, `Content`, `ContentPadding`, `ContentTemplate`, `CornerRadius`, `DialogSize` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RootLayout` | template node (Panel) | `DrawerContainerTheme.axaml` | DrawerContainer | `Background`, `Content`, `ContentPadding`, `ContentTemplate`, `DialogSize`, `Extra` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Mask` | template node (Border) | `DrawerContainerTheme.axaml` | DrawerContainer | `Background`, `IsShowMask` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_InfoContainerMotionActor` | template node (MotionActor) | `DrawerContainerTheme.axaml` | DrawerContainer | `Content`, `ContentPadding`, `ContentTemplate`, `DialogSize`, `Extra`, `ExtraTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_InfoContainer` | template node (DrawerInfoContainer) | `DrawerContainerTheme.axaml` | DrawerContainer | `Content`, `ContentPadding`, `ContentTemplate`, `DialogSize`, `Extra`, `ExtraTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DrawerInfoContainer` | internal container control theme | `DrawerInfoContainerTheme.axaml` | Drawer | `Content`, `ContentPadding`, `ContentTemplate`, `Extra`, `ExtraTemplate`, `Footer` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (Panel) | `DrawerInfoContainerTheme.axaml` | DrawerInfoContainer | `Content`, `ContentTemplate`, `Extra`, `ExtraTemplate`, `Footer`, `FooterTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (Border) | `DrawerInfoContainerTheme.axaml` | DrawerInfoContainer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `InfoLayout` | template node (Grid) | `DrawerInfoContainerTheme.axaml` | DrawerInfoContainer | `Content`, `ContentTemplate`, `Extra`, `ExtraTemplate`, `Footer`, `FooterTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `InfoHeader` | template node (Grid) | `DrawerInfoContainerTheme.axaml` | DrawerInfoContainer | `Extra`, `ExtraTemplate`, `HasExtra`, `IsMotionEnabled`, `IsShowCloseButton`, `Title` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CloseButton` | template node (IconButton) | `DrawerInfoContainerTheme.axaml` | DrawerInfoContainer | `IsMotionEnabled`, `IsShowCloseButton` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderText` | template node (TextBlock) | `DrawerInfoContainerTheme.axaml` | DrawerInfoContainer | `Title` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ExtraContentPresenter` | template node (ContentPresenter) | `DrawerInfoContainerTheme.axaml` | DrawerInfoContainer | `Extra`, `ExtraTemplate`, `HasExtra` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `InfoContainer` | template node (ContentPresenter) | `DrawerInfoContainerTheme.axaml` | DrawerInfoContainer | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `InfoFooter` | template node (ContentPresenter) | `DrawerInfoContainerTheme.axaml` | DrawerInfoContainer | `Footer`, `FooterTemplate`, `HasFooter` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Drawer` | control theme | `DrawerTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content`、`ContentTemplate`、`ExtraTemplate`、`FooterTemplate`、`Title` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsCloseOnMaskClick`、`IsMotionEnabled`、`IsOpen`、`IsPinnedOpen`、`IsShowCloseButton`、`IsShowMask` | 表达用户可观察状态、可用性、清除、加载或反馈语义；`IsPinnedOpen` 钉住常开（语义预览用），拦截遮罩点击与关闭按钮，不拦截外部 `IsOpen` 赋值。 |
| 视觉与布局 | `DialogSize`、`Placement`、`PushOffsetPercent`、`SizeType` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Extra`、`Footer`、`OpenOn` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Drawer Token + ControlTheme。 |

## State Flow

Drawer 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- open/close、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。
- TopLevel Drawer 使用 Window visible frame：包含 managed/drawn 标题栏，排除透明 frame shadow；该规则不按 OS 或 CSD 模式分叉。
- Drawer container 始终保留在 owning `TopLevel` 的 `ScopeAwareAdornerLayer`；Window drawn decorations 只绘制 chrome，不作为 Drawer host。

## Theme and Token Boundaries

Drawer 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `DrawerContainerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DrawerInfoContainerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Drawer 使用 `DrawerToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 open/close、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Drawer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `DrawerToken`，scope id 为 `Drawer`，源码位于 `src/AtomUI.Desktop.Controls/Drawer/DrawerToken.cs`。

## Customization Boundaries

维护 Drawer 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 不按 `OsType` 或 `IsCsdEnabled` 为 Drawer 建立平行窗口几何；Window 发布的 frame 与 titlebar metrics 是唯一几何信号。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Drawer 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- Windows、Linux、macOS 以及 CSD/non-CSD 设计上使用同一 visible-frame 语义；平台差异只存在于 Window 如何发布 frame、titlebar 与 shadow metrics，不能把共享设计规则误写成尚未执行平台的测试证据。
- Drawer 内容、popup placement target 与 owning Window 必须保持在同一 `TopLevel`；不得通过 drawn decorations host、局部 ZIndex、延迟打开或强制 native popup 掩盖跨父层遮挡。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
