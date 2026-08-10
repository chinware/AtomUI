# AtomUI 响应式机制设计

本文档定义 AtomUI 控件体系的响应式基础机制，用于 Grid、Descriptions、Masonry 以及其他需要按媒体断点解析公共属性的控件。

响应式机制属于 `AtomUI.Controls.Shared` 的跨控件基础能力。具体控件文档只描述该控件如何消费响应式值，不重复断点定义、解析顺序和 fallback 规则。

## 1. 定位

响应式机制负责把“当前媒体断点”和“用户声明的响应式配置”解析为控件可直接消费的有效值。

它提供三类能力：

- 统一媒体断点集合、顺序和 Token 来源。
- 统一 partial breakpoint map 的解析和求值规则。
- 统一 XAML 字符串格式、显式配置记录、fallback 处理边界和验证策略。

它不负责具体控件布局算法。Grid、Descriptions、Masonry 只消费解析后的有效值，并保留各自的布局、夹取、默认值和视觉契约。

## 2. 核心规则

AtomUI 响应式机制遵守以下稳定规则：

- `xs / sm / md / lg / xl / xxl / xxxl` 是统一断点集合。
- active screen 采用累计语义。当前为 `lg` 时，`xs`、`sm`、`md`、`lg` 均视为 active。
- partial breakpoint map 采用 mobile-first cascade。未配置的更大断点继承最近的较小断点配置。
- 求值时按 `xxxl -> xxl -> xl -> lg -> md -> sm -> xs` 从大到小查找，第一个“当前 active 且显式配置过”的值生效。
- 响应式解析器不拥有控件默认值。默认值由具体控件在 resolver 未命中时提供。

这些规则是 AtomUI 的长期响应式契约。实现调整必须通过公共模型和跨控件测试验证兼容性，不能以某个控件的
局部实现替代共享规则。

## 3. 断点模型

AtomUI 的媒体断点必须以 Alias Token 为唯一数值来源。

| 短名 | `MediaBreakPoint` | 起始 Token | 结束 Token |
|---|---|---|---|
| `xs` | `ExtraSmall` | 无 | `ScreenSMMin - 1` |
| `sm` | `Small` | `ScreenSMMin` | `ScreenSMMax` |
| `md` | `Medium` | `ScreenMDMin` | `ScreenMDMax` |
| `lg` | `Large` | `ScreenLGMin` | `ScreenLGMax` |
| `xl` | `ExtraLarge` | `ScreenXLMin` | `ScreenXLMax` |
| `xxl` | `ExtraExtraLarge` | `ScreenXXLMin` | `ScreenXXLMax` |
| `xxxl` | `ExtraExtraExtraLarge` | `ScreenXXXLMin` | 无 |

Token 要求：

- `ScreenXXXL` 与 `ScreenXXXLMin` 定义最大媒体断点。
- `ScreenXXLMax` 必须等于 `ScreenXXXLMin - 1`。
- `ScreenXS`、`ScreenXSMin`、`ScreenXSMax` 保留完整 Token 语义；运行时 `xs` 当前断点范围以 `ScreenSMMin - 1` 为上界。
- `MediaBreakPoint` 枚举值必须按从小到大排列，保证 `current >= configured` 可表达 mobile-first cascade。

## 4. Active Screen 与求值

响应式解析器以当前断点构造 active screen 集合。

```text
current = lg
active = xs, sm, md, lg

current = xxxl
active = xs, sm, md, lg, xl, xxl, xxxl
```

求值顺序固定为：

```text
xxxl -> xxl -> xl -> lg -> md -> sm -> xs
```

求值规则：

```text
Resolve(current, map, fallback)
  for key in descending breakpoint order:
    if key is active for current and map explicitly contains key:
      return map[key]
  return fallback
```

示例：

| 当前断点 | 配置 | 结果 |
|---|---|---|
| `xs` | `xs: 1, md: 3` | `1` |
| `sm` | `xs: 1, md: 3` | `1` |
| `md` | `xs: 1, md: 3` | `3` |
| `lg` | `xs: 1, md: 3` | `3` |
| `xxl` | `xl: 4`，fallback `3` | `4` |
| `sm` | `xl: 4`，fallback `2` | `2` |

解析器必须区分“未配置”和“配置值等于默认值”。不得在 Parse 阶段用默认值填满所有断点。

## 5. XAML 与公共 API 形态

