# Message 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Message 由两个 public owner 组成，因此公开两个独立 descriptor：

| Owner | 职责 |
| --- | --- |
| `MessageCard` | 单条消息卡片（上游 notice），承载 `root`、`wrapper`、`icon`、`title` 四个 Part。 |
| `WindowMessageManager` | 服务型消息宿主（上游 message list），承载 `root`（= 上游 `list`）与 `listContent` 两个 Part。 |

上游 `MessageSemanticType` 的 `classNames` / `styles` 均为
`{ root?, wrapper?, icon?, title?, list?, listContent? }` 六个键（上游 6.6.3 稳定发布源码
`message/interface.ts`，语义 demo `message/demo/_semantic.tsx`）。上游 DOM 嵌套为
`list > listContent > root > wrapper > [icon, title]`；`root` 自 6.0.0 起公开，`wrapper`、`title`、`list`、
`listContent` 自 6.4.0 起公开。

AtomUI 把六个上游 Part 一一映射到两个 owner 的真实节点：

```text
WindowMessageManager (root，对应上游 list)
  └─ ReversibleStackPanel#PART_Items (listContent, .semantic-list-content)
       └─ MessageCard (root，对应上游 notice root)
            └─ MotionActor
                 └─ Border#PART_Frame (root 表面投影：背景/圆角/阴影/内边距)
                      └─ DockPanel#PART_HeaderContainer (wrapper, .semantic-wrapper)
                           ├─ IconPresenter#PART_IconContent (icon, .semantic-icon)
                           └─ SelectableTextBlock#PART_Message (title, .semantic-title)
```

上游 `root` 就是 message notice 本身，因此映射到 `MessageCard` owner；上游 `list` 是承载全部 notice 的定位容器，
映射到 `WindowMessageManager` owner。两个 owner 各自拥有隐式 `root`，不额外声明 `.semantic-root` marker。

与上游 DOM 的两处结构差异（Part 名称、数量与样式语义不变）：

- 上游 `list` 自身承担 placement（`top` / `left` / `right`）；AtomUI 的 `WindowMessageManager` 铺满 TopLevel 并只
  负责安全区外边距，具体对齐由 `ReversibleStackPanel#PART_Items`（`listContent`）实际承担。因此"放置"语义在
  AtomUI 由 `root`（`Position` 属性与宿主层范围的作用域）与 `listContent`（实际排列容器）共同表达。以无宿主构造
  内联使用时没有宿主层，`root` 退化为普通可放置控件，placement 语义不适用。当前控件未在
  `Position` 变化时更新伪类，主题中的 `:topcenter` 对齐选择器不可达（见第 7 节残余风险）。实测宿主构造
  （`WindowFeedbackLayer`）下卡片顶部贴顶、水平居中：窗口 1280×900 时卡片 `x=347`、`y=0`、宽 586，即
  居中值 `(1280-586)/2=347`，与上游默认的视口顶部居中浮层一致；可见上边距来自 `list`（manager）的内边距，
  而不是卡片自身外边距（见 §5.2）。
- 上游 `wrapper` 用 flex `gap: marginXS` + `align-items: center` 排列 icon 与 title；AtomUI `DockPanel` 无 `Spacing`，
  等价的图标间距由 `IconPresenter` 的 `MessageIconMargin`（右外边距 `UniformlyMarginXS`）表达，视觉结果一致。

两个 owner 的 descriptor `Since` 统一为 `6.0`（AtomUI Semantic Part 首版约定，不逐 Part 记录上游小版本）。

以下类型不持有独立 Semantic descriptor：

- `Message`（`IMessage` 的实现）是服务调用的数据对象，不是 Control。
- `IMessage` / `IMessageManager` 是服务契约接口。
- `MessageCardPseudoClass`、`MessageType` 是状态与枚举类型。
- `MessageCardToken` 是 Token scope；Semantic Part 不产生 Token identity。

### 1.1 `MessageCard`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `MessageCard` |
| Part | `root` |
| Selector | MessageCard 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `MessageCard` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | MessageCard owner（表面投影到 `Border#PART_Frame`，动效由 `MotionActor` 承载） |
| 职责 | 单条消息项根元素：承载 `Message`、`MessageType`、`Icon`、`IsClosing`、`IsClosed`、`IsMotionEnabled` 与进入/退出动效；根表面（背景、圆角、阴影、内边距）投影到模板中的 `Border#PART_Frame`。对应上游 notice root。 |
| 相关 API | `Message`、`MessageType`、`Icon`、`IsClosing`、`IsClosed`、`IsMotionEnabled`、`Close()`、`MessageClosed` |
| 相关 Token | `ContentBg`、`ContentPadding`、SharedToken（`BoxShadows`、`BorderRadiusLG`） |
| 稳定性 | stable since 6.0 |

#### `wrapper`

| 字段 | 值 |
| --- | --- |
| Owner | `MessageCard` |
| Part | `wrapper` |
| Selector | `.semantic-wrapper` |
| SelectorRoute | `/template/ .semantic-wrapper` |
| Style Type | `MessageCardWrapperStyle` |
| ContractType | `DockPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | MessageCard 模板中的 `DockPanel#PART_HeaderContainer` |
| 职责 | 图标与标题的包裹元素：决定 icon/title 的排列方向、对齐与图标间距。对应上游 notice wrapper。 |
| 相关 API | `Icon`、`Message`（决定子节点可见性） |
| 相关 Token | `MessageIconMargin` |
| 稳定性 | stable since 6.0 |

#### `icon`

