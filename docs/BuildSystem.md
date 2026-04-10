# AtomUI 构建系统架构与规范

> 本文档描述 AtomUI 项目的 MSBuild 构建系统架构、目录布局、属性配置层级、多目标框架策略、NuGet 打包与发布流程，以及开发者日常构建指南。

---

## 1. 构建系统总览

AtomUI 使用 **MSBuild** 作为构建引擎，通过分层的 `.props` 文件实现配置的集中管理和复用。

### 1.1 构建技术栈

| 组件 | 技术 | 说明 |
|---|---|---|
| **构建引擎** | MSBuild（.NET SDK 风格） | 所有项目使用 `<Project Sdk="Microsoft.NET.Sdk">` |
| **解决方案格式** | `.slnx`（XML 格式） | `AtomUI.slnx`，新式解决方案格式 |
| **SDK 版本控制** | `global.json` | 指定最低 SDK 版本，配合 `rollForward: latestMajor` |
| **包管理** | Central Package Management (CPM) | `Directory.Packages.props` 集中管理所有 NuGet 包版本 |
| **源码生成** | Roslyn Source Generators | `AtomUI.Generator` 项目，以 Analyzer 方式引用 |
| **版本管理** | 集中式 `Version.props` | 所有版本号统一定义，详见 [VersioningAndBranching.md](./VersioningAndBranching.md) |

### 1.2 构建文件层级

```
AtomUI/
├── global.json                    # .NET SDK 版本约束
├── AtomUI.slnx                    # 解决方案定义（XML 格式）
├── Directory.Build.props          # 全局构建属性（MSBuild 自动导入）
├── Directory.Build.targets        # 全局构建目标（MSBuild 自动导入）
├── Directory.Packages.props       # Central Package Management（CPM）
├── build/
│   ├── Version.props              # 版本号定义（Avalonia、AtomUI、Gallery）
│   ├── Common.props               # 通用编译选项（TFM、警告、输出类型等）
│   ├── Output.props               # 统一输出路径（bin、obj、NuGet 包）
│   ├── Output.App.props           # Gallery 应用专用输出路径
│   └── PackageMetaInfo.props      # NuGet 包元数据（作者、许可证、图标等）
├── src/                           # 源码项目（各自 .csproj）
├── controlgallery/                # Gallery 演示应用项目
├── tests/                         # 测试项目
├── output/                        # 统一构建输出目录（不入版本控制）
└── scripts/
    └── PublishToLocalSources.ps1  # 本地 NuGet 发布脚本
```

---

## 2. MSBuild 属性文件详解

### 2.1 属性导入链

MSBuild 在构建每个项目时，会按以下顺序自动导入属性：

```
① global.json                        → 确定 SDK 版本
② Directory.Build.props (仓库根)      → MSBuild 自动发现并导入
    ├── build/Version.props           → 版本号
    ├── build/Common.props            → 编译选项
    ├── build/PackageMetaInfo.props   → NuGet 元数据
    └── build/Output.props            → 输出路径
③ Directory.Packages.props           → CPM 包版本（MSBuild 自动发现）
④ 各项目 .csproj                      → 项目特定配置
⑤ Directory.Build.targets (仓库根)   → 全局构建目标
```

### 2.2 `global.json` — SDK 版本约束

```json
{
  "sdk": {
    "version": "8.0.300",
    "rollForward": "latestMajor"
  }
}
```

| 字段 | 值 | 说明 |
|---|---|---|
| `version` | `8.0.300` | 最低要求的 SDK 版本 |
| `rollForward` | `latestMajor` | 允许自动向上滚动到已安装的最高大版本（如 .NET 10 SDK） |

**设计意图**：最低门槛设为 .NET 8 SDK（生产目标框架），但开发者若安装了 .NET 10 SDK 则自动使用最新版本，兼顾生产兼容性与开发体验。

### 2.3 `Directory.Build.props` — 全局构建属性

```xml
<Project>
    <PropertyGroup>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
        <LangVersion>latest</LangVersion>
    </PropertyGroup>

    <Import Project="$(MSBuildThisFileDirectory)/build/Version.props"/>
    <Import Project="$(MSBuildThisFileDirectory)/build/Common.props"/>
    <Import Project="$(MSBuildThisFileDirectory)/build/PackageMetaInfo.props"/>
    <Import Project="$(MSBuildThisFileDirectory)/build/Output.props"/>
</Project>
```

