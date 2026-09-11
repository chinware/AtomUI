# Steps Changelog

本文档记录 Steps 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-08-22

- API
  - 新增 `StepsType.Panel`，并新增 `StepsPanelVariant.Filled` / `StepsPanelVariant.Outlined` 与 `Steps.PanelVariant`。
- Layout
  - Panel 强制水平等宽布局，忽略垂直 Orientation 请求。
  - 隐藏 Indicator 和普通 Connector，使用 internal `PanelArrow` 在相邻 item 之间绘制可拉伸楔形箭头，并支持 RTL。
- Theme / Token
  - 新增 Panel 状态背景、active 背景、边框厚度、圆角和箭头尺寸 Token。
  - Filled 使用状态面板色，Outlined 使用容器背景、状态边框和浅色 active 背景；Small 使用独立箭头宽度和圆角。
- Tests
  - 增加 Panel 等宽、布局方向、Arrow 外溢、RTL、Filled/Outlined 和模板可见性回归覆盖。

## 2026-08-03

- API
  - 在 `Steps` 根控件定义 nullable `ItemHeaderForeground`、`ItemSubHeaderForeground` 和 `ItemRailBackground` 实例级语义样式属性；默认 `null` 保留当前状态和类型的 Token 视觉。
- Theme
  - 将三项公开语义值投影到 `StepsItem` internal StyledProperty，并仅由 `StepsItemTheme.axaml` 在自身模板边界内消费。
  - 禁止 Gallery 和外部样式依赖 `HeaderPresenter`、`SubHeaderPresenter`、`Connector` 或通过 `/template/` selector 穿透 `StepsItem`。
- Gallery
  - Inline style combination 通过 `Steps` 公开语义 API 表达，不再使用 class 和深层 selector 修改 item 内部节点。
- Docs
  - 同步根 API、容器投影生命周期、模板所有权和实例覆盖相对 StepsToken 的优先级契约。

## 2026-07-17

- API
  - 将 `Steps` 的基类从 `SelectingItemsControl` 改为 `ItemsControl`，删除 Selection、`CurrentContent` 和 `IsFinished` 契约。
  - 将根 API 统一为 `Current`、`Initial`、`Status`、nullable `Percent`、`Type` 和 `TitlePlacement`。
  - 将 `StepsStyle` 与 `StepsItemIndicatorType` 合并为 `StepsType`，将状态枚举统一为 `StepsStatus`。
  - 新增 `StepsType.OutlineDot`，作为与 `Dot` 共享布局的空心点状视觉类型。
  - 删除 `StepsItem.Description` / `DescriptionTemplate`，由 `Content` / `ContentTemplate` 表达步骤详情；`StepsItem.Status` 改为 nullable 显式覆盖。
  - 新增受控导航请求事件 `CurrentChangeRequested`；item 激活不直接修改 `Current`。
- Behavior
  - 采用 Ant Design 的 `Initial + index` 编号、Current 越界、item Status 覆盖和 Connector nextStatus 语义。
  - Wave 改为只响应真实 pointer click；程序化 Current、状态重算和 keyboard 激活不播放 Wave。
- Theme
  - 根、item 和 indicator 各使用一套统一语义模板，删除 Style、Orientation、Indicator 和 TitlePlacement 组合模板。
  - `OutlineDot` 使用透明背景和状态色边框，不播放 Indicator Wave。
  - 使用 internal `StepsPanel` 和 `StepsItemLayoutPanel` 分别承担 item 间和 item 内布局。
  - Navigation 当前项使用唯一 `NavigationActiveIndicator` 节点表达水平底线或垂直右侧线。
- Token
  - 删除冗余 `StepsProgressSize`；Progress 外径改为由 icon size 与 `ProgressFramePadding` 推导。
- Implementation
  - 将 EffectiveStatus 设为状态视觉唯一输入，删除 Current/Selection 双向同步和 Content observable 生命周期。
  - 容器状态改为无旧值依赖的确定性投影，并明确 prepare/index change/clear 的 owner 生命周期。
- Docs
  - 按 Ant Design 6.4.5 和 `@rc-component/steps` 1.2.2 更新架构、实现和 Token 文档。

## 2026-07-06

- API
  - 将 `CurrentStep` 设为默认 `TwoWay` 受控步骤状态。
- Implementation
  - `SelectedIndex` 变化回写 `CurrentStep`，让点击步骤和绑定源保持同步。
- Gallery
  - 在 Switch Step 示例中展示可点击步骤与 `CurrentStep` 绑定，标记为 `v6.0.8`。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Steps`.
  - Align generated output paths with `controls/steps/index-cn.md` and `controls/steps/semantic-cn.md`.

## 2026-06-22

- Docs
  - 建立 Steps 控件文档目录，补齐 `overview.md`、`implementation.md`、`token.md` 和 `changelog.md`。
  - 记录 Steps 的公共契约、CurrentStep/Selection 状态流、Status 派生、CurrentContent、Default/Navigation/Inline 主题结构、Dot 指示器、进度环、Navigation 箭头槽位和验证策略。
  - 在 Navigation 分类入口中登记 Steps 文档。
