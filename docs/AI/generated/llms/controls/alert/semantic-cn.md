# Alert 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Alert 只有一个 public descriptor owner：`Alert`。

| Part | Selector | Style Type | ContractType | Cardinality | AtomUI 节点 | 稳定性 |
| --- | --- | --- | --- | --- | --- | --- |
| `root` | Alert 本身 | 不适用 | `Alert` | `Single` | owner | stable since 6.0 |
| `icon` | `.semantic-icon` | `AlertIconStyle` | `Icon` | `Multiple` | 四个 `AlertType` 图标 | stable since 6.0 |
| `section` | `.semantic-section` | `AlertSectionStyle` | `StackPanel` | `Single` | 消息与描述布局 | stable since 6.0 |
| `title` | `.semantic-title` | `AlertTitleStyle` | `Control` | `Multiple` | `MessageLabel` 与 `MarqueeLabel` | stable since 6.0 |
| `description` | `.semantic-description` | `AlertDescriptionStyle` | `Label` | `Single` | `DescriptionLabel` | stable since 6.0 |
| `actions` | `.semantic-actions` | `AlertActionsStyle` | `ContentPresenter` | `Single` | `ExtraActionPresenter` | stable since 6.0 |
| `close` | `.semantic-close` | `AlertCloseStyle` | `IconButton` | `Single` | `PART_CloseBtn` | stable since 6.0 |

六个 selector Part 的 `SelectorRoute` 均为 `/template/ .semantic-<name>`，`Customization` 为 `Selector`，
`CrossVisualRoot=false`，`RuntimeCreated=false`。root 的 `Customization` 为 `Root`，不生成 `.semantic-root` 或 Style Type。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Alert/Themes/AlertTheme.axaml`

```xml
<PixelAlignedBorder>
    <DockPanel Name="RootLayout">
        <Panel>
            <CheckCircleFilled Name="SuccessIcon" />
            <InfoCircleFilled Name="InfoIcon" />
            <ExclamationCircleFilled Name="WarningIcon" />
            <CloseCircleFilled Name="ErrorIcon" />
        </Panel>
        <IconButton Name="PART_CloseBtn" />
        <ContentPresenter Name="ExtraActionPresenter" />
        <StackPanel>
            <Label Name="MessageLabel" />
            <MarqueeLabel Name="MarqueeLabel" />
            <Label Name="DescriptionLabel" />
        </StackPanel>
    </DockPanel>
</PixelAlignedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Alert
  -> Alert (control theme, AlertTheme.axaml)
     -> PixelAlignedBorder (template-stable)
        -> DockPanel#RootLayout (template-stable)
           -> Panel (template-stable)
              -> CheckCircleFilled#SuccessIcon (template-stable)
              -> InfoCircleFilled#InfoIcon (template-stable)
              -> ExclamationCircleFilled#WarningIcon (template-stable)
              -> CloseCircleFilled#ErrorIcon (template-stable)
           -> IconButton#PART_CloseBtn (template-stable)
           -> ContentPresenter#ExtraActionPresenter (internal-observable)
           -> StackPanel (template-stable)
              -> Label#MessageLabel (template-stable)
              -> MarqueeLabel#MarqueeLabel (template-stable)
              -> Label#DescriptionLabel (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Alert` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Alert` | control theme | `AlertTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `CloseIcon`, `CornerRadius`, `Description` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `RootLayout` | template node (DockPanel) | `AlertTheme.axaml` | Alert | `CloseIcon`, `Description`, `ExtraAction`, `IsClosable`, `IsMessageMarqueeEnabled`, `IsShowIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `AlertTheme.axaml` | Alert | `IsShowIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SuccessIcon` | template node (CheckCircleFilled) | `AlertTheme.axaml` | Alert | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `InfoIcon` | template node (InfoCircleFilled) | `AlertTheme.axaml` | Alert | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WarningIcon` | template node (ExclamationCircleFilled) | `AlertTheme.axaml` | Alert | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ErrorIcon` | template node (CloseCircleFilled) | `AlertTheme.axaml` | Alert | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CloseBtn` | template node (IconButton) | `AlertTheme.axaml` | Alert | `CloseIcon`, `IsClosable` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ExtraActionPresenter` | template node (ContentPresenter) | `AlertTheme.axaml` | Alert | `ExtraAction` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `StackPanel` | template node (StackPanel) | `AlertTheme.axaml` | Alert | `Description`, `IsMessageMarqueeEnabled`, `Message` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `MessageLabel` | template node (Label) | `AlertTheme.axaml` | Alert | `Message` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `MarqueeLabel` | template node (MarqueeLabel) | `AlertTheme.axaml` | Alert | `IsMessageMarqueeEnabled`, `Message` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DescriptionLabel` | template node (Label) | `AlertTheme.axaml` | Alert | `Description` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Message`、`Description`、`ExtraAction`、`CloseIcon` | 定义标题、详情、辅助操作和关闭图标内容。 |
| 交互与状态 | `Type`、`IsShowIcon`、`IsClosable`、`IsMessageMarqueeEnabled` | 表达反馈类型、图标、关闭入口和标题呈现状态。 |
| 视觉表面 | `StrokeDashArray` 与继承的 TemplatedControl 表面属性 | 支持 root Semantic Style 定制实线或虚线边框、背景、圆角与 padding。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | 基础交互和主题状态。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Alert Token + ControlTheme。 |

## State Flow

Alert 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- `Type` 决定背景、边框和当前可见的状态图标。
- `Description`、`ExtraAction`、`IsShowIcon`、`IsClosable` 与 `IsMessageMarqueeEnabled` 只切换静态模板节点状态，不改变 Semantic Part 数量。
- `CloseRequest` 由当前模板的 close button 触发，业务层决定是否隐藏、移除或替换 Alert。
- 模板重套用时必须解除旧 close button 订阅，并把 public API 对应状态投影到新模板。

## Theme and Token Boundaries

Alert 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AlertTheme.axaml` | 提供单一静态模板、状态 selector、Token 绑定和 Semantic marker。 |

Alert 使用 `AlertToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 基础交互和主题状态 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不删除或重命名 `root`、`icon`、`section`、`title`、`description`、`actions`、`close`，不改变 selector class、ContractType 或 cardinality。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Alert Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `AlertToken`，scope id 为 `Alert`，源码位于 `src/AtomUI.Desktop.Controls/Alert/AlertToken.cs`。

## Customization Boundaries

维护 Alert 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不破坏七个 Semantic Part 的名称、selector、ContractType、cardinality 和 marker 身份。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Alert 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- `root`、`icon`、`section`、`title`、`description`、`actions`、`close` 的名称、selector、ContractType、cardinality 和 marker 身份。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 `PART_CloseBtn` 的 Click 订阅释放路径与新 part 的重新订阅顺序。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
