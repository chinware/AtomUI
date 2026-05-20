# 详细回滚执行计划 — 性能优化引入的 ControlTheme 改动

> 总览文档：`docs/tasks/revert-perf-controltheme-changes.md`
> 本文给每个控件列出 **commit、文件、回滚动作、验证项**，以及分批执行顺序。
>
> 范围：`78bce1f3..release/6.0`（69 个 commit，53 个触及 axaml ControlTheme）。

## 0. 执行约定

### 0.1 通用流程
每个控件按 **R-V-C** 三步执行：

1. **R**ollback（回退文件）
   ```bash
   # axaml 一律取性能改动前最近一次的版本
   git checkout <perf-sha>~1 -- <axaml-path>
   ```
   ```bash
   # C# 文件先 review，确认是否有"非性能"改动需要保留
   git diff <perf-sha>~1..<perf-sha> -- <cs-path>
   ```
   - 若该 commit 的 C# 改动 **完全是为支撑 axaml** 的（`Ensure*/Clear*/Sync*` + 字段 + disposables），可直接：
     ```bash
     git checkout <perf-sha>~1 -- <cs-path>
     ```
   - 若 C# 改动里夹带了非性能的修复/特性，必须**手工剔除性能段**，保留其它。
2. **V**erify（构建 + Gallery）
   ```bash
   dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj
   ```
   构建过后跑一次 Gallery 看对应控件页面：
   ```bash
   dotnet run --project controlgallery/AtomUIGallery.Desktop --framework net10.0
   ```
3. **C**ommit（独立提交）
   ```
   revert(<Control>): roll back perf-driven theme/code changes
   
   Reverts perf-pattern from <perf-sha-7>:
   - <theme files>
   - <cs files>
   ```

### 0.2 不要回退的文件（K 类）
即使在某个 perf commit 内出现，也**保留**：
- `tools/performances/AtomUI.Performance/Suites/**/*.cs`
- `tools/performances/AtomUI.GalleryPerformance/Program.cs`
- `tools/performances/README.md`
- `docs/performances/**`
- `controlgallery/**`（ShowCase 改动通常是数据/布局层，与 ControlTheme 解耦）

### 0.3 回滚通用代码痕迹检查表
回滚一个控件之后，去文件内 `grep` 以下关键字，把残留全部删干净：
- `Ensure[A-Z]\w+\(\)` / `Clear[A-Z]\w+\(\)` / `Sync[A-Z]\w+Properties\(\)`
- `_[a-z]\w+Disposables` / `_[a-z]\w+Subscription`
- `SetTemplatedParent\(this\)` / `SetTemplatedParent\(null\)`
- `using AtomUI.Reflection;`（动态创建模式常引入，恢复后通常用不到）
- `DisableTransitions()` / `EnableTransitions` / `Dispatcher.Post(this.EnableTransitions)`
- 在 `OnApplyTemplate` 顶部"先 Clear、再 base.OnApplyTemplate、再 Find"的三段式

### 0.4 分支与提交策略

#### 工作分支：`release/6.0-fixup`
1. 从当前 `release/6.0` 拉出工作分支：
   ```bash
   git checkout release/6.0
   git checkout -b release/6.0-fixup
   ```
2. 所有回滚操作都在 `release/6.0-fixup` 上完成。
3. **每个控件单独一笔 commit**，便于 PR review、bisect 和阶段性验证；commit message 模板：
   ```
   revert(<Control>): roll back perf-driven theme/code changes
   
   Reverts perf-pattern from <perf-sha-7>:
   - <theme files>
   - <cs files>
   ```
4. 共享框架的 commit 放最后，标题写明影响面。
5. 全量回归通过后，进入第 11 节"历史重写"流程，把工作成果以**整洁的历史**写回 `release/6.0`。

#### 关键约束
- `release/6.0-fixup` 上的回滚 commit **只是工作过程**，最终不会进入 `release/6.0` 历史。
- 所以 commit 粒度可以放心做细，不必担心污染。
- 重写历史前必须有完整 Gallery 与 state-verification 通过的截图/记录留存。

---

## 1. 批次 1 — 叶子静态控件（无外部依赖，先开炮）

> 本批 commit 都是 Pattern A：单纯把 axaml 节点动态化。回滚最直接，互不干扰，强烈推荐用作 Pattern 验证。

### 1.1 Empty
- **Commit**：`41d572ae0`
- **回滚 axaml**：
  - `src/AtomUI.Desktop.Controls/Empty/Themes/EmptyTheme.axaml`
- **回滚 C#（剔除 Ensure/Clear，保留可能的 BuiltInImageBuilder 调整）**：
  - `src/AtomUI.Controls/Empty/AbstractEmpty.cs`
  - `src/AtomUI.Controls/Empty/BuiltInImageBuilder.cs` — review，可能 BuiltIn image 是新增内容，保留
  - `src/AtomUI.Desktop.Controls/Empty/Empty.cs`
- **验证**：Gallery `Empty`、`Result`（依赖 Empty 间接）。

### 1.2 Avatar / AvatarGroup
- **Commit**：`d75d54192`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/Avatar/Themes/AvatarTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Controls/Avatar/AbstractAvatar.cs`
  - `src/AtomUI.Desktop.Controls/Avatar/AvatarGroup.cs`
- **验证**：Gallery `Avatar` 全部 ShowCase（含 maxCount、tooltip、size）。

### 1.3 Badge
- **Commit**：`3bd15ccac`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/Badge/Themes/DotBadgeAdornerTheme.axaml`
- **回滚 C#**（数量较多，`AbstractRibbon` / `AbstractCount` / `AbstractDot` 三族）：
  - `src/AtomUI.Controls/Badge/AbstractCountBadge.cs` / `AbstractCountBadgeAdorner.cs`
  - `src/AtomUI.Controls/Badge/AbstractDotBadge.cs` / `AbstractDotBadgeAdorner.cs`
  - `src/AtomUI.Controls/Badge/AbstractRibbonBadge.cs` / `AbstractRibbonBadgeAdorner.cs`
  - `src/AtomUI.Controls/Badge/BadgeColorUtils.cs` — review 是否纯工具方法增删
