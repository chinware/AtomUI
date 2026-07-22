# Segmented

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Segmented 是桌面端数据展示类的紧凑单选控件，用于在少量互斥选项之间切换当前视图、展示模式、时间粒度或状态筛选。它通过分段轨道、选项 item、选中滑块、图标和文本表达“当前只有一个有效选择”的语义。

Segmented 的职责是承载一组有限选项并维护单选状态。它不是完整导航系统、Tab 容器、列表选择器、弹层选择器、表单编辑器或大数据集合控件；复杂页面导航、动态搜索、虚拟化、多选和异步加载应由专用控件或业务层承担。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented` |
| 状态 | Stable |

## 何时使用

Segmented 的设计语言来自 参考设计体系的分段控制器：浅色轨道承载一组互斥选项，选中滑块跟随当前项移动，选项文本和图标在 hover、pressed、selected、disabled 状态下提供即时反馈。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 单选语义 | 同一组中只有一个选项处于选中态。 | `SelectedIndex`、`SelectedItem`、`:selected`。 |
| 视觉反馈 | 用滑块和 item 状态区分 hover、pressed、selected。 | `SelectedThumbBg`、item 背景和前景色。 |
| 信息密度 | 通过尺寸类型控制高度、字体、图标和内边距。 | `SizeType=Large/Middle/Small/Custom`。 |
| 铺满能力 | 在块级场景中让可见 item 等分父容器宽度。 | `IsExpanding`。 |
| 图文组合 | 允许纯文本、纯图标或图标加文本。 | `SegmentedItem.Icon`、`Content`、`:has-icon`。 |
| 动效一致性 | 选中滑块移动和 item 背景变化受全局 motion 控制。 | `IsMotionEnabled`。 |

## 公共 API

Segmented 的公共契约由根控件、item 容器、继承的选择 API 和主题契约组成。

根控件 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `SizeType` | `CustomizableSizeType` | 控制轨道圆角、item 高度、字体、padding 和图标尺寸，支持 `Large`、`Middle`、`Small`、`Custom`。 |
| `IsExpanding` | `bool` | 是否让可见 item 等分可用宽度；为 `true` 时根控件水平对齐为 stretch。 |
| `IsMotionEnabled` | `bool` | 是否启用选中滑块位置/尺寸动画和 item 背景动画，默认来自 `SharedToken.EnableMotion`。 |
| `SelectedIndex` / `SelectedItem` | inherited | 当前选择，来自 `SelectingItemsControl`。 |
| `SelectionChanged` | inherited event | 选择变化事件，来自 `SelectingItemsControl`。 |
| `Items` / `ItemsSource` / `ItemTemplate` | inherited | 选项集合和内容模板入口。 |

item API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `SegmentedItem.Icon` | `PathIcon?` | 选项左侧图标；非空时启用 `:has-icon`。 |
| `SegmentedItem.IsSelected` | `bool` | item 选中状态，接入 Avalonia `ISelectable` 选择模型。 |
| `Content` / `ContentTemplate` | inherited | item 内容和模板。 |

稳定 template part 和主题节点：

| 模板 | 节点 | 职责 |
| --- | --- | --- |
| `SegmentedTheme.axaml` | `Frame` | 根轨道裁剪、圆角和 padding 承载。 |
| `SegmentedTheme.axaml` | `PART_ItemsPresenter` | item presenter 和 `SegmentedStackPanel` 承载。 |
| `SegmentedItemTheme.axaml` | `Frame` | item 背景、圆角和 padding 承载。 |
| `SegmentedItemTheme.axaml` | `IconPresenter` | `SegmentedItem.Icon` 的视觉承载。 |
| `SegmentedItemTheme.axaml` | `Content` | item 内容承载。 |

稳定伪类：

- `:selected`：item 被选中。
- `:pressed`：item 按下态。
- `:has-icon`：item 设置了 `Icon`。
- `:pointerover` 和 `:disabled`：由 Avalonia 标准状态驱动，主题使用这些状态表达 hover 和 disabled 视觉。

`Segmented` 桌面层默认为非 visual 数据项创建 `SegmentedItem` 容器，并把 `SizeType`、`IsMotionEnabled` 传递给容器。`PrepareSegmentedItem(SegmentedItem, object?, int)` 是受保护扩展点，用于派生控件补充容器准备逻辑。

## 事件与命令

| `SelectionChanged` | inherited event | 选择变化事件，来自 `SelectingItemsControl`。 |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented/Views/SegmentedShowCase.axaml:36`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel HorizontalAlignment="Left" Orientation="Vertical" Spacing="10">
    <atom:Segmented Margin="20">
        <atom:SegmentedItem Content="每日" />
        <atom:SegmentedItem Content="每周" />
        <atom:SegmentedItem Content="每月" />
        <atom:SegmentedItem Content="每季度" />
        <atom:SegmentedItem Content="每年" />
    </atom:Segmented>
