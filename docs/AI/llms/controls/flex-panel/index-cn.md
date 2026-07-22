# FlexPanel

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

FlexPanel 是 AtomUI 桌面控件体系中的弹性布局面板，用于按主轴、换行、对齐和 flex 参数排列子元素。

FlexPanel 不负责响应式断点系统、数据列表虚拟化或 Grid 栅格模型。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Controls/FlexPanel`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel` |
| 状态 | Stable |

## 何时使用

FlexPanel 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | FlexPanel 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | FlexPanel 是 AtomUI 桌面控件体系中的弹性布局面板，用于按主轴、换行、对齐和 flex 参数排列子元素。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `AlignContent`、`AlignItems`、`JustifyContent`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | collection/filter。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | SharedToken / 关联控件 Token + ControlTheme。 |

## 公共 API

FlexPanel 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `AlignContent`、`AlignItems`、`JustifyContent` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 视觉与布局 | `ColumnSpacing`、`RowSpacing` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Direction`、`Wrap` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：无。
- 枚举：`AlignContent`、`AlignItems`、`FlexBasisKind`、`FlexDirection`、`FlexWrap`、`JustifyContent`。

稳定 template part：

当前控件没有显式 `[TemplatePart]` 契约；主题节点仍通过 ControlTheme key、资源 key 和 Gallery 可观察行为形成稳定边界。

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

FlexPanel 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础布局

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelShowCase.axaml:33`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Margin="20" Spacing="16" AttachedToVisualTree="InitializeBasicExample">
    <StackPanel Orientation="Horizontal" Spacing="20">
        <atom:RadioButton x:Name="DirectionHorizontal" IsChecked="True" Content="horizontal" />
        <atom:RadioButton x:Name="DirectionVertical" Content="vertical" />
    </StackPanel>

    <atom:PixelAlignedBorder BorderBrush="#E5E5E5" BorderThickness="1" CornerRadius="6" Padding="12"
                             HorizontalAlignment="Stretch">
        <atom:FlexPanel x:Name="BasicFlexPanel"
                        Direction="Row"
                        AlignItems="Stretch"
                        HorizontalAlignment="Stretch">
            <Border x:Name="BasicItem1" Background="#4F7CF5" Height="60" MinWidth="80" />
            <Border x:Name="BasicItem2" Background="#1F5BFF" Height="60" MinWidth="80" />
            <Border x:Name="BasicItem3" Background="#4F7CF5" Height="60" MinWidth="80" />
            <Border x:Name="BasicItem4" Background="#1F5BFF" Height="60" MinWidth="80" />
        </atom:FlexPanel>
    </atom:PixelAlignedBorder>
</StackPanel>
```

### 对齐

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelShowCase.axaml:59`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Margin="20" Spacing="12" AttachedToVisualTree="InitializeAlignmentExample">
    <atom:TextBlock FontWeight="Bold" Text="选择 justify：" />
    <atom:Segmented x:Name="JustifySegmented">
        <atom:SegmentedItem IsSelected="True" Content="flex-start" />
        <atom:SegmentedItem Content="center" />
        <atom:SegmentedItem Content="flex-end" />
        <atom:SegmentedItem Content="space-between" />
        <atom:SegmentedItem Content="space-around" />
        <atom:SegmentedItem Content="space-evenly" />
    </atom:Segmented>

    <atom:TextBlock FontWeight="Bold" Text="选择 align：" />
    <atom:Segmented x:Name="AlignSegmented">
        <atom:SegmentedItem Content="flex-start" />
        <atom:SegmentedItem Content="center" />
        <atom:SegmentedItem IsSelected="True" Content="flex-end" />
        <atom:SegmentedItem Content="stretch" />
    </atom:Segmented>

    <atom:PixelAlignedBorder BorderBrush="#E5E5E5" BorderThickness="1" CornerRadius="6"  Height="120">
        <atom:FlexPanel x:Name="AlignFlexPanel"
                        Direction="Row"
                        JustifyContent="FlexStart"
                        AlignItems="FlexEnd">
            <atom:Button ButtonType="Primary" Content="主要" />
            <atom:Button ButtonType="Primary" Content="主要" />
            <atom:Button ButtonType="Primary" Content="主要" />
            <atom:Button ButtonType="Primary" Content="主要" />
        </atom:FlexPanel>
    </atom:PixelAlignedBorder>
