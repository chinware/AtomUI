# GroupBox Changelog

本文档记录 GroupBox 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-06-18

- Docs
  - 建立 GroupBox 控件文档目录，补齐 `overview.md`、`token.md` 和 `changelog.md`。
  - 记录 GroupBox Header、Content、Theme、Token、模板节点和兼容性不变量。
  - 明确 Header 缺口渲染模型：透明背景下标题区域不应依赖背景遮挡边框线。
- Theme
  - 明确 `PART_HeaderContent` 是 Header 缺口计算的稳定模板节点。
- Token
  - 按内容区域、Header 结构和 fieldset 语义分类记录 GroupBox Token 边界。

