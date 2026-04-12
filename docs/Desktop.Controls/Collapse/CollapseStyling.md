# Collapse 自定义样式指南

Collapse 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Collapse 的公共属性来控制外观：

```xml
<!-- 默认模式 -->
<atom:Collapse>
    <atom:CollapseItem Header="Panel 1">Content 1</atom:CollapseItem>
    <atom:CollapseItem Header="Panel 2">Content 2</atom:CollapseItem>
</atom:Collapse>

<!-- 手风琴模式 -->
<atom:Collapse IsAccordion="True">
    <atom:CollapseItem Header="Panel 1" IsSelected="True">Content 1</atom:CollapseItem>
    <atom:CollapseItem Header="Panel 2">Content 2</atom:CollapseItem>
</atom:Collapse>

<!-- 无边框 -->
<atom:Collapse IsBorderless="True">...</atom:Collapse>

<!-- 幽灵模式 -->
<atom:Collapse IsGhostStyle="True">...</atom:Collapse>

<!-- 三种尺寸 -->
<atom:Collapse SizeType="Small">...</atom:Collapse>
<atom:Collapse SizeType="Large">...</atom:Collapse>

<!-- 展开图标在右侧 -->
<atom:Collapse ExpandIconPosition="End">...</atom:Collapse>

<!-- 仅点击图标触发 -->
<atom:Collapse TriggerType="Icon">...</atom:Collapse>

<!-- 带附加内容的面板项 -->
<atom:Collapse>
    <atom:CollapseItem Header="Panel with extra"
                       AddOnContent="{antdicons:AntDesignIconProvider Kind=SettingOutlined}">
        Content
    </atom:CollapseItem>
</atom:Collapse>

<!-- 自定义头部和内容间距 -->
<atom:Collapse ItemHeaderPadding="5" ItemContentPadding="5">
    <atom:CollapseItem Header="Panel 1">Content 1</atom:CollapseItem>
</atom:Collapse>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CollapseShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Collapse 进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|Collapse">
        <Setter Property="Margin" Value="0,0,0,16" />
    </Style>
</Window.Styles>
```

### 自定义头部背景色

```xml
<Style Selector="atom|CollapseItem /template/ Border#PART_HeaderDecorator">
    <Setter Property="Background" Value="{atom:SharedTokenResource ColorPrimaryBg}" />
</Style>
```

### 展开状态的头部高亮

```xml
<Style Selector="atom|CollapseItem:selected /template/ Border#PART_HeaderDecorator">
    <Setter Property="Background" Value="{atom:SharedTokenResource ColorPrimaryBgHover}" />
</Style>
```

### 自定义头部标题颜色

```xml
<Style Selector="atom|CollapseItem /template/ ContentPresenter#PART_HeaderPresenter">
    <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorPrimary}" />
</Style>
```

### 自定义内容区域背景

```xml
<Style Selector="atom|CollapseItem /template/ ContentPresenter#PART_ContentPresenter">
    <Setter Property="Background" Value="{atom:SharedTokenResource ColorBgLayout}" />
</Style>
```

### 使用属性选择器按模式定制

```xml
<!-- 幽灵模式下的头部字体加粗 -->
<Style Selector="atom|Collapse[IsGhostStyle=True] atom|CollapseItem /template/ ContentPresenter#PART_HeaderPresenter">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>

<!-- 大号面板使用不同的内容间距 -->
<Style Selector="atom|Collapse[SizeType=Large] atom|CollapseItem /template/ ContentPresenter#PART_ContentPresenter">
    <Setter Property="Padding" Value="24" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Collapse 或 CollapseItem 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomCollapseItem" TargetType="atom:CollapseItem">
    <Setter Property="Template">
        <ControlTemplate>
            <DockPanel LastChildFill="True">
                <Border DockPanel.Dock="Top"
                        Background="{TemplateBinding Background}"
                        Padding="{TemplateBinding HeaderPadding}">
                    <ContentPresenter Content="{TemplateBinding Header}" />
                </Border>
                <ContentPresenter Content="{TemplateBinding Content}"
                                  Padding="{TemplateBinding ContentPadding}" />
            </DockPanel>
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的折叠/展开动画（`MotionActor`）、展开图标按钮、附加内容区域等功能。建议优先使用 Style 覆盖。

---

## 4. 嵌套折叠面板

Collapse 支持嵌套使用，内层折叠面板作为外层 CollapseItem 的内容：

```xml
<atom:Collapse>
    <atom:CollapseItem Header="Outer Panel 1">
        <atom:Collapse>
            <atom:CollapseItem Header="Inner Panel 1">
                <TextBlock TextWrapping="Wrap">Nested content</TextBlock>
            </atom:CollapseItem>
        </atom:Collapse>
    </atom:CollapseItem>
    <atom:CollapseItem Header="Outer Panel 2">
        <TextBlock TextWrapping="Wrap">Regular content</TextBlock>
    </atom:CollapseItem>
