# Tag 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 0 #4
> 状态：本轮已完成 + 微基准验证。

---

## 0. 微基准对比（控件级，60 instances/scenario）

工具：`tools/performances/AtomUI.Performance` 哈纳斯。
口径：`--suite tag --count 60`，同进程双侧。

| Scenario | ms/item baseline → optimized | KB/item baseline → optimized | Visual/root |
| --- | --- | --- | --- |
| `Tag.Default.NoText` | 0.502 → 0.491 | 90.2 → 88.2 **(−2 KB)** | 7.0 → 7.0 |
| `Tag.Default.Text` | 0.571 → 0.496 | 91.3 → 89.6 **(−2 KB)** | 7.0 → 7.0 |
| `Tag.PresetColor` | 0.561 → 0.823 (noise) | 97.0 → 95.3 **(−2 KB)** | 7.0 → 7.0 |
| `Tag.StatusColor` | 0.701 → 0.773 (noise) | 97.1 → 95.4 **(−2 KB)** | 7.0 → 7.0 |
| `Tag.CustomColor` | 0.690 → 0.712 (noise) | 96.5 → 94.8 **(−2 KB)** | 7.0 → 7.0 |
| `Tag.WithIcon` | 1.006 → 0.799 | 151.0 → 149.3 **(−2 KB)** | 9.0 → 9.0 |
| `Tag.Closable` | 1.366 → 1.205 (noise) | **189.3 → 189.3 (0%)** | 11.0 → 11.0 |
| `Tag.PresetColor.Closable` | 1.367 → 0.849 (noise) | **193.3 → 193.3 (0%)** | 11.0 → 11.0 |

**关键信号**：

- **非 closable Tag 全线减少约 2 KB/instance**（6 scenarios 一致）。
- **closable Tag 分配完全不变**——这正是行为保留信号（`IsClosable=true` 路径仍创建 default `CloseOutlined`）。
- ms/item 在 sub-millisecond 量级，单 scenario 差值多在变异范围内；非 closable 下趋势向下，closable 下持平/略改善（变异）。

Gallery 实际放大：`TagShowCase.axaml` 76 个 Tag 引用，约 70 个 `IsClosable` 未设置（默认 false），× 2 KB ≈ **Gallery 加载减少约 140 KB 分配**。Select multi-mode / Transfer 标签按 SelectedItem 数量乘倍。

---

## 1. 范围

- `src/AtomUI.Controls/Tag/AbstractTag.cs` — Tag 与 SelectTag 的共享基类，本轮把 default CloseIcon 创建条件门控到 `IsClosable=true`。
- `src/AtomUI.Desktop.Controls/Tag/Tag.cs` — Desktop Tag 派生（11 行壳，未改）。
- `src/AtomUI.Desktop.Controls/Select/SelectTag.cs` — `SelectTag : Tag`（未直接改，但 `IsClosable=true` 默认值保证它行为不变）。

---

## 2. 改动

### 2.1 `AbstractTag.SetupDefaultCloseIcon` 门控到 `IsClosable=true`

**原实现**（每个 Tag 实例无条件创建 `CloseOutlined`）：

```csharp
private void SetupDefaultCloseIcon()
{
    if (CloseIcon is null)
    {
        ClearValue(CloseIconProperty);
        SetValue(CloseIconProperty, new CloseOutlined());
    }
}
```

`OnApplyTemplate` 调用一次。即使 `IsClosable=false`、`PART_CloseButton.IsVisible=false`，default `CloseOutlined` 也已经分配。Gallery 76 个 Tag × 大多数非 closable = 浪费分配。

**新实现**：

```csharp
private void SetupDefaultCloseIcon()
{
    if (!IsClosable)
    {
        return;
    }
    if (CloseIcon is null)
    {
        ClearValue(CloseIconProperty);
        SetValue(CloseIconProperty, new CloseOutlined());
    }
}
```

并在 `OnPropertyChanged` 里把 `IsClosable` 变化也接到这条路径（`IsAttachedToVisualTree` 内）：

```csharp
else if (change.Property == IsClosableProperty)
{
    SetupDefaultCloseIcon();
}
```

