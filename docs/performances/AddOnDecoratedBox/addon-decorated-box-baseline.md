# AddOnDecoratedBox / LineEdit Baseline

- Date: 2026-05-14 08:20:45 +08:00
- Configuration: Debug
- Count per scenario: 60
- Runner: `tools/performances/AtomUI.Performance`

| Scenario | Count | Total ms | ms/item | KB/item | Visual/root | Logical/root | ContentPresenter/root | Button/root | TextBlock/root | Icon/root | StackPanel/root | AddOnDecoratedBox/root | Icon status calls | Icon brush calls | Icon scan visuals | Icon matches |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| LineEdit.Default | 60 | 171.32 | 2.855 | 503.8 | 26.0 | 1.0 | 7.0 | 2.0 | 2.0 | 0.0 | 1.0 | 1.0 | 180 | 480 | 360 | 0 |
| LineEdit.AllowClear | 60 | 258.79 | 4.313 | 598.9 | 30.0 | 1.0 | 7.0 | 2.0 | 2.0 | 1.0 | 1.0 | 1.0 | 180 | 480 | 360 | 0 |
| LineEdit.Reveal | 60 | 263.61 | 4.393 | 654.6 | 33.0 | 1.0 | 7.0 | 2.0 | 2.0 | 2.0 | 1.0 | 1.0 | 180 | 480 | 360 | 0 |
| LineEdit.Count | 60 | 211.37 | 3.523 | 504.3 | 26.0 | 1.0 | 7.0 | 2.0 | 2.0 | 0.0 | 1.0 | 1.0 | 180 | 480 | 360 | 0 |
| LineEdit.InnerRight | 60 | 147.68 | 2.461 | 510.4 | 27.0 | 1.0 | 7.0 | 2.0 | 3.0 | 0.0 | 1.0 | 1.0 | 180 | 480 | 360 | 0 |
| LineEdit.FormFeedback | 60 | 109.46 | 1.824 | 527.9 | 29.0 | 1.0 | 8.0 | 2.0 | 3.0 | 0.0 | 1.0 | 1.0 | 180 | 480 | 360 | 0 |
| LineEdit.OuterAddOns | 60 | 103.32 | 1.722 | 526.4 | 28.0 | 1.0 | 7.0 | 2.0 | 4.0 | 0.0 | 1.0 | 1.0 | 300 | 960 | 660 | 0 |
| SearchEdit.Default | 60 | 204.37 | 3.406 | 982.4 | 34.0 | 1.0 | 6.0 | 3.0 | 1.0 | 2.0 | 1.0 | 1.0 | 180 | 480 | 840 | 120 |
| DatePicker.Default | 60 | 163.98 | 2.733 | 801.3 | 43.0 | 1.0 | 10.0 | 3.0 | 2.0 | 1.0 | 4.0 | 1.0 | 180 | 480 | 180 | 0 |
| RangeDatePicker.Default | 60 | 242.09 | 4.035 | 1254.4 | 69.0 | 1.0 | 13.0 | 5.0 | 4.0 | 2.0 | 6.0 | 1.0 | 180 | 480 | 180 | 0 |
| Select.Default | 60 | 117.00 | 1.950 | 764.1 | 32.0 | 1.0 | 8.0 | 1.0 | 1.0 | 3.0 | 2.0 | 1.0 | 180 | 480 | 240 | 0 |
| TreeSelect.Default | 60 | 131.79 | 2.196 | 762.5 | 32.0 | 1.0 | 8.0 | 1.0 | 1.0 | 3.0 | 2.0 | 1.0 | 180 | 480 | 240 | 0 |
| Cascader.Default | 60 | 116.12 | 1.935 | 764.3 | 32.0 | 1.0 | 7.0 | 1.0 | 2.0 | 3.0 | 2.0 | 1.0 | 180 | 480 | 240 | 0 |
| CompactSpace.LineEdit.Horizontal | 60 | 317.82 | 5.297 | 1663.4 | 87.0 | 1.0 | 21.0 | 6.0 | 6.0 | 1.0 | 3.0 | 3.0 | 540 | 1440 | 1080 | 0 |
| CompactSpace.LineEdit.Vertical | 60 | 265.41 | 4.424 | 1655.3 | 87.0 | 1.0 | 21.0 | 6.0 | 6.0 | 1.0 | 3.0 | 3.0 | 540 | 1440 | 1080 | 0 |

Notes:

- `Visual/root` and `Logical/root` include the scenario root control itself.
- CompactSpace scenarios use three `LineEdit` children per root.
- Icon probe data is Debug-only and records `AddOnDecoratedBox.UpdateIconStatusColors()` plus `ApplyIconBrush()` scans.
- Binding expression count is not directly measured yet; current baseline uses node counts, allocation, timing, and AddOnDecoratedBox probe counters.
- This measures control-level template/style/materialization cost, not Gallery navigation.
