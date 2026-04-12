# Badge 自定义样式指南

Badge 系列控件的视觉表现通过 `ControlTheme` + Design Token 系统控制。由于 Badge 采用 Adorner 模式实现，其样式定制方式与普通 TemplatedControl 略有不同。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Badge 的公共属性来控制外观。

### CountBadge（数字徽标）

```xml
<!-- 基本用法 — 装饰目标控件 -->
<atom:CountBadge Count="5">
    <Border Width="40" Height="40" Background="rgb(191,191,191)" CornerRadius="8" />
</atom:CountBadge>

<!-- 封顶数字 -->
<atom:CountBadge Count="100" OverflowCount="99">
    <Border Width="40" Height="40" Background="rgb(191,191,191)" CornerRadius="8" />
</atom:CountBadge>

<!-- 显示零值 -->
<atom:CountBadge Count="0" ShowZero="True">
    <Border Width="40" Height="40" Background="rgb(191,191,191)" CornerRadius="8" />
</atom:CountBadge>

<!-- 小号尺寸 -->
<atom:CountBadge Count="5" Size="Small">
    <Border Width="40" Height="40" Background="rgb(191,191,191)" CornerRadius="8" />
</atom:CountBadge>

<!-- 自定义颜色 -->
<atom:CountBadge BadgeColor="#faad14" Count="11" ShowZero="True" />
<atom:CountBadge BadgeColor="#52c41a" Count="109" />

<!-- 偏移量 -->
<atom:CountBadge Count="5" Offset="10, 10">
    <Border Width="40" Height="40" Background="rgb(191,191,191)" CornerRadius="8" />
</atom:CountBadge>

<!-- 独立使用（不设 DecoratedTarget） -->
<atom:CountBadge Count="25" />
```

### DotBadge（圆点徽标）

```xml
<!-- 装饰模式 — 红点标记 -->
<atom:DotBadge Offset="-7,8">
    <atom:Button ButtonType="Link"
                 Icon="{antdicons:AntDesignIconProvider Kind=NotificationOutlined}" />
</atom:DotBadge>

<!-- 独立使用 + 五种语义状态 -->
<atom:DotBadge Status="Success" Text="Success" />
<atom:DotBadge Status="Error" Text="Error" />
<atom:DotBadge Status="Default" Text="Default" />
<atom:DotBadge Status="Processing" Text="Processing" />
<atom:DotBadge Status="Warning" Text="Warning" />

<!-- 仅圆点（不带文字） -->
<atom:DotBadge Status="Success" />
<atom:DotBadge Status="Error" />

<!-- 预设颜色 -->
<atom:DotBadge DotColor="Pink" Text="Pink" />
<atom:DotBadge DotColor="Blue" Text="Blue" />
<atom:DotBadge DotColor="GeekBlue" Text="GeekBlue" />
<atom:DotBadge DotColor="Gold" Text="Gold" />

<!-- CSS 颜色值 -->
<atom:DotBadge DotColor="#f50" Text="#f50" />
<atom:DotBadge DotColor="rgb(45, 183, 245)" Text="rgb(45, 183, 245)" />
<atom:DotBadge DotColor="hsl(102, 53%, 61%)" Text="hsl(102, 53%, 61%)" />
```

### RibbonBadge（缎带徽标）

```xml
<!-- 默认主色缎带（右上角） -->
<atom:RibbonBadge Text="精益求精，打造体验优秀的 UISDK">
    <Border Padding="10,0,10,0" BorderBrush="#d9d9d9" BorderThickness="1" CornerRadius="6">
        <StackPanel Orientation="Vertical">
            <TextBlock Height="38" FontWeight="Bold" LineHeight="38">
                Pushes open the window
            </TextBlock>
        </StackPanel>
    </Border>
</atom:RibbonBadge>

<!-- 预设颜色 -->
<atom:RibbonBadge RibbonColor="Pink" Text="甲辰计划雄起">
    <!-- ... -->
</atom:RibbonBadge>

<!-- 左上角放置 -->
<atom:RibbonBadge Placement="Start" RibbonColor="purple" Text="Hippies">
    <!-- ... -->
</atom:RibbonBadge>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/BadgeShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

Badge 系列控件的样式可通过 AXAML `Style` 进行全局或局部定制。但需注意：Badge 的视觉呈现主要由内部 Adorner 控件驱动，外层 Badge 控件本身的样式选择器作用有限。

### CountBadge 样式

```xml
<Window.Styles>
    <!-- 所有 CountBadge 的统一间距 -->
    <Style Selector="atom|CountBadge">
        <Setter Property="Margin" Value="10" />
    </Style>
