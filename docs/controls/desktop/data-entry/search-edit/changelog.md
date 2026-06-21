# SearchEdit Changelog

本文档记录 SearchEdit 控件级设计、API、主题契约、Token 和实现结构的变化。它用于维护控件设计历史，不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

记录原则：

- 只记录会影响 SearchEdit 设计理解、兼容边界、实现架构或维护方式的变化。
- 不记录临时讨论、纯格式化或没有长期价值的实现细节。
- 架构文档始终描述最新设计状态；历史变化记录在本文档。

## 2026-06-21

- Docs
  - Establish SearchEdit desktop architecture documentation under `docs/controls/desktop/data-entry/search-edit/overview.md`.
  - Add SearchEdit implementation documentation covering search button event flow, SearchEditDecoratedBox, SearchButton, SearchEditPanel layout and maintenance invariants.
  - Document that SearchEdit does not own a dedicated Token scope and instead reuses LineEditToken, AddOnDecoratedBoxToken, ButtonToken and SharedToken.
  - Add SearchEdit changelog and link the document set from the Data Entry category entry.
