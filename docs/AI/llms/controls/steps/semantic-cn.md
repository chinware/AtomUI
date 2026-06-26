# Steps 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Steps` | 导航控件根语义区域，承载 public API、状态归一和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载点击、键盘、打开关闭、跳转或提交入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `导航项区域` | 承载当前项、选中项、禁用项、层级项或分页项状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或内容区域` | 承载 flyout、dropdown、tab content、submenu 或候选内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达打开关闭、选中指示、切换和过渡反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Steps/Themes/StepsTheme.axaml`

```xml
<ItemsPresenter Name="PART_ItemsPresenter" />
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Steps
  -> StepsItemIndicator (control theme, StepsItemIndicatorTheme.axaml)
     -> Panel (template-stable)
        -> Border#ProgressFrame (template-stable)
           -> Border#Frame (template-stable)
              -> Panel (template-stable)
                 -> TextBlock#PositionText (template-stable)
                 -> CheckOutlined#FinishedMark (template-stable)
                 -> CloseOutlined#ErrorMark (template-stable)
        -> IconPresenter#CustomIconPresenter (internal-observable)
     -> Border#Frame (template-stable)
     -> Border#Frame (template-stable)
  -> StepsItem (item container control theme, StepsItemTheme.axaml)
     -> DockPanel#RootLayout (template-stable)
        -> StepsItemIndicator#PART_Indicator (template-stable)
        -> StackPanel (template-stable)
           -> DockPanel (template-stable)
              -> StackPanel#HeaderLayout (template-stable)
                 -> ContentPresenter#HeaderPresenter (internal-observable)
                 -> ContentPresenter#SubHeaderPresenter (internal-observable)
              -> Rectangle#SeparatorLine (template-stable)
           -> ContentPresenter#DescriptionPresenter (internal-observable)
     -> DockPanel#RootLayout (template-stable)
        -> StackPanel#ContentLayout (template-stable)
           -> Grid#IndicatorLayout (template-stable)
              -> Rectangle#BeginningSeparatorLine (template-stable)
              -> StepsItemIndicator#PART_Indicator (template-stable)
              -> Rectangle#EndingSeparatorLine (template-stable)
           -> StackPanel#HeaderLayout (template-stable)
              -> ContentPresenter#HeaderPresenter (internal-observable)
              -> ContentPresenter#SubHeaderPresenter (internal-observable)
           -> ContentPresenter#DescriptionPresenter (internal-observable)
        -> Panel#TailLineLayout (template-stable)
           -> Rectangle#TailSeparatorLine (template-stable)
     -> DockPanel#RootLayout (template-stable)
        -> StackPanel#ContentLayout (template-stable)
           -> Grid#IndicatorLayout (template-stable)
              -> Rectangle#BeginningSeparatorLine (template-stable)
              -> StepsItemIndicator#PART_Indicator (template-stable)
              -> Rectangle#EndingSeparatorLine (template-stable)
           -> StackPanel#HeaderLayout (template-stable)
              -> ContentPresenter#HeaderPresenter (internal-observable)
              -> ContentPresenter#SubHeaderPresenter (internal-observable)
           -> ContentPresenter#DescriptionPresenter (internal-observable)
        -> Panel#TailLineLayout (template-stable)
           -> Rectangle#TailSeparatorLine (template-stable)
     -> DockPanel#RootLayout (template-stable)
        -> DockPanel#IndicatorLayout (template-stable)
           -> StepsItemIndicator#PART_Indicator (template-stable)
           -> Rectangle#SeparatorLine (template-stable)
        -> StackPanel#ContentLayout (template-stable)
           -> StackPanel#HeaderLayout (template-stable)
              -> ContentPresenter#HeaderPresenter (internal-observable)
              -> ContentPresenter#SubHeaderPresenter (internal-observable)
           -> ContentPresenter#DescriptionPresenter (internal-observable)
  -> Steps (control theme, StepsTheme.axaml)
     -> ItemsPresenter#PART_ItemsPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Steps` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `StepsItemIndicator` | control theme | `StepsItemIndicatorTheme.axaml` | Steps | `Background`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `Height`, `Icon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | `Background`, `BorderBrush`, `CornerRadius`, `Icon`, `Position` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ProgressFrame` | template node (Border) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | `Background`, `BorderBrush`, `CornerRadius`, `Icon`, `Position` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (Border) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | `Background`, `BorderBrush`, `CornerRadius`, `Icon`, `Position` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PositionText` | template node (TextBlock) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | `Position` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FinishedMark` | template node (CheckOutlined) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ErrorMark` | template node (CloseOutlined) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CustomIconPresenter` | template node (IconPresenter) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | `Icon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `StepsItem` | item container control theme | `StepsItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `CornerRadius`, `Description`, `DescriptionForeground`, `DescriptionTemplate`, `Foreground` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `RootLayout` | template node (DockPanel) | `StepsItemTheme.axaml` | StepsItem | `Description`, `DescriptionForeground`, `DescriptionTemplate`, `Foreground`, `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Indicator` | template node (StepsItemIndicator) | `StepsItemTheme.axaml` | StepsItem | `Icon`, `IndicatorType`, `IsClickable`, `IsEffectiveShowProgress`, `IsMotionEnabled`, `IsSelected` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `StepsItemTheme.axaml` | StepsItem | `Description`, `DescriptionForeground`, `DescriptionTemplate`, `Foreground`, `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `StepsItemTheme.axaml` | StepsItem | `Foreground`, `Header`, `HeaderTemplate`, `IsLast`, `SubHeader`, `SubHeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderLayout` | template node (StackPanel) | `StepsItemTheme.axaml` | StepsItem | `Foreground`, `Header`, `HeaderTemplate`, `SubHeader`, `SubHeaderTemplate`, `SubTitleForeground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderPresenter` | template node (ContentPresenter) | `StepsItemTheme.axaml` | StepsItem | `Foreground`, `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SubHeaderPresenter` | template node (ContentPresenter) | `StepsItemTheme.axaml` | StepsItem | `SubHeader`, `SubHeaderTemplate`, `SubTitleForeground` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SeparatorLine` | template node (Rectangle) | `StepsItemTheme.axaml` | StepsItem | `IsLast` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DescriptionPresenter` | template node (ContentPresenter) | `StepsItemTheme.axaml` | StepsItem | `Description`, `DescriptionForeground`, `DescriptionTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentLayout` | template node (StackPanel) | `StepsItemTheme.axaml` | StepsItem | `Description`, `DescriptionForeground`, `DescriptionTemplate`, `Foreground`, `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IndicatorLayout` | template node (Grid) | `StepsItemTheme.axaml` | StepsItem | `Icon`, `IndicatorType`, `IsClickable`, `IsEffectiveShowProgress`, `IsFirst`, `IsLast` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `BeginningSeparatorLine` | template node (Rectangle) | `StepsItemTheme.axaml` | StepsItem | `IsFirst` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `EndingSeparatorLine` | template node (Rectangle) | `StepsItemTheme.axaml` | StepsItem | `IsLast` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TailLineLayout` | template node (Panel) | `StepsItemTheme.axaml` | StepsItem | `IsLast` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TailSeparatorLine` | template node (Rectangle) | `StepsItemTheme.axaml` | StepsItem | `IsLast` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IndicatorLayout` | template node (DockPanel) | `StepsItemTheme.axaml` | StepsItem | `Icon`, `IndicatorType`, `IsClickable`, `IsEffectiveShowProgress`, `IsLast`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Steps` | control theme | `StepsTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `StepsTheme.axaml` | Steps | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 模板 | 节点 | 职责 |
