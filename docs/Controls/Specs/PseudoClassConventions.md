# 伪类定义规范

> 本文档规定 AtomUI 控件中伪类（Pseudo-Class）的定义方式、命名约定、复用规则和运行时设置方式。

---

## 1. 伪类概述

伪类是 Avalonia 样式系统中基于控件状态匹配的选择器。在 AXAML 中，伪类以 `:name` 的形式出现在样式选择器中：

```xml
<Style Selector="^:pointerover">
    <Setter Property="Background" Value="{atom:TokenResource HoverBg}" />
</Style>
<Style Selector="^:pressed">
    <Setter Property="Background" Value="{atom:TokenResource ActiveBg}" />
</Style>
<Style Selector="^:danger">
    <Setter Property="Background" Value="{atom:TokenResource DangerBg}" />
</Style>
```

---

## 2. StdPseudoClass 复用

AtomUI 在 `AtomUI.Core` 中定义了 `StdPseudoClass` 静态类，包含所有通用伪类常量。在定义新伪类之前，**必须** 先检查此类中是否已有：

```csharp
// AtomUI.Core/StdPseudoClass.cs
public static class StdPseudoClass
{
    public const string Disabled = ":disabled";
    public const string Focus = ":focus";
    public const string FocusWithIn = ":focus-within";
    public const string PointerOver = ":pointerover";
    public const string Checked = ":checked";
    public const string UnChecked = ":unchecked";
    public const string Pressed = ":pressed";
    public const string Selected = ":selected";
    public const string Indeterminate = ":indeterminate";
    public const string Dragging = ":dragging";
    public const string Empty = ":empty";
    public const string FlyoutOpen = ":flyout-open";
    public const string Expanded = ":expanded";
    public const string Open = ":open";
    public const string Horizontal = ":horizontal";
    public const string Vertical = ":vertical";
    public const string Active = ":active";
    public const string InActive = ":inactive";
    public const string Error = ":error";
    public const string Information = ":information";
    public const string Success = ":success";
    public const string Warning = ":warning";
    // ... 更多
}
```

**规则**：如果 `StdPseudoClass` 中已有语义匹配的伪类，**禁止** 重新定义。

---

## 3. 控件专属伪类定义

### 3.1 伪类常量类

每个控件如果需要自定义伪类，**必须** 定义独立的伪类常量类：

```csharp
// 放置位置取决于伪类的共享范围：
// - Base 层共享伪类 → AtomUI.Controls/ControlName/ControlNamePseudoClass.cs
// - Platform 层特有伪类 → AtomUI.Desktop.Controls/ControlName/ControlNamePseudoClass.cs

namespace AtomUI.Controls;

public static class ButtonPseudoClass
{
    public const string IconOnly = ":icononly";
    public const string Loading = ":loading";
    public const string IsDanger = ":danger";
    public const string DefaultType = ":default";
    public const string DashedType = ":dashed";
    public const string PrimaryType = ":primary";
    public const string LinkType = ":link";
    public const string TextType = ":text";
    public const string Visited = ":visited";
}
```

```csharp
namespace AtomUI.Controls;

public static class TagPseudoClass
{
    public const string CustomColor = ":custom-color";
    public const string PresetColor = ":preset-color";
    public const string StatusColor = ":status-color";
}
```

### 3.2 命名规则

| 规则 | 说明 | 示例 |
|---|---|---|
| 类名：`{ControlName}PseudoClass` | 不带 `Abstract` 前缀 | `ButtonPseudoClass`、`TagPseudoClass` |
| 修饰符：`public static class` | 静态类，不可实例化 | |
| 字段修饰符：`public const string` | 编译时常量 | |
| 伪类字符串以 `:` 前缀开头 | Avalonia 约定 | `":loading"`、`":danger"` |
| 伪类名使用小写 kebab-case | Avalonia / CSS 约定 | `":custom-color"`、`":focus-within"` |
| 字段名使用 PascalCase | C# 命名约定 | `CustomColor`、`Loading` |

### 3.3 伪类放置规则

| 伪类范围 | 放置位置 | 说明 |
|---|---|---|
| 跨平台共享 | `AtomUI.Controls/ControlName/ControlNamePseudoClass.cs` | Desktop 和 Mobile 都使用 |
| 仅桌面使用 | `AtomUI.Desktop.Controls/ControlName/ControlNamePseudoClass.cs` | 仅桌面控件使用 |

> **判断依据**：如果伪类对应的控件状态在 Base 层抽象控件中已定义，则伪类应放在 Base 层。

---

## 4. `[PseudoClasses]` 特性

### 4.1 声明方式

控件类 **必须** 使用 `[PseudoClasses]` 特性声明所有使用到的自定义伪类：

```csharp
[PseudoClasses(ButtonPseudoClass.IconOnly,
    ButtonPseudoClass.Loading,
    ButtonPseudoClass.IsDanger,
    ButtonPseudoClass.DefaultType,
    ButtonPseudoClass.DashedType,
    ButtonPseudoClass.PrimaryType,
    ButtonPseudoClass.LinkType,
    ButtonPseudoClass.TextType)]
public class Button : AvaloniaButton, ISizeTypeAware, IWaveSpiritAwareControl
{
    // ...
}
```

### 4.2 规则

| 规则 | 说明 |
|---|---|
| 必须引用伪类常量 | 使用 `ButtonPseudoClass.XXX`，**禁止** 硬编码字符串 |
| 列出所有自定义伪类 | 不需要列出 Avalonia 内置伪类（如 `:disabled`、`:pointerover`） |
| 特性放在类声明之上 | 紧接 `public class` 之前 |

