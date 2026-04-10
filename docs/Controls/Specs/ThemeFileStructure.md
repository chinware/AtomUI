# 主题文件结构规范

> 本文档规定 AtomUI 控件的 Theme 文件夹布局、ControlTheme AXAML 结构、ThemeConstants 定义、以及 Theme 注册方式。

---

## 1. 文件夹结构

每个 Platform 层控件的 Theme 文件遵循以下标准目录结构：

```
AtomUI.Desktop.Controls/
└── ControlName/
    ├── ControlName.cs              # 具体控件类
    ├── ControlNameToken.cs         # 组件 Token 定义
    └── Themes/
        ├── ControlNameTheme.axaml          # ControlTheme AXAML 模板
        ├── ControlNameTheme.cs             # Theme 代码后台
        ├── ControlNameThemes.axaml         # ResourceDictionary 注册
        └── ControlNameThemeConstants.cs    # 模板部件名称常量
```

**实际示例 —— Button：**

```
Buttons/
├── Button.cs
├── ButtonToken.cs
├── IconButton.cs
├── DropdownButton.cs
└── Themes/
    ├── ButtonTheme.axaml
    ├── ButtonTheme.cs
    ├── ButtonThemes.axaml
    ├── ButtonThemeConstants.cs
    ├── DropdownButtonTheme.axaml
    ├── DropdownButtonTheme.cs
    ├── DropdownButtonThemeConstants.cs
    ├── IconButtonTheme.axaml
    ├── IconButtonTheme.cs
    └── ...
```

**实际示例 —— Tag：**

```
Tag/
├── Tag.cs
├── TagToken.cs
└── Themes/
    ├── TagTheme.axaml
    ├── TagTheme.cs
    └── TagThemes.axaml
```

---

## 2. ControlTheme AXAML（`ControlNameTheme.axaml`）

### 2.1 基本结构

```xml
<ControlTheme xmlns="https://github.com/avaloniaui"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:atom="https://atomui.net"
              xmlns:antdicons="https://atomui.net/icons/antdesign"
              x:ClassModifier="internal"
              x:Class="AtomUI.Desktop.Controls.Themes.TagTheme"
              TargetType="atom:Tag">

    <!-- 模板定义 -->
    <Setter Property="Template">
        <ControlTemplate TargetType="atom:Tag">
            <!-- 控件可视化树 -->
        </ControlTemplate>
    </Setter>

    <!-- 样式定义 -->
    <Style Selector="^:is(atom|Tag)">
        <Setter Property="Background" Value="{atom:TagTokenResource DefaultBg}" />
        <!-- ... -->
    </Style>
</ControlTheme>
```

### 2.2 强制规则

| 规则 | 说明 |
|---|---|
| `xmlns:atom="https://atomui.net"` | AtomUI 控件命名空间，**必须** 使用 `atom:` 前缀 |
| `xmlns:antdicons="https://atomui.net/icons/antdesign"` | 图标命名空间（如果使用图标） |
| `x:Class` 指向代码后台 | `AtomUI.Desktop.Controls.Themes.XxxTheme` |
| `TargetType` 使用 `atom:Xxx` | 指向 AtomUI 的具体控件类型 |
| **禁止** 硬编码颜色、尺寸、画刷 | 必须使用 Token 资源引用 |
| 模板部件使用常量名 | `Name="{x:Static atom:ButtonThemeConstants.WaveSpiritPart}"` |

### 2.3 Token 资源引用

在 AXAML 中引用 Token 的两种方式：

```xml
<!-- 组件 Token：使用组件专属标记扩展 -->
<Setter Property="Background" Value="{atom:TagTokenResource DefaultBg}" />
<Setter Property="Foreground" Value="{atom:TagTokenResource DefaultColor}" />
<Setter Property="FontSize" Value="{atom:TagTokenResource TagFontSize}" />
<Setter Property="Padding" Value="{atom:TagTokenResource TagPadding}" />

<!-- 全局共享 Token：使用 SharedTokenResource -->
<Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorder}" />
<Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadiusSM}" />
<Setter Property="BorderThickness" Value="{atom:SharedTokenResource BorderThickness}" />
```

