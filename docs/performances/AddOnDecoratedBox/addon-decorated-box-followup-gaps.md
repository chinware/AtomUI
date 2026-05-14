# AddOnDecoratedBox / LineEdit 性能优化遗漏评估

- 评估时间：2026-05-14
- 评估对象：commit `cdb6ed84e` 的 AddOnDecoratedBox / LineEdit 性能优化
- 参考数据：`docs/performances/AddOnDecoratedBox/addon-decorated-box-baseline.md`、`addon-decorated-box-final.md`
- 参考方案：`docs/performances/AddOnDecoratedBox/addon-decorated-box-performance-plan.md`

## 总体判断

本次优化对 `LineEdit` / `SearchEdit` 的右侧 accessory 按需创建以及 `AddOnDecoratedBox` 内框 brush selector 收敛做得比较彻底。但是同一类成本放大问题在派生控件和外层 addon 上仍然存在，主要分两部分：

1. **同样的"默认即创建完整 StackPanel + 多个子控件"模式** 在 Select / TreeSelect / Cascader / InfoPickerInput / RangeInfoPickerInput / RangeDatePicker / ComboBox / TextArea 等控件里**没有扩展处理**。
2. **AddOnDecoratedBox 外层 addon 的模板节点和 `/template/` selector** 是计划里显式 defer 的，可以作为下一阶段重点。

最终数据可以印证：

| Scenario | StackPanel/root Baseline | StackPanel/root Final |
| --- | ---: | ---: |
| LineEdit.Default | 1.0 | 0.0 |
| SearchEdit.Default | 1.0 | 0.0 |
| Select.Default | 2.0 | 2.0 |
| TreeSelect.Default | 2.0 | 2.0 |
| Cascader.Default | 2.0 | 2.0 |
| DatePicker.Default | 4.0 | 4.0 |
| RangeDatePicker.Default | 6.0 | 6.0 |

LineEdit / SearchEdit 已经把右侧 accessory 节点降到 0，其他控件维持 baseline。

## 实施记录

- 实施时间：2026-05-14
- 覆盖范围：
  - `TextArea` 改为 `TextAreaAccessoryHost` 按需创建 clear / form feedback / inner-right。
  - `Select` / `TreeSelect` / `Cascader` 收敛到 `AbstractSelect` 共享的 `SelectAccessoryHost`，默认保留必要 `SelectHandle`，`SelectMaxCountIndicator` 与自定义右侧内容按需创建。
  - `ComboBox` 改为 `ComboBoxAccessoryHost`，默认保留必要 handle，form feedback 与自定义右侧内容按需创建。
  - `InfoPickerInput` / `RangeInfoPickerInput` / `RangeDatePicker` 改为 `PickerAccessoryHost`，保留默认 `InfoIcon`，clear button / feedback / 自定义右侧内容按需创建。
  - `AddOnDecoratedBox` 的 outer / inner addon slot presenter 从模板固定节点改为代码侧按需创建，outer addon `/template/` selector 收敛为控件侧状态计算。
  - `LineEditAccessoryHost` 增加 owner property 变化批处理，避免连续属性变化触发整轮重复重算。
  - 修复 `InfoPickerInputTheme.axaml` 的 `LeftAddOnTemplate` 绑定错误，以及 `PickerClearUpButton` / `ComboBoxHandle` 事件 lambda 无法解绑的问题。
- 生命周期约束：
  - 新增 host 均提供 `DetachOwner()`，释放 owner subscriptions、token bindings、child event handlers，并清空 child content/template 引用。
  - 默认模板移除固定 accessory 节点后，旧模板命名 parts 仍保留兼容绑定路径。
- 验证：
  - `dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Debug --no-restore`
  - `dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 -- --verify-accessories --verify-effective-brushes --verify-addon-states`
  - 结果：Accessory lifecycle / Effective brush / Addon state verification 均通过。

当前快速计数（`count=1`，用于结构趋势，不作为稳定耗时基准）：

