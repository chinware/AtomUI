# TreeView Design Token

TreeView 控件使用 `TreeViewToken`（Token ID: `"TreeView"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:TokenResource TreeViewHeaderHeight}
{atom:TokenResource TreeViewNodeHoverBg}
{atom:TokenResource TreeViewNodeSelectedBg}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeightSM}
{atom:SharedTokenResource ColorBorder}
```

---

## 组件级 Token 一览

以下是 `TreeViewToken` 定义的全部组件级 Token，按功能分组说明。

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderHeight` | `double` | `SharedToken.ControlHeightSM` | 节点标题行高度 |

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `NodeHoverBg` | `Color` | `SharedToken.ControlItemBgHover` | 节点悬浮态背景色 |
| `NodeSelectedBg` | `Color` | `SharedToken.ControlItemBgActive` | 节点选中态背景色 |
| `DirectoryNodeSelectedColor` | `Color` | `SharedToken.ColorTextLightSolid` | 目录树节点选中文字颜色 |
| `DirectoryNodeSelectedBg` | `Color` | `SharedToken.ColorPrimary` | 目录树节点选中背景色 |
| `FilterHighlightColor` | `Color` | `SharedToken.ColorError` | 搜索过滤高亮颜色（默认使用错误色以醒目标识） |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TreeItemMargin` | `Thickness` | `0, 0, 0, SharedToken.UniformlyPaddingXXS / 2` | 树节点的外边距（节点间的垂直间距） |
| `TreeItemHeaderPadding` | `Thickness` | `SharedToken.UniformlyPaddingXS, 0` | 树节点标题的内间距 |
| `TreeItemHeaderMargin` | `Thickness` | `SharedToken.UniformlyMarginXXS, 0, 0, 0` | 树节点标题的外间距 |
| `TreeNodeSwitcherMargin` | `Thickness` | `0, 0, SharedToken.UniformlyPaddingXS / 2, 0` | 展开/折叠按钮的外边距 |
| `TreeNodeIconMargin` | `Thickness` | `SharedToken.UniformlyPaddingXS / 2, 0, 0, 0` | 节点图标的外边距 |

### 拖拽 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DragIndicatorLineWidth` | `double` | `SharedToken.LineWidthFocus` | 拖拽指示线的线宽 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，TreeView 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ControlHeightSM` | 节点标题行高度 |
| `ControlItemBgHover` | 节点悬浮背景色 |
| `ControlItemBgActive` | 节点选中背景色 |
| `ColorPrimary` | 目录树选中背景色、主题强调色 |
| `ColorTextLightSolid` | 目录树选中文字颜色 |
| `ColorError` | 搜索过滤高亮颜色 |
| `ColorText` | 节点文字默认颜色 |
| `ColorTextDisabled` | 禁用节点文字颜色 |
| `ColorBorder` | 连接线颜色 |
| `BorderThickness` | 连接线粗细 |
| `UniformlyPaddingXS` | 基础内间距单位 |
| `UniformlyPaddingXXS` | 微小内间距单位 |
| `UniformlyMarginXXS` | 微小外边距单位 |
| `LineWidthFocus` | 拖拽指示线宽度 |
| `EnableMotion` | 全局动画开关 |
| `FontSize` | 节点文字大小 |
| `BorderRadius` | 节点圆角半径 |

---

## Token 对外观的具体影响

### 节点状态与 Token 映射

| 节点状态 | 文字颜色 | 背景色 | 边框 |
|---|---|---|---|
| **正常** | `ColorText` | 透明 | 无 |
| **悬浮** | `ColorText` | `NodeHoverBg` | 无 |
| **选中** | `ColorText` | `NodeSelectedBg` | 无 |
| **禁用** | `ColorTextDisabled` | 透明 | 无 |

### 悬浮模式与 Token 映射

| 悬浮模式 | 高亮范围 | 使用 Token |
|---|---|---|
| `Default` | 节点文字区域 | `NodeHoverBg`（仅标题区域） |
| `Block` | 整个节点行（含缩进） | `NodeHoverBg`（扩展到完整行） |
| `WholeLine` | 从容器左到右完整行 | `NodeHoverBg`（扩展到容器宽度） |

### 连接线与 Token 映射

| 元素 | 使用 Token | 说明 |
|---|---|---|
| 纵向连接线 | `BorderBrush`（`ColorBorder`） | 父子节点间的垂直连接线 |
| 横向连接线 | `BorderBrush`（`ColorBorder`） | 叶子节点的水平连接线 |
| 线条粗细 | `BorderThickness` | 连接线的像素宽度 |

### 拖拽指示器与 Token 映射

| 元素 | 使用 Token | 说明 |
|---|---|---|
| 指示线 | `DragIndicatorLineWidth` | 拖拽时显示的位置指示线宽度 |
| 指示线颜色 | `ColorPrimary` | 拖拽指示线使用主题色 |

### 间距与 Token 映射

| 元素 | Token | 影响 |
|---|---|---|
| 节点之间的垂直间距 | `TreeItemMargin` | 控制节点列表的紧凑程度 |
| 标题文字内间距 | `TreeItemHeaderPadding` | 控制标题左右留白 |
| 标题与 Switcher 间距 | `TreeItemHeaderMargin` | 控制展开按钮和标题之间的距离 |
| 展开按钮右侧间距 | `TreeNodeSwitcherMargin` | 控制展开按钮与后续内容的间距 |
| 图标左侧间距 | `TreeNodeIconMargin` | 控制图标与前方内容的间距 |
