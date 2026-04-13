# Switch 开关

## 概述

开关（ToggleSwitch）用于在两种状态之间切换，类似于物理开关。与 Checkbox 不同的是，开关的切换会直接触发状态变更（而非等待表单提交），更适合即时生效的设置项。

AtomUI 的 `ToggleSwitch` 控件完整复刻了 [Ant Design 5.0 Switch](https://ant.design/components/switch-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为，包括滑块动画、加载状态、开/关文字等功能。

> **注意**：AtomUI 中该控件命名为 `ToggleSwitch`（非 `Switch`），以避免与 C# `switch` 关键字冲突。

---

## 设计原理

### Ant Design 的开关设计哲学

Ant Design 对开关的定位是：**「开关选择器，代替单个复选框，切换操作会直接触发状态变更」**。适用于：
- **即时生效**的开/关设置（如启用通知、开启暗黑模式）
- 明确的**二元状态**切换（开 vs 关，启用 vs 禁用）

### Avalonia ToggleButton 基础能力

AtomUI 的 `ToggleSwitch` 通过 `AbstractToggleSwitch` 继承自 Avalonia 的 `ToggleButton`，获得 `IsChecked` 状态管理和点击切换行为。

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **开/关内容** | `OnContent` / `OffContent` 属性 | 在开关轨道内显示文字或图标，增强状态辨识 |
| **两种尺寸** | `ISizeTypeAware` 接口 + `SizeType` | 默认尺寸和小号尺寸 |
| **加载状态** | `IsLoading` 属性 | 异步操作时显示加载指示，禁止切换 |
| **滑块动画** | 自定义 `SwitchKnob` + 滑动过渡 | 滑块移动 + 拉伸效果，复刻 Ant Design 动画 |
| **点击波纹** | `IWaveSpiritAwareControl` | 状态切换时的涟漪反馈 |
| **表单集成** | `IFormItemAware` 接口 | 可参与 Form 验证流程 |
| **Design Token** | `ToggleSwitchToken` | 所有视觉值从 Token 派生 |

---

## 功能详解

### 开/关内容（OnContent / OffContent）

开关轨道内可以显示文字或图标，增强状态辨识度：
- `OnContent`：开启状态时轨道内显示的内容
- `OffContent`：关闭状态时轨道内显示的内容
- 支持 `OnContentTemplate` / `OffContentTemplate` 自定义模板

### 加载状态（IsLoading）

当 `IsLoading = true` 时：
1. 滑块内显示旋转加载指示器
2. 开关不响应点击（`HitTest` 返回 `false`）
3. 光标变为箭头（非手型）

### 按压拉伸动画

按下开关时滑块宽度会拉伸（乘以 1.3 倍），释放后恢复正常宽度，复刻 Ant Design 的微交互效果。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 开关切换 | ✅ `checked` | ✅ `IsChecked` | ✅ 完全对齐 |
| 两种尺寸 | ✅ `size` | ✅ `SizeType` | ✅ 完全对齐 |
| 开关文字 | ✅ `checkedChildren/unCheckedChildren` | ✅ `OnContent/OffContent` | ✅ 完全对齐 |
| 加载中 | ✅ `loading` | ✅ `IsLoading` | ✅ 完全对齐 |
| 禁用 | ✅ `disabled` | ✅ `IsEnabled` | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.ToggleButton
  └── AtomUI.Controls.AbstractToggleSwitch (ISizeTypeAware, IWaveSpiritAwareControl, IFormItemAware)  ← 设备无关基类
        └── AtomUI.Desktop.Controls.ToggleSwitch                                                      ← 桌面实现
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ToggleButton` | `IsChecked` 状态管理、`:checked`/`:unchecked` 伪类、点击切换 |
| `AbstractToggleSwitch`（基类层） | 开/关内容、尺寸、加载状态、自定义渲染（滑槽绘制）、滑块动画、波纹效果、表单集成 |
| `ToggleSwitch`（桌面层） | 注册 `ToggleSwitchToken.ScopeProvider` |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Middle / Small）两种尺寸 |
| `IWaveSpiritAwareControl` | `AtomUI.Controls.Shared` | 状态切换时的涟漪动画 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 子控件参与表单验证 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/Switch/AbstractToggleSwitch.cs` | 设备无关基类 |
| 滑块 | `src/AtomUI.Controls/Switch/SwitchKnob.cs` | 滑块控件（内部） |
| 控件类 | `src/AtomUI.Desktop.Controls/Switch/ToggleSwitch.cs` | 桌面端 ToggleSwitch |
| Token 定义 | `src/AtomUI.Desktop.Controls/Switch/ToggleSwitchToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Switch/Themes/ToggleSwitchTheme.axaml` | ControlTheme AXAML |
| 主题代码 | `src/AtomUI.Desktop.Controls/Switch/Themes/ToggleSwitchTheme.cs` | 主题 Code-behind |

---

## 模板结构

ToggleSwitch 采用自定义渲染方式（override `Render`），滑槽由代码绘制，模板仅包含内部组件：

```
Panel                                            ← 根面板
├── WaveSpiritDecorator#PART_WaveSpirit          ← 波纹效果层
└── Canvas#PART_MainContainer (ClipToBounds)     ← 主容器（裁剪越界内容）
    ├── SwitchKnob#PART_SwitchKnob               ← 滑块（含加载指示器）
    ├── ContentPresenter#PART_OnContentPresenter  ← 开启状态内容
    └── ContentPresenter#PART_OffContentPresenter ← 关闭状态内容
```

### 模板部件常量

| 常量值 | 说明 |
|---|---|
| `"PART_WaveSpirit"` | 波纹动画装饰器 |
| `"PART_MainContainer"` | Canvas 主容器 |
| `"PART_SwitchKnob"` | 滑块控件（SwitchKnob） |
| `"PART_OnContentPresenter"` | 开启状态内容展示器 |
| `"PART_OffContentPresenter"` | 关闭状态内容展示器 |

**模板设计理由：**
- **自定义 Render**：滑槽（Groove）由 `DrawPilledRect` 绘制药丸形状，避免使用 Border 难以实现的圆角效果。
- **波纹层独立**：`WaveSpiritDecorator` 与 Canvas 同级放在 Panel 中，波纹效果不受 Canvas 的 `ClipToBounds` 裁剪影响，可向外扩散。
- **Canvas 容器**：滑块和内容使用绝对定位，配合动画精确控制位置。内容切换时通过动画移入/移出 Canvas 可视区域，`ClipToBounds` 裁剪不可见部分。
- **SwitchKnob 独立组件**：封装滑块渲染 + 加载指示器旋转动画，内部维护旋转角度状态。
