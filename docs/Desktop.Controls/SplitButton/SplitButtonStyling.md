# SplitButton 自定义样式指南

SplitButton 的视觉表现通过 `ControlTheme` + Design Token 系统控制。由于 SplitButton 内部包含两个完整的 AtomUI `Button` 实例，大部分视觉效果由内部 Button 的 Theme 驱动。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 SplitButton 的公共属性来控制外观：

```xml
<!-- 不同类型 -->
<atom:SplitButton>Default Split</atom:SplitButton>
<atom:SplitButton IsPrimaryButtonType="True">Primary Split</atom:SplitButton>

<!-- 不同尺寸 -->
<atom:SplitButton SizeType="Large">Large</atom:SplitButton>
<atom:SplitButton SizeType="Middle">Middle</atom:SplitButton>
<atom:SplitButton SizeType="Small">Small</atom:SplitButton>

<!-- 危险样式 -->
<atom:SplitButton IsDanger="True">Danger Default</atom:SplitButton>
<atom:SplitButton IsDanger="True" IsPrimaryButtonType="True">Danger Primary</atom:SplitButton>

<!-- 自定义触发方式和弹出位置 -->
<atom:SplitButton TriggerType="Hover"
                   Placement="Bottom"
                   IsShowArrow="True"
                   IsPointAtCenter="True">
    Options
</atom:SplitButton>

<!-- 自定义下拉按钮图标 -->
<atom:SplitButton OpenIndicator="{antdicons:AntDesignIconProvider Kind=UserOutlined}">
    Custom Icon
</atom:SplitButton>

<!-- 主按钮带图标 -->
<atom:SplitButton Icon="{antdicons:AntDesignIconProvider Kind=DownloadOutlined}">
    Download
</atom:SplitButton>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/SplitButtonShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 SplitButton 进行全局或局部样式覆盖：

### 全局统一间距和对齐

```xml
<Window.Styles>
    <Style Selector="atom|SplitButton">
        <Setter Property="Margin" Value="5" />
        <Setter Property="VerticalAlignment" Value="Bottom" />
    </Style>
</Window.Styles>
```

### 统一尺寸

```xml
<Window.Styles>
    <Style Selector="atom|SplitButton">
        <Setter Property="SizeType" Value="Large" />
    </Style>
</Window.Styles>
```

### 调整悬浮延迟

```xml
<Window.Styles>
    <Style Selector="atom|SplitButton">
        <Setter Property="MouseEnterDelay" Value="300" />
        <Setter Property="MouseLeaveDelay" Value="200" />
    </Style>
</Window.Styles>
```

### 按伪类定制样式

```xml
<!-- Flyout 打开时的样式 -->
<Style Selector="atom|SplitButton:flyout-open">
    <Setter Property="Opacity" Value="0.9" />
</Style>

<!-- 禁用态的自定义样式 -->
<Style Selector="atom|SplitButton:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

---

## 3. 访问内部 Button 模板部件

SplitButton 内部包含两个 AtomUI `Button`，可通过模板选择器 (`/template/`) 访问它们以定制更细粒度的样式：

### 定制内部按钮悬浮样式

```xml
<!-- 内部按钮悬浮时提升 ZIndex（已内置，此处仅作示例） -->
<Style Selector="atom|SplitButton /template/ #PART_MainLayout > atom|Button:pointerover">
    <Setter Property="ZIndex" Value="2000" />
</Style>
```

### 访问主按钮和下拉按钮

```xml
<!-- 仅定制主按钮（左侧）的外边距 -->
<Style Selector="atom|SplitButton /template/ atom|Button#PART_PrimaryButton">
    <Setter Property="Margin" Value="0" />
</Style>

<!-- 仅定制下拉按钮（右侧）的最小宽度 -->
<Style Selector="atom|SplitButton /template/ atom|Button#PART_SecondaryButton">
    <Setter Property="MinWidth" Value="36" />
</Style>
```

---

## 4. 尺寸与按钮类型组合

SplitButton 支持三种尺寸 × 两种按钮类型的组合：

