# 第三方 AtomUI Control Package 指南

> 状态：截至 2026-08-15，本文步骤对应已实现并验证的接入方式。普通第三方包默认使用 `Package` 粒度；只有明确承担发布
> 体积与回归验证的大型包才需要显式启用 `Directory`。

本文面向准备发布 AtomUI Control NuGet 的作者。目标是让你的控件同时支持普通应用、trimming、NativeAOT 和 WebAssembly
AOT，而不需要维护 linker XML、运行时反射或内部 Registration Unit 图。

## 先理解一个简单模型

普通第三方包只需要理解 Package：

```text
Acme.Controls
├── 你编写的 Control、Token 和 Theme
├── 一个公开 UseAcmeControls() 入口
└── Generator 编译期生成的注册代码
```

默认情况下，整个 `Acme.Controls` 是一个安全注册单元。应用使用包内任一 Control 时，linked publish 会保留这个包中的
Control descriptor、Own Token、内部 View/Presenter 和主题资源。

应用仍显式调用 `UseAcmeControls()`。Generator 只决定 linked publish 需要保留哪些静态内容，不会运行时扫描程序集，也不会
擅自启用一个可选包。

## 最小目录结构

一个只提供 `Rating` 的包可以这样组织：

```text
Acme.Controls/
├── Acme.Controls.csproj
├── Rating/
│   ├── Rating.cs
│   ├── RatingToken.cs              可选
│   └── Themes/
│       └── RatingTheme.axaml
├── AcmeControlThemesProvider.cs
└── ThemeManagerBuilderExtensions.cs
```

不需要创建聚合 AXAML、Theme manifest、Unit 文件或 linker descriptor。

## 第一步：配置项目

包项目声明一个稳定 Package identity：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>

    <AtomUIRegistrationPackageId>Acme.Controls</AtomUIRegistrationPackageId>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AtomUI.Desktop.Controls" />
  </ItemGroup>
</Project>
```

如果项目没有使用 Central Package Management，请填写目标 AtomUI 产品包的版本号。产品包会自动携带同版本 Generator、
Build Tasks 和 `buildTransitive` 资产；普通第三方作者不需要再引用 `AtomUI.Generator`。这些工具只参与编译，不会进入你的
运行时依赖、应用输出或 publish 目录。

只有不引用任何 AtomUI 产品包的底层构建场景，才需要把兼容版本 `AtomUI.Generator` 显式作为 private Analyzer 引入；这不是
普通 Control Package 的接入路径。

`AtomUIRegistrationPackageId` 是包在注册协议中的稳定身份。通常让它与 NuGet `PackageId` 和程序集身份一致；发布后不要随意
修改。它不包含入口类型名、方法名或 Unit 名。

默认粒度不需要写出来：

```xml
<!-- 默认即为 Package，普通第三方包不要添加这一行也可以。 -->
<AtomUIRegistrationGranularity>Package</AtomUIRegistrationGranularity>
```

## 第二步：按约定编写 Control 和 Theme

Control 使用正常的 public Avalonia 类型：

```csharp
using Avalonia.Controls.Primitives;

namespace Acme.Controls;

public class Rating : TemplatedControl
{
}
```

只有控件确实拥有不能由 Global Token 表达的设计值时，才添加 Own Token：

```csharp
using AtomUI.Theme.DesignTokens;

namespace Acme.Controls;

[ControlDesignToken]
internal sealed class RatingToken : AbstractControlDesignToken
{
    public double StarSize { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        StarSize = EffectiveGlobalToken.ControlHeight;
    }
}
```

主题放在控件目录的 `Themes/` 下：

```xml
<ResourceDictionary
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:acme="using:Acme.Controls"
    x:ClassModifier="internal">
  <ControlTheme x:Key="{x:Type acme:Rating}"
                TargetType="acme:Rating">
    <!-- 正常编写模板和强类型 TokenResource。 -->
  </ControlTheme>
</ResourceDictionary>
```

Generator 会从真实 CLR 类型、Token 和 AXAML 生成 identity、descriptor、资源键、Theme Asset manifest、full registrar 和
linked registration metadata。不要为每个 Theme 再写一份 C# 注册代码。

## 第三步：提供 Theme Provider

Provider 的 ID 与 Package identity 保持一致：

```csharp
using AtomUI.Theme.Resources;

namespace Acme.Controls;

internal sealed class AcmeControlThemesProvider : ControlThemesProvider
{
    public AcmeControlThemesProvider()
    {
        Id = ThemeManagerBuilderExtensions.PackageId;
    }
}
```

Package ID 应只在入口类型中声明一次，Provider 复用该常量，避免两份字符串失配。

## 第四步：提供公开注册入口

入口是应用启用这个可选包的唯一公开动作：

```csharp
using AtomUI.Generated.AcmeControls;
using AtomUI.Registration;

namespace Acme.Controls;

