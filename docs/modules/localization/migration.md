# 多语言模块迁移方案

本文定义从当前 `ThemeManager + LanguageProvider + C# 翻译类` 架构迁移到独立应用级本地化基础设施的边界。
它只描述迁移顺序和兼容结果，不允许旧实现反向决定目标架构。

## 当前实现问题

当前语言系统具有以下结构性限制：

- `ThemeManager` 同时实现主题和 `ILanguageManager`，语言资源被当作主题静态资源的一部分挂载。
- `LanguageCode` 和 `LanguageVariant` 只能表达 `en-US`、`zh-CN`、`zh-TW`。
- 每个语言由一份 C# 常量类表示，翻译不使用标准交换格式。
- `LanguageProviderAttribute`、Provider 构造和资源字段曾依赖运行时 Attribute/字段反射。
- `LanguageProviderPool` 和 `IThemeManagerBuilder.AddLanguageProvider()` 把本地化注册绑在主题/控件包注册上。
- 应用 ViewModel 和服务缺少独立、强类型、DI 中立的 `ILocalizer`。
- 第三方新增语言需要扩展固定 enum/API，无法形成静态 NuGet 翻译协议。

## 迁移后的对应关系

| 当前类型或机制 | 目标 |
|---|---|
| `LanguageCode` | `LanguageTag` + `LanguageTags` |
| `LanguageVariant` | `LanguageState.CurrentLanguage` / `LanguageDefinition` |
| `LanguageProvider` / `ILanguageProvider` | 生成式 `TranslationBundleDescriptor` |
| `LanguageProviderAttribute` | `[LanguageCatalog]` enum + XLIFF |
| 每语言 C# 常量类 | `en-US.xlf`、`zh-CN.xlf`、`zh-TW.xlf` |
| `LanguageProviderPool` | `GeneratedLanguageModuleRegistration` |
| ThemeManager language dictionaries | `LanguageCatalogRegistry` + `LanguageSnapshot` |
| ThemeManager merged dictionary switching | 稳定 `LanguageResourceProvider` Snapshot 交换 |
| `WithDefaultCultureInfo()` | `UseLanguages()` + `LanguageTag.FromCultureInfo()` 边界转换 |
| `IThemeManagerBuilder` 根入口 | `IAtomUIBuilder` |

## 必须保留的兼容面

现有应用和组件 XAML 形态必须保留：

```xml
<TextBlock Text="{atom:DatePickerLangResource Today}" />
<TextBlock Text="{atom:CommonLangResource Ok}" />
```

迁移后仍生成同名：

```text
DatePickerLangResourceKind
DatePickerLangResourceExtension
CommonLangResourceKind
CommonLangResourceExtension
```

AtomUI 公共 Catalog 的 enum namespace、类型名和成员名应尽量保持不变，使 AXAML namespace 与资源调用无需批量
改写。底层基类、descriptor、slot 和资源 Provider 可以完全替换。

## 明确不保留的 API

目标架构完成后删除：

```text
LanguageCode
LanguageVariant
LanguageVariantTypeConverter
LanguageProvider
ILanguageProvider
LanguageProviderAttribute
LanguageProviderPool
IThemeManagerBuilder.AddLanguageProvider()
IThemeManagerBuilder.WithDefaultLanguageVariant()
```

不创建逐语言兼容扩展：

```csharp
UseAtomUIZhCN();
UseAtomUIZhHans();
UseAtomUIJaJP();
```

也不保留 `UseAtomUILanguages()`；统一入口是根 `IAtomUIBuilder` 上的 `UseLanguages()`。

## Catalog 迁移

每组现有 Provider 按 `LanguageId` 转成一个源代码 Catalog enum：

```text
DatePicker en_US/zh_CN/zh_TW
  -> DatePickerLangResourceKind.cs
  -> en-US.xlf
  -> zh-CN.xlf
  -> zh-TW.xlf
```

迁移工具或脚本只负责一次性抽取现有常量、生成候选 XLIFF 和显式数字 ID；结果必须人工审查并进入正常源码。
生成文件不能长期依赖旧 C# Provider，也不能在运行时提供双轨资源。