| Scenario | Visual/root | ContentPresenter/root | StackPanel/root | IconUpdates | BrushCalls |
| --- | ---: | ---: | ---: | ---: | ---: |
| LineEdit.Default | 16.0 | 1.0 | 0.0 | 3 | 0 |
| SearchEdit.Default | 30.0 | 5.0 | 0.0 | 3 | 1 |
| Select.Default | 27.0 | 4.0 | 2.0 | 5 | 2 |
| TreeSelect.Default | 27.0 | 4.0 | 2.0 | 5 | 2 |
| Cascader.Default | 27.0 | 3.0 | 2.0 | 5 | 2 |
| DatePicker.Default | 34.0 | 5.0 | 1.0 | 5 | 2 |
| RangeDatePicker.Default | 60.0 | 8.0 | 1.0 | 5 | 2 |
| ButtonSpinner.Default | 25.0 | 6.0 | 0.0 | 3 | 0 |

补充修正：`SearchEditDecoratedBox` 与 `ButtonSpinnerDecoratedBox` 等派生模板存在专用 part，不能由 `AddOnDecoratedBox` 通用逻辑移动或移除。后续修复为：仅动态创建的 slot presenter 由通用逻辑重排/移除；模板内已有的专用 part 保持原模板所有权。

## Codex 复评结论

这份 follow-up 的主判断成立：本轮优化只把 `LineEdit` / `SearchEdit` 的默认右侧 accessory 路径打薄了，Select、TreeSelect、Cascader、TextArea、Picker、ComboBox 仍存在同类默认节点和默认 binding 成本。

需要修正的点：

- `TextArea` 不能只把 `LineEditAccessoryHost` 改一个 `Orientation` 就直接复用；当前 host 的 owner 类型是 `LineEdit`，还订阅 `LineEdit` 专属属性。要复用，需要先抽 owner 接口或做泛化 host。
- `PickerClearUpButton` 不只是 clear button，还承载 DatePicker/TimePicker 的默认 info icon 和 form feedback。优化时不能按 `IsAllowClear` 简单延迟整个控件，否则会破坏默认右侧日历/时间图标。更合理的是拆 `PickerAccessoryHost`，或先重构 `PickerClearUpButton` 内部的 clear/icon/feedback 子节点按状态创建。
- `AddOnDecoratedBox` 的 4 个 slot presenter 确实是默认成本，但它们同时覆盖 outer addon 与 inner addon。可以先收敛 outer addon，也可以做统一 slot host；不建议直接切两套大模板，运行期状态与 CompactSpace 回归面更大。
- `AddOnDecoratedBoxPerfProbe` 的调用点在 `#if DEBUG` 下，Release 热路径无 probe 调用；但类文件本身没有整体 `#if DEBUG` 包裹。因此它不是性能问题，只是诊断代码边界可以更清晰。

额外观察：

- `InfoPickerInputTheme.axaml` 存在一个明显绑定错误：`LeftAddOnTemplate="{TemplateBinding LeftAddOn}"` 应为 `LeftAddOnTemplate="{TemplateBinding LeftAddOnTemplate}"`。这不是性能问题，但建议作为独立正确性修复。
- `PickerClearUpButton.OnApplyTemplate()` 用 lambda 订阅 `_clearButton.Click`，没有显式解绑。由于 child 通常随模板释放，未必形成实际泄露；但后续重构时应改成具名 handler，并在 re-template / detach 时解绑，符合“不引入资源泄露”的约束。

按当前代码状态，建议把这些 gap 分成三类：

| 类别 | Gap | 结论 |
| --- | --- | --- |
| 应进入下一轮优化 | Gap 1、Gap 2、Gap 3、Gap 4、Gap 5、Gap 6、Gap 7 | 均存在真实成本，但实施顺序和方案需调整 |
| 技术债清理 | Gap 8 | 真实存在，优先级低于新一轮性能收益项 |
| 语义确认 | Gap 9 | SearchEdit 默认无 reveal 成本；是否禁用 reveal / inner-right 是 API 语义问题，不应作为性能主线 |
| 诊断边界 | Gap 10 | 非运行期性能问题，可后续顺手整理 |

## 漏网之鱼

### Gap 1: Select / TreeSelect / Cascader 右侧仍硬编码默认 StackPanel

