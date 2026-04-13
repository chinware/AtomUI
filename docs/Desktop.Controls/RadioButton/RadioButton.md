# RadioButton 单选框

## 概述

单选框（RadioButton）用于在一组互斥的选项中选择一个。与复选框不同，同一组内的单选框只能有一个被选中——选择新项时，之前的选中项自动取消。这种「互斥选择」的特性使其非常适合性别选择、支付方式、排序方式等场景。

AtomUI 的 `RadioButton` 控件复刻了 [Ant Design 5.0 Radio](https://ant.design/components/radio-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的单选框设计哲学

Ant Design 对单选框的定位是：**「用于在多个备选项中选中单个状态」**。其核心原则包括：

1. **互斥选择** — 同组中只能有一个选中，选中新项时旧项自动取消
2. **明确展示** — 所有选项同时可见，用户可以一次性比较所有选项
3. **适中选项数** — 当选项数量较多（>5）时，建议使用 Select 下拉框代替

**Radio 与 Radio.Group 的关系：**
- 单独使用 `Radio` 时，需手动分组管理
- 使用 `Radio.Group` 时，自动管理组内的互斥逻辑

**Radio 与 Radio.Button 的关系：**
- `Radio` 呈现经典圆形指示器 + 文本的样式
- `Radio.Button` 呈现按钮形态，常用于在几个选项间快速切换（如视图切换）

### Avalonia RadioButton 基础能力

AtomUI 的 `RadioButton` 继承自 Avalonia 框架的 `Avalonia.Controls.RadioButton`（通过 `AbstractRadioButton` 中间层）。Avalonia RadioButton 继承自 `ToggleButton`，通过 `GroupName` 实现同组互斥。其继承链为：

```
Control → TemplatedControl → ContentControl → Button → ToggleButton → RadioButton
```

**Avalonia RadioButton 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Content` | 单选框旁的文本内容，可以是任意控件 |
| `IsChecked` | 是否选中（`bool?` 类型，支持三态） |
| `GroupName` | 分组名称，同名的 RadioButton 互斥 |
| `Command` / `CommandParameter` | MVVM 命令绑定 |
| `ClickMode` | 控制 Click 事件触发时机 |

**Avalonia RadioButton 提供的基础伪类：**

| 伪类 | 说明 |
|---|---|
| `:checked` | 选中状态 |
| `:unchecked` | 未选中状态 |
| `:pointerover` | 鼠标悬浮在控件上 |
| `:pressed` | 控件被按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 控件获得焦点 |
| `:focus-visible` | 通过键盘（Tab）获得焦点 |

### AtomUI 的扩展设计

AtomUI `RadioButton` 在 Avalonia RadioButton 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **自定义指示器** | `RadioIndicator` 内部控件 | 精确复刻 Ant Design 的圆形指示器和选中动画效果 |
| **点击波纹** | `IWaveSpiritAwareControl` + `WaveSpiritDecorator` | 复刻 Ant Design 的 Wave 点击涟漪效果 |
| **过渡动画** | `IsMotionEnabled` + `Transitions` 动态配置 | 边框色、圆点大小、背景色平滑过渡 |
| **单选框组** | `RadioButtonGroup` 容器控件 | 对齐 Ant Design 的 `Radio.Group`，自动管理互斥 |
| **数据驱动分组** | `RadioButtonGroup.ItemsSource` + `IRadioButtonOption` | 支持绑定数据集合，动态生成选项 |
| **表单集成** | `IFormItemAware` 接口 | 可参与 FormItem 验证流程 |
| **Design Token** | `RadioButtonToken` + `RegisterTokenResourceScope` | 所有视觉值从 Token 派生，支持主题切换 |

---

## 功能详解

### 单选框组（RadioButtonGroup）

`RadioButtonGroup` 是管理一组单选框的容器控件，对应 Ant Design 的 `Radio.Group`：
- 自动管理组内互斥逻辑（选中新项时自动取消旧项）
- 通过 `CheckedItem` 属性获取/设置当前选中项
- 支持 `Orientation`（水平/垂直排列）和 `ItemSpacing` / `LineSpacing` 控制间距
- 触发 `CheckedChanged` 事件通知选中变化，提供 `OldCheckedItem` 和 `NewCheckedItem`
- 支持 `ItemsSource` + `ItemTemplate` 数据驱动模式，通过 `IRadioButtonOption` 接口绑定数据
- 默认使用 `WrapPanel` 作为布局面板，自动换行

**声明式用法 vs 数据驱动用法：**
- 声明式：直接在 AXAML 中声明 `<atom:RadioButton>` 子元素
- 数据驱动：通过 `ItemsSource` 绑定 `IRadioButtonOption` 集合，自动生成子项

### 指示器动画

RadioButton 使用自定义 `RadioIndicator` 控件渲染选中圆点：
- **未选中**：空心圆环，背景为 `ColorBgContainer`，边框为 `ColorBorder`
- **选中**：圆形指示器整体变为主色（`ColorPrimary`），内部绘制白色实心圆点，带缩放过渡动画
- **悬浮**：未选中状态下悬浮时，边框色变为 `ColorPrimary`
- **禁用**：背景灰色（`ColorBgContainerDisabled`），选中圆点变为灰色（`DotColorDisabled`）
- **波纹效果**：选中时通过 `WaveSpiritDecorator` 产生圆形波纹（`CircleWave` 类型）

指示器的过渡动画包括：
- `RadioBorderBrush` — 边框颜色平滑过渡
- `RadioDotEffectSize` — 圆点大小缩放过渡
- `RadioBackground` — 背景色快速过渡（使用 `MotionDurationFast`）

### 按钮样式单选框（OptionButtonGroup）

Ant Design 的 `Radio.Button` 在 AtomUI 中通过独立的 `OptionButtonGroup` + `OptionButton` 控件实现，它们是独立控件，拥有独立的 Token 和主题，但在 Gallery 中与 RadioButton 在同一 ShowCase 中展示。OptionButton 的详细文档请参见 `docs/Desktop.Controls/OptionButton/`。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本单选框 | ✅ `<Radio>` | ✅ `<atom:RadioButton>` | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ 布尔属性 | ✅ `IsEnabled` | ✅ 完全对齐 |
| 单选框组 `Radio.Group` | ✅ 组容器 | ✅ `RadioButtonGroup` | ✅ 完全对齐 |
| 按钮样式 `Radio.Button` | ✅ 按钮形态 | ✅ `OptionButtonGroup` + `OptionButton` | ✅ 完全对齐 |
| 按钮填充样式 `buttonStyle` | ✅ `outline`/`solid` | ✅ `ButtonStyle` 属性（OptionButtonGroup） | ✅ 完全对齐 |
| 三种尺寸 | ✅ `size` 属性 | ✅ `SizeType`（仅 OptionButtonGroup） | ✅ 完全对齐 |
| `onChange` | ✅ 回调 | ✅ `CheckedChanged` 事件 | ✅ 完全对齐 |
| `value` / `defaultValue` | ✅ 受控/非受控 | ✅ `CheckedItem` 属性 | ✅ 完全对齐 |
| 自定义内容 | ✅ `children` | ✅ `Content` 属性 | ✅ 完全对齐 |
| 数据驱动 `options` | ✅ 数组 | ✅ `ItemsSource` + `IRadioButtonOption` | ✅ 完全对齐 |

---

## 继承关系

### RadioButton

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Button
              └── Avalonia.Controls.Primitives.ToggleButton
                    └── Avalonia.Controls.RadioButton
                          └── AtomUI.Controls.Commons.AbstractRadioButton
                                ├── implements IWaveSpiritAwareControl
                                └── implements IFormItemAware
                                      └── AtomUI.Desktop.Controls.RadioButton
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ToggleButton` / `RadioButton` | 选中/取消切换行为、`IsChecked` 状态、`GroupName` 同组互斥 |
| `AbstractRadioButton` | 点击波纹接口 (`IWaveSpiritAwareControl`)、表单集成 (`IFormItemAware`)、动画开关 (`IsMotionEnabled`)、`OnPointerPressed` 事件透传 |
| `AtomUI.Desktop.Controls.RadioButton` | 桌面端 Token 注册 (`RadioButtonToken.ScopeProvider`) |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IWaveSpiritAwareControl` | `AtomUI.Controls.Shared` | 支持点击涟漪（Wave）动画效果 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |

### RadioButtonGroup

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ItemsControl
        └── AtomUI.Controls.Commons.AbstractRadioButtonGroup
              ├── implements IMotionAwareControl
              └── implements IFormItemAware
                    └── AtomUI.Desktop.Controls.RadioButtonGroup
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ItemsControl` | 子项集合管理、`ItemsSource` / `ItemTemplate` 数据绑定、`ItemsPanel` 布局面板 |
| `AbstractRadioButtonGroup` | 互斥选中管理 (`CheckedItem`)、`CheckedChanged` 事件、`Orientation` / `ItemSpacing` / `LineSpacing` 布局控制、容器生成与数据绑定适配、表单集成 |
| `AtomUI.Desktop.Controls.RadioButtonGroup` | 桌面端 Token 注册 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 动画开关，向下传递给子项 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/RadioButton/AbstractRadioButton.cs` | 跨平台基类，定义波纹接口和表单集成 |
| 基类 | `src/AtomUI.Controls/RadioButton/AbstractRadioButtonGroup.cs` | 单选框组跨平台基类，定义互斥逻辑 |
| 数据模型 | `src/AtomUI.Controls/RadioButton/RadioButtonOption.cs` | `IRadioButtonOption` 接口和默认实现 |
| 事件参数 | `src/AtomUI.Controls/RadioButton/RadioButtonGroupCheckedChangedEventArgs.cs` | 选中变化事件参数 |
| 指示器 | `src/AtomUI.Controls/RadioButton/RadioIndicator.cs` | 圆形选中指示器（内部控件，自绘制） |
| 控件类 | `src/AtomUI.Desktop.Controls/RadioButton/RadioButton.cs` | 桌面端 RadioButton |
| 控件类 | `src/AtomUI.Desktop.Controls/RadioButton/RadioButtonGroup.cs` | 桌面端 RadioButtonGroup |
| Token 定义 | `src/AtomUI.Desktop.Controls/RadioButton/RadioButtonToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/RadioButton/Themes/RadioButtonTheme.axaml` | RadioButton ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/RadioButton/Themes/RadioButtonGroupTheme.axaml` | RadioButtonGroup ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/RadioButton/Themes/RadioIndicatorTheme.axaml` | 指示器 ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/RadioButton/Themes/RadioButtonThemes.axaml` | ResourceDictionary 合并入口 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/RadioButtonShowCase.axaml` | 使用范例 |

---

## 模板结构

### RadioButton 模板

```
Border#Frame (Padding=0,1,0,1)
└── DockPanel (LastChildFill=True)
    ├── RadioIndicator#Indicator (DockPanel.Dock=Left, VerticalAlignment=Center)
    │   └── Panel
    │       └── WaveSpiritDecorator (PART_WaveSpirit)    ← 选中时触发圆形波纹
    │   └── [自绘制] 圆形外框 + 选中圆点
    └── ContentPresenter#ContentPresenter (VerticalAlignment=Center)
        └── 文本内容（仅当 Content 不为 null 时可见）
```

**RadioIndicator 的自绘制逻辑：**
- 使用 `DrawingContext.DrawEllipse` 绘制外圆（边框 + 背景）
- 选中时额外绘制内圆（`RadioInnerBackground`），大小由 `RadioDotEffectSize` 控制
- 圆点大小通过 `DoubleTransition` 实现缩放动画

### RadioButtonGroup 模板

```
Border#Frame
└── ItemsPresenter#PART_ItemsPresenter
    └── WrapPanel (默认 ItemsPanel)
        ├── RadioButton[0]
        ├── RadioButton[1]
        ├── ...
        └── RadioButton[n]
```

**布局面板特性：**
- 使用 `WrapPanel` 作为默认 ItemsPanel，支持自动换行
- `Orientation` 属性绑定到 WrapPanel，控制排列方向
- `ItemSpacing` 控制项目之间的间距（默认 `SpacingXS`）
- `LineSpacing` 控制换行后的行间距（默认 `SpacingXS`）

### 模板部件常量

| 部件名 | 控件类型 | 说明 |
|---|---|---|
| `Frame` | `Border` | RadioButton / RadioButtonGroup 根框架 |
| `Indicator` | `RadioIndicator` | 圆形选中指示器 |
| `ContentPresenter` | `ContentPresenter` | 文本内容区域 |
| `PART_ItemsPresenter` | `ItemsPresenter` | RadioButtonGroup 子项容器 |
| `PART_WaveSpirit` | `WaveSpiritDecorator` | 波纹动画装饰器（在 RadioIndicator 模板中） |
