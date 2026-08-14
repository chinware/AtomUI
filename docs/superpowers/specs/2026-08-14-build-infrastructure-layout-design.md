# Build 基础设施目录治理设计

> 本文记录 `build/` 目录重组的设计决策和实施边界。长期事实应同步到
> [`docs/architecture/foundations/build-and-packaging.md`](../../architecture/foundations/build-and-packaging.md)；
> 本文不替代正式构建与打包架构文档。

## 背景

当前 `build/` 同时保存以下不同性质的内容：

- 仓库级版本、Target Framework、输出路径和 NuGet 元数据配置。
- 随 `AtomUI.Generator` 和第一方产品包发布的 `buildTransitive` 资产。
- Generator、Localization、Theme Asset 和 Linked Registration 构建管线。
- macOS NativeAOT 链接参数。
- AOT/Trim 验证脚本。

这些文件全部平铺在同一层。目录无法表达文件是否只服务源码仓库、是否进入 NuGet、由哪个子系统拥有，也无法约束后续新增文件的放置位置。`AtomUI.Localization.targets` 已增长到 238 行，同时承担输入发现、项目引用桥接、模板导出和两类打包流程，进一步放大了维护成本。

## 目标

- 按交付边界区分仓库内部配置、NuGet 构建资产和平台构建片段。
- 让 `build/` 根目录不再包含散落文件。
- 让入口文件只负责导入和编排，具体逻辑由明确所有者维护。
- 拆分职责过多的 Localization targets。
- 统一 `AtomUI.Build.Tasks.dll` 的路径解析和错误检查。
- 用唯一资产清单驱动 Generator 包与产品包的构建资产收集。
- 把非 MSBuild 验证脚本移出 `build/`。
- 通过结构测试、真实打包、完整测试矩阵和 AOT 发布证明迁移正确。

## 非目标

- 不保留旧仓库路径、旧文件名或转发壳。
- 不保留旧 NuGet 包内辅助文件路径。
- 不保留旧的 Build Task 专用路径属性或兼容别名。
- 不改变 Generator、Localization、Theme、Linked Registration 的功能语义。
- 不修改运行时 API、控件行为或生成代码协议。
- 不借本次目录治理重构 Build Task 或 Generator 的业务实现。

本次变更明确不以兼容性为目标。旧路径和旧内部构建契约可以直接删除；验收标准是新结构在仓库构建、NuGet 消费、打包和发布场景中行为正确。

## 最终目录

```text
build/
├── repository/
│   ├── AtomUI.Repository.props
│   ├── AtomUI.Repository.targets
│   ├── Versions.props
│   ├── ProjectDefaults.props
│   ├── ProjectDefaults.targets
│   ├── PackageMetadata.props
│   ├── OutputPaths.props
│   ├── GeneratorBuildAssets.props
│   └── PackageGeneratorAssets.targets
├── nuget/
│   ├── AtomUI.Generator.props
│   ├── AtomUI.Generator.targets
│   ├── consumer/
│   │   └── ProductPackage.targets
│   ├── infrastructure/
│   │   └── BuildTasks.props
│   ├── linked-registration/
│   │   ├── LinkedRegistration.props
│   │   └── LinkedRegistration.targets
│   ├── localization/
│   │   ├── Localization.props
│   │   ├── Localization.targets
│   │   ├── Inputs.targets
│   │   ├── ProjectReferences.targets
│   │   ├── Export.targets
│   │   └── Packaging.targets
│   └── theme/
│       └── ThemeAssets.targets
└── platforms/
    └── macos/
        └── NativeAot.targets

scripts/
└── verification/
    └── verify-aot-trim-registration.sh
```

`build/` 根目录只允许 `repository/`、`nuget/` 和 `platforms/` 三个目录。新的构建文件必须先确定交付边界，再进入对应目录。

## 仓库构建入口

顶层文件只保留一个 props 入口和一个 targets 入口：

