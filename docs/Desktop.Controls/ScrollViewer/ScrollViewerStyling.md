# ScrollViewer 自定义样式指南

ScrollViewer 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 ScrollViewer 的公共属性来控制外观和行为：

```xml
<!-- 标准模式（默认），滚动条占据独立空间 -->
<atom:ScrollViewer VerticalScrollBarVisibility="Auto"
                   AllowAutoHide="False">
    <StackPanel>
        <!-- 内容 -->
    </StackPanel>
</atom:ScrollViewer>

<!-- 极简模式，滚动条覆盖在内容之上，鼠标移入时显示 -->
<atom:ScrollViewer IsLiteMode="True"
                   AllowAutoHide="True">
    <StackPanel>
        <!-- 内容 -->
    </StackPanel>
</atom:ScrollViewer>

<!-- 同时显示水平和垂直滚动条 -->
<atom:ScrollViewer HorizontalScrollBarVisibility="Auto"
                   VerticalScrollBarVisibility="Auto">
    <Canvas Width="2000" Height="2000" />
</atom:ScrollViewer>

<!-- 禁用过渡动画 -->
<atom:ScrollViewer IsMotionEnabled="False">
    <StackPanel>
        <!-- 内容 -->
    </StackPanel>
</atom:ScrollViewer>
```

> 📖 参考使用场景：`controlgallery/AtomUIGallery/Controls/ShowCasePanelTheme.axaml` 中 ScrollViewer 作为 ShowCasePanel 的滚动容器。

---

## 2. 通过附加属性控制内部 ScrollViewer

`IsLiteMode` 是一个附加属性，可以设置在任何使用 ScrollViewer 的控件上，使其内部 ScrollViewer 启用极简模式：

```xml
<!-- 在 NavMenu 上启用极简模式滚动条 -->
<atom:NavMenu atom:ScrollViewer.IsLiteMode="True"
              ScrollViewer.AllowAutoHide="True">
    <!-- 菜单项 -->
</atom:NavMenu>

<!-- 在 TreeView 上启用极简模式滚动条 -->
<atom:TreeView atom:ScrollViewer.IsLiteMode="True"
               ScrollViewer.VerticalScrollBarVisibility="Auto">
    <!-- 树节点 -->
</atom:TreeView>
```

> 📖 参考：`controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml` 中 NavMenu 使用附加属性启用极简模式。

---

## 3. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 ScrollViewer 及其子控件进行全局或局部样式覆盖：

### 全局设置极简模式

```xml
<Window.Styles>
    <!-- 所有 ScrollViewer 默认使用极简模式 -->
    <Style Selector="atom|ScrollViewer">
        <Setter Property="IsLiteMode" Value="True" />
    </Style>
</Window.Styles>
```

### 全局禁用自动隐藏

```xml
<Window.Styles>
    <Style Selector="atom|ScrollViewer">
        <Setter Property="AllowAutoHide" Value="False" />
    </Style>
</Window.Styles>
```

### 自定义滑块背景色

```xml
<Window.Styles>
    <!-- 覆盖滑块正常态背景色 -->
    <Style Selector="atom|ScrollBarThumb">
        <Setter Property="Background" Value="#999999" />
    </Style>
    
    <!-- 覆盖滑块悬浮态背景色 -->
    <Style Selector="atom|ScrollBarThumb:pointerover">
        <Setter Property="Background" Value="#666666" />
    </Style>
    
    <!-- 覆盖滑块按下态背景色 -->
    <Style Selector="atom|ScrollBarThumb:pressed">
        <Setter Property="Background" Value="#333333" />
    </Style>
</Window.Styles>
```

---

## 4. 通过 ControlTheme 完全替换主题

如果需要彻底替换 ScrollViewer 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomScrollViewer" TargetType="atom:ScrollViewer">
    <Setter Property="Template">
        <ControlTemplate>
            <Grid ColumnDefinitions="*,Auto" RowDefinitions="*,Auto">
                <ScrollContentPresenter Name="PART_ContentPresenter"
                                        Background="{TemplateBinding Background}"
                                        Padding="{TemplateBinding Padding}" />
                <!-- 自定义滚动条布局 -->
            </Grid>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:ScrollViewer Theme="{StaticResource MyCustomScrollViewer}">
    <StackPanel><!-- 内容 --></StackPanel>
