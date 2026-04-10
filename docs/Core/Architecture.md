# AtomUI.Core 项目架构文档

## 1. 项目概述

`AtomUI.Core` 是 AtomUI 组件库的核心基础框架，构建于 [Avalonia UI](https://avaloniaui.net/) 之上。它提供了完整的 **主题系统（Theme System）**、**Design Token 体系**、**动效引擎（Motion Scene）**、**国际化（i18n）** 以及一系列基础设施工具，为上层控件库（如 `AtomUI.Controls`、`AtomUI.Desktop.Controls` 等）提供统一的底层支撑。

设计哲学借鉴了 [Ant Design](https://ant.design/) 的 Design Token 体系，实现了主题的可定制、可扩展、可切换的能力。

### 依赖关系

```
AtomUI.Core
├── Avalonia                  (UI 框架)
├── ReactiveUI.Avalonia       (响应式 MVVM)
├── System.Reactive           (响应式扩展)
├── AtomUI.Generator          (源代码生成器，仅 Analyzer)
└── AtomUI.Native             (原生平台交互)
```

### InternalsVisibleTo

以下项目可访问 `AtomUI.Core` 的 `internal` 成员：

- `AtomUI.Controls`
- `AtomUI.Controls.Shared`
- `AtomUI.Desktop.Controls`
- `AtomUI.Desktop.Controls.DataGrid`
- `AtomUI.Desktop.Controls.ColorPicker`

---

## 2. 目录结构总览

```
AtomUI.Core/
├── ApplicationExtensions.cs          # Application 扩展方法（UseAtomUI 入口）
├── AtomUIRuntimeInfo.cs              # 运行时信息
├── Common.cs                         # 公共常量与日志区域定义
├── Dimension.cs                      # 尺寸相关定义
├── OperatingSystemType.cs            # 操作系统类型枚举
├── StdPseudoClass.cs                 # 标准伪类常量
│
├── Animations/                       # 动画基础设施
│   ├── Transitions/                  # 可通知的 Transition 实现
│   └── ...                           # 动画辅助工具
│
├── Assets/                           # 内嵌资源
│   └── Themes/                       # 内置主题定义文件 (XML)
│
├── Collections/                      # 集合工具类
│   └── Pooled/                       # 对象池化集合
│
├── Controls/                         # 核心控件基础设施
│   └── Icon/                         # Icon 系统
│
├── Data/                             # 数据绑定工具
├── Exceptions/                       # 自定义异常
├── Input/                            # 输入处理
├── Language/                         # 国际化 (i18n) 系统
├── Media/                            # 图形与媒体工具
│   └── TextFormatting/               # 文本排版
│
├── MotionScene/                      # 动效引擎
├── Reactive/                         # 响应式扩展
├── Reflection/                       # 反射工具
│
├── Theme/                            # 🔑 主题系统（核心）
│   ├── Palette/                      # 调色板生成器
│   ├── Styling/                      # 主题变体计算器
│   └── TokenSystem/                  # Design Token 体系
│       └── TokenDefinitions/         # 全局 Token 定义（Seed/Map/Alias）
│
└── Utils/                            # 通用工具类
```

---

## 3. 核心模块架构

### 3.1 主题系统 (Theme)

主题系统是 AtomUI.Core 的核心，实现了完整的主题生命周期管理，包括创建、加载、激活、切换和卸载。

#### 3.1.1 整体架构

```
┌─────────────────────────────────────────────────────┐
│                   Application                        │
│                                                      │
│  app.UseAtomUI(builder => { ... })                   │
│         │                                            │
│         ▼                                            │
│  ┌─────────────────────┐                             │
│  │ ThemeManagerBuilder  │ ── 构建器模式               │
│  │ (IThemeManagerBuilder)│                            │
│  └────────┬────────────┘                             │
│           │ .Build()                                 │
│           ▼                                          │
│  ┌─────────────────────┐    ┌──────────────────┐     │
│  │   ThemeManager      │───▶│   Theme (N个)    │     │
│  │   (IThemeManager)   │    │   (ITheme)       │     │
│  │                     │    └───────┬──────────┘     │
│  │ - 主题池管理         │            │                 │
│  │ - 主题切换           │            ▼                 │
│  │ - 语言管理           │    ┌──────────────────┐     │
│  │ - 动效开关           │    │  DesignToken     │     │
│  │ - ResourceDictionary │    │  (SharedToken)   │     │
│  └─────────────────────┘    │  ControlTokens   │     │
│                              └──────────────────┘     │
└─────────────────────────────────────────────────────┘
```

#### 3.1.2 关键接口与类

| 类/接口 | 职责 |
|--------|------|
| `IThemeManager` | 主题管理器公共接口，定义主���切换、语言、动效等属性 |
| `ThemeManager` | 主题管理器实现，继承 `Styles`，管理主题池、控件主题、语言资源 |
| `IThemeManagerBuilder` | 主题管理器构建器接口（Builder 模式） |
| `ThemeManagerBuilder` | 构建器实现，收集 Token、主题提供者、语言提供者，最终 `Build()` 出 `ThemeManager` |
| `ITheme` | 主题公共接口，包含 ID、显示名、暗色模式、Token 等 |
| `Theme` | 主题实现，负责加载主题定义文件、构建资源字典 |
| `ThemeDefinition` | 主题定义数据模型（从 XML 文件解析） |
| `ThemeDefinitionReader` | 主题定义文件（XML）解析器 |
| `ThemeAlgorithm` | 主题算法枚举：`Default` / `Dark` / `Compact` |
| `BaseControlTheme` | 控件主题基类，定义控件模板、样式的构建流程 |
| `IControlThemesProvider` | 控件主题提供者接口 |
| `IThemeAssetPathProvider` | 主题资产路径提供者接口 |

#### 3.1.3 主题生命周期

```
 扫描主题文件 (XML)
      │
      ▼
  创建 Theme 实例 ──▶ ThemeCreated 事件
      │
      ▼
  注册到 ThemePool (ThemeVariant → Theme)
      │
      ▼
  加载 Theme ──▶ ThemeAboutToLoad → 解析定义文件 → 构建资源 → ThemeLoaded
      │
      ▼
  激活 Theme ──▶ ThemeAboutToChange → 切换 ResourceDictionary → ThemeChanged
      │
      ▼
  运行中 (Active)
      │
      ▼
  切换/卸载 ──▶ NotifyAboutToDeActive → NotifyDeActivated
```

#### 3.1.4 主题算法组合

每个主题定义文件可以生成 **4 种变体**：

| 组合 | ThemeVariant ID 示例 |
|------|---------------------|
| Default | `DaybreakBlue` |
| Default + Dark | `DaybreakBlue-Dark` |
| Default + Dark + Compact | `DaybreakBlue-Dark-Compact` |
| Default + Compact | `DaybreakBlue-Compact` |

---

### 3.2 Design Token 体系 (TokenSystem)

借鉴 Ant Design 的 Design Token 分层体系，将设计变量分为三个层级。

#### 3.2.1 Token 分层

```
┌────────────────────────────────────────────┐
│            Seed Token (种子令牌)             │
│  colorPrimary, fontSize, borderRadius ...  │
│  用户可自定义的基础设计变量                    │
└────────────────────┬───────────────────────┘
                     │ ThemeVariantCalculator 计算
                     ▼
┌────────────────────────────────────────────┐
│            Map Token (梯度令牌)              │
│  colorPrimaryHover, fontSizeLG ...         │
│  由 Seed Token 经过算法派生                   │
└────────────────────┬───────────────────────┘
                     │ CalculateAliasTokenValues()
                     ▼
┌────────────────────────────────────────────┐
│            Alias Token (别名令牌)            │
│  colorText, colorBgContainer ...           │
│  语义化的 Token，直接用于组件样式              │
└────────────────────────────────────────────┘
```

#### 3.2.2 Token 类型

| 类型 | 描述 |
|------|------|
| `DesignToken` (SharedToken) | 全局共享 Token，包含 Seed、Map、Alias 三层 |
| `AbstractControlDesignToken` | 控件专属 Token 基类，每个控件可定义自己的 Token |
| `DesignTokenKind` | Token 分类枚举：Seed、Map、Alias |

#### 3.2.3 Token 定义文件

`TokenDefinitions/` 目录按 Token 类别使用 partial class 组织 `DesignToken`：

| 文件 | 内容 |
|------|------|
| `DesignToken.Seed.cs` | 种子令牌（colorPrimary 等） |
| `DesignToken.FontMap.cs` | 字体梯度令牌 |
| `DesignToken.HeightMap.cs` | 高度梯度令牌 |
| `DesignToken.SizeMap.cs` | 尺寸梯度令牌 |
| `DesignToken.StyleMap.cs` | 样式梯度令牌 |
| `DesignToken.ColorMap.cs` | 颜色梯度令牌 |
| `DesignToken.ColorPrimaryMap.cs` | 主色系梯度令牌 |
| `DesignToken.ColorSuccessMap.cs` | 成功色系梯度令牌 |
| `DesignToken.ColorWarningMap.cs` | 警告色系梯度令牌 |
| `DesignToken.ColorErrorMap.cs` | 错误色系梯度令牌 |
| `DesignToken.ColorInfoMap.cs` | 信息色系梯度令牌 |
| `DesignToken.ColorLinkMap.cs` | 链接色系梯度令牌 |
| `DesignToken.ColorNeutralMap.cs` | 中性色系梯度令牌 |
| `DesignToken.Alias.cs` | 别名令牌 |

#### 3.2.4 Token 资源构建流程

```
1. 加载 Seed Token 配置 (XML → LoadConfig)
2. ThemeVariantCalculator.Calculate() 计算 Map Token
3. 覆盖用户自定义 Map Token
4. CalculateAliasTokenValues() 计算 Alias Token
5. 覆盖用户自定义 Alias Token
6. BuildResourceDictionary() → 注入 Avalonia ResourceDictionary
7. 对每个 ControlToken 重复类似流程
```

---

### 3.3 主题变体计算器 (Styling)

负责根据不同的主题算法计算 Token 值。采用 **装饰器/链式模式**，支持算法叠加。

```
┌──────────────────────────┐
│ IThemeVariantCalculator  │ ◀── 接口
├──────────────────────────┤
│ Calculate(DesignToken)   │
│ ColorBgBase              │
│ ColorTextBase            │
└──────────┬───────────────┘
           │
     ┌─────┼──────────────┬───────────────────┐
     ▼     ▼              ▼                   ▼
  Default  Dark         Compact    IThemeVariantCalculatorFactory
 Calculator Calculator  Calculator   (可自定义工厂)
```

| 类 | 职责 |
|----|------|
| `DefaultThemeVariantCalculator` | 默认（亮色）算法 |
| `DarkThemeVariantCalculator` | 暗色算法，装饰 base calculator |
| `CompactThemeVariantCalculator` | 紧凑算法，装饰 base calculator |
| `IThemeVariantCalculatorFactory` | 自定义计算器工厂接口 |

---

### 3.4 调色板系统 (Palette)

实现了与 Ant Design 一致的调色板生成算法。

```
PaletteGenerator
├── GeneratePalette(Color, Option)  → IReadOnlyList<Color> (10 级色阶)
│   ├── 浅色 5 级 (Light)
│   ├── 主色 1 级 (Primary)
│   └── 深色 4 级 (Dark)
│
├── PresetPalettes              → 预设调色板集合
└── PresetPrimaryColor          → 预设主色枚举
```

调色板生成算法基于 **HSV 色彩空间**，通过色相（Hue）、饱和度（Saturation）、明度（Value）的阶梯变化生成 10 级色阶。暗色模式下使用不同的混合策略。

---

### 3.5 动效引擎 (MotionScene)

提供了一套完整的动画/过渡效果框架，支持 **Animation** 和 **Transition** 两种驱动模式。

#### 3.5.1 架构

```
┌─────────────┐     ┌──────────────────┐     ┌─────────────┐
│  IMotion     │────▶│ AbstractMotion    │────▶│ 具体 Motion  │
│  (接口)      │     │ (抽象基类)        │     │ (Fade/Slide │
└─────────────┘     └──────────────────┘     │  /Zoom/...)  │
                                              └─────────────┘
                              │
                              │ Run(actor)
                              ▼
┌─────────────┐     ┌──────────────────┐     ┌─────────────┐
│ IMotionActor │────▶│ BaseMotionActor  │────▶│ SceneLayer  │
│ (接口)       │     │ (ContentControl) │     │ (顶层窗口)   │
└─────────────┘     └──────────────────┘     └─────────────┘
```

#### 3.5.2 内置动效

| 类别 | 动效 |
|------|------|
| **Fade** | `FadeInMotion`, `FadeOutMotion` |
| **Slide** | `SlideUpIn/Out`, `SlideDownIn/Out`, `SlideLeftIn/Out`, `SlideRightIn/Out` |
| **Zoom** | `ZoomIn/Out`, `ZoomBigIn/Out`, `ZoomUpIn/Out`, `ZoomDownIn/Out`, `ZoomLeftIn/Out`, `ZoomRightIn/Out` |
| **Collapse** | `CollapseMotions` — 折叠/展开动效 |
| **Move** | `MoveMotions` — 移动动效 |

#### 3.5.3 动效执行流程

```
Motion.Run(actor)
  │
  ├─ Transition 模式:
  │   1. ConfigureTransitions()      — 配置过渡属性
  │   2. ConfigureMotionStartValue() — 设置起始值
  │   3. actor.Transitions = transitions
  │   4. ConfigureMotionEndValue()   — 设置结束值（触发过渡）
  │   5. await Task.WhenAny(...)     — 等待完成
  │   6. NotifyCompleted()
  │
  └─ Animation 模式:
      1. ConfigureAnimation()        — 配置关键帧动画
      2. foreach animation → RunAsync(actor)
      3. NotifyCompleted()
```

---

### 3.6 国际化系统 (Language)

提供多语言支持，基于 Avalonia 的 `ResourceDictionary` 实现。

```
┌───────────────────────┐
│ ILanguageProvider      │ ◀── 接口
├───────────────────────┤
│ LangCode              │
│ LangId                │
│ BuildResourceDictionary│
└───────────┬───────────┘
            │
            ▼
┌───────────────────────┐     ┌───────────────────┐
│ LanguageProvider       │     │ LanguageVariant    │
│ (abstract)             │     │ (zh_CN, en_US ...) │
├───────────────────────┤     └───────────────────┘
│ @LanguageProviderAttr │
│ GetResourceKindType() │
└───────────────────────┘
            │
            ▼
    ThemeManager._languages
    Dict<LanguageVariant, ResourceDictionary>
```

#### 关键类

| 类 | 职责 |
|----|------|
| `LanguageVariant` | 语言变体标识（如 `zh_CN`、`en_US`） |
| `LanguageCode` | 语言代码枚举 |
| `LanguageProvider` | 语言提供者抽象基类 |
| `LanguageProviderAttribute` | 语言提供者元数据注解 |
| `LanguageResourceExtension` | XAML 语言资源标记扩展 |

---

### 3.7 Icon 系统 (Controls/Icon)

提供可扩展的矢量图标框架。

```
┌──────────────────┐     ┌────────────────────────┐
│  Icon            │     │ IconProvider<TIconKind> │
│  (abstract)      │     │ (MarkupExtension)      │
│  extends PathIcon│     │ 泛型 XAML 标记扩展       │
├──────────────────┤     └────────────────────────┘
│ StrokeBrush      │              │
│ FillBrush        │              │ ProvideValue()
│ IconTheme        │              ▼
│ LoadingAnimation │     ┌────────────────────────┐
│ Render()         │     │ IconProviderCache      │
└──────────────────┘     │ (Expression Tree 缓存)  │
                         └────────────────────────┘
```

**特性：**
- 支持多主题类型：`Filled`、`Outlined`、`Rounded`、`Sharp` 等
- 支持旋转动画（Spin/Pulse）
- 支持颜色过渡动画
- 使用 `Expression Tree` 编译缓存优化图标实例创建性能
- 自定义渲染（`DrawingInstruction` 指令系统）

---

### 3.8 动画基础设施 (Animations)

#### 3.8.1 Notifiable Transitions

提供了一整套带完成通知的 Transition 实现，解决了 Avalonia 原生 Transition 缺少完成回调的问题。

```
INotifyTransitionCompleted
└── AbstractNotifiableTransition<T>
    ├── NotifiableDoubleTransition
    ├── NotifiableBoolTransition
    ├── NotifiableCornerRadiusTransition
    ├── NotifiableThicknessTransition
    ├── NotifiablePointTransition
    ├── NotifiableSizeTransition
    ├── NotifiableFloatTransition
    ├── NotifiableIntegerTransition
    ├── NotifiableRelativePointTransition
    ├── NotifiableVectorTransition
    ├── NotifiableBoxShadowsTransition
    ├── NotifiableTransformOperationsTransition
    ├── SolidColorBrushTransition
    └── BoxShadowsTransition
```

#### 3.8.2 工具类

| 类 | 职责 |
|----|------|
| `AnimationExtensions` | 动画相关扩展方法 |
| `BaseTransitionUtils` | Transition 创建工厂方法 |
| `InterpolateUtils` | 插值计算工具 |

---

### 3.9 数据绑定工具 (Data)

| 类 | 职责 |
|----|------|
| `BindUtils` | 通用绑定辅助 |
| `TokenResourceBinder` | Token 资源绑定器，将 Design Token 绑定到控件属性 |
| `LanguageResourceBinder` | 语言资源绑定器 |
| `TokenResourceUtils` | Token 资源查找工具 |
| `TokenFinderUtils` | Token 查找工具 |
| `RenderScaleAwareDoubleConfigure` | 渲染缩放感知的 double 配置 |
| `InstancedBindingFactory` | 实例化绑定工厂 |

---

### 3.10 媒体与图形 (Media)

| 类 | 职责 |
|----|------|
| `ColorUtils` | 颜色工具（HSV/RGB 转换等） |
| `ColorExtensions` | Color 扩展方法 |
| `GeometryUtils` | 几何图形工具 |
| `CommonShapeBuilder` | 常用形状构建器 |
| `FontUtils` | 字体工具 |
| `PenUtils` | 画笔工具 |
| `TextUtils` | 文本工具 |
| `DrawingContextExtensions` | DrawingContext 扩展 |
| `BoxShadowExtensions` | 盒阴影扩展 |
| `TransformParser` | 变换解析器 |
| `ColorTransition` | 颜色过渡 |
| `PixelPointTransition` | 像素点过渡 |
| `RectTransition` | 矩形区域过渡 |

---

### 3.11 工具类 (Utils)

| 类 | 职责 |
|----|------|
| `MathUtils` | 数学工具（浮点比较、四舍五入等） |
| `MatrixUtils` | 矩阵计算工具 |
| `BorderUtils` | 边框计算工具 |
| `StringUtils` | 字符串工具 |
| `StringBuilderCache` | StringBuilder 缓存池 |
| `SpanStringTokenizer` | 高性能字符串分词器 |
| `EnumExtensions` | 枚举扩展方法 |
| `TypeHelper` | 类型辅助 |
| `AssetsBitmapLoader` | 资源位图加载器 |
| `MotionAwareControlExtensions` | 动效感知控件扩展 |

---

### 3.12 集合 (Collections)

| 类 | 职责 |
|----|------|
| `ItemCollection` | 项目集合 |
| `ItemsSourceView` | 数据源视图 |
| `CollectionChangedEventManager` | 集合变更事件管理器 |
| `ICollectionChangedListener` | 集合变更监听接口 |
| `PooledList<T>` | 对象池化 List（减少 GC 压力） |
| `PooledStack<T>` | 对象池化 Stack |

---

## 4. 应用入口与初始化流程

```csharp
// Program.cs
app.UseAtomUI(builder =>
{
    builder.WithDefaultTheme("DaybreakBlue");
    builder.WithDefaultLanguageVariant(LanguageVariant.zh_CN);
    builder.WithDefaultFontFamily("Microsoft YaHei");
    builder.AddControlToken(typeof(MyControlToken));
    builder.AddControlThemesProvider(new MyThemesProvider());
    builder.AddLanguageProviders(new MyLanguageProvider());
});
```

**初始化流程：**

```
UseAtomUI()
  │
  ├── 1. 创建 ThemeManagerBuilder
  ├── 2. 用户配置（Token、主题、语言等）
  ├── 3. builder.Build() → ThemeManager
  │      ├── 注册 ControlThemesProvider
  │      ├── 注册 ControlTokenTypes
  │      ├── 注册 ThemeAssetPathProviders
  │      └── 注册 LanguageProviders
  ├── 4. themeManager.Configure()
  │      ├── ScanThemes()（扫描并创建所有主题变体）
  │      ├── 合并控件主题资源
  │      └── BuildLanguageResources()
  ├── 5. AvaloniaLocator 注册 ThemeManager
  ├── 6. NotifyInitialized()
  ├── 7. 设置 RequestedThemeVariant
  ├── 8. 设置 LanguageVariant
  └── 9. AttachApplication()（绑定到 Application.Styles）
```

---

## 5. 类关系图

```
                        ┌──────────────────────┐
                        │   ApplicationExtensions│
                        │   UseAtomUI()         │
                        └──────────┬───────────┘
                                   │
                    ┌──────────────▼──────────────┐
                    │    ThemeManagerBuilder       │
                    │    (IThemeManagerBuilder)    │
                    └──────────────┬──────────────┘
                                   │ Build()
                    ┌──────────────▼──────────────┐
                    │       ThemeManager           │
                    │       (IThemeManager)        │
                    │    extends Styles            │
                    ├─────────────────────────────┤
                    │ ThemeVariant                 │
                    │ LanguageVariant              │
                    │ IsMotionEnabled              │
                    │ IsDarkThemeMode              │
                    │ IsCompactThemeMode           │
                    └──────┬──────────┬───────────┘
                           │          │
              ┌────────────▼──┐  ┌────▼───────────────┐
              │   Theme (N)   │  │ LanguageProvider(N) │
              │   (ITheme)    │  │ (ILanguageProvider) │
              ├───────────────┤  └────────────────────┘
              │ SharedToken   │
              │ ControlTokens │
              │ ThemeResource │
              └──┬────────┬──┘
                 │        │
     ┌───────────▼──┐  ┌──▼──────────────────────┐
     │ DesignToken   │  │ AbstractControlDesignToken│
     │ (Shared)      │  │ (per-control Token)      │
     │ Seed/Map/Alias│  └──────────────────────────┘
     └───────────────┘
            │
            │ Calculate()
            ▼
  ┌─────────────────────────┐
  │ IThemeVariantCalculator  │
  ├─────────────────────────┤
  │ DefaultCalculator        │
  │ DarkCalculator           │
  │ CompactCalculator        │
  └─────────────────────────┘
```

---

## 6. 设计亮点

1. **Design Token 三层体系** — Seed → Map → Alias 的分层推导，兼顾灵活性与一致性
2. **主题算法装饰器模式** — Default/Dark/Compact 算法可链式叠加
3. **主题热切换** — 运行时切换主题，通过 `ResourceDictionary` 的 `ThemeDictionaries` 实现
4. **Notifiable Transitions** — 解决 Avalonia 原生 Transition 无法感知完成的问题
5. **动效引擎双模式** — 同时支持 Animation（关键帧）和 Transition（属性过渡）两种驱动
6. **Icon 性能优化** — Expression Tree 编译缓存，避免反复反射创建实例
7. **Builder 模式初始化** — 清晰的初始化流程，易于扩展和测试
8. **对象池化集合** — `PooledList`/`PooledStack` 减少 GC 压力

