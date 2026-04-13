# DropdownButton 自定义样式指南

DropdownButton 继承自 Button，其视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 DropdownButton 的公共属性来控制外观和行为：

```xml
<!-- 自定义触发方式和弹出位置 -->
<atom:DropdownButton TriggerType="Hover"
                     Placement="Bottom"
                     IsShowArrow="True"
                     IsPointAtCenter="True">
    Options
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<!-- 隐藏下拉指示器 -->
<atom:DropdownButton IsShowOpenIndicator="False">
    No Arrow
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<!-- 自定义下拉指示器图标 -->
<atom:DropdownButton OpenIndicator="{antdicons:AntDesignIconProvider Kind=MoreOutlined}">
    Custom Icon
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>
```

---

## 2. 通过继承 Button 的样式能力

DropdownButton 继承自 Button，所有 Button 的样式技巧均可使用：

```xml
<!-- 按钮类型 + 形状 + 尺寸 -->
<atom:DropdownButton ButtonType="Primary" Shape="Round" SizeType="Large">
    Actions
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<!-- 危险样式 -->
<atom:DropdownButton ButtonType="Primary" IsDanger="True">
    Danger Actions
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<!-- 幽灵样式（适合有色背景） -->
<Border Background="rgb(190, 200, 200)" Padding="10">
    <atom:DropdownButton ButtonType="Primary" IsGhost="True">
        Ghost Dropdown
        <!-- ... DropdownFlyout ... -->
    </atom:DropdownButton>
</Border>
```

---

## 3. 通过 Style 覆盖

### 全局统一设置

```xml
<Window.Styles>
    <Style Selector="atom|DropdownButton">
        <Setter Property="Margin" Value="5" />
        <Setter Property="TriggerType" Value="Hover" />
    </Style>
</Window.Styles>
```

### 调整悬浮延迟

```xml
<Window.Styles>
    <Style Selector="atom|DropdownButton">
        <Setter Property="MouseEnterDelay" Value="300" />
        <Setter Property="MouseLeaveDelay" Value="200" />
    </Style>
</Window.Styles>
```

### 按类型定制

```xml
<!-- Primary 类型的 DropdownButton 使用自定义背景色 -->
<Style Selector="atom|DropdownButton[ButtonType=Primary]:not(:disabled)">
    <Setter Property="Background" Value="#722ed1" />
</Style>
<Style Selector="atom|DropdownButton[ButtonType=Primary]:not(:disabled):pointerover">
    <Setter Property="Background" Value="#9254de" />
</Style>
```

### 使用伪类选择器

```xml
<!-- 加载中时调整不透明度 -->
<Style Selector="atom|DropdownButton:loading">
    <Setter Property="Opacity" Value="0.65" />
</Style>

<!-- 危险按钮加粗字体 -->
<Style Selector="atom|DropdownButton:danger:not(:disabled)">
    <Setter Property="FontWeight" Value="Bold" />
</Style>
```

---

## 4. 通过 ControlTheme 完全替换主题

如果需要彻底替换 DropdownButton 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomDropdown" TargetType="atom:DropdownButton">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    Padding="{TemplateBinding Padding}">
                <DockPanel>
                    <atom:IconPresenter DockPanel.Dock="Right"
                                        Icon="{TemplateBinding OpenIndicator}"
                                        IsVisible="{TemplateBinding IsShowOpenIndicator}" />
                    <ContentPresenter Content="{TemplateBinding Content}"
                                      HorizontalAlignment="Center"
                                      VerticalAlignment="Center" />
                </DockPanel>
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:DropdownButton Theme="{StaticResource MyCustomDropdown}">
    Custom Dropdown
</atom:DropdownButton>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的波纹效果、加载图标等功能。建议优先使用 Style 覆盖。

---

## 5. 控制下拉指示器样式

可以通过模板选择器访问下拉指示器部件：

```xml
<!-- 调整下拉指示器的尺寸 -->
<Style Selector="atom|DropdownButton /template/ atom|IconPresenter#PART_DropdownIndicator">
    <Setter Property="Width" Value="16" />
    <Setter Property="Height" Value="16" />
</Style>
```

---

## 6. 控制动画行为

```xml
<!-- 禁用过渡动画 -->
<atom:DropdownButton ButtonType="Primary" IsMotionEnabled="False">
    No Animation
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<!-- 禁用点击波纹 -->
<atom:DropdownButton ButtonType="Primary" IsWaveSpiritEnabled="False">
    No Wave
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>
```

> 📖 Gallery 示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/DropdownButtonShowCase.axaml`

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|DropdownButton` 语法引用 `atom` XML 命名空间下的 `DropdownButton` 类型，其中 `|` 是命名空间分隔符。

### 按类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|DropdownButton` | 匹配所有 DropdownButton 实例 |
| `atom\|DropdownButton[ButtonType=Primary]` | 匹配主按钮类型的下拉按钮 |
| `atom\|DropdownButton[ButtonType=Default]` | 匹配默认按钮类型的下拉按钮 |
| `atom\|DropdownButton[ButtonType=Text]` | 匹配文本按钮类型的下拉按钮 |
| `atom\|DropdownButton[ButtonType=Link]` | 匹配链接按钮类型的下拉按钮 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|DropdownButton[SizeType=Large]` | 匹配大号下拉按钮 |
| `atom\|DropdownButton[SizeType=Middle]` | 匹配中号下拉按钮（默认） |
| `atom\|DropdownButton[SizeType=Small]` | 匹配小号下拉按钮 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|DropdownButton:pointerover` | 鼠标悬浮 |
| `atom\|DropdownButton:pressed` | 按下状态 |
| `atom\|DropdownButton:disabled` | 禁用状态 |
| `atom\|DropdownButton:flyout-open` | 下拉菜单打开时 |
| `atom\|DropdownButton:loading` | 加载中状态 |
| `atom\|DropdownButton:danger` | 危险样式 |

### 模板部件选择

| 选择器 | 说明 |
|---|---|
| `atom\|DropdownButton /template/ atom\|IconPresenter#PART_DropdownIndicator` | 下拉指示器图标 |
| `atom\|DropdownButton /template/ Border#Frame` | 主框架边框 |
| `atom\|DropdownButton /template/ ContentPresenter#PART_ContentPresenter` | 内容展示器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|DropdownButton[ButtonType=Primary]:not(:disabled)` | 非禁用的主按钮下拉 |
| `atom\|DropdownButton:danger:pointerover` | 悬浮状态的危险下拉按钮 |
| `atom\|DropdownButton[SizeType=Large]:flyout-open` | 菜单打开时的大号下拉按钮 |

