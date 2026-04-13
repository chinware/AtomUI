# Input 使用文档

本文档介绍 AtomUI Input 控件族的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/LineEditShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Input 控件，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // LineEdit, SearchEdit, TextArea 控件
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本用法

最基础的 LineEdit 用法——单行文本输入：

```xml
<atom:LineEdit PlaceholderText="Basic usage" />
```

---

## 2. 三种尺寸

通过 `SizeType` 属性设置尺寸，支持 `Large`（40px）、`Middle`（32px，默认）、`Small`（24px）三种：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:LineEdit PlaceholderText="Large" SizeType="Large"
                   InnerLeftContent="{antdicons:AntDesignIconProvider Kind=UserOutlined}" />
    <atom:LineEdit PlaceholderText="Middle" SizeType="Middle"
                   InnerLeftContent="{antdicons:AntDesignIconProvider Kind=UserOutlined}" />
    <atom:LineEdit PlaceholderText="Small" SizeType="Small"
                   InnerLeftContent="{antdicons:AntDesignIconProvider Kind=UserOutlined}" />
</StackPanel>
```

---

## 3. 样式变体

通过 `StyleVariant` 属性切换四种样式变体：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:LineEdit PlaceholderText="Outlined" StyleVariant="Outline" />
    <atom:LineEdit PlaceholderText="Filled" StyleVariant="Filled" />
    <atom:LineEdit PlaceholderText="Borderless" StyleVariant="Borderless" />
    <atom:LineEdit PlaceholderText="Underlined" StyleVariant="Underlined" />
</StackPanel>
```

样式变体也适用于带 AddOn 的输入框：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:LineEdit LeftAddOn="http://" RightAddOn=".com" Text="mysite" />
    <atom:LineEdit LeftAddOn="http://" RightAddOn=".com" Text="mysite" StyleVariant="Filled" />
    <atom:LineEdit LeftAddOn="http://" RightAddOn=".com" Text="mysite" StyleVariant="Borderless" />
    <atom:LineEdit LeftAddOn="http://" RightAddOn=".com" Text="mysite" StyleVariant="Underlined" />
</StackPanel>
```

---

## 4. 前置/后置标签（AddOn）

通过 `LeftAddOn` 和 `RightAddOn` 属性添加输入框外侧的附加组件：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:LineEdit LeftAddOn="http://" RightAddOn=".com" Text="mysite" />
    <atom:LineEdit RightAddOn="{antdicons:AntDesignIconProvider Kind=SettingOutlined}"
                   Text="mysite" />
    <atom:LineEdit LeftAddOn="http://" InnerRightContent=".com" Text="mysite" />
</StackPanel>
```

---

## 5. 前缀/后缀图标

通过 `InnerLeftContent` 和 `InnerRightContent` 添加输入框内侧的装饰元素：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:LineEdit PlaceholderText="Enter your username"
                   InnerLeftContent="{antdicons:AntDesignIconProvider Kind=UserOutlined, StrokeBrush=#D7D7D7}"
                   InnerRightContent="{antdicons:AntDesignIconProvider Kind=InfoCircleOutlined, StrokeBrush=#8C8C8C}" />
    <atom:LineEdit InnerLeftContent="￥" InnerRightContent="RMB" />
    <atom:LineEdit InnerLeftContent="￥" InnerRightContent="RMB" IsEnabled="False" />
</StackPanel>
```

---

## 6. 清除按钮

通过 `IsAllowClear="True"` 启用清除按钮，点击后清空输入内容：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:LineEdit PlaceholderText="input with clear icon" IsAllowClear="True" />
    <atom:TextArea PlaceholderText="textarea with clear icon" IsAllowClear="True" />
</StackPanel>
```

---

## 7. 密码输入框

通过 `PasswordChar` + `IsEnableRevealButton` 组合实现密码输入功能：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 基础密码框 -->
    <atom:LineEdit PlaceholderText="input password"
                   RevealPassword="False"
                   PasswordChar="•"
                   IsEnableRevealButton="True" />

    <!-- 密码框 + 清除按钮 -->
    <atom:LineEdit PlaceholderText="input password"
                   RevealPassword="False"
                   PasswordChar="•"
                   IsEnableRevealButton="True"
                   IsAllowClear="True" />
