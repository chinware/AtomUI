# DataGrid Design Token 参考

## 1. 概述

DataGrid 的 Design Token 定义位于 `src/AtomUI.Desktop.Controls.DataGrid/DataGridToken.cs`，类名为 `DataGridToken`，继承自 `AbstractControlDesignToken`。Token ID 为 `"DataGrid"`。

所有 Token 值在 `CalculateTokenValues(bool isDarkMode)` 方法中根据 `SharedToken`（全局共享 Token）计算得出，自动适配亮色/暗色模式。

### Token 资源引用方式

在 AXAML 中通过 `DataGridTokenResourceExtension` 引用 Token：

```xml
<Border Background="{DynamicResource DataGridTokenResourceExtension.HeaderBg}" />
```

在代码中通过 `TokenResourceExtension` 获取：

```csharp
var headerBg = tokenResourceProvider.GetTokenValue<Color>("DataGrid", "HeaderBg");
```

---

## 2. 公共 Token

### 表头相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `HeaderBg` | `Color` | `ColorUtils.OnBackground(SharedToken.ColorFillAlter, SharedToken.ColorBgContainer)` | 表头背景色 |
| `HeaderColor` | `Color` | `SharedToken.ColorTextHeading` | 表头文字颜色 |
| `HeaderSortActiveBg` | `Color` | `ColorUtils.OnBackground(SharedToken.ColorFillSecondary, SharedToken.ColorBgContainer)` | 排序激活态表头背景色 |
| `HeaderSortHoverBg` | `Color` | `ColorUtils.OnBackground(SharedToken.ColorFillContent, SharedToken.ColorBgContainer)` | 排序悬浮态表头背景色 |
| `HeaderSplitColor` | `Color` | `SharedToken.ColorSplit` | 表头分割线颜色 |
| `HeaderBorderRadius` | `CornerRadius` | `SharedToken.BorderRadiusLG` | 表头圆角 |
| `HeaderFilterHoverBg` | `Color` | `SharedToken.ColorFillContent` | 过滤按钮悬浮背景色 |
| `FixedHeaderSortActiveBg` | `Color` | `ColorUtils.OnBackground(SharedToken.ColorFillSecondary, SharedToken.ColorBgContainer)` | 固定表头排序激活态背景色 |

### 表体相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `BodySortBg` | `Color` | `ColorUtils.OnBackground(SharedToken.ColorFillAlter, SharedToken.ColorBgContainer)` | 排序列背景色 |
| `RowHoverBg` | `Color` | `ColorUtils.OnBackground(SharedToken.ColorFillAlter, SharedToken.ColorBgContainer)` | 行悬浮背景色 |
| `RowSelectedBg` | `Color` | `SharedToken.ControlItemBgActive` | 行选中背景色 |
| `RowSelectedHoverBg` | `Color` | `SharedToken.ControlItemBgActiveHover` | 行选中+悬浮背景色 |
| `RowExpandedBg` | `Color` | `SharedToken.ColorFillAlter` | 展开行背景色 |

### 单元格相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `CellPadding` | `Thickness` | `SharedToken.Padding` | 单元格内间距（大尺寸） |
| `CellPaddingMD` | `Thickness` | `SharedToken.PaddingSM` | 单元格内间距（中等尺寸） |
| `CellPaddingSM` | `Thickness` | `SharedToken.PaddingXS` | 单元格内间距（小尺寸） |
| `CellFontSize` | `double` | `SharedToken.FontSize` | 单元格字体大小（大尺寸） |
| `CellFontSizeMD` | `double` | `SharedToken.FontSize` | 单元格字体大小（中等尺寸） |
| `CellFontSizeSM` | `double` | `SharedToken.FontSize` | 单元格字体大小（小尺寸） |

### 边框与分割线

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `BorderColor` | `Color` | `SharedToken.ColorBorderSecondary` | 表格边框/分割线颜色 |

