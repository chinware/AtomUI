# AtomUI Semantic Part 系统设计

Semantic Part 是 AtomUI Control 对稳定视觉区域提供的公共定制契约。它把控件的用户语义与具体
`ControlTemplate` 节点解耦，使应用能够通过 Avalonia Selector 定制局部视觉，而不依赖 `PART_*`、节点名称、
internal 类型或偶然的视觉树结构。

本文档定义 Semantic Part 的统一模型、Selector 契约、主题边界、构建期描述、兼容性和验证要求。ControlTheme
资产、Token schema 和主题运行时的完整架构见 [主题系统架构](runtime.md)；Core 注册和冻结实现见
[AtomUI.Core 模块](../../../modules/core/overview.md)；生成器实现见
[Semantic Part Generator](../../../modules/generator/semantic-part-generator.md)。

## 1. 设计定位

Semantic Part 解决以下职责：

- 为一个 Control 的稳定视觉区域分配与实现结构无关的语义名称。
- 为公开区域提供稳定的 `.semantic-*` Selector 标记。
- 允许 Application、局部容器和单个 Control 实例使用 Avalonia 原生 `Styles` 定制这些区域。
- 为文档、Gallery、主题校验和 NativeAOT 注册提供静态 descriptor。
- 允许真实 public 子 Control 在必要时额外提供强类型 Semantic Part Theme。

Semantic Part 不负责：

- 替代 Control 的 StyledProperty、伪类、事件或行为 API。
- 把所有模板节点提升为公共契约。
- 创建独立的运行时样式字典、样式合并器或 VisualTree 查找机制。
- 为 internal 节点创建 Token identity。
- 保证应用自定义 ControlTheme 自动保留 AtomUI 内置模板的 Semantic Part。

## 2. 设计原则

1. **Selector-first**：除 `root` 外，公开 Part 默认通过 `.semantic-*` 和 Avalonia Selector 定制。
2. **语义稳定**：Part 描述职责，不描述当前模板节点名称或布局容器层级。
3. **最小公开面**：只有可以跨版本承诺的区域进入公开 descriptor；临时节点只属于 Composition Model。
4. **Avalonia 原生**：样式作用域、优先级、状态选择和模板边界完全使用 Avalonia 12 的原生机制。
5. **匹配与类型分离**：`.semantic-*` 是 Part 的运行时匹配身份；`ContractType` 只提供 Setter 类型上下文和模板校验。
6. **Theme 按需**：强类型 `ControlTheme?` 只用于允许完整替换的真实 public 子 Control。
7. **构建期发现**：Part、模板 marker 和 typed theme 关系由生成器静态验证；运行时不扫描程序集或 AXAML。
8. **跨宿主一致**：默认模板、浏览器模板、Popup、Overlay 和虚拟化容器必须维持同一个公共语义契约。
9. **事实约束设计**：依赖 Avalonia Selector、编译器或样式激活器行为的规则，必须由当前解析版本源码或可复现
   编译与运行时测试确认，并在 Avalonia 升级后重新验证。

## 3. 术语与模型

### 3.1 Semantic Part

Semantic Part 是 Control owner 下的一个稳定视觉职责。公开名称使用 camelCase，例如：

```text
root
icon
content
clearIcon
popup
popup.option
```

名称可以形成层级 path。Path 用于描述语义关系，不要求内部视觉树采用相同层级。

### 3.2 Selector class

除 `root` 外，每个公开 Part 都具有一个保留 class：

```text
icon       -> semantic-icon
content    -> semantic-content
clearIcon  -> semantic-clear-icon
popup      -> semantic-popup
option     -> semantic-option
```

AXAML Selector 使用 class 语法：

```text
.semantic-icon
.semantic-content
.semantic-popup
```

`.semantic-*` 是 AtomUI 公共主题契约命名空间。AtomUI 不为同一 Part 提供短名称、旧名称或第二套 alias。

### 3.3 ContractType

Avalonia Setter 依赖目标 AvaloniaProperty 的 owner 类型。每个 Part 因此必须声明最低稳定 `ContractType`：

