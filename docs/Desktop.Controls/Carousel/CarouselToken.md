# Carousel Design Token

Carousel 控件使用 `CarouselToken`（Token ID: `"Carousel"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:CarouselTokenResource IndicatorWidth}
{atom:CarouselTokenResource IndicatorActiveWidth}
{atom:CarouselTokenResource ArrowSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBgContainer}
{atom:SharedTokenResource EnableMotion}
{atom:SharedTokenResource MotionDurationSlow}
```

---

## 组件级 Token 一览

以下是 `CarouselToken` 定义的全部组件级 Token。

### 指示器 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `IndicatorWidth` | `double` | 固定 `16` | 指示点默认宽度 |
| `IndicatorHeight` | `double` | 固定 `3` | 指示点高度 |
| `IndicatorActiveWidth` | `double` | 固定 `24` | 选中态指示点宽度（变宽以突出当前页） |
| `IndicatorGap` | `double` | `SharedToken.UniformlyMarginXS` | 指示点之间的间距 |
| `PaginationOffset` | `double` | 固定 `12` | 分页指示器距走马灯边缘的距离 |

### 导航箭头 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ArrowSize` | `double` | 固定 `16` | 导航箭头图标大小 |
| `ArrowOffset` | `double` | `SharedToken.SpacingXS` | 导航箭头距走马灯边缘的距离 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Carousel 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关，控制指示点宽度过渡和导航按钮不透明度过渡 |
| `MotionDurationSlow` | 页面过渡动画时长（默认值） |
| `ColorBgContainer` | 指示点背景色、导航箭头图标颜色 |
| `UniformlyMarginXS` | 指示点间距 |
| `SpacingXS` | 导航箭头边距 |

---

## Token 对外观的具体影响

### 指示点样式

| 状态 | 宽度 | 不透明度 | 进度条 |
|---|---|---|---|
| 正常 | `IndicatorWidth`（16px） | `0.2` | 不显示 |
| 悬浮（`:pointerover`） | `IndicatorWidth`（16px） | `0.75` | 不显示 |
| 选中（无进度条） | `IndicatorActiveWidth`（24px） | `1.0` | 不显示 |
| 选中（有进度条） | `IndicatorActiveWidth`（24px） | `0.2`（底层） | 显示（覆盖层按进度填充） |
| 选中 + 悬浮（有进度条） | `IndicatorActiveWidth`（24px） | `0.75` | 隐藏 |

### 导航箭头样式

| 状态 | 不透明度 | 可见性 |
|---|---|---|
| 正常 | `0.2` | 根据 `IsShowNavButtons` + 边界条件 |
| 悬浮（`:pointerover`） | `1.0` | — |
| 非无限循环 + 首页 | — | 前一页按钮隐藏 |
| 非无限循环 + 末页 | — | 下一页按钮隐藏 |

### 分页位置与布局 Token

| 位置 | 指示器对齐 | 指示器 Margin | 导航按钮 |
|---|---|---|---|
| `Bottom` | 底部水平居中 | `Bottom = PaginationOffset` | 左右两侧垂直居中 |
| `Top` | 顶部水平居中 | `Top = PaginationOffset` | 左右两侧垂直居中 |
| `Left` | 左侧垂直居中（旋转 90°） | `Left = PaginationOffset` | 上下两侧水平居中（旋转 90°） |
| `Right` | 右侧垂直居中（旋转 90°） | `Right = PaginationOffset` | 上下两侧水平居中（旋转 90°） |

### 过渡动画参数

| 参数 | Token / 默认值 | 说明 |
|---|---|---|
| `PageTransitionDuration` | `SharedToken.MotionDurationSlow` | 页面切换动画时长 |
| `PageInEasing` | `CubicEaseOut` | 页面进入缓动 |
| `PageOutEasing` | `CubicEaseOut` | 页面离开缓动 |
| 指示点宽度过渡 | `DoubleTransition`（默认时长） | 选中/取消选中时宽度变化 |
| 指示点不透明度过渡 | `DoubleTransition`（默认时长） | 悬浮/离开时不透明度变化 |
| 导航按钮不透明度 | `DoubleTransition`（默认时长） | 悬浮/离开时不透明度变化 |