响应式公共 API 必须同时满足 Avalonia 属性系统、XAML 可读性、AOT 兼容和控件语义。

### 5.1 属性注册

控件公开响应式属性时，必须声明对应 Avalonia 静态属性和 CLR wrapper。

```csharp
public static readonly StyledProperty<ResponsiveInt?> ColumnInfoProperty =
    AvaloniaProperty.Register<Masonry, ResponsiveInt?>(nameof(ColumnInfo));

public ResponsiveInt? ColumnInfo
{
    get => GetValue(ColumnInfoProperty);
    set => SetValue(ColumnInfoProperty, value);
}
```

attached responsive property 必须同样提供 `AttachedProperty`、`GetXxx`、`SetXxx`。

### 5.2 标准响应式类型

响应式值类型分为公共 XAML 友好类型和底层共享求值器。标准类型按值域命名，而不是按控件命名：

| 值域 | 标准类型 | 用途 |
|---|---|---|
| 整数 | `ResponsiveInt` | 列数、span、order 等整数型响应式值 |
| 浮点数 | `ResponsiveDouble` | 间距、尺寸等数值型响应式值 |
| GridLength | `ResponsiveGridLength` | 表单、布局中按断点变化的 GridLength |
| 双向间距 | `ResponsiveGutter` | Row、Masonry 等水平/垂直 gutter |
| 枚举 | 专项 wrapper | `RowAlign`、`RowJustify` 等枚举值，内部仍委托共享 resolver |

标准类型要求：

- 必须能表达 scalar 固定值和 partial breakpoint map。
- 必须记录哪些断点由用户显式配置。
- 必须委托底层共享求值器完成断点排序、显式配置检查和 fallback 处理。
- 不允许每个控件各自复制 breakpoint parser 和 switch 逻辑。
- 不再为单个控件新增 `DescriptionsMediaBreakInfo` 这类只服务单控件且复制规则的类型；既有同类类型完成标准 API 迁移后应删除。

### 5.3 字符串格式

响应式值必须支持以下 XAML 字符串格式：

```xml
ColumnInfo="3"
ColumnInfo="xs: 1, md: 3, xl: 4"
Gutter="16"
Gutter="16, 24"
Gutter="xs: 8, md: 16"
```

格式规则：

- scalar 值表示所有断点固定为同一值。
- Gutter scalar 值表示水平和垂直两个维度使用同一固定值。
- breakpoint map 只记录显式出现的断点。
- breakpoint key 大小写不敏感，但文档和示例统一使用小写。
- 未知 breakpoint、空 key、空 value、非法数值必须抛出明确异常。
- 重复 breakpoint key 必须抛出明确异常，避免字符串配置产生顺序依赖。
- 计数类值必须大于 `0`；间距类值必须大于等于 `0`；具体值域的合法范围由标准类型负责校验。
- Gutter 类值支持水平、垂直两个维度；两个维度分别使用同一套 responsive resolver。

## 6. 控件集成规则

### 6.1 Grid

Grid 是响应式机制的基准控件。

`Col` 的 `Xs / Sm / Md / Lg / Xl / Xxl / Xxxl` 断点属性属于显式 breakpoint 配置。解析时先建立基础布局，再按从小到大顺序应用当前断点以内的显式配置，等价于“取当前 active 范围内最大的已配置断点”。

`Col.Flex` 用于填充行内剩余空间。数值表示 flex grow/shrink，`auto` 表示自动基准的 flex 项，`none` 表示不伸缩，`100px` 这类固定像素基准表示固定 basis。断点级 `GridColSize.Flex` 会覆盖基础 span 宽度并参与同一行的剩余空间分配。

`Col.Span=0` 和断点级 `GridColSize.Span=0` 表示隐藏该列，布局时不占用行宽，并在 arrange 阶段收敛到零尺寸。未显式设置 `Span` 的 `Col` 仍保持自然宽度行为，不能因为默认 `GridColSpanInfo` 为 `0` 而被误判为隐藏。

`Row.Gutter` 的水平和垂直间距分别使用响应式 resolver。水平 gutter 的间距只出现在列之间，不出现在 Row 两端；实现上通过扩展 Row 的内部布局面并从 `-gutter / 2` 开始排布来抵消首尾半个 gutter。

`Row.JustifyInfo` 与 `Row.AlignInfo` 为响应式覆盖入口，未设置时完全沿用 `Row.Justify` 与 `Row.Align`。响应式行对齐必须使用同一套 resolver，不能引入单独解析规则。

