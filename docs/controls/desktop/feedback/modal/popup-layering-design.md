# Modal 内容弹层叠放设计

本文档定义 Overlay 宿主下 Dialog 内容区内弹层(popup)的宿主解析、叠放顺序与输入语义。公共契约见 [Modal 桌面版架构设计](overview.md)，状态机与宿主实现见 [Modal 桌面版实现原理](implementation.md)，宿主尺寸算法见 [Modal 宿主尺寸与 Resize 设计](host-sizing-design.md)。

## 1. 设计定位

Dialog 内容区可以放置任意控件，其中包括 ComboBox、Select、DatePicker、TimePicker、AutoComplete、Tooltip、Flyout、ContextMenu 等通过 popup 展示浮动内容的控件。本专项定义这些**内容弹层**在 Dialog 宿主层结构中的宿主位置、叠放顺序、light-dismiss 输入语义和坐标系契约。

覆盖范围：

- `DialogHostType.Overlay` 下所有宿主路径(drawn decorations Dialog host、TopLevel popup overlay layer、局部 `ScopeAwareOverlayLayer`)。
- Dialog 内容子树内直接声明或经内容控件模板间接创建的 popup。

不覆盖：

- `DialogHostType.Window` 的原生 Window 宿主。原生 Window 拥有独立 visual tree 与 TopLevel，其内容弹层由该 Window 自身的 `VisualLayerManager` 托管，不存在与所属 Dialog 的叠放问题。
- Dialog 自身的 mask 与 Surface 叠放，该关系由 Overlay presenter 主题固定，不属于内容弹层。
- Drawer 内容区的弹层。Drawer 与 Overlay Dialog 共享 drawn host 路由规则，但保留独立的 layer 与生命周期；其内容弹层遵循同一叠放模型，由 Drawer 文档单独维护。

## 2. 设计原则

- 内容弹层必须渲染在所属 Dialog 的 mask 与 Surface **之上**，并处于可命中状态；叠放顺序是契约，不允许依赖宿主层内子节点的插入顺序。
- 同一 Dialog scope 内的内容弹层相对于该 scope 的 presenter 栈恒置顶；弹层语义与 Ant Design 一致(下拉层 z 序高于 Modal)。
- 宿主解析按能力进行，不硬编码操作系统或 CSD 标志；drawn decorations、普通 TopLevel、局部 scope 三条宿主路径对内容弹层提供同一叠放语义。
- 内容弹层的 light-dismiss、输入穿透(`OverlayInputPassThroughElement` / `OverlayDismissEventPassThrough`)与定位行为，在 Dialog 内与在普通页面内保持一致。
- 不为内容弹层引入独立的弹层管理器或第二套定位算法；复用 Avalonia `PopupOverlayLayer` / `LightDismissOverlayLayer` 的祖先解析机制。

## 3. 专项模型与 Public API

### 3.1 术语

| 术语 | 定义 |
| --- | --- |
| Dialog 宿主层 | `DialogOverlayLayer` 解析出的实际父层：drawn decorations 的 `PART_DialogOverlayLayerHost`、TopLevel popup overlay layer 或局部 `ScopeAwareOverlayLayer`。 |
| Dialog 弹层作用域 | 包裹 `DialogOverlayLayer` 的 popup-capable `VisualLayerManager`，是 Dialog 内容弹层的宿主边界。 |
| 内容弹层 | Dialog 内容子树内控件打开的 popup host(`OverlayPopupHost` 或原生 `PopupRoot`)。 |

### 3.2 Public API

本专项不新增 Public API。内容控件无需感知自己是否位于 Dialog 内；`Popup.ShouldUseOverlayLayer`、light-dismiss 与定位属性的语义在 Dialog 内外保持一致。

内部契约边界：

- `VisualLayerManager.EnablePopupOverlayLayer` 是 Avalonia internal 成员，Dialog 弹层作用域通过 `AtomUI.Controls` 集中的 `VisualLayerManagerReflectionExtensions` 反射边界启用，并标注 `DynamicDependency`；控件层不新增反射入口。

## 4. 变体、平台或状态策略

内容弹层语义不随平台变化；差异只存在于 Dialog 宿主层的位置，由宿主解析能力决定：

| 宿主路径 | 触发条件 | Dialog 弹层作用域位置 | 内容弹层宿主 |
| --- | --- | --- | --- |
| drawn decorations host | AtomUI Window 暴露已附加的 `PART_DialogOverlayLayerHost`(Windows 自绘 chrome、Linux X11 自绘、Wayland 客户端装饰) | 装饰层 overlay 内，渲染在整个窗口内容之上 | 作用域自身的 popup overlay layer |
| TopLevel popup overlay layer | 无 drawn decorations host(如 macOS 原生 chrome) | 窗口 `VisualLayerManager.PopupOverlayLayer` 内 | 同上 |
| 局部 `ScopeAwareOverlayLayer` | 无可用 TopLevel popup layer 的 scope | scope layer 内 | 同上 |

三条路径下，内容弹层都解析到 Dialog 弹层作用域内的 popup overlay layer，渲染在该 scope 全部 presenter 之上。平台或装饰模式只改变作用域的挂载位置，不改变叠放契约。

`IsModal` 只控制 mask 是否绘制与阻断底层输入，不改变内容弹层的宿主与叠放。

## 5. 架构、文件结构与职责

```text
Dialog 宿主层(按能力三选一)
  -> Dialog 弹层作用域(VisualLayerManager,EnablePopupOverlayLayer)
     -> DialogOverlayLayer(Child)
        -> OverlayDialogPresenter(0..n)
     -> PopupOverlayLayer(Avalonia 惰性创建，z 高于 Child)
     -> LightDismissOverlayLayer(按需创建，z 低于 popup、高于 Child)
```