| --- | --- | --- |
| `StepsTheme.axaml` | `PART_ItemsPresenter` | 承载 StepsItem 容器和 Grid items panel。 |
| `StepsItemTheme.axaml` | `PART_Indicator` | 单个步骤指示器。 |
| `StepsItemTheme.axaml` | `RootLayout` | item 主布局容器。 |
| `StepsItemTheme.axaml` | `IndicatorLineLayout` / `IndicatorLayout` | 指示器与连接线布局。 |
| `StepsItemTheme.axaml` | `HeaderPresenter` / `SubHeaderPresenter` / `DescriptionPresenter` | 标题、副标题和描述承载。 |
| `StepsItemTheme.axaml` | `NavArrow` / `NavArrowLayout` | Navigation 风格下的步骤跳转箭头和箭头对齐槽位。 |
| `StepsItemIndicatorTheme.axaml` | `ProgressFrame` / `Frame` | 默认指示器和进度环承载。 |
| `StepsItemIndicatorTheme.axaml` | `PositionText` / `FinishedMark` / `ErrorMark` / `CustomIconPresenter` | 数字、完成、错误和自定义图标视觉。 |

## Pseudo Classes

稳定伪类：

- `:horizontal`：根控件水平排列。
- `:vertical`：根控件垂直排列。
- `:finished`：StepsItem 位于当前步骤之前。
- `:selected`、`:pointerover`、`:pressed`、`:disabled`：由 Avalonia 标准选择和交互状态驱动。

