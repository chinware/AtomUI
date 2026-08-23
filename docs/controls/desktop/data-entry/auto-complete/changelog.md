# AutoComplete Changelog

本文档记录 AutoComplete 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-23

- Architecture
  - Align `AutoCompleteLineEditBox`, `AutoCompleteSearchEditBox` and `AutoCompleteTextAreaBox` with the shared `AbstractTextInput` and `InputControlFrame` layers.
  - Keep candidate, filter, popup and async state in AutoComplete while delegating input-surface status to the shared frame.

## 2026-08-19

- Behavior
  - Unify pointer and keyboard candidate navigation through the shared `CandidateList` active candidate owner.
  - Keep pointer movement non-committing and non-scrolling while ensuring `Enter` commits the single visual active candidate.
- Theme
  - Stop inherited `:pointerover` styling from rendering a second candidate highlight; committed selection remains visually dominant.
- Tests
  - Add popup integration coverage for pointer migration followed by `Enter` commit.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `AutoComplete`.
  - Align generated output paths with `controls/auto-complete/index-cn.md` and `controls/auto-complete/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete AutoComplete desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish AutoComplete desktop architecture documentation under `docs/controls/desktop/data-entry/auto-complete/overview.md`.
  - Add AutoComplete implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add AutoComplete control-level changelog.
  - Add AutoComplete Token documentation covering AutoCompleteToken.
