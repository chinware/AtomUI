# TabStrip

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

TabStrip 是 AtomUI 桌面控件体系中的标签条控件，用于只展示和管理页签选择，不承载内容页。

TabStrip 不负责完整 TabControl 内容容器或主导航菜单。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/TabControl/TabStrip`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip` |
| 状态 | Stable |

## 何时使用

TabStrip 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | TabStrip 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | TabStrip 是 AtomUI 桌面控件体系中的标签条控件，用于只展示和管理页签选择，不承载内容页。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `CloseIcon`、`HeaderEndEdgePadding`、`HeaderEndExtraContent`、`HeaderEndExtraContentTemplate`、`HeaderStartEdgePadding`、`HeaderStartExtraContent`、`HeaderStartExtraContentTemplate`、`Icon`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | SharedToken / 关联控件 Token + ControlTheme。 |

## 公共 API

TabStrip 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CloseIcon`、`HeaderEndEdgePadding`、`HeaderEndExtraContent`、`HeaderEndExtraContentTemplate`、`HeaderStartEdgePadding`、`HeaderStartExtraContent`、`HeaderStartExtraContentTemplate`、`Icon` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsAutoHideCloseButton`、`IsClosable`、`IsMotionEnabled`、`IsShowAddTabButton`、`IsTabAutoHideCloseButton`、`IsTabClosable` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `SizeType`、`TabAlignmentCenter`、`TabStripPlacement` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |

稳定事件包括 `AddTabRequest`、`Closed`、`Closing`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`BaseTabStrip`、`CardTabStrip`、`TabStrip`、`TabStripClosedEventArgs`、`TabStripClosingEventArgs`、`TabStripItem`、`TabStripOverflowMenuItem`、`TabStripScrollViewer`。
- 枚举：`TabSharp`。

稳定 template part：

当前控件没有显式 `[TemplatePart]` 契约；主题节点仍通过 ControlTheme key、资源 key 和 Gallery 可观察行为形成稳定边界。

控件专属或内部伪类包括 `TabPseudoClass.Bottom`、`TabPseudoClass.Left`、`TabPseudoClass.Right`、`TabPseudoClass.Top`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

TabStrip 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `AddTabRequest`、`Closed`、`Closing`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。
- 类型：`BaseTabStrip`、`CardTabStrip`、`TabStrip`、`TabStripClosedEventArgs`、`TabStripClosingEventArgs`、`TabStripItem`、`TabStripOverflowMenuItem`、`TabStripScrollViewer`。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripShowCase.axaml:141`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:TabStrip>
        <atom:TabStripItem IsClosable="True" Content="标签页 1" />
        <atom:TabStripItem IsClosable="True" Content="标签页 2" />
        <atom:TabStripItem IsClosable="True" Content="标签页 3" />
    </atom:TabStrip>
</StackPanel>
```

### 通过 ItemSource 生成 TabStripItem

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripShowCase.axaml:158`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:TabStrip ItemsSource="{Binding TabStripItemDataSource}">
        <atom:TabStrip.ItemTemplate>
            <DataTemplate x:DataType="atom:TabItemData">
                <TextBlock Text="{Binding Header}" />
            </DataTemplate>
        </atom:TabStrip.ItemTemplate>
    </atom:TabStrip>
</StackPanel>
```

### 禁用标签

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripShowCase.axaml:177`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:CardTabStrip>
        <atom:TabStripItem Content="标签页 1" />
        <atom:TabStripItem IsEnabled="False" IsClosable="True" Content="标签页 2" />
        <atom:TabStripItem Content="标签页 3" />
    </atom:CardTabStrip>

    <atom:TabStrip>
        <atom:TabStripItem Content="标签页 1" />
        <atom:TabStripItem IsEnabled="False" Content="标签页 2" />
        <atom:TabStripItem Content="标签页 3" />
    </atom:TabStrip>
</StackPanel>
```

### 居中显示

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripShowCase.axaml:200`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:TabStrip TabAlignmentCenter="True">
        <atom:TabStripItem Content="标签页 1" />
        <atom:TabStripItem Content="标签页 2" />
        <atom:TabStripItem Content="标签页 3" />
    </atom:TabStrip>

    <atom:CardTabStrip TabAlignmentCenter="True">
        <atom:TabStripItem Content="标签页 1" />
        <atom:TabStripItem Content="标签页 2" />
        <atom:TabStripItem Content="标签页 3" />
    </atom:CardTabStrip>
</StackPanel>
```

## 状态模型

TabStrip 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

TabStrip 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

当前控件未抽取到专属 AXAML 主题文件；视觉契约主要来自继承控件、共享主题和资源 key。

TabStrip 当前没有专属 Token 文档；主题通过 SharedToken、关联控件 Token 或继承主题资源表达视觉语义。运行时状态不得写入 Token 模型。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

- TabStrip 当前没有专属 `token.md`；LLMS 生成按第 5 节视觉与主题模型、SharedToken、控件家族 Token 或主题资源说明 Token 边界。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/TabControl/TabStrip/BaseTabStrip.cs`
- `src/AtomUI.Desktop.Controls/TabControl/TabStrip/CardTabStrip.cs`
- `src/AtomUI.Desktop.Controls/TabControl/TabStrip/TabStrip.cs`
- `src/AtomUI.Desktop.Controls/TabControl/TabStrip/TabStripItem.cs`
- `src/AtomUI.Desktop.Controls/TabControl/TabStrip/TabStripOverflowMenuItem.cs`
- `src/AtomUI.Desktop.Controls/TabControl/TabStrip/TabStripScrollViewer.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/navigation/tab-strip/overview.md`
- 实现文档：`docs/controls/desktop/navigation/tab-strip/implementation.md`
- 变更记录：`docs/controls/desktop/navigation/tab-strip/changelog.md`
- 语义结构：`./semantic-cn.md`