```text
Directory.Build.props
└── build/repository/AtomUI.Repository.props
    ├── Versions.props
    ├── ProjectDefaults.props
    ├── PackageMetadata.props
    ├── OutputPaths.props
    ├── GeneratorBuildAssets.props
    ├── ../nuget/linked-registration/LinkedRegistration.props
    └── ../nuget/localization/Localization.props

Directory.Build.targets
└── build/repository/AtomUI.Repository.targets
    ├── ProjectDefaults.targets
    ├── PackageGeneratorAssets.targets
    ├── ../nuget/linked-registration/LinkedRegistration.targets
    ├── ../nuget/localization/Localization.targets
    └── ../nuget/theme/ThemeAssets.targets（条件导入）
```

`AtomUI.Repository.props` 和 `AtomUI.Repository.targets` 只负责稳定导入顺序，不承载具体构建行为。这样新增仓库配置时，可以在一个入口中看到完整依赖顺序，同时不会重新形成新的巨型文件。

## Repository 文件职责

| 文件 | 职责 |
|---|---|
| `Versions.props` | 只定义 `AtomUIVersion` 和 `AvaloniaVersion` |
| `ProjectDefaults.props` | Target Framework、默认 Configuration、警告、测试项目识别、Avalonia 构建默认值 |
| `ProjectDefaults.targets` | AOT/Trim 默认声明和 `.DotSettings` 排除 |
| `PackageMetadata.props` | NuGet 公共元数据及 logo、LICENSE、README 资产 |
| `OutputPaths.props` | NuGet、编译输出和中间产物路径 |
| `GeneratorBuildAssets.props` | 定义进入 Generator 包和产品包的共享构建资产集合 |
| `PackageGeneratorAssets.targets` | 为声明 `AtomUIRegistrationPackageId` 的产品包加入 Consumer target、共享构建资产和工具程序集 |

原 `Output.App.props` 在当前仓库没有任何 Import 或其他引用。它不进入新结构，迁移时直接删除，并由仓库引用扫描确认没有遗留使用者。

## NuGet 构建资产

`build/nuget/` 表示会进入 NuGet 包的构建资产源。其目录布局在打包时原样映射到 `buildTransitive/`，只有产品包入口需要按当前 `PackageId` 重命名。

`AtomUI.Generator` 包必须在 `buildTransitive/` 根目录包含：

```text
AtomUI.Generator.props
AtomUI.Generator.targets
```

这两个名称由 NuGet 自动导入规则和 `AtomUI.Generator` PackageId 决定，保留它们是当前正确性的必要条件，不是兼容层。

第一方产品包把：

```text
build/nuget/consumer/ProductPackage.targets
```

打包为：

```text
buildTransitive/<PackageId>.targets
```

产品入口导入 `AtomUI.Generator.props` 和 `AtomUI.Generator.targets`，并在 `ResolveReferences` 后只在尚未存在 `AtomUI.Generator` Analyzer 时注入包内 Generator。共享构建资产和工具程序集仍不得进入 `lib/`、应用输出或发布目录。

旧的根级辅助文件名不保留。Linked Registration、Localization、Theme 和 Build Tasks 只通过子目录内部 Import 连接。

## 唯一资产清单

`GeneratorBuildAssets.props` 是打包资产的唯一仓库权威。规则为：

- `build/nuget/**/*.props` 和 `build/nuget/**/*.targets` 默认属于共享 NuGet 构建资产。
- `build/nuget/consumer/**` 不作为共享资产原样打包，只由产品包重命名为 `<PackageId>.targets`。
- 共享资产保留相对 `build/nuget/` 的目录结构，映射到 `buildTransitive/`。
- `build/repository/**`、`build/platforms/**` 和 `scripts/**` 永远不能进入 NuGet 构建资产集合。

`AtomUI.Generator.csproj` 和 `PackageGeneratorAssets.targets` 都消费该资产清单，不再分别手写同一组文件。新增 NuGet 构建资产时，只要放入正确目录并通过结构测试，就会进入两个打包路径。

