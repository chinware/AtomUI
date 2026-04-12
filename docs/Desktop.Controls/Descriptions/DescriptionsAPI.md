# Descriptions API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## Descriptions 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsBordered` | `bool` | `false` | 是否显示边框。开启后使用表格风格展示，标签列有背景色 |
| `IsShowColon` | `bool` | `true` | 标签后是否显示冒号 |
| `ColumnInfo` | `DescriptionsMediaBreakInfo` | `new(3)` | 列数配置，支持固定数值或响应式断点配置 |
| `Layout` | `Orientation` | `Horizontal` | 布局方向。`Horizontal`=标签和内容同行；`Vertical`=标签在上、内容在下 |
| `SizeType` | `SizeType` | `SizeType.Large` | 尺寸（Large / Middle / Small），控制子项内间距 |
| `Header` | `object?` | `null` | 标题内容 |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 标题内容模板 |
| `Extra` | `object?` | `null` | 额外操作区域内容（显示在标题右侧） |
| `ExtraTemplate` | `IDataTemplate?` | `null` | 额外操作区域模板 |
| `ItemsSource` | `IEnumerable?` | `null` | 数据源，可绑定 `DescriptionItem` 集合 |

### Content 属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Items` | `DescriptionItems` | 子项集合（`[Content]` 标记），可在 AXAML 中直接声明 |

> 注意：`SizeType` 默认值为 `SizeType.Large`（通过 `OverrideDefaultValue` 覆盖），与大多数其他控件的 `SizeType.Middle` 默认值不同。这与 Ant Design 中 `Descriptions` 默认 `size="default"` 对应的是 Large 尺寸保持一致。

---

## DescriptionItem 数据模型

`DescriptionItem` 是一个 `record` 类，用于声明每个描述字段。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Label` | `string` | `string.Empty` | 标签文本 |
| `Content` | `object?` | `null` | 内容，可以是字符串或任何 Avalonia 控件 |
| `IsFilled` | `bool` | `false` | 是否占满当前行剩余列 |
| `Span` | `DescriptionsMediaBreakInfo` | `new(1)` | 跨列数，支持固定数值或按断点配置 |

---

## DescriptionsMediaBreakInfo 断点配置

`DescriptionsMediaBreakInfo` 是一个 `record` 类，同时用于 `ColumnInfo`（列数）和 `DescriptionItem.Span`（跨列数）。

### 属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `ExtraSmall` | `int` | xs 断点（≤576px）的值 |
| `Small` | `int` | sm 断点（≤768px）的值 |
| `Medium` | `int` | md 断点（≤992px）的值 |
| `Large` | `int` | lg 断点（≤1200px）的值 |
| `ExtraLarge` | `int` | xl 断点（≤1600px）的值 |
| `ExtraExtraLarge` | `int` | xxl 断点（>1600px）的值 |

### 构造函数

| 签名 | 说明 |
|---|---|
| `DescriptionsMediaBreakInfo()` | 默认构造，所有断点为 1 |
| `DescriptionsMediaBreakInfo(int column)` | 所有断点统一为指定列数 |
| `DescriptionsMediaBreakInfo(int xs, int sm, int md, int lg, int xl, int xxl)` | 每个断点独立指定 |

### 字符串解析

`DescriptionsMediaBreakInfo` 支持在 AXAML 中通过字符串赋值，自动调用 `Parse()` 方法：

```xml
<!-- 固定列数 -->
<atom:Descriptions ColumnInfo="3" />

<!-- 按断点配置 -->
<atom:Descriptions ColumnInfo="xs: 1, sm: 2, md: 3, lg: 3, xl: 4, xxl: 4" />

<!-- 子项跨列 -->
<atom:DescriptionItem Span="2" />
<atom:DescriptionItem Span="xl: 2, xxl: 2" />
```

---

## 公共方法

Descriptions 控件不暴露额外的公共方法。子项管理通过 `Items` 集合或 `ItemsSource` 数据源进行。

---

## 伪类（Pseudo-Classes）

Descriptions 控件未定义自定义伪类。它继承标准的 TemplatedControl 伪类：

| 伪类 | 触发条件 |
|---|---|
| `:disabled` | `IsEnabled == false` |
| `:pointerover` | 鼠标悬浮 |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

控制子项的内间距：
- `Large` — 使用 `ItemPaddingLG` Token
- `Middle` — 使用 `ItemPadding` Token
- `Small` — 使用 `ItemPaddingSM` Token

在无边框模式下，`SizeType` 还控制行间距（RowSpacing）：
- `Large` — `SharedToken.Spacing`
- `Middle` — `SharedToken.SpacingSM`
- `Small` — `SharedToken.SpacingXS`
