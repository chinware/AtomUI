# AtomUI 主题系统 — Token 资源绑定与 Source Generator

> 本文档详细描述 AtomUI 中 Design Token 如何从 C# 对象转化为 Avalonia 资源，控件如何在 XAML 和代码中消费 Token，以及 Roslyn Source Generator 如何保障类型安全。

---

## 1. Token 到 Avalonia 资源的桥接

### 1.1 资源构建过程

`DesignToken` 和 `AbstractControlDesignToken` 都通过 `BuildResourceDictionary()` 方法将 Token 属性写入 Avalonia 的 `ResourceDictionary`：

```csharp
// AbstractDesignToken.BuildResourceDictionary()
public virtual void BuildResourceDictionary(IResourceDictionary dictionary)
{
    foreach (var value in Enum.GetValues<SharedTokenKind>())
    {
        var tokenName = Enum.GetName(value);
        var property = tokenProperties[tokenName];
        var tokenValue = property.GetValue(this);
        
        // Color 类型自动转为 ImmutableSolidColorBrush
        if (property.PropertyType == typeof(Color) && tokenValue is not null)
        {
            tokenValue = new ImmutableSolidColorBrush((Color)tokenValue);
        }
        
        // 键是 Source Generator 生成的枚举值
        dictionary[value] = tokenValue;
    }
}
```

**关键点：**
- **键**是 Source Generator 生成的枚举值（`SharedTokenKind` 或 `{Control}TokenKind`）
- **Color 类型自动包装**为 `ImmutableSolidColorBrush`（XAML 中使用更方便）
- **反射驱动**：通过属性名与枚举名匹配实现映射

### 1.2 资源字典层次

```
ThemeManager.Resources
├── ThemeDictionaries
│   └── [ThemeVariant] = ResourceDictionary
│       ├── SharedTokenKind.ColorPrimary → ImmutableSolidColorBrush(#1677ff)
│       ├── SharedTokenKind.FontSize → 14.0
│       ├── SharedTokenKind.BorderRadius → CornerRadius(6)
│       ├── ButtonTokenKind.PrimaryColor → ImmutableSolidColorBrush(#fff)
│       ├── ButtonTokenKind.Padding → Thickness(15, 0)
│       ├── TagTokenKind.DefaultBg → ImmutableSolidColorBrush(...)
│       └── ... (所有 Token)
│
└── MergedDictionaries
    ├── ButtonThemes.axaml    (ControlTheme 引用上面的 Token)
    ├── TagThemes.axaml
    ├── LanguageResource      (当前语言)
    └── ...
```

---

## 2. XAML MarkupExtension

### 2.1 SharedTokenResource（全局 Token）

用于在 XAML 中引用全局 Token（Seed/Map/Alias）：

```xml
<!-- XAML 中使用全局 Token -->
<Border Background="{atom:SharedTokenResource ColorPrimary}"
        CornerRadius="{atom:SharedTokenResource BorderRadius}"
        BorderThickness="{atom:SharedTokenResource BorderThickness}" />
```

实现：

```csharp
// 基类
public abstract class TokenResourceExtension<TTokenKind> : DynamicResourceExtension
    where TTokenKind : Enum
{
    public TTokenKind? Kind
    {
        get => (TTokenKind?)ResourceKey;
        set => ResourceKey = value;
    }
}

// 全局 Token 扩展
public class SharedTokenResourceExtension : TokenResourceExtension<SharedTokenKind>
{
    public SharedTokenResourceExtension() { }
    public SharedTokenResourceExtension(SharedTokenKind kind) : base(kind) { }
}
```

**工作原理：**
- `SharedTokenResourceExtension` 继承自 `DynamicResourceExtension`
- 将 `SharedTokenKind` 枚举值作为 `ResourceKey`
- Avalonia 在运行时从 `ThemeDictionaries[currentVariant]` 查找该枚举键对应的值
- 因为使用 `DynamicResource`，主题切换时值会自动更新

