# AtomUI 多语言系统 — 开发者使用指南

> 本文档面向使用 AtomUI 框架开发自定义控件或应用的开发者，详细说明如何为控件和应用添加多语言支持。

---

## 1. 快速上手

为一个控件添加多语言支持只需 5 步：

1. 配置程序集元信息（`AssemblyInfo.cs`）
2. 创建语言 Provider 类（每种语言一个文件）
3. 编译 — Source Generator 自动生成枚举和标记扩展
4. 在 `ThemeManagerBuilderExtensions` 中注册语言 Provider
5. 在 AXAML 主题中使用生成的标记扩展绑定文本

下面以一个完整的 **QRCode 控件**为例，逐步演示。

---

## 2. 完整示例：QRCode 控件多语言

### 2.1 目录结构

```
AtomUI.Desktop.Controls/
├── Properties/
│   └── AssemblyInfo.cs              ← 步骤 1：程序集级配置
├── QRCode/
│   ├── QRCode.cs                    ← 控件类
│   ├── QRCodeToken.cs               ← Token 定义（含 const string ID）
│   ├── Localization/                ← 步骤 2：语言 Provider
│   │   ├── zh_CN.cs
│   │   └── en_US.cs
│   └── Themes/
│       └── QRCodeTheme.axaml        ← 步骤 5：使用语言资源
├── GeneratedFiles/                  ← 步骤 3：自动生成（不要手动编辑！）
│   └── AtomUI.Generator/
│       └── AtomUI.Generator.LanguageGenerator/
│           ├── LanguageResourceConst.g.cs    ← 生成的枚举 + 标记扩展
│           └── LanguageProviderPool.g.cs     ← 生成的 Provider 注册池
└── ThemeManagerBuilderExtensions.cs  ← 步骤 4：注册
```

### 2.2 步骤 1：配置程序集元信息

在项目的 `Properties/AssemblyInfo.cs` 中添加两项配置：

```csharp
using AtomUI.Theme.Language;
using Avalonia.Metadata;

// 1. 注册生成代码的 XAML 命名空间映射
[assembly: XmlnsDefinition("https://atomui.net", "AtomUI.Desktop.Controls.Localization")]

// 2. 指定 Source Generator 输出命名空间
[assembly: LanguageSgMetaInfo("AtomUI.Desktop.Controls.Localization")]
```

**说明**：
- `XmlnsDefinition` 确保生成的标记扩展可以在 AXAML 中通过 `atom:` 前缀访问
- `LanguageSgMetaInfo` 告诉 Source Generator 将枚举和标记扩展生成到指定命名空间
- 这两项配置**每个项目只需设置一次**

### 2.3 步骤 2：创建语言 Provider 类

在控件目录下创建 `Localization/` 文件夹，每种语言一个文件：

#### `QRCode/Localization/zh_CN.cs`

```csharp
using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.QRCodeLang;

[LanguageProvider(LanguageCode.zh_CN, QRCodeToken.ID)]
internal class zh_CN : LanguageProvider
{
    public const string Refresh = "点击刷新";
    public const string Expired = "二维码过期";
    public const string Scanned = "已扫描";
    
    protected override Type GetResourceKindType() => typeof(QRCodeLangResourceKind);
}
```

#### `QRCode/Localization/en_US.cs`

```csharp
using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.QRCodeLang;

[LanguageProvider(LanguageCode.en_US, QRCodeToken.ID)]
internal class en_US : LanguageProvider
{
    public const string Refresh = "Refresh";
    public const string Expired = "QR code expired";
    public const string Scanned = "Scanned";
    
    protected override Type GetResourceKindType() => typeof(QRCodeLangResourceKind);
}
```

**关键规则**：

