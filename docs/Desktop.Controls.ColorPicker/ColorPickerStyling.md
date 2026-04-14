# ColorPicker 自定义样式指南

ColorPicker 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 ColorPicker 的公共属性来控制外观：

```xml
<!-- 基本颜色选择器 -->
<atom:ColorPicker DefaultValue="Red" />

<!-- 显示颜色文本 -->
<atom:ColorPicker IsShowText="True" DefaultValue="#1677ff" />

<!-- 允许清除颜色 -->
<atom:ColorPicker IsClearEnabled="True" IsShowText="True" />

<!-- 禁用 Alpha 通道 -->
<atom:ColorPicker IsAlphaEnabled="False" />

<!-- 禁用格式切换 -->
<atom:ColorPicker IsFormatEnabled="False" />

<!-- 不同尺寸 -->
<atom:ColorPicker SizeType="Large" IsShowText="True" />
<atom:ColorPicker SizeType="Small" IsShowText="True" />

<!-- 不同样式变体 -->
<atom:ColorPicker StyleVariant="Outlined" IsShowText="True" />
<atom:ColorPicker StyleVariant="Filled" IsShowText="True" />
<atom:ColorPicker StyleVariant="Borderless" IsShowText="True" />

<!-- 悬浮触发 -->
<atom:ColorPicker TriggerType="Hover" />

<!-- 值同步模式：关闭面板后才更新 -->
<atom:ColorPicker ValueSyncStrategy="OnCompleted" />

<!-- 启用预设色板 -->
<atom:ColorPicker IsPaletteGroupEnabled="True" />

<!-- 渐变色选择器 -->
<atom:GradientColorPicker IsShowText="True" />

<!-- 禁用状态 -->
<atom:ColorPicker IsEnabled="False" />

<!-- 警告/错误状态 -->
<atom:ColorPicker Status="Warning" IsShowText="True" />
<atom:ColorPicker Status="Error" IsShowText="True" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/ColorPickerShowCase.axaml`

---

## 2. 自定义颜色文本格式化

通过 `ColorTextFormatter` 附加属性，可以自定义触发器上显示的颜色文本：

```csharp
// 在 C# 代码中设置自定义格式化
ColorPicker.SetColorTextFormatter(colorPicker, (color, format) =>
{
    var colorText = ColorToHexConverter.ToHexString(color, AlphaComponentPosition.Leading, false, true);
    return $"Custom Text ({colorText})";
});
```

对于 `GradientColorPicker`，自定义格式化函数接收渐变画刷和控件集合：

```csharp
GradientColorPicker.SetPresetColor(gradientPicker, (brush, format, controls) =>
{
    // 自定义渐变色文本渲染逻辑
});
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/ColorPickerShowCase.axaml.cs`

---

## 3. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 ColorPicker 进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|ColorPicker">
        <Setter Property="Margin" Value="5" />
    </Style>
</Window.Styles>
```

### 按样式变体定制

```xml
<!-- Filled 变体的自定义背景色 -->
<Style Selector="atom|ColorPicker[StyleVariant=Filled]">
    <Setter Property="Background" Value="#f5f5f5" />
</Style>

<!-- Borderless 变体 -->
<Style Selector="atom|ColorPicker[StyleVariant=Borderless]">
    <Setter Property="BorderThickness" Value="0" />
</Style>
```

### 按状态定制

```xml
<!-- 警告状态边框色 -->
<Style Selector="atom|ColorPicker[Status=Warning]">
    <Setter Property="BorderBrush" Value="#faad14" />
</Style>

<!-- 错误状态边框色 -->
<Style Selector="atom|ColorPicker[Status=Error]">
    <Setter Property="BorderBrush" Value="#ff4d4f" />
</Style>
```

### 按尺寸定制

```xml
<!-- 大号选择器额外内边距 -->
<Style Selector="atom|ColorPicker[SizeType=Large]">
    <Setter Property="Padding" Value="12,8" />
</Style>

<!-- 小号选择器 -->
<Style Selector="atom|ColorPicker[SizeType=Small]">
    <Setter Property="Padding" Value="4,2" />
</Style>
```

### 使用伪类选择器

```xml
<!-- Flyout 打开时的样式 -->
<Style Selector="atom|ColorPicker:flyout-open">
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorPrimary}" />
</Style>

<!-- 禁用状态 -->
<Style Selector="atom|ColorPicker:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>

<!-- 悬浮状态 -->
<Style Selector="atom|ColorPicker:pointerover:not(:disabled)">
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorPrimaryHover}" />
</Style>
```

---

## 4. 通过 ControlTheme 完全重定义

如需完全重定义 ColorPicker 的视觉表现，可以创建新的 `ControlTheme`：

```xml
<ControlTheme x:Key="CustomColorPicker" TargetType="atom:ColorPicker"
              BasedOn="{StaticResource {x:Type atom:ColorPicker}}">
    <Setter Property="CornerRadius" Value="20" />
    <Setter Property="BorderThickness" Value="2" />
    <Setter Property="Background" Value="Transparent" />
</ControlTheme>

<!-- 应用自定义主题 -->
<atom:ColorPicker Theme="{StaticResource CustomColorPicker}" />
```

---

## 5. GradientColorPicker 样式定制

`GradientColorPicker` 与 `ColorPicker` 共享相同的基类样式系统，所有适用于 `AbstractColorPicker` 的样式属性均可使用：

```xml
<!-- 渐变色选择器带文本显示 -->
<atom:GradientColorPicker IsShowText="True" IsClearEnabled="True" />

<!-- 渐变色选择器不同尺寸 -->
<atom:GradientColorPicker SizeType="Large" IsShowText="True" />

<!-- 渐变色选择器不同样式变体 -->
<atom:GradientColorPicker StyleVariant="Filled" IsShowText="True" />
```

### 渐变色选择器样式覆盖

```xml
<Style Selector="atom|GradientColorPicker">
    <Setter Property="Margin" Value="5" />
</Style>

<Style Selector="atom|GradientColorPicker:flyout-open">
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorPrimary}" />
</Style>
```

---

## 6. 在表单中使用

ColorPicker 实现了 `IFormItemAware` 接口，可直接在 Form 中使用：

```xml
<atom:Form Model="{Binding FormModel}">
    <atom:FormItem Label="主题色" Name="ThemeColor">
        <atom:ColorPicker IsShowText="True" />
    </atom:FormItem>
    <atom:FormItem Label="渐变背景" Name="GradientBg">
        <atom:GradientColorPicker IsShowText="True" />
    </atom:FormItem>
</atom:Form>
```

当表单验证失败时，FormItem 会自动将 ColorPicker 的 `Status` 设置为 `Error`，边框变为红色。

---

## 7. 在紧凑空间中使用

ColorPicker 实现了 `ICompactSpaceAware` 接口，可在 `Space.Compact` 中使用：

```xml
<atom:Space Compact="True">
    <atom:ColorPicker IsShowText="True" />
    <atom:GradientColorPicker IsShowText="True" />
</atom:Space>
```

在紧凑空间中，相邻的 ColorPicker 会自动调整圆角，形成视觉上的一体化组合。