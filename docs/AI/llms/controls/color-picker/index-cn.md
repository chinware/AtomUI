# ColorPicker

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

ColorPicker 是 AtomUI 桌面控件体系中的颜色选择控件，用于选择纯色、透明度、预设色和渐变色。

ColorPicker 不负责通用绘图编辑器或业务调色板持久化协议。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls.ColorPicker`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls.ColorPicker` |
| .NET 命名空间 | `AtomUI.Desktop.Controls.ColorPicker` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/ColorPicker` |
| 状态 | Stable |

## 何时使用

ColorPicker 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | ColorPicker 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | ColorPicker 是 AtomUI 桌面控件体系中的颜色选择控件，用于选择纯色、透明度、预设色和渐变色。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `ColorValue`、`ColorValueBrush`、`DefaultValue`、`EmptyColorText`、`GradientValue`、`IsTextVisible`、`MaxValue`、`MinValue` 等 10 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close、collection/filter、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | ColorPicker Token + ControlTheme。 |

## 公共 API

ColorPicker 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ColorValue`、`ColorValueBrush`、`DefaultValue`、`EmptyColorText`、`GradientValue`、`IsTextVisible`、`MaxValue`、`MinValue`、`Value`、`ValueSyncStrategy` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `ColorModel`、`IsEmptyColorMode`、`IsPaletteGroupEnabled` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsActivated`、`IsAlphaEnabled`、`IsAlphaVisible`、`IsArrowVisible`、`IsClearEnabled`、`IsColorSpectrumSliderVisible`、`IsFormatEnabled`、`IsMotionEnabled`、`IsPerceptive`、`IsPointAtCenter` 等 15 项 | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `Color`、`ColorComponent`、`ColorSpectrumComponents`、`HsvColor`、`MarginToAnchor`、`Placement`、`PlacementAnchor`、`PlacementGravity`、`Shape`、`Size` 等 12 项 | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 动效与异步 | `MouseEnterDelay`、`MouseLeaveDelay` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `ActivatedThumb`、`Components`、`DecreaseButton`、`Format`、`IncreaseButton`、`MaxHue`、`MaxSaturation`、`Maximum`、`MinHue`、`MinSaturation` 等 14 项 | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

`ColorPicker.Value` 和 `GradientColorPicker.Value` 是用户拥有的当前值：纯色选择器使用 `Color?`，渐变选择器使用 `LinearGradientBrush?`。两个 `Value` CLR wrapper 均可公开设置，默认 `TwoWay` 绑定，并接入 Avalonia `DataValidationErrors`。清除按钮、Form clear 和外部绑定写入 `null` 都必须让 `Value` 变为 `null`，不能只清空触发器文字或色块视觉。

稳定事件包括 `ClearRequest`、`GradientActiveStopChanged`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`AbstractColorPicker`、`AbstractColorPickerSliderTrack`、`AbstractColorPickerView`、`AbstractColorSlider`、`AtomUIColorPickerThemesProvider`、`ColorBlock`、`ColorChangedEventArgs`、`ColorPicker`、`ColorPickerCollapse`、`ColorPickerInput`、`ColorPickerLangResourceExtension`、`ColorPickerPalette`、`ColorPickerPaletteColorSelectedEventArgs`、`ColorPickerPaletteGroup` 等 36 项。
- 枚举：`ColorFormat`、`ColorPickerLangResourceKind`、`ColorPickerTokenKind`、`ColorPickerValueSyncMode`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_AlphaInput` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_BValueInput` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_CheckedMark` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ClearColor` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ColorFormatComboBox` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ColorIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_ColorPreview` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ColorPreviewFrame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_ColorText` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_ColorTextPanel` | `?` | 承载集合项、布局面板或虚拟化内容。 |
| `PART_DecreaseButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_EmptyColorFrame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_Frame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_GValueInput` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_GradientColorSlider` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_HValueInput` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_HexValueInput` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_HsvValuePanel` | `?` | 承载集合项、布局面板或虚拟化内容。 |
| `PART_IncreaseButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_InnerEllipse` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_InputTarget` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_LayoutRoot` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_PaletteGroup` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_Popup` | `?` | 承载弹层宿主、打开关闭或候选内容。 |
| 其他 part | 10 项 | 参见源码和主题文件；维护时按同一生命周期规则检查。 |

控件专属或内部伪类包括 `ColorPickerPseudoClass.FlyoutOpen`、`ColorSliderPseudoClass.DarkSelector`、`ColorSliderPseudoClass.LightSelector`、`ColorSpectrumPseudoClass.DarkSelector`、`ColorSpectrumPseudoClass.LargeSelector`、`ColorSpectrumPseudoClass.LightSelector`、`ColorSpectrumPseudoClass.Pressed`、`DarkSelector=:dark-selector`、`FlyoutOpen=:flyout-open`、`LargeSelector=:large-selector`、`LightSelector=:light-selector`、`Pressed=:pressed`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

ColorPicker 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `ClearRequest`、`GradientActiveStopChanged`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。
- 类型：`AbstractColorPicker`、`AbstractColorPickerSliderTrack`、`AbstractColorPickerView`、`AbstractColorSlider`、`AtomUIColorPickerThemesProvider`、`ColorBlock`、`ColorChangedEventArgs`、`ColorPicker`、`ColorPickerCollapse`、`ColorPickerInput`、`ColorPickerLangResourceExtension`、`ColorPickerPalette`、`ColorPickerPaletteColorSelectedEventArgs`、`ColorPickerPaletteGroup` 等 36 项。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/ColorPicker/Views/ColorPickerShowCase.axaml:35`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:ColorPicker DefaultValue="#1677ff"/>
```

### Value 绑定

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/ColorPicker/Views/ColorPickerShowCase.axaml:47`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical" Spacing="12">
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:ColorPicker Value="{Binding BoundColorValue}"
                          IsTextVisible="True"
                          IsClearEnabled="True" />
        <atom:GradientColorPicker Value="{Binding BoundGradientValue}"
                                  IsTextVisible="True"
                                  IsClearEnabled="True" />
    </StackPanel>
    <StackPanel Orientation="Horizontal" Spacing="8">
        <atom:TextBlock VerticalAlignment="Center"
                        Text="绑定颜色：" />
        <atom:TextBlock VerticalAlignment="Center"
                        Text="{Binding BoundColorValueText}" />
    </StackPanel>
    <StackPanel Orientation="Horizontal" Spacing="8">
        <atom:TextBlock VerticalAlignment="Center"
                        Text="绑定渐变：" />
        <atom:TextBlock VerticalAlignment="Center"
                        Text="{Binding BoundGradientValueText}" />
    </StackPanel>
    <WrapPanel ItemSpacing="10" LineSpacing="8">
        <atom:Button SizeType="Small"
                     Command="{Binding SetBoundColorValueCommand}"
                     Content="设置颜色" />
        <atom:Button SizeType="Small"
                     Command="{Binding SetBoundGradientValueCommand}"
                     Content="设置渐变" />
        <atom:Button SizeType="Small"
                     Command="{Binding ClearBoundColorValueCommand}"
                     Content="清空颜色" />
        <atom:Button SizeType="Small"
                     Command="{Binding ClearBoundGradientValueCommand}"
                     Content="清空渐变" />
    </WrapPanel>
