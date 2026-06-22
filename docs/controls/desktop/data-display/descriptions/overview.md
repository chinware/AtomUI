# Descriptions 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.Descriptions` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [Descriptions 桌面版实现原理](implementation.md)，Descriptions Token 的专项设计见 [Descriptions Token 设计](token.md)，设计和契约变化记录见 [Descriptions Changelog](changelog.md)。

## 1. 控件定位

Descriptions 是桌面端数据展示类控件，用于按标签和值展示对象属性、订单信息、配置详情、用户资料等结构化只读信息。它通过 `DescriptionItem` 集合、响应式列数、水平/纵向布局、边框模式和 Header/Extra 区域表达“属性说明列表”的展示语义。

Descriptions 的职责是把描述项数据排版为稳定的网格视觉。它不负责编辑、选择、排序、过滤、远程数据加载、表单提交、虚拟化列表或复杂主从详情关系；这些能力应由业务层、DataGrid、ListView、Form 或专用编辑控件承担。

## 2. 设计语言

Descriptions 的设计语言来自 Ant Design 的描述列表：标签提供字段名，内容提供字段值，列数和跨度提供信息密度，边框模式提供表格化阅读边界。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 展示密度 | 用尺寸和列数控制同屏信息量。 | `SizeType`、`ColumnInfo`。 |
| 结构层级 | 用 Header 和 Extra 表达当前信息组名称和辅助操作入口。 | `Header`、`Extra`。 |
| 标签和值 | 每个 `DescriptionItem` 表达一对 label/content。 | `Label`、`Content`。 |
| 跨列能力 | 通过 `Span` 和 `IsFilled` 表达长内容或整行内容。 | `Span`、`IsFilled`。 |
| 视觉边界 | 普通模式轻量展示，边框模式强调表格结构。 | `IsBordered`。 |
| 方向模型 | 水平方向把 label 和 content 放在同一行，纵向方向上下排列。 | `Layout`。 |

Descriptions 是展示控件，不提供 hover、pressed、selected、checked、loading 或 popup 等交互状态。

## 3. API 与契约模型

Descriptions 的公共 API 分为控件级属性、内容集合入口和描述项模型。

控件级属性：

| API | 类型 | 语义 |
| --- | --- | --- |
| `IsBordered` | `bool` | 是否使用边框模式。水平边框模式会为每个 item 生成独立 label/content cell。 |
| `IsShowColon` | `bool` | 普通 horizontal/vertical 非边框模式下是否显示冒号，默认 `true`。 |
| `ColumnInfo` | `ResponsiveInt?` | 响应式列数配置。为空时使用控件内置断点默认列数。 |
| `Header` / `HeaderTemplate` | `object?` / `IDataTemplate?` | 描述列表标题区域内容及模板。 |
| `Extra` / `ExtraTemplate` | `object?` / `IDataTemplate?` | 标题行右侧辅助内容及模板。 |
| `Layout` | `Orientation` | 描述项 label/content 排列方向，默认 `Horizontal`。 |
| `SizeType` | `SizeType` | 展示尺寸，默认 `Large`，支持 `Large/Middle/Small` 三种主题分支。 |
| `ItemsSource` | `IEnumerable?` | 外部描述项数据源。当前实现只物化其中的 `DescriptionItem`。 |
| `Items` | `DescriptionItems` | XAML 内容集合入口，也是布局算法的主数据源。 |

描述项模型：

| API | 类型 | 语义 |
| --- | --- | --- |
| `DescriptionItem.Label` | `string` | 字段标签文本。 |
| `DescriptionItem.Content` | `object?` | 字段内容，允许普通文本或复杂 Avalonia 内容。 |
| `DescriptionItem.IsFilled` | `bool` | 当前项是否填满本行剩余列。 |
| `DescriptionItem.Span` | `ResponsiveInt` | 当前项跨列数，默认 `1`，支持响应式配置。 |

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `HeaderLayout` | `DockPanel` | Header/Extra 行容器，固定存在，通过 `IsHeaderLayoutVisible` 控制显示。 |
| `ExtraPresenter` | `ContentPresenter` | Extra 内容和模板承载。 |
| `HeaderPresenter` | `ContentPresenter` | Header 内容和模板承载。 |
| `ContentFrame` | `Border` | 内容区域边框、圆角和裁剪承载。 |
| `PART_GridLayout` | `Grid` | 生成的描述项视觉子控件布局容器。 |

Descriptions 没有 public routed event、命令或专用伪类。内部生成的 `DescriptionDefaultItem`、`DescriptionBorderedItemLabel` 和 `DescriptionBorderedItemContent` 是主题承载类型，不是公开用户 API。

## 4. 行为与状态模型

Descriptions 的核心状态流：

```text
ItemsSource / Items
      ↓
DescriptionItem collection
      ↓
generated item controls
      ↓
responsive column count + item span
      ↓
PART_GridLayout row/column/column-span
```

列数解析：

- `ColumnInfo` 为空时，断点默认列数为 `ExtraSmall=1`、`Small=2`、`ExtraExtraExtraLarge=4`，其他断点为 `3`。
- `ColumnInfo` 不为空时，按 `ResponsiveInt.Resolve()` 使用移动端优先级解析列数。
- 水平边框模式下内部有效列数为展示列数的两倍，因为 label 和 content 分别占 grid cell。
- 非水平边框模式下内部有效列数等于展示列数。

布局语义：

