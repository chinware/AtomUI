# AddOnDecoratedBox / LineEdit Final Result

- Date: 2026-05-14 09:42:54 +08:00
- Configuration: Debug
- Count per scenario: 60
- Runner: `tools/performances/AtomUI.Performance`

| Scenario | Count | Total ms | ms/item | KB/item | Visual/root | Logical/root | ContentPresenter/root | Button/root | TextBlock/root | Icon/root | StackPanel/root | AddOnDecoratedBox/root | Icon status calls | Icon brush calls | Icon scan visuals | Icon matches |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| LineEdit.Default | 60 | 133.40 | 2.223 | 398.4 | 20.0 | 1.0 | 5.0 | 0.0 | 1.0 | 0.0 | 0.0 | 1.0 | 60 | 0 | 0 | 0 |
| LineEdit.AllowClear | 60 | 245.04 | 4.084 | 522.4 | 26.0 | 1.0 | 5.0 | 1.0 | 1.0 | 1.0 | 1.0 | 1.0 | 180 | 120 | 120 | 0 |
| LineEdit.Reveal | 60 | 260.91 | 4.348 | 573.1 | 29.0 | 1.0 | 5.0 | 1.0 | 1.0 | 2.0 | 1.0 | 1.0 | 180 | 120 | 120 | 0 |
| LineEdit.Count | 60 | 215.70 | 3.595 | 419.6 | 22.0 | 1.0 | 5.0 | 0.0 | 2.0 | 0.0 | 1.0 | 1.0 | 180 | 120 | 120 | 0 |
| LineEdit.InnerRight | 60 | 177.54 | 2.959 | 422.0 | 23.0 | 1.0 | 6.0 | 0.0 | 2.0 | 0.0 | 1.0 | 1.0 | 180 | 120 | 120 | 0 |
| LineEdit.FormFeedback | 60 | 143.85 | 2.398 | 440.3 | 25.0 | 1.0 | 7.0 | 0.0 | 2.0 | 0.0 | 1.0 | 1.0 | 180 | 120 | 120 | 0 |
| LineEdit.OuterAddOns | 60 | 142.40 | 2.373 | 420.5 | 22.0 | 1.0 | 5.0 | 0.0 | 3.0 | 0.0 | 0.0 | 1.0 | 180 | 360 | 180 | 0 |
| SearchEdit.Default | 60 | 273.15 | 4.553 | 896.4 | 30.0 | 1.0 | 5.0 | 1.0 | 1.0 | 2.0 | 0.0 | 1.0 | 60 | 60 | 0 | 0 |
| DatePicker.Default | 60 | 204.12 | 3.402 | 779.7 | 43.0 | 1.0 | 10.0 | 3.0 | 2.0 | 1.0 | 4.0 | 1.0 | 180 | 120 | 180 | 0 |
| RangeDatePicker.Default | 60 | 312.11 | 5.202 | 1229.0 | 69.0 | 1.0 | 13.0 | 5.0 | 4.0 | 2.0 | 6.0 | 1.0 | 180 | 120 | 180 | 0 |
| Select.Default | 60 | 135.01 | 2.250 | 685.3 | 32.0 | 1.0 | 8.0 | 1.0 | 1.0 | 3.0 | 2.0 | 1.0 | 180 | 120 | 240 | 0 |
| TreeSelect.Default | 60 | 128.24 | 2.137 | 685.0 | 32.0 | 1.0 | 8.0 | 1.0 | 1.0 | 3.0 | 2.0 | 1.0 | 180 | 120 | 240 | 0 |
| Cascader.Default | 60 | 137.69 | 2.295 | 684.6 | 32.0 | 1.0 | 7.0 | 1.0 | 2.0 | 3.0 | 2.0 | 1.0 | 180 | 120 | 240 | 0 |
| CompactSpace.LineEdit.Horizontal | 60 | 315.57 | 5.260 | 1376.6 | 73.0 | 1.0 | 15.0 | 1.0 | 4.0 | 1.0 | 2.0 | 3.0 | 420 | 240 | 240 | 0 |
| CompactSpace.LineEdit.Vertical | 60 | 242.33 | 4.039 | 1385.5 | 73.0 | 1.0 | 15.0 | 1.0 | 4.0 | 1.0 | 2.0 | 3.0 | 420 | 240 | 240 | 0 |

Notes:

- `Visual/root` and `Logical/root` include the scenario root control itself.
- CompactSpace scenarios use three `LineEdit` children per root.
- Icon probe data is Debug-only and records `AddOnDecoratedBox.UpdateIconStatusColors()` plus `ApplyIconBrush()` scans.
- Binding expression count is not directly measured yet; current baseline uses node counts, allocation, timing, and AddOnDecoratedBox probe counters.
- This measures control-level template/style/materialization cost, not Gallery navigation.

## Final 对比摘要

对比文件：`docs/performances/AddOnDecoratedBox/addon-decorated-box-baseline.md`。

| Scenario | Visual/root Baseline | Visual/root Final | KB/item Baseline | KB/item Final | Icon brush calls Baseline | Icon brush calls Final | Icon scan visuals Baseline | Icon scan visuals Final |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| LineEdit.Default | 26.0 | 20.0 | 503.8 | 398.4 | 480 | 0 | 360 | 0 |
| LineEdit.AllowClear | 30.0 | 26.0 | 598.9 | 522.4 | 480 | 120 | 360 | 120 |
| LineEdit.Reveal | 33.0 | 29.0 | 654.6 | 573.1 | 480 | 120 | 360 | 120 |
| LineEdit.Count | 26.0 | 22.0 | 504.3 | 419.6 | 480 | 120 | 360 | 120 |
| SearchEdit.Default | 34.0 | 30.0 | 982.4 | 896.4 | 480 | 60 | 840 | 0 |
| CompactSpace.LineEdit.Horizontal | 87.0 | 73.0 | 1663.4 | 1376.6 | 1440 | 240 | 1080 | 240 |
| CompactSpace.LineEdit.Vertical | 87.0 | 73.0 | 1655.3 | 1385.5 | 1440 | 240 | 1080 | 240 |

## Final 验证

已执行：

- `dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Debug --no-restore`
- `dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Debug --no-restore`
- `dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-build -- --verify-accessories --verify-effective-brushes --verify-addon-states`
- `dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-build -- --count 60 --markdown docs/performances/AddOnDecoratedBox/addon-decorated-box-final.md`

新增验证入口：

- `--verify-accessories`：验证 `LineEdit` / `SearchEdit` 右侧 accessory 按需创建、释放、clear/reveal 行为。
- `--verify-effective-brushes`：验证 inner frame effective brush 状态计算。
- `--verify-addon-states`：验证 addon status/disabled/runtime 更新、空 slot 不扫描、outer/inner addon 几何、CompactSpace 圆角、Select/TreeSelect/Cascader right addon template 绑定。

结论：

- 默认 `LineEdit` 已减少隐藏节点和默认 runtime binding 路径。
- 空 addon slot 在状态切换时不再进入 icon visual scan。
- Select/TreeSelect/Cascader 的 `ContentRightAddOnTemplate` 绑定错误已修复并有最小 headless 验证。
- Gallery 桌面项目已通过 Debug build；`LineEditShowCase` / `SpaceShowCase` 的改善以控件级 headless 数据作为可重复验收依据，真实窗口体感仍属于手动观察项。
