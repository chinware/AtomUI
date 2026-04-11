# LanguageGenerator 设计原理

> 本文档详细解析 `LanguageGenerator` 源生成器的内部实现，它是 AtomUI 多语言系统能够在 AXAML 中被类型安全引用的底层基础设施。

---

## 1. 解决的核心问题

AtomUI 的多语言系统需要解决：

- **语言文本定义在 C# 中**：每个控件的每种语言创建一个 `LanguageProvider` 子类，用 `public const string` 字段定义翻译文本。
- **语言文本消费在 AXAML 中**：Theme 文件通过 `{atom:UploadLangResource Uploading}` 引用本地化文本。
- **运行时切换**：用户切换语言时，需要动态替换 ResourceDictionary 中的资源值。

`LanguageGenerator` 的职责是 **自动从 LanguageProvider 类中提取所有字段名，生成枚举（资源键）、XAML 标记扩展类，以及 Provider 注册池**。

---

## 2. 输入与输出

### 2.1 输入：标记注解

**`[LanguageProvider(LanguageCode, LangId)]`** — 标记语言提供者类

```csharp
// src/AtomUI.Desktop.Controls/Upload/Localization/en_US.cs
namespace AtomUI.Desktop.Controls.UploadLang;

[LanguageProvider(LanguageCode.en_US, UploadToken.ID)]
internal class en_US : LanguageProvider
{
    public const string Uploading = "Uploading...";
    public const string Pending = "Pending...";
    public const string DragUploadHead = "Click or drag file to this area to upload";
    
    protected override Type GetResourceKindType() => typeof(UploadLangResourceKind);
}
```

```csharp
// src/AtomUI.Desktop.Controls/Upload/Localization/zh_CN.cs
namespace AtomUI.Desktop.Controls.UploadLang;

[LanguageProvider(LanguageCode.zh_CN, UploadToken.ID)]
internal class zh_CN : LanguageProvider
{
    public const string Uploading = "上传中...";
    public const string Pending = "等待调度...";
    public const string DragUploadHead = "点击或拖动文件到此区域进行上传";
    
    protected override Type GetResourceKindType() => typeof(UploadLangResourceKind);
}
```

**`[LanguageSgMetaInfo]`** — 程序集级注解，指定生成代码的目标命名空间

```csharp
// src/AtomUI.Desktop.Controls/Properties/AssemblyInfo.cs
[assembly: LanguageSgMetaInfo("AtomUI.Desktop.Controls.Localization")]
```

### 2.2 输出产物

**产物 1：`LanguageResourceConst.g.cs`**

为每个 `LanguageId`（即每个控件的语言组）生成一对枚举 + 标记扩展类：

```csharp
namespace AtomUI.Desktop.Controls.Localization
{
    // 枚举：合并该 LanguageId 下所有语言 Provider 的字段名（去重）
    public enum UploadLangResourceKind
    {
        DragUploadHead,
        Pending,
        Uploading
    }

    // XAML 标记扩展：在 AXAML 中作为 {atom:UploadLangResource Uploading} 使用
    public class UploadLangResourceExtension : LanguageResourceExtension<UploadLangResourceKind>
    {
        public UploadLangResourceExtension() { }
        public UploadLangResourceExtension(UploadLangResourceKind kind) : base(kind) { }
    }
}
```

**产物 2：`LanguageProviderPool.g.cs`**

一个静态方法，实例化当前程序集中所有 `[LanguageProvider]` 类：

```csharp
namespace AtomUI.Theme.Language
{
    internal class LanguageProviderPool
    {
        internal static IList<LanguageProvider> GetLanguageProviders()
        {
            List<LanguageProvider> languageProviders = new List<LanguageProvider>();
            languageProviders.Add(new AtomUI.Desktop.Controls.UploadLang.en_US());
            languageProviders.Add(new AtomUI.Desktop.Controls.UploadLang.zh_CN());
            // ... 所有语言 Provider 实例
            return languageProviders;
        }
    }
}
```

---

## 3. 内部管线详解

### 3.1 管线概览

```
LanguageGenerator.Initialize()
│
├─ languageProvider ─────────────────────────────────────────────┐
│  ForAttributeWithMetadataName("[LanguageProvider]")            │
│  → LanguageProviderWalker 遍历                                 │
│  → 收集 LanguageCode / LanguageId / 字段名+值 / 命名空间       │
│  → Collect                                                     │
│                                                                │
└─ RegisterImplementationSourceOutput ───────────────────────────┘
   │
   ├─ LangResourceKeyClassSourceWriter.Write()
   │  → 生成 LanguageResourceConst.g.cs
   │     ├─ 按 LanguageId 分组
   │     ├─ 合并同组所有 Provider 的字段名（去重 + 排序）
   │     ├─ XxxLangResourceKind 枚举
   │     └─ XxxLangResourceExtension 类
   │
   └─ LanguageProviderPoolClassSourceWriter.Write()
      → 生成 LanguageProviderPool.g.cs
         └─ GetLanguageProviders() 方法
```

### 3.2 LanguageProviderWalker（语言提供者遍历器）