| 规则 | 说明 |
|------|------|
| **特性标注** | 必须添加 `[LanguageProvider(LanguageCode.xx, LangId)]` |
| **继承** | 必须继承 `LanguageProvider` |
| **访问修饰符** | 类建议 `internal`，字段必须 `public const string` |
| **LangId** | 通常使用控件 Token 的 `ID`（如 `QRCodeToken.ID = "QRCode"`） |
| **命名空间** | 使用独立命名空间：`{项目}.{控件名}Lang`（如 `AtomUI.Desktop.Controls.QRCodeLang`） |
| **字段名一致** | 同一 LangId 下所有语言的字段名必须完全一致（大小写敏感） |
| **GetResourceKindType()** | 返回生成的 `{LangId}LangResourceKind` 枚举类型 |

### 2.4 步骤 3：编译 — Source Generator 自动生成

编译后，Source Generator 自动生成两个文件：

#### 生成的 `LanguageResourceConst.g.cs`

```csharp
namespace AtomUI.Desktop.Controls.Localization
{
    // 资源键枚举 — 字段名自动从所有语言 Provider 中提取并排序
    public enum QRCodeLangResourceKind
    {
        Expired,
        Refresh,
        Scanned
    }

    // XAML 标记扩展 — 在 AXAML 中绑定语言文本
    public class QRCodeLangResourceExtension : LanguageResourceExtension<QRCodeLangResourceKind>
    {
        public QRCodeLangResourceExtension() { }
        public QRCodeLangResourceExtension(QRCodeLangResourceKind kind) : base(kind) { }
    }
}
```

#### 生成的 `LanguageProviderPool.g.cs`

```csharp
namespace AtomUI.Theme.Language
{
    internal class LanguageProviderPool
    {
        internal static IList<LanguageProvider> GetLanguageProviders()
        {
            List<LanguageProvider> languageProviders = new List<LanguageProvider>();
            languageProviders.Add(new AtomUI.Desktop.Controls.QRCodeLang.en_US());
            languageProviders.Add(new AtomUI.Desktop.Controls.QRCodeLang.zh_CN());
            // ... 同项目中其他控件的 Provider
            return languageProviders;
        }
    }
}
```

> ⚠️ **不要手动编辑 `GeneratedFiles/` 目录下的文件！** 它们在每次编译时会被自动重新生成。

### 2.5 步骤 4：注册语言 Provider

在项目的 `ThemeManagerBuilderExtensions.cs` 中注册：

```csharp
public static IThemeManagerBuilder UseDesktopControls(this IThemeManagerBuilder themeManagerBuilder)
{
    // ... 注册 Token 和控件主题 ...

    // 注册语言 Provider（Source Generator 生成的 LanguageProviderPool）
    var languageProviders = LanguageProviderPool.GetLanguageProviders();
    foreach (var languageProvider in languageProviders)
    {
        themeManagerBuilder.AddLanguageProviders(languageProvider);
    }

    return themeManagerBuilder;
}
```

### 2.6 步骤 5：在 AXAML 主题中使用

在控件的 AXAML 主题文件中，使用生成的标记扩展绑定语言文本：

```xml
<ResourceDictionary
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:atom="https://atomui.net">

    <ControlTheme x:Key="{x:Type atom:QRCode}" TargetType="atom:QRCode">
        <!-- 使用语言资源标记扩展 -->
        <Setter Property="ExpiredDescription" Value="{atom:QRCodeLangResource Expired}" />
        <Setter Property="RefreshText" Value="{atom:QRCodeLangResource Refresh}" />
    </ControlTheme>
</ResourceDictionary>
```

**标记扩展语法**：

```xml
{atom:{LangId}LangResource {FieldName}}
```

例如：
- `{atom:QRCodeLangResource Expired}` — QRCode 的"过期"文本
- `{atom:TourLangResource Next}` — Tour 的"下一步"文本
- `{atom:CommonLangResource Ok}` — 通用的"确定"文本

---

## 3. 使用 Common 通用语言资源

对于跨控件共享的通用文本（如"确定"、"取消"、"提交"），AtomUI 提供了 `CommonLangId.Common` 分组。

### 3.1 Common 资源定义

定义在 `AtomUI.Controls/Localization/` 中：

