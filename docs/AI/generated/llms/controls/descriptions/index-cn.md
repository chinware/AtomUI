# Descriptions

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Descriptions 是桌面端数据展示类控件，用于按标签和值展示对象属性、订单信息、配置详情、用户资料等结构化只读信息。它通过 `DescriptionItem` 集合、响应式列数、水平/纵向布局、边框模式和 Header/Extra 区域表达“属性说明列表”的展示语义。

Descriptions 的职责是把描述项数据排版为稳定的网格视觉。它不负责编辑、选择、排序、过滤、远程数据加载、表单提交、虚拟化列表或复杂主从详情关系；这些能力应由业务层、DataGrid、ListView、Form 或专用编辑控件承担。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Descriptions` |
| 状态 | Stable |

## 何时使用

Descriptions 的设计语言来自 参考设计体系的描述列表：标签提供字段名，内容提供字段值，列数和跨度提供信息密度，边框模式提供表格化阅读边界。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 展示密度 | 用尺寸和列数控制同屏信息量。 | `SizeType`、`ColumnInfo`。 |
| 结构层级 | 用 Header 和 Extra 表达当前信息组名称和辅助操作入口。 | `Header`、`Extra`。 |
| 标签和值 | 每个 `DescriptionItem` 表达一对 label/content。 | `Label`、`Content`。 |
| 跨列能力 | 通过 `Span` 和 `IsFilled` 表达长内容或整行内容。 | `Span`、`IsFilled`。 |
| 视觉边界 | 普通模式轻量展示，边框模式强调表格结构。 | `IsBordered`。 |
| 方向模型 | 水平方向把 label 和 content 放在同一行，纵向方向上下排列。 | `Layout`。 |

Descriptions 是展示控件，不提供 hover、pressed、selected、checked、loading 或 popup 等交互状态。

## 公共 API

Descriptions 的公共 API 分为控件级属性、内容集合入口和描述项模型。

控件级属性：

| API | 类型 | 语义 |
| --- | --- | --- |
| `IsBordered` | `bool` | 是否使用边框模式。水平边框模式会为每个 item 生成独立 label/content cell。 |
| `IsShowColon` | `bool` | 普通 horizontal/vertical 非边框模式下是否显示冒号，默认 `true`。 |
| `ColumnInfo` | `ResponsiveInt?` | 响应式列数配置。为空时使用控件内置断点默认列数。 |
| `Header` / `HeaderTemplate` | `object?` / `IDataTemplate?` | 描述列表标题区域内容及模板。 |
| `Extra` / `ExtraTemplate` | `object?` / `IDataTemplate?` | 标题行右侧辅助内容及模板。 |
| `Layout` | `Orientation` | 描述项 label/content 排列方向，默认 `Horizontal`。 |
| `SizeType` | `SizeType` | 展示尺寸，默认 `Large`，支持 `Large/Middle/Small` 三种主题分支。 |
| `ItemsSource` | `IEnumerable?` | 外部描述项数据源。当前实现只物化其中的 `DescriptionItem`。 |
| `Items` | `DescriptionItems` | XAML 内容集合入口，也是布局算法的主数据源。 |

描述项模型：

| API | 类型 | 语义 |
| --- | --- | --- |
| `DescriptionItem.Label` | `string` | 字段标签文本，作为 Avalonia 属性支持 XAML 绑定。 |
| `DescriptionItem.Content` | `object?` | 字段内容，作为 Avalonia 属性支持 XAML 绑定，允许普通文本或复杂 Avalonia 内容。 |
| `DescriptionItem.IsFilled` | `bool` | 当前项是否填满本行剩余列，作为 Avalonia 属性参与布局刷新。 |
| `DescriptionItem.Span` | `ResponsiveInt` | 当前项跨列数，默认 `1`，作为 Avalonia 属性支持响应式配置和布局刷新。 |

`DescriptionItem` 是非视觉 `AvaloniaObject` 描述对象，不是 `Control`、`StyledElement` 或 template part。它的职责是承载 item 级 Avalonia 属性，让下面这种 XAML 绑定成为稳定契约：

```xml
<atom:DescriptionItem Label="Name" Content="{Binding Name}" />
```

Descriptions 仍然负责把 `DescriptionItem` 转换成内部视觉控件。`DescriptionItem` 自身不能直接进入视觉树，也不能承载需要视觉树生命周期才能安全释放的主题或资源状态。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `HeaderLayout` | `DockPanel` | Header/Extra 行容器，固定存在，通过 `IsHeaderLayoutVisible` 控制显示。 |
| `ExtraPresenter` | `ContentPresenter` | Extra 内容和模板承载。 |
| `HeaderPresenter` | `ContentPresenter` | Header 内容和模板承载。 |
| `ContentFrame` | `Border` | 内容区域边框、圆角和裁剪承载。 |
| `PART_GridLayout` | `Grid` | 生成的描述项视觉子控件布局容器。 |

Descriptions 没有 public routed event、命令或专用伪类。内部生成的 `DescriptionDefaultItem`、`DescriptionBorderedItemLabel` 和 `DescriptionBorderedItemContent` 是主题承载类型，不是公开用户 API。

## 事件与命令

Descriptions 没有 public routed event、命令或专用伪类。内部生成的 `DescriptionDefaultItem`、`DescriptionBorderedItemLabel` 和 `DescriptionBorderedItemContent` 是主题承载类型，不是公开用户 API。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Descriptions/Views/DescriptionsShowCase.axaml:33`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Descriptions Header="用户信息">
    <atom:DescriptionItem Label="用户名" Content="周毛毛" />
    <atom:DescriptionItem Label="电话" Content="1810000000" />
    <atom:DescriptionItem Label="居住地" Content="浙江杭州" />
    <atom:DescriptionItem Label="备注" Content="暂无" />
    <atom:DescriptionItem Label="地址"
                          Content="中国浙江省杭州市西湖区万塘路 18 号" />
