# Tour 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

`Tour` 公开 13 个 Semantic Part：`root` 由生成器隐式加入，不要求 `.semantic-root`；其余 12 个为 `popup.*`
弹层部件。除 `root` 外每个 Part 生成 public 强类型 Semantic Style，命名规则为
`AtomUI.Theme.Styling.Tour<PartPathPascalCase>Style`（如 `TourPopupMaskStyle`），用户在外层普通 `Style`
中按 owner 作用域嵌套使用。

部件名单对齐上游 Tour 的语义部件结构（`_semantic.tsx`）：`root`、`mask`、`section`、`cover`、`close`（上游 6.4.0
起）、`header`、`title`、`description`、`footer`、`actions`、`indicators`、`indicator`。上游 Tour 面板整体渲染在
`getPopupContainer=false` 的内联容器中，部件名不带弹层前缀；AtomUI 的引导卡片承载在 `Popup` 的独立视觉根中，
因此除 `root`（owner 本体）外统一以 `popup.` 前缀命名：上游 `mask` 对应 `popup.mask`，上游 `section` 对应
`popup.section`，其余同理。上游 `panel` 包裹节点与箭头不是语义部件，AtomUI 同样不发布（箭头由共享
`ArrowDecoratedBox` 的 `semantic-arrow` marker 承担，但不进入 Tour 契约）。

全部 `popup.*` 部件声明 `CrossVisualRoot=true`：卡片内容在 `Popup` overlay 宿主中（与 owner 同 TopLevel，
样式经逻辑树级联命中）；遮罩在 TopLevel `VisualLayerManager` 的共享 `TourLayer` 中，经逻辑父挂载归属当前打开的
Tour。所有 marker 使用静态 `Classes.semantic-*="True"` 声明（`popup.mask` 与 `popup.indicator` 为运行时代码注入
marker 类，无 TemplatedParent），内置主题不使用 `.semantic-*` selector 实现默认视觉。

### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `root` |
| Selector | 不适用（root 无 `.semantic-root`） |
| SelectorRoute | 不适用 |
| ContractType | `Tour` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `Tour` 控件根（隐式；服务型控件，静止态无视觉） |
| 职责 | 引导流程 owner：承载步骤集合、受控开关状态与目标锚定 |
| 相关 API | `IsOpen`、`CurrentIndex`、`Steps`、`StepsSource`、`ShowTour()` |
| 相关 Token | 无独立 Token（流程状态不落 Token） |
| 稳定性 | stable since 6.0 |

