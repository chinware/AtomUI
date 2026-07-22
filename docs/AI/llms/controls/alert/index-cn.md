# Alert

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Alert 是 AtomUI 桌面控件体系中的警告提示控件，用于展示页面内的成功、信息、警告和错误反馈。

Alert 不负责全局消息系统、模态确认或通知中心。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Alert`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Feedback/Alert` |
| 状态 | Stable |

## 何时使用

Alert 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Alert 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Alert 是 AtomUI 桌面控件体系中的警告提示控件，用于展示页面内的成功、信息、警告和错误反馈。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `CloseIcon`、`Description`、`IsShowIcon`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | 基础交互和主题状态。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Alert Token + ControlTheme。 |

## 公共 API

Alert 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CloseIcon`、`Description`、`IsShowIcon` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsClosable`、`IsMessageMarqueeEnabled` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 其他稳定入口 | `ExtraAction`、`Message`、`Type` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`Alert`。
- 枚举：`AlertType`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_CloseBtn` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

控件专属或内部伪类包括 `AlertPseudoClass.HasDescription`、`AlertPseudoClass.HasExtraAction`、`HasDescription=:has-description`、`HasExtraAction=:has-extra-action`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

Alert 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Alert/Views/AlertShowCase.axaml:36`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Orientation="Vertical">
    <atom:Alert Type="Success" Message="成功文本" />
</StackPanel>
```

### 更多类型

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Alert/Views/AlertShowCase.axaml:49`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Alert Type="Success" Message="成功文本" />
    <atom:Alert Type="Info" Message="信息文本" />
    <atom:Alert Type="Warning" Message="警告文本" />
    <atom:Alert Type="Error" Message="错误文本" />
</StackPanel>
```

### 含描述信息

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Alert/Views/AlertShowCase.axaml:83`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Alert Type="Success"
                Message="成功提示文本"
                Description="成功描述 成功描述 成功描述" />
    <atom:Alert Type="Info"
                Message="信息提示文本"
                Description="信息描述 信息描述 信息描述 信息描述" />
    <atom:Alert Type="Warning"
                Message="警告提示文本"
                Description="警告描述 警告描述 警告描述 警告描述" />
    <atom:Alert Type="Error"
                Message="错误提示文本"
                Description="错误描述 错误描述 错误描述 错误描述" />
</StackPanel>
```

### 图标

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Alert/Views/AlertShowCase.axaml:108`

Gallery key：`ExamplesContent` / item `4`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Alert Type="Success"
                Message="成功提示"
                IsShowIcon="True" />
    <atom:Alert Type="Info"
                Message="信息说明"
                IsShowIcon="True" />
    <atom:Alert Type="Warning"
                Message="警告"
                IsShowIcon="True"
                IsClosable="True" />
    <atom:Alert Type="Error"
                Message="错误"
                IsShowIcon="True" />

    <atom:Alert Type="Success"
                Message="成功提示"
                IsShowIcon="True"
                Description="关于成功提示文案的详细说明和建议。" />
    <atom:Alert Type="Info"
                Message="信息说明"
                IsShowIcon="True"
                Description="关于提示文案的补充说明和信息。" />
    <atom:Alert Type="Warning"
                Message="警告"
                IsClosable="True"
                IsShowIcon="True"
                Description="这是一条关于提示文案的警告通知。" />
    <atom:Alert Type="Error"
                Message="错误"
                IsShowIcon="True"
                Description="这是一条关于提示文案的错误信息。" />
</StackPanel>
```

## 状态模型

Alert 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- 基础交互和主题状态 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Alert 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AlertTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Alert 使用 `AlertToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 基础交互和主题状态 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Alert Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `AlertToken`，scope id 为 `Alert`，源码位于 `src/AtomUI.Desktop.Controls/Alert/AlertToken.cs`。

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

- `src/AtomUI.Desktop.Controls/Alert/Alert.cs`
- `src/AtomUI.Desktop.Controls/Alert/AlertToken.cs`
- `src/AtomUI.Desktop.Controls/Alert/Themes/AlertTheme.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/feedback/alert/overview.md`
- 实现文档：`docs/controls/desktop/feedback/alert/implementation.md`
- Token 文档：`docs/controls/desktop/feedback/alert/token.md`
- 变更记录：`docs/controls/desktop/feedback/alert/changelog.md`
- 语义结构：`./semantic-cn.md`
