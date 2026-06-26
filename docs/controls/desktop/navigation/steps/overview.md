# Steps 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.Steps` 桌面版的最新设计定位、公共契约、步骤状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [Steps 桌面版实现原理](implementation.md)，Steps Token 的专项设计见 [Steps Token 设计](token.md)，设计和契约变化记录见 [Steps Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Navigation/Steps` |
| 控件状态 | Stable |

Steps 是 AtomUI 桌面导航体系中的步骤进度控件，用于表达线性流程中的当前位置、已完成步骤、等待步骤和异常步骤。它适合安装向导、表单分步、审批流、支付流程、任务进度和带内容切换的步骤页。

Steps 的职责是生成步骤容器、维护当前步骤、计算每个步骤的状态、展示连接线、指示器、标题、副标题、描述、进度环和导航样式。它不负责业务流程校验、页面路由、异步任务编排、表单提交、权限控制或步骤内容的生命周期管理。

Steps 支持两种 item 入口：

- 直接在 `Steps.Items` 中放置 `StepsItem`。
- 通过 `Items` / `ItemsSource` 放置非 visual 数据项，由控件生成 `StepsItem` 容器并把数据项作为 item content。

## 2. 设计语言

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

## 3. API 与契约模型

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

## 4. 行为与状态模型

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

## 5. 视觉与主题模型

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

## 6. 控件家族或集成关系

Steps 位于 Desktop Navigation 分类，与 NavMenu、Breadcrumb、Pagination、TabControl 等控件同属导航体系，但不共享路由或菜单状态模型。

集成关系：

- `SelectingItemsControl`：提供 `Items`、`ItemsSource`、`SelectedIndex`、`SelectedItem` 和容器生成。
- `StepsItem`：公开 item 容器，承载 header、sub header、description、content、icon 和 status。
- `StepsItemIndicator`：内部指示器控件，承载数字、图标、dot 和进度环。
- AtomUI Token：通过 `StepsToken.ScopeProvider` 注册组件级资源。
- Motion：主题 transition 由 `IsMotionEnabled` 和 SharedToken motion duration 控制。
- Gallery：展示基础、小尺寸、自定义图标、交互切换、垂直、错误、Dot、可点击、Navigation、进度、LabelPlacement 和 Inline 示例，并展示 API 与 Design Token 表。

Steps 不实现 Form、CompactSpace、Popup 或路由接口。

## 7. 兼容性不变量

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

## 8. 专项模型

### 8.1 CurrentStep 与 Selection 模型

`CurrentStep` 是 Steps 的流程入口，`SelectedIndex` 是底层选择状态。`CurrentStep` 写入时同步 `SelectedIndex`，点击可选 item 时通过 selection pipeline 更新选择。维护者修改选择逻辑时，必须同时验证 root 状态、item `IsSelected`、`Status`、`:finished` 和 `CurrentContent`。

### 8.2 Status 派生模型

默认状态由当前选择派生，但 item 允许显式设置 `Status`。根控件使用模板优先级写入派生状态，以保留用户在 `StepsItem` 上设置的本地状态。该模型让 Gallery Navigation 示例可以在 item 上直接指定完成、处理中和等待状态。

### 8.3 Progress Indicator 模型

进度环由 `StepsItemIndicator.Render` 绘制。它只在当前 item、默认指示器、无自定义 icon、非 inline、非 dot 且 `IsShowItemProgress=true` 时生效。进度环大小来自指示器实际尺寸和 `ProgressFramePadding`，数值来自根控件 `ProgressValue`。

### 8.4 Navigation Arrow 模型

Navigation 风格下，水平模板把箭头放入右侧 `NavArrowLayout`，该槽位负责和指示器首行对齐，并提供足够绘制空间。默认指示器使用 `IconSize` / `IconSizeSM` 作为槽位高度；Dot 指示器使用 `IconFontSize` 保证箭头不被裁剪，同时让 dot 和箭头在同一首行槽位内居中。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [Steps 桌面版实现原理](implementation.md)
- [Steps Token 设计](token.md)
- [Steps Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Steps` | 导航控件根语义区域，承载 public API、状态归一和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载点击、键盘、打开关闭、跳转或提交入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `导航项区域` | 承载当前项、选中项、禁用项、层级项或分页项状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或内容区域` | 承载 flyout、dropdown、tab content、submenu 或候选内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达打开关闭、选中指示、切换和过渡反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + Gallery API / Token / ShowCase | 生成 `controls/steps/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/steps/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | Gallery DesignTokenDataGrid、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 层次 | 验证内容 |
| --- | --- |
| Public API | `CurrentStep`、`InitialStep`、`ProgressValue`、`CurrentStepStatus`、`Orientation`、`LabelPlacement`、`SizeType`、`ItemIndicatorType`、`Style`、`IsItemClickable`、`IsShowItemProgress`、`ContentTemplate`、`CurrentContent`。 |
| Item API | `Header`、`SubHeader`、`Description`、`Content`、`Icon`、`Status`、`IsSelected`。 |
| 状态行为 | CurrentStep/SelectedIndex 同步、InitialStep 应用、Status 派生、本地 Status 优先、动态 item 增删、CurrentContent 更新。 |
| AXAML / Template | `PART_ItemsPresenter`、`PART_Indicator`、Default/Navigation/Inline、Horizontal/Vertical、Dot、Small、LabelPlacement、Navigation 箭头槽位。 |
| Token | 指示器尺寸、dot 尺寸、状态色、连接线、Navigation 箭头、Inline token、进度环 token。 |
| Gallery | Basic、Mini、With Icon、Switch Step、Vertical、Error、Dot、Clickable、Navigation、Progress、LabelPlacement、Inline 示例，以及 API/Design Token 表。 |
| 文档 | 运行 `git diff --check`，检查相对链接存在。 |
