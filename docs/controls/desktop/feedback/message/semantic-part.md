# Message Semantic Part 契约

本文档定义 Message 对应用公开的 Semantic Part、选择器、类型约束、数量语义和定制边界。Message 的整体设计见
[Message 桌面版架构设计](overview.md)，内部实现原理见 [Message 桌面版实现原理](implementation.md)，Token 设计见
[Message Token 设计](token.md)，系统级规则见
[AtomUI Semantic Part 系统设计](../../../../architecture/systems/theming/semantic-parts.md)。

## 1. Semantic Parts

Message 由两个 public owner 组成，因此公开两个独立 descriptor：

| Owner | 职责 |
| --- | --- |
| `MessageCard` | 单条消息卡片（上游 notice），承载 `root`、`wrapper`、`icon`、`title` 四个 Part。 |
| `WindowMessageManager` | 服务型消息宿主（上游 message list），承载 `root`（= 上游 `list`）与 `listContent` 两个 Part。 |

上游 `MessageSemanticType` 的 `classNames` / `styles` 均为
`{ root?, wrapper?, icon?, title?, list?, listContent? }` 六个键（上游 6.6.3 稳定发布源码
`message/interface.ts`，语义 demo `message/demo/_semantic.tsx`）。上游 DOM 嵌套为
`list > listContent > root > wrapper > [icon, title]`；`root` 自 6.0.0 起公开，`wrapper`、`title`、`list`、
`listContent` 自 6.4.0 起公开。

AtomUI 把六个上游 Part 一一映射到两个 owner 的真实节点：

```text
WindowMessageManager (root，对应上游 list)
  └─ ReversibleStackPanel#PART_Items (listContent, .semantic-list-content)
       └─ MessageCard (root，对应上游 notice root)
            └─ MotionActor
                 └─ Border#PART_Frame (root 表面投影：背景/圆角/阴影/内边距)
                      └─ DockPanel#PART_HeaderContainer (wrapper, .semantic-wrapper)
                           ├─ IconPresenter#PART_IconContent (icon, .semantic-icon)
                           └─ SelectableTextBlock#PART_Message (title, .semantic-title)
```

上游 `root` 就是 message notice 本身，因此映射到 `MessageCard` owner；上游 `list` 是承载全部 notice 的定位容器，
映射到 `WindowMessageManager` owner。两个 owner 各自拥有隐式 `root`，不额外声明 `.semantic-root` marker。

与上游 DOM 的两处结构差异（Part 名称、数量与样式语义不变）：

- 上游 `list` 自身承担 placement（`top` / `left` / `right`）；AtomUI 的 `WindowMessageManager` 铺满 TopLevel 并只
  负责安全区外边距，具体对齐由 `ReversibleStackPanel#PART_Items`（`listContent`）实际承担。因此"放置"语义在
  AtomUI 由 `root`（`Position` 属性与宿主层范围的作用域）与 `listContent`（实际排列容器）共同表达。以无宿主构造
  内联使用时没有宿主层，`root` 退化为普通可放置控件，placement 语义不适用。当前控件未在
  `Position` 变化时更新伪类，主题中的 `:topcenter` 对齐选择器不可达，位置对齐尚未生效（见第 7 节残余风险）。
- 上游 `wrapper` 用 flex `gap: marginXS` + `align-items: center` 排列 icon 与 title；AtomUI `DockPanel` 无 `Spacing`，
  等价的图标间距由 `IconPresenter` 的 `MessageIconMargin`（右外边距 `UniformlyMarginXS`）表达，视觉结果一致。

两个 owner 的 descriptor `Since` 统一为 `6.0`（AtomUI Semantic Part 首版约定，不逐 Part 记录上游小版本）。

以下类型不持有独立 Semantic descriptor：

- `Message`（`IMessage` 的实现）是服务调用的数据对象，不是 Control。
- `IMessage` / `IMessageManager` 是服务契约接口。
- `MessageCardPseudoClass`、`MessageType` 是状态与枚举类型。
- `MessageCardToken` 是 Token scope；Semantic Part 不产生 Token identity。