文件：

- `src/AtomUI.Desktop.Controls/Select/Themes/SelectTheme.axaml:32-39`
- `src/AtomUI.Desktop.Controls/TreeSelect/Themes/TreeSelectTheme.axaml`
- `src/AtomUI.Desktop.Controls/Cascader/Themes/CascaderTheme.axaml`

当前形态：

```xml
<atom:AddOnDecoratedBox.ContentRightAddOn>
    <StackPanel Orientation="Horizontal" Spacing="...">
        <atom:SelectMaxCountIndicator Name="PART_SelectMaxCountIndicator" />
        <ContentPresenter Name="PART_ContentRightAddOnPresenter" />
        <atom:SelectHandle Name="PART_SelectHandle" />
    </StackPanel>
</atom:AddOnDecoratedBox.ContentRightAddOn>
```

问题：

- 即使没有 `MaxCount`、没有 `ContentRightAddOn`，也总是创建 `SelectMaxCountIndicator`、`ContentPresenter`、`SelectHandle` 三个控件 + 一个 `StackPanel`。
- `Select.cs:556-607` 的 `SetupContentRightAddOnBindings()` 在每个实例上为这些 part 创建大量 runtime binding（`MaxCount`、`SelectedCount`、`IsShowMaxCountIndicator`、`ContentRightAddOn`、`ContentRightAddOnTemplate`、`FormFeedback`、`SuffixLoadingIcon`、`SuffixIcon`、`IsFilterEnabled`、`IsLoading`、`IsAllowClear`、`IsSelectionEmpty`、`IsDropDownOpen`、`IsInnerBoxHover`、`IsInnerBoxPressed` 等十几条），未启用功能也会订阅。

建议路线：

- 抽 `SelectAccessoryHost`（或 `AbstractSelect` 共用的 host），按需创建 `SelectMaxCountIndicator` / `ContentPresenter` / `SelectHandle`：
  - `MaxCountIndicator`：仅 `IsShowMaxCountIndicator && MaxCount > 0` 时创建。
  - `ContentRightAddOn` presenter：仅 `ContentRightAddOn != null` 时创建。
  - `SelectHandle`：通常需要保留（dropdown 触发器），但其内部 binding 可以从 host owner-driven。
- 默认主题不再硬编码 `<atom:AddOnDecoratedBox.ContentRightAddOn>`，由 owner 在 `OnApplyTemplate` 后按状态填充。
- `Select.cs / TreeSelect.cs / Cascader.cs` 的 `SetupContentRightAddOnBindings()` 改为只在 host 真正存在 child 时建 binding。

复评：

- 结论成立，且三份代码当前还有重复的 `SetupContentRightAddOnBindings()`，适合收敛成共享 host / helper。
- `SelectHandle` 是默认交互入口，不一定能完全按需延迟到打开前；但可以由 host 常驻一个轻量 handle，并进一步优化 handle 内部隐藏 icon。
- `SelectMaxCountIndicator` 和 `ContentRightAddOnPresenter` 应优先按需创建，收益明确、风险相对低。

### Gap 2: TextArea 完全没改，是 LineEdit 优化前的原版结构

文件：`src/AtomUI.Desktop.Controls/Input/Themes/TextAreaTheme.axaml:34-53`

```xml
<atom:TextAreaDecoratedBox.ContentRightAddOn>
    <StackPanel Orientation="Vertical" Spacing="...">
        <atom:InputClearIconButton Name="PART_ClearButton" .../>
        <ContentPresenter Name="PART_FormFeedBack" .../>
        <ContentPresenter Name="PART_InnerRightContentPresenter" .../>
    </StackPanel>
</atom:TextAreaDecoratedBox.ContentRightAddOn>
```

这就是 Phase 2 之前 `LineEdit` 的原版结构。`TextArea` 当前每个实例都创建 1 个 `StackPanel` + 1 个 `InputClearIconButton` + 2 个 `ContentPresenter`。

建议路线：

