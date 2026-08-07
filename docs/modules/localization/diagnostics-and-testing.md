# 本地化诊断与验证规范

本文定义多语言模块如何把错误放在正确阶段暴露，以及实现和发布必须具备的测试证据。诊断 ID 的具体注册仍
遵守 [AtomUI 编译期诊断规范](../../engineering/compiler-diagnostics-guidelines.md)。

## 诊断所有权

本地化领域使用 `LOC` 作为编译期诊断 domain，形成 `ATOMUILOCNNN`。`ATOMUILOC001` 到
`ATOMUILOC009` 已在全局诊断规范中登记；ID 发布后不复用、不改变语义。

诊断由三个阶段拥有：

| 阶段 | 适合发现的问题 | 输出 |
|---|---|---|
| Roslyn Generator/Analyzer | enum symbol、Application partial、XLIFF、AdditionalFiles metadata 与引用 Catalog 对应 | `ATOMUILOCNNN` |
| MSBuild Tasks | XLIFF 文档、模板导出、语言包内容和确定性 manifest/props 产物 | MSBuild error/warning |
| Runtime Builder | 动态构造的支持语言、最终模块注册集合、Culture 元数据、首帧前完整覆盖 | typed startup exception |

同一根因只由一个阶段提供主诊断。下游为了安全仍需 guard，但不能再次输出一串语义不同的重复错误。

## 必须阻止生成或构建的问题

以下问题会使生成结果不可靠或应用声明失真，默认按 Error 处理：

### LanguageTag 与配置

- XLIFF、生成的包审计 manifest 或 MSBuild metadata 中存在非法或非规范 BCP 47 标签。
- `defaultLanguage` 不在 `supportedLanguages`。
- 支持语言集合为空、包含无效标签、缺少可解析的 `LanguageDefinition`，或私有标签无法解析
  FormattingCulture/TextDirection。

`UseLanguages()` 中重复的支持语言不是错误；Builder 按首次出现顺序静默去重。对同一标签重复注册显式
`LanguageDefinition` 才是配置错误。

### Catalog

- `[LanguageCatalog]` 目标不是非泛型 enum。
- enum 成员声明显式数值、别名或 `[Flags]`。
- 同一 Language Module 中生成相同 Catalog ID。
- XLIFF `file id` 与 Catalog metadata name 不一致。
- XLIFF `unit id` 缺失、重复，或与 enum member symbol 的 Key 集合不一致。
- 缺少完整 `en-US` 源 Catalog，或源 Catalog 包含未知/遗漏 unit。

### Translation Bundle

- XLIFF 不是受支持的 2.1 文档，XML namespace、srcLang 或 trgLang 错误。
- target 缺失、未达到可发布状态、源文本已变化但仍标记 translated。
- `CompositeFormat` 无效，源/目标占位符集合不同。
- 同一优先级存在重复 Catalog/language/unit 翻译。
- 非英语支持语言的有效回退链只能到达 `en-US`。

### Language Pack

- XLIFF item 缺少 module ID、ContractVersion、规范包内路径或 64 位小写源 fingerprint。
- props metadata 与实际 XLIFF、引用程序集中的 Catalog identity/ContractVersion/unit/source text 不一致。
- 同一个 Catalog 和语言在同一优先级由多个包或文件提供。
- 包含运行时 DLL、`.atomlang`、初始化代码或非声明式加载 target。
- 官方语言包存在低于 `AtomUILanguageMinimumState=final` 的有效 unit，或任何需要重新审核的 target。

`AtomUI.LanguagePack.xml` 由打包任务根据同一组 XLIFF 确定性生成，用于审计和工具读取。它不作为 Generator 的
`AdditionalFile`，也不存在独立于 XLIFF/props 的“manifest 未声明 Catalog”编译契约。

未安装 Language Module 的 `StaticLanguagePack` 输入保持 dormant，不属于错误。只有当 module ID 和 XLIFF
`file id` 在当前 Compilation 中都无法关联目标模块时才允许 dormant；模块或 Catalog 一旦存在，identity、
ContractVersion、权威 `en-US`、unit 和 fingerprint 的任何不匹配仍按 Error 处理。项目 XLIFF 和应用 Override 不使用
该豁免。XLIFF 结构、BCP 47 标签和必需 AdditionalFiles metadata 在 dormant 分类前校验，不能因目标模块未安装而
忽略损坏或不可信的包输入。

## Warning 边界

Warning 只用于产物仍然确定可用、但维护质量可能下降的场景，例如：

- XLIFF 中存在已经标记 obsolete、当前生成不会使用的旧 unit。
- translator note 缺少推荐上下文。
- 应用显式 Override 的源文本指纹落后，但目标仍通过人工状态确认。

不能把缺少必需翻译降级为 Warning 后继续声称该语言受支持。

## 启动期异常

编译期无法知道最终配置时，Builder 在 Application 首帧前抛出领域异常：

| 异常 | 语义 |
|---|---|
| `LanguageConfigurationException` | 默认/支持语言、Culture 或 TextDirection 配置无效 |
| `LanguageCatalogException` | Catalog 注册重复、schema 不一致或必需源资源缺失 |
| `LanguageCoverageException` | 最终支持语言不能覆盖全部已注册 Catalog |

异常消息必须包含规范语言标签、Catalog ID、unit Key、来源包和可操作修复建议。禁止捕获后静默回退到英文
继续启动，因为这会违背应用的支持语言契约。

## 运行时错误语义

- `ChangeLanguage()` 收到未支持标签时抛出 `LanguageNotSupportedException`，不临时创建 Snapshot，也不发布失败事件。
- 查询已注册 enum 但找不到 unit 表示生成/Registry 不变量损坏，应记录错误并使用同 Catalog 的 `en-US` 值；
  `en-US` 也不存在时抛出不可恢复异常。
