# Slider 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

### 2.1 `root`

| 字段 | 值 |
| --- | --- |
| Owner | `Slider` |
| Part | `root` |
| Selector | Slider 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `Slider` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Slider owner |
| 职责 | 数值选择控件根：承载 `Minimum` / `Maximum` / `Value` / `RangeValues` 值状态、方向、轨道与 mark 配置、键盘与 pointer 交互会话、Tooltip 与 Form 集成；作为全部 Part 的 owner-scoped Selector 作用域边界。对应上游 `.ant-slider`。 |
| 相关 API | `Orientation`、`IsDirectionReversed`、`IsSnapToTickEnabled`、`TickFrequency`、`IsRangeMode`、`RangeValues`、`DisabledHandles`、`IsDraggableTrack`、`TrackBarBrush`、`TracksBrush`、`Marks`、`IsIncluded`、`ValueFormatTemplate`、`IsMotionEnabled` |
| 相关 Token | `SliderPaddingHorizontal`、`SliderPaddingVertical`、SharedToken |
| 稳定性 | stable since 6.0 |

### 2.2 `rail`

| 字段 | 值 |
| --- | --- |
| Owner | `Slider` |
| Part | `rail` |
| Selector | `.semantic-rail` |
| SelectorRoute | `/template/ .semantic-rail` |
| Style Type | `SliderRailStyle` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `SliderTrack` 内代码创建的 rail `Border` 元素（几何 = `GetRailRect`，胶囊圆角，背景 = `TrackGrooveBrush`） |
| 职责 | 统一表示背景轨道区域：rail 画刷、胶囊圆角与过渡；对应上游 `.ant-slider-rail`。 |
| 相关 API | `TrackGrooveBrush`（SliderTrack）、`IsEnabled`、`Orientation` |
| 相关 Token | `RailBg`、`RailHoverBg`、`RailSize` |
| 稳定性 | stable since 6.0 |

### 2.3 `tracks`

| 字段 | 值 |
| --- | --- |
| Owner | `Slider` |
| Part | `tracks` |
| Selector | `.semantic-tracks` |
| SelectorRoute | `/template/ .semantic-tracks` |
| Style Type | `SliderTracksStyle` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `SliderTrack` 内代码创建的整体活动范围 `Border` 元素（Range 模式首值→末值；单值模式 `Minimum → Value`；背景 = `TracksBrush`） |
| 职责 | 统一表示整体活动范围容器：Range 模式覆盖首尾 handle 之间的整段跨度，单值模式覆盖最小到当前值的跨度；对应上游 `.ant-slider-tracks`。 |
| 相关 API | `TracksBrush`、`IsRangeMode`、`IsIncluded`、`IsDraggableTrack` |
| 相关 Token | 无专属 Token（默认画刷为 null） |
| 稳定性 | stable since 6.0 |

### 2.4 `track`

| 字段 | 值 |
| --- | --- |
| Owner | `Slider` |
| Part | `track` |
| Selector | `.semantic-track` |
| SelectorRoute | `/template/ .semantic-track` |
| Style Type | `SliderTrackStyle` |
| ContractType | `Border` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `SliderTrack` 内代码创建的活动 segment `Border` 元素（单值模式 1 个：`Minimum → Value`；Range 模式每对相邻值 1 个；背景 = `TrackBarBrush`） |
| 职责 | 统一表示相邻 handle 之间的活动轨道段：segment 画刷、胶囊圆角与过渡；对应上游 `.ant-slider-track`。 |
| 相关 API | `TrackBarBrush`、`RangeValues`、`IsIncluded`、`IsDraggableTrack` |
| 相关 Token | `TrackBg`、`TrackHoverBg`、`TrackBgDisabled`、`SliderTrackSize` |
| 稳定性 | stable since 6.0 |

### 2.5 `handle`

