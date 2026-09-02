# 构建与打包

AtomUI 使用集中化 MSBuild 配置。顶层 `Directory.Build.props` 和 `Directory.Build.targets` 只导入
`build/AtomUI.Repository.props` 与 `build/AtomUI.Repository.targets`；具体配置、NuGet 构建资产和功能入口由这两个
Repository 聚合文件按确定顺序管理。

## 基础设施目录边界

`build/` 是扁平、显式的 MSBuild 资产面：

```text
build/
├── AtomUI.Generator.props
├── AtomUI.Generator.targets
├── AtomUI.GeneratorConsumer.targets
├── AtomUI.LinkedRegistration.props
├── AtomUI.LinkedRegistration.targets
├── AtomUI.Localization.props
├── AtomUI.Localization.targets
├── AtomUI.Repository.props
├── AtomUI.Repository.targets
├── AtomUI.ThemeAssets.targets
├── MacOSHomebrewNativeAot.targets
├── OutputPaths.props
├── PackageMetadata.props
├── ProjectDefaults.props
└── Versions.props
```

文件名直接表达职责，不再用 `repository/`、`nuget/`、`platforms/` 或细粒度功能目录重复编码交付边界。进入 NuGet 的
资产由显式 item 白名单决定，而不是通过目录通配符推断。`MacOSHomebrewNativeAot.targets` 只服务仓库内 macOS
NativeAOT 应用和测试，补充 Homebrew OpenSSL/Brotli linker 搜索路径；它不进入 NuGet，也不是 AtomUI 的通用 AOT
配置。Windows 和 Linux 使用共享 AOT 配置及各自平台工具链，不需要空的对称文件。可执行验证脚本位于
`scripts/verification/`，不属于 MSBuild 交付资产。

新增基础设施文件时必须先确定职责和打包边界；需要随包交付时显式加入 `@(AtomUINuGetBuildAsset)`，不得扩大成目录通配符。

## Repository 入口

`AtomUI.Repository.props` 依次导入版本、项目默认值、包元信息和输出路径，定义源码构建使用的
`$(AtomUIBuildTasksAssembly)`、第一方语言 Catalog 的仓库级默认契约版本、显式 NuGet build asset 清单和 Generator
tool asset 清单，然后导入 `AtomUI.Generator.props`。项目文件不重复声明当前仓库统一使用的语言契约版本；NuGet
consumer 的 Localization props 不再伪造默认契约版本。

`AtomUI.Repository.targets` 定义第一方库的 AOT/Trim 默认值，集中排除 `.DotSettings` 和项目目录中的 compiler-generated
源码快照，导入 `AtomUI.Generator.targets`，并为声明 `AtomUIRegistrationPackageId` 的产品包注入 consumer target、
共享构建资产和工具程序集。注册型产品包必须包含 NuGet 自动导入约定对应的
`buildTransitive/<PackageId>.props`：有内置语言资源时由 Localization target 生成语言条目，没有语言资源时复用
`AtomUI.Generator.props` 作为 package-specific props；不得用空 props 文件规避 NuGet 包分析。

`AtomUI.Repository.props` 是 Generator build assets 的唯一清单：

- `@(AtomUINuGetBuildAsset)` 明确列出 Generator、Linked Registration、Localization 和 Theme Assets 的七个
  `.props`/`.targets` 文件，并映射到 `buildTransitive/` 根目录。
- `@(AtomUIGeneratorToolAsset)` 一次定义 Generator、Build Tasks 和任务运行所需依赖，统一进入
  `tools/netstandard2.0/`。
- `AtomUI.Generator.csproj` 与注册型产品包只能消费这两个 item，不得各自维护第二份文件清单。

`OutputPaths.props` 对仓库内所有项目统一设置 `PackageOutputPath`、`OutputPath` 和 `BaseIntermediateOutputPath`。这同样
适用于 `tools/` 下的项目；工具源码的 `.gitignore` 反向规则之后必须重新排除 `tools/**/bin/` 和 `tools/**/obj/`，
避免配置迁移或 IDE 中间态产生的本地二进制进入待提交列表。

## NuGet 入口

