# SearchEdit 桌面版实现原理

本文档描述 SearchEdit 桌面版的内部模板组合、搜索按钮事件流、按钮与输入框布局、状态传递和维护不变量。公共设计与 API 契约见 [SearchEdit 桌面版架构设计](overview.md)，LineEdit 输入家族 Token 语义见 [LineEdit Token 设计](../line-edit/token.md)，变化记录见 [SearchEdit Changelog](changelog.md)。

## 1. 实现定位

SearchEdit 的实现以 `LineEdit` 为文本输入内核，AtomUI 在主题和少量内部类中加入搜索按钮、按钮状态传递和一体化布局。实现文档聚焦 SearchEdit 增强层，不重新说明 LineEdit 的文本编辑、清除按钮、Form、CompactSpace 和 TextPresenter 细节。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/Input/SearchEdit.cs`：public API、默认 clear icon、模板接入和搜索点击事件抛出。
- `src/AtomUI.Desktop.Controls/Input/SearchEditDecoratedBox.cs`：内部输入壳体，转接 SearchEdit 属性并订阅搜索按钮 click。
- `src/AtomUI.Desktop.Controls/Input/SearchButton.cs`：搜索按钮类型，继承 Button 并接入输入 `StyleVariant`、`Status`。
- `src/AtomUI.Desktop.Controls/Input/SearchEditPanel.cs`：搜索输入布局面板，负责左侧 AddOn、搜索按钮和内容框重叠边框排布。
- `src/AtomUI.Desktop.Controls/Input/Themes/SearchEditTheme.axaml`：SearchEdit 根模板、文本区域、内部 action 和 focus/status selector。
- `src/AtomUI.Desktop.Controls/Input/Themes/SearchEditDecoratedBoxTheme.axaml`：搜索按钮和输入框一体化模板。
- `src/AtomUI.Desktop.Controls/Input/Themes/SearchButtonTheme.axaml`：搜索按钮在不同输入表面下的状态视觉。
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteSearchEdit.cs`：AutoComplete 搜索输入入口，复用 SearchEdit 搜索按钮属性。
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteSearchEditBox.cs`：AutoComplete 内部 SearchEdit box，使用 SearchEdit style key。

## 3. 核心类职责

`SearchEdit` 只增加搜索相关契约。它不重写 LineEdit 文本输入算法，也不管理搜索任务。`NotifySearchButtonClicked()` 是内部事件出口，负责在 `IsOperating=false` 时抛出 `SearchButtonClick`。

`SearchEditDecoratedBox` 是 SearchEdit 根模板中的输入壳体。它继承 `AddOnDecoratedBox`，复用输入边框、圆角、状态和 CompactSpace 计算，并把 `SearchButtonStyle`、`SearchButtonText`、`IsSearchButtonLoading` 传给内部搜索按钮。

`SearchButton` 是 SearchEdit 专用按钮。它继承 AtomUI Button 的尺寸、loading、variant、icon 和 wave 能力，并实现输入状态接口，使主题 selector 可以按 `StyleVariant` 与 `Status` 调整按钮视觉。

`SearchEditPanel` 是三段布局面板。它把左侧 AddOn 放在左侧，把搜索按钮放在右侧，把内容框扩展到搜索按钮左边框下方，以形成一体化边框。

## 4. 状态与数据流

搜索按钮属性流：

```text
SearchEdit.SearchButtonStyle / SearchButtonText / IsOperating
      ↓ TemplateBinding
SearchEditDecoratedBox.SearchButtonStyle / SearchButtonText / IsSearchButtonLoading
      ↓ TemplateBinding
SearchButton.ButtonType / Content / IsLoading
```

输入状态流：

```text
SearchEdit.SizeType / StyleVariant / Status / IsEnabled
      ↓ TemplateBinding
SearchEditDecoratedBox
      ↓ TemplateBinding / selector
SearchButton + PART_ContentFrame
```

搜索事件流：

```text
SearchButton.Click
      ↓
SearchEditDecoratedBox.HandleSearchButtonClick
      ↓
OwningSearchEdit.NotifySearchButtonClicked()
      ↓
