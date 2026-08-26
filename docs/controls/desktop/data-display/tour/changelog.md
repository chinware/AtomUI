# Tour Changelog

本文档记录 Tour 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record Tour as the semantic owner for Tour.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-07-06

- API
  - Keep `CurrentIndex` as default `BindingMode.TwoWay` and make `IsOpen` default to `BindingMode.TwoWay` for controlled tour state.
- Docs
  - Document `IsOpen` and `CurrentIndex` as controlled state rather than Form validation values.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Tour`.
  - Align generated output paths with `controls/tour/index-cn.md` and `controls/tour/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Tour desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Tour desktop architecture documentation under `docs/controls/desktop/data-display/tour/overview.md`.
  - Add Tour implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Tour control-level changelog.
  - Add Tour Token documentation covering TourToken.
