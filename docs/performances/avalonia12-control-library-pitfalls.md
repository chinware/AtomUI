# Avalonia 12 控件库研发性能陷阱总览

本文档聚焦 **AtomUI 这一类控件库** 在 Avalonia 12 上的研发场景，梳理框架内部已经会带来性能代价的所有关键点。
内容全部基于本仓库 `.referenceprojects/Avalonia` 的源码（路径前缀省略，下文 `<path>:<line>` 均指向该子树）。

每条结论后面都附 `<path>:<line>` 引用。控件性能优化讨论时，引用这里的条目，
不要再凭空假设"X 比 Y 便宜"。

> 配套文档：
> - 控件目录与基线：[`README.md`](README.md)
> - axaml selector vs C# 状态机：[`style-vs-code-guidelines.md`](style-vs-code-guidelines.md)
> - 优化流程纪律：`.agents/skills/atomui-control-performance/SKILL.md`

---

## 0. 阅读说明

- "陷阱"在本文档里指：默认实现成本不低、容易被新增控件无意触发、或者写法稍微偏离推荐就会成倍放大代价的点。
- 每节末尾给出"控件库结论"，是直接可以放进代码 review 的判据。
- 所有 `path:line` 引用在 Avalonia 12 当前 reference 快照下验证过；若后续 Avalonia 升级，需要重新核对。
- 对于"性能"和"正确性"冲突的场景，本文档严格优先正确性，参见 SKILL Tier 1 §1。

---

## 1. 属性系统

### 1.1 `StyledProperty<T>` 读取走 `ValueStore`，`DirectProperty<T>` 直接走字段

`StyledProperty.GetValue` 实际上是在 `ValueStore` 上做一次按 `AvaloniaProperty` 的字典查找；命中
则返回该 `EffectiveValue` 的当前值，未命中再走 inherited / default。
`DirectProperty` 完全绕过 `ValueStore`，直接调用注册时给的 getter 委托。

- `Avalonia.Base/PropertyStore/ValueStore.cs:352` — `IsSet` / `TryGetValue` 实现。
- `Avalonia.Base/AvaloniaObject.cs:251-258` — `GetValue` 分发。
- `Avalonia.Base/DirectPropertyBase.cs` — `GetValue` 直接经委托读字段。

**控件库结论**

- 频繁在 hot path（`MeasureOverride`、`ArrangeOverride`、`Render` 中）读取的、值由控件自身维护、
  不需要 Style/Animation/Template/Trigger 介入的纯运行时状态属性，注册成 `DirectProperty`。
- 所有对外暴露给主题/样式/绑定调控的属性必须是 `StyledProperty`，不要为了省一次字典查找把它降级成 `DirectProperty`，
  那样会丢失整条 priority frame 路径。

### 1.2 `BindingPriority` 数值越小优先级越高，且 `Unset = int.MaxValue`

```csharp
Animation     = -1
LocalValue    =  0
StyleTrigger  =  1
Template      =  2
Style         =  3
Inherited     =  4
Unset         =  int.MaxValue
```

源：`Avalonia.Base/Data/BindingPriority.cs:9-50`。

进 `ValueStore` 之前会被映射成内部的 `FramePriority`（结合 `FrameType`），
但相对顺序保持不变，`Avalonia.Base/PropertyStore/FramePriority.cs:6-35`。

**控件库结论（高频踩雷点）**

- "用户是否显式设置过此属性"不能用 `IsSet(property)` 判断。`ValueStore.IsSet`（`Avalonia.Base/PropertyStore/ValueStore.cs:352`）
  对 Style / Trigger / Template / LocalValue / Animation 任一来源都返回 true。
- 当某属性同时存在"内部 token 默认绑定"和"外部用户绑定"时，**两者必须落在不同优先级上**。
  否则后写入的会 Dispose 掉先写入的同优先级条目。
  这是 SKILL 中 Space `ItemSpacing/LineSpacing` 事故的根因。
- `LocalValue` 不能压住 `Animation`，但能压住 `Style` / `StyleTrigger` / `Template`。
  内部 token 走 `Style` 或更低优先级，给用户 `LocalValue` 留路。

### 1.3 `AffectsMeasure / AffectsArrange / AffectsRender` 注册成本只在变更时支付

`Layoutable.AffectsMeasure` / `AffectsArrange`、`Visual.AffectsRender` 是静态注册：把属性变更
订阅到对应的 `InvalidateMeasure` / `InvalidateArrange` / `InvalidateVisual` 上。
未注册的属性变更不会触发 layout / render，**注册的代价只在该属性确实变化时才支付**。

- `Avalonia.Base/Visual.cs:446-500` — `AffectsRender<T>(...)` 实现，包括对 `IAffectsRender` 的弱引用嵌套订阅。
- `Avalonia.Base/Layout/Layoutable.cs:502-512` — `AffectsMeasure<T>(...)` 等静态注册。

**控件库结论**

- 不要"为了保险"把所有自定义属性都加 `AffectsMeasure`/`AffectsArrange`/`AffectsRender`。
  只对真正会改变测量/排版/绘制结果的属性挂；纯文本展示用属性不该挂 `AffectsRender`。
- 一个属性挂上 `AffectsRender`、再让它每帧 set 一次（比如动画）= 每帧一次 `InvalidateVisual`。

### 1.4 `SetCurrentValue` ≠ `SetValue` ≠ `ClearValue`

- `SetValue(property, value)`：默认按 `LocalValue` 优先级写入，**会覆盖更低优先级的内部绑定**。
- `SetCurrentValue(property, value)`：在当前生效优先级原地写值，**不改变绑定来源**，不挤掉同/低优先级绑定。
- `ClearValue(property)`：移除 `LocalValue` 帧条目，让更低优先级（包括内部 token 默认绑定）重新生效。

源：`Avalonia.Base/AvaloniaObject.cs:333-355` / `407-422`，`Avalonia.Base/PropertyStore/ValueStore.cs:233-245`。

**控件库结论**

- 模板初始化里给值用 `SetCurrentValue`，避免无意把模板默认提升为 `LocalValue` 而压住 user binding。
- "将控件状态恢复成默认"是 `ClearValue`，不是 `SetValue(default)`。后者把"默认值"提升成了 `LocalValue`。

