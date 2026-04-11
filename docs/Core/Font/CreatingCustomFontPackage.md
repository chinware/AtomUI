# AtomUI 字体系统 — 制作自定义字体包

> 本文档面向希望制作自己的字体包的 **AtomUI 应用开发者**和 **库开发者**，提供从零开始创建字体包的完整步骤指南。模式完全对标 `AtomUI.Fonts.AlibabaSans` 和 Avalonia 官方的 `Avalonia.Fonts.Inter`。

---

## 1. 概述

自定义字体包将一组字体文件（`.ttf` / `.otf`）打包为独立的 NuGet 包或项目，使其可以：

- 在多个项目间共享。
- 通过一行代码注册到 `FontManager`。
- 与 AtomUI 主题系统无缝集成。
- 在 XAML 中通过自定义 URI Scheme 引用。

---

## 2. 步骤一：创建项目

### 2.1 创建类库项目

```bash
dotnet new classlib -n MyApp.Fonts.CustomFont
```

### 2.2 配置项目文件

编辑 `.csproj` 文件，添加 Avalonia 依赖和字体资源嵌入配置：

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFrameworks>net8.0;net10.0</TargetFrameworks>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>

    <!-- 关键！将字体文件嵌入为 Avalonia 资源 -->
    <ItemGroup>
        <AvaloniaResource Include="Assets/*" />
    </ItemGroup>

    <ItemGroup>
        <!-- 最小依赖：仅需要 Avalonia -->
        <PackageReference Include="Avalonia" Version="11.3.*" />
        
        <!-- 如果需要 AtomUI ThemeManagerBuilder 扩展，则添加 AtomUI.Core 依赖 -->
        <!-- <ProjectReference Include="../AtomUI.Core/AtomUI.Core.csproj" /> -->
    </ItemGroup>

</Project>
```

> **⚠️ 重要**：`<AvaloniaResource Include="Assets/*" />` 是必须的。没有这个配置，字体文件不会被嵌入到程序集中。

---

## 3. 步骤二：添加字体文件

在项目根目录创建 `Assets/` 文件夹，将字体文件放入：

```
MyApp.Fonts.CustomFont/
└── Assets/
    ├── CustomFont-Regular.ttf
    ├── CustomFont-Bold.ttf
    ├── CustomFont-Light.ttf
    └── CustomFont-Medium.ttf
```

**支持的格式：**
- TrueType Font (`.ttf`) ✅
- OpenType Font (`.otf`) ✅
- Variable Fonts — 暂不支持（参见 [Avalonia Issue #11092](https://github.com/AvaloniaUI/Avalonia/issues/11092)）

**确认字体族内部名称：** 用字体查看工具（如 macOS 的 Font Book、Windows 的字体查看器）打开字体文件，查看其 **Family Name** 元数据。这个名称将用于 URI 中的 `#` 后缀，必须**精确匹配**。

---

## 4. 步骤三：创建字体集合类

创建 `CustomFontCollection.cs`：

```csharp
using Avalonia.Media.Fonts;

namespace MyApp.Fonts.CustomFont;

public sealed class CustomFontCollection : EmbeddedFontCollection
{
    public CustomFontCollection() : base(
        new Uri("fonts:CustomFont", UriKind.Absolute),
        new Uri("avares://MyApp.Fonts.CustomFont/Assets", UriKind.Absolute))
    {
    }
}
```

**参数说明：**

| 参数 | 说明 | 示例 |
|------|------|------|
| 第一个 URI（自定义 Scheme） | 字体集合的标识键，用于 XAML 中引用 | `fonts:CustomFont` |
| 第二个 URI（资源路径） | 嵌入字体文件的 Avalonia 资源路径 | `avares://MyApp.Fonts.CustomFont/Assets` |

**命名约定：**
- URI Scheme 统一使用 `fonts:` 前缀。
- 集合键名推荐使用字体族名称（去空格），如 `fonts:SourceHanSans`、`fonts:Roboto`。
- 资源路径中的程序集名称必须与 `.csproj` 中的 `<AssemblyName>`（默认为项目名）完全匹配。

---

## 5. 步骤四：创建 AppBuilder 扩展方法

创建 `AppBuilderExtension.cs`，用于纯 Avalonia 场景：

```csharp
using Avalonia;
using MyApp.Fonts.CustomFont;

namespace MyApp.Fonts;

public static class AppBuilderExtension
{
    public static AppBuilder WithCustomFont(this AppBuilder appBuilder)
    {
        return appBuilder.ConfigureFonts(fontManager =>
        {
            fontManager.AddFontCollection(new CustomFontCollection());
        });
    }
}
```

**使用方式：**

```csharp
public static AppBuilder BuildAvaloniaApp() =>
    AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithCustomFont()
        .LogToTrace();
```

---

## 6. 步骤五：创建 ThemeManagerBuilder 扩展方法（AtomUI 集成）

如果你的字体包需要与 AtomUI 主题系统集成，创建 `ThemeManagerBuilderExtensions.cs`：

```csharp
using AtomUI.Theme;
using Avalonia.Media;
using MyApp.Fonts.CustomFont;

public static class CustomFontThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseCustomFont(this IThemeManagerBuilder themeManagerBuilder)
    {
        FontManager.Current.AddFontCollection(new CustomFontCollection());
        return themeManagerBuilder;
    }
}
```

**使用方式：**

```csharp
app.UseAtomUI(builder =>
{
    builder.UseCustomFont();         // 注册自定义字体包
    builder.UseDesktopControls();
});
```

