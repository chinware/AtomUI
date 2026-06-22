# Expander 桌面版实现原理

本文档描述 Expander 桌面版的模板接入、展开状态流、触发区域、展开方向、动效取消与归一、自定义 padding 度量和维护边界。公共设计与 API 契约见 [Expander 桌面版架构设计](overview.md)，Token 语义见 [Expander Token 设计](token.md)，变化记录见 [Expander Changelog](changelog.md)。

## 1. 实现定位

Expander 的实现重点是在 Avalonia `Expander` 基础上稳定扩展 AtomUI 的视觉、触发区域和 motion 行为。实现文档覆盖 `Expander.cs`、`ExpanderPseudoClass.cs`、`ExpanderToken.cs` 和 `ExpanderTheme.axaml` 的职责边界。

本文档不重新解释 Avalonia `Expander` 的基础 Header/Content 机制，也不重复 MotionScene、TokenResource 或 IconButton 的通用实现。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/Expander/Expander.cs`：公共 API、内部 effective state、template part 接入、触发区域、默认图标、边框状态、动效状态机和自定义 padding 度量。
- `src/AtomUI.Desktop.Controls/Expander/ExpanderPseudoClass.cs`：方向、自定义 padding 和展开状态相关伪类常量。
- `src/AtomUI.Desktop.Controls/Expander/ExpanderToken.cs`：Expander 组件 Token。
- `src/AtomUI.Desktop.Controls/Expander/Themes/ExpanderTheme.axaml`：控件模板、SizeType 分支、方向分支、图标位置分支、触发分支、Borderless/Ghost 分支和 token 引用。
- `tests/AtomUI.Desktop.Controls.Tests/Expander/ExpanderBehaviorTests.cs`：Expander 行为和布局回归测试。

## 3. 核心类职责

`Expander` 继承 `Avalonia.Controls.Expander`。它负责把 AtomUI 扩展属性转换为模板可消费的 internal state，并协调 Header 点击、图标点击、边框状态和 Content motion。

`ExpanderPseudoClass` 是主题状态常量集合。它定义方向伪类和自定义 padding 伪类，供主题 selector 使用。

`ExpanderToken` 提供组件级默认间距、背景和圆角。它不保存实例展开状态、Header 内容、Content 内容或 motion 运行状态。

`ExpanderTheme.axaml` 是模板和视觉状态的唯一来源。稳定 template part 不应在 C# 中动态创建，也不应通过 C# 绑定替代可由 AXAML 表达的关系。

## 4. 状态与数据流

展开状态流：

```text
OnPointerPressed / PART_ExpandButton.Click
  → ToggleExpanded()
  → IsExpanded changed
  → UpdateContentVisibility(IsExpanded)
  → motion or stable state
  → PART_ContentMotionActor.IsVisible
```

边框状态流：

```text
BorderThickness / IsGhostStyle / IsBorderless / ExpandDirection
  → SetupEffectiveBorderThickness()
  → SetupExpanderBorderThickness()
  → PART_Frame.BorderThickness
  → PART_HeaderDecorator.BorderThickness
```

自定义 padding 状态流：

```text
HeaderPadding changed
  → UpdateEffectiveExpandButtonMargin()
  → :custom-header-padding
  → HeaderDecorator.Padding + ExpandButton.Margin + HeaderPresenter.Margin

ContentPadding changed
  → :custom-content-padding
  → ContentPresenter.Padding
```

默认图标状态流：

```text
ExpandIcon == null
  → Clear local/current template value
  → Set RightOutlined at Template priority
```

## 5. 生命周期与模板接入

构造阶段：

- 注册 `ExpanderToken.ScopeProvider`。

模板接入：

1. 解除旧 `_expandButton.Click` 事件订阅。
2. 取消旧 content motion 并清理旧 motion actor 临时值。
3. 从 NameScope 获取 `PART_ContentMotionActor`、`PART_HeaderDecorator` 和 `PART_ExpandButton`。
4. 把新 motion actor 归一到当前 `IsExpanded` 的稳定状态。
5. 订阅新 `_expandButton.Click`。
6. 同步有效边框、Header 边框、图标 margin、默认图标和自定义 padding 伪类。

脱离视觉树：

- 取消正在运行的 content motion。
- 清理 motion actor 的临时布局和 transform 值。

模板重套用和 detach 不能让旧 part 继续被事件、动画任务或 cancellation 持有。

## 6. 交互与事件处理

Header 点击路径：

- `OnPointerPressed` 只处理鼠标左键。
- `CanToggleFromHeaderPointer` 检查 `TriggerType=Header`、鼠标状态和 `PART_HeaderDecorator` 命中。
- 命中后调用 `ToggleExpanded()` 并标记事件 handled。

图标点击路径：

- `PART_ExpandButton.Click` 直接调用 `ToggleExpanded()`。
- `TriggerType=Icon` 下 Header 点击不会触发展开，图标仍可触发。

禁用状态：

- 模板把 `IsEnabled` 传给 `PART_ExpandButton`。
- 主题通过 `:disabled` 同步 Header 和 ContentPresenter 前景。

Expander 不处理 keyboard、focus、drag/drop、popup 或 command 事件。

## 7. 内部算法与关键流程

### 7.1 展开方向转换

`DirectionFromExpandDirection` 把 Avalonia `ExpandDirection` 转为 MotionScene `Direction`：

```text
Left  → Direction.Left
Up    → Direction.Top
Right → Direction.Right
Down  → Direction.Bottom
```

该转换只服务 content motion。Header dock、Header layout transform 和图标旋转由 AXAML selector 表达，必须与这个转换保持一致。

### 7.2 Content motion 状态机

`UpdateContentVisibility` 按三种路径处理状态：

- 无 motion actor：直接返回。
- `IsMotionEnabled=false`：调用 `ApplyContentStableState`。
- 已经处于隐藏且没有运行中的取消源：直接归一隐藏状态。
- 其他场景：创建新的 `CancellationTokenSource`，取消旧 motion，启动异步 motion。

motion 完成时：

```text
CompleteContentMotion
  → 非当前 cancellation: dispose and return
  → actor 已替换或已取消: dispose and return
  → IsExpanded == targetVisible: ApplyContentStableState
  → otherwise: UpdateContentVisibility(IsExpanded)