</StackPanel>
```

### 触发器尺寸

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/ColorPicker/Views/ColorPickerShowCase.axaml:93`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <StackPanel Orientation="Horizontal" Spacing="12" VerticalAlignment="Center">
        <atom:TextBlock Width="64"
                        VerticalAlignment="Center"
                        Foreground="{atom:SharedTokenResource ColorTextSecondary}"
                        Text="小号" />
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:ColorPicker DefaultValue="#1677ff" SizeType="Small"/>
            <atom:ColorPicker DefaultValue="#1677ff" SizeType="Small" IsTextVisible="True"/>
        </StackPanel>
    </StackPanel>

    <StackPanel Orientation="Horizontal" Spacing="12" VerticalAlignment="Center">
        <atom:TextBlock Width="64"
                        VerticalAlignment="Center"
                        Foreground="{atom:SharedTokenResource ColorTextSecondary}"
                        Text="中号" />
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:ColorPicker DefaultValue="#1677ff" SizeType="Middle"/>
            <atom:ColorPicker DefaultValue="#1677ff" SizeType="Middle" IsTextVisible="True"/>
        </StackPanel>
    </StackPanel>

    <StackPanel Orientation="Horizontal" Spacing="12" VerticalAlignment="Center">
        <atom:TextBlock Width="64"
                        VerticalAlignment="Center"
                        Foreground="{atom:SharedTokenResource ColorTextSecondary}"
                        Text="大号" />
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:ColorPicker DefaultValue="#1677ff" SizeType="Large"/>
            <atom:ColorPicker DefaultValue="#1677ff" SizeType="Large" IsTextVisible="True"/>
        </StackPanel>
    </StackPanel>

    <StackPanel Orientation="Horizontal" Spacing="12" VerticalAlignment="Center">
        <atom:TextBlock Width="64"
                        VerticalAlignment="Center"
                        Foreground="{atom:SharedTokenResource ColorTextSecondary}"
                        Text="Custom" />
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:ColorPicker DefaultValue="#1677ff"
                              SizeType="Custom"
                              Width="48"
                              Height="48" />
            <atom:ColorPicker DefaultValue="#1677ff"
                              SizeType="Custom"
                              IsTextVisible="True"
                              Height="48"
                              FontSize="18" />
        </StackPanel>
    </StackPanel>
