# DataGrid 跨实例 RowDetails 死锁分析

## 现象

在 Gallery 的 DataGrid ShowCase 中：

1. 先点击的那个 DataGrid 的 RowDetails 展开收起，**功能完全正常**；
2. 紧接着点击 **另一个 DataGrid 实例** 的 RowDetails 展开按钮时，**UI 直接死锁**；
3. 顺序无关：先点哪个就哪个正常，第二个必死；
4. 在第一个 DataGrid 折叠后到第二次点击之间，**应用没有死**——`UI heartbeat` 仍然每秒 tick；
5. 死锁发生在第二个 grid 的 `PointerPressed` 处理路径里，日志在 `RowExpander class Tunnel PointerPressed` 这行后突然停止。

两个 DataGrid 在业务上没有任何直接耦合，只是绑定到同一个 `ExpandableRowDataSource`。

## 必须用「延迟触发」解释，不能用「持续布局风暴」解释

之前一种解释是：第一个 grid 折叠后留下了一个持续的 layout 风暴，第二次点击只是把已经接近饱和的 UI 线程压垮。这个解释 **与观测不符**：

- 如果 UI 线程已经被持续 layout 风暴占满，应用应该在第一次折叠后立刻死，不应该等到第二次点击；
- 实际上第一次折叠后每秒还有 heartbeat，输入路由也没问题；
- 死锁开始的时刻 **精确对齐第二个 grid 的 PointerPressed**，不是某个 timer/idle tick。

正确的描述应该是：第一个 grid 折叠后留下了一个 **条件性反馈环**——它需要再发生一次 layout pass 或一次 input pass 才会闭合。第二次点击恰好是触发这个 pass 的事件。

这是分析这个 bug 的基本前提：**根因一定是「条件性的延迟触发」，不是「立刻饱和」。**

## 之前几个修复为什么不解决问题

| 之前的修复 | 落点 | 为什么不解决 |
|---|---|---|
| `DataGridRowExpander.NotifyUnLoadingRow` 里 dispose `RelayBind` | row expander 卸载路径 | row 没有真正卸载（只是折叠），这条路径根本没走 |
| 折叠时 dispose `_detailsContent` 的 `LayoutUpdated` 订阅 | row 内部生命周期 | `LayoutUpdated` 是 `Layoutable` 的全局事件——只要某个 layout pass 跑了，所有订阅都会被通知；取消订阅只是让这个 row 看不到，不会让事件本身停下来 |
| 折叠时把 `_detailsContent` 从 `_detailsElement.Children` 物理移除 | row 内部 visual tree | `LayoutUpdated` 不依赖 visual tree 关系；移除子节点只是把症状的可见点挪走 |
| 把 RowsPresenter / 空指示器 / 拖拽指示器的 `Grid.RowSpan` 从 2 改成 1 | DataGrid 模板 | 是水平滚动条另一个独立 bug，与 RowDetails 无关 |
| `ScrollGestureRecognizer.IsScrollInertiaEnabled` 用 `TemplateBinding` 替代 `ReflectionBinding` | DataGrid 模板 | 是 Avalonia 12 的 binding 系统改动，与 RowDetails 无关 |

**共性问题：所有修复都假设根因在单个 row 内部的生命周期里**。但题目本身就告诉我们：根因不在 row 内部，否则两个完全独立的 DataGrid 不应该相互影响。修复必须解释「为什么 grid A 的状态会影响 grid B」。

## 当前最强嫌疑（按概率排序）

### 嫌疑 1：`AvaloniaPropertyReflectionExtensions.InvokeNotifying` —— 反射调用 Avalonia 静态/internal API

代码位置：

- `src/AtomUI.Core/Utils/AvaloniaPropertyReflectionExtensions.cs`
- `src/AtomUI.Desktop.Controls.DataGrid/DataGrid.Privates.cs:1564` —— `DataContextProperty.InvokeNotifying(cellContent, arg2);`
- `src/AtomUI.Desktop.Controls.DataGrid/DataGrid.cs:1339,1347` —— 在 `OnDataContextBeginUpdate / OnDataContextEndUpdate` 里调用

`InvokeNotifying` 通过反射拿到 `AvaloniaProperty.Notifying` 这个 **AvaloniaProperty 级别（即静态/全局）** 的委托，然后在指定 target 上手动触发。

为什么是跨实例耦合的可疑点：

