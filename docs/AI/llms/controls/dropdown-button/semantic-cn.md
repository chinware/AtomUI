# DropdownButton 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `DropdownButton` | 导航控件根语义区域，承载 public API、状态归一和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载点击、键盘、打开关闭、跳转或提交入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `导航项区域` | 承载当前项、选中项、禁用项、层级项或分页项状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或内容区域` | 承载 flyout、dropdown、tab content、submenu 或候选内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达打开关闭、选中指示、切换和过渡反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
DropdownButton
  -> DropdownButton (control theme, BrowserButtonThemes.axaml)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> DashedBorder#Frame (template-stable)
        -> Border#CustomBackgroundLayer (template-stable)
        -> Border (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> IconPresenter#PART_DropdownIndicator (template-stable)
              -> LoadingOutlined#PART_LoadingIcon (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
  -> DropdownButton (control theme, DropdownButtonTheme.axaml)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> DashedBorder#Frame (template-stable)
        -> Border#CustomBackgroundLayer (template-stable)
        -> Border (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> IconPresenter#PART_DropdownIndicator (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `DropdownButton` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `DropdownButton` | control theme | `BrowserButtonThemes.axaml` | 用户代码 / 控件宿主 | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `CustomBackground` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `BrowserButtonThemes.axaml` | DropdownButton | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `CustomBackground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_WaveSpirit` | template node (WaveSpiritDecorator) | `BrowserButtonThemes.axaml` | DropdownButton | `EffectiveCornerRadius`, `IsMotionEnabled`, `IsWaveSpiritEnabled`, `WaveSpiritType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ShadowsFrame` | template node (Border) | `BrowserButtonThemes.axaml` | DropdownButton | `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (DashedBorder) | `BrowserButtonThemes.axaml` | DropdownButton | `Background`, `BackgroundSizing`, `BorderBrush`, `EffectiveBorderThickness`, `EffectiveCornerRadius`, `Height` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CustomBackgroundLayer` | template node (Border) | `BrowserButtonThemes.axaml` | DropdownButton | `CustomBackground`, `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RootLayout` | template node (DockPanel) | `BrowserButtonThemes.axaml` | DropdownButton | `Content`, `ContentTemplate`, `Foreground`, `HorizontalContentAlignment`, `Icon`, `IsShowOpenIndicator` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_DropdownIndicator` | template node (IconPresenter) | `BrowserButtonThemes.axaml` | DropdownButton | `Foreground`, `IsShowOpenIndicator`, `OpenIndicator` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LoadingIcon` | template node (LoadingOutlined) | `BrowserButtonThemes.axaml` | DropdownButton | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ButtonIcon` | template node (IconPresenter) | `BrowserButtonThemes.axaml` | DropdownButton | `Foreground`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `BrowserButtonThemes.axaml` | DropdownButton | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DropdownButton` | control theme | `DropdownButtonTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `CustomBackground` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `DropdownButtonTheme.axaml` | DropdownButton | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `CustomBackground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_WaveSpirit` | template node (WaveSpiritDecorator) | `DropdownButtonTheme.axaml` | DropdownButton | `EffectiveCornerRadius`, `IsMotionEnabled`, `IsWaveSpiritEnabled`, `WaveSpiritType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ShadowsFrame` | template node (Border) | `DropdownButtonTheme.axaml` | DropdownButton | `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (DashedBorder) | `DropdownButtonTheme.axaml` | DropdownButton | `Background`, `BackgroundSizing`, `BorderBrush`, `EffectiveBorderThickness`, `EffectiveCornerRadius`, `Height` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CustomBackgroundLayer` | template node (Border) | `DropdownButtonTheme.axaml` | DropdownButton | `CustomBackground`, `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RootLayout` | template node (DockPanel) | `DropdownButtonTheme.axaml` | DropdownButton | `Content`, `ContentTemplate`, `Foreground`, `HorizontalContentAlignment`, `Icon`, `IsShowOpenIndicator` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_DropdownIndicator` | template node (IconPresenter) | `DropdownButtonTheme.axaml` | DropdownButton | `Foreground`, `IsShowOpenIndicator`, `OpenIndicator` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ButtonIcon` | template node (IconPresenter) | `DropdownButtonTheme.axaml` | DropdownButton | `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `DropdownButtonTheme.axaml` | DropdownButton | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content`、`Icon`、`Command` | 继承 Button 的展示内容、图标和动作命令入口。 |
| 交互与状态 | `IsArrowVisible`、`IsPointAtCenter`、`IsShowOpenIndicator`、`ShouldUseOverlayPopup` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `MarginToAnchor`、`Placement`、`PlacementAnchor`、`PlacementGravity` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 弹层与窗口 | `DropdownFlyout` | 控制 popup、flyout、dialog、window 或 overlay 宿主协作。 |
| 动效与异步 | `MouseEnterDelay`、`MouseLeaveDelay` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `OpenIndicator`、`TriggerType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | SharedToken / 关联控件 Token + ControlTheme。 |

## State Flow

DropdownButton 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- open/close、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

DropdownButton 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `BrowserButtonThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `ButtonThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `DropdownButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |

DropdownButton 当前没有专属 Token 文档；主题通过 SharedToken、关联控件 Token 或继承主题资源表达视觉语义。运行时状态不得写入 Token 模型。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

- DropdownButton 当前没有专属 `token.md`；LLMS 生成按第 5 节视觉与主题模型、SharedToken、控件家族 Token 或主题资源说明 Token 边界。

## Customization Boundaries

维护 DropdownButton 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 DropdownButton 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