- 通用视觉区域可以使用 `Control`。
- 内容展示区域可以承诺 `ContentPresenter`。
- 允许完整替换的 public 子 Control 使用自身 public 类型。
- Popup frame 如果公开 Border 能力，可以承诺 `Border`。

`ContractType` 具有三个职责：

1. 作为用户 Semantic Style 的 `x:SetterTargetType`，供 AXAML 编译器解析 Setter property。
2. 作为生成器验证 marker 节点类型兼容性的下界。
3. 作为文档、Gallery 和工具描述 Part 可稳定设置属性范围的类型元数据。

`ContractType` **不参与 Part selector 匹配**，不得作为 `.semantic-*` 前缀。完整用法为：

```xml
<Style Selector="atom|Button /template/ .semantic-icon"
       x:SetterTargetType="Control">
    <Setter Property="Opacity" Value="0.8" />
</Style>
```

Avalonia 的裸类型 selector 是精确 `StyleKey` 匹配，因此 `Control.semantic-icon` 不表示“任意 Control 派生类型”，
无法稳定命中 `Icon`、`IconPresenter` 等不同实现。`:is(Control).semantic-icon` 可以匹配派生类型，但会把 Setter
类型上下文编码进 Part 身份，也不能消除 class selector 的动态激活成本，因此不作为 AtomUI 公共 Semantic Part 写法。

class-only selector 在 `/template/` 之后没有可供 Setter 推断的目标类型；包含 Setter 的 AXAML Style 必须显式声明
`x:SetterTargetType`。该指令只参与 AXAML 编译，不改变运行时 Selector。

把 `ContractType` 收窄到更具体类型，或让实现节点不再兼容原类型，属于破坏性变更。

### 3.4 Cardinality

Part 使用以下数量契约：

| 值 | 语义 |
| --- | --- |
| `Single` | 每个已实例化模板存在一个目标节点。 |
| `Optional` | 当前状态或模板变体可以不存在目标节点。 |
| `Multiple` | 同一职责允许多个目标节点或多个替代实现。 |

多个实现节点可以共享同一个 Part。例如普通图标和 loading 图标可以同时带有 `.semantic-icon`，由可见性状态决定
当前展示节点。Semantic Style 必须稳定作用于所有替代实现。

### 3.5 Customization

| 值 | 语义 |
| --- | --- |
| `Root` | Part 是 owner Control 本身，通过 Control API、Classes、Styles 和 Theme 定制。 |
| `Selector` | Part 通过 `.semantic-*` Selector 定制。 |
| `SelectorAndTheme` | Part 同时支持 Selector 和强类型 `ControlTheme?` 完整替换。 |

公开 descriptor 不包含 `InspectOnly`。不允许用户依赖的节点不属于 Semantic Part，应记录在控件的 Composition
Model 或 Customization Boundaries 中。

## 4. 公共 descriptor

每个可定制 Control 具有一个静态 `ControlSemanticDescriptor`。它至少包含：

```text
Control identity
Part name
Part path
Selector class
ContractType identity
Cardinality
Customization
Optional Theme property
CrossVisualRoot
Since
RuntimeCreated
```

`root` 由生成器隐式加入 descriptor，不要求模板增加 `.semantic-root`。

Descriptor 服务于：

- 构建期模板契约校验。
- 文档与 LLMS Semantic Parts 输出校验。
- Gallery Semantic Preview；Preview 只读取 descriptor，不把实例、调试状态或高亮信息写回 registry。
- 控件包静态注册和第三方工具。
- 兼容性测试。

Descriptor 不参与 Avalonia Selector 的运行时匹配，也不保存 `Style`、Setter、Control 实例或 VisualTree 节点。

应用和开发工具通过 `IThemeManager.SemanticParts` 获取冻结的 `SemanticPartRegistry`。Registry 支持按 owner CLR type
或 `ControlTokenIdentity` 查询，且不提供运行时追加、删除或覆盖 descriptor 的 API。

## 5. Selector 契约

### 5.1 推荐结构

同一模板内的 Part 使用 owner、一个 `/template/` 边界和稳定 selector class；Setter 类型由 `ContractType` 单独提供：