### 6.2 Descriptions

`Descriptions.ColumnInfo` 表示每行 DescriptionItem 数量，使用共享规则解析 partial breakpoint map。

默认 fallback map：

| 断点 | 默认列数 |
|---|---|
| `xs` | `1` |
| `sm` | `2` |
| `md` | `3` |
| `lg` | `3` |
| `xl` | `3` |
| `xxl` | `3` |
| `xxxl` | `4` |

`DescriptionItem.Span` 表示 item 跨列数量。未声明时 fallback 为 `1`；声明为 partial map 时同样使用 mobile-first cascade。`IsFilled` 表示填满当前行剩余空间，优先级高于解析后的 span 数值。

### 6.3 Masonry

`Masonry.ColumnInfo` 表示列数配置，接受 scalar 或 partial breakpoint map。

列数优先级：

1. `ColumnInfo` 命中当前断点时，使用响应式列数。
2. `ColumnCount > 0` 时，使用固定列数。
3. 使用 `MinColumnWidth + MaxColumnCount + AvailableWidth` 的容器自适应列数。

`Masonry.Gutter` 表示间距配置，支持 scalar、水平/垂直 pair、responsive map 和水平/垂直 responsive pair。

间距优先级：

1. `Gutter` 命中当前断点时，使用响应式水平/垂直间距。
2. 使用兼容属性 `ColumnGap` 与 `RowGap`。

Masonry 的 shortest-column 布局、整行项、显式列、`ItemsSource` 容器模型和 `LayoutChanged` 事件不属于响应式 resolver 职责。

## 7. Fallback 边界

共享 resolver 只回答“用户配置在当前断点是否命中”。它不内置任何控件默认值。

控件必须在自己的 effective state 层提供 fallback：

- Grid 负责 24 栅格默认 span、offset、order、push、pull。
- Descriptions 负责默认列数 map、默认 item span 和 bordered 布局换算。
- Masonry 负责默认列数、容器自适应列数和默认间距。

该边界保证新增控件可以复用同一套 responsive resolver，同时保留自身业务语义。

## 8. 生命周期与性能

响应式控件通过 `MediaQueryHost.FindOwner` 获取最近的 `IMediaBreakAwareControl`，并订阅 `MediaBreakPointChanged`。

维护要求：

- 订阅必须在 detached 或 owner 替换时释放。
- breakpoint 变化只更新 effective state 并触发布局失效，不在事件回调中直接执行完整布局。
- 影响 layout 的响应式属性必须注册 `AffectsMeasure` 或在属性变更路径中触发父级布局失效。
- resolver 必须是无视觉依赖、无全局状态的纯计算逻辑。
- parser 与 resolver 不依赖 reflection，不要求运行时动态生成代码，保持 NativeAOT 兼容。

## 9. 兼容性不变量

响应式机制变更必须保持以下不变量：

- 同一配置在 Grid、Descriptions、Masonry 中使用相同断点集合和继承规则。
- partial map 不得在解析阶段被默认值填满。
- scalar 配置必须保持跨断点固定值语义。
- 控件 fallback 不得泄漏进共享 resolver。
- `xxxl` 是合法断点，文档、parser、resolver、Token 和媒体断点枚举必须一致。
- 公共响应式属性必须遵守 Avalonia 属性注册规范。
- XAML 示例中的短名 `xs / sm / md / lg / xl / xxl / xxxl` 是稳定契约。

## 10. 验证策略

共享响应式机制至少需要覆盖以下测试：

- `xs / sm / md / lg / xl / xxl / xxxl` key 解析。
- scalar 值在所有断点下返回同一结果。
- partial map 在大屏继承最近较小断点。
- 当前断点小于所有显式配置时返回控件 fallback。
- `0`、空字符串、未知 key、非法数字、重复 key 的处理符合对应值类型约束。
- Gutter 水平和垂直维度独立解析。
- `MediaBreakPointChanged` 只触发布局失效，不重复订阅或泄漏 owner。

控件级验证：

- Grid 验证 `Col` 断点属性、`Row.Gutter` 和响应式 align/justify 的有效状态。
- Descriptions 验证 `ColumnInfo="xs: 1, md: 3"` 在 `lg` 下解析为 `3`，并验证 item `Span` 同规则。
- Masonry 验证 `ColumnInfo` 优先级、`Gutter` 优先级和 fallback 到容器自适应列数。

文档验证：

```bash
git diff --check
```
