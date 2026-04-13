# Splitter 自定义样式指南

Splitter 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 通过属性直接控制

最简单的方式是通过 Splitter 的公共属性来控制外观：

```xml
<!-- 垂直分割（左右排列） -->
<atom:Splitter Orientation="Vertical" Height="220">
    <Border atom:Splitter.Size="30%"><TextBlock>Left</TextBlock></Border>
    <Border><TextBlock>Right</TextBlock></Border>
</atom:Splitter>

<!-- 水平分割（上下排列） -->
<atom:Splitter Orientation="Horizontal" Height="220">
    <Border atom:Splitter.Size="40%"><TextBlock>Top</TextBlock></Border>
    <Border><TextBlock>Bottom</TextBlock></Border>
</atom:Splitter>

<!-- 延迟渲染模式 -->
<atom:Splitter Orientation="Vertical" IsLazy="True">
    <Border atom:Splitter.Size="50%"><TextBlock>First</TextBlock></Border>
    <Border><TextBlock>Second</TextBlock></Border>
</atom:Splitter>

<!-- 自定义折叠图标 -->
<atom:Splitter CollapsePreviousIcon="{antdicons:AntDesignIconProvider Kind=LeftOutlined}"
               CollapseNextIcon="{antdicons:AntDesignIconProvider Kind=RightOutlined}">
    <Border atom:Splitter.Collapsible="Always"><TextBlock>Left</TextBlock></Border>
    <Border><TextBlock>Right</TextBlock></Border>
</atom:Splitter>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Layout/SplitterShowCase.axaml`

---

## 2. 通过子面板附加属性控制布局

Splitter 的面板布局行为完全通过附加属性在子控件上配置：

```xml
<atom:Splitter Orientation="Vertical" Height="220">
    <!-- 百分比尺寸 + 折叠 -->
    <Border atom:Splitter.Size="30%"
            atom:Splitter.MinSize="100"
            atom:Splitter.Collapsible="Always">
        <TextBlock>Collapsible Panel</TextBlock>
    </Border>
    
    <!-- 固定像素尺寸 + 禁止拖拽 -->
    <Border atom:Splitter.DefaultSize="200"
            atom:Splitter.IsResizable="False">
        <TextBlock>Fixed Panel</TextBlock>
    </Border>
    
    <!-- 弹性面板（自动分配剩余空间） -->
    <Border>
        <TextBlock>Flexible Panel</TextBlock>
    </Border>
</atom:Splitter>
```

---

## 3. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Splitter 及其内部组件进行全局或局部样式覆盖：

### 全局 Splitter 样式

```xml
<Window.Styles>
    <!-- 所有 Splitter 使用统一内边距 -->
    <Style Selector="atom|Splitter">
        <Setter Property="Padding" Value="4" />
    </Style>
</Window.Styles>
```

### 自定义手柄分割线样式

```xml
<Window.Styles>
    <!-- 手柄默认分割线颜色 -->
    <Style Selector="atom|SplitterHandle">
        <Setter Property="LineBrush" Value="#E0E0E0" />
    </Style>
    
    <!-- 手柄悬浮态分割线颜色 -->
    <Style Selector="atom|SplitterHandle:pointerover">
        <Setter Property="LineBrush" Value="#1677FF" />
    </Style>
    
    <!-- 拖拽中分割线颜色 -->
    <Style Selector="atom|SplitterHandle:dragging">
        <Setter Property="LineBrush" Value="#0958D9" />
    </Style>
</Window.Styles>
```

### 通过方向选择器定制

```xml
<Window.Styles>
    <!-- 仅对垂直分割的手柄设置宽度 -->
    <Style Selector="atom|SplitterHandle[Orientation=Vertical]">
        <Setter Property="Width" Value="8" />
    </Style>

    <!-- 仅对水平分割的手柄设置高度 -->
    <Style Selector="atom|SplitterHandle[Orientation=Horizontal]">
        <Setter Property="Height" Value="8" />
    </Style>
</Window.Styles>
```

---

## 4. 自定义面板容器样式

通常的做法是在 Splitter 的子面板上使用 CSS 类样式，而非直接修改 Splitter 内部控件。以下是 Gallery 中使用的面板样式：