```xml
<Style Selector="atom|Button /template/ .semantic-icon"
       x:SetterTargetType="Control" />
<Style Selector="atom|Button /template/ .semantic-content"
       x:SetterTargetType="ContentPresenter" />
```

Selector 不连续穿透多个子 ControlTemplate。父 Control 只能承诺自己的 Semantic Part，不能借此公开子 Control 的
internal `PART_*`。

层级 Part 使用稳定 semantic ancestor 收窄范围：

```xml
<Style Selector="atom|Select /template/ .semantic-popup .semantic-option"
       x:SetterTargetType="Control" />
```

### 5.2 全局、局部与实例作用域

全局规则放入 Application 或 AtomUI 主题 Styles：

```xml
<Application.Styles>
    <Style Selector="atom|Button /template/ .semantic-icon"
           x:SetterTargetType="Control">
        <Setter Property="Margin" Value="0,0,6,0" />
    </Style>
</Application.Styles>
```

局部规则放入 Window、UserControl 或其他样式宿主：

```xml
<UserControl.Styles>
    <Style Selector="atom|Button /template/ .semantic-content"
           x:SetterTargetType="ContentPresenter">
        <Setter Property="Opacity" Value="0.9" />
    </Style>
</UserControl.Styles>
```

单实例规则放入 owner 的 `Styles`：

```xml
<atom:Button>
    <atom:Button.Styles>
        <Style Selector=".semantic-icon"
               x:SetterTargetType="Control">
            <Setter Property="Width" Value="18" />
            <Setter Property="Height" Value="18" />
        </Style>
    </atom:Button.Styles>
</atom:Button>
```

多个实例共享规则时，在 root 增加业务 class，并通过 owner class 收窄语义 Selector：

```xml
<Style Selector="atom|Button.compact /template/ .semantic-icon"
       x:SetterTargetType="Control" />
```

Application 或较大 StyleHost 中的规则必须包含 owner scope。裸全局 `.semantic-icon` 会扩大样式候选范围，不属于推荐
用法；只有放在单个 owner 的 `Styles` 中时才省略 owner 与 `/template/`。

### 5.3 状态映射

Control 的运行时状态继续由 StyledProperty、伪类和有效状态属性拥有。Semantic Style 使用 Avalonia Selector 消费
这些状态：

```xml
<Style Selector="atom|Button:pointerover /template/ .semantic-icon"
       x:SetterTargetType="Control">
    <Setter Property="Opacity" Value="1" />
</Style>
```

Semantic Part 不提供状态 callback、动态样式 delegate 或按 Part 名称索引的状态字典。数据驱动值使用正常 Binding，
离散视觉状态使用伪类或属性 Selector。

## 6. 与 Token、Property 和 ControlTheme 的边界

| 能力 | Owner |
| --- | --- |
| 行为、交互状态和业务语义 | Control API、StyledProperty、事件和伪类 |
| Control 默认设计值 | Global Token 和 Control Own Token |
| 稳定视觉区域的局部覆盖 | Semantic Selector |
| public 子 Control 的完整视觉替换 | 可选 Semantic Part Theme |
| 整个 Control 的结构替换 | owner `ControlTheme` |

Token 不使用 `semantic-icon-width`、`popup-frame-2-padding` 等模板节点命名。一个值只有构成 Control 稳定设计语言时
才进入 Token；单个应用的局部视觉差异使用 Semantic Style。

Semantic Style 不拥有行为。`IsOpen`、selection、validation、loading 流程和键盘交互不能依赖用户是否为某个 Part
设置了 Setter。

## 7. 样式优先级

AtomUI 不创建 Semantic Style merge engine。所有 Setter 使用 Avalonia 12 原生 BindingPriority、StyleHost、Selector
激活和声明顺序。

必须明确以下边界：

- 目标节点的 LocalValue 保持 Avalonia 原生优先级。
- class、伪类和属性 Selector 可以覆盖模板投影到 Part 的视觉值。
- Semantic Style 可以有意覆盖 `TemplateBinding` 提供的默认视觉值。
- 相同优先级下的冲突由 Avalonia 样式宿主顺序和声明顺序解决。
- Token 和默认 ControlTheme 提供基线，不压制用户的合法 Semantic Style。
- AtomUI 文档不建立一套与 Avalonia 不一致的“全局、实例、Token”伪优先级表。

