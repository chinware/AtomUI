# RelativeSource AncestorType 优化计划

## 背景

`RelativeSource AncestorType` 绑定在控件 attach 到视觉树时会向上遍历祖先链查找目标类型。在嵌套较深的模板中（如 `AddOnDecoratedBox.ContentRightAddOn`），每个绑定需要遍历 6+ 层。大量控件实例（如表格中 1000 个输入框）会产生显著的性能开销。

## 修复方案

将 `RelativeSource AncestorType` 绑定迁移到 `OnApplyTemplate` 代码中，通过 `Bind(Property, new Binding(nameof(Prop)) { Source = this })` 直接绑定到模板父控件，消除祖先遍历。使用 `CompositeDisposable` 管理绑定生命周期，在模板重新应用时释放旧绑定。

**已完成参考实现**：`LineEdit` / `LineEditTheme.axaml`（11 处绑定已迁移）

---

## 第一优先级：AddOnDecoratedBox 嵌套内容（高影响）

这些控件在 `AddOnDecoratedBox.ContentRightAddOn` 或类似嵌套内容区域中使用 `RelativeSource AncestorType`，视觉树深度最深，影响最大。

| # | 文件 | 控件 | 绑定数 | AncestorType |
|---|------|------|--------|-------------|
| 1 | `Select/Themes/SelectTheme.axaml` | Select | 18 | atom:Select (16), atom:AddOnDecoratedBox (2) |
| 2 | `TreeSelect/Themes/TreeSelectTheme.axaml` | TreeSelect | 18 | atom:TreeSelect (16), atom:AddOnDecoratedBox (2) |
| 3 | `Cascader/Themes/CascaderTheme.axaml` | Cascader | 18 | atom:Cascader (16), atom:AddOnDecoratedBox (2) |
| 4 | `Input/Themes/SearchEditTheme.axaml` | SearchEdit | 8 | atom:SearchEdit |
| 5 | `NumericUpDown/Themes/NumericUpDownTheme.axaml` | NumericUpDown | 7 | atom:NumericUpDown |
| 6 | `ComboBox/Themes/ComboBoxTheme.axaml` | ComboBox | 7 | atom:ComboBox |
| 7 | `Primitives/InfoPickerInput/Themes/InfoPickerInputTheme.axaml` | InfoPickerInput | 7 | atom:InfoPickerInput |
| 8 | `Primitives/InfoPickerInput/Themes/RangeInfoPickerInputTheme.axaml` | RangeInfoPickerInput | 7 | atom:RangeInfoPickerInput (3), atom:InfoPickerInput (4) |
| 9 | `Input/Themes/TextAreaTheme.axaml` | TextArea | 6 | atom:TextArea |

**小计**：9 个文件，96 处绑定

### 实施步骤（每个控件）

1. 从 AXAML 中移除所有 `RelativeSource AncestorType` 绑定属性，保留控件名称和非绑定属性
2. 为无名控件添加 `Name`（如 ContentPresenter）
3. 在对应控件的 `OnApplyTemplate` 中：
   - 添加 `CompositeDisposable` 字段
   - `Dispose` 旧绑定，创建新 `CompositeDisposable`
   - 通过 `NameScope.Find` 获取模板控件
   - 使用 `Bind(Property, new Binding(...) { Source = this })` 建立直接绑定
   - 将所有 `Bind()` 返回值加入 `CompositeDisposable`

### 注意事项

- Select、TreeSelect、Cascader 三个控件模板结构高度相似（都有 `SelectorSuffixBox`），可以考虑提取公共基类方法
- `atom:AddOnDecoratedBox` 类型的 AncestorType 绑定（`IsInnerBoxHover`、`IsInnerBoxPressed`）需要绑定到 AddOnDecoratedBox 而非外层控件，需要通过 `NameScope.Find<AddOnDecoratedBox>` 获取后作为 Source
- TwoWay 绑定（如 `RevealPassword`）需要指定 `Mode = BindingMode.TwoWay`
- 使用 `ObjectConverters.IsNotNull` 的绑定需要在 `Binding` 上设置 `Converter`

---

## 第二优先级：其他嵌套内容（中等影响）

| # | 文件 | 控件 | 绑定数 | AncestorType | 嵌套位置 |
|---|------|------|--------|-------------|---------|
| 10 | `ButtonSpinner/Themes/ButtonSpinnerTheme.axaml` | ButtonSpinner | 4 | atom:ButtonSpinner | SpinnerContent |
| 11 | `TabControl/Themes/BaseTabScrollViewerTheme.axaml` | BaseTabScrollViewer | 3 | atom:TabScrollContentPresenter | GestureRecognizers |
| 12 | `Slider/Themes/SliderTheme.axaml` | Slider | 2 | atom:Slider | SliderTrack 子控件 |
| 13 | `Cascader/Themes/CascaderViewTheme.axaml` | CascaderView | 2 | atom:CascaderView | FilterList DataTemplate |
| 14 | `ImagePreviewer/Themes/ImageGroupPreviewerTheme.axaml` | ImageGroupPreviewer | 2 | atom:ImageGroupPreviewer | ItemTemplate DataTemplate |
| 15 | `ColorPicker/Themes/ColorPickerPaletteGroupTheme.axaml` | ColorPickerPaletteGroup | 2 | atom:ColorPickerPaletteGroup (1), atom:CollapseItem (1) | 嵌套 DataTemplate |

**小计**：6 个文件，15 处绑定

### 注意事项

- DataTemplate 内的 `RelativeSource AncestorType` 无法通过 `OnApplyTemplate` + `NameScope.Find` 解决（DataTemplate 内的控件在数据绑定时动态创建，不在模板 NameScope 中）
- 对于 DataTemplate 场景（CascaderView、ImageGroupPreviewer、ColorPickerPaletteGroup），需要考虑替代方案：
  - 将属性下推到 DataContext（数据模型）中
  - 或在 `ContainerForItemPreparedOverride` 中设置
  - 或保留 `RelativeSource AncestorType`（DataTemplate 内的控件数量通常有限）

---

## 第三优先级：ItemsPanel 模板绑定（低影响）

这些绑定在 `ItemsPanelTemplate` 中，Panel 只创建一次，影响极小。

| # | 文件 | 控件 | 绑定数 | AncestorType |
|---|------|------|--------|-------------|
| 16 | `Steps/Themes/StepsTheme.axaml` | Steps | 2 | atom:Steps |
| 17 | `Segmented/Themes/SegmentedTheme.axaml` | Segmented | 1 | atom:Segmented |
| 18 | `Timeline/Themes/TimelineTheme.axaml` | Timeline | 1 | atom:Timeline |
| 19 | `Pagination/Themes/PaginationNavTheme.axaml` | PaginationNav | 1 | atom:PaginationNav |
| 20 | `Carousel/Themes/CarouselPaginationTheme.axaml` | CarouselPagination | 1 | atom:CarouselPagination |

**小计**：5 个文件，6 处绑定

**建议**：可暂不处理，Panel 只实例化一次，祖先查找开销可忽略。

---

## 总计

| 优先级 | 文件数 | 绑定数 | 状态 |
|--------|--------|--------|------|
| 已完成（LineEdit） | 1 | 11 | ✅ |
| 第一优先级 | 9 | 96 | 待处理 |
| 第二优先级 | 6 | 15 | 待处理 |
| 第三优先级 | 5 | 6 | 建议暂不处理 |
| **合计** | **21** | **128** | |
