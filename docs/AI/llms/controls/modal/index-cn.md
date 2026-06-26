# Modal

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

Modal 是 AtomUI 桌面控件体系中的模态对话框控件，用于阻断当前流程并承载确认、表单或复杂内容。

Modal 不负责轻量消息、通知卡片或普通浮出层。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Dialog`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Feedback/Modal` |
| 状态 | Stable |

## 何时使用

Modal 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Modal 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Modal 是 AtomUI 桌面控件体系中的模态对话框控件，用于阻断当前流程并承载确认、表单或复杂内容。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `AbortButtonText`、`AddOnTemplate`、`ApplyButtonText`、`CancelButtonText`、`CheckedIcon`、`CloseButtonText`、`Content`、`ContentTemplate` 等 33 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、open/close、loading/async、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Modal Token + ControlTheme。 |

## 公共 API

Modal 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `AbortButtonText`、`AddOnTemplate`、`ApplyButtonText`、`CancelButtonText`、`CheckedIcon`、`CloseButtonText`、`Content`、`ContentTemplate`、`DialogContent`、`DialogContentTemplate` 等 33 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `IsChecked` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsActivated`、`IsClosable`、`IsCloseButtonEnabled`、`IsConfirmLoading`、`IsDragMovable`、`IsEffectiveFooterVisible`、`IsFooterVisible`、`IsLoading`、`IsMaximizable`、`IsMaximizeButtonEnabled` 等 17 项 | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `HorizontalOffset`、`HorizontalStartupLocation`、`HostHeight`、`HostMaxHeight`、`HostMaxWidth`、`HostMinHeight`、`HostMinWidth`、`HostWidth`、`PlacementTarget`、`VerticalOffset` 等 11 项 | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 弹层与窗口 | `DialogHostType` | 控制 popup、flyout、dialog、window 或 overlay 宿主协作。 |
| 动效与异步 | `AnimationDuration` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `AddOn`、`DefaultStandardButton`、`EscapeStandardButton`、`Logo`、`Result`、`StandardButtons` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`Dialog`、`DialogActionResult`、`DialogBoxButtonSyncEventArgs`、`DialogButton`、`DialogButtonBox`、`DialogButtonClickedEventArgs`、`DialogCaptionButton`、`DialogFinishedEventArgs`、`DialogHost`、`DialogWindowContent`、`OverlayDialogHeader`、`OverlayDialogHost`、`OverlayDialogMask`、`OverlayDialogResizeEventArgs` 等 19 项。
- 枚举：`DialogButtonRole`、`DialogCode`、`DialogHorizontalAnchor`、`DialogHostType`、`DialogStandardButton`、`DialogVerticalAnchor`、`OverlayDialogState`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ButtonBox` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_CenterGroup` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_CloseButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_Header` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_LeftGroup` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_MaximizeButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_RightGroup` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_RootLayout` | `?` | 承载根视觉、边框、背景或尺寸基线。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

Modal 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。
- 类型：`Dialog`、`DialogActionResult`、`DialogBoxButtonSyncEventArgs`、`DialogButton`、`DialogButtonBox`、`DialogButtonClickedEventArgs`、`DialogCaptionButton`、`DialogFinishedEventArgs`、`DialogHost`、`DialogWindowContent`、`OverlayDialogHeader`、`OverlayDialogHost`、`OverlayDialogMask`、`OverlayDialogResizeEventArgs` 等 19 项。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Modal/Views/ModalShowCase.axaml:139`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Orientation="Horizontal" Spacing="10">
    <Panel>
        <atom:Button ButtonType="Primary" Name="BasicOpenModalButton" Content="打开浮层模态框" />
        <atom:Dialog PlacementTarget="BasicOpenModalButton"
                     IsOpen="{Binding IsBasicModalOpened, Mode=TwoWay}"
                     Title="基础模态框"
                     IsModal="False"
                     IsResizable="False"
                     IsDragMovable="True"
                     IsMaximizable="False"
                     StandardButtons="Cancel,Ok"
                     DefaultStandardButton="Ok"
                     HorizontalStartupLocation="Center"
                     VerticalOffset="30%"
                     HostMinWidth="300">
            <StackPanel Orientation="Vertical">
                <TextBlock Text="一些内容..." />
                <TextBlock Text="一些内容..." />
                <TextBlock Text="一些内容..." />
            </StackPanel>
        </atom:Dialog>
    </Panel>
    <Panel>
        <atom:Button ButtonType="Primary" Name="BasicWindowOpenModalButton" Content="打开窗口模态框" />
        <atom:Dialog PlacementTarget="BasicWindowOpenModalButton"
                     IsOpen="{Binding IsBasicWindowModalOpened, Mode=TwoWay}"
                     Title="基础窗口模态框"
                     IsModal="True"
                     IsResizable="True"
                     IsClosable="True"
                     IsDragMovable="True"
                     IsMaximizable="False"
                     DialogHostType="Window"
                     HorizontalStartupLocation="Center"
                     VerticalOffset="30%"
                     StandardButtons="Yes"
                     DefaultStandardButton="Yes"
                     HostMinWidth="300">
            <StackPanel Orientation="Vertical">
                <TextBlock Text="一些内容..." />
                <TextBlock Text="一些内容..." />
                <TextBlock Text="一些内容..." />
            </StackPanel>
        </atom:Dialog>
    </Panel>
