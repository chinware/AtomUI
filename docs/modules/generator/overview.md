# AtomUI.Generator 模块概览

`AtomUI.Generator` 是 AtomUI 的 Roslyn 源生成器项目，TargetFramework 为 `netstandard2.0`。它不作为运行时依赖使用，而是以 Analyzer 方式被多个项目引用。

## 职责

- 根据 Global Token 定义生成强类型 schema、资源键和投影代码。
- 根据 public 可主题化 Control、无参数 `[ControlDesignToken]` 标记的可选 Own Token 类型和命名/目录约定生成独立
  Control identity 与 descriptor；Attribute 只负责 Token 发现，不携带 Control 类型、identity 或 ID。
- 分析构建系统提供的 `Themes/**/*.axaml`，生成 ControlTheme asset owner、引用 identity、Semantic Part 契约和
  包级 manifest；不收集 Global Token 消费白名单。
- 为每个 Control 包生成一次包级注册入口；不要求逐 Control 或逐 Theme 手工注册。
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

## 关键目录

| 目录 | 说明 |
|---|---|
| `DesignToken/` | Token Walker、TokenInfo、资源键和类型池 Writer |
| `Localization/` | Catalog symbol、XLIFF、Bundle 编译、模块注册、应用 bootstrap 与 BCP 47 数据生成 |
| `DataMemberAccessors/` | 数据成员访问器 Generator、Analyzer 和 SourceWriter |
| `ResourceHost/` | 非 Visual `AvaloniaObject` scoped resource host Generator、TypeInfo 和 SourceWriter |
| `TargetMarkConstants.cs` | 生成器识别的 Attribute 元数据名 |

## 专题文档

- [Scoped Resource Host Generator](scoped-resource-host-generator.md)：目标识别、生成输出、生命周期状态机、diagnostic 和测试契约。
- [Scoped Resource Host 开发规范](../../engineering/development/scoped-resource-host.md)：Control 作者的适用场景、owner 生命周期、验证和 review 规则。
- [Semantic Part Generator 设计](semantic-part-generator.md)：Control 语义区域的声明、AXAML 校验、descriptor、
  diagnostics、增量生成和 AOT 边界。

## 维护注意

新增可主题化 Control、带 `[ControlDesignToken]` 的可选 Own Token、`Themes/**/*.axaml` 资产、Catalog enum 或 XLIFF 后，应检查对应项目的
`GeneratedFiles/AtomUI.Generator/` 输出，确认 exact CLR type/identity、强类型 Token key、descriptor、asset manifest、本地化表和包级注册
结果完整。Control 没有 Own Token 时仍必须生成 identity、`XxxTokenResource` 和零 Own Token descriptor。由于生成
目录被 `<Compile Remove=...>` 排除且默认被 `.gitignore` 忽略，不应把生成文件当成普通源码维护；只有被结构测试
明确读取的 GalleryBase 快照才需要同步提交。

新增或修改 Generator 时，应同时检查 writer 代码、诊断规则和生成物稳定性。对于非 Visual `AvaloniaObject` 资源宿主类需求，按 [Scoped Resource Host 开发规范](../../engineering/development/scoped-resource-host.md) 管理 owner 生命周期，并由本模块的 Generator 生成样板代码。
