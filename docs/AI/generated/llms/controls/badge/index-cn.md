# Badge

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Badge 是 AtomUI 桌面控件体系中的徽标控件，用于在目标元素上展示数量、状态点和角标提示。

Badge 不负责通知系统、Tooltip 或复杂业务状态机。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Controls/Badge`
- `src/AtomUI.Desktop.Controls/Badge`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge` |
| 状态 | Stable |

## 何时使用

Badge 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Badge 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Badge 是 AtomUI 桌面控件体系中的徽标控件，用于在目标元素上展示数量、状态点和角标提示。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `Text`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Badge Token + ControlTheme。 |

## 公共 API

Badge 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Text` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `Count`、`IsAdornerMode`、`OverflowCount` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `BadgeIsVisible`、`IsMotionEnabled`、`IsZeroVisible`、`Status` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `BadgeColor`、`BadgeDotColor`、`DotColor`、`Offset`、`Placement`、`RibbonColor`、`Size` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 动效与异步 | `MotionDuration` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `DecoratedTarget` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`AbstractCountBadge`、`AbstractCountBadgeAdorner`、`AbstractDotBadge`、`AbstractDotBadgeAdorner`、`AbstractRibbonBadge`、`AbstractRibbonBadgeAdorner`、`BadgeZoomBadgeInMotion`、`BadgeZoomBadgeOutMotion`、`CountBadge`、`CountBadgeAdorner`、`DotBadge`、`DotBadgeAdorner`、`DotBadgeIndicator`、`RibbonBadge` 等 15 项。
- 枚举：`CountBadgeSize`、`DotBadgeStatus`、`RibbonBadgePlacement`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_LabelPart` | `TextBlock` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_MotionActor` | `BaseMotionActor` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

Badge 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### {gallery:BadgeShowCaseLangResource BasicTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge/Views/BadgeShowCase.axaml:35`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge Count="5">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="0" IsZeroVisible="True">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
</StackPanel>
```

### {gallery:BadgeShowCaseLangResource OverflowCountTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge/Views/BadgeShowCase.axaml:58`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge Count="99">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="100">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="99" OverflowCount="10">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="1000" OverflowCount="999">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
</StackPanel>
```

### {gallery:BadgeShowCaseLangResource OffsetTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge/Views/BadgeShowCase.axaml:93`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge Count="5" Offset="10, 10">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
</StackPanel>
```

### {gallery:BadgeShowCaseLangResource SizeTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge/Views/BadgeShowCase.axaml:109`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge Count="5">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="5" Size="Small">
        <Border Width="40"
                Height="40"
                Background="rgb(191,191,191)"
                CornerRadius="8" />
    </atom:CountBadge>
</StackPanel>
```

## 状态模型

Badge 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Badge 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `CountBadgeAdornerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DotBadgeAdornerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DotBadgeIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `RibbonBadgeAdornerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Badge 使用 `BadgeToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Badge Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `BadgeToken`，scope id 为 `Badge`，源码位于 `src/AtomUI.Desktop.Controls/Badge/BadgeToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

## 源码索引

主要源码文件：

- `src/AtomUI.Controls/Badge/AbstractCountBadge.cs`
- `src/AtomUI.Controls/Badge/AbstractCountBadgeAdorner.cs`
- `src/AtomUI.Controls/Badge/AbstractDotBadge.cs`
- `src/AtomUI.Controls/Badge/AbstractDotBadgeAdorner.cs`
- `src/AtomUI.Controls/Badge/AbstractRibbonBadge.cs`
- `src/AtomUI.Controls/Badge/AbstractRibbonBadgeAdorner.cs`
- `src/AtomUI.Controls/Badge/BadgeColorUtils.cs`
- `src/AtomUI.Controls/Badge/BadgeMotion.cs`
- `src/AtomUI.Controls/Badge/DotBadgeIndicator.cs`
- `src/AtomUI.Desktop.Controls/Badge/BadgeToken.cs`
- `src/AtomUI.Desktop.Controls/Badge/CountBadge.cs`
- `src/AtomUI.Desktop.Controls/Badge/CountBadgeAdorner.cs`
- `src/AtomUI.Desktop.Controls/Badge/DotBadge.cs`
- `src/AtomUI.Desktop.Controls/Badge/DotBadgeAdorner.cs`
- `src/AtomUI.Desktop.Controls/Badge/RibbonBadge.cs`
- `src/AtomUI.Desktop.Controls/Badge/RibbonBadgeAdorner.cs`
- `src/AtomUI.Desktop.Controls/Badge/Themes/CountBadgeAdornerTheme.axaml`
- `src/AtomUI.Desktop.Controls/Badge/Themes/DotBadgeAdornerTheme.axaml`
- `src/AtomUI.Desktop.Controls/Badge/Themes/DotBadgeIndicatorTheme.axaml`
- `src/AtomUI.Desktop.Controls/Badge/Themes/RibbonBadgeAdornerTheme.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/badge/overview.md`
- 实现文档：`docs/controls/desktop/data-display/badge/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/badge/token.md`
- 变更记录：`docs/controls/desktop/data-display/badge/changelog.md`
- 语义结构：`./semantic-cn.md`
