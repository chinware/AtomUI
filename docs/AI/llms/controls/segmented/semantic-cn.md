# Segmented 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Segmented` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

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

`Segmented` 桌面层默认为非 visual 数据项创建 `SegmentedItem` 容器，并把 `SizeType`、`IsMotionEnabled` 传递给容器。`PrepareSegmentedItem(SegmentedItem, object?, int)` 是受保护扩展点，用于派生控件补充容器准备逻辑。

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

尺寸语义：

- `Large`、`Middle`、`Small` 进入对应主题分支。
- `Custom` 复用 Middle 作为默认视觉基线；用户可在实例或 item 上显式设置 `Padding`、`MinHeight`、`FontSize`、`CornerRadius` 等 Avalonia 属性形成自定义尺寸。
- 根 `SizeType` 会同步给生成的 `SegmentedItem`，保证 item 的主题分支和根控件一致。

展开布局语义：

- `IsExpanding=false` 时，item 按自身期望宽度顺序排列，根控件默认左对齐。
- `IsExpanding=true` 时，可见 item 等分可用宽度，隐藏 item 不参与等分计数。

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
| `SegmentedTheme.axaml` | 根模板、轨道 padding/background、选中滑块资源、SizeType 圆角分支、展开对齐和滑块动画。 |
| `SegmentedItemTheme.axaml` | item 模板、图标/内容布局、hover/pressed/selected/disabled 状态、SizeType 尺寸分支和图标尺寸。 |
| `SegmentedThemes.axaml` | 汇总 Segmented 相关主题资源。 |

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

根控件在 `Render()` 中绘制轨道背景和选中滑块。item 模板绘制每个选项自身的背景、图标、内容和状态颜色。选中滑块位置来自当前选中容器相对根控件的坐标，尺寸来自当前选中容器的 `DesiredSize`。

Token 边界：

SegmentedToken 是 Segmented 的组件级 Token scope，描述分段轨道、选项文本状态、选项背景状态、选中滑块背景和 item 尺寸的主题语义。

SegmentedToken 不承载以下状态：

- `Items`、`ItemsSource`、`ItemTemplate`、`Content` 等数据状态。
- `SelectedIndex`、`SelectedItem`、`:selected`、`:pressed`、`:has-icon` 等实例或伪类状态本身。
- `SelectedThumbPos`、`SelectedThumbSize` 等运行时布局派生状态。
- `IsExpanding`、可见 item 数量、等分宽度等布局状态。
- `IsMotionEnabled` 或 transition 时长开关；motion 时长来自 SharedToken。

## Customization Boundaries

维护 Segmented 时必须保持以下不变量：

- `SelectionMode` 保持单选。
- 未提供选择且存在 item 时，模板应用后默认选择第一个 item。
- 已绑定或显式设置的 `SelectedIndex` / `SelectedItem` 不能被默认选择覆盖。
- 鼠标左键释放触发 item 选择的语义不能擅自改为按下触发。
- `Segmented` 必须为非 visual item 创建 `SegmentedItem` 容器。
- 容器准备时必须把根 `SizeType` 和 `IsMotionEnabled` 同步给 item。
- `IsExpanding=true` 时只按可见 `SegmentedItem` 等分宽度。
- 选中滑块必须跟随当前选中容器的位置和尺寸。
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
- 生成容器必须接收 owner 的 `SizeType` 和 `IsMotionEnabled`。
- `IsExpanding` 只能按可见 `AbstractSegmentedItem` 计数。
- 选中滑块矩形必须跟随当前选中容器的实际布局结果。
- 根 render 绘制和 item 主题状态不能互相替代；轨道/滑块在根，item 状态在 item。
- `Custom` 尺寸分支默认基线保持 Middle，除非获得 API/主题契约变更授权。