---

## 5. 运行时伪类设置

### 5.1 使用 `PseudoClasses.Set()`

在控件的 `UpdatePseudoClasses()` 方法中集中更新伪类状态：

```csharp
private void UpdatePseudoClasses()
{
    PseudoClasses.Set(ButtonPseudoClass.IconOnly, Icon is not null && Content is null);
    PseudoClasses.Set(ButtonPseudoClass.Loading, IsLoading);
    PseudoClasses.Set(ButtonPseudoClass.DefaultType, ButtonType == ButtonType.Default);
    PseudoClasses.Set(ButtonPseudoClass.DashedType, ButtonType == ButtonType.Dashed);
    PseudoClasses.Set(ButtonPseudoClass.PrimaryType, ButtonType == ButtonType.Primary);
    PseudoClasses.Set(ButtonPseudoClass.LinkType, ButtonType == ButtonType.Link);
    PseudoClasses.Set(ButtonPseudoClass.TextType, ButtonType == ButtonType.Text);
    PseudoClasses.Set(ButtonPseudoClass.IsDanger, IsDanger);
}
```

### 5.2 调用时机

`UpdatePseudoClasses()` **应在** 以下时机被调用：

| 时机 | 说明 |
|---|---|
| `OnApplyTemplate` | 初始化时设置 |
| `OnPropertyChanged` | 当相关属性变更时 |

```csharp
protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    UpdatePseudoClasses();  // ← 初始化
}

protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
{
    base.OnPropertyChanged(change);

    if (change.Property == ContentProperty ||
        change.Property == IsLoadingProperty)
    {
        UpdatePseudoClasses();  // ← 属性变更时
    }
}
```

### 5.3 也可以在特定场景直接设置

```csharp
// 在非 UpdatePseudoClasses 方法中直接设置也是允许的
PseudoClasses.Set(TagPseudoClass.PresetColor, true);
PseudoClasses.Set(TagPseudoClass.StatusColor, false);
PseudoClasses.Set(TagPseudoClass.CustomColor, false);
```

---

## 6. 在 AXAML Theme 中使用伪类

### 6.1 基本选择器

```xml
<Style Selector="^:is(atom|Button)">
    <Style Selector="^:danger">
        <Setter Property="Background" Value="{atom:SharedTokenResource ColorError}" />
    </Style>
    <Style Selector="^:loading">
        <Setter Property="IsHitTestVisible" Value="False" />
    </Style>
</Style>
```

### 6.2 组合选择器

```xml
<!-- 伪类 + 属性值 -->
<Style Selector="^:danger[ButtonType=Primary]">
    <Setter Property="Background" Value="{atom:SharedTokenResource ColorError}" />
</Style>

<!-- 多伪类组合 -->
<Style Selector="^:danger:pointerover">
    <Setter Property="Background" Value="{atom:SharedTokenResource ColorErrorHover}" />
</Style>
```

### 6.3 嵌套结构

```xml
<Style Selector="^:is(atom|Tag)">
    <!-- 基础样式 -->
    <Setter Property="Background" Value="{atom:TagTokenResource DefaultBg}" />

    <!-- 预设颜色 -->
    <Style Selector="^:preset-color">
        <!-- 预设颜色样式 -->
    </Style>

    <!-- 状态颜色 -->
    <Style Selector="^:status-color">
        <!-- 状态颜色样式 -->
    </Style>

    <!-- 自定义颜色 -->
    <Style Selector="^:custom-color">
        <Setter Property="Foreground"
                Value="{atom:SharedTokenResource ColorTextLightSolid}" />
    </Style>
</Style>
```

---

## 7. 互斥伪类模式

当多个伪类是互斥关系时（如控件类型），**必须** 在更新时确保同时清除其他互斥伪类：

```csharp
// ✅ 正确：互斥伪类全部更新
private void UpdatePseudoClasses()
{
    PseudoClasses.Set(ButtonPseudoClass.DefaultType, ButtonType == ButtonType.Default);
    PseudoClasses.Set(ButtonPseudoClass.DashedType, ButtonType == ButtonType.Dashed);
    PseudoClasses.Set(ButtonPseudoClass.PrimaryType, ButtonType == ButtonType.Primary);
    PseudoClasses.Set(ButtonPseudoClass.LinkType, ButtonType == ButtonType.Link);
    PseudoClasses.Set(ButtonPseudoClass.TextType, ButtonType == ButtonType.Text);
}
```

```csharp
// ✅ 正确：互斥切换
PseudoClasses.Set(TagPseudoClass.PresetColor, true);
PseudoClasses.Set(TagPseudoClass.StatusColor, false);  // ← 清除其他
PseudoClasses.Set(TagPseudoClass.CustomColor, false);  // ← 清除其他
```

---

## 8. 禁止事项

| 禁止事项 | 原因 |
|---|---|
| ❌ 硬编码伪类字符串 `":loading"` | 必须引用常量 `ButtonPseudoClass.Loading` |
| ❌ 重新定义 `StdPseudoClass` 中已有的伪类 | 必须复用 |
| ❌ 遗漏 `[PseudoClasses]` 特性声明 | 设计器和工具链依赖此声明 |
| ❌ 只设置一个互斥伪类而不清除其他 | 导致多个互斥伪类同时激活 |
| ❌ 在 `#region 公共属性定义` 中设置伪类 | 伪类更新应在方法中集中处理 |

