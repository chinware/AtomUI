# AtomUI Control Token 设计规范

本文档定义 `AtomUI.Controls`、`AtomUI.Desktop.Controls`、DataGrid、ColorPicker 和第三方 AtomUI Control 包的
Control Token、ControlTheme Asset 与组合主题规则。完整编译、snapshot 和资源发布模型见
[AtomUI 主题系统架构](../modules/core/theme-system.md)。单个 Control 的 `token.md` 只记录 Own Token 语义和该
Control 支持覆盖的 Global Token，不重复本文的通用机制。面向主题作者的完整定制流程见
[主题系统架构与主题定制指南](../modules/core/theme-architecture-and-customization.md)。

## Token 分层

```text
Global Token
    -> Control-specific Global Override
Effective Global Token
    -> calculate Control Own Token defaults
    -> Control Own Token Override
Effective Control Token
    -> ControlTheme
    -> Semantic Part Theme
    -> Template / Runtime visual state
```

| 层 | 职责 |
| --- | --- |
| Global Token | 定义全局颜色、尺寸、排版、动效、交互值和算法输入。 |
| Effective Global Token | 保存 Global Token 在某个 Control identity 下的有效值。 |
| Control Own Token | 定义只属于一个 Control 的稳定设计语义。 |
| Effective Control Token | 合并该 Control 的 Effective Global Token 与 Own Token。 |
| Theme Variables | 保存 Control 实例在当前状态下的最终视觉变量。 |
| ControlTheme / Template | 消费 Token 和 Theme Variables，完成状态映射与渲染。 |

## Control identity

每个对外可主题化的 Control 都拥有独立 `ControlTokenIdentity`。identity 不由以下信息推断：

- `ControlTheme.TargetType`。
- Control CLR 继承关系。
- ControlTheme `BasedOn` 关系。
- internal Part、Presenter、Decorator 或 Host 类型。
- 运行时资源位置或 Visual/Logical parent。

生成器按 Control、可选 Token 类型和主题资产的命名及目录约定生成 `XxxTokens.Identity`。C# 配置和 imperative
查询使用该强类型入口，不能手写 Catalog/Id 字符串或 `new ControlTokenIdentity(...)`。

## Own Token 定义

```csharp
public sealed class Rating : TemplatedControl
{
}

[ControlDesignToken]
internal sealed class RatingToken : AbstractControlDesignToken
{
    public double StarSize { get; set; }
    public double StarGap { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        StarSize = EffectiveGlobalToken.ControlHeight;
        StarGap  = EffectiveGlobalToken.UniformlyMarginXS;
    }
}
```

约束：

- `RatingToken.cs` 只在 Rating 存在 Own Token 时创建。
- Own Token 类型使用无参数 `[ControlDesignToken]` 标记，供生成器稳定发现。
- `[ControlDesignToken]` 不携带 Control 类型、identity、ID 或主题资产路径；Control 关联继续由命名和目录约定建立。
- Token 类型只继承统一基础类型，禁止继承另一个 Control Token。
- Own Token 禁止与任一 Global Token 同名。
- 禁止泛型 `[ControlDesignToken<TControl>]`、手写 ID 或 Theme Asset glob。
- Control 没有 Own Token 时仍然拥有 identity，并可通过主题依赖获得 Supported Global Token。

多个 Control 需要相同值时，先判断它是否属于全局设计语言。属于时提升为 Global Token；不属于时分别定义语义
清晰的 Own Token。不能通过 Control Token 继承共享实现，纯计算复用可以使用无 identity 的 internal helper。

## 可配置 Token 契约

每个 `ControlTokenDescriptor` 包含两个互斥集合：

```text
SupportedGlobalTokens
OwnTokens
```

Control 配置允许使用的 Token 为：

```text
Configurable Control Tokens = SupportedGlobalTokens + OwnTokens
```

不在这两个集合中的 Global Token 即使全局存在，也不能配置在该 Control 的 `Tokens` 下。这样配置列表、IDE、
Gallery 和运行时校验展示的每个 Token 都会真实影响当前注册的 Token 计算或主题资产。

## Supported Global Token 发现

开发者不维护 Global Token 白名单。生成器从以下来源自动产生依赖：

```csharp
StarSize = EffectiveGlobalToken.ControlHeight;
```

该访问把 `ControlHeight` 加入 Rating 的计算依赖。

```xml
<Setter Property="Foreground" Value="{atom:RatingTokenResource ColorPrimary}" />
```

该引用把 `ColorPrimary` 加入 Rating 的主题依赖。`SharedTokenResource ColorPrimary` 不产生 Rating 依赖。

当前 registry revision 的最终集合为：

```text
SupportedGlobalTokens
    = Control Own Token calculation dependencies
    + built-in ControlTheme dependencies
    + generated third-party theme dependency manifests
    + generated application theme dependency manifests
```

生成器必须静态分析 `EffectiveGlobalToken.X` 和 `XxxTokenResource X`。字符串、反射或无法证明归属的不透明间接
访问不允许进入正常路径。所有依赖 manifest 在 ThemeManager 构建和 ThemeConfig 解析前注册，随后 schema 冻结。

## 用户配置

```csharp
theme.ControlTokens
    .For(RatingTokens.Identity)
    .Set(GlobalTokens.ControlHeight, 36)
    .Set(GlobalTokens.ColorPrimary, Colors.Red)
    .Set(RatingTokens.StarGap, 8);
```

前两个值只覆盖 Rating 的 Effective Global Token，最后一个值覆盖 Rating Own Token。其他 Control 和真正的
Global Token snapshot 不受影响。配置未出现在 `SupportedGlobalTokens` 或 `OwnTokens` 时必须产生明确错误，不能
接受后静默无效。