- `LineEditAccessoryHost` 已经支持 clear / inner-right / form feedback。只要把 host 默认 `Orientation` 改成可配置（构造参数或属性），`TextArea` 直接复用同一个 host 即可，不需要新增控件。
- `TextAreaTheme.axaml` 移除默认 `ContentRightAddOn`，由 `TextArea` 走 `LineEdit` 已有的 owner-driven 路径填充。

复评：

- 结论成立，TextArea 仍是典型默认 accessory StackPanel 成本。
- 但“只改 Orientation 即可复用”不准确。`LineEditAccessoryHost` 当前强依赖 `LineEdit` owner，TextArea 的属性名、count 位置和类型都不同。
- 推荐做法是抽一个小接口，例如暴露 clear、form feedback、inner-right、motion 等最小状态，再让 `LineEdit` / `TextArea` 分别适配；或者先做 `TextAreaAccessoryHost`，减少一次性泛化风险。

### Gap 3: InfoPickerInput / RangeDatePicker / RangeInfoPickerInput 默认包含 PickerClearUpButton

文件：

- `src/AtomUI.Desktop.Controls/Primitives/InfoPickerInput/Themes/InfoPickerInputTheme.axaml:28-34`
- `src/AtomUI.Desktop.Controls/Primitives/InfoPickerInput/Themes/RangeInfoPickerInputTheme.axaml:33-40`
- `src/AtomUI.Desktop.Controls/DatePicker/Themes/RangeDatePickerTheme.axaml:35-42`

当前形态：

```xml
<atom:AddOnDecoratedBox.ContentRightAddOn>
    <StackPanel Orientation="Horizontal" Spacing="...">
        <atom:PickerClearUpButton Name="PART_ClearUpButton" />
        <ContentPresenter Name="PART_ContentRightAddOnPresenter" />
    </StackPanel>
</atom:AddOnDecoratedBox.ContentRightAddOn>
```

问题：

- `PickerClearUpButton` 不依赖 `IsAllowClear` 是否启用，永远创建。
- DatePicker / RangeDatePicker 默认场景的 `Visual/root=43.0 / 69.0`、`StackPanel/root=4.0 / 6.0` 仍维持 baseline。

建议路线：

- 抽 `PickerAccessoryHost`，按 `IsAllowClear`、`ContentRightAddOn != null` 等条件 materialize。
- 沿用 `LineEditAccessoryHost` 的 owner subscription / EnsureChildOrder 思路。

复评：

- 结论部分成立。当前默认确实创建 `StackPanel + PickerClearUpButton + ContentPresenter`，且 `PickerClearUpButton` 内部还默认创建 `InputClearIconButton + IconPresenter + FormFeedback ContentPresenter`。
- 但 `PickerClearUpButton` 不是纯 clear button，它承载默认 `InfoIcon`。DatePicker / TimePicker 默认会设置 `CalendarOutlined` / `ClockCircleOutlined`，所以右侧 icon 本身不是未使用功能。
- 优化边界应改为：默认 info icon 必须保留；clear button 和 feedback presenter 按需创建；自定义 `ContentRightAddOn` presenter 按需创建。

### Gap 4: ComboBox 默认包含 FormFeedback presenter

文件：`src/AtomUI.Desktop.Controls/ComboBox/Themes/ComboBoxTheme.axaml:25-35`

```xml
<atom:AddOnDecoratedBox.ContentRightAddOn>
    <StackPanel Orientation="Horizontal" Spacing="...">
        <ContentPresenter Name="PART_ContentRightAddOnPresenter" />
        <ContentPresenter Name="PART_FormFeedBack" .../>
    </StackPanel>
</atom:AddOnDecoratedBox.ContentRightAddOn>
```

`PART_FormFeedBack` 即使没有 `FormFeedback` 内容也总是创建 `ContentPresenter`。建议沿用 host 模式按需 materialize。

复评：

- 结论成立。`ComboBoxHandle` 通常需要默认存在，`ContentRightAddOnPresenter` 与 `PART_FormFeedBack` 可以按需创建。
- 这可以与 Select 系 host 共用一套轻量 accessory host 思路，但不要把 ComboBox 强行塞进 Select 专属 owner 类型。

### Gap 5: AddOnDecoratedBox 外层 addon 4 个 presenter 仍无条件创建

