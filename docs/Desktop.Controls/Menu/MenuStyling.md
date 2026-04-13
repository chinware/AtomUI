# Menu 自定义样式指南

本文档介绍如何通过 `Style` 选择器和 `ControlTheme` 覆盖 Menu 控件家族的视觉表现。

---

## 1. 通过属性直接控制

Menu 控件的大部分视觉属性通过 Design Token 系统控制。以下属性可直接在 AXAML 中设置：

### Menu

```xml
<!-- 尺寸 -->
<atom:Menu SizeType="Large" />
<atom:Menu SizeType="Small" />

<!-- 关闭动画 -->
<atom:Menu IsMotionEnabled="False" />

<!-- 子菜单可见项数 -->
<atom:Menu DisplayPageSize="15" />
```

### ContextMenu

```xml
<atom:ContextMenu SizeType="Small"
                  DisplayPageSize="8"
                  ShouldUseOverlayLayer="True" />
```

### MenuItem

```xml
<!-- 图标 -->
<atom:MenuItem Header="Cut"
               Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />

<!-- CheckBox/Radio 切换 -->
<atom:MenuItem Header="Option A" ToggleType="CheckBox" />
<atom:MenuItem Header="Choice 1" ToggleType="Radio" GroupName="MyGroup" />

<!-- 快捷键提示 -->
<atom:MenuItem Header="Save" InputGesture="Ctrl+S" />
```

---

## 2. 通过 Style 选择器覆盖

### 2.1 Menu 样式

```xml
<Window.Styles>
    <!-- 所有 Menu 的背景色 -->
    <Style Selector="atom|Menu">
        <Setter Property="Background" Value="#F5F5F5" />
    </Style>

    <!-- 大号 Menu 特殊边框 -->
    <Style Selector="atom|Menu[SizeType=Large]">
        <Setter Property="BorderThickness" Value="0,0,0,1" />
        <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorderSecondary}" />
    </Style>
</Window.Styles>
```

### 2.2 MenuItem 样式（子菜单项）

```xml
<Window.Styles>
    <!-- 所有子菜单项的文字颜色 -->
    <Style Selector="atom|MenuItem">
        <Setter Property="Foreground" Value="{atom:MenuTokenResource ItemColor}" />
    </Style>

    <!-- 悬浮时的背景色和文字色 -->
    <Style Selector="atom|MenuItem:pointerover">
        <Setter Property="Background" Value="{atom:MenuTokenResource ItemHoverBg}" />
        <Setter Property="Foreground" Value="{atom:MenuTokenResource ItemHoverColor}" />
    </Style>

    <!-- 禁用状态 -->
    <Style Selector="atom|MenuItem:disabled">
        <Setter Property="Foreground" Value="{atom:MenuTokenResource ItemDisabledColor}" />
    </Style>
</Window.Styles>
```

### 2.3 顶层菜单项样式

顶层菜单项（直接嵌套在 `Menu` 中的 `MenuItem`）使用 `TopLevelMenuItemTheme` 主题，通过 `:toplevel` 伪类匹配：

```xml
<Window.Styles>
    <!-- 顶层菜单项悬浮背景 -->
    <Style Selector="atom|MenuItem:toplevel:pointerover">
        <Setter Property="Background"
                Value="{atom:MenuTokenResource TopLevelItemHoverBg}" />
    </Style>

    <!-- 顶层菜单项打开子菜单时 -->
    <Style Selector="atom|MenuItem:toplevel[IsSubMenuOpen=True]">
        <Setter Property="Background"
                Value="{atom:MenuTokenResource TopLevelItemSelectedBg}" />
    </Style>
</Window.Styles>
```

### 2.4 深入模板内部

可以通过 `/template/` 选择器访问模板内部元素：

```xml
<Window.Styles>
    <!-- 子菜单弹窗背景 -->
    <Style Selector="atom|MenuItem /template/ Border#PopupFrame">
        <Setter Property="Background"
                Value="{atom:MenuTokenResource MenuPopupBgColor}" />
        <Setter Property="CornerRadius"
                Value="{atom:PopupHostTokenResource BorderRadius}" />
    </Style>

    <!-- 快捷键提示文字颜色 -->
    <Style Selector="atom|MenuItem /template/ atom|TextBlock#InputGestureText">
        <Setter Property="Foreground"
                Value="{atom:MenuTokenResource KeyGestureColor}" />
    </Style>

    <!-- 图标区域样式 -->
    <Style Selector="atom|MenuItem /template/ atom|IconPresenter#ItemIconPresenter">
        <Setter Property="Width" Value="{atom:MenuTokenResource ItemIconSize}" />
        <Setter Property="Height" Value="{atom:MenuTokenResource ItemIconSize}" />
        <Setter Property="IconBrush" Value="{atom:MenuTokenResource ItemColor}" />
    </Style>

    <!-- 子菜单指示箭头隐藏（无子项时） -->
    <Style Selector="atom|MenuItem:empty /template/ atom|Icon#MenuIndicatorIcon">
        <Setter Property="IsVisible" Value="False" />
    </Style>

    <!-- 鼠标指针为手型 -->
    <Style Selector="atom|MenuItem /template/ Border#Frame">
        <Setter Property="Cursor" Value="Hand" />
    </Style>
</Window.Styles>
```

