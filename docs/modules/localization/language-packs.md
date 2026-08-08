# AtomUI 静态语言包架构

本文定义 AtomUI 官方和第三方如何发布、命名、构建和消费附加翻译包。语言包只参与应用构建，不是运行时插件。

## 内置语言

AtomUI 官方组件内置以下语言：

```text
en-US
zh-CN
zh-TW
```

三种翻译随拥有 Catalog 的组件程序集发布，不拆分为独立 I18n NuGet。当前由 `AtomUI.Controls`、
`AtomUI.Desktop.Controls`、`AtomUI.Desktop.Controls.DataGrid` 和 `AtomUI.Desktop.Controls.ColorPicker` 各自编译并
注册自己拥有的 Catalog，不由一个中央程序集复制所有翻译。Extras 等模块只有在实际拥有 Catalog 后才进入语言包
发布集合，不预先创建空包。

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
| `pt-BR` | `PtBR` |
| `ja-JP` | `JaJP` |
| `ko-KR` | `KoKR` |
| `zh-Hant` | `ZhHant` |
| `zh-Hant-HK` | `ZhHantHK` |
| `sr-Latn-RS` | `SrLatnRS` |

NuGet package ID 不能充当语言身份解析来源。包内必须显式包含：

```xml
<AtomUILanguageTag>pt-BR</AtomUILanguageTag>
```

## 官方聚合包

产品级官方附加翻译使用：

```text
AtomUI.I18n.PtBR
AtomUI.I18n.JaJP
AtomUI.I18n.KoKR
AtomUI.I18n.FrFR
AtomUI.I18n.DeDE
```

`AtomUI.I18n.PtBR` 表示 AtomUI 官方组件体系的巴西葡萄牙语聚合包。聚合包必须是纯依赖 Meta Package：

- 不设置 `AtomUIBuildLanguagePackage=true`，也不运行语言包 Prepare/Props 任务。
- 不包含 XLIFF、`AtomUI.LanguagePack.xml`、`buildTransitive` props、analyzer、运行时 DLL 或其他文件 payload。
- 项目中的模块语言包 `ProjectReference` 只提供源码仓库内的构建顺序，不定义最终 NuGet 依赖版本。
- 使用无文件 payload 的自定义 nuspec 作为聚合包依赖图的唯一权威来源；每个官方模块语言包依赖都写成
  `[$version$]`，与聚合包版本精确一致。
- 不依赖任何组件运行时包，也不隐式注册组件、主题或 Catalog。

`AtomUI.I18n.PtBR` 的首个官方发布集合为：

```text
AtomUI.I18n.PtBR
├── AtomUI.Controls.I18n.PtBR
├── AtomUI.Desktop.Controls.I18n.PtBR
├── AtomUI.Desktop.Controls.DataGrid.I18n.PtBR
└── AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR
```

只有模块包携带 Translation Bundle。聚合包不复制翻译，因此聚合引用和显式模块包引用最终解析到同一个 NuGet
package identity，不会形成两个同优先级翻译来源。未来模块首次拥有 Catalog 时，先发布相应模块语言包，再在同一
AtomUI 版本的聚合包中增加精确依赖。

普通用户优先只引用一个聚合包：

```xml
<PackageReference Include="AtomUI.I18n.PtBR" Version="$(AtomUIVersion)" />
```

## 模块级包

模块语言包是真正的翻译载体，名称必须包含目标 Language Module：

```text
AtomUI.Controls.I18n.PtBR
AtomUI.Desktop.Controls.I18n.PtBR
AtomUI.Desktop.Controls.DataGrid.I18n.PtBR
AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR
```

每个模块包只包含该模块当前拥有的 Catalog。制作项目可以通过作者期 `PackageReference` 或仓库内
`ProjectReference` 取得权威 `en-US` 模板，但该引用不是第三方语言包能够构建和发布的前置条件。组件引用存在时，
Build Tasks 在打包阶段完成完整契约校验；组件引用不存在时，打包降级为延迟契约校验，并输出一次带修复建议的
warning。两种模式生成的 I18n NuGet 都不得依赖目标组件运行时包。应用仍需按自身功能引用组件包；聚合语言包不会
为了翻译而把未使用的 DataGrid 或 ColorPicker DLL 带入应用。