**文件**：`src/AtomUI.Generator/Language/LanguageProviderWalker.cs`

继承 `CSharpSyntaxWalker`，执行三项任务：

**① 提取程序集元信息**

在构造函数中调用 `ExtractLanguageMetaInfo()`，从程序集的 `[LanguageSgMetaInfo]` 注解中提取 `TargetNamespace`。这决定了生成代码的目标命名空间（如 `AtomUI.Desktop.Controls.Localization`）。

**② 遍历类声明**

`VisitClassDeclaration` 提取：
- `ClassName`：类名（如 `en_US`）
- `Namespace`：所在命名空间（如 `AtomUI.Desktop.Controls.UploadLang`）
- `LanguageCode`：从 `[LanguageProvider]` 注解的第一个参数获取（如 `"en_US"`）
- `LanguageId`：从注解的第二个参数获取（如 `"Upload"`，即 `UploadToken.ID` 的编译时常量值）

**③ 遍历字段声明**

`VisitFieldDeclaration` 收集每个 `public const string` 字段的名称和初始值：
- 字段名作为资源键（如 `Uploading`）
- 字段值作为翻译文本（如 `"Uploading..."`）

### 3.3 LanguageId 分组机制

生成器使用 `LanguageId`（即 Token 的 `ID` 常量，如 `"Upload"`、`"Dialog"`）将多个语言 Provider 分组。同一 LanguageId 下的所有 Provider（如 `en_US` 和 `zh_CN`）共享同一个生成的枚举类型。

```
LanguageId = "Upload"
├── UploadLang.en_US  → { Uploading, Pending, DragUploadHead }
└── UploadLang.zh_CN  → { Uploading, Pending, DragUploadHead }
    ↓ 合并去重
    UploadLangResourceKind { DragUploadHead, Pending, Uploading }
```

这确保了不同语言的 Provider 定义相同的字段集合时，生成的枚举是一致的。

### 3.4 LangResourceKeyClassSourceWriter（资源键写入器）

**文件**：`src/AtomUI.Generator/Language/LangResourceKeyClassSourceWriter.cs`

核心逻辑：

1. **按 LanguageId 分组**：将所有 `LanguageInfo` 按 `LanguageId` 分组到 `Dictionary<string, List<LanguageInfo>>`。
2. **合并字段名**：对同一组内所有 Provider 的字段名取并集（`UnionWith`），然后按字母排序。
3. **生成枚举**：枚举名为 `{LanguageId}LangResourceKind`，成员为排序后的字段名。
4. **生成标记扩展类**：类名为 `{LanguageId}LangResourceExtension`，继承 `LanguageResourceExtension<{LanguageId}LangResourceKind>`。
5. **目标命名空间**：优先使用 `[LanguageSgMetaInfo]` 指定的 `TargetNamespace`；若未指定，使用第一个 Provider 的命名空间。

### 3.5 LanguageProviderPoolClassSourceWriter（Provider 池写入器）

**文件**：`src/AtomUI.Generator/Language/LanguageProviderPoolClassSourceWriter.cs`

生成 `LanguageProviderPool` 类，内含 `GetLanguageProviders()` 方法。该方法使用完全限定名（`Namespace.ClassName`）实例化每个 Provider，并按命名空间 + 类名排序以保证生成结果稳定。

---

## 4. 语言资源从定义到消费的完整链路

以 `Upload` 控件的 `Uploading` 文本为例：

### 4.1 定义阶段

```csharp
// 开发者编写 en_US.cs
[LanguageProvider(LanguageCode.en_US, UploadToken.ID)]
internal class en_US : LanguageProvider
{
    public const string Uploading = "Uploading...";
    protected override Type GetResourceKindType() => typeof(UploadLangResourceKind);
}

// 开发者编写 zh_CN.cs
[LanguageProvider(LanguageCode.zh_CN, UploadToken.ID)]
internal class zh_CN : LanguageProvider
{
    public const string Uploading = "上传中...";
    protected override Type GetResourceKindType() => typeof(UploadLangResourceKind);
}
```

### 4.2 生成阶段（编译时）

```csharp
// 自动生成 — LanguageResourceConst.g.cs
namespace AtomUI.Desktop.Controls.Localization
{
    public enum UploadLangResourceKind { DragUploadHead, Pending, Uploading }
    
    public class UploadLangResourceExtension : LanguageResourceExtension<UploadLangResourceKind>
    {
        public UploadLangResourceExtension() { }
        public UploadLangResourceExtension(UploadLangResourceKind kind) : base(kind) { }
    }
}

// 自动生成 — LanguageProviderPool.g.cs
languageProviders.Add(new AtomUI.Desktop.Controls.UploadLang.en_US());
languageProviders.Add(new AtomUI.Desktop.Controls.UploadLang.zh_CN());
```

### 4.3 注册阶段（运行时启动）

ThemeManager 调用 `LanguageProviderPool.GetLanguageProviders()` 获取所有 Provider 实例。`LanguageProvider` 基类通过反射读取子类的 `public const string` 字段，将字段名（如 `"Uploading"`）映射到 `UploadLangResourceKind.Uploading` 枚举值，以此作为 ResourceKey 写入对应语言的 `ResourceDictionary`。

