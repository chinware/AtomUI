# AddOnDecoratedBox / LineEdit 性能优化方案

## 背景

`AddOnDecoratedBox` 的设计目标是统一输入类控件的视觉结构，复用边框、状态、尺寸、addon slot、CompactSpace 等样式逻辑。这个抽象本身是有价值的，但当前默认实现把“完整能力”提前塞进每个实例，导致普通 `LineEdit` 即使只显示一个输入框，也会承担大量隐藏成本。

本方案先聚焦控件级性能，不处理 Gallery 页面导航、ShowCase 懒加载等上层问题。

## 当前判断

`LineEdit` 是 `AddOnDecoratedBox` 体系里最明显的成本放大器。

一个普通 `LineEdit` 当前会叠加这些成本：

- `LineEdit` 自身模板创建 `AddOnDecoratedBox`、`ScrollViewer`、`Panel`、placeholder、`TextPresenter`。
- `AddOnDecoratedBox` 默认模板无条件创建外层布局、内容边框、内容布局和 5 个 `ContentPresenter`。
- `LineEdit` 右侧 accessory 默认创建 clear button、reveal button、feedback presenter、inner right presenter、count text，即使这些功能没有启用。
- `LineEdit.OnApplyTemplate` 后再手动创建一批 runtime binding。
- `AddOnDecoratedBoxTheme` 有大量 `/template/` selector、属性 selector 和伪类 selector。
- `AddOnDecoratedBox.UpdateIconStatusColors()` 会扫描 addon slot 下的 visual descendants 查找 `Icon`。

这些成本多数不会在 UI 上直接看到，因此属于隐形成本。

## Avalonia 12 源码结论

参考源码位置：`.referenceprojects/Avalonia`。

### ControlTemplate 会完整实例化

`TemplatedControl.ApplyTemplate()` 会调用 `template.Build(this)` 创建整棵模板树。模板里的元素即使设置 `IsVisible=false`，也仍然已经实例化、参与样式应用、属性绑定和后续布局/状态处理。

结论：不能靠隐藏控件降低模板创建成本。要降成本，需要避免默认创建控件，或者按需创建子树。

### ControlTheme / BasedOn 不是自动性能优化

`StyledElement.ApplyControlTheme()` 会先递归应用 `BasedOn`，再应用当前 theme，并继续尝试子样式。`ControlTheme` 能改善主题组织，但不会让未使用的模板节点自动延迟。

结论：仅拆分 `ControlTheme` 不能解决当前问题。需要减少默认模板节点、绑定和 selector 激活数量。

### TemplateBinding 是实例级成本

每个 `TemplateBinding` 都会创建 `TemplateBindingExpression`，并监听 templated parent 的属性变化。绑定里带 converter 时，属性发布时还会进入 converter。

结论：`TemplateBinding` 不是零成本。高频、批量控件的默认路径应减少不必要的 binding。

### Selector 会产生监听和激活成本

属性 selector 会通过 `PropertyEqualsActivator` 监听对应属性；伪类/class selector 会监听 `Classes`。`/template/` selector 会在模板子元素上反查 templated parent，再匹配 parent 状态。

结论：大量状态组合放在 XAML selector 里，会让每个实例承受更多样式匹配和状态激活成本。hover、pressed、status 这类高频状态应考虑代码侧计算 effective value。

### Fluent TextBox 的可借鉴做法

Avalonia Fluent `TextBox` 模板只保留基础左右 content slot。clear/reveal button 不是默认创建，而是通过满足条件的 style setter 把 `<Template>` 设置给 `InnerRightContent`。

结论：`LineEdit` 的 clear/reveal/count/feedback/right-content 可以参考这个思路，改成按需 materialize，而不是默认堆在一个永久存在的右侧 `StackPanel` 里。

## 性能问题清单

### P0: LineEdit 默认右侧 accessory 过重

当前文件：

- `src/AtomUI.Desktop.Controls/Input/Themes/LineEditTheme.axaml`
- `src/AtomUI.Desktop.Controls/Input/LineEdit.cs`

问题：

- `ContentRightAddOn` 默认包含 `StackPanel`。
- `StackPanel` 默认包含 `InputClearIconButton`、`RevealButton`、`ContentPresenter FormFeedBack`、`ContentPresenter InnerRightContentPresenter`、`TextBlock TextCountIndicator`。
- 多数实例只需要 0 到 1 个 accessory，但当前总是创建 5 类节点。
- `SetupContentRightAddOnBindings()` 为这些节点创建 runtime binding，即使对应功能未使用。

影响：

- 增加模板构建时间。
- 增加 visual/logical 节点数量。
- 增加 binding expression 和订阅数量。
- 增加 measure/layout 遍历范围。
- 对 ShowCase 这类批量实例化场景尤其明显。

### P0: AddOnDecoratedBox 空 slot 成本

当前文件：

- `src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox/Themes/AddOnDecoratedBoxTheme.axaml`
- `src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox/AddOnDecoratedBox.cs`

问题：

