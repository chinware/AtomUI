# Menu 弹层滚动模式设计

本文档定义 Menu 家族弹层内容的滚动模式、公共契约、模板组合和维护边界。整体架构见 [Menu 桌面版架构设计](overview.md)，内部实现原理见 [Menu 桌面版实现原理](implementation.md)。

## 1. 设计定位

Menu 家族的弹层滚动模式用于统一控制 `Menu` 顶层菜单弹层、`MenuItem` 子菜单弹层、`ContextMenu`、`MenuFlyout` 和 `MenuFlyoutPresenter` 的内容承载方式。该模式只控制弹层内容区域是否由 `ScrollViewer` 管理，不改变菜单项生成、hover intent、keyboard navigation、checked state、命令触发或 popup placement 语义。

`DisplayPageSize` 和 `IsScrollEnabled` 共同决定弹层内容高度策略：

- `IsScrollEnabled=true` 时，弹层内容由 `ScrollViewer` 承载，`DisplayPageSize` 用于计算弹层最大高度。
- `IsScrollEnabled=false` 时，弹层内容直接显示全部菜单项，不创建 `ScrollViewer`，也不使用 `DisplayPageSize` 限制弹层高度。

`NavMenu` 是导航菜单控件，不属于本文档定义的 Menu 弹层滚动模式。

## 2. 设计原则

- 默认行为保持滚动开启，现有 Menu、ContextMenu、MenuFlyout 和子菜单在未设置新属性时仍使用滚动弹层。
- 禁用滚动的语义是视觉树中不存在 `ScrollViewer`，不是隐藏滚动条、禁用滚动条或设置 `ScrollBarVisibility=Disabled`。
- 滚动模式沿 Menu 层级向子菜单继承，子 `MenuItem` 可以通过本地值覆盖父级配置。
- `DisplayPageSize` 只在滚动开启时参与最大高度计算；滚动禁用时弹层高度交给内容和外部布局约束决定。
- 模板分支集中在一个内部弹层内容宿主中，避免四套弹层模板复制同一段 `ScrollViewer`/`ItemsPresenter` 分支。
- 滚动模式是实例运行时状态，不进入 `MenuToken`，也不通过 DynamicResource 或主题 Token 表达。
- 模板结构优先使用 AXAML、`TemplateBinding` 和继承属性，不使用 C# `EnsureScrollViewer`/`ClearScrollViewer` 动态创建视觉树。

## 3. 专项模型与 Public API

### 3.1 通用滚动契约

滚动开关使用共享控件契约表达：

```csharp
public interface IScrollAwareControl
{
    bool IsScrollEnabled { get; set; }
}

public abstract class ScrollAwareControlProperty
{
    public const string IsScrollEnabledPropertyName = "IsScrollEnabled";

    public static readonly StyledProperty<bool> IsScrollEnabledProperty =
        AvaloniaProperty.Register<StyledElement, bool>(
            IsScrollEnabledPropertyName,
            defaultValue: true,
            inherits: true);
}
```

`IsScrollEnabled` 的默认值为 `true`。该默认值属于兼容性契约，不能在没有迁移说明和 Gallery 验证的情况下修改。

### 3.2 Menu 家族接入点

以下类型暴露或承载 `IsScrollEnabled`：

| 类型 | 职责 | 接入方式 |
| --- | --- | --- |
| `Menu` | 顶层菜单 owner。 | `ScrollAwareControlProperty.IsScrollEnabledProperty.AddOwner<Menu>()`，并公开 CLR wrapper。 |
| `ContextMenu` | 上下文菜单 owner 和 root popup content owner。 | `AddOwner<ContextMenu>()`，并公开 CLR wrapper。 |
| `MenuItem` | 子菜单 owner 和递归菜单项容器。 | `AddOwner<MenuItem>()`，并公开 CLR wrapper。 |
| `MenuFlyout` | 用户直接配置的 flyout 菜单入口。 | `AddOwner<MenuFlyout>()`，并公开 CLR wrapper，中继到 presenter。 |
| `MenuFlyoutPresenter` | `MenuFlyout` 的实际菜单项 presenter 和模板 owner。 | `AddOwner<MenuFlyoutPresenter>()`，并公开 CLR wrapper。 |
| `MenuPopupScrollHost` | 内部模板宿主。 | internal 控件，仅承载模板分支，不作为用户 API。 |

`MenuFlyout` 与 `MenuFlyoutPresenter` 的生命周期不同，`MenuFlyout` 不在菜单项逻辑树中承载弹层模板，因此 `MenuFlyout` 的 `IsScrollEnabled` 需要在 `CreatePresenter()` 中中继给 `MenuFlyoutPresenter`。这个中继与现有 `ItemTemplate`、`IsMotionEnabled`、`ShouldUseOverlayPopup` 等 presenter 绑定属于同一类 owner-to-presenter 配置。

## 4. 状态策略

滚动模式按以下矩阵解析：

