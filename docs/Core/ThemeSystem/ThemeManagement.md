# AtomUI 主题系统 — 主题管理器与主题生命周期

> 本文档详细描述 AtomUI 主题管理器（ThemeManager）的架构设计、主题的完整生命周期、运行时主题切换机制，以及局部主题配置（ThemeConfigProvider）的实现。

---

## 1. 主题管理器架构

### 1.1 ThemeManager 概述

`ThemeManager` 是 AtomUI 主题系统的核心管理器，继承自 Avalonia 的 `Styles` 类，实现 `IThemeManager` 接口。它承担以下职责：

- **主题池管理** — 扫描、创建、缓存所有主题变体
- **主题切换** — 运行时激活/切换主题
- **语言管理** — 多语言资源的加载和切换
- **全局开关** — 动效开关、波浪动画开关、暗色模式、紧凑模式

### 1.2 类继承结构

```
Avalonia.Styling.Styles
        │
        ▼
ThemeManager : Styles, IThemeManager
├── ThemePool: Dictionary<ThemeVariant, Theme>
├── ControlTokenTypes: List<Type>
├── Languages: Dictionary<LanguageVariant, ResourceDictionary>
├── ControlThemesProviders: IList<IControlThemesProvider>
└── ThemeVariantCalculatorFactory: IThemeVariantCalculatorFactory?
```

`ThemeManager` 继承 `Styles` 使其能够直接挂载到 `Application.Styles` 上，参与 Avalonia 的样式/资源解析链。

### 1.3 IThemeManager 接口

```csharp
public interface IThemeManager
{
    const string DEFAULT_THEME_ID = "DaybreakBlue";
    
    IReadOnlyCollection<ITheme> AvailableThemes { get; }
    ITheme? ActivatedTheme { get; }
    AvaloniaObject BindingSource { get; }
    
    LanguageVariant LanguageVariant { get; set; }
    bool IsMotionEnabled { get; set; }
    bool IsWaveSpiritEnabled { get; set; }
    bool IsDarkThemeMode { get; set; }
    bool IsCompactThemeMode { get; set; }
    
    event EventHandler<LanguageVariantChangedEventArgs>? LanguageVariantChanged;
}
```

### 1.4 ThemeManager 的 StyledProperty

| 属性 | 类型 | 说明 |
|------|------|------|
| `ThemeVariantProperty` | `ThemeVariant` | 当前主题变体 |
| `LanguageVariantProperty` | `LanguageVariant` | 当前语言 |
| `IsMotionEnabledProperty` | `bool` | 是否启用动效 |
| `IsWaveSpiritEnabledProperty` | `bool` | 是否启用波浪动画 |
| `IsDarkThemeModeProperty` | `bool` | 是否为暗色模式 |
| `IsCompactThemeModeProperty` | `bool` | 是否为紧凑模式 |

这些属性支持数据绑定，控件可以通过绑定感知当前主题状态。

---

## 2. Builder 模式初始化

### 2.1 ThemeManagerBuilder

`ThemeManagerBuilder` 采用 Builder 模式收集所有主题相关的配置：

```csharp
public interface IThemeManagerBuilder
{
    IList<Type> ControlDesignTokens { get; }
    IList<IThemeAssetPathProvider> ThemeAssetPathProviders { get; }
    IList<IControlThemesProvider> ControlThemesProviders { get; }
    IList<LanguageProvider> LanguageProviders { get; }
    IList<EventHandler> InitializedHandlers { get; }
    
    void AddControlToken(Type tokenType);
    void AddControlThemesProvider(IControlThemesProvider provider);
    void AddControlThemesProvider(IThemeAssetPathProvider provider);
    void AddLanguageProviders(LanguageProvider provider);
    
    void WithDefaultTheme(string themeId);
    void WithDefaultFontFamily(FontFamily fontFamily);
    void WithDefaultLanguageVariant(LanguageVariant variant);
    void WithThemeVariantCalculatorFactory(IThemeVariantCalculatorFactory factory);
}
```

### 2.2 应用入口初始化流程

```csharp
// ApplicationExtensions.cs
app.UseAtomUI(builder =>
{
    // 注册桌面控件库
    builder.UseDesktopControls();      // 注册 Token + Theme + Language
    
    // 可选配置
    builder.WithDefaultTheme("DaybreakBlue");
    builder.WithDefaultLanguageVariant(LanguageVariant.zh_CN);
    builder.WithDefaultFontFamily("Microsoft YaHei");
});
```

