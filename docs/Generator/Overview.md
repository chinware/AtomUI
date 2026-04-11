# AtomUI 源生成器（Source Generator）设计原理

> 本文档详细介绍 AtomUI 的两个 Roslyn 源生成器的设计原理与内部实现，重点聚焦于 Token 系统和多语言系统的底层代码生成机制。  
> 适用版本：AtomUI v5.0+，基于 Avalonia v11  
> 最后更新：2026-04-11

---

## 文档索引

| 文档 | 内容 |
|---|---|
| [TokenResourceKeyGenerator 设计原理](./TokenResourceKeyGenerator.md) | Token 资源键生成器：扫描 `[GlobalDesignToken]` 和 `[ControlDesignToken]`，自动生成 `SharedTokenKind` 枚举、`XxxTokenKind` 枚举、XAML 标记扩展类、`ControlTokenTypePool` 类型注册池 |
| [LanguageGenerator 设计原理](./LanguageGenerator.md) | 多语言资源生成器：扫描 `[LanguageProvider]`，自动生成 `XxxLangResourceKind` 枚举、XAML 标记扩展类、`LanguageProviderPool` 实例注册池 |

---

## 为什么需要源生成器

AtomUI 的 Token 系统和多语言系统需要在 **C# 定义** 和 **AXAML 消费** 之间建立类型安全的桥接。核心问题是：

1. **Token 属性** 定义在 C# 类中（如 `TagToken.DefaultBg`），但需要在 AXAML 中通过标记扩展引用（如 `{atom:TagTokenResource DefaultBg}`）。
2. **语言文本** 定义在 C# 类中（如 `en_US.Uploading`），但需要在 AXAML 中通过标记扩展引用（如 `{atom:UploadLangResource Uploading}`）。
3. **ThemeManager** 需要在运行时发现所有 Token 类型和 LanguageProvider 实例，以便完成 Token 注册和语言资源加载。

如果手写这些桥接代码，开发者每新增一个 Token 属性或语言字段，都需要手动同步更新枚举、标记扩展类和注册池——极易遗漏且高度重复。

源生成器通过 **编译时自动分析 C# 源码**，一次性生成所有桥接代码，实现 **单一事实来源（Single Source of Truth）**：

```
开发者编写                          源生成器自动生成                    AXAML/运行时消费
─────────────                      ──────────────────                  ────────────────
[ControlDesignToken]               TagTokenKind 枚举                  {atom:TagTokenResource DefaultBg}
class TagToken                ───► TagTokenResourceExtension 类   ──►
  DefaultBg { get; set; }          ControlTokenTypePool 注册          ThemeManager.RegisterControlTokenType()

[LanguageProvider]                 UploadLangResourceKind 枚举        {atom:UploadLangResource Uploading}
class en_US                   ───► UploadLangResourceExtension 类 ──►
  Uploading = "..."                LanguageProviderPool 注册          ThemeManager 加载语言资源
```

---

## 源生成器项目概览

### 项目信息

| 属性 | 值 |
|---|---|
| 项目 | `src/AtomUI.Generator/AtomUI.Generator.csproj` |
| 目标框架 | `netstandard2.0`（Roslyn Source Generator 强制要求） |
| 消费方式 | 作为 Analyzer 引用，不产出运行时 DLL |
| 依赖 | 仅 `Microsoft.CodeAnalysis.CSharp` 和 `Microsoft.CodeAnalysis.Analyzers` |

### 两个生成器

| 生成器类 | 标记注解 | 生成产物 |
|---|---|---|
| `TokenResourceKeyGenerator` | `[GlobalDesignToken]` / `[ControlDesignToken]` | `TokenResourceConst.g.cs` / `ControlTokenTypePool.g.cs` |
| `LanguageGenerator` | `[LanguageProvider]` | `LanguageResourceConst.g.cs` / `LanguageProviderPool.g.cs` |

### 生成文件位置

生成的文件输出到各项目的 `GeneratedFiles/AtomUI.Generator/` 目录下：

```
src/AtomUI.Desktop.Controls/
└── GeneratedFiles/
    └── AtomUI.Generator/
        ├── AtomUI.Generator.TokenResourceKeyGenerator/
        │   ├── TokenResourceConst.g.cs         # Token 枚举 + 标记扩展
        │   └── ControlTokenTypePool.g.cs       # Token 类型注册池
        └── AtomUI.Generator.LanguageGenerator/
            ├── LanguageResourceConst.g.cs       # 语言枚举 + 标记扩展
            └── LanguageProviderPool.g.cs        # 语言提供者注册池
```

> **重要**：这些文件由编译器自动生成，**禁止手动编辑**。项目通过 `<Compile Remove="GeneratedFiles/**">` 排除这些文件的直接编译（由源生成器注入）。

---

## 核心设计模式

两个生成器遵循相同的架构模式：

```
1. 标记注解（Attribute）    →  标识目标类
2. 语法树遍历器（Walker）   →  提取元数据（属性名、字段名、命名空间等）
3. 源码写入器（Writer）     →  通过 Roslyn SyntaxFactory 构建并输出 C# 代码
```

这种 **Attribute → Walker → Writer** 三段式管线是 AtomUI 源生成器的统一架构。详细原理请参阅各生成器的独立文档。

