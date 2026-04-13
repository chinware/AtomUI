# Tag 标签

## 概述

标签（Tag）用于进行标记和分类。它是一种轻量级的信息标注方式，可以为内容添加语义化的分类标识。标签支持多种预设颜色、状态颜色、自定义颜色，以及可关闭、可带图标等功能。

AtomUI 的 `Tag` 控件完整复刻了 [Ant Design 5.0 Tag](https://ant.design/components/tag-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验。

---

## 设计原理

### Ant Design 的标签设计哲学

Ant Design 对标签的定位是：**「进行标记和分类的小标签」**。标签通常出现在以下场景：
- **分类标识**：为文章、商品、任务等添加分类标签
- **属性标注**：标注状态（成功、错误、警告、信息）
- **可选标签**：用户可以选择或取消选择的标签（如筛选条件）
- **可关闭标签**：用户可以移除的标签（如已选筛选项、邮件标签）

**三种颜色模式**：

| 模式 | 设计意图 | 典型用途 |
|---|---|---|
| 🎨 **预设颜色（Preset Color）** | 使用 Ant Design 调色板生成的 13 种预设颜色，提供一致的视觉体验 | 分类标签、属性标注 |
| 🚦 **状态颜色（Status Color）** | 四种语义化状态色（Success/Info/Warning/Error），传达明确的业务含义 | 状态指示、告警标注 |
| 🖌️ **自定义颜色（Custom Color）** | 通过 CSS 颜色值直接指定，背景填充为指定色，文字自动变为白色 | 品牌色标签、特殊标注 |

**13 种预设颜色**：`magenta`、`red`、`volcano`、`orange`、`gold`、`lime`、`green`、`cyan`、`blue`、`geekblue`、`purple`、`pink`、`yellow`。每种颜色由调色板算法生成 1 号色（浅背景）、3 号色（浅边框）、6 号色（深色）、7 号色（文字色）。

### AtomUI 的扩展设计

AtomUI `Tag` 在 Ant Design 规范基础上做了以下实现：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **三种颜色模式** | `TagColor` 字符串属性 + 运行时解析 | 统一的颜色设置接口，自动识别预设色/状态色/自定义色 |
| **可关闭** | `IsClosable` 属性 + `CloseIcon` + `Closed` 事件 | 内置关闭按钮和事件，无需手动组装 |
| **图标支持** | `Icon` 属性（`PathIcon` 类型） | 图标 + 文字组合标签 |
| **有/无边框** | `Bordered` 属性 | 无边框模式适合背景色场景 |
| **Design Token** | `TagToken` + `RegisterTokenResourceScope` | 所有视觉值从 Token 派生 |

---

## 功能详解

### 颜色系统（TagColor）

Tag 通过 `TagColor` 属性统一设置颜色，运行时自动识别颜色类型：

| 颜色类型 | 识别规则 | 视觉效果 | 伪类 |
|---|---|---|---|
| **预设颜色** | 值匹配 13 种预设名称（如 `"blue"`、`"green"`） | 浅色背景 + 同系边框 + 深色文字 | `:preset-color` |
| **状态颜色** | 值匹配 4 种状态名（`"success"`/`"info"`/`"warning"`/`"error"`） | 语义化背景 + 语义化边框 + 语义色文字 | `:status-color` |
| **自定义颜色** | 值为合法 CSS 颜色（如 `"#f50"`、`"rgb(255,0,0)"`） | 纯色背景填充 + 白色文字 + 无边框 | `:custom-color` |

### 可关闭标签

当 `IsClosable = true` 时：
1. 模板右侧显示关闭图标（默认为 `CloseOutlined`）
2. 点击关闭图标触发 `Closed` 路由事件
3. 可通过 `CloseIcon` 属性自定义关闭图标

### 边框控制

`Bordered` 属性（默认 `true`）控制是否显示边框：
- `true`：正常显示边框（厚度由 `SharedToken.BorderThickness` 控制）
- `false`：隐藏边框，适合在有色背景下使用

当设置自定义颜色时，`Bordered` 会自动设为 `false`。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 预设颜色 `color` | ✅ 13 种预设色 | ✅ `TagColor` 属性 | ✅ 完全对齐 |
| 状态颜色 | ✅ success/info/warning/error | ✅ `TagColor` 属性 | ✅ 完全对齐 |
| 自定义颜色 | ✅ CSS 颜色值 | ✅ `TagColor` 属性 | ✅ 完全对齐 |
| 可关闭 `closable` | ✅ 布尔属性 | ✅ `IsClosable` 属性 | ✅ 完全对齐 |
| 关闭事件 `onClose` | ✅ 回调 | ✅ `Closed` 路由事件 | ✅ 完全对齐 |
| 图标 `icon` | ✅ ReactNode | ✅ `PathIcon` 属性 | ✅ 对齐（类型不同，语义一致） |
| 有边框 `bordered` | ✅ 布尔属性 | ✅ `Bordered` 属性 | ✅ 完全对齐 |
| 关闭图标 `closeIcon` | ✅ ReactNode | ✅ `CloseIcon` 属性 | ✅ 完全对齐 |
| 可选中 `CheckableTag` | ✅ 独立组件 | ❌ 暂未支持 | ⚠️ 待支持 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Controls.Commons.AbstractTag       ← 设备无关基类
        └── AtomUI.Desktop.Controls.Tag          ← 桌面实现
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础设施、`Background`/`Foreground`/`BorderBrush` 等外观属性 |
| `AbstractTag`（基类层） | `TagColor` 颜色系统、`IsClosable` 关闭功能、`Icon`/`CloseIcon` 图标支持、`Bordered` 边框控制、`Closed` 事件、预设色/状态色/自定义色解析逻辑 |
| `Tag`（桌面层） | 注册 `TagToken.ScopeProvider`，由主题控制视觉表现 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/Tag/AbstractTag.cs` | 设备无关基类 |
| 枚举 | `src/AtomUI.Controls/Tag/TagEnums.cs` | TagStatus 枚举 |
| 伪类常量 | `src/AtomUI.Controls/Tag/TagPseudoClass.cs` | 共享伪类定义 |
| 控件类 | `src/AtomUI.Desktop.Controls/Tag/Tag.cs` | 桌面端 Tag |
| Token 定义 | `src/AtomUI.Desktop.Controls/Tag/TagToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Tag/Themes/TagTheme.axaml` | ControlTheme AXAML |
| 主题代码 | `src/AtomUI.Desktop.Controls/Tag/Themes/TagTheme.cs` | 主题 Code-behind |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TagShowCase.axaml` | 使用范例 |

---

## 模板结构

```
Panel
├── Border#Frame                              ← 主框架（背景 + 边框 + 圆角）
└── DockPanel (LastChildFill=True)            ← 内容布局容器
    ├── IconPresenter#IconPresenter (Dock=Left) ← 图标展示器（可选）
    ├── IconButton#PART_CloseButton (Dock=Right) ← 关闭按钮（IsClosable=True 时可见）
    └── TextBlock#TagTextLabel                  ← 标签文字（填充剩余空间）
```

**模板设计理由：**
- **Panel + Border 分离**：Border 仅负责背景/边框渲染，DockPanel 独立管理内容布局，避免布局和装饰耦合。
- **DockPanel 布局**：图标固定在左侧、关闭按钮固定在右侧、文字填充中间，自然适应不同组合场景。
- **IconButton 作为关闭按钮**：复用 `IconButton` 控件，自带悬浮/点击反馈，无需额外实现交互。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `PART_CloseButton` | `"PART_CloseButton"` | 关闭按钮 |
