# TabControl API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### SizeType（来自 `AtomUI.Controls`）

| 值 | 说明 |
|---|---|
| `Small` | 小号标签页 |
| `Middle` | 中号标签页（默认） |
| `Large` | 大号标签页 |

### TabSharp

标签形状枚举（内部使用，由控件类型自动设置）。

| 值 | 说明 |
|---|---|
| `Line` | 线条式 |
| `Card` | 卡片式 |

---

## BaseTabControl 公共属性

`BaseTabControl` 是 `TabControl` 和 `CardTabControl` 的共享基类，以下属性对两者均有效。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `TabStripPlacement` | `Dock` | `Dock.Top` | 标签栏位置（Top/Right/Bottom/Left） |
| `SizeType` | `SizeType` | `SizeType.Middle` | 标签尺寸（共享属性） |
| `TabAlignmentCenter` | `bool` | `false` | 标签是否居中对齐 |
| `IsTabClosable` | `bool` | `false` | 是否全局启用标签关闭按钮 |
| `IsTabAutoHideCloseButton` | `bool` | `false` | 关闭按钮是否仅悬浮时显示 |
| `HeaderStartExtraContent` | `object?` | `null` | 标签栏起始端额外内容 |
| `HeaderStartExtraContentTemplate` | `IDataTemplate?` | `null` | 起始端额外内容模板 |
| `HeaderEndExtraContent` | `object?` | `null` | 标签栏末端额外内容 |
| `HeaderEndExtraContentTemplate` | `IDataTemplate?` | `null` | 末端额外内容模板 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容面板模板 |
| `ContentPadding` | `Thickness` | 由 Token 控制 | 内容区域内间距 |
| `TabAndContentGutter` | `double` | 由 Token 控制 | 标签栏与内容区域间距 |
| `HeaderStartEdgePadding` | `double` | `0` | 标签栏起始端边距 |
| `HeaderEndEdgePadding` | `double` | `0` | 标签栏末端边距 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `HorizontalContentAlignment` | `HorizontalAlignment` | `Stretch` | 内容水平对齐 |
| `VerticalContentAlignment` | `VerticalAlignment` | `Stretch` | 内容垂直对齐 |
| `SelectedContent` | `object?` | — (只读) | 当前选中标签的内容 |
| `SelectedContentTemplate` | `IDataTemplate?` | — (只读) | 当前选中标签的内容模板 |

### 继承自 SelectingItemsControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SelectedIndex` | `int` | `0` | 当前选中标签索引 |
| `SelectedItem` | `object?` | — | 当前选中项 |
| `ItemsSource` | `IEnumerable?` | `null` | 数据绑定源 |
| `ItemTemplate` | `IDataTemplate?` | `null` | 数据项模板 |

---

## CardTabControl 附加属性

`CardTabControl` 在 `BaseTabControl` 基础上增加：

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsShowAddTabButton` | `bool` | `false` | 是否显示添加标签按钮 |

---

## BaseTabStrip 公共属性

`BaseTabStrip` 是 `TabStrip` 和 `CardTabStrip` 的共享基类。属性与 `BaseTabControl` 高度一致，但无内容面板相关属性。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `TabStripPlacement` | `Dock` | `Dock.Top` | 标签栏位置 |
| `SizeType` | `SizeType` | `SizeType.Middle` | 标签尺寸 |
| `TabAlignmentCenter` | `bool` | `false` | 标签是否居中对齐 |
| `IsTabClosable` | `bool` | `false` | 是否全局启用标签关闭按钮 |
| `IsTabAutoHideCloseButton` | `bool` | `false` | 关闭按钮是否仅悬浮时显示 |
| `HeaderStartExtraContent` | `object?` | `null` | 标签栏起始端额外内容 |
| `HeaderStartExtraContentTemplate` | `IDataTemplate?` | `null` | 起始端额外内容模板 |
| `HeaderEndExtraContent` | `object?` | `null` | 标签栏末端额外内容 |
| `HeaderEndExtraContentTemplate` | `IDataTemplate?` | `null` | 末端额外内容模板 |
| `HeaderStartEdgePadding` | `double` | `0` | 标签栏起始端边距 |
| `HeaderEndEdgePadding` | `double` | `0` | 标签栏末端边距 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |

### CardTabStrip 附加属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsShowAddTabButton` | `bool` | `false` | 是否显示添加标签按钮 |

---

## TabItem 公共属性

