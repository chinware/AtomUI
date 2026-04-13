# OptionButtonGroup 自定义样式指南

OptionButtonGroup 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 OptionButtonGroup 的公共属性来控制外观：

```xml
<!-- 两种按钮样式 -->
<atom:OptionButtonGroup ButtonStyle="Outline">
    <atom:OptionButton IsChecked="True">Apple</atom:OptionButton>
    <atom:OptionButton>Pear</atom:OptionButton>
    <atom:OptionButton>Orange</atom:OptionButton>
</atom:OptionButtonGroup>

<atom:OptionButtonGroup ButtonStyle="Solid">
    <atom:OptionButton IsChecked="True">Apple</atom:OptionButton>
    <atom:OptionButton>Pear</atom:OptionButton>
    <atom:OptionButton>Orange</atom:OptionButton>
</atom:OptionButtonGroup>

<!-- 三种尺寸 -->
<atom:OptionButtonGroup SizeType="Large">...</atom:OptionButtonGroup>
<atom:OptionButtonGroup SizeType="Middle">...</atom:OptionButtonGroup>
<atom:OptionButtonGroup SizeType="Small">...</atom:OptionButtonGroup>

<!-- 带图标 -->
<atom:OptionButtonGroup ButtonStyle="Solid">
    <atom:OptionButton IsChecked="True"
        Icon="{antdicons:AntDesignIconProvider Kind=AppleOutlined}">macOS</atom:OptionButton>
    <atom:OptionButton
        Icon="{antdicons:AntDesignIconProvider Kind=LinuxOutlined}">Linux</atom:OptionButton>
    <atom:OptionButton
        Icon="{antdicons:AntDesignIconProvider Kind=WindowsOutlined}">Windows</atom:OptionButton>
</atom:OptionButtonGroup>

<!-- 禁用单个选项 -->
<atom:OptionButtonGroup>
    <atom:OptionButton IsChecked="True">Apple</atom:OptionButton>
    <atom:OptionButton IsEnabled="False">Pear</atom:OptionButton>
    <atom:OptionButton>Orange</atom:OptionButton>
</atom:OptionButtonGroup>

<!-- 禁用整个组 -->
<atom:OptionButtonGroup IsEnabled="False">...</atom:OptionButtonGroup>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/RadioButtonShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 OptionButton 或 OptionButtonGroup 进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|OptionButtonGroup">
        <Setter Property="Margin" Value="0,5" />
    </Style>
</Window.Styles>
```

### 按尺寸定制样式

```xml
<!-- 大号按钮使用粗体 -->
<Style Selector="atom|OptionButton[SizeType=Large]">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

### 自定义选中态颜色

```xml
<!-- Solid 模式选中时使用自定义颜色 -->
<Style Selector="atom|OptionButton[ButtonStyle=Solid]:checked">
    <Setter Property="Background" Value="#722ed1" />
</Style>

<!-- Outline 模式选中时使用自定义颜色 -->
<Style Selector="atom|OptionButton[ButtonStyle=Outline]:checked">
    <Setter Property="Foreground" Value="#722ed1" />
    <Setter Property="BorderBrush" Value="#722ed1" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomOptionButton" TargetType="atom:OptionButton">
    <Setter Property="Template">
        <ControlTemplate>
            <!-- 自定义模板 -->
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的波纹效果、自定义边框渲染等功能。建议优先使用 Style 覆盖。

---

## 4. 控制动画行为

```xml
<!-- 禁用过渡动画 -->
<atom:OptionButtonGroup IsMotionEnabled="False">...</atom:OptionButtonGroup>

<!-- 禁用点击波纹 -->
<atom:OptionButtonGroup IsWaveSpiritEnabled="False">...</atom:OptionButtonGroup>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|OptionButton` / `atom|OptionButtonGroup` 语法引用控件类型。

### OptionButtonGroup 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|OptionButtonGroup` | 匹配所有 OptionButtonGroup 实例 |
| `atom\|OptionButtonGroup[ButtonStyle=Solid]` | 匹配 Solid 样式的组 |
| `atom\|OptionButtonGroup[ButtonStyle=Outline]` | 匹配 Outline 样式的组 |
| `atom\|OptionButtonGroup[SizeType=Large]` | 匹配大号组 |
| `atom\|OptionButtonGroup[SizeType=Middle]` | 匹配中号组 |
| `atom\|OptionButtonGroup[SizeType=Small]` | 匹配小号组 |
| `atom\|OptionButtonGroup:disabled` | 匹配禁用态的组 |

### OptionButton 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|OptionButton` | 匹配所有 OptionButton 实例 |
| `atom\|OptionButton:checked` | 匹配选中状态 |
| `atom\|OptionButton:unchecked` | 匹配未选中状态 |
| `atom\|OptionButton:pointerover` | 匹配鼠标悬浮状态 |
| `atom\|OptionButton:pressed` | 匹配按下状态 |
| `atom\|OptionButton:disabled` | 匹配禁用状态 |
| `atom\|OptionButton[ButtonStyle=Solid]` | 匹配 Solid 模式下的按钮 |
| `atom\|OptionButton[ButtonStyle=Outline]` | 匹配 Outline 模式下的按钮 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|OptionButton[ButtonStyle=Solid]:checked:not(:disabled)` | Solid 模式下非禁用的选中按钮 |
| `atom\|OptionButton[ButtonStyle=Outline]:unchecked:pointerover` | Outline 模式下悬浮的未选中按钮 |
| `atom\|OptionButton:disabled:checked` | 禁用且选中的按钮 |
| `atom\|OptionButtonGroup /template/ Border#Frame` | 访问组模板内的外围边框 |
| `atom\|OptionButton /template/ atom\|WaveSpiritDecorator` | 访问按钮模板内的波纹装饰器 |
| `atom\|OptionButton /template/ atom\|IconPresenter#IconPresenter` | 访问按钮模板内的图标展示器 |