### 7.1 布局型 Setter 的协调边界

Semantic Style 的目标属性已经赢得优先级，不表示最终布局一定采用该值所暗示的尺寸。Avalonia 分别解析每个节点、每个
属性的有效值，然后由父子节点共同完成 Measure/Arrange。典型情况是 Semantic Style 修改子 Part 的 `Padding`、`Width`
或 `Height`，而 owner 根节点仍持有独立的 `Height`、`MinHeight`、`MaxHeight` 或自定义 `MeasureOverride`；两者不是同一
属性上的优先级冲突，子 Part 的 Setter 可以已经生效，同时仍被 owner 的最终布局边界裁剪或压缩。

因此开放布局型 Semantic Part 时，Control 作者必须同时审计：

- owner 的 `Height`、`MinHeight`、`MaxHeight`、`Padding` 和尺寸档主题映射。
- Part 自身以及从 Part 到 owner 根之间所有布局节点的 `Width`、`Height`、Min/Max、Margin、Padding 和裁剪。
- owner 是否在 `MeasureOverride` / `ArrangeOverride` 中根据内容尺寸计算 Circle、Round、正方形、纵横比或其他几何。
- Semantic Part 的 `Cardinality`；同一个 Setter 是否需要同时作用于 loading、普通、空态等替代实现。
- Desktop、Browser、状态模板和派生主题是否具有相同的尺寸协调方式。

对于允许内容驱动扩展的预设尺寸，优先使用 `MinHeight` 建立尺寸基线，并让自然测量决定是否增长。固定 `Height` 只用于
高度本身就是不可扩展公共契约的 Control；不能只为掩盖内部 Padding 或模板测量不一致而封死高度。移除固定高度后，
如果 owner 的几何依赖最终高度，几何计算必须使用已经合并 owner 布局约束的测量结果，不能直接使用未应用 Min/Max 的
内容期望尺寸。

布局异常的排查顺序固定为：

1. 读取目标 Part 的有效属性值，先证明 Semantic Setter 是否命中。
2. 区分同一属性的样式优先级冲突，与父子不同属性之间的布局约束冲突。
3. 沿 Part 到 owner 根检查固定尺寸、Min/Max、Padding、Margin、裁剪和模板绑定。
4. 检查 owner 的 Measure/Arrange 是否在应用布局约束前派生几何。
5. 覆盖所有尺寸档、shape、icon-only、loading、Desktop 和 Browser 变体，不能只验证默认矩形样例。

## 8. Template 集成

### 8.1 静态模板节点

AtomUI 内置模板使用 Avalonia class property 语法静态设置 semantic class：

```xml
<ContentPresenter Classes.semantic-content="True" />
```

该语法是 ControlTheme 作者的 marker 声明形式，不是 Application 用户的定制 API。Application 用户继续使用
`.semantic-content` Selector，不需要也不应在 Control 实例上复制模板 marker。

Avalonia 12 将静态布尔值编译为模板初始化阶段的一次 `Classes.Set("semantic-content", true)` 调用。它不创建
Binding、selector activator 或持久订阅。`Classes="semantic-content"` 仍是 Generator 支持的兼容输入，但不作为
AtomUI 自有模板的编写规范。`Classes.semantic-*` 的值必须是静态 `true`；`False`、Binding 或其他动态值不能承担公共
Part 契约，并由 `ATOMUIGEN029` 拒绝。

已有 `PART_*` 名称可以继续服务于 Control 代码查找；Semantic class 与 Template Part 名称承担不同职责。

### 8.2 模板变体

同一 Control 的所有内置模板变体必须提供相同公共 descriptor，包括：

- 状态或 variant 分支下的多个 ControlTemplate。
- Desktop 与 Browser 主题。
- 派生 Control 使用的主题变体。
- Light、Dark 或 Compact 不同资源路径。

