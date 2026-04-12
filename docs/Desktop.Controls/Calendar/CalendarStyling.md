# Calendar 自定义样式指南

Calendar 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Calendar 的公共属性来控制外观和行为：

```xml
<!-- 基本日历 -->
<atom:Calendar />

<!-- 关闭今日高亮 -->
<atom:Calendar IsTodayHighlighted="False" />

<!-- 设置一周起始日为周一 -->
<atom:Calendar FirstDayOfWeek="Monday" />

<!-- 自定义头部背景色 -->
<atom:Calendar HeaderBackground="#F0F5FF" />

<!-- 指定初始显示日期 -->
<atom:Calendar DisplayDate="2025/06/01" />

<!-- 不同选择模式 -->
<atom:Calendar SelectionMode="SingleDate" />
<atom:Calendar SelectionMode="SingleRange" />
<atom:Calendar SelectionMode="MultipleRange" />
<atom:Calendar SelectionMode="None" />

<!-- 限制日期范围 -->
<atom:Calendar DisplayDateStart="2025/01/01" DisplayDateEnd="2025/12/31" />

<!-- 禁用过渡动画 -->
<atom:Calendar IsMotionEnabled="False" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CalendarShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Calendar 及其子控件进行全局或局部样式覆盖：

### 全局统一外观

```xml
<Window.Styles>
    <!-- 所有 Calendar 统一边框和圆角 -->
    <Style Selector="atom|Calendar">
        <Setter Property="BorderBrush" Value="#D9D9D9" />
        <Setter Property="CornerRadius" Value="8" />
    </Style>
</Window.Styles>
```

### 定制日期按钮样式

日期按钮（`BaseCalendarDayButton`）是日历中最核心的可交互元素，支持通过伪类选择器进行精细定制：

```xml
<!-- 今日日期使用自定义边框色 -->
<Style Selector="atom|BaseCalendarDayButton:today">
    <Setter Property="BorderBrush" Value="#722ED1" />
</Style>

<!-- 选中日期使用自定义背景色 -->
<Style Selector="atom|BaseCalendarDayButton:selected:not(:inactive)">
    <Setter Property="Background" Value="#722ED1" />
    <Setter Property="Foreground" Value="White" />
</Style>

<!-- 悬浮时的背景色 -->
<Style Selector="atom|BaseCalendarDayButton:pointerover">
    <Setter Property="Background" Value="#F0E6FF" />
</Style>

<!-- 非当前月份日期（溢出日期）的颜色 -->
<Style Selector="atom|BaseCalendarDayButton:inactive">
    <Setter Property="Foreground" Value="#BFBFBF" />
</Style>
```

### 定制月/年视图按钮样式

年/十年视图中的月份和年份按钮（`BaseCalendarButton`）也支持样式覆盖：

```xml
<!-- 选中月份/年份的背景色 -->
<Style Selector="atom|BaseCalendarButton:selected">
    <Setter Property="Background" Value="#722ED1" />
    <Setter Property="Foreground" Value="White" />
</Style>

<!-- 悬浮时的背景色 -->
<Style Selector="atom|BaseCalendarButton:pointerover">
    <Setter Property="Background" Value="#F0E6FF" />
</Style>
```

### 定制头部标题按钮样式

头部年月标题按钮（`HeadTextButton`）支持字体和颜色定制：

```xml
<!-- 标题文字加粗 + 自定义悬浮色 -->
<Style Selector="atom|HeadTextButton">
    <Setter Property="FontWeight" Value="Bold" />
</Style>

<Style Selector="atom|HeadTextButton:pointerover">
    <Setter Property="Foreground" Value="#722ED1" />
</Style>
```

### 定制导航图标按钮样式

头部的导航图标按钮（`IconButton`）可通过模板选择器定制：

```xml
<!-- 通过 CalendarItem 模板内部选择器定制导航按钮 -->
<Style Selector="atom|CalendarItem /template/ atom|IconButton">
    <Setter Property="IconBrush" Value="#8C8C8C" />
</Style>

