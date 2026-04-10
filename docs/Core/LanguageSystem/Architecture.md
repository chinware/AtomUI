# AtomUI 多语言系统 — 内部架构详解

> 本文档深入描述 AtomUI 国际化系统的内部实现细节，包括核心类型设计、Source Generator 管线、资源生命周期和运行时切换机制。

---

## 1. 核心类型体系

### 1.1 类型关系图

```
LanguageCode (enum)
    ↓ 被封装为
LanguageVariant (sealed record, TypeConverter)
    ↓ 作为分组键
Dictionary<LanguageVariant, ResourceDictionary>  ←── ThemeManager._languages

ILanguageProvider (interface)
    ↑ 实现
LanguageProvider (abstract class, 反射驱动)
    ↑ 继承
[LanguageProvider(code, langId)] 具体类 (如 Dialog.zh_CN)
    │
    ├── public const string 字段 → 语言文本
    └── GetResourceKindType() → 生成的枚举类型 (如 DialogLangResourceKind)

LanguageResourceExtension<TResourceKind> (abstract XAML markup extension)
    ↑ 继承
DialogLangResourceExtension (Source Generator 生成)
    → 在 AXAML 中作为 {atom:DialogLangResource Ok} 使用
```

### 1.2 LanguageCode

最小的语言标识枚举，当前支持两种语言：

```csharp
// src/AtomUI.Core/Language/LanguageCode.cs
public enum LanguageCode
{
    zh_CN,
    en_US,
}

public static class LanguageCodeExtensions
{
    // zh_CN → "zh-CN"（用于 CultureInfo 转换）
    public static string ToHyphenString(this LanguageCode code)
    {
        var name = code.ToString();
        return name.Replace('_', '-');
    }
}
```

### 1.3 LanguageVariant

`LanguageVariant` 是语言变体的**类型安全封装**，作为 `sealed record` 实现，具备以下特性：

- **单例模式** — 通过静态属性 `zh_CN` / `en_US` 提供预定义实例
- **Avalonia StyledProperty** — 定义了 `LanguageVariantProperty`，支持通过 Avalonia 属性系统进行绑定和监听
- **CultureInfo 互转** — `FromCultureInfo()` / `ToCultureInfo()` 实现 .NET 本地化互操作
- **TypeConverter** — `LanguageVariantTypeConverter` 支持 XAML 中字符串到 `LanguageVariant` 的转换
- **值相等** — 基于 `LanguageCode` 进行相等性比较

```csharp
// src/AtomUI.Core/Language/LanguageVariant.cs
[TypeConverter(typeof(LanguageVariantTypeConverter))]
public sealed record LanguageVariant
{
    // 预定义单例
    public static LanguageVariant zh_CN { get; } = new(LanguageCode.zh_CN);
    public static LanguageVariant en_US { get; } = new(LanguageCode.en_US);
    
    // Avalonia 属性 — ThemeManager 通过此属性监听语言切换
    internal static readonly StyledProperty<LanguageVariant> LanguageVariantProperty =
        AvaloniaProperty.Register<StyledElement, LanguageVariant>(
            "LanguageVariant", defaultValue: en_US);

    public LanguageCode Code { get; }
    public string DisplayText => GetDisplayText(); // "简体中文" / "English"
    
    // CultureInfo 互转
    public static LanguageVariant FromCultureInfo(CultureInfo cultureInfo) { ... }
    public CultureInfo ToCultureInfo() => CultureInfo.GetCultureInfo(ToString());
}
```

**设计要点**：`LanguageVariant` 使用私有构造函数，确保只能通过 `FromCode()` 或静态属性获取实例。这保证了字典键的引用一致性。

### 1.4 ILanguageProvider 与 LanguageProvider

```csharp
// src/AtomUI.Core/Language/ILanguageProvider.cs
public interface ILanguageProvider
{
    public LanguageCode LangCode { get; }   // 此 Provider 对应的语言
    public string LangId { get; }           // 分组标识（如 "Dialog"、"QRCode"）
    public void BuildResourceDictionary(IResourceDictionary dictionary);
}
```

`LanguageProvider` 抽象基类实现了核心的资源构建逻辑：