</atom:Descriptions>
```

### 边框

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Descriptions/Views/DescriptionsShowCase.axaml:50`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:Descriptions IsBordered="True">
    <atom:DescriptionItem Label="产品" Content="云数据库" />
    <atom:DescriptionItem Label="计费方式" Content="预付费" />
    <atom:DescriptionItem Label="自动续费" Content="是" />
    <atom:DescriptionItem Label="订购时间" Content="2018-04-24 18:00:00" />
    <atom:DescriptionItem Label="使用时间" Content="2019-04-24 18:00:00" Span="2" />
    <atom:DescriptionItem Label="状态" Content="运行中" Span="3" />
    <atom:DescriptionItem Label="协商金额" Content="$80.00" />
    <atom:DescriptionItem Label="折扣" Content="$20.00" />
    <atom:DescriptionItem Label="官方收据" Content="$60.00" />
    <atom:DescriptionItem Label="配置信息">
        <atom:DescriptionItem.Content>
            <StackPanel Orientation="Vertical" Spacing="5">
                <TextBlock Text="数据盘类型：MongoDB" />
                <TextBlock Text="数据库版本：3.4" />
                <TextBlock Text="套餐：dds.mongo.mid" />
                <TextBlock Text="存储空间：10 GB" />
                <TextBlock Text="副本因子：3" />
                <TextBlock Text="地域：华东 1" />
            </StackPanel>
        </atom:DescriptionItem.Content>
    </atom:DescriptionItem>
