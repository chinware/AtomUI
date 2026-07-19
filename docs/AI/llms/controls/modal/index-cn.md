# Modal

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

`Dialog` 用于需要独立内容表面、明确结果和受控关闭流程的桌面对话。它支持 Overlay 与原生 Window 两种宿主、modal 与 modeless 两种交互模式、声明式 `IsOpen` 和静态异步 API。

`MessageBox` 是 `Dialog` 的语义专化，用于信息、成功、警告、错误和确认消息。它复用 Dialog 的 Session、Presenter、关闭、尺寸、资源和焦点模型，不维护第二套隐藏 Dialog。

Modal 不承担通知队列、轻量 Tooltip、Popup 菜单或业务级导航服务职责。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Feedback/Modal` |
| 状态 | Stable |

## 何时使用

- 对话表面由标题、内容、Footer 和操作按钮组成，Overlay 与 Window 共享同一个内容模型。
- modal 通过 mask 或原生 owner 关系阻断底层输入；modeless 保持底层可交互。
- 打开与关闭 motion 属于 Session 生命周期的一部分。异步打开在入场完成后触发 `Opened`，异步关闭在退出 motion、宿主移除和资源释放完成后结束。
- `MessageBoxStyle` 只表达消息语义和默认图标/按钮策略，不改变 Dialog 生命周期。

## 公共 API

### 3.1 Dialog 契约组

| 契约组 | 代表成员 | 语义 |
| --- | --- | --- |
| 内容 | `Title`, `TitleIcon`, `Content`, `ContentTemplate`, `DataContext` | 定义标题和任意内容对象或模板。 |
| 打开状态 | `IsOpen`, `OpenAsync(...)` | `IsOpen` 是默认 TwoWay 的声明式意图；`OpenAsync` 表示一次完整 Session。 |
| 展示方式 | `DialogHostType`, `IsModal`, `PlacementTarget`, startup anchor/offset | 选择 Overlay/Window、交互模态和初始位置。 |
| 尺寸与窗口能力 | `HostWidth/Height/Min/Max`, `IsResizable`, `IsClosable`, `IsDragMovable`, `IsMaximizable`, `IsMinimizable`, `IsTopmost` | 同一组请求映射到 Overlay Surface 或原生 Window。`NaN` 表示自然尺寸。 |
| 操作 | `StandardButtons`, `CustomButtons`, `DefaultStandardButton`, `EscapeStandardButton`, `ButtonsConfigure` | 生成标准按钮、加入自定义按钮并配置当前有效按钮序列。 |
| 状态与策略 | `IsLoading`, `IsConfirmLoading`, `IsFooterVisible`, `IsMotionEnabled`, `BeforeCloseAsync` | 控制加载、确认按钮 loading、Footer、motion 和关闭前校验。 |
| 结果 | `Result`, `Accept()`, `Reject()`, `Done(...)` | 所有关闭来源归一为结果与 `DialogCloseReason`。 |

静态入口只有异步形式：

- `ShowDialogAsync(...)` 创建 modeless Dialog。
- `ShowDialogModalAsync(...)` 创建 modal Dialog。
- 两者都在完整 teardown 后返回 `Task<object?>`。

同一个实例在 `Opening`、`Open`、`ClosePending` 或 `Closing` 时再次调用 `OpenAsync` 会抛出 `InvalidOperationException`。声明式 `IsOpen=true` 可以在旧 Session 完整关闭后创建新 Session。

### 3.2 关闭事件

Dialog 公开 `Opened`、`Closing`、`Accepted`、`Rejected`、`Finished`、`Closed` 和 `ButtonClicked`。

正常关闭顺序：

```text
按钮来源时 ButtonClicked
  -> Closing
  -> BeforeCloseAsync
  -> 提交 IsOpen=false 与 Result
  -> Accepted 或 Rejected
  -> Finished
  -> 退出 motion 与 presenter teardown
  -> IDialogAwareDataContext.NotifyClosed
  -> Closed
