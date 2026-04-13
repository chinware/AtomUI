# Pagination 分页

## 概述

分页组件（Pagination）用于在大量数据中切换页面，是列表、表格等数据展示场景中的核心导航控件。AtomUI 提供两种分页模式：**标准分页**（`Pagination`）和**简洁分页**（`SimplePagination`），分别对应 Ant Design 的 Pagination 默认模式和 `simple` 模式。

标准分页支持页码切换、页面大小选择（SizeChanger）、快速跳转（QuickJumper）、总数显示等完整功能。简洁分页仅展示当前页/总页数和前进/后退按钮，适合空间受限的场景。

AtomUI 的 `Pagination` 控件对应 [Ant Design 5.0 Pagination](https://ant.design/components/pagination-cn) 组件，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的分页设计哲学

Ant Design 对分页的定位是：**「采用分页的形式分隔长列表，每次只加载一个页面」**。其核心设计原则：

- **可预测的导航**：用户可以看到总页数，了解自己在整个数据集中的位置
- **省略折叠**：当页数超过阈值时，使用省略号（`...`）折叠中间页码，保持界面简洁
- **辅助功能**：支持页面大小选择器和快速跳转输入框，提升大数据量下的操作效率

**两种模式**：

| 模式 | 设计意图 | 视觉表现 |
|---|---|---|
| **标准模式** | 完整功能，适合数据量大的场景 | 页码按钮组 + 可选 SizeChanger/QuickJumper |
| **简洁模式** | 紧凑展示，适合空间受限的场景 | 仅前进/后退按钮 + 当前页/总页数文本 |

**三种对齐方式**：

| 对齐 | 说明 |
|---|---|
| `Start` | 左对齐（默认） |
| `Center` | 居中对齐 |
| `End` | 右对齐 |

**两种尺寸**：

| 尺寸 | 说明 |
|---|---|
| `Middle` | 标准尺寸（默认） |
| `Small` | 迷你尺寸，紧凑布局 |

### Avalonia 基础能力

AtomUI 的 Pagination **不继承自** Avalonia 内置控件，而是**完全自研**实现：

- `AbstractPagination` 继承自 `TemplatedControl`，作为分页控件的抽象基类
- `Pagination` 和 `SimplePagination` 均继承自 `AbstractPagination`
- 内部使用 `PaginationNav`（继承自 `SelectingItemsControl`）管理页码按钮
- 页码按钮由 `PaginationNavItem`（继承自 `ContentControl`）实现

### AtomUI 的实现设计

| 设计能力 | 实现方式 | 设计动机 |
|---|---|---|
| **标准分页** | `Pagination` 类 | 完整的页码导航 + 省略号折叠 |
| **简洁分页** | `SimplePagination` 类 | 紧凑的前进/后退 + 页码文本 |
| **两种尺寸** | `ISizeTypeAware` + `SizeType` | 标准/迷你两种尺寸 |
| **三种对齐** | `PaginationAlign` 枚举 | 左/中/右对齐 |
| **省略号折叠** | `PaginationItemType.Ellipses` | 自动计算可见页码范围，中间用省略号表示 |
| **页面大小选择** | `IsShowSizeChanger` + 内部 `ComboBox` | 下拉切换每页条数（10/20/50/100） |
| **快速跳转** | `IsShowQuickJumper` + 内部 `QuickJumperBar` | 输入页码按回车跳转 |
| **总数显示** | `IsShowTotalInfo` + `TotalInfoTemplate` | 显示总数据条数和当前范围 |
| **单页隐藏** | `IsHideOnSinglePage` | 仅一页时自动隐藏分页组件 |
| **本地化** | `LanguageProvider` | "Go to"、"Page"、"Total" 等文本支持中英文 |
| **Design Token** | `PaginationToken` | 所有视觉值从 Token 派生 |

---

## 功能详解

### 页码导航与省略号折叠

标准分页最多展示 11 个导航项（`MaxNavItemCount = 11`），包含：
- 1 个**上一页**按钮（`←`）
- 最多 9 个**页码**按钮（含省略号）
- 1 个**下一页**按钮（`→`）

当总页数超过 7 时，自动启用省略号折叠算法：
- 当前页靠近首部时：显示前 N 页 + `...` + 末页
- 当前页靠近尾部时：首页 + `...` + 后 N 页
- 当前页在中间时：首页 + `...` + 当前页前后各 2 页 + `...` + 末页

### 页面大小选择器（SizeChanger）

当 `IsShowSizeChanger = true` 时，页码导航右侧显示一个 `ComboBox`，提供四种页面大小选项：
- 10 / Page
- 20 / Page
- 50 / Page
- 100 / Page

切换页面大小会自动重新计算总页数和当前页。

### 快速跳转（QuickJumper）

当 `IsShowQuickJumper = true` 时，显示 "Go to [   ] Page" 输入框。用户输入页码后按 Enter 跳转。

### 总数信息（TotalInfo）

当 `IsShowTotalInfo = true` 时，显示总数据条数。支持自定义模板字符串：
- 默认格式：`"Total ${Total} items"`
- 支持变量：`${Total}`（总数）、`${RangeStart}`（当前页起始条数）、`${RangeEnd}`（当前页结束条数）

### 简洁模式（SimplePagination）

SimplePagination 提供两种子模式：
- **只读模式**（`IsReadOnly = true`，默认）：显示 "当前页 / 总页数" 文本
- **可编辑模式**（`IsReadOnly = false`）：当前页替换为可编辑输入框（`QuickJumpEdit`），用户可直接输入页码

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 标准分页 | ✅ | ✅ `Pagination` | ✅ 完全对齐 |
| 简洁模式 `simple` | ✅ | ✅ `SimplePagination` | ✅ 完全对齐 |
| 页码切换 | ✅ `current` / `onChange` | ✅ `CurrentPage` / `CurrentPageChanged` | ✅ 完全对齐 |
| 总数据量 `total` | ✅ | ✅ `Total` | ✅ 完全对齐 |
| 每页条数 `pageSize` | ✅ | ✅ `PageSize`（10/20/50/100） | ✅ 完全对齐 |
| 页面大小选择 `showSizeChanger` | ✅ | ✅ `IsShowSizeChanger` | ✅ 完全对齐 |
| 快速跳转 `showQuickJumper` | ✅ | ✅ `IsShowQuickJumper` | ✅ 完全对齐 |
| 总数显示 `showTotal` | ✅ 函数 | ✅ `IsShowTotalInfo` + `TotalInfoTemplate` | ⚠️ 字符串模板替代函数 |
| 单页隐藏 `hideOnSinglePage` | ✅ | ✅ `IsHideOnSinglePage` | ✅ 完全对齐 |
| 三种对齐 `align` | ✅ `start/center/end` | ✅ `PaginationAlign` | ✅ 完全对齐 |
| 迷你尺寸 `size="small"` | ✅ | ✅ `SizeType="Small"` | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ | ✅ `IsEnabled` | ✅ 完全对齐 |
| 省略号折叠 | ✅ | ✅ 自动折叠 | ✅ 完全对齐 |
| 自定义上下页图标 `itemRender` | ✅ | ❌ 暂未支持 | ⚠️ 待支持 |
| 响应式 `responsive` | ✅ | ❌ 暂未支持 | ⚠️ 待支持 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.AbstractPagination
        ├── implements ISizeTypeAware
        ├── implements IMotionAwareControl
        ├── AtomUI.Desktop.Controls.Pagination        ← 标准分页
        └── AtomUI.Desktop.Controls.SimplePagination   ← 简洁分页
```

> **注意**：Pagination 控件完全在 `AtomUI.Desktop.Controls` 中实现，**没有**在 `AtomUI.Controls` 中定义 Abstract 基类。这是因为分页组件的视觉和交互在不同平台可能差异较大，暂未抽取设备无关层。

**各层级职责划分：**

| 类 | 职责 |
|---|---|
| `AbstractPagination` | 公共分页逻辑：`CurrentPage` / `PageSize` / `Total` / `PageCount` 计算、`CurrentPageChanged` 事件、`IsHideOnSinglePage` 逻辑、`Align` 对齐 |
| `Pagination` | 标准分页：省略号折叠算法、`PaginationNav` 管理、SizeChanger 下拉框、QuickJumper 跳转栏、TotalInfo 信息 |
| `SimplePagination` | 简洁分页：前后页按钮、当前页/总页数文本（或可编辑输入框） |
| `PaginationNav` | 内部控件：管理页码按钮容器（`SelectingItemsControl`） |
| `PaginationNavItem` | 内部控件：单个页码按钮（支持 Previous/Next/PageIndicator/Ellipses 四种类型） |
| `QuickJumperBar` | 内部控件：快速跳转栏（"Go to [输入框] Page"） |
| `QuickJumpEdit` | 内部控件：纯数字输入框（`LineEdit` 子类，限制最小/最大值） |
| `PageSizeComboBoxItem` | 内部控件：页面大小下拉选项 |

---

## 源码位置

| 文件路径 | 说明 |
|---|---|
| `src/AtomUI.Desktop.Controls/Pagination/AbstractPagination.cs` | 分页抽象基类 |
| `src/AtomUI.Desktop.Controls/Pagination/Pagination.cs` | 标准分页 |
| `src/AtomUI.Desktop.Controls/Pagination/SimplePagination.cs` | 简洁分页 |
| `src/AtomUI.Desktop.Controls/Pagination/PaginationNav.cs` | 页码导航容器（internal） |
| `src/AtomUI.Desktop.Controls/Pagination/PaginationNavItem.cs` | 页码按钮项（internal） |
| `src/AtomUI.Desktop.Controls/Pagination/QuickJumperBar.cs` | 快速跳转栏（internal） |
| `src/AtomUI.Desktop.Controls/Pagination/QuickJumpEdit.cs` | 纯数字输入框（internal） |
| `src/AtomUI.Desktop.Controls/Pagination/PageSizeComboBoxItem.cs` | 页面大小下拉项（internal） |
| `src/AtomUI.Desktop.Controls/Pagination/PageNavRequestArgs.cs` | 页码导航请求事件参数（internal） |
| `src/AtomUI.Desktop.Controls/Pagination/PaginationToken.cs` | 组件级 Design Token |
| `src/AtomUI.Desktop.Controls/Pagination/Localization/en_US.cs` | 英文本地化 |
| `src/AtomUI.Desktop.Controls/Pagination/Localization/zh_CN.cs` | 中文本地化 |
| `src/AtomUI.Desktop.Controls/Pagination/Themes/PaginationTheme.axaml` | Pagination 主题 |
| `src/AtomUI.Desktop.Controls/Pagination/Themes/SimplePaginationTheme.axaml` | SimplePagination 主题 |
| `src/AtomUI.Desktop.Controls/Pagination/Themes/PaginationNavTheme.axaml` | PaginationNav 主题 |
| `src/AtomUI.Desktop.Controls/Pagination/Themes/PaginationNavItemTheme.axaml` | PaginationNavItem 主题 |
| `src/AtomUI.Desktop.Controls/Pagination/Themes/QuickJumperBarTheme.axaml` | QuickJumperBar 主题 |
| `src/AtomUI.Desktop.Controls/Pagination/Themes/PaginationThemeConstants.cs` | 模板部件常量 |
| `src/AtomUI.Controls.Shared/EventArgs/PageChangedArgs.cs` | 页变更事件参数（共享层） |
| `controlgallery/AtomUIGallery/ShowCases/Views/Navigation/PaginationShowCase.axaml` | Gallery 示例 |

---

## 模板结构

### Pagination（标准分页）模板

```
StackPanel#PART_RootLayout (Horizontal)               ← 根布局容器
  ├── ContentPresenter#PART_TotalInfoPresenter         ← 总数信息（可选）
  ├── PaginationNav#PART_Nav                           ← 页码导航区
  │     └── StackPanel#PART_Frame (Horizontal)
  │           ├── PaginationNavItem (Previous ←)       ← 上一页
  │           ├── PaginationNavItem (Page 1)           ← 页码
  │           ├── PaginationNavItem (...)              ← 省略号
  │           ├── PaginationNavItem (Page N)           ← 页码
  │           └── PaginationNavItem (Next →)           ← 下一页
  ├── ContentPresenter#PART_SizeChangerPresenter       ← 页面大小选择器（可选）
  │     └── ComboBox                                   ← 10/20/50/100 per page
  └── ContentPresenter#PART_QuickJumperBarPresenter    ← 快速跳转栏（可选）
        └── QuickJumperBar
              ├── TextBlock ("Go to")
              ├── LineEdit#PART_PageLineEdit            ← 页码输入框
              └── TextBlock ("Page")
```

### SimplePagination（简洁分页）模板

```
StackPanel#PART_RootLayoutPart (Horizontal)            ← 根布局
  ├── PaginationNavItem#PART_PreviousNavItem (←)       ← 上一页
  ├── TextBlock#PART_InfoIndicator                     ← "1 / 5" 页码文本
  │   或
  ├── QuickJumpEdit#PART_QuickJumper                   ← 可编辑页码输入
  ├── TextBlock#PART_InfoIndicator                     ← " / 5"
  └── PaginationNavItem#PART_NextNavItem (→)           ← 下一页
```

### PaginationNavItem 模板

```
Border#PART_MainFrame                                  ← 主框架（背景、圆角、边框）
  └── Panel
        ├── ContentPresenter#PART_Content              ← 页码数字
        └── IconPresenter                              ← 图标（←/→/...）
```
