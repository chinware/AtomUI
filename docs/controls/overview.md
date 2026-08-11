# 控件文档总览

具体控件文档放在 `docs/controls/`，按运行平台分目录。

```text
docs/controls/
├── overview.md
├── desktop/
└── mobile/
```

## 平台分层

- [桌面 Control](desktop/overview.md)：覆盖 `AtomUI.Desktop.Controls`、DataGrid、ColorPicker 中的具体 Control。
- [移动端 Control](mobile/overview.md)：当前项目未包含移动端 Control 实现；正式入口维护预实现架构、七分类导航和 82 项能力兼容清单。

## 与 modules 的关系

- `modules/` 面向源码维护者，解释项目/包级架构。
- `controls/` 面向控件使用和控件级 API，解释单个控件的属性、事件、主题、示例和注意事项。

例如 DataGrid 的公共行为、实现和维护契约统一由
`docs/controls/desktop/data-display/data-grid/` 下的控件文档维护。

桌面端控件按控件目录维护 `overview.md`、`implementation.md`、`changelog.md`，存在专属 Token 的控件同时维护 `token.md`。