</StackPanel>
```

---

## 8. 验证状态

通过 `Status` 属性设置验证反馈状态：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 错误状态 -->
    <atom:LineEdit PlaceholderText="Error" Status="Error" />
    <atom:LineEdit PlaceholderText="Error with prefix"
                   InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
                   Status="Error" />

    <!-- 警告状态 -->
    <atom:LineEdit PlaceholderText="Warning" Status="Warning" />
    <atom:LineEdit PlaceholderText="Warning with prefix"
                   InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
                   Status="Warning" />

    <!-- 不同样式变体的验证状态 -->
    <atom:LineEdit PlaceholderText="Error" Status="Error" StyleVariant="Filled"
                   InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}" />
    <atom:LineEdit PlaceholderText="Warning" Status="Warning" StyleVariant="Borderless"
                   InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}" />
</StackPanel>
```

---

## 9. 搜索框（SearchEdit）

SearchEdit 提供内置搜索按钮的输入框：

### 基础搜索框

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:SearchEdit PlaceholderText="input search text" SizeType="Large" />
    <atom:SearchEdit PlaceholderText="input search text"
                     SearchButtonText="Search" />
</StackPanel>
```

### 主色调搜索按钮

```xml
<atom:SearchEdit PlaceholderText="input search text"
                 SearchButtonStyle="Primary"
                 SearchButtonText="Search" />
```

### 带清除按钮和自定义内容

```xml
<atom:SearchEdit PlaceholderText="input search text"
                 SearchButtonStyle="Primary"
                 SearchButtonText="Search"
                 IsAllowClear="True" />

<atom:SearchEdit PlaceholderText="input search text"
                 SearchButtonStyle="Primary"
                 SearchButtonText="搜索一下"
                 InnerRightContent="{antdicons:AntDesignIconProvider Kind=AudioOutlined, StrokeBrush=#1677ff, Width=16, Height=16}"
                 IsAllowClear="True"
                 SizeType="Large" />
```

### 搜索框加载状态

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:SearchEdit PlaceholderText="input search loading default"
                     IsOperating="True" />
    <atom:SearchEdit PlaceholderText="input search loading with enterButton"
                     IsOperating="True"
                     SearchButtonStyle="Primary" />
    <atom:SearchEdit PlaceholderText="input search text"
                     IsOperating="True"
                     SearchButtonStyle="Primary"
                     SearchButtonText="Search"
                     SizeType="Large" />
</StackPanel>
```

### 不同变体的搜索框

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- Filled -->
    <atom:SearchEdit PlaceholderText="input search text"
                     SearchButtonText="Search"
                     LeftAddOn="https://"
                     StyleVariant="Filled" />
    <!-- Borderless -->
    <atom:SearchEdit PlaceholderText="input search text"
                     SearchButtonText="Search"
                     LeftAddOn="https://"
                     StyleVariant="Borderless" />
    <!-- Underlined -->
    <atom:SearchEdit PlaceholderText="input search text"
                     SearchButtonText="Search"
                     LeftAddOn="https://"
                     StyleVariant="Underlined" />
</StackPanel>
```

### 搜索按钮事件处理

```csharp
// AXAML
// <atom:SearchEdit SearchButtonClick="OnSearchButtonClick" SearchButtonStyle="Primary" SearchButtonText="Search" />

public void OnSearchButtonClick(object? sender, RoutedEventArgs args)
{
    if (sender is SearchEdit searchEdit)
    {
        var searchText = searchEdit.Text;
        // 执行搜索逻辑...
    }
}
```

---

## 10. 多行文本域（TextArea）

### 基本用法

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:TextArea Lines="4" />
    <atom:TextArea Lines="4" PlaceholderText="maxLength is 6" MaxLength="6" />
    <atom:TextArea Lines="4" PlaceholderText="Filled variant" MaxLength="6" StyleVariant="Filled" />
    <atom:TextArea Lines="4" PlaceholderText="disabled" MaxLength="6" IsEnabled="False" />
</StackPanel>
```

