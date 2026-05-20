# 回滚因性能优化引入的 ControlTheme 修改

> 范围：`78bce1f3..release/6.0`（共 69 个 commit，其中约 50 个触及 `*.axaml` ControlTheme 文件）。
> 本文记录所有"为性能优化对 ControlTheme 做的调整"以及随之而来的 C# 代码改动，并给出逐一回滚的策略。

## 1. 背景与目标

在 `78bce1f3` 之后的提交中，多个控件以"性能优化"为名重写了 `ControlTemplate`，把原本静态写在 axaml 里的子元素挪到 C# 中通过 `Ensure*/Clear*/Sync*` 方法按需创建（dynamic instantiation pattern），同时引入了大量 disposables、生命周期 hook 以及与之配套的属性同步逻辑。

实践证明这一改动带来了多类回归（订阅/解绑漏写、模板部件查找延迟、样式优先级丢失、与 popup motion 之间的时序冲突等）。本计划要求**全部回滚**这部分修改：
- 还原 ControlTheme（axaml）。
- 还原与 axaml 改动配套的 C# 代码（动态创建/清理逻辑）。
- 保留：性能基准用的 `tools/performances/AtomUI.Performance/Suites/.../*StateVerification.cs`、`*Scenarios.cs`、`docs/performances/**` 文档、`Gallery` 里的 ShowCase 改动。

## 2. 修改模式分类

按典型 diff 分为以下几类，回滚策略也按类划分。

### 模式 A：模板元素 → C# 动态创建
最常见。Theme 中删除若干子节点（CheckBox、IconPresenter、ContentPresenter、Border、Panel 容器等），改由控件 OnApplyTemplate / Open / Sync 流程里通过 `EnsureXxx`/`ClearXxx` 创建并 add 到模板里残留的占位 `Panel`/`Decorator`。
- 典型代码痕迹：
  - 私有字段 `private XXX? _xxxField;`
  - `EnsureXxx()`、`ClearXxx()`、`SyncXxxProperties()` 方法
  - `CompositeDisposable _xxxDisposables` / `IDisposable _xxxSubscription`
  - `OnApplyTemplate` 里 `ClearXxx()` 在 `base.OnApplyTemplate` 之前、然后从 NameScope 找占位 host
- 回滚策略：恢复原 axaml 节点（含 `TemplateBinding`、`MultiBinding`、`Style`/`Setter`），删除 C# 中的 `_xxxField`、`Ensure/Clear/Sync` 方法、对应 disposables、`OnApplyTemplate`/`OnDetachedFromVisualTree` 的相关清理代码。

### 模式 B：Popup 内容延迟创建
Popup 的内容控件（CascaderView、TreeView、SelectCandidateList 等）从 axaml 里的 `Popup.Child` / `PopupFrame` 静态节点，改为在 `OpenDropDown` 第一次触发时由 `EnsurePopupContent` 创建，并在控件 detach 时 `ClearPopupContent`。
- 典型代码痕迹：
  - `private protected override void EnsurePopupContent()` / `ClearPopupContent()`
  - 各类事件订阅在 EnsurePopupContent 内部 `+=`，在 ClearPopupContent 里 `-=`
  - 带补偿逻辑：`if (Popup.IsPlayingCloseMotion) Popup.CancelCloseAnimation()` 之类
- 回滚策略：恢复 axaml 中的静态 `Popup` / `PopupFrame.Child`；删除 `EnsurePopupContent`/`ClearPopupContent` 重写；事件订阅放回 `OnApplyTemplate`。

### 模式 C：DisableTransitions / EnableTransitions 生命周期
Theme 内 `Transitions` 改成静态写法、控件 `OnInitialized`/`OnLoaded` 调用 `this.DisableTransitions()` / `Dispatcher.Post(this.EnableTransitions)`，避免初始化时跑过渡动画。
- 回滚策略：删掉 `DisableTransitions/EnableTransitions` 调用，恢复原 axaml `Transitions` 写法（如有改动）。
- 备注：`78bce1f3` 之前也有该 pattern（`4c132ae9d`），那次提交不在本次回滚范围；只回滚本次窗口内新追加的改动。

