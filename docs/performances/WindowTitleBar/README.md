# WindowTitleBar 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 2 #8
> 状态：本轮完成 `CaptionButtonGroup` 结构优化；页面级耗时仅 smoke-only，不作为收益证明。

---

## 0. 结论

本轮优化窗口标题栏按钮组里的 `CaptionButton` 模板结构：每个按钮原来固定创建 normal / checked 两套 `IconPresenter`，并通过 `IsVisible` 互斥显示。实际同一时刻只需要一个图标，所以现在模板保留一个 `IconPresenter`，由 `CaptionButton.EffectiveIcon` 在 `NormalIcon` / `CheckedIcon` 之间切换。

这不是把主题视觉搬到 C# 动态创建；功能视觉仍在 `ControlTheme` 中，C# 只维护运行态 direct property，符合 Theme Static Rule。

| 指标 | baseline | optimized | 计算 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Windows 默认标题栏 visual/root | 40 | 27 | `(40 - 27) / 40` | 32.50% fewer | 主收益 |
| Windows 默认标题栏 `IconPresenter`/root | 10 | 5 | `(10 - 5) / 10` | 50.00% fewer | 主收益 |
| Windows 默认标题栏 `PathIcon`/root | 8 | 5 | `(8 - 5) / 8` | 37.50% fewer | 主收益 |
| Windows 默认标题栏 KB/item | 782.4 KB | 531.6 KB | `(782.4 - 531.6) / 782.4` | 32.05% less | 主收益 |
| Linux 默认标题栏 visual/root | 40 | 27 | `(40 - 27) / 40` | 32.50% fewer | 主收益 |
| Linux 默认标题栏 `IconPresenter`/root | 10 | 5 | `(10 - 5) / 10` | 50.00% fewer | 主收益 |
| macOS 默认标题栏 visual/root | 10 | 7 | `(10 - 7) / 10` | 30.00% fewer | 主收益 |
| macOS 默认标题栏 `IconPresenter`/root | 2 | 1 | `(2 - 1) / 2` | 50.00% fewer | 主收益 |
| Windows 默认标题栏 ms/item | 2.116 ms | 1.641 ms | `(2.116 - 1.641) / 2.116` | 22.45% faster | smoke-only，不作为收益证明 |

---

## 1. 资格门槛

`WindowTitleBar` 被 `Dialog`、`ImagePreviewer`、`Window` 复用。完整 `Window` 由于常驻实例数低，已按 SKILL Tier 1 §13 降级；但标题栏按钮组属于每个桌面窗口、弹窗、预览器窗口都会持有的基础结构，且 Windows / Linux 样式下每组包含多个 caption button。

本轮只处理可以用控件级 harness 稳定证明的 `CaptionButtonGroup` 模板结构，不声明 Gallery 页面级提升。

---

## 2. 根因

`CaptionButtonTheme.axaml` 和 `WindowsCaptionButtonTheme.axaml` 原模板对每个按钮创建：

- `PART_NormalIconPresenter`
- `PART_CheckedIconPresenter`
- 包住两者的 `Panel`

normal / checked 两个 presenter 使用 `IsVisible` 互斥显示。隐藏元素不会参与当前布局和渲染，但模板创建时两套 presenter、两套 icon child 和 wrapper panel 已经进入实例化成本。控件级 baseline 直接显示 Windows / Linux 默认按钮组有 `IconPresenter=10`，优化后降到 `5`，与“每个按钮只保留一套 presenter”的预期一致。

---

## 3. 改动

### 3.1 `CaptionButton` 增加当前图标属性

新增 `EffectiveIcon` direct property：

| 状态 | `EffectiveIcon` |
| --- | --- |
| `IsChecked=false` | `NormalIcon` |
| `IsChecked=true` 且 `CheckedIcon != null` | `CheckedIcon` |
| `IsChecked=true` 且 `CheckedIcon == null` | `NormalIcon` |

