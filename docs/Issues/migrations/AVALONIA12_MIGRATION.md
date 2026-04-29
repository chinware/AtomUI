# Avalonia 12 迁移指南

## 核心要求
- .NET 8+ 必需（移动平台推荐 .NET 10）
- 所有 Avalonia 包必须更新到 v12

## 绑定系统变更
- `IBinding` 接口移除 → 使用 `BindingBase`
- `InstancedBinding` 移除 → 使用 `BindingExpressionBase`
- 编译绑定默认启用（`AvaloniaUseCompiledBindingsByDefault=true`）
- 绑定插件移除（数据注解验证默认禁用）
- `FuncMultiValueConverter` 参数从 `IEnumerable<TIn>` 改为 `IReadOnlyList<TIn>`

## UI & 输入行为
- Touch/pen 选择现在在释放时触发（不是按下时）- 符合原生平台约定
- 选择处理统一：`SelectingItemsControl`、`ListBox`、`ComboBox`、`TabControl`、`TreeView`
- `UpdateSelection` / `UpdateSelectionFromEventSource` → `UpdateSelectionFromEvent`
- 焦点事件：`GotFocusEventArgs` / `RoutedEventArgs` → `FocusChangedEventArgs`
- `KeyboardNavigationHandler.GetNext` → `FocusManager.GetNextElement`
- 手势事件从 `Gestures` 类移到 `InputElement`（XAML 中移除 `Gestures.` 前缀）
- 访问键现在由符号触发（支持重音字符和数字）
- `AccessText.AccessKey` 类型从 `char` 改为 `string?`

## 窗口 & 装饰
- `TopLevel` 不再保证在视觉树根部 → 使用 `TopLevel.GetTopLevel(Visual)`
- 移除的接口：`IInputRoot`、`IRenderRoot`、`ILayoutRoot`、`ITextInputMethodRoot`、`IEmbeddableLayoutRoot`
- 新增 `IPresentationSource` 接口用于视觉树宿主
- 窗口装饰系统大修：
  - `TitleBar`、`CaptionButtons`、`ChromeOverlayLayer` 移除
  - 改用新的 `WindowDrawnDecorations` 类
  - `ExtendClientAreaChromeHints` 移除 → 使用 `WindowDecorations` 属性
  - `SystemDecorations` 枚举 → `WindowDecorations` 枚举
- `Window.WindowState` 从样式属性改为直接属性（不能从样式设置）
- `Window.ExtendClientAreaToDecorationsHint` 在 Windows 上改进（移除旧的变通方法）

## 渲染 & 文本
- Direct2D1 后端移除 → 使用 Skia
- 文本整形器现在可独立配置
  - 如果使用 `UseSkia()`，必须也调用 `UseHarfBuzz()`
  - 添加 `Avalonia.HarfBuzz` 包引用
- 文本格式化构造函数合并（单个构造函数带可选 `FontFeatureCollection`）
- `GenericTextRunProperties`、`TextCollapsingProperties`、`TextShaperOptions` 受影响
- 不再支持旧版字体（Type 1 .pfb/.pfm）

## 剪贴板 & 拖放
- `IDataObject` 接口移除 → 使用 `IAsyncDataTransfer`
- `DataObject` 类移除 → 使用 `DataTransfer`
- `DataFormats.*` → `DataFormat.*`
- `DragDrop.DoDragDrop` → `DragDrop.DoDragDropAsync`
- `DragEventArgs.Data` → `DragEventArgs.DataTransfer`
- `IClipboard.GetTextAsync()` → `IClipboard.TryGetTextAsync()`
- `IClipboard.SetDataObjectAsync()` → `IClipboard.SetDataAsync()`

## 诊断
- `Avalonia.Diagnostics` 包移除
- 改用 `AvaloniaUI.DiagnosticsSupport`
- `AttachDevTools()` → `AttachDeveloperTools()`

