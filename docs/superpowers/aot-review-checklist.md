# AtomUI AOT Review 清单

分支：`feature/aot`

用途：逐项跟踪 AOT 相关修改。每 review 通过一项，就把对应项改成 `[x]`，并把状态标为 `已通过`。

状态说明：

- `[ ] 待 review`：还没有 review。
- `[x] 已通过`：已经 review 并接受。
- `[ ] 需要修改`：已经 review，但需要继续修复后再确认。

通用 review 约束：

- AOT 改造不能改变原有绑定语义。
- AOT 改造不能引入新的资源泄露：新增订阅、binding、event handler、缓存和生成对象必须有明确生命周期，template reapply 时必须释放旧 template part 相关资源。
- AOT 改造不能引入明显性能退化：替换 reflection binding 时优先使用 `AvaloniaProperty`、`GetObservable`、直接 `Bind` 或生成代码，避免运行时字符串 path 解析、反射扫描、动态代码生成和不必要的 per-instance 额外对象。

## 当前 Review 进度

| Review 项 | 状态 | 提交状态 | 说明 |
| --- | --- | --- | --- |
| AOT-RV-001：BindUtils AOT-safe relay binding | `[x] 已通过` | 已提交 | `OneWay`、`OneTime`、`TwoWay`、`OneWayToSource` 和 getter overload 绑定语义已确认。 |
| AOT-RV-002：Template-parent binding 替换 | `[x] 已通过` | 已随 `fix(Button): preserve loading margin with AOT-safe binding` 提交 | HyperLinkButton loading margin 已恢复旧绑定语义，并移除 AXAML reflection binding。 |

最近一次验证快照：

- `src/` AOT analyzer：`dotnet build src/AtomUI.Core/AtomUI.Core.csproj -c Release --no-incremental /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
- 全解决方案 AOT analyzer：`dotnet build AtomUI.slnx -c Release --no-incremental /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Error(s)`。剩余 warning 在 `controlgallery/AtomUIGallery`，主要来自 ReactiveUI expression binding 和 demo 里的反射路径。

## Review 项

### AOT-RV-001：BindUtils AOT-safe relay binding

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Core/Data/BindUtils.cs`
- 为什么这样改：
  - 移除 core relay helper 里的 `new Binding(...)` / 字符串路径绑定，因为 Avalonia reflection binding 对 AOT 不安全。
  - 强约束是不能改变旧绑定语义：`BindingPriority`、初始值传播、source 写回、`IDisposable` 清理行为都必须和旧 `Binding` 对齐。
- Review 重点：
  - `OneWay` 仍然委托给 `target.Bind(..., priority)`。
  - `OneTime` 按指定 priority 写入，并和 Avalonia local/direct value 的释放语义保持一致。
  - `TwoWay` 的 source-to-target 走 Avalonia `Bind`，target-to-source 只处理 target 后续变化。
  - `OneWayToSource` 跳过 target 初始值，只把后续 target 变化写回 source。
  - `object + Func getter` 使用 observable + `target.Bind(..., priority)`，不再手写 `SetCurrentValue`。
- 验证：
  - legacy binding 对照测试已覆盖并通过：`OneWay`、`OneTime`、`TwoWay`、`OneWayToSource`、`object + Func getter`。
  - Core AOT analyzer 构建已通过。