## AXAML 访问契约

```xml
<!-- 永远读取全局值。 -->
<Setter Property="FontFamily" Value="{atom:SharedTokenResource FontFamily}" />

<!-- 读取 Rating 的 Effective Global Token。 -->
<Setter Property="MinHeight" Value="{atom:RatingTokenResource ControlHeight}" />
<Setter Property="Foreground" Value="{atom:RatingTokenResource ColorPrimary}" />

<!-- 读取 Rating Own Token。 -->
<Setter Property="Margin" Value="{atom:RatingTokenResource StarGap}" />
```

`RatingTokenResource` 同时读取两类 Token，消费语法不写 `Global=` 或 `Own=`。生成器产生强类型
`RatingTokenKey` 和 MarkupExtension 构造参数。主题作者可以选择 Rating Own Token 和 Global Token schema 中的
候选；引用新的 Global Token 会把它加入 Rating 的主题依赖。IDE 优先显示 Own Token 和已经支持的 Global Token，
并标注来源文档；错误名称在 AXAML 编译期失败，运行时不解析字符串。

选择规则：

- 希望某值永远跟随当前 ThemeContext 的全局结果时使用 `SharedTokenResource`。
- 希望用户能够在 Rating 配置下局部覆盖时使用 `RatingTokenResource`。
- ControlTheme 不声明 `ControlTokenScope.Identity`，也不存在 ambient fallback。

## ControlTheme Asset

标准 Control 的最小结构：

```text
Rating/
+-- Rating.cs
+-- RatingToken.cs            optional
\-- Themes/
    \-- RatingTheme.axaml
```

多个主题资产直接增加文件：

```text
Themes/
+-- DatePickerTheme.axaml
+-- DatePickerPresenterTheme.axaml
\-- RangeDatePickerTheme.axaml
```

构建集成把 `Themes/**/*.axaml` 提供给生成器。生成器产生 asset manifest、owner identity、Token 依赖和注册代码。
开发者不编写 `ControlThemeAssets` Attribute、Theme Module、手工 manifest、逐主题 identity 或只用于聚合的额外
AXAML。只有一个 ControlTheme 时就只有一个主题文件。

## Semantic Part Theme

稳定且允许用户替换的内部位置通过强类型 `ControlTheme?` 属性开放：

```csharp
public ControlTheme? SearchButtonTheme { get; set; }
```

Semantic Part Theme 不创建 Token identity，不使用字符串 Part 名称或 `Dictionary<string, ControlTheme>`。Part 是
真实 public Control 时继续使用自己的 identity；Part Theme 可以显式读取 owner 和 Part 的 TokenResource。

SearchEdit 模板直接组合 public Button：

```text
SearchEditTokenResource -> 输入区域、拼接边框、圆角、状态投射和搜索语义
ButtonTokenResource     -> Button 基础尺寸、字体、颜色和交互视觉
SearchButtonTheme       -> 稳定的搜索按钮定制入口
```

不能创建一个 TargetType 属于 SearchButton、基础视觉属于 Button、Token scope 又属于 LineEdit 的 internal
Control。组合关系不能通过借用 Token identity 表达。

## Theme Variables 与命名

Own Token 表达稳定 Control 语义，例如 `DefaultHoverColor`、`PrimaryShadow`、`ContentFontSizeLG`。不要使用
`Panel1Padding`、`FrameBorderPointerOverBrush` 等模板节点名称，也不要展开颜色、variant 和状态的笛卡尔积。

`IsPressed`、`IsPointerOver`、`IsLoading`、模板节点可见性和实例 API 解析结果属于运行时状态。需要通过 Selector
参与状态映射的最终变量使用 internal StyledProperty Theme Variables，不进入 Control Token。

## 第三方 Control 包

第三方按相同约定提供 Control、可选 Own Token 和主题资产，并只调用一次生成的包级注册入口：

```csharp
builder.UseAcmeControls();
```

该入口直接注册 descriptor、Token schema、依赖 manifest 和主题资产，不运行时扫描程序集或 AXAML。第三方包不
需要每 Theme 注册代码、泛型 Token Attribute、glob Attribute 或反射 fallback。

## 构建期诊断

以下情况必须构建失败：

- Own Token 与 Global Token 同名。
- Control Token 继承另一个 Control Token。
- TokenResource 引用了不存在或归属错误的 Token。
- Control 配置覆盖未支持的 Global Token。
- Control、Token、主题资产和 identity 约定存在歧义。
- 重复 identity、重复资产 URI 或未注册 manifest。
- Semantic Part Theme 的 TargetType 与属性契约不兼容。
- internal 实现类型被错误声明为独立 Token owner。
- Global Token 依赖使用字符串、反射或不可分析路径。

## 验证要求

| 改动类型 | 验证要求 |
| --- | --- |
| 新增 Control | 验证 identity、可选 Own Token、主题 manifest、包级注册和无反射路径。 |
| 修改 Own Token | 验证强类型 key、默认计算、override 和 AXAML 消费。 |
| 修改 Global Token 依赖 | 验证 `SupportedGlobalTokens`、配置校验和 dependency manifest。 |
| 修改 Semantic Part Theme | 验证 owner/Part Token 分工、TargetType 和默认/实例替换。 |
| 修改生成器 | 验证确定性输出、增量构建、NativeAOT 注册和错误诊断。 |
| 修改主题视觉 | 验证 Light/Dark、Control 算法和相关 Control 家族。 |
