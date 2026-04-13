# MessageBox 消息弹框

## 概述

消息弹框（MessageBox）用于重要操作的二次确认或重要信息的告知。它以模态弹窗的形式呈现，包含标题、内容正文、语义图标和操作按钮，要求用户明确做出响应后才能继续操作。MessageBox 支持 Overlay（页面内浮层）和 Window（原生窗口）两种宿主模式。

AtomUI 的 `MessageBox` 对标 Ant Design 5.0 中 [Modal.confirm / Modal.info / Modal.success / Modal.error / Modal.warning](https://ant.design/components/modal-cn#modalmethod) 的设计规范。它与 AtomUI 的 `Dialog` 系统深度集成，在 Dialog 基础设施之上封装了预设的语义样式和按钮组合。

---

## 设计原理

### Ant Design 的确认弹窗设计哲学

Ant Design 的 `Modal.method()` 系列（confirm / info / success / error / warning）是一套快捷的静态方法，用于在不需要手动管理组件状态的情况下快速弹出一个带语义图标的确认/提示弹窗。其核心设计要点：

- **语义化图标**：每种样式自动匹配对应的颜色和图标，帮助用户快速理解信息性质
- **预设按钮组合**：Confirm 类型自动提供「确认 + 取消」按钮，其他类型仅提供「确认」按钮
- **异步/同步调用**：支持 Promise 风格的异步等待用户操作结果
- **轻量级 API**：无需在 JSX 中声明组件，通过函数调用即可弹出

**六种样式类型**：

| 样式 | 图标 | 按钮组合 | 设计意图 |
|---|---|---|---|
| **Normal** | 无图标 | OK | 普通消息框，无语义修饰 |
| **Confirm** | `ExclamationCircleFilled`（黄色） | OK + Cancel | 需要用户确认的操作 |
| **Information** | `InfoCircleFilled`（蓝色） | OK | 信息通知 |
| **Success** | `CheckCircleFilled`（绿色） | OK | 操作成功反馈 |
| **Warning** | `ExclamationCircleFilled`（黄色） | OK | 警告信息 |
| **Error** | `CloseCircleFilled`（红色） | OK | 错误信息 |

### AtomUI 的架构设计

AtomUI 的 MessageBox 建立在 Dialog 基础设施之上，采用组合模式而非继承。其架构由以下层次组成：

```
MessageBox（公共 API 层）
  └── MessageBoxDialog（内部 Dialog 子类）
       ├── MessageBoxDialogHost（Window 宿主模式）
       │    └── 基于 DialogHost，添加 StyleIcon 支持
       └── MessageBoxOverlayDialogHost（Overlay 宿主模式）
            └── 基于 OverlayDialogHost，添加 StyleIcon 支持
```

**两种使用方式**：
1. **声明式（AXAML）**：在 AXAML 中声明 `<atom:MessageBox>` 控件，通过 `IsOpen` 双向绑定控制开关
2. **命令式（静态 API）**：通过 `MessageBox.ShowMessageBox()` / `MessageBox.ShowMessageBoxAsync()` 等静态方法在代码中直接调用

**两种宿主模式**：
- **Overlay（默认）**：作为浮层覆盖在当前窗口内容上方，不创建新窗口
- **Window**：创建独立的原生窗口承载弹窗，适用于需要独立窗口的场景

---

## 功能详解

### 消息样式（MessageBoxStyle）

通过 `Style` 属性设置，控制图标类型、颜色和默认按钮组合：

| 样式 | 默认图标 | 图标颜色 | 默认按钮 |
|---|---|---|---|
| `Normal` | 无 | — | OK |
| `Confirm` | `ExclamationCircleFilled` | `SharedToken.ColorWarning` | OK + Cancel |
| `Information` | `InfoCircleFilled` | `SharedToken.ColorInfo` | OK |
| `Success` | `CheckCircleFilled` | `SharedToken.ColorSuccess` | OK |
| `Warning` | `ExclamationCircleFilled` | `SharedToken.ColorWarning` | OK |
| `Error` | `CloseCircleFilled` | `SharedToken.ColorError` | OK |

### 确认按钮样式（OkButtonStyle）

通过 `OkButtonStyle` 属性控制确认按钮的外观：
- `Primary`（默认）：确认按钮使用 Primary 样式（蓝色实心）
- `Default`：确认按钮使用 Default 样式（白色边框）

### 加载状态

- `IsLoading`：整个弹窗处于加载状态
- `IsConfirmLoading`：仅确认按钮显示加载状态，适用于异步确认操作场景

### 位置控制

- `IsCenterOnStartup`（默认 `true`）：弹窗初始居中显示
- `HorizontalOffset` / `VerticalOffset`：自定义偏移量（使用 `Dimension` 类型，支持像素和百分比）
- `PlacementTarget`：放置目标控件引用

### 拖拽移动

通过 `IsDragMovable = true` 启用标题栏拖拽移动功能。

### 轻量关闭

通过 `IsLightDismissEnabled = true`（仅 Overlay 模式有效）启用点击遮罩层关闭弹窗。

### 自定义按钮

通过 `CustomButtons` 集合添加自定义按钮，或通过 `ButtonsConfigure` 回调配置标准按钮的文本和行为。

### 静态 API

MessageBox 提供了一组静态方法，支持在代码中快速创建和显示消息弹框：

| 方法 | 说明 |
|---|---|
| `ShowMessageBox(content, dataContext?, options?, topLevel?)` | 同步显示，返回操作结果 |
| `ShowMessageBox<TView, TViewModel>(dataContext?, options?, topLevel?)` | 泛型版同步显示 |
| `ShowMessageBoxAsync<TView, TViewModel>(...)` | 异步显示，通过回调获取结果 |
| `ShowMessageBoxModalAsync<TView, TViewModel>(...)` | 异步模态显示，返回 `Task<object?>` |
| `ShowMessageAsync(content, ...)` | 异步显示自定义内容 |
| `ShowMessageModalAsync(content, ...)` | 异步模态显示自定义内容 |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 语义样式 | ✅ `confirm / info / success / error / warning` | ✅ `MessageBoxStyle` 枚举 + Normal | ✅ 完全对齐 |
| 自动匹配图标 | ✅ | ✅ 根据 Style 自动设置 | ✅ 完全对齐 |
| 预设按钮组合 | ✅ | ✅ Confirm → OK+Cancel，其他 → OK | ✅ 完全对齐 |
| 静态方法 API | ✅ `Modal.confirm()` 等 | ✅ `MessageBox.ShowMessageBoxAsync()` 等 | ✅ 对齐 |
| 异步等待结果 | ✅ Promise 模式 | ✅ `Task<object?>` 模式 | ✅ 对齐 |
| 自定义图标 | ✅ `icon` 属性 | ✅ `Icon` 属性 | ✅ 对齐 |
| 自定义按钮文本 | ✅ `okText` / `cancelText` | ✅ `OkButtonText` / `CancelButtonText` | ✅ 对齐 |
| 确认按钮加载 | ✅ `confirmLoading` | ✅ `IsConfirmLoading` | ✅ 对齐 |
| 关闭回调 | ✅ `onCancel` / `afterClose` | ✅ `Cancelled` / `Closed` 事件 | ✅ 对齐 |
| Overlay 宿主 | ❌ 仅 DOM 弹层 | ✅ Overlay + Window 双模式 | ✅ 扩展 |
| 拖拽移动 | ❌ | ✅ `IsDragMovable` | ✅ 扩展（桌面端增强） |
| AXAML 声明式使用 | ❌ 仅 API 调用 | ✅ 声明式 + API 双模式 | ✅ 扩展 |

---

## 继承关系

### MessageBox

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.MessageBox
        └── implements IMotionAwareControl
```

### 内部类继承链

```
Dialog (AtomUI Dialog 基类)
  └── MessageBoxDialog（内部，承载弹窗逻辑）

DialogHost
  └── MessageBoxDialogHost（内部，Window 宿主）

OverlayDialogHost
  └── MessageBoxOverlayDialogHost（内部，Overlay 宿主）
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础设施 |
| `MessageBox` | 公共 API 层：属性暴露、静态方法、事件转发、图标和按钮自动配置 |
| `MessageBoxDialog`（内部） | 继承 `Dialog`，添加 `Icon` / `Style` 属性，创建对应的宿主 |
| `MessageBoxDialogHost`（内部） | Window 宿主模式下的渲染，包含 StyleIcon 展示 |
| `MessageBoxOverlayDialogHost`（内部） | Overlay 宿主模式下的渲染，包含 StyleIcon 展示 |

**与 Dialog 系统的关系：**

MessageBox 不直接继承自 Dialog，而是在模板中包含一个 `MessageBoxDialog`（`PART_Dialog`），通过属性绑定将公共属性传递给内部 Dialog。这种组合模式使 MessageBox 可以拥有简化的公共 API，同时复用 Dialog 的全部基础设施（宿主管理、按钮系统、动画、位置计算等）。

**实现的接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | 支持动画启用/禁用控制 |
| `IMessageBoxActionResult` | `AtomUI.Desktop.Controls` | 消息弹框操作结果契约（`Result` 属性） |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/MessageBox/MessageBox.cs` | MessageBox 公共 API |
| 样式枚举 | `src/AtomUI.Desktop.Controls/MessageBox/MessageBoxStyle.cs` | MessageBoxStyle 枚举 |
| 选项类 | `src/AtomUI.Desktop.Controls/MessageBox/MessageBoxOptions.cs` | 静态 API 选项 record |
| 结果接口 | `src/AtomUI.Desktop.Controls/MessageBox/IMessageBoxActionResult.cs` | 操作结果接口 |
| 结果实现 | `src/AtomUI.Desktop.Controls/MessageBox/MessageBoxActionResult.cs` | 操作结果实现 |
| 内部 Dialog | `src/AtomUI.Desktop.Controls/MessageBox/MessageBoxDialog.cs` | 内部 Dialog 子类 |
| Window 宿主 | `src/AtomUI.Desktop.Controls/MessageBox/MessageBoxDialogHost.cs` | Window 模式宿主 |
| Overlay 宿主 | `src/AtomUI.Desktop.Controls/MessageBox/MessageBoxOverlayDialogHost.cs` | Overlay 模式宿主 |
| Token 定义 | `src/AtomUI.Desktop.Controls/MessageBox/MessageBoxToken.cs` | 组件级 Design Token |
| 主题（MessageBox） | `src/AtomUI.Desktop.Controls/MessageBox/Themes/MessageBoxTheme.axaml` | MessageBox ControlTheme |
| 主题（DialogHost） | `src/AtomUI.Desktop.Controls/MessageBox/Themes/MessageBoxDialogHostTheme.axaml` | Window 宿主主题 |
| 主题（OverlayHost） | `src/AtomUI.Desktop.Controls/MessageBox/Themes/MessageBoxOverlayDialogHostTheme.axaml` | Overlay 宿主主题 |
| 主题注册 | `src/AtomUI.Desktop.Controls/MessageBox/Themes/MessageBoxThemes.axaml` | 主题合并注册 |
| 模板常量 | `src/AtomUI.Desktop.Controls/MessageBox/Themes/MessageBoxThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ModalShowCase.axaml` | 使用范例（"MessageBox Style" 部分） |

---

## 模板结构

### MessageBox 模板

MessageBox 本身的模板非常简洁——它只包含一个 `MessageBoxDialog`，所有属性通过 TemplateBinding 传递：

```
MessageBoxDialog (PART_Dialog)                    ← 内部 Dialog，承载全部弹窗逻辑
```

### MessageBoxDialogHost 模板（Window 宿主模式）

```
Panel
├── Border#WindowFrame                             ← 窗口背景框
├── Border#BodyFrame                               ← 主体框架
│   └── VisualLayerManager
│       └── DockPanel#ContentLayout
│           ├── WindowTitleBar (PART_TitleBar)      ← 标题栏（DockPanel.Dock="Top"）
│           └── DockPanel
│               ├── Border#FooterFrame              ← 底部按钮区（DockPanel.Dock="Bottom"）
│               │   └── DialogButtonBox (PART_ButtonBox)
│               └── Border#ContentFrame             ← 内容区
│                   └── DockPanel
│                       ├── IconPresenter#StyleIconPresenter  ← 语义图标
│                       └── ContentPresenter                  ← 自定义内容
└── WindowResizer                                  ← 窗口缩放器
```

### MessageBoxOverlayDialogHost 模板（Overlay 宿主模式）

```
Panel
├── Border#ShadowFrame                             ← 阴影框
├── Border#Frame                                   ← 主框架
│   └── DockPanel#RootLayout
│       ├── OverlayDialogHeader (PART_Header)      ← 标题栏 + 关闭按钮（DockPanel.Dock="Top"）
│       ├── Border#FooterFrame                     ← 底部按钮区（DockPanel.Dock="Bottom"）
│       │   └── DialogButtonBox (PART_ButtonBox)
│       └── Border#ContentFrame                    ← 内容区
│           └── DockPanel
│               ├── IconPresenter#StyleIconPresenter  ← 语义图标
│               └── ContentPresenter                  ← 自定义内容
└── OverlayDialogResizer                           ← Overlay 缩放器
```

### 模板部件常量

| 常量名 | 值 | 所属 | 说明 |
|---|---|---|---|
| `MessageBoxThemeConstants.DialogPart` | `"PART_Dialog"` | MessageBox | 内部 MessageBoxDialog 引用 |
