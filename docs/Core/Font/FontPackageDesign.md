# AtomUI 字体系统 — 字体包内部设计原理

> 本文档以 `AtomUI.Fonts.AlibabaSans` 为例，详细描述 AtomUI 字体包的内部设计模式，解释其每个源文件的职责和设计决策，并对比 Avalonia 官方的 `Avalonia.Fonts.Inter` 字体包。

---

## 1. 设计理念

### 1.1 为什么需要字体包

在跨平台桌面应用中，不同操作系统的默认系统字体各不相同：

| 平台 | 默认 UI 字体 |
|------|-------------|
| Windows | Segoe UI |
| macOS | San Francisco (PingFang SC) |
| Linux | 取决于发行版配置 |

为了保证应用在所有平台上呈现一致的视觉效果，AtomUI 采用字体嵌入策略：将字体文件直接打包到应用程序集中，运行时由 Avalonia 的 `FontManager` 加载，不依赖用户系统上安装的字体。

### 1.2 设计模式：EmbeddedFontCollection

AtomUI 字体包遵循 Avalonia 定义的 **嵌入字体集合（Embedded Font Collection）** 模式。该模式的核心是：

1. 将字体文件标记为 `AvaloniaResource` 嵌入到程序集中。
2. 创建 `EmbeddedFontCollection` 子类，将自定义 URI Scheme（如 `fonts:AlibabaSans`）映射到嵌入资源路径。
3. 通过 `FontManager.AddFontCollection()` 注册字体集合。
4. 在 XAML 或代码中通过 `fonts:AlibabaSans#Alibaba Sans` 格式引用字体。

这与 Avalonia 官方的 `Avalonia.Fonts.Inter` 字体包（将 Inter 字体以 `fonts:Inter#Inter` 方式注册）采用完全相同的模式。

---

## 2. 项目结构分析

```
AtomUI.Fonts.AlibabaSans/
├── AlibabaSansFontCollection.cs     # 字体集合定义
├── AppBuilderExtension.cs           # Avalonia AppBuilder 扩展（纯 Avalonia 用法）
├── ThemeManagerBuilderExtensions.cs  # AtomUI ThemeManagerBuilder 扩展（AtomUI 主题用法）
├── AtomUI.Fonts.AlibabaSans.csproj  # 项目文件
├── Properties/
│   └── AssemblyInfo.cs              # XAML 命名空间注册
└── Assets/
    ├── AlibabaSans-Black.ttf        # 900 weight
    ├── AlibabaSans-Bold.ttf         # 700 weight
    ├── AlibabaSans-Heavy.ttf        # 800 weight
    ├── AlibabaSans-Light.ttf        # 300 weight
    ├── AlibabaSans-Medium.ttf       # 500 weight
    └── AlibabaSans-Regular.ttf      # 400 weight
```

---

## 3. 核心源文件详解

### 3.1 AlibabaSansFontCollection.cs — 字体集合

```csharp
using Avalonia.Media.Fonts;

namespace AtomUI.Fonts.AlibabaSans;

public class AlibabaSansFontCollection : EmbeddedFontCollection
{
    public AlibabaSansFontCollection() : base(
        new Uri("fonts:AlibabaSans", UriKind.Absolute), 
        new Uri("avares://AtomUI.Fonts.AlibabaSans/Assets", UriKind.Absolute))
    {
    }
}
```

**设计要点：**

- **继承 `EmbeddedFontCollection`** — Avalonia 提供的嵌入字体集合基类，处理字体文件的发现、加载、缓存。
- **第一个 URI（`fonts:AlibabaSans`）** — 自定义 URI Scheme，作为字体集合的标识键。在代码或 XAML 中引用字体时使用此前缀。
- **第二个 URI（`avares://AtomUI.Fonts.AlibabaSans/Assets`）** — Avalonia 资源协议路径，指向程序集中嵌入的字体文件目录。`avares://` 是 Avalonia 内置的资源访问协议，格式为 `avares://程序集名称/路径`。
- `EmbeddedFontCollection` 会自动扫描指定目录下的所有字体文件，根据字体元数据中的 Family Name 进行匹配。

**与 Avalonia.Fonts.Inter 的对比：**

| 属性 | AtomUI (AlibabaSans) | Avalonia (Inter) |
|------|---------------------|------------------|
| 自定义 URI | `fonts:AlibabaSans` | `fonts:Inter` |
| 资源路径 | `avares://AtomUI.Fonts.AlibabaSans/Assets` | `avares://Avalonia.Fonts.Inter/Assets` |
| 字重数量 | 6 种 (Light ~ Black) | 视 Inter 包版本而定 |

### 3.2 AppBuilderExtension.cs — Avalonia AppBuilder 扩展