> **注意**：添加此扩展方法需要项目引用 `AtomUI.Core`。如果字体包是纯 Avalonia 包（不依赖 AtomUI），可以省略此步骤。

---

## 7. 步骤六：添加 XAML 命名空间注册（可选）

如果希望字体包的类型能在 AXAML 中通过 `atom:` 或自定义前缀访问，创建 `Properties/AssemblyInfo.cs`：

```csharp
using Avalonia.Metadata;

[assembly: XmlnsDefinition("https://atomui.net", "MyApp.Fonts.CustomFont")]
```

对于纯 Avalonia 场景，可以映射到 Avalonia 命名空间：

```csharp
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "MyApp.Fonts.CustomFont")]
```

---

## 8. 步骤七：使用自定义字体

### 8.1 在 XAML 中直接引用

```xml
<TextBlock FontFamily="fonts:CustomFont#My Custom Font"
           FontSize="16"
           Text="Hello in Custom Font!" />
```

`#` 后面的名称必须与字体文件中的 Family Name 元数据一致。

### 8.2 设置为 AtomUI 默认字体

如果想将自定义字体设置为整个应用的默认字体（替代 Alibaba Sans），使用 `WithDefaultFontFamily()`：

```csharp
app.UseAtomUI(builder =>
{
    builder.UseCustomFont();         // 注册字体包
    builder.UseDesktopControls();
    
    // 将自定义字体设置为默认字体族
    builder.WithDefaultFontFamily(
        "fonts:CustomFont#My Custom Font, " +
        "Segoe UI, PingFang SC, Microsoft YaHei, $Default"
    );
});
```

这会在主题加载时将 `SharedTokenKind.FontFamily` Token 替换为你指定的字体族，所有使用 `{atom:SharedTokenResource FontFamily}` 的控件都会自动使用新字体。

### 8.3 同时注册多个字体包

```csharp
app.UseAtomUI(builder =>
{
    builder.UseAlibabaSansFont();   // 内置字体
    builder.UseCustomFont();         // 自定义字体
    builder.UseDesktopControls();
    
    // 选择其中一个作为默认字体
    builder.WithDefaultFontFamily("fonts:CustomFont#My Custom Font, $Default");
});
```

---

## 9. 完整文件结构

```
MyApp.Fonts.CustomFont/
├── CustomFontCollection.cs            # 字体集合（EmbeddedFontCollection 子类）
├── AppBuilderExtension.cs             # Avalonia AppBuilder 扩展方法
├── ThemeManagerBuilderExtensions.cs   # AtomUI ThemeManagerBuilder 扩展方法（可选）
├── MyApp.Fonts.CustomFont.csproj      # 项目文件
├── Properties/
│   └── AssemblyInfo.cs                # XAML 命名空间注册（可选）
└── Assets/
    ├── CustomFont-Regular.ttf
    ├── CustomFont-Bold.ttf
    ├── CustomFont-Light.ttf
    └── CustomFont-Medium.ttf
```

---

## 10. 发布为 NuGet 包（可选）

如果需要将字体包发布为 NuGet 包供多个项目使用：

### 10.1 配置包元数据

在 `.csproj` 中添加 NuGet 包配置：

```xml
<PropertyGroup>
    <PackageId>MyApp.Fonts.CustomFont</PackageId>
    <Version>1.0.0</Version>
    <Authors>YourName</Authors>
    <Description>Custom font package for Avalonia / AtomUI applications</Description>
    <PackageTags>Avalonia;AtomUI;Fonts</PackageTags>
</PropertyGroup>
```

### 10.2 打包发布

```bash
dotnet pack -c Release
dotnet nuget push bin/Release/MyApp.Fonts.CustomFont.1.0.0.nupkg -s https://api.nuget.org/v3/index.json
```

---

## 11. 高级：自定义字符匹配

如果你的字体包需要控制字符回退行为（例如，为特定语言的字符优先使用特定字体），可以重写 `TryMatchCharacter` 方法：

```csharp
using System.Globalization;
using Avalonia.Media;
using Avalonia.Media.Fonts;

public sealed class CustomFontCollection : EmbeddedFontCollection
{
    public CustomFontCollection() : base(
        new Uri("fonts:CustomFont", UriKind.Absolute),
        new Uri("avares://MyApp.Fonts.CustomFont/Assets", UriKind.Absolute))
    {
    }

    public override bool TryMatchCharacter(
        int codepoint,
        FontStyle fontStyle,
        FontWeight fontWeight,
        FontStretch fontStretch,
        CultureInfo? culture,
        out Typeface typeface)
    {
        // 自定义字符匹配逻辑
        // 例如：为 CJK 字符使用特定字体
        return base.TryMatchCharacter(
            codepoint, fontStyle, fontWeight, fontStretch,
            culture, out typeface);
    }
}
```

---

## 12. 检查清单

创建自定义字体包时，确认以下事项：

- [ ] `.csproj` 中包含 `<AvaloniaResource Include="Assets/*" />`
- [ ] 字体文件格式为 `.ttf` 或 `.otf`（不支持 Variable Fonts）
- [ ] `EmbeddedFontCollection` 中的 `avares://` URI 程序集名称正确
- [ ] `#` 后的字体族名称与字体文件元数据中的 Family Name 一致
- [ ] 提供了 `AppBuilder` 扩展方法（纯 Avalonia 场景）
- [ ] 提供了 `IThemeManagerBuilder` 扩展方法（AtomUI 场景，可选）
- [ ] 如果发布为 NuGet 包，字体许可证允许嵌入分发
- [ ] 在目标平台（Windows/macOS/Linux）上测试字体渲染效果