### 2.3 完整初始化流程

```
UseAtomUI(Action<IThemeManagerBuilder> configure)
│
├── 1. new ThemeManagerBuilder()
│
├── 2. configure(builder)
│      ├── UseDesktopControls() 注册所有 DesktopControls 的:
│      │   ├── ControlDesignTokens (ButtonToken, TagToken, ...)
│      │   ├── ControlThemesProviders (DesktopControlThemesProvider)
│      │   ├── LanguageProviders (en_US, zh_CN)
│      │   └── ThemeAssetPathProviders
│      ├── WithDefaultTheme("DaybreakBlue")
│      └── WithDefaultLanguageVariant(...)
│
├── 3. builder.Build() → ThemeManager
│      ├── 注册所有 ControlThemesProvider
│      ├── 注册所有 ControlTokenType
│      ├── 注册所有 ThemeAssetPathProvider
│      └── 注册所有 LanguageProvider
│
├── 4. themeManager.Configure()
│      ├── ScanThemes()
│      │   ├── 扫描自定义主题目录
│      │   ├── 扫描内置主题目录
│      │   ├── 扫描 ThemeAssetPathProviders
│      │   └── 扫描 Assets 内嵌资源
│      │   对每个主题定义文件 × 4 种算法组合 → 创建 Theme 实例
│      │
│      ├── 合并 ControlThemes 到 Resources.MergedDictionaries
│      └── BuildLanguageResources()
│
├── 5. AvaloniaLocator 注册 ThemeManager 单例
│      ThemeManager.Current = themeManager
│
├── 6. themeManager.NotifyInitialized()
│
├── 7. Application.RequestedThemeVariant = 计算的 ThemeVariant
│
├── 8. themeManager.LanguageVariant = 配置的语言
│
└── 9. themeManager.AttachApplication(app)
       ├── 绑定 ThemeVariant ← Application.ActualThemeVariant
       ├── ConfigureThemeVariant() → 加载并激活主题
       └── app.Styles.Add(themeManager)  ← 挂入样式链
```

---

## 3. Theme（主题实例）

### 3.1 ITheme 接口

```csharp
public interface ITheme
{
    string Id { get; }
    string DisplayName { get; }
    bool IsLoaded { get; }
    bool IsDarkMode { get; }
    bool IsActivated { get; }
    bool IsPrimary { get; }
    bool IsBuiltIn { get; }
    List<string> ThemeResourceKeys { get; }
    IControlDesignToken? GetControlToken(string tokenId);
    DesignToken SharedToken { get; }
    ThemeVariant ThemeVariant { get; }
    IList<ThemeAlgorithm> Algorithms { get; }
    ResourceDictionary ThemeResource { get; }
}
```

### 3.2 Theme 内部结构

```csharp
internal class Theme : AvaloniaObject, ITheme
{
    // 核心数据
    private DesignToken _sharedToken;                              // 全局 SharedToken
    private Dictionary<string, IControlDesignToken> ControlTokens; // 组件 Token 池
    private ResourceDictionary ResourceDictionary;                 // 主题资源字典
    private ThemeDefinition ThemeDefinition;                       // 主题定义（从 XML 解析）
    
    // 状态
    private bool Loaded, Activated;
    private bool _isPrimary;              // 算法组合是否与定义文件一致
    private IList<ThemeAlgorithm> _algorithms;
    private ThemeVariant _themeVariant;
    
    // 关键方法
    internal void Load();                 // 加载主题
    internal void BuildThemeResource();   // 构建资源
}
```

### 3.3 ThemeVariant 命名规则

```
ThemeVariant = "{ThemeId}[-Dark][-Compact]"

示例：
  DaybreakBlue                → Default 算法
  DaybreakBlue-Dark           → Default + Dark
  DaybreakBlue-Compact        → Default + Compact
  DaybreakBlue-Dark-Compact   → Default + Dark + Compact
```

---

## 4. 主题生命周期

### 4.1 生命周期事件

```
ThemeCreated           → 主题实例创建完成
ThemeAboutToLoad       → 即将加载主题
ThemeLoaded            → 主题加载完成
ThemeLoadFailed        → 主题加载失败
ThemeAboutToChange     → 即将切换主题
ThemeChanged           → 主题切换完成
ThemeAboutToUnload     → 即将卸载主题
ThemeUnloaded          → 主题卸载完成
```

