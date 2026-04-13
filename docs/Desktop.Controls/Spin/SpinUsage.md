# Spin 使用文档

本文档介绍 AtomUI Spin 控件的各种使用方式和常见场景。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SpinShowCase.axaml`

---

## 前置准备

### AXAML 命名空间

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

### C# 命名空间

```csharp
using AtomUI.Desktop.Controls;  // Spin, SpinIndicator
using AtomUI.Controls;           // SizeType 等共享类型
```

---

## 1. 基本用法（SpinIndicator 独立模式）

最简单的加载指示器——直接使用 `SpinIndicator`：

```xml
<atom:SpinIndicator />
```

SpinIndicator 附加到可视树后立即开始旋转动画，无需设置任何属性。

---

## 2. 三种尺寸

通过 `SizeType` 属性设置指示器大小。Small 适合文本行内，Middle 适合卡片区块，Large 适合页面级加载：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:SpinIndicator SizeType="Small" VerticalAlignment="Center" />
    <atom:SpinIndicator SizeType="Middle" VerticalAlignment="Center" />
    <atom:SpinIndicator SizeType="Large" VerticalAlignment="Center" />
</StackPanel>
```

---

## 3. 自定义指示器

### SpinIndicator 自定义图标

通过 `CustomIndicator` 属性替换默认的四圆点旋转为 Ant Design 图标：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:SpinIndicator SizeType="Small" VerticalAlignment="Center"
                        CustomIndicator="{antdicons:AntDesignIconProvider Kind=LoadingOutlined, StrokeBrush=#1677ff}" />
    <atom:SpinIndicator SizeType="Middle" VerticalAlignment="Center"
                        CustomIndicator="{antdicons:AntDesignIconProvider Kind=LoadingOutlined, StrokeBrush=#1677ff}" />
    <atom:SpinIndicator SizeType="Large" VerticalAlignment="Center"
                        CustomIndicator="{antdicons:AntDesignIconProvider Kind=LoadingOutlined, StrokeBrush=#1677ff}" />
</StackPanel>
```

自定义图标同样会应用旋转动画，图标尺寸由 `SizeType` 对应的 `IndicatorSize` Token 自动控制。

### Spin 容器中的自定义指示器

```xml
<atom:Spin IsSpinning="True">
    <atom:Spin.CustomIndicator>
        <antdicons:LoadingOutlined Width="32" Height="32"
                                    StrokeBrush="{atom:SharedTokenResource ColorPrimary}" />
    </atom:Spin.CustomIndicator>
    <Border Padding="20" Background="White">
        <TextBlock Text="Custom loading indicator" />
    </Border>
</atom:Spin>
```

---

## 4. 带提示文字的容器模式

通过 `Tip` 和 `IsShowTip` 在指示器下方显示加载提示。三种尺寸均支持：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:Spin IsSpinning="True" SizeType="Small"
               IsShowTip="True" Tip="Loading...">
        <Border Width="100" Height="100" Background="#0D000000" />
    </atom:Spin>
    <atom:Spin IsSpinning="True" SizeType="Middle"
               IsShowTip="True" Tip="Loading...">
        <Border Width="100" Height="100" Background="#0D000000" />
    </atom:Spin>
    <atom:Spin IsSpinning="True" SizeType="Large"
               IsShowTip="True" Tip="Loading...">
        <Border Width="100" Height="100" Background="#0D000000" />
    </atom:Spin>
</StackPanel>
```

### 包裹 Alert 组件

```xml
<atom:Spin IsSpinning="True" IsShowTip="True" Tip="Loading...">
    <atom:Alert Message="Alert message title"
                Description="Further details about the context of this alert."
                Type="Info" />
</atom:Spin>
```

---

## 5. 嵌入模式（动态切换）

最常见的使用模式——配合 ToggleSwitch 或数据绑定动态切换加载状态（参考 Gallery "Embedded mode" 示例）：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 默认遮罩（降低透明度） -->
    <atom:Spin IsSpinning="{Binding IsLoadingSwitchChecked}"
               IsShowTip="True"
               HorizontalAlignment="Stretch"
               Tip="Loading...">
        <atom:Alert Message="Alert message title"
                    Description="Further details about the context of this alert."
                    Type="Info" />
    </atom:Spin>
    
    <!-- 模糊遮罩 -->
    <atom:Spin IsSpinning="{Binding IsLoadingSwitchChecked}"
               IsShowTip="True"
               Tip="Loading..."
               HorizontalAlignment="Stretch"
               IsMaskBlurEnabled="True">
        <atom:Alert Message="Alert message title"
                    Description="Further details about the context of this alert."
                    Type="Info" />
    </atom:Spin>
    
    <!-- 加载状态切换 -->
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:TextBlock>Loading state：</atom:TextBlock>
        <atom:ToggleSwitch IsChecked="{Binding IsLoadingSwitchChecked}" />
    </StackPanel>