仓库内语言包的组件 `ProjectReference` 使用普通 `Include` 即可。标准 localization targets 会自动把非 Analyzer
项目引用配置为作者期契约输入：不参与程序集引用和常规引用构建，跳过 TFM 协商，也不形成语言包的运行时依赖；
构建系统仍会显式调用组件项目的契约资产 target 读取权威 `en-US`。语言包项目不得手写 `BuildReference`、
`SkipGetTargetFrameworkProperties` 或 AtomUI 专用识别 metadata。

语言包项目仍必须在项目级声明唯一 `AtomUILanguageModuleId`。它用于没有安装目标组件时可靠地把静态输入分类为
dormant，不是对每个 XLIFF 重复维护的 Catalog metadata。`AtomUILanguageContractVersion`、Catalog ID 和源
fingerprint 不要求第三方作者手写；有权威源契约时由构建系统绑定，没有时由目标 XLIFF 和消费应用中的真实 Catalog
完成延迟绑定。

### 契约校验级别

每个语言包 Catalog 资产具有以下校验级别，并通过 `AtomUILanguageContractValidation` 写入审计 manifest 和
`buildTransitive` item metadata：

| Level | 打包时输入 | 打包时保证 | 消费时行为 |
|---|---|---|---|
| `Verified` | 存在目标组件发布的权威 `en-US` 契约 | 校验 Catalog 集合、Key 完整性、source、占位符、ContractVersion 和 fingerprint | 再次与消费应用实际引用的 Catalog 校验，防止错误版本组合 |
| `Deferred` | 没有权威组件契约 | 校验 XLIFF 2.1、语言标签、目标状态、重复 Key、source/target 占位符、module ID 和包结构 | 目标模块 active 后根据真实 Catalog 完成全部契约校验；未安装模块保持 dormant |

同一目标模块只允许整体 `Verified` 或整体 `Deferred`。如果已经发现该模块的任何权威源资产，则所有目标 Catalog 都
必须与权威集合完整匹配；文件名、`file id` 或 Key 写错不能通过把单个文件降级为 `Deferred` 来隐藏。只有完全没有
发现该模块的权威源资产时，才允许整个模块包进入 `Deferred`。

默认允许 `Deferred`，并由 MSBuild 输出一次 `ATOMUILOC010` warning，明确说明当前没有执行完整契约校验，并建议
添加作者期 `PrivateAssets="all"` 组件包引用。warning 不阻止社区作者独立发布。AtomUI 官方语言包设置
`AtomUIRequireVerifiedLanguageContract=true`；此时任何 `Deferred` 资产都使打包失败。

生成的 props 形态为：

```xml
<AtomUILanguage
    Include=".../Localization/Dialog/pt-BR.xlf"
    AtomUILanguageSourceKind="StaticLanguagePack"
    AtomUILanguageSourceIdentity="Acme.AtomUI.Desktop.Controls.I18n.PtBR"
    AtomUILanguageModuleId="AtomUI.Desktop.Controls"
    AtomUILanguageContractValidation="Deferred"
    AtomUILanguagePackagePath="Localization/Dialog/pt-BR.xlf"
    AtomUILanguageSourceFingerprint="..." />
```

`Verified` item 在此基础上增加 `AtomUILanguageContractVersion`。manifest 的对应 `catalog` 节点使用
`contractValidation="Verified|Deferred"`；`Deferred` 节点省略 `contractVersion`，不能写入 `0`、默认值 `1` 或作者
猜测的版本。

## 未引用模块与 dormant 输入

聚合包会传递所有模块语言包的 `buildTransitive` 输入，但消费应用不一定引用所有组件模块。Generator 对
`StaticLanguagePack` 使用以下激活规则：

1. 所有输入都先完成 XLIFF 2.1 结构、语言标签和必需 item metadata 校验；格式损坏的文件不能进入 dormant。
2. 当前 Compilation 已声明目标 Catalog，或引用程序集存在相同 `AtomUILanguageModuleId`，或 XLIFF `file id` 能解析
   到 Catalog enum 时，该输入为 active，必须完成全部 Catalog、ContractVersion、unit、源文本和 fingerprint 校验。