### 1.5 `RaisePropertyChanged` 在没有订阅者时是 null check

`AvaloniaObject.RaisePropertyChanged` 检查 `_propertyChanged` 委托是否为 null（`Avalonia.Base/AvaloniaObject.cs:761-806`）。
属性变更带的额外成本主要来自：

1. 注册的 `AffectsMeasure / AffectsArrange / AffectsRender` 订阅。
2. `Style` selector activator 监听该属性时的 `ReevaluateIsActive`。
3. `OnPropertyChanged` 虚方法。

**控件库结论**

- 单纯多注册一个 `StyledProperty<T>` 而不挂任何 affects 副作用，每次变更只额外多一个虚方法调用 + null check。
- 真正贵的是挂上 `AffectsX` + 有 selector 在监听 + 又每帧改一次。

---

## 2. 数据绑定

### 2.1 `TemplateBinding` 是专用快路径，不是常规 `Binding`

`{TemplateBinding X}` 编译为 `TemplateBindingExpression`，直接订阅 `templatedParent.PropertyChanged`，
读值即 `templatedParent.GetValue(_property)`。
`{Binding RelativeSource={RelativeSource TemplatedParent}, Path=X}` 走完整 `BindingExpression` + `ExpressionNode` 链，
每次变更要走表达式节点遍历。

- `Avalonia.Base/Data/TemplateBinding.cs:57-68`
- `Avalonia.Base/Data/TemplateBindingExpression.cs:37-43`
- `Avalonia.Base/Data/Core/BindingExpression.cs:60-134`

**控件库结论**

- ControlTheme axaml 里所有"目标控件 ↔ 模板子元素"的同 owner 同生命周期绑定，**全部用 `TemplateBinding`**。
- 不要拿 `{Binding RelativeSource=...}` 替换它，纯亏。

### 2.2 `[!]` 直接绑定与 `BindUtils.RelayBind` 的差异

- `[!Property]`（即 `AvaloniaObjectExtensions.Bind(IObservable<T>)`）走 `DirectBindingObserver<T>`，
  无 `Binding` 实例、无 `Path` 解析、单一订阅持有。
  - `Avalonia.Base/AvaloniaObjectExtensions.cs:188-204`
  - `Avalonia.Base/PropertyStore/DirectBindingObserver.cs:7-84`
- `BindUtils.RelayBind` 是 AtomUI 包装：构造一个 `Binding` + `target.Bind(...)`，比 `[!]` 多一个 `Binding` 对象和反射路径。

**控件库结论**

- source 与 target 同 owner、同生命周期 → axaml `[!]`。
- 跨生命周期、需要在 detach 时显式释放、需要 converter 链路、需要在 C# 中根据条件切换绑定 →
  `BindUtils.RelayBind` + `CompositeDisposable`。
- 这条规则在 SKILL Tier 1 §8 中是硬性边界。

### 2.3 `$parent[T]` / `LogicalAncestorElementNode` 是订阅式重算，不是缓存

`LogicalAncestorElementNode` 走 `ControlLocator.Track`，订阅 `AttachedToLogicalTree` /
`DetachedFromLogicalTree`，每次树变化都重新 `GetLogicalAncestors().ElementAtOrDefault(_ancestorLevel)`。

- `Avalonia.Base/Data/Core/ExpressionNodes/LogicalAncestorElementNode.cs:59-68`
- `Avalonia.Base/LogicalTree/ControlLocator.cs:63-68`

**控件库结论**

- 在模板里用 `$parent[atom:Popup]` 这种深层 ancestor 绑定，每次 detach/reattach 都要重新走 logical 树。
  控件深度大 + 重 attach 频繁的场景里能成为瓶颈。
- 优先用 `TemplateBinding`；不得已要跨层时，把状态推到 templated 控件本身的 `StyledProperty`，
  popup 内部再 `TemplateBinding` 引用。

### 2.4 `MultiBinding` 单次变更触发一次 converter

`MultiBindingExpression` 持有 `UntypedBindingExpressionBase[] _expressions` 和 `_values`，任一子绑定变化
触发一次 `OnChanged`，所有子值就绪后 converter 跑一次。

- `Avalonia.Base/Data/Core/MultiBindingExpression.cs:23-49`、`86-98`、`116`

**控件库结论**

- 让 converter 保持无状态、无分配。
- 多源里有一项是高频变化属性时，converter 也会被高频调用；考虑把可缓存的输入合并成单 binding。

### 2.5 编译绑定（`x:DataType`/`x:CompileBindings`）显著快于反射绑定

XAML 编译期把 binding path 转换成直接 IL 属性访问节点，不经反射。

- `Avalonia.Markup.Xaml.Loader/CompilerExtensions/XamlIlBindingPathHelper.cs:140-200`
- `Avalonia.Base/Data/CompiledBinding.cs:195-235`

**控件库结论**

- 控件库自身的 ControlTheme 里所有跨 ViewModel 的绑定都应当能走 `CompiledBinding`；
  但绝大多数 ControlTheme 内部绑定本就是 `TemplateBinding`，不会触及这条。
- 真正能受益的是 Gallery / Demo 项目里的跨 VM 绑定。

### 2.6 绑定漏 Dispose 不会泄漏 source，但订阅链仍然活着

`BindingExpression._source` 是 `WeakReference<object?>`，target 强引用 binding。
忘记 `Dispose()` 不会阻止 source 回收，但 binding 条目会跟着 target 一直活着，订阅链路依然占资源。

- `Avalonia.Base/Data/Core/BindingExpression.cs:26-90`
- `Avalonia.Base/Data/Core/UntypedBindingExpressionBase.cs:100-113`

**控件库结论**

- 在循环里 / 在 detach-reattach 路径里创建的 `Bind(...)` 一律存到 `CompositeDisposable` 并在
  `OnDetachedFromVisualTree` 释放。
- 静态 axaml 模板里一对一的 `[!]` 不需要管，绑定生命周期等同于 templated child。

---

## 3. 布局与视觉树

### 3.1 `IsVisible=false` 真正零成本

