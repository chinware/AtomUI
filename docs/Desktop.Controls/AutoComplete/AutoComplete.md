# AutoComplete 自动补全

## 概述

自动补全（AutoComplete）是输入型控件的增强组件，在用户键入文本时根据已有或动态获取的候选数据，实时展示匹配建议的下拉列表，用户可从中选取以快速完成输入。它广泛应用于搜索框联想、标签输入、邮箱/地址自动填充等需要"边输入边提示"的场景。

AtomUI 的 `AutoComplete` 控件对齐了 [Ant Design 5.0 AutoComplete](https://ant.design/components/auto-complete-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为，并在此基础上提供了异步数据加载、SearchEdit 集成、TextArea 集成等桌面端增强能力。

---

## 设计原理

### Ant Design 的自动补全设计哲学

Ant Design 对自动补全的定位是：**「输入框的自动补全功能，根据输入内容提供对应的输入建议」**。它与 Select 组件的本质区别在于：

- **AutoComplete** 是一个带辅助提示的**输入框**，用户的最终值是自由文本，候选只是建议。
- **Select** 是一个**选择器**，用户必须从预定义选项中选取。

Ant Design 的 AutoComplete 核心理念：
1. **实时过滤**：随着用户输入，候选列表实时筛选匹配项
2. **自定义渲染**：候选项可以自定义展示内容（不仅限于纯文本）
3. **灵活组合**：可与 Input、TextArea、Search 等输入组件组合使用
4. **异步加载**：支持远程搜索场景，根据输入内容动态请求候选数据

### Avalonia 基础能力

AtomUI 的 `AbstractAutoComplete` 直接继承自 Avalonia 的 `TemplatedControl`，而非 Avalonia 内置的 `AutoCompleteBox`。这是因为 AtomUI 需要：

1. 更深度地控制弹出层行为（使用 AtomUI 自定义的 `Popup` 和 `CandidateList`）
2. 集成 Design Token 系统和动画系统
3. 支持多种内嵌输入框类型（LineEdit、SearchEdit、TextArea）
4. 与 AtomUI 的 Form 验证体系、CompactSpace 等系统深度集成

继承链为：

```
TemplatedControl
  └── AbstractAutoComplete (核心逻辑)
        └── CompactSpaceAwareAutoComplete (紧凑空间适配)
              └── AutoComplete (标准自动补全)
              └── AutoCompleteSearchEdit (搜索型自动补全)
        └── AutoCompleteTextArea (多行文本自动补全)
```

### AtomUI 的扩展设计

AtomUI AutoComplete 在基础输入能力之上做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **三种输入形态** | `AutoComplete` / `AutoCompleteSearchEdit` / `AutoCompleteTextArea` | 覆盖单行输入、搜索输入、多行输入三种典型场景 |
| **四种样式变体** | `StyleVariant` 枚举（Outline / Filled / Borderless / Underlined） | 对齐 Ant Design 5.0 的 `variant` 属性 |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **验证状态** | `IInputControlStatusAware` 接口 + `Status` 属性 | Error / Warning 状态视觉反馈 |
| **异步数据加载** | `ICompleteOptionsAsyncLoader` 接口 + `OptionsAsyncLoader` 属性 | 支持远程搜索、API 查询等异步场景 |
| **本地过滤** | `IValueFilter` 接口 + `Filter` 属性 | 内置 StartsWith / Contains 等多种过滤模式 |
| **自定义选项模板** | `OptionTemplate`（`IDataTemplate`） | 支持自定义候选项渲染（图标+文字、多列信息等） |
| **弹出方向控制** | `Placement` 属性（Top / Bottom） | 候选列表可在输入框上方或下方弹出 |
| **清除按钮** | `IsAllowClear` + `ClearIcon` | 一键清空输入内容 |
| **附加内容** | `ContentLeftAddOn` / `ContentRightAddOn` | 输入框前后附加图标或自定义内容 |
| **过渡动画** | `IsMotionEnabled` + Popup 动画 | 弹出/关闭候选列表的平滑过渡 |
| **紧凑空间** | `ICompactSpaceAware` 接口 | 在 `Space.Compact` 容器中自动调整圆角和边框 |
| **表单集成** | `IFormItemAware` + `IFormItemFeedbackAware` 接口 | 可参与 FormItem 验证流程 |
| **Design Token** | `AutoCompleteToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 三种输入形态

AutoComplete 家族包含三种控件，分别适用于不同的输入场景：

| 控件 | 内嵌输入框 | 适用场景 |
|---|---|---|
| `AutoComplete` | `LineEdit`（单行输入框） | 最常见的搜索联想、地址补全、标签输入等 |
| `AutoCompleteSearchEdit` | `SearchEdit`（搜索输入框） | 带搜索按钮的搜索联想场景 |
| `AutoCompleteTextArea` | `TextArea`（多行文本框） | 代码编辑器、评论框中的 @提及 等多行输入联想 |

三者共享 `AbstractAutoComplete` 中定义的全部核心逻辑（候选列表管理、过滤、弹出/关闭、键盘导航等），仅在内嵌输入框类型上有差异。

### 数据源与候选列表

AutoComplete 支持两种数据提供方式：

**1. 静态数据源（`OptionsSource`）**

直接设置 `IEnumerable<IAutoCompleteOption>` 集合：

```csharp
autoComplete.OptionsSource = new List<IAutoCompleteOption>
{
    new AutoCompleteOption { Header = "选项一", Content = "选项一" },
    new AutoCompleteOption { Header = "选项二", Content = "选项二" },
};
```

**2. 异步数据加载（`OptionsAsyncLoader`）**

实现 `ICompleteOptionsAsyncLoader` 接口，根据用户输入动态加载候选数据：

```csharp
public class MyLoader : ICompleteOptionsAsyncLoader
{
    public async Task<CompleteOptionsLoadResult> LoadAsync(string? context, CancellationToken token)
    {
        var results = await MyApi.SearchAsync(context, token);
        return new CompleteOptionsLoadResult { Data = results };
    }
}
```

当设置了 `OptionsAsyncLoader` 时，每次用户输入满足 `MinimumPrefixLength` 条件后，自动调用加载器获取候选数据。

### 过滤机制

AutoComplete 提供灵活的过滤能力：

- **默认过滤**：`StartsWith`（前缀匹配，不区分大小写）
- **内置过滤模式**：通过 `{atom:ValueFilterProvider xxx}` 标记扩展在 AXAML 中直接使用
  - `StartsWith` / `StartsWithCaseSensitive` / `StartsWithOrdinal` / `StartsWithOrdinalCaseSensitive`
  - `Contains` / `ContainsCaseSensitive` / `ContainsOrdinal` / `ContainsOrdinalCaseSensitive`
  - `Equals` / `EqualsCaseSensitive` / `EqualsOrdinal` / `EqualsOrdinalCaseSensitive`
- **自定义过滤**：实现 `IValueFilter` 接口
- **自定义取值**：通过 `FilterValueSelector` 委托控制从 `IAutoCompleteOption` 中取哪个字段做匹配

### 弹出候选列表

- 用户输入满足 `MinimumPrefixLength`（默认 1 个字符）后自动弹出
- 可通过 `MinimumPopulateDelay` 设置防抖延迟
- 支持 `DisplayCandidateCount` 控制最大可见候选项数量（默认 10）
- `MaxDropDownHeight` 直接控制弹出层最大高度
- `Placement` 控制弹出方向（`Top` / `Bottom`，默认 `Bottom`）
- `IsPopupMatchSelectWidth` 控制弹出层是否与输入框等宽（默认 `true`）

### 文本自动完成（IsCompletionEnabled）

当 `IsCompletionEnabled = true` 时，输入过程中会自动将最佳匹配项的文本填充到输入框，未输入部分自动选中，用户可继续输入覆盖或按 Enter 确认。

### 键盘导航

| 按键 | 行为 |
|---|---|
| `↓` | 下拉列表关闭时：打开。打开时：移动到下一候选项 |
| `↑` | 移动到上一候选项 |
| `Enter` | 确认当前选中的候选项 |
| `Escape` | 关闭候选列表，恢复输入值 |
| `F4` | 切换候选列表的打开/关闭状态 |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本自动补全 | ✅ | ✅ `AutoComplete` | ✅ 完全对齐 |
| 自定义选项 `options` | ✅ | ✅ `OptionsSource` / `Options` | ✅ 完全对齐 |
| 自定义选项渲染 | ✅ `optionRender` | ✅ `OptionTemplate` | ✅ 对齐（机制不同，效果一致） |
| 尺寸 `size` | ✅ `large / middle / small` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 变体 `variant` | ✅ `outlined / filled / borderless` | ✅ `StyleVariant` + `Underlined` | ✅ 超集（多 Underlined） |
| 状态 `status` | ✅ `error / warning` | ✅ `Status` 属性 | ✅ 完全对齐 |
| 允许清除 `allowClear` | ✅ | ✅ `IsAllowClear` | ✅ 完全对齐 |
| 占位文本 `placeholder` | ✅ | ✅ `PlaceholderText` | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ | ✅ `IsEnabled` | ✅ 完全对齐 |
| 默认值 `defaultValue` | ✅ | ✅ `DefaultValue` | ✅ 完全对齐 |
| 过滤模式 `filterOption` | ✅ | ✅ `Filter` 属性 | ✅ 完全对齐 |
| 弹出位置 `placement` | ✅ `topLeft / bottomLeft` | ✅ `Placement`（Top / Bottom） | ✅ 基本对齐 |
| 下拉宽度匹配 `popupMatchSelectWidth` | ✅ | ✅ `IsPopupMatchSelectWidth` | ✅ 完全对齐 |
| 值变更 `onChange` | ✅ | ✅ `ValueChanged` 事件 | ✅ 完全对齐 |
| 选中变更 `onSelect` | ✅ | ✅ `SelectionChanged` 事件 | ✅ 完全对齐 |
| 搜索 `onSearch` | ✅ | ✅ `Populating` 事件 | ✅ 对齐（名称不同，语义一致） |
| 异步加载 | ✅ (通过 onSearch 手动实现) | ✅ `ICompleteOptionsAsyncLoader` | ✅ 增强（内置异步加载器抽象） |
| TextArea 集成 | ❌ 需手动组合 | ✅ `AutoCompleteTextArea` | ✅ AtomUI 增强 |
| Search 集成 | ❌ 需手动组合 | ✅ `AutoCompleteSearchEdit` | ✅ AtomUI 增强 |
| 回填 `autoComplete` | ✅ | ✅ `IsCompletionEnabled` | ✅ 完全对齐 |
| `open` / `onDropdownVisibleChange` | ✅ | ✅ `IsDropDownOpen` + 事件 | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.AbstractAutoComplete
        ├── implements ISizeTypeAware
        ├── implements IMotionAwareControl
        ├── implements IFormItemAware
        ├── implements IInputControlStatusAware
        ├── implements IInputControlStyleVariantAware
        └── implements IFormItemFeedbackAware
          └── AtomUI.Desktop.Controls.CompactSpaceAwareAutoComplete
                ├── implements ICompactSpaceAware
                │     └── AtomUI.Desktop.Controls.AutoComplete
                │     └── AtomUI.Desktop.Controls.AutoCompleteSearchEdit
          └── AtomUI.Desktop.Controls.AutoCompleteTextArea
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | Avalonia 模板化控件基础设施 |
| `AbstractAutoComplete` | 全部核心逻辑：候选列表管理、过滤、弹出/关闭、键盘导航、异步加载、表单集成、值管理 |
| `CompactSpaceAwareAutoComplete` | 紧凑空间适配能力（在 `Space.Compact` 中自动调整圆角和边框） |
| `AutoComplete` | 标准自动补全：使用 `LineEdit` 作为内嵌输入框 |
| `AutoCompleteSearchEdit` | 搜索型自动补全：使用 `SearchEdit` 作为内嵌输入框，增加搜索按钮相关属性 |
| `AutoCompleteTextArea` | 多行自动补全：使用 `TextArea` 作为内嵌输入框，增加多行相关属性，弹出位置跟随光标 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持过渡动画开关 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |
| `IFormItemFeedbackAware` | `AtomUI.Controls.Shared` | 支持表单验证反馈控件（如错误/警告图标） |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | 支持 Error / Warning 验证状态 |
| `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` | 支持 Outline / Filled / Borderless / Underlined 样式变体 |
| `ICompactSpaceAware` | `AtomUI.Controls.Shared` | 在 `Space.Compact` 中使用时自动调整圆角和边框 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 核心基类 | `src/AtomUI.Desktop.Controls/AutoComplete/AbstractAutoComplete.cs` | 核心逻辑（候选管理、过滤、弹出、键盘导航） |
| 紧凑空间 | `src/AtomUI.Desktop.Controls/AutoComplete/CompactSpaceAwareAutoComplete.cs` | 紧凑空间适配中间类 |
| 标准控件 | `src/AtomUI.Desktop.Controls/AutoComplete/AutoComplete.cs` | 标准自动补全控件 |
| 搜索控件 | `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteSearchEdit.cs` | 搜索型自动补全控件 |
| 多行控件 | `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteTextArea.cs` | 多行文本自动补全控件 |
| 选项接口 | `src/AtomUI.Desktop.Controls/AutoComplete/IAutoCompleteOption.cs` | 候选选项接口 |
| 选项实现 | `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteOption.cs` | 默认候选选项 record |
| 伪类常量 | `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompletePseudoClass.cs` | 伪类定义 |
| Token 定义 | `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteToken.cs` | 组件级 Design Token |
| 模板常量 | `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteThemeConstants.cs` | 模板部件名称常量 |
| 异步加载 | `src/AtomUI.Desktop.Controls/AutoComplete/DataLoad/` | 异步加载器接口与结果模型 |
| 事件参数 | `src/AtomUI.Desktop.Controls/AutoComplete/CompleteValueChangedEventArgs.cs` | 值变更事件 |
| 事件参数 | `src/AtomUI.Desktop.Controls/AutoComplete/CompletePopulatingEventArgs.cs` | 填充开始事件 |
| 事件参数 | `src/AtomUI.Desktop.Controls/AutoComplete/CompletePopulatedEventArgs.cs` | 填充完成事件 |
| 主题模板 | `src/AtomUI.Desktop.Controls/AutoComplete/Themes/AutoCompleteTheme.axaml` | AutoComplete ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/AutoComplete/Themes/AutoCompleteSearchEditTheme.axaml` | SearchEdit 变体 ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/AutoComplete/Themes/AutoCompleteTextAreaTheme.axaml` | TextArea 变体 ControlTheme |
| 基础主题 | `src/AtomUI.Desktop.Controls/AutoComplete/Themes/AbstractAutoCompleteTheme.axaml` | 共享基础样式 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml` | 使用范例 |

---

## 模板结构

所有 AutoComplete 变体共享类似的模板结构，仅内嵌输入框类型不同：

```
Panel
├── InputBox (PART_TextBox)                        ← 内嵌输入框（LineEdit / SearchEdit / TextArea）
│   └── [输入框自身模板内容]
└── Popup (PART_Popup)                             ← 候选列表弹出层
    └── Border#PopupFrame                          ← 弹出层框架（背景、圆角）
        └── CandidateList (PART_CandidateList)     ← 候选列表（单选模式）
            └── [OptionTemplate 渲染每个候选项]
```

**分层设计理由：**
- **输入框独立**：输入框使用对应控件的完整主题（LineEdit / SearchEdit / TextArea），保持各自的视觉一致性。
- **弹出层独立**：Popup 使用 AtomUI 自定义实现，支持动画、点击外部关闭、Overlay 层等功能。
- **候选列表独立**：CandidateList 负责选项渲染和选择交互，与 AutoComplete 的核心逻辑通过事件解耦。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `AutoCompleteThemeConstants.TextBoxPart` | `"PART_TextBox"` | 内嵌输入框 |
| `AutoCompleteThemeConstants.PopupPart` | `"PART_Popup"` | 候选列表弹出层 |
| `AutoCompleteThemeConstants.CandidateListPart` | `"PART_CandidateList"` | 候选列表 |