### `popup.root`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `popup.root` |
| Selector | `.semantic-popup-root` |
| SelectorRoute | `/template/ .semantic-popup-root` |
| ContractType | `ArrowDecoratedBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `TourTheme.axaml` 模板内 `PART_ArrowDecorator`（静态 marker） |
| 职责 | 引导卡片容器根：承载卡片内容与方向箭头（对齐上游 `.ant-tour` 面板根的容器职责） |
| 相关 API | `Placement`、`IsArrowVisible`、`StyleType` |
| 相关 Token | `TourBorderRadius`、`ColorBgElevated` |
| 稳定性 | stable since 6.0 |

### `popup.mask`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `popup.mask` |
| Selector | `.semantic-popup-mask` |
| SelectorRoute | `>> .semantic-popup-mask` |
| ContractType | `Control` |
| Cardinality | `Optional` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | TopLevel `VisualLayerManager` 中的共享 `TourLayer`（internal；marker 类由 `Tour` 代码经生成常量 `TourSemanticParts.PopupMaskClass` 注入） |
| 职责 | 遮罩层：覆盖目标区域以外的整屏、镂空高亮当前步骤目标并阻挡交互（对齐上游 mask 的全屏覆盖与指针事件语义） |
| 相关 API | `IsShowMask`、`MaskColor`、`GapRadius`、`GapOffsetX/Y` |
| 相关 Token | 遮罩色默认 `ColorBgMask`（经 `MaskColor` 中继） |
| 稳定性 | stable since 6.0 |

遮罩是跨根部件中的特殊形态：物理节点是 VLM 级共享单例（`TourLayer.GetTourLayer` 创建并 `AddLayer`），归属规则
"谁打开谁拥有，关闭即释放"。`Tour.ShowTour()` 挂载时执行三步：`AddLayer`（VLM 内部把层逻辑挂到自身）→
`SetParent(null)`（解除 VLM 归属）→ `SetParent(tourOwner)`（挂到当前 Tour）；`HideTour()` 与生命周期关闭在归属
自己时执行 `SetParent(null)` 释放，下一个打开的 Tour 可重新挂载。owner 作用域 Semantic Style 经 `>>`（沿逻辑树
匹配的 Descendant 选择器）跨视觉根命中遮罩；`Tour` 实现 `ISemanticPartCrossRootProvider`，在遮罩可见时经
`GetCrossRoots()` 上报该层，挂载/释放触发 `CrossRootsChanged` 供语义高亮会话跟随刷新。

`ContractType` 为 `Control`：`TourLayer` 是 internal 类型，不进入公共契约。遮罩颜色定制走 `MaskColor` API
（含镂空几何的绘制由 `TourLayer.Render` 承担，样式通道只承诺 `Visual` 层属性如 `Opacity`、`IsHitTestVisible`）。

### `popup.section`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `popup.section` |
| Selector | `.semantic-container`（共享 ArrowDecoratedBox marker） |
| SelectorRoute | `/template/ .semantic-popup-root /template/ .semantic-container` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 共享 `ArrowDecoratedBoxTheme.axaml` 模板内 `PART_ContentDecorator` Border |
| 职责 | 卡片主要内容区域：圆角、背景、边框与内边距（对齐上游 `.ant-tour` 内 `section` 的卡片样式职责） |
| 相关 API | `StyleType`（Primary 经 popup.root 背景表达） |
| 相关 Token | `TourBorderRadius`、`ColorBgElevated` |
| 稳定性 | stable since 6.0 |

marker 声明在共享 `ArrowDecoratedBoxTheme`（与 ToolTip、DatePicker 等共享容器复用同一 `semantic-container`
marker），路由经 owner 模板的 `.semantic-popup-root` 锚点加第二段 `/template/` 进入嵌套模板；生成器按
`CrossNestedOwners` 跨主题资产校验 marker 存在性。

### `popup.cover`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `popup.cover` |
| Selector | `.semantic-popup-cover` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-cover` |
| ContractType | `ContentPresenter` |
| Cardinality | `Optional` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TourStepTheme.axaml` 模板内 `CoverPresenter`（marker 静态声明，节点由 ItemsControl 按步骤物化） |
| 职责 | 卡片封面区域：承载步骤封面图片等内容（对齐上游 `cover`） |
| 相关 API | `TourStep.Cover`、`CoverTemplate` |
| 相关 Token | 无独立 Token |
| 稳定性 | stable since 6.0 |

### `popup.close`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `popup.close` |
| Selector | `.semantic-popup-close` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-close` |
| ContractType | `Button`（Avalonia `Button` 契约，实际节点为 `DialogCaptionButton`） |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TourStepTheme.axaml` 模板内 `CloseButton`（`DialogCaptionButton`，marker 静态声明） |
| 职责 | 关闭按钮：结束引导流程（对齐上游 `close`，上游自 6.4.0 发布） |
| 相关 API | `CloseIcon` |
| 相关 Token | `CloseBtnSize`、`IconSize` |
| 稳定性 | stable since 6.0 |

### `popup.header`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `popup.header` |
| Selector | `.semantic-popup-header` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-header` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TourStepTheme.axaml` 模板内 header 包裹 Border（组合标题与关闭按钮） |
| 职责 | 卡片头部区域：组合标题与关闭按钮的头部容器（对齐上游 `header`） |
| 相关 API | 无独立 API（内容经 `Title`/`CloseIcon` 进入） |
| 相关 Token | 无独立 Token |
| 稳定性 | stable since 6.0 |

### `popup.title`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `popup.title` |
| Selector | `.semantic-popup-title` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-title` |
| ContractType | `ContentPresenter` |
| Cardinality | `Optional` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TourStepTheme.axaml` 模板内 `Title` ContentPresenter |
| 职责 | 引导步骤标题文字（对齐上游 `title`） |
| 相关 API | `TourStep.Title`、`TitleTemplate` |
| 相关 Token | `HeaderColor`、`FontWeightStrong` |
| 稳定性 | stable since 6.0 |

### `popup.description`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `popup.description` |
| Selector | `.semantic-popup-description` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-description` |
| ContractType | `ContentPresenter` |
| Cardinality | `Optional` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TourStepTheme.axaml` 模板内 `DescriptionPresenter` |
| 职责 | 引导步骤描述文字（对齐上游 `description`） |
| 相关 API | `TourStep.Description`、`DescriptionTemplate` |
| 相关 Token | 无独立 Token（沿用文本 Token） |
| 稳定性 | stable since 6.0 |

### `popup.footer`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `popup.footer` |
| Selector | `.semantic-popup-footer` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-footer` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TourStepsViewTheme.axaml` 模板内 `FooterFrame` Border |
| 职责 | 卡片底部操作区域：组合指示器与操作按钮组（对齐上游 `footer`） |
| 相关 API | 无独立 API（内容经 `Indicator`/`CustomActions` 进入） |
| 相关 Token | 无独立 Token |
| 稳定性 | stable since 6.0 |

### `popup.actions`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `popup.actions` |
| Selector | `.semantic-popup-actions` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-actions` |
| ContractType | `StackPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TourStepsViewTheme.axaml` 模板内 `ActionsLayout` StackPanel |
| 职责 | 操作按钮组容器：承载上一步/下一步/完成按钮（对齐上游 `actions`） |
| 相关 API | `CustomActions`、步骤导航事件 |
| 相关 Token | `PrimaryPrevBtnBg`、`PrimaryNextBtnHoverBg` |
| 稳定性 | stable since 6.0 |

