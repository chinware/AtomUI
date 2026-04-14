# TimePicker 自定义样式指南

TimePicker 和 RangeTimePicker 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 TimePicker 的公共属性来控制外观：

```xml
<!-- 不同时钟格式 -->
<atom:TimePicker PlaceholderText="Select time" ClockIdentifier="HourClock24" />
<atom:TimePicker PlaceholderText="Select time" ClockIdentifier="HourClock12" />

<!-- 不同尺寸 -->
<atom:TimePicker PlaceholderText="Select time" SizeType="Large" />
<atom:TimePicker PlaceholderText="Select time" SizeType="Middle" />
<atom:TimePicker PlaceholderText="Select time" SizeType="Small" />

<!-- 不同样式变体 -->
<atom:TimePicker PlaceholderText="Outline" StyleVariant="Outline" />
<atom:TimePicker PlaceholderText="Filled" StyleVariant="Filled" />
<atom:TimePicker PlaceholderText="Borderless" StyleVariant="Borderless" />

<!-- 验证状态 -->
<atom:TimePicker PlaceholderText="Select time" Status="Warning" />
<atom:TimePicker PlaceholderText="Select time" Status="Error" />

<!-- 确认模式 + 步进 -->
<atom:TimePicker PlaceholderText="Select time" IsNeedConfirm="True"
                 MinuteIncrement="15" SecondIncrement="10" />

<!-- 默认值 -->
<atom:TimePicker PlaceholderText="Select time" DefaultTime="12:08:23" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TimePickerShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 TimePicker 进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|TimePicker">
        <Setter Property="Margin" Value="5" />
    </Style>
</Window.Styles>
```

### 按状态定制样式

```xml
<!-- 禁用态的自定义透明度 -->
<Style Selector="atom|TimePicker:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>

<!-- 弹出面板打开时的样式 -->
<Style Selector="atom|TimePicker:flyout-open">
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorPrimary}" />
</Style>
```

### 按尺寸定制

```xml
<!-- 大号时间选择器使用粗体 -->
<Style Selector="atom|TimePicker[SizeType=Large]">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

TimePicker 的 ControlTheme 基于 `InfoPickerInputTheme`。如果需要彻底替换，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomTimePicker" TargetType="atom:TimePicker">
    <ControlTheme.BasedOn>
        <atom:InfoPickerInputTheme TargetType="atom:TimePicker" />
    </ControlTheme.BasedOn>
    <!-- 自定义样式设置 -->
    <Setter Property="Margin" Value="8" />
</ControlTheme>

<!-- 使用 -->
<atom:TimePicker Theme="{StaticResource MyCustomTimePicker}"
                 PlaceholderText="Custom" />
```

> ⚠️ 注意：完全替换 ControlTheme 需要谨慎处理基类主题的依赖。建议优先使用 Style 覆盖方式。

---

## 4. RangeTimePicker 样式

RangeTimePicker 的样式方式与 TimePicker 类似：

```xml
<!-- 基本使用 -->
<atom:RangeTimePicker PlaceholderText="Start time"
                      SecondaryPlaceholderText="End time" />

<!-- 不同样式变体 -->
<atom:RangeTimePicker StyleVariant="Outline"
                      PlaceholderText="Start time"
                      SecondaryPlaceholderText="End time" />
<atom:RangeTimePicker StyleVariant="Filled"
                      PlaceholderText="Start time"
                      SecondaryPlaceholderText="End time" />
<atom:RangeTimePicker StyleVariant="Borderless"
                      PlaceholderText="Start time"
                      SecondaryPlaceholderText="End time" />

<!-- 带默认值 -->
<atom:RangeTimePicker PlaceholderText="Start time"
                      SecondaryPlaceholderText="End time"
                      RangeStartDefaultTime="10:09:20"
                      RangeEndDefaultTime="12:12:20" />
```

---

## 5. 控制动画行为

```xml
<!-- 禁用过渡动画 -->
<atom:TimePicker PlaceholderText="Select time" IsMotionEnabled="False" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|TimePicker` 语法引用 `atom` XML 命名空间下的 `TimePicker` 类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|TimePicker` | 匹配所有 TimePicker 实例 |
| `atom\|RangeTimePicker` | 匹配所有 RangeTimePicker 实例 |

### 按样式变体选择

| 选择器 | 说明 |
|---|---|
| `atom\|TimePicker[StyleVariant=Outline]` | 匹配 Outline 变体的 TimePicker |
| `atom\|TimePicker[StyleVariant=Filled]` | 匹配 Filled 变体的 TimePicker |
| `atom\|TimePicker[StyleVariant=Borderless]` | 匹配 Borderless 变体的 TimePicker |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|TimePicker[SizeType=Large]` | 匹配大号 TimePicker |
| `atom\|TimePicker[SizeType=Middle]` | 匹配中号 TimePicker（默认） |
| `atom\|TimePicker[SizeType=Small]` | 匹配小号 TimePicker |

### 按状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|TimePicker[Status=Warning]` | 匹配警告状态 |
| `atom\|TimePicker[Status=Error]` | 匹配错误状态 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|TimePicker:pointerover` | 鼠标悬浮状态 |
| `atom\|TimePicker:disabled` | 禁用状态 |
| `atom\|TimePicker:focus` | 获得焦点状态 |
| `atom\|TimePicker:focus-visible` | 通过键盘获得焦点 |
| `atom\|TimePicker:flyout-open` | 弹出面板打开状态 |
| `atom\|TimePicker:choosing` | 正在选择中状态 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|TimePicker[StyleVariant=Outline]:not(:disabled)` | 非禁用的 Outline 样式 TimePicker |
| `atom\|TimePicker[SizeType=Large][Status=Error]` | 大号错误状态的 TimePicker |
| `atom\|RangeTimePicker:flyout-open` | 弹出面板打开中的 RangeTimePicker |
| `atom\|TimePicker /template/ TextBox#PART_InfoInputBox` | 访问模板内的时间输入框部件 |
