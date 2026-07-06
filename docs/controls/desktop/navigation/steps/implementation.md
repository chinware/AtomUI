# Steps 桌面版实现原理

本文档描述 Steps 桌面版的容器生成、当前步骤同步、状态派生、Grid 布局、当前内容跟踪、指示器绘制、Navigation 箭头布局和维护边界。公共设计与 API 契约见 [Steps 桌面版架构设计](overview.md)，Token 语义见 [Steps Token 设计](token.md)，变化记录见 [Steps Changelog](changelog.md)。

## 1. 实现定位

Steps 的实现重点是在 `SelectingItemsControl` 容器体系内维护线性步骤状态，并按 `Style`、`Orientation`、`LabelPlacement` 和 `ItemIndicatorType` 选择不同模板布局。实现文档覆盖 `Steps`、`StepsItem`、`StepsItemIndicator`、`StepsToken` 和 Steps 主题文件的职责边界。

本文档不重复 Avalonia `SelectingItemsControl` 的选择机制，也不展开 TokenResource、Motion transition 或 IconPresenter 的通用实现。

## 2. 源码文件结构

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

## 3. 核心类职责

`Steps` 是步骤根控件。它注册 `StepsToken.ScopeProvider`，固定单选模式，生成 `StepsItem` 容器，把根属性绑定到 item，并在模板接入后配置 Grid items panel。

`StepsItem` 是步骤容器。它承载标题、副标题、描述、内容、自定义 icon 和显式 status，并接收根控件下发的内部状态。它通过 `PART_Indicator` 把 hover 状态转发给指示器，并计算进度环是否有效显示。

`StepsItemIndicator` 是内部视觉控件。它根据 `IndicatorType`、`Status`、`Icon`、`IsCurrent`、`IsClickable`、`IsItemHover` 和 `IsShowProgress` 选择数字、完成图标、错误图标、自定义 icon 或 dot 视觉。进度环由 `Render` 绘制。

`StepsToken` 是组件级主题变量层。它不保存当前步骤、选择状态、item 数量或 progress 值，只把 SharedToken 转换为 Steps 可消费的尺寸、间距和颜色语义。

## 4. 状态与数据流

根状态下发：

```text
Steps public API
  SizeType / Style / ItemIndicatorType / IsItemClickable
  IsMotionEnabled / Orientation / LabelPlacement
  IsShowItemProgress / ProgressValue
      ↓
PrepareContainerForItemOverride
      ↓
StepsItem internal properties
      ↓
StepsItemTheme + StepsItemIndicatorTheme
```

当前步骤状态流：

```text
CurrentStep changed
      ↓
SyncCurrentStepToSelectedItem
      ↓
SelectedIndex changed
      ↓
SyncSelectedIndexToCurrentStep for user selection
      ↓
ConfigureCurrentStepsItem
      ↓
Position / IsFirst / IsLast / IsFinished / Status
```

内容状态流：

```text
SelectedIndex / SelectedItem changed
      ↓
UpdateCurrentContent
      ↓
subscribe selected container Content + ContentTemplate
      ↓
CurrentContent + CurrentContentTemplate
```

`CurrentContentTemplate` 使用当前容器 `ContentTemplate`，为空时回退根控件 `ContentTemplate`。当模板将要变化时，先清空 `CurrentContentTemplate`，再更新内容，避免旧模板生成的控件在 DataContext 变化时被复用。

`CurrentStep` 注册为默认 `TwoWay` 受控状态。外部设置 `CurrentStep` 时仍由 `SyncCurrentStepToSelectedItem` 推动底层选择；用户点击可选 item 或代码设置 `SelectedIndex` 时，`SelectedIndexProperty` class handler 必须把新索引回写到 `CurrentStep`，让绑定源、当前内容和 item 状态保持同一事实来源。

## 5. 生命周期与模板接入

构造阶段：

- 注册 `StepsToken.ScopeProvider`。
- 设置 `SelectionMode=Single`。

