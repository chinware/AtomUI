# Button

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

Button 是 AtomUI 桌面控件体系中的基础动作触发控件，用于承载用户可执行的明确操作。Button 负责把动作语义稳定映射为公共 API、交互状态、主题 Token、模板结构和反馈效果。

Button 不承担复杂内容布局、导航结构管理或业务状态表达职责。复杂按钮形态应通过 Button 家族控件扩展，而不是扩大 Button 本体的职责边界。

Button 家族包括 `DropdownButton`、`SplitButton`、`IconButton` 和 `HyperLinkButton`。这些控件可以拥有不同模板结构，但必须共享 Button 的动作语义、状态解释、尺寸体系和主题资源。

典型使用场景：

- 提交、保存、删除、确认、取消等明确动作。
- 表达主动作、普通动作、弱强调动作、链接式动作和危险动作。
- 在工具栏、表单、对话框、卡片和列表项中提供可点击操作入口。
- 使用图标、加载态、禁用态、尺寸和形状增强动作可识别性。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/General/Button` |
| 状态 | Stable |

## 何时使用

典型使用场景：

- 提交、保存、删除、确认、取消等明确动作。
- 表达主动作、普通动作、弱强调动作、链接式动作和危险动作。
- 在工具栏、表单、对话框、卡片和列表项中提供可点击操作入口。
- 使用图标、加载态、禁用态、尺寸和形状增强动作可识别性。

## 公共 API

Button 的公共 API 是控件最重要的稳定契约。公共属性、事件和方法集中在 `Button.cs`，内部主题变量和实现细节不得替代公共 API。

兼容 API：

- `ButtonType`: `Default`、`Dashed`、`Primary`、`Link`、`Text`。
- `ButtonShape`: `Default`、`Circle`、`Round`。
- `IsDanger`、`IsGhost`、`IsLoading`。
- `SizeType`、`Icon`、`IconPlacement`。
- `IsMotionEnabled`、`IsWaveSpiritEnabled`。
- `CustomBackground`。

正交 API：

```csharp
public ButtonColor? Color { get; set; }
public ButtonVariant? Variant { get; set; }
```

`Color` 与 `Variant` 使用 nullable 类型，用于区分“未参与新模型”和“显式设置默认值”。该区分是保持 `ButtonType`、`IsDanger` 与正交 API 兼容优先级的必要条件。

解析优先级：

```text
显式 Color + Variant
> ButtonType / IsDanger 兼容映射
> Variant=Solid 自动补 Color=Primary
> Color=Default, Variant=Outlined
```

旧 API 映射：

| 旧 API | 正交模型 |
| --- | --- |
| `ButtonType=Default` | `Color=Default, Variant=Outlined` |
| `ButtonType=Primary` | `Color=Primary, Variant=Solid` |
| `ButtonType=Dashed` | `Color=Default, Variant=Dashed` |
| `ButtonType=Text` | `Color=Default, Variant=Text` |
| `ButtonType=Link` | internal link color, `Variant=Link` |

`ButtonColor` 不暴露 `Link`。`ButtonType.Link` 是兼容入口，内部映射到链接视觉。

`CustomBackground` 表示 Button normal 状态的受控自定义背景覆层，主要用于渐变、图片或其他非纯色表面。它不是颜色语义，不参与 `Color + Variant` 的状态归一、文字色、边框色、阴影或 wave 颜色计算。`CustomBackground == null` 表示不启用自定义背景覆层。

`SizeType` 使用可自定义尺寸模型，支持 `Large`、`Middle`、`Small` 和 `Custom`。`Large`、`Middle`、`Small` 是 Button 预设尺寸档，完全由 Token 和主题决定。`Custom` 表示用户希望基于 Button 现有属性进行实例级尺寸定制，而不是引入 Button 专属的 `CustomHeight`、`CustomPadding` 或尺寸对象。

`IconPlacement` 控制 `Icon` 相对内容的位置，支持 `Start` 和 `End`。默认值必须是 `Start`，以保持既有 `Icon` 使用方式不变。`End` 只改变用户图标与内容的排列方向，不改变 loading、icon-only、尺寸、颜色、变体或交互状态语义。

```csharp
public enum ButtonIconPlacement
{
    Start,
    End
}