**全局编译设置**：
- **Nullable**：全局启用可空引用类型
- **ImplicitUsings**：启用隐式 using（`System`, `System.Collections.Generic` 等）
- **LangVersion**：始终使用最新 C# 语言版本

**导入顺序**：Version → Common → PackageMetaInfo → Output，确保后续文件可以引用前面定义的属性（如 `$(AtomUIVersion)`）。

### 2.4 `build/Version.props` — 版本号定义

```xml
<Project>
    <PropertyGroup>
        <NoWarn>$(NoWarn);CS7035</NoWarn>
        <AvaloniaVersion>11.3.12</AvaloniaVersion>
        <AtomUIVersion>5.2.0-build.3</AtomUIVersion>
        <AtomUIGalleryVersion>5.2.0-build.3</AtomUIGalleryVersion>
    </PropertyGroup>
</Project>
```

| 属性 | 用途 | 消费位置 |
|---|---|---|
| `AvaloniaVersion` | Avalonia 框架版本 | `Directory.Packages.props` 中所有 Avalonia 包的版本 |
| `AtomUIVersion` | AtomUI 库版本 | NuGet 包版本、`PackageMetaInfo.props`、Assembly 元数据 |
| `AtomUIGalleryVersion` | Gallery 应用版本 | Gallery 项目版本（与 `AtomUIVersion` 保持同步） |

> `CS7035` 警告抑制：允许版本字符串中包含预发布标签（如 `-build.3`）。

**规则**：
- `AtomUIVersion` 和 `AtomUIGalleryVersion` **必须保持同步**
- 修改版本号时 **只修改此文件**，不允许在 `.csproj` 中硬编码版本
- 版本号规范详见 [VersioningAndBranching.md](./VersioningAndBranching.md)

### 2.5 `build/Common.props` — 通用编译选项

```xml
<Project>
    <PropertyGroup>
        <AtomUIDevelopTargetFramework>net10.0</AtomUIDevelopTargetFramework>
        <AtomUIProductionTargetFramework>net8.0</AtomUIProductionTargetFramework>

        <AtomUITargetFrameworks Condition=" '$(Configuration)' == 'Debug' ">
            $(AtomUIDevelopTargetFramework)
        </AtomUITargetFrameworks>
        <AtomUITargetFrameworks Condition=" '$(Configuration)' == 'Release' ">
            $(AtomUIDevelopTargetFramework);$(AtomUIProductionTargetFramework)
        </AtomUITargetFrameworks>

        <WarningsAsErrors>Nullable</WarningsAsErrors>
        <AvaloniaAccessUnstablePrivateApis>true</AvaloniaAccessUnstablePrivateApis>
        <!-- ...其他选项... -->
    </PropertyGroup>
</Project>
```

#### 多目标框架 (Multi-Targeting) 策略

| 配置 (Configuration) | 目标框架 (TargetFrameworks) | 说明 |
|---|---|---|
| `Debug` | `net10.0` | 仅编译开发框架，**加速日常开发构建** |
| `Release` | `net10.0;net8.0` | 同时编译开发和生产框架，**确保双 TFM 兼容** |

**设计意图**：
- 开发者日常编码只编译 `net10.0`，构建速度更快
- 发布时自动多目标编译，保证 NuGet 包同时支持 .NET 8 和 .NET 10 消费者
- 所有 `src/` 项目通过 `<TargetFrameworks>$(AtomUITargetFrameworks)</TargetFrameworks>` 统一引用此变量

#### 关键编译选项

| 属性 | 值 | 说明 |
|---|---|---|
| `OutputType` | `Library` | 默认输出类型为类库 |
| `WarningsAsErrors` | `Nullable` | 可空引用类型警告视为错误 |
| `ProduceReferenceAssembly` | `false` | 不生成引用程序集（减少输出） |
| `AppendRuntimeIdentifierToOutputPath` | `false` | 输出路径不附加 RID |
| `AccelerateBuildsInVisualStudio` | `true` | 启用 VS/Rider 增量构建优化 |
| `AvaloniaAccessUnstablePrivateApis` | `true` | 允许访问 Avalonia 内部不稳定 API |
| `NoWarn` | `CS1591;CS0436;CS7035;AVA3001` | 抑制特定警告 |

