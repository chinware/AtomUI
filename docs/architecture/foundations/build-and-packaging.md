# 构建与打包

AtomUI 使用集中化 MSBuild 配置。顶层 `Directory.Build.props` 和 `Directory.Build.targets` 只导入
`build/repository/AtomUI.Repository.props` 与 `build/repository/AtomUI.Repository.targets`；具体配置和功能入口由
Repository 聚合文件按确定顺序管理。

## 基础设施目录边界

`build/` 根目录不放文件，只保留三个按交付边界划分的目录：

```text
build/
├── repository/    # 只服务源码仓库构建、打包和输出布局
├── nuget/         # 随 NuGet 包交付的 props/targets 源资产
└── platforms/     # 可复用的平台发布目标
```

- `build/repository/` 拥有版本、项目默认值、包元信息、输出路径、Repository 聚合入口和 Generator 资产清单。
- `build/nuget/` 拥有 `AtomUI.Generator.props`、`AtomUI.Generator.targets` 以及
  `linked-registration/`、`localization/`、`theme/`、`infrastructure/` 等包内功能目录。
- `build/platforms/macos/NativeAot.targets` 是 macOS NativeAOT 验证共享目标，不进入 NuGet 包。
- 可执行验证脚本位于 `scripts/verification/`，不属于 MSBuild 交付资产。

新增基础设施文件时必须先判断它服务 Repository、NuGet consumer、平台发布还是可执行验证；不得重新把文件放回
`build/` 根目录。

## Repository 入口

`AtomUI.Repository.props` 依次导入版本、项目默认值、包元信息、输出路径和
`GeneratorBuildAssets.props`，然后设置源码构建使用的 `$(AtomUIBuildTasksAssembly)` 并导入 NuGet feature props。
`AtomUI.Repository.targets` 统一导入项目默认 target、linked registration、产品包 Generator 资产注入、本地化和
Theme Asset targets。

`build/repository/GeneratorBuildAssets.props` 是 Generator build assets 的唯一清单：

- `@(AtomUINuGetBuildAsset)` 通过 `build/nuget/**/*.props` 与 `build/nuget/**/*.targets` 收集资产，排除
  `consumer/**`，并使用 `%(RecursiveDir)` 保留 `buildTransitive/` 内部目录。
- `@(AtomUIGeneratorToolAsset)` 一次定义 Generator、Build Tasks 和任务运行所需依赖，统一进入
  `tools/netstandard2.0/`。
- `AtomUI.Generator.csproj` 与注册型产品包只能消费这两个 item，不得各自维护第二份文件清单。

## NuGet 入口

`build/nuget/AtomUI.Generator.props` 和 `build/nuget/AtomUI.Generator.targets` 是包根自动导入入口。它们只编排功能目录，
具体实现由各 owner 文件维护。本地化入口进一步按 `Inputs.targets`、`ProjectReferences.targets`、`Export.targets` 和
`Packaging.targets` 拆分职责。

`build/nuget/consumer/ProductPackage.targets` 是产品包模板，打包时单独重命名为
`buildTransitive/<PackageId>.targets`。它继续从包根导入 Generator 入口，并只在最终 `@(Analyzer)` 中没有
`AtomUI.Generator` 时注入同包工具程序集。

所有需要 `AtomUI.Build.Tasks` 的 feature target 都使用唯一属性 `$(AtomUIBuildTasksAssembly)`。Repository 构建将它
指向 `output/bin/<Configuration>/netstandard2.0/AtomUI.Build.Tasks.dll`；NuGet consumer 通过
`buildTransitive/infrastructure/BuildTasks.props` 解析包内 `tools/netstandard2.0/AtomUI.Build.Tasks.dll`。不得新增功能专用
的 Build Tasks 路径属性。

## Target Framework

`build/repository/ProjectDefaults.props` 定义：

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

AtomUI 自身版本由 `build/repository/Versions.props` 中的 `AtomUIVersion` 管理。

## 正确性验证

构建基础设施变更至少执行与影响面匹配的验证：

```bash
dotnet test tests/AtomUI.Build.Tasks.Tests/AtomUI.Build.Tasks.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --framework net10.0 --no-restore
dotnet pack src/AtomUI.Generator/AtomUI.Generator.csproj -c Release --no-restore
scripts/verification/verify-aot-trim-registration.sh --quick
```