静态初始化：

- `SizeType` 影响根控件测量。
- `AutoScrollToSelectedItem` 默认关闭。
- `Orientation` 默认值覆盖为 `Horizontal`。
- `SelectedItem` 变化触发 `UpdateCurrentContent`。

模板接入：

1. 从 NameScope 获取 `PART_ItemsPresenter`。
2. 触发 presenter 模板应用，获取其 `Grid` items panel。
3. 调用 `ConfigureItemsPanel()` 重建 Grid 行列定义。
4. 同步 `:horizontal` / `:vertical` 伪类。
5. 如果 `InitialStep != -1`，写入 `CurrentStep`。
6. 调用 `SyncCurrentStepToSelectedItem()`。
7. 调用 `ConfigureCurrentStepsItem()`。

容器准备：

- 非 visual item 会被写入 `StepsItem.Content`。
- 根 `ItemTemplate` 会绑定到 `StepsItem.ContentTemplate`。
- 根属性通过 Avalonia 绑定下发到 item。
- `ContainerForItemPreparedOverride`、`ContainerIndexChangedOverride` 和 `ClearContainerForItemOverride` 都会重新配置 items layout。

`StepsItem` 模板接入：

- 获取 `PART_Indicator`。
- 同步 `:finished`。
- 计算 `IsEffectiveShowProgress`。

`StepsItemIndicator` 生命周期：

- 尺寸变化时把圆角设置为当前宽度，保证圆形指示器。
- `Icon` 变化时同步 `IsCustom`。
- 初始化时禁用 transition，加载后通过 dispatcher 重新启用，避免初始布局阶段播放过渡。

## 6. 交互与事件处理

pointer 选择：

- 鼠标左键按下时，如果 `IsItemClickable=true`，通过事件源查找 item 容器并调用 `UpdateSelectionFromEvent`。
- 非鼠标 pointer 在释放时处理，且要求释放点仍命中原容器。
- 禁用 `IsItemClickable` 时，pointer 不改变选择。

hover 转发：

- `StepsItem.OnPointerEntered` 在 item 未选中时把 `_indicator.IsItemHover` 设置为 `true`。
- `StepsItem.OnPointerExited` 把 hover 状态清回 `false`。
- item 变为 selected 时，立即清理 indicator hover 状态，避免选中视觉和 hover 视觉叠加。

Steps 不处理 keyboard、focus、drag/drop、popup、context menu 或 command 事件。

## 7. 内部算法与关键流程

### 7.1 Grid items panel 配置

`ConfigureItemsPanel` 使用 `ItemCount` 重建 Grid 定义：

- 水平方向：每个 item 一列。非最后列使用 star；最后列在 `Default` 和 `Inline` 下使用 auto，在 `Navigation` 下使用 star。
- 垂直方向：每个 item 一行。`Navigation` 垂直模式使用 `SharedSizeGroup=NavStepsGridSizeGroup`。

`ConfigureCurrentStepsItem` 再按 item index 设置 Grid 行列。动态增加、删除或重排 item 时，这两个步骤必须一起执行，避免 Grid definitions 与实际 item 数量不一致。

### 7.2 Status 派生

`ConfigureCurrentStepsItem` 对每个容器执行：

```text
set Position = index + 1
set IsFirst / IsLast
if SelectedIndex != -1:
    index < SelectedIndex  → IsFinished=true, Status=Finish at Template priority
    index == SelectedIndex → Status=CurrentStepStatus at Template priority
    index > SelectedIndex  → IsFinished=false, Status=Wait at Template priority
else if CurrentStep >= ItemCount:
    IsFinished=true, Status=Finish at Template priority
```

`Status` 使用模板优先级写入，因此用户在 `StepsItem` 上设置的本地 `Status` 可以覆盖派生状态。维护这段逻辑时，不能把状态写入改成更高优先级，否则会破坏 item 显式状态契约。

### 7.3 Progress ring 绘制