| 字段 | 值 |
| --- | --- |
| Owner | `MessageCard` |
| Part | `icon` |
| Selector | `.semantic-icon` |
| SelectorRoute | `/template/ .semantic-icon` |
| Style Type | `MessageCardIconStyle` |
| ContractType | `IconPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | MessageCard 模板中的 `IconPresenter#PART_IconContent` |
| 职责 | 状态图标元素：尺寸、画刷与行高；`MessageType` 决定默认图标与状态色（Information/Loading = `ColorPrimary`，Success = `ColorSuccess`，Warning = `ColorWarning`，Error = `ColorError`）。对应上游 notice icon。 |
| 相关 API | `Icon`、`MessageType` |
| 相关 Token | `MessageIconSize`、SharedToken（`ColorPrimary`、`ColorSuccess`、`ColorWarning`、`ColorError`） |
| 稳定性 | stable since 6.0 |

#### `title`

| 字段 | 值 |
| --- | --- |
| Owner | `MessageCard` |
| Part | `title` |
| Selector | `.semantic-title` |
| SelectorRoute | `/template/ .semantic-title` |
| Style Type | `MessageCardTitleStyle` |
| ContractType | `Avalonia.Controls.SelectableTextBlock` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | MessageCard 模板中的 `SelectableTextBlock#PART_Message`（Avalonia 类型，非 `atom:` 前缀） |
| 职责 | 消息文本元素：文本颜色、字号、行高、换行与文本选择样式。对应上游 notice title（上游把 `content` 作为 notice title 渲染）。 |
| 相关 API | `Message` |
| 相关 Token | SharedToken（`FontSize`、`FontHeight`、`ColorText`、`SelectionBackground`、`SelectionForeground`） |
| 稳定性 | stable since 6.0 |

### 1.2 `WindowMessageManager`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `WindowMessageManager` |
| Part | `root` |
| Selector | WindowMessageManager 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `WindowMessageManager` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | WindowMessageManager owner（宿主层/full-screen 覆盖层；无宿主构造时为内联可放置实例） |
| 职责 | 消息列表根元素：承载 `Position`、`MaxItems`、`IsMotionEnabled`，管理宿主层安装、消息队列、超时关闭与宿主 detach；对应上游 `list` 的定位/层级/宽度语义。 |
| 相关 API | `Position`、`MaxItems`、`IsMotionEnabled`、`Show(IMessage)`、`Dispose()` |
| 相关 Token | SharedToken（`EnableMotion`） |
| 稳定性 | stable since 6.0 |

#### `listContent`

| 字段 | 值 |
| --- | --- |
| Owner | `WindowMessageManager` |
| Part | `listContent` |
| Selector | `.semantic-list-content` |
| SelectorRoute | `/template/ .semantic-list-content` |
| Style Type | `WindowMessageManagerListContentStyle` |
| ContractType | `ReversibleStackPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | WindowMessageManager 模板中的 `ReversibleStackPanel#PART_Items` |
| 职责 | 消息列表内容元素：notice 的排列方向、顺序与对齐；对应上游 `listContent` 的 notice 排列/间距语义。 |
| 相关 API | `Position`、`MaxItems`（决定内容区可见项数量） |
| 相关 Token | SharedToken（`EnableMotion`） |
| 稳定性 | stable since 6.0 |

隐式 `root` 不声明 `.semantic-root` marker。`MessageCard` 的 `wrapper`、`icon`、`title` marker 静态声明在
`MessageCardTheme.axaml` 模板内；`WindowMessageManager` 的 `listContent` marker 静态声明在
`WindowMessageManagerTheme.axaml` 模板内。四个 marker 都在 owner 自身的 `ControlTheme` 资产中，因此两个 descriptor
都不需要 `RuntimeCreated=true`，生成器按 owner 主题资产做静态校验。

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Message
  -> MessageCard (control theme, MessageCardTheme.axaml)
     -> MotionActor#{x:Static atom:BaseMotionActor.MotionActorPart} (internal-observable)
        -> Border#PART_Frame (template-stable)
           -> DockPanel#PART_HeaderContainer (template-stable)
              -> IconPresenter#PART_IconContent (template-stable)
              -> SelectableTextBlock#PART_Message (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Message` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `MessageCard` | control theme | `MessageCardTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `BoxShadow`, `CornerRadius`, `Icon` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `{x:Static atom:BaseMotionActor.MotionActorPart}` | template node (MotionActor) | `MessageCardTheme.axaml` | MessageCard | `Background`, `BorderBrush`, `BorderThickness`, `BoxShadow`, `CornerRadius`, `Icon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_Frame` | template node (Border) | `MessageCardTheme.axaml` | MessageCard | `Background`, `BorderBrush`, `BorderThickness`, `BoxShadow`, `CornerRadius`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderContainer` | template node (DockPanel) | `MessageCardTheme.axaml` | MessageCard | `Icon`, `Message` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_IconContent` | template node (IconPresenter) | `MessageCardTheme.axaml` | MessageCard | `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Message` | template node (SelectableTextBlock) | `MessageCardTheme.axaml` | MessageCard | `Message` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Icon`、`MaxItems` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsClosed`、`IsClosing`、`IsMotionEnabled` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `Position` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Message`、`MessageType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | collection/filter、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Message Token + ControlTheme。 |

## State Flow

Message 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- collection/filter、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

Message 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `MessageCardTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `WindowMessageManagerTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |

Message 使用 `MessageToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 collection/filter、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Message Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `MessageToken`，scope id 为 `Message`，源码位于 `src/AtomUI.Desktop.Controls/Message/MessageToken.cs`。

## Customization Boundaries

维护 Message 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Message 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
- `IsClosing` / `IsClosed` public 状态不得与 internal `MotionExecutionState` 合并；重复调度不得创建并行退出动效。
