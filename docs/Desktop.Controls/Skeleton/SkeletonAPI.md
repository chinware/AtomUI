# Skeleton API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;   // 所有 Skeleton 系列控件
namespace AtomUI;                     // Dimension, DimensionUnitType
namespace AtomUI.Controls;            // AvatarShape, CustomizableSizeType
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### SkeletonButtonShape

按钮骨架形状枚举。

| 值 | 说明 |
|---|---|
| `Square` | 方形（默认），圆角由 `BorderRadiusSM` 控制 |
| `Round` | 胶囊形，圆角 = 高度/2 |
| `Circle` | 圆形，宽度 = 高度，完全圆角 |

### AvatarShape（来自 `AtomUI.Controls`）

头像形状枚举。

| 值 | 说明 |
|---|---|
| `Circle` | 圆形（默认） |
| `Square` | 方形 |

### CustomizableSizeType（来自 `AtomUI`）

可自定义尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Large` | 大号 |
| `Middle` | 中号（默认） |
| `Small` | 小号 |
| `Custom` | 自定义尺寸（通过 `Size` 属性指定） |

### DimensionUnitType（来自 `AtomUI`）

尺寸单位类型枚举。

| 值 | 说明 |
|---|---|
| `Percentage` | 百分比（0～100），AXAML 中写 `"50%"` |
| `Pixel` | 像素值，AXAML 中写 `"200"` |

---

## AbstractSkeleton（流光动画基类）

所有 Skeleton 系列控件的抽象基类，提供流光动画基础设施。

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsActive` | `bool` | `false` | 是否启用流光动画。启用后骨架块显示从左到右的渐变流光效果 |
| `MotionDuration` | `TimeSpan` | `1.4s`（由 Token 控制） | 流光动画一次循环的时长 |
| `MotionEasingCurve` | `Easing?` | `CubicEaseOut` | 流光动画的缓动曲线 |

### 内部属性（由主题/动画驱动）

| 属性名 | 类型 | 说明 |
|---|---|---|
| `LoadingBackgroundStart` | `IBrush?` | 流光动画起始帧画刷 |
| `LoadingBackgroundMiddle` | `IBrush?` | 流光动画中间帧画刷 |
| `LoadingBackgroundEnd` | `IBrush?` | 流光动画结束帧画刷 |
| `AnimationLayerFill` | `IBrush?` | 当前动画帧的画刷值（由动画 KeyFrame 驱动） |

---

## Skeleton（组合骨架屏）

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsLoading` | `bool` | `false` | 加载状态。`true` 显示骨架占位，`false` 显示 `Content` 中的真实内容 |
| `IsShowAvatar` | `bool` | `false` | 是否显示头像骨架区域 |
| `IsShowParagraph` | `bool` | `true` | 是否显示段落骨架区域 |
| `IsShowTitle` | `bool` | `true` | 是否显示标题骨架区域 |
| `IsRound` | `bool` | `false` | 段落行是否使用胶囊形圆角 |
| `TitleWidth` | `Dimension` | `50%` | 标题行宽度，支持百分比和像素 |
| `ParagraphRows` | `int` | `2` | 段落行数（≥1） |
| `ParagraphLastLineWidth` | `Dimension` | `61%` | 段落最后一行宽度 |
| `ParagraphLineWidths` | `List<Dimension>?` | `null` | 段落每行的自定义宽度列表。设置后覆盖 `ParagraphLastLineWidth` |
| `AvatarShape` | `AvatarShape` | `Circle` | 头像骨架的形状 |
| `AvatarSizeType` | `CustomizableSizeType` | `Middle` | 头像骨架的尺寸类型（主题默认覆盖为 `Large`） |
| `AvatarSize` | `double` | `NaN` | 头像骨架的自定义尺寸（像素）。`NaN` 时由 `AvatarSizeType` 决定 |
| `Content` | `object?` | `null` | 真实内容（内容属性 `[Content]`），加载完成后显示 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `HorizontalContentAlignment` | `HorizontalAlignment` | `Left` | 真实内容的水平对齐 |
| `VerticalContentAlignment` | `VerticalAlignment` | `Top` | 真实内容的垂直对齐 |

### 主题自动调整规则

| 组合条件 | 自动调整 |
|---|---|
| `IsShowAvatar=True` + `IsShowParagraph=True` | `TitleWidth` → `50%` |
| `IsShowAvatar=False` + `IsShowParagraph=True` | `TitleWidth` → `38%` |
| `IsShowAvatar=False` + `IsShowTitle=False` | `ParagraphLastLineWidth` → `61%` |
| `IsShowAvatar=False` + `IsShowTitle=True` | `ParagraphRows` → `3` |
| `IsShowTitle=True` + `IsShowParagraph=False` | `AvatarShape` → `Square` |

---

## SkeletonElement（独立元素基类）

`SkeletonButton` 和 `SkeletonInput` 的抽象基类。

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `CustomizableSizeType` | `Middle` | 尺寸类型，控制高度（Large/Middle/Small/Custom） |
| `IsBlock` | `bool` | `false` | 是否撑满父容器宽度。设为 `true` 时 `HorizontalAlignment` 切换为 `Stretch` |

---

## SkeletonButton（按钮骨架）

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Shape` | `SkeletonButtonShape` | `Square` | 按钮骨架形状。`Square`：方形；`Round`：胶囊形；`Circle`：圆形 |