| Owner | 子项默认值来源 | 本地覆盖 | `DisplayPageSize` 生效条件 |
| --- | --- | --- | --- |
| `Menu` | 属性默认值 `true` 或调用方设置。 | 直接设置 `MenuItem.IsScrollEnabled`。 | 当前弹层 owner 的 `IsScrollEnabled=true`。 |
| `ContextMenu` | 属性默认值 `true` 或调用方设置。 | 直接设置 `MenuItem.IsScrollEnabled`。 | 当前弹层 owner 的 `IsScrollEnabled=true`。 |
| `MenuItem` | 继承父 `Menu`、`ContextMenu` 或父 `MenuItem`。 | 当前 `MenuItem` 本地值覆盖继承值，并继续影响其子菜单。 | 当前 `MenuItem.IsScrollEnabled=true`。 |
| `MenuFlyout` | 属性默认值 `true` 或调用方设置。 | presenter 接收 `MenuFlyout` 的配置；菜单项仍可本地覆盖。 | `MenuFlyoutPresenter.IsScrollEnabled=true`。 |

`IsScrollEnabled=false` 不关闭 popup，不改变 popup placement，也不改变菜单项交互状态。它只改变弹层内容区域的承载结构和高度上限策略。

## 5. 架构、文件结构与职责

稳定职责边界如下：

| 文件或类型 | 职责 |
| --- | --- |
| `src/AtomUI.Controls.Shared/IScrollAware.cs` | 定义通用滚动开关接口和可继承 styled property。 |
| `src/AtomUI.Desktop.Controls/Menu/Menu.cs` | 暴露 `Menu.IsScrollEnabled`，依赖继承属性向顶层 `MenuItem` 传递滚动模式。 |
| `src/AtomUI.Desktop.Controls/Menu/ContextMenu.cs` | 暴露 `ContextMenu.IsScrollEnabled`，根据滚动模式计算 root popup 最大高度。 |
| `src/AtomUI.Desktop.Controls/Menu/MenuItem.cs` | 暴露 `MenuItem.IsScrollEnabled`，根据滚动模式计算子菜单最大高度。 |
| `src/AtomUI.Desktop.Controls/Flyouts/MenuFlyout.cs` | 暴露用户配置入口，并把滚动模式中继给 `MenuFlyoutPresenter`。 |
| `src/AtomUI.Desktop.Controls/Flyouts/MenuFlyoutPresenter.cs` | 承载 `MenuFlyout` 菜单项模板，根据滚动模式计算 presenter 最大高度。 |
| `src/AtomUI.Desktop.Controls/Menu/MenuPopupScrollHost.cs` | internal 模板宿主，集中表达有滚动和无滚动两种视觉分支。 |

`MenuPopupScrollHost` 不负责菜单项生成、选择状态、popup 打开关闭或最大高度计算。它只根据 `IsScrollEnabled` 选择是否创建 `ScrollViewer`。

## 6. Template、组合与集成契约

弹层内容组合结构：

```text
ContextMenu
  -> Border#Frame
     -> MenuPopupScrollHost
        -> ScrollViewer when IsScrollEnabled=true
           -> ItemsPresenter#PART_ItemsPresenter
        -> ItemsPresenter#PART_ItemsPresenter when IsScrollEnabled=false

MenuItem submenu / TopLevelMenuItem
  -> Popup#PART_Popup
     -> Border#PopupFrame
        -> MenuPopupScrollHost
           -> ScrollViewer when IsScrollEnabled=true
              -> ItemsPresenter#PART_ItemsPresenter
           -> ItemsPresenter#PART_ItemsPresenter when IsScrollEnabled=false

MenuFlyoutPresenter
  -> ArrowDecoratedBox#PART_ArrowDecorator
     -> MenuPopupScrollHost
        -> ScrollViewer when IsScrollEnabled=true
           -> ItemsPresenter#PART_ItemsPresenter
        -> ItemsPresenter#PART_ItemsPresenter when IsScrollEnabled=false
```

主题接入规则：

- `PART_ItemsPresenter` 的名称和职责保持稳定，不因是否滚动而改变。
- `ItemsPresenter` 必须由外层 Menu/ContextMenu/MenuFlyoutPresenter 模板创建并作为宿主内容传入，`MenuPopupScrollHost` 模板不得创建第二个 `ItemsPresenter`。
- `MenuPopupScrollHost` 的有滚动模板必须继续传递 `IsMotionEnabled`、`ScrollViewer.IsLiteMode`、`ScrollViewer.AllowAutoHide` 和 `IsScrollChainingEnabled=False`。
- 无滚动模板直接承载同一个 `ItemsPresenter` 内容，不添加禁用状态的 `ScrollViewer`。
- `ContextMenuTheme.axaml`、`MenuItemTheme.axaml`、`TopLevelMenuItemTheme.axaml` 和 `MenuFlyoutPresenterTheme.axaml` 复用同一个内部宿主，不复制完整模板分支。

