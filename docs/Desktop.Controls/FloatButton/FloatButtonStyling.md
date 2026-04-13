# FloatButton 自定义样式指南

本文档介绍如何通过属性、Style 选择器和 ControlTheme 自定义 AtomUI FloatButton 的外观。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/General/FloatButtonShowCase.axaml`

---

## 1. 通过属性直接控制

### 按钮类型

```xml
<atom:FloatButtonHost ButtonType="Default" />
<atom:FloatButtonHost ButtonType="Primary" />
```

### 按钮形状

```xml
<atom:FloatButtonHost Shape="Circle" />
<atom:FloatButtonHost Shape="Square" />
```

### 自定义图标

```xml
<atom:FloatButtonHost Icon="{antdicons:AntDesignIconProvider Kind=CustomerServiceOutlined}" />
```

### 自定义位置和偏移

```xml
<atom:FloatButtonHost Placement="BottomRight" FloatOffsetX="40" FloatOffsetY="40" />
```

### 带描述文本（Square 形状）

```xml
<atom:FloatButtonHost Shape="Square"
                       Icon="{antdicons:AntDesignIconProvider Kind=FileTextOutlined}"
                       Description="HELP INFO" />
```

### 带 Tooltip

```xml
<atom:FloatButtonHost Tooltip="Documents" />
<atom:FloatButtonHost Tooltip="Since 5.25.0+" TooltipColor="blue" />
```

### 徽标

```xml
<!-- 圆点徽标 -->
<atom:FloatButtonHost IsBadgeEnabled="True" IsDotBadge="True" />

<!-- 数字徽标 -->
<atom:FloatButtonHost IsBadgeEnabled="True" IsDotBadge="False"
                       BadgeCount="5" BadgeColor="blue" />
```

---

## 2. 通过 Style 覆盖

### 统一按钮样式

```xml
<Window.Styles>
    <Style Selector="atom|FloatButtonHost">
        <Setter Property="Shape" Value="Square" />
        <Setter Property="ButtonType" Value="Primary" />
    </Style>
</Window.Styles>
```

### 统一按钮组样式

```xml
<Window.Styles>
    <Style Selector="atom|FloatButtonGroupHost">
        <Setter Property="Shape" Value="Circle" />
        <Setter Property="ButtonType" Value="Primary" />
    </Style>
</Window.Styles>
```

---

## 3. 分组触发模式

### 始终展开模式

```xml
<atom:FloatButtonGroupHost Shape="Circle">
    <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=QuestionCircleOutlined}" />
    <atom:FloatButton />
    <atom:BackTopFloatButton />
</atom:FloatButtonGroupHost>
```

### 点击触发展开

```xml
<atom:FloatButtonGroupHost Trigger="Click"
                            Icon="{antdicons:AntDesignIconProvider Kind=CustomerServiceOutlined}"
                            ButtonType="Primary">
    <atom:FloatButton />
    <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=CommentOutlined}" />
</atom:FloatButtonGroupHost>
```

### 悬浮触发展开

```xml
<atom:FloatButtonGroupHost Trigger="Hover"
                            Icon="{antdicons:AntDesignIconProvider Kind=CustomerServiceOutlined}"
                            ButtonType="Primary">
    <atom:FloatButton />
    <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=CommentOutlined}" />
</atom:FloatButtonGroupHost>
```

### 自定义展开方向

```xml
<atom:FloatButtonGroupHost Trigger="Click" Placement="Center"
                            MenuPlacement="Top"
                            Icon="{antdicons:AntDesignIconProvider Kind=UpOutlined}">
    <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=WechatOutlined}" />
    <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=QqOutlined}" />
</atom:FloatButtonGroupHost>
```

---

## 4. 返回顶部

```xml
<atom:ScrollViewer Height="300">
    <Panel Height="1000">
        <atom:BackTopFloatButtonHost ButtonType="Default" />
    </Panel>
</atom:ScrollViewer>
```

---

## 样式选择器速查

| 选择器 | 说明 |
|---|---|
| `atom\|FloatButton` | 匹配单个 FloatButton |
| `atom\|FloatButtonHost` | 匹配 FloatButtonHost |
| `atom\|FloatButtonGroupHost` | 匹配按钮分组宿主 |
| `atom\|BackTopFloatButtonHost` | 匹配返回顶部宿主 |
| `atom\|FloatButton:pointerover` | 鼠标悬浮 |
| `atom\|FloatButton:pressed` | 按下 |
| `atom\|FloatButton:icononly` | 仅图标模式 |
| `atom\|FloatButton:disabled` | 禁用 |
