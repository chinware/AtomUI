# Button 自定义样式指南

Button 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Button 的公共属性来控制外观：

```xml
<!-- 不同类型 -->
<atom:Button ButtonType="Primary">主按钮</atom:Button>
<atom:Button ButtonType="Dashed">虚线按钮</atom:Button>
<atom:Button ButtonType="Text">文本按钮</atom:Button>
<atom:Button ButtonType="Link">链接按钮</atom:Button>

<!-- 不同形状 -->
<atom:Button ButtonType="Primary" Shape="Round">胶囊按钮</atom:Button>
<atom:Button ButtonType="Primary" Shape="Circle"
             Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}" />

<!-- 不同尺寸 -->
<atom:Button ButtonType="Primary" SizeType="Large">大按钮</atom:Button>
<atom:Button ButtonType="Primary" SizeType="Small">小按钮</atom:Button>

<!-- 危险 + 幽灵 -->
<atom:Button ButtonType="Primary" IsDanger="True" IsGhost="True">危险幽灵</atom:Button>

<!-- 带图标 -->
<atom:Button ButtonType="Primary"
             Icon="{antdicons:AntDesignIconProvider Kind=DownloadOutlined}">
    Download
</atom:Button>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/ButtonShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Button 进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|Button">
        <Setter Property="Margin" Value="5" />
    </Style>
</Window.Styles>
```

### 按类型定制颜色

```xml
<Style Selector="atom|Button[ButtonType=Primary]:not(:disabled)">
    <Setter Property="Background" Value="#722ed1" />
</Style>
<Style Selector="atom|Button[ButtonType=Primary]:not(:disabled):pointerover">
    <Setter Property="Background" Value="#9254de" />
</Style>
<Style Selector="atom|Button[ButtonType=Primary]:not(:disabled):pressed">
    <Setter Property="Background" Value="#531dab" />
</Style>
```

### 使用伪类选择器

```xml
<!-- 仅图标按钮添加额外内边距 -->
<Style Selector="atom|Button:icononly">
    <Setter Property="Padding" Value="8" />
</Style>

<!-- 加载中时隐藏特定内容 -->
<Style Selector="atom|Button:loading">
    <Setter Property="Opacity" Value="0.65" />
</Style>

<!-- 危险按钮的自定义样式 -->
<Style Selector="atom|Button:danger:not(:disabled)">
    <Setter Property="FontWeight" Value="Bold" />
</Style>
```

### 通过属性选择器定制形状

```xml
<!-- 所有圆形按钮的额外样式 -->
<Style Selector="atom|Button[Shape=Circle]">
    <Setter Property="MinWidth" Value="40" />
</Style>

<!-- 所有大号按钮使用粗体 -->
<Style Selector="atom|Button[SizeType=Large]">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Button 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomButton" TargetType="atom:Button">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    Padding="{TemplateBinding Padding}">
                <ContentPresenter Content="{TemplateBinding Content}"
                                  HorizontalAlignment="Center"
                                  VerticalAlignment="Center" />
            </Border>
        </ControlTemplate>
    </Setter>
    <!-- 自定义样式... -->
</ControlTheme>

<!-- 使用 -->
<atom:Button Theme="{StaticResource MyCustomButton}">自定义按钮</atom:Button>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的波纹效果、加载图标等功能。建议优先使用 Style 覆盖。

---

## 4. Block 按钮（撑满父容器）

Ant Design 的 `block` 属性在 AtomUI 中通过 Avalonia 原生布局属性实现：

```xml
<StackPanel>
    <atom:Button ButtonType="Primary" HorizontalAlignment="Stretch">
        Primary Block
    </atom:Button>
    <atom:Button ButtonType="Default" HorizontalAlignment="Stretch">
        Default Block
    </atom:Button>
    <atom:Button ButtonType="Dashed" HorizontalAlignment="Stretch">
        Dashed Block
    </atom:Button>
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/ButtonShowCase.axaml` 中 "Block Button" 示例。

---

## 5. 加载状态交互示例

```xml
<atom:Button ButtonType="Primary"
             Click="HandleLoadingBtnClick"
             Icon="{antdicons:AntDesignIconProvider Kind=PoweroffOutlined}">
    Click me!
</atom:Button>
```

```csharp
public void HandleLoadingBtnClick(object? sender, RoutedEventArgs args)
{
    if (sender is Button button)
    {
        button.IsLoading = true;
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            button.IsLoading = false;
        });
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/ButtonShowCase.axaml.cs`

---

## 6. 幽灵按钮（Ghost）使用场景

幽灵按钮常用于有色背景上：

```xml
<Border Background="rgb(190, 200, 200)" Padding="10">
    <WrapPanel>
        <atom:Button ButtonType="Primary" IsGhost="True">Primary</atom:Button>
        <atom:Button ButtonType="Default" IsGhost="True">Default</atom:Button>
        <atom:Button ButtonType="Dashed" IsGhost="True">Dashed</atom:Button>
        <atom:Button ButtonType="Primary" IsDanger="True" IsGhost="True">Danger</atom:Button>
    </WrapPanel>
