# Drawer Design Token

Drawer 控件使用 `DrawerToken`（Token ID: `"Drawer"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:TokenResource DrawerSmallSize}
{atom:TokenResource DrawerMiddleSize}
{atom:TokenResource DrawerContentPadding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBgElevated}
{atom:SharedTokenResource ColorBgMask}
{atom:SharedTokenResource PaddingLG}
```

---

## 组件级 Token 一览

以下是 `DrawerToken` 定义的全部组件级 Token，按功能分组说明。

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `SmallSize` | `Dimension` | 固定 `378px` | 小号抽屉宽/高度 |
| `MiddleSize` | `Dimension` | 固定 `520px` | 中号抽屉宽/高度 |
| `LargeSize` | `Dimension` | 固定 `736px` | 大号抽屉宽/高度 |

### 阴影 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `BoxShadowDrawerLeft` | `BoxShadows` | 自定义 3 层阴影 | 从左侧滑入时的阴影效果（向右投射） |
| `BoxShadowDrawerRight` | `BoxShadows` | 自定义 3 层阴影 | 从右侧滑入时的阴影效果（向左投射） |
| `BoxShadowDrawerUp` | `BoxShadows` | 自定义 3 层阴影 | 从顶部滑入时的阴影效果（向下投射） |
| `BoxShadowDrawerDown` | `BoxShadows` | 自定义 3 层阴影 | 从底部滑入时的阴影效果（向上投射） |

每个方向的阴影均由 3 层组成：
- 主阴影层：offset 6px, blur 16px, opacity 8%
- 内阴影层：offset 3px, blur 6px, spread -4px, opacity 12%
- 外阴影层：offset 9px, blur 28px, spread 8px, opacity 5%

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderMargin` | `Thickness` | `SharedToken.UniformlyMarginLG`, `SharedToken.UniformlyMargin` | 标题区域外间距 |
| `FooterPadding` | `Thickness` | `SharedToken.UniformlyPadding`, `SharedToken.UniformlyPaddingXS` | 底部区域内间距 |
| `ContentPadding` | `Thickness` | `SharedToken.PaddingLG` | 内容区域内间距 |
| `CloseIconPadding` | `Thickness` | `SharedToken.PaddingXXS` | 关闭按钮内间距 |
| `CloseIconMargin` | `Thickness` | 基于 `SharedToken.UniformlyMarginXS` | 关闭按钮外间距 |

### 行为 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PushOffsetPercent` | `double` | 固定 `0.4`（40%） | 多级抽屉 Push 偏移比率 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Drawer 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorBgElevated` | 抽屉面板背景色 |
| `ColorBgMask` | 遮罩层背景色 |
| `PaddingLG` | 内容区域内间距 |
| `PaddingXXS` | 关闭按钮内间距 |
| `UniformlyMarginLG` / `UniformlyMargin` | 标题区域间距 |
| `UniformlyPadding` / `UniformlyPaddingXS` | 底部区域间距 |
| `UniformlyMarginXS` | 关闭按钮外间距 |
| `ColorText` | 标题文字颜色 |
| `FontSizeLG` | 标题字号 |
| `EnableMotion` | 全局动画开关 |

---

## Token 对外观的具体影响

### 尺寸控制

| SizeType | 使用的 Token | 默认值 |
|---|---|---|
| `Small` | `SmallSize` | 378px |
| `Middle` | `MiddleSize` | 520px |
| `Large` | `LargeSize` | 736px |
| `Custom` | 用户设置的 `DialogSize` | — |

当 `Placement` 为 Left / Right 时，尺寸控制抽屉宽度；当 `Placement` 为 Top / Bottom 时，尺寸控制抽屉高度。

### 阴影方向

阴影方向自动跟随 `Placement`：
- `Left` → `BoxShadowDrawerLeft`（向右投射）
- `Right` → `BoxShadowDrawerRight`（向左投射）
- `Top` → `BoxShadowDrawerUp`（向下投射）
- `Bottom` → `BoxShadowDrawerDown`（向上投射）

### 多级抽屉推移

当子抽屉打开时，父抽屉推移距离 = 子抽屉尺寸 × `PushOffsetPercent`。默认 40% 意味着如果子抽屉宽 378px，父抽屉会推开约 151px。