| 字段 | 值 |
| --- | --- |
| Owner | `Slider` |
| Part | `handle` |
| Selector | `.semantic-handle` |
| SelectorRoute | `/template/ .semantic-handle` |
| Style Type | `SliderHandleStyle` |
| ContractType | `SliderThumb` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个动态 `SliderThumb` 节点 |
| 职责 | 统一表示滑块控制点：圆点背景、边框、outline、hover / focus / pressed / disabled 视觉与 Tooltip 宿主；对应上游 `.ant-slider-handle`。 |
| 相关 API | `SliderThumb.OutlineBrush`、`SliderThumb.OutlineThickness`、`SliderThumb.ThumbCircleSize`、`DisabledHandles`、`ValueFormatTemplate` |
| 相关 Token | `ThumbSize`、`ThumbCircleSize`、`ThumbCircleSizeHover`、`ThumbCircleBorderColor`、`ThumbCircleBorderActiveColor`、`ThumbCircleBorderColorDisabled`、`ThumbCircleBorderThickness`、`ThumbCircleBorderThicknessHover`、`ThumbOutlineColor`、`ThumbOutlineThickness` |
| 稳定性 | stable since 6.0 |

### 2.6 marker 放置与路由

`root` 是隐式 Part，不声明 `.semantic-root` marker。非 root Part 全部 `RuntimeCreated=true`，marker 在节点创建时用
生成常量一次性添加：

- `rail`、`tracks` 元素在 `SliderTrack` attach 时创建一次，恒存在；`track` segment 元素随 `EffectiveRangeValues`
  数量同步（单值 1 个、Range N-1 个），数量变化时增删节点。
- `handle` marker 在 `SliderTrack.AddThumb` 时添加；thumb 数量随 handle 数量同步，回收 / 重建路径 marker 随实例。
- 三个主题文件（`SliderTheme.axaml` / `SliderTrackTheme.axaml` / `SliderThumbTheme.axaml`）不声明任何静态
  `.semantic-*` marker；所有 marker 均为运行时创建。
- mark 点 / 标签（`SliderMarksElement`）不携带任何 semantic marker。

路由：四个运行时 Part 的节点都由 `SliderTrack` 创建并设置外层 `Slider` 为 TemplatedParent（与既有
`SliderThumb` 一致），因此从 `Slider` owner 出发均为单跳 `/template/ .semantic-*`。生成器对
`RuntimeCreated=true` 的 Part 只要求显式 `SelectorRoute`，不按 owner 主题资产做静态校验。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Slider/Themes/SliderTheme.axaml`

```xml
<SliderTrack Name="PART_Track" />
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Slider
  -> Slider (control theme, SliderTheme.axaml)
     -> SliderTrack#PART_Track (template-stable)
  -> SliderThumb (control theme, SliderThumbTheme.axaml)
  -> SliderTrack (control theme, SliderTrackTheme.axaml)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Slider` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Slider` | control theme | `SliderTheme.axaml` | 用户代码 / 控件宿主 | `DisabledHandles`, `FontFamily`, `FontSize`, `IsDirectionReversed`, `IsDraggableTrack`, `IsEnabled` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_Track` | template node (SliderTrack) | `SliderTheme.axaml` | Slider | `DisabledHandles`, `FontFamily`, `FontSize`, `IsDirectionReversed`, `IsDraggableTrack`, `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SliderThumb` | control theme | `SliderThumbTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `SliderTrack` | control theme | `SliderTrackTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Track` | `SliderTrack` | 动态 handle 管理、轨道和 mark 渲染、布局和拖动几何。 |

## Pseudo Classes

