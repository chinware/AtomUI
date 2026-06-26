# TreeView Changelog

本文档记录 TreeView 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `TreeView`.
  - Align generated output paths with `controls/tree-view/index-cn.md` and `controls/tree-view/semantic-cn.md`.

## 2026-06-22

- Docs
  - 补充 TreeView 节点模型分层设计，明确 `TreeItemNode` 保持轻量 POCO / record 定位，不直接改造成 `AvaloniaObject`。
  - 记录 `BindableTreeItemNode` 绑定型节点设计边界，用于 XAML binding target、`DynamicResource` 和动态资源场景。
  - 补充绑定型节点的 scoped resource host、owner attach/release、属性同步和容器生命周期约束。
- Implementation
  - 新增 `BindableTreeItemNode`，通过 Avalonia DirectProperty 承载节点属性，并使用 scoped resource host generator 管理动态资源宿主。
  - TreeView / TreeViewItem 生成容器时接入绑定型节点的资源宿主 attach、属性同步、checked / selected / expanded 回写和容器 clear 释放。

## 2026-06-19

- Docs
  - 新增 TreeView 桌面版架构设计文档，记录控件定位、公共契约、行为状态模型、视觉主题模型、兼容性不变量和专项模型。
  - 新增 TreeView 桌面版实现原理文档，记录源码职责、默认状态回放、路径遍历、勾选同步、过滤、异步加载、拖拽和维护不变量。
  - 新增 TreeView Token 设计文档，记录节点尺寸、状态色、结构间距、拖拽和过滤 Token 边界。
  - 在 Data Display 分类入口中登记 TreeView 文档。
