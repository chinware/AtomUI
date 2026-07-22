# Slider

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Slider 是 AtomUI 桌面数据录入体系中的连续或离散数值选择控件，用于在有限范围内通过轨道、滑块、刻度、键盘和 tooltip 表达数值选择。它继承 Avalonia `RangeBase` 的 `Minimum`、`Maximum`、`Value`、`SmallChange` 和 `LargeChange` 语义，并扩展范围选择、刻度标记、方向反转、tooltip 格式化、动效、Form 和 Token 体系。

Slider 的职责是选择一个 `double` 数值，或在范围模式下选择一个 `SliderRangeValue` 区间。它不负责文本输入、数值解析、单位换算、复杂范围校验、异步数据加载或业务格式化模型。需要展示单位、区间说明或业务校验信息时，应由外层表单、文本或业务 ViewModel 承担。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider` |
| 状态 | Stable |

## 何时使用

Slider 的设计语言来自轨道、进度段、滑块和刻度标记的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 数值范围 | 用户在有限区间中选择值。 | `Minimum`、`Maximum`、`Value`、`RangeValue`。 |
| 操作方向 | 数值沿水平或垂直轨道变化。 | `Orientation`、`:horizontal`、`:vertical`。 |
| 方向语义 | 视觉方向与数值增长方向可反转。 | `IsDirectionReversed`。 |
| 离散步进 | 数值可吸附到固定 tick。 | `IsSnapToTickEnabled`、`TickFrequency`。 |
| 区间选择 | 两个 thumb 表达起止值。 | `IsRangeMode`、`SliderRangeValue`。 |
| 刻度说明 | mark 展示关键点和标签。 | `Marks`、`SliderMark`、`IsIncluded`。 |
| 反馈展示 | thumb tooltip 展示当前值。 | `ValueFormatTemplate`。 |
| 动效 | hover、focus 和 track 状态平滑变化。 | `IsMotionEnabled`、`IsWaveSpiritEnabled`。 |

Slider 的主视觉必须以轨道和 thumb 为第一视觉焦点。mark 和 tooltip 是辅助信息，不应改变 `Value` / `RangeValue` 的数据契约。

## 公共 API

Slider 的公共 API 由 Avalonia RangeBase 数值 API、AtomUI Slider 扩展 API、范围值类型、mark 类型和 Form 集成组成。

RangeBase 数值 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Minimum` | `double` | 允许的最小值。 |
| `Maximum` | `double` | 允许的最大值。 |
| `Value` | `double` | 单值模式的当前值，沿用 `RangeBase` 数值契约并支持数据校验。 |
| `SmallChange` | `double` | 方向键单次调整量。 |
| `LargeChange` | `double` | PageUp / PageDown 单次调整量。 |

Slider 扩展 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Orientation` | `Orientation` | 轨道方向，默认 `Horizontal`。 |
| `IsDirectionReversed` | `bool` | 反转数值增长方向。 |
| `IsSnapToTickEnabled` | `bool` | 是否把指针和键盘输入吸附到 tick。 |
| `TickFrequency` | `double` | tick 间隔；大于 0 时参与吸附计算。 |
| `RangeValue` | `SliderRangeValue` | 范围模式的起止值，默认 `TwoWay` 绑定并接入 Avalonia `DataValidationErrors`。 |
| `IsRangeMode` | `bool` | 是否显示第二个 thumb 并使用范围选择模型。 |
| `Marks` | `List<SliderMark>?` | 刻度点和标签集合。 |
| `ValueFormatTemplate` | `string` | tooltip 文本格式，默认 `{0:0}`。 |
| `IsIncluded` | `bool` | 是否绘制已选择 track 段和 mark 激活态。 |
| `IsMotionEnabled` | `bool` | 是否启用 track、thumb 和 mark 相关动效。 |
| `IsWaveSpiritEnabled` | `bool` | 是否启用 wave spirit 能力。 |

范围和 mark 类型：

| 类型 | 语义 |
| --- | --- |
| `SliderRangeValue` | 值类型，包含 `StartValue` 和 `EndValue`，表达范围模式的起止值。 |
| `SliderRangeValue.Parse(string)` | 解析两个 double 值组成的范围表达式。 |
| `SliderMark` | 刻度标签模型，包含 `Label`、`Value`、可选 `LabelBrush`、`LabelFontStyle` 和 `LabelFontWeight`。 |

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Track` | `SliderTrack` | 轨道渲染、mark 渲染、thumb 布局和范围轨道计算。 |
| `PART_StartThumb` | `SliderThumb` | 单值模式 thumb；范围模式起始 thumb。 |
| `PART_EndThumb` | `SliderThumb` | 范围模式结束 thumb；非范围模式隐藏。 |

