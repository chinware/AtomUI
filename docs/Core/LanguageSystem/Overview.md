# AtomUI 多语言系统概览

> 本文档描述 AtomUI 国际化（i18n）系统的整体架构，包括设计目标、核心概念、系统分层以及源文件清单。

---

## 1. 设计目标

AtomUI 多语言系统遵循以下设计原则：

1. **所有用户可见文本必须通过多语言系统管理** — 禁止在控件中硬编码字符串。
2. **运行时热切换** — 用户可在运行时切换语言，UI 即时响应刷新，无需重启应用。
3. **Source Generator 自动化** — 通过 Roslyn Source Generator 自动生成资源枚举键和 XAML 标记扩展，消除手动维护的负担。
4. **与 Avalonia 资源系统深度集成** — 多语言文本以 `ResourceDictionary` 形式注入 Avalonia 资源树，利用 `DynamicResource` 机制实现动态绑定。
5. **按组件分组管理** — 每个控件的语言资源独立定义，通过 `LangId` 分组，避免全局命名冲突。
6. **可扩展** — 第三方开发者可使用相同机制为自定义控件添加多语言支持。

---

## 2. 系统架构总览

```
┌──────────────────────────────────────────────────────────────────────┐
│  构建时 (Build Time) — Roslyn Source Generator                       │
│                                                                      │
│  LanguageGenerator (IIncrementalGenerator)                           │
│    ├── LanguageProviderWalker   →  扫描 [LanguageProvider] 类        │
│    ├── LangResourceKeyClassSourceWriter                              │
│    │     → 生成 {LangId}LangResourceKind 枚举                        │
│    │     → 生成 {LangId}LangResourceExtension 标记扩展               │
│    └── LanguageProviderPoolClassSourceWriter                         │
│          → 生成 LanguageProviderPool.GetLanguageProviders()          │
└──────────────────────────────┬───────────────────────────────────────┘
                               │
                               ▼
┌──────────────────────────────────────────────────────────────────────┐
│  应用初始化 (App Init)                                                │
│                                                                      │
│  Application.UseAtomUI(builder => {                                  │
│      builder.WithDefaultLanguageVariant(LanguageVariant.zh_CN);      │
│      builder.UseDesktopControls(); // 注册 LanguageProviderPool      │
│  })                                                                  │
│    │                                                                 │
│    ├── ThemeManagerBuilder 收集所有 LanguageProvider                  │
│    ├── ThemeManagerBuilder.Build() → 创建 ThemeManager               │
│    ├── ThemeManager.Configure() → BuildLanguageResources()           │
│    │     └── 按 LanguageVariant 分组，构建 ResourceDictionary        │
│    └── 设置初始 LanguageVariant → 对应语言 ResourceDictionary        │
│         注入 MergedDictionaries                                      │
└──────────────────────────────┬───────────────────────────────────────┘
                               │
                               ▼
┌──────────────────────────────────────────────────────────────────────┐
│  运行时 (Runtime)                                                     │
│                                                                      │
│  AXAML 主题中使用 {atom:XxxLangResource Key} 绑定语言文本             │
│                                                                      │
│  运行时切换语言：                                                     │
│    application.SetLanguageVariant(LanguageVariant.en_US)              │
│    │                                                                 │
│    └── ThemeManager.LanguageVariant 属性变更                          │
│         ├── 移除旧语言 ResourceDictionary                             │
│         ├── 注入新语言 ResourceDictionary                             │
│         └── 触发 LanguageVariantChanged 事件                          │
│              → DynamicResource 自动刷新所有绑定控件                    │
└──────────────────────────────────────────────────────────────────────┘
```

---

## 3. 核心概念

| 概念 | 说明 |
|------|------|
| **LanguageCode** | 语言代码枚举，如 `zh_CN`、`en_US` |
| **LanguageVariant** | 语言变体的类型安全封装，支持与 `CultureInfo` 互转 |
| **LanguageProvider** | 单个语言+单个组件的文本定义类，包含所有 `public const string` 字段 |
| **LangId** | 语言资源分组标识（通常与控件 Token ID 一致），同一 `LangId` 下不同语言共享同一组资源键 |
| **LangResourceKind** | Source Generator 自动生成的资源键枚举，作为 `ResourceDictionary` 的键 |
| **LangResourceExtension** | Source Generator 自动生成的 XAML 标记扩展，在 AXAML 中绑定语言文本 |
| **LanguageProviderPool** | Source Generator 自动生成的工厂类，列举当前程序集中所有 `LanguageProvider` |
| **CommonLangId** | 跨控件共享的通用语言分组，如 "确定"、"取消"、"提交" 等 |