### 4.4 消费阶段（AXAML 渲染）

```xml
<!-- UploadDefaultDropAreaTheme.axaml -->
<Setter Property="Header" Value="{atom:UploadLangResource DragUploadHead}" />
```

AXAML 解析器将 `{atom:UploadLangResource DragUploadHead}` 解析为 `UploadLangResourceExtension(UploadLangResourceKind.DragUploadHead)`。由于继承自 `DynamicResourceExtension`，切换语言时 ResourceDictionary 被替换，绑定自动更新。

### 4.5 语言切换

当用户调用 `ThemeManager.Current.SetLanguage(LanguageCode.zh_CN)` 时：
1. ThemeManager 替换 `Application.Resources` 中的语言 ResourceDictionary。
2. 所有通过 `DynamicResourceExtension` 绑定的语言资源自动更新。
3. UI 即时反映新语言文本，无需重新加载。

---

## 5. 关键数据结构

### LanguageInfo

```csharp
internal class LanguageInfo
{
    public string? TargetNamespace { get; set; }  // 来自 [LanguageSgMetaInfo]
    public string Namespace { get; set; }          // Provider 类的命名空间
    public string LanguageId { get; set; }         // 如 "Upload"（来自注解第二个参数）
    public string LanguageCode { get; set; }       // 如 "en_US"（来自注解第一个参数）
    public string ClassName { get; set; }          // 如 "en_US"
    public Dictionary<string, string> Items { get; set; }  // 字段名 → 文本值
}
```

---

## 6. 与 LanguageResourceExtension 运行时基类的关系

生成的 `XxxLangResourceExtension` 继承自 `LanguageResourceExtension<TResourceKind>`，其定义在 `AtomUI.Core`：

```csharp
// src/AtomUI.Core/Language/LanguageResourceExtension.cs
public abstract class LanguageResourceExtension<TResourceKind> : DynamicResourceExtension
    where TResourceKind : Enum
{
    public TResourceKind? Kind
    {
        get => (TResourceKind?)ResourceKey; 
        set => ResourceKey = value; 
    }
    
    public new IBinding ProvideValue(IServiceProvider serviceProvider)
    {
        base.ProvideValue(serviceProvider);
        // 特殊处理：当目标不是 StyledElement 时（如 Application 级资源），
        // 设置 anchor 为 Application.Current，确保资源查找正确
        var provideTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (provideTarget?.TargetObject is not StyledElement)
        {
            this.SetAnchor(Application.Current);
        }
        return this;
    }
}
```

与 `TokenResourceExtension` 的差异：`LanguageResourceExtension` 重写了 `ProvideValue`，增加了 `SetAnchor` 逻辑。这是因为语言资源可能在非 StyledElement 上下文中使用（如 Application 级别的默认值），需要显式设置锚点以确保资源查找链路正确。

---

## 7. GetResourceKindType() 的闭环设计

与 Token 系统的 `GetTokenKindType()` 对应，每个 LanguageProvider 必须实现 `GetResourceKindType()`：

```csharp
protected override Type GetResourceKindType() => typeof(UploadLangResourceKind);
```

运行时 `LanguageProvider` 基类通过此方法获取枚举类型，将 `const string` 字段名与枚举成员进行匹配，构建 `ResourceKey → 文本值` 的映射。完整闭环：

```
en_US.Uploading = "Uploading..." (C# const 字段)
    ↓ 源生成器
UploadLangResourceKind.Uploading (枚举值)
    ↓ GetResourceKindType() + 反射匹配
ResourceDictionary[UploadLangResourceKind.Uploading] = "Uploading..."
    ↓ DynamicResourceExtension
{atom:UploadLangResource Uploading} (AXAML 消费)
    ↓ 语言切换
ResourceDictionary[UploadLangResourceKind.Uploading] = "上传中..."
```

---

## 8. 与 Token 生成器的对比

| 维度 | TokenResourceKeyGenerator | LanguageGenerator |
|---|---|---|
| 触发注解 | `[GlobalDesignToken]` / `[ControlDesignToken]` | `[LanguageProvider]` |
| 数据来源 | C# 属性（`{ get; set; }`） | C# `const string` 字段 |
| 分组键 | Token 类名 | LanguageId（注解第二个参数） |
| 生成枚举 | `SharedTokenKind` / `XxxTokenKind` | `XxxLangResourceKind` |
| 生成标记扩展 | `XxxTokenResourceExtension` | `XxxLangResourceExtension` |
| 注册池 | `ControlTokenTypePool`（返回 `Type` 列表） | `LanguageProviderPool`（返回实例列表） |
| 运行时基类 | `TokenResourceExtension<T>` | `LanguageResourceExtension<T>` |
| 闭环方法 | `GetTokenKindType()` | `GetResourceKindType()` |
| 动态切换 | 主题切换时重新计算 Token 值 | 语言切换时替换 ResourceDictionary |
| 目标命名空间 | `{控件NS}.DesignTokens` | `[LanguageSgMetaInfo]` 指定 |

