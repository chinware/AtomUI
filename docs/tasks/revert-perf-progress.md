# 回滚进度跟踪 — 性能优化引入的 ControlTheme 改动

> 配套计划：[revert-perf-controltheme-rollback-plan.md](./revert-perf-controltheme-rollback-plan.md)
>
> 工作分支：`release/6.0-fixup`
>
> 状态图例：☐ 未开始 / ◐ 进行中 / ☑ 完成 / ⏭ 跳过 / ⚠ 阻塞
>
> 每完成一个条目就把对应的 ☐ 改为 ☑，同时在"备注"列写下 commit sha、遇到的问题或验证结果。

## 总览

| 批次 | 范围 | 进度 | 状态 |
| --- | --- | --- | --- |
| 0 | 准备工作（分支、备份） | 3/3 | ☑ |
| 1 | 叶子静态控件（10 个） | 10/10 | ☑ |
| 2 | 状态控件（8 个） | 8/8 | ☑ |
| 3 | 容器控件（8 个） | 8/8 | ☑ |
| 4 | 反馈/通知（6 个） | 6/6 | ☑ |
| 5 | 列表/菜单/标签（7 个） | 7/7 | ☑ |
| 6 | 中段 popup 控件（7 个） | 7/7 | ☑ |
| 7 | Cascader 复核 + Select / TreeSelect | 3/3 | ☑ |
| 8 | 共享框架（5 项） | 5/5 | ☑ |
| 9 | 收官全量回归 | 3/5 | ◐ |
| 11 | 历史重写 + push | 0/7 | ☐ |
| **合计** | | **0/69** | |

---

## 批次 0 — 准备工作

| # | 状态 | 任务 | 备注 |
| --- | --- | --- | --- |
| 0.1 | ☑ | `git checkout -b release/6.0-fixup release/6.0` | 已存在并就位 |
| 0.2 | ☑ | 本地备份 `release/6.0-prerebase` | 已 `git branch release/6.0-prerebase release/6.0` |
| 0.3 | ☑ | 通知协作者 release/6.0 即将 rewrite | 用户已知悉 |

---

## 批次 1 — 叶子静态控件

| # | 状态 | 控件 | Commit | 验证 ShowCase | 备注 |
| --- | --- | --- | --- | --- | --- |
| 1.1 | ☑ | Empty | `41d572ae0` | `Empty`、`Result` | revert `bc02c9350` — theme 仅空格、C# 整体 revert，build 通过 |
| 1.2 | ☑ | Avatar / AvatarGroup | `d75d54192` | `Avatar`（maxCount/tooltip/size） | revert `3a617c2a1` — build 通过 |
| 1.3 | ☑ | Badge | `3bd15ccac` | `Badge`、`Notification` 间接 | revert `10ee9cbb7` — build 通过 |
| 1.4 | ☑ | Result | `755145557` | `Result`（成功/失败/警告/自定义） | revert `4c4c23030` — build 通过 |
| 1.5 | ☑ | QRCode | `04710c1f2` | `QRCode`（带/不带 logo） | revert `b2219fd1e` — build 通过 |
| 1.6 | ☑ | Descriptions | `34d6a0172` | `Descriptions`（横纵/border/嵌套） | revert `ab44e23e5` — build 通过 |
| 1.7 | ☑ | Statistic + Skeleton 同步 | `a13538490` | `Statistic`、`TimerStatistic`、`Skeleton` | revert `39a8f451a` — build 通过；3.4 Skeleton 时复核 |
| 1.8 | ☑ | Steps | `2bd55af39` | `Steps`（horizontal/vertical/dot/icon） | revert `ca1a02eca` — build 通过 |
| 1.9 | ☑ | SplitView | `e6d9c45bc` | `SplitView`（pane/display mode） | revert `ec7119e54` — build 通过 |
| 1.10 | ☑ | Splitter | `3137a536b` | `Splitter`（拖动、min/max、reverse） | revert `bb03a8d24` — build 通过 |

> 批次 1 收尾：☐ 全量构建 / ☐ Gallery 烟囱 / ☐ grep 残留 / ☐ tidy 提交

---

## 批次 2 — 状态控件

