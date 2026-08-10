# AtomUI.Desktop.Controls 模块概览

`AtomUI.Desktop.Controls` 是 AtomUI 桌面主控件包，RootNamespace 为 `AtomUI.Desktop.Controls`。它依赖 `AtomUI.Controls` 和 `AtomUI.Generator`。

## 职责

- 提供大多数桌面端 Ant Design 控件。
- 组织桌面控件 Token、主题、本地化 Catalog 和内置 Translation Bundle。
- 提供 Popup、Overlay、Flyout、Window、WindowTitleBar、Dialog、Message、Notification 等桌面系统能力。
- 在浏览器环境下提供可用的主题 Provider 和安全 Token 子集。
- 向 DataGrid、ColorPicker、Extras、性能工具开放内部成员。

## 关键入口

- `ThemeManagerBuilderExtensions.UseDesktopControls()`
- `DesktopControlThemesProvider`
- `BrowserDesktopControlThemesProvider`
- `MediaBreakPointThemeBootstrapper`

## 推荐阅读

- [architecture.md](architecture.md)
- [theme-registration.md](theme-registration.md)
- [browser-compatibility.md](browser-compatibility.md)
- [../core/semantic-part-system.md](../core/semantic-part-system.md)
- [../../controls/desktop/overview.md](../../controls/desktop/overview.md)

DataGrid、ColorPicker 和 Extras 虽然属于桌面控件体系，但它们是独立包，内部架构分别放在：

- [../desktop-controls-datagrid/overview.md](../desktop-controls-datagrid/overview.md)
- [../desktop-controls-colorpicker/overview.md](../desktop-controls-colorpicker/overview.md)
- [../desktop-controls-extras/overview.md](../desktop-controls-extras/overview.md)
