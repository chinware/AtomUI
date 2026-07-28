# Timeline 桌面版实现原理

本文档描述 Timeline 桌面版的内部实现范围、源码职责、状态流、生命周期、资源边界和维护规则。公共设计与 API 契约见 [Timeline 桌面版架构设计](overview.md)，方向布局矩阵和算法边界见 [Timeline 方向与布局设计](orientation-layout-design.md)，变化记录见 [Timeline Changelog](changelog.md)。涉及组件 Token 的实现应同时阅读 [Timeline Token 设计](token.md)。

## 1. 实现定位

本文档覆盖 Timeline 的控件实现、主题接入、状态同步和 Gallery 可见维护边界。具体属性注册、默认值、绘制细节和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构和不变量。

## 2. 源码文件结构

主要源码文件：

- `src/AtomUI.Controls/Timeline/AbstractTimeline.cs`
- `src/AtomUI.Controls/Timeline/AbstractTimelineItem.cs`
- `src/AtomUI.Controls/Timeline/TimeLineEnums.cs`
- `src/AtomUI.Controls/Timeline/TimelineIndicator.cs`
- `src/AtomUI.Controls/Timeline/TimelineItemPanel.cs`
- `src/AtomUI.Controls/Timeline/TimelineStackPanel.cs`
- `src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineIndicatorTheme.axaml`
- `src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineItemTheme.axaml`
- `src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineTheme.axaml`
- `src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineThemes.axaml`
- `src/AtomUI.Desktop.Controls/Timeline/Timeline.cs`
- `src/AtomUI.Desktop.Controls/Timeline/TimelineItem.cs`
- `src/AtomUI.Desktop.Controls/Timeline/TimelineToken.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 3. 核心类职责

- `AbstractTimeline`：Orientation、Mode、IsReverse、Items 和 Pending 的状态 owner；负责容器准备、可见项视觉顺序和内部状态投影。
- `AbstractTimelineItem`：承载 Label、Content、Indicator API，并接收 owner 投影的 Orientation、Mode、奇偶、首尾、Reverse、Pending 和 Label 布局状态。
- `Timeline`：桌面公开控件，负责创建普通 TimelineItem 和 Pending TimelineItem，不复制共享布局逻辑。
- `TimelineIndicator`：内部 renderer，根据 Orientation、首尾、节点尺寸和自定义图标 Bounds 绘制节点与连接线。
- `TimelineItem`：桌面公开 Item 容器，复用 AbstractTimelineItem 的内容和内部状态契约。
- `TimelineItemPanel`：负责 Label、Indicator 和 Content 的方向化 Measure/Arrange，并把 Alternate 解析为 Start 或 End。
- `TimelineStackPanel`：负责主轴排列、Reverse、可见项过滤、Spacing 和水平等宽槽位。
- `TimelineToken`：组件 Token scope，负责从全局 token 派生控件语义变量。

核心协作规则：

- AbstractTimeline 是集合级方向、模式、视觉顺序和 Pending 邻接状态的唯一 owner。
- AbstractTimelineItem 不根据 Parent Items 索引自行推导位置，只保存 owner 的单向投影。
- TimelineStackPanel 只负责 Item 主轴槽位，不计算 Alternate、首尾或 Pending 状态。
- TimelineItemPanel 只负责单项内部布局，不修改集合级状态。
- TimelineIndicator 只消费布局和视觉属性进行绘制，不参与容器顺序计算。
- Template part 是视觉协作对象，生命周期受 ControlTemplate 和 `OnApplyTemplate` 管理。

## 4. 状态与数据流

Timeline 的状态流遵循下面路径：

```text
Orientation / Mode / IsReverse / Items / item visibility / Label / Pending
  -> AbstractTimeline builds visible visual order
  -> IsOdd / IsFirst / IsLast / NextIsPending / IsLabelLayout
  -> AbstractTimelineItem internal properties and pseudo-classes
  -> TimelineStackPanel / TimelineItemPanel / TimelineIndicator
  -> ControlTheme selector and rendered Timeline
