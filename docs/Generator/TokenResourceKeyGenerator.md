# TokenResourceKeyGenerator 设计原理

> 本文档详细解析 `TokenResourceKeyGenerator` 源生成器的内部实现，它是 AtomUI Design Token 系统能够在 AXAML 中被类型安全引用的底层基础设施。

---

## 1. 解决的核心问题

AtomUI 的 Design Token 系统需要解决一个关键矛盾：

- **Token 定义在 C# 中**：如 `TagToken.DefaultBg`、`TagToken.TagFontSize` 等属性定义在 `AbstractControlDesignToken` 子类中。
- **Token 消费在 AXAML 中**：Theme 文件需要通过 `{atom:TagTokenResource DefaultBg}` 引用 Token 值。

Avalonia 的 `DynamicResourceExtension` 需要一个资源键（Resource Key）来查找资源字典中的值。AtomUI 使用 **枚举值** 作为资源键，这既提供了编译时类型安全，又与 Avalonia 的资源系统无缝集成。

`TokenResourceKeyGenerator` 的职责就是 **自动从 Token 类的属性定义中生成这些枚举和对应的 XAML 标记扩展类**。

---

## 2. 输入与输出

### 2.1 输入：标记注解

生成器识别两种标记注解：

**`[GlobalDesignToken]`** — 标记全局 Design Token 的 partial class 片段

```csharp
// src/AtomUI.Core/Theme/TokenSystem/TokenDefinitions/DesignToken.Seed.cs
[GlobalDesignToken]
public partial class DesignToken : AbstractDesignToken
{
    public Color ColorPrimary { get; set; }
    public double FontSize { get; set; }
    public double BorderRadius { get; set; }
    // ...
}
```

全局 DesignToken 使用 `partial class` 拆分到多个文件（`.Seed.cs`、`.ColorPrimaryMap.cs`、`.Alias.cs` 等），每个片段都标注 `[GlobalDesignToken]`。生成器会收集所有片段中的属性，合并后生成一个统一的 `SharedTokenKind` 枚举。

**`[ControlDesignToken]`** — 标记组件 Token 类

```csharp
// src/AtomUI.Desktop.Controls/Tag/TagToken.cs
[ControlDesignToken]
internal class TagToken : AbstractControlDesignToken
{
    public Color DefaultBg { get; set; }
    public Color DefaultColor { get; set; }
    public double TagFontSize { get; set; }
    // ...
}
```

### 2.2 排除机制

标注 `[NotTokenDefinition]` 的属性会被跳过：

```csharp
[NotTokenDefinition] 
public IDictionary<PresetPrimaryColor, ColorMap> ColorPalettes { get; set; }
```

此外，只有同时具备 `getter` 和 `setter` 的属性才会被收集（只读计算属性不生成资源键）。

### 2.3 输出产物

**产物 1：`TokenResourceConst.g.cs`**

包含两类生成代码：

（1）**全局 `SharedTokenKind` 枚举**（命名空间 `AtomUI.Theme.Styling`）：

```csharp
namespace AtomUI.Theme.Styling
{
    public enum SharedTokenKind
    {
        BorderRadius,
        BorderRadiusLG,
        ColorBgContainer,
        ColorPrimary,
        FontSize,
        // ... 所有 DesignToken 属性（约 250+ 个）
    }
}
```

（2）**每个控件的 `XxxTokenKind` 枚举 + `XxxTokenResourceExtension` 标记扩展类**（命名空间 `{控件命名空间}.DesignTokens`）：

```csharp
namespace AtomUI.Desktop.Controls.DesignTokens
{
    // 枚举：列出 TagToken 的所有属性名
    public enum TagTokenKind
    {
        DefaultBg,
        DefaultColor,
        TagBorderlessBg,
        TagCloseIconSize,
        TagFontSize,
        TagIconSize,
        TagLineHeight,
        TagPadding,
        TagTextPaddingInline
    }

    // XAML 标记扩展：在 AXAML 中作为 {atom:TagTokenResource DefaultBg} 使用
    public class TagTokenResourceExtension : TokenResourceExtension<TagTokenKind>
    {
        public TagTokenResourceExtension() { }
        public TagTokenResourceExtension(TagTokenKind kind) : base(kind) { }
    }
}
```

**产物 2：`ControlTokenTypePool.g.cs`**

一个静态方法，返回当前程序集中所有 `[ControlDesignToken]` 类的 `Type` 列表：

```csharp
namespace AtomUI.Theme
{
    internal class ControlTokenTypePool
    {
        internal static IList<Type> GetTokenTypes()
        {
            List<Type> tokenTypes = new List<Type>();
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.TagToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ButtonToken));
            // ... 所有控件 Token 类型
            return tokenTypes;
        }
    }
}
```

---

## 3. 内部管线详解

### 3.1 管线概览