</StackPanel>
```

### 块级分段控制器

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented/Views/SegmentedShowCase.axaml:55`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel HorizontalAlignment="Stretch" Orientation="Vertical">
    <atom:Segmented IsExpanding="True" Margin="20">
        <atom:SegmentedItem>123</atom:SegmentedItem>
        <atom:SegmentedItem>456</atom:SegmentedItem>
        <atom:SegmentedItem Content="长文本-长文本-长文本-长文本" />
    </atom:Segmented>
</StackPanel>
```

### 禁用

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented/Views/SegmentedShowCase.axaml:72`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel HorizontalAlignment="Stretch" Orientation="Vertical" Spacing="10">
    <atom:Segmented Margin="20">
        <atom:SegmentedItem IsEnabled="False" Content="地图" />
        <atom:SegmentedItem IsEnabled="False" Content="公交" />
        <atom:SegmentedItem IsEnabled="False" Content="卫星" />
    </atom:Segmented>
    <atom:Segmented>
        <atom:SegmentedItem Content="每日" />
        <atom:SegmentedItem IsEnabled="False" Content="每周" />
        <atom:SegmentedItem Content="每月" />
        <atom:SegmentedItem IsEnabled="False" Content="每季度" />
        <atom:SegmentedItem Content="每年" />
    </atom:Segmented>
</StackPanel>
```

### Segmented 尺寸

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented/Views/SegmentedShowCase.axaml:96`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel HorizontalAlignment="Left" Orientation="Vertical" Spacing="10">
    <atom:Segmented SizeType="Large" Margin="20">
        <atom:SegmentedItem Content="每日" />
        <atom:SegmentedItem Content="每周" />
        <atom:SegmentedItem Content="每月" />
        <atom:SegmentedItem Content="每季度" />
        <atom:SegmentedItem Content="每年" />
    </atom:Segmented>

    <atom:Segmented Margin="20">
        <atom:SegmentedItem Content="每日" />
        <atom:SegmentedItem Content="每周" />
        <atom:SegmentedItem Content="每月" />
        <atom:SegmentedItem Content="每季度" />
        <atom:SegmentedItem Content="每年" />
    </atom:Segmented>

    <atom:Segmented SizeType="Small" Margin="20">
        <atom:SegmentedItem Content="每日" />
        <atom:SegmentedItem Content="每周" />
        <atom:SegmentedItem Content="每月" />
        <atom:SegmentedItem Content="每季度" />
        <atom:SegmentedItem Content="每年" />
    </atom:Segmented>

    <atom:Segmented SizeType="Custom"
                    Margin="20"
                    Padding="3"
                    CornerRadius="8">
        <atom:SegmentedItem Content="每日"
                            MinHeight="36"
                            Padding="16,0"
                            FontSize="15"
                            CornerRadius="6" />
        <atom:SegmentedItem Content="每周"
                            IsSelected="True"
                            MinHeight="36"
                            Padding="16,0"
                            FontSize="15"
                            CornerRadius="6" />
        <atom:SegmentedItem Content="每月"
                            MinHeight="36"
                            Padding="16,0"
                            FontSize="15"
                            CornerRadius="6" />
        <atom:SegmentedItem Content="每季度"
                            MinHeight="36"
                            Padding="16,0"
                            FontSize="15"
                            CornerRadius="6" />
    </atom:Segmented>
</StackPanel>
```

## 状态模型

Segmented 的核心状态流：

```text
Items / ItemsSource
      ↓
SegmentedItem containers
      ↓
ISelectable selected state
      ↓
SelectedItem / SelectedIndex
      ↓
selected thumb position + size
```

选择语义：

- 构造阶段固定 `SelectionMode=Single`。
- 模板应用时，如果没有显式 `SelectedIndex`、`SelectedItem` 或已选中容器，并且 `Items.Count > 0`，控件选择第一个 item。
- 已绑定或已显式设置的选择必须在模板应用后保留。
- 鼠标左键释放在 item 上触发选择，避免按下时立即改变选择造成的交互跳变。
- `SelectionChanged` 后如果控件仍附加在视觉树，会重新计算选中滑块的位置和尺寸。

尺寸语义：

- `Large`、`Middle`、`Small` 进入对应主题分支。
- `Custom` 复用 Middle 作为默认视觉基线；用户可在实例或 item 上显式设置 `Padding`、`MinHeight`、`FontSize`、`CornerRadius` 等 Avalonia 属性形成自定义尺寸。
- 根 `SizeType` 会同步给生成的 `SegmentedItem`，保证 item 的主题分支和根控件一致。

展开布局语义：

- `IsExpanding=false` 时，item 按自身期望宽度顺序排列，根控件默认左对齐。
- `IsExpanding=true` 时，可见 item 等分可用宽度，隐藏 item 不参与等分计数。

Form 语义：

- Segmented 实现 `IFormItemAware`。
- `SetFormValue(value)` 设置 `SelectedItem`。
- `GetFormValue()` 返回 `SelectedItem`。
- `ClearFormValue()` 清空 `SelectedItem`。
- `SelectedItem` 变化会通知 Form value changed。

## 主题与 Design Token

Segmented 的视觉由根主题、item 主题、专属 Token 和 SharedToken 共同决定。

| 主题文件 | 职责 |
| --- | --- |
| `SegmentedTheme.axaml` | 根模板、轨道 padding/background、选中滑块资源、SizeType 圆角分支、展开对齐和滑块动画。 |
| `SegmentedItemTheme.axaml` | item 模板、图标/内容布局、hover/pressed/selected/disabled 状态、SizeType 尺寸分支和图标尺寸。 |
| `SegmentedThemes.axaml` | 汇总 Segmented 相关主题资源。 |

视觉关系：

```text
SharedToken
   ↓