某个模板变体无法提供 Part 时，必须把 Part 声明为 `Optional`，或者重新设计公共语义，不能静默漏标。

### 8.3 动态创建节点

C# 创建的公开 Part 必须：

1. 使用生成的 semantic class 常量添加 marker。
2. 建立正确的 logical parent、inheritance parent 或 templated parent。
3. 在回收、re-template 和释放路径中保持 marker 与 owner 一致。
4. 通过控件测试验证 Selector 命中，不依赖源码文本扫描证明运行时契约。

### 8.4 自定义 ControlTheme

应用替换 owner `ControlTheme` 后，由应用决定是否继续实现 AtomUI Semantic Part 契约。缺失 marker 不影响 Control
基本行为，但对应 Semantic Selector 不再命中。

AtomUI 内置 Theme 必须完整实现 descriptor；生成器不扫描应用程序集中的任意第三方 AXAML 来修复自定义模板。

## 9. Popup、Overlay 与容器

### 9.1 模板内 Popup

Avalonia 12 的 Popup 在打开时保留 Popup、PopupRoot 或 OverlayPopupHost 的样式宿主关系，并为模板 Popup 内容传播
`TemplatedParent`。因此 Select、ComboBox、AutoComplete 等模板内 Popup 使用 Selector 作为默认契约：

```xml
<Style Selector="atom|Select /template/ .semantic-popup"
       x:SetterTargetType="Border" />
```

`CrossVisualRoot=true` 用于描述和测试，不自动要求 `PopupPresenterTheme`、`PopupHostTheme` 或其他新属性。

### 9.2 独立宿主

ContextMenu、Flyout、Dialog、Message、Notification 等由服务或独立 host 创建的内容，必须分别确认：

- semantic root 的 owner identity。
- StyleHost 和 ThemeContext 来源。
- owner-scoped Style 是否可达。
- 独立 TopLevel 的 ThemeContextLease 生命周期。
- 关闭、回收和 host 切换后的资源释放。

无法维持 owner Selector scope 时，应为该宿主定义明确的作用域或 public host 契约，不能通过 VisualTree 全局搜索
复制 Style。

### 9.3 ItemContainer 与虚拟化

重复 Part 使用 `Multiple`。ItemContainer 创建、prepare、clear 和 recycle 必须保证：

- semantic class 在首次创建和重复使用时一致。
- 从一个 owner 转移到另一个 owner 时更新所属状态。
- 旧的业务 class、伪类和绑定不会泄漏到新 item。
- Selector 不依赖当前虚拟化面板的具体类型。

`ItemContainerTheme` 只在控件确实允许完整替换 container ControlTheme 时提供，不是 Semantic Part 的必要条件。

## 10. Semantic Part Theme

`SelectorAndTheme` Part 必须对应真实 public Control，并通过 owner 的强类型 `ControlTheme?` 属性开放完整替换：

```csharp
public ControlTheme? SearchButtonTheme { get; set; }
```

约束如下：

- Selector 仍然是该 Part 的基础契约。
- Theme 属性必须 public 可读写，不接受 private setter 或 init-only 属性。
- Theme 属性只用于完整替换，不承担普通局部 Setter 的职责。
- Theme target 必须与 Part `ContractType` 兼容并且是 public Control。
- Part 保留自己的 Control identity；owner 与 Part 可以分别消费自己的 TokenResource。
- 不使用 `Dictionary<string, ControlTheme>`、字符串查找或运行时 Theme factory。

现有 `ControlThemeSemanticPartDescriptor` 只描述主题资产与强类型 Theme 属性的关系。它是
`ControlSemanticDescriptor` 中可选 Theme 扩展的资产元数据，不代替 Selector Part descriptor。

## 11. 声明与生成

Control 使用可重复声明描述公开 Part。声明模型表达语义，不携带 Style 实例或模板节点引用：

```csharp
[SemanticPart(
    "icon",
    SelectorClass = "semantic-icon",
    ContractType = typeof(Control),
    Cardinality = SemanticPartCardinality.Multiple,
    Since = "6.0")]
[SemanticPart(
    "content",
    SelectorClass = "semantic-content",
    ContractType = typeof(ContentPresenter),
    Since = "6.0")]
public partial class Button;
```