`Layoutable.MeasureCore` 在 `if (IsVisible)` 之外直接返回（实际返回 `default(Size)`），
`ArrangeCore` 同样在 `if (IsVisible)` 之外不做任何事。

- `Avalonia.Base/Layout/Layoutable.cs:546`
- `Avalonia.Base/Layout/Layoutable.cs:671`

并且 `ImmediateRenderer` 在 visual `IsVisible == false` 时直接 return（不递归 children），
`Avalonia.Base/Rendering/ImmediateRenderer.cs:34`。

**控件库结论**

- 这条是 [Theme Static Rule] 的支柱：模板里 `IsVisible="False"` + selector 切换比 C# `EnsureXxx/ClearXxx`
  在绝大多数场景下都更划算。
- 唯一例外见 SKILL "Allowed exceptions"：popup 重内容、adorner、不定数 N。

### 3.2 设置 `IsVisible` 仍然有副作用

`Layoutable.OnPropertyChanged` 在 `IsVisibleProperty` 变化时触发：

1. `Visual.UpdateIsEffectivelyVisible` 递归更新所有 children `IsEffectivelyVisible`。
2. 自身 `InvalidateMeasure` 并通知 parent `ChildDesiredSizeChanged`。
3. 把 `DesiredSize` 重置为 `default`。

`Avalonia.Base/Layout/Layoutable.cs:842-868`、`Avalonia.Base/Visual.cs:507-509`。

**控件库结论**

- 不要在每帧的 hot path（动画、滚动）里频繁切 `IsVisible`。
- 尤其不要"先设 false 再设 true"作为强制重排的手段；用 `InvalidateMeasure()` 即可。

### 3.3 `InvalidateMeasure` 不会盲目向上传播

`Layoutable.InvalidateMeasure` 只设置自身 `IsMeasureValid=false` 并入队 `LayoutManager`。
向上传播只在子节点 `DesiredSize` 实际改变时才发生（`ChildDesiredSizeChanged`）。
`LayoutManager.Measure` 中遇到 `IsMeasureValid==true` 的祖先就停。

- `Avalonia.Base/Layout/Layoutable.cs:443-459`、`480-486`
- `Avalonia.Base/Layout/LayoutManager.cs:304`

**控件库结论**

- `InvalidateMeasure` 本身廉价，主要成本是接下来的 `MeasureOverride`。控件的 `MeasureOverride` 必须保持 O(子节点数)。
- "全树 invalidate" 是写错代码（自己一路 walk 调用 `InvalidateMeasure`）造成的，框架默认不这样做。

### 3.4 `LayoutManager` 单 pass 内合并去重

`LayoutQueue.Enqueue` 用 `_loopQueueInfo` dict 去重，同一控件的多次 `InvalidateMeasure` 在同一 pass 里入队一次；
最大 pass 数 10。

- `Avalonia.Base/Layout/LayoutManager.cs:23`、`116-179`、`348-355`
- `Avalonia.Base/Layout/LayoutQueue.cs:48-68`

**控件库结论**

- 同一帧内连续设 N 个会触发 measure 的属性，最终也只跑一次 `MeasureOverride`。优化时不必为合并属性写组合 setter。
- 但 layout pass 之间的依赖（Grid 星号、容器 query 触发）可能让 pass 数从 1 涨到 4+，见下条。

### 3.5 `Grid` 星号尺寸最多 4+ 次 measure

`Grid.MeasureOverride` 含 4 组（Group 1-4）测量逻辑，并在循环依赖场景里循环最多 `c_layoutLoopMaxCount` 次。
`ResolveStar` 在组之间反复调用。

- `Avalonia.Controls/Grid.cs:234-527`、`448-514`、`490-509`

**控件库结论**

- 控件 ControlTheme 里 root 容器优先 `DockPanel` / `StackPanel` / 自定义 `FlexPanel`，避免一行 `Grid` 用了 star
  又被嵌进另一个 star 里。
- 真正需要的对齐场景再用 `Grid`。AtomUI 现有大量 `LineEdit` / `AddOnDecoratedBox` / `Cascader` 模板已经是这条规则的产物。

### 3.6 Reparent 子树触发 detach + attach + 全子树 measure 失效

`Visual.SetVisualParent` 同步触发：

1. `OnDetachedFromVisualTreeCore`（旧父）：detach compositor、`DisableTransitions`。
2. `OnAttachedToVisualTreeCore`（新父）：attach compositor、`EnableTransitions`、`UpdateIsEffectivelyVisible`。
3. `Layoutable.OnVisualParentChanged` → `LayoutHelper.InvalidateSelfAndChildrenMeasure`（递归整棵子树）。
4. `OnTemplatedParentControlThemeChanged` 重新走 ControlTheme 应用链。

- `Avalonia.Base/Visual.cs:715-738`、`551`、`745-762`、`776-791`
- `Avalonia.Base/Layout/Layoutable.cs:872-877`

**控件库结论**

- "为了节省一处 ContentPresenter，把现有控件 reparent 到不同 host"几乎一定亏：full subtree invalidation + 主题重套。
- ItemsControl 容器虚拟化场景里 reparent 不可避免，但应当走容器循环池而不是手工 `Children.Remove + Add`。

### 3.7 LogicalChildren 与 VisualChildren 是两套独立集合

LogicalChildren 用于 DataContext / style / 资源继承；VisualChildren 决定布局/渲染。
LogicalChildren 变化不直接 invalidate layout，但 `PresentationSource?.Renderer.RecalculateChildren` 会触发。

- `Avalonia.Base/StyledElement.cs:264-275`、`516-525`
- `Avalonia.Base/Visual.cs:343`

**控件库结论**

- Popup.Child、Tooltip.Content 等"逻辑子项 ≠ 视觉子项"的位置，在做 lifecycle 释放时两套都要清。
- "为了视觉位置而手动改 logical parent"几乎一定是错的。

### 3.8 `EffectiveViewportChanged` 订阅每个 layout pass 都跑

`Layoutable` 暴露 `EffectiveViewportChanged` 事件，添加首个订阅时把自己注册到 `LayoutManager._effectiveViewportChangedListeners`，
`LayoutManager.RaiseEffectiveViewportChanged` 每个 layout pass 检查所有 listener。

