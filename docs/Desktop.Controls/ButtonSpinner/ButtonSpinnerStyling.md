# ButtonSpinner 自定义样式指南

ButtonSpinner 的视觉表现通过 `ControlTheme` + Design Token 系统控制。由于 ButtonSpinner 是一个复合控件（由 `ButtonSpinnerDecoratedBox`、`ButtonSpinnerHandle`、`IconButton` 等内部组件组成），样式定制涉及多个层级。

---

## 1. 通过属性直接控制

最简单的方式是通过 ButtonSpinner 的公共属性来控制外观。

### 尺寸

```xml
<atom:ButtonSpinner SizeType="Large">  <!-- 40px 高度 -->
<atom:ButtonSpinner SizeType="Middle"> <!-- 32px 高度（默认） -->
<atom:ButtonSpinner SizeType="Small">  <!-- 24px 高度 -->
```

### 样式变体

```xml
<atom:ButtonSpinner StyleVariant="Outline">    <!-- 线框（默认） -->
<atom:ButtonSpinner StyleVariant="Filled">     <!-- 填充背景 -->
<atom:ButtonSpinner StyleVariant="Borderless"> <!-- 无边框 -->
```

### 验证状态

```xml
<atom:ButtonSpinner Status="Error">   <!-- 红色边框/背景 -->
<atom:ButtonSpinner Status="Warning"> <!-- 黄色边框/背景 -->
```

### 操作按钮位置

```xml
<atom:ButtonSpinner ButtonSpinnerLocation="Right"> <!-- 右侧（默认） -->
<atom:ButtonSpinner ButtonSpinnerLocation="Left">  <!-- 左侧 -->
```

### 操作按钮控制

```xml
<atom:ButtonSpinner ShowButtonSpinner="False">          <!-- 隐藏操作按钮 -->
<atom:ButtonSpinner IsButtonSpinnerFloatable="True">    <!-- 浮动操作按钮 -->
<atom:ButtonSpinner AllowSpin="False">                  <!-- 禁止 Spin 操作 -->
```

### 附加组件与前后缀

```xml
<atom:ButtonSpinner LeftAddOn="http://" RightAddOn=".com">
<atom:ButtonSpinner InnerLeftContent="￥" InnerRightContent="RMB">
<atom:ButtonSpinner
    InnerLeftContent="{antdicons:AntDesignIconProvider Kind=UserOutlined, FillBrush=#D7D7D7}"
    InnerRightContent="{antdicons:AntDesignIconProvider Kind=InfoCircleOutlined, FillBrush=#8C8C8C}">
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/ButtonSpinnerShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

### 全局样式

```xml
<Window.Styles>
    <!-- 所有 ButtonSpinner 的统一宽度 -->
    <Style Selector="atom|ButtonSpinner">
        <Setter Property="Width" Value="300" />
        <Setter Property="HorizontalAlignment" Value="Left" />
    </Style>

    <!-- 大号 ButtonSpinner 的特殊样式 -->
    <Style Selector="atom|ButtonSpinner[SizeType=Large]">
        <Setter Property="Margin" Value="0,0,0,10" />
    </Style>
</Window.Styles>
```

### 按样式变体定制

```xml
<Window.Styles>
    <!-- Filled 变体的自定义背景 -->
    <Style Selector="atom|ButtonSpinner[StyleVariant=Filled]">
        <Setter Property="Margin" Value="0,5" />
    </Style>

    <!-- Borderless 变体的底部间距 -->
    <Style Selector="atom|ButtonSpinner[StyleVariant=Borderless]">
        <Setter Property="Margin" Value="0,5" />
    </Style>
</Window.Styles>
```

### 按状态定制

```xml
<Window.Styles>
    <!-- Error 状态的自定义间距 -->
    <Style Selector="atom|ButtonSpinner[Status=Error]">
        <Setter Property="Margin" Value="0,5" />
    </Style>

    <!-- Warning 状态的自定义间距 -->
    <Style Selector="atom|ButtonSpinner[Status=Warning]">
        <Setter Property="Margin" Value="0,5" />
    </Style>
</Window.Styles>
```

---

## 3. 伪类驱动的样式

### 按钮位置伪类

```xml
<Window.Styles>
    <!-- 左侧按钮模式 -->
    <Style Selector="atom|ButtonSpinner:left">
        <Setter Property="Margin" Value="0,5" />
    </Style>

    <!-- 右侧按钮模式（默认） -->
    <Style Selector="atom|ButtonSpinner:right">
        <Setter Property="Margin" Value="0,5" />
    </Style>