- **验证**：Gallery `Badge`（dot/count/ribbon 全形态）、`Notification`（顶部小红点依赖）。

### 1.4 Result
- **Commit**：`755145557`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/Result/Themes/ResultTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Controls/Result/AbstractResult.cs`
  - `src/AtomUI.Desktop.Controls/Result/Result.cs`
- **验证**：Gallery `Result`（成功 / 失败 / 警告 / 自定义图标各一）。

### 1.5 QRCode
- **Commit**：`04710c1f2`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/QRCode/Themes/QRCodeTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Controls/QRCode/AbstractQRCode.cs`
  - `src/AtomUI.Desktop.Controls/QRCode/QRCode.cs`
- **验证**：Gallery `QRCode`（带/不带 logo、不同 status）。

### 1.6 Descriptions
- **Commit**：`34d6a0172`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/Descriptions/Themes/DescriptionsTheme.axaml`
- **回滚 C#**：`src/AtomUI.Desktop.Controls/Descriptions/Descriptions.cs`
- **验证**：Gallery `Descriptions`（横向/纵向、bordered、嵌套）。

### 1.7 Statistic（含 TimerStatistic + Skeleton 同步）
- **Commit**：`a13538490`
- **回滚 axaml**：
  - `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonTheme.axaml`
  - `src/AtomUI.Desktop.Controls/Statistic/Themes/AbstractStatisticTheme.axaml`
  - `src/AtomUI.Desktop.Controls/Statistic/Themes/TimerStatisticTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Desktop.Controls/Skeleton/Skeleton.cs`
  - `src/AtomUI.Desktop.Controls/Statistic/Statistic.cs`
  - `src/AtomUI.Desktop.Controls/Statistic/StatisticUtils.cs`
  - `src/AtomUI.Desktop.Controls/Statistic/TimerStatistic.cs`
- **注意**：与 1.18 Skeleton 有交叉，先做 Statistic，做完再做 Skeleton 时 review 是否还有残留。
- **验证**：Gallery `Statistic`、`TimerStatistic`、`Skeleton`。

### 1.8 Steps
- **Commit**：`2bd55af39`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/Steps/Themes/StepsItemIndicatorTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Desktop.Controls/Steps/Steps.cs`
  - `src/AtomUI.Desktop.Controls/Steps/StepsItem.cs`
  - `src/AtomUI.Desktop.Controls/Steps/StepsItemIndicator.cs`
- **验证**：Gallery `Steps`（horizontal/vertical/dot/icon 全形态）。

### 1.9 SplitView
- **Commit**：`e6d9c45bc`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/SplitView/Themes/SplitViewTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Desktop.Controls/SplitView/SplitView.cs`
  - `src/AtomUI.Desktop.Controls/SplitView/SplitViewToken.cs`
- **验证**：Gallery `SplitView`（pane open/close、display mode 切换）。

### 1.10 Splitter
- **Commit**：`3137a536b`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/Splitter/Themes/SplitterHandleTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Desktop.Controls/Splitter/Splitter.cs`
  - `src/AtomUI.Desktop.Controls/Splitter/SplitterHandle.cs`
  - `src/AtomUI.Desktop.Controls/Splitter/SplitterPanel.cs`
- **验证**：Gallery `Splitter`（拖动、min/max、reverse 方向）。

---

**批次 1 收尾**：跑一次完整构建 + Gallery 烟囱测试。如有 grep 残留，统一清理后提一笔 `chore(perf-revert): tidy up pattern leftovers from batch 1`。

---

## 2. 批次 2 — 简单输入 / 状态控件

### 2.1 ToggleSwitch
- **Commits（合并回滚）**：`5a04a5b2e`、`31663962a`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/Switch/Themes/ToggleSwitchTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Controls/Switch/AbstractToggleSwitch.cs`
  - `src/AtomUI.Controls/Switch/SwitchKnob.cs`
  - `src/AtomUI.Desktop.Controls/Switch/ToggleSwitch.cs`
- **执行**：直接回到 `5a04a5b2e~1` 即可，因为两次都是性能改动。
- **验证**：Gallery `ToggleSwitch`（loading、checked/unchecked、disabled）。

### 2.2 RadioButton / RadioIndicator
- **Commit**：`6d69c8ae6`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/RadioButton/Themes/RadioIndicatorTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Controls/RadioButton/AbstractRadioButtonGroup.cs`
  - `src/AtomUI.Controls/RadioButton/RadioIndicator.cs`
- **验证**：Gallery `RadioButton`（单选 group、disabled、size）。

### 2.3 CheckBox（首版） + CheckBox indicator + layout
- **Commits**：`a91ea4ade`、`9a5e7c761`、`17fc74662`
- **回滚顺序**：先 `17fc74662`（layout 校验，最近一笔），再 `9a5e7c761`，最后 `a91ea4ade`。
- **回滚 axaml**：
  - `src/AtomUI.Desktop.Controls/CheckBox/Themes/CheckBoxIndicatorTheme.axaml`
  - `src/AtomUI.Desktop.Controls/CheckBox/Themes/CheckBoxTheme.axaml`
- **回滚 C#（含被删除文件需要 restore）**：
  - `src/AtomUI.Controls/CheckBox/AbstractCheckBox.cs`
  - `src/AtomUI.Controls/CheckBox/AbstractCheckBoxGroup.cs`
  - `src/AtomUI.Controls/CheckBox/AbstractCheckBoxItemsControl.cs`
  - `src/AtomUI.Controls/CheckBox/CheckBoxIndicator.cs`
  - `src/AtomUI.Controls/CheckBox/Converters/CheckBoxIndicatorStateConverter.cs` — `9a5e7c761` 删除了它，需要 `git checkout 9a5e7c761~1 -- <path>` 把文件 restore 回来
- **验证**：Gallery `CheckBox`、`CheckBoxGroup`、级联 `Cascader` 多选（间接依赖）。