```csharp
// src/AtomUI.Core/Language/LanguageProvider.cs
public abstract class LanguageProvider : ILanguageProvider
{
    public LanguageCode LangCode { get; }
    public string LangId { get; }

    public LanguageProvider()
    {
        // 通过反射读取 [LanguageProvider] 特性获取元信息
        var type = GetType();
        var attribute = type.GetCustomAttribute<LanguageProviderAttribute>();
        LangCode = attribute.LanguageCode;
        LangId   = attribute.LanguageId;
    }

    public void BuildResourceDictionary(IResourceDictionary dictionary)
    {
        var type = GetType();
        var resourceKindType = GetResourceKindType(); // 获取生成的枚举类型
        
        // 反射获取所有 public static const string 字段
        var languageFields = type.GetFields(
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .ToDictionary(x => x.Name);
        
        // 遍历枚举值，将每个枚举值作为 key，对应字段值作为 value 写入字典
        foreach (var value in Enum.GetValues(resourceKindType))
        {
            var languageKey = Enum.GetName(resourceKindType, value);
            if (languageFields.TryGetValue(languageKey, out var field))
            {
                dictionary[value] = field.GetValue(this); // key=枚举值, value=字符串
            }
        }
    }

    protected abstract Type GetResourceKindType();
}
```

**关键设计**：

1. `BuildResourceDictionary` 使用 **枚举值作为 ResourceDictionary 的 key**（而非字符串），确保类型安全和编译期检查。
2. 反射发生在初始化阶段，之后资源字典可被高效查找。
3. `GetResourceKindType()` 返回 Source Generator 生成的枚举类型，实现了编译期到运行时的桥接。

### 1.5 LanguageProviderAttribute

```csharp
// src/AtomUI.Core/Language/LanguageProviderAttribute.cs
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class LanguageProviderAttribute : Attribute
{
    public const string DefaultLanguageId = "Default";
    public LanguageCode LanguageCode { get; }
    public string LanguageId { get; }

    public LanguageProviderAttribute(LanguageCode languageCode, 
                                     string languageId = DefaultLanguageId) { ... }
}
```

`LanguageProviderAttribute` 同时服务于两个阶段：
- **构建时**：Source Generator 通过 `ForAttributeWithMetadataName` 识别标注了此特性的类
- **运行时**：`LanguageProvider` 基类构造函数通过反射读取此特性获取 `LangCode` 和 `LangId`

### 1.6 LanguageSgMetaInfoAttribute

```csharp
// src/AtomUI.Core/Language/LanguageSgMetaInfoAttribute.cs
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class LanguageSgMetaInfoAttribute : Attribute
{
    public string? TargetNamespace { get; set; }
    
    public LanguageSgMetaInfoAttribute(string? targetNamespace) { ... }
}
```

这是一个**程序集级别**的特性，告诉 Source Generator 将生成的枚举和标记扩展输出到哪个命名空间。

示例：
```csharp
// AtomUI.Desktop.Controls/Properties/AssemblyInfo.cs
[assembly: LanguageSgMetaInfo("AtomUI.Desktop.Controls.Localization")]
```

这意味着该程序集中所有 `[LanguageProvider]` 类生成的枚举和标记扩展都将放在 `AtomUI.Desktop.Controls.Localization` 命名空间下。

