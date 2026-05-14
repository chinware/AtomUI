# Button Phase 3-7

## 范围

本轮覆盖 Phase 3-7：

- Phase 3：普通 `Button` 移除固定 `PART_ButtonIcon`，仅在 `Icon != null && IsIconVisible && !IsLoading` 时创建 `IconPresenter#PART_ButtonIcon`。
- Phase 4：普通 `Button` 移除固定 `PART_WaveSpirit`，仅在 `Default` / `Primary` / `Dashed`、非 loading、且 `IsWaveSpiritEnabled=true` 时创建 `WaveSpiritDecorator`。
- Phase 5：`DropdownButtonTheme.axaml` 同步复用 Button 的 icon/wave 按需创建路径；移除 `DropdownButton.OnApplyTemplate()` 中未使用的空 `Border` 分配。
- Phase 6：基于 Phase 3-5 结果评估后，本轮不继续把 icon/loading brush selector 迁到代码。当前收益来自结构减重，继续迁移颜色状态会明显提高主题维护风险。
- Phase 7：复测控件级 Button suite 和真实 `ButtonShowCase` Gallery 场景。

## 与 Phase 0 对比

| 场景 | 指标 | Phase 0 | Phase 3-7 | 变化 |
| --- | --- | ---: | ---: | ---: |
| `Button.Default.Text` | `Visual/root` | 10.0 | 8.0 | -20.00% |
| `Button.Default.Text` | `IconPresenter/root` | 1.0 | 0.0 | -100.00% |
| `Button.Default.Text` | `ButtonLoadingHost/root` | 1.0 | 0.0 | -100.00% |
| `Button.Default.Text` | `ms/item` | 1.863 | 1.299 | -30.27% |
| `Button.Default.Text` | `KB/item` | 298.8 | 209.9 | -29.75% |
| `Button.Text.Text` | `Visual/root` | 10.0 | 7.0 | -30.00% |
| `Button.Link.Text` | `Visual/root` | 10.0 | 7.0 | -30.00% |
| `Button.Primary.Loading` | `Visual/root` | 12.0 | 9.0 | -25.00% |
| `Button.Mixed.Batch10` | `Visual/root` | 110.0 | 91.0 | -17.27% |
| `Button.Mixed.Batch10` | `KB/item` | 3296.9 | 2615.6 | -20.67% |

说明：

- 无 icon Button 已不再创建 `IconPresenter` 和默认 `MultiBinding`。
- `Text` / `Link` / loading / `IsWaveSpiritEnabled=false` 不再创建 `WaveSpiritDecorator`。
- `Default` / `Primary` / `Dashed` 仍保留 wave，因为这些类型会实际播放波浪动画。
- `SplitButton` 由两个 Button 组合，结构收益取决于内部两个 Button 是否实际有 icon/wave；本轮没有为 SplitButton 引入独立模板改造。

## 验证

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --no-restore
```

结果：通过。

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-button-states
```

结果：通过。覆盖 icon presenter 按需创建/移除、loading 切换、re-template、wave 按需创建/移除、旧实例 visual parent 清理、wave property 注册名。

## 原始结果

- Date: 2026-05-14 21:15:48 +08:00
- Configuration: Debug
- Suite: `button`
- Count per scenario: 60
- Runner: `tools/performances/AtomUI.Performance`

