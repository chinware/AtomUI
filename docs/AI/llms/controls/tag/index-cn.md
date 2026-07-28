# Tag

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Tag 是 AtomUI 桌面控件体系中的标签控件，用于展示状态、分类、可关闭标记和预设色语义。

Tag 不负责复杂筛选器、按钮或徽标计数。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Controls/Tag`
- `src/AtomUI.Desktop.Controls/Tag`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tag` |
| 状态 | Stable |

## 何时使用

Tag 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Tag 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Tag 是 AtomUI 桌面控件体系中的标签控件，用于展示状态、分类、可关闭标记和预设色语义。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `CloseIcon`、`Icon`、`Text`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | input/value、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Tag Token + ControlTheme。 |

## 公共 API

Tag 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CloseIcon`、`Icon`、`Text` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsBordered`、`IsClosable` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `TagColor` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |

稳定事件包括 `Closed`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`AbstractTag`、`Tag`。
- 枚举：`TagStatus`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_CloseButton` | `AbstractIconButton` | 承载用户触发入口、导航或关闭动作。 |

控件专属或内部伪类包括 `CustomColor=:custom-color`、`PresetColor=:preset-color`、`StatusColor=:status-color`、`TagPseudoClass.CustomColor`、`TagPseudoClass.PresetColor`、`TagPseudoClass.StatusColor`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

Tag 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `Closed`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 多彩标签

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tag/Views/TagShowCase.axaml:58`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical">
    <atom:TextBlock FontWeight="Bold" FontSize="14" Margin="0, 0, 0, 10" Text="预设" />
    <WrapPanel HorizontalAlignment="Left">
        <atom:Tag TagColor="magenta" Text="品红色" />
        <atom:Tag TagColor="red" Text="红色" />
        <atom:Tag TagColor="volcano" Text="火山色" />
        <atom:Tag TagColor="orange" Text="橙色" />
        <atom:Tag TagColor="gold" Text="金色" />
        <atom:Tag TagColor="lime" Text="青柠色" />
        <atom:Tag TagColor="green" Text="绿色" />
        <atom:Tag TagColor="cyan" Text="青色" />
        <atom:Tag TagColor="blue" Text="蓝色" />
        <atom:Tag TagColor="geekblue" Text="极客蓝" />
        <atom:Tag TagColor="purple" Text="紫色" />
    </WrapPanel>

    <atom:TextBlock FontWeight="Bold" FontSize="14" Margin="0, 20, 0, 10" Text="自定义" />
    <WrapPanel HorizontalAlignment="Left">
        <atom:Tag TagColor="#f50">#f50</atom:Tag>
        <atom:Tag TagColor="#2db7f5" IsClosable="True">#2db7f5</atom:Tag>
        <atom:Tag TagColor="#87d068">#87d068</atom:Tag>
        <atom:Tag TagColor="#108ee9">#108ee9</atom:Tag>
    </WrapPanel>
</StackPanel>
```

### 无边框

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tag/Views/TagShowCase.axaml:154`

Gallery key：`ExamplesContent` / item `4`

```axaml
<StackPanel Orientation="Vertical">
    <WrapPanel HorizontalAlignment="Left">
        <atom:Tag IsBordered="False" Text="标签1" />
        <atom:Tag IsBordered="False" Text="标签2" />
        <atom:Tag IsBordered="False" IsClosable="True" Text="标签3" />
        <atom:Tag IsBordered="False" IsClosable="True" Text="标签4" />
    </WrapPanel>

    <atom:Separator FontWeight="Bold" FontSize="14" Margin="0, 20, 0, 20" />
    <WrapPanel HorizontalAlignment="Left">
        <atom:Tag TagColor="magenta" IsBordered="False" Text="品红色" />
        <atom:Tag TagColor="red" IsBordered="False" Text="红色" />
        <atom:Tag TagColor="volcano" IsBordered="False" Text="火山色" />
        <atom:Tag TagColor="orange" IsBordered="False" Text="橙色" />
        <atom:Tag TagColor="gold" IsBordered="False" Text="金色" />
        <atom:Tag TagColor="lime" IsBordered="False" Text="青柠色" />
        <atom:Tag TagColor="green" IsBordered="False" Text="绿色" />
        <atom:Tag TagColor="cyan" IsBordered="False" Text="青色" />
        <atom:Tag TagColor="blue" IsBordered="False" Text="蓝色" />
        <atom:Tag TagColor="geekblue" IsBordered="False" Text="极客蓝" />
        <atom:Tag TagColor="purple" IsBordered="False" Text="紫色" />

        <atom:Tag TagColor="success" IsBordered="False" Text="成功" />
        <atom:Tag TagColor="info" IsBordered="False" Text="处理中" />
        <atom:Tag TagColor="error" IsBordered="False" Text="错误" />
        <atom:Tag TagColor="warning" IsBordered="False" Text="警告" />
    </WrapPanel>
</StackPanel>
```

## 状态模型

Tag 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- input/value、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Tag 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `TagTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TagThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Tag 使用 `TagToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 input/value、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Tag Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TagToken`，scope id 为 `Tag`，源码位于 `src/AtomUI.Desktop.Controls/Tag/TagToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
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

- `src/AtomUI.Controls/Tag/AbstractTag.cs`
- `src/AtomUI.Controls/Tag/TagEnums.cs`
- `src/AtomUI.Controls/Tag/TagPseudoClass.cs`
- `src/AtomUI.Desktop.Controls/Tag/Tag.cs`
- `src/AtomUI.Desktop.Controls/Tag/TagToken.cs`
- `src/AtomUI.Desktop.Controls/Tag/Themes/TagTheme.axaml`
- `src/AtomUI.Desktop.Controls/Tag/Themes/TagTheme.cs`
- `src/AtomUI.Desktop.Controls/Tag/Themes/TagThemes.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/tag/overview.md`
- 实现文档：`docs/controls/desktop/data-display/tag/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/tag/token.md`
- 变更记录：`docs/controls/desktop/data-display/tag/changelog.md`
- 语义结构：`./semantic-cn.md`
