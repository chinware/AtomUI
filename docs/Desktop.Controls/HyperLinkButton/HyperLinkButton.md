# HyperLinkButton 超链接按钮

## 概述

超链接按钮（HyperLinkButton）是一个外观类似超链接的按钮控件，支持点击后自动打开浏览器导航到指定 URI。它继承了 Button 的所有能力（尺寸、图标、加载状态、命令绑定），但视觉上呈现为链接文本的样式，用于需要跳转或导航的场景。

HyperLinkButton 对应 Ant Design `Button` 的 `type="link"` 变体，但在 AtomUI 中被提取为一个独立的控件类，提供了更丰富的链接交互能力（如 `NavigateUri` 自动导航、`IsVisited` 已访问状态跟踪）。

---

## 设计原理

### Ant Design 的链接按钮设计

Ant Design 的按钮体系中，`Button type="link"` 是视觉权重最低的按钮变体——无背景、无边框，仅保留链接色文字。它的设计意图是**暗示导航而非操作**，适合放在内容流中作为跳转入口。在 Web 环境中，Ant Design 通过 `href` 属性支持真实的超链接导航。

### AtomUI 的扩展设计

AtomUI 将链接按钮提取为独立的 `HyperLinkButton` 控件，主要基于以下考量：

1. **桌面端导航需求**：桌面应用同样有「打开浏览器」的导航需求，需要 `NavigateUri` + `Launcher.LaunchUriAsync` 集成
2. **已访问状态**：Web 浏览器天然支持 `:visited` 伪类，桌面端需要显式实现 `IsVisited` 属性和对应的伪类
3. **关注点分离**：链接按钮的颜色体系（`ColorLink` 系列）和交互模式（导航而非操作）与普通 Button 有本质区别，独立为专用控件更清晰

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **真实导航** | `NavigateUri` 属性 + `Launcher.LaunchUriAsync` | 桌面端的超链接跳转，对齐 Ant Design 的 `href` |
| **已访问状态** | `IsVisited` 属性 + `:visited` 伪类 | 跟踪链接是否已被点击过，可进行视觉区分 |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 与全局尺寸体系一致 |
| **危险样式** | `IsDanger` 属性 + `:danger` 伪类 | 红色链接，适用于危险操作（删除、移除等） |
| **幽灵模式** | `IsGhost` 属性 | 背景透明，适用于有色背景场景 |
| **加载状态** | `IsLoading` 属性 + `LoadingOutlined` 图标 | 旋转图标 + 降低不透明度，用于异步操作等待 |
| **图标支持** | `Icon` 属性（`PathIcon` 类型） | 在链接文本前显示图标 |
| **过渡动画** | `IMotionAwareControl` + `Transitions` | 前景色、背景色平滑过渡 |
| **Design Token** | 复用 `ButtonToken` 作用域 | 间距和字体 Token 与 Button 共享，颜色使用链接色系 |

### 导航机制

当设置 `NavigateUri` 属性后，点击按钮触发以下流程：

1. 调用 `OnClick()` 基类方法（触发 `Click` 事件和 `Command` 执行）
2. 检查 `NavigateUri` 是否不为 `null`
3. 通过 `TopLevel.GetTopLevel(this).Launcher.LaunchUriAsync(uri)` 异步打开系统默认浏览器
4. 导航成功后自动将 `IsVisited` 设为 `true`，激活 `:visited` 伪类

> **注意**：即使设置了 `NavigateUri`，`Click` 事件和 `Command` 仍然会正常触发。`NavigateUri` 是额外的导航行为，不会替代 Button 的基本点击机制。

---

## 功能详解

### 链接导航（NavigateUri）

通过 `NavigateUri` 属性设置目标 URI：

- 支持任意 URI 格式（`https://`、`mailto:`、`file://` 等）
- 点击后异步调用系统 Launcher 打开默认程序
- 导航成功后 `IsVisited` 自动设为 `true`
- 导航失败（如无效 URI）时 `IsVisited` 保持不变

### 已访问状态（IsVisited）

- 导航成功后自动设置
- 可通过 `:visited` 伪类在样式中进行视觉区分
- 可手动设置 `IsVisited = true/false` 来控制状态
- 可通过数据绑定持久化已访问记录

### 危险样式（IsDanger）

通过 `IsDanger` 属性启用红色链接样式：
- 正常态使用 `ColorError`
- 悬浮态使用 `ColorErrorHover`
- 按下态使用 `ColorErrorActive`
- 图标颜色同步切换

### 加载状态（IsLoading）

当 `IsLoading = true` 时：
1. `:loading` 伪类被激活
2. 原图标隐藏，替换为旋转的 `LoadingOutlined` 图标
3. 整体不透明度降至 `SharedToken.OpacityLoading`

### 过渡动画

HyperLinkButton 在加载时配置以下过渡动画：
- `Background`：背景色渐变过渡
- `Foreground`：前景色（文本色）渐变过渡

