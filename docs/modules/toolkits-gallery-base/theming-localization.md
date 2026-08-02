# GalleryBase 主题与本地化设计

本文档细化 GalleryBase 如何注册 AtomUI 主题资源、Control Token、XAML namespace 和 Shell 级语言资源。GalleryBase 可以使用 AtomUI 作为 UI 实现，但主题和语言边界必须保持产品中立。

## 设计目标

- GalleryBase 拥有自己的 Token 和主题，不借用 `AtomUIGallery` 命名空间。
- 产品页面继续使用产品自己的语言资源，不进入 GalleryBase。
- Shell 文案由 GalleryBase 提供，产品品牌和页面文案由产品提供。
- Control、可选 Own Token 和主题资产遵循 AtomUI 约定，并由生成器建立 descriptor 与 manifest。
- 整个 GalleryBase 包只使用一次生成的注册入口，不逐 Control 或逐 Theme 手工注册。
- 支持 Browser 和 Desktop 共用同一套主题 Provider。

## XAML namespace

GalleryBase 使用独立 XAML namespace：

```csharp
[assembly: XmlnsPrefix("https://atomui.net/toolkits/gallery-base", "gallery")]
[assembly: XmlnsDefinition("https://atomui.net/toolkits/gallery-base", "AtomUI.Toolkits.GalleryBase")]
[assembly: XmlnsDefinition("https://atomui.net/toolkits/gallery-base", "AtomUI.Toolkits.GalleryBase.Controls")]
[assembly: XmlnsDefinition("https://atomui.net/toolkits/gallery-base", "AtomUI.Toolkits.GalleryBase.Controls.DesignTokens")]
[assembly: XmlnsDefinition("https://atomui.net/toolkits/gallery-base", "AtomUI.Toolkits.GalleryBase.Models")]
[assembly: XmlnsDefinition("https://atomui.net/toolkits/gallery-base", "AtomUI.Toolkits.GalleryBase.Shell")]
```

产品 XAML 使用：

```xml
xmlns:gallery="https://atomui.net/toolkits/gallery-base"
```

`AtomUIGallery.ShowCases.*` 不再放进 GalleryBase 的 XML namespace 定义。产品自己的 ViewModel namespace 继续使用 `using:` 或产品自有 XML namespace。

## 主题注册入口

```csharp
public static class ThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseGalleryBase(
        this IThemeManagerBuilder builder,
        Action<GalleryBaseOptions>? configure = null);
}
```

入口职责：

- 注册 GalleryBase 生成的 exact CLR type/identity Control descriptor、ControlTheme asset manifest 和主题 Provider。
- 注册 GalleryBase 语言 Provider 池。
- 构建并保存 `GalleryBaseConfiguration`。

入口不维护逐 Control Token 列表、主题 URI 列表或聚合 AXAML；这些内容由构建期生成结果提供。

推荐启动顺序：

```csharp
this.UseAtomUI(builder =>
{
    builder.UseDesktopControls();
    builder.UseDesktopColorPicker();
    builder.UseDesktopDataGrid();
    builder.UseGalleryBase(...);
});
```

GalleryBase 依赖 AtomUI Desktop 控件作为 Shell 默认 UI，因此产品应先注册 Desktop 控件主题。

GalleryBase 本身不注册产品主题定义。具体产品通过 `IThemeDefinitionResolver` 显式追加自己的 XML 资源；例如
AtomUIGallery 在 `UseGalleryControls()` 中注册四个产品主题，而单独使用 `UseGalleryBase()` 的应用不会看到这些主题。

## Shell 主题选择状态

`GalleryWorkspaceViewModel` 把 ThemeManager 已提交的 Catalog 和主题状态投影成 Shell 可消费的只读状态：

- `AvailableThemes`：`IThemeManager.AvailableThemes` 的防御性快照；`ThemeCatalogChanged` 后整体替换。
- `CurrentThemeId`：只由已提交的 `ThemeChanged`/Catalog 回退状态更新，不把菜单点击当成提交结果。
- `SwitchThemeCommand`：接收 Theme Id，并沿统一请求路径组合 Dark、Compact、Motion 和 Wave Spirit 设置。

