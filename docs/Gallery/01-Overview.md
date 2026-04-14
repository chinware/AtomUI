# AtomUIGallery 项目概览

## 1. 项目简介

AtomUIGallery 是 **AtomUI** 组件库的官方演示程序（Gallery），用于展示和测试 AtomUI 提供的所有 UI 控件、布局、主题和国际化功能。它同时也是一个**开发调试工具**，支持运行时切换主题、语言、动效等。

## 2. 项目组成

Gallery 由 **3 个 .NET 项目** 组成：

| 项目 | 路径 | 类型 | 目标框架 | 职责 |
|------|------|------|----------|------|
| **AtomUIGallery** | `controlgallery/AtomUIGallery` | 类库 (Library) | `net10.0` | 核心逻辑：ShowCase、Workspace、国际化、自定义控件 |
| **AtomUIGallery.Desktop** | `controlgallery/AtomUIGallery.Desktop` | 可执行程序 (WinExe) | `net10.0; net8.0` | 桌面平台入口：Program.cs、应用配置、发布脚本 |
| **AtomUIGallery.Icons.Desktop** | `controlgallery/AtomUIGallery.Icons.Desktop` | 类库 (Library) | `net10.0` | 桌面图标资源包：自定义图标 Provider |

### 项目依赖关系

```
AtomUIGallery.Desktop (WinExe - 启动项目)
    ├── AtomUIGallery (ProjectReference)
    └── [平台特定资源: .icns/.ico/.png]

AtomUIGallery (核心类库)
    ├── AtomUIGallery.Icons.Desktop (ProjectReference)
    ├── AtomUI.Desktop.Controls (Debug: ProjectReference / Release: PackageReference)
    ├── AtomUI.Desktop.Controls.DataGrid (Debug: ProjectReference / Release: PackageReference)
    ├── AtomUI.Desktop.Controls.ColorPicker (Debug: ProjectReference / Release: PackageReference)
    ├── AtomUI.Generator (Source Generator - Analyzer)
    ├── Avalonia (NuGet)
    ├── Avalonia.Diagnostics (Debug only)
    └── CommunityToolkit.Mvvm (NuGet)

AtomUIGallery.Icons.Desktop (图标资源)
    ├── AtomUI.Icons.AntDesign (Debug: ProjectReference / Release: PackageReference)
    ├── AtomUI.Icons.Shared (Debug: ProjectReference / Release: PackageReference)
    └── AtomUI.Generator (Source Generator - Analyzer)
```

> **重要**：Debug 配置使用 `ProjectReference` 引用本地 AtomUI 源码，Release 配置使用 `PackageReference` 引用 NuGet 包。这是通过 `.csproj` 中的 `Condition` 实现的。

## 3. 完整目录树

### 3.1 AtomUIGallery（核心项目）

```
AtomUIGallery/
├── AtomUIGallery.csproj
├── BaseGalleryApplication.axaml              # 应用程序 XAML 定义
├── BaseGalleryApplication.axaml.cs           # 应用程序基类
├── ThemeManagerBuilderExtensions.cs          # 主题管理器扩展方法
│
├── Assets/                                   # 资源文件
│   └── AtomUIGallery.ico                     # 应用图标
│
├── Controls/                                 # 自定义 Gallery 控件
│   ├── ColorItemControl.cs                   # 颜色项控件
│   ├── ColorItemControlTheme.axaml           # 颜色项控件主题
│   ├── ColorListControl.cs                   # 颜色列表控件
│   ├── ColorListControlTheme.axaml           # 颜色列表控件主题
│   ├── GalleryControlThemesProvider.axaml    # Gallery 控件主题提供者
│   ├── GalleryControlThemesProvider.cs       # Gallery 控件主题提供者代码
│   ├── IconGallery.axaml.cs                  # 图标展示控件
│   ├── IconGalleryTheme.axaml                # 图标展示控件主题
│   ├── IconInfoItem.axaml.cs                 # 图标信息项控件
│   ├── IconInfoItemTheme.axaml               # 图标信息项控件主题
│   ├── ShowCaseItem.axaml.cs                 # ShowCase 项控件
│   ├── ShowCaseItemTheme.axaml               # ShowCase 项控件主题
│   ├── ShowCasePanel.axaml.cs                # ShowCase 面板控件
│   └── ShowCasePanelTheme.axaml              # ShowCase 面板控件主题
│
├── GeneratedFiles/                           # Source Generator 生成文件
│   ├── AtomUI.Generator/
│   │   ├── AtomUI.Generator.LanguageGenerator/
│   │   │   ├── LanguageProviderPool.g.cs     # 语言提供者池（自动生成）
│   │   │   └── LanguageResourceConst.g.cs    # 语言资源常量枚举（自动生成）
│   │   └── AtomUI.Generator.TokenResourceKeyGenerator/
│   │       └── TokenResourceConst.g.cs       # Token 资源键常量（自动生成）
│   └── Avalonia.Generators/
│       └── Avalonia.Generators.NameGenerator.AvaloniaNameSourceGenerator/
│           ├── *.g.cs                        # 72+ 个 View 的 x:Name 强类型访问器
│           └── ...
│
├── Models/                                   # 数据模型
│
├── Properties/                               # 项目属性
│
├── ShowCases/                                # ★ ShowCase 系统（核心）
│   ├── ViewModels/                           # ViewModel 层
│   │   ├── DataDisplay/                      # 数据展示类 (21个)
│   │   ├── DataEntry/                        # 数据录入类 (18个)
│   │   ├── Feedback/                         # 反馈类 (11个)
│   │   ├── General/                          # 通用类 (9个)
│   │   ├── Layout/                           # 布局类 (5个)
│   │   └── Navigation/                       # 导航类 (8个)
│   └── Views/                                # View 层
│       ├── DataDisplay/                      # 数据展示类 ShowCase
│       ├── DataEntry/                        # 数据录入类 ShowCase
│       ├── Feedback/                         # 反馈类 ShowCase
│       ├── General/                          # 通用类 ShowCase
│       ├── Layout/                           # 布局类 ShowCase
│       └── Navigation/                       # 导航类 ShowCase
│
├── Utils/                                    # 工具类
│   ├── EnumExtension.cs                      # 枚举扩展方法
│   └── LinuxDistributionDetector.cs          # Linux 发行版检测
│
└── Workspace/                                # ★ Workspace 系统（主窗口）
    ├── ViewModels/
    │   ├── CaseNavigationViewModel.cs         # 导航面板 ViewModel
    │   └── WorkspaceWindowViewModel.cs        # 主窗口 ViewModel
    ├── Views/
    │   ├── CaseNavigation.axaml(.cs)          # 导航面板 View
    │   └── WorkspaceWindow.axaml(.cs)         # 主窗口 View
    └── Localization/                          # Workspace 国际化
        ├── CaseNavigationLang/
        │   ├── en_US.cs                       # 导航英文翻译
        │   └── zh_CN.cs                       # 导航中文翻译
        └── WorkspaceWindowLang/
            ├── en_US.cs                       # 窗口英文翻译
            └── zh_CN.cs                       # 窗口中文翻译
```

