# Button 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.Button` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [Button 桌面版实现原理](implementation.md)，Button Token 的专项设计见 [Button Token 设计](token.md)，设计和契约变化记录见 [Button Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/General/Button` |
| 控件状态 | Stable |

Button 是 AtomUI 桌面控件体系中的基础动作触发控件，用于承载用户可执行的明确操作。Button 负责把动作语义稳定映射为公共 API、交互状态、主题 Token、模板结构和反馈效果。

Button 不承担复杂内容布局、导航结构管理或业务状态表达职责。复杂按钮形态应通过 Button 家族控件扩展，而不是扩大 Button 本体的职责边界。

Button 家族包括 `DropdownButton`、`SplitButton`、`IconButton` 和 `HyperLinkButton`。这些控件可以拥有不同模板结构，但必须共享 Button 的动作语义、状态解释、尺寸体系和主题资源。

典型使用场景：

- 提交、保存、删除、确认、取消等明确动作。
- 表达主动作、普通动作、弱强调动作、链接式动作和危险动作。
- 在工具栏、表单、对话框、卡片和列表项中提供可点击操作入口。
- 使用图标、加载态、禁用态、尺寸和形状增强动作可识别性。

## 2. 设计语言

Button 的设计语言由三个正交维度组成。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 动作层级 | 操作在当前界面中的优先级。 | 主动作、普通动作、弱强调动作、链接式动作。 |
| 语义颜色 | 操作的语义属性。 | 默认、主要、危险、业务预设色。 |
| 视觉强度 | 同一语义在视觉上的强调程度。 | 实心、描边、虚线、填充、文本、链接。 |

Button 的当前设计模型以 `Color + Variant` 表达语义颜色和视觉强度。`ButtonType` 是兼容合成模型，同时承载动作层级、语义颜色和视觉强度，应保留为兼容入口。

颜色不应被视为装饰属性。`Danger` 表达破坏性或高风险动作，预设色表达业务分类或语义扩展。视觉强度不应改变语义颜色，只改变同一语义的呈现强度。

## 3. API 与契约模型

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

## 4. 行为与状态模型

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

## 5. 视觉与主题模型

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

## 6. 控件家族或集成关系

Button 家族控件应共享一致的动作语义和状态解释。

- `DropdownButton` 继承 Button 的基础动作状态，并扩展下拉触发能力。
- `SplitButton` 保持主动作和附加动作的语义区分，但两部分的颜色、尺寸和禁用状态应与 Button 体系一致。
- `IconButton` 保留 Button 的动作语义，只改变内容呈现密度。
- `HyperLinkButton` 保持链接式动作语义，并复用 Button 尺寸、字体和 icon 相关 Token。
- `BrowserButtonThemes` 可以存在平台视觉差异，但同一 API 的语义解释必须与桌面主题一致。

Button 与 CompactSpace、FormItem、Wave、Browser 主题协同。`Color + Variant` 等语义能力不应只覆盖默认 Button，Button 家族控件必须明确继承、覆盖或声明不支持该语义。

## 7. 兼容性不变量

优化或扩展 Button 时必须保持以下不变量：

- 现有五种 `ButtonType` 的默认、hover、pressed、disabled、danger、ghost 渲染不变。
- `IsDanger` 与各 `ButtonType` 的组合行为不变。
- `IsGhost` 与现有按钮类型的组合行为不变。
- loading icon、opacity、原 icon 隐藏逻辑不变。
- icon-only 判断与布局不变。
- `IconPlacement` 默认值必须保持 `Start`；`IconPlacement=End` 只允许改变用户 icon 的内容侧位置和间距方向。
- `Shape=Circle`、`Shape=Round` 的尺寸和圆角计算不变。
- `SizeType=Large/Middle/Small` 的预设尺寸、字体、内边距、圆角和 icon 尺寸不变。
- `SizeType=Custom` 未显式设置尺寸相关属性时必须按 `Middle` 默认值渲染；用户在 Button 上设置的本地 `Height`、`Padding`、`FontSize`、`CornerRadius` 等现有属性必须覆盖 Custom 默认值。
- CompactSpace 下的有效圆角、有效边框和 z-index 行为不变。
- wave 播放条件和危险态 wave brush 不变。
- `CustomBackground` 不改变 `WaveSpiritDecorator` 的 wave brush，wave 颜色仍由 `EffectiveColor + EffectiveVariant` 推导。
- Browser 主题与桌面主题在同一 API 下语义一致。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

## 8. 专项模型

### 8.1 Color / Variant 模型

多彩按钮属于 Button 当前设计模型的一部分，不应通过扩展 `ButtonType` 枚举表达。

