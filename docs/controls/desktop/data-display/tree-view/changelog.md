# TreeView Changelog

本文档记录 TreeView 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-06-19

- Docs
  - 新增 TreeView 桌面版架构设计文档，记录控件定位、公共契约、行为状态模型、视觉主题模型、兼容性不变量和专项模型。
  - 新增 TreeView 桌面版实现原理文档，记录源码职责、默认状态回放、路径遍历、勾选同步、过滤、异步加载、拖拽和维护不变量。
  - 新增 TreeView Token 设计文档，记录节点尺寸、状态色、结构间距、拖拽和过滤 Token 边界。
  - 在 Data Display 分类入口中登记 TreeView 文档。
