# Input 性能优化记录

日期：2026-05-17

## 结论

`Input` 本轮聚焦 `TextBox` 与 `TextArea` 的默认关闭态成本：不使用 clear、reveal、count、right addon、resize handle 时，不再提前创建对应 visual、icon、button 和 presenter。`LineEdit`/`SearchEdit` 已在前序阶段基于 `LineEditAccessoryHost` 做过大量优化，本轮只验证它们在真实 `LineEditShowCase` 中没有因为 Input 底层调整产生可重复退化。

本轮完成的低风险优化：

- `TextBoxTheme.axaml` 去掉默认创建的左右 accessory layout、clear button、reveal button、feedback presenter、right content presenter 和 count text。
- `TextBox` 改为按实际属性创建 accessory visual，并在关闭、清空、re-template 时移除 child、清理 content/icon、解绑事件。
- `InputClearIconButton` 负责自己的默认 `CloseCircleFilled`，避免默认无 clear 的 `TextBox` 也创建 clear icon。
- `TextAreaTheme.axaml` 去掉默认 `TextCountIndicator` 和 `ResizeHandle`，由 `TextArea` 按 `IsShowCount`/`IsResizable` 创建和释放。
- `TextArea` 的行高计算加入缓存，避免相同 font/line/spacing 下重复创建 `TextLayout`。
- `CountText` 在 `Text`、`IsShowCount` 和 `MaxLength` 改变时保持同步。

未保留的改动：

- `SearchEditDecoratedBox` 模板减重曾尝试把静态 presenter 改为运行时创建，但真实 `LineEditShowCase` repeated 导航变慢，已回退。当前 `SearchEdit` 保持原有静态 presenter 结构，本轮不声明 SearchEdit 独立收益。

## 测试口径

控件级：

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -- --suite input --count 30 \
  --markdown /tmp/atomui-input-control-current.md
```

状态与生命周期验证：

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -- --verify-accessories --verify-input-states
```

真实 Gallery：

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -- --showcase lineedit --warmup 5 --iterations 30 --cold-iterations 10 \
  --timeout-ms 45000 --label input-current \
  --markdown /tmp/atomui-input-lineedit-gallery-current.md