public ButtonIconPlacement IconPlacement { get; set; }
```

Template part 与主题入口：

| 节点 | 职责 |
| --- | --- |
| `PART_WaveSpirit` | 承载点击 wave 反馈。 |
| `ShadowsFrame` | 承载按钮阴影。 |
| `Frame` | 承载主体背景、边框、圆角和尺寸基底。 |
| `CustomBackgroundLayer` | 主题内部自定义背景覆层，不作为用户 template part。 |
| `PART_RootLayout` | 排列 loading icon、icon 和 content，并根据 `IconPlacement` 调整用户 icon 位置。 |
| `PART_LoadingIcon` | 展示 loading 状态图标。 |
| `PART_ButtonIcon` | 展示用户设置的 icon，位置由 `IconPlacement` 控制。 |
| `PART_ContentPresenter` | 展示用户内容。 |

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Button` | 控件根语义区域，承载 public API、命令、点击、状态归一和伪类。 | `ButtonType`、`Color`、`Variant`、`IsDanger`、`IsGhost`、`IsLoading`、`SizeType`、`Shape`、`Icon`、`IconPlacement` | ButtonToken、SharedToken | stable |
| `wave` | `PART_WaveSpirit` | 点击 wave 反馈区域，跟随有效圆角和 wave 类型。 | `IsWaveSpiritEnabled`、`IsMotionEnabled` | SharedToken motion / wave 资源 | stable |
| `shadow` | `ShadowsFrame` | 阴影绘制层，独立于主体背景和边框。 | effective state | `DefaultShadow`、`PrimaryShadow`、`DangerShadow` | stable |
| `surface` | `Frame` | 主体背景、边框、圆角、尺寸和虚线边框绘制层。 | `ButtonType`、`Color`、`Variant`、`Shape`、`SizeType`、`CornerRadius`、`Padding` | default、primary、danger、text、link、padding、corner radius 相关 Token | stable |
| `customBackground` | `CustomBackgroundLayer` | normal 状态自定义背景覆层，只服务 `CustomBackground` 视觉模型。 | `CustomBackground` | 不新增专属 Token | internal-stable |
| `contentLayout` | `PART_RootLayout` | loading icon、用户 icon 和内容的排列区域。 | `IconPlacement`、`HorizontalContentAlignment`、`VerticalContentAlignment` | `IconMargin`、尺寸 Token | stable |
| `loadingIcon` | `PART_LoadingIcon` | loading 状态图标区域。 | `IsLoading` | `IconSize`、`OnlyIconSize` 相关 Token | stable |
| `icon` | `PART_ButtonIcon` | 用户 icon 区域，支持内容前后位置和 icon-only 场景。 | `Icon`、`IconPlacement` | `IconSize`、`OnlyIconSize`、`IconMargin` | stable |
| `content` | `PART_ContentPresenter` | 用户内容展示区域。 | `Content`、`ContentTemplate` | `ContentFontSize`、`ContentLineHeight`、`FontWeight` | stable |

`CustomBackgroundLayer` 是主题内部实现细节，不作为用户可直接依赖的 template part。LLMS semantic 文档可以记录它的存在和边界，但应明确它只服务 `CustomBackground` 受控视觉模型。

## 事件与命令

Button 的公共 API 是控件最重要的稳定契约。公共属性、事件和方法集中在 `Button.cs`，内部主题变量和实现细节不得替代公共 API。
| `root` | `Button` | 控件根语义区域，承载 public API、命令、点击、状态归一和伪类。 | `ButtonType`、`Color`、`Variant`、`IsDanger`、`IsGhost`、`IsLoading`、`SizeType`、`Shape`、`Icon`、`IconPlacement` | ButtonToken、SharedToken | stable |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 按钮类型

来源：`controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml:43`

SourceKey：`button-type`

