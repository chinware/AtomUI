# ImagePreviewer Changelog

本文档记录 ImagePreviewer 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-06-30

- Docs
  - Document the redesigned `ImageSourceUri` image source contract for local, asset and remote image loading.
  - Add the `ImagePreviewItem` loading state model, default Skeleton/Spin loading behavior, fallback handling and renderer layout invalidation requirements.
  - Define `IImageSourceLoader` responsibilities for asynchronous loading, cancellation, source identity and stale result prevention.

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
