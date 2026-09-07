# Tooltip Changelog

本文档记录 Tooltip 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-09-07

- API
  - Expose `root` / `container` / `arrow` Semantic Parts on `ToolTip`, aligned with the upstream antd Tooltip semantic DOM.
  - Declare `container` and `arrow` with `CrossNestedOwners=true`, routing through the `.semantic-scope-arrow-decorated-box` scope anchor into the shared `ArrowDecoratedBox` template; `arrow` is `Optional`.
- Theme
  - Add the `.semantic-scope-arrow-decorated-box` scope anchor to the `ArrowDecoratedBox#PART_ArrowDecorator` node in `ToolTipTheme.axaml`.
  - Add `.semantic-container` / `.semantic-arrow` markers to `Border#PART_ContentDecorator` / `ArrowIndicator#PART_ArrowIndicator` in the shared `ArrowDecoratedBoxTheme.axaml`.
- Docs
  - Add `semantic-part.md` as the single source for the Tooltip Semantic Part contract.
  - Re-scope overview's semantic region summary to `root` / `container` / `arrow` and link the new contract; document the descriptor↔marker mapping in implementation.
- Gallery
  - Add a Semantic Parts tab to the Tooltip showcase with a `SemanticPartPreview` and localized part descriptions.
  - Center the Semantic Parts preview `ToolTip` (`HorizontalAlignment="Center"`) so the pill hugs its content instead of stretching to `ToolTipMaxWidth`.
  - Add a `Custom Semantic Part styling` example (`SemanticPartStyleTitle` / `SemanticPartStyleDescription`) that customizes `container` via the generated `ToolTipContainerStyle` and hides `arrow` via `ToolTip.IsArrowVisible=False` on the trigger buttons, aligned with the antd `style-class` demo.
- Fix
  - The popup shadow now follows the visible container `Border` radius instead of the `ArrowDecoratedBox`'s own `CornerRadius`: overriding only `ToolTipContainerStyle`'s `CornerRadius` previously left the shadow mask at `ToolTipCornerRadius`, exposing white page corners behind the rounded pill.

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record ToolTip as the semantic owner for Tooltip.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-08-19

- Architecture
  - Define `ToolTip.IsOpen` as a declarative desired-open state reconciled against `Tip` readiness and host visual-tree attachment, replacing edge-triggered open-on-set semantics.
  - Establish a single idempotent reconciliation entry as the only path that changes the physical popup state; `ToolTipService` and other callers only write `IsOpen`.
  - `Tip` not being ready no longer resets `IsOpen`; only `ToolTipOpening` cancellation and external popup close write `IsOpen` back to `false`.
  - Host detach closes the physical popup while preserving `IsOpen`; re-attachment reopens through reconciliation.
- Implementation
  - Implement `ReconcileOpenState` in `ToolTip` as the single idempotent entry, driven by `IsOpen`/`Tip` changes and a self-limited one-shot host `AttachedToVisualTree` subscription.
  - Defer popup-closed convergence to after the current detach/attach pass, distinguishing lifecycle closes from external closes by actual host attachment state.
- API
  - Add `TextWrapping` (default `Wrap`) and `TextTrimming` (default `None`) attached properties controlling tip text layout within the maximum width constraint; they propagate to `PART_ContentPresenter` through live bindings while the popup is open.
  - Define the tip-instance customization model: when `Tip` is a `ToolTip` instance, presentation attached properties explicitly set on the instance take precedence over host values, unset ones fall back to the host; lifecycle attached properties stay host-owned.
- Theme
  - `PART_ContentPresenter` declares `TextBlock.TextWrapping="Wrap"` so long tip text wraps within `ToolTipMaxWidth` instead of being clipped.
- Docs
  - Document the ToolTip attached-property contract surface and the `ToolTipOpening`/`ToolTipClosing` routed events in overview.
  - Correct the `ToolTipService` source location in the implementation source index.
  - Document the tip text layout contract in overview and clarify the wrap semantics of `ToolTipMaxWidth` in token.md.

## 2026-08-03

- Architecture
  - Replace OverflowTip's dependency on TextBox visual descendants with an internal text viewport metric published by the input owner.
  - Define reactive viewport updates, template reapply cleanup and the compatibility fallback for third-party Avalonia TextBox templates.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Tooltip`.
  - Align generated output paths with `controls/tooltip/index-cn.md` and `controls/tooltip/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Tooltip desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Tooltip desktop architecture documentation under `docs/controls/desktop/data-display/tooltip/overview.md`.
  - Add Tooltip implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Tooltip control-level changelog.
  - Add Tooltip Token documentation covering ToolTipToken.
