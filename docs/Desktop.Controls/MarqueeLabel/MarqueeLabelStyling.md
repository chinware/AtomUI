# MarqueeLabel 自定义样式指南

MarqueeLabel 的视觉表现主要通过属性和 Design Token 系统控制。由于 MarqueeLabel 继承自 `TextBlock`（非 `TemplatedControl`），它没有 `ControlTheme` 模板，样式自定义主要通过属性设置和 `Style` 选择器实现。

---

## 1. 使用属性直接控制

最简单的方式是通过 MarqueeLabel 的公共属性来控制外观和行为：

```xml
<!-- 基本用法 -->
<atom:MarqueeLabel Text="这是一段很长的文本，当超出容器宽度时会自动滚动" />

<!-- 自定义滚动速度 -->
<atom:MarqueeLabel Text="快速滚动文本" MoveSpeed="300" />

<!-- 自定义循环间隔 -->
<atom:MarqueeLabel Text="宽间距循环文本" CycleSpace="400" />

<!-- 同时自定义速度和间隔 -->
<atom:MarqueeLabel Text="慢速滚动 + 紧凑间距" MoveSpeed="80" CycleSpace="50" />
```

**控制文本样式（继承自 TextBlock）：**

```xml
<!-- 自定义字体和颜色 -->
<atom:MarqueeLabel Text="自定义样式的滚动文本"
                   FontSize="16"
                   FontWeight="Bold"
                   Foreground="DarkBlue" />

<!-- 自定义字体族 -->
<atom:MarqueeLabel Text="Scrolling text with custom font"
                   FontFamily="Consolas"
                   FontSize="14" />
```

> 📖 间接示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/AlertShowCase.axaml` 中 "Loop Banner" 示例展示了 MarqueeLabel 在 Alert 中的效果。

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 MarqueeLabel 进行全局或局部样式覆盖：

### 全局统一样式

```xml
<Window.Styles>
    <!-- 所有 MarqueeLabel 使用统一字号和前景色 -->
    <Style Selector="atom|MarqueeLabel">
        <Setter Property="FontSize" Value="14" />
        <Setter Property="Foreground" Value="#333333" />
    </Style>
</Window.Styles>
```

### 自定义滚动参数

```xml
<Window.Styles>
    <!-- 全局调慢滚动速度 -->
    <Style Selector="atom|MarqueeLabel">
        <Setter Property="MoveSpeed" Value="100" />
        <Setter Property="CycleSpace" Value="150" />
    </Style>
</Window.Styles>
```

### 按父容器定制

```xml
<!-- 在特定容器内的 MarqueeLabel 使用不同样式 -->
<Border Classes="notice-bar">
    <Border.Styles>
        <Style Selector="atom|MarqueeLabel">
            <Setter Property="Foreground" Value="#FF6600" />
            <Setter Property="FontWeight" Value="SemiBold" />
            <Setter Property="MoveSpeed" Value="120" />
        </Style>
    </Border.Styles>
    <atom:MarqueeLabel Text="重要公告：这是一条需要特别关注的滚动通知消息" />
</Border>
```

### 悬浮时的样式变化

```xml
<Window.Styles>
    <!-- 鼠标悬浮时改变文本颜色（动画已自动暂停） -->
    <Style Selector="atom|MarqueeLabel:pointerover">
        <Setter Property="Foreground" Value="#1677ff" />
        <Setter Property="Cursor" Value="Hand" />
    </Style>
</Window.Styles>
```

---

## 3. 在 Alert 中使用（内部集成示例）

MarqueeLabel 目前最主要的使用场景是 Alert 控件的消息滚动功能。Alert 通过 `IsMessageMarqueEnabled` 属性控制是否启用跑马灯：

```xml
<!-- Alert 中的跑马灯用法 -->
<atom:Alert Type="Warning" IsShowIcon="True" IsMessageMarqueEnabled="True">
    I can be a React component, multiple React components, or just some text,
    Info Description Info Description Info Description Info Description
</atom:Alert>
```

Alert 的 ControlTheme 中对 MarqueeLabel 的样式设置参考：

```xml
<!-- Alert 模板中嵌入 MarqueeLabel -->
<atom:MarqueeLabel Name="MarqueeLabel"
                   HorizontalAlignment="Stretch"
                   Padding="0"
                   IsVisible="{TemplateBinding IsMessageMarqueEnabled}"
                   Text="{TemplateBinding Message}" />

<!-- Alert 主题中根据是否有描述设置字号 -->
<Style Selector="^:has-description">
    <Style Selector="^ /template/ atom|MarqueeLabel#MarqueeLabel">
        <Setter Property="TemplatedControl.FontSize"
                Value="{atom:SharedTokenResource FontSizeLG}" />
    </Style>
</Style>
<Style Selector="^:not(:has-description)">
    <Style Selector="^ /template/ atom|MarqueeLabel#MarqueeLabel">
        <Setter Property="TemplatedControl.FontSize"
                Value="{atom:SharedTokenResource FontSize}" />
    </Style>
</Style>
```

> 📖 参考源码：`src/AtomUI.Desktop.Controls/Alert/Themes/AlertTheme.axaml`

---

## 4. 容器宽度约束

MarqueeLabel 只有在文本宽度**超出容器宽度**时才会启动滚动。因此，必须确保控件有明确的宽度约束：

```xml
<!-- ✅ 正确：在有限宽度容器中使用 -->
<Border Width="300">
    <atom:MarqueeLabel Text="这段文字超出 300 像素宽度时会自动滚动" />
</Border>

<!-- ✅ 正确：利用 Stretch 占满父容器（默认行为） -->
<StackPanel Width="400">
    <atom:MarqueeLabel Text="在 StackPanel 中自动占满宽度" />
</StackPanel>

<!-- ❌ 避免：无宽度约束，文本永远不会溢出 -->
<Canvas>
    <atom:MarqueeLabel Text="这里没有宽度约束，不会滚动" />
</Canvas>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|MarqueeLabel` 语法引用 `atom` XML 命名空间下的 `MarqueeLabel` 类型，其中 `|` 是命名空间分隔符。

### 基本选择器

| 选择器 | 说明 |
|---|---|
| `atom\|MarqueeLabel` | 匹配所有 MarqueeLabel 实例，用于设置全局通用样式 |
| `atom\|MarqueeLabel:pointerover` | 鼠标悬浮状态（此时滚动动画暂停），可用于改变文本颜色或光标样式 |
| `atom\|MarqueeLabel:disabled` | 禁用状态（`IsEnabled == false`） |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|MarqueeLabel[MoveSpeed=300]` | 匹配指定滚动速度的 MarqueeLabel |
| `atom\|MarqueeLabel[FontWeight=Bold]` | 匹配粗体文本的 MarqueeLabel |

### 嵌套选择器（在其他控件模板中）

| 选择器 | 说明 |
|---|---|
| `atom\|Alert /template/ atom\|MarqueeLabel` | 匹配 Alert 模板中的 MarqueeLabel |
| `atom\|Alert /template/ atom\|MarqueeLabel#MarqueeLabel` | 精确匹配 Alert 模板中名为 `MarqueeLabel` 的实例 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|MarqueeLabel:not(:disabled)` | 非禁用状态的 MarqueeLabel |
| `atom\|MarqueeLabel:pointerover:not(:disabled)` | 悬浮且非禁用状态 |
