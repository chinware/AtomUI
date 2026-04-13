# Splitter Design Token

Splitter 控件使用 `SplitterToken`（Token ID: `"Splitter"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:SplitterTokenResource SplitBarSize}
{atom:SplitterTokenResource HandleLineColor}
{atom:SplitterTokenResource SplitBarDraggableSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorFill}
{atom:SharedTokenResource BorderRadiusXS}
{atom:SharedTokenResource FontSizeSM}
```

---

## 组件级 Token 一览

以下是 `SplitterToken` 定义的全部组件级 Token，按功能分组说明。

### 尺寸 Token

| Token 名 | 类型 | 默认值 | 来源 | 说明 |
|---|---|---|---|---|
| `SplitBarDraggableSize` | `double` | `20` | 固定值 | 手柄握持区可拖拽指示器的尺寸（像素）。垂直分割时为高度，水平分割时为宽度 |
| `SplitBarSize` | `double` | `2` | 固定值 | 分割线的可见宽度（像素）。这是用户看到的细线的粗细 |
| `SplitTriggerSize` | `double` | `6` | 固定值 | 拖拽触发区域大小（像素）。大于 `SplitBarSize`，提供更大的拖拽热区以提升可用性 |
| `SplitBarHandleSize` | `double` | `SplitTriggerSize + SharedToken.FontSizeSM × 2` | 计算值 | 手柄整体占据的空间（像素），包括分割线和折叠按钮区域 |

### 折叠按钮位置 Token

| Token 名 | 类型 | 默认值 | 来源 | 说明 |
|---|---|---|---|---|
| `SplitBarCollapseOffset` | `double` | `SplitBarSize / 2 + 1` | 计算值 | 折叠按钮距分割线中心的正方向偏移（像素） |
| `SplitBarCollapseOffsetNegative` | `double` | `-(SharedToken.FontSizeSM + CollapseOffset)` | 计算值 | 折叠按钮距分割线中心的负方向偏移（像素） |
| `SplitBarCollapseCrossOffset` | `double` | `-SplitBarDraggableSize / 2` | 计算值 | 折叠按钮在交叉轴上的偏移（使按钮在手柄上居中） |

### 手柄分割线颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HandleLineColor` | `Color` | `SharedToken.ControlItemBgHover` | 手柄分割线的默认颜色 |
| `HandleLineHoverColor` | `Color` | `SharedToken.ControlItemBgActive` | 鼠标悬浮时的分割线颜色 |
| `HandleLineDragColor` | `Color` | `SharedToken.ControlItemBgActiveHover` | 拖拽中的分割线颜色 |
| `HandleLineThickness` | `double` | `SplitBarSize`（`2`） | 手柄分割线的粗细（像素） |

### 折叠图标 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HandleIconColor` | `Color` | `SharedToken.ColorText` | 折叠箭头图标的默认颜色 |
| `HandleIconHoverColor` | `Color` | `HandleIconColor` | 折叠箭头图标悬浮时的颜色 |
| `HandleIconPressedColor` | `Color` | `HandleIconColor` | 折叠箭头图标按下时的颜色 |
| `HandleIconSize` | `double` | `SharedToken.FontSizeSM` | 折叠箭头图标的尺寸（像素） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Splitter 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorFill` | 手柄握持区（`PART_HandleGrip`）的背景色 |
| `BorderRadiusXS` | 手柄握持区和折叠按钮的圆角半径 |
| `FontSizeSM` | 折叠按钮的尺寸基准（宽度/高度） |
| `ZIndexPopupBase` | 折叠图标容器和按钮的 ZIndex 层级 |
| `ControlItemBgHover` | 折叠按钮的默认背景色 |
| `ControlItemBgActive` | 折叠按钮悬浮态的背景色 |
| `ControlItemBgActiveHover` | 折叠按钮按下态的背景色 |

---

## Token 对外观的具体影响

### 手柄分割线状态变化

| 状态 | 分割线颜色 | 握持区可见性 |
|---|---|---|
| **正常** | `HandleLineColor`（来自 `ControlItemBgHover`） | 可见 |
| **鼠标悬浮** | `HandleLineHoverColor`（来自 `ControlItemBgActive`） | 可见 |
| **拖拽中** | `HandleLineDragColor`（来自 `ControlItemBgActiveHover`） | 可见 |
| **禁止拖拽** | `HandleLineColor`（固定，不响应交互） | 隐藏 |

### 折叠按钮状态变化

| 状态 | 按钮背景 | 图标颜色 |
|---|---|---|
| **正常** | `ControlItemBgHover` | `HandleIconColor` |
| **鼠标悬浮** | `ControlItemBgActive` | `HandleIconHoverColor` |
| **按下** | `ControlItemBgActiveHover` | `HandleIconPressedColor` |

### 尺寸关系图

```
│◄─── SplitBarHandleSize ───►│
│                              │
│  ┌── SplitTriggerSize ──┐   │
│  │   ┌ SplitBarSize ┐   │   │
│  │   │  ▓▓▓▓▓▓▓▓▓▓  │   │   │
│  │   └──────────────┘   │   │
│  └──────────────────────┘   │
│                              │
```

- **SplitBarSize（2px）**：用户可见的分割线粗细。
- **SplitTriggerSize（6px）**：实际的拖拽热区大小，比可见线更宽以提升易用性。
- **SplitBarHandleSize**：手柄整体占据的布局空间，包含折叠按钮。
- **SplitBarDraggableSize（20px）**：手柄握持区的尺寸（中央圆角矩形指示器）。

### 暗色主题

当 `isDarkMode = true` 时，所有颜色 Token 从 `SharedToken` 派生的值会自动适配暗色主题：
- `HandleLineColor` → 暗色主题下的 `ControlItemBgHover`
- `HandleIconColor` → 暗色主题下的 `ColorText`
- 其他颜色 Token 同理自动适配