- `AvaloniaProperty.Notifying` 是按 `AvaloniaProperty` 维度共享的——所有订阅它的对象都在一个委托链里；
- 两个 DataGrid 都会调用 `DataContextProperty.InvokeNotifying`，调用同一个委托链；
- Avalonia 12 调整过 `Notifying` 在 `ItemsControl` 内部的使用方式（DataContext 流动 / recycling 相关）；如果它对调用顺序、reentrancy 或者已 detach 的对象有新的假设，AtomUI 这种「手动反射触发」的用法很可能踩到这些假设；
- 第一个 grid 的 row 在折叠后没有真正卸载，但其 cell content 还活着，仍然挂在 `Notifying` 委托链或受其影响；当第二个 grid 的 row 因为输入触发再次走到 `OnDataContextBeginUpdate` 时，反射触发会把 **第一个 grid 留下的状态** 一起带进来，从而出现跨实例。

要确认这条路径，最直接的做法是把 `InvokeNotifying` 调用临时屏蔽掉（先注释 `DataGrid.cs:1339,1347` 这两处），看死锁是否消失。如果消失，说明就是这条反射路径的问题，需要去掉这种反射用法或者按 Avalonia 12 重新实现 DataContext 的临时屏蔽逻辑。

### 嫌疑 2：Avalonia 12 的 `LayoutUpdated` / transition 行为变更

观测：

- 第一个 grid 折叠后，日志中 `Row Details LayoutUpdated` 仍以每秒几十次的频率持续触发；
- 同时 `Grid UpdateDisplayedRows` 的结果在 `deltaY=274 / 220` 之间出现过振荡，最终稳定在 220；
- `Row NotifyHeightChanged` 看到 `new=22, previous=22, desired=22, visible=False`——内层守卫 `IsDetailsVisible && _appliedDetailsTemplate != null` 让它什么都不做，所以 row 自身不是把 layout 推回去的人。

如果 row 自己不再触发 invalidation，那么 `LayoutUpdated` 持续触发只能解释为：**有别的东西在持续推动 layout pass**。最可能的来源：

- `DataGridRowTheme.axaml` 上 `Background` 的 `SolidColorBrushTransition`；
- `DataGridRowExpanderTheme.axaml` 上 `Foreground` 的 `SolidColorBrushTransition`；
- `DataGridRowExpander.OnLoaded` 里 `Dispatcher.Post(EnableTransitions)` 启用的 transitions。

Avalonia 12 调整了哪些属性归类为 `AffectsMeasure` / `AffectsArrange` / `AffectsRender`，以及「不可见控件停止动画」的判定逻辑。如果某个原本只影响 render 的属性在 12 里被错归到 `AffectsMeasure`（或者反过来），就会让一个本应只在 render 层 tick 的 transition 持续触发 layout invalidation，从而解释为什么 `LayoutUpdated` 在折叠后还会持续触发。

要确认这条路径，可以临时把 `DataGridRowTheme.axaml` 里 `Background` 的 transition、以及 `DataGridRowExpanderTheme.axaml` 里 `Foreground` 的 transition 全部去掉，再复现。如果死锁消失或 `LayoutUpdated` 风暴消失，问题就在 transition 与 layout 触发链上。

### 嫌疑 3：`Dispatcher.Post(EnableTransitions)` 在 Avalonia 12 多 dispatcher 模型下的语义

代码位置：`src/AtomUI.Desktop.Controls.DataGrid/Column/DataGridRowExpander.cs:137`

```csharp
protected override void OnLoaded(RoutedEventArgs e)
{
    base.OnLoaded(e);
    Dispatcher.Post(this.EnableTransitions);
}
```

Avalonia 12 引入了多 dispatcher（每线程一个）。`Dispatcher.Post` 不带显式 dispatcher 时使用 **当前线程的 dispatcher**。如果 row expander 的 `OnLoaded` 在某些 recycling 路径下不是 UI 线程跑的，或者它在 popup / overlay 子树里被构造，post 出去的 `EnableTransitions` 可能落到「不正确」的 dispatcher 上。

这个嫌疑比 1、2 弱，但要排查的话，把 `Dispatcher.Post` 换成在 UI dispatcher 上显式调度（例如直接同步 `EnableTransitions()`，或者使用 owning grid 自己的 `Dispatcher`）很容易验证。

### 嫌疑 4：共享 `ItemsSource` 引发的状态串扰

`ExpandableDataGrid`、`OrderSpecificColumnDataGrid`、`RowAndColumnHeaderDataGrid` 共享同一个 `ExpandableRowDataSource` 集合，连 item 实例也是同一个。`DataGridDataConnection` 本身没有按 item 订阅 `INotifyPropertyChanged`，且每个 grid 各有自己的 `_showDetailsTable`。所以这条路径直接耦合的可能性不大，但仍然不能完全排除：