</StackPanel>
```

### 异步关闭

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Modal/Views/ModalShowCase.axaml:195`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Horizontal" Spacing="10">
    <Panel>
        <atom:Button ButtonType="Primary" Name="AsyncDialogOpenModalButton" Content="打开带异步逻辑的模态框" />
        <atom:Dialog PlacementTarget="AsyncDialogOpenModalButton"
                     IsOpen="{Binding IsAsyncDialogOpened, Mode=TwoWay}"
                     Title="异步关闭模态框"
                     IsModal="True"
                     IsDragMovable="True"
                     IsResizable="False"
                     IsMaximizable="False"
                     StandardButtons="Ok, Cancel"
                     HorizontalStartupLocation="Center"
                     VerticalStartupLocation="Center"
                     DefaultStandardButton="Ok"
                     ButtonClicked="HandleAsyncDialogButtonClicked"
                     HostMinWidth="400">
            <StackPanel>
                <TextBlock Text="模态框内容" />
            </StackPanel>
        </atom:Dialog>
    </Panel>
</StackPanel>
```

### 加载状态

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Modal/Views/ModalShowCase.axaml:320`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Horizontal" Spacing="10">
    <Panel>
        <atom:Button ButtonType="Primary" Name="LoadingDialogOpenModalButton" Content="打开模态框" />
        <atom:Dialog PlacementTarget="LoadingDialogOpenModalButton"
                     IsOpen="{Binding IsLoadingMsgBoxOpened, Mode=TwoWay}"
                     Title="加载中模态框"
                     IsModal="True"
                     IsLoading="True"
                     IsDragMovable="True"
                     IsResizable="False"
                     IsMaximizable="False"
                     StandardButtons="Reload"
                     HorizontalStartupLocation="Center"
                     VerticalStartupLocation="Center"
                     DefaultStandardButton="Reload"
                     Opened="HandleLoadingDialogOpened"
                     ButtonClicked="HandleLoadingDialogButtonClicked"
                     HostMinWidth="400">
            <StackPanel>
                <TextBlock Text="一些内容..." />
                <TextBlock Text="一些内容..." />
                <TextBlock Text="一些内容..." />
            </StackPanel>
        </atom:Dialog>
    </Panel>
</StackPanel>
```

### 自定义页脚按钮

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Modal/Views/ModalShowCase.axaml:356`

Gallery key：`ExamplesContent` / item `4`