### 自适应高度

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 完全自适应 -->
    <atom:TextArea PlaceholderText="Autosize height based on content lines"
                   IsAutoSize="True" />
    <!-- 限制最小/最大行数 -->
    <atom:TextArea PlaceholderText="Autosize height with minimum and maximum number of lines"
                   MinLines="3" MaxLines="6" />
</StackPanel>
```

### 字符计数

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:LineEdit IsShowCount="True" MaxLength="20" />
    <atom:TextArea IsShowCount="True" MaxLength="100"
                   PlaceholderText="can resize" IsResizable="True" />
    <atom:TextArea IsShowCount="True" MaxLength="100"
                   PlaceholderText="disable resize" Height="120" IsResizable="True" />
</StackPanel>
```

### TextArea 验证状态

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:TextArea IsShowCount="True" MaxLength="100"
                   PlaceholderText="Error" IsResizable="True" Status="Error" />
    <atom:TextArea IsShowCount="True" MaxLength="100"
                   PlaceholderText="Warning" IsResizable="True" Status="Warning" />
</StackPanel>
```

---

## 11. 禁用状态

通过 `IsEnabled="False"` 禁用输入框，所有控件和变体均支持：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:LineEdit PlaceholderText="Outlined" StyleVariant="Outline" IsEnabled="False" />
    <atom:LineEdit PlaceholderText="Filled" StyleVariant="Filled" IsEnabled="False" />
    <atom:LineEdit PlaceholderText="Borderless" StyleVariant="Borderless" IsEnabled="False" />
    <atom:LineEdit PlaceholderText="Underlined" StyleVariant="Underlined" IsEnabled="False" />

    <!-- 禁用搜索框 -->
    <atom:SearchEdit PlaceholderText="input search text"
                     SearchButtonStyle="Primary"
                     SearchButtonText="Search"
                     IsEnabled="False" />

    <!-- 禁用文本域 -->
    <atom:TextArea Lines="4" PlaceholderText="disabled" IsEnabled="False" />
</StackPanel>
```

---

## 12. MVVM 数据绑定

Input 控件完整支持 Avalonia 的数据绑定：

```xml
<!-- AXAML：绑定到 ViewModel -->
<atom:LineEdit Text="{Binding Username}" PlaceholderText="Enter username" />
<atom:LineEdit Text="{Binding Email}" PlaceholderText="Enter email"
               Status="{Binding EmailStatus}" />
<atom:TextArea Text="{Binding Description}" PlaceholderText="Enter description" Lines="4" />
```

```csharp
// ViewModel（使用 ReactiveUI）
public class FormViewModel : ReactiveObject
{
    [Reactive] public string? Username { get; set; }
    [Reactive] public string? Email { get; set; }
    [Reactive] public string? Description { get; set; }
    [Reactive] public InputControlStatus EmailStatus { get; set; }
}
```

---

## 常见组合模式

### 登录表单

```xml
<StackPanel Orientation="Vertical" Spacing="10" Width="300">
    <atom:LineEdit PlaceholderText="Username"
                   InnerLeftContent="{antdicons:AntDesignIconProvider Kind=UserOutlined}" />
    <atom:LineEdit PlaceholderText="Password"
                   PasswordChar="•"
                   IsEnableRevealButton="True"
                   InnerLeftContent="{antdicons:AntDesignIconProvider Kind=LockOutlined}" />
    <atom:Button ButtonType="Primary" HorizontalAlignment="Stretch">Log in</atom:Button>
</StackPanel>
```

### 搜索栏

```xml
<atom:SearchEdit PlaceholderText="Search articles..."
                 SearchButtonStyle="Primary"
                 SearchButtonText="Search"
                 IsAllowClear="True"
                 SizeType="Large" />
```

### 网址输入

```xml
<atom:LineEdit LeftAddOn="https://"
               RightAddOn=".com"
               PlaceholderText="mysite"
               IsAllowClear="True" />
```

### 带前缀符号的输入

```xml
<atom:LineEdit InnerLeftContent="￥"
               InnerRightContent="RMB"
               PlaceholderText="Amount" />
```