### 4.2 完整生命周期流程

```
┌───────────────────────────────────────────────┐
│               ScanThemes()                     │
│  扫描 XML 文件 → 为每个文件 × 4 种算法组合创建  │
│  Theme 实例 → 注册到 ThemePool                  │
└────────────────────┬──────────────────────────┘
                     │ ThemeCreated
                     ▼
┌───────────────────────────────────────────────┐
│            LoadTheme(themeVariant)              │
│                                                │
│  ┌──────────────────────────────────────────┐  │
│  │ Theme.Load()                              │  │
│  │                                           │  │
│  │ 1. NotifyLoadThemeDef()                   │  │
│  │    └── ThemeDefinitionReader.Load()        │  │
│  │        解析 XML → ThemeDefinition          │  │
│  │                                           │  │
│  │ 2. BuildThemeResource()                   │  │
│  │    ├── 构建算法链                           │  │
│  │    ├── Seed → LoadConfig()                 │  │
│  │    ├── Calculator.Calculate() → Map Token  │  │
│  │    ├── Map → LoadConfig() (用户覆盖)       │  │
│  │    ├── CalculateAliasTokenValues()         │  │
│  │    ├── Alias → LoadConfig() (用户覆盖)     │  │
│  │    ├── SharedToken.BuildResourceDictionary()│ │
│  │    ├── CollectControlTokens()              │  │
│  │    ├── 为每个 ControlToken:                │  │
│  │    │   ├── AssignSharedToken()             │  │
│  │    │   ├── CalculateTokenValues(isDark)    │  │
│  │    │   ├── LoadConfig() (用户覆盖)         │  │
│  │    │   └── BuildResourceDictionary()       │  │
│  │    └── BuildSharedResourceDeltaDictionary() │  │
│  └──────────────────────────────────────────┘  │
│                                                │
└────────────────────┬──────────────────────────┘
                     │ ThemeLoaded
                     ▼
┌───────────────────────────────────────────────┐
│         SetActiveTheme(themeVariant)            │
│                                                │
│  1. oldTheme.NotifyAboutToDeActive()           │
│  2. theme.NotifyAboutToActive()                │
│  3. Resources.ThemeDictionaries[variant]        │
│       = theme.ThemeResource                    │
│  4. oldTheme.NotifyDeActivated()               │
│  5. theme.NotifyActivated()                    │
│  6. 更新 IsDarkThemeMode / IsCompactThemeMode  │
│                                                │
└────────────────────┬──────────────────────────┘
                     │ ThemeChanged
                     ▼
┌───────────────────────────────────────────────┐
│                Running (Active)                │
│                                                │
│  控件通过 DynamicResource 消费 Token           │
│  运行时可切换: IsDarkThemeMode / IsCompactMode │
│                                                │
└───────────────────────────────────────────────┘
```

---

## 5. 主题定义文件（XML）

### 5.1 文件结构

主题通过 XML 文件定义，存放在：
- 内嵌资源：`AtomUI.Core/Assets/Themes/*.xml`
- 文件系统：自定义路径或应用数据目录

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Theme Name="Daybreak Blue" IsDefault="true">
    <!-- 全局 Seed/Map/Alias Token 覆盖 -->
    <SharedTokens>
        <Token Name="ColorPrimary" Value="#1677ff" />
        <Token Name="FontSize" Value="14" />
    </SharedTokens>
    
    <!-- 组件级 Token 覆盖 -->
    <ControlTokens>
        <ControlToken Id="Button" EnableAlgorithm="true">
            <!-- IsShared="true" 表示覆盖组件的 SharedToken -->
            <Token Name="ColorPrimary" Value="#ff0000" IsShared="true" />
            <!-- 不带 IsShared 表示覆盖组件自己的 Token -->
            <Token Name="PrimaryColor" Value="#ffffff" />
        </ControlToken>
    </ControlTokens>
