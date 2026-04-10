# AtomUI 预置动效参考

> 本文档详细描述 AtomUI 动效系统中所有预置 Motion 类的参数、效果、变换原点、缓动曲线及适用场景，并标注与 Ant Design CSS `@keyframes` 定义的精确对应关系。

---

## 1. 命名规则

所有预置 Motion 遵循统一命名模式，与 Ant Design 的 CSS 动画命名保持对应：

```
AtomUI:      {Category}{Direction}{In|Out}Motion
Ant Design:  ant{Category}{Direction}{In|Out}       (CSS @keyframes 名称)
```

| 组成部分 | 说明 | 示例 |
|---------|------|------|
| **Category** | `Fade` / `Zoom` / `Slide` / `Move` / `Collapse` / `Expand` | `ZoomBigInMotion` ↔ `antZoomBigIn` |
| **Direction** | `Up` / `Down` / `Left` / `Right` / `Big`（部分类别无方向） | `SlideUpInMotion` ↔ `antSlideUpIn` |
| **In/Out** | `In` 表示进入（对应 CSS enter/appear），`Out` 表示退出（对应 CSS leave） | — |

---

## 2. Fade 动效

最简单的动效 — 仅控制 **透明度** 变化。

**Ant Design 源码对应**: `components/style/motion/fade.ts` — `antFadeIn` / `antFadeOut`

```typescript
// Ant Design CSS @keyframes 定义
antFadeIn:  { '0%': { opacity: 0 }, '100%': { opacity: 1 } }
antFadeOut: { '0%': { opacity: 1 }, '100%': { opacity: 0 } }
// animationTimingFunction: 'linear'
// duration: token.motionDurationMid (200ms)
```

### FadeInMotion — 淡入

| 属性 | 值 | Ant Design 对应 |
|------|-----|-----------------|
| 默认时长 | 300ms | `motionDurationMid` |
| 默认缓动 | `LinearEasing` | `animationTimingFunction: 'linear'` |
| 驱动模式 | `Transition` | — |
| 变换原点 | (0, 0) Relative | — |
| Opacity | 0.0 → 1.0 | `opacity: 0` → `opacity: 1` |
| Transform | 无 | 无 |

### FadeOutMotion — 淡出

| 属性 | 值 | Ant Design 对应 |
|------|-----|-----------------|
| 默认时长 | 300ms | `motionDurationMid` |
| 默认缓动 | `LinearEasing` | `animationTimingFunction: 'linear'` |
| 驱动模式 | `Transition` | — |
| 变换原点 | (0, 0) Relative | — |
| Opacity | 1.0 → 0.0 | `opacity: 1` → `opacity: 0` |
| Transform | 无 | 无 |

**适用场景**: 简单的显示/隐藏过渡，如 ToolTip、Badge 等。

**设计原则**: Fade 使用线性缓动（无加速/减速），因为仅有透明度变化，不涉及空间运动，不需要模拟物理运动的节奏感。

---

## 3. Zoom 动效

控制 **透明度 + 等比缩放**。有多种变换原点变体。

**Ant Design 源码对应**: `components/style/motion/zoom.ts`

**缓动规律**（与 Ant Design 一致）:
- **进入 (Enter/Appear)**: `motionEaseOutCirc` → `CircularEaseOut` — 快速放大到位，体现"自然"原则
- **退出 (Leave)**: `motionEaseInOutCirc` → `CircularEaseInOut` — 平滑缩小消失，体现"克制"原则

### 3.1 基础 Zoom

**Ant Design**: `antZoomIn` / `antZoomOut`

```typescript
// Ant Design CSS @keyframes 定义
antZoomIn:  { '0%': { transform: 'scale(0.2)', opacity: 0 }, '100%': { transform: 'scale(1)', opacity: 1 } }
antZoomOut: { '0%': { transform: 'scale(1)' },               '100%': { transform: 'scale(0.2)', opacity: 0 } }
```

#### ZoomInMotion

| 属性 | 值 | Ant Design 对应 |
|------|-----|-----------------|
| 默认缓动 | `CircularEaseOut` | `motionEaseOutCirc` |
| 驱动模式 | `Transition` | — |
| 变换原点 | **(0, 0) Relative**（左上角） | — |
| Opacity | 0.0 → 1.0 | `opacity: 0` → `opacity: 1` |
| Scale | 0.2 → 1.0 | `scale(0.2)` → `scale(1)` |