### 2.4 Rate
- **Commit**：`3de9cfcbc`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/Rate/Themes/RateItemTheme.axaml`、`RateTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Controls/Rate/AbstractRate.cs`
  - `src/AtomUI.Controls/Rate/RateCharacter.cs`
  - `src/AtomUI.Controls/Rate/RateItem.cs`
- **验证**：Gallery `Rate`（默认 / 半星 / 自定义字符 / 颜色）。

### 2.5 Segmented
- **Commit**：`884d21ee6`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedItemTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Controls/Segmented/AbstractSegmented.cs`
  - `src/AtomUI.Controls/Segmented/AbstractSegmentedItem.cs`
  - `src/AtomUI.Controls/Segmented/SegmentedStackPanel.cs`
- **验证**：Gallery `Segmented`（icon/label、disabled item、size）。

### 2.6 OptionButtonGroup
- **Commit**：`71cb534c8`
- **回滚 axaml**：`src/AtomUI.Desktop.Controls/OptionButtonGroup/Themes/OptionButtonTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Controls/OptionButtonGroup/AbstractOptionButton.cs`
  - `src/AtomUI.Controls/OptionButtonGroup/AbstractOptionButtonGroup.cs`
  - `src/AtomUI.Desktop.Controls/OptionButtonGroup/OptionButtonToken.cs`
- **验证**：Gallery `OptionButtonGroup`、`Cascader > Placement`（用到该组件）。

### 2.7 Slider
- **Commit**：`089e6d114`
- **回滚 axaml**：
  - `src/AtomUI.Desktop.Controls/Slider/Themes/SliderTheme.axaml`
  - `src/AtomUI.Desktop.Controls/Slider/Themes/SliderThumbTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Desktop.Controls/Slider/Slider.cs`
  - `src/AtomUI.Desktop.Controls/Slider/SliderThumb.cs`
  - `src/AtomUI.Desktop.Controls/Slider/SliderTrack.cs`
- **验证**：Gallery `Slider`（mark、tooltip、range、disabled）。

### 2.8 ProgressBar（Line / Circle / Steps / Dashboard）
- **Commit**：`2fb2739a5`
- **回滚 axaml**：
  - `AbstractCircleProgressTheme.axaml`
  - `AbstractLineProgressTheme.axaml`
- **回滚 C#（7 个）**：
  - `AbstractCircleProgress.cs` / `AbstractGeneralCircleProgress.cs` / `AbstractGeneralDashboardProgress.cs`
  - `AbstractLineProgress.cs` / `AbstractGeneralProgressBar.cs` / `AbstractGeneralStepsProgressBar.cs`
  - `AbstractProgressBar.cs`
- **验证**：Gallery `ProgressBar`（line/circle/dashboard/steps、indeterminate）。

---

## 3. 批次 3 — 容器型控件

### 3.1 Card / CardGridItem
- **Commit**：`abf7006e4`
- **回滚 axaml**：`Card/Themes/CardGridItemTheme.axaml`、`CardTheme.axaml`
- **回滚 C#**：`Card/Card.cs`、`CardActionPanel.cs`、`CardGridContent.cs`、`CardGridItem.cs`
- **验证**：Gallery `Card`（含 actions、grid、cover）。

### 3.2 Collapse
- **Commit**：`52e5c7c53`
- **回滚 axaml**：`Collapse/Themes/CollapseItemTheme.axaml`
- **回滚 C#**：`Collapse/Collapse.cs`、`CollapseItem.cs`
- **验证**：Gallery `Collapse`（borderless、accordion、ghost、icon）。

### 3.3 Carousel
- **Commit**：`05e5a88ef`
- **回滚 axaml**：`Carousel/Themes/CarouselNavButtonTheme.axaml`、`CarouselPageIndicatorTheme.axaml`、`CarouselTheme.axaml`
- **回滚 C#**：`Carousel/Carousel.cs`、`CarouselPageIndicator.cs`、`VirtualizingCarouselPanel.cs`
- **验证**：Gallery `Carousel`（自动播放、nav button、indicator 位置）。

### 3.4 Skeleton（与 Statistic 关联，先做完 1.7）
- **Commit**：`35f26975c`
- **回滚 axaml**：`Separator/Themes/SeparatorTheme.axaml`
- **回滚 C#**：`Separator/AbstractSeparator.cs`、`Separator/Separator.cs`、`Skeleton/AbstractSkeleton.cs`、`Skeleton/Skeleton.cs`、`Skeleton/SkeletonAvatar.cs`、`Skeleton/SkeletonLine.cs`、`Skeleton/SkeletonParagraph.cs`
- **验证**：Gallery `Skeleton`（avatar/paragraph/title）、`Separator`、`Statistic`。

### 3.5 Spin
- **Commit**：`fbe3b1db9`
- **回滚 axaml**：`Spin/Themes/SpinTheme.axaml`
- **回滚 C#**：`Spin/AbstractSpin.cs`、`AbstractSpinIndicator.cs`、`Spin/Spin.cs`
- **验证**：Gallery `Spin`、`Mentions`（异步 loading 间接依赖）、`Cascader`（async load）。

### 3.6 Drawer
- **Commit**：`bd3d3f85a`
- **回滚 axaml**：`Drawer/Themes/DrawerContainerTheme.axaml`、`DrawerInfoContainerTheme.axaml`
- **回滚 C#**：`Drawer/Drawer.cs`、`Drawer/DrawerContainer.cs`
- **验证**：Gallery `Drawer`（4 个方向、size、mask）。

### 3.7 Expander
- **Commit**：`5cb566813`
- **回滚 axaml**：`Expander/Themes/ExpanderTheme.axaml`
- **回滚 C#**：`Expander/Expander.cs`
- **验证**：Gallery `Expander`（多种 placement、内含其它控件）。