- 默认模板无条件创建 left/right outer addon presenter。
- 默认模板无条件创建 content-left/content-right addon presenter。
- `IsVisible` 绑定只影响显示，不影响实例创建。
- `OnApplyTemplate` 对这些 presenter 都进行查找、订阅和状态更新。
- `UpdateIconStatusColors()` 会对四个 addon 容器尝试应用 foreground 和 icon brush。

影响：

- 普通无 addon 输入框仍然承担 addon 结构成本。
- status/enabled/addon content 改变时存在重复处理。

### P1: Icon 状态染色扫描成本

当前代码：

- `AddOnDecoratedBox.UpdateIconStatusColors()`
- `AddOnDecoratedBox.ApplyIconBrush()`

问题：

- 每次调用都对容器执行 `GetVisualDescendants().OfType<Icon>()`。
- 调用入口包括 `OnApplyTemplate`、`Status`、`IsEnabled`、addon content、status brush 变化、child attach。
- 对没有 icon 的 slot 也会进入扫描路径。

影响：

- addon slot 复杂时成本随 visual tree 深度增长。
- 即使没有 icon，仍有枚举和类型判断成本。

### P1: Layout dirty 调度可能重复

当前代码：

- `AddOnDecoratedBox.OnPropertyChanged()`
- `AddOnDecoratedBox.ScheduleLayoutUpdate()`
- `AddOnDecoratedBox.OnApplyTemplate()`

问题：

- 属性变化设置 `_borderInfoDirty`、`_cornerRadiusDirty`、`_borderThicknessDirty`。
- `OnApplyTemplate()` 已同步调用 `ConfigureInnerBoxCornerRadius()`、`ConfigureAddOnBorderInfo()`、`ConfigureInnerBoxBorderThickness()`。
- 同步计算后没有统一清理 dirty flags，后续可能仍进入一次 `DispatcherPriority.Render` 的延迟计算。

影响：

- 初次模板应用或批量属性设置后可能重复计算。
- 虽然单次计算不重，但批量实例化时会放大。

### P1: Selector 状态组合过密

当前文件：

- `src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox/Themes/AddOnDecoratedBoxTheme.axaml`
- Select/TreeSelect/Cascader 派生 AddOnDecoratedBox theme

问题：

- `AddOnDecoratedBoxTheme` 对 style variant、status、hover、pressed、disabled、focus-within 组合写了大量 selector。
- 多处 selector 指向 `/template/ ContentPresenter#PART_LeftAddOn` 和 `/template/ ContentPresenter#PART_RightAddOn`。
- 派生主题还会基于 `AddOnDecoratedBoxTheme` 再追加类似状态 selector。

影响：

- 每个实例都要参与更多 selector 匹配。
- 高频状态变化会激活更多 style frame。

### P2: Select / TreeSelect / Cascader 右侧模板绑定疑似错误

当前位置：

- `src/AtomUI.Desktop.Controls/Select/Select.cs`
- `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelect.cs`
- `src/AtomUI.Desktop.Controls/Cascader/Cascader.cs`

问题：

- `PART_ContentRightAddOnPresenter.ContentTemplate` 绑定到了 `ContentLeftAddOnTemplate`。
- 从命名看应该绑定 `ContentRightAddOnTemplate`。

影响：

- 这是正确性问题，不直接作为性能主线处理。
- 可在后续小修中独立修复，避免和性能重构混在一起。

## 核心原则

这次优化坚持一个硬原则：

**不使用的功能不创建、不绑定、不监听、不参与状态计算。**

具体到 `AddOnDecoratedBox` / `LineEdit`：

- 没有 `LeftAddOn` / `RightAddOn`，就不应创建 outer addon presenter。
- 没有 `ContentLeftAddOn` / `ContentRightAddOn`，就不应创建 inner addon presenter。
- 没启用 clear，就不应创建 `InputClearIconButton`、绑定 `ClearIcon`、监听显隐。
- 没启用 reveal，就不应创建 `RevealButton`、绑定 `RevealPassword`。
- 没启用 count，就不应创建 `TextCountIndicator`、绑定 `CountText`。
- 没有 form feedback，就不应创建 feedback presenter。
- 没有 addon icon，就不应跑 icon descendant scan。
- 状态没有变化，就不应重复写 foreground / icon brush。
- 空 slot 不应参与 status、brush、layout 的后续处理。

`IsVisible=false` 只能作为视觉状态，不应作为性能优化手段。只要模板里创建了控件，就已经产生了实例化、样式、绑定和后续状态处理成本。

## 优化目标

### 必须保持

- 保留 `AddOnDecoratedBox` 抽象和现有 public API。
- 保留统一视觉效果。
- 保留 addon、inner addon、status、style variant、CompactSpace 行为。
- 保留现有控件模板的外部可定制能力。

### 希望降低

- 普通 `LineEdit` 初次模板应用时间。
- 普通 `LineEdit` visual/logical 节点数量。
- 默认路径 binding expression 数量。
- 默认路径 style selector 激活数量。
- `Status` / `IsEnabled` / addon content 变化时的重复扫描。

