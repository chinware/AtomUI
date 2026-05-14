# ButtonShowCase navigation performance - button-phase0-baseline

- Timestamp: 2026-05-14 19:57:23 +08:00
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
| Cold first navigation | NavigateToCommand | 357.01 | 357.01 | 357.01 | 357.01 | 357.01 | 34101.13 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| Repeated navigation | NavigateToCommand | 134.60 | 136.20 | 155.73 | 113.90 | 157.06 | 32177.02 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |

## Samples

| Iteration | Phase | Trigger | Elapsed ms | Alloc KB | Visuals | Logical | Icon | IconPresenter | PathIcon | LineEdit total | LineEdit direct | SearchEdit | TextArea | Button | ToggleIconButton | Select | Menu | MenuItem | NavMenuHeader | ShowCaseItem | IconGallery | IconInfoItem | AddOnDecoratedBox |
| ---: | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 0 | Cold | NavigateToCommand | 357.01 | 34101.13 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 1 | Measured | NavigateToCommand | 149.13 | 32185.15 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 2 | Measured | NavigateToCommand | 157.06 | 32176.45 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 3 | Measured | NavigateToCommand | 154.10 | 32176.45 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 4 | Measured | NavigateToCommand | 141.57 | 32176.71 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 5 | Measured | NavigateToCommand | 135.47 | 32186.02 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 6 | Measured | NavigateToCommand | 136.93 | 32176.45 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 7 | Measured | NavigateToCommand | 124.07 | 32176.45 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 8 | Measured | NavigateToCommand | 113.90 | 32176.17 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 9 | Measured | NavigateToCommand | 118.99 | 32172.16 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
| 10 | Measured | NavigateToCommand | 114.80 | 32168.16 | 1128 | 218 | 52 | 92 | 52 | 0 | 0 | 0 | 0 | 89 | 0 | 0 | 0 | 0 | 0 | 10 | 0 | 0 | 0 |
