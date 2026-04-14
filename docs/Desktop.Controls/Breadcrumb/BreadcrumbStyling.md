# Breadcrumb 自定义样式指南

Breadcrumb 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Breadcrumb 的公共属性来控制外观：

```xml
<!-- 基本用法 -->
<atom:Breadcrumb>
    <atom:BreadcrumbItem>Home</atom:BreadcrumbItem>
    <atom:BreadcrumbItem NavigateContext="#">Application Center</atom:BreadcrumbItem>
    <atom:BreadcrumbItem>An Application</atom:BreadcrumbItem>
</atom:Breadcrumb>

<!-- 自定义全局分隔符 -->
<atom:Breadcrumb Separator=">">
    <atom:BreadcrumbItem>Home</atom:BreadcrumbItem>
    <atom:BreadcrumbItem>Application</atom:BreadcrumbItem>
</atom:Breadcrumb>

<!-- 带图标 -->
<atom:Breadcrumb>
    <atom:BreadcrumbItem Icon="{antdicons:AntDesignIconProvider Kind=HomeOutlined}" />
    <atom:BreadcrumbItem Icon="{antdicons:AntDesignIconProvider Kind=UserOutlined}"
                         NavigateContext="#">Users</atom:BreadcrumbItem>
    <atom:BreadcrumbItem>Profile</atom:BreadcrumbItem>
</atom:Breadcrumb>

<!-- 面包屑项独立分隔符 -->
<atom:Breadcrumb>
    <atom:BreadcrumbItem Separator=":">Location</atom:BreadcrumbItem>
    <atom:BreadcrumbItem NavigateContext="#">Application Center</atom:BreadcrumbItem>
    <atom:BreadcrumbItem>An Application</atom:BreadcrumbItem>
</atom:Breadcrumb>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/BreadcrumbShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Breadcrumb 和 BreadcrumbItem 进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|Breadcrumb">
        <Setter Property="Margin" Value="0,8" />
    </Style>
</Window.Styles>
```

### 自定义链接颜色

```xml
<!-- 可导航面包屑项的悬浮颜色 -->
<Style Selector="atom|BreadcrumbItem[IsNavigateResponsive=True]:pointerover">
    <Setter Property="Foreground" Value="#1677ff" />
</Style>
```

### 自定义最后一项样式

```xml
<!-- 最后一项加粗 -->
<Style Selector="atom|BreadcrumbItem:is-last">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

### 使用伪类选择器

```xml
<!-- 带图标的面包屑项额外样式 -->
<Style Selector="atom|BreadcrumbItem:has-icon">
    <Setter Property="FontWeight" Value="Medium" />
</Style>

<!-- 禁用状态 -->
<Style Selector="atom|BreadcrumbItem:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Breadcrumb 或 BreadcrumbItem 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomBreadcrumbItem" TargetType="atom:BreadcrumbItem">
    <Setter Property="Template">
        <ControlTemplate>
            <StackPanel Orientation="Horizontal">
                <ContentPresenter Content="{TemplateBinding Content}" />
                <TextBlock Text=">" Margin="4,0"
                           IsVisible="{TemplateBinding IsLast, Converter={x:Static BoolConverters.Not}}" />
            </StackPanel>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:Breadcrumb>
    <atom:BreadcrumbItem Theme="{StaticResource MyCustomBreadcrumbItem}">
        Home
    </atom:BreadcrumbItem>
</atom:Breadcrumb>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的悬浮效果、图标展示器等功能。建议优先使用 Style 覆盖。

---

## 4. 控制动画行为

```xml
<!-- 禁用过渡动画（前景色、背景色不再渐变过渡） -->
<atom:Breadcrumb IsMotionEnabled="False">
    <atom:BreadcrumbItem NavigateContext="#">Home</atom:BreadcrumbItem>
    <atom:BreadcrumbItem>Current</atom:BreadcrumbItem>
</atom:Breadcrumb>
```

---

## 5. 自定义分隔符模板

使用 `SeparatorTemplate` 实现更复杂的分隔符样式（如图标作为分隔符）：

```xml
<atom:Breadcrumb>
    <atom:Breadcrumb.SeparatorTemplate>
        <DataTemplate>
            <antdicons:RightOutlined Width="12" Height="12" />
        </DataTemplate>
    </atom:Breadcrumb.SeparatorTemplate>
    <atom:BreadcrumbItem>Home</atom:BreadcrumbItem>
    <atom:BreadcrumbItem NavigateContext="#">Application Center</atom:BreadcrumbItem>
    <atom:BreadcrumbItem>An Application</atom:BreadcrumbItem>
</atom:Breadcrumb>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Breadcrumb` 语法引用 `atom` XML 命名空间下的类型，其中 `|` 是命名空间分隔符。

### Breadcrumb 容器选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Breadcrumb` | 匹配所有 Breadcrumb 实例，用于设置全局通用样式（如统一 Margin） |
| `atom\|Breadcrumb:disabled` | 匹配禁用状态的面包屑 |

### BreadcrumbItem 基本选择器

| 选择器 | 说明 |
|---|---|
| `atom\|BreadcrumbItem` | 匹配所有面包屑项实例 |
| `atom\|BreadcrumbItem:is-last` | 匹配最后一项（当前页面），由父 Breadcrumb 自动标记 |
| `atom\|BreadcrumbItem:has-icon` | 匹配设有图标的面包屑项 |

### BreadcrumbItem 交互状态选择器

| 选择器 | 说明 |
|---|---|
| `atom\|BreadcrumbItem[IsNavigateResponsive=True]` | 匹配可导航的面包屑项（设有 `NavigateUri` 或 `NavigateContext`），光标为手形 |
| `atom\|BreadcrumbItem[IsNavigateResponsive=True]:pointerover` | 匹配可导航面包屑项的悬浮状态，此时显示悬浮背景色和深色文字 |
| `atom\|BreadcrumbItem:pressed` | 匹配按下状态 |
| `atom\|BreadcrumbItem:disabled` | 匹配禁用状态 |
| `atom\|BreadcrumbItem:focus-visible` | 匹配通过键盘获得焦点的状态 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|BreadcrumbItem[IsLast=True][IsNavigateResponsive=False]` | 匹配不可导航的最后一项（当前页面，深色文字） |
| `atom\|BreadcrumbItem:has-icon:is-last` | 匹配带图标的最后一项 |
| `atom\|BreadcrumbItem /template/ atom\|IconPresenter#IconPresenter` | 访问模板内的图标展示器部件 |
| `atom\|BreadcrumbItem /template/ ContentPresenter#Separator` | 访问模板内的分隔符展示器部件 |
| `atom\|BreadcrumbItem /template/ Border#ContentInfoFrame` | 访问模板内的内容框架 Border |