触发更新的属性：

- `NormalIcon`
- `CheckedIcon`
- `IsChecked`

### 3.2 模板保留单个 presenter

两个 caption button theme 都改为单个模板子项：

```xml
<atom:IconPresenter
    Name="PART_IconPresenter"
    Icon="{TemplateBinding EffectiveIcon}"
    Width="{TemplateBinding IconWidth}"
    Height="{TemplateBinding IconHeight}"
    IsMotionEnabled="{TemplateBinding IsMotionEnabled}" />
```

保留行为：

- normal / checked 图标切换；
- icon size；
- motion 开关；
- Windows / Linux / macOS caption button group 可见性状态。

---

## 4. 实测结果

命令：

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite windowtitlebar --count 80
```

### 4.1 控件结构收益

| Scenario | visual baseline | visual optimized | `IconPresenter` baseline | `IconPresenter` optimized | `PathIcon` baseline | `PathIcon` optimized | KB/item baseline | KB/item optimized |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `CaptionButtonGroup.Windows.Default` | 40 | 27 | 10 | 5 | 8 | 5 | 782.4 | 531.6 |
| `CaptionButtonGroup.Windows.FullScreen` | 27 | 19 | 6 | 3 | 5 | 3 | 526.8 | 368.8 |
| `CaptionButtonGroup.Windows.Maximized` | 33 | 23 | 8 | 4 | 6 | 4 | 630.1 | 452.7 |
| `CaptionButtonGroup.Linux.Default` | 40 | 27 | 10 | 5 | 8 | 5 | 740.4 | 513.3 |
| `CaptionButtonGroup.macOS.Default` | 10 | 7 | 2 | 1 | 2 | 1 | 181.0 | 117.4 |

### 4.2 耗时 smoke

| Scenario | ms/item baseline | ms/item optimized | 变化 | 结论 |
| --- | ---: | ---: | ---: | --- |
| `CaptionButtonGroup.Windows.Default` | 2.116 | 1.641 | +22.45% | smoke-only |
| `CaptionButtonGroup.Windows.FullScreen` | 1.475 | 0.881 | +40.27% | smoke-only |
| `CaptionButtonGroup.Windows.Maximized` | 1.650 | 1.318 | +20.12% | smoke-only |
| `CaptionButtonGroup.Linux.Default` | 1.829 | 1.432 | +21.71% | smoke-only |
| `CaptionButtonGroup.macOS.Default` | 0.354 | 0.258 | +27.12% | smoke-only |
| `WindowTitleBar.Batch.Windows8` | 6.798 | 7.467 | -9.84% | outer titlebar batch 噪声，不作为本轮收益或回退依据 |

裁剪复测里 `WindowTitleBar.Batch.Windows8` 为 `6.900 ms/item`，而 caption group 结构计数保持不变；因此本轮收益只按 caption button group 的结构与分配下降声明。

---

## 5. 正确性验证

命令：

```bash
dotnet build -c Release -f net10.0 --no-restore \
  tools/performances/AtomUI.Performance/AtomUI.Performance.csproj

dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-windowtitlebar-states
```

结果：

| 验证 | 结果 |
| --- | --- |
| Release build | passed |
| `--verify-windowtitlebar-states` | passed |
| Windows / Linux / macOS caption buttons | exactly one `IconPresenter` |
| 初始图标 | presenter 指向 `NormalIcon` |
| checked 切换 | presenter 指向 `CheckedIcon` |
| unchecked 切回 | presenter 指回 `NormalIcon` |

---

## 6. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*` 方法 | 0 |
| 新增 `_ignoreXxx` / suppression flag | 0 |
| 新增 disposable 字段 | 0 |
| 新增 timer / global subscription | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 生产文件范围 | `CaptionButton.cs` + 2 个 caption button theme |

结论：这是模板结构收敛，不改变 public API，不引入生命周期释放点。
