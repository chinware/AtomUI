# AutoComplete 性能优化记录

## 目标

`AutoComplete`、`AutoCompleteSearchEdit`、`AutoCompleteTextArea` 默认关闭态不应该承担候选列表的创建成本。Gallery 的 `AutoCompleteShowCase` 有 13 个 AutoComplete-family 控件，优化前每个控件都会在模板应用时创建并持有 `_candidateList`。

本轮原则：

- 未打开 Popup 的控件不创建 `CandidateList` 和 `PopupFrame`。
- 首次打开时创建重 popup content，关闭后复用，避免频繁开关抖动。
- re-template、detach 时清理 popup content、事件、订阅、timer 和视觉父子关系。
- 修复 `DropDownClosed` 被重复触发的正确性问题。

## 优化内容

- 三个主题模板只保留轻量 `Popup#PART_Popup` 壳：
  - `AutoCompleteTheme.axaml`
  - `AutoCompleteSearchEditTheme.axaml`
  - `AutoCompleteTextAreaTheme.axaml`
- `AbstractAutoComplete` 新增 lazy popup content 生命周期：
  - `EnsurePopupContent()` 首次 materialize `CandidateList` + `PopupFrame`。
  - `ClearPopupContent()` / `ClearPopupFrame()` 负责 detach/re-template 清理。
  - `SyncCandidateListProperties()` 在 materialized 后同步 `OptionTemplate`、`IsMotionEnabled`、`ItemsSource`。
- 将窗口 deactivated 订阅从 attached-time 改成 open-time。
- detach 时清理 `_delayTimer`、`_textInputBoxSubscriptions`、open subscriptions，并取消 async loader。
- 复用静态 `ValueFilter`，减少热路径 filter 对象重复创建。

## 基线与结果

优化前 Gallery trace 数据：

- `AutoCompleteShowCase` runtime AutoComplete count: `13`
- `_popup` field count: `13`
- `_candidateList` field count: `13`
- `CandidateList` visuals: `0`
- repeated mean: `74.97ms`
- repeated alloc: `8143KB`

优化后 Gallery trace：

- `AutoCompleteShowCase` runtime AutoComplete count: `13`
- `_popup` field count: `13`
- `_candidateList` field count: `0`
- `CandidateList` visuals: `0`
- cold trace: `226.80ms`
- second trace: `58.32ms`

优化后 10 次 repeated run：

- cold: `225.23ms`
- repeated mean: `55.63ms`
- repeated median: `55.58ms`
- repeated alloc: `7664.37KB`

按 repeated mean 计算，本轮 Gallery 打开耗时约提升 `25.79%`；按分配计算，约下降 `5.88%`。冷启动基本持平，说明本轮主要消除了关闭态隐藏对象成本，未解决 Gallery 首次构造/主题加载的整体成本。

## 验证命令

```bash
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Debug -f net10.0 --no-restore
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 --no-build -- --verify-autocomplete-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 --no-build -- --suite autocomplete --count 60
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug -f net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug -f net10.0 --no-build -- --showcase autocomplete --trace-navigation --timeout-ms 15000
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug -f net10.0 --no-build -- --showcase autocomplete --warmup 3 --iterations 10 --timeout-ms 15000
```

## 后续观察

- 如果后续需要继续压低冷启动，应拆解 `AutoCompleteLineEditBox` / `AutoCompleteSearchEditBox` / `AutoCompleteTextAreaBox` 的 AddOnDecoratedBox、Icon、Button 组合成本。
- `AutoCompleteSearchEdit` 关闭态仍明显比默认 `AutoComplete` 重，主要来自 search button、icon presenter 和 button 体系。
- async loader 的真实输入路径需要在交互级测试里补充首输、二次输入、detach 中断场景。