### 2.4 模板部件引用

模板部件的 `Name` 属性 **必须** 使用 `x:Static` 引用 `ThemeConstants` 中的常量：

```xml
<!-- ✅ 正确：使用常量引用 -->
<atom:WaveSpiritDecorator Name="{x:Static atom:ButtonThemeConstants.WaveSpiritPart}" />
<ContentPresenter Name="{x:Static atom:ButtonThemeConstants.ContentPresenterPart}" />

<!-- ❌ 错误：硬编码字符串 -->
<atom:WaveSpiritDecorator Name="PART_WaveSpirit" />
```

### 2.5 样式选择器语法

使用 `^:is(atom|ControlName)` 选择器语法实现样式继承：

```xml
<!-- 基础样式 -->
<Style Selector="^:is(atom|Tag)">
    <Setter Property="HorizontalAlignment" Value="Left" />
    <Setter Property="Background" Value="{atom:TagTokenResource DefaultBg}" />
</Style>

<!-- 伪类样式 -->
<Style Selector="^:is(atom|Tag)">
    <Style Selector="^:custom-color">
        <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorTextLightSolid}" />
    </Style>
    <Style Selector="^:pointerover">
        <Setter Property="Background" Value="{atom:TokenResource HoverBg}" />
    </Style>
</Style>

<!-- 属性值匹配 -->
<Style Selector="^[ButtonType=Default], ^[ButtonType=Link]">
    <Setter Property="Template">
        <!-- ... -->
    </Setter>
</Style>
```

### 2.6 TemplateBinding 使用

在 `ControlTemplate` 内部，使用 `TemplateBinding` 绑定控件属性：

```xml
<ControlTemplate TargetType="atom:Tag">
    <Panel>
        <Border Name="Frame"
                Background="{TemplateBinding Background}"
                BorderBrush="{TemplateBinding BorderBrush}"
                CornerRadius="{TemplateBinding CornerRadius}"
                BorderThickness="{TemplateBinding BorderThickness}" />
        <DockPanel Margin="{TemplateBinding Padding}">
            <atom:IconPresenter Icon="{TemplateBinding Icon}" />
            <atom:TextBlock Text="{TemplateBinding TagText}" />
        </DockPanel>
    </Panel>
</ControlTemplate>
```

### 2.7 完整示例

以下是 `TagTheme.axaml` 的核心结构：

```xml
<ControlTheme xmlns="https://github.com/avaloniaui"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:atom="https://atomui.net"
              x:ClassModifier="internal"
              x:Class="AtomUI.Desktop.Controls.Themes.TagTheme"
              TargetType="atom:Tag">
    <Setter Property="Template">
        <ControlTemplate TargetType="atom:Tag">
            <Panel>
                <Border Name="Frame"
                        Background="{TemplateBinding Background}"
                        BorderBrush="{TemplateBinding BorderBrush}"
                        CornerRadius="{TemplateBinding CornerRadius}"
                        BorderThickness="{TemplateBinding BorderThickness}" />
                <DockPanel LastChildFill="True" Margin="{TemplateBinding Padding}">
                    <atom:IconPresenter Name="IconPresenter"
                                        DockPanel.Dock="Left"
                                        Icon="{TemplateBinding Icon}" />
                    <atom:IconButton Name="PART_CloseButton"
                                     DockPanel.Dock="Right"
                                     IsVisible="{TemplateBinding IsClosable}"
                                     Icon="{TemplateBinding CloseIcon}" />
                    <atom:TextBlock Name="TagTextLabel"
                                    Text="{TemplateBinding TagText}"
                                    Padding="{TemplateBinding TagTextPaddingInline}" />
                </DockPanel>
            </Panel>
        </ControlTemplate>
    </Setter>

    <Style Selector="^:is(atom|Tag)">
        <Setter Property="HorizontalAlignment" Value="Left" />
        <Setter Property="Background" Value="{atom:TagTokenResource DefaultBg}" />
        <Setter Property="Foreground" Value="{atom:TagTokenResource DefaultColor}" />
        <Setter Property="FontSize" Value="{atom:TagTokenResource TagFontSize}" />
        <Setter Property="Padding" Value="{atom:TagTokenResource TagPadding}" />
        <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorder}" />
        <Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadiusSM}" />

        <Style Selector="^:custom-color">
            <Setter Property="Foreground"
                    Value="{atom:SharedTokenResource ColorTextLightSolid}" />
        </Style>
    </Style>
</ControlTheme>
```