### `popup.indicators`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `popup.indicators` |
| Selector | `.semantic-popup-indicators` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-indicators` |
| ContractType | `ContentPresenter` |
| Cardinality | `Optional` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TourStepsViewTheme.axaml` 模板内 `IndicatorPresenter` ContentPresenter |
| 职责 | 指示器组容器：承载当前 `Indicator` 实例（对齐上游 `indicators`） |
| 相关 API | `Indicator` |
| 相关 Token | 无独立 Token |
| 稳定性 | stable since 6.0 |

### `popup.indicator`

| 字段 | 值 |
| --- | --- |
| Owner | `Tour` |
| Part | `popup.indicator` |
| Selector | `.semantic-popup-indicator` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-indicator` |
| ContractType | `Ellipse` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `DefaultTourIndicator` 代码按 `StepCount` 物化的 `Ellipse` 圆点（marker 类与 `active` 状态类由代码注入） |
| 职责 | 单个步骤指示器圆点，含激活态（对齐上游 `indicator`） |
| 相关 API | `IndicatorSize`、`IndicatorColor`、`IndicatorActiveColor`、`ItemSpacing` |
| 相关 Token | `IndicatorSize`、`ColorFill`、`ColorPrimary` |
| 稳定性 | stable since 6.0 |

圆点由 `DefaultTourIndicator` 代码物化并只挂 `semantic-popup-indicator` marker 类与 `active` 激活态类；
视觉（尺寸/颜色/间距）全部由 `DefaultTourIndicatorTheme` 表达——圆点无 TemplatedParent，`/template/` 选择器链
无法命中，主题样式声明在模板内 `DotsLayout` 元素作用域（`StackPanel.Styles`）沿逻辑树流到圆点，尺寸/颜色经
`RelativeSource AncestorType` 绑定到指示器实例。激活态经 `.active` 类选择器表达。布局契约保持旧自绘版公式
`N*size + (N+1)*spacing`（`MeasureOverride` 覆写承担，左右各留一份间距）。

`TextTourIndicator` 不物化圆点，使用该指示器时 `popup.indicator` 实例数为 0（`Multiple` 允许 0..N）。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Tour/Themes/TourTheme.axaml`

