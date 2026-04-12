# AtomUI 主题定制 — 开发者指南

> 本文档面向 **AtomUI 应用开发者**，提供主题定制的完整实践指南：从全局主题配置到局部覆盖，从 Shared Token 修改到组件级 Token 定制。配合 Gallery 示例代码进行说明。如需了解底层原理，请参阅 [ThemeCustomizationPrinciples.md](ThemeCustomizationPrinciples.md)。

---

## 1. 快速开始：主题定制能力概览

AtomUI 提供以下四种主题定制方式，按作用范围从大到小排列：

| 定制方式 | 作用范围 | 配置方式 | 典型场景 |
|---------|---------|---------|---------|
| **全局主题 XML** | 整个应用 | XML 定义文件 | 品牌主色、全局圆角、全局字号 |
| **全局 Shared Token 覆盖** | `ThemeConfigProvider` 子树 | AXAML `SharedTokenSetters` | 页面区域使用暗色主题 |
| **组件级 Token 覆盖** | 特定组件类型 | AXAML `ControlTokenInfoSetters` | 让所有 Button 使用绿色主色 |
| **算法切换** | `ThemeConfigProvider` 子树 | AXAML `Algorithms` | 局部暗色/紧凑模式 |

---

## 2. 修改全局 Shared Token

### 2.1 在 AXAML 中使用 ThemeConfigProvider

最常见的定制方式是通过 `ThemeConfigProvider` 修改 Seed Token。修改一个 Seed Token（如 `ColorPrimary`），所有由它派生的梯度 Token 会自动重新计算。

```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.SharedTokenSetters>
        <!-- Seed Token，影响范围大 -->
        <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
        <atom:TokenSetter Key="BorderRadius" Value="2" />
        <!-- Map/Alias Token，影响范围小 -->
        <atom:TokenSetter Key="ColorBgContainer" Value="#f6ffed" />
    </atom:ThemeConfigProvider.SharedTokenSetters>

    <!-- 此范围内的所有控件都使用修改后的 Token -->
    <WrapPanel>
        <atom:Button ButtonType="Primary">Primary Button</atom:Button>
        <atom:Button>Default Button</atom:Button>
    </WrapPanel>
</atom:ThemeConfigProvider>
```

**效果**：`ThemeConfigProvider` 内的按钮会使用绿色（`#00b96b`）作为主色，圆角为 2px，背景色为浅绿色。`ThemeConfigProvider` 外的控件不受影响。

### 2.2 常用 Seed Token

| Token 名称 | 类型 | 默认值 | 说明 |
|-----------|------|--------|------|
| `ColorPrimary` | `Color` | `#1677ff` | 品牌主色，影响所有主色系梯度 |
| `ColorSuccess` | `Color` | `#52c41a` | 成功色 |
| `ColorWarning` | `Color` | `#faad14` | 警戒色 |
| `ColorError` | `Color` | `#ff4d4f` | 错误色 |
| `ColorInfo` | `Color` | `#1677ff` | 信息色 |
| `FontSize` | `double` | `14` | 基准字号，影响所有字号梯度 |
| `BorderRadius` | `CornerRadius` | `6` | 基础圆角 |
| `SizeUnit` | `double` | `4` | 尺寸变化单位 |
| `SizeStep` | `double` | `4` | 尺寸步长 |
| `ControlHeight` | `double` | `32` | 基础控件高度 |
| `LineWidth` | `double` | `1` | 基础线宽 |
| `EnableMotion` | `bool` | `true` | 是否启用动画 |

> **提示**：完整的 Token 列表请参考 [TokenArchitecture.md](TokenArchitecture.md)。修改 Seed Token 是最高效的定制方式 — 修改一个值即可级联影响数十个派生值。

---

## 3. 使用预设算法

### 3.1 算法切换

通过 `Algorithms` 属性可以切换主题算法，实现暗色或紧凑布局：

```xml
<!-- 暗色主题 -->
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.Algorithms>
        <x:String>Dark</x:String>
    </atom:ThemeConfigProvider.Algorithms>
    <Border Background="#2b2d30" Padding="20">
        <StackPanel Orientation="Horizontal" Spacing="5">
            <atom:LineEdit PlaceholderText="Please input" Width="200" />
            <atom:Button ButtonType="Primary">Submit</atom:Button>
        </StackPanel>
    </Border>
</atom:ThemeConfigProvider>
```