主题色切换只改变 `ThemeRequest.ThemeId`。AtomUIGallery 的内置产品主题在对应 Theme Definition XML 中显式声明
相同颜色的 `ColorPrimary` 与 `ColorLink`，使品牌控件和 Link 同步换色；`ColorInfo` 等语义色保持独立。颜色 Seed
不写入运行时 `ThemeConfig`。Dark、Compact、Motion 和 Wave Spirit 与主题 Id 正交组合；失败或被更新请求替代时，
Shell 会重新投影已提交的 `CurrentThemeId`，避免 Radio 菜单停留在未提交选项。

ViewModel 对 `ThemeChanged`、`ThemeCatalogChanged` 和语言事件都保存对称的 handler，并在 `Dispose()` 中解除订阅。
产品窗口可以基于 `AvailableThemes` 动态创建同组 Radio 菜单，但不应复制主题名称/Id，也不应自行解析 XML。
AtomUIGallery 将这些 Radio 项放在本地化的 Theme Settings 子菜单中；主题声明 `AccentColor` 时，菜单项在文字右侧
显示 `12 × 12`、`2px` 圆角的实心正方形色块，未声明时不创建占位色块。Dark、Compact、Motion 和 Wave Spirit
仍是外层 Theme 菜单中的正交开关。

## 主题资产与生成注册

```text
<ControlFolder>/Themes/**/*.axaml
    -> ControlTheme asset owner/reference manifest
    -> package-level UseGalleryBase() registration
```

GalleryBase 当前主题资产包括：

- `ShowCaseItemTheme.axaml`
- `ShowCasePanelTheme.axaml`
- `GalleryShowCaseHeaderTheme.axaml`
- `GalleryStickyTabsHostTheme.axaml`
- `ColorItemControlTheme.axaml`
- `ColorListControlTheme.axaml`
- `IconGalleryTheme.axaml`
- `IconInfoItemTheme.axaml`
- `GalleryWindowTitleBarTheme.axaml`

构建集成按 `Themes/**/*.axaml` 约定收集这些资产。平台 Provider 只把生成 manifest 中适用的资产接入 Avalonia
Styles；开发者不维护 `GalleryControlThemesProvider.axaml` 聚合字典、Theme Module、glob Attribute 或手工
manifest。单个 Control 只有一个 ControlTheme 时只保留对应主题文件，不增加包装层。

`GalleryShellView` 当前主要通过 C# 组合 AtomUI 现有控件和 Shared Token 完成 sidebar、footer、分隔线与内容区背景；后续如果 sidebar/navigation 有独立主题状态，再补对应 GalleryBase 控件主题和 token。

## Control Token

GalleryBase Token 清单：

| Token | 职责 |
|---|---|
| `ShowCaseItemToken` | 卡片 padding、圆角、阴影、标题权重、placeholder |
| `ShowCasePanelToken` | item 宽度、列数、行列间距、内容 margin |
| `GalleryShowCaseHeaderToken` | Header margin、标题字号、Tag 间距、metadata 卡片 padding、label/value 宽度 |
| `GalleryStickyTabsHostToken` | sticky 背景、边线、padding |
| `GalleryWindowTitleBarToken` | 标题栏菜单字体和间距 |
| `GallerySidebarToken` | 侧边栏宽度、brand/footer padding、分隔线 |
| `GalleryNavigationToken` | 导航 margin、默认缩进、滚动条间距 |

Own Token 类只在 Control 存在独有设计值时创建，并由 AtomUI generator 按约定识别：

```csharp
[ControlDesignToken]
internal sealed class ShowCaseItemToken : AbstractControlDesignToken
{
}
```

`[ControlDesignToken]` 只标记 Own Token 类型，不携带 Control 类型、identity 或 ID。禁止泛型
`[ControlDesignToken<TControl>]` 和手写 ID。即使某个 GalleryBase Control 没有 Own Token，生成器仍为它生成独立
identity、零 Own Token descriptor 和 `XxxTokenResource`。

生成资源扩展后，主题使用：

```xml
Padding="{gallery:ShowCaseItemTokenResource CardPadding}"
```

`GalleryShowCaseHeaderToken` 只承载 Gallery 文档页头的结构性视觉值，例如 margin、间距、字号、metadata 卡片边框和默认宽度。分类、状态、引入版本 Tag 的颜色仍通过 `GalleryShowCaseHeader` 属性传给 AtomUI `Tag`，不在 token 中写死具体业务状态。