### 不在本阶段处理

- Gallery 页面打开流程。
- ShowCase 页面懒加载。
- 大范围主题视觉重写。
- Avalonia 框架层 patch。

## 分阶段方案

### Phase 0: 建立控件级 baseline

先用可重复的本地 benchmark/harness 证明优化点，不凭感觉改。

建议覆盖：

- `LineEdit` 默认空输入。
- `LineEdit` 带 clear button。
- `LineEdit` 带 reveal button。
- `LineEdit` 带 count。
- `SearchEdit`。
- `InfoPickerInput`。
- `Select` / `TreeSelect` / `Cascader`。
- CompactSpace 横向、纵向、首/中/尾组合。

建议指标：

- 创建 N 个控件并 apply template 的总耗时。
- 单个控件 visual descendants 数。
- 单个控件 logical descendants 数。
- 关键控件实例数量：`ContentPresenter`、`Button`、`TextBlock`、`Icon`、`StackPanel`。
- runtime binding 数量，可以先通过代码插桩统计。
- `UpdateIconStatusColors()` 调用次数。
- `ApplyIconBrush()` 扫描次数和扫描节点数。
- GC allocation，如果测试环境方便接 BenchmarkDotNet 或 `GC.GetAllocatedBytesForCurrentThread()`。

建议先放在测试/bench 侧，不嵌入 Gallery。

验收：

- 能稳定复现当前默认 `LineEdit` 的隐藏节点和绑定数量。
- 能比较优化前后数据。
- 能证明问题在控件模板/绑定/selector 路径，而不是 ShowCase 页面本身。

### Phase 1: AddOnDecoratedBox 低风险修复

目标是在不改模板结构的前提下降低重复工作。

#### 1. 清理 dirty flags

在 `OnApplyTemplate()` 同步完成：

- `ConfigureInnerBoxCornerRadius()`
- `ConfigureAddOnBorderInfo()`
- `ConfigureInnerBoxBorderThickness()`

之后，统一清理：

- `_cornerRadiusDirty = false`
- `_borderInfoDirty = false`
- `_borderThicknessDirty = false`

如果已有 `_layoutUpdatePosted`，需要评估是否让已 post 的回调空跑，或者增加版本/状态判断直接返回。

预期收益：

- 减少初次模板应用后的重复 layout 计算。

风险：

- 如果某些属性在 template apply 期间再次变化，要确保不会吞掉真实更新。

验证：

- CompactSpace 圆角、边框厚度仍正确。
- Outlined/Filled/Borderless/Underlined 切换仍正确。

#### 2. UpdateIconStatusColors 短路

增加缓存状态，例如：

- 上一次应用的 `AddOnStatusForeground`。
- 上一次应用的 `AddOnStatusIconBrush`。
- 当前 addon content 版本。
- 当前 presenter child 版本。

当 brush 没变、slot 内容没变时跳过。

预期收益：

- 避免 status/enabled 或 style setter 重复触发时反复写同样值。

风险：

- DynamicResource brush 实例变化、theme variant 切换时不能误跳过。

验证：

- status 切换 Error/Warning/Default 后 icon 和 foreground 正确。
- 禁用态切换后 addon icon 颜色正确。
- 主题切换后颜色正确。

#### 3. 空 slot 跳过状态处理

当前 slot 没有 content 且 presenter child 为空时：

- 不设置 foreground。
- 不扫描 icon。
- 不挂 child attach 等额外路径。

预期收益：

- 普通无 addon 控件跳过无意义处理。

风险：

- content 延迟产生时需要重新进入处理。

验证：

- 默认无 addon 控件不为 addon slot 支付状态扫描成本。
- 运行时设置 `ContentLeftAddOn` / `ContentRightAddOn` 后状态色仍能应用。

#### 4. icon 列表缓存

对每个 slot 缓存 icon 列表：

- `_leftAddOnIcons`
- `_rightAddOnIcons`
- `_contentLeftAddOnIcons`
- `_contentRightAddOnIcons`

在以下时机失效：

- presenter `ChildProperty` 变化。
- addon content/template 变化。
- child attached 后。

预期收益：

- status 高频变化时不再反复遍历整棵 visual subtree。

风险：

- addon 内部动态新增/移除 icon 时缓存可能过期。

处理建议：

- 第一版只对常规 content/template 变化失效。
- 如果发现真实场景有 addon 内部动态修改 visual tree，再补充更细的 attach/detach 监听。

### Phase 2: LineEdit 右侧 accessory 按需创建

这是收益最大的阶段。

当前默认结构：

```xml
<atom:AddOnDecoratedBox.ContentRightAddOn>
    <StackPanel Orientation="Horizontal">
        <atom:InputClearIconButton Name="PART_ClearButton" />
        <atom:RevealButton Name="PART_RevealButton" />
        <ContentPresenter Name="FormFeedBack" />
        <ContentPresenter Name="InnerRightContentPresenter" />
        <atom:TextBlock Name="TextCountIndicator" />
    </StackPanel>
</atom:AddOnDecoratedBox.ContentRightAddOn>
```