## 7. 核心算法、数据流与生命周期

最大高度计算使用当前弹层 owner 的 item height、page size 和 popup padding：

```csharp
MaxPopupHeight = IsScrollEnabled
    ? ItemHeight * DisplayPageSize + verticalPadding
    : double.PositiveInfinity;
```

其中：

- `ItemHeight` 来自 Menu Token。
- `DisplayPageSize` 来自当前 owner 的 public API。
- `verticalPadding` 对 `ContextMenu` 使用 `Padding.Top + Padding.Bottom`，对子菜单和 `MenuFlyoutPresenter` 使用其 popup content padding。
- `double.PositiveInfinity` 表示 AtomUI 不再主动限制弹层内容高度；最终裁剪或屏幕约束由 Avalonia popup host、窗口边界和外部布局决定。

重新计算触发条件：

- `IsScrollEnabled` 变化。
- `DisplayPageSize` 变化。
- `ItemHeight` 变化。
- popup padding 变化。
- 模板重新应用后需要回放当前高度策略。

滚动模式传播优先使用继承 styled property。`Menu`、`ContextMenu` 和 `MenuItem` 之间不需要为 `IsScrollEnabled` 增加逐项 `RelayBind`。`MenuFlyout` 到 `MenuFlyoutPresenter` 是独立 owner 到 presenter 的生命周期中继，需要与现有 presenter binding 一起释放。

## 8. 资源、性能与 AOT 边界

该设计不引入运行时反射、assembly 扫描、动态发现或 source generator 变更。AXAML 模板分支和 `TemplateBinding` 对 NativeAOT 友好。

性能边界：

- 默认滚动路径比原模板多一个轻量 internal 宿主节点，用来集中管理模板分支。
- `IsScrollEnabled=false` 会移除 `ScrollViewer` 子树，但所有菜单项都会直接参与显示和测量。禁用滚动的大菜单可能变高，并把更多布局成本暴露给外部宿主。
- 不使用 C# 动态创建或重挂载 `ScrollViewer`，避免额外事件订阅、释放路径和 `/template/` selector 失效风险。
- `IsScrollEnabled` 是低频配置属性，不注册为热路径 render 状态。

生命周期边界：

- `MenuPopupScrollHost` 不持有外部事件、timer、subscription 或缓存。
- `MenuFlyout` 到 `MenuFlyoutPresenter` 的属性中继由 presenter binding disposable 管理。
- 模板重套用时旧 `MenuPopupScrollHost` 和其内部 `ScrollViewer` 由 Avalonia 模板生命周期释放。

## 9. 兼容性与定制边界

- 默认 `IsScrollEnabled=true` 保持现有滚动行为和 `DisplayPageSize` 默认语义。
- `DisplayPageSize` 不被废弃；它仍然是滚动开启时的菜单弹层显示页数上限。
- `IsScrollEnabled=false` 的稳定语义是“不创建 `ScrollViewer` 并显示全部菜单项”，应用不应依赖隐藏滚动条或禁用滚动条这样的中间状态。
- 主题自定义可以替换 `MenuPopupScrollHost` 的模板，但必须保持 `PART_ItemsPresenter` 在两种模式下都可被菜单项生成逻辑找到。
- 外部模板若绕过 `MenuPopupScrollHost`，需要自行承担滚动开关和 `DisplayPageSize` 的语义一致性。
- Token 不表达滚动开关；滚动模式属于实例 API 和模板组合策略。

## 10. 验证要求

验证按以下层级执行：

| 层级 | 要求 |
| --- | --- |
| API 契约 | 覆盖 `IsScrollEnabled` 默认值为 `true`，`Menu`、`ContextMenu`、`MenuItem`、`MenuFlyout` 和 `MenuFlyoutPresenter` 均可读取和设置。 |
| 属性传播 | 覆盖 `Menu` 到顶层 `MenuItem`、`MenuItem` 到子 `MenuItem`、`ContextMenu` 到 `MenuItem` 的继承传播，以及子项本地值覆盖。 |
| `MenuFlyout` 中继 | 覆盖 `MenuFlyout.IsScrollEnabled` 传递到 `MenuFlyoutPresenter`，并在 presenter 重建时保持配置。 |
| 模板结构 | 覆盖滚动开启时存在 `ScrollViewer`，滚动禁用时视觉树中不存在 `ScrollViewer`。 |
| 高度算法 | 覆盖滚动开启时按 `ItemHeight * DisplayPageSize + padding` 计算，滚动禁用时为 `double.PositiveInfinity`。 |
| 交互回归 | 覆盖 hover intent、keyboard navigation、checked item、separator、click close 和 popup close 行为不因滚动模式变化而改变。 |
| Gallery | 走查 Menu、ContextMenu 和 MenuFlyout 示例，验证长菜单滚动与禁用滚动的可观察行为。 |
| AOT | 确认没有新增反射、字符串 binding、动态注册或手工维护生成文件。 |