- Avalonia 12 的 binding 系统对共享 source 的处理路径变了；
- compiled bindings 默认开启后，对同一 item 的 binding 在两个 grid 间是否复用了某些 plan/cache；
- `DataContext` 通过 logical tree 的 inherit 机制在 row 之间是否产生了非预期的复用。

排查代价低：让三个 DataGrid 各自绑定 `ObservableCollection` 的独立深拷贝即可。如果死锁消失，根因就在共享 ItemsSource 上；如果仍然死锁，这条嫌疑就排除掉。

### 嫌疑 5：`DataGridDetailsPresenter.IsVisible` 与 `ContentHeight` 的语义

`DataGridDetailsPresenter` 自定义了 `MeasureOverride`，返回 `ContentHeight` 作为 `desiredHeight`，但子节点仍以 `PositiveInfinity` 测量自身，子节点 `DesiredSize.Height` 始终是 22。Avalonia 11 里这种「父说我高 0，但子还是测出 22」的组合可以正常工作；Avalonia 12 不可见控件不再 tick 动画的改动是否对这种模式产生副作用，需要单独验证。

## 优先级和验证顺序

1. **先做嫌疑 4 的实验**（成本最低）：把三个 DataGrid 改成各自独立的数据集合。
   - 死锁消失 → 进入嫌疑 1 / 4 的深查；
   - 死锁仍在 → 排除嫌疑 4，跳到第 2 步。
2. **再做嫌疑 1 的实验**：临时把 `DataGrid.cs:1339, 1347` 两处 `NotifyDataContextPropertyForAllRowCells` 调用注释掉。
   - 死锁消失 → 锁定 `InvokeNotifying` 反射路径，按 Avalonia 12 重写或移除；
   - 死锁仍在 → 进入嫌疑 2。
3. **再做嫌疑 2 的实验**：把 `DataGridRowTheme.axaml`、`DataGridRowExpanderTheme.axaml` 上的 transitions 临时移除。
   - `LayoutUpdated` 风暴消失或死锁消失 → 是 transition 与 layout 触发链的问题，定位到具体属性的 `AffectsMeasure` 归类；
   - 死锁仍在 → 进入嫌疑 3。
4. **嫌疑 3 实验**：把 `Dispatcher.Post(this.EnableTransitions)` 换成同步直接调用，或显式指定 owning grid 的 `Dispatcher`。

每一步实验都是**单变量、可在分钟级验证**的。这是这次调试的关键纪律——之前迭代成本太高，主要是因为每次都改 row 内部生命周期，建一遍工程，又没办法立刻区分假设。

## 不要做什么

- 不要再改 `DataGridRow.Privates.cs` 里 RowDetails 的生命周期，它已经和这个 bug 没关系；
- 不要再加更多的 `[DGDiag]` 路由诊断；要加诊断就只加在嫌疑 1 / 嫌疑 2 的具体触发点；
- 不要在没排除嫌疑 1-3 之前去查共享 `ItemsSource` 之外的「业务逻辑耦合」——两个 DataGrid 没有业务耦合，根因一定在 AtomUI / Avalonia 12 框架层。

## 当前结论

最强嫌疑是 `AvaloniaPropertyReflectionExtensions.InvokeNotifying` 的反射路径在 Avalonia 12 下踩到了静态 / internal 委托语义变化，把第一个 grid 的残留状态带到了第二个 grid 的输入处理流程里。次强是 transitions / layout 触发链在 Avalonia 12 的归类变化导致折叠后的持续 `LayoutUpdated`。

下一步应当严格按上面 1→2→3→4 的顺序做单变量实验，不要再做行内修复。

---

## 最终诊断（2026-05-07）

### 五个嫌疑全部证伪

按文档给出的实验顺序逐个回滚验证，**全部证伪**：

1. ❌ 共享 `ItemsSource` / item 实例 → 改成独立深拷贝集合，仍死锁
2. ❌ `DataGrid.NotifyDataContextPropertyForAllRowCells` 触发 `AvaloniaProperty.Notifying` 静态委托 → 注释掉调用，仍死锁
3. ❌ `DataGridRow` / `DataGridRowExpander` 的 `Background`/`Foreground` Transitions 在 attach 时跑动画 → 全部去掉，仍死锁
4. ❌ `DataGridRowExpander.OnLoaded` 用 `Dispatcher.Post(EnableTransitions)` 异步起转场 → 改同步直调，仍死锁

之前几轮 GPT 在 `DataGridRow.Privates.cs` 的 `Attach/DetachDetailsContent` 物理 attach/detach 改动也并不在根因路径上。

