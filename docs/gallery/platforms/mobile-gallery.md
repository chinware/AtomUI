# AtomUI Mobile Gallery

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

Mobile Gallery 是 Mobile Controls 的示例、结构化文档输入和平台体验验证入口。Foundation 从独立的最小 Mobile Shell 起步，
不依赖当前 Desktop/Browser-bound `AtomUI.Toolkits.GalleryBase`，也不提前冻结项目名、namespace、路由或源码目录。

## 三层 Ownership

| 层级 | 拥有 | 不拥有 |
| --- | --- | --- |
| Mobile Gallery content | Mobile ShowCase、结构化 API table、Token table、能力兼容状态、导航内容和稳定示例 | iOS/Android 生命周期、签名、部署和平台 SDK 调用 |
| iOS Host | iOS 生命周期、应用启动、签名/部署、资源入口和 iOS capability adapter 注册 | Mobile Control API、Token、跨平台 ShowCase 内容和兼容结论 |
| Android Host | Android 生命周期、应用启动/部署、资源入口和 Android capability adapter 注册 | Mobile Control API、Token、跨平台 ShowCase 内容和兼容结论 |

Mobile Gallery content 中展示的兼容状态必须与
[Mobile Control 分类清单](../../controls/mobile/overview.md)同步；正式文档拥有文档状态事实，Gallery 拥有产品内可扫描的
结构化投射，不能维护相互矛盾的第二套结论。

## Foundation 最小 Shell

最小 Shell 只建立证明 Mobile 基础设施协作所需的骨架：

- Mobile 内容导航和一个稳定垂直切片的 ShowCase 承载位置。
- 结构化 API、Token 和兼容状态的展示位置。
- Theme、Localization 和字号变化入口。
- 每 `TopLevel` Mobile Runtime scope 与 Host adapter 的连接。
- iOS/Android Host 对生命周期、返回、viewport 和输入环境的可诊断状态。

Foundation 不复制 Desktop Window、标题栏、Browser Shell、Desktop NavMenu 或当前 GalleryBase 的完整功能。只有真实 Mobile
ShowCase 出现重复后，才评估独立、无 Desktop 依赖的共享 Gallery contracts。

## 验证角色

Desktop 或 Headless preview 可以辅助开发以下内容：

- Public API table、Token table 和静态文案结构。
- 纯逻辑状态机、Theme/Localization 接入和基本布局诊断。
- 无平台输入依赖的 ShowCase 导航与数据准备。

这些 preview 不能作为最终 Mobile 验证。下列能力必须保留平台 Host 证据：

| 场景 | iOS 证据 | Android 证据 |
| --- | --- | --- |
| Safe Area 与 system bars | Simulator 与真机 viewport/旋转 | Emulator 与真机 viewport/multi-window |
| Back/navigation | iOS 返回手势与导航事务 | Android system back 与导航事务 |
| IME/input pane | 组合输入、键盘遮挡和焦点 | 组合输入、键盘遮挡和焦点 |
| Touch/gesture | 真机触摸、nested scroll、cancel/snap | 真机触摸、nested scroll、cancel/snap |
| Foreground/background | Host suspend/resume/recreation | Activity pause/resume/recreation |
| Accessibility | VoiceOver 真机 | TalkBack 真机 |

Simulator/Emulator、真机、Release 和 Publication 状态分别记录；一个 Host 的证据不能提升另一个 Host 的状态。

## ShowCase 同步门禁

单个 Mobile Control 进入 Gallery 时至少同步：

1. 稳定 ShowCase 场景和可访问状态。
2. 由真实 Public API 生成或维护的结构化 API table。
3. Own Token、Semantic Part 和可定制职责的 Token table；无 Own Token 时明确记录。
4. iOS、Android、Release 和 Publication 的独立兼容状态。
5. 对应 Control 文档、Contract/Headless tests 和平台证据链接。

Gallery 示例不能替代 Public API、Theme 或测试事实。源码、结构化表和文档不一致时，先确定真实契约再同步修正。

## 当前状态

当前没有 Mobile Gallery content、iOS Host 或 Android Host 源码，也没有 Mobile ShowCase、平台体验或 Release 证据。本页只定义
批准的 ownership 和验证边界。