#### ZoomOutMotion

| 属性 | 值 | Ant Design 对应 |
|------|-----|-----------------|
| 默认缓动 | `CircularEaseOut` | `motionEaseInOutCirc` |
| 变换原点 | (0, 0) Relative | — |
| Opacity | 1.0 → 0.0 | `opacity: 1` → `opacity: 0` |
| Scale | 1.0 → 0.2 | `scale(1)` → `scale(0.2)` |
| 完成后 | 重置 Opacity=1.0, MotionTransform=null | — |

### 3.2 ZoomBig（中心缩放）

**Ant Design**: `antZoomBigIn` / `antZoomBigOut`

```typescript
// Ant Design CSS @keyframes 定义
antZoomBigIn:  { '0%': { transform: 'scale(0.8)', opacity: 0 }, '100%': { transform: 'scale(1)', opacity: 1 } }
antZoomBigOut: { '0%': { transform: 'scale(1)' },               '100%': { transform: 'scale(0.8)', opacity: 0 } }
// duration: motionDurationMid（标准）或 motionDurationFast（zoom-big-fast 变体）
```

#### ZoomBigInMotion

| 属性 | 值 | Ant Design 对应 |
|------|-----|-----------------|
| 默认缓动 | `CircularEaseOut` | `motionEaseOutCirc` |
| 变换原点 | **(0.5, 0.5) Relative**（正中心） | — |
| Opacity | 0.01 → 1.0 | `opacity: 0` → `opacity: 1` |
| Scale | 0.35 → 1.0 | `scale(0.8)` → `scale(1)` |

#### ZoomBigOutMotion

| 属性 | 值 | Ant Design 对应 |
|------|-----|-----------------|
| 默认缓动 | `CircularEaseOut` | `motionEaseInOutCirc` |
| 变换原点 | **RelativePoint.Center** | — |
| Opacity | 1.0 → 0.01 | `opacity: 1` → `opacity: 0` |
| Scale | 1.0 → 0.85 | `scale(1)` → `scale(0.8)` |
| 完成后 | 重置 Opacity=1.0, MotionTransform=null | — |

**适用场景**: Popup、ContextMenu、Select 下拉面板 — 从中心放大出现/缩小消失。这是 AtomUI 最常用的 Popup 动效，体现了 Ant Design "描述层级关系"的动效价值 — 弹出层从触发点位置缩放出现，建立了弹层与触发元素之间的空间关联。

### 3.3 方向性 Zoom

**Ant Design**: `antZoomUpIn/Out`、`antZoomDownIn/Out`、`antZoomLeftIn/Out`、`antZoomRightIn/Out`

```typescript
// Ant Design CSS @keyframes 定义（以 ZoomUp 为例）
antZoomUpIn:  { '0%': { transform: 'scale(0.8)', transformOrigin: '50% 0%', opacity: 0 }, '100%': { transform: 'scale(1)', transformOrigin: '50% 0%' } }
antZoomUpOut: { '0%': { transform: 'scale(1)', transformOrigin: '50% 0%' },               '100%': { transform: 'scale(0.8)', transformOrigin: '50% 0%', opacity: 0 } }
```

| 类名 | 变换原点 | Ant Design `transformOrigin` | 效果描述 |
|------|---------|------------------------------|---------|
| `ZoomUpInMotion` | (0.5, **0.0**) | `50% 0%` | 从顶部边缘放大进入 |
| `ZoomUpOutMotion` | (0.5, **0.0**) | `50% 0%` | 向顶部边缘缩小退出 |
| `ZoomDownInMotion` | (0.5, **1.0**) | `50% 100%` | 从底部边缘放大进入 |
| `ZoomDownOutMotion` | (0.5, **1.0**) | `50% 100%` | 向底部边缘缩小退出 |
| `ZoomLeftInMotion` | (**0.0**, 0.5) | `0% 50%` | 从左侧边缘放大进入 |
| `ZoomLeftOutMotion` | (**0.0**, 0.5) | `0% 50%` | 向左侧边缘缩小退出 |
| `ZoomRightInMotion` | (**1.0**, 0.5) | `100% 50%` | 从右侧边缘放大进入 |
| `ZoomRightOutMotion` | (**1.0**, 0.5) | `100% 50%` | 向右侧边缘缩小退出 |