---

## 4. 源文件清单

### 4.1 核心基础设施 (`src/AtomUI.Core/Language/`)

| 文件 | 职责 |
|------|------|
| `LanguageCode.cs` | `LanguageCode` 枚举定义（`zh_CN`、`en_US`），含 `ToHyphenString()` 扩展方法 |
| `LanguageVariant.cs` | `LanguageVariant` record — 语言变体的类型安全封装，含 `StyledProperty`、`CultureInfo` 互转 |
| `LanguageVariantTypeConverter.cs` | XAML 字符串到 `LanguageVariant` 的类型转换器 |
| `LanguageVariantChangedEventArgs.cs` | 语言切换事件参数 |
| `ILanguageProvider.cs` | 语言提供者接口 — `LangCode`、`LangId`、`BuildResourceDictionary()` |
| `LanguageProvider.cs` | 语言提供者抽象基类 — 通过反射将 `const string` 字段写入 `ResourceDictionary` |
| `LanguageProviderAttribute.cs` | `[LanguageProvider]` 标注 — 指定语言代码和分组 ID |
| `LanguageSgMetaInfoAttribute.cs` | `[assembly: LanguageSgMetaInfo]` — 指定 Source Generator 输出命名空间 |
| `LanguageResourceExtension.cs` | `LanguageResourceExtension<T>` 泛型 XAML 标记扩展基类 |
| `CommonLangId.cs` | `CommonLangId.Common` — 跨控件通用语言分组常量 |

### 4.2 Source Generator (`src/AtomUI.Generator/Language/`)

| 文件 | 职责 |
|------|------|
| `LanguageInfo.cs` | 数据模型 — 存储单个 LanguageProvider 类的解析结果 |
| `LanguageProviderWalker.cs` | Roslyn Syntax Walker — 遍历 `[LanguageProvider]` 类，提取字段和元信息 |
| `LangResourceKeyClassSourceWriter.cs` | 代码生成器 — 输出 `{LangId}LangResourceKind` 枚举 + `{LangId}LangResourceExtension` 标记扩展 |
| `LanguageProviderPoolClassSourceWriter.cs` | 代码生成器 — 输出 `LanguageProviderPool.GetLanguageProviders()` 工厂方法 |

Source Generator 入口：`src/AtomUI.Generator/LanguageGenerator.cs`

### 4.3 运行时集成

| 文件 | 职责 |
|------|------|
| `src/AtomUI.Core/Theme/ThemeManager.cs` | 管理 `_languages` 字典，处理语言切换的 `ResourceDictionary` 替换 |
| `src/AtomUI.Core/Theme/ThemeManagerBuilder.cs` | 收集 `LanguageProvider` 实例，构建 `ThemeManager` |
| `src/AtomUI.Core/Theme/IThemeManager.cs` | 公开 `LanguageVariant` 属性和 `LanguageVariantChanged` 事件 |
| `src/AtomUI.Core/ApplicationExtensions.cs` | `UseAtomUI()` 入口 — 设置默认语言并初始化 |
| `src/AtomUI.Controls.Shared/ApplicationExtensions.cs` | `SetLanguageVariant()` / `GetLanguageVariant()` 便捷扩展方法 |

---

## 5. 当前支持的语言

| LanguageCode | LanguageVariant | 显示名称 | CultureInfo |
|---|---|---|---|
| `zh_CN` | `LanguageVariant.zh_CN` | 简体中文 | `zh-CN` |
| `en_US` | `LanguageVariant.en_US` | English | `en-US` |

默认语言为 `en_US`（在 `ApplicationExtensions.UseAtomUI()` 中设置），但 Gallery 示例应用使用 `zh_CN` 作为默认语言。

---

## 6. 相关文档

| 文档 | 内容 |
|------|------|
| [Architecture.md](./Architecture.md) | 多语言系统内部架构详解 — 核心类型、Source Generator 管线、资源生命周期 |
| [DeveloperGuide.md](./DeveloperGuide.md) | 开发者使用指南 — 如何为自定义控件添加多语言支持（含完整示例） |