生成器负责产生静态 descriptor、Part 名称与 class 常量、包级注册以及诊断。详细输入输出见
[Semantic Part Generator](../../../modules/generator/semantic-part-generator.md)。

同一 Control 的 partial 声明按 CLR symbol 合并。公开 Semantic Control 必须是 non-generic public Control。模板复用只在
typed `BasedOn="{StaticResource {x:Type ...}}"` 可静态解析时沿继承链验证；同一模板节点不得同时承担多个公开 Part。

## 12. 性能与 AOT

Semantic Part 的默认固定成本是确保公开节点具有 `Classes` 集合并保存稳定 marker；节点已有 class 时只增加对应
marker entry。静态 `Classes.semantic-*="True"` 在模板初始化时执行一次 `Classes.Set`，不建立 Binding 或持久 listener。
未声明用户 Semantic Style 时，descriptor 和 marker 不创建 selector activator、VisualTree 查询或实例级 Part 对象。

用户声明 `.semantic-*` Style 后，Avalonia 把 class selector 作为动态条件处理。对于进入匹配 owner/template scope 的
候选节点，Style 会保留 class activator 并监听 `Classes` 变化，即使节点当前没有目标 class。`ContractType` 写成
`:is(...)` 不能消除该成本；`x:SetterTargetType` 是编译期元数据，不增加运行时 selector 或 subscription。

因此必须遵守：

- AtomUI 内置 ControlTheme 不使用 `.semantic-*` 实现默认视觉，继续使用内部精确 selector、属性和 Token。
- semantic marker 使用静态 `Classes.semantic-*="True"` 在模板创建时设置，运行期间不根据状态动态增删。
- 一个 Part 的多个 Setter 合并在同一个 Style 中，避免重复 class activator。
- Application 与大范围 StyleHost 的 Semantic Style 必须使用 owner scope；高密度场景优先缩小 StyleHost 范围。
- 虚拟化 ItemContainer、DataGrid cell 等高频实例在开放或使用 Semantic Style 前必须验证 marker 数量、候选节点数、
  class listener 数量和回收释放。
- 默认设计值、常用尺寸和热路径状态优先使用 Token、StyledProperty、伪类或 ItemContainer Theme，不把 Semantic
  Style 当作内部主题基础设施。

Semantic Part 不引入：

- VisualTree 搜索。
- 运行时 AXAML 解析。
- 按帧、布局或 pointer move 分配。
- 反射扫描 Control、Theme 或属性。
- Part 到 Style 的动态字典合并。
- Control 实例级 descriptor 对象。

Descriptor、ContractType identity、Part 常量和注册入口全部由生成器静态产生。Gallery 可以使用 public
VisualTree API 查找已实例化 marker 进行预览，但该路径不能进入控件运行时样式逻辑。Gallery Preview 必须位于
`AtomUI.Toolkits.GalleryBase` 或具体产品 Gallery，Control 包不得反向引用、注册或感知 Preview。

Avalonia 升级后必须重新验证裸类型与 `:is(...)` 的匹配语义、class activator 订阅模型、`/template/` 的
`TemplatedParent` 边界以及 `x:SetterTargetType` 的编译行为，不能把当前实现细节无条件外推到新版本。

## 13. 兼容性

以下变更属于公共主题契约变更：

| 变更 | 兼容性 |
| --- | --- |
| 新增 Optional Part | 兼容增加。 |
| 新增稳定 Single/Multiple Part | 兼容增加，但必须覆盖所有内置模板。 |
| 删除或重命名 Part | 破坏性变更。 |
| 修改 selector class | 破坏性变更。 |
| 收窄 ContractType | 破坏性变更。 |
| 修改 Single/Optional/Multiple 语义 | 需要兼容性评估。 |
| 替换内部节点但保留语义、class 和 ContractType | 兼容。 |
| 增加可选 Semantic Part Theme | 兼容增加。 |

用户依赖未声明的节点类型、`PART_*`、Name、视觉祖先顺序或 internal class 不属于 Semantic Part 兼容保证。

