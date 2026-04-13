# Rate 评分

## 概述

评分（Rate）用于对事物进行快速的评级操作，默认以星星图标展示。支持整星和半星两种精度，支持自定义图标/字符，支持清除评分、只读模式和键盘操作。评分控件广泛用于电商商品评价、服务满意度调查、内容质量打分等场景。

AtomUI 的 `Rate` 控件复刻了 [Ant Design 5.0 Rate](https://ant.design/components/rate-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的评分设计哲学

Ant Design 对评分的定位是：**「对事物进行快速的评级操作」**。其核心原则包括：

1. **快速评级** — 通过鼠标点击/悬浮实现直觉化的评级操作，无需额外操作步骤
2. **半星精度** — 通过 `allowHalf` 支持更精细的 0.5 步长评级，适用于需要更高分辨率评分的场景
3. **可清除** — 再次点击已选星级可清除评分，回到初始状态
4. **自定义字符** — 不限于星星，可替换为心形、字母、文字等任意字符，适应不同业务语境
5. **提示文案** — 每颗星可对应不同的描述文案（如 "terrible"、"bad"、"normal"、"good"、"wonderful"），增强评分的语义明确性

### Avalonia 基础能力

与其他 AtomUI 控件不同，Rate 没有继承自 Avalonia 的任何具体控件——它直接继承自 `TemplatedControl`（通过 `AbstractRate` 中间层）。这是因为 Avalonia 没有提供原生的评分控件，Rate 是 AtomUI 完全自研的控件。

```
Control → TemplatedControl → AbstractRate → Rate
```

Rate 控件内部组合使用了 `ItemsControl`（`RateItemsControl`）来管理多个 `RateItem` 子控件，每个 `RateItem` 代表一颗星。这种组合模式使得星星的数量可以动态变化，且每颗星独立管理自己的选中/悬浮/半选状态。

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **半星评分** | `IsAllowHalf` 属性 + 裁剪区域计算 | 对齐 Ant Design `allowHalf`，通过检测鼠标在星星左半/右半的位置实现精细评级 |
| **自定义字符** | `Character` 属性（支持 `Icon` / `char` / `string`） | 支持 PathIcon 图标、单字符或文字替换默认星星，适应不同业务场景 |
| **可清除** | `IsAllowClear` 属性 | 再次点击相同值可归零，默认启用 |
| **三种尺寸** | `ISizeTypeAware` + `SizeType` | Large/Middle/Small 三种尺寸，通过 Token 映射到不同的星星大小 |
| **悬浮缩放** | `StarHoverScale` Token + `TransformOperationsTransition` | 悬浮时星星放大至 120%，带过渡动画 |
| **提示文案** | `ToolTips` 属性 | 为每颗星设置 ToolTip，鼠标悬浮时显示对应文案 |
| **键盘操作** | `IsKeyboardEnabled` + 方向键 | 左/右方向键调整评分，支持半星步长和整星步长 |
| **悬浮预览** | 内部 `EffectiveValue` + 全局指针监听 | 鼠标移入时实时预览评分效果，移出后恢复到实际值 |
| **表单集成** | `IFormItemAware` 接口 | 可参与 FormItem 验证流程，表单值类型为 `double` |
| **Design Token** | `RateToken` + `RegisterTokenResourceScope` | 星星颜色、大小、缩放比例全部从 Token 派生 |

---

## 功能详解

### 星星字符渲染

Rate 控件的每颗星由内部 `RateItem` 控件渲染。每个 RateItem 通过 `VisualBrush` 技术将字符（图标或文字）渲染为两层：
- **背景层**（`CharacterBgBrush`）— 使用 `StarBgColor` 颜色渲染，始终可见
- **前景层**（`CharacterBrush`）— 使用 `StarColor` 颜色渲染，通过裁剪区域控制可见范围

支持三种字符类型：
- **Icon 图标**（默认 `StarFilled`）— 通过 `FillBrush` / `StrokeBrush` 着色
- **单字符 `char`** — 通过 `RateCharacter` 内部控件自绘制
- **字符串 `string`** — 取首字符，同样通过 `RateCharacter` 渲染

### 半星评分机制

当 `IsAllowHalf = true` 时，每颗星被水平等分为左右两半：
- 鼠标位于星星**左半区域**：评分为 `index + 0.5`（半星）
- 鼠标位于星星**右半区域**：评分为 `index + 1`（整星）

半星状态通过 `RectangleGeometry` 裁剪实现——将前景层裁剪至星星宽度的一半，仅显示左半部分。

### 悬浮预览与点击确认

Rate 采用「悬浮预览 + 点击确认」的交互模型：
1. **悬浮预览**：鼠标在星星上移动时，内部 `EffectiveValue` 实时更新，触发 `HoverValueChanged` 事件，星星即时反映预览效果
2. **移出恢复**：鼠标移出 Rate 区域时，`EffectiveValue` 恢复为 `Value`（已确认的值）
3. **点击确认**：鼠标在星星上点击释放时，如果位置未变化，将 `EffectiveValue` 写入 `Value`，触发 `ValueChanged` 事件

全局指针监听（通过 `IInputManager.Process`）确保即使鼠标快速移出也能正确恢复状态。

### 清除评分

当 `IsAllowClear = true`（默认）时：
- 再次点击当前已选中的星级，`Value` 被清零
- 半星模式下需精确点击同一半星才会清除
- 整星模式下点击同一星即清除

### 键盘操作

当 `IsKeyboardEnabled = true`（默认）且 Rate 获得焦点时：
- **← 左方向键**：减少评分（半星模式减 0.5，整星模式减 1）
- **→ 右方向键**：增加评分（半星模式加 0.5，整星模式加 1）
- 评分值被限制在 `[0, Count]` 范围内
- 键盘操作时会在当前焦点星上显示虚线边框（`IsFocusStartItem`），提供视觉反馈

### 悬浮缩放动画

每颗 RateItem 在悬浮时触发缩放动画：
- 悬浮时以中心点为原点缩放至 `StarHoverScale`（默认 1.2 倍）
- 通过 `TransformOperationsTransition` 实现平滑过渡
- 动画受 `IsMotionEnabled` 控制，可全局禁用

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本评分 | ✅ `<Rate />` | ✅ `<atom:Rate />` | ✅ 完全对齐 |
| 半星 `allowHalf` | ✅ 布尔 | ✅ `IsAllowHalf` | ✅ 完全对齐 |
| 可清除 `allowClear` | ✅ 布尔 | ✅ `IsAllowClear` | ✅ 完全对齐 |
| 自定义字符 `character` | ✅ ReactNode / 函数 | ✅ `Character`（Icon / char / string） | ⚠️ 不支持按 index 动态字符函数 |
| 总数 `count` | ✅ 数字 | ✅ `Count` | ✅ 完全对齐 |
| 默认值 `defaultValue` | ✅ 数字 | ✅ `DefaultValue` | ✅ 完全对齐 |
| 当前值 `value` | ✅ 受控 | ✅ `Value` | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ 布尔 | ✅ `IsEnabled` | ✅ 完全对齐 |
| 提示 `tooltips` | ✅ 字符串数组 | ✅ `ToolTips` | ✅ 完全对齐 |
| `onChange` | ✅ 回调 | ✅ `ValueChanged` 事件 | ✅ 完全对齐 |
| `onHoverChange` | ✅ 回调 | ✅ `HoverValueChanged` 事件 | ✅ 完全对齐 |
| 键盘支持 | ✅ 方向键 | ✅ `IsKeyboardEnabled` + 方向键 | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Controls.Commons.AbstractRate
        ├── implements IMotionAwareControl
        ├── implements ISizeTypeAware
        └── implements IFormItemAware
              └── AtomUI.Desktop.Controls.Rate
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础设施、`OnApplyTemplate`、样式系统 |
| `AbstractRate` | 全部评分核心逻辑：属性定义、星星数量管理、悬浮/点击/键盘交互、`EffectiveValue` 预览机制、半星计算、清除逻辑、ToolTip 配置、表单集成 |
| `AtomUI.Desktop.Controls.Rate` | 桌面端 Token 注册 (`RateToken.ScopeProvider`) |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 过渡动画开关（控制悬浮缩放动画） |
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 三种尺寸切换（Small / Middle / Large） |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |

---

## 内部控件架构

Rate 控件由以下内部控件组成（均为 `internal`，不对外暴露）：

| 内部控件 | 基类 | 作用 |
|---|---|---|
| `RateItemsControl` | `ItemsControl` | 管理 RateItem 子项集合，传递属性绑定 |
| `RateItem` | `TemplatedControl` | 单颗星的渲染与交互，管理选中状态、悬浮缩放、裁剪 |
| `RateCharacter` | `Control` | 文字字符的自绘制控件（当 `Character` 为 `char`/`string` 时使用） |

**RateItem 的三种选中状态：**

| 枚举值 | 说明 |
|---|---|
| `RateItemSelectedState.None` | 未选中，仅显示背景层 |
| `RateItemSelectedState.FullSelected` | 整星选中，前景层完全覆盖 |
| `RateItemSelectedState.HalfSelected` | 半星选中，前景层裁剪至左半 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/Rate/AbstractRate.cs` | 跨平台基类（547 行核心逻辑） |
| 内部控件 | `src/AtomUI.Controls/Rate/RateItem.cs` | 单颗星控件 |
| 内部控件 | `src/AtomUI.Controls/Rate/RateItemsControl.cs` | 星星集合容器 |
| 内部控件 | `src/AtomUI.Controls/Rate/RateCharacter.cs` | 文字字符自绘制控件 |
| 事件参数 | `src/AtomUI.Controls/Rate/RateValueChangedEventArgs.cs` | 值变化事件参数 |
| 控件类 | `src/AtomUI.Desktop.Controls/Rate/Rate.cs` | 桌面端 Rate |
| Token 定义 | `src/AtomUI.Desktop.Controls/Rate/RateToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Rate/Themes/RateTheme.axaml` | Rate ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Rate/Themes/RateItemTheme.axaml` | RateItem ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Rate/Themes/RateItemsControlTheme.axaml` | RateItemsControl ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/Rate/Themes/RateThemes.axaml` | ResourceDictionary 合并入口 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/RateShowCase.axaml` | 使用范例 |

---

## 模板结构

### Rate 模板

```
Border#Frame
└── RateItemsControl (PART_RateItems)
    └── StackPanel (Orientation=Horizontal, Spacing=SpacingXS)
        ├── RateItem[0]  ← 每颗星一个内部控件
        ├── RateItem[1]
        ├── ...
        └── RateItem[Count-1]
```

### RateItem 模板

```
Border (Background=Transparent)
└── Panel
    ├── DashedBorder (BorderThickness=0.75, StrokeDashArray=4,2)  ← 键盘焦点虚线框（IsFocusStartItem 时可见）
    ├── Rectangle (Fill=CharacterBgBrush)                         ← 背景层（未选中颜色）
    └── Rectangle#ActiveItem (Clip=StarClip)                      ← 前景层（选中颜色，半星时被裁剪）
```

**RateItem 模板分层设计：**
- **虚线焦点框**：使用 `DashedBorder`，仅在键盘操作时（`IsFocusStartItem = true`）显示，提供无障碍视觉反馈
- **背景层**：始终显示，使用 `StarBgColor` 着色的字符/图标，作为未选中状态的视觉
- **前景层**：选中时显示，使用 `StarColor` 着色的字符/图标。半星模式下通过 `StarClip`（`RectangleGeometry`）裁剪至左半

### 模板部件常量

| 部件名 | 控件类型 | 说明 |
|---|---|---|
| `PART_RateItems` | `RateItemsControl` (内部 `ItemsControl`) | 星星集合容器 |
| `Frame` | `Border` | Rate 根框架 |
| `ActiveItem` | `Rectangle` | RateItem 中的前景层（选中色） |
| `PART_ItemsPresenter` | `ItemsPresenter` | RateItemsControl 中的子项呈现器 |
| `ItemsLayout` | `StackPanel` | 星星水平排列容器 |
| `PART_WaveSpirit` | — | Rate 不使用波纹效果 |

