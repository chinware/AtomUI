# LineEdit Changelog

本文档记录 LineEdit 输入控件家族级设计、API、主题契约、Token 和实现结构的变化。它用于维护控件设计历史，不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

记录原则：

- 只记录会影响 LineEdit 家族设计理解、兼容边界、实现架构或维护方式的变化。
- 不记录临时讨论、纯格式化或没有长期价值的实现细节。
- 架构文档始终描述最新设计状态；历史变化记录在本文档。

## 2026-06-21

- Docs
  - Establish LineEdit desktop architecture documentation under `docs/controls/desktop/data-entry/line-edit/overview.md`.
  - Add LineEdit implementation documentation covering TextBox, LineEdit, SearchEdit, TextArea, template hookup, Form, CompactSpace and resize flows.
  - Add LineEdit Token documentation for `LineEditToken` and `TextAreaToken`.
  - Add LineEdit changelog and link the document set from the Data Entry category entry.
  - Document `CustomizableSizeType.Custom` as a Middle-baseline custom size path for LineEdit family controls.
  - Link SearchEdit's dedicated control documentation and clarify that SearchEdit's right external add-on slot is occupied by the search button.
