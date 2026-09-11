# TreeSelect Changelog

本文档记录 TreeSelect 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-09-05

- Theme
  - Apply the reused SelectToken `MultiModePrefixIndent*` values as the extra left `Margin` of `PART_ContentLeftAddOn` in `TreeSelectAddOnDecoratedBoxTheme` for `IsMultiple=True` with a non-empty selection, aligning the TreeSelect multiple-mode prefix with its single-mode horizontal padding.

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record AbstractSelect as the semantic owner for TreeSelect.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-08-23

- Architecture
  - Align TreeSelect and `TreeSelectAddOnDecoratedBox` with the shared `InputControlFrame` surface owner.
  - Keep tree selection, filtering and popup state in TreeSelect while reusing shared input status and Form/native validation semantics.

## 2026-07-05

- API
  - Make `SelectedItem` and `SelectedItems` default to `BindingMode.TwoWay` and enable Avalonia data validation.
- Implementation
  - Subscribe to `SelectedItems` `INotifyCollectionChanged` sources while attached and refresh tags, count, Form value notifications, max-count state, and popup TreeView selection / checked items on collection mutation.
- Gallery
  - Add a `v6.0.8` binding example that demonstrates single and multiple TreeSelect selection binding.
- Tests
  - Add TreeSelect selection binding regression coverage for default binding mode, data validation metadata, collection mutation, and checkable mode.

- API
  - Add inherited `IsShowOverflowTip`, `OverflowTipDelay` and `OverflowTipPlacement` support for selected result overflow tooltips.
- Theme
  - Wire single selected text and multiple selected tags to the shared `OverflowTip` attached behavior, including tooltip placement.
  - Align single selected text overflow tooltip against the outer `TreeSelectAddOnDecoratedBox` instead of the padded inner text node.
- Docs
  - Document the overflow tooltip API and template ownership boundary.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `TreeSelect`.
  - Align generated output paths with `controls/tree-select/index-cn.md` and `controls/tree-select/semantic-cn.md`.

## 2026-06-21

- Docs
  - Add TreeSelect control documentation set with architecture overview, implementation notes, Token design and changelog.
  - Document TreeSelect tree data, selection, checkable mode, filter strategy, popup lifecycle, Form / CompactSpace integration and compatibility invariants.
  - Add TreeSelect to the Data Entry control documentation index.
- Implementation
  - Reorder `TreeSelect` so control-owned public/protected API members stay before the Form interface region, with private implementation methods after the contract region.
  - Move stable TreeSelect right add-on, count and handle state binding to AXAML compiled ancestor binding.
  - Keep C# `BindUtils.RelayBind` only for AddOnDecoratedBox to SelectHandle hover / pressed sibling part coordination.
- Tests
  - Add TreeSelect template binding coverage for AXAML-owned right add-on / count / handle state and the remaining AddOnDecoratedBox hover / pressed relay binding.
