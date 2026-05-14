# Select 性能优化方案

## 背景

`Select` 是 AtomUI 数据录入体系中的高频控件，而且它不是孤立控件。`TreeSelect`、`Cascader` 直接继承 `AbstractSelect`，`ComboBox`、`AutoComplete`、`InfoPickerInput` 也采用了相近的 AddOnDecoratedBox、accessory、popup/candidate-list 架构。

本轮优化原则保持一致：

**不使用的功能不承担成本。**

本方案先做 review，不实施。优化优先级按正确性、资源释放、可验证性、收益排序。

## 当前观测

### SelectShowCase 真实 Gallery 基线

命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-restore -- \
  --showcase select --warmup 2 --iterations 5 --timeout-ms 15000
```

结果：

| 指标 | 当前值 |
| --- | ---: |
| source `atom:Select` | `33` |
| source `ShowCaseItem` | `14` |
| cold first navigation | `494.50ms` |
| repeated navigation mean | `220.31ms` |
| repeated navigation median | `215.32ms` |
| repeated navigation p95 | `249.40ms` |
| repeated allocation mean | `28027.36KB` |
| runtime visuals | `1483` |
| runtime logical | `68` |
| runtime Select | `33` |
| runtime AddOnDecoratedBox | `33` |
| runtime Icon | `46` |
| runtime IconPresenter | `59` |
| runtime PathIcon | `46` |
| runtime ToggleIconButton | `17` |

解释：

- 33 个 `Select` 打开页面 repeated 约 `220ms`，alloc 约 `28MB`，对一个控件展示页偏高。
- 运行时 `AddOnDecoratedBox=33` 是预期成本，但每个 Select 背后还有默认 accessory、handle、popup、candidate list、filter/result box 等隐形成本。
- `PathIcon=46` 只统计已进入 visual tree 的图标；当前默认 loading icon 即使没有进入 visual tree，也仍在 `AbstractSelect.OnInitialized()` 被分配。

### SpaceShowCase item 7 拆解

`SpaceShowCase` 第 7 个 `ShowCaseItem` 是 `Compact Mode for form component`，其中包含 7 个 `Select`、1 个 `TreeSelect`、1 个 `Cascader`，并混合 LineEdit、Picker、Button 等复合控件。

已有拆解结果：

| 场景 | stable mean | stable median | visuals | Select | AddOnDecoratedBox | alloc mean |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| 原始 item 7 | `100.92ms` | `100.84ms` | `1123` | `7` | `36` | `21772KB` |
| 去掉 TreeSelect + Cascader | `94.00ms` | `89.03ms` | `1055` | `7` | `34` | `20588KB` |
| 再去掉 Select | `81.73ms` | `72.09ms` | `866` | `0` | `27` | `16897KB` |

解释：

- TreeSelect/Cascader 有成本，但 7 个 `Select` 的增量更明显。
- 在去掉 TreeSelect/Cascader 后，再去掉 7 个 `Select`，stable mean 下降 `12.27ms`，alloc 下降约 `3691KB`，visuals 下降 `189`。
- 单个 Select 在这个场景下大约贡献 `1.75ms` stable mean、约 `527KB` alloc、约 `27` 个 visual 的量级。这个成本对普通表单密集页面非常危险。

## 控件家族

### 核心继承链

- `AbstractSelect`
  - `Select`
  - `TreeSelect`
  - `Cascader`

这些控件共享：

- 默认 suffix/loading icon 初始化
- 下拉打开/关闭状态
- popup placement、popup width、max height
- clear/loading/form feedback/accessory handle
- `AddOnDecoratedBox` 视觉封装
- compact space 状态透传

### 同构控件

- `ComboBox`：继承 Avalonia `ComboBox`，但自己实现 `AddOnDecoratedBox`、accessory host、popup frame、handle。
- `AutoComplete`：有 input + popup + candidate list 结构。
- `InfoPickerInput`：Picker 类输入控件的 accessory host 与 AddOnDecoratedBox 体系。

本轮应先优化 `AbstractSelect` 与 `Select`，再把低风险收益同步到 `TreeSelect` / `Cascader`，最后评估是否抽取到 ComboBox/AutoComplete/Picker 同构层。

## 性能问题清单

### P0: 正确性问题优先

`SelectTagAwareTextBox.IsResponsiveTagModeProperty` 当前注册 owner 是 `SelectResultOptionsBox`：

```csharp
public static readonly StyledProperty<bool> IsResponsiveTagModeProperty =
    Select.IsResponsiveTagModeProperty.AddOwner<SelectResultOptionsBox>();
