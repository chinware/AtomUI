# AtomUI.Generator 模块概览

`AtomUI.Generator` 是 AtomUI 的普通 Roslyn 源生成器项目，TargetFramework 为 `netstandard2.0`。它不作为运行时依赖使用，而是以
Analyzer 方式被多个项目引用。AOT/Trim usage、Sidecar 和 Application Plan 位于独立的 linked-publish Analyzer 中，只在对应
发布构建或 Package Manifest Pack 时加载。

## 职责

- 根据 Global Token 定义生成强类型 schema、资源键和投影代码。
- 根据 public 可主题化 Control、无参数 `[ControlDesignToken]` 标记的可选 Own Token 类型和命名/目录约定生成独立
  Control identity 与 descriptor；Attribute 只负责 Token 发现，不携带 Control 类型、identity 或 ID。
- Control Token 定义继承要求 Generator 区分无 identity 的标记抽象层与 `sealed` 终端类型，将跨程序集
  继承属性扁平化到终端 schema，并在构建期验证继承链、属性冲突和默认计算 base 调用。
- 分析构建系统提供的 `Themes/**/*.axaml`，生成 ControlTheme asset owner、引用 identity、Semantic Part 契约和
  包级 manifest；不收集 Global Token 消费白名单。
- 为每个 Control 包生成包级注册 helper 和 manifest，由真实 `UseXxxControls()` 入口调用；不要求逐 Control 或逐 Theme
  手工注册。
- 根据统一 `AtomUIRegistrationGranularity` 策略生成 Package/Directory Unit identity 和 leaf fragment；普通包默认单一
  Package Unit，大型包才显式按目录拆分。
- linked-publish Generator 从 C#/AXAML 直接证据生成 Sidecar UnitEdge/Usage，在应用编译期计算 closure/SCC 并输出静态计划。
- 根据 `[LanguageCatalog]` enum 与 XLIFF 2.1 生成强类型资源扩展、Catalog/Bundle 注册和应用 bootstrap。
- 消除 Control 包对 Token、主题资产和语言手工清单及运行时程序集扫描的依赖。

## 生成器

| 生成器 | 说明 |
|---|---|
| `TokenResourceKeyGenerator` | 分析 Global Token、可主题化 Control、`[ControlDesignToken]` Own Token 和主题资产输入，生成 exact CLR type/identity、强类型资源键、descriptor、asset manifest 与包级注册代码 |
| `LocalizationGenerator` | 编译 Catalog/XLIFF，生成强类型 Markup Extension、模块注册与应用语言包 bootstrap |
| `LanguageTagsGenerator` | 从固定语言数据生成常用 BCP 47 `LanguageTags` 和标准 `LanguageDefinition` 元数据 |
| `DataMemberAccessorGenerator` | 根据数据模型 Attribute 生成 AOT 友好的数据成员访问器注册 |
| `ScopedResourceHostGenerator` | 根据 `[GenerateScopedResourceHost]` 为非 Visual `AvaloniaObject` 生成 scoped 资源宿主生命周期样板代码 |
| Linked-publish generators | 只在 AOT/Trim 或 Package Manifest Pack 中生成 Sidecar、usage 和 Application Plan；不进入普通编译 |

## 关键目录

| 目录 | 说明 |
|---|---|
| `DesignToken/` | Token Walker、TokenInfo、资源键和类型池 Writer |
| `Localization/` | Catalog symbol、XLIFF、Bundle 编译、模块注册、应用 bootstrap 与 BCP 47 数据生成 |
| `DataMemberAccessors/` | 数据成员访问器 Generator、Analyzer 和 SourceWriter |
| `ResourceHost/` | 非 Visual `AvaloniaObject` scoped resource host Generator、TypeInfo 和 SourceWriter |
| `LinkedRegistration/` | Package 粒度、入口 identity、leaf Unit fragment 和共享协议模型 |
| `TargetMarkConstants.cs` | 生成器识别的 Attribute 元数据名 |

## 专题文档

- [Control Design Token 继承架构](../../architecture/systems/theming/control-design-token-inheritance.md)：抽象/终端类型模型、
  跨程序集属性扁平化、diagnostic、schema revision 和运行时/AOT 边界。
- [Scoped Resource Host Generator](scoped-resource-host-generator.md)：目标识别、生成输出、生命周期状态机、diagnostic 和测试契约。
- [Scoped Resource Host 开发规范](../../engineering/development/scoped-resource-host.md)：Control 作者的适用场景、owner 生命周期、验证和 review 规则。
- [Semantic Part Generator 设计](semantic-part-generator.md)：Control 语义区域的声明、AXAML 校验、descriptor、
  diagnostics、增量生成和 AOT 边界。
- [AOT 与裁剪架构](../../architecture/foundations/aot-and-trimming.md)：linked publish、Registration Unit、Application Plan
  和安全 fallback。
- [Linked Registration Generator](linked-registration.md)：Analyzer 物理隔离、候选增量分析、Sidecar、leaf fragment、
  Application Plan 和性能预算。
- [AOT Linked Registration Pipeline](../../architecture/foundations/aot-linked-registration-pipeline.md)：Sidecar 资产边界、
  构建模式、运行时不变量和验证契约。
- [AOT Registration Unit 粒度](../../architecture/foundations/aot-registration-unit-granularity.md)：粒度策略、资源归属、
  第一方 Package 映射和第三方包契约。
- [第三方 AtomUI Control Package 指南](../../guides/theming/third-party-control-packages.md)：包作者的最小项目、入口和验证步骤。

## 维护注意

新增可主题化 Control、带 `[ControlDesignToken]` 的可选 Own Token、`Themes/**/*.axaml` 资产、Catalog enum 或 XLIFF 后，应检查对应项目的
`GeneratedFiles/AtomUI.Generator/` 输出，确认 exact CLR type/identity、强类型 Token key、descriptor、asset manifest、本地化表和包级注册
结果完整。Control 没有 Own Token 时仍必须生成 identity、`XxxTokenResource` 和零 Own Token descriptor。由于生成
目录被 `<Compile Remove=...>` 排除且默认被 `.gitignore` 忽略，不应把生成文件当成普通源码维护；只有被结构测试
明确读取的 GalleryBase 快照才需要同步提交。

新增或修改 Generator 时，应同时检查 writer 代码、诊断规则和生成物稳定性。linked registration 不得全树遍历、递归进入
callee body 或生成 Unit 间调用；普通 Debug/Release 必须从 compiler command line 上完全排除 linked analyzer。对于非 Visual
`AvaloniaObject` 资源宿主类需求，按 [Scoped Resource Host 开发规范](../../engineering/development/scoped-resource-host.md)
管理 owner 生命周期，并由本模块的 Generator 生成样板代码。

实现 Control Token 定义继承时，抽象层不得生成 identity、descriptor、resource key、Registration Unit 或 linked fragment；
只有终端 Token 产生运行时输出。声明层类型、程序集和继承深度不得进入终端 schema fingerprint。