`TabItem` 用于 `TabControl` / `CardTabControl` 中的标签项。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Header` | `object?` | `null` | 标签标题（继承自 `HeaderedContentControl`） |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 标题模板 |
| `Content` | `object?` | `null` | 标签面板内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 面板内容模板 |
| `Icon` | `PathIcon?` | `null` | 标签图标 |
| `CloseIcon` | `PathIcon?` | `CloseOutlined` | 关闭按钮图标 |
| `IsClosable` | `bool` | `false` | 该标签是否可关闭 |
| `IsAutoHideCloseButton` | `bool` | `false` | 关闭按钮是否自动隐藏 |
| `IsSelected` | `bool` | `false` | 是否选中 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `TabStripPlacement` | `Dock?` | — (只读) | 标签栏位置（由父容器传递） |

---

## TabStripItem 公共属性

`TabStripItem` 用于 `TabStrip` / `CardTabStrip` 中的标签项。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 标签文本内容（继承自 `ContentControl`） |
| `Icon` | `PathIcon?` | `null` | 标签图标 |
| `CloseIcon` | `PathIcon?` | `CloseOutlined` | 关闭按钮图标 |
| `IsClosable` | `bool` | `false` | 该标签是否可关闭 |
| `IsAutoHideCloseButton` | `bool` | `false` | 关闭按钮是否自动隐藏 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `TabStripPlacement` | `Dock?` | — (只读) | 标签栏位置（由父容器传递） |

---

## ITabItemData 接口

用于 `ItemsSource` 数据绑定场景，自动映射属性到 TabItem / TabStripItem。

```csharp
public interface ITabItemData : IHeadered
{
    PathIcon? Icon { get; }
    PathIcon? CloseIcon { get; }
    bool IsEnabled { get; }
    bool IsClosable { get; }
    bool IsAutoHideCloseButton { get; }
}
```

`TabItemData` 是其默认实现，所有属性为 `init` 属性。

---

## 事件

### BaseTabControl / BaseTabStrip 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Closing` | `EventHandler<TabClosingEventArgs>` | 标签关闭前触发，可设置 `Cancel = true` 阻止关闭 |
| `Closed` | `EventHandler<TabClosedEventArgs>` | 标签成功关闭后触发 |
| `SelectionChanged` | `EventHandler<SelectionChangedEventArgs>` | 选中标签变更时触发（继承自 `SelectingItemsControl`） |

### CardTabControl / CardTabStrip 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `AddTabRequest` | `EventHandler<RoutedEventArgs>` | 点击添加按钮时触发 |

### TabClosingEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `TabItem` | `TabItem` | 即将被关闭的标签项 |
| `Cancel` | `bool` | 设置为 `true` 阻止关闭 |

### TabClosedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `TabItem` | `TabItem` | 已关闭的标签项 |

---

## 公共方法

### BaseTabControl.CloseTab

```csharp
public bool CloseTab(TabItem tabItem)
```

以编程方式关闭指定标签。触发 `Closing` / `Closed` 事件流。返回 `true` 表示成功关闭。

### BaseTabStrip.CloseTab

```csharp
public bool CloseTab(TabStripItem tabStripItem)
```

同上，用于 TabStrip 系列。

---

## 伪类（Pseudo-Classes）

### BaseTabControl / BaseTabStrip 伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:top` | `TabPseudoClass.Top` | `TabStripPlacement == Dock.Top` |
| `:right` | `TabPseudoClass.Right` | `TabStripPlacement == Dock.Right` |
| `:bottom` | `TabPseudoClass.Bottom` | `TabStripPlacement == Dock.Bottom` |
| `:left` | `TabPseudoClass.Left` | `TabStripPlacement == Dock.Left` |

### TabItem / TabStripItem 伪类

| 伪类 | 触发条件 |
|---|---|
| `:selected` | 该标签被选中 |
| `:pressed` | 鼠标按下 |
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |

---

## 实现的接口

### IMotionAwareControl（BaseTabControl / BaseTabStrip）

```csharp
public bool IsMotionEnabled { get; set; }
```

### ISizeTypeAware（BaseTabStrip）

```csharp
public SizeType SizeType { get; set; }
```

---

## 键盘交互

| 按键 | 行为 |
|---|---|
| `←` / `→` | 水平布局时切换标签 |
| `↑` / `↓` | 垂直布局时切换标签 |
| `Tab` | 焦点在标签间导航 |
