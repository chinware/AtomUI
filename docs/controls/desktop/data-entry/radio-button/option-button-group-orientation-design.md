# OptionButtonGroup 方向布局设计

本文档定义 `OptionButtonGroup` 的横向与纵向布局契约、组合几何、尺寸映射、渲染职责和定制边界。公共控件家族设计见 [RadioButton 桌面版架构设计](overview.md)，源码职责与生命周期见 [RadioButton 桌面版实现原理](implementation.md)，视觉变量见 [RadioButton Token 设计](token.md)。

## 1. 设计定位

`OptionButtonGroup` 将互斥选项呈现为共享外边框和相邻分隔线的按钮组。`Orientation` 决定选项沿水平轴还是垂直轴连接，但不改变选择模型、事件语义、数据容器生成方式或 `ButtonStyle` 状态视觉。

方向布局覆盖以下场景：

- 直接声明 `OptionButton` 子项。
- 通过 `ItemsSource` 和 `ItemTemplate` 生成选项容器。
- `Outline` 与 `Solid` 两种按钮样式。
- `Large`、`Middle`、`Small` 和 `Custom` 尺寸模式。
- 运行时方向切换、集合增删移动、容器回收和主题变化。

## 2. 设计原则

- `Orientation` 是排列方向、组合圆角、分隔线方向和方向键导航的唯一 public source of truth。
- 横向与纵向复用同一个 `OptionButtonGroup`、选择模型、ControlTheme 和 renderer，不复制控件类型或模板。
- 布局使用 Avalonia 标准 `StackPanel` 语义；不为方向切换引入专用 Panel、`Block` 或 `IsExpanding` API。
- Group 拥有组合几何和选择边框，Item 拥有单项内容、背景、前景、Wave 和有效圆角。
- 横向既有行为是兼容性基线；纵向只改变方向相关布局和几何。
- 方向和位置是实例状态，不进入 Design Token。
- 运行时方向切换依赖 Avalonia 属性失效与状态投影，不使用延迟刷新、强制同步或反射发现。

## 3. 方向模型与 Public API

`AbstractOptionButtonGroup` 公开 Avalonia 标准方向枚举：

```csharp
public static readonly StyledProperty<Orientation> OrientationProperty =
    StackPanel.OrientationProperty.AddOwner<AbstractOptionButtonGroup>();

public Orientation Orientation { get; set; }
```

`Orientation` 默认值为 `Orientation.Horizontal`。不增加方向专属枚举，也不改变 `OptionButtonStyle` 和 `OptionButtonPositionTrait` 的值。

方向语义矩阵：

| `Orientation` | 主轴 | 默认排列 | 共享边界 | 方向键导航 |
| --- | --- | --- | --- | --- |
| `Horizontal` | X 轴 | 沿布局起始方向排列 | 垂直分隔线 | Left / Right |
| `Vertical` | Y 轴 | 从上到下排列 | 水平分隔线 | Up / Down |

`FlowDirection` 继续由 Avalonia 布局和视觉变换处理。垂直模式的首尾始终表示上端和下端；横向模式不得增加与 Avalonia 镜像机制竞争的第二套 RTL 排序状态。

## 4. 尺寸与宽度策略

### 4.1 方向尺寸

| 方向 | Group 主轴尺寸 | Item 主轴尺寸 | 交叉轴尺寸 |
| --- | --- | --- | --- |
| Horizontal | Group 高度由 `SizeType` 或用户值决定 | Item 由横向 Panel 排列并共享 Group 高度 | 每项保持自然宽度 |
| Vertical | Group 高度为所有可见 Item 高度之和 | 每项高度由 `SizeType` 或用户样式决定 | 所有 Item 共享 Group 最终宽度 |

纵向 Group 不使用单行固定高度。空纵向 Group 在没有显式 `Height` 或 `MinHeight` 时保持零内容高度。

### 4.2 纵向宽度

纵向宽度遵循 Avalonia 原生布局属性：

- `HorizontalAlignment=Stretch` 时，Group 使用父容器提供的可用宽度，所有 Item 等宽铺满。
- `HorizontalAlignment=Left`、`Center` 或 `Right` 且没有显式宽度时，Group 使用最宽 Item 的自然宽度，其他 Item 拉伸到该宽度。
- 显式 `Width` 时，Group 和所有 Item 使用该宽度。
- Group 不增加 `Block` 或 `IsExpanding` 属性，业务布局继续通过 `Width`、`MinWidth`、`MaxWidth` 和 Alignment 表达。

默认 Panel 不换行、不换列且 `Spacing=0`。调用方替换 `ItemsPanel` 时，必须保证 Panel 的排列方向与 Group 的 `Orientation` 一致；AtomUI 不从任意自定义 Panel 反向推断方向。

### 4.3 Custom 尺寸

