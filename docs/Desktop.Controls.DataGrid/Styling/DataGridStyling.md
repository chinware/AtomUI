# DataGrid 自定义样式指南

## 1. 样式系统概述

DataGrid 使用 AtomUI 的 Design Token 体系管理视觉表现。控件的主题定义位于 `src/AtomUI.Desktop.Controls.DataGrid/Themes/DataGrid.axaml`，通过 `ControlTheme` 和 Token 资源键实现样式定制。

### 控件主题层级

| 控件 | ControlTheme 键 | 说明 |
|------|-----------------|------|
| `DataGrid` | `DataGrid` | 主控件主题 |
| `DataGridRow` | `DataGridRow` | 行主题 |
| `DataGridCell` | `DataGridCell` | 单元格主题 |
| `DataGridColumnHeader` | `DataGridColumnHeader` | 列头主题 |
| `DataGridRowHeader` | `DataGridRowHeader` | 行头主题 |
| `DataGridRowGroupHeader` | `DataGridRowGroupHeader` | 分组行头主题 |
| `DataGridColumnGroupHeader` | `DataGridColumnGroupHeader` | 分组列头主题 |

---

## 2. 通过 ControlTheme 覆盖样式

### 覆盖 DataGrid 主控件样式

```xml
<Style Selector="atom|DataGrid">
    <Setter Property="Background" Value="{DynamicResource DataGridTokenResourceExtension.TableBg}" />
    <Setter Property="BorderBrush" Value="{DynamicResource DataGridTokenResourceExtension.TableBorderColor}" />
    <Setter Property="BorderThickness" Value="1" />
    <Setter Property="CornerRadius" Value="{DynamicResource DataGridTokenResourceExtension.TableRadius}" />
</Style>
```

### 覆盖行样式

```xml
<!-- 普通行 -->
<Style Selector="atom|DataGridRow">
    <Setter Property="Background" Value="Transparent" />
</Style>

<!-- 悬浮行 -->
<Style Selector="atom|DataGridRow:pointerover">
    <Setter Property="Background" Value="{DynamicResource DataGridTokenResourceExtension.TableRowHoverBg}" />
</Style>

<!-- 选中行 -->
<Style Selector="atom|DataGridRow:selected">
    <Setter Property="Background" Value="{DynamicResource DataGridTokenResourceExtension.TableSelectedRowBg}" />
</Style>

<!-- 选中+悬浮行 -->
<Style Selector="atom|DataGridRow:selected:pointerover">
    <Setter Property="Background" Value="{DynamicResource DataGridTokenResourceExtension.TableSelectedRowHoverBg}" />
</Style>
```

### 覆盖列头样式

```xml
<!-- 普通列头 -->
<Style Selector="atom|DataGridColumnHeader">
    <Setter Property="Background" Value="{DynamicResource DataGridTokenResourceExtension.TableHeaderBg}" />
    <Setter Property="Foreground" Value="{DynamicResource DataGridTokenResourceExtension.TableHeaderTextColor}" />
</Style>

<!-- 悬浮列头 -->
<Style Selector="atom|DataGridColumnHeader:pointerover">
    <Setter Property="Background" Value="{DynamicResource DataGridTokenResourceExtension.TableHeaderSortHoverBg}" />
</Style>

<!-- 排序激活列头 -->
<Style Selector="atom|DataGridColumnHeader:sorted">
    <Setter Property="Background" Value="{DynamicResource DataGridTokenResourceExtension.TableHeaderSortBg}" />
</Style>
```

### 覆盖单元格样式

```xml
<!-- 普通单元格 -->
<Style Selector="atom|DataGridCell">
    <Setter Property="Padding" Value="{DynamicResource DataGridTokenResourceExtension.TablePadding}" />
    <Setter Property="FontSize" Value="{DynamicResource DataGridTokenResourceExtension.TableFontSize}" />
</Style>

<!-- 编辑中单元格 -->
<Style Selector="atom|DataGridCell:editing">
    <Setter Property="Padding" Value="0" />
</Style>
```

---

## 3. 通过 Token 资源键定制

### 使用 Token 资源扩展

在 AXAML 中通过 `DataGridTokenResourceExtension` 引用 Token 值：

```xml
<Border Background="{DynamicResource DataGridTokenResourceExtension.HeaderBg}"
        CornerRadius="{DynamicResource DataGridTokenResourceExtension.HeaderBorderRadius}" />
```

### 在 Style 中引用 Token

```xml
<Style Selector="atom|DataGridColumnHeader">
    <Setter Property="Background" 
            Value="{DynamicResource {x:Static atom:DataGridTokenResourceExtension.HeaderBg}}" />
</Style>
```

---

## 4. 尺寸相关样式

DataGrid 通过 `SizeType` 属性切换尺寸，不同尺寸影响以下 Token：

