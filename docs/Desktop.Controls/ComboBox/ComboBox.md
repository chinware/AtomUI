# ComboBox 组合框

## 概述

组合框（ComboBox）是最基础的下拉选择控件，用户通过点击触发下拉面板，从预定义的选项列表中选择一个值。它广泛用于表单填写、筛选条件设置、配置项选择等场景——当候选项较多时，下拉选择比单选框更节省空间；当需要精确匹配时，下拉选择比自由输入更可靠。

AtomUI 的 `ComboBox` 控件对齐 [Ant Design 5.0 Select (basic)](https://ant.design/components/select-cn) 的基础模式设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的 Select 设计哲学

Ant Design 的 Select 组件定位为：**「弹出一个下拉菜单给用户选择操作，用于代替原生的选择器」**。其核心设计理念包括：

**三种样式变体**（按视觉风格区分）：

| 变体 | 设计意图 | 典型用途 |
|---|---|---|
| 🔲 **描边（Outlined）** | 默认样式，白色背景 + 实线边框，中等视觉权重 | 表单中的标准选择器 |
| 🟫 **填充（Filled）** | 灰色填充背景，无明显边框，较低视觉干扰 | 搜索栏、筛选器等辅助场景 |
| ➖ **无边框（Borderless）** | 无背景无边框，最轻量的视觉呈现 | 表格内嵌、只读展示等场景 |

**两种状态修饰**（用于表单验证反馈）：

| 状态 | 设计意图 |
|---|---|
| ❌ **错误（Error）** | 红色系边框/底线，提示用户当前选择不满足验证规则 |
| ⚠️ **警告（Warning）** | 橙色系边框/底线，提示用户注意但不阻止提交 |

**AddOn 扩展区域**（前后附加内容）：

| 区域 | 设计意图 |
|---|---|
| 🏷️ **前置标签（LeftAddOn）** | 选择器左侧外部附加区域，常用于协议前缀（如 `http://`） |
| 🏷️ **后置标签（RightAddOn）** | 选择器右侧外部附加区域，常用于域名后缀（如 `.com`） |
| 🔤 **内部前缀（ContentLeftAddOn）** | 内容区域左侧，常用于图标或货币符号 |
| 🔤 **内部后缀（ContentRightAddOn）** | 内容区域右侧，常用于单位或辅助图标 |

### Avalonia ComboBox 基础能力

AtomUI 的 `ComboBox` 继承自 Avalonia 框架的 `Avalonia.Controls.ComboBox`。理解 Avalonia ComboBox 的基础能力有助于理解 AtomUI 在其之上做了哪些扩展。

**Avalonia ComboBox 的核心职责：**

Avalonia 的 `ComboBox` 是一个标准的 `SelectingItemsControl`（选择型项目控件），它管理一组选项并允许用户从中选择一个。其继承链为：

```
Control → TemplatedControl → ItemsControl → SelectingItemsControl → ComboBox
```

作为 `SelectingItemsControl`，ComboBox 提供了完整的选择管理（`SelectedItem`、`SelectedIndex`、`SelectionChanged` 事件）、项目容器化（`ComboBoxItem`）、数据绑定（`ItemsSource` + `ItemTemplate`）、以及 `Popup` 弹出面板等基础设施。

**Avalonia ComboBox 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `ItemsSource` | 数据源集合，支持绑定任意 `IEnumerable` |
| `ItemTemplate` | 项目数据模板，定义每个选项的呈现方式 |
| `SelectedItem` | 当前选中项，支持双向绑定 |
| `SelectedIndex` | 当前选中项索引 |
| `PlaceholderText` | 未选中时显示的占位提示文本 |
| `IsDropDownOpen` | 下拉面板是否打开 |
| `MaxDropDownHeight` | 下拉面板最大高度 |
| `ItemsPanel` | 项目面板模板（默认为 `VirtualizingStackPanel`） |

**Avalonia ComboBox 提供的基础伪类：**

| 伪类 | 说明 |
|---|---|
| `:pointerover` | 鼠标悬浮在控件上 |
| `:pressed` | 控件被按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 控件获得焦点 |
| `:focus-visible` | 通过键盘（Tab）获得焦点 |

### AtomUI 的扩展设计

AtomUI `ComboBox` 在 Avalonia ComboBox 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **三种样式变体** | `InputControlStyleVariant` 枚举 + `AddOnDecoratedBox` 委托样式 | 对齐 Ant Design 的 `variant`，支持 Outlined / Filled / Borderless |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **验证状态** | `IInputControlStatusAware` 接口 + `Status` 属性 | 对齐 Ant Design 的 `status`，支持 Error / Warning 视觉反馈 |
| **AddOn 扩展区域** | `AddOnDecoratedBox` 装饰容器 + 8 个 AddOn 属性 | 复刻 Ant Design 的 addonBefore / addonAfter / prefix / suffix |
| **下拉手柄** | `ComboBoxHandle` 内置组件 + `IconButton` | 独立的下拉箭头按钮，带悬浮/按下颜色反馈 |
| **分页下拉高度** | `DropDownDisplayPageSize` 属性 + 自动计算 `MaxDropDownHeight` | 按项目数量控制下拉面板可见行数 |
| **过渡动画** | `IsMotionEnabled` + `Popup` 动画 | 下拉面板展开/收起的平滑过渡动画 |
| **表单集成** | `IFormItemAware` + `IFormItemFeedbackAware` 接口 | 可参与 FormItem 验证流程，自动显示反馈图标 |
| **Design Token** | `ComboBoxToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |

---

## 功能详解

### 样式变体（StyleVariant）

样式变体通过 `StyleVariant` 属性设置，ComboBox 将该属性传递给内部的 `AddOnDecoratedBox` 容器，由后者统一管理边框/背景样式。三种变体的视觉差异：

| 变体 | 正常态 | 聚焦态 | 错误态 |
|---|---|---|---|
| `Outlined` | 白色背景 + 灰色边框 | 主色边框 + 外发光 | 红色边框 + 红色外发光 |
| `Filled` | 灰色填充背景 | 白色背景 + 主色边框 | 红色填充背景 + 红色边框 |
| `Borderless` | 无背景无边框 | 无边框 | 文本变为红色 |

### AddOn 扩展区域

ComboBox 支持四种扩展区域，均通过 `AddOnDecoratedBox` 实现：

| 区域 | 属性 | 位置 | 说明 |
|---|---|---|---|
| 前置标签 | `LeftAddOn` / `LeftAddOnTemplate` | 输入框外部左侧 | 带独立背景，常用于 `http://` 等前缀 |
| 后置标签 | `RightAddOn` / `RightAddOnTemplate` | 输入框外部右侧 | 带独立背景，常用于 `.com` 等后缀 |
| 内部前缀 | `ContentLeftAddOn` / `ContentLeftAddOnTemplate` | 内容区域左侧 | 与选中文本同行，常用于图标 |
| 内部后缀 | `ContentRightAddOn` / `ContentRightAddOnTemplate` | 内容区域右侧（手柄左侧） | 与选中文本同行，常用于辅助信息 |

### 下拉手柄（ComboBoxHandle）

ComboBox 内置一个 `ComboBoxHandle` 组件作为右侧内容附加件。手柄包含一个 `IconButton`（默认显示 `DownOutlined` 箭头图标），点击手柄可切换下拉面板的打开/关闭状态。手柄支持悬浮和按下时的颜色反馈。

### 下拉面板分页

`DropDownDisplayPageSize` 属性控制下拉面板最多可见的选项数量（默认 10 项）。AtomUI 会根据 `DropDownDisplayPageSize × ItemHeight + PopupContentPadding` 自动计算 `MaxDropDownHeight`，超出部分通过滚动条浏览。

### 验证状态（Status）

通过 `Status` 属性设置验证状态，对应的伪类（`:error`、`:warning`）会被激活。不同样式变体下的表现：

- **Outlined / Filled**：边框/背景颜色变为对应的错误/警告色系
- **Borderless**：选中文本的前景色变为对应的错误/警告色

### 表单集成

ComboBox 实现了 `IFormItemAware` 和 `IFormItemFeedbackAware` 接口：
- 当放置在 `FormItem` 中时，表单验证状态会自动映射为 `Status` 属性
- 验证反馈图标通过 `FormFeedback` 属性注入，显示在手柄左侧

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 三种变体 `variant` | ✅ `outlined / filled / borderless` | ✅ `StyleVariant` 枚举 | ✅ 完全对齐 |
| 三种尺寸 `size` | ✅ `large / medium / small` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 验证状态 `status` | ✅ `error / warning` | ✅ `Status` 属性 | ✅ 完全对齐 |
| 占位文本 `placeholder` | ✅ 字符串 | ✅ `PlaceholderText` 属性 | ✅ 完全对齐 |
| 前后附加 `addonBefore / addonAfter` | ✅ ReactNode | ✅ `LeftAddOn` / `RightAddOn` | ✅ 完全对齐 |
| 前后缀 `prefix / suffix` | ✅ ReactNode | ✅ `ContentLeftAddOn` / `ContentRightAddOn` | ✅ 完全对齐 |
| 允许清除 `allowClear` | ✅ 布尔 | ✅ `IsAllowClear` 属性 | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ 布尔 | ✅ `IsEnabled` 属性 | ✅ 完全对齐 |
| 搜索 `showSearch` | ✅ 搜索过滤 | ❌ 暂未支持 | ⚠️ 待支持 |
| 多选 `mode="multiple"` | ✅ 多选/标签 | ❌ 暂未支持 | ⚠️ 待支持 |
| 分组 `optionGroup` | ✅ 选项分组 | ❌ 暂未支持 | ⚠️ 待支持 |
| 虚拟滚动 `virtual` | ✅ 默认开启 | ⚠️ 通过 Avalonia VirtualizingStackPanel 部分支持 | ⚠️ 部分对齐 |
| 下拉面板自定义 `dropdownRender` | ✅ 自定义渲染 | ❌ 暂未支持 | ⚠️ 待支持 |
| 远程搜索 | ✅ 异步加载 | ❌ 暂未支持 | ⚠️ 待支持 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ItemsControl
        └── Avalonia.Controls.SelectingItemsControl
              └── Avalonia.Controls.ComboBox
                    └── AtomUI.Desktop.Controls.ComboBox
                          ├── implements IMotionAwareControl
                          ├── implements ISizeTypeAware
                          ├── implements IInputControlStatusAware
                          ├── implements IInputControlStyleVariantAware
                          ├── implements IFormItemAware
                          └── implements IFormItemFeedbackAware
```

`ComboBox` 通过 `using AvaloniaComboBox = Avalonia.Controls.ComboBox;` 别名引用 Avalonia 原生 ComboBox，避免类名冲突。

> **注意**：ComboBox 没有对应的 `Abstract*` 基类（在 `AtomUI.Controls` 层没有抽象基类），这是因为 ComboBox 直接继承自 Avalonia 的 `ComboBox`，扩展逻辑完全在 `AtomUI.Desktop.Controls` 层实现。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ItemsControl` | 项目集合管理、`ItemsSource` 数据绑定、`ItemTemplate` 模板化、容器回收 |
| `SelectingItemsControl` | 单选管理、`SelectedItem` / `SelectedIndex`、`SelectionChanged` 事件 |
| `Avalonia.Controls.ComboBox` | 下拉弹出 `Popup`、`PlaceholderText` 占位提示、`MaxDropDownHeight`、`IsDropDownOpen`、键盘导航、`SelectionBoxItem` 显示选中内容 |
| `AtomUI.Desktop.Controls.ComboBox` | Ant Design 视觉体系（三种变体/三种尺寸/验证状态）、AddOn 扩展区域、Design Token 集成、下拉手柄、分页下拉高度、表单集成 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持 `IsMotionEnabled` 动画开关 |
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | 支持 `Status`（Error / Warning / Default）验证状态 |
| `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` | 支持 `StyleVariant`（Outlined / Filled / Borderless）样式变体 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |
| `IFormItemFeedbackAware` | `AtomUI.Controls.Shared` | 可接收并显示表单验证反馈控件 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/ComboBox/ComboBox.cs` | 桌面端 ComboBox 具体实现 |
| 控件类 | `src/AtomUI.Desktop.Controls/ComboBox/ComboBoxHandle.cs` | 下拉手柄组件（内部） |
| 控件类 | `src/AtomUI.Desktop.Controls/ComboBox/ComboBoxItem.cs` | 下拉选项容器 |
| 反射扩展 | `src/AtomUI.Desktop.Controls/ComboBox/ComboBoxReflectionExtensions.cs` | Avalonia ComboBox 内部字段访问 |
| Token 定义 | `src/AtomUI.Desktop.Controls/ComboBox/ComboBoxToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/ComboBox/Themes/ComboBoxTheme.axaml` | ComboBox ControlTheme |
| 选项主题 | `src/AtomUI.Desktop.Controls/ComboBox/Themes/ComboBoxItemTheme.axaml` | ComboBoxItem ControlTheme |
| 手柄主题 | `src/AtomUI.Desktop.Controls/ComboBox/Themes/ComboBoxHandleTheme.axaml` | ComboBoxHandle ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/ComboBox/Themes/ComboBoxThemes.axaml` | ResourceDictionary 聚合注册 |
| 模板常量 | `src/AtomUI.Desktop.Controls/ComboBox/Themes/ComboBoxThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Navigation/ComboBoxShowCase.axaml` | 使用范例 |

---

## 模板结构

ComboBox 的 ControlTemplate 采用 Panel 布局，分为两个主要区域：**AddOn 装饰容器**和**弹出下拉面板**。

```
Panel
├── AddOnDecoratedBox (PART_AddOnDecoratedBox)    ← 装饰容器（管理边框/背景/AddOn 区域）
│   ├── [LeftAddOn]                                ← 前置标签（外部左侧）
│   ├── [ContentLeftAddOn]                         ← 内部前缀（内容左侧）
│   ├── Panel                                      ← 内容区域
│   │   ├── TextBlock#PlaceholderText              ← 占位提示文本（未选中时显示）
│   │   └── ContentPresenter#SelectedContentPresenter ← 选中内容展示器
│   ├── StackPanel (ContentRightAddOn)             ← 内部后缀区域
│   │   ├── ContentPresenter (ContentRightAddOn)   ← 用户自定义后缀
│   │   ├── ContentPresenter#FormFeedBack          ← 表单验证反馈图标
│   │   └── ComboBoxHandle                         ← 下拉箭头手柄
│   └── [RightAddOn]                               ← 后置标签（外部右侧）
└── Popup (PART_Popup)                             ← 弹出下拉面板
    └── Border#PopupFrame                          ← 弹出框架（背景/圆角/阴影）
        └── ScrollViewer                           ← 滚动容器
            └── ItemsPresenter (PART_ItemsPresenter) ← 选项列表
```

**分层设计理由：**
- **AddOnDecoratedBox 统一管理**：所有输入型控件（TextBox、ComboBox、ButtonSpinner 等）共用 `AddOnDecoratedBox` 作为外层装饰，确保样式变体（Outlined/Filled/Borderless）、验证状态、AddOn 区域的一致表现。
- **Popup 独立**：弹出面板使用 `atom:Popup`（AtomUI 增强版），支持动画展开/收起（`IsMotionAwareOpen`）和自动阴影。
- **手柄内置**：`ComboBoxHandle` 作为固定的内部后缀元素，始终显示在最右侧，确保下拉箭头的交互一致性。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `ComboBoxThemeConstants.PopupPart` | `"PART_Popup"` | 弹出下拉面板 |
| `ComboBoxThemeConstants.SpinnerInnerBoxPart` | `"PART_SpinnerInnerBox"` | 内部输入框（预留） |
| `ComboBoxThemeConstants.ItemsPresenterPart` | `"PART_ItemsPresenter"` | 选项列表展示器 |
| `ComboBoxHandleThemeConstants.OpenIndicatorButtonPart` | `"PART_OpenIndicatorButton"` | 下拉指示器按钮 |

### ComboBoxItem 模板结构

```
ContentPresenter#ContentPresenter              ← 选项内容展示器
    Content → StringToTextBlockConverter        ← 字符串自动转 TextBlock
    Background / Padding / CornerRadius         ← 由 Token 控制
    Margin                                      ← 由 ComboBoxToken.ItemMargin 控制
```

### ComboBoxHandle 模板结构

```
IconButton (PART_OpenIndicatorButton)          ← 下拉指示器按钮
    Icon = DownOutlined                         ← Ant Design 向下箭头图标
```
