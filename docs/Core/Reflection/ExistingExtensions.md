# 现有反射扩展清单

> 本文档列出 AtomUI 项目中所有反射扩展类及其 Hook 的 Avalonia 内部成员。  
> 升级 Avalonia 版本时，请逐一对照此清单验证兼容性。  
> 最后更新：2026-04-11（基于 Avalonia v11.3.x）

---

## AtomUI.Core

### AnimatableReflectionExtensions

| 文件 | `src/AtomUI.Core/Animations/AnimatableReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Animations` |
| 目标类型 | `Avalonia.Animation.Animatable` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `EnableTransitions` | Method | NonPublicMethods | `EnableTransitions()` |
| `DisableTransitions` | Method | NonPublicMethods | `DisableTransitions()` |

---

### DynamicResourceReflectionExtension

| 文件 | `src/AtomUI.Core/Data/DynamicResourceReflectionExtension.cs` |
|---|---|
| 命名空间 | `AtomUI.Data` |
| 目标类型 | `Avalonia.Markup.Xaml.MarkupExtensions.DynamicResourceExtension` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `_anchor` | Field | NonPublicFields | `SetAnchor(object?)` |

---

### StyledElementReflectionExtensions

| 文件 | `src/AtomUI.Core/Reflection/StyledElementReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Reflection` |
| 目标类型 | `Avalonia.Controls.StyledElement` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `LogicalChildren` | Property | NonPublicProperties | `GetLogicalChildrenList()`, `AddToLogicalChildren()`, `InsertToLogicalChildren()` |
| `TemplatedParent` | Property | NonPublicProperties | `SetTemplatedParent()`, `SetTemplatedParentRecursive()` |

---

### VisualReflectionExtensions

| 文件 | `src/AtomUI.Core/Controls/VisualReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Controls` |
| 目标类型 | `Avalonia.Visual` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `SetVisualParent` | Method | NonPublicMethods | `SetVisualParent()`, `ClearVisualParentRecursive()` |
| `VisualChildren` | Property | NonPublicProperties | `GetVisualChildrenList()`, `IndexOfVisualChildren()`, `AddToVisualChildren()`, `InsertToVisualChildren()` |

---

### ILayoutRootReflectionExtensions

| 文件 | `src/AtomUI.Core/Controls/ILayoutRootReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Controls` |
| 目标类型 | `Avalonia.Layout.ILayoutRoot` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `LayoutManager` | Property | NonPublicProperties | `GetLayoutManager()` |

---

### RawPointerEventTypeReflectionExtensions

| 文件 | `src/AtomUI.Core/Controls/RawPointerEventTypeReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Controls` |
| 目标类型 | `Avalonia.Input.Raw.RawPointerEventArgs` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `InputHitTestResult` | Property | NonPublicProperties | `GetInputHitTestResult()` |

---

### ItemCollectionReflectionExtensions

| 文件 | `src/AtomUI.Core/Controls/ItemCollectionReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Controls` |
| 目标类型 | `Avalonia.Controls.ItemCollection` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `SetItemsSource` | Method | NonPublicMethods | `SetItemsSource(IEnumerable?)` |

---

### ManagedPopupPositionerReflectionExtensions

| 文件 | `src/AtomUI.Core/Utils/ManagedPopupPositionerReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Utils` |
| 目标类型 | `Avalonia.Controls.Primitives.PopupPositioning.ManagedPopupPositioner` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `_popup` | Field | NonPublicFields | `GetManagedPopupPositionerPopup()` |

---

### AvaloniaPropertyReflectionExtensions

| 文件 | `src/AtomUI.Core/Utils/AvaloniaPropertyReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Utils` |
| 目标类型 | `Avalonia.AvaloniaProperty`（动态类型） |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `Notifying` | Property | NonPublicProperties | `InvokeNotifying(AvaloniaObject, bool)` |

> ⚠️ **注意**：此类未使用标准的 `Lazy<T>` + `[DynamicDependency]` 模式，因为目标类型是运行时动态确定的（`property.GetType()` 返回具体的泛型闭合类型）。这是一个特殊情况。

---

### TransformTrackingHelper

| 文件 | `src/AtomUI.Core/Input/TransformTrackingHelper.cs` |
|---|---|
| 命名空间 | `AtomUI.Input` |
| 目标类型 | `Avalonia.Threading.DispatcherPriority` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法/使用方式 |
|---|---|---|---|
| `AfterRender` | Field (static) | NonPublicFields | 在 `EnqueueForUpdate()` 中获取调度优先级 |

> **注意**：此类不是标准的 `ReflectionExtensions` 扩展类，而是一个独立的辅助类，内部使用了反射来访问 `DispatcherPriority.AfterRender` 静态私有字段。

---

### TextParagraphPropertiesReflectionExtensions

| 文件 | `src/AtomUI.Core/Media/TextFormatting/TextParagraphPropertiesReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Media.TextFormatting` |
| 目标类型 | `Avalonia.Media.TextFormatting.TextParagraphProperties` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `LineSpacing` | Property | NonPublicProperties | `GetLineSpacing()`, `SetLineSpacing(double)` |

---

## AtomUI.Desktop.Controls

### TextBlockReflectionExtensions

| 文件 | `src/AtomUI.Desktop.Controls/TextBlock/TextBlockReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Desktop.Controls` |
| 目标类型 | `Avalonia.Controls.TextBlock` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `GetMaxSizeFromConstraint` | Method | NonPublicMethods | `GetMaxSizeFromConstraint()` |
| `HasComplexContent` | Property | NonPublicProperties | `GetHasComplexContent()` |

---

### ScrollContentPresenterReflectionExtensions

