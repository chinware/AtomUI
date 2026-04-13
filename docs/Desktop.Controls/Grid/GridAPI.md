# Grid API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;  // Row, Col, ColInfo, ColInfoExtension, GridGutter, GridGutterInfo, GridColSpanInfo, GridColSize, RowJustify, RowAlign
namespace AtomUI.Controls;           // MediaBreakPoint, IMediaBreakAwareControl
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### RowJustify

主轴对齐方式枚举。支持 TypeConverter，AXAML 中可使用 CSS 风格字符串（如 `"space-between"`）。

| 值 | CSS 等价 | 说明 |
|---|---|---|
| `Start` | `flex-start` | 左对齐（默认） |
| `Center` | `center` | 居中对齐 |
| `End` | `flex-end` | 右对齐 |
| `SpaceBetween` | `space-between` | 两端对齐，项之间等间距 |
| `SpaceAround` | `space-around` | 每项两侧等间距 |
| `SpaceEvenly` | `space-evenly` | 所有间距完全相等 |

### RowAlign

交叉轴对齐方式枚举。支持 TypeConverter，AXAML 中可使用 `"top"` / `"middle"` / `"bottom"` / `"stretch"` 字符串。

| 值 | 说明 |
|---|---|
| `Top` | 顶部对齐 |
| `Middle` | 垂直居中 |
| `Bottom` | 底部对齐 |
| `Stretch` | 拉伸填满行高（默认） |

### MediaBreakPoint（来自 `AtomUI.Controls`）

响应式断点枚举。

| 值 | 宽度阈值 |
|---|---|
| `ExtraSmall` | < 576px |
| `Small` | ≥ 576px |
| `Medium` | ≥ 768px |
| `Large` | ≥ 992px |
| `ExtraLarge` | ≥ 1200px |
| `ExtraExtraLarge` | ≥ 1600px |

---

## Row 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Gutter` | `GridGutter` | `new GridGutter()` (0,0) | 列间距（水平 + 垂直），支持 TypeConverter 字符串解析 |
| `Justify` | `RowJustify` | `Start` | 主轴对齐方式 |
| `Align` | `RowAlign` | `Stretch` | 交叉轴对齐方式 |
| `Wrap` | `bool` | `true` | 是否允许列换行（Span 总和超过 24 时） |

> `Row` 继承自 `Panel`，拥有 `Panel` 的所有属性（`Background`、`Children`、`ClipToBounds` 等）。

---

## Col 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 验证规则 | 说明 |
|---|---|---|---|---|
| `Span` | `GridColSpanInfo` | `0` | 0-24 | 列占据的栅格数，支持响应式字符串 |
| `Offset` | `int` | `0` | 0-24 | 列向右偏移的栅格数 |
| `Order` | `int` | `0` | 任意整数 | 列的排列顺序 |
| `Push` | `int` | `0` | 0-24 | 列向右视觉位移的栅格数 |
| `Pull` | `int` | `0` | 0-24 | 列向左视觉位移的栅格数 |
| `Xs` | `GridColSize?` | `null` | — | ExtraSmall 断点配置 |
| `Sm` | `GridColSize?` | `null` | — | Small 断点配置 |
| `Md` | `GridColSize?` | `null` | — | Medium 断点配置 |
| `Lg` | `GridColSize?` | `null` | — | Large 断点配置 |
| `Xl` | `GridColSize?` | `null` | — | ExtraLarge 断点配置 |
| `Xxl` | `GridColSize?` | `null` | — | ExtraExtraLarge 断点配置 |

> `Col` 继承自 `ContentControl`，拥有 `Content`、`ContentTemplate`、`HorizontalContentAlignment`、`VerticalContentAlignment` 等属性。

---

## 数据类型

### GridGutter

列间距配置，支持 TypeConverter 从字符串解析。

| 属性 | 类型 | 说明 |
|---|---|---|
| `Horizontal` | `GridGutterInfo` | 水平间距配置 |
| `Vertical` | `GridGutterInfo` | 垂直间距配置 |

**AXAML 字符串格式**：

| 格式 | 示例 | 说明 |
|---|---|---|
| 单数字 | `"16"` | 水平间距 16，垂直间距 0 |
| 两个数字 | `"16,24"` | 水平 16，垂直 24 |
| 响应式 | `"xs:8,sm:16,md:24"` | 按断点设置水平间距 |
| 水平+垂直响应式 | `"xs:8,sm:16;xs:16,sm:24"` | 用 `;` 分隔水平和垂直 |

