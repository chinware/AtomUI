# Segmented 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Segmented 主控件公开 `root`、`item`、`icon` 与 `label` 四个职责区域，与上游稳定 Semantic DOM 对齐。上游基线为
6.6.0 稳定发布的 `SegmentedSemanticType`（`classNames` / `styles` 均为 `{ root?, icon?, label?, item? }`）：

- `root` 消费于 `.ant-segmented` 根节点，`icon` 消费于 `.ant-segmented-item-icon`，由上游 `Segmented` 组件下发；
- `item` 消费于 `.ant-segmented-item` 选项容器，`label` 消费于 `.ant-segmented-item-label`，由 rc-segmented 选项
  渲染路径消费；
- 上游选中滑块（MotionThumb）没有 Semantic key，AtomUI 同样不公开。

AtomUI 四个 Part 随本次 Semantic Part 改造同时公开，descriptor 的 `Since` 统一为 `6.0`。

`SegmentedItem` 不持有独立 Semantic descriptor：

- 上游 `Segmented` 只提供一个 owner 的 Semantic DOM；选项没有独立公开的 Semantic DOM Props。
- `SegmentedItem` 是 Segmented 的运行时容器，其职责通过 `Segmented` 的 `item` Part 对外公开；item 模板内的图标与
  文本节点通过 `icon`、`label` Part 以多跳 route 公开。
- `AbstractSegmented` 与 `AbstractSegmentedItem` 是跨平台共享基类，不是对应用公开的独立 owner，不声明 descriptor。

因此本控件的 Semantic Part 只由 `Segmented` owner 公开。

### 1.1 `Segmented`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `Segmented` |
| Part | `root` |
| Selector | Segmented 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `Segmented` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Segmented owner（表面投影到 `Frame`） |
| 职责 | Segmented root 是选项数据、选择状态、方向、形状与轨道表面样式的统一 owner。轨道背景由 owner `Render` 直接绘制，`Frame` 承载圆角、内边距与内容裁剪。 |
| 相关 API | `ItemsSource`、`ItemTemplate`、`SelectedIndex`、`SelectedItem`、`SelectionChanged`、`SizeType`、`Orientation`、`Shape`、`IsExpanding`、`IsMotionEnabled` |
| 相关 Token | `TrackBg`、`TrackPadding`、SharedToken |
| 稳定性 | stable since 6.0 |

#### `item`

| 字段 | 值 |
| --- | --- |
| Owner | `Segmented` |
| Part | `item` |
| Selector | `.semantic-item` |
| SelectorRoute | `> .semantic-item` |
| Style Type | `SegmentedItemStyle` |
| ContractType | `SegmentedItem` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 `SegmentedItem` 容器 |
| 职责 | 统一表示单个选项容器的背景、前景、圆角、内边距、最小高度、光标与选择 / 悬浮 / 按压 / 禁用视觉；对应上游 `.ant-segmented-item`。 |
| 相关 API | `SegmentedItem.Icon`、`SegmentedItem.Content`、`SegmentedItem.IsSelected`、`SizeType`、`Shape`、`IsMotionEnabled` |
| 相关 Token | `ItemColor`、`ItemHoverColor`、`ItemSelectedColor`、`ItemHoverBg`、`ItemActiveBg`、`ItemSelectedBg`、`ItemMinHeightLG`、`ItemMinHeight`、`ItemMinHeightSM`、`SegmentedItemPadding`、`SegmentedItemPaddingSM` |
| 稳定性 | stable since 6.0 |

#### `icon`

| 字段 | 值 |
| --- | --- |
| Owner | `Segmented` |
| Part | `icon` |
| Selector | `.semantic-icon` |
| SelectorRoute | `> .semantic-item /template/ .semantic-icon` |
| Style Type | `SegmentedIconStyle` |
| ContractType | `IconPresenter` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 `SegmentedItem` 模板中的 `IconPresenter#IconPresenter` |
| 职责 | 统一表示每个选项的图标区域：图标画刷状态色、图标尺寸与可见性；对应上游 `.ant-segmented-item-icon`。 |
| 相关 API | `SegmentedItem.Icon`、`SizeType` |
| 相关 Token | `ItemColor`、`ItemHoverColor`、`ItemSelectedColor`、SharedToken（`IconSizeLG` / `IconSize` / `IconSizeSM`、`ColorTextDisabled`） |
| 稳定性 | stable since 6.0 |

