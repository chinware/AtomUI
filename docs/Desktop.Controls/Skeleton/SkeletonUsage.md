# Skeleton 使用文档

本文档介绍 AtomUI Skeleton 控件族的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SkeletonShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Skeleton，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Skeleton, SkeletonButton, SkeletonAvatar 等
using AtomUI.Controls;            // CustomizableSizeType, AvatarShape
using AtomUI;                      // Dimension, DimensionUnitType
```

---

## 1. 基本骨架屏

最简单的骨架屏，默认显示标题 + 2 行段落：

```xml
<atom:Skeleton IsLoading="True" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SkeletonShowCase.axaml` 中 "Basic" 示例。

---

## 2. 带头像的组合骨架屏

通过 `IsShowAvatar="True"` 显示头像区域，模拟用户信息列表的加载态：

```xml
<atom:Skeleton IsShowAvatar="True" ParagraphRows="4" IsLoading="True" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SkeletonShowCase.axaml` 中 "Complex combination" 示例。

---

## 3. 流光动画

通过 `IsActive="True"` 启用流光动画效果，骨架块呈现从左到右的渐变闪烁：

```xml
<atom:Skeleton IsActive="True" IsLoading="True" />
```

**使用建议**：建议在实际加载场景中始终启用流光动画，给予用户明确的"正在加载"视觉反馈。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SkeletonShowCase.axaml` 中 "Active Animation" 示例。

---

## 4. 包裹真实内容

`Skeleton` 控件最常见的用法是包裹真实内容，通过 `IsLoading` 属性控制显示骨架还是真实内容：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Skeleton IsLoading="{Binding SkeletonLoading}">
        <StackPanel Orientation="Vertical" Spacing="20">
            <atom:TextBlock FontWeight="Bold">Ant Design, a design language</atom:TextBlock>
            <atom:TextBlock TextWrapping="Wrap">
                We supply a series of design principles, practical patterns and high quality design
                resources (Sketch and Axure), to help people create their product prototypes beautifully
                and efficiently.
            </atom:TextBlock>
        </StackPanel>
    </atom:Skeleton>
    <atom:Button IsEnabled="{Binding SkeletonLoading, Converter={x:Static BoolConverters.Not}}"
                  Click="HandleLoadingButtonClicked">
        Show Skeleton
    </atom:Button>