```

这一流程保证连续切换 `IsExpanded` 时最终视觉状态以最新值为准。

### 7.3 Motion 临时值清理

`ClearContentMotionValues` 清理：

- `Transitions`
- `MotionTransform`
- `MotionTransformOperations`
- `Height`

这些属性由 motion 过程写入。模板重套用、detach、取消 motion 和进入稳定状态时都必须清理，避免旧动画影响下一次布局。

### 7.4 Header 边框计算

`SetupExpanderBorderThickness` 先取 `BorderThickness.Bottom` 作为 Header 与 Content 分割线厚度。`IsGhostStyle` 或 `IsBorderless` 下分割线厚度归零。

方向映射：

- `Down` / `Left`：分割线在 Header 下边。
- `Up` / `Right`：分割线在 Header 上边。

### 7.5 自定义 HeaderPadding 下的图标间距

`UpdateEffectiveExpandButtonMargin` 只在 `HeaderPadding` 非空时计算有效图标 margin：

```text
ExpandIconPosition=Start → Thickness(0, 0, HeaderPadding.Left, 0)
ExpandIconPosition=End   → Thickness(HeaderPadding.Right, 0, 0, 0)
```

默认 HeaderPadding 为空时，`EffectiveExpandButtonMargin` 为默认值，主题继续使用 `LeftExpandButtonHMargin` / `RightExpandButtonHMargin` token。这样默认 SizeType 分支保持 token 驱动，自定义 padding 分支保持与用户显式 spacing 对齐。

## 8. 资源、性能与 AOT 边界

资源边界：

- `ExpanderToken` 通过 token generator 注册，主题通过 `ExpanderTokenResource` 消费。
- SharedToken 提供图标大小、动效开关、动效时长、边框厚度和基础颜色。
- 默认图标使用显式 `new RightOutlined()`，不做运行时图标扫描。

生命周期边界：

- `_expandButton.Click` 在模板重套用时先解除旧订阅，再订阅新 part。
- `_contentMotionCancellation` 在新 motion、稳定状态、模板重套用和 detach 时取消并释放。
- motion actor 临时值必须在状态归一时清理。

AOT 边界：

- 不新增字符串 path binding、运行时反射扫描或动态类型创建。
- 模板内固定关系优先使用 AXAML `TemplateBinding`、selector 和 TokenResource。
- C# 只处理 AXAML 无法表达的运行时状态流，例如 motion cancellation 和 HeaderPadding 派生 margin。

性能边界：

- Header/Content 默认视觉由静态 AXAML 提供，不在运行时动态构造模板视觉。
- 展开/收起只操作单个 `LayoutAwareMotionActor`，不遍历复杂子树。
- 自定义 padding 的 margin 计算是常量时间，不进入渲染热路径。

## 9. 维护不变量

内部重构必须保持以下不变量：

- template reapply 时旧 `_expandButton.Click` 必须解除。
- detach 时必须取消 motion 并清理临时值。
- `TriggerType=Icon` 不能通过 Header 点击切换状态。
- 默认 `ExpandIcon` 为空时必须使用 `RightOutlined`，且不覆盖用户显式图标。
- `IsMotionEnabled=false` 不能留下 Height 或 transform 临时值。
- `CompleteContentMotion` 必须校验当前 cancellation 和当前 motion actor。
- `ExpandDirection` 的 motion 方向、Header dock、Header transform 和图标旋转必须同步维护。
- 自定义 HeaderPadding 下的图标间距必须跟随 HeaderPadding 对应方向，不回退到默认 SizeType token。
- `:custom-header-padding` 和 `:custom-content-padding` 的伪类语义不能混用。
- Expander 不引入多面板或手风琴状态；这属于 Collapse 的职责。

## 10. 测试与验证

验证范围：

- `ExpanderBehaviorTests.Custom_Header_Padding_Uses_Compact_Expand_Icon_Layout`：自定义 HeaderPadding 下展开图标间距和垂直居中。
- `ExpanderBehaviorTests.Ghost_And_Borderless_Mode_Update_Frame_Border_At_Runtime`：运行时边框模式切换。
- `ExpanderBehaviorTests.Icon_Trigger_Does_Not_Toggle_From_Header_Click`：Icon 触发模式下 Header 点击不切换。
- `ExpanderBehaviorTests.Content_Motion_Reconciles_Final_Visibility_When_Expanded_Changes_During_Animation`：动画中状态变化后的最终可见性。
- Gallery Expander 示例：Basic、Size、Borderless、Ghost、Custom Padding、Direction、Nested、No Arrow、Icon Position、Trigger。
- 修改主题或动效时运行 Expander 相关测试；影响 shared motion 或 theme 时扩大到完整 `AtomUI.Desktop.Controls.Tests`。
- 文档改动运行 `git diff --check`，并检查相对链接存在。