```axaml
<WrapPanel HorizontalAlignment="Left" Orientation="Horizontal">
    <atom:Button ButtonType="Primary" Content="主要按钮" />
    <atom:Button Content="默认按钮" />
    <atom:Button ButtonType="Dashed" Content="虚线" />
    <atom:Button ButtonType="Text" Content="文本按钮" />
    <atom:Button ButtonType="Link" Content="链接按钮" />
</WrapPanel>
```

### 按钮形状

来源：`controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml:61`

SourceKey：`button-shape`

```axaml
<StackPanel Orientation="Vertical">
    <WrapPanel HorizontalAlignment="Left" Orientation="Horizontal" Margin="0, 0, 0, 20">
        <atom:Button ButtonType="Primary" Content="主要" />
        <atom:Button Content="默认" />

        <atom:Button ButtonType="Text" Content="文本" />
        <atom:Button ButtonType="Link" Content="链接" />
    </WrapPanel>
    <WrapPanel HorizontalAlignment="Left" Orientation="Horizontal" Margin="0, 0, 0, 20">
        <atom:Button ButtonType="Primary" Shape="Round" Content="主要" />
        <atom:Button Shape="Round" Content="默认" />
        <atom:Button ButtonType="Text" Shape="Round" Content="文本" />
        <atom:Button ButtonType="Link" Shape="Round" Content="链接" />
    </WrapPanel>
    <StackPanel HorizontalAlignment="Left" Spacing="10" Orientation="Horizontal" Margin="0, 0, 0, 20">
        <atom:Button ButtonType="Primary" Shape="Circle" Content="AA" />
        <atom:Button Shape="Circle" Content="AA" />
        <atom:Button ButtonType="Text" Shape="Circle" Content="AA" />
        <atom:Button ButtonType="Link" Shape="Circle" Content="AA" />
    </StackPanel>
</StackPanel>
```

### 通栏按钮

来源：`controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml:318`

SourceKey：`button-block`

```axaml
<StackPanel HorizontalAlignment="Stretch" Orientation="Vertical" Margin="10">
    <atom:Button ButtonType="Primary" HorizontalAlignment="Stretch" Content="主要" />
    <atom:Button ButtonType="Default" HorizontalAlignment="Stretch" Content="默认" />
    <atom:Button ButtonType="Dashed" HorizontalAlignment="Stretch" Content="虚线" />
    <atom:Button ButtonType="Text" HorizontalAlignment="Stretch" Content="文本" />
    <atom:Button ButtonType="Link" HorizontalAlignment="Stretch" Content="链接" />
</StackPanel>
```

### 危险按钮

来源：`controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml:336`

SourceKey：`button-danger`

```axaml
<WrapPanel HorizontalAlignment="Stretch" Orientation="Horizontal">
    <atom:Button ButtonType="Primary" IsDanger="True" Content="主要" />
    <atom:Button ButtonType="Default" IsDanger="True" Content="默认" />
    <atom:Button ButtonType="Dashed" IsDanger="True" Content="虚线" />
    <atom:Button ButtonType="Text" IsDanger="True" Content="文本" />
    <atom:Button ButtonType="Link" IsDanger="True" Content="链接" />
</WrapPanel>
```

## 状态模型

Button 的交互优先级：

```text
Disabled
> Loading
> Pressed
> PointerOver
> Normal
```

`Disabled` 表示控件不可交互，应屏蔽 hover、pressed、wave 等交互反馈。`Loading` 表示操作正在进行中，应影响 loading icon、透明度、原 icon 展示和 wave 播放条件，但不等价于 API 层面的禁用状态。

Button 继承 Avalonia Button 的基础点击、命令和键盘行为。AtomUI 扩展逻辑不得绕过或破坏基础控件语义。

Button 的 effective state 由 C# 层归一，AXAML 主题只消费已经归一的状态或主题变量。核心 effective state 包括：