- `Avalonia.Base/Layout/Layoutable.cs:169-190`
- `Avalonia.Base/Layout/LayoutManager.cs:219-220`、`357-396`

**控件库结论**

- 该事件适合"少量惰性订阅 + 真的需要响应 viewport 的控件"。不要批量为每个 item 注册。
- 视图列表里如果每个 item 都订阅它，总成本是 listeners × layout-passes / s。

---

## 4. 渲染与合成

### 4.1 `InvalidateVisual` 是入队，不是同步重绘

`Visual.InvalidateVisual` 调 `PresentationSource?.Renderer.AddDirty(this)`，进 dirty 队列，下一个 render tick 才真正绘制。

- `Avalonia.Base/Visual.cs:418-421`
- `Avalonia.Base/Rendering/IRenderer.cs:30-33`

**控件库结论**

- 一帧内多次 `InvalidateVisual` 不放大成本，下一帧绘制一次。
- 但"在每个属性 setter 里 InvalidateVisual"叠加 `AffectsRender` 等价于双重失效，应只走 `AffectsRender`。

### 4.2 部分变换是 compositor-only，不需要 layout

server 端 `ServerCompositionVisual.DirtyInputs` 把 `Opacity / Offset / Scale / RotationAngle / Translation /
AnchorPoint / CenterPoint` 变化合并到 transform-dirty，不触发 UI 线程 layout。
但 `Size` / `ClipToBounds` 触发 `TriggerCompositionFieldsDirty`，会牵连 layout。

- `Avalonia.Base/Rendering/Composition/Server/ServerCompositionVisual/ServerCompositionVisual.DirtyInputs.cs:89-118`

**控件库结论**

- 入场动画首选 `Opacity` + `RenderTransform`（Translate/Scale/Rotate）。
- 不要 animate `Width / Height`，会跑 layout pass。
- AtomUI 现有的 `MotionActor` 已经是这条原则的产物。

### 4.3 `SolidColorBrush` / `Pen` 是可变对象，per-frame `new` 很贵

`SolidColorBrush` 是 `StyledProperty` 持有者；`Pen` 同样可变。它们带完整属性存储 + change notification。
有 `ImmutableSolidColorBrush`、`ImmutablePen` 等 immutable 变体可用。

- `Avalonia.Base/Media/SolidColorBrush.cs:13-95`
- `Avalonia.Base/Media/Pen.cs:17`

**控件库结论**

- 自定义 `Render(DrawingContext)` 里用到的"始终一种颜色 / 始终一种线条"画笔，必须 cache 在 static 字段或控件字段里。
- token 颜色驱动的 brush 可缓存到字段，token 变化时再重建。

### 4.4 `StreamGeometry.Parse` 每次解析路径字符串

`StreamGeometry.Parse(string)` 每次都新建 geometry 并跑 `PathMarkupParser`。

- `Avalonia.Base/Media/StreamGeometry.cs:34-44`

**控件库结论**

- Icon / Indicator / Arrow 的 path data 在 axaml 里写成 `StreamGeometry` 资源，由 Avalonia 编译期完成解析。
- 不要在 C# 控件代码里反复 `StreamGeometry.Parse(theSamePathData)`。AtomUI 的 Icon 已经走 generated metadata，
  其他自定义控件也应遵循同样模式。

### 4.5 `RenderTransform` 不触发 layout，`Width/Height` / Margin / Bounds 改变会

参见 4.2。`RenderTransform` 只挂在 visual 上，渲染时做矩阵乘法，不影响兄弟节点排版。

**控件库结论**

- 短时高频的"按下抖一下 / hover 抬一格"动画用 `RenderTransform`。
- 真的需要影响布局（panel 间距变化）才用尺寸属性。

### 4.6 `IsHitTestVisible=false` 不省渲染，`IsVisible=false` 才省

- `IsHitTestVisible=false` 仅在命中测试阶段被跳过。
- `IsVisible=false` 在 `MeasureCore` / `ArrangeCore` / `ImmediateRenderer` 三处都被短路。

来源：`Avalonia.Base/Input/InputElement.cs:64-65`、`Avalonia.Base/Visual.cs:58-59`、`Avalonia.Base/Rendering/ImmediateRenderer.cs:34`。

**控件库结论**

- "想要 disabled 时整体不参与布局/渲染" → `IsVisible="False"`。
- "只想禁交互保留可见" → `IsHitTestVisible=false` 或语义化的 `IsEnabled` + 主题。

---

## 5. 路由事件

### 5.1 路由路径每次 raise 重新构建（带池化）

`Interactive.BuildEventRoute` 每次 raise 都从 source 沿 `InteractiveParent` 走到 root，把途经的 handlers
收集进 `EventRoute._route`（`PooledList<RouteItem>`）。

- `Avalonia.Base/Interactivity/Interactive.cs:143-177`
- `Avalonia.Base/Interactivity/EventRoute.cs:13`

**控件库结论**

- 每次 `RaiseEvent` 的成本与"从 source 到 root 的 InteractiveParent 深度"+"沿途 handler 数量"线性相关。
- 高频场景（Pointer move、wheel）尽量在 source 处先判 short-circuit，再决定是否 raise。
- 不要为一次性内部信号写自定义 RoutedEvent，普通 `event Action`/`Action<T>` 就够。

### 5.2 `PointerEntered` / `PointerExited` 是 `RoutingStrategies.Direct`

- `Avalonia.Base/Input/InputElement.cs:144-147`
- `Avalonia.Base/Input/InputElement.cs:152-155`

含义：每个被指针进入/离开的 visual 都会单独收到事件，没有 tunnel/bubble。
父子嵌套的 hover 区域里，父和子都会各自触发 `PointerEntered`。

**控件库结论**

- 利用 `:pointerover` selector 而不是手挂 `PointerEntered`：selector 是 activator-based，单次属性变化触发。
- 必须在代码里响应时，要在 handler 里自己去重（比如 `e.Source != this` 等）。

### 5.3 `Handled = true` 不停止路由，只过滤未声明 `HandledEventsToo` 的 handler

`EventRoute` 在调用每个 handler 时检查 `(!e.Handled || entry.HandledEventsToo)`。

