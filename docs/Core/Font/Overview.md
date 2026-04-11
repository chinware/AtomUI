# AtomUI 字体系统 — 总览

> 本文档描述 AtomUI 字体系统的整体设计架构。AtomUI 字体系统建立在 Avalonia 的 `EmbeddedFontCollection` 和 `FontManager` 基础之上，结合 AtomUI 的 Design Token 主题系统，实现了从字体包嵌入、字体族注册、Token 驱动的字体大小/行高派生，到运行时全局字体切换的完整能力。

---

## 1. 设计背景与目标

### 1.1 Ant Design 5.0 排版规范

Ant Design 5.0 在排版（Typography）方面有如下设计要求：

- **字体族（Font Family）**：优先使用系统默认界面字体，同时提供备用字体库，在不同平台保持良好的易读性和可读性。
- **字体大小（Font Size）**：以 14px 为基准字号，通过数学公式自动派生出从小号到一级标题的完整字号梯度。
- **行高（Line Height）**：与字号配套，遵循 `(fontSize + 8) / fontSize` 的计算公式自动派生。
- **字重（Font Weight）**：标题类组件使用 `SemiBold`，正文使用 `Normal`。

AtomUI 在 .NET / Avalonia 平台完整还原了这一排版设计。

### 1.2 AtomUI 字体系统的三大集成点

```
┌───────────────────────┐     ┌─────────────────────────┐     ┌──────────────────────┐
│   字体包（Font Package）│     │ Design Token 主题系统     │     │  AXAML 资源绑定        │
│                       │     │                         │     │                      │
│  EmbeddedFontCollection│────►│  Seed Token: FontFamily │────►│ {atom:SharedToken-   │
│  字体文件嵌入 (.ttf)    │     │  Map Token: FontSize*   │     │  Resource FontFamily}│
│  自定义 URI Scheme     │     │  Alias Token: FontSize- │     │ {atom:SharedToken-   │
│  (fonts:AlibabaSans)   │     │  Icon, FontWeightStrong │     │  Resource FontSize}  │
│                       │     │  算法派生字号/行高梯度     │     │                      │
└───────────────────────┘     └─────────────────────────┘     └──────────────────────┘
```

1. **字体包（Font Package）** — 将字体文件（`.ttf`/`.otf`）嵌入为 Avalonia 资源，通过 `EmbeddedFontCollection` 注册到 `FontManager`，使字体在应用中全局可用。
2. **Design Token 主题系统** — 字体族作为 Seed Token (`FontFamily`) 参与主题推导链，字号和行高通过主题算法自动派生为 Map Token 梯度。
3. **AXAML 资源绑定** — 控件主题通过 `{atom:SharedTokenResource FontFamily}` 等标记扩展消费字体 Token，实现字体与主题的动态绑定。

### 1.3 设计目标

1. **开箱即用** — 内置 Alibaba Sans 字体包，一行代码即可启用，无需依赖系统字体。
2. **Token 驱动** — 字号、行高、字重等排版属性全部由 Design Token 系统管理，修改 Seed Token 即可全局级联。
3. **可扩展** — 开发者可以制作自己的字体包，替换默认字体或添加额外字体。
4. **运行时可切换** — 通过修改 Token 资源字典中的 `FontFamily` 值即可运行时切换全局字体。
5. **跨平台一致性** — 字体包嵌入机制确保在 Windows/macOS/Linux（未来还有 iOS/Android）上字体显示一致。

---

## 2. 系统架构

### 2.1 组件关系图

```
┌──────────────────────────────────────────────────────────────────────────┐
│  应用层（Application Layer）                                               │
│                                                                          │
│  app.UseAtomUI(builder => {                                              │
│      builder.UseAlibabaSansFont();      // 注册字体包                      │
│      builder.UseDesktopControls();                                       │
│      builder.WithDefaultFontFamily(...) // 可选：覆盖默认字体              │
│  });                                                                     │
└─────────────────────────┬──────────────────────────────────┬─────────────┘
                          │                                  │
┌─────────────────────────▼────────────────┐  ┌──────────────▼─────────────┐
│  AtomUI.Fonts.AlibabaSans                │  │  AtomUI.Core 主题系统        │
│  ┌─────────────────────────────────────┐ │  │  ┌──────────────────────┐   │
│  │ AlibabaSansFontCollection           │ │  │  │ DesignToken          │   │
│  │   : EmbeddedFontCollection          │ │  │  │  .FontFamily (Seed)  │   │
│  │   fonts:AlibabaSans → avares://     │ │  │  │  .FontSize   (Seed)  │   │
│  │   AtomUI.Fonts.AlibabaSans/Assets   │ │  │  │  .FontSizeSM (Map)   │   │
│  │                                     │─┼──►  │  .FontSizeLG (Map)   │   │
│  │ ThemeManagerBuilderExtensions       │ │  │  │  ...更多 Map/Alias    │   │
│  │   .UseAlibabaSansFont()             │ │  │  └──────────┬───────────┘   │
│  │                                     │ │  │             │               │
│  │ AppBuilderExtension                 │ │  │  CalculatorUtils            │
│  │   .WithAlibabaSansFont()            │ │  │   .CalculateFontMap-       │
│  └─────────────────────────────────────┘ │  │    TokenValues()            │
└──────────────────────────────────────────┘  └─────────────────────────────┘
                                                            │
                                              ┌─────────────▼─────────────┐
                                              │  控件主题（Control Themes） │
                                              │                           │
                                              │  WindowTheme.axaml:       │
                                              │    FontFamily="{atom:     │
                                              │    SharedTokenResource    │
                                              │    FontFamily}"           │
                                              │                           │
                                              │  PopupRootTheme.axaml:    │
                                              │    FontFamily="{atom:     │
                                              │    SharedTokenResource    │
                                              │    FontFamily}"           │
                                              └───────────────────────────┘
```

