# Button 按钮

## 概述

按钮（Button）是图形界面中最基础、最常见的交互控件，用于触发一个即时操作。用户通过点击按钮来启动业务逻辑，如提交表单、打开对话框、执行删除等。按钮是用户意图到应用响应之间的桥梁——它既承载视觉提示（告诉用户「这里可以点击」），又承载交互反馈（告诉用户「你的操作已被接受」）。

AtomUI 的 `Button` 控件完整复刻了 [Ant Design 5.0 Button](https://ant.design/components/button-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的按钮设计哲学

Ant Design 对按钮的定位是：**「标记一个（或封装一组）操作命令，响应用户点击行为，触发相应的业务逻辑」**。为了帮助用户在不同场景中快速识别操作的优先级和性质，Ant Design 建立了一套完整的按钮分类体系：

**五种按钮类型**（按视觉权重从高到低排列）：

| 类型 | 设计意图 | 典型用途 |
|---|---|---|
| 🔵 **主按钮（Primary）** | 视觉权重最高，实心填充主色调。一个操作区域**只应有一个**主按钮，引导用户完成核心操作 | 「提交」「确认」「保存」 |
| ⚪️ **默认按钮（Default）** | 中等视觉权重，白色背景 + 边框。用于同一区域中**无主次之分**的多个操作 | 「取消」「重置」「上一步」 |
| 😶 **虚线按钮（Dashed）** | 虚线边框传达「未完成 / 可添加」语义，暗示用户此处可以补充内容 | 「+ 添加条目」「上传文件」 |
| 🔤 **文本按钮（Text）** | 最低视觉权重，无边框无背景，仅保留文字。用于最次级操作，减少视觉干扰 | 「编辑」「更多」内联操作 |
| 🔗 **链接按钮（Link）** | 外观类似超链接，暗示"导航"而非"操作"。适合跳转类交互 | 「查看详情」「忘记密码？」 |

**四种状态修饰**（可与上述五种类型自由组合）：

| 状态 | 设计意图 |
|---|---|
| ⚠️ **危险（Danger）** | 使用红色系警示用户该操作具有破坏性（删除、移动、修改权限），通常配合二次确认 |
| 👻 **幽灵（Ghost）** | 背景透明，文字/边框使用浅色或主题色。专为复杂背景场景设计（首页横幅、产品展示页） |
| 🚫 **禁用（Disabled）** | 降低不透明度 + 灰色调，告诉用户该操作当前不可用，通常需配合提示文案说明原因 |
| 🔃 **加载中（Loading）** | 显示旋转图标 + 降低不透明度，用于异步操作的等待反馈，同时防止用户重复提交 |

### Avalonia Button 基础能力

AtomUI 的 `Button` 继承自 Avalonia 框架的 `Avalonia.Controls.Button`。理解 Avalonia Button 的基础能力有助于理解 AtomUI 在其之上做了哪些扩展。

**Avalonia Button 的核心职责：**

Avalonia 的 `Button` 是一个标准的 `ContentControl`（内容控件），它实现了 `ICommandSource` 和 `IClickableControl` 接口。其核心行为是：**响应指针操作，在指针按下时提供视觉按压反馈（`:pressed` 伪类），在指针释放时触发 `Click` 事件**。它的继承链为：

```
Control → TemplatedControl → ContentControl → Button
```

作为 `ContentControl`，Button 能容纳任意内容（文本、图标、甚至复杂布局），并通过 `ContentPresenter` 在模板中呈现。Button 还提供了完整的命令绑定（MVVM `Command` / `CommandParameter`）、键盘快捷键（`HotKey`）、`IsDefault` / `IsCancel` 键盘激活、以及 `Flyout` 弹出等基础设施。

**Avalonia Button 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Content` | 按钮内容，可以是文本字符串，也可以是任意控件 |
| `Command` / `CommandParameter` | MVVM 命令绑定，按钮点击时调用 `ICommand.Execute`；当 `CanExecute` 返回 `false` 时自动禁用按钮 |
| `ClickMode` | 控制 `Click` 事件的触发时机：`Release`（指针释放时，默认）或 `Press`（指针按下时） |
| `HotKey` | 键盘快捷键（`KeyGesture`），注册后按下对应组合键等同于点击按钮 |
| `IsDefault` | 设为 `true` 时，用户按 **Enter** 键等同于点击该按钮（常用于对话框「确认」按钮） |
| `IsCancel` | 设为 `true` 时，用户按 **Escape** 键等同于点击该按钮（常用于对话框「取消」按钮） |
| `IsPressed` | 只读属性，指示按钮当前是否处于按压状态 |
| `Flyout` | 附加弹出层，点击按钮时自动展开 `Flyout` / `MenuFlyout` |

**Avalonia Button 提供的基础伪类：**

| 伪类 | 说明 |
|---|---|
| `:pointerover` | 鼠标悬浮在按钮上 |
| `:pressed` | 按钮被按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 按钮获得焦点 |
| `:focus-visible` | 通过键盘（Tab）获得焦点 |
| `:flyout-open` | 附加的 Flyout 处于打开状态 |

> **重要提示**：应始终使用 `Click` 事件而非 `PointerPressed` 来处理按钮点击。`Click` 是 Button 专属的高级语义事件；而 `PointerPressed` 是底层输入事件，Button 内部会将其标记为 `IsHandled = true`，外部通常无法接收到。

### AtomUI 的扩展设计

AtomUI `Button` 在 Avalonia Button 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **五种按钮类型** | `ButtonType` 枚举 + 伪类驱动样式 | 对齐 Ant Design 的 `type` 属性，支持 Primary / Default / Dashed / Text / Link |
| **三种按钮形状** | `ButtonShape` 枚举 + `MeasureOverride` 动态计算 | 对齐 Ant Design 的 `shape`，在布局阶段自动调整尺寸和圆角 |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **危险/幽灵状态** | `IsDanger` / `IsGhost` 属性 + 主题样式变体 | 通过 Token 系统自动切换颜色方案 |
| **加载状态** | `IsLoading` 属性 + 模板内 `LoadingOutlined` 图标 | 旋转加载图标 + 降低不透明度，防止重复操作 |
| **图标支持** | `Icon` 属性（`PathIcon` 类型）+ `IconPresenter` | 内置图标栏位，无需手动组装 StackPanel + Icon |
| **点击波纹** | `IWaveSpiritAwareControl` + `WaveSpiritDecorator` | 复刻 Ant Design 的 Wave 点击涟漪效果 |
| **过渡动画** | `IsMotionEnabled` + `Transitions` 动态配置 | 背景色、前景色、边框色平滑过渡 |
| **紧凑空间** | `ICompactSpaceAware` 接口 | 在 `Space.Compact` 容器中自动调整圆角和边框 |
| **表单集成** | `IFormItemAware` 接口 | 可参与 FormItem 验证流程 |
| **Design Token** | `ButtonToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |

---

## 功能详解

### 按钮类型（ButtonType）

按钮类型通过 `ButtonType` 属性设置，每种类型对应不同的伪类（`:primary`、`:default`、`:dashed`、`:text`、`:link`），ControlTheme 通过这些伪类应用不同的视觉样式。

不同类型的模板差异：
- **Default / Primary / Link / Text**：使用标准 `Border` 作为主框架。
- **Dashed**：使用自定义 `DashedBorder` 控件绘制虚线边框，通过 `StrokeDashArray = [4, 2]` 实现虚线效果。

不同类型的阴影差异：
- **Default / Dashed**：有 `DefaultShadow` 底部微阴影，增强立体感。
- **Primary**：有 `PrimaryShadow` 底部微阴影。
- **Text / Link**：无阴影，保持轻量感。

不同类型的波纹差异：
- **Primary / Default / Dashed**：按下释放时触发 WaveSpirit 波纹动画。
- **Text / Link**：不触发波纹（这两种类型视觉权重低，波纹会造成过度干扰）。

### 按钮形状（ButtonShape）

形状通过 `MeasureOverride` 在布局阶段动态计算：

| 形状 | 布局行为 | 圆角计算 |
|---|---|---|
| `Default` | 宽度 = `max(内容宽度, 高度)`，保证最小正方形 | 由 Token 中 `BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG` 控制 |
| `Circle` | 强制 `宽度 = 高度`，呈正圆形 | `CornerRadius = 高度`（完全圆角） |
| `Round` | 宽度 = `max(内容宽度, 高度 × 1.5)`，呈胶囊形 | `CornerRadius = 高度`（两端完全圆角） |

形状还影响波纹类型：`Default` → `RoundRectWave`，`Round` → `PillWave`，`Circle` → `CircleWave`。

### 加载状态（IsLoading）

当 `IsLoading = true` 时：
1. `:loading` 伪类被激活
2. 按钮图标（`PART_ButtonIcon`）隐藏，替换为旋转的 `LoadingOutlined` 图标
3. 整体不透明度降至 `SharedToken.OpacityLoading`
4. 波纹效果被禁止（`IsPressed` 变化时检查 `IsLoading`）

### 过渡动画（Transitions）

动画在 `OnLoaded` 时配置，`OnUnloaded` 时清除（避免不可见控件消耗资源）。不同 `ButtonType` 配置不同的过渡属性组合：

| ButtonType | 过渡属性 |
|---|---|
| Primary（非 Ghost） | `Background` |
| Primary（Ghost） | `Background` + `BorderBrush` + `Foreground` |
| Default / Dashed | `Background` + `BorderBrush` + `Foreground` |
| Link | `Background` + `Foreground` |
| Text | `Background` |

### 紧凑空间（Compact Space）

当 Button 放置在 `Space.Compact` 容器中时：
- 容器通知每个按钮其 `CompactSpaceItemPosition`（First / Middle / Last / OnlyOne）和 `CompactSpaceOrientation`（Horizontal / Vertical）
- Button 根据位置自动裁剪圆角（例如水平排列的中间按钮圆角为 0）
- Primary 类型按钮始终处于活跃 ZIndex 层，确保边框不被相邻按钮遮挡

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 五种类型 `type` | ✅ `primary / default / dashed / text / link` | ✅ `ButtonType` 枚举 | ✅ 完全对齐 |
| 三种形状 `shape` | ✅ `default / circle / round` | ✅ `ButtonShape` 枚举 | ✅ 完全对齐 |
| 三种尺寸 `size` | ✅ `large / medium / small` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 危险 `danger` | ✅ 布尔属性 | ✅ `IsDanger` 属性 | ✅ 完全对齐 |
| 幽灵 `ghost` | ✅ 布尔属性 | ✅ `IsGhost` 属性 | ✅ 完全对齐 |
| 加载中 `loading` | ✅ 布尔/对象 | ✅ `IsLoading` 布尔 | ⚠️ 不支持延迟加载（`{ delay }` 语法） |
| 图标 `icon` | ✅ ReactNode | ✅ `PathIcon` 属性 | ✅ 对齐（类型不同，语义一致） |
| Block 模式 | ✅ `block` 属性 | ✅ `HorizontalAlignment="Stretch"` | ✅ 通过 Avalonia 原生布局实现 |
| 点击波纹 | ✅ Wave 组件 | ✅ `WaveSpiritDecorator` | ✅ 完全对齐 |
| 颜色+变体 `color` / `variant` | ✅ 5.21.0 新增 | ❌ 暂未支持 | ⚠️ 通过 `ButtonType` 覆盖主要场景 |
| 图标位置 `iconPlacement` | ✅ `start / end` | ❌ 暂仅支持左侧 | ⚠️ 待支持 |
| `href` / `target` | ✅ 链接跳转 | ❌ 不适用 | — 桌面端无需 HTML 超链接语义 |
| `htmlType` | ✅ 原生表单提交 | ❌ 不适用 | — 非 Web 平台 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Button (ICommandSource, IClickableControl)
              └── AtomUI.Desktop.Controls.Button
                    ├── implements ISizeTypeAware
                    ├── implements IWaveSpiritAwareControl
                    ├── implements ICompactSpaceAware
                    └── implements IFormItemAware
```

`Button` 通过 `using AvaloniaButton = Avalonia.Controls.Button;` 别名引用 Avalonia 原生 Button，避免类名冲突。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ContentControl` | 任意内容容纳、`Content` / `ContentTemplate`、`ContentPresenter` 模板化呈现 |
| `Avalonia.Controls.Button` | 指针交互 → Click 事件、`ICommand` 绑定、`IsPressed` 状态、`HotKey` 快捷键、`Flyout` 弹出、`IsDefault` / `IsCancel` 键盘激活、无障碍支持 |
| `AtomUI.Desktop.Controls.Button` | Ant Design 视觉体系（五种类型/三种形状/三种尺寸）、Design Token 集成、WaveSpirit 波纹、加载状态、危险/幽灵样式、紧凑空间适配、表单集成 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IWaveSpiritAwareControl` | `AtomUI.Controls.Shared` | 支持点击涟漪（Wave）动画效果 |
| `ICompactSpaceAware` | `AtomUI.Controls.Shared` | 在 `Space.Compact` 中使用时自动调整圆角和边框 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Buttons/Button.cs` | 桌面端 Button 具体实现 |
| 伪类常量 | `src/AtomUI.Controls/Buttons/ButtonPseudoClass.cs` | 共享伪类定义（跨平台） |
| Token 定义 | `src/AtomUI.Desktop.Controls/Buttons/ButtonToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonTheme.axaml` | ControlTheme AXAML |
| 主题代码 | `src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonTheme.cs` | 主题 code-behind |
| 模板常量 | `src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/General/ButtonShowCase.axaml` | 使用范例 |

---

## 模板结构

Button 的 ControlTemplate 采用分层 Panel 布局，每一层各司其职：

```
Panel
├── WaveSpiritDecorator (PART_WaveSpirit)     ← 点击波纹效果层（最底层，不影响内容）
├── Border#ShadowsFrame                       ← 阴影层（独立于主框架，避免 BoxShadow 被裁剪）
├── Border#Frame (或 DashedBorder#Frame)      ← 主框架（承载背景、边框和全部内容）
│   └── DockPanel (PART_RootLayout)           ← 内容布局容器
│       ├── LoadingOutlined (PART_LoadingIcon) ← 加载旋转图标（DockPanel.Dock="Left"）
│       ├── IconPresenter (PART_ButtonIcon)    ← 按钮图标展示器（DockPanel.Dock="Left"）
│       └── ContentPresenter (PART_ContentPresenter) ← 文本/自定义内容（填充剩余空间）
└── Border#FocusVisual                        ← 焦点指示器（最顶层，仅键盘焦点时可见，ZIndex=1）
```

**分层设计理由：**
- **波纹层独立**：`WaveSpiritDecorator` 位于最底层，波纹动画在按钮边界外扩散时不会被主框架裁剪。
- **阴影层独立**：`BoxShadow` 放在单独的 `Border#ShadowsFrame` 上，避免阴影与虚线边框（Dashed 类型）冲突。
- **焦点层独立**：`Border#FocusVisual` 使用 `ZIndex=1` 和负 `Margin` 绘制在按钮外围，仅在 `:focus-visible` 时显示。

> 注意：当 `ButtonType=Dashed` 时，主框架使用 `DashedBorder` 代替标准 `Border`，通过 `StrokeDashArray = [4, 2]` 绘制虚线边框效果。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `ButtonThemeConstants.WaveSpiritPart` | `"PART_WaveSpirit"` | 波纹动画装饰器 |
| `ButtonThemeConstants.RootLayoutPart` | `"PART_RootLayout"` | 根布局面板 |
| `ButtonThemeConstants.LoadingIconPart` | `"PART_LoadingIcon"` | 加载旋转图标 |
| `ButtonThemeConstants.ButtonIconPart` | `"PART_ButtonIcon"` | 按钮图标展示器 |
| `ButtonThemeConstants.ContentPresenterPart` | `"PART_ContentPresenter"` | 内容展示器 |

