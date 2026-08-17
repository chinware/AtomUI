# Segmented Changelog

本文档记录 Segmented 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-08-17

- Layout and Theme
  - `OptionButtonGroup` 根渲染补画自身 `Background`（此前该属性被忽略），背景绘制在外框、分隔线与选中描边之下；Calendar Header 的 Month/Year 切换组由此获得白色容器背景且不遮挡边框。

## 2026-07-29

- Design
  - 定义横向和纵向共享同一选择、容器和滑块模型的方向布局契约。
  - 定义垂直 `IsExpanding` 只适配父容器宽度、不扩展父容器高度的组合语义。
  - 定义 `SegmentedShape.Default` / `Round`，Round 统一覆盖轨道、item 和滑块为胶囊几何。
- API
  - 为根控件定义默认 `Horizontal` 的 `Orientation` 和默认 `Default` 的 `Shape`。
- Interaction
  - 定义四方向键按前后顺序循环选择，并跳过 disabled 和 hidden item。
- Layout and Theme
  - 选中滑块尺寸以容器最终 `Bounds.Size` 为准，并在 arrange 完成后校准。
  - Round 不新增 Design Token、Visual、template part 或伪类，Shape 覆盖位于 SizeType 圆角分支之后。
- Gallery
  - 按参考设计源码新增 Vertical、Round Shape 与 Dynamic 示例；Round 示例支持通过 `small`、`medium`、`large` 动态切换胶囊控件尺寸，Dynamic 示例支持运行时追加选项。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Segmented`.
  - Align generated output paths with `controls/segmented/index-cn.md` and `controls/segmented/semantic-cn.md`.

## 2026-06-22

- Docs
  - 建立 Segmented 控件文档目录，补齐 `overview.md`、`implementation.md`、`token.md` 和 `changelog.md`。
  - 记录 Segmented 的公共契约、单选状态、选中滑块、expanding 布局、Custom SizeType、主题结构、Token 分类和验证策略。
  - 在 Data Display 分类入口中登记 Segmented 文档。