文件：`src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox/Themes/AddOnDecoratedBoxTheme.axaml:9-67`

`PART_LeftAddOn`、`PART_RightAddOn`、`PART_ContentLeftAddOn`、`PART_ContentRightAddOn` 这 4 个 `ContentPresenter` 默认都创建，靠 `IsVisible="{TemplateBinding LeftAddOn, Converter=...IsNotNull}"` 隐藏。

最终数据里默认 `LineEdit.Default` 的 `Visual/root=20.0`、`ContentPresenter/root=5.0`，其中至少 4 个就是这些空槽位 presenter。

Phase 3.2 显式 defer 了这个（"不建议第一步做全局 simple/full XAML 模板切换"），但当下情况已经变了：

- `ContentRightAddOn` 已经在 `LineEdit` / `SearchEdit` 上做了 owner-driven。
- `LeftAddOn` / `RightAddOn` / `ContentLeftAddOn` 在运行期变化频次比 right accessory 更低，simple/full 切换或代码侧 host 的风险更小。

建议路线（任选其一）：

- **A. 代码侧 host（推荐）**：`AddOnDecoratedBox.OnApplyTemplate()` 找到 `Border#PART_ContentFrame` 后，以编程方式把 outer addon 节点附加到 `RootLayout` 的 left/right。slot 不存在时不创建。
- **B. simple/full 两套模板**：根据 `HasOuterAddOn = LeftAddOn != null || RightAddOn != null` 切 template。运行时切换时重建一次模板树。

无论哪种都要确保 CompactSpace 圆角、`UpdateIconStatusColors()`、`/template/` selector 行为一致。

复评：

- 结论成立，但风险高于单个派生控件 accessory host。
- 当前默认 `LineEdit.Default` 仍有 `ContentPresenter/root=5.0`，其中大部分来自 `AddOnDecoratedBox` 的固定 slot presenter。
- 推荐优先做代码侧 slot host，而不是 simple/full 两套模板。理由是 runtime addon 切换、CompactSpace 圆角和 status/icon brush 更新都集中在代码里更容易做最小变更和验证。

### Gap 6: 外层 addon 的 `/template/` selector 仍在批量激活

文件：`src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox/Themes/AddOnDecoratedBoxTheme.axaml`

仍保留：

- `Style Selector="^ /template/ ContentPresenter#PART_LeftAddOn"`（line 84）
- `Style Selector="^ /template/ ContentPresenter#PART_RightAddOn"`（line 89）
- `Style Selector="^[SizeType=Large] /template/ ContentPresenter#PART_LeftAddOn"`（line 100-138）
- `Style Selector="^:outline[Status=Error] /template/ ContentPresenter#PART_LeftAddOn,^:outline[Status=Error] /template/ ContentPresenter#PART_RightAddOn"`（line 202）
- `Style Selector="^:filled /template/ ContentPresenter#PART_LeftAddOn,..."`（line 214、222、229、236、242）
- `Style Selector="^:disabled:outline /template/ ContentPresenter#PART_LeftAddOn,..."`（line 254）

问题：

- 每个实例都参与这些 selector 匹配 + activator 订阅。
- 没有 outer addon 时仍然激活完整状态计算路径。

Phase 4 计划里写了"outer addon 的 `/template/` 视觉 selector 暂时保留"。这是 Phase 4 的延续：

建议路线：

- 新增 `EffectiveLeftAddOnBackground` / `EffectiveLeftAddOnBorderBrush` / `EffectiveRightAddOnBackground` / `EffectiveRightAddOnBorderBrush`、`EffectiveOuterAddOnPadding` 等 styled properties。
- 由 `AddOnDecoratedBox.ConfigureEffectiveOuterAddOnBrushes()` 在 `StyleVariant` / `Status` / `IsEffectivelyEnabled` / `SizeType` 变化时统一计算。
- 默认主题里 outer addon presenter 的 `Background` / `BorderBrush` / `Padding` 改为 `TemplateBinding Effective…`。
- 删除上面这一批 `/template/` selector。

复评：