```
TokenResourceKeyGenerator.Initialize()
│
├─ globalTokensProvider ─────────────────────────────────────────┐
│  ForAttributeWithMetadataName("[GlobalDesignToken]")           │
│  → TokenPropertyWalker 遍历                                    │
│  → 收集属性名 + TokenResourceCatalog                            │
│  → Collect + Merge 去重                                        │
│                                                                │
├─ controlTokensProvider ────────────────────────────────────────┤
│  ForAttributeWithMetadataName("[ControlDesignToken]")          │
│  → ControlTokenPropertyWalker 遍历                              │
│  → 收集属性名 + 控件类名/命名空间 + 基类属性                     │
│  → Collect                                                     │
│                                                                │
└─ tokensProvider = globalTokensProvider.Combine(controlTokens) ─┘
   │
   RegisterImplementationSourceOutput
   │
   ├─ ResourceKeyClassWriter.Write()
   │  → 生成 TokenResourceConst.g.cs
   │     ├─ SharedTokenKind 枚举
   │     ├─ XxxTokenKind 枚举（每个控件一个）
   │     └─ XxxTokenResourceExtension 类（每个控件一个）
   │
   └─ ControlTokenTypePoolClassWriter.Write()
      → 生成 ControlTokenTypePool.g.cs
         └─ GetTokenTypes() 方法
```

### 3.2 TokenPropertyWalker（全局 Token 遍历器）

**文件**：`src/AtomUI.Generator/DesignToken/TokenPropertyWalker.cs`

继承 `CSharpSyntaxWalker`，遍历标注了 `[GlobalDesignToken]` 的类的语法树：

- `VisitPropertyDeclaration`：收集每个属性名（跳过标注了 `[NotTokenDefinition]` 的属性）。
- `VisitClassDeclaration`：从类的 `[GlobalDesignToken]` 注解的构造函数参数中提取 `TokenResourceCatalog`（资源目录分类标识）。

由于 `DesignToken` 是 `partial class`，分布在 14 个文件中，每个文件都有 `[GlobalDesignToken]` 注解。生成器通过 `Collect().Select()` 将所有片段的属性合并去重到一个 `HashSet<TokenName>` 中。

### 3.3 ControlTokenPropertyWalker（控件 Token 遍历器）

**文件**：`src/AtomUI.Generator/DesignToken/ControlTokenPropertyWalker.cs`

比 `TokenPropertyWalker` 更复杂，因为需要处理 **继承链**：

1. **当前类属性收集**：遍历 `VisitPropertyDeclaration`，筛选同时具备 getter 和 setter、且未标注 `[NotTokenDefinition]` 的属性。
2. **基类属性收集**：通过 `AddBaseClassProperties()` 方法，沿着继承链向上遍历（直到 `AbstractControlDesignToken`），收集所有基类中的非私有、非静态、同时有 getter/setter 的属性。

这一设计确保：当 `ButtonToken` 继承自 `BaseButtonToken` 时，基类中定义的 Token 属性也会出现在生成的 `ButtonTokenKind` 枚举中。

### 3.4 ResourceKeyClassWriter（资源键类写入器）

**文件**：`src/AtomUI.Generator/DesignToken/ResourceKeyClassWriter.cs`

使用 Roslyn `SyntaxFactory` API 构建完整的编译单元（CompilationUnit），包含：

1. **SharedTokenKind 枚举**：所有全局 Token 属性名按字母排序后生成枚举成员，放入 `AtomUI.Theme.Styling` 命名空间。

2. **XxxTokenKind 枚举**：每个控件 Token 类生成一个独立枚举，放入 `{控件命名空间}.DesignTokens` 命名空间（如 `AtomUI.Desktop.Controls.DesignTokens`）。

3. **XxxTokenResourceExtension 类**：继承 `TokenResourceExtension<XxxTokenKind>`，提供无参和带参两个构造函数。这个类就是 AXAML 中 `{atom:XxxTokenResource ...}` 标记扩展的实现。

### 3.5 ControlTokenTypePoolClassWriter（类型注册池写入器）

**文件**：`src/AtomUI.Generator/DesignToken/ControlTokenTypePoolClassWriter.cs`

生成一个 `internal class ControlTokenTypePool`，内含 `GetTokenTypes()` 静态方法。该方法返回一个 `IList<Type>`，包含当前程序集中所有 `[ControlDesignToken]` 类的 `typeof()`。

ThemeManager 在启动时调用此方法，自动发现并注册所有控件 Token 类型，无需开发者手动注册。

---

## 4. Token 资源从定义到消费的完整链路

以 `TagToken.DefaultBg` 为例，完整追踪从 C# 定义到 AXAML 消费的全过程：

### 4.1 定义阶段

```csharp
// 开发者编写
[ControlDesignToken]
internal class TagToken : AbstractControlDesignToken
{
    public Color DefaultBg { get; set; }
    // ...
    public override void CalculateTokenValues(bool isDarkMode)
    {
        DefaultBg = ColorUtils.OnBackground(SharedToken.ColorFillQuaternary, SharedToken.ColorBgContainer);
    }
    protected override Type GetTokenKindType() => typeof(TagTokenKind);
}
```

