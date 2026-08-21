# AtomUI Desktop Controls TestApp

该项目是 Desktop Controls 的长期人工回归应用，不是 Gallery 示例或自动化测试可执行文件。它直接引用仓库源码项目，
并加入 `AtomUI.slnx` 构建，以防人工验收场景随 API 演进而失效。

## PopupInDialog

`Scenarios/PopupInDialog` 保留 Issue #441 的 ComboBox-in-Overlay-Dialog 复现，并扩展为 Popup 家族人工矩阵：

- 选择器：ComboBox、Select、Cascader、TreeSelect。
- 自动建议：AutoComplete、AutoCompleteSearchEdit、AutoCompleteTextArea、Mentions。
- Picker：DatePicker、TimePicker、range variants、ColorPicker、GradientColorPicker。
- 原语与消费控件：Popup、Flyout、MenuFlyout、ToolTip、ContextMenu、DropdownButton、SplitButton、PopupConfirm。
- 菜单与特殊路径：Menu/MenuItem、NavMenu、Tour。
- 间接消费路径：AvatarGroup 折叠、Transfer 选择菜单、TabControl 溢出菜单、DataGrid 过滤菜单。

应用显示 public open/selection/action 状态、`TopLevel.GetTopLevel` 结果和 pointer route / popup host 快照，并将同一份带
时间戳的日志写入启动终端；不得加入 Avalonia 私有字段反射、自动打开 Dialog/Popup 或吞未处理异常的逻辑。

原语组中的 Direct Popup 是表面所有权的长期验收项：它不设置 `SurfaceBackground`，继承 Popup 原语的 `null` 默认值；
它的 Child 作为表面所有者，通过 `ColorBgElevated` 绘制内容背景。正确结果是 host frame 不产生额外 surface，同时
Child 的背景遮挡下层内容；Flyout、MenuFlyout、ToolTip、ContextMenu 及其他专用控件仍由自己的 Presenter /
`PopupFrame` 绘制表面。

运行：

```powershell
dotnet run --project tests/AtomUI.Desktop.Controls.TestApp/AtomUI.Desktop.Controls.TestApp.csproj --framework net10.0
```

Linux Wayland：

```bash
ATOMUI_WINDOWING_PLATFORM=wayland dotnet run --project tests/AtomUI.Desktop.Controls.TestApp/AtomUI.Desktop.Controls.TestApp.csproj --framework net10.0
```

Linux X11：

```bash
ATOMUI_WINDOWING_PLATFORM=x11 dotnet run --project tests/AtomUI.Desktop.Controls.TestApp/AtomUI.Desktop.Controls.TestApp.csproj --framework net10.0
```

验收每组时，先由用户打开 Dialog，再逐个点击 popup-bearing 控件。预期弹层可见、可命中、可选择或执行、可
light-dismiss，Dialog 保持打开且进程不崩溃。

进入“Popup / Flyout / ToolTip”组后，Direct Popup 的 Child 背景应完整遮挡下层文字，且不得再叠加第二层 host frame
surface；随后逐项打开 Flyout、MenuFlyout、ToolTip 和 ContextMenu，确认它们仍只绘制自己的背景、圆角、Padding 和
阴影，位置与交互保持一致。
Linux 命令仅提供验证入口，在 X11 与 Wayland 实机执行并记录结果前，平台状态继续标为未测试。

当前本专项实机证据：

| 平台 | 状态 | 已验证范围 |
| --- | --- | --- |
| Windows | 已测试 | `PopupInDialog` 真实窗口人工回归。 |
| macOS | 已测试 | `PopupInDialog` 真实窗口人工回归。 |
| Linux Wayland (GNOME) | 已测试 | Ubuntu 26.04、GNOME Shell 50.1 下的 `PopupInDialog` 真实窗口人工回归。 |
| Linux X11 | 未测试 | 尚无本专项实机证据，不能标记为通过。 |

本次 Wayland 人工回归分组覆盖 ComboBox、Popup/Flyout/MenuFlyout、Select/Cascader/TreeSelect、
AutoComplete/SearchEdit/TextArea/Mentions，以及 DatePicker/TimePicker 和两种 range picker。各组均验证打开、
内容命中、选择或执行、light-dismiss，且 Dialog 在交互后保持打开。运行日志确认 Popup 托管于所属 Window 的
`OverlayPopupHost`，light-dismiss 由 `LightDismissOverlayLayer` 接收；AutoComplete 关闭允许约 70–140ms 的异步延迟。

Headless 环境中，`Window.InputHitTest(point)` 对顶层 Popup 合成分支的结果与真实 Wayland 不一致，可能命中 Window
模板内的匿名透明 `Panel`。相关回归测试因此向已确认布局有效的目标控件直接注入 routed pointer 事件，向
`LightDismissOverlayLayer` 注入外部点击；对于依赖 Headless 未维护的全局 `IsPointerOver` 状态的 Button，使用
automation invoke。该策略用于验证控件事件、Popup 层级和关闭行为，不把 Headless 的顶层坐标命中结果当作真实平台结论。