### 表尾相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `FooterBg` | `Color` | `ColorUtils.OnBackground(SharedToken.ColorFillAlter, SharedToken.ColorBgContainer)` | 表尾背景色 |
| `FooterColor` | `Color` | `SharedToken.ColorTextHeading` | 表尾文字颜色 |

### 过滤相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `FilterDropdownMenuBg` | `Color` | `SharedToken.ColorBgContainer` | 过滤下拉菜单选项背景 |
| `FilterDropdownBg` | `Color` | `SharedToken.ColorBgElevated` | 过滤下拉菜单背景 |

### 展开行相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `ExpandIconBg` | `Color` | `SharedToken.ColorBgContainer` | 展开按钮背景色 |

### 选择列相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `SelectionColumnWidth` | `double` | `SharedToken.ControlHeight` | 选择列宽度 |

### 固定列阴影

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `LeftFrozenShadows` | `BoxShadows` | OffsetX=-10, Blur=8, Color=`SharedToken.ColorSplit` | 左侧固定列阴影 |
| `RightFrozenShadows` | `BoxShadows` | OffsetX=10, Blur=8, Color=`SharedToken.ColorSplit` | 右侧固定列阴影 |

### 拖拽排序相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `ColumnReorderActiveBg` | `Color` | `ColorUtils.OnBackground(SharedToken.ColorFillContent, SharedToken.ColorBgContainer)` | 列拖拽排序时当前列背景色 |

### 分页器相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `PaginationMargin` | `Thickness` | `(0, SharedToken.UniformlyMargin, 0, 0)` | 分页器外边距 |
| `PaginationMarginSM` | `Thickness` | `(0, SharedToken.UniformlyMarginXS, 0, 0)` | 分页器外边距（小号） |

---

## 3. 内部 Token

以下 Token 标记为 `internal`，仅供控件内部使用，不建议外部直接引用。

### 展开图标相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `ExpandIconMargin` | `Thickness` | 根据字体行高和线宽计算 | 展开图标外边距 |
| `ExpandIconHalfInner` | `double` | `SharedToken.ControlInteractiveSize / 2 - SharedToken.LineWidth` | 展开图标半内径 |
| `ExpandIconSize` | `double` | `ExpandIconHalfInner * 2 + SharedToken.LineWidth * 3` | 展开图标尺寸 |
| `ExpandIconScale` | `double` | `SharedToken.ControlInteractiveSize / ExpandIconSize` | 展开图标缩放比 |

### 排序图标相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `SortIconSize` | `double` | `SharedToken.FontHeight / 2.5` | 排序图标尺寸 |
| `SortIndicatorLayoutMargin` | `Thickness` | `(SharedToken.UniformlyMarginXS, 0, 0, 0)` | 排序指示器布局边距 |

### 表头图标相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `HeaderIconColor` | `Color` | `SharedToken.ColorIcon * SharedToken.OpacityLoading` | 表头图标颜色 |
| `HeaderIconHoverColor` | `Color` | `SharedToken.ColorIconHover * SharedToken.OpacityLoading` | 表头图标悬浮颜色 |

### 过滤指示器相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `FilterIndicatorPadding` | `Thickness` | `(SharedToken.UniformlyPaddingXXS, SharedToken.UniformlyPaddingXS)` | 过滤指示器内边距 |

### 行拖拽相关

| Token 名 | 类型 | 计算来源 | 说明 |
|----------|------|---------|------|
| `RowReorderIndicatorSize` | `double` | `SharedToken.SizeMD` | 行拖拽指示器尺寸 |

---

## 4. 内部别名 Token

以下 Token 为公共 Token 的别名，提供与 Ant Design Table 规范一致的命名：

