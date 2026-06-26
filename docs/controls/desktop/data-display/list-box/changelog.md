# ListBox Changelog

本文档记录 ListBox 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `ListBox`.
  - Align generated output paths with `controls/list-box/index-cn.md` and `controls/list-box/semantic-cn.md`.

## 2026-06-20

- Docs
  - 新增 ListBox 桌面版架构设计文档，记录控件定位、公共契约、行为状态模型、视觉主题模型、兼容性不变量和专项模型。
  - 新增 ListBox 桌面版实现原理文档，记录源码职责、容器生命周期、过滤、点击、空状态、虚拟化上下文和 CandidateList 复用边界。
  - 新增 ListBox Token 设计文档，记录 root 结构、条目文字、条目背景、条目间距、选中指示器和过滤 Token 边界。
  - 在 Data Display 分类入口中登记 ListBox 文档。