## Build Tasks 路径

`build/nuget/infrastructure/BuildTasks.props` 定义唯一的：

```text
AtomUIBuildTasksAssembly
```

仓库构建由 `AtomUI.Repository.props` 在导入 NuGet 功能资产前，把它设置为当前 Configuration 下的 `AtomUI.Build.Tasks.dll` 输出路径。NuGet 消费场景则由 `BuildTasks.props` 回退到包内 `tools/netstandard2.0/AtomUI.Build.Tasks.dll`。

Linked Registration、Localization 和 Theme targets 统一引用该属性。原有 `AtomUILocalizationBuildTasksAssembly` 和 `AtomUILinkedRegistrationBuildTasksAssembly` 删除，不提供别名。

`UsingTask` 继续无条件登记，真正需要 Task 的 Target 在执行期检查程序集是否存在。不得在 `UsingTask` 上增加求值期 `Exists(...)` 条件，以免破坏冷构建、静态图构建和 NativeAOT publish。

## Localization 拆分

`Localization.props` 只定义 module identity、ContractVersion 和语言 item 默认 metadata。

`Localization.targets` 是聚合入口，只按固定顺序导入：

1. `Inputs.targets`
2. `ProjectReferences.targets`
3. `Export.targets`
4. `Packaging.targets`

各文件职责如下：

| 文件 | 职责 |
|---|---|
| `Inputs.targets` | XLIFF 排除规则、自动发现、AdditionalFiles 投影、compiler-visible metadata |
| `ProjectReferences.targets` | 模块源契约和静态语言包项目引用的 provider/consumer target 协议 |
| `Export.targets` | `AtomUIExportLanguageTemplates` 公开命令及模板导出 Task |
| `Packaging.targets` | 静态语言包准备、模块语言资产准备、manifest/props/contentFiles 打包 |

Target 名称、依赖顺序和执行阶段保持当前语义。拆分只改变文件所有权，不重新设计本地化协议。

## 平台与验证脚本

原 `AtomUI.NativeAot.MacOS.targets` 移到：

```text
build/platforms/macos/NativeAot.targets
```

它是仓库发布验证使用的平台片段，不进入 NuGet。相关 fixture、文档和脚本全部切换到新路径。

原 AOT/Trim 验证脚本移到：

```text
scripts/verification/verify-aot-trim-registration.sh
```

`build/` 从此只保存 MSBuild XML 资产，不保存 Bash、PowerShell 或其他流程脚本。后续验证脚本统一进入 `scripts/verification/`。

## 直接迁移策略

本次迁移在一个变更集中完成：

1. 先增加结构和打包契约测试，固定最终目录规则与必要自动导入入口。
2. 创建 Repository 聚合入口和 NuGet 资产清单。
3. 移动仓库配置文件并按职责重命名。
4. 移动 NuGet 资产，直接切换到子目录 Import。
5. 拆分 Localization targets。
6. 统一 Build Tasks 路径属性并删除旧专用属性。
7. 移动 macOS target 和验证脚本。
8. 更新项目文件、测试、正式文档和发布脚本中的全部路径。
9. 删除旧文件，不保留转发 Import 或重复资产。

迁移完成前不允许同时维护新旧两套导入链。每个阶段通过测试提交保持可审查，但最终分支只包含新结构。

## 正确性不变量

