# TabStrip Changelog

本文档记录 TabStrip 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-28

- Behavior
  - Route overflow close requests through `BaseTabStrip.CloseTab`, including mutable `ItemsSource` lists, and reject read-only or fixed-size sources without changing the source collection or flyout.

- Docs
  - Define overflow menu items as alternate presentations of the source `TabStripItem`, including propagation of effective `IsClosable` and `ContentTemplate` semantics.
  - Define `BaseOverflowMenuItemTheme` close-button visibility from `IsClosable`, and require overflow close requests to delegate to `BaseTabStrip.CloseTab` so `Closing`, cancellation, selection, collection and `Closed` semantics remain unified.
  - Require canceled or rejected closes to retain both the source tab and its overflow menu item; only a successful owner close may remove the menu item.

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record BaseTabStrip as the semantic owner for TabStrip, with TabStripScrollViewer used only as the relay adapter.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-08-18

- Behavior
  - Preserve each tab header's `ContentTemplate` in overflow menu items so `ItemTemplate` rendering remains consistent after a tab moves into the overflow menu.

## 2026-07-09

- Docs
  - Define `TabActivationTrigger` for `TabStrip`, with `PointerReleased` as the default pointer activation mode and `PointerPressed` as the opt-in immediate activation mode.
  - Document press/release same-Tab activation semantics, cancellation paths, reorder precedence and verification requirements.
  - Define Tab drag reorder API, events, axis model, collection commit semantics, lifecycle cleanup and verification boundaries for `TabStrip`.
  - Document that reorder must mutate logical `ItemsSource` / `Items` order instead of visual container order, and selection state must follow the same logical item after reorder.
  - Refine the reorder preview as a Chrome-style track-constrained model: dragged tabs move only on the placement main axis, overlapping siblings displace proportionally to avoid empty old slots, half-overlap switches the target index, sibling displacement is animated, the selected indicator follows preview transforms, and the dragged surface remains opaque.
  - Document the vertical placement icon-slot alignment model for mixed icon/no-icon tabs without adding public API or new tokens.
  - Document that default Line `Left` / `Right` spacing and item padding are compact and independent from Card spacing, and that `TabStripPlacement` changes must preserve the selected logical item without refreshing containers.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `TabStrip`.
  - Align generated output paths with `controls/tab-strip/index-cn.md` and `controls/tab-strip/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete TabStrip desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish TabStrip desktop architecture documentation under `docs/controls/desktop/navigation/tab-strip/overview.md`.
  - Add TabStrip implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add TabStrip control-level changelog.
  - Document that TabStrip does not require a dedicated Token document and records its theme dependencies in the overview.
