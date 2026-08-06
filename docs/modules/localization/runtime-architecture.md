# AtomUI 多语言运行时架构

本文定义本地化运行时的状态所有权、启动构建、查询、回退和语言切换语义。运行时位于
`AtomUI.Localization`，不依赖 ThemeManager 的内部事务或语言字段。

## 核心组件

```text
LanguageManager
├── LanguageCatalogRegistry
├── IReadOnlyDictionary<LanguageTag, LanguageSnapshot>
├── LanguageResourceProvider
├── LanguageState
└── Localizer
```

| 组件 | 职责 | 不负责 |
|---|---|---|
| `LanguageCatalogRegistry` | 冻结 Catalog schema、slot、翻译来源和语言元数据 | 当前语言、资源通知 |
| `LanguageSnapshot` | 保存某一语言完成回退后的全部资源值 | 解析 XLIFF、动态修改 |
| `LanguageManager` | 验证并提交全局语言变化，发布状态和事件 | 主题事务、下载语言包 |
| `LanguageResourceProvider` | 把当前 Snapshot 投影为 Avalonia 动态资源 | 保存第二份翻译状态 |
| `Localizer` | 为 C# 调用提供强类型查询和格式化 | 字符串 Catalog 路径解析 |

## 启动构建

`UseAtomUI()` 按以下顺序构建本地化运行时：

1. 创建根 `IAtomUIBuilder`，并接入应用生成的 Language Module bootstrap。
2. 执行用户配置；`UseXxxControls()` 等扩展注册各自生成的 Language Module。
3. `ILocalizationBuilder` 收集 Catalog descriptor、内置 Translation Bundle、语言包 Bundle 和应用 Override。
4. 规范化 `defaultLanguage`、`supportedLanguages` 与 `LanguageDefinition`；支持语言按首次出现顺序去重，显式
   `LanguageDefinition` 仍要求标签唯一，并验证 Culture 与方向元数据。
5. 构建并冻结 `LanguageCatalogRegistry`，解析来源优先级、Catalog 契约和 slot。
6. 为每个支持语言构建完整不可变 `LanguageSnapshot`；任何必需资源无法解析时启动失败。
7. 用默认语言 Snapshot 初始化唯一 `LanguageManager` 和稳定 `LanguageResourceProvider`。
8. 将 Provider 挂载到 Application 资源链，并注册 `ILanguageManager`、`ILocalizer` 的同一运行时实例。
9. 发布完整初始资源后再让应用进入首帧，不在首帧后补做默认语言切换。

Builder 只收集输入，`Build()` 后全部注册表冻结。运行时不允许追加 Catalog 或 Translation Bundle。

## Catalog registry

Registry 为每个 Catalog 分配进程内稳定 slot，并记录：

- 生成式 Catalog ID、ContractVersion 和 enum CLR 类型。
- 数字 unit ID 到 Catalog 内部 slot 的生成式映射。
- 每个语言的 Translation Bundle、来源优先级和源文本指纹。
- `en-US` 完整源文本。
- 用于 Avalonia enum resource key 查询的生成式映射。

Registry 可以使用 `Type` 和 enum 数值作为已经静态可见的字典键，但不得通过 `GetFields()`、`Enum.GetNames()`、
Attribute 反射或 `Activator.CreateInstance()` 发现 Catalog。

## LanguageSnapshot

Snapshot 是某个请求语言的完整、不可变、已经完成回退解析的资源表：

```text
LanguageSnapshot
├── RequestedLanguage
├── Resolved values by catalog slot and unit slot
├── Resolved source language per value
└── Revision-independent immutable storage
```

查询热路径不再遍历 BCP 47 回退链，也不合并 ResourceDictionary。Snapshot 构建阶段完成全部解析，
`ILocalizer.Get()` 和 Avalonia 资源查询只执行 slot 定位与数组读取。

Snapshot 不暴露可变 Dictionary，不缓存调用方格式化结果。所有支持语言 Snapshot 在启动时构建，确保静态
语言切换为确定的内存引用交换，不在用户操作时突然解析大量资源。

## LanguageState

```csharp
public sealed record LanguageState(
    LanguageTag CurrentLanguage,
    CultureInfo FormattingCulture,
    LanguageTextDirection TextDirection,
    long Revision);
```

