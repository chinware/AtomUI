# GroupBox Design Token

GroupBox 控件使用 `GroupBoxToken`（Token ID: `"GroupBox"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:GroupBoxTokenResource ContentPadding}
{atom:GroupBoxTokenResource HeaderContainerMargin}
{atom:GroupBoxTokenResource HeaderIconMargin}
{atom:GroupBoxTokenResource HeaderContentPadding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBgContainer}
{atom:SharedTokenResource ColorBorder}
{atom:SharedTokenResource BorderThickness}
{atom:SharedTokenResource BorderRadius}
{atom:SharedTokenResource ColorText}
{atom:SharedTokenResource FontSize}
```

---

## 组件级 Token 一览

以下是 `GroupBoxToken` 定义的全部组件级 Token。

### 间距与布局 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TextPaddingInline` | `double` | 固定 `1.0`（单位 em） | 标题文本的横向内间距，以 em 为单位 |
| `OrientationMarginPercent` | `double` | 固定 `0.05` | 标题与边缘距离的比例，取值 0～1 |
| `VerticalMarginInline` | `double` | `SharedToken.UniformlyMarginXS` | 纵向分割线的横向外间距 |
| `ContentPadding` | `Thickness` | `SharedToken.PaddingXS` | 内容区域内间距（Content 与边框之间的间距） |
| `HeaderContainerMargin` | `Thickness` | `Margin(SharedToken.UniformlyMargin, SharedToken.UniformlyMarginXS, SharedToken.UniformlyMargin, 0)` | 标题区域容器（`PART_HeaderContainer`）的外间距 |
| `HeaderContentPadding` | `Thickness` | `Thickness(SharedToken.UniformlyPaddingXXS, 0)` | 标题内容（`PART_HeaderContent`）的内间距，控制标题文本两侧的留白 |
| `HeaderIconMargin` | `Thickness` | `Thickness(0, 0, SharedToken.UniformlyMarginXXS, 0)` | 标题图标的外间距（控制图标与文本之间的间距） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，GroupBox 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 | 说明 |
|---|---|---|
| `ColorBgContainer` | `Background` 属性默认值 | 分组框背景色，同时用于标题遮挡区域的填充色 |
| `ColorBorder` | `BorderBrush` 属性默认值 | 边框颜色 |
| `BorderThickness` | `BorderThickness` 属性默认值 | 边框粗细 |
| `BorderRadius` | `CornerRadius` 属性默认值 | 边框圆角半径 |
| `ColorText` | `HeaderTitleColor` 属性默认值 | 标题文本颜色 |
| `FontSize` | `HeaderFontSize` 属性默认值 | 标题字体大小 |
| `IconSizeLG` | 标题图标的 `Width` / `Height` | 标题图标尺寸（使用大号图标尺寸） |

---

## Token 对外观的具体影响

### 整体视觉

GroupBox 的视觉外观由以下 Token 组合决定：

```
┌─────── HeaderContainerMargin ─────────────────────────────┐
│  ┌── HeaderContentPadding ──┐                             │
│  │ [Icon]  HeaderTitle Text │                             │
│  └──────────────────────────┘                             │
│  ↑ HeaderIconMargin                                       │
├─── Border (ColorBorder, BorderThickness, BorderRadius) ───┤
│                                                           │
│  ┌─────── ContentPadding ──────────┐                      │
│  │                                 │                      │
│  │     Content Area                │                      │
│  │                                 │                      │
│  └─────────────────────────────────┘                      │
│                                                           │
└───────────────────────────────────────────────────────────┘
     Background = ColorBgContainer
```

### Token 与属性的映射关系

| 视觉元素 | 控制 Token | 影响说明 |
|---|---|---|
| 背景色 | `SharedToken.ColorBgContainer` | 整个分组框的背景色，包括标题遮挡区域 |
| 边框色 | `SharedToken.ColorBorder` | 分组框的边框线颜色 |
| 边框粗细 | `SharedToken.BorderThickness` | 边框线的宽度 |
| 圆角 | `SharedToken.BorderRadius` | 边框的圆角半径 |
| 标题文本色 | `SharedToken.ColorText` | 标题文本的默认颜色 |
| 标题字号 | `SharedToken.FontSize` | 标题文本的默认大小 |
| 标题图标大小 | `SharedToken.IconSizeLG` | 标题图标的宽高 |
| 标题图标色 | `SharedToken.ColorText` | 标题图标的填充色（`IconBrush`） |
| 内容区域间距 | `GroupBoxToken.ContentPadding` | Content 与边框之间的留白 |
| 标题区域间距 | `GroupBoxToken.HeaderContainerMargin` | 标题容器距离边框的外间距 |
| 标题内间距 | `GroupBoxToken.HeaderContentPadding` | 标题文本两侧的留白（影响遮挡区域宽度） |
| 图标间距 | `GroupBoxToken.HeaderIconMargin` | 图标与标题文本之间的间距 |

### 主题切换的影响

当在 Light / Dark 主题间切换时：
- `ColorBgContainer` 自动切换为对应主题的容器背景色
- `ColorBorder` 自动切换为对应主题的边框色
- `ColorText` 自动切换为对应主题的文本色
- 标题遮挡区域的填充色随 `Background`（即 `ColorBgContainer`）自动适配

所有这些切换由 Token 系统自动完成，无需手动处理。