</Theme>
```

### 5.2 ThemeDefinitionReader

`ThemeDefinitionReader` 使用 `XmlReader` 流式解析 XML：

| 元素 | 说明 |
|------|------|
| `<Theme>` | 根元素，包含 `Name`（显示名）和 `IsDefault` 属性 |
| `<Algorithms>` | 算法列表（逗号分隔），如 `Default, Dark` |
| `<SharedTokens>` | 全局 Token 覆盖区域 |
| `<ControlTokens>` | 组件 Token 覆盖区域 |
| `<ControlToken>` | 单个组件配置，`Id` 对应 Token 类的 ID，`EnableAlgorithm` 控制是否重新推导 |
| `<Token>` | 单个 Token 设置，`Name` + `Value`，可选 `IsShared` |

### 5.3 ThemeDefinition 数据模型

```csharp
internal class ThemeDefinition
{
    public string Id { get; }
    public string DisplayName { get; set; }
    public bool IsDefault { get; set; }
    public ISet<ThemeAlgorithm> Algorithms { get; set; }
    public IDictionary<string, ControlTokenConfigInfo> ControlTokens { get; set; }
    public IDictionary<string, string> SharedTokens { get; set; }
}
```

### 5.4 主题扫描策略

`ThemeManager.ScanThemes()` 按优先级从高到低扫描：

```
1. 自定义主题目录（用户指定路径）
2. 内置主题目录（AppData/AtomUIApplication/Themes/）
3. ThemeAssetPathProviders（各控件库提供的路径）
4. 内嵌资源（avares://AtomUI.Core/Assets/Themes/）
```

同名主题只保留优先级最高的版本。每个主题文件自动生成 4 种算法组合变体。

---

## 6. 运行时主题切换

### 6.1 切换暗色/紧凑模式

```csharp
// 通过 ThemeManager 属性切换
ThemeManager.Current.IsDarkThemeMode = true;     // 切换到暗色
ThemeManager.Current.IsCompactThemeMode = true;  // 开启紧凑
```

内部流程：

```
IsDarkThemeMode = true
    │
    ▼
OnPropertyChanged()
    │
    ├── ConfigureActiveThemeAlgorithms()
    │   ├── 构建新算法列表 [Default, Dark]
    │   └── Application.RequestedThemeVariant = "DaybreakBlue-Dark"
    │
    ▼
Application.ActualThemeVariant 变化
    │
    ▼
ThemeManager.ThemeVariant 变化（双向绑定）
    │
    ▼
ConfigureThemeVariant("DaybreakBlue-Dark")
    │
    ├── SetActiveTheme("DaybreakBlue-Dark")
    │   ├── 如果未加载 → LoadTheme() → 构建所有 Token
    │   ├── 切换 ResourceDictionary
    │   └── 触发 ThemeChanged 事件
    │
    └── 更新 IsDarkThemeMode / IsCompactThemeMode
```

### 6.2 运行时动效/波浪开关

```csharp
ThemeManager.Current.IsMotionEnabled = false;      // 关闭动效
ThemeManager.Current.IsWaveSpiritEnabled = false;   // 关闭波浪
```

这直接修改当前激活主题的 `ResourceDictionary` 中的对应 Token：

```csharp
private void ConfigureEnableMotion()
{
    var themeResource = Resources.ThemeDictionaries[ThemeVariant];
    if (themeResource is ResourceDictionary dict)
    {
        dict[SharedTokenKind.EnableMotion] = IsMotionEnabled;
    }
}
```

### 6.3 资源字典挂载机制

```
Application.Styles
    └── ThemeManager (extends Styles)
            ├── Resources.MergedDictionaries
            │   ├── ControlTheme1.axaml (Button 的 ControlTheme)
            │   ├── ControlTheme2.axaml (Tag 的 ControlTheme)
            │   ├── ...
            │   └── LanguageResource (当前语言的 ResourceDictionary)
            │
            └── Resources.ThemeDictionaries
                ├── "DaybreakBlue" → ResourceDictionary (all tokens)
                ├── "DaybreakBlue-Dark" → ResourceDictionary
                ├── "DaybreakBlue-Compact" → ResourceDictionary
                └── "DaybreakBlue-Dark-Compact" → ResourceDictionary
```

Avalonia 在解析 `DynamicResource` 时，会根据当前 `ThemeVariant` 从 `ThemeDictionaries` 中选择对应的 `ResourceDictionary`，实现主题切换。

---

## 7. ThemeConfigProvider（局部主题配置）

### 7.1 概述

`ThemeConfigProvider` 是一个 XAML 组件，实现了 Ant Design `ConfigProvider` 的**嵌套主题**功能。它允许在视觉树的某个子树中覆盖 Token 值，而不影响全局主题。

### 7.2 使用方式

```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.Algorithms>
        <sys:String>Dark</sys:String>
    </atom:ThemeConfigProvider.Algorithms>
    
    <atom:ThemeConfigProvider.SharedTokenSetters>
        <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
        <atom:TokenSetter Key="BorderRadius" Value="2" />
    </atom:ThemeConfigProvider.SharedTokenSetters>
    
    <atom:ThemeConfigProvider.ControlTokenInfoSetters>
        <atom:ControlTokenInfoSetter TokenId="Button" EnableAlgorithm="True">
            <atom:ControlTokenSetter Key="PrimaryColor" Value="#ffffff" />
            <atom:TokenSetter Key="ColorPrimary" Value="#ff0000" />
        </atom:ControlTokenInfoSetter>
    </atom:ThemeConfigProvider.ControlTokenInfoSetters>
    
    <!-- 子控件使用覆盖后的 Token -->
    <StackPanel>
        <atom:Button Content="Custom Theme Button" />
    </StackPanel>
