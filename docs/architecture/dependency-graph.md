# 项目依赖关系

本文档记录 `AtomUI.slnx` 中项目的主要引用关系。它描述源码维护视角下的依赖方向，不等同于 NuGet 包的最终依赖闭包。

## 解决方案项目

`AtomUI.slnx` 当前包含核心库、控件库、图标字体、Gallery 宿主：

- `src/AtomUI.Native`
- `src/AtomUI.Core`
- `src/AtomUI.Controls.Shared`
- `src/AtomUI.Controls`
- `src/AtomUI.Desktop.Controls`
- `src/AtomUI.Desktop.Controls.DataGrid`
- `src/AtomUI.Desktop.Controls.ColorPicker`
- `src/AtomUI.Generator`
- `src/AtomUI.Icons.Shared`
- `src/AtomUI.Icons.AntDesign`
- `src/AtomUI.Icons.AntDesign.Generator`
- `src/AtomUI.Fonts.AlibabaSans`
- `src/AtomUI.Fonts.AlibabaPuHuiTi`
- `controlgallery/AtomUIGallery`
- `controlgallery/AtomUIGallery.Desktop`
- `controlgallery/AtomUIGallery.Browser`

## 主要项目引用

| 项目 | 直接引用 | 说明 |
|---|---|---|
| `AtomUI.Native` | `Avalonia` | 原生窗口 API 基础层 |
| `AtomUI.Core` | `AtomUI.Native`, `AtomUI.Generator` | 主题、Token、语言、动画基础设施 |
| `AtomUI.Controls.Shared` | `AtomUI.Core`, `AtomUI.Generator` | 控件共享契约和协调器 |
| `AtomUI.Controls` | `AtomUI.Core`, `AtomUI.Controls.Shared`, `AtomUI.Fonts.AlibabaSans`, `AtomUI.Icons.AntDesign`, `AtomUI.Generator` | 公共控件、Primitives、公共主题 |
| `AtomUI.Desktop.Controls` | `AtomUI.Controls`, `AtomUI.Generator` | 桌面主控件包 |
| `AtomUI.Desktop.Controls.DataGrid` | `AtomUI.Desktop.Controls`, `AtomUI.Generator` | 独立 DataGrid 包 |
| `AtomUI.Desktop.Controls.ColorPicker` | `AtomUI.Desktop.Controls`, `AtomUI.Generator`, `Avalonia.Controls.ColorPicker` | 独立 ColorPicker 包 |
| `AtomUI.Icons.AntDesign` | `AtomUI.Core` | Ant Design 图标注册与生成图标 |
| `AtomUI.Icons.AntDesign.Generator` | `AtomUI.Icons.Shared` | 从 SVG 生成 Ant Design 图标源码 |
| `AtomUI.Fonts.*` | `AtomUI.Core` | 字体集合注册 |
| `AtomUIGallery` | `AtomUI.Desktop.Controls`, `AtomUI.Desktop.Controls.DataGrid`, `AtomUI.Desktop.Controls.ColorPicker`, `AtomUI.Generator` | 控件示例主体 |
| `AtomUIGallery.Desktop` | `AtomUIGallery` | 桌面宿主 |
| `AtomUIGallery.Browser` | `AtomUIGallery`, `AtomUI.Fonts.AlibabaPuHuiTi` | 浏览器宿主 |

## 内部可见性

多个包通过 `InternalsVisibleTo` 共享内部实现，维护时需要注意这些不是公开 API：

- `AtomUI.Core` 对 `AtomUI.Controls`、`AtomUI.Controls.Shared`、`AtomUI.Desktop.Controls`、DataGrid、ColorPicker 开放内部成员。
- `AtomUI.Controls` 对 `AtomUI.Desktop.Controls`、DataGrid、ColorPicker 开放内部成员。
- `AtomUI.Desktop.Controls` 对 DataGrid、ColorPicker、性能工具开放内部成员。
- `AtomUI.Native` 对 `AtomUI.Core`、`AtomUI.Desktop.Controls`、未来 `AtomUI.Mobile.Controls` 开放内部成员。

这些关系说明 DataGrid 和 ColorPicker 虽然是独立包，但它们不是完全隔离的第三方扩展，而是桌面控件体系的同源扩展。