```

位置：`src/AtomUI.Desktop.Controls/Select/SelectTagAwareTextBox.cs`

这明显应该是：

```csharp
Select.IsResponsiveTagModeProperty.AddOwner<SelectTagAwareTextBox>();
```

这是正确性问题，不属于性能优化，应优先独立修复并补验证。后续凡是发现此类 owner/template binding 明显错误，优先级高于性能工作。

### P1: 默认初始化创建了未使用的 loading icon

`AbstractSelect.OnInitialized()` 默认创建：

- `SuffixIcon = new DownOutlined()`
- `SuffixLoadingIcon = new LoadingOutlined { LoadingAnimation = IconAnimation.Spin }`

位置：`src/AtomUI.Desktop.Controls/Select/AbstractSelect.cs`

问题：

- 默认下拉箭头是可见功能，创建成本合理，但仍应只由 handle 可见路径承担。
- loading icon 只有 `IsLoading=true` 才需要，现在每个 Select/TreeSelect/Cascader 都分配。
- 这类 hidden allocation 不会完全体现在 visual count 中，但会体现在页面打开 alloc 和 GC 压力上。

建议：

- 保留 `SuffixLoadingIcon` 公共属性，不改 API。
- 默认值不在 `OnInitialized()` 创建。
- `SelectHandle` 进入 loading 状态时，如果用户没有提供 `SuffixLoadingIcon`，由代码创建内部默认 loading icon。
- loading 结束或 handle detach 时，清理默认 icon 引用、visual parent、templated parent。

### P2: 每个 Select 都创建 accessory host、订阅和 SelectHandle

`AbstractSelect.ConfigureOwnerDrivenAccessoryHost()` 在模板应用时总是创建 `SelectAccessoryHost` 并塞进 `ContentRightAddOn`。

`SelectAccessoryHost.AttachOwner()` 立即订阅大量属性：

- max count / selected count / content right add-on
- form feedback / suffix icon / loading icon / filter / enabled / motion
- loading / allow clear / selection empty / dropdown open
- Select 的 effective filter
- AddOnDecoratedBox hover / pressed

随后 `UpdateAccessoryState()` 总是调用 `UpdateSelectHandle()`，也就是默认路径也创建 `SelectHandle`。

位置：

- `src/AtomUI.Desktop.Controls/Select/AbstractSelect.cs`
- `src/AtomUI.Desktop.Controls/Select/SelectAccessoryHost.cs`

问题：

- 默认单选 Select 实际只需要一个下拉箭头，但承担了 max count、自定义 content right add-on、form feedback、clear、loading、hover/pressed 等完整管线。
- hover/pressed 订阅只有 clear 按钮可见判断需要，但现在所有 Select 都订阅。
- owner 与 host 双向引用、ContentRightAddOn 注入、订阅释放都增加生命周期风险。

建议：

- 引入轻量默认 accessory 路径：
  - 默认仅创建 `SelectHandle` 或更轻量的 `SelectHandlePresenter`。
  - 只有启用 `IsShowMaxCountIndicator`、`ContentRightAddOn`、form feedback 等复合场景时才升级为 `SelectAccessoryHost`。
- 由 owner 在 `OnPropertyChanged()` 中同步 accessory 状态，减少 host 对 owner 的长期 `GetObservable()` 订阅。
- hover/pressed 只在 clear 可能显示时订阅：
  - `IsAllowClear && !IsSelectionEmpty`
  - 关闭 clear 或 selection empty 后释放订阅。
- re-template / detach 必须清空 `ContentRightAddOn` 中的内部 host/handle，避免 visual parent 异常。

### P3: closed Select 仍创建 popup 内容和 candidate list

`SelectTheme.axaml` 模板默认创建：

- `Popup#PART_Popup`
- `Border#PopupFrame`
- `SelectCandidateList#PART_CandidateList`

即使下拉从未打开，`Select.OnApplyTemplate()` 也会获取 candidate list 并挂事件：

- `SelectionChanged`
- `Commit`
- `Cancel`

位置：

