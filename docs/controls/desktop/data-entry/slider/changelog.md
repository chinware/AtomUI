# Slider Changelog

本文档记录 Slider 控件级设计、API、主题契约、Token 和实现结构的变化。它用于维护控件设计历史，不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

记录原则：

- 只记录会影响 Slider 设计理解、兼容边界、实现架构或维护方式的变化。
- 不记录临时讨论、纯格式化或没有长期价值的实现细节。
- 架构文档始终描述最新设计状态；历史变化记录在本文档。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Slider`.
  - Align generated output paths with `controls/slider/index-cn.md` and `controls/slider/semantic-cn.md`.

## 2026-06-21

- Docs
  - Establish Slider desktop architecture documentation under `docs/controls/desktop/data-entry/slider/overview.md`.
  - Add Slider implementation documentation for track rendering, thumb layout, mark handling, tooltip synchronization, Form integration and maintenance invariants.
  - Add Slider Token documentation for track, rail, mark, thumb, outline, color and orientation padding tokens.
  - Add Slider documentation links to the Data Entry category entry.
- Implementation
  - Reorder `Slider`, `SliderTrack` and `SliderThumb` members to keep public/protected control API and explicit interface regions before private helpers.
  - Move `SliderThumb` routed event wrappers into a dedicated public event region.
  - Centralize `Slider` pointer handler cleanup in a paired helper without changing pointer behavior.
