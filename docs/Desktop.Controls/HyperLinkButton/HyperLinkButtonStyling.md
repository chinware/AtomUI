# HyperLinkButton 自定义样式指南

HyperLinkButton 的视觉表现通过 `ControlTheme` + Design Token 系统控制。它复用 `ButtonToken` 作用域中的间距和字体 Token，颜色使用 `SharedToken` 中的链接色系（`ColorLink` / `ColorError`）。

---

## 1. 使用属性直接控制

最简单的方式是通过 HyperLinkButton 的公共属性来控制外观：

```xml
<!-- 基本超链接按钮 -->
<atom:HyperLinkButton NavigateUri="https://github.com/AntDesign/AtomUI">
    Visit AtomUI
</atom:HyperLinkButton>

<!-- 带图标 -->
<atom:HyperLinkButton Icon="{antdicons:AntDesignIconProvider Kind=LinkOutlined}"
                       NavigateUri="https://example.com">
    Open Link
</atom:HyperLinkButton>

<!-- 危险样式 -->
<atom:HyperLinkButton IsDanger="True">
    Delete Account
</atom:HyperLinkButton>

<!-- 不同尺寸 -->
<atom:HyperLinkButton SizeType="Large">Large Link</atom:HyperLinkButton>
<atom:HyperLinkButton SizeType="Small">Small Link</atom:HyperLinkButton>

<!-- 加载状态 -->
<atom:HyperLinkButton IsLoading="True">Loading...</atom:HyperLinkButton>
```

> 📖 实际使用参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/AboutUsPage.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 HyperLinkButton 进行全局或局部样式覆盖：

### 统一链接样式

```xml
<Window.Styles>
    <Style Selector="atom|HyperLinkButton">
        <Setter Property="SizeType" Value="Small" />
        <Setter Property="Margin" Value="0,0,8,0" />
    </Style>
</Window.Styles>
```

### 已访问状态的视觉区分

```xml
<!-- 已访问的链接使用紫色 -->
<Style Selector="atom|HyperLinkButton:visited">
    <Setter Property="Foreground" Value="#531dab" />
</Style>
<Style Selector="atom|HyperLinkButton:visited:pointerover">
    <Setter Property="Foreground" Value="#722ed1" />
</Style>
```

### 自定义危险链接的悬浮效果

```xml
<Style Selector="atom|HyperLinkButton:danger:not(:disabled):pointerover">
    <Setter Property="Foreground" Value="#ff7875" />
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

### 自定义模板内部元素

```xml
<!-- 自定义图标颜色 -->
<Style Selector="atom|HyperLinkButton /template/ atom|IconPresenter#PART_ButtonIcon">
    <Setter Property="IconBrush" Value="#1677FF" />
</Style>

<!-- 加载图标的自定义样式 -->
<Style Selector="atom|HyperLinkButton:loading /template/ atom|Icon#PART_LoadingIcon">
    <Setter Property="FillBrush" Value="#faad14" />
</Style>
```

---

## 3. 颜色体系

HyperLinkButton 使用链接色系，与 Button 的颜色体系完全不同。不同状态下的颜色映射：

### 默认链接

| 状态 | 前景色 | 图标色 | 背景色 |
|---|---|---|---|
| 正常 | `ColorLink` | `ColorLink` | 透明 |
| 悬浮（`:pointerover`） | `ColorLinkHover` | `ColorLinkHover` | 透明 |
| 按下（`:pressed`） | `ColorLinkActive` | `ColorLinkActive` | 透明 |
| 禁用（`:disabled`） | `ColorTextDisabled` | `ColorTextDisabled` | 透明 |

### 危险链接（`:danger`）

| 状态 | 前景色 | 图标色 | 背景色 |
|---|---|---|---|
| 正常 | `ColorError` | `ColorError` | 透明 |
| 悬浮（`:pointerover`） | `ColorErrorHover` | `ColorErrorHover` | 透明 |
| 按下（`:pressed`） | `ColorErrorActive` | `ColorErrorActive` | 透明 |
| 禁用（`:disabled`） | `ColorTextDisabled` | `ColorTextDisabled` | 透明 |

---

## 4. 通过 ControlTheme 完全替换主题

```xml
<ControlTheme x:Key="MyCustomHyperLink" TargetType="atom:HyperLinkButton">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Padding="{TemplateBinding Padding}">
                <StackPanel Orientation="Horizontal" VerticalAlignment="Center">
                    <atom:IconPresenter Icon="{TemplateBinding Icon}"
                                        IsVisible="{TemplateBinding Icon, 
                                            Converter={x:Static ObjectConverters.IsNotNull}}" />
                    <ContentPresenter Content="{TemplateBinding Content}" />
                </StackPanel>
            </Border>
        </ControlTemplate>
    </Setter>
    <Setter Property="Foreground" Value="Blue" />
    <Setter Property="Cursor" Value="Hand" />
</ControlTheme>

<!-- 使用 -->
<atom:HyperLinkButton Theme="{StaticResource MyCustomHyperLink}"
                       NavigateUri="https://example.com">
    Custom Link
</atom:HyperLinkButton>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的加载图标切换、图标间距适配等功能。建议优先使用 Style 覆盖。

---

## 5. 控制动画行为

```xml
<!-- 禁用过渡动画 -->
<atom:HyperLinkButton IsMotionEnabled="False"
                       NavigateUri="https://example.com">
    No Animation
</atom:HyperLinkButton>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|HyperLinkButton` 语法引用 `atom` XML 命名空间下的 `HyperLinkButton` 类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|HyperLinkButton` | 匹配所有 HyperLinkButton 实例 |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|HyperLinkButton[SizeType=Large]` | 匹配大号链接按钮 |
| `atom\|HyperLinkButton[SizeType=Small]` | 匹配小号链接按钮 |
| `atom\|HyperLinkButton[IsDanger=True]` | 匹配危险链接按钮 |
| `atom\|HyperLinkButton[IsGhost=True]` | 匹配幽灵链接按钮 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|HyperLinkButton:visited` | 已访问状态（`IsVisited == true`） |
| `atom\|HyperLinkButton:icononly` | 仅图标模式（`Icon` 不为 null 且 `Content` 为 null） |
| `atom\|HyperLinkButton:loading` | 加载中状态（`IsLoading == true`） |
| `atom\|HyperLinkButton:danger` | 危险样式（`IsDanger == true`） |
| `atom\|HyperLinkButton:pointerover` | 鼠标悬浮 |
| `atom\|HyperLinkButton:pressed` | 按下 |
| `atom\|HyperLinkButton:disabled` | 禁用（`IsEnabled == false`） |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|HyperLinkButton /template/ Border#Frame` | 主框架 Border |
| `atom\|HyperLinkButton /template/ StackPanel#PART_MainInfoLayout` | 内容布局面板 |
| `atom\|HyperLinkButton /template/ atom\|IconPresenter#PART_ButtonIcon` | 按钮图标展示器 |
| `atom\|HyperLinkButton /template/ atom\|Icon#PART_LoadingIcon` | 加载旋转图标 |
| `atom\|HyperLinkButton /template/ ContentPresenter#PART_ContentPresenter` | 内容展示器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|HyperLinkButton:not(:disabled)` | 非禁用状态的链接按钮 |
| `atom\|HyperLinkButton:danger:not(:disabled):pointerover` | 悬浮状态的非禁用危险链接 |
| `atom\|HyperLinkButton:visited:not(:danger)` | 已访问但非危险的链接 |
| `atom\|HyperLinkButton[SizeType=Large]:icononly` | 大号仅图标链接按钮 |