### 改用 dotnet-stack 抓主线程栈

抓到的栈是 **CPU_TIME 而非阻塞**，调用链：

```
DataGrid.HandleLostFocus          ← 主线程在这里 spin
← Interactive.RaiseEvent (LostFocus)
← KeyboardDevice.SetFocusedElement
← FocusManager.Focus
← FocusManager.OnPreviewPointerEventHandler
← MouseDevice.MouseDown
```

也就是说，第二次点击 grid B 的 `MouseDown` 同步派发到 `FocusManager`，给原焦点元素发 `LostFocus` 事件，`DataGrid.HandleLostFocus` 在那里**死循环烧 CPU**。

### 真正根因：`HandleLostFocus` 的祖先 walk 没有环检测

`DataGrid.Privates.cs` 的 `HandleLostFocus`（约 501-571 行）实现里，需要判断「焦点是否离开了本 DataGrid」：

```csharp
var focusedObject = FocusManager.GetFocusedElement() as Visual;
while (focusedObject != null)
{
    if (focusedObject == this)
    {
        focusLeftDataGrid = false;
        break;
    }
    // 升一层：先 logical parent，再 fallback 到 visual parent
    focusedObject = focusedObject.Parent as Visual ?? focusedObject.GetVisualParent();
}
```

`FocusManager.GetFocusedElement()` 拿到的是**新焦点元素**（已经在 grid B 里）。从那里向上 walk：
- 撞到 grid A（`this`）→ break，正常
- 撞不到 → 一直走，直到 `Parent` 和 `GetVisualParent()` 都为 `null`

**问题**：在 Avalonia 12 下，walk 走到顶端的窗口（`WorkspaceWindow`）时，`Parent` / `GetVisualParent()` 链居然形成了一个**回到 Window 自身的循环**。Window 在 Avalonia 11 里是树根（两个父引用都返回 null），但 Avalonia 12 重做了窗口装饰系统（`WindowDrawnDecorations` / Overlay / Adorner 层），新引入的层级关系把 Window 本身嵌进了一条循环边。

### 跨实例耦合机制（焦点系统是全局通道）

为什么两个完全独立的 DataGrid 会互相影响？因为 **`FocusManager` 是窗口/应用级单例**：

| 场景 | 行为 |
|---|---|
| 第一次点 grid A | 焦点已经在 grid A 内或刚跳进 grid A。walk 第一步就 `focusedObject == this`，立即 break，**永远走不到 Window** |
| 折叠后 idle | 焦点还停在 grid A 内。`HandleLostFocus` 不会被再次触发。`UI heartbeat` 正常 tick |
| 点 grid B | `FocusManager` 把焦点转给 grid B 的元素，**给 grid A 派 `LostFocus`**。grid A 的 walk 从「grid B 里的新焦点元素」开始升，撞不到 grid A，一路升到 `WorkspaceWindow`，**撞进环 → CPU 烧死** |

这完美解释了三件诡异事：
- 两个独立 grid 为什么会互相影响（焦点系统是全局通道）
- 为什么先点的功能正常（walk 第一步就 break）
- 为什么 heartbeat 还在 tick 但点第二个就挂（CPU spin 发生在 `MouseDown` 同步派发栈里，spin 之前 dispatcher 还能 tick，spin 一旦开始 input 就再无机会派发）

### 修复

`DataGrid.Privates.cs:HandleLostFocus` 的 ancestor walk 加 visited-set 守卫：

```csharp
HashSet<Visual>? visitedAncestors = null;
while (focusedObject != null)
{
    if (focusedObject == this)
    {
        focusLeftDataGrid = false;
        break;
    }
    visitedAncestors ??= new HashSet<Visual>(ReferenceEqualityComparer.Instance);
    if (!visitedAncestors.Add(focusedObject))
    {
        // 撞环，按「焦点确实离开了本 grid」处理（focusLeftDataGrid 保持 true）
        break;
    }
    focusedObject = focusedObject.Parent as Visual ?? focusedObject.GetVisualParent();
}
```

按引用对等比较（`ReferenceEqualityComparer.Instance`），惰性创建 HashSet。撞环时 break，等同于 walk 走到根，与 Avalonia 11 行为兼容。

### 实测验证

打开 `Console.WriteLine` 诊断后真实抓到的环报告：

```
[DataGrid.HandleLostFocus] Ancestor cycle detected.
  CycleNode = AtomUIGallery.Workspace.Views.WorkspaceWindow
  VisitedChain = DataGridRowExpander -> DataGridCell -> DataGridCellsPresenter
                 -> DataGridFrozenGrid -> Border -> DataGridRow -> DataGridRowsPresenter
                 -> Grid -> DockPanel -> Spin -> Border -> DataGrid -> ...
```

