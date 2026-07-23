# ImagePreviewer Changelog

本文档记录 ImagePreviewer 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-07-23

- Behavior
  - Make preview dialog title alignment default to `WindowCenter` on Windows, Linux and macOS while preserving explicit `TitleAlignment` projection to `ImagePreviewerTitleBar`.
  - Paint the dialog image viewer with the dialog background so Windows CSD maximize/restore frames do not expose the generic window underlay.
- Docs
  - Document the preview dialog title alignment invariant and its boundary with the shared `WindowTitleBarLayoutPanel` algorithm.
  - Document the dialog-only viewer background boundary for Windows CSD state transitions.

## 2026-07-08

- API
  - Define `IImagePreviewSource` as the unified image source contract, with `UriImagePreviewSource` for URI inputs and `StreamImagePreviewSource` for lazy stream inputs.
  - Keep source identity out of the main `IImagePreviewSource` contract; add optional `IImagePreviewSourceIdentity` for advanced source reuse.
  - Replace the old URI property family with `Source`, `Sources` and `FallbackSource`; URI inputs now use `UriImagePreviewSource` explicitly.
  - Replace the independent `CoverSourceUri` cover model with `CoverIndex`, keeping cover selection inside the same `Source` / `Sources` source set.
  - Add `MaxConcurrentLoads` and `PreloadCount` as the public loading scheduler contract.
- Behavior
  - Load only the closed-state cover for single `ImagePreviewer`, load default `ImageGroupPreviewer` cover items through the shared scheduler, and load only `CurrentIndex`, `CoverIndex` and the `PreloadCount` neighbor window while preview is open.
  - Preserve loaded cover items when opening preview from an unchanged `Sources` collection, and keep the cover item in the open-state scheduler window so non-modal preview navigation does not blank the page cover.
  - Keep `CoverIndex` display-only: changing cover, loading cover, failing cover or opening preview no longer writes `CurrentIndex`.
  - Preserve `CurrentIndex` when dialog or overlay `ItemsSource` changes; hosts clamp only for display and do not write the public/bound value back to `0`.
  - Treat fallback as a source batch result: single item failure stays local, mixed batches keep loaded items and skip failed items, and `FallbackSource` is used only after all source items in the batch have failed.
- Implementation
  - Keep URI and stream inputs on one loading path: source materialization creates lightweight items, scheduled loads call `OpenReadAsync`, and all results flow through `ImagePreviewLoadScheduler`.
  - Resolve source identity internally: URI sources use `ImageSourceUri.CacheKey`, stream sources use optional identity, and sources without identity use object reference identity.
  - Require stream sources to return a fresh readable stream for each load; ImagePreviewer owns returned stream disposal.
  - Route cover, current image, neighbor preload and fallback loads through `ImagePreviewLoadScheduler`, including priority, generation checks, bounded concurrency and cancellation.
  - Keep canceled running loads counted until the loader actually settles, so `MaxConcurrentLoads` is not exceeded during rapid current-window changes.
- Docs
  - Document the unified source model and default source implementations, including `new UriImagePreviewSource("https://example.com/image.png")` for ordinary web images and `StreamImagePreviewSource` for authenticated, database, encrypted or memory-backed images.
  - Clarify that source objects do not carry cover, fallback, title metadata or business metadata; those concerns remain separate public contracts.
  - Define the lazy loading redesign for large image sets: closed preview loads only the `CoverIndex` cover, open preview loads `CurrentIndex`, `CoverIndex` and the `PreloadCount` neighbor window, and all load paths share `MaxConcurrentLoads`.
  - Document `CoverIndex` as a display-only cover selector that is decoupled from `CurrentIndex`; clicking the cover opens preview without synchronizing the current preview index.
  - Keep the public source model lightweight by using `IImagePreviewSource` only for lazy stream opening, with optional identity extension separated from cover, current item, fallback and scheduler semantics.
  - Add the internal `ImagePreviewLoadScheduler` responsibility for priority, bounded concurrency, generation checks, cancellation, stale result disposal and lazy result writeback.
  - Define source collection fallback as a batch-level decision: individual failed items must not replace the whole preview list with `FallbackSource`; fallback is used only when the current `Source` / `Sources` batch fully fails.
  - Document that mixed success/failure batches keep loaded items, skip failed items, and prevent stale batches from writing back after source replacement or cancellation.

## 2026-07-06

- API
  - Make `IsOpen` and `CurrentIndex` default to `BindingMode.TwoWay` so controlled preview open state and current image index update the bound ViewModel without explicit binding mode.
- Implementation
  - Keep dialog and overlay `CurrentIndex` relay bindings `TwoWay` and owned by host disposables.
- Docs
  - Document the controlled state contract and clarify that these properties are not Form validation values.

## 2026-07-01

- API
  - Add `PreviewTitleIcon` for configuring the optional `PathIcon` displayed before the preview dialog title.
- Theme
  - Add `PART_IconPresenter` to the preview title group so an explicitly configured icon appears before the preview title with the standard logo-title spacing.
  - Keep Windows and Linux caption buttons outside the centered title area, and rely on macOS title bar offset handling instead of inheriting a window logo.
- Docs
  - Document the `PathIcon` based preview title icon model and the `PART_TitleLayout` / `PART_IconPresenter` title group contract.
  - Align ImagePreviewer docs with the control documentation structure by adding the composition model and making semantic parts and template parts source-specific.
  - Define the remote image loading UX contract: inline cover loading uses an image Skeleton, loading/error placeholders keep a stable cover size, default failure text is localized, and custom `LoadingContent` / `ErrorContent` only replaces presenter content.
  - Document that single `ImagePreviewer` and `ImageGroupPreviewer` must consume `CoverWidth` / `CoverHeight` consistently so failed remote images do not collapse to text-height strips.

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
