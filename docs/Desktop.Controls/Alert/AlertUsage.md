# Alert 使用文档

本文档介绍 AtomUI Alert 控件的各种使用方式，覆盖所有常见场景。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/AlertShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Alert，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Alert 控件、AlertType 枚举
```

---

## 1. 四种类型

AtomUI 提供四种 Alert 类型，通过 `Type` 属性设置。不设置时默认为 `Success`。

```xml
<StackPanel Spacing="16">
    <atom:Alert Type="Success">Success Tips</atom:Alert>
    <atom:Alert Type="Info">Informational Notes</atom:Alert>
    <atom:Alert Type="Warning">Warning</atom:Alert>
    <atom:Alert Type="Error">Error</atom:Alert>
</StackPanel>
```

**使用场景指引**：
- **Success**：操作成功后的正面反馈，如"保存成功""提交已完成"
- **Info**：中性提示信息，如使用说明、功能介绍
- **Warning**：需要用户注意但不阻断操作的提醒，如"即将过期""请备份数据"
- **Error**：操作失败或存在风险的严重警告，如"验证失败""权限不足"

---

## 2. 带图标

通过 `IsShowIcon="True"` 自动显示匹配类型的图标，增强视觉辨识度：

```xml
<StackPanel Spacing="16">
    <atom:Alert Type="Success" IsShowIcon="True">Success Tips</atom:Alert>
    <atom:Alert Type="Info" IsShowIcon="True">Informational Notes</atom:Alert>
    <atom:Alert Type="Warning" IsShowIcon="True">Warning</atom:Alert>
    <atom:Alert Type="Error" IsShowIcon="True">Error</atom:Alert>
</StackPanel>
```

图标自动匹配规则：
- `Success` → `CheckCircleFilled`（绿色）
- `Info` → `InfoCircleFilled`（蓝色）
- `Warning` → `ExclamationCircleFilled`（橙色）
- `Error` → `CloseCircleFilled`（红色）

---

## 3. 带描述信息

添加 `Description` 可在消息下方展示更多细节。注意此时需使用 `Message` 属性而非标签内容：

```xml
<StackPanel Spacing="16">
    <atom:Alert Type="Success" IsShowIcon="True"
                Message="Success Tips"
                Description="Detailed description and advice about successful copywriting." />
    <atom:Alert Type="Info" IsShowIcon="True"
                Message="Informational Notes"
                Description="Additional description and information about copywriting." />
    <atom:Alert Type="Warning" IsShowIcon="True"
                Message="Warning"
                Description="This is a warning notice about copywriting." />
    <atom:Alert Type="Error" IsShowIcon="True"
                Message="Error"
                Description="This is an error message about copywriting." />
</StackPanel>
```

**视觉变化**：设置 `Description` 后，消息文字自动变大（充当标题角色），图标也随之增大，整体内间距增加。

---

## 4. 可关闭

通过 `IsClosable="True"` 在右侧显示关闭按钮：

```xml
<StackPanel x:Name="AlertContainer" Spacing="16">
    <atom:Alert Type="Warning" IsClosable="True"
                CloseRequest="HandleAlertClose">
        Warning Text. Warning Text. Warning Text.
    </atom:Alert>

    <atom:Alert Type="Error" IsClosable="True" IsShowIcon="True"
                Message="Error Text"
                Description="Error Description. Error Description."
                CloseRequest="HandleAlertClose" />
</StackPanel>
```

```csharp
// Code-behind
private void HandleAlertClose(object? sender, EventArgs e)
{
    if (sender is Alert alert && alert.Parent is Panel panel)
    {
        panel.Children.Remove(alert);
    }
}
```

> **重要**：`CloseRequest` 事件触发后 Alert **不会自动移除**，你需要在事件处理中手动操作。这使得你可以在关闭前执行额外逻辑（如确认对话框、记录日志等）。

---

## 5. 自定义关闭图标

```xml
<atom:Alert Type="Info" IsClosable="True"
            CloseIcon="{antdicons:AntDesignIconProvider Kind=CloseCircleOutlined}"
            CloseRequest="HandleAlertClose">
    Info with custom close icon
