# Button Changelog

本文档记录 Button 控件级设计、API、主题契约、Token 和实现结构的变化。它用于维护控件设计历史，不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

记录原则：

- 只记录会影响 Button 设计理解、兼容边界、实现架构或维护方式的变化。
- 不记录临时讨论、纯格式化或没有长期价值的实现细节。
- 架构文档始终描述最新设计状态；历史变化记录在本文档。

## 2026-06-17

- Feature
  - Add the Button `Color + Variant` public API model with nullable `ButtonColor?` and `ButtonVariant?` properties.
  - Add effective Button state normalization for compatibility with `ButtonType`, `IsDanger` and `IsGhost`.
  - Add internal StyledProperty theme variables for variant text, background, border and shadow states.
  - Map preset colors through the active shared token palette instead of Button-private color tables.
  - Support colorful Button rendering in desktop and Browser Button themes.
  - Add a Gallery Color/Variant matrix example and document the new API rows.

- Docs
  - Establish Button desktop architecture documentation under `docs/controls/desktop/general/button/overview.md`.
  - Add dedicated Button Token design documentation under `docs/controls/desktop/general/button/token.md`.
  - Introduce per-control changelog under `docs/controls/desktop/general/button/changelog.md`.
  - Adopt the per-control documentation directory structure for Button.
  - Define `Color + Variant` as the current Button design model while preserving `ButtonType` and `IsDanger` as compatibility entries.
  - Document Button template contract, behavior priorities, Button family coordination, and Token boundaries.
  - Move global Token design rules out of Button Token documentation into `docs/engineering/control-token-guidelines.md`.
  - Promote the Button documentation structure into the global control documentation guideline at `docs/engineering/control-documentation-guidelines.md`.
