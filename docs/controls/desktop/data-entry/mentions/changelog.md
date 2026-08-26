# Mentions Changelog

本文档记录 Mentions 控件级设计、API、主题契约、Token 和实现结构的变化。它用于维护控件设计历史，不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

记录原则：

- 只记录会影响 Mentions 设计理解、兼容边界、实现架构或维护方式的变化。
- 不记录临时讨论、纯格式化或没有长期价值的实现细节。
- 架构文档始终描述最新设计状态；历史变化记录在本文档。

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record Mentions as the semantic owner for Mentions.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-08-23

- Architecture
  - Align `MentionTextArea` with the shared `AbstractTextInput` logic layer and `InputControlFrame` input-surface layer.
  - Keep trigger, candidate, popup and async state in Mentions while reusing the shared validation and Form status pipeline.

## 2026-08-19

- Behavior
  - Unify pointer and keyboard candidate navigation through the shared `CandidateList` active candidate owner.
  - Keep the current mention trigger context intact while `Enter` inserts the single visual active candidate.
- Theme
  - Stop inherited `:pointerover` styling from rendering a second candidate highlight; committed selection remains visually dominant.
- Tests
  - Add trigger-popup integration coverage for pointer migration followed by mention insertion.

## 2026-07-06

- API
  - Make `Value` a default `TwoWay` Form value and keep validation integrated with Avalonia `DataValidationErrors`.
- Gallery
  - Add a `v6.0.8` `Value` binding example.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Mentions`.
  - Align generated output paths with `controls/mentions/index-cn.md` and `controls/mentions/semantic-cn.md`.

## 2026-06-21

- Docs
  - Establish Mentions desktop architecture documentation under `docs/controls/desktop/data-entry/mentions/overview.md`.
  - Add Mentions implementation documentation covering trigger detection, candidate popup, filtering, async loading, option insertion, Form integration and lifecycle boundaries.
  - Add Mentions Token documentation for `PopupContentPadding`, `OptionHeight` and `MinPopupWidth`.
  - Add Mentions changelog and link the document set from the Data Entry category entry.
