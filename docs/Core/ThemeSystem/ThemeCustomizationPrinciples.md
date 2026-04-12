# AtomUI 主题自定义 — 原理说明

> 本文档详细阐述 AtomUI 主题自定义的内部实现原理，包括主题加载机制、局部主题作用域（`ThemeConfigProvider`）的实现，以及针对特定组件的主题覆盖和 Token 传递路径。本文是 [Overview.md](Overview.md) 和 [TokenArchitecture.md](TokenArchitecture.md) 的延伸，聚焦于"自定义"能力的底层机制。

---

## 1. 设计背景：Ant Design 5.0 主题定制体系

Ant Design 5.0 的主题定制核心能力：

1. **全局 Token 修改** — 通过 `ConfigProvider` 的 `theme.token` 修改 Seed/Map/Alias Token，级联影响所有组件。
2. **组件级 Token 修改** — 通过 `theme.components[Component]` 修改某个组件的 Component Token 或覆盖该组件消费的 Shared Token。
3. **算法切换** — 通过 `theme.algorithm` 切换 Default/Dark/Compact 算法。
4. **局部主题（嵌套主题）** — 嵌套 `ConfigProvider` 实现页面局部的主题更换，未改变的 Token 继承父主题。
5. **组件级算法开关** — 组件 Token 配置中可选开启 `algorithm`，控制覆盖的 Shared Token 是否经过算法重新派生。

AtomUI 在 .NET / Avalonia 平台完整还原了上述所有能力。

