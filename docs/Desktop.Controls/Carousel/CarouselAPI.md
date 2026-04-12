# Carousel API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### CarouselPaginationPosition

分页指示器位置枚举。

| 值 | 说明 |
|---|---|
| `Top` | 顶部居中 |
| `Bottom` | 底部居中（默认） |
| `Left` | 左侧居中（指示器旋转 90°） |
| `Right` | 右侧居中（指示器旋转 90°） |

### CarouselTransitionEffect

页面过渡效果枚举。

| 值 | 说明 |
|---|---|
| `Scroll` | 滑动切换（默认），方向跟随 `PaginationPosition` |
| `Fade` | 淡入淡出切换 |

---

## Carousel 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsShowNavButtons` | `bool` | `false` | 是否显示导航箭头按钮 |
| `IsAutoPlay` | `bool` | `false` | 是否自动播放 |
| `AutoPlaySpeed` | `TimeSpan` | `3000ms` | 自动播放间隔时间 |
| `PaginationPosition` | `CarouselPaginationPosition` | `Bottom` | 分页指示器位置 |
| `IsShowPagination` | `bool` | `true` | 是否显示分页指示器 |
| `IsShowTransitionProgress` | `bool` | `false` | 是否在指示点上显示播放进度条 |
| `IsInfinite` | `bool` | `true` | 是否无限循环 |
| `PageTransitionDuration` | `TimeSpan` | `MotionDurationSlow`（Token） | 页面过渡动画时长 |
| `PageInEasing` | `Easing` | `CubicEaseOut` | 页面进入缓动函数 |
| `PageOutEasing` | `Easing` | `CubicEaseOut` | 页面离开缓动函数 |
| `TransitionEffect` | `CarouselTransitionEffect` | `Scroll` | 过渡效果类型 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token（`EnableMotion`） | 是否启用过渡动画（共享属性） |
| `IsSwipeEnabled` | `bool` | `false` | 是否启用滑动手势切换 |

### 继承自 SelectingItemsControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SelectedIndex` | `int` | `0` | 当前选中页索引 |
| `SelectedItem` | `object?` | `null` | 当前选中页对象 |
| `ItemCount` | `int` | （只读） | 页面总数 |
| `Items` | `ItemCollection` | 空 | 子项集合 |
| `ItemsSource` | `IEnumerable?` | `null` | 数据源（用于数据绑定） |
| `ItemTemplate` | `IDataTemplate?` | `null` | 子项模板 |
| `Background` | `IBrush?` | `null` | 背景色 |
| `Foreground` | `IBrush?` | `null` | 前景色 |
| `Padding` | `Thickness` | `0` | 内间距 |

---

## 公共方法

| 方法名 | 返回值 | 说明 |
|---|---|---|
| `Next()` | `void` | 切换到下一页。无限循环模式下，末尾页切换到首页 |
| `Previous()` | `void` | 切换到上一页。无限循环模式下，首页切换到末尾页 |

---

## 伪类（Pseudo-Classes）

Carousel 本身没有定义额外的自定义伪类。

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |

---

## 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制指示点宽度过渡动画和导航按钮不透明度过渡动画。

---

## CarouselPage API

走马灯页面容器，继承自 `ContentControl`。无额外属性定义。

```csharp
namespace AtomUI.Desktop.Controls;
public class CarouselPage : ContentControl
```

### 继承的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 页面内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `Background` | `IBrush?` | `null` | 页面背景色 |
| `HorizontalContentAlignment` | `HorizontalAlignment` | `Stretch` | 内容水平对齐 |
| `VerticalContentAlignment` | `VerticalAlignment` | `Stretch` | 内容垂直对齐 |

---

## CarouselNavButton API

导航箭头按钮，继承自 `IconButton`。添加了 `Opacity` 的过渡动画。

```csharp
namespace AtomUI.Desktop.Controls;
public class CarouselNavButton : IconButton
```

### 主题默认样式

| 属性 | 值 | 说明 |
|---|---|---|
| `IconWidth` / `IconHeight` | `ArrowSize`（Token） | 箭头图标大小 |
| `IconBrush` | `ColorBgContainer` | 箭头图标颜色（白色系） |
| `Opacity` | `0.2` → `:pointerover` 时变为 `1.0` | 默认半透明，悬浮时完全显示 |
