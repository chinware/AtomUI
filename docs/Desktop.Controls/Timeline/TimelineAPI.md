# Timeline API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;        // Timeline, TimelineItem
namespace AtomUI.Controls.Commons;         // AbstractTimeline, AbstractTimelineItem（基类）
namespace AtomUI.Controls;                 // TimeLineMode 枚举
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### TimeLineMode

时间轴布局模式枚举，控制内容和指示器的相对位置。

| 值 | 说明 |
|---|---|
| `Left` | 左对齐模式（默认），指示器在左、内容在右 |
| `Right` | 右对齐模式，指示器在右、内容在左 |
| `Alternate` | 交替模式，内容在指示器左右交替排列 |

---

## Timeline 公共属性（StyledProperty）

> 以下属性定义在 `AbstractTimeline` 基类中，`Timeline` 继承使用。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Mode` | `TimeLineMode` | `TimeLineMode.Left` | 布局模式，控制内容与指示器的相对位置 |
| `Pending` | `object?` | `null` | 待办节点内容，非 null 时自动在末尾添加带 Loading 图标的幽灵节点 |
| `PendingIcon` | `PathIcon?` | `null`（默认 `LoadingOutlined`） | 待办节点的自定义指示器图标 |
| `IsReverse` | `bool` | `false` | 是否反转排列所有节点 |

### 继承自 ItemsControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Items` | `ItemCollection` | — | 子项集合，通常为 `TimelineItem` 实例 |
| `ItemTemplate` | `IDataTemplate?` | `null` | 子项数据模板 |
| `ItemsSource` | `IEnumerable?` | `null` | 数据绑定源集合 |
| `Background` | `IBrush?` | `SharedToken.ColorBgContainer` | 时间轴容器背景色 |
| `BorderBrush` | `IBrush?` | `SharedToken.ColorBorder` | 容器边框颜色 |

---

## TimelineItem 公共属性（StyledProperty）

> 以下属性定义在 `AbstractTimelineItem` 基类中，`TimelineItem` 继承使用。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Label` | `string?` | `null` | 标签文本（如日期），设置后触发标签布局模式 |
| `IndicatorIcon` | `PathIcon?` | `null` | 自定义指示器图标，替代默认空心圆点 |
| `IndicatorColor` | `IBrush?` | `null`（默认 `ColorPrimary`） | 指示器颜色（圆点边框或图标填充色） |

### 继承自 ContentControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 节点内容（可以是文本字符串或任意控件） |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容数据模板 |

---

## 事件

Timeline 和 TimelineItem 没有定义额外的公共事件。可使用继承的标准事件：

| 事件名 | 来源 | 说明 |
|---|---|---|
| `PointerPressed` | `Control` | 指针按下 |
| `PointerReleased` | `Control` | 指针释放 |

---

## 伪类（Pseudo-Classes）

### TimelineItem 特有伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `order-odd` | `AbstractTimelineItem.OrderOddPC` | 节点处于奇数位置（从 0 开始计算） |
| `order-even` | `AbstractTimelineItem.OrderEvenPC` | 节点处于偶数位置 |
| `order-first` | `AbstractTimelineItem.OrderFirstPC` | 第一个节点 |
| `order-last` | `AbstractTimelineItem.OrderLastPC` | 最后一个节点 |
| `pending-item` | `AbstractTimelineItem.PendingItemPC` | 待办（Pending）节点 |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |

---

## 静态构造器影响

### AbstractTimeline

以下属性变更会触发重新测量（`AffectsMeasure`）：
- `Mode`

以下属性变更会触发重新排列（`AffectsArrange`）：
- `IsReverse`

### AbstractTimelineItem

以下属性变更会触发重新排列（`AffectsArrange`）：
- `Mode`

以下属性变更会触发重新测量（`AffectsMeasure`）：
- `IsReverse`