</atom:Descriptions>
```

### 自定义尺寸

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Descriptions/Views/DescriptionsShowCase.axaml:82`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Vertical" Spacing="30">
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:RadioButton Name="DefaultSizeRadioButton"
                          IsChecked="True"
                          Tag="{x:Static atom:SizeType.Large}"
                          IsCheckedChanged="SizeTypeCheckChanged"
                          Content="大号" />
        <atom:RadioButton Name="MiddleSizeRadioButton"
                          Tag="{x:Static atom:SizeType.Middle}"
                          IsCheckedChanged="SizeTypeCheckChanged"
                          Content="中号" />
        <atom:RadioButton Name="SmallSizeRadioButton"
                          Tag="{x:Static atom:SizeType.Small}"
                          IsCheckedChanged="SizeTypeCheckChanged"
                          Content="小号" />
    </StackPanel>
    <atom:Descriptions IsBordered="True"
                       SizeType="{Binding DescriptionsSizeType}"
                       Header="自定义尺寸">
        <atom:Descriptions.Extra>
            <atom:Button ButtonType="Primary" Content="编辑" />
        </atom:Descriptions.Extra>
        <atom:DescriptionItem Label="产品" Content="云数据库" />
        <atom:DescriptionItem Label="计费方式" Content="预付费" />
        <atom:DescriptionItem Label="自动续费" Content="是" />
        <atom:DescriptionItem Label="订购时间" Content="2018-04-24 18:00:00" />
        <atom:DescriptionItem Label="使用时间" Content="2019-04-24 18:00:00" Span="2" />
        <atom:DescriptionItem Label="状态" Content="运行中" Span="3" />
        <atom:DescriptionItem Label="协商金额" Content="$80.00" />
        <atom:DescriptionItem Label="折扣" Content="$20.00" />
        <atom:DescriptionItem Label="官方收据" Content="$60.00" />
        <atom:DescriptionItem Label="配置信息">
            <atom:DescriptionItem.Content>
                <StackPanel Orientation="Vertical" Spacing="5">
                    <TextBlock Text="数据盘类型：MongoDB" />
                    <TextBlock Text="数据库版本：3.4" />
                    <TextBlock Text="套餐：dds.mongo.mid" />
                    <TextBlock Text="存储空间：10 GB" />
                    <TextBlock Text="副本因子：3" />
                    <TextBlock Text="地域：华东 1" />
                </StackPanel>
            </atom:DescriptionItem.Content>
        </atom:DescriptionItem>
    </atom:Descriptions>

    <atom:Descriptions Header="自定义尺寸"
                       SizeType="{Binding DescriptionsSizeType}">
        <atom:Descriptions.Extra>
            <atom:Button ButtonType="Primary" Content="编辑" />
        </atom:Descriptions.Extra>
        <atom:DescriptionItem Label="用户名" Content="周毛毛" />
        <atom:DescriptionItem Label="电话" Content="1810000000" />
        <atom:DescriptionItem Label="居住地" Content="浙江杭州" />
        <atom:DescriptionItem Label="备注" Content="暂无" />
        <atom:DescriptionItem Label="地址"
                              Content="中国浙江省杭州市西湖区万塘路 18 号" />
    </atom:Descriptions>
</StackPanel>
```

### 响应式

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Descriptions/Views/DescriptionsShowCase.axaml:150`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:Descriptions IsBordered="True"
                   SizeType="{Binding DescriptionsSizeType}"
                   Header="响应式描述列表"
                   ColumnInfo="xs: 1, sm: 2, md: 3, lg: 3, xl: 4, xxl: 4">
    <atom:DescriptionItem Label="产品" Content="云数据库" />
    <atom:DescriptionItem Label="计费" Content="预付费" />
    <atom:DescriptionItem Label="时间" Content="18:00:00" />
    <atom:DescriptionItem Label="金额" Content="$80.00" />
    <atom:DescriptionItem Label="折扣" Content="$20.00" Span="xl: 2, xxl: 2" />
    <atom:DescriptionItem Label="官方" Content="$60.00" Span="xl: 2, xxl: 2" />
    <atom:DescriptionItem Label="配置信息" Span="xs: 1, sm: 2, md: 3, lg: 3, xl: 2, xxl: 2">
        <atom:DescriptionItem.Content>
            <StackPanel Orientation="Vertical">
                <TextBlock Text="数据盘类型：MongoDB" />
                <TextBlock Text="数据库版本：3.4" />
                <TextBlock Text="套餐：dds.mongo.mid" />
            </StackPanel>
        </atom:DescriptionItem.Content>
    </atom:DescriptionItem>
    <atom:DescriptionItem Label="硬件信息" Span="xs: 1, sm: 2, md: 3, lg: 3, xl: 2, xxl: 2">
        <atom:DescriptionItem.Content>
            <StackPanel Orientation="Vertical">
                <TextBlock Text="CPU：6 核 3.5 GHz" />
                <TextBlock Text="副本因子：3" />
                <TextBlock Text="地域：华东 1" />
            </StackPanel>
        </atom:DescriptionItem.Content>
    </atom:DescriptionItem>