**构造方法**：

| 签名 | 说明 |
|---|---|
| `GridGutter()` | 全零间距 |
| `GridGutter(GridGutterInfo horizontal, GridGutterInfo vertical)` | 指定水平和垂直间距 |

**静态方法**：

| 方法 | 签名 | 说明 |
|---|---|---|
| `Parse` | `static GridGutter Parse(string input)` | 从字符串解析 |

### GridGutterInfo

响应式间距值，每个断点一个 `double` 值。

| 属性 | 类型 | 说明 |
|---|---|---|
| `ExtraSmall` | `double` | xs 断点间距值 |
| `Small` | `double` | sm 断点间距值 |
| `Medium` | `double` | md 断点间距值 |
| `Large` | `double` | lg 断点间距值 |
| `ExtraLarge` | `double` | xl 断点间距值 |
| `ExtraExtraLarge` | `double` | xxl 断点间距值 |

**构造方法**：

| 签名 | 说明 |
|---|---|
| `GridGutterInfo()` | 全零 |
| `GridGutterInfo(double value)` | 所有断点使用同一值 |
| `GridGutterInfo(double xs, double sm, double md, double lg, double xl, double xxl)` | 每个断点独立值 |

**AXAML 字符串格式**：`"xs:8,sm:16,md:24,lg:32,xl:40,xxl:48"` 或单数字 `"16"`

### GridColSpanInfo

响应式 Span 值（`readonly record struct`），每个断点一个 `int` 值。支持隐式从 `int` 转换。

| 属性 | 类型 | 说明 |
|---|---|---|
| `ExtraSmall` | `int` | xs 断点 Span 值 |
| `Small` | `int` | sm 断点 Span 值 |
| `Medium` | `int` | md 断点 Span 值 |
| `Large` | `int` | lg 断点 Span 值 |
| `ExtraLarge` | `int` | xl 断点 Span 值 |
| `ExtraExtraLarge` | `int` | xxl 断点 Span 值 |

**AXAML 字符串格式**：`"xs:24,sm:12,md:8,lg:6"` 或单数字 `"12"`

**隐式转换**：`GridColSpanInfo span = 12;`

### GridColSize / ColInfo

按断点存储列的完整布局配置。`ColInfo` 是 `GridColSize` 的别名（继承），带有 `GridColSizeConverter` 支持 AXAML 解析。

| 属性 | 类型 | 说明 |
|---|---|---|
| `Span` | `int?` | 栅格数（0-24），null 表示不覆盖 |
| `Offset` | `int?` | 偏移栅格数（0-24），null 表示不覆盖 |
| `Order` | `int?` | 排列顺序，null 表示不覆盖 |
| `Push` | `int?` | 向右视觉位移（0-24），null 表示不覆盖 |
| `Pull` | `int?` | 向左视觉位移（0-24），null 表示不覆盖 |

**AXAML 字符串格式**：
- 纯数字：`"12"` → `Span = 12`
- 键值对：`"span:12,offset:4,order:2"` → `Span = 12, Offset = 4, Order = 2`

### ColInfoExtension（MarkupExtension）

AXAML MarkupExtension，用于在属性内联中构造 `GridColSize` 对象。

| 属性 | 类型 | 说明 |
|---|---|---|
| `Span` | `int?` | 栅格数（0-24） |
| `Offset` | `int?` | 偏移栅格数（0-24） |
| `Order` | `int?` | 排列顺序 |
| `Push` | `int?` | 向右视觉位移（0-24） |
| `Pull` | `int?` | 向左视觉位移（0-24） |

**使用方式**：

```xml
<atom:Col Sm="{atom:ColInfo Span=12, Order=2}" />
```

---

## TypeConverter 汇总

| 类型 | TypeConverter | AXAML 示例 |
|---|---|---|
| `GridGutter` | `GridGutterConverter` | `Gutter="16,24"` |
| `GridGutterInfo` | 内部使用 `Parse()` | — |
| `GridColSpanInfo` | `GridColSpanInfoConverter` | `Span="xs:24,sm:12,md:8"` |
| `GridColSize` / `ColInfo` | `GridColSizeConverter` | `Xs="span:12,offset:4"` 或 `Xs="12"` |
| `RowJustify` | `RowJustifyConverter` | `Justify="space-between"` |
| `RowAlign` | `RowAlignConverter` | `Align="middle"` |