## State Flow

Steps 的核心状态流：

```text
CurrentStep / SelectedIndex
      ↓
SelectingItemsControl selected item
      ↓
ConfigureCurrentStepsItem
      ↓
Position / IsFirst / IsLast / IsFinished / Status
      ↓
StepsItem theme + StepsItemIndicator theme
```

当前步骤语义：

- `CurrentStep` 在有效范围内同步到 `SelectedIndex`。
- `CurrentStep` 超出范围时 `SelectedIndex` 被设置为 `-1`。
- `InitialStep != -1` 时，模板应用阶段把 `InitialStep` 写入 `CurrentStep`。
- `SelectedIndex` 变化后会重新计算所有 item 的位置、完成态和状态。

状态计算语义：

- 已选择步骤之前的 item 设置为 `Finish` 并进入 `:finished`。
- 当前 item 使用 `CurrentStepStatus`。
- 当前 item 之后的 item 使用 `Wait`。
- 如果 `StepsItem.Status` 以本地值显式设置，它优先于根控件按模板优先级写入的状态。
- `ProgressValue` 只对当前默认指示器有效，且仅在 `IsShowItemProgress=true`、非 `Inline`、无自定义 `Icon`、非 `Dot` 时展示。

点击选择语义：

- `IsItemClickable=false` 时，Steps 不通过 pointer 改变选择。
- 鼠标左键按下在 item 上触发选择。
- 非鼠标 pointer 在左键释放且仍命中原 item 时触发选择。
- 点击选择复用 `SelectingItemsControl` 的 selection pipeline，保持 `SelectedIndex`、`SelectedItem` 和 `IsSelected` 一致。

内容语义：

- `CurrentContent` 跟随当前选中容器的 `Content`。
- `CurrentContentTemplate` 优先使用当前容器的 `ContentTemplate`，为空时回退到根 `ContentTemplate`。
- 当前选择清空时，`CurrentContent` 和 `CurrentContentTemplate` 同时清空。

## Theme and Token Boundaries

Steps 的视觉由根主题、item 主题、indicator 主题、StepsToken 和 SharedToken 共同决定。

| 主题文件 | 职责 |
| --- | --- |
| `StepsTheme.axaml` | 根模板、ItemsPresenter、Grid items panel、根间距和 Dot 风格 spacing。 |
| `StepsItemTheme.axaml` | item 模板、Default/Navigation/Inline 三种风格、水平/垂直布局、标题/描述、连接线、Navigation 箭头和状态颜色。 |
| `StepsItemIndicatorTheme.axaml` | 默认指示器、点状指示器、内联点、完成/错误图标、自定义图标、进度环和 motion transition。 |
| `StepsThemes.axaml` | 汇总 Steps 相关主题资源。 |

