# Mobile 主题、本地化与 API 映射

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

Mobile 不重建 Ant Design 或 AtomUI 的全局主题系统。它复用现有 Theme、Token、Localization 和 Avalonia API 模型，只在
移动控件具有独立、稳定视觉职责时新增 Mobile Own Token 或受约束的 Mobile Alias Token。

## Theme 与 Token

| 能力 | Owner |
| --- | --- |
| Seed/Map/Alias Global Token | 现有 AtomUI Theme 系统 |
| Theme Algorithm | 现有 AtomUI Theme Algorithm |
| 应用级和局部主题作用域 | `ThemeConfigProvider` 与资源作用域 |
| Mobile 共享视觉语义 | Mobile Alias Token，满足创建门槛后新增 |
| 单 Control 视觉差异 | Mobile Control Own Token |
| ControlTheme、pseudo-class、Semantic Part | 对应 Mobile Control 契约 |
| Safe Area、IME、方向、选择、打开和手势进度 | Mobile Runtime 状态，不是 Token |

Mobile Alias Token 只有同时满足以下条件才新增：

1. 至少三个 Mobile Control 真实共享同一稳定语义。
2. 应用级覆盖该语义具有明确价值。
3. 不能由现有 Global Token 组合表达。
4. 名称表达 AtomUI 稳定语义，不复制上游 CSS variable 名称。

平台最小触控目标来自 platform profile。Theme 可以消费已经解析的尺寸资源，但不能读取 OS 名称决定 Token。

## `ConfigProvider` 职责映射

Mobile 不新增同名 `ConfigProvider` Control：

| 上游职责 | AtomUI owner |
| --- | --- |
| locale | AtomUI Localization、Catalog、Snapshot 和 language pack |
| 全局主题变量 | Global Token、Theme Algorithm、`ThemeConfigProvider` |
| 局部主题作用域 | `ThemeConfigProvider` 与资源作用域 |
| 组件默认配置 | 目标 `UseMobileControls` options、Control 属性或主题资源 |
| icon/empty 内容替换 | Icon package、`DataTemplate`、ControlTheme 和语义资源 |
| 命令式默认配置 | 应用启动注册，不使用可变全局静态字典 |

目标 `UseMobileControls` 名称属于已批准注册契约，当前没有对应源码实现。

## Localization

- Mobile 包拥有自己的产品文案 catalog 和默认语言资源。
- 日期、日历、Picker、Form、上传和错误状态复用现有 snapshot、fallback 和静态生成链路。
- iOS 与 Android 使用相同文本契约；adapter 不提供产品文案。
- Control 不缓存会随语言 snapshot 失效的字符串。
- 不使用运行时程序集扫描发现语言资源。
- 平台系统返回、键盘和无障碍能力可以影响行为，但不改变产品文案 key。

## Avalonia API 映射

| React/Web 形态 | AtomUI Mobile 形态 |
| --- | --- |
| `children` | `Content`、`ItemsSource`、`DataTemplate` |
| render prop | 强类型 template、selector 或 factory |
| `visible` + callback | `IsOpen` 与 opening/closing/closed lifecycle |
| ref imperative method | 明确的 Control 方法或服务 API |
| Promise show API | `Task<TResult>` 与 `CancellationToken` |
| Portal container | TopLevel-scoped Overlay session |
| CSS class/variable | ControlTheme、pseudo-class、Semantic Part、Token |
| touch event hook | Gesture policy 与 `MobileGestureCoordinator` |
| controlled/default value pair | Avalonia property system、binding priority、事件或 Command |

当前值使用 `StyledProperty` 或 `DirectProperty`；用户变化使用 RoutedEvent、普通事件或 Command；异步动作使用 `Task`、
`CancellationToken` 和显式 loading/error state。输入验证以 `DataValidationErrors` 为最高优先级，Form 只做协调和投射。

## API 复用层级

| 层级 | 条件 | 结果 |
| --- | --- | --- |
| Tier 1 | 属性、状态机、表单和无障碍语义真正一致 | 继承/复用 `AtomUI.Controls` 抽象，Mobile Theme 独立 |
| Tier 2 | 数据和协调逻辑一致，视觉与交互不同 | 共享 model/coordinator，Mobile Control 独立 |
| Tier 3 | 移动 viewport、overlay、手势或输入语义占主导 | 设计独立 Mobile API |

兼容清单中的 `Tier 1 候选` 和 `Tier 2 候选` 不是已冻结继承关系。对应控件族设计必须以真实 API、Theme、测试和 Gallery
证据完成评审后才能移除“候选”。

## AOT 与资源边界

- Token、Localization、ControlTheme 和 descriptor 使用现有 Generator 或显式静态注册。
- 不通过反射扫描程序集、字符串成员访问或动态 factory 发现 Mobile Control。
- 非 Visual `AvaloniaObject` 需要动态资源时遵循 scoped resource host 规范。
- owner/container/template reapply 必须有 acquire/release 对。
- 真实 iOS/Android Release publish 才能证明裁剪后资源和模板仍可发现。

## 维护不变量

1. Mobile 不拥有第二套 Global Token、Theme Algorithm 或 Localization runtime。
2. 运行时 viewport 和交互状态不进入 Token。
3. CSS variable 名称不直接成为 AtomUI Token API。
4. iOS 与 Android 共享 Public API 和文本契约。
5. API 复用由语义一致性决定，不由同名 Control 决定。
6. `ConfigProvider` 职责由现有 AtomUI 系统承接。

主题系统入口见 [AtomUI 主题系统](../theming/overview.md)，本地化入口见
[AtomUI 本地化系统](../localization/overview.md)。
