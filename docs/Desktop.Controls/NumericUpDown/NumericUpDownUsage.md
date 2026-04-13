# NumericUpDown 使用文档

本文档介绍 AtomUI NumericUpDown 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/NumberUpDownShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 NumericUpDown，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // NumericUpDown 控件
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本用法

最简单的数字输入框，直接设置初始值：

```xml
<atom:NumericUpDown Value="3" HorizontalAlignment="Stretch" />
```

默认配置：
- 尺寸：`Middle`（32px 高度）
- 样式：`Outline`（标准边框）
- 步长：`1`
- 范围：`decimal.MinValue` ~ `decimal.MaxValue`

---

## 2. 高精度字符串模式（StringMode）

当需要处理超出 `decimal` 精度范围的数值时，启用 `StringMode`：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:NumericUpDown StringMode="True"
                        StringValue="{Binding StringModeValue, Mode=TwoWay}"
                        Increment="0.0001"
                        Minimum="0"
                        Maximum="100"
                        PlaceholderText="Input weight" />
    <TextBlock Text="{Binding StringModeValue, StringFormat=Raw: {0}}" />
</StackPanel>
```

```csharp
// ViewModel
public class NumberUpDownViewModel : ReactiveObject
{
    private string? _stringModeValue = "0.123456789012345678901234";
    public string? StringModeValue
    {
        get => _stringModeValue;
        set => this.RaiseAndSetIfChanged(ref _stringModeValue, value);
    }
}
```

**使用场景**：科学计算、财务高精度场景，数值以原始字符串形式传输，避免精度丢失。

---

## 3. 键盘控制

通过 `Keyboard` 属性控制是否允许使用方向键步进：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:CheckBox Content="Keyboard enabled"
                   IsChecked="{Binding KeyboardEnabled}" />
    <atom:NumericUpDown Value="1.2"
                        Increment="0.1"
                        Keyboard="{Binding KeyboardEnabled}"
                        PlaceholderText="Keyboard disabled" />
</StackPanel>
```

```csharp
private bool _keyboardEnabled = true;
public bool KeyboardEnabled
{
    get => _keyboardEnabled;
    set => this.RaiseAndSetIfChanged(ref _keyboardEnabled, value);
}
```

当 `Keyboard="False"` 时，Up/Down/PageUp/PageDown 键不会触发数值增减。

---

## 4. 鼠标滚轮控制

通过 `MouseWheel` 属性控制是否允许使用鼠标滚轮步进：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:CheckBox Content="Mouse wheel enabled"
                   IsChecked="{Binding MouseWheelEnabled}" />
    <atom:NumericUpDown Value="2.5"
                        Increment="0.5"
                        MouseWheel="{Binding MouseWheelEnabled}"
                        PlaceholderText="Mouse wheel disabled" />
</StackPanel>
```

```csharp
private bool _mouseWheelEnabled = true;
public bool MouseWheelEnabled
{
    get => _mouseWheelEnabled;
    set => this.RaiseAndSetIfChanged(ref _mouseWheelEnabled, value);
}
```

---

## 5. 取值范围限制

通过 `Minimum` 和 `Maximum` 属性限制输入范围。步进按钮会自动感知边界并在到达极值时禁用：

```xml
<atom:NumericUpDown Minimum="0"
                    Maximum="10"
                    Value="3"
                    Increment="1" />
```

当 `Value = 10` 时，增加按钮自动禁用；当 `Value = 0` 时，减少按钮自动禁用。

---

## 6. 小数步长

通过 `Increment` 属性设置小数步长：

```xml
<atom:NumericUpDown Value="1.2"
                    Increment="0.25"
                    Minimum="0"
                    Maximum="2" />
```

每次点击步进按钮，数值增加或减少 `0.25`。

---

## 7. 三种尺寸

通过 `SizeType` 属性设置尺寸，支持 `Large`、`Middle`（默认）、`Small` 三种：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:NumericUpDown SizeType="Large" Value="3" />
    <atom:NumericUpDown SizeType="Middle" Value="3" />
    <atom:NumericUpDown SizeType="Small" Value="3" />
</StackPanel>
```

| 尺寸 | 高度 | 字体大小 |
|---|---|---|
| `Large` | 40px | `InputFontSizeLG` |
| `Middle` | 32px | `InputFontSize` |
| `Small` | 24px | `InputFontSizeSM` |

---

## 8. 三种样式变体

通过 `StyleVariant` 属性设置样式变体：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:NumericUpDown Value="3" StyleVariant="Outline" />
    <atom:NumericUpDown Value="3" StyleVariant="Filled" />
    <atom:NumericUpDown Value="3" StyleVariant="Borderless" />
</StackPanel>
```

- **Outline**：标准边框样式（默认）
- **Filled**：灰色填充背景
- **Borderless**：无边框，适合嵌入其他控件内

---

## 9. 禁用状态

通过 `IsEnabled="False"` 禁用输入框，所有变体均支持：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:NumericUpDown Value="3" StyleVariant="Outline" IsEnabled="False" />
    <atom:NumericUpDown Value="3" StyleVariant="Filled" IsEnabled="False" />
    <atom:NumericUpDown Value="3" StyleVariant="Borderless" IsEnabled="False" />
</StackPanel>
```

