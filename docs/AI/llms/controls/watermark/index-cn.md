# Watermark

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Watermark 是 AtomUI 桌面控件体系中的水印控件，用于在目标元素上绘制文本或图片水印。

Watermark 不负责背景装饰、权限控制或图片处理服务。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Controls/Watermark`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark` |
| 状态 | Stable |

## 何时使用

Watermark 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Watermark 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Watermark 是 AtomUI 桌面控件体系中的水印控件，用于在目标元素上绘制文本或图片水印。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `Source`、`Text`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | collection/filter、input/value、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | SharedToken / 关联控件 Token + ControlTheme。 |

## 公共 API

Watermark 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Source`、`Text` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsCrossUsed`、`IsMirrorUsed` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `FontSize`、`Foreground`、`Height`、`HorizontalOffset`、`HorizontalSpace`、`VerticalOffset`、`VerticalSpace` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Opacity`、`Rotate` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`ImageGlyph`、`ImageGlyphExtension`、`TextGlyph`、`TextGlyphExtension`、`WatermarkGlyph`、`WatermarkGlyphExtension`。
- 枚举：无。

稳定 template part：

当前控件没有显式 `[TemplatePart]` 契约；主题节点仍通过 ControlTheme key、资源 key 和 Gallery 可观察行为形成稳定边界。

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

Watermark 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Views/WatermarkShowCase.axaml:34`

Gallery key：`ExamplesContent` / item `0`

```axaml
<Border Height="300" HorizontalAlignment="Stretch" atom:Watermark.Glyph="{atom:TextGlyph 'AtomUI'}" />
```

### 多行水印

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Views/WatermarkShowCase.axaml:44`

Gallery key：`ExamplesContent` / item `1`

```axaml
<Border Height="200"
        HorizontalAlignment="Stretch">
    <atom:Watermark.Glyph>
        <atom:TextGlyph Text="AtomUI
快乐工作"
                        FontSize="18"
                        Foreground="Gray" />
    </atom:Watermark.Glyph>
</Border>
```

### 图片水印

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Views/WatermarkShowCase.axaml:61`

Gallery key：`ExamplesContent` / item `2`

```axaml
<Border Height="400">
    <atom:Watermark.Glyph>
        <atom:ImageGlyph Source="/Assets/ATOMUI-LOGO.png" />
    </atom:Watermark.Glyph>
</Border>
```

### 自定义配置

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Views/WatermarkShowCase.axaml:75`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel atom:Watermark.Glyph="{atom:TextGlyph 'AtomUI'}">
    <atom:TextBlock TextWrapping="Wrap"
                    Text="数字世界的高速迭代让产品变得更加复杂，而人的意识和注意力资源是有限的。面对这种设计矛盾，追求自然交互将始终是 AtomUI 的一致方向。

自然的用户认知：根据认知心理学，外部信息约有 80% 通过视觉通道获得。界面设计中最重要的视觉元素，包括布局、色彩、插图、图标等，都应充分吸收自然规律，从而降低用户的认知成本，并带来真实、顺畅的感受。在一些场景中，适时加入听觉、触觉等其他感官通道，也可以创造更丰富、更自然的产品体验。

自然的用户行为：在与系统交互时，设计师应充分理解用户、系统角色和任务目标之间的关系，并结合上下文组织系统功能和服务。同时，可以运用行为分析、人工智能、传感器等方法辅助用户做出有效决策，减少用户的额外操作，节省用户的心智和体力资源，让人机交互更加自然。" />
    <Image Source="/Assets/watermark-sample.png" />
</StackPanel>
```

## 状态模型

Watermark 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- collection/filter、input/value、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Watermark 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

当前控件未抽取到专属 AXAML 主题文件；视觉契约主要来自继承控件、共享主题和资源 key。

Watermark 当前没有专属 Token 文档；主题通过 SharedToken、关联控件 Token 或继承主题资源表达视觉语义。运行时状态不得写入 Token 模型。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

- Watermark 当前没有专属 `token.md`；LLMS 生成按第 5 节视觉与主题模型、SharedToken、控件家族 Token 或主题资源说明 Token 边界。

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

- `src/AtomUI.Controls/Watermark/Glyphs/ImageGlyph.cs`
- `src/AtomUI.Controls/Watermark/Glyphs/ImageGlyphExtension.cs`
- `src/AtomUI.Controls/Watermark/Glyphs/TextGlyph.cs`
- `src/AtomUI.Controls/Watermark/Glyphs/TextGlyphExtension.cs`
- `src/AtomUI.Controls/Watermark/Glyphs/WatermarkGlyph.cs`
- `src/AtomUI.Controls/Watermark/Glyphs/WatermarkGlyphExtension.cs`
- `src/AtomUI.Controls/Watermark/Watermark.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/feedback/watermark/overview.md`
- 实现文档：`docs/controls/desktop/feedback/watermark/implementation.md`
- 变更记录：`docs/controls/desktop/feedback/watermark/changelog.md`
- 语义结构：`./semantic-cn.md`