### 1.1 `MessageCard`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `MessageCard` |
| Part | `root` |
| Selector | MessageCard 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `MessageCard` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | MessageCard owner（表面投影到 `Border#PART_Frame`，动效由 `MotionActor` 承载） |
| 职责 | 单条消息项根元素：承载 `Message`、`MessageType`、`Icon`、`IsClosing`、`IsClosed`、`IsMotionEnabled` 与进入/退出动效；根表面（背景、圆角、阴影、内边距）投影到模板中的 `Border#PART_Frame`。对应上游 notice root。 |
| 相关 API | `Message`、`MessageType`、`Icon`、`IsClosing`、`IsClosed`、`IsMotionEnabled`、`Close()`、`MessageClosed` |
| 相关 Token | `ContentBg`、`ContentPadding`、`MessageTopMargin`、SharedToken（`BoxShadows`、`BorderRadiusLG`） |
| 稳定性 | stable since 6.0 |

#### `wrapper`

| 字段 | 值 |
| --- | --- |
| Owner | `MessageCard` |
| Part | `wrapper` |
| Selector | `.semantic-wrapper` |
| SelectorRoute | `/template/ .semantic-wrapper` |
| Style Type | `MessageCardWrapperStyle` |
| ContractType | `DockPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | MessageCard 模板中的 `DockPanel#PART_HeaderContainer` |
| 职责 | 图标与标题的包裹元素：决定 icon/title 的排列方向、对齐与图标间距。对应上游 notice wrapper。 |
| 相关 API | `Icon`、`Message`（决定子节点可见性） |
| 相关 Token | `MessageIconMargin` |
| 稳定性 | stable since 6.0 |

#### `icon`

| 字段 | 值 |
| --- | --- |
| Owner | `MessageCard` |
| Part | `icon` |
| Selector | `.semantic-icon` |
| SelectorRoute | `/template/ .semantic-icon` |
| Style Type | `MessageCardIconStyle` |
| ContractType | `IconPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | MessageCard 模板中的 `IconPresenter#PART_IconContent` |
| 职责 | 状态图标元素：尺寸、画刷与行高；`MessageType` 决定默认图标与状态色（Information/Loading = `ColorPrimary`，Success = `ColorSuccess`，Warning = `ColorWarning`，Error = `ColorError`）。对应上游 notice icon。 |
| 相关 API | `Icon`、`MessageType` |
| 相关 Token | `MessageIconSize`、SharedToken（`ColorPrimary`、`ColorSuccess`、`ColorWarning`、`ColorError`） |
| 稳定性 | stable since 6.0 |

#### `title`

| 字段 | 值 |
| --- | --- |
| Owner | `MessageCard` |
| Part | `title` |
| Selector | `.semantic-title` |
| SelectorRoute | `/template/ .semantic-title` |
| Style Type | `MessageCardTitleStyle` |
| ContractType | `Avalonia.Controls.SelectableTextBlock` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | MessageCard 模板中的 `SelectableTextBlock#PART_Message`（Avalonia 类型，非 `atom:` 前缀） |
| 职责 | 消息文本元素：文本颜色、字号、行高、换行与文本选择样式。对应上游 notice title（上游把 `content` 作为 notice title 渲染）。 |
| 相关 API | `Message` |
| 相关 Token | SharedToken（`FontSize`、`FontHeight`、`ColorText`、`SelectionBackground`、`SelectionForeground`） |
| 稳定性 | stable since 6.0 |

### 1.2 `WindowMessageManager`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `WindowMessageManager` |
| Part | `root` |
| Selector | WindowMessageManager 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `WindowMessageManager` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | WindowMessageManager owner（宿主层/full-screen 覆盖层；无宿主构造时为内联可放置实例） |
| 职责 | 消息列表根元素：承载 `Position`、`MaxItems`、`IsMotionEnabled`，管理宿主层安装、消息队列、超时关闭与宿主 detach；对应上游 `list` 的定位/层级/宽度语义。 |
| 相关 API | `Position`、`MaxItems`、`IsMotionEnabled`、`Show(IMessage)`、`Dispose()` |
| 相关 Token | SharedToken（`EnableMotion`） |
| 稳定性 | stable since 6.0 |

#### `listContent`

| 字段 | 值 |
| --- | --- |
| Owner | `WindowMessageManager` |
| Part | `listContent` |
| Selector | `.semantic-list-content` |
| SelectorRoute | `/template/ .semantic-list-content` |
| Style Type | `WindowMessageManagerListContentStyle` |
| ContractType | `ReversibleStackPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | WindowMessageManager 模板中的 `ReversibleStackPanel#PART_Items` |
| 职责 | 消息列表内容元素：notice 的排列方向、顺序与对齐；对应上游 `listContent` 的 notice 排列/间距语义。 |
| 相关 API | `Position`、`MaxItems`（决定内容区可见项数量） |
| 相关 Token | SharedToken（`EnableMotion`） |
| 稳定性 | stable since 6.0 |

