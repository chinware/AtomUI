# GroupBox

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

GroupBox 是桌面端数据展示类分组容器，用于在一块有边框的区域中展示一组相关内容，并通过 Header 标题、图标和标题位置表达分组语义。

GroupBox 的职责是组织和标注内容区域，不负责内容项布局、折叠展开、表单校验、数据绑定集合管理或异步加载。需要折叠行为时应使用 Collapse/Expander 类控件；需要卡片信息组织时应使用 Card。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/GroupBox` |
| 状态 | Stable |

## 何时使用

GroupBox 的视觉语义接近 fieldset：边框提供分组边界，Header 嵌入在边框上方的视觉通道中，内容区域在边框内部保持稳定留白。

Header 可以位于左侧、居中或右侧，但标题本身始终是分组名称，不应承担操作入口或复杂工具栏职责。Header 图标用于强化分组主题，不改变 GroupBox 的交互模型。

## 公共 API

GroupBox 继承 `ContentControl`，公共 API 分为 Header API 和继承的容器 API。

Header API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `HeaderTitle` | `string?` | Header 显示的分组标题。 |
| `HeaderTitleColor` | `IBrush?` | Header 标题颜色，默认来自 `ColorText`。 |
| `HeaderIcon` | `PathIcon?` | Header 标题前的图标；为 `null` 时图标节点不可见。 |
| `HeaderTitlePosition` | `GroupBoxTitlePosition` | Header 内容水平位置，支持 `Left`、`Right`、`Center`。 |
| `HeaderFontSize` | `double` | Header 标题字号。 |
| `HeaderFontStyle` | `FontStyle` | Header 标题字体样式。 |
| `HeaderFontWeight` | `FontWeight` | Header 标题字重。 |

继承 API：

- `Content` / `ContentTemplate` 定义分组内容。
- `Background` 定义内容区域背景，允许透明或半透明。
- `BorderBrush` / `BorderThickness` 定义分组边框。
- `CornerRadius` 定义边框圆角。
- `Padding` 定义内容区域内边距，默认由 GroupBox Token 提供。

GroupBox 在未显式设置 `Height` / `MaxHeight` 等外部约束时，会根据模板根节点的测量结果自动确定高度。该高度包含 Header 通道、内容区域 `Padding` 和内容自身 `DesiredSize`。内容容器仍需遵循 Avalonia 布局语义主动汇报期望尺寸，例如使用 `StackPanel`、`Grid` 或显式尺寸；裸 `Panel` / `Canvas` 等不会自然按子元素累加高度的容器不会被 GroupBox 特殊改写。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Frame` | `Border` | 承载整体布局根节点，并提供边框 bounds 参考。 |
| `PART_HeaderContainer` | `Panel` | Header 行容器。 |
| `PART_HeaderContent` | `Decorator` | Header 内容实际 bounds，用于标题缺口和对齐计算。 |
| `PART_HeaderIconPresenter` | `IconPresenter` | Header 图标展示。 |
| `PART_HeaderPresenter` | `TextBlock` | Header 标题展示。 |
| `PART_ContentPresenter` | `ContentPresenter` | 内容承载。 |

## 事件与命令

GroupBox 的事件与命令以控件文档、源码 public surface 和 Avalonia 基类契约为准；生成器不从源码发明额外事件。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/GroupBox/Views/GroupBoxShowCase.axaml:36`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:GroupBox HeaderTitle="标题信息">
    <Panel Height="100">
        <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="分组框内容" />
    </Panel>
</atom:GroupBox>
```

### 自动高度

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/GroupBox/Views/GroupBoxShowCase.axaml:52`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:GroupBox HeaderTitle="自动高度">
    <StackPanel Spacing="8">
        <atom:TextBlock Text="下面的 GroupBox 没有设置 Height，内容区域会随着文本行数自动增长。" TextWrapping="Wrap" />
        <atom:TextBlock Text="当内容来自 StackPanel、Grid 或显式尺寸控件时，GroupBox 会使用内容的 DesiredSize 计算整体高度。" TextWrapping="Wrap" />
        <atom:TextBlock Text="如果父容器设置了固定高度或 MaxHeight，则仍然会按 Avalonia 布局约束进行裁剪或滚动。" TextWrapping="Wrap" />
    </StackPanel>
</atom:GroupBox>
```

### 标题位置

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/GroupBox/Views/GroupBoxShowCase.axaml:69`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:GroupBox HeaderTitle="标题信息">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="分组框内容" />
        </Panel>
    </atom:GroupBox>
    <atom:GroupBox HeaderTitle="标题信息" HeaderTitlePosition="Center">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="分组框内容" />
        </Panel>
    </atom:GroupBox>
    <atom:GroupBox HeaderTitle="标题信息" HeaderTitlePosition="Right">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="分组框内容" />
        </Panel>
    </atom:GroupBox>
</StackPanel>
```

