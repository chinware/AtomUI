# Select Changelog

本文档记录 Select 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-06-21

- Docs
  - Add Select control documentation set with architecture overview, implementation notes, Token design and changelog.
  - Document Select mode, option source, selection, filtering, async loading, popup lifecycle, Form / CompactSpace integration and compatibility invariants.
  - Add Select to the Data Entry control documentation index.
- Implementation
  - Reorder `Select` and `AbstractSelect` so control-owned public/protected API members stay before explicit interface regions, with private implementation methods after those contract regions.
  - Consolidate internal drop-down state restoration and candidate selection sync suppression into paired helper paths.
  - Move stable Select template part state binding to AXAML compiled ancestor binding; keep C# `BindUtils.RelayBind` only for AddOnDecoratedBox to SelectHandle hover / pressed sibling part coordination.
- Tests
  - Add Select behavior coverage for option replacement selection preservation, default value selection, Form value mapping and candidate sync suppression recovery.
  - Add Select template binding coverage for AXAML-owned right add-on / count / handle state and the remaining AddOnDecoratedBox hover / pressed relay binding.
