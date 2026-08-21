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

应用只显示 public open/selection/action 状态与 `TopLevel.GetTopLevel` 结果；不得加入 Avalonia 私有字段反射、临时日志、
自动打开 Dialog/Popup 或吞未处理异常的逻辑。

运行：

```powershell
dotnet run --project tests/AtomUI.Desktop.Controls.TestApp/AtomUI.Desktop.Controls.TestApp.csproj --framework net10.0
```

验收每组时，先由用户打开 Dialog，再逐个点击 popup-bearing 控件。预期弹层可见、可命中、可选择或执行、可
light-dismiss，Dialog 保持打开且进程不崩溃。

当前本专项实机证据：

| 平台 | 状态 |
| --- | --- |
| Windows | 已测试 |
| macOS | 已测试 |
| Linux X11 / Wayland | 未测试 |