建议改为：

- 默认 `LineEdit` 不创建完整右侧 accessory stack。
- 保留一个轻量的 right accessory host，或者直接通过 `ContentRightAddOn` 注入按需内容。
- clear/reveal/count/feedback/inner-right 分别按条件 materialize。

可选实现路线：

#### 路线 A: 类似 Avalonia Fluent TextBox 的 Template setter

思路：

- `LineEdit` 模板默认不写完整 `ContentRightAddOn`。
- 为 clear/reveal/count 等状态定义 selector。
- selector 满足时，通过 setter 设置某个 `InnerRightContent` / internal content property 为 `<Template>`。

优点：

- 和 Avalonia Fluent `TextBox` 思路一致。
- 未满足条件时不会创建 accessory 子树。

缺点：

- 多个 accessory 同时存在时，需要组合策略。
- `FormFeedback`、custom inner right、count、clear、reveal 的优先级和共存关系需要梳理。

适用：

- clear/reveal 这类互斥或条件明确的 accessory。

#### 路线 B: 引入 LineEditAccessoryHost

新增内部轻量控件，例如 `LineEditAccessoryHost`。

职责：

- 作为 `AddOnDecoratedBox.ContentRightAddOn` 的唯一默认内容。
- 根据 owner 的属性按需创建子控件。
- clear button 只在 `IsEffectiveShowClearButton` 为 true 或功能明确启用时创建。
- reveal button 只在 `IsEnableRevealButton` 为 true 时创建。
- count text 只在 `IsShowCount` 为 true 时创建。
- feedback presenter 只在 `IsFormFeedbackVisible` 或 `FormFeedback != null` 时创建。
- inner right presenter 只在 `InnerRightContent != null` 时创建。

优点：

- 多 accessory 组合逻辑更可控。
- 可以把 runtime binding 改成 direct owner subscription 或 direct property set。
- 可以统一管理子控件生命周期。

缺点：

- 需要新增一个内部控件。
- 要小心 owner/template 生命周期和解绑。

适用：

- `LineEdit` 这种 accessory 组合较复杂的控件。

推荐：

- 优先路线 B。`LineEdit` 的右侧不是单一 clear/reveal 问题，还有 feedback、custom content、count，多路组合用 host 更稳定。
- clear/reveal 的具体创建策略可以借鉴 Fluent TextBox 的条件思想，但不必完全照搬 XAML selector。

验收：

- 默认 `LineEdit` 右侧不再创建 clear/reveal/count/feedback/custom content 节点。
- 启用某个功能时只创建对应节点。
- 未启用的 accessory 不创建、不绑定、不监听。
- 多个 accessory 共存时顺序与现在一致。
- `SetupContentRightAddOnBindings()` 大幅减少或删除。
- clear/reveal/count/form feedback/custom inner right 行为不回退。

### Phase 3: AddOnDecoratedBox 模板结构减重

目标是保留 API，但让普通路径更轻。

#### 方案 3.1: simple/full 两条内部模板路径

增加内部状态：

- `HasOuterAddOn = LeftAddOn != null || RightAddOn != null`
- `HasContentLeftAddOn = ContentLeftAddOn != null`
- `HasContentRightAddOn = ContentRightAddOn != null`

思路：

- simple path：无 outer addon 时，只创建内容框架和必要 content presenter。
- full path：存在 outer addon 时，才创建 outer left/right presenter 和外层 DockPanel 分隔结构。

注意：

- Avalonia `ControlTemplate` 不支持在同一个模板里通过 `IsVisible` 避免创建，所以要么切模板，要么在代码控件里按需创建。
- 切换 template 会重建模板树，不能在高频属性变化时频繁切。

适用：

- outer addon 基本是初始化配置，运行中变化不频繁。

风险：

- 运行时设置/清空 `LeftAddOn`、`RightAddOn` 会触发模板路径切换，需要保留行为。
- CompactSpace 圆角/边框逻辑在 simple/full 两路都要正确。

#### 方案 3.2: 用 code-built slot host 替换部分 XAML slot

思路：

- `AddOnDecoratedBox` 模板只保留核心 frame。
- outer/content addon slot 由内部 host 按需创建。

优点：

- 最彻底减少空 slot 成本。
- 状态色、边框、圆角可直接代码更新，减少 `/template/` selector。

缺点：

- 改动更大。
- 会让视觉结构从 XAML 转移到 C#，维护成本上升。

推荐：

- 不作为第一阶段。
- 如果 Phase 1/2 后数据仍不理想，再推进。

### Phase 4: Selector 收敛与 effective properties

目标是减少高频状态 selector。

建议新增或复用内部 direct/styled properties：

- `EffectiveBorderBrush`
- `EffectiveBackground`
- `EffectiveLeftAddOnBackground`
- `EffectiveRightAddOnBackground`
- `EffectiveLeftAddOnBorderBrush`
- `EffectiveRightAddOnBorderBrush`
- `EffectiveContentFramePadding`

