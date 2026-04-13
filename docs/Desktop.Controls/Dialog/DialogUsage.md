# Dialog 使用文档

本文档介绍 AtomUI Dialog 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ModalShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Dialog，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Dialog, DialogOptions, DialogButton 等
```

---

## 1. 基础模态对话框

### Overlay 模式（默认）

Dialog 声明在一个 `Panel` 中，通过 `IsOpen` 属性控制显示：

```xml
<Panel>
    <atom:Button ButtonType="Primary" Name="OpenModalButton">
        Open Modal Overlay
    </atom:Button>
    <atom:Dialog PlacementTarget="OpenModalButton"
                 IsOpen="{Binding IsModalOpened, Mode=TwoWay}"
                 Title="Basic Modal"
                 IsModal="True"
                 IsDragMovable="True"
                 StandardButtons="Cancel,Ok"
                 DefaultStandardButton="Ok"
                 HorizontalStartupLocation="Center"
                 VerticalOffset="30%"
                 MinWidth="300">
        <StackPanel Orientation="Vertical">
            <TextBlock>Some contents...</TextBlock>
            <TextBlock>Some contents...</TextBlock>
            <TextBlock>Some contents...</TextBlock>
        </StackPanel>
    </atom:Dialog>
</Panel>
```

```csharp
// Code-behind：点击按钮打开
private void HandleOpenButtonClick(object? sender, RoutedEventArgs e)
{
    if (DataContext is MyViewModel vm)
    {
        vm.IsModalOpened = true;
    }
}
```

### Window 模式

使用 `DialogHostType="Window"` 创建独立窗口对话框：

```xml
<atom:Dialog PlacementTarget="OpenWindowButton"
             IsOpen="{Binding IsWindowModalOpened, Mode=TwoWay}"
             Title="Basic Window Modal"
             IsModal="True"
             DialogHostType="Window"
             IsClosable="True"
             IsDragMovable="True"
             IsMaximizable="False"
             StandardButtons="Yes"
             DefaultStandardButton="Yes"
             MinWidth="300">
    <StackPanel Orientation="Vertical">
        <TextBlock>Some contents...</TextBlock>
    </StackPanel>
</atom:Dialog>
```

---

## 2. 异步关闭

点击确认按钮后执行异步操作，完成后再关闭对话框。关键是通过 `ButtonClicked` 事件拦截并设置 `IsConfirmLoading`：

```xml
<atom:Dialog Title="Asynchronously close Modal"
             IsModal="True"
             IsDragMovable="True"
             IsLightDismissEnabled="True"
             StandardButtons="Ok, Cancel"
             DefaultStandardButton="Ok"
             ButtonClicked="HandleAsyncDialogButtonClicked"
             MinWidth="400">
    <TextBlock>Content of the modal</TextBlock>
</atom:Dialog>
```

```csharp
private void HandleAsyncDialogButtonClicked(object? sender, DialogButtonClickedEventArgs e)
{
    if (sender is Dialog dialog && e.SourceButton.Role == DialogButtonRole.AcceptRole)
    {
        dialog.IsConfirmLoading = true;
        e.Handled = true; // 阻止默认的 Accept 行为
        DispatcherTimer.RunOnce(() =>
        {
            dialog.IsConfirmLoading = false;
            dialog.Done(); // 异步操作完成后手动关闭
        }, TimeSpan.FromMilliseconds(3000));
    }
}
```

> **要点**：`e.Handled = true` 阻止了按钮的默认 Accept/Reject 行为；`IsConfirmLoading = true` 阻止了对话框被其他方式关闭。

---

## 3. 加载状态

对话框可以在打开时处于加载状态，适用于需要加载数据的场景：

```xml
<atom:Dialog Title="Loading Modal"
             IsModal="True"
             IsLoading="True"
             StandardButtons="Reload"
             DefaultStandardButton="Reload"
             Opened="HandleLoadingDialogOpened"
             ButtonClicked="HandleLoadingDialogButtonClicked"
             MinWidth="400">
    <StackPanel>
        <TextBlock>Some contents...</TextBlock>
    </StackPanel>
</atom:Dialog>
```

```csharp
// 打开后 3 秒自动结束加载
private void HandleLoadingDialogOpened(object? sender, EventArgs e)
{
    if (sender is Dialog dialog)
    {
        DispatcherTimer.RunOnce(() => dialog.IsLoading = false,
            TimeSpan.FromMilliseconds(3000));
    }
}

// 点击 Reload 按钮重新加载
private void HandleLoadingDialogButtonClicked(object? sender, DialogButtonClickedEventArgs e)
{
    if (sender is Dialog dialog)
    {
        dialog.IsLoading = true;
        DispatcherTimer.RunOnce(() => dialog.IsLoading = false,
            TimeSpan.FromMilliseconds(3000));
    }
}
```

---

## 4. 自定义底部按钮

### 标准按钮 + 自定义按钮

```xml
<atom:Dialog Title="Title"
             StandardButtons="Ok, Cancel"
             DefaultStandardButton="Ok">
    <atom:Dialog.CustomButtons>
        <atom:DialogButton Role="ActionRole">Custom Button</atom:DialogButton>
    </atom:Dialog.CustomButtons>
    <StackPanel Spacing="5">
        <TextBlock>Some contents...</TextBlock>
        <TextBlock>Some contents...</TextBlock>
    </StackPanel>