```csharp
// AtomUI.Controls/Localization/zh_CN.cs
[LanguageProvider(LanguageCode.zh_CN, CommonLangId.Common)]
internal class zh_CN : LanguageProvider
{
    public const string Ok = "确定";
    public const string Submit = "提交";
    public const string Cancel = "取消";
    public const string Reset = "重置";
    public const string Edit = "编辑";
    public const string Delete = "删除";
    public const string Save = "保存";
    public const string NoData = "暂无数据";
    public const string Loading = "正在加载数据";
    public const string Optional = "(可选)";

    protected override Type GetResourceKindType() => typeof(CommonLangResourceKind);
}
```

### 3.2 在 AXAML 中使用 Common 资源

```xml
<!-- PopupConfirm 使用通用的 Ok/Cancel -->
<Setter Property="OkText" Value="{atom:CommonLangResource Ok}"/>
<Setter Property="CancelText" Value="{atom:CommonLangResource Cancel}"/>

<!-- Form SubmitButton 使用通用的 Submit -->
<Setter Property="Content" Value="{atom:CommonLangResource Submit}"/>

<!-- Form ResetButton 使用通用的 Reset -->
<Setter Property="Content" Value="{atom:CommonLangResource Reset}"/>
```

### 3.3 何时使用 Common vs 控件专属

| 场景 | 使用 |
|------|------|
| 通用操作按钮文本（确定、取消、提交等） | `CommonLangResource` |
| 控件特有的专业术语 | 控件专属 `{LangId}LangResource` |
| 同一文本在不同控件中含义相同 | `CommonLangResource` |
| 同一文本在不同控件中含义不同 | 各自定义专属资源 |

例如，Dialog 控件虽然也有 "确定" 和 "取消"，但它定义了自己的完整按钮文本集（包括 "打开"、"保存"、"丢弃" 等 Dialog 特有的选项），因此使用独立的 `DialogLangResource`。

---

## 4. 更多控件示例

### 4.1 Dialog 控件（大量语言项）

```csharp
// Dialog/Localization/zh_CN.cs
namespace AtomUI.Desktop.Controls.DialogLang;

[LanguageProvider(LanguageCode.zh_CN, DialogToken.ID)]
internal class zh_CN : LanguageProvider
{
    public const string Ok = "确定";
    public const string Open = "打开";
    public const string Save = "保存";
    public const string Cancel = "取消";
    public const string Close = "关闭";
    public const string Discard = "丢弃";
    public const string Apply = "应用";
    public const string Reset = "重置";
    public const string Reload = "重新加载";
    public const string RestoreDefaults = "恢复默认值";
    public const string Help = "帮助";
    public const string SaveAll = "全部保存";
    public const string Yes = "是";
    public const string YesToAll = "全部是";
    public const string No = "否";
    public const string NoToAll = "全部否";
    public const string Abort = "中止";
    public const string Retry = "重试";
    public const string Ignore = "忽略";
    
    protected override Type GetResourceKindType() => typeof(DialogLangResourceKind);
}
```

### 4.2 Tour 控件（在 AXAML Template 内使用）

```csharp
// Tour/Localization/zh_CN.cs
namespace AtomUI.Desktop.Controls.TourLang;

[LanguageProvider(LanguageCode.zh_CN, TourToken.ID)]
internal class zh_CN : LanguageProvider
{
    public const string Previous = "上一步";
    public const string Next = "下一步";
    public const string Finish = "结束导览";
    
    protected override Type GetResourceKindType() => typeof(TourLangResourceKind);
}
```

AXAML 中在 `ControlTemplate` 内部使用：

```xml
<ControlTemplate>
    <StackPanel Orientation="Horizontal">
        <atom:Button Content="{atom:TourLangResource Previous}"/>
        <atom:Button Content="{atom:TourLangResource Next}"/>
        <atom:Button Content="{atom:TourLangResource Finish}"/>
    </StackPanel>
</ControlTemplate>
```

### 4.3 Pagination 控件（含格式化字符串）