</atom:Collapse>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CollapseShowCase.axaml` 中 "Nested panel" 示例。

---

## 5. 附加内容（AddOnContent）

在头部右侧放置操作按钮或图标：

```xml
<atom:Collapse ExpandIconPosition="End">
    <atom:CollapseItem Header="Panel with settings icon"
                       AddOnContent="{antdicons:AntDesignIconProvider Kind=SettingOutlined}">
        <TextBlock TextWrapping="Wrap">Content with extra action</TextBlock>
    </atom:CollapseItem>
</atom:Collapse>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CollapseShowCase.axaml` 中 "Expand icon location" 示例。

---

## 6. 禁用状态

```xml
<!-- 禁用整个折叠面板 -->
<atom:Collapse IsEnabled="False">
    <atom:CollapseItem Header="This panel can't be collapsed">
        <TextBlock TextWrapping="Wrap">Content is visible but panel cannot be toggled</TextBlock>
    </atom:CollapseItem>
</atom:Collapse>

<!-- 禁用但保持展开 -->
<atom:Collapse IsEnabled="False">
    <atom:CollapseItem Header="Disabled but expanded" IsSelected="True">
        <TextBlock TextWrapping="Wrap">This content stays visible</TextBlock>
    </atom:CollapseItem>
</atom:Collapse>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CollapseShowCase.axaml` 中 "Collapsible" 示例。

---

## 7. 控制动画行为

```xml
<!-- 禁用折叠/展开动画 -->
<atom:Collapse IsMotionEnabled="False">
    <atom:CollapseItem Header="No animation">Content</atom:CollapseItem>
</atom:Collapse>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Collapse` 语法引用 `atom` XML 命名空间下的 `Collapse` 类型，其中 `|` 是命名空间分隔符。

### 按样式模式选择

| 选择器 | 说明 |
|---|---|
| `atom\|Collapse` | 匹配所有 AtomUI Collapse 实例，用于设置全局通用样式（如统一 Margin 等） |
| `atom\|Collapse[IsGhostStyle=True]` | 匹配幽灵模式折叠面板（无边框无背景），可定制幽灵模式下的头部/内容样式 |
| `atom\|Collapse[IsBorderless=True]` | 匹配无边框模式折叠面板，可定制无边框模式下的内容背景等 |
| `atom\|Collapse[IsAccordion=True]` | 匹配手风琴模式折叠面板 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|Collapse[SizeType=Large]` | 匹配大号折叠面板（更大的头部字体和内间距） |
| `atom\|Collapse[SizeType=Middle]` | 匹配中号折叠面板（默认尺寸） |
| `atom\|Collapse[SizeType=Small]` | 匹配小号折叠面板（更紧凑的内间距） |

### 按触发方式和图标位置选择

| 选择器 | 说明 |
|---|---|
| `atom\|Collapse[TriggerType=Header]` | 匹配头部触发模式（默认），头部显示手形光标 |
| `atom\|Collapse[TriggerType=Icon]` | 匹配图标触发模式，仅图标显示手形光标 |
| `atom\|Collapse[ExpandIconPosition=Start]` | 匹配展开图标在左侧的折叠面板 |
| `atom\|Collapse[ExpandIconPosition=End]` | 匹配展开图标在右侧的折叠面板 |

### CollapseItem 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|CollapseItem` | 匹配所有折叠面板项 |
| `atom\|CollapseItem:selected` | 匹配已展开的面板项 |
| `atom\|CollapseItem:not(:selected)` | 匹配已折叠的面板项 |
| `atom\|CollapseItem:pressed` | 匹配被按下的面板项 |
| `atom\|CollapseItem:disabled` | 匹配禁用的面板项 |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|CollapseItem /template/ Border#PART_HeaderDecorator` | 头部装饰器 Border，可定制头部背景、边框等 |
| `atom\|CollapseItem /template/ ContentPresenter#PART_HeaderPresenter` | 标题内容展示器，可定制标题前景色、字体等 |
| `atom\|CollapseItem /template/ ContentPresenter#PART_ContentPresenter` | 内容展示器，可定制内容区域背景、边框、间距等 |
| `atom\|CollapseItem /template/ atom\|IconButton#PART_ExpandButton` | 展开/折叠图标按钮，可定制图标大小、间距等 |
| `atom\|CollapseItem /template/ ContentPresenter#PART_AddOnContentPresenter` | 附加内容展示器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|CollapseItem:selected /template/ Border#PART_HeaderDecorator` | 已展开面板的头部装饰器 |
| `atom\|CollapseItem:disabled /template/ ContentPresenter#PART_HeaderPresenter` | 禁用面板的头部标题 |
| `atom\|Collapse[IsGhostStyle=True] atom\|CollapseItem /template/ Border#PART_HeaderDecorator` | 幽灵模式下的面板头部 |
| `atom\|Collapse[SizeType=Large] atom\|CollapseItem /template/ Border#PART_HeaderDecorator` | 大号面板的头部装饰器 |
| `atom\|CollapseItem[IsShowExpandIcon=False]` | 隐藏了展开图标的面板项 |
