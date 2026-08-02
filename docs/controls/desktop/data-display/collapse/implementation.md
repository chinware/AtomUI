# Collapse 桌面版实现原理

本文档描述 Collapse 桌面版的内部实现范围、源码职责、状态流、生命周期、资源边界和维护规则。公共设计与 API 契约见 [Collapse 桌面版架构设计](overview.md)，变化记录见 [Collapse Changelog](changelog.md)。涉及控件 Token 的实现应同时阅读 [Collapse Token 设计](token.md)。

## 1. 实现定位

本文档覆盖 Collapse 的控件实现、主题接入、状态同步和 Gallery 可见维护边界。具体属性注册、默认值、绘制细节和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构和不变量。

## 2. 源码文件结构

主要源码文件：

- `src/AtomUI.Desktop.Controls/Collapse/Collapse.cs`
- `src/AtomUI.Desktop.Controls/Collapse/CollapseItem.cs`
- `src/AtomUI.Desktop.Controls/Collapse/CollapseToken.cs`
- `src/AtomUI.Desktop.Controls/Collapse/ICollapseItemData.cs`
- `src/AtomUI.Desktop.Controls/Collapse/Themes/CollapseItemTheme.axaml`
- `src/AtomUI.Desktop.Controls/Collapse/Themes/CollapseTheme.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 3. 核心类职责

- `Collapse`：维护 items、Avalonia selection model、selection mode、输入路由、容器生成、模式切换归一和 item 位置投影。
- `CollapseItem`：承载单项 public 内容与 `IsSelected` 投影，处理 header 命中、展开按钮和 content motion 生命周期。
- `CollapseToken`：控件 Token scope，负责从全局 token 派生控件语义变量。

核心协作规则：

- Avalonia selection model 是展开状态的唯一 owner；`CollapseItem.IsSelected` 是容器投影，不存在内部 active-key 镜像。
- Template part 是视觉协作对象，生命周期必须受 `OnApplyTemplate` 或模板加载流程管理。
- 数据对象、选项对象、任务对象或节点对象只保存业务数据，不应反向持有不可释放的视觉对象。
- 弹层、窗口、计时器、异步 loader 和全局管理器必须有明确关闭、解绑或释放路径。

## 4. 状态与数据流

Collapse 的展开状态流遵循下面路径：

```text
Pointer / keyboard / CollapseItem.IsSelected / inherited selection API
  -> SelectingItemsControl selection model
  -> CollapseItem.IsSelected
  -> content visibility + arrow direction + motion target
```

源码中的状态入口按以下语义维护：

- 内容与数据：`AddOnContent`、`AddOnContentTemplate`、`ContentPadding`、`ExpandIcon`、`ExpandIconPosition`、`HeaderPadding`、`IsShowExpandIcon`、`ItemContentPadding`、`ItemHeaderPadding`。
- 选择与集合：`IsSelected`。
- 交互与状态：`IsAccordion`、`IsBorderless`、`IsGhostStyle`、`IsMotionEnabled`。
- 视觉与布局：`SizeType`。
- 其他稳定入口：`TriggerType`。

普通模式将 `SelectionMode` 设为 `Multiple | Toggle`。手风琴模式将其设为 `Single | Toggle`，利用原生 selection 操作完成旧项关闭、目标项打开和当前项收起。模式从普通切换到手风琴时，只保留视觉索引最小的已展开项。

禁止在 `SelectionChanged` 中遍历容器并回写 `IsSelected`，也禁止保存初始 active item 或单独的 active-key 集合。`ItemsSource` replace 会清除被替换索引的选择并让替换容器从收起状态开始，reset 和 clear 会清空 owner selection；这些结果由 selection model 与容器生成生命周期共同处理。

### 4.1 组合结构模型

```text
Collapse
└── PixelAlignedBorder#PART_Frame
    └── ItemsPresenter#PART_ItemsPresenter
        └── CollapseItem
            └── item shell border
                └── DockPanel#PART_MainLayout
                    ├── PixelAlignedBorder#PART_HeaderDecorator
                    │   ├── IconButton#PART_ExpandButton
                    │   ├── ContentPresenter#PART_HeaderPresenter
                    │   └── ContentPresenter#PART_AddOnContentPresenter
                    └── LayoutAwareMotionActor#PART_ContentMotionActor
                        └── PixelAlignedBorder#PART_ContentFrame
                            └── ContentPresenter#PART_ContentPresenter
