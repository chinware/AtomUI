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
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Alert Token + ControlTheme。 |

## 公共 API

Alert 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Message`、`Description`、`ExtraAction`、`CloseIcon` | 定义标题、详情、辅助操作和关闭图标内容。 |
| 交互与状态 | `Type`、`IsShowIcon`、`IsClosable`、`IsMessageMarqueeEnabled` | 表达反馈类型、图标、关闭入口和标题呈现状态。 |
| 视觉表面 | `StrokeDashArray` 与继承的 TemplatedControl 表面属性 | 支持 root Semantic Style 定制实线或虚线边框、背景、圆角与 padding。 |

`CloseRequest` 是控件专属 public 事件。点击 `PART_CloseBtn` 时触发该事件；Alert 不自动移除自身，也不拥有关闭动画或业务关闭策略。

主要公开类型与枚举：

- 类型：`Alert`。
- 枚举：`AlertType`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_CloseBtn` | `IconButton` | 关闭请求入口；模板重套用时解除旧订阅并订阅新 part。 |

控件专属或内部伪类包括 `AlertPseudoClass.HasDescription`、`AlertPseudoClass.HasExtraAction`、`HasDescription=:has-description`、`HasExtraAction=:has-extra-action`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

Alert 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
`CloseRequest` 是控件专属 public 事件。点击 `PART_CloseBtn` 时触发该事件；Alert 不自动移除自身，也不拥有关闭动画或业务关闭策略。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Alert/Views/AlertShowCase.axaml:90`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Orientation="Vertical">
    <atom:Alert Type="Success" Message="成功文本" />
</StackPanel>
```

### 更多类型

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Alert/Views/AlertShowCase.axaml:103`

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

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Alert/Views/AlertShowCase.axaml:137`

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

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Alert/Views/AlertShowCase.axaml:162`

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

- `Type` 决定背景、边框和当前可见的状态图标。
- `Description`、`ExtraAction`、`IsShowIcon`、`IsClosable` 与 `IsMessageMarqueeEnabled` 只切换静态模板节点状态，不改变 Semantic Part 数量。
- `CloseRequest` 由当前模板的 close button 触发，业务层决定是否隐藏、移除或替换 Alert。
- 模板重套用时必须解除旧 close button 订阅，并把 public API 对应状态投影到新模板。

## 主题与 Design Token

Alert 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AlertTheme.axaml` | 提供单一静态模板、状态 selector、Token 绑定和 Semantic marker。 |

Alert 使用 `AlertToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 基础交互和主题状态 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不删除或重命名 `root`、`icon`、`section`、`title`、`description`、`actions`、`close`，不改变 selector class、ContractType 或 cardinality。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Alert Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `AlertToken`，scope id 为 `Alert`，源码位于 `src/AtomUI.Desktop.Controls/Alert/AlertToken.cs`。

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
- Semantic Part 使用生成期 descriptor 和静态 marker，不增加运行时 VisualTree 搜索、反射或 selector 字符串组装。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/Alert/Alert.cs`
- `src/AtomUI.Desktop.Controls/Alert/Alert.SemanticParts.cs`
- `src/AtomUI.Desktop.Controls/Alert/AlertToken.cs`
- `src/AtomUI.Desktop.Controls/Alert/Themes/AlertTheme.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- SemanticParts 文件只声明 descriptor 元数据；生成器产生 descriptor、名称常量和六个 public Style Type。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/feedback/alert/overview.md`
- 实现文档：`docs/controls/desktop/feedback/alert/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/feedback/alert/semantic-part.md`
- Token 文档：`docs/controls/desktop/feedback/alert/token.md`
- 变更记录：`docs/controls/desktop/feedback/alert/changelog.md`
- 语义结构：`./semantic-cn.md`
