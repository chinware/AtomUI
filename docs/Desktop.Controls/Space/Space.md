# Space 间距

## 概述

间距（Space）是布局辅助组件，用于设置子元素之间的统一间距。在日常 UI 开发中，最常见的需求之一就是为一组并列组件（按钮、标签、输入框等）添加统一的水平或垂直间距。如果逐一为每个元素设置 `Margin`，不仅繁琐，而且难以在设计变更时统一调整。Space 正是为解决这个问题而设计的——它将「间距」抽象为一个容器级别的概念，让开发者只需声明一个属性，即可统一管理所有子元素之间的距离。

AtomUI 的 `Space` 控件复刻了 [Ant Design 5.0 Space](https://ant.design/components/space-cn) 的设计规范。`Space` 同时承担了 Ant Design 中 `Space` 和 `Flex` 的部分职责（如自动换行、对齐），同时还提供了 Ant Design 的 `Space.Compact`（紧凑模式）对应的 `CompactSpace` 组件。

---

## 设计原理

### Ant Design 的间距设计哲学

Ant Design 对间距组件的定位是：**「设置组件之间的间距，避免组件紧贴在一起，拉开统一的空间」**。其核心设计理念包括：

1. **统一间距** — 避免为每个子元素手动设置 `margin`，通过 Space 统一管理间距。这不仅提高了开发效率，也保证了间距在整个应用中的一致性。
2. **预设尺寸** — Small / Middle / Large 三档间距对应 Token 系统中的 spacing 值，与全局设计语言保持一致。
3. **自动换行** — 当水平空间不足时自动折行，类似 CSS `flex-wrap: wrap`，无需手动处理响应式布局。
4. **紧凑模式（Compact）** — 将一组表单控件紧密排列，去除间距、共享边框，形成视觉上的整体。适用于搜索栏、组合输入等场景。

**Ant Design 间距预设值对照表：**

| 预设尺寸 | Ant Design 值 | AtomUI Token |
|---|---|---|
| Small（默认） | 8px | `SharedToken.SpacingXS` |
| Middle | 16px | `SharedToken.Spacing` |
| Large | 24px | `SharedToken.SpacingLG` |

### AtomUI 的扩展设计

AtomUI 在 Ant Design 规范的基础上，结合 Avalonia 布局系统的特点，做了以下扩展和增强：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **预设间距** | `SizeType`（Small / Middle / Large / Custom） | 自动从 Token 系统映射间距值，与全局设计语言保持一致 |
| **自定义间距** | `ItemSpacing` / `LineSpacing` | 精确控制项目间距和行间距，满足特殊布局需求 |
| **自动换行** | 内置 WrapPanel 布局算法（`MeasureOverride` / `ArrangeOverride`） | 水平空间不足时自动折行，无需额外容器 |
| **交叉轴对齐** | `ItemsAlignment`（Start / Center / End） | 控制每行子项在交叉轴方向上的对齐方式 |
| **分割线模板** | `SplitTemplate`（`ITemplate<Control>?`） | 在相邻子项之间自动插入分隔控件（如 Divider） |
| **固定子项尺寸** | `ItemWidth` / `ItemHeight` | 统一所有子项的宽度/高度，适用于网格类布局 |
| **紧凑组合** | `CompactSpace` 组件 | 对齐 Ant Design 的 `Space.Compact`，子控件紧密排列、共享边框 |
| **CompactSpace AddOn** | `CompactSpaceAddOn` 组件 | 在紧凑空间中添加附加内容区域（如货币符号、前缀/后缀） |
| **CompactSpace Filler** | `CompactSpaceFiller` 组件 | 在紧凑空间中占位填充剩余空间 |
| **子项尺寸控制** | `CompactSpace.ItemSize` 附加属性 | 精确控制紧凑空间中每个子项的宽度分配（Auto / 像素 / Star） |
| **键盘导航** | `INavigableContainer` 接口 | 支持方向键在子项之间导航 |

---

## 功能详解

### 布局算法

Space 采用自定义的 WrapPanel 式布局算法（在 `MeasureOverride` 和 `ArrangeOverride` 中实现）。其核心逻辑为：

1. **Measure 阶段**：沿主轴方向逐项累积子项尺寸 + 间距，当累积值超过约束宽度时换行，记录每行的 U 值（主轴尺寸）和 V 值（交叉轴尺寸）。
2. **Arrange 阶段**：逐行排列子项，根据 `ItemsAlignment` 在交叉轴方向调整位置。

布局使用内部的 `UVSize` 结构体来抽象水平/垂直方向，使同一套代码同时支持 `Horizontal` 和 `Vertical` 两种方向。

### 预设尺寸与自定义尺寸

Space 实现了 `ICustomizableSizeTypeAware` 接口，支持四种尺寸模式：

| 模式 | 行为 |
|---|---|
| `Small`（默认） | `ItemSpacing` 和 `LineSpacing` 自动设为 `GapSmallSize`（`SharedToken.SpacingXS`） |
| `Middle` | 自动设为 `GapMiddleSize`（`SharedToken.Spacing`） |
| `Large` | 自动设为 `GapLargeSize`（`SharedToken.SpacingLG`） |
| `Custom` | 用户通过 `ItemSpacing` / `LineSpacing` 手动指定具体数值 |

预设尺寸通过实例级样式（`ConfigureInstanceStyle`）实现，构造函数中动态生成三条样式规则，根据 `SizeType` 属性值自动切换间距。

### 分割线模板

当设置 `SplitTemplate` 时，Space 会在每两个相邻子项之间自动插入一个由模板创建的分隔控件。分割线的插入逻辑在 `HandleSplitTemplateChanged` 中统一处理——每次 `SplitTemplate` 变更或子项集合变更时，都会重建完整的可视树（Children + Split 交替排列）。

### 紧凑模式（CompactSpace）

`CompactSpace` 是 Ant Design `Space.Compact` 的 AtomUI 实现，用于将多个控件紧密排列、共享边框，形成视觉上的整体。典型场景包括：

- 搜索栏（Select + Input + Button）
- 组合输入（区号 + 电话号码）
- 工具栏按钮组

**CompactSpace 的核心机制：**

1. **子项包装**：每个子控件被包装在 `CompactSpaceItem` 中，通过 `ICompactSpaceAware` 接口通知子控件其在紧凑空间中的位置（First / Middle / Last）和方向。
2. **圆角裁剪**：根据子项位置自动裁剪圆角——首项保留起始圆角，中间项圆角为零，末项保留结束圆角。
3. **边框重叠消除**：通过负 `RenderTransform` 平移（基于 `PositionIndex × BorderThickness`）使相邻子项边框重叠，消除双边框效果。
4. **ZIndex 管理**：获得焦点或被指针进入的子项提升到活跃 ZIndex 层（1000），确保其边框不被相邻控件遮挡。
5. **Grid 布局**：内部使用 `Grid` 实现子项排列，支持通过 `CompactSpace.ItemSize` 附加属性精确控制每个子项的宽度分配。

**CompactSpace.ItemSize 尺寸类型：**

| 类型 | 语法 | 说明 |
|---|---|---|
| Auto | `Auto`（默认） | 子项按内容自然大小 |
| Pixel | `100` | 固定像素宽度 |
| Star | `2*` / `*` | 按比例分配剩余空间 |

### CompactSpaceAddOn

`CompactSpaceAddOn` 用于在紧凑空间中添加装饰性内容区域（如货币符号 `$`、单位后缀等）。它实现了 `ISizeTypeAware`、`IInputControlStatusAware`、`IInputControlStyleVariantAware` 等接口，能够自动适配尺寸、状态和样式变体。

### CompactSpaceFiller

`CompactSpaceFiller` 是一个空占位控件，用于在紧凑空间中填充剩余空间。配合 `CompactSpace.ItemSize="*"` 使用，可将控件组推向一侧。

**约束**：紧凑空间中最多只能有一个 `CompactSpaceFiller`，且必须是最后一个子项，否则抛出 `InvalidSpaceFillerUsageException`。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本间距 | ✅ `<Space>` | ✅ `<atom:Space>` | ✅ 完全对齐 |
| 方向 `direction` | ✅ `horizontal` / `vertical` | ✅ `Orientation` | ✅ 完全对齐 |
| 尺寸 `size` | ✅ `small` / `middle` / `large` / 数值 | ✅ `SizeType` + `ItemSpacing` | ✅ 完全对齐 |
| 对齐 `align` | ✅ `start` / `center` / `end` / `baseline` | ✅ `ItemsAlignment`（无 baseline） | ⚠️ 缺少 `baseline` 对齐 |
| 自动换行 `wrap` | ✅ 布尔属性 | ✅ 自动换行（内置，始终启用） | ✅ 完全对齐 |
| 分割线 `split` | ✅ `ReactNode` | ✅ `SplitTemplate`（`ITemplate<Control>`） | ✅ 完全对齐 |
| Space.Compact | ✅ `<Space.Compact>` | ✅ `<atom:CompactSpace>` | ✅ 完全对齐 |
| Compact 方向 | ✅ `direction` | ✅ `Orientation` | ✅ 完全对齐 |
| Compact 尺寸 | ✅ `size` | ✅ `SizeType` | ✅ 完全对齐 |
| Compact 子项尺寸分配 | ❌ 无 | ✅ `CompactSpace.ItemSize` 附加属性 | 🆕 AtomUI 扩展 |
| CompactSpaceAddOn | ❌ 无独立组件 | ✅ `CompactSpaceAddOn` | 🆕 AtomUI 扩展 |
| CompactSpaceFiller | ❌ 无 | ✅ `CompactSpaceFiller` | 🆕 AtomUI 扩展 |
| `classNames` / `styles` | ✅ 自定义样式 | ✅ Avalonia Style / ControlTheme | ✅ 平台适配 |

---

## 继承关系

### Space

```
Avalonia.Controls.Control
  └── AtomUI.Desktop.Controls.Space
        ├── implements IChildIndexProvider        ← 子控件索引管理
        ├── implements ICustomizableSizeTypeAware ← 预设 + 自定义尺寸
        └── implements INavigableContainer       ← 键盘导航支持
```

Space 直接继承 `Control` 而非 `Panel` 或 `StackPanel`，这是因为它需要自定义的 WrapPanel 式布局算法，同时管理分割线模板的动态插入。

### CompactSpace

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.CompactSpace
        ├── implements ISizeTypeAware          ← 三种预设尺寸
        ├── implements IChildIndexProvider     ← 子控件索引管理
        └── implements INavigableContainer    ← 键盘导航支持
```

CompactSpace 继承 `TemplatedControl`，使用 ControlTheme 定义模板（内部为 `Grid`），通过模板应用后动态管理 `CompactSpaceItem` 包装。

### CompactSpaceAddOn

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.CompactSpaceAddOn
        ├── implements ISizeTypeAware                  ← 三种预设尺寸
        ├── implements ICompactSpaceAware (internal)   ← 紧凑空间位置感知
        ├── implements IInputControlStatusAware        ← 输入控件状态感知
        └── implements IInputControlStyleVariantAware  ← 样式变体感知
```

### 相关类型

| 类型 | 可见性 | 说明 |
|---|---|---|
| `Space` | public | 间距容器主控件 |
| `CompactSpace` | public | 紧凑模式容器 |
| `CompactSpaceItem` | internal | 紧凑模式子项包装器 |
| `CompactSpaceAddOn` | public | 紧凑模式附加内容区域 |
| `CompactSpaceFiller` | public | 紧凑模式空间填充器 |
| `CompactSpaceSize` | public | 子项尺寸值类型（Auto / Pixel / Star） |
| `CompactSpaceUnitType` | public | 尺寸单位枚举 |
| `SpaceItemsAlignment` | public | 交叉轴对齐枚举（Start / Center / End） |
| `ICompactSpaceAware` | internal | 紧凑空间位置感知接口 |
| `CompactSpaceAwareControlProperty` | internal | 紧凑空间感知共享属性 |
| `InvalidSpaceFillerUsageException` | public | Filler 使用不当异常 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Space/Space.cs` | Space 主控件实现 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Space/SpaceToken.cs` | 组件级 Design Token |
| 紧凑模式 | `src/AtomUI.Desktop.Controls/Space/CompactSpace.cs` | CompactSpace 容器 |
| 紧凑子项 | `src/AtomUI.Desktop.Controls/Space/CompactSpaceItem.cs` | CompactSpaceItem 包装器 |
| 紧凑 AddOn | `src/AtomUI.Desktop.Controls/Space/CompactSpaceAddOn.cs` | CompactSpaceAddOn 附加区域 |
| 紧凑填充 | `src/AtomUI.Desktop.Controls/Space/CompactSpaceFiller.cs` | CompactSpaceFiller 占位控件 |
| 尺寸类型 | `src/AtomUI.Desktop.Controls/Space/CompactSpaceSize.cs` | CompactSpaceSize 值类型 |
| 感知接口 | `src/AtomUI.Desktop.Controls/Space/ICompactSpaceAware.cs` | ICompactSpaceAware 接口 |
| 异常 | `src/AtomUI.Desktop.Controls/Space/InvalidSpaceFillerUsageException.cs` | Filler 异常 |
| 模板常量 | `src/AtomUI.Desktop.Controls/Space/Themes/CompactSpaceThemeConstants.cs` | 模板部件名称常量 |
| CompactSpace 主题 | `src/AtomUI.Desktop.Controls/Space/Themes/CompactSpaceTheme.axaml` | CompactSpace ControlTheme |
| AddOn 主题 | `src/AtomUI.Desktop.Controls/Space/Themes/CompactSpaceAddOnTheme.axaml` | CompactSpaceAddOn ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/Space/Themes/SpaceThemes.axaml` | 合并资源字典 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Layout/SpaceShowCase.axaml` | 使用范例 |

---

## 模板结构

### Space

Space 直接继承 `Control`，不使用 ControlTemplate。子控件直接添加到 `VisualChildren` 中，布局通过重写 `MeasureOverride` / `ArrangeOverride` 实现。当设置 `SplitTemplate` 时，分隔控件会在子项之间交替插入到可视树中。

### CompactSpace

CompactSpace 使用 ControlTemplate，结构非常简洁：

```
Grid (PART_ContentLayout, UseLayoutRounding=False)
  ├── CompactSpaceItem → Child1（ICompactSpaceAware）
  ├── CompactSpaceItem → Child2（ICompactSpaceAware）
  ├── ...
  └── CompactSpaceItem → ChildN（ICompactSpaceAware）
```

Grid 根据 `Orientation` 动态生成 `ColumnDefinitions`（水平）或 `RowDefinitions`（垂直），每个子项的宽度/高度由 `CompactSpace.ItemSize` 附加属性控制。

### CompactSpaceAddOn

```
ContentPresenter
  ├── Content（TemplateBinding）
  ├── CornerRadius（TemplateBinding EffectiveCornerRadius）← 根据紧凑空间位置自动计算
  ├── BorderBrush / BorderThickness / Background
  └── Padding
```

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `CompactSpaceThemeConstants.ContentLayoutPart` | `"PART_ContentLayout"` | CompactSpace 内容布局 Grid |
