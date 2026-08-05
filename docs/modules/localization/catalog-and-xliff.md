# Language Catalog 与 XLIFF 规范

本文定义 AtomUI 应用、组件和第三方类库如何声明本地化契约、组织翻译源文件，以及构建系统如何判断
Catalog 和翻译是否兼容。运行时不读取本文描述的 XLIFF 文件。

## Catalog 是本地化契约

Language Catalog 是由稳定 enum 定义的本地化单元。它可以属于应用公共区域、功能模块、页面、ViewModel、
服务、控件或可复用类库，不使用“一个控件对应一个 Catalog”作为架构限制。

```csharp
namespace MyApplication.Features.Login.Localization;

[LanguageCatalog(ContractVersion = 1)]
public enum LoginLangResourceKind
{
    Title = 1,
    UserName = 2,
    Password = 3,
    SignIn = 4
}
```

Catalog enum 遵守以下规则：

- Catalog enum 及其所有 containing type 必须是 `public`，使生成的 public XAML Markup Extension 与翻译契约
  可以被消费程序集稳定引用。
- 所有成员必须显式指定正整数 ID；禁止依赖 enum 自动递增。
- ID 在 Catalog 生命周期内唯一且稳定，删除后永不复用。
- 成员名必须是合法、稳定、能表达语义的 C# 标识符，不编码具体语言文本。
- 重命名成员时数字 ID 保持不变；Build Tasks 把它识别为名称变更并保留翻译历史。
- 同一个 ID 改变业务语义属于不兼容变更，不能伪装成普通源文本修订。
- `[Flags]`、别名成员、负数、零和重复数值均不允许。

## Catalog identity

Catalog ID 由构建系统生成，不作为 Attribute 字符串参数交给开发者：

```text
{LanguageModuleId}:{FullyQualifiedMetadataName}
```

`LanguageModuleId` 默认取项目的 `PackageId`，非打包应用回退到 `AssemblyName`；两者都由 MSBuild 提供给
Generator。完整类型名使用 CLR metadata name，而不是文件路径或 XAML namespace。库作者移动 Catalog 类型、
更改包身份或拆分程序集时，必须把它作为公开翻译契约变更处理。

`ContractVersion` 是 Catalog 结构兼容代数。删除 ID、复用 ID、改变 ID 语义或改变格式化参数契约时必须递增；
只修改源文案措辞时由源文本指纹识别，不要求用结构版本掩盖内容变化。

## 源语言

所有 Catalog 的源语言固定为 `en-US`。`LanguageCatalogAttribute` 不接受 `LanguageTag`，原因包括：

- Attribute 参数不能使用 `LanguageTag` 值类型。
- 允许每个 Catalog 自定义源语言会使第三方语言包导出、回退和完整性校验失去统一基线。
- 应用可以不把 `en-US` 暴露为可选择语言，但仍必须携带完整 `en-US` 源文本作为最终安全回退。

## 目录约定

一个 Localization 目录只拥有一个 Catalog：

```text
MyApplication/
├── Localization/
│   └── AppCommon/
│       ├── AppCommonLangResourceKind.cs
│       ├── en-US.xlf
│       └── zh-CN.xlf
└── Features/
    └── Login/
        └── Localization/
            └── Login/
                ├── LoginLangResourceKind.cs
                ├── en-US.xlf
                ├── zh-CN.xlf
                └── zh-TW.xlf
```

Generator 通过同目录 Catalog enum、XLIFF `file` identity 和项目 module identity 交叉确认归属。禁止仅根据
文件名猜测 Catalog，也禁止在同一目录放置两个 `[LanguageCatalog]` enum。

组件包采用相同结构：

```text
DatePicker/Localization/
└── DatePicker/
    ├── DatePickerLangResourceKind.cs
    ├── en-US.xlf
    ├── zh-CN.xlf
    └── zh-TW.xlf
```

## XLIFF 2.1

XLIFF 2.1 是唯一翻译交换源格式。源文件使用 `srcLang="en-US"`，目标文件同时声明规范 BCP 47 `trgLang`：

