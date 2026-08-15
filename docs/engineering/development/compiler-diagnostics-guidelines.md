# AtomUI 编译期诊断规范

本文档定义 AtomUI 编译期 warning / error 的统一维护规则，适用于 Roslyn analyzer、source generator diagnostic、AOT 兼容诊断，以及后续 AXAML、Theme、Token 等可在编译期发现的问题。

编译期诊断的目标是把可静态证明的问题尽早暴露给开发者。诊断不是运行时异常说明，也不是一次性 bug 修复记录。

## 适用范围

新增或修改以下能力时必须遵循本文档：

- `DiagnosticAnalyzer` 报告的 warning / error。
- source generator 通过 `ReportDiagnostic` 报告的问题。
- AOT、trimming、动态成员访问、字符串 path 等编译期兼容性检查。
- 后续 AXAML、Theme、Token、资源注册等静态检查。

专项文档只描述领域背景和触发场景；诊断 ID、命名、severity、编码组织、测试要求统一维护在本文档中。

## 诊断 ID 规则

AtomUI 诊断 ID 使用固定格式：

```text
ATOMUI<DOMAIN><NNN>
```

示例：

```text
ATOMUIAOT001
ATOMUIGEN001
ATOMUIXAML001
ATOMUITOKEN001
```

规则：

- `ATOMUI` 是固定项目前缀。
- `DOMAIN` 使用大写英文领域名，不加下划线、点号或连字符。
- `NNN` 使用三位数字，从 `001` 开始递增。
- ID 一旦发布，不能复用，不能改变语义。
- 诊断废弃时在注册表中标记 deprecated，不把旧 ID 分配给新问题。

.NET / Roslyn 生态通常使用 `CS8602`、`CA1852`、`IDE0005`、`IL2026`、`SYSLIB0011`、`RS2008` 这类“字母前缀 + 数字”格式。AtomUI 也采用这种格式，避免 `ATOMUI_AOT_001`、`ATOMUI.AOT.001`、`ATOMUI-AOT-001`。

这种格式更适合 `.editorconfig` 和 `#pragma`：

```ini
dotnet_diagnostic.ATOMUIAOT001.severity = error
```

```csharp
#pragma warning disable ATOMUIAOT001
```

## 领域前缀

已保留的领域前缀：

| Prefix | 领域 | 用途 |
|---|---|---|
| `AOT` | AOT / trimming | NativeAOT、trimming、动态成员访问、字符串 path |
| `GEN` | Source generator | generator 输入、生成失败、生成物一致性 |
| `XAML` | AXAML | AXAML 静态结构、绑定、资源引用 |
| `TOKEN` | Theme / Token | Token 定义、注册、资源键、主题约束 |
| `LOC` | Localization | Language Catalog、XLIFF、语言元数据和静态语言包 |

新增领域前缀前必须先更新本文档，说明用途和 owner。

## 当前诊断注册表

| ID | Category | Severity | Trigger | Fix | Owner |
|---|---|---|---|---|---|
| `ATOMUIAOT001` | AOT | Warning | 已知 data member path 的 model type，但找不到 generated accessor | 给类型、接口或基类添加 `[GenerateDataMemberAccessors]`，或显式传入 `IDataMemberAccessorDescriptor` | DataMemberAccessors |
| `ATOMUIAOT002` | AOT | Warning | model type 有 generated accessor，但目标 path 不会被生成 | 改成可访问实例属性、修正 path，或显式传入 descriptor | DataMemberAccessors |
| `ATOMUIAOT003` | AOT | Warning | 使用字符串常量 path，编译期无法确认 model type | 使用 `nameof(Type.Property)`，或显式传入 descriptor | DataMemberAccessors |
| `ATOMUIGEN001` | Generator | Warning | `[GenerateScopedResourceHost]` 标记的类型不是非泛型 partial 顶层 class | 改为非泛型 `partial class`，或移除该 attribute | ScopedResourceHost |
| `ATOMUIGEN002` | Generator | Warning | `[GenerateScopedResourceHost]` 标记的类型未继承 `AvaloniaObject` | 只在非 Visual `AvaloniaObject` 描述对象上使用该 attribute | ScopedResourceHost |
| `ATOMUIGEN003` | Generator | Warning | `[GenerateScopedResourceHost]` 标记的类型继承了 `Control`、`StyledElement` 或 `Visual` | Visual 控件应使用视觉树资源宿主，不使用该 attribute | ScopedResourceHost |
| `ATOMUIGEN004` | Generator | Warning | `[GenerateScopedResourceHost]` 标记的类型已经实现 `IResourceHost` 或 `IThemeVariantHost` | 删除手写实现后再使用该 attribute，或移除该 attribute | ScopedResourceHost |
| `ATOMUIGEN101` | Generator | Warning | Gallery source code display generator 发现参与默认源码匹配的 `ShowCasePanel` 缺少 `Name` | 给 `ShowCasePanel` 设置稳定 `Name`，或使用显式源码 key 规则 | GallerySourceCodeDisplay |
| `ATOMUILOC001` | Localization | Error | AtomUI 固定语言数据记录的 schema、标识符、BCP 47 标签或元数据无效 | 按数据 schema 修正发生错误的具体记录 | LanguageTags |
| `ATOMUILOC002` | Localization | Error | AtomUI 固定语言数据包含重复的属性标识符或规范 BCP 47 标签 | 删除重复记录并为每个属性和标签保留唯一映射 | LanguageTags |
| `ATOMUILOC003` | Localization | Error | `[LanguageCatalog]` 声明不是可生成的稳定 enum 契约 | 使用 public 非泛型 enum、至少一个隐式顺序资源项且不使用 `[Flags]` | LocalizationGenerator |
| `ATOMUILOC004` | Localization | Error | Catalog 成员缺少显式正整数 ID，或 ID 重复、越界 | 为每个成员分配唯一且永久保留的显式正 `Int32` ID | LocalizationGenerator |
| `ATOMUILOC005` | Localization | Error | XLIFF 文档不符合 AtomUI 支持的 XLIFF 2.1 结构或语言元数据规则 | 修正 XML namespace、版本、语言标签、file/unit/segment 结构或翻译状态 | LocalizationGenerator |
| `ATOMUILOC006` | Localization | Error | XLIFF 的 Catalog identity、unit 或源契约与代码声明不匹配 | 使用目标 Catalog 导出的模板同步 file ID、unit Key、源文本、占位符和 fingerprint | LocalizationGenerator |
| `ATOMUILOC007` | Localization | Error | 翻译文本不可发布或 CompositeFormat 参数契约无效 | 提供 translated target，并保持源/目标占位符索引与格式语法一致 | LocalizationGenerator |
| `ATOMUILOC008` | Localization | Error | 应用类型无法实现生成式语言 bootstrap | 保留唯一的非抽象 partial Avalonia Application host，或移除应用级语言输入 | LocalizationGenerator |
| `ATOMUILOC009` | Localization | Error | 静态语言包包含运行时代码/二进制、非法路径、缺失必需 metadata、未满足 Verified 要求或混合目标语言 | 删除运行时资产，并使用模板生成的声明式 contentFiles/buildTransitive 包结构 | AtomUI.Build.Tasks |
| `ATOMUILOC010` | Localization | Warning | 静态语言包没有取得目标模块的权威 `en-US` 契约，打包只能执行延迟契约校验 | 添加作者期 `PrivateAssets=all` 组件 PackageReference；社区包也可保留 Deferred 并由消费应用完成完整校验 | AtomUI.Build.Tasks |