禁用状态下：输入框不可编辑、步进按钮不可点击、整体使用灰色调。

---

## 10. 前后置标签（AddOn）

通过 `LeftAddOn` 和 `RightAddOn` 属性在输入框外部添加附加内容：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 文字标签 -->
    <atom:NumericUpDown LeftAddOn="http://" RightAddOn=".com"
                        Width="400" Value="3" />

    <!-- 图标标签 -->
    <atom:NumericUpDown
        RightAddOn="{antdicons:AntDesignIconProvider Kind=SettingOutlined}"
        Width="400" Value="3" />

    <!-- 混合：左侧文字 + 右侧内部内容 -->
    <atom:NumericUpDown LeftAddOn="http://" InnerRightContent=".com"
                        Width="400" Value="3" />
</StackPanel>
```

AddOn 是输入框**外部**的独立区域，有独立的背景色和边框，适合显示协议前缀、域名后缀等。

---

## 11. 清除按钮

通过 `IsAllowClear="True"` 启用清除按钮：

```xml
<atom:NumericUpDown PlaceholderText="input with clear icon"
                    Width="400"
                    IsAllowClear="True" />
```

- 输入框非空且非只读时，右侧显示清除图标（默认 `CloseCircleFilled`）
- 点击清除图标将 `Value` 设为 `null`
- 可通过 `ClearIcon` 属性自定义图标

---

## 12. 内部前后置内容（Prefix / Suffix）

通过 `InnerLeftContent` 和 `InnerRightContent` 属性在输入框**内部**添加图标或文本：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 图标前后缀 -->
    <atom:NumericUpDown PlaceholderText="Enter your value"
        InnerLeftContent="{antdicons:AntDesignIconProvider Kind=UserOutlined, FillBrush=#D7D7D7}"
        InnerRightContent="{antdicons:AntDesignIconProvider Kind=InfoCircleOutlined, FillBrush=#8C8C8C}" />

    <!-- 文本前后缀 -->
    <atom:NumericUpDown InnerLeftContent="￥" InnerRightContent="RMB" />

    <!-- 禁用状态下的前后缀 -->
    <atom:NumericUpDown InnerLeftContent="￥" InnerRightContent="RMB" IsEnabled="False" />
</StackPanel>
```

与 AddOn 的区别：`InnerLeftContent` / `InnerRightContent` 嵌入在输入框**内部**，没有独立背景，适合显示货币符号、单位等。

---

## 13. 验证状态

通过 `Status` 属性设置错误或警告状态，可与所有变体组合：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- Outline 变体 -->
    <atom:NumericUpDown PlaceholderText="Error" Status="Error" />
    <atom:NumericUpDown PlaceholderText="Warning" Status="Warning" />

    <!-- 带前缀图标的状态 -->
    <atom:NumericUpDown PlaceholderText="Error with prefix"
        InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
        Status="Error" />
    <atom:NumericUpDown PlaceholderText="Warning with prefix"
        InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
        Status="Warning" />

    <!-- Filled 变体 -->
    <atom:NumericUpDown PlaceholderText="Error" Status="Error"
        InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
        StyleVariant="Filled" />
    <atom:NumericUpDown PlaceholderText="Warning" Status="Warning"
        InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
        StyleVariant="Filled" />

    <!-- Borderless 变体 -->
    <atom:NumericUpDown PlaceholderText="Error" Status="Error"
        InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
        StyleVariant="Borderless" />
    <atom:NumericUpDown PlaceholderText="Warning" Status="Warning"
        InnerLeftContent="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
        StyleVariant="Borderless" />
</StackPanel>
```

验证状态通过颜色变化提供视觉反馈：
- **Error**：边框/底部线变为红色系（`ColorError`）
- **Warning**：边框/底部线变为黄色系（`ColorWarning`）

---

## 常见组合模式

### 表单中的数值输入

```xml
<atom:FormItem Label="Age" IsRequired="True">
    <atom:NumericUpDown Minimum="0" Maximum="150" Increment="1"
                        PlaceholderText="Enter age" />
</atom:FormItem>
```

### 带单位的金额输入

```xml
<atom:NumericUpDown InnerLeftContent="￥"
                    InnerRightContent="元"
                    Minimum="0"
                    Increment="0.01"
                    FormatString="N2"
                    PlaceholderText="请输入金额" />
```

### 紧凑空间中的数值输入

```xml
<atom:Space Compact="True">
    <atom:NumericUpDown Value="100" />
    <atom:NumericUpDown Value="200" />
    <atom:NumericUpDown Value="300" />
</atom:Space>
```

### MVVM 值绑定

```xml
<atom:NumericUpDown Value="{Binding Quantity}"
                    Minimum="{Binding MinQuantity}"
                    Maximum="{Binding MaxQuantity}"
                    Increment="1" />
```

```csharp
// ViewModel
[Reactive]
public decimal? Quantity { get; set; } = 1;
public decimal MinQuantity => 0;
public decimal MaxQuantity => 999;
```
