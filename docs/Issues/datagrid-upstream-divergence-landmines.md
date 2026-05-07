# AtomUI DataGrid vs 上游 Avalonia DataGrid — 隐藏地雷扫描

## 背景

AtomUI 的 DataGrid（`src/AtomUI.Desktop.Controls.DataGrid/`）从 Avalonia 上游 DataGrid（`.referenceprojects/Avalonia.Controls.DataGrid/src/Avalonia.Controls.DataGrid/`）派生并大幅扩展。代码量对比：

| 项目 | 文件数 | 总行数 |
|---|---|---|
| 上游 Avalonia DataGrid | 56 | 29,897 |
| AtomUI DataGrid | 122 | 36,324 |

由于体量差异和 Avalonia 11 → 12 的破坏性变更，AtomUI 中保留了一些**从 WPF/Silverlight 移植过来、但上游已经用框架 API 简化掉的老代码**。这些代码在 Avalonia 11 时代能正常工作，但在 Avalonia 12（特别是窗口装饰系统重做后）可能形成隐藏的运行时陷阱。

本扫描的契机是 [datagrid-cross-instance-rowdetails-hang.md](datagrid-cross-instance-rowdetails-hang.md) 那个跨实例死锁 bug：`HandleLostFocus` 用手写 `while` 循环上溯 `Parent ?? GetVisualParent()`，撞到 Avalonia 12 下 `WorkspaceWindow` 自身的循环边导致 CPU spin。该 bug 已通过对齐上游 Avalonia 的 `IsVisualAncestorOf` 修复。

本文档记录系统性扫描中发现的**同型隐藏地雷**——即「沿用老移植代码、上游已经用框架 API 简化」的地方。

## 扫描方法

派出 explore agent 对照两份代码，按以下 10 个 pattern 扫描：

1. 手写树遍历 / 祖先链 walk（`while (X != null) X = X.Parent ?? X.GetVisualParent()`）
2. 手动焦点管理（直接操作 `FocusManager` 内部状态、手写「找下一个 focusable」）
3. 手动 `InvalidateVisual` / `InvalidateMeasure` 风暴
4. 手写 `PointerPressed/Released` hit-testing 中的祖先查找
5. 手动 DataContext / inheritance 传播
6. 绕过 Avalonia 内置事件转发
7. 过期的 reflection / internal 访问
8. 手写 `Dispatcher.Post` / `Dispatcher.UIThread.InvokeAsync` 来「打散执行」
9. 手写 `if (_inSomething) return;` 重入守卫
10. 直接 Equality 比较 Visual 引用做祖先判断

排除项：AtomUI 新增功能（FilterColumn / TransferSupports / Localization / Themes）、命名差异、style/AXAML 差异。

## 扫描结果

agent 报告 10 处疑点。经手工复核，筛除噪音后分为以下三类：

---

## 一、🔴 CRITICAL — 必须修复（与已知 bug 完全同型）

### A1. `HandleGotFocus` 的 visual tree walk（`DataGrid.Privates.cs:487-498`）

**当前代码：**
```csharp
Visual? focusedElement = e.Source as Visual;
_focusedObject = focusedElement;
DataGridRow? focusedRow;
while (focusedElement != null)
{
    focusedRow = focusedElement as DataGridRow;
    if (focusedRow != null && focusedRow.OwningGrid == this && _focusedRow != focusedRow)
    {
        ResetFocusedRow();
        _focusedRow = focusedRow.IsVisible ? focusedRow : null;
        break;
    }
    focusedElement = focusedElement.GetVisualParent();
}
```

**风险：**
- 与 `HandleLostFocus` 完全同型的隐藏地雷
- 当焦点元素不是任何 `DataGridRow` 的后代（落在 popup / column header / 装饰层 / overlay 时），`focusedRow` 永远为 `null`，循环走 `GetVisualParent()` 一路上溯
- 在 Avalonia 12 重做窗口装饰系统后，`GetVisualParent()` 链可能形成回到 Window 自身的循环边 → CPU spin
- 之所以还没爆，是因为触发条件比 `HandleLostFocus` 更刁钻：需要焦点直接落在非 row 后代的元素上

**建议修复：** 改用 `FindAncestorOfType<DataGridRow>()` 一行替代，既消除 walk 也避免循环：
```csharp
var focusedElement = e.Source as Visual;
_focusedObject = focusedElement;
var focusedRow = focusedElement?.FindAncestorOfType<DataGridRow>(includeSelf: true);
if (focusedRow != null && focusedRow.OwningGrid == this && _focusedRow != focusedRow)
{
    ResetFocusedRow();
    _focusedRow = focusedRow.IsVisible ? focusedRow : null;
}
```

注意：上游 Avalonia 在该位置也保留了 while-loop 写法（行 3943-3953 完全相同）。**这是上游也有的潜在地雷**，但 AtomUI 的窗口装饰是自定义的（`WorkspaceWindow`），更容易触发。建议主动修复。

---

### A2. `TreeHelper.ContainsChild`（`Utils/TreeHelper.cs:21-47`）