- `Avalonia.Base/Interactivity/EventRoute.cs:158-170`
- `Avalonia.Base/Interactivity/RoutedEventArgs.cs:43`

**控件库结论**

- 控件库的 class handler 想要"不管 user 已经 handled 都跑"时，加 `handledEventsToo: true`。
- 反过来，user 把事件 mark handled 不会自动让"控件内部清理 handler"也跳过。

### 5.4 Class handler vs instance handler

class handler 注册到 `RoutedEvent.Raised` 全局 observable，订阅一次，事件触发时对所有 instance 走一遍。
instance handler 存在 `Interactive._eventHandlers` 字典里，仅当前 instance 收到。

- `Avalonia.Base/Interactivity/RoutedEvent.cs:84-94`
- `Avalonia.Base/Interactivity/Interactive.cs:16`、`39-41`、`181`

**控件库结论**

- 控件库的 `OnXxx` 默认实现写成 `static OnGotFocus(args)` + `AddClassHandler<T>` 注册一次。
- 实例化每个控件再去 `AddHandler` 是浪费。

---

## 6. 样式 / Selector / ControlTheme

### 6.1 `/template/` 的匹配条件是 `TemplatedParent != null`

```csharp
private protected override SelectorMatch Evaluate(StyledElement control, IStyle? parent, bool subscribe)
{
    var templatedParent = control.TemplatedParent as StyledElement;
    if (templatedParent == null)
        return SelectorMatch.NeverThisInstance;
    return _parent.Match(templatedParent, parent, subscribe);
}
```

`Avalonia.Base/Styling/TemplateSelector.cs:39-49`。

**控件库结论（高频踩雷点）**

- 把 ControlTemplate 里的子元素改为 C# 动态创建并 `Children.Add` 时，**没有自动设置 `TemplatedParent`**。
  原本依赖 `^[Foo=true] /template/ X#bar` 的 selector 全部失配。
- 这是 SKILL "Theme Static Rule" 的根因之一：搬到 C# 之后样式 cascade 必须手工重写一次，复杂度爆炸。
- 真要在 C# 创建模板子元素，必须显式 `child.SetTemplatedParent(this)`，并在销毁时 `SetTemplatedParent(null)`。

### 6.2 Selector 是 activator-driven，订阅式重算

`PropertyEqualsSelector` 创建 `PropertyEqualsActivator`，订阅属性 observable；每次属性变化跑一次
`ReevaluateIsActive`。pseudo-class（`:pointerover` / `:pressed`）走 `StyleClassActivator` + `Classes` 集合。

- `Avalonia.Base/Styling/Selector.cs:44-68`
- `Avalonia.Base/Styling/Activators/PropertyEqualsActivator.cs:25-40`
- `Avalonia.Base/Styling/Activators/StyleClassActivator.cs:10-54`
- `Avalonia.Controls/Classes.cs:284-303`

**控件库结论**

- pseudo-class toggle 是 O(1)。控件里大胆用 `:pointerover`/`:pressed`/`:focus`/`:disabled`。
- 复杂 selector（多 attribute 等于、`:not(...)` 套 `^[X=...]`）每次相关属性变化都要重算；保持 selector 平实。

### 6.3 `ControlTheme.BasedOn` 是线性递归

```csharp
private void ApplyControlTheme(ControlTheme theme, FrameType type)
{
    if (theme.BasedOn is ControlTheme basedOn)
        ApplyControlTheme(basedOn, type);
    theme.TryAttach(this, type);
}
```

`Avalonia.Base/StyledElement.cs:776-793`。

**控件库结论**

- BasedOn 链层级保持 ≤ 3。AtomUI 内部 token 类 BasedOn 已是这样。
- "为复用一两个 setter 而扩展 BasedOn"不值得，复制更便宜。

### 6.4 Setter 在 style attach 时立即应用，模板 setter 首读时构建

- 普通 `Setter`：`Setter.Instance` → `PropertySetterInstance` 在 `StyleBase.Attach` 阶段直接 `SetValue`
  （activator 类除外，那种走 binding）。
  - `Avalonia.Base/Styling/Setter.cs:69-92`、`PropertySetterInstance.cs:45-71`
- 含 `Template` / `DataTemplate` 的 setter（`PropertySetterTemplateInstance`）：`GetValue() => _value ??= _template.Build();`
  首次读时 `Build()` 一次后缓存。
  - `Avalonia.Base/Styling/PropertySetterTemplateInstance.cs:7-31`

**控件库结论**

- 在 ControlTheme 里把"很贵的子树（svg 资源、复杂 panel）"放进 setter 的 `Template` 里，未触发的话不会构建。
- 普通 setter（颜色、margin）attach 即应用，没必要懒。

### 6.5 运行时 `Styles.Add/Remove` 是 O(N×M)

`Styles.OnCollectionChanged` 触发 `StylesAdded` / `StylesRemoved`，对作用域内每个元素跑一次 selector 匹配。

- `Avalonia.Base/Styling/Styles.cs:286-310`、`266`、`282`

**控件库结论**

- 不要在控件 hot path 里给 `Application.Styles` 或某个 host 的 `Styles` 集合做加减。这等于全局重新跑一遍样式。
- 主题切换走 `ThemeVariant` 通道，而不是替换 `Styles`。

### 6.6 `NthChild` / `NthLastChild` 在兄弟变化时重算

`NthChildActivator` 订阅 `IChildIndexProvider`，索引或总数变化触发 `ReevaluateIsActive`。

- `Avalonia.Base/Styling/Activators/NthChildActivator.cs:46-76`

**控件库结论**

- 虚拟化列表里如果 `:nth-child` 被频繁触发 reevaluate，可能成为瓶颈。
- 自定义 panel 为虚拟化列表实现 `IChildIndexProvider` 时要保证 `GetChildIndex` 是 O(1)，
  否则每次 sibling 变化都触发线性扫描。

---

## 7. 模板与 ItemsControl

### 7.1 `ApplyTemplate` 在 measure 阶段，且有缓存

`TemplatedControl.ApplyTemplate` 在 `MeasureCore` 时调用（`MeasureCore` 走完一次 styling 后 `ApplyTemplate`），
缓存在 `_appliedTemplate`，模板未变就不重建。

