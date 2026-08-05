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

组件 NuGet 可以包含供语言包作者使用的 `*.catalog.xlf` 和 manifest；它们属于构建资产，不会在应用运行时解析。

## 包命名规则

附加语言包名称使用：

```text
{TranslationScope}.I18n.{LanguageTagIdentifier}
```

BCP 47 标签转换为 PascalCase 标识后缀，包 manifest 仍保存规范标签：

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

`AtomUI.I18n.JaJP` 表示 AtomUI 官方组件体系的日语聚合包，必须覆盖该发布集合声明的全部官方 Catalog，不能只
提供 `AtomUI.Desktop.Controls` 翻译却使用产品级名称。聚合包可以同时包含 Controls、Desktop Controls、DataGrid、
ColorPicker 和 Extras 的 Translation Bundle；应用 Runtime 只为实际注册的 Catalog 构建 Snapshot。

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

产品聚合包与模块包不是两个可以任意叠加的覆盖层。它们提供相同 Catalog/语言/unit 时，应用构建报告冲突；
聚合包若通过依赖组合模块包，其 manifest 必须把这些模块包声明为组成部分，由 package identity 去重，而不是
再次复制相同 XLIFF。

## 第三方命名

第三方使用自己拥有的产品或模块作用域：

```text
Acme.AtomUI.I18n.JaJP
Acme.AtomUI.Desktop.Controls.I18n.JaJP
Contoso.DataGrid.I18n.ZhTW
Acme.ProductivitySuite.I18n.DeDE
```

名称只表达所有者和预期范围，真实目标始终由 package manifest 中的 Catalog ID 列表决定。构建系统不能仅根据
包名前缀猜测它有权翻译哪些 Catalog。

## NuGet 内容协议

静态语言包是 content/build assets 包：

```text
AtomUI.I18n.JaJP.nupkg
├── contentFiles/any/any/AtomUI/Localization/ja-JP/**/*.xlf
├── buildTransitive/AtomUI.I18n.JaJP.props
├── buildTransitive/AtomUI.I18n.JaJP.targets
└── build/AtomUI.I18n.JaJP.manifest.json
```

实际 `.nupkg` 文件名由 NuGet 自动加入包版本。包设置 `IncludeBuildOutput=false`，不得包含运行时翻译 DLL。

manifest 至少记录：

- package identity 和规范目标 `LanguageTag`。
- 聚合或模块翻译作用域。
- 每个目标 Catalog ID、ContractVersion 和源 manifest identity。
- 每个 unit 的源文本指纹或等价 Catalog fingerprint。
- 生成该包所使用的 AtomUI 本地化构建协议版本。

`buildTransitive` 只把 XLIFF 和 manifest 追加为带来源元数据的 `AdditionalFiles`。最终应用 Generator 将翻译编译进
应用程序集；运行时不需要知道翻译来自哪个 NuGet 文件。

## 语言包制作模板

不提供 `AtomUI.Localization.Tool` CLI。保留标准 `dotnet new` 项目模板：

```bash
dotnet new install AtomUI.LanguagePack.Template

dotnet new atomui-language-pack \
  --name Acme.AtomUI.I18n.JaJP \
  --language ja-JP
```

模板生成：

- 不产出 DLL 的 SDK-style pack 项目。
- `AtomUILanguageTag`、PackageId 和 package metadata。
- Catalog 来源 PackageReference 示例。
- `Localization/<tag>/` 目录。
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
3. 引用 `AtomUI.Generator`，发布生成的 Catalog manifest/template 构建资产。
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

不存在以下 API：

```csharp
builder.LoadLanguagePack(...);
builder.AddLanguagePackDescriptor(...);
builder.UseAtomUIJaJP();
```

## 冲突与兼容

- 一个包提供 manifest 未声明的 Catalog 翻译时构建失败。
- Catalog ContractVersion 不兼容时构建失败。
- 源文本指纹变化时目标翻译进入需复核状态，不能静默沿用。
- 两个非组合关系的包提供相同 Catalog/语言/unit 时构建失败。
- 语言包缺少目标 Catalog 的新增 unit 时视为覆盖不完整。
- 应用显式 Override 是唯一允许高于语言包的覆盖来源。

## 明确不支持

- 应用运行后下载 NuGet、XLIFF 或任意语言文件。
- `.atomlang` 文件。
- 第三方语言程序集、插件目录或 AssemblyLoadContext 加载。
- 运行时修改 Catalog、追加资源或刷新翻译目录。

未来如果确有动态语言需求，必须设计独立显式加载协议、签名和二进制格式，不能复用这里的静态 XLIFF
构建协议，也不能预留一个当前没有完整安全语义的半成品 API。
