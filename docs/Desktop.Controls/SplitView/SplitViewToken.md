# SplitView Design Token

SplitView 控件使用 `SplitViewToken`（Token ID: `"SplitView"`）作为组件级 Design Token。SplitView 的 Token 较为精简，主要控制面板尺寸和动画参数。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:SplitViewTokenResource OpenPaneThemeLength}
{atom:SplitViewTokenResource CompactPaneThemeLength}
{atom:SplitViewTokenResource PaneOpenMotionDuration}
{atom:SplitViewTokenResource PaneCloseMotionDuration}
{atom:SplitViewTokenResource PaneMotionEasing}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBgContainer}
{atom:SharedTokenResource ColorBorder}
{atom:SharedTokenResource ColorBgMask}
```

---

## 组件级 Token 一览

以下是 `SplitViewToken` 定义的全部组件级 Token。

### 尺寸 Token

| Token 名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `OpenPaneThemeLength` | `double` | `320` | 面板完全展开时的默认宽度/高度（像素），通过 ControlTheme 绑定到 `OpenPaneLength` 属性 |
| `CompactPaneThemeLength` | `double` | `48` | 紧凑模式下面板的默认宽度/高度（像素），通过 ControlTheme 绑定到 `CompactPaneLength` 属性 |

### 动画 Token

| Token 名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `PaneOpenMotionDuration` | `TimeSpan` | `200ms` | 面板展开动画时长 |
| `PaneCloseMotionDuration` | `TimeSpan` | `100ms` | 面板收起动画时长（比展开快，提供「快速响应关闭」的体验） |
| `PaneMotionEasing` | `Easing?` | `cubic-bezier(0.1, 0.9, 0.2, 1.0)` | 缓动曲线，控制动画的加速/减速节奏 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，SplitView 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorBgContainer` | 面板默认背景色（`PaneBackground` 属性的默认值） |
| `ColorBorder` | 面板分隔线颜色（`HCPaneBorder` 的 `Fill`） |
| `ColorBgMask` | 轻量关闭遮罩层颜色（`LightDismissLayer` 在 `:lightDismiss` 伪类激活时的 `Fill`） |

---

## Token 对外观的具体影响

### 尺寸 Token 映射

| Token | 映射属性 | 影响 |
|---|---|---|
| `OpenPaneThemeLength` | `OpenPaneLength` | 控制面板展开后的最终宽度（水平面板）或高度（垂直面板） |
| `CompactPaneThemeLength` | `CompactPaneLength` | 控制 CompactInline / CompactOverlay 模式下面板收起时保留的紧凑宽度/高度 |

### 动画 Token 映射

| Token | 映射属性 | 影响 |
|---|---|---|
| `PaneOpenMotionDuration` | `PaneOpenMotionDuration` | 展开动画的持续时长。值越大，展开越慢 |
| `PaneCloseMotionDuration` | `PaneCloseMotionDuration` | 收起动画的持续时长。默认比展开快，提供敏捷的关闭体验 |
| `PaneMotionEasing` | `PaneMotionEasing` | 缓动曲线。默认曲线 `(0.1, 0.9, 0.2, 1.0)` 实现「快启动、缓停止」效果 |

### 颜色 Token 映射

| SharedToken | 目标元素 | 影响 |
|---|---|---|
| `ColorBgContainer` | `Panel#PART_PaneRoot` 背景 | 面板背景色，跟随主题自动切换（亮色/暗色模式） |
| `ColorBorder` | `Rectangle#HCPaneBorder` 填充 | 面板边缘的 1px 分隔线颜色 |
| `ColorBgMask` | `Rectangle#LightDismissLayer` 填充 | Overlay 模式展开时的半透明遮罩色 |

### 动画行为说明

SplitView 的动画系统通过以下链路工作：

```
SplitViewToken (Token 定义)
  ↓ ControlTheme Setter 绑定
SplitView 属性 (PaneOpenMotionDuration, PaneCloseMotionDuration, PaneMotionEasing)
  ↓ ConfigureTransitions() 方法
PaneOpenTransitions / PaneCloseTransitions (内部 Transitions 集合)
  ↓ 伪类选择器绑定到 PART_PaneRoot
面板尺寸过渡动画 (Width 或 Height 的 DoubleTransition)
```

当 `IsMotionEnabled = false` 时，`PaneOpenTransitions` 和 `PaneCloseTransitions` 被设为 `null`，面板尺寸变化将不使用任何过渡动画。
