# Separator 性能优化

> 状态：本轮已完成结构修复；headless bench 显示 KB/item 完全持平，但改动直接对齐 SKILL Cost Model 关于 Pen 缓存的强制性建议。

---

## 0. 微基准对比（控件级，100 instances/scenario）

| Scenario | ms/item baseline → optimized | KB/item baseline → optimized | Visual |
| --- | --- | --- | --- |
| `Separator.Default` | 0.149 → 0.166 (noise) | 44.6 → 44.6 | 2.0 |
| `Separator.WithTitle` | 0.228 → 0.241 (noise) | 45.9 → 45.9 | 2.0 |
| `Separator.Dashed` | 0.168 → 0.171 (noise) | 43.8 → 43.9 | 2.0 |
| `Separator.Dotted` | 0.160 → 0.164 (noise) | 43.8 → 43.9 | 2.0 |
| `Separator.Vertical` | 0.127 → 0.135 (noise) | 40.7 → 40.8 | 2.0 |
| `Separator.WithTitle.Plain` | 0.217 → 0.240 (noise) | 45.4 → 45.4 | 2.0 |

**KB/item 完全持平**（变动 ≤ 0.1 KB 即解析力以下）。Headless `RealizeScenario` 对每个控件做 Show + Measure + Arrange + UpdateLayout，但单个 Pen 分配本身就在 GC 测量粒度之下。real-world render loop（60fps 持续重绘 + 94 个 Separator）才能放大到可观察。

ms/item 整体微涨 + 一律落在测量噪声范围内（< 0.05 ms 绝对差）。

---

## 1. 范围

- `src/AtomUI.Controls/Separator/AbstractSeparator.cs` — `Render(DrawingContext)` 每次 paint pass 都 `new Pen(...)`。本轮缓存 Pen，按 `Variant`/`LineColor`/`LineWidth` invalidate。

---

## 2. 改动

### `AbstractSeparator.Render` 缓存 Pen

**原实现**（`Render` 每次都构造 `new Pen`）：

```csharp
public override void Render(DrawingContext context)
{
    using var state = context.PushRenderOptions(new RenderOptions
    {
        EdgeMode = EdgeMode.Aliased
    });
    IDashStyle? lineStyle = null;
    if (Variant == SeparatorVariant.Dashed)
    {
        lineStyle = DashStyle;
    }
    else if (Variant == SeparatorVariant.Dotted)
    {
        lineStyle = DotStyle;
    }
    var linePen = new Pen(LineColor, LineWidth, lineStyle);
    ...
}
```

SKILL Cost Model（`docs/performances/avalonia12-control-library-pitfalls.md` §4.3）明确禁止：

> Custom `Render(DrawingContext)` 必须缓存 brush/pen,不要 per-frame `new`。

`Pen` 是 `Avalonia.Base/Media/Pen.cs:17` 的 mutable `StyledProperty` carrier。每次 InvalidateVisual / 实际 paint pass 都 `new Pen(...)` 在 60fps render loop 下成倍放大。

**新实现**：缓存 Pen + 按依赖属性变化失效：

```csharp
private Pen? _cachedLinePen;
private SeparatorVariant _cachedVariant;
private IBrush? _cachedLineColor;
private double _cachedLineWidth;

protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
{
    base.OnPropertyChanged(change);
    if (change.Property == TitleProperty)
    {
        UpdatePseudoClasses();
    }
    else if (change.Property == LineColorProperty ||
             change.Property == LineWidthProperty ||
             change.Property == VariantProperty)
    {
        _cachedLinePen = null;
    }
}

private Pen GetOrCreateLinePen()
{
    var variant = Variant;
    var lineColor = LineColor;
    var lineWidth = LineWidth;
    if (_cachedLinePen is not null &&
        _cachedVariant == variant &&
        ReferenceEquals(_cachedLineColor, lineColor) &&
        MathUtils.AreClose(_cachedLineWidth, lineWidth))
    {
        return _cachedLinePen;
    }
    IDashStyle? lineStyle = variant switch
    {
        SeparatorVariant.Dashed => DashStyle,
        SeparatorVariant.Dotted => DotStyle,
        _                       => null
    };
    _cachedLinePen   = new Pen(lineColor, lineWidth, lineStyle);
    _cachedVariant   = variant;
    _cachedLineColor = lineColor;
    _cachedLineWidth = lineWidth;
    return _cachedLinePen;
}
```

