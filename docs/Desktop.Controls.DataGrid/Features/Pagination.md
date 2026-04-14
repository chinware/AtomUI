# DataGrid 分页功能

## 概述

DataGrid 内置分页功能，通过 `PaginationVisibility` 和 `PageSize` 配置，无需额外控件。

---

## 基础分页

```xml
<atom:DataGrid PaginationVisibility="Bottom" PageSize="10" Height="400">
    <atom:DataGrid.Columns>
        <!-- ... -->
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 分页器位置

`PaginationVisibility` 为标志枚举，可组合使用：

```csharp
// 仅底部
dataGrid.PaginationVisibility = DataGridPaginationVisibility.Bottom;

// 顶部和底部
dataGrid.PaginationVisibility = DataGridPaginationVisibility.Top | DataGridPaginationVisibility.Bottom;
```

| 值 | 说明 |
|----|------|
| `None` | 不显示分页器 |
| `Top` | 在表格顶部显示 |
| `Bottom` | 在表格底部显示 |
| `Top | Bottom` | 顶部和底部同时显示 |

---

## 分页属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `PageSize` | `int` | `10` | 每页数据条数 |
| `PaginationVisibility` | `DataGridPaginationVisibility` | `None` | 分页器显示位置 |

---

## 分页事件

| 事件 | 参数类型 | 说明 |
|------|---------|------|
| `PageChanging` | `PageChangingEventArgs` | 分页索引变化前触发，可取消 |
| `PageChanged` | `PageChangedEventArgs` | 分页索引变化后触发 |

```csharp
dataGrid.PageChanging += (s, e) => {
    // 可取消分页
    e.Cancel = false;
};

dataGrid.PageChanged += (s, e) => {
    var newIndex = e.NewPageIndex;
};
```

---

## 分页器视觉

| 元素 | Token | 说明 |
|------|-------|------|
| 分页器外边距 | `PaginationMargin` | 大尺寸外边距 |
| 分页器外边距（小号） | `PaginationMarginSM` | 小尺寸外边距 |

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Basic Paging