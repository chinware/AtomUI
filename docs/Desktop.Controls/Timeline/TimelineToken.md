# Timeline Design Token

Timeline 控件使用 `TimelineToken`（Token ID: `"Timeline"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:TimelineTokenResource IndicatorTailColor}
{atom:TimelineTokenResource IndicatorTailWidth}
{atom:TimelineTokenResource ItemPaddingBottom}
{atom:TimelineTokenResource IndicatorSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBorder}
{atom:SharedTokenResource ColorBgContainer}
{atom:SharedTokenResource RelativeLineHeight}
```

---

## 组件级 Token 一览

以下是 `TimelineToken` 定义的全部组件级 Token。

### 尾线 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `IndicatorTailColor` | `Color` | `SharedToken.ColorSplit` | 连接相邻节点的尾线颜色 |
| `IndicatorTailWidth` | `double` | `SharedToken.LineWidthBold` | 尾线宽度 |

### 指示器 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `IndicatorSize` | `double` | `SharedToken.SizeMS` | 指示器区域的整体尺寸 |
| `IndicatorDotSize` | `double` | 固定 `8` | 指示器内置空心圆点的直径 |
| `IndicatorDotBorderWidth` | `double` | `SharedToken.LineWidth * 3` | 空心圆点的边框宽度 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemPaddingBottom` | `Thickness` | `Thickness(0, 0, 0, SharedToken.UniformlyPadding * 1.25)` | 节点内容的下方内间距 |
| `ItemPaddingBottomLG` | `Thickness` | `ItemPaddingBottom * 2` | 待办节点前节点的加大下方内间距 |
| `LastItemContentMinHeight` | `double` | `SharedToken.ControlHeightLG * 1.2` | 最后一个节点内容区域的最小高度 |

### 指示器边距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `IndicatorLeftModeMargin` | `Thickness` | `Thickness(0, 0, SharedToken.UniformlyMargin, 0)` | 左对齐模式下指示器的外边距（右侧） |
| `IndicatorRightModeMargin` | `Thickness` | `Thickness(SharedToken.UniformlyMargin, 0, 0, 0)` | 右对齐模式下指示器的外边距（左侧） |
| `IndicatorMiddleModeMargin` | `Thickness` | `Thickness(SharedToken.UniformlyMargin, 0)` | 居中模式（Alternate/Label 布局）下指示器的外边距（两侧） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Timeline 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPrimary` | 默认指示器颜色（空心圆点和图标的默认填充色） |
| `ColorBorder` | Timeline 容器边框颜色 |
| `ColorBgContainer` | Timeline 容器背景色 |
| `RelativeLineHeight` | 指示器最小高度的行高比例因子 |
| `IconSizeXS` | （通过 `IndicatorSize` 间接使用）小图标尺寸参考 |

---

## Token 对外观的具体影响

### 指示器渲染

| 元素 | Token 映射 |
|---|---|
| **空心圆点** | 边框色 = `IndicatorColor`（默认 `ColorPrimary`），边框宽 = `IndicatorDotBorderWidth`，直径 = `IndicatorDotSize` |
| **自定义图标** | 填充色 = `IndicatorColor`（默认 `ColorPrimary`），尺寸 = `IndicatorSize` × `IndicatorSize` |
| **尾线** | 颜色 = `IndicatorTailColor`，宽度 = `IndicatorTailWidth` |

### 节点间距

| 场景 | 间距 Token |
|---|---|
| 普通节点 | `ItemPaddingBottom` |
| 待办节点前的节点（非 Reverse） | `ItemPaddingBottomLG`（加大间距，与幽灵节点拉开距离） |
| 待办节点自身（Reverse 下） | `ItemPaddingBottomLG`（Reverse 模式下待办节点自身加大间距） |

### 布局模式与指示器边距

| 布局模式 | 指示器边距 Token |
|---|---|
| Left（无标签） | `IndicatorLeftModeMargin`（指示器右侧留间距） |
| Right（无标签） | `IndicatorRightModeMargin`（指示器左侧留间距） |
| Alternate / 标签布局 | `IndicatorMiddleModeMargin`（指示器两侧留间距） |