稳定伪类：

| 伪类 | 语义 |
| --- | --- |
| `:horizontal` | 当前为水平布局。 |
| `:vertical` | 当前为垂直布局。 |
| `:pressed` | Slider 处于 pressed 状态，由 PressedMixin 维护。 |

Form 集成以 `IsRangeMode` 决定值模型：单值模式读取和设置 `Value`，范围模式读取和设置 `RangeValue`。`RangeValue` 是用户拥有的受控值，默认双向绑定；绑定错误和 Form error 都通过 Avalonia `DataValidationErrors` 投射。Slider 不把 tooltip 文本、mark 标签或格式化字符串作为表单值。

## 事件与命令

Slider 的事件与命令以控件文档、源码 public surface 和 Avalonia 基类契约为准；生成器不从源码发明额外事件。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Views/SliderShowCase.axaml:35`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:Slider
        Maximum="100"
        Minimum="0"
        TickFrequency="5"
        IsEnabled="{Binding NormalEnabled}"
        Value="50" />

    <atom:Slider
        Maximum="100"
        Minimum="0"
        IsRangeMode="True"
        TickFrequency="5"
        IsEnabled="{Binding NormalEnabled}"
        RangeValue="20, 80" />
    <atom:Slider
        Name="Slider1"
        Marks="{Binding SliderMarks}"
        Maximum="100"
        Minimum="0"
        IsEnabled="{Binding NormalEnabled}"
        IsRangeMode="True"
        TickFrequency="5"
        RangeValue="20, 80" />

    <StackPanel Orientation="Horizontal" Spacing="2">
        <atom:TextBlock VerticalAlignment="Center" Text="Enabled:" />
        <atom:ToggleSwitch VerticalAlignment="Center" SizeType="Small" IsChecked="{Binding NormalEnabled, Mode=TwoWay}" />
    </StackPanel>
</StackPanel>
```

### RangeValue 绑定

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Views/SliderShowCase.axaml:76`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical" Spacing="12">
    <atom:Slider
        Width="260"
        Maximum="100"
        Minimum="0"
        IsRangeMode="True"
        IsSnapToTickEnabled="True"
        TickFrequency="5"
        RangeValue="{Binding BoundRangeValue}" />
    <StackPanel Orientation="Horizontal" Spacing="8">
        <atom:TextBlock VerticalAlignment="Center"
                        Text="绑定范围：" />
        <atom:TextBlock VerticalAlignment="Center"
                        Text="{Binding BoundRangeValueText}" />
    </StackPanel>
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button SizeType="Small"
                     Command="{Binding SetBoundRangeValueCommand}"
                     Content="设为 35-85" />
        <atom:Button SizeType="Small"
                     Command="{Binding ClearBoundRangeValueCommand}"
                     Content="清空" />
    </StackPanel>
</StackPanel>
```

### 自定义提示

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Views/SliderShowCase.axaml:110`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:Slider
        Maximum="100"
        Minimum="0"
        TickFrequency="1"
        IsSnapToTickEnabled="True"
        ValueFormatTemplate="\{0\}%"
        Value="20" />
</StackPanel>
```

