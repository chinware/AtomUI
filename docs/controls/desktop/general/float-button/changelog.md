# FloatButton Changelog

本文档记录 FloatButton 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-16

- Semantic Parts
  - Add Ant Design-aligned Semantic Part descriptors for `FloatButton`, `BackTopFloatButton` and `FloatButtonGroup`.
  - Add static template markers and generated owner-scoped style contracts without runtime discovery; `FloatButtonTheme` now owns its leaf shape templates so the generator can statically validate markers.
  - Document the lazily materialized trigger-mode `list` part and the exclusion of badge adorners, tooltip and item-level group parts.
- Gallery
  - Add the Semantic Parts Preview and Ant Design-aligned custom styling example using AtomUI `v6.1.3` version metadata.

## 2026-07-09

- Docs
  - Document the FloatButton host command projection model based on Avalonia Button command semantics.
  - Clarify that `FloatButtonGroupHost` trigger controls open/close while child `FloatButton` items own business commands.
  - Define group child `DataContext` inheritance requirements for command bindings inside overlay-hosted groups.
- API
  - Establish `Command` and `CommandParameter` as host-level projected command contracts for `FloatButtonHost` and `BackTopFloatButtonHost`.
- Implementation
  - Define lifecycle and test requirements for host-to-overlay command projection, `CanExecute` behavior and group child command binding.

## 2026-07-06

- API
  - Make `FloatButtonGroup.IsOpen` and inherited `FloatButtonGroupHost.IsOpen` default to `BindingMode.TwoWay`.
- Implementation
  - Use current value updates for group and host open/close interactions so controlled bindings are not replaced by style-priority values.
- Docs
  - Document the controlled open state contract for group and host usage.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `FloatButton`.
  - Align generated output paths with `controls/float-button/index-cn.md` and `controls/float-button/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete FloatButton desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish FloatButton desktop architecture documentation under `docs/controls/desktop/general/float-button/overview.md`.
  - Add FloatButton implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add FloatButton control-level changelog.
  - Add FloatButton Token documentation covering FloatButtonToken.
