# HyperLinkButton 使用文档

本文档介绍 AtomUI HyperLinkButton 控件的各种使用方式。

> 📖 Gallery 中使用了 HyperLinkButton 的页面：
> - `controlgallery/AtomUIGallery/ShowCases/Views/General/AboutUsPage.axaml`（带图标的导航链接）
> - `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CardShowCase.axaml`（Card 中的 "More" 链接）
> - `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/FormShowCase.axaml`（表单中的 "Forgot password" 链接）

---

## 前置准备

在 AXAML 中使用 HyperLinkButton，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // HyperLinkButton 控件
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本用法

最基本的超链接按钮，点击后在浏览器中打开链接：

```xml
<atom:HyperLinkButton NavigateUri="https://github.com/AntDesign/AtomUI">
    Visit AtomUI
</atom:HyperLinkButton>
```

**要点**：
- 设置 `NavigateUri` 后，点击会自动调用系统浏览器打开链接
- 导航成功后，`IsVisited` 自动变为 `true`
- 支持任意 URI 格式（`https://`、`mailto:`、`file://` 等）

---

## 2. 尺寸

通过 `SizeType` 控制尺寸，支持 `Large`、`Middle`（默认）、`Small` 三种：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:HyperLinkButton SizeType="Large" NavigateUri="https://example.com">
        Large Link
    </atom:HyperLinkButton>
    <atom:HyperLinkButton SizeType="Middle" NavigateUri="https://example.com">
        Middle Link
    </atom:HyperLinkButton>
    <atom:HyperLinkButton SizeType="Small" NavigateUri="https://example.com">
        Small Link
    </atom:HyperLinkButton>
</StackPanel>
```

---

## 3. 带图标

通过 `Icon` 属性在链接文本前显示图标：

```xml
<!-- 图标 + 文本 -->
<atom:HyperLinkButton Icon="{antdicons:AntDesignIconProvider Kind=LinkOutlined}"
                       NavigateUri="https://example.com">
    Open Link
</atom:HyperLinkButton>

<!-- 仅图标模式（不设置 Content） -->
<atom:HyperLinkButton Icon="{antdicons:AntDesignIconProvider Kind=GithubOutlined}"
                       NavigateUri="https://github.com" />
```

**Gallery 实际示例**（来自 `AboutUsPage.axaml`）：

```xml
<atom:HyperLinkButton NavigateUri="https://qinware.com">Home Page</atom:HyperLinkButton>
<atom:HyperLinkButton Icon="{antdicons:AntDesignIconProvider GiteeOutlined}"
                       NavigateUri="https://gitee.com/AntDesign/AtomUI">
    Gitee
</atom:HyperLinkButton>
<atom:HyperLinkButton Icon="{antdicons:AntDesignIconProvider GithubOutlined}"
                       NavigateUri="https://github.com/AntDesign/AtomUI">
    Github
</atom:HyperLinkButton>
```

---

## 4. 危险样式

通过 `IsDanger="True"` 使用红色链接样式，适用于删除、移除等破坏性操作：

```xml
<atom:HyperLinkButton IsDanger="True">
    Delete Account
</atom:HyperLinkButton>

<atom:HyperLinkButton IsDanger="True"
                       Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}">
    Remove Data
</atom:HyperLinkButton>
```

---

## 5. 加载状态

通过 `IsLoading` 属性显示旋转加载图标，适合异步操作等待场景：

```xml
<!-- 静态加载状态 -->
<atom:HyperLinkButton IsLoading="True">
    Loading...
</atom:HyperLinkButton>
```

```xml
<!-- 点击后动态加载 -->
<atom:HyperLinkButton Click="HandleLinkClick">
    Fetch Data
</atom:HyperLinkButton>
```

```csharp
// Code-behind
public void HandleLinkClick(object? sender, RoutedEventArgs args)
{
    if (sender is HyperLinkButton link)
    {
        link.IsLoading = true;
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2)); // 模拟异步操作
            link.IsLoading = false;
        });
    }
}
```

---

## 6. 作为普通按钮使用（不导航）

不设置 `NavigateUri` 时，HyperLinkButton 就是一个链接样式的普通按钮，适合内联操作：

```xml
<!-- 使用 Click 事件 -->
<atom:HyperLinkButton Click="HandleLinkClick">
    Click Me
