# ColorPicker 使用文档

本文档介绍 AtomUI ColorPicker 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/ColorPickerShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 ColorPicker，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // ColorPicker、GradientColorPicker
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本用法

最简单的颜色选择器，点击触发器即可弹出颜色面板：

```xml
<atom:ColorPicker DefaultValue="#1677ff"/>
```

- `DefaultValue` 设置初始颜色值
- 点击触发器弹出 Flyout 面板，面板中包含色谱、色相滑块、输入框等

---

## 2. 触发器尺寸

通过 `SizeType` 属性设置触发器尺寸，支持 `Small`、`Middle`（默认）、`Large` 三种：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 小号 -->
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:ColorPicker DefaultValue="#1677ff" SizeType="Small"/>
        <atom:ColorPicker DefaultValue="#1677ff" SizeType="Small" IsShowText="True"/>
    </StackPanel>

    <!-- 中号（默认） -->
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:ColorPicker DefaultValue="#1677ff" SizeType="Middle"/>
        <atom:ColorPicker DefaultValue="#1677ff" SizeType="Middle" IsShowText="True"/>
    </StackPanel>

    <!-- 大号 -->
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:ColorPicker DefaultValue="#1677ff" SizeType="Large"/>
        <atom:ColorPicker DefaultValue="#1677ff" SizeType="Large" IsShowText="True"/>
    </StackPanel>
</StackPanel>
```

**提示**：尺寸影响触发器色块的大小和内间距，不影响 Flyout 面板的大小。

---

## 3. 受控模式

通过 `ValueSyncStrategy` 属性控制颜色值的同步时机：

| 模式 | 说明 |
|---|---|
| `Immediate`（默认） | 拖动滑块时实时更新 `Value`，触发 `ValueChanged` 事件 |
| `OnCompleted` | 仅在关闭 Flyout 面板时更新 `Value`，触发 `ValueSelected` 事件 |

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <!-- 实时同步（默认） -->
    <atom:ColorPicker DefaultValue="#1677ff" />

    <!-- 关闭面板后同步 -->
    <atom:ColorPicker DefaultValue="#1677ff" ValueSyncStrategy="OnCompleted"/>
</StackPanel>
```

**使用场景**：
- `Immediate`：需要实时预览颜色效果的场景（如实时修改背景色）
- `OnCompleted`：只在用户确认选择后才生效的场景（如提交表单）

---

## 4. 渐变色选择

使用 `GradientColorPicker` 选取线性渐变色，支持多色标（GradientStop）编辑：

```xml
<atom:GradientColorPicker IsShowText="True">
    <atom:GradientColorPicker.DefaultValue>
        <LinearGradientBrush>
            <GradientStop Color="#108ee9" Offset="0"/>
            <GradientStop Color="#87d068" Offset="1"/>
        </LinearGradientBrush>
    </atom:GradientColorPicker.DefaultValue>
</atom:GradientColorPicker>
```

**渐变色选择器面板功能**：
- 渐变预览条：显示当前渐变效果
- 色标指示器：可拖动调整位置，点击选中编辑颜色
- 添加/删除色标
- 色谱和滑块：编辑选中色标的颜色

> 📖 与 Ant Design 的区别：Ant Design 通过 `mode` 属性切换纯色/渐变模式，AtomUI 将其拆分为两个独立控件（`ColorPicker` 和 `GradientColorPicker`），更符合 Avalonia 的类型系统。

---

## 5. 触发器文本

通过 `IsShowText="True"` 在触发器上显示颜色文本值：

```xml
<atom:ColorPicker DefaultValue="#1677ff" IsShowText="True" IsClearEnabled="True"/>
```

### 自定义文本格式化

通过 `ColorTextFormatter` 附加属性自定义触发器文本渲染：

```xml
<atom:ColorPicker Name="CustomRenderText" DefaultValue="#1677ff" IsShowText="True" />
```

```csharp
// 在 Code-behind 或 ViewModel 中设置
var colorPicker = this.FindControl<ColorPicker>("CustomRenderText");
ColorPicker.SetColorTextFormatter(colorPicker, (color, format) =>
{
    var hex = ColorToHexConverter.ToHexString(color, AlphaComponentPosition.Leading, false, true);
    return $"🎨 {hex}";
});
```

**文本内容随 `Format` 属性变化**：
- `Hex`：显示 `#1677ff`
- `Hsva`：显示 `hsva(217, 100%, 100%, 1.00)`
- `Rgba`：显示 `rgba(22, 119, 255, 1.00)`

---

## 6. 禁用状态

通过 `IsEnabled="False"` 禁用颜色选择器：

```xml
<atom:ColorPicker DefaultValue="#1677ff" IsShowText="True" IsEnabled="False"/>
```

禁用时触发器变灰，不可点击，Flyout 不会弹出。

