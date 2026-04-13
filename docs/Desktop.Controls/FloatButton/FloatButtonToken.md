# FloatButton Design Token

FloatButton 使用 `FloatButtonToken`（Token ID: `"FloatButton"`）作为组件级 Design Token。所有 Token 值均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:FloatButtonTokenResource FloatButtonSize}
{atom:FloatButtonTokenResource FloatButtonIconSize}
{atom:FloatButtonTokenResource PrimaryColor}
{atom:FloatButtonTokenResource FloatOffsetX}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBgContainer}
{atom:SharedTokenResource ColorText}
```

---

## 组件级 Token 一览

以下是 `FloatButtonToken` 定义的全部组件级 Token。

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `FloatButtonSize` | `double` | `SharedToken.ControlHeightLG` | 悬浮按钮整体尺寸（宽高相等） |
| `FloatButtonIconSize` | `double` | `SharedToken.FontSizeIcon × 1.5` | 按钮内图标尺寸 |
| `DescriptionLineHeight` | `double` | `SharedToken.FontSizeSM × 1.2` | Square 形状描述文本的行高 |

### 定位 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `FloatOffsetX` | `double` | `SharedToken.UniformlyMargin` | 默认水平偏移量 |
| `FloatOffsetY` | `double` | `SharedToken.UniformlyMargin` | 默认垂直偏移量 |

### 徽标偏移 Token

| Token 名 | 类型 | 来源计算公式 | 说明 |
|---|---|---|---|
| `SquareBadgeOffset` | `double` | `BorderRadius × (√2 - 1) / √2` | 方形按钮徽标偏移 |
| `CircleBadgeOffset` | `double` | `ControlHeight / 2 × (√2 - 1) / √2` | 圆形按钮徽标偏移 |

> 徽标偏移量基于几何计算：对于圆角矩形或圆形，徽标需要偏移到圆弧切线位置才能视觉上贴合角落。公式 `r × (√2 - 1) / √2` 正是将偏移定位到 45° 切线处。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PrimaryColor` | `Color` | `SharedToken.ColorTextLightSolid` | Primary 类型按钮的文本/图标颜色 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，FloatButton 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### 颜色 Token

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPrimary` | Primary 类型按钮背景色 |
| `ColorPrimaryHover` | Primary 类型按钮悬浮背景色 |
| `ColorBgContainer` | Default 类型按钮背景色 |
| `ColorText` | Default 类型图标颜色 |
| `ColorTextLightSolid` | Primary 类型文本/图标色（通过 `PrimaryColor` Token 间接引用） |
| `ColorTextDescription` | Square 形状描述文本颜色 |

### 尺寸 / 布局 Token

| Token 资源键 | 使用场景 |
|---|---|
| `ControlHeightLG` | 按钮尺寸基础值 |
| `FontSizeIcon` | 图标字号基础值 |
| `FontSizeSM` | 描述文本字号 |
| `BorderRadius` | 方形按钮圆角 |
| `UniformlyMargin` | 默认偏移量基础值 |

### 其他 Token

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关 |
| `MotionDurationMid` | 背景色过渡动画时长 |
| `BoxShadowSecondary` | 按钮阴影 |

---

## Token 对外观的具体影响

### 按钮类型与颜色映射

| 按钮类型 | 背景色 | 悬浮背景色 | 图标/文本色 |
|---|---|---|---|
| `Default` | `ColorBgContainer` | 稍浅灰色 | `ColorText` |
| `Primary` | `ColorPrimary` | `ColorPrimaryHover` | `PrimaryColor`（`ColorTextLightSolid`） |

### 形状与圆角映射

| 形状 | 圆角计算方式 |
|---|---|
| `Circle` | `Height / 2`（运行时动态计算，确保完全圆形） |
| `Square` | `SharedToken.BorderRadius` |

### 徽标偏移计算

徽标位置根据按钮形状使用不同的偏移值：

| 形状 | 偏移 Token | 位置效果 |
|---|---|---|
| `Circle` | `CircleBadgeOffset` | 徽标贴合圆形边缘 |
| `Square` | `SquareBadgeOffset` | 徽标贴合圆角矩形角落 |
