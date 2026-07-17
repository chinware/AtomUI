# WindowTitleBar 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `WindowTitleBar` | 窗口控件根语义区域，承载窗口 public API、平台状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `chrome` | `窗口装饰区域` | 承载标题栏、caption buttons、drag region、边框和阴影。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `窗口内容区域` | 承载业务内容、系统交互和布局边界。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `state` | `窗口状态区域` | 表达最大化、最小化、激活、失焦、resize 和平台能力。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/WindowTitleBarTheme.axaml`

```xml
<Border Name="Frame">
    <DockPanel>
        <StackPanel>
            <ContentPresenter Name="PART_Logo" />
            <ContentPresenter Name="PART_LeftAddOn" />
            <ContentPresenter Name="PART_ContentPresenter" />
        </StackPanel>
        <DockPanel>
            <CaptionButtonGroup Name="PART_CaptionButtonGroup" />
            <ContentPresenter Name="PART_RightAddOn" />
        </DockPanel>
    </DockPanel>
</Border>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
WindowTitleBar
  -> CaptionButtonGroup (control theme, CaptionButtonGroupTheme.axaml)
     -> StackPanel#RootLayout (template-stable)
        -> CaptionButton#PART_FullScreenButton (template-stable)
        -> CaptionButton#PART_PinButton (template-stable)
        -> CaptionButton#PART_MinimizeButton (template-stable)
        -> CaptionButton#PART_MaximizeButton (template-stable)
        -> CaptionButton#PART_CloseButton (template-stable)
     -> StackPanel#RootLayout (template-stable)
        -> CaptionButton#PART_PinButton (template-stable)
     -> StackPanel#RootLayout (template-stable)
        -> WindowsCaptionButton#PART_FullScreenButton (template-stable)
        -> WindowsCaptionButton#PART_PinButton (template-stable)
        -> WindowsCaptionButton#PART_MinimizeButton (template-stable)
        -> WindowsCaptionButton#PART_MaximizeButton (template-stable)
        -> WindowsCaptionButton#PART_CloseButton (template-stable)
  -> CaptionButton (control theme, CaptionButtonTheme.axaml)
     -> Panel (template-stable)
        -> Border#PART_Frame (template-stable)
        -> Border (template-stable)
           -> IconPresenter#PART_IconPresenter (template-stable)
  -> WindowTitleBar (control theme, WindowTitleBarTheme.axaml)
     -> Border#Frame (template-stable)
        -> DockPanel (template-stable)
           -> StackPanel (template-stable)
              -> ContentPresenter#PART_Logo (template-stable)
              -> ContentPresenter#PART_LeftAddOn (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
           -> DockPanel (template-stable)
              -> CaptionButtonGroup#PART_CaptionButtonGroup (template-stable)
              -> ContentPresenter#PART_RightAddOn (template-stable)
     -> DockPanel (template-stable)
        -> CaptionButtonGroup#PART_CaptionButtonGroup (template-stable)
        -> Border#Frame (template-stable)
           -> DockPanel (template-stable)
              -> StackPanel (template-stable)
                 -> ContentPresenter#PART_Logo (template-stable)
                 -> ContentPresenter#PART_LeftAddOn (template-stable)
                 -> ContentPresenter#PART_ContentPresenter (template-stable)
              -> ContentPresenter#PART_RightAddOn (template-stable)
     -> Border#Frame (template-stable)
        -> DockPanel (template-stable)
           -> ContentPresenter#PART_LeftAddOn (template-stable)
           -> DockPanel (template-stable)
              -> CaptionButtonGroup#PART_CaptionButtonGroup (template-stable)
              -> ContentPresenter#PART_RightAddOn (template-stable)
           -> Panel (template-stable)
              -> StackPanel (template-stable)
                 -> ContentPresenter#PART_Logo (template-stable)
                 -> ContentPresenter#PART_ContentPresenter (template-stable)
  -> WindowsCaptionButton (control theme, WindowsCaptionButtonTheme.axaml)
     -> Border#PART_Frame (template-stable)
        -> IconPresenter#PART_IconPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `WindowTitleBar` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `CaptionButtonGroup` | control theme | `CaptionButtonGroupTheme.axaml` | WindowTitleBar | `IsCloseCaptionButtonVisible`, `IsFullScreenButtonEffectivelyVisible`, `IsFullScreenCaptionButtonVisible`, `IsMaximizeButtonEffectivelyVisible`, `IsMinimizeButtonEffectivelyVisible`, `IsMotionEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (StackPanel) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `IsCloseCaptionButtonVisible`, `IsFullScreenButtonEffectivelyVisible`, `IsFullScreenCaptionButtonVisible`, `IsMaximizeButtonEffectivelyVisible`, `IsMinimizeButtonEffectivelyVisible`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FullScreenButton` | template node (CaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `IsFullScreenButtonEffectivelyVisible`, `IsFullScreenCaptionButtonVisible`, `IsMotionEnabled`, `IsWindowActive`, `IsWindowFullScreen` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PinButton` | template node (CaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `IsMotionEnabled`, `IsPinButtonEffectivelyVisible`, `IsWindowActive`, `IsWindowPinned` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MinimizeButton` | template node (CaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `IsMinimizeButtonEffectivelyVisible`, `IsMotionEnabled`, `IsWindowActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MaximizeButton` | template node (CaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `IsMaximizeButtonEffectivelyVisible`, `IsMotionEnabled`, `IsWindowActive`, `IsWindowMaximized` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CloseButton` | template node (CaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `IsCloseCaptionButtonVisible`, `IsMotionEnabled`, `IsWindowActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FullScreenButton` | template node (WindowsCaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `IsFullScreenButtonEffectivelyVisible`, `IsFullScreenCaptionButtonVisible`, `IsMotionEnabled`, `IsWindowActive`, `IsWindowFullScreen` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PinButton` | template node (WindowsCaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `IsMotionEnabled`, `IsPinButtonEffectivelyVisible`, `IsWindowActive`, `IsWindowPinned` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MinimizeButton` | template node (WindowsCaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `IsMinimizeButtonEffectivelyVisible`, `IsMotionEnabled`, `IsWindowActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MaximizeButton` | template node (WindowsCaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `IsMaximizeButtonEffectivelyVisible`, `IsMotionEnabled`, `IsWindowActive`, `IsWindowMaximized` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CloseButton` | template node (WindowsCaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `IsCloseCaptionButtonVisible`, `IsMotionEnabled`, `IsWindowActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CaptionButton` | control theme | `CaptionButtonTheme.axaml` | WindowTitleBar | `Background`, `BackgroundInset`, `EffectiveCornerRadius`, `EffectiveIcon`, `HorizontalAlignment`, `IconHeight` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `CaptionButtonTheme.axaml` | CaptionButton | `Background`, `BackgroundInset`, `EffectiveCornerRadius`, `EffectiveIcon`, `HorizontalAlignment`, `IconHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Frame` | template node (Border) | `CaptionButtonTheme.axaml` | CaptionButton | `Background`, `BackgroundInset`, `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_IconPresenter` | template node (IconPresenter) | `CaptionButtonTheme.axaml` | CaptionButton | `EffectiveIcon`, `IconHeight`, `IconWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowTitleBar` | control theme | `WindowTitleBarTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `IsEffectiveLogoVisible`, `IsMotionEnabled`, `IsWindowActive`, `LeftAddOn`, `LeftAddOnTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `Background`, `IsEffectiveLogoVisible`, `IsMotionEnabled`, `IsWindowActive`, `LeftAddOn`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `IsEffectiveLogoVisible`, `IsMotionEnabled`, `IsWindowActive`, `LeftAddOn`, `LeftAddOnTemplate`, `Logo` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `IsEffectiveLogoVisible`, `LeftAddOn`, `LeftAddOnTemplate`, `Logo`, `LogoTemplate`, `Title` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Logo` | template node (ContentPresenter) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `IsEffectiveLogoVisible`, `Logo`, `LogoTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LeftAddOn` | template node (ContentPresenter) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `LeftAddOn`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `Title`, `TitleTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CaptionButtonGroup` | template node (CaptionButtonGroup) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `IsMotionEnabled`, `IsWindowActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RightAddOn` | template node (ContentPresenter) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `RightAddOn`, `RightAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `IsEffectiveLogoVisible`, `Logo`, `LogoTemplate`, `Title`, `TitleTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowsCaptionButton` | control theme | `WindowsCaptionButtonTheme.axaml` | WindowTitleBar | `Background`, `EffectiveCornerRadius`, `EffectiveIcon`, `IconHeight`, `IconWidth`, `Padding` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_Frame` | template node (Border) | `WindowsCaptionButtonTheme.axaml` | WindowsCaptionButton | `Background`, `EffectiveCornerRadius`, `EffectiveIcon`, `IconHeight`, `IconWidth`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_IconPresenter` | template node (IconPresenter) | `WindowsCaptionButtonTheme.axaml` | WindowsCaptionButton | `EffectiveIcon`, `IconHeight`, `IconWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CheckedIcon`、`IconHeight`、`IconWidth`、`LeftAddOnTemplate`、`LogoTemplate`、`NormalIcon`、`RightAddOnTemplate`、`Title`、`TitleTemplate` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `IsChecked`、`IsWindowActive` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsCloseCaptionButtonVisible`、`IsFullScreenCaptionButtonVisible`、`IsMaximizeCaptionButtonVisible`、`IsMinimizeCaptionButtonVisible`、`IsMotionEnabled`、`IsPinCaptionButtonVisible` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 其他稳定入口 | `LeftAddOn`、`Logo`、`LogoVisibility`、`OsType`、`OsVersion`、`RightAddOn` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、open/close、motion。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | WindowTitleBar Token + ControlTheme。 |

## State Flow

WindowTitleBar 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、open/close、motion 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

WindowTitleBar 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `CaptionButtonGroupTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `CaptionButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `WindowTitleBarTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `WindowTitleBarThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `WindowsCaptionButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |

WindowTitleBar 使用 `WindowTitleBarToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、open/close、motion 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

WindowTitleBar Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `WindowTitleBarToken`，scope id 为 `WindowTitleBar`，源码位于 `src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBarToken.cs`。

## Customization Boundaries

维护 WindowTitleBar 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 WindowTitleBar 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。
