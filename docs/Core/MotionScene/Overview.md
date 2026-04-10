# AtomUI 动效系统概览

> 本文档描述 AtomUI 动效系统（Motion System）的整体设计、核心概念和系统分层。AtomUI 动效系统是 [Ant Design 5.0 动效规范](https://ant.design/docs/spec/motion) 在 .NET / Avalonia 平台上的忠实实现，完整复刻了 [Ant Motion 动画语言](https://motion.ant.design/language/basic) 的设计理念与动画体系。

---

## 1. Ant Design 动效设计哲学

### 1.1 动效价值

界面动效能加强用户认知且增加活力。Ant Design 定义了动效的四大核心价值：

| 价值 | 说明 |
|------|------|
| **增加体验舒适度** | 让用户认知过程更为自然，平滑衔接界面状态的切换 |
| **增加界面活力** | 第一时间吸引注意力，突出重点内容 |
| **描述层级关系** | 体现元素之间的层级与空间关系（如弹层从触发点缩放出现） |
| **提供反馈、明确意向** | 让用户清楚知道操作产生了什么效果（如点击按钮的涟漪反馈） |

### 1.2 三大设计原则

在企业级应用中，Ant Design 衍生出动效设计的三原则。AtomUI 在 C# / Avalonia 平台上严格遵循这些原则：

#### 自然（Natural）

自然的动效背后体现的是自然运动规律。动效在转换时保证视觉上的连贯性，让用户感知到动作是成立的、能够引起共鸣的。

> 以 Button 的动效设计为例，设计师将其想象成一片树叶飘浮在水面之上 — 当你去触碰它时，叶子会下浮再反弹，然后出现涟漪效果。这正是 AtomUI 中 `WaveSpiritDecorator` 点击涟漪的设计来源。

#### 高效（Performant）

企业级应用追求高效的用户体验，尽量节省过渡时间，快速完成动画效果。

> **进场与出场的不对称性**：出场动效不用大张旗鼓地吸引用户注意力，做到简单清晰即可。因此出场时间采用更快的速度，也不设置队列依次出场的形式，整块直接消失。这在 AtomUI 中体现为：**In 动效使用 Ease-Out 缓动**（快速出现，缓慢到位），**Out 动效使用 Ease-In 缓动**（缓慢启动，快速消失）。

#### 克制（Concise）

做有意义的动效，不去做太多修饰而干扰用户。

> 如菜单展开时，更注重的是菜单内容，而右侧图标切换并不是主要元素，不需要过度强调以分散用户注意。只需在不经意间切换、明确指示变化即可。

### 1.3 过渡设计模式

Ant Design 总结了过渡动效的三种核心应用场景，AtomUI 在各控件中均有对应实现：

| 应用场景 | 说明 | AtomUI 中的体现 |
|---------|------|----------------|
| **保持上下文** | 视图切换时维持空间连续性 | Slide 滑入/滑出（Drawer）、Zoom 缩放（Popup）、Collapse 折叠（TreeView） |
| **解释发生了什么** | 帮助用户理解增删改操作 | Fade 淡入/淡出（Tag 新增/删除）、Move 滑入（Message/Notification 弹出） |
| **改善感知性能** | 转移注意力缩短操作感知时间 | Loading 旋转动画、骨架屏脉冲动效 |

### 1.4 衡量动效意义

一个动效是否有意义，可通过以下标准衡量：

- **合理性** — 是否带有明确的目的性？是否助力交互体验？没有多余的动效。
- **性能** — 不能出现大幅度波动丢帧或卡顿。动效的体验须是流畅的，且不影响产品性能。

---

## 2. 设计目标

AtomUI 动效系统在忠实复刻 Ant Design 动效规范的基础上，针对 .NET / Avalonia 平台的特性进行了架构设计：

1. **忠实复刻 Ant Design 5.0 动效体系** — 五大类预置动效（Fade、Zoom、Slide、Move、Collapse）完全对应 Ant Design CSS 动画关键帧（`antFadeIn/Out`、`antZoomIn/Out`、`antSlideUpIn/Out`、`antMoveDownIn/Out`、`ant-motion-collapse`），参数与缓动曲线均与原版一致。
2. **Actor-Motion 分离** — 动画的 *定义*（Motion）与 *载体*（MotionActor）解耦，一个 Motion 可以作用于任何 MotionActor。
3. **双驱动模式** — 同时支持 **Transition 驱动**（属性变化插值）和 **Animation 驱动**（关键帧动画）两种模式，按场景选择最优方案。
4. **布局感知** — `BaseLayoutAwareMotionActor` 可在 Scale/Translate 动画期间正确重新计算布局，确保父容器尺寸随动画平滑变化。
5. **与 Design Token 深度集成** — 动画时长和缓动曲线从全局 `SharedToken`（DesignToken）的 Seed → Map 层派生，保证全局一致性，且支持 Compact 主题缩短时长。
6. **全局开关与无障碍** — 通过 `EnableMotion` Seed Token 控制全局动画开关；控件通过 `IMotionAwareControl` 接口响应该开关。类似 Ant Design 中 `@media (prefers-reduced-motion: reduce)` 的无动画降级。
7. **可扩展** — 开发者可继承 `AbstractMotion` 创建自定义动效，也可继承 `BaseMotionActor` 创建自定义动画载体。

---

## 3. Ant Design 与 AtomUI 动效映射

### 3.1 五大动效类别映射

AtomUI 完整复刻了 Ant Design `components/style/motion/` 目录下定义的五大类 CSS 动画关键帧：

| Ant Design CSS 动画 | AtomUI Motion 类 | 变换属性 | 典型适用控件 |
|---------------------|-----------------|---------|-------------|
| `antFadeIn` / `antFadeOut` | `FadeInMotion` / `FadeOutMotion` | opacity | Tooltip、Badge |
| `antZoomIn` / `antZoomOut` | `ZoomInMotion` / `ZoomOutMotion` | scale + opacity | — |
| `antZoomBigIn` / `antZoomBigOut` | `ZoomBigInMotion` / `ZoomBigOutMotion` | scale + opacity | Popup、Select、ContextMenu |
| `antZoomUpIn` / `antZoomUpOut` ... | `ZoomUpInMotion` / `ZoomUpOutMotion` ... | scale + opacity (方向性) | DatePicker、Dropdown |
| `antSlideUpIn` / `antSlideUpOut` ... | `SlideUpInMotion` / `SlideUpOutMotion` ... | scaleY/scaleX + opacity | CollapseItem、NavMenu |
| `antMoveDownIn` / `antMoveDownOut` ... | `MoveDownInMotion` / `MoveDownOutMotion` ... | translate + opacity | Drawer、Message、Notification |
| `ant-motion-collapse` | `CollapseMotion` / `ExpandMotion` | scaleY/scaleX + opacity | TreeViewItem、Expander |

### 3.2 缓动曲线 Token 映射

Ant Design 在 Seed Token 中定义了一组预设缓动曲线（`motionEaseOutCirc`、`motionEaseInQuint` 等），AtomUI 将其映射为 Avalonia 的 Easing 类：

| Ant Design Seed Token | CSS 值 | AtomUI Easing 类 | 用于 |
|-----------------------|--------|------------------|------|
| `motionEaseOutCirc` | `cubic-bezier(0.08, 0.82, 0.17, 1)` | `CircularEaseOut` | Zoom 进入、Move 进入 |
| `motionEaseInOutCirc` | `cubic-bezier(0.78, 0.14, 0.15, 0.86)` | `CircularEaseInOut` | Zoom 退出、Move 退出 |
| `motionEaseOutQuint` | `cubic-bezier(0.23, 1, 0.32, 1)` | `QuinticEaseOut` | Slide 进入 |
| `motionEaseInQuint` | `cubic-bezier(0.755, 0.05, 0.855, 0.06)` | `QuinticEaseIn` | Slide 退出 |
| `motionEaseInOut` | `cubic-bezier(0.645, 0.045, 0.355, 1)` | `CubicEaseInOut` | Collapse 折叠/展开 |
| `motionEaseOut` | `cubic-bezier(0.215, 0.61, 0.355, 1)` | `CubicEaseOut` | 通用出场 |
| `motionEaseOutBack` | `cubic-bezier(0.12, 0.4, 0.29, 1.46)` | `BackEaseOut` | 弹性过冲效果 |
| `motionEaseInBack` | `cubic-bezier(0.71, -0.46, 0.88, 0.6)` | `BackEaseIn` | 弹性回缩效果 |
| — (linear) | `linear` | `LinearEasing` | Fade |

> **设计原则体现**: Ant Design 的 Zoom/Move 动效 **进入使用 `motionEaseOutCirc`**，**退出使用 `motionEaseInOutCirc`**；Slide 动效 **进入使用 `motionEaseOutQuint`**，**退出使用 `motionEaseInQuint`**。这体现了"高效"原则 — 进入时快速出现（Ease-Out），退出时不过多吸引注意力。

### 3.3 WaveSpirit 与 Ant Design Wave 效果

Ant Design 的 Wave（涟漪）效果是其标志性的交互反馈，源自"树叶飘浮在水面"的设计隐喻。原版实现在 `components/_util/wave/` 中，通过 `box-shadow` 从 0 扩展到 6px 并伴随 opacity 从 0.2 淡出到 0 来实现。AtomUI 的 `WaveSpiritDecorator` 使用独立的 Size + Opacity 双动画管线忠实复刻了此效果，并增加了三种波纹形状（`RoundRectWave`、`PillWave`、`CircleWave`）以适配更多控件形态。

---

## 4. 系统架构总览

```
┌──────────────────────────────────────────────────────────────────────────────────────┐
│  AtomUI.Core — 动效基础设施                                                          │
│                                                                                      │
│  MotionScene/                              Animations/                                │
│  ├── IMotion (接口)                        ├── InterpolateUtils (插值算法库)           │
│  ├── AbstractMotion (抽象基类)             ├── BaseTransitionUtils (过渡创建工厂)      │
│  ├── IMotionActor (接口)                   ├── AnimationExtensions (辅助扩展)          │
│  ├── BaseMotionActor (渲染变换载体)        └── Transitions/                            │
│  ├── BaseLayoutAwareMotionActor            　   ├── INotifyTransitionCompleted         │
│  │   (布局感知载体)                         　  ├── AbstractNotifiableTransition<T>     │
│  ├── SceneLayer (顶层浮动渲染层)           　   ├── NotifiableDoubleTransition          │
│  ├── MotionTransformOptionsAnimator        　   ├── NotifiableTransformOperationsTransition │
│  │                                          　  ├── SolidColorBrushTransition           │
│  │  预置动效 (← Ant Design CSS @keyframes):　  ├── BoxShadowsTransition                │
│  ├── FadeMotions  (← antFadeIn/Out)        　   └── ... (更多类型)                      │
│  ├── ZoomMotions  (← antZoom*In/Out)                                                  │
│  ├── SlideMotions (← antSlide*In/Out)                                                 │
│  ├── MoveMotions  (← antMove*In/Out)                                                  │
│  └── CollapseMotions (← ant-motion-collapse)                                          │
│                                                                                      │
│  Controls/                                  Utils/                                    │
│  ├── IMotionAwareControl (全局开关接口)     └── MotionAwareControlExtensions           │
│  └── TransitionUtils (Token 感知过渡工厂)       (自动绑定 EnableMotion Token)           │
│                                                                                      │
├──────────────────────────────────────────────────────────────────────────────────────┤
│  AtomUI.Controls — 基础控件层                                                         │
│  ├── MotionActor (BaseMotionActor 的具体实现，用于 XAML 模板)                          │
│  └── WaveSpiritDecorator (← Ant Design Wave 点击涟漪波纹动画)                          │
│                                                                                      │
├──────────────────────────────────────────────────────────────────────────────────────┤
│  AtomUI.Desktop.Controls — 桌面控件层                                                  │
│  ├── Popup (SceneLayer 顶层动画、ZoomBigIn/Out)                                       │
│  ├── Drawer (MoveIn/Out 滑入滑出)                                                     │
│  ├── Notification/Message (自定义 NotificationMotion)                                  │
│  ├── CollapseItem / Expander / TreeViewItem (Collapse/Expand + Slide)                │
│  ├── FloatButtonGroup (MoveIn/Out 方向性动画)                                          │
│  └── ... (150+ 控件使用动效系统)                                                       │
└──────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 5. 核心概念

### 5.1 Motion（动效定义）

**Motion** 是一个纯粹的"动画方案描述"，对应 Ant Design 中 `components/style/motion/` 下的 CSS `@keyframes` 定义。它定义了：

- 动画的 **时长**（Duration）— 对应 `motionDurationMid`、`motionDurationFast` 等 Token
- 动画的 **缓动曲线**（Easing）— 对应 `motionEaseOutCirc`、`motionEaseInQuint` 等 Token
- 动画的 **填充模式**（FillMode）— 对应 CSS `animationFillMode: both`
- 动画的 **变换原点**（RenderTransformOrigin）— 对应 CSS `transformOrigin`
- 动画的 **起始值** 和 **结束值**（Opacity、Transform）— 对应 CSS `@keyframes` 的 0% 和 100%
- 动画的 **驱动模式**（Transition 或 Animation）

Motion 本身 **不持有任何 UI 状态**，它是一个可重用的动画模板。

所有预置 Motion 均继承自 `AbstractMotion`：

| 类别 | In（进入） | Out（退出） | Ant Design 对应 |
|------|-----------|------------|-----------------|
| **Fade** | `FadeInMotion` | `FadeOutMotion` | `antFadeIn` / `antFadeOut` |
| **Zoom** | `ZoomInMotion`、`ZoomBigInMotion`、`ZoomUpInMotion` 等 | 对应 `*OutMotion` | `antZoomIn` / `antZoomBigIn` / `antZoomUpIn` 等 |
| **Slide** | `SlideUpInMotion`、`SlideDownInMotion` 等 | 对应 `*OutMotion` | `antSlideUpIn` / `antSlideDownIn` 等 |
| **Move** | `MoveUpInMotion`、`MoveDownInMotion` 等 | 对应 `*OutMotion` | `antMoveUpIn` / `antMoveDownIn` 等 |
| **Collapse** | `ExpandMotion` | `CollapseMotion` | `ant-motion-collapse` |

### 5.2 MotionActor（动画载体）

**MotionActor** 是动画的执行载体 — 它是一个 `ContentControl`，通过 `MotionTransform` 属性驱动 RenderTransform 变化。

- `BaseMotionActor` — 基础载体，仅做 RenderTransform 变换，不影响布局。
- `BaseLayoutAwareMotionActor` — 布局感知载体，Scale 变换会重新计算 Measure/Arrange，使父容器随动画平滑缩放。
- `MotionActor` — `BaseMotionActor` 的具体实现类，用于 XAML ControlTheme 模板中。

### 5.3 MotionSpiritType（驱动模式）

| 模式 | 适用场景 | 实现机制 | Ant Design 对应 |
|------|---------|---------|-----------------|
| `Transition` | 简单的属性插值过渡（Fade、Zoom、Slide、Collapse） | 设置起始值 → 添加 Avalonia Transition → 设置结束值，框架自动插值 | CSS `transition` 属性（如 Collapse 的 `height/opacity` 过渡） |
| `Animation` | 复杂的多关键帧动画（Move 系列） | 使用 Avalonia `Animation` + `KeyFrame` + `Cue` 描述完整时间线 | CSS `@keyframes` + `animation` 属性 |

### 5.4 SceneLayer（浮动渲染层）

`SceneLayer` 是一个轻量级的顶层透明窗口（基于 `IPopupImpl`），用于在控件所在视觉树之外渲染动画。典型用途是 Popup 打开/关闭时在独立层上播放缩放动画，避免被父容器 ClipToBounds 裁剪。

### 5.5 WaveSpirit（点击涟漪）

`WaveSpiritDecorator` 实现了 Ant Design 标志性的"点击波纹扩散"效果 — 源自"树叶飘浮在水面"的设计隐喻。Ant Design 中通过 `box-shadow: 0 0 0 6px currentcolor` + opacity 动画实现；AtomUI 使用独立的 Size + Opacity 双动画管线实现，支持三种波纹形状：`RoundRectWave`、`PillWave`、`CircleWave`。

---

## 6. 与 Design Token 集成

### 6.1 动效时长 Token

动效时长通过 **Seed Token → Map Token** 层级派生，与 Ant Design 的 `motionUnit` / `motionBase` Token 完全对应：

```
Seed Token (← Ant Design SeedToken):
  MotionBase = 0          // 基础时长（ms）← motionBase
  MotionUnit = 100        // 时长步进（ms）← motionUnit
  EnableMotion = true     // 全局开关      ← motion

Map Token (由 CalculatorUtils 自动计算):
  MotionDurationFast     = MotionBase + MotionUnit × 1  = 100ms  // 快速，用于小型元素
  MotionDurationMid      = MotionBase + MotionUnit × 2  = 200ms  // 中速，用于中型元素（默认值）
  MotionDurationSlow     = MotionBase + MotionUnit × 3  = 300ms  // 慢速，用于大型面板
  MotionDurationVerySlow = MotionBase + MotionUnit × 8  = 800ms  // 最慢，用于特殊场景
```

在 Ant Design 中，大多数动效默认使用 `motionDurationMid`（200ms），如 Fade、Zoom、Slide、Move、Collapse 均是如此。`zoom-big-fast` 变体使用 `motionDurationFast`（100ms）。在 Compact 主题下，`MotionUnit` 可能被缩短以匹配紧凑节奏。

### 6.2 缓动曲线 Token

Ant Design 将缓动曲线定义为 Seed Token（CSS `cubic-bezier` 值），AtomUI 将其映射为 Avalonia 的 `Easing` 类。各动效类别使用的缓动曲线遵循以下规律：

| 动效类别 | 进入 (Enter/Appear) | 退出 (Leave) |
|---------|---------------------|-------------|
| **Fade** | `linear` | `linear` |
| **Zoom** | `motionEaseOutCirc` → `CircularEaseOut` | `motionEaseInOutCirc` → `CircularEaseInOut` |
| **Slide** | `motionEaseOutQuint` → `QuinticEaseOut` | `motionEaseInQuint` → `QuinticEaseIn` |
| **Move** | `motionEaseOutCirc` → `CircularEaseOut` | `motionEaseInOutCirc` → `CircularEaseInOut` |
| **Collapse** | `motionEaseInOut` → `CubicEaseInOut` | `motionEaseInOut` → `CubicEaseInOut` |

### 6.3 全局开关绑定

控件通过实现 `IMotionAwareControl` 接口并调用 `this.ConfigureMotionBindingStyle()` 自动将 `IsMotionEnabled` 属性绑定到全局 `EnableMotion` Token：

```csharp
// 在控件构造函数中
this.ConfigureMotionBindingStyle();
// 等效于
var style = new Style();
style.Add(MotionAwareControlProperty.IsMotionEnabledProperty, SharedTokenKind.EnableMotion);
this.Styles.Add(style);
```

> **无障碍设计**: Ant Design 通过 `@media (prefers-reduced-motion: reduce)` 提供无动画降级。AtomUI 通过 `EnableMotion = false` 实现相同效果 — 当全局开关关闭时，所有实现了 `IMotionAwareControl` 的控件自动跳过动画，直接到达最终状态。

---

## 7. 源文件清单

### MotionScene（`src/AtomUI.Core/MotionScene/`）

| 文件 | 职责 | Ant Design 对应 |
|------|------|-----------------|
| `IMotion.cs` | Motion 接口定义 + `MotionSpiritType` 枚举 | `initMotion()` 入口 |
| `AbstractMotion.cs` | Motion 抽象基类，包含双模式执行引擎 | `motion.ts` 公共样式生成 |
| `IMotionActor.cs` | MotionActor 接口 + `MotionActorControlProperty` 共享属性 | — |
| `BaseMotionActor.cs` | 基础动画载体（仅 RenderTransform） | — |
| `BaseLayoutAwareMotionActor.cs` | 布局感知动画载体（Scale 影响 Measure/Arrange） | — |
| `SceneLayer.cs` | 顶层浮动透明渲染窗口 | — |
| `MotionTransformOptionsAnimator.cs` | TransformOperations 自定义插值器 | — |
| `FadeMotions.cs` | Fade 系列动效（FadeIn/FadeOut） | `fade.ts` — `antFadeIn/Out` |
| `ZoomMotions.cs` | Zoom 系列动效（12 种方向变体） | `zoom.ts` — `antZoom*In/Out` |
| `SlideMotions.cs` | Slide 系列动效（8 种方向变体） | `slide.ts` — `antSlide*In/Out` |
| `MoveMotions.cs` | Move 系列动效（8 种方向变体） | `move.ts` — `antMove*In/Out` |
| `CollapseMotions.cs` | Collapse/Expand 动效（4 个方向） | `collapse.ts` — `ant-motion-collapse` |

### Animations（`src/AtomUI.Core/Animations/`）

| 文件 | 职责 |
|------|------|
| `InterpolateUtils.cs` | 各类型插值算法（Color、Double、BoxShadows、CornerRadius 等） |
| `BaseTransitionUtils.cs` | 通用 Transition 创建工厂 |
| `AnimationExtensions.cs` | Avalonia Animation 辅助扩展 |
| `AnimatableReflectionExtensions.cs` | 通过反射操作 Avalonia 内部 EnableTransitions/DisableTransitions |
| `Transitions/INotifyTransitionCompleted.cs` | 可通知完成的 Transition 接口 |
| `Transitions/AbstractNotifiableTransition.cs` | 可通知完成的 Transition 抽象基类 |
| `Transitions/Notifiable*.cs` | 各类型的可通知 Transition 实现 |
| `Transitions/SolidColorBrushTransition.cs` | SolidColorBrush 插值过渡 |
| `Transitions/BoxShadowsTransition.cs` | BoxShadows 插值过渡 |

### 相关辅助文件

| 文件 | 职责 |
|------|------|
| `Controls/IMotionAwareControl.cs` | 全局动画开关接口 + 共享属性 |
| `Controls/AnimationUtils.cs` | Token 感知的 Transition 创建工厂 |
| `Utils/MotionAwareControlExtensions.cs` | 自动绑定 EnableMotion Token |

---

## 8. 文档导航

| 文档 | 内容 |
|------|------|
| [Architecture.md](./Architecture.md) | 核心架构详解 — Actor-Motion 模型、双驱动引擎、生命周期 |
| [BuiltinMotions.md](./BuiltinMotions.md) | 预置动效完整参考 — 五大类动效与 Ant Design CSS 关键帧的精确对照 |
| [DeveloperGuide.md](./DeveloperGuide.md) | 开发者指南 — 使用预置动效、创建自定义动效、控件集成最佳实践 |

