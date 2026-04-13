# Dialog 对话框

## 概述

对话框（Dialog）是一个从界面中弹出的模态或非模态面板，用于承载需要用户关注或交互的内容。典型使用场景包括：确认操作、表单填写、详情展示、流程引导等。对话框在出现时会在视觉上「聚焦」用户注意力，阻止（模态）或不阻止（非模态）用户与底层界面的交互。

AtomUI 的 `Dialog` 控件对应 Ant Design 的 [Modal 组件](https://ant.design/components/modal-cn)。在 AtomUI 中采用 `Dialog` 命名（而非 `Modal`），是因为 `Dialog` 更准确地描述了桌面端的语义——桌面应用中「对话框」是长期沿用的标准术语，且 AtomUI 的实现不仅支持模态（Modal），还支持非模态（Modeless）模式。

---

## 设计原理

### Ant Design 的对话框设计哲学

Ant Design 对 Modal 的定位是：**在当前页面打开一个浮层，承载相关操作或信息。用户不需要离开当前页面就能完成任务或获取信息**。

**典型使用场景：**

| 场景 | 说明 |
|---|---|
| 🔔 **信息确认** | 操作前的二次确认（如删除、提交），避免误操作 |
| 📝 **表单填写** | 在不离开当前页面的情况下完成新建/编辑等操作 |
| 📄 **详情展示** | 展示详细信息或预览内容 |
| ⚠️ **消息提示** | 成功、错误、警告、确认等快捷提示（MessageBox） |

### AtomUI 的扩展设计

AtomUI `Dialog` 在 Ant Design Modal 基础上做了大量桌面端增强，核心设计决策包括：

**双宿主架构（Dual Host）：**

| 宿主类型 | 说明 | 适用场景 |
|---|---|---|
| `Overlay` | 基于 Overlay 层渲染，在当前窗口内覆盖显示 | 大多数场景（默认）；对话框内容与主窗口共享渲染上下文 |
| `Window` | 创建独立的原生窗口作为宿主 | 需要独立标题栏、系统窗口管理器交互、最小化/最大化等 |

**标准按钮系统（Standard Buttons）：**

Dialog 提供了一套完整的标准按钮系统，基于 `DialogStandardButton` 枚举（Flags），支持 `Ok`、`Cancel`、`Save`、`Yes`、`No`、`Apply`、`Reset` 等 18 种标准按钮。每个标准按钮都有预定义的 `DialogButtonRole`（AcceptRole / RejectRole / DestructiveRole 等），Dialog 根据角色自动决定点击后的行为（Accept / Reject / Done）。

**定位系统（Positioning）：**

Dialog 提供了灵活的定位能力：
- `HorizontalStartupLocation` / `VerticalStartupLocation`：锚点定位（Left / Center / Right / Top / Bottom / Custom）
- `HorizontalOffset` / `VerticalOffset`：偏移量，支持像素和百分比（`Dimension` 类型）
- `PlacementTarget`：定位参考控件
- `CustomDialogPlacementCallback`：完全自定义定位回调

**MVVM 集成：**

通过 `IDialogAwareDataContext` 接口，ViewModel 可以感知自身被关联到哪个 Dialog 实例，从而在 ViewModel 层主动关闭对话框、获取结果等。

---

## 功能详解

### 宿主类型（DialogHostType）

通过 `DialogHostType` 属性选择宿主：

- **Overlay（默认）**：对话框渲染在当前窗口的 Overlay 层上，通过 `OverlayDialogHost` 实现。支持拖拽移动、调整大小、遮罩层。
- **Window**：对话框在独立的 `DialogHost`（继承自 `Window`）中渲染。支持系统窗口标题栏、最小化、最大化、原生拖拽。

### 模态与非模态

- **模态（IsModal = true，默认）**：打开后阻止用户与底层界面交互。`OpenAsync()` 返回的 Task 在对话框关闭后才完成，可以直接 `await` 获取 `Result`。
- **非模态（IsModal = false）**：打开后不阻止用户操作底层界面。`OpenAsync()` 在对话框显示后立即完成。

### 标准按钮与自定义按钮

- **标准按钮**：通过 `StandardButtons` 属性配置，例如 `StandardButtons="Ok, Cancel"`。
- **默认按钮**：通过 `DefaultStandardButton` 指定哪个标准按钮为默认确认按钮（视觉上高亮显示）。
- **自定义按钮**：通过 `CustomButtons` 集合添加 `DialogButton` 实例，可自定义文本、角色和样式。
- **按钮属性配置回调**：`ButtonsConfigure` 回调可在按钮创建后统一修改属性（如禁用所有按钮）。

### 加载状态

- **IsLoading**：整体加载状态，对话框内容区域显示加载遮罩。
- **IsConfirmLoading**：确认按钮加载状态。当 `IsConfirmLoading = true` 时，对话框不会响应关闭请求，防止用户在异步操作完成前关闭对话框。

### 拖拽与调整大小

- **IsDragMovable**：启用标题栏拖拽移动。
- **IsResizable**：启用对话框边缘拖拽调整大小。
- **IsMaximizable**：启用最大化按钮。

### 关闭流程

Dialog 提供三种关闭方式：
1. **Accept()**：设置 `Result = DialogCode.Accepted`，触发 `Accepted` 事件后关闭。
2. **Reject()**：设置 `Result = DialogCode.Rejected`，触发 `Rejected` 事件后关闭。
3. **Done(result?)**：设置自定义 Result 后关闭，触发 `Finished` 事件。

关闭前会触发 `Closing` 事件（`CancelEventArgs`），可以通过设置 `Cancel = true` 阻止关闭。当 `IsConfirmLoading = true` 时，所有关闭请求均被忽略。

**按钮角色到关闭行为的映射：**

当底部按钮被点击时，Dialog 首先触发 `ButtonClicked` 事件。若未被 `Handled`，则根据按钮角色自动执行：

| 按钮角色 | 关闭行为 |
|---|---|
| `AcceptRole` / `YesRole` / `ApplyRole` / `ResetRole` | 调用 `Accept()`（Result = Accepted） |
| `RejectRole` / `NoRole` | 调用 `Reject()`（Result = Rejected） |
| `ActionRole` / `HelpRole` / `CustomRole` / `DestructiveRole` | 不自动关闭，需在 `ButtonClicked` 事件中自行处理 |

### 静态 API

Dialog 提供了一组静态方法，用于在代码中快速创建和显示对话框，无需在 AXAML 中声明：

- `Dialog.ShowDialog(...)` / `Dialog.ShowDialogModal(...)` — 同步打开
- `Dialog.ShowDialogAsync(...)` / `Dialog.ShowDialogModalAsync(...)` — 异步打开
- 泛型版本 `Dialog.ShowDialogModalAsync<TView, TViewModel>(...)` — 用于自定义视图 + ViewModel

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 模态弹窗 | ✅ `Modal` | ✅ `Dialog` | ✅ 完全对齐（命名不同） |
| 标题 | ✅ `title` | ✅ `Title` 属性 | ✅ 完全对齐 |
| 内容 | ✅ `children` | ✅ `Content` 属性 | ✅ 完全对齐 |
| 页脚按钮 | ✅ `footer` / `okText` / `cancelText` | ✅ `StandardButtons` + `CustomButtons` | ✅ 增强：支持 18 种标准按钮 |
| 确认加载 | ✅ `confirmLoading` | ✅ `IsConfirmLoading` | ✅ 完全对齐 |
| 关闭按钮 | ✅ `closable` | ✅ `IsClosable` | ✅ 完全对齐 |
| 遮罩层 | ✅ `mask` | ✅ Overlay 模式自带遮罩 | ✅ 完全对齐 |
| 点击遮罩关闭 | ✅ `maskClosable` | ✅ `IsLightDismissEnabled` | ✅ 完全对齐 |
| 居中显示 | ✅ `centered` | ✅ `HorizontalStartupLocation="Center"` | ✅ 完全对齐（更灵活） |
| 宽度 | ✅ `width` | ✅ `Width` / `MinWidth` / `MaxWidth` | ✅ 完全对齐 |
| 加载中 | ✅ `loading` | ✅ `IsLoading` | ✅ 完全对齐 |
| 拖拽移动 | ❌ 不支持 | ✅ `IsDragMovable` | ⭐ AtomUI 增强 |
| 调整大小 | ❌ 不支持 | ✅ `IsResizable` | ⭐ AtomUI 增强 |
| 窗口宿主 | ❌ 不适用（Web） | ✅ `DialogHostType.Window` | ⭐ AtomUI 桌面端增强 |
| 最大化/最小化 | ❌ 不适用 | ✅ `IsMaximizable` / `IsMinimizable` | ⭐ AtomUI 桌面端增强 |
| 标准按钮系统 | ❌ 仅 Ok/Cancel | ✅ 18 种标准按钮 + 角色系统 | ⭐ AtomUI 增强 |
| MVVM 集成 | ❌ 不适用 | ✅ `IDialogAwareDataContext` | ⭐ AtomUI 增强 |
| `Modal.confirm()` | ✅ 静态方法 | ✅ `Dialog.ShowDialogModalAsync()` | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.Dialog
        ├── implements IDialogHostProvider
        ├── implements IMotionAwareControl
        └── implements IDialog
```

`Dialog` 直接继承自 Avalonia 的 `TemplatedControl`，未使用 `ContentControl` 或 `Window`。内容通过 `Content` 属性传递给宿主（`OverlayDialogHost` 或 `DialogHost`）进行渲染。Dialog 本身不参与可视化树的测量和排列（`MeasureCore` 返回空尺寸），其唯一职责是管理对话框的生命周期和状态。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础设施 |
| `Dialog` | 对话框生命周期管理（打开/关闭/结果）、双宿主架构、标准按钮系统、定位系统、加载状态、拖拽/调整大小、MVVM 集成 |

**实现的接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IDialog` | `AtomUI.Desktop.Controls` | 对话框核心协议：Title、Result、生命周期事件、Accept/Reject/Done |
| `IDialogHostProvider` | `AtomUI.Desktop.Controls` | 提供对宿主（DialogHost）的访问 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持 `IsMotionEnabled` 动画开关 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Dialog/Dialog.cs` | Dialog 核心实现 |
| 静态 API | `src/AtomUI.Desktop.Controls/Dialog/Dialog.StaticAPI.cs` | 静态方法 |
| 配置选项 | `src/AtomUI.Desktop.Controls/Dialog/DialogOptions.cs` | 静态 API 的配置参数 |
| 接口 | `src/AtomUI.Desktop.Controls/Dialog/IDialog.cs` | IDialog 接口 |
| MVVM 接口 | `src/AtomUI.Desktop.Controls/Dialog/IDialogAwareDataContext.cs` | ViewModel 感知接口 |
| 宿主类型 | `src/AtomUI.Desktop.Controls/Dialog/DialogHostType.cs` | Overlay / Window 枚举 |
| 定位枚举 | `src/AtomUI.Desktop.Controls/Dialog/DialogPlacement.cs` | 水平/垂直锚点枚举 |
| 按钮系统 | `src/AtomUI.Desktop.Controls/Dialog/ButtonBox/` | DialogButton、DialogButtonRole、DialogStandardButton 等 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Dialog/DialogToken.cs` | 组件级 Design Token |
| 主题 | `src/AtomUI.Desktop.Controls/Dialog/Themes/` | ControlTheme 及 code-behind |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ModalShowCase.axaml` | 使用范例 |

---

## 模板结构

Dialog 本身不直接渲染可视内容，而是将属性代理给宿主。根据 `DialogHostType` 的不同，使用不同的宿主模板：

### Overlay 宿主模板结构

```
OverlayDialogHost
├── OverlayDialogMask                           ← 遮罩层（模态时显示）
└── OverlayDialogResizer (PART_Resizer)          ← 调整大小控件
    └── StackPanel
        ├── OverlayDialogHeader (PART_Header)    ← 标题栏（标题、图标、关闭/最大化按钮）
        ├── ContentPresenter                     ← 内容区域
        └── DialogButtonBox (PART_ButtonBox)     ← 底部按钮区域
```

### Window 宿主模板结构

```
DialogHost (extends Window)
├── 系统窗口标题栏                                ← 原生标题栏（标题、最小化/最大化/关闭）
└── StackPanel
    ├── ContentPresenter                         ← 内容区域
    └── DialogButtonBox (PART_ButtonBox)         ← 底部按钮区域
```

### 模板部件常量

| 常量名 | 值 | 所属类 | 说明 |
|---|---|---|---|
| `OverlayDialogThemeConstants.ResizerPart` | `"PART_Resizer"` | OverlayDialogHost | 调整大小控件 |
| `OverlayDialogThemeConstants.HeaderPart` | `"PART_Header"` | OverlayDialogHost | 标题栏 |
| `OverlayDialogHeaderThemeConstants.CloseButtonPart` | `"PART_CloseButton"` | OverlayDialogHeader | 关闭按钮 |
| `OverlayDialogHeaderThemeConstants.MaximizeButtonPart` | `"PART_MaximizeButton"` | OverlayDialogHeader | 最大化按钮 |
| `DialogThemeConstants.ButtonBoxPart` | `"PART_ButtonBox"` | Dialog/DialogHost | 按钮区域 |