`OptionButtonGroup` 遵循 `ICustomizableSizeTypeAware` 契约：

- ControlTheme 先提供可用的 Middle 基础值。
- `Small`、`Middle`、`Large` selector 映射预设 Token。
- 不为相关尺寸维度定义 `SizeType=Custom` selector，也不把 `Custom` 合并进 `Middle` selector。
- 横向 `Custom` 可以通过 Group 的 `Height` 接管行高。
- 纵向 `Custom` 的单项高度通过显式 `OptionButton.Height`、Item 样式或 `ItemContainerTheme` 接管；Group 的 `Height` 仍表示整个控件高度，不重解释为单项高度。

## 5. 架构与职责

| 类型或文件 | 职责 | 主要输入 | 输出或可观察结果 |
| --- | --- | --- | --- |
| `AbstractOptionButtonGroup` | 方向和选择 owner；投影容器位置；绘制外边框、分隔线和选中边框 | Orientation、Items、SelectedIndex、ButtonStyle、Bounds、Border | 组合状态和组级几何 |
| `AbstractOptionButton` | 呈现单项状态；根据方向和位置计算有效圆角 | CornerRadius、GroupOrientation、GroupPositionTrait | Item 背景、边框、Wave 有效圆角 |
| `OptionButtonGroupTheme.axaml` | 组合 ItemsPresenter，绑定默认 ItemsPanel，映射 Group 尺寸和资源 | Orientation、SizeType、ItemsPanel | 标准布局和主题基线 |
| `OptionButtonTheme.axaml` | 映射 Item 尺寸、内容对齐、状态颜色和 Wave | GroupOrientation、SizeType、ButtonStyle、checked 状态 | 单项布局和状态视觉 |
| 标准 `StackPanel` | 测量和排列容器 | Orientation、available/final size | 横向自然宽度或纵向等宽布局 |

不引入方向专用控件、自定义布局 Panel、方向策略对象或每个 Item 的方向订阅。方向变化频率低，Group 在属性变化和容器生命周期入口中投影内部值即可。

## 6. Template 与组合契约

默认组合结构：

```text
OptionButtonGroup
  -> PixelAlignedBorder#Frame
     -> ItemsPresenter#PART_ItemsPresenter
        -> StackPanel
           -> OptionButton...

OptionButton
  -> Panel
     -> WaveSpiritDecorator#PART_WaveSpirit
     -> DockPanel#ContentLayout
        -> IconPresenter#IconPresenter
        -> TextBlock
```

- `PART_ItemsPresenter` 和 `PART_WaveSpirit` 保持稳定。
- `Frame` 负责模板承载边界；Group renderer 负责组合外边框，不能让 Frame 重复绘制同一边框。
- 默认 ItemsPanel 将 `Orientation` 单向绑定到 Group。
- 横向 `ContentLayout` 保持居中。
- 纵向 `ContentLayout` 在 Item 内 Stretch，图标与文本从内容起始侧按 Padding 对齐。
- `WaveSpiritDecorator` 使用 Item 的 `EffectiveCornerRadius`，保证首项、尾项和单项 Wave 与组合外形一致。
- 自定义 ControlTheme 必须继续传递 ItemsPanel、方向和有效圆角所需状态；否则方向布局结果由自定义主题负责。

## 7. 位置、圆角与渲染算法

### 7.1 容器位置

Group 根据集合索引和总数为每个已实现容器计算 `GroupPositionTrait`：

```text
count <= 1             -> OnlyOne
index == 0             -> First
index == count - 1     -> Last
otherwise              -> Middle
```

集合增删、移动、Reset、容器索引变化和重新实现容器时都必须重放位置。算法通过 `ContainerFromIndex()` 访问容器，不能假设 `Items[index]` 本身是 `OptionButton`。

### 7.2 有效圆角

`EffectiveCornerRadius` 从 Item 的 nominal `CornerRadius`、Group 方向和位置单向推导：

| 位置 | Horizontal | Vertical |
| --- | --- | --- |
| `First` | 保留 TopLeft、BottomLeft | 保留 TopLeft、TopRight |
| `Middle` | 四角为 0 | 四角为 0 |
| `Last` | 保留 TopRight、BottomRight | 保留 BottomLeft、BottomRight |
| `OnlyOne` | 保留全部四角 | 保留全部四角 |

不得通过修改 public `CornerRadius` 保存派生值。`CornerRadius`、方向或位置变化时重新计算内部有效值，使运行时主题和尺寸切换不会读取陈旧圆角。

### 7.3 Group renderer

所有绘制坐标使用 Group 本地坐标系。容器 Bounds 必须转换或归一到 Group 本地坐标，不能依赖任意自定义 Panel 与 Group 恰好零偏移。

渲染顺序和规则：

