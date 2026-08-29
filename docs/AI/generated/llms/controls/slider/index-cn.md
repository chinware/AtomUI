# Slider

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Slider 是桌面数据录入体系中的连续或离散数值选择控件。它支持单值模式和任意数量 handle 的 Range 模式，并通过轨道、thumb、mark、键盘和 Tooltip 表达数值选择。

Slider 继承 Avalonia `RangeBase`，使用 `Minimum`、`Maximum`、`Value`、`SmallChange` 和 `LargeChange` 的基础语义。Range 模式不使用独立的起止值类型，而是通过 `RangeValues` 维护有序的多值集合。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider` |
| 状态 | Stable |

## 何时使用

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 数值范围 | 用户在有限区间中选择值。 | `Minimum`、`Maximum`、`Value`。 |
| 多点范围 | 多个 thumb 表达多个边界和活动 segment。 | `IsRangeMode`、`RangeValues`。 |
| Handle 状态 | 每个 thumb 可独立 disabled、聚焦和拖动。 | `DisabledHandles`。 |
| 整体平移 | 首尾 handle 之间的范围整体移动。 | `IsDraggableTrack`。 |
| 操作方向 | 数值沿水平或垂直轨道变化。 | `Orientation`。 |
| 方向语义 | 视觉方向与数值增长方向可反转。 | `IsDirectionReversed`。 |
| 离散步进 | 数值吸附到固定 tick。 | `IsSnapToTickEnabled`、`TickFrequency`。 |
| 轨道语义 | 局部 segment 和整体活动范围可独立绘制。 | `TrackBarBrush`、`TracksBrush`。 |
| 刻度说明 | mark 展示关键点和标签。 | `Marks`、`SliderMark`、`IsIncluded`。 |
| 反馈展示 | 每个 handle 的 Tooltip 展示当前值。 | `ValueFormatTemplate`。 |

## 公共 API

### RangeBase 数值 API

| API | 类型 | 语义 |
| --- | --- | --- |
| `Minimum` | `double` | 允许的最小值。 |
| `Maximum` | `double` | 允许的最大值。 |
| `Value` | `double` | 单值模式的当前值。 |
| `SmallChange` | `double` | 方向键单次调整量。 |
| `LargeChange` | `double` | PageUp / PageDown 单次调整量。 |

### Slider API

| API | 类型 | 语义 |
| --- | --- | --- |
| `Orientation` | `Orientation` | 轨道方向，默认 `Horizontal`。 |
| `IsDirectionReversed` | `bool` | 反转数值增长方向。 |
| `IsSnapToTickEnabled` | `bool` | 是否把指针和键盘输入吸附到 tick。 |
| `IsRangeMode` | `bool` | 是否使用 `RangeValues` 和多 handle 视觉。 |
| `RangeValues` | `IReadOnlyList<double>?` | Range 模式下所有 handle 的当前值，至少两个值。 |
| `DisabledHandles` | `IReadOnlyList<bool>?` | 按索引禁用指定 handle。 |
| `IsDraggableTrack` | `bool` | 是否允许整体活动范围平移。 |
| `TrackBarBrush` | `IBrush?` | 相邻 handle 之间局部轨道的画刷。 |
| `TracksBrush` | `IBrush?` | 首个到最后一个 handle 的整体轨道画刷。 |
| `Marks` | `List<SliderMark>?` | 刻度点和标签集合。 |
| `ValueFormatTemplate` | `string` | Tooltip 文本格式，默认 `{0:0}`。 |
| `IsIncluded` | `bool` | 是否绘制活动轨道和 mark 激活态。 |
| `IsMotionEnabled` | `bool` | 是否启用轨道、thumb 和 mark 动效。 |
| `IsWaveSpiritEnabled` | `bool` | 是否启用 wave spirit 能力。 |

Range 模式使用示例：

```xml
<atom:Slider
    IsRangeMode="True"
    RangeValues="0, 10, 20" />