### 1.7 LanguageResourceExtension\<T\>

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
        var provideTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        // 处理非 StyledElement 目标（如在 Style Setter 中使用）
        if (provideTarget?.TargetObject is not StyledElement)
        {
            this.SetAnchor(Application.Current);
        }
        return this;
    }
}
```

**关键设计**：

- 继承自 Avalonia 的 `DynamicResourceExtension`，这意味着语言文本绑定是**动态的** — 当对应的 `ResourceDictionary` 被替换时，所有绑定自动刷新。
- `SetAnchor` 处理了 AXAML 中在非 `StyledElement` 上下文（如 `ControlTheme` 的 `Setter`）中使用标记扩展的场景。

### 1.8 CommonLangId

```csharp
// src/AtomUI.Core/Language/CommonLangId.cs
public static class CommonLangId
{
    public const string Common = "Common";
}
```

`CommonLangId.Common` 用于标识跨控件共享的通用语言字符串（如 "确定"、"取消"、"提交" 等），定义在 `AtomUI.Controls/Localization/` 中。

---

## 2. Source Generator 管线

### 2.1 管线总览

```
┌───────────────────────────────────────────────────────────────┐
│  编译触发 Source Generator                                     │
│                                                               │
│  LanguageGenerator.Initialize()                               │
│    │                                                          │
│    ├── ForAttributeWithMetadataName(LanguageProviderAttribute)│
│    │     → 对每个匹配的类创建 LanguageProviderWalker          │
│    │     → Walker 提取：                                      │
│    │        ├── [LanguageSgMetaInfo] → TargetNamespace         │
│    │        ├── [LanguageProvider(code, langId)] → 元信息      │
│    │        ├── 类名、命名空间                                 │
│    │        └── 所有 public const string 字段名 → Items       │
│    │                                                          │
│    ├── .Collect() → List<LanguageInfo>                        │
│    │                                                          │
│    └── RegisterImplementationSourceOutput:                    │
│         ├── LangResourceKeyClassSourceWriter.Write()          │
│         │     → LanguageResourceConst.g.cs                    │
│         └── LanguageProviderPoolClassSourceWriter.Write()     │
│               → LanguageProviderPool.g.cs                     │
└───────────────────────────────────────────────────────────────┘
```

### 2.2 LanguageGenerator 入口

```csharp
// src/AtomUI.Generator/LanguageGenerator.cs
[Generator]
public class LanguageGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var languageProvider = initContext.SyntaxProvider.ForAttributeWithMetadataName(
            "AtomUI.Theme.Language.LanguageProviderAttribute",
            (node, token) => true, // 接受所有匹配节点
            (context, token) =>
            {
                var walker = new LanguageProviderWalker(context.SemanticModel);
                walker.Visit(context.TargetNode);
                return walker.LanguageInfo;
            }).Collect();
            
        initContext.RegisterImplementationSourceOutput(languageProvider, (context, providers) =>
        {
            var providerList = providers.ToList();
            // 生成资源键枚举 + 标记扩展
            new LangResourceKeyClassSourceWriter(context, providerList).Write();
            // 生成 LanguageProviderPool 工厂类
            new LanguageProviderPoolClassSourceWriter(context, providerList).Write();
        });
    }
}
```

### 2.3 LanguageProviderWalker

`LanguageProviderWalker` 是一个 `CSharpSyntaxWalker`，遍历每个 `[LanguageProvider]` 类的语法树，提取：

1. **程序集级元信息**：从 `[assembly: LanguageSgMetaInfo("...")]` 读取 `TargetNamespace`
2. **类级元信息**：从 `[LanguageProvider(LanguageCode.xx, "LangId")]` 读取语言代码和分组 ID
3. **字段定义**：所有 `public const string` 字段的名称和初始值

提取结果存入 `LanguageInfo` 数据类：

```csharp
// src/AtomUI.Generator/Language/LanguageInfo.cs
internal class LanguageInfo
{
    public string? TargetNamespace { get; internal set; }  // 生成代码的目标命名空间
    public string Namespace { get; internal set; }         // 原始类的命名空间
    public string LanguageId { get; internal set; }        // 分组 ID（如 "Dialog"）
    public string LanguageCode { get; internal set; }      // 语言代码（如 "zh_CN"）
    public string ClassName { get; internal set; }         // 类名（如 "zh_CN"）
    public Dictionary<string, string> Items { get; }       // 字段名 → 初始值
}
```

### 2.4 端到端示例：Dialog 控件从定义到生成

下面以 Dialog 控件为完整示例，展示从开发者编写的 `LanguageProvider` 源码，经过 Source Generator 处理后，最终生成哪些代码。

#### 输入：开发者编写的两个 LanguageProvider 类

```csharp
// Dialog/Localization/zh_CN.cs
namespace AtomUI.Desktop.Controls.DialogLang;

[LanguageProvider(LanguageCode.zh_CN, DialogToken.ID)]  // DialogToken.ID = "Dialog"
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

```csharp
// Dialog/Localization/en_US.cs
namespace AtomUI.Desktop.Controls.DialogLang;

[LanguageProvider(LanguageCode.en_US, DialogToken.ID)]
internal class en_US : LanguageProvider
{
    public const string Ok = "OK";
    public const string Open = "Open";
    public const string Save = "Save";
    public const string Cancel = "Cancel";
    public const string Close = "Close";
    public const string Discard = "Discard";
    public const string Apply = "Apply";
    public const string Reset = "Reset";
    public const string Reload = "Reload";
    public const string RestoreDefaults = "Restore Defaults";
    public const string Help = "Help";
    public const string SaveAll = "Save All";
    public const string Yes = "Yes";
    public const string YesToAll = "Yes to All";
    public const string No = "No";
    public const string NoToAll = "No to All";
    public const string Abort = "Abort";
    public const string Retry = "Retry";
    public const string Ignore = "Ignore";
    
    protected override Type GetResourceKindType() => typeof(DialogLangResourceKind);
}
```

