# DatePicker 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 2 / TimePicker shared presenter path
> 状态：本轮追加 structural-only 生命周期收敛；不声明新的页面 timing 收益。

---

## 本轮结论

本轮只优化打开态 presenter 事件生命周期，不改变 DatePicker / RangeDatePicker 的模板视觉、padding、popup shell 或 Calendar 行为。

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| `DatePickerPresenter` re-template retained handlers / presenter | 14 | 0 | `(14 - 0) / 14` | 100.00% | 旧 `CalendarView` / `TimeView` / 三个按钮的事件在重新套模板前可完整解绑 |
| `DatePickerPresenter` anonymous pointer handler callsites | 6 | 0 | `(6 - 0) / 6` | 100.00% | Today / Now / Confirm enter/exit 改为 method-group handler |
| attached presenter choosing-state stale subscription risk | 1 | 0 | `(1 - 0) / 1` | 100.00% | `OnApplyTemplate()` 后如果已经 attached，会刷新 Calendar / TimeView observable 订阅 |

说明：这是生命周期与结构收益。`dotnet run -- --suite datepicker --count 30` 只作为 smoke 检查，未作为页面速度提升证明。

## 验证

```bash
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 -- --verify-datepicker-states --verify-timepicker-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 -- --suite datepicker --count 30
```

结果：

- `AtomUI.Desktop.Controls` build 通过，0 warning / 0 error。
- `DatePicker state verification passed.`
- `TimePicker state verification passed.`
- `DatePicker` suite smoke 通过，visual/logical 形态保持稳定。