- `Avalonia.Controls/Primitives/TemplatedControl.cs:306-348`、`316`、`123`

**控件库结论**

- 控件的 `OnApplyTemplate` 内部不要做"扫整个 Template 子树注册一堆订阅"这种重活；只接 PART_ 名字、订阅必要事件。
- 模板真正的 instantiate 成本在 `_template.Build()` 内；大模板请走 axaml 编译。

### 7.2 `ItemsControl` 容器分两条路径：虚拟化 vs 全量

`ItemsPresenter.ApplyTemplate`：

```csharp
if (Panel is VirtualizingPanel v)
    v.Attach(ItemsControl);
else
    _generator = new ItemContainerGenerator(...);
```

- `Avalonia.Controls/Presenters/ItemsPresenter.cs:86-119`、`107-110`、`150-154`
- `Avalonia.Controls/VirtualizingPanel.cs:133`

**控件库结论**

- 列表型控件默认 `VirtualizingStackPanel`。AtomUI 的 `ListView` / `ComboBox` / `Cascader` 都是这条路径。
- 切 `ItemsPanel` = full reset：旧 panel detach、所有容器重生。一次性配置，不要做成可热切换。

### 7.3 `DataTemplate` lookup 不缓存

`DataTemplateExtensions.FindDataTemplate` 每次都从当前控件向上走 logical 树，调用每个 host 的 `Match`。
`ContentPresenter` 自己缓存最后一次 `IRecyclingDataTemplate` 用于 recycle，但跨 instance 不共享。

- `Avalonia.Controls/Templates/DataTemplateExtensions.cs:20-62`
- `Avalonia.Controls/Presenters/ContentPresenter.cs:633`、`640-650`

**控件库结论**

- 列表场景配 `IRecyclingDataTemplate` / 用统一容器类型，让 ContentPresenter 走 recycle。
- 不要用 N 个互斥 `DataTemplate` 让 lookup 每次都走完整 chain。

### 7.4 `ItemsControl.Items` 变化按事件传递，不做 diff

`ItemsControl.OnItemsViewCollectionChanged` 仅根据 `NotifyCollectionChangedAction` 更新 logical children
+ `:empty`/`:singleitem` pseudo class，**不做内容 diff**，最终决定权在 panel。

- `Avalonia.Controls/ItemsControl.cs:639-656`、`655`

**控件库结论**

- 数据源 reset 等同于全量重建。对外暴露的"刷新"功能尽量走 ObservableCollection 的细粒度事件，不要 reset。

### 7.5 `TabControl` 双 `ContentPresenter`，`Carousel` 走虚拟化 panel

- `TabControl._contentPart` + `_contentPresenter2`：用于切 tab 的过渡动画。
  状态默认不保留：`SelectedItem` 变化时 `UpdateSelectedContent` 重建。
  - `Avalonia.Controls/TabControl.cs:29-37`、`98`
- `Carousel`：`VirtualizingCarouselPanel`，items 自身就是容器；切换不重建 ContentPresenter。
  - `Avalonia.Controls/Carousel.cs:47`

**控件库结论**

- TabControl 切 tab 重建子树。tab 内有重 form / 重 list 时考虑用 `ItemContainerTheme` + 缓存。
- Carousel 切 page 不重建容器；适合"几个等价子项不停切换"。

---

## 8. Popup

### 8.1 Popup 打开成本：window-hosted vs overlay-hosted

`OverlayPopupHost.CreatePopupHost` 决定 host 类型：

- 走 `topLevel.PlatformImpl?.CreatePopup()` 时新建原生窗口（`PopupRoot`），代价大。
- 退化为 overlay 时复用 `PopupOverlayLayer`，仅做一次视觉树插入，代价小。

- `Avalonia.Controls/Primitives/Popup.cs:449`、`791-804`
- `Avalonia.Controls/Primitives/OverlayPopupHost.cs:74`、`158-160`、`164-166`

**控件库结论**

- 频繁 open/close 的 popup（autocomplete suggest、tooltip）显式选 overlay。AtomUI 现行做法。
- 第一次 open 之前重内容（候选列表、TreeView）应当 lazy 创建，参见 SKILL "Popup Lazy Content Rule"。

### 8.2 Popup.Child 跨视觉树边界，路由事件经 `InteractiveParent` 仍可冒泡

- `Popup.Child` 加进 `Popup` 的 logical children；open 时再 `popupHost.SetChild(Child)` 进入 host 视觉树。
  - `Avalonia.Controls/Primitives/Popup.cs:36-37`、`454`、`818-819`
- `PopupRoot.InteractiveParent => (Interactive?)Parent`，即逻辑父 `Popup`，因此 `RoutedEvent` 仍能冒泡到外层。
  - `Avalonia.Controls/Primitives/PopupRoot.cs:96`

**控件库结论**

- popup 内部内容可以收到外层挂的 class handler；routed event 不会因为跨 popup 而消失。
- 但 **visual ancestor**（`GetVisualAncestors`）查不过去：popup 内部找不到外层 Window。
  涉及 visual ancestry 的代码（adorner 定位、`TransformToVisual`）必须显式经 popup host。

### 8.3 Light dismiss 是 per-TopLevel overlay layer

- `Popup` 打开时令 `LightDismissOverlayLayer` 可见，由该层接管 outside-click 检测。
  - `Avalonia.Controls/Primitives/Popup.cs:542`、`557`

**控件库结论**

- Light dismiss 是局部行为；嵌套 popup 必须自己处理"内部点击不该 dismiss 父 popup"的情况。
- 这正是 [popup-click-guard](../../.claude/projects/...) 反复踩雷的点（参见 memory `feedback_popup_click_guard`）。

---

## 9. Dispatcher / 线程

### 9.1 `DispatcherPriority` 数值越大越优先

```
SystemIdle < ApplicationIdle < ContextIdle < Background < Input
< Default(0) < Loaded < UiThreadRender < AfterRender < Render < BeforeRender
< AsyncRenderTargetResize < DataBind < Normal < Send
```

源：`Avalonia.Base/Threading/DispatcherPriority.cs:25-127`。Dispatcher 队列每次取出"剩余项中数值最大的"先执行。

