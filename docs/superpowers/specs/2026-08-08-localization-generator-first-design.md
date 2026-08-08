# 本地化 Generator 优先构建设计

## 目标

将尽可能多的本地化语义从 `build/AtomUI.Localization.props` 和
`build/AtomUI.Localization.targets` 下沉到 `AtomUI.Generator`，同时保持 MSBuild 文件简洁，
并保留现有工作流的核心语义：本地 XLIFF 编译为静态运行时数据，语言包项目生成声明式包资产，
项目引用可以提供源契约，模板导出仍然通过 MSBuild 命令完成。

这是一次有意的构建契约重设计。不要求兼容当前 targets 使用者，现有 MSBuild 属性和 metadata
可以删除或重命名。

## 非目标

- 不让运行时解析 XLIFF 或动态检查程序集。
- 不让 Source Generator 向 `obj`、`bin` 或 NuGet 包任意写文件。
- 不用反射、文件系统探测或包目录扫描替代 MSBuild 的项目间 target 协议。
- 除了把语义校验迁移到 Generator 所必需的部分，不改变现有运行时注册模型。

## 职责划分

### `AtomUI.Generator`

Generator 成为编译项目中 XLIFF 输入的唯一语义权威，负责：

- XLIFF 2.1 解析以及带源文件位置的诊断。
- 语言标签、翻译状态、占位符、源文本和完整 Bundle 校验。
- 根据当前和引用程序集中的 Roslyn symbol 解析 Catalog/module。
- `Verified` 与 `Deferred` 契约处理，包括引用模块出现后的 Deferred 激活。
- source fingerprint 和 ContractVersion 校验。
- 重复翻译来源和 override unit 冲突检测。
- 文件、Catalog、Bundle 和生成源码的确定性排序。
- 生成 Catalog descriptor、Translation Bundle、模块注册和 Application bootstrap。

语义处理流水线应拆分为可复用的解析/规范化与校验步骤，并接入现有增量生成流程。
Generator 必须保持 AOT 友好，禁止使用反射、运行时 XML 解析、路径 glob 或包探测。

### `AtomUI.Build.Tasks`

Build Tasks 只保留必须使用 MSBuild 或产生文件系统副作用的操作：

- `PrepareLanguagePackageTask`：校验和准备静态语言包，存在权威源契约时完成绑定，计算包路径和
  fingerprint，并写入 manifest 输入。
- `GenerateLanguagePackagePropsTask`：写出语言包声明式 `buildTransitive` props。
- `ExportLanguageTemplatesTask`：将权威 `en-US` 源 Catalog 合并到目标语言模板。
- 如果没有独立使用者，可以将 `CollectLanguageCatalogsTask` 合并进导出流程。

普通模块/应用编译不再执行 `ValidateLanguageFilesTask`。静态语言包项目仍通过
`PrepareLanguagePackageTask` 执行校验，因为这类项目不会运行用于生成运行时代码的本地化 Generator。

XLIFF parser、规范化文档模型、fingerprint 计算和包 writer 继续放在
`AtomUI.Localization.Build.Shared`，由 Generator 和 Build Tasks 共同使用。

### 第三方语言包项目

第三方作者是正式支持的构建场景，不应依赖官方模块包的特殊路径。第三方静态语言包项目没有运行时程序集，
也不需要编译 Catalog；它只需显式声明 package ID、目标语言和 module ID，然后由 pack targets 校验并打包 XLIFF。

支持两种作者模式：

- **Verified 模式**：项目以仅用于创作的 `PackageReference` 引用目标组件包，并设置 `PrivateAssets=all`。
  Build Tasks 从该包读取权威 `en-US` XLIFF 和 Catalog metadata，将每个目标文件绑定到唯一 Catalog，复制真实
  ContractVersion，并输出 `Verified` 包 metadata。
- **Deferred 模式**：作者无法取得权威源契约。Build Tasks 仍校验 XLIFF 结构、目标语言、翻译状态、包内容和
  规范化路径，但不伪造 ContractVersion，输出 `Deferred` metadata 并给出 warning。该包可以发布，契约校验延迟
  到消费应用完成。

消费应用中的 Generator 对两种模式拥有最终权威：

1. 读取语言包生成的声明式 `buildTransitive` 输入。
2. 根据当前或引用组件程序集中的 module identity 和 Catalog enum，将目标文件的 `file id` 解析到 Catalog。
3. 如果模块存在，激活 Bundle，并完整校验源文本、unit、占位符、状态、ContractVersion 和 fingerprint。Deferred
   输入一旦激活，不得降低校验强度。
4. 如果模块不存在，静态输入保持 dormant，不生成 Bundle，也不报告错误。

因此，第三方包可以在无法引用所有可选 AtomUI 组件时先行发布；应用真正安装组件后，仍会得到严格的编译期诊断。
一个语言包必须只对应一个 `AtomUILanguageModuleId`；多模块翻译应拆成多个包，或另建显式的聚合 Meta Package。
聚合包只携带依赖，不得传递 XLIFF、localization `buildTransitive` item、analyzer 或运行时资产。

### `AtomUI.Localization.props`

props 文件只包含稳定默认值和 item 定义：

- module identity 回退值（优先 `PackageId`，其次 `AssemblyName`）；
- 默认 ContractVersion；
- module-built-in 与 application-override item 的默认 metadata。

不在 props 中定义校验策略、target 顺序、Task 路径或打包行为。

### `AtomUI.Localization.targets`

targets 文件压缩为三个职责域。

