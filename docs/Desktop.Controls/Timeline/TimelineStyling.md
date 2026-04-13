# Timeline 自定义样式指南

Timeline 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 通过属性直接控制

最简单的方式是通过 Timeline 和 TimelineItem 的公共属性来控制外观：

```xml
<!-- 基本时间轴 -->
<atom:Timeline>
    <atom:TimelineItem>事件一</atom:TimelineItem>
    <atom:TimelineItem>事件二</atom:TimelineItem>
</atom:Timeline>

<!-- 自定义指示器颜色 -->
<atom:Timeline>
    <atom:TimelineItem IndicatorColor="green">成功</atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="red">失败</atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="gray">未完成</atom:TimelineItem>
</atom:Timeline>

<!-- 自定义图标 -->
<atom:TimelineItem IndicatorIcon="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
                   IndicatorColor="Red">
    进行中
</atom:TimelineItem>

<!-- 交替布局 -->
<atom:Timeline Mode="Alternate">
    <atom:TimelineItem>左侧内容</atom:TimelineItem>
    <atom:TimelineItem>右侧内容</atom:TimelineItem>
</atom:Timeline>

<!-- 带标签的时间轴 -->
<atom:Timeline>
    <atom:TimelineItem Label="2024-01-01">创建项目</atom:TimelineItem>
    <atom:TimelineItem Label="2024-08-12">正式开源</atom:TimelineItem>
</atom:Timeline>

<!-- 待办节点 -->
<atom:Timeline Pending="Recording...">
    <atom:TimelineItem>已完成事件</atom:TimelineItem>
</atom:Timeline>
```

---

## 2. 通过 Style 覆盖样式

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|TimelineItem">
        <Setter Property="Margin" Value="0,0,0,4" />
    </Style>
</Window.Styles>
```

### 按位置定制

```xml
<!-- 首节点加粗 -->
<Style Selector="atom|TimelineItem:order-first">
    <Setter Property="FontWeight" Value="Bold" />
</Style>

<!-- 待办节点半透明 -->
<Style Selector="atom|TimelineItem:pending-item">
    <Setter Property="Opacity" Value="0.6" />
</Style>

<!-- 末节点样式 -->
<Style Selector="atom|TimelineItem:order-last">
    <Setter Property="FontStyle" Value="Italic" />
</Style>
```

### 标签文本样式

```xml
<!-- 标签文本使用次要颜色 -->
<Style Selector="atom|TimelineItem /template/ atom|TextBlock#Label">
    <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorTextSecondary}" />
    <Setter Property="FontSize" Value="{atom:SharedTokenResource FontSizeSM}" />
</Style>
```

### 内容区域样式

```xml
<!-- 内容区域使用自定义字号 -->
<Style Selector="atom|TimelineItem /template/ ContentPresenter#ContentPresenter">
    <Setter Property="FontSize" Value="14" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

> ⚠️ 注意：完全替换 ControlTheme 需要理解 Timeline 的三层模板结构（Timeline / TimelineItem / TimelineIndicator），建议优先使用 Style 覆盖。

```xml
<!-- 仅替换 Timeline 的容器主题 -->
<ControlTheme x:Key="MyTimeline" TargetType="atom:Timeline">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    Padding="{TemplateBinding Padding}">
                <ItemsPresenter Name="ItemsPresenter">
                    <ItemsPresenter.ItemsPanel>
                        <ItemsPanelTemplate>
                            <StackPanel />
                        </ItemsPanelTemplate>
                    </ItemsPresenter.ItemsPanel>
                </ItemsPresenter>
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Timeline` 语法引用 `atom` XML 命名空间下的类型，其中 `|` 是命名空间分隔符。

### Timeline 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Timeline` | 匹配所有 Timeline 实例 |
| `atom\|Timeline[Mode=Alternate]` | 匹配交替布局的时间轴 |
| `atom\|Timeline[Mode=Right]` | 匹配右对齐的时间轴 |
| `atom\|Timeline[IsReverse=True]` | 匹配反转排列的时间轴 |

### TimelineItem 位置伪类选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TimelineItem:order-first` | 第一个节点 |
| `atom\|TimelineItem:order-last` | 最后一个节点 |
| `atom\|TimelineItem:order-odd` | 奇数位置节点（交替布局中的右侧内容） |
| `atom\|TimelineItem:order-even` | 偶数位置节点（交替布局中的左侧内容） |
| `atom\|TimelineItem:pending-item` | 待办节点 |

### TimelineItem 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TimelineItem /template/ atom\|TextBlock#Label` | 标签文本部件 |
| `atom\|TimelineItem /template/ ContentPresenter#ContentPresenter` | 内容呈现部件 |
| `atom\|TimelineItem /template/ atomc\|TimelineIndicator#Indicator` | 指示器部件 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|TimelineItem:order-first:not(:pending-item)` | 非待办的第一个节点 |
| `atom\|TimelineItem:pending-item /template/ ContentPresenter#ContentPresenter` | 待办节点的内容区域 |