- `src/AtomUI.Desktop.Controls/Select/Themes/SelectTheme.axaml`
- `src/AtomUI.Desktop.Controls/Select/Select.cs`

问题：

- 绝大部分表单页面打开时 Select 都处于关闭状态，但每个 Select 都提前构建候选列表。
- candidate list 继承 ListView，包含 selection、filter、empty indicator、virtualizing panel、item container 等复杂管线。
- 默认关闭状态不应该承担下拉列表、过滤、分组、hide selected、max count、auto scroll 等成本。

建议：

- 保留 `Popup#PART_Popup` shell，避免重写基础开关与 light dismiss 语义。
- 将 popup 内容 lazy materialize：
  - 第一次打开前创建 `PopupFrame` 和 `SelectCandidateList`。
  - 创建后挂事件、同步 ItemsSource/filter/selection/mode。
  - 关闭后默认保留已创建内容，避免反复打开抖动；detach/re-template 时释放。
- `IsDropDownOpen=true`、键盘打开、async options loaded 后打开、未来 `IsDefaultOpen` 都必须走同一 materialize 入口。
- `SelectedOption/SelectedOptions` 改变时，只在 candidate list 已创建时同步到列表。

### P4: hidden mode-specific 控件默认创建

`SelectTheme.axaml` 默认创建：

- single result presenter
- `SelectFilterTextBox#PART_SingleFilterInput`
- `SelectResultOptionsBox#SelectedOptionsBox`

`SelectResultOptionsBox.OnApplyTemplate()` 又默认创建：

- `SelectFilterTextBox`
- `SelectRemainInfoTag`
- default panel / max-count-aware panel

位置：

- `src/AtomUI.Desktop.Controls/Select/Themes/SelectTheme.axaml`
- `src/AtomUI.Desktop.Controls/Select/SelectResultOptionsBox.cs`

问题：

- 默认 `Mode=Single` 且 `IsFilterEnabled=false` 的 Select 不需要 filter textbox。
- Single mode 不需要 `SelectResultOptionsBox`。
- Multiple mode 如果没有 selected options，不应创建一堆 tag/remaining/search 子控件。
- `MaxTagCount` / `IsResponsiveTagMode` 是特殊功能，应只在启用后承担成本。

建议：

- Single + no filter：
  - 保留 placeholder 和 selected result 的轻量展示。
  - 不创建 `PART_SingleFilterInput`。
  - 不创建 `SelectResultOptionsBox`。
- Single + filter：
  - filter textbox 可在 `IsEffectiveFilterEnabled=true` 时创建。
  - 可评估延迟到首次打开，因为 closed 状态只需要显示 selected text/placeholder。
- Multiple/Tags：
  - `SelectResultOptionsBox` 只在 `Mode != Single` 时创建。
  - 搜索 textbox 只在 `IsEffectiveFilterEnabled=true` 或 `Mode=Tags` 时创建。
  - collapsed info tag 只在 `MaxTagCount != null || IsResponsiveTagMode=true` 时创建。
- selected tags 更新采用 diff 或小型对象池，避免每次 `SelectedOptions` 变化全量 new `SelectTag`。

### P5: TreeSelect / Cascader 继承了同一类 hidden cost

`TreeSelectTheme.axaml` 默认创建：

- `TreeSelectAddOnDecoratedBox`
- hidden filter textbox
- hidden `SelectTagAwareTextBox`
- `Popup`
- `TreeSelectTreeView#PART_TreeView`

`CascaderTheme.axaml` 默认创建：

- `CascaderAddOnDecoratedBox`
- hidden filter textbox
- hidden `SelectTagAwareTextBox`
- `Popup`
- `CascaderView#PART_CascaderView`

`TreeSelect.OnApplyTemplate()` / `Cascader.OnApplyTemplate()` 默认给 view 挂事件并执行 `Items.Cast<...>().ToList()` 复制，即使 popup 未打开。

位置：

- `src/AtomUI.Desktop.Controls/TreeSelect/Themes/TreeSelectTheme.axaml`
- `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelect.cs`
- `src/AtomUI.Desktop.Controls/Cascader/Themes/CascaderTheme.axaml`
- `src/AtomUI.Desktop.Controls/Cascader/Cascader.cs`

问题：

