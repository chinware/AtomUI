# DataGrid 排序功能

## 概述

DataGrid 支持单列排序和多列排序，点击列头触发排序。排序状态通过列头图标和背景色直观反馈。

---

## 排序控制属性

| 属性 | 位置 | 默认值 | 说明 |
|------|------|--------|------|
| `CanUserSortColumns` | DataGrid | `true` | 全局排序开关 |
| `CanUserSort` | DataGridColumn | `true` | 单列排序开关 |
| `SupportedSortDirections` | DataGridColumn | `All` | 支持的排序方向 |
| `ShowSorterTooltip` | DataGridColumn | `true` | 排序提示 |

---

## 单列排序

点击列头在三种状态间循环：**无排序 → 升序 → 降序 → 无排序**。

```xml
<atom:DataGrid CanUserSortColumns="True">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" CanUserSort="True" />
        <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" CanUserSort="True" />
        <atom:DataGridTextColumn Header="Address" Binding="{Binding Address}" CanUserSort="False" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 多列排序

按住 Shift 键点击列头可添加额外排序列，实现多列组合排序。

---

## 排序方向限制

通过 `SupportedSortDirections` 限制列支持的排序方向：

```xml
<!-- 仅支持升序 -->
<atom:DataGridTextColumn Header="Name" Binding="{Binding Name}"
    SupportedSortDirections="Ascending" />

<!-- 仅支持降序 -->
<atom:DataGridTextColumn Header="Age" Binding="{Binding Age}"
    SupportedSortDirections="Descending" />
```

---

## 自定义排序

监听 `Sorting` 事件实现自定义排序逻辑：

```csharp
dataGrid.Sorting += (s, e) => {
    var column = e.Column;
    // 自定义排序逻辑
    e.Handled = true; // 标记已处理，阻止默认排序
};
```

---

## 排序视觉反馈

| 状态 | 列头背景 | 图标 |
|------|---------|------|
| 无排序 | `HeaderBg` | 无 |
| 升序 | `HeaderSortActiveBg` | ↑ 图标 |
| 降序 | `HeaderSortActiveBg` | ↓ 图标 |
| 悬浮 | `HeaderSortHoverBg` | 淡色图标 |

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Filter And Sorter / Multi Sorter