#### Walker 提取：两个 LanguageInfo

Source Generator 触发后，`LanguageProviderWalker` 分别遍历这两个类，提取出两个 `LanguageInfo`：

```
LanguageInfo #1:
  TargetNamespace = "AtomUI.Desktop.Controls.Localization"  ← 来自 [assembly: LanguageSgMetaInfo]
  Namespace       = "AtomUI.Desktop.Controls.DialogLang"
  LanguageId      = "Dialog"                                ← 来自 [LanguageProvider] 第二个参数
  LanguageCode    = "zh_CN"
  ClassName       = "zh_CN"
  Items           = { Ok, Open, Save, Cancel, Close, Discard, Apply, Reset, Reload,
                      RestoreDefaults, Help, SaveAll, Yes, YesToAll, No, NoToAll,
                      Abort, Retry, Ignore }

LanguageInfo #2:
  TargetNamespace = "AtomUI.Desktop.Controls.Localization"
  Namespace       = "AtomUI.Desktop.Controls.DialogLang"
  LanguageId      = "Dialog"
  LanguageCode    = "en_US"
  ClassName       = "en_US"
  Items           = { Ok, Open, Save, Cancel, Close, Discard, Apply, Reset, Reload,
                      RestoreDefaults, Help, SaveAll, Yes, YesToAll, No, NoToAll,
                      Abort, Retry, Ignore }
```

#### 分组：按 LangId 合并

`LangResourceKeyClassSourceWriter` 将两个 `LanguageInfo` 按 `LanguageId = "Dialog"` 分为一组，然后对所有 Items 的字段名取**并集**并按**字母排序**：

```
LangId = "Dialog"
合并后的字段名（排序）: Abort, Apply, Cancel, Close, Discard, Help, Ignore,
                       No, NoToAll, Ok, Open, Reload, Reset, RestoreDefaults,
                       Retry, Save, SaveAll, Yes, YesToAll
```

#### 输出：生成的 `LanguageResourceConst.g.cs`（Dialog 部分）

```csharp
// GeneratedFiles/AtomUI.Generator/AtomUI.Generator.LanguageGenerator/LanguageResourceConst.g.cs
namespace AtomUI.Desktop.Controls.Localization  // ← TargetNamespace
{
    // 生成物 1：资源键枚举
    public enum DialogLangResourceKind
    {
        Abort,
        Apply,
        Cancel,
        Close,
        Discard,
        Help,
        Ignore,
        No,
        NoToAll,
        Ok,
        Open,
        Reload,
        Reset,
        RestoreDefaults,
        Retry,
        Save,
        SaveAll,
        Yes,
        YesToAll
    }

    // 生成物 2：XAML 标记扩展
    public class DialogLangResourceExtension : LanguageResourceExtension<DialogLangResourceKind>
    {
        public DialogLangResourceExtension() { }
        public DialogLangResourceExtension(DialogLangResourceKind kind) : base(kind) { }
    }
}
```

#### 运行时：枚举值如何变成 ResourceDictionary 的键

当 `ThemeManager.BuildLanguageResources()` 初始化时，`LanguageProvider.BuildResourceDictionary()` 被调用。以 `DialogLang.zh_CN` 为例：

```
遍历 DialogLangResourceKind 枚举的每个值：
  DialogLangResourceKind.Abort  → 查找字段 "Abort"  → 值 "中止"  → dictionary[Abort枚举值]  = "中止"
  DialogLangResourceKind.Apply  → 查找字段 "Apply"  → 值 "应用"  → dictionary[Apply枚举值]  = "应用"
  DialogLangResourceKind.Cancel → 查找字段 "Cancel" → 值 "取消"  → dictionary[Cancel枚举值] = "取消"
  ...
  DialogLangResourceKind.YesToAll → 查找字段 "YesToAll" → 值 "全部是" → dictionary[YesToAll枚举值] = "全部是"
```

最终 `_languages[LanguageVariant.zh_CN]` 这个 `ResourceDictionary` 中包含：

