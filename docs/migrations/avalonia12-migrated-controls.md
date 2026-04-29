# Avalonia 12 已迁移控件列表

跟踪 AtomUI 各控件模块的 Avalonia 12 迁移状态。控件清单来源于 `release/5.0` 分支。

## 迁移状态说明

| 状态 | 说明 |
|------|------|
| 已完成 | 已扫描全部 50+ 类别，修复所有问题，构建通过 |
| 进行中 | 部分迁移，仍有待修复项 |
| 待迁移 | 尚未开始 |

## AtomUI.Controls

| 控件 | 状态 | 修改内容 | 日期 |
|------|------|----------|------|
| Avatar | 待迁移 | | |
| Badge | 待迁移 | | |
| Buttons | 待迁移 | | |
| CheckBox | 待迁移 | | |
| Embedding | 待迁移 | | |
| Empty | 待迁移 | | |
| FlexPanel | 待迁移 | | |
| FloatButton | 待迁移 | | |
| Form | 待迁移 | | |
| Grid | 待迁移 | | |
| Icon | 待迁移 | | |
| ItemsControl | 待迁移 | | |
| MarqueeLabel | 待迁移 | | |
| OptionButtonGroup | 待迁移 | | |
| Primitives | 待迁移 | | |
| ProgressBar | 待迁移 | | |
| QRCode | 待迁移 | | |
| RadioButton | 待迁移 | | |
| Rate | 待迁移 | | |
| Result | 待迁移 | | |
| ScrollViewer | 已完成 | `Dispatcher.UIThread` → `this.Dispatcher`；`Root` → `Root.GetRootElement()` 修复 PresentationSource 比较 | 2026-04-29 |
| Segmented | 待迁移 | | |
| Separator | 待迁移 | | |
| Spin | 待迁移 | | |
| Switch | 待迁移 | | |
| Tag | 待迁移 | | |
| Timeline | 待迁移 | | |
| TransitioningContentControl | 待迁移 | | |
| Watermark | 待迁移 | | |

## AtomUI.Desktop.Controls

| 控件 | 状态 | 修改内容 | 日期 |
|------|------|----------|------|
| AdornerLayer | 待迁移 | | |
| Alert | 待迁移 | | |
| AutoComplete | 待迁移 | | |
| Avatar | 待迁移 | | |
| Badge | 待迁移 | | |
| Breadcrumb | 已完成 | 无需修改，已兼容 Avalonia 12 | 2026-04-29 |
| ButtonSpinner | 待迁移 | | |
| Buttons | 待迁移 | | |
| Calendar | 待迁移 | | |
| Card | 待迁移 | | |
| Carousel | 待迁移 | | |
| Cascader | 待迁移 | | |
| CheckBox | 待迁移 | | |
| Chrome | 待迁移 | | |
| Collapse | 待迁移 | | |
| ComboBox | 待迁移 | | |
| DatePicker | 待迁移 | | |
| Descriptions | 待迁移 | | |
| Dialog | 待迁移 | | |
| Drawer | 待迁移 | | |
| Empty | 待迁移 | | |
| Expander | 待迁移 | | |
| FloatButton | 待迁移 | | |
| Flyouts | 待迁移 | | |
| Form | 待迁移 | | |
| GroupBox | 待迁移 | | |
| HeaderedContentControl | 待迁移 | | |
| ImagePreviewer | 待迁移 | | |
| Input | 待迁移 | | |
| ListBox | 待迁移 | | |
| ListView | 待迁移 | | |
| MarqueeLabel | 待迁移 | | |
| Mentions | 待迁移 | | |
| Menu | 待迁移 | | |
| Message | 待迁移 | | |
| MessageBox | 待迁移 | | |
| NavMenu | 待迁移 | | |
| Notifications | 待迁移 | | |
| NumericUpDown | 待迁移 | | |
| OptionButtonGroup | 待迁移 | | |
| Pagination | 待迁移 | | |
| Popup | 待迁移 | | |
| PopupConfirm | 待迁移 | | |
| Primitives | 待迁移 | | |
| ProgressBar | 待迁移 | | |
| QRCode | 待迁移 | | |
| RadioButton | 待迁移 | | |
| Rate | 待迁移 | | |
| Result | 待迁移 | | |
| ScrollViewer | 已完成 | 无需修改，已兼容 Avalonia 12 | 2026-04-29 |
| Segmented | 待迁移 | | |
| Select | 待迁移 | | |
| Separator | 待迁移 | | |
| Skeleton | 待迁移 | | |
| Slider | 待迁移 | | |
| Space | 待迁移 | | |
| Spin | 待迁移 | | |
| SplitView | 待迁移 | | |
| Splitter | 待迁移 | | |
| Statistic | 待迁移 | | |
| Steps | 待迁移 | | |
| Switch | 待迁移 | | |
| TabControl | 待迁移 | | |
| Tag | 待迁移 | | |
| TextBlock | 已完成 | `Dispatcher.UIThread` → `this.Dispatcher` (HyperLinkTextBlock) | 2026-04-29 |
| TimePicker | 待迁移 | | |
| Timeline | 待迁移 | | |
| Tooltip | 待迁移 | | |
| Tour | 待迁移 | | |
| Transfer | 待迁移 | | |
| TreeSelect | 待迁移 | | |
| TreeView | 待迁移 | | |
| Upload | 待迁移 | | |
| Window | 待迁移 | | |

## AtomUI.Desktop.Controls.DataGrid

| 控件 | 状态 | 修改内容 | 日期 |
|------|------|----------|------|
| DataGrid | 待迁移 | | |

## AtomUI.Desktop.Controls.ColorPicker

| 控件 | 状态 | 修改内容 | 日期 |
|------|------|----------|------|
| ColorPicker | 待迁移 | | |
