# AtomUI.Desktop.Controls.Extras 模块概览

`AtomUI.Desktop.Controls.Extras` 是桌面稳定补充控件包，RootNamespace 为 `AtomUI.Desktop.Controls`。它依赖 `AtomUI.Desktop.Controls`，并以 Analyzer 方式引用 `AtomUI.Generator`。

## 职责

- 承载 Ant Design 标准之外、已经准备作为稳定 API 发布的桌面控件。
- 复用桌面主控件包的主题、Token、语言和控件基础设施。
- 为补充控件保留独立按需引用边界，避免扩大主控件包依赖面。

## 约束

- 新增控件必须遵循桌面控件研发标准、AOT 规则和控件文档规则。
- 控件进入 Extras 前应具备稳定的公共 API、主题契约、Gallery 示例和验证覆盖。
- 仍处于探索阶段的控件不应进入 Extras，待 API、主题契约和验证边界稳定后再纳入。
