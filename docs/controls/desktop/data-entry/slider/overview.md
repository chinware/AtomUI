# Slider 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.Slider` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [Slider 桌面版实现原理](implementation.md)，Slider Token 的专项设计见 [Slider Token 设计](token.md)，设计和契约变化记录见 [Slider Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider` |
| 控件状态 | Stable |

Slider 是 AtomUI 桌面数据录入体系中的连续或离散数值选择控件，用于在有限范围内通过轨道、滑块、刻度、键盘和 tooltip 表达数值选择。它继承 Avalonia `RangeBase` 的 `Minimum`、`Maximum`、`Value`、`SmallChange` 和 `LargeChange` 语义，并扩展范围选择、刻度标记、方向反转、tooltip 格式化、动效、Form 和 Token 体系。

Slider 的职责是选择一个 `double` 数值，或在范围模式下选择一个 `SliderRangeValue` 区间。它不负责文本输入、数值解析、单位换算、复杂范围校验、异步数据加载或业务格式化模型。需要展示单位、区间说明或业务校验信息时，应由外层表单、文本或业务 ViewModel 承担。

## 2. 设计语言

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

## 3. API 与契约模型

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

## 4. 行为与状态模型

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

## 5. 视觉与主题模型

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

## 6. 控件家族或集成关系

Slider 属于 Data Entry 数值选择控件，与 NumericUpDown 共享“有限范围数值输入”的业务语义，但交互形态不同：NumericUpDown 以文本和步进按钮编辑数值，Slider 以轨道和 thumb 编辑数值。

集成关系：

- `RangeBase`：提供 `Minimum`、`Maximum`、`Value`、`SmallChange`、`LargeChange` 和自动化基础。
- `SliderTrack`：承载轨道、mark、范围段、thumb 布局和轨道绘制。
- `SliderThumb`：承载 pointer capture、drag routed events、focus 和 thumb 绘制。
- `IFormItemAware`：允许 Form 读取、设置和监听 Slider 值。
- `IMotionAwareControl`：统一动效开关。
- `SliderToken`：提供组件级尺寸、颜色、padding 和 outline token。

## 7. 兼容性不变量

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

## 8. 专项模型

### 8.1 范围值模型

`SliderRangeValue` 是范围模式的数据模型。`StartValue` 和 `EndValue` 表示同一数值轴上的起止点。解析字符串时必须得到两个值，且 `StartValue <= EndValue`。控件内部在 `SliderTrack.RangeValue` 层对非 NaN、非 Infinity 值做约束，并把起止值裁剪到 `Minimum` / `Maximum` 范围内。

### 8.2 Mark 模型

`SliderMark` 同时承载 mark 值、标签文本和标签局部样式。Slider 会根据标签字体测量最大标签尺寸，并在渲染阶段计算 mark 点和文本区域。点击 mark 文本区域应按 mark 值更新当前有效 thumb。

Mark 不负责格式化 tooltip，也不改变 `ValueFormatTemplate`。Mark 标签局部样式只影响 mark 文本，不应影响控件整体字体和 token。

### 8.3 Tick 吸附模型

`IsSnapToTickEnabled=true` 时，Slider 根据 `TickFrequency` 把计算值吸附到最近 tick。`TickFrequency <= 0` 时，吸附边界退化为 `Minimum` / `Maximum`。当当前位置已经落在当前 tick 时，键盘移动会继续寻找下一个 tick，避免方向键没有可见响应。

### 8.4 Tooltip 模型

Tooltip 内容由 `ValueFormatTemplate` 格式化当前值。Tooltip 宿主宽度以 `Maximum` 格式化后的文本测量结果作为基准，并保留少量宽度余量，避免常见最大值文本被裁剪。

### 8.5 自动化模型

Slider 使用 `SliderAutomationPeer`，自动化类型为 `Slider`。`SliderThumb` 使用 `SliderThumbAutomationPeer`，自动化类型为 `Thumb`，且不作为 content element 暴露。维护自动化时不得把内部 thumb 当作业务内容项暴露给辅助技术。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [Slider 桌面版实现原理](implementation.md)
- [Slider Token 设计](token.md)
- [Slider Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Slider` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + Gallery API / Token / ShowCase | 生成 `controls/slider/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/slider/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | Gallery DesignTokenDataGrid、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 层次 | 验证内容 |
| --- | --- |
| Public API | 检查 `SliderViewModel.EnsureApiRows()`、Gallery API 表和源码属性保持一致。 |
| 行为状态 | 验证单值、范围值、mark 点击、tick 吸附、方向反转、禁用态、键盘和 tooltip。 |
| AXAML | 验证 `PART_Track`、`PART_StartThumb`、`PART_EndThumb`、horizontal / vertical 样式和 disabled 样式。 |
| Token | 验证 `SliderTokenKind`、主题引用和 Gallery Token 表一致。 |
| 文档 | 运行 `git diff --check`，并检查控件文档相对链接存在。 |