### 模式 D：Layout 切片化（PART 重命名 / 多 host）
为给 Ensure 模式留 host，把原本一个根 layout（如 Button 的 `PART_RootLayout`）拆成 `PART_FrameLayout` + `PART_ContentLayout`，并去掉与之耦合的子元素。
- 回滚策略：恢复原 PART 名与节点；删除 C# 中针对新 PART 的 host 字段和绑定逻辑。

### 模式 E：Light dismiss / 关闭动画相关 popup theme 改动
`Flyout`、`Dialog`、`Popup` 类控件的 axaml + AbstractSelect.PopupOpened/PopupClosed 强制同步 IsDropDownOpen 的逻辑（`IgnorePropertyChange = true`）属于此类。
- 回滚策略：还原 axaml；移除 AbstractSelect.PopupOpened/PopupClosed 中新加的强制写 `IsDropDownOpen` 的分支与 `Popup.IsPlayingCloseMotion` 抢救分支。

### 模式 F：Icon slot / metadata 生成器
`6df17d7ca`、`77364bf3b`、`b24f5dfe4`、`50662c49e` 一组 commit 改了 IconProvider 缓存、AntDesign 生成器输出，以及 Button/Menu/NavMenu 中"按名查 icon slot"的 axaml + 配套 C#。回滚意味着 AntDesign metadata 也要重新生成。
- 回滚策略：先回滚生成器和缓存层，再回滚使用方 axaml/C#，最后跑生成器命令更新生成产物（如有）。

## 3. 受影响 Commit 清单（按时间顺序）

> 标记说明：
> - `[T]` 表示该 commit 修改了 axaml ControlTheme 文件
> - `[C]` 表示该 commit 修改了与 theme 紧耦合的控件源码（伴随回滚）
> - `[K]` 表示该 commit 还包含可保留的内容（State Verification / Scenarios / Docs），回滚时**不要**碰这些文件

### 入口 / 公共组件

| Commit | Title | Pattern |
| --- | --- | --- |
| `cdb6ed84e` | LineEdit accessory host management | A,D — `LineEdit*`、`AddOnDecoratedBox`、`Cascader/Select/TreeSelect AddOnDecoratedBox` 全套 |
| `65eb0616e` | Refactor accessory host management for ComboBox/Picker | A — `AddOnDecoratedBox`、`InfoPickerInput`、`Select/TreeSelect/Cascader/ComboBox` |
| `b24f5dfe4` | Icon brush/pen init optimize, transitions | C(only) |
| `50662c49e` | DrawingInstruction PushTransform | C(only) |
| `77364bf3b` | Icons regenerate antdesign metadata | F — 生成产物 |
| `6df17d7ca` | Icons optimize slots & metadata | F — Button/Menu/NavMenu/SelectHandle theme + C# |
| `df625e0a9` | Icon handling perf across components | C |
| `1e1d9e469` | GridColLayout responsive | 跳过（无 theme 改动） |
| `196508793` | AtomUI.Performance scaffolding | K — 跳过 |

### 单控件（按字母序）

