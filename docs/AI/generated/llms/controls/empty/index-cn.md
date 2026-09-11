# Empty

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Empty 是 AtomUI 桌面控件体系中的空状态控件，用于表达无数据、无结果或占位状态。

Empty 不负责加载状态、错误状态或业务异常处理。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Controls/Empty`
- `src/AtomUI.Desktop.Controls/Empty`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Empty` |
| 状态 | Stable |

## 何时使用

Empty 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Empty 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Empty 是 AtomUI 桌面控件体系中的空状态控件，用于表达无数据、无结果或占位状态。 |
| 内容承载 | 用户数据、展示内容或操作入口如何进入控件。 | `Description`、`Footer`、`FooterTemplate`、`ImagePath`、`ImageSource`、`PresetImage`。 |
| 状态反馈 | public API 和模板绑定如何形成用户可感知反馈。 | 描述与 Footer 可见性、图片来源、三档 SizeType。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Empty Token + ControlTheme。 |

## 公共 API

Empty 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Description`、`Footer`、`FooterTemplate`、`ImagePath`、`ImageSource`、`IsDescriptionVisible`、`PresetImage` | 定义空状态图片、描述和后续操作内容。 |
| 视觉与布局 | `SizeType`、`StrokeDashArray` | `SizeType` 选择预设尺寸基线；`StrokeDashArray` 配置 root 边框的虚线节奏。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`AbstractEmpty`、`Empty`。
- 枚举：`PresetEmptyImage`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_SvgImage` | `Avalonia.Svg.Svg` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

Empty 公开 `root`、`image`、`description`、`footer` 四个 Semantic Part。完整 Selector、ContractType、cardinality 和定制边界见
[Empty Semantic Part 契约](semantic-part.md)。`Footer` 与 `FooterTemplate` 是 6.0 新增的公共内容入口，用于承载创建、刷新、
返回等空状态后续操作；`StrokeDashArray` 为 root 表面提供可绑定的虚线边框入口。Empty 保持
`TemplatedControl` 基类不变。

## 事件与命令

Empty 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Empty/Views/EmptyShowCase.axaml:75`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Empty PresetImage="Default" />
```

### 尺寸

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Empty/Views/EmptyShowCase.axaml:86`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical">
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Empty PresetImage="Simple" SizeType="Small" />
        <atom:Empty PresetImage="Simple" SizeType="Middle" />
        <atom:Empty PresetImage="Simple" SizeType="Large" />
    </StackPanel>
</StackPanel>
```

### 无描述

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Empty/Views/EmptyShowCase.axaml:121`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:Empty PresetImage="Default" IsDescriptionVisible="False" />
```

### 自定义语义结构的样式

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Empty/Views/EmptyShowCase.axaml:134`

SourceKey：`empty-semantic-part`

```axaml
<StackPanel Orientation="Vertical"
            HorizontalAlignment="Stretch"
            Spacing="16">
    <StackPanel.Styles>
        <Style Selector="atom|Empty.semantic-style-demo">
            <Setter Property="HorizontalAlignment" Value="Stretch" />
            <Setter Property="Padding" Value="16" />
            <Setter Property="BorderBrush" Value="#CCCCCC" />
            <Setter Property="BorderThickness" Value="1" />
            <Setter Property="StrokeDashArray" Value="4,2" />
        </Style>
        <Style Selector="atom|Empty.semantic-object-styles">
            <Setter Property="Background" Value="#F5F5F5" />
            <Setter Property="CornerRadius" Value="8" />
            <atom:EmptyDescriptionStyle x:SetterTargetType="atom:TextBlock">
                <Setter Property="Foreground" Value="#1890FF" />
                <Setter Property="FontWeight" Value="Bold" />
            </atom:EmptyDescriptionStyle>
            <atom:EmptyFooterStyle x:SetterTargetType="ContentPresenter">
                <Setter Property="Margin" Value="0,16,0,0" />
            </atom:EmptyFooterStyle>
        </Style>
        <Style Selector="atom|Empty.semantic-function-styles">
            <Setter Property="Background" Value="#E6F7FF" />
            <Setter Property="BorderBrush" Value="#91D5FF" />
            <Setter Property="StrokeDashArray" Value="{x:Null}" />
            <atom:EmptyDescriptionStyle x:SetterTargetType="atom:TextBlock">
                <Setter Property="Foreground" Value="#1890FF" />
                <Setter Property="FontWeight" Value="Bold" />
            </atom:EmptyDescriptionStyle>
        </Style>
    </StackPanel.Styles>

    <atom:Empty Classes="semantic-style-demo semantic-object-styles"
                PresetImage="Simple"
                SizeType="Small"
                Description="对象样式">
        <atom:Empty.Footer>
            <atom:Button ButtonType="Primary"
                         Content="Create Now" />
        </atom:Empty.Footer>
    </atom:Empty>

    <atom:Empty Classes="semantic-style-demo semantic-function-styles"
                PresetImage="Simple"
                SizeType="Small"
                Description="功能样式">
        <atom:Empty.Footer>
            <atom:Button ButtonType="Primary"
                         Content="Create Now" />
        </atom:Empty.Footer>
    </atom:Empty>
</StackPanel>
```

## 状态模型

Empty 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- `PresetImage`、`ImagePath`、`ImageSource` 三者互斥，并始终更新同一个 image 模板节点。
- `IsDescriptionVisible` 直接控制 description 模板节点的可见性，不通过 C# 动态创建或删除 Visual。
- `Footer=null` 时 footer Presenter 隐藏；非空时由 `FooterTemplate` 或 Avalonia DataTemplate 机制生成内容。
- Large、Middle、Small 只改变 image 高度和描述间距，不改变 Semantic marker 数量。
- 模板重套用时必须把 public API 对应状态回放到新的 part 和主题变量。

## 主题与 Design Token

Empty 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `EmptyTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Empty 使用 `EmptyToken` 作为控件 Token scope。Token 只表达图片高度、描述间距、Footer 间距和图形颜色等视觉语义，不承载
实例内容或可见性状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Empty Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `EmptyToken`，scope id 为 `Empty`，源码位于 `src/AtomUI.Desktop.Controls/Empty/EmptyToken.cs`。

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
- Footer 内容替换依赖 Avalonia ContentPresenter 的标准 logical/visual ownership，不额外缓存内容 Control。

## 源码索引

主要源码文件：

- `src/AtomUI.Controls/Empty/AbstractEmpty.cs`
- `src/AtomUI.Controls/Empty/BuiltInImageBuilder.cs`
- `src/AtomUI.Controls/Empty/PresetEmptyImage.cs`
- `src/AtomUI.Desktop.Controls/Empty/Empty.cs`
- `src/AtomUI.Desktop.Controls/Empty/Empty.SemanticParts.cs`
- `src/AtomUI.Desktop.Controls/Empty/EmptyToken.cs`
- `src/AtomUI.Desktop.Controls/Empty/Themes/EmptyTheme.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/empty/overview.md`
- 实现文档：`docs/controls/desktop/data-display/empty/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/data-display/empty/semantic-part.md`
- Token 文档：`docs/controls/desktop/data-display/empty/token.md`
- 变更记录：`docs/controls/desktop/data-display/empty/changelog.md`
- 语义结构：`./semantic-cn.md`
