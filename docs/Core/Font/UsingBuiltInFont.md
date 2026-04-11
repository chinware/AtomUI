# AtomUI 字体系统 — 使用内置字体包

> 本文档面向 **AtomUI 应用开发者**，介绍如何在应用中启用 AtomUI 内置的 Alibaba Sans 字体包、如何自定义默认字体族，以及字体在主题系统中的消费方式。

---

## 1. 前置条件

- 项目已引用 `AtomUI.Fonts.AlibabaSans` NuGet 包（或项目引用）。
- 项目已引用 `AtomUI.Desktop.Controls`（或其他 AtomUI 控件包）。

---

## 2. 在 AtomUI 主题中启用内置字体

### 2.1 标准用法（推荐）

在 `Application` 类的 `Initialize()` 方法中，通过 `UseAtomUI()` 的 Builder 回调注册字体：

```csharp
using AtomUI.Theme;

public class App : Application
{
    public override void Initialize()
    {
        base.Initialize();
        this.UseAtomUI(builder =>
        {
            builder.UseAlibabaSansFont();   // ← 注册 Alibaba Sans 字体包
            builder.UseDesktopControls();   // 注册桌面控件
        });
    }
}
```

调用 `builder.UseAlibabaSansFont()` 后：

1. `AlibabaSansFontCollection` 被注册到 Avalonia 的 `FontManager`。
2. `fonts:AlibabaSans#Alibaba Sans` URI 变为可用的字体引用。
3. AtomUI 默认的 `DesignToken.FontFamily` Seed Token 已经配置了 `fonts:AlibabaSans#Alibaba Sans` 作为首选字体，因此所有使用 `{atom:SharedTokenResource FontFamily}` 的控件会自动使用 Alibaba Sans。

> **注意：** 如果不调用 `builder.UseAlibabaSansFont()`，`fonts:AlibabaSans#Alibaba Sans` URI 将无法解析，字体会自动 Fallback 到回退链中的下一个可用字体（如 Segoe UI、PingFang SC 等系统字体）。

### 2.2 完整示例（Gallery 应用）

以下是 AtomUI Gallery 应用的完整配置，作为参考：

```csharp
using AtomUI.Desktop.Controls;
using AtomUI.Theme;
using AtomUI.Theme.Language;

public class GalleryApplication : BaseGalleryApplication
{
    public override void Initialize()
    {
        base.Initialize();
        this.UseAtomUI(builder =>
        {
            builder.WithDefaultLanguageVariant(LanguageVariant.zh_CN);
            builder.WithDefaultTheme(IThemeManager.DEFAULT_THEME_ID);
            builder.UseAlibabaSansFont();    // 启用内置字体
            builder.UseDesktopControls();
            builder.UseDesktopDataGrid();
            builder.UseDesktopColorPicker();
        });
    }
}
```

---

## 3. 自定义默认字体族

### 3.1 使用 WithDefaultFontFamily 覆盖

如果你想使用其他字体作为默认字体族（而非 Alibaba Sans），可以通过 `WithDefaultFontFamily()` 方法覆盖：

```csharp
this.UseAtomUI(builder =>
{
    builder.UseAlibabaSansFont();   // 仍然注册字体包（以防某些控件需要）
    builder.UseDesktopControls();
    
    // 使用系统字体或其他字体作为默认字体
    builder.WithDefaultFontFamily("Segoe UI, PingFang SC, $Default");
});
```

或者使用 `FontFamily` 对象：

```csharp
using Avalonia.Media;

this.UseAtomUI(builder =>
{
    builder.UseDesktopControls();
    builder.WithDefaultFontFamily(new FontFamily("Inter, Segoe UI, $Default"));
});
```

### 3.2 工作原理

`WithDefaultFontFamily()` 的工作原理是：在每个主题加载完成后（`ThemeLoaded` 事件），将 `ResourceDictionary` 中的 `SharedTokenKind.FontFamily` 资源值替换为指定的字体族：

```csharp
// ApplicationExtensions.cs 内部实现
var defaultFontFamily = themeManagerBuilder.FontFamily;
themeManager.ThemeLoaded += (sender, args) =>
{
    if (defaultFontFamily != null && args.Theme != null)
    {
        var loadedTheme = args.Theme;
        loadedTheme.ThemeResource[SharedTokenKind.FontFamily] = defaultFontFamily;
    }
};
```

这意味着 `WithDefaultFontFamily()` 的优先级**高于** `DesignToken.FontFamily` Seed Token 的默认值，但**不会**影响字号梯度的推导（字号梯度由 `FontSize` Seed Token 推导，与 `FontFamily` 无关）。

---

## 4. 在 AXAML 中使用字体 Token

### 4.1 全局字体族

在自定义控件或页面中，通过 Token 标记扩展引用字体：