注意：`DispatcherPriority.Loaded`（`Default + 1`）落在 `Default` 之上、`Render` 之下；
注释明确写"after layout and render but before input"，描述的是它在一帧内的目标位置。

**控件库结论**

- 只是想"等当前 sync 工作做完再跑" → `Dispatcher.UIThread.Post(action)`（默认 `Default`）。
- "组件刚 attach 完，等 Loaded 时再跑初始化" → `DispatcherPriority.Loaded`。
- "改变后立即 sync"（带阻塞）→ `Invoke`，但要承担线程切换/重入风险。

### 9.2 `Post` ≠ "稍后立即" — 它是入队，不保证下一个 cycle 跑

`Post`/`InvokeAsync` 仅入队，O(1) 插入。等待时间取决于队列前面的更高优先级项。

- `Avalonia.Base/Threading/Dispatcher.Invoke.cs:107-111`、`257-260`、`616-620`

**控件库结论**

- 不要把 `Dispatcher.Post(...)` 当成"睡 0ms 然后跑"。它会被 Render / DataBind / Input 抢占。
- "解决时序问题"靠 `Post` 几乎一定写错；时序问题应在事件流上解决（`OnAttachedToVisualTree`、`Loaded`、
  `TemplateApplied`、`PropertyChanged`）。

### 9.3 `DispatcherTimer` 是 O(timer 总数) tick

每次 tick 都扫所有活跃 timer 求最近下一次。

- `Avalonia.Base/Threading/Dispatcher.Timers.cs:52-65`、`90`

**控件库结论**

- 短期一次性延时（防抖、消息自动关闭）建议合并到全局 message scheduler 或单一 timer，按 due time 排序。
- AtomUI `Notification` / `Message` 已按这个原则收敛。

### 9.4 `Dispatcher.Post(this.EnableTransitions)` 写法

`Animatable.EnableTransitions` 是参数无关 method group。`Dispatcher.UIThread.Post(this.EnableTransitions)` 直接传 method group，
不要包成 lambda。这条来自 SKILL `Style & Best Practice` 节的硬性规定。

---

## 10. 动画 / Transition

### 10.1 `Animatable.Transitions` 仅在 visual tree 内启用

`Animatable.EnableTransitions` 在 `OnAttachedToVisualTree` 时被框架调用；未挂入视觉树前，
即使设了 `Transitions`，集合 `CollectionChanged` 也未被订阅、值变化不会跑 transition。

- `Avalonia.Base/Animation/Animatable.cs:62-103`、`87-103`

**控件库结论**

- 在控件构造里设 `Transitions` 是可以的，但真正的"transition 生效"要等到 `OnAttachedToVisualTree` 之后。
- 模板初始化时给值，要么 detach 状态下批量改，要么显式 `DisableTransitions` / `EnableTransitions` 包住。

### 10.2 Compositor-driven 动画属性集合参见 §4.2

不再赘述。控件库一般把"显隐过渡"做成 `Opacity` + `RenderTransform`，避开 layout。

---

## 11. 决策矩阵速查

| 场景 | 推荐做法 | 反例 / 触发 SKILL 边界 |
| --- | --- | --- |
| 模板里有时显示 / 有时隐藏的子元素 | axaml 静态 + `IsVisible="False"` + selector 切换 | C# `EnsureXxx/ClearXxx` 动态创建 |
| 内部 token 默认值绑定 vs 外部用户绑定 同一属性 | 不同 `BindingPriority` 帧 | 同优先级，互相 Dispose |
| ControlTheme 内部模板 ↔ templated parent 绑定 | `{TemplateBinding X}` | `{Binding RelativeSource=...}` |
| 同 owner 同生命周期的属性同步 | axaml `[!]` | `BindUtils.RelayBind` + `CompositeDisposable` |
| 跨生命周期 / 需要 detach 时释放 | `BindUtils.RelayBind` + `CompositeDisposable` | `[!]` |
| 控件运行时状态属性（不需要 Style 控制） | `DirectProperty<T>` | `StyledProperty<T>` |
| 控件主题可调控属性 | `StyledProperty<T>` | `DirectProperty<T>` |
| 单/双向常驻 hover 视觉 | `:pointerover` selector | C# `PointerEntered/Exited` 自管 |
| 控件库公共行为（OnGotFocus 默认） | `AddClassHandler<T>` | per-instance `AddHandler` |
| 一次性内部信号 | C# `event` / `Action` | 自定义 `RoutedEvent` |
| 出现/消失动画 | `Opacity` + `RenderTransform` | 动画 `Width/Height` |
| 反复用的 SolidColorBrush | static / 字段缓存 + `Immutable*` 变体 | `new SolidColorBrush(color)` per render |
| 复杂 popup 内容 | 首次 open 时 lazy materialize；保留 | 每次 open/close 重建 |
| ControlTheme 资源继承 | BasedOn ≤ 3 层 | 5+ 层链式继承 |
| 模板里大量条件子树 | axaml `IsVisible` + selector | C# 动态构建（除 SKILL exception 外禁止） |
| 频繁 timer | 合并到单 timer + due time | 每个组件自带 DispatcherTimer |

---

## 12. 已验证条目索引

下表用于在 SKILL Cost Model 中替换 `[VERIFY]`。每条都已通过本仓库 reference 源码核实。

