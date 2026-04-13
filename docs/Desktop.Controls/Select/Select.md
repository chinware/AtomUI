# Select 选择器

## 概述

下拉选择器（Select）是数据录入场景中最常见的表单控件之一，用于在大量选项中选择一个或多个值。它通过弹出下拉面板呈现可选项列表，配合搜索过滤、远程加载、分组等高级功能，满足从简单枚举到复杂数据源的各种选择需求。

AtomUI 的 `Select` 控件完整复刻了 [Ant Design 5.0 Select](https://ant.design/components/select-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的 Select 设计哲学

Ant Design 对 Select 的定位是：**「弹出一个下拉菜单给用户选择操作，用于代替原生的选择器，或者需要一个更优雅的多选器时」**。其核心设计准则：

**三种选择模式**（满足不同业务场景）：

| 模式 | 设计意图 | 典型用途 |
|---|---|---|
| 🔘 **单选（Single）** | 从选项列表中选择唯一值，选中后下拉关闭 | 城市选择、状态筛选、分类选择 |
| ☑️ **多选（Multiple）** | 从选项列表中选择多个值，以标签（Tag）形式展示已选项 | 标签选择、权限分配、多条件筛选 |
| 🏷️ **标签（Tags）** | 在多选基础上允许用户自由输入创建新标签 | 文章标签、自定义分类 |

**三种样式变体**（对齐输入控件家族）：

| 变体 | 设计意图 |
|---|---|
| **Outlined** | 默认外观，有明确的边框轮廓，适合大多数表单场景 |
| **Filled** | 填充背景，适合需要视觉层次感的场景 |
| **Borderless** | 无边框，适合嵌入表格或其他容器中的轻量选择 |

**核心交互特性**：

- **搜索过滤**：展开时可通过输入关键字过滤选项
- **清空操作**：可选的清除按钮一键清空已选值
- **异步加载**：支持从远程数据源动态加载选项
- **分组展示**：选项支持按分类分组，配合分组标题
- **响应式标签**：多选模式下标签自动折叠，超出显示计数
- **自定义渲染**：通过模板自定义下拉选项的展示方式

### Avalonia 基础能力

AtomUI 的 `Select` 并非继承自 Avalonia 内置的 `ComboBox`，而是基于 `TemplatedControl` 全新实现。这一设计决策的原因：

1. Ant Design Select 的功能远超传统 ComboBox（多选、标签、搜索过滤、异步加载等）
2. 需要对 Popup、选项列表、标签面板等进行完全定制化的控制
3. 与 AtomUI 的 Design Token 系统和 AddOnDecoratedBox 装饰器体系深度集成

**继承链：**

```
Control → TemplatedControl → AbstractSelect → Select
```

### AtomUI 的扩展设计

AtomUI `Select` 基于 `TemplatedControl` 从零构建，实现了以下能力：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **三种选择模式** | `SelectMode` 枚举（Single / Multiple / Tags） | 对齐 Ant Design 的 `mode` 属性 |
| **三种样式变体** | `IInputControlStyleVariantAware` 接口 + `StyleVariant` | 对齐 Ant Design 的 `variant`，支持 Outlined / Filled / Borderless |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **搜索过滤** | `IsFilterEnabled` + `Filter` + `FilterValueSelector` | 展开时输入关键字实时过滤选项 |
| **异步加载** | `OptionsLoader` + `ISelectOptionsAsyncLoader` 接口 | 支持从远程数据源异步加载选项 |
| **分组展示** | `IsGroupEnabled` + `GroupPropertySelector` | 选项按分组标题聚合展示 |
| **清空操作** | `IsAllowClear` + `SelectHandle` 清除按钮 | 一键清空已选值 |
| **响应式标签** | `IsResponsiveTagMode` + `MaxTagCount` | 多选标签自动折叠，超出显示 `+N` 计数 |
| **最大选择数** | `MaxCount` + `IsShowMaxCountIndicator` | 限制最大可选数量，超出后选项禁用 |
| **下拉弹出** | 内置 `Popup` + 动画支持 | 下拉面板支持动画打开/关闭 |
| **AddOn 装饰** | 继承 `AddOnDecoratedBox` 属性 | 支持前/后附加内容区域 |
| **表单集成** | `IFormItemAware` + `IFormItemFeedbackAware` 接口 | 参与 FormItem 验证流程，支持 Error/Warning 状态 |
| **紧凑空间** | `ICompactSpaceAware` 接口 | 在 `Space.Compact` 容器中自动调整圆角和边框 |
| **Design Token** | `SelectToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |

---

## 功能详解

### 选择模式（SelectMode）

选择模式通过 `Mode` 属性设置：

| 模式 | 行为 | 已选展示 |
|---|---|---|
| `Single` | 单选，选中后自动关闭下拉。支持搜索输入框替代已选文本 | 直接显示已选项的 `Header` 文本 |
| `Multiple` | 多选，选中后不关闭下拉。每个已选项以 Tag 形式展示 | `SelectTag` 标签列表 + 可选的搜索输入框 |
| `Tags` | 在多选基础上，输入内容无匹配时自动创建新选项 | 同 `Multiple`，但支持动态添加选项 |

**Tags 模式的特殊行为**：
- 搜索功能强制启用（`IsEffectiveFilterEnabled = true`）
- 当搜索无匹配结果时，自动创建临时 `SelectOption`（`IsDynamicAdded = true`）
- 选中后动态选项保留；取消选中时自动从选项列表移除

### 搜索过滤

当 `IsFilterEnabled = true`（或 `Mode = Tags`）时启用搜索过滤：

- **Single 模式**：展开时显示输入框，输入关键字过滤选项
- **Multiple/Tags 模式**：标签区域内嵌输入框，支持边选边搜

过滤机制：
- 默认按 `Header` 属性匹配（`HeaderFilterPropertySelector`）
- 可通过 `FilterValueSelector` 自定义匹配字段
- 可通过 `Filter` 属性提供自定义匹配逻辑（`IValueFilter` 实现）

### 异步加载

通过 `OptionsLoader` 属性绑定 `ISelectOptionsAsyncLoader` 实现：

1. 首次点击打开时触发 `LoadAsync`
2. 加载期间显示 Loading 图标（`SuffixLoadingIcon`）
3. 加载完成后自动填充选项并展开下拉
4. 支持通过 `OptionsAsyncLoadContext` 传递加载上下文
5. 提供 `OptionsLoading` / `OptionsLoaded` 事件监听加载过程

### 分组展示

当 `IsGroupEnabled = true` 时启用分组：
- 选项通过 `Group` 属性（`IGroupListItemData`）进行分组
- 可通过 `GroupPropertySelector` 自定义分组属性提取逻辑
- 每个分组显示分组标题

### 响应式标签

多选模式下处理大量已选项的展示：

| 属性 | 功能 |
|---|---|
| `MaxTagCount` | 固定最多显示 N 个标签，超出部分折叠为 `+N` 计数 |
| `IsResponsiveTagMode` | 根据控件实际宽度自动计算可见标签数量 |
| `MaxTagPlaceholder` | 自定义折叠后的显示文本 |

### 下拉弹出控制

| 属性 | 功能 |
|---|---|
| `IsDropDownOpen` | 控制下拉是否打开 |
| `IsDefaultOpen` | 初始时是否默认打开 |
| `Placement` | 下拉面板弹出位置（`BottomEdgeAlignedLeft` / `TopEdgeAlignedLeft` 等） |
| `IsPopupMatchSelectWidth` | 下拉面板是否与 Select 控件等宽（默认 `true`） |
| `DisplayPageSize` | 下拉面板最多显示的选项条数（默认 10，控制最大高度） |

### 键盘交互

| 按键 | 行为 |
|---|---|
| `F4` / `Alt+Down` / `Alt+Up` | 切换下拉展开/关闭 |
| `Down` / `Up`（下拉关闭时） | 打开下拉 |
| `Escape`（下拉打开时） | 关闭下拉 |
| `Enter` / `Space`（下拉关闭时） | 打开下拉 |
| `Backspace` / `Delete`（多选模式） | 删除最后一个已选标签 |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 三种模式 `mode` | ✅ `/ multiple / tags` | ✅ `SelectMode` 枚举 | ✅ 完全对齐 |
| 三种变体 `variant` | ✅ `outlined / filled / borderless` | ✅ `StyleVariant` 属性 | ✅ 完全对齐 |
| 三种尺寸 `size` | ✅ `large / middle / small` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 搜索过滤 `showSearch` | ✅ 布尔属性 | ✅ `IsFilterEnabled` | ✅ 完全对齐 |
| 自定义过滤 `filterOption` | ✅ 函数 | ✅ `Filter`（`IValueFilter`） | ✅ 对齐（类型不同，语义一致） |
| 允许清空 `allowClear` | ✅ 布尔/对象 | ✅ `IsAllowClear` 布尔 | ✅ 完全对齐 |
| 远程加载 | ✅ `filterOption={false}` | ✅ `OptionsLoader` + `ISelectOptionsAsyncLoader` | ✅ 对齐（API 不同，能力一致） |
| 最大选择数 `maxCount` | ✅ 数值 | ✅ `MaxCount` 属性 | ✅ 完全对齐 |
| 响应式标签 `maxTagCount` | ✅ `responsive` / 数值 | ✅ `IsResponsiveTagMode` / `MaxTagCount` | ✅ 完全对齐 |
| 分组 `OptGroup` | ✅ 组件 | ✅ `IsGroupEnabled` + `Group` 属性 | ✅ 对齐（声明式 → 属性驱动） |
| 自定义渲染 `optionRender` | ✅ 函数 | ✅ `OptionTemplate`（`IDataTemplate`） | ✅ 对齐（模板化方式） |
| 弹出位置 `placement` | ✅ `bottomLeft` 等 | ✅ `Placement` 枚举 | ✅ 完全对齐 |
| 占位文本 `placeholder` | ✅ 字符串 | ✅ `PlaceholderText` | ✅ 完全对齐 |
| 默认值 `defaultValue` | ✅ 任意类型 | ✅ `DefaultValues` 属性 | ✅ 完全对齐 |
| 前/后附加 `prefix` / `suffixIcon` | ✅ 自定义内容 | ✅ `ContentLeftAddOn` / `SuffixIcon` | ✅ 完全对齐 |
| 隐藏已选 | ✅ 通过 `filterOption` 实现 | ✅ `IsHideSelectedOptions` | ✅ 完全对齐 |
| 状态 `status` | ✅ `error / warning` | ✅ `Status` 属性 | ✅ 完全对齐 |
| 下拉宽度匹配 `popupMatchSelectWidth` | ✅ 布尔/数值 | ✅ `IsPopupMatchSelectWidth` | ⚠️ 仅支持布尔，不支持自定义宽度值 |
| `labelInValue` | ✅ 返回 label 和 value | ❌ 暂未支持 | — 通过 `ISelectOption` 可获取全部信息 |
| `virtual` 虚拟滚动 | ✅ 默认开启 | ✅ 通过 `VirtualizingStackPanel` 实现 | ✅ 完全对齐 |
| 空状态 `notFoundContent` | ✅ 自定义内容 | ✅ `EmptyIndicator` / `EmptyIndicatorTemplate` | ✅ 完全对齐 |

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
              └── AtomUI.Desktop.Controls.Select
                    └── 注册 SelectToken.ScopeProvider
```

> 注意：`AbstractSelect` 和 `Select` 均定义在 `AtomUI.Desktop.Controls` 命名空间下（位于 `src/AtomUI.Desktop.Controls/Select/` 目录）。当前 Select 不存在跨平台基类层（`AtomUI.Controls` 中无 AbstractSelect），所有逻辑均在桌面控件层实现。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础设施、`OnApplyTemplate`、样式系统 |
| `AbstractSelect` | 下拉选择器通用基础设施：Popup 管理（打开/关闭/动画）、下拉展开/折叠事件、占位文本、输入框样式变体（Outlined/Filled/Borderless）、验证状态（Error/Warning）、AddOn 装饰器属性、紧凑空间支持、表单集成、清除按钮逻辑、弹出位置管理 |
| `Select` | 具体选择行为实现：三种选择模式（Single/Multiple/Tags）、选项数据管理（`Options` / `OptionsSource`）、选中值管理（`SelectedOption` / `SelectedOptions`）、搜索过滤、异步加载、分组展示、标签管理、默认值处理、键盘交互 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持 `IsMotionEnabled` 动画开关 |
| `ICompactSpaceAware` | `AtomUI.Controls.Shared` | 在 `Space.Compact` 中使用时自动调整圆角和边框 |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | 支持 `Status`（Error / Warning）验证状态 |
| `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` | 支持 `StyleVariant`（Outlined / Filled / Borderless）样式变体 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |
| `IFormItemFeedbackAware` | `AtomUI.Controls.Shared` | 接收表单验证反馈控件 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Desktop.Controls/Select/AbstractSelect.cs` | 下拉选择器通用基础设施 |
| 控件类 | `src/AtomUI.Desktop.Controls/Select/Select.cs` | Select 主控件类 |
| 异步加载 | `src/AtomUI.Desktop.Controls/Select/Select.AsyncOptionsLoad.cs` | 异步选项加载逻辑（partial class） |
| 选项接口 | `src/AtomUI.Desktop.Controls/Select/SelectOption.cs` | `ISelectOption` 接口 + `SelectOption` 记录类 |
| 伪类常量 | `src/AtomUI.Desktop.Controls/Select/SelectPseudoClass.cs` | 伪类定义 |
| 弹出位置枚举 | `src/AtomUI.Desktop.Controls/Select/SelectPopupPlacement.cs` | 弹出位置枚举 |
| 选择变更事件 | `src/AtomUI.Desktop.Controls/Select/SelectSelectionChangedEventArgs.cs` | 选择变更事件参数 |
| 异步加载接口 | `src/AtomUI.Desktop.Controls/Select/DataLoad/ISelectOptionsAsyncLoader.cs` | 异步加载器接口 |
| 加载结果 | `src/AtomUI.Desktop.Controls/Select/DataLoad/SelectOptionsLoadResult.cs` | 加载结果类 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Select/SelectToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Select/Themes/SelectTheme.axaml` | ControlTheme AXAML |
| 模板常量 | `src/AtomUI.Desktop.Controls/Select/Themes/SelectThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` | 使用范例 |

---

## 模板结构

Select 的 ControlTemplate 采用 Panel 布局，将选择器区域和弹出下拉面板分离：

```
Panel
├── SelectAddOnDecoratedBox (PART_AddOnDecoratedBox)     ← 输入框装饰容器（边框、AddOn 区域）
│   ├── [ContentRightAddOn]                                ← 右侧附加区域
│   │   ├── SelectMaxCountIndicator                        ← 最大选择数指示器（可选显示）
│   │   ├── ContentPresenter (ContentRightAddOn)           ← 用户自定义右侧内容
│   │   └── SelectHandle                                   ← 下拉箭头/清除/加载图标区域
│   └── Panel                                              ← 内容主区域
│       ├── TextBlock#PlaceholderText                      ← 占位文本
│       ├── ContentPresenter#SingleSelectResultPresenter   ← 单选结果展示（仅 Single 模式可见）
│       ├── SelectFilterTextBox (PART_SingleFilterInput)   ← 单选搜索输入框（仅 Single+Filter 可见）
│       └── SelectResultOptionsBox#SelectedOptionsBox      ← 多选标签+搜索区域（仅 Multiple/Tags 可见）
└── Popup (PART_Popup)                                     ← 下拉弹出面板
    └── Border#PopupFrame                                  ← 弹出框架（背景、圆角、阴影）
        └── SelectCandidateList (PART_CandidateList)       ← 候选选项列表（虚拟滚动）
```

**分层设计理由：**
- **AddOnDecoratedBox 装饰**：统一输入控件的边框、AddOn 和样式变体系统，与 Input、AutoComplete 等控件保持一致的外观风格
- **SelectHandle 独立**：集中管理下拉箭头、清除按钮、加载图标和表单反馈控件的切换逻辑
- **弹出面板独立**：Popup 使用 OverlayLayer，不影响主控件布局

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `SelectThemeConstants.CandidateListPart` | `"PART_CandidateList"` | 候选选项列表 |
| `SelectThemeConstants.PopupPart` | `"PART_Popup"` | 下拉弹出面板 |
| `SelectThemeConstants.SingleFilterInputPart` | `"PART_SingleFilterInput"` | 单选搜索输入框 |
| `SelectHandleThemeConstants.ClearButtonPart` | `"PART_ClearButton"` | 清除按钮 |
