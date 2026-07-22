# Notification

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

Notification 是 AtomUI 桌面控件体系中的通知控件，用于在窗口角落展示可关闭的较重反馈和进度信息。

Notification 不负责即时消息气泡、页面内 Alert 或模态确认。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Notifications`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Feedback/Notification` |
| 状态 | Stable |

## 何时使用

Notification 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Notification 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Notification 是 AtomUI 桌面控件体系中的通知控件，用于在窗口角落展示可关闭的较重反馈和进度信息。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `Icon`、`MaxItems`、`Title`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、loading/async、collection/filter、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Notification Token + ControlTheme。 |

## 公共 API

Notification 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Icon`、`MaxItems`、`Title` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `CurrentExpiration` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsClosed`、`IsClosing`、`IsMotionEnabled`、`IsPauseOnHover`、`IsShowProgress` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `Position`、`ProgressIndicatorBrush`、`ProgressIndicatorThickness` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `CardExpiredPollingInterval`、`CleanupPollingInterval`、`Expiration`、`NotificationType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

稳定事件包括 `NotificationClosed`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`Notification`、`NotificationCard`、`NotificationMoveDownInMotion`、`NotificationMoveDownOutMotion`、`NotificationMoveLeftInMotion`、`NotificationMoveLeftOutMotion`、`NotificationMoveRightInMotion`、`NotificationMoveRightOutMotion`、`NotificationMoveUpInMotion`、`NotificationMoveUpOutMotion`、`NotificationProgressBar`、`NotificationProgressBarVisibleConverter`、`WindowNotificationManager`。
- 枚举：`NotificationPosition`、`NotificationType`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_CloseButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_Items` | `Panel` | 承载集合项、布局面板或虚拟化内容。 |
| `PART_Layout` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

控件专属或内部伪类包括 `BottomCenter=:bottomcenter`、`BottomLeft=:bottomleft`、`BottomRight=:bottomright`、`NotificationPseudoClass.BottomCenter`、`NotificationPseudoClass.BottomLeft`、`NotificationPseudoClass.BottomRight`、`NotificationPseudoClass.TopCenter`、`NotificationPseudoClass.TopLeft`、`NotificationPseudoClass.TopRight`、`TopCenter=:topcenter`、`TopLeft=:topleft`、`TopRight=:topright`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

Notification 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `NotificationClosed`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Views/NotificationShowCase.axaml:32`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Button ButtonType="Primary" Click="ShowSimpleNotification" Content="显示通知" />
```

### 自动关闭时长

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Views/NotificationShowCase.axaml:41`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:Button ButtonType="Primary" Click="ShowNeverCloseNotification" Content="打开通知框" />
```

### 带图标通知

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Views/NotificationShowCase.axaml:50`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:Button ButtonType="Default" Click="ShowSuccessNotification" Content="成功" />
    <atom:Button ButtonType="Default" Click="ShowInfoNotification" Content="信息" />
    <atom:Button ButtonType="Default" Click="ShowWarningNotification" Content="警告" />
    <atom:Button ButtonType="Default" Click="ShowErrorNotification" Content="错误" />
</StackPanel>
```

### 弹出位置

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Views/NotificationShowCase.axaml:64`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button ButtonType="Primary" Click="ShowTopNotification" Content="顶部" />
        <atom:Button ButtonType="Primary" Click="ShowBottomNotification" Content="底部" />
    </StackPanel>
    <atom:Separator />
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button ButtonType="Primary" Click="ShowTopLeftNotification" Content="左上" />
        <atom:Button ButtonType="Primary" Click="ShowTopRightNotification" Content="右上" />
    </StackPanel>
    <atom:Separator />
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button ButtonType="Primary" Click="ShowBottomLeftNotification" Content="左下" />
        <atom:Button ButtonType="Primary" Click="ShowBottomRightNotification" Content="右下" />
    </StackPanel>
</StackPanel>
```

## 状态模型

Notification 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、loading/async、collection/filter、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Notification 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `NotificationCardTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `NotificationProgressBarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `NotificationsThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `WindowNotificationManagerTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |

Notification 使用 `NotificationToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、loading/async、collection/filter、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Notification Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `NotificationToken`，scope id 为 `Notification`，源码位于 `src/AtomUI.Desktop.Controls/Notifications/NotificationToken.cs`。

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

- `src/AtomUI.Desktop.Controls/Notifications/INotification.cs`
- `src/AtomUI.Desktop.Controls/Notifications/INotificationManager.cs`
- `src/AtomUI.Desktop.Controls/Notifications/Notification.cs`
- `src/AtomUI.Desktop.Controls/Notifications/NotificationCard.cs`
- `src/AtomUI.Desktop.Controls/Notifications/NotificationMotions.cs`
- `src/AtomUI.Desktop.Controls/Notifications/NotificationPosition.cs`
- `src/AtomUI.Desktop.Controls/Notifications/NotificationProgressBar.cs`
- `src/AtomUI.Desktop.Controls/Notifications/NotificationPseudoClass.cs`
- `src/AtomUI.Desktop.Controls/Notifications/NotificationToken.cs`
- `src/AtomUI.Desktop.Controls/Notifications/NotificationType.cs`
- `src/AtomUI.Desktop.Controls/Notifications/Themes/NotificationCardTheme.axaml`
- `src/AtomUI.Desktop.Controls/Notifications/Themes/NotificationProgressBarTheme.axaml`
- `src/AtomUI.Desktop.Controls/Notifications/Themes/NotificationsThemes.axaml`
- `src/AtomUI.Desktop.Controls/Notifications/Themes/WindowNotificationManagerTheme.axaml`
- `src/AtomUI.Desktop.Controls/Notifications/Utils/NotificationProgressBarVisibleConverter.cs`
- `src/AtomUI.Desktop.Controls/Notifications/WindowNotificationManager.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/feedback/notification/overview.md`
- 实现文档：`docs/controls/desktop/feedback/notification/implementation.md`
- Token 文档：`docs/controls/desktop/feedback/notification/token.md`
- 变更记录：`docs/controls/desktop/feedback/notification/changelog.md`
- 语义结构：`./semantic-cn.md`