所有方向性 Zoom 的共同参数：
- 缓动: 进入 `CircularEaseOut`（← `motionEaseOutCirc`），退出 `CircularEaseInOut`（← `motionEaseInOutCirc`）
- Scale: 0.8 ↔ 1.0（← Ant Design `scale(0.8)` ↔ `scale(1)`）
- Opacity: 0.0 ↔ 1.0
- Out 变体完成后重置 Opacity=1.0, MotionTransform=null

**适用场景**: 需要从特定方向"生长"出来的面板，如 DatePicker 日历面板从顶部出现。变换原点的选择遵循 Ant Design "保持上下文"的过渡设计模式 — 面板从触发位置方向出现，维持空间连续性。

---

## 4. Slide 动效

控制 **透明度 + 单轴缩放**（ScaleX 或 ScaleY），产生"展开/收起"的视觉效果。

**Ant Design 源码对应**: `components/style/motion/slide.ts`

**缓动规律**（与 Ant Design 一致）:
- **进入 (Enter/Appear)**: `motionEaseOutQuint` → `QuinticEaseOut` — 快速展开
- **退出 (Leave)**: `motionEaseInQuint` → `QuinticEaseIn` — 快速收起

> **设计原则体现**: Slide 使用了比 Zoom 更激进的五次方缓动曲线（Quintic），因为展开/收起是一种更直接的空间变化，需要更快速地到达最终状态（"高效"原则）。

### 4.1 垂直 Slide

**Ant Design**: `antSlideUpIn/Out`、`antSlideDownIn/Out`

```typescript
// Ant Design CSS @keyframes 定义
antSlideUpIn:  { '0%': { transform: 'scaleY(0.8)', transformOrigin: '0% 0%', opacity: 0 }, '100%': { transform: 'scaleY(1)', transformOrigin: '0% 0%', opacity: 1 } }
antSlideUpOut: { '0%': { transform: 'scaleY(1)', transformOrigin: '0% 0%', opacity: 1 },   '100%': { transform: 'scaleY(0.8)', transformOrigin: '0% 0%', opacity: 0 } }
antSlideDownIn:  { '0%': { transform: 'scaleY(0.8)', transformOrigin: '100% 100%', opacity: 0 }, '100%': { ... } }
antSlideDownOut: { '0%': { transform: 'scaleY(1)', transformOrigin: '100% 100%', opacity: 1 },   '100%': { ... } }
```

#### SlideUpInMotion — 从顶部向下展开

| 属性 | 值 | Ant Design 对应 |
|------|-----|-----------------|
| 默认缓动 | `CubicEaseOut` | `motionEaseOutQuint` |
| 驱动模式 | `Transition` | — |
| 变换原点 | (0, 0) Relative（顶部左侧） | `transformOrigin: '0% 0%'` |
| Opacity | 0.0 → 1.0 | `opacity: 0` → `opacity: 1` |
| ScaleY | 0.8 → 1.0 | `scaleY(0.8)` → `scaleY(1)` |

#### SlideUpOutMotion — 向顶部收起

| 属性 | 值 | Ant Design 对应 |
|------|-----|-----------------|
| 默认时长 | 300ms | `motionDurationMid` |
| 默认缓动 | `CubicEaseIn` | `motionEaseInQuint` |
| 变换原点 | (0, 0) Relative | `transformOrigin: '0% 0%'` |
| Opacity | 1.0 → 0.0 | `opacity: 1` → `opacity: 0` |
| ScaleY | 1.0 → 0.8 | `scaleY(1)` → `scaleY(0.8)` |

#### SlideDownInMotion — 从底部向上展开

| 属性 | 值 | Ant Design 对应 |
|------|-----|-----------------|
| 默认缓动 | `CubicEaseOut` | `motionEaseOutQuint` |
| 变换原点 | **(1.0, 1.0) Relative**（右下角） | `transformOrigin: '100% 100%'` |
| Opacity | 0.0 → 1.0 | `opacity: 0` → `opacity: 1` |
| ScaleY | 0.8 → 1.0 | `scaleY(0.8)` → `scaleY(1)` |

#### SlideDownOutMotion — 向底部收起