#### `label`

| 字段 | 值 |
| --- | --- |
| Owner | `Segmented` |
| Part | `label` |
| Selector | `.semantic-label` |
| SelectorRoute | `> .semantic-item /template/ .semantic-label` |
| Style Type | `SegmentedLabelStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 `SegmentedItem` 模板中的 `ContentPresenter#Content` |
| 职责 | 统一表示每个选项的文本区域：文本呈现、居中对齐、省略与图文间距；对应上游 `.ant-segmented-item-label`。 |
| 相关 API | `SegmentedItem.Content`、`SegmentedItem.ContentTemplate` |
| 相关 Token | `SegmentedItemContentMargin` |
| 稳定性 | stable since 6.0 |

`root` 是隐式 Part，不声明 `.semantic-root` marker。`item` 的 marker `.semantic-item` 在 `SegmentedItem` 创建路径
一次性添加，`PrepareContainerForItemOverride` 幂等补齐（覆盖回收容器与用户直接提供容器的路径）。`icon`、`label`
的 marker 声明在 `SegmentedItemTheme.axaml` 模板内的 `IconPresenter#IconPresenter` 与 `ContentPresenter#Content`
节点上，随容器模板实例化而存在；由于它们只在运行时随 item 容器创建，descriptor 声明为 `RuntimeCreated`，route 以
`> .semantic-item` 为作用域跳点，再经 `/template/` 进入 item 模板。

`ContractType` 只定义 Setter 可以稳定依赖的最低 public 类型，并通过 `x:SetterTargetType` 提供 AXAML 编译期类型
上下文；它不参与 `.semantic-*` 的身份匹配。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedTheme.axaml`

```xml
<Border Name="Frame">
    <ItemsPresenter Name="PART_ItemsPresenter" />
</Border>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Segmented
  -> SegmentedItem (item container control theme, SegmentedItemTheme.axaml)
     -> Border#Frame (template-stable)
        -> DockPanel (template-stable)
           -> IconPresenter#IconPresenter (internal-observable)
           -> ContentPresenter#Content (internal-observable)
  -> Segmented (control theme, SegmentedTheme.axaml)
     -> Border#Frame (template-stable)
        -> ItemsPresenter#PART_ItemsPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Segmented` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `SegmentedItem` | item container control theme | `SegmentedItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `Content`, `ContentTemplate`, `CornerRadius`, `Icon`, `Padding` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `SegmentedItemTheme.axaml` | SegmentedItem | `Background`, `Content`, `ContentTemplate`, `CornerRadius`, `Icon`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `SegmentedItemTheme.axaml` | SegmentedItem | `Content`, `ContentTemplate`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IconPresenter` | template node (IconPresenter) | `SegmentedItemTheme.axaml` | SegmentedItem | `Icon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Content` | template node (ContentPresenter) | `SegmentedItemTheme.axaml` | SegmentedItem | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Segmented` | control theme | `SegmentedTheme.axaml` | 用户代码 / 控件宿主 | `CornerRadius`, `Padding` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `SegmentedTheme.axaml` | Segmented | `CornerRadius`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `SegmentedTheme.axaml` | Segmented | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 模板 | 节点 | 职责 |
| --- | --- | --- |
| `SegmentedTheme.axaml` | `Frame` | 根轨道裁剪、圆角和 padding 承载。 |
| `SegmentedTheme.axaml` | `PART_ItemsPresenter` | item presenter 和 `SegmentedStackPanel` 承载。 |
| `SegmentedItemTheme.axaml` | `Frame` | item 背景、圆角和 padding 承载。 |
| `SegmentedItemTheme.axaml` | `IconPresenter` | `SegmentedItem.Icon` 的视觉承载。 |
| `SegmentedItemTheme.axaml` | `Content` | item 内容承载。 |

## Pseudo Classes

稳定伪类：