隐式 `root` 不声明 `.semantic-root` marker。`MessageCard` 的 `wrapper`、`icon`、`title` marker 静态声明在
`MessageCardTheme.axaml` 模板内；`WindowMessageManager` 的 `listContent` marker 静态声明在
`WindowMessageManagerTheme.axaml` 模板内。四个 marker 都在 owner 自身的 `ControlTheme` 资产中，因此两个 descriptor
都不需要 `RuntimeCreated=true`，生成器按 owner 主题资产做静态校验。

## 2. Part 说明

### 2.1 MessageCard root

`root` 是 `MessageCard` owner 本身，每条可见消息恰好一个。它承载 `Message`、`MessageType`、`Icon` 三个内容/状态
API，`IsClosing` / `IsClosed` 关闭状态，以及 `IsMotionEnabled` 与进出场动效。

它负责：

- 单条消息的根语义与状态机：`Close()` 置 `IsClosing`，关闭动效完成后提交一次 `IsClosed=true` 并抛出
  `MessageClosed`。
- 根视觉表面：背景（`ContentBg`）、内边距（`ContentPadding`）、圆角（`BorderRadiusLG`）、阴影（`BoxShadows`）
  与卡片间距（`MessageTopMargin`），投影到模板中的 `Border#PART_Frame`。
- 作为 `wrapper` / `icon` / `title` owner-scoped Selector 的作用域边界。

适合通过 `Background`、`Padding`、`CornerRadius`、`Foreground`、`FontSize` 等 owner 侧 Setter 定制整卡视觉。
`root` 不表示模板中的 `MotionActor`、`Border#PART_Frame` 或 `DockPanel#PART_HeaderContainer` 节点本身。

### 2.2 MessageCard wrapper / icon / title

三个 Part 在 `MessageCard` 模板内各对应一个常驻节点，cardinality 均为 `Single`：

- `wrapper` 承载 `DockPanel#PART_HeaderContainer`：icon 停靠左侧、title 填充剩余宽度；`MessageIconMargin` 提供
  图标与文本之间的间距。适合定制 `Padding`、`Margin`、`HorizontalAlignment` 等布局属性。
- `icon` 承载 `IconPresenter#PART_IconContent`：`Icon` 为空时由 `MessageType` 提供默认图标，`IsVisible` 由
  `Icon` 是否为 null 决定。适合定制 `Width`、`Height`、`Margin`、`IconBrush`。
- `title` 承载 `SelectableTextBlock#PART_Message`：`Message` 文本呈现，支持文本选择。该节点是 Avalonia 的
  `Avalonia.Controls.SelectableTextBlock`（主题中无前缀书写），不是 AtomUI 的 `atom:SelectableTextBlock`，
  `ContractType` 与 `x:SetterTargetType` 据此声明。适合定制 `Foreground`、`FontSize`、`LineHeight`、
  `TextWrapping`。

`icon` 的可见性由 `Icon` 数据状态驱动（`SetupDefaultMessageIcon` 会为所有 `MessageType` 设置默认图标）；
Semantic Style 覆盖 `IsVisible` 会绕过数据状态机，属于不推荐用法。

### 2.3 WindowMessageManager root / listContent

两个 Part 在 `WindowMessageManager` 内各对应一个稳定节点，cardinality 均为 `Single`：

- `root` 承载 manager owner 本身：安装到 TopLevel 宿主层（`WindowFeedbackLayer` 优先，回退
  `AdornerLayer`）时它是 full-screen 覆盖层，负责安全区外边距、消息队列（`_pendingMessages`）、超时计时器
  （`_messageCloseTimers`）、`MaxItems` 清理与 `Dispose` 卸载。`WindowMessageManager` 另提供无参构造，不安装到
  任何层，作为普通可放置控件放进调用方自己的视觉树（上游把 list 改为内联容器的等价物），此时 `root` 即该内联实例。
- `listContent` 承载 `ReversibleStackPanel#PART_Items`：所有 `MessageCard` 的逻辑父级，决定 notice 的排列方向、
  `ReverseOrder` 与对齐。主题按 `Position` 伪类声明对齐分支（当前仅有 `^:topcenter`）；该伪类在控件侧尚未触发，
  位置对齐暂未生效（见第 7 节）。

适合通过 `root` 定制 manager 的安全区外边距与宿主层相关视觉，通过 `listContent` 定制 notice 排列间距与对齐。