| Token | Large | Middle | Small |
|-------|-------|--------|-------|
| `CellPadding` / `TablePadding` | `SharedToken.Padding` | `SharedToken.PaddingSM` | `SharedToken.PaddingXS` |
| `CellFontSize` / `TableFontSize` | `SharedToken.FontSize` | `SharedToken.FontSize` | `SharedToken.FontSize` |

### 自定义尺寸样式

```xml
<!-- 小尺寸表格 -->
<Style Selector="atom|DataGrid[SizeType=Small] atom|DataGridCell">
    <Setter Property="Padding" Value="8,4" />
    <Setter Property="FontSize" Value="12" />
</Style>
```

---

## 5. 固定列阴影样式

固定列通过 `LeftFrozenShadows` 和 `RightFrozenShadows` Token 控制阴影效果：

```xml
<!-- 左侧固定列阴影 -->
<Border BoxShadow="{DynamicResource DataGridTokenResourceExtension.LeftFrozenShadows}" />

<!-- 右侧固定列阴影 -->
<Border BoxShadow="{DynamicResource DataGridTokenResourceExtension.RightFrozenShadows}" />
```

默认阴影参数：
- **左侧阴影**：OffsetX=-10, OffsetY=0, Blur=8, Spread=0, Color=`SharedToken.ColorSplit`
- **右侧阴影**：OffsetX=10, OffsetY=0, Blur=8, Spread=0, Color=`SharedToken.ColorSplit`

---

## 6. 列级样式定制

### 通过 CellTheme 定制列样式

每列可通过 `CellTheme` 属性设置独立的单元格主题：

```xml
<atom:DataGridTextColumn Header="Status">
    <atom:DataGridTextColumn.CellTheme>
        <ControlTheme TargetType="atom:DataGridCell">
            <Setter Property="Foreground" Value="Green" />
        </ControlTheme>
    </atom:DataGridTextColumn.CellTheme>
</atom:DataGridTextColumn>
```

### 通过 HeaderTemplate 定制列头

```xml
<atom:DataGridTextColumn Header="Name">
    <atom:DataGridTextColumn.HeaderTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" Spacing="5">
                <atom:Icon Icon="{x:Static atom:AntDesignIconPackage.UserOutlined}" />
                <TextBlock Text="Name" FontWeight="Bold" />
            </StackPanel>
        </DataTemplate>
    </atom:DataGridTextColumn.HeaderTemplate>
</atom:DataGridTextColumn>
```

---

## 7. 行级样式定制

### 通过 LoadingRow 事件

```csharp
dataGrid.LoadingRow += (s, e) => {
    if (e.Row.DataContext is DataItem item && item.IsHighlighted)
    {
        e.Row.Background = Brushes.LightYellow;
    }
};
```

### 通过 Style Selector

```xml
<!-- 交替行颜色 -->
<Style Selector="atom|DataGridRow:nth-child(odd)">
    <Setter Property="Background" Value="Transparent" />
</Style>
<Style Selector="atom|DataGridRow:nth-child(even)">
    <Setter Property="Background" Value="#FAFAFA" />
</Style>
```

---

## 8. 完整样式覆盖示例

```xml
<Styles>
    <!-- 自定义表格背景 -->
    <Style Selector="atom|DataGrid">
        <Setter Property="Background" Value="#FFFFFF" />
        <Setter Property="BorderBrush" Value="#E8E8E8" />
        <Setter Property="BorderThickness" Value="1" />
    </Style>
    
    <!-- 自定义列头 -->
    <Style Selector="atom|DataGridColumnHeader">
        <Setter Property="Background" Value="#FAFAFA" />
        <Setter Property="Foreground" Value="#333333" />
        <Setter Property="FontWeight" Value="600" />
        <Setter Property="Padding" Value="16,12" />
    </Style>
    
    <!-- 自定义行悬浮效果 -->
    <Style Selector="atom|DataGridRow:pointerover">
        <Setter Property="Background" Value="#E6F7FF" />
    </Style>
    
    <!-- 自定义选中行 -->
    <Style Selector="atom|DataGridRow:selected">
        <Setter Property="Background" Value="#BAE7FF" />
    </Style>
    
    <!-- 自定义单元格 -->
    <Style Selector="atom|DataGridCell">
        <Setter Property="Padding" Value="16,12" />
        <Setter Property="BorderBrush" Value="#F0F0F0" />
        <Setter Property="BorderThickness" Value="0,0,1,1" />
    </Style>
</Styles>
```

---

## 9. 暗色模式适配

DataGrid 的 Token 体系自动支持暗色模式。当全局主题切换为暗色时，所有 Token 值会自动重新计算。

如需手动覆盖暗色模式下的样式：

```xml
<Style Selector=":dark atom|DataGridColumnHeader">
    <Setter Property="Background" Value="#1F1F1F" />
    <Setter Property="Foreground" Value="#A0A0A0" />
</Style>
```

---

## 10. Gallery 示例参考

- `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml`
- `src/AtomUI.Desktop.Controls.DataGrid/Themes/DataGrid.axaml`（默认主题定义）