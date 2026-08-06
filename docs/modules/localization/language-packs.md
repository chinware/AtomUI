# AtomUI 静态语言包架构

本文定义 AtomUI 官方和第三方如何发布、命名、构建和消费附加翻译包。语言包只参与应用构建，不是运行时插件。

## 内置语言

AtomUI 官方组件内置以下语言：

```text
en-US
zh-CN
zh-TW
```

三种翻译随拥有 Catalog 的组件程序集发布，不拆分为独立 I18n NuGet。`AtomUI.Controls`、
`AtomUI.Desktop.Controls`、DataGrid、ColorPicker 和 Extras 各自编译并注册自己拥有的 Catalog，不由一个中央
程序集复制所有翻译。

拥有 Catalog 的组件 NuGet 自动包含完整 `en-US.xlf` 和 `buildTransitive/<PackageId>.props`。这些构建资产既是
语言包模板来源，也是最终应用校验源文本和 ContractVersion 的权威输入；它们不会在应用运行时解析。

## 包命名规则

附加语言包名称使用：

```text
{TranslationScope}.I18n.{LanguageTagIdentifier}
```

BCP 47 标签转换为 PascalCase 标识后缀，包内 XLIFF 与审计 manifest 仍保存规范标签：

| BCP 47 | 包名后缀 |
|---|---|
| `ja-JP` | `JaJP` |
| `ko-KR` | `KoKR` |
| `zh-Hant` | `ZhHant` |
| `zh-Hant-HK` | `ZhHantHK` |
| `sr-Latn-RS` | `SrLatnRS` |

NuGet package ID 不能充当语言身份解析来源。包内必须显式包含：

```xml
<AtomUILanguageTag>ja-JP</AtomUILanguageTag>
```

## 官方聚合包

产品级官方附加翻译使用：

```text
AtomUI.I18n.JaJP
AtomUI.I18n.KoKR
AtomUI.I18n.FrFR
AtomUI.I18n.DeDE
```

`AtomUI.I18n.JaJP` 表示 AtomUI 官方组件体系的日语聚合包。完整覆盖发布集合是官方包的发布策略；当前编译器
不会根据产品级包名推断或单独校验“官方 Catalog 全集”。聚合包可以同时包含 Controls、Desktop Controls、
DataGrid、ColorPicker 和 Extras 的 Translation Bundle；应用 Runtime 只为实际注册的 Catalog 构建 Snapshot。

普通用户优先只引用一个聚合包：

```xml
<PackageReference Include="AtomUI.I18n.JaJP" Version="$(AtomUIVersion)" />
```

## 模块级包

确实独立维护、发布或兼容某个模块时，名称必须包含目标模块：

```text
AtomUI.Controls.I18n.JaJP
AtomUI.Desktop.Controls.I18n.JaJP
AtomUI.Desktop.Controls.DataGrid.I18n.JaJP
AtomUI.Desktop.Controls.ColorPicker.I18n.JaJP
```

产品聚合包与模块包不是两个可以任意叠加的覆盖层。它们提供相同 Catalog/语言时，应用构建报告同优先级冲突。
当前协议没有 manifest 组成关系或按 package identity 合并翻译的语义；聚合包不能让消费图同时暴露自身副本和
模块包中的同一份 XLIFF。

## 第三方命名

第三方使用自己拥有的产品或模块作用域：

```text
Acme.AtomUI.I18n.JaJP
Acme.AtomUI.Desktop.Controls.I18n.JaJP
Contoso.DataGrid.I18n.ZhTW
Acme.ProductivitySuite.I18n.DeDE
```

名称只表达所有者和预期范围。编译目标由 `buildTransitive/<PackageId>.props` 上的 module/contract/source
metadata、XLIFF `file id` 和消费应用引用的 Catalog enum 共同确定；构建系统不根据包名前缀或 manifest 猜测
Catalog 所有权。

## NuGet 内容协议

静态语言包是 content/build assets 包：

```text
{TranslationScope}.I18n.JaJP.nupkg
├── contentFiles/any/any/AtomUI.LanguagePack.xml
├── contentFiles/any/any/<CatalogPath>/ja-JP.xlf
└── buildTransitive/{PackageId}.props
```

实际 `.nupkg` 文件名由 NuGet 自动加入包版本。包设置 `IncludeBuildOutput=false`，不得包含运行时翻译 DLL。

`AtomUI.LanguagePack.xml` 是由打包任务根据 XLIFF 确定性生成的审计和工具产物，至少记录：

