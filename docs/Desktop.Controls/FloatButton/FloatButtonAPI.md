# FloatButton API 参考

## 命名空间

- **C#**：`AtomUI.Desktop.Controls`（平台层）、`AtomUI.Controls.Commons`（基类层）
- **AXAML**：`xmlns:atom="https://atomui.net"`

---

## FloatButton / FloatButtonHost 公共属性

以下属性同时适用于 `FloatButtonHost`（AXAML 中使用）和 `FloatButton`（内部按钮实例）。Host 通过绑定将属性值同步到内部 FloatButton。

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Placement` | `FloatButtonPlacement` | — | 悬浮位置（Top/Bottom/Left/Right/TopLeft/TopRight/BottomLeft/BottomRight/Center） |
| `FloatOffsetX` | `double` | 由 `FloatOffsetX` Token 控制 | 水平偏移量 |
| `FloatOffsetY` | `double` | 由 `FloatOffsetY` Token 控制 | 垂直偏移量 |
| `Icon` | `PathIcon?` | `FileTextOutlined` | 按钮图标（未设置时使用默认图标） |
| `Tooltip` | `string?` | `null` | 鼠标悬浮时的提示文本 |
| `TooltipColor` | `Color?` | `null` | 提示文本颜色 |
| `ButtonType` | `FloatButtonType` | `Default` | 按钮类型（Default / Primary） |
| `Shape` | `FloatButtonShape` | `Circle` | 按钮形状（Circle / Square） |
| `Href` | `Uri?` | `null` | 点击跳转的链接 |
| `BoxShadow` | `BoxShadows` | 由主题控制 | 按钮阴影 |
| `IsMotionEnabled` | `bool` | 由 `EnableMotion` Token 控制 | 是否启用过渡动画 |
| `IsBadgeEnabled` | `bool` | `false` | 是否启用徽标 |
| `IsDotBadge` | `bool` | `false` | 是否使用圆点徽标（否则为数字徽标） |
| `BadgeCount` | `int` | `0` | 徽标数字 |
| `BadgeColor` | `string?` | `null` | 徽标颜色（支持预设色名或 hex 值） |
| `BadgeOffset` | `Point` | `(0, 0)` | 徽标偏移 |
| `BadgeOverflowCount` | `int` | `99` | 徽标数字上限（超过显示 `99+`），最小值 0 |

### FloatButtonHost 专属属性

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Description` | `object?` | `null` | 描述文本（仅 Square 形状有效），标注 `[DependsOn(nameof(DescriptionTemplate))]` |
| `DescriptionTemplate` | `IDataTemplate?` | `null` | 描述文本模板 |

---

## FloatButtonGroup / FloatButtonGroupHost 公共属性

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Placement` | `FloatButtonPlacement` | — | 悬浮位置 |
| `FloatOffsetX` | `double` | 由 Token 控制 | 水平偏移 |
| `FloatOffsetY` | `double` | 由 Token 控制 | 垂直偏移 |
| `Icon` | `PathIcon?` | `FileTextOutlined` | 触发按钮图标 |
| `CloseIcon` | `PathIcon?` | `CloseOutlined` | 收起时的图标 |
| `ButtonType` | `FloatButtonType` | `Default` | 按钮类型 |
| `Shape` | `FloatButtonShape` | `Circle` | 按钮形状（子按钮继承此属性） |
| `BoxShadow` | `BoxShadows` | 由主题控制 | 阴影 |
| `IsMotionEnabled` | `bool` | 由 Token 控制 | 是否启用动画（子按钮继承此属性） |
| `MenuPlacement` | `FloatButtonGroupMenuPlacement` | `Top` | 子按钮展开方向（Top / Bottom / Left / Right） |
| `Trigger` | `FloatButtonGroupTrigger` | `Default` | 触发模式（Default / Click / Hover） |
| `IsOpen` | `bool` | `false` | 是否展开（可双向绑定） |
| `MenuMotionDuration` | `TimeSpan` | 由主题控制 | 展开/收起动画时长 |
| `Children` | `Controls` | — | 子 FloatButton 集合（`[Content]` 内容属性） |

---

## BackTopFloatButton / BackTopFloatButtonHost 额外属性

继承 FloatButton / FloatButtonHost 的全部属性，额外增加：

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ToTopDuration` | `TimeSpan` | `450ms` | 滚动到顶部的动画时长 |
| `Target` | `ScrollViewer?` | `null` | 关联的 ScrollViewer（未设置时自动查找父级） |
| `VisibilityHeight` | `double` | `400` | 滚动超过此高度时显示按钮 |

---

## 事件

### FloatButtonGroup / FloatButtonGroupHost

| 事件 | 类型 | 路由策略 | 说明 |
|---|---|---|---|
| `Clicked` | `EventHandler<RoutedEventArgs>` | Bubble | 触发按钮被点击 |
| `Opened` | `EventHandler<RoutedEventArgs>` | Bubble | 子按钮展开时触发 |
| `Closed` | `EventHandler<RoutedEventArgs>` | Bubble | 子按钮收起时触发 |

---

## 枚举

### FloatButtonType

| 值 | 说明 |
|---|---|
| `Default` | 默认样式（白色背景 + 阴影） |
| `Primary` | 主要样式（主色背景 + 白色图标） |

### FloatButtonShape

| 值 | 说明 |
|---|---|
| `Circle` | 圆形（自动计算圆角） |
| `Square` | 方形（支持描述文本） |

### FloatButtonPlacement

| 值 | 说明 |
|---|---|
| `Top` / `Bottom` / `Left` / `Right` | 四边居中 |
| `TopLeft` / `TopRight` / `BottomLeft` / `BottomRight` | 四角（`BottomRight` 最常用） |
| `Center` | 居中 |

### FloatButtonGroupTrigger

| 值 | 说明 |
|---|---|
| `Default` | 所有按钮始终展开显示，无触发按钮 |
| `Click` | 点击展开/收起，点击外部区域自动收起 |
| `Hover` | 悬浮展开/离开收起 |

### FloatButtonGroupMenuPlacement

| 值 | 说明 |
|---|---|
| `Top` | 向上展开 |
| `Bottom` | 向下展开 |
| `Left` | 向左展开 |
| `Right` | 向右展开 |

---

## 伪类

### FloatButton

| 伪类 | 触发条件 |
|---|---|
| `:icononly` | 圆形按钮始终为 true；方形按钮在仅有图标、无内容时 |
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 按下 |
| `:disabled` | 禁用 |

---

## 实现的接口

| 接口 | 控件 | 说明 |
|---|---|---|
| `IMotionAwareControl` | 所有 FloatButton 体系控件 | 动画开关（`IsMotionEnabled`） |