```xml
<Popup Name="PART_Popup">
    <ArrowDecoratedBox Name="{x:Static atom:AbstractArrowDecoratedBox.ArrowDecoratorPart}">
        <TourStepsView Name="StepsView" />
    </ArrowDecoratedBox>
</Popup>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Tour
  -> DefaultTourIndicator (control theme, DefaultTourIndicatorTheme.axaml)
     -> StackPanel#DotsLayout (template-stable)
  -> TextTourIndicator (control theme, TextTourIndicatorTheme.axaml)
     -> TextBlock (template-stable)
  -> TourStep (control theme, TourStepTheme.axaml)
     -> Border#Frame (template-stable)
        -> DockPanel#RootLayout (template-stable)
           -> Border (template-stable)
              -> DockPanel (template-stable)
                 -> DialogCaptionButton#CloseButton (template-stable)
                 -> ContentPresenter#Title (internal-observable)
           -> StackPanel#ContentLayout (template-stable)
              -> ContentPresenter#CoverPresenter (internal-observable)
              -> ContentPresenter#DescriptionPresenter (internal-observable)
  -> TourStepsView (control theme, TourStepsViewTheme.axaml)
     -> Border#Frame (template-stable)
        -> DockPanel#RootLayout (template-stable)
           -> Border#FooterFrame (template-stable)
              -> DockPanel (template-stable)
                 -> ContentPresenter#IndicatorPresenter (internal-observable)
                 -> Panel (template-stable)
                    -> StackPanel#ActionsLayout (template-stable)
                       -> Button#PreviousButton (template-stable)
                       -> Button#NextButton (template-stable)
                       -> Button#FinishButton (template-stable)
           -> ItemsPresenter (internal-observable)
  -> Tour (control theme, TourTheme.axaml)
     -> Popup#PART_Popup (template-stable)
        -> ArrowDecoratedBox#{x:Static atom:AbstractArrowDecoratedBox.ArrowDecoratorPart} (template-stable)
           -> TourStepsView#StepsView (internal-observable)
  -> TourLayer (control theme, TourTheme.axaml)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Tour` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `DefaultTourIndicator` | control theme | `DefaultTourIndicatorTheme.axaml` | Tour | `ItemSpacing` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `DotsLayout` | template node (StackPanel) | `DefaultTourIndicatorTheme.axaml` | DefaultTourIndicator | `ItemSpacing` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TextTourIndicator` | control theme | `TextTourIndicatorTheme.axaml` | Tour | `IndicatorText` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TourStep` | control theme | `TourStepTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `CloseIcon`, `Cover`, `CoverTemplate`, `Description`, `DescriptionTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `TourStepTheme.axaml` | TourStep | `Background`, `CloseIcon`, `Cover`, `CoverTemplate`, `Description`, `DescriptionTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RootLayout` | template node (DockPanel) | `TourStepTheme.axaml` | TourStep | `CloseIcon`, `Cover`, `CoverTemplate`, `Description`, `DescriptionTemplate`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `TourStepTheme.axaml` | TourStep | `CloseIcon`, `IsMotionEnabled`, `Title`, `TitleTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CloseButton` | template node (DialogCaptionButton) | `TourStepTheme.axaml` | TourStep | `CloseIcon`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Title` | template node (ContentPresenter) | `TourStepTheme.axaml` | TourStep | `Title`, `TitleTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentLayout` | template node (StackPanel) | `TourStepTheme.axaml` | TourStep | `Cover`, `CoverTemplate`, `Description`, `DescriptionTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CoverPresenter` | template node (ContentPresenter) | `TourStepTheme.axaml` | TourStep | `Cover`, `CoverTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `DescriptionPresenter` | template node (ContentPresenter) | `TourStepTheme.axaml` | TourStep | `Description`, `DescriptionTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TourStepsView` | control theme | `TourStepsViewTheme.axaml` | Tour | `Background`, `Indicator`, `ItemsPanel`, `Padding` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (Border) | `TourStepsViewTheme.axaml` | TourStepsView | `Background`, `Indicator`, `ItemsPanel`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RootLayout` | template node (DockPanel) | `TourStepsViewTheme.axaml` | TourStepsView | `Indicator`, `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FooterFrame` | template node (Border) | `TourStepsViewTheme.axaml` | TourStepsView | `Indicator` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `TourStepsViewTheme.axaml` | TourStepsView | `Indicator` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IndicatorPresenter` | template node (ContentPresenter) | `TourStepsViewTheme.axaml` | TourStepsView | `Indicator` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `TourStepsViewTheme.axaml` | TourStepsView | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ActionsLayout` | template node (StackPanel) | `TourStepsViewTheme.axaml` | TourStepsView | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PreviousButton` | template node (Button) | `TourStepsViewTheme.axaml` | TourStepsView | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `NextButton` | template node (Button) | `TourStepsViewTheme.axaml` | TourStepsView | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FinishButton` | template node (Button) | `TourStepsViewTheme.axaml` | TourStepsView | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsPresenter` | template node (ItemsPresenter) | `TourStepsViewTheme.axaml` | TourStepsView | `ItemsPanel` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Tour` | control theme | `TourTheme.axaml` | 用户代码 / 控件宿主 | `ArrowPosition`, `CloseIcon`, `CurrentArrowVisible`, `CurrentIndex`, `CurrentStyleType`, `Indicator` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_Popup` | template node (Popup) | `TourTheme.axaml` | Tour | `ArrowPosition`, `CloseIcon`, `CurrentArrowVisible`, `CurrentIndex`, `CurrentStyleType`, `Indicator` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AbstractArrowDecoratedBox.ArrowDecoratorPart}` | template node (ArrowDecoratedBox) | `TourTheme.axaml` | Tour | `ArrowPosition`, `CloseIcon`, `CurrentArrowVisible`, `CurrentIndex`, `CurrentStyleType`, `Indicator` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StepsView` | template node (TourStepsView) | `TourTheme.axaml` | Tour | `CloseIcon`, `CurrentIndex`, `CurrentStyleType`, `Indicator`, `IsArrowVisible`, `IsMotionEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TourLayer` | control theme | `TourTheme.axaml` | Tour | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CloseIcon`、`CoverTemplate`、`Description`、`DescriptionTemplate`、`ItemSpacing`、`ItemTemplate`、`Title`、`TitleTemplate` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `ActiveIndex`、`CurrentIndex`、`IndicatorActiveColor`、`StepCount` | 维护选择、展开、过滤、分页、分组或集合状态；`CurrentIndex` 默认双向绑定。 |
| 交互与状态 | `IsArrowVisible`、`IsDisabledInteraction`、`IsMotionEnabled`、`IsOpen`、`IsPointAtCenter`、`IsPopupPinnedOpen`、`IsScrollIntoView`、`IsShowMask` | 表达用户可观察状态、可用性、清除、加载或反馈语义；`IsOpen` 默认双向绑定；`IsPopupPinnedOpen` 钉住弹层常开（语义预览场景）。 |
| 视觉与布局 | `Background`、`GapOffsetX`、`GapOffsetY`、`GapRadius`、`IndicatorColor`、`IndicatorSize`、`MaskColor`、`Placement`、`StyleType`、`TargetRegionCornerRadius` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Cover`、`Indicator`、`Target`、`TargetRegion` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、open/close、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Tour Token + ControlTheme。 |

## State Flow

Tour 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、open/close、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `IsOpen` 与 `CurrentIndex` 是默认 `TwoWay` 的受控状态；popup、indicator 和步骤视图只能消费或回写 public 状态，不能形成局部当前步骤。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

Tour 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `DefaultTourIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TextTourIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TourStepTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TourStepsViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TourTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Tour 使用 `TourToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、open/close、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Tour Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TourToken`，scope id 为 `Tour`，源码位于 `src/AtomUI.Desktop.Controls/Tour/TourToken.cs`。

## Customization Boundaries

维护 Tour 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Tour 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
