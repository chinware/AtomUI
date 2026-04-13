# SplitView API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### SplitViewDisplayMode（来自 `Avalonia.Controls`）

显示模式枚举，控制面板与主内容的空间关系。

| 值 | 说明 |
|---|---|
| `Overlay` | 面板覆盖在主内容之上，收起时完全隐藏 |
| `Inline` | 面板与主内容并排，展开时压缩主内容 |
| `CompactInline` | 收起时保留紧凑条，展开时压缩主内容 |
| `CompactOverlay` | 收起时保留紧凑条，展开时覆盖主内容 |

### SplitViewPanePlacement（来自 `Avalonia.Controls`）

面板放置位置枚举。

| 值 | 说明 |
|---|---|
| `Left` | 面板在左侧（默认） |
| `Right` | 面板在右侧 |
| `Top` | 面板在顶部 |
| `Bottom` | 面板在底部 |

---

## AtomUI 扩展的公共属性（StyledProperty）

以下属性由 `AtomUI.Desktop.Controls.SplitView` 新增，不存在于 Avalonia 原生 SplitView 中。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `PaneMotionEasing` | `Easing?` | 由 Token 控制（`cubic-bezier(0.1, 0.9, 0.2, 1.0)`） | 面板展开/收起的缓动曲线 |
| `PaneOpenMotionDuration` | `TimeSpan` | 由 Token 控制（200ms） | 面板展开动画时长 |
| `PaneCloseMotionDuration` | `TimeSpan` | 由 Token 控制（100ms） | 面板收起动画时长 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（共享属性，通过 `AddOwner` 从 `MotionAwareControlProperty` 注册） |

---

## 继承自 Avalonia.Controls.SplitView 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 主内容区域，可以是任意控件 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 主内容数据模板 |
| `Pane` | `object?` | `null` | 侧边栏面板内容 |
| `PaneTemplate` | `IDataTemplate?` | `null` | 面板内容数据模板 |
| `IsPaneOpen` | `bool` | `false` | 面板是否展开 |
| `DisplayMode` | `SplitViewDisplayMode` | `Overlay` | 显示模式 |
| `PanePlacement` | `SplitViewPanePlacement` | `Left` | 面板放置位置 |
| `OpenPaneLength` | `double` | 由 Token 控制（320） | 面板完全展开时的宽度/高度 |
| `CompactPaneLength` | `double` | 由 Token 控制（48） | 紧凑模式下面板的宽度/高度 |
| `PaneBackground` | `IBrush?` | 由 Token 控制（`ColorBgContainer`） | 面板背景画刷 |
| `UseLightDismissOverlayMode` | `bool` | `false` | 是否在 Overlay 模式下启用点击遮罩关闭面板 |
| `Background` | `IBrush?` | `null` | 整体背景色 |
| `IsEnabled` | `bool` | `true` | 是否启用 |

---

## 内部属性（Internal）

以下属性仅供控件内部和主题系统使用，不对外公开。

| 属性名 | 类型 | 说明 |
|---|---|---|
| `PaneOpenTransitions` | `Transitions?` | 面板展开时的过渡动画集合，由 `ConfigureTransitions` 自动管理 |
| `PaneCloseTransitions` | `Transitions?` | 面板收起时的过渡动画集合，由 `ConfigureTransitions` 自动管理 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `PaneClosing` | `EventHandler<CancelRoutedEventArgs>` | 面板即将关闭时触发，可通过 `Cancel = true` 取消关闭（继承自 `Avalonia.Controls.SplitView`） |
| `PaneClosed` | `EventHandler<RoutedEventArgs>` | 面板关闭完成后触发（继承自 `Avalonia.Controls.SplitView`） |

---

## 伪类（Pseudo-Classes）

SplitView 支持以下伪类，可在样式选择器中使用。所有伪类均继承自 Avalonia 原生 SplitView。

### 面板状态伪类

| 伪类 | 触发条件 |
|---|---|
| `:open` | `IsPaneOpen == true` |
| `:closed` | `IsPaneOpen == false` |

### 面板位置伪类

| 伪类 | 触发条件 |
|---|---|
| `:left` | `PanePlacement == Left` |
| `:right` | `PanePlacement == Right` |
| `:top` | `PanePlacement == Top` |
| `:bottom` | `PanePlacement == Bottom` |

### 显示模式伪类

| 伪类 | 触发条件 |
|---|---|
| `:overlay` | `DisplayMode == Overlay` |
| `:inline` | `DisplayMode == Inline` |
| `:compactinline` | `DisplayMode == CompactInline` |
| `:compactoverlay` | `DisplayMode == CompactOverlay` |

### 其他伪类

| 伪类 | 触发条件 |
|---|---|
| `:lightDismiss` | `UseLightDismissOverlayMode == true` |
| `:disabled` | `IsEnabled == false` |

---

## 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制面板展开/收起时是否使用过渡动画。设为 `false` 时面板将瞬间切换尺寸，不产生动画效果。