## 3. Selector 用法

应用级样式先限定 owner，再通过生成的 Semantic Style 进入 Part。生成类型已经封装 owner 类型保护和
`SelectorRoute`，用户不需要复制模板路径：

```xml
<Application.Styles>
    <Style Selector="atom|MessageCard">
        <atom:MessageCardWrapperStyle x:SetterTargetType="DockPanel">
            <Setter Property="Margin" Value="0" />
        </atom:MessageCardWrapperStyle>
        <atom:MessageCardIconStyle x:SetterTargetType="atom:IconPresenter">
            <Setter Property="IconBrush" Value="#1677FF" />
        </atom:MessageCardIconStyle>
        <atom:MessageCardTitleStyle x:SetterTargetType="SelectableTextBlock">
            <Setter Property="FontWeight" Value="SemiBold" />
        </atom:MessageCardTitleStyle>    </Style>

    <Style Selector="atom|WindowMessageManager">
        <atom:WindowMessageManagerListContentStyle x:SetterTargetType="atom:ReversibleStackPanel">
            <Setter Property="Spacing" Value="12" />
        </atom:WindowMessageManagerListContentStyle>
    </Style>
</Application.Styles>
```

对特定 class 或状态定制时，把 class、属性或伪类放在 owner 一侧：

```xml
<Style Selector="atom|MessageCard:error">
    <atom:MessageCardIconStyle x:SetterTargetType="atom:IconPresenter">
        <Setter Property="IconBrush" Value="#CF1322" />
    </atom:MessageCardIconStyle>
</Style>
```

不得把 `ContractType` 写入 Part Selector。以下写法不属于公共契约：

- `atom|IconPresenter.semantic-icon` 或 `:is(atom|IconPresenter).semantic-icon`。
- 直接复制 `/template/ .semantic-wrapper` 等 route 作为用户主路径；route 只属于 descriptor 与生成 Style 的
  实现元数据。
- 依赖 `PART_*`、`Name` 或视觉祖先顺序。
- 通过 Semantic Style 设置 `icon` 的 `IsVisible` 绕过 `Icon` / `MessageType` 数据状态机。

## 4. 状态与数量语义

数量契约以已实例化的 AtomUI 内置节点为边界。四个 Part 都是 owner 自身 `ControlTheme` 的静态模板节点
（`RuntimeCreated=false`），marker 随 owner 实例存在，不随数据项迁移。

| 场景 | MessageCard root / wrapper / icon / title | WindowMessageManager root / listContent | 说明 |
| --- | --- | --- | --- |
| 单条消息 | 各 1 | 各 1 | 每条可见消息一个 MessageCard。 |
| N 条可见消息 | 各 N | 各 1 | manager 只有一个模板实例与一个 `PART_Items`。 |
| `Icon` 为 null | `icon` marker 保留，节点 `IsVisible=false` | — | 高亮与命中只针对有效可见实例。 |
| 自定义 `Icon` | `icon` marker 保留 | — | 自定义图标不新增节点。 |
| Loading 类型 | 各 1 | 各 1 | `LoadingOutlined` 旋转由图标动画承载，不增删 marker。 |
| 超时自动关闭 | 关闭后随卡片移除 | 各 1 | `MessageClosed` 后 manager 从 `PART_Items` 移除卡片。 |
| 手动 `Close()` | 同上 | 各 1 | 与超时走同一关闭动效状态流。 |
| 超过 `MaxItems` | 只保留 `MaxItems` 条可见 | 各 1 | 超出部分被 `Close()`，marker 随卡片移除。 |
| 宿主 detach / `Dispose` | 全部移除 | root 随 manager 卸载 | `OnDetachedFromVisualTree` 清空 `PART_Items`；`Dispose` 卸载事件、计时器与宿主层。 |

## 5. 尺寸基线

Message 没有 `SizeType` 分档，视觉基线由 `MessageCardToken` 与全局 Token 常量表达：

- 卡片背景 `ContentBg`（`ColorBgElevated`），内边距 `ContentPadding`
  （垂直 `(ControlHeightLG - FontSize * RelativeLineHeight) / 2`，水平 `UniformlyPaddingXS`），对齐上游
  `contentPadding` 与 `notificationPaddingVertical` / `notificationPaddingHorizontal`。
- 卡片间距 `MessageTopMargin`（上 `UniformlyMargin`，其余 0，下 0），圆角 SharedToken `BorderRadiusLG`，
  阴影 SharedToken `BoxShadows`。
