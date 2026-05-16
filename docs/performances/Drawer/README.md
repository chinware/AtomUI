# Drawer 性能优化记录

## 结论

`Drawer` 关闭态 visual 成本本来就低，本轮没有大改 UI 模板结构，重点处理隐藏生命周期成本和资源泄露风险：

- 关闭态不再为默认 `OpenOn` 创建 ancestor binding。
- `OpenOn.SizeChanged` 只在 `IsOpen && DialogSize.IsPercentage` 时订阅，关闭和 detach 都会解除。
- 打开后的 `DrawerContainer` 保留用于二次打开复用，detach 时统一释放 visual parent、事件订阅、binding/content 引用。
- open/close 异步动画使用版本号收敛，避免旧回调在 close/reopen/detach 后继续通知或操作已移除 visual。
- 多层 Drawer 打开后切换 `Placement` 时，父层 push transform 会跟随 child Drawer 的最终位置合并更新，并在 child 关闭或 detach 时恢复，避免旧方向位移造成闪烁或残留。
- Phase 5 的 mask/close/extra/footer 代码按需创建方案已评估并撤回：对真实 `DrawerShowCase` 关闭态加载没有收益，还引入过回归风险。当前保持 XAML 模板行为，后续只在打开路径专项数据证明收益时再做。

## 测试策略

- 日期：2026-05-16
- 配置：Debug / Avalonia Headless / net10.0
- 控件级：`tools/performances/AtomUI.Performance --suite drawer --count 1000`
- Gallery：`tools/performances/AtomUI.GalleryPerformance --showcase drawer --cold-iterations 10 --warmup 30 --iterations 60`
- Gallery 触发点：从 `AboutUs` settled 后触发 `DrawerShowCase` route，等待 visual tree 与 layout 稳定。
- 对比方式：baseline 使用临时 worktree 的产品代码，加上同一版性能工具；after 使用当前产品代码和同一版性能工具。

## 控件级结果

关闭态 root visual/logical 数仍为 `1/1`，没有提前创建 `DrawerContainer`、`DrawerInfoContainer`、mask、motion actor 或 close button。

| 场景 | Before | After | 提升 |
| --- | ---: | ---: | ---: |
| `Drawer.Closed.Basic` time | 0.075ms/item | 0.064ms/item | 快 14.67% |
| `Drawer.Closed.Basic` alloc | 20.8KB/item | 18.5KB/item | 少 11.06% |
| `Drawer.Closed.NoMask` time | 0.071ms/item | 0.060ms/item | 快 15.49% |
| `Drawer.Closed.NoMask` alloc | 20.5KB/item | 18.3KB/item | 少 10.73% |
| `Drawer.Closed.ExtraFooter` time | 0.165ms/item | 0.140ms/item | 快 15.15% |
| `Drawer.Closed.ExtraFooter` alloc | 37.1KB/item | 35.1KB/item | 少 5.39% |
| `Drawer.Closed.Nested` time | 0.155ms/item | 0.089ms/item | 快 42.58% |
| `Drawer.Closed.Nested` alloc | 49.5KB/item | 47.4KB/item | 少 4.24% |

## 真实 Gallery 结果

`DrawerShowCase.axaml` 源码中有 8 个 `Drawer`、7 个 `ShowCaseItem`、10 个 `Button`。页面稳定后运行时结构为 259 visual、65 logical、7 个运行时 `Drawer`；少于源码的 1 个 Drawer 是嵌套示例的子 Drawer 位于未打开父 Drawer 内容中，未进入 visual tree。

| 指标 | Before | After | 提升 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 189.50ms | 184.00ms | 快 2.90% |
| Cold first navigation median | 191.12ms | 182.84ms | 快 4.33% |
| Cold first navigation P95 | 198.83ms | 192.21ms | 快 3.33% |
| Cold first navigation alloc | 6409.14KB | 6361.65KB | 少 0.74% |
| Repeated navigation mean | 24.24ms | 22.87ms | 快 5.65% |
| Repeated navigation median | 23.66ms | 22.87ms | 快 3.34% |
| Repeated navigation P95 | 27.60ms | 24.21ms | 快 12.28% |
| Repeated navigation alloc | 4891.41KB | 4866.79KB | 少 0.50% |
| Visuals | 259 | 259 | 持平 |
| Logical | 65 | 65 | 持平 |
| Drawer runtime count | 7 | 7 | 持平 |

这个结果符合 Drawer 当前结构的预期：关闭态 visual 本来就很少，因此 Gallery 页面级耗时不会出现大比例下降；真正收益在隐藏订阅、detach 清理、open/close 竞态和关闭态分配上。

## 已完成优化

### Phase 0：基线与观测

已建立控件级关闭态基线和真实 `DrawerShowCase` 导航基线。结论是 Drawer 本体不是 Gallery 冷启动主瓶颈，但存在关闭态默认 `OpenOn` binding、`SizeChanged` 订阅和 detach 清理风险。

### Phase 1：生命周期与泄露修复

- 增加 `_openOnSizeChangedTarget`，统一管理 `OpenOn.SizeChanged` 订阅。
- `OnDetachedFromVisualTree()` 释放 size subscription、relay binding、token binding 和 materialized container。
- `DrawerContainer.Release()` 解除 `DrawerInfoContainer.CloseRequested`，移出 adorner layer，清除 logical parent 和内容引用。

### Phase 2：关闭态 OpenOn 与尺寸计算按需化

- 移除默认 `OpenOn` ancestor binding。
- 打开时通过 `OpenOn ?? TopLevel.GetTopLevel(this)` 解析目标。
- 百分比尺寸只在打开且需要时监听目标尺寸变化。

### Phase 3：open/close 状态机收敛

- `DrawerContainer` 使用 `_operationVersion` 使旧的 open/close async callback 自动失效。
- `EnsureLayerParent()` 避免重复加入 adorner layer 或跨父级 reparent 异常。
- `RemoveFromLayer()` 同时清 visual parent 和 logical parent。

### Phase 4：容器保留策略

采用“首次打开后保留，detach 时释放”。这符合 Popup lazy content 的策略：避免频繁 open/close 重建，同时保证页面卸载后没有 visual/event/content 引用残留。

### Phase 5：模板减重评估

已评估并撤回。将 mask、close button、extra/footer 改为代码按需创建后，真实 Gallery 加载没有稳定收益，并且增加了 UI 行为和模板状态同步风险。本轮不为了微小或不可证明收益牺牲模板稳定性。

### Phase 6：工具与验证补强

- `AtomUI.Performance` 增加 `drawer` suite。
- `AtomUI.GalleryPerformance` 增加 `--showcase drawer` 和运行时 `Drawer` 统计。
- 增加 `--verify-drawer-states`，覆盖关闭态 lazy、open/close/reopen、detach release、percentage size subscription、mask/close/extra/footer 状态同步。
- 补充多层 Drawer 位置切换验证：child Drawer 打开时从 Right 切 Left 再切回 Right，父层 push transform 必须按最终 `Placement` 更新，child 关闭后必须恢复。

## 验证命令

```bash
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj --framework net10.0 --no-restore
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-drawer-states
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite drawer --count 1000 --markdown /tmp/drawer-control-final-seq.md
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase drawer --cold-iterations 10 --warmup 30 --iterations 60 --timeout-ms 30000 --markdown /tmp/drawer-showcase-final-rerun.md
git diff --check
```
