# OptionButtonGroup 选项按钮组

## 概述

选项按钮组（OptionButtonGroup）是以按钮样式呈现的单选组件，用于在多个互斥选项中选择一个。它将传统单选按钮的逻辑与按钮的视觉形态相结合，适用于数量有限且语义明确的选项集合，如切换视图模式、选择排序方式、切换布局方向等。

AtomUI 的 `OptionButtonGroup` 控件对应 [Ant Design 5.0 Radio.Group](https://ant.design/components/radio-cn) 中 `optionType="button"` 的变体（即按钮样式的单选组），在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的按钮单选设计哲学

Ant Design 的 Radio.Group 在 `optionType="button"` 模式下，将每个单选选项渲染为按钮外观。与普通单选按钮（圆形勾选框）相比，按钮样式的单选组更加紧凑、视觉权重更高，适合放置在工具栏或控制面板中。

**两种按钮样式**：

| 样式 | 设计意图 | 视觉表现 |
|---|---|---|
| 🔲 **Outline（轮廓）** | 默认样式，选中项通过边框颜色和文字颜色区分 | 选中项边框变为主色（`ColorPrimary`），文字变为主色 |
| 🔳 **Solid（实色）** | 更强调选中状态，选中项有实色填充背景 | 选中项背景变为主色填充，文字变为白色 |

**三种尺寸**（通过 `SizeType` 控制）：

| 尺寸 | 高度 | 说明 |
|---|---|---|
| `Large` | 40px | 大号，醒目场景 |
| `Middle` | 32px | 中号（默认），常规场景 |
| `Small` | 24px | 小号，紧凑布局 |

### Avalonia 基础能力

AtomUI 的 OptionButtonGroup 体系基于 Avalonia 的以下基础能力构建：

- **OptionButton**（`AbstractOptionButton`）继承自 `Avalonia.Controls.RadioButton`，获得单选互斥行为、`IsChecked` 状态管理、`Content` 内容呈现等能力
- **OptionButtonGroup**（`AbstractOptionButtonGroup`）继承自 `SelectingItemsControl`，获得 `ItemsSource` 数据驱动、`SelectedItem` / `SelectedIndex` 选中管理、容器化子项（`PrepareContainerForItemOverride`）等能力

### AtomUI 的实现设计

AtomUI 的 OptionButtonGroup 采用**跨平台两层继承模型**：

| 设计能力 | 实现方式 | 设计动机 |
|---|---|---|
| **两种按钮样式** | `OptionButtonStyle` 枚举 + 伪类驱动样式 | 对齐 Ant Design 的 `buttonStyle`（`solid` / `outline`） |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **自动位置感知** | `OptionButtonPositionTrait` 枚举 | 自动计算每个按钮的位置（First/Middle/Last/OnlyOne），裁剪圆角 |
| **图标支持** | `Icon` 属性（`PathIcon` 类型） | 按钮支持文字 + 图标组合 |
| **选中边框绘制（Outline）** | 自定义 `Render` 方法 | Outline 模式下选中项绘制主色边框覆盖 |
| **分隔线绘制** | 自定义 `Render` 方法 | 按钮之间绘制细分隔线 |
| **点击波纹** | `IWaveSpiritAwareControl` + `WaveSpiritDecorator` | 复刻 Ant Design 的 Wave 点击涟漪效果 |
| **过渡动画** | `IsMotionEnabled` + `Transitions` | 背景色/前景色/边框色平滑过渡 |
| **表单集成** | `IFormItemAware` 接口 | 可参与 FormItem 验证流程 |
| **Design Token** | `OptionButtonToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 按钮样式（OptionButtonStyle）

通过 `ButtonStyle` 属性设置，影响选中项的视觉表现：

- **Outline**（默认）：选中项使用主色边框 + 主色文字，背景保持透明。Group 容器在选中项位置绘制一个覆盖边框
- **Solid**：选中项使用主色背景填充 + 白色文字。Solid 模式下相邻选中项之间不绘制分隔线

### 自动位置感知与圆角裁剪

OptionButtonGroup 自动为每个子按钮分配 `GroupPositionTrait`：
- `First`：第一个按钮，保留左侧圆角，右侧圆角为 0
- `Middle`：中间按钮，四个圆角均为 0
- `Last`：最后一个按钮，保留右侧圆角，左侧圆角为 0
- `OnlyOne`：唯一按钮，保留全部圆角

### 分隔线与选中边框

Group 容器的 `Render` 方法负责绘制：
1. **外部统一边框**：整个组的外围边框（`BorderBrush` + `BorderThickness`）
2. **分隔线**：每两个相邻按钮之间绘制垂直分隔线
3. **选中边框（Outline 模式）**：在选中按钮的位置绘制 `SelectedOptionBorderColor` 覆盖边框

### 数据驱动

OptionButtonGroup 支持两种使用方式：
- **声明式子项**：直接在 AXAML 中添加 `OptionButton` 子控件
- **数据绑定**：通过 `ItemsSource` 绑定 `OptionButtonData` 集合，自动生成按钮容器

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 按钮样式单选 `optionType="button"` | ✅ | ✅ `OptionButtonGroup` + `OptionButton` | ✅ 完全对齐 |
| Outline 样式 `buttonStyle="outline"` | ✅ | ✅ `ButtonStyle="Outline"` | ✅ 完全对齐 |
| Solid 样式 `buttonStyle="solid"` | ✅ | ✅ `ButtonStyle="Solid"` | ✅ 完全对齐 |
| 三种尺寸 `size` | ✅ `large / middle / small` | ✅ `SizeType` | ✅ 完全对齐 |
| 禁用单个选项 | ✅ `disabled` | ✅ `IsEnabled` | ✅ 完全对齐 |
| 禁用整个组 | ✅ `disabled` | ✅ `IsEnabled` | ✅ 完全对齐 |
| 选中变更回调 `onChange` | ✅ 函数 | ✅ `OptionCheckedChanged` 事件 | ✅ 完全对齐 |
| 图标支持 | ✅ `icon` (5.x) | ✅ `Icon` 属性 | ✅ 完全对齐 |
| 默认选中 `defaultValue` | ✅ | ✅ `OptionButton.IsChecked="True"` | ✅ 完全对齐 |
| 数据驱动 `options` | ✅ 数组 | ✅ `ItemsSource` + `OptionButtonData` | ✅ 完全对齐 |

---

## 继承关系

### 跨平台两层模型

```
AtomUI.Controls (基础层，设备无关)                  AtomUI.Desktop.Controls (平台层)
──────────────────────────────────                  ──────────────────────────────────
AbstractOptionButton : RadioButton         ───►     OptionButton : AbstractOptionButton
AbstractOptionButtonGroup : SelectingItemsControl ─► OptionButtonGroup : AbstractOptionButtonGroup
```

### AbstractOptionButton（基础层）

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Primitives.ToggleButton
              └── Avalonia.Controls.RadioButton
                    └── AtomUI.Controls.Commons.AbstractOptionButton
                          └── AtomUI.Desktop.Controls.OptionButton
```

### AbstractOptionButtonGroup（基础层）

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ItemsControl
        └── Avalonia.Controls.Primitives.SelectingItemsControl
              └── AtomUI.Controls.Commons.AbstractOptionButtonGroup
                    ├── implements ISizeTypeAware
                    ├── implements IWaveSpiritAwareControl
                    ├── implements IFormItemAware
                    └── AtomUI.Desktop.Controls.OptionButtonGroup
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `RadioButton` | 单选互斥行为、`IsChecked` 状态、`Content` 内容、`GroupName` 分组 |
| `SelectingItemsControl` | `ItemsSource` 数据驱动、`SelectedItem` / `SelectedIndex`、容器化管理 |
| `AbstractOptionButton` | 位置感知圆角裁剪、`Icon` 图标支持、WaveSpirit 波纹、过渡动画、自定义渲染（边框绘制） |
| `AbstractOptionButtonGroup` | 按钮样式（Outline/Solid）、尺寸传播、分隔线绘制、选中边框绘制、位置分配、`OptionCheckedChanged` 事件、表单集成 |
| `OptionButton` / `OptionButtonGroup` | Desktop Token 注册（`OptionButtonToken.ScopeProvider`） |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IWaveSpiritAwareControl` | `AtomUI.Controls.Shared` | 支持点击涟漪（Wave）动画效果 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |

---

## 源码位置

### 基础层（AtomUI.Controls）

| 文件路径 | 说明 |
|---|---|
| `src/AtomUI.Controls/OptionButtonGroup/AbstractOptionButton.cs` | 设备无关的选项按钮基类 |
| `src/AtomUI.Controls/OptionButtonGroup/AbstractOptionButtonGroup.cs` | 设备无关的选项按钮组基类 |
| `src/AtomUI.Controls/OptionButtonGroup/OptionButtonGroupEnums.cs` | `OptionButtonStyle` / `OptionButtonPositionTrait` 枚举 |
| `src/AtomUI.Controls/OptionButtonGroup/OptionButtonData.cs` | 数据驱动模型 |
| `src/AtomUI.Controls/OptionButtonGroup/OptionButtonPointerEventArgs.cs` | 指针事件参数 |
| `src/AtomUI.Controls/OptionButtonGroup/OptionCheckedChangedEventArgs.cs` | 选中变更事件参数 |

### 平台层（AtomUI.Desktop.Controls）

| 文件路径 | 说明 |
|---|---|
| `src/AtomUI.Desktop.Controls/OptionButtonGroup/OptionButton.cs` | 桌面端 OptionButton |
| `src/AtomUI.Desktop.Controls/OptionButtonGroup/OptionButtonGroup.cs` | 桌面端 OptionButtonGroup |
| `src/AtomUI.Desktop.Controls/OptionButtonGroup/OptionButtonToken.cs` | 组件级 Design Token |
| `src/AtomUI.Desktop.Controls/OptionButtonGroup/Themes/OptionButtonTheme.axaml` | OptionButton 主题 |
| `src/AtomUI.Desktop.Controls/OptionButtonGroup/Themes/OptionButtonGroupTheme.axaml` | OptionButtonGroup 主题 |
| `src/AtomUI.Desktop.Controls/OptionButtonGroup/Themes/OptionButtonBoxThemes.axaml` | 主题汇总注册 |

### Gallery 示例

| 文件路径 | 说明 |
|---|---|
| `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/RadioButtonShowCase.axaml` | RadioButton 与 OptionButton 综合范例 |

---

## 模板结构

### OptionButton 模板

```
Panel
├── WaveSpiritDecorator (PART_WaveSpirit)     ← 点击波纹效果层
└── DockPanel#ContentLayout                    ← 内容布局
      ├── IconPresenter#IconPresenter          ← 图标（DockPanel.Dock="Left"，可选）
      └── TextBlock                            ← 文本内容
```

### OptionButtonGroup 模板

```
Border#Frame                                    ← 外围容器（边框+圆角）
  └── ItemsPresenter (PART_ItemsPresenter)     ← 子项呈现器
        └── StackPanel (Horizontal)            ← 默认水平排列面板
              ├── OptionButton                  ← 选项按钮 1
              ├── OptionButton                  ← 选项按钮 2
              └── ...
```

**布局设计要点：**
- **自定义渲染**：OptionButtonGroup 的 `Render` 方法负责绘制外围边框、分隔线和选中边框覆盖，而非依赖模板中的多层 Border
- **圆角自动裁剪**：每个 OptionButton 根据在组中的位置自动计算圆角（仅首尾按钮保留外侧圆角）
- **分隔线绘制**：使用 `DrawLine` 在相邻按钮间绘制像素对齐的分隔线（`EdgeMode.Aliased`）
