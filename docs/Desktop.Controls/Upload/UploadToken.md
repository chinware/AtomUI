# Upload Design Token

Upload 控件使用 `UploadToken`（Token ID: `"Upload"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:UploadTokenResource ActionsColor}
{atom:UploadTokenResource PictureCardSize}
{atom:UploadTokenResource DragIconSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBorder}
{atom:SharedTokenResource BorderThickness}
```

---

## 组件级 Token 一览

以下是 `UploadToken` 定义的全部组件级 Token，按功能分组说明。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ActionsColor` | `Color` | `SharedToken.ColorIcon` | 操作按钮颜色（如删除、预览按钮的图标色） |

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PictureCardSize` | `double` | `SharedToken.ControlHeightLG * 2.55` | 卡片类型文件列表项的尺寸（PictureCard 和 PictureCircle 类型的卡片宽高） |
| `UploadThumbnailSize` | `double` | `SharedToken.FontSizeHeading2` | 文件类型图标缩略图大小（Picture 列表中的文件图标尺寸） |
| `PictureListPreviewerSize` | `double` | `SharedToken.SizeXXL` | 图片列表中图片预览器的大小 |

### 拖拽区域 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DragIconSize` | `double` | `SharedToken.FontSizeHeading3 * 2` | 拖拽上传区域图标大小 |
| `DragIconMargin` | `Thickness` | `(0, 0, 0, SharedToken.UniformlyMargin)` | 拖拽上传区域图标外间距（底部间距） |
| `DragHeaderMargin` | `Thickness` | `(0, 0, 0, SharedToken.UniformlyMarginXXS)` | 拖拽上传区域主标题的外间距（底部间距） |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TextListItemMargin` | `Thickness` | `(0, SharedToken.UniformlyMarginXS, 0, 0)` | 文本列表项的外间距（顶部间距） |
| `TextListNamePadding` | `Thickness` | `(SharedToken.UniformlyPaddingXS, 0)` | 文本列表中文件名的内间距（水平方向） |
| `PictureListItemMargin` | `Thickness` | `(0, SharedToken.UniformlyMarginXS, 0, 0)` | 图片列表项的外间距（顶部间距） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Upload 的 ControlTheme 及其子组件还直接引用了以下全局 `SharedToken`：

### Upload 主控件

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关 |
| `ColorTextDisabled` | 禁用态前景色 |

### UploadTriggerContent（触发区）

| Token 资源键 | 使用场景 |
|---|---|
| `ColorTextSecondary` | 触发区默认前景色 |
| `ColorFillAlter` | PictureCard/PictureCircle 触发区背景色 |
| `BorderRadiusLG` | PictureCard/PictureCircle 触发区圆角 |
| `ColorBorder` | PictureCard/PictureCircle 触发区边框颜色 |
| `BorderThickness` | PictureCard/PictureCircle 触发区边框厚度 |
| `ColorPrimary` | PictureCard/PictureCircle 触发区悬浮边框颜色 |
| `ColorTextDisabled` | 禁用态前景色 |

### UploadDefaultDropArea（拖拽区域）

| Token 资源键 | 使用场景 |
|---|---|
| `ColorFillAlter` | 拖拽区域背景色 |
| `BorderRadiusLG` | 拖拽区域圆角 |
| `ColorBorder` | 拖拽区域边框颜色 |
| `BorderThickness` | 拖拽区域边框厚度 |
| `Padding` | 拖拽区域内间距 |
| `EnableMotion` | 拖拽区域动画开关 |
| `ColorPrimary` | 拖拽区域图标颜色 |
| `ColorPrimaryHover` | 拖拽区域悬浮时边框颜色 |
| `ColorTextHeading` | 主标题文字颜色 |
| `FontSizeLG` | 主标题字体大小 |
| `ColorTextDescription` | 副标题文字颜色 |
| `FontSize` | 副标题字体大小 |
| `ColorTextDisabled` | 禁用态文字和图标颜色 |

---

## Token 对外观的具体影响

### 各列表类型的 Token 使用

| 列表类型 | 使用的组件级 Token | 说明 |
|---|---|---|
| **Text** | `TextListItemMargin`、`TextListNamePadding`、`ActionsColor`、`UploadThumbnailSize` | 文本列表项间距、文件名内间距、操作按钮颜色、文件图标大小 |
| **Picture** | `PictureListItemMargin`、`PictureListPreviewerSize`、`ActionsColor`、`UploadThumbnailSize` | 图片列表项间距、缩略图大小、操作按钮颜色 |
| **PictureCard** | `PictureCardSize`、`ActionsColor` | 卡片尺寸（宽高）、操作按钮颜色 |
| **PictureCircle** | `PictureCardSize`、`ActionsColor` | 圆形卡片尺寸（直径）、操作按钮颜色 |

### 拖拽区域的 Token 使用

| Token | 影响 |
|---|---|
| `DragIconSize` | 拖拽区域中心图标的宽高 |
| `DragIconMargin` | 图标与标题之间的间距 |
| `DragHeaderMargin` | 主标题与副标题之间的间距 |

### 卡片尺寸计算

`PictureCardSize` 的值为 `SharedToken.ControlHeightLG * 2.55`，这意味着：
- 默认主题下（`ControlHeightLG = 40`），卡片尺寸约为 `102px`
- 紧凑主题下卡片尺寸会相应缩小
- 此值同时控制 PictureCard 和 PictureCircle 两种类型的触发区和列表项尺寸

### Token 推导链示例

```
SharedToken.ColorIcon
  └── UploadToken.ActionsColor          → 操作按钮（删除/预览）图标颜色

SharedToken.ControlHeightLG
  └── UploadToken.PictureCardSize       → 卡片列表项和触发区的宽高

SharedToken.FontSizeHeading3
  └── UploadToken.DragIconSize          → 拖拽区域中心图标尺寸

SharedToken.UniformlyMarginXS
  ├── UploadToken.TextListItemMargin    → 文本列表项顶部间距
  └── UploadToken.PictureListItemMargin → 图片列表项顶部间距

SharedToken.UniformlyPaddingXS
  └── UploadToken.TextListNamePadding   → 文件名水平内间距

SharedToken.SizeXXL
  └── UploadToken.PictureListPreviewerSize → 图片列表缩略图预览尺寸
```