---

## 3. Theme 代码后台（`ControlNameTheme.cs`）

### 3.1 最简形式

```csharp
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

internal class TagTheme : ControlTheme
{
}
```

### 3.2 包含静态资源的形式

当 Theme 需要提供转换器或其他静态资源时：

```csharp
using AtomUI.Controls.Converters;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

public class ButtonTheme : ControlTheme
{
    public static readonly ButtonIconVisibleConverter IconVisibleConverter = new();
    public static IList<double> DashedStyle = [4, 2];
}
```

### 3.3 规则

| 规则 | 说明 |
|---|---|
| 继承 `ControlTheme`（来自 `Avalonia.Styling`） | Avalonia 的主题基类 |
| 命名空间 `AtomUI.Desktop.Controls.Themes` | 所有 Theme 代码后台使用此命名空间 |
| 访问修饰符视需要而定 | 如果 AXAML 中 `x:ClassModifier="internal"` 则为 `internal`；如果 AXAML 中引用了静态成员则可为 `public` |

---

## 4. ThemeConstants（`ControlNameThemeConstants.cs`）

### 4.1 定义

模板部件名称常量 **必须** 在独立的 `ThemeConstants` 类中定义：

```csharp
namespace AtomUI.Desktop.Controls.Themes;

internal class ButtonThemeConstants
{
    public const string MainInfoLayoutPart = "PART_MainInfoLayout";
    public const string RootLayoutPart = "PART_RootLayout";
    public const string LoadingIconPart = "PART_LoadingIcon";
    public const string ButtonIconPart = "PART_ButtonIcon";
    public const string WaveSpiritPart = "PART_WaveSpirit";
    public const string ContentPresenterPart = "PART_ContentPresenter";
}
```

### 4.2 规则

| 规则 | 说明 |
|---|---|
| 类修饰符 `internal` | 模板部件名称是实现细节 |
| 所有部件名以 `PART_` 前缀 | Avalonia 约定 |
| 常量字段以 `Part` 后缀 | 如 `WaveSpiritPart`、`ContentPresenterPart` |
| 使用 `public const string` | 可在 AXAML 的 `x:Static` 中引用 |

### 4.3 在控件代码中引用

```csharp
protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    _waveSpiritDecorator = e.NameScope.Find<WaveSpiritDecorator>(
        ButtonThemeConstants.WaveSpiritPart);
}
```

### 4.4 在 AXAML 中引用

```xml
<atom:WaveSpiritDecorator
    Name="{x:Static atom:ButtonThemeConstants.WaveSpiritPart}" />
```

---

## 5. Theme 注册（`ControlNameThemes.axaml`）

### 5.1 基本结构

```xml
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:atom="https://atomui.net"
                    x:ClassModifier="internal">
    <atom:TagTheme x:Key="{x:Type atom:Tag}" TargetType="atom:Tag"/>
</ResourceDictionary>
```

### 5.2 带合并字典的形式

当一个控件组包含多个子控件 Theme 时：

