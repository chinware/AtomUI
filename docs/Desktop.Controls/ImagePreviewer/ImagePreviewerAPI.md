# ImagePreviewer API 参考

## 命名空间

- **C#**：`AtomUI.Desktop.Controls`
- **AXAML**：`xmlns:atom="https://atomui.net"`

---

## 公共类

| 类名 | 说明 |
|---|---|
| `AbstractImagePreviewer` | 图片预览基类，定义图片源、对话框和缩放相关属性 |
| `ImagePreviewer` | 单图预览控件，展示封面 + 遮罩，点击弹出预览 |
| `ImageGroupPreviewer` | 多图组预览控件，展示多张缩略图，点击弹出预览 |
| `ImageFitToWindowEventArgs` | 适应窗口事件参数（`IsFitToWindow` 属性） |

---

## 枚举类型

### DialogHorizontalAnchor（来自 `AtomUI.Desktop.Controls.DialogPositioning`）

| 值 | 说明 |
|---|---|
| `Custom` | 自定义偏移（默认） |
| `Left` | 左对齐 |
| `Center` | 水平居中 |
| `Right` | 右对齐 |

### DialogVerticalAnchor（来自 `AtomUI.Desktop.Controls.DialogPositioning`）

| 值 | 说明 |
|---|---|
| `Custom` | 自定义偏移（默认） |
| `Top` | 顶部对齐 |
| `Center` | 垂直居中 |
| `Bottom` | 底部对齐 |

---

## 公共属性（StyledProperty）

### AbstractImagePreviewer — 核心属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ItemsSource` | `IList<string>?` | `null` | 图片路径列表（支持 `avares://` 和本地路径），自动检测 `.svg` 后缀走 SVG 渲染 |
| `FallbackImageSrc` | `string?` | `null` | 容错图片路径，当 `ItemsSource` 为空或加载失败时使用 |
| `IsOpen` | `bool` | `false` | 预览对话框是否打开。可双向绑定以编程方式控制对话框 |
| `IsMotionEnabled` | `bool` | 由 `SharedToken.EnableMotion` 控制 | 是否启用过渡动画（`IMotionAwareControl`） |
| `CoverWidth` | `double` | `double.NaN` | 封面宽度。`NaN` 时跟随图片原始宽度或父容器宽度 |
| `CoverHeight` | `double` | `double.NaN` | 封面高度。`NaN` 时跟随图片原始高度或父容器高度 |
| `CurrentIndex` | `int` | `0` | 当前预览图片的索引（从 0 开始） |

### AbstractImagePreviewer — 预览窗口属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsImageMovable` | `bool` | `true` | 是否允许鼠标拖拽移动图片（缩放后有效） |
| `ImageScaleStep` | `double` | `0.5` | 缩放步进比例（0.5 = 每次缩放 50%） |
| `ImageMinScale` | `double` | `1.0` | 最小缩放倍数 |
| `ImageMaxScale` | `double` | `50.0` | 最大缩放倍数 |
| `IsDialogModal` | `bool` | `false` | 是否以模态方式打开预览对话框。模态时阻塞主窗口交互 |
| `DialogHorizontalStartupLocation` | `DialogHorizontalAnchor` | `Custom` | 对话框水平初始位置锚点 |
| `DialogVerticalStartupLocation` | `DialogVerticalAnchor` | `Custom` | 对话框垂直初始位置锚点 |
| `DialogHorizontalOffset` | `Dimension?` | `null` | 对话框水平偏移量（仅 `Custom` 定位时生效） |
| `DialogVerticalOffset` | `Dimension?` | `null` | 对话框垂直偏移量（仅 `Custom` 定位时生效） |
| `CustomDialogPlacementCallback` | `CustomDialogPlacementCallback?` | `null` | 自定义对话框位置计算回调 |
| `IsDialogTopmost` | `bool` | `false` | 预览对话框是否置顶显示 |

### ImagePreviewer — 封面属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `CoverImageSrc` | `string?` | `null` | 自定义封面图路径。未设置时自动使用 `ItemsSource` 的首张图 |
| `CoverIndicatorContent` | `object?` | `null` | 遮罩指示器的内容对象 |
| `CoverIndicatorContentTemplate` | `IDataTemplate?` | 默认模板（眼睛图标 + 本地化 "Preview" 文字） | 遮罩指示器的内容模板 |
| `IsShowCoverMask` | `bool` | `true` | 是否显示封面悬浮遮罩 |

### ImageGroupPreviewer — 布局属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ItemsPanel` | `ITemplate<Panel?>` | `StackPanel (Horizontal)` | 封面缩略图的布局面板模板 |

---

## 公共方法

| 方法名 | 返回值 | 说明 |
|---|---|---|
| `OpenDialog()` | `void` | 编程方式打开预览对话框。若对话框已打开则无操作 |
| `CloseDialog()` | `void`（`protected virtual`） | 编程方式关闭预览对话框。会触发 `DialogClosing` 事件，可被取消 |

---

## 公共事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `DialogOpened` | `EventHandler?` | 预览对话框打开后触发 |
| `DialogClosed` | `EventHandler?` | 预览对话框关闭后触发 |
| `DialogClosing` | `EventHandler<CancelEventArgs>?` | 预览对话框即将关闭时触发。设置 `Cancel = true` 可阻止关闭 |

---

## 伪类

ImagePreviewer 和 ImageGroupPreviewer 本身不定义自定义伪类。内部组件使用的伪类：

| 伪类 | 控件 | 触发条件 |
|---|---|---|
| `:pointerover` | `ImagePreviewerCover` | 鼠标悬浮在封面上，触发遮罩淡入动画 |
| `:pointerover` | `ImagePreviewNavButton` | 鼠标悬浮在导航按钮上，背景加深 |
| `:disabled` | `ImagePreviewNavButton` | 到达首张/末张图片时导航按钮禁用 |
| `:disabled` | 工具栏 `IconButton` | 缩放到极限时缩小/放大按钮禁用 |
| `:pressed` | 工具栏 `IconButton` | 按钮被按下 |

---

## 实现的接口

| 接口 | 定义位置 | 说明 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | 支持 `IsMotionEnabled` 过渡动画开关 |

---

## 本地化资源

| 资源键 | en_US | zh_CN | 使用位置 |
|---|---|---|---|
| `Preview` | `"Preview"` | `"预览"` | 封面遮罩指示器默认文字 |

---

## 键盘交互

| 按键 | 说明 |
|---|---|
| `←` (Left) | 切换到上一张图片（多图模式时） |
| `→` (Right) | 切换到下一张图片（多图模式时） |
