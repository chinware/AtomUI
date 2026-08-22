# Pagination Changelog

本文档记录 Pagination 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-22

- Semantic Part
  - 为 `Pagination` 与 `SimplePagination` 公开 `root` + `item` Semantic Part，对齐 Ant Design 6 的
    `PaginationSemanticType`（since 6.0.0）；新增 `Pagination.SemanticParts.cs` 与
    `SimplePagination.SemanticParts.cs` descriptor，生成 `PaginationItemStyle` 与
    `SimplePaginationItemStyle`（`AtomUI.Theme.Styling`）。
  - `Pagination.item` 为运行时标记（`RuntimeCreated`），路由经 `semantic-scope-nav` 作用域；Ellipsis 单元格
    动态移除 `semantic-item` marker，对应上游 jump-prev / jump-next 不接受 `styles.item`。`SimplePagination.item`
    为静态标记，只覆盖上一页/下一页，排除快速跳转输入与信息文本。
  - `SimplePagination` 新增 AtomUI 扩展的 `info` Part（`TextBlock`，静态标记），覆盖
    `PART_InfoIndicator`（"当前页 / 总页数"），生成 `SimplePaginationInfoStyle` 用于格式化分页信息文本；
    上游 `PaginationSemanticType` 无对应成员。
  - 新增 `docs/controls/desktop/navigation/pagination/semantic-part.md`，overview 与 implementation 同步链接。
- API
  - `AbstractPagination` 新增 `BorderDashArray` / `BorderDashOffset` 根框架属性，补齐 `styles.root` 的虚线
    边框定制面；`Pagination` 与 `SimplePagination` 共用。
- Theme
  - `PaginationTheme.axaml` 在 `PART_Nav` 上声明 `Classes.semantic-scope-nav="True"` 作用域标记；
    `SimplePaginationTheme.axaml` 在上一页/下一页节点上声明 `Classes.semantic-item="True"` 静态标记，
    并在 `PART_InfoIndicator` 上声明 `Classes.semantic-info="True"` 静态标记。
  - 两个根模板在根布局外包裹 TemplateBind 根视觉属性的 `atom:DashedBorder`：`Background` /
    `BackgroundSizing` / `BorderBrush` / `BorderThickness` / `CornerRadius` / `Padding` 来自
    `TemplatedControl`，`StrokeDashArray` / `StrokeDaskOffset` 来自 `BorderDashArray` / `BorderDashOffset`，
    使 `root` Part 视觉定制（含虚线边框）可渲染，默认值不改变既有外观。
- Gallery
  - Showcase 迁移到 `GalleryShowCaseHost`，增加 Pagination 与 SimplePagination 两个 `SemanticPartPreview`
    （SimplePagination 预览含 `root` / `item` / `info` 三个 Part 描述）以及 `pagination-semantic-part` 语义样式
    示例：两行 `Pagination` 共享虚线 root 边框与内边距，对象式 `styles.item` 圆角、函数式
    （`SizeType=Small`）`styles.item` 背景与间距，并用嵌套 `^:selected` 保留主题选中态，整体对齐 antd
    style-class 示例。

## 2026-07-06

- API
  - 将 `CurrentPage` 和 `PageSize` 设为默认 `TwoWay` 受控分页状态。
- Implementation
  - 内部页码和页大小更新改为 `SetCurrentValue`，避免破坏外部 binding owner。
- Gallery
  - 增加默认双向绑定示例，标记为 `v6.0.8`。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Pagination`.
  - Align generated output paths with `controls/pagination/index-cn.md` and `controls/pagination/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Pagination desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Pagination desktop architecture documentation under `docs/controls/desktop/navigation/pagination/overview.md`.
  - Add Pagination implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Pagination control-level changelog.
  - Add Pagination Token documentation covering PaginationToken.