### 2.2 TokenResource（组件 Token）

每个组件都有自己的 `TokenResource` 扩展（由 Source Generator 生成）：

```xml
<!-- 在 ButtonTheme.axaml 中使用 Button 的 Token -->
<ContentPresenter Foreground="{atom:TokenResource ButtonPrimaryColor}"
                  Padding="{atom:TokenResource ButtonPadding}" />
```

### 2.3 MarkupExtension 类型解析

XAML 中 `{atom:SharedTokenResource ColorPrimary}` 的解析过程：

```
1. XAML 解析器识别 `atom:SharedTokenResource` 为 MarkupExtension
2. 参数 `ColorPrimary` 被解析为 `SharedTokenKind.ColorPrimary` 枚举值
3. 内部调用 DynamicResourceExtension.ProvideValue()
4. 返回一个 DynamicResourceBinding，ResourceKey = SharedTokenKind.ColorPrimary
5. 运行时 Avalonia 沿资源树查找此键
6. 在 ThemeDictionaries[currentVariant] 中找到对应值
7. 主题切换时自动重新查找
```

---

## 3. 代码中的 Token 消费

### 3.1 StyleExtensions

在 C# 代码构建主题时（`BaseControlTheme.BuildStyles()`），使用扩展方法绑定 Token：

```csharp
// 方式一：绑定 Token 枚举值（DynamicResource）
style.Add(TextBlock.ForegroundProperty, SharedTokenKind.ColorText);
style.Add(Button.PaddingProperty, ButtonTokenKind.Padding);

// 方式二：直接设置值
style.Add(TextBlock.FontSizeProperty, 14.0);

// 方式三：设置值工厂
style.Add<Icon>(Button.IconProperty, () => new LoadingOutlined());
```

```csharp
// StyleExtensions 实现
public static class StyleExtensions
{
    // 绑定 Token 枚举值作为 DynamicResource
    public static StyleBase Add<TTokenKind>(
        this StyleBase style, 
        AvaloniaProperty targetProperty, 
        TTokenKind resourceKey) where TTokenKind : Enum
    {
        style.Add(new Setter(targetProperty, 
            new DynamicResourceExtension(resourceKey)));
        return style;
    }
}
```

### 3.2 TokenResourceBinder

在控件的代码逻辑中动态绑定 Token：

```csharp
// 在控件中动态绑定 Token
TokenResourceBinder.CreateTokenBinding(this, BackgroundProperty, SharedTokenKind.ColorBgContainer);
```

---

## 4. 组件级 Token 作用域

### 4.1 问题场景

当组件有自定义的 SharedToken 覆盖时（例如某个 Button 的 `ColorPrimary` 被改为红色），这个组件内部的子元素也需要使用修改后的 `ColorPrimary`。

但 Avalonia 的 `ThemeDictionaries` 是全局的，所有控件共享同一份 `ResourceDictionary`。如何实现组件级别的资源覆盖？

### 4.2 解决方案：ControlTokenResourceScopeHost

AtomUI 通过在控件级别注入差值 `ResourceDictionary` 实现组件级 Token 隔离：

```
资源查找顺序（由近到远）：
Control.Resources.MergedDictionaries
    └── [差值 ResourceDictionary]   ← 组件级 SharedToken 覆盖
            │
            │ 仅包含与全局 SharedToken 不同的值
            │ 例如: SharedTokenKind.ColorPrimary → red
            │
            ▼ (找不到时继续向上)
Parent.Resources → ... → ThemeManager.Resources.ThemeDictionaries
```

### 4.3 注册流程

```csharp
// 控件构造函数中注册 Token 作用域
public class Tag : AbstractTag
{
    public Tag()
    {
        this.RegisterTokenResourceScope(TagToken.ScopeProvider);
    }
}
```