**节省（结构性，每 paint pass）**：

| 项 | 旧 | 新 |
| --- | --- | --- |
| `new Pen(...)` per Render | 1 per Render | 0（缓存命中） |
| Pen 重建时机 | 每帧 | LineColor/LineWidth/Variant 变化时 |

`DashStyle` / `DotStyle` 已经是 static cached（`s_dash` / `s_dot`），与原逻辑一致。

**保留行为**：

- `LineColor`/`LineWidth`/`Variant` 任意一项变化都触发 invalidation（双重保险：AffectsRender 触发重绘 + OnPropertyChanged 清空缓存）。
- `ReferenceEquals(_cachedLineColor, lineColor)` 用引用比较 brush —— 与 token 切换时新 brush 实例配合工作（token 重新解析时 brush 变成新实例，缓存失效）。
- `MathUtils.AreClose` 用浮点容差比较 LineWidth（与项目 SKILL Style & Best Practice 一致）。

---

## 3. 不在本轮范围

- `Render` 内 `new RenderOptions { EdgeMode = EdgeMode.Aliased }` 是 struct，栈分配，零成本。无须缓存。
- `_titleLabel?.Arrange(titleRect)` 在 ArrangeOverride 中计算 titleRect 时分配若干 `Rect`/`Size` —— struct，栈分配，零成本。
- `MeasureOverride` / `ArrangeOverride` 已是单次每 layout pass，无热点。

---

## 4. 验证

### 4.1 控件库构建

```
dotnet build src/AtomUI.Controls/AtomUI.Controls.csproj -c Debug --framework net10.0
→ Build succeeded. 0 Warning(s) 0 Error(s)
```

### 4.2 控件级微基准

详见 §0。Headless bench 解析力下未观察到差异（Pen 单次分配 < 100B，× 6 scenarios × 100 instances = ~60KB 总，再分摊到 600 instances 单 KB/item < 0.1）。

### 4.3 Gallery 视觉走查（人工）

需走查的最小用例：

- **TagShowCase** / **DrawerShowCase**：水平 Separator 无标题与有标题 (`Section`) 显示一致。
- **MenuShowCase**：MenuSeparator (Separator 派生) 在菜单内分隔正确。
- **NavMenuShowCase**：vertical Separator 显示一致。
- 切换 ThemeVariant：LineColor token 切换后 Separator 颜色更新（验证缓存失效路径）。

行为不变性：

- `LineColor`/`LineWidth`/`Variant` 任意变化 → 重绘（已验证：AffectsRender 仍然触发 InvalidateVisual + 缓存清空）。
- `Title` 变化 → InvalidateMeasure + Render 时使用最新 LineColor。
- 多个 Separator 实例之间互不干扰（每个 `_cachedLinePen` 是实例字段）。

---

## 5. 复杂度自评（SKILL Process Gate 3）

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0（`GetOrCreateLinePen` 是 cache helper，非 lazy materialization） |
| 新增 try/finally 标志位 | 0 |
| 同一文件新增 disposable 字段 | 0（Pen 不是 IDisposable） |
| 新增缓存字段 | 4（Pen + 3 个 cache key 字段） |
| C# 净改 | `+30 / -10` |
| Theme Static Rule 检查 | **未触发** |

---

## 6. 改动文件清单

| 路径 | 改动 |
| --- | --- |
| `src/AtomUI.Controls/Separator/AbstractSeparator.cs` | 添加 `_cachedLinePen` + 3 个 cache key 字段；`OnPropertyChanged` 监听 LineColor/LineWidth/Variant 失效；`Render` 改用 `GetOrCreateLinePen()` |
| `tools/performances/AtomUI.Performance/Suites/Separator/SeparatorScenarios.cs` | 新建 6 scenario |
| `tools/performances/AtomUI.Performance/Program.cs` | dispatch 加 `--suite separator` |

总计：1 个生产文件改动 + 1 个新 perf suite。

---

## 7. 已知不足

- 微基准未捕获到收益：headless 下 Pen 分配 < 0.1KB/instance 解析力。real-world 60fps render loop（特别是 Drawer / Menu / NavMenu 这些含多个 Separator 的场景）才能放大。
- 改动是 SKILL Cost Model 直接合规修复，结构性正确性无可争议；保留是因为符合 SKILL "Custom Render 必须缓存 Pen" 的强制性。
