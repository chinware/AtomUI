# Spin 自定义样式指南

Spin 和 SpinIndicator 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍各种自定义方式。

---

## 1. 使用属性直接控制

### 基本加载指示器

最简单的方式是通过属性控制外观和行为：

```xml
<!-- 独立指示器 -->
<atom:SpinIndicator />

<!-- 三种尺寸 -->
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:SpinIndicator SizeType="Small" VerticalAlignment="Center" />
    <atom:SpinIndicator SizeType="Middle" VerticalAlignment="Center" />
    <atom:SpinIndicator SizeType="Large" VerticalAlignment="Center" />
</StackPanel>
```

### 带提示文字的容器模式

```xml
<!-- 三种尺寸 + 提示文字 -->
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

### 遮罩效果对比

```xml
<!-- 默认遮罩（降低透明度） -->
<atom:Spin IsSpinning="True" IsShowTip="True" Tip="Loading..."
           HorizontalAlignment="Stretch">
    <atom:Alert Message="Alert message title"
                Description="Further details about the context of this alert."
                Type="Info" />
</atom:Spin>

<!-- 模糊遮罩 -->
<atom:Spin IsSpinning="True" IsShowTip="True" Tip="Loading..."
           HorizontalAlignment="Stretch"
           IsMaskBlurEnabled="True">
    <atom:Alert Message="Alert message title"
                Description="Further details about the context of this alert."
                Type="Info" />
</atom:Spin>

<!-- 背景遮罩 -->
<atom:Spin IsSpinning="True" IsMaskBackgroundEnabled="True">
    <Border Padding="20" Background="White">
        <TextBlock Text="Content with background mask overlay" />
    </Border>
</atom:Spin>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SpinShowCase.axaml`

---

## 2. 自定义指示器

### 使用 Ant Design 图标替换默认圆点

```xml
<!-- SpinIndicator 使用自定义图标 -->
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:SpinIndicator SizeType="Small" VerticalAlignment="Center"
                        CustomIndicator="{antdicons:AntDesignIconProvider Kind=LoadingOutlined, StrokeBrush=#1677ff}" />
    <atom:SpinIndicator SizeType="Middle" VerticalAlignment="Center"
                        CustomIndicator="{antdicons:AntDesignIconProvider Kind=LoadingOutlined, StrokeBrush=#1677ff}" />
    <atom:SpinIndicator SizeType="Large" VerticalAlignment="Center"
                        CustomIndicator="{antdicons:AntDesignIconProvider Kind=LoadingOutlined, StrokeBrush=#1677ff}" />
</StackPanel>
```

### 在 Spin 容器中使用自定义指示器

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

### 使用 CustomIndicatorTemplate

```xml
<atom:Spin IsSpinning="True">
    <atom:Spin.CustomIndicatorTemplate>
        <DataTemplate>
            <antdicons:LoadingOutlined Width="24" Height="24"
                                        StrokeBrush="{atom:SharedTokenResource ColorPrimary}" />
        </DataTemplate>
    </atom:Spin.CustomIndicatorTemplate>
    <Border Padding="20">
        <TextBlock Text="Template-based indicator" />
    </Border>
</atom:Spin>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SpinShowCase.axaml` 中 "Custom spinning indicator" 示例。

---

## 3. 通过 Style 覆盖样式

### 全局统一样式

```xml
<Window.Styles>
    <!-- 全局 Spin 间距 -->
    <Style Selector="atom|Spin">
        <Setter Property="Margin" Value="10" />
    </Style>

    <!-- 提示文字样式 -->
    <Style Selector="atom|Spin /template/ atom|TextBlock#Tip">
        <Setter Property="FontSize" Value="12" />
        <Setter Property="Opacity" Value="0.8" />
    </Style>
</Window.Styles>
```

### 按尺寸定制

```xml
<!-- 大号 Spin 使用更大的间距 -->
<Style Selector="atom|Spin[SizeType=Large]">
    <Setter Property="Margin" Value="20" />
</Style>

<!-- 小号 Spin 的提示文字更小 -->
<Style Selector="atom|Spin[SizeType=Small] /template/ atom|TextBlock#Tip">
    <Setter Property="FontSize" Value="10" />
</Style>
```

### 自定义遮罩层样式

```xml
<!-- 自定义遮罩背景 -->
<Style Selector="atom|Spin[IsSpinning=True][IsMaskBackgroundEnabled=True] /template/ Border#Mask">
    <Setter Property="Background" Value="#40000000" />