| # | 状态 | 控件 | Commit | 验证 ShowCase | 备注 |
| --- | --- | --- | --- | --- | --- |
| 2.1 | ☑ | ToggleSwitch | `5a04a5b2e` + `31663962a` | `ToggleSwitch`（loading/checked/disabled） | revert `500cf4e80` — build 通过，两次合并到 5a04a5b2e~1 |
| 2.2 | ☑ | RadioButton / RadioIndicator | `6d69c8ae6` | `RadioButton`（单选 group/disabled/size） | revert `9f5e70e7d` — build 通过 |
| 2.3 | ☑ | CheckBox | `9a5e7c761` + `17fc74662` | `CheckBox`、`CheckBoxGroup`、级联 `Cascader` 多选 | revert `b62c1fed7` — Converter 已 restore；a91ea4ade 是 verification-only 保留 |
| 2.4 | ☑ | Rate | `3de9cfcbc` | `Rate`（默认/半星/自定义/颜色） | revert `564c37e23` — build 通过 |
| 2.5 | ☑ | Segmented | `884d21ee6` | `Segmented`（icon/label/disabled/size） | revert `cb37a0d01` — build 通过 |
| 2.6 | ☑ | OptionButtonGroup | `71cb534c8` | `OptionButtonGroup`、`Cascader > Placement` | revert `ec2cc1182` — build 通过 |
| 2.7 | ☑ | Slider | `089e6d114` | `Slider`（mark/tooltip/range/disabled） | revert `01a8fe78c` — build 通过 |
| 2.8 | ☑ | ProgressBar 一族 | `2fb2739a5` | `ProgressBar`（line/circle/dashboard/steps） | revert `1e696eaf1` — 7 个 C# 文件 + 2 axaml，build 通过 |

> 批次 2 收尾：☐ 全量构建 / ☐ Gallery 烟囱 / ☐ tidy 提交

---

## 批次 3 — 容器控件

| # | 状态 | 控件 | Commit | 验证 ShowCase | 备注 |
| --- | --- | --- | --- | --- | --- |
| 3.1 | ☑ | Card / CardGridItem | `abf7006e4` | `Card`（actions/grid/cover） | revert `2598cbb44` — build 通过 |
| 3.2 | ☑ | Collapse | `52e5c7c53` | `Collapse`（borderless/accordion/ghost/icon） | revert `74f093b5f` — build 通过 |
| 3.3 | ☑ | Carousel | `05e5a88ef` | `Carousel`（autoplay/nav/indicator） | revert `13a23b8e1` — build 通过 |
| 3.4 | ☑ | Skeleton | `35f26975c` | `Skeleton`、`Separator`、`Statistic` | revert `a6b72ca03` — Statistic 1.7 已处理部分文件，本次进一步还原 |
| 3.5 | ☑ | Spin | `fbe3b1db9` | `Spin`、`Mentions`、`Cascader` async | revert `95960f9df` — build 通过 |
| 3.6 | ☑ | Drawer | `bd3d3f85a` | `Drawer`（4 方向/size/mask） | revert `ff8b6603b` — build 通过 |
| 3.7 | ☑ | Expander | `5cb566813` | `Expander`（多 placement/嵌套） | revert `66dceb32c` — build 通过 |
| 3.8 | ☑ | Pagination + PopupConfirm | `28542036f` | `Pagination`、`PopupConfirm` | revert `35eb7daef` — 5 axaml + 6 C# build 通过 |

> 批次 3 收尾：☐ 全量构建 / ☐ Gallery 烟囱 / ☐ tidy 提交

---

## 批次 4 — 反馈 / 通知

| # | 状态 | 控件 | Commit | 验证 ShowCase | 备注 |
| --- | --- | --- | --- | --- | --- |
| 4.1 | ☑ | Alert | `cdad8c780` | `Alert`（marquee/closable/icon） | revert `a40900acf` — build 通过 |
| 4.2 | ☑ | Notification | `9f817fd0e` | `Notification`（success/info/warn/error/auto） | revert `9678894a3` — Converter 已 restore，build 通过 |
| 4.3 | ☑ | Message | `57836720b` | `Message`、Form `MessageBox` 调用 | revert `70f1c6f0b` — build 通过 |
| 4.4 | ☑ | Dialog + MessageBox | `83547f32d` + `ff076f72b` | `Dialog`、`MessageBox` | revert `80cfd16ed` — Converter restore + 删 LoadingContentPresenter 文件 |
| 4.5 | ☑ | Flyout | `d4097343d` | `Flyout`、`Tooltip` | revert `3ac32da69` — 无 axaml 改动，纯 C# revert |
| 4.6 | ☑ | ImagePreviewer + GroupBox | `652073adf` | `Image` / `ImagePreviewer`（toolbar/缩放/旋转） | revert `a49383c27` — 10+ C# 文件 |