</StackPanel>
```

### 间距

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelShowCase.axaml:97`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Margin="20" Spacing="16" AttachedToVisualTree="InitializeGapExample">
    <StackPanel Orientation="Horizontal" Spacing="20">
        <atom:RadioButton x:Name="GapSmall" IsChecked="True" GroupName="GapMode" Content="small" />
        <atom:RadioButton x:Name="GapMiddle" GroupName="GapMode" Content="middle" />
        <atom:RadioButton x:Name="GapLarge" GroupName="GapMode" Content="large" />
        <atom:RadioButton x:Name="GapCustomize" GroupName="GapMode" Content="customize" />
    </StackPanel>

    <StackPanel x:Name="GapCustomPanel" Orientation="Horizontal" Spacing="8" IsVisible="False">
        <atom:TextBlock VerticalAlignment="Center" Text="自定义间距：" />
        <Slider x:Name="GapValueSlider"
                Width="200"
                Minimum="0"
                Maximum="100"
                TickFrequency="4"
                Value="12" />
    </StackPanel>

    <atom:FlexPanel x:Name="GapFlexPanel" Direction="Row" ColumnSpacing="8" RowSpacing="8">
        <atom:Button ButtonType="Primary" Content="主要" />
        <atom:Button ButtonType="Default" Content="默认" />
        <atom:Button ButtonType="Dashed" Content="虚线" />
        <atom:Button ButtonType="Link" Content="链接" />
    </atom:FlexPanel>
</StackPanel>
```

### 自动换行

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelShowCase.axaml:129`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Margin="20" Spacing="16" AttachedToVisualTree="InitializeAutoWrapExample">
    <StackPanel Orientation="Horizontal" Spacing="20">
        <atom:RadioButton x:Name="WrapEnabled" IsChecked="True" GroupName="WrapMode" Content="wrap" />
        <atom:RadioButton x:Name="WrapDisabled" GroupName="WrapMode" Content="nowrap" />
    </StackPanel>
    <atom:PixelAlignedBorder BorderBrush="#E5E5E5" BorderThickness="1" CornerRadius="6" Padding="12">
        <atom:ScrollViewer x:Name="WrapScrollViewer"
                           HorizontalScrollBarVisibility="Disabled"
                           VerticalScrollBarVisibility="Disabled">
            <atom:FlexPanel x:Name="WrapFlexPanel"
                            Direction="Row"
                            Wrap="Wrap"
                            ColumnSpacing="8"
                            RowSpacing="8"
                            JustifyContent="FlexStart">
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
                <atom:Button ButtonType="Primary" Content="按钮" />
            </atom:FlexPanel>
        </atom:ScrollViewer>
    </atom:PixelAlignedBorder>
</StackPanel>
```

## 状态模型

FlexPanel 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- collection/filter 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

FlexPanel 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

当前控件未抽取到专属 AXAML 主题文件；视觉契约主要来自继承控件、共享主题和资源 key。

FlexPanel 当前没有专属 Token 文档；主题通过 SharedToken、关联控件 Token 或继承主题资源表达视觉语义。运行时状态不得写入 Token 模型。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

- FlexPanel 当前没有专属 `token.md`；LLMS 生成按第 5 节视觉与主题模型、SharedToken、控件家族 Token 或主题资源说明 Token 边界。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
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

- `src/AtomUI.Controls/FlexPanel/AlignContent.cs`
- `src/AtomUI.Controls/FlexPanel/AlignItems.cs`
- `src/AtomUI.Controls/FlexPanel/Flex.cs`
- `src/AtomUI.Controls/FlexPanel/FlexBasis.cs`
- `src/AtomUI.Controls/FlexPanel/FlexBasisKind.cs`
- `src/AtomUI.Controls/FlexPanel/FlexDirection.cs`
- `src/AtomUI.Controls/FlexPanel/FlexPanel.cs`
- `src/AtomUI.Controls/FlexPanel/FlexWrap.cs`
- `src/AtomUI.Controls/FlexPanel/JustifyContent.cs`
- `src/AtomUI.Controls/FlexPanel/Uv.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/layout/flex-panel/overview.md`
- 实现文档：`docs/controls/desktop/layout/flex-panel/implementation.md`
- 变更记录：`docs/controls/desktop/layout/flex-panel/changelog.md`
- 语义结构：`./semantic-cn.md`
