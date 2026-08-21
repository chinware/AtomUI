# Popup Token 设计

本文档定义 Popup 专属 Token 与共享表面 Token 的语义。Popup 公共契约见
[Popup 桌面版架构设计](overview.md)，内部映射见 [Popup 桌面版实现原理](implementation.md)。控件 Token 通用规则见
[AtomUI 控件 Token 设计规范](../../../../engineering/development/control-token-guidelines.md)。

## 1. Popup 专属 Token

`PopupToken` 是 internal control token，服务 Popup frame 和多个 Popup-bearing 控件：

| Token | 派生来源 | 语义与消费者 |
| --- | --- | --- |
| `PopupRootShadow` | 两层固定 alpha shadow | native `PopupRoot` frame shadow；菜单、选择器和自动建议等家族复用。 |
| `OverlayHostShadow` | `BoxShadowsSecondary` | `OverlayPopupHost` frame shadow；Flyout/Menu/ContextMenu 等 overlay 路径复用。 |
| `PopupCornerRadius` | `BorderRadiusLG` | 专用 Presenter / `PopupFrame` 的默认圆角。 |
| `MarginToAnchor` | `UniformlyMarginXXS` | Popup 内容 frame 与 anchor 的默认间距。 |

`PopupCornerRadius` 不由 transparent host 绘制；它由 Child、Presenter 或 `PopupFrame` 提供给
`ShadowsAwareContainer`，使 frame shadow/default surface 与内容圆角一致。

## 2. 默认表面

`Popup.SurfaceBackground` 直接映射 Shared Token `ColorBgElevated`，不新增重复的 Popup 专属颜色 Token。该资源表达所有
elevated surface 的共享主题语义，并自动响应 light/dark 和主题切换。

| surface ownership | Token 消费 |
| --- | --- |
| Direct Popup / host-owned | Popup Theme 消费 `ColorBgElevated` 并由 frame renderer 绘制。 |
| 专用控件 / content-owned | Popup 设置 `SurfaceBackground=null`；Presenter 或 `PopupFrame` 继续消费自己的背景 Token。 |

`null` 是所有权选择，不是新的透明颜色 Token。不得用 `Transparent` Brush 替代 `null` 来表达 content-owned，因为非空
Brush 会使 host 仍被视为表面所有者。

## 3. 兼容与验证

- 改变 `ColorBgElevated` 会影响 Direct Popup 和其他共享 elevated surfaces；专用控件仍按自身 Theme/Token 映射。
- 改变 shadow Token 不得改变 surface ownership、Child bounds 或 placement。
- 改变 `PopupCornerRadius` 只影响消费该 Token 的 Presenter/PopupFrame，不给 Popup 增加默认圆角或 wrapper。
- Token 验证必须覆盖 light/dark 主题、native/overlay shadow 选择、Direct Popup 默认 surface 和 content-owned opt-out。

当前 Windows 与 macOS 已做实机验证；Linux X11/Wayland 未测试。