- package identity 和规范目标 `LanguageTag`。
- 每个目标 Catalog 的 module ID、Catalog metadata ID、ContractVersion 和包内路径。
- 每个目标 Catalog 的规范源文本指纹。

`buildTransitive/<PackageId>.props` 是编译权威入口。它只把每个 XLIFF 作为带 source kind、source identity、
module ID、ContractVersion、包内路径和源 fingerprint 的 `AtomUILanguage` item 注入；标准 targets 再将这些 XLIFF
加入 `AdditionalFiles`。manifest 不进入 `AdditionalFiles`，Generator 也不会独立发现或读取它。最终应用 Generator
校验 metadata、实际 XLIFF 和引用 Catalog 后把翻译编译进应用程序集；运行时不需要知道翻译来自哪个 NuGet 文件。

## 语言包制作模板

不提供 `AtomUI.Localization.Tool` CLI。保留标准 `dotnet new` 项目模板：

```bash
dotnet new install AtomUI.LanguagePack.Template

dotnet new atomui-language-pack \
  --name Acme.AtomUI.I18n.JaJP \
  --packageId Acme.AtomUI.I18n.JaJP \
  --moduleId AtomUI.Desktop.Controls \
  --languageTag ja-JP \
  --languageTagIdentifier JaJP
```

模板生成：

- 不产出 DLL 的 SDK-style pack 项目。
- `AtomUILanguageTag`、PackageId 和 package metadata。
- `AtomUI.Generator` 构建期 PackageReference。
- `Localization/` 目录和带完整来源元数据的 `AtomUILanguage` item。
- build/pack 校验 target 和最小翻译说明。

模板不复制 AtomUI 当前 Catalog。作者通过项目引用和导出目标取得准确模板：

```bash
dotnet msbuild \
  -t:AtomUIExportLanguageTemplates \
  -p:AtomUITargetLanguage=ja-JP

dotnet build
dotnet pack
```

## 第三方组件作者职责

希望别人制作语言包的组件或类库必须：

1. 使用 `[LanguageCatalog]` 和稳定显式数字 ID。
2. 提供完整 `en-US`，并声明自身内置语言。
3. 引用 `AtomUI.Generator`；模块主包由标准 targets 自动发布 `en-US` 与声明式 props 构建资产。
4. 保持 Catalog ID、ContractVersion 和源文本指纹可追踪。
5. 不提供运行时反射 Provider、手写 LanguagePack Descriptor 或自定义 DLL 加载入口。

第三方类库也可以在自身主包内置多种语言；只有需要让翻译独立发布、独立维护或由社区提供时才创建 I18n 包。

## 应用消费

应用只添加 PackageReference，并继续通过统一入口声明支持语言：

```csharp
builder.UseLanguages(
    defaultLanguage: LanguageTags.JaJP,
    supportedLanguages:
    [
        LanguageTags.EnUS,
        LanguageTags.JaJP
    ]);
```

```xml
<PackageReference Include="AtomUI.I18n.JaJP" Version="$(AtomUIVersion)" />
```

还必须正常注册目标组件模块，例如 `builder.UseDesktopControls()`。语言包只提供 Translation Bundle，不会隐式
注册组件、主题或 Catalog。应用 Generator 从模块程序集解析强类型 Catalog，并把语言包目标 XLIFF 编译进应用程序集。

不存在以下 API：

```csharp
builder.LoadLanguagePack(...);
builder.AddLanguagePackDescriptor(...);
builder.UseAtomUIJaJP();
```

## 冲突与兼容

- props metadata、XLIFF `file id` 或引用 Catalog identity 不一致时构建失败。
- Catalog ContractVersion 不兼容时构建失败。
- props 中的源 fingerprint 必须是当前 XLIFF 源契约的 64 位小写 SHA-256；目标源文本还必须与权威 `en-US` 一致。
- 两个来源提供相同 Catalog/语言且优先级相同时构建失败。
- 语言包缺少目标 Catalog 的新增 unit 时视为覆盖不完整。
- 应用显式 Override 是唯一允许高于语言包的覆盖来源。

## 明确不支持

- 应用运行后下载 NuGet、XLIFF 或任意语言文件。
- `.atomlang` 文件。
- 第三方语言程序集、插件目录或 AssemblyLoadContext 加载。
- 运行时修改 Catalog、追加资源或刷新翻译目录。

动态语言加载不属于当前协议；不得复用静态 XLIFF 构建链或为其预留缺少完整安全语义的 API。
