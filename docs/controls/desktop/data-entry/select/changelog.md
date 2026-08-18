# Select Changelog

本文档记录 Select 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-08-18

- Design
  - Define a single active candidate shared by pointer movement and keyboard navigation across `Single`, `Multiple` and `Tags` modes.
  - Separate active candidate, committed selection and Avalonia pointer hit state so the visual target and `Enter` commit target remain identical.
  - Define pointer hot-path, keyboard scrolling, virtualization projection, lifecycle invalidation and selected-visual precedence invariants.
- Docs
  - Add `candidate-interaction-design.md` and synchronize the Select overview and implementation maintenance boundaries.

## 2026-07-05

- API
  - Add `IsShowOverflowTip`, `OverflowTipDelay` and `OverflowTipPlacement` to the Select family input surface for selected result overflow tooltips.
- Theme
  - Wire overflow tooltip behavior and placement to single selected text and multiple selected tags through the shared `OverflowTip` attached behavior.
  - Align single selected text overflow tooltip against the outer `SelectAddOnDecoratedBox` instead of the padded inner text node.
- Docs
  - Document the overflow tooltip API, template boundary and lifecycle ownership.

## 2026-07-03

- Docs
  - Document the root ownership model for Tags dynamic options: user option sources, runtime dynamic options and effective candidate options are separate state layers.
  - Clarify that Tags runtime options must not be written into user `OptionsSource` or XAML child `Options`, preserving the ItemsSource contract.
  - Add verification expectations for `Mode=Tags` with `OptionsSource` and custom tag creation.
- Implementation
  - Move Tags runtime options into Select-owned internal state and bind the candidate list to effective options built from user options plus runtime options.
  - Preserve selected runtime tags across `OptionsSource` replacement and remap them to formal user options when an identity match appears.
- Tests
  - Add regression coverage for `Mode=Tags` with `OptionsSource`, custom tag creation and source replacement remapping.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Select`.
  - Align generated output paths with `controls/select/index-cn.md` and `controls/select/semantic-cn.md`.

## 2026-06-21

- Docs
  - Add Select control documentation set with architecture overview, implementation notes, Token design and changelog.
  - Document Select mode, option source, selection, filtering, async loading, popup lifecycle, Form / CompactSpace integration and compatibility invariants.
  - Add Select to the Data Entry control documentation index.
- Implementation
  - Reorder `Select` and `AbstractSelect` so control-owned public/protected API members stay before explicit interface regions, with private implementation methods after those contract regions.
  - Consolidate internal drop-down state restoration and candidate selection sync suppression into paired helper paths.
  - Move stable Select template part state binding to AXAML compiled ancestor binding; keep C# `BindUtils.RelayBind` only for AddOnDecoratedBox to SelectHandle hover / pressed sibling part coordination.
- Tests
  - Add Select behavior coverage for option replacement selection preservation, default value selection, Form value mapping and candidate sync suppression recovery.
  - Add Select template binding coverage for AXAML-owned right add-on / count / handle state and the remaining AddOnDecoratedBox hover / pressed relay binding.
