# Expander Changelog

本文档记录 Expander 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-09-10

- Design
  - 纳入[内容展开与收起动效设计](../../../../architecture/systems/control-infrastructure/content-expansion.md)，定义四方向的尺寸轴、内容锚定、稳定排版、请求反转和生命周期归一契约。
  - 保留 `IsExpanded`、方向 selector、标题/内容分隔线和有效主题时长的现有所有权，不增加手风琴状态或公开动效配置。
- Implementation
  - 展开进度由动画执行器的私有附加属性持有，基础 actor 通过内部 `IMotionActorLayout` 协作，分离通用布局与内容开合职责。
  - 接入 Core 内部 `ContentExpansionAnimator`，通过原生 `Animation` 同步驱动内部布局进度与透明度，四方向保持完整内容排版及标题侧锚定。
  - 关闭动效、方向切换、模板替换及 detach / reattach 统一收敛到当前展开状态，只释放机制自身资源，保留自定义尺寸与变换。
- Docs
  - 同步四方向执行链、生命周期和帧级验证要求。

## 2026-07-13

- Theme
  - Move the internal Header/Content separator from `PART_HeaderDecorator` to an unnamed Content border inside `PART_ContentMotionActor`, preserving public API, stable template parts, Token contracts, and Borderless/Ghost behavior across all four expand directions.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Expander`.
  - Align generated output paths with `controls/expander/index-cn.md` and `controls/expander/semantic-cn.md`.

## 2026-06-22

- Docs
  - 建立 Expander 控件文档目录，补齐 `overview.md`、`implementation.md`、`token.md` 和 `changelog.md`。
  - 记录 Expander 的公共契约、触发区域、展开方向、动效状态机、自定义 padding、主题结构、Token 分类和验证策略。
  - 在 Data Display 分类入口中登记 Expander 文档。
