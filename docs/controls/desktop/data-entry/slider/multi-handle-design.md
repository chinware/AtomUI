# Slider 多 Handle 设计

本文档定义 Slider 的多 handle 范围模型、指定 handle 禁用、整体活动范围拖动、轨道语义和动态视觉节点结构。Slider 控件定位见 [Slider 桌面版架构设计](overview.md)，内部职责和生命周期见 [Slider 桌面版实现原理](implementation.md)，Token 语义见 [Slider Token 设计](token.md)。

## 1. 设计定位

Slider Range 模式使用一个有序数值集合表达任意数量的 handle。每个 handle 都拥有独立的值、索引、焦点、Tooltip、拖动和 disabled 状态；相邻 handle 之间形成独立轨道段，首个 handle 到最后一个 handle 形成整体活动范围。

该模型覆盖：

- 两个或更多 handle 的范围选择。
- 按索引禁用指定 handle。
- 在没有 disabled handle 时整体平移活动范围。
- 多段轨道和整体轨道的独立绘制。
- 水平、垂直和反向坐标下的统一交互。

动态增删 handle、`editable`、`minCount` 和 `maxCount` 不属于该模型。

## 2. 设计原则

- `RangeValues` 是 Range 模式唯一值源，不为起止 handle 建立特殊字段。
- 单值模式使用 `RangeBase.Value`，Range 模式使用 `RangeValues`。
- handle 的索引是值集合归一化后的稳定位置。
- disabled handle 是固定交互锚点，不参与拖动和键盘编辑。
- 存在任意 disabled handle 时，整体活动范围不可拖动。
- `TrackBarBrush` 表达相邻 handle 之间的轨道段，`TracksBrush` 表达首尾 handle 之间的整体区域。
- handle 数量变化只改变动态视觉节点数量，不改变 Slider 的公共模板结构。
- 状态由 Slider 持有，SliderTrack 负责布局、绘制和输入几何，SliderThumb 负责单个 handle 的视觉与输入事件。

## 3. 模型与 Public API

### 3.1 值模型

```csharp
public IReadOnlyList<double>? RangeValues { get; set; }
```

`IsRangeMode=true` 时，`RangeValues` 至少包含两个值。控件接收值后依次执行：

1. 拒绝 `NaN` 和无穷值。
2. 裁剪到 `[Minimum, Maximum]`。
3. 按数值升序归一化。
4. 保留重复值和 handle 数量。

RangeValues 少于两个值时使用 `[Minimum, Minimum]`，避免 Range 模式生成无效视觉结构。集合属性使用快照语义，调用方通过重新赋值触发更新，不依赖集合内部变更通知。

### 3.2 Handle 状态

```csharp
public IReadOnlyList<bool>? DisabledHandles { get; set; }
```

`DisabledHandles[index]` 对应归一化后的 handle 索引。缺失项按 `false` 处理，超出 handle 数量的项忽略。

单个 handle disabled 时：

- thumb 不响应 pointer drag。
- thumb 不可获得键盘焦点。
- thumb 不响应键盘值修改。
- thumb 使用 disabled 视觉。
- 其值作为相邻 enabled handle 的固定边界。

### 3.3 轨道行为和视觉

```csharp
public bool IsDraggableTrack { get; set; }
public IBrush? TrackBarBrush { get; set; }
public IBrush? TracksBrush { get; set; }
```

`IsDraggableTrack=true` 仅在 Range 模式、handle 数量不少于两个且没有 disabled handle 时生效。拖动时所有 handle 平移相同数值并保持相对间距。

`TrackBarBrush` 绘制每个相邻 handle 之间的 segment；`TracksBrush` 绘制首个 handle 到最后一个 handle 的整体范围。两种画刷都受 `IsIncluded` 控制。

## 4. 架构与职责

### Slider

Slider 是公共状态 owner，负责：

- `Value`、`RangeValues`、`DisabledHandles` 和行为属性。
- 把模板绑定的数据传递给 SliderTrack。
- 接收 SliderTrack 的内部交互结果并提交新的值集合。
- Tooltip 文本、Form 值和数据校验。
- 单值与 Range 模式切换。

### SliderTrack

SliderTrack 负责：

- 根据 handle 数量创建和回收 SliderThumb。
- 计算所有 handle 的中心点。
- 计算 segment rect 和整体 tracks rect。
- 绘制 rail、整体活动范围、局部轨道段和 marks。
- 将 pointer 坐标转换为 handle 或整体拖动请求。
- 维护动态 thumb、渲染几何和全局 focus 处理订阅的生命周期。

### SliderThumb

SliderThumb 负责：

- 单个 handle 的圆点和 outline 绘制。
- pointer capture 和 drag routed event。
- focus、pressed、disabled 状态。
- 暴露内部 `HandleIndex`，不直接维护业务值。

### 内部状态

实现不创建并行的 `HandleState` 数据模型。SliderTrack 按相同索引关联三类派生状态：

```text
EffectiveRangeValues[index] -> SliderThumb.HandleIndex
EffectiveRangeValues        -> RenderContextData.SegmentRects / TrackRangeRect
DisabledHandles[index]      -> SliderThumb.IsEnabled + SliderRangeMath boundary
```