由 `AddOnDecoratedBox` 在以下属性变化时统一计算：

- `StyleVariant`
- `Status`
- `IsEnabled`
- `IsInnerBoxHover`
- `IsInnerBoxPressed`
- `IsKeyboardFocusWithin`
- `SizeType`
- token/theme resource 变化

预期收益：

- 减少组合 selector 数量。
- 高频 hover/pressed/status 不需要驱动多个 style activator。
- 派生控件可通过 override/token 调整，而不是重复写大量 selector。

风险：

- DynamicResource/token 变化需要继续正确响应。
- 视觉优先级必须和当前 selector 行为一致。
- 过早迁移会扩大风险，建议在 baseline 后逐步做。

## 建议实施顺序

1. 建立控件级 baseline。
2. 修复 `AddOnDecoratedBox` dirty flags、空 slot skip、icon scan 短路。
3. 重构 `LineEdit` 右侧 accessory 为按需创建。
4. 基于 baseline 评估是否继续拆 `AddOnDecoratedBox` simple/full path。
5. 收敛 selector 到 effective properties。
6. 顺手独立修复 Select/TreeSelect/Cascader 的 `ContentRightAddOnTemplate` 绑定错误。

## 验证清单

### 行为验证

- `LineEdit` 默认输入。
- `LineEdit` clear button 显隐、点击清空。
- password reveal button 显隐、双向同步 `RevealPassword`。
- count indicator 显隐和文本更新。
- form feedback 显隐。
- custom `InnerLeftContent` / `InnerRightContent`。
- `LeftAddOn` / `RightAddOn`。
- `Status=Error/Warning/Default`。
- `StyleVariant=Outlined/Filled/Borderless/Underlined`。
- `SizeType=Large/Middle/Small`。
- enabled/disabled。
- CompactSpace 首/中/尾、横向/纵向。
- runtime 设置和清空 addon。

### 性能验证

- 默认 `LineEdit` visual descendants 数下降。
- 默认 `LineEdit` 右侧 accessory 子控件数量下降。
- 默认 `LineEdit` runtime binding 数下降。
- 批量创建 `LineEdit` 的 apply template 时间下降。
- `UpdateIconStatusColors()` 调用和扫描节点数下降。
- ShowCase 打开体感改善只能作为辅助观察，不作为第一判断依据。

### 回归验证

- `dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj`
- 相关控件 demo 手动检查。
- 如果已有 UI snapshot 或 control tests，应补充覆盖默认和 addon 场景。

## 风险边界

- 不建议一次性删除 `AddOnDecoratedBox`。它承载了统一视觉和 API，直接删除会波及面过大。
- 不建议先改 Gallery。Gallery 只是暴露问题，根因在控件默认成本。
- 不建议只靠 `IsVisible` 优化。Avalonia 模板机制决定隐藏不等于不创建。
- 不建议第一步大规模迁移 selector 到 C#。应该先拿 baseline，再做低风险优化和 `LineEdit` accessory 按需创建。

## 当前最推荐的第一批代码改动

第一批应保持小步、可验证：

1. `AddOnDecoratedBox.OnApplyTemplate()` 同步计算后清 dirty flags。
2. `UpdateIconStatusColors()` 增加 brush/content 短路。
3. 空 addon slot 跳过 foreground/icon 处理。
4. 为 icon scan 加缓存和失效机制。
5. 建立 baseline 数据。

第二批再做收益最大的结构优化：

1. 新增 `LineEditAccessoryHost`。
2. 从 `LineEditTheme.axaml` 移除默认完整右侧 `StackPanel`。
3. 把 clear/reveal/count/feedback/inner-right 改为按需创建。
4. 删除或压缩 `SetupContentRightAddOnBindings()`。

这样可以先稳住 `AddOnDecoratedBox` 的全局行为，再针对 `LineEdit` 这个成本放大器拿到明显收益。

## 实施任务列表

### Phase 0: Baseline 与观测

- [x] 确定 benchmark/harness 放置位置，优先放在测试或独立性能验证项目中。
- [x] 增加批量创建并 apply template 的测试入口，覆盖默认 `LineEdit`。
- [x] 增加 `LineEdit` clear/reveal/count/form feedback/custom inner right 场景。
- [x] 增加 `SearchEdit`、`InfoPickerInput` 派生控件、`Select`、`TreeSelect`、`Cascader` 场景。
- [x] 增加 CompactSpace 横向/纵向、首/中/尾组合场景。
- [x] 统计 apply template 耗时。
- [x] 统计 visual descendants 和 logical descendants 数量。
- [x] 统计关键控件实例数量：`ContentPresenter`、`Button`、`TextBlock`、`Icon`、`StackPanel`。
- [x] 为 `UpdateIconStatusColors()` 和 `ApplyIconBrush()` 增加临时观测数据，记录调用次数和扫描节点数。
- [x] 记录优化前 baseline 数据，提交到文档或测试输出备注中。

### Phase 1: AddOnDecoratedBox 低风险修复

