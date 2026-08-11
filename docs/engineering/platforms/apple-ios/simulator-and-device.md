# Apple iOS Simulator 与真机

本文维护设备发现、安装、启动、Simulator 生命周期和截图证据。命令使用 `<device-id>`、`<bundle-id>`、`<app-path>` 等
占位符，不假定仓库当前存在特定 iOS Host。

## 真机前置条件

- 设备已在 Xcode 配对并信任开发机。
- 设备启用开发者模式。
- Device ID 已加入 development provisioning profile。
- 安装时使用与 Bundle ID、证书和 profile 匹配的签名产物。
- 远程启动时设备处于解锁状态。

## 设备发现、安装与启动

列出可用设备：

```bash
xcrun devicectl list devices
```

安装已签名 `.app`：

```bash
xcrun devicectl device install app \
  --device <device-id> \
  <app-path>
```

远程启动：

```bash
xcrun devicectl device process launch \
  --device <device-id> \
  --terminate-existing \
  <bundle-id>
```

安装成功与远程启动成功是两个证据。设备锁屏时，安装可能已完成，但 `devicectl ... process launch` 会因设备无法解锁而失败；
此时先解锁设备并重试启动，不要把锁屏错误误判为签名或安装失败。

## Simulator 发现与创建

列出已安装 runtime、设备类型和实例：

```bash
xcrun simctl list runtimes available
xcrun simctl list devicetypes
xcrun simctl list devices
```

需要固定尺寸/设备族证据时，可以显式创建 Simulator：

```bash
xcrun simctl create <simulator-name> <device-type-id> <runtime-id>
```

不得把某次 runtime ID 或设备型号写成仓库支持矩阵。选择依据应记录在日期化 evidence 中，例如与目标真机尺寸一致、覆盖
Safe Area 差异或复现特定布局。

## Boot、安装、启动与截图

```bash
xcrun simctl boot <simulator-id>
xcrun simctl bootstatus <simulator-id> -b
open -a Simulator
xcrun simctl install <simulator-id> <app-path>
xcrun simctl launch <simulator-id> <bundle-id>
xcrun simctl io <simulator-id> screenshot <screenshot-path>
```

截图文件应与构建配置、设备类型、runtime、方向、字号/主题、场景和日期一起记录。Simulator 截图适合验证 LaunchScreen 与
正式页面坐标、资源打包和稳定视觉状态；手机拍屏存在透视、曝光和刷新差异，只适合最终体验补充。

## 平台场景证据

真机/Simulator 验证至少区分：

| 场景 | Simulator | 真机 |
| --- | --- | --- |
| 安装与启动 | bundle 可安装并启动 | 签名产物可安装并启动 |
| Safe Area/旋转 | 多设备类型和方向 | 目标设备实际切口、方向和恢复 |
| IME | 组合输入、遮挡和焦点 | 真实键盘、输入法和交互延迟 |
| Touch/gesture | 基础 pointer 流 | 多指、边界、速度、取消和触感 |
| 生命周期 | 模拟前后台和重启 | 锁屏、前后台和 Host recreation |
| Accessibility | 基础 Automation 诊断 | VoiceOver 阅读、焦点和 action |

Simulator 通过不能自动提升真机状态；真机 Debug 也不能替代 Release/AOT 验证。

## 证据收尾

每次平台验证记录：

- Host commit、构建配置、target framework 和 RID。
- Device/Simulator identity、系统 runtime 和方向。
- Bundle ID、安装/启动结果和日志位置。
- 场景输入、期望结果、实际结果和截图。
- 是否为 Debug、Release、真机、Simulator、VoiceOver 或纯开发辅助。
