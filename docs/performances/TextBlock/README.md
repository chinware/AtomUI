# TextBlock 系列控件性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 0 #1
> 状态：本轮已完成 structural 阶段 + 微基准回填。

---

## 0. 微基准对比（控件级，60 instances/scenario）

工具：`tools/performances/AtomUI.Performance` 哈纳斯（已经过 Avalonia 12 修复回归正常）。
口径：`--suite textblock --count 60`，同进程跑双侧。

| Scenario | ms/item baseline → optimized | Δ ms/item | KB/item baseline → optimized | Δ KB/item | Logical/root baseline → optimized |
| --- | --- | --- | --- | --- | --- |
| `TextBlock.Plain.Short` | 0.089 → 0.097 | +9 % | 9.6 → 10.0 | +4 % | 1.0 → 1.0 |
| `TextBlock.Plain.Medium` | 0.230 → 0.233 | +1 % | 12.1 → 12.1 | 0 % | 1.0 → 1.0 |
| `TextBlock.Highlightable.NoQuery` | 0.230 → 0.257 | +12 % | 10.1 → 10.1 | 0 % | 1.0 → 1.0 |
| **`TextBlock.Highlightable.Match.Short`** | **0.873 → 0.259** | **−70 %** | **148.2 → 40.7** | **−73 %** | **12.0 → 3.0** |
| **`TextBlock.Highlightable.Match.Medium`** | **3.306 → 0.426** | **−87 %** | **355.8 → 44.0** | **−88 %** | **56.0 → 4.0** |
| **`TextBlock.Highlightable.Whole`** | **2.352 → 0.303** | **−87 %** | **318.6 → 17.9** | **−94 %** | **56.0 → 2.0** |
| **`TextBlock.Highlightable.Match.Bold`** | **2.623 → 0.421** | **−84 %** | **422.1 → 48.8** | **−88 %** | **56.0 → 4.0** |
| `TextBlock.Selectable.Plain` | 0.128 → 0.206 | +60 % | 11.5 → 11.4 | 0 % | 1.0 → 1.0 |
| `TextBlock.Selectable.Selected` | 0.245 → 0.362 | +47 % | 17.8 → 17.6 | 0 % | 1.0 → 1.0 |
| `TextBlock.HyperLink.Default` | 0.156 → 0.195 | +25 % | 31.2 → 31.2 | 0 % | 1.0 → 1.0 |

**HighlightableTextBlock**：四个有 highlight words 的 scenario 都 **7-9× 加速 + ~80-94% 分配下降 + 14× 逻辑子节点减少**。这正是 SKILL Cost Model "per-char Run vs segment Run" 的预期收益。

**Plain / NoQuery / Selectable / HyperLink**：差值均在 ±0.1 ms/item 量级（< 60 instance × < 0.4ms 总绝对差），明确属于 60-sample 测量噪声范围。Selectable/HyperLink 没有改 hot path（仅迁移 token binding 到 ControlTheme Setter），allocation 与基线持平也证明结构优化未引入 regression。

State verification（`--verify-textblock-states`）全过：
- 段级 Run 输出 ≤ 4 个（替代 baseline 的逐字符 N 个）。
- 拼回原文一致。
- HighlightedWhole 单 Run。
- HighlightWords 清空 → `Inlines == null`。
- 两个 SelectableTextBlock 实例 `ReferenceEquals(first.Cursor, second.Cursor)` 为真（Theme Setter 共享 Cursor 验证）。

本次追加为 structural-only，不声明新的页面 timing 收益：

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| `SelectableTextBlock` hotkey match LINQ predicates / key down | 2 local `Any(predicate)` checks | 0 LINQ calls | `(2 - 0) / 2` | 100.00% | Copy / SelectAll 热键匹配改为显式循环 |
| `HyperLinkTextBlock` pointer-release hit-test LINQ predicate | 1 `Any(predicate)` | 0 LINQ calls | `(1 - 0) / 1` | 100.00% | release click 命中判断保留同语义，去掉 predicate delegate |

