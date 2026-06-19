# NavMenu Token 设计

本文档定义 `AtomUI.Desktop.Controls.NavMenuToken` 的 NavMenu 专属语义、分类、使用范围和兼容边界。控件 Token 的通用分层、命名、计算、Theme Variables 边界和预设色规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/control-token-guidelines.md)。NavMenu 整体架构见 [NavMenu 桌面版架构设计](overview.md)，设计和契约变化记录见 [NavMenu Changelog](changelog.md)。

## 1. 定位

NavMenuToken 是 NavMenu 的组件级设计变量层。它把全局颜色、尺寸、间距、圆角、字体和 popup 体系转换为 NavMenu 可消费的语义值。

NavMenuToken 服务以下主题：

- `NavMenuTheme.axaml`
- `NavMenuItemTheme.axaml`
- `BaseNavMenuItemHeaderTheme.axaml`
- `HorizontalNavMenuItemHeaderTheme.axaml`
- `VerticalNavMenuItemHeaderTheme.axaml`
- `InlineNavMenuItemHeaderTheme.axaml`

NavMenuToken 不承载 `SelectedItem`、`IsSubMenuOpen`、`IsInSelectedPath`、`IsPointerOverSubMenu`、`Level`、`IsTopLevel` 等实例状态。这些状态由控件状态模型、容器层和主题 selector 处理。

## 2. Token 分类

NavMenuToken 当前按 NavMenu 语义分为八类。

### 2.1 根与容器背景 Token

- `ItemBg`
- `DarkMenuBg`
- `MenuSubMenuBg`

用于 root 菜单背景和子菜单容器背景。`ItemBg` 对应 light root 背景，`DarkMenuBg` 对应 dark root 背景。背景 token 不等同于 header 背景；header 默认背景应保持透明，再由 hover / selected state 决定。

### 2.2 菜单项文字与状态 Token

- `ItemColor`
- `ItemHoverColor`
- `ItemSelectedColor`
- `ItemDisabledColor`
- `HorizontalItemHoverColor`
- `HorizontalItemSelectedColor`
- `KeyGestureColor`
- `GroupTitleColor`
- `GroupTitleLineHeight`
- `GroupTitleFontSize`

用于菜单项前景、快捷键、分组标题和 horizontal 顶层文字状态。`ItemSelectedColor` 同时服务 selected leaf 和 selected path ancestor 的前景语义。

### 2.3 菜单项背景 Token

- `ItemHoverBg`
- `ItemActiveBg`
- `ItemSelectedBg`
- `SubMenuItemBg`
- `HorizontalItemHoverBg`
- `HorizontalItemSelectedBg`

用于 header hover、active、selected 以及 inline submenu 背景块。`SubMenuItemBg` 只应用于 inline child frame，不应被用作 root menu 背景。

### 2.4 尺寸、圆角与间距 Token

- `ItemHeight`
- `ItemContentMargin`
- `ItemContentPadding`
- `ItemMargin`
- `VerticalItemsPanelSpacing`
- `VerticalChildItemsMargin`
- `VerticalMenuContentPadding`
- `HorizontalItemMargin`
- `ItemBorderRadius`
- `SubMenuItemBorderRadius`

用于菜单项高度、Ant Design block margin 映射、header 内边距、inline child frame 外距和菜单内容 padding。`ItemContentMargin` 表达 item header 的外部 margin；`VerticalChildItemsMargin` 表达 inline submenu 背景块在背景模式下的外部 margin，不是所有 inline 子菜单的无条件间距。

### 2.5 Icon 与箭头 Token

- `ItemIconSize`
- `IconSize`
- `IconMargin`
- `CollapsedIconSize`
- `MenuArrowSize`
- `InlineItemIndentUnit`

用于菜单项图标、horizontal 顶层图标间距、箭头尺寸和 inline 缩进。`InlineItemIndentUnit` 默认来自 `ItemHeight / 2`，使层级缩进与菜单项高度保持比例关系。

### 2.6 Popup Token

- `MenuPopupBg`
- `DarkMenuPopupBg`
- `MenuPopupContentPadding`
- `MenuPopupMinWidth`
- `MenuPopupMaxWidth`
- `MenuPopupMaxHeight`
- `TopLevelItemPopupMarginToAnchor`

用于 `Vertical` / `Horizontal` 弹出式子菜单。Popup 视觉不能直接使用 shared elevated background 绕过组件 token，因为 NavMenu popup 背景和 root/background/dark style 是稳定主题契约。

### 2.7 Horizontal 导航 Token

- `MenuHorizontalHeight`
- `HorizontalLineHeight`
- `HorizontalItemMargin`
- `HorizontalItemBorderRadius`
- `ActiveBarScaleX`
- `ActiveBarHeight`
- `HorizontalItemHoverBg`
- `HorizontalItemSelectedBg`

用于 horizontal 顶层菜单高度、文字行高、active indicator、水平 item margin 和 light/dark selected 表达。Light style 顶层选中主要通过 active indicator 表达；dark style 可以通过 selected background 表达。

### 2.8 Dark 与 Danger Token

