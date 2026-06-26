# Segmented 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.Segmented` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [Segmented 桌面版实现原理](implementation.md)，Segmented Token 的专项设计见 [Segmented Token 设计](token.md)，设计和契约变化记录见 [Segmented Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented` |
| 控件状态 | Stable |

Segmented 是桌面端数据展示类的紧凑单选控件，用于在少量互斥选项之间切换当前视图、展示模式、时间粒度或状态筛选。它通过分段轨道、选项 item、选中滑块、图标和文本表达“当前只有一个有效选择”的语义。

Segmented 的职责是承载一组有限选项并维护单选状态。它不是完整导航系统、Tab 容器、列表选择器、弹层选择器、表单编辑器或大数据集合控件；复杂页面导航、动态搜索、虚拟化、多选和异步加载应由专用控件或业务层承担。

## 2. 设计语言

Segmented 的设计语言来自 参考设计体系的分段控制器：浅色轨道承载一组互斥选项，选中滑块跟随当前项移动，选项文本和图标在 hover、pressed、selected、disabled 状态下提供即时反馈。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 单选语义 | 同一组中只有一个选项处于选中态。 | `SelectedIndex`、`SelectedItem`、`:selected`。 |
| 视觉反馈 | 用滑块和 item 状态区分 hover、pressed、selected。 | `SelectedThumbBg`、item 背景和前景色。 |
| 信息密度 | 通过尺寸类型控制高度、字体、图标和内边距。 | `SizeType=Large/Middle/Small/Custom`。 |
| 铺满能力 | 在块级场景中让可见 item 等分父容器宽度。 | `IsExpanding`。 |
| 图文组合 | 允许纯文本、纯图标或图标加文本。 | `SegmentedItem.Icon`、`Content`、`:has-icon`。 |
| 动效一致性 | 选中滑块移动和 item 背景变化受全局 motion 控制。 | `IsMotionEnabled`。 |

## 3. API 与契约模型

Segmented 的公共契约由根控件、item 容器、继承的选择 API 和主题契约组成。

根控件 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `SizeType` | `CustomizableSizeType` | 控制轨道圆角、item 高度、字体、padding 和图标尺寸，支持 `Large`、`Middle`、`Small`、`Custom`。 |
| `IsExpanding` | `bool` | 是否让可见 item 等分可用宽度；为 `true` 时根控件水平对齐为 stretch。 |
| `IsMotionEnabled` | `bool` | 是否启用选中滑块位置/尺寸动画和 item 背景动画，默认来自 `SharedToken.EnableMotion`。 |
| `SelectedIndex` / `SelectedItem` | inherited | 当前选择，来自 `SelectingItemsControl`。 |
| `SelectionChanged` | inherited event | 选择变化事件，来自 `SelectingItemsControl`。 |
| `Items` / `ItemsSource` / `ItemTemplate` | inherited | 选项集合和内容模板入口。 |

item API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `SegmentedItem.Icon` | `PathIcon?` | 选项左侧图标；非空时启用 `:has-icon`。 |
| `SegmentedItem.IsSelected` | `bool` | item 选中状态，接入 Avalonia `ISelectable` 选择模型。 |
| `Content` / `ContentTemplate` | inherited | item 内容和模板。 |

稳定 template part 和主题节点：

| 模板 | 节点 | 职责 |
| --- | --- | --- |
| `SegmentedTheme.axaml` | `Frame` | 根轨道裁剪、圆角和 padding 承载。 |
| `SegmentedTheme.axaml` | `PART_ItemsPresenter` | item presenter 和 `SegmentedStackPanel` 承载。 |
| `SegmentedItemTheme.axaml` | `Frame` | item 背景、圆角和 padding 承载。 |
| `SegmentedItemTheme.axaml` | `IconPresenter` | `SegmentedItem.Icon` 的视觉承载。 |
| `SegmentedItemTheme.axaml` | `Content` | item 内容承载。 |

稳定伪类：