- TreeView/CascaderView 比 SelectCandidateList 更重，不应在 closed 状态默认创建。
- `IsDefaultExpandAll`、filter、async loader、selection sync 等应在 popup materialized 后执行。
- `ItemsSource = Items.Cast<...>().ToList()` 是 closed 状态的多余分配。

建议：

- 复用 `AbstractSelect` 的 lazy popup shell 思路。
- `TreeSelectTreeView` / `CascaderView` 第一次打开时创建。
- ItemsSource 只在 view 已创建时同步；closed 状态仅维护原始 `Items` / `Options`。
- selection/default path 同步改为“如果 view 已创建则同步，否则记录待同步状态”。
- `IsDefaultExpandAll` 只在 view 创建后执行一次。

### P6: 每个 closed Select 都订阅 Window.Deactivated

`AbstractSelect.OnAttachedToVisualTree()` 为每个实例订阅 `Window.Deactivated`，用于失焦关闭下拉。

位置：`src/AtomUI.Desktop.Controls/Select/AbstractSelect.cs`

问题：

- 页面上 33 个 Select 会有 33 个 window deactivated handler。
- 只有打开状态才需要响应 window deactivated。

建议：

- 改为下拉打开时订阅 window deactivated，关闭或 detach 时释放。
- `ComboBox` 也有同类逻辑，Select 完成后同步评估。

### P7: selection/default values 同步存在不必要扫描和复制

`Select.ConfigureDefaultValues()`：

- Single 模式扫描 `Options`
- Multiple/Tags 模式对每个 default value 再扫描所有 Options

`SelectedOptions` 同步到 candidate list 时多处 `ToList()`。

位置：`src/AtomUI.Desktop.Controls/Select/Select.cs`

问题：

- Gallery 目前 options 数量小，不是主瓶颈。
- 业务真实大 options 场景会放大，尤其是 default values 多选。
- candidate list 未创建时不应同步列表。

建议：

- default values 多选用 `HashSet<string>` 或用户 compare fn 的分支优化。
- 只在 `Options` 或 `DefaultValues` 首次可用时执行一次默认选中，避免重复。
- `_candidateList != null` 时才做 selection list copy。
- 大数据下进一步依赖虚拟化与 async loader，不在 closed 状态展开容器。

### P8: ComboBox / AutoComplete 同构问题

`ComboBoxTheme.axaml` 默认创建 `Popup`、`ScrollViewer`、`ItemsPresenter`，`ComboBox` 也有 owner-driven accessory host。

`AutoCompleteTheme.axaml` 默认创建 `Popup` 和 `CandidateList`。

问题：

- 它们不是 `AbstractSelect` 子类，但和 Select 有相同的 closed popup/list 默认成本。
- 不应在 Select 第一期里大范围改，避免风险扩散。

建议：

- Select 核心 lazy popup 和 accessory 机制稳定后，再提炼到共享 helper 或按控件复制低风险模式。
- ComboBox 优先评估 accessory host 订阅和 `Window.Deactivated` 按需化。
- AutoComplete 优先评估 candidate list 是否可在首次输入/打开时创建。

## 预期收益

保守目标以 Debug headless 为准：

| 阶段 | SelectShowCase repeated mean 目标 | SelectShowCase alloc 目标 | Space item 7 目标 |
| --- | ---: | ---: | --- |
| Phase 1-2 | `220ms -> 190-200ms` | `-10%` 到 `-15%` | 小幅下降，主要减少 hidden allocation |
| Phase 3-5 | `220ms -> 150-170ms` | `-25%` 到 `-35%` | 7 个 Select 增量显著下降 |
| Final | `<160ms` | `<20MB` | item 7 stable mean 进入 `70-85ms` 区间 |

不承诺 50% 以上整体提升，因为 `ShowCaseItem`、AddOnDecoratedBox、LineEdit、Picker、Button、Gallery route/layout 仍有固定成本。但 Select 自身的默认关闭状态成本应有明显下降。

## 验证要求

### 控件级验证

新增 `tools/performances/AtomUI.Performance/Suites/Select/`：

- `Select.Default.Single.Closed`
- `Select.Single.AllowClear.Empty`
- `Select.Single.AllowClear.Selected`
- `Select.Single.Filter.Closed`
- `Select.Single.Filter.Open`
- `Select.Multiple.Empty`
- `Select.Multiple.Selected`
- `Select.Tags.Empty`
- `Select.MaxCountIndicator`
- `Select.ContentRightAddOn`
- `Select.AsyncLoading`
- `TreeSelect.Default.Closed`
- `TreeSelect.Filter.DefaultExpandAll`
- `Cascader.Default.Closed`

