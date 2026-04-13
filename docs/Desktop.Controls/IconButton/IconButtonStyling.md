# IconButton 自定义样式指南

IconButton 的视觉表现通过 `ControlTheme` + 全局 SharedToken 系统控制。由于 IconButton 是极简设计，自定义方式以属性和 Style 为主。

---

## 1. 通过属性直接控制

```xml
<!-- 自定义图标、尺寸和颜色 -->
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}"
                 IconWidth="20" IconHeight="20"
                 IconBrush="DodgerBlue" />

<!-- 禁用动画 -->
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=LoadingOutlined}"
                 IsMotionEnabled="False" />

<!-- 加载旋转动画 -->
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=SyncOutlined}"
                 LoadingAnimation="Spin" />
```

> ⚠️ 直接设置 `IconBrush` 会覆盖主题的伪类驱动颜色。如需保留悬浮/按下颜色切换，请通过 Style 分别设置各状态的 `IconBrush`。

---

## 2. 通过 Style 覆盖

### 全局调整图标尺寸

```xml
<Window.Styles>
    <Style Selector="atom|IconButton">
        <Setter Property="IconWidth" Value="18" />
        <Setter Property="IconHeight" Value="18" />
    </Style>
</Window.Styles>
```

### 悬浮态自定义颜色

```xml
<Style Selector="atom|IconButton:pointerover">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorPrimary}" />
</Style>
```

### 添加悬浮背景反馈

独立使用 IconButton 时（非嵌入其他控件），添加悬浮背景可增强交互反馈：

```xml
<StackPanel Orientation="Horizontal" Spacing="4">
    <StackPanel.Styles>
        <Style Selector="atom|IconButton">
            <Setter Property="Padding" Value="6" />
            <Setter Property="CornerRadius" Value="4" />
        </Style>
        <Style Selector="atom|IconButton:pointerover">
            <Setter Property="Background" Value="{atom:SharedTokenResource ColorBgTextHover}" />
        </Style>
    </StackPanel.Styles>
    <atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=AlignLeftOutlined}" />
    <atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=AlignCenterOutlined}" />
    <atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=AlignRightOutlined}" />
</StackPanel>
```

### 使用 Token 资源作为图标颜色

```xml
<!-- 使用语义色 Token -->
<Style Selector="atom|IconButton.success">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorSuccess}" />
</Style>
<Style Selector="atom|IconButton.error">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorError}" />
</Style>
<Style Selector="atom|IconButton.warning">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorWarning}" />
</Style>
```

---

## 3. 通过 ControlTheme 替换

```xml
<ControlTheme x:Key="CustomIconButton" TargetType="atom:IconButton">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    Padding="{TemplateBinding Padding}">
                <atom:IconPresenter Icon="{TemplateBinding Icon}"
                                    IconBrush="{TemplateBinding IconBrush}"
                                    Width="{TemplateBinding IconWidth}"
                                    Height="{TemplateBinding IconHeight}" />
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<atom:IconButton Theme="{StaticResource CustomIconButton}"
                 Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}" />
```

> ⚠️ 替换 ControlTheme 会丢失主题中预设的伪类颜色切换逻辑，需要自行在新 ControlTheme 中定义各状态的样式。

---

## 4. 控制动画行为

```xml
<!-- 禁用背景过渡动画 -->
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}"
                 IsMotionEnabled="False" />

<!-- 添加旋转加载动画 -->
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=LoadingOutlined}"
                 LoadingAnimation="Spin" />
```

---

## 5. 事件穿透设置

当 IconButton 嵌入其他控件模板中时，通过 `IsPassthroughMouseEvent` 控制鼠标事件是否冒泡：

```xml
<!-- 在自定义控件模板内使用，允许事件穿透到父控件 -->
<atom:IconButton Name="PART_CloseButton"
                 Icon="{antdicons:AntDesignIconProvider CloseOutlined}"
                 IsPassthroughMouseEvent="True" />
```

---

## 样式选择器速查

### 基本选择

| 选择器 | 说明 |
|---|---|
| `atom\|IconButton` | 匹配所有 IconButton 实例 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|IconButton:pointerover` | 鼠标悬浮状态，图标颜色由 `ColorIcon` → `ColorIconHover` |
| `atom\|IconButton:pressed` | 按下状态，图标颜色变为 `ColorText` |
| `atom\|IconButton:disabled` | 禁用状态，图标颜色变为 `ColorTextDisabled` |
| `atom\|IconButton:focus-visible` | 键盘焦点状态 |
| `atom\|IconButton:flyout-open` | 附加 Flyout 打开状态 |

### 模板部件访问

| 选择器 | 说明 |
|---|---|
| `atom\|IconButton /template/ Border#Frame` | 访问主框架 Border |
| `atom\|IconButton /template/ atom\|IconPresenter#IconPresenter` | 访问图标展示器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|IconButton:not(:disabled):pointerover` | 非禁用状态的悬浮 IconButton |
| `atom\|IconButton:not(:disabled):pressed` | 非禁用状态的按下 IconButton |
| `atom\|IconButton.my-class` | 匹配带有 `Classes="my-class"` 的 IconButton |

