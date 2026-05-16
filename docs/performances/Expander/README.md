# Expander 性能优化记录

## 目标

Expander 本轮优化遵循“未使用功能不承担成本”：默认关闭态不应创建内容 motion actor、内容 presenter；没有 addon 时不应创建 addon presenter；`IsShowExpandIcon=False` 时不应创建按钮和默认图标。所有按需创建对象必须有释放路径，不能引入事件、binding、visual parent 泄露。

## 优化内容

- `PART_ExpandButton` 从模板移到代码按需创建，`IsShowExpandIcon=False` 时不创建按钮、不创建默认 `RightOutlined`。
- `PART_AddOnContentPresenter` 仅在 `AddOnContent` 或 `AddOnContentTemplate` 存在时创建，清空后移除并清理 `Content`、`ContentTemplate`、`TemplatedParent`。
- `PART_ContentMotionActor` 和 `PART_ContentPresenter` 改为首次展开或初始 `IsExpanded=True` 时创建；关闭态默认不创建内容树。
- 替换匿名 `Click` 事件为方法组，并在 re-template、detach、隐藏箭头时解绑。
- 代码创建的 Content binding 使用 `CompositeDisposable` 保存，re-template/detach 时统一释放。
- 修复 `IsBorderless`/`IsGhostStyle`/`BorderThickness` 的有效边框厚度同步路径，避免旧的 `else if` 分支让 `IsBorderless` 更新不可达。
- `TriggerType=Icon` 时不再允许 header 区域点击触发展开；点击 icon 仍然触发。

## 控件级结果

测试命令：

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite expander --count 80
```

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| Closed.Basic time | 2.254 ms/item | 1.253 ms/item | 44.41% faster |
| Closed.Basic alloc | 232.5 KB/item | 185.1 KB/item | 20.39% less |
| Closed.Basic visuals | 15/root | 13/root | 13.33% less |
| Closed.NoArrow time | 1.666 ms/item | 0.714 ms/item | 57.14% faster |
| Closed.NoArrow alloc | 165.6 KB/item | 96.4 KB/item | 41.79% less |
| Closed.NoArrow visuals | 11/root | 8/root | 27.27% less |
| Closed.WithAddOn time | 2.849 ms/item | 1.517 ms/item | 46.75% faster |
| Closed.WithAddOn alloc | 263.1 KB/item | 223.4 KB/item | 15.09% less |
| Expanded.Basic time | 7.471 ms/item | 2.659 ms/item | 64.41% faster |
| Expanded.Basic alloc | 367.4 KB/item | 244.9 KB/item | 33.34% less |
| Direction.LeftClosed time | 2.345 ms/item | 1.982 ms/item | 15.48% faster |
| GalleryShape time | 28.054 ms/item | 18.916 ms/item | 32.57% faster |
| GalleryShape alloc | 4397.6 KB/item | 3594.3 KB/item | 18.27% less |
| GalleryShape visuals | 277/root | 246/root | 11.19% less |

结构变化：

| Metric | Before | After |
| --- | ---: | ---: |
| Closed.Basic content motion actor | 1/root | 0/root |
| Closed.NoArrow IconButton | 1/root | 0/root |
| Closed.NoArrow default Icon/PathIcon | 1/root | 0/root |
| GalleryShape ContentPresenter | 34/root | 19/root |
| GalleryShape IconButton | 16/root | 15/root |
| GalleryShape motion actor | 16/root | 1/root |

## ExpanderShowCase 加载结果

测试命令：

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase expander --iterations 20 --warmup 3 --cold-iterations 10
```

测试口径：Debug + Avalonia headless，`1300x900`，从 AboutUs route 稳定后导航到真实 `ExpanderShowCase`，等待 visual tree 和 layout 稳定。冷启动为 10 个独立进程样本，重复导航为 warmup 3 + measured 20。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 191.70 ms | 170.47 ms | 11.07% faster |
| Cold first navigation median | 188.86 ms | 166.36 ms | 11.91% faster |
| Cold first navigation P95 | 211.01 ms | 188.02 ms | 10.90% faster |
| Cold alloc mean | 7740.78 KB | 6802.82 KB | 12.12% less |
| Repeated navigation mean | 82.18 ms | 67.30 ms | 18.11% faster |
| Repeated navigation median | 83.17 ms | 55.91 ms | 32.78% faster |
| Repeated navigation P95 | 113.85 ms | 100.58 ms | 11.66% faster |
| Repeated alloc mean | 6786.71 KB | 5995.03 KB | 11.67% less |
| Visual nodes | 391 | 360 | 7.93% less |
| MotionActor nodes | 16 | 1 | 93.75% less |
| IconButton nodes | 16 | 15 | 6.25% less |

## 验证

```bash
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj --framework net10.0 --no-restore
dotnet run --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-expander-states
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
```

`--verify-expander-states` 覆盖关闭态懒创建、NoArrow 生命周期、addon 生命周期、内容 actor 复用与 detach 释放、边框状态同步，以及动态创建 PART 后 theme padding 与自定义 padding 不丢失。