- 结论成立，应和 Gap 5 同步或紧随其后处理。
- 如果 Gap 5 改成代码侧 materialize，outer addon 的 Background / BorderBrush / Padding 更适合由 `AddOnDecoratedBox` 写入 effective property 或直接初始化到 presenter，减少 selector 激活。

### Gap 7: LineEditAccessoryHost 一次属性变化触发整轮重算

文件：`src/AtomUI.Desktop.Controls/Input/LineEditAccessoryHost.cs:104-117`

```csharp
_ownerSubscriptions = new CompositeDisposable
{
    owner.GetObservable(TextBox.IsEffectiveShowClearButtonProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
    owner.GetObservable(TextBox.ClearIconProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
    // ... 共 11 条订阅
};
```

每条 subscription 触发 `HandleOwnerPropertyChanged → UpdateAccessoryState`，`UpdateAccessoryState` 跑一遍包含：

- `UpdateClearButton`
- `UpdateRevealButton`
- `UpdateFormFeedbackPresenter`
- `UpdateInnerRightContentPresenter`
- `UpdateCountTextIndicator`
- `EnsureChildOrder`

问题：

- 一次性把 `ClearIcon` / `IsMotionEnabled` / `IsEffectiveShowClearButton` 都改了 → 3 次完整重算。
- 默认 init 时这些属性会按顺序触发，11 条 subscription 在 init/template 应用阶段会多次触发 `UpdateAccessoryState`。

建议路线：

- 加一个 dirty flag：`_accessoryStateDirty = true` + `Dispatcher.Post(ApplyAccessoryState, DispatcherPriority.Render)`，合并到一次 frame 内。
- 或者按 property 分类：仅与 clear button 相关的属性只调 `UpdateClearButton`，与 reveal 相关只调 `UpdateRevealButton`。
- 注意不要回退到 `IsAttachingOwner` 时还需要立即同步状态（首帧不能空）。

复评：

- 结论成立，但优先级低于未处理控件的默认节点成本。
- 当前 `UpdateAccessoryState()` 内部多数分支是幂等 no-op，主要问题是 init / 批量属性变更时重复跑完整流程。
- 如果做 dirty 合并，attach 阶段必须保留同步更新，否则首帧可能缺 accessory。

### Gap 8: LineEdit 仍同时维护三套 accessory 接线路径

文件：`src/AtomUI.Desktop.Controls/Input/LineEdit.cs:135-165`

`OnApplyTemplate` 中：

1. 找到 `PART_RightAccessoryHost` → template 已经放置了 host。
2. 否则尝试 `SetupContentRightAddOnBindings()` → 旧 axaml part 模式（`PART_ClearButton` 等命名）。
3. 否则走 `ConfigureOwnerDrivenAccessoryHost()` → owner 自己 materialize host。

Phase 2 计划"删除或压缩 `SetupContentRightAddOnBindings()`"，最终是当 fallback 留下了。当前 LineEdit.cs 维护三条 path 任意一处改动都要同步三处。

建议路线：

- 短期：在 `SetupContentRightAddOnBindings` 前面加 `[Obsolete]` 注释 + 文档明确移除节点（比如下个 minor 版本随官方 Theme 包升级）。
- 长期：清掉 legacy 命名的 part fallback，只保留 host 路径（template-provided / owner-driven）。

复评：

- 结论成立，这是技术债，不是当前最大性能点。
- private 方法不能靠 `[Obsolete]` 给外部迁移信号。更合适的是在文档中标记 legacy theme fallback，并在下一次破坏性主题升级时删除。

### Gap 9: SearchEdit accessory 配置不对称

文件：`src/AtomUI.Desktop.Controls/Input/SearchEdit.cs:90-101`

```csharp
private protected override LineEditAccessoryHost CreateAccessoryHost()
{
    return new LineEditAccessoryHost
    {
        IsFormFeedbackSlotEnabled    = false,
        IsCountIndicatorSlotEnabled  = false
    };
}
```

只关掉了 `FormFeedback` / `CountIndicator`，但 `Reveal` / `InnerRightContent` 仍然走默认开启路径。如果 `SearchEdit` 业务上不希望支持 reveal（密码 reveal 不适合搜索框），需要在 host 上加同样的 enable 开关并显式关掉。

