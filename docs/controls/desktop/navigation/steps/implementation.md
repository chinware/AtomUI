# Steps 桌面版实现原理

本文档描述 Steps 桌面版的单一状态投影、容器生命周期、统一语义模板、布局 Panel、受控交互、Wave 和 Progress 实现边界。公共设计与 API 契约见 [Steps 桌面版架构设计](overview.md)，Token 语义见 [Steps Token 设计](token.md)，变化记录见 [Steps Changelog](changelog.md)。

## 1. 实现定位

Steps 的实现目标是在 `ItemsControl` 容器体系内，把根输入和 item 显式状态确定性投影为视觉状态。实现不依赖 Selection、模板应用顺序、VisualTree attach 顺序或上一次计算结果。

本文档覆盖 `Steps`、`StepsItem`、`StepsItemIndicator`、两个 internal LayoutPanel 和三个主题文件的稳定职责。通用 ItemsControl、TokenResource、Motion 和 PathIcon 实现不在本文档重复说明。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/Steps/Steps.cs`：public API、事件、容器生成、根输入分发和 item 状态协调。
- `src/AtomUI.Desktop.Controls/Steps/StepsItem.cs`：public item 契约、internal 派生状态、owner 生命周期和激活入口。
- `src/AtomUI.Desktop.Controls/Steps/StepsItemIndicator.cs`：Indicator 状态、Wave part、Progress 绘制和渲染失效。
- `src/AtomUI.Desktop.Controls/Steps/StepsPanel.cs`：item 间水平 flex、Navigation 等宽、Inline 和垂直 stack 布局。
- `src/AtomUI.Desktop.Controls/Steps/StepsItemLayoutPanel.cs`：Indicator、Header、SubHeader、Connector、Content、NavigationArrow 和 NavigationActiveIndicator 的 item 内布局。
- `src/AtomUI.Desktop.Controls/Steps/StepsToken.cs`：Steps 组件 Token。
- `src/AtomUI.Desktop.Controls/Steps/Themes/StepsTheme.axaml`：根模板和 StepsPanel。
- `src/AtomUI.Desktop.Controls/Steps/Themes/StepsItemTheme.axaml`：统一 item 语义模板和状态样式。
- `src/AtomUI.Desktop.Controls/Steps/Themes/StepsItemIndicatorTheme.axaml`：统一 Indicator、Dot、Icon、Progress 和 Wave 模板。
- `src/AtomUI.Desktop.Controls/Steps/Themes/StepsThemes.axaml`：Steps 主题聚合入口。

测试目录：

- `tests/AtomUI.Desktop.Controls.Tests/Steps`：状态、Items、交互、Wave、Progress、布局和生命周期回归测试。

Gallery 目录：

- `controlgallery/AtomUIGallery/ShowCases/Navigation/Steps`：示例、API 表、Token 表和本地化资源。

## 3. 核心类职责

### 3.1 Steps

`Steps : ItemsControl` 是状态协调 owner：

- 保存根 public 输入。
- 创建和清理 `StepsItem` 容器。
- 根据 index 计算 StepNumber 和 AutomaticStatus。
- 维护相邻 item 的 ConnectorStatus。
- 接收 item 激活并发出 `CurrentChangeRequested`。
- 不保存第二套当前步骤状态、根级页面内容投影或视觉缓存集合。

### 3.2 StepsItem

`StepsItem : HeaderedContentControl` 是单项 public 容器：

- 承载 Header、SubHeader、Content、Icon 和 nullable Status。
- 保存 StepNumber、IsCurrent、AutomaticStatus、EffectiveStatus、ConnectorStatus、IsFirst、IsLast 和 CanInvoke 的 internal 投影。
- 处理 pointer、keyboard、focus 和 hover。
- 把激活请求交给 owner，不写入根 Current。

### 3.3 StepsItemIndicator

Indicator 是 internal-observable 视觉控件：

- 展示步骤编号、完成图标、错误图标、Dot 或自定义 Icon。
- 在有效条件成立时绘制 Progress ring。
- 持有当前模板中的 `PART_WaveSpirit`，并只响应 item 的真实 pointer click 调用。
- 不监听 IsCurrent 变化播放 Wave。

### 3.4 LayoutPanel

- `StepsPanel` 只排列 StepsItem，不读取 Status、不生成视觉。
- `StepsItemLayoutPanel` 只排列固定语义子节点，不读取 Current、不修改 item 属性。

两个 Panel 都通过 `AffectsMeasure` / `AffectsArrange` 响应相关布局属性，不依赖根控件手工重建 Grid definitions。

## 4. 状态与数据流

### 4.1 根输入

```text
Current
Initial
Status
Percent
Type
Orientation
TitlePlacement
SizeType
IsItemClickable
IsMotionEnabled
```

状态输入和展示输入分开处理。只有 Current、Initial、根 Status、item Status 和 item index 参与 EffectiveStatus 计算。

### 4.2 Item 投影

```text
StepNumber = Initial + index

