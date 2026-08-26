# Masonry 桌面版实现原理

本文档描述 Masonry 桌面版的内部布局引擎、响应式状态解析、item container 元数据和布局变化通知。公共设计与 API 契约见 [Masonry 桌面版架构设计](overview.md)，变化记录见 [Masonry Changelog](changelog.md)。

## 1. 实现定位

Masonry 的实现由公开 `ItemsControl` 外壳和 internal `Panel` 布局引擎组成。`Masonry` 暴露布局属性、attached property 和事件；`MasonryPanel` 负责实际 `MeasureOverride` / `ArrangeOverride`。

实现文档聚焦布局计算、响应式订阅、container 元数据读取和事件派发，不描述子项控件自身的渲染、图片加载或业务状态。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/Masonry/Masonry.cs`：公开控件类型、布局属性、attached property、`LayoutChanged` 事件入口。
- `src/AtomUI.Desktop.Controls/Masonry/MasonryPanel.cs`：internal 布局引擎，执行测量、排列、响应式断点监听和布局结果比较。
- `src/AtomUI.Desktop.Controls/Masonry/MasonryItemSpan.cs`：子项 span 枚举。
- `src/AtomUI.Desktop.Controls/Masonry/MasonryLayoutChangedEventArgs.cs`：布局结果事件参数。
- `src/AtomUI.Desktop.Controls/Masonry/Themes/MasonryTheme.axaml`：默认 ControlTheme，装配 `ItemsPresenter` 和 `MasonryPanel`。

## 3. 核心类职责

`Masonry` 是公共 API 宿主，不直接计算子项矩形。它定义 `ColumnCount`、`ColumnInfo`、`MinColumnWidth`、`MaxColumnCount`、`ColumnGap`、`RowGap`、`Gutter` 和 `Masonry.Column` / `Masonry.Span`。

`MasonryPanel` 是默认布局引擎。它只读取自身 `Children` 中的 item container，不进入 `ItemTemplate` 内部查找 attached property。这样直接子元素和 `ItemsSource` 两种模式可以共享同一布局路径。

`MasonryLayoutChangedEventArgs` 表达有效布局分配，不表达像素矩形。业务 key 由用户数据模型维护，事件只暴露 item container、顺序索引、有效列和整行状态。

## 4. 状态与数据流

布局状态流：

```text
Masonry public properties
  ColumnCount / ColumnInfo / MinColumnWidth / MaxColumnCount
  ColumnGap / RowGap / Gutter
      ↓ theme binding
MasonryPanel properties
      ↓
Resolve effective breakpoint values
      ↓
Measure children and build layout rect cache
      ↓
Arrange children
      ↓
Compare effective assignment snapshot
      ↓
Post LayoutChanged outside layout pass
```

响应式属性只在当前断点命中显式配置时覆盖兼容属性。`ColumnInfo` 未命中时回退 `ColumnCount` 或容器自适应列数；`Gutter` 未命中或某个维度未声明时回退 `ColumnGap` / `RowGap`。

## 5. 生命周期与模板接入

`MasonryTheme.axaml` 通过默认 `ItemsPanel` 装配 `MasonryPanel`。布局属性通过 binding 从 `Masonry` 传递到 `MasonryPanel`，避免在 `Masonry` 中重复布局计算。

`MasonryPanel` 在进入可用视觉树后查找最近的媒体断点宿主，订阅断点变化并触发 `InvalidateMeasure`。宿主替换、detached 或控件释放时必须解除订阅。

Masonry 不 override `ItemsControl` 的 `NeedsContainer`、`CreateContainer`、`PrepareContainer` 等容器生成方法。两种内容提供方式的容器层级由 Avalonia 基类决定。

## 6. 交互与事件处理

Masonry 不拦截 pointer、keyboard、focus、drag/drop、context menu 或子项动画事件。所有交互保留给子项控件和外层页面。

`LayoutChanged` 派发必须避开 Avalonia layout pass。布局过程中只记录最新有效分配快照；需要通知时通过 dispatcher 或等价机制延后派发，避免用户 handler 修改属性造成 layout re-entry。

空集合也是有效布局状态。从非空布局变为空布局时派发一次空结果；布局持续为空时不重复派发。

## 7. 内部算法与关键流程

布局算法由 Avalonia `MeasureOverride` 与 `ArrangeOverride` 驱动：

```text
MeasureOverride(availableSize)
  resolve width
  resolve effective columns and gaps
  for each visible child in order:
    resolve span and explicit column
    measure child with column width or full width
    compute rect using shortest-column state
  cache rects for arrange
  return Size(resolvedWidth, maxColumnHeight)

