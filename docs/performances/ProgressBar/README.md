# ProgressBar 性能优化

> 状态：已完成页面级主优化；本轮追加 CircleProgress / LineProgress structural-only 收口。

既有主收益来自 status icon 按需创建：`ProgressBarShowCase` repeated mean `154.17ms -> 111.26ms`，runtime visuals `1389 -> 977`。本轮不重新声明页面 timing 收益，只记录 CircleProgress / LineProgress 计算与分配路径的结构性下降。

## 本轮追加：CircleProgress 尺寸计算收敛

`AbstractCircleProgress` 的尺寸阈值是固定常量，旧实现仍为每个实例创建一个 `Dictionary<SizeType, double>(3)`，并在 `OnApplyTemplate` 时追加三档阈值。现在改为 `switch` 常量路径；同时 width/height、size-type、extra-info visibility 路径复用同一次 `CalculateCircleSize()` 结果，避免连续重复计算。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| CircleProgress size threshold dictionary / instance | 1 dictionary | 0 dictionaries | `(1 - 0) / 1` | 100.00% | 结构收益；固定阈值不再分配字典 |
| CircleProgress threshold dictionary adds / template apply | 3 adds | 0 adds | `(3 - 0) / 3` | 100.00% | 结构收益；不再按实例填充固定阈值 |
| CircleProgress threshold dictionary lookups / effective size calculation | 2 lookups | 0 lookups | `(2 - 0) / 2` | 100.00% | 结构收益；直接比较 large/middle 常量 |
| CircleProgress circle-size calculations / width-height update | 3 calls | 1 call | `(3 - 1) / 3` | 66.67% | 结构收益；stroke/font/icon size 共用同一尺寸快照 |
| CircleProgress circle-size calculations / size-type update | 3 calls | 1 call | `(3 - 1) / 3` | 66.67% | 结构收益；`NotifyEffectSizeTypeChanged` 不再重复走 base + 两个 setup |
| CircleProgress circle-size calculations / extra-info visibility update | 2 calls | 1 call | `(2 - 1) / 2` | 50.00% | 结构收益；stroke thickness 使用已计算尺寸 |

说明：本轮没有改模板、padding、status icon 创建策略或视觉层级，不声明新的页面级 timing 提升。

## 本轮追加：LineProgress 阈值表字段化

`AbstractGeneralProgressBar` / `AbstractGeneralStepsProgressBar` 的 large / middle / small 阈值在 template apply 后只需要按当前 token 和方向保存三档数值。旧路径每个线形进度实例都会创建一个 `Dictionary<SizeType, SizeTypeThresholdValue>(3)`，template apply 时再追加 3 个 class entry，之后每次有效尺寸判断还要做 2 次字典查找。

本轮改为 `SizeTypeThresholdValue` value-type 字段：large / middle / small 三档直接赋值，计算时直接读字段。重复 template apply 也不再依赖 `Dictionary.Add()`，避免同 key 追加的冲突风险。没有改模板、padding、stroke 规则或 status icon 逻辑。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| LineProgress size threshold dictionary / instance | 1 dictionary | 0 dictionaries | `(1 - 0) / 1` | 100.00% | 结构收益；三档阈值直接存字段 |
| LineProgress threshold entry objects / template apply | 3 class objects | 0 class objects | `(3 - 0) / 3` | 100.00% | 结构收益；`SizeTypeThresholdValue` 改为 value-type 字段 |
| LineProgress dictionary adds / template apply | 3 adds | 0 adds | `(3 - 0) / 3` | 100.00% | 结构收益；重复套模板不再走同 key Add |
| LineProgress dictionary lookups / effective size calculation | 2 lookups | 0 lookups | `(2 - 0) / 2` | 100.00% | 结构收益；直接比较 middle/small 或 large/middle 字段 |

## 验证

```
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-restore
→ Build succeeded. 0 Warning(s) 0 Error(s)
```

`--verify-progressbar-states` 当前仍失败在既有伪类、status icon lifecycle 和 inner label 断言上；这些失败覆盖 Line ProgressBar 等路径，不作为本轮 CircleProgress structural-only 收益证明。
