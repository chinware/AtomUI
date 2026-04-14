# 技术栈清单

## 1. 编程语言

| 语言 | 版本 | 用途 |
|------|------|------|
| **C#** | 13.0+（.NET 10 默认） | 所有业务逻辑、ViewModel、View code-behind |
| **XAML** | Avalonia XAML | UI 布局定义、样式、主题 |
| **PowerShell** | — | 发布脚本 `PublishToLocal.ps1` |
| **XML** | — | MSBuild 配置、安装程序配置、应用清单 |

## 2. 框架与运行时

| 框架 | 版本 | 用途 |
|------|------|------|
| **.NET** | 10.0 (主目标) | 核心运行时 |
| **.NET** | 8.0 (多目标) | Desktop 项目兼容性支持 |
| **Avalonia UI** | 11.3.12 | 跨平台 UI 框架 |
| **Avalonia.Desktop** | — | 桌面平台支持 |

## 3. UI 技术

| 技术 | 说明 |
|------|------|
| **Avalonia XAML** | 声明式 UI 定义，类似 WPF XAML 但跨平台 |
| **TemplatedControl** | AtomUI 控件基类，支持 ControlTheme 模板化 |
| **ControlTheme** | Avalonia 的样式系统，替代 WPF 的 Style+Template |
| **StyledProperty** | Avalonia 依赖属性等价物 |
| **DirectProperty** | 高性能非跟踪属性 |
| **Avalonia Markup Extensions** | `{Binding}`, `{x:Static}`, `{CaseNavigationLangResource}` 等 |

## 4. MVVM 框架

| 框架 | 版本 | 用途 |
|------|------|------|
| **ReactiveUI** | 通过 Avalonia 集成 | 核心 MVVM 框架 |
| **ReactiveUI.Avalonia** | — | Avalonia 平台集成 |
| **CommunityToolkit.Mvvm** | — | 辅助 MVVM 工具（部分使用） |

### ReactiveUI 核心使用

| 功能 | 使用方式 |
|------|---------|
| `ReactiveObject` | 所有 ViewModel 基类 |
| `RaiseAndSetIfChanged` | 属性变更通知 |
| `IScreen` / `RoutingState` | 导航路由 |
| `IRoutableViewModel` | 可路由 ViewModel |
| `IActivatableViewModel` / `ViewModelActivator` | 生命周期管理 |
| `ReactiveWindow<T>` | 强类型 Window |
| `ReactiveUserControl<T>` | 强类型 UserControl |
| `WhenActivated` | View 激活回调 |
| `Locator` | Service Locator（ViewModel 注册/解析） |

## 5. Source Generators

| Generator | 所属包 | 生成内容 |
|-----------|--------|---------|
| **AtomUI.Generator.LanguageGenerator** | AtomUI.Generator | 语言资源枚举、XAML 扩展、LanguageProviderPool |
| **AtomUI.Generator.IconGenerator** | AtomUI.Generator | 图标提供者枚举、图标加载代码 |
| **AtomUI.Generator.TokenResourceKeyGenerator** | AtomUI.Generator | Token 资源键常量 |
| **AvaloniaNameSourceGenerator** | Avalonia.Generators | XAML x:Name 强类型访问器 |

### Source Generator 配置

```xml
<PropertyGroup>
    <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
    <CompilerGeneratedFilesOutputPath>GeneratedFiles</CompilerGeneratedFilesOutputPath>
</PropertyGroup>

<ItemGroup>
    <Compile Remove="$(CompilerGeneratedFilesOutputPath)/**/*.cs"/>
</ItemGroup>
```

- `EmitCompilerGeneratedFiles` — 将生成文件写入磁盘（便于调试）
- `CompilerGeneratedFilesOutputPath` — 生成文件输出目录
- `Compile Remove` — 排除生成文件参与编译（避免重复编译）

## 6. 构建工具

| 工具 | 用途 |
|------|------|
| **MSBuild** | 构建引擎 |
| **dotnet CLI** | 命令行构建/运行/发布 |
| **Directory.Build.props** | 全局 MSBuild 属性（目标框架、版本号等） |
| **Directory.Packages.props** | 中央包版本管理（CPM） |

### 构建配置层级

```
src/Directory.Build.props          # 全局属性
src/Directory.Packages.props       # 全局包版本
src/Version.props                  # 版本号定义
src/Common.props                   # 通用属性
controlgallery/Directory.Build.props  # Gallery 特定属性（可能覆盖全局）
```

## 7. AtomUI 组件库依赖

| 包 | 用途 |
|----|------|
| `AtomUI.Desktop.Controls` | 桌面控件（Button, Menu, NavMenu 等） |
| `AtomUI.Desktop.Controls.DataGrid` | DataGrid 数据表格控件 |
| `AtomUI.Desktop.Controls.ColorPicker` | ColorPicker 颜色选择器控件 |
| `AtomUI.Icons.AntDesign` | Ant Design 图标库 |
| `AtomUI.Icons.Shared` | 图标共享基础设施 |
| `AtomUI.Generator` | Source Generator（Analyzer） |

## 8. 第三方 NuGet 包

| 包 | 用途 |
|----|------|
| `Avalonia` | UI 框架核心 |
| `Avalonia.Desktop` | 桌面平台支持 |
| `Avalonia.Diagnostics` | DevTools（Debug only） |
| `ReactiveUI` | 响应式 MVVM 框架 |
| `ReactiveUI.Avalonia` | ReactiveUI Avalonia 集成 |
| `CommunityToolkit.Mvvm` | MVVM 工具包 |

## 9. 测试框架

| 框架 | 用途 |
|------|------|
| **xUnit** | 单元测试框架（AtomUIGallery.Tests 项目） |

> **注意**：当前测试项目内容较少，Gallery 主要依赖手动测试和 F5 自动导航测试。

## 10. 发布与部署

| 工具 | 用途 |
|------|------|
| **dotnet publish** | 单文件发布 |
| **PublishToLocal.ps1** | 自动化发布脚本 |
| **WiX** | Windows 安装程序 |
| **DMG** | macOS 安装镜像 |
| **AppImage** | Linux 安装包 |

### 发布配置

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
    <UseAppHost>true</UseAppHost>
</PropertyGroup>
<PropertyGroup>
    <BuiltInComInteropSupport>true</BuiltInComInteropSupport>
    <ApplicationManifest>app.manifest</ApplicationManifest>
</PropertyGroup>
```

- `UseAppHost` — 生成平台特定的可执行文件
- `BuiltInComInteropSupport` — 内置 COM 互操作支持
- `ApplicationManifest` — Windows 应用清单（UAC 等）
- `Roots.xml` — Trimmer 根描述符（防止裁剪关键类型）

### AOT 与 Trimming

当前 AOT 和 Trimming 被注释掉：

```xml
<!-- <IsTrimmable>true</IsTrimmable> -->
<!-- <PublishTrimmed>true</PublishTrimmed> -->
<!-- <PublishAot>true</PublishAot> -->
```

> **风险**：Avalonia + ReactiveUI + 反射依赖使得 AOT/Trimming 支持需要额外工作。

## 11. 日志、DI、序列化

| 功能 | 使用情况 |
|------|---------|
| **日志** | 未使用专门的日志框架 |
| **依赖注入** | 使用 ReactiveUI 的 `Locator` 作为轻量级 DI |
| **序列化** | 未使用 |
| **配置** | 硬编码（无 appsettings.json 等） |