AutomaticStatus =
    StepNumber == Current ? root Status :
    StepNumber < Current  ? Finish :
                            Wait

EffectiveStatus = item Status ?? AutomaticStatus
IsCurrent       = StepNumber == Current
```

每次协调完整写入所有派生值，不保留依赖旧状态的分支。公开 `Status` 从不被根控件覆盖；根控件只写 internal AutomaticStatus。

### 4.3 Connector 投影

item `i` 的 ConnectorStatus 等于 item `i + 1` 的 EffectiveStatus。最后一个 item 通过 IsLast 隐藏 Connector。

根输入变化时线性刷新全部已实现容器。单个 item Status 变化时只刷新自身 EffectiveStatus 和前一个 item 的 ConnectorStatus。

### 4.4 Progress 投影

```text
IsProgressVisible =
    Percent.HasValue
    && IsCurrent
    && EffectiveStatus == Process
    && Icon == null
    && Type is Default or Navigation
```

Icon、Type、Percent、IsCurrent 或 EffectiveStatus 变化都必须重新计算 IsProgressVisible。

### 4.5 数据项路径

- `item is StepsItem`：直接使用，不创建包装容器。
- 普通数据项：创建 StepsItem，把数据项写入 Content，把 Steps.ItemTemplate 写入 ContentTemplate。
- 数据源 ItemTemplate 负责渲染完整文字区域；Indicator 和 Connector 仍由容器主题管理。

## 5. 组合结构模型

### 5.1 控件角色图

```text
Steps (public)
└── ItemsPresenter#PART_ItemsPresenter (template-stable)
    └── StepsPanel (internal-observable)
        └── StepsItem (public container)
            └── StepsItemLayoutPanel (internal-observable)
                ├── StepsItemIndicator#PART_Indicator (template-stable)
                │   └── WaveSpiritDecorator#PART_WaveSpirit (internal-observable)
                ├── ContentPresenter#HeaderPresenter (internal-observable)
                ├── ContentPresenter#SubHeaderPresenter (internal-observable)
                ├── PixelAlignedBorder#Connector (internal-observable)
                ├── ContentPresenter#ContentPresenter (internal-observable)
                ├── PathIcon#NavigationArrow (internal-observable)
                └── PixelAlignedBorder#NavigationActiveIndicator (internal-observable)
```

### 5.2 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Steps` | public control | `Steps.cs` / `StepsTheme.axaml` | application visual tree | 全部根 API | public | 用户可直接使用。 |
| `StepsItem` | public container | `StepsItem.cs` / `StepsItemTheme.axaml` | Steps container lifecycle | item API | public | 用户可直接声明。 |
| `StepsPanel` | layout panel | `StepsTheme.axaml` | ItemsPresenter | Type、Orientation | internal-observable | 只用于理解布局，不作为用户 API。 |
| `StepsItemLayoutPanel` | layout panel | `StepsItemTheme.axaml` | StepsItem template | Type、Orientation、TitlePlacement | internal-observable | 不直接依赖或替换。 |
| `PART_Indicator` | indicator | `StepsItemTheme.axaml` | StepsItem template | Icon、Status、Percent、Wave | template-stable | 自定义主题必须保留。 |
| `PART_WaveSpirit` | wave decorator | `StepsItemIndicatorTheme.axaml` | Indicator template | IsMotionEnabled、pointer click | internal-observable | 不由用户直接调用。 |
| `Connector` | border | `StepsItemTheme.axaml` | StepsItem template | ConnectorStatus、Type | internal-observable | ConnectorStatus 来自 next item EffectiveStatus。 |
| `NavigationArrow` | path icon | `StepsItemTheme.axaml` | StepsItem template | Type、Orientation、SizeType | internal-observable | 只在 Navigation 类型可见。 |
| `NavigationActiveIndicator` | border | `StepsItemTheme.axaml` | StepsItem template | Type、Orientation、IsCurrent | internal-observable | 只在 Navigation 当前项可见。 |

