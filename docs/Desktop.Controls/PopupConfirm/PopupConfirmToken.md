# PopupConfirm Design Token

PopupConfirm 控件使用 `PopupConfirmToken`（Token ID: `"PopupConfirm"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:PopupConfirmTokenResource PopupMinWidth}
{atom:PopupConfirmTokenResource ButtonSpacing}
{atom:PopupConfirmTokenResource IconMargin}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorWarning}
{atom:SharedTokenResource ColorError}
{atom:SharedTokenResource IconSizeLG}
```

---

## 组件级 Token 一览

以下是 `PopupConfirmToken` 定义的全部组件级 Token。

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PopupMinWidth` | `double` | 固定 `240` | 气泡确认框弹出层最小宽度 |
| `PopupMinHeight` | `double` | 固定 `80` | 气泡确认框弹出层最小高度 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ButtonSpacing` | `double` | `SharedToken.UniformlyMarginXS` | 确认按钮与取消按钮之间的水平间距 |
| `IconMargin` | `Thickness` | `(0, UniformlyMarginXS/2, UniformlyMarginXS, 0)` | 图标的外边距（上方留半间距，右侧留间距） |
| `ContentContainerMargin` | `Thickness` | `(0, 0, 0, UniformlyMarginXS)` | 描述内容区域的外边距（底部留间距） |
| `ButtonContainerMargin` | `Thickness` | `(0, UniformlyMarginXS, 0, 0)` | 按钮区域的外边距（顶部留间距） |
| `TitleMargin` | `Thickness` | `(0, 0, 0, UniformlyMarginXS)` | 标题文本的外边距（底部留间距） |

---

## 关联的基类 Token

PopupConfirm 通过继承 `FlyoutHost` 还间接使用以下 Token：

### FlyoutHostToken（Token ID: `"FlyoutHost"`）

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `MarginToAnchor` | `double` | `SharedToken.UniformlyMarginXXS` | 弹出层与触发元素之间的默认间距 |

### PopupHostToken（Token ID: `"PopupHost"`）

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `BorderRadius` | `CornerRadius` | `SharedToken.BorderRadiusLG` | 弹出层圆角半径 |
| `BoxShadows` | `BoxShadows` | `SharedToken.BoxShadowsSecondary` | 弹出层阴影效果 |
| `MarginToAnchor` | `double` | `SharedToken.UniformlyMarginXXS` | 弹出层与锚点之间的间距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，PopupConfirm 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorTextHeading` | 标题文本颜色（`TextBlock#PART_Title` 的 `Foreground`） |
| `IconSizeLG` | 图标展示器的宽度和高度 |
| `ColorPrimary` | `ConfirmStatus=Info` 时图标颜色（蓝色） |
| `ColorWarning` | `ConfirmStatus=Warning` 时图标颜色（橙色） |
| `ColorError` | `ConfirmStatus=Error` 时图标颜色（红色） |
| `EnableMotion` | 全局动画开关 |
| `MotionDurationMid` | 弹出/收起动画持续时间 |

---

## Token 对外观的具体影响

### 确认状态与图标颜色映射

| ConfirmStatus | 图标颜色 Token | 视觉效果 |
|---|---|---|
| `Info` | `SharedToken.ColorPrimary` | 蓝色图标，适合信息性确认 |
| `Warning`（默认） | `SharedToken.ColorWarning` | 橙色图标，适合一般性警告确认 |
| `Error` | `SharedToken.ColorError` | 红色图标，适合高风险操作确认 |

### 空内容状态下的 Token 行为

当 `ConfirmContent` 为 `null` 时（`:empty-content` 伪类激活），`ButtonContainerMargin` 被重置为 `0`，消除按钮区域与标题之间的额外间距，使仅有标题的气泡更加紧凑。

### 间距 Token 在布局中的位置

```
┌─────────────────────────────────────┐
│  ┌──────────┐  Title                │
│  │   Icon   │  ← TitleMargin ↓     │
│  │ IconMargin│  Content             │
│  └──────────┘  ← ContentContainerMargin ↓
│                                     │
│  ← ButtonContainerMargin ↑          │
│              [Cancel] [Ok]          │
│               ← ButtonSpacing →     │
└─────────────────────────────────────┘
```