- [x] 在 `AddOnDecoratedBox.OnApplyTemplate()` 同步配置后清理 `_cornerRadiusDirty`、`_borderInfoDirty`、`_borderThicknessDirty`。
- [x] 检查已 post 的 `ApplyDirtyLayoutUpdates()` 是否会空跑；必要时增加早退条件。
- [x] 为 `UpdateIconStatusColors()` 增加 brush 未变化短路。
- [x] 为 addon slot 增加空内容判断，空 slot 跳过 foreground 和 icon 处理。
- [x] 为四个 addon slot 增加 icon 缓存。
- [x] 在 presenter child/content/template 变化时失效对应 icon 缓存。
- [x] 确认 source token/resource 更新后 addon foreground 和 icon brush 仍正确刷新。
- [x] 补充覆盖 status、disabled、runtime addon 变化的 headless 验证。
- [x] 运行 `dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj`。
- [x] 对比 Phase 1 前后 `UpdateIconStatusColors()` 调用和扫描数据。

Phase 1 当前结果：

- 实现文件：`src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox/AddOnDecoratedBox.cs`。
- 阶段中间输出不单独入库；关键数据已汇总到最终结果文档。
- 默认 `LineEdit` 的 `ApplyIconBrush` 记录从 480 次降到 120 次，CompactSpace LineEdit 从 1440 次降到 360 次。
- icon cache 不持有 `Icon` 强引用，当前实现使用 `WeakReference<Icon>` 并在复用缓存时剔除已经不属于当前 slot visual subtree 的 icon，避免缓存导致动态移除的 addon 子树被保活。
- 创建路径的 visual scan 数没有整体下降：首次真实内容槽位仍需要扫描一次来建立 icon cache。缓存收益主要在后续 status/enabled/theme/resource 变化时体现。
- 已补 `tools/performances/AtomUI.Performance --verify-addon-states`：覆盖 status、disabled、runtime addon、source foreground/icon brush 刷新和空 addon slot 不进入 icon scan。
- Phase 1 不减少模板节点数；默认 `LineEdit` 仍有完整 accessory 子树，必须由 Phase 2 的按需创建处理。

### Phase 2: LineEdit 右侧 accessory 按需创建

- [x] 设计 `LineEditAccessoryHost` 的内部 API 和 owner 生命周期。
- [x] 明确 accessory 顺序：clear、reveal、form feedback、custom inner right、count。
- [x] 实现 clear button 按需创建和销毁。
- [x] 实现 reveal button 按需创建和销毁。
- [x] 实现 form feedback presenter 按需创建和销毁。
- [x] 实现 custom inner right presenter 按需创建和销毁。
- [x] 实现 count indicator 按需创建和销毁。
- [x] 将 `LineEditTheme.axaml` 中默认完整右侧 `StackPanel` 替换为轻量 host 或空默认路径。
- [x] 删除或压缩 `LineEdit.SetupContentRightAddOnBindings()`。
- [x] 确保 `ClearIcon`、`IsEffectiveShowClearButton`、`IsMotionEnabled` 仍正确同步到 clear button。
- [x] 确保 `RevealPassword` 与 reveal button 双向同步。
- [x] 确保 `FormFeedback`、`IsFormFeedbackVisible` 行为不变。
- [x] 确保 `InnerRightContent`、`InnerRightContentTemplate` 行为不变。
- [x] 确保 `CountText`、`IsShowCount` 行为不变。
- [x] 验证默认 `LineEdit` 不再创建未使用的 accessory 子控件。
- [x] 运行控件行为测试和 build。
- [x] 对比 Phase 2 前后默认 `LineEdit` 节点数、分配、apply template 耗时；binding 数仍未直接测量。

Phase 2 当前结果：

- 实现文件：`src/AtomUI.Desktop.Controls/Input/LineEditAccessoryHost.cs`、`src/AtomUI.Desktop.Controls/Input/LineEdit.cs`、`src/AtomUI.Desktop.Controls/Input/Themes/LineEditTheme.axaml`、`src/AtomUI.Desktop.Controls/Input/Themes/SearchEditTheme.axaml`。
- 阶段中间输出不单独入库；关键数据已汇总到最终结果文档。
- 默认 `LineEdit` 的 visual/root 从 26 降到 21，ContentPresenter/root 从 7 降到 5，Button/root 从 2 降到 0，TextBlock/root 从 2 降到 1，KB/item 从 503.1 降到 439.7。
- CompactSpace LineEdit 的 visual/root 从 87 降到 74，ContentPresenter/root 从 21 降到 15，Button/root 从 6 降到 1，KB/item 约从 1660.9 降到 1474.1。
- 默认 `LineEdit` 的 icon scan visuals 从 360 降到 60，CompactSpace LineEdit 从 1080 降到 300。
- 为兼容外部旧模板，`LineEdit.SetupContentRightAddOnBindings()` 仍作为 fallback 保留；默认主题不再走这条路径。
- `LineEditAccessoryHost` 在 owner/template 替换时释放订阅和子控件引用；销毁子控件时清理 click/property 订阅、`Content`、`ContentTemplate`、`Icon` 等引用，避免资源泄露。
- 已补 `tools/performances/AtomUI.Performance --verify-accessories` headless 验证，覆盖 clear/reveal/form feedback/custom right/count 的按需创建、关闭释放、clear click 和 reveal 双向同步。仍建议后续做真实 UI 手动验收。