```
Key (枚举值)                          → Value (字符串)
DialogLangResourceKind.Abort          → "中止"
DialogLangResourceKind.Apply          → "应用"
DialogLangResourceKind.Cancel         → "取消"
...
DialogLangResourceKind.YesToAll       → "全部是"
```

而 `_languages[LanguageVariant.en_US]` 中，同样的枚举键对应英文值：

```
DialogLangResourceKind.Abort          → "Abort"
DialogLangResourceKind.Apply          → "Apply"
DialogLangResourceKind.Cancel         → "Cancel"
...
DialogLangResourceKind.YesToAll       → "Yes to All"
```

语言切换时，只需替换 `MergedDictionaries` 中的 `ResourceDictionary`，所有通过 `{atom:DialogLangResource Ok}` 绑定的控件文本自动更新。

### 2.5 LangResourceKeyClassSourceWriter

在理解了完整示例后，总结 `LangResourceKeyClassSourceWriter` 的生成规则：

按 `LangId` 分组后，为每组生成两个类型：

#### 生成物 1：`{LangId}LangResourceKind` 枚举

- 枚举名 = `{LangId}LangResourceKind`（如 `DialogLangResourceKind`）
- 枚举成员 = 同组所有 LanguageProvider 的字段名取**并集**
- **枚举值按字母顺序排列**（`sortedKeys = keys.ToList().OrderBy(key => key)`），保证生成结果的确定性
- 命名空间 = `TargetNamespace`（来自 `[assembly: LanguageSgMetaInfo]`），若未指定则使用原始类的命名空间

#### 生成物 2：`{LangId}LangResourceExtension` 标记扩展类

- 类名 = `{LangId}LangResourceExtension`（如 `DialogLangResourceExtension`）
- 继承自 `LanguageResourceExtension<{LangId}LangResourceKind>`
- 提供无参和带参两个构造函数

这个类可在 AXAML 中直接使用：

```xml
<TextBlock Text="{atom:DialogLangResource Ok}" />
```

**命名空间规则**：
- 如果 `[assembly: LanguageSgMetaInfo("AtomUI.Desktop.Controls.Localization")]` 存在，使用指定的 `TargetNamespace`
- 否则，使用原始 LanguageProvider 类的命名空间

### 2.6 LanguageProviderPoolClassSourceWriter

生成一个固定命名空间 `AtomUI.Theme.Language` 下的工厂类：

```csharp
// 生成文件：LanguageProviderPool.g.cs
namespace AtomUI.Theme.Language
{
    internal class LanguageProviderPool
    {
        internal static IList<LanguageProvider> GetLanguageProviders()
        {
            List<LanguageProvider> languageProviders = new List<LanguageProvider>();
            languageProviders.Add(new AtomUI.Desktop.Controls.DatePickerLang.en_US());
            languageProviders.Add(new AtomUI.Desktop.Controls.DatePickerLang.zh_CN());
            languageProviders.Add(new AtomUI.Desktop.Controls.DialogLang.en_US());
            languageProviders.Add(new AtomUI.Desktop.Controls.DialogLang.zh_CN());
            // ... 所有注册的 LanguageProvider
            return languageProviders;
        }
    }
}
```

**注意**：每个引用了 `AtomUI.Generator` 的程序集都会独立生成自己的 `LanguageProviderPool`，因此不会出现跨程序集冲突。Provider 列表按 `Namespace` + `ClassName` 排序，保证确定性。

---

## 3. 资源生命周期

### 3.1 初始化流程