ID 分配规则：

- 以迁移时确定的 Catalog 成员清单分配从 1 开始的稳定 ID。
- 分配后立即写入 enum 和 XLIFF `unit id`，不再依赖字母排序或声明顺序。
- 迁移清单记录旧成员名到新 ID，防止分批迁移时不同语言产生不同编号。
- 已发布的 enum 底层数值如果被外部持久化，应在对应 Catalog 迁移前单独评估；不能假设现有隐式值是契约。

## 实施阶段

### 1. 建立新基础设施

- 创建 `AtomUI.Localization`、运行时类型和测试项目。
- 创建 `AtomUI.Build.Tasks`，扩展 `AtomUI.Generator` 和 NuGet buildTransitive 资产。
- 引入 `IAtomUIBuilder`，让主题和本地化成为并列子系统。
- 新旧语言系统暂时可在源码中并存，但新运行时不适配旧 Provider。

### 2. 迁移基础 Catalog

- 先迁移 AtomUI.Core/Common Catalog，验证 XLIFF、Generator、Extension 和 Snapshot 全链路。
- 迁移 `AtomUI.Controls` 的公共 Catalog，并验证应用只使用公共控件时的启动注册。
- 建立三种内置语言完整性检查。

### 3. 迁移桌面和可选包

- 按 Desktop Controls、DataGrid、ColorPicker、Extras 的包边界迁移。
- 每个包只注册自己拥有的 Language Module，不把翻译集中回 Core。
- 迁移期间每完成一个包就删除该包旧 Provider，避免同一 Catalog 双重注册。

### 4. 切换应用与 Gallery

- Gallery 和测试 Application 改用 `UseLanguages()`。
- Gallery 页面、导航、ViewModel 和服务统一使用 Catalog enum/`ILocalizer`。
- 验证应用级 Catalog 与控件 Catalog 在同一 Snapshot 中工作。

### 5. 删除旧架构

- 从 `ThemeManager` 删除 `ILanguageManager`、LanguageVariantProperty、Provider 列表、语言字典和切换逻辑。
- 从 `ControlPackageRegistration`、Theme Builder 和生成注册中移除语言 Provider。
- 删除旧 Generator Walker/Writer、Attribute、反射 fallback 和 C# 翻译类。
- 搜索仓库确认不存在旧类型和逐语言注册扩展。

### 6. 发布与文档同步

- 发布 `AtomUI.Localization` 与更新后的 `AtomUI.Generator` 构建资产。
- 验证语言包模板、模板导出和一个真实附加语言包。
- 更新 `docs/architecture/dependency-graph.md`、`startup-and-registration.md`、Core/Generator overview 和 AGENTS
  Required Reading，使它们描述已经落地的事实。
- 在实现完成前不得提前把当前架构总览改写成新项目已经存在。

## 双轨限制

迁移期间允许源码层面分包推进，但单个 Catalog 在任何可运行应用中只能由一个系统提供。禁止：

- 同时把旧 ResourceDictionary 和新 Provider 挂到 Application。
- 让新 `ILocalizer` 回退查询旧 ThemeManager。
- 用 adapter 把 `LanguageProvider` 包装成 Translation Bundle 作为长期兼容层。
- 在新 Snapshot 缺值时搜索旧 Provider。

这些做法会制造两个语言状态源、重复资源通知和无法删除的过渡架构。

## 完成条件

迁移只有同时满足以下条件才算完成：

1. 应用、AtomUI 控件和第三方示例都可定义并使用自己的 Catalog。
2. 所有官方 Catalog 使用 XLIFF 2.1，三种内置语言覆盖完整。
3. 现有 `{atom:XxxLangResource Key}` XAML 用法保持工作。
4. `ThemeManager` 不再拥有任何语言状态或 Provider。
5. 仓库不存在运行时 XLIFF 解析、语言程序集扫描或旧 LanguageProvider 注册。
6. 一个静态 I18n NuGet 可以通过 PackageReference 为新语言提供完整翻译。
7. 普通测试、Generator/Task 测试、trim/AOT analyzer 和真实 NativeAOT publish 全部通过。
