# GroupBox Changelog

本文档记录 GroupBox 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-06-19

- Docs
  - 新增 `implementation.md`，记录 GroupBox template part 接入、Header 缺口几何、自绘边框和维护不变量。
  - 将 `overview.md` 收敛为控件定位、公共契约、行为状态、视觉主题模型和验证入口。
  - 在 Data Display 分类入口中登记 GroupBox 实现原理文档。

## 2026-06-18

- Docs
  - 建立 GroupBox 控件文档目录，补齐 `overview.md`、`token.md` 和 `changelog.md`。
  - 记录 GroupBox Header、Content、Theme、Token、模板节点和兼容性不变量。
  - 明确 Header 缺口渲染模型：透明背景下标题区域不应依赖背景遮挡边框线。
- Theme
  - 明确 `PART_HeaderContent` 是 Header 缺口计算的稳定模板节点。
- Token
  - 按内容区域、Header 结构和 fieldset 语义分类记录 GroupBox Token 边界。
- Fix
  - 将 Header 缺口从背景遮挡改为边框几何排除，修复 `Background="Transparent"` 时标题下方露出边框短线的问题。
