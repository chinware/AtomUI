# Steps

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

Steps 是 AtomUI 桌面导航体系中的步骤进度控件，用于表达线性流程中的当前位置、已完成步骤、等待步骤和异常步骤。它适合安装向导、表单分步、审批流、支付流程、任务进度和带内容切换的步骤页。

Steps 的职责是生成步骤容器、维护当前步骤、计算每个步骤的状态、展示连接线、指示器、标题、副标题、描述、进度环和导航样式。它不负责业务流程校验、页面路由、异步任务编排、表单提交、权限控制或步骤内容的生命周期管理。

Steps 支持两种 item 入口：

- 直接在 `Steps.Items` 中放置 `StepsItem`。
- 通过 `Items` / `ItemsSource` 放置非 visual 数据项，由控件生成 `StepsItem` 容器并把数据项作为 item content。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Navigation/Steps` |
| 状态 | Stable |

## 何时使用

Steps 表达的是“线性流程 + 当前进度”的导航语义。用户应能通过顺序编号、点状指示器、完成标记、错误标记、进度环、连接线和文字状态快速判断流程位置。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 顺序语义 | 每个步骤有稳定顺序和位置编号。 | `Position`、水平/垂直排列。 |
| 当前语义 | 当前步骤由根控件控制。 | `CurrentStep`、`SelectedIndex`、`CurrentStepStatus`。 |
| 历史语义 | 当前步骤之前的步骤默认为完成。 | `StepsItemStatus.Finish`、`:finished`。 |
| 等待语义 | 当前步骤之后的步骤默认为等待。 | `StepsItemStatus.Wait`。 |
| 错误语义 | 当前步骤可进入错误态。 | `CurrentStepStatus=Error` 或 `StepsItem.Status=Error`。 |
| 导航语义 | 可点击步骤可作为轻量导航入口。 | `IsItemClickable=true`、`Style=Navigation`。 |
| 密度语义 | 通过尺寸类型控制指示器和间距。 | `SizeType=Large/Middle/Small`。 |

`Style=Default` 强调普通流程进度；`Style=Navigation` 强调步骤作为导航入口；`Style=Inline` 用于紧凑的内联流程状态。

## 公共 API

Steps 的公共契约由根控件 API、item API、枚举模型、选择 API 和主题契约组成。

根控件 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `CurrentStep` | `int` | 当前步骤索引，默认 `0`；有效范围内会同步到 `SelectedIndex`。 |
| `InitialStep` | `int` | 模板应用时写入 `CurrentStep` 的初始步骤，默认 `-1` 表示不接管。 |
| `ProgressValue` | `double` | 当前步骤进度环数值，写入时被约束到 `0..100`。 |
| `CurrentStepStatus` | `StepsItemStatus` | 当前步骤的默认状态，默认 `Process`。 |
| `Orientation` | `Orientation` | 步骤排列方向，默认 `Horizontal`。 |
| `LabelPlacement` | `Orientation` | 水平方向下标题与指示器的布局关系，默认 `Horizontal`。 |
| `SizeType` | `SizeType` | 指示器、文字和间距尺寸，默认 `Middle`。 |
| `ItemIndicatorType` | `StepsItemIndicatorType` | 指示器类型，支持 `Default` 和 `Dot`。 |
| `Style` | `StepsStyle` | 视觉风格，支持 `Default`、`Navigation`、`Inline`。 |
| `IsMotionEnabled` | `bool` | 是否启用主题过渡，默认来自 `SharedToken.EnableMotion`。 |
| `IsItemClickable` | `bool` | item 是否可通过 pointer 选择，默认 `false`。 |
| `IsShowItemProgress` | `bool` | 当前步骤是否展示进度环，默认 `false`。 |
| `ContentTemplate` | `IDataTemplate?` | 当前步骤内容的 fallback 模板。 |
| `CurrentContent` | `object?` | 当前选中步骤的内容，只读。 |
| `CurrentContentTemplate` | `IDataTemplate?` | 当前选中步骤的有效内容模板，只读。 |
| `SelectedIndex` / `SelectedItem` | inherited | 选择状态，来自 `SelectingItemsControl`。 |
| `Items` / `ItemsSource` / `ItemTemplate` | inherited | 步骤集合和 item 模板入口。 |

StepsItem API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Header` / `HeaderTemplate` | inherited | 步骤标题。 |
| `SubHeader` / `SubHeaderTemplate` | `object?` / `IDataTemplate?` | 标题旁的辅助信息，例如耗时或剩余时间。 |
| `Description` / `DescriptionTemplate` | `object?` / `IDataTemplate?` | 步骤说明文本或说明模板。 |
| `Content` / `ContentTemplate` | inherited | 选中步骤内容，由 `CurrentContent` 读取。 |
| `Icon` | `PathIcon?` | 自定义指示器图标；设置后进入 custom indicator 分支。 |
| `Status` | `StepsItemStatus` | item 显式状态；本地值优先于根控件计算出的模板优先级状态。 |
| `IsSelected` | `bool` | item 选择状态，接入 Avalonia `ISelectable`。 |

