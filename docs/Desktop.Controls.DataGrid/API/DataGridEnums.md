# DataGrid 枚举类型参考

**命名空间**: `AtomUI.Desktop.Controls`

---

## DataGridSelectionMode

| 值 | 说明 |
|----|------|
| `Single` | 单行选择，点击即选中，不可多选 |
| `Extended` | 扩展选择，支持 Ctrl+点击多选、Shift+点击范围选 |

---

## DataGridHeadersVisibility

| 值 | 说明 |
|----|------|
| `None` | 不显示表头 |
| `Row` | 仅显示行头 |
| `Column` | 仅显示列头 |
| `All` | 显示行头和列头 |

---

## DataGridGridLinesVisibility

| 值 | 说明 |
|----|------|
| `None` | 不显示网格线 |
| `Horizontal` | 仅显示水平网格线 |
| `Vertical` | 仅显示垂直网格线 |
| `All` | 显示所有网格线 |

---

## DataGridRowDetailsVisibilityMode

| 值 | 说明 |
|----|------|
| `Collapsed` | 行详情默认折叠 |
| `Visible` | 行详情默认可见 |
| `VisibleWhenSelected` | 选中时显示行详情 |

---

## DataGridSortDirections（标志枚举）

| 值 | 说明 |
|----|------|
| `None` | 不支持排序 |
| `Ascending` | 升序 |
| `Descending` | 降序 |
| `All` | 升序和降序（`Ascending | Descending`） |

---

## DataGridFilterMode

| 值 | 说明 |
|----|------|
| `Equals` | 等于过滤 |
| `Contains` | 包含过滤 |
| `StartsWith` | 开头匹配过滤 |
| `EndsWith` | 结尾匹配过滤 |
| `GreaterThan` | 大于过滤 |
| `LessThan` | 小于过滤 |
| `GreaterThanOrEqual` | 大于等于过滤 |
| `LessThanOrEqual` | 小于等于过滤 |

---

## DataGridPaginationVisibility（标志枚举）

| 值 | 说明 |
|----|------|
| `None` | 不显示分页器 |
| `Top` | 在表格顶部显示分页器 |
| `Bottom` | 在表格底部显示分页器 |

> 可组合使用，如 `Top | Bottom` 同时在顶部和底部显示分页器。

---

## DataGridLength

列宽类型，支持以下模式：

| 模式 | 说明 | 示例 |
|------|------|------|
| `Auto` | 自动宽度，根据内容调整 | `Width="Auto"` |
| `SizeToCells` | 根据单元格内容调整宽度 | `Width="SizeToCells"` |
| `SizeToHeader` | 根据列头内容调整宽度 | `Width="SizeToHeader"` |
| `Pixel` | 固定像素宽度 | `Width="100"` |
| `Star` | 按比例分配剩余空间 | `Width="2*"` |

---

## SizeType

| 值 | 说明 |
|----|------|
| `Large` | 大尺寸（默认），单元格内间距为 `SharedToken.Padding` |
| `Middle` | 中等尺寸，单元格内间距为 `SharedToken.PaddingSM` |
| `Small` | 小尺寸，单元格内间距为 `SharedToken.PaddingXS` |

---

## DataGridEditAction

| 值 | 说明 |
|----|------|
| `Begin` | 开始编辑 |
| `Commit` | 提交编辑 |
| `Cancel` | 取消编辑 |

---

## DataGridCommitEditAction

| 值 | 说明 |
|----|------|
| `Normal` | 正常提交（失去焦点） |
| `Tab` | Tab 键提交 |
| `Enter` | Enter 键提交 |