## 6. 生命周期与模板接入

### 6.1 容器准备

准备容器时：

1. 设置 `Owner` 和当前 index。
2. 计算 StepNumber、IsFirst、IsLast、AutomaticStatus、EffectiveStatus 和 IsCurrent。
3. 更新前一个 item 的 ConnectorStatus。
4. 普通数据项接入 Content / ContentTemplate。

初始实现每个容器只执行 O(1) 初始化，不在每次 ContainerPrepared 时遍历全部 Items。

### 6.2 Index 和集合变化

- Add/Remove/Move：重新编号受影响区间并更新边界 Connector。
- Replace：清理旧容器后准备新容器。
- Reset：按当前 public 输入完整重建已实现容器投影。
- Current、Initial 或根 Status 变化：O(n) 刷新已实现容器。

### 6.3 容器清理

清理容器时必须：

- `Owner = null`。
- `ItemIndex = -1`。
- 清除 owner 写入的 internal 派生值和数据项 Content 映射。
- 让 Indicator visual detach；WaveSpiritDecorator 在 detach 中取消自身 animation owner。

外部继续持有被移除 StepsItem 时，它不能保留旧 Steps owner。

### 6.4 模板接入

`Steps` 不重写 `OnApplyTemplate()`；根模板通过 `ItemsPresenter.ItemsPanel` 直接接入 `StepsPanel`，模板生命周期不修改 Current、Initial 或 item 状态。

`StepsItem.OnApplyTemplate()` 和 `StepsItemIndicator.OnApplyTemplate()` 在获取新 part 前先清空旧引用。AXAML Ancestor Binding 的建立和释放由 Avalonia template 生命周期管理。

不创建根级步骤页面内容 observable、长期 Relay Binding、全局事件订阅或 detach 后仍存活的 CompositeDisposable。

## 7. 交互与事件处理

### 7.1 Pointer

- 整个 StepsItem 是命中区域。
- 同一 item 内完成 press/release 才形成 click。
- 移出释放、失去 capture 或取消不触发。
- Clickable、root enabled 和 item enabled 共同决定 CanInvoke。

pointer click 首先调用 Indicator.PlayWave，再在目标 StepNumber 不等于 Current 时发出 CurrentChangeRequested。事件处理器是否更新 Current 不影响本次 click Wave。

### 7.2 Keyboard

- CanInvoke item 可获得焦点。
- Enter/Space 在目标不是当前 item 时发出 CurrentChangeRequested。
- keyboard 路径不调用 Indicator.PlayWave。
- Space 被标记 handled，避免宿主滚动。

### 7.3 Focus、Hover 和 Disabled

CanInvoke=false 时不进入 Tab 焦点序列，不显示 hand cursor 和 clickable hover 视觉。视觉使用标准 focus-visible、pointerover 和 disabled 伪类，不创建 Selection 伪类。

## 8. 内部算法与关键流程

### 8.1 根状态刷新

根状态刷新是确定性 O(n) 投影。它不检查 IsLoaded、VisualTree attachment 或 template part 是否存在，也不使用 suppression flag、Dispatcher 延迟或强制刷新。

### 8.2 布局算法

`StepsPanel`：

- Horizontal Default/Dot：非末 item 参与伸展，末 item 使用内容宽度。
- Horizontal Navigation：item 等宽。
- Inline：按紧凑 inline 规则排列。
- Vertical：按 DesiredSize 顺序堆叠。