新增验证：

- closed Select 不创建 `SelectCandidateList`
- first open 创建 candidate list，close 后状态正确
- detach/re-template 释放 candidate list 事件、accessory host、window deactivated handler
- `IsLoading` on/off/on 不泄露 loading icon
- `IsAllowClear` on/off/on 不重复订阅 hover/pressed
- `ContentRightAddOn` on/off/on 不留下旧 presenter visual parent
- `Mode` Single/Multiple/Tags 切换后 mode-specific 控件正确创建/移除
- `MaxTagCount` / `IsResponsiveTagMode` 切换后 collapsed info tag 行为正确
- `TreeSelect` / `Cascader` 第一次打开、关闭、再次打开行为正确

### Gallery 验证

必须保留真实 Gallery 场景：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase select --warmup 3 --iterations 10 --timeout-ms 15000
```

同时复测：

- `SpaceShowCase` item breakdown 原始场景
- `SpaceShowCase` 去掉 TreeSelect/Cascader
- `SpaceShowCase` 去掉 TreeSelect/Cascader/Select
- `TreeSelectShowCase`
- `CascaderShowCase`

### 资源释放验证

每个 lazy 创建点必须有释放路径：

- visual child remove 前确认没有旧 visual parent
- event subscription 存入 disposable 或显式 unsubscribe
- binding disposable 可释放
- re-template 前先 detach 旧 part
- detached from visual tree 时释放 window/subscription/popup content
- loading/search/clear icon 清理 `TemplatedParent` 和引用

## 最终验证数据

### SelectShowCase

| 指标 | 优化前 | 优化后 | 变化 |
| --- | ---: | ---: | ---: |
| repeated mean | `220.31ms` | `143.76ms` | `-34.7%` |
| alloc mean | `28027.36KB` | `23904.61KB` | `-14.7%` |
| visuals | `1483` | `1410` | `-73` |
| logical | `137` | `68` | `-69` |

### 控件级与关联 Gallery 场景

| 场景 | 结果 |
| --- | --- |
| `tools/performances/AtomUI.Performance --verify-select-states` | Passed |
| `tools/performances/AtomUI.Performance --suite select --count 60` | 默认 closed `Select`：`20` visuals/root，`SelectCandidateList=0`，`SelectAccessoryHost=0`，`SelectHandle=1` |
| `SpaceShowCase` item 7 | 最终 run：`146.36ms` mean，`1098` visuals，`20716.59KB` alloc；去掉 TreeSelect/Cascader/Select 后：`99.24ms` mean，`866` visuals，`16890.51KB` alloc |
| `TreeSelectShowCase` | repeated mean `107.47ms`，`720` visuals，`12213KB` alloc |
| `CascaderShowCase` | repeated mean `163.53ms`，`1250` visuals，`22862KB` alloc |

`SpaceShowCase` item 7 的 timing 仍明显偏高，且在不同 run 间波动较大；但 visual/alloc 已随 lazy popup 和 accessory 按需化下降。后续应继续针对 item 7 中的 LineEdit、Picker、Button、CompactSpace 组合成本拆分，而不是把剩余耗时全部归因于 Select。

## 实施任务列表

### Phase 0: Baseline 与测试基础设施

- [x] 新增 `tools/performances/AtomUI.Performance/Suites/Select/`。
- [x] 给控件级 `TreeStats` 增加 Select 相关细分计数：`SelectHandle`、`SelectAccessoryHost`、`SelectCandidateList`、`SelectFilterTextBox`、`SelectResultOptionsBox`、`TreeSelect`、`Cascader`、`ComboBox`。
- [x] 给 Gallery `RouteStats` 增加 `TreeSelect`、`Cascader` 细分计数，并补 `TreeSelectShowCase` / `CascaderShowCase` 路由。
- [x] 固化 `SelectShowCase` 当前基线与最终数据。
- [x] 固化 `SpaceShowCase` item 7 对照数据。
- [x] 补 closed/materialize/toggle/detach 验证。Headless 环境不能真实打开 platform Popup，因此控件级验证直接触发 lazy popup materialization；真实 Gallery 导航另行验证。

### Phase 1: 正确性与低风险 hidden allocation

- [x] 修复 `SelectTagAwareTextBox.IsResponsiveTagModeProperty` owner。
- [x] `SuffixLoadingIcon` 默认 lazy 创建，不再在 `OnInitialized()` 分配。
- [x] window deactivated 订阅改为 dropdown open 期间按需存在。
- [x] 验证 loading、form feedback、clear、window deactivate 相关状态不回归。

### Phase 2: Accessory host 与 handle 按需化

- [x] 设计默认轻量 handle 路径，避免默认 Select 创建完整 `SelectAccessoryHost`。
- [x] host 仅在 max count/custom content 等复合场景创建；form feedback 由轻量 handle 直接承载。
- [x] hover/pressed 订阅仅在 clear 按钮可能显示时创建。
- [x] owner 直接同步 accessory 状态，减少 host 对 owner 的 `GetObservable()` 订阅。
- [x] 补 on/off/on 和 detach 资源释放验证。

### Phase 3: Select popup/candidate list lazy materialization

- [x] 从默认 closed template 中移除 `SelectCandidateList` 实例化。
- [x] 第一次打开前创建 popup content 和 candidate list。
- [x] 将 candidate list 事件挂接、ItemsSource/filter/selection/mode 同步移动到 materialize 入口。
- [x] close 后保持内容，detach/re-template 释放内容。
- [x] 覆盖 group、hide selected、max count、auto scroll 等属性同步；真实键鼠打开行为需继续由 Gallery/手工交互覆盖。

### Phase 4: Select mode-specific content 按需化

- [x] Single + no filter 不创建 `SelectFilterTextBox`。
- [x] Single 不创建 `SelectResultOptionsBox`。
- [x] Multiple/Tags 才创建 result/tag 容器。
- [x] search textbox、collapsed info tag、responsive max tag 跟随 mode-specific 容器按需创建。
- [x] 评估 selected tags diff 更新：本轮不改 tag diff 算法，避免扩大行为风险。

### Phase 5: TreeSelect / Cascader 同步优化

- [x] `TreeSelectTreeView` 第一次打开时创建。
- [x] `CascaderView` 第一次打开时创建。
- [x] closed 状态不执行 `Items.Cast<...>().ToList()` / `Options.Cast<...>().ToList()`。
- [x] selection/default path/filter 状态在 view materialized 后同步。
- [x] Cascader 默认路径展示改为轻量解析，不依赖 `CascaderView` 已创建。
- [x] 验证 filter、multiple/checkable、max count、lazy view lifecycle；async loader 仍需真实打开交互继续覆盖。

### Phase 6: ComboBox / AutoComplete 同构评估

- [x] 完成同构评估：ComboBox/AutoComplete 不纳入本轮 Select 改动，需独立基线和独立风险评估。
- [x] 判断暂不抽取共享 lazy popup helper，先保留 `AbstractSelect` 内聚实现，避免影响非 Select 体系控件。

### Phase 7: Selector 与状态计算收敛

- [x] 评估 `SelectAddOnDecoratedBox` / `TreeSelectAddOnDecoratedBox` / `CascaderAddOnDecoratedBox` 的 mode/selection/size padding selector。
- [x] 本轮只迁移 hidden runtime cost，不继续迁移视觉 selector，避免为了性能过度牺牲样式表达。
- [x] 对照 `docs/performances/style-vs-code-guidelines.md` 做决策：高频状态/运行时创建归代码，纯视觉设计仍留 XAML。

### Phase 8: DefaultValues 与 selection sync 优化

- [x] Multiple/Tags default value 匹配在无自定义比较函数时使用 value lookup，减少 O(defaults * options) 扫描。
- [x] candidate list 未 materialized 时不做 selection list copy。
- [x] OptionsSource 更新继续保持清空语义，避免改变 API 行为；默认选中只在 loaded 阶段执行。

### Phase 9: Final 验证与文档

- [x] `dotnet build` owning projects。
- [x] 跑控件级 Select suite。
- [x] 跑 Gallery `SelectShowCase`、`SpaceShowCase` item 7、`TreeSelectShowCase`、`CascaderShowCase`。
- [x] 记录最终数据与提升百分比。
- [x] `git diff --check`。