`build/AtomUI.Generator.props` 和 `build/AtomUI.Generator.targets` 是包根自动导入入口。它们分别导入 Linked
Registration、Localization 和 Theme Asset 的扁平 feature 文件。`AtomUI.Localization.targets` 集中拥有输入发现、
项目引用桥接、模板导出和打包流程，不再为每段几十行逻辑增加独立 fragment。

`build/AtomUI.GeneratorConsumer.targets` 是产品包模板，打包时单独重命名为
`buildTransitive/<PackageId>.targets`。它继续从包根导入 Generator 入口，并只在最终 `@(Analyzer)` 中没有
`AtomUI.Generator` 时注入同包工具程序集。

所有需要 `AtomUI.Build.Tasks` 的 feature target 都使用唯一属性 `$(AtomUIBuildTasksAssembly)`。Repository 构建将它
指向每个项目自己的影子副本 `.artifacts/<ProjectName>/obj/<Configuration>/AtomUIBuildTasksShadow/<TargetFramework>/<ShadowKey>/AtomUI.Build.Tasks.dll`；
影子目录的 `<ShadowKey>` 来自 `AtomUI.Build.Tasks` 构建后盖章的
`.artifacts/bin/<Configuration>/netstandard2.0/AtomUI.BuildTasks.ShadowKey.props`（键为编译产物的 SHA256，确定性编译保证
无变化时键稳定），`_AtomUIStageBuildTasksToolset`（`AtomUI.Repository.targets`）在任务执行前把工具集复制到该目录，
正常 consumer 构建只追加或复用影子副本，不删除其他键的目录。另一个已完成求值的并发构建可能仍引用旧键；若 staging
期间清理非当前目录，会在任务延迟加载前删除其 DLL 并随机触发 `MSB4062`。旧影子副本随显式 clean 或整个输出目录清理，
不得在普通 Build target 中回收。
NuGet consumer 由 `AtomUI.Generator.props` 回退解析相邻 `tools/netstandard2.0/AtomUI.Build.Tasks.dll`。不得新增功能专用的 Build Tasks
路径属性或只为该属性增加单独文件。调用 Build Tasks 的 target 必须同时按真实输入 item 门控；没有 AXAML、语言文件或
linked registration 输入的项目不得仅因导入共享 targets 就要求任务程序集已经存在。这样可以保证直接、干净的项目构建
不依赖解决方案项目顺序，也不会给无输入的 Debug 编译增加任务成本。

所有引用 `$(AtomUIBuildTasksAssembly)` 的 `UsingTask` 只声明 `AssemblyFile`，使用默认的进程内
`AssemblyTaskFactory` 从影子副本加载。不得重新引入 `Runtime="NET"` 或 `TaskFactory="TaskHostFactory"`：
.NET 10 SDK 的嵌套 publish 图里，TaskHost 可能在 `MetadataLoadContext` 生命周期结束后仍尝试跨进程回传
MSBuild item，随机触发 `MSB4216` / `MSB4027`。文件锁隔离由上述按 ShadowKey 版本化的影子副本承担：
重编译后的工具集落在全新目录，永不覆盖仍被活动 build node 锁定的旧副本。该约束同时适用于仓库构建和随
NuGet 交付的 buildTransitive targets，并由 build-assets 架构测试全局守卫。

## Target Framework

`build/ProjectDefaults.props` 定义：

- 开发目标框架：`net10.0`
- 生产目标框架：`net8.0`
- Debug：只构建开发目标框架。
- Release：同时构建开发目标框架和生产目标框架。

`AtomUIGallery.Browser` 单独使用 `net10.0-browser`。

## 包版本管理

`Directory.Packages.props` 启用 Central Package Management：

```xml
<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
```

Avalonia、ReactiveUI、Roslyn、测试依赖等版本在此统一管理。Release 条件下还会配置 AtomUI 各 NuGet 包版本。

AtomUI 自身版本由 `build/Versions.props` 中的 `AtomUIVersion` 管理。

## 正确性验证

构建基础设施变更至少执行与影响面匹配的验证：