| 属性 | 值 | Ant Design 对应 |
|------|-----|-----------------|
| 默认缓动 | `CubicEaseIn` | `motionEaseInQuint` |
| 变换原点 | (1.0, 1.0) Relative | `transformOrigin: '100% 100%'` |
| Opacity | 1.0 → 0.0 | `opacity: 1` → `opacity: 0` |
| ScaleY | 1.0 → 0.8 | `scaleY(1)` → `scaleY(0.8)` |

### 4.2 水平 Slide

**Ant Design**: `antSlideLeftIn/Out`、`antSlideRightIn/Out`

```typescript
// Ant Design CSS @keyframes 定义
antSlideLeftIn:  { '0%': { transform: 'scaleX(0.8)', transformOrigin: '0% 0%', opacity: 0 }, '100%': { ... } }
antSlideRightIn: { '0%': { transform: 'scaleX(0.8)', transformOrigin: '100% 0%', opacity: 0 }, '100%': { ... } }
```

| 类名 | 变换原点 | Ant Design `transformOrigin` | 缩放轴 | 缓动（进入/退出） |
|------|---------|------------------------------|--------|------------------|
| `SlideLeftInMotion` | (0, 0) | `0% 0%` | ScaleX 0.8→1.0 | `CubicEaseOut` / — |
| `SlideLeftOutMotion` | (0, 0) | `0% 0%` | ScaleX 1.0→0.8 | — / `CubicEaseIn` |
| `SlideRightInMotion` | (1.0, 0) | `100% 0%` | ScaleX 0.8→1.0 | `CubicEaseOut` / — |
| `SlideRightOutMotion` | (1.0, 0) | `100% 0%` | ScaleX 1.0→0.8 | — / `CubicEaseIn` |

**适用场景**: CollapseItem 展开/折叠内容区域、NavMenuItem 子菜单展开。体现了 Ant Design "保持上下文"的过渡设计模式 — 通过方向性展开/收起维持空间连续性。

---

## 5. Move 动效

控制 **透明度 + 位移（Translate）**，产生"滑入/滑出"的视觉效果。

**Ant Design 源码对应**: `components/style/motion/move.ts`

**缓动规律**（与 Ant Design 一致）:
- **进入 (Enter/Appear)**: `motionEaseOutCirc` → `CircularEaseOut`
- **退出 (Leave)**: `motionEaseInOutCirc` → `CircularEaseInOut`

> **重要**: Move 系列使用 **Animation 驱动模式**（`MotionSpiritType.Animation`），因为它需要三个关键帧（0% → 80% → 100%）实现非线性的位移曲线。Ant Design 的 CSS 版本虽然只定义了 0% 和 100% 两帧，但 AtomUI 增加了中间关键帧以获得更流畅的运动效果。

```typescript
// Ant Design CSS @keyframes 定义
antMoveDownIn:  { '0%': { transform: 'translate3d(0, 100%, 0)', transformOrigin: '0 0', opacity: 0 }, '100%': { transform: 'translate3d(0, 0, 0)', transformOrigin: '0 0', opacity: 1 } }
antMoveDownOut: { '0%': { transform: 'translate3d(0, 0, 0)', transformOrigin: '0 0', opacity: 1 },    '100%': { transform: 'translate3d(0, 100%, 0)', transformOrigin: '0 0', opacity: 0 } }
antMoveUpIn:    { '0%': { transform: 'translate3d(0, -100%, 0)', ... }, '100%': { transform: 'translate3d(0, 0, 0)', ... } }
antMoveLeftIn:  { '0%': { transform: 'translate3d(-100%, 0, 0)', ... }, '100%': { transform: 'translate3d(0, 0, 0)', ... } }
antMoveRightIn: { '0%': { transform: 'translate3d(100%, 0, 0)', ... },  '100%': { transform: 'translate3d(0, 0, 0)', ... } }
```

### 5.1 构造参数

所有 Move 动效需要一个额外的 `offset` 参数，指定位移距离（像素），对应 Ant Design 中 `translate3d` 的百分比位移（AtomUI 使用绝对像素值，因为 Avalonia 不支持百分比变换）：

```csharp
var motion = new MoveDownInMotion(
    offset: 200.0,        // 位移距离（Ant Design 使用 100% 容器尺寸）
    duration: duration,   // 时长
    easing: new CubicEaseOut()
);
```

### 5.2 关键帧结构

以 `MoveDownInMotion` 为例：