- 图标尺寸 `MessageIconSize`（`FontSizeSM * RelativeLineHeightSM`），图标外边距 `MessageIconMargin`
  （右 `UniformlyMarginXS`，对齐上游 `gap: marginXS`）。
- 文本字号/行高/颜色为 SharedToken `FontSize`、`FontHeight`、`ColorText`。
- 状态色 Information/Loading = `ColorPrimary`，Success = `ColorSuccess`，Warning = `ColorWarning`，
  Error = `ColorError`。

`WindowMessageManager` 没有独立尺寸 Token：其 `root` 铺满 TopLevel 并由 `TopLevelMarginBinder` 投影安全区外边距，
`listContent` 的对齐与顺序来自 `Position` 伪类。

Semantic Style 覆盖 `wrapper` / `listContent` 的 `Padding` / `Margin` / 对齐时，应验证 notice 仍正确对齐、
`root` 的圆角/阴影仍完整可见；覆盖 `icon` 尺寸时应验证文本基线与垂直居中没有被破坏。注意 `listContent` 当前没有
生效的 `Position` 对齐基线（见第 7 节残余风险），覆盖对齐前应先确认该缺口是否已修复。

## 6. 定制边界

以下区域明确不属于 Message Semantic Part：

- 关闭动效执行状态（`MotionExecutionState` 与 `MotionActor`）、`Close()` 调度与 `IsClosed` 提交路径是行为状态，
  不是 Part。
- `Border#PART_Frame`、`MotionActor` 是模板结构节点：`root` 的表面投影到 `PART_Frame`，但 `PART_Frame` 的名称、
  数量与层级不属于 `root` 契约。
- `_pendingMessages` 队列、`_messageCloseTimers` 计时器、`MaxHostLayerRetryCount` 重试与宿主层安装/卸载是
  manager 的行为实现，不是 Part。
- `MessageCard` 的默认图标选择（`SetupDefaultMessageIcon`）与 `MessageType` 伪类是行为状态；`icon` Part 只表达
  图标元素的视觉。
- `Message`、`IMessage`、`IMessageManager` 数据/契约类型。

Semantic Style 服从 Avalonia 原生属性优先级。Message 没有用户可见的颜色状态机入口：卡片状态色由
`MessageType` 属性 Selector 在主题内设置，用户 Semantic Style setter 覆盖主题 setter，样式移除后主题恢复。

## 7. 兼容性与验证

删除或重命名 Part、修改 selector class、收窄 `ContractType`、改变 cardinality，或者让任一内置模板缺少
marker，均属于公共主题契约变更。

验证至少覆盖：

- descriptor 中 `MessageCard` 只有 `root`、`wrapper`、`icon`、`title`，`WindowMessageManager` 只有 `root`、
  `listContent`，字段值与本文表格一致；`Message`、`IMessage`、`IMessageManager`、`MessageCardToken` 不持有
  descriptor。
- 两个 owner 的主题资产各自携带批准 marker：`MessageCardTheme.axaml` 为 `semantic-wrapper:DockPanel`、
  `semantic-icon:IconPresenter`、`semantic-title:SelectableTextBlock`；`WindowMessageManagerTheme.axaml` 为
  `semantic-list-content:ReversibleStackPanel`。
- 五种 `MessageType`、自定义 `Icon`、多项 queue、超时与手动关闭后 marker 数量符合第 4 节表。
- owner-scoped Semantic Style（四个生成 Style 类型）与 `x:SetterTargetType` 可以编译并命中对应最低 public
  类型。
- 宿主 attach/detach、`Dispose` 后不保留旧卡片、计时器或宿主层引用。
- 默认主题不消费 `.semantic-*`，未声明用户 Semantic Style 时不增加 selector activator。
- Generator 静态输出和 NativeAOT 路径不依赖反射或运行时扫描。

### 7.1 残余风险（不属于本次 Semantic Part 范围，需单独授权）

`WindowMessageManager` 未在 `Position` 变化时更新伪类（`UpdatePseudoClasses` 缺失，`WindowNotificationManager`
有对应实现），因此 `WindowMessageManagerTheme.axaml` 中的 `^:topcenter` 选择器不可达，`Position` 的
`TopCenter` / `BottomCenter` 等对齐分支目前不会生效。该缺口是既有行为问题，不是本次 Semantic Part 引入，修复会改变
渲染结果，需要单独的行为变更授权与视觉验收。`listContent` Part 本身（`PART_Items` 排列容器）不受该缺口影响。