### 模板部件与伪类

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Track` | `SliderTrack` | 动态 handle 管理、轨道和 mark 渲染、布局和拖动几何。 |

| 伪类 | 语义 |
| --- | --- |
| `:horizontal` | 当前为水平布局。 |
| `:vertical` | 当前为垂直布局。 |
| `:pressed` | Slider 处于 pressed 状态。 |

## State Flow

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

## Theme and Token Boundaries

Slider 的默认视觉由三层主题组成：

| 主题 | 职责 |
| --- | --- |
| `SliderTheme.axaml` | 根模板、SliderTrack、属性传递和方向状态。 |
| `SliderTrackTheme.axaml` | 轨道尺寸、rail 尺寸、mark 尺寸、mark 标签和轨道过渡。 |
| `SliderThumbTheme.axaml` | thumb 尺寸、圆点、边框、outline、focus、hover 和 disabled。 |

视觉模型：

- `SliderTrack` 管理 rail、整体活动范围、相邻 segment 与 mark 的**元素**节点并计算其几何：rail 元素（
  `TrackGrooveBrush`）、tracks 元素（`TracksBrush`）、segment 元素（`TrackBarBrush`）按值比例排列，mark 点与文本
  由 internal `SliderMarksElement` 自绘；四个区域元素化后与历史自绘几何一致，默认视觉不变。
- `SliderThumb` 绘制圆点、边框和 focus / hover outline。
- `TrackBarBrush` 只表达相邻 handle 之间的 segment。
- `TracksBrush` 只表达整体活动范围（Range 模式首尾 handle 之间；单值模式 `Minimum → Value`）。
- `IsIncluded=false` 时不绘制两种活动轨道和 mark 激活态。
- disabled handle 使用对应 thumb disabled token，不改变其他 handle 的颜色。
- 全部 handle disabled 时使用 Slider 整体 disabled 状态。
- 水平布局使用 `SliderPaddingHorizontal`，垂直布局使用 `SliderPaddingVertical`。

`TrackBarBrush` 和 `TracksBrush` 是实例级画刷，不属于 SliderToken。SliderToken 只提供默认视觉值。

Token 边界：

SliderToken 是 Slider 的控件级 Token scope，定义轨道尺寸、rail 尺寸、thumb 尺寸、mark 尺寸、track / rail / mark / thumb 颜色、outline 和 orientation padding。

SliderToken 不承载：

- `Value`、`RangeValues`、`Minimum`、`Maximum`、`TickFrequency` 等实例数值。
- `DisabledHandles`、`IsDraggableTrack` 和当前拖动状态。
- `Marks` 集合、mark 文本和命中状态。
- Tooltip 文本、focus、hover、pressed 和 pointer capture 状态。
- `TrackBarBrush`、`TracksBrush` 这类实例级画刷。

这些状态由 Slider、SliderTrack、SliderThumb、Gallery ViewModel 和主题 selector 分别持有。

## Customization Boundaries

- Range 模式唯一值源是 `RangeValues`，不建立起止 handle 特殊状态。
- `RangeValues` 必须经过有限性校验、范围裁剪和升序归一化。
- Range 模式至少保留两个 handle。
- disabled handle 不能被 pointer 或 keyboard 修改。
- 任意 disabled handle 存在时，整体活动范围不可拖动。
- `IsDraggableTrack` 平移所有 handle，并保持 handle 相对距离。
- `TrackBarBrush` 和 `TracksBrush` 的职责不能互换。
- `IsIncluded=false` 只影响活动轨道和 mark 激活视觉，不影响值计算。
- SliderTrack 负责动态 thumb 的创建、复用、回收和事件解绑。
- 模板重建必须释放旧事件订阅和拖动状态。
- SliderTrack detach 必须释放全局输入订阅。
- Tooltip 只展示格式化值，不改变 Form 值。
- Token 不承载 Value、RangeValues、DisabledHandles 或拖动状态。

维护不变量：

- Slider 是公共值、Form 状态和交互会话 owner；SliderTrack 是动态节点、元素、布局、渲染和输入几何 owner。
- Range 模式唯一值源是 `RangeValues`，不建立固定 start / end handle 或并行 `HandleState` 模型。
- `RangeValues` 必须经过有限性检查、范围裁剪和升序归一化，并保留重复值和 handle 数量。
- 连续 pointer 拖动必须保留 `double` 精度；仅 `IsSnapToTickEnabled=true` 时按 tick 量化。
- disabled handle 不可被 pointer 或 keyboard 修改，并作为相邻 handle 的移动边界。
- 任意 disabled handle 存在时，整体活动范围不可拖动。
- `TracksBrush` 只绘制整体活动范围（单值 `Minimum → Value`、Range 首尾值）；`TrackBarBrush` 只绘制相邻 handle segment。
- `PART_Track` 是唯一固定 Slider Template Part；动态 thumb 与 segment 元素不得重新变成固定命名部件。
- rail / tracks 元素每 Slider 恒一个，segment 元素数量只随值数量变化；`IsIncluded=false` 只切换 tracks / segment 可见性，不增删节点或 marker。
- 动态 thumb 与元素必须继承外层 Slider 的模板状态，并实时同步 motion 状态。
- rail / tracks / segment / mark 元素不可命中测试；pointer 与 mark 命中仍走 SliderTrack 几何计算路径。
- 模板重建和 detach 必须释放事件、pointer capture、全局 input subscription、拖动会话与全部动态节点。
