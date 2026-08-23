# TabControl Changelog

本文档记录 TabControl 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-08-28

- Behavior
  - Route overflow close requests through `BaseTabControl.CloseTab` so `IsClosable`, `Closing`, cancellation, selection, collection and `Closed` semantics remain unified before the menu item is removed.

- Docs
  - Define overflow menu items as alternate presentations of the source `TabItem`, including propagation of effective `IsClosable` and `HeaderTemplate` semantics.
  - Define `BaseOverflowMenuItemTheme` close-button visibility from `IsClosable`, and require overflow close requests to delegate to `BaseTabControl.CloseTab` so `Closing`, cancellation, selection, collection and `Closed` semantics remain unified.
  - Require canceled or rejected closes to retain both the source tab and its overflow menu item; only a successful owner close may remove the menu item.

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record BaseTabControl as the semantic owner for TabControl, with TabControlScrollViewer used only as the relay adapter.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-08-23

- Semantic Part
  - 为 `TabControl` 与 `CardTabControl` 公开 `root` + `content` + `item` Semantic Part（since 6.0），并为
    `CardTabControl` 额外公开 `add` Part；新增 `TabControl.SemanticParts.cs`、`CardTabControl.SemanticParts.cs`
    descriptor，生成 `TabControlItemStyle`、`TabControlContentStyle`、`CardTabControlAddStyle`、
    `CardTabControlContentStyle`、`CardTabControlItemStyle`（`AtomUI.Theme.Styling`）。
  - `item` 为运行时标记（`RuntimeCreated`），路由 `> .semantic-item`，由 owner 在
    `CreateContainerForItemOverride` / `PrepareContainerForItemOverride` 应用到 `TabItem` 容器，直接提供的
    `TabItem` 实例同样获得 marker；`content` 为静态标记，覆盖内容区 `ContentPresenter`。
  - `TabItem` 公开 `root` + `icon` + `label` + `close`，生成 `TabItemIconStyle`、`TabItemLabelStyle`、
    `TabItemCloseStyle`；选中指示墨条、header extra、overflow 菜单项与 Card `LineMask` 不参与 Semantic Part。
  - 新增 `docs/controls/desktop/navigation/tab-control/semantic-part.md`；overview 同步 LLMS 语义区域表与导出
    来源，implementation 同步 Semantic marker 接入点、运行时 marker 同步规则与 Semantic Part 尺寸基线矩阵。
- Theme
  - `TabControlTheme.axaml` 与 `CardTabControlTheme.axaml` 在内容区 `ContentPresenter` 上声明
    `Classes.semantic-content="True"` 静态标记；`CardTabControlTheme.axaml` 在 `PART_AddTabButton` 上声明
    `Classes.semantic-add="True"`。
  - `BaseTabItemTheme.axaml` 与 `CardTabItemTheme.axaml` 在 `ItemIconPresenter`、标题 `ContentPresenter` 与
    `PART_ItemCloseButton` 上声明 `Classes.semantic-icon="True"` / `Classes.semantic-label="True"` /
    `Classes.semantic-close="True"` 静态标记。
- Test
  - 新增 `tests/AtomUI.Desktop.Controls.Tests/TabControl/TabControlSemanticPartTests.cs`，覆盖 descriptor 注册、
    内置主题静态 marker、运行时 item marker、生成 Style 应用、TabItem 子 Part 与 SizeType 尺寸基线。
  - 重写 `tests/AtomUIGallery.Tests/ShowCases/TabControlShowCasePageTests.cs` 并更新展示用例快照，覆盖
    Gallery Semantic Part 预览、样式示例与新增语义本地化文案；同步更新 `CatalogMemberOrder.baseline` 与
    `GalleryCatalogCoverageTests` 的单元计数期望（4175 → 4192），保持枚举、xlf 与编译期翻译契约一致。

## 2026-08-18

- Behavior
  - Preserve each tab header's `HeaderTemplate` in overflow menu items so custom header rendering remains consistent after a tab moves into the overflow menu.

## 2026-07-09

- Docs
  - Define `TabActivationTrigger` for `TabControl`, with `PointerReleased` as the default pointer activation mode and `PointerPressed` as the opt-in immediate activation mode.
  - Document press/release same-Tab activation semantics, cancellation paths, reorder precedence and verification requirements.
  - Define Tab drag reorder API, events, axis model, collection commit semantics, lifecycle cleanup and verification boundaries for `TabControl`.
  - Document that reorder must mutate logical `ItemsSource` / `Items` order instead of visual container order, and selection/content state must follow the same logical item after reorder.
  - Refine the reorder preview as a Chrome-style track-constrained model: dragged tabs move only on the placement main axis, overlapping siblings displace proportionally to avoid empty old slots, half-overlap switches the target index, sibling displacement is animated, the selected indicator follows preview transforms, and the dragged surface remains opaque.
  - Document the vertical placement icon-slot alignment model for mixed icon/no-icon tabs without adding public API or new tokens.
  - Document that default Line `Left` / `Right` spacing and item padding are compact and independent from Card spacing, and that `TabStripPlacement` changes must preserve the selected logical item without refreshing containers.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `TabControl`.
  - Align generated output paths with `controls/tab-control/index-cn.md` and `controls/tab-control/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete TabControl desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish TabControl desktop architecture documentation under `docs/controls/desktop/navigation/tab-control/overview.md`.
  - Add TabControl implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add TabControl control-level changelog.
  - Add TabControl Token documentation covering TabControlToken.
