# Form 性能优化记录

日期：2026-05-17

`Form` 是 Data Entry 里的复合容器控件。它自身负责 `FormItem`、布局、校验、feedback、提交/重置和动态增删 item，但真实 `FormShowCase` 同时包含 `LineEdit`、`Select`、`Cascader`、`TreeSelect`、`DatePicker`、`RangeDatePicker`、`TextArea`、`CheckBox`、Button/Icon 等子控件。因此本轮优化把 Form 自身可控成本、子控件固定成本和 Gallery 页面级耗时分开记录。

## 结论

本轮完成了 Form 的低风险结构、生命周期和观测优化：

- 新增 Form 控件级性能套件和状态验证。
- `GalleryPerformance` 支持 `--showcase form`，可以复现真实 `FormShowCase` 加载。
- 修复 `FormItem` 内容替换、feedback、校验 debounce timer、window breakpoint 订阅状态等生命周期风险。
- 无 Tooltip 的 `FormItem` 不再默认创建 `QuestionCircleOutlined`。
- `IsValidateFeedbackEnabled` 变化会同步到现有 items。
- 去掉 `Debug.Assert(...)` 后直接使用 nullable 对象的写法。

未继续把 delete button、custom mark、error/help 区域大规模搬出模板。原因是这类改动会明显增加模板/代码协作复杂度，并且当前页面级耗时没有证明稳定大幅改善；继续做会更容易引入 UI/动画/行为回归，不符合“性能优化不能增加实现复杂度、不能改变 UI 行为”的边界。

## 测试口径

控件级：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -- --suite form --count 60 --markdown /tmp/atomui-form-control-final.md
```

状态验证：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -- --verify-form-states
```

真实 Gallery：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -- --showcase form --warmup 6 --iterations 40 --cold-iterations 10 \
  --timeout-ms 45000 --label form-final \
  --markdown /tmp/atomui-form-gallery-final.md