通过 `IsMotionEnabled` 属性可控制是否启用过渡动画。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 链接样式 | ✅ `Button type="link"` | ✅ `HyperLinkButton` | ✅ 完全对齐 |
| 三种尺寸 `size` | ✅ `large / medium / small` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 图标 `icon` | ✅ ReactNode | ✅ `Icon` 属性（`PathIcon`） | ✅ 完全对齐 |
| 危险样式 `danger` | ✅ 布尔属性 | ✅ `IsDanger` 属性 | ✅ 完全对齐 |
| 加载中 `loading` | ✅ 布尔属性 | ✅ `IsLoading` 属性 | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ 布尔属性 | ✅ `IsEnabled` 属性 | ✅ 完全对齐 |
| 真实导航 `href` | ✅ `href` + `target` | ✅ `NavigateUri` | ✅ 完全对齐（桌面端通过系统 Launcher） |
| 已访问状态 | — （浏览器原生） | ✅ `IsVisited` + `:visited` 伪类 | ✅ AtomUI 扩展 |
| 幽灵模式 `ghost` | ✅ 布尔属性 | ✅ `IsGhost` 属性 | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Button (ICommandSource, IClickableControl)
              └── AtomUI.Controls.Commons.AbstractHyperLinkButton (ISizeTypeAware, IMotionAwareControl)
                    └── AtomUI.Desktop.Controls.HyperLinkButton
```

### 跨平台架构

HyperLinkButton 遵循 AtomUI 的两层控件模型：

| 层级 | 类 | 职责 |
|---|---|---|
| **基类**（`AtomUI.Controls`） | `AbstractHyperLinkButton` | 设备无关的共享行为：属性定义、导航逻辑、伪类管理、过渡动画配置 |
| **桌面端**（`AtomUI.Desktop.Controls`） | `HyperLinkButton` | 注册 `ButtonToken` 作用域，桌面端特定的主题和样式 |

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `Avalonia.Controls.Button` | 指针交互 → Click 事件、`ICommand` 绑定、`IsPressed` 状态、`HotKey` 快捷键、`Flyout` 弹出 |
| `AbstractHyperLinkButton` | `NavigateUri` + `Launcher` 导航、`IsVisited` 已访问追踪、`SizeType` 尺寸、`IsDanger` / `IsGhost` / `IsLoading` 状态、`Icon` 图标、`IsMotionEnabled` 动画开关、伪类管理 |
| `HyperLinkButton` | `ButtonToken` 作用域注册（复用 Button 的间距/字体 Token） |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持动画开关，控制过渡动画的启用/禁用 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/Buttons/AbstractHyperLinkButton.cs` | 设备无关基类（行为、属性、导航逻辑） |
| 伪类常量 | `src/AtomUI.Controls/Buttons/ButtonPseudoClass.cs` | 共享伪类定义（与 Button 共用） |
| 控件类 | `src/AtomUI.Desktop.Controls/Buttons/HyperLinkButton.cs` | 桌面端具体实现 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Buttons/Themes/HyperLinkButtonTheme.axaml` | ControlTheme AXAML |
| 模板常量 | `src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonThemeConstants.cs` | 模板部件名称常量（与 Button 共用） |
| Gallery 使用 | `controlgallery/AtomUIGallery/ShowCases/Views/General/AboutUsPage.axaml` | 实际使用示例 |

---

## 模板结构

HyperLinkButton 采用简洁的单层 Border + StackPanel 布局：

```
Border#Frame                                ← 主框架（承载 Padding、CornerRadius）
└── StackPanel#PART_MainInfoLayout          ← 内容布局容器（水平排列、居中对齐）
    ├── LoadingOutlined#PART_LoadingIcon     ← 加载旋转图标（默认隐藏，:loading 时显示）
    ├── IconPresenter#PART_ButtonIcon        ← 按钮图标展示器（默认显示，:loading 时隐藏）
    └── ContentPresenter#PART_ContentPresenter  ← 文本内容
```

**与 Button 模板的区别：**
- **无波纹层**：HyperLinkButton 不使用 `WaveSpiritDecorator`（链接按钮不需要点击波纹）
- **无阴影层**：链接按钮无阴影效果
- **无焦点指示器层**：使用更简洁的焦点样式
- **背景始终透明**：正常态和幽灵态背景均为透明

### 模板部件常量

HyperLinkButton 复用 `ButtonThemeConstants` 中的常量：

| 常量名 | 值 | 说明 |
|---|---|---|
| `ButtonThemeConstants.MainInfoLayoutPart` | `"PART_MainInfoLayout"` | 内容布局面板 |
| `ButtonThemeConstants.LoadingIconPart` | `"PART_LoadingIcon"` | 加载旋转图标 |
| `ButtonThemeConstants.ButtonIconPart` | `"PART_ButtonIcon"` | 按钮图标展示器 |
| `ButtonThemeConstants.ContentPresenterPart` | `"PART_ContentPresenter"` | 内容展示器 |