- `DarkItemColor`
- `DarkDangerItemColor`
- `DarkItemBg`
- `DarkSubMenuItemBg`
- `DarkItemSelectedColor`
- `DarkItemSelectedBg`
- `DarkItemHoverBg`
- `DarkGroupTitleColor`
- `DarkItemHoverColor`
- `DarkItemDisabledColor`
- `DarkDangerItemSelectedBg`
- `DarkDangerItemHoverColor`
- `DarkDangerItemSelectedColor`
- `DarkDangerItemActiveBg`
- `DangerItemColor`
- `DangerItemHoverColor`
- `DangerItemSelectedColor`
- `DangerItemActiveBg`
- `DangerItemSelectedBg`

Dark token 服务 NavMenu dark style，不应由 light token 自动反推。Danger token 保留菜单危险语义的主题能力；即使当前节点模型没有公开危险节点 API，也不应随意删除或重命名。

## 3. NavMenu 专项模型中的 Token 使用

### 3.1 Ant Design Menu 间距映射

NavMenu 使用 `ItemContentMargin` 映射 Ant Design `itemMarginInline` 与 `itemMarginBlock`：

```text
ItemContentMargin = marginXXS, 0, marginXXS, marginXXS
VerticalItemsPanelSpacing = 0
VerticalMenuContentPadding = 0, marginXXS, 0, 0
```

相邻 item 的垂直间距由 item header 的 bottom margin 表达，不通过 StackPanel spacing 叠加。

inline 子菜单第一项与父 header 之间不增加额外顶部 margin。inline 子菜单背景块与下一个根项之间的背景模式 gap 由 `VerticalChildItemsMargin` 表达，并且只在 `IsItemBackgroundEnabled=true` 时由主题 selector 应用。

### 3.2 Root、Popup、Header 与 Inline 背景分离

背景相关 token 必须按职责使用：

| 背景职责 | Token |
| --- | --- |
| Light root menu | `ItemBg` |
| Dark root menu | `DarkMenuBg` |
| Light popup menu | `MenuPopupBg` |
| Dark popup menu | `DarkMenuPopupBg` |
| Light inline submenu background block | `SubMenuItemBg` |
| Dark inline submenu background block | `DarkSubMenuItemBg` |
| Header hover | `ItemHoverBg` / `DarkItemHoverBg` |
| Header selected | `ItemSelectedBg` / `DarkItemSelectedBg` |

不能用一个背景 token 同时承担 root、popup、header 和 inline submenu block。这样会破坏 `IsItemBackgroundEnabled`、dark style 和 popup 视觉的独立性。

### 3.3 Selection 与 Horizontal Active Indicator

选中状态使用两组 token：

- 文字：`ItemSelectedColor`、`DarkItemSelectedColor`、`HorizontalItemSelectedColor`
- 背景/指示：`ItemSelectedBg`、`DarkItemSelectedBg`、`HorizontalItemSelectedBg`、`ActiveBarScaleX`、`ActiveBarHeight`

Horizontal light style 的顶层选中主要由 `PART_ActiveIndicator` 表达，背景保持透明。Dark style 顶层选中可以使用 `DarkItemSelectedBg`。

## 4. 控件家族影响

NavMenuToken 当前服务 NavMenu 及其 internal container/header 体系：

- `NavMenu`
- `NavMenuItem`
- `HorizontalNavMenuItemHeader`
- `VerticalNavMenuItemHeader`
- `InlineNavMenuItemHeader`
- `BaseNavMenuItemHeader`

Token 修改必须同时评估三种 mode、light/dark 两套主题、root/popup/inline child 三类背景和 Gallery Navigation/Menu Showcase。

NavMenuToken 不服务 Breadcrumb、Pagination、Steps 或 TabControl。导航分类内的其他控件不能直接复用 NavMenuToken 来表达自身状态。

## 5. 兼容性要求

NavMenuToken 属于 NavMenu 主题契约。即使 `NavMenuToken` 是 internal 类型，生成的 `NavMenuTokenKind` 和 AXAML resource 使用点已经形成稳定依赖。

Token 变更要求：

- 不擅自重命名或删除既有 Token。
- 不改变既有 Token 的语义含义。
- 不把实例状态迁移到 Token。
- 不把 root、popup、header、inline submenu block 背景合并为同一职责。
- 不让 `VerticalChildItemsMargin` 在 `IsItemBackgroundEnabled=false` 时影响布局。
- 不把 Ant Design block margin 映射改为 StackPanel spacing 叠加。
- 需要破坏性变更时，必须先说明影响范围并获得授权。

## 6. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| 新增 NavMenuToken | 检查生成的 `NavMenuTokenKind`、AXAML 引用和默认值计算。 |
| 修改颜色 Token | 覆盖 light / dark root、popup、inline child、header hover、selected path。 |
| 修改间距 Token | 运行 `NavMenuLayoutTests`，覆盖 root inset、inline child gap、popup inset 和 `IsItemBackgroundEnabled` true/false。 |
| 修改 popup Token | 覆盖 vertical/horizontal popup 背景、尺寸、padding 和 overlay popup 行为。 |
| 修改 horizontal Token | 覆盖 active indicator、top-level margin、line height 和 dark selected background。 |
| 删除或重命名 Token | 默认不允许；如获授权，需同步所有 AXAML 引用和生成文件。 |
