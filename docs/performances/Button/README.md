# Button 性能优化

`Button` 是 AtomUI 最基础、使用范围最广的控件之一。它既作为独立按钮使用，也被 `DropdownButton`、`SplitButton`、Form、Input accessory、Select/Menu 等更高层控件间接放大。

当前已完成 Phase 3-7：loading、icon presenter、wave decorator 均已按需创建，并完成真实 `ButtonShowCase` Gallery 复测。

阶段中间跑分、一次性计划和 smoke 原始输出不单独入库；长期结论、风险边界和最终数据已汇总到本 README。

## 当前结论

`Button` 的默认 slot 隐形成本已完成第一轮结构减重：

- 非 loading Button 不创建 loading host / loading icon。
- 无 icon Button 不创建 `IconPresenter` 和三路 `MultiBinding`。
- `Text` / `Link` / loading / no-wave Button 不创建 `WaveSpiritDecorator`。
- `DropdownButton` 已同步复用 Button 的 icon/wave 按需创建路径。
- `DropdownButton.OnApplyTemplate()` 不再创建未使用的空 `Border`。
- selector 颜色状态本轮保留在 XAML 中；现阶段没有足够数据证明继续迁移到代码值得承担主题维护风险。

## 最终结果

数据来自 Debug headless，主要用于判断结构性成本和阶段收益。

| 场景 | 优化前 | 优化后 | 改善 |
| --- | ---: | ---: | ---: |
| `ButtonShowCase` repeated mean | `134.60ms` | `101.53ms` | `24.57%` |
| `ButtonShowCase` alloc mean | `32177.02KB` | `26653.55KB` | `17.17%` |
| `ButtonShowCase` visuals | `1128` | `964` | `-164` |
| `ButtonShowCase` `IconPresenter` | `92` | `51` | `-41` |
| `Button.Default.Text` `Visual/root` | `10` | `8` | `-2` |
| `Button.Default.Text` `KB/item` | `298.8` | `209.9` | `29.75%` |

联动 smoke 结果已汇总，不保留独立原始文档：

| 场景 | Phase 7 smoke |
| --- | --- |
| `DropdownButtonShowCase` | mean `54.91ms`，alloc mean `6940.50KB`，`Visuals=227`，`Button=17`，`IconPresenter=17` |
| `SplitButtonShowCase` | mean `74.90ms`，alloc mean `9693.20KB`，`Visuals=318`，`Button=26`，`IconPresenter=13` |

## 验证覆盖

最终验证覆盖：

- runtime 设置 `Icon` 后 `:icononly` 与 presenter 创建正确。
- runtime 切换 `ButtonType` / `IsDanger` 后伪类同步正确。
- 非 loading Button 不创建 loading slot，loading 反复切换和 re-template 后旧实例无 visual parent 残留。
- 无 icon Button 不创建 `PART_ButtonIcon`；loading 时移除 normal icon，关闭 loading 后重新创建。
- `Text` / `Link` / loading / no-wave Button 不创建 `PART_WaveSpirit`。
- `Default` / `Primary` / `Dashed` 在启用 wave 且非 loading 时创建 `PART_WaveSpirit`。
- `WaveSpiritDecorator` 动画取消路径会清理 CTS 和 `_isPlaying` 状态。

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-button-states
```