---

## 7. 禁用 Alpha 通道

通过 `IsAlphaEnabled="False"` 隐藏透明度滑块，仅允许选取不透明色：

```xml
<atom:ColorPicker DefaultValue="#1677ff" IsAlphaEnabled="False"/>
```

禁用 Alpha 通道后：
- Flyout 面板中不显示透明度滑块
- 输入区域中不显示 Alpha 输入框
- 颜色值的 Alpha 分量始终为 1.0

---

## 8. 清除颜色

通过 `IsClearEnabled="True"` 允许用户清除已选颜色：

```xml
<!-- 纯色选择器 -->
<atom:ColorPicker DefaultValue="#1677ff" IsShowText="True" IsClearEnabled="True"/>

<!-- 渐变色选择器 -->
<atom:GradientColorPicker IsShowText="True" IsClearEnabled="True">
    <atom:GradientColorPicker.DefaultValue>
        <LinearGradientBrush>
            <GradientStop Color="#108ee9" Offset="0"/>
            <GradientStop Color="#87d068" Offset="1"/>
        </LinearGradientBrush>
    </atom:GradientColorPicker.DefaultValue>
</atom:GradientColorPicker>
```

清除颜色后：
- 触发器色块显示透明棋盘格背景（表示无颜色）
- `Value` 变为 `null`
- 可通过 `EmptyColorText` 属性自定义空状态文本

---

## 9. 触发方式

通过 `TriggerType` 属性设置 Flyout 的触发方式：

| 触发方式 | 说明 |
|---|---|
| `Click`（默认） | 点击触发器弹出面板 |
| `Hover` | 鼠标悬浮触发器弹出面板 |

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 悬浮触发 -->
    <atom:ColorPicker DefaultValue="#1677ff" TriggerType="Hover"/>

    <!-- 点击触发（默认） -->
    <atom:ColorPicker DefaultValue="#1677ff" TriggerType="Click"/>
</StackPanel>
```

---

## 10. 颜色格式

通过 `Format` 属性设置颜色编码格式，支持 `Hex`（默认）、`Hsva`、`Rgba`：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- HEX 格式 -->
    <atom:ColorPicker DefaultValue="#1677ff" Format="Hex" IsShowText="True"/>

    <!-- HSVA 格式 -->
    <atom:ColorPicker DefaultValue="#1677ff" Format="Hsva" IsShowText="True"/>

    <!-- RGBA 格式 -->
    <atom:ColorPicker DefaultValue="#1677ff" Format="Rgba" IsShowText="True"/>
</StackPanel>
```

**格式影响范围**：
- 触发器文本显示（当 `IsShowText=True`）
- Flyout 面板中输入框的显示格式

当 `IsFormatEnabled="True"`（默认）时，用户可在面板中点击格式切换按钮自由切换格式。

---

## 11. 预设色板

通过 `IsPaletteGroupEnabled="True"` 启用预设色板，面板底部会显示 Ant Design 标准色板：

```xml
<!-- 纯色选择器 + 预设色板 -->
<atom:ColorPicker DefaultValue="#1677ff" IsPaletteGroupEnabled="True"/>

<!-- 渐变色选择器 + 预设色板 -->
<atom:GradientColorPicker IsPaletteGroupEnabled="True">
    <atom:GradientColorPicker.DefaultValue>
        <LinearGradientBrush>
            <GradientStop Color="#108ee9" Offset="0"/>
            <GradientStop Color="#87d068" Offset="1"/>
        </LinearGradientBrush>
    </atom:GradientColorPicker.DefaultValue>
</atom:GradientColorPicker>
```

### 自定义预设色板

通过 `PaletteGroup` 属性设置自定义预设色组：

```csharp
var customPalettes = new List<ColorPickerPalette>
{
    new("品牌色", new List<Color>
    {
        Color.FromRgb(0x16, 0x77, 0xff),
        Color.FromRgb(0x09, 0x58, 0xf9),
        Color.FromRgb(0x3c, 0x86, 0xff),
    }),
    new("主题色", new List<Color>
    {
        Color.FromRgb(0x52, 0xc4, 0x1a),
        Color.FromRgb(0xfa, 0xad, 0x14),
        Color.FromRgb(0xff, 0x4d, 0x4f),
    }),
};
colorPicker.PaletteGroup = customPalettes;
```

---

## 12. 样式变体

通过 `StyleVariant` 属性设置触发器的样式变体，与 Input 控件的样式系统对齐：

```xml
<!-- Outlined（默认，描边样式） -->
<atom:ColorPicker StyleVariant="Outlined" IsShowText="True" DefaultValue="#1677ff"/>

<!-- Filled（填充样式） -->
<atom:ColorPicker StyleVariant="Filled" IsShowText="True" DefaultValue="#1677ff"/>

<!-- Borderless（无边框样式） -->
<atom:ColorPicker StyleVariant="Borderless" IsShowText="True" DefaultValue="#1677ff"/>
```