</atom:Dialog>
```

### 配置按钮属性

通过 `ButtonsConfigure` 回调可以在按钮创建后统一修改其属性：

```csharp
// Code-behind
dialog.ButtonsConfigure = buttons =>
{
    foreach (var button in buttons)
    {
        button.IsEnabled = false; // 例如：禁用所有按钮
    }
};
```

---

## 5. 可拖拽对话框

```xml
<atom:Dialog Title="Draggable Modal"
             IsModal="True"
             IsDragMovable="True"
             IsLightDismissEnabled="True"
             StandardButtons="Ok, Cancel"
             HorizontalStartupLocation="Center"
             VerticalStartupLocation="Center"
             Width="400">
    <StackPanel Spacing="10">
        <TextBlock TextWrapping="Wrap">
            Just don't learn physics at school and your life will be full of magic and miracles.
        </TextBlock>
    </StackPanel>
</atom:Dialog>
```

---

## 6. 静态 API

### Overlay 模态对话框

```csharp
var content = new StackPanel { Orientation = Orientation.Vertical, Spacing = 5 };
content.Children.Add(new TextBlock { Text = "Some contents..." });

var options = new DialogOptions
{
    Title                     = "Basic Modal",
    IsResizable               = false,
    IsDragMovable             = true,
    IsMaximizable             = false,
    StandardButtons           = DialogStandardButtons.Parse("Cancel,Ok"),
    DefaultStandardButton     = DialogStandardButton.Ok,
    HorizontalStartupLocation = DialogHorizontalAnchor.Center,
    VerticalOffset            = new Dimension(30, DimensionUnitType.Percentage),
    MinWidth                  = 400
};
await Dialog.ShowDialogModalAsync(content, null, options);
```

### Window 模态对话框

```csharp
var options = new DialogOptions
{
    Title                     = "Window Modal",
    DialogHostType            = DialogHostType.Window,
    IsResizable               = false,
    IsDragMovable             = true,
    StandardButtons           = DialogStandardButtons.Parse("Cancel,Ok"),
    DefaultStandardButton     = DialogStandardButton.Ok,
    HorizontalStartupLocation = DialogHorizontalAnchor.Center,
    VerticalOffset            = new Dimension(30, DimensionUnitType.Percentage),
    MinWidth                  = 400
};
await Dialog.ShowDialogModalAsync(content, null, options);
```

### 自定义视图 + ViewModel

```csharp
var viewModel = new MyViewModel { Name = "AtomUI", Age = 2 };
var options = new DialogOptions
{
    Title                     = "Custom View Dialog",
    DialogHostType            = DialogHostType.Window,
    StandardButtons           = DialogStandardButtons.Parse("Cancel,Ok"),
    DefaultStandardButton     = DialogStandardButton.Ok,
    HorizontalStartupLocation = DialogHorizontalAnchor.Center,
    MinWidth                  = 400
};
await Dialog.ShowDialogModalAsync<MyView, MyViewModel>(viewModel, options);
```

### 非模态异步对话框（带关闭回调）

对于非模态对话框，`ShowDialogAsync` 提供 `closed` 回调参数，在对话框关闭时调用：

```csharp
var content = new StackPanel();
content.Children.Add(new TextBlock { Text = "Non-modal dialog content" });

var options = new DialogOptions
{
    Title                     = "Non-Modal",
    StandardButtons           = DialogStandardButtons.Parse("Ok,Cancel"),
    HorizontalStartupLocation = DialogHorizontalAnchor.Center,
    VerticalOffset            = new Dimension(30, DimensionUnitType.Percentage),
    MinWidth                  = 400
};

// ShowDialogAsync 在对话框显示后立即返回
// 通过 closed 回调获取结果
await Dialog.ShowDialogAsync(content, null, options, result =>
{
    // result.Result 可能是 DialogCode.Accepted / DialogCode.Rejected / null
    if (result.Result is DialogCode code && code == DialogCode.Accepted)
    {
        // 处理确认逻辑
    }
});
```

> **注意**：`ShowDialogAsync`（非 Modal）的 Task 在对话框显示后就完成，`await` 之后无法直接读取 `Result`。需要通过 `Action<IDialogActionResult>? closed` 回调处理关闭逻辑。

---

## 7. MVVM 集成（IDialogAwareDataContext）

ViewModel 实现 `IDialogAwareDataContext` 接口后，可以在 ViewModel 层感知和控制对话框：

```csharp
public class MyDialogViewModel : ReactiveObject, IDialogAwareDataContext
{
    private IDialog? _dialog;

    public void NotifyAttachedToDialog(IDialog dialog)
    {
        _dialog = dialog;
    }

    public void NotifyClosed()
    {
        // 对话框关闭时的清理逻辑
    }

    // ViewModel 中主动关闭对话框
    public void ConfirmAndClose()
    {
        _dialog?.Accept();
    }

    public void CancelAndClose()
    {
        _dialog?.Reject();
    }
}
```

---

## 常见组合模式

### 确认操作对话框

```xml
<atom:Dialog Title="确认删除"
             IsModal="True"
             StandardButtons="Ok, Cancel"
             DefaultStandardButton="Ok"
             HorizontalStartupLocation="Center"
             VerticalStartupLocation="Center">
    <TextBlock>确���要删除选中的项目吗？此操作不可撤销。</TextBlock>
</atom:Dialog>
```

### 表单对话框

```xml
<atom:Dialog Title="新建用户"
             IsModal="True"
             IsDragMovable="True"
             StandardButtons="Save, Cancel"
             DefaultStandardButton="Save"
             Width="500"
             HorizontalStartupLocation="Center"
             VerticalOffset="20%">
    <StackPanel Spacing="10">
        <atom:TextBox Watermark="用户名" />
        <atom:TextBox Watermark="邮箱" />
    </StackPanel>
</atom:Dialog>
```

### 非模态工具窗口

```xml
<atom:Dialog Title="属性面板"
             IsModal="False"
             IsClosable="True"
             IsDragMovable="True"
             IsResizable="True"
             IsFooterVisible="False"
             Width="300"
             Height="400">
    <!-- 工具面板内容 -->
</atom:Dialog>
```