```

环境：Debug / net10.0 / Avalonia Headless / 1300x900。

## 控件级结果

主对比场景是 `Form.GalleryShape`，它尽量贴近 `FormShowCase` 的控件构成，但不包含 Gallery route、ShowCaseItem 容器和页面级调度成本。

| 指标 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| `Form.GalleryShape` 创建耗时 | 92.094ms/item | 89.364ms/item | 快 2.96% |
| 分配 | 24487.8KB/item | 23604.3KB/item | 少 3.61% |
| Visual nodes | 1544/root | 1508/root | 少 36/root |
| Icon/PathIcon | 50/root | 14/root | 少 72.00% |

控件级收益是明确的，但不是数量级变化。主要收益来自无 Tooltip 时不创建默认 tooltip icon，以及生命周期路径减少无效对象和悬挂回调。

## FormShowCase 真实加载结果

`FormShowCase.axaml` 静态源形态：

| Form | FormItem | FormActionsItem | ShowCaseItem | LineEdit | Select | Cascader | TreeSelect | DatePicker | RangeDatePicker | TextArea | CheckBox |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 21 | 118 | 19 | 20 | 47 | 8 | 5 | 4 | 7 | 6 | 4 | 11 |

运行时结构：

| 指标 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| Visual nodes | 6435 | 6300 | 少 135 |
| Logical nodes | 216 | 216 | 持平 |
| Icon / PathIcon | 215 / 215 | 80 / 80 | 少 135 / 135 |
| AddOnDecoratedBox | 92 | 92 | 持平 |
| Repeated alloc | 104419.21KB | 100722.49KB | 少 3.54% |

页面加载时间：

| 指标 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 2473.27ms | 1964.20ms | 快 20.58% |
| Cold first navigation median | 2104.07ms | 1971.32ms | 快 6.31% |
| Cold first navigation P95 | 4088.79ms | 2076.31ms | 快 49.22% |
| Repeated mean | 705.09ms | 700.96ms | 快 0.59% |
| Repeated median | 695.50ms | 680.68ms | 快 2.13% |
| Repeated P95 | 761.10ms | 817.23ms | 慢 7.37% |

解释：

- Cold mean/P95 的 before 样本里有一次明显异常慢样本，所以只能看作“冷启动没有变差”的参考，不能宣称稳定 20%+ 提升。
- Repeated mean/median 基本持平，小幅变好；P95 变差，说明页面级耗时仍受子控件、GC、调度和 Gallery 容器影响。
- 本轮可确定的收益是结构和分配下降，不是 `FormShowCase` 打开耗时的大幅提升。

## 已实施 Phase

### Phase 0：基线与观测

已完成：

- 新增 `tools/performances/AtomUI.Performance/Suites/Form/FormScenarios.cs`。
- 新增 `tools/performances/AtomUI.Performance/Suites/Form/FormStateVerification.cs`。
- `TreeStats`/Markdown 输出新增 Form、FormItem、FormValidateFeedback、Submit/Reset/Delete/Tooltip 等统计。
- `AtomUI.GalleryPerformance` 支持 `--showcase form`。

### Phase 1：生命周期与泄露修复

已完成：

- `FormItem.Content` 替换时解绑旧 content 的 `ValueChanged`。
- 旧 `IFormItemFeedbackAware` content 会清空 feedback control。
- feedback rebuild 前释放旧 `_feedbackDisposable`。
- `ValidateDebounce` 的 `DispatcherTimer.RunOnce(...)` 返回值已保存并在重新校验或 detach 时释放。
- detach 时清理 pending validation token/timer。
- window breakpoint 订阅 detach 后清空状态。

验证覆盖：

- content replacement 后旧 content 不再触发 Form value changed。
- feedback enable/disable/re-enable 正常。
- debounce detach 后没有 pending timer 和 CTS。
- decorator child replacement 后旧 child 不再触发 ValueChanged。

### Phase 2：FormItem 热路径收敛

已评估：

- `LayoutUpdated` 写 property 前比较旧值的实验没有带来稳定收益，且页面级耗时没有改善，已放弃。
- `ConfigureLayout()` 只做了局部变量收敛，没有引入布局状态缓存，避免把布局逻辑复杂化。

结论：这一阶段不继续加状态机。

### Phase 3：可选区按需创建

已完成：

- 无 `Tooltip` 且无自定义 `TooltipIcon` 时，不再创建默认 `QuestionCircleOutlined`。
- 设置 `Tooltip` 时按需创建默认图标。
- 清空 `Tooltip` 时释放默认图标。
- 用户自定义 `TooltipIcon` 不被覆盖。

未实施：

- delete button、custom required/optional mark、extra/help/error 区域未继续从模板搬到代码中。当前收益不足以覆盖 UI 回归风险。

### Phase 4：Form -> FormItem 配置传播收敛

已完成：

- `IsValidateFeedbackEnabled` 变化纳入 `SyncConfigToItems()`，保证运行时切换 feedback 时现有 item 同步。
- `Reset()` 使用 method-group dispatcher callback，避免额外 lambda。
- `Items.CollectionChanged` 使用方法组，便于审查生命周期。

未继续做：

- 没有重写 `PrepareContainerForItemOverride()` 与 `SyncConfigToItems()` 的职责边界。这里涉及 styled property priority，收益不明确，误改容易复现 Space 之前的 binding priority 问题。

### Phase 5：FormItem -> Content 转发审查

已审查：

- `FormItem.Content` 和 `FormItemDecorator.Child` 都支持 replacement path，因此继续保留可释放 binding plumbing，而不是盲目改 `[!]`。
- replacement 验证已覆盖旧 child/content 的事件解绑。

结论：这里不适合为了减少一点 disposable 而放弃 replacement cleanup。

### Phase 6：校验路径收敛

已完成：

- 删除 `Debug.Assert(x != null)` 后直接使用 nullable 的模式，改为明确 runtime guard。
- 校验开始前检查 cancellation。
- validator 循环中检查 cancellation，避免旧校验继续改 UI 状态。
- debounce timer 现在可取消。

### Phase 7：Gallery 分层判断

已完成初步判断：

- `FormShowCase` 的 6300 个 visual 里，Form 自身只是放大器。
- 页面仍包含 47 个 LineEdit、8 个 Select、5 个 Cascader、4 个 TreeSelect、7 个 DatePicker、6 个 RangeDatePicker、180 个 IconButton。
- 继续压页面打开时间，优先级应放在这些子控件和 Gallery 示例拆分上，而不是继续把 Form 模板复杂化。

### Phase 8：最终对比

本轮最终结论：

- 控件级 `Form.GalleryShape`：耗时快 2.96%，分配少 3.61%，visual 少 36/root。
- 真实 `FormShowCase`：visual 少 135，分配少 3.54%，repeated mean/median 基本持平，P95 未改善。
- 这不是一次页面打开时间的大幅优化，但清除了 Form 自身可控的无效 tooltip icon 成本和若干生命周期风险。

## 后续建议

如果继续压 `FormShowCase`，建议不要继续在 Form 容器里堆复杂逻辑，而是做两件事：

- 分组拆测 `Select`、`Cascader`、`TreeSelect`、`DatePicker`、`RangeDatePicker` 在 Form 中的组合成本。
- 评估 Gallery 示例是否一次性展示过多复杂场景；若展示目的允许，考虑懒加载折叠区或按 demo 分页。
