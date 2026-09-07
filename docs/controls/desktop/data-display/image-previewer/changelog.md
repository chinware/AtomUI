# ImagePreviewer Changelog

本文档记录 ImagePreviewer 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-09-07

- Design
  - Define the formal ImagePreviewer loading architecture around `ImageSourceKey -> ImageSourceVersion -> ImageContentId -> ImageDecodeKey`, so source addresses no longer act as content identities.
  - Separate source snapshots, validated encoded content and decoded content into application-owned stores while keeping Previewer collection and cache lifecycles independent.
  - Define source-first validation, two-level single-flight, monotonic source commit generations, waiter-local cancellation, cache epochs and result leases as the shared loading invariants.
  - Define the closed `ImageSource` hierarchy and `ImageFileValidationMode` as the public source contract without compatibility shims.
  - Separate the public cache contract into `ImageCacheReadPolicy` and `ImageCacheStoragePolicy`; ordinary loads default to `ValidateSource`, while Reload uses a per-request `RefreshSource` override.
  - Separate `ImageLoadOrigin` from `ImageSourceValidation` so cache location and source validation are not conflated.
- Behavior
  - Restore strict `Immediate` switching semantics for both the preview surface and the single cover: a target without a displayable image clears the previous frame and reports loading in the same UI update cycle, including the Idle interval before its request starts. Remove the retained-frame grace period and delayed callback from this mode.
  - Keep `WaitForLoaded` as the only retained-frame mode and scope target markers to one tracker session, preventing late older requests, never-targeted preloads and stale markers from a closed host session from replacing the current visible frame.
  - Recompute preview and cover display state immediately when `ImageSwitchMode` changes at runtime.
  - Bind the Gallery rapid-switch cover and preview to the same rolling index so the demonstration compares the two display modes without targeting different items.
- Breaking API
  - Replace `ImageLoadSource` and its factories with the closed `ImageSource` hierarchy and concrete source constructors.
  - Replace `ImageCacheMode` with orthogonal `ImageCacheReadPolicy` and `ImageCacheStoragePolicy` properties.
  - Replace `ImageCacheSource` diagnostics with independent `ImageLoadOrigin`, `ImageSourceValidation` and `ContentId` result fields.
- Implementation
  - Make every ordinary load resolve or validate the source snapshot before constructing `ImageDecodeKey`; remove the SourceKey-based decoded fast path that could return stale images after same-path replacement.
  - Store validated encoded bytes and decoded entries by SHA-256 `ImageContentId`, allowing different sources with identical content to share work without conflating source freshness.
  - Add source commit generations, cache epochs and persistent-write serialization so late source/decode operations cannot roll back a newer snapshot or repopulate a cleared scope.
  - Replace the persistent layout with stable `image-cache/` content and source stores; keep `formatRevision` internal and rebuild incompatible layouts without migration shims.
  - Keep `CacheStorage=None` as a strict no-write policy even on persistent hits, and invalidate a source's prior memory/disk variant when HTTP changes to `no-store` without allowing late responses to erase newer generations.
  - Keep ImagePreviewer Full and Thumbnail generations, cancellation sources, progress, states and result leases independent; collection and host lifecycle never clear the application cache.
  - Propagate same-bucket Preload-to-Critical upgrades through a shared mutable waiter-priority state so queued source and decode work is reprioritized without canceling or restarting the request.
  - Detach Full and Thumbnail results, publish a consistent image/state transition, and only then release old leases during unload, failed replacement and entry disposal, preventing a host from observing an already released image or a failed state paired with the previous frame.
- Tests
  - Add regressions for same-path file replacement, identical content across different sources, metadata versus content-hash file validation, cache read/storage policies and persistent content/source separation.
  - Add an ImagePreviewer integration regression proving that clear-and-readd of the same file path renders the replacement without a manual cache clear.
  - Add regressions for Immediate loading before request start, continuous rapid switching, runtime mode changes, monotonic WaitForLoaded fallback selection, atomic failed-state publication, display cleanup before lease release, queued priority promotion and synchronized Gallery cover/current bindings.
- Docs
  - Rewrite the formal ImagePreviewer architecture document with the content-addressed cache model, persistent `image-cache/` naming, source validation matrix, Previewer collection ownership and verification contract.
  - Synchronize the switching design and implementation documents with strict Immediate loading semantics, WaitForLoaded-only retention and the no-timer resource boundary.

## 2026-09-03

- API
  - Add `ImageSwitchMode` (`Immediate`, `WaitForLoaded`) and `AbstractImagePreviewer.ImageSwitchMode` (default `Immediate`) to select when the loading placeholder replaces the previous image while the target item switches.
