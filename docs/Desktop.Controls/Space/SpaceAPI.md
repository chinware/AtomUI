# Space API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### SpaceItemsAlignment

Space 子项交叉轴对齐方式。

| 值 | 说明 |
|---|---|
| `Start` | 顶部/左对齐（默认） |
| `Center` | 居中对齐 |
| `End` | 底部/右对齐 |

### CustomizableSizeType（来自 `AtomUI.Core`）

可定制的尺寸类型枚举，比标准 `SizeType` 多一个 `Custom` 选项。

| 值 | 说明 |
|---|---|
| `Large` | 大号间距 |
| `Middle` | 中号间距 |
| `Small` | 小号间距（默认） |
| `Custom` | 自定义间距（由用户通过 `ItemSpacing` / `LineSpacing` 设置） |

### CompactSpaceUnitType

紧凑空间子项尺寸单位类型。

| 值 | 说明 |
|---|---|
| `Auto` | 自动尺寸，按内容大小 |
| `Pixel` | 固定像素值 |
| `Star` | 按比例分配剩余空间 |

### SizeType（来自 `AtomUI.Core`）

标准尺寸类型枚举，供 CompactSpace 和 CompactSpaceAddOn 使用。

| 值 | 说明 |
|---|---|
| `Large` | 大号 |
| `Middle` | 中号（默认） |
| `Small` | 小号 |

---

## Space 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Orientation` | `Orientation` | `Horizontal` | 子项排列方向（水平或垂直） |
| `SizeType` | `CustomizableSizeType` | `Small` | 预设间距尺寸，自动映射 Token 中的间距值 |
| `ItemSpacing` | `double` | 由 `SizeType` Token 控制 | 同一行内相邻子项之间的间距 |
| `LineSpacing` | `double` | 由 `SizeType` Token 控制 | 换行后行与行之间的间距 |
| `ItemsAlignment` | `SpaceItemsAlignment` | `Start` | 每行子项在交叉轴方向上的对齐方式 |
| `ItemWidth` | `double` | `NaN`（自然宽度） | 固定所有子项的宽度（设为 `NaN` 时使用子项自然宽度） |
| `ItemHeight` | `double` | `NaN`（自然高度） | 固定所有子项的高度（设为 `NaN` 时使用子项自然高度） |
| `SplitTemplate` | `ITemplate<Control>?` | `null` | 分割线模板，设置后在每两个相邻子项之间插入分隔控件 |
| `Children` | `AvaloniaList<Control>` | `[]` | 子控件集合（`[Content]` 属性，AXAML 中直接嵌套子元素） |

### CustomizableSizeType 与 Token 映射

| SizeType | Token 属性 | Token 来源 | 默认值（Light 主题） |
|---|---|---|---|
| `Small` | `GapSmallSize` | `SharedToken.SpacingXS` | 8 |
| `Middle` | `GapMiddleSize` | `SharedToken.Spacing` | 16 |
| `Large` | `GapLargeSize` | `SharedToken.SpacingLG` | 24 |
| `Custom` | 用户通过 `ItemSpacing` / `LineSpacing` 直接设置 | — | — |

---

## CompactSpace 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Orientation` | `Orientation` | `Horizontal` | 子项排列方向 |
| `SizeType` | `SizeType` | `Middle` | 预设尺寸（Small / Middle / Large），自动传递给子控件 |
| `Children` | `AvaloniaList<Control>` | `[]` | 子控件集合（`[Content]` 属性），子控件必须实现 `ICompactSpaceAware` |

### CompactSpace 附加属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `CompactSpace.ItemSize` | `CompactSpaceSize` | `Auto` | 控制子项在主轴方向上的尺寸分配 |

**CompactSpaceSize 语法**（字符串解析规则）：

| 输入 | 解析结果 | 说明 |
|---|---|---|
| `Auto` | `CompactSpaceSize.Auto` | 按内容自然大小 |
| `100` | `CompactSpaceSize(100, Pixel)` | 固定 100 像素 |
| `2*` | `CompactSpaceSize(2, Star)` | 占剩余空间的 2 份 |
| `*` | `CompactSpaceSize(1, Star)` | 占剩余空间的 1 份 |

---

## CompactSpaceAddOn 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 附加区域内容（`[Content]` 属性） |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `SizeType` | `SizeType` | `Middle` | 尺寸类型 |
| `StyleVariant` | `InputControlStyleVariant` | `Outline` | 样式变体（Outline / Filled / Underlined） |
| `Status` | `InputControlStatus` | `Default` | 输入控件状态（Default / Error / Warning） |

### 继承自 TemplatedControl 的常用属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Background` | `IBrush?` | 背景色（由主题控制） |
| `BorderBrush` | `IBrush?` | 边框颜色（由主题控制） |
| `BorderThickness` | `Thickness` | 边框粗细（由 Token 控制） |
| `CornerRadius` | `CornerRadius` | 圆角半径（由 Token + 紧凑位置控制） |
| `Padding` | `Thickness` | 内间距（由 Token + SizeType 控制） |
| `FontSize` | `double` | 字体大小（由 Token + SizeType 控制） |

---

## CompactSpaceSize 结构体

| 属性/方法 | 类型 | 说明 |
|---|---|---|
| `Value` | `double` | 尺寸数值 |
| `CompactSpaceUnitType` | `CompactSpaceUnitType` | 尺寸单位类型 |
| `IsAbsolute` | `bool` | 是否为固定像素 |
| `IsAuto` | `bool` | 是否为自动尺寸 |
| `IsStar` | `bool` | 是否为按比例分配 |
| `Auto` | `CompactSpaceSize`（静态） | 获取 Auto 实例 |
| `Star` | `CompactSpaceSize`（静态） | 获取 Star(1) 实例 |
| `Parse(string)` | `CompactSpaceSize`（静态） | 从字符串解析 |
| `ParseLengths(string)` | `IEnumerable<CompactSpaceSize>`（静态） | 解析多个尺寸值 |

---

## 实现的接口

### Space

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ICustomizableSizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `CustomizableSizeType`（Small / Middle / Large / Custom）四种尺寸模式 |
| `IChildIndexProvider` | `Avalonia.LogicalTree` | 提供子控件索引，支持样式系统按索引匹配 |
| `INavigableContainer` | `Avalonia.Input` | 支持方向键在子项之间导航 |

### CompactSpace

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸 |
| `IChildIndexProvider` | `Avalonia.LogicalTree` | 子控件索引提供 |
| `INavigableContainer` | `Avalonia.Input` | 键盘导航支持 |

### CompactSpaceAddOn

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 三种尺寸切换 |
| `ICompactSpaceAware` | `AtomUI.Desktop.Controls`（internal） | 紧凑空间位置感知 |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | 输入控件状态感知（Error / Warning） |
| `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` | 样式变体切换（Outline / Filled / Underlined） |

---

## 异常

### InvalidSpaceFillerUsageException

| 属性 | 说明 |
|---|---|
| 继承自 | `SystemException` |
| 触发条件 | `CompactSpaceFiller` 未放在最后位置，或放置了多个 Filler |
| 消息示例 | `"The CompactSpaceFiller is misplaced, it can only be the last child."` |
| 消息示例 | `"There can only be one CompactSpaceFiller."` |
