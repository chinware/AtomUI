# Slider 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Slider` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

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
| `Slider` | control theme | `SliderTheme.axaml` | 用户代码 / 控件宿主 | `FontFamily`, `FontSize`, `IsDirectionReversed`, `IsEnabled`, `IsIncluded`, `IsMotionEnabled` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_Track` | template node (SliderTrack) | `SliderTheme.axaml` | Slider | `FontFamily`, `FontSize`, `IsDirectionReversed`, `IsEnabled`, `IsIncluded`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SliderThumb` | control theme | `SliderThumbTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `SliderTrack` | control theme | `SliderTrackTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Track` | `SliderTrack` | 轨道渲染、mark 渲染、thumb 布局和范围轨道计算。 |
| `PART_StartThumb` | `SliderThumb` | 单值模式 thumb；范围模式起始 thumb。 |
| `PART_EndThumb` | `SliderThumb` | 范围模式结束 thumb；非范围模式隐藏。 |

## Pseudo Classes

稳定伪类：

| 伪类 | 语义 |
| --- | --- |
| `:horizontal` | 当前为水平布局。 |
| `:vertical` | 当前为垂直布局。 |
| `:pressed` | Slider 处于 pressed 状态，由 PressedMixin 维护。 |

Form 集成以 `IsRangeMode` 决定值模型：单值模式读取和设置 `Value`，范围模式读取和设置 `RangeValue`。Slider 不把 tooltip 文本、mark 标签或格式化字符串作为表单值。

## State Flow

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

## Theme and Token Boundaries

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

Token 边界：

SliderToken 是 Slider 的组件级 Token scope。它定义轨道尺寸、rail 尺寸、thumb 尺寸、mark 尺寸、track / rail / mark / thumb 颜色、outline 和 orientation padding。

SliderToken 不承载以下状态：

- `Value`、`RangeValue`、`Minimum`、`Maximum`、`TickFrequency` 等实例数值。
- `Marks` 集合、mark label 文本和 mark 命中状态。
- `IsRangeMode`、`IsIncluded`、`IsSnapToTickEnabled` 等行为状态。
- `IsDragging`、thumb focus、pointer hover、tooltip 打开状态等运行时交互状态。

这些状态分别由 `Slider`、`SliderTrack`、`SliderThumb`、主题 selector 和运行时布局计算处理。

## Customization Boundaries

维护 Slider 时必须保持以下不变量：

- 不修改继承自 `RangeBase` 的 `Minimum`、`Maximum`、`Value`、`SmallChange`、`LargeChange` 和自动化语义。
- 不擅自改变 `SliderRangeValue.Parse` 的起止值顺序校验。
- 不擅自新增、删除、重命名或改变 Slider public API、Template Part、伪类或 Token。
- `PART_Track`、`PART_StartThumb`、`PART_EndThumb` 名称和职责不变。
- 非范围模式必须隐藏结束 thumb，范围模式必须保留两个 thumb。
- `IsIncluded=false` 不应改变 thumb 值、mark 命中或 tooltip，只影响 active track / active mark 表达。
- `IsDirectionReversed` 必须同时影响指针移动和键盘方向。
- `ValueFormatTemplate` 只影响 tooltip 展示，不参与实际值计算。
- `Marks` 的 `Value` 仍按 `Minimum` / `Maximum` 所在坐标系计算位置，不作为独立数据源排序或校验模型。
- 重新套用模板必须释放旧 pointer handler，并重新接入 track 和 thumb tooltip。
- `SliderTrack` detach 时必须释放全局输入订阅。
- Token 名称和语义不擅自重命名、删除或迁移为实例状态。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- `Slider` 是 pointer / keyboard 写值入口；`SliderTrack` 不应在 `IgnoreThumbDrag=true` 路径下同时写值。
- `StartSliderThumb` / `EndSliderThumb` 的 logical / visual children 和 drag 事件由 `ThumbChanged` 成对管理。
- 模板重新应用时旧 pointer handler 必须释放。
- `SliderTrack` detach 时释放全局 input subscription。
- `RangeValue` 必须保持非 NaN、非 Infinity，并裁剪到 `[Minimum, Maximum]`。
- `SliderRangeValue.Parse` 必须拒绝起始值大于结束值的表达式。
- `Marks` 改变后必须重新测量 mark 标签。
- `IsIncluded=false` 只影响 active track / active mark 绘制，不影响值计算和 mark 命中。
- tooltip 格式化只由 `ValueFormatTemplate` 控制。
- Thumb focus / hover outline 由 `SliderThumbTheme` 和 token 控制，不在 `Slider` 中手写视觉状态。
