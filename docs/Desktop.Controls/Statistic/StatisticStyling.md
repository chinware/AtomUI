# Statistic 自定义样式指南

Statistic 控件家族的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Statistic 的公共属性来控制外观：

```xml
<!-- 基础用法 -->
<atom:Statistic Header="Active Users" Value="112893" />

<!-- 控制小数精度 -->
<atom:Statistic Header="Account Balance (CNY)" Value="112893" Precision="2" />

<!-- 自定义分隔符 -->
<atom:Statistic Header="Population" Value="1400000000" GroupSeparator=" " />

<!-- 自定义数值颜色 -->
<atom:Statistic Header="Active" Value="11.28" Precision="2"
                ContentForeground="#3f8600" />

<!-- 自定义数值字体大小 -->
<atom:Statistic Header="Small Stat" Value="42" ContentFontSize="20" />

<!-- 加载状态 -->
<atom:Statistic Header="Active Users" Value="112893" IsLoading="True" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/StatisticShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Statistic 进行全局或局部样式覆盖：

### 全局统一数值颜色

```xml
<Window.Styles>
    <Style Selector="atom|Statistic">
        <Setter Property="ContentForeground" Value="#1677ff" />
    </Style>
</Window.Styles>
```

### 定制标题样式

```xml
<!-- 修改标题字体大小和颜色 -->
<Style Selector="atom|Statistic /template/ ContentPresenter#HeaderPresenter">
    <Setter Property="FontSize" Value="16" />
    <Setter Property="Foreground" Value="#333333" />
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

### 定制数值区域间距

```xml
<!-- 增大标题与数值之间的间距 -->
<Style Selector="atom|Statistic /template/ StackPanel#RootLayout">
    <Setter Property="Spacing" Value="8" />
</Style>

<!-- 增大前缀/数值/后缀之间的间距 -->
<Style Selector="atom|Statistic /template/ StackPanel#ContentLayout">
    <Setter Property="Spacing" Value="4" />
</Style>
```

### 加载态样式定制

```xml
<!-- 加载态下增大标题与骨架屏的间距 -->
<Style Selector="atom|Statistic[IsLoading=True] /template/ StackPanel#RootLayout">
    <Setter Property="Spacing" Value="12" />
</Style>
```

### 前缀/后缀图标样式

```xml
<!-- 调整前缀/后缀中图标的大小 -->
<Style Selector="atom|Statistic /template/ atom|Icon">
    <Setter Property="Width" Value="28" />
    <Setter Property="Height" Value="28" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Statistic 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomStatistic" TargetType="atom:Statistic">
    <Setter Property="Template">
        <ControlTemplate>
            <StackPanel Spacing="4">
                <ContentPresenter Content="{TemplateBinding Header}"
                                  FontSize="12" Foreground="Gray" />
                <ContentPresenter Content="{TemplateBinding Content}"
                                  FontSize="24" FontWeight="Bold" />
            </StackPanel>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:Statistic Theme="{StaticResource MyCustomStatistic}"
                Header="Total" Value="12345" />
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的加载骨架屏、前缀/后缀展示等功能。建议优先使用 Style 覆盖。

---

## 4. 在 Card 中使用

Statistic 常与 Card 组合使用，在卡片中展示统计数据：

```xml
<Border Background="#F0F2F5" Padding="20, 30">
    <UniformGrid Columns="2" ColumnSpacing="20">
        <atom:Card SizeType="Large">
            <atom:Statistic Header="Active" Value="11.28" Precision="2"
                            ValuePrefixAddOn="{antdicons:AntDesignIconProvider ArrowUpOutlined}"
                            ValueSuffixAddOn="%"
                            ContentForeground="#3f8600" />
        </atom:Card>
        <atom:Card SizeType="Large">
            <atom:Statistic Header="Idle" Value="9.3" Precision="2"
                            ValuePrefixAddOn="{antdicons:AntDesignIconProvider ArrowDownOutlined}"
                            ValueSuffixAddOn="%"
                            ContentForeground="#cf1322" />
        </atom:Card>
    </UniformGrid>
</Border>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/StatisticShowCase.axaml` 中 "In Card" 示例。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Statistic` 语法引用 `atom` XML 命名空间下的类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|Statistic` | 匹配所有 Statistic 实例 |
| `atom\|TimerStatistic` | 匹配所有 TimerStatistic 实例 |
| `atom\|StatisticCountUp` | 匹配所有 StatisticCountUp 实例 |
| `:is(atom\|AbstractStatistic)` | 匹配所有继承自 AbstractStatistic 的控件（Statistic + TimerStatistic） |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|Statistic[IsLoading=True]` | 匹配加载中的 Statistic |
| `atom\|TimerStatistic[IsLoading=True]` | 匹配加载中的 TimerStatistic |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Statistic /template/ Border#Frame` | 最外层边框 |
| `atom\|Statistic /template/ StackPanel#RootLayout` | 根布局（垂直排列：标题 + 内容） |
| `atom\|Statistic /template/ ContentPresenter#HeaderPresenter` | 标题展示器 |
| `atom\|Statistic /template/ StackPanel#ContentLayout` | 内容布局（水平排列：前缀 + 数值 + 后缀） |
| `atom\|Statistic /template/ ContentPresenter#ValuePrefixAddOn` | 前缀展示器 |
| `atom\|Statistic /template/ ContentPresenter#ContentPresenter` | 主内容展示器 |
| `atom\|Statistic /template/ ContentPresenter#ValueSuffixAddOn` | 后缀展示器 |
| `atom\|Statistic /template/ atom\|Skeleton` | 加载骨架屏 |
| `atom\|Statistic /template/ atom\|Icon` | 模板内的图标（前缀/后缀中的图标） |
