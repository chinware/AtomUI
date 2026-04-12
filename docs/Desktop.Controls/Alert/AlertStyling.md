# Alert 自定义样式指南

Alert 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Alert 的公共属性来控制外观：

```xml
<!-- 四种类型 -->
<atom:Alert Type="Success">Operation completed successfully.</atom:Alert>
<atom:Alert Type="Info">This is an informational message.</atom:Alert>
<atom:Alert Type="Warning">Please be careful with this operation.</atom:Alert>
<atom:Alert Type="Error">An error has occurred.</atom:Alert>

<!-- 带图标 -->
<atom:Alert Type="Success" IsShowIcon="True">Success with icon</atom:Alert>

<!-- 可关闭 -->
<atom:Alert Type="Warning" IsClosable="True">Closable warning</atom:Alert>

<!-- 带描述 -->
<atom:Alert Type="Info" IsShowIcon="True"
            Message="Information"
            Description="Additional description and information about this alert." />

<!-- 带额外操作 -->
<atom:Alert Type="Info" IsShowIcon="True" IsClosable="True"
            Message="Information">
    <atom:Alert.ExtraAction>
        <atom:Button ButtonType="Text" SizeType="Small">Detail</atom:Button>
    </atom:Alert.ExtraAction>
</atom:Alert>

<!-- 跑马灯 -->
<atom:Alert Type="Warning" IsMessageMarqueEnabled="True" IsShowIcon="True">
    Very long message that will scroll automatically
</atom:Alert>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/AlertShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Alert 进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|Alert">
        <Setter Property="Margin" Value="0,0,0,16" />
    </Style>
</Window.Styles>
```

### 按类型定制

```xml
<!-- 错误类型加粗显示 -->
<Style Selector="atom|Alert[Type=Error]">
    <Setter Property="FontWeight" Value="Bold" />
</Style>

<!-- Warning 类型自定义背景 -->
<Style Selector="atom|Alert[Type=Warning]">
    <Setter Property="Background" Value="#fff7e6" />
    <Setter Property="BorderBrush" Value="#ffd591" />
</Style>
```

### 使用伪类选择器

```xml
<!-- 带描述的 Alert 添加额外下边距 -->
<Style Selector="atom|Alert:has-description">
    <Setter Property="Margin" Value="0,0,0,24" />
</Style>

<!-- 带额外操作的 Alert 最小高度 -->
<Style Selector="atom|Alert:has-extra-action">
    <Setter Property="MinHeight" Value="60" />
</Style>
```

### 定制内部元素

```xml
<!-- 自定义关闭按钮大小 -->
<Style Selector="atom|Alert /template/ atom|IconButton#PART_CloseBtn">
    <Setter Property="IconWidth" Value="14" />
    <Setter Property="IconHeight" Value="14" />
</Style>

<!-- 自定义描述文本样式 -->
<Style Selector="atom|Alert /template/ Label#DescriptionLabel">
    <Setter Property="Opacity" Value="0.75" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Alert 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomAlert" TargetType="atom:Alert">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    Padding="{TemplateBinding Padding}">
                <TextBlock Text="{TemplateBinding Message}" />
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<atom:Alert Theme="{StaticResource MyCustomAlert}" Type="Info">
    Custom styled alert
</atom:Alert>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的图标自动匹配、关闭按钮、描述信息等功能。建议优先使用 Style 覆盖。

---

## 4. 处理关闭事件

Alert 的关闭按钮不会自动移除控件，需在事件处理中手动移除：

```xml
<atom:Alert Type="Warning" IsClosable="True" CloseRequest="HandleAlertClose">
    This alert can be closed.
</atom:Alert>
```

```csharp
private void HandleAlertClose(object? sender, EventArgs e)
{
    if (sender is Alert alert && alert.Parent is Panel panel)
    {
        panel.Children.Remove(alert);
    }
}
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Alert` 语法引用 `atom` XML 命名空间下的 `Alert` 类型，其中 `|` 是命名空间分隔符。

### 按类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|Alert` | 匹配所有 Alert 实例，用于设置全局通用样式 |
| `atom\|Alert[Type=Success]` | 匹配成功类型（绿色系），可定制背景色、边框色 |
| `atom\|Alert[Type=Info]` | 匹配信息类型（蓝色系） |
| `atom\|Alert[Type=Warning]` | 匹配警告类型（橙色系） |
| `atom\|Alert[Type=Error]` | 匹配错误类型（红色系） |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|Alert[IsClosable=True]` | 匹配可关闭的 Alert |
| `atom\|Alert[IsShowIcon=True]` | 匹配显示图标的 Alert |
| `atom\|Alert[IsMessageMarqueEnabled=True]` | 匹配启用跑马灯的 Alert |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|Alert:has-description` | 匹配带描述信息的 Alert，此时内间距增大、图标变大、消息字号变大 |
| `atom\|Alert:has-extra-action` | 匹配带额外操作区的 Alert |
| `atom\|Alert:disabled` | 匹配禁用状态的 Alert |

### 模板内部元素选择

| 选择器 | 说明 |
|---|---|
| `atom\|Alert /template/ DockPanel#RootLayout` | 根布局面板 |
| `atom\|Alert /template/ atom\|IconButton#PART_CloseBtn` | 关闭按钮 |
| `atom\|Alert /template/ Label#MessageLabel` | 消息文本标签 |
| `atom\|Alert /template/ Label#DescriptionLabel` | 描述文本标签 |
| `atom\|Alert /template/ atom\|MarqueeLabel#MarqueeLabel` | 跑马灯标签 |
| `atom\|Alert /template/ atom\|Icon#SuccessIcon` | Success 类型图标 |
| `atom\|Alert /template/ atom\|Icon#InfoIcon` | Info 类型图标 |
| `atom\|Alert /template/ atom\|Icon#WarningIcon` | Warning 类型图标 |
| `atom\|Alert /template/ atom\|Icon#ErrorIcon` | Error 类型图标 |
| `atom\|Alert /template/ ContentPresenter#ExtraActionPresenter` | 额外操作区内容展示器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Alert[Type=Error]:has-description` | 带描述信息的错误类型 Alert |
| `atom\|Alert[Type=Info][IsClosable=True]` | 可关闭的信息类型 Alert |
| `atom\|Alert:has-description /template/ atom\|Icon` | 带描述时的图标（此时图标更大，垂直顶部对齐） |
