# Steps Token 设计

本文档定义 `AtomUI.Desktop.Controls.StepsToken` 的专属语义、分类、使用范围和兼容边界。控件 Token 的通用分层、命名、计算、Theme Variables 边界和预设色规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/control-token-guidelines.md)。Steps 整体架构见 [Steps 桌面版架构设计](overview.md)，内部实现原理见 [Steps 桌面版实现原理](implementation.md)，设计和契约变化记录见 [Steps Changelog](changelog.md)。

## 1. 定位

StepsToken 是 Steps 的组件级 Token scope，描述步骤指示器、点状指示器、连接线、标题描述状态色、Navigation 箭头、Inline 风格和 progress ring 的主题语义。

StepsToken 不承载以下状态：

- `CurrentStep`、`SelectedIndex`、`SelectedItem` 或 `CurrentContent`。
- `StepsItem.Status`、`:finished`、`:selected`、pointer over、pressed 或 disabled 状态本身。
- item 数量、Grid 行列、箭头实际坐标或 progress ring 当前角度。
- `IsItemClickable`、`IsShowItemProgress`、`IsMotionEnabled` 等实例行为属性。

## 2. Token 分类

### 2.1 内容与文本区域 Token

- `DescriptionMaxWidth`
- `HorizontalHeaderMargin`
- `VerticalDescriptionPadding`
- `VerticalLabelContentMargin`

这些 Token 控制标题、副标题和描述区域的布局边界。`DescriptionMaxWidth` 用于垂直 label 布局的描述宽度约束；`HorizontalHeaderMargin` 用于水平 label 下标题区域与后方内容的间距；`VerticalDescriptionPadding` 控制描述区域底部 padding；`VerticalLabelContentMargin` 控制垂直 label 模式下指示器与内容之间的距离。

### 2.2 默认指示器尺寸 Token

- `IconSize`
- `IconFontSize`
- `IconSizeSM`
- `CustomIconSize`
- `CustomIconFontSize`

`IconSize` 和 `IconSizeSM` 控制默认数字/状态图标容器尺寸。`IconFontSize` 控制默认指示器内部文字和图标尺寸，也用于 Navigation Dot 分支的箭头槽位高度，保证箭头不被裁剪。`CustomIconSize` 和 `CustomIconFontSize` 控制设置 `StepsItem.Icon` 后的 custom indicator 容器和图标尺寸。

### 2.3 Dot 指示器与连接线 Token

- `DotSize`
- `DotCurrentSize`
- `HorizontalDotMargin`
- `VerticalDotMargin`
- `DotLineThickness`

这些 Token 控制 `ItemIndicatorType=Dot` 下的点状指示器、当前点尺寸、dot 外距和点状连接线厚度。Dot token 只表达点状步骤视觉，不应用作 Navigation 箭头可绘制区域的唯一尺寸来源。

### 2.4 Navigation Token

- `NavArrowColor`
- `VerticalNavArrowMargin`
- `VerticalNavArrowMarginSM`
- `NavItemGutter`
- `NavItemGutterSM`
- `StepsNavActiveColor`

Navigation Token 控制导航箭头颜色、垂直 Navigation 箭头外距、Navigation item 内容与指示线之间的间距，以及导航激活色。`NavArrowColor` 同时写入 `Foreground`、`FillBrush`、`StrokeBrush` 和 `FallbackBrush`，保证 Ant Design icon 的实际绘制颜色一致。

### 2.5 状态颜色 Token

- `WaitIconColor`
- `WaitIconBgColor`
- `WaitIconBorderColor`
- `WaitTitleColor`
- `WaitDescriptionColor`
- `WaitTailColor`
- `WaitDotColor`
- `ProcessIconColor`
- `ProcessTitleColor`
- `ProcessDescriptionColor`
- `ProcessIconBgColor`
- `ProcessIconBorderColor`
- `ProcessTailColor`
- `ProcessDotColor`
- `FinishIconColor`
- `FinishIconBgColor`
- `FinishIconBorderColor`
- `FinishTitleColor`
- `FinishDescriptionColor`
- `FinishTailColor`
- `FinishDotColor`
- `ErrorIconColor`
- `ErrorIconBgColor`
- `ErrorIconBorderColor`
- `ErrorTitleColor`
- `ErrorDescriptionColor`
- `ErrorTailColor`
- `ErrorDotColor`

这些 Token 按 `Wait`、`Process`、`Finish`、`Error` 状态组织，分别控制默认指示器、标题、描述、连接线和 dot 的状态色。状态本身仍由控件和 item 属性决定，不能迁移到 Token。

### 2.6 Progress ring Token

- `StepsProgressSize`
- `ProgressFramePadding`
- `ProgressFramePaddingSM`
- `ProgressGrooveColor`
- `ProgressColor`

这些 Token 服务默认指示器进度环。`ProgressFramePadding` 和 `ProgressFramePaddingSM` 控制 progress frame 的内边距，`ProgressGrooveColor` 和 `ProgressColor` 控制背景圆环和进度圆弧颜色。`StepsProgressSize` 表达 progress 视觉尺寸语义，不能替代运行时的 `ProgressValue`。

### 2.7 Inline 风格 Token