`StepsItem.ConfigureEffectiveShowProgress` 控制进度环入口：

```text
IsShowProgress
&& Style != Inline
&& Icon == null
&& IndicatorType != Dot
```

`StepsItemIndicator.Render` 在 `IsShowProgress && IsCurrent` 时绘制两个椭圆弧：

- groove 使用 `ProgressGrooveColor` 和 `ProgressLineThickness`。
- progress 使用 `ProgressColor`，从 `-90` 度开始，角度为 `360 * ProgressValue / 100`。

绘制矩形来自 `DesiredSize.Deflate(Margin).Deflate(ProgressLineThickness / 2)`。修改指示器 padding、margin 或 progress frame 时，必须验证进度环不被裁剪。

### 7.4 Navigation 箭头布局

水平 `Navigation` 模板中，`NavArrow` 不直接作为 `RootLayout` 的右侧 child，而是放入 `NavArrowLayout`。该 layout 有两个职责：

- 提供足够的绘制空间，避免箭头 icon 被裁剪。
- 与当前指示器首行中心对齐，避免受 description 高度影响。

默认指示器分支用 `IconSize` / `IconSizeSM` 控制 `NavArrowLayout` 高度。Dot 分支用 `IconFontSize` 控制首行槽位高度，同时 `IndicatorLayout` 也使用 `IconFontSize`，让 dot 和箭头在同一个不裁剪的槽位中居中。

这一路径不使用 `#PART_Indicator.Bounds.Height` 绑定，因为模板中前向 element-name 绑定会触发 AXAML/IDE 解析问题。固定模板关系应优先由 token 和 selector 表达。

### 7.5 CurrentContent 订阅

`UpdateCurrentContent` 在每次选择变化时释放旧 `_currentItemSubscriptions`，再订阅当前容器的 `Content` 和 `ContentTemplate` observable。这样可以让 `CurrentContent` 跟随当前 item 之后的内容变化。

模板替换和选择清空不能留下旧容器订阅，否则旧 item 会继续影响根控件的 current content。

## 8. 资源、性能与 AOT 边界

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

## 9. 维护不变量

内部重构必须保持以下不变量：

- `CurrentStep` 写入必须同步 `SelectedIndex`。
- `SelectedIndex` 由用户选择路径改变时必须回写 `CurrentStep`，且不能形成递归状态竞争。
- `SelectedItem` 变化必须更新 `CurrentContent`。
- 动态 item 增删、容器准备、index 变化和容器清理必须触发布局同步。
- `Status` 派生必须使用不覆盖本地值的优先级。
- `IsEffectiveShowProgress` 必须排除 `Inline`、custom icon 和 dot indicator。
- `PART_ItemsPresenter` 使用 Grid items panel，行列定义必须与 `ItemCount` 一致。
- `PART_Indicator` 是 item 与 indicator 交互状态转发的稳定 template part。
- Navigation 箭头宿主不得小于箭头自身尺寸。
- Dot Navigation 的 dot 与箭头必须在同一个首行槽位内居中。
- 不能通过关闭 motion 来规避状态同步、点击或布局问题。

## 10. 测试与验证

验证范围：

- `CurrentStep`、`InitialStep`、`SelectedIndex` 和 item status 计算。
- 动态 `Items.Clear()` / `Items.Add()` 后 Grid definitions 和 item Grid.Column / Grid.Row。
- 水平、垂直、Default、Navigation、Inline、Dot 和 Small 分支。
- `IsItemClickable=true/false` 下 pointer 选择。
- `CurrentContent` 和 `CurrentContentTemplate` 跟随选择变化。
- 进度环在 Default 指示器下显示，在 Dot、Inline 和 custom icon 下不显示。
- Navigation 箭头与指示器中心对齐，并且箭头宿主不裁剪 icon。
- `StepsItemTheme.axaml` 不重新引入 `#PART_Indicator.Bounds.Height` 前向绑定。
- 文档改动运行 `git diff --check`，并检查相对链接存在。