#### 测试项目自动检测

```xml
<IsTestProject Condition="$(MSBuildProjectFullPath.Contains('test')) and 
    ($(MSBuildProjectName.EndsWith('.Tests')) or 
     $(MSBuildProjectName.EndsWith('.TestBase')))">true</IsTestProject>
```

路径包含 `test` 且项目名以 `.Tests` 或 `.TestBase` 结尾的项目自动标记为测试项目。

### 2.6 `build/Output.props` — 统一输出路径

```xml
<Project>
    <PropertyGroup>
        <PackageOutputPath>$(MSBuildThisFileDirectory)../output/Nuget/$(Configuration)</PackageOutputPath>
        <OutputPath>$(MSBuildThisFileDirectory)../output/bin/$(Configuration)</OutputPath>
        <BaseIntermediateOutputPath>$(MSBuildThisFileDirectory)../output/$(MSBuildProjectName)/obj</BaseIntermediateOutputPath>
        <IntermediateOutputPath>$(BaseIntermediateOutputPath)/$(Configuration)</IntermediateOutputPath>
    </PropertyGroup>
</Project>
```

所有构建产物统一输出到仓库根 `output/` 目录：

```
output/
├── bin/
│   └── Debug/                     # 所有项目的 Debug 编译输出
│       ├── net10.0/
│       └── net8.0/                # 仅 Release 时存在
├── Nuget/
│   └── Release/                   # dotnet pack 输出的 .nupkg 文件
├── AtomUI.Core/
│   └── obj/                       # AtomUI.Core 的中间文件
├── AtomUI.Controls/
│   └── obj/                       # AtomUI.Controls 的中间文件
└── ...                            # 每个项目一个 obj 子目录
```

**设计意图**：
- 将所有 `bin/` 和 `obj/` 移出源码树，保持 `src/` 目录整洁
- NuGet 包输出到独立的 `output/Nuget/` 目录，便于发布脚本定位
- `output/` 目录整体不纳入版本控制（通过 `.gitignore` 排除）

### 2.7 `build/PackageMetaInfo.props` — NuGet 包元数据

```xml
<Project>
    <PropertyGroup>
        <PackageId>$(MSBuildProjectName)</PackageId>
        <Title>AtomUI</Title>
        <Author>Qinware Technologies Ltd.</Author>
        <Version>$(AtomUIVersion)</Version>
        <PackageIcon>logo.png</PackageIcon>
        <PackageReadmeFile>README.nuget.md</PackageReadmeFile>
        <PackageLicenseFile>LICENSE</PackageLicenseFile>
        <!-- ...其他元数据... -->
    </PropertyGroup>

    <ItemGroup>
        <None Include="$(MSBuildThisFileDirectory)../resources/logo.png" Pack="True" PackagePath="/"/>
        <None Include="$(MSBuildThisFileDirectory)../LICENSE" Pack="True" PackagePath="/"/>
        <None Include="$(MSBuildThisFileDirectory)../README.nuget.md" Pack="True" PackagePath="/"/>
    </ItemGroup>
</Project>
```

所有 `src/` 项目自动继承统一的 NuGet 元数据：
- `PackageId` 默认为项目名（如 `AtomUI.Core`、`AtomUI.Desktop.Controls`）
- `Version` 引用 `$(AtomUIVersion)`，确保所有包版本一致
- Logo、README、LICENSE 文件自动打入每个 NuGet 包

### 2.8 `Directory.Build.targets` — 全局构建目标

```xml
<Project>
    <ItemGroup>
        <None Remove="*.csproj.DotSettings"/>
    </ItemGroup>
</Project>
```

排除 ReSharper/Rider 的 `.csproj.DotSettings` 文件，避免它们被 MSBuild 处理。

---

## 3. Central Package Management (CPM)

