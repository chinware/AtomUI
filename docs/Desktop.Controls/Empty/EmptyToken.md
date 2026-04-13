# Empty Design Token

Empty 控件使用 `EmptyToken`（Token ID: `"Empty"`）作为组件级 Design Token。所有 Token 值均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:EmptyTokenResource EmptyImgHeight}
{atom:EmptyTokenResource EmptyImgHeightMD}
{atom:EmptyTokenResource EmptyImgHeightSM}
{atom:EmptyTokenResource DescriptionMargin}
{atom:EmptyTokenResource DescriptionMarginSM}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorFill}
{atom:SharedTokenResource ColorBorderSecondary}
{atom:SharedTokenResource ColorFillTertiary}
{atom:SharedTokenResource ColorTextDescription}
```

---

## 组件级 Token 一览

以下是 `EmptyToken` 定义的全部组件级 Token。

### 图片高度 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `EmptyImgHeight` | `double` | `SharedToken.ControlHeightLG × 2.5` | Large 尺寸图片高度（约 100px） |
| `EmptyImgHeightMD` | `double` | `SharedToken.ControlHeightLG × 1.85` | Middle 尺寸图片高度（约 74px） |
| `EmptyImgHeightSM` | `double` | `SharedToken.ControlHeightLG × 0.875` | Small 尺寸图片高度（约 35px） |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DescriptionMargin` | `Thickness` | `Thickness(0, SharedToken.UniformlyMarginSM, 0, 0)` | Large 尺寸描述文字上边距 |
| `DescriptionMarginSM` | `Thickness` | `Thickness(0, SharedToken.UniformlyMarginXS, 0, 0)` | Middle / Small 尺寸描述文字上边距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Empty 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### 预设图片颜色 Token

这些 Token 用于动态生成 SVG 预设图片的颜色参数：

| Token 资源键 | 绑定到内部属性 | 用途 |
|---|---|---|
| `ColorFill` | `BorderColor` | Default 图片边框色、Simple 图片边框色 |
| `ColorBorderSecondary` | `BorderColorSecondary` | Default 图片次要边框色 |
| `ColorFillTertiary` | `ShadowColor` | Default 图片阴影色、Simple 图片阴影色 |
| `ColorFillQuaternary` | `ContentColor` | Simple 图片内容区域填充色 |
| `ColorBgElevated` | `BgColor` | 作为颜色混合基色（`ColorUtils.OnBackground` 的背景参数） |

### 描述文字 Token

| Token 资源键 | 使用场景 |
|---|---|
| `ColorTextDescription` | 描述文字 `TextBlock` 的前景色 |

### 本地化资源

| 资源键 | 使用场景 |
|---|---|
| `CommonLangResource.NoData` | 描述文字的默认值（`"No Data"` / `"暂无数据"`） |

---

## Token 对外观的具体影响

### 尺寸与 Token 映射

| 尺寸 | 图片高度 Token | 描述文字间距 Token |
|---|---|---|
| `Large` | `EmptyImgHeight` | `DescriptionMargin` |
| `Middle` | `EmptyImgHeightMD` | `DescriptionMarginSM` |
| `Small` | `EmptyImgHeightSM` | `DescriptionMarginSM` |

### 预设图片颜色生成流程

预设图片的颜色不是直接使用 SharedToken 的值，而是经过 `ColorUtils.OnBackground()` 混合处理：

1. 从 SharedToken 获取原始颜色（如 `ColorFill`、`ColorFillTertiary`）
2. 以 `ColorBgElevated` 作为背景基色
3. 通过 `ColorUtils.OnBackground(原始色, 背景色)` 计算最终显示色
4. 将计算后的颜色注入 SVG 模板

这种处理确保了在不同背景色（亮色/暗色主题）下，预设图片的颜色始终协调：

| 预设类型 | SVG 方法 | 参数 1 | 参数 2 | 参数 3 |
|---|---|---|---|---|
| `Default` | `BuildDefaultImage()` | `shadowColor` → `ColorFillTertiary` | `borderColor` → `ColorFill` | `borderColorSecondary` → `ColorBorderSecondary` |
| `Simple` | `BuildSimpleImage()` | `contentColor` → `ColorFillQuaternary` | `borderColor` → `ColorFill` | `shadowColor` → `ColorFillTertiary` |

### 主题切换影响

| 场景 | 亮色主题表现 | 暗色主题表现 |
|---|---|---|
| Default 预设图片 | 彩色，边框和阴影使用浅灰色调 | 彩色，边框和阴影自动适配深色背景 |
| Simple 预设图片 | 灰色线条，浅色填充 | 灰色线条，自动适配深色背景 |
| 描述文字颜色 | `ColorTextDescription`（通常为较浅灰色） | `ColorTextDescription`（自动适配暗色） |