---

## 3. 通过 ControlTheme 完全重写

如果需要完全自定义菜单的外观，可以创建新的 `ControlTheme`：

```xml
<ControlTheme x:Key="CustomMenuItemTheme" TargetType="atom:MenuItem">
    <Setter Property="Template">
        <ControlTemplate TargetType="atom:MenuItem">
            <!-- 自定义模板 -->
            <Border Background="{TemplateBinding Background}"
                    Padding="{TemplateBinding Padding}"
                    CornerRadius="{TemplateBinding CornerRadius}">
                <ContentPresenter Content="{TemplateBinding Header}"
                                  ContentTemplate="{TemplateBinding HeaderTemplate}" />
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

> ⚠️ 完全重写 ControlTheme 需要确保模板中包含必要的 Template Part（如 `PART_Popup`、`PART_ToggleCheckbox` 等），否则会丢失子菜单弹出和 Toggle 功能。

---

## 样式选择器速查

### Menu

| 选择器 | 说明 |
|---|---|
| `atom\|Menu` | 匹配所有 Menu |
| `atom\|Menu[SizeType=Large]` | 大号菜单 |
| `atom\|Menu[SizeType=Middle]` | 中号菜单 |
| `atom\|Menu[SizeType=Small]` | 小号菜单 |

### MenuItem

| 选择器 | 说明 |
|---|---|
| `atom\|MenuItem` | 匹配所有 MenuItem |
| `atom\|MenuItem:toplevel` | 顶层菜单项（直接在 Menu 中） |
| `atom\|MenuItem:pointerover` | 鼠标悬浮 |
| `atom\|MenuItem:disabled` | 禁用状态 |
| `atom\|MenuItem:checked` | 选中状态 |
| `atom\|MenuItem:empty` | 无子菜单项 |
| `atom\|MenuItem:pressed` | 按下状态 |
| `atom\|MenuItem[IsSubMenuOpen=True]` | 子菜单已打开 |
| `atom\|MenuItem[ToggleType=CheckBox]` | CheckBox 类型 |
| `atom\|MenuItem[ToggleType=Radio]` | Radio 类型 |
| `atom\|MenuItem[SizeType=Large]` | 大号 |
| `atom\|MenuItem[SizeType=Small]` | 小号 |

### ContextMenu

| 选择器 | 说明 |
|---|---|
| `atom\|ContextMenu` | 匹配所有 ContextMenu |

### MenuSeparator

| 选择器 | 说明 |
|---|---|
| `atom\|MenuSeparator` | 匹配所有菜单分隔线 |

### 模板内部元素

| 选择器 | 说明 |
|---|---|
| `atom\|MenuItem /template/ Border#Frame` | 菜单项背景框 |
| `atom\|MenuItem /template/ atom\|IconPresenter#ItemIconPresenter` | 图标展示器 |
| `atom\|MenuItem /template/ ContentPresenter#ItemTextPresenter` | 文字展示器 |
| `atom\|MenuItem /template/ atom\|TextBlock#InputGestureText` | 快捷键文字 |
| `atom\|MenuItem /template/ atom\|Icon#MenuIndicatorIcon` | 子菜单箭头 |
| `atom\|MenuItem /template/ Border#PopupFrame` | 子菜单弹窗框 |
| `atom\|MenuItem /template/ atom\|CheckBox#PART_ToggleCheckbox` | CheckBox 控件 |
| `atom\|MenuItem /template/ atom\|RadioButton#PART_ToggleRadio` | RadioButton 控件 |

---

## 4. Token 覆盖（全局主题定制）

通过自定义 `MenuToken` 的值来全局调整所有菜单的视觉表现。详见 [MenuToken.md](MenuToken.md)。

常见的 Token 定制场景：

| 定制目标 | Token |
|---|---|
| 修改悬浮背景色 | `ItemHoverBg` |
| 修改菜单项文字色 | `ItemColor` |
| 修改禁用文字色 | `ItemDisabledColor` |
| 修改弹窗背景色 | `MenuPopupBgColor` |
| 修改弹窗最小宽度 | `MenuPopupMinWidth` |
| 修改菜单项圆角 | `ItemBorderRadius` |
| 修改图标尺寸 | `ItemIconSize` |
| 修改快捷键文字颜色 | `KeyGestureColor` |