SegmentedToken
   ↓
SegmentedTheme / SegmentedItemTheme
   ↓
track + selected thumb + item states
```

根控件在 `Render()` 中绘制轨道背景和选中滑块。item 模板绘制每个选项自身的背景、图标、内容和状态颜色。选中滑块位置来自当前选中容器相对根控件的坐标，尺寸来自当前选中容器的 `DesiredSize`。

Token 来源：

SegmentedToken 是 Segmented 的组件级 Token scope，描述分段轨道、选项文本状态、选项背景状态、选中滑块背景和 item 尺寸的主题语义。

SegmentedToken 不承载以下状态：

- `Items`、`ItemsSource`、`ItemTemplate`、`Content` 等数据状态。
- `SelectedIndex`、`SelectedItem`、`:selected`、`:pressed`、`:has-icon` 等实例或伪类状态本身。
- `SelectedThumbPos`、`SelectedThumbSize` 等运行时布局派生状态。
- `IsExpanding`、可见 item 数量、等分宽度等布局状态。
- `IsMotionEnabled` 或 transition 时长开关；motion 时长来自 SharedToken。

## AOT 与裁剪注意事项

Segmented 不依赖运行时反射或动态成员访问。主题协作通过固定模板节点、显式控件类型、Token resource 和 Avalonia property binding 完成。

资源与生命周期边界：

- `SegmentedToken` 通过 token generator 注册，Theme 通过 `SegmentedTokenResource` 使用。
- `SegmentedStackPanel.IsExpanding` 在 AXAML 中绑定到最近的 `Segmented` ancestor。
- 容器的 `SizeType` 和 `IsMotionEnabled` 是生成容器与 owner 的固定关系，生命周期由容器准备和 Avalonia 绑定系统管理。
- `SelectionChanged` 订阅必须在 attach/detach 中成对管理。
- 选中滑块动画只在 `IsMotionEnabled=true` 时通过 transitions 启用。

AOT 边界：

- 不新增字符串反射、动态类型扫描、`Activator.CreateInstance(Type)` 或运行时属性名访问。
- Gallery API/Token 表使用显式 ViewModel 数据。
- 主题内数据模板使用显式 `x:DataType`。

## 源码索引

共享源码：

- `src/AtomUI.Controls/Segmented/AbstractSegmented.cs`：共享根控件，定义公共属性、内部滑块属性、选择生命周期、容器准备、Form 接口和 render 绘制。
- `src/AtomUI.Controls/Segmented/AbstractSegmentedItem.cs`：共享 item，定义 `IsSelected`、`Icon`、内部 `SizeType` / `IsMotionEnabled`、图标伪类和 pointer release 选择。
- `src/AtomUI.Controls/Segmented/SegmentedStackPanel.cs`：内部 items panel，执行普通排列和 expanding 等分排列。
- `src/AtomUI.Controls/Segmented/SegmentedPseudoClass.cs`：Segmented 专用伪类常量。

桌面源码：

- `src/AtomUI.Desktop.Controls/Segmented/Segmented.cs`：桌面公开根控件，注册 Token scope，创建 `SegmentedItem` 容器。
- `src/AtomUI.Desktop.Controls/Segmented/SegmentedItem.cs`：桌面公开 item，注册 Token scope。
- `src/AtomUI.Desktop.Controls/Segmented/SegmentedToken.cs`：Segmented 组件 Token。
- `src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedTheme.axaml`：根模板、轨道、滑块、SizeType 和 motion 样式。
- `src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedItemTheme.axaml`：item 模板、状态样式、SizeType 和图标样式。
- `src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedThemes.axaml`：主题资源聚合入口。

Gallery 和测试：

- `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented/`：Segmented 示例、API 表、Token 表和本地化文案。
- `tests/AtomUI.Desktop.Controls.Tests/Segmented/SegmentedSelectionInitializationTests.cs`：选择初始化、Form value 和 expanding 布局回归测试。
- `tests/AtomUI.Desktop.Controls.Tests/SizeType/CustomizableSizeTypeContractTests.cs`：`ICustomizableSizeTypeAware` 契约测试。
- `tests/AtomUIGallery.Tests/ShowCases/SegmentedShowCasePageTests.cs`：Gallery 页面结构和示例快照测试。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/segmented/overview.md`
- 实现文档：`docs/controls/desktop/data-display/segmented/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/segmented/token.md`
- 变更记录：`docs/controls/desktop/data-display/segmented/changelog.md`
- 语义结构：`./semantic-cn.md`
