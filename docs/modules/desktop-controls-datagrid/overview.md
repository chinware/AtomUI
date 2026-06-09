# AtomUI.Desktop.Controls.DataGrid 模块概览

`AtomUI.Desktop.Controls.DataGrid` 是桌面 DataGrid 独立包，RootNamespace 为 `AtomUI.Desktop.Controls`。它依赖 `AtomUI.Desktop.Controls`，并以 Analyzer 方式引用 `AtomUI.Generator`。

## 职责

- 提供 DataGrid 主控件、行、列、单元格、表头、选择列、操作列、拖拽重排等能力。
- 提供 DataGrid 自己的数据视图、排序、分组、过滤描述。
- 提供 DataGrid Token、主题和本地化资源。
- 通过 `UseDesktopDataGrid()` 注册到 AtomUI 主题系统。

## 关键目录

| 目录 | 说明 |
|---|---|
| `Column/` | 列基类、文本列、模板列、选择列、操作列、表头、排序和过滤 |
| `Row/` | 行、行头、行详情、分组行 |
| `Cell/` | 单元格和单元格 Presenter |
| `Data/` | DataGrid 专用 CollectionView、排序、分组、过滤 |
| `Themes/` | DataGrid 控件、行、列、单元格主题 |
| `Localization/` | DataGrid 多语言资源 |
| `Utils/` | 冻结列网格、键盘、视觉树和校验辅助 |

## 注册入口

`ThemeManagerBuilderExtensions.UseDesktopDataGrid()` 会注册：

- `ControlTokenTypePool.GetTokenTypes()`
- `AtomUIDataGridThemesProvider`
- `LanguageProviderPool.GetLanguageProviders()`

具体 DataGrid 使用/API 文档应放在 `docs/controls/desktop/data-display/datagrid.md`。

