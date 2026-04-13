# QRCode Design Token

QRCode 控件使用 `QRCodeToken`（Token ID: `"QRCode"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:QRCodeTokenResource QRCodeTextColor}
{atom:QRCodeTokenResource QRCodeMaskBackgroundColor}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBgContainer}
{atom:SharedTokenResource ColorSplit}
{atom:SharedTokenResource BorderThickness}
{atom:SharedTokenResource BorderRadiusLG}
{atom:SharedTokenResource PaddingSM}
```

---

## 组件级 Token 一览

以下是 `QRCodeToken` 定义的全部组件级 Token。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `QRCodeTextColor` | `Color` | `SharedToken.ColorText` | 二维码状态文案的文字颜色（用于 "已过期"、"已扫描"等文案） |
| `QRCodeMaskBackgroundColor` | `Color` | 基于 `SharedToken.ColorBgContainer`，Alpha 通道设为 `244` | 非 Active 状态下遮罩层的半透明背景色，覆盖在原始二维码图像上方 |

### Token 计算逻辑

```csharp
public override void CalculateTokenValues(bool isDarkMode)
{
    base.CalculateTokenValues(isDarkMode);
    var colorBgContainer = SharedToken.ColorBgContainer;
    
    // 文字颜色直接使用全局文本色
    QRCodeTextColor = SharedToken.ColorText;
    
    // 遮罩背景色：使用容器背景色，但设置高 Alpha 值实现半透明效果
    // Alpha = 244 (约 95.7% 不透明)，既遮挡二维码又不完全遮盖
    QRCodeMaskBackgroundColor = Color.FromArgb(244, 
        colorBgContainer.R, 
        colorBgContainer.G, 
        colorBgContainer.B);
}
```

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，QRCode 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 | 说明 |
|---|---|---|
| `ColorBgContainer` | 图标背景色 `IconBgColor` 的默认值 | 中心图标区域的背景色 |
| `ColorSplit` | `IsBordered=True` 时的边框颜色 | 有边框模式下的分割线色 |
| `BorderThickness` | 边框粗细 | 控件默认边框厚度 |
| `BorderRadiusLG` | 圆角半径 | 控件外框的大圆角半径 |
| `PaddingSM` | `IsBordered=True` 时的内间距 | 有边框模式下二维码与边框之间的间距 |
| `PaddingXXS` | 中心图标框的内间距 | 图标与图标框边缘之间的微小间距 |
| `ColorText` | 前景色（通过 `QRCodeTextColor`） | 状态文案的文字颜色 |

---

## Token 对外观的具体影响

### 状态与 Token 映射

| 状态 | 遮罩可见性 | 使用的 Token |
|---|---|---|
| **Active** | 遮罩隐藏 | — |
| **Loading** | 遮罩可见 | `QRCodeMaskBackgroundColor`（遮罩背景） |
| **Expired** | 遮罩可见 | `QRCodeMaskBackgroundColor`（遮罩背景）、`QRCodeTextColor`（"二维码过期"文案） |
| **Scanned** | 遮罩可见 | `QRCodeMaskBackgroundColor`（遮罩背景）、`QRCodeTextColor`（"已扫描"文案） |

### 边框模式与 Token 映射

| 模式 | 边框颜色 | 内间距 | 圆角 |
|---|---|---|---|
| `IsBordered=True` | `ColorSplit` | `PaddingSM` | `BorderRadiusLG` |
| `IsBordered=False` | `Transparent` | `0` | `BorderRadiusLG` |

### 主题切换影响

由于所有 Token 值均从 `SharedToken` 派生：
- **暗色模式**：`ColorBgContainer` 变为深色，`QRCodeMaskBackgroundColor` 的 RGB 分量随之变化，遮罩层自动适配暗色背景；`ColorText` 变为浅色文字。
- **紧凑模式**：`PaddingSM` 缩小，有边框模式下的内间距相应减小；`BorderRadiusLG` 缩小，圆角变化。
- **自定义主题**：修改 Seed Token（如 `ColorPrimary`）不会直接影响 QRCode 的 Token，因为 QRCode 主要使用中性色系 Token（`ColorBgContainer`、`ColorText`、`ColorSplit`）。