</StackPanel>
```

### 线性渐变

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/ColorPicker/Views/ColorPickerShowCase.axaml:155`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:GradientColorPicker IsTextVisible="True">
        <atom:GradientColorPicker.DefaultValue>
            <LinearGradientBrush>
                <GradientStop Color="#108ee9" Offset="0"/>
                <GradientStop Color="#87d068" Offset="1"/>
            </LinearGradientBrush>
        </atom:GradientColorPicker.DefaultValue>
    </atom:GradientColorPicker>
</StackPanel>
```

## 状态模型

ColorPicker 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- open/close、collection/filter、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `Value`、trigger 色块、trigger 文本、picker presenter 和 Form 值必须由同一份 current value 派生；清空状态以 `Value=null` 为源头。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

ColorPicker 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AtomUIColorPickerThemesProvider.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `AbstractColorPickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorBlockTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorPickerPaletteGroupTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorPickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `AbstractColorPickerSliderTrackTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorPickerSliderTrackTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorSliderTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorSliderThumbTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `GradientColorPickerTrackTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `GradientColorSliderTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `AbstractColorPickerViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorPickerInput.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorPickerViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorSpectrumTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `GradientColorPickerViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `GradientColorPickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `PaletteColorItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |

ColorPicker 使用 `ColorPickerToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 open/close、collection/filter、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

ColorPicker Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ColorPickerToken`，scope id 为 `ColorPicker`，源码位于 `src/AtomUI.Desktop.Controls.ColorPicker/ColorPickerToken.cs`。

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

- `src/AtomUI.Desktop.Controls.ColorPicker`：20 个文件，代表文件 `AbstractColorPicker.cs`、`AtomUIColorPickerThemesProvider.axaml`、`AtomUIColorPickerThemesProvider.cs`、`ColorBlock.cs`、`ColorChangedEventArgs.cs` 等。
- `src/AtomUI.Desktop.Controls.ColorPicker/ColorSlider`：10 个文件，代表文件 `AbstractColorPickerSliderTrack.cs`、`AbstractColorSlider.cs`、`ColorPickerSliderTrack.cs`、`ColorSlider.cs`、`ColorSliderPseudoClass.cs` 等。
- `src/AtomUI.Desktop.Controls.ColorPicker/ColorView`：6 个文件，代表文件 `AbstractColorPickerView.cs`、`ColorPickerInput.cs`、`ColorPickerView.cs`、`ColorSpectrum.cs`、`ColorSpectrumPseudoClass.cs` 等。
- `src/AtomUI.Desktop.Controls.ColorPicker/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.LanguageGenerator`：2 个文件，代表文件 `LanguageProviderPool.g.cs`、`LanguageResourceConst.g.cs`。
- `src/AtomUI.Desktop.Controls.ColorPicker/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.ResourceHost.ScopedResourceHostGenerator`：1 个文件，代表文件 `GenerateScopedResourceHostAttribute.g.cs`。
- `src/AtomUI.Desktop.Controls.ColorPicker/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator`：2 个文件，代表文件 `ControlTokenTypePool.g.cs`、`TokenResourceConst.g.cs`。
- `src/AtomUI.Desktop.Controls.ColorPicker/Localization`：3 个文件，代表文件 `en_US.cs`、`zh_CN.cs`、`zh_TW.cs`。
- `src/AtomUI.Desktop.Controls.ColorPicker/Properties`：1 个文件，代表文件 `AssemblyInfo.cs`。
- `src/AtomUI.Desktop.Controls.ColorPicker/Themes`：21 个文件，代表文件 `AbstractColorPickerTheme.axaml`、`AbstractColorPickerTheme.cs`、`ColorBlockTheme.axaml`、`ColorPickerPaletteGroupTheme.axaml`、`ColorPickerTheme.axaml` 等。
- `src/AtomUI.Desktop.Controls.ColorPicker/Utils`：5 个文件，代表文件 `ColorPickerHelpers.cs`、`Hsv.cs`、`IncrementAmount.cs`、`Rgb.cs`、`TransparentBgBrushUtils.cs`。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/color-picker/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/color-picker/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/color-picker/token.md`
- 变更记录：`docs/controls/desktop/data-entry/color-picker/changelog.md`
- 语义结构：`./semantic-cn.md`
