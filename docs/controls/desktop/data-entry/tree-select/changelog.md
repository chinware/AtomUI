# TreeSelect Changelog

本文档记录 TreeSelect 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `TreeSelect`.
  - Align generated output paths with `controls/tree-select/index-cn.md` and `controls/tree-select/semantic-cn.md`.

## 2026-06-21

- Docs
  - Add TreeSelect control documentation set with architecture overview, implementation notes, Token design and changelog.
  - Document TreeSelect tree data, selection, checkable mode, filter strategy, popup lifecycle, Form / CompactSpace integration and compatibility invariants.
  - Add TreeSelect to the Data Entry control documentation index.
- Implementation
  - Reorder `TreeSelect` so control-owned public/protected API members stay before the Form interface region, with private implementation methods after the contract region.
  - Move stable TreeSelect right add-on, count and handle state binding to AXAML compiled ancestor binding.
  - Keep C# `BindUtils.RelayBind` only for AddOnDecoratedBox to SelectHandle hover / pressed sibling part coordination.
- Tests
  - Add TreeSelect template binding coverage for AXAML-owned right add-on / count / handle state and the remaining AddOnDecoratedBox hover / pressed relay binding.
