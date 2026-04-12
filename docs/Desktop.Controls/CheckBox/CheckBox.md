# CheckBox 复选框

## 概述

复选框（CheckBox）用于在一组选项中进行多选操作，允许用户从多个备选项中选择零个或多个。它支持三种状态：选中（Checked）、未选中（Unchecked）和半选中/不确定态（Indeterminate），半选态常用于实现「全选/部分选」的级联交互。

AtomUI 的 `CheckBox` 控件复刻了 [Ant Design 5.0 Checkbox](https://ant.design/components/checkbox-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的复选框设计哲学

Ant Design 对复选框的定位是：**「在一组可选项中进行多项选择」**。其核心交互特征包括：

1. **独立使用** — 单个复选框表示两种状态（选中/未选中）之间的切换
2. **分组使用** — 多个复选框组成一组，用户可从中选择零个或多个选项
3. **全选联动** — 通过 Indeterminate 半选态实现「全选/部分选」的级联交互

**设计建议（来自 Ant Design）：**
- 单独使用时，用于选择一个或取消一个选项
- 集合使用时，用于收集用户的多项偏好
- 不要使用复选框来替代开关（Switch），复选框侧重于"收集选择"，开关侧重于"立即切换"

### Avalonia CheckBox 基础能力

AtomUI 的 `CheckBox` 继承自 Avalonia 框架的 `Avalonia.Controls.CheckBox`（通过 `AbstractCheckBox` 中间层）。Avalonia CheckBox 继承自 `ToggleButton`，支持三态切换。其继承链为：

```
Control → TemplatedControl → ContentControl → Button → ToggleButton → CheckBox
```

**Avalonia CheckBox 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Content` | 复选框旁的文本内容 |
| `IsChecked` | 选中状态：`true`（选中）、`false`（未选中）、`null`（不确定态） |
| `IsThreeState` | 是否启用三态模式（允许 `null` 状态） |
| `Command` / `CommandParameter` | MVVM 命令绑定 |

**Avalonia CheckBox 提供的基础伪类：**

| 伪类 | 说明 |
|---|---|
| `:checked` | 选中状态 |
| `:unchecked` | 未选中状态 |
| `:indeterminate` | 不确定态（半选） |
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 按下 |
| `:disabled` | 禁用 |

### AtomUI 的扩展设计

AtomUI `CheckBox` 在 Avalonia CheckBox 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **自定义指示器** | `CheckBoxIndicator` 内部控件 | 独立的选中/半选指示器，精确复刻 Ant Design 的勾选动画和圆角样式 |
| **点击波纹** | `IWaveSpiritAwareControl` + `WaveSpiritDecorator` | 复刻 Ant Design 的 Wave 点击涟漪效果 |
| **选中动画** | `CheckedMarkRenderTransform` + scale 过渡 | 选中时勾号从缩放 0.1 过渡到 1.0 的动画效果 |
| **复选框组** | `CheckBoxGroup` 容器控件 | 对齐 Ant Design 的 `Checkbox.Group`，支持分组管理、自动布局 |
| **数据驱动分组** | `CheckBoxGroup.ItemsSource` + `ICheckBoxOption` | 支持绑定数据集合生成复选框组 |
| **表单集成** | `IFormItemAware` 接口 | 可参与 FormItem 验证流程 |
| **过渡动画** | `IsMotionEnabled` | 全局动画开关 |
| **Design Token** | `CheckBoxToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 三种状态

CheckBox 支持三种选中状态：

| 状态 | `IsChecked` 值 | 视觉表现 | 典型用途 |
|---|---|---|---|
| **未选中** | `false` | 空白方框 | 默认状态 |
| **选中** | `true` | 蓝色背景 + 白色对勾 | 用户主动选择 |
| **不确定** | `null` | 白色背景 + 蓝色横杠 | 全选框部分子项被选中 |

默认为两态模式（`IsThreeState = false`），仅在 `true` ↔ `false` 之间切换。设置 `IsThreeState = true` 后启用三态循环：`false` → `true` → `null` → `false`。

### 复选框组（CheckBoxGroup）

`CheckBoxGroup` 是管理一组复选框的容器控件，提供统一的布局和选中状态管理：

- 支持 `Orientation`（水平/垂直排列）
- 支持 `ItemSpacing` / `LineSpacing` 控制间距
- 通过 `CheckedItems` 属性获取/设置选中项集合
- 触发 `CheckedChanged` 路由事件通知选中状态变化
- 支持 `ItemsSource` + `ItemTemplate` 数据驱动模式

### 指示器动画

CheckBox 使用自定义 `CheckBoxIndicator` 控件渲染选中标记：
- **选中时**：对勾图标从 `scale(0.1)` 过渡到 `scale(1.0)`，产生弹入效果
- **半选时**：显示蓝色横杠
- **波纹效果**：点击时通过 `WaveSpiritDecorator` 产生方形波纹动画

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本复选框 | ✅ `<Checkbox>` | ✅ `<atom:CheckBox>` | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ 布尔属性 | ✅ `IsEnabled` | ✅ 完全对齐 |
| 三态 `indeterminate` | ✅ 布尔属性 | ✅ `IsChecked=null` + `IsThreeState` | ✅ 完全对齐 |
| 复选框组 `Checkbox.Group` | ✅ 组容器 | ✅ `CheckBoxGroup` | ✅ 完全对齐 |
| 分组选项 `options` | ✅ 数组配置 | ✅ `ItemsSource` + `ICheckBoxOption` | ✅ 完全对齐 |
| `onChange` | ✅ 回调 | ✅ `IsCheckedChanged` / `CheckedChanged` 事件 | ✅ 完全对齐 |
| 全选联动 | ✅ 示例实现 | ✅ 通过 Indeterminate 态实现 | ✅ 完全对齐 |

---

## 继承关系

### CheckBox

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Button
              └── Avalonia.Controls.Primitives.ToggleButton
                    └── Avalonia.Controls.CheckBox
                          └── AtomUI.Controls.Commons.AbstractCheckBox
                                ├── implements IWaveSpiritAwareControl
                                └── implements IFormItemAware
                                      └── AtomUI.Desktop.Controls.CheckBox

```

### CheckBoxGroup

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Controls.Commons.AbstractCheckBoxGroup
        ├── implements IMotionAwareControl
        └── implements IFormItemAware
              └── AtomUI.Desktop.Controls.CheckBoxGroup
```

**实现的共享接口：**

| 接口 | 控件 | 作用 |
|---|---|---|
| `IWaveSpiritAwareControl` | CheckBox | 支持点击涟漪（Wave）动画效果 |
| `IFormItemAware` | CheckBox / CheckBoxGroup | 可参与 FormItem 验证流程 |
| `IMotionAwareControl` | CheckBoxGroup | 支持 `IsMotionEnabled` 过渡动画开关 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/CheckBox/AbstractCheckBox.cs` | 跨平台基类 |
| 基类 | `src/AtomUI.Controls/CheckBox/AbstractCheckBoxGroup.cs` | 复选框组跨平台基类 |
| 数据模型 | `src/AtomUI.Controls/CheckBox/CheckBoxOption.cs` | `ICheckBoxOption` 接口和默认实现 |
| 控件类 | `src/AtomUI.Desktop.Controls/CheckBox/CheckBox.cs` | 桌面端 CheckBox |
| 控件类 | `src/AtomUI.Desktop.Controls/CheckBox/CheckBoxGroup.cs` | 桌面端 CheckBoxGroup |
| Token 定义 | `src/AtomUI.Desktop.Controls/CheckBox/CheckBoxToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/CheckBox/Themes/CheckBoxTheme.axaml` | CheckBox ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/CheckBox/Themes/CheckBoxGroupTheme.axaml` | CheckBoxGroup ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/CheckBox/Themes/CheckBoxIndicatorTheme.axaml` | 指示器 ControlTheme |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/CheckBoxShowCase.axaml` | 使用范例 |

---

## 模板结构

### CheckBox 模板

```
Border#Frame (Padding=0,1,0,1)
└── DockPanel
    ├── CheckBoxIndicator#Indicator (DockPanel.Dock=Left)  ← 选中指示器
    │   └── Panel
    │       ├── WaveSpiritDecorator (PART_WaveSpirit)     ← 点击波纹
    │       └── Border#Frame                               ← 指示器框架
    │           └── Panel
    │               ├── CheckBoldOutlined#CheckedMark     ← 对勾图标（选中时可见）
    │               └── Rectangle#TristateMark            ← 横杠（半选时可见）
    └── ContentPresenter#ContentPresenter                  ← 文本内容

```

### CheckBoxGroup 模板

```
Border#Frame
└── CheckBoxItemsControl (PART_CheckBoxItems)  ← 内部 SelectingItemsControl
    └── WrapPanel                               ← 自动换行布局
        ├── CheckBox[0]
        ├── CheckBox[1]
        └── ...
```