## 14. 文档与工具

每个采用 Semantic Part 的 Control 必须在 `overview.md` 中维护自身 Part 表，并在 `implementation.md` 中解释 marker
对应的真实模板或运行时节点。表格至少包含：

```text
Part
Selector
ContractType
Cardinality
AtomUI Node
Responsibility
Related API
Related Token
Customization
Stability
```

文档描述当前公共契约，不从 Part 表反推出不存在的 AXAML。Gallery Semantic Preview 读取生成 descriptor，并通过
semantic class 高亮已实例化节点；Popup Part 只有在对应 Popup 打开后才参与可视高亮。

Gallery Preview 是独立工具层，不属于 Control Semantic Part Runtime。它必须遵守以下边界：

- Semantic Parts 使用独立 ShowCase Tab，并在用户第一次进入该 Tab 时才创建 Preview 和演示 Control。
- Preview 已创建但没有 Hover/Pin 时，不扫描 owner VisualTree、不创建 Adorner、不监听 Popup 或布局。
- Hover/Pin 时只进行 owner-scoped 查找，并把临时高亮 Adorner 放入目标对应的 Avalonia `AdornerLayer`。
- 取消选择、切换 Tab、Popup 关闭或页面 detach 时先清空高亮 Adorner 的 `AdornedElement` 关联，再移除 Adorner 并释放临时订阅。
- Preview 不向 Control、ControlTheme 或模板节点注入 class、Style、Binding、属性、事件或调试状态。
- 独立宿主由具体 Gallery Demo 显式提供附加 root，不允许通过全局 TopLevel 搜索补偿。

完整页面模型、目标解析、Popup、性能预算和验证契约见
[Semantic Part Gallery Preview](../../../gallery/authoring/semantic-part-preview.md)。

## 15. 验证要求

Semantic Part 实现至少验证：

1. 声明名称、class、ContractType 和 cardinality。
2. 所有内置 ControlTemplate 与平台主题变体的 marker 完整性。
3. Application、局部 StyleHost 和 owner 实例 Styles 的 Selector 命中。
4. class-only selector 配合 `x:SetterTargetType=ContractType` 的 AXAML 编译，并确认文档不生成
   `ContractType.semantic-*` 或 `:is(ContractType).semantic-*` 公共示例。
5. TemplateBinding、Semantic Style 和 LocalValue 的优先级边界。
6. 布局型 Setter 与 owner Height/MinHeight/MaxHeight、Padding、裁剪和自定义 Measure/Arrange 的协调结果。
7. 替代实现、状态切换和 Optional Part 的一致性。
8. PopupRoot 与 OverlayPopupHost 两种路径。
9. ItemContainer 创建、回收、re-template 和 owner 切换，并确认 detach 后不保留 class listener。
10. 高密度控件在真实模板下的 marker、候选节点与 class listener 结构预算。
11. descriptor、Control 文档与 Gallery 元数据一致性。
12. 生成结果确定性、裁剪和 NativeAOT publish。

Button 的 `root`、`icon`、`content` 可以作为基础契约测试样本；它不拥有 Semantic Part 系统架构。

## 16. 相关文档

- [Semantic Part Generator 设计](../../../modules/generator/semantic-part-generator.md)：构建期输入、模板分析、descriptor、诊断和
  AOT 输出。
- [AtomUI.Core 模块](../../../modules/core/overview.md)：Core descriptor 校验、包级注册、冻结 registry 与运行时查询。
- [AtomUI 控件研发标准规范](../../../engineering/development/control-development-guidelines.md)：Control 作者必须遵守的 Part
  声明、模板和兼容性规则。
- [AtomUI 控件 Token 设计规范](../../../engineering/development/control-token-guidelines.md)：Semantic Part、Part Theme 和 Token
  identity 的职责边界。
- [AtomUI 控件文档规范](../../../engineering/contributing/control-documentation-guidelines.md)：单控件 Semantic Parts 表与 LLMS
  文档同步规则。
- [AOT 编程规范](../../../engineering/development/aot-programming-guidelines.md)：静态注册、反射和运行时发现边界。