### 3.2 可用算法

| 算法名称 | 说明 |
|---------|------|
| `Default` | 默认亮色算法（始终包含，无需显式声明） |
| `Dark` | 暗色算法 |
| `Compact` | 紧凑算法（更小的尺寸和间距） |

算法可以组合使用。例如同时启用暗色和紧凑：

```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.Algorithms>
        <x:String>Dark</x:String>
        <x:String>Compact</x:String>
    </atom:ThemeConfigProvider.Algorithms>
    <!-- 暗色 + 紧凑布局 -->
</atom:ThemeConfigProvider>
```

### 3.3 算法与 Token 覆盖组合

算法和 Token 覆盖可以同时使用：

```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.Algorithms>
        <x:String>Dark</x:String>
    </atom:ThemeConfigProvider.Algorithms>
    <atom:ThemeConfigProvider.SharedTokenSetters>
        <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
    </atom:ThemeConfigProvider.SharedTokenSetters>
    <!-- 暗色主题 + 绿色主色 -->
</atom:ThemeConfigProvider>
```

---

## 4. 修改组件 Token

### 4.1 组件级 Shared Token 覆盖

通过 `ControlTokenInfoSetters` 可以修改特定组件消费的 Shared Token，不影响其他组件：

```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.ControlTokenInfoSetters>
        <!-- 只修改 Button 的主色 -->
        <atom:ControlTokenInfoSetter TokenId="Button" EnableAlgorithm="True">
            <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
        </atom:ControlTokenInfoSetter>
        <!-- 只修改 AddOnDecoratedBox（输入框装饰器）的主色 -->
        <atom:ControlTokenInfoSetter TokenId="AddOnDecoratedBox" EnableAlgorithm="True">
            <atom:TokenSetter Key="ColorPrimary" Value="#eb2f96" />
        </atom:ControlTokenInfoSetter>
    </atom:ThemeConfigProvider.ControlTokenInfoSetters>
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:LineEdit PlaceholderText="Please input" Width="200" />
        <atom:Button ButtonType="Primary">Submit</atom:Button>
    </StackPanel>
</atom:ThemeConfigProvider>
```

**效果**：Button 使用绿色主色，输入框装饰使用粉色主色，其他控件保持全局默认。

### 4.2 EnableAlgorithm 的作用

`EnableAlgorithm` 控制覆盖的 Shared Token 是否经过算法重新派生：

| `EnableAlgorithm` | 行为 | 适用场景 |
|---|---|---|
| `True` | 修改 `ColorPrimary` 后，`ColorPrimaryHover`、`ColorPrimaryBg` 等梯度色自动重算 | 需要完整色系联动时 |
| `False`（默认） | 只有被显式修改的 Token 值变化，其他保持全局值 | 精确控制单个值时 |

**对比示例（来自 Gallery）：**

```xml
<!-- EnableAlgorithm=True：修改 ColorPrimary 后 Hover/Active 等梯度色自动联动 -->
<atom:ControlTokenInfoSetter TokenId="Button" EnableAlgorithm="True">
    <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
</atom:ControlTokenInfoSetter>

<!-- EnableAlgorithm=False：只有 ColorPrimary 本身变化，Hover 等保持默认 -->
<atom:ControlTokenInfoSetter TokenId="Button">
    <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
</atom:ControlTokenInfoSetter>
```

### 4.3 组件自身 Token（ControlTokenSetter）

要修改组件自身的专属 Token（非 Shared Token），使用 `ControlTokenSetter`：

```xml
<atom:ControlTokenInfoSetter TokenId="Button">
    <!-- 修改组件消费的 Shared Token -->
    <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
    
    <!-- 修改组件自身的专属 Token -->
    <atom:ControlTokenSetter Key="PaddingInline" Value="20" />
</atom:ControlTokenInfoSetter>
```

> **注意**：`TokenSetter` 和 `ControlTokenSetter` 是不同的类型。`TokenSetter` 用于 Shared Token（Seed/Map/Alias），`ControlTokenSetter` 用于组件自身的 Component Token。

### 4.4 常见的 TokenId