</StackPanel>
```

```csharp
// Code-behind
private void HandleLoadingButtonClicked(object? sender, RoutedEventArgs e)
{
    if (DataContext is SkeletonViewModel viewModel)
    {
        viewModel.SkeletonLoading = !viewModel.SkeletonLoading;
        DispatcherTimer.RunOnce(() =>
        {
            viewModel.SkeletonLoading = !viewModel.SkeletonLoading;
        }, TimeSpan.FromSeconds(3));
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SkeletonShowCase.axaml` 中 "Contains sub component" 示例。

---

## 5. 按钮骨架（SkeletonButton）

模拟按钮的加载态，支持三种形状和三种尺寸：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <!-- 不同形状 -->
    <atom:SkeletonButton IsActive="True" Shape="Square" />
    <atom:SkeletonButton IsActive="True" Shape="Round" />
    <atom:SkeletonButton IsActive="True" Shape="Circle" />
</StackPanel>

<!-- Block 模式（撑满宽度） -->
<atom:SkeletonButton IsActive="True" IsBlock="True" />
```

尺寸和形状支持数据绑定：

```xml
<atom:SkeletonButton IsActive="{Binding IsSkeletonActive}"
                      IsBlock="{Binding IsSkeletonBlock}"
                      SizeType="{Binding SkeletonButtonAndInputSizeType}"
                      Shape="{Binding SkeletonButtonShape}" />
```

---

## 6. 头像骨架（SkeletonAvatar）

模拟头像的加载态，支持圆形和方形：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:SkeletonAvatar IsActive="True" Shape="Circle" />
    <atom:SkeletonAvatar IsActive="True" Shape="Square" SizeType="Large" />
</StackPanel>
```

---

## 7. 输入框骨架（SkeletonInput）

模拟输入框的加载态：

```xml
<atom:SkeletonInput IsActive="True" />

<!-- Block 模式 -->
<atom:SkeletonInput IsActive="True" IsBlock="True" />
```

---

## 8. 图片骨架（SkeletonImage）

模拟图片的加载态，内嵌图片图标：

```xml
<atom:SkeletonImage IsActive="True" />
```

---

## 9. 自定义节点骨架（SkeletonNode）

在骨架框内放入自定义内容：

```xml
<!-- 默认尺寸 -->
<atom:SkeletonNode IsActive="True">
    <antdicons:DotChartOutlined StrokeBrush="#bfbfbf" Width="40" Height="40" />
</atom:SkeletonNode>

<!-- 自定义宽度 -->
<atom:SkeletonNode IsActive="True" Width="160" />
```

---

## 10. 独立元素组合

在自定义布局中组合多种独立元素骨架：

```xml
<StackPanel Orientation="Vertical" Spacing="20">
    <!-- 横向组合 -->
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:SkeletonButton IsActive="True" />
        <atom:SkeletonAvatar IsActive="True" />
        <atom:SkeletonInput IsActive="True" />
    </StackPanel>

    <!-- Block 模式元素 -->
    <atom:SkeletonButton IsActive="True" IsBlock="True" />
    <atom:SkeletonInput IsActive="True" IsBlock="True" />

    <!-- 图片和自定义节点 -->
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:SkeletonImage IsActive="True" />
        <atom:SkeletonNode IsActive="True" Width="160" />
        <atom:SkeletonNode IsActive="True">
            <antdicons:DotChartOutlined StrokeBrush="#bfbfbf" Width="40" Height="40" />
        </atom:SkeletonNode>
    </StackPanel>
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SkeletonShowCase.axaml` 中 "Button/Avatar/Input/Image/Node" 示例。

---

## 11. 运行时动态控制

通过 ViewModel 属性动态切换骨架屏参数：

```xml
<WrapPanel Orientation="Horizontal" ItemSpacing="10" LineSpacing="20">
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:TextBlock VerticalAlignment="Center">Active:</atom:TextBlock>
        <atom:ToggleSwitch IsChecked="{Binding IsSkeletonActive, Mode=TwoWay}" />
    </StackPanel>

    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:TextBlock VerticalAlignment="Center">Block:</atom:TextBlock>
        <atom:ToggleSwitch IsChecked="{Binding IsSkeletonBlock, Mode=TwoWay}" />
    </StackPanel>
</WrapPanel>
```

```csharp
// ViewModel
public class SkeletonViewModel : ReactiveObject
{
    [Reactive] public bool IsSkeletonActive { get; set; }
    [Reactive] public bool IsSkeletonBlock { get; set; }
    [Reactive] public CustomizableSizeType SkeletonButtonAndInputSizeType { get; set; }
    [Reactive] public SkeletonButtonShape SkeletonButtonShape { get; set; }
    [Reactive] public AvatarShape SkeletonAvatarShape { get; set; }
    [Reactive] public bool SkeletonLoading { get; set; } = true;
}
```

---

## 常见组合模式

### 列表项加载

```xml
<StackPanel Orientation="Vertical" Spacing="16">
    <atom:Skeleton IsShowAvatar="True" IsActive="True" IsLoading="True" />
    <atom:Skeleton IsShowAvatar="True" IsActive="True" IsLoading="True" />
    <atom:Skeleton IsShowAvatar="True" IsActive="True" IsLoading="True" />
</StackPanel>
```

### 卡片内容加载

```xml
<Border CornerRadius="8" BorderBrush="#d9d9d9" BorderThickness="1" Padding="16">
    <atom:Skeleton IsActive="True" IsLoading="{Binding IsCardLoading}">
        <StackPanel Orientation="Vertical" Spacing="8">
            <atom:TextBlock FontWeight="Bold" FontSize="16">Card Title</atom:TextBlock>
            <atom:TextBlock TextWrapping="Wrap">Card content here...</atom:TextBlock>
        </StackPanel>
    </atom:Skeleton>
</Border>
```

### 表单加载态

```xml
<StackPanel Orientation="Vertical" Spacing="16">
    <atom:SkeletonInput IsActive="True" IsBlock="True" />
    <atom:SkeletonInput IsActive="True" IsBlock="True" />
    <atom:SkeletonInput IsActive="True" IsBlock="True" />
    <atom:SkeletonButton IsActive="True" Shape="Round" />
</StackPanel>
```

### 用户信息加载

```xml
<StackPanel Orientation="Horizontal" Spacing="16">
    <atom:SkeletonAvatar IsActive="True" SizeType="Large" />
    <StackPanel Orientation="Vertical" Spacing="8">
        <atom:SkeletonInput IsActive="True" SizeType="Small" />
        <atom:SkeletonInput IsActive="True" SizeType="Small" />
    </StackPanel>
</StackPanel>
```

### 图片画廊加载

```xml
<WrapPanel Orientation="Horizontal" ItemSpacing="16" LineSpacing="16">
    <atom:SkeletonImage IsActive="True" />
    <atom:SkeletonImage IsActive="True" />
    <atom:SkeletonImage IsActive="True" />
    <atom:SkeletonImage IsActive="True" />
</WrapPanel>
```
