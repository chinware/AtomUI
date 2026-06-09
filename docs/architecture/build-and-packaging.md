# 构建与打包

AtomUI 使用集中化 MSBuild 配置。顶层 `Directory.Build.props` 引入版本、通用配置、包元信息和输出路径配置。

## Target Framework

`build/Common.props` 定义：

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

## 源生成输出

多个项目启用：

```xml
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
<CompilerGeneratedFilesOutputPath>GeneratedFiles</CompilerGeneratedFilesOutputPath>
<Compile Remove="$(CompilerGeneratedFilesOutputPath)/**/*.cs"/>
```

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

## 打包边界

README 中声明的主要包边界为：

- `AtomUI.Native`
- `AtomUI.Core`
- `AtomUI.Fonts.AlibabaSans`
- `AtomUI.Icons.Shared`
- `AtomUI.Icons.AntDesign`
- `AtomUI.Controls.Shared`
- `AtomUI.Controls`
- `AtomUI.Desktop.Controls`
- `AtomUI.Desktop.Controls.DataGrid`
- `AtomUI.Desktop.Controls.ColorPicker`
- `AtomUI.Generator`

DataGrid 和 ColorPicker 是独立按需包，但源码上依赖 `AtomUI.Desktop.Controls` 并访问其内部成员。