```
Application.UseAtomUI(builder => { ... })
│
├── 1. new ThemeManagerBuilder()
│      └── 默认 LanguageVariant = en_US
│
├── 2. builder.WithDefaultLanguageVariant(LanguageVariant.zh_CN)
│      └── 用户指定默认语言
│
├── 3. builder.UseDesktopControls()
│      ├── UseCommonControls()
│      │     └── AtomUI.Controls 的 LanguageProviderPool.GetLanguageProviders()
│      │           → 注册 Common 通用语言 Provider (Ok, Cancel, Submit, ...)
│      └── AtomUI.Desktop.Controls 的 LanguageProviderPool.GetLanguageProviders()
│            → 注册所有控件语言 Provider (Dialog, QRCode, DatePicker, ...)
│
├── 4. ThemeManagerBuilder.Build()
│      └── 创建 ThemeManager，注册所有 LanguageProvider
│           themeManager.RegisterLanguageProvider(provider)
│
├── 5. ThemeManager.Configure()
│      └── BuildLanguageResources()
│           │
│           │  对每个 ILanguageProvider：
│           │    languageVariant = LanguageVariant.FromCode(provider.LangCode)
│           │    _languages[languageVariant] ??= new ResourceDictionary()
│           │    provider.BuildResourceDictionary(_languages[languageVariant])
│           │
│           └── 结果：_languages 字典包含：
│                ├── LanguageVariant.zh_CN → ResourceDictionary {
│                │     QRCodeLangResourceKind.Expired = "二维码过期",
│                │     QRCodeLangResourceKind.Refresh = "点击刷新",
│                │     DialogLangResourceKind.Ok = "确定",
│                │     CommonLangResourceKind.Cancel = "取消",
│                │     ... 
│                │   }
│                └── LanguageVariant.en_US → ResourceDictionary {
│                      QRCodeLangResourceKind.Expired = "QR code expired",
│                      QRCodeLangResourceKind.Refresh = "Refresh",
│                      DialogLangResourceKind.Ok = "OK",
│                      CommonLangResourceKind.Cancel = "Cancel",
│                      ...
│                    }
│
├── 6. SetValue(LanguageVariantProperty, zh_CN)
│      └── 触发 OnPropertyChanged → 将 zh_CN 对应的 ResourceDictionary
│           加入 ThemeManager.Resources.MergedDictionaries
│
└── 7. ThemeManager.AttachApplication(application)
       └── ThemeManager 作为 Styles 加入 application.Styles
            → 语言 ResourceDictionary 进入全局资源树
```

### 3.2 资源合并结构

初始化完成后，Avalonia 资源树结构如下：

```
Application
└── Styles
    └── ThemeManager (Styles)
        └── Resources
            └── MergedDictionaries
                ├── ControlTheme ResourceDictionaries (控件主题)
                ├── Token ResourceDictionaries (设计令牌)
                └── Language ResourceDictionary (当前语言)
                      ├── QRCodeLangResourceKind.Expired = "二维码过期"
                      ├── QRCodeLangResourceKind.Refresh = "点击刷新"
                      ├── CommonLangResourceKind.Ok = "确定"
                      └── ...
```

### 3.3 所有语言的资源合并到单个 ResourceDictionary

**重要**：同一语言（`LanguageVariant`）下所有控件的语言资源被合并到**同一个 `ResourceDictionary`** 中。这是因为：

1. 不同控件的 `LangResourceKind` 枚举类型不同（`QRCodeLangResourceKind` vs `DialogLangResourceKind`），枚举值作为键不会冲突
2. 运行时切换语言只需**替换一个 ResourceDictionary**，效率最高

---

## 4. 运行时语言切换

### 4.1 切换流程

```
application.SetLanguageVariant(LanguageVariant.en_US)
│
├── themeManager.LanguageVariant = LanguageVariant.en_US
│     └── SetValue(LanguageVariantProperty, en_US)
│
└── ThemeManager.OnPropertyChanged()
      │
      ├── 移除旧语言资源：
      │     Resources.MergedDictionaries.Remove(oldResource)
      │     // 移除 zh_CN 的 ResourceDictionary
      │
      ├── 添加新语言资源：
      │     Resources.MergedDictionaries.Add(newResource)
      │     // 添加 en_US 的 ResourceDictionary
      │
      └── 触发事件：
            LanguageVariantChanged?.Invoke(...)
            // → DynamicResource 绑定自动刷新
            // → 所有使用 {atom:XxxLangResource Key} 的控件文本立即更新
```

### 4.2 DynamicResource 自动刷新机制

语言切换能实现"无刷新"热更新，关键在于：

1. **枚举值作为 ResourceKey** — `LanguageResourceExtension<T>` 继承自 `DynamicResourceExtension`，将枚举值设置为 `ResourceKey`
2. **ResourceDictionary 替换** — 当 `MergedDictionaries` 中的字典被替换时，Avalonia 自动通知所有通过 `DynamicResource` 绑定到被替换字典中键的控件
3. **相同的键，不同的值** — 新旧语言的 `ResourceDictionary` 使用相同的枚举值作为键，但对应不同的翻译文本

因此，整个切换过程对上层控件完全透明。

### 4.3 便捷 API

