# Pagination Design Token

Pagination 控件使用 `PaginationToken`（Token ID: `"Pagination"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:PaginationTokenResource ItemBg}
{atom:PaginationTokenResource ItemSize}
{atom:PaginationTokenResource PaginationLayoutSpacing}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorText}
{atom:SharedTokenResource BorderRadius}
```

---

## 组件级 Token 一览

以下是 `PaginationToken` 定义的全部组件级 Token，按功能分组说明。

### 页码项颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemBg` | `Color` | `SharedToken.ColorBgContainer` | 页码选项背景色 |
| `ItemActiveBg` | `Color` | `SharedToken.ColorBgContainer` | 页码激活态（选中）背景色 |
| `ItemLinkBg` | `Color` | `SharedToken.ColorBgContainer` | 页码链接背景色 |
| `ItemActiveBgDisabled` | `Color` | `SharedToken.ControlItemBgActiveDisabled` | 页码激活态禁用状态背景色 |
| `ItemActiveColorDisabled` | `Color` | `SharedToken.ColorTextDisabled` | 页码激活态禁用状态文字颜色 |
| `ItemInputBg` | `Color` | `SharedToken.ColorBgContainer` | 输入框背景色 |

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemSize` | `double` | `SharedToken.ControlHeight` | 页码项标准尺寸（中/大号尺寸使用） |
| `ItemSizeSM` | `double` | `SharedToken.ControlHeightSM` | 小号页码项尺寸 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PaginationLayoutSpacing` | `double` | `SharedToken.UniformlyMarginXS` | 分页布局各区块之间的水平间距（标准/大号尺寸） |
| `PaginationLayoutMiniSpacing` | `double` | `SharedToken.UniformlyMarginXXS / 2` | 分页布局迷你间距（小号尺寸使用） |
| `PaginationItemPaddingInline` | `Thickness` | `SharedToken.UniformlyMarginXXS * 1.5` | 页码项横向内边距 |
| `InputOutlineOffset` | `Thickness` | `Thickness(0)` | 输入框轮廓偏移量 |

### 快速跳转 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PaginationQuickJumperInputWidth` | `double` | `SharedToken.ControlHeightLG * 1.25` | 快速跳转输入框宽度 |
| `PaginationMiniQuickJumperInputWidth` | `double` | `SharedToken.ControlHeightLG * 1.1` | 迷你快速跳转输入框宽度 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Pagination 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关，控制 `IsMotionEnabled` 默认值 |
| `BorderRadius` | 页码项圆角半径 |
| `BorderThickness` | 页码项边框厚度 |
| `FontSize` | 页码项字体大小 |
| `ColorPrimary` | 选中页码的前景色和边框色 |
| `ColorPrimaryHover` | 选中页码悬浮态的前景色和边框色 |
| `ColorText` | 未选中页码的文字颜色 |
| `ColorTextDisabled` | 禁用态文字和图标颜色 |
| `ColorTransparent` | 上一页/下一页按钮的默认背景和边框色 |
| `ColorBgTextHover` | 未选中页码悬浮态背景色 |
| `ColorBgTextActive` | 未选中页码按下态背景色 |
| `IconSize` / `IconSizeSM` / `IconSizeLG` | 不同尺寸下的页码项图标大小 |
| `UniformlyMarginXS` | 导航项之间的间距（中/大号尺寸） |

---

## Token 对外观的具体影响

### 页码项状态与 Token 映射

| 页码项状态 | 背景色 | 前景色 | 边框色 |
|---|---|---|---|
| **未选中-正常** | `ItemBg` | `ColorText` | `ColorTransparent` |
| **未选中-悬浮** | `ColorBgTextHover` | `ColorText` | — |
| **未选中-按下** | `ColorBgTextActive` | `ColorText` | — |
| **选中-正常** | `ItemActiveBg` | `ColorPrimary` | `ColorPrimary` |
| **选中-悬浮** | `ItemActiveBg` | `ColorPrimaryHover` | `ColorPrimaryHover` |
| **禁用-未选中** | — | `ItemActiveColorDisabled` | — |
| **禁用-选中** | `ItemActiveBgDisabled` | `ItemActiveColorDisabled` | 无边框 |

### 上一页/下一页按钮

| 状态 | 背景色 | 前景色 |
|---|---|---|
| **正常** | `ColorTransparent` | `ColorText` |
| **悬浮** | `ColorBgTextHover` | `ColorText` |
| **按下** | `ColorBgTextActive` | `ColorText` |
| **禁用** | — | `ColorTextDisabled`（图标） |

### 尺寸与 Token 映射

| 尺寸 | 页码项高度/最小宽度 | 图标尺寸 | 布局间距 |
|---|---|---|---|
| `Large` | `ItemSize` | `IconSizeLG` | `PaginationLayoutSpacing` |
| `Middle` | `ItemSize` | `IconSize` | `PaginationLayoutSpacing` |
| `Small` | `ItemSizeSM` | `IconSizeSM` | `PaginationLayoutMiniSpacing` |

### 快速跳转栏

- 输入框宽度由 `PaginationQuickJumperInputWidth` 控制
- 快速跳转栏各子项间距由 `PaginationLayoutSpacing` 控制
- "Go to" 和 "Page" 文本由本地化资源提供

### 禁用态

当 `IsEnabled=false` 时，整个分页组件的 `Foreground` 使用 `ItemActiveColorDisabled`，同时：
- 选中页码的边框消失（`BorderThickness = 0`）
- 选中页码的背景变为 `ItemActiveBgDisabled`
- 所有图标颜色变为 `ColorTextDisabled`
