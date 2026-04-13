# Drawer API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### DrawerPlacement

抽屉滑入方向。

| 值 | 说明 |
|---|---|
| `Left` | 从左侧滑入 |
| `Top` | 从顶部滑入 |
| `Right` | 从右侧滑入（默认） |
| `Bottom` | 从底部滑入 |

### CustomizableSizeType（来自 `AtomUI.Controls`）

可自定义的尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号（默认，378px） |
| `Middle` | 中号（520px） |
| `Large` | 大号（736px） |
| `Custom` | 自定义（使用 `DialogSize` 指定） |

---

## 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 抽屉内容（支持任意控件），标记 `[Content]` 可在 AXAML 中直接作为子元素 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容数据模板 |
| `IsOpen` | `bool` | `false` | 抽屉是否打开。默认绑定模式为 `TwoWay`，可直接与 ViewModel 属性双向绑定 |
| `Placement` | `DrawerPlacement` | `Right` | 抽屉滑入方向 |
| `OpenOn` | `Control?` | `null`（自动绑定到 `TopLevel`） | 指定渲染范围的目标控件。子抽屉自动继承父抽屉的 `OpenOn`；若无父抽屉则自动绑定到 `TopLevel` |
| `IsShowMask` | `bool` | `true` | 是否显示半透明遮罩层 |
| `IsShowCloseButton` | `bool` | `true` | 是否显示关闭按钮 |
| `CloseWhenClickOnMask` | `bool` | `true` | 点击遮罩层是否关闭抽屉 |
| `Title` | `string` | — | 抽屉标题 |
| `Footer` | `object?` | `null` | 底部区域内容（标记 `[DependsOn(FooterTemplate)]`） |
| `FooterTemplate` | `IDataTemplate?` | `null` | 底部区域数据模板 |
| `Extra` | `object?` | `null` | 标题栏右侧额外操作区域（标记 `[DependsOn(ExtraTemplate)]`） |
| `ExtraTemplate` | `IDataTemplate?` | `null` | 额外操作区域数据模板 |
| `SizeType` | `CustomizableSizeType` | `Small` | 预设尺寸（共享属性，通过 `AddOwner` 注册，默认值被覆盖为 `Small`） |
| `DialogSize` | `Dimension` | 由 Token 控制 | 自定义尺寸（像素或百分比）。当 `SizeType` 为 `Small`/`Middle`/`Large` 时由 Token 自动设置；`Custom` 时由用户指定 |
| `PushOffsetPercent` | `double` | 由 Token 控制（0.4） | 多级抽屉 Push 偏移比率 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用滑入/滑出动画（共享属性）。子抽屉自动继承父抽屉的 `IsMotionEnabled` |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Opened` | `EventHandler?` | 抽屉打开后触发 |
| `Closed` | `EventHandler?` | 抽屉关闭后触发 |

---

## 静态方法

| 方法 | 返回值 | 说明 |
|---|---|---|
| `Drawer.GetDrawer(Visual)` | `Drawer?` | 从可视树中查找指定元素所属的 Drawer 实例 |

---

## 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制滑入/滑出动画。

### ICustomizableSizeTypeAware

```csharp
public CustomizableSizeType SizeType { get; set; }
```

支持 Small / Middle / Large / Custom 四种尺寸模式。