```

```xml
<atom:Slider
    IsRangeMode="True"
    RangeValues="20, 50, 80"
    DisabledHandles="False, True, False"
    IsDraggableTrack="True" />
```

`RangeValues` 进入控件后会过滤非法值、裁剪到最小最大值并按数值升序归一化。RangeValues 少于两个值时使用 `[Minimum, Minimum]`。`DisabledHandles` 缺失项为 enabled，超出 handle 数量的项忽略。集合采用快照语义，修改时重新赋值集合实例。

### 模板部件与伪类

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Track` | `SliderTrack` | 动态 handle 管理、轨道和 mark 渲染、布局和拖动几何。 |

| 伪类 | 语义 |
| --- | --- |
| `:horizontal` | 当前为水平布局。 |
| `:vertical` | 当前为垂直布局。 |
| `:pressed` | Slider 处于 pressed 状态。 |

SliderThumb 是 SliderTrack 管理的动态视觉节点，不作为固定数量的公共 Template Part 暴露。

## 事件与命令

Slider 的事件与命令以控件文档、源码 public surface 和 Avalonia 基类契约为准；生成器不从源码发明额外事件。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Views/SliderShowCase.axaml:34`

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
        RangeValues="{Binding DefaultRangeValues, Mode=OneWay}" />
    <atom:Slider
        Name="Slider1"
        Marks="{Binding SliderMarks}"
        Maximum="100"
        Minimum="0"
        IsEnabled="{Binding NormalEnabled}"
        IsRangeMode="True"
        TickFrequency="5"
        RangeValues="{Binding DefaultRangeValues, Mode=OneWay}" />

    <StackPanel Orientation="Horizontal" Spacing="2">
        <atom:TextBlock VerticalAlignment="Center" Text="Enabled:" />
        <atom:ToggleSwitch VerticalAlignment="Center" SizeType="Small" IsChecked="{Binding NormalEnabled, Mode=TwoWay}" />
    </StackPanel>
</StackPanel>
```

### 多点组合

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Views/SliderShowCase.axaml:74`

Gallery key：`ExamplesContent` / item `1`

```axaml
<Grid MinHeight="160">
    <atom:Slider
        VerticalAlignment="Center"
        Maximum="100"
        Minimum="0"
        IsRangeMode="True"
        TrackBarBrush="{x:Null}"
        RangeValues="{Binding MultiHandleRangeValues, Mode=OneWay}">
        <atom:Slider.TracksBrush>
            <LinearGradientBrush StartPoint="0%,50%" EndPoint="100%,50%">
                <GradientStop Color="#52C41A" Offset="0" />
                <GradientStop Color="#FAAD14" Offset="0.5" />
                <GradientStop Color="#FF4D4F" Offset="1" />
            </LinearGradientBrush>
        </atom:Slider.TracksBrush>
    </atom:Slider>
</Grid>
```

### 禁用指定滑块

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Views/SliderShowCase.axaml:101`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel MinHeight="160"
            VerticalAlignment="Center"
            Spacing="24">
    <atom:Slider
        Maximum="100"
        Minimum="0"
        IsRangeMode="True"
        RangeValues="{Binding DisabledHandleRangeValues, Mode=OneWay}"
        DisabledHandles="{Binding DisabledHandles}" />
    <WrapPanel Orientation="Horizontal"
               ItemSpacing="20"
               LineSpacing="12">
        <atom:CheckBox
            Content="Disabled Handle 1"
            IsChecked="{Binding IsHandle1Disabled, Mode=TwoWay}" />
        <atom:CheckBox
            Content="Disabled Handle 2"
            IsChecked="{Binding IsHandle2Disabled, Mode=TwoWay}" />
        <atom:CheckBox
            Content="Disabled Handle 3"
            IsChecked="{Binding IsHandle3Disabled, Mode=TwoWay}" />
    </WrapPanel>
