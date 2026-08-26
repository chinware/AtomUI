# Transfer Changelog

本文档记录 Transfer 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record AbstractTransfer as the semantic owner, with TransferSelectDropdown used only as the relay adapter.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-07-05

- API
  - Document `TargetKeys` and `SelectedKeys` as default `TwoWay` key-state contracts with collection mutation support.
- Implementation
  - Record the single-owner Transfer key flow, attached-only collection subscriptions and in-place writable collection updates.
- Docs
  - Add the controlled key binding model to the architecture and implementation documents.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Transfer`.
  - Align generated output paths with `controls/transfer/index-cn.md` and `controls/transfer/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Transfer desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Transfer desktop architecture documentation under `docs/controls/desktop/data-entry/transfer/overview.md`.
  - Add Transfer implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Transfer control-level changelog.
  - Add Transfer Token documentation covering TransferToken.
