# MessageBox 使用文档

本文档介绍 AtomUI MessageBox 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ModalShowCase.axaml`（"MessageBox Style" 部分）

---

## 前置准备

在 AXAML 中使用 MessageBox，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // MessageBox, MessageBoxOptions, MessageBoxStyle
using Avalonia.Controls;          // TopLevel, Control
```

> ⚠️ **前提条件**：使用静态 API 时，应用程序必须使用 `atom:Window` 作为顶级窗口，因为 MessageBox 需要通过 `GlobalDialogManager` 管理弹窗，而该管理器仅在 `atom:Window` 中自动注册。

---

## 1. 声明式使用（AXAML）

在 AXAML 中直接声明 `<atom:MessageBox>`，通过 `IsOpen` 双向绑定控制开关状态：

### 基本 Confirm 弹框

```xml
<Panel>
    <atom:MessageBox PlacementTarget="ConfirmMsgBoxBtn"
                     Title="Do you want to delete these items?"
                     IsOpen="{Binding IsConfirmMsgBoxOpened, Mode=TwoWay}"
                     Style="Confirm"
                     HostType="{Binding MessageBoxStyleCaseHostType}">
        <StackPanel Orientation="Vertical" Spacing="5">
            <TextBlock>Some descriptions</TextBlock>
        </StackPanel>
    </atom:MessageBox>
    <atom:Button Name="ConfirmMsgBoxBtn">Confirm</atom:Button>
</Panel>
```

### 不同消息样式

```xml
<!-- Information 样式 -->
<Panel>
    <atom:MessageBox PlacementTarget="InformationMsgBoxBtn"
                     Title="This is a notification message"
                     IsOpen="{Binding IsInformationMsgBoxOpened, Mode=TwoWay}"
                     Style="Information">
        <StackPanel Orientation="Vertical" Spacing="5">
            <TextBlock>some messages...some messages...</TextBlock>
            <TextBlock>some messages...some messages...</TextBlock>
        </StackPanel>
    </atom:MessageBox>
    <atom:Button Name="InformationMsgBoxBtn">Information</atom:Button>
</Panel>

<!-- Success 样式 -->
<Panel>
    <atom:MessageBox PlacementTarget="SuccessMsgBoxBtn"
                     Title="Operation successful"
                     Style="Success"
                     IsOpen="{Binding IsSuccessMsgBoxOpened, Mode=TwoWay}">
        <StackPanel Orientation="Vertical" Spacing="5">
            <TextBlock>some messages...some messages...</TextBlock>
        </StackPanel>
    </atom:MessageBox>
    <atom:Button Name="SuccessMsgBoxBtn">Success</atom:Button>
</Panel>

<!-- Error 样式 -->
<Panel>
    <atom:MessageBox PlacementTarget="ErrorMsgBoxBtn"
                     Title="This is an error message"
                     Style="Error"
                     IsOpen="{Binding IsErrorMsgBoxOpened, Mode=TwoWay}">
        <TextBlock>some messages...</TextBlock>
    </atom:MessageBox>
    <atom:Button Name="ErrorMsgBoxBtn">Error</atom:Button>
</Panel>

<!-- Warning 样式 -->
<Panel>
    <atom:MessageBox PlacementTarget="WarningMsgBoxBtn"
                     Title="This is a warning message"
                     Style="Warning"
                     IsOpen="{Binding IsWarningMsgBoxOpened, Mode=TwoWay}">
        <TextBlock>some messages...</TextBlock>
    </atom:MessageBox>
    <atom:Button Name="WarningMsgBoxBtn">Warning</atom:Button>
</Panel>
```

**声明式使用要点**：
- MessageBox 与触发按钮放在同一个 `Panel` 中
- `PlacementTarget` 指向触发按钮的 `Name`，用于定位弹窗位置
- `IsOpen` 必须使用 `Mode=TwoWay` 双向绑定，以便弹框关闭时自动重置为 `false`
- 消息内容作为 MessageBox 的子内容（`Content` 属性标记了 `[Content]`）