</StackPanel>
```

```csharp
// ViewModel
public class SpinViewModel : ReactiveObject
{
    private bool _isLoadingSwitchChecked;
    public bool IsLoadingSwitchChecked
    {
        get => _isLoadingSwitchChecked;
        set => this.RaiseAndSetIfChanged(ref _isLoadingSwitchChecked, value);
    }
}
```

---

## 6. 模糊遮罩

加载时对内容区域启用高斯模糊效果（替代降低透明度）：

```xml
<atom:Spin IsSpinning="True" IsMaskBlurEnabled="True"
           IsShowTip="True" Tip="Loading...">
    <Border Padding="20" Background="White">
        <StackPanel Spacing="8">
            <TextBlock Text="Card Title" FontSize="16" FontWeight="Bold" />
            <TextBlock Text="This content will be blurred during loading." />
        </StackPanel>
    </Border>
</atom:Spin>
```

模糊效果与默认透明度遮罩互斥——设置 `IsMaskBlurEnabled="True"` 后，内容不会降低透明度。

---

## 7. 背景遮罩

加载时在内容上方显示半透明背景遮罩层（颜色为 `ColorBgMask`）：

```xml
<atom:Spin IsSpinning="True" IsMaskBackgroundEnabled="True">
    <Border Padding="20" Background="White">
        <TextBlock Text="This content has a semi-transparent mask overlay." />
    </Border>
</atom:Spin>
```

`IsMaskBackgroundEnabled` 可与 `IsMaskBlurEnabled` 组合使用：

```xml
<!-- 模糊 + 背景遮罩 -->
<atom:Spin IsSpinning="True" IsMaskBlurEnabled="True" IsMaskBackgroundEnabled="True">
    <Border Padding="20">
        <TextBlock Text="Blurred content with background mask" />
    </Border>
</atom:Spin>
```

---

## 8. 异步加载场景

最常见的实际使用模式——发起异步请求前设置加载状态，完成后恢复：

```csharp
// ViewModel
public class DataViewModel : ReactiveObject
{
    [Reactive] public bool IsLoading { get; set; }
    [Reactive] public string? Data { get; set; }

    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            Data = await _repository.FetchDataAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

```xml
<atom:Spin IsSpinning="{Binding IsLoading}" Tip="Loading data..." IsShowTip="True">
    <Border Padding="20">
        <TextBlock Text="{Binding Data}" />
    </Border>
</atom:Spin>
```

---

## 9. 禁用动画

通过 `IsMotionEnabled="False"` 禁用 `MaskOpacity` 过渡动画，透明度变化将立即生效：

```xml
<atom:Spin IsSpinning="True" IsMotionEnabled="False">
    <Border Padding="20">
        <TextBlock Text="No transition animation" />
    </Border>
</atom:Spin>
```

> 注意：`IsMotionEnabled` 仅控制 `MaskOpacity` 的过渡动画，旋转动画始终运行（由 `SpinIndicator` 内部的 `Animation` API 驱动）。

---

## 常见组合模式

### 页面加载

```xml
<atom:Spin IsSpinning="{Binding IsPageLoading}"
           HorizontalAlignment="Stretch"
           VerticalAlignment="Stretch"
           Tip="Loading page..."
           IsShowTip="True">
    <!-- 页面内容 -->
</atom:Spin>
```

### 列表加载

```xml
<atom:Spin IsSpinning="{Binding IsListLoading}" Tip="Loading items..." IsShowTip="True">
    <atom:ListBox ItemsSource="{Binding Items}" />
</atom:Spin>
```

### 表单提交

```xml
<atom:Spin IsSpinning="{Binding IsSubmitting}" Tip="Submitting..." IsShowTip="True">
    <StackPanel Spacing="16">
        <!-- 表单字段 -->
        <atom:Button ButtonType="Primary" Command="{Binding SubmitCommand}">Submit</atom:Button>
    </StackPanel>
</atom:Spin>
```

### 卡片加载

```xml
<atom:Spin IsSpinning="{Binding IsCardLoading}"
           SizeType="Middle"
           IsShowTip="True"
           Tip="Loading...">
    <atom:Card Header="Dashboard" SizeType="Middle">
        <StackPanel Spacing="8">
            <TextBlock Text="{Binding CardTitle}" />
            <TextBlock Text="{Binding CardContent}" />
        </StackPanel>
    </atom:Card>
</atom:Spin>
```

### 行内加载指示

在文本行中嵌入小型加载指示器：

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <TextBlock Text="Loading data" VerticalAlignment="Center" />
    <atom:SpinIndicator SizeType="Small" VerticalAlignment="Center" />
</StackPanel>
```