| 主题 | 关键路径 |
| --- | --- |
| `IsVisible=false` 短路 measure / arrange / render | `Avalonia.Base/Layout/Layoutable.cs:546`、`:671`，`Avalonia.Base/Rendering/ImmediateRenderer.cs:34` |
| `BindingPriority` 数值表 | `Avalonia.Base/Data/BindingPriority.cs:9-50` |
| `FramePriority` 与 frame 排序 | `Avalonia.Base/PropertyStore/FramePriority.cs:6-35` |
| `IsSet` 实现 | `Avalonia.Base/PropertyStore/ValueStore.cs:352`、`Avalonia.Base/AvaloniaObject.cs:298-304` |
| `SetCurrentValue` / `SetValue` / `ClearValue` 写入路径 | `Avalonia.Base/AvaloniaObject.cs:333-355`、`:407-422`，`Avalonia.Base/PropertyStore/ValueStore.cs:233-245` |
| `AffectsRender` / `AffectsMeasure` / `AffectsArrange` 静态注册 | `Avalonia.Base/Visual.cs:446-500`、`Avalonia.Base/Layout/Layoutable.cs:502-512` |
| `TemplateBinding` 直接订阅 | `Avalonia.Base/Data/TemplateBinding.cs:57-68`、`Avalonia.Base/Data/TemplateBindingExpression.cs:37-43` |
| `[!]` 直接观察者 | `Avalonia.Base/AvaloniaObjectExtensions.cs:188-204`、`Avalonia.Base/PropertyStore/DirectBindingObserver.cs:7-84` |
| `LogicalAncestorElementNode` 重算 | `Avalonia.Base/Data/Core/ExpressionNodes/LogicalAncestorElementNode.cs:59-68`、`Avalonia.Base/LogicalTree/ControlLocator.cs:63-68` |
| `LayoutManager` pass 与 `LayoutQueue` 去重 | `Avalonia.Base/Layout/LayoutManager.cs:23`、`:116-179`、`:348-355`，`Avalonia.Base/Layout/LayoutQueue.cs:48-68` |
| `InvalidateMeasure` 不向上传播 | `Avalonia.Base/Layout/Layoutable.cs:443-459`、`:480-486`，`Avalonia.Base/Layout/LayoutManager.cs:304` |
| Reparent 触发链 | `Avalonia.Base/Visual.cs:715-738`、`:551`、`:776-791`，`Avalonia.Base/Layout/Layoutable.cs:872-877` |
| `Grid` star sizing 多 pass | `Avalonia.Controls/Grid.cs:234-527` |
| `EffectiveViewportChanged` 订阅成本 | `Avalonia.Base/Layout/Layoutable.cs:169-190`、`Avalonia.Base/Layout/LayoutManager.cs:219-220`、`:357-396` |
| `InvalidateVisual` 入队 | `Avalonia.Base/Visual.cs:418-421`、`Avalonia.Base/Rendering/IRenderer.cs:30-33` |
| Compositor-only 动画字段 | `Avalonia.Base/Rendering/Composition/Server/ServerCompositionVisual/ServerCompositionVisual.DirtyInputs.cs:89-118` |
| `PointerEntered`/`PointerExited` 路由策略 | `Avalonia.Base/Input/InputElement.cs:144-147`、`:152-155` |
| Class vs instance handler | `Avalonia.Base/Interactivity/RoutedEvent.cs:84-94`、`Avalonia.Base/Interactivity/Interactive.cs:16`、`:39-41`、`:181` |
| 路由 build & `Handled` 处理 | `Avalonia.Base/Interactivity/Interactive.cs:143-177`、`Avalonia.Base/Interactivity/EventRoute.cs:158-170` |
| `/template/` 匹配规则 | `Avalonia.Base/Styling/TemplateSelector.cs:39-49` |
| Selector activator 模式 | `Avalonia.Base/Styling/Selector.cs:44-68`、`Activators/PropertyEqualsActivator.cs:25-40` |
| `ControlTheme.BasedOn` 递归 | `Avalonia.Base/StyledElement.cs:776-793` |
| Setter 应用时机 | `Avalonia.Base/Styling/Setter.cs:69-92`、`PropertySetterInstance.cs:45-71`、`PropertySetterTemplateInstance.cs:7-31` |
| `Styles.Add/Remove` 重新匹配 | `Avalonia.Base/Styling/Styles.cs:286-310` |
| `NthChild` 重算 | `Avalonia.Base/Styling/Activators/NthChildActivator.cs:46-76` |
| `ApplyTemplate` 缓存 | `Avalonia.Controls/Primitives/TemplatedControl.cs:306-348` |
| `ItemsPresenter` 虚拟化分支 | `Avalonia.Controls/Presenters/ItemsPresenter.cs:86-119`、`:107-110`、`:150-154` |
| `DataTemplate` lookup 不缓存 | `Avalonia.Controls/Templates/DataTemplateExtensions.cs:20-62`、`Avalonia.Controls/Presenters/ContentPresenter.cs:633`、`:640-650` |
| `ItemsControl` 集合事件 | `Avalonia.Controls/ItemsControl.cs:639-656` |
| `TabControl` 双 ContentPresenter | `Avalonia.Controls/TabControl.cs:29-37`、`:98` |
| Popup host 选择 | `Avalonia.Controls/Primitives/Popup.cs:449`、`:791-804`，`OverlayPopupHost.cs:74`、`:158-160`、`:164-166` |
| Popup `InteractiveParent` | `Avalonia.Controls/Primitives/PopupRoot.cs:96` |
| Light dismiss layer | `Avalonia.Controls/Primitives/Popup.cs:542`、`:557` |
| `DispatcherPriority` 排序 | `Avalonia.Base/Threading/DispatcherPriority.cs:25-127` |
| `Post`/`InvokeAsync`/`Invoke` 区别 | `Avalonia.Base/Threading/Dispatcher.Invoke.cs:107-111`、`:257-260`、`:616-620` |
| `DispatcherTimer` O(N) tick | `Avalonia.Base/Threading/Dispatcher.Timers.cs:52-65`、`:90` |
| `Animatable.Transitions` 启用条件 | `Avalonia.Base/Animation/Animatable.cs:62-103` |

---

## 13. 仍待补全（PARTIAL）

下列条目在本轮源码核对中未完整验证，后续再开 PR 补：

- `Initialized` / `AttachedToVisualTree` / `Loaded` 的精确触发顺序与 dispatcher priority 关系。
- 编译 XAML 对 `ControlTemplate` 与普通 `Style` 的具体 IL 生成差异。
- `DynamicResource` 在 theme variant 切换时的精确订阅链路（出于 markup 与 styling 的边界）。
- `RenderOptions.BitmapInterpolationMode` 在 Skia 后端实际开销曲线（需要 bench）。
- `ItemsRepeater`（如启用）与 `VirtualizingStackPanel` 的虚拟化差异。

补全时请追加到本节，并在被引用条目中去掉 PARTIAL 标记。
