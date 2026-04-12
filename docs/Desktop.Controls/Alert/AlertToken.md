# Alert Design Token

Alert 控件使用 `AlertToken`（Token ID: `"Alert"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:AlertTokenResource DefaultPadding}
{atom:AlertTokenResource WithDescriptionPadding}
{atom:AlertTokenResource IconSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorSuccessBg}
{atom:SharedTokenResource ColorInfoBorder}
{atom:SharedTokenResource BorderRadiusLG}
```

---

## 组件级 Token 一览

以下是 `AlertToken` 定义的全部组件级 Token，按功能分组说明。

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DefaultPadding` | `Thickness` | `Thickness(12, SharedToken.PaddingContentVerticalSM)` | 默认内间距（无描述信息时使用） |
| `WithDescriptionPadding` | `Thickness` | `Thickness(SharedToken.PaddingContentHorizontalLG, SharedToken.UniformlyPaddingMD)` | 带描述时内间距（更大，为描述文本留出空间） |
| `MessageWithDescriptionMargin` | `Thickness` | `Thickness(0, 0, 0, SharedToken.UniformlyMarginXS)` | 带描述时消息文本的底部间距（与描述文本之间的间距） |
| `IconDefaultMargin` | `Thickness` | `Thickness(0, 0, SharedToken.UniformlyMarginXS, 0)` | 图标右侧间距（无描述时） |
| `IconWithDescriptionMargin` | `Thickness` | `Thickness(0, 0, SharedToken.UniformlyMarginSM, 0)` | 图标右侧间距（有描述时，更大） |
| `ExtraElementMargin` | `Thickness` | `Thickness(SharedToken.UniformlyMarginXS, 0, 0, 0)` | 额外操作区和关闭按钮的左侧间距 |
| `DescriptionLabelMargin` | `Thickness` | `Thickness(0, SharedToken.UniformlyMarginXS, 0, 0)` | 描述文本的顶部间距 |

### 图标 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `IconSize` | `double` | `SharedToken.FontSizeLG` | 无描述时图标尺寸 |
| `WithDescriptionIconSize` | `double` | `SharedToken.FontSizeHeading3` | 带描述时图标尺寸（更大，因为内容区域更高） |
| `CloseIconSize` | `double` | `SharedToken.FontSizeIcon + 2` | 关闭按钮图标尺寸 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Alert 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorSuccessBg` / `ColorSuccessBorder` | Success 类型背景/边框 |
| `ColorInfoBg` / `ColorInfoBorder` | Info 类型背景/边框 |
| `ColorWarningBg` / `ColorWarningBorder` | Warning 类型背景/边框 |
| `ColorErrorBg` / `ColorErrorBorder` | Error 类型背景/边框 |
| `ColorSuccess` / `ColorPrimary` / `ColorWarning` / `ColorError` | 四种类型图标的填充色 |
| `BorderRadiusLG` | 圆角半径 |
| `BorderThickness` | 边框粗细 |
| `SpacingXXS` | DockPanel 元素间距 |
| `FontSize` / `FontSizeLG` | 消息文本字号（无描述/有描述） |
| `FontHeight` | 消息文本行高 |
| `IconSizeSM` | 关闭按钮图标尺寸 |

---

## Token 对外观的具体影响

### 类型与颜色映射

| 类型 | 背景色 | 边框色 | 图标色 | 图标 |
|---|---|---|---|---|
| `Success` | `ColorSuccessBg` | `ColorSuccessBorder` | `ColorSuccess` | `CheckCircleFilled` |
| `Info` | `ColorInfoBg` | `ColorInfoBorder` | `ColorPrimary` | `InfoCircleFilled` |
| `Warning` | `ColorWarningBg` | `ColorWarningBorder` | `ColorWarning` | `ExclamationCircleFilled` |
| `Error` | `ColorErrorBg` | `ColorErrorBorder` | `ColorError` | `CloseCircleFilled` |

### 描述信息对布局的影响

| 维度 | 无描述 | 有描述 |
|---|---|---|
| 内间距 | `DefaultPadding` | `WithDescriptionPadding` |
| 消息字号 | `FontSize` | `FontSizeLG` |
| 图标尺寸 | `IconSize` | `WithDescriptionIconSize` |
| 图标间距 | `IconDefaultMargin` | `IconWithDescriptionMargin` |
| 图标垂直对齐 | `Center` | `Top` |
| 关闭按钮垂直对齐 | `Center` | `Top` |
