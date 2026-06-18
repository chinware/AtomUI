# NumericUpDown Changelog

本文档记录 NumericUpDown 控件级设计、API、主题契约、Token 和实现结构的变化。它用于维护控件设计历史，不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

记录原则：

- 只记录会影响 NumericUpDown 设计理解、兼容边界、实现架构或维护方式的变化。
- 不记录临时讨论、纯格式化或没有长期价值的实现细节。
- 架构文档始终描述最新设计状态；历史变化记录在本文档。

## 2026-06-18

- Docs
  - Establish NumericUpDown desktop architecture documentation under `docs/controls/desktop/data-entry/numeric-up-down/overview.md`.
  - Add dedicated NumericUpDown Token design documentation under `docs/controls/desktop/data-entry/numeric-up-down/token.md`.
  - Introduce per-control changelog under `docs/controls/desktop/data-entry/numeric-up-down/changelog.md`.
  - Document the NumericUpDown composition model based on Avalonia NumericUpDown, ButtonSpinner, TextBox, clear button and AddOnDecoratedBox.
  - Document the string mode, floating Handle and Form integration models.
  - Document NumericUpDown compatibility invariants, template parts, Token scope and validation strategy.
  - Document `NumericUpDownMode` / `Mode=Spinner` design, on-demand template switching, zero-cost default path and spinner mode Token reuse.
- Implementation
  - Add public `NumericUpDownMode` and `Mode` API with `Input` as the default value.
  - Implement `Mode=Spinner` with an independent NumericUpDown template containing inline decrease and increase buttons.
  - Reuse `ButtonSpinner` spin handling for the spinner template so min/max, `AllowSpin`, keyboard and wheel semantics remain shared.
  - Add Gallery API metadata and a NumberUpDown spinner mode showcase.
