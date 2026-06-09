# 桌面控件文档入口

桌面控件文档按 Ant Design/Gallery 的使用分类组织。每个具体控件文档应说明控件用途、核心 API、主题 Token、数据流、常见坑和源码索引。

## 建议分类

```text
docs/controls/desktop/
├── general/        # Button、Icon、Typography 等
├── layout/         # Space、Grid、Flex、Splitter 等
├── navigation/     # Menu、Tabs、Breadcrumb、Pagination、Steps 等
├── data-entry/     # LineEdit、Select、TreeSelect、DatePicker、ColorPicker、Upload、Form 等
├── data-display/   # List、TreeView、DataGrid、Card、Descriptions 等
├── feedback/       # Dialog、Message、Notification、Tooltip、PopupConfirm、Spin 等
└── window/         # Window、WindowTitleBar、ImagePreviewer 等
```

## 文档边界

- 控件使用文档写在本目录。
- 控件内部架构和包级注册写在 `docs/modules/desktop-controls/`。
- DataGrid 和 ColorPicker 作为独立包，使用文档仍归到本目录，模块架构归到对应 `modules/` 子目录。

## 单控件文档建议结构

```text
# 控件名

## 用途
## 包与命名空间
## 核心属性
## 事件与命令
## 数据流
## 主题与 Token
## 示例
## 常见问题
## 源码索引
```