### 4.2 生成阶段（编译时）

源生成器自动产出：

```csharp
// 自动生成 — TokenResourceConst.g.cs
namespace AtomUI.Desktop.Controls.DesignTokens
{
    public enum TagTokenKind { DefaultBg, DefaultColor, /* ... */ }
    
    public class TagTokenResourceExtension : TokenResourceExtension<TagTokenKind>
    {
        public TagTokenResourceExtension() { }
        public TagTokenResourceExtension(TagTokenKind kind) : base(kind) { }
    }
}
```

### 4.3 注册阶段（运行时启动）

ThemeManager 通过 `ControlTokenTypePool.GetTokenTypes()` 发现 `TagToken` 类型，并调用其 `CalculateTokenValues()` 方法计算出具体的 Token 值，然后将 `TagTokenKind.DefaultBg → Color 值` 写入 Avalonia 的 `ResourceDictionary`。

### 4.4 消费阶段（AXAML 渲染）

```xml
<!-- TagTheme.axaml -->
<Setter Property="Background" Value="{atom:TagTokenResource DefaultBg}" />
```

AXAML 解析器将 `{atom:TagTokenResource DefaultBg}` 解析为 `TagTokenResourceExtension(TagTokenKind.DefaultBg)`，它继承自 `DynamicResourceExtension`，以 `TagTokenKind.DefaultBg` 作为资源键从 ResourceDictionary 中动态查找对应的 Color 值。

---

## 5. 关键数据结构

### 5.1 TokenName

```csharp
internal record TokenName
{
    public string Name { get; }            // 属性名（如 "DefaultBg"）
    public string ResourceCatalog { get; } // 资源目录（如 "Tag"）
}
```

### 5.2 ControlTokenInfo

```csharp
internal class ControlTokenInfo
{
    public string ControlNamespace { get; set; }  // 如 "AtomUI.Desktop.Controls"
    public string ControlName { get; set; }       // 如 "TagToken"
    public HashSet<TokenName> Tokens { get; }     // 该控件的所有 Token 属性名
}
```

### 5.3 TokenInfo

```csharp
internal class TokenInfo
{
    public HashSet<TokenName> Tokens { get; }                  // 全局 Token 属性名集合
    public List<ControlTokenInfo> ControlTokenInfos { get; }   // 所有控件 Token 信息
}
```

---

## 6. 命名空间映射规则

生成器使用特定的命名空间映射规则：

| 输入 | 输出命名空间 |
|---|---|
| 全局 DesignToken | `AtomUI.Theme.Styling` |
| 控件 Token（命名空间 `X`） | `X.DesignTokens` |

例如：

| Token 类 | 命名空间 | 生成的枚举/扩展命名空间 |
|---|---|---|
| `DesignToken` | `AtomUI.Theme.TokenSystem` | `AtomUI.Theme.Styling` |
| `TagToken` | `AtomUI.Desktop.Controls` | `AtomUI.Desktop.Controls.DesignTokens` |
| `IndicatorScrollViewerToken` | `AtomUI.Desktop.Controls.Primitives` | `AtomUI.Desktop.Controls.Primitives.DesignTokens` |

---

## 7. 与 TokenResourceExtension 运行时基类的关系

生成的 `XxxTokenResourceExtension` 继承自 `TokenResourceExtension<TTokenKind>`，其定义在 `AtomUI.Core`：

```csharp
// src/AtomUI.Core/Theme/TokenResourceExtension.cs
public abstract class TokenResourceExtension<TTokenKind> : DynamicResourceExtension
    where TTokenKind : Enum
{
    public TTokenKind? Kind
    {
        get => (TTokenKind?)ResourceKey; 
        set => ResourceKey = value; 
    }
}
```

核心机制：`Kind` 属性的 setter 将枚举值赋给 `DynamicResourceExtension.ResourceKey`。Avalonia 的 `DynamicResourceExtension` 使用 `ResourceKey` 在 `ResourceDictionary` 中查找资源。因此枚举值同时作为：
- **编译时的类型安全键**（AXAML 编译器验证枚举成员存在）
- **运行时的资源查找键**（ResourceDictionary 的 key）

---

## 8. GetTokenKindType() 的闭环设计

每个控件 Token 类必须实现 `GetTokenKindType()` 方法：

```csharp
protected override Type GetTokenKindType() => typeof(TagTokenKind);
```

这个方法将 Token 类和生成的枚举类型关联起来。运行时 ThemeManager 通过此方法获取枚举类型，遍历其所有成员，将每个枚举值作为 ResourceKey 注册到 ResourceDictionary 中。这形成了一个完整的闭环：

```
TagToken.DefaultBg (C# 属性)
    ↓ 源生成器
TagTokenKind.DefaultBg (枚举值)
    ↓ GetTokenKindType() + 反射
ResourceDictionary[TagTokenKind.DefaultBg] = Color 值
    ↓ DynamicResourceExtension
{atom:TagTokenResource DefaultBg} (AXAML 消费)
```