### 3.8 Pagination + PopupConfirm
- **Commit**：`28542036f`
- **回滚 axaml**：5 个文件（PaginationNavItem/Pagination/QuickJumperBar/SimplePagination + PopupConfirmContainer）
- **回滚 C#**：6 个 Pagination 文件 + `PopupConfirmContainer.cs`
- **验证**：Gallery `Pagination`、`PopupConfirm`（注意它跟 Popup motion 也有交集，验证 confirm 弹窗动画）。

---

## 4. 批次 4 — 反馈 / 通知

### 4.1 Alert
- **Commit**：`cdad8c780`
- **回滚 axaml**：`Alert/Themes/AlertTheme.axaml`
- **回滚 C#**：`Alert/Alert.cs`
- **验证**：Gallery `Alert`（含 marquee、closable、icon）。

### 4.2 Notification（含 NotificationCard、ProgressBar、WindowNotificationManager）
- **Commit**：`9f817fd0e`
- **回滚 axaml**：`NotificationCardTheme.axaml`、`WindowNotificationManagerTheme.axaml`
- **回滚 C#**：`NotificationCard.cs`、`NotificationProgressBar.cs`、`Utils/NotificationProgressBarVisibleConverter.cs`（`9f817fd0e` 删了 1 个文件，注意 restore）、`WindowNotificationManager.cs`
- **验证**：Gallery `Notification`（success/info/warn/error、auto close、custom）。

### 4.3 Message
- **Commit**：`57836720b`
- **回滚 axaml**：`Message/Themes/MessageCardTheme.axaml`
- **回滚 C#**：`MessageCard.cs`、`WindowMessageManager.cs`
- **验证**：Gallery `Message`、Form `MessageBox` 调用。

### 4.4 Dialog / OverlayDialogHost / MessageBox
- **Commits（合并）**：`83547f32d`、`ff076f72b`
- **回滚顺序**：先 `ff076f72b`（最新），再 `83547f32d`。
- **回滚 axaml**：
  - `Dialog/Themes/DialogButtonBoxTheme.axaml`
  - `Dialog/Themes/OverlayDialogHostTheme.axaml`
  - `Dialog/Themes/DialogLoadingContentPresenterTheme.axaml`（`83547f32d` 引入，回滚需要删除文件 + 从 `DialogThemes.axaml` 引用列表移除）
  - `Dialog/Themes/DialogThemes.axaml`
  - `Dialog/Themes/DialogWindowContentTheme.axaml`
  - `MessageBox/Themes/MessageBoxTheme.axaml`
- **回滚 C#**：
  - `Dialog/ButtonBox/DialogButtonBox.cs`、`DialogStandardButton.cs`
  - `Dialog/Converters/OverlayDialogResizerVisibleConverter.cs`（`ff076f72b` 删了，需 restore）
  - `Dialog/OverlayHost/OverlayDialogHost.cs`
  - `Dialog/Themes/OverlayDialogHostTheme.cs`
  - `Dialog/DialogLoadingContentPresenter.cs`（`83547f32d` 新增，回滚要删）
  - `MessageBox/MessageBox.cs`
- **验证**：Gallery `Dialog`、`MessageBox`（含 loading、button bar、confirm/info/warn/error）。

### 4.5 Flyout
- **Commit**：`d4097343d`
- **回滚 axaml**：本 commit 没改 axaml？复核一遍（`git show d4097343d --name-only --format=`）
- **回滚 C#**：复核全文件，重点删除 lazy popup content 与 light dismiss 配置。
- **验证**：Gallery `Flyout`、`Tooltip`（如间接受影响）。

### 4.6 ImagePreviewer + GroupBox + Toolbar
- **Commit**：`652073adf`
- **回滚 axaml**：`GroupBox/Themes/GroupBoxTheme.axaml`、`ImagePreviewer/Themes/ImagePreviewToolbarTheme.axaml`
- **回滚 C#（10 个）**：`GroupBox/GroupBox.cs`、整个 `ImagePreviewer/*` 目录、`WindowTitleBar/WindowTitleBar.cs`
- **验证**：Gallery `Image` / `ImagePreviewer`（含工具栏、打开/关闭动画、缩放、旋转）。

---

## 5. 批次 5 — 列表 / 菜单 / 标签

### 5.1 ListBox / ListView
- **Commits**：`512f12bca`、`a2220c3d9`
- **回滚顺序**：先 `512f12bca`（filtering & indicator），再 `a2220c3d9`（pagination）。
- **回滚 axaml**：`ListBox/Themes/ListBoxItemTheme.axaml`、`ListView/Themes/ListViewItemTheme.axaml`
- **回滚 C#**：`ListBox/ListBox.cs`、`ListBoxItem.cs`、`ListView/ListView.cs`、`ListViewItem.cs`
- **验证**：Gallery `ListBox`、`ListView`（含 paginate、filter、selected indicator）。

### 5.2 Menu / MenuItem / TopLevelMenuItem
- **Commit**：`59be2268e`（`1aaadb96c` 不在范围）
- **回滚 axaml**：`Menu/Themes/MenuItemTheme.axaml`、`TopLevelMenuItemTheme.axaml`
- **注意**：`1aaadb96c` 是纯 hover transition 增强（在 `59be2268e` 之后），**不能整体回滚**到 `59be2268e~1`，否则会丢掉 1aaadb96c 的样式。建议手工把 `59be2268e` diff 反向 apply：
  ```bash
  git show 59be2268e -- '*.axaml' | git apply -R
  ```
  Apply 后再 reapply `1aaadb96c` 的 axaml 改动（手工合并）。
- **回滚 C#**：`Menu/ContextMenu.cs`、`Menu/MenuItem.cs`
- **验证**：Gallery `Menu`、`ContextMenu`、Top-level menu hover transition 仍然存在。

### 5.3 NavMenu（Inline / Vertical / NavMenuItem）
- **Commit**：`0c74cc485`
- **回滚 axaml**：`InlineNavMenuItemHeaderTheme.axaml`、`NavMenuItemTheme.axaml`、`VerticalNavMenuItemHeaderTheme.axaml`
- **回滚 C#**：`DefaultNavMenuInteractionHandler.cs`、`NavMenu.cs`、`NavMenuItem.cs`
- **验证**：Gallery `NavMenu`（horizontal/inline/vertical、submenu、icon）。