`StepsItemLayoutPanel` 根据 Type、Orientation 和 EffectiveTitlePlacement 排列固定语义节点。Connector 的方向和伸展范围由布局 Panel 决定，状态由 item 投影决定。

### 8.3 Indicator 和 Progress

Indicator 使用单一模板切换 number、finish mark、error mark、dot 和 custom icon。Progress ring 在 Indicator.Render 中绘制。

Progress 外径由 `IconSize` / `IconSizeSM` 与 `ProgressFramePadding` / `ProgressFramePaddingSM` 推导，不保存重复的固定 Progress size。

Percent coercion：

```text
null             -> null
NaN / Infinity   -> null
value < 0        -> 0
value > 100      -> 100
其他             -> value
```

所有 Render 输入进入 AffectsRender；只有影响尺寸的属性进入 AffectsMeasure。Progress Pen 使用可复用实例，避免 render 热路径分配。

## 9. 资源、性能与 AOT 边界

资源边界：

- TokenResource 和 SharedToken 只提供视觉值，不保存实例状态。
- 根展示属性使用 AXAML Ancestor Binding 投影到 item 和 internal panel。
- 固定模板关系使用 TemplateBinding、Ancestor Binding 和 selector，不使用字符串路径反射。

性能边界：

- 单容器准备 O(1)。
- 单 item Status 变化 O(1)。
- 根状态变化和 Reset O(n)。
- 不维护第二份 item 列表、状态字典或延迟更新队列。
- 两个 Panel 在 Measure/Arrange 中不创建视觉，不修改 public 状态。

AOT 边界：

- 不新增运行时反射扫描、动态类型注册或编译期不可分析的 binding 路径。
- 不通过反射访问 Wave 播放状态；测试使用 internal 可观察入口或渲染结果。
- 新 internal panel 由静态 AXAML 和显式类型引用创建。

## 10. 维护不变量

- Current 是唯一当前步骤输入；不得引入第二套选择状态或双向同步。
- 每次状态协调必须完整覆盖派生状态，不依赖旧值。
- item public Status 不被根控件写入或覆盖。
- EffectiveStatus 是所有状态视觉的唯一输入。
- Connector 使用 next item EffectiveStatus。
- Initial 不在 OnApplyTemplate 或 attach 中写入 Current。
- 根级步骤页面内容投影和内容订阅不得重新引入。
- pointer click 是 Wave 的唯一触发源；Current 变化不能播放 Wave。
- 每个主题只维护一套语义模板。
- StepsPanel 和 StepsItemLayoutPanel 只负责布局。
- 容器清理必须释放 Owner，模板重套必须释放旧 part 引用。
- Percent、Icon、Type 和 EffectiveStatus 运行时变化必须立即更新 Progress。

## 11. 测试与验证

状态：

- Current 正向、反向、重复、低于 Initial 和高于所有 item。
- Initial 非零以及 Indicator 编号。
- 根 Status 运行时变化。
- item nullable Status 的 Wait、Process、Finish、Error 覆盖。
- Add、Remove、Replace、Move、Reset。
- 属性在加载前、加载后和模板重套后设置。

交互与 Wave：

- Indicator、Header、SubHeader 和 Content click。
- 非当前 item click 产生 Wave 和 request。
- 当前 item click 只有 Wave。
- 程序化 Current 不产生 Wave。
- pointer cancel、root/item disabled、不可点击和 motion disabled。
- Enter/Space 产生 request 但不产生 Wave。

Progress：

- null、0、100、越界、NaN 和 Infinity。
- 四种 EffectiveStatus、自定义 Icon、Dot 和 Inline。
- Render 输入变化触发 InvalidateVisual。

布局：

- `Type x Orientation x TitlePlacement x SizeType`。
- 语义节点唯一、Bounds 有效、无重叠、Connector 正确。
- 运行时布局切换和动态 Items。

生命周期：

- 移除 item 后 Owner 释放。
- 模板重套不改变 Current。
- 旧 Indicator/Wave part 不被保留。
- detach/reattach 和数据容器回收不保留旧状态。

收尾运行 Steps 定向测试、相邻 Desktop Controls 测试、Gallery 测试和 `git diff --check`。