ArrangeOverride(finalSize)
  reuse the Measure layout when the effective width is unchanged
  recompute only when no compatible Measure result exists or width changed
  arrange each child using cached rect
  return finalSize
```

`MeasureOverride` 保存本次测量生成的完整 `MasonryLayout`（矩形、列分配、整行标记、有效宽度）。`ArrangeOverride` 先解析最终尺寸对应的有效宽度；如果与 Measure 结果一致，直接复用该结果，不再重复遍历 item 和列高集合。宽度变化、断点变化或下一次 Measure 会使缓存失效。缓存只跨越当前 Measure→Arrange 周期，Arrange 消费后立即释放，避免保留已脱离视觉树的 item container。

有效列数：

```text
if ColumnInfo hits current breakpoint:
  EffectiveColumnCount = ColumnInfo
else if ColumnCount > 0:
  EffectiveColumnCount = ColumnCount
else:
  EffectiveColumnCount = floor((AvailableWidth + EffectiveColumnGap) / (MinColumnWidth + EffectiveColumnGap))
  EffectiveColumnCount = clamp(EffectiveColumnCount, 1, MaxColumnCount)
```

有效列宽：

```text
EffectiveColumnWidth =
  (AvailableWidth - EffectiveColumnGap * (EffectiveColumnCount - 1)) / EffectiveColumnCount
```

排列规则：

1. 按 `Children` 顺序处理子元素。
2. 不可见子元素不参与列高计算。
3. 普通子元素以有效列宽、无限高度测量。
4. `Masonry.Span=Full` 子元素以完整有效宽度、无限高度测量。
5. 指定 `Masonry.Column` 的子元素放入有效列；未指定列的子元素放入当前最短列。
6. 多列等高时选择索引最小列。
7. 整行项从当前最高列之后开始，并将所有列高度同步到该子项底部。

Measure 与 Arrange 的布局结果必须对同一输入保持一致。缓存不能跨越影响布局的属性变更、子元素变更或可用宽度变更。

## 8. 资源、性能与 AOT 边界

Masonry 不使用反射读取 item template 内部元素，不创建不可见测量控件，不通过透明元素扩展命中区域。

响应式 resolver 必须保持纯计算属性，不依赖 VisualTree、主题资源或控件实例默认值。控件级 fallback 始终保留在 Masonry effective state 层。

布局变化通知比较维度只包含 item container 数量、顺序、有效列和 `IsFullSpan`。不把像素矩形作为事件比较维度，可以避免宽度变化、字体渲染或子项高度细微变化造成高频事件。

## 9. 维护不变量

内部重构必须保持以下不变量：

- `MasonryPanel` 保持 internal。
- `MasonryPanel` 只读取直接 child 上的 `Masonry.Column` 和 `Masonry.Span`。
- 直接子元素模式不额外包装子项。
- `ItemsSource` 模式通过基类生成 `ContentPresenter`，Masonry 不重写容器生成。
- 响应式断点变化只触发布局失效，不在断点回调中执行完整布局或派发事件。
- Measure→Arrange 同一有效宽度必须复用已测量的布局结果；不得在正常布局周期中无条件重复执行第二次 `O(items × columns)` 计算。
- 布局缓存不得跨越宽度、断点或下一次 Measure；Arrange 消费后必须释放缓存引用。
- `LayoutChanged` 不在 layout pass 内同步派发。
- 替换 `ItemsPanel` 等价于替换布局引擎，Masonry-specific 布局语义不再由默认面板保证。

## 10. 测试与验证

验证范围：

- 默认值和无效值归一：列数、列宽、最大列数、水平/垂直间距。
- 响应式：`ColumnInfo`、`Gutter`、partial breakpoint map、水平/垂直维度独立 fallback。
- 布局：shortest-column、显式列、整行项、不可见子项、无限宽度、tie-break。
- 容器：直接子元素、`ItemsSource`、`ItemContainerTheme`、替换 `ItemsPanel`。
- 事件：有效布局分配变化、空集合通知、重复通知合并、layout pass 外派发。
- Gallery：图片加载、异步高度变化、响应式示例和整行项示例。
- 文档改动：运行 `git diff --check`。
