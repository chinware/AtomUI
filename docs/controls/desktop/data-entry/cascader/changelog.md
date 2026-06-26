# Cascader Changelog

本文档记录 Cascader 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Cascader`.
  - Align generated output paths with `controls/cascader/index-cn.md` and `controls/cascader/semantic-cn.md`.

## 2026-06-23

- Docs
  - 新增 Cascader 控件文档集，记录控件定位、数据节点模型、级联展开、选择 / 勾选、异步加载、过滤、Token 和兼容性不变量。
  - 记录绑定型选项模型边界，明确 `CascaderOption` 保持轻量数据对象定位。
  - 补齐 Cascader 设计文档中的外层 public API、内部 CascaderView 事件、template part、伪类、单选 / 多选状态流、过滤、默认路径、Form、Token 使用和分层验证策略。
  - 在 Data Entry 分类入口登记 Cascader 文档。
- API
  - 新增 `BindableCascaderOption`，用于选项属性需要作为 Avalonia binding target、`DynamicResource` 或 scoped resource host 的场景。
- Implementation
  - `CascaderViewItem` 接入绑定型选项的 owner resource host attach/release、属性同步和 checked / expanded 反写。
  - `CascaderViewLevelList` 在容器准备和清理路径管理绑定型选项生命周期，回收容器时先释放订阅再清空容器值。
- Tests
  - 新增绑定型选项 parent 维护、容器同步、资源宿主优先级和 WeakReference 生命周期覆盖。