---

## 2. 命令式使用（静态 API）

通过静态方法在代码中快速弹出消息弹框，无需在 AXAML 中预先声明：

### 异步模态调用（推荐）

```csharp
private async void HandleCreateConfirmMessageBox(object? sender, RoutedEventArgs e)
{
    await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);

    var content = new StackPanel
    {
        Orientation = Orientation.Vertical,
        Spacing = 5,
        Children =
        {
            new TextBlock { Text = "Some contents..." },
            new TextBlock { Text = "Some contents..." }
        }
    };

    var options = new MessageBoxOptions
    {
        Title             = "Do you want to delete these items?",
        IsDragMovable     = true,
        IsCenterOnStartup = true,
        Style             = MessageBoxStyle.Confirm
    };

    await MessageBox.ShowMessageModalAsync(content, null, options);
}
```

### 不同样式的静态调用

```csharp
// Information 样式
private async void ShowInfoMessage(object? sender, RoutedEventArgs e)
{
    await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
    var content = BuildContent();
    var options = new MessageBoxOptions
    {
        Title = "This is a notification message",
        Style = MessageBoxStyle.Information,
        IsDragMovable = true
    };
    await MessageBox.ShowMessageModalAsync(content, null, options);
}

// Success 样式
private async void ShowSuccessMessage(object? sender, RoutedEventArgs e)
{
    await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
    var content = BuildContent();
    var options = new MessageBoxOptions
    {
        Title = "Operation successful",
        Style = MessageBoxStyle.Success,
        IsDragMovable = true
    };
    await MessageBox.ShowMessageModalAsync(content, null, options);
}

// Error 样式
private async void ShowErrorMessage(object? sender, RoutedEventArgs e)
{
    await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
    var content = BuildContent();
    var options = new MessageBoxOptions
    {
        Title = "This is an error message",
        Style = MessageBoxStyle.Error,
        IsDragMovable = true
    };
    await MessageBox.ShowMessageModalAsync(content, null, options);
}
```

**静态 API 使用要点**：
- 在事件处理器中使用 `await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background)` 确保 UI 线程完成当前布局后再弹出
- `ShowMessageModalAsync` 是最常用的方法——模态弹出并返回用户操作结果
- 静态方法会自动查找 `GlobalDialogManager` 并管理弹窗的创建和清理
- 弹窗关闭后自动从 `GlobalDialogManager` 中移除

---

## 3. 获取操作结果

### 模态方式获取结果

```csharp
var result = await MessageBox.ShowMessageModalAsync(content, null, options);
// result 包含用户的操作结果
if (result != null)
{
    // 用户点击了确认
}
```

### 回调方式获取结果

```csharp
await MessageBox.ShowMessageBoxAsync<MyView, MyViewModel>(
    viewModel,
    options,
    closed: (actionResult) =>
    {
        var result = actionResult.Result;
        // 处理关闭后的逻辑
    }
);
```

---

## 4. 使用 Window 宿主模式

默认使用 Overlay 模式（页面内浮层），可以切换为 Window 模式（独立原生窗口）：

```xml
<!-- AXAML 中指定 Window 模式 -->
<atom:MessageBox HostType="Window"
                 Title="原生窗口弹框"
                 Style="Information"
                 IsOpen="{Binding IsOpen, Mode=TwoWay}">
    <TextBlock>这将在独立窗口中显示。</TextBlock>
</atom:MessageBox>
```

```csharp
// 静态 API 中指定 Window 模式
var options = new MessageBoxOptions
{
    Title    = "原生窗口弹框",
    Style    = MessageBoxStyle.Information,
    HostType = DialogHostType.Window
};
await MessageBox.ShowMessageModalAsync(content, null, options);
```

---

## 5. 拖拽移动

