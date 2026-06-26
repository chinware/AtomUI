# Form Changelog

本文档记录 Form 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Form`.
  - Align generated output paths with `controls/form/index-cn.md` and `controls/form/semantic-cn.md`.

## 2026-06-23

- Docs
  - Add Form control documentation set with architecture overview, implementation notes, Token design and changelog.
  - Document Form layout, FormItem content contract, validation lifecycle, feedback model, submit/reset flow, SizeType forwarding and compatibility invariants.
  - Add Form to the Data Entry control documentation index.
- Token
  - Document FormToken categories for labels, required mark, colon margin, item spacing and vertical label layout.