```csharp
// 获取当前语言
LanguageVariant? lang = Application.Current.GetLanguageVariant();

// 设置语言（推荐在 UI 线程执行）
Dispatcher.UIThread.Post(() =>
{
    Application.Current.SetLanguageVariant(LanguageVariant.zh_CN);
});

// 监听语言变化
var themeManager = Application.Current.GetThemeManager();
themeManager.LanguageVariantChanged += (sender, args) =>
{
    var oldLang = args.OldLanguage;
    var newLang = args.NewLanguage;
    // 处理语言变化
};
```

---

## 5. 命名空间约定

### 5.1 LanguageProvider 类的命名空间

每个控件的 LanguageProvider 类使用独立的命名空间，命名规则：

```
{项目根命名空间}.{控件名}Lang
```

| 控件 | 命名空间 | 示例 |
|------|---------|------|
| Dialog | `AtomUI.Desktop.Controls.DialogLang` | `DialogLang.zh_CN` |
| QRCode | `AtomUI.Desktop.Controls.QRCodeLang` | `QRCodeLang.en_US` |
| DatePicker | `AtomUI.Desktop.Controls.DatePickerLang` | `DatePickerLang.zh_CN` |
| Common (共享) | `AtomUI.Controls.Localization` | `Localization.zh_CN` |

### 5.2 生成代码的命名空间

由 `[assembly: LanguageSgMetaInfo("...")]` 指定，通常为：

| 项目 | TargetNamespace |
|------|----------------|
| `AtomUI.Controls` | `AtomUI.Controls.Localization` |
| `AtomUI.Desktop.Controls` | `AtomUI.Desktop.Controls.Localization` |
| `AtomUI.Desktop.Controls.ColorPicker` | `AtomUI.Desktop.Controls.Localization` |
| `AtomUI.Desktop.Controls.DataGrid` | `AtomUI.Desktop.Controls.Localization` |
| `AtomUIGallery` | `AtomUIGallery.Localization` |

### 5.3 XAML 命名空间注册

生成的标记扩展要在 AXAML 中使用 `atom:` 前缀，需要在 `AssemblyInfo.cs` 中注册命名空间映射：

```csharp
[assembly: XmlnsDefinition("https://atomui.net", "AtomUI.Desktop.Controls.Localization")]
```

这样在 AXAML 中就可以直接使用：

```xml
<TextBlock Text="{atom:QRCodeLangResource Expired}" />
```

---

## 6. LangId 与控件 Token ID 的关系

在 AtomUI 中，`LangId`（语言分组标识）通常与控件的 **Token ID** 保持一致：

```csharp
// DialogToken.cs
[ControlDesignToken]
internal class DialogToken : AbstractControlDesignToken
{
    public const string ID = "Dialog";
    // ...
}

// Dialog/Localization/zh_CN.cs
[LanguageProvider(LanguageCode.zh_CN, DialogToken.ID)]  // LangId = "Dialog"
internal class zh_CN : LanguageProvider { ... }
```

这种约定有以下好处：
- **一致性** — 控件的 Token、语言资源使用相同的标识符
- **可发现性** — 通过控件名即可推断 LangId
- **规范性** — 减少命名决策，降低出错概率

对于非控件级别的语言资源（如 Gallery 应用的菜单项），可以自定义 LangId：

```csharp
// WorkspaceWindow.cs
public partial class WorkspaceWindow
{
    public const string LanguageId = "WorkspaceWindow";
}

// 语言 Provider
[LanguageProvider(LanguageCode.zh_CN, WorkspaceWindow.LanguageId)]
internal class zh_CN : LanguageProvider { ... }
```

---

## 7. 与主题系统的集成

多语言系统与主题系统（Theme System）在 `ThemeManager` 中并行管理：

```
ThemeManager (Styles)
├── Resources.MergedDictionaries
│     ├── 控件主题 ResourceDictionary (ControlTheme)
│     ├── Token ResourceDictionary (设计令牌)
│     └── Language ResourceDictionary (当前语言)  ← 多语言系统管理
│
├── ThemeVariant ← 主题变体（切换主题）
├── LanguageVariant ← 语言变体（切换语言）
├── IsDarkThemeMode ← 暗色模式
├── IsCompactThemeMode ← 紧凑模式
└── IsMotionEnabled ← 动效开关
```

两个系统独立运作：
- **主题切换** — 替换整个 Theme 的 Token ResourceDictionary
- **语言切换** — 仅替换 Language ResourceDictionary

因此可以独立切换主题和语言，互不影响。

