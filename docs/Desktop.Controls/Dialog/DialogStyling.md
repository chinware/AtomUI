# Dialog 自定义样式指南

Dialog 的视觉表现通过 `ControlTheme` + Design Token 系统控制。由于 Dialog 的特殊架构（内容代理给宿主渲染），样式自定义主要通过控件属性和事件回调实现。

---

## 1. 使用属性直接控制

最简单的方式是通过 Dialog 的公共属性来控制外观和行为：

```xml
<!-- 基础模态对话框 -->
<atom:Dialog Title="Basic Modal"
             IsModal="True"
             StandardButtons="Ok, Cancel"
             DefaultStandardButton="Ok"
             MinWidth="300">
    <TextBlock>Some contents...</TextBlock>
</atom:Dialog>

<!-- 居中显示 -->
<atom:Dialog Title="Centered Modal"
             HorizontalStartupLocation="Center"
             VerticalStartupLocation="Center" />

<!-- 百分比偏移定位 -->
<atom:Dialog Title="Offset Modal"
             HorizontalStartupLocation="Center"
             VerticalOffset="30%" />

<!-- 可拖拽对话框 -->
<atom:Dialog Title="Draggable"
             IsDragMovable="True" />

<!-- 窗口宿主模式 -->
<atom:Dialog Title="Window Modal"
             DialogHostType="Window"
             IsClosable="True"
             IsMaximizable="False" />

<!-- 隐藏底部按钮 -->
<atom:Dialog Title="No Footer"
             IsFooterVisible="False" />

<!-- 点击遮罩关闭 -->
<atom:Dialog Title="Light Dismiss"
             IsLightDismissEnabled="True" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ModalShowCase.axaml`

---

## 2. 自定义底部按钮

### 标准按钮配置

通过 `StandardButtons` 属性指定标准按钮组合：

```xml
<!-- Ok + Cancel -->
<atom:Dialog StandardButtons="Ok, Cancel" DefaultStandardButton="Ok" />

<!-- Yes + No -->
<atom:Dialog StandardButtons="Yes, No" DefaultStandardButton="Yes" />

<!-- 仅 Reload 按钮 -->
<atom:Dialog StandardButtons="Reload" DefaultStandardButton="Reload" />
```

### 添加自定义按钮

通过 `CustomButtons` 集合添加额外按钮：

```xml
<atom:Dialog StandardButtons="Ok, Cancel"
             DefaultStandardButton="Ok">
    <atom:Dialog.CustomButtons>
        <atom:DialogButton Role="ActionRole">Custom Button</atom:DialogButton>
    </atom:Dialog.CustomButtons>
    <TextBlock>Content with custom button</TextBlock>
</atom:Dialog>
```

### 通过回调配置按钮属性

使用 `ButtonsConfigure` 回调在按钮创建后统一修改属性：

```csharp
// Code-behind
dialog.ButtonsConfigure = buttons =>
{
    foreach (var button in buttons)
    {
        button.IsEnabled = false; // 禁用所有按钮
    }
};
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ModalShowCase.axaml` 中 "Customize footer buttons props" 示例。

---

## 3. 异步关闭模式

通过 `ButtonClicked` 事件拦截按钮点击，实现异步关闭：

```xml
<atom:Dialog StandardButtons="Ok, Cancel"
             ButtonClicked="HandleDialogButtonClicked" />
```

```csharp
private void HandleDialogButtonClicked(object? sender, DialogButtonClickedEventArgs e)
{
    if (sender is Dialog dialog && e.SourceButton.Role == DialogButtonRole.AcceptRole)
    {
        dialog.IsConfirmLoading = true;
        e.Handled = true; // 阻止默认关闭行为
        DispatcherTimer.RunOnce(() =>
        {
            dialog.IsConfirmLoading = false;
            dialog.Done(); // 手动关闭
        }, TimeSpan.FromMilliseconds(3000));
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ModalShowCase.axaml` 中 "Asynchronously close" 示例。

---

## 4. 加载状态

```xml
<!-- 整体加载状态 -->
<atom:Dialog IsLoading="True" StandardButtons="Reload">
    <TextBlock>Loading content...</TextBlock>
</atom:Dialog>
```

```csharp
// 打开后自动结束加载
private void HandleDialogOpened(object? sender, EventArgs e)
{
    if (sender is Dialog dialog)
    {
        DispatcherTimer.RunOnce(() =>
        {
            dialog.IsLoading = false;
        }, TimeSpan.FromMilliseconds(3000));
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ModalShowCase.axaml` 中 "Loading" 示例。

---

## 5. 静态 API 创建对话框

```csharp
// Overlay 模态对话框
var options = new DialogOptions
{
    Title                     = "Basic Modal",
    IsResizable               = false,
    IsDragMovable             = true,
    StandardButtons           = DialogStandardButtons.Parse("Cancel,Ok"),
    DefaultStandardButton     = DialogStandardButton.Ok,
    HorizontalStartupLocation = DialogHorizontalAnchor.Center,
    VerticalOffset            = new Dimension(30, DimensionUnitType.Percentage),
    MinWidth                  = 400
};
var result = await Dialog.ShowDialogModalAsync(content, null, options);

// Window 模态对话框
var windowOptions = new DialogOptions
{
    Title          = "Window Modal",
    DialogHostType = DialogHostType.Window,
    // ...其他选项同上
};
await Dialog.ShowDialogModalAsync(content, null, windowOptions);

// 泛型视图 + ViewModel
await Dialog.ShowDialogModalAsync<MyView, MyViewModel>(viewModel, options);
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ModalShowCase.axaml.cs` 中 "Static Dialog API" 示例。

---

## 6. 使用 Closing 事件阻止关闭

通过 `Closing` 事件可以在对话框关闭前进行验证，设置 `Cancel = true` 阻止关闭：

```csharp
dialog.Closing += (sender, e) =>
{
    // 例如：校验表单是否已保存
    if (!IsFormSaved)
    {
        e.Cancel = true; // 阻止关闭
    }
};
```

> **注意**：当 `IsConfirmLoading = true` 时，`NotifyClose()` 方法会直接返回，不触发 `Closing` 事件。这是一个更强的关闭保护机制。

---

## 样式选择器速查

> 说明：由于 Dialog 将内容代理给 `OverlayDialogHost` 或 `DialogHost` 渲染，样式选择器应针对宿主类型使用。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|Dialog` | 匹配 Dialog 控件本身（但 Dialog 不渲染可视内容） |
| `atom\|OverlayDialogHost` | 匹配 Overlay 模式的对话框宿主 |
| `atom\|DialogHost` | 匹配 Window 模式的对话框宿主 |

### 内部元素选择

| 选择器 | 说明 |
|---|---|
| `atom\|OverlayDialogHost /template/ atom\|OverlayDialogHeader#PART_Header` | Overlay 对话框标题栏 |
| `atom\|OverlayDialogHost /template/ atom\|OverlayDialogResizer#PART_Resizer` | Overlay 对话框调整大小控件 |
| `atom\|OverlayDialogHost /template/ atom\|DialogButtonBox#PART_ButtonBox` | Overlay 对话框按钮区域 |
| `atom\|OverlayDialogHost /template/ atom\|OverlayDialogMask` | Overlay 对话框遮罩层 |