</Window.Styles>
```

### 标准伪类

```xml
<Window.Styles>
    <Style Selector="atom|ButtonSpinner:disabled">
        <!-- 禁用态样式 -->
    </Style>
    <Style Selector="atom|ButtonSpinner:pointerover">
        <!-- 悬浮态样式 -->
    </Style>
    <Style Selector="atom|ButtonSpinner:focus-within">
        <!-- 子控件获得焦点态 -->
    </Style>
</Window.Styles>
```

---

## 4. 控制动画行为

```xml
<!-- 禁用过渡动画 -->
<atom:ButtonSpinner IsMotionEnabled="False">
    <atom:TextBlock Text="无动画" />
</atom:ButtonSpinner>
```

ButtonSpinner 的以下动画效果受 `IsMotionEnabled` 控制：
- 操作按钮区域的悬浮/焦点边框颜色过渡
- 浮动模式下操作按钮的滑入/滑出动画
- 浮动模式下内容区域的位移动画

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|ButtonSpinner` 语法引用 `atom` XML 命名空间下的控件类型。

### 按控件类型

| 选择器 | 说明 |
|---|---|
| `atom\|ButtonSpinner` | 匹配所有 ButtonSpinner 实例 |

### 按属性

| 选择器 | 说明 |
|---|---|
| `atom\|ButtonSpinner[SizeType=Large]` | 大号尺寸 |
| `atom\|ButtonSpinner[SizeType=Middle]` | 中号尺寸（默认） |
| `atom\|ButtonSpinner[SizeType=Small]` | 小号尺寸 |
| `atom\|ButtonSpinner[StyleVariant=Outline]` | 线框样式变体 |
| `atom\|ButtonSpinner[StyleVariant=Filled]` | 填充样式变体 |
| `atom\|ButtonSpinner[StyleVariant=Borderless]` | 无边框样式变体 |
| `atom\|ButtonSpinner[Status=Error]` | 错误验证状态 |
| `atom\|ButtonSpinner[Status=Warning]` | 警告验证状态 |
| `atom\|ButtonSpinner[ButtonSpinnerLocation=Left]` | 操作按钮在左侧 |
| `atom\|ButtonSpinner[ButtonSpinnerLocation=Right]` | 操作按钮在右侧 |

### 按伪类

| 选择器 | 说明 |
|---|---|
| `atom\|ButtonSpinner:left` | 操作按钮在左侧时 |
| `atom\|ButtonSpinner:right` | 操作按钮在右侧时 |
| `atom\|ButtonSpinner:disabled` | 禁用状态 |
| `atom\|ButtonSpinner:pointerover` | 鼠标悬浮 |
| `atom\|ButtonSpinner:focus` | 获得焦点 |
| `atom\|ButtonSpinner:focus-within` | 子控件获得焦点 |

### 内部组件选择器

ButtonSpinner 的视觉呈现由多个内部组件协作实现。在高级定制场景下，可以通过模板部件选择器访问内部组件：

| 选择器 | 说明 |
|---|---|
| `atom\|ButtonSpinner /template/ atom\|ButtonSpinnerDecoratedBox#PART_DecoratedBox` | 装饰容器 |
| `atom\|ButtonSpinnerHandle` | 操作按钮容器（内部控件） |
| `atom\|ButtonSpinnerHandle /template/ atom\|IconButton#PART_IncreaseButton` | 增加按钮 |
| `atom\|ButtonSpinnerHandle /template/ atom\|IconButton#PART_DecreaseButton` | 减少按钮 |
| `atom\|ButtonSpinnerHandle /template/ atom\|IconButton#PART_IncreaseButton:pointerover` | 增加按钮悬浮态 |
| `atom\|ButtonSpinnerHandle /template/ atom\|IconButton#PART_DecreaseButton:pointerover` | 减少按钮悬浮态 |
| `atom\|ButtonSpinnerHandle /template/ atom\|IconButton#PART_IncreaseButton:pressed` | 增加按钮按下态 |
| `atom\|ButtonSpinnerHandle /template/ atom\|IconButton#PART_DecreaseButton:pressed` | 减少按钮按下态 |
