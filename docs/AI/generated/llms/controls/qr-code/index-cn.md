# QRCode

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

QRCode 是 AtomUI 桌面控件体系中的二维码控件，用于把文本或业务字符串渲染为可扫描二维码。

QRCode 不负责条码识别器、扫码硬件接口或业务短链服务。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/QRCode`
- `src/AtomUI.Controls/QRCode`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode` |
| 状态 | Stable |

## 何时使用

QRCode 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | QRCode 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | QRCode 是 AtomUI 桌面控件体系中的二维码控件，用于把文本或业务字符串渲染为可扫描二维码。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `ExpiredContent`、`ExpiredContentTemplate`、`Icon`、`IconBgColor`、`IconSize`、`LoadingContent`、`LoadingContentTemplate`、`ScannedContent` 等 10 项。 |
| 状态反馈 | public API、内部状态和模板节点如何形成用户可感知反馈。 | `Active`、`Loading`、`Expired`、`Scanned` 与刷新请求。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | QRCode Token + ControlTheme。 |

## 公共 API

QRCode 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ExpiredContent`、`ExpiredContentTemplate`、`Icon`、`IconBgColor`、`IconSize`、`LoadingContent`、`LoadingContentTemplate`、`ScannedContent`、`ScannedContentTemplate`、`Value` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsBordered`、`Status`、`RefreshRequested` | 表达边框模式、状态遮罩和过期状态下的刷新请求。 |
| 视觉与布局 | `Color`、`Size` 及继承的 root 表面属性 | `Size` 统一拥有二维码方形边长；颜色、背景、边框、圆角和 Padding 形成 root 视觉。 |
| 其他稳定入口 | `EccLevel` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

`RefreshRequested` 是 QRCode 的控件专属 public 事件，由默认过期状态中的 `PART_RefreshButton` 触发。自定义
`ExpiredContent` 不会自动转发该事件，调用方需要在自定义内容中显式处理自己的命令或事件。

主要公开类型与枚举：

- 类型：`AbstractQRCode`、`QRCode`。
- 枚举：`QRCodeEccLevel`、`QRCodeStatus`。
- 本地化 Catalog：`QRCodeLangResourceKind`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_RefreshButton` | `Button` | 承载用户触发入口、导航或关闭动作。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

QRCode 公开 `root` 和 `cover` 两个 Semantic Part。`root` 是 QRCode owner；`cover` 是状态遮罩与状态内容共同占用的静态 overlay
区域。完整 Selector、ContractType、cardinality 和定制边界见 [QRCode Semantic Part 契约](semantic-part.md)。二维码 bitmap、中心图标、
刷新按钮和各状态内部内容不单独公开为 Semantic Part。

## 事件与命令

QRCode 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
`RefreshRequested` 是 QRCode 的控件专属 public 事件，由默认过期状态中的 `PART_RefreshButton` 触发。自定义
`ExpiredContent` 不会自动转发该事件，调用方需要在自定义内容中显式处理自己的命令或事件。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Views/QRCodeShowCase.axaml:55`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Orientation="Vertical">
    <atom:QRCode Value="{Binding QRCodeInput}" />
    <atom:LineEdit Text="{Binding QRCodeInput}" Margin="0,20,0,0" />
    <ListBox>
        <ListBox.ItemTemplate></ListBox.ItemTemplate>
    </ListBox>
</StackPanel>
```

### 带 Icon 的例子

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Views/QRCodeShowCase.axaml:70`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical">
    <atom:QRCode Value="https://atomui.net" Icon="avares://AtomUIGallery/Assets/ATOMUI-LOGO.png" />
</StackPanel>
```

### 不同的状态

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Views/QRCodeShowCase.axaml:82`

Gallery key：`ExamplesContent` / item `2`

```axaml
<WrapPanel ItemSpacing="20" LineSpacing="20" Orientation="Horizontal">
    <atom:QRCode Value="https://atomui.net" Status="Loading" />
    <atom:QRCode Value="https://atomui.net" Status="Expired" />
    <atom:QRCode Value="https://atomui.net" Status="Scanned" />
</WrapPanel>
```

### 自定义颜色

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Views/QRCodeShowCase.axaml:157`

Gallery key：`ExamplesContent` / item `5`

```axaml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:QRCode Value="https://atomui.net"
                 Color="{atom:SharedTokenResource ColorSuccessText}" />
    <atom:QRCode Value="https://atomui.net"
                 Color="{atom:SharedTokenResource ColorInfoText}"
                 Background="{atom:SharedTokenResource ColorBgLayout}" />
</StackPanel>
```

## 状态模型

QRCode 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- `Active` 隐藏 cover；`Loading`、`Expired`、`Scanned` 显示同一个 cover 节点，并在节点内部切换对应状态内容。
- `Value`、`Color`、`EccLevel` 或 `Size` 变化时重新生成透明背景 bitmap，不替换 Semantic target；`Background` 只更新 root 表面。
- `Icon` 只控制二维码中心图标内容，不增加 Semantic Part，也不改变 root/cover 数量。
- 模板重套用时重新接入 `PART_RefreshButton`，先移除旧按钮订阅，再回放当前 public API 状态。
- `Size` 是二维码外框与绘制源的统一边长；Semantic Style 不建立第二套 Width/Height 尺寸 owner。

## 主题与 Design Token

QRCode 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `QRCodeTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

QRCode 使用 `QRCodeToken` 作为控件 Token scope。Token 只表达文字色和 cover 背景色等视觉语义，不承载 `Status`、`Value` 或 bitmap
运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

QRCode Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `QRCodeToken`，scope id 为 `QRCode`，源码位于 `src/AtomUI.Desktop.Controls/QRCode/QRCodeToken.cs`。

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
- 状态切换不得重新生成 Semantic marker、cover 或状态内容容器；只有二维码绘制输入变化时才重新生成 bitmap。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/QRCode/Localization/QRCodeLangResourceKind.cs`
- `src/AtomUI.Desktop.Controls/QRCode/Localization/en-US.xlf`
- `src/AtomUI.Desktop.Controls/QRCode/Localization/zh-CN.xlf`
- `src/AtomUI.Desktop.Controls/QRCode/Localization/zh-TW.xlf`
- `src/AtomUI.Desktop.Controls/QRCode/QRCode.cs`
- `src/AtomUI.Desktop.Controls/QRCode/QRCode.SemanticParts.cs`
- `src/AtomUI.Desktop.Controls/QRCode/QRCodeToken.cs`
- `src/AtomUI.Desktop.Controls/QRCode/Themes/QRCodeTheme.axaml`
- `src/AtomUI.Controls/QRCode/AbstractQRCode.cs`
- `src/AtomUI.Controls/QRCode/QRCodeEnums.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/qr-code/overview.md`
- 实现文档：`docs/controls/desktop/data-display/qr-code/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/data-display/qr-code/semantic-part.md`
- Token 文档：`docs/controls/desktop/data-display/qr-code/token.md`
- 变更记录：`docs/controls/desktop/data-display/qr-code/changelog.md`
- 语义结构：`./semantic-cn.md`