| Commit | Control | Pattern |
| --- | --- | --- |
| `cdad8c780` | Alert | A |
| `4f87a97c9` | AutoComplete | A,B |
| `d75d54192` | Avatar / AvatarGroup | A |
| `3bd15ccac` | Badge / DotBadge | A |
| `a6648fc17` | Button / DropdownButton | A,D — Wave / Loading / Icon presenter |
| `b1973674e` | ButtonSpinner | A |
| `abf7006e4` | Card / CardGridItem | A |
| `05e5a88ef` | Carousel | A |
| `04adf2169` | Cascader / CascaderView / FilterList / EmptyIndicator | A,B（已部分手动还原，但需要按本计划复核） |
| `a91ea4ade` | CheckBox（首发） | A |
| `9a5e7c761` | CheckBox indicator | A — 含 `CheckBoxIndicatorStateConverter.cs` 删除 |
| `17fc74662` | CheckBox layout | C — 仅尺寸校验，可能不需回滚（待复核） |
| `52e5c7c53` | Collapse / CollapseItem | A |
| `80526c3bb` | ComboBox / Handle | A,B |
| `da827f00d` | CompactSpace | C |
| `db89536bd` | DatePicker / RangeDatePicker / InfoPickerInput | A,B |
| `34d6a0172` | Descriptions | A |
| `ff076f72b` | Dialog / OverlayDialogHost / MessageBox | A,E |
| `83547f32d` | MessageBox loading content | A |
| `bd3d3f85a` | Drawer / DrawerContainer / Info | A |
| `41d572ae0` | Empty | A |
| `5cb566813` | Expander | A,B（lazy） |
| `e9dd1a2f6` | FloatButton（含 Group/Items/BackTop） | A,B |
| `d4097343d` | Flyout | E,B |
| `8f581b371` | Form | A,C |
| `652073adf` | ImagePreviewer / GroupBox / Toolbar | A |
| `25c2c3d8f` | Input / TextArea / TextBox | A |
| `512f12bca` | ListBox / ListView | A |
| `a2220c3d9` | ListView pagination | C |
| `94cafd5e4` | Mentions | A,B |
| `59be2268e` | Menu / MenuItem / TopLevelMenuItem | A |
| `1aaadb96c` | TopLevelMenuItem hover transition | **跳过，纯 UX，不在回滚范围** |
| `57836720b` | Message / WindowMessageManager | A |
| `0c74cc485` | NavMenu / NavMenuItem / 各种 Header | A |
| `1e931e24f` | SelectCandidateList | A |
| `9f817fd0e` | Notification / NotificationCard / WindowNotificationManager | A |
| `2911b1182` | NumericUpDown / ButtonSpinnerDecoratedBox | A |
| `71cb534c8` | OptionButtonGroup / OptionButton | A |
| `28542036f` | Pagination / PopupConfirm | A |
| `2fb2739a5` | ProgressBar (Circle/Line/Steps/Dashboard) | A |
| `04710c1f2` | QRCode | A |
| `6d69c8ae6` | RadioButton / RadioIndicator | A |
| `3de9cfcbc` | Rate / RateItem / RateCharacter | A |
| `755145557` | Result | A |
| `e94eb1d64` | ScrollViewer / AvaScrollViewer / ScrollBar | A |
| `884d21ee6` | Segmented / SegmentedItem | A |
| `85933d57e` | Select 同步 | A,B（部分已在 d79cb69f5 / 65eb0616e 处理） |
| `d79cb69f5` | Select / Cascader / TreeSelect 响应式 | A,E（包含 AbstractSelect 强制 close 分支） |
| `35f26975c` | Skeleton / Separator | A |
| `089e6d114` | Slider / Thumb / Track | A |
| `a47a619b8` | Space | C |
| `fbe3b1db9` | Spin / Indicator | A |
| `3137a536b` | Splitter / Handle / Panel | A |
| `e6d9c45bc` | SplitView | A |
| `a13538490` | Statistic / TimerStatistic / Skeleton 同步 | A |
| `2bd55af39` | Steps / StepsItem / StepsItemIndicator | A |
| `e17cd490c` | TabControl / TabItem / TabStrip | A |
| `5a04a5b2e` | ToggleSwitch（首发） | A |
| `31663962a` | ToggleSwitch（精修） | A |

> 完整 commit→files 映射见仓库内 `git log --name-only 78bce1f3..release/6.0`。

## 4. 回滚策略