**当前代码：**
```csharp
internal static bool ContainsChild(this Visual? element, Visual? child)
{
    if (element != null)
    {
        while (child != null)
        {
            if (child == element)
            {
                return true;
            }

            Visual? parent = child.GetVisualParent();
            if (parent == null)
            {
                if (child is Control childElement)
                {
                    parent = childElement.GetVisualParent();   // ← 同一对象重复调用
                }
            }
            child = parent;
        }
    }
    return false;
}
```

**风险：**
- 与 `HandleLostFocus` 完全同型的手写 walk
- 双重 `GetVisualParent()` 调用是 WPF 时代的 popup 特殊性 workaround，在 Avalonia 12 下既无意义又增加成环概率
- 上游 Avalonia 没有这个 helper，所有 caller 都直接用 `IsVisualAncestorOf`
- 缺乏环检测，撞循环即 spin

**建议修复：** 直接删除整个 `TreeHelper.ContainsChild`，所有 caller 改用 Avalonia 内建的 `Visual.IsVisualAncestorOf(...)`。

```csharp
// 旧用法
if (someElement.ContainsChild(target)) { ... }

// 新用法
if (someElement.IsVisualAncestorOf(target)) { ... }
```

---

## 二、🟡 HIGH — 建议修复（同型但触发面窄）

### B1. `DataGridColumn.GetColumnContainingElement`（`Column/DataGridColumn.cs:650-664`）

**当前代码：**
```csharp
public static DataGridColumn? GetColumnContainingElement(Visual element)
{
    Visual? parent = element;
    while (parent != null)
    {
        if (parent is DataGridCell cell)
        {
            return cell.OwningColumn;
        }
        if (parent is DataGridColumnHeader columnHeader)
        {
            return columnHeader.OwningColumn;
        }
        parent = parent.GetVisualParent();
    }
    return null;
}
```

**风险：**
- 在指针事件处理中调用此方法，e.Source 可能落在 popup / overlay 中
- 同样的 visual tree walk 模式，缺乏环检测
- 在 Avalonia 12 下，如果调用时 source 是 popup 内元素，walk 上溯到 Window 撞循环 → spin

**建议修复：**
```csharp
public static DataGridColumn? GetColumnContainingElement(Visual element)
{
    return element.FindAncestorOfType<DataGridCell>(includeSelf: true)?.OwningColumn
           ?? element.FindAncestorOfType<DataGridColumnHeader>(includeSelf: true)?.OwningColumn;
}
```

`FindAncestorOfType<T>` 是 Avalonia 内建的扩展方法，纯 visual tree、有内部环防御。

---

## 三、🟢 已审视 — 不需要改动

以下项 explore agent 也标记了，但经过手工复核**不属于真正的地雷**，不应该改：

### `NotifyDataContextPropertyForAllRowCells` / `InvokeNotifying`（HIGH 标记）

**位置：** `DataGrid.Privates.cs:1525-1538`、`DataGrid.cs:1339, 1347`

**为什么不动：**
- 在跨实例死锁的实验 1 中已证伪它是元凶（注释掉调用、问题仍在）
- AtomUI 这样写有特定原因：`DataGridCell.Content` 是手动 attach 到 cell 的（不是通过 ItemsControl 容器机制），DataContext 自动继承管道在这条路径上不工作，必须手动通知
- 这是 AtomUI 自有的 cell content 模型决定的，不是老代码残留
- 上游 Avalonia 没有此方法是因为它的 cell 模型不同

**建议：** 保留现状，但在方法上方加一段注释解释为什么需要手动通知。

### `WaitForLostFocus` / `_executingLostFocusActions`（HIGH 标记）

**位置：** `DataGrid.Privates.cs:1211-1229`、`HandleEditingElementLostFocus`

**为什么不动：**
- 上游 Avalonia DataGrid 也有同名同实现的机制（`DataGrid.cs:3210` 的 `WaitForLostFocus`）
- 不是 AtomUI 独有的老代码，是 Avalonia DataGrid 标准模式
- 编辑模式提交需要等待 LostFocus 是合理设计，不是 hack

**建议：** 不动。

### `Dispatcher.Post(EnableTransitions)` / `Dispatcher.Post(ProcessSort)` 等（MEDIUM 标记）

**位置：** `Column/DataGridColumnHeader.Sorting.cs:184`、`DataGridColumnHeader.cs:955`、`DataGridRowExpander.cs:137`、`Column/Filter.cs:40` 等

**为什么不动：**
- 跨实例死锁实验 3 已证伪 `Dispatcher.Post(EnableTransitions)` 是问题
- transitions 必须延迟到 attach + 首次 layout 完成后才能启用，否则初始值会被当成动画起点
- `ProcessSort` 在 `CommitEdit` 后异步执行是为了让数据状态先稳定
- 这些都是有意图的延迟，不是「隐藏 bug」

**建议：** 保留。

### `SetFrozenForColumnGroupItem` 的 while 循环（MEDIUM 标记）

**位置：** `DataGrid.Privates.cs:1273-1295`

**为什么不动：**
- agent 分析有误：那个循环走的是 column group 数据结构（业务模型）而非 visual tree
- column group 是平展的数据结构，不会成环