```xml
<!-- Default 样式的三种尺寸 -->
<atom:SplitButton SizeType="Large">Large Default</atom:SplitButton>
<atom:SplitButton SizeType="Middle">Middle Default</atom:SplitButton>
<atom:SplitButton SizeType="Small">Small Default</atom:SplitButton>

<!-- Primary 样式的三种尺寸 -->
<atom:SplitButton SizeType="Large" IsPrimaryButtonType="True">Large Primary</atom:SplitButton>
<atom:SplitButton SizeType="Middle" IsPrimaryButtonType="True">Middle Primary</atom:SplitButton>
<atom:SplitButton SizeType="Small" IsPrimaryButtonType="True">Small Primary</atom:SplitButton>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/SplitButtonShowCase.axaml` 中 "Size" 示例。

---

## 5. 控制动画行为

```xml
<!-- 禁用过渡动画（背景色、前景色不再渐变过渡） -->
<atom:SplitButton IsMotionEnabled="False">无动画</atom:SplitButton>

<!-- 禁用点击波纹效果 -->
<atom:SplitButton IsWaveSpiritEnabled="False">无波纹</atom:SplitButton>
```

---

## 6. 通过 ControlTheme 完全替换主题

如果需要彻底替换 SplitButton 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomSplitButton" TargetType="atom:SplitButton">
    <Setter Property="Template">
        <ControlTemplate>
            <DockPanel>
                <atom:Button Name="PART_PrimaryButton"
                             Content="{TemplateBinding Content}"
                             Icon="{TemplateBinding Icon}"
                             SizeType="{TemplateBinding SizeType}" />
                <atom:Button Name="PART_SecondaryButton"
                             Icon="{TemplateBinding OpenIndicator}"
                             SizeType="{TemplateBinding SizeType}" />
            </DockPanel>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:SplitButton Theme="{StaticResource MyCustomSplitButton}">自定义</atom:SplitButton>
```

> ⚠️ 注意：完全替换 ControlTheme 时，必须保留 `PART_PrimaryButton` 和 `PART_SecondaryButton` 这两个模板部件名称，否则 SplitButton 内部的点击事件、Flyout 管理和圆角分配等逻辑将失效。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|SplitButton` 语法引用 `atom` XML 命名空间下的 `SplitButton` 类型，其中 `|` 是命名空间分隔符。

### 按类型和属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|SplitButton` | 匹配所有 SplitButton 实例，用于设置全局通用样式 |
| `atom\|SplitButton[IsPrimaryButtonType=True]` | 匹配 Primary 样式的 SplitButton（蓝色背景） |
| `atom\|SplitButton[IsDanger=True]` | 匹配危险样式的 SplitButton（红色系） |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|SplitButton[SizeType=Large]` | 匹配大号 SplitButton |
| `atom\|SplitButton[SizeType=Middle]` | 匹配中号 SplitButton（默认） |
| `atom\|SplitButton[SizeType=Small]` | 匹配小号 SplitButton |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|SplitButton:flyout-open` | 匹配 Flyout 打开状态，可用于定制弹出时的视觉反馈 |
| `atom\|SplitButton:pressed` | 匹配主按钮按下状态（键盘触发），可用于定制按压视觉 |
| `atom\|SplitButton:checked` | 匹配选中状态（仅 `ToggleSplitButton` 派生类） |
| `atom\|SplitButton:disabled` | 匹配禁用状态 |
| `atom\|SplitButton:pointerover` | 匹配鼠标悬浮状态 |
| `atom\|SplitButton:focus-visible` | 匹配通过键盘获得焦点的状态 |

### 访问内部模板部件

| 选择器 | 说明 |
|---|---|
| `atom\|SplitButton /template/ DockPanel#PART_MainLayout` | 访问根布局 DockPanel |
| `atom\|SplitButton /template/ atom\|Button#PART_PrimaryButton` | 访问左侧主按钮 |
| `atom\|SplitButton /template/ atom\|Button#PART_SecondaryButton` | 访问右侧下拉按钮 |
| `atom\|SplitButton /template/ #PART_MainLayout > atom\|Button` | 匹配所有内部 Button |
| `atom\|SplitButton /template/ #PART_MainLayout > atom\|Button:pointerover` | 匹配悬浮状态的内部 Button |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|SplitButton[IsPrimaryButtonType=True]:not(:disabled)` | 非禁用状态的 Primary SplitButton |
| `atom\|SplitButton[IsDanger=True]:flyout-open` | 危险 SplitButton 在 Flyout 打开时 |
| `atom\|SplitButton[SizeType=Large][IsPrimaryButtonType=True]` | 大号 Primary SplitButton |
