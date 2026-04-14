# Breadcrumb Design Token

Breadcrumb 控件使用 `BreadcrumbToken`（Token ID: `"Breadcrumb"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:BreadcrumbTokenResource ItemColor}
{atom:BreadcrumbTokenResource LinkColor}
{atom:BreadcrumbTokenResource SeparatorColor}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource BorderRadius}
{atom:SharedTokenResource IconSizeSM}
```

---

## 组件级 Token 一览

以下是 `BreadcrumbToken` 定义的全部组件级 Token，按功能分组说明。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemColor` | `Color` | `SharedToken.ColorTextDescription` | 面包屑项默认文字颜色（描述色，较浅） |
| `LastItemColor` | `Color` | `SharedToken.ColorText` | 最后一项文字颜色（主文字色，较深），表示当前页面 |
| `LinkColor` | `Color` | `SharedToken.ColorTextDescription` | 可导航面包屑项的链接文字颜色 |
| `LinkHoverColor` | `Color` | `SharedToken.ColorText` | 可导航面包屑项悬浮时的文字颜色 |
| `LinkHoverBgColor` | `Color` | `SharedToken.ColorBgTextHover` | 可导航面包屑项悬浮时的背景颜色 |
| `SeparatorColor` | `Color` | `SharedToken.ColorTextDescription` | 分隔符文字颜色 |

### 图标 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `IconSize` | `double` | `SharedToken.IconSize` | 面包屑项图标大小 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `BreadcrumbItemContentPadding` | `Thickness` | 基于 `SharedToken.UniformlyPaddingXXS` | 面包屑项内容区域的内间距（上下左右均为 `UniformlyPaddingXXS`） |
| `SeparatorMargin` | `Thickness` | 基于 `SharedToken.UniformlyMarginXXS` | 分隔符的左右外间距（`UniformlyMarginXXS, 0`） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Breadcrumb 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关，控制 `IsMotionEnabled` 默认值 |
| `BorderRadius` | BreadcrumbItem 内容框架圆角半径 |
| `SpacingXXS` | 图标与文字之间的间距 |
| `IconSizeSM` | BreadcrumbItem 模板中 `IconPresenter` 的宽高 |

---

## Token 对外观的具体影响

### 面包屑项状态与 Token 映射

| 状态 | 文字颜色 | 背景颜色 | 光标 |
|---|---|---|---|
| **普通项（非最后、不可导航）** | `ItemColor` | `Transparent` | 默认 |
| **最后一项（不可导航）** | `LastItemColor` | `Transparent` | 默认 |
| **可导航项（正常态）** | `LinkColor` | `Transparent` | `Hand` |
| **可导航项（悬浮态）** | `LinkHoverColor` | `LinkHoverBgColor` | `Hand` |

### 分隔符样式

| 属性 | Token |
|---|---|
| 文字颜色 | `SeparatorColor` |
| 左右间距 | `SeparatorMargin` |
| 可见性 | 最后一项自动隐藏 |

### 图标样式

| 状态 | 图标颜色 | 图标大小 |
|---|---|---|
| 普通状态 | `ItemColor` | `SharedToken.IconSizeSM` |
| 可导航悬浮态 | `LinkHoverColor` | `SharedToken.IconSizeSM` |
