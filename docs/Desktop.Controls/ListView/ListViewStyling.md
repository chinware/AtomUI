# ListView 自定义样式指南

ListView 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 ListView 的公共属性来控制外观：

```xml
<!-- 基础用法 -->
<atom:ListView ItemsSource="{Binding ListItems}" />

<!-- 无边框模式 -->
<atom:ListView ItemsSource="{Binding ListItems}" IsBorderless="True" />

<!-- 不同尺寸 -->
<atom:ListView ItemsSource="{Binding ListItems}" SizeType="Large" />
<atom:ListView ItemsSource="{Binding ListItems}" SizeType="Small" />

<!-- 显示选中指示器 -->
<atom:ListView ItemsSource="{Binding ListItems}"
               IsShowSelectedIndicator="True" />

<!-- 自定义悬浮/选中背景色 -->
<atom:ListView ItemsSource="{Binding ListItems}"
               ItemHoverBg="LightBlue"
               ItemSelectedBg="LightGreen" />

<!-- 禁用选择（纯展示模式） -->
<atom:ListView ItemsSource="{Binding ListItems}" IsSelectable="False" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 ListView 进行全局或局部样式覆盖：

### 全局统一外观

```xml
<Window.Styles>
    <Style Selector="atom|ListView">
        <Setter Property="Margin" Value="10" />
        <Setter Property="BorderBrush" Value="Gray" />
    </Style>
</Window.Styles>
```

### 按尺寸定制

```xml
<Style Selector="atom|ListView[SizeType=Large]">
    <Setter Property="FontSize" Value="16" />
</Style>

<Style Selector="atom|ListView[SizeType=Small]">
    <Setter Property="FontSize" Value="12" />
</Style>
```

### 空状态下的特殊样式

```xml
<!-- 数据为空时增加最小高度 -->
<Style Selector="atom|ListView:empty">
    <Setter Property="MinHeight" Value="200" />
</Style>
```

### 自定义 ListViewItem 样式

```xml
<!-- 所有列表项添加底部边框线 -->
<Style Selector="atom|ListViewItem">
    <Setter Property="BorderThickness" Value="0,0,0,1" />
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorderSecondary}" />
</Style>

<!-- 选中项使用粗体 -->
<Style Selector="atom|ListViewItem:selected">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

---

## 3. 自定义 ItemTemplate

通过 `ItemTemplate` 完全定制每个列表项的展示内容：

```xml
<atom:ListView ItemsSource="{Binding ListItems}">
    <atom:ListView.ItemTemplate>
        <DataTemplate x:DataType="atom:IListItemData">
            <StackPanel Orientation="Horizontal" Spacing="8">
                <atom:Avatar SizeType="Small" />
                <StackPanel VerticalAlignment="Center">
                    <atom:TextBlock Text="{Binding Content}" FontWeight="SemiBold" />
                    <atom:TextBlock Text="Description text"
                                    Foreground="{atom:SharedTokenResource ColorTextDescription}"
                                    FontSize="{atom:SharedTokenResource FontSizeSM}" />
                </StackPanel>
            </StackPanel>
        </DataTemplate>
    </atom:ListView.ItemTemplate>
</atom:ListView>
```

---

## 4. 自定义分组标题模板

当启用分组时，可通过 `GroupItemTemplate` 定制分组标题：

```xml
<atom:ListView ItemsSource="{Binding GroupListItems}"
               IsGroupEnabled="True">
    <atom:ListView.GroupItemTemplate>
        <DataTemplate x:DataType="atom:IListItemData">
            <StackPanel Orientation="Horizontal" Spacing="4">
                <atom:TextBlock Text="{Binding Content}"
                                FontWeight="Bold"
                                Foreground="{atom:SharedTokenResource ColorPrimary}" />
            </StackPanel>
        </DataTemplate>
    </atom:ListView.GroupItemTemplate>
</atom:ListView>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml` 中 "Group" 示例。

---

## 5. 自定义空状态指示器

```xml
<!-- 使用自定义空状态内容 -->
<atom:ListView ItemsSource="{Binding Items}"
               IsShowEmptyIndicator="True">
    <atom:ListView.EmptyIndicatorTemplate>
        <DataTemplate>
            <StackPanel HorizontalAlignment="Center" Spacing="8">
                <atom:Empty PresetImage="Default" />
                <atom:TextBlock Text="暂无数据，请点击添加"
                                HorizontalAlignment="Center" />
            </StackPanel>
        </DataTemplate>
    </atom:ListView.EmptyIndicatorTemplate>
</atom:ListView>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml` 中 "Empty" 示例。

---

## 6. 通过 ControlTheme 完全替换主题

如果需要彻底替换 ListView 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomListView" TargetType="atom:ListView">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    Padding="{TemplateBinding Padding}">
                <ScrollViewer>
                    <ItemsPresenter ItemsPanel="{TemplateBinding ItemsPanel}" />
                </ScrollViewer>
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:ListView Theme="{StaticResource MyCustomListView}"
               ItemsSource="{Binding Items}" />
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的分页器集成、空状态展示、加载状态覆盖等功能。建议优先使用 Style 覆盖和 ItemTemplate 自定义。

---

## 7. 控制动画行为

```xml
<!-- 禁用列表项背景/前景色过渡动画 -->
<atom:ListView ItemsSource="{Binding Items}" IsMotionEnabled="False" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|ListView` 语法引用 `atom` XML 命名空间下的 `ListView` 类型，其中 `|` 是命名空间分隔符。

### 按类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|ListView` | 匹配所有 AtomUI ListView 实例 |
| `atom\|ListViewItem` | 匹配所有 ListView 列表项 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|ListView[SizeType=Large]` | 匹配大号列表 |
| `atom\|ListView[SizeType=Middle]` | 匹配中号列表（默认） |
| `atom\|ListView[SizeType=Small]` | 匹配小号列表 |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|ListView[IsBorderless=True]` | 匹配无边框列表 |
| `atom\|ListView[IsGroupEnabled=True]` | 匹配启用分组的列表 |
| `atom\|ListView[IsSelectable=False]` | 匹配禁用选择的列表 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|ListView:empty` | 匹配无数据的列表（`TotalItemCount == 0`） |
| `atom\|ListView:singleitem` | 匹配仅有一项数据的列表 |

### ListViewItem 伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|ListViewItem:selected` | 匹配选中状态的列表项 |
| `atom\|ListViewItem:pointerover` | 匹配鼠标悬浮的列表项 |
| `atom\|ListViewItem:pressed` | 匹配按下状态的列表项 |
| `atom\|ListViewItem:disabled` | 匹配禁用的列表项 |

### ListViewItem 属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|ListViewItem[IsGroupItem=True]` | 匹配分组标题项 |
| `atom\|ListViewItem[IsGroupItem=False]` | 匹配非分组标题项（普通数据项） |
| `atom\|ListViewItem[SizeType=Large]` | 匹配大号列表项 |
| `atom\|ListViewItem[SizeType=Small]` | 匹配小号列表项 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|ListViewItem[IsGroupItem=False]:selected:not(:disabled)` | 非禁用状态的选中数据项 |
| `atom\|ListViewItem[IsGroupItem=False]:pointerover` | 鼠标悬浮的数据项（排除分组标题） |
| `atom\|ListView /template/ Border#Frame` | 访问 ListView 模板内的主框架 Border |
| `atom\|ListViewItem /template/ ContentPresenter#ContentPresenter` | 访问列表项模板内的内容展示器 |