`TokenId` 对应组件 Token 类中的 `const string ID`。常见值：

| TokenId | 对应组件 |
|---------|---------|
| `Button` | 按钮 |
| `LineEdit` | 单行输入框 |
| `AddOnDecoratedBox` | 输入框装饰器 |
| `CheckBox` | 复选框 |
| `Radio` | 单选按钮 |
| `Select` | 选择器 |
| `Tag` | 标签 |
| `Alert` | 警告提示 |
| `Badge` | 徽标 |
| `Tabs` | 标签页 |

> 要查看某个组件的 TokenId，请查看对应的 `*Token.cs` 文件中的 `public const string ID` 常量。

---

## 5. 嵌套主题

### 5.1 基本嵌套

`ThemeConfigProvider` 可以任意层级嵌套，每层定义自己的 Token 覆盖：

```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.SharedTokenSetters>
        <atom:TokenSetter Key="ColorPrimary" Value="#1677ff" />
    </atom:ThemeConfigProvider.SharedTokenSetters>
    <Border Padding="20">
        <StackPanel Orientation="Horizontal" Spacing="5">
            <!-- 使用 #1677ff 蓝色 -->
            <atom:Button ButtonType="Primary">Theme 1</atom:Button>
            
            <atom:ThemeConfigProvider>
                <atom:ThemeConfigProvider.SharedTokenSetters>
                    <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
                </atom:ThemeConfigProvider.SharedTokenSetters>
                <!-- 使用 #00b96b 绿色 -->
                <atom:Button ButtonType="Primary">Theme 2</atom:Button>
            </atom:ThemeConfigProvider>
        </StackPanel>
    </Border>
</atom:ThemeConfigProvider>
```

### 5.2 典型场景

- **侧边栏暗色**：主内容区使用亮色，侧边栏嵌套暗色 `ThemeConfigProvider`。
- **卡片高亮**：某张卡片使用不同的品牌色突出显示。
- **表单区域**：表单区域使用紧凑布局，其他区域保持默认。

---

## 6. 全局主题 XML 定义文件

### 6.1 创建自定义主题文件

创建 XML 文件（如 `MyBrand.xml`）：

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Theme Name="My Brand Theme" IsDefault="false">
    <Algorithms>Default</Algorithms>
    <SharedTokens>
        <Token Name="ColorPrimary" Value="#722ed1" />
        <Token Name="ColorSuccess" Value="#13c2c2" />
        <Token Name="BorderRadius" Value="8" />
        <Token Name="FontSize" Value="16" />
    </SharedTokens>
    <ControlTokens>
        <ControlToken Id="Button" EnableAlgorithm="true">
            <Token Name="ColorPrimary" Value="#722ed1" IsShared="true" />
        </ControlToken>
    </ControlTokens>