| Scenario | Count | Total ms | ms/item | KB/item | Visual/root | Logical/root | ContentPresenter/root | Button/root | TextBlock/root | Panel/root | Border/root | DockPanel/root | Icon/root | IconPresenter/root | ButtonIconPresenter/root | PathIcon/root | StackPanel/root | WaveSpiritDecorator/root | DashedBorder/root | ButtonLoadingHost/root | AddOnDecoratedBox/root | Icon status calls | Icon brush calls | Icon scan visuals | Icon matches |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Button.Default.Text | 60 | 77.92 | 1.299 | 209.9 | 8.0 | 2.0 | 1.0 | 1.0 | 1.0 | 2.0 | 2.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Default.NoWave | 60 | 70.27 | 1.171 | 200.3 | 7.0 | 2.0 | 1.0 | 1.0 | 1.0 | 2.0 | 2.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Primary.Text | 60 | 80.10 | 1.335 | 210.7 | 8.0 | 2.0 | 1.0 | 1.0 | 1.0 | 2.0 | 2.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Dashed.Text | 60 | 105.20 | 1.753 | 211.3 | 8.0 | 2.0 | 1.0 | 1.0 | 1.0 | 2.0 | 1.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 1.0 | 1.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Text.Text | 60 | 40.51 | 0.675 | 198.6 | 7.0 | 2.0 | 1.0 | 1.0 | 1.0 | 2.0 | 2.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Link.Text | 60 | 40.42 | 0.674 | 200.2 | 7.0 | 2.0 | 1.0 | 1.0 | 1.0 | 2.0 | 2.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Default.IconOnly | 60 | 57.81 | 0.963 | 297.3 | 10.0 | 1.0 | 1.0 | 1.0 | 0.0 | 2.0 | 3.0 | 1.0 | 1.0 | 1.0 | 1.0 | 1.0 | 0.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Default.IconText | 60 | 58.01 | 0.967 | 308.9 | 11.0 | 2.0 | 1.0 | 1.0 | 1.0 | 2.0 | 3.0 | 1.0 | 1.0 | 1.0 | 1.0 | 1.0 | 0.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Primary.Loading | 60 | 59.32 | 0.989 | 316.4 | 9.0 | 2.0 | 1.0 | 1.0 | 1.0 | 2.0 | 3.0 | 1.0 | 1.0 | 0.0 | 0.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Primary.IconLoading | 60 | 47.96 | 0.799 | 306.3 | 8.0 | 1.0 | 1.0 | 1.0 | 0.0 | 2.0 | 3.0 | 1.0 | 1.0 | 0.0 | 0.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Primary.GhostDangerIcon | 60 | 53.81 | 0.897 | 308.6 | 11.0 | 2.0 | 1.0 | 1.0 | 1.0 | 2.0 | 3.0 | 1.0 | 1.0 | 1.0 | 1.0 | 1.0 | 0.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Shape.CircleText | 60 | 36.59 | 0.610 | 211.0 | 8.0 | 2.0 | 1.0 | 1.0 | 1.0 | 2.0 | 2.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0.0 | 0.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Shape.RoundIconText | 60 | 51.23 | 0.854 | 310.1 | 11.0 | 2.0 | 1.0 | 1.0 | 1.0 | 2.0 | 3.0 | 1.0 | 1.0 | 1.0 | 1.0 | 1.0 | 0.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Dropdown.Default | 60 | 58.18 | 0.970 | 316.6 | 11.0 | 2.0 | 1.0 | 1.0 | 1.0 | 2.0 | 3.0 | 1.0 | 1.0 | 1.0 | 0.0 | 1.0 | 0.0 | 1.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Split.Primary | 60 | 128.53 | 2.142 | 716.9 | 23.0 | 1.0 | 2.0 | 2.0 | 1.0 | 5.0 | 6.0 | 3.0 | 2.0 | 2.0 | 2.0 | 2.0 | 0.0 | 2.0 | 0.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |
| Button.Mixed.Batch10 | 60 | 528.26 | 8.804 | 2615.6 | 91.0 | 20.0 | 10.0 | 10.0 | 9.0 | 21.0 | 24.0 | 10.0 | 5.0 | 4.0 | 4.0 | 5.0 | 0.0 | 7.0 | 1.0 | 0.0 | 0.0 | 0 | 0 | 0 | 0 |

Notes:

- `Visual/root` and `Logical/root` include the scenario root control itself.
- The suite measures materialization, template application and layout in headless mode; it does not isolate GPU/platform render cost.
- Icon probe data is Debug-only and records `AddOnDecoratedBox.UpdateIconStatusColors()` plus `ApplyIconBrush()` scans.
- Binding expression count is not directly measured yet; current baseline uses node counts, allocation, timing, and AddOnDecoratedBox probe counters.
- This measures control-level template/style/materialization cost, not Gallery navigation.