3. 基础校验通过后，目标 module ID 不存在且 `file id` 也无法解析的输入为 dormant：不生成 Bundle、不参与冲突和
   覆盖计算，也不报告“referenced Catalog could not be found”。
4. 模块存在时，`Verified` 输入校验声明的 ContractVersion，`Deferred` 输入从实际 Catalog 绑定 ContractVersion；
   两者都必须完成 Catalog、unit、source、占位符和 fingerprint 校验。Catalog 缺失、identity 错误或版本不兼容时
   仍然构建失败，不能用 dormant 或 `Deferred` 隐藏损坏的语言包。
5. dormant 只适用于 NuGet 提供的 `StaticLanguagePack`。ModuleBuiltIn、项目 XLIFF 和应用 Override 指向不存在的
   Catalog 时仍然报错。

因此应用可以始终引用 `AtomUI.I18n.PtBR`。未引用 DataGrid 时 DataGrid 翻译保持 dormant；以后增加 DataGrid 组件
引用后，同一语言包输入会在下一次编译自动激活。运行时仍只为实际注册的 Catalog 构建 Snapshot。

## 官方源码组织

官方附加语言按规范 BCP 47 标签集中维护，每个 NuGet 仍拥有独立项目和 XLIFF：

```text
src/LanguagePacks/pt-BR/
├── AtomUI.Controls.I18n.PtBR/
│   └── Localization/Common/pt-BR.xlf
├── AtomUI.Desktop.Controls.I18n.PtBR/
│   └── Localization/<Control>/pt-BR.xlf
├── AtomUI.Desktop.Controls.DataGrid.I18n.PtBR/
│   └── Localization/pt-BR.xlf
├── AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR/
│   └── Localization/pt-BR.xlf
└── AtomUI.I18n.PtBR/
    └── AtomUI.I18n.PtBR.csproj
```

语言优先的目录使同一译者可以在一个边界内完成审校，也避免以后新增语言时把大量 I18n 项目散落到 `src/` 根目录。
Catalog 的真实身份仍来自 module metadata 和 XLIFF `file id`，不能从目录推断。

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

第三方产品若以一个包聚合多个模块语言包，也必须使用相同的纯依赖 Meta Package 语义；不得同时在聚合包和模块包
中发布同一 Catalog 的 XLIFF。

## NuGet 内容协议

模块级静态语言包是 content/build assets 包：

```text
{TranslationScope}.I18n.PtBR.nupkg
├── contentFiles/any/any/AtomUI.LanguagePack.xml
├── contentFiles/any/any/<CatalogPath>/pt-BR.xlf
└── buildTransitive/{PackageId}.props
```

实际 `.nupkg` 文件名由 NuGet 自动加入包版本。包设置 `IncludeBuildOutput=false`，不得包含运行时翻译 DLL。

`AtomUI.LanguagePack.xml` 是由打包任务根据 XLIFF 确定性生成的审计和工具产物，至少记录：

- package identity 和规范目标 `LanguageTag`。
- 每个目标 Catalog 的 module ID、Catalog metadata ID、契约校验级别和包内路径。
- `Verified` Catalog 的 ContractVersion；`Deferred` Catalog 省略该字段。
- 每个目标 Catalog 的规范源文本指纹。

`buildTransitive/<PackageId>.props` 是编译权威入口。它只把每个 XLIFF 作为带 source kind、source identity、
module ID、契约校验级别、包内路径和源 fingerprint 的 `AtomUILanguage` item 注入；`Verified` 资产另外携带绑定后的
ContractVersion，`Deferred` 资产不得伪造 ContractVersion。标准 targets 再将这些 XLIFF 加入 `AdditionalFiles`。
manifest 不进入 `AdditionalFiles`，Generator 也不会独立发现或读取它。最终应用 Generator 校验 metadata、实际
XLIFF 和引用 Catalog 后把翻译编译进应用程序集；运行时不需要知道翻译来自哪个 NuGet 文件。