```csharp
// Pagination/Localization/zh_CN.cs
namespace AtomUI.Desktop.Controls.PaginationLang;

[LanguageProvider(LanguageCode.zh_CN, PaginationToken.ID)]
internal class zh_CN : LanguageProvider
{
    public const string JumpToText = "跳至";
    public const string PageText = "页";
    public const string TotalInfoFormat = "共 ${Total} 项";
    
    protected override Type GetResourceKindType() => typeof(PaginationLangResourceKind);
}
```

> **注意**：`TotalInfoFormat` 中的 `${Total}` 是一个占位符模板，需要控件代码在运行时进行字符串替换。多语言系统只负责提供翻译后的模板字符串。

---

## 5. 为应用级页面添加多语言

多语言系统不仅适用于控件库，也适用于应用程序自身的 UI 文本。以下是 AtomUI Gallery 应用的实际示例：

### 5.1 配置程序集元信息

```csharp
// AtomUIGallery/Properties/AssemblyInfo.cs
[assembly: LanguageSgMetaInfo("AtomUIGallery.Localization")]
```

### 5.2 定义语言 Provider

```csharp
// AtomUIGallery/Workspace/Localization/WorkspaceWindowLang/zh_CN.cs
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using AtomUIGallery.Workspace.Views;

namespace AtomUIGallery.Workspace.Localization.WorkspaceWindowLang;

[LanguageProvider(LanguageCode.zh_CN, WorkspaceWindow.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string MenuItemSettings = "设置";
    public const string MenuItemTheme = "主题";
    public const string MenuItemLanguage = "语言";
    public const string MenuItemDarkMode = "暗黑模式";
    public const string MenuItemCompactMode = "紧凑模式";
    public const string MenuItemEnableMotion = "开启动效";
    
    protected override Type GetResourceKindType() => typeof(WorkspaceWindowLangResourceKind);
}
```

```csharp
// 对应的 en_US.cs
[LanguageProvider(LanguageCode.en_US, WorkspaceWindow.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string MenuItemSettings = "Settings";
    public const string MenuItemTheme = "Theme";
    public const string MenuItemLanguage = "Language";
    public const string MenuItemDarkMode = "Dark Mode";
    public const string MenuItemCompactMode = "Compact Mode";
    public const string MenuItemEnableMotion = "Enable Motion";
    
    protected override Type GetResourceKindType() => typeof(WorkspaceWindowLangResourceKind);
}
```

### 5.3 定义 LangId 常量

应用级页面没有 Token ID，因此自定义一个常量：

```csharp
// WorkspaceWindow.cs
public partial class WorkspaceWindow
{
    public const string LanguageId = "WorkspaceWindow";
}
```

### 5.4 注册到 ThemeManagerBuilder

```csharp
// AtomUIGallery/ThemeManagerBuilderExtensions.cs
public static IThemeManagerBuilder UseGalleryControls(this IThemeManagerBuilder themeManagerBuilder)
{
    themeManagerBuilder.AddControlThemesProvider(new GalleryControlThemesProvider());
    
    var languageProviders = LanguageProviderPool.GetLanguageProviders();
    foreach (var languageProvider in languageProviders)
    {
        themeManagerBuilder.AddLanguageProviders(languageProvider);
    }
    return themeManagerBuilder;
}
```

---

## 6. 运行时语言切换

### 6.1 设置默认语言

在应用初始化时设置默认语言：

```csharp
// GalleryApplication.cs
public override void Initialize()
{
    base.Initialize();
    this.UseAtomUI(builder =>
    {
        builder.WithDefaultLanguageVariant(LanguageVariant.zh_CN);
        builder.UseDesktopControls();
    });
}
```

### 6.2 运行时切换

```csharp
// 切换到英文
Dispatcher.UIThread.Post(() =>
{
    Application.Current!.SetLanguageVariant(LanguageVariant.en_US);
});

// 切换到中文
Dispatcher.UIThread.Post(() =>
{
    Application.Current!.SetLanguageVariant(LanguageVariant.zh_CN);
});
```