### 4.1 总体顺序（自下而上）
1. **末端控件（叶子）先回滚** — 顺序大致按表中字母序中 Pattern 仅含 A 的控件，例如 Avatar、Empty、Segmented、Steps、Statistic、Splitter、SplitView、Result、QRCode、Rate、ProgressBar、Pagination、Notification、Message、Skeleton、ToggleSwitch、Spin、Slider、Drawer、Carousel 等。
2. **中段（Pattern A + B）** — Cascader、TreeSelect、ComboBox、AutoComplete、Mentions、Expander、FloatButton、Flyout、DatePicker、Dialog、ListBox/ListView。
3. **共享框架（最后）** — AddOnDecoratedBox 系列、AbstractSelect / AbstractAutoComplete 的"strong popup close"逻辑、Icon slot 生成器及其消费方、CompactSpace。
4. **重新生成 Icon metadata**（若回滚到 `77364bf3b` / `6df17d7ca` 之前的生成器，需要执行：）
   ```bash
   dotnet build src/AtomUI.Icons.AntDesign.Generator/AtomUI.Icons.AntDesign.Generator.csproj
   ```
   生成产物会刷新 `src/AtomUI.Icons.AntDesign/Generated/**`。

### 4.2 单 commit 回滚动作

每个 commit 按以下流程操作：

1. `git show <sha> --stat` 拿到改动列表。
2. 对每个 axaml 文件：
   ```bash
   git show <sha>~1:<path> > <path>
   ```
   把内容回退到上一个 commit 的版本。
3. 对每个紧耦合的 C# 文件，先 review diff：
   ```bash
   git show <sha> -- <path>
   ```
   再使用 `git checkout <sha>~1 -- <path>` 还原；或者人工去掉以下痕迹：
   - 字段 `_xxxXxx`、`_xxxDisposables`、`_xxxSubscription`
   - `EnsureXxx()`、`ClearXxx()`、`SyncXxxProperties()`
   - `OnApplyTemplate` 中"先 Clear、再 base、再 Find"的三段式
   - `OnDetachedFromVisualTree`/`OnAttachedToVisualTree` 里新增的清理/恢复
4. 不要回退的文件（K 类标记）：
   - `tools/performances/AtomUI.Performance/Suites/**`
   - `tools/performances/AtomUI.GalleryPerformance/**`
   - `tools/performances/README.md`
   - `docs/performances/**`
   - `controlgallery/**`（除非该 ShowCase 完全依赖被回滚的 API）
5. `dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj` 跑一遍。失败按报错单独修。
6. 如果该 commit 还包含跨控件的共享改动（AddOnDecoratedBox / AbstractSelect），先记录到 4.4 的"全局清单"，最后统一处理。

### 4.3 跨控件共享改动（必须最后处理）

| 文件 | 涉及 commit | 回滚要点 |
| --- | --- | --- |
| `src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox/Themes/AddOnDecoratedBoxTheme.axaml` | `cdb6ed84e`、`65eb0616e` | 恢复完整 PART 节点（左/右 addon、accessory host 槽位） |
| `src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox/AddOnDecoratedBox.cs` | 同上 | 删 accessory host 注册、Ensure 系列 |
| `src/AtomUI.Desktop.Controls/Select/AbstractSelect.cs` | `d79cb69f5`、`65eb0616e` | 删 PopupClosed 内 `if (IsDropDownOpen) IgnorePropertyChange=true; …` 与 OpenDropDown 内 `Popup.IsPlayingCloseMotion`/`CancelCloseAnimation` 抢救分支 |
| `src/AtomUI.Desktop.Controls/AutoComplete/AbstractAutoComplete.cs` | `4f87a97c9` | 同上 popup 强制同步段，以及 lazy popup content |
| `src/AtomUI.Desktop.Controls/Primitives/InfoPickerInput/InfoPickerInput.cs` 与 `PickerAccessoryHost.cs` | `cdb6ed84e`、`65eb0616e`、`db89536bd` | 恢复 host 静态化前的写法 |
| `src/AtomUI.Core/Controls/Icon/IconProviderCache.cs`、`src/AtomUI.Icons.AntDesign.Generator/AntDesignGenerator.cs`、`src/AtomUI.Icons.AntDesign/AntDesignIcon.cs` | `6df17d7ca`、`77364bf3b` | 一并回滚生成器和缓存；之后重跑生成 |

