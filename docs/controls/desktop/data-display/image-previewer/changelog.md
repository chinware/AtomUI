# ImagePreviewer Changelog

本文档记录 ImagePreviewer 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-06-30

- Behavior
  - Make inline cover selection follow `CurrentIndex` when `CoverSourceUri` is not set, keeping dialog and overlay preview on the same current item contract.
  - Resolve preview dialog title from explicit title first and current image file name second.
- API
  - Add `PreviewTitle`, `PreviewTitleResolver`, `IImagePreviewTitleResolver`, `ImagePreviewTitleResolveContext` and `DefaultImagePreviewTitleResolver`.
- Docs
  - Document the redesigned `ImageSourceUri` image source contract for local, asset and remote image loading.
  - Add the `ImagePreviewItem` loading state model, default Skeleton/Spin loading behavior, fallback handling and renderer layout invalidation requirements.
  - Define `IImageSourceLoader` responsibilities for asynchronous loading, cancellation, source identity and stale result prevention.
  - Define `CurrentIndex` as the shared current item contract for inline cover, dialog and overlay preview, with `CoverSourceUri` kept as the explicit cover override.
  - Define the preview title resolution model: non-empty `Window.Title` or `PreviewTitle` wins; otherwise the default resolver derives a title from the current image source file name.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `ImagePreviewer`.
  - Align generated output paths with `controls/image-previewer/index-cn.md` and `controls/image-previewer/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete ImagePreviewer desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish ImagePreviewer desktop architecture documentation under `docs/controls/desktop/data-display/image-previewer/overview.md`.
  - Add ImagePreviewer implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add ImagePreviewer control-level changelog.
  - Add ImagePreviewer Token documentation covering ImagePreviewerToken.