## 其他变更
- `ResourcesChangedEventArgs` 现在是结构体 → 使用 `ResourcesChangedEventArgs.Create()`
- `Screen` 类现在是抽象的（不要直接构造）
- 自定义控件默认启用数据验证（移除 `UpdateDataValidation` 重写）
- 不可见控件上的动画停止（如需要设置 `Animation.PlaybackBehavior = Always`）
- 多调度程序支持（使用 `AvaloniaObject.Dispatcher` 和 `Dispatcher.CurrentDispatcher`）
- `Dispatcher.InvokeAsync` 现在捕获执行上下文

## 平台特定
- **Android**: `AvaloniaMainActivity<TApp>` → `AvaloniaMainActivity` + 新的 `AvaloniaAndroidApplication<TApp>` 类
- **Android**: `ISingleViewApplicationLifetime` → `IActivityApplicationLifetime`（带 `MainViewFactory`）
- **iOS**: 基于场景的生命周期，`AvaloniaAppDelegate.Window` 初始化后保持为 null
- **浏览器**: `Avalonia.Browser.Blazor` 包移除
- **Tizen**: 不再支持
- **测试**: xUnit.net v3（从 v2）、NUnit v4（从 v3）

---

## NavMenu 移植问题与解决方案

### 问题 1: IRenderRoot 接口被移除
**症状**: `IRenderRoot` 类型无法找到

**解决方案**:
- 移除 `INavMenu.VisualRoot` 属性（因为 `Visual.VisualRoot` 是 `protected internal`，无法在公共接口中实现）
- 在需要获取顶级元素时，使用 `TopLevel.GetTopLevel(visual)` 替代

### 问题 2: Focus 事件参数变更
**症状**: `GotFocusEventArgs` 类型不存在

**解决方案**:
- `GotFocus` 事件: `GotFocusEventArgs` → `FocusChangedEventArgs`
- `LostFocus` 事件: 保持使用 `RoutedEventArgs`（与 Avalonia 12 一致）
- 参考 Avalonia 的 `DefaultMenuInteractionHandler` 实现

### 问题 3: ILayoutRoot 接口被移除
**症状**: `ILayoutRoot` 接口不可访问

**解决方案**:
- 移除 `ILayoutRoot` 类型检查
- 改用 `InvalidateArrange()` 和 `InvalidateMeasure()` 来触发布局更新
- 或使用 `TopLevel.GetTopLevel()` 获取顶级元素后进行布局操作

### 问题 4: Popup.Host 属性被移除
**症状**: `Popup.Host` 属性不存在

**解决方案**:
- 改用 `Popup.IsOpen` 属性检查 Popup 是否已打开
- 示例: `parent?.Popup?.Host != null` → `parent?.Popup?.IsOpen == true`

### 问题 5: IsMotionAwareOpen 和 MotionAwareCloseAsync 方法不存在
**症状**: 自定义扩展方法在 Avalonia 12 中不可用

**解决方案**:
- 改用 `Popup.IsOpen` 属性
- 示例: 
  ```csharp
  // 旧代码
  if (navMenuItem._popup != null && navMenuItem._popup.IsMotionAwareOpen)
  {
      await navMenuItem._popup.MotionAwareCloseAsync();
  }
  
  // 新代码
  if (navMenuItem._popup != null && navMenuItem._popup.IsOpen)
  {
      navMenuItem._popup.IsOpen = false;
      await Task.Delay(50);
  }
  ```

### 问题 6: UpdateSelection 方法被标记为过时
**症状**: 编译警告 "Call UpdateSelectionFromEvent instead"

**解决方案**:
- 改用 `SetCurrentValue(IsSelectedProperty, isSelected)` 直接设置属性
- 示例:
  ```csharp
  // 旧代码
  UpdateSelection(child, isSelected);
  
  // 新代码
  child.SetCurrentValue(IsSelectedProperty, isSelected);
  ```

### 迁移建议
1. 参考 Avalonia 官方的 `Menu.cs` 和 `DefaultMenuInteractionHandler.cs` 实现
2. 使用 `TopLevel.GetTopLevel(visual)` 替代所有 `VisualRoot` 和 `IRenderRoot` 的使用
3. 检查所有焦点事件处理器，确保使用正确的事件参数类型
4. 测试 Popup 的打开/关闭行为，确保动画和交互正常工作
