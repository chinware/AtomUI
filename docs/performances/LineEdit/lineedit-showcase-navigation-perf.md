# LineEditShowCase 导航性能对比

日期：2026-05-14

## 测量目标

量化从 Gallery Workspace 已稳定显示 `AboutUs` 页面后，触发左侧菜单对应的 `LineEdit` 导航，到 `LineEditShowCase` 进入视觉树并完成稳定布局的耗时。

本次测量加载的是 Gallery 真实页面 `AtomUIGallery.ShowCases.Views.LineEditShowCase`，源文件为 `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/LineEditShowCase.axaml`。工具会解析该 XAML 并同时输出运行时视觉树计数，确保测量对象和 Gallery 中实际控件例子一致。

Headless 环境下 `LineEdit` 菜单项没有实例化为可见 `NavMenuItem`，因此测量触发点落在 `CaseNavigationViewModel.NavigateToCommand.Execute(LineEditViewModel.ID)`。这等价于左侧菜单点击处理器拿到 `ItemKey` 后的主体路径，包含 route/view 创建、XAML 加载、模板应用和布局稳定；不包含鼠标命中测试、菜单项事件派发和 GPU 实际上屏。

## 环境与样本

- 工具：`tools/performances/AtomUI.GalleryPerformance`
- 配置：Debug, net10.0, Avalonia Headless
- 窗口：1300 x 900
- 样本：冷首开 1 次；warmup 5 次；重复导航 30 次
- Baseline：临时 worktree `/tmp/AtomUIV6-lineedit-baseline`，commit `78bce1f39`
- Optimized：当前工作区优化后代码
- 逐次 sample 原始输出不入库；需要时使用复现命令重新生成到 `/tmp`。

## 结论

冷首开为单次 fresh process 样本，波动会比 30 次重复导航更大；重复导航统计更适合判断控件实例化和布局成本的稳定改善。

| 指标 | 未优化 | 优化后 | 改善 |
| --- | ---: | ---: | ---: |
| 冷首开耗时 | 830.00 ms | 732.75 ms | -97.25 ms / -11.72% |
| 重复导航均值 | 229.07 ms | 183.98 ms | -45.09 ms / -19.68% |
| 重复导航中位数 | 223.15 ms | 182.87 ms | -40.28 ms / -18.05% |
| 重复导航 P95 | 263.45 ms | 207.98 ms | -55.47 ms / -21.05% |
| 冷首开分配 | 73,913.03 KB | 64,880.65 KB | -9,032.38 KB / -12.22% |
| 重复导航平均分配 | 69,075.57 KB | 60,508.11 KB | -8,567.46 KB / -12.40% |
| Visual 数量 | 2,967 | 2,553 | -414 / -13.95% |

## Gallery 页面一致性校验

| 来源 | LineEdit direct | SearchEdit | LineEdit total | TextArea | ShowCaseItem |
| --- | ---: | ---: | ---: | ---: | ---: |
| `LineEditShowCase.axaml` | 52 | 30 | 82 | 11 | 16 |
| 未优化运行时视觉树 | 52 | 30 | 82 | 11 | 16 |
| 优化后运行时视觉树 | 52 | 30 | 82 | 11 | 16 |

## 解释

优化是有效的：`LineEditShowCase` 冷首开减少约 97ms；重复导航均值减少约 45ms。视觉树少了 414 个节点，分配减少约 8.4 MB 到 8.8 MB。

这也说明之前的 AddOnDecoratedBox 隐形成本确实会放大到 Gallery 级页面打开时间上。当前优化没有减少 `LineEditShowCase` 中真实例子数量：仍然是 52 个 direct `LineEdit`、30 个 `SearchEdit`、11 个 `TextArea`、16 个 `ShowCaseItem`，运行时仍然是 93 个 `AddOnDecoratedBox`；改善来自每个实例的模板和 accessory 成本降低。

## 复现命令

优化后：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --label optimized --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/lineedit-showcase-navigation-optimized.md
```

未优化：

```bash
git worktree add /tmp/AtomUIV6-lineedit-baseline HEAD
mkdir -p /tmp/AtomUIV6-lineedit-baseline/tools/performances
cp -R tools/performances/AtomUI.GalleryPerformance /tmp/AtomUIV6-lineedit-baseline/tools/performances/
# 如果 baseline 的 Directory.Packages.props 尚未声明 Avalonia.Headless，需要临时补充同版本 PackageVersion。
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --label baseline --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/lineedit-showcase-navigation-baseline.md
```