### 5.4 SelectCandidateList
- **Commit**：`1e931e24f`
- **回滚 C#**：`SelectCandidateList`（如有 axaml，列出来；当前看只 C#）
- **验证**：Gallery `Mentions`、`AutoComplete`（依赖 candidate list）。

### 5.5 TabControl / TabItem / TabStrip
- **Commit**：`e17cd490c`
- **回滚 axaml**：`BaseTabItemTheme.axaml`、`CardTabItemTheme.axaml`、`TabStrip/BaseTabStripItemTheme.axaml`、`TabStrip/CardTabStripItemTheme.axaml`
- **回滚 C#（9 个）**：`BaseTabScrollViewer.cs`、`CardTabControl.cs`、`TabControl.cs`、`TabControlScrollViewer.cs`、`TabItem.cs`、`TabStrip/CardTabStrip.cs`、`TabStrip/TabStrip.cs`、`TabStrip/TabStripItem.cs`、`TabStrip/TabStripScrollViewer.cs`
- **验证**：Gallery `TabControl`（line/card、closable、addable）、`TabStrip`。

### 5.6 FloatButton / Group / Items / BackTop
- **Commit**：`e9dd1a2f6`
- **回滚 axaml**：`AbstractFloatButtonTheme.axaml`、`BackTopFloatButtonTheme.axaml`、`FloatButtonGroupTheme.axaml`、`FloatButtonItemsControlTheme.axaml`
- **回滚 C#（10 个）**：`AbstractBackTopFloatButton.cs`、`AbstractFloatButton.cs`、`AbstractFloatButtonHost.cs`、`BackTopFloatButton.cs`、`BackTopFloatButtonHost.cs`、`FloatButton.cs`、`FloatButtonGroup.cs`、`FloatButtonGroupHost.cs`、`FloatButtonHost.cs`、`FloatButtonItemsControl.cs`
- **验证**：Gallery `FloatButton`（group、shape、tooltip、BackTop）。

### 5.7 ScrollViewer
- **Commit**：`e94eb1d64`
- **回滚 axaml**：`AvaScrollViewerTheme.axaml`、`ScrollViewerTheme.axaml`
- **回滚 C#**：`AbstractScrollBar.cs`
- **验证**：Gallery 任意带滚动的页面（如 `TabControl`、`Cascader`、长列表）。

---

## 6. 批次 6 — 中段控件（带 Popup，依赖 AbstractSelect/AddOnDecoratedBox）

> 这一批先做 **不**强依赖共享框架的 popup 控件，等 7、8 批次的共享框架处理完之后，再回过头补全。

### 6.1 AutoComplete
- **Commit**：`4f87a97c9`
- **回滚 axaml**：`AutoCompleteSearchEditTheme.axaml`、`AutoCompleteTextAreaTheme.axaml`、`AutoCompleteTheme.axaml`
- **回滚 C#**：`AutoComplete/AbstractAutoComplete.cs`（重点删除 lazy popup + IsDropDownOpen 强同步）
- **验证**：Gallery `AutoComplete`（含 textarea、search edit 模式、dropdown render）。

### 6.2 Mentions
- **Commit**：`94cafd5e4`
- **回滚 axaml**：`Mentions/Themes/MentionsTheme.axaml`
- **回滚 C#**：`MentionTextArea.cs`、`Mentions.cs`
- **验证**：Gallery `Mentions`（含异步 candidate）。

### 6.3 ComboBox
- **Commit**：`80526c3bb`（`65eb0616e` 中的 ComboBox 部分稍后在 8.2 一起做）
- **回滚 axaml**：`ComboBoxHandleTheme.axaml`、`ComboBoxTheme.axaml`
- **回滚 C#**：`ComboBox/ComboBox.cs`、`ComboBoxHandle.cs`
- **验证**：Gallery `ComboBox`（带 filter、disabled、custom item）。

### 6.4 DatePicker / RangeDatePicker
- **Commit**：`db89536bd`（`65eb0616e` 中的 InfoPickerInput 部分留到 8.2）
- **回滚 axaml**：`DatePicker/Themes/RangeDatePickerTheme.axaml`、`InfoPickerInput/Themes/InfoPickerInputTheme.axaml`、`RangeInfoPickerInputTheme.axaml`（InfoPicker 两个先回滚到 `db89536bd~1`，留意 `65eb0616e` 之后还会再覆盖一次，最终以 `cdb6ed84e~1` 为准）
- **回滚 C#**：`DatePicker.cs`、`RangeDatePicker.cs`、`InfoPickerInput.cs`、`PickerAccessoryHost.cs`、`RangeInfoPickerInput.cs`
- **验证**：Gallery `DatePicker`、`RangeDatePicker`（含 picker bar、ranges、time picker）。

### 6.5 NumericUpDown / ButtonSpinner / ButtonSpinnerDecoratedBox
- **Commit**：`2911b1182`、`b1973674e`
- **回滚顺序**：先 `2911b1182`（最新），再 `b1973674e`。
- **回滚 axaml**：`ButtonSpinnerDecoratedBoxTheme.axaml`、`NumericUpDownTheme.axaml`、`ButtonSpinnerTheme.axaml`
- **回滚 C#**：`ButtonSpinner.cs`、`ButtonSpinnerDecoratedBox.cs`、`NumericUpDown.cs`
- **验证**：Gallery `NumericUpDown`、`ButtonSpinner`（spin 双键、disabled、step）。

### 6.6 Form
- **Commit**：`8f581b371`
- **回滚 axaml**：本 commit 是否改了 axaml 待复核（按 stat 没列在 grep 结果）。
- **回滚 C#**：Form 整族
- **验证**：Gallery `Form`（含 validation、layout、submit）。

