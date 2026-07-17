# Breadcrumb

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

Breadcrumb 是 AtomUI 桌面控件体系中的面包屑导航控件，用于展示当前位置路径并提供上级跳转入口。

Breadcrumb 不负责树控件、主导航菜单或完整路由系统。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Breadcrumb`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb` |
| 状态 | Stable |

## 何时使用

Breadcrumb 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Breadcrumb 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Breadcrumb 是 AtomUI 桌面控件体系中的面包屑导航控件，用于展示当前位置路径并提供上级跳转入口。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `Icon`、`SeparatorTemplate`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | motion。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Breadcrumb Token + ControlTheme。 |

## 公共 API

Breadcrumb 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Icon`、`SeparatorTemplate` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsMotionEnabled` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 其他稳定入口 | `NavigateContext`、`NavigateUri`、`Separator` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`Breadcrumb`、`BreadcrumbItem`、`BreadcrumbItemData`、`BreadcrumbNavigateEventArgs`。
- 枚举：无。

稳定 template part：

当前控件没有显式 `[TemplatePart]` 契约；主题节点仍通过 ControlTheme key、资源 key 和 Gallery 可观察行为形成稳定边界。

控件专属或内部伪类包括 `BreadcrumbPseudoClass.HasIcon`、`BreadcrumbPseudoClass.IsLast`、`HasIcon=:has-icon`、`IsLast=:is-last`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

Breadcrumb 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。
- 类型：`Breadcrumb`、`BreadcrumbItem`、`BreadcrumbItemData`、`BreadcrumbNavigateEventArgs`。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb/Views/BreadcrumbShowCase.axaml:37`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel>
    <atom:Breadcrumb>
        <atom:BreadcrumbItem Content="Home" />
        <atom:BreadcrumbItem NavigateContext="#" Content="Application Center" />
        <atom:BreadcrumbItem NavigateContext="#" Content="Application List" />
        <atom:BreadcrumbItem Content="An Application" />
    </atom:Breadcrumb>
</StackPanel>
```

### 带参数

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb/Views/BreadcrumbShowCase.axaml:74`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel>
    <atom:Breadcrumb NavigateRequest="HandleNavigateRequest">
        <atom:BreadcrumbItem Content="Users" />
        <atom:BreadcrumbItem NavigateContext="Param(1)" Content="Param" />
    </atom:Breadcrumb>
</StackPanel>
```

### 配置分隔符

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb/Views/BreadcrumbShowCase.axaml:91`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel>
    <atom:Breadcrumb Separator=">">
        <atom:BreadcrumbItem Content="Home" />
        <atom:BreadcrumbItem NavigateContext="#" Content="Application Center" />
        <atom:BreadcrumbItem NavigateContext="#" Content="Application List" />
        <atom:BreadcrumbItem Content="An Application" />
    </atom:Breadcrumb>
</StackPanel>
```

### 单独配置分隔符

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb/Views/BreadcrumbShowCase.axaml:110`

Gallery key：`ExamplesContent` / item `4`

```axaml
<StackPanel>
    <atom:Breadcrumb>
        <atom:BreadcrumbItem Separator=":" Content="Location" />
        <atom:BreadcrumbItem NavigateContext="#" Content="Application Center" />
        <atom:BreadcrumbItem NavigateContext="#" Content="Application List" />
        <atom:BreadcrumbItem Content="An Application" />
    </atom:Breadcrumb>
</StackPanel>
```

## 状态模型

Breadcrumb 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- motion 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Breadcrumb 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `BreadcrumbItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `BreadcrumbTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `BreadcrumbThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Breadcrumb 使用 `BreadcrumbToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 motion 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Breadcrumb Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `BreadcrumbToken`，scope id 为 `Breadcrumb`，源码位于 `src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbToken.cs`。

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

- `src/AtomUI.Desktop.Controls/Breadcrumb/Breadcrumb.cs`
- `src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbItem.cs`
- `src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbItemData.cs`
- `src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbNavigateEventArgs.cs`
- `src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbPseudoClass.cs`
- `src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbToken.cs`
- `src/AtomUI.Desktop.Controls/Breadcrumb/IBreadcrumbItemData.cs`
- `src/AtomUI.Desktop.Controls/Breadcrumb/Themes/BreadcrumbItemTheme.axaml`
- `src/AtomUI.Desktop.Controls/Breadcrumb/Themes/BreadcrumbTheme.axaml`
- `src/AtomUI.Desktop.Controls/Breadcrumb/Themes/BreadcrumbThemes.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/navigation/breadcrumb/overview.md`
- 实现文档：`docs/controls/desktop/navigation/breadcrumb/implementation.md`
- Token 文档：`docs/controls/desktop/navigation/breadcrumb/token.md`
- 变更记录：`docs/controls/desktop/navigation/breadcrumb/changelog.md`
- 语义结构：`./semantic-cn.md`
