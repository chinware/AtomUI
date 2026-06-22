# Card Token 设计

本文档定义 `AtomUI.Desktop.Controls.CardToken` 的专属语义、分类、使用范围和兼容边界。控件 Token 的通用分层、命名、计算、Theme Variables 边界和预设色规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/control-token-guidelines.md)。Card 整体架构见 [Card 桌面版架构设计](overview.md)，内部实现原理见 [Card 桌面版实现原理](implementation.md)，设计和契约变化记录见 [Card Changelog](changelog.md)。

## 1. 定位

CardToken 是 Card 的组件级 Token scope，描述卡片 Header、Body、Actions、Tabs、Extra、阴影、Grid item 和 action icon 的组件语义值。

CardToken 不承载以下状态：

- `Content`、`Header`、`Extra`、`Cover`、`Actions` 等实际内容。
- `ContentType`、`IsLoading`、`IsHoverable`、`IsInnerMode`、`StyleVariant` 等实例行为状态。
- `HeaderBorderThickness`、`EffectiveBorderThickness`、`EffectiveCornerRadius`、`IsActionsPanelVisible` 等内部派生状态。
- `CardGridItem.Row`、`Column`、`RowSpan`、`ColumnSpan` 等布局位置。
- `TabControl` 的 selected item、current content 或 tab collection。

## 2. Token 分类

### 2.1 Header Token

- `HeaderBg`
- `HeaderFontSizeLG`
- `HeaderFontSize`
- `HeaderFontSizeSM`
- `HeaderHeightLG`
- `HeaderHeight`
- `HeaderHeightSM`
- `HeaderPaddingLG`
- `HeaderPadding`
- `HeaderPaddingSM`
- `CardHeadPadding`

这些 Token 控制 Card Header 区域的背景、字体、最小高度、水平 padding 和含 tabs 场景的 header 内容 padding。`SizeType` 负责选择 Large、Middle 或 Small 分支。

### 2.2 Body padding Token

- `BodyPaddingLG`
- `BodyPadding`
- `BodyPaddingSM`
- `CardPaddingBase`

这些 Token 控制普通内容、Meta 内容、Tabs 内容和 Grid item 的内容 padding。Grid 内容本身不使用 Card body padding，padding 由 `CardGridItem` 消费。

### 2.3 Actions Token

- `ActionsBg`
- `ActionsSpacing`
- `CardActionsIconSize`

这些 Token 控制底部操作区背景、操作项间距语义和 action icon 尺寸。当前操作区分隔线颜色和粗细来自 SharedToken 的边框资源。

### 2.4 Tabs Token

- `TabsMarginBottom`

`TabsMarginBottom` 用于 Card 内置 tabs 场景，协调 TabControl header 与 Card header/body 边界的垂直间距。

### 2.5 文本和阴影 Token

- `ExtraColor`
- `CardShadows`
- `CardGridItemShadows`

`ExtraColor` 控制 Header Extra 区域文字色。`CardShadows` 是 hoverable Card 和 hoverable grid item 的强化阴影。`CardGridItemShadows` 使用多层阴影表达 grid item 之间的边线。

## 3. 控件专项模型中的 Token 使用

CardToken 使用路径：

```text
SharedToken
   ↓
CardToken
   ↓
CardTheme / CardActionPanelTheme / CardActionButtonTheme / CardGridItemTheme / CardTabsContentTheme / CardMetaContentTheme
   ↓
Header + Body + Cover + Actions + Grid + Tabs + Meta
```

重要映射：

| Token | 主要使用点 |
| --- | --- |
| `HeaderBg` | 根 Card HeaderFrame 背景。 |
| `HeaderFontSizeLG` / `HeaderFontSize` / `HeaderFontSizeSM` | HeaderFrame 文本尺寸。 |
| `HeaderHeightLG` / `HeaderHeight` / `HeaderHeightSM` | HeaderFrame 最小高度。 |
| `HeaderPaddingLG` / `HeaderPadding` / `HeaderPaddingSM` | HeaderFrame 水平 padding。 |
| `BodyPaddingLG` / `BodyPadding` / `BodyPaddingSM` | Card body、CardGridItem 和 CardTabsContent content padding。 |
| `ActionsBg` | CardActionPanel Frame 背景。 |
| `TabsMarginBottom` | Card 内置 tabs 的下边距语义。 |
| `ExtraColor` | Extra 区域文字颜色。 |
| `CardShadows` | Card hover 和 grid item hover 阴影。 |
| `CardActionsIconSize` | CardActionButton icon 宽高。 |
| `CardGridItemShadows` | CardGridItem 默认边线阴影。 |

实例状态派生：

```text
SizeType
  -> choose Header / Body size branch

StyleVariant
  -> choose bordered or borderless surface

ContentType
  -> choose body padding and corner/border behavior

IsHoverable
  -> choose normal or hover shadow state
```

这些实例状态本身不是 Token。

## 4. 控件家族影响

CardToken 直接影响 Card 家族控件：

- `Card`
- `CardActionPanel`
- `CardActionButton`
- `CardGridContent`
- `CardGridItem`
- `CardTabsContent`
- `CardMetaContent`
- Gallery Card Design Token 表

CardToken 不应被其他 Data Display 控件直接复用。若多个控件需要共享同一语义值，应评估是否上升为 SharedToken，而不是跨控件引用 CardToken。

## 5. 兼容性要求

CardToken 属于 Card 主题契约。即使 `CardToken` 是 internal 类型，生成的 token kind、AXAML resource 使用点和 Gallery Token 表已经形成稳定依赖。

Token 变更要求：

- 不擅自重命名或删除现有 Token。
- 不改变 Header、Body、Actions、Tabs、Extra、Shadow 或 Grid item Token 的语义范围。
- 不把 `ContentType`、`IsLoading`、`IsHoverable`、`StyleVariant` 或 Grid item 位置状态迁移为 Token。
- 不在 CardToken 中复制 SharedToken 已表达的根边框、圆角、基础背景或通用文字颜色语义。
- 修改 size 相关 Token 时必须同时验证 Large / Middle / Small 下 Header、Body、Grid item 和 Tabs 内容。
- 修改 shadow Token 时必须验证 Card hover、borderless 和 grid item hover。
- 需要破坏性变更时，必须先说明影响范围并获得授权。

## 6. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| 修改 Header Token | 验证 Large / Middle / Small Card Header 高度、padding、字体和 Headerless 状态。 |
| 修改 Body padding Token | 验证普通内容、Meta 内容、Tabs 内容和 Grid item padding。 |
| 修改 Actions Token | 验证底部操作区背景、action icon 尺寸、分隔线对齐和 hover 颜色。 |
| 修改 Tabs Token | 验证 CardTabsContent 与 Header/Body 边界。 |
| 修改 `ExtraColor` | 验证 Header Extra 和 HyperLinkButton 等内容视觉。 |
| 修改 shadow Token | 验证 hoverable Card、borderless Card 和 hoverable CardGridItem。 |
| 删除或重命名 Token | 默认不允许；如获授权，需同步 AXAML 引用、生成文件、Gallery Token 表和控件文档。 |
| 文档改动 | 运行 `git diff --check`，检查文档链接存在。 |
