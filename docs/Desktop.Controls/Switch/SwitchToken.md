# Switch Design Token

ToggleSwitch 控件使用 `ToggleSwitchToken`（Token ID: `"ToggleSwitch"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:ToggleSwitchTokenResource TrackHeight}
{atom:ToggleSwitchTokenResource HandleBg}
{atom:ToggleSwitchTokenResource SwitchColor}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorTextQuaternary}
```

---

## 组件级 Token 一览

### 轨道 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TrackHeight` | `double` | `SharedToken.FontSize × SharedToken.RelativeLineHeight` | 默认开关高度 |
| `TrackHeightSM` | `double` | `SharedToken.ControlHeight / 2` | 小号开关高度 |
| `TrackMinWidth` | `double` | `HandleSize × 2 + TrackPadding × 4` | 默认开关最小宽度 |
| `TrackMinWidthSM` | `double` | `HandleSizeSM × 2 + TrackPadding × 2` | 小号开关最小宽度 |
| `TrackPadding` | `double` | 固定 `2` | 轨道内边距 |

### 滑块 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HandleBg` | `Color` | `SharedToken.ColorWhite` | 滑块背景色（白色） |
| `HandleSize` | `Size` | 基于 `TrackHeight - TrackPadding × 2` | 默认滑块尺寸 |
| `HandleSizeSM` | `Size` | 基于 `TrackHeightSM - TrackPadding × 2` | 小号滑块尺寸 |
| `HandleShadow` | `BoxShadow` | `0 2px 4px rgba(0,35,11,0.2)` | 滑块阴影 |

### 内容区域 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `InnerMinMargin` | `double` | `HandleSize / 2` | 内容区域最小边距 |
| `InnerMaxMargin` | `double` | `HandleSize + TrackPadding × 3` | 内容区域最大边距 |
| `InnerMinMarginSM` | `double` | `HandleSizeSM / 2 - TrackPadding` | 小号内容区域最小边距 |
| `InnerMaxMarginSM` | `double` | `HandleSizeSM + TrackPadding × 3` | 小号内容区域最大边距 |

### 颜色与字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `SwitchColor` | `Color` | `SharedToken.ColorPrimary` | 开启态轨道颜色（主色调） |
| `SwitchDisabledOpacity` | `double` | `SharedToken.OpacityLoading` | 禁用态不透明度 |
| `ExtraInfoFontSize` | `double` | `SharedToken.FontSizeSM` | 开关内文字字号 |
| `ExtraInfoFontSizeSM` | `double` | `ExtraInfoFontSize - 1` | 小号开关内文字字号 |

### 图标与动画 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `IconSize` | `double` | `TrackHeightSM` | 图标尺寸 |
| `IconSizeSM` | `double` | `TrackHeightSM - SharedToken.UniformlyPaddingXXS` | 小号图标尺寸 |
| `LoadingAnimationDuration` | `TimeSpan` | 固定 `1200ms` | 加载旋转动画周期 |
| `OffStateLoadIndicatorColor` | `Color` | `rgba(0, 0, 0, 0.4)` | 关闭态加载指示器颜色 |

---

## Token 对外观的具体影响

### 状态与颜色映射

| 状态 | 轨道背景色 | 前景色 |
|---|---|---|
| **关闭态** | `ColorTextQuaternary` | `ColorTextLightSolid`（白色） |
| **关闭态 + 悬浮** | `ColorTextTertiary` | `ColorTextLightSolid` |
| **开启态** | `SwitchColor`（`ColorPrimary`） | `ColorTextLightSolid` |
| **开启态 + 悬浮** | `ColorPrimaryHover` | `ColorTextLightSolid` |
| **禁用态 / 加载中** | 同上 + `SwitchDisabledOpacity` | 同上 |

### 尺寸与 Token 映射

| 尺寸 | 轨道高度 | 最小宽度 | 滑块大小 | 字号 |
|---|---|---|---|---|
| `Middle` | `TrackHeight` | `TrackMinWidth` | `HandleSize` | `ExtraInfoFontSize` |
| `Small` | `TrackHeightSM` | `TrackMinWidthSM` | `HandleSizeSM` | `ExtraInfoFontSizeSM` |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，ToggleSwitch 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPrimary` | 开启态轨道背景色（通过 `SwitchColor` 间接引用） |
| `ColorPrimaryHover` | 开启态 + 鼠标悬浮时轨道背景色 |
| `ColorTextQuaternary` | 关闭态轨道背景色 |
| `ColorTextTertiary` | 关闭态 + 鼠标悬浮时轨道背景色 |
| `ColorTextLightSolid` | 开关前景色（文字/图标颜色，通常为白色） |
| `ColorWhite` | 滑块背景色（通过 `HandleBg` 间接引用） |
| `EnableMotion` | 全局动画开关 |
| `EnableWaveSpirit` | 全局波纹开关 |
| `OpacityLoading` | 禁用/加载态不透明度（通过 `SwitchDisabledOpacity` 间接引用） |