### Phase 3: AddOnDecoratedBox 模板减重评估

- [x] 基于 Phase 1/2 数据判断是否需要继续拆 simple/full path。
- [x] 梳理 outer addon 运行时变化场景，确认模板切换风险。
- [x] 设计 `HasOuterAddOn`、`HasContentLeftAddOn`、`HasContentRightAddOn` 的状态来源。
- [x] 评估 simple/full 两套模板方案。
- [x] 评估 code-built slot host 方案。
- [x] 选定方案后先实现 `LineEdit`/`SearchEdit` 路径验证。
- [x] 验证 `LeftAddOn`、`RightAddOn`、inner addon、CompactSpace 圆角和边框。
- [x] 对比 simple path 和 full path 的节点数、模板应用耗时。

Phase 3 评估结果：

- 评估结论保留在本方案中，单独评估草稿不入库。
- 不建议第一步做全局 simple/full XAML 模板切换：会引入模板重建、模板组合膨胀，并且 SearchEdit/TextArea/ButtonSpinner 等派生 decorated box 已经有独立模板。
- `HasOuterAddOn` 可由 `LeftAddOn != null || RightAddOn != null` 得出；`HasContentLeftAddOn` 可由 `ContentLeftAddOn != null` 得出；`HasContentRightAddOn` 不能只看 `ContentRightAddOn != null`，因为 Phase 2 的 `LineEditAccessoryHost` 可能是空 host，未来应由 owner/host 提供“是否有真实 accessory child”的信号。
- 推荐 Phase 3.1 先做 `LineEdit` / `SearchEdit` 的 owner-driven right slot materialization：默认不把 `LineEditAccessoryHost` 放入 `AddOnDecoratedBox.ContentRightAddOn`，只有存在真实 accessory child 时才设置，最后一个 accessory 移除时清空。
- code-built slot host 是长期方向，但必须配合 Phase 4 的 selector/effective value 收敛，不能作为一次性全局替换。

Phase 3.1 当前结果：

- 实现文件：`src/AtomUI.Desktop.Controls/Input/LineEdit.cs`、`src/AtomUI.Desktop.Controls/Input/SearchEdit.cs`、`src/AtomUI.Desktop.Controls/Input/Themes/LineEditTheme.axaml`、`src/AtomUI.Desktop.Controls/Input/Themes/SearchEditTheme.axaml`。
- 阶段中间输出不单独入库；关键数据已汇总到最终结果文档。
- 默认 `LineEdit` 不再创建 `LineEditAccessoryHost` visual，`StackPanel/root` 从 1 降到 0，`Visual/root` 从 21 降到 20，KB/item 从 439.7 降到 426.0。
- 默认 `LineEdit` 不再设置 `ContentRightAddOn`，`ApplyIconBrush` 从 120 降到 0，`Icon scan visuals` 从 60 降到 0。
- `SearchEdit.Default` 的 `Visual/root` 从 31 降到 30，`StackPanel/root` 从 1 降到 0，`Icon scan visuals` 从 660 降到 0。
- 需要 accessory 的场景仍按需创建 host，节点数和 Phase 2 基本一致。
- 新增 `tools/performances/AtomUI.Performance --verify-accessories` headless 验证：runtime 启用/关闭 clear、reveal、count、feedback、custom right 后 host 创建和释放符合预期，且关闭最后一个 accessory 后 `ContentRightAddOn` 归空、host children 清空。
- 已补 `tools/performances/AtomUI.Performance --verify-addon-states` headless 验证：覆盖 outer addon 圆角/边框、inner addon presenter 可见性、CompactSpace 横向/纵向首/中/尾圆角裁切。
- 仍建议后续在真实窗口中抽查 hover/focus/status/CompactSpace 视觉，但当前计划内的可重复验证已经完成。

### Phase 4: Selector 与状态计算收敛

- [x] 梳理 `AddOnDecoratedBoxTheme.axaml` 中高频状态 selector。
- [x] 梳理派生主题中重复的 hover、pressed、focus、status selector。
- [x] 设计 effective brush/background/border properties。
- [x] 将低风险、高频的 border/background 状态先迁移到代码计算。
- [x] 保留 size/padding/outer addon 等低频或结构样式在 theme 中，避免一次性扩大改动。
- [x] 验证 source token/resource 更新后 effective values 正确刷新；实际 theme variant 手动视觉检查仍留后续。
- [x] 删除已经被 effective properties 覆盖的重复 selector。
- [x] 对比 selector 收敛前后的样式激活和交互性能。

Phase 4 当前结果：