- Behavior
  - Unify display continuity for both preview surface and cover: while the target has no image yet (including the Idle transient right after a switch) the previously loaded image stays visible; `Immediate` falls back to the loading placeholder only after a 300 ms grace period, `WaitForLoaded` keeps it indefinitely. High-frequency add-and-track-latest switching no longer produces blank-frame or spinner strobing in either mode.
  - Feed the retained frame from two levels: the current item when it completes, and the latest completed full load across entries when the current item is superseded before finishing (switching faster than loading).
  - Keep `ImageOpened` / `ImageFailed` timing bound to the target entry state machine; cancellations never produce failure events.
- Loading pipeline
  - Classify shared-operation internal cancellations (racing waiter teardown, `ClearCache(CancelInFlight)`) as cancellations instead of `InvalidSource` source failures in `ImageLoader`.
  - Resolve cancellations benignly in `ImagePreviewEntry`: keep a previously committed image or return to Idle, without a Failed commit; the non-fatal fallback no longer swallows `OperationCanceledException`.
  - Broadcast cached Full/Thumbnail reset notifications from `ImagePreviewEntry.Dispose()` before clearing subscribers so display holders drop references to released bitmaps.
  - Adopt in-flight requests on same-bucket priority upgrades (Preload→Critical) instead of restarting them, so requests can complete when switching outpaces loading.
- Hosts and theme
  - Introduce the internal `ImagePreviewDisplayTracker` as the single display state machine shared by `ImagePreviewerDialog` and `ImagePreviewerOverlayHost`, replacing their duplicated current-entry tracking.
  - Make both hosts observe effective-collection incremental changes so the display target follows entry replacement even when `CurrentIndex` keeps the same value (capped-list trimming no longer leaves a stale, eventually disposed entry displayed as a blank preview).
  - Gate the viewer loading presenter with the `:loading:not(:has-image)` selector and make the cover hover mask depend only on `IsShowCoverMask`, decoupling both from raw load-state flicker.
- Docs
  - Add the switching display design document covering the display matrix, retained-frame lifecycle with grace and seeding, host collection following, subscription pairing, pipeline cancellation delivery and resource bounds; link it from the overview and implementation docs.

## 2026-08-24

- Breaking API
  - Replace the single/multiple source properties and preview-specific source interfaces with `ItemsSource: IEnumerable<ImagePreviewItem>?`.
  - Make `ImagePreviewItem` an immutable configuration record using `ImageLoadSource`, optional `ThumbnailSource`, per-item `FallbackSource`, `RequestOptions`, `Title` and `Tag`.
  - Remove the control-private loader, loaded result types, scheduler and `MaxConcurrentLoads`; do not provide compatibility shims.
  - Add current and cover load state projections, `ImageOpened`/`ImageFailed`, `ReloadCurrent()`, `ReloadItem(index)` and `ReloadCover()`.
- Loading
  - Route current, cover and neighbor requests through the application-scoped `IImageLoader` with Critical, High and Preload priorities.
  - Keep Full and Thumbnail generations, cancellation and result leases isolated in internal `ImagePreviewEntry` instances.
  - Apply fallback per item and per channel; one failed item never replaces the collection.
  - Make Full and Thumbnail requests size-aware: re-decode when the 16 px physical bucket grows after real layout, resize or DPI change, keep
    the previous lease during replacement, and deduplicate repeated requests for the same bucket.
- Collections and lifecycle
  - Support enumerable replacement and observable Add, Remove, Move, Replace and Reset while reusing unchanged entries.
  - Re-materialize on reattach, release Full leases when the preview host closes, and release Full/Thumbnail leases on detach.
- Titles
  - Resolve titles in the order explicit preview/window title, item title, resolver and empty state.

## 2026-07-23

- Behavior
  - Make preview dialog title alignment default to `WindowCenter` on Windows, Linux and macOS while preserving explicit `TitleAlignment` projection to `ImagePreviewerTitleBar`.
  - Paint the dialog image viewer with the dialog background so Windows CSD maximize/restore frames do not expose the generic window underlay.
  - Suppress preview image transform transitions during dialog window state/size transitions so maximize/restore does not combine outer window resizing with inner image translate animation.
- Docs
  - Document the preview dialog title alignment invariant and its boundary with the shared `WindowTitleBarLayoutPanel` algorithm.
  - Document the dialog-only viewer background and transform suppression boundary for Windows CSD state transitions.

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
