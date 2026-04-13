# Steps API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### StepsItemStatus

步骤项状态。

| 值 | 说明 |
|---|---|
| `Wait` | 等待执行 |
| `Process` | 正在执行（默认） |
| `Finish` | 已完成 |
| `Error` | 出错 |

### StepsStyle

步骤条展示风格。

| 值 | 说明 |
|---|---|
| `Default` | 标准步骤条（默认） |
| `Navigation` | 导航式步骤条 |
| `Inline` | 内联式步骤条 |

### StepsItemIndicatorType

指示器类型。

| 值 | 说明 |
|---|---|
| `Default` | 数字圆圈或自定义图标（默认） |
| `Dot` | 小圆点指示器 |

---

## Steps（步骤条容器）

继承自 `SelectingItemsControl`，实现 `ISizeTypeAware`、`IMotionAwareControl`。

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `CurrentStep` | `int` | `0` | 当前步骤索引（从 0 开始），自动同步到 `SelectedIndex` |
| `InitialStep` | `int` | `-1` | 初始步骤索引，在 `OnApplyTemplate` 时设置 `CurrentStep`（-1 表示不设置） |
| `ProgressValue` | `double` | `0` | 当前步骤的进度值（0-100），仅在 `IsShowItemProgress=true` 时生效 |
| `CurrentStepStatus` | `StepsItemStatus` | `Process` | 当前步骤的状态（之前的步骤自动为 Finish，之后自动为 Wait） |
| `Orientation` | `Orientation` | `Horizontal` | 排列方向：水平或垂直 |
| `LabelPlacement` | `Orientation` | `Horizontal` | 标签位置：水平（标题在指示器右侧）或垂直（标题在指示器下方） |
| `SizeType` | `SizeType` | `Middle` | 尺寸变体：Small / Middle / Large |
| `ItemIndicatorType` | `StepsItemIndicatorType` | `Default` | 指示器类型：Default 或 Dot |
| `Style` | `StepsStyle` | `Default` | 展示风格：Default / Navigation / Inline |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `IsItemClickable` | `bool` | `false` | 是否允许点击步骤进行跳转 |
| `IsShowItemProgress` | `bool` | `false` | 是否在当前步骤显示进度环 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 步骤内容的默认数据模板 |

### 只读属性（DirectProperty）

| 属性名 | 类型 | 说明 |
|---|---|---|
| `CurrentContent` | `object?` | 当前选中步骤的 `Content`，自动跟踪 `SelectedItem` 变化 |
| `CurrentContentTemplate` | `IDataTemplate?` | 当前选中步骤的 `ContentTemplate`，若步骤项未指定则回退到 `Steps.ContentTemplate` |

### 继承自 SelectingItemsControl 的常用属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Items` | `IEnumerable` | 步骤项数据源 |
| `ItemTemplate` | `IDataTemplate?` | 步骤项的数据模板 |
| `SelectedIndex` | `int` | 当前选中索引（与 `CurrentStep` 自动同步） |
| `SelectedItem` | `object?` | 当前选中项 |
| `ItemCount` | `int` | 步骤项总数 |

---

## StepsItem（步骤项）

继承自 `HeaderedContentControl`，实现 `ISelectable`。

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SubHeader` | `object?` | `null` | 副标题（显示在标题右侧或下方） |
| `SubHeaderTemplate` | `IDataTemplate?` | `null` | 副标题数据模板 |
| `Description` | `object?` | `null` | 描述文本（显示在标题下方） |
| `DescriptionTemplate` | `IDataTemplate?` | `null` | 描述数据模板 |
| `IsSelected` | `bool` | `false` | 是否为当前选中步骤 |
| `Icon` | `PathIcon?` | `null` | 自定义图标（替换默认的数字指示器） |
| `Status` | `StepsItemStatus` | `Process` | 步骤状态（通常由 Steps 容器自动管理） |
| `LabelPlacement` | `Orientation` | 继承自 Steps | 标签位置 |

### 继承自 HeaderedContentControl 的常用属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Header` | `object?` | 步骤标题 |
| `HeaderTemplate` | `IDataTemplate?` | 标题数据模板 |
| `Content` | `object?` | 步骤关联内容（通过 `Steps.CurrentContent` 展示） |
| `ContentTemplate` | `IDataTemplate?` | 内容数据模板 |

---

## 伪类（Pseudo-Classes）

### Steps 伪类

| 伔类 | 触发条件 |
|---|---|
| `:horizontal` | `Orientation == Horizontal` |
| `:vertical` | `Orientation == Vertical` |

### StepsItem 伪类

| 伔类 | 触发条件 |
|---|---|
| `:finished` | 步骤已完成（`IsFinished == true`） |
| `:selected` | 当前选中步骤 |
| `:pressed` | 鼠标按下 |

### 属性选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Steps[Style=Default]` | 默认风格 |
| `atom\|Steps[Style=Navigation]` | 导航风格 |
| `atom\|Steps[Style=Inline]` | 内联风格 |
| `atom\|Steps[Orientation=Horizontal]` | 水平方向 |
| `atom\|Steps[Orientation=Vertical]` | 垂直方向 |
| `atom\|Steps[SizeType=Small]` | 小尺寸 |
| `atom\|StepsItem[Status=Wait]` | 等待状态 |
| `atom\|StepsItem[Status=Process]` | 进行中状态 |
| `atom\|StepsItem[Status=Finish]` | 完成状态 |
| `atom\|StepsItem[Status=Error]` | 错误状态 |
| `atom\|StepsItem[IsClickable=True]` | 可点击步骤 |
| `atom\|StepsItem[IndicatorType=Dot]` | 点状指示器 |

---

## 实现的接口

### ISizeTypeAware（Steps）

```csharp
public SizeType SizeType { get; set; }
```

支持 `Small` / `Middle` / `Large` 三种尺寸。Small 模式下指示器尺寸缩小，间距减小。

### IMotionAwareControl（Steps）

```csharp
public bool IsMotionEnabled { get; set; }
```

控制步骤切换时的颜色和变换过渡动画。

### ISelectable（StepsItem）

```csharp
public bool IsSelected { get; set; }
```

使 StepsItem 可被 SelectingItemsControl 选择。