```xml
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:atom="https://atomui.net"
                    x:ClassModifier="internal">
    <ResourceDictionary.MergedDictionaries>
        <ResourceInclude Source="SplitButtonTheme.axaml"/>
        <ResourceInclude Source="HyperLinkButtonTheme.axaml"/>
    </ResourceDictionary.MergedDictionaries>
    <atom:ButtonTheme x:Key="{x:Type atom:Button}" TargetType="atom:Button"/>
    <atom:DropdownButtonTheme x:Key="{x:Type atom:DropdownButton}" TargetType="atom:DropdownButton"/>
    <atom:IconButtonTheme x:Key="{x:Type atom:IconButton}" TargetType="atom:IconButton"/>
</ResourceDictionary>
```

### 5.3 规则

| 规则 | 说明 |
|---|---|
| `x:Key="{x:Type atom:Xxx}"` | 使用 `x:Type` 将 Theme 与控件类型关联 |
| `TargetType="atom:Xxx"` | 指定目标控件类型 |
| 同一组控件的 Theme 可合并到一个 `Themes.axaml` | 如 Button 组 |
| `x:ClassModifier="internal"` | 资源字典为内部可见 |

---

## 6. Theme 注册到 Provider

所有控件的 Theme 最终需要在 `DesktopControlThemesProvider` 中注册。这通过 `ThemeManagerBuilderExtensions` 的 `UseDesktopControls()` 扩展方法完成。

新增控件时，需要将 `ControlNameThemes.axaml` 添加到 Provider 的资源列表中。

---

## 7. 常见 AXAML 模式

### 7.1 WaveSpiritDecorator 集成

```xml
<Panel>
    <atom:WaveSpiritDecorator
        Name="{x:Static atom:ButtonThemeConstants.WaveSpiritPart}"
        CornerRadius="{TemplateBinding EffectiveCornerRadius}"
        WaveType="{TemplateBinding WaveSpiritType}" />
    <!-- 控件主体 -->
    <Border Name="Frame" ... />
</Panel>
```

### 7.2 图标显示

```xml
<atom:IconPresenter
    Name="{x:Static atom:ButtonThemeConstants.ButtonIconPart}"
    Icon="{TemplateBinding Icon}"
    DockPanel.Dock="Left" />
```

### 7.3 尺寸变体样式

```xml
<Style Selector="^:is(atom|Button)">
    <!-- 默认 Middle 尺寸 -->
    <Setter Property="Padding" Value="{atom:ButtonTokenResource Padding}" />
    <Setter Property="FontSize" Value="{atom:ButtonTokenResource ContentFontSize}" />

    <Style Selector="^[SizeType=Small]">
        <Setter Property="Padding" Value="{atom:ButtonTokenResource PaddingSM}" />
        <Setter Property="FontSize" Value="{atom:ButtonTokenResource ContentFontSizeSM}" />
    </Style>

    <Style Selector="^[SizeType=Large]">
        <Setter Property="Padding" Value="{atom:ButtonTokenResource PaddingLG}" />
        <Setter Property="FontSize" Value="{atom:ButtonTokenResource ContentFontSizeLG}" />
    </Style>
</Style>
```

### 7.4 焦点视觉

```xml
<Border Name="FocusVisual"
        IsVisible="False"
        IsHitTestVisible="False"
        BorderThickness="{atom:SharedTokenResource FocusVisualBorderThickness}"
        BorderBrush="{atom:SharedTokenResource ColorFocusBorder}"
        CornerRadius="{TemplateBinding EffectiveCornerRadius}" />
```

---

## 8. 禁止事项

| 禁止事项 | 原因 |
|---|---|
| ❌ AXAML 中硬编码 `Color`、`Thickness`、`FontSize` 等值 | 必须使用 Token 资源引用 |
| ❌ 模板部件 `Name` 硬编码字符串 | 必须使用 `x:Static` 引用 `ThemeConstants` |
| ❌ 遗漏 `Themes.axaml` 注册 | 控件无法找到 Theme |
| ❌ Theme 文件放在 `Themes/` 目录之外 | 违反标准目录结构 |
| ❌ 在 AXAML 中使用 Avalonia 原生控件命名空间替代 `atom:` | 应使用 `atom:` 前缀引用 AtomUI 控件 |