- `:selected`：item 被选中。
- `:pressed`：item 按下态。
- `:has-icon`：item 设置了 `Icon`。
- `:pointerover` 和 `:disabled`：由 Avalonia 标准状态驱动，主题使用这些状态表达 hover 和 disabled 视觉。

`Segmented` 桌面层默认为非 visual 数据项创建 `SegmentedItem` 容器，并把 `SizeType`、`IsMotionEnabled` 传递给容器。`PrepareSegmentedItem(SegmentedItem, object?, int)` 是受保护扩展点，用于派生控件补充容器准备逻辑。

## 4. 行为与状态模型

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

## 5. 视觉与主题模型

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

## 6. 控件家族或集成关系

Segmented 的共享实现位于 `AtomUI.Controls`，桌面实现位于 `AtomUI.Desktop.Controls`。

集成关系：

- `AbstractSegmented`：共享单选、motion、form、布局和滑块渲染逻辑。
- `Segmented`：桌面公开控件，注册 `SegmentedToken.ScopeProvider`，创建 `SegmentedItem` 容器。
- `AbstractSegmentedItem`：共享 item 选择、图标伪类和 pointer release 选择逻辑。
- `SegmentedItem`：桌面公开 item，注册 `SegmentedToken.ScopeProvider`。
- `SegmentedStackPanel`：内部 items panel，负责普通排列和 expanding 等分排列。
- Form：通过 `IFormItemAware` 把 `SelectedItem` 暴露为表单值。
- Gallery：通过 Segmented ShowCase 展示基础、块级、禁用、尺寸、纯图标和图标文本示例，并展示 API 与 Token 表。

## 7. 兼容性不变量

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

## 8. 专项模型

### 8.1 选中滑块模型

选中滑块是根控件 render 层的视觉，不是单独的 visual child。它的 `SelectedThumbPos` 来自选中容器相对根控件的坐标，`SelectedThumbSize` 来自选中容器的 `DesiredSize`。当选择或根控件尺寸变化时，控件重新计算该矩形。

该模型要求 item 布局完成后再同步滑块尺寸。修改布局、容器创建或选择时序时，必须验证滑块和 item 边界一致。

### 8.2 Expanding 布局模型

`SegmentedStackPanel` 是 Segmented 专用 items panel。非 expanding 模式按 item 自然宽度测量和排列；expanding 模式按可见 item 数量等分可用宽度。隐藏 item 不占宽度，也不影响等分计数。

### 8.3 Custom SizeType 模型

`SizeType=Custom` 是 `ICustomizableSizeTypeAware` 的自定义尺寸入口。默认主题下，Custom 使用 Middle 分支作为初始视觉基线；实例上的显式属性值覆盖主题 setter。控件不新增 Segmented 专属 `CustomHeight`、`CustomPadding` 等公开 API。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [Segmented 桌面版实现原理](implementation.md)
- [Segmented Token 设计](token.md)
- [Segmented Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Segmented` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + Gallery API / Token / ShowCase | 生成 `controls/segmented/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/segmented/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | Gallery DesignTokenDataGrid、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 层次 | 验证内容 |
| --- | --- |
| Public API | `SizeType`、`IsExpanding`、`IsMotionEnabled`、`SelectedIndex`、`SelectedItem`、`SelectionChanged`、`SegmentedItem.Icon`、`SegmentedItem.IsSelected`。 |
| 状态行为 | 默认选择、显式选择保留、绑定选择保留、pointer release 选择、Form value、disabled item、hidden item。 |
| AXAML / Template | 根 `Frame`、`PART_ItemsPresenter`、`SegmentedStackPanel`、item `Frame`、`IconPresenter`、`Content` 和 SizeType 样式分支。 |
| Token | 轨道 padding/background、item 文本/背景状态色、item 最小高度、图标和图文间距。 |
| Gallery | Basic、Block、Disabled、Sizes、Icon Only、With Icon 示例，以及 API/Design Token 表。 |
| 文档 | 运行 `git diff --check`，检查相对链接存在。 |
