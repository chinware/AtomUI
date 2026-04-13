# PopupConfirm 气泡确认框

## 概述

气泡确认框（PopupConfirm）是一个点击触发的弹出式确认控件，在目标元素附近弹出带有标题、内容描述和操作按钮的气泡卡片。它适用于轻量级的确认场景——当操作不需要全屏遮罩的模态对话框，但又需要用户二次确认时（如删除一条记录、取消一个任务），PopupConfirm 是最佳选择。

AtomUI 的 `PopupConfirm` 控件对齐 [Ant Design 5.0 Popconfirm](https://ant.design/components/popconfirm-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的气泡确认框设计哲学

Ant Design 对 Popconfirm 的定位是：**「在操作元素附近弹出气泡式确认框，供用户对操作进行二次确认」**。与模态对话框（Modal）相比：

| 维度 | Modal（模态对话框） | Popconfirm（气泡确认框） |
|---|---|---|
| **视觉权重** | 高 — 全屏遮罩 + 居中弹窗 | 低 — 附着在触发元素旁的小气泡 |
| **打断程度** | 强 — 遮挡全部内容 | 弱 — 仅占据局部空间 |
| **适用场景** | 重要/复杂确认（如删除整个项目） | 轻量确认（如删除一条记录） |
| **触发方式** | 通常由 API 调用触发 | 直接附着在交互元素上 |

**设计要素：**

- **警示图标**：默认显示 `ExclamationCircleFilled` 警告图标，颜色根据 `ConfirmStatus` 变化（Info → 主色、Warning → 警告色、Error → 错误色），让用户快速感知操作性质。
- **标题 + 描述**：`Title` 为必要的确认标题，`ConfirmContent` 为可选的详细描述，二者配合向用户传达完整的操作后果。
- **操作按钮**：右下角放置「取消」和「确认」按钮（均为 Small 尺寸），确认按钮默认为 `Primary` 类型，引导用户注意。
- **12 种弹出方位**：通过 `Placement` 属性支持上/下/左/右及其边缘对齐共 12 种位置，自动适配不同布局场景。

### Avalonia / FlyoutHost 基础能力

AtomUI 的 `PopupConfirm` 继承自 `FlyoutHost`，而 `FlyoutHost` 继承自 Avalonia 的 `ContentControl`。`FlyoutHost` 是 AtomUI 为所有需要弹出浮层的控件（Tooltip、Popover、PopupConfirm 等）提供的通用基类。

**FlyoutHost 的核心职责：**

- **内容宿主**：作为 `ContentControl`，它包裹触发弹出的目标元素（如 Button）
- **弹出管理**：通过内部 `FlyoutStateHelper` 管理弹出层的显示/隐藏，支持 Click、Hover、Focus 三种触发方式
- **属性中继**：将自身的 `Placement`、`IsShowArrow`、`IsPointAtCenter`、`MarginToAnchor`、`IsMotionEnabled` 等属性自动中继绑定到内部 `Flyout` 对象
- **动画支持**：通过 `IMotionAwareControl` 接口支持弹出/收起动画

**FlyoutHost 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Flyout` | 内部弹出层对象（PopupConfirm 自动创建 `PopupConfirmFlyout`） |
| `Trigger` | 触发方式：`Click`（默认）、`Hover`、`Focus` |
| `Placement` | 弹出方位（默认 `Top`） |
| `IsShowArrow` | 是否显示箭头指向 |
| `IsPointAtCenter` | 箭头是否指向触发元素中心 |
| `MarginToAnchor` | 弹出层与触发元素之间的间距 |
| `MouseEnterDelay` / `MouseLeaveDelay` | 悬浮触发模式下的延迟（毫秒） |
| `IsMotionEnabled` | 是否启用弹出动画 |
| `MotionDuration` | 动画持续时间 |

### AtomUI 的扩展设计

`PopupConfirm` 在 `FlyoutHost` 基础上增加了确认对话框的语义能力：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **标题 + 内容** | `Title` / `ConfirmContent` / `ConfirmContentTemplate` 属性 | 传达确认信息，ConfirmContent 支持任意内容模板 |
| **操作按钮** | `OkText` / `CancelText` / `OkButtonType` / `IsShowCancelButton` | 可定制按钮文本和行为，默认文本通过本地化资源提供 |
| **确认状态** | `ConfirmStatus` 枚举（Info / Warning / Error） | 控制图标颜色，传达操作风险等级 |
| **自定义图标** | `Icon` 属性 | 允许替换默认的 `ExclamationCircleFilled` 图标 |
| **事件反馈** | `Confirmed` / `Cancelled` / `PopupClick` 事件 | 分别处理确认、取消及统一点击逻辑 |
| **自动关闭** | 点击确认/取消后自动隐藏弹出层 | 符合用户预期的交互流程 |
| **Design Token** | `PopupConfirmToken` + `RegisterTokenResourceScope` | 间距、尺寸等值从 Token 派生 |

---

## 架构设计

### 三组件协作模式

PopupConfirm 采用三个类协作的模式实现弹出确认功能：

```
PopupConfirm (宿主)
  │── 包裹触发元素（Content）
  │── 自动创建 PopupConfirmFlyout
  │
  └── PopupConfirmFlyout (弹出层)
        │── 继承 Flyout，创建 FlyoutPresenter
        │── 创建 PopupConfirmContainer 作为弹出内容
        │── 通过 RelayBind 中继所有属性
        │
        └── PopupConfirmContainer (内容容器)
              │── 承载实际的 UI 布局（图标 + 标题 + 内容 + 按钮）
              │── 处理按钮点击，通过弱引用回调 PopupConfirm 的事件
              └── 管理默认图标和伪类更新
```

**设计理由：**
- **PopupConfirm**：作为公共 API 面，持有所有用户可设置的属性和事件
- **PopupConfirmFlyout**：桥接 PopupConfirm 和 Avalonia 的 Flyout 系统，负责创建 Presenter 和属性中继
- **PopupConfirmContainer**：实际的 UI 模板承载者，通过弱引用避免内存泄漏

---

## 功能详解

### 确认状态（ConfirmStatus）

通过 `ConfirmStatus` 属性控制图标颜色，传达不同的风险等级：

| 状态 | 图标颜色 | 适用场景 |
|---|---|---|
| `Warning`（默认） | `ColorWarning`（橙色） | 一般性确认操作 |
| `Info` | `ColorPrimary`（蓝色） | 信息性确认 |
| `Error` | `ColorError`（红色） | 高风险/破坏性操作 |

### 默认图标

当未设置 `Icon` 属性时，`PopupConfirmContainer` 会自动使用 `ExclamationCircleFilled`（实心感叹号圆圈）作为默认图标。图标大小由 `SharedToken.IconSizeLG` 控制。

### 按钮本地化

`OkText` 和 `CancelText` 的默认值通过 `CommonLangResource` 本地化资源提供：
- 英文：`Ok` / `Cancel`
- 中文：`确定` / `取消`

可通过属性显式设置覆盖默认值。

### 空内容伪类

当 `ConfirmContent` 为 `null` 时，`PopupConfirmContainer` 会激活 `:empty-content` 伪类。此时按钮区域的上边距被移除（`Margin = 0`），使仅有标题的气泡更加紧凑。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 标题 `title` | ✅ ReactNode | ✅ `Title` 字符串属性 | ✅ 对齐 |
| 描述 `description` | ✅ ReactNode | ✅ `ConfirmContent` + `ConfirmContentTemplate` | ✅ 对齐（支持模板化） |
| 确认按钮文本 `okText` | ✅ 字符串 | ✅ `OkText` 属性 | ✅ 完全对齐 |
| 取消按钮文本 `cancelText` | ✅ 字符串 | ✅ `CancelText` 属性 | ✅ 完全对齐 |
| 确认按钮类型 `okType` | ✅ `primary` | ✅ `OkButtonType` 属性（ButtonType 枚举） | ✅ 完全对齐 |
| 隐藏取消按钮 | ✅ `showCancel` | ✅ `IsShowCancelButton` 属性 | ✅ 完全对齐 |
| 自定义图标 `icon` | ✅ ReactNode | ✅ `Icon` 属性（PathIcon） | ✅ 对齐 |
| 12 种弹出位置 `placement` | ✅ | ✅ `Placement` 属性 | ✅ 完全对齐 |
| 箭头显示 | ✅ `arrow` | ✅ `IsShowArrow` 属性 | ✅ 完全对齐 |
| 确认事件 `onConfirm` | ✅ 回调 | ✅ `Confirmed` 路由事件 | ✅ 对齐 |
| 取消事件 `onCancel` | ✅ 回调 | ✅ `Cancelled` 路由事件 | ✅ 对齐 |
| 弹出回调 `onPopupClick` | ✅ 回调 | ✅ `PopupClick` 路由事件 | ✅ 对齐 |
| 条件触发 `condition` | ✅ | ❌ 暂未支持 | ⚠️ 可通过事件逻辑实现 |
| 禁用 `disabled` | ✅ | ❌ 通过 `IsEnabled` 控制 | ⚠️ 语义略有差异 |
| `onOpenChange` | ✅ 回调 | ❌ 暂未暴露 | ⚠️ 待支持 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── AtomUI.Desktop.Controls.FlyoutHost (IMotionAwareControl)
              └── AtomUI.Desktop.Controls.PopupConfirm
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ContentControl` | 包裹触发元素（Button 等），通过 `ContentPresenter` 模板化呈现 |
| `FlyoutHost` | 弹出层管理（显示/隐藏/触发方式）、方位控制、箭头、动画、属性中继到 Flyout |
| `PopupConfirm` | 确认对话框语义（标题/内容/按钮/状态/图标）、确认/取消事件、自动关闭 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Desktop.Controls/Flyouts/FlyoutHost.cs` | FlyoutHost 弹出层宿主基类 |
| 基类 Token | `src/AtomUI.Desktop.Controls/Flyouts/FlyoutHostToken.cs` | FlyoutHost 组件 Token |
| 控件类 | `src/AtomUI.Desktop.Controls/PopupConfirm/PopupConfirm.cs` | PopupConfirm 主控件 |
| 内部容器 | `src/AtomUI.Desktop.Controls/PopupConfirm/PopupConfirmContainer.cs` | 弹出内容容器（internal） |
| 内部弹出层 | `src/AtomUI.Desktop.Controls/PopupConfirm/PopupConfirmFlyout.cs` | 弹出层桥接（internal） |
| 伪类常量 | `src/AtomUI.Desktop.Controls/PopupConfirm/PopupConfirmPseudoClass.cs` | 伪类定义 |
| Token | `src/AtomUI.Desktop.Controls/PopupConfirm/PopupConfirmToken.cs` | 组件 Token |
| 主题 | `src/AtomUI.Desktop.Controls/PopupConfirm/Themes/PopupConfirmTheme.axaml` | PopupConfirm 主题 |
| 主题 | `src/AtomUI.Desktop.Controls/PopupConfirm/Themes/PopupConfirmContainerTheme.axaml` | 容器主题 |
| 主题注册 | `src/AtomUI.Desktop.Controls/PopupConfirm/Themes/PopupConfirmThemes.axaml` | 主题资源汇总 |
| 模板常量 | `src/AtomUI.Desktop.Controls/PopupConfirm/Themes/PopupConfirmContainerThemeConstants.cs` | 模板部件名称 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/PopupConfirmShowCase.axaml` | 使用范例 |

---

## 模板结构

### PopupConfirm 外层模板

PopupConfirm 的外层模板非常简洁，仅包含一个 `ContentPresenter` 用于呈现触发元素：

```
ContentPresenter (PART_ContentPresenter)
  └── [用户设置的 Content，如 Button]
```

弹出层（PopupConfirmFlyout）在 `OnAttachedToVisualTree` 时自动创建，挂载到 FlyoutHost 的 Flyout 属性上。

### PopupConfirmContainer 内容模板

弹出的气泡卡片内部使用 `DockPanel` + `StackPanel` 布局：

```
DockPanel (PART_MainLayout)
├── StackPanel (PART_ButtonLayout)           ← DockPanel.Dock="Bottom"，右对齐
│   ├── Button (PART_CancelButton)           ← 取消按钮（SizeType=Small）
│   └── Button (PART_OkButton)               ← 确认按钮（SizeType=Small, ButtonType=绑定）
└── DockPanel                                ← 填充剩余空间
    ├── IconPresenter (PART_IconPresenter)    ← DockPanel.Dock="Left"，状态图标
    └── StackPanel
        ├── TextBlock (PART_Title)           ← 标题（粗体，ColorTextHeading）
        └── ContentPresenter (PART_Content)  ← 确认内容（可选）
```

### 模板部件常量

**PopupConfirmContainerThemeConstants：**

| 常量名 | 值 | 说明 |
|---|---|---|
| `MainLayoutPart` | `"PART_MainLayout"` | 主布局面板 |
| `ButtonLayoutPart` | `"PART_ButtonLayout"` | 按钮区域容器 |
| `OkButtonPart` | `"PART_OkButton"` | 确认按钮 |
| `CancelButtonPart` | `"PART_CancelButton"` | 取消按钮 |
| `TitlePart` | `"PART_Title"` | 标题文本 |
| `ContentPart` | `"PART_Content"` | 内容展示器 |
| `IconPresenterPart` | `"PART_IconPresenter"` | 图标展示器 |

**PopupConfirmThemeConstants：**

| 常量名 | 值 | 说明 |
|---|---|---|
| `ContentPresenterPart` | `"PART_ContentPresenter"` | 外层内容展示器 |
