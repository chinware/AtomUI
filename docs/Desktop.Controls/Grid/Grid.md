# Grid 栅格

## 概述

栅格系统（Grid）是页面布局中最基础的结构化工具，用于将页面水平方向划分为等分列，使内容能够在不同屏幕尺寸下按比例排列。AtomUI 的栅格系统完整复刻了 [Ant Design 5.0 Grid](https://ant.design/components/grid-cn) 的设计规范，采用 **24 等分栅格体系**，通过 `Row`（行）和 `Col`（列）两个组件协作实现灵活的响应式布局。

栅格系统是一个纯布局控件，不包含任何视觉样式或 Design Token——它只关注空间分配和子元素排列。

---

## 设计原理

### Ant Design 的栅格设计哲学

Ant Design 的栅格系统遵循以下核心设计原则：

**24 等分栅格**：

将容器宽度等分为 24 份，每个 `Col` 通过 `Span` 属性指定占据的份数。24 栅格体系能被 2、3、4、6、8、12 整除，覆盖绝大多数布局场景。

**响应式断点**：

| 断点 | 缩写 | 宽度范围 | 典型设备 |
|---|---|---|---|
| ExtraSmall | `xs` | < 576px | 手机竖屏 |
| Small | `sm` | ≥ 576px | 手机横屏 |
| Medium | `md` | ≥ 768px | 平板竖屏 |
| Large | `lg` | ≥ 992px | 平板横屏 / 小桌面 |
| ExtraLarge | `xl` | ≥ 1200px | 桌面 |
| ExtraExtraLarge | `xxl` | ≥ 1600px | 大桌面 |

**核心布局能力**：

| 特性 | 说明 |
|---|---|
| 🔢 **Span** | 列占据的栅格数（0-24） |
| ↔️ **Gutter** | 列之间的间距（水平 + 垂直），支持响应式 |
| ➡️ **Offset** | 列向右偏移的栅格数 |
| 🔄 **Push / Pull** | 列的视觉位移（不影响布局流） |
| 📐 **Order** | 列的排列顺序（类似 CSS `order`） |
| 📏 **Justify** | 主轴对齐方式（start / center / end / space-between / space-around / space-evenly） |
| 📐 **Align** | 交叉轴对齐方式（top / middle / bottom / stretch） |
| 🔁 **Wrap** | 是否允许换行 |
| 📱 **响应式** | 通过 `Xs` / `Sm` / `Md` / `Lg` / `Xl` / `Xxl` 属性按断点覆盖所有布局参数 |

### Avalonia 基础能力

AtomUI 的 `Row` 继承自 Avalonia 的 `Panel`，`Col` 继承自 `ContentControl`。

**Avalonia Panel 的核心职责**：

`Panel` 是 Avalonia 中最基础的布局容器，提供 `MeasureOverride` 和 `ArrangeOverride` 两个虚方法用于自定义布局逻辑。`Row` 利用这一机制实现了 24 栅格的测量和排列算法。

```
Control → Panel → Row
Control → TemplatedControl → ContentControl → Col
```

### AtomUI 的扩展设计

AtomUI 在 Avalonia 基础控件之上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **24 栅格体系** | `Row` 内置 `GridColumns = 24` 常量 | 对齐 Ant Design 的 24 等分体系 |
| **水平 + 垂直间距** | `GridGutter` 类型（支持 TypeConverter） | 对齐 Ant Design 的 `gutter` 属性 |
| **响应式间距** | `GridGutterInfo` 按断点存储不同间距值 | 不同屏幕尺寸使用不同间距 |
| **主轴对齐** | `RowJustify` 枚举（6 种模式） | 对齐 CSS Flexbox 的 `justify-content` |
| **交叉轴对齐** | `RowAlign` 枚举（4 种模式） | 对齐 CSS Flexbox 的 `align-items` |
| **响应式 Span** | `GridColSpanInfo`（按断点存储不同 Span 值） | Span 随屏幕尺寸变化 |
| **响应式列覆盖** | `GridColSize` + `ColInfo` MarkupExtension | 按断点覆盖 Span / Offset / Order / Push / Pull |
| **断点感知** | `IMediaBreakAwareControl` 接口 | 自动响应窗口尺寸变化 |
| **TypeConverter** | 所有核心类型都支持 AXAML 字符串解析 | 简化 AXAML 写法 |

---

## 功能详解

### 24 栅格体系

`Row` 将可用宽度等分为 24 份，每个 `Col` 通过 `Span` 属性指定占据的份数。所有 `Col` 的 `Span` 之和建议不超过 24，超出时自动换行（当 `Wrap="True"`）。

```
|  1  |  2  |  3  |  4  |  5  |  6  |  7  |  8  |  9  | 10  | 11  | 12  | 13  | 14  | 15  | 16  | 17  | 18  | 19  | 20  | 21  | 22  | 23  | 24  |
|◄─────── Span=6 ────────►|◄─────── Span=6 ────────►|◄─────── Span=6 ────────►|◄─────── Span=6 ────────►|
```

### Gutter（间距）

`Gutter` 属性支持多种写法：

| AXAML 写法 | 含义 |
|---|---|
| `Gutter="16"` | 水平间距 16px，垂直间距 0 |
| `Gutter="16,24"` | 水平间距 16px，垂直间距 24px |
| `Gutter="xs:8,sm:16,md:24,lg:32"` | 响应式水平间距 |
| `Gutter="xs:8,sm:16;xs:16,sm:24"` | 响应式水平间距 + 响应式垂直间距（用 `;` 分隔） |

间距通过在每个 `Col` 两侧添加等量的 `padding` 实现，不会影响 `Col` 的栅格宽度计算。

### Offset（偏移）

`Offset` 使列向右偏移指定的栅格数。偏移量计算为 `ColumnWidth × Offset`，占据的空间会影响后续列的位置。

### Push / Pull（视觉位移）

`Push` 和 `Pull` 改变列的视觉位置但不影响布局流。类似 CSS 的 `position: relative` + `left` / `right`：
- `Push=6`：视觉上向右移动 6 个栅格宽度
- `Pull=18`：视觉上向左移动 18 个栅格宽度

常用于交换两列的视觉顺序。

### Order（排列顺序）

`Order` 属性控制列的排列顺序，值越小越靠前。等效于 CSS Flexbox 的 `order` 属性。支持通过 `GridColSize`（`Xs` / `Sm` / `Md` / `Lg` / `Xl` / `Xxl`）按断点设置不同的排列顺序。

### 响应式列配置

通过 `Col` 的 `Xs` / `Sm` / `Md` / `Lg` / `Xl` / `Xxl` 属性，可以为不同断点设置不同的布局参数。每个属性接受 `GridColSize` 类型，可以覆盖 `Span`、`Offset`、`Order`、`Push`、`Pull`。

设置方式有三种：
1. **纯数字**：`Xs="24"` 等价于 `Xs="{atom:ColInfo Span=24}"`
2. **ColInfo MarkupExtension**：`Sm="{atom:ColInfo Order=2}"`
3. **ColInfo 子元素**：`<atom:Col.Md><atom:ColInfo Span="12" Order="2" /></atom:Col.Md>`

### 断点感知机制

`Row` 通过 `IMediaBreakAwareControl` 接口监听窗口尺寸变化。当窗口宽度跨越断点阈值时，`Row` 会重新测量和排列所有子元素，使响应式配置生效。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 24 栅格 | ✅ 24 列 | ✅ `GridColumns = 24` | ✅ 完全对齐 |
| `gutter` | ✅ `number / [number, number] / responsive` | ✅ `GridGutter`（TypeConverter 解析字符串） | ✅ 完全对齐 |
| `span` | ✅ 0-24 | ✅ `GridColSpanInfo`（支持响应式） | ✅ 完全对齐 |
| `offset` | ✅ 0-24 | ✅ `Col.Offset` | ✅ 完全对齐 |
| `push` / `pull` | ✅ 0-24 | ✅ `Col.Push` / `Col.Pull` | ✅ 完全对齐 |
| `order` | ✅ 整数 | ✅ `Col.Order` | ✅ 完全对齐 |
| `justify` | ✅ 6 种模式 | ✅ `RowJustify` 枚举 | ✅ 完全对齐 |
| `align` | ✅ `top / middle / bottom / stretch` | ✅ `RowAlign` 枚举 | ✅ 完全对齐 |
| `wrap` | ✅ 布尔 | ✅ `Row.Wrap` | ✅ 完全对齐 |
| 响应式 `xs/sm/md/lg/xl/xxl` | ✅ `number / ColSize` | ✅ `GridColSize` + `ColInfo` | ✅ 完全对齐 |
| `flex` 属性 | ✅ CSS flex | ❌ 不适用（非 CSS 环境） | — 平台差异 |
| `useBreakpoint` Hook | ✅ React Hook | ✅ `IMediaBreakAwareControl` 接口 | ✅ 等效实现 |

---

## 继承关系

```
Row（栅格行）:
  Avalonia.Controls.Panel
    └── AtomUI.Desktop.Controls.Row

Col（栅格列）:
  Avalonia.Controls.Primitives.TemplatedControl
    └── Avalonia.Controls.ContentControl
          └── AtomUI.Desktop.Controls.Col

ColInfo（列配置别名）:
  AtomUI.Desktop.Controls.GridColSize
    └── AtomUI.Desktop.Controls.ColInfo

ColInfoExtension（AXAML MarkupExtension）:
  Avalonia.Markup.Xaml.MarkupExtension
    └── AtomUI.Desktop.Controls.ColInfoExtension
```

**各层级职责划分**：

| 层级 | 提供的能力 |
|---|---|
| `Panel` | 子元素集合管理（Children）、自定义 MeasureOverride / ArrangeOverride |
| `Row` | 24 栅格计算、Gutter 间距、主轴/交叉轴对齐、换行、响应式断点监听 |
| `ContentControl` | 单个子内容管理（Content）、内容模板化 |
| `Col` | 列的 Span / Offset / Order / Push / Pull 配置、响应式断点覆盖、向 Row 报告布局参数 |
| `GridColSize` / `ColInfo` | 按断点存储列配置的数据模型，支持 TypeConverter |
| `ColInfoExtension` | AXAML MarkupExtension，简化内联响应式配置 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 行容器 | `src/AtomUI.Desktop.Controls/Grid/Row.cs` | Row（24 栅格布局引擎） |
| 列控件 | `src/AtomUI.Desktop.Controls/Grid/Col.cs` | Col（栅格列） |
| 列配置别名 | `src/AtomUI.Desktop.Controls/Grid/ColInfo.cs` | ColInfo（GridColSize 的别名） |
| MarkupExtension | `src/AtomUI.Desktop.Controls/Grid/ColInfoExtension.cs` | ColInfoExtension（AXAML 内联配置） |
| 对齐枚举 | `src/AtomUI.Desktop.Controls/Grid/GridAlignment.cs` | RowJustify / RowAlign 枚举 + TypeConverter |
| 间距类型 | `src/AtomUI.Desktop.Controls/Grid/GridGutter.cs` | GridGutter + GridGutterConverter |
| 间距数据 | `src/AtomUI.Desktop.Controls/Grid/GridGutterInfo.cs` | GridGutterInfo（响应式间距值） |
| Span 数据 | `src/AtomUI.Desktop.Controls/Grid/GridColSpanInfo.cs` | GridColSpanInfo（响应式 Span 值） |
| 列尺寸数据 | `src/AtomUI.Desktop.Controls/Grid/GridColSize.cs` | GridColSize + GridColLayout + GridColSizeConverter |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Layout/GridShowCase.axaml` | 使用范例 |
