# 本地化生成与构建架构

本文定义 `AtomUI.Generator`、`AtomUI.Build.Tasks` 和 MSBuild targets 如何把 Catalog/XLIFF 转换为 AOT 友好的
运行时代码。运行时设计见 [runtime-architecture.md](runtime-architecture.md)，语言包协议见
[language-packs.md](language-packs.md)。

## 构建链总览

```mermaid
flowchart LR
    Enum["LanguageCatalog enum"] --> Generator["AtomUI.Generator"]
    Xlf["Project XLIFF"] --> Additional["AdditionalFiles"]
    Pack["I18n package XLIFF"] --> Additional
    Manifest["Referenced catalog manifests"] --> Additional
    Additional --> Generator
    Tasks["AtomUI.Build.Tasks"] -->|"validate / export / merge"| Xlf
    Generator --> Source["Generated descriptors / tables / extensions / bootstrap"]
    Source --> Assembly["Application or library assembly"]
```

MSBuild 负责发现、分类、校验和传递文件；Generator 负责把文件模型与 Roslyn symbol 结合并生成强类型代码。
两者都不得把 XLIFF 留给运行时处理。

## MSBuild items

`AtomUI.Localization.targets` 自动收集标准目录中的 XLIFF，并排除 `bin/`、`obj/` 和生成目录：

```xml
<AtomUILanguage Include="**/Localization/**/*.xlf" />
<AtomUILanguageOverride Include="Localization/Overrides/**/*.xlf" />
<AtomUILanguageCatalog Include=".../**/*.catalog.xlf" />
```

targets 将这些 item 作为带元数据的 `AdditionalFiles` 传给 Generator。包来源、目标语言、是否 Override、Catalog
manifest 路径和 NuGet package identity 必须保存在 item metadata 中，Generator 不从磁盘路径猜测优先级。

第三方语言包的 `buildTransitive/*.props` 只能追加声明式 item，不能运行初始化代码、修改应用源码或注册运行时
程序集。重复 Include 由规范化绝对路径和 package identity 去重。

## Generator 输入

Generator 使用 Incremental Generator API 组合以下输入：

1. 当前 Compilation 中带 `[LanguageCatalog]` 的 enum symbol。
2. 当前项目 XLIFF `AdditionalText`。
3. 引用项目或 NuGet 暴露的 Catalog manifest/template。
4. 静态 I18n 包和应用 Override 提供的 XLIFF。
5. AnalyzerConfigOptions 提供的 `AssemblyName`、`PackageId`、RootNamespace 和构建策略。

输入必须按规范化 Catalog ID、语言标签、来源优先级和 unit ID 排序，确保不同操作系统、文件枚举顺序和增量
构建下生成结果一致。

## Generator 输出

对于每个 Catalog，Generator 生成：

```text
XxxLangResourceExtension
LanguageCatalogDescriptor
catalog/unit slot mapping
built-in TranslationBundleDescriptor
compiled string and CompositeFormat tables
```

对于每个 Language Module，Generator 生成：

```text
GeneratedLanguageModuleRegistration
LanguageCatalogManifest
catalog template metadata
```

对于最终应用，Generator 另外生成：

```text
GeneratedApplicationLanguageBootstrap
I18n package TranslationBundle registration
application Override registration
```

`XxxLangResourceKind` 由开发者声明，不再根据三份 C# 翻译类推导。`XxxLangResourceExtension` 保留现有 XAML
形态，但底层改为生成式 descriptor 和 Snapshot 查询。

## 应用 bootstrap

当项目存在本地化输入时，Generator 为唯一的具体、顶层、非泛型 `partial` Avalonia `Application` 类型实现一个
受控的生成式 host 契约。抽象 Application 基类不参与唯一性判断；没有 Application 的类库仍只生成模块注册，
不生成应用 bootstrap，也不回退到程序集扫描。`UseAtomUI()` 只做接口判断和直接调用：

```text
Application
  -> generated application module
  -> application Catalogs
  -> language-pack bundles
  -> application overrides
```

这不是程序集扫描：具体 Application 类型、生成类型和调用目标都在编译期可见。应用包含本地化输入但
Application 类型不可扩展时，Generator 必须产生诊断，不能回退到 `Assembly.GetTypes()`。