| 别名 Token | 对应公共 Token | 说明 |
|------------|---------------|------|
| `TableFontSize` | `CellFontSize` | 表格字体大小 |
| `TableBg` | `SharedToken.ColorBgContainer` | 表格背景色 |
| `TableRadius` | `HeaderBorderRadius` | 表格圆角 |
| `TablePadding` | `CellPadding` | 表格内间距（大尺寸） |
| `TablePaddingMiddle` | `CellPaddingMD` | 表格内间距（中等尺寸） |
| `TablePaddingSmall` | `CellPaddingSM` | 表格内间距（小尺寸） |
| `TableBorderColor` | `BorderColor` | 表格边框颜色 |
| `TableHeaderTextColor` | `HeaderColor` | 表头文字颜色 |
| `TableHeaderBg` | `HeaderBg` | 表头背景色 |
| `TableFooterTextColor` | `FooterColor` | 表尾文字颜色 |
| `TableFooterBg` | `FooterBg` | 表尾背景色 |
| `TableHeaderCellSplitColor` | `HeaderSplitColor` | 表头单元格分割线颜色 |
| `TableHeaderSortBg` | `HeaderSortActiveBg` | 排序态表头背景色 |
| `TableHeaderSortHoverBg` | `HeaderSortHoverBg` | 排序悬浮态表头背景色 |
| `TableBodySortBg` | `BodySortBg` | 排序列背景色 |
| `TableFixedHeaderSortActiveBg` | `FixedHeaderSortActiveBg` | 固定表头排序激活态背景色 |
| `TableHeaderFilterActiveBg` | `HeaderFilterHoverBg` | 过滤按钮激活背景色 |
| `TableFilterDropdownBg` | `FilterDropdownBg` | 过滤下拉菜单背景色 |
| `TableRowHoverBg` | `RowHoverBg` | 行悬浮背景色 |
| `TableSelectedRowBg` | `RowSelectedBg` | 行选中背景色 |
| `TableSelectedRowHoverBg` | `RowSelectedHoverBg` | 行选中悬浮背景色 |
| `TableFontSizeMiddle` | `CellFontSizeMD` | 中等尺寸字体大小 |
| `TableFontSizeSmall` | `CellFontSizeSM` | 小尺寸字体大小 |
| `TableSelectionColumnWidth` | `SelectionColumnWidth` | 选择列宽度 |
| `TableExpandIconBg` | `ExpandIconBg` | 展开图标背景色 |
| `TableExpandColumnWidth` | `SharedToken.ControlInteractiveSize + SharedToken.UniformlyPadding * 2` | 展开列宽度 |
| `TableExpandedRowBg` | `RowExpandedBg` | 展开行背景色 |
| `TableTopLeftColumnCornerRadius` | `CornerRadius(SharedToken.BorderRadiusLG.TopLeft, 0, 0, 0)` | 左上角圆角 |

### 过滤下拉菜单别名

| 别名 Token | 计算来源 | 说明 |
|------------|---------|------|
| `TableFilterButtonSpacing` | `SharedToken.UniformlyMarginXS` | 过滤按钮间距 |
| `TableFilterButtonContainerMargin` | `(0, SharedToken.UniformlyMarginXS, 0, 0)` | 过滤按钮容器边距 |
| `TableFilterButtonLayoutSeparatorMargin` | `(0, SharedToken.UniformlyMarginXXS, 0, 0)` | 过滤按钮分隔线边距 |
| `TableFilterDropdownHeight` | `264` | 过滤下拉菜单高度 |
| `TableFilterDropdownWidth` | `120` | 过滤下拉菜单宽度 |
| `TableFilterDropdownPadding` | `SharedToken.PaddingXS` | 过滤下拉菜单内边距 |
| `TableFilterDropdownSearchWidth` | `140` | 过滤搜索框宽度 |

---

## 5. Token 与 Ant Design 的对应关系

