# Segmented 自定义样式指南

Segmented 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Segmented 的公共属性来控制外观：

```xml
<!-- 基本使用 -->
<atom:Segmented>
    <atom:SegmentedItem>Daily</atom:SegmentedItem>
    <atom:SegmentedItem>Weekly</atom:SegmentedItem>
    <atom:SegmentedItem>Monthly</atom:SegmentedItem>
</atom:Segmented>

<!-- Block 模式 — 撑满父容器 -->
<atom:Segmented IsExpanding="True">
    <atom:SegmentedItem>123</atom:SegmentedItem>
    <atom:SegmentedItem>456</atom:SegmentedItem>
    <atom:SegmentedItem>longtext-longtext</atom:SegmentedItem>
</atom:Segmented>

<!-- 不同尺寸 -->
<atom:Segmented SizeType="Large">...</atom:Segmented>
<atom:Segmented SizeType="Small">...</atom:Segmented>

<!-- 带图标 -->
<atom:Segmented>
    <atom:SegmentedItem Icon="{antdicons:AntDesignIconProvider Kind=BarsOutlined}">
        List
    </atom:SegmentedItem>
    <atom:SegmentedItem Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}">
        Kanban
    </atom:SegmentedItem>
</atom:Segmented>

<!-- 仅图标 -->
<atom:Segmented>
    <atom:SegmentedItem Icon="{antdicons:AntDesignIconProvider Kind=BarsOutlined}" />
    <atom:SegmentedItem Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}" />
</atom:Segmented>

<!-- 禁用某些选项 -->
<atom:Segmented>
    <atom:SegmentedItem>Daily</atom:SegmentedItem>
    <atom:SegmentedItem IsEnabled="False">Weekly</atom:SegmentedItem>
    <atom:SegmentedItem>Monthly</atom:SegmentedItem>
</atom:Segmented>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/SegmentedShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Segmented 进行全局或局部样式覆盖：

### 全局统一外边距

```xml
<Window.Styles>
    <Style Selector="atom|Segmented">
        <Setter Property="Margin" Value="10" />
    </Style>
</Window.Styles>
```

### 自定义选项悬浮态颜色

```xml
<Window.Styles>
    <!-- 非选中选项悬浮时的背景色 -->
    <Style Selector="atom|SegmentedItem[IsEnabled=True]:not(:selected):pointerover">
        <Setter Property="Background" Value="#E6F7FF" />
    </Style>
    
    <!-- 非选中选项悬浮时的文字色 -->
    <Style Selector="atom|SegmentedItem[IsEnabled=True]:not(:selected):pointerover">
        <Setter Property="Foreground" Value="#1677FF" />
    </Style>
</Window.Styles>
```

### 使用伪类选择器

```xml
<!-- 选中选项的自定义样式 -->
<Style Selector="atom|SegmentedItem:selected">
    <Setter Property="FontWeight" Value="Bold" />
</Style>

<!-- 带图标的选项添加额外样式 -->
<Style Selector="atom|SegmentedItem:has-icon">
    <Setter Property="Padding" Value="8,0" />
</Style>

<!-- 禁用选项的样式 -->
<Style Selector="atom|SegmentedItem:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

### 按尺寸定制

```xml
<!-- 大号 Segmented 使用半粗体 -->
<Style Selector="atom|Segmented[SizeType=Large]">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Segmented 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomSegmented" TargetType="atom:Segmented">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    Padding="{TemplateBinding Padding}">
                <ItemsPresenter Name="PART_ItemsPresenter">
                    <ItemsPresenter.ItemsPanel>
                        <ItemsPanelTemplate>
                            <StackPanel Orientation="Horizontal" />
                        </ItemsPanelTemplate>
                    </ItemsPresenter.ItemsPanel>
                </ItemsPresenter>
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:Segmented Theme="{StaticResource MyCustomSegmented}">
    <atom:SegmentedItem>A</atom:SegmentedItem>
    <atom:SegmentedItem>B</atom:SegmentedItem>
</atom:Segmented>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的滑动色块渲染和动画效果。Segmented 的色块是在 `Render` 方法中绘制的，替换模板不会影响色块，但更换 ItemsPanel 可能影响布局和动画定位。建议优先使用 Style 覆盖。

---

## 4. Block 模式（撑满父容器）

Ant Design 的 `block` 属性在 AtomUI 中通过 `IsExpanding` 属性实现：

```xml
<StackPanel Orientation="Vertical" Width="400">
    <atom:Segmented IsExpanding="True">
        <atom:SegmentedItem>123</atom:SegmentedItem>
        <atom:SegmentedItem>456</atom:SegmentedItem>
        <atom:SegmentedItem>longtext-longtext-longtext-longtext</atom:SegmentedItem>
    </atom:Segmented>
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/SegmentedShowCase.axaml` 中 "Block Segmented" 示例。

---

## 5. 控制动画行为

```xml
<!-- 禁用色块滑动动画和选项背景色过渡 -->
<atom:Segmented IsMotionEnabled="False">
    <atom:SegmentedItem>A</atom:SegmentedItem>
    <atom:SegmentedItem>B</atom:SegmentedItem>
    <atom:SegmentedItem>C</atom:SegmentedItem>
</atom:Segmented>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Segmented` 语法引用 `atom` XML 命名空间下的 `Segmented` 类型，其中 `|` 是命名空间分隔符。

### Segmented 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Segmented` | 匹配所有 AtomUI Segmented 实例 |
| `atom\|Segmented[SizeType=Large]` | 匹配大号 Segmented |
| `atom\|Segmented[SizeType=Middle]` | 匹配中号 Segmented（默认尺寸） |
| `atom\|Segmented[SizeType=Small]` | 匹配小号 Segmented |
| `atom\|Segmented[IsExpanding=True]` | 匹配 Block 模式的 Segmented |

### SegmentedItem 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|SegmentedItem` | 匹配所有 SegmentedItem 实例 |
| `atom\|SegmentedItem:selected` | 匹配当前选中的选项 |
| `atom\|SegmentedItem:not(:selected)` | 匹配未选中的选项 |
| `atom\|SegmentedItem:pointerover` | 匹配鼠标悬浮状态的选项 |
| `atom\|SegmentedItem:pressed` | 匹配鼠标按下状态的选项 |
| `atom\|SegmentedItem:disabled` | 匹配禁用的选项 |
| `atom\|SegmentedItem:has-icon` | 匹配带图标的选项 |
| `atom\|SegmentedItem[SizeType=Large]` | 匹配大号选项 |
| `atom\|SegmentedItem[SizeType=Small]` | 匹配小号选项 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|SegmentedItem[IsEnabled=True]:not(:selected):pointerover` | 未选中且可交互的悬浮态选项 |
| `atom\|SegmentedItem[IsEnabled=True]:not(:selected):pressed` | 未选中且可交互的按下态选项 |
| `atom\|SegmentedItem:selected /template/ atom\|IconPresenter#IconPresenter` | 选中选项的图标展示器 |
| `atom\|SegmentedItem:has-icon /template/ ContentPresenter#Content` | 带图标选项的内容展示器 |
| `atom\|Segmented /template/ Border#Frame` | Segmented 模板内的 Frame Border |