- `ButtonColor` 表达颜色语义。
- `ButtonVariant` 表达视觉强度。
- `ButtonType` 保留为兼容语法糖。
- `IsDanger` 保留为危险语义的旧入口。
- 预设色来源于 AtomUI palette / token 系统，不在 Button 主题中维护私有色表。

### 8.2 CustomBackground 视觉覆层模型

`CustomBackground` 是 Button 的受控视觉覆层模型，用于表达 normal 状态下的自定义按钮表面。覆层只在 `EffectiveVariant=Solid`、非危险态、非禁用态下显示；hover 与 pressed 状态隐藏覆层，露出标准 Button 状态背景。

自定义背景覆层是主题内部实现细节，不形成用户可依赖的 `/template/` 样式入口。该模型用于表达 normal 状态的受控自定义表面，交互状态回落到 Button 原有语义状态。

### 8.3 Custom 尺寸模型

Button 的尺寸模型由预设档和实例定制组成。预设档 `Large`、`Middle`、`Small` 表达 Button 的三档尺寸；AtomUI 额外通过 `CustomizableSizeType.Custom` 提供实例级自定义入口。

`SizeType=Custom` 的设计契约：

- 未设置本地尺寸属性时，`Custom` 使用 `Middle` 的默认视觉指标，包括高度、字体、内边距、圆角、普通 icon 尺寸和 icon-only 尺寸。
- 用户通过 Button 已有属性定制尺寸，例如 `Height`、`MinHeight`、`Width`、`MinWidth`、`Padding`、`FontSize` 和 `CornerRadius`。
- 主题只能以 Style 默认值或可被 Button 本地属性覆盖的模板绑定提供 Custom 默认值，不得用更高优先级写入覆盖用户本地值。
- Button 不提供 `CustomHeight`、`CustomPadding`、`CustomFontSize`、`CustomIconSize`、`CustomOnlyIconSize` 或 `ButtonSizeMetrics`。
- Custom 模式下 icon 与 loading icon 默认沿用 `Middle` Token；需要特殊 icon 尺寸时，应通过现有 icon 或样式能力定制，不扩展 Button 公共 API。

### 8.4 Icon 位置模型

Button 的 `Icon` 是单一用户图标入口，`IconPlacement` 只描述这个图标相对内容的位置。

- `Start` 表示图标位于内容起始侧，是默认值和兼容行为。
- `End` 表示图标位于内容结束侧，适用于下一步、跳转、查看更多等需要尾随图标的动作。
- `IconPlacement` 不创建第二个图标 slot，也不改变 `Icon` 类型、图标创建方式或 template part 名称。
- 图标和内容之间的间距继续由 Button token 管理；结束侧图标使用起始侧间距的镜像方向。
- icon-only 按钮没有文本内容侧差异，`IconPlacement` 不应改变 icon-only 的尺寸、padding 或居中行为。
- loading icon 仍由 `IsLoading` 状态控制，不作为 `IconPlacement` 的目标；loading 状态下原用户 icon 隐藏逻辑保持不变。
- `DropdownButton`、`SplitButton`、`IconButton`、`HyperLinkButton` 是否暴露同名能力应按各自模板职责单独评估，不由 Button 本体隐式要求。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [Button 桌面版实现原理](implementation.md)
- [Button Token 设计](token.md)
- [Button Changelog](changelog.md)

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + Gallery API / Token / ShowCase | 生成 `controls/button/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + `ButtonTheme.axaml` | 生成 `controls/button/semantic-cn.md` |
| API 表 | `ButtonViewModel.EnsureApiRows()` + `Button.cs` public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | `ButtonDesignTokenDataGrid` + `ButtonToken.cs` + `token.md` | `token.md` 解释 Token 语义边界 |
| 示例 | `ButtonShowCase.axaml` + source snippet catalog | 覆盖类型、形状、尺寸、图标、加载、危险、幽灵、禁用、渐变、颜色与变体 |
| 源码索引 | `implementation.md` | 用于定位 Button 源码、主题、伪类和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | `git diff --check`，确认未改实现文件。 |
| C# 状态模型改动 | Button 行为测试，覆盖旧 API 兼容映射。 |
| AXAML 主题改动 | Button 视觉状态检查，必要时补 Gallery 截图。 |
| Token / Palette 改动 | Light / Dark 主题检查，确认 Browser 主题一致性。 |
| Button 家族影响 | 覆盖 `DropdownButton`、`SplitButton`、`IconButton`、`HyperLinkButton` 关联场景。 |
| Public API 改动 | 需要授权，并补充 API 兼容测试与文档。 |
| LLMS 导出改动 | 重新生成 `controls/button/index-cn.md`、`controls/button/semantic-cn.md`、`llms-full-cn.txt` 和 `llms-semantic-cn.md`，确认来源表、API、Token、示例和 semantic parts 一致。 |
