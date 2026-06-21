# Slider Token 设计

本文档定义 `AtomUI.Desktop.Controls.SliderToken` 的 Slider 专属语义、分类、使用范围和兼容边界。控件 Token 的通用分层、命名、计算、Theme Variables、预设色规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/control-token-guidelines.md)。Slider 整体架构见 [Slider 桌面版架构设计](overview.md)，内部实现原理见 [Slider 桌面版实现原理](implementation.md)，设计和契约变化记录见 [Slider Changelog](changelog.md)。

## 1. 定位

SliderToken 是 Slider 的组件级 Token scope。它定义轨道尺寸、rail 尺寸、thumb 尺寸、mark 尺寸、track / rail / mark / thumb 颜色、outline 和 orientation padding。

SliderToken 不承载以下状态：

- `Value`、`RangeValue`、`Minimum`、`Maximum`、`TickFrequency` 等实例数值。
- `Marks` 集合、mark label 文本和 mark 命中状态。
- `IsRangeMode`、`IsIncluded`、`IsSnapToTickEnabled` 等行为状态。
- `IsDragging`、thumb focus、pointer hover、tooltip 打开状态等运行时交互状态。

这些状态分别由 `Slider`、`SliderTrack`、`SliderThumb`、主题 selector 和运行时布局计算处理。

## 2. Token 分类

### 2.1 轨道与布局尺寸 Token

- `SliderTrackSize`
- `RailSize`
- `SliderPaddingHorizontal`
- `SliderPaddingVertical`
- `MarginPartWithMark`

`SliderTrackSize` 表示 Slider 控件轨道区域推荐尺寸，`RailSize` 表示 rail 厚度。水平和垂直 padding 分别用于不同 orientation 下为 thumb、rail 和 mark 留出空间。`MarginPartWithMark` 描述带 mark 场景下的额外布局余量语义。

### 2.2 Mark Token

- `MarkSize`
- `MarkBorderColor`
- `MarkBorderColorHover`
- `MarkBorderColorActive`

Mark Token 服务 mark 点本身的尺寸和边框状态色。Mark 标签字体、文本和局部 label brush 来自 `SliderMark`、控件字体属性和 SharedToken，不在 SliderToken 中定义。

### 2.3 Thumb 尺寸 Token

- `ThumbSize`
- `ThumbCircleSize`
- `ThumbCircleSizeHover`
- `ThumbCircleBorderThickness`
- `ThumbCircleBorderThicknessHover`
- `ThumbOutlineThickness`

`ThumbSize` 是 thumb 控件自身的推荐最大尺寸，用于容纳 hover/focus 圆点、边框和 outline。`ThumbCircleSize` 与 `ThumbCircleSizeHover` 控制实际圆点直径。边框和 outline token 必须与 `SliderThumb.Render` 的圆形绘制模型保持一致。

### 2.4 Thumb 颜色 Token

- `ThumbCircleBorderColor`
- `ThumbCircleBorderHoverColor`
- `ThumbCircleBorderActiveColor`
- `ThumbCircleBorderColorDisabled`
- `ThumbOutlineColor`

Thumb 颜色 Token 控制普通、hover、focus、active、disabled 和 outline 视觉。Thumb 背景默认使用 SharedToken `ColorBgContainer`，不在 SliderToken 中定义独立背景 Token。

### 2.5 Rail 与 Track 颜色 Token

- `RailBg`
- `RailHoverBg`
- `TrackBg`
- `TrackHoverBg`
- `TrackBgDisabled`

Rail 表示完整轨道背景，Track 表示已选择区间或单值已覆盖部分。Disabled 状态通过 `TrackBgDisabled` 和 thumb disabled token 共同表达。

## 3. 控件专项模型中的 Token 使用

Token 使用路径：

```text
SharedToken
    ↓
SliderToken.CalculateTokenValues()
    ↓
SliderTheme / SliderTrackTheme / SliderThumbTheme
    ↓
SliderTrack.Render + SliderThumb.Render
```