### 垂直方向

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Views/SliderShowCase.axaml:129`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Horizontal" Spacing="20" Height="300">
    <atom:Slider
        Maximum="100"
        Minimum="0"
        Orientation="Vertical"
        TickFrequency="1"
        Value="20" />
    <atom:Slider
        Maximum="100"
        Minimum="0"
        Orientation="Vertical"
        TickFrequency="5"
        IsRangeMode="True"
        RangeValue="20, 80"
        IsSnapToTickEnabled="True"
        Value="20" />

    <atom:Slider
        Name="Slider2"
        Marks="{Binding SliderMarks}"
        Maximum="100"
        Minimum="0"
        Orientation="Vertical"
        TickFrequency="1"
        Value="20" />

    <atom:Slider
        Name="Slider3"
        Marks="{Binding SliderMarks}"
        Maximum="100"
        Minimum="0"
        IsRangeMode="True"
        Orientation="Vertical"
        TickFrequency="5"
        RangeValue="20, 80" />

</StackPanel>
```

## 状态模型

Slider 的核心状态流：

```text
Minimum / Maximum
      ↓
Value or RangeValue
      ↓
SliderTrack density and thumb center offsets
      ↓
thumb arrange + track render + tooltip text
```

指针行为：

- 点击轨道时，单值模式移动 `PART_StartThumb`，范围模式移动距离点击位置最近的 thumb。
- 点击 mark 标签区域时，优先把对应值写入当前有效 thumb，而不是按轨道坐标连续计算。
- 拖动过程中根据当前 `Orientation`、`IsDirectionReversed`、`Minimum` 和 `Maximum` 计算值。
- `IsSnapToTickEnabled=true` 时，指针和键盘调整都会进入 tick 吸附逻辑。
- 禁用状态下指针移动不会更新值。

键盘行为：

- `Left` / `Down` 按 `SmallChange` 向较小方向移动；`Right` / `Up` 按 `SmallChange` 向较大方向移动。
- `PageDown` / `PageUp` 按 `LargeChange` 调整。
- `Home` 写入 `Minimum`，`End` 写入 `Maximum`。
- `IsDirectionReversed=true` 时方向键和 Page 键的数值方向反转。
- XY focus navigation 场景下，`Enter` / `Escape` 用于切换和取消 focus engaged 状态。

范围模式行为：

```text
IsRangeMode=false
  Value → StartThumb visible → EndThumb hidden

IsRangeMode=true
  RangeValue.StartValue → StartThumb
  RangeValue.EndValue   → EndThumb
```

范围模式主要由轨道点击、mark 点击和 thumb 拖动更新 `RangeValue`。键盘路径沿用 `RangeBase.Value` 单值模型，不应被文档或示例描述为完整的范围键盘编辑契约。

Tooltip 行为：

- 单值模式下，`PART_StartThumb` tooltip 显示格式化后的 `Value`。
- 范围模式下，两个 thumb 分别显示格式化后的 `RangeValue.StartValue` 和 `RangeValue.EndValue`。
- 水平布局 tooltip 放置在 `Top`，垂直布局 tooltip 放置在 `Right`。
- `ValueFormatTemplate` 和 `Maximum` 变化会重新计算 tooltip 宿主宽度。

## 主题与 Design Token

Slider 的默认视觉由三层主题组成：

| 主题 | 职责 |
| --- | --- |
| `SliderTheme.axaml` | 根模板、`PART_Track`、两个 thumb、方向布局、hover / disabled 状态和主题 token 绑定。 |
| `SliderTrackTheme.axaml` | track 尺寸、rail 尺寸、mark 尺寸、mark 标签颜色、动效 transition。 |
| `SliderThumbTheme.axaml` | thumb 尺寸、圆点、边框、outline、focus、hover、ZIndex 和动效 transition。 |

视觉模型：

