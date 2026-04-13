# Result 使用文档

本文档介绍 AtomUI Result 控件的使用方式，涵盖全部七种状态和常见使用场景。

---

## 前置准备

### AXAML 命名空间

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

### C# 引用

```csharp
using AtomUI.Desktop.Controls;
using AtomUI.Controls; // ResultStatus 枚举
```

---

## 基本用法

### Success — 操作成功

最常见的用法，展示操作成功的结果并提供后续操作按钮。

```xml
<atom:Result Status="Success"
             Header="Successfully Purchased Cloud Server ECS!"
             SubHeader="Order number: 2017182818828182881 Cloud server configuration takes 1-5 minutes, please wait.">
    <atom:Result.Extra>
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:Button ButtonType="Primary">Go Console</atom:Button>
            <atom:Button>Buy Again</atom:Button>
        </StackPanel>
    </atom:Result.Extra>
</atom:Result>
```

### Info — 一般信息

```xml
<atom:Result Status="Info"
             Header="Your operation has been executed.">
    <atom:Result.Extra>
        <atom:Button ButtonType="Primary">Go Console</atom:Button>
    </atom:Result.Extra>
</atom:Result>
```

### Warning — 警告信息

```xml
<atom:Result Status="Warning"
             Header="There are some problems with your operation.">
    <atom:Result.Extra>
        <atom:Button ButtonType="Primary">Go Console</atom:Button>
    </atom:Result.Extra>
</atom:Result>
```

### Error — 操作失败

可以通过 `Content`（子内容区域）展示具体的错误详情：

```xml
<atom:Result Status="Error"
             Header="Submission Failed"
             SubHeader="Please check and modify the following information before resubmitting.">
    <atom:Result.Extra>
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:Button ButtonType="Primary">Go Console</atom:Button>
            <atom:Button>Buy Again</atom:Button>
        </StackPanel>
    </atom:Result.Extra>
    <!-- 子内容区域：展示详细错误信息 -->
    <StackPanel Spacing="8">
        <TextBlock FontWeight="Bold" FontSize="16">
            The content you submitted has the following error:
        </TextBlock>
        <StackPanel Orientation="Horizontal" Spacing="8">
            <antdicons:CloseCircleOutlined Foreground="{atom:SharedTokenResource ColorError}" />
            <TextBlock>Your account has been frozen. </TextBlock>
            <TextBlock Foreground="{atom:SharedTokenResource ColorPrimary}">Thaw immediately ></TextBlock>
        </StackPanel>
        <StackPanel Orientation="Horizontal" Spacing="8">
            <antdicons:CloseCircleOutlined Foreground="{atom:SharedTokenResource ColorError}" />
            <TextBlock>Your account is not yet eligible to apply. </TextBlock>
            <TextBlock Foreground="{atom:SharedTokenResource ColorPrimary}">Apply Unlock ></TextBlock>
        </StackPanel>
    </StackPanel>
</atom:Result>
```

---

## HTTP 错误码页面

### 403 — 无权限访问

```xml
<atom:Result Status="ErrorCode403"
             Header="403"
             SubHeader="Sorry, you are not authorized to access this page.">
    <atom:Result.Extra>
        <atom:Button ButtonType="Primary">Back Home</atom:Button>
    </atom:Result.Extra>
</atom:Result>
```

### 404 — 页面未找到

```xml
<atom:Result Status="ErrorCode404"
             Header="404"
             SubHeader="Sorry, the page you visited does not exist.">
    <atom:Result.Extra>
        <atom:Button ButtonType="Primary">Back Home</atom:Button>
    </atom:Result.Extra>
</atom:Result>
```

### 500 — 服务器错误

```xml
<atom:Result Status="ErrorCode500"
             Header="500"
             SubHeader="Sorry, something went wrong.">
    <atom:Result.Extra>
        <atom:Button ButtonType="Primary">Back Home</atom:Button>
    </atom:Result.Extra>
</atom:Result>
```

---

## 自定义图标

通过 `Icon` 属性覆盖默认的状态图标（仅对 Info/Success/Warning/Error 有效）：

```xml
<atom:Result Status="Info"
             Icon="{antdicons:AntDesignIconProvider SmileOutlined}"
             Header="Great, we have done all the operations!">
    <atom:Result.Extra>
        <atom:Button ButtonType="Primary">Next</atom:Button>
    </atom:Result.Extra>
</atom:Result>
```

---

## C# 代码创建

```csharp
var result = new Result
{
    Status = ResultStatus.Success,
    Header = "操作成功",
    SubHeader = "订单号: 2024XXXXXXXX",
};

// 设置额外操作区
var buttonPanel = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 10 };
buttonPanel.Children.Add(new Button { Content = "返回首页", ButtonType = ButtonType.Primary });
buttonPanel.Children.Add(new Button { Content = "再次购买" });
result.Extra = buttonPanel;
```

---

## Gallery 示例

完整的使用范例可在 Gallery 应用中查看：

- **文件路径**：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ResultShowCase.cs.axaml`
- **代码后台**：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ResultShowCase.cs`

Gallery 示例涵盖以下场景：
1. Success — 成功结果（含双按钮操作区）
2. Info — 信息提示
3. Warning — 警告信息
4. 403 — 无权限页面
5. 404 — 页面未找到
6. 500 — 服务器错误
7. Error — 失败结果（含子内容详细错误信息）
8. Custom icon — 自定义图标（SmileOutlined）
