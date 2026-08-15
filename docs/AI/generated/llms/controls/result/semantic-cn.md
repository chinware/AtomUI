# Result 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Result 的 Part 名称与公开 Result Semantic 结构保持一致：`root`、`icon`、`title`、`subTitle`、`extra`、`body`。
普通反馈图标和 403/404/500 异常图是同一 `icon` 职责的静态替代实现，不拆分为状态专属 Part。

### 1.1 `root`

| 字段 | 值 |
| --- | --- |
| Owner | `Result` |
| Part | `root` |
| Selector | Result 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `Result` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 结果页 owner |
| 职责 | 承载整体结果布局、状态、表面属性和 Semantic Style 作用域。 |
| 相关 API | 全部 Result public API，包含 `StrokeDashArray` 与继承的标准表面属性 |
| 相关 Token | ResultToken、SharedToken |
| 稳定性 | stable since 6.0 |

### 1.2 `icon`

| 字段 | 值 |
| --- | --- |
| Owner | `Result` |
| Part | `icon` |
| Selector | `.semantic-icon` |
| SelectorRoute | `/template/ .semantic-icon` |
| Style Type | `ResultIconStyle` |
| ContractType | `Control` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 普通状态图标区域与异常状态图像 |
| 职责 | 表达 Info、Success、Warning、Error 图标和 403、404、500 图像的替代呈现。 |
| 相关 API | `Status`、`Icon` |
| 相关 Token | `StatusIconSize`、`StatusImageMargin`、`ImageWidth`、`ImageHeight`、状态色 Token |
| 稳定性 | stable since 6.0 |

`icon` 同时标记 `PART_StatusIconPresenter` 和 `PART_ErrorCodeImage`。两个 target 始终存在，`Status` 只切换可见性；
Semantic Style 必须适用于两个实现。它适合定制 Margin、Opacity、Width、Height 和对齐。普通 presenter 内由默认状态或 `Icon`
提供的 Child、异常 SVG 的内部图元和 source 文本不属于公开 Part。

### 1.3 `title`

| 字段 | 值 |
| --- | --- |
| Owner | `Result` |
| Part | `title` |
| Selector | `.semantic-title` |
| SelectorRoute | `/template/ .semantic-title` |
| Style Type | `ResultTitleStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 主结果标题 presenter |
| 职责 | 展示 Header 内容并提供标题排版边界。 |
| 相关 API | `Header`、`HeaderTemplate`、`HeaderFontSize` |
| 相关 Token | `HeaderFontSize`、`HeaderMargin`、标题色与相对行高 Token |
| 稳定性 | stable since 6.0 |

`title` 始终存在，`Header=null` 时通过 `IsVisible=false` 隐藏。它适合定制 Foreground、FontSize、FontStyle、FontWeight、
LineHeight、Margin、Padding、Opacity、换行和对齐；HeaderTemplate 创建的用户子树不属于 Result Semantic Part。

### 1.4 `subTitle`

| 字段 | 值 |
| --- | --- |
| Owner | `Result` |
| Part | `subTitle` |
| Selector | `.semantic-sub-title` |
| SelectorRoute | `/template/ .semantic-sub-title` |
| Style Type | `ResultSubTitleStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 结果副标题 presenter |
| 职责 | 展示可选结果说明并提供副标题排版边界。 |
| 相关 API | `SubHeader`、`SubHeaderTemplate`、`SubHeaderFontSize` |
| 相关 Token | `SubHeaderFontSize`、描述色与相对行高 Token |
| 稳定性 | stable since 6.0 |

`subTitle` 始终存在，`SubHeader=null` 时隐藏。它适合定制 Foreground、FontSize、FontStyle、FontWeight、LineHeight、Margin、
Padding、Opacity、换行和对齐；SubHeaderTemplate 创建的用户子树不属于 Result Semantic Part。

### 1.5 `extra`

| 字段 | 值 |
| --- | --- |
| Owner | `Result` |
| Part | `extra` |
| Selector | `.semantic-extra` |
| SelectorRoute | `/template/ .semantic-extra` |
| Style Type | `ResultExtraStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 操作与辅助内容 presenter |
| 职责 | 承载 Extra 内容并提供操作区布局边界。 |
| 相关 API | `Extra`、`ExtraTemplate` |
| 相关 Token | `ExtraMargin` |
| 稳定性 | stable since 6.0 |

`extra` presenter 始终存在，横向拉伸到 Result 内容区宽度，并在区域内部居中排列 Extra 内容；`Extra=null` 时保持空内容。
它适合定制 Background、Padding、Margin、Opacity、对齐和 TextAlignment。调用方放入的 Button、Panel 或其他内容子树
拥有自己的主题契约，Result 不继续匹配其内部节点。

### 1.6 `body`

| 字段 | 值 |
| --- | --- |
| Owner | `Result` |
| Part | `body` |
| Selector | `.semantic-body` |
| SelectorRoute | `/template/ .semantic-body` |
| Style Type | `ResultBodyStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 详细结果内容 presenter |
| 职责 | 承载 Content 并提供正文背景、内边距和外边距边界。 |
| 相关 API | `Content`、`ContentTemplate` |
| 相关 Token | `ContentMargin`、`ContentPadding`、`ColorFillAlter` |
| 稳定性 | stable since 6.0 |

