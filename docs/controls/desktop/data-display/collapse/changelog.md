# Collapse Changelog

本文档记录 Collapse 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-09-10

- Design
  - 纳入[内容展开与收起动效设计](../../../../architecture/systems/control-infrastructure/content-expansion.md)，定义普通与手风琴模式共用的尺寸/透明度进度、稳定内容裁剪、反转连续性与执行所有权。
  - 保持 selection model、模板分隔线、既有 Token 和 ColorPicker 派生接入边界。
- Implementation
  - 展开进度由动画执行器的私有附加属性持有，基础 actor 通过内部 `IMotionActorLayout` 协作，分离通用布局与内容开合职责。
  - `CollapseItem` 接入 Core 内部 `ContentExpansionAnimator`，以原生 `Animation` 同步驱动内部布局进度和透明度，完整内容按自然尺寸排版并裁剪。
  - 统一取消、反转、首尾帧和生命周期收敛；清理只释放机制自身资源，保留自定义尺寸与变换。
- Docs
  - 同步共享执行链、ColorPicker 直接消费者边界与帧级验证入口。

## 2026-07-13

- Design
  - Define Avalonia selection model as the sole expansion-state owner for normal and accordion modes.
  - Define structural separator ownership so item and content borders no longer depend on selection or motion timing.
- Compatibility
  - Preserve the existing public API, stable template parts, resource keys, Token names and theme values.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Collapse`.
  - Align generated output paths with `controls/collapse/index-cn.md` and `controls/collapse/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Collapse desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Collapse desktop architecture documentation under `docs/controls/desktop/data-display/collapse/overview.md`.
  - Add Collapse implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Collapse control-level changelog.
  - Add Collapse Token documentation covering CollapseToken.