</atom:Descriptions>
```

## 状态模型

Descriptions 的核心状态流：

```text
ItemsSource / Items
      ↓
DescriptionItem collection
      ↓
generated item controls
      ↓
responsive column count + item span
      ↓
PART_GridLayout row/column/column-span
```

列数解析：

- `ColumnInfo` 为空时，断点默认列数为 `ExtraSmall=1`、`Small=2`、`ExtraExtraExtraLarge=4`，其他断点为 `3`。
- `ColumnInfo` 不为空时，按 `ResponsiveInt.Resolve()` 使用移动端优先级解析列数。
- 水平边框模式下内部有效列数为展示列数的两倍，因为 label 和 content 分别占 grid cell。
- 非水平边框模式下内部有效列数等于展示列数。

布局语义：

- 普通水平模式使用一个 `DescriptionDefaultItem` 展示 label、冒号和 content。
- 水平边框模式为每个 item 生成一个 label cell 和一个 content cell。
- 纵向模式使用一个 `DescriptionDefaultItem`，label 在上、content 在下；当 `IsBordered=true` 时通过模板显示内部边框和分隔线。
- `Span` 限制在当前行剩余列范围内，最小为 `1`。
- 最后一个 item 或 `IsFilled=true` 的 item 会填满当前行剩余列。

Header/Extra 状态：

```text
Header != null || Extra != null
      ↓
IsHeaderLayoutVisible
      ↓
HeaderLayout.IsVisible
```

## 主题与 Design Token

Descriptions 的默认视觉由根主题、默认项主题和边框 cell 主题组成。

| 主题文件 | 职责 |
| --- | --- |
| `DescriptionsTheme.axaml` | 根模板、Header/Extra、ContentFrame、`PART_GridLayout`、边框模式内容框和非边框 RowSpacing。 |
| `DescriptionDefaultItemTheme.axaml` | 普通项 horizontal/vertical/vertical bordered 三种模板和冒号、label、content 视觉。 |
| `DescriptionBorderedItemLabelTheme.axaml` | 水平边框模式 label cell 视觉。 |
| `DescriptionBorderedItemContentTheme.axaml` | 水平边框模式 content cell 视觉。 |

Token 关系：

```text
SharedToken
   ↓
DescriptionsToken
   ↓
DescriptionsTheme / DescriptionDefaultItemTheme / bordered cell themes
```

根 `ContentFrame` 的边框、圆角、裁剪来自 SharedToken。label 背景、label/content/title/extra 颜色、Header margin、item padding 和冒号 margin 来自 DescriptionsToken。

Token 来源：

DescriptionsToken 是 Descriptions 的控件级 Token scope，描述描述列表的 label 背景、文本颜色、标题颜色、Header 间距、item padding、冒号间距、内容颜色和 Extra 颜色。

DescriptionsToken 不承载以下状态：

- `Items`、`ItemsSource`、`DescriptionItem.Content` 等数据状态。
- `ColumnInfo`、当前断点、有效列数、row/column/column-span 等布局状态。
- `IsBordered`、`Layout`、`IsShowColon`、`IsFilled`、`Span` 等实例行为状态。
- `Header`、`Extra` 的实际内容或模板。
- `IsLastRow`、`IsLastColumn`、`EffectiveBorderThickness` 等生成视觉派生状态。

## AOT 与裁剪注意事项

Descriptions 不依赖运行时反射发现模板结构。模板协作通过固定 part 名称、显式内部类型和 Avalonia property binding 完成。

资源与生命周期边界：

- `DescriptionsToken` 通过 token generator 注册，Theme 通过 `DescriptionsTokenResource` 使用。
- 生成的内部控件是 Visual，Token resource 由主题和视觉树生命周期管理。
- `DescriptionItem` 是非视觉 `AvaloniaObject`，只能承载 item 级数据属性和普通 binding target 语义。
- `IsShowColon`、`IsBordered` 和 `SizeType` 的内部同步使用 Avalonia property binding，不使用 `BindUtils.RelayBind`。
- 媒体断点事件订阅由 `OnAttachedToVisualTree` / `OnDetachedFromVisualTree` 配对管理。
- 不在 `Render`、layout hot path 或数据物化路径中读取全局资源。

### 8.1 非视觉 DescriptionItem 的泄露边界

`DescriptionItem` 继承 `AvaloniaObject` 后，binding expression、property changed subscription 和资源查找都有了真实生命周期成本。实现必须把 acquire/release 写成对称路径，不能只依赖 GC 或视觉树清理。

需要防范的泄露链：

```text
DescriptionItem.PropertyChanged subscription
  → Descriptions / generated control
  → old ShowCase visual tree