> **建议**：使用 `Dispatcher.UIThread.Post()` 确保在 UI 线程执行。

### 6.3 使用 CultureInfo 切换

```csharp
// 通过 CultureInfo 设置语言
var cultureInfo = CultureInfo.GetCultureInfo("zh-CN");
var variant = LanguageVariant.FromCultureInfo(cultureInfo);
Application.Current!.SetLanguageVariant(variant);
```

### 6.4 监听语言变化

```csharp
var themeManager = Application.Current!.GetThemeManager();
themeManager!.LanguageVariantChanged += (sender, args) =>
{
    Console.WriteLine($"语言从 {args.OldLanguage} 切换到 {args.NewLanguage}");
};
```

---

## 7. 常见问题

### 7.1 编译时报错 `QRCodeLangResourceKind` 找不到

**原因**：Source Generator 尚未运行，或 `[LanguageProvider]` 特性标注不正确。

**解决**：
1. 确认项目引用了 `AtomUI.Generator`（通常通过 `AtomUI.Core` 传递引用）
2. 确认 `[LanguageProvider]` 特性的参数正确
3. 重新编译项目

### 7.2 不同语言的字段名不一致

**现象**：运行时报 `Language item: Xxx does not exist in ...` 异常。

**原因**：`LanguageProvider` 基类的 `BuildResourceDictionary` 方法遍历枚举值时，会查找同名的 `const string` 字段。如果某种语言缺少字段，会抛出异常。

**解决**：确保同一 `LangId` 下所有语言 Provider 的字段名完全一致（大小写敏感）。

### 7.3 AXAML 中标记扩展不可用

**原因**：`XmlnsDefinition` 未注册生成代码的命名空间。

**解决**：在 `AssemblyInfo.cs` 中添加：
```csharp
[assembly: XmlnsDefinition("https://atomui.net", "YourProject.Localization")]
```

### 7.4 语言切换后文本没有更新

**原因**：使用了 `StaticResource` 而非 `DynamicResource`（或生成的标记扩展）。

**解决**：始终使用生成的 `{atom:XxxLangResource Key}` 标记扩展。它继承自 `DynamicResourceExtension`，能自动响应语言切换。

### 7.5 在代码中获取语言文本

如果需要在 C# 代码中（而非 AXAML 中）获取语言文本，可以通过 Avalonia 资源系统查找：

```csharp
if (Application.Current!.TryFindResource(QRCodeLangResourceKind.Expired, out var text))
{
    var expiredText = text as string; // "二维码过期" 或 "QR code expired"
}
```

---

## 8. 新控件多语言支持清单

为新控件添加多语言支持时，按以下清单逐项检查：

- [ ] **`AssemblyInfo.cs`** — 确认已配置 `[assembly: LanguageSgMetaInfo("...")]` 和对应的 `XmlnsDefinition`（每个项目只需一次）
- [ ] **创建 `Localization/` 目录** — 在控件目录下创建
- [ ] **创建 `zh_CN.cs`** — 中文语言 Provider
- [ ] **创建 `en_US.cs`** — 英文语言 Provider
- [ ] **字段名一致** — 两个文件的 `public const string` 字段名完全相同
- [ ] **正确的 `LangId`** — 使用 `{ControlName}Token.ID` 或自定义常量
- [ ] **正确的命名空间** — 使用 `{项目}.{控件名}Lang` 格式
- [ ] **`GetResourceKindType()`** — 返回 `typeof({LangId}LangResourceKind)`
- [ ] **编译通过** — 确认 Source Generator 成功生成枚举和标记扩展
- [ ] **注册到 Builder** — 通过 `LanguageProviderPool.GetLanguageProviders()` 自动注册（如果是现有项目的新控件，通常无需额外操作）
- [ ] **AXAML 绑定** — 所有用户可见文本使用 `{atom:{LangId}LangResource Key}` 绑定
- [ ] **验证语言切换** — 在 Gallery 应用中测试中英文切换是否生效