AtomUI 使用 [Central Package Management](https://learn.microsoft.com/nuget/consume-packages/central-package-management) 集中管理所有 NuGet 依赖版本。

### 3.1 `Directory.Packages.props` 结构

```xml
<Project>
    <PropertyGroup>
        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    </PropertyGroup>

    <!-- 外部依赖 -->
    <ItemGroup>
        <PackageVersion Include="Avalonia" Version="$(AvaloniaVersion)"/>
        <PackageVersion Include="ReactiveUI.Avalonia" Version="11.4.12"/>
        <PackageVersion Include="System.Reactive" Version="6.1.0"/>
        <PackageVersion Include="xunit.v3" Version="3.2.2"/>
        <!-- ...更多包... -->
    </ItemGroup>

    <!-- AtomUI 自身包（仅 Release 配置） -->
    <ItemGroup Condition=" '$(Configuration)' == 'Release' ">
        <PackageVersion Include="AtomUI.Core" Version="$(AtomUIVersion)"/>
        <PackageVersion Include="AtomUI.Desktop.Controls" Version="$(AtomUIVersion)"/>
        <!-- ...更多 AtomUI 包... -->
    </ItemGroup>
</Project>
```

### 3.2 依赖分类

| 类别 | 包 | 说明 |
|---|---|---|
| **Avalonia 框架** | `Avalonia`, `Avalonia.Desktop`, `Avalonia.Diagnostics`, `Avalonia.Controls.ColorPicker` | 版本统一由 `$(AvaloniaVersion)` 控制 |
| **响应式编程** | `ReactiveUI.Avalonia`, `System.Reactive` | MVVM 和响应式支持 |
| **UI 扩展** | `Svg.Controls.Avalonia`, `SkiaSharp.QrCode` | SVG 渲染和二维码生成 |
| **代码生成** | `Microsoft.CodeAnalysis.CSharp`, `Microsoft.CodeAnalysis.Analyzers` | Roslyn 源码生成器依赖 |
| **测试** | `Microsoft.NET.Test.Sdk`, `xunit.v3`, `NSubstitute`, `Shouldly` | 测试框架和工具 |
| **Gallery 应用** | `CommunityToolkit.Mvvm` | Gallery 应用的 MVVM 工具包 |
| **AtomUI 自身包** | `AtomUI.Core`, `AtomUI.Desktop.Controls` 等 | 仅 Release 配置下注册（用于 Gallery 的 NuGet 包引用模式） |

### 3.3 Debug/Release 双模式引用

Gallery 项目使用一种巧妙的双模式引用策略：

```xml
<!-- Debug：直接引用源码项目（快速编译、可调试） -->
<ItemGroup Condition=" '$(Configuration)' == 'Debug' ">
    <ProjectReference Include="../../src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj"/>
</ItemGroup>

<!-- Release：引用已发布的 NuGet 包（验证打包正确性） -->
<ItemGroup Condition=" '$(Configuration)' == 'Release' ">
    <PackageReference Include="AtomUI.Desktop.Controls"/>
</ItemGroup>
```

**设计意图**：
- **Debug 模式**：直接引用项目源码，支持断点调试和即时修改反馈
- **Release 模式**：切换为 NuGet 包引用，**验证 NuGet 包的完整性和正确性**，模拟真实用户的消费方式

---

## 4. 解决方案与项目组织

### 4.1 `AtomUI.slnx` 解决方案结构

```
AtomUI.slnx
├── /（根文件夹）                     # 源码库项目
│   ├── AtomUI.Native                 # 平台原生互操作
│   ├── AtomUI.Core                   # 核心基础设施
│   ├── AtomUI.Controls.Shared        # 共享接口和枚举
│   ├── AtomUI.Controls               # 设备无关的基础控件
│   ├── AtomUI.Desktop.Controls       # 桌面平台控件
│   ├── AtomUI.Desktop.Controls.ColorPicker  # 颜色选择器扩展
│   ├── AtomUI.Desktop.Controls.DataGrid     # 数据网格扩展
│   ├── AtomUI.Icons.Shared           # 图标基础设施
│   ├── AtomUI.Icons.AntDesign        # Ant Design 图标集
│   ├── AtomUI.Icons.AntDesign.Generator     # 图标代码生成工具
│   ├── AtomUI.Fonts.AlibabaSans      # 阿里巴巴普惠体字体
│   └── AtomUI.Generator              # Roslyn 源码生成器
├── /Gallery/                          # 演示应用
│   ├── AtomUIGallery                  # Gallery 共享层
│   ├── AtomUIGallery.Desktop          # Gallery 桌面入口
│   └── AtomUIGallery.Icons.Desktop    # Gallery 自定义图标
└── /Tests/                            # 测试项目
    └── AtomUI.Generator.Tests         # 源码生成器测试
```

### 4.2 项目依赖图

```
AtomUI.Desktop.Controls.ColorPicker ─┐
AtomUI.Desktop.Controls.DataGrid ────┤
                                     ├→ AtomUI.Desktop.Controls
                                     │     ├→ AtomUI.Controls
                                     │     │     ├→ AtomUI.Core ──→ AtomUI.Native
                                     │     │     ├→ AtomUI.Controls.Shared ──→ AtomUI.Core
                                     │     │     ├→ AtomUI.Icons.AntDesign ──→ AtomUI.Icons.Shared ──→ AtomUI.Core
                                     │     │     └→ AtomUI.Fonts.AlibabaSans ──→ AtomUI.Core
                                     │     └→ AtomUI.Generator (Analyzer)
                                     └→ AtomUI.Generator (Analyzer)

AtomUIGallery.Desktop ──→ AtomUIGallery
                              ├→ AtomUI.Desktop.Controls (Debug: ProjectRef / Release: PackageRef)
                              ├→ AtomUI.Desktop.Controls.DataGrid (同上)
                              ├→ AtomUI.Desktop.Controls.ColorPicker (同上)
                              └→ AtomUIGallery.Icons.Desktop ──→ AtomUI.Core
```

### 4.3 源码生成器的特殊引用方式

`AtomUI.Generator` 是 Roslyn Source Generator 项目，使用特殊的引用方式：

```xml
<ProjectReference Include="../AtomUI.Generator/AtomUI.Generator.csproj"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false"
                  PrivateAssets="all" />
```

| 属性 | 值 | 说明 |
|---|---|---|
| `OutputItemType` | `Analyzer` | 将此项目作为 Analyzer（而非普通依赖）加载 |
| `ReferenceOutputAssembly` | `false` | 不引用其输出程序集（源码生成器在编译时运行，不是运行时依赖） |
| `PrivateAssets` | `all` | 不传递此依赖（消费者不需要源码生成器） |

`AtomUI.Generator` 自身的特殊配置：

```xml
<TargetFramework>netstandard2.0</TargetFramework>    <!-- 源码生成器必须目标 netstandard2.0 -->
<IsRoslynComponent>true</IsRoslynComponent>           <!-- 标记为 Roslyn 组件 -->
<SuppressDependenciesWhenPacking>true</SuppressDependenciesWhenPacking>
<IncludeBuildOutput>false</IncludeBuildOutput>         <!-- 包中不含标准输出 -->

<!-- 打包时将 DLL 放入 analyzers 路径 -->
<None Include="$(OutputPath)/$(AssemblyName).dll" 
      Pack="true" PackagePath="analyzers/dotnet/cs" />
```

### 4.4 生成文件处理

使用源码生成器的项目统一配置：

```xml
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
<CompilerGeneratedFilesOutputPath>GeneratedFiles</CompilerGeneratedFilesOutputPath>

<ItemGroup>
    <Compile Remove="$(CompilerGeneratedFilesOutputPath)/**/*.cs"/>
</ItemGroup>
```

- 生成的文件输出到项目内 `GeneratedFiles/` 目录（仅供开发者查看/调试）
- 通过 `<Compile Remove>` 避免双重编译（生成器已在编译管线中注入了这些代码）

---

## 5. InternalsVisibleTo 关系

AtomUI 通过 `InternalsVisibleTo` 在不暴露公共 API 的前提下，允许特定项目访问内部成员：

```
AtomUI.Native
  └→ AtomUI.Core, AtomUI.Desktop.Controls, AtomUI.Mobile.Controls

AtomUI.Core
  └→ AtomUI.Controls, AtomUI.Controls.Shared
  └→ AtomUI.Desktop.Controls, AtomUI.Desktop.Controls.DataGrid, AtomUI.Desktop.Controls.ColorPicker

AtomUI.Controls.Shared
  └→ AtomUI.Controls
  └→ AtomUI.Desktop.Controls, AtomUI.Desktop.Controls.DataGrid, AtomUI.Desktop.Controls.ColorPicker

AtomUI.Controls
  └→ AtomUI.Desktop.Controls, AtomUI.Desktop.Controls.DataGrid, AtomUI.Desktop.Controls.ColorPicker

AtomUI.Desktop.Controls
  └→ AtomUI.Desktop.Controls.DataGrid, AtomUI.Desktop.Controls.ColorPicker
```

**规则**：`InternalsVisibleTo` 的方向必须与项目依赖方向一致，上层可以暴露给下游，但 **不得** 形成反向或循环依赖。

---

## 6. 日常开发构建指南

### 6.1 环境要求

| 要求 | 最低版本 | 推荐版本 |
|---|---|---|
| .NET SDK | 8.0.300 | 10.0 最新 |
| IDE | JetBrains Rider 2024.3+ 或 Visual Studio 2022 17.12+ | Rider 最新 |
| OS | Windows 10+ / macOS 12+ / Linux (glibc 2.17+) | — |

### 6.2 常用命令

```bash
# 日常开发构建（仅编译 net10.0，速度快）
dotnet build

# Release 构建（编译 net10.0 + net8.0）
dotnet build -c Release

# 运行 Gallery 应用
dotnet run --project controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj

# 运行测试
dotnet test

# 创建 NuGet 包
dotnet pack -c Release

# 清理所有构建产物
dotnet clean
rm -rf output/
```

### 6.3 本地 NuGet 发布

`scripts/PublishToLocalSources.ps1` 脚本实现了按依赖顺序的本地 NuGet 发布：

```powershell
# 在 PowerShell 中运行（默认推送到 D:/nuget.local）
cd scripts
./PublishToLocalSources.ps1

# 自定义本地源路径
./PublishToLocalSources.ps1 -localSourcesDir "/path/to/local/nuget"
```

**构建顺序**（按依赖链从底层到上层）：

```
第一轮（基础层）:
  AtomUI.Native → AtomUI.Core → AtomUI.Fonts.AlibabaSans
  → AtomUI.Controls.Shared → AtomUI.Desktop.Controls
  → AtomUI.Generator → AtomUI.Icons.Shared → AtomUI.Icons.AntDesign
  → 推送到本地 NuGet 源

第二轮（扩展层，依赖第一轮的包）:
  AtomUI.Desktop.Controls.DataGrid
  → AtomUI.Desktop.Controls.ColorPicker
  → 推送到本地 NuGet 源
```

分两轮发布是因为 DataGrid 和 ColorPicker 扩展包在 Release 模式下依赖已发布的 `AtomUI.Desktop.Controls` NuGet 包。

---

## 7. 特殊项目配置

### 7.1 `AtomUI.Generator` — 源码生成器

| 属性 | 值 | 原因 |
|---|---|---|
| `TargetFramework` | `netstandard2.0` | Roslyn 分析器/生成器必须目标 netstandard2.0 |
| `IsRoslynComponent` | `true` | 标记为 Roslyn 组件，启用源码生成器工具链 |
| `IncludeBuildOutput` | `false` | NuGet 包中不包含标准 `lib/` 输出 |
| `PackagePath` | `analyzers/dotnet/cs` | DLL 打包到 NuGet 的分析器路径 |

### 7.2 `AtomUI.Core` — 核心库

```xml
<!-- 嵌入版本元数据到程序集 -->
<AssemblyMetadata Include="AvaloniaVersion" Value="$(AvaloniaVersion)" />
<AssemblyMetadata Include="AtomUIVersion" Value="$(AtomUIVersion)" />

<!-- 仅 Debug 配置引入诊断工具 -->
<PackageReference Condition="'$(Configuration)' == 'Debug'" Include="Avalonia.Diagnostics"/>
```

- 通过 `AssemblyMetadata` 将版本信息烙入程序集，运行时可查询
- `Avalonia.Diagnostics`（开发者工具）仅在 Debug 模式下引用，不随 NuGet 包分发

### 7.3 `AtomUI.Icons.AntDesign.Generator` — 图标生成工具

```xml
<OutputType>Exe</OutputType>
```

这是唯一一个 `OutputType=Exe` 的源码项目（非 Gallery），用于从 Ant Design SVG 图标源文件生成 C# 代码。作为独立的命令行工具运行，不作为 NuGet 包发布。

### 7.4 `AtomUIGallery.Desktop` — Gallery 应用入口

```xml
<OutputType>WinExe</OutputType>
<ApplicationManifest>app.manifest</ApplicationManifest>
```

- `WinExe`：Windows 上隐藏控制台窗口
- 包含跨平台图标资源处理逻辑（Windows `.ico`、macOS `.icns`、Linux `hicolor` 图标集）
- Release 配置预留了 `PublishTrimmed` 和 `PublishAot` 注释（待未来启用）

---

## 8. 构建规范与约定

### 8.1 新增项目检查清单

添加新的 `src/` 项目时，需遵循以下步骤：

1. **创建 `.csproj`**：使用 `<TargetFrameworks>$(AtomUITargetFrameworks)</TargetFrameworks>` 引用统一 TFM
2. **添加到 `AtomUI.slnx`**：在适当的文件夹分组中注册项目
3. **设置 `RootNamespace`**：明确指定根命名空间
4. **配置 InternalsVisibleTo**：按架构层级配置内部可见性
5. **注册到 CPM**：若新项目产出 NuGet 包，在 `Directory.Packages.props` 的 Release 条件组中添加 `<PackageVersion>` 条目
6. **更新发布脚本**：在 `scripts/PublishToLocalSources.ps1` 中按依赖顺序添加 `dotnet build` 和 `dotnet pack` 命令
7. **源码生成器引用**：若项目使用 `AtomUI.Generator`，添加 Analyzer 方式的 `ProjectReference` 并配置 `GeneratedFiles` 排除规则

### 8.2 禁止事项

- ❌ 在 `.csproj` 中硬编码版本号 — 必须引用 `$(AtomUIVersion)`
- ❌ 在 `.csproj` 中直接指定 `<PackageReference ... Version="x.y.z"/>` — 必须通过 CPM 管理
- ❌ 将构建产物（`bin/`、`obj/`、`output/`）提交到版本控制
- ❌ 在 `AtomUI.Generator` 中引用非 `netstandard2.0` 兼容的依赖
- ❌ 违反项目层级依赖方向（如 `AtomUI.Core` 引用 `AtomUI.Controls`）

### 8.3 编译警告策略

| 类别 | 处理方式 | 说明 |
|---|---|---|
| **Nullable 警告** | 视为 **错误** (`WarningsAsErrors`) | 强制可空安全 |
| `CS1591` (缺少 XML 注释) | **抑制** | 不强制所有成员都有文档注释 |
| `CS0436` (类型冲突) | **抑制** | 源码生成器可能产生的类型冲突 |
| `CS7035` (版本字符串) | **抑制** | 允许预发布版本标签 |
| `AVA3001` (Avalonia 标记) | **抑制** | Avalonia XAML 相关警告 |

---

## 9. 构建文件索引

| 文件 | 路径 | 职责 |
|---|---|---|
| `global.json` | 仓库根 | .NET SDK 版本约束 |
| `AtomUI.slnx` | 仓库根 | 解决方案定义 |
| `Directory.Build.props` | 仓库根 | 全局属性入口（自动导入） |
| `Directory.Build.targets` | 仓库根 | 全局构建目标（自动导入） |
| `Directory.Packages.props` | 仓库根 | NuGet 包版本集中管理 |
| `build/Version.props` | `build/` | 版本号定义 |
| `build/Common.props` | `build/` | 通用编译选项和 TFM |
| `build/Output.props` | `build/` | 统一构建输出路径 |
| `build/Output.App.props` | `build/` | Gallery 应用输出路径 |
| `build/PackageMetaInfo.props` | `build/` | NuGet 包元数据 |
| `scripts/PublishToLocalSources.ps1` | `scripts/` | 本地 NuGet 发布脚本 |