</atom:HyperLinkButton>

<!-- 使用 MVVM Command -->
<atom:HyperLinkButton Command="{Binding OpenDetailCommand}">
    View Details
</atom:HyperLinkButton>
```

**Gallery 实际示例**（来自 `CardShowCase.axaml`）：

```xml
<atom:HyperLinkButton>More</atom:HyperLinkButton>
```

---

## 7. 已访问状态

`IsVisited` 属性在导航成功后自动设置，也可手动控制：

```xml
<!-- 手动设置已访问状态 -->
<atom:HyperLinkButton IsVisited="True"
                       NavigateUri="https://example.com">
    Already Visited
</atom:HyperLinkButton>

<!-- 绑定已访问状态 -->
<atom:HyperLinkButton IsVisited="{Binding IsLinkVisited}"
                       NavigateUri="https://example.com">
    Tracked Link
</atom:HyperLinkButton>
```

可以通过 `:visited` 伪类为已访问链接设置不同样式：

```xml
<Window.Styles>
    <Style Selector="atom|HyperLinkButton:visited">
        <Setter Property="Foreground" Value="#531dab" />
    </Style>
</Window.Styles>
```

---

## 8. C# 代码操作

### 动态创建

```csharp
using AtomUI.Desktop.Controls;
using AtomUI.Controls;

var link = new HyperLinkButton
{
    Content = "Visit Documentation",
    NavigateUri = new Uri("https://docs.example.com"),
    SizeType = SizeType.Small
};

parentPanel.Children.Add(link);
```

### 监听导航状态

```csharp
var link = new HyperLinkButton
{
    NavigateUri = new Uri("https://example.com"),
    Content = "Open Link"
};

link.PropertyChanged += (sender, e) =>
{
    if (e.Property == HyperLinkButton.IsVisitedProperty && link.IsVisited)
    {
        Console.WriteLine("链接已被访问");
    }
};
```

---

## 常见组合模式

### 操作链接列表

```xml
<StackPanel Orientation="Horizontal" Spacing="16">
    <atom:HyperLinkButton>Edit</atom:HyperLinkButton>
    <atom:Separator Orientation="Vertical" />
    <atom:HyperLinkButton IsDanger="True">Delete</atom:HyperLinkButton>
    <atom:Separator Orientation="Vertical" />
    <atom:HyperLinkButton NavigateUri="https://example.com/help">Help</atom:HyperLinkButton>
</StackPanel>
```

### 表单中的辅助链接

**Gallery 实际示例**（来自 `FormShowCase.axaml`）：

```xml
<atom:HyperLinkButton>Forgot password</atom:HyperLinkButton>
```

### 页面底部导航

```xml
<StackPanel Orientation="Horizontal" Spacing="8"
            HorizontalAlignment="Center">
    <atom:HyperLinkButton NavigateUri="https://example.com/about">About</atom:HyperLinkButton>
    <atom:TextBlock VerticalAlignment="Center">|</atom:TextBlock>
    <atom:HyperLinkButton NavigateUri="https://example.com/privacy">Privacy</atom:HyperLinkButton>
    <atom:TextBlock VerticalAlignment="Center">|</atom:TextBlock>
    <atom:HyperLinkButton NavigateUri="https://example.com/terms">Terms</atom:HyperLinkButton>
</StackPanel>
```

### 关于页面

**Gallery 实际示例**（来自 `AboutUsPage.axaml`）：

```xml
<StackPanel Spacing="4">
    <atom:HyperLinkButton NavigateUri="https://qinware.com">Home Page</atom:HyperLinkButton>
    <atom:HyperLinkButton Icon="{antdicons:AntDesignIconProvider GiteeOutlined}"
                           NavigateUri="https://gitee.com/AntDesign/AtomUI">
        Gitee
    </atom:HyperLinkButton>
    <atom:HyperLinkButton Icon="{antdicons:AntDesignIconProvider GithubOutlined}"
                           NavigateUri="https://github.com/AntDesign/AtomUI">
        Github
    </atom:HyperLinkButton>
</StackPanel>
```

### 禁用链接

```xml
<atom:HyperLinkButton IsEnabled="False"
                       NavigateUri="https://example.com">
    Unavailable Link
</atom:HyperLinkButton>
```
