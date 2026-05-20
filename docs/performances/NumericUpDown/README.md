# NumericUpDown 性能优化

`NumericUpDown` 是 Data Entry 高频控件，内部依赖 `ButtonSpinner`、`AddOnDecoratedBox`、`TextBox` 和 `IconButton`。本轮优化目标是让默认不使用的 clear/accessory 能力不承担固定模板成本，同时收敛 `ButtonSpinner` floatable handle 的全局输入订阅成本。

真实 Gallery 对应页面是 `NumberUpDownShowCase`。当前页面包含 `30` 个 `NumericUpDown` 运行时实例，实际运行形态为 `30` 个 `ButtonSpinner`、`30` 个 `AddOnDecoratedBox` 和 `13` 个 `ShowCaseItem`。

## Phase 0：基线与观测

新增：

- 控件级套件：`tools/performances/AtomUI.Performance --suite numericupdown`
- Gallery 真实场景：`tools/performances/AtomUI.GalleryPerformance --showcase numberupdown`
- 状态验证：`--verify-numericupdown-states`

优化前关键数据，Debug headless，控件级 `--count 80`，Gallery `1300x900`、cold 5 samples、warmup 3、measured 12：

| 场景 | 指标 | 优化前 |
| --- | --- | ---: |
| `NumericUpDown.Default` | ms/item | `2.831` |
| `NumericUpDown.Default` | KB/item | `739.7` |
| `NumericUpDown.Default` | Visual/root | `39` |
| `NumericUpDown.Default` | IconButton/root | `3` |
| `NumericUpDown.GalleryShape.Batch30` | ms/item | `86.948` |
| `NumericUpDown.GalleryShape.Batch30` | KB/item | `23434.9` |
| `NumericUpDown.GalleryShape.Batch30` | Visual/root | `1217` |
| `NumberUpDownShowCase` cold mean | ms | `480.45` |
| `NumberUpDownShowCase` repeated mean | ms | `186.43` |
| `NumberUpDownShowCase` visuals | count | `1351` |

## 实施内容

- 从 `NumericUpDownTheme.axaml` 中移除默认固定创建的 `ButtonSpinner.InnerRightContent`、`InputClearIconButton` 和 `PART_InnerRightContentPresenter`。
- `NumericUpDown` 改为仅在 `IsEffectiveShowClearButton=true` 或 `InnerRightContent != null` 时创建右侧 accessory host。
- clear button 按需创建，点击事件使用方法组订阅；销毁时解绑事件、清理 icon/motion 属性、移除 visual parent 并清理 templated parent。
- inner right presenter 按需创建；内容清空时清理 `Content`、`ContentTemplate`、visual parent 和 templated parent。
- accessory host 的 token spacing binding 在 host 销毁时 dispose。
- `TextBox.IsCustomFontSize` 改用 `[!]` 同生命周期绑定；替换 text box 时清理旧绑定。
- `NumericUpDownTextConverter` 改为 StringMode 首次使用时创建，默认数字模式不承担 converter 实例成本。
- `ICompactSpaceAware.GetBorderThickness()` 不再强制触发 layout pass。
- `ButtonSpinnerDecoratedBox` 的 floatable hover 跟踪不再在 attached 后常驻全局 `IInputManager.Process` 订阅；改为 pointer 进入当前控件时临时订阅全局坐标，离开、禁用、隐藏 handle 或 detach 时立即释放，避免 TextBox/overlay/addon 组合下打开和隐藏状态漂移。
- `ButtonSpinnerDecoratedBox` 的 `PART_OverlayLayout` 开启裁剪，避免 floatable handle 在外部 `RightAddOn` 存在时绘制到 addon 区域。
- 清理 `NumberUpDownShowCase.axaml.cs` 中空的 `WhenActivated` 代码和无效 `ReactiveUI` using。

## Phase 记录

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 0 | 建立控件级和真实 Gallery 基线，补充 NumberUpDown Gallery route | Done |
| Phase 1 | NumericUpDown 默认右侧 clear/accessory 模板成本按需化 | Done |
| Phase 2 | clear button 和 inner right presenter 生命周期与释放验证 | Done |
| Phase 3 | StringMode converter、TextBox 内部绑定和 CompactSpace layout pass 收敛 | Done |
| Phase 4 | ButtonSpinner floatable handle 常驻全局输入订阅移除，活动期临时跟踪并释放 | Done |
| Phase 5 | 状态验证覆盖 clear/custom icon/content/string mode/keyboard/ButtonSpinner subscription | Done |
| Phase 6 | 控件级与真实 Gallery 复测、文档沉淀 | Done |

## 最终结果

