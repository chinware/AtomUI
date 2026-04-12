# CheckBox 自定义样式指南

CheckBox 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

```xml
<!-- 基本复选框 -->
<atom:CheckBox>Checkbox</atom:CheckBox>

<!-- 默认选中 -->
<atom:CheckBox IsChecked="True">Checked</atom:CheckBox>

<!-- 不确定态 -->
<atom:CheckBox IsChecked="{x:Null}" IsThreeState="True">Indeterminate</atom:CheckBox>

<!-- 禁用状态 -->
<atom:CheckBox IsEnabled="False">Disabled</atom:CheckBox>
<atom:CheckBox IsChecked="True" IsEnabled="False">Checked Disabled</atom:CheckBox>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/CheckBoxShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|CheckBox">
        <Setter Property="Margin" Value="0,0,5,5" />
    </Style>
</Window.Styles>
```

### 自定义选中态指示器颜色

```xml
<!-- 选中态使用绿色 -->
<Style Selector="atom|CheckBox:checked /template/ atom|CheckBoxIndicator#Indicator">
    <Setter Property="Background" Value="#52c41a" />
    <Setter Property="BorderBrush" Value="#52c41a" />
</Style>
```

### 使用伪类选择器

```xml
<!-- 悬浮时加粗文字 -->
<Style Selector="atom|CheckBox:pointerover">
    <Setter Property="FontWeight" Value="Medium" />
</Style>

<!-- 禁用态降低不透明度 -->
<Style Selector="atom|CheckBox:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 CheckBox 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomCheckBox" TargetType="atom:CheckBox">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    Padding="{TemplateBinding Padding}">
                <DockPanel>
                    <atom:CheckBoxIndicator Name="Indicator"
                                            DockPanel.Dock="Left"
                                            IsMotionEnabled="{TemplateBinding IsMotionEnabled}"
                                            IsWaveSpiritEnabled="{TemplateBinding IsWaveSpiritEnabled}"
                                            IsEnabled="{TemplateBinding IsEnabled}"
                                            State="{TemplateBinding IsChecked, Converter={StaticResource CheckBoxIndicatorStateConverter}}" />
                    <ContentPresenter Name="ContentPresenter"
                                      VerticalAlignment="Center"
                                      Content="{TemplateBinding Content}" />
                </DockPanel>
            </Border>
        </ControlTemplate>
    </Setter>
    <!-- 自定义样式... -->
</ControlTheme>

<!-- 使用 -->
<atom:CheckBox Theme="{StaticResource MyCustomCheckBox}">自定义复选框</atom:CheckBox>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的波纹效果、选中动画等功能。建议优先使用 Style 覆盖。

---

## 4. 控制动画行为

```xml
<!-- 禁用波纹效果 -->
<atom:CheckBox IsWaveSpiritEnabled="False">No Wave</atom:CheckBox>

<!-- 禁用过渡动画 -->
<atom:CheckBox IsMotionEnabled="False">No Animation</atom:CheckBox>

<!-- 通过全局 Style 统一禁用波纹 -->
<!-- <Style Selector="atom|CheckBox">
    <Setter Property="IsWaveSpiritEnabled" Value="False" />
</Style> -->
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|CheckBox` 语法引用 `atom` XML 命名空间下的类型。

### CheckBox 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|CheckBox` | 匹配所有 CheckBox 实例 |
| `atom\|CheckBox:checked` | 匹配选中状态 |
| `atom\|CheckBox:unchecked` | 匹配未选中状态 |
| `atom\|CheckBox:indeterminate` | 匹配不确定态（半选） |
| `atom\|CheckBox:pointerover` | 匹配鼠标悬浮状态 |
| `atom\|CheckBox:pressed` | 匹配按下状态 |
| `atom\|CheckBox:disabled` | 匹配禁用状态 |
| `atom\|CheckBox:focus-visible` | 匹配键盘焦点状态 |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|CheckBox /template/ atom\|CheckBoxIndicator#Indicator` | 访问选中指示器 |
| `atom\|CheckBox /template/ ContentPresenter#ContentPresenter` | 访问文本内容区域 |

### 组合选择器

| 选择器 | 说明 |
|---|---|
| `atom\|CheckBox:checked /template/ atom\|CheckBoxIndicator#Indicator` | 选中态的指示器（蓝色背景） |
| `atom\|CheckBox:checked:pointerover /template/ atom\|CheckBoxIndicator#Indicator` | 选中态悬浮的指示器 |
| `atom\|CheckBox:not(:checked):pointerover /template/ atom\|CheckBoxIndicator#Indicator` | 未选中态悬浮的指示器（边框变蓝） |
| `atom\|CheckBox:disabled:checked /template/ atom\|CheckBoxIndicator#Indicator` | 禁用选中态的指示器 |

### CheckBoxGroup 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|CheckBoxGroup` | 匹配所有 CheckBoxGroup 实例 |
