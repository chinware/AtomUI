# ToggleIconButton 自定义样式指南

ToggleIconButton 的视觉表现通过 `ControlTheme` + Design Token 系统控制。由于 ToggleIconButton 是纯图标控件（无文本、无边框、无背景），其自定义主要集中在图标颜色和图标尺寸上。

---

## 1. 通过属性直接控制

最简单的方式是通过 ToggleIconButton 的公共属性来控制外观：

```xml
<!-- 基本切换图标按钮 -->
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=EyeOutlined}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=EyeInvisibleOutlined}" />

<!-- 自定义图标尺寸 -->
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}"
    IconWidth="20" IconHeight="20" />

<!-- 自定义各状态图标颜色 -->
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=StarFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=StarOutlined}"
    NormalIconBrush="Gray"
    ActiveIconBrush="DarkGoldenrod"
    SelectedIconBrush="Gold"
    DisabledIconBrush="LightGray" />
```

### 颜色属性优先级

ToggleIconButton 的颜色由主题根据当前状态自动设置 `IconBrush`。当用户显式设置了 `NormalIconBrush`、`ActiveIconBrush`、`SelectedIconBrush`、`DisabledIconBrush` 时，这些值会覆盖主题默认颜色。

| 状态 | 优先使用的属性 | 默认来源（主题） |
|---|---|---|
| 未选中正常态 | `NormalIconBrush` | `SharedToken.ColorIcon` |
| 悬浮/按下态 | `ActiveIconBrush` | `SharedToken.ColorIconHover`（悬浮）/ `SharedToken.ColorText`（按下） |
| 选中态 | `SelectedIconBrush` | 同正常态 |
| 禁用态 | `DisabledIconBrush` | `SharedToken.ColorTextDisabled` |

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 ToggleIconButton 进行全局或局部样式覆盖：

### 全局统一图标尺寸

```xml
<Window.Styles>
    <Style Selector="atom|ToggleIconButton">
        <Setter Property="IconWidth" Value="16" />
        <Setter Property="IconHeight" Value="16" />
    </Style>
</Window.Styles>
```

### 选中状态自定义颜色

```xml
<Style Selector="atom|ToggleIconButton:checked">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorPrimary}" />
</Style>
```

### 悬浮态自定义背景

```xml
<!-- 为悬浮态添加微弱背景 -->
<Style Selector="atom|ToggleIconButton:pointerover">
    <Setter Property="Background" Value="{atom:SharedTokenResource ColorBgTextHover}" />
</Style>
```

### 按选中状态组合悬浮色

```xml
<!-- 选中 + 悬浮态使用主色调亮色 -->
<Style Selector="atom|ToggleIconButton:checked:pointerover">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorPrimaryHover}" />
</Style>

<!-- 未选中 + 悬浮态使用图标悬浮色 -->
<Style Selector="atom|ToggleIconButton:unchecked:pointerover">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorIconHover}" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 ToggleIconButton 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomToggleIconButton" TargetType="atom:ToggleIconButton">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    Padding="{TemplateBinding Padding}">
                <Panel>
                    <atom:IconPresenter
                        Icon="{TemplateBinding CheckedIcon}"
                        IsVisible="{TemplateBinding IsChecked}"
                        Width="{TemplateBinding IconWidth}"
                        Height="{TemplateBinding IconHeight}"
                        IconBrush="{TemplateBinding IconBrush}" />
                    <atom:IconPresenter
                        Icon="{TemplateBinding UnCheckedIcon}"
                        IsVisible="{TemplateBinding IsChecked, Converter={x:Static BoolConverters.Not}}"
                        Width="{TemplateBinding IconWidth}"
                        Height="{TemplateBinding IconHeight}"
                        IconBrush="{TemplateBinding IconBrush}" />
                </Panel>
            </Border>
        </ControlTemplate>
    </Setter>
    <!-- 自定义样式... -->
</ControlTheme>

<!-- 使用 -->
<atom:ToggleIconButton Theme="{StaticResource MyCustomToggleIconButton}"
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}" />
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的过渡动画等功能。建议优先使用 Style 覆盖。

---

## 4. 控制动画行为

```xml
<!-- 禁用过渡动画（背景色、变换不再渐变过渡） -->
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}"
    IsMotionEnabled="False" />
```

过渡动画在 `OnLoaded` 时配置，`OnUnloaded` 时清除（避免不可见控件消耗资源）。

| 过渡属性 | 过渡类型 | 说明 |
|---|---|---|
| `Background` | `SolidColorBrushTransition` | 背景色平滑过渡 |
| `RenderTransform` | `TransformOperationsTransition` | 缩放/旋转等变换平滑过渡 |

---

## 5. 颜色状态映射

主题默认使用 SharedToken 中的图标色系：

| 状态 | 伪类 | 默认 Token | 说明 |
|---|---|---|---|
| 正常 | `:unchecked`（无悬浮） | `ColorIcon` | 未选中且无交互时的图标色 |
| 悬浮 | `:pointerover` | `ColorIconHover` | 鼠标悬浮时的图标色 |
| 按下 | `:pressed` | `ColorText` | 鼠标按下时的图标色 |
| 禁用 | `:disabled` | `ColorTextDisabled` | 禁用态灰色 |

可通过 `NormalIconBrush`、`ActiveIconBrush`、`SelectedIconBrush`、`DisabledIconBrush` 属性覆盖默认颜色。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|ToggleIconButton` 语法引用 `atom` XML 命名空间下的 `ToggleIconButton` 类型，其中 `|` 是命名空间分隔符。

### 基础选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ToggleIconButton` | 匹配所有 ToggleIconButton 实例，用于设置全局通用样式（如统一图标尺寸） |

### 按状态伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|ToggleIconButton:checked` | 匹配选中状态（`IsChecked == true`），可定制选中态的图标颜色 |
| `atom\|ToggleIconButton:unchecked` | 匹配未选中状态（`IsChecked == false`），可定制未选中态的图标颜色 |
| `atom\|ToggleIconButton:indeterminate` | 匹配不确定状态（`IsChecked == null`，三态模式下） |
| `atom\|ToggleIconButton:pointerover` | 匹配鼠标悬浮状态，用于定制 hover 时的图标颜色或背景 |
| `atom\|ToggleIconButton:pressed` | 匹配按下状态，用于定制 active 时的视觉反馈 |
| `atom\|ToggleIconButton:disabled` | 匹配禁用状态（`IsEnabled == false`），使用灰色调 |
| `atom\|ToggleIconButton:focus-visible` | 匹配通过键盘（Tab 键）获得焦点的状态 |
| `atom\|ToggleIconButton:focus` | 匹配获得焦点的状态（包含鼠标点击和键盘） |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|ToggleIconButton:checked:not(:disabled)` | 非禁用状态的选中按钮 |
| `atom\|ToggleIconButton:checked:pointerover` | 选中状态 + 鼠标悬浮 |
| `atom\|ToggleIconButton:unchecked:pointerover` | 未选中状态 + 鼠标悬浮 |
| `atom\|ToggleIconButton /template/ atom\|IconPresenter#PART_CheckedIconPresenter` | 访问模板内的选中态图标展示器部件 |
| `atom\|ToggleIconButton /template/ atom\|IconPresenter#PART_UnCheckedIconPresenter` | 访问模板内的未选中态图标展示器部件 |
| `atom\|ToggleIconButton /template/ Border#Frame` | 访问模板内名为 `Frame` 的 Border 部件 |
