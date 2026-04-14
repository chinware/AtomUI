# Transfer 穿梭框

## 概述

穿梭框（Transfer）用于在两栏之间移动元素，完成选择行为。左侧为「源数据」面板，右侧为「目标数据」面板，用户通过勾选和穿梭按钮将数据在两栏间转移。适用于有大量选项需要筛选和整理的场景。

AtomUI 的 Transfer 控件复刻了 [Ant Design 5.0 Transfer](https://ant.design/components/transfer-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的 Transfer 设计哲学

Ant Design 对 Transfer 的定位是：**「双栏穿梭选择框，在两栏之间移动元素，完成选择行为」**。Transfer 的核心价值在于为用户提供直观的双栏对比操作界面——左侧展示可选数据，右侧展示已选数据，通过穿梭按钮在两栏间转移。

**关键设计决策**：

| 特性 | 设计意图 |
|---|---|
| **双栏布局** | 左源右目标的标准布局，清晰展示数据流向 |
| **全选/反选操作** | 支持批量操作，提高大量数据场景下的效率 |
| **搜索过滤** | 内置搜索框，在大量数据中快速定位目标项 |
| **自定义项渲染** | 通过 `ItemTemplate` 自定义列表项展示方式 |
| **单向模式** | `IsOneWay` 模式下仅支持从源到目标的单向转移，目标面板直接删除 |
| **分页支持** | 大量数据时启用分页，提高性能和可操作性 |
| **树形穿梭** | `TreeTransfer` 支持树形结构数据的穿梭操作 |
| **尺寸自适应** | 支持 Large/Middle/Small 三种尺寸 |
| **状态反馈** | 支持 Error/Warning 边框状态，配合表单验证使用 |

### AtomUI 的实现架构

AtomUI Transfer 采用**抽象基类 + 具体实现**的分层架构，支持列表和树两种数据展示模式：

| 组成部分 | 说明 |
|---|---|
| **AbstractTransfer** | 抽象基类，定义穿梭框的通用属性、事件和穿梭逻辑 |
| **ListTransfer** | 列表穿梭框，源和目标面板均为列表视图 |
| **TreeTransfer** | 树形穿梭框，源面板为树视图，目标面板为列表视图 |
| **TransferItemDecorator** | 面板装饰器（内部），负责头部信息栏、搜索框、全选控件、页脚的布局 |
| **TransferListView** | 列表视图实现（内部），实现 `ITransferView` |
| **TransferTreeView** | 树视图实现，实现 `ITransferTreeView` |
| **TransferSelectDropdown** | 选择操作下拉菜单（内部），提供全选/反选/删除等批量操作 |
| **ITransferView** | 视图接口，定义穿梭视图的通用能力 |
| **ITransferTreeView** | 树视图接口，扩展 `ITransferView`，增加遮罩项支持 |

**核心机制**：
- 数据流基于 `ItemsSource`（全量数据）和 `TargetKeys`（目标键集合）驱动，通过键集合的增减完成穿梭操作
- 源面板展示 `ItemsSource - TargetKeys`，目标面板展示 `ItemsSource ∩ TargetKeys`
- 搜索过滤通过 `IValueFilter` 接口实现，支持自定义过滤逻辑
- 树形穿梭框的源面板使用遮罩机制（`SetMaskedItems`）隐藏已转移的节点
- 穿梭按钮的启用/禁用状态由面板选中状态驱动

---

## 功能详解

### 列表穿梭（ListTransfer）

最常用的穿梭模式，源和目标面板均为扁平列表：

- 支持多选后批量穿梭
- 支持全选/反选操作
- 支持搜索过滤
- 支持分页（`PageSize > 0`）
- 支持自定义 `SourceView` 和 `TargetView`

### 树形穿梭（TreeTransfer）

用于树形结构数据的穿梭，源面板展示为树视图，目标面板为扁平列表：

- 源面板展示完整树结构，选中的节点在穿梭后被遮罩（灰显）
- 目标面板展示扁平化的已选项列表
- 支持默认展开所有节点（`IsDefaultExpandAll`）
- 支持自定义源面板树视图

### 单向模式

`IsOneWay=True` 时：
- 隐藏"转移到源"按钮
- 目标面板不再显示选择框（改为删除按钮）
- 用户只能从源穿梭到目标，在目标面板通过删除操作移回

### 搜索过滤

`IsFilterEnabled=True` 时在面板头部下方显示搜索框：
- 内置 `Contains` 过滤器，也可通过 `Filter` 属性自定义
- `FilterValueSelector` 委托指定从数据项中提取过滤值的方式
- `FilterPlaceholderText` 设置搜索框占位文本
- 源/目标面板独立过滤

### 分页

`PageSize > 0` 时启用分页（仅 `ListTransfer` 支持）：
- 面板底部显示分页控件
- 列表宽度自动扩大（使用 `ListWidthLG` Token）
- 选择操作下拉菜单增加"选择当页"/"反选当页"/"删除当页"选项

### 自定义面板视图

通过 `SourceView` / `TargetView` 属性可以替换面板的内部视图实现：

```xml
<atom:TreeTransfer>
    <atom:TreeTransfer.SourceView>
        <atom:TransferTreeView IsDefaultExpandAll="True" />
    </atom:TreeTransfer.SourceView>
</atom:TreeTransfer>
```

### 自定义页脚

通过 `SourceViewFooter` / `TargetViewFooter` 属性可以在面板底部添加自定义内容：

```xml
<atom:ListTransfer>
    <atom:ListTransfer.SourceViewFooter>
        <atom:Button SizeType="Small">Reload</atom:Button>
    </atom:ListTransfer.SourceViewFooter>
</atom:ListTransfer>
```

### 本地化

Transfer 的操作文本通过本地化系统管理：

| 资源键 | 中文 | 英文 |
|---|---|---|
| `Item` | 项 | item |
| `Items` | 项 | items |
| `SelectAll` | 全选所有 | select all data |
| `DeSelectAll` | 取消全选 | deselect all data |
| `RemoveCurrentPage` | 删除当页 | remove current page |
| `RemoveAll` | 删除所有 | remove all data |
| `InvertSelectCurrentPage` | 反选当页 | invert current page |
| `SelectCurrentPage` | 选择当页 | select current page |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本穿梭 | ✅ 双栏穿梭 | ✅ `ListTransfer` | ✅ 完全对齐 |
| 搜索过滤 `showSearch` | ✅ 内置搜索 | ✅ `IsFilterEnabled` | ✅ 完全对齐 |
| 单向模式 `oneWay` | ✅ 单向穿梭 | ✅ `IsOneWay` | ✅ 完全对齐 |
| 分页 `pagination` | ✅ 分页支持 | ✅ `PageSize` | ✅ 完全对齐 |
| 树形穿梭 | ✅ TreeTransfer | ✅ `TreeTransfer` | ✅ 完全对齐 |
| 自定义渲染 `render` | ✅ 自定义项渲染 | ✅ `ItemTemplate` | ✅ 完全对齐 |
| 自定义页脚 `footer` | ✅ 页脚插槽 | ✅ `SourceViewFooter` / `TargetViewFooter` | ✅ 完全对齐 |
| 标题 `titles` | ✅ 面板标题 | ✅ `SourceTitle` / `TargetTitle` | ✅ 完全对齐 |
| 自定义按钮文本 `operations` | ✅ 按钮文字 | ✅ `ToSourceButtonText` / `ToTargetButtonText` | ✅ 完全对齐 |
| 受控 `targetKeys` | ✅ 受控属性 | ✅ `TargetKeys` 绑定 | ✅ 完全对齐 |
| 受控 `selectedKeys` | ✅ 受控属性 | ✅ `SelectedKeys` 绑定 | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ 禁用态 | ✅ `IsEnabled` | ✅ 完全对齐 |
| 状态 `status` | ✅ error/warning | ✅ `Status` 枚举 | ✅ 完全对齐 |
| 表格穿梭 | ✅ Table Transfer | ⚠️ 注释中有 DataGrid 方案 | ⚠️ 开发中 |
| 列表宽高 `listStyle` | ✅ 样式对象 | ✅ `ListWidth` / `ListHeight` | ✅ 完全对齐 |
| `onChange` 回调 | ✅ 回调函数 | ✅ `SelectionChanged` 事件 | ✅ 完全对齐 |
| 尺寸 | ✅ 跟随全局 | ✅ `SizeType` 属性 | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.AbstractTransfer (abstract)
        ├── implements IMotionAwareControl
        ├── implements IInputControlStatusAware
        ├── implements ISizeTypeAware
        ├── AtomUI.Desktop.Controls.ListTransfer
        └── AtomUI.Desktop.Controls.TreeTransfer
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化呈现、主题/样式支持 |
| `AbstractTransfer` | 数据源管理、穿梭逻辑（源↔目标键集合操作）、搜索过滤、全选操作处理、布局配置（拉伸/固定宽度）、事件定义 |
| `ListTransfer` | 列表视图默认创建（`TransferListView`）、分页支持（`PageSize`）、自定义 `SourceView` / `TargetView` |
| `TreeTransfer` | 树视图默认创建（`TransferTreeView`）、树节点遮罩、目标面板扁平化计算 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | 支持 `IsMotionEnabled` 动画开关 |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | 支持 `Status`（Error/Warning）验证状态 |
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small/Middle/Large）尺寸切换 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Transfer/AbstractTransfer.cs` | 穿梭框抽象基类 |
| 控件类 | `src/AtomUI.Desktop.Controls/Transfer/ListTransfer.cs` | 列表穿梭框 |
| 控件类 | `src/AtomUI.Desktop.Controls/Transfer/TreeTransfer.cs` | 树形穿梭框 |
| 内部控件 | `src/AtomUI.Desktop.Controls/Transfer/TransferItemDecorator.cs` | 面板装饰器 |
| 内部控件 | `src/AtomUI.Desktop.Controls/Transfer/TransferListView.cs` | 列表视图 |
| 内部控件 | `src/AtomUI.Desktop.Controls/Transfer/TransferTreeView.cs` | 树视图 |
| 内部控件 | `src/AtomUI.Desktop.Controls/Transfer/TransferSelectDropdown.cs` | 选择操作下拉菜单 |
| 内部控件 | `src/AtomUI.Desktop.Controls/Transfer/TransferRemoveItemButton.cs` | 单向模式删除按钮 |
| 接口 | `src/AtomUI.Desktop.Controls/Transfer/ITransferView.cs` | 视图接口 |
| 接口 | `src/AtomUI.Desktop.Controls/Transfer/ITransferTreeView.cs` | 树视图接口 |
| 枚举 | `src/AtomUI.Desktop.Controls/Transfer/TransferDirection.cs` | 穿梭方向枚举 |
| 枚举 | `src/AtomUI.Desktop.Controls/Transfer/TransferViewType.cs` | 视图类型枚举 |
| 枚举 | `src/AtomUI.Desktop.Controls/Transfer/TransferSelectAction.cs` | 选择操作枚举 |
| 事件参数 | `src/AtomUI.Desktop.Controls/Transfer/TransferSelectionChangedEventArgs.cs` | 选择变更事件参数 |
| 事件参数 | `src/AtomUI.Desktop.Controls/Transfer/TransferItemsRemovedEventArgs.cs` | 项移除事件参数 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Transfer/TransferToken.cs` | 组件级 Design Token |
| 本地化 | `src/AtomUI.Desktop.Controls/Transfer/Localization/zh_CN.cs` | 中文语言资源 |
| 本地化 | `src/AtomUI.Desktop.Controls/Transfer/Localization/en_US.cs` | 英文语言资源 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Transfer/Themes/AbstractTransferTheme.axaml` | 抽象基类 ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Transfer/Themes/ListTransferTheme.axaml` | 列表穿梭框 ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Transfer/Themes/TreeTransferTheme.axaml` | 树穿梭框 ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Transfer/Themes/TransferItemDecoratorTheme.axaml` | 装饰器 ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/Transfer/Themes/TransferThemes.axaml` | 主题资源合并 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TransferShowCase.axaml` | 使用范例 |

---

## 模板结构

### ListTransfer / TreeTransfer 模板

```
Grid#RootLayout (三列布局)
├── TransferItemDecorator#SourceDecoratorView (Column=0)  ← 源面板
├── StackPanel#ActionsLayout (Column=1)                    ← 穿梭操作按钮
│   ├── Button#ToTargetButton (→)                          ← 转移到目标
│   └── Button#ToSourceButton (←)                          ← 转移到源（单向模式隐藏）
└── TransferItemDecorator#TargetDecoratorView (Column=2)  ← 目标面板
```

### TransferItemDecorator 模板（面板装饰器）

```
Border#Frame                                    ← 主框架（边框+圆角）
└── DockPanel#RootLayout                        ← 根布局
    ├── Border#HeaderFrame (Dock=Top)           ← 头部信息栏
    │   └── DockPanel#HeaderLayout
    │       ├── CheckBox#SelectAllCheckBox       ← 全选复选框
    │       ├── TransferSelectDropdown#MenuIndicator ← 操作下拉菜单
    │       ├── ContentPresenter#SelectedInfo    ← 选中信息（"2/10 items"）
    │       └── ContentPresenter#TitleContentPresenter ← 面板标题
    ├── LineEdit#FilterInput (Dock=Top)         ← 搜索框（IsFilterEnabled 控制）
    ├── Border#FooterFrame (Dock=Bottom)        ← 页脚区域
    │   └── ContentPresenter#FooterPresenter
    └── ContentPresenter#ContentPresenter       ← 视图内容（ITransferView）
```
