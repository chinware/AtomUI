# Empty 自定义样式指南

Empty 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Empty 的公共属性来控制外观：

```xml
<!-- 默认空状态 -->
<atom:Empty PresetImage="Default" />

<!-- 简洁空状态 -->
<atom:Empty PresetImage="Simple" />

<!-- 自定义描述 -->
<atom:Empty PresetImage="Simple" Description="暂无数据" />

<!-- 隐藏描述 -->
<atom:Empty PresetImage="Default" IsShowDescription="False" />

<!-- 小尺寸 -->
<atom:Empty PresetImage="Simple" SizeType="Small" />

<!-- 大尺寸 + 自定义描述 -->
<atom:Empty PresetImage="Default" SizeType="Large" Description="No items found" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/EmptyShowCase.axaml`

---

## 2. 使用自定义图片

```xml
<!-- SVG 文件路径（avares 协议） -->
<atom:Empty ImagePath="avares://MyApp/Assets/custom-empty.svg"
            SizeType="Large"
            Description="Custom illustration" />

<!-- 无预设图片，无文件路径，仅自定义文字 -->
<atom:Empty Description="Nothing to show here" />
```

---

## 3. 通过 Style 覆盖

### 全局设置默认预设图片

```xml
<Window.Styles>
    <Style Selector="atom|Empty">
        <Setter Property="SizeType" Value="Middle" />
    </Style>
</Window.Styles>
```

### 调整描述文字样式

通过模板选择器访问描述文字 `TextBlock`：

```xml
<Style Selector="atom|Empty /template/ atom|TextBlock#Description">
    <Setter Property="FontSize" Value="16" />
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

### 调整图片高度

通过模板选择器访问 SVG 控件：

```xml
<!-- 自定义图片高度（覆盖 Token 值） -->
<Style Selector="atom|Empty /template/ Svg#PART_SvgImage">
    <Setter Property="Height" Value="120" />
</Style>
```

### 按尺寸定制

```xml
<!-- 小尺寸时隐藏描述文字 -->
<Style Selector="atom|Empty[SizeType=Small] /template/ atom|TextBlock#Description">
    <Setter Property="IsVisible" Value="False" />
</Style>

<!-- 大尺寸时加粗描述文字 -->
<Style Selector="atom|Empty[SizeType=Large] /template/ atom|TextBlock#Description">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

---

## 4. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Empty 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomEmpty" TargetType="atom:Empty">
    <Setter Property="Template">
        <ControlTemplate>
            <StackPanel Orientation="Vertical"
                        HorizontalAlignment="Center"
                        VerticalAlignment="Center">
                <Svg Name="PART_SvgImage"
                     HorizontalAlignment="Center"
                     VerticalAlignment="Center" />
                <TextBlock HorizontalAlignment="Center"
                           Text="{TemplateBinding Description}"
                           Foreground="Gray"
                           FontSize="14"
                           Margin="0,12,0,0" />
            </StackPanel>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:Empty Theme="{StaticResource MyCustomEmpty}" PresetImage="Default" />
```

> ⚠️ 注意：自定义模板必须保留 `PART_SvgImage` 部件名（`Avalonia.Svg.Svg` 类型），否则预设图片和自定义 SVG 功能将失效。

---

## 5. 配合外部布局组合

Empty 本身不提供底部插槽，但可通过外部 `StackPanel` 组合额外内容：

```xml
<StackPanel Orientation="Vertical" Spacing="10" HorizontalAlignment="Center">
    <atom:Empty PresetImage="Simple" SizeType="Middle" />
    <atom:Button ButtonType="Primary" HorizontalAlignment="Center">
        Add New Item
    </atom:Button>
</StackPanel>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Empty` 语法引用 `atom` XML 命名空间下的 `Empty` 类型，其中 `|` 是命名空间分隔符。

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|Empty` | 匹配所有 Empty 实例 |
| `atom\|Empty[SizeType=Large]` | 匹配大号 Empty |
| `atom\|Empty[SizeType=Middle]` | 匹配中号 Empty（默认） |
| `atom\|Empty[SizeType=Small]` | 匹配小号 Empty |

### 模板部件选择

| 选择器 | 说明 |
|---|---|
| `atom\|Empty /template/ Svg#PART_SvgImage` | 访问 SVG 图片控件（可设置 Height 等） |
| `atom\|Empty /template/ atom\|TextBlock#Description` | 访问描述文字 TextBlock（可设置字体、颜色等） |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Empty[SizeType=Large] /template/ Svg#PART_SvgImage` | 大号 Empty 的图片控件 |
| `atom\|Empty[SizeType=Small] /template/ atom\|TextBlock#Description` | 小号 Empty 的描述文字 |