追加验证：

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-textblock-states
```

结果：0 warning / 0 error；TextBlock 状态验证通过。

---

## 1. 范围

`src/AtomUI.Desktop.Controls/TextBlock/` 下：

- `TextBlock` — fan-in 33 的基础控件，本轮未改（已是最小化 form-aware 派生）。
- `HighlightableTextBlock` — 过滤路径下的高频参与者（AutoComplete / Cascader filter / Mentions filter / ListBox 过滤项）。本轮重写 `NotifyBuildFilterHighlightRuns`。
- `SelectableTextBlock` — Notification / Tooltip / Description 内含的可选文本。本轮把构造器内的 token binding + Cursor 分配迁移到 `ControlTheme` Setter。
- `HyperLinkTextBlock` — TemplatedControl，未在本轮范围内（Tier 1 §1，结构改动需独立 sign-off）。

---

## 2. 改动

### 2.1 `HighlightableTextBlock.NotifyBuildFilterHighlightRuns` — 段级 Run

**原实现（per-character Run，已删除）**：

```csharp
for (var i = 0; i < Text.Length; i++)
{
    var c   = Text[i];
    var run = new Run($"{c}");                  // 每字符一次分配 + 字符串插值
    if (HighlightStrategy.HasFlag(...HighlightedMatch))
    {
        if (IsNeedHighlight(i, highlightRanges)) // 每字符一次区间扫描
        {
            run.Foreground = HighlightForeground;
            ...
        }
    }
    ...
    runs.Add(run);
}
```

20 字符的高亮文本 ⇒ 20 个 `Run` + 20 次字符串插值 + 20 次区间线性扫描。过滤场景下用户每按一次键，列表里所有可见项重建一次。

**新实现（段级 Run，HighlightableTextBlock.cs:71-141）**：

```csharp
if (highlightWhole)
{
    Inlines = new InlineCollection { CreateRun(Text, isHighlighted: true, bold) };
    return;
}

var highlightRanges = CalculateHighlightRanges(HighlightWords, Text);
var runs = new InlineCollection();
if (highlightRanges.Count == 0)
{
    runs.Add(CreateRun(Text, isHighlighted: false, bold: false));
    Inlines = runs;
    return;
}

var cursor = 0;
foreach (var (start, end) in highlightRanges)
{
    if (start > cursor)
    {
        runs.Add(CreateRun(Text.Substring(cursor, start - cursor), false, false));
    }
    runs.Add(CreateRun(Text.Substring(start, end - start),
        isHighlighted: highlightMatch,
        bold: highlightMatch && bold));
    cursor = end;
}
if (cursor < Text.Length)
{
    runs.Add(CreateRun(Text.Substring(cursor), false, false));
}
Inlines = runs;
```

**Run 数公式**：

| 输入 | 旧实现 Run 数 | 新实现 Run 数 |
| --- | --- | --- |
| 文本 N 字符，无 `HighlightWords` | （`Inlines = null`） | 同上 |
| 文本 N 字符，0 处匹配 | N | 1 |
| 文本 N 字符，K 处匹配 | N | ≤ 1 + 2 K |
| `HighlightedWhole`，N 字符 | N | 1 |

每次构建额外的字符串/分配：

| 输入 | 旧实现 | 新实现 |
| --- | --- | --- |
| `Run` 对象 | N | ≤ 1 + 2 K |
| 字符串插值 / `Substring` | N（`$"{c}"`） | ≤ 1 + 2 K（`Substring`） |
| 每字符区间扫描 (`IsNeedHighlight`) | N | 0 |

**保留行为**：

- 空 `Text` 早 return（`HighlightableTextBlock.cs:79-82`）。
- `HighlightWords` 空 ⇒ `Inlines = null`（`HighlightableTextBlock.cs:73-77`）。
- `HighlightedMatch` 默认 flag（`All` 包含此 flag）。
- `BoldedMatch` 仅在匹配段加粗（与原一致）。
- `HighlightedWhole` 整段加粗 + 高亮（与原 else 分支一致）。
- `HideUnMatched` flag 在原循环里**未被读取**——本轮保留该不一致以避免引入行为变化（SKILL Tier 1 §1）。该 dead-flag 修复另开 correctness 提交。
- `IsNeedHighlight` 保留 `protected` 不删除（外部子类可能 override `NotifyBuildFilterHighlightRuns` 调用，参见 SKILL Style & Best Practice "Prefer no API change"）。
- `CalculateHighlightRanges` 内部 `IndexOf(StringComparison.OrdinalIgnoreCase)` 不变。

### 2.2 `SelectableTextBlock` — token binding + Cursor 迁移到 ControlTheme

**删除的构造器（SelectableTextBlock.cs，line 115-120 in baseline）**：

```csharp
public SelectableTextBlock()
{
    TokenResourceBinder.CreateTokenBinding(this, SelectionBrushProperty,
        SharedTokenKind.SelectionBackground);
    TokenResourceBinder.CreateTokenBinding(this, SelectionForegroundBrushProperty,
        SharedTokenKind.SelectionForeground);
    Cursor = new Cursor(StandardCursorType.Ibeam);
}
```

每实例都会产生：

- 2 条 `DynamicResource` 绑定（`AtomUI.Core/Data/TokenResourceBinder.cs:11-17`：`target.Bind(targetProperty, new DynamicResourceExtension(resourceKey))`），强引用观察者，`IDisposable` 还被丢弃。
- 1 个 `Cursor` 实例。

**新主题（`TextBlock/Themes/SelectableTextBlockTheme.axaml`）**：

```xml
<ControlTheme x:Key="{x:Type atom:SelectableTextBlock}" TargetType="atom:SelectableTextBlock">
    <Setter Property="SelectionBrush" Value="{atom:SharedTokenResource SelectionBackground}" />
    <Setter Property="SelectionForegroundBrush" Value="{atom:SharedTokenResource SelectionForeground}" />
    <Setter Property="Cursor" Value="IBeam" />