> 批次 4 收尾：☐ 全量构建 / ☐ Gallery 烟囱 / ☐ tidy 提交

---

## 批次 5 — 列表 / 菜单 / 标签

| # | 状态 | 控件 | Commit | 验证 ShowCase | 备注 |
| --- | --- | --- | --- | --- | --- |
| 5.1 | ☑ | ListBox / ListView | `512f12bca` + `a2220c3d9` | `ListBox`、`ListView`（paginate/filter/indicator） | revert `a1b9643f3` — 含 ListCollectionView，amend 一次补全；fix `39236ab71` 单独恢复 OnLoaded 过滤刷新 |
| 5.2 | ☑ | Menu / MenuItem | `59be2268e` | `Menu`、`ContextMenu`、TopLevel hover | revert `8c284a7f1` — 手工保留 1aadb96c 的 hover transition |
| 5.3 | ☑ | NavMenu | `0c74cc485` | `NavMenu`（horizontal/inline/vertical/submenu） | revert `04974aaa7` — build 通过 |
| 5.4 | ☑ | SelectCandidateList | `1e931e24f` | `Mentions`、`AutoComplete` | revert `eedb0ad1c` — 仅 1 个 C# 文件；fix `30e1ccf6d` 单独恢复 filter 状态下 visible-item 选择映射 |
| 5.5 | ☑ | TabControl / TabStrip | `e17cd490c` | `TabControl`（line/card/closable）、`TabStrip` | revert `8f66c9eaa` — 9 个 C# 文件 + 4 axaml |
| 5.6 | ☑ | FloatButton | `e9dd1a2f6` | `FloatButton`（group/shape/tooltip/BackTop） | revert `d0434f583` — 10 个 C# 文件 + 4 axaml |
| 5.7 | ☑ | ScrollViewer | `e94eb1d64` | 任意带滚动的页面 | revert `635269007` — 改动很少 |

> 批次 5 收尾：☐ 全量构建 / ☐ Gallery 烟囱 / ☐ tidy 提交

---

## 批次 6 — 中段 popup 控件

| # | 状态 | 控件 | Commit | 验证 ShowCase | 备注 |
| --- | --- | --- | --- | --- | --- |
| 6.1 | ☑ | AutoComplete | `4f87a97c9` | `AutoComplete`（textarea/searchedit/render） | revert `a4c4954fd` — build 通过 |
| 6.2 | ☑ | Mentions | `94cafd5e4` | `Mentions`（异步 candidate） | revert `5ac08f490` — build 通过 |
| 6.3 | ☑ | ComboBox | `80526c3bb` | `ComboBox`（filter/disabled/custom） | revert `151122808` — `65eb0616e` 留 8.2 |
| 6.4 | ☑ | DatePicker / RangeDatePicker | `db89536bd` | `DatePicker`、`RangeDatePicker` | revert `01d793f12` — InfoPicker 8.2 再覆盖 |
| 6.5 | ☑ | NumericUpDown / ButtonSpinner | `2911b1182` + `b1973674e` | `NumericUpDown`、`ButtonSpinner` | revert `c4a9789e0` — 合并到 b1973674e~1 |
| 6.6 | ☑ | Form | `8f581b371` | `Form`（validation/layout/submit） | revert `98daf67bd` — 无 axaml，仅 C# |
| 6.7 | ☑ | Input / TextArea / TextBox | `25c2c3d8f` | `Input`、`TextArea`、`SearchEdit` | revert `66f8ba5f4` — 11 个文件，部分文件 8.2 再覆盖 |

> 批次 6 收尾：☐ 全量构建 / ☐ Gallery 烟囱 / ☐ tidy 提交

---

## 批次 7 — Cascader 复核 + Select / TreeSelect

| # | 状态 | 控件 | Commit | 验证 ShowCase | 备注 |
| --- | --- | --- | --- | --- | --- |
| 7.1 | ☑ | Cascader 严格复核 | `04adf2169` | `Cascader` 5 个已知用例 | revert `be9316be8` — 整目录 reset 到 04adf2169~1 |
| 7.2 | ☑ | Select 一族 | `85933d57e` + `d79cb69f5` | `Select`、`TreeSelect`、`Cascader` | revert `84506d188` — 合并到 d79cb69f5~1，AbstractSelect popup 强同步段已移除 |
| 7.3 | ☑ | 全 popup 控件回归一遍 | — | Cascader/Select/TreeSelect/AutoComplete/ComboBox/Mentions | 各控件 commit 时已逐一 build 通过；待 8.x 完成后再做完整 Gallery 回归 |