- 普通水平模式使用一个 `DescriptionDefaultItem` 展示 label、冒号和 content。
- 水平边框模式为每个 item 生成一个 label cell 和一个 content cell。
- 纵向模式使用一个 `DescriptionDefaultItem`，label 在上、content 在下；当 `IsBordered=true` 时通过模板显示内部边框和分隔线。
- `Span` 限制在当前行剩余列范围内，最小为 `1`。
- 最后一个 item 或 `IsFilled=true` 的 item 会填满当前行剩余列。

Header/Extra 状态：

```text
Header != null || Extra != null
      ↓
IsHeaderLayoutVisible
      ↓
HeaderLayout.IsVisible
```

## 5. 视觉与主题模型

Descriptions 的默认视觉由根主题、默认项主题和边框 cell 主题组成。

| 主题文件 | 职责 |
| --- | --- |
| `DescriptionsTheme.axaml` | 根模板、Header/Extra、ContentFrame、`PART_GridLayout`、边框模式内容框和非边框 RowSpacing。 |
| `DescriptionDefaultItemTheme.axaml` | 普通项 horizontal/vertical/vertical bordered 三种模板和冒号、label、content 视觉。 |
| `DescriptionBorderedItemLabelTheme.axaml` | 水平边框模式 label cell 视觉。 |
| `DescriptionBorderedItemContentTheme.axaml` | 水平边框模式 content cell 视觉。 |
| `DescriptionsThemes.axaml` | 汇总 Descriptions 相关主题资源。 |

Token 关系：

```text
SharedToken
   ↓
DescriptionsToken
   ↓
DescriptionsTheme / DescriptionDefaultItemTheme / bordered cell themes
```

根 `ContentFrame` 的边框、圆角、裁剪来自 SharedToken。label 背景、label/content/title/extra 颜色、Header margin、item padding 和冒号 margin 来自 DescriptionsToken。

## 6. 控件家族或集成关系

Descriptions 是 Data Display 分类下的独立展示控件，不属于输入控件家族，不实现 Form、CompactSpace、选择、弹层或 motion 接口。

集成关系：

- Media Query：通过 `IMediaBreakAwareControl` owner 感知断点变化，并重新计算列数和布局。
- Token 系统：通过 `DescriptionsToken.ScopeProvider` 注册控件 Token 资源作用域。
- Gallery：通过 Descriptions ShowCase 展示基础、边框、尺寸、响应式、纵向、纵向边框和整行填充。
- `ResponsiveInt`：同时用于控件列数和单个 item 的 span。

## 7. 兼容性不变量

维护 Descriptions 时必须保持以下不变量：

- `Items` 是布局主数据源，`ItemsSource` 只物化其中的 `DescriptionItem`。
- `DescriptionItem` 是 record，布局必须按集合位置定位生成视觉，不能用值相等查找 item index。
- `IsBordered` 或 `Layout` 改变时，必须按新视觉模式重建生成子控件。
- 水平边框模式必须为每个 item 生成 label/content 两个 cell。
- 普通水平和纵向非边框模式必须保持 `IsShowColon` 到冒号显示状态的绑定。
- `HeaderLayout` 是固定模板节点，Header/Extra 为空时隐藏而不是销毁。
- `PART_GridLayout`、`HeaderLayout`、`HeaderPresenter`、`ExtraPresenter`、`ContentFrame` 的名称和职责不能在未授权情况下改变。
- `SizeType` 默认值保持 `Large`。
- Token 名称和语义不擅自重命名或删除。
- 媒体断点订阅必须在 detach 时释放。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

## 8. 专项模型

### 8.1 响应式列数模型

`ColumnInfo` 使用 `ResponsiveInt` 表达断点列数。控件附加到视觉树时查找最近的 `IMediaBreakAwareControl`，并在断点变化时重新布局。

### 8.2 Span 与填充模型

`DescriptionItem.Span` 表达当前 item 的目标跨列数。布局时先解析当前断点下的 span，再限制到当前行剩余列。`IsFilled=true` 和最后一个 item 会直接填满当前行剩余列。

### 8.3 边框模式模型

水平边框模式把 label 和 content 拆成两个 cell，因此内部 grid 列数加倍。content cell 的 span 计算为 `Span * 2 - 1`，扣除 label cell 后覆盖对应内容区域。

纵向边框模式不拆分 cell，而是在 `DescriptionDefaultItem` 模板内显示 label/content 分隔线，并通过 `IsLastRow`、`IsLastColumn` 计算有效边框厚度。

## 9. 文档导航与验证策略

关联文档：

- [Descriptions 桌面版实现原理](implementation.md)
- [Descriptions Token 设计](token.md)
- [Descriptions Changelog](changelog.md)

验证策略：

| 层次 | 验证内容 |
| --- | --- |
| Public API | `IsBordered`、`IsShowColon`、`ColumnInfo`、`Header`、`Extra`、`Layout`、`SizeType`、`ItemsSource`、`Items` 和 `DescriptionItem` 属性语义。 |
| 状态行为 | 集合增删清空、Items 替换、Header/Extra 显隐、媒体断点变化、边框切换、布局切换、冒号绑定。 |
| AXAML / Template | 稳定 template part、固定 HeaderLayout、ContentFrame 边框、默认项三种模板和水平边框 cell 模板。 |
| Token | label 背景/颜色、标题/内容/extra 颜色、Header margin、item padding、冒号 margin。 |
| Gallery | Basic、Border、Custom Size、Responsive、Vertical、Vertical Border、Row 示例。 |
| 文档 | 运行 `git diff --check`，检查相对链接存在。 |
