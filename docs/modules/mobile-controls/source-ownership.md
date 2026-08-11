# AtomUI.Mobile.Controls 源码职责

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

本文按稳定职责域定义目标包的 source ownership，不虚构尚不存在的项目文件树、namespace 清单或实现类。Foundation 落地后，
本页必须以真实 Public API、Theme、测试和生成产物替换预实现描述。

## 职责域

| 职责域 | 输入 | 输出 | Public / internal 边界 | 替换预实现描述所需证据 |
| --- | --- | --- | --- | --- |
| Public Controls | `AtomUI.Controls` 抽象、移动交互设计、平台无关状态模型 | Mobile Control API、事件、Command、AutomationPeer 契约 | 应用语义为 public；session、policy 和 adapter 细节为 internal | 可审查 Public API 源码、默认值、Contract tests 和 Gallery API 表 |
| Runtime scopes | `TopLevel`、平台 snapshot、导航与 gesture policy | 每 `TopLevel` 的 Viewport、Overlay 和 Gesture 服务 | 应用只消费 Control/受约束服务 API；scope owner 和 coordinator 为 internal | attach/detach 源码、Headless 生命周期测试和资源释放证据 |
| Themes and Tokens | Global Token、Theme Algorithm、`ThemeConfigProvider`、Control descriptor | Mobile ControlTheme、Own Token、必要的 Mobile Alias Token | Token/Theme 定制契约为 public；资源装配与 snapshot 消费为 internal | AXAML Theme、生成 descriptor、Light/Dark visual baseline 和 Token 表 |
| Localization resources | Catalog、Snapshot、fallback、Generator | Mobile 文案 key、默认语言资源和静态注册 | 面向应用的本地化行为稳定；catalog 装配为 internal | 真实 catalog、生成注册、fallback tests 和 Gallery locale 场景 |
| Platform capability consumption | 统一 adapter snapshot、Host 生命周期、可选 Native 能力 | Safe Area、IME、back、haptic、touch profile 等平台无关状态 | Control 不暴露 OS-specific 类型；adapter 和 profile 为 internal | iOS/Android/Headless adapter 源码、平台测试和缺失能力测试 |
| Registration and generated descriptors | `IAtomUIBuilder`、Generator 输入、Theme/Localization 资产 | `UseMobileControls(...)` 与静态 descriptor/manifest 注册 | 包级启动入口为 public；生成池、adapter validation 和 scope factory 为 internal | 真实生成输出、启动失败测试、trim/AOT publish 和安装启动证据 |

## Public Controls

同名 Desktop/Mobile Control 只有在 API、状态机、Form 和无障碍语义真正一致时才复用 `AtomUI.Controls` 抽象。数据和
协调逻辑一致但视觉/交互不同的能力只共享 model/coordinator；移动 viewport、Overlay、手势或输入语义占主导时设计独立
Mobile API。Desktop ControlTheme、Desktop Token 和 Desktop 私有类型不能成为 Mobile 的实现依赖。

具体 Control 目录只有满足 [Mobile 单 Control 文档门禁](../../engineering/contributing/mobile-documentation-guidelines.md#单-control-文档创建门禁)
后才创建；本模块文档不提前枚举 82 个源码目录。

## Runtime Scopes

每个活动 `TopLevel` 恰好拥有一个 Mobile Runtime scope。scope 装配 `MobileViewportContext`、
`MobileOverlayManager`、`MobileGestureCoordinator` 和 Host adapter，并在 detach 时统一取消 pending work、释放订阅、
pointer capture、animation owner 与 session。跨模块状态机以 [Mobile Runtime 架构](../../architecture/systems/mobile/overview.md)
为准，包内实现不得另建不兼容副本。

## Themes、Tokens 与 Localization

Mobile 复用现有 Global Token、Theme Algorithm、`ThemeConfigProvider`、Catalog、Snapshot、fallback 和 Generator。
移动特有的运行时状态不进入 Token；平台 adapter 不提供产品文案；内置资产通过生成式 descriptor 或显式静态注册进入
启动链路，不使用反射扫描或字符串成员访问。

## 平台与 Native 边界

平台目录只实现统一 internal capability contract，不新增平台专属 Public Control API。能由 Avalonia 公共 API 表达的能力
直接通过 adapter 投射；只有 P/Invoke、native handle、原生结构体、hook 或协议对象确实必要时才调用 `AtomUI.Native`。
Native 管理底层资源和错误边界，Mobile 管理 owner、viewport、gesture、Overlay、Theme 和产品默认策略。

## 同步门禁

Foundation 源码进入仓库后，本页至少同步：

1. 真实项目与 namespace 定位，但不罗列无维护价值的完整文件树。
2. Public/internal 类型 owner、注册入口和生成资产来源。
3. Runtime scope acquire/release 对和 adapter 生命周期。
4. Theme、Localization、测试、Gallery 和 AOT 证据链接。
5. iOS、Android、Release 与 Publication 的独立状态。