</Theme>
```

### 6.2 XML Schema 参考

**`<Theme>` 根元素：**

| 属性 | 必填 | 说明 |
|------|------|------|
| `Name` | 是 | 主题显示名称 |
| `IsDefault` | 是 | 是否为默认主题（`true`/`false`） |

**`<Algorithms>` 元素：**
- 内容为逗号分隔的算法名：`Default`、`Dark`、`Compact`。
- 示例：`<Algorithms>Default,Dark</Algorithms>`

**`<SharedTokens>` 子元素 `<Token>`：**

| 属性 | 必填 | 说明 |
|------|------|------|
| `Name` | 是 | Token 属性名（如 `ColorPrimary`） |
| `Value` | 是 | Token 值（字符串格式，自动转换） |

**`<ControlTokens>` → `<ControlToken>` 子元素：**

| 属性 | 必填 | 说明 |
|------|------|------|
| `Id` | 是 | 组件 Token ID（如 `Button`） |
| `EnableAlgorithm` | 否 | 是否启用算法派生（默认 `false`） |

**`<ControlToken>` 内的 `<Token>`：**

| 属性 | 必填 | 说明 |
|------|------|------|
| `Name` | 是 | Token 名称 |
| `Value` | 是 | Token 值 |
| `IsShared` | 否 | `true` 表示覆盖 Shared Token，`false`/省略 表示覆盖组件自身 Token |

### 6.3 注册自定义主题

将主题文件放置在以下位置之一：

1. **应用数据目录**：`{AppData}/{AppName}/Themes/MyBrand.xml` — 自动扫描。
2. **自定义目录**：通过 `ThemeManager.AddCustomThemePaths()` 注册。
3. **嵌入资源**：通过 `IThemeAssetPathProvider` 注册（库开发者模式）。

### 6.4 激活自定义主题

```csharp
this.UseAtomUI(builder =>
{
    builder.UseDesktopControls();
    builder.WithDefaultTheme("MyBrand");  // 使用主题文件名（不含扩展名）
});
```

---

## 7. 关键 API 参考

### 7.1 AXAML 类型与标记扩展

| 类型 | AXAML 标签 | 说明 |
|------|-----------|------|
| `ThemeConfigProvider` | `<atom:ThemeConfigProvider>` | 局部主题配置容器 |
| `TokenSetter` | `<atom:TokenSetter>` | Shared Token 设置器 |
| `ControlTokenSetter` | `<atom:ControlTokenSetter>` | 组件自身 Token 设置器 |
| `ControlTokenInfoSetter` | `<atom:ControlTokenInfoSetter>` | 组件级 Token 配置块 |
| `SharedTokenResourceExtension` | `{atom:SharedTokenResource ...}` | 全局 Token 资源引用 |
| `TokenResourceExtension<T>` | `{atom:ButtonTokenResource ...}` 等 | 组件 Token 资源引用 |

### 7.2 ThemeConfigProvider 属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `Content` | `Control?` | 子内容（`[Content]` 标记，可直接写子控件） |
| `Algorithms` | `List<string>` | 算法列表（`"Default"`, `"Dark"`, `"Compact"`） |
| `SharedTokenSetters` | `List<TokenSetter>` | 全局 Token 覆盖列表 |
| `ControlTokenInfoSetters` | `List<ControlTokenInfoSetter>` | 组件级 Token 覆盖列表 |

### 7.3 TokenSetter 属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `Key` | `string` | Token 属性名（如 `"ColorPrimary"`） |
| `Value` | `string` | Token 值（字符串格式） |

### 7.4 ControlTokenInfoSetter 属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `TokenId` | `string` | 组件 Token ID |
| `EnableAlgorithm` | `bool` | 是否对覆盖的 Shared Token 启用算法派生（默认 `false`） |
| `Setters` | `List<TokenSetter>` | Token 设置器列表（支持 `TokenSetter` 和 `ControlTokenSetter`） |

### 7.5 C# API

```csharp
// IThemeManagerBuilder — 应用启动时配置
public interface IThemeManagerBuilder
{
    void WithDefaultTheme(string themeId);              // 设置默认主题
    void WithDefaultFontFamily(FontFamily fontFamily);  // 覆盖默认字体
    void WithDefaultFontFamily(string fontFamily);
    void WithThemeVariantCalculatorFactory(IThemeVariantCalculatorFactory factory); // 自定义算法工厂
}

// IThemeManager — 运行时访问
public interface IThemeManager
{
    IReadOnlyCollection<ITheme> AvailableThemes { get; }  // 所有可用主题
    ITheme? ActivatedTheme { get; }                        // 当前激活的主题
    bool IsDarkThemeMode { get; set; }                     // 暗色模式开关
    bool IsCompactThemeMode { get; set; }                  // 紧凑模式开关
    bool IsMotionEnabled { get; set; }                     // 动效开关
}

// ITheme — 主题实例
public interface ITheme
{
    DesignToken SharedToken { get; }                        // 全局 SharedToken
    IControlDesignToken? GetControlToken(string tokenId);   // 获取组件 Token
}
```

---

## 8. Gallery 示例解读

Gallery 应用中的 `CustomizeThemeShowCase.axaml` 展示了四个典型的主题定制场景。

### 8.1 示例 1：修改 Design Token

```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.SharedTokenSetters>
        <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
        <atom:TokenSetter Key="BorderRadius" Value="2" />
        <atom:TokenSetter Key="ColorBgContainer" Value="#f6ffed" />
    </atom:ThemeConfigProvider.SharedTokenSetters>
    <WrapPanel>
        <atom:Button ButtonType="Primary">Primary Button</atom:Button>
        <atom:Button>Default Button</atom:Button>
    </WrapPanel>