## Global Token 使用规则

GalleryBase 可以使用以下 AtomUI Global Token：

- `ColorBgContainer`
- `ColorBgLayout`
- `ColorBorderSecondary`
- `ColorTextSecondary`
- `BoxShadowsTertiary`
- `SizeUnit`
- `BorderRadius`

每个 GalleryBase Control 天然可以覆盖完整 Global Token schema。ControlTheme 希望读取当前 Control 的 Effective
Global Token 时使用对应的 `XxxTokenResource`；只有要求该值永远跟随当前 ThemeContext 全局结果时才使用
`SharedTokenResource`。两者都不产生 Global Token 消费清单。不允许在 GalleryBase 主题中写入产品色值，例如固定
品牌蓝、AtomUI logo 色或产品状态色。
产品色应通过产品页面或产品品牌配置表达。

## Shell 本地化

GalleryBase 提供 Shell 语言资源：

```text
Localization/
  GalleryShellLang/
    en_US.cs
    zh_CN.cs
    zh_TW.cs
  GalleryNavigationLang/
    en_US.cs
    zh_CN.cs
    zh_TW.cs
```

资源范围：

| 资源 | 示例 |
|---|---|
| 设置菜单 | Settings |
| 窗口选项 | Window Options |
| 主题菜单 | Theme |
| 暗色模式 | Dark Mode |
| 紧凑模式 | Compact Mode |
| 动效 | Motion |
| 语言菜单 | Language |
| 搜索提示 | Search |

GalleryBase 也提供 ShowCase Header 的通用 metadata label：

| 资源 | 示例 |
|---|---|
| Namespace label | Namespace |
| Package label | Package |
| Base class label | Base class |

这些 label 属于通用文档页头结构，不应再由每个产品 ShowCase 重复定义。产品页面继续提供控件自己的 title、category、status、subtitle、description 和引入版本文案。

不属于 GalleryBase 的资源：

- `Button`
- `DataGrid`
- `Overview`
- `Community`
- `Install`
- `API`
- `Design Token` 页面内容
- 控件分类、稳定状态、Preview 状态和页面描述

这些由产品项目定义。

## 产品导航文案

第一阶段产品导航 header 可以直接传字符串。后续支持可更新语言资源对象：

```csharp
public sealed class GalleryLanguageText : IGalleryLocalizedText
{
    public object ResourceKey { get; }
    public string Fallback { get; }
}
```

刷新策略：

- Shell 监听 `ThemeManager.LanguageVariantChanged`。
- Header 是 `IGalleryLocalizedText` 时重新解析。
- Header 是字符串时保持不变。

## 语言 Provider 注册

GalleryBase 的 `UseGalleryBase` 注册自己的语言 Provider：

```csharp
var languageProviders = LanguageProviderPool.GetLanguageProviders();
foreach (var provider in languageProviders)
{
    builder.AddLanguageProviders(provider);
}
```

产品语言 Provider 仍由产品项目自己的注册扩展加入。GalleryBase 不扫描产品程序集。

## Browser 主题限制

Browser 使用同一套主题 Provider，但要避免：

- 隐式启用用户主题目录；Browser 只能使用随应用发布的显式 Resolver。

- 依赖仅 Desktop 原生窗口 API 的主题资源。
- 在 theme 中引用 Desktop-only 控件模板。
- 因 Browser 裁剪导致 token 类型未被 DynamicDependency 保留。

如果某个 GalleryBase 控件在 Browser 需要不同模板，应在 Provider 内按平台提供安全模板，而不是让产品项目自己替换。

## 测试要求

- GalleryBase AssemblyInfo 不包含产品 ShowCase namespace。
- `UseGalleryBase` 注册 GalleryBase token 和 theme provider。
- `GalleryControlThemesProvider` 包含 `GalleryShowCaseHeaderTheme.axaml`。
- GalleryBase 语言 Provider 不包含产品页面语言资源。
- GalleryBase 语言 Provider 包含 ShowCase Header metadata 通用 label。
- 主题文件不包含 `AtomUIGallery/Assets` 或产品 URI。
- Browser 构建时能解析 GalleryBase theme provider。
- 语言切换后 Shell 菜单状态和文案刷新。