- `SliderTrack` 负责直接渲染 rail、active track、mark 点和 mark 文本。
- `SliderThumb` 负责直接渲染 thumb 圆点和 focus / hover outline。
- `IsIncluded=true` 时绘制 active track 和 mark active 状态；`false` 时只保留 rail、thumb 和 mark 基础视觉。
- `Orientation=Horizontal` 使用 `SliderPaddingHorizontal`；`Orientation=Vertical` 使用 `SliderPaddingVertical`。
- disabled 状态使用 `TrackBgDisabled` 和 `ThumbCircleBorderColorDisabled`，mark 标签使用 SharedToken disabled 文本色。

Slider 不使用 `CustomizableSizeType`。尺寸由 SliderToken、控件 `Width` / `Height`、`Orientation` 和布局容器共同决定。

Token 来源：

SliderToken 是 Slider 的组件级 Token scope。它定义轨道尺寸、rail 尺寸、thumb 尺寸、mark 尺寸、track / rail / mark / thumb 颜色、outline 和 orientation padding。

SliderToken 不承载以下状态：

- `Value`、`RangeValue`、`Minimum`、`Maximum`、`TickFrequency` 等实例数值。
- `Marks` 集合、mark label 文本和 mark 命中状态。
- `IsRangeMode`、`IsIncluded`、`IsSnapToTickEnabled` 等行为状态。
- `IsDragging`、thumb focus、pointer hover、tooltip 打开状态等运行时交互状态。

这些状态分别由 `Slider`、`SliderTrack`、`SliderThumb`、主题 selector 和运行时布局计算处理。

## AOT 与裁剪注意事项

Slider 不依赖反射扫描或运行时动态发现控件成员。模板协同通过 `TemplateBinding`、稳定 template part 和 Avalonia property 完成。

性能边界：

- `RenderContextData` 是单次渲染准备数据，不应被外部缓存为长期状态。
- Mark 文本测量只在 mark、字体或 enabled 相关变化时刷新，避免每次 render 重新测量文本。
- `SliderThumb` 使用 cached pen helper 更新画笔，避免每帧创建多余对象。
- 全局 input subscription 必须在 detach 时释放。

AOT 边界：

- 不新增反射访问 public API、template part 或 token kind。
- 新增 Token 必须走 source generator 支持的 `SliderToken` 属性。
- Gallery API / Token 表应显式维护，不依赖运行时反射扫描。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Slider/Slider.cs`：public API、pointer / keyboard 交互、tooltip 同步、tick 吸附、Form 接口和自动化 peer。
- `src/AtomUI.Desktop.Controls/Slider/SliderTrack.cs`：track / mark 渲染、thumb 布局、范围值裁剪、drag delta 转值、全局 pointer 订阅和内部 render context。
- `src/AtomUI.Desktop.Controls/Slider/SliderThumb.cs`：thumb pointer capture、drag routed events、focus / pressed 状态和直接绘制。
- `src/AtomUI.Desktop.Controls/Slider/SliderToken.cs`：Slider Token scope、尺寸、颜色、padding、outline 默认值计算。
- `src/AtomUI.Desktop.Controls/Slider/SliderAutomationPeer.cs`：Slider 自动化 peer。
- `src/AtomUI.Desktop.Controls/Slider/SliderThumbAutomationPeer.cs`：SliderThumb 自动化 peer。
- `src/AtomUI.Desktop.Controls/Slider/Themes/SliderTheme.axaml`：Slider 根模板、track / thumb 创建和状态样式。
- `src/AtomUI.Desktop.Controls/Slider/Themes/SliderTrackTheme.axaml`：track token、mark token 和 transition。
- `src/AtomUI.Desktop.Controls/Slider/Themes/SliderThumbTheme.axaml`：thumb 尺寸、边框、outline、focus / hover 和 transition。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/slider/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/slider/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/slider/token.md`
- 变更记录：`docs/controls/desktop/data-entry/slider/changelog.md`
- 语义结构：`./semantic-cn.md`