| 文件 | `src/AtomUI.Desktop.Controls/TabControl/ScrollContentPresenterReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Desktop.Controls` |
| 目标类型 | `Avalonia.Controls.Presenters.ScrollContentPresenter` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `SnapOffset` | Method | NonPublicMethods | `SnapOffset(Vector, Vector, bool)` |

---

### SelectingItemsControlReflectionExtensions

| 文件 | `src/AtomUI.Desktop.Controls/Primitives/SelectingItemsControlReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Desktop.Controls.Primitives` |
| 目标类型 | `Avalonia.Controls.Primitives.SelectingItemsControl` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `MarkContainerSelected` | Method | NonPublicMethods | `MarkContainerSelected(Control, bool)` |
| `InitializeSelectionModel` | Method | NonPublicMethods | `InitializeSelectionModel(ISelectionModel)` |
| `_selection` | Field | NonPublicFields | `SetSelection(ISelectionModel?)`, `GetSelection()` |

---

### SelectionModelReflectionExtensions

| 文件 | `src/AtomUI.Desktop.Controls/Primitives/SelectionModelReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Desktop.Controls.Primitives` |
| 目标类型 | `Avalonia.Controls.Selection.SelectionModel<>` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `SetInitSelectedItems` | Method | NonPublicMethods | `SetInitSelectedItems<T>(IList)` |
| `SetSource` | Method | NonPublicMethods | `SetSource<T>(IEnumerable?)` |

> **注意**：泛型目标类型，使用 `ConcurrentDictionary<Type, MethodInfo>` 缓存。

---

### MenuReflectionExtensions

| 文件 | `src/AtomUI.Desktop.Controls/Menu/MenuReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Desktop.Controls` |
| 目标类型 | `Avalonia.Controls.ContextMenu` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `_popup` | Field | NonPublicFields | `SetPopup(Popup)` |
| 动态方法名 | Method | NonPublicMethods | `CreateEventHandler<T>(string)`, `CreateEventHandler(string)` |

---

### ComboBoxReflectionExtensions

| 文件 | `src/AtomUI.Desktop.Controls/ComboBox/ComboBoxReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Desktop.Controls` |
| 目标类型 | `Avalonia.Controls.ComboBox` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `_popup` | Field | NonPublicFields | `SetPopup(Popup?)` |

---

### WindowBaseReflectionExtensions

| 文件 | `src/AtomUI.Desktop.Controls/Window/WindowBaseReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Desktop.Controls` |
| 目标类型 | `Avalonia.Controls.WindowBase` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `FreezeVisibilityChangeHandling` | Method | NonPublicMethods | `FreezeVisibilityChangeHandling()` (private) |
| `EnsureInitialized` | Method | NonPublicMethods | `EnsureInitialized()` (private) |
| `_hasExecutedInitialLayoutPass` | Field | NonPublicFields | `HasExecutedInitialLayoutPass()`, `SetHasExecutedInitialLayoutPass()` (private) |
| `StartRendering` | Method | NonPublicMethods | `StartRendering()` (private) |
| `OnOpened` | Method | NonPublicMethods | `OnOpened(EventArgs)` (private) |

> **注意**：此类展示了入口方法 `ShowWithoutActive()` 为 `public`，内部子步骤为 `private` 的组合模式。

---

### PopupReflectionExtensions

| 文件 | `src/AtomUI.Desktop.Controls/Popup/PopupReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Desktop.Controls` |
| 目标类型 | `AtomUI.Desktop.Controls.Popup` / `Avalonia.Controls.Primitives.Popup` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `SetPopupParent` | Method | NonPublicMethods | `SetPopupParent(Control?)` |
| `UpdateHostPosition` | Method | NonPublicMethods | `UpdateHostPosition(IPopupHost, Control)` |
| `Closing` | Event | NonPublicEvents | `AddClosingEventHandler()`, `RemoveClosingEventHandler()` |
| `_ignoreIsOpenChanged` | Field | NonPublicFields | `SetIgnoreIsOpenChanged(bool)`, `GetIgnoreIsOpenChanged()` |

---

### TextBoxReflectionExtensions

| 文件 | `src/AtomUI.Desktop.Controls/Input/Utils/TextBoxReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Desktop.Controls.Utils` |
| 目标类型 | `Avalonia.Controls.TextBox` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `_scrollViewer` | Field | NonPublicFields | `GetScrollViewer()`, `SetScrollViewer()` |
| `_presenter` | Field | NonPublicFields | `GetTextPresenter()` |
| `ScrollViewer_ScrollChanged` | Method | NonPublicMethods | `HandleScrollChanged()` |
| `GetVerticalSpaceBetweenScrollViewerAndPresenter` | Method | NonPublicMethods | `GetVerticalSpaceBetweenScrollViewerAndPresenter()` |
| `SnapshotUndoRedo` | Method | NonPublicMethods | `SnapshotUndoRedo()` |
| `HandleTextInput` | Method | NonPublicMethods | `HandleTextInput(string?)` |

---

### TextLayoutReflectionExtensions

| 文件 | `src/AtomUI.Desktop.Controls/Input/Utils/TextLayoutReflectionExtensions.cs` |
|---|---|
| 命名空间 | `AtomUI.Desktop.Controls.Utils` |
| 目标类型 | `Avalonia.Media.TextFormatting.TextLayout` |

| Hook 成员 | 类型 | 成员类型 | 扩展方法 |
|---|---|---|---|
| `CreateTextParagraphProperties` | Method (static) | NonPublicMethods | `CreateTextParagraphProperties(...)` |

---

## 统计摘要

| 项目 | 扩展类数量 | Hook 成员总数 |
|---|---|---|
| AtomUI.Core | 10 | 17 |
| AtomUI.Desktop.Controls | 9 | 24 |
| **合计** | **19** | **41** |