```

状态归一顺序为：

1. 从已生成容器中过滤不可见项。
2. 根据 IsReverse 得到视觉顺序。
3. 按视觉索引投影奇偶和首尾状态。
4. 按视觉邻接关系投影 NextIsPending。
5. 检测任一可见 Item 是否声明 Label，并统一投影 IsLabelLayout。
6. Item 模板把集合状态传递给 Panel 和 Indicator。

Items 增删、Reset、容器索引变化、Item 可见性、Label、IsReverse 和 Pending 变化必须重放相关派生状态。Mode 与 Orientation 不改变源集合顺序，通过 Avalonia 属性失效机制触发重新测量、排列或绘制。

## 5. 组合结构模型

### 5.1 控件角色图

```text
Timeline (public)
  -> Border#Frame (TimelineTheme.axaml, template-stable)
     -> ScrollViewer#ScrollViewer (template-stable)
        -> ItemsPresenter#ItemsPresenter (template-stable)
           -> TimelineStackPanel (internal-observable)
              -> TimelineItem (public item container)
                 -> TimelineItemPanel#RootLayout (internal-observable)
                    -> TextBlock#Label (template-stable)
                    -> TimelineIndicator#Indicator (internal-observable)
                       -> IconPresenter#PART_IconPresenter (template-stable)
                    -> ContentPresenter#ContentPresenter (template-stable)
```

### 5.2 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Timeline` | public control | `Timeline.cs` | 应用/视觉树 | Orientation、Mode、IsReverse、Pending、Items | public | 用户直接创建和配置。 |
| `TimelineItem` | public item container | `TimelineItem.cs` / `AbstractTimelineItem.cs` | Timeline ItemsControl | Label、Content、IndicatorIcon、IndicatorColor | public | 用户可显式声明，也可由 Timeline 生成。 |
| `TimelineStackPanel` | internal Panel | `TimelineTheme.axaml` | ItemsPresenter | Orientation、IsReverse、Items | internal-observable | 只用于理解主轴布局，不应由用户直接依赖。 |
| `RootLayout` | internal TimelineItemPanel | `TimelineItemTheme.axaml` | TimelineItem template | Orientation、Mode、Label、Content | internal-observable | 主题维护入口，不是 public API。 |
| `Indicator` | internal TimelineIndicator | `TimelineItemTheme.axaml` | TimelineItem template | IndicatorIcon、IndicatorColor、首尾状态 | internal-observable | 负责轴线渲染，不作为用户控件暴露。 |
| `PART_IconPresenter` | IconPresenter template part | `TimelineIndicatorTheme.axaml` | TimelineIndicator template | IndicatorIcon | template-stable | 模板替换时必须保留图标承载职责。 |
| `Label` | TextBlock theme node | `TimelineItemTheme.axaml` | TimelineItem template | Label、Orientation、Mode | template-stable | 主题可以定制视觉，不能改变状态 owner。 |
| `ContentPresenter` | ContentPresenter theme node | `TimelineItemTheme.axaml` | TimelineItem template | Content、ContentTemplate、Orientation、Mode | template-stable | 负责内容呈现和受限宽度换行。 |

## 6. 生命周期与模板接入

生命周期规则：

- 构造阶段只注册集合变化入口，不访问 template part。
- `PrepareContainerForItemOverride` 将 Orientation、Mode 和 IsReverse 从 Timeline 单向绑定到 TimelineItem。
- 容器准备、索引变化和集合 Reset 后重算视觉顺序派生状态。
- Item 的 IsVisible 或 Label 变化通过 owner 回调请求重算，不建立无释放路径的长期订阅。
- TimelineIndicator 在 `OnApplyTemplate` 获取新的 `PART_IconPresenter`；绘制只使用当前模板实例的 Bounds。
- 模板绑定由 AXAML 和 AvaloniaProperty 管理，不为固定 template part 关系创建 C# relay binding。
- Browser 和 Desktop 宿主下的主题加载顺序不得改变 Orientation、Mode 或视觉顺序语义。

稳定 template part 接入点：

- `PART_IconPresenter`：展示用户内容、文本、图标或模板化数据。

## 7. 交互与事件处理

Timeline 没有控件专属 pointer、keyboard、focus、command 或 popup 状态机。交互语义主要来自 ItemsControl 基础行为和 public 属性变化。Mode、Orientation 和 IsReverse 可以在运行时通过 binding 更新，更新必须依赖属性失效和状态重放完成，不能使用延迟刷新或强制同步。

## 8. 内部算法与关键流程

方向布局的完整公式和模式矩阵见 [Timeline 方向与布局设计](orientation-layout-design.md)。实现层必须维持以下算法边界：