```

环境：Debug / net10.0 / Avalonia Headless / Gallery 1300x900。

## 控件级结果

同口径确认样本：baseline `/tmp/atomui-input-control-baseline-confirm.md`，current `/tmp/atomui-input-control-current-final-confirm.md`。单个场景毫秒在 Debug/headless 下仍有抖动，结构和分配是本轮主要验收指标。

| 场景 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| `TextBox.Default` ms/item | 0.915ms | 0.603ms | 快 34.10% |
| `TextBox.Default` KB/item | 332.6KB | 236.7KB | 少 28.83% |
| `TextBox.Default` visual/root | 21 | 13 | 少 8 |
| `TextBox.AllowClear.Empty` ms/item | 0.748ms | 0.548ms | 快 26.74% |
| `TextBox.AllowClear.Empty` KB/item | 330.2KB | 239.5KB | 少 27.47% |
| `TextBox.AllowClear.Empty` visual/root | 21 | 13 | 少 8 |
| `TextBox.AllowClear.Text` ms/item | 1.061ms | 0.834ms | 快 21.39% |
| `TextBox.AllowClear.Text` KB/item | 396.3KB | 325.7KB | 少 17.81% |
| `TextBox.AllowClear.Text` visual/root | 25 | 19 | 少 6 |
| `TextBox.Reveal` ms/item | 1.241ms | 0.942ms | 快 24.09% |
| `TextBox.Reveal` KB/item | 414.5KB | 341.6KB | 少 17.59% |
| `TextBox.Reveal` visual/root | 26 | 20 | 少 6 |
| `TextBox.Count` ms/item | 0.927ms | 0.733ms | 快 20.93% |
| `TextBox.Count` KB/item | 337.1KB | 258.5KB | 少 23.32% |
| `TextBox.Count` visual/root | 21 | 15 | 少 6 |
| `TextArea.Default` ms/item | 0.949ms | 0.860ms | 快 9.38% |
| `TextArea.Default` KB/item | 422.7KB | 394.1KB | 少 6.77% |
| `TextArea.Default` visual/root | 24 | 21 | 少 3 |
| `TextArea.AllowClear` KB/item | 520.1KB | 492.6KB | 少 5.29% |
| `TextArea.AllowClear` visual/root | 30 | 27 | 少 3 |

合成 `Input.GalleryShape` 结构变化：

| 指标 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| Visual/root | 2158 | 2139 | 少 19 |
| TextBlock/root | 148 | 143 | 少 5 |
| Border/root | 241 | 234 | 少 7 |
| KB/item | 45847.2KB | 45686.7KB | 少 160.5KB / 0.35% |
| ms/item | 176.779ms | 180.913ms | 慢 2.34%，不作为收益声明 |

解释：`Input.GalleryShape` 是一个 93 个输入控件的大批量合成场景，Debug/headless 的 timing 对调度噪声很敏感。本轮保留结构和分配下降；页面级收益用真实 `LineEditShowCase` 判断。

## LineEditShowCase 真实加载结果

真实 `LineEditShowCase.axaml` 形态：52 个直接 `LineEdit`、30 个 `SearchEdit`、11 个 `TextArea`、16 个 `ShowCaseItem`，运行时 `AddOnDecoratedBox=93`。样本策略一致：`cold-iterations=10`、`warmup=5`、`iterations=30`，并做两轮 same-time rerun 合并观察 repeated 指标。

| 指标 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 796.70ms | 860.48ms | 慢 8.00%，冷启动抖动偏大，不声明提升 |
| Cold first navigation median | 784.67ms | 804.87ms | 慢 2.57% |
| Cold first navigation P95 | 902.93ms | 1119.53ms | 慢 23.99%，受尾部样本影响 |
| Repeated mean | 282.35ms | 281.69ms | 快 0.23% |
| Repeated median | 279.45ms | 274.47ms | 快 1.78% |
| Repeated P95 | 308.71ms | 318.31ms | 慢 3.11%，尾部仍有抖动 |
| Repeated alloc mean | 约 50550KB | 约 50280KB | 少约 270KB |
| Visual nodes | 2307 | 2284 | 少 23 |
| TextBlock | 220 | 213 | 少 7 |

解释：

- 本轮真实 Gallery 的可确认收益是结构和分配下降；repeated mean/median 基本持平并略好。
- 冷启动 timing 未证明改善，主要仍由 route、XAML load、ShowCaseItem、`AddOnDecoratedBox`、`SearchEdit` 和布局稳定成本构成。
- aggressive `SearchEdit` 模板减重已回退，因为它会让真实 repeated 导航变慢。

## 已实施 Phase

### Phase 0：基线与观测

已完成：

- 新增 `tools/performances/AtomUI.Performance/Suites/Input/InputScenarios.cs`。
- 覆盖 `TextBox`、`LineEdit`、`SearchEdit`、`TextArea` 和 `Input.GalleryShape`。
- 使用 `AtomUI.GalleryPerformance --showcase lineedit` 复现实 Gallery 页面。

### Phase 1：TextBox accessory 按需创建

已完成：

- 默认 `TextBox` 不再创建左右 addon layout、clear/reveal button、feedback presenter、right presenter、count text。
- 属性打开时创建；关闭或内容清空时释放。
- re-template 前解绑 click/reveal observable，移除旧 child，清理 `TemplatedParent`。

### Phase 2：ClearIcon 默认成本下沉

已完成：

- 默认 clear icon 从 `TextBox.OnInitialized()` 下沉到 `InputClearIconButton.SyncIcon(null)`。
- 未启用 clear 的输入框不再创建默认 `CloseCircleFilled`。
- 验证覆盖默认 icon、自定义 icon、清空回默认 icon。

### Phase 3：TextArea optional visual 按需创建

已完成：

- `TextArea` 默认不创建 count indicator 和 resize handle。
- `IsShowCount`/`IsResizable` 开启时创建，关闭或 re-template 时释放。
- count indicator 保留原 selector 样式，resize handle 保留原 `Owner` 与 cursor 行为。

### Phase 4：TextArea 行高计算缓存

已完成：

- `CalculateScrollViewerHeight()` 缓存相同 lines/font/line-height/features/vertical-space 的结果。
- 不改变 autosize、min/max lines 和 resize 语义。

### Phase 5：SearchEdit 模板减重评估

已完成并回退：

- 评估过 SearchEdit 静态 presenter 按需化。
- 因真实 `LineEditShowCase` repeated 导航退化，回退模板改动，只保留 clear icon 同步修复。

### Phase 6：状态、生命周期与泄露验证

已完成：

- `--verify-input-states` 覆盖 TextBox/TextArea clear/reveal/count/left/right/resize 的创建、移除和 parent 清理。
- 覆盖 `MaxLength` 改变后 count text 同步。
- `--verify-accessories` 继续覆盖共享 accessory lifecycle。

### Phase 7：最终对比

本轮结论：

- `TextBox` 默认路径收益明确，默认 visual/root `21 -> 13`，KB/item `332.6 -> 236.7`。
- `TextArea` 默认路径收益明确，默认 visual/root `24 -> 21`，KB/item `422.7 -> 394.1`。
- 真实 `LineEditShowCase` 结构少 23 个 visual，分配少约 270KB；repeated mean/median 基本持平略好，冷启动未证明提升。
- 后续如果继续压 `LineEditShowCase` 冷启动，应转向 `SearchEdit` 自身、`AddOnDecoratedBox` 剩余固定成本、ShowCaseItem/XAML load，而不是继续增加 `TextBox` lazy 状态复杂度。