if !IsOperating raise SearchButtonClick
```

`IsOperating` 同时进入 `SearchButton.IsLoading` 和 `NotifySearchButtonClicked()` 的重复点击保护。该状态不参与文本值同步，也不改变 Form value。

## 5. 生命周期与模板接入

`SearchEdit.OnInitialized()` 在未设置 `ClearIcon` 时写入默认 `CloseCircleFilled`，与搜索输入常见清除语义保持一致。

`SearchEdit.OnApplyTemplate()` 调用基类 LineEdit 模板接入后，查找 `PART_AddOnDecoratedBox` 并设置 `SearchEditDecoratedBox.OwningSearchEdit`。该 owner 引用只用于把搜索按钮点击回调到 SearchEdit。

`SearchEditDecoratedBox.OnApplyTemplate()` 必须先解除旧 `_searchButton.Click` 订阅，再查找新的 `PART_RightAddOn` 并订阅 click。模板重新应用时不能保留旧按钮事件订阅。

`SearchEditDecoratedBox.NotifyAddOnBorderInfoCalculated()` 把 `RightAddOnBorderThickness` 设置为当前 `BorderThickness`。搜索按钮右侧边框由 decorated box 统一计算，保证搜索按钮与输入框圆角、边框折叠和 CompactSpace 状态一致。

## 6. 交互与事件处理

搜索按钮点击是 SearchEdit 唯一新增交互。它不会自动提交 Form，也不会在 `TextChanged` 时触发搜索。用户按回车触发搜索的行为如果需要支持，应在业务层或更高层输入处理里显式连接到同一个搜索命令。

清除按钮、密码 reveal、文本选择、光标移动、复制粘贴和滚动仍由 LineEdit / Avalonia TextBox 路径处理。SearchEdit 不应在搜索按钮事件中直接修改这些状态。

Disabled 状态通过模板传递到 `SearchEditDecoratedBox` 和 `SearchButton`。搜索按钮不应在 disabled 时成为独立可点击入口。

## 7. 内部算法与关键流程

### 7.1 SearchEditPanel 布局

`SearchEditPanel.ArrangeOverride()` 使用最终尺寸安排三个区域：

1. 左侧 AddOn 使用自身 `DesiredSize.Width`，高度为 `finalSize.Height`。
2. 搜索按钮使用自身 `DesiredSize.Width`，放在右侧，安排高度为 `finalSize.Height`。
3. 内容框宽度为总宽度减去左右 AddOn 宽度，再加回搜索按钮左边框厚度，使输入框右边框与按钮左边框重叠成单线。

该布局要求搜索按钮的可视 frame 高度跟随实际安排高度。Custom 高度下，如果按钮模板仍按 Button 自身默认 `Height` 绘制 frame，会出现按钮边框和输入框边框不齐。

### 7.2 搜索按钮高度同步

`SearchEditDecoratedBoxTheme.axaml` 将内部 `SearchButton.Height` 绑定到 `SearchEditDecoratedBox.Bounds.Height`。Button 模板中的 `Frame` 使用 Button `Height` 绘制，因此该绑定使 Custom 高度和内置尺寸下搜索按钮 frame 与输入框 frame 保持同高。

该同步属于 SearchEdit 组合模板职责，不应通过 Gallery 示例、固定 magic height 或 SearchButton 全局主题覆盖来修补。

### 7.3 搜索按钮 style 映射

`SearchButtonStyle=Default` 时，搜索按钮使用 `ButtonType=Default`。Outlined 表面下，按钮 hover/pressed 会提升 z-index，避免按钮边框被内容框遮住。

`SearchButtonStyle=Primary` 时，搜索按钮使用 `ButtonType=Primary`。Outlined 表面下按钮保持激活 z-index。

非 Outlined 表面下，搜索按钮统一映射为 `ButtonType=Text`，让 Filled、Borderless 和 Underlined 输入表面保持轻量视觉。

### 7.4 SearchButton icon 状态

`SearchButton.OnPropertyChanged()` 在 `Icon` 变化时维护 `skip-status` class。搜索图标不直接接收输入 status 颜色，按钮主题通过按钮前景和状态 selector 控制整体视觉。

## 8. 资源、性能与 AOT 边界

SearchEdit 不依赖运行时反射发现模板结构。跨模板协作使用固定 template part、`TemplateBinding`、selector 和 owner 引用完成。

资源和生命周期边界：

- 搜索按钮 click 订阅必须在重新套用模板时解绑旧实例。
- `OwningSearchEdit` 只保存当前模板 owner，不创建全局订阅。
- 搜索按钮高度同步使用 XAML binding，不在布局过程中写本地 `Height` 值。
- 搜索按钮状态不创建异步任务；业务异步状态由外部设置 `IsOperating`。
- SearchEdit 没有独立 Token scope，不应为运行时状态新增 Token。

AOT 边界：

- SearchEdit 主题通过显式 ResourceInclude 注册。
- `AutoCompleteSearchEditBox` 使用显式 `StyleKeyOverride` 复用 SearchEdit 主题。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护，不依赖运行时反射扫描。

## 9. 维护不变量

内部重构必须保持以下不变量：

- 搜索按钮 click 只能通过 `SearchEdit.NotifySearchButtonClicked()` 抛出 `SearchButtonClick`。
- `IsOperating=true` 必须阻止重复搜索事件，并继续驱动按钮 loading。
- 搜索按钮和内容框的边框必须在同一布局高度下绘制。
- `SearchEditPanel` 的按钮左边框重叠算法不能破坏单线边框视觉。
- 搜索按钮必须继承 SearchEdit 的 `SizeType`、`StyleVariant`、`Status` 和 `IsEnabled`。
- 搜索按钮右侧外部 AddOn 位置不可被用户内容替代。
- `InnerRightContent`、clear、reveal 和文本 presenter 的绑定仍由 LineEdit 模板路径维护。
- AutoCompleteSearchEdit 复用 SearchEdit 视觉时不能绕过 SearchEdit 搜索按钮契约。

## 10. 测试与验证

验证范围：

- `SearchEditLayoutTests` 覆盖 Custom 和内置 SizeType 下搜索按钮 frame 与输入框 frame 高度一致。
- `CustomizableSizeTypeContractTests` 覆盖 SearchEdit 和 AutoCompleteSearchEdit 的 customizable size contract。
- `LineEditShowCasePageTests` 覆盖 LineEdit / SearchEdit Gallery 示例结构、SearchEdit SizeType 示例和 snapshot。
- Gallery 走查 SearchEdit 基础、状态、尺寸、Custom、loading、disabled、内部右侧内容和 AutoComplete 搜索示例。
- 文档改动运行 `git diff --check`，并检查相对链接存在。
