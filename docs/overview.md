# AtomUI 文档总览

本文档目录面向 AtomUI 源码维护者和控件使用者，目标是把项目结构、模块职责、启动注册链路、主题与 Token 体系、具体控件文档分开维护。

## 阅读路径

- 想快速理解项目整体：先读 [architecture/overview.md](architecture/overview.md)，再读 [architecture/dependency-graph.md](architecture/dependency-graph.md)。
- 想新增或修改会影响 trimming / NativeAOT 的代码：先读 [engineering/aot-programming-guidelines.md](engineering/aot-programming-guidelines.md)。
- 想理解某个源码包：从 [modules/](modules/) 下对应模块的 `overview.md` 开始。
- 想查具体控件：从 [controls/overview.md](controls/overview.md) 进入，按桌面端和移动端分层。
- 想查已有专题沉淀：通过本文档索引跳转。

## 文档分层

```text
docs/
├── overview.md
├── architecture/            # 全局架构，不写单个控件使用细节
├── engineering/             # 工程实践与编程规范
├── modules/                 # 项目/包级架构，按 NuGet/源码模块分目录
├── controls/                # 具体控件文档，按平台分目录
└── gallery/                 # Gallery 专题文档，当前不纳入 modules
```

`modules/` 和 `controls/` 的边界需要保持清楚：

- `modules/desktop-controls/` 写 `AtomUI.Desktop.Controls` 这个包如何组织、注册和加载资源。
- `controls/desktop/` 写 Button、Select、TreeView、Dialog、Window 等具体控件。
- `modules/desktop-controls-datagrid/` 和 `modules/desktop-controls-colorpicker/` 写独立包内部架构。
- `controls/desktop/data-display/datagrid.md`、`controls/desktop/data-entry/color-picker.md` 才是具体控件文档的位置。

## 当前模块目录

- [modules/core/overview.md](modules/core/overview.md) - `AtomUI.Core`
- [modules/native/overview.md](modules/native/overview.md) - `AtomUI.Native`
- [modules/controls-shared/overview.md](modules/controls-shared/overview.md) - `AtomUI.Controls.Shared`
- [modules/controls/overview.md](modules/controls/overview.md) - `AtomUI.Controls`
- [modules/desktop-controls/overview.md](modules/desktop-controls/overview.md) - `AtomUI.Desktop.Controls`
- [modules/desktop-controls-datagrid/overview.md](modules/desktop-controls-datagrid/overview.md) - `AtomUI.Desktop.Controls.DataGrid`
- [modules/desktop-controls-colorpicker/overview.md](modules/desktop-controls-colorpicker/overview.md) - `AtomUI.Desktop.Controls.ColorPicker`
- [modules/generator/overview.md](modules/generator/overview.md) - `AtomUI.Generator`
- [modules/icons/overview.md](modules/icons/overview.md) - 图标相关项目
- [modules/fonts/overview.md](modules/fonts/overview.md) - 字体相关项目

`AtomUIGallery` 和性能工具当前作为辅助项目处理，不放入 `modules/`。

## 现有专题文档

- [AsyncLoadingArchitecture.md](AsyncLoadingArchitecture.md) - 异步加载体系
- [FilteringArchitecture.md](FilteringArchitecture.md) - 过滤体系
- [PopupAnchorScopeGuide.md](PopupAnchorScopeGuide.md) - Popup Anchor 作用域检查
- [engineering/aot-programming-guidelines.md](engineering/aot-programming-guidelines.md) - AtomUI AOT 编程规范
- [engineering/windows-native-aot-publish.md](engineering/windows-native-aot-publish.md) - Windows 11 Gallery Native AOT 发布维护手册
- [engineering/linux-native-aot-publish.md](engineering/linux-native-aot-publish.md) - Linux Gallery Native AOT 发布维护手册
- [modules/native/architecture.md](modules/native/architecture.md) - Native 项目架构
- [modules/native/atomui-window-cross-platform-guide.md](modules/native/atomui-window-cross-platform-guide.md) - Window 跨平台定制
- [gallery/organization.md](gallery/organization.md) - Gallery 组织规范