控件级 suite，Debug headless，`--suite numericupdown --count 80`：

| 场景 | 指标 | 优化前 | 优化后 | 变化 |
| --- | --- | ---: | ---: | ---: |
| `NumericUpDown.Default` | ms/item | `2.831` | `2.970` | 慢 `4.91%` |
| `NumericUpDown.Default` | KB/item | `739.7` | `698.6` | 少 `5.56%` |
| `NumericUpDown.Default` | Visual/root | `39` | `36` | 少 `3` |
| `NumericUpDown.Default` | IconButton/root | `3` | `2` | 少 `1` |
| `NumericUpDown.AllowClear.Empty` | KB/item | `805.2` | `789.3` | 少 `1.97%` |
| `NumericUpDown.AllowClear.Value` | ms/item | `2.543` | `2.182` | 快 `14.19%` |
| `NumericUpDown.GalleryShape.Batch30` | ms/item | `86.948` | `73.072` | 快 `15.96%` |
| `NumericUpDown.GalleryShape.Batch30` | KB/item | `23434.9` | `22237.7` | 少 `5.11%` |
| `NumericUpDown.GalleryShape.Batch30` | Visual/root | `1217` | `1135` | 少 `82` |
| `NumericUpDown.GalleryShape.Batch30` | Button/IconButton | `90 / 90` | `60 / 60` | 各少 `30` |
| `NumericUpDown.GalleryShape.Batch30` | ContentPresenter | `154` | `128` | 少 `26` |
| `NumericUpDown.GalleryShape.Batch30` | StackPanel | `31` | `5` | 少 `26` |
| `NumericUpDown.GalleryShape.Batch30` | Icon brush scanned | `11280` | `2320` | 少 `79.43%` |

说明：单个默认控件的 `ms/item` 在 `count 80` 样本里有噪声，表中保留了这次原始结果；随后用 `--count 160` 复测，`NumericUpDown.Default` 为 `2.828ms/item`、`699.1KB/item`、`36` visuals，没有形成可重复回退。后续不能把默认 `ms/item` 当作主要收益，只能说明隐藏 accessory 的结构和分配成本确实被移除了。

真实 `NumberUpDownShowCase`，Debug headless，`1300x900`，cold 5 samples，warmup 3，measured 12：

| 指标 | 优化前 | 优化后 | 变化 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | `480.45ms` | `427.73ms` | 快 `10.97%` |
| Cold first navigation median | `456.45ms` | `428.84ms` | 快 `6.05%` |
| Cold first navigation P95 | `558.71ms` | `440.73ms` | 快 `21.12%` |
| Cold alloc mean | `28186.58KB` | `26803.75KB` | 少 `4.91%` |
| Repeated navigation mean | `186.43ms` | `179.03ms` | 快 `3.97%` |
| Repeated navigation median | `168.57ms` | `161.84ms` | 快 `3.99%` |
| Repeated navigation P95 | `257.28ms` | `247.40ms` | 快 `3.84%` |
| Repeated alloc mean | `26217.32KB` | `24990.82KB` | 少 `4.68%` |
| Runtime visuals | `1351` | `1269` | 少 `82` |
| Runtime Button/IconButton | `90 / 90` | `60 / 60` | 各少 `30` |

结论：本轮符合“不使用的功能不承担成本”的目标，真实 `NumberUpDownShowCase` 加载时间、visual 数量和分配均下降。页面级 repeated timing 只提升约 `4%`，说明 NumericUpDown 的剩余成本仍有较大部分来自 `ButtonSpinner`、`AddOnDecoratedBox`、`TextBox` 和 Gallery `ShowCaseItem` 固定成本，不适合继续用复杂代码换小幅收益。

## 生命周期与正确性覆盖

- 默认 `NumericUpDown` 不创建 `PART_ClearButton` 和 `PART_InnerRightContentPresenter`。
- `IsAllowClear=true` 且有文本时创建 clear button；点击后清空 `Value` 并释放 clear button。
- `ClearIcon` 自定义和清空后默认图标恢复均通过验证。
- `InnerRightContent` 设置后创建 presenter，清空后释放 presenter 且旧 presenter 无 visual parent。
- `StringMode` 保留原始字符串，并能同步可解析的 `StringValue -> Value`。
- `IsKeyboardEnabled=false` 状态保持。
- `ButtonSpinner` 验证覆盖 idle 状态无 pointer tracking subscription，以及 handle/presenter/addon lifecycle。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
```

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-numericupdown-states --verify-buttonspinner-states
```

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite numericupdown --count 80
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
```

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase numberupdown --warmup 3 --iterations 12 --cold-iterations 5 --timeout-ms 20000
```
