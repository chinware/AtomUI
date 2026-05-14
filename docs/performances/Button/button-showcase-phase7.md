# ButtonShowCase navigation performance - button-phase7

## 与 Phase 0 / Phase 2 对比

| 指标 | Phase 0 | Phase 2 | Phase 7 | Phase 0 -> Phase 7 |
| --- | ---: | ---: | ---: | ---: |
| repeated mean | 134.60ms | 107.82ms | 101.53ms | -24.57% |
| repeated median | 136.20ms | 110.89ms | 102.87ms | -24.47% |
| repeated p95 | 155.73ms | 124.01ms | 116.51ms | -25.19% |
| repeated alloc mean | 32177.02KB | 31587.43KB | 26653.55KB | -17.17% |
| runtime visuals | 1128 | 1039 | 964 | -164 |
| runtime Button | 89 | 89 | 89 | 0 |
| runtime Icon | 52 | 52 | 51 | -1 |
| runtime IconPresenter | 92 | 92 | 51 | -41 |
| runtime PathIcon | 52 | 52 | 51 | -1 |

结论：

- `ButtonShowCase` 使用真实 Gallery XAML，source 中仍是 89 个 `Button` 和 49 个 `AntDesignIconProvider`。
- Phase 7 后 `IconPresenter 92 -> 51`，说明无 icon Button 的固定 `PART_ButtonIcon` 成本已移除。
- Phase 7 后 `Visuals 1128 -> 964`，比 Phase 2 再少 75 个 visual；收益来自 icon presenter 按需创建、Text/Link/loading/no-wave 的 wave 按需创建，以及 DropdownButton 同步减重。
- repeated mean 从 Phase 0 的 `134.60ms` 降到 `101.53ms`，提升约 `24.57%`。未达到 50%，主要因为 ShowCase 容器、布局、文本、路由和仍实际使用的 icon/wave 成本仍在。

- Timestamp: 2026-05-14 21:16:18 +08:00
- Configuration: Debug, headless, 1300x900 window
- Measurement: AboutUs route settled -> trigger ButtonShowCase navigation -> visual tree and layout stable
- Route type: `AtomUIGallery.ShowCases.Views.ButtonShowCase`
- XAML source: `/Users/chinboy/Projects/dotnet/AtomUIV6/controlgallery/AtomUIGallery/ShowCases/Views/General/ButtonShowCase.axaml`
- Warmup: 3, measured iterations: 10, timeout: 15s

## Gallery source shape

| Source | AntDesignIconProvider | IconPresenter | IconGallery | LineEdit direct | SearchEdit | LineEdit total | TextArea | Button | ToggleIconButton | Select | Menu | MenuItem | ShowCaseItem |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `/Users/chinboy/Projects/dotnet/AtomUIV6/controlgallery/AtomUIGallery/ShowCases/Views/General/ButtonShowCase.axaml` | 49 | 0 | 0 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 9 |

| Set | Trigger | Mean ms | Median ms | P95 ms | Min ms | Max ms | Alloc KB mean | Visuals | Logical | Icon | IconPresenter | PathIcon | LineEdit total | LineEdit direct | SearchEdit | TextArea | Button | ToggleIconButton | Select | Menu | MenuItem | NavMenuHeader | ShowCaseItem | IconGallery | IconInfoItem | AddOnDecoratedBox |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | NavigateToCommand | 316.17 | 316.17 | 316.17 | 316.17 | 316.17 | 28563.40 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| Repeated navigation | NavigateToCommand | 101.53 | 102.87 | 116.51 | 86.08 | 120.82 | 26653.55 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |

## Samples

| Iteration | Phase | Trigger | Elapsed ms | Alloc KB | Visuals | Logical | Icon | IconPresenter | PathIcon | LineEdit total | LineEdit direct | SearchEdit | TextArea | Button | ToggleIconButton | Select | Menu | MenuItem | NavMenuHeader | ShowCaseItem | IconGallery | IconInfoItem | AddOnDecoratedBox |
| ---: | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 0 | Cold | NavigateToCommand | 316.17 | 28563.40 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 1 | Measured | NavigateToCommand | 120.82 | 26654.43 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 2 | Measured | NavigateToCommand | 104.40 | 26653.24 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 3 | Measured | NavigateToCommand | 105.20 | 26653.24 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 4 | Measured | NavigateToCommand | 101.87 | 26653.50 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 5 | Measured | NavigateToCommand | 103.87 | 26654.07 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 6 | Measured | NavigateToCommand | 111.23 | 26653.24 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 7 | Measured | NavigateToCommand | 97.94 | 26653.24 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 8 | Measured | NavigateToCommand | 94.42 | 26653.24 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 9 | Measured | NavigateToCommand | 89.51 | 26654.07 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 10 | Measured | NavigateToCommand | 86.08 | 26653.24 | 964 | 218 | 51 | 51 | 51 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