1. **输入投影**
   - 用一个规范化 item 流发现本地 XLIFF 和 override 文件。
   - 排除输出目录、中间目录和生成目录。
   - 将本地输入以及项目引用解析出的资产投影为 `AdditionalFiles`，只保留 Generator 无法从 symbol 推断的最小 metadata。
   - 只暴露必需的 compiler-visible item metadata 和项目属性。

2. **项目引用桥接**
   - 保留一个小型 provider target，返回模块权威 `en-US` 源资产。
   - 保留一个小型 consumer target，为显式语言包项目引用调用 provider，并在 editorconfig 生成和编译前将返回资产加入
     `AdditionalFiles`。
   - 该协议必须保留在 MSBuild 中，因为 Source Generator 无法在当前求值阶段调用另一个项目的 target。

3. **Pack/export 钩子**
   - 保留调用三个副作用 Build Task 的 pack 准备 target。
   - 保留公开的 `AtomUIExportLanguageTemplates` MSBuild 命令。
   - 删除编译期校验 target、重复的 metadata 转发块和可配置的 minimum-state 属性。普通编译输入固定要求
     `translated`，静态语言包准备固定要求 `final`。

尽量通过 `ItemDefinitionGroup` 只声明一次 item metadata，并用一个合并的 `AdditionalFiles` item 转发 module 与
override 输入，避免目前的重复 Include 块。

## 输入与诊断流

```text
MSBuild glob/项目引用桥接
  -> AdditionalFiles + 最小 source metadata
  -> 增量 Generator 解析并规范化 XLIFF
  -> symbol resolver 分类 active/dormant/deferred 输入
  -> 语义校验器报告诊断
  -> 有效的 active Catalog 生成确定性源码
```

即使某个 Catalog 最终不生成源码，Generator 也必须报告其诊断。唯一的有意例外是 dormant 静态输入：在对应模块
存在前忽略它；一旦激活，Deferred 输入必须执行与 Verified 输入相同的严格契约和 Bundle 校验。

## 建议的内部结构

在 `src/AtomUI.Generator/Localization` 下增加聚焦的语义层：

```text
Localization/
  Semantic/
    LocalizationInputNormalizer.cs
    LocalizationSemanticValidator.cs
    LocalizationSourceIndex.cs
    LocalizationDiagnosticFactory.cs
```

实际文件拆分可以遵循现有 Generator 风格，但职责必须保持分离：

- normalizer 将 analyzer options 和解析后的 XLIFF 转成不可变输入记录；
- source index 解析当前及引用的 Catalog symbol；
- validator 只产生诊断，不写源码；
- 现有 compiler/writer 消费校验后的不可变模型。

不新增运行时 public API。仅供生成源码使用的 marker 或 helper 保持 `internal` 或仅存在于 Generator 中。

## 错误处理

- XLIFF 或 metadata 无效时，生成带文件位置的 Source Generator diagnostic。
- 没有源契约的静态包只有在 pack 过程标记为 `Deferred` 时才允许继续；消费端 Generator 决定它是 dormant 还是 active。
- 模块只能部分解析时报告错误；不能对同一模块的不同文件静默降级为 Deferred。
- 不吞掉诊断以生成部分源码。只有完整语义编译成功的 Catalog 才生成源码。

## 测试策略

### Generator 测试

扩展 `tests/AtomUI.Generator.Tests/Localization`，覆盖：

- 原 `ValidateLanguageFilesTask` 覆盖的全部 XLIFF 结构和翻译诊断；
- module/source identity 默认值和显式 metadata 覆盖；
- 重复 Bundle 和重复 override unit；
- Verified/Deferred ContractVersion 与 fingerprint 路径；
- dormant 静态输入及通过引用 symbol 激活 Deferred 输入；
- 确定性输出和增量缓存行为。

### Build Task 测试

保留以下聚焦测试：

- 包内容安全和规范化包路径；
- Verified/Deferred 包准备及 manifest 输出；
- 生成的包 props；
- 模板导出和项目引用资产返回。

### 集成测试

更新端到端语言包 fixture，验证：

- 普通模块/应用构建依赖 Generator 诊断；
- 静态语言包项目中的无效内容仍在 pack 前失败；
- 消费应用编译 active Bundle，并忽略 dormant 可选包；
- 第三方 Verified 包能绑定引用模块契约；
- 第三方 Deferred 包发布时不伪造 ContractVersion，只在 pack 时 warning 一次；模块安装后激活并校验，模块缺失时保持 dormant；
- 聚合 Meta Package 不泄漏本地化输入或运行时资产；
- 生成包资产保持确定性。

最终验证命令：

```bash
dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --no-restore
dotnet test tests/AtomUI.Build.Tasks.Tests/AtomUI.Build.Tasks.Tests.csproj --no-restore
dotnet test tests/AtomUI.Localization.IntegrationTests/AtomUI.Localization.IntegrationTests.csproj --no-restore
git diff --check
```

## 验收标准

- `build/AtomUI.Localization.props` 只包含默认值和 item 定义。
- `build/AtomUI.Localization.targets` 不包含普通编译校验 target，且只有一条合并的本地化输入投影路径。
- 对编译项目而言，Generator 是唯一的语义校验器。
- 静态语言包的 pack/export 行为仍有明确的 Build Task 覆盖。
- 第三方 Verified 和 Deferred 包流程都有端到端测试。
- 不引入运行时反射、运行时 XLIFF 解析或生成源码非确定性。
- 所有聚焦测试及 `git diff --check` 通过。
