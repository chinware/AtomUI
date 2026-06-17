# Masonry Changelog

本文档记录 Masonry 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-06-17

- Docs
  - 新增 Masonry 桌面版架构设计文档。
  - 明确 Masonry 采用 Avalonia 原生布局容器模型，作为布局原语而非数据组件。
  - 明确 Masonry 不承担数据源管理、内容渲染或项模板职责，数据绑定场景通过 `ItemsControl.ItemsPanel` 组合实现。
- API
  - 定义 `MasonryPanel` 布局原语、核心布局属性、子项 attached property 和布局变化事件的设计契约。
- Theme
  - 明确 Masonry 默认不需要 `ControlTemplate`，Theme 只提供布局默认值，不绘制子项外观。
- Token
  - 明确 Masonry 当前不定义专属 Token，不创建 `token.md`。