| Cue | Opacity | TranslateY | ScaleY |
|-----|---------|------------|--------|
| 0% | 0.0 | +offset | 1.0 |
| 80% | 0.1 | +offset/4 | 1.0 |
| 100% | 1.0 | 0 | 1.0 |

### 5.3 完整变体列表

| 类名 | 位移方向 | 缓动 | Ant Design 对应 |
|------|---------|------|-----------------|
| `MoveDownInMotion` | 从下方滑入（Y: +offset → 0） | `QuinticEaseOut` | `antMoveDownIn` |
| `MoveDownOutMotion` | 向下方滑出（Y: 0 → +offset） | `QuinticEaseIn` | `antMoveDownOut` |
| `MoveUpInMotion` | 从上方滑入（Y: -offset → 0） | `QuinticEaseOut` | `antMoveUpIn` |
| `MoveUpOutMotion` | 向上方滑出（Y: 0 → -offset） | `QuinticEaseIn` | `antMoveUpOut` |
| `MoveLeftInMotion` | 从左方滑入（X: -offset → 0） | `QuinticEaseOut` | `antMoveLeftIn` |
| `MoveLeftOutMotion` | 向左方滑出（X: 0 → -offset） | `QuinticEaseIn` | `antMoveLeftOut` |
| `MoveRightInMotion` | 从右方滑入（X: +offset → 0） | `QuinticEaseOut` | `antMoveRightIn` |
| `MoveRightOutMotion` | 向右方滑出（X: 0 → +offset） | `QuinticEaseIn` | `antMoveRightOut` |

### 5.4 SceneLayer 尺寸计算

Move 动效因为涉及位移，动画过程中控件会超出原始边界。因此 Move 动效重写了 `CalculateSceneSize` 和 `CalculateScenePosition`，将浮动渲染层的尺寸扩大为原始尺寸的 2 倍：

```csharp
// MoveDownInMotion
internal override Size CalculateSceneSize(Size actorSize)
    => actorSize.WithHeight(actorSize.Height * 2);

internal override Point CalculateScenePosition(Size actorSize, Point actorPosition)
    => actorPosition.WithY(actorPosition.Y + actorSize.Height);
```

**适用场景**: Drawer 抽屉滑入/滑出、FloatButtonGroup 菜单展开、Message/Notification 弹出。Move 动效建立了元素运动方向与空间位置的关联，体现了 Ant Design "描述层级关系"和"保持上下文"的设计价值。

---

## 6. Collapse/Expand 动效

控制 **透明度 + 单轴缩放**，用于折叠/展开。与 Slide 类似，但支持四个方向且缩放到几乎为零。

**Ant Design 源码对应**: `components/style/motion/collapse.ts` + `components/_util/motion.ts`

Ant Design 的 Collapse 动效实现方式与其他四类不同 — 它使用 CSS `transition`（过渡）而非 `@keyframes`（关键帧动画），通过 JavaScript 动态计算 `height` 值并配合 `overflow: hidden`：

```typescript
// Ant Design collapse.ts
transition: `height ${motionDurationMid} ${motionEaseInOut}, opacity ${motionDurationMid} ${motionEaseInOut}`
// Ant Design _util/motion.ts — initCollapseMotion
onAppearStart: getCollapsedHeight  // { height: 0, opacity: 0 }
onEnterStart: getCollapsedHeight
onAppearActive: getRealHeight      // { height: scrollHeight, opacity: 1 }
onEnterActive: getRealHeight
onLeaveStart: getCurrentHeight     // { height: offsetHeight }
onLeaveActive: getCollapsedHeight
motionDeadline: 500                // 500ms 超时兜底
```

AtomUI 使用 Scale 变换（而非直接操作 Height）配合 `BaseLayoutAwareMotionActor` 实现相同效果，既保留了布局感知特性，又避免了直接操作像素高度的复杂性。

### CollapseMotion — 折叠

```csharp
var motion = new CollapseMotion(
    direction: Direction.Top,     // 折叠方向
    duration: duration,
    easing: new CubicEaseOut()
);
```

| 方向 | 变换原点 | 缩放轴 | 起始→结束 |
|------|---------|--------|----------|
| `Left` | (1.0, 0.5) | ScaleX | 1.0 → 0.01 |
| `Right` | (0.0, 0.5) | ScaleX | 1.0 → 0.01 |
| `Top` | (0.5, 1.0) | ScaleY | 1.0 → 0.01 |
| `Bottom` | (0.5, 0.0) | ScaleY | 1.0 → 0.01 |