```

边框职责固定为：`PART_Frame` 绘制外框；item shell 绘制非末项底线；`PART_ContentFrame` 在默认 bordered 模式绘制内容顶线。Header 不承担 item 分隔线，动效状态不参与边框计算。

## 5. 生命周期与模板接入

生命周期规则：

- 构造阶段只注册必要状态，不依赖 template part。
- 模板应用时获取 part、建立事件订阅和绑定，并先释放旧 part 订阅。
- 控件卸载、弹层关闭、窗口关闭、集合替换或 container recycle 时释放事件订阅和资源宿主。
- DynamicResource、TokenResourceBinder 或 C# binding 必须有明确 owner 和释放点。
- Browser 和 Desktop 宿主下的主题加载顺序不得影响 public API 语义。

稳定 template part 接入点：

- `PART_AddOnContentPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_ContentMotionActor`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_ContentPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_ExpandButton`：承载用户触发入口、导航或关闭动作。
- `PART_Frame`：承载根视觉、边框、背景或尺寸基线。
- `PART_HeaderDecorator`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_HeaderPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_ItemsPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_MainLayout`：稳定模板协作入口，重命名前必须同步主题和实现。

## 6. 交互与事件处理

Header 模式下，header 区域和展开图标触发 selection；Icon 模式下只有展开图标触发 selection。Enter/Space 和方向键导航复用 `SelectingItemsControl` 的 selection 入口。Disabled item 不进入 selection 操作。

输入处理器只识别目标容器和合法触发区域，不能直接修改其他 item 的 `IsSelected`。普通/手风琴差异由 selection mode 表达，不在 pointer、keyboard 或 button handler 中复制两套算法。

## 7. 内部算法与关键流程

关键流程：

1. `IsAccordion` 变化时先切换 selection mode，再通过 selection model 归一非法多选状态；容器准备期间若新容器携带显式选中值，则在基础 selection 投影后保留已有的较小已选索引。
2. 容器 prepare 和 index change 只投影 owner 属性、padding 和“是否末项”结构状态，不根据 selection 重算边框。
3. `CollapseItem.IsSelected` 变化只改变内容目标可见性和箭头方向。
4. 新 content motion 开始前取消旧 motion；完成时仅在目标仍与最新 `IsSelected` 一致时应用稳定状态。
5. content 顶边是 content frame 的固定视觉，收起时随 content 一起被裁剪，不需要父控件等待动画完成。

## 8. 资源、性能与 AOT 边界

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 单项展开不得遍历全部容器重算边框。
- 模式切换只处理当前 selection indexes；item 位置只在集合结构或容器索引变化时更新。
- 边框、背景、padding 和 icon placement 使用 AXAML、selector 和 template binding 表达。
- 不新增运行时反射、字符串 binding、全局事件、timer、active-key 缓存或长期状态对象。
- template reapply 必须解绑旧展开按钮；detach 和新动效开始前必须取消旧 content motion。

## 9. 维护不变量

维护 Collapse 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- Selection model 是唯一展开状态 owner，不能增加 active-key 镜像或 `SelectionChanged` 回写循环。
- 手风琴模式最多展开一项，并允许点击当前项后全部收起。
- 分隔线只由 item 位置、视觉模式和固定模板结构决定，不能依赖 selection 或 motion 时序。
- 旧 template part、事件订阅和 content motion cancellation 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。

## 10. 测试与验证

推荐验证：

- 纯文档改动运行 `git diff --check` 并检查相对链接。
- 状态测试覆盖普通多开、手风琴切换、点击当前项收起、初始多选归一、运行时模式切换和 items reset/replace/clear。
- 输入测试覆盖 Header、Icon、keyboard 和 disabled。
- 视觉测试覆盖默认、Borderless、Ghost 的首项、中间项、末项以及展开/收起/反向动画过程。
- 生命周期测试覆盖 template reapply、旧按钮解绑、detach 和 motion cancellation。
- 运行 Collapse 定向测试、Gallery 测试和 `git diff --check`。
