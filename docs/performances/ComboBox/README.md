# ComboBox 性能优化

`ComboBox` 是 Navigation 分类下的高频输入型控件，当前 Gallery 真实场景包含：

- `23` 个 `ComboBox`
- `124` 个 XAML 直写 `ComboBoxItem`
- `8` 个 `ShowCaseItem`

本轮优化原则：关闭态不承担 Popup 重内容成本；默认路径不创建复合 accessory host；可见 handle 不再承担 `IconButton`/`Button` 模板成本；所有 lazy visual、事件和订阅必须有释放路径。

## Phase 0：基线与观测

新增：

- 控件级套件：`tools/performances/AtomUI.Performance/Suites/ComboBox`
- Gallery 真实场景：`tools/performances/AtomUI.GalleryPerformance --showcase combobox`
- 生命周期验证：`--verify-combobox-states`

优化前关键数据：

| 场景 | 指标 | 优化前 |
| --- | --- | ---: |
| `ComboBox.FourItems.Closed` | ms/item | `2.408` |
| `ComboBox.FourItems.Closed` | KB/item | `368.8` |
| `ComboBox.FourItems.Closed` | Visual/root | `19.0` |
| `ComboBox.FourItems.Closed` | Button/IconButton/root | `1.0 / 1.0` |
| `ComboBoxShowCase` repeated mean | ms | `109.35` |
| `ComboBoxShowCase` repeated alloc mean | KB | `11384.83` |
| `ComboBoxShowCase` visuals | count | `562` |
| `ComboBoxShowCase` ComboBoxHost | count | `23` |

## Phase 1：Window.Deactivated 订阅按打开态启用

原问题：每个 `ComboBox` attached 后都会订阅 `Window.Deactivated`，即使从未打开下拉。

变更：

- `Window.Deactivated` 只在 `IsDropDownOpen=true` 时订阅。
- 关闭、detach、re-template 时统一释放。
- 修正 `Window` 类型解析，显式使用 `Avalonia.Controls.Window`，避免只匹配 AtomUI 派生 Window。

## Phase 2：默认 accessory 使用轻量 handle

原问题：默认关闭态也创建 `ComboBoxAccessoryHost`，并订阅 owner 的多项状态。

变更：

- 默认路径只创建 `ComboBoxHandle`。
- 只有 `ContentRightAddOn` 或可见 `FormFeedback` 存在时才创建 `ComboBoxAccessoryHost`。
- `ContentRightAddOn` 移除后会回退到轻量 handle，并释放 host 事件/visual parent。

## Phase 3：Popup 模板减重

原问题：`ControlTheme` 关闭态预先创建 `PopupFrame -> ScrollViewer -> ItemsPresenter`。

变更：

- `ControlTheme` 保留轻量 `PART_Popup` shell。
- `PopupFrame`、`ScrollViewer`、`ItemsPresenter` 改为首次 materialize 时创建。
- 关闭后保留已 materialize 内容，避免开关反复创建。
- detach/re-template 时释放 child、templated parent 和 popup child。

## Phase 4：Handle 热路径收敛

原问题：每个默认 `ComboBoxHandle` 内部都有 `IconButton`，导致 `ComboBoxShowCase` 关闭态有 `23` 个 `Button/IconButton`。

变更：

- `ComboBoxHandle` 模板改为 `IconPresenter`。
- hover/pressed 图标颜色 selector 迁移到 `ComboBoxHandle` 状态。
- click 由 `ComboBoxHandle` pointer 逻辑转发给 owner，避免默认 handle 承担 `IconButton` 模板成本。

## Phase 5：长列表与首开路径评估

本轮不改容器生成/虚拟化策略。原因：

- XAML 直写 `ComboBoxItem` 的对象创建成本来自 Gallery 示例本身，控件无法在不改变语义的情况下消除。
- 真正打开 Popup 的平台 host 在 headless 工具中不可用，本轮用 `PopupMaterialized` 验证 lazy content 生命周期，不把平台 popup 创建时间混入控件结论。
- 当前收益主要来自关闭态默认成本收敛；长列表首开如后续仍慢，应单独针对 popup host、items virtualization、selection/focus 语义建立专项测试。

## 最终结果

| 场景 | 指标 | 优化前 | 优化后 | 变化 |
| --- | --- | ---: | ---: | ---: |
| `ComboBox.FourItems.Closed` | ms/item | `2.408` | `1.800` | `-25.25%` |
| `ComboBox.FourItems.Closed` | KB/item | `368.8` | `289.4` | `-21.53%` |
| `ComboBox.FourItems.Closed` | Visual/root | `19.0` | `16.0` | `-15.79%` |
| `ComboBox.FourItems.Closed` | Button/IconButton/root | `1.0 / 1.0` | `0.0 / 0.0` | removed |
| `ComboBox.FortyItems.Closed` | ms/item | `4.027` | `3.328` | `-17.36%` |
| `ComboBox.FortyItems.Closed` | KB/item | `688.8` | `609.2` | `-11.56%` |
| `ComboBoxShowCase` repeated mean | ms | `109.35` | `91.07` | `-16.72%` |
| `ComboBoxShowCase` repeated alloc mean | KB | `11384.83` | `9534.28` | `-16.25%` |
| `ComboBoxShowCase` visuals | count | `562` | `497` | `-65` |
| `ComboBoxShowCase` Button/IconButton | count | `23 / 23` | `0 / 0` | removed |
| `ComboBoxShowCase` ComboBoxHost | count | `23` | `4` | `-19` |

## 验证命令

```bash
dotnet run --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-combobox-states
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite combobox --count 30
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase combobox --warmup 2 --iterations 6 --timeout-ms 30000
```