</atom:ThemeConfigProvider>
```

**要点**：
- `ColorPrimary` 是 Seed Token，修改后所有主色系梯度自动联动。
- `BorderRadius` 是 Seed Token，修改后所有圆角梯度自动联动。
- `ColorBgContainer` 是 Alias Token，只影响容器背景色这一个值。

### 8.2 示例 2：使用预设算法

```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.Algorithms>
        <x:String>Dark</x:String>
    </atom:ThemeConfigProvider.Algorithms>
    <Border Background="#2b2d30" Padding="20">
        <StackPanel Orientation="Horizontal" Spacing="5">
            <atom:LineEdit PlaceholderText="Please input" Width="200" />
            <atom:Button ButtonType="Primary">Submit</atom:Button>
        </StackPanel>
    </Border>
</atom:ThemeConfigProvider>
```

**要点**：
- `Algorithms` 列表中添加 `"Dark"` 即可切换到暗色算法。
- 暗色算法会自动反转色彩体系：浅色背景变深色，深色文字变浅色。
- 外层 `Border` 设置深色背景以匹配暗色主题。

### 8.3 示例 3：定制组件 Token

```xml
<!-- EnableAlgorithm=True：自动派生梯度色 -->
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.ControlTokenInfoSetters>
        <atom:ControlTokenInfoSetter TokenId="Button" EnableAlgorithm="True">
            <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
        </atom:ControlTokenInfoSetter>
        <atom:ControlTokenInfoSetter TokenId="AddOnDecoratedBox" EnableAlgorithm="True">
            <atom:TokenSetter Key="ColorPrimary" Value="#eb2f96" />
        </atom:ControlTokenInfoSetter>
    </atom:ThemeConfigProvider.ControlTokenInfoSetters>
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:LineEdit PlaceholderText="Please input" Width="200" />
        <atom:Button ButtonType="Primary">Submit</atom:Button>
    </StackPanel>
</atom:ThemeConfigProvider>

<!-- EnableAlgorithm=False：直接覆盖，不派生 -->
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.ControlTokenInfoSetters>
        <atom:ControlTokenInfoSetter TokenId="Button">
            <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
        </atom:ControlTokenInfoSetter>
    </atom:ThemeConfigProvider.ControlTokenInfoSetters>
    ...
</atom:ThemeConfigProvider>
```

**要点**：
- 不同组件可以使用不同的主色，互不影响。
- `EnableAlgorithm="True"` 时 Hover/Active 等交互色会自动联动。

### 8.4 示例 4：嵌套主题

```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.SharedTokenSetters>
        <atom:TokenSetter Key="ColorPrimary" Value="#1677ff" />
    </atom:ThemeConfigProvider.SharedTokenSetters>
    <StackPanel Orientation="Horizontal" Spacing="5">
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

**要点**：
- "Theme 1" 按钮使用外层蓝色（`#1677ff`）。
- "Theme 2" 按钮使用内层绿色（`#00b96b`）。
- 嵌套层的 Token 不继承父层 — 未设置的 Token 使用框架默认值。

---

## 9. 运行时主题切换

### 9.1 全局暗色/紧凑模式切换

通过 `IThemeManager` 可以在运行时切换暗色模式和紧凑模式：

```csharp
var themeManager = AvaloniaLocator.Current.GetRequiredService<IThemeManager>();

// 切换暗色模式
themeManager.IsDarkThemeMode = true;

// 切换紧凑模式
themeManager.IsCompactThemeMode = true;

// 组合：暗色 + 紧凑
themeManager.IsDarkThemeMode = true;
themeManager.IsCompactThemeMode = true;
```

### 9.2 切换主题变体

```csharp
// 切换到特定主题变体
Application.Current.RequestedThemeVariant = 
    Theme.BuildThemeVariant("DaybreakBlue", new List<ThemeAlgorithm> 
    { 
        ThemeAlgorithm.Default, 
        ThemeAlgorithm.Dark 
    });
```

### 9.3 禁用动画

```csharp
var themeManager = AvaloniaLocator.Current.GetRequiredService<IThemeManager>();
themeManager.IsMotionEnabled = false;
```

或在 AXAML 中通过 Token 覆盖：