```

```text
generated control / presenter binding
  → old DescriptionItem
  → old owner / content visual
```

```text
Application.ResourcesChanged
  → DynamicResourceExpression
  → DescriptionItem.ValueStore
  → owner / generated control / ShowCase
```

标准防范规则：

- item attach 时注册属性变化订阅，item detach 时解除同一个订阅。
- generated control 的 binding、事件和 disposable 归属 generated control 或 item attach 生命周期；视觉重建前必须统一释放。
- item 到 generated control 的映射只服务当前模板和布局周期；生成视觉清理路径必须同时清理映射和 item 订阅。
- 主题资源优先绑定在生成出来的 Visual 控件上，让视觉树生命周期管理 `DynamicResource`。
- `DescriptionItem` 上禁止直接放 `DynamicResource` 或 token-resource binding，除非它实现 scoped `IResourceHost` / `IThemeVariantHost`，并有 owner attach/detach 与 WeakReference 测试。

### 8.2 DynamicResource Scoped Host 规则

如果 `DescriptionItem` 必须承载动态资源，它必须作为 owning `Descriptions` 的 scoped resource host：

- `TryGetResource()` 先查询 owning `Descriptions`，再 fallback 到 `Application.Current`。
- owner 变化时先 unsubscribe 旧 owner 的 `ResourcesChanged` / `ActualThemeVariantChanged`，再保存并订阅新 owner。
- owner detach、Items remove、Items reset、template reapply 和控件 detach 都必须能释放 owner 订阅。
- owner 资源和主题变化需要通过 `DescriptionItem` 转发，确保动态资源仍能更新。
- 必须补 WeakReference 测试，模拟 XAML `DynamicResource` anchor，证明 item 离开集合后不会被 `Application.ResourcesChanged` root 住。

AOT 边界：

- 不新增字符串 path binding、反射扫描、`Activator.CreateInstance(Type)` 或动态成员访问。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护。
- `ItemsSource` 只接受已经构造好的 `DescriptionItem`，不做基于反射的对象属性展开。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Descriptions/Descriptions.cs`：公共 API、Items 集合、ItemsSource 物化、媒体断点订阅、生成视觉子项和布局算法。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionItem.cs`：非视觉 `AvaloniaObject` 描述项模型和 `DescriptionItems` 集合类型。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionDefaultItem.cs`：普通项和纵向边框项的内部承载控件。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionBorderedCell.cs`：水平边框模式 label/content cell 的共享内部基类。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionBorderedItemLabel.cs`：水平边框模式 label cell 类型。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionBorderedItemContent.cs`：水平边框模式 content cell 类型。
- `src/AtomUI.Desktop.Controls/Descriptions/DescriptionsToken.cs`：Descriptions 控件 Token。
- `src/AtomUI.Desktop.Controls/Descriptions/Themes/*.axaml`：根模板、普通项模板、边框 cell 模板和 token 样式。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/descriptions/overview.md`
- 实现文档：`docs/controls/desktop/data-display/descriptions/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/descriptions/token.md`
- 变更记录：`docs/controls/desktop/data-display/descriptions/changelog.md`
- 语义结构：`./semantic-cn.md`