### 6.7 Input / TextArea / TextBox
- **Commit**：`25c2c3d8f`
- **回滚 axaml**：`Input/Themes/TextAreaTheme.axaml`、`TextBoxTheme.axaml`
- **回滚 C#（9 个）**：`InputClearIconButton.cs`、`LineEditAccessoryHost.cs`、`ResizeHandle.cs`、`SearchEditPanel.cs`、`TextArea.cs`、`TextAreaAccessoryHost.cs`、`TextBox.cs`、`Utils/TextBoxReflectionExtensions.cs`、`Utils/TextLayoutReflectionExtensions.cs`
- **注意**：与 `cdb6ed84e`、`65eb0616e` 中 Input/TextArea 部分有交叉，最终以批次 8 的"AddOnDecoratedBox 共享回滚"为准。先把 `25c2c3d8f` 自己的部分回退干净。
- **验证**：Gallery `Input`、`TextArea`、`SearchEdit`（multi-line、clear icon、prefix/suffix、resize）。

---

## 7. 批次 7 — Cascader 复核 + Select / TreeSelect

### 7.1 Cascader
> 已在 `revert/perf-theme-rollback` 之前的会话中**部分手动**还原。本批次做严格复核。
- **Commit**：`04adf2169`
- **复核项**：
  - `Cascader/Themes/CascaderTheme.axaml` ✓
  - `Cascader/Themes/CascaderViewFilterListTheme.axaml` ✓
  - `Cascader/Themes/CascaderViewItemTheme.axaml` ✓
  - `Cascader/Themes/CascaderViewTheme.axaml` ✓
  - `Cascader/Cascader.cs`：检查 `_optionsSnapshot` 已恢复、`HandleCascaderViewItemClicked` 已 no-op、`HandleCascaderViewItemSelected` 单一负责关闭
  - `CascaderView.cs`：检查 `_filterList`/`_emptyIndicatorPresenter` 改回静态查找；`_ignoreSelectedPropertyChanged` 用 try/finally
  - `CascaderView.Filter.cs`：相同
  - `CascaderView.ExpandAndCollapse.cs`：`MaterializeCascaderViewItem` 保留与否按场景需要
  - `CascaderViewItem.cs`：移除 `_toggleCheckbox` 等动态化字段
- **验证**：Gallery `Cascader`（已知 5 个修复用例：单选首次同步、popup 重开、async load、ShowCheckedStrategy、re-click leaf）。

### 7.2 Select（首版）+ Select（响应式）
- **Commits**：`85933d57e`、`d79cb69f5`
- **回滚顺序**：先 `d79cb69f5`，再 `85933d57e`。`d79cb69f5` 还包含 `AbstractSelect` 的 popup 强同步分支，与 8.1 重叠，需注意。
- **回滚 axaml**：
  - `Select/Themes/SelectTheme.axaml`
  - `TreeSelect/Themes/TreeSelectTheme.axaml`
  - `Cascader/Themes/CascaderTheme.axaml`（`d79cb69f5` 顺手改了 Cascader，应该已经在 7.1 复核里恢复）
- **回滚 C#**：
  - `Select/AbstractSelect.cs`（移除 `IsPlayingCloseMotion` / `IgnorePropertyChange = true` 强同步）
  - `Select/Select.cs`
  - `Select/SelectHandle.cs`
  - `Select/SelectTagAwareTextBox.cs`
  - `TreeSelect/TreeSelect.cs`
  - `Cascader/Cascader.cs`（如果还有残留与 `d79cb69f5` 关联的代码）
- **验证**：Gallery `Select`、`TreeSelect`、`Cascader`、`AutoComplete`、`ComboBox`、`Mentions` —— 全部 popup 控件回归一遍。

---

## 8. 批次 8 — 共享框架（最后做）

### 8.1 AbstractSelect popup 强同步
> 已经穿插在 7.2、6.x 中处理，本节做最终核对。
- **文件**：`src/AtomUI.Desktop.Controls/Select/AbstractSelect.cs`
- **核对内容**：
  - `PopupClosed` 不再有 `if (IsDropDownOpen) IgnorePropertyChange = true; SetCurrentValue(IsDropDownOpenProperty, false); …`
  - `OpenDropDown` 不再有 `if (Popup.IsPlayingCloseMotion) Popup.CancelCloseAnimation();`
  - `OpeningDropDown` 与 `ClosingDropDown` 没有新增 cancel 处理
- **验证**：所有依赖 AbstractSelect 的控件二次 popup 回归（Cascader/Select/TreeSelect/ComboBox/AutoComplete/Mentions）。

### 8.2 AddOnDecoratedBox 一族（最终统一）
- **Commits**：`65eb0616e`、`cdb6ed84e`
- **回滚顺序**：先 `cdb6ed84e`（更近），再 `65eb0616e`。最终目标是 axaml 全部回到 `65eb0616e~1`，C# 回到对应版本。
- **回滚 axaml**（9 个）：
  - `Primitives/AddOnDecoratedBox/Themes/AddOnDecoratedBoxTheme.axaml`
  - `ButtonSpinner/Themes/ButtonSpinnerDecoratedBoxTheme.axaml`
  - `Cascader/Themes/CascaderAddOnDecoratedBoxTheme.axaml`
  - `Input/Themes/LineEditTheme.axaml`、`SearchEditDecoratedBoxTheme.axaml`、`SearchEditTheme.axaml`、`TextAreaDecoratedBoxTheme.axaml`、`TextAreaTheme.axaml`
  - `Select/Themes/SelectAddOnDecoratedBoxTheme.axaml`
  - `TreeSelect/Themes/TreeSelectAddOnDecoratedBoxTheme.axaml`
  - `Primitives/InfoPickerInput/Themes/InfoPickerInputTheme.axaml`、`RangeInfoPickerInputTheme.axaml`
  - `DatePicker/Themes/RangeDatePickerTheme.axaml`
  - `ComboBox/Themes/ComboBoxTheme.axaml`
