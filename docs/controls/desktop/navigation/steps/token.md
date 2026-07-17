# Steps Token 设计

本文档定义 `AtomUI.Desktop.Controls.StepsToken` 的专属语义、分类、使用范围和稳定边界。控件 Token 的通用分层、命名、计算、Theme Variables 和预设色规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/control-token-guidelines.md)。Steps 整体架构见 [Steps 桌面版架构设计](overview.md)，内部实现原理见 [Steps 桌面版实现原理](implementation.md)，设计和契约变化记录见 [Steps Changelog](changelog.md)。

## 1. 定位

StepsToken 描述步骤标题、详情内容、Indicator、Dot、Connector、Navigation、Inline 和 Progress ring 的组件级视觉语义。

StepsToken 不承载：

- Current、Initial、item index 或 StepNumber。
- public Status、AutomaticStatus、EffectiveStatus、IsCurrent 或 ConnectorStatus。
- item 数量、layout bounds、pointer、keyboard、focus 或 Wave 播放状态。
- Percent 当前值、CanInvoke、IsItemClickable 或 IsMotionEnabled。

## 2. Token 分类

### 2.1 标题与内容区域

- `DescriptionMaxWidth`
- `HorizontalHeaderMargin`
- `VerticalDescriptionPadding`
- `VerticalLabelContentMargin`

`DescriptionMaxWidth` 和 `VerticalDescriptionPadding` 沿用既有 Token 名称，但它们作用于 `StepsItem.Content` 详情区域，不对应已删除的 Description API。

### 2.2 默认 Indicator

- `IconSize`
- `IconFontSize`
- `IconSizeSM`
- `CustomIconSize`
- `CustomIconFontSize`

这些 Token 控制数字、状态图标和自定义 PathIcon 的容器及内容尺寸。

### 2.3 Dot 与 Connector

- `DotSize`
- `DotCurrentSize`
- `HorizontalDotMargin`
- `VerticalDotMargin`
- `DotLineThickness`

Dot Token 只在 `Type=Dot` 下生效。Connector 方向和长度由 LayoutPanel 决定，Token 只提供尺寸与颜色。

### 2.4 Navigation

- `NavArrowColor`
- `VerticalNavArrowMargin`
- `VerticalNavArrowMarginSM`
- `NavItemGutter`
- `NavItemGutterSM`
- `StepsNavActiveColor`

Navigation Token 控制导航间距、active 表达和可选箭头视觉，不保存 Current 或交互状态。

### 2.5 状态颜色

- `WaitIconColor`、`WaitIconBgColor`、`WaitIconBorderColor`
- `WaitTitleColor`、`WaitDescriptionColor`、`WaitTailColor`、`WaitDotColor`
- `ProcessIconColor`、`ProcessIconBgColor`、`ProcessIconBorderColor`
- `ProcessTitleColor`、`ProcessDescriptionColor`、`ProcessTailColor`、`ProcessDotColor`
- `FinishIconColor`、`FinishIconBgColor`、`FinishIconBorderColor`
- `FinishTitleColor`、`FinishDescriptionColor`、`FinishTailColor`、`FinishDotColor`
- `ErrorIconColor`、`ErrorIconBgColor`、`ErrorIconBorderColor`
- `ErrorTitleColor`、`ErrorDescriptionColor`、`ErrorTailColor`、`ErrorDotColor`

名称中的 `DescriptionColor` 作用于 Content 详情区域。状态选择只读取 EffectiveStatus；Token 不参与状态推导。

### 2.6 Progress ring

- `ProgressFramePadding`
- `ProgressFramePaddingSM`
- `ProgressGrooveColor`
- `ProgressColor`

Progress 外径由当前 `IconSize` / `IconSizeSM` 加对应 padding 推导，不维护重复的固定 size Token。Progress Token 控制 padding、groove 和 arc 颜色。Percent 是运行时输入，不进入 Token。

### 2.7 Inline

- `InlineDotSize`
- `InlineHeaderMargin`
- `InlineHeaderPadding`
- `InlineItemPadding`
- `InlineTitleColor`
- `InlineTailColor`

Inline Token 只服务 `Type=Inline` 的紧凑视觉。

## 3. 控件专项模型中的 Token 使用

```text
SharedToken
    -> StepsToken
    -> StepsTheme / StepsItemTheme / StepsItemIndicatorTheme
    -> root + item + indicator + connector + progress
```

主要映射：

| 输入或状态 | Token 选择 |
| --- | --- |
| `SizeType` | IconSize/IconSizeSM、NavItemGutter/NavItemGutterSM、ProgressFramePadding。 |
| `Type=Dot` | DotSize、DotCurrentSize、DotLineThickness 和 Dot 状态色。 |
| `Type=Navigation` | Navigation 间距、active 色和箭头色。 |
| `Type=Inline` | Inline dot、padding、title 和 tail。 |
| `EffectiveStatus` | 对应 Wait/Process/Finish/Error 的 icon、title、content、tail 和 dot 色。 |
| `IsProgressVisible` | Progress 尺寸、padding、groove 和 arc 色。 |

状态流：

```text
Current / Initial / Status / item.Status
    -> EffectiveStatus
    -> 选择状态颜色 Token

Type / Orientation / EffectiveTitlePlacement / SizeType
    -> LayoutPanel 几何
    -> 选择尺寸和间距 Token
```

Token 不能替代 EffectiveStatus、LayoutPanel 计算或 Percent coercion。

## 4. 控件家族影响

StepsToken 只影响：

- `Steps`
- `StepsItem`
- `StepsItemIndicator`
- `StepsPanel`
- `StepsItemLayoutPanel`
- Steps 三个主题文件
- Gallery Steps Design Token 表

其他 Navigation 控件不跨控件引用 StepsToken。真正共享的颜色、尺寸或 motion 语义应上升为 SharedToken。

## 5. 兼容性要求

完成 Steps 重构后，以下 Token 边界保持稳定：

- 不把实例 Current、Status、Percent、item 数量或 layout bounds 迁移为 Token。
- 状态色必须同时覆盖 Indicator、Title、Content、Connector 和 Dot。
- Indicator 尺寸变化必须验证 Default、Small、状态图标和 custom Icon。
- Dot 尺寸变化必须验证水平、垂直和 current dot 对齐。
- Progress Token 必须由真实 Progress ring 消费，不能只计算而无使用点。
- Inline Token 不能泄漏到 Default、Dot 或 Navigation。
- Token 名称中的 Description 是既有主题术语，不恢复 Description public API。
- 未被主题、Panel 或 Indicator 消费的 Token 必须接入明确语义或从 Token 契约删除，不能保留无效计算值。

## 6. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| 标题/内容 Token | 验证 Header、SubHeader、Content 的宽度、间距和换行。 |
| Indicator Token | 验证 Middle/Small、四种状态、自定义 Icon 和文字居中。 |
| Dot Token | 验证水平、垂直、current dot 和 Connector 对齐。 |
| 状态颜色 | 验证 Wait/Process/Finish/Error 的 Indicator、Title、Content、Connector 和 Dot。 |
| Navigation Token | 验证水平/垂直 Navigation、active 表达、间距和箭头不裁剪。 |
| Progress Token | 验证 Default/Navigation、0/100 Percent、尺寸、厚度、颜色和裁剪。 |
| Inline Token | 验证 Inline dot、Connector、padding、hover 和 current 表达。 |
| Token 删除或重命名 | 同步类型、生成资源、AXAML、Gallery Token 表和控件文档。 |
| 文档 | 运行 `git diff --check`，检查相对链接和 LLMS 源文档一致。 |