1. Horizontal 使用 `DesiredSize` 保留既有自然内容外边框；Vertical 使用 `Bounds.Size`，使 Stretch 和显式 Width 下的外边框覆盖纵向 Group 实际布局区域。
2. 遍历已实现容器，在相邻容器之间绘制一条共享分隔线。
3. Horizontal 的分隔线位于当前项尾端 X 坐标，线段沿 Group 高度延伸。
4. Vertical 的分隔线位于当前项底部 Y 坐标，线段沿 Group 宽度延伸。
5. `Solid` 模式下，分隔线与选中项相邻时不绘制，避免穿过选中背景。
6. `Outline` 模式下，选中边框覆盖选中项及其前一个共享边界；首项不向起始侧扩展。
7. 选中容器通过 `SelectedIndex` 解析，不能比较容器对象和数据项 `SelectedItem`。

Horizontal 使用横向边界对应的 render-scale-aware thickness，Vertical 使用纵向边界对应的 thickness。分隔线保持 aliased edge mode，外边框和选中边框继续使用 `BorderRenderHelper`。零尺寸、空集合、未实现容器或无有效 Pen 时跳过对应绘制，不生成替代视觉。

## 8. 状态流与生命周期

方向状态流：

```text
OptionButtonGroup.Orientation
  -> default StackPanel.Orientation
  -> realized OptionButton.GroupOrientation
  -> measure / arrange
  -> EffectiveCornerRadius
  -> Group separator and selected-outline renderer
```

- Orientation 在模板应用前设置也必须在容器生成后正确投影。
- 运行时切换 Orientation 使 Group 和 Item 重新测量，并使 Group、Item 和 Wave 重新绘制。
- ChildIndex 订阅在 visual attach 时获取、detach 时释放；重新 attach 后重新获取。
- 清理或回收生成容器时释放 Group 建立的 relay binding、事件和 ContentTemplate，并重置 GroupOrientation 和 GroupPositionTrait。
- 直接声明的 OptionButton 从 Items 移除或 Reset 时也必须释放 Group 建立的 binding、事件和组合状态；不能依赖只覆盖生成容器的清理回调。
- 不为每个 Item 建立 Group 方向 observable、timer、异步任务或全局事件。
- 方向变化不改变 `SelectedIndex`、`SelectedItem`、`SelectionMode`、`OptionCheckedChanged` 或 Form value。

## 9. 资源、性能与 AOT 边界

- 方向、位置、Bounds 和有效圆角都是值类型实例状态，不进入 Token 或 DynamicResource。
- Measure/Arrange 复用标准 StackPanel，方向切换不创建新的 Item 或 Panel。
- 位置投影复杂度为 O(realized item count)，仅在方向或容器结构变化时执行。
- Group renderer 保持 O(item count)，不在每帧创建容器列表、订阅或长期缓存。
- API、属性、selector 和容器访问均为编译期已知，不使用反射、动态类型发现或字符串 binding。
- Gallery 使用普通 AXAML public 属性，不增加 NativeAOT 特殊注册路径。

## 10. 兼容性、定制与验证

兼容性不变量：

- `Orientation` 默认保持 Horizontal。
- Horizontal 的选项顺序、自然宽度、Group 高度、圆角、分隔线、选中边框、动效和事件语义保持不变。
- `OptionButtonStyle`、`OptionButtonPositionTrait`、ControlTheme key、template part 和 Token 名称保持稳定。
- 未显式定制的 `Custom` 使用 Middle 基础视觉，但 Theme 不通过 Custom selector 阻止实例和 owner-scoped Style 接管。
- 自定义 ItemsPanel、ControlTheme 或 ItemContainerTheme 必须保持方向、尺寸和有效圆角协作契约。

验证矩阵：

| 层级 | 必须覆盖 |
| --- | --- |
| API | Orientation 类型、默认 Horizontal、运行时切换和属性失效 |
| 布局 | Horizontal/Vertical、Stretch/自然/显式宽度、空组、单项、长文本 |
| 尺寸 | Orientation x Large/Middle/Small/Custom，Custom 实例和 ItemContainerTheme 覆盖 |
| 集合 | 直接 Items、ItemsSource、Add/Remove/Move/Reset、容器回收和重新 attach |
| 几何 | 方向 x First/Middle/Last/OnlyOne x 非对称 CornerRadius |
| 渲染 | Outline/Solid、首中尾选中、相邻分隔线、非均匀 BorderThickness、1x/1.5x/2x scaling |
| 交互 | Horizontal Left/Right、Vertical Up/Down、disabled 和运行时切换后选择保持 |
| Theme | ItemsPanel 方向绑定、内容对齐、三档 SizeType、Custom 无专属 selector、Light/Dark |
| Gallery | 纵向 OptionButtonGroup 的 Stretch、自然宽度和选中行为 |
| AOT | 无反射或字符串 binding；Gallery 编译和既有 NativeAOT 路径不新增 warning |