</atom:ThemeConfigProvider>
```

### 7.3 实现原理

`ThemeConfigProvider` 继承 `Control`，实现 `IThemeConfigProvider`：

1. 当挂载到视觉树时（`OnAttachedToVisualTree`），执行 `CalculateTokenResources()`
2. 计算流程与 `Theme.BuildThemeResource()` 完全一致
3. 计算结果写入自身的 `Resources.MergedDictionaries`
4. Avalonia 的资源查找是沿视觉树向上冒泡的，所以子控件会优先使用 `ThemeConfigProvider` 的 Token 值

```
视觉树资源查找顺序：
Control → ThemeConfigProvider.Resources → ... → ThemeManager.Resources
```

### 7.4 TokenSetter 与 ControlTokenSetter

| 类 | XAML 标签 | 作用 |
|---|---|---|
| `TokenSetter` | `<atom:TokenSetter>` | 设置 SharedToken（组件级 Token 中等价于 `IsShared="true"`） |
| `ControlTokenSetter` | `<atom:ControlTokenSetter>` | 设置组件自身的 Token |

在 `ControlTokenInfoSetter` 中，`TokenSetter` 会被放入 `SharedTokens`，`ControlTokenSetter` 会被放入 `Tokens`。

---

## 8. 主题事件系统

ThemeManager 提供丰富的生命周期事件：

| 事件 | 参数类型 | 触发时机 |
|------|---------|---------|
| `ThemeCreated` | `ThemeOperateEventArgs` | 主题实例创建完成 |
| `ThemeAboutToLoad` | `ThemeOperateEventArgs` | 即将加载主题 |
| `ThemeLoaded` | `ThemeOperateEventArgs` | 主题加载完成 |
| `ThemeLoadFailed` | `ThemeOperateEventArgs` | 主题加载失败 |
| `ThemeAboutToUnload` | `ThemeOperateEventArgs` | 即将卸载主题 |
| `ThemeUnloaded` | `ThemeOperateEventArgs` | 主题已卸载 |
| `ThemeAboutToChange` | `ThemeOperateEventArgs` | 即将切换主题 |
| `ThemeChanged` | `ThemeChangedEventArgs` | 主题已切换（包含新/旧主题） |
| `LanguageVariantChanged` | `LanguageVariantChangedEventArgs` | 语言已切换 |
| `Initialized` | `EventArgs` | ThemeManager 初始化完成 |

---

## 9. 异常处理

| 异常类 | 场景 |
|--------|------|
| `ThemeDefinitionParserException` | XML 主题定义文件解析错误 |
| `ThemeLoadException` | 主题加载失败（Token 配置错误等） |
| `ThemeNotFoundException` | 请求的 ThemeVariant 不存在 |
| `ThemeResourceRegisterException` | Token/Provider 重复注册 |
| `LanguageMetaInfoParseException` | 语言元信息解析错误 |

---

## 10. 内置默认主题

AtomUI 内置了一个默认主题定义文件 `DaybreakBlue.xml`：

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Theme Name="Daybreak Blue" IsDefault="true">
    <SharedTokens>
    </SharedTokens>
    <ControlTokens>
    </ControlTokens>
</Theme>
```

这是一个"空"定义——没有任何覆盖，完全使用 `DesignToken` 类中定义的默认值（`ColorPrimary = #1677ff` 等）。"DaybreakBlue"（拂晓蓝）是 Ant Design 的默认品牌色名称。

用户可以通过以下方式创建自定义主题：
1. 创建新的 XML 文件放入主题目录
2. 在 XML 中覆盖需要修改的 Token
3. 通过 `builder.WithDefaultTheme("CustomThemeId")` 指定默认主题

