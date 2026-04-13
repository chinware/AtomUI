# Notification 通知提醒

## 概述

通知提醒框（Notification）在页面角落弹出全局通知，用于展示需要用户关注但不需要强制中断操作的信息。不同于 `Message` 的顶部居中短提示，Notification 更适合展示**较长的通知内容**，包含标题和正文，并支持自定义图标和自动关闭倒计时。

AtomUI 的 Notification 控件完整复刻了 [Ant Design 5.0 Notification](https://ant.design/components/notification-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的通知设计哲学

Ant Design 对通知提醒框的定位是：**「在页面的角落弹出全局通知，用于较复杂的通知内容」**。与 Message 的区别在于：

| 对比维度 | Message 消息 | Notification 通知 |
|---|---|---|
| 弹出位置 | 页面顶部居中 | 页面四角或上下居中 |
| 内容复杂度 | 简短的一行文字 | 标题 + 正文，可包含自定义图标 |
| 关闭方式 | 自动关闭 | 自动关闭 + 手动关闭按钮 |
| 适用场景 | 操作反馈（成功/失败/警告） | 系统级通知、异步任务结果、较长描述信息 |

**四种通知类型**（通过图标和颜色区分语义）：

| 类型 | 设计意图 | 图标 | 颜色 |
|---|---|---|---|
| ℹ️ **Information** | 一般性信息通知 | `InfoCircleFilled` | 主色（`ColorPrimary`） |
| ✅ **Success** | 操作成功反馈 | `CheckCircleFilled` | 成功色（`ColorSuccess`） |
| ⚠️ **Warning** | 警告通知 | `ExclamationCircleFilled` | 警告色（`ColorWarning`） |
| ❌ **Error** | 错误/失败通知 | `CloseCircleFilled` | 错误色（`ColorError`） |

**六种弹出位置**：

| 位置 | 说明 |
|---|---|
| `TopLeft` | 左上角 |
| `TopRight` | 右上角（默认） |
| `TopCenter` | 顶部居中 |
| `BottomLeft` | 左下角 |
| `BottomRight` | 右下角 |
| `BottomCenter` | 底部居中 |

### Avalonia 基础能力

AtomUI 的 Notification 系统不继承自 Avalonia 内置的通知组件，而是**完全自研**实现，直接基于 Avalonia 基础控件构建：

- `NotificationCard` 继承自 `ContentControl`，作为单条通知的可视化载体。
- `WindowNotificationManager` 继承自 `TemplatedControl`，负责通知的生命周期管理、定位和容器布局。

**核心设计思路：**

```
WindowNotificationManager (容器，挂载到 AdornerLayer)
  └── ReversibleStackPanel (PART_Items)
        ├── NotificationCard (单条通知)
        ├── NotificationCard (单条通知)
        └── ...
```

`WindowNotificationManager` 通过 `AdornerLayer` 挂载到窗口的最顶层，确保通知悬浮于所有业务内容之上。内部使用 `ReversibleStackPanel` 管理通知卡片的排列方向。

### AtomUI 的实现设计

| 设计能力 | 实现方式 | 设计动机 |
|---|---|---|
| **四种通知类型** | `NotificationType` 枚举 + 伪类驱动图标/颜色 | 对齐 Ant Design 的通知类型系统 |
| **六种弹出位置** | `NotificationPosition` 枚举 + 伪类驱动布局 | 支持页面四角及上下居中 |
| **自定义图标** | `PathIcon? Icon` 属性 | 允许替换默认类型图标 |
| **自动关闭** | `Expiration` + `DispatcherTimer` 周期检测 | 超时自动关闭通知 |
| **手动关闭** | `IconButton` 关闭按钮 | 右上角关闭按钮 |
| **进度条** | `NotificationProgressBar` + `ShowProgress` | 可视化自动关闭倒计时 |
| **悬停暂停** | `IsPauseOnHover` + 指针事件控制计时器 | 鼠标悬停时暂停自动关闭倒计时 |
| **出入动画** | 方向感知的自定义 Motion 动画 | 根据位置选择合适的滑入/滑出方向 |
| **最大通知数限制** | `MaxItems` + 超限自动关闭旧通知 | 防止通知堆积占满屏幕 |
| **Design Token** | `NotificationToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 通知类型（NotificationType）

通知类型通过 `Notification` 构造函数的 `type` 参数设置，在 `NotificationCard` 中通过 `NotificationType` 属性传递。每种类型自动分配对应的图标和颜色：

| 类型 | 默认图标 | 图标颜色 Token |
|---|---|---|
| `Information` | `InfoCircleFilled` | `ColorPrimary` |
| `Success` | `CheckCircleFilled` | `ColorSuccess` |
| `Warning` | `ExclamationCircleFilled` | `ColorWarning` |
| `Error` | `CloseCircleFilled` | `ColorError` |

如果提供了自定义 `Icon`，则使用自定义图标替换默认图标。

### 弹出位置（NotificationPosition）

弹出位置通过 `WindowNotificationManager.Position` 属性设置。不同位置影响三个方面：

1. **容器布局**：通过 `ReversibleStackPanel` 的 `HorizontalAlignment` 和 `VerticalAlignment` 控制。
2. **卡片边距**：顶部位置使用 `NotificationTopMargin`，底部位置使用 `NotificationBottomMargin`。
3. **出入动画方向**：
   - `TopLeft` / `BottomLeft` → 从左侧滑入/滑出
   - `TopRight` / `BottomRight` → 从右侧滑入/滑出
   - `TopCenter` → 从上方滑入/滑出
   - `BottomCenter` → 从下方滑入/滑出

### 自动关闭与进度条

通知的自动关闭由 `Expiration` 属性控制：
- 默认超时时间为 **5 秒**（`TimeSpan.FromSeconds(5)`）。
- 设置 `Expiration = TimeSpan.Zero` 可使通知**永不自动关闭**，仅能通过关闭按钮手动关闭。
- 当 `ShowProgress = true` 且 `Expiration > TimeSpan.Zero` 时，通知底部会显示一个渐变进度条，随倒计时逐渐缩短。

### 悬停暂停（Pause on Hover）

当 `WindowNotificationManager.IsPauseOnHover = true`（默认开启）时，鼠标悬停在通知卡片上会暂停所有通知的自动关闭计时器，鼠标移出后恢复。这确保用户在阅读通知内容时不会被意外关闭。

### 出入动画

通知的出入动画通过 `MotionActor` 机制实现，使用自定义的 `AbstractMotion` 子类：

| 位置 | 进入动画 | 退出动画 |
|---|---|---|
| `TopLeft` / `BottomLeft` | `NotificationMoveLeftInMotion` | `NotificationMoveLeftOutMotion` |
| `TopRight` / `BottomRight` | `NotificationMoveRightInMotion` | `NotificationMoveRightOutMotion` |
| `TopCenter` | `NotificationMoveUpInMotion` | `NotificationMoveUpOutMotion` |
| `BottomCenter` | `NotificationMoveDownInMotion` | `NotificationMoveDownOutMotion` |

每个动画包含三阶段关键帧（开始 → 中间 → 结束），同时控制 `Opacity`（透明度）和 `TranslateTransform`（位移），实现平滑的滑入/滑出效果。动画时长由 `SharedToken.MotionDurationMid` 控制。

### 最大通知数限制

`WindowNotificationManager.MaxItems` 属性（默认值 5）限制同时可见的通知数量。当新通知到来使数量超限时，最早的通知会被自动关闭（触发退出动画后移除），确保通知区域不会无限膨胀。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 四种类型 `type` | ✅ `success / info / warning / error` | ✅ `NotificationType` 枚举 | ✅ 完全对齐 |
| 弹出位置 `placement` | ✅ `top / bottom / topLeft / topRight / bottomLeft / bottomRight` | ✅ `NotificationPosition` 枚举 | ✅ 完全对齐 |
| 自动关闭 `duration` | ✅ 默认 4.5s，`0` 为不关闭 | ✅ 默认 5s，`TimeSpan.Zero` 为不关闭 | ⚠️ 默认时长略有不同 |
| 自定义图标 `icon` | ✅ ReactNode | ✅ `PathIcon?` 属性 | ✅ 对齐（类型不同，语义一致） |
| 关闭按钮 | ✅ 右上角关闭 | ✅ `IconButton` 关闭按钮 | ✅ 完全对齐 |
| 进度条 `showProgress` | ✅ 5.18.0 新增 | ✅ `ShowProgress` 属性 | ✅ 完全对齐 |
| 悬停暂停 `pauseOnHover` | ✅ 5.18.0 新增 | ✅ `IsPauseOnHover` 属性 | ✅ 完全对齐 |
| 自定义操作按钮 `btn` | ✅ ReactNode | ❌ 暂未支持 | ⚠️ 待支持 |
| 底部区域 `description` | ✅ 单独 description | ✅ 通过 `Content` 实现 | ✅ 语义对齐 |
| 点击回调 `onClick` | ✅ 函数 | ✅ `OnClick` Action | ✅ 完全对齐 |
| 关闭回调 `onClose` | ✅ 函数 | ✅ `OnClose` Action | ✅ 完全对齐 |
| 全局配置 `maxCount` | ✅ 最大显示数 | ✅ `MaxItems` 属性 | ✅ 完全对齐 |
| 堆叠模式 `stack` | ✅ 5.18.0 新增 | ❌ 暂未支持 | ⚠️ 待支持 |
| 编程式 API `api.open()` | ✅ hooks API | ⚠️ 通过 `INotificationManager.Show()` 方法调用 | ✅ 等效实现 |

---

## 继承关系

### WindowNotificationManager

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.WindowNotificationManager
        ├── implements INotificationManager
        ├── implements IMotionAwareControl
        └── implements IDisposable
```

### NotificationCard

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── AtomUI.Desktop.Controls.NotificationCard
              └── implements IMotionAwareControl
```

### Notification（数据模型）

```
AtomUI.Desktop.Controls.Notification
  ├── implements INotification
  └── implements INotifyPropertyChanged
```

**各层级职责划分：**

| 类 | 职责 |
|---|---|
| `INotification` | 通知数据契约：标题、内容、类型、图标、过期时间、回调 |
| `Notification` | `INotification` 的默认实现，支持属性变更通知 |
| `INotificationManager` | 通知管理器契约：`Show()` 方法 |
| `WindowNotificationManager` | 通知管理器实现：挂载到窗口 AdornerLayer、管理通知生命周期、定时关闭、位置控制 |
| `NotificationCard` | 单条通知的可视化控件：图标、标题、内容、关闭按钮、进度条、出入动画 |
| `NotificationProgressBar` | 通知内的进度条控件（internal），绘制倒计时进度指示 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 接口 | `src/AtomUI.Desktop.Controls/Notifications/INotification.cs` | 通知数据契约接口 |
| 接口 | `src/AtomUI.Desktop.Controls/Notifications/INotificationManager.cs` | 通知管理器契约接口 |
| 数据模型 | `src/AtomUI.Desktop.Controls/Notifications/Notification.cs` | `INotification` 默认实现 |
| 控件类 | `src/AtomUI.Desktop.Controls/Notifications/NotificationCard.cs` | 通知卡片控件 |
| 控件类 | `src/AtomUI.Desktop.Controls/Notifications/WindowNotificationManager.cs` | 通知管理器控件 |
| 进度条 | `src/AtomUI.Desktop.Controls/Notifications/NotificationProgressBar.cs` | 内部进度条控件 |
| 枚举 | `src/AtomUI.Desktop.Controls/Notifications/NotificationType.cs` | 通知类型枚举 |
| 枚举 | `src/AtomUI.Desktop.Controls/Notifications/NotificationPosition.cs` | 通知位置枚举 |
| 伪类常量 | `src/AtomUI.Desktop.Controls/Notifications/NotificationPseudoClass.cs` | 位置伪类常量 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Notifications/NotificationToken.cs` | 组件级 Design Token |
| 动画 | `src/AtomUI.Desktop.Controls/Notifications/NotificationMotions.cs` | 8 种方向动画定义 |
| 工具类 | `src/AtomUI.Desktop.Controls/Notifications/Utils/NotificationProgressBarVisibleConverter.cs` | 进度条可见性转换器 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Notifications/Themes/NotificationCardTheme.axaml` | 通知卡片主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Notifications/Themes/WindowNotificationManagerTheme.axaml` | 管理器主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Notifications/Themes/NotificationProgressBarTheme.axaml` | 进度条主题 |
| 主题注册 | `src/AtomUI.Desktop.Controls/Notifications/Themes/NotificationsThemes.axaml` | ResourceDictionary 汇总 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/NotificationShowCase.axaml` | 使用范例 |
| Gallery 代码 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/NotificationShowCase.axaml.cs` | 范例 code-behind |

---

## 模板结构

### NotificationCard 模板

```
LayoutAwareMotionActor (PART_MotionActor)           ← 动画容器，出入动画作用目标
  └── Border#Frame                                   ← 主框架（背景、圆角、阴影）
        └── Grid [Auto,*] × [Auto,*,Auto]           ← 内容网格布局
              ├── IconPresenter#IconPresenter         ← 类型图标（Row=0, Col=0）
              ├── DockPanel#HeaderContainer           ← 标题栏容器（Row=0, Col=1）
              │     ├── IconButton#PART_CloseButton   ← 关闭按钮（DockPanel.Dock=Right）
              │     └── SelectableTextBlock#HeaderTitle ← 标题文本
              ├── ContentPresenter#Content            ← 正文内容（Row=1, Col=1）
              └── NotificationProgressBar#ProgressBar ← 进度条（Row=1, Col=0, ColSpan=2）
```

**布局设计要点：**
- **动画层独立**：`LayoutAwareMotionActor` 包裹整个通知卡片，确保出入动画不受布局约束影响。
- **图标与标题对齐**：Grid 的第一列（Auto）放置图标，第二列（`*`）放置标题和内容，图标与标题行顶部对齐。
- **关闭按钮右对齐**：通过 `DockPanel` 将关闭按钮停靠在标题栏右侧。
- **进度条底部**：进度条位于内容区底部，跨越两列。

### WindowNotificationManager 模板

```
ReversibleStackPanel (PART_Items)    ← 通知容器，支持正序/逆序排列
  ├── NotificationCard               ← 单条通知
  ├── NotificationCard               ← 单条通知
  └── ...
```

- 顶部位置（Top*）使用 `ReverseOrder=True`，使新通知出现在底部（从上往下堆叠）。
- 底部位置（Bottom*）使用正序排列，新通知出现在顶部。