| Ant Design (React) | AtomUI (C# / Avalonia) |
|---|---|
| `ConfigProvider` 的 `theme` prop | `ThemeConfigProvider` XAML 控件 |
| `theme.token` | `ThemeConfigProvider.SharedTokenSetters` |
| `theme.algorithm` | `ThemeConfigProvider.Algorithms` |
| `theme.components[X]` | `ThemeConfigProvider.ControlTokenInfoSetters` |
| 全局 `ConfigProvider` | 主题 XML 定义文件 (`DaybreakBlue.xml`) + `ThemeManager` |
| 嵌套 `ConfigProvider` | 嵌套 `ThemeConfigProvider` |

---

## 2. 全局主题加载机制

### 2.1 主题定义文件

AtomUI 的全局主题通过 XML 定义文件描述。内置主题存放在 `AtomUI.Core/Assets/Themes/` 目录下：

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Theme Name="Daybreak Blue" IsDefault="true">
    <SharedTokens>
        <!-- 全局 Shared Token 覆盖，如：-->
        <!-- <Token Name="ColorPrimary" Value="#1677ff" /> -->
    </SharedTokens>
    <ControlTokens>
        <!-- 组件级 Token 覆盖，如：-->
        <!-- <ControlToken Id="Button" EnableAlgorithm="true">
                 <Token Name="ColorPrimary" Value="#00b96b" IsShared="true" />
                 <Token Name="PaddingInline" Value="20" />
             </ControlToken> -->
    </ControlTokens>
</Theme>
```

**XML 结构说明：**

| 元素/属性 | 说明 |
|---|---|
| `<Theme Name="..." IsDefault="...">` | 主题根节点，`Name` 为显示名称，`IsDefault` 标记是否为默认主题 |
| `<Algorithms>` | 算法声明（可选），逗号分隔，如 `Default,Dark` |
| `<SharedTokens>` | 全局 Shared Token 覆盖区域 |
| `<Token Name="..." Value="..." />` | 单个 Token 设置，`Name` 为 Token 属性名，`Value` 为字符串值 |
| `<ControlTokens>` | 组件级 Token 覆盖区域 |
| `<ControlToken Id="..." EnableAlgorithm="...">` | 针对特定组件的 Token 配置块，`Id` 为组件 Token ID |
| `<Token ... IsShared="true" />` | 在 `ControlToken` 内，`IsShared="true"` 表示这是覆盖组件消费的 Shared Token |

### 2.2 主题扫描与发现

`ThemeManager.ScanThemes()` 按以下优先级顺序扫描主题定义文件：

```
1. 用户自定义目录（CustomThemeDirs）       ← 最高优先级
2. 内置主题目录（BuiltInThemeDirs）        ← 应用数据目录下的 Themes/
3. ThemeAssetPathProviders（注册的提供者）  ← 控件包提供的主题路径
4. AtomUI.Core/Assets/Themes/              ← 框架内置默认主题（最低优先级）
```

对于每个发现的主题定义文件，系统自动创建**四种算法组合**的 `Theme` 实例：

```
DaybreakBlue.xml → Theme("DaybreakBlue", {Default})
                 → Theme("DaybreakBlue", {Default, Dark})
                 → Theme("DaybreakBlue", {Default, Dark, Compact})
                 → Theme("DaybreakBlue", {Default, Compact})
```

每种组合生成唯一的 `ThemeVariant`（如 `DaybreakBlue`、`DaybreakBlue-Dark`、`DaybreakBlue-Dark-Compact`），存入 `ThemePool`。

### 2.3 主题加载流程

当一个 `Theme` 被激活（`SetActiveTheme`）时，如果尚未加载，会触发完整的加载流程：

```
Theme.Load()
│
├── 1. NotifyLoadThemeDef()
│   └── ThemeDefinitionReader.Load(ThemeDefinition)
│       ├── 解析 XML → 填充 ThemeDefinition.SharedTokens
│       ├── 解析 XML → 填充 ThemeDefinition.ControlTokens
│       └── 解析 XML → 填充 ThemeDefinition.Algorithms
│
├── 2. BuildThemeResource(algorithms)
│   ├── 2a. 创建算法链
│   │   ├── DefaultThemeVariantCalculator
│   │   ├── (可选) DarkThemeVariantCalculator(base)
│   │   └── (可选) CompactThemeVariantCalculator(base)
│   │
│   ├── 2b. Shared Token 推导
│   │   ├── _sharedToken.LoadConfig(seedTokens)     ← 加载 Seed 覆盖
│   │   ├── calculator.Calculate(_sharedToken)       ← 算法推导 Map Token
│   │   ├── _sharedToken.LoadConfig(mapTokens)       ← 加载 Map 覆盖
│   │   ├── _sharedToken.CalculateAliasTokenValues() ← 推导 Alias Token
│   │   ├── _sharedToken.LoadConfig(aliasTokens)     ← 加载 Alias 覆盖
│   │   └── _sharedToken.BuildResourceDictionary()   ← 写入 ResourceDictionary
│   │
│   ├── 2c. Control Token 收集与计算
│   │   ├── CollectControlTokens()                    ← 实例化所有已注册的 ControlDesignToken
│   │   ├── controlToken.AssignSharedToken()          ← 注入全局 SharedToken
│   │   ├── (若有覆盖) Clone SharedToken → 应用组件级覆盖
│   │   │   ├── EnableAlgorithm=true  → 重新走算法推导
│   │   │   └── EnableAlgorithm=false → 直接覆盖值（不派生）
│   │   ├── controlToken.CalculateTokenValues()       ← 组件自身 Token 计算
│   │   ├── controlToken.LoadConfig(customTokens)     ← 加载组件自身 Token 覆盖
│   │   └── controlToken.BuildResourceDictionary()    ← 写入 ResourceDictionary
│   │
│   └── 2d. SharedResourceDelta 构建
│       └── 对有自定义配置的 ControlToken，计算其 SharedToken 与全局 SharedToken 的差异
│
└── 3. ResourceDictionary 挂载到 ThemeManager.Resources.ThemeDictionaries
```

### 2.4 Token 覆盖的三层优先级

在加载过程中，Token 值的确定遵循以下优先级（从高到低）：

```
用户 XML/AXAML 中显式设置的值
        ↓ 覆盖
算法计算得到的派生值
        ↓ 覆盖
DesignToken 类中定义的代码默认值
```

具体而言，对于 Shared Token：

1. `DesignToken` 类中 `InitSeedTokenValues()` 设置代码默认值。
2. `LoadConfig(seedTokens)` 加载用户配置的 Seed Token 覆盖。
3. `calculator.Calculate()` 从 Seed 推导 Map Token（覆盖代码默认的 Map 值）。
4. `LoadConfig(mapTokens)` 加载用户配置的 Map Token 覆盖（覆盖算法推导值）。
5. `CalculateAliasTokenValues()` 从 Map 推导 Alias Token。
6. `LoadConfig(aliasTokens)` 加载用户配置的 Alias Token 覆盖。

这确保了用户在任何层级的显式覆盖都能生效，同时未覆盖的 Token 仍由算法自动派生。

---

## 3. 局部主题作用域：ThemeConfigProvider

### 3.1 核心设计

`ThemeConfigProvider` 是 AtomUI 对 Ant Design `ConfigProvider` 嵌套主题能力的实现。它是一个 Avalonia `Control`，可以在 AXAML 中作为容器使用，为其子控件树建立**独立的 Token 计算作用域**。

```csharp
public class ThemeConfigProvider : Control, IThemeConfigProvider
{
    // 子内容
    public Control? Content { get; set; }
    
    // 算法列表（如 "Default", "Dark"）
    public List<string> Algorithms { get; set; }
    
    // 全局 Shared Token 覆盖
    public List<TokenSetter> SharedTokenSetters { get; set; }
    
    // 组件级 Token 覆盖
    public List<ControlTokenInfoSetter> ControlTokenInfoSetters { get; set; }
    
    // 计算后的 Token 实例
    public DesignToken SharedToken { get; }
    public Dictionary<string, IControlDesignToken> ControlTokens { get; }
}
```

### 3.2 Token 计算流程

当 `ThemeConfigProvider` 首次挂载到可视树（`OnAttachedToVisualTree`）时，触发 `CalculateTokenResources()`：

```
ThemeConfigProvider.CalculateTokenResources()
│
├── 1. 解析 Algorithms → 创建算法链
│
├── 2. 分类 SharedTokenSetters
│   ├── seedTokenKeys → Seed 覆盖
│   ├── mapTokenKeys  → Map 覆盖
│   └── aliasTokenKeys → Alias 覆盖
│
├── 3. Shared Token 推导（与全局主题完全相同的流程）
│   ├── _sharedToken.LoadConfig(seed)
│   ├── calculator.Calculate(_sharedToken)
│   ├── _sharedToken.LoadConfig(map)
│   ├── _sharedToken.CalculateAliasTokenValues()
│   ├── _sharedToken.LoadConfig(alias)
│   └── _sharedToken.BuildResourceDictionary(localDict)
│
├── 4. Control Token 计算
│   ├── CollectControlTokens() → 实例化所有 ControlDesignToken
│   ├── 对每个 ControlTokenInfoSetter：
│   │   ├── Clone SharedToken
│   │   ├── EnableAlgorithm=true → 重新走算法推导
│   │   ├── EnableAlgorithm=false → 直接覆盖
│   │   └── AssignSharedToken(copiedSharedToken)
│   ├── controlToken.CalculateTokenValues()
│   ├── controlToken.LoadConfig(customTokens)
│   └── controlToken.BuildResourceDictionary(localDict)
│
└── 5. Resources.MergedDictionaries.Add(localDict) ← 挂载到自身的资源字典
```

### 3.3 资源解析机制

`ThemeConfigProvider` 将计算好的 `ResourceDictionary` 添加到自身的 `Resources.MergedDictionaries` 中。由于 Avalonia 的**资源就近查找**机制，子控件在解析 `{DynamicResource}` 时会**先查找最近的祖先资源字典**，再向上查找直到 `Application` 级别。

```
Window (全局主题 ResourceDictionary)
│
├── StackPanel
│   └── Button → 使用全局 ColorPrimary = #1677ff
│
├── ThemeConfigProvider (局部 ResourceDictionary: ColorPrimary = #00b96b)
│   └── Button → 使用局部 ColorPrimary = #00b96b ← 就近查找
│
└── ThemeConfigProvider (局部 ResourceDictionary: ColorPrimary = red)
    ├── Button → 使用局部 ColorPrimary = red
    └── ThemeConfigProvider (嵌套, ColorPrimary = #7D3C98)
        └── Button → 使用嵌套 ColorPrimary = #7D3C98
```

**关键点**：`ThemeConfigProvider` 的 Token 计算是**完全独立**的 — 它从零创建 `DesignToken` 实例，走完整的算法推导流程，不依赖父级 `ThemeConfigProvider` 或全局主题的计算结果。这确保了每个作用域的 Token 值都是自洽的。

### 3.4 嵌套主题

`ThemeConfigProvider` 支持任意层级的嵌套：

```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.SharedTokenSetters>
        <atom:TokenSetter Key="ColorPrimary" Value="#1677ff" />
    </atom:ThemeConfigProvider.SharedTokenSetters>
    <StackPanel>
        <atom:Button ButtonType="Primary">Theme 1</atom:Button>
        
        <atom:ThemeConfigProvider>
            <atom:ThemeConfigProvider.SharedTokenSetters>
                <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
            </atom:ThemeConfigProvider.SharedTokenSetters>
            <atom:Button ButtonType="Primary">Theme 2</atom:Button>
        </atom:ThemeConfigProvider>
    </StackPanel>
</atom:ThemeConfigProvider>
```

在上例中，"Theme 1" 按钮使用 `#1677ff` 主色，"Theme 2" 按钮使用 `#00b96b` 主色。

> **与 Ant Design 的区别**：Ant Design 的嵌套 `ConfigProvider` 默认继承父级未修改的 Token。AtomUI 的 `ThemeConfigProvider` 则是独立计算的 — 未设置的 Token 使用 `DesignToken` 的代码默认值而非父级值。这在大多数场景下效果相同，因为默认值即是 Ant Design 的标准设计值。

---

## 4. 针对特定组件的主题覆盖

### 4.1 组件级 Token 覆盖的两种路径

AtomUI 支持两种覆盖特定组件主题的路径：

**路径 A：覆盖组件自身 Token（Component Token）**

修改组件 `AbstractControlDesignToken` 子类中定义的专属属性，如 Button 的 `PaddingInline`、`FontWeight` 等。

**路径 B：覆盖组件消费的 Shared Token**

为某个组件建立一份**独立的 SharedToken 副本**，修改其中的值（如 `ColorPrimary`），使该组件的所有派生值都基于修改后的 SharedToken 计算。

### 4.2 ControlTokenInfoSetter 机制

在 AXAML 中，通过 `ControlTokenInfoSetters` 配置组件级覆盖：

```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.ControlTokenInfoSetters>
        <atom:ControlTokenInfoSetter TokenId="Button" EnableAlgorithm="True">
            <!-- 覆盖 Button 消费的 Shared Token（路径 B） -->
            <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
            
            <!-- 覆盖 Button 自身 Token（路径 A） -->
            <atom:ControlTokenSetter Key="PaddingInline" Value="20" />
        </atom:ControlTokenInfoSetter>
    </atom:ThemeConfigProvider.ControlTokenInfoSetters>
    <atom:Button ButtonType="Primary">Custom Button</atom:Button>
</atom:ThemeConfigProvider>
```

**`TokenSetter` vs `ControlTokenSetter` 的区别：**

| 类型 | 目标 | 说明 |
|------|------|------|
| `TokenSetter` | Shared Token (Seed/Map/Alias) | 覆盖组件消费的全局 Token，放入 `ControlTokenConfigInfo.SharedTokens` |
| `ControlTokenSetter` | Component Token | 覆盖组件自身专属 Token，放入 `ControlTokenConfigInfo.Tokens` |

### 4.3 EnableAlgorithm 开关

`ControlTokenInfoSetter` 的 `EnableAlgorithm` 属性控制 Shared Token 覆盖是否经过算法重新派生：

**`EnableAlgorithm = true`（启用算法）：**

```
1. Clone 全局 SharedToken → copiedSharedToken
2. copiedSharedToken.LoadConfig(seed覆盖)    ← 如 ColorPrimary = #00b96b
3. calculator.Calculate(copiedSharedToken)   ← 从修改后的 Seed 重新推导 Map
4. copiedSharedToken.LoadConfig(map覆盖)     ← 加载 Map 覆盖
5. copiedSharedToken.CalculateAliasTokenValues() ← 重新推导 Alias
6. copiedSharedToken.LoadConfig(alias覆盖)   ← 加载 Alias 覆盖
7. controlToken.AssignSharedToken(copiedSharedToken)
```

这意味着修改 `ColorPrimary` 为 `#00b96b` 后，该组件的 `ColorPrimaryHover`、`ColorPrimaryBg` 等梯度色都会自动重新计算。

**`EnableAlgorithm = false`（禁用算法，默认值）：**

```
1. Clone 全局 SharedToken → copiedSharedToken
2. copiedSharedToken.LoadConfig(seed覆盖)    ← 直接覆盖值
3. copiedSharedToken.LoadConfig(map覆盖)     ← 直接覆盖值
4. copiedSharedToken.LoadConfig(alias覆盖)   ← 直接覆盖值
5. controlToken.AssignSharedToken(copiedSharedToken)
```

此模式下，只有被显式覆盖的 Token 会变化，其他派生 Token 保持全局值不变。这对应 Ant Design 中组件级 `algorithm` 为 `false` 时的行为。

### 4.4 SharedResourceDelta 与控件级资源注入

当组件的 SharedToken 被自定义覆盖后，需要一种机制让**该组件实例**能看到修改后的 Shared Token 值。AtomUI 通过 `SharedResourceDelta`（差异资源字典）实现：

```
AbstractControlDesignToken.BuildSharedResourceDeltaDictionary(globalSharedToken)
│
├── 比较 controlToken.SharedToken vs globalSharedToken
├── 找出所有不同的属性
└── 将差异写入 _sharedResourceDeltaDictionary
    例：SharedTokenKind.ColorPrimary → #00b96b（局部值，全局为 #1677ff）
```

当控件实例挂载到逻辑树时（通过 `ControlTokenResourceScopeHost` 机制）：

```
控件.AttachedToLogicalTree
│
├── 获取 TokenResourceScopeProvider（控件注册时设置）
├── TokenFinderUtils.FindControlToken() → 沿逻辑树向上查找
│   ├── 遇到 ThemeConfigProvider？→ 返回其 ControlToken
│   └── 到达根节点？→ 返回全局主题的 ControlToken
├── 获取 ControlToken.GetSharedResourceDeltaDictionary()
└── 如果有差异 → 创建 ResourceDictionary 注入到控件的 Resources.MergedDictionaries
```

这使得组件级的 Shared Token 覆盖只影响目标组件实例，不影响其他组件。

### 4.5 Token 查找路径（TokenFinderUtils）

`TokenFinderUtils` 提供了沿控件树向上查找 Token 的工具方法：

```csharp
internal static class TokenFinderUtils
{
    // 查找 SharedToken：沿 StylingParent 链向上找 IThemeConfigProvider
    public static DesignToken FindSharedToken(Control control);
    
    // 查找 ControlToken：沿 StylingParent 链向上找 IThemeConfigProvider
    public static IControlDesignToken? FindControlToken(Control control, string tokenId, string? catalog);
    
    // 查找 ThemeVariant
    public static ThemeVariant? FindThemeVariant(Control control);
}
```

查找逻辑：

```
控件实例
  │
  ├── 检查父级是否是 IThemeConfigProvider？
  │   ├── 是 → 返回其 Token
  │   └── 否 → 继续向上
  │
  ├── 检查祖父级是否是 IThemeConfigProvider？
  │   ├── 是 → 返回其 Token
  │   └── 否 → 继续向上
  │
  └── ... 直到根节点
      └── 从 ThemeManager.ActivatedTheme 获取全局 Token
```

---

## 5. 动态响应机制

### 5.1 属性变更触发重新计算

`ThemeConfigProvider` 监听其 `SharedTokenSetters`、`ControlTokenInfoSetters`、`Algorithms` 属性的变化：

```csharp
protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
{
    if (VisualRoot != null)
    {
        if (change.Property == ControlTokenInfoSettersProperty ||
            change.Property == SharedTokenSettersProperty ||
            change.Property == AlgorithmsProperty)
        {
            CalculateTokenResources();
        }
    }
}
```

当这些属性在运行时变化时，会重新执行完整的 Token 计算流程并更新 `ResourceDictionary`。由于控件主题通过 `DynamicResource` 绑定 Token 值，UI 会自动刷新。

### 5.2 全局主题切换

全局主题切换通过 `ThemeManager` 的 `ThemeVariant` 属性驱动，利用 Avalonia 的 `ThemeDictionaries` 机制实现无缝切换（详见 [ThemeManagement.md](ThemeManagement.md)）。

---

## 6. 架构总图

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          全局主题层                                       │
│                                                                         │
│  XML 定义文件 (DaybreakBlue.xml)                                         │
│       │                                                                 │
│       ▼                                                                 │
│  ThemeDefinitionReader → ThemeDefinition                                │
│       │                                                                 │
│       ▼                                                                 │
│  Theme.BuildThemeResource()                                             │
│  ┌────────────────────────────────────────────────────────────────┐     │
│  │ 1. DesignToken (Seed → Algorithm → Map → Alias)                │     │
│  │ 2. ControlDesignToken × N (AssignSharedToken + Calculate)      │     │
│  │ 3. ResourceDictionary (SharedTokenKind + ControlTokenKind)     │     │
│  └───────────────────────────┬────────────────────────────────────┘     │
│                               │                                         │
│  ThemeManager.Resources.ThemeDictionaries[ThemeVariant] = ResourceDict  │
└───────────────────────────────┼─────────────────────────────────────────┘
                                │
                                │ Avalonia 资源解析链
                                ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                       局部主题层 (可选, 可嵌套)                            │
│                                                                         │
│  ThemeConfigProvider                                                    │
│  ┌────────────────────────────────────────────────────────────────┐     │
│  │ Algorithms: ["Dark"]                                           │     │
│  │ SharedTokenSetters: [ColorPrimary=#00b96b, ...]                │     │
│  │ ControlTokenInfoSetters: [Button{ColorPrimary=#eb2f96}, ...]   │     │
│  │                                                                │     │
│  │ → 独立计算 DesignToken + ControlDesignToken                    │     │
│  │ → 生成局部 ResourceDictionary                                   │     │
│  │ → 挂载到 self.Resources.MergedDictionaries                     │     │
│  └───────────────────────────┬────────────────────────────────────┘     │
│                               │                                         │
│  子控件通过资源就近查找消费局部 Token                                      │
└───────────────────────────────┼─────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                       控件级作用域 (按需)                                  │
│                                                                         │
│  ControlTokenResourceScopeHost                                          │
│  ┌────────────────────────────────────────────────────────────────┐     │
│  │ 控件挂载时：                                                     │     │
│  │ 1. TokenFinderUtils.FindControlToken() 向上查找最近的 Provider  │     │
│  │ 2. 获取 SharedResourceDelta（覆盖 vs 全局的差异）               │     │
│  │ 3. 将差异注入到控件自身的 Resources.MergedDictionaries          │     │
│  │                                                                │     │
│  │ 效果：同一 ThemeConfigProvider 下，                              │     │
│  │       Button 看到 ColorPrimary=#eb2f96                         │     │
│  │       LineEdit 看到 ColorPrimary=#00b96b                       │     │
│  └────────────────────────────────────────────────────────────────┘     │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 7. 源文件索引

| 文件 | 职责 |
|------|------|
| `Theme.cs` | 全局主题实现：加载定义文件、构建资源、Token 推导 |
| `ThemeDefinition.cs` | 主题定义数据模型 |
| `ThemeDefinitionReader.cs` | XML 主题定义文件解析器 |
| `ThemeConfigProvider.cs` | 局部主题配置控件（对应 Ant Design ConfigProvider） |
| `IThemeConfigProvider.cs` | 局部主题配置接口 |
| `TokenSetter.cs` | Token 设置器（AXAML 中的 `<atom:TokenSetter>`） |
| `ControlTokenInfoSetter.cs` | 组件级 Token 配置设置器 |
| `ControlTokenResourcesHost.cs` | 控件级 Token 资源作用域注入机制 |
| `IControlTokenResourceScopeProvider.cs` | 控件级 Token 作用域提供者接口 |
| `TokenFinderUtils.cs` | Token 查找工具（沿控件树向上查找） |
| `AbstractControlDesignToken.cs` | 组件 Token 抽象基类（含 SharedResourceDelta 构建） |
| `AbstractDesignToken.cs` | Design Token 抽象基类（LoadConfig、BuildResourceDictionary） |
| `ControlTokenConfigInfo.cs` | 组件 Token 配置信息数据模型 |
| `CalculatorUtils.cs` | Token 推导算法工具类 |

