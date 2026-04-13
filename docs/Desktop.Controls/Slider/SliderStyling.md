# Slider 自定义样式指南

本文档说明如何通过 `Style`、`ControlTheme` 和 Design Token 自定义 Slider 控件的视觉表现。

> 📖 主题源码位置：
> - `src/AtomUI.Desktop.Controls/Slider/Themes/SliderTheme.axaml`
> - `src/AtomUI.Desktop.Controls/Slider/Themes/SliderThumbTheme.axaml`
> - `src/AtomUI.Desktop.Controls/Slider/Themes/SliderTrackTheme.axaml`

---

## 1. 使用属性直接控制

最常见的自定义方式是通过 Slider 自身的属性：

```xml
<!-- 基本滑动条 -->
<atom:Slider Minimum="0" Maximum="100" Value="50" />

<!-- 垂直方向 -->
<atom:Slider Orientation="Vertical" Minimum="0" Maximum="100" Value="30" Height="200" />

<!-- 范围选择 -->
<atom:Slider IsRangeMode="True" Minimum="0" Maximum="100" RangeValue="20, 80" />

<!-- 刻度吸附 -->
<atom:Slider TickFrequency="10" IsSnapToTickEnabled="True" Maximum="100" />

<!-- 自定义 Tooltip 格式 -->
<atom:Slider ValueFormatTemplate="\{0\}%" Maximum="100" Value="50" />

<!-- 不高亮覆盖部分 -->
<atom:Slider Included="False" Maximum="100" Value="50" />

<!-- 方向反转 -->
<atom:Slider IsDirectionReversed="True" Maximum="100" Value="70" />
```

---

## 2. 通过 Style 覆盖样式

### 全局 Slider 样式

```xml
<Window.Styles>
    <!-- 为所有 Slider 设置默认刻度吸附 -->
    <Style Selector="atom|Slider">
        <Setter Property="TickFrequency" Value="5" />
        <Setter Property="IsSnapToTickEnabled" Value="True" />
    </Style>
</Window.Styles>
```

### 按方向定制样式

```xml
<Window.Styles>
    <!-- 水平 Slider -->
    <Style Selector="atom|Slider:horizontal">
        <Setter Property="Margin" Value="0,8" />
    </Style>

    <!-- 垂直 Slider -->
    <Style Selector="atom|Slider:vertical">
        <Setter Property="Margin" Value="8,0" />
    </Style>
</Window.Styles>
```

### 禁用态样式

```xml
<Window.Styles>
    <Style Selector="atom|Slider:disabled">
        <Setter Property="Opacity" Value="0.5" />
    </Style>
</Window.Styles>
```

---

## 3. 模板内部样式覆盖

通过 `/template/` 选择器可以深入修改内部控件的样式：

### 修改轨道颜色

```xml
<Window.Styles>
    <!-- 自定义轨道底色 -->
    <Style Selector="atom|Slider /template/ atom|SliderTrack">
        <Setter Property="TrackGrooveBrush" Value="#E8E8E8" />
        <Setter Property="TrackBarBrush" Value="#52C41A" />
    </Style>
    
    <!-- 悬浮时的轨道颜色 -->
    <Style Selector="atom|Slider /template/ atom|SliderTrack:pointerover">
        <Setter Property="TrackGrooveBrush" Value="#D9D9D9" />
        <Setter Property="TrackBarBrush" Value="#73D13D" />
        <Setter Property="Cursor" Value="Hand" />
    </Style>
</Window.Styles>
```

### 修改滑块外观

```xml
<Window.Styles>
    <!-- 默认态滑块 -->
    <Style Selector="atom|Slider /template/ atom|SliderThumb">
        <Setter Property="BorderBrush" Value="#52C41A" />
    </Style>
    
    <!-- 悬浮态滑块 -->
    <Style Selector="atom|Slider /template/ atom|SliderThumb:pointerover">
        <Setter Property="BorderBrush" Value="#73D13D" />
    </Style>
    
    <!-- 聚焦态滑块 -->
    <Style Selector="atom|Slider /template/ atom|SliderThumb:focus">
        <Setter Property="BorderBrush" Value="#389E0D" />
    </Style>
</Window.Styles>
```

---

## 4. 默认 ControlTheme 结构解析

### SliderTheme.axaml

Slider 的默认主题定义在 `SliderTheme.axaml` 中，核心结构：

