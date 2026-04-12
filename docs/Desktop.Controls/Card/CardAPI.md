# Card API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### CardStyleVariant

卡片风格变体枚举，定义卡片的视觉风格。

| 值 | 说明 |
|---|---|
| `Outline` | 有边框风格（默认），适用于白色/浅色背景 |
| `Borderless` | 无边框风格，使用阴影代替，适用于灰色/有色背景 |

### CardContentType（内部枚举）

卡片内容类型，根据 `Content` 的实际类型自动推断，不可外部设置。

| 值 | 说明 |
|---|---|
| `Default` | 默认内容类型 |
| `Meta` | Meta 描述型（Content 为 `CardMetaContent`） |
| `Grid` | 网格型（Content 为 `CardGridContent`） |
| `Tabs` | 标签页型（Content 为 `CardTabsContent`） |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## Card 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `BoxShadow` | `BoxShadows` | 无阴影（Outline）/ `BoxShadowsTertiary`（Borderless） | 卡片阴影效果，悬浮时自动切换为 `CardShadows` |
| `Extra` | `object?` | `null` | 头部右侧额外区域内容（如 "More" 链接） |
| `ExtraTemplate` | `IDataTemplate?` | `null` | 额外区域数据模板 |
| `StyleVariant` | `CardStyleVariant` | `Outline` | 卡片风格变体（有边框 / 无边框） |
| `SizeType` | `SizeType` | `SizeType.Middle` | 卡片尺寸（共享属性，通过 `AddOwner` 注册） |
| `IsLoading` | `bool` | `false` | 加载中状态，显示骨架屏占位 |
| `IsInnerMode` | `bool` | `false` | 内嵌模式，头部背景变为 `ColorFillAlter` 以区分嵌套层级 |
| `IsHoverable` | `bool` | `false` | 悬浮效果，鼠标悬浮时卡片升起（阴影加深、光标变手形） |
| `Cover` | `object?` | `null` | 封面内容（通常为 `Image` 控件） |
| `CoverTemplate` | `IDataTemplate?` | `null` | 封面数据模板 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token（`EnableMotion`） | 是否启用过渡动画（共享属性） |

### 集合属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Actions` | `Avalonia.Controls.Controls` | 操作区控件集合（CLR 属性，非 StyledProperty），推荐使用 `CardActionButton` |

### 继承自 HeaderedContentControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 卡片主体内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `Header` | `object?` | `null` | 卡片标题（设为字符串或控件） |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 标题模板 |
| `Background` | `IBrush?` | `ColorBgContainer` | 背景色（由主题控制） |
| `BorderBrush` | `IBrush?` | `ColorBorderSecondary` | 边框颜色（由主题控制） |
| `BorderThickness` | `Thickness` | 由 Token `BorderThickness` 控制 | 边框粗细 |
| `CornerRadius` | `CornerRadius` | 由 Token `BorderRadiusLG` 控制 | 圆角半径 |
| `Padding` | `Thickness` | 由 Token 控制 | 内间距 |
| `HorizontalContentAlignment` | `HorizontalAlignment` | `Stretch` | 内容水平对齐 |
| `VerticalContentAlignment` | `VerticalAlignment` | `Stretch` | 内容垂直对齐 |
| `IsEnabled` | `bool` | `true` | 是否启用 |

---

## 伪类（Pseudo-Classes）

Card 支持以下伪类，可在样式选择器中使用：

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:headerless` | `CardPseudoClass.Headerless` | `Header`、`HeaderTemplate`、`Extra`、`ExtraTemplate` 均为 `null` |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮（配合 `IsHoverable` 使用时触发阴影切换） |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

支持 `Small` / `Middle` / `Large` 三种尺寸，影响头部高度、字号、内容内边距。

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制 `BoxShadow` 过渡动画是否启用。

---

## CardMetaContent API

结构化 Meta 内容控件，继承自 `HeaderedContentControl`。用于在卡片中展示「头像 + 标题 + 描述」的结构化信息。

```csharp
namespace AtomUI.Desktop.Controls;
public class CardMetaContent : HeaderedContentControl
```

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Avatar` | `Avatar?` | `null` | 头像控件 |
| `Header`（继承） | `object?` | `null` | 标题内容（显示为粗体、大字号） |
| `HeaderTemplate`（继承） | `IDataTemplate?` | `null` | 标题模板 |
| `Content`（继承） | `object?` | `null` | 描述内容（显示为描述色） |
| `ContentTemplate`（继承） | `IDataTemplate?` | `null` | 描述模板 |