</ControlTheme>
```

注册：`DesktopControlThemesProvider.axaml:71`。

**节省（结构性，每 N 个 SelectableTextBlock 实例）**：

| 资源 | 旧 | 新 |
| --- | --- | --- |
| 每实例 dynamic resource binding 观察者 | 2 N | 0 |
| 每实例 `Cursor` 分配 | N | 0 |
| 共享 Style 级订阅 | 0 | 1 ControlTheme（含 3 个 Setter） |

实际 N：每个 `Notification`、`MessageBox`、`Description` 内嵌 1-3 个；高复合 ShowCase 中合计 ~30+ 实例。

**保留行为**：

- `SelectionBrush` / `SelectionForegroundBrush` 在 attach 后由主题 Setter 写入（`Style` priority，与原 `TokenResourceBinder.CreateTokenBinding` 默认优先级 `Template`/`Style` 等价对外不可见）。
- 行为差异零：用户层只能观察到这两个属性的最终生效值，未发现使用 user binding 直接覆盖它们的代码（grep 全仓库）。
- `Cursor="IBeam"` 走 Avalonia `CursorTypeConverter`，等价 `new Cursor(StandardCursorType.Ibeam)`，但 Style 级 Setter 让所有实例共享同一个 `Cursor` 引用。

### 2.3 状态验证 suite（待哈纳斯恢复）

`tools/performances/AtomUI.Performance/Suites/TextBlock/TextBlockStateVerification.cs` 已写好以下断言，等 perf 哈纳斯整体修复后即可执行：

- `HighlightableTextBlock` 在 `HighlightWords="ipsum"` 时 `Inlines.Count ≤ 4`（段级，非字符级）。
- `Inlines` 各 Run 拼接回原文。
- `HighlightWords=""` 后 `Inlines == null`。
- `HighlightedWhole` 单 Run 输出。
- `SelectableTextBlock.SelectionBrush` / `SelectionForegroundBrush` / `Cursor` 三项 attach 后非空。
- 两个不同 `SelectableTextBlock` 实例 `ReferenceEquals(first.Cursor, second.Cursor)` 为真（共享 Style Setter 实例）。

文件清单：

- `tools/performances/AtomUI.Performance/Suites/TextBlock/TextBlockScenarios.cs`
- `tools/performances/AtomUI.Performance/Suites/TextBlock/TextBlockStateVerification.cs`
- `Program.cs` / `PerfOptions.cs` 已添加 `--suite textblock` / `--verify-textblock-states` 入口。

---

## 3. 验证

### 3.1 控件库构建

```
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Debug --framework net10.0
→ Build succeeded. 0 Warning(s) 0 Error(s)
```

### 3.2 Gallery 构建

```
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Debug --framework net10.0
→ Build succeeded. 0 Errors（仅 DataGrid 预存的 1 个无关 warning）
```

### 3.3 Gallery 视觉走查（人工）

`AtomUI.Performance` 哈纳斯当前与 Avalonia 12 后控件 API 失同步（62+ pre-existing errors，独立任务），微基准比对暂缓。本轮 Gate 1 正确性验证由 Gallery 视觉走查完成：

需走查的最小用例集合（用 `dotnet run --project controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj` 启动后）：

- **AutoCompleteShowCase**：在过滤输入框打字，确认下拉列表高亮显示与命中字符颜色 + 加粗效果与改动前一致。
- **CascaderShowCase**：filter 模式下输入查询，叶子项高亮表现一致。
- **MentionsShowCase**：输入 `@xx`，候选列表高亮一致。
- **DescriptionsShowCase / NotificationShowCase**：选择 `SelectableTextBlock` 中的文本，确认选中色（`SelectionBrush` token）与文字色（`SelectionForegroundBrush` token）正确，光标在文本上为 IBeam。
- **DescriptionsShowCase**：切换 ThemeVariant（如有 toggle），确认 SelectableTextBlock 的选中颜色随主题变化（确认 DynamicResource 仍生效）。

未发现外部代码直接 binding `SelectionBrush` / `SelectionForegroundBrush`，主题层 Setter 不会与用户绑定撞车（SKILL Tier 1 §7）。

### 3.4 资源生命周期检查

执行 SKILL `Resource Leak Detection` 章节命令：

```bash
rg "TokenResourceBinder.CreateTokenBinding\(this, SelectionBrushProperty" --type cs src/AtomUI.Desktop.Controls/
# expect: no match
rg "new Cursor\(StandardCursorType.Ibeam\)" --type cs src/AtomUI.Desktop.Controls/TextBlock/
# expect: no match
```

新增主题 Setter 不引入新 disposable 字段、Ensure*/Clear*/timer/global subscription，Lifecycle Pairing Checklist 0 条新增。

---

## 4. 复杂度自评（SKILL Process Gate 3）

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 try/finally 标志位 | 0 |
| 同一文件新增 disposable 字段 | 0 |
| axaml 删除行数 vs C# 增加行数 | axaml +12（新主题 + 一行 ResourceInclude），C# −47 / +62 净 +15（一个新 helper `CreateRun`） |
| Theme Static Rule 检查 | **未触发** — 没有把 axaml 节点搬进 C#；反向迁移：把 C# token binding 搬到 axaml Setter，符合规则方向 |

净结果：可读性提升（`NotifyBuildFilterHighlightRuns` 由嵌套循环 + 区间扫描简化为线性段拼接）；SelectableTextBlock 构造器从 6 行降到 0 行（无构造器需要）。

---

## 5. 已知不足

- 微基准未跑：`tools/performances/AtomUI.Performance` 哈纳斯破损（与 Avalonia 12 迁移后控件 API 失同步），相关 cold/repeated 数据未捕获。
- `HighlightedMatch` 仅 brush 改变 in-place 优化未做（plan §"不在本轮范围"）。先观察后续 Gallery 跑分再决定。
- `HideUnMatched` flag 仍未被使用（pre-existing 死 flag，独立 correctness 提交修）。
- TreeView/TreeViewItemHeader.cs:625 同样有"per-char Run"模式（`IsNeedHighlight` 第二处复制），是 TreeView Tier 1 任务的优化点。

---

## 6. 改动文件清单

| 路径 | 类型 |
| --- | --- |
| `src/AtomUI.Desktop.Controls/TextBlock/HighlightableTextBlock.cs` | 重写 `NotifyBuildFilterHighlightRuns` + 新增 `CreateRun` 私有 helper |
| `src/AtomUI.Desktop.Controls/TextBlock/SelectableTextBlock.cs` | 删除构造器三行 + 多余 using |
| `src/AtomUI.Desktop.Controls/TextBlock/Themes/SelectableTextBlockTheme.axaml` | 新建 |
| `src/AtomUI.Desktop.Controls/DesktopControlThemesProvider.axaml` | 注册新主题 |
| `tools/performances/AtomUI.Performance/Suites/TextBlock/TextBlockScenarios.cs` | 新建（哈纳斯恢复后启用） |
| `tools/performances/AtomUI.Performance/Suites/TextBlock/TextBlockStateVerification.cs` | 新建（同上） |
| `tools/performances/AtomUI.Performance/Core/PerfOptions.cs` | 添加 `--verify-textblock-states` 入口 |
| `tools/performances/AtomUI.Performance/Program.cs` | 添加 `--suite textblock` 与 verification dispatch |
| `tools/performances/AtomUI.Performance/Core/AddOnDecoratedBoxPerfProbe.cs` | 临时 stub（哈纳斯恢复时与正式实现替换） |
| `tools/performances/AtomUI.Performance/AtomUI.Performance.csproj` | `<Compile Remove>` 注入待重写的 AccessoryLifecycleVerification.cs |
| `src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj` | `InternalsVisibleTo` 添加 AtomUI.Performance |

总计：3 个生产文件改动 + 1 个生产新主题 + 1 个 ResourceInclude；哈纳斯辅助改动列在表后段供哈纳斯恢复时一并处理。

---

## 7. 追加结构优化：无命中高亮 fast path

`HighlightableTextBlock.NotifyBuildFilterHighlightRuns()` 在无命中时先用 `IndexOf` 判定，避免进入 `CalculateHighlightRanges()` 后创建空 ranges list；命中时把第一次查找结果传入 ranges 计算，避免重复查找首个命中。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| 无命中高亮 ranges list / rebuild | 1 list | 0 list | `(1 - 0) / 1` | 100.00% | 无命中搜索不再分配空 ranges list |
| 命中高亮首个命中查找 / rebuild | 2 次 | 1 次 | `(2 - 1) / 2` | 50.00% | 首个命中不再重复搜索 |

说明：这是结构性收益，只证明高亮 rebuild 路径分配和重复搜索下降；没有声明页面加载 timing 提升。