### AOT-RV-002：Template-parent binding 替换

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Core/Theme/BaseControlTheme.cs`
  - `src/AtomUI.Desktop.Controls/Buttons/HyperLinkButton.cs`
  - `src/AtomUI.Desktop.Controls/Buttons/Themes/HyperLinkButtonTheme.axaml`
  - 使用生成代码或强类型 template-parent binding 的控件
- 为什么这样改：
  - 字符串 template-parent path 会创建 reflection binding path，在 trimming/AOT 下不安全。
  - 替代实现应尽量使用 `AvaloniaProperty` 或 compiled binding path。
- Review 重点：
  - 字符串 overload 已标记为编译期不可用，能阻止新的 unsafe 调用。
  - 新 overload 保留旧的 binding mode、priority、converter 和目标属性行为。
  - AXAML 修改不能改变视觉状态或 style selector 行为。
- 当前 review 备注：
  - `HyperLinkButtonTheme.axaml` 中直接删除 `PART_LoadingIcon.Margin` 的 `ReflectionBinding` 不严格等价；默认 token/style 路径下两个 icon 的 margin 一致，但旧绑定会跟随 `PART_ButtonIcon.Margin` 的任意 effective value。
  - `[x] 已通过`：已改为在 `HyperLinkButton.OnApplyTemplate` 中用 `PART_LoadingIcon.Bind(Margin, PART_ButtonIcon.GetObservable(Margin), LocalValue)` 恢复旧语义，同时避免 AXAML reflection binding。
  - 资源生命周期：`HyperLinkButton.OnApplyTemplate` 会先 dispose 旧 binding，再绑定新的 template part，避免 template reapply 后旧 part 被订阅链持有。
  - 性能影响：新实现每个 `HyperLinkButton` 保留一个 margin 观察关系，和旧 `ReflectionBinding` 的实例级绑定数量一致；实现改为直接 `AvaloniaObject.Bind + GetObservable`，不再做 reflection path 解析，运行时成本低于旧 reflection binding。
- 验证：
  - 构建应能发现仍在调用 obsolete string overload 的位置。
  - 建议对变更 template binding 的控件做视觉 smoke check。
  - `HyperLinkButton` margin 同步修复后，`dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Release --no-incremental /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `HyperLinkButton` margin 同步修复后，`dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Release --no-incremental /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。

### AOT-RV-003：Theme token 注册与 resource key cache

- 状态：`[ ] 待 review`
- 主要文件：
  - `src/AtomUI.Core/Theme/ControlTokenRegistration.cs`
  - `src/AtomUI.Core/Theme/ThemeManager.cs`
  - `src/AtomUI.Core/Theme/ThemeManagerBuilder.cs`
  - `src/AtomUI.Core/Theme/IThemeManagerBuilder.cs`
  - `src/AtomUI.Core/Theme/ThemeConfigProvider.cs`
  - `src/AtomUI.Core/Theme/ThemeResourceKeyCache.cs`
  - `src/AtomUI.Core/Theme/TokenSystem/AbstractDesignToken.cs`
  - `src/AtomUI.Core/Theme/TokenSystem/AbstractControlDesignToken.cs`
- 为什么这样改：
  - Theme token 发现和 resource key 创建之前依赖运行时类型/成员反射。
  - AOT-safe 注册需要显式 control token metadata，不能在运行时扫描任意成员。
- Review 重点：
  - Token 注册顺序仍然确定。
  - Global、alias、control token 的 resource key 保持稳定。
  - Resource cache 的失效行为不变。
  - 用户自定义 theme 仍能注册并解析预期 token。
- 验证：
  - 在 Gallery 或 focused sample 中检查 theme 切换和 token override。

### AOT-RV-004：Theme manager builder extension 生成

- 状态：`[ ] 待 review`
- 主要文件：
  - `src/AtomUI.Generator/AtomUI.Generator.csproj`
  - `src/AtomUI.Generator/DesignToken/ControlTokenTypePoolClassWriter.cs`
  - `src/AtomUI.Controls/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator/ControlTokenTypePool.g.cs`
  - `src/AtomUI.Desktop.Controls/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator/ControlTokenTypePool.g.cs`
  - `src/AtomUI.Desktop.Controls.ColorPicker/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator/ControlTokenTypePool.g.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator/ControlTokenTypePool.g.cs`
  - `src/AtomUI.Controls/ThemeManagerBuildExtensions.cs`
  - `src/AtomUI.Desktop.Controls/ThemeManagerBuilderExtensions.cs`
  - `src/AtomUI.Desktop.Controls.ColorPicker/ThemeManagerBuilderExtensions.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/ThemeManagerBuilderExtensions.cs`
- 为什么这样改：
  - 用生成 metadata 替代反射式 token type 发现。
  - Extension API 显式注册具体 token pool，便于 AOT 保留和分析。
- Review 重点：
  - 生成的 token pool 内容完整，没有遗漏控件。
  - Extension method 仍注册和之前相同的模块。
  - 生成文件的变更是有意的，并且可以由 generator 稳定复现。
- 验证：
  - 如果 generator 流程可用，重新生成 token 文件并比较 diff。
  - 构建所有消费 theme builder extension 的包。

### AOT-RV-005：Icon provider 构造与 factory 生成

- 状态：`[ ] 待 review`
- 主要文件：
  - `src/AtomUI.Core/Controls/Icon/IconProvider.cs`
  - `src/AtomUI.Core/Controls/Icon/IconProviderCache.cs`
  - `src/AtomUI.Icons.AntDesign/AntDesignIconProvider.cs`
  - `src/AtomUI.Icons.AntDesign/AntDesignIconProvider.Factory.g.cs`
  - `src/AtomUI.Icons.Shared/IconPackageGenerator.cs`
  - `src/AtomUI.Icons.Shared/DefaultIconPackageGenerator.cs`
- 为什么这样改：
  - Expression 编译和宽泛的 `Activator.CreateInstance` 对 AOT 不友好。
  - Icon factory 应显式或生成，避免动态代码并保证构造函数被保留。
- Review 重点：
  - 每个 icon enum value 都映射到正确 icon type/factory。
  - Cache key 行为和淘汰逻辑仍然正确。
  - 生成的 factory 文件被正确提交或能稳定生成。
  - 缺少 icon constructor 时失败信息清晰。
- 验证：
  - 抽查多个 AntDesign icon enum。
  - AOT analyzer 构建不再出现 icon provider 相关 warning。

### AOT-RV-006：Icon brush clone 路径

- 状态：`[ ] 待 review`
- 主要文件：
  - `src/AtomUI.Core/Controls/Icon/Icon.cs`
  - `src/AtomUI.Controls/Rate/RateItem.cs`
- 为什么这样改：
  - 旧实现可能通过反射或类似序列化的方式 clone icon 实例。
  - AOT-safe clone 应复用 drawing instructions，并在不反射构造的情况下应用 brush。
- Review 重点：
  - Clone 后 icon 保留 size、theme、stroke/fill brush、secondary brush、view box、animation settings 和 geometry transform。
  - 复用 drawing instruction 不会意外修改源 icon 状态。
  - `Rate` 在 normal、hover、selected、disabled 状态下视觉不变。
- 验证：
  - Icon 和 `Rate` 视觉 smoke check。

### AOT-RV-007：Reflection helper 注解与边界收敛

- 状态：`[ ] 待 review`
- 主要文件：
  - `src/AtomUI.Core/Reflection/ObjectExtension.cs`
  - `src/AtomUI.Core/Reflection/TypeMemberExtension.cs`
  - `src/AtomUI.Core/Utils/TypeHelper.cs`
  - `src/AtomUI.Core/Utils/AvaloniaPropertyReflectionExtensions.cs`
- 为什么这样改：
  - 部分反射 helper 仍有必要，但需要 trimming annotation、明确约束或有理由的 suppression。
  - AOT-compatible API 不能隐藏对 public constructor 或 public property metadata 的假设。
- Review 重点：
  - Suppression 足够局部且理由明确，没有掩盖大范围 unsafe reflection。
  - 公开 API 在需要调用方保留 metadata 时暴露 `DynamicallyAccessedMembers` 要求。
  - Metadata 被 trim 后 helper 不会静默返回错误结果。
- 验证：
  - `src/` AOT analyzer 构建保持 warning-free。

### AOT-RV-008：Language provider 与 dynamic resource 查找

- 状态：`[ ] 待 review`
- 主要文件：
  - `src/AtomUI.Core/Language/LanguageProvider.cs`
  - `src/AtomUI.Core/Data/` 下相关 resource binding helper
- 为什么这样改：
  - Resource/language provider 容易隐式依赖反射或运行时发现成员。
  - AOT-safe lookup 应显式，并保留运行时 culture/resource 行为。
- Review 重点：
  - Language variant 变化仍能通知所有依赖资源。
  - Resource lookup fallback 行为不变。
  - 没有新的强引用环导致 theme/language resource 无法释放。
- 验证：
  - 在 focused sample 或 Gallery 中运行时切换 language/theme。

### AOT-RV-009：Shared collection view 排序与构造反射

- 状态：`[ ] 待 review`
- 主要文件：
  - `src/AtomUI.Controls.Shared/Data/ListCollectionViews/ListCollectionView.cs`
  - `src/AtomUI.Controls.Shared/Data/ListCollectionViews/ListSortDescription.cs`
- 为什么这样改：
  - Dynamic collection view 会在运行时推断 item type 和 comparer。
  - AOT 路径需要在不支持动态代码时避免 `MakeGenericType`，并明确 `AddNew` 对 metadata 的要求。
- Review 重点：
  - `IComparable`、nullable、string、自定义 comparer 的排序语义不变。
  - 当 model constructor 可用时，`AddNew` 行为仍然保留。
  - AOT fallback 不应让常见 value type 排序静默失效。
- 验证：
  - 用 string、number、nullable、自定义 comparable item 做排序样例。

### AOT-RV-010：DataGrid 动态 model binding 与自动生成列

- 状态：`[ ] 待 review`
- 主要文件：
  - `src/AtomUI.Desktop.Controls.DataGrid/Data/DataGridCollectionView.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/Data/DataGridDataConnection.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/Data/DataGridSortDescription.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/DataGrid.Columns.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/Utils/DataGridValueConverter.cs`
- 为什么这样改：
  - DataGrid 自动生成列和排序高度依赖用户 model property 反射。
  - AOT-safe 行为需要基于 property-info/compiled path 的 accessor，并明确 trim 边界。
- Review 重点：
  - 自动生成列绑定到正确 property，并能响应 `INotifyPropertyChanged`。
  - 双向 cell editing 能通过新 accessor 写回。
  - Validation/conversion error 仍产生相同的 `BindingNotification` 行为。
  - 排序行为和旧 dynamic binding 一致。
- 验证：
  - 使用可编辑属性的自动生成 DataGrid 样例。
  - 排序与 validation 场景。

### AOT-RV-011：Control template-part binding 替换

- 状态：`[ ] 待 review`
- 主要文件：
  - `src/AtomUI.Desktop.Controls/Cascader/Cascader.cs`
  - `src/AtomUI.Desktop.Controls/ComboBox/ComboBox.cs`
  - `src/AtomUI.Desktop.Controls/Input/LineEdit.cs`
  - `src/AtomUI.Desktop.Controls/Input/TextArea.cs`
  - `src/AtomUI.Desktop.Controls/NumericUpDown/NumericUpDown.cs`
  - `src/AtomUI.Desktop.Controls/Primitives/InfoPickerInput/InfoPickerInput.cs`
  - `src/AtomUI.Desktop.Controls/Select/Select.cs`
  - `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelect.cs`
  - `src/AtomUI.Desktop.Controls/Drawer/Drawer.cs`
- 为什么这样改：
  - 很多 template-part binding 使用 `new Binding(nameof(...)) { Source = this }`，依赖 reflection binding。
  - 改成 `BindUtils.RelayBind` 可以移除字符串路径反射，同时保留绑定行为。
- Review 重点：
  - 每处替换都使用正确的 source/target `AvaloniaProperty`。
  - `ObjectConverters.IsNotNull` 等 converter 替换保持相同的 null/visibility 语义。
  - Two-way binding，特别是 reveal password 和 popup/dialog 属性，保持更新方向和清理行为。
  - Template detach/reapply 不泄露 binding。
- 验证：
  - 对每个变更控件做 template apply smoke test。
  - 检查 clear button、reveal password、selection handle、add-on、drawer nesting 交互。

### AOT-RV-012：NavMenu node binding

- 状态：`[ ] 待 review`
- 主要文件：
  - `src/AtomUI.Desktop.Controls/NavMenu/NavMenu.cs`
  - `src/AtomUI.Desktop.Controls/NavMenu/NavMenuItem.cs`
- 为什么这样改：
  - `INavMenuNode` item binding 原来对任意对象使用字符串路径。
  - AOT-safe 替代使用强类型 getter-based relay binding。
- Review 重点：
  - Node 的 icon、enabled state、header template 能在 node 触发 `PropertyChanged` 后更新。
  - 容器复用时会释放旧 node binding。
  - 当 node header template 为空时，`ItemTemplate` fallback 仍然正确。
- 验证：
  - 使用可变 `INavMenuNode` 和容器复用场景测试 NavMenu。

### AOT-RV-013：ReactiveWindow AOT 边界

- 状态：`[ ] 待 review`
- 主要文件：
  - `src/AtomUI.Desktop.Controls/Window/ReactiveWindow.cs`
- 为什么这样改：
  - ReactiveUI activation API 可能因为 expression-based binding path 带有 trim/AOT annotation。
  - AtomUI wrapper 应明确这个边界，让下游应用知道使用要求。
- Review 重点：
  - Attribute 或 suppression 准确描述 ReactiveUI 风险。
  - 与现有 `ReactiveWindow<TViewModel>` 用户的 constructor/API 兼容性不变。
  - 不制造“ReactiveUI expression binding 已完全 AOT-safe”的误导。
- 验证：
  - 用 analyzer 构建，并检查 app/Gallery 层 warning。

### AOT-RV-014：Generated files 与 source generator packaging

- 状态：`[ ] 待 review`
- 主要文件：
  - `src/AtomUI.Generator/AtomUI.Generator.csproj`
  - 生成的 `ControlTokenTypePool.g.cs` 文件
  - `src/AtomUI.Icons.AntDesign/AntDesignIconProvider.Factory.g.cs`
- 为什么这样改：
  - AOT-safe metadata 依赖生成产物确定且被正确包含。
  - Generator packaging 不能在 NuGet 场景下漏掉 analyzer/source output。
- Review 重点：
  - 生成产物是有意提交，或作为 build 的一部分生成。
  - Incremental build 和 clean build 产生相同源码。
  - Packaging 包含必要 generator assets。
- 验证：
  - 从空 `output/` clean build。
  - 可选：重新生成后做 diff 检查。

### AOT-RV-015：剩余 controlgallery AOT warning

- 状态：`[ ] 待 review`
- 主要文件：
  - `controlgallery/AtomUIGallery/**`
- 为什么这样改：
  - 全解决方案 analyzer 仍报告 Gallery warning，主要来自 ReactiveUI expression API 和 demo reflection。
  - 这些 warning 不阻塞 `src/` library AOT 状态，但需要单独跟踪。
- Review 重点：
  - 决定 Gallery 是否纳入 AOT 发布范围。
  - 如果纳入，需要替换 ReactiveUI expression binding 或制定明确 preservation/suppression 策略。
  - 如果不纳入，应记录 Gallery 是非 AOT-clean demo app。
- 验证：
  - 当前全解决方案 AOT analyzer 能成功退出，但仍有 warning。