- `CurrentLanguage` 是当前请求并已提交的文本语言。
- `FormattingCulture` 用于 `ILocalizer.Format()` 以及 AtomUI 控件的数字、日期和时间显示。
- `TextDirection` 是 LTR 或 RTL，供根视觉宿主投影 `FlowDirection`。
- `Revision` 每次成功切换递增，`NoOp` 不递增。

语言、格式化 Culture 和文字方向相关但不等价。应用可以为私有语言标签提供自定义 Culture 和方向。

## 回退算法

每个 Catalog、每个 unit 独立使用同一确定链：

```text
exact tag -> script/parent candidates -> neutral language -> en-US
```

示例：

```text
zh-Hant-HK -> zh-Hant -> zh -> en-US
zh-CN      -> zh-Hans -> zh -> en-US
zh-TW      -> zh-Hant -> zh -> en-US
fr-CA      -> fr -> en-US
```

候选链由 BCP 47 结构和固定 CLDR likely-subtags 数据生成，不用字符串 `Split('-')` 临时猜测。`zh-CN` 不把
`zh-TW` 作为候选，反向同理。只有显式提供的中性 `zh` Translation Bundle 才能被两个脚本族共同使用。

`en-US` 是最终安全值，但对于非英语支持语言，构建时直接落到 `en-US` 不计为翻译覆盖；这一规则防止应用
声明支持日语却悄悄显示整页英文。

## 语言切换事务

静态语言切换使用小型、独立的同步提交，不复用 ThemeTransaction：

1. 验证 UI 线程访问、Manager 未释放且目标属于支持语言；失败分别抛出文档规定的类型化异常。
2. 目标与当前语言相同则返回 `NoOp`。
3. 读取启动时构建的目标 Snapshot 和对应 `LanguageDefinition`。
4. 在提交点原子替换当前 Snapshot 与 `LanguageState`。
5. `LanguageResourceProvider` 发布一次 hosted resources changed 通知。
6. 根文字方向投影与 Snapshot 属于同一提交，不暴露文本已变而方向未变的状态。
7. 发布一次 `LanguageChanged`，事件参数包含旧/新 State 和结果 revision。

提交前失败不改变旧状态；提交点之后不执行可能失败的解析、加载或用户代码。事件订阅方抛出异常不得回滚
已提交语言，也不得阻止其他订阅方按既定事件策略收到通知。

## Stable LanguageResourceProvider

Provider 在 Application 初始化时只挂载一次。它始终从 `LanguageManager` 的当前 Snapshot 读取，不为每种语言
维护一份可变 Avalonia `ResourceDictionary`，切换时也不从 MergedDictionaries 删除或添加字典。

Generator 生成的 `XxxLangResourceExtension` 继续基于 enum key 创建动态资源引用。Provider 负责把 exact enum
类型和值映射到 Catalog/unit slot。语言通知只使已有动态资源重新查询，不替换 Markup Extension 或资源 key。

非 StyledElement 的 XAML 目标如果只能在 ProvideValue 时取得静态值，应通过 `ILocalizer` 读取当前 Snapshot；
这类对象不会假装获得自身不存在的 Avalonia 动态资源生命周期。

## 线程模型

- `ChangeLanguage()`、Provider 挂载和资源通知只在 UI 线程发生。
- Snapshot 与 State 使用不可变对象和原子引用发布，`ILocalizer.Get()` 可从后台线程无锁读取一个完整 revision。
- 单次 `Get()` 或 `Format()` 必须只观察一个 Snapshot；切换不能让一次格式化混用两个语言 revision。
- 事件在 UI 线程发布。后台服务需要响应语言变化时自行调度耗时工作，不阻塞提交路径。

## Culture 策略

AtomUI 默认不修改以下全局状态：

```csharp
CultureInfo.CurrentCulture
CultureInfo.CurrentUICulture
CultureInfo.DefaultThreadCurrentCulture
CultureInfo.DefaultThreadCurrentUICulture
```

AtomUI 控件必须读取 `LanguageState.FormattingCulture`，不能依赖调用线程碰巧具有相同 Culture。应用确实需要
同步进程 Culture 时，应在应用层订阅已提交的 `LanguageChanged` 并采用显式策略；该行为不进入 Manager 默认语义。

## 生命周期

`LanguageManager` 与 Application 生命周期一致。释放时必须解绑 Application 资源宿主、根 FlowDirection 投影和
内部订阅，并清空公开事件。Snapshot 和生成表不持有 Visual、ViewModel、DI Scope 或翻译包文件句柄。
