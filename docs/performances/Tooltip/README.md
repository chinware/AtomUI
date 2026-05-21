# Tooltip 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 0 #6
> 状态：本轮已完成结构清理；headless bench 无法触发 Show 流程（pointer 事件不会自动 fire），bench 验证留待 Gallery 级 hover 场景。

---

## 1. 范围

- `src/AtomUI.Desktop.Controls/Tooltip/ToolTipService.cs` — 全局 ToolTip 服务（singleton via AvaloniaLocator），处理所有 ToolTip.Tip 控件的 hover-触发。
- `src/AtomUI.Desktop.Controls/Tooltip/ToolTip.cs` — 596 行，附加属性 + Show/Close 状态机。本轮未改。

---

## 2. 改动

### `ToolTipService.StartShowTimer` 复用 DispatcherTimer

**原实现**（每次 Show 触发都 allocate 一个 timer + 闭包）：

```csharp
private void StartShowTimer(int showDelay, Control control)
{
    _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(showDelay), Tag = (this, control) };
    _timer.Tick += (o, e) =>
    {
        if (_timer != null)
        {
            Open(control);
        }
    };
    _timer.Start();
}

private void StopTimer()
{
    _timer?.Stop();
    _timer = null;
}
```

`ToolTipService` 是 `AvaloniaLocator` 注册的 singleton（`ThemeManagerBuilderExtensions.cs:41`），整个会话只有一个实例，但每次 hover 进入新控件 → `HandleTipControlChanged` → `StartShowTimer` 都 `new DispatcherTimer(...)` + 一个 lambda 闭包（捕获 `this` 和 `control`）。然后 `StopTimer` 把字段置 null —— timer 对象 GC，闭包 GC。

Form 30 个 tooltipped 字段，用户 hover 划过一遍：30 次 timer 分配 + 30 次闭包分配。

**新实现**：复用 timer 实例，Tick handler 是绑定到方法的 EventHandler（不是闭包）：

```csharp
private void StartShowTimer(int showDelay, Control control)
{
    if (_timer == null)
    {
        _timer = new DispatcherTimer();
        _timer.Tick += HandleShowTimerTick;
    }
    _timer.Stop();
    _timer.Tag = control;
    _timer.Interval = TimeSpan.FromMilliseconds(showDelay);
    _timer.Start();
}

private void HandleShowTimerTick(object? sender, EventArgs e)
{
    if (_timer is null)
    {
        return;
    }
    _timer.Stop();
    if (_timer.Tag is Control pendingControl)
    {
        Open(pendingControl);
    }
}

private void StopTimer()
{
    _timer?.Stop();
    if (_timer != null)
    {
        _timer.Tag = null;
    }
}
```

**节省（结构性，每次 Show）**：

| 项 | 旧 | 新 |
| --- | --- | --- |
| `DispatcherTimer` 实例 | 1 per Show | 1 lifetime |
| `Tick` lambda 闭包 | 1 per Show | 0（method group 引用） |
| `Tag` tuple `(this, control)` 装箱 | 1 per Show | 0（直接存 control） |
| `_timer != null` 状态切换 | new/null 反复 | 持久存在，仅 Stop/Start |

Form/Slider/Steps/DataGrid 30+ tooltipped 字段 hover 一遍：~30 timer allocations 消除 + ~30 closure allocations 消除。

---

## 3. 不在本轮范围

- **`ToolTip.Open` 中 9 条 CompositeDisposable 绑定**（`ToolTip.cs:380-390`）：每次 Show 创建一个 `CompositeDisposable` + 9 个 binding observer。Close 时全部 dispose。这是按"每次 hover 进入不同控件需要重新绑定到当前 AdornedControl 属性"设计的，不能简单复用。要改的话需要把 popup 改成观察一个 `_currentAdornedControl` 字段而不是直接 bind 到 control 属性。**结构性较大改动，留作 follow-up**。
- **`ToolTipService.Update` 在每次 pointer move 都走 visual 树查找 tooltipped 祖先**（`ToolTipService.cs:95-117`）：这是 ToolTip 服务的根本设计——hover 即触发，无可缓存的状态。pointer-move 频率高但单次成本低（visual 树深度 < 30）。无可优化点。
- **`AbstractTag` 风格的"构造器 token binding 迁主题 Setter"**：ToolTip 没有这个反模式。Theme 已经是干净的 Setter 形式。

---

## 4. 验证

### 4.1 控件库构建

```
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Debug --framework net10.0
→ Build succeeded. 0 Warning(s) 0 Error(s)
```

### 4.2 微基准

`AtomUI.Performance` headless 环境无法触发 RawPointerEventArgs，所以 ToolTipService 的 Show 流程跑不到。本轮改动只能靠**结构性论证** + Gallery 走查验证。

未建立 `--suite tooltip`，原因同上：bench 跑不出 hover 流程，建场景成本 > 收益。

### 4.3 Gallery 视觉走查（人工）

需走查的最小用例：

- **TooltipShowCase**：基本 Tip 显示/消失、preset color、custom color、不同 placement、IsArrowVisible 切换。
- **FormShowCase**：表单字段的 tooltipped 帮助 icon hover 体验。
- **SliderShowCase**：thumb 上的 value tooltip。

行为不变性：

- Show delay / between-show delay 行为：第一次 hover 默认 400ms，连续 hover 间 100ms（与原一致）。
- Close 行为：pointer 离开/点击其他位置触发 close，与原一致。
- 多个 tooltipped 控件之间快速切换：timer 复用不引入跨控件污染（每次 `_timer.Tag = control` 覆盖）。

---

## 5. 复杂度自评（SKILL Process Gate 3）

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0（分离 lambda 为命名方法 `HandleShowTimerTick`，是清理） |
| 新增 try/finally 标志位 | 0 |
| 同一文件新增 disposable 字段 | 0 |
| C# 净改 | `+11 / -7` |
| Theme Static Rule 检查 | **未触发** |

---

## 6. 改动文件清单

| 路径 | 改动 |
| --- | --- |
| `src/AtomUI.Desktop.Controls/Tooltip/ToolTipService.cs` | `StartShowTimer` 改为复用 timer；新增 `HandleShowTimerTick` method group |

总计：1 个生产文件，~18 行改动。

---

## 7. 已知不足

- 无 microbench 对比数据（headless 限制）。real-world hover-heavy 场景（Form / DataGrid / Slider tooltip）走 Gallery 实测才能得到数字。
- `ToolTip.Open` 9 条 popup binding 的 per-show 分配是更大的潜在杠杆，留作下一轮 Tooltip-specific 优化（需要更小心的 popup-state 绑定重定向）。