| Ant Design Token | AtomUI Token | 说明 |
|------------------|-------------|------|
| `headerBg` | `HeaderBg` / `TableHeaderBg` | 表头背景 |
| `headerColor` | `HeaderColor` / `TableHeaderTextColor` | 表头文字颜色 |
| `headerSortActiveBg` | `HeaderSortActiveBg` / `TableHeaderSortBg` | 排序态表头背景 |
| `headerSortHoverBg` | `HeaderSortHoverBg` / `TableHeaderSortHoverBg` | 排序悬浮态表头背景 |
| `bodySortBg` | `BodySortBg` / `TableBodySortBg` | 排序列背景 |
| `rowHoverBg` | `RowHoverBg` / `TableRowHoverBg` | 行悬浮背景 |
| `selectedRowBg` | `RowSelectedBg` / `TableSelectedRowBg` | 选中行背景 |
| `selectedRowHoverBg` | `RowSelectedHoverBg` / `TableSelectedRowHoverBg` | 选中行悬浮背景 |
| `rowExpandedBg` | `RowExpandedBg` / `TableExpandedRowBg` | 展开行背景 |
| `cellPadding` | `CellPadding` / `TablePadding` | 单元格内间距 |
| `cellPaddingMD` | `CellPaddingMD` / `TablePaddingMiddle` | 中等尺寸内间距 |
| `cellPaddingSM` | `CellPaddingSM` / `TablePaddingSmall` | 小尺寸内间距 |
| `borderColor` | `BorderColor` / `TableBorderColor` | 边框颜色 |
| `headerBorderRadius` | `HeaderBorderRadius` / `TableRadius` | 表头圆角 |
| `footerBg` | `FooterBg` / `TableFooterBg` | 表尾背景 |
| `footerColor` | `FooterColor` / `TableFooterTextColor` | 表尾文字颜色 |
| `cellFontSize` | `CellFontSize` / `TableFontSize` | 单元格字体大小 |
| `headerSplitColor` | `HeaderSplitColor` / `TableHeaderCellSplitColor` | 表头分割线颜色 |
| `fixedHeaderSortActiveBg` | `FixedHeaderSortActiveBg` / `TableFixedHeaderSortActiveBg` | 固定表头排序背景 |
| `headerFilterHoverBg` | `HeaderFilterHoverBg` / `TableHeaderFilterActiveBg` | 过滤按钮悬浮背景 |
| `filterDropdownMenuBg` | `FilterDropdownMenuBg` | 过滤菜单选项背景 |
| `filterDropdownBg` | `FilterDropdownBg` / `TableFilterDropdownBg` | 过滤下拉菜单背景 |
| `expandIconBg` | `ExpandIconBg` / `TableExpandIconBg` | 展开图标背景 |
| `selectionColumnWidth` | `SelectionColumnWidth` / `TableSelectionColumnWidth` | 选择列宽度 |

---

## 6. Token 计算逻辑说明

### 颜色计算

DataGrid Token 中的颜色值使用了 `ColorUtils.OnBackground()` 方法进行计算。该方法确保颜色在不同背景上具有正确的视觉效果：

```csharp
// 将半透明颜色合成到容器背景上
var colorFillSecondarySolid = ColorUtils.OnBackground(
    SharedToken.ColorFillSecondary,    // 前景色（可能半透明）
    SharedToken.ColorBgContainer       // 背景色
);
```

这种计算方式确保了：
- 在亮色模式下，半透明颜色正确合成到白色背景上
- 在暗色模式下，半透明颜色正确合成到深色背景上
- Token 值始终是不透明的，可以直接用于 `Background` 等属性

### 图标颜色计算

表头图标颜色使用了透明度叠加计算：

```csharp
HeaderIconColor = ColorUtils.FromRgbF(
    baseColorAction.GetAlphaF() * SharedToken.OpacityLoading,  // 降低透明度
    baseColorAction.GetRedF(),
    baseColorAction.GetGreenF(),
    baseColorAction.GetBlueF()
);
```

这使得排序/过滤图标在未激活状态下呈现较淡的颜色，悬浮时变深。

---

## 7. 源码参考

- Token 定义：`src/AtomUI.Desktop.Controls.DataGrid/DataGridToken.cs`
- Token 资源键生成：`src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator/TokenResourceConst.g.cs`
- 主题样式：`src/AtomUI.Desktop.Controls.DataGrid/Themes/DataGrid.axaml`