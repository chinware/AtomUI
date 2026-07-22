# ListView Changelog

本文档记录 ListView 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-07-22

- Design
  - 定义 source entry / EntryId 选择模型，统一 source index、view projection、选择状态和容器状态的映射契约。
  - 明确 Reset 与 ItemsSource 替换仅通过唯一 item key 恢复选择，item equality 不参与源条目标识。
  - 只有具备 internal entry bridge 的 collection view 可以直接接入 ListView，其他 view 从 `SourceCollection` 重新归一。
- API
  - 将 `Selection` 定义为 ListView 持有的选择状态 owner；`SelectedItem`、`SelectedItems` 和 `SelectedValue` 为只读投影。
  - 选择索引统一使用 source index，并增加 `SelectedIndexes` 与 `ItemKeySelector` 契约。
  - `IListItemData` 不再承载运行时 `IsSelected`；`ListItemData` 与 `GroupListItemData` 使用可变 class 实体语义。

## 2026-07-06

- API
  - 将 `SelectedItems` 明确为默认 `TwoWay` 的受控选择集合，并接入 Avalonia data validation。
- Gallery
  - 增加 `SelectedItems` 默认双向绑定示例，标记为 `v6.0.8`。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `ListView`.
  - Align generated output paths with `controls/list-view/index-cn.md` and `controls/list-view/semantic-cn.md`.

## 2026-06-20

- Docs
  - 新增 ListView 桌面版架构设计文档，记录控件定位、公共契约、行为状态模型、视觉主题模型、兼容性不变量和专项模型。
  - 新增 ListView 桌面版实现原理文档，记录源码职责、ItemsSource 归一、collection view 协作、选择、分页、分组、过滤、虚拟化上下文和主题接入。
  - 新增 ListView Token 设计文档，记录 root 结构、条目文字、条目背景、条目间距、分页、分组和选中标记 Token 边界。
  - 补充分组设计说明，明确 group key、`GroupListItemData`、`GroupItemTemplate`、selection source、分页组合和虚拟化清理边界。
  - 在 Data Display 分类入口中登记 ListView 文档。
