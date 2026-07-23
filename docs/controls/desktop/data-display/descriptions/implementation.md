# Descriptions 桌面版实现原理

本文档描述 Descriptions 桌面版的数据物化、媒体断点订阅、生成视觉子项、Grid 布局算法、边框状态同步和维护边界。公共设计与 API 契约见 [Descriptions 桌面版架构设计](overview.md)，Token 语义见 [Descriptions Token 设计](token.md)，变化记录见 [Descriptions Changelog](changelog.md)。

## 1. 实现定位

Descriptions 的实现重点是把 `DescriptionItem` 集合转换为一组内部控件，并根据响应式列数、布局方向、边框模式、span 和填充规则写入 `PART_GridLayout` 的 row、column 和 column span。

本文档只描述 Descriptions 相关实现结构，不重新说明 `Grid`、`ResponsiveInt`、`TemplatedControl` 或 Token 系统的通用机制。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/Descriptions/Descriptions.cs`：公共 API、Items 集合、ItemsSource 物化、媒体断点订阅、生成视觉子项和布局算法。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionItem.cs`：非视觉 `AvaloniaObject` 描述项模型和 `DescriptionItems` 集合类型。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionDefaultItem.cs`：普通项和纵向边框项的内部承载控件。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionBorderedCell.cs`：水平边框模式 label/content cell 的共享内部基类。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionBorderedItemLabel.cs`：水平边框模式 label cell 类型。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionBorderedItemContent.cs`：水平边框模式 content cell 类型。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionsToken.cs`：Descriptions 组件 Token。
- `src/AtomUI.Desktop.Controls/Descriptions/Themes/*.axaml`：根模板、普通项模板、边框 cell 模板和 token 样式。

## 3. 核心类职责

`Descriptions` 是状态协调器。它持有 `Items` 集合，监听集合变化，感知媒体断点，生成内部视觉子控件，并执行 Grid 布局。

`DescriptionItem` 是非视觉 `AvaloniaObject` 描述项模型。它承载 item 级 Avalonia 属性，但不进入视觉树；属性绑定、属性变化订阅和生成视觉映射都由 owning `Descriptions` 管理。

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
  Add    → attach item property subscriptions
         → insert generated controls at grid child index
  Remove → detach item property subscriptions
         → remove generated controls by original collection index
  Reset  → detach all item property subscriptions
         → clear generated controls and item mappings
  Move / Replace → NotSupportedException
  → DoLayoutChildren()
  → InvalidateMeasure()
```

DescriptionItem 属性流：

```text
DescriptionItem.Label / Content changed
  → update generated label/content presenter

DescriptionItem.Span / IsFilled changed
  → DoLayoutChildren()
  → InvalidateMeasure()
```

`DescriptionItem` 不参与视觉树，因此它的 property changed handler 不能反向持有旧生成视觉。生成视觉重建前必须先解除旧 item 订阅并清空 item 到 visual 的映射。

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
- 对默认 `Items` 中已有 item 建立属性变化订阅时，必须与集合替换、reset、detach 或模板重建释放路径配对。

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

`DescriptionItem` 生命周期：

- item 进入 `Items` 时，由 `Descriptions` 订阅该 item 的 Avalonia property changed。
- item 离开 `Items`、集合 reset、`Items` 替换、模板重新应用或控件 detach 时，必须解除该 item 的属性订阅。
- 生成视觉重建时，必须先释放旧 generated control 与 item 的映射，再按当前布局模式建立新映射。
- 任何从 item 属性转接到 generated control 的 binding、事件或 disposable，都必须归属同一个 item attach 生命周期。

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
- `DescriptionItem.Label` / `Content`：更新对应 generated control 的 label/content。
- `DescriptionItem.Span` / `IsFilled`：重新计算布局和测量。

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

布局时必须使用 `for` 循环中的 `itemIndex`。不能使用 `Items.IndexOf(item)` 作为布局主路径，因为生成视觉的顺序语义来自集合位置；item 对象引用只用于维护当前生成周期内的订阅和映射。

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
- `DescriptionItem` 是非视觉 `AvaloniaObject`，只能承载 item 级数据属性和普通 binding target 语义。
- `IsShowColon`、`IsBordered` 和 `SizeType` 的内部同步使用 Avalonia property binding，不使用 `BindUtils.RelayBind`。
- 媒体断点事件订阅由 `OnAttachedToVisualTree` / `OnDetachedFromVisualTree` 配对管理。
- 不在 `Render`、layout hot path 或数据物化路径中读取全局资源。

### 8.1 非视觉 DescriptionItem 的泄露边界

`DescriptionItem` 继承 `AvaloniaObject` 后，binding expression、property changed subscription 和资源查找都有了真实生命周期成本。实现必须把 acquire/release 写成对称路径，不能只依赖 GC 或视觉树清理。

需要防范的泄露链：

```text
DescriptionItem.PropertyChanged subscription
  → Descriptions / generated control
  → old ShowCase visual tree
```

```text
generated control / presenter binding
  → old DescriptionItem
  → old owner / content visual
```

```text
Application.ResourcesChanged
  → DynamicResourceExpression
  → DescriptionItem.ValueStore
  → owner / generated control / ShowCase
```

标准防范规则：

- item attach 时注册属性变化订阅，item detach 时解除同一个订阅。
- generated control 的 binding、事件和 disposable 归属 generated control 或 item attach 生命周期；视觉重建前必须统一释放。
- item 到 generated control 的映射只服务当前模板和布局周期；生成视觉清理路径必须同时清理映射和 item 订阅。
- 主题资源优先绑定在生成出来的 Visual 控件上，让视觉树生命周期管理 `DynamicResource`。
- `DescriptionItem` 上禁止直接放 `DynamicResource` 或 token-resource binding，除非它实现 scoped `IResourceHost` / `IThemeVariantHost`，并有 owner attach/detach 与 WeakReference 测试。

### 8.2 DynamicResource Scoped Host 规则

如果 `DescriptionItem` 必须承载动态资源，它必须作为 owning `Descriptions` 的 scoped resource host：

- `TryGetResource()` 先查询 owning `Descriptions`，再 fallback 到 `Application.Current`。
- owner 变化时先 unsubscribe 旧 owner 的 `ResourcesChanged` / `ActualThemeVariantChanged`，再保存并订阅新 owner。
- owner detach、Items remove、Items reset、template reapply 和控件 detach 都必须能释放 owner 订阅。
- owner 资源和主题变化需要通过 `DescriptionItem` 转发，确保动态资源仍能更新。
- 必须补 WeakReference 测试，模拟 XAML `DynamicResource` anchor，证明 item 离开集合后不会被 `Application.ResourcesChanged` root 住。

AOT 边界：

- 不新增字符串 path binding、反射扫描、`Activator.CreateInstance(Type)` 或动态成员访问。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护。
- `ItemsSource` 只接受已经构造好的 `DescriptionItem`，不做基于反射的对象属性展开。

## 9. 维护不变量

内部重构必须保持以下不变量：

- `Items` 集合订阅必须在集合替换时成对解除和重新订阅。
- `DescriptionItem` 属性订阅必须在 item remove、reset、Items 替换、template reapply 和 detach 时释放。
- item 到 generated control 的映射必须随生成视觉重建清理，不能保留旧视觉控件。
- `MediaBreakPointChanged` 必须在 detach 时解除。
- `IsBordered` 和 `Layout` 变化必须重建生成视觉。
- 水平边框模式的 grid child 数量必须是 `Items.Count * 2`。
- 非水平边框模式的 grid child 数量必须是 `Items.Count`。
- 布局必须按集合位置定位 item，不能按值查找。
- `IsFilled` 和最后 item 必须填满当前行剩余列。
- `IsShowColon` 必须能在生成视觉存在期间重复更新。
- `HeaderLayout` 固定存在，通过 `IsHeaderLayoutVisible` 控制显示。
- 内部 marker 类型 `DescriptionBorderedItemLabel` 和 `DescriptionBorderedItemContent` 不能作为“空类”随意删除；它们承担主题选择器边界。
- `DescriptionItem` 不能升级成视觉控件，也不能永久持有 generated control、owner container 或 ShowCase。
- 非视觉 `DescriptionItem` 承载动态资源前必须补 scoped resource host 和资源生命周期测试。

## 10. 测试与验证

验证范围：

- `DescriptionsResponsiveLayoutTests`：响应式列数、span 解析、边框切换重建视觉、重复内容 item 按位置布局。
- `--verify-descriptions-states`：Header/Extra 显隐、集合生命周期、item 属性变化、布局和边框切换、冒号绑定、媒体断点订阅释放。
- 生命周期测试：item remove/reset/Items 替换/template reapply/detach 后，旧 `DescriptionItem`、旧 generated control、binding expression 和资源订阅不被保留。
- 如果允许 `DescriptionItem` 使用 `DynamicResource`，必须增加 WeakReference 测试、owner resource 优先级测试和 resource update 测试。
- Gallery Descriptions 示例：Basic、Border、Custom Size、Responsive、Vertical、Vertical Border、Row。
- 修改 Token 或主题时验证 源码片段和 Gallery 示例视觉。
- 修改控件实现时运行 `tests/AtomUI.Desktop.Controls.Tests` 中的 Descriptions 相关测试，并按影响范围扩大到完整 Desktop 控件测试。
- 文档改动运行 `git diff --check`，并检查相对链接存在。
