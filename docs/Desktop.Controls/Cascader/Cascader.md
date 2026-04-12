# Cascader 级联选择

## 概述

级联选择器（Cascader）用于从一组相关联的层级数据集合中进行选择，例如省/市/区、公司/部门/组、商品分类等多级分类数据。用户逐级展开并选择目标选项，最终获得完整的层级路径。

AtomUI 的 `Cascader` 控件完整复刻了 [Ant Design 5.0 Cascader](https://ant.design/components/cascader-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的级联选择设计哲学

Ant Design 的级联选择器定位是：**「需要从一组相关联的数据集合进行选择，例如省市区、公司层级、事物分类等」**。当一个数据集合有清晰的层级结构时，可通过级联选择器逐级查看并选择。

**核心交互模式：**

| 模式 | 设计意图 | 典型用途 |
|---|---|---|
| 🔽 **单选级联** | 逐级展开，点击叶子节点完成选择，选择框显示完整路径 | 省市区选择、组织架构选择 |
| ☑️ **多选级联** | 通过复选框选择多个选项，支持父子联动勾选 | 权限分配、标签筛选 |
| 🔍 **可搜索级联** | 在输入框中输入关键词过滤选项，直接选择匹配结果 | 快速定位特定选项 |
| 📥 **异步加载** | 展开节点时动态加载子级数据，适合大量数据场景 | 区域数据动态加载、远程分类检索 |

**选中策略（多选模式）：**

| 策略 | 说明 |
|---|---|
| `All` | 显示所有选中的节点（包含父节点和子节点） |
| `ShowParent` | 当所有子节点都被选中时，只显示父节点 |
| `ShowChild` | 只显示叶子节点 |

### Avalonia 基础设施

AtomUI 的 `Cascader` 继承自内部的 `AbstractSelect`，后者是一个基于 `TemplatedControl` 的复合选择控件基类。`AbstractSelect` 提供了以下基础设施：

**AbstractSelect 提供的核心能力：**

| 能力 | 说明 |
|---|---|
| 弹出层管理 | `IsDropDownOpen` + `Popup` 控件，管理下拉面板的展开与收起 |
| 尺寸适配 | `ISizeTypeAware` 接口，支持 Large / Middle / Small 三种尺寸 |
| 输入框装饰 | `AddOnDecoratedBox` 提供前后附加内容、样式变体（Outlined / Filled / Borderless / Underlined） |
| 占位文本 | `PlaceholderText` + `PlaceholderForeground` |
| 清除功能 | `IsAllowClear` 一键清除选中值 |
| 搜索过滤 | `IsFilterEnabled` + `FilterValue` 支持在输入框中搜索 |
| 多选标签 | `MaxTagCount` / `IsResponsiveTagMode` / `MaxCount` 控制多选标签展示 |
| 表单集成 | `IFormItemAware` / `IInputControlStatusAware` 接口 |
| 紧凑空间 | `ICompactSpaceAware` 接口 |
| 样式变体 | `IInputControlStyleVariantAware` 支持 Outlined / Filled / Borderless / Underlined |
| 状态反馈 | `Status` 属性支持 Error / Warning 状态 |

### AtomUI 的扩展设计

AtomUI `Cascader` 在 `AbstractSelect` 基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **层级选项数据模型** | `ICascaderOption` 接口 + `CascaderOption` 记录类 | 提供树形层级数据模型，支持 Header / Value / Children / IsLeaf 等属性 |
| **单选模式** | `SelectedOption` + `SelectedOptionPath` | 选中叶子节点后显示完整路径（如 "浙江/杭州/西湖"） |
| **多选模式** | `IsMultiple` + `SelectedOptions` + 复选框联动 | 父子节点关联勾选，支持三态复选框 |
| **选中策略** | `ShowCheckedStrategy` 枚举 | 控制多选模式下标签显示策略（All / ShowParent / ShowChild） |
| **展开触发方式** | `ExpandTrigger` 枚举（Click / Hover） | 支持点击展开或鼠标悬浮展开子菜单 |
| **异步数据加载** | `ICascaderItemDataLoader` 接口 + `DataLoader` 属性 | 展开节点时动态加载子级，显示加载动画 |
| **搜索过滤** | `ICascaderItemFilter` + `Filter` 属性 | 过滤匹配项并高亮显示关键词 |
| **过滤高亮** | `FilterHighlightStrategy` + `FilterHighlightForeground` | 控制搜索关键词的高亮策略和颜色 |
| **自定义图标** | `ExpandIcon` / `LoadingIcon` / `SuffixIcon` | 自定义展开图标、加载图标和后缀图标 |
| **前后附加内容** | `LeftAddOn` / `RightAddOn` / `ContentLeftAddOn` | 支持在输入框前后添加自定义内容 |
| **默认选中路径** | `DefaultSelectOptionPath` | 通过路径字符串设置初始选中值 |
| **允许选择父节点** | `IsAllowSelectParent` | 默认仅叶子节点可选，启用后父节点也可选择 |
| **弹出位置** | `PopupPlacement`（继承自 AbstractSelect） | 支持 TopLeft / TopRight / BottomLeft / BottomRight |
| **Design Token** | `CascaderToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |

---

## 功能详解

### 单选模式

默认模式下，Cascader 以单选方式工作：用户逐级展开选项菜单，点击叶子节点完成选择。选中后，输入框中显示完整的路径文本（如 "Zhejiang/Hangzhou/West Lake"）。

- 点击选项展开其子级（或通过 `ExpandTrigger="Hover"` 设置为鼠标悬浮展开）
- 点击叶子节点时自动关闭下拉面板并记录选中值
- `SelectedOption` 属性保存当前选中的 `ICascaderOption` 对象
- `SelectedOptionPath` 属性保存选中项的完整路径字符串

### 多选模式（IsMultiple）

设置 `IsMultiple="True"` 启用多选模式，此时：

- 选项列表中每个叶子节点前显示复选框
- 勾选子节点会自动联动父节点状态（全选 / 半选 / 未选 三态）
- 选中项以标签（Tag）形式显示在输入框中
- `SelectedOptions` 属性保存当前选中的全部 `ICascaderOption` 列表
- 可通过 `MaxCount` 限制最大选择数量
- 可通过 `ShowCheckedStrategy` 控制标签展示策略

### 选中策略（ShowCheckedStrategy）

多选模式下，`ShowCheckedStrategy` 控制输入框中标签的展示方式：

| 策略 | 行为 |
|---|---|
| `All` | 显示所有选中的节点标签（默认） |
| `ShowParent` | 当某父节点的所有子节点都被选中时，只显示该父节点的标签 |
| `ShowChild` | 只显示叶子节点的标签 |

### 展开触发方式（ExpandTrigger）

| 值 | 行为 |
|---|---|
| `Click` | 点击选项展开子级（默认） |
| `Hover` | 鼠标悬浮在选项上时自动展开子级，点击选中 |

### 搜索过滤

设置 `IsFilterEnabled="True"` 启用搜索功能。用户输入关键词后：

1. 下拉面板切换为过滤结果列表视图
2. 显示所有路径匹配的叶子节点（如 "Zhejiang/Hangzhou/West Lake"）
3. 匹配的关键词文本高亮显示（颜色由 `FilterHighlightForeground` 控制，默认使用 `ColorError`）
4. 点击过滤结果项直接选中

自定义过滤逻辑可通过实现 `ICascaderItemFilter` 接口并设置 `Filter` 属性来实现。默认使用 `DefaultCascaderItemFilter`（基于 Contains 模式匹配）。

### 异步数据加载

通过实现 `ICascaderItemDataLoader` 接口并设置 `DataLoader` 属性，可以实现动态加载子级数据：

1. 展开某个非叶子节点时，触发 `LoadAsync` 方法
2. 加载期间显示旋转的 `LoadingIcon`
3. 加载完成后自动填充子级选项并展开

### 允许选择父节点（IsAllowSelectParent）

默认情况下只有叶子节点（没有子节点的节点）可以被选中。设置 `IsAllowSelectParent="True"` 后，点击任何层级的节点都可以完成选择。在非叶子节点上需要**双击**才能关闭下拉面板。

### 默认选中路径（DefaultSelectOptionPath）

可通过 `DefaultSelectOptionPath` 属性设置初始选中值，使用 `TreeNodePath` 指定路径：

```csharp
cascader.DefaultSelectOptionPath = new TreeNodePath("zhejiang/hangzhou/xihu");
```

路径中的段与 `CascaderOption` 的 `ItemKey` 或 `Value` 匹配。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本级联选择 | ✅ 逐级展开选择 | ✅ `Cascader` 控件 | ✅ 完全对齐 |
| 默认值 | ✅ `defaultValue` | ✅ `DefaultSelectOptionPath` | ✅ 完全对齐 |
| 悬浮展开 | ✅ `expandTrigger="hover"` | ✅ `ExpandTrigger="Hover"` | ✅ 完全对齐 |
| 禁用选项 | ✅ `disabled` 属性 | ✅ `IsEnabled` 属性 | ✅ 完全对齐 |
| 选择即改变 | ✅ `changeOnSelect` | ✅ `IsAllowSelectParent` | ✅ 完全对齐 |
| 多选 | ✅ `multiple` | ✅ `IsMultiple` | ✅ 完全对齐 |
| 选中策略 | ✅ `showCheckedStrategy` | ✅ `ShowCheckedStrategy` | ✅ 完全对齐 |
| 搜索 | ✅ `showSearch` | ✅ `IsFilterEnabled` | ✅ 完全对齐 |
| 异步加载 | ✅ `loadData` | ✅ `DataLoader` (ICascaderItemDataLoader) | ✅ 完全对齐 |
| 自定义图标 | ✅ `suffixIcon` / `expandIcon` | ✅ `SuffixIcon` / `ExpandIcon` / `LoadingIcon` | ✅ 完全对齐 |
| 前缀 | ✅ `prefix` | ✅ `ContentLeftAddOn` | ✅ 完全对齐 |
| 三种尺寸 | ✅ `size` | ✅ `SizeType` | ✅ 完全对齐 |
| 状态 | ✅ `status` | ✅ `Status` (Error / Warning) | ✅ 完全对齐 |
| 四种变体 | ✅ `variant` | ✅ `StyleVariant` (Outlined / Filled / Borderless / Underlined) | ✅ 完全对齐 |
| 弹出位置 | ✅ `placement` | ✅ `PopupPlacement` | ✅ 完全对齐 |
| 可清除 | ✅ `allowClear` | ✅ `IsAllowClear` | ✅ 完全对齐 |
| 禁用整体 | ✅ `disabled` | ✅ `IsEnabled` | ✅ 完全对齐 |
| `fieldNames` 自定义字段 | ✅ 自定义字段映射 | ❌ 通过接口实现 | ⚠️ 通过 `ICascaderOption` 接口约定字段 |
| `displayRender` 自定义渲染 | ✅ 函数式渲染 | ❌ 暂不支持 | ⚠️ 待支持 |
| `tagRender` 自定义标签 | ✅ 函数式渲染 | ❌ 暂不支持 | ⚠️ 待支持 |

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
              └── AtomUI.Desktop.Controls.Cascader
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础、样式系统、属性继承 |
| `AbstractSelect` | 弹出层管理、尺寸适配、输入框装饰（AddOn）、清除功能、搜索过滤基础设施、多选标签展示、表单集成、紧凑空间、样式变体、状态反馈 |
| `Cascader` | 级联层级数据模型、单选/多选逻辑、选中策略、展开触发方式、异步加载、过滤高亮、默认选中路径、父节点可选 |

**组成控件：**

| 控件 | 角色 |
|---|---|
| `CascaderView` | 级联选项面板，负责展示层级列表、处理展开/折叠/选中/勾选逻辑 |
| `CascaderViewItem` | 单个选项项，承载 Header、Icon、Checkbox、展开箭头 |
| `CascaderViewLevelList` | 单级选项列表，管理同一层级的所有选项 |
| `CascaderViewFilterList` | 搜索过滤结果列表 |
| `CascaderAddOnDecoratedBox` | Cascader 专用输入框装饰器 |
| `CascaderOption` | 选项数据模型记录类 |

**实现的共享接口（通过 AbstractSelect 继承）：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持 `IsMotionEnabled` 动画开关 |
| `ICompactSpaceAware` | `AtomUI.Controls.Shared` | 在 `Space.Compact` 中使用时自动调整圆角和边框 |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | 支持 `Status`（Error / Warning）状态反馈 |
| `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` | 支持样式变体（Outlined / Filled / Borderless / Underlined） |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Cascader/Cascader.cs` | Cascader 主控件 |
| 基类 | `src/AtomUI.Desktop.Controls/Select/AbstractSelect.cs` | 抽象选择器基类 |
| 面板视图 | `src/AtomUI.Desktop.Controls/Cascader/CascaderView.cs` | 级联面板视图 |
| 面板勾选 | `src/AtomUI.Desktop.Controls/Cascader/CascaderView.Check.cs` | 多选勾选逻辑 |
| 面板过滤 | `src/AtomUI.Desktop.Controls/Cascader/CascaderView.Filter.cs` | 搜索过滤逻辑 |
| 面板展开 | `src/AtomUI.Desktop.Controls/Cascader/CascaderView.ExpandAndCollapse.cs` | 展开折叠逻辑 |
| 面板异步 | `src/AtomUI.Desktop.Controls/Cascader/CascaderView.AsyncItemDataLoad.cs` | 异步加载逻辑 |
| 选项项 | `src/AtomUI.Desktop.Controls/Cascader/CascaderViewItem.cs` | 单个选项控件 |
| 选项数据 | `src/AtomUI.Desktop.Controls/Cascader/CascaderOption.cs` | ICascaderOption 接口 + CascaderOption 记录类 |
| 装饰器 | `src/AtomUI.Desktop.Controls/Cascader/CascaderAddOnDecoratedBox.cs` | 输入框装饰控件 |
| 伪类常量 | `src/AtomUI.Desktop.Controls/Cascader/CascaderViewPseudoClass.cs` | 伪类定义 |
| 过滤接口 | `src/AtomUI.Desktop.Controls/Cascader/ICascaderItemFilter.cs` | 自定义过滤接口 |
| 默认过滤 | `src/AtomUI.Desktop.Controls/Cascader/DefaultCascaderItemFilter.cs` | 默认过滤实现 |
| 数据加载 | `src/AtomUI.Desktop.Controls/Cascader/DataLoad/ICascaderItemDataLoader.cs` | 异步加载接口 |
| Token | `src/AtomUI.Desktop.Controls/Cascader/CascaderToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Cascader/Themes/CascaderTheme.axaml` | ControlTheme AXAML |
| 模板常量 | `src/AtomUI.Desktop.Controls/Cascader/Themes/CascaderThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/CascaderShowCase.axaml` | 使用范例 |

---

## 模板结构

Cascader 的 ControlTemplate 采用 Panel 布局，主要分为两个区域：输入框和弹出层。

```
Panel
├── CascaderAddOnDecoratedBox (PART_AddOnDecoratedBox)  ← 输入框装饰器（样式变体、前后附加内容）
│   ├── ContentRightAddOn                                ← 右侧附加区域
│   │   ├── SelectMaxCountIndicator                      ← 最大数量指示器（多选时可见）
│   │   ├── ContentPresenter                             ← 自定义右侧内容
│   │   └── SelectHandle                                 ← 操作手柄（清除按钮 / 加载图标 / 展开箭头）
│   └── Panel                                            ← 内容区域
│       ├── TextBlock#PlaceholderText                    ← 占位文本
│       ├── TextBlock#SingleSelectResultPresenter        ← 单选结果路径文本
│       ├── SelectFilterTextBox (PART_SingleFilterInput)  ← 单选搜索输入框
│       └── SelectTagAwareTextBox#SelectedOptionsBox     ← 多选标签展示区
└── Popup (PART_Popup)                                   ← 弹出层
    └── Border#PopupFrame                                ← 弹出框架
        └── CascaderView (PART_CascaderView)             ← 级联面板视图
```

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `CascaderThemeConstants.CascaderViewPart` | `"PART_CascaderView"` | 级联面板视图 |
| `CascaderThemeConstants.PopupPart` | `"PART_Popup"` | 弹出层 |
| `CascaderThemeConstants.SingleFilterInputPart` | `"PART_SingleFilterInput"` | 单选搜索输入框 |