```csharp
// RegisterTokenResourceScope 的实现
public static void RegisterTokenResourceScope(
    this Control host, IControlTokenResourceScopeProvider scopeProvider)
{
    host.AttachedToLogicalTree += HandleAttachedToLogicalTree;
    host.DetachedFromLogicalTree += HandleDetachedToLogicalTree;
    ControlTokenResourceScopeHost.SetTokenResourceScopeProvider(host, scopeProvider);
}
```

### 4.4 Attach 时的处理

```csharp
private static void HandleAttachedToLogicalTree(object? sender, ...)
{
    // 1. 通过 TokenFinderUtils 找到此控件对应的 ControlToken
    var controlToken = TokenFinderUtils.FindControlToken(control, 
        scopeProvider.Id, scopeProvider.ResourceCatalog);
    
    // 2. 获取差值字典（组件 SharedToken 与全局 SharedToken 的差异）
    var delta = controlToken.GetSharedResourceDeltaDictionary();
    
    // 3. 如果有差异，注入到控件的资源中
    if (delta.Count > 0)
    {
        var resourceDictionary = new ResourceDictionary();
        foreach (var entry in delta)
        {
            if (entry.Value is Color color)
                resourceDictionary[entry.Key] = new SolidColorBrush(color);
            else
                resourceDictionary[entry.Key] = entry.Value;
        }
        control.Resources.MergedDictionaries.Add(resourceDictionary);
    }
}
```

### 4.5 IControlTokenResourceScopeProvider

```csharp
public interface IControlTokenResourceScopeProvider
{
    string Id { get; }                    // Token ID，如 "Button"
    string? ResourceCatalog => null;      // 资源目录（可选）
}

public class ControlTokenResourceScopeProvider : IControlTokenResourceScopeProvider
{
    public string Id { get; }
    public string? ResourceCatalog { get; }
    
    public ControlTokenResourceScopeProvider(string id, string? resourceCatalog = null)
    {
        Id = id;
        ResourceCatalog = resourceCatalog;
    }
}
```

---

## 5. Source Generator（TokenResourceKeyGenerator）

### 5.1 概述

`AtomUI.Generator` 项目中的 `TokenResourceKeyGenerator` 是一个 Roslyn Incremental Source Generator，它在编译期扫描所有 Token 定义，自动生成类型安全的资源键枚举。

### 5.2 扫描标记

Generator 识别两种 Attribute：

| Attribute | 扫描目标 | 生成物 |
|-----------|---------|--------|
| `[GlobalDesignToken]` | `DesignToken` partial class 的 Token 属性 | `SharedTokenKind` 枚举 |
| `[ControlDesignToken]` | 组件 Token 类的所有属性 | `{Control}TokenKind` 枚举 |

### 5.3 生成流程

```
编译期
│
├── 1. ForAttributeWithMetadataName("GlobalDesignToken")
│      ├── TokenPropertyWalker 遍历所有 [DesignTokenKind] 属性
│      └── 收集属性名 → Set<TokenName>
│
├── 2. ForAttributeWithMetadataName("ControlDesignToken")
│      ├── ControlTokenPropertyWalker 遍历所有属性
│      └── 收集控件名 + 属性名 → List<ControlTokenInfo>
│
├── 3. ResourceKeyClassWriter.Write()
│      ├── 生成 SharedTokenKind 枚举
│      │   enum SharedTokenKind {
│      │       ColorPrimary,
│      │       ColorSuccess,
│      │       FontSize,
│      │       BorderRadius,
│      │       ...
│      │   }
│      │
│      ├── 生成每个组件的 TokenKind 枚举
│      │   enum ButtonTokenKind {
│      │       PrimaryColor,
│      │       Padding,
│      │       ...
│      │   }
│      │
│      └── 生成对应的 TokenResourceExtension
│          class TokenResourceExtension : TokenResourceExtension<ButtonTokenKind> { }
│
└── 4. ControlTokenTypePoolClassWriter.Write()
       生成 ControlToken 类型注册池
```

