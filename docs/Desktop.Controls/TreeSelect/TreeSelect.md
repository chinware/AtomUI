# TreeSelect 树选择

## 概述

树选择（TreeSelect）是一种将树形数据与下拉选择结合的复合控件，用于从层级结构的数据中选择一个或多个值。它本质上是将 `TreeView` 嵌入到下拉弹窗中，让用户在不占用大量页面空间的前提下，浏览和选择树形数据。

TreeSelect 常见于：组织架构选择、分类目录选择、地区层级选择、权限树选择等场景——凡是数据具有父子嵌套关系、且需要通过下拉方式呈现的，都适合使用 TreeSelect。

AtomUI 的 `TreeSelect` 控件对齐了 [Ant Design 5.0 TreeSelect](https://ant.design/components/tree-select-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的 TreeSelect 设计哲学

Ant Design 对 TreeSelect 的定位是：**「类似 Select 的选择控件，可选择的数据结构是一个树形结构时，可以使用 TreeSelect」**。与普通的 `Select` 相比，TreeSelect 的核心差异在于：

- **数据源是树形的**：每个选项可以有子选项，形成多层嵌套。
- **支持展开/折叠**：用户可以通过展开/折叠来逐层浏览。
- **支持勾选模式**：除了单选/多选之外，还支持 Checkbox 勾选方式，并提供 `ShowCheckedStrategy` 控制显示策略。

**三种选择模式**：

| 模式 | 设计意图 | 典型用途 |
|---|---|---|
| 🔘 **单选** | 默认模式，点击节点即选中，弹窗自动关闭 | 选择一个分类、一个部门 |
| ✅ **多选（Multiple）** | 允许选择多个节点，选中项以 Tag 形式展示 | 选择多个标签、多个地区 |
| ☑️ **勾选（Checkable）** | 使用 Checkbox 进行勾选，支持父子联动 | 权限树、批量分类选择 |

**勾选策略（ShowCheckedStrategy）**：

| 策略 | 行为 |
|---|---|
| `All` | 展示所有勾选的节点（默认） |
| `ShowParent` | 当所有子节点都勾选时，仅展示父节点 |
| `ShowChild` | 仅展示叶子节点 |

### AbstractSelect 基类能力

AtomUI 的 `TreeSelect` 继承自 `AbstractSelect`，这是一个桌面端选择类控件的公共基类（定义在 `AtomUI.Desktop.Controls` 中），它提供了所有选择类控件共享的基础设施。

**AbstractSelect 的核心职责：**

`AbstractSelect` 继承自 Avalonia 的 `TemplatedControl`，并实现了 7 个共享接口。其核心行为是：**管理下拉弹窗的打开/关闭、提供统一的输入框装饰（AddOnDecoratedBox）、处理筛选/清除/加载/状态反馈等通用逻辑**。继承链为：

```
Control → TemplatedControl → AbstractSelect
```

**AbstractSelect 提供的基础能力：**

| 能力 | 说明 |
|---|---|
| 下拉弹窗管理 | `IsDropDownOpen` 控制弹窗开关，自动处理 `Opening` / `Opened` / `Closing` / `Closed` 生命周期 |
| 占位文本 | `PlaceholderText` + `PlaceholderForeground` 提供空状态提示 |
| 筛选支持 | `IsFilterEnabled` + `FilterValue` 提供搜索筛选基础 |
| 清除按钮 | `IsAllowClear` 支持一键清除选中项 |
| AddOn 装饰 | `LeftAddOn` / `RightAddOn` / `ContentLeftAddOn` / `ContentRightAddOn` 提供前后缀附加内容 |
| 弹窗位置 | `Placement` 控制弹窗相对于控件的弹出方向 |
| 样式变体 | `StyleVariant`（Outline / Filled / Borderless / Underlined） |
| 状态反馈 | `Status`（Default / Error / Warning）配合表单验证 |
| 尺寸系统 | `SizeType`（Small / Middle / Large）统一尺寸 |
| 多选限制 | `MaxCount` 限制最大可选数量，`MaxTagCount` 限制 Tag 最大显示数量 |
| 空状态指示 | `EmptyIndicator` 在无数据时展示空提示 |
| 加载状态 | `IsLoading` 显示旋转加载图标 |
| 动画控制 | `IsMotionEnabled` 控制弹窗展开/收起动画 |
| 紧凑空间 | `ICompactSpaceAware` 支持在 `Space.Compact` 中使用 |
| 表单集成 | `IFormItemAware` 参与 Form 验证流程 |

### AtomUI TreeSelect 的扩展设计

TreeSelect 在 AbstractSelect 的基础上，增加了树形数据特有的能力：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **树形数据源** | `ItemsSource`（`IEnumerable<ITreeItemNode>`）+ `Items` 集合 | 支持树形数据绑定和直接操作 |
| **自定义模板** | `ItemTemplate`（`IDataTemplate`） | 自定义节点渲染方式 |
| **单选/多选/勾选** | `IsMultiple` + `IsTreeCheckable` 属性 | 三种选择模式灵活切换 |
| **勾选策略** | `ShowCheckedStrategy` 枚举 | 控制勾选模式下 Tag 的显示策略 |
| **搜索筛选** | `Filter`（`ITreeItemFilter`）+ `FilterHighlightStrategy` | 支持关键字搜索并高亮匹配项 |
| **异步加载** | `DataLoader`（`ITreeItemNodeLoader`） | 支持树节点按需异步加载 |
| **树线/图标** | `IsShowTreeLine` / `IsShowIcon` / `IsShowLeafIcon` / `IsShowLine` | 控制树形结构的视觉展示 |
| **展开控制** | `IsDefaultExpandAll` / `TreeDefaultExpandedPaths` | 控制初始展开状态 |
| **严格勾选** | `IsTreeCheckStrictly` | 父子节点勾选互不关联 |
| **选中项** | `SelectedItem`（单选）/ `SelectedItems`（多选）| 双向绑定选中值 |
| **弹窗匹配宽度** | `IsPopupMatchSelectWidth`（继承自 AbstractSelect） | 控制弹窗是否匹配控件宽度 |
| **最大选择数量** | `MaxCount`（继承自 AbstractSelect） | 达到上限后禁止继续选择 |

---

## 功能详解

### 选择模式

TreeSelect 根据 `IsMultiple` 和 `IsTreeCheckable` 的组合，提供三种选择模式：

| `IsMultiple` | `IsTreeCheckable` | 效果 |
|---|---|---|
| `false` | `false` | **单选模式**：点击节点即选中，下拉自动关闭 |
| `true` | `false` | **多选模式**：可选择多个节点，选中项以 Tag 展示 |
| `true`* | `true` | **勾选模式**：显示 Checkbox，支持父子联动勾选 |

> *注：设置 `IsTreeCheckable = true` 会自动将 `IsMultiple` 设为 `true`。

### 勾选策略（ShowCheckedStrategy）

在勾选模式下，`ShowCheckedStrategy` 控制选中项在选择框中的展示方式：

| 策略值 | 行为 | 典型效果 |
|---|---|---|
| `All` | 展示所有被勾选的节点 | 勾选了父节点和所有子节点，则全部显示为 Tag |
| `ShowParent` | 当一个父节点的所有子节点都被勾选时，仅显示父节点 | 简化展示，减少 Tag 数量 |
| `ShowChild` | 仅展示被勾选的叶子节点 | 只关注最终数据，忽略中间层级 |

### 搜索筛选

当 `IsFilterEnabled = true` 时，TreeSelect 在输入框中提供搜索功能：

- 单选模式：输入框变为搜索框，打开弹窗后可直接输入关键字。
- 多选模式：在 Tag 区域末尾提供内嵌搜索输入框。
- 筛选逻辑由 `Filter`（`ITreeItemFilter` 接口）控制，默认使用 `DefaultTreeItemFilter`。
- `FilterHighlightStrategy` 控制匹配高亮方式（高亮整行、加粗匹配文字、展开匹配路径、隐藏不匹配项等）。
- `FilterHighlightForeground` 控制高亮颜色，默认使用 `ColorPrimary`。

### 异步加载

通过 `DataLoader`（`ITreeItemNodeLoader`）属性，TreeSelect 支持树节点按需异步加载：

- 展开一个非叶子节点时，自动调用 `DataLoader` 加载其子节点。
- 加载过程中节点显示旋转加载图标。
- 适用于数据量大、不适合一次性加载全部树形数据的场景。

### 最大选择数量（MaxCount）

在多选/勾选模式下，可通过 `MaxCount` 限制最大可选数量：

- 当选择数量达到 `MaxCount` 时，未选中的节点变为禁用状态。
- 配合 `IsShowMaxCountIndicator` 可在右侧显示已选/最大数量指示器。

### 树线与图标

| 属性 | 效果 |
|---|---|
| `IsShowTreeLine` | 在节点之间显示连接线 |
| `IsShowLine` | 控制 TreeView 中的连接线显示 |
| `IsShowIcon` | 显示节点图标 |
| `IsShowLeafIcon` | 显示叶子节点图标 |
| `IsSwitcherRotation` | 展开/折叠按钮使用旋转动画（而非切换图标） |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 树形数据源 `treeData` | ✅ 数组对象 | ✅ `ItemsSource`（`IEnumerable<ITreeItemNode>`） | ✅ 完全对齐 |
| 单选 | ✅ 默认模式 | ✅ 默认模式 | ✅ 完全对齐 |
| 多选 `multiple` | ✅ 布尔属性 | ✅ `IsMultiple` 属性 | ✅ 完全对齐 |
| 勾选 `treeCheckable` | ✅ 布尔属性 | ✅ `IsTreeCheckable` 属性 | ✅ 完全对齐 |
| 勾选策略 `showCheckedStrategy` | ✅ `SHOW_ALL / SHOW_PARENT / SHOW_CHILD` | ✅ `TreeSelectCheckedStrategy` 枚举 | ✅ 完全对齐 |
| 搜索筛选 `showSearch` / `filterTreeNode` | ✅ 属性 + 回调 | ✅ `IsFilterEnabled` + `Filter` 接口 | ✅ 完全对齐 |
| 异步加载 `loadData` | ✅ 回调函数 | ✅ `DataLoader`（`ITreeItemNodeLoader`） | ✅ 完全对齐 |
| 默认展开所有 `treeDefaultExpandAll` | ✅ 布尔属性 | ✅ `IsDefaultExpandAll` | ✅ 完全对齐 |
| 默认展开指定节点 `treeDefaultExpandedKeys` | ✅ 数组 | ✅ `TreeDefaultExpandedPaths` | ✅ 完全对齐 |
| 显示树线 `treeLine` | ✅ 布尔/对象 | ✅ `IsShowTreeLine` / `IsShowLine` | ✅ 完全对齐 |
| 显示图标 `treeIcon` | ✅ 布尔属性 | ✅ `IsShowIcon` | ✅ 完全对齐 |
| 严格勾选 `treeCheckStrictly` | ✅ 布尔属性 | ✅ `IsTreeCheckStrictly` | ✅ 完全对齐 |
| 清除按钮 `allowClear` | ✅ 布尔属性 | ✅ `IsAllowClear` | ✅ 完全对齐 |
| 占位文本 `placeholder` | ✅ 字符串 | ✅ `PlaceholderText` | ✅ 完全对齐 |
| 弹窗匹配宽度 `popupMatchSelectWidth` | ✅ 布尔/数值 | ✅ `IsPopupMatchSelectWidth` | ✅ 完全对齐 |
| 弹窗位置 `placement` | ✅ 字符串 | ✅ `Placement` 枚举 | ✅ 完全对齐 |
| 最大选择数 `maxCount` | ✅ 数值 | ✅ `MaxCount` | ✅ 完全对齐 |
| 样式变体 `variant` | ✅ `outlined / filled / borderless` | ✅ `StyleVariant`（Outline / Filled / Borderless / Underlined） | ✅ 超越对齐（额外支持 Underlined） |
| 状态 `status` | ✅ `error / warning` | ✅ `Status` 枚举 | ✅ 完全对齐 |
| 尺寸 `size` | ✅ `large / middle / small` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 前后缀 | ✅ `suffixIcon` | ✅ `LeftAddOn` / `RightAddOn` / `ContentLeftAddOn` / `SuffixIcon` | ✅ 超越对齐 |
| 虚拟滚动 `virtual` | ✅ 支持 | ⚠️ 暂未支持 | ⚠️ 待支持 |
| 下拉菜单自定义渲染 `dropdownRender` | ✅ 回调函数 | ❌ 暂不支持 | ⚠️ 待支持 |
| `treeTitleRender` | ✅ 回调函数 | ✅ `ItemTemplate` | ✅ 完全对齐（方式不同） |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.AbstractSelect
        ├── implements IMotionAwareControl
        ├── implements ISizeTypeAware
        ├── implements ICompactSpaceAware
        ├── implements IInputControlStatusAware
        ├── implements IInputControlStyleVariantAware
        ├── implements IFormItemAware
        └── implements IFormItemFeedbackAware
              └── AtomUI.Desktop.Controls.TreeSelect
```

> **注意**：`AbstractSelect` 和 `TreeSelect` 都位于 `AtomUI.Desktop.Controls`。当前 TreeSelect 没有跨平台的抽象基类（即没有 `AtomUI.Controls.AbstractTreeSelect`），这意味着它目前是纯桌面端控件。未来如需支持移动端，可能需要提取共享基类。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | Avalonia 模板化控件基础设施 |
| `AbstractSelect` | 下拉弹窗管理、AddOn 装饰框、筛选/清除/加载、样式变体、状态反馈、尺寸系统、紧凑空间适配、表单集成 |
| `TreeSelect` | 树形数据源、单选/多选/勾选模式、勾选策略、树线/图标控制、异步加载、搜索筛选（树专用）、选中项管理 |

**实现的共享接口（通过 AbstractSelect 继承）：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持动画开关控制 |
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸 |
| `ICompactSpaceAware` | `AtomUI.Controls.Shared` | 在 `Space.Compact` 中使用时自动调整圆角和边框 |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | 支持 `Status`（Default / Error / Warning）状态切换 |
| `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` | 支持 `StyleVariant`（Outline / Filled / Borderless / Underlined） |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |
| `IFormItemFeedbackAware` | `AtomUI.Controls.Shared` | 支持表单验证反馈控件展示 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Desktop.Controls/Select/AbstractSelect.cs` | 选择类控件公共基类 |
| 控件类 | `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelect.cs` | TreeSelect 主控件 |
| 辅助控件 | `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelectAddOnDecoratedBox.cs` | 内部装饰框（处理多选内边距） |
| 辅助控件 | `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelectTreeView.cs` | 内部 TreeView（处理 CheckBox 切换和最大选择数） |
| 辅助控件 | `src/AtomUI.Desktop.Controls/TreeSelect/TreeViewSelectTreeViewItem.cs` | 内部 TreeViewItem（传递最大选择数状态） |
| 枚举类型 | `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelectCheckedStrategy.cs` | 勾选策略枚举 |
| Token 定义 | `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelectToken.cs` | 组件级 Design Token |
| 转换器 | `src/AtomUI.Desktop.Controls/TreeSelect/Converters/TreeItemNodeConverter.cs` | ITreeItemNode → Header 转换器 |
| 伪类常量 | `src/AtomUI.Desktop.Controls/Select/SelectPseudoClass.cs` | 共享伪类定义 |
| 主题模板 | `src/AtomUI.Desktop.Controls/TreeSelect/Themes/TreeSelectTheme.axaml` | ControlTheme AXAML |
| 主题模板 | `src/AtomUI.Desktop.Controls/TreeSelect/Themes/TreeSelectAddOnDecoratedBoxTheme.axaml` | 装饰框 ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/TreeSelect/Themes/TreeSelectTreeViewItemTheme.axaml` | TreeViewItem ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/TreeSelect/Themes/TreeSelectThemes.axaml` | ResourceDictionary 聚合 |
| 模板常量 | `src/AtomUI.Desktop.Controls/TreeSelect/Themes/TreeSelectThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml` | 使用范例 |
| Gallery 代码 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml.cs` | 示例 code-behind |

---

## 模板结构

TreeSelect 的 ControlTemplate 由两个核心部分组成：装饰框（选择框外观）和弹窗（下拉树）。

```
Panel
├── TreeSelectAddOnDecoratedBox (PART_AddOnDecoratedBox)  ← 选择框外观层
│   ├── ContentRightAddOn                                  ← 右侧附加内容
│   │   ├── SelectMaxCountIndicator                        ← 最大数量指示器（可选）
│   │   ├── ContentPresenter (ContentRightAddOn)           ← 自定义右侧内容（可选）
│   │   └── SelectHandle                                   ← 下拉箭头 / 清除按钮 / 加载图标
│   └── Panel                                              ← 内容区域
│       ├── TextBlock#PlaceholderText                      ← 占位文本
│       ├── ContentPresenter#SingleSelectResultPresenter   ← 单选模式：显示选中项文本
│       ├── SelectFilterTextBox (PART_SingleFilterInput)   ← 单选模式：搜索输入框
│       └── SelectTagAwareTextBox#SelectedItemsBox         ← 多选模式：Tag 展示 + 搜索输入
└── Popup (PART_Popup)                                     ← 下拉弹窗
    └── Border#PopupFrame                                  ← 弹窗容器（背景、圆角、阴影）
        └── TreeSelectTreeView (PART_TreeView)             ← 内嵌 TreeView
```

**模板设计要点：**
- **单选 vs 多选**：通过 `IsMultiple` 属性切换 `SingleSelectResultPresenter`/`SelectFilterTextBox`（单选）和 `SelectTagAwareTextBox`（多选）的可见性。
- **搜索输入**：单选搜索时，`SelectFilterTextBox` 覆盖在选中文本之上，选中文本降为 Placeholder 色。
- **弹窗 TreeView**：使用自定义的 `TreeSelectTreeView`，继承自标准 `TreeView`，增加了 `IsMaxSelectReached` 状态传递能力。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `TreeSelectThemeConstants.PopupPart` | `"PART_Popup"` | 下拉弹窗 |
| `TreeSelectThemeConstants.SingleFilterInputPart` | `"PART_SingleFilterInput"` | 单选搜索输入框 |
| `TreeSelectThemeConstants.TreeViewPart` | `"PART_TreeView"` | 内嵌 TreeView |