</Border>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/ButtonShowCase.axaml` 中 "Ghost Button" 示例。

---

## 7. 控制动画行为

```xml
<!-- 禁用过渡动画 -->
<atom:Button ButtonType="Primary" IsMotionEnabled="False">无动画</atom:Button>

<!-- 禁用点击波纹 -->
<atom:Button ButtonType="Primary" IsWaveSpiritEnabled="False">无波纹</atom:Button>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Button` 语法引用 `atom` XML 命名空间下的 `Button` 类型，其中 `|` 是命名空间分隔符。

### 按类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|Button` | 匹配所有 AtomUI Button 实例，用于设置全局通用样式（如统一 Margin、Cursor 等） |
| `atom\|Button[ButtonType=Primary]` | 匹配主按钮（实心填充主色调背景），可定制主按钮的背景色、文字色等 |
| `atom\|Button[ButtonType=Default]` | 匹配默认按钮（白色背景 + 实线边框），最常见的次级操作按钮 |
| `atom\|Button[ButtonType=Dashed]` | 匹配虚线按钮（虚线边框），通常用于「添加」类操作 |
| `atom\|Button[ButtonType=Text]` | 匹配文本按钮（无边框无背景），适用于最次级的内联操作 |
| `atom\|Button[ButtonType=Link]` | 匹配链接按钮（文字呈链接色，无边框），用于导航类操作 |

### 按形状选择

| 选择器 | 说明 |
|---|---|
| `atom\|Button[Shape=Circle]` | 匹配圆形按钮（宽 = 高，完全圆角），通常仅含图标 |
| `atom\|Button[Shape=Round]` | 匹配胶囊形按钮（两端完全圆角，宽度 ≥ 高度 × 1.5） |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|Button[SizeType=Large]` | 匹配大号按钮（高度 = `ControlHeightLG`，字号 = `ContentFontSizeLG`） |
| `atom\|Button[SizeType=Middle]` | 匹配中号按钮（默认尺寸，通常无需单独匹配） |
| `atom\|Button[SizeType=Small]` | 匹配小号按钮（高度 = `ControlHeightSM`，字号 = `ContentFontSizeSM`） |

### 按状态属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|Button[IsDanger=True]` | 匹配危险按钮（红色系样式），可结合类型如 `atom\|Button[ButtonType=Primary][IsDanger=True]` |
| `atom\|Button[IsGhost=True]` | 匹配幽灵按钮（背景透明），常用于有色背景场景 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|Button:icononly` | 匹配仅含图标的按钮（`Icon` 不为 null 且 `Content` 为 null），可用于调整仅图标模式下的内间距和图标尺寸 |
| `atom\|Button:loading` | 匹配处于加载中状态的按钮（`IsLoading == true`），此时加载旋转图标可见、原图标隐藏 |
| `atom\|Button:danger` | 匹配标记为危险的按钮（`IsDanger == true`），与 `[IsDanger=True]` 属性选择器等效 |
| `atom\|Button:pointerover` | 匹配鼠标悬浮状态，用于定制 hover 时的背景色、文字色、边框色变化 |
| `atom\|Button:pressed` | 匹配按下状态（鼠标按住未释放），用于定制 active 时的视觉反馈 |
| `atom\|Button:disabled` | 匹配禁用状态（`IsEnabled == false`），此时按钮不响应交互，使用灰色调 |
| `atom\|Button:focus-visible` | 匹配通过键盘（Tab 键）获得焦点的状态，显示外围焦点指示框 |
| `atom\|Button:focus` | 匹配获得焦点的状态（包含鼠标点击和键盘），通常优先使用 `:focus-visible` |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Button[ButtonType=Primary]:not(:disabled)` | 非禁用状态的主按钮，用于定制可交互态的样式 |
| `atom\|Button[ButtonType=Default]:danger:pointerover` | 悬浮状态的危险默认按钮 |
| `atom\|Button[SizeType=Large]:icononly` | 大号仅图标按钮 |
| `atom\|Button /template/ Border#Frame` | 访问按钮模板内名为 `Frame` 的 Border 部件 |
| `atom\|Button /template/ atom\|IconPresenter#PART_ButtonIcon` | 访问模板内的图标展示器部件 |