- 视觉顺序只在 AbstractTimeline 中归一，不能使用包含隐藏项的 ItemCount 直接计算 Reverse 索引。
- Alternate 使用视觉索引偶数映射 Start、奇数映射 End，第一可见项始终为 Start。
- TimelineStackPanel 的有限水平宽度按 `(width - spacing * (visibleCount - 1)) / visibleCount` 分配；零项和无限宽必须显式退化。
- TimelineItemPanel 的 Measure、Arrange 和 alignment 使用同一个 Effective Mode 解析结果。
- 无 Label 且非 Alternate 的水平布局使用单侧紧凑模型；存在可见 Label 或 Alternate 时使用上区、Indicator、下区双侧模型。
- 双侧水平 Item 使用对称 side extent，TimelineStackPanel 使用所有 Item 的最大 DesiredHeight 排列，保证节点同轴。
- TimelineIndicator 按 Orientation 绘制主轴前后线段，首尾状态阻止产生悬空线段，自定义图标 Bounds 决定线段终点。

## 9. 资源、性能与 AOT 边界

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Orientation、Mode、Token 或 Gallery 示例数据。
- 不把可静态声明的 TimelineItem 模板迁移到 C# 动态创建，也不为 Horizontal 创建第二套视觉树。
- 视觉顺序重算为 O(N)，只在结构状态变化时执行，不进入 Render 热路径。
- 水平 Measure/Arrange 为 O(N)，只使用已有容器和局部尺寸值。
- TimelineIndicator 的 dot Pen 和 line Pen 与 Brush/Width 缓存键保持一致，属性变化时精准失效。
- 新增属性使用静态 AvaloniaProperty 注册和 AXAML 绑定，不引入反射、动态发现或 trimming 风险。
- Source generator 和 LLMS 生成文件不手工编辑；需要修改时更新源码、主题、Gallery 和人工维护文档源。

性能边界：

- 控件优先复用 Avalonia 属性失效、模板绑定和资源系统。
- 运行时切换 Orientation 或 Mode 不能创建新 Item、Panel、Indicator、订阅或动画对象。
- 容器重用后必须覆盖 Orientation、Mode、顺序和 Pending 派生状态，不能保留旧 Item 状态。

## 10. 维护不变量

维护 Timeline 时不得破坏：

- `TimelineMode` 精确包含 Start、End、Alternate，不保留 Left/Right 或重复值别名。
- Orientation 默认 Vertical，Mode 默认 Start。
- 第一可见 Alternate Item 为 Start，Reverse 后仍按最终视觉顺序重新从 Start 计算。
- 隐藏项不占水平槽位，不参与奇偶、首尾、Label 布局或 Pending 邻接计算。
- 水平可见 Item 等宽、节点同轴、长文本项内换行且不创建内部水平滚动。
- Vertical Start/End 遵循 FlowDirection；Horizontal Start/End 的下方/上方语义不受 RTL 交换。
- Template part 名称、ControlTheme key、伪类和资源 key。
- Item、Panel 和 Indicator 只能消费 AbstractTimeline 投影的状态，不能成为第二状态 owner。
- Light/Dark、Browser/Desktop 和运行时方向切换下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。

## 11. 测试与验证

推荐验证：

- API 测试覆盖 TimelineMode 精确值、Orientation/Mode 默认值和运行时切换。
- 状态测试覆盖 Reverse、隐藏项、Alternate 首项、首尾、Label 布局和 Pending 邻接。
- Panel 测试覆盖 Vertical 既有布局、Horizontal 等宽、Spacing、零项、无限宽和 final width 重算。
- Item 测试覆盖 Orientation x Start/End/Alternate x Label 布局矩阵和窄宽度文本换行。
- Renderer 测试覆盖垂直/水平首尾线段、内置圆点和自定义图标边界。
- Theme 与 Gallery 测试覆盖 Orientation 传递、方向 selector、三种水平示例和 approved snapshot。
- 人工走查覆盖 Light/Dark、Desktop/Browser、LTR/RTL、动态切换和有限宽度。
- Gallery NativeAOT publish 验证 AXAML 属性绑定、枚举引用和 trimming 边界。
- 所有文档和源码改动收尾运行 `git diff --check`；LLMS 产物由生成器生成并执行 verify。