### 5.4 生成物示例

**SharedTokenKind（全局 Token 枚举）：**

```csharp
// 自动生成 — 不要手动编辑
namespace AtomUI.Theme.Styling;

public enum SharedTokenKind
{
    // Seed Tokens
    ColorPrimary,
    ColorSuccess,
    ColorWarning,
    ColorError,
    ColorInfo,
    FontSize,
    LineWidth,
    BorderRadius,
    ControlHeight,
    EnableMotion,
    EnableWaveSpirit,
    // ...
    
    // Map Tokens
    ColorPrimaryBg,
    ColorPrimaryBgHover,
    ColorPrimaryHover,
    FontSizeSM,
    FontSizeLG,
    SizeSM,
    SizeLG,
    ControlHeightSM,
    // ...
    
    // Alias Tokens
    ColorTextDisabled,
    ColorBgContainer,
    PaddingContentHorizontal,
    ControlItemBgHover,
    BoxShadows,
    // ...
}
```

**组件 Token 枚举：**

```csharp
// 自动生成
namespace AtomUI.Desktop.Controls;

public enum ButtonTokenKind
{
    PrimaryColor,
    DefaultColor,
    DefaultBg,
    DefaultBorderColor,
    DangerColor,
    Padding,
    PaddingSM,
    PaddingLG,
    // ...
}
```

### 5.5 类型安全保障

通过 Source Generator，Token 的使用在编译期就能得到类型检查：

```csharp
// ✅ 编译通过 — SharedTokenKind.ColorPrimary 是有效枚举值
style.Add(Border.BackgroundProperty, SharedTokenKind.ColorPrimary);

// ❌ 编译错误 — SharedTokenKind 中不存在此值
style.Add(Border.BackgroundProperty, SharedTokenKind.NonExistent);

// ✅ XAML 中也有智能提示
// {atom:SharedTokenResource ColorPrimary}
```

---

## 6. BaseControlTheme

### 6.1 概述

`BaseControlTheme` 是所有控件主题的基类，继承自 Avalonia 的 `ControlTheme`。它定义了控件主题的构建流程：

```csharp
public abstract class BaseControlTheme : ControlTheme
{
    public void Build()
    {
        NotifyPreBuild();           // 预构建钩子
        BuildThemeAnimations();     // 构建动画
        BuildStyles();              // 构建样式（绑定 Token）
        BuildTemplateStyle();       // 构建控件模板
        NotifyBuildCompleted();     // 构建完成钩子
    }
    
    protected virtual IControlTemplate? BuildControlTemplate() => null;
    protected virtual void BuildStyles() { }
    protected virtual void BuildThemeAnimations() { }
}
```

### 6.2 TemplateParentBinding 辅助方法

`BaseControlTheme` 提供了丰富的模板绑定辅助方法，简化控件模板内部元素与模板父级的属性绑定：

```csharp
// 绑定模板内元素到 TemplatedParent 的属性
CreateTemplateParentBinding(target, TextBlock.TextProperty, Button.ContentProperty);

// 带优先级的绑定
CreateTemplateParentBinding(target, Border.BackgroundProperty, 
    Button.BackgroundProperty, BindingPriority.Template);

// 带转换器的绑定
CreateTemplateParentBinding(target, TextBlock.IsVisibleProperty, 
    Button.ContentProperty, converter: ObjectConverters.IsNotNull);
```

---

## 7. 控件主题注册

### 7.1 IControlThemesProvider

每个控件包（如 `AtomUI.Desktop.Controls`）提供一个 `IControlThemesProvider` 实现：

```csharp
public interface IControlThemesProvider
{
    string Id { get; }
    IReadOnlyList<IResourceProvider> ControlThemes { get; }
}
```

Provider 会收集所有标注了 `[ControlThemeProvider]` 的主题类，构建并注册。