```

`ButtonClicked.Handled=true`、`Closing.Cancel=true` 或 `BeforeCloseAsync` 返回 `false` 会取消本次普通关闭。owner close、placement target detach、外部取消和 presenter 打开失败属于强制 teardown；它们忽略业务 veto 和 confirm loading，但仍执行完整释放。

事件处理器或 presenter 在结果提交后抛出的首个异常，只会在 teardown 完成后传播。

### 3.3 MessageBox 契约

`MessageBox : Dialog` 增加 `Style`、`Icon`、`OkButtonStyle`、`OkButtonText`、`CancelButtonText`、`IsCenterOnStartup`、`Confirmed`、`Cancelled`、`Confirm()` 和 `Cancel()`。

`ShowMessageBoxAsync(...)` 与 `ShowMessageBoxModalAsync(...)` 复用继承的异步 Session 语义。未显式指定 `MessageBoxOptions.MinWidth` 时保留 MessageBox Token 的默认最小宽度。

### 3.4 Template Parts

| Part | 所属类型 | 职责 |
| --- | --- | --- |
| `PART_Header` | `DialogSurface` | Overlay 标题栏、拖动与 caption 操作。 |
| `PART_ButtonBox` | `DialogSurface` | 当前标准/自定义按钮序列。 |
| `PART_Resizer` | `DialogSurface` | Overlay resize handles。 |
| `PART_LeftGroup` / `PART_CenterGroup` / `PART_RightGroup` | `DialogButtonBox` | 按按钮角色布局。 |
| `PART_MaskMotionActor` | `OverlayDialogPresenter` | modal mask 及其 motion。 |
| `PART_SurfaceMotionActor` | `OverlayDialogPresenter` | DialogSurface 入场/退出 motion。 |

当前没有 Modal 专属 pseudo class。

## 事件与命令

### 3.2 关闭事件
事件处理器或 presenter 在结果提交后抛出的首个异常，只会在 teardown 完成后传播。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Modal/Views/ModalShowCase.axaml:35`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Orientation="Horizontal" Spacing="10" Loaded="HandleDialogExampleLoaded">
    <Panel>
        <atom:Button ButtonType="Primary" Name="BasicOpenModalButton" Content="打开浮层模态框" />
        <atom:Dialog Name="BasicDialog"
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
        <atom:Dialog Name="BasicWindowDialog"
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

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Modal/Views/ModalShowCase.axaml:91`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Horizontal" Spacing="10" Loaded="HandleDialogExampleLoaded">
    <Panel>
        <atom:Button ButtonType="Primary" Name="AsyncDialogOpenModalButton" Content="打开带异步逻辑的模态框" />
        <atom:Dialog Name="AsyncDialog"
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

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Modal/Views/ModalShowCase.axaml:216`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Horizontal" Spacing="10" Loaded="HandleDialogExampleLoaded">
    <Panel>
        <atom:Button ButtonType="Primary" Name="LoadingDialogOpenModalButton" Content="打开模态框" />
        <atom:Dialog Name="LoadingDialog"
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

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Modal/Views/ModalShowCase.axaml:252`

Gallery key：`ExamplesContent` / item `4`

```axaml
<StackPanel Orientation="Horizontal" Spacing="10" Loaded="HandleDialogExampleLoaded">
    <Panel>
        <atom:Button ButtonType="Primary" Name="CustomFooterDialogOpenButton" Content="打开模态框" />
        <atom:Dialog Name="CustomFooterDialog"
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
        <atom:MessageBox Name="CustomFooterMsgBox"
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

- `IsOpen` 表示最新声明式意图；`DialogSession` 表示一次实际展示。两者不能由 presenter 或 template part 反向拥有。
- modal Overlay 的真实 pointer 输入命中 mask，modeless Overlay 在 Surface 外穿透到底层。只有栈顶 presenter 响应 mask 与 Escape。
- Enter/Escape 根据当前有效按钮序列查找 default/escape 按钮，运行时修改标准按钮或自定义按钮会立即生效。
- `IsConfirmLoading=true` 只阻止用户发起的普通关闭，不阻止 owner close、detach、取消和失败 teardown。
- 打开后焦点进入 DialogSurface；嵌套 Dialog 关闭时恢复下层 Surface，最后一层关闭时恢复原触发控件。
- Overlay 与 Window 都等待 opening/closing motion；`IsMotionEnabled=false` 跳过 motion，但不跳过宿主打开、关闭和释放。

## 主题与 Design Token

