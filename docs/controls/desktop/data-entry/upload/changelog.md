# Upload Changelog

本文档记录 Upload 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## Unreleased

- Design
  - Define the cross-platform drag-and-drop contract in `drag-drop-design.md`.
  - Establish `UploadDropZone` as the sole DragDrop owner and `UploadDefaultDropArea` as a visual-only control.
  - Lock the existing DropZone/DropArea ControlTheme, layout, Token mapping and rendered result as compatibility invariants.
- API
  - Remove `Upload.IsOpenFileDialogOnClick`; click-to-select ownership moves to `UploadDropZone.IsOpenFileDialogOnClick` and `UploadDropZone.SourceKind`.
  - Remove `Upload.Accepts`; replace it with `Upload.AllowedFileTypes` based on `FilePickerFileType`.
  - Add `Upload.CountOverflowBehavior`, `Upload.AdmissionPolicy` and `Upload.InputBatchCompleted` for deterministic batch admission and diagnostics.
  - Remove `UploadDefaultDropArea.FilesDropped`, `FilesDroppedEvent` and `UploadFilesDroppedEventArgs`; `UploadDropZone` is the only drag data entry.
  - Replace URI-only `UploadFileInfo` with immutable nullable metadata plus required `IUploadFileSource` stream access.
  - Add `UploadDropZone` directory policies, traversal limits and read-only drag/drop processing state.
  - Redesign Upload around `Files` as the single state owner.
  - Replace directory mode with composable `UploadTrigger.SourceKind`.
- Theme
  - Move list scrolling into `UploadList`.
  - Replace picture trigger fake task with append trigger content.
- Behavior
  - Require explicit Copy/None negotiation, Drop-only data materialization and one shared picker/drop/programmatic input pipeline.
  - Define directory traversal, per-item rejection, cancellation and StorageItem lease ownership.
  - Serialize input batches so type admission, business policy and count overflow decisions preserve arrival order.
  - Delay accepted source lease release until the corresponding upload execution exits for remove/reset, external collection changes, `Files` replacement, Form Set/Clear and detach paths.
  - Observe picker, DropZone and Upload lifecycle tasks through named cancellation/error handlers instead of `async void` or nested dispatcher async delegates.
  - Make scheduler cancellation wait for the transport execution task to finish before reporting completion.
  - Serialize scheduler maintenance operations and preserve pending/running task ownership when cancellation is requested during cleanup.
  - Continue cancellation and source lease release when collection, Form, batch-completion or task callbacks throw, then propagate the cleanup error.
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