- `:selected`：item 被选中。
- `:pressed`：item 按下态。
- `:has-icon`：item 设置了 `Icon`。
- `:pointerover` 和 `:disabled`：由 Avalonia 标准状态驱动，主题使用这些状态表达 hover 和 disabled 视觉。

`SegmentedShape` 包含 `Default` 和 `Round`。该枚举只表达 Segmented 家族的轨道形状，不复用包含 `Circle` 等无效值的其他控件 Shape 枚举。

`Segmented` 桌面层默认为非 visual 数据项创建 `SegmentedItem` 容器，并把 `SizeType`、`Shape`、`IsMotionEnabled` 传递给容器。`PrepareSegmentedItem(SegmentedItem, object?, int)` 是受保护扩展点，用于派生控件补充容器准备逻辑。

## State Flow

Segmented 的核心状态流：

```text
Items / ItemsSource
      ↓
SegmentedItem containers
      ↓
ISelectable selected state
      ↓
SelectedItem / SelectedIndex
      ↓
selected thumb position + size
```

选择语义：

- 构造阶段固定 `SelectionMode=Single`。
- 模板应用时，如果没有显式 `SelectedIndex`、`SelectedItem` 或已选中容器，并且 `Items.Count > 0`，控件选择第一个 item。
- 已绑定或已显式设置的选择必须在模板应用后保留。
- 鼠标左键释放在 item 上触发选择，避免按下时立即改变选择造成的交互跳变。
- `SelectionChanged` 后如果控件仍附加在视觉树，会重新计算选中滑块的位置和尺寸。
- `Left` / `Up` 选择前一个有效 item，`Right` / `Down` 选择后一个有效 item，并在首尾之间循环。
- 键盘选择跳过 disabled 和 hidden item，选择完成后焦点进入对应容器。

尺寸语义：

- `Large`、`Middle`、`Small` 进入对应主题分支。
- `Custom` 复用 Middle 作为默认视觉基线；用户可在实例或 item 上显式设置 `Padding`、`MinHeight`、`FontSize`、`CornerRadius` 等 Avalonia 属性形成自定义尺寸。
- 根 `SizeType` 会同步给生成的 `SegmentedItem`，保证 item 的主题分支和根控件一致。

方向与展开布局语义：

| `Orientation` | `IsExpanding` | 布局语义 |
| --- | --- | --- |
| `Horizontal` | `false` | item 按自然宽度横向排列，根控件默认左对齐。 |
| `Horizontal` | `true` | 根控件填满父容器宽度，可见 item 等分可用宽度。 |
| `Vertical` | `false` | item 按自然高度纵向排列，轨道宽度取可见 item 的最大自然宽度。 |
| `Vertical` | `true` | 根轨道和 item 填满父容器宽度，item 仍按自然高度纵向排列，不填满父容器高度。 |

隐藏 item 不参与测量累计、排列、expanding 计数或键盘导航。

Form 语义：

- Segmented 实现 `IFormItemAware`。
- `SetFormValue(value)` 设置 `SelectedItem`。
- `GetFormValue()` 返回 `SelectedItem`。
- `ClearFormValue()` 清空 `SelectedItem`。
- `SelectedItem` 变化会通知 Form value changed。

## Theme and Token Boundaries

Segmented 的视觉由根主题、item 主题、专属 Token 和 SharedToken 共同决定。

| 主题文件 | 职责 |
| --- | --- |
| `SegmentedTheme.axaml` | 根模板、轨道 padding/background、选中滑块资源、SizeType/Shape 圆角分支、方向/展开对齐和滑块动画。 |
| `SegmentedItemTheme.axaml` | item 模板、图标/内容布局、hover/pressed/selected/disabled 状态、SizeType/Shape 分支和图标尺寸。 |

视觉关系：

```text
SharedToken
   ↓
SegmentedToken
   ↓
SegmentedTheme / SegmentedItemTheme
   ↓
track + selected thumb + item states
```

根控件在 `Render()` 中绘制轨道背景和选中滑块。item 模板绘制每个选项自身的背景、图标、内容和状态颜色。选中滑块位置来自当前选中容器相对根控件的坐标，尺寸来自当前选中容器最终排列后的 `Bounds.Size`。