Opacity 同步 1.0 → 0.0。

### ExpandMotion — 展开

与 CollapseMotion 相反：Scale 从 0.01 → 1.0，Opacity 从 0.0 → 1.0。

**适用场景**: TreeViewItem 子节点展开/折叠、Expander 内容区域。体现了 Ant Design "保持上下文"的过渡设计模式 — 折叠/展开维持了信息的空间连续性。

---

## 7. 缓动曲线参考

预置动效使用的缓动曲线完全对应 Ant Design Seed Token 中定义的预设缓动（`motionEaseOutCirc`、`motionEaseInQuint` 等）：

| Ant Design Token | AtomUI Easing | 特征 | 使用场景 | 设计原则 |
|-----------------|---------------|------|---------|---------|
| — (linear) | `LinearEasing` | 匀速 | Fade | 简单透明度变化无需运动节奏 |
| `motionEaseOutCirc` | `CircularEaseOut` | 快速启动，缓慢停止 | Zoom In、Move In | "自然" — 进入时快速到位 |
| `motionEaseInOutCirc` | `CircularEaseInOut` | 先加速后减速 | Zoom Out、Move Out | "克制" — 退出时不过分强调 |
| `motionEaseOutQuint` | `QuinticEaseOut` | 极快启动，很慢停止 | Slide In | "高效" — 展开时极速到位 |
| `motionEaseInQuint` | `QuinticEaseIn` | 很慢启动，极快结束 | Slide Out | "高效" — 收起时快速完成 |
| `motionEaseInOut` | `CubicEaseInOut` | 对称加减速 | Collapse/Expand | 对称的折叠/展开运动 |
| `motionEaseOut` | `CubicEaseOut` | 适中加速，缓慢减速 | 通用出场 | 通用缓出 |
| `motionEaseOutBack` | `BackEaseOut` | 过冲后回弹 | 弹性效果 | "自然" — 模拟物理弹性 |
| `motionEaseInBack` | `BackEaseIn` | 回缩后加速 | 弹性回缩 | "自然" — 模拟物理回弹 |

### Ant Design 动效节奏原则

- **进入**（In）使用 **Ease Out** — 快速出现，缓慢到达最终位置，给用户"跃入"感。
- **退出**（Out）使用 **Ease In** — 缓慢启动后加速消失，给用户"收回"感。
- 这符合人类对物理运动的心理预期 — 物体从远处快速接近（进入），然后缓慢停下；物体开始缓慢移动然后加速远去（退出）。
- **不对称的进出场节奏**是 Ant Design "高效"原则的核心体现 — 出场比进场更快，因为用户关注的是新出现的内容而非消失的内容。

---

## 8. 各控件默认动效对照表

| 控件 | 打开动效 | 关闭动效 | Ant Design 动效类型 |
|------|---------|---------|-------------------|
| **Popup** | `ZoomBigInMotion` | `ZoomBigOutMotion` | `zoom-big` |
| **ContextMenu** | `ZoomBigInMotion` | `ZoomBigOutMotion` | `zoom-big` |
| **Select 下拉** | `ZoomBigInMotion` | `ZoomBigOutMotion` | `zoom-big` |
| **Tooltip** | `ZoomBigInMotion` | `ZoomBigOutMotion` | `zoom-big-fast` |
| **Drawer** | `MoveLeftIn/RightIn/UpIn/DownIn` | 对应 `*OutMotion` | `move-*` |
| **Message** | `MoveUpInMotion` | `MoveUpOutMotion` | `move-up` |
| **Notification** | 方向性 `NotificationMove*In/Out` | 同左 | `move-*` |
| **CollapseItem** | `SlideUpInMotion` | `SlideUpOutMotion` | `slide-up` |
| **Expander** | `ExpandMotion` | `CollapseMotion` | `ant-motion-collapse` |
| **TreeViewItem** | `ExpandMotion(Top)` | `CollapseMotion(Top)` | `ant-motion-collapse` |
| **NavMenuItem** | `SlideUpInMotion` | `SlideUpOutMotion` | `slide-up` |
| **FloatButtonGroup** | `MoveDown/Up/Left/RightIn` | 对应 `*OutMotion` | `move-*` |
