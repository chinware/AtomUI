# TreeView Changelog

本文档记录 TreeView 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record TreeView as the semantic owner, with TreeViewFlyout used only as the relay adapter.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-07-08

- Docs
  - 明确 TreeView 拖拽重排采用数据源驱动设计：UI 容器只负责命中和视觉状态，结构修改由内部数据控制器操作 root 数据源或节点 `Children`。
  - 记录轻量 `TreeNodeIndex` 的性能边界：drop 定位使用增量索引，拖拽过程中不按节点引用全树扫描。
  - 补充拖拽 move 的状态一致性要求：节点移动不按删除清理选中、勾选和展开状态，跨父级移动必须同步 parent node、权威集合和索引。

## 2026-07-05

- Implementation
  - 修复多选 TreeView 在 ItemsSource 变化或首次容器状态回放时优先恢复 `SelectedItem`，导致 `SelectedItems` 被折叠成单选的问题；多选模式现在以 `SelectedItems` 路径作为运行期选择恢复的权威来源。
  - 修复 `IFormItemAware.SetFormValue` 写入 TreeView 选择值时把节点对象转换为字符串的问题；单选保持 `SelectedItem` 节点实例，多选保持 `SelectedItems` 列表实例。
- Gallery
  - 新增 `SelectedItem` / `SelectedItems` 绑定示例，并标记 `v6.0.8`。
- Tests
  - 新增 TreeView 多选 ItemsSource 回放优先级测试，覆盖 `SelectedItem` 与 `SelectedItems` 同时存在时的恢复顺序。
  - 新增 TreeView Form 值读写回归测试，覆盖单选、多选、读取和清空语义。

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