`body` 始终存在，`Content=null` 时隐藏。它适合定制 Background、Padding、Margin、Opacity、对齐和 ClipToBounds；
ContentTemplate 创建的用户子树不属于 Result Semantic Part。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Result/Themes/ResultTheme.axaml`

```xml
<DashedBorder Name="Frame">
    <StackPanel Name="RootLayout">
        <ContentPresenter Name="PART_StatusIconPresenter" />
        <Svg Name="PART_ErrorCodeImage" />
        <ContentPresenter Name="Header" />
        <ContentPresenter Name="SubHeader" />
        <ContentPresenter Name="ExtraContent" />
        <ContentPresenter Name="Content" />
    </StackPanel>
</DashedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Result
  -> Result (control theme, ResultTheme.axaml)
     -> DashedBorder#Frame (template-stable)
        -> StackPanel#RootLayout (template-stable)
           -> ContentPresenter#PART_StatusIconPresenter (template-stable)
           -> Svg#PART_ErrorCodeImage (template-stable)
           -> ContentPresenter#Header (internal-observable)
           -> ContentPresenter#SubHeader (internal-observable)
           -> ContentPresenter#ExtraContent (internal-observable)
           -> ContentPresenter#Content (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Result` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Result` | control theme | `ResultTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (DashedBorder) | `ResultTheme.axaml` | Result | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RootLayout` | template node (StackPanel) | `ResultTheme.axaml` | Result | `Content`, `ContentTemplate`, `Extra`, `ExtraTemplate`, `Header`, `HeaderFontSize` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_StatusIconPresenter` | template node (ContentPresenter) | `ResultTheme.axaml` | Result | `StatusIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ErrorCodeImage` | template node (Svg) | `ResultTheme.axaml` | Result | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Header` | template node (ContentPresenter) | `ResultTheme.axaml` | Result | `Header`, `HeaderFontSize`, `HeaderLineHeight`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SubHeader` | template node (ContentPresenter) | `ResultTheme.axaml` | Result | `SubHeader`, `SubHeaderFontSize`, `SubHeaderLineHeight`, `SubHeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ExtraContent` | template node (ContentPresenter) | `ResultTheme.axaml` | Result | `Extra`, `ExtraTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Content` | template node (ContentPresenter) | `ResultTheme.axaml` | Result | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Header`、`HeaderTemplate`、`SubHeader`、`SubHeaderTemplate`、`Extra`、`ExtraTemplate`、`Content`、`ContentTemplate` | 定义标题、副标题、操作区域和正文内容入口。 |
| 状态与图标 | `Status`、`Icon` | 选择普通反馈图标或 403/404/500 异常图，并允许普通状态使用自定义 `PathIcon`。 |
| 排版 | `HeaderFontSize`、`SubHeaderFontSize` | 覆盖标题与副标题字号；行高仍由控件根据相对行高计算。 |
| 根表面 | 继承的 `Background`、`BorderBrush`、`BorderThickness`、`CornerRadius`、`Padding` 与 `StrokeDashArray` | 由根 owner Style 控制 Result 的背景、边框、圆角、内边距和虚线节奏。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Result Token + ControlTheme。 |

## State Flow

Result 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- `Status` 是状态视觉的唯一 owner。Info、Success、Warning、Error 显示普通图标 presenter，403、404、500 显示异常 SVG。
- `Icon` 只替换四种普通反馈状态的图标内容，不改变 `icon` Semantic Part 的 target 身份或数量。
- `Header`、`SubHeader` 和 `Content` 的空值只改变对应 presenter 的可见性；`Extra` 为空时保留空 presenter。
- 模板重套用时重新获取两个图标 template part，并把当前状态、图标尺寸、画刷和文本行高回放到新模板。

## Theme and Token Boundaries

Result 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `ResultTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Result 使用 `ResultToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不删除或重命名 `root`、`icon`、`title`、`subTitle`、`extra`、`body`，不改变 selector class、ContractType 或 cardinality。
- `DashedBorder#Frame` 必须继续投影 Result 的标准根表面属性；该内部 frame 不成为独立 Part。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Result Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ResultToken`，scope id 为 `Result`，源码位于 `src/AtomUI.Desktop.Controls/Result/ResultToken.cs`。

## Customization Boundaries

维护 Result 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不破坏六个 Semantic Part 的名称、selector、ContractType、cardinality 和 marker 身份。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Result 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- `root`、`icon`、`title`、`subTitle`、`extra`、`body` 的名称、selector、ContractType、cardinality 和 marker 身份。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 `PART_StatusIconPresenter` 的 PropertyChanged 订阅释放路径与新 presenter 的重新订阅顺序。
- Light/Dark、Browser/Desktop 下的主题一致性，以及无 `SizeType` 的 Token 尺寸基线。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