### 3.2 AtomUIGallery.Desktop（桌面入口项目）

```
AtomUIGallery.Desktop/
├── AtomUIGallery.Desktop.csproj              # 项目文件
├── GalleryApplication.cs                     # 应用程序入口类
├── Program.cs                                # Main 入口点
├── app.manifest                              # Windows 应用清单
├── Roots.xml                                 # Trimmer 根描述符
│
├── Assets/                                   # 平台特定资源
│   └── Images/
│       ├── AtomUIGallery.ico                 # Windows 图标
│       ├── AtomUIGallery.icns                # macOS 图标
│       ├── AppLogo.iconset/                  # macOS 图标集
│       ├── DmgInstallerBg@2x.png             # DMG 安装背景
│       ├── Wix/                              # Windows 安装程序图片
│       │   ├── InstallerBanner.bmp
│       │   └── InstallerWizard.bmp
│       └── AtomUIGalleryInstaller.ico        # 安装程序图标
│
├── configs/                                  # 安装程序配置
│   ├── InstallerConfig.appimage.xml          # Linux AppImage 配置
│   ├── InstallerConfig.dmg.xml               # macOS DMG 配置
│   └── InstallerConfig.wix.xml               # Windows WiX 配置
│
└── scripts/                                  # 发布脚本
    └── PublishToLocal.ps1                    # 本地发布 PowerShell 脚本
```

### 3.3 AtomUIGallery.Icons.Desktop（图标资源项目）

```
AtomUIGallery.Icons.Desktop/
├── AtomUIGallery.Icons.Desktop.csproj        # 项目文件
├── GalleryIconProvider.cs                    # 图标提供者实现
│
├── Assets/                                   # 图标 SVG 资源
│   └── (SVG 图标文件)
│
├── GeneratedFiles/                           # Source Generator 生成
│   └── AtomUI.Generator/
│       └── AtomUI.Generator.IconGenerator/
│           └── IconProvider.g.cs             # 图标提供者生成代码
│
└── Properties/                               # 项目属性
```

## 4. 构建与运行

### 4.1 前置条件

- **.NET SDK 10.0**（主目标框架）
- **.NET SDK 8.0**（Desktop 项目多目标支持）
- 操作系统：Windows / macOS / Linux

### 4.2 构建命令

```bash
# Debug 模式（使用本地 ProjectReference）
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj

# Release 模式（使用 NuGet PackageReference）
dotnet build -c Release controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj

# 运行
dotnet run --project controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj
```

### 4.3 发布

```powershell
# 使用发布脚本（macOS arm64 示例）
cd controlgallery/AtomUIGallery.Desktop/scripts
./PublishToLocal.ps1 -publishRootPath "D:/publish" -buildType "Release" -framework "net10.0" -runtime "osx-arm64"
```

支持的 Runtime Identifier：
- `osx-arm64` — macOS Apple Silicon
- `win-x64` — Windows 64-bit
- `linux-x64` — Linux 64-bit

### 4.4 目标框架

目标框架由 `$(AtomUITargetFrameworks)` MSBuild 属性控制，定义在 `Directory.Build.props` 中，当前值为 `net10.0;net8.0`（Desktop 项目）和 `net10.0`（核心项目）。

## 5. 应用启动流程

```
Program.Main()
  └→ BuildAvaloniaApp()
       └→ AppBuilder.Configure<GalleryApplication>()
            └→ GalleryApplication (继承 BaseGalleryApplication)
                 └→ OnFrameworkInitializationCompleted()
                      └→ new WorkspaceWindow() { DataContext = new WorkspaceWindowViewModel() }
                           └→ window.Show()
```

1. `Program.cs` — 创建并配置 Avalonia `AppBuilder`
2. `GalleryApplication.cs` — 平台特定的 Application 子类
3. `BaseGalleryApplication` — 通用 Application 基类，初始化主题和语言系统
4. `WorkspaceWindow` — 主窗口，承载导航面板和内容区域