### 尺寸计算逻辑

| 形状 | 宽度 | 圆角 |
|---|---|---|
| `Square` | `Height × 2` | `BorderRadiusSM` |
| `Round` | `Height × 2` | `Height / 2` |
| `Circle` | `Height`（强制正方形） | `Height`（完全圆角） |

当 `IsBlock=True` 时，宽度计算被跳过，控件撑满可用宽度。

---

## SkeletonInput（输入框骨架）

### 尺寸计算逻辑

- 默认宽度 = `Height × 5`
- 当 `IsBlock=True` 时，宽度计算被跳过，控件撑满可用宽度
- 圆角使用 `BorderRadiusSM`

---

## SkeletonAvatar（头像骨架）

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Shape` | `AvatarShape` | `Circle` | 头像形状。`Circle`：圆形（完全圆角）；`Square`：方形（使用 `BorderRadius*`） |
| `SizeType` | `CustomizableSizeType` | `Middle` | 尺寸类型 |
| `Size` | `double` | `NaN` | 自定义尺寸（像素）。设置后覆盖 `SizeType` 对应的宽高 |

### 尺寸与圆角映射

| SizeType | 宽高 | Square 圆角 |
|---|---|---|
| `Large` | `ControlHeightLG` | `BorderRadiusLG` |
| `Middle` | `ControlHeight` | `BorderRadius` |
| `Small` | `ControlHeightSM` | `BorderRadiusSM` |
| `Custom`（通过 `Size`） | `Size` | `BorderRadius` |

Circle 形状下，圆角始终等于宽度（完全圆角），无论 SizeType。

---

## SkeletonImage（图片骨架）

无额外公共属性。尺寸由 Token 控制：
- 宽高 = `ImageContainerSize`
- 最大宽高 = `ImageContainerMaxSize`
- 内嵌 `ImageFilled` 图标，尺寸 = `ImageSize`

---

## SkeletonNode（自定义节点骨架）

继承自 `SkeletonImage`，支持放入自定义内容。

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 自定义内容（内容属性 `[Content]`） |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |

---

## SkeletonLine（行骨架）

段落的组成单元，由 `SkeletonParagraph` 动态创建。

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `LineWidth` | `Dimension` | `100%` | 行宽度，支持百分比和像素 |
| `IsRound` | `bool` | `false` | 是否使用胶囊形圆角 |

---

## SkeletonTitle（标题骨架）

继承自 `SkeletonLine`，使用 `SkeletonLine` 的 StyleKey。

- 默认 `LineWidth` = `50%`（通过 `OverrideDefaultValue` 设置）

---

## SkeletonParagraph（段落骨架）

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Rows` | `int` | `2` | 段落行数（≥1） |
| `LastLineWidth` | `Dimension` | `100%` | 最后一行宽度 |
| `LineWidths` | `List<Dimension>?` | `null` | 每行的自定义宽度列表 |
| `IsRound` | `bool` | `false` | 行是否使用胶囊形圆角 |

---

## Dimension 结构体

用于表达尺寸值，支持百分比和像素两种单位。

### 构造方式

```csharp
// 像素值
new Dimension(200)                            // 200px
new Dimension(200, DimensionUnitType.Pixel)   // 200px

// 百分比
new Dimension(50, DimensionUnitType.Percentage) // 50%
```

### AXAML 中的写法

```xml
TitleWidth="50%"      <!-- 百分比 -->
TitleWidth="200"      <!-- 像素 -->
```

### 属性

| 属性 | 类型 | 说明 |
|---|---|---|
| `Value` | `double` | 数值 |
| `UnitType` | `DimensionUnitType` | 单位类型 |
| `IsPercentage` | `bool` | 是否为百分比 |
| `IsAbsolute` | `bool` | 是否为像素绝对值 |