- 所有 MSBuild Import 都能解析到唯一存在的文件，且不存在导入循环。
- `Directory.Build.props` 和 `Directory.Build.targets` 只导入 Repository 聚合入口。
- `build/` 根目录没有文件，且只有批准的三个一级目录。
- Repository、Platform 和 Script 文件不会进入任何 NuGet 包。
- `build/nuget/` 中除 `consumer/` 外的每个 `.props`、`.targets` 都进入 Generator 包和注册型产品包。
- Generator 包拥有 NuGet 自动导入所需的 `AtomUI.Generator.props` 和 `AtomUI.Generator.targets`。
- 产品包只生成一个匹配自身 PackageId 的根级 consumer target。
- Generator 和 Build Tasks 不进入应用运行时依赖、应用输出或 publish 目录。
- Localization 的 Target 名称、BeforeTargets、DependsOnTargets、Returns 和 Task 参数保持行为等价。
- Linked publish、Theme wrapper 生成、语言包 pack 和项目引用桥接仍在原有执行阶段运行。
- macOS NativeAOT fixture 和 Gallery 发布使用同一个平台 targets 文件。
- 仓库不存在旧 `build/*.props`、`build/*.targets` 和旧验证脚本路径引用。

## 测试设计

### 结构测试

在 `tests/AtomUI.Generator.Tests` 增加聚焦的 Build Infrastructure 测试，验证：

- 最终目录白名单和根目录无散文件。
- Repository 聚合入口的导入顺序。
- 所有 Import 路径存在且无循环。
- NuGet 资产清单完整、无重复、无越界文件。
- Generator 与产品包入口导入正确。
- Localization 聚合入口按固定顺序导入四个 fragment。
- Build Tasks 使用唯一属性，旧专用属性不再出现。
- `Output.App.props` 和全部旧路径已消失。

现有 Localization、Theme Asset 和 Linked Registration 构建资产测试迁移到新路径，并从检查旧文件布局改为检查职责、执行顺序和打包结果。

### 实际打包

至少实际打包：

- `AtomUI.Generator`
- 一个声明 `AtomUIRegistrationPackageId` 的代表性产品包

检查 `.nupkg`：

- 自动导入入口存在且名称正确。
- 内部子目录资产完整。
- Consumer target 只以产品 PackageId 命名一次。
- Generator、Build Tasks 及其依赖只位于预期的 analyzer/tools/buildTransitive 路径。
- Repository、Platform、Script 和旧路径均不存在。

### 功能与发布验证

```bash
dotnet test tests/AtomUI.Build.Tasks.Tests/AtomUI.Build.Tasks.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUI.Localization.IntegrationTests/AtomUI.Localization.IntegrationTests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --framework net10.0 --no-restore
scripts/verification/verify-aot-trim-registration.sh --quick
scripts/verification/verify-aot-trim-registration.sh --full
pwsh -NoLogo -NoProfile -File controlgallery/AtomUIGallery.Desktop/scripts/PublishToLocal.ps1 \
  -publishRootPath /tmp/atomui-gallery-aot-run \
  -runtime osx-arm64 \
  -buildType Release \
  -publishAot true
dotnet build AtomUI.slnx -c Release --no-restore
git diff --check
```

`--full` 必须覆盖 trimmed JIT、macOS NativeAOT、Browser AOT 和现有体积门槛。Gallery NativeAOT publish 用于验证真实应用的 Build Task、Theme Asset、Linked Registration 和发布路径，而不是只依赖 fixture。

## 文档治理

实现完成后更新 `docs/architecture/foundations/build-and-packaging.md`，使其成为以下长期规则的正式所有者：

- `build/` 的三类交付边界。
- Repository 和 NuGet 聚合入口。
- NuGet 资产新增规则。
- Build Tasks 单一路径属性。
- 验证脚本目录与发布验证要求。

涉及旧路径的 AOT、本地化、Changelog、Typography 和 Gallery 发布文档同步更新链接，但不重复目录治理规则。

## 验收标准

- `build/` 根目录只有 `repository/`、`nuget/` 和 `platforms/`。
- 非 MSBuild 脚本全部离开 `build/`。
- 不存在旧路径、旧辅助文件名、旧专用 Build Task 属性或兼容转发文件。
- `AtomUI.Localization.targets` 的四类职责由独立 fragment 拥有。
- Generator 包和产品包共享同一份资产清单。
- 结构测试、实际打包检查、目标测试、完整 AOT/Trim 验证、Gallery NativeAOT publish、Release build 和 `git diff --check` 全部通过。
