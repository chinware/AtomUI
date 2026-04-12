# ButtonSpinner 使用文档

本文档介绍 AtomUI ButtonSpinner 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/ButtonSpinnerShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 ButtonSpinner，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // ButtonSpinner, ButtonSpinnerLocation
using AtomUI.Controls;            // SizeType, InputControlStyleVariant, InputControlStatus
using Avalonia.Controls.Primitives; // SpinEventArgs, SpinDirection
```

---

## 1. 基本用法

最简单的 ButtonSpinner 包裹一段文本内容，右侧自带上下操作按钮：

```xml
<atom:ButtonSpinner>
    <atom:TextBlock
        HorizontalAlignment="Left"
        VerticalAlignment="Center"
        Text="床前明月光" />
</atom:ButtonSpinner>
```

---

## 2. 三种尺寸

通过 `SizeType` 属性设置控件尺寸，支持 `Large`（40px）、`Middle`（32px，默认）、`Small`（24px）三种。

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:ButtonSpinner SizeType="Large">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>
    <atom:ButtonSpinner SizeType="Middle">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>
    <atom:ButtonSpinner SizeType="Small">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>
</StackPanel>
```

---

## 3. 样式变体

通过 `StyleVariant` 属性设置样式变体，支持 `Outline`（线框，默认）、`Filled`（填充）、`Borderless`（无边框）三种。

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:ButtonSpinner StyleVariant="Outline">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>
    <atom:ButtonSpinner StyleVariant="Filled">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>
    <atom:ButtonSpinner StyleVariant="Borderless">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>
</StackPanel>
```

---

## 4. 禁用状态

通过 `IsEnabled="False"` 禁用控件，三种样式变体均支持：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:ButtonSpinner StyleVariant="Outline" IsEnabled="False">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>
    <atom:ButtonSpinner StyleVariant="Filled" IsEnabled="False">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>
    <atom:ButtonSpinner StyleVariant="Borderless" IsEnabled="False">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>
</StackPanel>
```

---

## 5. 前后附加组件（AddOn）

通过 `LeftAddOn` 和 `RightAddOn` 在输入框外部添加文字或图标附加组件：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 文字附加组件 -->
    <atom:ButtonSpinner
        LeftAddOn="http://"
        RightAddOn=".com"
        Width="400"
        HorizontalAlignment="Left">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>

    <!-- 图标附加组件 -->
    <atom:ButtonSpinner
        RightAddOn="{antdicons:AntDesignIconProvider Kind=SettingOutlined}"
        Width="400"
        HorizontalAlignment="Left">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>

    <!-- 混合：文字附加组件 + 内部后缀 -->
    <atom:ButtonSpinner
        LeftAddOn="http://"
        Width="400"
        HorizontalAlignment="Left"
        InnerRightContent=".com">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>
</StackPanel>
```

---

## 6. 前后缀（Prefix / Suffix）

通过 `InnerLeftContent` 和 `InnerRightContent` 在输入区域内部添加前缀和后缀：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 图标前后缀 -->
    <atom:ButtonSpinner
        InnerLeftContent="{antdicons:AntDesignIconProvider Kind=UserOutlined, FillBrush=#D7D7D7}"
        InnerRightContent="{antdicons:AntDesignIconProvider Kind=InfoCircleOutlined, FillBrush=#8C8C8C}"
        Width="400"
        HorizontalAlignment="Left">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>

    <!-- 文字前后缀 -->
    <atom:ButtonSpinner
        InnerLeftContent="￥"
        InnerRightContent="RMB"
        Width="400"
        HorizontalAlignment="Left">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>

    <!-- 禁用状态下的前后缀 -->
    <atom:ButtonSpinner
        InnerLeftContent="￥" InnerRightContent="RMB" IsEnabled="False"
        Width="400"
        HorizontalAlignment="Left">
        <atom:TextBlock
            HorizontalAlignment="Left"
            VerticalAlignment="Center"
            Text="床前明月光" />
    </atom:ButtonSpinner>
</StackPanel>
```

---

## 7. 验证状态