- **回滚 C#**：
  - `Primitives/AddOnDecoratedBox/AddOnDecoratedBox.cs`、`AddOnDecoratedBoxPerfProbe.cs`（review 是否纯 perf 探针；如是，可整文件删除）
  - `Input/LineEdit.cs`、`LineEditAccessoryHost.cs`、`SearchEdit.cs`、`TextArea.cs`、`TextAreaAccessoryHost.cs`
  - `Cascader/CascaderAddOnDecoratedBox.cs`、`Cascader/Cascader.cs`
  - `Select/Select.cs`、`SelectAddOnDecoratedBox.cs`、`SelectAccessoryHost.cs`
  - `TreeSelect/TreeSelect.cs`、`TreeSelectAddOnDecoratedBox.cs`
  - `ComboBox/ComboBox.cs`、`ComboBoxAccessoryHost.cs`、`ComboBoxHandle.cs`
  - `Primitives/InfoPickerInput/InfoPickerInput.cs`、`PickerAccessoryHost.cs`、`PickerClearUpButton.cs`
- **验证**：所有带 AddOn / accessory 的控件全部回归一遍（LineEdit、SearchEdit、TextArea、Cascader、Select、TreeSelect、ComboBox、ButtonSpinner、InfoPicker、NumericUpDown）。

### 8.3 Icon 系统
> 这是整套回滚里风险最大的一段，因为会触发 generated metadata 重新生成。

- **Commits**（按时间从老到新）：`b24f5dfe4` → `50662c49e` → `77364bf3b` → `6df17d7ca` → `df625e0a9`
- **回滚顺序**：从 `df625e0a9` 反向到 `b24f5dfe4`，逐个 revert。
- **回滚 axaml**：
  - `Buttons/Themes/ButtonTheme.axaml`、`ToggleIconButtonTheme.axaml`
  - `Menu/Themes/MenuItemTheme.axaml`
  - `NavMenu/Themes/BaseNavMenuItemHeaderTheme.axaml`、`HorizontalNavMenuItemHeaderTheme.axaml`、`InlineNavMenuItemHeaderTheme.axaml`、`VerticalNavMenuItemHeaderTheme.axaml`
  - `Select/Themes/SelectHandleTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Core/Controls/Icon/IconProviderCache.cs`
  - `src/AtomUI.Icons.AntDesign.Generator/AntDesignGenerator.cs`
  - `src/AtomUI.Icons.AntDesign/AntDesignIcon.cs`
  - `Buttons/Themes/ToggleIconButtonTheme.cs`
  - `Buttons/Button.cs`、`Menu/MenuItem.cs`、`NavMenu/Header/BaseNavMenuItemHeader.cs`、`HorizontalNavMenuItemHeader.cs`、`Select/SelectHandle.cs`
- **重新生成**（必须）：
  ```bash
  dotnet build src/AtomUI.Icons.AntDesign.Generator/AtomUI.Icons.AntDesign.Generator.csproj
  ```
  生成产物可能位于 `src/AtomUI.Icons.AntDesign/Generated/**`，确认 diff 与回滚到的 generator 版本一致后再提交。
- **验证**：所有用到 icon 的控件（覆盖面非常广）。建议跑 Gallery 全量。

### 8.4 Button / DropdownButton（与 Icon 系统耦合）
- **Commit**：`a6648fc17`
- **回滚 axaml**：`Buttons/Themes/ButtonTheme.axaml`、`DropdownButtonTheme.axaml`
- **回滚 C#**：
  - `src/AtomUI.Controls/Primitives/WaveSpiritDecorator.cs`
  - `Buttons/Button.cs`（删 222 行新增的 Ensure 系列）
  - `Buttons/DropdownButton.cs`
- **注意**：`a6648fc17` 改了 PART 命名（`PART_RootLayout` → `PART_FrameLayout` + `PART_ContentLayout`），需要在 Button.cs 里 grep 这两个新 PART 名是否还有引用未清理。
- **验证**：Gallery `Button`、`DropdownButton`、`Wave`（点击波纹动画依赖）。

### 8.5 CompactSpace / Space
- **Commits**：`da827f00d`、`a47a619b8`
- **回滚 axaml**：理论上无 axaml 改动（待复核）。
- **回滚 C#**：`CompactSpace/*.cs`、`Space/*.cs`
- **验证**：Gallery `Space`、所有用到 compact 模式的子组件（Button/Input/Select 组合）。

---

## 9. 收官

1. 整库 `grep` 0.3 节列的关键字残留：
   ```bash
   rg --no-heading "Ensure[A-Z]\w+\(\)" src/
   rg --no-heading "Clear[A-Z]\w+\(\)" src/
   rg --no-heading "_[a-z]\w+Disposables" src/
   rg --no-heading "DisableTransitions\(\)" src/
   rg --no-heading "Dispatcher\.Post\(this\.EnableTransitions\)" src/
   rg --no-heading "SetTemplatedParent\(this\)" src/
   rg --no-heading "using AtomUI\.Reflection;" src/
   ```
   逐个核对是否是回滚遗漏的痕迹（部分关键字在 78bce1f3 之前也存在，注意区分）。
2. **完整 Gallery 走查**：每个 ShowCase 页面手动过一遍。
3. **跑 state-verification 测试套件**：
   ```bash
   dotnet run --project tools/performances/AtomUI.Performance --framework net10.0 -- --suite all --verify
   ```
4. **跑 Gallery performance**（可选，但作为 baseline 留底）：
   ```bash
   dotnet run --project tools/performances/AtomUI.GalleryPerformance --framework net10.0
   ```
5. 最后一笔合并提交：`chore(perf-revert): finalize rollback of perf-driven theme/code changes`，正文里贴本计划文档链接。

---

## 10. 时间预估（粗）

