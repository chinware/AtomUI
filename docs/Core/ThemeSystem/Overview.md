# AtomUI 主题系统架构文档 — 总览

> 本文档描述 AtomUI 主题系统的整体设计架构。AtomUI 主题系统是对 [Ant Design 5.0 Design Token 体系](https://ant.design/docs/react/customize-theme-cn) 的 C# / Avalonia 完整移植，实现了与 Ant Design TypeScript 版本功能等价的主题定制能力。

---

## 1. 设计背景与目标

### 1.1 Ant Design 5.0 主题定制体系回顾

Ant Design 5.0 采用了 **Design Token** 驱动的主题系统，核心理念是：

- **Seed Token（种子变量）**：极少数用户可直接修改的基础设计意图（如品牌主色 `colorPrimary`、字体大小 `fontSize`），修改后自动级联派生下层所有 Token。
- **Map Token（梯度变量）**：由 Seed Token 通过 **主题算法（Algorithm）** 自动派生，形成完整的色彩梯度、尺寸梯度、字体梯度等。
- **Alias Token（别名变量）**：对 Map Token 的语义化封装，面向组件批量消费（如 `colorTextDisabled`、`colorBgContainer`）。
- **Component Token（组件变量）**：每个组件独有的 Token，可以从全局 SharedToken 派生，也可以被用户单独覆盖。
- **主题算法**：Default（亮色）、Dark（暗色）、Compact（紧凑），算法可自由组合。

Ant Design 在 React 中通过 `ConfigProvider` 的 `theme` 属性实现上述能力。AtomUI 在 .NET / Avalonia 平台完整还原了这一设计。

### 1.2 AtomUI 的对应实现

| Ant Design (TypeScript)             | AtomUI (C#)                                      |
|-------------------------------------|--------------------------------------------------|
| `theme.token` (Seed/Map/Alias)      | `DesignToken` (partial class, 三层 Token 定义)    |
| `theme.algorithm`                   | `ThemeAlgorithm` enum + `IThemeVariantCalculator` |
| `theme.components[Component].token` | `AbstractControlDesignToken` 子类                  |
| `ConfigProvider`                    | `ThemeConfigProvider` (XAML 组件) + XML 主题定义    |
| `@ant-design/colors` (色板生成)     | `PaletteGenerator` (HSV 色板算法)                  |
| CSS-in-JS Token 注入               | Avalonia `ResourceDictionary` + Token MarkupExtension |

### 1.3 设计目标

1. **完全还原 Ant Design 5.0 Design Token 语义** — 从 Seed → Map → Alias → Component 四层推导链完整实现。
2. **主题算法可组合** — Default / Dark / Compact 可任意叠加（装饰器模式）。
3. **运行时主题热切换** — 利用 Avalonia `ThemeVariant` + `ResourceDictionary.ThemeDictionaries` 实现零重启切换。
4. **XML + XAML 双通道配置** — 全局主题通过 XML 定义文件配置，局部嵌套通过 XAML `ThemeConfigProvider` 配置。
5. **源代码生成器驱动** — Token ResourceKey 枚举通过 Roslyn Source Generator 自动生成，编译期保障类型安全。
6. **组件级 Token 隔离** — 每个组件有独立的 Token 作用域，支持组件级别的 SharedToken 覆盖。

---

## 2. 整体架构

### 2.1 系统层次图

```
┌──────────────────────────────────────────────────────────────────┐
│                        Application Layer                         │
│                                                                  │
│  app.UseAtomUI(builder => {                                      │
│      builder.UseDesktopControls();                               │
│      builder.WithDefaultTheme("DaybreakBlue");                   │
│  });                                                             │
└────────────────────────────┬─────────────────────────────────────┘
                             │
┌────────────────────────────▼─────────────────────────────────────┐
│                    ThemeManagerBuilder                            │
│  收集: ControlDesignTokens, ControlThemesProviders,              │
│        LanguageProviders, ThemeAssetPathProviders                 │
│  Build() ──────────────────▼─────────────────────────            │
│                        ThemeManager                               │
│  (extends Avalonia.Styling.Styles, implements IThemeManager)      │
│  ┌─────────────────────────────────────────────────────────┐     │
│  │ ThemePool: Dict<ThemeVariant, Theme>                     │     │
│  │                                                         │     │
│  │  ┌──────────────┐ ┌───────────────────┐ ┌────────────┐ │     │
│  │  │ DaybreakBlue │ │ DaybreakBlue-Dark │ │ ...        │ │     │
│  │  │ (Theme)      │ │ (Theme)           │ │            │ │     │
│  │  └──────┬───────┘ └────────┬──────────┘ └────────────┘ │     │
│  │         │                  │                            │     │
│  │         ▼                  ▼                            │     │
│  │  ┌───────────────────────────────┐                      │     │
│  │  │      Theme.BuildThemeResource()                      │     │
│  │  │  1. Load Seed Token config                           │     │
│  │  │  2. Calculator.Calculate() → Map Tokens              │     │
│  │  │  3. CalculateAliasTokenValues() → Alias Tokens       │     │
│  │  │  4. ControlToken.CalculateTokenValues()              │     │
│  │  │  5. BuildResourceDictionary() → ResourceDictionary   │     │
│  │  └───────────────────────────────┘                      │     │
│  └─────────────────────────────────────────────────────────┘     │
└──────────────────────────────────────────────────────────────────┘
                             │
┌────────────────────────────▼─────────────────────────────────────┐
│                    Avalonia Resource System                       │
│                                                                  │
│  ThemeManager.Resources.ThemeDictionaries[themeVariant]           │
│    = Theme.ThemeResource (ResourceDictionary)                    │
│                                                                  │
│  控件通过 DynamicResource / MarkupExtension 消费 Token:          │
│    {atom:SharedTokenResource ColorPrimary}                       │
│    {atom:TokenResource ButtonPrimaryColor}                       │
└──────────────────────────────────────────────────────────────────┘
```

### 2.2 核心模块划分

| 模块 | 目录 | 职责 |
|------|------|------|
| **主题管理** | `Theme/` 根目录 | ThemeManager, Theme, ThemeDefinition, Builder 模式 |
| **Token 体系** | `Theme/TokenSystem/` | DesignToken, AbstractControlDesignToken, Token 属性/Attribute |
| **Token 定义** | `Theme/TokenSystem/TokenDefinitions/` | DesignToken partial class 的 Seed/Map/Alias 定义 |
| **主题算法** | `Theme/Styling/` | IThemeVariantCalculator 及 Default/Dark/Compact 实现 |
| **调色板** | `Theme/Palette/` | PaletteGenerator, PresetPrimaryColor, PresetPalettes |
| **资源扩展** | `Theme/` (TokenResourceExtension 等) | XAML MarkupExtension, StyleExtensions |
| **Source Generator** | `AtomUI.Generator/` (外部项目) | TokenResourceKeyGenerator 自动生成 Enum 资源键 |

### 2.3 与 Ant Design TypeScript 版本的对应关系

```
Ant Design (TypeScript)                    AtomUI (C#)
──────────────────────────                 ──────────────────────────────────
theme/interface/seeds.ts                   DesignToken.Seed.cs
theme/interface/maps/colors.ts             DesignToken.ColorPrimaryMap.cs 等
theme/interface/maps/font.ts               DesignToken.FontMap.cs
theme/interface/maps/size.ts               DesignToken.SizeMap.cs
theme/interface/maps/style.ts              DesignToken.StyleMap.cs
theme/interface/alias.ts                   DesignToken.Alias.cs
theme/themes/default/index.ts              DefaultThemeVariantCalculator.cs
theme/themes/dark/index.ts                 DarkThemeVariantCalculator.cs
theme/themes/compact/index.ts              CompactThemeVariantCalculator.cs
@ant-design/colors                         PaletteGenerator.cs
theme/util/genComponentStyleHook.ts        AbstractControlDesignToken.cs
ConfigProvider (theme prop)                ThemeConfigProvider.cs + XML 定义
CSS-in-JS token injection                  ResourceDictionary + MarkupExtension
```

---

## 3. 文档索引

本主题系统文档拆分为以下模块文档：

| 文档 | 描述 |
|------|------|
| [Overview.md](Overview.md) | 本文件 — 总览 |
| [TokenArchitecture.md](TokenArchitecture.md) | Design Token 四层架构详解（Seed → Map → Alias → Component） |
| [ThemeAlgorithm.md](ThemeAlgorithm.md) | 主题算法（Default/Dark/Compact）与调色板生成 |
| [ThemeManagement.md](ThemeManagement.md) | 主题管理器、主题生命周期、运行时切换 |
| [TokenResourceBinding.md](TokenResourceBinding.md) | Token 资源绑定、XAML MarkupExtension、Source Generator |
| [ThemeCustomizationPrinciples.md](ThemeCustomizationPrinciples.md) | 主题自定义原理说明（加载机制、作用域实现、组件级覆盖） |
| [ThemeCustomizationGuide.md](ThemeCustomizationGuide.md) | 主题定制开发者指南（完整步骤、API 参考、Gallery 示例解读） |

---

## 4. 快速参考：源文件清单

### Theme/ 根目录

| 文件 | 职责 |
|------|------|
| `ITheme.cs` | 主题公共接口 |
| `Theme.cs` | 主题实现（加载、构建资源、生命周期） |
| `IThemeManager.cs` | 主题管理器公共接口 |
| `ThemeManager.cs` | 主题管理器实现（主题池、切换、语言、动效开关） |
| `IThemeManagerBuilder.cs` | 构建器接口 |
| `ThemeManagerBuilder.cs` | 构建器实现 |
| `ThemeAlgorithm.cs` | 主题算法枚举（Default, Dark, Compact） |
| `ThemeDefinition.cs` | 主题定义数据模型 |
| `ThemeDefinitionReader.cs` | XML 主题定义文件解析器 |
| `ThemeConfigProvider.cs` | XAML 局部主题配置组件 |
| `IThemeConfigProvider.cs` | 局部主题配置接口 |
| `BaseControlTheme.cs` | 控件主题基类 |
| `IControlThemesProvider.cs` | 控件主题提供者接口 |
| `IThemeAssetPathProvider.cs` | 主题资产路径提供者接口 |
| `ControlTokenResourcesHost.cs` | 控件级 Token 资源作用域宿主 |
| `IControlTokenResourceScopeProvider.cs` | 控件级 Token 作用域提供者接口 |
| `TokenResourceExtension.cs` | 泛型 Token 资源 XAML 标记扩展基类 |
| `TokenSetter.cs` | Token 设置器（用于 XAML 配置） |
| `ControlTokenInfoSetter.cs` | 控件 Token 信息设置器 |
| `ThemeExceptions.cs` | 主题系统异常定义 |

### Theme/TokenSystem/

| 文件 | 职责 |
|------|------|
| `IDesignToken.cs` | Design Token 接口 |
| `AbstractDesignToken.cs` | Design Token 抽象基类（配置加载、资源构建、Clone） |
| `IControlDesignToken.cs` | 控件 Design Token 接口 |
| `AbstractControlDesignToken.cs` | 控件 Design Token 抽象基类（组件级 Token） |
| `DesignTokenKind.cs` | Token 分类枚举（Seed, Map, Alias） |
| `DesignTokenKindAttribute.cs` | Token 分类标注 Attribute |
| `GlobalDesignTokenAttribute.cs` | 全局 Token 标注 Attribute |
| `ControlDesignTokenAttribute.cs` | 控件 Token 标注 Attribute |
| `NotTokenDefinitionAttribute.cs` | 非 Token 属性排除标注 |
| `ControlTokenConfigInfo.cs` | 控件 Token 配置信息 |
| `ITokenValueConverter.cs` | Token 值转换器接口 |
| `TokenValueConverterAttribute.cs` | Token 值转换器标注 |
| `BuiltInTokenValueConverters.cs` | 内置 Token 值转换器集合 |

### Theme/TokenSystem/TokenDefinitions/

| 文件 | 内容 | Token 层级 |
|------|------|-----------|
| `DesignToken.cs` | DesignToken 主类（构造、初始化、Alias 计算） | — |
| `DesignToken.Seed.cs` | Seed Token 定义 | Seed |
| `DesignToken.ColorPrimaryMap.cs` | 主色系梯度（1-10号色） | Map |
| `DesignToken.ColorSuccessMap.cs` | 成功色系梯度 | Map |
| `DesignToken.ColorWarningMap.cs` | 警戒色系梯度 | Map |
| `DesignToken.ColorErrorMap.cs` | 错误色系梯度 | Map |
| `DesignToken.ColorInfoMap.cs` | 信息色系梯度 | Map |
| `DesignToken.ColorLinkMap.cs` | 链接色系梯度 | Map |
| `DesignToken.ColorNeutralMap.cs` | 中性色系（文本/边框/填充/背景） | Map |
| `DesignToken.ColorMap.cs` | 通用颜色（黑/白/遮罩/选区） | Map |
| `DesignToken.FontMap.cs` | 字体梯度（大小/行高） | Map |
| `DesignToken.SizeMap.cs` | 尺寸梯度（XXS-XXL） | Map |
| `DesignToken.HeightMap.cs` | 控件高度梯度 | Map |
| `DesignToken.StyleMap.cs` | 样式梯度（圆角/线宽/动效时长） | Map |
| `DesignToken.Alias.cs` | 别名 Token（语义化消费层） | Alias |

### Theme/Styling/

| 文件 | 职责 |
|------|------|
| `IThemeVariantCalculator.cs` | 主题变体计算器接口 |
| `AbstractThemeVariantCalculator.cs` | 计算器抽象基类（装饰器模式） |
| `DefaultThemeVariantCalculator.cs` | 默认（亮色）算法 |
| `DarkThemeVariantCalculator.cs` | 暗色算法 |
| `CompactThemeVariantCalculator.cs` | 紧凑算法 |
| `IThemeVariantCalculatorFactory.cs` | 自定义计算器工厂接口 |
| `CalculatorUtils.cs` | 计算工具（字体/尺寸/圆角/动效时长派生） |
| `ColorMap.cs` | 10 级色阶映射 |
| `SharedTokenResourceExtension.cs` | 全局 Token XAML 标记扩展 |
| `IControlThemeProvider.cs` | 控件主题提供者接口 |
| `ControlThemeProviderAttribute.cs` | 控件主题提供者标注 |
| `SetterValueFactory.cs` | Setter 值工厂 |
| `StyleExtensions.cs` | Style 扩展方法 |

### Theme/Palette/

| 文件 | 职责 |
|------|------|
| `PaletteGenerator.cs` | 色板生成器（HSV 算法，对标 `@ant-design/colors`） |
| `PresetPrimaryColor.cs` | 14 种预设主色 |
| `PresetPalettes.cs` | 预计算的亮色/暗色调色板集合 |

