# Form 表单

## 概述

表单（Form）是数据录入场景中最核心的复合控件，用于收集、校验和提交用户输入。表单将多个数据录入控件（输入框、选择器、日期选择器等）组织在统一的布局中，提供字段级和表单级的验证能力，以及提交/重置等操作流程。

AtomUI 的 `Form` 控件完整复刻了 [Ant Design 5.0 Form](https://ant.design/components/form-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的表单体验。表单系统由多个协作控件组成：`Form`（容器）、`FormItem`（字段项）、`FormActionsItem`（操作区）、`SubmitButton`（提交按钮）、`ResetButton`（重置按钮）和 `FormItemDecorator`（装饰器）。

---

## 设计原理

### Ant Design 的表单设计哲学

Ant Design 对表单的定位是：**「高性能表单控件，自带数据域管理。包含数据录入、校验以及对应样式」**。其核心设计理念包括：

**三种表单布局**：

| 布局 | 设计意图 | 典型用途 |
|---|---|---|
| 📐 **水平布局（Horizontal）** | 标签在左、控件在右，一行一个字段。适合字段数量适中、标签宽度统一的场景 | 注册表单、设置页面、数据编辑 |
| 📏 **垂直布局（Vertical）** | 标签在上、控件在下，每个字段独占一行。标签不受宽度限制 | 移动端表单、标签文本较长的场景 |
| ➡️ **内联布局（Inline）** | 所有字段水平排列在同一行，紧凑布局 | 搜索栏、筛选条件、导航栏登录 |

**验证体系**：

| 特性 | 说明 |
|---|---|
| 🔴 **必填标记** | 视觉提示用户该字段必填，支持 Default / Optional / Hidden / Customize 四种标记模式 |
| ⚡ **验证触发** | 支持值变化时（OnChanged）、失焦时（OnBlur）、提交时（OnSubmit）三种触发方式 |
| 🕐 **防抖验证** | 通过 `ValidateDebounce` 降低高频验证对后端的压力 |
| 🔗 **验证策略** | 遇到第一个错误即停（StopWhenFirstFailed）、顺序执行（Sequential）、并行执行（Parallel） |
| ⚠️ **仅警告规则** | `WarningOnly` 标记的验证器不阻止表单提交，仅显示警告信息 |
| 🔄 **验证反馈** | 在输入控件内显示验证状态图标（成功✓ / 警告⚠ / 错误✗ / 验证中⏳） |

### Avalonia 基础能力

AtomUI 的 `Form` 继承自 Avalonia 的 `ItemsControl`，而 `FormItem` 继承自 `TemplatedControl`。

**Avalonia ItemsControl 的核心职责：**

`ItemsControl` 是 Avalonia 中用于展示集合数据的基础控件，它管理一组子项（Items），支持容器化（ContainerForItemOverride）和数据模板化。`Form` 利用这一机制将每个 `FormItem` 作为子项进行管理。

```
Control → TemplatedControl → ItemsControl → Form
Control → TemplatedControl → FormItem
```

### AtomUI 的扩展设计

AtomUI `Form` 在 Avalonia ItemsControl 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **三种表单布局** | `FormLayout` 枚举（Horizontal / Vertical / Inline） | 对齐 Ant Design 的 `layout` 属性 |
| **字段验证系统** | `IFormValidator` 接口 + `FormValidatorProvider` + 内置验证器 | 声明式验证配置，支持异步验证 |
| **三种验证触发** | `FormValidateTrigger` 枚举 | 对齐 Ant Design 的 `validateTrigger` |
| **三种验证策略** | `FormValidateStrategy` 枚举 | 支持短路、顺序、并行三种执行方式 |
| **四种必填标记** | `FormRequiredMark` 枚举 + 自定义标记模板 | 对齐 Ant Design 的 `requiredMark` |
| **表单操作路由** | `SubmitButton` / `ResetButton` 路由事件冒泡 | 任意位置的提交/重置按钮自动关联到 Form |
| **初始值/重置** | `InitialValues` + `Reset()` | 支持表单数据的初始化和重置 |
| **表单值收集** | `IFormValues` / `FormValues` | 统一的表单值收集与提交 |
| **动态表单项** | `Items.Add()` / `DeleteFormItem()` | 运行时动态增删表单字段 |
| **尺寸统一** | `ISizeTypeAware` | 全局控制所有子控件尺寸 |
| **样式变体** | `IInputControlStyleVariantAware` | Outlined / Filled / Borderless / Underlined |
| **验证反馈图标** | `IsValidateFeedbackEnabled` + `FormValidateFeedback` | 输入框内显示验证状态图标 |
| **标签列比例** | `LabelColInfo` / `WrapperColInfo`（`MediaBreakGridLength`） | 响应式标签与内容宽度分配 |
| **Design Token** | `FormToken` + `RegisterTokenResourceScope` | 所有视觉值从全局 Token 派生 |

---

## 功能详解

### 表单布局（FormLayout）

布局通过 `FormLayout` 属性设置。Form 根据布局模式决定内部排列方向和 FormItem 的标签/内容排布方式。

| 布局 | Form 排列方向 | FormItem 内部布局 | 说明 |
|---|---|---|---|
| `Horizontal` | 垂直（一行一个 FormItem） | 标签在左、内容在右（`FormItemLayout.Horizontal`） | 默认布局，最常用 |
| `Vertical` | 垂直（一行一个 FormItem） | 标签在上、内容在下（`FormItemLayout.Vertical`） | 标签独占一行 |
| `Inline` | 水平（多个 FormItem 同一行） | 标签在左、内容在右（`FormItemLayout.Horizontal`） | 紧凑搜索栏 |

**FormItem 的 Layout 属性可以独立覆盖**：在 `Horizontal` 或 `Vertical` 的表单中，个别 FormItem 可以通过设置自身的 `Layout` 属性来使用不同的排布方式。

### 标签与内容宽度（LabelColInfo / WrapperColInfo）

在水平布局中，标签和内容区域的宽度通过 `MediaBreakGridLength` 控制，支持响应式断点：

```xml
<!-- 固定比例 -->
<atom:Form LabelColInfo="8*" WrapperColInfo="16*" />

<!-- 固定像素宽度 -->
<atom:Form LabelColInfo="200" WrapperColInfo="*" />

<!-- 单个 FormItem 覆盖 -->
<atom:FormItem LabelColInfo="4*" WrapperColInfo="20*" Layout="Horizontal" />
```

### 验证生命周期

1. **触发**：用户操作（值变化 / 失焦 / 提交）触发验证
2. **防抖**：如果设置了 `ValidateDebounce`，延迟指定时间后执行
3. **执行**：按 `ValidateStrategy` 执行 `Validators` 列表中的验证器
4. **结果**：每个验证器返回 `FormValidateResult`（Success / Error / Warning）
5. **通知**：更新 `ValidateStatus`、错误/警告消息、通知子控件更新状态图标
6. **汇总**：Form 级别汇总所有 FormItem 的验证结果，更新 `IsFormValid`

### IFormItemAware 集成

所有可参与表单验证的控件必须实现 `IFormItemAware` 接口：

```csharp
public interface IFormItemAware
{
    event EventHandler ValueChanged;      // 值变化事件
    void SetFormValue(object? value);     // 外部设置值
    object? GetFormValue();               // 获取当前值
    void ClearFormValue();                // 清除值（重置）
    void NotifyValidateStatus(FormValidateStatus status); // 通知验证状态
}
```

AtomUI 内置的数据录入控件（`LineEdit`、`Select`、`DatePicker`、`CheckBox` 等）均已实现此接口。自定义控件需要实现该接口才能参与表单验证。

### FormItemDecorator

当一个 FormItem 内需要在主控件旁边附加额外内容（如单位文本）时，使用 `FormItemDecorator` 包装：

```xml
<atom:FormItem LabelText="InputNumber" FieldName="inputNumber">
    <atom:FormItemDecorator>
        <atom:NumericUpDown Minimum="1" Maximum="10" Width="120"/>
        <atom:FormItemDecorator.Extra>
            machines
        </atom:FormItemDecorator.Extra>
    </atom:FormItemDecorator>
</atom:FormItem>
```

### 提交/重置按钮路由

`SubmitButton` 和 `ResetButton` 通过路由事件（Bubble）与 Form 通信，无论放在 Form 内部的任何位置，事件都会冒泡到最近的 Form 容器：

- `SubmitButton.SubmitEvent` → `Form.HandleSubmitButtonClick` → `Form.Submit()`
- `ResetButton.ResetEvent` → `Form.HandleResetButtonClick` → `Form.Reset()`

### 动态表单项

Form 支持在运行时动态添加和删除 FormItem：

- **添加**：`form.Items.Add(newFormItem)` 或 `form.Items.Insert(index, newFormItem)`
- **删除**：`form.DeleteFormItem(formItem)` 或设置 `IsShowItemDeleteButton` 显示删除按钮
- **最少数量**：`MinItemCount` 控制最少保留的数据项数量，少于此数量时删除按钮自动隐藏

### SubmitButton 的 IsWatchValidateResult

当 `IsWatchValidateResult="True"` 时，`SubmitButton` 会实时监听 Form 的 `IsFormValid` 属性变化，自动启用/禁用按钮。只有当所有必填字段验证通过后，提交按钮才变为可用状态。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 三种布局 `layout` | ✅ `horizontal / vertical / inline` | ✅ `FormLayout` 枚举 | ✅ 完全对齐 |
| 字段校验 `rules` | ✅ 声明式规则数组 | ✅ `Validators` 属性 + `FormValidatorProvider` | ✅ 对齐（语法不同，能力一致） |
| 验证触发 `validateTrigger` | ✅ `onChange / onBlur / onSubmit` | ✅ `FormValidateTrigger` 枚举 | ✅ 完全对齐 |
| 必填标记 `requiredMark` | ✅ `boolean / 'optional'` | ✅ `FormRequiredMark` 枚举（Default / Hidden / Optional / Customize） | ✅ 扩展对齐（增加 Customize） |
| 表单方法 `submit / reset` | ✅ 实例方法 | ✅ `Submit()` / `Reset()` 方法 | ✅ 完全对齐 |
| 初始值 `initialValues` | ✅ 对象属性 | ✅ `InitialValues` 属性 | ✅ 完全对齐 |
| 动态增删字段 `Form.List` | ✅ 专用组件 | ✅ `Items.Add/Remove` + `DeleteFormItem` | ⚠️ 方式不同，能力覆盖 |
| 仅警告规则 `warningOnly` | ✅ 规则属性 | ✅ `IFormValidator.WarningOnly` | ✅ 完全对齐 |
| 验证反馈 `hasFeedback` | ✅ 属性 | ✅ `IsValidateFeedbackEnabled` | ✅ 完全对齐 |
| 标签对齐 `labelAlign` | ✅ `left / right` | ✅ `FormLabelAlign` 枚举 | ✅ 完全对齐 |
| 标签换行 `labelWrap` | ✅ 布尔属性 | ✅ `LabelWrapping` 属性 | ✅ 完全对齐 |
| 尺寸 `size` | ✅ `small / middle / large` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 变体 `variant` | ✅ `outlined / filled / borderless` | ✅ `StyleVariant` + Underlined | ✅ 扩展对齐 |
| 显示冒号 `colon` | ✅ 布尔属性 | ✅ `IsShowColon` 属性 | ✅ 完全对齐 |
| 字段联动 `dependencies` | ✅ 声明式 | ✅ `NotifyFormItemChanged` | ⚠️ 基础支持 |
| `Form.Provider` | ✅ 多表单联动 | ❌ 暂未实现 | ⚠️ 待支持 |
| `useWatch` | ✅ Hook | ❌ 不适用（非 React） | — 平台差异 |
| `scrollToFirstError` | ✅ 布尔/对象 | ✅ `ScrollToFirstError` 属性（预留） | ⚠️ 属性已定义 |

---

## 继承关系

```
Form（表单容器）:
  Avalonia.Controls.Primitives.TemplatedControl
    └── Avalonia.Controls.ItemsControl
          └── AtomUI.Desktop.Controls.Form
                ├── implements ISizeTypeAware
                ├── implements IMotionAwareControl
                ├── implements IForm
                └── implements IInputControlStyleVariantAware

FormItem（表单字段项）:
  Avalonia.Controls.Primitives.TemplatedControl
    └── AtomUI.Desktop.Controls.FormItem
          └── implements IFormItem

FormActionsItem（操作区）:
  AtomUI.Desktop.Controls.FormItem
    └── AtomUI.Desktop.Controls.FormActionsItem

SubmitButton / ResetButton:
  AtomUI.Desktop.Controls.Button
    ├── AtomUI.Desktop.Controls.SubmitButton
    └── AtomUI.Desktop.Controls.ResetButton

FormItemDecorator（字段装饰器）:
  Avalonia.Controls.Primitives.TemplatedControl
    └── AtomUI.Desktop.Controls.FormItemDecorator
          ├── implements IFormItemAware
          ├── implements IFormItemFeedbackAware
          ├── implements ISizeTypeAware
          ├── implements IMotionAwareControl
          └── implements IInputControlStyleVariantAware
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ItemsControl` | 子项集合管理（Items）、容器创建（CreateContainerForItemOverride）、数据模板化 |
| `Form` | 表单布局（三种）、全局验证配置、提交/重置流程、值收集、动态增删、Design Token 集成 |
| `FormItem` | 字段标签、验证器绑定、验证状态管理、错误消息展示、标签/内容响应式布局 |
| `FormActionsItem` | 非数据项容器（用于放置按钮等操作控件），跳过验证 |
| `SubmitButton` / `ResetButton` | 路由事件触发提交/重置，`IsWatchValidateResult` 自动禁用 |
| `FormItemDecorator` | 包装子控件并附加额外内容，代理 `IFormItemAware` 接口 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 表单容器 | `src/AtomUI.Desktop.Controls/Form/Form.cs` | Form 桌面端实现 |
| 表单字段项 | `src/AtomUI.Desktop.Controls/Form/FormItem.cs` | FormItem / FormActionsItem |
| 字段装饰器 | `src/AtomUI.Desktop.Controls/Form/FormItemDecorator.cs` | FormItemDecorator |
| 提交按钮 | `src/AtomUI.Desktop.Controls/Form/SubmitButton.cs` | SubmitButton |
| 重置按钮 | `src/AtomUI.Desktop.Controls/Form/ResetButton.cs` | ResetButton |
| 删除按钮 | `src/AtomUI.Desktop.Controls/Form/ItemDeleteButton.cs` | ItemDeleteButton |
| Token 定义 | `src/AtomUI.Desktop.Controls/Form/FormToken.cs` | 组件级 Design Token |
| 枚举定义 | `src/AtomUI.Controls/Form/FormEnums.cs` | 共享枚举（跨平台） |
| 接口定义 | `src/AtomUI.Controls/Form/IForm.cs` | IForm 接口 |
| 接口定义 | `src/AtomUI.Controls/Form/IFormItem.cs` | IFormItem 接口 |
| 接口定义 | `src/AtomUI.Controls/Form/IFormItemAware.cs` | IFormItemAware / IFormItemFeedbackAware |
| 接口定义 | `src/AtomUI.Controls/Form/IFormValidator.cs` | IFormValidator 接口 |
| 验证器基类 | `src/AtomUI.Controls/Form/Validators/AbstractFormValidator.cs` | 验证器抽象基类 |
| 内置验证器 | `src/AtomUI.Controls/Form/Validators/FormStringNotEmptyValidator.cs` | 字符串非空验证 |
| 内置验证器 | `src/AtomUI.Controls/Form/Validators/FormStringLengthValidator.cs` | 字符串长度验证 |
| 内置验证器 | `src/AtomUI.Controls/Form/Validators/FormUrlValidator.cs` | URL 格式验证 |
| 内置验证器 | `src/AtomUI.Controls/Form/Validators/FormNotNullValidator.cs` | 非空验证 |
| 内置验证器 | `src/AtomUI.Controls/Form/Validators/FormAssertValidator.cs` | 断言验证（用于测试） |
| 主题模板 | `src/AtomUI.Desktop.Controls/Form/Themes/FormTheme.axaml` | Form ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Form/Themes/FormItemTheme.axaml` | FormItem ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Form/Themes/FormItemDecoratorTheme.axaml` | FormItemDecorator ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Form/Themes/SubmitButtonTheme.axaml` | SubmitButton ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Form/Themes/ResetButtonTheme.axaml` | ResetButton ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/Form/Themes/FormThemes.axaml` | 主题资源注册 |
| 模板常量 | `src/AtomUI.Desktop.Controls/Form/Themes/FormThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/FormShowCase.axaml` | 使用范例 |
