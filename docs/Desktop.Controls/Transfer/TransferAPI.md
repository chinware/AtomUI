# Transfer API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### TransferDirection

穿梭方向枚举。

| 值 | 说明 |
|---|---|
| `ToSource` | 从目标转移到源 |
| `ToTarget` | 从源转移到目标 |

### TransferViewType

视图类型枚举，标识面板角色。

| 值 | 说明 |
|---|---|
| `Source` | 源数据面板 |
| `Target` | 目标数据面板 |

### TransferSelectAction

批量选择操作枚举。

| 值 | 说明 |
|---|---|
| `SelectAll` | 全选所有 |
| `DeselectAll` | 取消全选 |
| `SelectCurrentPage` | 选择当页（分页模式） |
| `InvertSelectCurrentPage` | 反选当页（分页模式） |
| `RemoveCurrentPage` | 删除当页（单向模式 + 分页模式） |
| `RemoveAll` | 删除所有（单向模式） |

---

## AbstractTransfer 公共属性（基类）

以下属性在 `AbstractTransfer` 抽象基类上定义，被 `ListTransfer` 和 `TreeTransfer` 共同继承。

### 数据源相关

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ItemsSource` | `IEnumerable<IItemKey>?` | `null` | 全量数据源，每项需实现 `IItemKey` 接口 |
| `TargetKeys` | `IList<EntityKey>?` | `null` | 目标面板的键集合，驱动穿梭状态 |
| `SelectedKeys` | `IList<EntityKey>?` | `null` | 当前选中项的键集合 |
| `ItemTemplate` | `IDataTemplate?` | 默认文本模板 | 列表项渲染模板 |

### 布局相关

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ListWidth` | `double` | `double.NaN`（由 Token 控制） | 面板宽度 |
| `ListHeight` | `double` | `double.NaN`（由 Token 控制） | 面板内容区域高度 |
| `IsStretchView` | `bool` | `false` | 是否拉伸面板填满可用空间 |

### 面板标题

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SourceTitle` | `object?` | `null` | 源面板标题 |
| `SourceTitleTemplate` | `IDataTemplate?` | `null` | 源面板标题模板 |
| `TargetTitle` | `object?` | `null` | 目标面板标题 |
| `TargetTitleTemplate` | `IDataTemplate?` | `null` | 目标面板标题模板 |

### 面板页脚

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SourceViewFooter` | `object?` | `null` | 源面板页脚内容 |
| `SourceViewFooterTemplate` | `IDataTemplate?` | `null` | 源面板页脚模板 |
| `TargetViewFooter` | `object?` | `null` | 目标面板页脚内容 |
| `TargetViewFooterTemplate` | `IDataTemplate?` | `null` | 目标面板页脚模板 |

### 操作按钮

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ToSourceTransferIcon` | `PathIcon?` | `LeftOutlined` | "转移到源"按钮图标 |
| `ToTargetTransferIcon` | `PathIcon?` | `RightOutlined` | "转移到目标"按钮图标 |
| `ToSourceButtonText` | `string?` | `null` | "转移到源"按钮文本 |
| `ToTargetButtonText` | `string?` | `null` | "转移到目标"按钮文本 |
| `SelectionsIcon` | `IIconTemplate?` | `DownOutlined` | 选择操作下拉菜单图标 |

### 搜索过滤

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsFilterEnabled` | `bool` | `false` | 是否启用搜索过滤 |
| `Filter` | `IValueFilter?` | 自动创建 `Contains` 过滤器 | 自定义过滤逻辑接口 |
| `FilterValueSelector` | `DefaultFilterValueSelector?` | `null` | 从数据项提取过滤值的委托 |
| `FilterPlaceholderText` | `string?` | `null` | 搜索框占位文本 |

### 行为控制

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsOneWay` | `bool` | `false` | 是否为单向穿梭模式 |
| `IsShowSearch` | `bool` | `false` | 是否显示搜索按钮 |
| `IsShowSelectAll` | `bool` | `false` | 是否显示全选按钮 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |

### 共享接口属性

| 属性名 | 类型 | 默认值 | 来源接口 | 说明 |
|---|---|---|---|---|
| `Status` | `InputControlStatus` | `Default` | `IInputControlStatusAware` | 验证状态（Error/Warning） |
| `SizeType` | `SizeType` | `Middle` | `ISizeTypeAware` | 尺寸类型 |

---

## ListTransfer 额外公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `PageSize` | `int` | `0` | 分页大小，> 0 时启用分页 |
| `SourceView` | `ITransferView?` | `TransferListView` | 自定义源面板视图 |
| `TargetView` | `ITransferView?` | `TransferListView` | 自定义目标面板视图 |

---

## TreeTransfer 额外公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SourceView` | `ITransferTreeView?` | `TransferTreeView` | 自定义源面板树视图 |

---

## 公共事件

| 事件名 | 事件参数类型 | 说明 |
|---|---|---|
| `SelectionChanged` | `TransferSelectionChangedEventArgs` | 穿梭操作导致面板数据变更时触发 |

### TransferSelectionChangedEventArgs

```csharp
public class TransferSelectionChangedEventArgs : EventArgs
{
    public IList<EntityKey>? SourceSelectedKeys { get; }
    public IList<EntityKey>? TargetSelectedKeys { get; }
}
```

---

## 接口

### ITransferView

穿梭视图的通用接口，所有面板视图必须实现。

```csharp
public interface ITransferView
{
    IList<EntityKey>? SelectedKeys { get; set; }
    int ItemCount { get; }
    bool IsSupportItemTemplate { get; }
    bool IsSupportPagination { get; }
    TransferViewType ViewType { get; set; }

    event EventHandler<TransferItemsRemovedEventArgs>? ItemsRemoved;
    event EventHandler<ItemCountChangedEventArgs>? ItemCountChanged;
    event EventHandler<SelectionCountChangedEventArgs>? SelectionCountChanged;
    event EventHandler? SelectedKeyChanged;

    void SelectAll();
    void DeselectAll();
    void NotifyAboutToTransfer(TransferDirection transferDirection);
    void NotifyTransferCompleted(TransferDirection transferDirection);
    void NotifySelectAction(TransferSelectAction selectAction);
    void NotifyIsOneWay(bool isOneWay);
    void SetSelectionEnabled(bool enabled);
    void SetSelectionsIcon(PathIcon? icon);
    void SetItemsSource(IEnumerable? itemsSource);
    void SetItemTemplate(IDataTemplate? itemTemplate);
    void SetPaginationEnabled(bool enabled);
    void SetPageSize(int pageSize);
}
```

### ITransferTreeView

树视图专用接口，扩展 `ITransferView`。

```csharp
public interface ITransferTreeView : ITransferView
{
    void SetMaskedItems(IList<EntityKey>? maskedItems);
}
```

### ITourStepOption / TourStepOption

`TourStepOption` 是步骤选项的 POCO 实现，用于数据绑定场景。

---

## 数据项接口

Transfer 的数据项需要实现 `IItemKey` 接口，其 `ItemKey` 属性（`EntityKey` 类型）用于标识唯一项。对于列表穿梭，数据通常实现 `IListItemData` 接口；对于树穿梭，数据需实现 `ITreeItemNode` 接口。
