# Upload Changelog

本文档记录 Upload 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## Unreleased

- Design
  - Define the cross-platform drag-and-drop contract in `drag-drop-design.md`.
  - Establish `UploadDropZone` as the sole DragDrop owner and `UploadDefaultDropArea` as a visual-only control.
  - Lock the existing DropZone/DropArea ControlTheme, layout, Token mapping and rendered result as compatibility invariants.
- API
  - Move click-to-select ownership from `Upload` to `UploadDropZone`.
  - Replace ambiguous string accept rules with typed allowed file types and a unified input batch result.
  - Redesign Upload around `Files` as the single state owner.
  - Replace directory mode with composable `UploadTrigger.SourceKind`.
- Theme
  - Move list scrolling into `UploadList`.
  - Replace picture trigger fake task with append trigger content.
- Behavior
  - Require explicit Copy/None negotiation, Drop-only data materialization and one shared picker/drop/programmatic input pipeline.
  - Define directory traversal, per-item rejection, cancellation and StorageItem lease ownership.
  - Add configurable success auto-remove and pending text.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Upload`.
  - Align generated output paths with `controls/upload/index-cn.md` and `controls/upload/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Upload desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Upload desktop architecture documentation under `docs/controls/desktop/data-entry/upload/overview.md`.
  - Add Upload implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Upload control-level changelog.
  - Add Upload Token documentation covering UploadToken.
