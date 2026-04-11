# AtomUI 组件尺寸系统设计原理与使用规范

## 1. 概述

AtomUI 的组件尺寸系统是对 [Ant Design 5.0 三级尺寸规范](https://ant.design/docs/react/customize-theme#sizetype) 的严格 C# 实现。通过 `ISizeTypeAware` 接口和 `SizeType` 枚举，所有支持尺寸变化的控件共享统一的尺寸语义，并与 Design Token 系统深度集成，实现从全局到组件的尺寸一致性。

### 核心理念

Ant Design 5.0 定义了三个标准尺寸档位（**Large / Middle / Small**），分别对应不同的高度、内边距、字体大小和圆角半径。AtomUI 将这一规范抽象为：

1. **`SizeType` 枚举**（`Large` / `Middle` / `Small`）— 标准三级尺寸
2. **`ISizeTypeAware` 接口** — 标记控件支持标准尺寸切换
3. **`SizeTypeControlProperty` 共享属性** — 提供 `AddOwner` 模式的 `StyledProperty<SizeType>`
4. **Design Token 尺寸梯度** — `ControlHeightSM` / `ControlHeight` / `ControlHeightLG` 等全局 Token 定义每个档位的具体像素值

此外还有**扩展变体** `CustomizableSizeType`（多一个 `Custom` 选项），用于少数需要自定义精确尺寸的控件。

---

## 2. 类型定义

### 2.1 SizeType 枚举

定义在 `AtomUI.Core/Common.cs`：

```csharp
public enum SizeType
{
    Large,
    Middle,
    Small
}
```

这是最常用的尺寸类型，对应 Ant Design 的三级尺寸体系。**绝大多数控件**使用此枚举。

### 2.2 CustomizableSizeType 枚举

```csharp
public enum CustomizableSizeType
{
    Large,
    Middle,
    Small,
    Custom   // 额外的自定义尺寸
}
```

仅用于少数**需要支持精确自定义尺寸**的控件（如 `Avatar`、`Drawer`、`Space`、`Skeleton` 系列），当设置为 `Custom` 时通常配合一个额外的 `Size` / `DialogSize` 属性来指定精确值。

### 2.3 接口定义

定义在 `AtomUI.Controls.Shared/ISizeTypeAware.cs`：

```csharp
/// 标记控件支持标准三级尺寸切换
public interface ISizeTypeAware
{
    public SizeType SizeType { get; set; }
}

/// 标记控件支持扩展尺寸切换（含 Custom）
public interface ICustomizableSizeTypeAware
{
    public CustomizableSizeType SizeType { get; set; }
}
```

### 2.4 共享属性定义

```csharp
/// 标准尺寸共享属性持有者
public abstract class SizeTypeControlProperty
{
    public const string SizeTypePropertyName = "SizeType";
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AvaloniaProperty.Register<StyledElement, SizeType>(SizeTypePropertyName, SizeType.Middle);
}

/// 扩展尺寸共享属性持有者
public abstract class CustomizableSizeTypeControlProperty
{
    public const string SizeTypePropertyName = "SizeType";
    public static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
        AvaloniaProperty.Register<StyledElement, CustomizableSizeType>(SizeTypePropertyName, CustomizableSizeType.Middle);
}
```

> **设计亮点**: 使用抽象持有者类 + `AddOwner` 模式确保所有控件共享同一个属性元数据（属性名 `"SizeType"`），使 Avalonia 的属性继承和绑定系统能够正确工作。

---

## 3. 设计原理与优势

### 3.1 与 Ant Design 5.0 尺寸 Token 体系的对应

`SizeType` 的三个值直接映射到 Design Token 系统的三级尺寸梯度：

| SizeType | ControlHeight Token | Font Token | BorderRadius Token | Padding 级别 |
|---|---|---|---|---|
| `Large` | `ControlHeightLG` (40px) | `FontSizeLG` | `BorderRadiusLG` | `PaddingLG` |
| `Middle` | `ControlHeight` (32px) | `FontSize` | `BorderRadius` | `Padding` |
| `Small` | `ControlHeightSM` (24px) | `FontSize` | `BorderRadiusSM` | `PaddingSM` |

**全部尺寸值来源于 Token，而非硬编码**。当用户自定义主题（如 Compact 模式）时，Token 值自动变化，所有使用 `SizeType` 的控件尺寸自动适配。

### 3.2 统一的属性名实现级联传播

所有实现 `ISizeTypeAware` 的控件都通过 `AddOwner` 共享同一个属性名 `"SizeType"`，这使得：

1. **容器级联**：父容器可以通过绑定一次性将 `SizeType` 传播给所有子控件
2. **Form 级联**：`Form` → `FormItem` → 具体输入控件，三级 SizeType 自动级联
3. **CompactSpace 级联**：`CompactSpace` 自动将 `SizeType` 绑定到所有 `ISizeTypeAware` 子控件
4. **AXAML 属性选择器**：所有控件都可以使用 `^[SizeType=Large]` 等一致的选择器语法

### 3.3 SizeType 级联机制详解

AtomUI 中存在两种重要的 SizeType 级联场景：

#### 场景 A: Form → FormItem → 输入控件

```
Form (SizeType=Large)
  └─ FormItem ←──── 绑定 Form.SizeType
       └─ TextBox ←── 检测 ISizeTypeAware → 绑定 FormItem.SizeType
       └─ Select  ←── 检测 ISizeTypeAware → 绑定 FormItem.SizeType
       └─ Button  ←── 检测 ISizeTypeAware → 绑定 FormItem.SizeType
```

```csharp
// Form.cs — 将 SizeType 绑定到 FormItem
formItem[!FormItem.SizeTypeProperty] = this[!SizeTypeProperty];

// FormItem.cs — 将 SizeType 中继到内容控件
if (Content is ISizeTypeAware)
{
    _disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, Content, SizeTypeProperty));
}
```

用户只需在 `Form` 上设置一次 `SizeType`，表单内所有输入控件的尺寸自动同步。

#### 场景 B: CompactSpace → 子控件

```
CompactSpace (SizeType=Small)
  └─ Button   ←── 检测 ISizeTypeAware → 绑定 CompactSpace.SizeType
  └─ Select   ←── 检测 ISizeTypeAware → 绑定 CompactSpace.SizeType
  └─ TextBox  ←── 检测 ISizeTypeAware → 绑定 CompactSpace.SizeType
```

```csharp
// CompactSpace.cs — 将 SizeType 绑定到所有 ISizeTypeAware 子控件
if (target is ISizeTypeAware)
{
    target[!SizeTypeProperty] = this[!SizeTypeProperty];
}
```

### 3.4 接口作为鸭子类型检测标记

`ISizeTypeAware` 和 `ICustomizableSizeTypeAware` 的核心作用是**运行时类型检测标记**。容器控件（如 `FormItem`、`CompactSpace`）通过 `is ISizeTypeAware` 检查来决定是否将 `SizeType` 传播给子控件：

```csharp
if (Content is ISizeTypeAware)
{
    // 是的，这个子控件支持尺寸切换，传播 SizeType
    BindUtils.RelayBind(this, SizeTypeProperty, Content, SizeTypeProperty);
}
// 否则跳过，不强制传播
```

这意味着**只有显式声明支持尺寸感知的控件**才会参与级联，未实现接口的控件不受影响。

### 3.5 与 AXAML 主题系统的集成

`SizeType` 作为 `StyledProperty`，可以在 AXAML ControlTheme 中通过**属性选择器**驱动样式：

```xml
<!-- 根据 SizeType 值切换不同的 Token 值 -->
<Style Selector="^[SizeType=Large]">
    <Setter Property="FontSize" Value="{atom:TokenResource ContentFontSizeLG}" />
    <Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadiusLG}" />
    <Style Selector="^ /template/ Border#Frame">
        <Setter Property="Height" Value="{atom:SharedTokenResource ControlHeightLG}" />
    </Style>
</Style>

<Style Selector="^[SizeType=Middle]">
    <Setter Property="FontSize" Value="{atom:TokenResource ContentFontSize}" />
    <Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadius}" />
    <Style Selector="^ /template/ Border#Frame">
        <Setter Property="Height" Value="{atom:SharedTokenResource ControlHeight}" />
    </Style>
</Style>

<Style Selector="^[SizeType=Small]">
    <Setter Property="FontSize" Value="{atom:TokenResource ContentFontSizeSM}" />
    <Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadiusSM}" />
    <Style Selector="^ /template/ Border#Frame">
        <Setter Property="Height" Value="{atom:SharedTokenResource ControlHeightSM}" />
    </Style>
</Style>
```

这种模式的优势：
- **声明式**：尺寸样式全部在 AXAML 中声明，不需要代码逻辑
- **Token 驱动**：所有值引用 Token 资源，主题切换时自动适配
- **可组合**：可以与其他属性选择器组合（如 `^[SizeType=Large]:pointerover`）

---

## 4. 已实现 ISizeTypeAware 的控件一览

### 4.1 ISizeTypeAware（标准三级尺寸）

#### AtomUI.Controls（基础层）

| 控件 | 说明 |
|---|---|
| `AbstractToggleSwitch` | 开关 |
| `AbstractSpinIndicator` | Spin 加载指示器 |
| `AbstractProgressBar` | 进度条（含 EffectiveSizeType 自适应机制） |

#### AtomUI.Desktop.Controls（桌面层）

| 控件 | 说明 |
|---|---|
| `Button` | 按钮 |
| `Form` / `FormItem` | 表单及表单项 |
| `CompactSpace` / `CompactSpaceAddOn` | 紧凑间距容器 |
| `BaseTabControl` / `BaseTabStrip` / `TabItem` / `TabStripItem` | 标签页系列 |
| `AbstractSelect` / `SelectTag` / `SelectResultOptionsBox` / `SelectTagAwareTextBox` | 选择器系列 |
| `Menu` / `MenuItem` / `ContextMenu` | 菜单系列 |
| `Steps` / `StepsItem` / `StepsItemIndicator` | 步骤条系列 |
| `Card` / `CardGridItem` / `CardGridContent` / `CardTabsContent` | 卡片系列 |
| `AddOnDecoratedBox` | 输入框装饰器（内部） |
| `InfoPickerInput` | 日期/时间选择器输入框（内部） |
| `ComboBox` | 组合框 |
| `ListBox` | 列表框 |
| `ListView` | 列表视图 |
| `Mentions` | 提及输入框 |
| `ButtonSpinner` | 按钮旋转器 |
| `AbstractPagination` | 分页器 |
| `AbstractTransfer` / `TransferItemDecorator` | 穿梭框系列 |
| `SizeTypeAwareIconPresenter` | 尺寸感知图标呈现器（内部） |

### 4.2 ICustomizableSizeTypeAware（扩展四级尺寸）

| 控件 | 说明 | Custom 语义 |
|---|---|---|
| `Space` | 间距容器 | 自定义间距值 |
| `Drawer` | 抽屉 | 自定义抽屉宽度/高度 |
| `SkeletonAvatar` | 骨架屏头像 | 自定义尺寸 |
| `SkeletonElement` (`SkeletonButton`) | 骨架屏按钮 | 自定义尺寸 |

---

## 5. 控件开发指南

### 5.1 何时实现 ISizeTypeAware

如果你的控件满足以下任一条件，应当实现 `ISizeTypeAware`：

- 控件的高度/内边距/字体大小需要根据场景调整
- 控件可能嵌入在 `Form`、`CompactSpace` 等容器中，需要尺寸级联
- 控件遵循 Ant Design 5.0 的三级尺寸规范

如果你的控件**额外**需要支持精确自定义尺寸（如像素值），使用 `ICustomizableSizeTypeAware`。

### 5.2 标准实现模式

#### 步骤 1: 实现接口 + 注册属性

```csharp
using AtomUI.Controls;
using Avalonia;

namespace AtomUI.Desktop.Controls;

public class MyControl : TemplatedControl, ISizeTypeAware
{
    // 通过 AddOwner 共享属性（关键！不要自己重新注册）
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<MyControl>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
}
```

> ⚠️ **关键规则**: 必须使用 `SizeTypeControlProperty.SizeTypeProperty.AddOwner<T>()`，**不要** 自己 `AvaloniaProperty.Register` 一个新属性。`AddOwner` 确保属性名和元数据与其他控件一致，级联绑定才能正确工作。

#### 步骤 2: 在 Token 中定义三级尺寸值

```csharp
[ControlDesignToken]
internal class MyControlToken : AbstractControlDesignToken
{
    public const string ID = "MyControl";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    /// <summary>
    /// 中号内间距
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// 大号内间距
    /// </summary>
    public Thickness PaddingLG { get; set; }

    /// <summary>
    /// 小号内间距
    /// </summary>
    public Thickness PaddingSM { get; set; }

    /// <summary>
    /// 中号字体大小
    /// </summary>
    public double FontSize { get; set; }

    /// <summary>
    /// 大号字体大小
    /// </summary>
    public double FontSizeLG { get; set; }

    /// <summary>
    /// 小号字体大小
    /// </summary>
    public double FontSizeSM { get; set; }

    public MyControlToken() : base(ID) { }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        var lineWidth = SharedToken.LineWidth;

        // 从 SharedToken 派生尺寸值 — 不要硬编码！
        Padding   = new Thickness(SharedToken.PaddingContentHorizontal - lineWidth,
            Math.Max((SharedToken.ControlHeight - SharedToken.FontHeight) / 2 - lineWidth, 0));
        PaddingLG = new Thickness(SharedToken.PaddingContentHorizontal - lineWidth,
            Math.Max((SharedToken.ControlHeightLG - SharedToken.FontHeightLG) / 2 - lineWidth, 0));
        PaddingSM = new Thickness(8 - lineWidth,
            Math.Max((SharedToken.ControlHeightSM - SharedToken.FontHeightSM) / 2 - lineWidth, 0));

        FontSize   = SharedToken.FontSize;
        FontSizeLG = SharedToken.FontSizeLG;
        FontSizeSM = SharedToken.FontSize;  // Small 通常使用基础字体大小
    }

    protected override Type GetTokenKindType() => typeof(MyControlTokenKind);
}
```

**Token 命名约定**：

| 尺寸 | 后缀 | 示例 |
|---|---|---|
| Large | `LG` | `PaddingLG`、`FontSizeLG`、`IconSizeLG` |
| Middle | 无后缀（基准值） | `Padding`、`FontSize`、`IconSize` |
| Small | `SM` | `PaddingSM`、`FontSizeSM`、`IconSizeSM` |

#### 步骤 3: 在 AXAML 主题中使用属性选择器

```xml
<ControlTheme TargetType="atom:MyControl">
    <!-- 公共样式（尺寸无关） -->
    <Setter Property="Background" Value="{atom:SharedTokenResource ColorBgContainer}" />
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorder}" />

    <!-- 根据 SizeType 切换尺寸相关样式 -->
    <Style Selector="^[SizeType=Large]">
        <Setter Property="Padding" Value="{atom:MyControlTokenResource PaddingLG}" />
        <Setter Property="FontSize" Value="{atom:MyControlTokenResource FontSizeLG}" />
        <Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadiusLG}" />
        <Style Selector="^ /template/ Border#Frame">
            <Setter Property="MinHeight" Value="{atom:SharedTokenResource ControlHeightLG}" />
        </Style>
    </Style>

    <Style Selector="^[SizeType=Middle]">
        <Setter Property="Padding" Value="{atom:MyControlTokenResource Padding}" />
        <Setter Property="FontSize" Value="{atom:MyControlTokenResource FontSize}" />
        <Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadius}" />
        <Style Selector="^ /template/ Border#Frame">
            <Setter Property="MinHeight" Value="{atom:SharedTokenResource ControlHeight}" />
        </Style>
    </Style>

    <Style Selector="^[SizeType=Small]">
        <Setter Property="Padding" Value="{atom:MyControlTokenResource PaddingSM}" />
        <Setter Property="FontSize" Value="{atom:MyControlTokenResource FontSizeSM}" />
        <Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadiusSM}" />
        <Style Selector="^ /template/ Border#Frame">
            <Setter Property="MinHeight" Value="{atom:SharedTokenResource ControlHeightSM}" />
        </Style>
    </Style>
</ControlTheme>
```

### 5.3 CustomizableSizeType 实现模式

当控件需要支持精确自定义尺寸时：

```csharp
public class MyCustomControl : TemplatedControl, ICustomizableSizeTypeAware
{
    public static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
        CustomizableSizeTypeControlProperty.SizeTypeProperty.AddOwner<MyCustomControl>();

    // Custom 模式下的精确尺寸属性
    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<MyCustomControl, double>(nameof(Size), double.NaN);

    public CustomizableSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
}
```

在 AXAML 中添加 Custom 选择器：

```xml
<Style Selector="^[SizeType=Large]">
    <Setter Property="Width" Value="48" />
    <Setter Property="Height" Value="48" />
</Style>
<Style Selector="^[SizeType=Middle]">
    <Setter Property="Width" Value="32" />
    <Setter Property="Height" Value="32" />
</Style>
<Style Selector="^[SizeType=Small]">
    <Setter Property="Width" Value="24" />
    <Setter Property="Height" Value="24" />
</Style>
<Style Selector="^[SizeType=Custom]">
    <!-- Custom 模式下从 Size 属性取值 -->
    <Setter Property="Width" Value="{Binding Size, RelativeSource={RelativeSource Self}}" />
    <Setter Property="Height" Value="{Binding Size, RelativeSource={RelativeSource Self}}" />
</Style>
```

### 5.4 容器控件级联 SizeType

如果你的控件是容器类型（管理子控件），需要将 SizeType 传播给子控件：

```csharp
// 在子控件初始化/添加时检测并绑定
private void ConfigureChild(Control child)
{
    if (child is ISizeTypeAware)
    {
        // 方式 1: 直接绑定（适合 AXAML 声明的子控件）
        child[!SizeTypeProperty] = this[!SizeTypeProperty];

        // 方式 2: RelayBind（适合需要 Disposable 管理的场景）
        _disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, child, SizeTypeProperty));
    }
}
```

### 5.5 ProgressBar 的 EffectiveSizeType 自适应模式

`AbstractProgressBar` 展示了一种高级用法——**根据控件实际尺寸自动推断 SizeType**：

```csharp
// ProgressBar 不让用户直接设置 SizeType，
// 而是根据控件的宽度/高度自动计算 EffectiveSizeType
internal SizeType EffectiveSizeType
{
    get => _effectiveSizeType;
    set => SetAndRaise(EffectiveSizeTypeProperty, ref _effectiveSizeType, value);
}

// 子类覆写，提供不同的阈值
protected virtual SizeType CalculateEffectiveSizeType(double size)
{
    // 例如：宽度 > 120 → Large，> 80 → Middle，否则 Small
}
```

这适用于**尺寸由内容或布局动态决定**的控件。

---

## 6. 使用规范

### 6.1 在 AXAML 中使用

```xml
<!-- 直接设置尺寸 -->
<atom:Button SizeType="Large" Content="大按钮" />
<atom:Button SizeType="Middle" Content="中按钮" />
<atom:Button SizeType="Small" Content="小按钮" />

<!-- Form 容器级联 -->
<atom:Form SizeType="Small">
    <atom:FormItem Label="Name">
        <atom:TextBox />  <!-- 自动继承 Small -->
    </atom:FormItem>
    <atom:FormItem Label="Action">
        <atom:Button Content="Submit" />  <!-- 自动继承 Small -->
    </atom:FormItem>
</atom:Form>

<!-- CompactSpace 级联 -->
<atom:CompactSpace SizeType="Large">
    <atom:Button Content="Search" />  <!-- 自动继承 Large -->
    <atom:Select />                   <!-- 自动继承 Large -->
</atom:CompactSpace>
```

### 6.2 在 C# 中使用

```csharp
// 直接设置
var button = new Button { SizeType = SizeType.Small };

// 动态切换
myControl.SizeType = SizeType.Large;

// 绑定到 ViewModel
myControl[!Button.SizeTypeProperty] = new Binding("SelectedSize");
```

### 6.3 选择 SizeType vs CustomizableSizeType 的准则

| 使用 `ISizeTypeAware` | 使用 `ICustomizableSizeTypeAware` |
|---|---|
| 控件遵循标准三级尺寸 | 控件需要额外支持精确像素尺寸 |
| 如：Button、TextBox、Select | 如：Avatar（自定义像素大小）、Drawer（自定义面板宽度） |
| 绝大多数控件 | 少数特殊控件 |

### 6.4 禁止事项

- ❌ 不要自行 `AvaloniaProperty.Register` 一个名为 `"SizeType"` 的属性，必须使用 `AddOwner`
- ❌ 不要在 Token 中硬编码尺寸数值（如 `Padding = new Thickness(12, 8)`），必须从 `SharedToken` 派生
- ❌ 不要在 AXAML 中为尺寸相关样式硬编码像素值（如 `Height="32"`），必须引用 Token 资源
- ❌ 不要在控件代码中根据 `SizeType` 使用 `if/switch` 来设置样式值（应在 AXAML 属性选择器中声明）
- ❌ 不要忘记在容器控件中检测 `ISizeTypeAware` 并传播 SizeType

---

## 7. Design Token 尺寸梯度速查

### 7.1 全局高度 Token

| SizeType | Token | Seed 默认值 |
|---|---|---|
| Small | `ControlHeightSM` | 24px |
| Middle | `ControlHeight` | 32px（Seed Token） |
| Large | `ControlHeightLG` | 40px |

> `ControlHeight` 是 Seed Token（可由用户修改），`ControlHeightSM` 和 `ControlHeightLG` 是 Map Token（由算法从 `ControlHeight` 派生）。

### 7.2 全局圆角 Token

| SizeType | Token | Seed 默认值 |
|---|---|---|
| Small | `BorderRadiusSM` | 4px |
| Middle | `BorderRadius` | 6px |
| Large | `BorderRadiusLG` | 8px |

### 7.3 全局字体 Token

| SizeType | Token | Seed 默认值 |
|---|---|---|
| Small | `FontSize` | 14px |
| Middle | `FontSize` | 14px |
| Large | `FontSizeLG` | 16px |

### 7.4 组件 Token 命名约定

| 属性类别 | Small | Middle | Large |
|---|---|---|---|
| 内间距 | `PaddingSM` | `Padding` | `PaddingLG` |
| 字体大小 | `ContentFontSizeSM` | `ContentFontSize` | `ContentFontSizeLG` |
| 图标大小 | `IconSizeSM` | `IconSize` | `IconSizeLG` |
| 行高 | `ContentLineHeightSM` | `ContentLineHeight` | `ContentLineHeightLG` |

---

## 8. 完整示例：创建一个支持 SizeType 的 Tag 控件

以下是一个简化的完整示例，展示从控件类到 Token 到主题的完整模式：

### 8.1 控件类

```csharp
// AtomUI.Desktop.Controls/Tag/Tag.cs
public class Tag : AbstractTag
{
    public Tag()
    {
        this.RegisterTokenResourceScope(TagToken.ScopeProvider);
    }
}

// AtomUI.Controls/Tag/AbstractTag.cs
public abstract class AbstractTag : TemplatedControl, ISizeTypeAware
{
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<AbstractTag>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
}
```

### 8.2 组件 Token

```csharp
[ControlDesignToken]
internal class TagToken : AbstractControlDesignToken
{
    public const string ID = "Tag";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    public Thickness Padding { get; set; }
    public Thickness PaddingLG { get; set; }
    public Thickness PaddingSM { get; set; }
    public double FontSize { get; set; }
    public double FontSizeLG { get; set; }
    public double FontSizeSM { get; set; }

    public TagToken() : base(ID) { }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        // 全部从 SharedToken 派生
        FontSize   = SharedToken.FontSize;
        FontSizeLG = SharedToken.FontSizeLG;
        FontSizeSM = SharedToken.FontSizeSM;
        Padding    = new Thickness(SharedToken.PaddingXS, 0);
        PaddingLG  = new Thickness(SharedToken.Padding, 0);
        PaddingSM  = new Thickness(SharedToken.PaddingXXS, 0);
    }

    protected override Type GetTokenKindType() => typeof(TagTokenKind);
}
```

### 8.3 AXAML 主题

```xml
<ControlTheme TargetType="atom:Tag">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Name="Frame"
                    Padding="{TemplateBinding Padding}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    Background="{TemplateBinding Background}">
                <ContentPresenter Content="{TemplateBinding Content}" />
            </Border>
        </ControlTemplate>
    </Setter>

    <Style Selector="^[SizeType=Large]">
        <Setter Property="Padding" Value="{atom:TagTokenResource PaddingLG}" />
        <Setter Property="FontSize" Value="{atom:TagTokenResource FontSizeLG}" />
        <Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadiusLG}" />
    </Style>

    <Style Selector="^[SizeType=Middle]">
        <Setter Property="Padding" Value="{atom:TagTokenResource Padding}" />
        <Setter Property="FontSize" Value="{atom:TagTokenResource FontSize}" />
        <Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadius}" />
    </Style>

    <Style Selector="^[SizeType=Small]">
        <Setter Property="Padding" Value="{atom:TagTokenResource PaddingSM}" />
        <Setter Property="FontSize" Value="{atom:TagTokenResource FontSizeSM}" />
        <Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadiusSM}" />
    </Style>
</ControlTheme>
```

### 8.4 使用效果

```xml
<!-- 独立使用 -->
<atom:Tag SizeType="Large">Large Tag</atom:Tag>

<!-- 在 Form 中自动级联 -->
<atom:Form SizeType="Small">
    <atom:FormItem Label="Tags">
        <atom:Tag>Auto Small Tag</atom:Tag>
    </atom:FormItem>
</atom:Form>
```