| 主题文件 | 职责 |
| --- | --- |
| `DialogTheme.axaml` | Dialog 默认属性和 Token 映射。 |
| `DialogSurfaceTheme.axaml` | 共享标题、内容、Footer、按钮与 resize 结构。 |
| `OverlayDialogPresenterTheme.axaml` | 同一 presenter 内组合 mask 与 Surface motion。 |
| `DialogButtonBoxTheme.axaml` | 三组按钮布局。 |
| `OverlayDialogHeaderTheme.axaml` | Overlay 标题栏。 |
| `OverlayDialogMaskTheme.axaml` | modal mask。 |
| `OverlayDialogResizerTheme.axaml` | Overlay resize handles。 |
| `MessageBoxTheme.axaml` / `MessageBoxContentTheme.axaml` | MessageBox 默认尺寸和语义内容。 |

`DialogToken` 提供背景、文字、间距、尺寸和 Footer 视觉。motion duration 使用 Dialog scope 的 SharedToken `MotionDurationMid`，不在代码中硬编码。

Token 来源：

Modal Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `DialogToken`，scope id 为 `Dialog`，源码位于 `src/AtomUI.Desktop.Controls/Dialog/DialogToken.cs`。

## AOT 与裁剪注意事项

- Overlay presenter 在 Dialog 已附加时以 Dialog 为 inheritance parent，否则以 placement target 为 parent。
- Window 在 `Show()` 后把 DialogSurface inheritance parent 指向已附加 Dialog 或 owner，避开 TopLevel 的全局 styling parent；dispose 前清空。
- runtime binding 只用于动态 presenter/Surface/按钮关系，并由 owning presenter、Surface 或 ButtonBox 对称释放。
- 不使用反射修改 TemplatedParent，不扫描程序集发现 Dialog API，不使用同步 DispatcherFrame。
- Session、Presenter、Surface 和 Content 的关闭回收由 Overlay/Window WeakReference 测试覆盖。
- 状态机、按钮表和 presenter 选择都是静态类型路径，保持 NativeAOT 友好。

## 源码索引

| 路径 | 职责 |
| --- | --- |
| `Dialog.cs` | public 属性、事件、内容、按钮和内部协作入口。 |
| `Dialog.Lifecycle.cs` | `IsOpen` reconcile、`OpenAsync`、事件通知和 presenter 选择。 |
| `Dialog.StaticAPI.cs` | 静态 modeless/modal 异步创建入口。 |
| `DialogSession.cs` | 单次展示状态机、关闭仲裁、取消、结果、焦点和 teardown。 |
| `IDialogPresenter.cs` | Overlay/Window 共用的最小异步协议。 |
| `DialogSurface.cs` | 标题、内容、Footer、按钮和 Overlay resize 的共享表面。 |
| `ButtonBox/DialogButtonBox.cs` | 标准按钮生成、唯一有效按钮序列和自定义集合同步。 |
| `OverlayHost/DialogOverlayLayer.cs` | owner scope 内的 presenter stack。 |
| `OverlayHost/OverlayDialogPresenter.cs` | 同时拥有 mask、Surface、placement、drag/resize 和 motion。 |
| `WindowHost/WindowDialogPresenter.cs` | 原生 Window 属性映射、modal owner、尺寸、位置和 motion。 |
| `WindowHost/DialogWindow.cs` | 原生 caption close 仲裁和显式尺寸应用。 |
| `MessageBox/MessageBox.cs` | Dialog 派生的消息语义、静态 API 和按钮配置。 |
| `MessageBox/MessageBoxContent.cs` | MessageBox 的图标与内容组合。 |
| `Dialog/Themes` / `MessageBox/Themes` | 共享 Surface、Overlay presenter 和 MessageBox AXAML 结构。 |

对应回归测试位于 `tests/AtomUI.Desktop.Controls.Tests/Dialog` 和 `tests/AtomUI.Desktop.Controls.Tests/MessageBox`。

## 相关文档

- 源设计文档：`docs/controls/desktop/feedback/modal/overview.md`
- 实现文档：`docs/controls/desktop/feedback/modal/implementation.md`
- Token 文档：`docs/controls/desktop/feedback/modal/token.md`
- 变更记录：`docs/controls/desktop/feedback/modal/changelog.md`
- 语义结构：`./semantic-cn.md`
