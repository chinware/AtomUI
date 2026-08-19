# Tooltip Changelog

本文档记录 Tooltip 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

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
