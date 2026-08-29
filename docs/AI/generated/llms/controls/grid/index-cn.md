# Grid

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Grid 是 AtomUI 桌面控件体系中的响应式栅格控件，用 Row、Col 和断点信息组织十二栅格布局。

Grid 不负责替代 Avalonia 原生 Grid 的全部布局能力，也不承担复杂数据表格职责。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Controls/Grid`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Layout/Grid` |
| 状态 | Stable |

## 何时使用

Grid 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Grid 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Grid 是 AtomUI 桌面控件体系中的响应式栅格控件，用 Row、Col 和断点信息组织十二栅格布局。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `Row` / `Col` 子元素、栅格断点信息和布局属性。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | 基础交互和主题状态。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | SharedToken / 关联控件 Token + ControlTheme。 |

## 公共 API

Grid 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Row`、`Col`、`Children` | 继承布局容器的子元素入口和栅格单元。 |
| 交互与状态 | `IsWrapped` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `Align`、`AlignInfo`、`Offset` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Flex`、`Gutter`、`Justify`、`JustifyInfo`、`Lg`、`Md`、`Order`、`Pull`、`Push`、`Sm` 等 15 项 | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`Col`、`ColInfoExtension`、`GridColFlexConverter`、`GridColSizeConverter`、`GridColSpanInfoConverter`、`GridGutterConverter`、`GridRowAlignInfoConverter`、`GridRowJustifyInfoConverter`、`Row`、`RowAlignConverter`、`RowJustifyConverter`。
- 枚举：`RowAlign`、`RowJustify`。

稳定 template part：

当前控件没有显式 `[TemplatePart]` 契约；主题节点仍通过 ControlTheme key、资源 key 和 Gallery 可观察行为形成稳定边界。

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

Grid 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础栅格

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/Grid/Views/GridShowCase.axaml:57`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Margin="20" Spacing="12">
    <atom:Row Gutter="16,16">
        <atom:Col Span="24">
            <Border Classes="grid-cell">
                <TextBlock Classes="grid-cell-text" Text="col" />
            </Border>
        </atom:Col>
    </atom:Row>
    <atom:Row Gutter="16,16">
        <atom:Col Span="12">
            <Border Classes="grid-cell">
                <TextBlock Classes="grid-cell-text" Text="列-12" />
            </Border>
        </atom:Col>
        <atom:Col Span="12">
            <Border Classes="grid-cell alt">
                <TextBlock Classes="grid-cell-text" Text="列-12" />
            </Border>
        </atom:Col>
    </atom:Row>
    <atom:Row Gutter="16,16">
        <atom:Col Span="8">
            <Border Classes="grid-cell">
                <TextBlock Classes="grid-cell-text" Text="列-8" />
            </Border>
        </atom:Col>
        <atom:Col Span="8">
            <Border Classes="grid-cell alt">
                <TextBlock Classes="grid-cell-text" Text="列-8" />
            </Border>
        </atom:Col>
        <atom:Col Span="8">
            <Border Classes="grid-cell">
                <TextBlock Classes="grid-cell-text" Text="列-8" />
            </Border>
        </atom:Col>
    </atom:Row>
    <atom:Row Gutter="16,16">
        <atom:Col Span="6">
            <Border Classes="grid-cell">
                <TextBlock Classes="grid-cell-text" Text="列-6" />
            </Border>
        </atom:Col>
        <atom:Col Span="6">
            <Border Classes="grid-cell alt">
                <TextBlock Classes="grid-cell-text" Text="列-6" />
            </Border>
        </atom:Col>
        <atom:Col Span="6">
            <Border Classes="grid-cell">
                <TextBlock Classes="grid-cell-text" Text="列-6" />
            </Border>
        </atom:Col>
        <atom:Col Span="6">
            <Border Classes="grid-cell alt">
                <TextBlock Classes="grid-cell-text" Text="列-6" />
            </Border>
        </atom:Col>
    </atom:Row>
</StackPanel>
```

### 偏移

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/Grid/Views/GridShowCase.axaml:248`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Margin="20" Spacing="12">
    <atom:Row Gutter="16,16">
        <atom:Col Span="8">
            <Border Classes="grid-cell">
                <TextBlock Classes="grid-cell-text" Text="列-8" />
            </Border>
        </atom:Col>
        <atom:Col Span="8" Offset="8">
            <Border Classes="grid-cell alt">
                <TextBlock Classes="grid-cell-text" Text="列-8 列-offset-8" />
            </Border>
        </atom:Col>
    </atom:Row>
    <atom:Row Gutter="16,16">
        <atom:Col Span="6" Offset="6">
            <Border Classes="grid-cell">
                <TextBlock Classes="grid-cell-text" Text="列-6 列-offset-6" />
            </Border>
        </atom:Col>
    </atom:Row>
    <atom:Row Gutter="16,16">
        <atom:Col Span="12" Offset="6">
            <Border Classes="grid-cell alt">
                <TextBlock Classes="grid-cell-text" Text="列-12 列-offset-6" />
            </Border>
        </atom:Col>
    </atom:Row>
    <atom:Row Gutter="16,16">
        <atom:Col Span="6" Offset="18">
            <Border Classes="grid-cell">
                <TextBlock Classes="grid-cell-text" Text="列-6 列-offset-18" />
            </Border>
        </atom:Col>
    </atom:Row>
</StackPanel>
```