public static class ThemeManagerBuilderExtensions
{
    internal const string PackageId = "Acme.Controls";

    [ControlPackageRegistrationEntry]
    public static IAtomUIBuilder UseAcmeControls(
        this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var provider = new AcmeControlThemesProvider();
        if (AotTrimRegistration.IsEnabled)
        {
            AotTrimRegistrationPlanRegistry.ApplyPackage(
                builder,
                PackageId,
                provider);
        }
        else
        {
            GeneratedControlPackageRegistration.Register(
                builder.Theme,
                provider);
        }

        return builder;
    }
}
```

`AtomUI.Generated.AcmeControls` 由程序集名 `Acme.Controls` 折叠得到。生成命名空间和类型在编译期可用，不需要把生成文件提交
到 Git。

`[ControlPackageRegistrationEntry]` 只声明编译期入口身份。方法必须满足：

- public、static、非泛型。
- 是 `IAtomUIBuilder` 扩展方法。
- 第一个参数是 `this IAtomUIBuilder`。
- 返回值可以赋给 `IAtomUIBuilder`。
- 同一包含类型中没有无法唯一识别的同名重载。

无效声明会在包自身构建时报告 `ATOMUILINK009`，不会推迟到消费应用发布时才失败。

如果包还注册 Localization、initializer 或其他 Package Core 内容，把它们保留在这个方法中，并保持普通路径与 generated
路径的相同顺序。Generator 不解析方法体，也不会猜测你的初始化顺序。

## 第五步：应用显式启用

应用安装 NuGet 后，在构建 `ThemeManager` 前调用入口：

```csharp
this.UseAtomUI(builder =>
{
    builder.UseDesktopControls();
    builder.UseAcmeControls();
});
```

应用写 AXAML 或 C# 时正常使用控件：

```xml
<acme:Rating />
```

普通构建会执行完整包级注册。Trimmed、NativeAOT 或 WebAssembly AOT 构建会由应用 Generator 生成静态调用计划，但应用代码
和入口调用不需要切换。

## 普通作者不需要做什么

不要添加下面这些内容：

```text
AtomUIRegistrationEntries
AtomUIRegistrationUnit
AtomUIRegistrationUnitRoot
AtomUIPackageRoot
AvaloniaXaml Update="..." ownership 修补
Unit dependency 列表
linker XML
运行时 Assembly.GetTypes() 扫描
手工 Control descriptor 或 Theme manifest
```

其中 `AtomUIRegistrationUnitRoot` 和 `AtomUIPackageRoot` 是消费应用处理真正动态输入的高级开关，不是 Control Package 的正常
接入配置。

## 什么时候才使用 Directory 模式

只有一个 NuGet 中包含大量相互独立的公开控件族，并且完整 Package Unit 的发布体积已经成为可重复测量的问题时，才考虑：

```xml
<PropertyGroup>
  <AtomUIRegistrationGranularity>Directory</AtomUIRegistrationGranularity>
</PropertyGroup>
```

启用后，稳定控件族目录才成为可独立裁剪的 Unit。此时必须满足：

- 每个目录代表真正可以独立运行的公开控件族，而不只是 `Cell`、`Utils`、`View` 等内部代码分类。
- 内部 Presenter、Cell、View、Track 和主题跟随其公开控件族。
- AXAML 和 C# 中的跨 Unit 使用能够被 Generator 证明；不确定性必须允许 Package full fallback。
- 只有无法从 public owner 推导的 resource-only Theme 才使用 `AtomUIRegistrationUnit`。
- 只有真正跨多个 Unit 且必须随 Package Core 加载的资源才使用 `AtomUIPackageSharedTheme`。
- CI 覆盖 generated registration、trimmed JIT、NativeAOT 和体积对比。

Directory 模式是高级体积优化，不是 AOT 正确性的前置条件。无法证明它有收益时，继续使用默认 Package 模式。

## 发布前检查

至少确认：

- 普通构建调用 `UseAcmeControls()` 后，所有 Control、Token 和主题正常。
- generated registration 模式的 descriptor、Theme Asset、Provider、Localization 和 initializer 与普通模式一致。
- 包内 Control 在 C# 中创建的内部 View/Presenter 仍有完整主题。
- Generator 和 Build Tasks 没有进入应用 runtime dependency、普通输出或 publish 目录。
- 声明 AOT 兼容前，真实 trimmed JIT 和 NativeAOT 应用可以启动并显示控件。
- `git diff --check` 和包自身测试通过。

系统级模式、fallback 和诊断契约见
[AOT 与裁剪架构](../../architecture/foundations/aot-and-trimming.md)。Token 与主题作者规则见
[Control Token 设计规范](../../engineering/development/control-token-guidelines.md)。Package/Directory 粒度和资源归属的正式契约见
[AOT Registration Unit 粒度](../../architecture/foundations/aot-registration-unit-granularity.md)。
