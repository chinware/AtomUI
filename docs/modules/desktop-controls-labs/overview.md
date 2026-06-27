# AtomUI.Desktop.Controls.Labs 模块概览

`AtomUI.Desktop.Controls.Labs` 是桌面实验性补充控件包，RootNamespace 为 `AtomUI.Desktop.Controls`。它依赖 `AtomUI.Desktop.Controls`，并以 Analyzer 方式引用 `AtomUI.Generator`。

## 职责

- 承载 Ant Design 标准之外、仍处于实验或评估阶段的桌面控件。
- 允许在独立按需包中验证控件 API、主题契约和交互模型。
- 为成熟控件迁移到 `AtomUI.Desktop.Controls.Extras` 或主桌面控件包提供过渡空间。

## 约束

- Labs 控件仍需遵循资源生命周期、AOT、主题 Token 和控件文档规则。
- 实验性边界必须在控件文档和 Gallery 示例中明确。
- 控件从 Labs 晋升到 Extras 前，应完成 API/主题契约稳定性评审和相邻测试补齐。