| 职责 | Owner | 说明 |
| --- | --- | --- |
| 宿主路径解析与 presenter 栈管理 | `OverlayHost/DialogOverlayLayer.cs` | 维持现有三级解析；额外负责创建并挂载弹层作用域。 |
| 内容弹层宿主 | 弹层作用域的 `PopupOverlayLayer` | 由 Avalonia `VisualLayerManager` 惰性创建与管理，AtomUI 不接管其生命周期。 |
| light-dismiss 层 | 弹层作用域的 `LightDismissOverlayLayer` | 由 Avalonia 按 popup 的 `IsLightDismissEnabled` 惰性创建。 |
| 内部能力启用反射 | `AtomUI.Controls/Primitives/VisualLayers/VisualLayerManagerReflectionExtensions.cs` | 集中反射边界，含 `DynamicDependency` 标注。 |

`OverlayDialogPresenter`、`DialogSurface` 与全部内容控件不承担弹层叠放职责，不感知弹层作用域的存在。

## 6. Template、组合与集成契约

- 弹层作用域是运行时 C# 组合节点，不出现在任何 ControlTheme 中；`OverlayDialogPresenterTheme.axaml` 的 mask/Surface 组合结构不变。
- `PART_DialogOverlayLayerHost`(drawn decorations 路径)仍是 Dialog 在装饰层 overlay 中的唯一定位点；弹层作用域作为其子节点填满宿主层。
- 弹层作用域的 `PopupOverlayLayer` 是该 scope 内所有内容弹层的统一父层；外部定制不得把 Dialog 内容弹层重定向到其他宿主层，也不应依赖 popup host 的具体父层类型。

## 7. 核心算法、数据流与生命周期

### 7.1 弹层宿主解析

内容控件打开 popup 时，Avalonia 沿 visual 祖先查找最近的 `VisualLayerManager` 并使用其 popup overlay layer;Dialog 弹层作用域作为 Dialog 内容子树的祖先被命中，内容弹层因此挂载在作用域内。查找不到祖先 `VisualLayerManager` 时的原有 TopLevel 回退逻辑不受影响。

### 7.2 坐标系

- popup 锚点由 Avalonia 按 placement target 到 `PresentationSource.RootVisual` 的变换计算，坐标系为窗口根坐标。
- 弹层作用域在其宿主层内 Stretch 填满且原点对齐宿主层原点，三条宿主路径的宿主层均与窗口根坐标对齐(drawn decorations overlay 由 TopLevelHost 以全尺寸铺于原点，`WindowVisualLayerClip` 只裁剪不平移)，因此作用域内 popup 定位无需额外坐标换算。
- 内容弹层的 flip 边界为作用域可用区域，与窗口客户区一致；Dialog 拖动、resize、maximize 改变 Surface 位置时，popup 的 transform 跟踪与位置更新沿用 popup 自身机制。

### 7.3 生命周期

- 弹层作用域随 `DialogOverlayLayer.GetOrCreate` 创建并挂载，随最后一个 presenter 移除后与 Dialog layer 一并从宿主层删除；宿主 size 与 TopLevel size 订阅的释放规则不变。
- Dialog 关闭移除 presenter 时，其内容弹层随作用域子树一并脱离 visual tree;popup 的 placement target detach 订阅负责 popup 自身关闭，不需要 Dialog 额外干预。
- 弹层作用域不持有 presenter、popup host 或内容控件的引用，不引入新的释放点。

## 8. 资源、性能与 AOT 边界

- 弹层作用域的 popup overlay layer 与 light-dismiss 层均由 Avalonia 惰性创建；无内容弹层打开时只存在一个空 `VisualLayerManager` 节点，不产生 measure/render 热路径分配。
- `EnablePopupOverlayLayer` 的启用通过集中反射一次性完成，带 `DynamicDependency` 标注，保持 NativeAOT 与 trimming 安全；不在拖动、resize 等热路径反射。
- 不引入新的 binding、timer、事件订阅或 Dispatcher 调度。

## 9. 兼容性与定制边界

- 无 Public API、默认值、Template Part、ControlTheme key 或 Token 变化。
- 行为契约：Dialog 内容区 popup 在所有 Overlay 宿主路径下可见、可命中、可 light-dismiss。
- 用户不应依赖内容弹层 popup host 的具体父层类型或名称；唯一稳定语义是“弹层位于所属 Dialog 之上”。
- Drawer 与 Dialog 各自维护独立的弹层作用域，不共享 layer 与生命周期。

## 10. 验证要求

- 结构断言：在 drawn decorations host 与 TopLevel popup layer 两条路径下，断言 Dialog 内容 ComboBox 的 popup host 位于 Dialog 弹层作用域内，且渲染顺序高于该 scope 的 presenter(headless 测试)。
- 输入语义：内容弹层 light-dismiss 在 Dialog 内生效；`OverlayInputPassThroughElement` 穿透目标仍为弹层锚点控件；mask 的 `IsMaskClosable` 门控不受内容弹层开关影响。
- Demo 走查：`tools/DialogComboBoxPopupDemo` 的实验 1(macOS 退化路径)保持正常；实验 2(模拟 drawn decorations 层级)中下拉应可见可点。
- 平台实机：Windows 自绘 chrome、Linux X11、Wayland GNOME(客户端装饰)验证 Dialog 内 ComboBox 下拉；macOS 验证无回归。
- 回归：多 Dialog 叠放、Dialog 拖动/resize/maximize 过程中弹层位置跟踪、Dialog 关闭时弹层一并释放。
- AOT：涉及反射边界的改动执行 Gallery NativeAOT publish 验证。
