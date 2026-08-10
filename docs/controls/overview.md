# 控件文档总览

具体控件文档放在 `docs/controls/`，按运行平台分目录。

```text
docs/controls/
├── overview.md
├── desktop/
└── mobile/
```

## 平台分层

- [desktop/overview.md](desktop/overview.md)：桌面端控件文档，覆盖 `AtomUI.Desktop.Controls`、DataGrid、ColorPicker 中的具体控件。
- [mobile/overview.md](mobile/overview.md)：移动端控件文档入口。当前项目未包含移动端控件实现，只保留结构。

## 与 modules 的关系

- `modules/` 面向源码维护者，解释项目/包级架构。
- `controls/` 面向控件使用和控件级 API，解释单个控件的属性、事件、主题、示例和注意事项。

例如 DataGrid：

- 包架构：`docs/modules/desktop-controls-datagrid/index.md`
- 控件文档：`docs/controls/desktop/data-display/data-grid/overview.md`

桌面端控件按控件目录维护 `overview.md`、`implementation.md`、`changelog.md`，存在专属 Token 的控件同时维护 `token.md`。
