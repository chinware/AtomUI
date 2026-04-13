# Drawer 抽屉

## 概述

抽屉（Drawer）是从屏幕边缘滑出的面板，用于展示附加内容或进行操作。支持上下左右四个方向滑入/滑出，适合展示详情、编辑表单、设置面板等不需要离开当前页面的辅助内容。

AtomUI 的 `Drawer` 控件对应 Ant Design 的 [Drawer 组件](https://ant.design/components/drawer-cn)。

---

## 设计原理

### Ant Design 的抽屉设计哲学

Ant Design 对 Drawer 的定位是：**屏幕边缘滑出的浮层面板**。与 Modal（对话框）相比，Drawer 的特点是：

| 对比维度 | Modal（对话框） | Drawer（抽屉） |
|---|---|---|
| 出现方式 | 居中弹出 | 从边缘滑入 |
| 适用内容 | 简短确认、简单表单 | 详情展示、长表单、多步操作 |
| 视觉焦点 | 强聚焦（居中阻断） | 半聚焦（侧边展开，不完全遮挡主内容） |
| 典型场景 | 确认删除、简单编辑 | 查看详情、编辑属性、设置面板、帮助文档 |

**典型使用场景：**

| 场景 | 说明 |
|---|---|
| 📋 **详情查看** | 在列表页点击条目，从右侧滑出详情面板 |
| 📝 **表单编辑** | 不离开当前页面，从侧边展开编辑表单 |
| ⚙️ **设置面板** | 应用设置、过滤器配置等 |
| 📑 **多级抽屉** | 在一个抽屉内打开另一个抽屉，处理多分支任务 |

### AtomUI 的扩展设计

AtomUI `Drawer` 在 Ant Design 基础上提供了以下能力：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **四个方向** | `DrawerPlacement` 枚举 | 支持 Left / Top / Right / Bottom 四个方向滑入 |
| **三种预设尺寸** | `CustomizableSizeType` + Token | Small（378px）、Middle（520px）、Large（736px） |
| **自定义尺寸** | `Dimension` 类型的 `DialogSize` | 支持像素值和百分比 |
| **多级抽屉** | 自动检测父级 Drawer | 打开子抽屉时父抽屉自动推开（Push 效果） |
| **作用域渲染** | `OpenOn` + `ScopeAwareAdornerLayer` | 可以在指定控件区域内渲染，而非全屏 |
| **遮罩控制** | `IsShowMask` / `CloseWhenClickOnMask` | 精细控制遮罩层的显示和交互行为 |
| **标题/额外/底部插槽** | `Title` / `Extra` / `Footer` | 丰富的内容插槽 |
| **滑入动画** | `IMotionAwareControl` | 可控的滑入/滑出动画效果 |

---

## 功能详解

### 四方向支持（DrawerPlacement）

通过 `Placement` 属性设置抽屉从哪个边缘滑入：

| 值 | 说明 |
|---|---|
| `Left` | 从左侧滑入 |
| `Top` | 从顶部滑入 |
| `Right` | 从右侧滑入（默认） |
| `Bottom` | 从底部滑入 |

### 预设尺寸

通过 `SizeType` 属性选择预设尺寸：

| SizeType | 默认宽/高度 | 说明 |
|---|---|---|
| `Small`（默认） | 378px | 适合简单内容 |
| `Middle` | 520px | 中等内容 |
| `Large` | 736px | 大量内容或复杂表单 |
| `Custom` | 由 `DialogSize` 指定 | 自定义像素或百分比 |

### 多级抽屉（Push 效果）

在一个已打开的 Drawer 内容中声明另一个 Drawer，打开子抽屉时：
- 父抽屉会自动向反方向推移（Push），推移量由 `PushOffsetPercent`（默认 40%）控制
- 子抽屉关闭后父抽屉自动恢复原位

**属性自动继承**：当 Drawer 检测到自身位于另一个 Drawer（DrawerContainer）内部时，会在 `OnAttachedToVisualTree` 阶段自动继承以下父级属性（优先级为 `BindingPriority.Template`，即用户显式设置的值不会被覆盖）：

| 自动继承的属性 | 说明 |
|---|---|
| `OpenOn` | 子抽屉的渲染范围自动与父抽屉一致 |
| `IsMotionEnabled` | 子抽屉的动画开关自动跟随父抽屉 |

若 Drawer 不在任何父级 Drawer 内部，`OpenOn` 会自动绑定到最近的 `TopLevel`（窗口）。

### 作用域渲染（OpenOn）

默认情况下 Drawer 在 `TopLevel`（整个窗口）上渲染。通过 `OpenOn` 属性可以指定渲染范围：

```xml
<atom:Drawer OpenOn="{Binding $parent[Panel]}" />
```

此时 Drawer 仅在目标控件的区域内显示，不会遮挡窗口的其他部分。

### 遮罩层控制

- `IsShowMask`（默认 `true`）：是否显示半透明遮罩层
- `CloseWhenClickOnMask`（默认 `true`）：点击遮罩层是否关闭抽屉

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 四方向 `placement` | ✅ `top / right / bottom / left` | ✅ `DrawerPlacement` 枚举 | ✅ 完全对齐 |
| 标题 `title` | ✅ | ✅ `Title` 属性 | ✅ 完全对齐 |
| 关闭按钮 `closable` | ✅ | ✅ `IsShowCloseButton` | ✅ 完全对齐 |
| 遮罩 `mask` | ✅ | ✅ `IsShowMask` | ✅ 完全对齐 |
| 点击遮罩关闭 `maskClosable` | ✅ | ✅ `CloseWhenClickOnMask` | ✅ 完全对齐 |
| 预设尺寸 `size` | ✅ `default / large` | ✅ `SizeType`（Small/Middle/Large） | ✅ 增强：多一个 Middle 尺寸 |
| 自定义宽/高 `width` / `height` | ✅ | ✅ `DialogSize`（Dimension 类型） | ✅ 增强：支持百分比 |
| 额外操作 `extra` | ✅ | ✅ `Extra` / `ExtraTemplate` | ✅ 完全对齐 |
| 底部区域 `footer` | ✅ | ✅ `Footer` / `FooterTemplate` | ✅ 完全对齐 |
| 多级抽屉 push | ✅ | ✅ 自动 Push + `PushOffsetPercent` | ✅ 完全对齐 |
| 指定容器 `getContainer` | ✅ | ✅ `OpenOn` 属性 | ✅ 完全对齐 |
| 加载中 `loading` | ✅ | ❌ 暂未支持 | ⚠️ 待支持 |

---

## 继承关系

```
Avalonia.Controls.Control
  └── AtomUI.Desktop.Controls.Drawer
        ├── implements IMotionAwareControl
        └── implements ICustomizableSizeTypeAware
```

`Drawer` 直接继承自 Avalonia 的 `Control`。Drawer 本身不渲染内容，而是创建 `DrawerContainer`（在 `ScopeAwareAdornerLayer` 中渲染）来承载实际的抽屉面板。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `Control` | 基础控件设施 |
| `Drawer` | 抽屉生命周期管理（打开/关闭）、属性代理到 DrawerContainer、多级抽屉 Push 协调、尺寸计算、作用域渲染 |

**实现的接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持 `IsMotionEnabled` 动画开关 |
| `ICustomizableSizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `CustomizableSizeType`（Small/Middle/Large/Custom） |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Drawer/Drawer.cs` | Drawer 核心实现 |
| 容器类 | `src/AtomUI.Desktop.Controls/Drawer/DrawerContainer.cs` | DrawerContainer（AdornerLayer 中渲染的面板） |
| 信息容器 | `src/AtomUI.Desktop.Controls/Drawer/DrawerInfoContainer.cs` | 抽屉内容区域容器 |
| 方向枚举 | `src/AtomUI.Desktop.Controls/Drawer/DrawerPlacement.cs` | Left / Top / Right / Bottom |
| Token 定义 | `src/AtomUI.Desktop.Controls/Drawer/DrawerToken.cs` | 组件级 Design Token |
| 主题 | `src/AtomUI.Desktop.Controls/Drawer/Themes/DrawerContainerTheme.axaml` | DrawerContainer 主题 |
| 主题 | `src/AtomUI.Desktop.Controls/Drawer/Themes/DrawerInfoContainerTheme.axaml` | DrawerInfoContainer 主题 |
| 主题注册 | `src/AtomUI.Desktop.Controls/Drawer/Themes/DrawerThemes.axaml` | 主题注册 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/DrawerShowCase.axaml` | 使用范例 |

---

## 模板结构

Drawer 通过创建 `DrawerContainer` 在 `ScopeAwareAdornerLayer` 中渲染：

```
ScopeAwareAdornerLayer
└── DrawerContainer
    ├── Mask Panel                                    ← 遮罩层（点击可关闭）
    └── MotionActorControl (PART_InfoContainerMotionActor)  ← 滑入动画控制器
        └── DrawerInfoContainer (PART_InfoContainer)  ← 抽屉信息容器
            ├── DockPanel (Header)                    ← 标题栏
            │   ├── CloseButton (PART_CloseButton)    ← 关闭按钮
            │   ├── ExtraPresenter                    ← 额外操作区域
            │   └── TitlePresenter                    ← 标题
            ├── ContentPresenter                      ← 内容区域
            └── FooterPresenter                       ← 底部区域
```

### 模板部件常量

| 常量名 | 值 | 所属类 | 说明 |
|---|---|---|---|
| `DrawerContainerThemeConstants.InfoContainerPart` | `"PART_InfoContainer"` | DrawerContainer | 信息容器 |
| `DrawerContainerThemeConstants.InfoContainerMotionActorPart` | `"PART_InfoContainerMotionActor"` | DrawerContainer | 动画控制器 |
| `DrawerInfoContainerThemeConstants.CloseButtonPart` | `"PART_CloseButton"` | DrawerInfoContainer | 关闭按钮 |