`EffectiveRangeValues` 是 `Value` 或 `RangeValues` 的只读投影，动态 thumb 保存在 SliderTrack 的内部列表中，渲染几何在 `PrepareRenderInfo()` 中从当前值重新计算。值变化时复用已有 thumb；只有 handle 数量变化时才创建或回收视觉节点。整体活动范围的拖动快照由 Slider 持有，不属于 SliderTrack 的渲染状态。

## 5. Template 与主题集成

Slider 的稳定模板部件只有：

```text
PART_Track -> SliderTrack
```

SliderTheme 不再声明固定数量的 thumb。SliderTrack 直接管理动态 SliderThumb，所有 thumb 通过类型主题使用 SliderThumbTheme。

SliderTheme 将以下状态传递给 SliderTrack：

- `Minimum`
- `Maximum`
- `Value`
- `RangeValues`
- `DisabledHandles`
- `Orientation`
- `IsDirectionReversed`
- `IsIncluded`
- `IsDraggableTrack`
- `TrackBarBrush`
- `TracksBrush`

SliderThumbTheme 使用标准 `:disabled` 状态表达单个 handle disabled。disabled selector 必须优先于 hover 和 focus selector，避免 disabled thumb 被放大或显示 active outline。

## 6. 核心算法

### 6.1 Handle 布局

输入：

- `Minimum`、`Maximum`。
- 归一化后的 handle values。
- `Orientation`。
- `IsDirectionReversed`。
- SliderTrack 实际可用尺寸。
- thumb 尺寸。

输出：

- 每个 handle 的中心点。
- 每个相邻 handle 之间的 segment rect。
- 首个 handle 到最后一个 handle 的 tracks rect。

值到坐标的计算先扣除 thumb 半径，再把 `[Minimum, Maximum]` 映射到可用轨道长度。垂直模式反转 Y 轴；`IsDirectionReversed` 在最终坐标映射中反转数值方向。

### 6.2 单 Handle 移动

点击轨道后先进行命中判断：

1. 命中 disabled thumb 时终止操作。
2. 命中 enabled thumb 时操作该 thumb。
3. 未命中 thumb 时选择最近的 enabled handle。
4. 根据前后 disabled handle 计算有效边界。
5. 将新值裁剪到有效边界。
6. 应用 tick 吸附并提交新的完整 RangeValues。

disabled 边界只约束 enabled handle，不改变 disabled handle 的值。

### 6.3 整体轨道拖动

拖动会话保存：

```text
InitialValues
PointerOrigin
MinimumOffset
MaximumOffset
```

每次 pointer move 根据坐标轴计算偏移量：

```text
offset = pointerDelta * valueDensity
offset = clamp(offset, MinimumOffset, MaximumOffset)
next[i] = InitialValues[i] + offset
```

所有 handle 使用同一个 offset，保证内部间距不变。启用 tick 吸附时对整个结果集合统一格式化。

任意 handle disabled 时不创建 track drag session。

### 6.4 Range Form 值

单值模式 Form 值为 `double`，Range 模式 Form 值为 `IReadOnlyList<double>`。Form 设置值和用户交互写值都经过同一个归一化入口，避免绑定路径和 pointer 路径产生不同结果。

## 7. 生命周期、性能与 AOT

- 动态 thumb 由 SliderTrack 创建、持有和释放。
- handle 数量变化时解绑并回收多余 thumb 的 drag 事件、templated parent 和 logical / visual tree 关联。
- SliderTrack detach 时释放全局 input subscription 和当前 track drag session。
- pointer capture lost 或 pointer released 会结束 thumb drag；整体活动范围的 session 在 Slider pointer release 时清理。
- handle 数量通常较少，不使用 ItemsControl、虚拟化或反射动态发现。
- pointer move 热路径只更新值快照、坐标和 render context，不创建新的控件。
- `RangeValues` 和 `DisabledHandles` 不订阅外部集合事件，避免集合 owner 生命周期不明确。
- 不使用反射、运行时类型扫描或动态模板发现，保持 trimming 和 NativeAOT 友好。

## 8. 定制边界

- `TrackBarBrush` 和 `TracksBrush` 是实例级画刷，不属于 SliderToken。
- SliderToken 只提供默认轨道、rail、thumb、mark 和 outline 视觉值。
- 外部可以替换 SliderTheme，但必须继续提供 `PART_Track`。
- 外部不能依赖固定的 StartThumb 或 EndThumb；handle 数量由 RangeValues 决定。
- disabled handle 的交互保护由控件逻辑负责，主题只负责视觉表达。
- Gallery 的渐变颜色计算属于示例 ViewModel，不进入 Slider 控件内部。

## 9. 验证要求

### 纯逻辑

- 值有限性、裁剪、排序和重复值处理。
- disabled handle 索引映射。
- 最近 enabled handle 选择。
- disabled 边界计算。
- 整体拖动 offset 和边界限制。

### 控件行为

- 两个、三个和更多 handle 的创建、回收和复用。
- pointer、keyboard、focus、Tooltip 和 Form。
- 部分 disabled 与全部 disabled。
- 整体活动范围拖动和相对距离保持。
- Horizontal、Vertical、Reversed 和 tick 吸附。

### 主题和 Gallery

- `PART_Track` 模板结构。
- SliderThumb disabled 样式不被 hover/focus 覆盖。
- TrackBarBrush、TracksBrush 和 IsIncluded 的绘制关系。
- 多点组合和禁用指定滑块 Showcase。
- Light / Dark 主题和 Gallery 页面结构。