类库/控件包生成自己的 `GeneratedLanguageModuleRegistration`。包级 `UseXxx()` 入口调用生成注册代码，将该模块的
Catalog 和内置翻译交给 `ILocalizationBuilder`；开发者不手写 descriptor、Catalog ID 或 Provider 列表。

语言包没有程序集，其 Translation Bundle 由最终应用 Generator 生成。Bundle 可以早于或晚于目标 Language Module
进入 Builder，Registry 构建阶段统一关联，注册顺序不构成覆盖规则。

## AOT 约束

生成代码必须直接包含：

- Catalog enum CLR 类型和生成式 Catalog ID。
- enum 数字值到 unit slot 的 switch/只读表。
- 每个语言的编译后字符串数组。
- 格式化资源的预验证 `CompositeFormat` 数据或等价静态构造路径。
- Language Module 和应用 bootstrap 的直接注册调用。

正常路径禁止：

- `Assembly.GetTypes()`、`Type.GetFields()`、`Enum.GetNames()`。
- 通过 Attribute 反射发现 Catalog。
- `Activator.CreateInstance()` 创建 Provider 或 Markup Extension 注册项。
- 运行时 XML/XLIFF 解析、路径 glob 或 NuGet 包探测。
- 依赖字符串类型名构造资源键。

## AtomUI.Build.Tasks

物理项目位于：

```text
src/AtomUI.Build.Tasks
```

它是内部编译型 MSBuild Task 程序集，不作为应用运行时引用，也不单独发布
`AtomUI.Localization.Build`。主要 Task 为：

| Task | 职责 |
|---|---|
| `CollectLanguageCatalogsTask` | 收集项目与依赖 Catalog manifest，规范化来源元数据 |
| `ValidateLanguageFilesTask` | 校验 XLIFF 2.1、BCP 47、unit、状态、占位符和重复来源 |
| `ExportLanguageTemplatesTask` | 按目标语言导出/更新可翻译 XLIFF 模板 |
| `PrepareLanguagePackageTask` | 校验目标 Catalog 契约、生成包 manifest 和 contentFiles 清单 |
| `GenerateLanguagePackagePropsTask` | 生成声明式 buildTransitive props/targets |

Task 内部协作组件包括：

```text
Xliff21Reader
Xliff21Writer
XliffMergeEngine
XliffValidator
LanguageCatalogManifestReader
PackagePropsWriter
```

Generator 与 Build Tasks 对 XLIFF 使用同一规范化模型和诊断定义。纯 XLIFF 解析/模型代码以构建期内部共享源码
编译进两个程序集，不增加公开运行时包，也不让 MSBuild Task 依赖 Roslyn workspace。

## Generator NuGet 布局

```text
AtomUI.Generator.nupkg
├── analyzers/dotnet/cs/AtomUI.Generator.dll
├── tools/netstandard2.0/AtomUI.Build.Tasks.dll
└── buildTransitive/
    ├── AtomUI.Generator.props
    ├── AtomUI.Generator.targets
    └── AtomUI.Localization.targets
```

`AtomUI.Build.Tasks` 的依赖必须随 tools 目录完整发布。Generator 项目继续隔离 `PublishAot`、trim、single-file 和
RuntimeIdentifier 等全局发布属性，不能被最终应用当作运行时项目参与 NativeAOT publish。

## 编译期与启动期校验边界

XLIFF 结构、单个 Bundle 完整性、Catalog 契约、重复来源和包 manifest 在构建期校验。
`UseLanguages()` 是普通 C# 配置；Analyzer 能静态识别直接使用 `LanguageTags.*` 的标准调用时，应提前报告最终
覆盖缺失。配置由变量、条件或私有标签动态构造时，完整支持语言集合只能在 Builder 冻结 Registry 时校验，并在
首帧前失败。实现不得为了声称“全部构建期校验”而使用运行时反射或要求应用重复维护字符串语言列表。

## 导出与合并

标准命令使用 MSBuild target，不创建独立 CLI：

```bash
dotnet msbuild \
  -t:AtomUIExportLanguageTemplates \
  -p:AtomUITargetLanguage=ja-JP
```

重复导出必须确定性合并：新增 unit 加入目标文件，删除 unit 标为 obsolete，源文本变化将目标标记为需复核，
已有 target、state 和 notes 保留。工具不得静默删除译文或把旧译文视为新源文本的已确认翻译。