环节点确认是 `WorkspaceWindow`，链路从 grid B 的 `DataGridRowExpander` 经过 grid B 的内部容器，再向上经过页面导航/`Spin`/`DataGrid` 容器到达 Window，然后从 Window 通过某个 Avalonia 12 装饰层引用绕回自身。

### 后续可深挖（非阻塞）

修复已生产可用。但环本身是 Avalonia 12 的窗口装饰系统留下的一条 logical/visual parent 引用，建议进一步定位：

- 在窗口的 attached `WindowDrawnDecorations` 模板里检查 Overlay/Adorner/FullscreenPopover 的 logical parent 设置
- 以 `WorkspaceWindow` 为根，`Parent` / `GetVisualParent()` 各 walk 一遍，找出哪条边回到 Window 自身
- 若是 AtomUI 自定义的 `WorkspaceWindow` 模板/样式造成，修模板；若是 Avalonia 12 框架本身的引用回环，向 Avalonia 上游报 issue

### 关键教训

- **焦点系统是窗口级全局通道**，任何「在焦点变化时遍历祖先链」的代码都必须有环检测，不能假定 visual/logical 树永远是 DAG
- 之前 GPT 死磕 row/details/transitions 的内部生命周期是错的方向，因为根因在 DataGrid 主类的焦点处理路径，与 row 生命周期无关
- **CPU_TIME 死循环 ≠ 死锁**。表象都是 UI 卡死，但前者在 `dotnet-stack` 下显示 `CPU_TIME` 标记，后者会显示阻塞调用栈（`WaitOne` / `Monitor.Wait` 等）。区分这两种是排查方向的关键

---

## 最终修复（对齐上游 Avalonia）

补充对照 `.referenceprojects/Avalonia.Controls.DataGrid` 后发现：上游 Avalonia 的 `DataGrid_LostFocus` 早已不是手写 walk，而是一行 API：

```csharp
private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
{
    _focusedObject = null;
    if (ContainsFocus)
    {
        var focusedObject = TopLevel.GetTopLevel(this)?.FocusManager.GetFocusedElement() as Visual;
        var focusLeftDataGrid = !this.IsVisualAncestorOf(focusedObject);
        ...
    }
}
```

`Visual.IsVisualAncestorOf(...)` 是 Avalonia 内建的 visual tree 检查，**纯 visual 路径，不会走 logical Parent，不会撞环**。

AtomUI 这段是从 WPF/Silverlight DataGrid 移植过来的老代码，保留了「先走 logical Parent，再 fallback 到 visual Parent」的 walk + `dataGridWillReceiveRoutedEvent` 标志位双逻辑。在 Avalonia 12 重做窗口装饰系统后，logical Parent 链上多出回到 `WorkspaceWindow` 自身的循环边，这段老 walk 立刻翻车。

### 最终修复方案：直接对齐上游

废弃 visited-set 补丁，重写 `HandleLostFocus` 为上游写法：
- 去掉手写 while-loop
- 去掉 `dataGridWillReceiveRoutedEvent` 标志位
- 第二个分支条件由 `!dataGridWillReceiveRoutedEvent` 改为 `focusLeftDataGrid`，与上游一致

### 行为差异说明

理论上两种写法在「焦点元素位于 grid 的 logical 子树（如 popup 内 editor）但不在 visual 子树」时行为不同：
- 旧 AtomUI：walk 通过 logical Parent 找到 `this`，`focusLeftDataGrid = false` + `dataGridWillReceiveRoutedEvent = false`，挂外部 LostFocus 监听
- 新（对齐上游）：`IsVisualAncestorOf` 返回 false，若是 template 列走 `else if focusLeftDataGrid` 挂监听；若是非 template 列直接 `CommitEdit + ResetFocusedRow`

实际上 AtomUI 的 popup 编辑（FilterColumn / 自定义编辑器）都用 `DataGridTemplateColumn`，差异分支不会触发。实测正常。

### 教训

- **跨实例「神秘耦合」八成是经过框架级单例**（FocusManager / InputManager / Renderer / ToolTipService），不要急着怀疑业务层共享状态
- **`dotnet-stack` 抓 CPU_TIME 是定位 spin loop 的首选工具**，比 dotnet-dump 快、信息够用
- **从老平台移植代码时务必对照上游当前实现**，框架已经简化掉的逻辑往往是踩坑后被淘汰的
- **任何「在祖先链上 walk」的代码都该有环检测，或更好——直接用框架 API**
