# AtomUI.Native 模块概览

`AtomUI.Native` 是 AtomUI 的原生平台交互层，RootNamespace 为 `AtomUI.Native`。它通过 P/Invoke 封装 Windows、macOS、Linux 下的窗口级能力。

## 职责

- 提供跨平台窗口鼠标事件穿透能力。
- 提供 macOS 标题栏按钮位置、尺寸、关闭按钮状态等窗口能力。
- 提供 Linux/X11 输入区域、窗口几何等底层辅助能力。
- 为 `AtomUI.Core` 和 `AtomUI.Desktop.Controls` 提供内部基础设施。

## 目录结构

```text
src/AtomUI.Native/
├── WindowExtensions.cs
├── Windows/
├── MacOS/
└── Linux/
```

每个平台目录中通常拆分为：

- `WindowUtils.Interop.cs`：P/Invoke 声明、结构体、枚举。
- `WindowUtils.<Platform>.cs`：平台业务逻辑。

## 模块文档

Native 模块专题文档已合并到本目录：

- [architecture.md](architecture.md)
- [windows.md](windows.md)
- [macos.md](macos.md)
- [linux.md](linux.md)
- [atomui-window-cross-platform-guide.md](atomui-window-cross-platform-guide.md)
- [window-drawn-decorations.md](window-drawn-decorations.md)
- [windows-disable-csd-shadow-scheme.md](windows-disable-csd-shadow-scheme.md)