| 批次 | 内容 | 预计时长 |
| --- | --- | --- |
| 1 | 10 个叶子控件 | 0.5 天 |
| 2 | 8 个状态控件 | 0.5 天 |
| 3 | 7 个容器控件 | 0.5 天 |
| 4 | 6 个反馈/通知 | 1 天（Dialog/MessageBox 复杂） |
| 5 | 7 个列表/菜单/标签 | 1 天 |
| 6 | 7 个中段 popup 控件 | 1 天 |
| 7 | Cascader 复核 + Select/TreeSelect | 1 天 |
| 8 | 共享框架（含 Icon 生成） | 1.5 天 |
| 9 | 收官 + 全量回归 | 0.5 天 |
| **合计** | | **~7 天** |

可以按 1→9 顺序串行执行；批次内的控件互相独立，可适度并行。

---

## 11. 历史重写：把 `release/6.0-fixup` 干净地写回 `release/6.0`

### 11.1 目标
- `release/6.0` 最终的 **代码 tree** = 验证通过的 `release/6.0-fixup` HEAD tree。
- `release/6.0` 的 **历史** 不出现"revert(...)"提交；保留窗口内 commit 中"非性能"内容（state verification、scenarios、docs、ShowCase 等）。
- 实现方式：基于 `78bce1f3` 重新构造一段干净的线性历史，强制覆盖原 `release/6.0`。

### 11.2 准备
1. 先 push 现状：`git push origin release/6.0:release/6.0-prerebase`，作为只读历史备份。
2. 通知协作方："`release/6.0` 即将 rewrite，请把本地未推送的工作 stash/拉到 fixup 分支"。
3. 确认 `release/6.0-fixup` 已经跑过：
   - 全量构建
   - Gallery 全 ShowCase 走查
   - `tools/performances/AtomUI.Performance` 全套 state-verification 通过
4. 用 `git diff release/6.0..release/6.0-fixup` 把"最终要保留的总 diff"导出留底：
   ```bash
   git diff release/6.0..release/6.0-fixup > /tmp/fixup-vs-old-release.diff
   ```
   这份 diff 可以作为 review 时和实际 rewrite 后的对照。

### 11.3 推荐方案 A：基于"目标 tree"重建线性历史
> 适合当前场景。最少 commit、最干净，但会丢掉原 commit 的细粒度作者/时间。

1. 创建重写起点：
   ```bash
   git checkout 78bce1f3
   git checkout -b release/6.0-rewrite
   ```
2. 把 `release/6.0-fixup` 的 tree 一次性应用过来：
   ```bash
   git checkout release/6.0-fixup -- .
   ```
3. 按"逻辑分组"分批 stage + commit，比如：
   - `feat(performance): add gallery performance harness` — `tools/performances/**`、`docs/performances/**` 中的纯文档/工具
   - `feat(verification): add state verification suites` — 各控件 `*StateVerification.cs`、`*Scenarios.cs`
   - `feat(controlgallery): refresh showcases for performance scenarios` — Gallery 改动
   - `feat(<Control>): <非性能改动概述>` — 各控件中 78bce1f3..release/6.0 范围内**确实保留**的特性/修复
   - 视情况合并/拆分
4. push：
   ```bash
   git push --force-with-lease origin release/6.0-rewrite:release/6.0
   ```
5. 删除工作分支：
   ```bash
   git branch -D release/6.0-fixup release/6.0-rewrite
   git push origin :release/6.0-fixup   # 如果之前 push 过
   ```

### 11.4 备选方案 B：保留原 commit 形态、逐个去掉性能段
> 适合需要保留细粒度作者历史的场景。代价是手工成本极高（50+ commit）。

1. `git checkout -b release/6.0-rewrite release/6.0`
2. `git rebase -i 78bce1f3` 进入交互式 rebase。
3. 对每个性能 commit：
   - 标记为 `edit`
   - rebase 暂停后用 `git reset HEAD~`，把改动放回工作区
   - 删除性能段（axaml + 配套 C#），保留其它
   - 检查是否还有 effective diff；有则 `git commit --amend --no-edit`，否则 `git rebase --skip`
4. 整套完成后用 11.2.4 的 diff 文件比对最终 tree 是否一致。
5. force-push 同 11.3.4。

### 11.5 推荐方案 C：用 `git filter-repo` 批量过滤
> 适合 50+ commit、且性能改动具有明确路径模式时。需要 `pip install git-filter-repo`。

1. 准备一份"性能 commit 路径白名单"，例如把所有 `Themes/*.axaml`、性能改动涉及的 `*.cs` 路径列入。
2. 用 `git filter-repo --path-glob` 反向过滤，移除每个 commit 中的对应 hunk。
3. 风险：filter-repo 是按文件粒度，无法精细到 hunk；只适合"整文件可丢"或"整文件可保留"的场景。
4. 本仓库情况下不建议作为主路径，更适合 11.3 之外的修补手段。

### 11.6 验证 rewrite 是否正确

1. 构建：
   ```bash
   git checkout release/6.0          # 重写后
   git submodule update --recursive
   dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj
   ```
2. tree 比对：
   ```bash
   git diff release/6.0 release/6.0-prerebase  # 与备份对比，应只剩"性能改动"的反向 diff
   ```
3. **`tree`-级一致性检查**（最关键的一步）：
   ```bash
   git diff release/6.0-fixup release/6.0  # 应为空
   ```
   如果有差异，说明 rewrite 漏掉/多带了内容，回到 `release/6.0-prerebase` 重做。
4. Gallery 全量走查 + state verification 套件再跑一次。
5. 如果全部通过：
   ```bash
   git push --force-with-lease origin release/6.0
   git push origin --delete release/6.0-prerebase  # 完全确认无误后再删除备份
   ```

### 11.7 团队协作注意事项
- Force push 之后，所有同事必须执行：
  ```bash
  git fetch origin
  git checkout release/6.0
  git reset --hard origin/release/6.0
  ```
  或重新 clone。否则会产生"幽灵 merge"。
- 如果有未合并的 PR 基于旧 `release/6.0`，需要 rebase 到新 `release/6.0`。
- 如果有 tag 指向窗口内的 commit（`v6.x` 的预发版），保留旧 tag 不动，但说明它们对应的是 prerebase 历史。
- CI 上的缓存（dotnet build cache、NuGet restore cache）通常需要清一次。