涉及 linked publish、平台 target 或发布脚本时，还必须运行 `--full` 和真实 Gallery NativeAOT publish。打包验证不能只看
命令退出码：必须检查 `.nupkg` ZIP 条目，确认根自动导入入口、内部 feature 目录、Analyzer/Tools 位置正确，且不包含
`repository/`、`platforms/` 或 `scripts/` 资产。

## 源生成输出

多个项目启用：

```xml
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
<CompilerGeneratedFilesOutputPath>GeneratedFiles</CompilerGeneratedFilesOutputPath>
<Compile Remove="$(CompilerGeneratedFilesOutputPath)/**/*.cs"/>
```

`GeneratedFiles/` 是本地编译产物，默认由 `.gitignore` 忽略；只有 GalleryBase 中被结构测试直接读取的少量快照保留跟踪。

生成文件输出到项目内 `GeneratedFiles/`，但从编译输入中移除该目录，避免重复编译。源生成器通过 Analyzer 方式参与当前编译。

## Analyzer 引用方式

`AtomUI.Generator` 在多个项目中以 Analyzer 形式引用：

```xml
<ProjectReference Include="../AtomUI.Generator/AtomUI.Generator.csproj"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false"
                  PrivateAssets="all" />
```

这意味着生成器不是运行时依赖。它在编译期生成 Token 和语言相关代码，运行时依赖的是生成后的类型和资源键。

## Linked publish 注册

AtomUI 把 `PublishTrimmed=true`、`PublishAot=true` 和 WebAssembly `RunAOTCompilation=true` 统一视为 linked publish。
普通非裁剪构建继续使用包级全量注册；linked publish 由 Generator 聚合应用、类库和第三方包的 Usage Manifest，
按 Package 生成 Registration Unit 调用计划。Control descriptor、Own Token schema 和控件族专属 AXAML Theme 跟随
Unit 裁剪；Language、包级初始化逻辑、Global Token、Theme Algorithm、Provider、平台 selector 和显式
`PackageShared` 资源作为 Package Core 整体保留。
完整模式矩阵和注册协议见 [AOT 与裁剪架构](aot-and-trimming.md)。

每个可直接安装、且声明 `AtomUIRegistrationPackageId` 的第一方产品 NuGet 都内嵌同版本的 Generator、Build Tasks 和
`buildTransitive` assets。应用只引用 `AtomUI.Controls`、`AtomUI.Desktop.Controls`、DataGrid、ColorPicker、Extras 或
GalleryBase 中的实际产品包，即可获得编译期使用分析和 linked registration；不要求额外添加
`AtomUI.Generator` PackageReference。显式 Generator 引用继续作为兼容路径支持。

产品包入口在 `ResolveReferences` 后检查最终 `@(Analyzer)`，只在尚未存在 `AtomUI.Generator` 时注入同包工具程序集。
多个产品包同时引用或应用保留显式 Generator 引用时，编译器仍只能收到一份 Generator。共享 props/targets 使用幂等
property 防止重复导入。Generator 和 Build Tasks 只允许位于包的 `tools/` 与 `buildTransitive/` 目录，不得进入 `lib/`
或应用输出、发布目录。

这些 `buildTransitive` assets 负责向编译器暴露 `PublishTrimmed`、`PublishAot`、AtomUI 验证开关和
`@(AvaloniaXaml)` 使用信息，配置 linker 可替换的 `AtomUI.AotTrimRegistration.Enabled` 注册标记，并在
ILLink/ILCompiler 前验证计划标记。该标记只属于 AOT/Trim 注册基础设施，不得作为通用运行时 feature 使用。

类库在普通构建中也要生成只含稳定字符串 identity 的 Usage Manifest，因为最终入口应用可能以 linked 模式引用它；
普通构建不得安装 Application Plan、改变 `UseDesktopControls()` 的全量行为或要求动态 root。

Application Plan 不计算 Theme Asset、Catalog、Feature 或 Initializer 图，也不执行应用级依赖闭包。Unit 内依赖由强类型
Unit fragment 直接表达；无法可靠确定 Unit 时，只对对应 Package 使用 full fallback。

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

注册型产品包必须从同一次、同版本 Release 构建中封装 Generator 和 Build Tasks。不得在修改 `AtomUIVersion` 后使用
`dotnet pack --no-build` 复用另一个版本留下的工具输出；多个同版本产品包中的编译资产必须具有一致内容。