```csharp
using Avalonia;

namespace AtomUI.Fonts.AlibabaSans;

public static class AppBuilderExtension
{
    public static AppBuilder WithAlibabaSansFont(this AppBuilder appBuilder)
    {
        return appBuilder.ConfigureFonts(fontManager =>
        {
            fontManager.AddFontCollection(new AlibabaSansFontCollection());
        });
    }
}
```

**设计要点：**

- 提供 `AppBuilder` 扩展方法 `WithAlibabaSansFont()`，遵循 Avalonia 的 Fluent API 风格。
- 用于**纯 Avalonia 场景**（不使用 AtomUI 主题系统时），在 `AppBuilder` 链中调用。
- 内部通过 `ConfigureFonts` 回调将字体集合注册到 `FontManager`。

**使用示例（纯 Avalonia）：**

```csharp
public static AppBuilder BuildAvaloniaApp() =>
    AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithAlibabaSansFont()  // 注册 Alibaba Sans 字体
        .LogToTrace();
```

### 3.3 ThemeManagerBuilderExtensions.cs — AtomUI 主题扩展

```csharp
using AtomUI.Fonts.AlibabaSans;
using AtomUI.Theme;
using Avalonia.Media;

public static class AlibabaSansThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseAlibabaSansFont(this IThemeManagerBuilder themeManagerBuilder)
    {
        FontManager.Current.AddFontCollection(new AlibabaSansFontCollection());
        return themeManagerBuilder;
    }
}
```

**设计要点：**

- 提供 `IThemeManagerBuilder` 扩展方法 `UseAlibabaSansFont()`，遵循 AtomUI 的 Builder 模式。
- 用于 **AtomUI 主题场景**，在 `UseAtomUI()` 回调中调用。
- 直接通过 `FontManager.Current` 注册字体集合（此时 Avalonia 已初始化，`FontManager.Current` 可用）。

**使用示例（AtomUI）：**

```csharp
app.UseAtomUI(builder =>
{
    builder.UseAlibabaSansFont();  // 注册 Alibaba Sans 字体
    builder.UseDesktopControls();
});
```

**为什么有两种注册方式？**

| 注册方式 | 场景 | 时机 |
|---------|------|------|
| `AppBuilder.WithAlibabaSansFont()` | 纯 Avalonia 应用 | `AppBuilder` 链式调用阶段，Avalonia 初始化前 |
| `IThemeManagerBuilder.UseAlibabaSansFont()` | AtomUI 主题应用 | `UseAtomUI()` 回调阶段，Avalonia 已初始化 |

两种方式的最终效果相同：都是将 `AlibabaSansFontCollection` 注册到 `FontManager`。区别在于调用时机和 API 风格的匹配。

### 3.4 AtomUI.Fonts.AlibabaSans.csproj — 项目文件

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFrameworks>$(AtomUITargetFrameworks)</TargetFrameworks>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>

    <ItemGroup>
        <AvaloniaResource Include="Assets/*" />
    </ItemGroup>

    <ItemGroup>
        <ProjectReference Include="../AtomUI.Core/AtomUI.Core.csproj" />
    </ItemGroup>
</Project>
```

**设计要点：**

- **`<AvaloniaResource Include="Assets/*" />`** — 关键配置！将 `Assets/` 目录下的所有文件标记为 Avalonia 嵌入资源。编译时这些 `.ttf` 文件会嵌入到程序集中，运行时通过 `avares://` 协议访问。
- **依赖 `AtomUI.Core`** — 因为 `ThemeManagerBuilderExtensions` 使用了 `IThemeManagerBuilder` 接口（定义在 `AtomUI.Core` 中）。
- **`$(AtomUITargetFrameworks)`** — 使用中央管理的目标框架（Debug: `net10.0`; Release: `net10.0;net8.0`）。

### 3.5 Properties/AssemblyInfo.cs — XAML 命名空间

```csharp
using Avalonia.Metadata;

[assembly: XmlnsDefinition("https://atomui.net", "AtomUI.Fonts.AlibabaSans")]
```

**设计要点：**

- 将 `AtomUI.Fonts.AlibabaSans` C# 命名空间映射到 `https://atomui.net` XAML 命名空间。
- 这使得在 XAML 中使用 `atom:` 前缀即可引用此命名空间下的类型（如果需要）。
- 与 Avalonia 的 Inter 字体包对比：Inter 映射到 `https://github.com/avaloniaui`，而 AtomUI 使用自己的命名空间 `https://atomui.net`。

### 3.6 Assets/ — 字体文件

包含 6 个字重的 Alibaba Sans 字体文件：