聚合包使用普通 SDK-style pack 项目，设置 `IncludeBuildOutput=false`，并保留指向模块语言包项目的
`ProjectReference` 作为仓库构建顺序边。由于 .NET SDK pack 会把普通 `ProjectReference` 版本序列化为最低版本范围，
聚合包必须通过无文件 payload 的自定义 nuspec 声明每个 `[$version$]` 精确依赖；该 nuspec 是最终依赖图的权威来源。
聚合包不使用本节的静态语言包内容协议，也不得包含 XLIFF、manifest、props、analyzer、DLL、runtime asset 或组件包
依赖。NuGet package ID 仍只是分发 identity，不能作为编译期语言或 Catalog identity。

## 官方翻译与发布门禁

官方语言包从对应模块发布的权威 `en-US.xlf` 导出，XLIFF 必须保存 `srcLang="en-US"`、`trgLang="pt-BR"`、原始
`file id` 和 enum Key。机器预翻译可以作为初稿，但不能自动获得官方发布状态。

通用语言包默认接受 `translated`、`reviewed` 或 `final`。官方包项目必须设置
`AtomUILanguageMinimumState=final`，要求所有非 obsolete unit 经过人工审校并达到 `final`；源文本变化、
`subState="needs-review"`、占位符变化或缺少 target 都会阻止模块包发布。只有发布集合中的全部模块包通过门禁后，
才允许生成并发布同版本聚合包。

## 语言包制作模板

不提供 `AtomUI.Localization.Tool` CLI。保留标准 `dotnet new` 项目模板：

```bash
dotnet new install AtomUI.LanguagePack.Template

dotnet new atomui-language-pack \
  --name Acme.AtomUI.Desktop.Controls.I18n.PtBR \
  --packageId Acme.AtomUI.Desktop.Controls.I18n.PtBR \
  --moduleId AtomUI.Desktop.Controls \
  --languageTag pt-BR \
  --languageTagIdentifier PtBR
```

模板生成：

- 不产出 DLL 的 SDK-style pack 项目。
- `AtomUILanguageTag`、唯一项目级 `AtomUILanguageModuleId`、PackageId 和 package metadata。
- `AtomUI.Generator` 构建期 PackageReference。
- 自动扫描 `Localization/**/*.xlf`，不要求作者逐文件维护 ContractVersion、package path 或 fingerprint。
- build/pack 校验 target 和最小翻译说明。

模板不复制 AtomUI 当前 Catalog。作者可以直接维护已有 XLIFF，并以 `Deferred` 模式独立打包。推荐添加作者期组件
NuGet 引用，以便导出准确模板并在打包时获得 `Verified`：

```xml
<PackageReference Include="AtomUI.Desktop.Controls"
                  Version="6.0.0"
                  PrivateAssets="all" />
```

`PrivateAssets="all"` 只阻止该作者期依赖传递到语言包消费者，不阻止当前项目读取组件包发布的
`buildTransitive` 契约资产。语言包模板命令应根据 `--module` 和 `--atomui-version` 自动生成该引用，作者不需要手写
路径、Catalog 或 ContractVersion。

存在组件契约时，作者可运行：

```bash
dotnet msbuild \
  -t:AtomUIExportLanguageTemplates \
  -p:AtomUITargetLanguage=pt-BR

dotnet build
dotnet pack
```

没有组件引用时，`AtomUIExportLanguageTemplates` 没有权威输入可导出，必须输出可操作提示；这不影响作者对已有 XLIFF
执行 `dotnet build` 或 `dotnet pack`。打包产生 `ATOMUILOC010` warning，并将资产标记为 `Deferred`。

## 第三方组件作者职责

希望别人制作语言包的组件或类库必须：

1. 使用 `[LanguageCatalog]`，并以 enum 成员名作为稳定 unit Key，不声明显式数字值。
2. 提供完整 `en-US`，并声明自身内置语言。
3. 引用 `AtomUI.Generator`；模块主包由标准 targets 自动发布 `en-US` 与声明式 props 构建资产。
4. 保持 Catalog ID、ContractVersion 和源文本指纹可追踪。
5. 不提供运行时反射 Provider、手写 LanguagePack Descriptor 或自定义 DLL 加载入口。