</Style>
```

---

## 4. 通过 ControlTheme 替换主题

如果需要完全自定义 Spin 的模板结构：

```xml
<ControlTheme x:Key="MyCustomSpin" TargetType="atom:Spin">
    <Setter Property="Template">
        <ControlTemplate>
            <Panel>
                <ContentPresenter Content="{TemplateBinding Content}"
                                  Opacity="{TemplateBinding MaskOpacity}" />
                <Panel IsVisible="{TemplateBinding IsSpinning}">
                    <!-- 自定义遮罩和指示器 -->
                    <atom:SpinIndicator SizeType="{TemplateBinding SizeType}"
                                        HorizontalAlignment="Center"
                                        VerticalAlignment="Center" />
                </Panel>
            </Panel>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:Spin Theme="{StaticResource MyCustomSpin}" IsSpinning="True">
    <Border Padding="20">
        <TextBlock Text="Custom themed spin" />
    </Border>
</atom:Spin>
```

> ⚠️ 注意：替换 ControlTheme 后将失去内置的模糊效果、遮罩背景、提示文字等功能。建议优先使用 Style 覆盖。

---

## 5. 动态切换加载状态

最常见的使用模式——配合 ToggleSwitch 或数据绑定动态切换（参考 Gallery "Embedded mode" 示例）：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Spin IsSpinning="{Binding IsLoadingSwitchChecked}"
               IsShowTip="True"
               HorizontalAlignment="Stretch"
               Tip="Loading...">
        <atom:Alert Message="Alert message title"
                    Description="Further details about the context of this alert."
                    Type="Info" />
    </atom:Spin>
    
    <atom:Spin IsSpinning="{Binding IsLoadingSwitchChecked}"
               IsShowTip="True"
               Tip="Loading..."
               HorizontalAlignment="Stretch"
               IsMaskBlurEnabled="True">
        <atom:Alert Message="Alert message title"
                    Description="Further details about the context of this alert."
                    Type="Info" />
    </atom:Spin>
    
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

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SpinShowCase.axaml` 中 "Embedded mode" 示例。

---

## 样式选择器速查

### Spin 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Spin` | 匹配所有 Spin 实例 |
| `atom\|Spin[SizeType=Large]` | 匹配大号 Spin |
| `atom\|Spin[SizeType=Middle]` | 匹配中号 Spin |
| `atom\|Spin[SizeType=Small]` | 匹配小号 Spin |
| `atom\|Spin[IsSpinning=True]` | 匹配正在加载的 Spin |
| `atom\|Spin[IsMaskBlurEnabled=True]` | 匹配启用模糊遮罩的 Spin |
| `atom\|Spin[IsMaskBlurEnabled=False]` | 匹配使用透明度遮罩的 Spin |
| `atom\|Spin[IsMaskBackgroundEnabled=True]` | 匹配启用背景遮罩的 Spin |

### Spin 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Spin /template/ ContentPresenter#ContentPresenter` | 内容区域（控制透明度、模糊效果） |
| `atom\|Spin /template/ Panel#MaskLayout` | 遮罩层容器 |
| `atom\|Spin /template/ Border#Mask` | 遮罩背景 Border |
| `atom\|Spin /template/ StackPanel#IndicatorLayout` | 指示器 + 提示文字的布局容器 |
| `atom\|Spin /template/ atom\|SpinIndicator#Indicator` | 默认旋转指示器 |
| `atom\|Spin /template/ ContentPresenter#CustomIndicator` | 自定义指示器展示器 |
| `atom\|Spin /template/ atom\|TextBlock#Tip` | 提示文字 |

### SpinIndicator 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|SpinIndicator` | 匹配所有 SpinIndicator 实例 |
| `atom\|SpinIndicator[SizeType=Large]` | 匹配大号指示器 |
| `atom\|SpinIndicator[SizeType=Middle]` | 匹配中号指示器 |
| `atom\|SpinIndicator[SizeType=Small]` | 匹配小号指示器 |
| `atom\|SpinIndicator /template/ ContentPresenter#PART_CustomIndicatorPresenter` | 自定义指示器展示器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Spin[IsSpinning=True][IsMaskBlurEnabled=True] /template/ ContentPresenter#ContentPresenter` | 模糊遮罩模式下的内容区域 |
| `atom\|Spin[IsSpinning=True][IsMaskBackgroundEnabled=True] /template/ Border#Mask` | 背景遮罩模式下的遮罩 Border |
| `atom\|Spin[SizeType=Large] /template/ atom\|TextBlock#Tip` | 大号 Spin 的提示文字 |