### 2.2 核心模块划分

| 模块 | 项目 | 职责 |
|------|------|------|
| **字体包** | `AtomUI.Fonts.AlibabaSans` | 内置 Alibaba Sans 字体嵌入、`FontManager` 注册 |
| **字体 Token** | `AtomUI.Core` (Theme/TokenSystem/) | Seed Token `FontFamily`/`FontSize`，Map Token 字号/行高梯度派生 |
| **字体梯度算法** | `AtomUI.Core` (Theme/Styling/CalculatorUtils.cs) | 从基准字号自动派生完整字号/行高序列 |
| **字体工具** | `AtomUI.Core` (Media/FontUtils.cs, TextUtils.cs) | em 转 px 工具、文本尺寸测量工具 |
| **控件字体绑定** | `AtomUI.Desktop.Controls` (Themes/*.axaml) | 通过 Token 标记扩展在控件主题中消费字体 Token |

---

## 3. 文档索引

本字体系统文档拆分为以下子文档：

| 文档 | 描述 |
|------|------|
| [Overview.md](Overview.md) | 本文件 — 总览 |
| [FontPackageDesign.md](FontPackageDesign.md) | 字体包内部设计原理（以 `AtomUI.Fonts.AlibabaSans` 为例） |
| [FontTokenIntegration.md](FontTokenIntegration.md) | 字体与 Design Token 系统的集成（Seed → Map → Alias 字体推导链） |
| [UsingBuiltInFont.md](UsingBuiltInFont.md) | 开发者指南：在应用中使用 AtomUI 内置字体 |
| [CreatingCustomFontPackage.md](CreatingCustomFontPackage.md) | 开发者指南：制作自定义字体包 |
| [RuntimeFontSwitching.md](RuntimeFontSwitching.md) | 运行时字体切换机制与实践 |

---

## 4. 源文件清单

### AtomUI.Fonts.AlibabaSans

| 文件 | 职责 |
|------|------|
| `AlibabaSansFontCollection.cs` | 字体集合定义，继承 `EmbeddedFontCollection` |
| `AppBuilderExtension.cs` | Avalonia `AppBuilder` 扩展方法（纯 Avalonia 场景） |
| `ThemeManagerBuilderExtensions.cs` | AtomUI `IThemeManagerBuilder` 扩展方法（AtomUI 主题场景） |
| `Properties/AssemblyInfo.cs` | XAML 命名空间定义 (`XmlnsDefinition`) |
| `AtomUI.Fonts.AlibabaSans.csproj` | 项目文件，配置 `AvaloniaResource` 嵌入字体文件 |
| `Assets/*.ttf` | 6 个字重的 Alibaba Sans 字体文件 |

### AtomUI.Core（字体相关）

| 文件 | 职责 |
|------|------|
| `Theme/TokenSystem/TokenDefinitions/DesignToken.Seed.cs` | Seed Token：`FontFamily`、`FontSize` |
| `Theme/TokenSystem/TokenDefinitions/DesignToken.FontMap.cs` | Map Token：字号梯度、行高梯度、文字高度 |
| `Theme/TokenSystem/TokenDefinitions/DesignToken.Alias.cs` | Alias Token：`FontSizeIcon`、`FontWeightStrong` |
| `Theme/TokenSystem/TokenDefinitions/DesignToken.cs` | Alias 计算逻辑（`CalculateAliasTokenValues`） |
| `Theme/Styling/CalculatorUtils.cs` | 字体梯度派生算法（`CalculateFontMapTokenValues`、`CalculateFontSize`） |
| `Theme/ThemeManagerBuilder.cs` | `WithDefaultFontFamily()` API |
| `ApplicationExtensions.cs` | `UseAtomUI()` 中的字体覆盖逻辑 |
| `Media/FontUtils.cs` | em 到 px 转换工具 |
| `Media/TextUtils.cs` | 文本尺寸计算工具 |

