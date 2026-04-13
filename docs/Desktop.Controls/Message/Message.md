# Message 全局提示

## 概述

全局提示（Message）用于在页面顶部居中展示轻量级的操作反馈信息，属于非阻断性反馈组件。它在操作完成后自动出现在页面顶部，短暂停留后自动消失，不打断用户的操作流程。适用于表单提交成功/失败、异步操作状态提示、系统信息通知等场景。

AtomUI 的 `Message` 控件对标 [Ant Design 5.0 Message](https://ant.design/components/message-cn) 的设计规范，在 .NET / Avalonia 平台上提供一致的全局提示体验。

---

## 设计原理

### Ant Design 的全局提示设计哲学

Ant Design 对全局提示的定位是：**「全局展示操作反馈信息，居顶部居中显示并自动消失，是一种不打断用户操作的轻量级提示方式」**。它与 Notification（通知提醒）的关键区别在于：

| 特性 | Message（全局提示） | Notification（通知提醒） |
|---|---|---|
| **位置** | 页面顶部居中 | 页面四角 |
| **内容量** | 仅文本，轻量单行 | 标题 + 详细描述 + 操作按钮 |
| **交互** | 无交互，自动消失 | 可关闭、可操作 |
| **适用场景** | 操作反馈（成功/失败/警告） | 需要用户关注的复杂通知 |

**五种消息类型**（对应不同操作结果和语义）：

| 类型 | 图标 | 设计意图 | 典型用途 |
|---|---|---|---|
| ℹ️ **Information** | `InfoCircleFilled`（蓝色） | 普通信息提示，中性语义 | 一般性操作提示、说明信息 |
| ✅ **Success** | `CheckCircleFilled`（绿色） | 操作成功确认 | 表单提交成功、保存完成 |
| ⚠️ **Warning** | `ExclamationCircleFilled`（黄色） | 警告提示，需要注意 | 操作有风险但可继续 |
| ❌ **Error** | `CloseCircleFilled`（红色） | 操作失败，需要关注 | 提交失败、网络错误 |
| 🔄 **Loading** | `LoadingOutlined`（蓝色旋转） | 异步操作进行中 | 数据加载、文件上传中 |

### AtomUI 的架构设计

AtomUI 的 Message 系统由三个核心组件协作完成：

```
WindowMessageManager（容器管理器）
  ├── 挂载在 TopLevel 的 AdornerLayer 上
  ├── 管理消息的生命周期（显示、自动关闭、最大数量控制）
  └── MessageCard（单条消息卡片）× N
       ├── 图标（根据 MessageType 自动匹配）
       ├── 文本内容
       └── 入场/出场动画（MoveUpIn / MoveUpOut）
```

**职责分离**：
- `IMessage` / `Message` — 数据模型，承载消息内容、类型、过期时间等信息
- `MessageCard` — 视觉控件，负责单条消息的渲染和动画
- `WindowMessageManager` — 容器管理器，负责消息的创建、排布和生命周期管理

---

## 功能详解

### 消息类型（MessageType）

消息类型通过 `MessageType` 枚举指定，每种类型会自动匹配对应的图标和颜色：

| 类型 | 默认图标 | 图标颜色来源 |
|---|---|---|
| `Information` | `InfoCircleFilled` | `SharedToken.ColorPrimary`（蓝色） |
| `Success` | `CheckCircleFilled` | `SharedToken.ColorSuccess`（绿色） |
| `Warning` | `ExclamationCircleFilled` | `SharedToken.ColorWarning`（黄色） |
| `Error` | `CloseCircleFilled` | `SharedToken.ColorError`（红色） |
| `Loading` | `LoadingOutlined`（旋转） | `SharedToken.ColorPrimary`（蓝色） |

### 自动关闭与过期时间

每条消息都有 `Expiration` 属性控制自动关闭时间：
- 默认值为 **5 秒**
- 设置为 `TimeSpan.Zero` 时消息不会自动关闭，需要手动调用 `Close()`
- 过期后通过 `DispatcherTimer.RunOnce` 触发关闭流程

### 最大显示数量

`WindowMessageManager.MaxItems` 控制同时可见的最大消息数量（默认为 5）。当新消息导致超出上限时，最早显示的消息会被自动关闭。

### 入场/出场动画

MessageCard 使用 `MoveUpIn` / `MoveUpOut` 动画实现消息的入场和出场效果：
- **入场**：从上方滑入 + 透明度从 0 到 1，使用 `CubicEaseOut` 缓动
- **出场**：向上滑出 + 透明度从 1 到 0，使用 `CubicEaseIn` 缓动
- 动画偏移量固定为 100px（`AnimationMaxOffsetY`）
- 动画时长由 `SharedToken.MotionDurationMid` 控制
- 可通过 `IsMotionEnabled = false` 禁用动画

### 关闭回调

通过 `Message` 构造函数的 `onClose` 参数传入关闭回调，当消息关闭（无论自动或手动）时执行。这支持链式消息——在一条消息关闭后立即显示下一条。

### 自定义图标

通过 `Message` 构造函数的 `icon` 参数可以传入自定义 `PathIcon`，覆盖默认的类型图标。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 五种消息类型 | ✅ `info / success / warning / error / loading` | ✅ `MessageType` 枚举 | ✅ 完全对齐 |
| 顶部居中显示 | ✅ | ✅ `HorizontalAlignment="Center"` + `VerticalAlignment="Top"` | ✅ 完全对齐 |
| 自动关闭 | ✅ 默认 3 秒 | ✅ 默认 5 秒 | ⚠️ 默认时长不同 |
| 自定义图标 | ✅ `icon` 属性 | ✅ `Icon` 参数 | ✅ 对齐 |
| 关闭回调 | ✅ `onClose` | ✅ `OnClose` 回调 | ✅ 对齐 |
| Loading 类型 | ✅ | ✅ 旋转 `LoadingOutlined` 图标 | ✅ 完全对齐 |
| 自定义时长 | ✅ `duration` 参数 | ✅ `Expiration` 参数 | ✅ 对齐 |
| 最大显示数量 | ✅ `maxCount` | ✅ `MaxItems` 属性 | ✅ 对齐 |
| Promise 接口 | ✅ `message.open().then(...)` | ❌ 使用回调模式 | ⚠️ C# 无 Promise，使用 `OnClose` 回调 |
| 全局配置 | ✅ `message.config()` | ❌ 通过 `WindowMessageManager` 实例配置 | ⚠️ 实例级配置 vs 全局配置 |
| 更新消息内容 | ✅ 通过 `key` 更新 | ✅ `Message.Content` 支持 `INotifyPropertyChanged` | ⚠️ 机制不同但功能可实现 |

---

## 继承关系

### MessageCard

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.MessageCard
        └── implements IMotionAwareControl
```

### WindowMessageManager

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.WindowMessageManager
        ├── implements IMessageManager
        ├── implements IMotionAwareControl
        └── implements IDisposable
```

### Message（数据模型）

```
AtomUI.Desktop.Controls.Message
  ├── implements IMessage
  └── implements INotifyPropertyChanged
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础设施、`OnApplyTemplate`、`CornerRadius` 等 |
| `MessageCard` | 单条消息的视觉呈现：图标自动匹配、伪类管理、入场/出场动画 |
| `WindowMessageManager` | 消息容器管理：挂载到 TopLevel、消息排布、最大数量控制、自动关闭计时器 |
| `Message` | 消息数据模型：内容、类型、过期时间、关闭回调 |

**实现的接口：**

| 接口 | 定义位置 | 控件 | 作用 |
|---|---|---|---|
| `IMessage` | `AtomUI.Desktop.Controls` | 数据模型 | 定义消息数据契约（Content、Type、Expiration、OnClose、Icon） |
| `IMessageManager` | `AtomUI.Desktop.Controls` | WindowMessageManager | 定义消息管理器契约（`Show` 方法） |
| `IMotionAwareControl` | `AtomUI.Controls` | MessageCard / WindowMessageManager | 支持动画启用/禁用控制 |
| `INotifyPropertyChanged` | `System.ComponentModel` | Message | 支持属性变更通知（Content、Icon 可动态更新） |
| `IDisposable` | `System` | WindowMessageManager | 支持资源释放（从 AdornerLayer 卸载、清理事件订阅） |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 数据接口 | `src/AtomUI.Desktop.Controls/Message/IMessage.cs` | IMessage 消息数据契约 |
| 管理器接口 | `src/AtomUI.Desktop.Controls/Message/IMessageManager.cs` | IMessageManager 管理器契约 |
| 数据模型 | `src/AtomUI.Desktop.Controls/Message/Message.cs` | Message 消息数据实现 |
| 消息卡片 | `src/AtomUI.Desktop.Controls/Message/MessageCard.cs` | MessageCard 视觉控件 |
| 消息类型枚举 | `src/AtomUI.Desktop.Controls/Message/MessageType.cs` | MessageType 枚举 |
| 伪类常量 | `src/AtomUI.Desktop.Controls/Message/MessageCardPseudoClass.cs` | 伪类常量定义 |
| 容器管理器 | `src/AtomUI.Desktop.Controls/Message/WindowMessageManager.cs` | WindowMessageManager 容器 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Message/MessageToken.cs` | 组件级 Design Token |
| 消息卡片主题 | `src/AtomUI.Desktop.Controls/Message/Themes/MessageCardTheme.axaml` | MessageCard ControlTheme |
| 容器主题 | `src/AtomUI.Desktop.Controls/Message/Themes/WindowMessageManagerTheme.axaml` | WindowMessageManager ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/Message/Themes/MessageThemes.axaml` | 主题合并注册 |
| 模板常量 | `src/AtomUI.Desktop.Controls/Message/Themes/MessageThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/MessageShowCase.axaml` | 使用范例 |

---

## 模板结构

### MessageCard 模板

MessageCard 使用 `MotionActor` 包裹以支持入场/出场动画：

```
MotionActor (PART_MotionActor)                   ← 动画容器
└── Border (PART_Frame)                          ← 主框架（背景、圆角、阴影）
    └── DockPanel (PART_HeaderContainer)         ← 内容布局容器
        ├── IconPresenter (PART_IconContent)      ← 消息图标（DockPanel.Dock="Left"）
        └── SelectableTextBlock (PART_Message)   ← 消息文本（可选中复制）
```

**设计理由：**
- **MotionActor 包裹**：使整个消息卡片可以参与 Move 动画，实现滑入/滑出效果
- **SelectableTextBlock**：使用可选择文本控件，用户可以选中并复制消息内容
- **DockPanel 布局**：图标靠左固定，文本填满剩余空间

### WindowMessageManager 模板

```
ReversibleStackPanel (PART_Items)                 ← 消息堆叠容器
  ├── MessageCard                                ← 消息卡片 1
  ├── MessageCard                                ← 消息卡片 2
  └── ...
```

`ReversibleStackPanel` 支持 `ReverseOrder`，当 Position 为 `TopCenter` 时反转堆叠顺序，使最新消息出现在最上方。

### 模板部件常量

| 常量名 | 值 | 所属 | 说明 |
|---|---|---|---|
| `MessageCardThemeConstants.FramePart` | `"PART_Frame"` | MessageCard | 主框架边框 |
| `MessageCardThemeConstants.IconContentPart` | `"PART_IconContent"` | MessageCard | 图标展示器 |
| `MessageCardThemeConstants.HeaderContainerPart` | `"PART_HeaderContainer"` | MessageCard | 内容布局容器 |
| `MessageCardThemeConstants.MessagePart` | `"PART_Message"` | MessageCard | 消息文本 |
| `WindowMessageManagerThemeConstants.ItemsPart` | `"PART_Items"` | WindowMessageManager | 消息列表面板 |
