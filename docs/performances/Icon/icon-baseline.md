# Icon 性能基线

本文件记录 Icon 优化前的可复现基线。逐次 sample 原始输出不入库，只保留关键汇总、复现命令和解释。

## 环境

- 日期：2026-05-14
- 配置：Debug / `net10.0`
- 控件级工具：`tools/performances/AtomUI.Performance`
- Gallery 工具：`tools/performances/AtomUI.GalleryPerformance`
- Gallery 口径：Headless，1300x900 window，从 AboutUs route 稳定后触发真实 Gallery route，再等待目标 showcase visual tree 与 layout 稳定。

## 控件级基线

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite icon --count 60 \
  --markdown /tmp/icon-micro-baseline.md
```

结果：

| Scenario | Count | Total ms | ms/item | KB/item | Visual/root | Logical/root | Icon/root | IconPresenter/root | PathIcon/root | AddOnDecoratedBox/root |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Icon.SearchOutlined.Direct | 60 | 9.37 | 0.156 | 47.0 | 2.0 | 1.0 | 1.0 | 0.0 | 1.0 | 0.0 |
| Icon.SearchOutlined.Presenter | 60 | 15.06 | 0.251 | 65.5 | 3.0 | 2.0 | 1.0 | 1.0 | 1.0 | 0.0 |
| Icon.SearchOutlined.Many10 | 60 | 121.84 | 2.031 | 476.3 | 21.0 | 11.0 | 10.0 | 0.0 | 10.0 | 0.0 |
| Icon.LoadingOutlined.Spin | 60 | 14.81 | 0.247 | 58.0 | 2.0 | 1.0 | 1.0 | 0.0 | 1.0 | 0.0 |
| Icon.TwoTone.Bulb | 60 | 11.62 | 0.194 | 49.0 | 2.0 | 1.0 | 1.0 | 0.0 | 1.0 | 0.0 |
| Icon.Provider.SearchOutlined | 60 | 10.93 | 0.182 | 48.0 | 2.0 | 1.0 | 1.0 | 0.0 | 1.0 | 0.0 |
| Icon.HiddenSlots.SelectDefault | 60 | 212.87 | 3.548 | 594.7 | 27.0 | 1.0 | 3.0 | 2.0 | 3.0 | 1.0 |
| Icon.HiddenSlots.MenuItemLeaf | 60 | 78.77 | 1.313 | 239.4 | 13.0 | 1.0 | 1.0 | 1.0 | 1.0 | 0.0 |

观察：

- 直接 `Icon` 的 `Visual/root=2`，说明自绘 Icon 之外仍有一个模板视觉节点。
- `IconPresenter + Icon` 比 direct Icon 多 1 个 visual 和 1 个 logical 节点，符合包装成本预期。
- `Many10` 基本线性放大：10 个图标为 `21` visuals，说明每个 direct Icon 约 2 个 visuals。
- `Select.Default` 默认创建 3 个 Icon / 2 个 IconPresenter，并触发 AddOnDecoratedBox 的 icon 状态扫描；这是典型隐藏 slot 成本。
- `MenuItemLeaf` 即使是叶子项也会创建 1 个 indicator IconPresenter/Icon，属于后续高频模板按需创建候选。

## Gallery IconShowCase 基线

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase icon --label icon-baseline \
  --iterations 10 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/icon-showcase-navigation-baseline.md
```

源 XAML：

| Source | AntDesignIconProvider | IconPresenter | IconGallery | LineEdit direct | SearchEdit | LineEdit total | TextArea | ShowCaseItem |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `controlgallery/AtomUIGallery/ShowCases/Views/General/IconShowCase.axaml` | 0 | 0 | 3 | 0 | 0 | 0 | 0 | 0 |

结果：

| Set | Trigger | Mean ms | Median ms | P95 ms | Min ms | Max ms | Alloc KB mean | Visuals | Logical | Icon | IconPresenter | PathIcon | IconGallery | IconInfoItem |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | NavigateToCommand | 756.28 | 756.28 | 756.28 | 756.28 | 756.28 | 66222.01 | 3712 | 11 | 456 | 459 | 456 | 1 | 456 |
| Repeated navigation | NavigateToCommand | 193.31 | 189.73 | 217.25 | 170.31 | 222.16 | 57900.83 | 3712 | 11 | 456 | 459 | 456 | 1 | 456 |

解释：

- `IconShowCase.axaml` 有 3 个 `IconGallery`，但运行时稳定后只有 1 个 `IconGallery` 物化；当前 TabControl 只物化选中 tab 内容。
- 当前选中 gallery 物化 456 个 `IconInfoItem`，对应 456 个 `Icon`、459 个 `IconPresenter`。
- Repeated navigation 均值 `193.31ms`，分配约 `57.9MB`，说明 Icon 批量场景主要成本在控件/模板/数据项物化，而不是单个 provider 调用。

## 兼容包基线约束

本次同步阅读了两个外部图标包：

- `/Users/chinboy/Projects/dotnet/IconParkIconsPackage`
- `/Users/chinboy/Projects/dotnet/MaterialIconsPackages`

需要作为后续优化约束：

- Material 当前生成约 `10732` 个图标类，每个主题是独立类，直接继承 `Icon`。
- IconPark 当前生成约 `2659` 个图标类，所有主题复用同一个图标类，通过 `IconParkIcon.FindIconBrush()` 根据 `IconTheme` 动态映射 brush。
- IconPark 依赖 `StrokeBrush`、`FillBrush`、`SecondaryStrokeBrush`、`SecondaryFillBrush`、`FallbackBrush`、`StrokeWidth`、`StrokeLineCap`、`StrokeLineJoin` 和 `ProcessBrush()`。
- Material 主要依赖 `FillBrush` / `StrokeBrush` 的 theme 映射，但仍使用 `IconTheme` 表达 generated class 的主题类型。
- 因此，Icon 基类优化不能删除多色、stroke、theme switch、brush processing、animation 等能力，只能把它们改成按需付费。

## 当前结论

Icon 当前性能不是单点慢，而是高频场景里节点、binding、transition、pen、render 分配和隐藏 slot 叠加。

Phase 1 应优先做低风险修复：

- 修正 `strokeIndex` 正确性问题。
- 避免静态单色 Icon 在初始化阶段承担完整 pen / transition 成本。
- 避免 `IconPresenter` 重复配置同一个 icon。
- 修复 `IconTemplatePresenter` 的 disposable 与 parent 清理边界。

Phase 2 再处理 render 热路径，尤其是停止在每次 render 中创建 `MatrixTransform` 并修改共享 geometry。