</Window.Styles>
```

### DotBadge 样式

```xml
<Window.Styles>
    <!-- 所有 DotBadge 的统一间距 -->
    <Style Selector="atom|DotBadge">
        <Setter Property="Margin" Value="5" />
    </Style>
</Window.Styles>
```

### RibbonBadge 样式

```xml
<Window.Styles>
    <!-- 所有 RibbonBadge 的统一外边距 -->
    <Style Selector="atom|RibbonBadge">
        <Setter Property="Margin" Value="20,0,20,0" />
    </Style>
</Window.Styles>
```

---

## 3. 动态控制显隐

通过数据绑定 `BadgeIsVisible` 属性，可以实现 Badge 的动态显隐（配合缩放动画）：

```xml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge BadgeIsVisible="{Binding DynamicDotBadgeVisible}" Count="9">
        <Border Width="40" Height="40" Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:CountBadge>
    <atom:DotBadge BadgeIsVisible="{Binding DynamicDotBadgeVisible}">
        <Border Width="40" Height="40" Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:DotBadge>
    <atom:ToggleSwitch VerticalAlignment="Center"
                       IsChecked="{Binding DynamicDotBadgeVisible, Mode=TwoWay}" />
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/BadgeShowCase.axaml` 中 "Dynamic" 示例。

---

## 4. 控制动画行为

```xml
<!-- 禁用 CountBadge 的显隐动画 -->
<atom:CountBadge Count="5" IsMotionEnabled="False">
    <Border Width="40" Height="40" Background="rgb(191,191,191)" CornerRadius="8" />
</atom:CountBadge>

<!-- 禁用 DotBadge 的显隐动画 -->
<atom:DotBadge IsMotionEnabled="False">
    <Border Width="40" Height="40" Background="rgb(191,191,191)" CornerRadius="8" />
</atom:DotBadge>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|CountBadge` 语法引用 `atom` XML 命名空间下的控件类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|CountBadge` | 匹配所有数字徽标实例 |
| `atom\|DotBadge` | 匹配所有圆点徽标实例 |
| `atom\|RibbonBadge` | 匹配所有缎带徽标实例 |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|CountBadge[Size=Small]` | 匹配小号数字徽标 |
| `atom\|DotBadge[Status=Error]` | 匹配错误状态的圆点徽标 |
| `atom\|DotBadge[Status=Success]` | 匹配成功状态的圆点徽标 |
| `atom\|DotBadge[Status=Processing]` | 匹配处理中状态的圆点徽标 |
| `atom\|DotBadge[Status=Warning]` | 匹配警告状态的圆点徽标 |
| `atom\|DotBadge[Status=Default]` | 匹配默认状态的圆点徽标 |
| `atom\|RibbonBadge[Placement=Start]` | 匹配左上角放置的缎带徽标 |
| `atom\|RibbonBadge[Placement=End]` | 匹配右上角放置的缎带徽标 |

### 内部 Adorner 选择器

Badge 的视觉呈现由内部 Adorner 控件实现。在高级定制场景下，可以通过 Adorner 的主题来调整样式。以下是 Adorner 控件支持的选择器：

| 选择器 | 说明 |
|---|---|
| `atom\|CountBadgeAdorner` | 数字徽标装饰器（内部控件） |
| `atom\|CountBadgeAdorner[IsAdornerMode=True]` | 装饰模式下的数字徽标装饰器 |
| `atom\|CountBadgeAdorner[Size=Small]` | 小号数字徽标装饰器 |
| `atom\|CountBadgeAdorner /template/ Border#BadgeIndicator` | 数字徽标的红色背景圆角框 |
| `atom\|CountBadgeAdorner /template/ atom\|TextBlock#BadgeText` | 数字徽标的数字文本 |
| `atom\|DotBadgeAdorner` | 圆点徽标装饰器（内部控件） |
| `atom\|DotBadgeAdorner[IsAdornerMode=True]` | 装饰模式下的圆点装饰器 |
| `atom\|DotBadgeAdorner[Status=Error]` | 错误状态圆点装饰器 |
| `atom\|DotBadgeAdorner /template/ Label#Label` | 圆点旁的文字标签 |
| `atom\|RibbonBadgeAdorner` | 缎带徽标装饰器（内部控件） |
| `atom\|RibbonBadgeAdorner /template/ atom\|TextBlock#PART_LabelPart` | 缎带上的文字 |