`Shape=Default` 时，根、item 和滑块圆角继续由 SizeType 对应的 SharedToken 决定。`Shape=Round` 时，Shape 分支在 SizeType 分支之后统一覆盖根、item 和滑块圆角为胶囊几何；该覆盖不新增 Design Token，也不改变模板结构。

Token 边界：

SegmentedToken 是 Segmented 的控件级 Token scope，描述分段轨道、选项文本状态、选项背景状态、选中滑块背景和 item 尺寸的主题语义。

SegmentedToken 不承载以下状态：

- `Items`、`ItemsSource`、`ItemTemplate`、`Content` 等数据状态。
- `SelectedIndex`、`SelectedItem`、`:selected`、`:pressed`、`:has-icon` 等实例或伪类状态本身。
- `SelectedThumbPos`、`SelectedThumbSize` 等运行时布局派生状态。
- `Orientation`、`IsExpanding`、可见 item 数量、排列轴和等分宽度等布局状态。
- `Shape` 和 Round 胶囊圆角；它们是实例形状状态和几何覆盖，不是主题尺度。
- `IsMotionEnabled` 或 transition 时长开关；motion 时长来自 SharedToken。

## Customization Boundaries

维护 Segmented 时必须保持以下不变量：

- `SelectionMode` 保持单选。
- 未提供选择且存在 item 时，模板应用后默认选择第一个 item。
- 已绑定或显式设置的 `SelectedIndex` / `SelectedItem` 不能被默认选择覆盖。
- 鼠标左键释放触发 item 选择的语义不能擅自改为按下触发。
- `Segmented` 必须为非 visual item 创建 `SegmentedItem` 容器。
- `Orientation` 默认值必须保持 `Horizontal`，`Shape` 默认值必须保持 `Default`。
- 容器准备时必须把根 `SizeType`、`Shape` 和 `IsMotionEnabled` 同步给 item。
- 水平 `IsExpanding=true` 时只按可见 `SegmentedItem` 等分宽度；垂直模式不能因此扩展父容器高度。
- 选中滑块必须跟随当前选中容器的最终排列位置和 `Bounds.Size`。
- 四个方向键必须按前后顺序循环选择，并跳过 disabled 和 hidden item。
- `Frame`、`PART_ItemsPresenter`、`IconPresenter`、`Content` 等模板节点名称和职责不能在未授权情况下改变。
- `:selected`、`:pressed`、`:has-icon` 伪类语义不能改变。
- `SegmentedToken` 的名称、语义和资源使用点不能擅自删除或重命名。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- `SelectionMode=Single` 只能在明确授权时改变。
- 模板应用时不能覆盖已绑定或已显式设置的选择。
- 默认选择第一个 item 的行为只能在没有任何选择输入时触发。
- `SelectionChanged` 订阅和解除必须配对。
- item pointer release 选择路径必须避免重复处理 handled 事件。
- `Orientation` 默认保持 `Horizontal`，`Shape` 默认保持 `Default`。
- 生成容器必须接收 owner 的 `SizeType`、`Shape` 和 `IsMotionEnabled`。
- 水平 `IsExpanding` 只能按可见 `AbstractSegmentedItem` 计数；垂直模式不能扩展父容器高度。
- 选中滑块矩形必须跟随当前选中容器最终的 `Bounds` 布局结果。
- 键盘导航必须首尾循环并跳过 disabled 和 hidden item。
- Round 必须覆盖所有 SizeType 圆角，但不能改变其他尺寸、颜色、状态或模板契约。
- 根 render 绘制和 item 主题状态不能互相替代；轨道/滑块在根，item 状态在 item。
- `Custom` 尺寸分支默认基线保持 Middle，除非获得 API/主题契约变更授权。
- Semantic Part 边界：`Segmented` 只发布 `root` / `item` / `icon` / `label` 四个 Part；`item` 的 marker 在容器创建
  与 prepare 路径一次性幂等建立，`icon` / `label` 的 marker 固定在 `SegmentedItemTheme.axaml` 模板节点上；
  marker 不随选择、图文形态或集合重置增删，默认主题不消费 `.semantic-*` selector；选中滑块与
  `SegmentedStackPanel` 不属于任何 Part。