效果：
- `IsClosable=false` 默认：跳过分配，CloseIcon 保持 null（IconButton.IsVisible=false 已经隐藏，视觉无影响）。
- `IsClosable=true`：路径不变，仍创建 default `CloseOutlined`。
- 运行时 `IsClosable=false → true` 切换：`OnPropertyChanged` 触发，按需创建。
- 运行时 `IsClosable=true → false` 切换：CloseIcon 留在内存（不主动清空），下次切回 true 时复用——避免反复分配。

### 2.2 Theme 不变

`Tag/Themes/TagTheme.axaml` 保持原结构。IconPresenter 与 PART_CloseButton 都是 axaml 静态 + `IsVisible="{TemplateBinding ...}"` 切换——符合 SKILL Theme Static Rule，未触发任何 C# 动态创建。

---

## 3. 不在本轮范围

- `SetupTagColorInfo` 的 `foreach (var entry in PresetColorMap)` 线性查找（按 string 匹配 12 个 preset color 项）：每次 TagColor 变化做一次。Gallery 76 tags × 16 字符串比较 ≈ 1200 次 ToLower 比较；可改成预构建 `Dictionary<string, TagCalcColor>`，但单 tag 每次 < 0.05 ms，变异范围内难以验证收益。**留作 follow-up**，先看 Gallery 实测是否成为瓶颈。
- `SetupPresetColorMap` / `SetupStatusColorMap` 已经是 static 全局 dict + `force` 参数，theme 切换时重建。结构 OK，未改。

---

## 4. 验证

### 4.1 控件库构建

```
dotnet build src/AtomUI.Controls/AtomUI.Controls.csproj -c Debug --framework net10.0
→ Build succeeded. 0 Warning(s) 0 Error(s)
```

### 4.2 控件级微基准对比

详见 §0 表格。关键确认：

- 非 closable scenarios 全线 **-2 KB/instance**。
- closable scenarios **KB 完全不变**——证明 `IsClosable=true` 路径未受影响（行为保留）。

### 4.3 Gallery 视觉走查（人工）

`TagShowCase` 是首选：
- "Basic" section：非 closable + 1-2 closable 的 Tag，确认 close × 显示位置正确。
- "Colorful" section：preset color + status color tags 颜色与原一致。
- "Custom" section：`#f50` 之类自定义颜色，IsClosable + custom 组合也正确。
- 切换主题（如有 toggle）：preset color tags 颜色随主题变化（确认 SetupPresetColorMap force 路径生效）。

兼容性：Select multi mode（每个 SelectedItem 一个 `SelectTag`，IsClosable 默认 true）应该完全不变——SelectTag 走 closable 路径，`new CloseOutlined()` 仍执行。

---

## 5. 复杂度自评（SKILL Process Gate 3）

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 try/finally 标志位 | 0 |
| 同一文件新增 disposable 字段 | 0 |
| axaml 删除/新增 | 不变 |
| C# 净 `+5 / -1` | `if (!IsClosable) return;` + IsClosable 接 OnPropertyChanged |
| Theme Static Rule 检查 | **未触发** —— C# 内门控既有路径，无 axaml/C# 边界变动 |

---

## 6. 改动文件清单

| 路径 | 改动 |
| --- | --- |
| `src/AtomUI.Controls/Tag/AbstractTag.cs` | `SetupDefaultCloseIcon` 加 `if (!IsClosable) return;` 守卫；`OnPropertyChanged` 添加 IsClosableProperty → SetupDefaultCloseIcon 触发 |
| `tools/performances/AtomUI.Performance/Suites/Tag/TagScenarios.cs` | 新建 8 scenario |
| `tools/performances/AtomUI.Performance/Program.cs` | dispatch 加 `--suite tag` |

总计：1 个生产文件改动 + 1 个新 perf suite。

---

## 7. 已知不足

- 单 Tag 每实例 KB 收益（~2 KB）相对 Tier 0 的 TextBlock / Popup 较小。Tag 优化的真正杠杆在 **Gallery 累积量**：76 实例 × ~70 非 closable × 2 KB ≈ 140 KB。Select 多选场景按 SelectedItems 数量再乘倍。
- ms/item 在 sub-millisecond 量级，单 scenario 测得的变异范围内 ±20%；KB/item 是更可靠的指标。
- `SetupTagColorInfo` 字符串查找微优化属下一轮。