主题分层：

```text
SharedToken
   ↓
StepsToken
   ↓
StepsTheme / StepsItemTheme / StepsItemIndicatorTheme
   ↓
root layout + item state + indicator + tail line + navigation arrow
```

视觉风格：

- `Default`：展示指示器、连接线、标题、副标题、描述和可选进度环。
- `Navigation`：增加导航线和箭头，选中态通过导航指示线表达。
- `Inline`：以右侧紧凑点状步骤表达流程状态，用于和正文内容并排展示。

指示器类型：

- `Default` 使用数字、完成图标、错误图标或自定义图标。
- `Dot` 使用点状指示器，当前步骤使用 `DotCurrentSize`。
- `Inline` 总是使用 inline 点状视觉，不复用默认数字容器。

Navigation 箭头的槽位必须不小于箭头自身尺寸，且仍与对应指示器首行中心对齐。维护 Dot Navigation 模板时，不能只把箭头宿主高度收敛到 dot 尺寸，否则会裁剪箭头图标。

Token 边界：

StepsToken 是 Steps 的组件级 Token scope，描述步骤指示器、点状指示器、连接线、标题描述状态色、Navigation 箭头、Inline 风格和 progress ring 的主题语义。

StepsToken 不承载以下状态：

- `CurrentStep`、`SelectedIndex`、`SelectedItem` 或 `CurrentContent`。
- `StepsItem.Status`、`:finished`、`:selected`、pointer over、pressed 或 disabled 状态本身。
- item 数量、Grid 行列、箭头实际坐标或 progress ring 当前角度。
- `IsItemClickable`、`IsShowItemProgress`、`IsMotionEnabled` 等实例行为属性。

## Customization Boundaries

维护 Steps 时必须保持以下不变量：

- `Steps` 继承 `SelectingItemsControl`，`SelectionMode` 保持单选。
- `Orientation` 默认值保持 `Horizontal`。
- `CurrentStep` 与 `SelectedIndex` 的同步语义保持不变。
- `InitialStep=-1` 表示不接管初始当前步骤。
- `ProgressValue` 必须保持 `0..100` coercion。
- `StepsItem.Status` 本地值优先于根控件模板优先级状态。
- `CurrentContent` 和 `CurrentContentTemplate` 必须跟随当前选中容器，并在选择清空时清空。
- `PART_ItemsPresenter` 和 `PART_Indicator` 的名称和职责不能在未授权情况下改变。
- `Default`、`Navigation`、`Inline` 三种 `StepsStyle` 的名称、布局语义和主要视觉职责保持不变。
- `Default` 与 `Dot` 两种 `StepsItemIndicatorType` 的名称和主题分支保持不变。
- `Navigation` 水平 Dot 模式下，箭头图标不能被宿主槽位裁剪。
- `IsShowItemProgress` 不能在 Dot、Inline 或自定义 Icon 分支下强行展示进度环。
- 动态添加、删除或重排 item 时，Grid 行列定义和 item 的 Grid 位置必须同步更新。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- `CurrentStep` 写入必须同步 `SelectedIndex`。
- `SelectedItem` 变化必须更新 `CurrentContent`。
- 动态 item 增删、容器准备、index 变化和容器清理必须触发布局同步。
- `Status` 派生必须使用不覆盖本地值的优先级。
- `IsEffectiveShowProgress` 必须排除 `Inline`、custom icon 和 dot indicator。
- `PART_ItemsPresenter` 使用 Grid items panel，行列定义必须与 `ItemCount` 一致。
- `PART_Indicator` 是 item 与 indicator 交互状态转发的稳定 template part。
- Navigation 箭头宿主不得小于箭头自身尺寸。
- Dot Navigation 的 dot 与箭头必须在同一个首行槽位内居中。
- 不能通过关闭 motion 来规避状态同步、点击或布局问题。