### 推拉排序

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/Grid/Views/GridShowCase.axaml:290`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Margin="20" Spacing="12">
    <atom:Row Gutter="16,16">
        <atom:Col Span="18" Push="6">
            <Border Classes="grid-cell alt">
                <TextBlock Classes="grid-cell-text" Text="列-18 列-push-6" />
            </Border>
        </atom:Col>
        <atom:Col Span="6" Pull="18">
            <Border Classes="grid-cell">
                <TextBlock Classes="grid-cell-text" Text="列-6 列-pull-18" />
            </Border>
        </atom:Col>
    </atom:Row>
</StackPanel>
```

### 交叉轴对齐

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/Grid/Views/GridShowCase.axaml:475`

Gallery key：`ExamplesContent` / item `5`

```axaml
<StackPanel Margin="20" Spacing="12">
    <TextBlock Classes="grid-section-title" Text="Align Top" />
    <Border Classes="grid-row-surface">
        <atom:Row Gutter="16,16" Justify="center" Align="top">
            <atom:Col Span="4">
                <Border Classes="grid-cell" Height="72">
                    <TextBlock Classes="grid-cell-text" Text="列-4" />
                </Border>
            </atom:Col>
            <atom:Col Span="4">
                <Border Classes="grid-cell alt" Height="48">
                    <TextBlock Classes="grid-cell-text" Text="列-4" />
                </Border>
            </atom:Col>
            <atom:Col Span="4">
                <Border Classes="grid-cell" Height="64">
                    <TextBlock Classes="grid-cell-text" Text="列-4" />
                </Border>
            </atom:Col>
            <atom:Col Span="4">
                <Border Classes="grid-cell alt" Height="56">
                    <TextBlock Classes="grid-cell-text" Text="列-4" />
                </Border>
            </atom:Col>
        </atom:Row>
    </Border>

    <TextBlock Classes="grid-section-title" Text="Align Middle" />
    <Border Classes="grid-row-surface">
        <atom:Row Gutter="16,16" Justify="center" Align="middle">
            <atom:Col Span="4">
                <Border Classes="grid-cell" Height="72">
                    <TextBlock Classes="grid-cell-text" Text="列-4" />
                </Border>
            </atom:Col>
            <atom:Col Span="4">
                <Border Classes="grid-cell alt" Height="48">
                    <TextBlock Classes="grid-cell-text" Text="列-4" />
                </Border>
            </atom:Col>
            <atom:Col Span="4">
                <Border Classes="grid-cell" Height="64">
                    <TextBlock Classes="grid-cell-text" Text="列-4" />
                </Border>
            </atom:Col>
            <atom:Col Span="4">
                <Border Classes="grid-cell alt" Height="56">
                    <TextBlock Classes="grid-cell-text" Text="列-4" />
                </Border>
            </atom:Col>
        </atom:Row>
    </Border>

    <TextBlock Classes="grid-section-title" Text="Align Bottom" />
    <Border Classes="grid-row-surface">
        <atom:Row Gutter="16,16" Justify="center" Align="bottom">
            <atom:Col Span="4">
                <Border Classes="grid-cell" Height="72">
                    <TextBlock Classes="grid-cell-text" Text="列-4" />
                </Border>
            </atom:Col>
            <atom:Col Span="4">
                <Border Classes="grid-cell alt" Height="48">
                    <TextBlock Classes="grid-cell-text" Text="列-4" />
                </Border>
            </atom:Col>
            <atom:Col Span="4">
                <Border Classes="grid-cell" Height="64">
                    <TextBlock Classes="grid-cell-text" Text="列-4" />
                </Border>
            </atom:Col>
            <atom:Col Span="4">
                <Border Classes="grid-cell alt" Height="56">
                    <TextBlock Classes="grid-cell-text" Text="列-4" />
                </Border>
            </atom:Col>
        </atom:Row>
    </Border>
</StackPanel>
```

## 状态模型

Grid 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- 基础交互和主题状态 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Grid 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

当前控件未抽取到专属 AXAML 主题文件；视觉契约主要来自继承控件、共享主题和资源 key。

Grid 当前没有专属 Token 文档；主题通过 SharedToken、关联控件 Token 或继承主题资源表达视觉语义。运行时状态不得写入 Token 模型。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

- Grid 当前没有专属 `token.md`；LLMS 生成按第 5 节视觉与主题模型、SharedToken、控件家族 Token 或主题资源说明 Token 边界。

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

- `src/AtomUI.Controls/Grid/Col.cs`
- `src/AtomUI.Controls/Grid/ColInfo.cs`
- `src/AtomUI.Controls/Grid/ColInfoExtension.cs`
- `src/AtomUI.Controls/Grid/GridAlignment.cs`
- `src/AtomUI.Controls/Grid/GridColFlex.cs`
- `src/AtomUI.Controls/Grid/GridColSize.cs`
- `src/AtomUI.Controls/Grid/GridColSpanInfo.cs`
- `src/AtomUI.Controls/Grid/GridGutter.cs`
- `src/AtomUI.Controls/Grid/GridGutterInfo.cs`
- `src/AtomUI.Controls/Grid/GridRowResponsiveInfo.cs`
- `src/AtomUI.Controls/Grid/Row.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/layout/grid/overview.md`
- 实现文档：`docs/controls/desktop/layout/grid/implementation.md`
- 变更记录：`docs/controls/desktop/layout/grid/changelog.md`
- 语义结构：`./semantic-cn.md`