```xml
<!-- 使用全局字体族 -->
<TextBlock FontFamily="{atom:SharedTokenResource FontFamily}"
           Text="Hello, AtomUI!" />

<!-- 使用全局基准字号 -->
<TextBlock FontFamily="{atom:SharedTokenResource FontFamily}"
           FontSize="{atom:SharedTokenResource FontSize}"
           Text="14px 基准字号" />

<!-- 使用大号字号 -->
<TextBlock FontFamily="{atom:SharedTokenResource FontFamily}"
           FontSize="{atom:SharedTokenResource FontSizeLG}"
           Text="16px 大号字号" />

<!-- 使用标题字号 -->
<TextBlock FontFamily="{atom:SharedTokenResource FontFamily}"
           FontSize="{atom:SharedTokenResource FontSizeHeading1}"
           Text="38px 一级标题" />
```

### 4.2 直接引用嵌入字体

如果你需要在某处使用非默认的嵌入字体（绕过 Token 系统），可以直接使用字体 URI：

```xml
<!-- 直接引用嵌入的 Alibaba Sans -->
<TextBlock FontFamily="fonts:AlibabaSans#Alibaba Sans"
           Text="直接引用" />
```

但推荐通过 Token 标记扩展引用，以便主题切换和字体覆盖能自动生效。

### 4.3 常用字体 Token 速查

| Token 资源键 | 类型 | 默认值 | 说明 |
|-------------|------|--------|------|
| `FontFamily` | `FontFamily` | Alibaba Sans + Fallback | 全局字体族 |
| `FontSize` | `double` | 14 | 基准字号 |
| `FontSizeSM` | `double` | 12 | 小号字号 |
| `FontSizeLG` | `double` | 16 | 大号字号 |
| `FontSizeXL` | `double` | 20 | 超大号字号 |
| `FontSizeHeading1` | `double` | 38 | H1 标题字号 |
| `FontSizeHeading2` | `double` | 30 | H2 标题字号 |
| `FontSizeHeading3` | `double` | 24 | H3 标题字号 |
| `FontSizeHeading4` | `double` | 20 | H4 标题字号 |
| `FontSizeHeading5` | `double` | 16 | H5 标题字号 |
| `FontSizeIcon` | `double` | 12 | 图标字号 |
| `FontWeightStrong` | `FontWeight` | SemiBold | 加粗字重 |
| `RelativeLineHeight` | `double` | ≈1.571 | 基准行高 |
| `FontHeight` | `double` | 22 | 文字高度 (px) |

---

## 5. 字体继承机制

AtomUI 的字体设置利用了 Avalonia 的 **属性继承（Property Inheritance）** 机制：

1. **Window** / **PopupRoot** / **OverlayPopupHost** 控件主题设置 `FontFamily` 和 `FontSize`。
2. 所有子控件自动继承父控件的 `FontFamily` 和 `FontSize` 值。
3. 个别控件可以在自己的主题中覆盖这些值（如标题控件使用 `FontSizeHeading*`）。

```
Window
  FontFamily = {atom:SharedTokenResource FontFamily}  ← 根级设置
  FontSize = {atom:SharedTokenResource FontSize}
  │
  ├── StackPanel (继承 FontFamily, FontSize)
  │   ├── TextBlock (继承) → 14px Alibaba Sans
  │   └── Button (继承) → 14px Alibaba Sans
  │
  └── Typography.Title (覆盖 FontSize)
      └── TextBlock FontSize = FontSizeHeading1 → 38px Alibaba Sans
```

---

## 6. 在纯 Avalonia 应用中使用（不使用 AtomUI 主题）

如果你的应用不使用 AtomUI 主题系统，但仍想使用 Alibaba Sans 字体，可以通过 `AppBuilder` 扩展方法注册：

```csharp
class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithAlibabaSansFont()  // ← 纯 Avalonia 方式注册字体
            .LogToTrace();
}
```

然后在 XAML 中直接引用：

```xml
<TextBlock FontFamily="fonts:AlibabaSans#Alibaba Sans"
           FontSize="14"
           Text="Hello, Avalonia!" />
```

或者设置为全局默认字体：

```xml title="App.axaml"
<Application.Resources>
    <FontFamily x:Key="DefaultFont">fonts:AlibabaSans#Alibaba Sans</FontFamily>
</Application.Resources>
```

---

## 7. 排障指南

### 7.1 字体显示为系统默认字体（不是 Alibaba Sans）

**可能原因：**
1. 忘记调用 `builder.UseAlibabaSansFont()` — 字体包未注册。
2. `fonts:AlibabaSans#Alibaba Sans` 中的字体族名称拼写错误。
3. 字体文件未正确嵌入 — 检查 `.csproj` 中是否有 `<AvaloniaResource Include="Assets/*" />`。

**诊断方法：** 检查应用启动日志，Avalonia 在找不到字体时通常会有警告日志。

### 7.2 中文字符显示为方块（□）

**可能原因：** 使用的字体不包含中文字符。Alibaba Sans 包含中文字符集，但如果通过 `WithDefaultFontFamily()` 替换为不支持中文的西文字体（如 Inter），中文字符将无法显示。

**解决方案：** 在字体族回退链中包含支持中文的字体：

```csharp
builder.WithDefaultFontFamily("Inter, PingFang SC, Microsoft YaHei, $Default");
```

### 7.3 字号梯度不符合预期

`FontSize` Seed Token 的默认值为 14。如果需要修改基准字号，需要在主题 XML 配置文件中覆盖 `FontSize` Seed Token，所有派生的字号梯度会自动重新计算。

