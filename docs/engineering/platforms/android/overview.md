# Android 工程环境

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

本文是 AtomUI/Avalonia Android Host 的工程文档入口。当前仓库没有 Android Mobile Gallery Host、已执行构建命令、Emulator/
真机或 Release 证据，因此本目录只定义工具链、签名、Host/adapter、验证和排障责任，不声明版本组合或平台可用性。

## 范围

- Android SDK、JDK、.NET Android workload 和本地/CI ownership。
- Application identity、keystore/signing identity 和 secret 边界。
- Emulator/physical device 的发现、部署、启动、日志、截图和场景证据。
- Debug/Release、target framework、ABI/RID、package、trim/AOT 和启动验证。
- Activity recreation、system back、system bars、IME、multi-window 和 TalkBack。

Mobile Control API、Viewport/Overlay/Gesture 状态机和 Gallery content 分别由
[Mobile 系统架构](../../../architecture/systems/mobile/overview.md)、
[Mobile Controls 模块](../../../modules/mobile-controls/overview.md)和
[Mobile Gallery](../../../gallery/platforms/mobile-gallery.md)拥有。

## Host 与 Adapter 边界

Android Host 拥有应用生命周期、启动、资源/manifest、部署和 Android capability adapter 注册。Adapter 把 system bars、
input pane、system back、haptic、font scale、Reduce Motion/accessibility environment、foreground/background 与 Activity recreation
投射到统一 Mobile Runtime contract。Control 不读取 Android OS 名称或平台 SDK 类型。

当前没有 Host 或 adapter 源码；任何目标类型名、项目路径和命令必须等真实实现出现后再补写。

## 阅读顺序

| 文档 | 职责 |
| --- | --- |
| [工具链与签名](toolchain-and-signing.md) | SDK/JDK/workload、keystore、secret 和版本证据门禁 |
| [Emulator 与真机](emulator-and-device.md) | AVD/device、部署、日志、截图、输入、返回、旋转和 TalkBack 证据 |
| [构建与验证](build-and-validation.md) | Debug/Release、TFM、ABI/RID、package、trim/AOT、Activity recreation 和发布门禁 |
| [排障](troubleshooting.md) | 按 owner 和证据组织的诊断框架，不提供未经执行的修复配方 |

## 当前证据矩阵

| 维度 | 当前状态 | 提升要求 |
| --- | --- | --- |
| Design | 能力已纳入总体设计 | Host、adapter 和构建边界评审完成 |
| Source | 未实现 | Android Host、adapter、manifest/resources 和注册入口存在 |
| Compile | 未验证 | 真实 target framework 与 ABI/RID 编译通过 |
| Emulator | 未验证 | 安装、启动、system back、IME、旋转、system bars 和生命周期场景有记录 |
| Physical device | 未验证 | 真机触摸、IME、前后台、multi-window/设备差异和日志证据存在 |
| TalkBack | 未验证 | 真机阅读顺序、焦点、state/action 和 modal 语义证据存在 |
| Activity recreation | 未验证 | 重建后 navigation/session/viewport 不重复提交或泄漏 |
| Release | 未验证 | trim/AOT 产物构建、安装并启动，资产可发现 |
| Publication | 未发布 | 可追溯包或应用版本与发布日期存在 |

Compile、Emulator、真机、TalkBack、Activity recreation、Release 和 Publication 互不替代。iOS 证据也不能提升 Android 状态。

## 非目标

- 不发布未经验证的标准命令、推荐 SDK/JDK/workload 版本或机器路径。
- 不把 Android compile 描述为 Emulator/真机支持。
- 不把 Headless adapter 描述为 system back、IME、TalkBack 或 Activity 生命周期验证。
- 不定义 Play Store、企业分发或产品 entitlement/permission 策略。
