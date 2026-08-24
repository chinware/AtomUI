# Notification Changelog

本文档记录 Notification 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-24

- Motion
  - Reuse the shared internal `AtomUI.MotionScene.MotionExecutionState` for NotificationCard exit motion scheduling, playback and final `IsClosed` submission while preserving the public `IsClosing` / `IsClosed` contract.
  - Coalesce property-change and template-reapply close scheduling into one execution flow.

## 2026-07-20

- Fix
  - Add `NotificationType.Default` for plain notifications so the default notification path renders without a type icon.
  - Align the close button size, hover background and pressed background with shared text/icon state tokens.
  - Derive `NotificationProgressBg` from primary border hover color to primary color.
  - Align the default auto-close expiration with the documented 4.5 second duration.
  - Reduce the default internal card spacing tokens by one third while preserving the external stack spacing.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Notification`.
  - Align generated output paths with `controls/notification/index-cn.md` and `controls/notification/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Notification desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Notification desktop architecture documentation under `docs/controls/desktop/feedback/notification/overview.md`.
  - Add Notification implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Notification control-level changelog.
  - Add Notification Token documentation covering NotificationToken.