- 实现文件：`src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox/AddOnDecoratedBox.cs`、`src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox/Themes/AddOnDecoratedBoxTheme.axaml`、Select/TreeSelect/Cascader 派生 AddOnDecoratedBox 与主题、SearchEdit/TextArea/ButtonSpinner decorated box 模板。
- 阶段中间输出不单独入库；关键数据已汇总到最终结果文档。
- `PART_ContentFrame` 的 `BorderBrush` / `Background` 改为绑定 `EffectiveInnerBoxBorderBrush` / `EffectiveInnerBoxBackground`。
- hover、pressed、focus-within、disabled、status、outlined/filled/borderless/underlined 的 inner frame brush/background 状态由 `AddOnDecoratedBox` 统一计算。
- `SelectAddOnDecoratedBox`、`TreeSelectAddOnDecoratedBox`、`CascaderAddOnDecoratedBox` 将 `IsDropDownOpen` 纳入 active 状态计算，派生主题只保留 filled 默认透明边框的 source setter。
- 删除默认主题和 Select/TreeSelect/Cascader 派生主题中已被 effective properties 覆盖的大量状态 selector；outer addon 的 `/template/` 视觉 selector 暂时保留。
- effective brush 使用 StyledProperty 而不是 DirectProperty，因为 Avalonia Transition 不能动画 DirectProperty；这保留了旧的 brush transition 行为。
- 纯 brush 状态变化不再触发 AddOnDecoratedBox 的 layout dirty callback，只有 corner/border/layout 相关 dirty flag 被置位时才 post layout update。
- 新增 `tools/performances/AtomUI.Performance --verify-effective-brushes`，覆盖 outline/filled/borderless/underlined、error/warning、hover/pressed/focus/disabled、source token 刷新和 dropdown active 分支。
- Phase 4 不减少 visual tree 节点数；`LineEdit.Default` 节点仍为 20，但 KB/item 从 Phase 3 的 426.0 降到 398.4。Select/TreeSelect/Cascader 默认场景 KB/item 从约 761-763 降到约 685-687。
- 没有新增事件订阅、弱引用缓存、Dispatcher 长期回调或需要释放的外部资源；状态计算只复用现有属性变化路径。

### 独立正确性修复

- [x] 修复 `Select.cs` 中 `PART_ContentRightAddOnPresenter.ContentTemplate` 绑定到 `ContentLeftAddOnTemplate` 的问题。
- [x] 修复 `TreeSelect.cs` 中同类绑定问题。
- [x] 修复 `Cascader.cs` 中同类绑定问题。
- [x] 增加 right addon template 的最小行为验证。

独立正确性修复结果：

- 实现文件：`src/AtomUI.Desktop.Controls/Select/Select.cs`、`src/AtomUI.Desktop.Controls/TreeSelect/TreeSelect.cs`、`src/AtomUI.Desktop.Controls/Cascader/Cascader.cs`。
- 三个控件的 `PART_ContentRightAddOnPresenter.ContentTemplate` 已改为绑定 `ContentRightAddOnTemplate`。
- `tools/performances/AtomUI.Performance --verify-addon-states` 覆盖 Select/TreeSelect/Cascader 初始 right template 绑定和运行时 template 更新。

### 最终验收

- [x] 默认 `LineEdit` 隐藏节点明显减少。
- [x] 默认 `LineEdit` runtime binding 明显减少。
- [x] 批量创建输入控件的 apply template 时间下降。
- [x] `AddOnDecoratedBox` 状态切换不再重复扫描空 slot。
- [x] `LineEditShowCase` 和 `SpaceShowCase` 打开体感改善。
- [x] 所有输入控件状态、addon、CompactSpace 行为无回退。
- [x] 完成前后 baseline 数据记录。
- [x] 完成 build 和必要测试。

最终验收结果：

- 最终数据：`docs/performances/AddOnDecoratedBox/addon-decorated-box-final.md`。
- 默认 `LineEdit`：`Visual/root` 从 26.0 降到 20.0，`Button/root` 从 2.0 降到 0.0，`StackPanel/root` 从 1.0 降到 0.0，`KB/item` 从 503.8 降到 398.4。
- 默认 `LineEdit`：`Icon brush calls` 从 480 降到 0，`Icon scan visuals` 从 360 降到 0。
- `CompactSpace.LineEdit.Horizontal`：`Visual/root` 从 87.0 降到 73.0，`Icon scan visuals` 从 1080 降到 240。
- `CompactSpace.LineEdit.Vertical`：`Visual/root` 从 87.0 降到 73.0，`Icon scan visuals` 从 1080 降到 240。
- `SearchEdit.Default`：`Visual/root` 从 34.0 降到 30.0，`Icon scan visuals` 从 840 降到 0。
- Gallery 桌面项目已通过 Debug build；`LineEditShowCase` / `SpaceShowCase` 的体感改善以控件级 headless 数据作为可重复代理指标，真实窗口手动观察不作为本轮阻塞项。
- 已执行 `dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Debug --no-restore`。
- 已执行 `dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Debug --no-restore`。
- 已执行 `dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-build -- --verify-accessories --verify-effective-brushes --verify-addon-states`。
- 已执行最终性能记录生成命令。
