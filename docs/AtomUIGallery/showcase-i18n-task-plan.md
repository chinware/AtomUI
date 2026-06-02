# AtomUI Gallery ShowCase 多语言任务计划

本文档记录 AtomUI Gallery ShowCase 多语言完善工作的代码分析结果和实施计划。当前计划基于 `ShowCases/<Category>/<ShowCase>/Views|ViewModels|Localization` 的最新组织规范。

## 当前进度

| 项 | 结果 |
| --- | ---: |
| ShowCase 总数 | 71 |
| 已有 ShowCase 本地多语言 | 69 个，P1 标题/描述迁移全部完成；`Icon`、`Palette` 无 `ShowCaseItem` 标题/描述，P1 无需新增 provider |
| 仍有硬编码 `ShowCaseItem.Title` / `Description` 的 ShowCase | 0 个 |
| 待迁移 `ShowCaseItem.Title` | 0 处 |
| 待迁移 `ShowCaseItem.Description` | 0 处 |
| 已存在共享场景语言 | `ShowCaseScenarioLang`，主要用于 `Form`、`DataGrid`、`Space` 的 tab 标题 |

当前所有 `ShowCaseItem.Title` / `Description` 均已按 ShowCase 本地 `Localization/en_US.cs` 和 `Localization/zh_CN.cs` 组织；`Icon`、`Palette` 没有 `ShowCaseItem` 标题和描述，P1 不需要迁移。P2 的控件内部按钮文字、占位符、示例数据等可见文案仍按后续阶段推进。

## 分类工作量

| 分类 | ShowCase 数 | 已有本地多语言 | 硬编码标题/描述迁移量 | 优先级 |
| --- | ---: | ---: | ---: | --- |
| DataEntry | 18 | 18 | 0 / 0 | 已完成 P1 |
| DataDisplay | 22 | 22 | 0 / 0 | 已完成 P1 |
| Navigation | 8 | 8 | 0 / 0 | 已完成 P1 |
| Feedback | 11 | 11 | 0 / 0 | 已完成 P1 |
| Layout | 4 | 4 | 0 / 0 | 已完成 P1 |
| General | 8 | 6 | 0 / 0 | 已完成 P1，`Icon` / `Palette` 无 P1 迁移项 |

## 实施计划

| 阶段 | 范围 | 任务 | 产出 | 验证 |
| --- | --- | --- | --- | --- |
| P0 | 多语言契约 | 统一 `LanguageId`、资源类命名、资源 key 命名、XAML 引用方式 | 可复用迁移模板 | 选择 1-2 个 ShowCase 验证资源生成正常 |
| P1 | ShowCase 标题和描述 | 迁移所有 `ShowCaseItem.Title` / `Description` | 每个 ShowCase 拥有自己的 `Localization/en_US.cs` 和 `Localization/zh_CN.cs` | 确认不再存在硬编码 `ShowCaseItem` 标题和描述 |
| P2 | 主要可见演示文案 | 迁移明显展示给用户的 `Content`、`Header`、`HeaderTitle`、`Watermark`、按钮文字等 | Gallery 主体演示文案可随语言切换 | 中英文切换抽查重点页面 |
| P3 | 复杂 ShowCase | 处理 `Form`、`DataGrid`、`Space` 的子场景页面和局部辅助控件 | 子场景文案跟随对应 ShowCase 就近维护 | 验证 lazy tab 首次加载、切换和语言显示 |
| P4 | 共享资源收敛 | 评估 `ShowCaseScenarioLang` 是否保留为公共场景名资源，或拆入本地资源 | 避免共享资源持续膨胀 | 编译和资源生成检查 |
| P5 | 回归验证 | Gallery Debug build、运行抽查、语言切换验证 | 可提交的稳定批次 | `dotnet build` 通过，关键页面无空资源或错位文案 |

## 建议批次

| 批次 | 内容 | 目的 |
| --- | --- | --- |
| Batch 1 | `Button`、`AutoComplete`、`DataGrid` | 覆盖简单 ShowCase、输入类 ShowCase、复杂 tab ShowCase，先验证迁移模式 |
| Batch 2 | `DataEntry` 全部 | 文案量最大，优先完成高收益区域 |
| Batch 3 | `DataDisplay` 全部 | ShowCase 数量最多，和 `DataGrid` 场景相关 |
| Batch 4 | `Navigation` 和 `Feedback` | 已完成中等规模迁移 |
| Batch 5 | `Layout` 和 `General` 剩余项 | 已完成收尾，保留并复用 `AboutUs` 已有模式 |

## 执行进度