- `EffectiveColor`、`EffectiveVariant`。
- `EffectiveIsDanger`、`EffectiveIsGhost`、`EffectiveIsBordered`。
- `EffectiveBorderThickness`、`EffectiveCornerRadius`。
- `WaveSpiritType`。
- icon-only、loading、custom background 可见性相关伪类。

## 主题与 Design Token

Button 模板应保持阴影层、主体绘制层、内容层、wave 层和自定义背景覆层的职责分离。可以移除无明确职责的包装层，但不得合并承担不同视觉职责的节点。

Button 主题采用分层变量模型，避免直接展开 `Color × Variant × State` 的组合样式。

```text
Color Selector
  设置颜色语义变量

Variant Selector
  将颜色语义变量映射到文字、背景、边框、阴影变量

State Selector
  将 Normal / PointerOver / Pressed / Disabled / Loading 状态应用到最终视觉属性

Custom Background Selector
  在受支持状态显示自定义背景覆层，在 hover / pressed / disabled / danger 状态隐藏覆层
```

用于 AXAML `Setter`、selector、动态资源和主题切换的变量应定义为 internal `StyledProperty`。普通 CLR 属性不适合作为主题变量，`DirectProperty` 仅适用于不参与 Style 系统的内部运行时状态。

ButtonToken 负责提供组件级语义值，Theme 负责把状态映射为视觉属性。Token 的分类、用途、边界和扩展策略见 [Button Token 设计](token.md)。

Token 来源：

ButtonToken 是 Button 的组件级设计变量层。它把全局设计体系中的颜色、尺寸、间距、字体和阴影转换为 Button 可消费的语义值。

ButtonToken 服务以下主题和控件：

- `ButtonTheme.axaml`
- `BrowserButtonThemes.axaml`
- `DropdownButtonTheme.axaml`
- `SplitButtonTheme.axaml`
- `HyperLinkButtonTheme.axaml`
- Button 家族控件中的 icon、额外内容、下拉间距和状态视觉

ButtonToken 不承载 `IsPressed`、`IsPointerOver`、`IsLoading`、`EffectiveColor`、`EffectiveVariant` 等实例状态。这些状态由 Button 状态模型和主题变量处理。

## AOT 与裁剪注意事项

Button 主题变量使用 Avalonia 属性和动态资源，不使用反射读取模板状态。Token 资源由 ButtonToken scope 提供，并跟随主题切换。

Custom background 覆层是现有模板内的一层视觉节点，启用时不应增加额外控件实例或重建模板。未设置 `CustomBackground` 时，覆层保持不可见，不应影响默认路径的命中测试、wave 或内容布局。

Button 实现不得引入运行时反射、动态代码生成或非 AOT 友好的资源查找路径。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Buttons/Button.cs`：Button public API、Avalonia 属性注册、effective state、伪类同步、CompactSpace / Form / Wave 接口实现。
- `src/AtomUI.Desktop.Controls/Buttons/ButtonToken.cs`：Button 组件 Token 定义与派生。
- `src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonTheme.axaml`：桌面 Button 模板、状态 selector 和主题变量映射。
- `src/AtomUI.Desktop.Controls/Buttons/Themes/BrowserButtonThemes.axaml`：Browser 风格 Button 主题。
- `src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonTheme.cs`：主题资源注册辅助。
- `src/AtomUI.Controls/Buttons/ButtonPseudoClass.cs`：共享 Button 伪类定义。
- `src/AtomUI.Controls/Buttons/Converters/ButtonIconVisibleConverter.cs`：icon 可见性转换辅助。
- `src/AtomUI.Desktop.Controls/Buttons/DropdownButton.cs`、`SplitButton.cs`、`IconButton.cs`、`HyperLinkButton.cs`：Button 家族控件。

## 相关文档

- 源设计文档：`docs/controls/desktop/general/button/overview.md`
- 实现文档：`docs/controls/desktop/general/button/implementation.md`
- Token 文档：`docs/controls/desktop/general/button/token.md`
- 变更记录：`docs/controls/desktop/general/button/changelog.md`
- 语义结构：`./semantic-cn.md`
