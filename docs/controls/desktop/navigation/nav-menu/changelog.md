# NavMenu Changelog

本文档记录 NavMenu 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-06-19

- Docs
  - 按控件文档规范补齐 NavMenu 桌面版架构设计、Token 设计和控件级 changelog。
  - 在 Navigation 分类入口中登记 NavMenu 文档。
- Theme
  - 记录 `IsItemBackgroundEnabled` 对 item 背景块和 inline submenu 背景块的控制边界。
  - 明确 root background、popup background、header background 和 inline submenu background 的职责分离。
- Token
  - 记录 NavMenuToken 的颜色、间距、popup、horizontal、dark 和 danger 分类。
  - 明确 `VerticalChildItemsMargin` 只服务背景模式下的 inline submenu 背景块外距。