### 标题样式

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/GroupBox/Views/GroupBoxShowCase.axaml:96`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:GroupBox HeaderTitle="标题信息" HeaderFontStyle="Italic">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="分组框内容" />
        </Panel>
    </atom:GroupBox>
    <atom:GroupBox HeaderTitle="标题信息" HeaderTitlePosition="Center" HeaderFontWeight="Bold">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="分组框内容" />
        </Panel>
    </atom:GroupBox>
    <atom:GroupBox HeaderTitle="标题信息" HeaderTitlePosition="Right" HeaderFontStyle="Oblique">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="分组框内容" />
        </Panel>
    </atom:GroupBox>
    <atom:GroupBox HeaderTitle="标题信息" HeaderTitlePosition="Center" HeaderFontStyle="Oblique"
                   HeaderTitleColor="Coral" HeaderFontWeight="Medium">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="分组框内容" />
        </Panel>
    </atom:GroupBox>
</StackPanel>
```

## 状态模型

GroupBox 自身没有 hover、pressed、loading、selected、expanded 或 checked 状态。它不拦截输入事件，也不为 Header 提供默认点击行为。

GroupBox 的有效状态来自公共属性、模板测量结果和主题资源：

```text
HeaderTitle / HeaderIcon / HeaderTitlePosition
      + Header font properties
      + template measured bounds
        ↓
Header content bounds
        ↓
Frame border bounds + Header gap bounds
        ↓
Background / BorderBrush / BorderThickness / CornerRadius render state
```

`HeaderIcon`、Header 字体、标题位置和 Header 内容变化会影响缺口尺寸或位置。内容尺寸变化会通过模板根 `PART_Frame` 参与 GroupBox 的 measure pass，使自动高度随内容 `DesiredSize` 增长，同时仍尊重父容器可用空间和显式高度约束。`Background`、`BorderBrush`、`BorderThickness`、`CornerRadius` 改变会影响自绘边框和背景。

## 主题与 Design Token

GroupBox 的默认 Theme 位于 `src/AtomUI.Desktop.Controls/GroupBox/Themes/GroupBoxTheme.axaml`。Theme 提供 Header、Frame 和 Content 的可测量结构，并设置默认背景、边框、圆角、标题颜色、标题字号和间距。

标题缺口是 GroupBox 的视觉契约：Header 内容区域必须从上边框中形成缺口。缺口不能依赖用 `Background` 覆盖边框线，因为 `Background` 允许为透明或半透明。

Theme 职责：

- 设置 `Background`、`BorderBrush`、`BorderThickness`、`CornerRadius` 默认值。
- 设置 `HeaderTitleColor`、`HeaderFontSize`。
- 通过 GroupBox Token 设置 Header 容器外边距、Header 内容内边距、Header 图标间距和内容内边距。
- 根据 `HeaderTitlePosition` 设置 `PART_HeaderContent` 的水平对齐。

Theme 不负责动态构建 Header 或绘制边框。Header 缺口属于控件渲染模型。

Token 来源：

GroupBox Token 将全局 SharedToken 转换为 GroupBox 可消费的组件级结构值，主要覆盖 Header 和内容区域的间距。颜色、边框厚度、圆角和字体基础值直接来自 SharedToken，不在 GroupBox Token 中重复定义。

GroupBox Token 不表达实例状态，也不负责 Header 缺口的运行时 bounds。Header 缺口由模板测量结果和控件渲染模型共同决定。

## AOT 与裁剪注意事项

GroupBox 渲染应使用 Avalonia 绘制 API 和稳定 template part，不使用反射读取模板内部状态。

Header 缺口通过几何排除实现，不增加遮挡用背景层。该方式保持 VisualTree 简洁，也避免透明背景下依赖父背景颜色。

Token 通过动态资源进入 Theme，不应在 `Render` 中主动查找全局资源。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/GroupBox/GroupBox.cs`：公共 API、template part 获取、测量 bounds、渲染和失效逻辑。
- `src/AtomUI.Desktop.Controls/GroupBox/GroupBoxToken.cs`：GroupBox 控件 Token。
- `src/AtomUI.Desktop.Controls/GroupBox/Themes/GroupBoxTheme.axaml`：模板结构、Header 对齐、TokenResource 引用和默认视觉属性。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/group-box/overview.md`
- 实现文档：`docs/controls/desktop/data-display/group-box/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/group-box/token.md`
- 变更记录：`docs/controls/desktop/data-display/group-box/changelog.md`
- 语义结构：`./semantic-cn.md`