| ShowCase | 分类 | 状态 | 已完成内容 | 验证 |
| --- | --- | --- | --- | --- |
| Button | General | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 9 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| AutoComplete | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 9 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| DataGrid | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 22 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Avatar | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 4 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Badge | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 10 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Calendar | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 1 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Card | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 10 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Carousel | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 6 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Collapse | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 10 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Descriptions | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 7 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Empty | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 4 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Expander | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 10 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| GroupBox | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 4 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| ImagePreviewer | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 5 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| InfoFlyout | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 4 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| List | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 11 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| QRCode | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 8 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Segmented | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 6 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Statistic | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 5 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Tag | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 5 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Timeline | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 6 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Tooltip | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 4 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Tour | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 7 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| TreeView | DataDisplay | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 10 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Cascader | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 20 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| CheckBox | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 6 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| ColorPicker | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 11 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| DatePicker | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 9 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Form | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 20 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| LineEdit | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 16 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Mentions | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 9 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| NumberUpDown | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 13 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| RadioButton | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 10 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Rate | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 6 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Select | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 14 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Slider | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 4 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| TimePicker | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 9 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| ToggleSwitch | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 5 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Transfer | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 7 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| TreeSelect | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 11 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Upload | DataEntry | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 10 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Breadcrumb | Navigation | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 6 个 `ShowCaseItem.Title` 和 5 个 `Description` | Debug build 通过 |
| ButtonSpinner | Navigation | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 7 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| ComboBox | Navigation | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 8 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| DropdownButton | Navigation | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 4 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Menu | Navigation | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 15 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Pagination | Navigation | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 7 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Steps | Navigation | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 14 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| TabControl | Navigation | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 25 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Alert | Feedback | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 7 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Drawer | Feedback | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 7 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Message | Feedback | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 4 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Modal | Feedback | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 9 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Notification | Feedback | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 6 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| PopupConfirm | Feedback | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 4 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| ProgressBar | Feedback | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 19 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Result | Feedback | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 8 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Skeleton | Feedback | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 5 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Spin | Feedback | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 5 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Watermark | Feedback | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 4 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| CustomizeTheme | General | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 4 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| FloatButton | General | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 11 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Separator | General | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 6 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| SplitButton | General | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 5 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| FlexPanel | Layout | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 11 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Grid | Layout | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 8 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Space | Layout | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 9 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |
| Splitter | Layout | 已完成 P1 | 新增本地 `en_US` / `zh_CN` provider，迁移 7 组 `ShowCaseItem.Title` / `Description` | Debug build 通过 |

## 资源命名规则

| 类型 | 命名示例 | 说明 |
| --- | --- | --- |
| 基础标题 | `BasicTitle` | 对应 `ShowCaseItem.Title` |
| 基础描述 | `BasicDescription` | 对应 `ShowCaseItem.Description` |
| 语义场景标题 | `CustomIconTitle` | 同一 ShowCase 内有多个相近场景时使用语义名 |
| 语义场景描述 | `CustomIconDescription` | 与标题保持前缀一致 |
| 控件内文案 | `SubmitButtonText`、`UserNameWatermark` | 按控件用途命名，避免只用 `Text1` 这类无意义 key |

## 验收标准

| 项 | 标准 |
| --- | --- |
| 本地化文件 | 每个包含可见演示文案的 ShowCase 都有本地 `Localization/en_US.cs` 和 `Localization/zh_CN.cs` |
| 标题和描述 | 不再存在硬编码的 `ShowCaseItem.Title` / `Description`，除非该文本是刻意展示的代码或数据内容 |
| 主要演示文案 | 用户可见的主要标题、按钮、输入提示、分组标题支持语言切换 |
| 生成资源 | `LanguageResourceConst.g.cs` 能生成所有新增资源扩展和资源 kind |
| 编译 | `AtomUIGallery.Desktop` Debug build 通过 |
| 行为 | `Form`、`DataGrid`、`Space` 等 lazy tab 页面首次打开正常，不因多语言迁移引入延迟创建行为问题 |

## 边界和风险

| 项 | 处理方式 |
| --- | --- |
| 左侧菜单 | 不纳入本轮 ShowCase 本地多语言范围，菜单已有独立资源体系 |
| 代码示例和 mock 数据 | 初期不强制翻译，避免破坏示例语义或测试数据 |
| 公共场景名 | `ShowCaseScenarioLang` 暂时保留，后续按实际复用价值决定是否拆分 |
| 未提交改动 | 实施时避免把工作区已有的无关改动混入多语言提交 |
| 文案质量 | 英文和中文都需要按 UI 场景表达，不做机械直译 |
