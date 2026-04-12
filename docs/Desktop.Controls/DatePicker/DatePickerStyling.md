# DatePicker 自定义样式指南

DatePicker 和 RangeDatePicker 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过控件的公共属性来控制外观：

```xml
<!-- 基础日期选择 -->
<atom:DatePicker PlaceholderText="Select date" />

<!-- 范围日期选择 -->
<atom:RangeDatePicker PlaceholderText="Start date"
                      SecondaryPlaceholderText="End date" />

<!-- 不同样式变体 -->
<atom:DatePicker StyleVariant="Outline" PlaceholderText="Outline" />
<atom:DatePicker StyleVariant="Filled" PlaceholderText="Filled" />
<atom:DatePicker StyleVariant="Borderless" PlaceholderText="Borderless" />

<!-- 不同验证状态 -->
<atom:DatePicker Status="Warning" PlaceholderText="Warning" />
<atom:DatePicker Status="Error" PlaceholderText="Error" />

<!-- 不同尺寸 -->
<atom:DatePicker SizeType="Large" PlaceholderText="Large" />
<atom:DatePicker SizeType="Middle" PlaceholderText="Default" />
<atom:DatePicker SizeType="Small" PlaceholderText="Small" />

<!-- 日期+时间选择 -->
<atom:DatePicker IsShowTime="True" IsNeedConfirm="True"
                 PlaceholderText="Select date and time" />

<!-- 12小时制 -->
<atom:DatePicker IsShowTime="True" ClockIdentifier="HourClock12"
                 PlaceholderText="Select date (12h)" />

<!-- 带默认值 -->
<atom:DatePicker DefaultDateTime="2024-01-20" PlaceholderText="Select date" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/DatePickerShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 DatePicker 进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|DatePicker">
        <Setter Property="Margin" Value="0,5" />
    </Style>
</Window.Styles>
```

### 按状态定制样式

```xml
<!-- 自定义警告状态下的外观 -->
<Style Selector="atom|DatePicker[Status=Warning]">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>

<!-- 自定义错误状态下的外观 -->
<Style Selector="atom|DatePicker[Status=Error]">
    <Setter Property="FontWeight" Value="Bold" />
</Style>
```

### 按尺寸定制样式

```xml
<!-- 大号日期选择器使用粗体 -->
<Style Selector="atom|DatePicker[SizeType=Large]">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

### 使用伪类选择器

```xml
<!-- Flyout 打开时的边框样式 -->
<Style Selector="atom|DatePicker:flyout-open">
    <Setter Property="Opacity" Value="1" />
</Style>

<!-- 禁用状态的自定义样式 -->
<Style Selector="atom|DatePicker:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 DatePicker 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomDatePicker" TargetType="atom:DatePicker">
    <ControlTheme.BasedOn>
        <atom:InfoPickerInputTheme TargetType="atom:DatePicker" />
    </ControlTheme.BasedOn>
    <!-- 自定义样式覆盖... -->
</ControlTheme>

<!-- 使用 -->
<atom:DatePicker Theme="{StaticResource MyCustomDatePicker}"
                 PlaceholderText="Custom DatePicker" />
```

> ⚠️ 注意：DatePicker 的主题基于 `InfoPickerInputTheme`，建议通过 `BasedOn` 继承后进行局部修改，而非完全重写模板。完全替换可能导致 Flyout 弹出、清除按钮等内置功能失效。

---

## 4. 弹出方向控制

通过 `PickerPlacement` 属性控制日历面板的弹出方向：

```xml
<!-- 默认：底部左对齐 -->
<atom:DatePicker PickerPlacement="BottomLeft" PlaceholderText="Select date" />

<!-- 顶部右对齐 -->
<atom:DatePicker PickerPlacement="TopRight" PlaceholderText="Select date" />
```

还可以通过数据绑定动态切换弹出方向：

```xml
<atom:DatePicker PickerPlacement="{Binding PickerPlacement}"
                 PlaceholderText="Select date" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/DatePickerShowCase.axaml` 中 "Placement" 示例。

---

## 5. 样式变体与状态组合

DatePicker 支持三种样式变体与三种验证状态的自由组合：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- Outline + Error -->
    <atom:DatePicker StyleVariant="Outline" Status="Error"
                     PlaceholderText="Outline Error" />

    <!-- Filled + Warning -->
    <atom:DatePicker StyleVariant="Filled" Status="Warning"
                     PlaceholderText="Filled Warning" />

    <!-- Borderless + Default -->
    <atom:DatePicker StyleVariant="Borderless" Status="Default"
                     PlaceholderText="Borderless" />
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/DatePickerShowCase.axaml` 中 "Variants" 和 "Status" 示例。

---

## 6. 紧凑空间中使用

DatePicker 和 RangeDatePicker 均支持在 `Space.Compact` 容器中使用：

```xml
<atom:Space Compact="True">
    <atom:DatePicker atom:CompactSpace.ItemSize="5*" />
    <atom:Button ButtonType="Primary">查询</atom:Button>
    <atom:RangeDatePicker atom:CompactSpace.ItemSize="7*" />
</atom:Space>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Layout/SpaceShowCase.axaml` 中紧凑空间示例。

---

## 7. 控制动画行为

```xml
<!-- 禁用过渡动画 -->
<atom:DatePicker IsMotionEnabled="False" PlaceholderText="No animation" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|DatePicker` 语法引用 `atom` XML 命名空间下的 `DatePicker` 类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|DatePicker` | 匹配所有单日期选择器 |
| `atom\|RangeDatePicker` | 匹配所有范围日期选择器 |

### 按样式变体选择

| 选择器 | 说明 |
|---|---|
| `atom\|DatePicker[StyleVariant=Outline]` | 匹配轮廓样式日期选择器 |
| `atom\|DatePicker[StyleVariant=Filled]` | 匹配填充样式日期选择器 |
| `atom\|DatePicker[StyleVariant=Borderless]` | 匹配无边框样式日期选择器 |

### 按状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|DatePicker[Status=Warning]` | 匹配警告状态日期选择器 |
| `atom\|DatePicker[Status=Error]` | 匹配错误状态日期选择器 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|DatePicker[SizeType=Large]` | 匹配大号日期选择器 |
| `atom\|DatePicker[SizeType=Middle]` | 匹配中号日期选择器（默认尺寸） |
| `atom\|DatePicker[SizeType=Small]` | 匹配小号日期选择器 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|DatePicker:pointerover` | 鼠标悬浮状态 |
| `atom\|DatePicker:disabled` | 禁用状态（`IsEnabled == false`） |
| `atom\|DatePicker:focus` | 获得焦点状态 |
| `atom\|DatePicker:focus-visible` | 通过键盘获得焦点 |
| `atom\|DatePicker:flyout-open` | Flyout 面板打开状态 |
| `atom\|DatePicker:choosing` | 用户正在面板中选择日期 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|DatePicker[StyleVariant=Filled]:not(:disabled)` | 非禁用的填充样式日期选择器 |
| `atom\|DatePicker[Status=Error]:flyout-open` | 错误状态且 Flyout 打开 |
| `atom\|RangeDatePicker[SizeType=Large]` | 大号范围日期选择器 |
| `atom\|DatePicker:disabled:pointerover` | 禁用状态下鼠标悬浮 |
