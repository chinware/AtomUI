# Descriptions 桌面版实现原理

本文档描述 Descriptions 桌面版的数据物化、媒体断点订阅、生成视觉子项、Grid 布局算法、边框状态同步和维护边界。公共设计与 API 契约见 [Descriptions 桌面版架构设计](overview.md)，Token 语义见 [Descriptions Token 设计](token.md)，变化记录见 [Descriptions Changelog](changelog.md)。

## 1. 实现定位

Descriptions 的实现重点是把 `DescriptionItem` 集合转换为一组内部控件，并根据响应式列数、布局方向、边框模式、span 和填充规则写入 `PART_GridLayout` 的 row、column 和 column span。

本文档只描述 Descriptions 相关实现结构，不重新说明 `Grid`、`ResponsiveInt`、`TemplatedControl` 或 Token 系统的通用机制。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/Descriptions/Descriptions.cs`：公共 API、Items 集合、ItemsSource 物化、媒体断点订阅、生成视觉子项和布局算法。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionItem.cs`：`DescriptionItem` 数据模型和 `DescriptionItems` 集合类型。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionDefaultItem.cs`：普通项和纵向边框项的内部承载控件。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionBorderedCell.cs`：水平边框模式 label/content cell 的共享内部基类。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionBorderedItemLabel.cs`：水平边框模式 label cell 类型。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionBorderedItemContent.cs`：水平边框模式 content cell 类型。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionsToken.cs`：Descriptions 组件 Token。
- `src/AtomUI.Desktop.Controls/Descriptions/Themes/*.axaml`：根模板、普通项模板、边框 cell 模板和 token 样式。

## 3. 核心类职责

`Descriptions` 是状态协调器。它持有 `Items` 集合，监听集合变化，感知媒体断点，生成内部视觉子控件，并执行 Grid 布局。

`DescriptionItem` 是描述项数据模型。它是 record，因此值相等不代表集合位置相同。布局实现必须使用循环位置定位生成视觉。

`DescriptionDefaultItem` 是普通布局和纵向边框布局的内部控件。它承载 Header/Content、冒号显示、布局方向、边框状态和有效边框厚度。

`DescriptionBorderedCell` 是水平边框模式 label/content cell 的共享内部基类，负责尺寸、行高、最后行/列状态和有效边框厚度。

`DescriptionBorderedItemLabel` 和 `DescriptionBorderedItemContent` 是主题区分 label/content 的 marker 类型。它们没有额外逻辑，但其类型名称是主题选择器和视觉语义边界。

## 4. 状态与数据流

ItemsSource 流：

```text
ItemsSource changed
  → if ItemsSource != null
  → Items.Clear()
  → CollectItemsSourceItems()
  → Items.AddRange(description items)
  → collection changed
  → generated controls + layout
```

当前实现只在 `ItemsSource` 非 null 时重新物化数据。把 `ItemsSource=null` 改成清空 `Items` 属于可观察行为变化，不能混入结构优化。

Items 集合流：

```text
Items.CollectionChanged
  Add    → insert generated controls at grid child index
  Remove → remove generated controls by original collection index
  Reset  → clear generated controls
  Move / Replace → NotSupportedException
  → DoLayoutChildren()
  → InvalidateMeasure()
```

媒体断点流：

```text
OnAttachedToVisualTree
  → MediaQueryHost.FindOwner(this)
  → subscribe MediaBreakPointChanged
  → resolve columns
  → layout

OnDetachedFromVisualTree
  → unsubscribe MediaBreakPointChanged
```

生成视觉流：

```text
Layout=Horizontal && IsBordered=true
  → DescriptionBorderedItemLabel + DescriptionBorderedItemContent

otherwise
  → DescriptionDefaultItem
```

## 5. 生命周期与模板接入

构造阶段：

- 注册 `DescriptionsToken.ScopeProvider`。
- 订阅默认 `Items.CollectionChanged`。

附加到视觉树：

- 查找最近的 `IMediaBreakAwareControl`。
- 保存 `_mediaOwner` 和 `_breakPoint`。
- 订阅 `MediaBreakPointChanged`。
- 根据当前断点更新列数并布局。

脱离视觉树：

- 解除 `_mediaOwner.MediaBreakPointChanged`。
- 清空 `_mediaOwner`。

模板接入：

- `OnApplyTemplate()` 获取 `PART_GridLayout`。
- 根据当前 `Items` 重建生成视觉。
- 根据当前断点更新列数并布局。

`Items` 属性替换时必须从旧集合解绑，再订阅新集合；如果模板已经接入且控件附加到视觉树，则清空旧视觉、生成新视觉并重新布局。

## 6. 交互与事件处理

Descriptions 不处理 pointer、keyboard、focus、drag/drop、popup 或 command 事件。

实现中需要维护的事件路径只有：

- `Items.CollectionChanged`
- `IMediaBreakAwareControl.MediaBreakPointChanged`
- Avalonia property changed

property changed 分发：

- `ItemsSource`：非 null 时物化到 `Items`。
- `IsBordered`：水平模式下重建生成视觉，并重新计算列数与布局。
- `Header` / `Extra`：同步 `IsHeaderLayoutVisible`。
- `ColumnInfo`：当前有断点时重新计算列数与布局。
- `Layout`：控件已附加到视觉树时重建生成视觉，并重新布局。

## 7. 内部算法与关键流程

### 7.1 有效列数

控件先解析展示列数：

```text
columns = ColumnInfo?.Resolve(currentBreakPoint, fallback)
       ?? fallback
```

默认 fallback：

```text
ExtraSmall = 1
Small = 2
ExtraExtraExtraLarge = 4
others = 3
```

再转换为内部有效列数：

```text
Layout=Horizontal && IsBordered
  → effectiveColumns = columns * 2
otherwise
  → effectiveColumns = columns
```

### 7.2 生成子控件索引

Grid 子控件索引由当前布局模式决定：

```text
horizontal bordered: itemIndex * 2
otherwise: itemIndex
```

布局时必须使用 `for` 循环中的 `itemIndex`。不能使用 `Items.IndexOf(item)`，因为 `DescriptionItem` 是 record，两个不同位置的值相等 item 会返回同一个 index。

### 7.3 水平边框布局

每个 item 生成 label cell 和 content cell：

```text
label cell:
  row = current row
  column = current column
  column += 1

content cell:
  span = GetItemSpan(item.Span) * 2 - 1
  span = clamp(span, 1, effectiveColumns - column)
  if last item or IsFilled: span = effectiveColumns - column
  column += span
```

当 `column >= effectiveColumns` 时，当前 content cell 标记为最后列，进入下一行。

### 7.4 普通和纵向布局

普通水平、纵向非边框和纵向边框都使用 `DescriptionDefaultItem`。布局算法对这些模式使用相同的 span 计算：

```text
span = GetItemSpan(item.Span)
span = clamp(span, 1, effectiveColumns - column)
if last item or IsFilled: span = effectiveColumns - column
```

纵向布局还会同步 `IsLastColumn` 和 `IsLastRow`，用于 `DescriptionDefaultItem` 计算有效边框厚度。

### 7.5 最后一行和边框厚度

布局完成后，控件遍历生成视觉并根据 `Grid.GetRow(control) == rowCount - 1` 标记最后行。

内部 item 根据 `IsLastRow`、`IsLastColumn` 和 `BorderThickness` 计算 `EffectiveBorderThickness`：

- 非最后行、非最后列：保留右边和下边。
- 非最后行、最后列：保留下边。
- 最后行、非最后列：保留右边。
- 最后行、最后列：不保留内部边框。

### 7.6 生成视觉重建

`IsBordered` 和 `Layout` 会改变生成视觉的类型和数量，因此不能只重新布局。水平边框模式和普通模式之间切换时必须先清空 `PART_GridLayout.Children`，再按当前 `Items` 重建视觉。

## 8. 资源、性能与 AOT 边界

Descriptions 不依赖运行时反射发现模板结构。模板协作通过固定 part 名称、显式内部类型和 Avalonia property binding 完成。

资源与生命周期边界：

- `DescriptionsToken` 通过 token generator 注册，Theme 通过 `DescriptionsTokenResource` 使用。
- 生成的内部控件是 Visual，Token resource 由主题和视觉树生命周期管理。
- `IsShowColon`、`IsBordered` 和 `SizeType` 的内部同步使用 Avalonia property binding，不使用 `BindUtils.RelayBind`。
- 媒体断点事件订阅由 `OnAttachedToVisualTree` / `OnDetachedFromVisualTree` 配对管理。
- 不在 `Render`、layout hot path 或数据物化路径中读取全局资源。

AOT 边界：

- 不新增字符串 path binding、反射扫描、`Activator.CreateInstance(Type)` 或动态成员访问。
- Gallery API/Token 表使用显式 ViewModel 数据。
- `ItemsSource` 只接受已经构造好的 `DescriptionItem`，不做基于反射的对象属性展开。

## 9. 维护不变量

内部重构必须保持以下不变量：

- `Items` 集合订阅必须在集合替换时成对解除和重新订阅。
- `MediaBreakPointChanged` 必须在 detach 时解除。
- `IsBordered` 和 `Layout` 变化必须重建生成视觉。
- 水平边框模式的 grid child 数量必须是 `Items.Count * 2`。
- 非水平边框模式的 grid child 数量必须是 `Items.Count`。
- 布局必须按集合位置定位 item，不能按值查找。
- `IsFilled` 和最后 item 必须填满当前行剩余列。
- `IsShowColon` 必须能在生成视觉存在期间重复更新。
- `HeaderLayout` 固定存在，通过 `IsHeaderLayoutVisible` 控制显示。
- 内部 marker 类型 `DescriptionBorderedItemLabel` 和 `DescriptionBorderedItemContent` 不能作为“空类”随意删除；它们承担主题选择器边界。

## 10. 测试与验证

验证范围：

- `DescriptionsResponsiveLayoutTests`：响应式列数、span 解析、边框切换重建视觉、值相等 item 按位置布局。
- `--verify-descriptions-states`：Header/Extra 显隐、集合生命周期、布局和边框切换、冒号绑定、媒体断点订阅释放。
- Gallery Descriptions 示例：Basic、Border、Custom Size、Responsive、Vertical、Vertical Border、Row。
- 修改 Token 或主题时验证 API 表、Token 表和 Gallery 示例视觉。
- 修改控件实现时运行 `tests/AtomUI.Desktop.Controls.Tests` 中的 Descriptions 相关测试，并按影响范围扩大到完整 Desktop 控件测试。
- 文档改动运行 `git diff --check`，并检查相对链接存在。