```bash
dotnet test tests/AtomUI.Build.Tasks.Tests/AtomUI.Build.Tasks.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --framework net10.0 --no-restore
dotnet pack src/AtomUI.Generator/AtomUI.Generator.csproj -c Release --no-restore
scripts/verification/verify-aot-trim-registration.sh --quick
```

涉及 linked publish、平台 target 或发布脚本时，还必须运行 `--full` 和真实 Gallery NativeAOT publish。打包验证不能只看
命令退出码：必须检查 `.nupkg` ZIP 条目，确认根自动导入入口、显式 build assets、Analyzer/Tools 位置正确，且不包含
Repository 配置、`MacOSHomebrewNativeAot.targets` 或 `scripts/` 资产。

## 源生成输出

需要把源生成结果写到仓库目录的项目只声明标准 SDK 属性：

```xml
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
<CompilerGeneratedFilesOutputPath>GeneratedFiles</CompilerGeneratedFilesOutputPath>
```

`GeneratedFiles/` 是本地编译产物，默认由 `.gitignore` 忽略；只有 GalleryBase 中被结构测试直接读取的少量快照保留跟踪。

`AtomUI.Repository.targets` 根据 `CompilerGeneratedFilesOutputPath` 统一从 `Compile` 移除这些快照，避免第二次构建把上次
生成结果作为普通源码再次编译。项目文件不得重复声明同一条 `Compile Remove`。源生成器通过 Analyzer 方式参与当前编译。

## Analyzer 引用方式

ordinary `AtomUI.Generator` 在多个项目中以 Analyzer 形式引用：

```xml
<ProjectReference Include="../AtomUI.Generator/AtomUI.Generator.csproj"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false"
                  PrivateAssets="all" />
```

这意味着生成器不是运行时依赖。它在编译期生成 Token、Theme、语言和 leaf Registration Unit 代码，运行时依赖的是生成后的
类型和资源键。

linked registration 的应用 usage、Sidecar 和 Application Plan 位于独立的 `AtomUI.Generator.LinkedPublish` Analyzer 中。
它只在 `PublishTrimmed=true`、`PublishAot=true`、`RunAOTCompilation=true`、Package Manifest Pack 或显式 strict 验证时注入。
普通 Debug 和未启用 AOT/Trim 的 Release compiler command line 中不得出现该 Analyzer。

## Linked publish 注册

AtomUI 把 `PublishTrimmed=true`、`PublishAot=true` 和 WebAssembly `RunAOTCompilation=true` 统一视为 linked publish。
普通非裁剪构建继续使用包级全量注册；linked publish 由 Generator 聚合应用、类库和第三方包的 Sidecar Manifest，计算
UnitEdge closure 后按 Package 生成静态 Registration Unit 调用计划。默认一个 Package 生成一个完整 Unit；只有显式设置
`AtomUIRegistrationGranularity=Directory` 的大型多控件包才按稳定控件族拆分。Control descriptor、Own Token schema 和
Control-owned AXAML Theme 跟随对应 Unit；Language、包级初始化逻辑、Global Token、Theme Algorithm、Provider、平台
selector 和显式 `PackageShared` 资源作为 Package Core 整体保留。
完整模式矩阵和注册协议见 [AOT Linked Registration Pipeline](aot-linked-registration-pipeline.md)。

每个可直接安装、且声明 `AtomUIRegistrationPackageId` 的第一方产品 NuGet 都内嵌 ordinary/linked Generator、Build Tasks、
Sidecar 和 `buildTransitive` assets。应用只引用 `AtomUI.Controls`、`AtomUI.Desktop.Controls`、DataGrid、ColorPicker、Extras 或
GalleryBase 中的实际产品包，即可获得编译期使用分析和 linked registration；不要求额外添加
`AtomUI.Generator` PackageReference。显式 Generator 引用继续作为兼容路径支持。

产品包入口在 `ResolveReferences` 后检查最终 `@(Analyzer)`，只在尚未存在对应 Analyzer 时注入同包工具程序集。
多个产品包同时引用时，编译器仍只能收到一份 ordinary Generator 和至多一份 linked Generator。共享 props/targets 使用幂等
property 防止重复导入。Generator、Sidecar 和 Build Tasks 只允许位于包的 `tools/` 与 `buildTransitive/` 目录，不得进入 `lib/`
或应用输出、发布目录。