- `InlineDotSize`
- `InlineHeaderMargin`
- `InlineHeaderPadding`
- `InlineItemPadding`
- `InlineTitleColor`
- `InlineTailColor`

Inline Token 只服务 `Style=Inline` 的紧凑步骤视觉。它们控制 inline dot 尺寸、header 外距与内距、item padding、标题颜色和 inline 连接线颜色。

## 3. 控件专项模型中的 Token 使用

StepsToken 使用路径：

```text
SharedToken
   ↓
StepsToken
   ↓
StepsTheme / StepsItemTheme / StepsItemIndicatorTheme
   ↓
root layout + item status + indicator + tail line + navigation arrow + inline style
```

重要映射：

| Token | 主要使用点 |
| --- | --- |
| `DescriptionMaxWidth` | 垂直 label 和 Navigation 垂直 label 内容宽度。 |
| `IconSize` / `IconSizeSM` | 默认指示器容器、Navigation 默认箭头槽位。 |
| `IconFontSize` | 默认指示器字体/icon、Navigation Dot 箭头槽位。 |
| `CustomIconSize` / `CustomIconFontSize` | 自定义 icon 指示器。 |
| `DotSize` / `DotCurrentSize` | Dot 指示器默认和当前尺寸。 |
| `DotLineThickness` | Dot 水平连接线厚度。 |
| `HorizontalHeaderMargin` | 水平 label 标题区域间距。 |
| `VerticalDescriptionPadding` | 描述区域 padding。 |
| `NavArrowColor` | Navigation 箭头实际绘制颜色。 |
| `NavItemGutter` / `NavItemGutterSM` | Navigation 内容与指示线间距。 |
| `StepsNavActiveColor` | Navigation 选中态线条和 active 表达。 |
| `StepsProgressSize` / `ProgressFramePadding` | 默认指示器 progress ring 尺寸体系。 |
| `InlineDotSize` / `InlineItemPadding` | Inline 风格 dot 和 item padding。 |

实例状态派生：

```text
CurrentStep / SelectedIndex
  → Status / IsFinished
  → choose status color tokens

SizeType
  → choose IconSize / IconSizeSM, NavItemGutter / NavItemGutterSM, progress padding

ItemIndicatorType
  → choose default indicator tokens or dot tokens

Style
  → choose Default / Navigation / Inline token usage
```

`CurrentStep`、`ProgressValue`、item count、Grid 行列和箭头坐标是运行时状态，不是 Token。

## 4. 控件家族影响

StepsToken 当前直接影响 Steps 及其内部 item / indicator 体系：

- `Steps`
- `StepsItem`
- `StepsItemIndicator`
- `StepsTheme.axaml`
- `StepsItemTheme.axaml`
- `StepsItemIndicatorTheme.axaml`
- Gallery Steps Design Token 表

其他 Navigation 控件不复用 StepsToken。若多个控件需要共享同一颜色或尺寸语义，应评估是否上升为 SharedToken，而不是跨控件引用 StepsToken。

## 5. 兼容性要求

StepsToken 属于 Steps 主题契约。即使 `StepsToken` 是 internal 类型，生成的 token kind、AXAML resource 使用点和 Gallery Token 表已经形成稳定依赖。

Token 变更要求：

- 不擅自重命名或删除现有 Token。
- 不把 `CurrentStep`、`SelectedIndex`、`ProgressValue`、item 数量、箭头坐标或 pointer 状态迁移为 Token。
- 修改状态色时必须同时验证默认指示器、dot、title、description 和 tail line。
- 修改 `IconSize`、`IconSizeSM` 或 `IconFontSize` 时必须验证 Default、Small、Navigation 和 Dot Navigation 箭头裁剪。
- 修改 dot 尺寸时必须验证 horizontal dot、vertical dot、Navigation dot 和 inline style 没有相互误用。
- 修改 progress token 时必须验证 progress ring 不被 frame、padding 或 margin 裁剪。
- 修改 Inline token 时必须验证 `Style=Inline` 的 dot、连接线、标题和 item padding。
- 删除或重命名 Gallery 表中展示的 Token 必须先获得授权，并同步 AXAML、生成 token kind、Gallery 和文档。

## 6. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| 修改指示器尺寸 Token | 验证 Large / Middle / Small 下默认指示器、完成图标、错误图标、自定义 icon 和文字居中。 |
| 修改 Dot Token | 验证水平、垂直、Navigation 和 Small Dot 分支，当前 dot 与连接线对齐。 |
| 修改状态颜色 Token | 验证 Wait、Process、Finish、Error 下 icon、标题、描述、tail line 和 dot 颜色。 |
| 修改 Navigation Token | 验证水平/垂直 Navigation、Default/Dot、Small/Middle 下箭头颜色、对齐和不裁剪。 |
| 修改 Progress Token | 验证 `IsShowItemProgress=true` 下进度环位置、厚度、颜色和 Small 分支。 |
| 修改 Inline Token | 验证 `Style=Inline` 下 dot、连接线、header padding、hover 和当前态。 |
| 删除或重命名 Token | 默认不允许；如获授权，需同步 AXAML 引用、生成文件、Gallery Token 表和控件文档。 |
| 文档改动 | 运行 `git diff --check`，检查文档链接存在。 |