```xml
<?xml version="1.0" encoding="utf-8"?>
<xliff xmlns="urn:oasis:names:tc:xliff:document:2.0"
       version="2.1"
       srcLang="en-US"
       trgLang="zh-CN">
  <file id="MyApplication.Features.Login.Localization.LoginLangResourceKind">
    <unit id="1" name="Title">
      <segment>
        <source>Sign in</source>
        <target state="translated">登录</target>
      </segment>
    </unit>
  </file>
</xliff>
```

对应关系固定为：

| XLIFF | Catalog |
|---|---|
| `file id` | Catalog enum 的完整 metadata name；完整 Catalog ID 还包含构建提供的 module identity |
| `unit id` | enum 显式数字值，是资源项的长期身份 |
| `unit name` | enum 成员名，服务可读性、重命名和工具提示 |
| `source` | 当前 `en-US` 源文本 |
| `target` | `trgLang` 对应翻译 |

运行时只按生成的 Catalog slot 和数字 ID 读取，不按 `unit name` 或源文本查找。

## 翻译状态与备注

Build Tasks 必须保留 XLIFF 的 `target state`、translator notes 和源文本修订信息。以下目标不能视为可发布翻译：

- 缺少 `target`。
- `state` 为 `initial`，或项目 `subState` 明确表示 needs-translation/需要复核。
- 目标内容在规范化后为空，而源项又不允许空字符串。
- 源文本指纹已经变化且目标仍绑定旧源文本。

模板合并不能删除译者备注或把需要复核的目标自动标记为 translated。

## 格式化消息

普通资源是纯文本。带参数的资源采用 .NET `CompositeFormat` 兼容占位符，并通过 `ILocalizer.Format()` 使用：

```xml
<source>{0} items selected</source>
<target>已选择 {0} 项</target>
```

构建期对每个源/目标单元校验：

- 占位符索引集合相同。
- format component 的语法有效。
- 花括号转义有效。
- 目标不得新增源文本不存在的参数。

参数格式化使用当前 `LanguageState.FormattingCulture`。复数、性别和选择语义在没有明确统一消息语法前使用
独立稳定资源项表达，不能把某个第三方消息格式悄悄塞进 XLIFF 字符串。

## 支持语言与覆盖

应用只需要覆盖 `UseLanguages()` 声明的语言，不必支持 AtomUI 内置的全部语言。覆盖判断以最终注册的全部
Catalog 为范围，包括应用、AtomUI 控件、可选控件包和已注册第三方类库。

目标语言可以由精确标签或有效父级翻译满足；对于非英语目标，仅回退到最终 `en-US` 不算完整覆盖。
例如应用声明 `ja-JP`，但 DatePicker 只有 `en-US`，构建必须要求引入日语翻译或移除 `ja-JP` 支持声明。

## 翻译来源与显式覆盖

翻译值具有固定优先级：

```text
Application Override
Static I18n package
Module built-in translation
en-US source
```

应用自定义组件文案时使用显式 MSBuild item：

```xml
<ItemGroup>
  <AtomUILanguageOverride Include="Localization/Overrides/**/*.xlf" />
</ItemGroup>
```

普通应用翻译、类库内置翻译和语言包文件不得冒充 Override。同一优先级对相同 Catalog、语言和 unit 提供
多个目标时直接诊断为冲突，不采用文件顺序、PackageReference 顺序或最后写入获胜。

`Application Override` 可以只包含需要替换的 unit；生成的 Bundle 对其他 slot 保留 `null`，Snapshot 构建时继续按
静态语言包、模块内置翻译和 `en-US` 的固定顺序解析。模块内置 Bundle 与静态语言包 Bundle 必须保持完整，不能
利用部分 Bundle 隐藏支持语言覆盖缺口。

## Catalog 模板

可被外部翻译的类库 NuGet 必须发布生成式 `*.catalog.xlf` 模板和 Catalog manifest。模板包含源文本、数字 ID、
成员名、ContractVersion、源文本指纹和 translator notes，但不作为运行时资源加载。

应用内部 Catalog 默认不导出到 NuGet；应用若需要把翻译工作拆成独立仓库，可通过同一个 MSBuild 导出目标
生成受版本控制的模板，而不是复制内部 Generator 输出。