</StackPanel>
```

### RangeValues 绑定

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Views/SliderShowCase.axaml:135`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Vertical" Spacing="12">
    <atom:Slider
        Width="260"
        Maximum="100"
        Minimum="0"
        IsRangeMode="True"
        IsSnapToTickEnabled="True"
        TickFrequency="5"
        RangeValues="{Binding BoundRangeValues}" />
    <StackPanel Orientation="Horizontal" Spacing="8">
        <atom:TextBlock VerticalAlignment="Center"
                        Text="绑定范围：" />
        <atom:TextBlock VerticalAlignment="Center"
                        Text="{Binding BoundRangeValuesText}" />
    </StackPanel>
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button SizeType="Small"
                     Command="{Binding SetBoundRangeValuesCommand}"
                     Content="设为 35-85" />
        <atom:Button SizeType="Small"
                     Command="{Binding ClearBoundRangeValuesCommand}"
                     Content="清空" />
    </StackPanel>
</StackPanel>
```

## 状态模型

Slider 的核心状态流：

```text
Minimum / Maximum
      ↓
Value 或归一化后的 RangeValues
      ↓
SliderTrack.EffectiveRangeValues
      ↓
动态 SliderThumb[] + RenderContextData
      ↓
thumb arrange + track render + Tooltip
```

单值模式：

```text
IsRangeMode=false
  Value → 一个 enabled handle
```

Range 模式：

```text
IsRangeMode=true
  RangeValues[0] → handle[0]
  RangeValues[1] → handle[1]
  RangeValues[n] → handle[n]
