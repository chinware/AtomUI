# FloatButton 悬浮按钮

## 概述

悬浮按钮（FloatButton）是悬浮在页面特定位置的操作按钮，常用于全局性的快捷操作，如返回顶部、在线客服、反馈等。它始终浮动在内容上方，不随页面滚动而消失。

FloatButton 对应 Ant Design 的 [FloatButton](https://ant.design/components/float-button-cn) 组件，在 AtomUI 中提供了完整的悬浮按钮体系，包括单个悬浮按钮（`FloatButton`）、悬浮按钮宿主（`FloatButtonHost`）、悬浮按钮组（`FloatButtonGroup` / `FloatButtonGroupHost`）以及回到顶部按钮（`BackTopFloatButton` / `BackTopFloatButtonHost`）。

---

## 设计原理

### Ant Design 对标

Ant Design 的 `FloatButton` 是一个固定在页面右下角的操作按钮，支持圆形和方形两种形状。`FloatButton.Group` 则将多个悬浮按钮组合在一起，支持展开/收起的触发模式。`FloatButton.BackTop` 提供了返回顶部的快捷功能。

AtomUI 完整实现了以上所有功能，并在架构上进行了跨平台分层设计。

### Host / Button 分离架构

FloatButton 体系采用了 **Host（宿主）+ Button（按钮）** 的分离架构。开发者在 AXAML 中使用 `*Host` 控件，Host 负责在 `OverlayLayer` 中创建和管理实际的 `FloatButton` 实例：

```
用户代码 → FloatButtonHost (AXAML 声明)
                │
                └── 创建 FloatButton 并放置到 ScopeAwareOverlayLayer
                         │
                         └── 悬浮在页面内容之上
```

这种设计的优势：
- **OverlayLayer 管理**：悬浮按钮需要脱离常规布局流，Host 自动处理 OverlayLayer 的创建和销毁
- **属性同步**：Host 将所有属性通过绑定转发给内部 FloatButton，对外保持统一的 API
- **生命周期管理**：Host 在 `OnAttachedToVisualTree` / `OnDetachedFromVisualTree` 时自动管理 FloatButton 的创建和清理

### 控件体系

FloatButton 体系包含以下控件：

| 控件 | 层级 | 说明 |
|---|---|---|
| `AbstractFloatButton` | 基类（`AtomUI.Controls`） | 设备无关的按钮基类 |
| `FloatButton` | 平台（`AtomUI.Desktop.Controls`） | 桌面悬浮按钮 |
| `AbstractFloatButtonHost` | 基类（`AtomUI.Controls`） | 宿主基类 |
| `FloatButtonHost` | 平台（`AtomUI.Desktop.Controls`） | 单按钮宿主 |
| `FloatButtonGroup` | 平台（`AtomUI.Desktop.Controls`） | 按钮分组（内部控件） |
| `FloatButtonGroupHost` | 平台（`AtomUI.Desktop.Controls`） | 分组宿主（用户使用） |
| `AbstractBackTopFloatButton` | 基类（`AtomUI.Controls`） | 返回顶部按钮基类 |
| `BackTopFloatButton` | 平台（`AtomUI.Desktop.Controls`） | 桌面返回顶部按钮 |
| `AbstractBackTopFloatButtonHost` | 基类（`AtomUI.Controls`） | 返回顶部宿主基类 |
| `BackTopFloatButtonHost` | 平台（`AtomUI.Desktop.Controls`） | 返回顶部宿主 |

### 定位机制

FloatButton 使用 `ScopeAwareOverlayLayer` 进行绝对定位。`Placement` 属性控制悬浮按钮的位置（支持 9 种方位），`FloatOffsetX` / `FloatOffsetY` 用于微调偏移。定位计算通过 `AbstractFloatButton.CalculatePosition()` 静态方法实现，根据按钮尺寸和 OverlayLayer 尺寸计算绝对坐标。

### 徽标系统

FloatButton 内置了 Badge 支持，通过 `IsBadgeEnabled`、`IsDotBadge`、`BadgeCount` 等属性可以在按钮上显示数字徽标或圆点徽标。徽标使用 `Canvas` 布局实现绝对定位，偏移量由 Token 中的 `SquareBadgeOffset` / `CircleBadgeOffset` 计算。

---

## 功能详解

### 按钮类型

通过 `ButtonType` 属性控制样式：
- `Default`：默认样式（白色背景 + 阴影）
- `Primary`：主要样式（主色背景 + 白色图标）

### 按钮形状

通过 `Shape` 属性控制形状：
- `Circle`（默认）：圆形悬浮按钮，自动根据尺寸计算圆角
- `Square`：方形悬浮按钮（支持 Description 描述文本）

### 描述文本

当 `Shape` 为 `Square` 时，可通过 `Description` 属性在按钮图标下方显示描述文本。通过 `DescriptionTemplate` 可自定义描述内容的数据模板。

### Tooltip

通过 `Tooltip` 属性设置鼠标悬浮时的工具提示文本，`TooltipColor` 可自定义提示颜色。

### 默认图标

当未设置图标时，FloatButton 会自动使用 `FileTextOutlined` 作为默认图标。FloatButtonGroup 的触发按钮默认图标为 `FileTextOutlined`，关闭图标为 `CloseOutlined`。

### FloatButtonGroup 触发模式

`FloatButtonGroupHost` 支持三种触发模式（`Trigger` 属性）：
- `Default`：所有按钮始终展开显示，无触发按钮
- `Click`：点击触发按钮展开/收起子按钮，点击外部区域自动收起
- `Hover`：鼠标悬浮展开、离开收起

子按钮展开方向由 `MenuPlacement` 控制（`Top` / `Bottom` / `Left` / `Right`）。展开/收起使用 `MoveIn/Out` 系列动画。

### 返回顶部

`BackTopFloatButton` / `BackTopFloatButtonHost` 提供一键回到页面顶部的功能：
- 当关联的 `ScrollViewer` 滚动超过 `VisibilityHeight`（默认 400px）时显示
- 点击后在 `ToTopDuration`（默认 450ms）内平滑滚动到顶部
- 通过 `Target` 属性可显式指定关联的 `ScrollViewer`，未设置时自动查找父级

---

## 与 Ant Design 对齐程度

| 特性 | Ant Design | AtomUI | 对齐情况 |
|---|---|---|---|
| 圆形/方形 | ✅ `shape` | ✅ `Shape` | ✅ 完全对齐 |
| Default/Primary 类型 | ✅ `type` | ✅ `ButtonType` | ✅ 完全对齐 |
| 图标 | ✅ `icon` | ✅ `Icon` | ✅ 完全对齐 |
| Tooltip | ✅ `tooltip` | ✅ `Tooltip` | ✅ 完全对齐 |
| 描述文本 | ✅ `description` | ✅ `Description` | ✅ 完全对齐 |
| 按钮组 | ✅ `FloatButton.Group` | ✅ `FloatButtonGroupHost` | ✅ 完全对齐 |
| 触发模式 | ✅ `trigger` | ✅ `Trigger` | ✅ 完全对齐 |
| 菜单展开方向 | ✅ — | ✅ `MenuPlacement` | ✅ 扩展 |
| 徽标 | ✅ `badge` | ✅ `IsBadgeEnabled` / `BadgeCount` | ✅ 完全对齐 |
| 返回顶部 | ✅ `FloatButton.BackTop` | ✅ `BackTopFloatButtonHost` | ✅ 完全对齐 |
| 定位 | ✅ CSS fixed | ✅ `Placement` + `FloatOffset` | ✅ 完全对齐 |
| 9 方位定位 | ❌ 仅右下 | ✅ 9 种 `Placement` | ✅ 超越 |
| 链接跳转 | ✅ `href` | ✅ `Href` | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Button
  └── AtomUI.Controls.Commons.AbstractFloatButton (IMotionAwareControl)
        ├── AtomUI.Desktop.Controls.FloatButton
        └── AtomUI.Controls.Commons.AbstractBackTopFloatButton
              └── AtomUI.Desktop.Controls.BackTopFloatButton

Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Controls.Commons.AbstractFloatButtonHost (IMotionAwareControl)
        ├── AtomUI.Desktop.Controls.FloatButtonHost
        └── AtomUI.Controls.Commons.AbstractBackTopFloatButtonHost
              └── AtomUI.Desktop.Controls.BackTopFloatButtonHost

Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.FloatButtonGroup (IMotionAwareControl)

Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.FloatButtonGroupHost (IMotionAwareControl)
```

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/FloatButton/AbstractFloatButton.cs` | 悬浮按钮基类 |
| 基类 | `src/AtomUI.Controls/FloatButton/AbstractFloatButtonHost.cs` | 宿主基类 |
| 基类 | `src/AtomUI.Controls/FloatButton/AbstractBackTopFloatButton.cs` | 返回顶部按钮基类 |
| 基类 | `src/AtomUI.Controls/FloatButton/AbstractBackTopFloatButtonHost.cs` | 返回顶部宿主基类 |
| 枚举 | `src/AtomUI.Controls/FloatButton/FloatButtonEnums.cs` | 枚举定义 |
| 控件 | `src/AtomUI.Desktop.Controls/FloatButton/FloatButton.cs` | 桌面悬浮按钮 |
| 宿主 | `src/AtomUI.Desktop.Controls/FloatButton/FloatButtonHost.cs` | 桌面宿主容器 |
| 分组 | `src/AtomUI.Desktop.Controls/FloatButton/FloatButtonGroup.cs` | 按钮分组 |
| 分组宿主 | `src/AtomUI.Desktop.Controls/FloatButton/FloatButtonGroupHost.cs` | 分组宿主 |
| 返回顶部 | `src/AtomUI.Desktop.Controls/FloatButton/BackTopFloatButton.cs` | 桌面返回顶部按钮 |
| 返回顶部宿主 | `src/AtomUI.Desktop.Controls/FloatButton/BackTopFloatButtonHost.cs` | 返回顶部宿主 |
| Token | `src/AtomUI.Desktop.Controls/FloatButton/FloatButtonToken.cs` | 组件 Token |
| 主题 | `src/AtomUI.Desktop.Controls/FloatButton/Themes/` | ControlTheme 定义 |
| Gallery | `controlgallery/AtomUIGallery/ShowCases/Views/General/FloatButtonShowCase.axaml` | 使用范例 |