</atom:Alert>
```

---

## 6. 带额外操作

通过 `ExtraAction` 在消息行右侧放置操作控件，通常是 Text 或 Link 类型的小按钮：

```xml
<StackPanel Spacing="16">
    <!-- 简单文本按钮 -->
    <atom:Alert Type="Info" IsShowIcon="True" IsClosable="True"
                Message="Information">
        <atom:Alert.ExtraAction>
            <atom:Button ButtonType="Text" SizeType="Small">Detail</atom:Button>
        </atom:Alert.ExtraAction>
    </atom:Alert>

    <!-- 带描述的 Alert + 额外操作 -->
    <atom:Alert Type="Success" IsShowIcon="True" IsClosable="True"
                Message="Submission Successful"
                Description="Your application has been submitted. We will review it within 3 business days.">
        <atom:Alert.ExtraAction>
            <atom:Button ButtonType="Link" SizeType="Small">View Details</atom:Button>
        </atom:Alert.ExtraAction>
    </atom:Alert>
</StackPanel>
```

---

## 7. 跑马灯效果

长文本自动水平滚动，适合在有限宽度内展示长消息：

```xml
<atom:Alert Type="Warning" IsMessageMarqueEnabled="True" IsShowIcon="True">
    This is a very long warning message that will scroll automatically when the content overflows the available width.
</atom:Alert>
```

> 💡 跑马灯是 AtomUI 的扩展功能，Ant Design React 版不具备此特性。

---

## 8. 数据绑定

Alert 支持所有属性的数据绑定：

```xml
<atom:Alert Type="{Binding AlertType}"
            Message="{Binding AlertMessage}"
            Description="{Binding AlertDescription}"
            IsShowIcon="True"
            IsClosable="{Binding IsAlertClosable}" />
```

```csharp
// ViewModel（使用 ReactiveUI）
public class AlertViewModel : ReactiveObject
{
    [Reactive] public AlertType AlertType { get; set; } = AlertType.Info;
    [Reactive] public string AlertMessage { get; set; } = "Dynamic message";
    [Reactive] public string? AlertDescription { get; set; }
    [Reactive] public bool IsAlertClosable { get; set; } = true;
}
```

---

## 常见组合模式

### 页面顶部提示

```xml
<StackPanel Spacing="16">
    <atom:Alert Type="Info" IsShowIcon="True" IsClosable="True"
                CloseRequest="HandleAlertClose">
        A new version is available. Please refresh the page.
    </atom:Alert>
    <!-- 页面内容 -->
</StackPanel>
```

### 表单验证错误

```xml
<StackPanel Spacing="16">
    <atom:Alert Type="Error" IsShowIcon="True"
                Message="Form Validation Failed"
                Description="Please check the following fields: Name, Email, Phone number." />
    <!-- 表单内容 -->
</StackPanel>
```

### 操作结果反馈

```xml
<atom:Alert Type="Success" IsShowIcon="True" IsClosable="True"
            Message="Submission Successful"
            Description="Your application has been submitted. We will review it within 3 business days."
            CloseRequest="HandleAlertClose">
    <atom:Alert.ExtraAction>
        <atom:Button ButtonType="Link" SizeType="Small">View Details</atom:Button>
    </atom:Alert.ExtraAction>
</atom:Alert>
```

### 多条提示堆叠

```xml
<StackPanel Spacing="8">
    <atom:Alert Type="Error" IsShowIcon="True" IsClosable="True"
                CloseRequest="HandleAlertClose">
        Database connection failed. Retrying in 30 seconds.
    </atom:Alert>
    <atom:Alert Type="Warning" IsShowIcon="True" IsClosable="True"
                CloseRequest="HandleAlertClose">
        Your session will expire in 5 minutes.
    </atom:Alert>
    <atom:Alert Type="Info" IsShowIcon="True">
        System maintenance scheduled for tonight 10:00 PM.
    </atom:Alert>
</StackPanel>
```
