# LineEdit Changelog

本文档记录 LineEdit 输入控件家族级设计、API、主题契约、Token 和实现结构的变化。它用于维护控件设计历史，不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

记录原则：

- 只记录会影响 LineEdit 家族设计理解、兼容边界、实现架构或维护方式的变化。
- 不记录临时讨论、纯格式化或没有长期价值的实现细节。
- 架构文档始终描述最新设计状态；历史变化记录在本文档。

## 2026-08-24

- Theme
  - Make the TextBox input frame fill the width allocated by its owner so placeholder and text measurement cannot resize the visible input surface.
- Tests
  - Cover stable TextBox frame width across placeholder, short-text and long-text states.

## 2026-08-03

- Architecture
  - Define an internal text viewport metric owned by TextBox/TextArea so consumers can react to effective text width without traversing input templates.
  - Define viewport, padding, presenter margin and template reapply lifecycle as the single source of truth for input text width.

## 2026-08-23

- Architecture
  - Establish `AbstractTextInput` as the shared text-input logic owner for `TextBox`, `LineEdit` and `TextArea`.
  - Establish `InputControlFrame` as the shared input-surface owner for variant, effective status, border, background, corner, shadow, CompactSpace and motion.
  - Define `AddOnDecoratedBox` and its specialized descendants as layout extensions of `InputControlFrame`; they no longer own duplicated input-surface status selectors.
  - Define template-owned events and subscriptions as template-lifetime resources replaced on template reapply, while external Form feedback subscriptions follow logical attach/detach.
- API
  - Align `StyleVariant`, `Status`, clear, count, Form, native validation and CompactSpace semantics across the three base text-input controls.
  - Define `NativeValidationStatus`, `FormStatus`, `ExplicitStatus` and the `EffectiveStatus` priority used by all input surfaces.
- Theme
  - Move shared input-surface values to `SharedToken` and the frame theme; retain control tokens only for stable text/layout or TextArea resize differences.
  - Move stable clear, reveal, feedback, inner-right and count state projection from C# relay bindings to AXAML template or typed ancestor bindings.
- Tests
  - Cover logical detach/reattach for the complete `AbstractTextInput` descendant set and AutoComplete composition hosts.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `LineEdit`.
  - Align generated output paths with `controls/line-edit/index-cn.md` and `controls/line-edit/semantic-cn.md`.

## 2026-06-21

- Docs
  - Establish LineEdit desktop architecture documentation under `docs/controls/desktop/data-entry/line-edit/overview.md`.
  - Add LineEdit implementation documentation covering TextBox, LineEdit, SearchEdit, TextArea, template hookup, Form, CompactSpace and resize flows.
  - Add LineEdit Token documentation for `LineEditToken` and `TextAreaToken`.
  - Add LineEdit changelog and link the document set from the Data Entry category entry.
  - Document `CustomizableSizeType.Custom` as a Middle-baseline custom size path for LineEdit family controls.
  - Link SearchEdit's dedicated control documentation and clarify that SearchEdit's right external add-on slot is occupied by the search button.