```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.SharedTokenSetters>
        <atom:TokenSetter Key="EnableMotion" Value="false" />
    </atom:ThemeConfigProvider.SharedTokenSetters>
    <!-- 此范围内的控件动画被禁用 -->
</atom:ThemeConfigProvider>
```

---

## 10. 常见问题与注意事项

### 10.1 Token 名称必须精确匹配

`TokenSetter` 的 `Key` 必须与 `DesignToken` 类中的**属性名**完全一致（PascalCase）。常见错误：

```xml
<!-- ❌ 错误：使用了 camelCase -->
<atom:TokenSetter Key="colorPrimary" Value="#00b96b" />

<!-- ✅ 正确：使用 PascalCase -->
<atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
```

### 10.2 值类型自动转换

Token 值在 XML/AXAML 中以字符串形式提供，AtomUI 内置的 `ITokenValueConverter` 会自动转换为目标类型：

| 字符串格式 | 转换为 | 示例 |
|-----------|--------|------|
| `#1677ff` | `Color` | 十六进制颜色 |
| `red`, `blue` | `Color` | 颜色名称 |
| `14` | `double` | 数值 |
| `6` | `CornerRadius` | 圆角（统一四角） |
| `true` / `false` | `bool` | 布尔值 |
| `1` | `double` (LineWidth) | 线宽 |

### 10.3 ThemeConfigProvider 的独立性

每个 `ThemeConfigProvider` 都是**独立计算**的 — 它不继承父级 `ThemeConfigProvider` 的 Token 值。未显式设置的 Token 使用 `DesignToken` 类中的代码默认值（即 Ant Design 5.0 标准值）。

如果你希望只修改某几个 Token 而保持其他不变，直接设置那几个即可 — 其他 Token 会使用标准默认值。

### 10.4 性能考虑

- **`ThemeConfigProvider` 的计算开销**：每个 `ThemeConfigProvider` 实例会创建独立的 `DesignToken` 和所有注册的 `ControlDesignToken` 实例，并走完整的算法推导流程。在页面上大量使用 `ThemeConfigProvider` 可能有性能影响。
- **建议**：尽量在较高层级使用 `ThemeConfigProvider`，避免在列表项等重复元素中使用。
- **组件级覆盖** 相对轻量，因为只 Clone 和重算目标组件的 SharedToken。

### 10.5 TokenSetter vs ControlTokenSetter

在 `ControlTokenInfoSetter` 内部，正确区分两种 Setter：

| Setter 类型 | 用于 | 示例 |
|------------|------|------|
| `<atom:TokenSetter>` | 覆盖组件消费的 Shared Token | `ColorPrimary`, `FontSize`, `BorderRadius` |
| `<atom:ControlTokenSetter>` | 覆盖组件自身的 Component Token | `PaddingInline`, `PrimaryColor`, `DangerColor` |

### 10.6 多主题切换与持久化

AtomUI 的 `ThemePool` 包含所有已发现主题的算法组合变体。切换主题时：

1. 目标主题会按需加载（首次激活时加载）。
2. 已加载的主题会缓存在 `ThemePool` 中。
3. 主题切换通过 Avalonia 的 `ThemeDictionaries` 机制实现，无需重启。

如需持久化用户的主题选择，应用层需要自行保存主题 ID 和算法组合，并在下次启动时通过 `WithDefaultTheme()` 恢复。

---

## 11. 文档索引

| 文档 | 描述 |
|------|------|
| [Overview.md](Overview.md) | 主题系统总览 |
| [TokenArchitecture.md](TokenArchitecture.md) | Design Token 四层架构详解 |
| [ThemeAlgorithm.md](ThemeAlgorithm.md) | 主题算法与调色板生成 |
| [ThemeManagement.md](ThemeManagement.md) | 主题管理器与主题生命周期 |
| [TokenResourceBinding.md](TokenResourceBinding.md) | Token 资源绑定与 Source Generator |
| [ThemeCustomizationPrinciples.md](ThemeCustomizationPrinciples.md) | 主题自定义原理说明（本指南的姐妹篇） |
| [ThemeCustomizationGuide.md](ThemeCustomizationGuide.md) | 本文件 — 主题定制开发者指南 |