---

## 13. 输入状态

通过 `Status` 属性设置输入控件状态，用于表单验证反馈：

```xml
<!-- 默认状态 -->
<atom:ColorPicker Status="Default" IsShowText="True" DefaultValue="#1677ff"/>

<!-- 警告状态 -->
<atom:ColorPicker Status="Warning" IsShowText="True" DefaultValue="#1677ff"/>

<!-- 错误状态 -->
<atom:ColorPicker Status="Error" IsShowText="True" DefaultValue="#1677ff"/>
```

**使用场景**：通常由 FormItem 自动管理，无需手动设置。当表单验证失败时，FormItem 会自动将 ColorPicker 的 `Status` 设为 `Error`。

---

## 14. MVVM 数据绑定

ColorPicker 支持标准的 Avalonia 数据绑定模式：

### 绑定颜色值

```xml
<!-- 双向绑定 Color 值 -->
<atom:ColorPicker Value="{Binding ThemeColor}" IsShowText="True" />

<!-- 绑定渐变色值 -->
<atom:GradientColorPicker Value="{Binding GradientBackground}" IsShowText="True" />
```

```csharp
// ViewModel（使用 ReactiveUI）
[Reactive]
public Color? ThemeColor { get; set; } = Color.FromRgb(0x16, 0x77, 0xff);

[Reactive]
public LinearGradientBrush? GradientBackground { get; set; }
```

### 监听颜色变化

```xml
<atom:ColorPicker DefaultValue="#1677ff"
                   ValueChanged="HandleColorChanged"
                   ValueSelected="HandleColorSelected" />
```

```csharp
// Code-behind
public void HandleColorChanged(object? sender, ColorChangedEventArgs e)
{
    // 实时响应颜色变化（Immediate 模式下拖动时持续触发）
    var newColor = e.NewColor;
    PreviewBorder.Background = new SolidColorBrush(newColor ?? Colors.Transparent);
}

public void HandleColorSelected(object? sender, ColorSelectedEventArgs e)
{
    // 用户确认选择（Flyout 关闭时触发）
    var selectedColor = e.SelectedColor;
    ApplyColor(selectedColor);
}
```

### 命令绑定

```xml
<atom:ColorPicker DefaultValue="#1677ff"
                   ValueChangedCommand="{Binding ColorChangedCommand}" />
```

---

## 15. Flyout 弹出位置

通过 `Placement` 属性控制 Flyout 的弹出位置：

```xml
<!-- 下方弹出（默认） -->
<atom:ColorPicker Placement="Bottom" DefaultValue="#1677ff"/>

<!-- 右侧弹出 -->
<atom:ColorPicker Placement="Right" DefaultValue="#1677ff"/>

<!-- 上方弹出 -->
<atom:ColorPicker Placement="Top" DefaultValue="#1677ff"/>

<!-- 左侧弹出 -->
<atom:ColorPicker Placement="Left" DefaultValue="#1677ff"/>
```

---

## 16. 控制动画行为

```xml
<!-- 禁用过渡动画（边框色不再渐变过渡） -->
<atom:ColorPicker IsMotionEnabled="False" DefaultValue="#1677ff"/>
```

---

## 常见组合模式

### 表单集成

ColorPicker 实现了 `IFormItemAware` 接口，可直接在 Form 中使用：

```xml
<atom:Form Model="{Binding SettingsModel}">
    <atom:FormItem Label="主题色" Name="ThemeColor">
        <atom:ColorPicker IsShowText="True" />
    </atom:FormItem>
    <atom:FormItem Label="渐变背景" Name="GradientBg">
        <atom:GradientColorPicker IsShowText="True" />
    </atom:FormItem>
</atom:Form>
```

### 紧凑空间

ColorPicker 实现了 `ICompactSpaceAware` 接口，可在 `Space.Compact` 中使用：

```xml
<atom:Space Compact="True">
    <atom:ColorPicker IsShowText="True" />
    <atom:Button ButtonType="Primary">Apply</atom:Button>
</atom:Space>
```

### 实时预览

结合 `ValueSyncStrategy="Immediate"` 和 `ValueChanged` 事件实现实时预览：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:ColorPicker DefaultValue="#1677ff"
                       ValueSyncStrategy="Immediate"
                       ValueChanged="HandleColorChanged" />
    <Border Name="PreviewBorder" Width="200" Height="100"
            Background="#1677ff" CornerRadius="8" />
</StackPanel>
```

```csharp
public void HandleColorChanged(object? sender, ColorChangedEventArgs e)
{
    PreviewBorder.Background = new SolidColorBrush(e.NewColor ?? Colors.Transparent);
}