### 7.2 注册链路

```
UseDesktopControls() [扩展方法]
    │
    ├── builder.AddControlThemesProvider(new DesktopControlThemesProvider())
    │       ├── 收集所有 [ControlThemeProvider] 类
    │       ├── 实例化 BaseControlTheme
    │       └── 调用 Build() 构建主题
    │
    ├── builder.AddControlToken(typeof(ButtonToken))
    │       ... 注册所有组件 Token 类型
    │
    └── builder.AddLanguageProviders(...)
            ... 注册语言资源
```

---

## 8. 完整的 Token 消费链路图

```
┌─────────────────────────────────────────────────────────┐
│                  编译期 (Source Generator)                │
│                                                         │
│  [GlobalDesignToken] DesignToken                        │
│  [ControlDesignToken] ButtonToken                       │
│          │                                              │
│          ▼                                              │
│  SharedTokenKind enum    ButtonTokenKind enum            │
│  SharedTokenResourceExtension                            │
│  ButtonTokenResourceExtension                            │
└────────────────────────┬────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────┐
│                  运行期 (Theme Loading)                   │
│                                                         │
│  DesignToken 实例 ──CalculateTokenValues──▶ 值就绪       │
│  ButtonToken 实例 ──CalculateTokenValues──▶ 值就绪       │
│          │                                              │
│          ▼                                              │
│  BuildResourceDictionary()                               │
│  ResourceDictionary[SharedTokenKind.ColorPrimary]        │
│    = ImmutableSolidColorBrush(#1677ff)                  │
│  ResourceDictionary[ButtonTokenKind.PrimaryColor]        │
│    = ImmutableSolidColorBrush(#ffffff)                  │
│          │                                              │
│          ▼                                              │
│  ThemeManager.Resources.ThemeDictionaries[variant]       │
│    = ResourceDictionary                                  │
└────────────────────────┬────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────┐
│                  XAML 消费层                               │
│                                                         │
│  <!-- 全局 Token -->                                     │
│  Background="{atom:SharedTokenResource ColorPrimary}"    │
│                                                         │
│  <!-- 组件 Token -->                                     │
│  Foreground="{atom:TokenResource ButtonPrimaryColor}"    │
│                                                         │
│  Avalonia 解析 DynamicResource:                          │
│  1. 获取当前 ThemeVariant                                │
│  2. 在 ThemeDictionaries[variant] 中查找键               │
│  3. 返回值 (ImmutableSolidColorBrush)                    │
│  4. 主题切换时自动重新查找                                │
│                                                         │
│  <!-- C# Style 构建 -->                                  │
│  style.Add(ForegroundProperty, SharedTokenKind.ColorText)│
│  // 等价于 DynamicResourceExtension(SharedTokenKind.X)   │
└─────────────────────────────────────────────────────────┘
```

---

## 9. 与 Ant Design CSS-in-JS 的对比

| 维度 | Ant Design (CSS-in-JS) | AtomUI (Avalonia Resource) |
|------|----------------------|---------------------------|
| Token 存储 | JavaScript 对象 / CSS Variables | ResourceDictionary（ThemeDictionaries） |
| Token 消费 | `token.colorPrimary` 在 JS 中直接访问 | `{atom:SharedTokenResource ColorPrimary}` XAML 标记 |
| 组件级隔离 | CSS-in-JS Scope（hash class name） | ControlTokenResourceScopeHost（注入差值 ResourceDictionary） |
| 类型安全 | TypeScript interface | Roslyn Source Generator 枚举 |
| 运行时切换 | React Context re-render | Avalonia ThemeVariant 切换 ResourceDictionary |
| 嵌套主题 | `<ConfigProvider theme={...}>` | `<atom:ThemeConfigProvider>` |
| 颜色类型 | CSS String (`#1677ff`) | `ImmutableSolidColorBrush` (自动从 Color 转换) |