| 文件 | 字重 | FontWeight 值 |
|------|------|-------------|
| `AlibabaSans-Light.ttf` | Light | 300 |
| `AlibabaSans-Regular.ttf` | Regular / Normal | 400 |
| `AlibabaSans-Medium.ttf` | Medium | 500 |
| `AlibabaSans-Bold.ttf` | Bold | 700 |
| `AlibabaSans-Heavy.ttf` | Heavy / ExtraBold | 800 |
| `AlibabaSans-Black.ttf` | Black | 900 |

Alibaba Sans（阿里巴巴普惠体）是阿里巴巴集团开源的免费商用字体，支持中文、英文、数字等多种字符集，是 Ant Design 5.0 生态中推荐的界面字体之一。

---

## 4. 字体引用格式

注册字体集合后，在代码或 XAML 中引用字体的完整格式为：

```
fonts:AlibabaSans#Alibaba Sans
```

- `fonts:AlibabaSans` — URI Scheme + 集合键名（对应 `EmbeddedFontCollection` 构造函数的第一个 URI）。
- `#Alibaba Sans` — 字体族内部名称（Font Family Name），必须与 `.ttf` 文件中的元数据 Family Name 一致，**不是**文件名。

> ⚠️ **注意**: `#` 后面的字体族名称是必须的。如果省略，Avalonia 无法定位字体，会静默回退到默认字体。

### 4.1 AtomUI 的默认字体族 Fallback 链

在 `DesignToken.InitSeedTokenValues()` 中定义了完整的字体族回退链：

```csharp
FontFamily = FontFamily.Parse(
    "fonts:AlibabaSans#Alibaba Sans, " +  // 首选：嵌入的 Alibaba Sans
    "Segoe UI, " +                         // Windows 回退
    "Segoe UI Symbol, " +                  // Windows 符号字符回退
    "Helvetica Neue, " +                   // macOS 回退
    "Noto Sans, " +                        // Linux 回退
    "Noto Sans CJK SC, " +                // Linux CJK 回退
    "文泉驿正黑, " +                        // Linux 中文回退
    "Microsoft YaHei, " +                  // Windows 中文回退
    "PingFang SC, " +                      // macOS 中文回退
    "$Default"                             // 系统默认字体（终极回退）
);
```

这个 Fallback 链确保：

1. 如果 Alibaba Sans 字体包已注册且包含所需字符 → 使用 Alibaba Sans。
2. 如果 Alibaba Sans 中缺少某些字符（如特殊符号）→ 按顺序回退到系统字体。
3. `$Default` 是 Avalonia 的特殊标记，表示系统默认字体，作为终极保底。

---

## 5. 与 Avalonia.Fonts.Inter 的架构对比

| 维度 | AtomUI.Fonts.AlibabaSans | Avalonia.Fonts.Inter |
|------|-------------------------|---------------------|
| **字体集合类** | `AlibabaSansFontCollection` | `InterFontCollection` |
| **URI Scheme** | `fonts:AlibabaSans` | `fonts:Inter` |
| **资源路径** | `avares://AtomUI.Fonts.AlibabaSans/Assets` | `avares://Avalonia.Fonts.Inter/Assets` |
| **AppBuilder 扩展** | `WithAlibabaSansFont()` | `WithInterFont()` |
| **主题扩展** | `UseAlibabaSansFont()` (IThemeManagerBuilder) | 无（Avalonia 无主题构建器） |
| **XAML 命名空间** | `https://atomui.net` | `https://github.com/avaloniaui` |
| **项目依赖** | `AtomUI.Core`（需要 IThemeManagerBuilder） | `Avalonia.Base` + `Avalonia.Controls` |

**关键区别：** AtomUI 字体包提供了额外的 `IThemeManagerBuilder` 扩展方法，使字体注册能够融入 AtomUI 的 Builder 模式链，而 Avalonia 的 Inter 包只提供 `AppBuilder` 扩展。

---

## 6. 设计原则总结

1. **模式复用** — 完全遵循 Avalonia 的 `EmbeddedFontCollection` 模式，确保与 Avalonia 生态兼容。
2. **双入口 API** — 同时提供 `AppBuilder` 和 `IThemeManagerBuilder` 两种注册入口，覆盖纯 Avalonia 和 AtomUI 两种使用场景。
3. **资源嵌入** — 通过 `AvaloniaResource` MSBuild Item 将字体文件编译进程序集，确保跨平台部署无需额外文件。
4. **Fallback 策略** — 在 Design Token 层面定义完整的字体族回退链，兼顾嵌入字体首选和系统字体兜底。
5. **命名空间注册** — 通过 `XmlnsDefinition` 将字体包命名空间纳入 AtomUI XAML 命名空间体系。

