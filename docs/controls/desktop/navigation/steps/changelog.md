# Steps Changelog

本文档记录 Steps 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

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