主题使用关系：

| Token 分类 | 主要消费点 |
| --- | --- |
| 轨道尺寸 | `SliderTrackTheme.axaml`、`SliderTrack.MeasureOverride`、`SliderTrack.PrepareRenderInfo`。 |
| Padding | `SliderTheme.axaml` 根据 horizontal / vertical 设置 `SliderTrack.Padding`。 |
| Rail / Track 颜色 | `SliderTheme.axaml` 设置 `TrackGrooveBrush` 和 `TrackBarBrush`。 |
| Mark 颜色 | `SliderTheme.axaml` 和 `SliderTrackTheme.axaml` 设置 mark brush。 |
| Thumb 尺寸 | `SliderThumbTheme.axaml` 设置 `Width`、`Height`、`ThumbCircleSize` 和边框。 |
| Thumb outline | `SliderThumbTheme.axaml` 设置 focus / hover outline。 |

`SliderToken.CalculateTokenValues()` 使用 SharedToken 推导默认值：

- rail 厚度固定为 Slider 视觉语义下的基础轨道厚度。
- thumb 圆点尺寸来自全局控件高度比例。
- hover 圆点和边框比普通态更大。
- active / hover / disabled 颜色来自 SharedToken 主题色、填充色和 disabled 语义。
- outline 颜色由 active thumb 色生成透明投影感。

## 4. 控件家族影响

SliderToken 只服务 Slider、SliderTrack 和 SliderThumb，不应被 NumericUpDown、Progress、ScrollBar 或其他数值控件直接消费。

Token 变更需要评估：

- `SliderTheme.axaml`
- `SliderTrackTheme.axaml`
- `SliderThumbTheme.axaml`
- Gallery Slider 示例
- Gallery Slider Design Token 表
- `SliderTrack.Render` 和 `SliderThumb.Render` 的直接绘制结果

如果某个视觉值同时影响 Progress、ScrollBar 或其他轨道类控件，应先确认是否属于 SharedToken 或独立控件 Token，而不是直接复用 SliderToken。

## 5. 兼容性要求

SliderToken 属于 Slider 主题契约。即使 `SliderToken` 是 internal 类型，生成的 `SliderTokenKind`、AXAML resource 使用点和 Gallery Token 表已经形成稳定依赖。

Token 变更要求：

- 不擅自重命名既有 Token。
- 不擅自删除既有 Token。
- 不改变 `SliderTrackSize`、`RailSize`、`ThumbSize`、`ThumbCircleSize`、`ThumbCircleSizeHover` 的尺寸语义。
- 不把 `Value`、`RangeValue`、`Marks`、tooltip 文本或 pointer 状态迁移为 Token。
- 不在 SliderToken 中复制 preset 色表或硬编码业务颜色。
- `ThumbSize` 必须继续能容纳 hover/focus 最大圆点、边框和 outline。
- Padding Token 必须继续区分 horizontal 和 vertical orientation。
- 需要破坏性变更时，必须先说明影响范围并获得授权。

## 6. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| 新增 SliderToken | 检查生成的 `SliderTokenKind`、AXAML 引用、Gallery Token 表和默认值计算。 |
| 修改轨道尺寸 | 验证 horizontal / vertical 布局、thumb 居中、mark 文本位置和 tooltip。 |
| 修改 thumb 尺寸 | 验证 normal、hover、focus、drag、disabled 状态，以及 thumb 不裁剪 outline。 |
| 修改颜色 Token | 验证普通、hover、active、disabled、included / not included 和 dark theme。 |
| 修改 padding Token | 验证带 mark 与不带 mark 场景，horizontal / vertical 轨道和 label 不重叠。 |
| 删除或重命名 Token | 默认不允许；如获授权，需同步所有 AXAML 引用、生成文件、Gallery Token 表和控件文档。 |