### 4.4 已部分手动还原的控件
- **Cascader**（`04adf2169`、`d79cb69f5`、`cdb6ed84e`、`65eb0616e` 的 Cascader 部分）已经在最近一次会话中手动还原 axaml 与部分 C#，需对照本计划复核：
  - `Cascader.cs` / `CascaderView.cs` / `CascaderView.Filter.cs` / `CascaderView.ExpandAndCollapse.cs` / `CascaderViewItem.cs` 与 `Themes/Cascader*.axaml`。
  - `CascaderAddOnDecoratedBox.cs` 还原由 4.3 第 1 行集中处理。

## 5. 验证方案

每次回滚一个控件之后：
1. **构建**：
   ```bash
   dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj
   ```
2. **Gallery 烟囱测试**：
   ```bash
   dotnet run --project controlgallery/AtomUIGallery.Desktop --framework net10.0
   ```
   打开对应控件页面，验证：
   - 渲染显示无空白/错位
   - 状态切换（hover/pressed/disabled）样式正确
   - 控件特性（多选/筛选/异步加载/expand 等）行为正确
3. **State Verification（保留的）**：跑一次 `tools/performances/AtomUI.Performance` 对应套件，确认状态机功能性指标没有 regression。
4. **Cascader 专项**：单选与多选两路、Search、AsyncLoad、IsAllowSelectParent 都测一遍（参考最近修复过的 5 个 bug）。

## 6. 提交规范

建议每控件一笔提交，commit message 模板：
```
revert(<Control>): roll back perf-driven theme/code changes

Reverts the dynamic-instantiation pattern from <perf-commit-sha-7>.
- Restored axaml structure for <theme files>
- Removed Ensure/Clear/Sync helpers and disposables in <code files>

Refs: docs/tasks/revert-perf-controltheme-changes.md
```

## 7. 风险与备注

- **生命周期 hook 残留**：很多控件现在 `DisableTransitions()`/`EnableTransitions` 被加在 `OnInitialized`/`OnLoaded`，回滚 axaml 后这些调用就成了 dead code，注意一并删除，否则首次显示时还是会跳过过渡。
- **`SetTemplatedParent`**：动态创建模式里大量出现 `child.SetTemplatedParent(this)` 与 `child.SetTemplatedParent(null)`。axaml 还原后这些调用失去配对，需要全部清理。
- **`AtomUI.Reflection`**：动态创建里大量 `using AtomUI.Reflection`（用于 `IListItemVirtualizingContextAware`、`SetTemplatedParent` 扩展），axaml 还原后 using 可能未使用，需要扫一遍。
- **`AddOnDecoratedBox` 的 accessory host 接口**：是回滚最复杂的一块。回滚此处会同步影响 LineEdit、SearchEdit、TextArea、Cascader、Select、TreeSelect、ComboBox、ButtonSpinner、InfoPickerInput、NumericUpDown，需要按 4.1 顺序最后处理。
- **AbstractSelect popup 强同步段**：与 Cascader 的 popup 关闭/重开 bug 直接相关。回滚后要重新 review 那批关闭逻辑（Cascader、Select、TreeSelect、AutoComplete、ComboBox、Mentions）。
- **Icon 生成器**：回滚生成器会触发大量 `*.g.cs` 文件变化。建议在一次单独提交里完成生成器与生成产物的同步。

## 8. 参考命令片段

```bash
# 列出窗口内所有 commit
git log --reverse --format='%h %s' 78bce1f3..release/6.0

# 查某 commit 改了哪些 axaml
git show --name-only --format= <sha> | grep '\.axaml$'

# 查某 commit 改了哪些 C#
git show --name-only --format= <sha> | grep '\.cs$' | grep -v controlgallery | grep -v AtomUI.Performance

# 把单文件回退到上一个版本
git show <sha>~1:<path> > <path>

# 直接 checkout 上一个版本（覆盖）
git checkout <sha>~1 -- <path>

# 跑构建验证
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj
```
