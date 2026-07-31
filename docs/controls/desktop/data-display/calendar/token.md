# Calendar Token 设计

本文档定义 Calendar 组件 Token 的专属语义、分类与兼容边界。控件 Token 的通用规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/control-token-guidelines.md)。Calendar 整体架构见 [Calendar 桌面版架构设计](overview.md)，实现原理见 [Calendar 桌面版实现原理](implementation.md)，行为规则见 [Calendar 行为设计](behavior-design.md)，变更记录见 [Calendar Changelog](changelog.md)。

## 1. 定位

新 Calendar 的 Token 收敛为七个公开视觉语义。日期值、周标题、Padding、Border、Typography 与 Motion 均从 SharedToken 派生；Fullscreen 单元最小高度通过 `FullCellMinHeight` 固化 Calendar 完整单元的测量规则。

当前 Token scope：

- `CalendarControlToken`，scope id 为 `CalendarControl`，源码位于 `src/AtomUI.Desktop.Controls/Calendar/CalendarControlToken.cs`。

> 注意：旧 `CalendarToken`（scope id `Calendar`）现归 DatePicker 的 CalendarView 使用，与新 Calendar 无关。新 Calendar 通过 `CalendarControlTokenResource` 引用自己的 Token，两者完全独立。

## 2. Token 清单

| Token | 类型 | 默认语义 |
| --- | --- | --- |
| `FullBg` | Color | 完整 Calendar 背景，派生自容器背景（`ColorBgContainer`） |
| `FullPanelBg` | Color | 完整 Calendar Panel 背景，派生自容器背景（`ColorBgContainer`） |
| `ItemActiveBg` | Color | 完整模式选中日期/月单元背景，派生自 active item 背景（`ControlItemBgActive`） |
| `YearControlWidth` | double | Year Select 最小宽度，默认 80 |
| `MonthControlWidth` | double | Month Select 最小宽度，默认 70 |
| `MiniContentHeight` | double | Mini 内容高度，默认 256 |
| `FullCellMinHeight` | double | Fullscreen 日期/月单元最小高度，按 `ControlHeightSM + ((FontHeightSM + MarginXS) * 3 + LineWidth * 2) + PaddingXS / 2 + LineWidthBold` 派生 |

## 3. Token 消费

- C# 控件负责状态归一与伪类同步。
- AXAML/ControlTheme 通过 `{atom:CalendarControlTokenResource Xxx}` 消费 Token。
- Token 默认值从 SharedToken 派生，不读取控件实例状态。
- Fullscreen 与 Mini 共用同一 CalendarView 与 Cell Model，只由 selector 和布局资源改变视觉。

## 4. 伪类

根伪类：`:fullscreen`、`:mini`、`:month`、`:year`、`:show-week`。

Cell 伪类：`:date`、`:month`、`:week`、`:today`、`:selected`、`:outside`、`:disabled`、`:focused`。

## 5. 兼容性要求

- 新 Calendar 不复用旧 `CalendarToken` 的 range 选择、固定 Cell 尺寸和旧 Header 导航 Token。
- 不把实例状态或交互状态写成 Token。
- 如需引入新 Token，必须同步 Token 类型、生成资源、主题引用和本文档。

## 6. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| Token 文档 | `git diff --check`，检查相对链接存在。 |
| Token 默认值 | 运行 Calendar 控件测试，走查 Light/Dark 主题。 |
| Token 名称或数量 | 检查 generated `CalendarControlTokenResource` key、AXAML 引用和 token.md。 |
| 主题映射 | 走查 today、selected、outside、disabled、focused 状态视觉，并确认 CellTemplate/FullCellTemplate 不改变容器状态。 |