```xml
<atom:MessageBox IsDragMovable="True"
                 Title="可拖拽弹框" Style="Confirm"
                 IsOpen="{Binding IsOpen, Mode=TwoWay}">
    <TextBlock>拖拽标题栏移动此弹框。</TextBlock>
</atom:MessageBox>
```

---

## 6. 轻量关闭（点击遮罩关闭）

仅 Overlay 模式有效：

```xml
<atom:MessageBox IsLightDismissEnabled="True"
                 Title="点击外部关闭" Style="Information"
                 IsOpen="{Binding IsOpen, Mode=TwoWay}">
    <TextBlock>点击弹框外部区域即可关闭。</TextBlock>
</atom:MessageBox>
```

---

## 7. 自定义尺寸

```csharp
var options = new MessageBoxOptions
{
    Title     = "固定尺寸弹框",
    Style     = MessageBoxStyle.Information,
    Width     = 500,
    Height    = 300,
    MinWidth  = 400,
    MaxWidth  = 600
};
await MessageBox.ShowMessageModalAsync(content, null, options);
```

---

## 8. 监听事件

```csharp
// 声明式使用时，在 Code-behind 中订阅事件
var messageBox = new MessageBox
{
    Title = "确认操作",
    Style = MessageBoxStyle.Confirm
};

messageBox.Confirmed += (sender, e) =>
{
    // 用户点击了确认按钮
    Console.WriteLine("Confirmed!");
};

messageBox.Cancelled += (sender, e) =>
{
    // 用户点击了取消按钮
    Console.WriteLine("Cancelled!");
};

messageBox.Closed += (sender, e) =>
{
    // 弹框关闭（无论确认还是取消）
    Console.WriteLine("Closed!");
};
```

---

## 9. MVVM 绑定模式

MessageBox 天然支持 MVVM 模式——内容通过 `Content` 属性传入，DataContext 可以绑定到 ViewModel：

### 泛型方式（推荐）

```csharp
// 自动创建 TView 实例并设置 DataContext
await MessageBox.ShowMessageBoxModalAsync<ConfirmView, ConfirmViewModel>(
    viewModel,
    new MessageBoxOptions
    {
        Title = "确认操作",
        Style = MessageBoxStyle.Confirm
    }
);
```

### 自定义内容方式

```csharp
var content = new ConfirmView();
await MessageBox.ShowMessageModalAsync(
    content,
    viewModel,   // 设置为 content.DataContext
    new MessageBoxOptions { Title = "确认操作", Style = MessageBoxStyle.Confirm }
);
```

---

## 常见使用模式

### 删除确认

```csharp
private async void OnDeleteClick(object? sender, RoutedEventArgs e)
{
    await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);

    var content = new TextBlock { Text = "确定要删除选中的项目吗？此操作不可恢复。" };
    var options = new MessageBoxOptions
    {
        Title = "确认删除",
        Style = MessageBoxStyle.Confirm,
        IsCenterOnStartup = true
    };

    var result = await MessageBox.ShowMessageModalAsync(content, null, options);
    // 根据 result 判断用户操作
}
```

### 操作成功提示

```csharp
private async void ShowSuccessNotice()
{
    await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);

    var content = new TextBlock { Text = "数据已成功保存到服务器。" };
    var options = new MessageBoxOptions
    {
        Title = "保存成功",
        Style = MessageBoxStyle.Success
    };

    await MessageBox.ShowMessageModalAsync(content, null, options);
}
```

### 错误详情展示

```csharp
private async void ShowError(Exception ex)
{
    await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);

    var content = new StackPanel
    {
        Spacing = 8,
        Children =
        {
            new TextBlock { Text = ex.Message },
            new TextBlock { Text = ex.StackTrace, FontSize = 10, Opacity = 0.6 }
        }
    };

    var options = new MessageBoxOptions
    {
        Title    = "操作失败",
        Style    = MessageBoxStyle.Error,
        MaxWidth = 600
    };

    await MessageBox.ShowMessageModalAsync(content, null, options);
}
```