```

范围模式行为：

- 每两个相邻 handle 之间形成一个活动轨道段。
- 首个 handle 到最后一个 handle 形成整体活动范围。
- 点击轨道时选择最近的 enabled handle。
- disabled handle 不响应 pointer、keyboard 和 focus。
- disabled handle 作为 enabled handle 的固定边界。
- `IsDraggableTrack=true` 时整体平移所有 handle。
- 任意 handle disabled 时整体轨道拖动关闭。

键盘行为：

- `Left`、`Right`、`Up`、`Down` 使用 `SmallChange`。
- `PageUp`、`PageDown` 使用 `LargeChange`。
- `Home`、`End` 移动当前焦点 handle 到有效边界。
- `IsDirectionReversed` 影响 pointer 和 keyboard 的数值方向。

Form 集成：

- 单值模式 Form 值为 `double`。
- Range 模式 Form 值为 `IReadOnlyList<double>`。
- Form 设置值、pointer 写值和 keyboard 写值共用同一套归一化路径。

连续 pointer 输入使用轨道本地坐标直接映射为 `double` 值，默认不按整数或像素量化。只有 `IsSnapToTickEnabled=true` 时，pointer 和 keyboard 写入才在提交前吸附到 `TickFrequency` 对应的 tick。

## 主题与 Design Token

Slider 的默认视觉由三层主题组成：

| 主题 | 职责 |
| --- | --- |
| `SliderTheme.axaml` | 根模板、SliderTrack、属性传递和方向状态。 |
| `SliderTrackTheme.axaml` | 轨道尺寸、rail 尺寸、mark 尺寸、mark 标签和轨道过渡。 |
| `SliderThumbTheme.axaml` | thumb 尺寸、圆点、边框、outline、focus、hover 和 disabled。 |

视觉模型：

- `SliderTrack` 绘制 rail、`TracksBrush`、`TrackBarBrush`、mark 点和 mark 文本。
- `SliderThumb` 绘制圆点、边框和 focus / hover outline。
- `TrackBarBrush` 只表达相邻 handle 之间的 segment。
- `TracksBrush` 只表达首尾 handle 之间的整体范围。
- `IsIncluded=false` 时不绘制两种活动轨道和 mark 激活态。
- disabled handle 使用对应 thumb disabled token，不改变其他 handle 的颜色。
- 全部 handle disabled 时使用 Slider 整体 disabled 状态。
- 水平布局使用 `SliderPaddingHorizontal`，垂直布局使用 `SliderPaddingVertical`。

`TrackBarBrush` 和 `TracksBrush` 是实例级画刷，不属于 SliderToken。SliderToken 只提供默认视觉值。

Token 来源：

SliderToken 是 Slider 的控件级 Token scope，定义轨道尺寸、rail 尺寸、thumb 尺寸、mark 尺寸、track / rail / mark / thumb 颜色、outline 和 orientation padding。

SliderToken 不承载：

- `Value`、`RangeValues`、`Minimum`、`Maximum`、`TickFrequency` 等实例数值。
- `DisabledHandles`、`IsDraggableTrack` 和当前拖动状态。
- `Marks` 集合、mark 文本和命中状态。
- Tooltip 文本、focus、hover、pressed 和 pointer capture 状态。
- `TrackBarBrush`、`TracksBrush` 这类实例级画刷。

这些状态由 Slider、SliderTrack、SliderThumb、Gallery ViewModel 和主题 selector 分别持有。

## AOT 与裁剪注意事项

- 动态 thumb 的创建、事件订阅、templated parent 和释放由 `SliderTrack` 成对管理。
- Slider 的模板 handler、SliderTrack 的全局 input subscription 和 pointer capture 都有明确释放入口。
- 外部 `RangeValues` 与 `DisabledHandles` 不建立集合变更订阅，调用方必须替换快照触发更新。
- pointer move 不创建或替换 thumb；只提交值快照并触发布局、Tooltip 与渲染更新。
- `RenderContextData`、pen 和 mark 文本度量由 SliderTrack 持有，不进入公共状态。
- 实现不使用反射、动态类型发现、字符串属性路径或运行时程序集扫描，保持 trimming 和 NativeAOT 友好。
- `UseLayoutRounding=false` 只应用于默认 `SliderThumbTheme`，用于保留连续拖动得到的 fractional Bounds；自定义主题需要自行保持同一平滑布局契约。

## 源码索引

- `src/AtomUI.Desktop.Controls/Slider/Slider.cs`：公共属性、Range 值归一化入口、pointer / keyboard 交互协调、Tooltip、Form、数据校验和自动化 peer 创建。
- `src/AtomUI.Desktop.Controls/Slider/SliderTrack.cs`：`EffectiveRangeValues` 投影、动态 thumb 生命周期、handle 布局、track / tracks / mark 渲染和几何换算。
- `src/AtomUI.Desktop.Controls/Slider/SliderThumb.cs`：单个 handle 的 pointer capture、drag routed event、focus / pressed / disabled 状态和绘制。
- `src/AtomUI.Desktop.Controls/Slider/SliderRangeMath.cs`：Range 值归一化、handle 边界、整体 offset、值与坐标比例及 segment 几何的纯计算。
- `src/AtomUI.Desktop.Controls/Slider/SliderToken.cs`：Slider Token scope、尺寸、颜色、padding 和 outline 默认值计算。
- `src/AtomUI.Desktop.Controls/Slider/SliderAutomationPeer.cs`：Slider 自动化 peer。
- `src/AtomUI.Desktop.Controls/Slider/SliderThumbAutomationPeer.cs`：单个动态 thumb 的自动化 peer。
- `src/AtomUI.Desktop.Controls/Slider/Themes/SliderTheme.axaml`：根模板、`PART_Track`、属性传递和方向样式。
- `src/AtomUI.Desktop.Controls/Slider/Themes/SliderTrackTheme.axaml`：track 尺寸、mark Token 和轨道 transition。
- `src/AtomUI.Desktop.Controls/Slider/Themes/SliderThumbTheme.axaml`：thumb 尺寸、边框、outline、focus、hover、disabled 和布局取整策略。
- `tests/AtomUI.Desktop.Controls.Tests/Slider/SliderBehaviorTests.cs`：值计算、动态 thumb、pointer、布局和生命周期回归。
- `controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/`：Slider Gallery 示例、ViewModel 和本地化资源。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/slider/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/slider/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/slider/token.md`
- 变更记录：`docs/controls/desktop/data-entry/slider/changelog.md`
- 语义结构：`./semantic-cn.md`