```xml
<UserControl.Styles>
    <Style Selector="Border.splitter-surface">
        <Setter Property="Background" Value="{atom:SharedTokenResource ColorBgContainer}" />
        <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorderSecondary}" />
        <Setter Property="CornerRadius" Value="6" />
        <Setter Property="BoxShadow" Value="0 0 10 0 #1A000000" />
    </Style>
    <Style Selector="Border.splitter-panel">
        <Setter Property="Background" Value="{atom:SharedTokenResource ColorBgContainer}" />
        <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorder}" />
    </Style>
    <Style Selector="Border.splitter-panel.alt">
        <Setter Property="Background" Value="{atom:SharedTokenResource ColorFillAlter}" />
    </Style>
    <Style Selector="TextBlock.splitter-label">
        <Setter Property="HorizontalAlignment" Value="Center" />
        <Setter Property="VerticalAlignment" Value="Center" />
        <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorTextDescription}" />
        <Setter Property="FontWeight" Value="SemiBold" />
    </Style>
</UserControl.Styles>

<Border Classes="splitter-surface">
    <atom:Splitter Orientation="Vertical" Height="220">
        <Border Classes="splitter-panel" atom:Splitter.Size="30%">
            <TextBlock Classes="splitter-label">First</TextBlock>
        </Border>
        <Border Classes="splitter-panel alt">
            <TextBlock Classes="splitter-label">Second</TextBlock>
        </Border>
    </atom:Splitter>
</Border>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Layout/SplitterShowCase.axaml`

---

## 5. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Splitter 或 SplitterHandle 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomSplitter" TargetType="atom:Splitter">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    Padding="{TemplateBinding Padding}">
                <atom:SplitterPanel Name="PART_SplitterPanel"
                                    HandleSize="{TemplateBinding HandleSize}"
                                    Orientation="{TemplateBinding Orientation}"
                                    IsLazy="{TemplateBinding IsLazy}" />
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:Splitter Theme="{StaticResource MyCustomSplitter}" Height="220">
    <Border><TextBlock>Left</TextBlock></Border>
    <Border><TextBlock>Right</TextBlock></Border>
</atom:Splitter>
```

> ⚠️ 注意：完全替换 ControlTheme 需要确保模板中包含名为 `PART_SplitterPanel` 的 `SplitterPanel`，否则 Splitter 将无法正常工作。

---

## 6. 禁止拖拽时的样式

当手柄两侧任一面板设置了 `IsResizable="False"` 时，手柄自动禁用拖拽。主题会自动应用以下样式变化：

- 分割线颜色恢复为默认的 `HandleLineColor`（不再响应 hover 和 drag 颜色变化）
- 手柄握持区（`PART_HandleGrip`）自动隐藏
- 鼠标光标不会变为调整大小光标

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Splitter` 语法引用 `atom` XML 命名空间下的 `Splitter` 类型，其中 `|` 是命名空间分隔符。

### Splitter 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Splitter` | 匹配所有 Splitter 实例 |
| `atom\|Splitter[Orientation=Vertical]` | 匹配垂直方向分割的 Splitter |
| `atom\|Splitter[Orientation=Horizontal]` | 匹配水平方向分割的 Splitter |
| `atom\|Splitter[IsLazy=True]` | 匹配启用延迟渲染模式的 Splitter |

### SplitterHandle 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|SplitterHandle` | 匹配所有手柄实例 |
| `atom\|SplitterHandle:pointerover` | 鼠标悬浮在手柄上 |
| `atom\|SplitterHandle:dragging` | 手柄正在被拖拽 |
| `atom\|SplitterHandle[Orientation=Vertical]` | 垂直方向手柄 |
| `atom\|SplitterHandle[Orientation=Horizontal]` | 水平方向手柄 |
| `atom\|SplitterHandle[IsDragEnabled=False]` | 禁止拖拽的手柄 |

### 模板内部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|SplitterHandle /template/ Border#PART_HandleLine` | 手柄分割线 |
| `atom\|SplitterHandle /template/ Border#PART_HandleGrip` | 手柄握持区 |
| `atom\|SplitterHandle /template/ atom\|IconButton#PART_CollapsePrevButton` | 向前折叠按钮 |
| `atom\|SplitterHandle /template/ atom\|IconButton#PART_CollapseNextButton` | 向后折叠按钮 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|SplitterHandle[Orientation=Vertical] /template/ Border#PART_HandleLine` | 垂直手柄的分割线 |
| `atom\|SplitterHandle[IsDragEnabled=False]:pointerover` | 禁止拖拽的手柄在悬浮态 |
| `atom\|SplitterHandle /template/ atom\|IconButton:pointerover` | 折叠按钮的悬浮态 |