第三方类库也可以在自身主包内置多种语言；只有需要让翻译独立发布、独立维护或由社区提供时才创建 I18n 包。

## 应用消费

应用只添加 PackageReference，并继续通过统一入口声明支持语言：

```csharp
builder.UseLanguages(
    defaultLanguage: LanguageTags.PtBR,
    supportedLanguages:
    [
        LanguageTags.EnUS,
        LanguageTags.PtBR
    ]);
```

```xml
<PackageReference Include="AtomUI.I18n.PtBR" Version="$(AtomUIVersion)" />
```

还必须正常注册目标组件模块，例如 `builder.UseDesktopControls()`。语言包只提供 Translation Bundle，不会隐式
注册组件、主题或 Catalog。应用 Generator 从实际引用的模块程序集解析强类型 Catalog，把 active 的目标 XLIFF 编译
进应用程序集，并忽略未安装模块的 dormant 输入。

不存在以下 API：

```csharp
builder.LoadLanguagePack(...);
builder.AddLanguagePackDescriptor(...);
builder.UseAtomUIPtBR();
```

## Gallery 应用级翻译

AtomUIGallery 的页面、导航和示例文案属于应用 Catalog，不进入任何 `AtomUI.*.I18n.PtBR` 公共包。Gallery 在现有
Catalog 目录中维护自己的 `pt-BR.xlf`，同时消费官方聚合包取得控件翻译。仓库内开发可以直接导入模块语言包项目的
同一组 XLIFF；NuGet 集成测试必须从临时 feed 消费真实 nupkg，不能复制另一份控件译文。

Gallery 支持语言配置加入 `LanguageTags.PtBR`。系统语言自动选择只将规范 `pt-BR` 和中性 `pt` 映射到巴西
葡萄牙语；`pt-PT` 不自动映射为 `pt-BR`。格式化使用标准 `CultureInfo("pt-BR")` 和从语言数据生成的 LTR 定义，
不得在译文中手工模拟日期、数字或货币格式。

仓库源码构建时，`AtomUIGallery` 类库继续拥有 Gallery 应用 Catalog 和对应 `pt-BR.xlf`，但四个官方模块语言包的
`AtomUILanguagePackProjectReference` 必须由 `AtomUIGallery.Desktop`、`AtomUIGallery.Browser` 和执行完整
Localization Snapshot 的测试宿主直接声明。宿主项目还必须以 Analyzer 方式引用 `AtomUI.Generator`，宿主
`Application` 类型必须声明为 `partial`，以便生成并调用应用语言 Bootstrap。

## 冲突与兼容

- `Verified` props metadata、XLIFF `file id` 或引用 Catalog identity 不一致时构建失败。
- `Verified` Catalog ContractVersion 不兼容时构建失败；`Deferred` 在目标模块 active 后绑定并校验消费应用实际
  ContractVersion。
- props 中的源 fingerprint 必须是当前 XLIFF 源契约的 64 位小写 SHA-256；目标源文本还必须与权威 `en-US` 一致。
- 两个来源提供相同 Catalog/语言且优先级相同时构建失败。
- 语言包缺少目标 Catalog 的新增 unit 时视为覆盖不完整。
- 未安装模块的 dormant 静态输入不参与上述冲突和覆盖判断；模块出现后立即恢复全部严格校验。
- 官方聚合包与显式模块包必须解析到同一个精确 package identity；聚合包不得携带翻译副本。
- 应用显式 Override 是唯一允许高于语言包的覆盖来源。

## 明确不支持

- 应用运行后下载 NuGet、XLIFF 或任意语言文件。
- `.atomlang` 文件。
- 第三方语言程序集、插件目录或 AssemblyLoadContext 加载。
- 运行时修改 Catalog、追加资源或刷新翻译目录。

动态语言加载不属于当前协议；不得复用静态 XLIFF 构建链或为其预留缺少完整安全语义的 API。