```axaml
<StackPanel Orientation="Horizontal" Spacing="10">
    <Panel>
        <atom:Button ButtonType="Primary" Name="CustomFooterDialogOpenButton" Content="打开模态框" />
        <atom:Dialog PlacementTarget="CustomFooterDialogOpenButton"
                     IsOpen="{Binding IsCustomFooterDialogOpened, Mode=TwoWay}"
                     Title="标题"
                     IsModal="True"
                     DialogHostType="Window"
                     StandardButtons="Ok, Cancel"
                     HorizontalStartupLocation="Center"
                     VerticalStartupLocation="Center"
                     DefaultStandardButton="Ok"
                     HostMinWidth="400">
            <atom:Dialog.CustomButtons>
                <atom:DialogButton Role="ActionRole" Content="自定义按钮" />
            </atom:Dialog.CustomButtons>
            <StackPanel Spacing="5">
                <TextBlock Text="一些内容..." />
                <TextBlock Text="一些内容..." />
                <TextBlock Text="一些内容..." />
                <TextBlock Text="一些内容..." />
                <TextBlock Text="一些内容..." />
            </StackPanel>
        </atom:Dialog>
    </Panel>

    <Panel>
        <atom:Button ButtonType="Primary" Name="CustomFooterMsgBoxOpenButton" Content="打开模态框" />
        <atom:MessageBox PlacementTarget="CustomFooterMsgBoxOpenButton"
                         IsOpen="{Binding IsCustomFooterMsgBoxOpened, Mode=TwoWay}"
                         Title="确认"
                         IsModal="True"
                         Style="Confirm"
                         HostMinWidth="400">
            <atom:MessageBox.CustomButtons>
                <atom:DialogButton Role="ActionRole" Content="自定义按钮" />
            </atom:MessageBox.CustomButtons>
            <StackPanel Spacing="5">
                <TextBlock Text="一些文本 ..." />
            </StackPanel>
        </atom:MessageBox>
    </Panel>
</StackPanel>
```

## 状态模型

Modal 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、open/close、loading/async、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Modal 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `DialogButtonBoxTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `DialogCaptionButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `DialogHostTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `DialogTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `DialogThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `DialogWindowContentTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `OverlayDialogHeaderTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `OverlayDialogHostTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `OverlayDialogMaskTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `OverlayDialogResizerTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |

Modal 使用 `DialogToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、open/close、loading/async、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Modal Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `DialogToken`，scope id 为 `Dialog`，源码位于 `src/AtomUI.Desktop.Controls/Dialog/DialogToken.cs`。

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

- `src/AtomUI.Desktop.Controls/Dialog`：16 个文件，代表文件 `Dialog.StaticAPI.cs`、`Dialog.cs`、`DialogActionResult.cs`、`DialogButtonCollectionUtils.cs`、`DialogFinishedEventArgs.cs` 等。
- `src/AtomUI.Desktop.Controls/Dialog/ButtonBox`：8 个文件，代表文件 `DialogBoxButtonSyncEventArgs.cs`、`DialogButton.cs`、`DialogButtonBox.cs`、`DialogButtonClickedEventArgs.cs`、`DialogButtonRole.cs` 等。
- `src/AtomUI.Desktop.Controls/Dialog/Converters`：1 个文件，代表文件 `OverlayDialogResizerVisibleConverter.cs`。
- `src/AtomUI.Desktop.Controls/Dialog/Localization`：3 个文件，代表文件 `en_US.cs`、`zh_CN.cs`、`zh_TW.cs`。
- `src/AtomUI.Desktop.Controls/Dialog/OverlayHost`：6 个文件，代表文件 `OverlayDialogHeader.cs`、`OverlayDialogHost.cs`、`OverlayDialogMask.cs`、`OverlayDialogResizeEventArgs.cs`、`OverlayDialogResizer.cs` 等。
- `src/AtomUI.Desktop.Controls/Dialog/Themes`：12 个文件，代表文件 `DialogButtonBoxTheme.axaml`、`DialogCaptionButtonTheme.axaml`、`DialogHostTheme.axaml`、`DialogHostTheme.cs`、`DialogTheme.axaml` 等。
- `src/AtomUI.Desktop.Controls/Dialog/WindowHost`：2 个文件，代表文件 `DialogHost.cs`、`DialogWindowContent.cs`。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/feedback/modal/overview.md`
- 实现文档：`docs/controls/desktop/feedback/modal/implementation.md`
- Token 文档：`docs/controls/desktop/feedback/modal/token.md`
- 变更记录：`docs/controls/desktop/feedback/modal/changelog.md`
- 语义结构：`./semantic-cn.md`