枚举契约：

| 枚举 | 成员 | 语义 |
| --- | --- | --- |
| `StepsItemStatus` | `Wait`、`Process`、`Finish`、`Error` | 步骤状态。 |
| `StepsItemIndicatorType` | `Default`、`Dot` | 默认数字/图标指示器或点状指示器。 |
| `StepsStyle` | `Default`、`Navigation`、`Inline` | 普通、导航和内联视觉风格。 |

稳定 template part 和主题节点：

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

稳定伪类：

- `:horizontal`：根控件水平排列。
- `:vertical`：根控件垂直排列。
- `:finished`：StepsItem 位于当前步骤之前。
- `:selected`、`:pointerover`、`:pressed`、`:disabled`：由 Avalonia 标准选择和交互状态驱动。

## 事件与命令

Steps 的事件与命令以公共 API、Avalonia 基类契约和 Gallery API 表为准；生成器不从源码发明额外事件。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml:144`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Steps CurrentStep="0">
    <atom:StepsItem Header="已完成" Description="这是一段描述。" />
    <atom:StepsItem Header="进行中" Description="这是一段描述。" SubHeader="剩余 00:00:08" />
    <atom:StepsItem Header="等待中" Description="这是一段描述。" />
</atom:Steps>
```

### 迷你版本

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml:161`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:Steps CurrentStep="0" SizeType="Small">
    <atom:StepsItem Header="已完成" Description="这是一段描述。" />
    <atom:StepsItem Header="进行中" Description="这是一段描述。" SubHeader="剩余 00:00:08" />
    <atom:StepsItem Header="等待中" Description="这是一段描述。" />
</atom:Steps>
```

### 垂直方向

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml:233`

Gallery key：`ExamplesContent` / item `4`

```axaml
<atom:Steps CurrentStep="1" Orientation="Vertical">
    <atom:StepsItem Header="已完成" Description="这是一段描述。" />
    <atom:StepsItem Header="进行中" Description="这是一段描述。" SubHeader="剩余 00:00:08" />
    <atom:StepsItem Header="等待中" Description="这是一段描述。" />
</atom:Steps>
```

### 垂直迷你版本

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml:250`

Gallery key：`ExamplesContent` / item `5`

```axaml
<atom:Steps CurrentStep="1" Orientation="Vertical" SizeType="Small">
    <atom:StepsItem Header="已完成" Description="这是一段描述。" />
    <atom:StepsItem Header="进行中" Description="这是一段描述。" SubHeader="剩余 00:00:08" />
    <atom:StepsItem Header="等待中" Description="这是一段描述。" />
</atom:Steps>
```

## 状态模型

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

## 主题与 Design Token

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

Token 来源：

StepsToken 是 Steps 的组件级 Token scope，描述步骤指示器、点状指示器、连接线、标题描述状态色、Navigation 箭头、Inline 风格和 progress ring 的主题语义。