- 查询未注册 enum 类型不得调用 `ToString()` 返回伪翻译，应抛出包含 CLR 类型的 Catalog 异常。
- `Format()` 参数数量或类型导致格式化失败时保留原异常作为 inner exception，并报告 Catalog/unit 上下文。
- LanguageChanged 订阅方异常不回滚已经提交的 Snapshot；Manager 按统一事件异常策略隔离并记录订阅方失败。

## 单元测试

目标测试项目和覆盖面：

| 测试项目 | 必测内容 |
|---|---|
| `AtomUI.Localization.Tests` | BCP 47 规范化、CLDR 回退、Registry、Snapshot、优先级、切换、Culture、RTL、线程语义 |
| `AtomUI.Generator.Tests` | Catalog/Extension/registration/bootstrap 输出、诊断正反例、确定性与增量更新 |
| `AtomUI.Build.Tasks.Tests` | XLIFF reader/writer/merge、状态保留、manifest、props、冲突和模板导出 |
| `AtomUI.Localization.IntegrationTests` | 模块/静态语言包真实 pack 布局、临时 NuGet feed、Generator 消费和运行时切换 |
| 控件测试项目 | 现有 XAML Extension、控件 C# 查询、三种内置语言完整性 |
| `AtomUIGallery.Tests` | 应用 Catalog、语言菜单、ViewModel/导航刷新和支持语言配置 |

每个 Generator diagnostic 必须覆盖 ID、severity、最小 Location、message 关键字段、正确写法不报告和边界输入。
Generator 还必须覆盖 dormant/active 分类的增量测试，证明增加或移除组件引用只改变对应 Catalog 的生成结果，不使
无关模块 source 失效。

## 契约测试

以下测试保护跨包兼容性：

1. Catalog ID 在文件移动但类型/模块身份不变时保持稳定。
2. unit slot、fingerprint、manifest 和生成源码按 ordinal Key 确定，不受 XLIFF 文件顺序影响。
3. 公共 Catalog 的成员序列由契约基线保护，只允许末尾追加；重排或删除已有成员必须失败。
4. 新增 Key 会使旧语言包产生缺失诊断，已有译文和 notes 不丢失。
5. 重命名 Key 时旧 unit 标为 obsolete、新 unit 标为待翻译，并要求递增 ContractVersion。
6. 删除或复用 Key、改变格式化参数契约、使用旧 identity 模型或错误 ContractVersion 必须失败。
7. 静态语言包的 manifest 与 props 必须由同一组 XLIFF 确定性生成，并记录一致的 Catalog identity 和源指纹。
8. 模块主包必须包含权威 `en-US` 与 `<PackageId>.props`，静态语言包不得包含 DLL，Consumer 必须只靠 PackageReference 生效。
9. 纯聚合包不得包含 XLIFF、manifest、props、analyzer 或 DLL，只能精确依赖同语言、同版本的模块语言包。
10. Consumer 引用聚合包但未引用 DataGrid 等组件时构建成功，dormant 模块不产生 Bundle 或 Catalog 诊断。
11. Consumer 后续引用该组件时，同一静态输入自动激活；正确包生成 Bundle，错误 Catalog/module/contract 必须失败。
12. 同时显式引用聚合包和其中一个模块包时，NuGet 只解析一个 package identity，不产生重复翻译来源。

## Avalonia 集成测试

- `{atom:DatePickerLangResource Today}` 和应用 `{login:LoginLangResource Title}` 均能通过 compiled XAML。
- 多次切换语言始终复用同一个 `LanguageResourceProvider`，每次成功切换只发布一次资源通知。
- `StyledElement` 动态资源刷新，非 StyledElement 的静态 ProvideValue 行为符合文档。
- 文本、FormattingCulture 和根 FlowDirection 在同一次提交后保持同一 revision。
- 独立 Window、Dialog、Popup/Flyout 从 Application 的 `LanguageResourceProvider` 获得一致资源，不依赖 ThemeContext。
- 释放 Application/Manager 后不保留 Window、Control、ViewModel 或 DI scope。

## 性能验证

基准至少记录：

- Registry 和所有支持语言 Snapshot 的启动构建时间、分配和包体积。
- `ILocalizer.Get()` 的热路径吞吐与分配。
- 语言切换提交时间、资源通知数量和 UI 更新期间分配。
- Catalog/语言数量增长时 Snapshot 内存的线性关系。

不得为了减少启动时间恢复运行时按需 XLIFF 解析。需要优化时应改进生成表布局、字符串去重或 Snapshot 构建，
保持运行时输入仍为编译后数据。

## AOT 与发布验证

除普通测试外，必须执行：

- 所有受影响项目的 AOT/trim/single-file analyzer。
- Gallery 真实 NativeAOT publish。
- 发布产物启动并切换内置语言与至少一个静态 I18n 包语言；首个官方验收语言为 `pt-BR`。
- Gallery 的 `pt-BR` 应用 Catalog、官方聚合包控件翻译、`CultureInfo("pt-BR")` 和 LTR 状态在同一 Snapshot 生效。
- 聚合包完整引用、缺少 DataGrid 组件引用和随后加入 DataGrid 引用三种消费图都必须执行真实 pack/restore/build。
- 检查发布目录不包含运行时 XLIFF、`.atomlang`、Build Tasks 或 Generator 程序集。
- `git diff --check`。

Analyzer 通过不代表生成注册链已被 trimmer 保留，必须以真实 publish 和运行验证作为最终证据。
