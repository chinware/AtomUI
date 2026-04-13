# RadioButton 自定义样式指南

RadioButton 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍常见自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 RadioButton 的公共属性来控制外观：

```xml
<!-- 基本单选框 -->
<atom:RadioButton>Radio</atom:RadioButton>

<!-- 默认选中 -->
<atom:RadioButton IsChecked="True">Selected</atom:RadioButton>

<!-- 禁用 -->
<atom:RadioButton IsEnabled="False">Disabled</atom:RadioButton>

<!-- 选中 + 禁用 -->
<atom:RadioButton IsChecked="True" IsEnabled="False">Disabled Checked</atom:RadioButton>
```

单选框组的排列和间距控制：

```xml
<!-- 水平排列（默认） -->
<atom:RadioButtonGroup>
    <atom:RadioButton>Option A</atom:RadioButton>
    <atom:RadioButton>Option B</atom:RadioButton>
</atom:RadioButtonGroup>

<!-- 垂直排列 -->
<atom:RadioButtonGroup Orientation="Vertical">
    <atom:RadioButton>Option A</atom:RadioButton>
    <atom:RadioButton>Option B</atom:RadioButton>
</atom:RadioButtonGroup>

<!-- 自定义间距 -->
<atom:RadioButtonGroup ItemSpacing="20" LineSpacing="10">
    <atom:RadioButton>Option A</atom:RadioButton>
    <atom:RadioButton>Option B</atom:RadioButton>
</atom:RadioButtonGroup>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/RadioButtonShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 RadioButton 进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|RadioButton">
        <Setter Property="Margin" Value="0,0,5,5" />
    </Style>
</Window.Styles>
```

### 按状态定制

```xml
<!-- 选中态加粗 -->
<Style Selector="atom|RadioButton:checked">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>

<!-- 禁用态降低不透明度 -->
<Style Selector="atom|RadioButton:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

### 自定义指示器颜色

通过模板部件选择器定制 RadioIndicator 的视觉：

```xml
<!-- 自定义选中态指示器边框/背景颜色 -->
<Style Selector="atom|RadioButton:checked /template/ atomc|RadioIndicator#Indicator">
    <Setter Property="RadioBorderBrush" Value="#722ed1" />
    <Setter Property="RadioBackground" Value="#722ed1" />
</Style>

<!-- 自定义未选中悬浮态边框颜色 -->
<Style Selector="atom|RadioButton:not(:checked):pointerover /template/ atomc|RadioIndicator#Indicator">
    <Setter Property="RadioBorderBrush" Value="#722ed1" />
</Style>
```

### 自定义文本间距

```xml
<!-- 调整文本与指示器的间距 -->
<Style Selector="atom|RadioButton /template/ ContentPresenter#ContentPresenter">
    <Setter Property="Margin" Value="12,0,12,0" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 RadioButton 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomRadioButton" TargetType="atom:RadioButton">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    Padding="{TemplateBinding Padding}">
                <StackPanel Orientation="Horizontal" Spacing="8">
                    <!-- 自定义指示器 -->
                    <Border Width="16" Height="16" CornerRadius="8"
                            BorderBrush="Gray" BorderThickness="1" />
                    <ContentPresenter Content="{TemplateBinding Content}"
                                      VerticalAlignment="Center" />
                </StackPanel>
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:RadioButton Theme="{StaticResource MyCustomRadioButton}">自定义</atom:RadioButton>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的波纹效果和选中动画。建议优先使用 Style 覆盖。

---

## 4. 控制动画行为

```xml
<!-- 禁用选中波纹效果 -->
<atom:RadioButton IsWaveSpiritEnabled="False">No Wave</atom:RadioButton>

<!-- 禁用过渡动画（边框色、圆点大小不再渐变过渡） -->
<atom:RadioButton IsMotionEnabled="False">No Animation</atom:RadioButton>

<!-- 通过 RadioButtonGroup 统一禁用动画 -->
<atom:RadioButtonGroup IsMotionEnabled="False">
    <atom:RadioButton>Option A</atom:RadioButton>
    <atom:RadioButton>Option B</atom:RadioButton>
</atom:RadioButtonGroup>
```

---

## 5. 禁用状态交互示例

```xml
<StackPanel Orientation="Vertical">
    <StackPanel Orientation="Horizontal">
        <atom:RadioButton x:Name="Radio1">Radio1</atom:RadioButton>
        <atom:RadioButton x:Name="Radio2" IsChecked="True">Radio2</atom:RadioButton>
    </StackPanel>
    <atom:Button ButtonType="Primary" Click="ToggleDisabledStatus">
        toggle disabled
    </atom:Button>
</StackPanel>
```

```csharp
public void ToggleDisabledStatus(object? sender, RoutedEventArgs args)
{
    Radio1.IsEnabled = !Radio1.IsEnabled;
    Radio2.IsEnabled = !Radio2.IsEnabled;
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/RadioButtonShowCase.axaml` 中 "Disabled" 示例。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|RadioButton` 语法引用 `atom` XML 命名空间下的 `RadioButton` 类型，其中 `|` 是命名空间分隔符。

### RadioButton 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|RadioButton` | 匹配所有 AtomUI RadioButton 实例 |
| `atom\|RadioButton:checked` | 匹配选中状态的单选框 |
| `atom\|RadioButton:unchecked` | 匹配未选中状态的单选框 |
| `atom\|RadioButton:pointerover` | 匹配鼠标悬浮状态 |
| `atom\|RadioButton:disabled` | 匹配禁用状态 |
| `atom\|RadioButton:focus-visible` | 匹配通过键盘获得焦点 |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|RadioButton /template/ atomc\|RadioIndicator#Indicator` | 圆形选中指示器 |
| `atom\|RadioButton /template/ ContentPresenter#ContentPresenter` | 文本内容区 |
| `atom\|RadioButton /template/ Border#Frame` | RadioButton 根框架 |

### RadioButtonGroup 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|RadioButtonGroup` | 匹配所有 RadioButtonGroup 实例 |
| `atom\|RadioButtonGroup /template/ Border#Frame` | 匹配 RadioButtonGroup 根框架 |
| `atom\|RadioButtonGroup /template/ ItemsPresenter#PART_ItemsPresenter` | 匹配子项容器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|RadioButton:checked:not(:disabled)` | 非禁用的选中单选框 |
| `atom\|RadioButton:not(:checked):pointerover` | 悬浮中的未选中单选框 |
| `atom\|RadioButton:disabled:checked` | 禁用且选中的单选框 |

### 注意事项

- RadioButton 使用 `atomc` 命名空间前缀（`xmlns:atomc="https://atomui.net/common-controls"`）来引用内部控件如 `RadioIndicator`。在样式选择器中需使用 `atomc|RadioIndicator` 语法。
- 指示器是内部控件，只能通过模板部件选择器 `/template/` 访问。