这些 `buildTransitive` assets 负责在 linked build 中导入 Sidecar、生成结构化 `@(AvaloniaXaml)` usage、配置 linker 可替换的
`AtomUI.AotTrimRegistration.Enabled` 注册标记，并在 ILLink/ILCompiler 前验证计划标记。普通构建中这些 linked targets 必须
skip，连空 usage 文件都不能创建。该标记只属于 AOT/Trim 注册基础设施，不得作为通用运行时 feature 使用。

linked build 必须在 `ResolveReferences` 后以最终 `ReferencePath` 为程序集全集，建立带来源的 Sidecar candidate catalog，再按
Sidecar 声明的 `assembly.name` 和 `contractHash`
解析唯一 canonical 输入。ProjectReference companion、NuGet package 和 metadata extraction 的来源优先级只用于选择同 hash
候选；不同 hash 必须构建失败，不能按路径、文件名或 item 顺序静默覆盖。NuGet package 已交付正式 Sidecar 时，不得因为
程序集 `ReferencePath` 旁没有 companion 文件而再次执行 metadata extraction。只有解析后的 canonical Sidecar 可以进入
`AdditionalFiles`。

类库只在被 linked 应用作为 ProjectReference 构建或执行 NuGet Pack 时生成 Usage Sidecar。Pack 自动把 Sidecar 和唯一的
`<PackageId>.targets` 放入 `buildTransitive`；普通 Debug/Release 不扫描 usage，不生成 Sidecar，也不安装 Application Plan。

Application Plan 只对 Sidecar 中紧凑的 UnitEdge 图计算 closure/SCC，不计算 Theme Asset、Catalog、Feature 或 Initializer 图。
Unit fragment 是叶子，不调用其他 Unit。无法可靠确定 Unit 时，只对对应 Package 使用 full fallback。

## 打包边界

当前源码打包边界为：

- `AtomUI.Native`
- `AtomUI.Core`
- `AtomUI.Fonts.AlibabaSans`
- `AtomUI.Icons.Shared`
- `AtomUI.Icons.AntDesign`
- `AtomUI.Controls.Shared`
- `AtomUI.Controls`
- `AtomUI.Desktop.Controls`
- `AtomUI.Toolkits.GalleryBase`
- `AtomUI.Desktop.Controls.DataGrid`
- `AtomUI.Desktop.Controls.ColorPicker`
- `AtomUI.Desktop.Controls.Extras`
- `AtomUI.Generator`

DataGrid、ColorPicker 和 Extras 是独立按需包，但源码上依赖 `AtomUI.Desktop.Controls` 并访问其内部成员。
GalleryBase 是产品中立的 Gallery 应用底座包，跟随主库版本发布，供 AtomUI 生态内的产品 Gallery、Demo 和文档应用复用。

正式 NuGet 项目、Package ID 和发布分组统一声明在 `scripts/NuGetPackageProjects.ps1`。GitHub Actions 发布 workflow、
本地 NuGet 发布脚本和产物完整性校验必须共同消费该清单，不得分别维护项目列表。新增、拆分或移除正式包时，先更新该清单，
并让缺包或多包校验在上传与推送前失败。

`scripts/BuildNuGetPackages.ps1` 是正式包的唯一构建编排入口。它先构建清单中的 Build Tasks 和 linked-publish Generator
等非包前置项目，再完成全部包项目的 build，之后才允许执行任何 pack。发布构建必须关闭 MSBuild 节点复用、串行访问共享
工具输出，并在完整包集合校验通过后才进入本地 feed、artifact upload 或 nuget.org push；任一 `dotnet` 命令失败都必须立即
终止流程。pack 为生成 linked-registration Sidecar 发起的内部 build 只构建当前包，复用外层阶段已经生成的项目引用产物。

注册型产品包必须从同一次、同版本 Release 构建中封装 Generator 和 Build Tasks。不得在修改 `AtomUIVersion` 后使用
`dotnet pack --no-build` 复用另一个版本留下的工具输出；多个同版本产品包中的编译资产必须具有一致内容。