</atom:ScrollViewer>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的自动隐藏逻辑、极简模式切换和过渡动画。建议优先使用 Style 覆盖。

---

## 5. 在 ShowCasePanel 等容器中使用

ScrollViewer 常用作其他容器控件的滚动外壳。以下示例摘自 Gallery 的 ShowCasePanel 模板：

```xml
<ControlTheme TargetType="gallery:ShowCasePanel">
    <Setter Property="Template">
        <ControlTemplate>
            <atom:ScrollViewer VerticalScrollBarVisibility="Auto"
                               AllowAutoHide="False"
                               BringIntoViewOnFocusChange="False"
                               IsLiteMode="False">
                <Grid Margin="5" Name="PART_MainPanel"
                      RowSpacing="15" ColumnSpacing="15">
                    <!-- 内容 -->
                </Grid>
            </atom:ScrollViewer>
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

> 📖 参考：`controlgallery/AtomUIGallery/Controls/ShowCasePanelTheme.axaml`

---

## 6. 控制动画行为

```xml
<!-- 禁用所有过渡动画（滚动条显隐不再渐变） -->
<atom:ScrollViewer IsMotionEnabled="False">
    <StackPanel><!-- 内容 --></StackPanel>
</atom:ScrollViewer>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|ScrollViewer` 语法引用 `atom` XML 命名空间下的控件类型，其中 `|` 是命名空间分隔符。

### ScrollViewer 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ScrollViewer` | 匹配所有 AtomUI ScrollViewer 实例 |
| `atom\|ScrollViewer[IsLiteMode=True]` | 匹配极简模式的 ScrollViewer |
| `atom\|ScrollViewer[IsLiteMode=False]` | 匹配标准模式的 ScrollViewer |
| `atom\|ScrollViewer[AllowAutoHide=True]` | 匹配启用自动隐藏的 ScrollViewer |
| `atom\|ScrollViewer[IsExpanded=true]` | 匹配滚动条处于展开状态的 ScrollViewer |

### ScrollBar 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ScrollBar` | 匹配所有 AtomUI ScrollBar 实例 |
| `atom\|ScrollBar:vertical` | 匹配垂直方向的 ScrollBar |
| `atom\|ScrollBar:horizontal` | 匹配水平方向的 ScrollBar |
| `atom\|ScrollBar[IsLiteMode=True]` | 匹配极简模式的 ScrollBar |

### ScrollBarThumb 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ScrollBarThumb` | 匹配所有 AtomUI ScrollBarThumb 实例 |
| `atom\|ScrollBarThumb:pointerover` | 匹配鼠标悬浮状态的滑块，此时滑块使用 `ThumbHoverBg` 背景色 |
| `atom\|ScrollBarThumb:pressed` | 匹配按下/拖拽状态的滑块，此时滑块使用 `ThumbActiveBg` 背景色 |
| `atom\|ScrollBarThumb[Orientation=Vertical]` | 匹配垂直方向的滑块 |
| `atom\|ScrollBarThumb[Orientation=Horizontal]` | 匹配水平方向的滑块 |
| `atom\|ScrollBarThumb[IsLiteMode=True]` | 匹配极简模式下的滑块（初始为细线） |
| `atom\|ScrollBarThumb[IsLiteMode=True][IsExpanded=True]` | 匹配极简模式下膨胀状态的滑块（悬浮/拖拽时） |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|ScrollBar:vertical /template/ Border#Frame` | 访问垂直滚动条模板内的 Frame Border |
| `atom\|ScrollViewer /template/ Panel#ScrollBarsSeparator` | 访问滚动条交叉区域的分隔面板 |
| `atom\|ScrollViewer[AllowAutoHide=True] /template/ ScrollContentPresenter#PART_ContentPresenter` | 自动隐藏模式下的内容呈现器 |