<Style Selector="atom|CalendarItem /template/ atom|IconButton:pointerover">
    <Setter Property="IconBrush" Value="#1677FF" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Calendar 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomCalendar" TargetType="atom:Calendar">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}"
                    Padding="{TemplateBinding Padding}">
                <Panel Name="PART_Root" ClipToBounds="True">
                    <atom:CalendarItem Name="PART_CalendarItem"
                                       IsMotionEnabled="{TemplateBinding IsMotionEnabled}" />
                </Panel>
            </Border>
        </ControlTemplate>
    </Setter>
    <!-- 自定义样式... -->
</ControlTheme>

<!-- 使用 -->
<atom:Calendar Theme="{StaticResource MyCustomCalendar}" />
```

> ⚠️ 注意：完全替换 ControlTheme 需要保留 `PART_Root` 和 `PART_CalendarItem` 模板部件，否则日历功能将失效。

---

## 4. 禁用状态

```xml
<!-- 禁用后日历呈灰色调，不可交互 -->
<atom:Calendar IsEnabled="False" />
```

---

## 5. 控制动画行为

```xml
<!-- 禁用单元格悬浮过渡动画 -->
<atom:Calendar IsMotionEnabled="False" />
```

当 `IsMotionEnabled="False"` 时，日期按钮和月/年按钮在鼠标悬浮时不再有背景色渐变过渡效果。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Calendar` 语法引用 `atom` XML 命名空间下的类型，其中 `|` 是命名空间分隔符。

### Calendar 主控件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Calendar` | 匹配所有 Calendar 实例，用于设置全局通用样式 |
| `atom\|Calendar:disabled` | 匹配禁用状态的 Calendar |

### 日期按钮选择器（BaseCalendarDayButton）

| 选择器 | 说明 |
|---|---|
| `atom\|BaseCalendarDayButton` | 匹配所有月视图中的日期按钮 |
| `atom\|BaseCalendarDayButton:today` | 匹配今日日期按钮，默认显示主色调边框 |
| `atom\|BaseCalendarDayButton:selected` | 匹配已选中的日期按钮 |
| `atom\|BaseCalendarDayButton:selected:not(:inactive)` | 匹配当前月份已选中的日期按钮（排除溢出日期） |
| `atom\|BaseCalendarDayButton:inactive` | 匹配非当前月份的溢出日期按钮（上月/下月） |
| `atom\|BaseCalendarDayButton:blackout` | 匹配被禁用的日期按钮 |
| `atom\|BaseCalendarDayButton:dayfocused` | 匹配键盘焦点所在的日期按钮 |
| `atom\|BaseCalendarDayButton:pointerover` | 匹配鼠标悬浮状态的日期按钮 |
| `atom\|BaseCalendarDayButton:pressed` | 匹配按下状态的日期按钮 |
| `atom\|BaseCalendarDayButton:disabled` | 匹配禁用状态的日期按钮 |

### 月/年按钮选择器（BaseCalendarButton）

| 选择器 | 说明 |
|---|---|
| `atom\|BaseCalendarButton` | 匹配所有年/十年视图中的月份/年份按钮 |
| `atom\|BaseCalendarButton:selected` | 匹配当前选中的月份/年份按钮 |
| `atom\|BaseCalendarButton:inactive` | 匹配十年视图中不属于当前十年的年份按钮 |
| `atom\|BaseCalendarButton:btnfocused` | 匹配键盘焦点所在的月份/年份按钮 |
| `atom\|BaseCalendarButton:pointerover` | 匹配鼠标悬浮状态的按钮 |

### 头部标题按钮选择器（HeadTextButton）

| 选择器 | 说明 |
|---|---|
| `atom\|HeadTextButton` | 匹配头部年月/年/十年标题按钮 |
| `atom\|HeadTextButton:pointerover` | 匹配悬浮状态，默认文字变为主色调 |

### 日历项选择器（CalendarItem）

| 选择器 | 说明 |
|---|---|
| `atom\|CalendarItem` | 匹配日历核心面板 |
| `atom\|CalendarItem:calendardisabled` | 匹配日历被禁用时的面板状态 |

### 模板内部选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Calendar /template/ Border#PART_Frame` | 访问日历外边框 |
| `atom\|CalendarItem /template/ Grid#PART_HeaderLayout` | 访问头部导航布局 |
| `atom\|CalendarItem /template/ atom\|IconButton` | 访问头部导航按钮 |
| `atom\|CalendarItem /template/ atom\|HeadTextButton#PART_HeaderButton` | 访问头部标题按钮 |