StepsToken 不承载以下状态：

- `CurrentStep`、`SelectedIndex`、`SelectedItem` 或 `CurrentContent`。
- `StepsItem.Status`、`:finished`、`:selected`、pointer over、pressed 或 disabled 状态本身。
- item 数量、Grid 行列、箭头实际坐标或 progress ring 当前角度。
- `IsItemClickable`、`IsShowItemProgress`、`IsMotionEnabled` 等实例行为属性。

## AOT 与裁剪注意事项

资源边界：

- `StepsToken` 通过 token generator 注册，主题通过 `StepsTokenResource` 消费。
- 状态色、尺寸、间距和 progress 色均由 StepsToken / SharedToken 提供。
- 自定义 icon 通过显式 `PathIcon` 或 IconProvider 入口传入，不在 Steps 内部做运行时图标扫描。

生命周期边界：

- `_currentItemSubscriptions` 必须在选择变化和选择清空时释放。
- 动态 item 变化后必须重建 Grid 行列定义并同步 item Grid 位置。
- `StepsItem` 和 `StepsItemIndicator` 初始加载时禁用 transition，加载后再启用，避免初始渲染动画。

AOT 边界：

- 不新增运行时反射扫描、字符串路径动态绑定或 C# 创建的 template 绑定。
- 模板内固定关系优先使用 `TemplateBinding`、selector 和 TokenResource。
- 需要跨 part 对齐时优先使用同源 token，而不是前向 element-name binding。

性能边界：

- item 状态同步按 `ItemCount` 线性遍历，适合步骤数量有限的流程控件。
- 根 layout 使用 Grid definitions 表达列/行，不在 render 热路径中动态创建视觉。
- progress ring 只在当前有效 item 的 indicator render 中绘制。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Steps/Steps.cs`：公开控件、根属性、容器生成、Grid items panel 配置、当前步骤同步、当前内容跟踪和 pointer 选择。
- `src/AtomUI.Desktop.Controls/Steps/StepsItem.cs`：公开 item 容器、item 内容契约、内部状态、指示器 template part 接入、hover 转发、进度可见性计算和 transition 启用时序。
- `src/AtomUI.Desktop.Controls/Steps/StepsItemIndicator.cs`：内部指示器控件、数字/图标/dot 状态、progress ring 绘制和尺寸变化圆角同步。
- `src/AtomUI.Desktop.Controls/Steps/StepsPseudoClass.cs`：StepsItem 完成态伪类常量。
- `src/AtomUI.Desktop.Controls/Steps/StepsToken.cs`：Steps 组件 Token。
- `src/AtomUI.Desktop.Controls/Steps/Themes/StepsTheme.axaml`：根模板和根 spacing。
- `src/AtomUI.Desktop.Controls/Steps/Themes/StepsItemTheme.axaml`：item 模板、状态色、连接线、Default/Navigation/Inline 分支和 Navigation 箭头布局。
- `src/AtomUI.Desktop.Controls/Steps/Themes/StepsItemIndicatorTheme.axaml`：默认指示器、dot、inline dot、自定义 icon、完成/错误 icon 和 progress frame。
- `src/AtomUI.Desktop.Controls/Steps/Themes/StepsThemes.axaml`：Steps 主题汇总入口。
- `tests/AtomUI.Desktop.Controls.Tests/Steps/StepsDynamicItemsTests.cs`：动态 item 增加时 Grid 行列和位置同步回归。
- `tests/AtomUI.Desktop.Controls.Tests/Steps/StepsNavigationLayoutTests.cs`：Navigation 箭头对齐、裁剪和主题契约回归。

## 相关文档

- 源设计文档：`docs/controls/desktop/navigation/steps/overview.md`
- 实现文档：`docs/controls/desktop/navigation/steps/implementation.md`
- Token 文档：`docs/controls/desktop/navigation/steps/token.md`
- 变更记录：`docs/controls/desktop/navigation/steps/changelog.md`
- 语义结构：`./semantic-cn.md`