当前实现允许 `SearchEdit` 上仍然出现 reveal button — 这是行为问题，不是性能问题。建议：

- 明确每个 accessory 在派生控件上的语义。
- 不希望支持的 accessory 显式关闭，避免被设置后意外 materialize。

复评：

- 默认路径下没有 reveal / inner-right 的实际 materialize 成本，因为 host 是按需创建。
- 是否允许 `SearchEdit` 设置 reveal / inner-right 属于 API 语义问题。若确认搜索框不支持 reveal，应显式关闭；若保留扩展性，则不应作为性能优化项处理。

### Gap 10: AddOnDecoratedBoxPerfProbe 是 Debug 全局静态

文件：`src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox/AddOnDecoratedBoxPerfProbe.cs`

`#if DEBUG` 下用 `Interlocked` 累加 long。Release 构建里整段被裁掉，所以无运行成本。但是逻辑上有个风险：

- Debug 下并发实例化场景里只能拿到全局调用计数，不能区分实例。
- 当前作为回归基线没问题，未来想做单实例的 perf assertion 需要换成 thread-static / per-instance counter。

仅是设计提示，非性能问题。

复评：

- 判断为非性能问题。
- 文档中“Release 构建里整段被裁掉”不完全准确：当前调用点在 `#if DEBUG` 下，Release 热路径无调用；但 `AddOnDecoratedBoxPerfProbe.cs` 文件本身没有整体条件编译。

## 推荐下一阶段优先级

按收益和风险：

1. **Gap 2（TextArea）**：范围最小、收益明确。先做 `TextAreaAccessoryHost` 或抽最小 owner 接口验证 host 模式可迁移。
2. **Gap 1（Select / TreeSelect / Cascader）**：新增 `SelectAccessoryHost` 或共享 helper，三个控件复用；优先按需创建 `SelectMaxCountIndicator` 和自定义 right add-on presenter。
3. **Gap 4（ComboBox FormFeedBack）**：与 Select 系 accessory host 思路一致，但保留独立 owner 边界。
4. **Gap 3（InfoPickerInput / RangeDatePicker / RangeInfoPickerInput）**：保留默认 info icon，按需创建 clear button、feedback presenter 和自定义 right add-on presenter。
5. **Gap 5（AddOnDecoratedBox outer / inner slot presenter）**：代码侧 materialize；预计默认 `LineEdit.Default` 仍可继续降 visual count，但需要单独回归。
6. **Gap 6（outer addon `/template/` selector）**：与 Gap 5 同步或紧随其后收敛到 `Effective…` 属性。
7. **Gap 7（LineEditAccessoryHost batch）**：dirty flag + Dispatcher.Post 合并重算，保留 attach 阶段同步更新。
8. **Gap 8（LineEdit 三路 accessory path）**：文档标记 legacy fallback，后续主题破坏性升级时删除。
9. **Gap 9（SearchEdit reveal/inner-right 显式关闭）**：先做 API 语义确认，不进入性能主线。
10. **Gap 10（PerfProbe 诊断边界）**：非性能项，可后续顺手整理。

## 验证建议

每完成一个 Gap，跑一次：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
    -c Debug --framework net10.0 -- \
    --count 60 \
    --markdown docs/performances/AddOnDecoratedBox/addon-decorated-box-followup-<gap>.md
```

并跑：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
    -c Debug --framework net10.0 -- \
    --verify-accessories --verify-effective-brushes --verify-addon-states
```

确保现有 headless 验证不回退。

## 风险边界

- 不建议一次性把 Gap 1 / 2 / 3 / 4 全部并到一个 PR — accessory host 的 owner subscription 边界容易踩坑，分控件分阶段验证。
- Gap 5 / 6 改动 AddOnDecoratedBox 模板结构，会影响 SearchEdit / TextArea / ButtonSpinnerDecoratedBox 等所有派生 decorated box，需要单独一轮回归。
- Gap 7 改动 LineEditAccessoryHost 的事件时序，要在 init / template 应用 / runtime 三个时间点分别验证首帧状态正确。
