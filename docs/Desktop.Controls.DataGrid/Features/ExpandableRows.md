# DataGrid 可展开行

## 概述

DataGrid 支持行展开/折叠功能，通过 `DataGridDetailExpanderColumn` 和 `RowDetailsTemplate` 实现。展开后显示行详情内容。

---

## 基础用法

```xml
<atom:DataGrid RowDetailsVisibilityMode="Collapsed">
    <atom:DataGrid.Columns>
        <atom:DataGridDetailExpanderColumn />
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />
    </atom:DataGrid.Columns>
    <atom:DataGrid.RowDetailsTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
        </DataTemplate>
    </atom:DataGrid.RowDetailsTemplate>
</atom:DataGrid>
```

---

## 行详情可见性模式

| 模式 | 说明 |
|------|------|
| `Collapsed` | 默认折叠，点击展开按钮展开 |
| `Visible` | 默认展开所有行详情 |
| `VisibleWhenSelected` | 选中行时自动展开详情 |

---

## 展开列位置控制

`DataGridDetailExpanderColumn` 可放在 `Columns` 集合的任意位置：

```xml
<atom:DataGrid.Columns>
    <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
    <atom:DataGridDetailExpanderColumn />  <!-- 放在第二列 -->
    <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />
</atom:DataGrid.Columns>
```

---

## 行详情事件

| 事件 | 参数类型 | 说明 |
|------|---------|------|
| `LoadingRowDetails` | `DataGridRowDetailsEventArgs` | 行详情模板应用时触发 |
| `RowDetailsVisibilityChanged` | `DataGridRowDetailsEventArgs` | 行详情可见性变化时触发 |
| `UnloadingRowDetails` | `DataGridRowDetailsEventArgs` | 行详情可被回收时触发 |

---

## 编程式控制

```csharp
// 通过 DataGridRow 控制
row.IsDetailsVisible = true;   // 展开行详情
row.IsDetailsVisible = false;  // 折叠行详情
```

---

## 展开行视觉

| 元素 | Token | 说明 |
|------|-------|------|
| 展开按钮背景 | `ExpandIconBg` | 展开图标背景色 |
| 展开行背景 | `RowExpandedBg` | 展开行详情区域背景色 |

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Expandable Row / Order Specific Column