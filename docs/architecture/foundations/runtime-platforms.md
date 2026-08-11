# 运行平台策略

AtomUI 当前面向 Windows、macOS、Linux 桌面运行，同时支持 Browser Gallery 作为展示和验证路径。移动端已有批准的
iOS/Android 目标架构，但当前解决方案没有 `AtomUI.Mobile.Controls` 项目或 Mobile Gallery Host。

## 桌面端

桌面端通过 Avalonia Desktop/Native 平台运行，主包是 `AtomUI.Desktop.Controls`。桌面端支持：

- Native Window 能力。
- Popup、Overlay、Window、WindowTitleBar 等依赖桌面窗口系统的控件和服务。
- `ToolTipService` 等在 `UseDesktopControls()` 初始化回调中注册的桌面服务。
- Native 层提供的窗口穿透、macOS 标题栏按钮控制、Linux X11 相关能力。

## 浏览器端

Browser Gallery 使用 `net10.0-browser` 和 `Avalonia.Browser`。浏览器环境下 `RuntimePlatform.Features.SupportsNativeWindow` 为 false，注册链路会切换到浏览器 Provider：

- `BrowserCommonControlThemesProvider`
- `BrowserDesktopControlThemesProvider`

`UseDesktopControls()` 也会使用浏览器安全的 Token 类型列表，避免注册需要 Native Window 的控件 Token。浏览器端主要用于控件展示和兼容验证，不应假定所有桌面窗口能力都可用。

## Native 层

`AtomUI.Native` 采用 partial class 加平台目录组织：

- `Windows/`：Win32 P/Invoke。
- `MacOS/`：Objective-C Runtime P/Invoke。
- `Linux/`：XCB/X11 P/Invoke 与 Wayland input-region 协议 helper。

上层通过 `WindowExtensions` 调用统一入口，内部按运行时平台路由。Native 能力属于基础设施，不直接面向普通控件使用者暴露。

## 移动端

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

Mobile 采用与 Desktop 平行的产品包和 iOS/Android Host：

- `AtomUI.Mobile.Controls` 消费 `AtomUI.Controls` 的公共抽象，并拥有 Mobile Control、Runtime scope、Theme 和平台默认策略。
- 每个 Host 提供统一 capability adapter，向 Runtime 投射 Safe Area、system bars、input pane、返回/导航手势、haptic、
  touch profile、前后台、Reduce Motion、字体缩放和无障碍环境。
- iOS 是首个实现与体验验收平台；Android adapter、公共 API 和验证责任从 Foundation 开始存在。
- Control 只消费 capability/profile，不读取 OS 名称，也不在控件内散布条件编译。
- `AtomUI.Native` 只在 Avalonia 公共 API 无法表达所需 OS 能力时提供底层调用、handle、hook 和确定释放。

跨模块契约见 [Mobile 系统架构](../systems/mobile/overview.md)，目标包边界见
[Mobile Controls 模块](../../modules/mobile-controls/overview.md)。当前 iOS 工程环境记录见
[Apple iOS 开发环境](../../engineering/platforms/apple-ios-development-environment.md)；Android 工程文档在获得真实工具链证据前
只定义验证边界，不声明未经验证的命令或版本。

`AtomUI.Native` 已对目标程序集名称开放 `InternalsVisibleTo`，但当前没有 Mobile 实现消费者；该声明不能作为源码、平台验证
或发布证据。
