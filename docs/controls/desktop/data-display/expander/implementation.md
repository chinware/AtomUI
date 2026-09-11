# Expander 桌面版实现原理

本文档描述 Expander 桌面版的模板接入、展开状态流、触发区域、展开方向、动效取消与归一、自定义 padding 度量和维护边界。公共设计与 API 契约见 [Expander 桌面版架构设计](overview.md)，Token 语义见 [Expander Token 设计](token.md)，变化记录见 [Expander Changelog](changelog.md)。

## 1. 实现定位

Expander 的实现重点是在 Avalonia `Expander` 基础上稳定扩展 AtomUI 的视觉、触发区域和 motion 行为。实现文档覆盖 `Expander.cs`、`ExpanderPseudoClass.cs`、`ExpanderToken.cs` 和 `ExpanderTheme.axaml` 的职责边界。

本文档不重新解释 Avalonia `Expander` 的基础 Header/Content 机制，也不重复 MotionScene、TokenResource 或 IconButton 的通用实现。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/Expander/Expander.cs`：公共 API、内部 effective state、template part 接入、触发区域、默认图标、边框状态、动效状态机和自定义 padding 度量。
- `src/AtomUI.Desktop.Controls/Expander/ExpanderPseudoClass.cs`：方向、自定义 padding 和展开状态相关伪类常量。
- `src/AtomUI.Desktop.Controls/Expander/ExpanderToken.cs`：Expander 控件 Token。
- `src/AtomUI.Desktop.Controls/Expander/Themes/ExpanderTheme.axaml`：控件模板、SizeType 分支、方向分支、图标位置分支、触发分支、Borderless/Ghost 分支和 token 引用。
- `tests/AtomUI.Desktop.Controls.Tests/Expander/ExpanderBehaviorTests.cs`：Expander 行为和布局回归测试。
- `src/AtomUI.Core/MotionScene/ContentExpansionAnimator.cs`：共用内容测量、进度插值与执行资源 owner。
- `tests/AtomUI.Desktop.Controls.Tests/Motion`：共用展开机制的帧级几何和边界回归。

## 3. 核心类职责

`Expander` 继承 `Avalonia.Controls.Expander`。它负责把 AtomUI 扩展属性转换为模板可消费的 internal state，并协调 Header 点击、图标点击、结构化 Content 分隔线和 Content motion。

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
BorderThickness / IsGhostStyle / IsBorderless
  → SetupEffectiveBorderThickness()
  → PART_Frame.BorderThickness

BorderThickness / IsGhostStyle / IsBorderless / ExpandDirection
  → ConfigureContentBorderThickness()
  → PART_ContentMotionActor / unnamed PixelAlignedBorder.BorderThickness
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
6. 同步有效外框、Content 分隔线、图标 margin、默认图标和自定义 padding 伪类。

脱离视觉树：

- 通过当前 `ContentExpansionAnimator.ApplyState` 按 `IsExpanded` 归一 motion actor。
- 归一过程取消执行并解除内部进度与布局接入，同步最终 `Opacity` / `IsVisible`；重新附着时再次投影当前状态。

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

`Expander` 为当前 `PART_ContentMotionActor` 持有 Core 内部 `ContentExpansionAnimator`。状态入口提交
`IsExpanded`、转换后的方向以及有效 `MotionDuration`；动画执行器统一使用 Avalonia 原生 `Animation` 驱动存储在
actor 上的私有展开进度附加属性与 `Opacity`。进度属性及其布局失效通知由动画执行器拥有，actor 仅通过内部
`IMotionActorLayout` 委托测量和排列来控制内容视口，不使用内容缩放表达展开进度。

- 无 motion actor：保留业务状态，不启动内容动画。
- 初始模板接入、未附着、动效关闭或时长不大于零：直接投影稳定状态。
- 新请求：先读取当前呈现的尺寸和透明度，再使旧执行失效并取消，从所捕获的起点开始插值。
- 正常完成：当前执行恢复自然布局或隐藏内容，释放其执行资源并报告 actor 完成。
- 反转、模板替换、方向变化或 detach：旧执行失效，其 continuation 无权覆盖新请求或新 actor。

### 7.3 Motion 资源清理

动画执行器拥有单次执行的取消源、原生动画、内部进度值和 actor 布局接入。稳定状态通过 `ApplyState` 取消执行，
解除内部接入并同步 `Opacity` / `IsVisible`。运行中关闭 `IsMotionEnabled` 立即按最新 `IsExpanded` 收敛；改变
`ExpandDirection` 同样取消旧轴执行，并让模板在新方向建立稳定布局。

清理只释放共用机制拥有的值和资源，不清除用户或模板设置的 `Height`、`Width`、Min/Max 约束、transform 或
transition，也不修改内容自身的布局属性。

### 7.4 Content 结构化分隔线

`ConfigureContentBorderThickness` 取 `BorderThickness.Bottom` 作为 Header 与 Content 分隔线厚度。`IsGhostStyle` 或 `IsBorderless` 下，`SetupEffectiveBorderThickness` 和该方法分别把根边框与分隔线厚度归零，既有背景 selector 保持不变。

分隔线位于 `PART_ContentMotionActor` 内部的 unnamed `PixelAlignedBorder`，Header 不承担边框。方向映射：

- `Down`：Content 顶边。
- `Up`：Content 底边。
- `Left`：Content 右边。
- `Right`：Content 左边。

边框计算不读取 `IsExpanded`。收起时 motion actor 隐藏，分隔线随 Content 自然消失；展开、收起或快速反向 motion 不修改分隔线 Thickness 或 Brush。

### 7.5 自定义 HeaderPadding 下的图标间距

`UpdateEffectiveExpandButtonMargin` 只在 `HeaderPadding` 非空时计算有效图标 margin：

```text
ExpandIconPosition=Start → Thickness(0, 0, HeaderPadding.Left, 0)
ExpandIconPosition=End   → Thickness(HeaderPadding.Right, 0, 0, 0)
```

默认 HeaderPadding 为空时，`EffectiveExpandButtonMargin` 为默认值，主题继续使用 `LeftExpandButtonHMargin` / `RightExpandButtonHMargin` token。这样默认 SizeType 分支保持 token 驱动，自定义 padding 分支保持与用户显式 spacing 对齐。

### 7.6 共用内容展开接入契约

共用机制的设计所有者为 [内容展开与收起动效设计](../../../../architecture/systems/control-infrastructure/content-expansion.md)。
Expander 提供 `IsExpanded`、`ExpandDirection`、有效 `MotionDuration`、内容宿主及其正常展开约束；Core 内部机制
负责尺寸/透明度插值、自然测量、首尾帧及请求生命周期。执行链与释放边界分别见 7.2、7.3。

| 方向 | 动画尺寸 | Header 所在侧 | 内容锚定边 / 分隔线 |
| --- | --- | --- | --- |
| Down | Height | 顶部 | 顶边 |
| Up | Height | 底部 | 底边 |
| Left | Width | 右侧 | 右边 |
| Right | Width | 左侧 | 左边 |

Header dock、Header 旋转和展开图标继续由方向 selector 表达；内部内容视口按相同方向排列和裁剪。
完整内容测量包含未命名 border 及 `PART_ContentPresenter.Padding`，不包含 Header。横向内容采用正常展开布局的
有效排版宽度，不能把每一帧的视口 Width 当作新的 TextWrapping 宽度。

反转先捕获实际尺寸与透明度，再使旧请求失效并取消；原执行及其清理不能改变新请求的布局值。
运行中改变方向先释放旧轴的临时值，再按新方向和当前 `IsExpanded` 建立稳定布局。关动效、模板替换、detach 和
重新接入同样释放内部尺寸接管，保持模板原有尺寸值、绑定和内容边框的结构所有权。

内容、有效宿主约束、字体或嵌套 Expander 尺寸变化时，自然测量需要失效；动画视口自身变化不重新定义自然尺寸。
输入触发、继承的展开状态语义和主题映射由 Expander 持有，共享层不增加 selection 或手风琴规则。

## 8. 资源、性能与 AOT 边界

资源边界：

- `ExpanderToken` 通过 token generator 注册，主题通过 `ExpanderTokenResource` 消费。
- SharedToken 提供图标大小、动效开关、动效时长、边框厚度和基础颜色。
- 默认图标使用显式 `new RightOutlined()`，不做运行时图标扫描。

生命周期边界：

- `_expandButton.Click` 在模板重套用时先解除旧订阅，再订阅新 part。
- `_contentExpansion` 随当前 actor 建立；单次执行的取消源由动画执行器在完成或取消后释放。
- motion actor 临时值必须在状态归一时清理。

AOT 边界：

- 不新增字符串 path binding、运行时反射扫描或动态类型创建。
- 模板内固定关系优先使用 AXAML `TemplateBinding`、selector 和 TokenResource。
- C# 只处理 AXAML 无法表达的运行时状态流，例如 motion cancellation 和 HeaderPadding 派生 margin。

性能边界：

- Header/Content 默认视觉和未命名 Content 分隔线由静态 AXAML 提供，不在运行时动态构造模板视觉。
- 展开/收起只操作单个 `LayoutAwareMotionActor`，不遍历复杂子树。
- 展开、收起和 motion completion 不重新计算分隔线，也不创建边框 transition 或透明占位。
- 自定义 padding 的 margin 计算是常量时间，不进入渲染热路径。

## 9. 维护不变量

内部重构必须保持以下不变量：

- template reapply 时旧 `_expandButton.Click` 必须解除。
- detach 时必须取消 motion 并清理临时值。
- `TriggerType=Icon` 不能通过 Header 点击切换状态。
- 默认 `ExpandIcon` 为空时必须使用 `RightOutlined`，且不覆盖用户显式图标。
- `IsMotionEnabled=false` 立即释放内部进度和布局接入，保留用户或模板的尺寸与变换。
- 只有当前动画执行器的当前执行可以提交稳定状态或完成通知；旧 actor 的回调不能影响替换后的模板。
- Header/Content 分隔线只能由方向、边框厚度和视觉模式决定，不能依赖 `IsExpanded` 或 motion 时序。
- `ExpandDirection` 的 motion 方向、Header dock、Header transform 和图标旋转必须同步维护。
- 自定义 HeaderPadding 下的图标间距必须跟随 HeaderPadding 对应方向，不回退到默认 SizeType token。
- `:custom-header-padding` 和 `:custom-content-padding` 的伪类语义不能混用。
- Expander 不引入多面板或手风琴状态；这属于 Collapse 的职责。

## 10. 测试与验证

验证范围：

- `ExpanderBehaviorTests.Custom_Header_Padding_Uses_Compact_Expand_Icon_Layout`：自定义 HeaderPadding 下展开图标间距和垂直居中。
- `ExpanderBehaviorTests.Content_Separator_Is_Owned_By_Content_And_Follows_Direction`：Header 分隔线归零、未命名 Content border owner 和四方向 thickness 映射。
- `ExpanderBehaviorTests.Content_Separator_Remains_Adjacent_To_Header_In_Every_Direction`：四方向下 Header 与 Content 的物理相邻关系。
- `ExpanderBehaviorTests.Ghost_And_Borderless_Mode_Update_Frame_And_Content_Separator_At_Runtime`：运行时切换默认、Borderless 和 Ghost 模式时根边框与 Content 分隔线同步归零或恢复。
- `ExpanderBehaviorTests.Icon_Trigger_Does_Not_Toggle_From_Header_Click`：Icon 触发模式下 Header 点击不切换。
- `ExpanderBehaviorTests.Content_Motion_Reversal_Does_Not_Change_Separator_And_Uses_Latest_Expanded_State`：快速反向 motion 使用最新展开状态，且分隔线保持不变。
- `ExpanderBehaviorTests.Template_Reapply_Clears_Old_Content_Motion_Actor_Values` / `Detach_Clears_Active_Content_Motion_Actor_Values`：模板重套用和 detach 的 cancellation 与临时值清理。
- Gallery Expander 示例：Basic、Size、Borderless、Ghost、Custom Padding、Direction、Nested、No Arrow、Icon Position、Trigger。
- 修改主题或动效时运行 Expander 定向测试；共享机制变化时增加该机制、NavMenu、Collapse 及必要直接消费者的定向验证。
- 文档改动运行 `git diff --check`，并检查相对链接存在。

共享契约的中间帧验证需要四方向分别覆盖首个 tick 前、最终 tick 后且 continuation 前、播放中反转、方向改变和
关闭动效。检查内容实际尺寸、文字行布局与局部坐标、标题相邻边及分隔线，不以 transform 非空或最终 `IsVisible`
作为连续动画的充分证据。

Gallery Direction 示例中的固定高度容器、换行文本和原有对齐约束必须保留；同时覆盖嵌套内容、自定义 padding、
Borderless / Ghost 及 1× / 2× 缩放。布局与截图验证不能替代原生宿主下的观感和交互走查。