通过 `Status` 属性设置 Error 或 Warning 状态，搭配三种样式变体使用：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- Outline 变体 -->
    <atom:ButtonSpinner Status="Error" Width="400" HorizontalAlignment="Left">
        <atom:TextBlock HorizontalAlignment="Left" VerticalAlignment="Center"
                        Text="床前明月光" />
    </atom:ButtonSpinner>
    <atom:ButtonSpinner Status="Warning" Width="400" HorizontalAlignment="Left">
        <atom:TextBlock HorizontalAlignment="Left" VerticalAlignment="Center"
                        Text="床前明月光" />
    </atom:ButtonSpinner>

    <!-- 带前缀图标的状态 -->
    <atom:ButtonSpinner Status="Error" Width="400"
                        InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
                        HorizontalAlignment="Left">
        <atom:TextBlock HorizontalAlignment="Left" VerticalAlignment="Center"
                        Text="床前明月光" />
    </atom:ButtonSpinner>
    <atom:ButtonSpinner Status="Warning" Width="400"
                        InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
                        HorizontalAlignment="Left">
        <atom:TextBlock HorizontalAlignment="Left" VerticalAlignment="Center"
                        Text="床前明月光" />
    </atom:ButtonSpinner>

    <!-- Filled 变体 + 状态 -->
    <atom:ButtonSpinner Status="Error" Width="400" StyleVariant="Filled"
                        InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
                        HorizontalAlignment="Left">
        <atom:TextBlock HorizontalAlignment="Left" VerticalAlignment="Center"
                        Text="床前明月光" />
    </atom:ButtonSpinner>

    <!-- Borderless 变体 + 状态 -->
    <atom:ButtonSpinner Status="Error" Width="400" StyleVariant="Borderless"
                        InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
                        HorizontalAlignment="Left">
        <atom:TextBlock HorizontalAlignment="Left" VerticalAlignment="Center"
                        Text="床前明月光" />
    </atom:ButtonSpinner>
</StackPanel>
```

---

## 8. Spin 事件处理

ButtonSpinner 自身不管理数值，通过 `Spin` 事件将增减意图传递给外部：

```xml
<atom:ButtonSpinner Spin="HandleSpin">
    <atom:TextBlock
        Name="ValueText"
        HorizontalAlignment="Left"
        VerticalAlignment="Center"
        Text="{Binding CurrentValue}" />
</atom:ButtonSpinner>
```

```csharp
// Code-behind
private int _currentValue = 0;

private void HandleSpin(object? sender, SpinEventArgs e)
{
    if (e.Direction == SpinDirection.Increase)
    {
        _currentValue++;
    }
    else
    {
        _currentValue--;
    }
    // 更新 UI...
}
```

### 使用 ValidSpinDirection 限制方向

```xml
<!-- 只允许增加（减少按钮禁用） -->
<atom:ButtonSpinner ValidSpinDirection="Increase">
    <atom:TextBlock Text="只能增加" />
</atom:ButtonSpinner>

<!-- 两个方向都禁用 -->
<atom:ButtonSpinner ValidSpinDirection="None">
    <atom:TextBlock Text="不可操作" />
</atom:ButtonSpinner>
```

---

## 9. 控制操作按钮

### 隐藏操作按钮

```xml
<atom:ButtonSpinner ShowButtonSpinner="False">
    <atom:TextBlock Text="无操作按钮" />
</atom:ButtonSpinner>
```

### 禁止 Spin

```xml
<atom:ButtonSpinner AllowSpin="False">
    <atom:TextBlock Text="按钮可见但不可操作" />
</atom:ButtonSpinner>
```

### 操作按钮放在左侧

```xml
<atom:ButtonSpinner ButtonSpinnerLocation="Left">
    <atom:TextBlock Text="左侧操作按钮" />
</atom:ButtonSpinner>
```

### 浮动操作按钮

```xml
<!-- 操作按钮平时隐藏，鼠标悬浮时滑入 -->
<atom:ButtonSpinner IsButtonSpinnerFloatable="True">
    <atom:TextBlock Text="悬浮显示操作按钮" />
</atom:ButtonSpinner>
```

---

## 10. 控制动画

```xml
<!-- 禁用所有过渡动画 -->
<atom:ButtonSpinner IsMotionEnabled="False">
    <atom:TextBlock Text="无动画" />
</atom:ButtonSpinner>
```

---

## 常见组合模式

### 自定义数值输入器

```xml
<atom:ButtonSpinner SizeType="Middle" Width="200"
                    Spin="HandleSpin"
                    InnerLeftContent="￥">
    <atom:TextBlock VerticalAlignment="Center"
                    Text="{Binding Price}" />
</atom:ButtonSpinner>
```

### 带单位的数值调整

```xml
<atom:ButtonSpinner Width="300"
                    InnerRightContent="kg">
    <atom:TextBlock VerticalAlignment="Center"
                    Text="{Binding Weight}" />
</atom:ButtonSpinner>
```

### 紧凑布局组合

```xml
<atom:Space Compact="True">
    <atom:ButtonSpinner Width="150">
        <atom:TextBlock Text="Min" VerticalAlignment="Center" />
    </atom:ButtonSpinner>
    <atom:ButtonSpinner Width="150">
        <atom:TextBlock Text="Max" VerticalAlignment="Center" />
    </atom:ButtonSpinner>
</atom:Space>
```