```xml
<ControlTheme x:Key="{x:Type atom:Slider}" TargetType="atom:Slider">
    <Setter Property="Template">
        <ControlTemplate>
            <atom:SliderTrack Name="PART_Track"
                              IsMotionEnabled="{TemplateBinding IsMotionEnabled}"
                              Minimum="{TemplateBinding Minimum}"
                              Maximum="{TemplateBinding Maximum}"
                              Value="{TemplateBinding Value, Mode=TwoWay}"
                              ...>
                <atom:SliderTrack.StartSliderThumb>
                    <atom:SliderThumb Name="PART_StartThumb" ... />
                </atom:SliderTrack.StartSliderThumb>
                <atom:SliderTrack.EndSliderThumb>
                    <atom:SliderThumb Name="PART_EndThumb" ... />
                </atom:SliderTrack.EndSliderThumb>
            </atom:SliderTrack>
        </ControlTemplate>
    </Setter>
    <!-- 状态样式选择器 -->
</ControlTheme>
```

### SliderThumbTheme.axaml

SliderThumb 的主题通过 Token 资源配置各状态的视觉效果：

| 状态 | 圆形尺寸 | 边框粗细 | 边框颜色 | Outline |
|---|---|---|---|---|
| 默认 | `ThumbCircleSize` | `ThumbCircleBorderThickness` | `ThumbCircleBorderColor` | 无 |
| `:pointerover` | `ThumbCircleSizeHover` | `ThumbCircleBorderThicknessHover` | `ThumbCircleBorderActiveColor` | `ThumbOutlineThickness` |
| `:focus` | `ThumbCircleSizeHover` | `ThumbCircleBorderThicknessHover` | `ThumbCircleBorderActiveColor` | `ThumbOutlineThickness` |

### SliderTrackTheme.axaml

SliderTrack 的主题配置轨道尺寸和刻度标记外观：

| 属性 | Token 来源 | 说明 |
|---|---|---|
| `SliderTrackSize` | `SliderToken.SliderTrackSize` | 轨道控件总高度 |
| `SliderMarkSize` | `SliderToken.MarkSize` | 刻度标记大小 |
| `SliderRailSize` | `SliderToken.RailSize` | 轨道条高度 |
| `MarkBackgroundBrush` | `SharedToken.ColorBgElevated` | 刻度标记背景色 |
| `MarkBorderThickness` | `SliderToken.ThumbCircleBorderThickness` | 刻度标记边框粗细 |
| `MarkLabelBrush` | `SharedToken.ColorText`（禁用时 `ColorTextDisabled`） | 标签文字颜色 |

---

## 5. 样式选择器速查

### Slider 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Slider` | 匹配所有 Slider |
| `atom\|Slider:horizontal` | 水平方向 |
| `atom\|Slider:vertical` | 垂直方向 |
| `atom\|Slider:pressed` | 拖拽中 |
| `atom\|Slider:disabled` | 禁用态 |
| `atom\|Slider:pointerover` | 鼠标悬浮 |

### 模板内部选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Slider /template/ atom\|SliderTrack` | 轨道控件 |
| `atom\|Slider /template/ atom\|SliderTrack:pointerover` | 轨道悬浮态 |
| `atom\|Slider /template/ atom\|SliderThumb` | 所有滑块 |
| `atom\|Slider /template/ atom\|SliderThumb:pointerover` | 滑块悬浮态 |
| `atom\|Slider /template/ atom\|SliderThumb:focus` | 滑块聚焦态 |
| `atom\|Slider /template/ atom\|SliderThumb:pressed` | 滑块拖拽中 |
| `atom\|Slider /template/ atom\|SliderTrack[Orientation=Horizontal]` | 水平轨道 |
| `atom\|Slider /template/ atom\|SliderTrack[Orientation=Vertical]` | 垂直轨道 |

### 复合状态选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Slider:pointerover /template/ atom\|SliderThumb[IsFocused=False]:not(:pointerover)` | Slider 悬浮时未聚焦且未悬浮的滑块 |
| `atom\|Slider:disabled /template/ atom\|SliderTrack` | 禁用态轨道 |
| `atom\|Slider:disabled /template/ atom\|SliderThumb` | 禁用态滑块 |

---

## 6. 注意事项

- **SliderThumb 是自绘控件**：`SliderThumb` 通过重写 `Render()` 绘制圆形和 outline 环，不使用控件模板。因此无法通过替换模板来修改其外观，只能通过属性（`BorderBrush`、`OutlineBrush`、`ThumbCircleSize` 等）控制。
- **SliderTrack 是自绘控件**：轨道底层、覆盖部分、刻度标记点和标签文字均通过自绘渲染，只能通过属性（`TrackBarBrush`、`TrackGrooveBrush`、`MarkLabelBrush` 等）控制。
- **Token 优先**：建议优先通过修改 Design Token 全局调整 Slider 外观，而非逐个覆盖样式选择器。
- **动画控制**：`IsMotionEnabled` 控制 SliderThumb 的过渡动画是否生效。禁用动画后，状态切换会立即生效而无平滑过渡。
