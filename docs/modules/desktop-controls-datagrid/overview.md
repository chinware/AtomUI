# AtomUI.Desktop.Controls.DataGrid 模块概览

`AtomUI.Desktop.Controls.DataGrid` 是桌面 DataGrid 独立包，RootNamespace 为 `AtomUI.Desktop.Controls`。它依赖 `AtomUI.Desktop.Controls`，并以 Analyzer 方式引用 `AtomUI.Generator`。

## 职责

- 提供 DataGrid 主控件、行、列、单元格、表头、选择列、操作列、拖拽重排等能力。
- 提供 DataGrid 自己的数据视图、排序、分组、过滤描述。
- 提供 DataGrid 行拖动会话和可选 CollectionView 移动能力；交互状态由 DataGrid 持有，数据移动由
  CollectionView 按 View 索引提交。
- 提供 DataGrid Token、主题和本地化资源。
- 通过 `UseDesktopDataGrid()` 注册到 AtomUI 主题系统。

## 关键目录

| 目录 | 说明 |
|---|---|
| `Column/` | 列基类、文本列、模板列、选择列、操作列、表头、排序和过滤 |
| `Row/` | 行、行头、行详情、分组行 |
| `Cell/` | 单元格和单元格 Presenter |
| `Data/` | DataGrid 专用 CollectionView、排序、分组、过滤和可选移动能力 |
| `Themes/` | DataGrid 控件、行、列、单元格主题 |
| `Localization/` | DataGrid 多语言资源 |
| `Utils/` | 冻结列网格、键盘、视觉树和校验辅助 |

## 注册入口

`ThemeManagerBuilderExtensions.UseDesktopDataGrid()` 会注册：

- `GeneratedControlPackageRegistration` 产生的 exact Control descriptor、可选 Own Token schema 和强类型 TokenResource。
- `GeneratedControlThemeAssetManifest` 中的独立 DataGrid 主题叶子，并通过 `AtomUIDataGridThemesProvider` 接入 Styles。
- 该包生成的 Language Provider。

注册入口不维护 Token 类型列表、主题聚合 AXAML 或逐 Control 清单。

## 行重排边界

- `DataGrid` 持有每个实例唯一的行拖动会话，Pointer、源行、源项目、CollectionView 和目标索引不能跨实例共享。
- `DataGridRowReorderHandle` 负责 Pointer 输入，`DataGridRowsPresenter` 负责 ghost row，CollectionView 负责移动提交。
- `IDataGridCollectionViewMoveSupport` 是独立 opt-in 接口，不修改既有 `IDataGridCollectionView` 契约。
- 内置 `DataGridCollectionView` 只对无排序、过滤、分组、分页和编辑事务的可变平面列表提供移动能力。
- 捕获丢失、owner 变化、行回收、模板重建、列移除和 detach 都必须通过统一取消路径释放拖动状态。

具体 DataGrid 设计与 API 契约见
[DataGrid 桌面版架构设计](../../controls/desktop/data-display/data-grid/overview.md)，内部实现边界见
[DataGrid 桌面版实现原理](../../controls/desktop/data-display/data-grid/implementation.md)。