### 模板结构

```
DockPanel (HorizontalSpacing=Spacing)
├── ContentPresenter#AvatarContentPresenter (Dock=Left)  ← 头像
└── DockPanel (VerticalSpacing=SpacingXS)
    ├── ContentPresenter#TitleContentPresenter (Dock=Top) ← 标题（粗体、FontSizeLG）
    └── ContentPresenter#DescriptionContentPresenter      ← 描述（ColorTextDescription）
```

---

## CardTabsContent API

内嵌标签页内容控件，继承自 `TemplatedControl`。将 `TabControl` 嵌入卡片中，实现同一卡片内切换多个内容面板。

```csharp
namespace AtomUI.Desktop.Controls;
public class CardTabsContent : TemplatedControl
```

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `TabBarExtraContent` | `object?` | `null` | 标签栏右侧额外内容（如 "More" 按钮） |
| `TabBarExtraContentTemplate` | `IDataTemplate?` | `null` | 标签栏额外内容模板 |
| `TabItemsSource` | `IEnumerable?` | `null` | 标签页数据源（用于数据绑定场景） |
| `TabItemTemplate` | `IDataTemplate?` | `null` | 标签页项模板 |

### 集合属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Items` | `Controls`（标记 `[Content]`） | 标签页子项集合，直接添加 `TabItem` 控件 |

---

## CardGridContent API

网格布局内容控件，继承自 `ItemsControl`。将内容以等分网格方式布局，每个格子可独立交互。

```csharp
namespace AtomUI.Desktop.Controls;
public class CardGridContent : ItemsControl
```

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ColumnDefinitions` | `ColumnDefinitions` | 空 | Grid 列定义（语法同 Avalonia `Grid`） |
| `RowDefinitions` | `RowDefinitions` | 空 | Grid 行定义 |
| `IsHoverable` | `bool` | `true` | 网格项是否支持悬浮效果 |

---

## CardGridItem API

网格单元格控件，继承自 `ContentControl`。作为 `CardGridContent` 的子项使用。

```csharp
namespace AtomUI.Desktop.Controls;
public class CardGridItem : ContentControl
```

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `BoxShadow` | `BoxShadows` | `CardGridItemShadows`（Token） | 单元格边框阴影（模拟分割线效果） |
| `Row` | `int` | `0` | 在 Grid 中的行索引 |
| `Column` | `int` | `0` | 在 Grid 中的列索引 |
| `RowSpan` | `int` | `0` | 跨行数 |
| `ColumnSpan` | `int` | `0` | 跨列数 |
| `IsHoverable` | `bool` | `true` | 鼠标悬浮时是否显示阴影升起效果 |
| `SizeType` | `SizeType` | `SizeType.Middle` | 尺寸（影响单元格内边距） |
| `Content`（继承） | `object?` | `null` | 单元格内容 |

---

## CardActionButton API

卡片操作区按钮，继承自 `IconButton`。无额外属性定义，主题中自定义了默认样式。

```csharp
namespace AtomUI.Desktop.Controls;
public class CardActionButton : IconButton
```

### 主题默认样式

| 属性 | 值 | 说明 |
|---|---|---|
| `HorizontalAlignment` | `Stretch` | 水平撑满操作区格子 |
| `VerticalAlignment` | `Stretch` | 垂直撑满 |
| `IconWidth` / `IconHeight` | `CardActionsIconSize` | 图标大小由 Token 控制 |
| `IconBrush` | `ColorIcon` | 正常态图标颜色 |
| `IconBrush`（`:pointerover`） | `ColorPrimary` | 悬浮态图标变为主色 |