**建议：** 不动。

### Layout invalidate 系列（MEDIUM/HIGH 标记）

**位置：** `DataGrid.Privates.cs:2136-2195` 的 `InvalidateColumnHeadersMeasure` / `InvalidateRowsMeasure` 等

**为什么不动：**
- 这是 Avalonia layout 系统的正常调用方式
- 上游 Avalonia 也有几乎相同的 invalidate 调用
- 不存在「Invalidate 风暴」——每次 invalidate 是 O(1)，框架自带去重和批处理

**建议：** 不动。

---

## 工作量估算

真正需要修复的 3 项（A1、A2、B1）：

| 项目 | 改动行数 | 影响 caller 数 | 风险 |
|---|---|---|---|
| A1. `HandleGotFocus` | ~10 行（同文件） | 0（内部方法） | 低 |
| A2. `TreeHelper.ContainsChild` | 删整个方法 + 替换所有 caller | 需 grep 确认 | 低 |
| B1. `GetColumnContainingElement` | ~12 行（同文件） | 数处指针事件路径 | 低 |

三项均为「删手写 walk → 换框架 API」同质修复，1:1 等价替换。

## 教训

- **从 WPF/Silverlight 移植的代码**几乎所有「手写树遍历」段落都该审视一遍：上游 Avalonia 早已用 `IsVisualAncestorOf` / `FindAncestorOfType<T>` / `GetVisualAncestors` 等扩展替代
- **手写 walk 的两个核心风险**：(a) 缺乏环检测；(b) 走 logical parent 比 visual parent 更危险，logical 链在 Avalonia 12 下更可能成环
- **explore agent 报告需要人工复核**：agent 倾向于把所有「看上去复杂的代码」标为风险，但有些是合理设计（如 `WaitForLostFocus`）、有些是 AtomUI 自有需求（如 `NotifyDataContextPropertyForAllRowCells`）
- **一次只对齐一类 pattern**：本次只对齐手写树遍历，不顺手改 invalidate / dispatcher / 重入守卫等无关项

## 后续

待用户确认是否实施 A1、A2、B1 的修复。修复完成后本文档将更新「实施情况」章节。

---

## 实施情况（2026-05-07）

A1、A2、B1 均已按建议方案实施，构建通过。

### A1. `HandleGotFocus` ✅

**文件：** `src/AtomUI.Desktop.Controls.DataGrid/DataGrid.Privates.cs:483-494`

将手写 `while (focusedElement != null) ... focusedElement = focusedElement.GetVisualParent()` 替换为：

```csharp
var focusedElement = e.Source as Visual;
_focusedObject = focusedElement;
var focusedRow = focusedElement?.FindAncestorOfType<DataGridRow>(includeSelf: true);
if (focusedRow != null && focusedRow.OwningGrid == this && _focusedRow != focusedRow)
{
    ResetFocusedRow();
    _focusedRow = focusedRow.IsVisible ? focusedRow : null;
}
```

行为等价：原循环遇到第一个 `DataGridRow` 后代即 break，等同于 `FindAncestorOfType<DataGridRow>(includeSelf: true)`。

### A2. `TreeHelper.ContainsChild` ✅

**文件：** `src/AtomUI.Desktop.Controls.DataGrid/Utils/TreeHelper.cs`

整个 `ContainsChild` 扩展方法已删除。文件保留 `ContainsFocusedElement`（与 visual tree walk 无关）。

唯一 caller `DataGrid.Privates.cs:1217` 改为：

```csharp
// before
if (editingElement != null && editingElement.ContainsChild(_focusedObject))

// after
if (editingElement != null && _focusedObject is Visual focusedVisual && editingElement.IsVisualAncestorOf(focusedVisual))
```

### B1. `DataGridColumn.GetColumnContainingElement` ✅

**文件：** `src/AtomUI.Desktop.Controls.DataGrid/Column/DataGridColumn.cs:647-653`

将手写 walk 替换为：

```csharp
public static DataGridColumn? GetColumnContainingElement(Control element)
{
    return element.FindAncestorOfType<DataGridCell>(includeSelf: true)?.OwningColumn
           ?? element.FindAncestorOfType<DataGridColumnHeader>(includeSelf: true)?.OwningColumn;
}
```

行为等价：原循环对每一层依次匹配 `DataGridCell` 和 `DataGridColumnHeader`。由于 cell 和 header 不会互为祖先，「找到 cell 的祖先 → 没找到再找 header 的祖先」结果与「逐层匹配 cell 或 header」一致。

### 构建验证

```
dotnet build src/AtomUI.Desktop.Controls.DataGrid/AtomUI.Desktop.Controls.DataGrid.csproj
Build succeeded.
13 Warning(s) (均为既存警告，与本次修复无关)
0 Error(s)
```

### 注释

A1 和 B1 的新代码上方都加了一段注释，引用本文档和 `datagrid-cross-instance-rowdetails-hang.md`，说明这是为了对齐 Avalonia 12 框架 API、规避手写 walk 撞循环边的隐患。