> 批次 7 收尾：☐ 全量构建 / ☐ Gallery popup 控件全量走查

---

## 批次 8 — 共享框架（最后做）

| # | 状态 | 项目 | 涉及 Commit | 验证范围 | 备注 |
| --- | --- | --- | --- | --- | --- |
| 8.1 | ☑ | AbstractSelect popup 强同步终核 | `d79cb69f5` 等 | 所有 popup 控件 | 已在 7.2 通过；PopupClosed/OpenDropDown 干净 |
| 8.2 | ☑ | AddOnDecoratedBox 一族 | `cdb6ed84e` + `65eb0616e` | LineEdit/SearchEdit/TextArea/Cascader/Select/TreeSelect/ComboBox/ButtonSpinner/InfoPicker/NumericUpDown | revert `2ad0808e8` — 合并到 cdb6ed84e~1，删除 6 个 AccessoryHost/PerfProbe 文件 |
| 8.3 | ☑ | Icon 系统 + metadata 重生成 | `b24f5dfe4` → `df625e0a9`（5 个） | 所有用 Icon 的控件 | revert `78242ff88` — 863 个文件，含 GeneratedIcons 全量回退；df625e0a9 仅 gallery 跳过 |
| 8.4 | ☑ | Button / DropdownButton | `a6648fc17` | `Button`、`DropdownButton`、`Wave` | revert `25f12ff0f` — 与 8.3 配套，b24f5dfe4~1 |
| 8.5 | ☑ | CompactSpace / Space | `da827f00d` + `a47a619b8` | `Space`、所有 compact 组合 | revert `d3d16e9f0` — 无 axaml，仅 C#，合并到 da827f00d~1 |

> 批次 8 收尾：☐ 全量构建 / ☐ Gallery 全量走查

---

## 批次 9 — 收官全量回归

| # | 状态 | 项目 | 备注 |
| --- | --- | --- | --- |
| 9.1 | ☑ | 整库 grep 残留扫描（计划 9 节列的关键字） | 检查通过：`DisableTransitions/EnableTransitions` 清零；`_xxxDisposables` 与 `AtomUI.Reflection` 仅剩预先存在的合法用法 |
| 9.2 | ☐ | Gallery 全 ShowCase 手动走查 | 待用户验证 |
| 9.3 | ☐ | state-verification 套件 `tools/performances/AtomUI.Performance --suite all --verify` | 待用户运行 |
| 9.4 | ☐ | GalleryPerformance baseline（可选） | 待用户运行（可选） |
| 9.5 | ☑ | 最终全量构建 | AtomUI.Desktop.Controls + DataGrid 都通过；DataGrid 仅有 1 个非本次的 CS0169 warning |

---

## 批次 11 — 历史重写

| # | 状态 | 项目 | 备注 |
| --- | --- | --- | --- |
| 11.1 | ☐ | 导出对照 diff `git diff release/6.0..release/6.0-fixup > /tmp/fixup-vs-old-release.diff` | |
| 11.2 | ☐ | 创建重写起点 `git checkout -b release/6.0-rewrite 78bce1f3` | |
| 11.3 | ☐ | 把 fixup tree 应用过来 `git checkout release/6.0-fixup -- .` | |
| 11.4 | ☐ | 按逻辑分组分批 stage + commit（performance harness / verification / showcases / per-control） | |
| 11.5 | ☐ | tree 一致性检查 `git diff release/6.0-fixup release/6.0-rewrite` 应为空 | 关键步骤 |
| 11.6 | ☐ | force push `git push --force-with-lease origin release/6.0-rewrite:release/6.0` | |
| 11.7 | ☐ | 清理工作分支 + 删除备份分支 + 通知协作者 reset | |

---

## 进度更新规约

每完成一个条目，按以下格式更新该行：
1. 把 ☐ 改成 ☑。
2. 在"备注"列填入：
   - 实际 commit sha（前 7 位）
   - 验证记录（哪几个 ShowCase 跑过、有无异常）
   - 任何偏离计划的处理
3. 如果遇到阻塞，把 ☐ 改成 ⚠，备注里写阻塞原因。
4. 如果决定跳过（例如 commit 不需回滚），改成 ⏭ 并备注理由。
5. 每完成一个**批次**，更新顶部"总览"表的进度数字与状态。

> 例：完成 1.1 Empty 后改为 `| 1.1 | ☑ | Empty | 41d572ae0 | Empty/Result 已通过 | restored axaml + 删 Ensure/Clear |`。