`ATOMUIGEN005` 和 `ATOMUIGEN006` 原本约束 `[ControlDesignToken]` 类型上的 `public const ID`，该手工 ID 契约已
删除，因此这两个诊断不在新主题架构中复用。无参数 `[ControlDesignToken]` 本身继续保留，只负责标记 Own Token
类型，不携带 Control 类型或 identity。Control identity 由可主题化 Control 和主题资产约定生成；相关歧义、Token
继承、Global/Own 重名、资产依赖和注册错误应使用新的独立诊断 ID。

## Severity 规则

- 默认使用 `Warning`。
- 只有生成结果必然错误、编译产物不可用、或继续编译会掩盖严重错误时才使用 `Error`。
- 对迁移期可能大量触发的问题，先使用 `Warning`，再由具体项目通过 `.editorconfig` 升级为 `Error`。
- 不要用 analyzer warning 替代运行时 guard。编译期无法证明安全时，运行时仍要有明确异常或 fallback 边界。

## 代码组织

公共诊断定义集中放在：

```text
src/AtomUI.Generator/Diagnostics/
├── AtomUIDiagnosticIds.cs
├── AtomUIDiagnosticCategories.cs
└── AtomUIDiagnosticDescriptors.cs
```

规则：

- diagnostic ID 必须来自 `AtomUIDiagnosticIds`。
- category 必须来自 `AtomUIDiagnosticCategories`。
- `DiagnosticDescriptor` 优先定义在 `AtomUIDiagnosticDescriptors`。
- analyzer 可以放在对应领域目录，例如 `DataMemberAccessors/`、`DesignToken/`、`Language/`。
- analyzer 不要在业务文件中散落硬编码 ID、category 或重复 descriptor。

## Descriptor 编码规则

每个 `DiagnosticDescriptor` 必须满足：

- `Id` 使用 `AtomUIDiagnosticIds` 常量。
- `Category` 使用 `AtomUIDiagnosticCategories` 常量。
- `Title` 使用英文短句，不加句号。
- `MessageFormat` 使用英文，必须包含触发对象，并给出修复方向。
- `DefaultSeverity` 按本文档 severity 规则选择。
- `isEnabledByDefault` 默认 `true`。
- `Location` 指向最小可修复语法节点，例如 path、attribute、类型名，而不是整个文件或整个 class。
- 一个 diagnostic 只表达一个根因，不把多个修复方向混到一个 ID 中。

## Analyzer 编码规则

- 只报告可以静态证明的问题；无法证明时可以报告“不可验证”类诊断，但不能编造具体类型或 path。
- 避免对 generated code 报告诊断，除非诊断专门检查 generated code。
- 不要在 analyzer 中执行昂贵的全编译扫描；优先注册具体 syntax / symbol action。
- 不要依赖源码字符串匹配判断语义；使用 `SemanticModel`、`ISymbol`、`ITypeSymbol` 等 Roslyn API。
- 诊断位置要落在用户需要修改的表达式上。
- analyzer 自身不能引入运行时依赖，必须保持可作为 analyzer 包加载。

## 测试规则

每个新增 diagnostic 必须包含：

- 正例：应报告，并断言 ID、数量和关键 message。
- 反例：正确写法不报告。
- 边界：无法静态证明时不误报，或按设计报告“不可验证”类诊断。
- 如果诊断与 AOT 相关，至少覆盖 generated accessor、显式 descriptor 或等价修复路径中的一种。

测试应放在对应 generator 测试项目中：

```text
tests/AtomUI.Generator.Tests/
```

测试中如果故意验证 fallback 或错误路径，需要用局部 `#pragma warning disable <ID>` 标明意图，不要在项目级别全局 suppress。

## 文档要求

新增 diagnostic 时必须同步更新：

- 本文档的诊断注册表。
- 对应领域文档，例如 AOT 规则写到 [aot-programming-guidelines.md](aot-programming-guidelines.md)。
- 如果诊断需要用户迁移，补充修复示例。
