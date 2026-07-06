# ListView Changelog

本文档记录 ListView 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

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
